using System;
using System.Web.Mvc;
using System.Text;
using System.Web;

using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.SiteProvider;

namespace Kentico.Xperience.Intercom
{
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Registers Intercom scripts according to the current site's Intercom configuration.
        /// </summary>
        public static MvcHtmlString IntercomScripts(this HtmlHelper _)
        {
            var currentSite = SiteContext.CurrentSite;
            if (currentSite == null)
            {
                return MvcHtmlString.Empty;
            }

            var intercomEnabled = SettingsKeyInfoProvider.GetBoolValue($"{SiteContext.CurrentSiteName}.CMSIntercomEnabled");
            var intercomAppID = SettingsKeyInfoProvider.GetValue($"{SiteContext.CurrentSiteName}.CMSIntercomAppID");

            if (!intercomEnabled || String.IsNullOrEmpty(intercomAppID))
            {
                return MvcHtmlString.Empty;
            }

            var currentContact = ContactManagementContext.CurrentContact;
            string contactName = null;

            if (currentContact != null && !currentContact.ContactLastName.StartsWith(ContactHelper.ANONYMOUS, StringComparison.Ordinal))
            {
                contactName = $"{currentContact.ContactFirstName} {currentContact.ContactLastName}".Trim();
            }

            var sb = new StringBuilder();

            sb.AppendFormat(@"<script>
window.intercomSettings = {{
    app_id: ""{0}""", intercomAppID);

            if (currentContact != null)
            {
                sb.AppendFormat(@",
    user_id: ""{0}""", currentContact.ContactGUID.ToString());
            }

            if (!String.IsNullOrEmpty(contactName))
            {
                sb.AppendFormat(@",
    name: ""{0}""", HttpUtility.JavaScriptStringEncode(contactName));
            }

            if (!String.IsNullOrEmpty(currentContact?.ContactEmail))
            {
                sb.AppendFormat(@",
    email: ""{0}""", HttpUtility.JavaScriptStringEncode(currentContact.ContactEmail));
            }

            var identityVerificationSecret = SettingsKeyInfoProvider.GetValue($"{SiteContext.CurrentSiteName}.CMSIntercomIdentityVerificationSecret");
            if (currentContact != null && !String.IsNullOrEmpty(identityVerificationSecret))
            {
                sb.AppendFormat(@",
    user_hash: ""{0}""", SecurityMethods.CalculateHash(currentContact.ContactGUID.ToString(), identityVerificationSecret));
            }

            sb.AppendLine("};");

            sb.AppendFormat("(function () {{ var w = window; var ic = w.Intercom; if (typeof ic === \"function\") {{ ic('reattach_activator'); ic('update', w.intercomSettings); }} else {{ var d = document; var i = function () {{ i.c(arguments); }}; i.q = []; i.c = function (args) {{ i.q.push(args); }}; w.Intercom = i; var l = function () {{ var s = d.createElement('script'); s.type = 'text/javascript'; s.async = true; s.src = 'https://widget.intercom.io/widget/{0}'; var x = d.getElementsByTagName('script')[0]; x.parentNode.insertBefore(s, x); }}; if (w.attachEvent) {{ w.attachEvent('onload', l); }} else {{ w.addEventListener('load', l, false); }} }} }})();", intercomAppID);

            sb.AppendLine("</script>");

            return MvcHtmlString.Create(sb.ToString());
        }
    }
}
