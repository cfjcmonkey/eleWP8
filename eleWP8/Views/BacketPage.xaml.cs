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
    public partial class BacketPage : PhoneApplicationPage
    {
        public BacketPage()
        {
            InitializeComponent();
            this.Loaded += BacketPage_Loaded;

            var proIndicator = new ProgressIndicator()
            {
                IsIndeterminate = true,
                IsVisible = false,
                Text = "同步购物车"
            };
            SystemTray.SetProgressIndicator(this, proIndicator);
        }

        private void BacketPage_Loaded(object sender, RoutedEventArgs e) //init in loaded or constructor?
        {
            FoodListBox.ItemsSource = CartManagement.Instance.GetFoodList();
            PricePanel.DataContext = CartManagement.Instance;
            //TotalPriceTbl.DataContext = CartManagement.Instance.TotalPrice;
        }

        private void SyncOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            SystemTray.ProgressIndicator.IsVisible = true;
            var task = CartManagement.Instance.SyncCart();
            task.ContinueWith(t => {
                if (t.IsCompleted)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        MessageBox.Show(String.Format("同步订单成功，需付款{0}元", t.Result));
                        SystemTray.ProgressIndicator.IsVisible = false;
                    });
                }
                else
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        MessageBox.Show("同步订单失败！");
                        SystemTray.ProgressIndicator.IsVisible = false;
                    });
                }
            });
        }

        private void ClearBacketBtn_Click(object sender, RoutedEventArgs e)
        {
            CartManagement.Instance.Clear();
            //FoodListBox.ItemsSource = null;
            //FoodListBox.ItemsSource = CartManagement.Instance.GetFoodList();
            //TotalPriceTbl.Text = CartManagement.Instance.TotalPrice.ToString();
        }
    }
}