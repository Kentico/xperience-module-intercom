using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

using CMS.Activities;
using CMS.ContactManagement;
using CMS.Core;
using CMS.DataEngine;
using CMS.SiteProvider;
using CMS.WebApi;

using Kentico.Xperience.Intercom.Admin;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[assembly: RegisterCMSApiController(typeof(IntercomController), RequiresSessionState = false)]

namespace Kentico.Xperience.Intercom.Admin
{
    public class IntercomController : CMSApiController
    {
        private const string INTERCOM_SECURITY_HEADER = "X-Hub-Signature";


        /// <summary>
        /// Updates the contact data with provided <paramref name="contactData"/> details.
        /// </summary>
        [HttpPost]
        public async Task<IHttpActionResult> UpdateContact()
        {
            var site = SiteContext.CurrentSite;

            if (site == null)
            {
                return NotFound();
            }

            if (!IsIntercomEnabled(site))
            {
                return BadRequest("Intercom integration is disabled.");
            }

            var requestBody = await Request.Content.ReadAsStringAsync();
            VerifySecurityHeaders(requestBody, site);

            var contactData = JObject.Parse(requestBody);

            if (contactData == null)
            {
                return BadRequest("Contact data is missing.");
            }

            if (!contactData.TryGetValue("ContactGUID", StringComparison.OrdinalIgnoreCase, out var contactGuidToken))
            {
                return BadRequest("Contact identifier is not provided");
            }

            if (!Guid.TryParse(contactGuidToken.Value<string>(), out var contactGuid))
            {
                return BadRequest("Contact identifier is in incorrect format.");
            }

            var contact = ContactInfo.Provider.Get(contactGuid) ?? VisitorToContactInfoProvider.GetContactForVisitor(contactGuid);

            if (contact == null)
            {
                return BadRequest("Contact not found.");
            }

            ContactFieldMapper.MapDefaultContactFields(contactData, contact);

            IntercomEvents.UpdateContact.StartEvent(new IntercomUpdateContactEventArgs(contactData, contact));

            ContactInfo.Provider.Set(contact);

            return Ok();
        }


        /// <summary>
        /// Log the activity of the specified type.
        /// </summary>
        [HttpPost]
        public async Task<IHttpActionResult> LogActivity()
        {
            var site = SiteContext.CurrentSite;

            if (site == null)
            {
                return NotFound();
            }

            if (!IsIntercomEnabled(site))
            {
                return BadRequest("Intercom integration is disabled.");
            }

            var requestBody = await Request.Content.ReadAsStringAsync();

            VerifySecurityHeaders(requestBody, site);

            var activityData = JsonConvert.DeserializeObject<ActivityDTO>(requestBody, new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore });

            if (activityData == null)
            {
                return BadRequest("Activity data is missing.");
            }

            if (String.IsNullOrEmpty(activityData.ActivityType))
            {
                return BadRequest("Activity type is missing.");
            }

            if (!ActivityTypeInfoProvider.GetActivityTypeEnabled(activityData.ActivityType))
            {
                return BadRequest("Activity type is incorrect or disabled.");
            }

            if (!Guid.TryParse(activityData.ContactGuid, out var contactGuid))
            {
                return BadRequest("Contact identifier is in incorrect format.");
            }

            var contact = ContactInfo.Provider.Get(contactGuid) ?? VisitorToContactInfoProvider.GetContactForVisitor(contactGuid);

            if (contact == null)
            {
                return BadRequest("Contact identifier is incorrect");
            }

            string conversationHistory = await IntercomConversationHelper.GetConversationHistory(contact, site.SiteID);

            var activity = new IntercomActivityInitializer(activityData.ActivityType, activityData.ActivityURL, activityData.ActivityValue, conversationHistory)
                    .WithSiteId(site.SiteID)
                    .WithContactId(contact.ContactID);

            Service.Resolve<IActivityLogService>().LogWithoutModifiersAndFilters(activity);

            return Ok();
        }


        private void VerifySecurityHeaders(string requestBody, SiteInfo site)
        {
            string securityHeader = null;
            if (Request.Headers.TryGetValues(INTERCOM_SECURITY_HEADER, out var securityHeaderValues))
            {
                securityHeader = securityHeaderValues.FirstOrDefault();
            }

            if (!String.IsNullOrEmpty(securityHeader))
            {
                // TODO: remove if and validate always after Intercom bugfix
                IntercomSecurityMethods.VerifySignature(requestBody, securityHeader, site);
            }
            else
            {
                // Temporary else branch
                VerifyTemporaryAPIKey(site);
            }
        }


        private void VerifyTemporaryAPIKey(SiteInfo site)
        {
            var currentApiKey = SettingsKeyInfoProvider.GetValue($"{site.SiteName}.CMSIntercomAPIKey");

            string apiKeyHeader;
            if (Request.Headers.TryGetValues("XperienceApiKey", out var apiKeyHeaderValues))
            {
                apiKeyHeader = apiKeyHeaderValues.FirstOrDefault();
            }
            else
            {
                throw new InvalidOperationException("Missing Xperience security header.");
            }

            if (String.IsNullOrEmpty(currentApiKey))
            {
                throw new InvalidOperationException($"Intercom API key is not configured on site '{SiteContext.CurrentSite?.SiteName}'.");
            }

            if (!String.Equals(currentApiKey, apiKeyHeader, StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"Provided API Key is incorrect or outdated.");
            }
        }


        private bool IsIntercomEnabled(SiteInfo site)
        {
            return SettingsKeyInfoProvider.GetBoolValue($"{site.SiteName}.CMSIntercomEnabled");
        }
    }
}
