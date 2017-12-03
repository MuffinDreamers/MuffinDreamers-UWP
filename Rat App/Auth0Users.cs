using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rat_App.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Rat_App
{
    static class Auth0Users
    {
        private const string tokenEndpoint = "https://muffindreamers.auth0.com/oauth/token";
        private const string usersEndpoint = "https://muffindreamers.auth0.com/api/v2/users";

        private static AccessToken accessToken;

        private static IEnumerable<User> users;

        private class AccessToken
        {
            public AccessToken(string token, DateTime expiration)
            {
                Token = token;
                Expiration = expiration;
            }
            public string Token { get; }
            public DateTime Expiration { get; }
            public bool Expired { get { return DateTime.UtcNow > Expiration; } }
        }

        public static IEnumerable<User> GetUsers()
        {
            if (users == null)
                Debug.WriteLine("Warning: users list is null. Did you call LoadUsers()?");
            return users;
        }

        public static async Task LoadUsers()
        {
            Debug.WriteLine("Beginning LoadUsers");
            accessToken = await GetAccessToken();
            users = await GetUserListAsync();
            Debug.WriteLine("Finished LoadUsers");
        }

        public static async Task<User> GetUserAsync(string userID)
        {
            accessToken = await GetAccessToken();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(usersEndpoint + "/" + userID);
            request.Method = "GET";
            request.Headers["Authorization"] = "Bearer " + accessToken.Token;

            try
            {
                HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                    {
                        User user = JsonConvert.DeserializeObject<User>(sr.ReadToEnd());
                        return user;
                    }
                }
            } catch (WebException e)
            {
                Debug.WriteLine(e);
            }
            return null;
        }

        public static async Task<bool> BlockUser(string userID, Action<bool> callback = null)
        {
            JObject o = new JObject();
            o.Add("blocked", true);
            bool result = await UpdateUser(userID, o);
            callback?.Invoke(result);
            return result;
        }

        public static async Task<bool> UnblockUser(string userID, Action<bool> callback = null)
        {
            JObject o = new JObject();
            o.Add("blocked", false);
            bool result = await UpdateUser(userID, o);
            callback?.Invoke(result);
            return result;
        }

        private static async Task<AccessToken> GetAccessToken()
        {
            if (accessToken == null || accessToken.Expired)
            {
                accessToken = await GetApiKeyAsync();
            }
            return accessToken;
        }

        private static async Task<AccessToken> GetApiKeyAsync()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(tokenEndpoint);
            request.ContentType = "application/json";
            request.Method = "POST";

            using (StreamWriter sw = new StreamWriter(await request.GetRequestStreamAsync()))
            {
                var json = JsonConvert.SerializeObject(new
                {
                    grant_type = "client_credentials",
                    client_id = Credentials.auth0ManagementClientId,
                    client_secret = Credentials.auth0ManagementClientSecret,
                    audience = "https://muffindreamers.auth0.com/api/v2/"
                });
                sw.Write(json);
                sw.Flush();
            }

            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    JObject json = JObject.Parse(sr.ReadToEnd());
                    string token = json.GetValue("access_token").Value<string>();
                    DateTime expiration = DateTime.UtcNow.AddSeconds(json.GetValue("expires_in").Value<int>());
                    return new AccessToken(token, expiration);
                }
            }
            return null;
        }

        private static async Task<IEnumerable<User>> GetUserListAsync()
        {
            accessToken = await GetAccessToken();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(usersEndpoint);
            request.Method = "GET";
            request.Headers["Authorization"] = "Bearer " + accessToken.Token;

            try
            {
                HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                    {
                        User[] users = JsonConvert.DeserializeObject<User[]>(sr.ReadToEnd());
                        return users;
                    }
                }
            } catch (WebException e)
            {
                Debug.WriteLine(e);
            }
            return null;
        }

        private static async Task<bool> UpdateUser(string userID, JObject value)
        {
            accessToken = await GetAccessToken();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(usersEndpoint + "/" + userID);
            request.Method = "PATCH";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + accessToken.Token;

            using (StreamWriter sw = new StreamWriter(await request.GetRequestStreamAsync()))
            {
                sw.Write(value.ToString(Formatting.None));
                sw.Flush();
            }

            try
            {
                HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
            }
            catch (WebException e)
            {
                Debug.WriteLine(e);
            }
            return false;
        }
    }
}
