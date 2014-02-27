using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Data.SqlClient;
using System.Configuration;
using System.Globalization;
using Common;
using System.Runtime.Remoting;
using System.IO;


namespace RemotingClient
{
    public partial class LoginForm : Form
    {
        Common.IRemoteObject clientproxy;
        public LoginForm()
        {
            //eg:if both and client and server apps are on the same machine
            clientproxy = (IRemoteObject)Activator.GetObject(typeof(IRemoteObject), "http://localhost:1234/Service");

            //eg:if both and client and server apps are on different machines. 
            // clientproxy = (Common.IRemoteObject)Activator.GetObject(typeof(Common.IRemoteObject), "http://192.168.0.104:1234/Service");

            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;          
        }
        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
           
           DataSet DepartmentSet = clientproxy.GetDepartment();

           if (DepartmentSet != null && LoginInfo.Authenticated)
           {
               MainForm _mainform = new MainForm(DepartmentSet);
               _mainform.FormBorderStyle = FormBorderStyle.FixedSingle;
               _mainform.MaximizeBox = false;
               _mainform.MinimizeBox = false;
               _mainform.Show();
               this.Hide();
           }         
        }

    }
}
