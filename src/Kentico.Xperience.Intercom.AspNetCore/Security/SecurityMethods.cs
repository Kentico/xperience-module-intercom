using System;
using System.Security.Cryptography;
using System.Text;

using CMS.Base;
using CMS.DataEngine;

namespace Kentico.Xperience.Intercom
{
    internal static class SecurityMethods
    {
        internal static void VerifySignature(string requestBody, string providedSignature, ISiteInfo site)
        {
            var clientSecret = SettingsKeyInfoProvider.GetValue($"{site.SiteName}.CMSIntercomClientSecret");

            if (String.IsNullOrWhiteSpace(clientSecret))
            {
                throw new InvalidOperationException($"Intercom client secret is not configured for site '{site.SiteName}'");
            }

            if (String.IsNullOrWhiteSpace(providedSignature))
            {
                throw new InvalidOperationException($"Missing signature.");
            }

            var calculatedSignature = "sha1=" + CalculateHash(requestBody, clientSecret);

            if (!String.Equals(calculatedSignature, providedSignature))
            {
                throw new InvalidOperationException("The provided signature is invalid.");
            }
        }


        public static string CalculateHash(string message, string secret)
        {
            var encoding = new ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha1 = new HMACSHA1(keyByte))
            {
                byte[] hashmessage = hmacsha1.ComputeHash(messageBytes);
                var sb = new StringBuilder();
                for (var i = 0; i <= hashmessage.Length - 1; i++)
                {
                    sb.Append(hashmessage[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
