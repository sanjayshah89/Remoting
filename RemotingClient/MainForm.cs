using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotingClient
{
    public partial class MainForm : Form
    {      

        public MainForm(DataSet DepartmentSet)
        {
            InitializeComponent();

            if (LoginInfo.Authenticated && DepartmentSet != null)
            {
                lblLoginMessage.Text = "Welcome " + LoginInfo.LoginName;               
                GVDeptDetails.DataSource = DepartmentSet.Tables[0];
                GVDeptDetails.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
                GVDeptDetails.ReadOnly = true;

                GVDeptDetails.AutoResizeColumns();
                GVDeptDetails.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;               
                GVDeptDetails.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            }
            else
            {
                MessageBox.Show("The credentials provided is wrong or has been lost.", "Authentication Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

    }
}
