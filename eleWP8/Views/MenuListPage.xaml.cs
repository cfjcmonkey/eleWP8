using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using eleWP8.Models;

namespace eleWP8.Views
{
    public partial class MenuListPage : PhoneApplicationPage
    {
        public RestaurantBlock restInfo { get; set; }

        public MenuListPage()
        {
            InitializeComponent();
            this.Loaded += MenuListPage_Loaded;

            var proIndicator = new ProgressIndicator()
            {
                IsIndeterminate = true,
                IsVisible = false,
                Text = "加载菜单列表"
            };
            SystemTray.SetProgressIndicator(this, proIndicator);
        }

        private void MenuListPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (restInfo == null)
            {
                MessageBox.Show("Error: not find restaurant name.");
                return;
            }
            MenuPivot.Title = restInfo.name;
            StartPriceTbl.Text = restInfo.minimum_order_amount.ToString();
            DeliverFeeTbl.Text = restInfo.delivery_fee.ToString();
            TipsTbl.Text = restInfo.restaurant_tips;
            CartSimplePanel.DataContext = null;
            CartSimplePanel.DataContext = CartManagement.Instance;

            SystemTray.ProgressIndicator.IsVisible = true;
            var task = eleAPI.Instance.GetMenuList(restInfo.name_for_url);
            task.ContinueWith(t =>
            {
                Dispatcher.BeginInvoke(() =>
                {
                    MenuPivot.ItemsSource = t.Result;
                    SystemTray.ProgressIndicator.IsVisible = false;
                });
            });
        }

        private void FoodBtn_Click(object sender, RoutedEventArgs e)
        {
            var food = (sender as Button).DataContext as Food;
            if (CartManagement.Instance.AddFood(restInfo, food, 1))
            {
                MessageBox.Show(String.Format("添加 {0} 到购物车", food.name), "添加成功!", MessageBoxButton.OK);
            }
            else
            {
                var result = MessageBox.Show("是否要清空购物车?", "添加失败!", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    CartManagement.Instance.Clear();
                }
            }
        }

        private void CheckBacketBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/BacketPage.xaml", UriKind.Relative));
        }

        private void ApplicationBarLoginBtn_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/LoginPage.xaml", UriKind.Relative));
        }

        private void ApplicationBarUserProfileBtn_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/ProfilePage.xaml", UriKind.Relative));
        }

        private void ApplicationBarNoteBtn_Click(object sender, EventArgs e)
        {
            MessageBox.Show(restInfo.minimum_order_description);
        }
    }
}