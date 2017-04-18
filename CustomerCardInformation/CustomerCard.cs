using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Globalization;

namespace CustomerCardInformation
{
    public partial class CustomerCard : Form
    {
        private UserDAO currentValidUser;
        private UserManager objUserManager;
        
        string connectionString;
        public CustomerCard()
        {
            InitializeComponent();

            connectionString = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=custdatadb.mdb;Jet OLEDB:Database Password=faisik;Persist Security Info=True;";
            this.objUserManager = new UserManager("dataconfig.xml");
           
            currentValidUser = null;
            getAllData();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Login objLogin = new Login();
            objLogin.ShowDialog();
            //if you exit without puting correct password from login
            //it will put IsValidLogin False and will exit the program
            if (!objLogin.IsValidLogin)
            {
                this.Close();
            }
            else
            {
                this.currentValidUser = objLogin.ValidUser;
                if (this.currentValidUser.Username.ToLower() == "root")
                {
                    this.userManagementToolStripMenuItem.Enabled = true;
                }
                else
                {
                    this.userManagementToolStripMenuItem.Enabled = false;
                }
                this.labelUser.Text = this.currentValidUser.Username;
                ButtonNew_Click(this, null);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void userManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BaseUserManagement objBase = new BaseUserManagement(this.currentValidUser.Username, this.currentValidUser.AccessType, objUserManager);
            objBase.ShowDialog();
        }

        private void buttonLogout_Click(object sender, EventArgs e)
        {
            this.Hide();
            Login objLogin = new Login();
            objLogin.ShowDialog();
            //if you exit without puting correct password from login
            //it will put IsValidLogin False and will exit the program
            if (!objLogin.IsValidLogin)
            {
                this.Close();
            }
            else
            {
                this.currentValidUser = objLogin.ValidUser;
                if (this.currentValidUser.Username.ToLower() == "root")
                {
                    this.userManagementToolStripMenuItem.Enabled = true;
                }
                else
                {
                    this.userManagementToolStripMenuItem.Enabled = false;
                }
                this.labelUser.Text = this.currentValidUser.Username;
            
            }
            this.Show();
        }

        private void getAllData()
        {
            OleDbConnection objCon = new OleDbConnection(this.connectionString);
            try
            {
                objCon.Open();
                OleDbCommand objCmd = objCon.CreateCommand();
                objCmd.CommandText = "SELECT id,loanno,customar,ldate,ltime,lamount,method,cardno,exp,cvv,usern FROM cardinfo";
                OleDbDataAdapter objAdaptor = new OleDbDataAdapter(objCmd);
                DataTable objTable = new DataTable();
                objAdaptor.Fill(objTable);
                string[] value = { "", "", "", "", "", "", "", "", "","",""};
                this.objDataGridViewList.Rows.Clear();
                for (int i = 0; i < objTable.Rows.Count; i++)
                {
                    for (int j = 0; j < value.Length; j++)
                    {
                        value[j] = objTable.Rows[i][j].ToString(); 
                    }
                    this.objDataGridViewList.Rows.Add(value);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
            finally
            {
                objCon.Close();
            }
        }

        private void ButtonDelete_Click(object sender, EventArgs e)
        {
            DialogResult yesno = MessageBox.Show("Do you want to delete this information?", "Delete Conformation!", MessageBoxButtons.YesNo);
            if (yesno == DialogResult.No)
            {
                return;
            }

            OleDbConnection objCon = new OleDbConnection(this.connectionString);
            try
            {
                objCon.Open();
                OleDbCommand objCmd = objCon.CreateCommand();
                objCmd.CommandText = "DELETE FROM cardinfo WHERE id="+this.labelID.Text;
                objCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
            finally
            {
                objCon.Close();
            }
            this.getAllData();
            ButtonNew_Click(this, null);
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            if (this.TextBoxLoan.Text.Trim().Length < 4 || this.TextBoxname.Text.Trim().Length < 3 || this.TextBoxAmount.Text.Trim().Length < 1 || this.TextBoxCard.Text.Trim().Length<4)
            {
                MessageBox.Show("Please filled up the form properly, then try to save!");
                return;
            }
            string temp = "";
            OleDbConnection objCon = new OleDbConnection(this.connectionString);
            try
            {
                objCon.Open();
                OleDbCommand objCmd = objCon.CreateCommand();
                objCmd.Parameters.Clear();
                if (this.labelID.Text == "0")
                {
                    objCmd.CommandText = "INSERT INTO cardinfo(loanno,customar,ldate,ltime,lamount,method,cardno,exp,cvv,usern) VALUES(@loanno,@customar,@ldate,@ltime,@lamount,@method,@cardno,@exp,@cvv,@usern)";
                    
                }
                else
                {
                    objCmd.CommandText = "UPDATE cardinfo SET loanno = @loanno,customar=@customar,ldate=@ldate,ltime=@ltime,lamount=@lamount,method=@method,cardno=@cardno,exp=@exp,cvv=@cvv WHERE id = " + this.labelID.Text;   
                }
                objCmd.Parameters.Add("@loanno", OleDbType.VarChar,100).Value = this.TextBoxLoan.Text.Trim();
                objCmd.Parameters.Add("@customar", OleDbType.VarChar,100).Value = this.TextBoxname.Text.Trim();
                objCmd.Parameters.Add("@ldate", OleDbType.VarChar,100).Value = this.DateTimePickerDate.Value.ToShortDateString();
                objCmd.Parameters.Add("@ltime", OleDbType.VarChar, 100).Value = this.DateTimePickerTime.Value.ToShortTimeString();
                objCmd.Parameters.Add("@lamount", OleDbType.VarChar,100).Value = this.TextBoxAmount.Text.Trim();
                objCmd.Parameters.Add("@method", OleDbType.VarChar,100).Value = this.TextBoxMethod.Text.Trim();
                objCmd.Parameters.Add("@cardno", OleDbType.VarChar,100).Value = this.TextBoxCard.Text.Trim();
                objCmd.Parameters.Add("@exp", OleDbType.VarChar,100).Value = this.TextBoxExp.Text.Trim();
                objCmd.Parameters.Add("@cvv", OleDbType.VarChar, 100).Value = this.textBoxCVV.Text.Trim();
                if (this.labelID.Text == "0")
                {
                    objCmd.Parameters.Add("@usern", OleDbType.VarChar, 100).Value = this.labelUser.Text.Trim();
                }
                temp = objCmd.CommandText;
                objCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString()+temp);
            }
            finally
            {
                objCon.Close();
            }
            this.getAllData();
            ButtonNew_Click(this, null);
        }

        private void ButtonNew_Click(object sender, EventArgs e)
        {
            this.labelID.Text = "0";
            this.TextBoxLoan.Text = "";
            this.TextBoxname.Text = "";
            this.DateTimePickerDate.Value=DateTime.Now;
            this.DateTimePickerTime.Value = DateTime.Now;
            this.TextBoxAmount.Text = "";
            this.TextBoxMethod.Text = "";
            this.TextBoxCard.Text = "";
            this.TextBoxExp.Text = "";
            this.textBoxCVV.Text = "";
            this.labelUser.Text = this.currentValidUser.Username;
        }

        private void objDataGridViewList_SelectionChanged(object sender, EventArgs e)
        {
            if (this.objDataGridViewList.Rows.Count < 1)
                return;
            DataGridViewRow row;
            try
            {
                row = this.objDataGridViewList.SelectedRows[0];
            }
            catch {
                return;
            }

            this.labelID.Text = row.Cells[0].Value.ToString();
            this.TextBoxLoan.Text = row.Cells[1].Value.ToString();
            this.TextBoxname.Text = row.Cells[2].Value.ToString();
            try
            {
                this.DateTimePickerDate.Value = DateTime.Parse(row.Cells[3].Value.ToString(), new CultureInfo("en-US", false));
                this.DateTimePickerTime.Value = DateTime.Parse(row.Cells[4].Value.ToString(), new CultureInfo("en-US", false));
            }
            catch { }

            this.TextBoxAmount.Text = row.Cells[5].Value.ToString();
            this.TextBoxMethod.Text = row.Cells[6].Value.ToString();
            this.TextBoxCard.Text = row.Cells[7].Value.ToString();
            this.TextBoxExp.Text = row.Cells[8].Value.ToString();
            this.textBoxCVV.Text = row.Cells[9].Value.ToString();
            this.labelUser.Text = row.Cells[10].Value.ToString();
        }

        private void objDataGridViewList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            objDataGridViewList_SelectionChanged(sender,null);
        }

    }
}
