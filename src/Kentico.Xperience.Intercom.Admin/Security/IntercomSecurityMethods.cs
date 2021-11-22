using System;
using System.Security.Cryptography;
using System.Text;

using CMS.DataEngine;
using CMS.SiteProvider;

namespace Kentico.Xperience.Intercom.Admin
{
    public static class IntercomSecurityMethods
    {
        public static string GenerateAPIKey(int keyLength = 16)
        {
            var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            byte[] randomBytes = new byte[keyLength];
            rngCryptoServiceProvider.GetBytes(randomBytes);

            return Convert.ToBase64String(randomBytes);
        }
    }
}
