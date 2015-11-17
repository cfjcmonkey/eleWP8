using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;
using System.ComponentModel;

namespace eleWP8.Views
{
    public partial class LoginPage : PhoneApplicationPage
    {
        private IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;

        public LoginPage()
        {
            InitializeComponent();
            this.Loaded += LoginPage_Loaded;

            var proIndicator = new ProgressIndicator()
            {
                IsIndeterminate = true,
                IsVisible = false,
                Text = "加载验证码"
            };
            SystemTray.SetProgressIndicator(this, proIndicator);
        }

        private void LoginPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (appSettings.Contains("username")) Txt_Username.Text = (string)appSettings["username"];
            if (appSettings.Contains("password")) Txt_Password.Password = (string)appSettings["password"];
            GetLoginCodePicture();
        }

        void GetLoginCodePicture()
        {
            SystemTray.ProgressIndicator.IsVisible = true;
            var task = eleAPI.Instance.GetLoginCodePicture();
            task.ContinueWith(t =>
            {
                if (t.Status == TaskStatus.RanToCompletion && t.Result != null)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        BitmapImage image = new BitmapImage();
                        image.SetSource(t.Result);
                        codeImage.Source = image;
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

        private void Login_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            (sender as Button).IsEnabled = false;
            var task = eleAPI.Instance.LoginAccount(Txt_Username.Text, Txt_Password.Password, Txt_code.Text);
            task.ContinueWith(t => {
                if (t.Status == TaskStatus.RanToCompletion && t.Result != null)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        if (appSettings.Contains("username")) appSettings["username"] = Txt_Username.Text;
                        else appSettings.Add("username", Txt_Username.Text);
                        if (appSettings.Contains("password")) appSettings["password"] = Txt_Password.Password;
                        else appSettings.Add("password", Txt_Password.Password);
                        (Application.Current as App).IsLogin = true;

                        Txt_Username.Text = t.Result.ToString();
                        appSettings["user_id"] = t.Result;
                        SystemTray.SetIsVisible(this, false);
                        if (NavigationService.CanGoBack) NavigationService.GoBack();
                        else NavigationService.Navigate(new Uri(String.Format(@"/Views/ProfilePage.xaml?user_id={0}", t.Result), UriKind.Relative));
                    });
                }
                else
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        NotifyTbl.Text = "登陆失败!请检查用户名，密码和验证码.";
                    });
                    GetLoginCodePicture();
                }
            });
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            if (NavigationService.BackStack.First().Source.ToString() == "/Views/ProfilePage.xaml")
                NavigationService.RemoveBackEntry();
            base.OnBackKeyPress(e);
        }
    }
}