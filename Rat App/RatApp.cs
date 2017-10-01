using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Auth0.OidcClient;
using Auth0.ManagementApi;
using IdentityModel.OidcClient;

namespace Rat_App
{
    class RatApp
    {
        private const string ClientId = "trq9MLo8GsyhnNe6iKDUIOBZrauB1cAD";
        private static RatApp INSTANCE = null;
        User currentUser;

        private RatApp() { }

        public async Task<bool> LoginAsync()
        {
            var client = new Auth0Client(new Auth0ClientOptions
            {
                Domain = "muffindreamers.auth0.com",
                ClientId = ClientId
            });

            LoginResult loginResult = await client.LoginAsync();

            if (loginResult.IsError)
            {
                Debug.WriteLine($"An error occured during login: {loginResult.Error}");
            }
            else
            {
                string metadataUrl = "https://muffindreamers.auth0.com/api/v2/users/user_id?fields=user_metadata&include_fields=true";

                string userID = loginResult.User.FindFirst(c => c.Type == "sub").Value;
                userID = userID.Replace("auth0|", "");
                string email = loginResult.User.FindFirst(c => c.Type == "name").Value;
                string nickname = loginResult.User.FindFirst(c => c.Type == "nickname").Value;

                currentUser = new User(userID, email, nickname);

                Debug.WriteLine($"Successfully authenticated {email}");

                foreach (var claim in loginResult.User.Claims)
                {
                    Debug.WriteLine($"{claim.Type} = {claim.Value}");
                }
            }

            return !loginResult.IsError;
        }

        public async Task<bool> LogoutAsync()
        {
            string url = "https://muffindreamers.auth0.com/v2/logout?client_id=" + ClientId;
            //Debug.WriteLine(url);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            WebResponse response = await request.GetResponseAsync();
            using(System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
            {
                Debug.WriteLine(sr.ReadToEnd());
            }

            return true;
        }

        public static RatApp GetInstance()
        {
            if(INSTANCE == null)
            {
                INSTANCE = new RatApp();
            }
            return INSTANCE;
        }
    }
}
