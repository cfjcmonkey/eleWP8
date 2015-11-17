using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace eleWP8.Models
{
    public class FoodEntity
    {
        public Food food { get; set; }
        public int quantity { get; set; }
    };

    public class CartManagement : INotifyPropertyChanged
    {
        private string cart_id = "";
        private string sig = "";
        private RestaurantBlock restaurant = null;
        private double _totalPrice = 0;
        private int _backetNum = 0;
        static private CartManagement _instance = null;
        public ObservableCollection<FoodEntity> foodList { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        static public CartManagement Instance {
            get {
                if (_instance == null) _instance = new CartManagement();
                return _instance;
            }
        }

        public bool CanDeliver {
            get {
                if (restaurant == null) return false;
                return TotalPrice >= restaurant.minimum_order_amount && TotalPrice >= restaurant.minimum_free_delivery_amount;
            }
        }
        public double TotalPrice {
            get { return _totalPrice; }
            set {
                if (_totalPrice != value) {
                    _totalPrice = value;
                    OnPropertyChanged();
                }
                _totalPrice = value;                    
            }
        }
        public int BacketNum {
            get { return _backetNum; }
            set {
                if (_backetNum != value)
                {
                    _backetNum = value;
                    OnPropertyChanged();
                }
                _backetNum = value;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string info = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private CartManagement() { foodList = new ObservableCollection<FoodEntity>(); }

        public ObservableCollection<FoodEntity> GetFoodList() { return foodList; }

        public bool AddFood(RestaurantBlock restInfo, Food food, int quantity = 1)
        {
            if (restaurant != restInfo && foodList.Count > 0) return false;
            restaurant = restInfo; //use copy to instead
            int idx;
            for (idx = 0; idx < foodList.Count; idx++)
            {
                if (foodList[idx].food == food)
                {
                    var entity = foodList[idx];
                    entity.quantity += quantity;
                    break;
                }
            }
            if (idx >= foodList.Count)
            {
                foodList.Add(new FoodEntity() { food = food, quantity = quantity });
            }
            UpdatePrice();
            return true;
        }

        public bool RemoveFood(RestaurantBlock restInfo, Food food, int quantity = 1)
        {
            if (restaurant != restInfo) return false;
            for (int idx = 0; idx < foodList.Count; idx++)
            {
                if (foodList[idx].food == food)
                {
                    var entity = foodList[idx];
                    if (entity.quantity > quantity)
                    {
                        entity.quantity -= quantity;
                    }
                    else foodList.RemoveAt(idx);
                    UpdatePrice();
                    return true;
                }
            }
            return false;
        }

        public void Clear()
        {
            foodList.Clear();
            UpdatePrice();
        }

        public void UpdatePrice()
        {
            if (foodList.Count == 0 || restaurant == null)
            {
                TotalPrice = 0;
                BacketNum = 0;
                return;
            }

            double price = restaurant.delivery_fee;
            int num = 0;
            foreach (var entity in foodList)
            {
                price += entity.food.price * entity.quantity;
                num += entity.quantity;
            }
            TotalPrice = price;
            BacketNum = num;
        }

        public async Task<double> SyncCart()
        {
            List<CartEntity> cartEntityList = new List<CartEntity>();
            foreach (var entity in foodList)
            {
                cartEntityList.Add(new CartEntity() { id = entity.food.id.ToString(), quantity = entity.quantity });
            }

            if (String.IsNullOrEmpty(cart_id) || String.IsNullOrEmpty(sig))
            {
                var result = await eleAPI.Instance.GetCartInfo();
                cart_id = result.cart.id;
                sig = result.sig;
            }

            var result2 = await eleAPI.Instance.UpdateCart(cart_id, sig, cartEntityList);
            return result2.total;
        }
    }
}
