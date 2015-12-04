using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.Security.Cryptography;
using KryptPadDesktop.Extensions;
using System.IO;

namespace KryptPadDesktop.Security.Legacy
{
    public class EncryptionService
    {
        /// <summary>
        /// Generates a cryptographic key from a password and salt
        /// </summary>
        /// <param name="password"></param>
        /// <param name="saltString"></param>
        /// <returns></returns>
        public static byte[] GenerateEncryptionKey(SecureString password, string saltString)
        {
            //convert the salt string to bytes
            byte[] salt = Convert.FromBase64String(saltString);

            //derive the key from our password and salt
            var keyGen = new Rfc2898DeriveBytes(password.ConvertToUnsecureString(), salt, 4958);

            //return the first 32 bytes (256bits)
            return keyGen.GetBytes(32);
        }

        /// <summary>
        /// Encrypts the data using AES with a password
        /// </summary>
        /// <param name="data"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string Encrypt(string data, SecureString password, string saltString)
        {
            
            try
            {
                
                using (var aes = new RijndaelManaged())
                {
                    
                    //get some bytes
                    aes.Key = GenerateEncryptionKey(password, saltString);
                    aes.BlockSize = 256;
                    aes.Padding = PaddingMode.PKCS7;

                    //generate an IV
                    aes.GenerateIV();

                    //get the bytes for our message
                    var plainBytes = Encoding.UTF8.GetBytes(data);

                    //start up the encryption
                    using (var ms = new MemoryStream())
                    using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        //write the bytes to the cryptostream
                        cs.Write(plainBytes, 0, plainBytes.Length);
                        cs.FlushFinalBlock();

                        //get message bytes
                        var msgBytes = ms.ToArray();

                        //create a new array big enough for the both of 'em
                        var cypherBytes = new byte[aes.IV.Length + msgBytes.Length];

                        //return the string with the iv as the first 32 bytes. will need this when decrypting
                        Buffer.BlockCopy(aes.IV, 0, cypherBytes, 0, aes.IV.Length);
                        Buffer.BlockCopy(msgBytes, 0, cypherBytes, aes.IV.Length, msgBytes.Length);

                        //now convert it to base64 string
                        var cypherText = Convert.ToBase64String(cypherBytes);

                        //return cypher text
                        return cypherText;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>
        /// Decrypts data
        /// </summary>
        /// <param name="cypherData"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string Decrypt(string cypherData, SecureString password, string saltString)
        {
            //generate key
            byte[] key = GenerateEncryptionKey(password, saltString);
            //decrypt
            return Decrypt(cypherData, key);
        }
        public static string Decrypt(string cypherData, byte[] key)
        {
            
            try
            {

                using (var aes = new RijndaelManaged())
                {

                    //get some bytes
                    aes.Key = key;
                    aes.BlockSize = 256;
                    aes.Padding = PaddingMode.PKCS7;

                    //get the bytes for our message
                    var cypherBytes = Convert.FromBase64String(cypherData);
                    var iv = new byte[aes.IV.Length];
                    var msgBytes = new byte[cypherBytes.Length - iv.Length];

                    //we use the first 32 bytes of the cypherdata for the IV
                    Buffer.BlockCopy(cypherBytes, 0, iv, 0, iv.Length);
                    Buffer.BlockCopy(cypherBytes, iv.Length, msgBytes, 0, msgBytes.Length);

                    //set the IV for the instance
                    aes.IV = iv;

                    //start up the decryption
                    using (var ms = new MemoryStream())
                    using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        //write the bytes to the cryptostream
                        cs.Write(msgBytes, 0, msgBytes.Length);
                        cs.FlushFinalBlock();

                        //the plain text has been decrypted.
                        var plainText = System.Text.Encoding.UTF8.GetString(ms.ToArray());

                        //return plain text
                        return plainText;
                    }
                }
            }
            catch (Exception)
            {

                return null;
            }


        }

    }
}
