using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace eleWP8
{
    [DataContract]
    public class SingleResponsePackage
    {
        [DataMember]
        public string body { get; set; }
        [DataMember]
        public int code { get; set; }
    }

    [DataContract]
    public class UserProfile
    {
        [DataMember]
        public int current_address_id { get; set; }
        [DataMember]
        public int balance { get; set; }
        [DataMember]
        public int point { get; set; }
        [DataMember]
        public string email { get; set; }
        [DataMember]
        public string mobile { get; set; }
        [DataMember]
        public string username { get; set; }
        [DataMember]
        public int user_id { get; set; }
        [DataMember]
        public int payment_quota { get; set; }
        [DataMember]
        public string referal_code { get; set; }
    }

    [DataContract]
    public class Order
    {
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public int restaurant_id { get; set; }
        [DataMember]
        public string restaurant_name { get; set; }
        [DataMember]
        public int user_id { get; set; }
        [DataMember]
        public string user_name { get; set; }
        [DataMember]
        public double total { get; set; }
        [DataMember]
        public double deliver_fee { get; set; }
        [DataMember]
        public string created_at { get; set; }
        [DataMember]
        public string address { get; set; }
        [DataMember]
        public string phone { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public string unique_id { get; set; }
        [DataMember]
        public string consignee { get; set; }
        [DataMember]
        public string pay_method { get; set; }
        [DataMember]
        public Basket basket { get; set; }
        [DataMember]
        public RestaurantBlock restaurant { get; set; }
    }

    [DataContract]
    public class Basket
    {
        [DataMember]
        public string phone { get; set; }
        [DataMember]
        public List<List<BasketItem>> group { get; set; }
        [DataMember]
        public string come_from { get; set; } //web
        [DataMember]
        public List<ExtraItem> extra { get; set; }
        [DataMember]
        public string cart_id { get; set; }

        public string Abstract
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (var rlist in group)
                {
                    foreach (var item in rlist)
                    {
                        sb.AppendFormat("{0}x{1};", item.name, item.quantity);
                        if (sb.Length >= 50) break;
                    }
                    if (sb.Length >= 50) break;
                }
                return sb.ToString();
            }
        }

        public Basket()
        {
            group = new List<List<BasketItem>>();
            extra = new List<ExtraItem>();
        }
    }

    [DataContract]
    public class BasketItem
    {
        [DataMember]
        public int category_id { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public double price { get; set; }
        [DataMember]
        public int id { get; set;}
        [DataMember]
        public int quantity { get; set; }
        [DataMember]
        public string order_item_id { get; set; }
    }

    [DataContract]
    public class ExtraItem
    {
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public double price { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public int category_id { get; set; }
        [DataMember]
        public int id { get; set; }
        [DataMember]
        public int quantity { get; set; }
    }

    [DataContract]
    public class LoginResult
    {
        [DataMember]
        public int user_id { get; set; }
    }

    [DataContract]
    public class CodePicture
    {
        [DataMember]
        public string code { get; set; }
    }

    [DataContract]
    public class LoginPackage
    {
        [DataMember]
        public string username { get; set; }
        [DataMember]
        public string password { get; set; }
        [DataMember]
        public string captcha_code { get; set; }
    }

    [DataContract]
    public class PlaceData
    {
        [DataMember]
        public string name { set; get; }
        [DataMember]
        public string address { set; get; }
        [DataMember]
        public string geohash { set; get; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return geohash.Equals((obj as PlaceData).geohash);
        }

        public override int GetHashCode()
        {
            return geohash.GetHashCode();
        }
    }

    [DataContract]
    public class PlaceSelectionResponse
    {
        [DataMember]
        public string status { set; get; }
        [DataMember]
        public List<PlaceData> data { set; get; }
        public PlaceSelectionResponse()
        {
            data = new List<PlaceData>();
        }
    }

    [DataContract]
    public class RestaurantBlock
    {
        [DataMember]
        public int id { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string name_for_url { get; set; }
        [DataMember]
        public string address { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public int delivery_fee { get; set; }
        [DataMember]
        public int minimum_free_delivery_amount { get; set; }
        [DataMember]
        public int minimum_order_amount { get; set; }
        [DataMember]
        public string restaurant_tips { get; set; }
        [DataMember]
        public string phone { get; set; }
        [DataMember]
        public string promotion_info { get; set; }

        [DataMember]
        public int month_sales { get; set; }
        [DataMember]
        public int distance { get; set; }
        [DataMember]
        public string food_tips { get; set; }
        [DataMember]
        public string minimum_order_description { get; set; }
        [DataMember]
        public double rating { get; set; }
        [DataMember]
        public int rating_count { get; set; }
        [DataMember]
        public int recent_order_num { get; set; }

        [DataMember]
        public bool is_free_delivery { get; set; }
        [DataMember]
        public bool is_in_book_time { get; set; }
        [DataMember]
        public bool is_online_payment { get; set; }
        [DataMember]
        public bool is_open_time { get; set; }
    }

    [DataContract]
    public class RestaurantMenu
    {
        [DataMember]
        public string name { get; set; }
        //public Object activity { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public List<Food> foods { get; set; }

        [DataMember]
        public bool is_activity { get; set; }
        [DataMember]
        public bool is_selected { get; set; }
        [DataMember]
        public bool must_new_user { get; set; }
        [DataMember]
        public bool must_pay_online { get; set; }

        public RestaurantMenu()
        {
            foods = new List<Food>();
        }
    }

    [DataContract]
    public class Food
    {
        //public Object activity { get; set; }
        [DataMember]
        public List<FoodAttribute> attributes { get; set; }
        [DataMember]
        public int category_id { get; set; }
        [DataMember]
        public int id { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public string image_path { get; set; }
        [DataMember]
        public int month_sales { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public int packing_fee { get; set; }
        [DataMember]
        public string pinyin_name { get; set; }
        [DataMember]
        public double price { get; set; }
        [DataMember]
        public double rating { get; set; }
        [DataMember]
        public int rating_count { get; set; }
        [DataMember]
        public int restaurant_id { get; set; }
        [DataMember]
        public int stock { get; set; }
        [DataMember]
        public string tips { get; set; }

        public Food()
        {
            attributes = new List<FoodAttribute>();
        }
    }

    [DataContract]
    public class FoodAttribute
    {
        public string icon_color { get; set; }
        public string icon_name { get; set; }
    }
}
