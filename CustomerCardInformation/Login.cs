using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CustomerCardInformation
{
    public partial class Login : Form
    {
        public bool IsValidLogin { get; set; }
        public UserDAO ValidUser {get;set;}
        private UserManager objUserManager;
        //private string[] USERNAME = { "admin", "root", "user", "3ztelecom" };
        //private string[] PASSWORD = { "admin", "root", "user", "3ztelecom" };
        public Login()
        {
            InitializeComponent();
            IsValidLogin = false;
            this.ValidUser = null;
            this.objUserManager = new UserManager("dataconfig.xml");
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.IsValidLogin = false;
            this.ValidUser = null;
            this.Close();
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            UserDAO objUser = this.objUserManager.checkUsernameAndPassword(this.textBoxUser.Text.Trim(), this.textBoxPass.Text);
            if (objUser != null)
            {
                this.IsValidLogin = true;
                this.ValidUser = objUser;
                this.Close();
                return;
            }
            this.labelMsg.ForeColor = Color.Red;
            this.labelMsg.Text = "Invalid Login !!";
            this.textBoxPass.Text = "";
        }
    }
}
