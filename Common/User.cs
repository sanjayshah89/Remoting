using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Objects
{
    public class User
    {

        public User()
        {
            
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public User(string username, string password)
		{
			if (username == null || password == null)
			{
				throw new Exception("both username and password must be specified in the credentials attribute");
			}
            this.Username = username;
            this.Password = password;
		}

        /// <summary>
        /// Gets or Sets the ID of the User
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or Sets the User Name
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or Sets the Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or Sets the Last Logged In Date
        /// </summary>
        public DateTime LoginDate { get; set; }

        public Hashtable AddressTable = new Hashtable();

    }
}
