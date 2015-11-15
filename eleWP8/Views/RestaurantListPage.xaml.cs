using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.ComponentModel;
using Microsoft.Phone.Tasks;

namespace eleWP8.Views
{
    public partial class RestaurantListPage : PhoneApplicationPage
    {
        private IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;

        public string geohash { get; set; }
        public int offset { get; set; }
        public int limit { get; set; }
        List<RestaurantBlock> restaurantList { get; set; }

        public RestaurantListPage()
        {
            InitializeComponent();
            this.Loaded += RestaurantListPage_Loaded;
            restaurantList = new List<RestaurantBlock>();
            var proIndicator = new ProgressIndicator()
            {
                IsIndeterminate = true,
                IsVisible = false,
                Text = "加载餐厅列表"
            };
            SystemTray.SetProgressIndicator(this, proIndicator);
        }

        private void RestaurantListPage_Loaded(object sender, RoutedEventArgs e)
        {
            //check geohash
            if (NavigationContext.QueryString.ContainsKey("geohash")) geohash = NavigationContext.QueryString["geohash"].ToString();
            else geohash = (string)appSettings["geohash"];
            if (String.IsNullOrEmpty(geohash))
                NavigationService.Navigate(new Uri("/Views/PlaceSelectionPage.xaml", UriKind.Relative));

            offset = 0;
            limit = 10;

            UpdateMoreRestaurant();
        }

        private void UpdateMoreRestaurant()
        {
            SystemTray.ProgressIndicator.IsVisible = true;
            var task = eleAPI.Instance.GetResturantList(geohash, offset, limit);
            task.ContinueWith(t =>
            {
                Dispatcher.BeginInvoke(() =>
                {
                    restaurantList.AddRange(t.Result);
                    ResultListControl.ItemsSource = null;
                    ResultListControl.ItemsSource = restaurantList;
                    offset += t.Result.Count;
                    SystemTray.ProgressIndicator.IsVisible = false;
                });
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RestaurantBlock rest = (sender as Button).DataContext as RestaurantBlock;
            string url = String.Format("{0}/{1}", eleAPI.host, rest.name_for_url);
            WebBrowserTask webBrowserTask = new WebBrowserTask();
            webBrowserTask.Uri = new Uri(url);
            webBrowserTask.Show();
        }

        private void refreshPanel_RefreshRequested(object sender, EventArgs e)
        {
            UpdateMoreRestaurant();
        }
    }
}