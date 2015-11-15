using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Net.NetworkInformation;

namespace eleWP8.Views
{
    public partial class PlaceSelectionPage : PhoneApplicationPage
    {
        private IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;
        public static IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
        HashSet<PlaceData> placeHistorySet;
        string placeHistoryFilename = "placeHistory.txt";

        public PlaceSelectionPage()
        {
            InitializeComponent();
            this.Loaded += PlaceSelectionPage_Loaded;

            var proIndicator = new ProgressIndicator()
            {
                IsIndeterminate = true,
                IsVisible = false,
                Text = "无网络连接，请检查网络！"
            };
            SystemTray.SetProgressIndicator(this, proIndicator);
            if (DeviceNetworkInformation.IsNetworkAvailable == false)
                proIndicator.IsVisible = true;
        }

        private void PlaceSelectionPage_Loaded(object sender, RoutedEventArgs e)
        {
            HistoryListBox.SelectionChanged += PlaceSelectionChanged;
            SearchResultListBox.SelectionChanged += PlaceSelectionChanged;

            placeHistorySet = new HashSet<PlaceData>();
            if (store.FileExists(placeHistoryFilename))
            {
                using(StreamReader reader = new StreamReader(store.OpenFile(placeHistoryFilename, FileMode.Open)))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] items = line.Split('\t');
                        placeHistorySet.Add(new PlaceData() { name = items[0], address = items[1], geohash = items[2] });
                    }
                }
            }

            HistoryListBox.ItemsSource = placeHistorySet.ToList();
        }

        private void PlaceSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PlaceData place = (sender as ListBox).SelectedItem as PlaceData;
            if (place == null) return;
            if (placeHistorySet.Add(place))
            {
                using (StreamWriter writer = new StreamWriter(store.OpenFile(placeHistoryFilename, FileMode.Append)))
                {
                    writer.Write(String.Format("{0}\t{1}\t{2}\n", place.name, place.address, place.geohash));
                }
            }
            appSettings["geohash"] = place.geohash;
            NavigationService.Navigate(new Uri(String.Format(@"/Views/RestaurantListPage.xaml?geohash={0}", place.geohash), UriKind.Relative));
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            var task = eleAPI.Instance.SearchPlace(SearchBox.Text);
            task.ContinueWith(t =>
            {
                Dispatcher.BeginInvoke(() =>
                {
                    SearchResultListBox.ItemsSource = t.Result.data;
                });
            });
        }

        private void ApplicationBarLoginBtn_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/LoginPage.xaml", UriKind.Relative));
        }

        private void ApplicationBarUserProfileBtn_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/ProfilePage.xaml", UriKind.Relative));
        }
    }
}