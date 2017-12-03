using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rat_App
{
    class User
    {
        [JsonProperty(PropertyName = "user_id")]
        public string ID { get; set; }
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
        [JsonProperty(PropertyName = "email_verified")]
        public bool EmailVerified { get; set; }
        [JsonProperty(PropertyName = "created_at")]
        public DateTime CreationDate { get; set; }
        [JsonProperty(PropertyName = "logins_count")]
        public int LoginCount { get; set; }
        [JsonProperty(PropertyName = "last_ip")]
        public string LastIp { get; set; }
        [JsonProperty(PropertyName = "last_login")]
        public DateTime LastLogin { get; set; }
        [JsonProperty(PropertyName = "blocked")]
        public bool Blocked { get; set; }
        [JsonProperty(PropertyName = "user_metadata")]
        public UserMetadata Metadata { get; set; }

        public enum Roles
        {
            Admin, User
        }

        public class UserMetadata
        {
            [JsonProperty(PropertyName = "full_name")]
            public string Name { get; set; }
            [JsonProperty(PropertyName = "role")]
            public Roles Role { get; set; }
        }
    }
}
