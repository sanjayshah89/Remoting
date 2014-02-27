using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotingClient
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            string path = Directory.GetCurrentDirectory();
            //path for bin->debug folder
            RemotingConfiguration.Configure(path + "\\App.config", false);           

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm());
        }
    }
}
