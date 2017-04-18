using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomerCardInformation
{
    public class UserDAO
    {
        public string Username { set; get; }
        public string Password { set; get; }
        public string EmpName { set; get; }
        public string AccessType { set; get; }
        public string EmpDesignation { set; get; }
        public string UserStatus { set; get; }

        public UserDAO()
        {
            this.Username = "";
            this.Password = "";
            this.EmpName = "";
            this.AccessType = "";
            this.EmpDesignation = "";
            this.UserStatus = "Disable";
        }
    }
}
