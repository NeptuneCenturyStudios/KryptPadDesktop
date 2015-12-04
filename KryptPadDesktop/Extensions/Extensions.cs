using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace KryptPadDesktop.Extensions
{
    public static class Extensions
    {
        /// <summary>
        /// Converts a string to a secure string
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static SecureString ConvertToSecureString(this string password)
        {
            if (password == null)
                throw new ArgumentNullException("password");

            unsafe
            {
                //create a fixed pointer of char array from the password string
                fixed (char* passwordChars = password)
                {
                    var securePassword = new SecureString(passwordChars, password.Length);

                    //prevent the secure string from being manipulated any more
                    securePassword.MakeReadOnly();

                    //return it
                    return securePassword;
                }
            }
        }

        /// <summary>
        /// Converts a secure string to a not-so-secure string
        /// </summary>
        /// <param name="securePassword"></param>
        /// <returns></returns>
        public static string ConvertToUnsecureString(this SecureString securePassword)
        {
            if (securePassword == null)
                throw new ArgumentNullException("securePassword");

            //create an unmanaged char array
            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                //get the secure string as a char*
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);

                //return it as a string obj
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                //destroy it!
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

    }
}
