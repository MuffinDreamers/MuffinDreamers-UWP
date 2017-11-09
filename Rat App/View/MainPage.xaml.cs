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
            PopulateListView();
        }

        private async void PopulateListView()
        {
            progressRingSightings.IsActive = true;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://muffindreamers.azurewebsites.net/sightings");
            request.ContentType = "application/json";

            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            Sighting[] sightings;
            using(StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                sightings = Sighting.FromJson(reader.ReadToEnd());
            }

            progressRingSightings.IsActive = false;

            foreach(Sighting s in sightings)
            {
                listViewSightings.Items.Add(s);
            }
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
            //TODO Graph page
        }

        private void buttonMap_Click(object sender, RoutedEventArgs e)
        {
            //TODO Map page
        }

        private void listViewSightings_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sighting sighting = (Sighting)e.ClickedItem;

            Frame.Navigate(typeof(SightingPage), sighting);
        }
    }
}
