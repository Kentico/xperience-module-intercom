using System;

using CMS;
using CMS.DataEngine;
using CMS.SiteProvider;

using Kentico.Xperience.Intercom.Admin;

[assembly: RegisterModule(typeof(KXIntercomAdminModule))]

namespace Kentico.Xperience.Intercom.Admin
{
    public class KXIntercomAdminModule : Module
    {
        public KXIntercomAdminModule() : base("Kentico.Xperience.Intercom.Admin")
        {
        }

        protected override void OnInit()
        {
            base.OnInit();

            EnsureAPIKeysForSites();
        }


        /// <summary>
        /// Ensures API key that provides access to webhook API endpoints.
        /// This API key is temporary until the X-Hub-Signature header is fixed on the Intercom side.
        /// </summary>
        private static void EnsureAPIKeysForSites()
        {
            var siteNames = SiteInfo.Provider.Get().Column("SiteName").GetListResult<string>();

            foreach (var siteName in siteNames)
            {
                if (String.IsNullOrEmpty(SettingsKeyInfoProvider.GetValue($"{siteName}.CMSIntercomAPIKey")))
                {
                    SettingsKeyInfoProvider.SetValue("CMSIntercomAPIKey", siteName, IntercomSecurityMethods.GenerateAPIKey());
                }
            }
        }
    }
}
