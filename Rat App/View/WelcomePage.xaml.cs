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
using System.Diagnostics;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Popups;
using IdentityModel.OidcClient;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Rat_App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WelcomePage : Page
    {
        public WelcomePage()
        {
            this.InitializeComponent();

            if(ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                StatusBar bar = StatusBar.GetForCurrentView();

                if(bar != null)
                {
                    bar.BackgroundOpacity = 1;
                    bar.BackgroundColor = Color.FromArgb(255, 0, 121, 107);
                    bar.ForegroundColor = Colors.White;
                }
            }
        }

        private async void buttonLogin_Click(object sender, RoutedEventArgs e)
        {
            LoginResult result = await RatApp.GetInstance().LoginAsync();

            if (!result.IsError)
                Frame.Navigate(typeof(MainPage));
            else
            {
                if (result.Error == "unauthorized")
                    new MessageDialog("Your account has been banned").ShowAsync();
                else
                    new MessageDialog("There was an error authenticating your account").ShowAsync();
            }
        }
    }
}
