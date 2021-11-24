using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using CMS.Activities;
using CMS.Base;
using CMS.ContactManagement;
using CMS.Core;
using CMS.DataEngine;
using CMS.SiteProvider;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kentico.Xperience.Intercom
{
    /// <summary>
    /// Controller that provides endpoints for Intercom Series webhook calls.
    /// </summary>
    public class KenticoIntercomController : ControllerBase
    {
        private const string INTERCOM_SECURITY_HEADER = "X-Hub-Signature";

        private readonly IActivityLogService activityLogService;
        private readonly ISiteService siteService;
        private readonly ISettingsService settingsService;
        private readonly IIntercomConversationService intercomConversationService;


        /// <summary>
        /// Creates a new instance of <see cref="KenticoIntercomController"/>.
        /// </summary>
        public KenticoIntercomController(IActivityLogService activityLogService, ISiteService siteService, ISettingsService settingsService, IIntercomConversationService intercomConversationService)
        {
            this.activityLogService = activityLogService ?? throw new ArgumentNullException(nameof(activityLogService));
            this.siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            this.settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
            this.intercomConversationService = intercomConversationService ?? throw new ArgumentNullException(nameof(intercomConversationService));
        }


        /// <summary>
        /// Updates the contact's fields with the provided values.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UpdateContact()
        {
            var site = siteService.CurrentSite;

            if (site == null || !IsIntercomEnabled(site))
            {
                return NotFound();
            }

            var requestBody = await GetBodyFromRequestAsync(Request);
            VerifySecurityHeaders(requestBody, site);

            var contactData = JObject.Parse(requestBody);

            if (contactData == null)
            {
                return BadRequest("Contact data is missing.");
            }

            if (!contactData.TryGetValue("ContactGUID", StringComparison.OrdinalIgnoreCase, out var contactGuidToken))
            {
                return BadRequest("Contact identifier is not provided.");
            }

            if (!Guid.TryParse(contactGuidToken.Value<string>(), out var contactGuid))
            {
                return BadRequest("Contact identifier is in incorrect format.");
            }

            var contact = await ContactInfo.Provider.GetAsync(contactGuid) ?? VisitorToContactInfoProvider.GetContactForVisitor(contactGuid);

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
        /// Logs custom activity of the specified type.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> LogActivity()
        {
            var site = siteService.CurrentSite;

            if (site == null || !IsIntercomEnabled(site))
            {
                return NotFound();
            }

            var requestBody = await GetBodyFromRequestAsync(Request);

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

            var contact = await ContactInfo.Provider.GetAsync(contactGuid) ?? VisitorToContactInfoProvider.GetContactForVisitor(contactGuid);

            if (contact == null)
            {
                return BadRequest("Contact identifier is incorrect");
            }

            var conversationLinks = await intercomConversationService.GetConversationLinks(contact, site.SiteID);
            var activityComment = String.Join("<br/>", conversationLinks);

            var activity = new IntercomActivityInitializer(activityData.ActivityType, activityData.ActivityURL, activityData.ActivityValue, activityComment)
                    .WithSiteId(site.SiteID)
                    .WithContactId(contact.ContactID);

            activityLogService.LogWithoutModifiersAndFilters(activity);

            return Ok();
        }


        private void VerifySecurityHeaders(string requestBody, ISiteInfo site)
        {
            string securityHeader = null;
            if (Request.Headers.TryGetValue(INTERCOM_SECURITY_HEADER, out var securityHeaderValues))
            {
                securityHeader = securityHeaderValues;
            }

            // Intercom Series webhook requests do not include X-Hub-Signature header at the moment - this is a confirmed bug on their side.
            // We use temporary API key to protect the endpoints.
            if (!String.IsNullOrEmpty(securityHeader))
            {
                SecurityMethods.VerifySignature(requestBody, securityHeader, site);
            }
            else
            {
                VerifyTemporaryAPIKey(site);
            }
        }


        private void VerifyTemporaryAPIKey(ISiteInfo site)
        {
            var currentApiKey = settingsService[$"{site.SiteName}.CMSIntercomAPIKey"];

            string apiKeyHeader;
            if (Request.Headers.TryGetValue("XperienceApiKey", out var apiKeyHeaderValues))
            {
                apiKeyHeader = apiKeyHeaderValues.FirstOrDefault();
            }
            else
            {
                throw new InvalidOperationException("Missing 'XperienceApiKey' header.");
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


        private bool IsIntercomEnabled(ISiteInfo site)
        {
            return settingsService[$"{site.SiteName}.CMSIntercomEnabled"].ToBoolean(false);
        }


        /// <summary>
        /// Gets the current body from http request.
        /// </summary>
        /// <param name="httpRequest">The current http request.</param>
        private static async Task<string> GetBodyFromRequestAsync(HttpRequest httpRequest)
        {
            using (var reader = new StreamReader(httpRequest.Body))
            {
                var body = await reader.ReadToEndAsync();

                return body;
            }
        }
    }
}
