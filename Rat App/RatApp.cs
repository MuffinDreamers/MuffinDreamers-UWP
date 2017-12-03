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
using System.Net.Http;
using System.IO;
using Rat_App.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Rat_App
{
    class RatApp
    {
        private const string ClientId = "trq9MLo8GsyhnNe6iKDUIOBZrauB1cAD";
        private static RatApp INSTANCE = null;

        public static User currentUser { get; private set; }

        private RatApp() { }

        public async Task<LoginResult> LoginAsync()
        {
            var client = new Auth0Client(new Auth0ClientOptions
            {
                Domain = "muffindreamers.auth0.com",
                ClientId = ClientId
            });

            LoginResult loginResult = await client.LoginAsync();

            currentUser = null;

            if (loginResult.IsError)
            {
                Debug.WriteLine($"An error occured during login: {loginResult.Error}");
            }
            else
            {
                currentUser = await Auth0Users.GetUserAsync(loginResult.User.FindFirst(c => c.Type == "sub").Value);
            }

            return loginResult;
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
