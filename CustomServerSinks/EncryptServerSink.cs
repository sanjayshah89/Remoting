using Common;
using DataLayer;
using Objects;
using System;
using System.Configuration;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Linq;


namespace CustomServerSinks
{
    public class EncryptServerSink : BaseCustomSink
    {
     
        private byte delta;
        public EncryptServerSink(ServerSinkData data)
        {
            this.delta = 1;
            if (data.ConfigurationData.Properties["delta"] != null)
            {
                this.delta = byte.Parse(data.ConfigurationData.Properties["delta"].ToString());
            }

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

            while ((tempIntData = source.ReadByte()) != -1)
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

            if (headers["CustomEncryption"] != null && headers["Credentials"] != null)
            {

                var args = new EncryptCredentials(headers["Credentials"].ToString());
                User user = new User();
                user.Username = string.Format("{0}", args["Username"]);
                user.Password = string.Format("{0}", args["Password"]);
                user.AddressTable.Add(user.Username, string.Format("{0}", args["Uri"]));
                UserDataManager userManager = new UserDataManager();
                if (user != null && userManager.UserLogOn(user.Username, user.Password) != null)
                {
                    // okay
                    Console.WriteLine(
                        "AuthServerSink: retrieved valid credential information: username: {0}, password {1}.",
                        user.Username, user.Password);
                    LoginInfo.Authenticated = true;
                    LoginInfo.LoginName = user.Username;
                    LoginInfo.LoginPassword = user.Password;

                    Console.WriteLine("EncryptServerSink: decrypting request");
                    stream = this.Decrypt(stream);
                    state = true;

                    return;
                }
            }
            // not good!
            Console.WriteLine(
                "DemoCredentialsServerSink: retrieved bad credentials or credentials are missing. Throwing an exception.");
            throw new RemotingException("Invalid credentials.");

        }

        protected override void ProcessResponse(IMessage message, ITransportHeaders headers, ref Stream stream, object state)
        {
            if (state != null)
            {
                Console.WriteLine("EncryptServerSink: encrypting response");
                stream = this.Encrypt(stream);
                headers["CustomEncryption"] = "Yes";
                headers["LoginInfo"] = LoginInfo.LoginName + "," + LoginInfo.Authenticated.ToString();
            }
        }
    }
}



