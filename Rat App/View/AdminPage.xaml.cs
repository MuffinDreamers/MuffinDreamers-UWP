using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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
    public sealed partial class AdminPage : Page
    {
        private Task loadUserTask;
        public AdminPage()
        {
            this.InitializeComponent();
            adminSearchBox.DisplayMemberPath = "Email";
        }

        private void UpdateText(User user)
        {
            nameText.Text = (string)Resources["adminNamePrefix"] + user.Metadata.Name;
            emailText.Text = (string)Resources["adminEmailPrefix"] + user.Email;
            verifiedText.Text = (string)Resources["adminVerifiedPrefix"] + user.EmailVerified;
            createdText.Text = (string)Resources["adminCreatedPrefix"] + user.CreationDate;
            loginCountText.Text = (string)Resources["adminLoginCountPrefix"] + user.LoginCount;
            lastIpText.Text = (string)Resources["adminLastIpPrefix"] + user.LastIp;
            lastLoginText.Text = (string)Resources["adminLastLoginPrefix"] + user.LastLogin;
            blockedText.Text = (string)Resources["adminBlockedPrefix"] + user.Blocked;

            blockButton.Content = user.Blocked ? "Unblock" : "Block";
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            loadUserTask = Auth0Users.LoadUsers();
        }

        private void blockButton_Click(object sender, RoutedEventArgs e)
        {
            User user = MatchUser(adminSearchBox.Text);
            if (user == null)
                return;
            if (user.Blocked)
                Auth0Users.UnblockUser(user.ID, BlockCallback);
            else
                Auth0Users.BlockUser(user.ID, BlockCallback);
        }

        private void BlockCallback(bool result)
        {
            if (result)
            {
                User user = MatchUser(adminSearchBox.Text);
                if (user == null)
                    return;
                user.Blocked = !user.Blocked;
                UpdateText(user);
            }
            else
            {
                new MessageDialog("There was an updating the user's blocked status").ShowAsync();
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void adminSearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if(args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                sender.ItemsSource = Auth0Users.GetUsers().Where(x => x.Email.StartsWith(sender.Text));
            }
            User user = MatchUser(sender.Text);
            if(user != null)
                UpdateText(user);
        }

        private User MatchUser(string email)
        {
            return Auth0Users.GetUsers().First(x => x.Email.Equals(email));
        }

        private void adminSearchBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            sender.Text = ((User)args.SelectedItem).Email;
        }

        private void adminSearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!loadUserTask.IsCompleted && !loadUserTask.IsCanceled && !loadUserTask.IsFaulted)
                loadUserTask.Wait();

            adminSearchBox.ItemsSource = Auth0Users.GetUsers();
        }
    }
}
