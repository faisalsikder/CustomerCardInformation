using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace CustomerCardInformation
{
    public partial class BaseUserManagement : Form
    {        
        private string currentUser;
        private string currentUserAccesssType;
        private UserManager objUserManager;

        public BaseUserManagement(string curUser, string curUserAccessType,UserManager uManager)
        {
            InitializeComponent();
            this.dataGridViewUserList.ForeColor = Color.Black;
            this.currentUser = curUser;
            this.currentUserAccesssType = curUserAccessType;
            this.objUserManager = uManager;
            this.fillUserList();
            this.clearNewUser();
        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void fillUserList()
        {
            
            this.dataGridViewUserList.Rows.Clear();
            foreach (DictionaryEntry entry in this.objUserManager.userTable)
            {
                UserDAO objUser = (UserDAO)entry.Value;
                string[] item = {objUser.Username , objUser.EmpName, objUser.AccessType, objUser.EmpDesignation,objUser.UserStatus, "Delete", "Edit" };
                this.dataGridViewUserList.Rows.Add(item);
            }
            this.colorGrid(this.dataGridViewUserList);
        }
        /// <summary>
        /// Put Color into Grid in ODD rows
        /// </summary>
        /// <param name="objGrid"></param>
        private void colorGrid(DataGridView objGrid)
        {
            for (int i = 0; i < objGrid.Rows.Count; i++)
            {
                if (i % 2 == 1)
                {
                    objGrid.Rows[i].DefaultCellStyle.BackColor = Color.LightGray;
                }
            }
        }

        private void buttonSaveChangePassword_Click(object sender, EventArgs e)
        {
            if (this.textBoxNewPassword.Text.Length < 4)
            {
                MessageBox.Show("Password length must be 4 characters or more !");
                return;
            }
            else if (!this.textBoxNewPassword.Text.Equals(this.textBoxConfNewPassword.Text))
            {
                MessageBox.Show("New password and Confirm password don't match !");
                return;
            }
            if (this.objUserManager.userTable.Contains(this.currentUser))
            {
                UserDAO objUser = (UserDAO)this.objUserManager.userTable[this.currentUser];
                string hashPass = this.objUserManager.Sha1Encryption(this.textBoxCurPassword.Text);
                if (hashPass.Equals(objUser.Password))
                {
                    hashPass = this.objUserManager.Sha1Encryption(this.textBoxNewPassword.Text);
                    objUser.Password = hashPass;
                    this.objUserManager.saveXMLDocument();

                    this.textBoxConfNewPassword.Text = "";
                    this.textBoxCurPassword.Text = "";
                    this.textBoxNewPassword.Text = "";

                    this.labelChgPassMsg.Text = "Password changed successfully !!";
                    this.labelChgPassMsg.ForeColor = Color.Green;
                }
                else
                {
                    MessageBox.Show("Current password doesn't match !");
                    return;
                }
            }
            else
            {
                this.labelChgPassMsg.Text = "Error Changing Password !!!";
                this.labelChgPassMsg.ForeColor = Color.Red;
            }
        }

        private void dataGridViewUserList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
            // this If will Do the Delete Operation on the Datagrid, So after save this Remote Sensor won't be available to Base
            if ((e.ColumnIndex == 5) && (e.RowIndex >= 0) && (e.RowIndex < dataGridViewUserList.Rows.Count))
            {
                if(this.dataGridViewUserList.Rows[e.RowIndex].Cells[0].Value.ToString().Equals(this.currentUser,StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("You can't delete yourself from the system!");
                    return;
                }

                if (dataGridViewUserList.Rows.Count <= 1)
                {
                    MessageBox.Show("You have only one user, can't delete this user");
                    return;
                }
                DialogResult yesNo = MessageBox.Show("Do you want to remove User?\nName:" +
                    this.dataGridViewUserList.Rows[e.RowIndex].Cells[1].Value.ToString() + "\nUsername: " +
                    this.dataGridViewUserList.Rows[e.RowIndex].Cells[0].Value.ToString(), "User Remove Confirmation?", MessageBoxButtons.YesNo);
                if (yesNo.Equals(DialogResult.Yes))
                {
                    this.objUserManager.userTable.Remove(this.dataGridViewUserList.Rows[e.RowIndex].Cells[0].Value.ToString());
                    this.dataGridViewUserList.Rows.RemoveAt(e.RowIndex);
                    this.objUserManager.saveXMLDocument();
                }
            } // this Else if will put the Selected sensor value into Edit window.
            else if ((e.ColumnIndex == 6) && (e.RowIndex >= 0) && (e.RowIndex < dataGridViewUserList.Rows.Count))
            {
                DataGridViewRow row = this.dataGridViewUserList.Rows[e.RowIndex];
                this.textBoxUsername.Text = row.Cells[0].Value.ToString();
                this.textBoxEmployeeName.Text = row.Cells[1].Value.ToString();
                this.textBoxDesignation.Text = row.Cells[3].Value.ToString();
                this.comboBoxStatus.SelectedItem = row.Cells[4].Value.ToString();
                this.comboBoxAccess.SelectedItem = row.Cells[2].Value.ToString();
                this.textBoxUsername.ReadOnly = true;
                this.buttonSaveUser.Text = "Save User";
                this.buttonCancel.Text = "Cancel";
                if (dataGridViewUserList.Rows.Count <= 1 ||
                    this.dataGridViewUserList.Rows[e.RowIndex].Cells[0].Value.ToString().Equals(this.currentUser, StringComparison.OrdinalIgnoreCase))
                {
                    this.comboBoxStatus.Enabled = false;
                }
            }
        }

        private void clearNewUser()
        {
            this.textBoxUsername.Text = "";
            this.textBoxEmployeeName.Text = "";
            this.textBoxDesignation.Text = "";
            this.comboBoxStatus.SelectedItem = "Active";
            this.comboBoxAccess.SelectedIndex = -1;
            this.textBoxUsername.ReadOnly = false;
            this.buttonSaveUser.Text = "Add New User";
            this.buttonCancel.Text = "Clear";
            this.textBoxConfPassword.Text = "";
            this.textBoxPassword.Text = "";
            this.comboBoxStatus.Enabled = true;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            clearNewUser();
        }

        private void buttonSaveUser_Click(object sender, EventArgs e)
        {
            if (!this.textBoxUsername.ReadOnly) //Add operation
            {
                UserDAO objUser = new UserDAO();
                if (this.textBoxPassword.Text.Length < 4 || this.textBoxUsername.Text.Length < 4)
                {
                    MessageBox.Show("Password Length and username must be 4 characters or more !!!");
                    this.textBoxUsername.Focus();
                    return;
                }
                else if (!this.textBoxPassword.Text.Equals(this.textBoxConfPassword.Text))
                {
                    MessageBox.Show("Password & Confirm Password Doesn't match !!!");
                    this.textBoxConfPassword.Focus();
                    return;
                }
                else if (this.objUserManager.userTable.ContainsKey(this.textBoxUsername.Text.Trim()))
                {
                    MessageBox.Show("User "+this.textBoxUsername.Text + ", already exist in the system, try with different name");
                    this.textBoxUsername.Focus();
                    return;
                }
                objUser.Username = this.textBoxUsername.Text.Trim();
                objUser.Password = objUserManager.Sha1Encryption(this.textBoxPassword.Text);
                if (this.comboBoxStatus.SelectedItem.ToString().Equals("Disable"))
                {
                    MessageBox.Show("You canno't create a disabled user!");
                    this.comboBoxStatus.Focus();
                    return;
                }
                objUser.UserStatus = this.comboBoxStatus.SelectedItem.ToString();
                try
                {
                    objUser.AccessType = this.comboBoxAccess.SelectedItem.ToString();
                }
                catch (Exception) {
                    MessageBox.Show("Please select a access type !");
                    this.comboBoxAccess.Focus();
                    return;
                }
                objUser.EmpName = this.textBoxEmployeeName.Text.Trim();
                objUser.EmpDesignation = this.textBoxDesignation.Text.Trim();
                this.objUserManager.userTable.Add(objUser.Username, objUser);
                this.objUserManager.saveXMLDocument();
                string[] gridValue = { objUser.Username, objUser.EmpName, objUser.AccessType, objUser.EmpDesignation, objUser.UserStatus, "Delete", "Edit" };
                this.dataGridViewUserList.Rows.Add(gridValue);
                this.colorGrid(dataGridViewUserList);
                clearNewUser();
            }
            else // Edit operation
            {   
                if(this.objUserManager.userTable.ContainsKey(this.textBoxUsername.Text.Trim())){
                    UserDAO objUser = (UserDAO)this.objUserManager.userTable[this.textBoxUsername.Text.Trim()];
                    if ((this.textBoxPassword.Text.Length > 0) && (this.textBoxPassword.Text.Length < 4))
                    {
                        MessageBox.Show("Password Length must be 4 characters or more !!!");
                        return;
                    }
                    else if ((this.textBoxPassword.Text.Length >= 4) && !(this.textBoxPassword.Text.Equals(this.textBoxConfPassword.Text)))
                    {
                        MessageBox.Show("Password & Confirm Password Doesn't match !!!");
                        return;
                    }
                    else if (this.textBoxPassword.Text.Length >= 4)
                    {
                        objUser.Password = this.objUserManager.Sha1Encryption(this.textBoxPassword.Text);
                    }

                    if (this.comboBoxStatus.Enabled)
                    {
                        objUser.UserStatus = this.comboBoxStatus.SelectedItem.ToString();
                    }
                    objUser.EmpName = this.textBoxEmployeeName.Text;
                    objUser.EmpDesignation = this.textBoxDesignation.Text;
                    objUser.AccessType = this.comboBoxAccess.SelectedItem.ToString();
                    this.objUserManager.saveXMLDocument();
                    clearNewUser();
                    fillUserList();                    
                }else{
                    MessageBox.Show("Error Saving User Information !");
                }
            }
        }

        private void buttonAccCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
