using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using System.Diagnostics;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Rat_App.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddSightingPage : Page
    {
        public AddSightingPage()
        {
            this.InitializeComponent();

            AutoPopulate();
        }

        private async void AutoPopulate()
        {
            GeolocationAccessStatus accessStatus = await Geolocator.RequestAccessAsync();

            try
            {
                if (accessStatus == GeolocationAccessStatus.Allowed)
                {
                    Geolocator geolocator = new Geolocator();

                    Geoposition position = await geolocator.GetGeopositionAsync();

                    if (position != null)
                    {
                        if (position.Coordinate != null && position.Coordinate.Point != null)
                        {
                            textboxLatitude.Text = position.Coordinate.Point.Position.Latitude.ToString();
                            textboxLongitude.Text = position.Coordinate.Point.Position.Longitude.ToString();

                            MapService.ServiceToken = "KbZihYGbZqdTfhIbgIXJ~y3QYoNVTvXXYxWIO63EVVg~Al0zvLSZjcmFjxyDzDuL8MnGEZSYm20FEt6LIiVHjuVII2y2oxabq0Sxe0Fm0Oip";
                            MapLocationFinderResult result = await MapLocationFinder.FindLocationsAtAsync(position.Coordinate.Point);

                            if(result.Status == MapLocationFinderStatus.Success)
                            {
                                textboxCity.Text = result.Locations[0].Address.Town;
                                textboxAddress.Text = (result.Locations[0].Address.StreetNumber + " " + result.Locations[0].Address.Street).Trim();
                                textboxZipcode.Text = result.Locations[0].Address.PostCode;
                            }
                        }
                    }
                }
            } catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        private async void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog invalidInputDialog = new ContentDialog();
            invalidInputDialog.Title = "Invalid Sighting";
            invalidInputDialog.CloseButtonText = "Ok";
            if (comboBoxLocationType.SelectedIndex == -1)
            {
                invalidInputDialog.Content += "You must select a location type\n";
            }
            if(comboBoxBorough.SelectedIndex == -1)
            {
                invalidInputDialog.Content += "You must select a Borough\n";
            }
            bool result = int.TryParse(textboxZipcode.Text, out int intOutput);
            if ((textboxZipcode.Text.Trim().Length != 5) || !result)
            {
                invalidInputDialog.Content += "You must enter a 5 digit zipcode\n";
            }
            if (textboxCity.Text.Trim().Equals(""))
            {
                invalidInputDialog.Content += "You must enter a city\n";
            }
            if (textboxAddress.Text.Trim().Equals(""))
            {
                invalidInputDialog.Content += "You must enter a street address\n";
            }
            result = double.TryParse(textboxLatitude.Text, out double output);
            if (!result)
            {
                invalidInputDialog.Content += "You must enter a latitude as a decimal number\n";
            }
            result = double.TryParse(textboxLongitude.Text, out output);
            if (!result)
            {
                invalidInputDialog.Content += "You must enter a longitude as a decimal number\n";
            }

            if((invalidInputDialog.Content != null) && (!invalidInputDialog.Content.Equals("")))
            {
                invalidInputDialog.ShowAsync();
                return;
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://muffindreamers.azurewebsites.net/sightings/report");
            request.ContentType = "application/json";
            request.Method = "POST";

            using (StreamWriter writer = new StreamWriter(await request.GetRequestStreamAsync()))
            {
                Sighting sighting = new Sighting();
                sighting.LocationType = ((ComboBoxItem)comboBoxLocationType.SelectedItem).Content.ToString();
                sighting.Borough = ((ComboBoxItem)comboBoxBorough.SelectedItem).Content.ToString();
                sighting.Zipcode = int.Parse(textboxZipcode.Text);
                sighting.City = textboxCity.Text;
                sighting.StreetAddress = textboxAddress.Text;
                sighting.Latitude = double.Parse(textboxLatitude.Text);
                sighting.Longitude = double.Parse(textboxLongitude.Text);
                sighting.DateCreated = DateTime.Now;

                string json = JsonConvert.SerializeObject(sighting);

                writer.Write(json);
                writer.Flush();
            }

            try
            {
                HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();

                if (response.StatusCode == HttpStatusCode.Created)
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        Sighting sighting = Sighting.FromJson(reader.ReadToEnd());
                        ContentDialog createdDialog = new ContentDialog() { Title = "Success!", Content = $"Sighting report created with ID {sighting.Id}", CloseButtonText="Ok" };
                        createdDialog.ShowAsync();
                    }
                }
                else
                {
                    ContentDialog failedDialog = new ContentDialog() { Title = "Error!", Content = $"There was an error creating your sighting report: {response.StatusCode}", CloseButtonText="Ok" };
                    failedDialog.ShowAsync();
                }

                Frame.Navigate(typeof(MainPage));
            } catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void IntTextbox(object sender, KeyRoutedEventArgs args)
        {
            VirtualKey[] allowedKeys = {VirtualKey.Number0, VirtualKey.Number1, VirtualKey.Number2,
                VirtualKey.Number3, VirtualKey.Number4, VirtualKey.Number5, VirtualKey.Number6,
                VirtualKey.Number7, VirtualKey.Number8, VirtualKey.Number9, VirtualKey.Decimal};

            if(!allowedKeys.Contains(args.Key))
            {
                args.Handled = true;
            }
        }

        private void DoubleTextBox(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            double dtemp;
            if (!double.TryParse(sender.Text, out dtemp) && sender.Text != "")
            {
                int pos = sender.SelectionStart - 1;
                sender.Text = sender.Text.Remove(pos, 1);
                sender.SelectionStart = pos;
            }
        }
    }
}
