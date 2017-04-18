using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml;
using System.Windows.Forms;
using System.Globalization;
using System.Security.Cryptography;

namespace CustomerCardInformation
{
    public class UserManager
    {
        public Hashtable userTable;
        private XmlDocument userFileDoc;
        private XmlNode userNodelist;
        

        string fileName;
        public UserManager(string xml_file)
        {
           
            try
            {
                this.userTable = new Hashtable();
                userFileDoc = new XmlDocument();
                userFileDoc.Load(xml_file);
                this.fileName = xml_file;
                //now read all users and put it into a hashtable
                this.userNodelist = userFileDoc.SelectSingleNode("/root/userslist");
                XmlNodeList userNodeList = userNodelist.SelectNodes("user");

                foreach (XmlNode user in userNodeList)
                {
                    UserDAO objUser = new UserDAO();
                    objUser.Username = user.SelectSingleNode("username").InnerText.Trim();
                    objUser.Password = user.SelectSingleNode("pcode").InnerText.Trim();
                    objUser.EmpName = user.SelectSingleNode("empname").InnerText.Trim();
                    objUser.AccessType = user.SelectSingleNode("accesstype").InnerText.Trim();
                    objUser.EmpDesignation = user.SelectSingleNode("empdesignation").InnerText.Trim();
                    objUser.UserStatus = user.SelectSingleNode("status").InnerText.Trim();
                    this.userTable.Add(objUser.Username, objUser);
                }
                this.userNodelist.RemoveAll();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.ToString());
            }
        }

        public void SaveChangeHostConnInfo()
        {
            this.setAllValueToXMLDoc();
            userFileDoc.Save(this.fileName);
        }

        public UserDAO checkUsernameAndPassword(string uName, string password)
        {
            if (this.userTable.ContainsKey(uName))
            {
                UserDAO objUser = (UserDAO)this.userTable[uName];
                string mypass = this.Sha1Encryption(password);
                if (objUser.Password.Equals(mypass) && objUser.UserStatus.Equals("Active"))
                {
                    return objUser;
                }
            }
            return null;
        }


        private void setAllValueToXMLDoc()
        {
            this.userNodelist.RemoveAll();
            //now read sensors
            foreach (DictionaryEntry entry in userTable)
            {
                UserDAO objUser = (UserDAO)entry.Value;

                //user node
                XmlNode userNode = userFileDoc.CreateElement("user");

                XmlNode username = userFileDoc.CreateElement("username");
                username.InnerText = objUser.Username;
                userNode.AppendChild(username);

                XmlNode password = userFileDoc.CreateElement("pcode");
                password.InnerText = objUser.Password;
                userNode.AppendChild(password);

                XmlNode empName = userFileDoc.CreateElement("empname");
                empName.InnerText = objUser.EmpName;
                userNode.AppendChild(empName);

                XmlNode accessType = userFileDoc.CreateElement("accesstype");
                accessType.InnerText = objUser.AccessType;
                userNode.AppendChild(accessType);

                XmlNode empDesig = userFileDoc.CreateElement("empdesignation");
                empDesig.InnerText = objUser.EmpDesignation;
                userNode.AppendChild(empDesig);

                XmlNode userStatus = userFileDoc.CreateElement("status");
                userStatus.InnerText = objUser.UserStatus;
                userNode.AppendChild(userStatus);

                this.userNodelist.AppendChild(userNode);
            }            
        }

        public void saveXMLDocument(){
            this.setAllValueToXMLDoc();
            userFileDoc.Save(this.fileName);            
        }

        public string Sha1Encryption(string data)
        {
            byte[] byteData = Encoding.UTF8.GetBytes(data);
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] hashData = sha1.ComputeHash(byteData);
            string returnValue = "";
            for (int i = 0; i < hashData.Length; i++)
            {
                returnValue += hashData[i].ToString();
            }
            return returnValue;
        }
    }
}
