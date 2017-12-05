using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Geolocation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Rat_App.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MapPage : Page
    {
        IEnumerable<Sighting> allSightings;

        public MapPage()
        {
            this.InitializeComponent();
        }

        private void PopulateMap(IEnumerable<Sighting> sightings)
        {
            if (sightings.Count() == 0)
                return;

            MapIcon[] icons = new MapIcon[sightings.Count()];

            mapSightings.MapElements.Clear();//TODO Only remove the pins that don't correspond to an entry in the sightings list

            for (int i = 0; i < sightings.Count(); i++)
            {
                Sighting s = sightings.ElementAt(i);

                if (s.Latitude < -90 || s.Latitude > 90 || s.Longitude < -180 || s.Longitude > 180)
                    continue;
                try
                {
                    BasicGeoposition basicGeoposition = new BasicGeoposition();
                    basicGeoposition.Latitude = s.Latitude;
                    basicGeoposition.Longitude = s.Longitude;
                    Geopoint geopoint = new Geopoint(basicGeoposition);
                    MapIcon mapIcon = new MapIcon()
                    {
                        Location = geopoint,
                        Title = s.ToString()
                    };
                    mapSightings.MapElements.Add(mapIcon);
                } catch (ArgumentException e)
                {
                    //Debug.WriteLine($"Skipped adding sighting {s.Id} to map");
                }
            }
        }

        private void buttonReturn_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            allSightings = (IEnumerable<Sighting>)e.Parameter;

            if (allSightings != null)
                PopulateMap(allSightings);

            GeolocationAccessStatus accessStatus = await Geolocator.RequestAccessAsync();

            if (accessStatus == GeolocationAccessStatus.Allowed)
            {
                Geolocator geolocator = new Geolocator();

                Geoposition position = await geolocator.GetGeopositionAsync();

                if (position != null)
                {
                    mapSightings.Center = position.Coordinate.Point;
                    mapSightings.ZoomLevel = 10;
                }
            }
        }

        private void DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if (datePickerEndDate.Date != null)
            {
                datePickerStartDate.MaxDate = datePickerEndDate.Date.Value;
            }
            if (datePickerStartDate.Date != null)
            {
                datePickerEndDate.MinDate = datePickerStartDate.Date.Value;
            }

            if ((datePickerEndDate.Date != null) && (datePickerStartDate.Date != null))
            {
                PopulateMap(from sighting in allSightings
                            where ((sighting.DateCreated >= datePickerStartDate.Date) && (sighting.DateCreated <= datePickerEndDate.Date))
                            select sighting);
            }
        }

        private void buttonReportAtMyLocation_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddSightingPage), true);
        }
    }
}
