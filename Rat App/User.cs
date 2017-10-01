using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rat_App
{
    class User
    {
        private string id;
        private string email;
        private string nickname;

        public User(string id, string email, string nickname)
        {
            this.id = id;
            this.email = email;
            this.nickname = nickname;
        }

        public string ID {
            get { return id; }
        }

        public string Email {
            get { return email; }
        }

        public string Nickname {
            get { return nickname; }
        }
    }
}
