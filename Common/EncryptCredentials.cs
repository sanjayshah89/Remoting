using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Configuration;


namespace Common
{
    public class EncryptCredentials : Dictionary<string, string>
    {
        // Change the following keys to ensure uniqueness
        // Must be 8 bytes  
        protected byte[] _keyBytes = null;        

        // Must be at least 8 characters
        protected string _keyString = ConfigurationManager.AppSettings["KeyString"];

        // Name for checksum value (unlikely to be used as arguments by user)
        protected string _checksumKey = "__$$";

        /// <summary>
        /// Creates an empty dictionary
        /// </summary>
        public EncryptCredentials()
        {
           AssignKeyBytes();
        }
        
        /// <summary>
        /// Assigns the the 8 key bytes to_keyBytes from web.config 
        /// </summary>
        private void AssignKeyBytes()
        {
            string value = ConfigurationManager.AppSettings["KeyBytes"];            
            _keyBytes = value.Split(new[] { ',' }).Select(s => Convert.ToByte(s.Trim(), 16)).ToArray();            
        }

        /// <summary>
        /// Creates a dictionary from the given, encrypted string
        /// </summary>
        /// <param name="encryptedData"></param>
        public EncryptCredentials(string encryptedData)
        {
            AssignKeyBytes();

            // Descrypt string
            string data = Decrypt(encryptedData);

            // Parse out key/value pairs and add to dictionary
            string checksum = null;
            string[] args = data.Split('&');

            foreach (string arg in args)
            {
                int i = arg.IndexOf('=');
                if (i != -1)
                {
                    string key = arg.Substring(0, i);
                    string value = arg.Substring(i + 1);
                    if (key == _checksumKey)
                        checksum = value;
                    else
                        base.Add(key,value);
                }
            }

            // Clear contents if valid checksum not found
            if (checksum == null || checksum != ComputeChecksum())
                base.Clear();
        }

        /// <summary>
        /// Returns an encrypted string that contains the current dictionary
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // Build query string from current contents
            StringBuilder content = new StringBuilder();

            foreach (string key in base.Keys)
            {
                if (content.Length > 0)
                    content.Append('&');
                content.AppendFormat("{0}={1}",key,base[key]);
            }

            // Add checksum
            if (content.Length > 0)
                content.Append('&');
            content.AppendFormat("{0}={1}", _checksumKey, ComputeChecksum());

            return Encrypt(content.ToString());
        }

        /// <summary>
        /// Returns a simple checksum for all keys and values in the collection
        /// </summary>
        /// <returns></returns>
        protected string ComputeChecksum()
        {
            int checksum = 0;

            foreach (KeyValuePair<string, string> pair in this)
            {
                checksum += pair.Key.Sum(c => c - '0');
                checksum += pair.Value.Sum(c => c - '0');
            }
            return checksum.ToString("X");
        }

        /// <summary>
        /// Encrypts the given text
        /// </summary>
        /// <param name="text">Text to be encrypted</param>
        /// <returns></returns>
        protected string Encrypt(string text)
        {
            try
            {
                byte[] keyData = Encoding.UTF8.GetBytes(_keyString.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] textData = Encoding.UTF8.GetBytes(text);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms,
                  des.CreateEncryptor(keyData, _keyBytes), CryptoStreamMode.Write);
                cs.Write(textData, 0, textData.Length);
                cs.FlushFinalBlock();
                return GetString(ms.ToArray());
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Decrypts the given encrypted text
        /// </summary>
        /// <param name="text">Text to be decrypted</param>
        /// <returns></returns>
        protected string Decrypt(string text)
        {
            try
            {
                byte[] keyData = Encoding.UTF8.GetBytes(_keyString.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] textData = GetBytes(text);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms,
                  des.CreateDecryptor(keyData, _keyBytes), CryptoStreamMode.Write);
                cs.Write(textData, 0, textData.Length);
                cs.FlushFinalBlock();
                return Encoding.UTF8.GetString(ms.ToArray());
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Converts a byte array to a string of hex characters
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected string GetString(byte[] data)
        {
            StringBuilder results = new StringBuilder();

            foreach (byte b in data)
                results.Append(b.ToString("X2"));

            return results.ToString();
        }

        /// <summary>
        /// Converts a string of hex characters to a byte array
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected byte[] GetBytes(string data)
        {
            // GetString() encodes the hex-numbers with two digits
            byte[] results = new byte[data.Length / 2];

            for (int i = 0; i < data.Length; i += 2)
                results[i / 2] = Convert.ToByte(data.Substring(i, 2), 16);

            return results;
        }
    }
}