using Common;
using Objects;
using System;
using System.Configuration;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CustomClientSinks
{
    public class EncryptClientSink : BaseCustomSink
    {
        User currentCredentials;
        private byte delta;      

        public EncryptClientSink(ClientSinkData creationData)
        {
            SinkProviderData usersListConfig = null;
            Uri sanjayuri = new Uri(creationData.Url);
            this.delta = 1;

            if (creationData.ConfigurationData.Properties["delta"] != null)
            {
                delta = byte.Parse(creationData.ConfigurationData.Properties["delta"].ToString());
            }

            foreach (SinkProviderData d in creationData.ConfigurationData.Children)
            {
                if (d.Name == "credentialsList")
                {
                    usersListConfig = d;
                    break;
                }
            }

            if (usersListConfig == null)
            {
                return;
            }

          
            string host;
            int port;


            SinkProviderData credConfig = (SinkProviderData)usersListConfig.Children[0];


            if (credConfig.Name != "credentials")
            {
                return;
            }

            currentCredentials = new User(
                (string)credConfig.Properties["username"],
                (string)credConfig.Properties["password"]);
            currentCredentials.LoginDate = DateTime.Now;

            // retrieve host and port
            host = (string)credConfig.Properties["host"];
            port = credConfig.Properties["port"] != null ?
                int.Parse((string)credConfig.Properties["port"]) : -1;
                      
            currentCredentials.AddressTable.Add(currentCredentials.Username, host+":"+port.ToString());

        }

        public Stream Encrypt(Stream source)
        {
            byte tempByteData;
            int tempIntData;
            MemoryStream encrypted = new MemoryStream();

            while ((tempIntData = source.ReadByte()) != -1)
            {
                tempByteData = (byte)tempIntData;
                tempByteData += this.delta;
                encrypted.WriteByte(tempByteData);
            }

            encrypted.Position = 0;
            return encrypted;
        }

        public Stream Decrypt(Stream source)
        {
            byte tempByteData;
            int tempIntData;
            MemoryStream decrypted = new MemoryStream();

            while (source != null && (tempIntData = source.ReadByte()) != -1)
            {
                tempByteData = (byte)tempIntData;
                tempByteData -= this.delta;
                decrypted.WriteByte(tempByteData);
            }

            decrypted.Position = 0;
            return decrypted;
        }
      

        protected override void ProcessRequest(IMessage message, ITransportHeaders headers, ref Stream stream, ref object state)
        {
            if (this.currentCredentials != null)
            {
                EncryptCredentials cred = new EncryptCredentials();
                cred["Username"] = this.currentCredentials.Username;
                cred["Password"] = this.currentCredentials.Password;
                cred["Uri"] = this.currentCredentials.AddressTable[this.currentCredentials.Username.ToString()].ToString();
                headers["Credentials"] = cred.ToString();
                Console.WriteLine("EncryptClientSink: encrypting request");
                stream = this.Encrypt(stream);
                headers["CustomEncryption"] = "Yes";
            }

        }

        protected override void ProcessResponse(IMessage message, ITransportHeaders headers, ref Stream stream, object state)
        {
            if (headers["CustomEncryption"] != null && headers["LoginInfo"]!= null)
            {
                string[] LoginInfoArry = headers["LoginInfo"].ToString().Split(',');               
                LoginInfo.LoginName = LoginInfoArry[0];
                LoginInfo.Authenticated = Convert.ToBoolean(LoginInfoArry[1]);
                Console.WriteLine("EncryptClientSink: decrypting response");
                stream = this.Decrypt(stream);
            }
        }
    }   

}
