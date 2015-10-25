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
using Microsoft.Phone.Tasks;

namespace eleWP8.Views
{
    public partial class ProfilePage : PhoneApplicationPage
    {
        private IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;

        int offset = 0;
        int limit = 10;
        int user_id = 0;
        List<Order> orderList;

        public ProfilePage()
        {
            InitializeComponent();
            this.Loaded += ProfilePage_Loaded;
            orderList = new List<Order>();

            var proIndicator = new ProgressIndicator()
            {
                IsIndeterminate = true,
                IsVisible = false,
                Text = "加载用户信息"
            };
            SystemTray.SetProgressIndicator(this, proIndicator);
        }

        private void ProfilePage_Loaded(object sender, RoutedEventArgs e)
        {
            //check user_id
            if (NavigationContext.QueryString.ContainsKey("user_id")){
                if (!Int32.TryParse(NavigationContext.QueryString["user_id"].ToString(), out user_id))
                    user_id = 0;
            }
            if (user_id == 0) user_id = (int)appSettings["user_id"];
            if (user_id == 0 || !(Application.Current as App).IsLogin)
            {
                NavigationService.Navigate(new Uri("/Views/LoginPage.xaml", UriKind.Relative));
                return;
            }

            //query user profile
            SystemTray.ProgressIndicator.Text = "加载用户信息";
            SystemTray.ProgressIndicator.IsVisible = true;
            var task = eleAPI.Instance.GetUserProfile();
            task.ContinueWith(t => {
                if (t.Status == System.Threading.Tasks.TaskStatus.RanToCompletion && t.Result != null)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        UserProfilePanel.DataContext = t.Result;
                        SystemTray.ProgressIndicator.IsVisible = false;
                    });
                }
                else
                {
                    Dispatcher.BeginInvoke(() => {
                        SystemTray.ProgressIndicator.Text = String.Format("Exception: {0}", t.Exception.Message);
                    });                    
                }
            });
        }

        private void LoadOrderBtn_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            SystemTray.ProgressIndicator.Text = "加载历史订单";
            SystemTray.ProgressIndicator.IsVisible = true;
            var task = eleAPI.Instance.GetHistoryOrder(user_id, offset, limit);
            task.ContinueWith(t =>
            {
                Dispatcher.BeginInvoke(() =>
                {
                    offset += t.Result.Count;
                    orderList.AddRange(t.Result);
                    OrderListControl.ItemsSource = null;
                    OrderListControl.ItemsSource = orderList;
                    SystemTray.ProgressIndicator.IsVisible = false;
                });
            });
        }

        private void OrderBtn_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Order order = (sender as Button).DataContext as Order;
            string url = String.Format("{0}/{1}", eleAPI.host, order.restaurant.name_for_url);
            WebBrowserTask webBrowserTask = new WebBrowserTask();
            webBrowserTask.Uri = new Uri(url);
            webBrowserTask.Show();
        }

    }
}