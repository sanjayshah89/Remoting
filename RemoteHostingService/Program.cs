using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.IO;
using Common;


namespace RemoteHostingService
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = Directory.GetCurrentDirectory();

            RemotingConfiguration.Configure(path+"\\App.config", false);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(RemoteObject), "Service", WellKnownObjectMode.Singleton);
            Console.WriteLine("Remoting Service has started at " + DateTime.Now);
            Console.ReadLine();
        }
    }
}
