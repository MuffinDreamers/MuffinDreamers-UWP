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
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Rat_App.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GraphPage : Page
    {
        const int DEFAULT_MONTHS = 12;
        const string DATE_FORMAT = "MM/yy";

        IEnumerable<Sighting> allSightings;

        public GraphPage()
        {
            this.InitializeComponent();
        }

        private void PopulateGraph(IEnumerable<Sighting> sightings)
        {
            PopulateGraph(sightings, DateTime.Now.AddMonths(-DEFAULT_MONTHS), DateTime.Now);
        }

        private void PopulateGraph(IEnumerable<Sighting> sightings, DateTime beginningDate, DateTime endDate)
        {
            if (beginningDate > endDate)
                throw new ArgumentOutOfRangeException("Beginning date must be before end date");

            sightings = FilterByDate(sightings, beginningDate, endDate);

            List<ChartEntry> entries = new List<ChartEntry>();

            for(DateTime d = beginningDate; d <= endDate; d = d.AddMonths(1))
            {
                if(!entries.Any(x => x.Month == d.ToString(DATE_FORMAT)))
                {
                    entries.Add(new ChartEntry() { Month = d.ToString(DATE_FORMAT), Reports = 0 });
                }
            }

            //Double check that the last month got added
            if(!entries.Any(x => x.Month == endDate.ToString(DATE_FORMAT)))
            {
                entries.Add(new ChartEntry() { Month = endDate.ToString(DATE_FORMAT), Reports = 0 });
            }

            for (int i = sightings.Count() - 1; i >= 0; i--)
            {
                Sighting s = sightings.ElementAt(i);
                ChartEntry entry = entries.Find(x => x.Month == s.DateCreated.ToString(DATE_FORMAT));
                if(entry == null)
                {
                    continue;
                }
                entry.Reports++;
            }

            ((LineSeries)chartSightings.Series[0]).ItemsSource = entries;
        }

        private IEnumerable<Sighting> FilterByDate(IEnumerable<Sighting> sightings, DateTime beginning, DateTime end)
        {
            return (from sighting in sightings
                   where ((sighting.DateCreated >= beginning) && (sighting.DateCreated <= end))
                   select sighting);
        }

        private class ChartEntry
        {
            public string Month { get; set; }
            public int Reports { get; set; }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            allSightings = (IEnumerable<Sighting>)e.Parameter;

            if (allSightings != null)
                PopulateGraph(allSightings);
        }

        private void DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if(datePickerEndDate.Date != null)
            {
                datePickerStartDate.MaxDate = datePickerEndDate.Date.Value;
            }
            if (datePickerStartDate.Date != null)
            {
                datePickerEndDate.MinDate = datePickerStartDate.Date.Value;
            }

            if ((datePickerEndDate.Date != null) && (datePickerStartDate.Date != null))
            {
                PopulateGraph(allSightings, datePickerStartDate.Date.Value.DateTime, datePickerEndDate.Date.Value.DateTime);
            }
        }

        private void buttonReturn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
