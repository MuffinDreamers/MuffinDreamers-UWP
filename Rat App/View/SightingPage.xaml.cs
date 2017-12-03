using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Rat_App.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SightingPage : Page
    {
        private Sighting sighting;

        public SightingPage()
        {
            this.InitializeComponent();
            
            if(sighting != null)
            {
                UpdateTextBlocks();
            }
        }

        private void UpdateTextBlocks()
        {
            textId.Text = Resources["idPreText"].ToString() + sighting.Id.ToString();
            textLocationType.Text = Resources["locationTypePreText"].ToString() + sighting.LocationType;
            textBorough.Text = Resources["boroughPreText"].ToString() + sighting.Borough;
            textZipcode.Text = Resources["zipcodePreText"].ToString() + sighting.Zipcode.ToString();
            textStreetAddress.Text = Resources["streetAddressPreText"].ToString() + sighting.StreetAddress;
            textCity.Text = Resources["cityPreText"].ToString() + sighting.City;
            textLatitude.Text = Resources["latitudePreText"].ToString() + sighting.Latitude.ToString();
            textLongitude.Text = Resources["longitudePreText"].ToString() + sighting.Longitude.ToString();
            textDateCreated.Text = Resources["dateCreatedPreText"].ToString() + sighting.DateCreated.ToString();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            sighting = (Sighting)e.Parameter;

            if (sighting != null)
            {
                UpdateTextBlocks();
            }
        }

        private void buttonBack_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
