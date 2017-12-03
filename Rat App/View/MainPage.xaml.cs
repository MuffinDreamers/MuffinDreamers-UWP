using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using System.Diagnostics;
using Rat_App.View;
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Rat_App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            switch(RatApp.currentUser.Metadata.Role)
            {
                case User.Roles.Admin:
                    buttonAdmin.Visibility = Visibility.Visible;
                    break;
                case User.Roles.User:
                default:
                    buttonAdmin.Visibility = Visibility.Collapsed;
                    break;
            }
            PopulateListView();
        }

        private async void PopulateListView()
        {
            buttonGraph.IsEnabled = false;
            buttonMap.IsEnabled = false;

            progressRingSightings.IsActive = true;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://muffindreamers.azurewebsites.net/sightings");
            request.ContentType = "application/json";

            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            Sighting[] sightings;
            using(StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                sightings = Sighting.ArrayFromJson(reader.ReadToEnd());
            }

            progressRingSightings.IsActive = false;

            foreach(Sighting s in sightings)
            {
                listViewSightings.Items.Add(s);
            }

            buttonGraph.IsEnabled = true;
            buttonMap.IsEnabled = true;
        }

        private async void buttonLogout_Click(object sender, RoutedEventArgs e)
        {
            bool success = await RatApp.GetInstance().LogoutAsync();
            if(success)
                Frame.Navigate(typeof(WelcomePage));
        }

        private void buttonAddNew_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddSightingPage));
        }

        private void buttonGraph_Click(object sender, RoutedEventArgs e)
        {
            Sighting[] sightings = new Sighting[listViewSightings.Items.Count()];

            for (int i = 0; i < sightings.Length; i++)
            {
                sightings[i] = (Sighting)listViewSightings.Items[i];
            }

            Frame.Navigate(typeof(GraphPage), sightings);
        }

        private void buttonMap_Click(object sender, RoutedEventArgs e)
        {
            Sighting[] sightings = new Sighting[listViewSightings.Items.Count()];

            for(int i = 0; i < sightings.Length; i++)
            {
                sightings[i] = (Sighting)listViewSightings.Items[i];
            }

            Frame.Navigate(typeof(MapPage), sightings);
        }

        private void listViewSightings_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sighting sighting = (Sighting)e.ClickedItem;

            Frame.Navigate(typeof(SightingPage), sighting);
        }

        private void buttonAdmin_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AdminPage));
        }
    }
}
