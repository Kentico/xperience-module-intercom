using System;
using System.Web;

using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.SiteProvider;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Kentico.Xperience.Intercom
{
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Registers Intercom scripts according to the current site's Intercom configuration.
        /// </summary>
        public static IHtmlContent IntercomScripts(this IHtmlHelper _)
        {
            var currentSite = SiteContext.CurrentSite;
            if (currentSite == null)
            {
                new HtmlContentBuilder();
            }

            var intercomEnabled = SettingsKeyInfoProvider.GetBoolValue($"{SiteContext.CurrentSiteName}.CMSIntercomEnabled");
            var intercomAppID = SettingsKeyInfoProvider.GetValue($"{SiteContext.CurrentSiteName}.CMSIntercomAppID");

            if (!intercomEnabled || String.IsNullOrEmpty(intercomAppID))
            {
                new HtmlContentBuilder();
            }

            var currentContact = ContactManagementContext.CurrentContact;
            string contactName = null;

            if (currentContact != null && !currentContact.ContactLastName.StartsWith(ContactHelper.ANONYMOUS, StringComparison.Ordinal))
            {
                contactName = $"{currentContact.ContactFirstName} {currentContact.ContactLastName}".Trim();
            }

            var generatedHtml = new HtmlContentBuilder().AppendFormat(@"<script>
window.intercomSettings = {{
    app_id: ""{0}""", intercomAppID);

            if (currentContact != null)
            {
                generatedHtml.AppendFormat(@",
    user_id: ""{0}""", currentContact.ContactGUID.ToString());
            }

            if (!String.IsNullOrEmpty(contactName))
            {
                generatedHtml.AppendFormat(@",
    name: ""{0}""", HttpUtility.JavaScriptStringEncode(contactName));
            }

            if (!String.IsNullOrEmpty(currentContact?.ContactEmail))
            {
                generatedHtml.AppendFormat(@",
    email: ""{0}""", HttpUtility.JavaScriptStringEncode(currentContact.ContactEmail));
            }

            var identityVerificationSecret = SettingsKeyInfoProvider.GetValue($"{SiteContext.CurrentSiteName}.CMSIntercomIdentityVerificationSecret");
            if (currentContact != null && !String.IsNullOrEmpty(identityVerificationSecret))
            {
                generatedHtml.AppendFormat(@",
    user_hash: ""{0}""", SecurityMethods.CalculateHash(currentContact.ContactGUID.ToString(), identityVerificationSecret));
            }


            generatedHtml.AppendLine("};");

            generatedHtml.AppendFormat("(function () {{ var w = window; var ic = w.Intercom; if (typeof ic === \"function\") {{ ic('reattach_activator'); ic('update', w.intercomSettings); }} else {{ var d = document; var i = function () {{ i.c(arguments); }}; i.q = []; i.c = function (args) {{ i.q.push(args); }}; w.Intercom = i; var l = function () {{ var s = d.createElement('script'); s.type = 'text/javascript'; s.async = true; s.src = 'https://widget.intercom.io/widget/{0}'; var x = d.getElementsByTagName('script')[0]; x.parentNode.insertBefore(s, x); }}; if (w.attachEvent) {{ w.attachEvent('onload', l); }} else {{ w.addEventListener('load', l, false); }} }} }})();", intercomAppID);

            generatedHtml.AppendHtmlLine("</script>");

            return generatedHtml;
        }
    }
}
