using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Add sighting to database
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
