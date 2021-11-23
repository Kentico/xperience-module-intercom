using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using CMS.ContactManagement;
using CMS.Core;
using CMS.DataEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kentico.Xperience.Intercom
{
    internal class IntercomConversationService : IIntercomConversationService
    {
        private const string SEARCH_CONVERSATIONS_URL = "https://api.intercom.io/conversations/search";
        private const string SEARCH_CONTACTS_URL = "https://api.intercom.io/contacts/search";
        private const string GET_SPECIFIC_CONVERSATION_URL_FORMAT = "https://api.intercom.io/conversations/{0}";
        private const string LINK_TO_CONVERSATION_URL_FORMAT = "https://app.intercom.com/a/apps/{0}/inbox/inbox/conversation/{1}";

        private readonly IHttpClientFactory httpClientFactory;
        private readonly IEventLogService eventLogService;


        public IntercomConversationService(IHttpClientFactory httpClientFactory, IEventLogService eventLogService)
        {
            this.httpClientFactory = httpClientFactory;
            this.eventLogService = eventLogService;
        }


        public async Task<string> GetConversationHistory(ContactInfo contact, SiteInfoIdentifier siteIdentifier)
        {
            if (contact == null)
            {
                throw new ArgumentNullException(nameof(contact));
            }

            if (siteIdentifier == null)
            {
                throw new ArgumentNullException(nameof(siteIdentifier));
            }

            try
            {
                var conversationIDs = await GetConversationIDs(contact, siteIdentifier);

                var sb = new StringBuilder();
                bool skipSeparator = true;
                foreach (var conversationID in conversationIDs)
                {
                    if (conversationIDs.Count > 1 && !skipSeparator)
                    {
                        sb.AppendLine();
                        sb.AppendLine("----------------------------");
                        sb.AppendLine();
                    }
                    skipSeparator = false;

                    sb.AppendLine(await GetConversationText(conversationID, siteIdentifier));
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                eventLogService.LogException("Intercom", "GETCONVERSATIONSHISTORY", ex, additionalMessage: $"Could not load conversation history for contact '{contact.ContactGUID}' due to unexpected exception.");
                return null;
            }
        }


        public async Task<IEnumerable<string>> GetConversationLinks(ContactInfo contact, SiteInfoIdentifier siteIdentifier)
        {
            if (contact == null)
            {
                throw new ArgumentNullException(nameof(contact));
            }

            if (siteIdentifier == null)
            {
                throw new ArgumentNullException(nameof(siteIdentifier));
            }

            var conversationIDs = await GetConversationIDs(contact, siteIdentifier);
            var intercomAppID = SettingsKeyInfoProvider.GetValue($"{siteIdentifier.ObjectCodeName}.CMSIntercomAppID");

            return conversationIDs.Select(id => String.Format(LINK_TO_CONVERSATION_URL_FORMAT, intercomAppID, id));
        }


        private async Task<string> GetConversationText(string conversationId, SiteInfoIdentifier siteIdentifier)
        {
            if (String.IsNullOrEmpty(conversationId))
            {
                throw new ArgumentException("Conversation ID cannot be empty", nameof(conversationId));
            }

            if (siteIdentifier == null)
            {
                throw new ArgumentNullException(nameof(siteIdentifier));
            }

            var accessToken = SettingsKeyInfoProvider.GetValue($"{siteIdentifier.ObjectCodeName}.CMSIntercomOAuthToken");

            if (String.IsNullOrEmpty(accessToken))
            {
                LogWarning("GETCONVERSATION", $"Could not retrieve Intercom conversation. Missing intercom access token for site {siteIdentifier.ObjectCodeName}.");
                return null;
            }

            var client = httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            client.DefaultRequestHeaders.Add("Accept", $"application/json");

            var requestUrl = String.Format(GET_SPECIFIC_CONVERSATION_URL_FORMAT, conversationId);

            var responseMessage = await client.GetAsync(requestUrl);
            var responseBody = await responseMessage.Content.ReadAsStringAsync();

            if (!responseMessage.IsSuccessStatusCode)
            {
                LogWarning("GETCONVERSATION", $"Could not retrieve Intercom conversation.{Environment.NewLine}Response status: {responseMessage.StatusCode}.{Environment.NewLine}Error details:{Environment.NewLine}{responseBody}");
                return null;
            }

            var conversation = JsonConvert.DeserializeObject<IntercomConversation>(responseBody, new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore });

            return conversation.ToString();
            
        }


        private async Task<IList<string>> GetConversationIDs(ContactInfo contact, SiteInfoIdentifier siteIdentifier)
        {
            if (contact == null)
            {
                throw new ArgumentNullException(nameof(contact));
            }

            if (siteIdentifier == null)
            {
                throw new ArgumentNullException(nameof(siteIdentifier));
            }

            var accessToken = SettingsKeyInfoProvider.GetValue($"{siteIdentifier.ObjectCodeName}.CMSIntercomOAuthToken");

            if (String.IsNullOrEmpty(accessToken))
            {
                LogWarning("GETCONVERSATIONS", $"Could not retrieve Intercom conversations. Missing intercom access token for site {siteIdentifier.ObjectCodeName}.");
                return new List<string>();
            }

            var client = httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            client.DefaultRequestHeaders.Add("Accept", $"application/json");
              
            var contactFilter = new IntercomWhereEqualsFilter("external_id", contact.ContactGUID.ToString());

            var contactFilterContent = new StringContent(JsonConvert.SerializeObject(contactFilter), Encoding.UTF8, "application/json");
            var contactResponseMessage = await client.PostAsync(SEARCH_CONTACTS_URL, contactFilterContent);
            var contactResponseBody = await contactResponseMessage.Content.ReadAsStringAsync();

            var contactResponse = JsonConvert.DeserializeObject<JObject>(contactResponseBody);

            if (!contactResponseMessage.IsSuccessStatusCode)
            {
                LogWarning("GETCONTACT", $"Could not retrieve Intercom contact for Xperience contact '{contact.ContactGUID}'.{Environment.NewLine}Response status: {contactResponseMessage.StatusCode}.{Environment.NewLine}Error details:{Environment.NewLine}{contactResponseBody}");
                return new List<string>();
            }

            var intercomContactID = (string)contactResponse.SelectToken("data[0].id");

            var conversationFilter = new IntercomWhereEqualsFilter("contact_ids", intercomContactID);
            var conversationFilterContent = new StringContent(JsonConvert.SerializeObject(conversationFilter), Encoding.UTF8, "application/json");
            var conversationsResponseMessage = await client.PostAsync(SEARCH_CONVERSATIONS_URL, conversationFilterContent);
            var conversationsResponseBody = await conversationsResponseMessage.Content.ReadAsStringAsync();
            var conversationsResponse = JsonConvert.DeserializeObject<JObject>(conversationsResponseBody);

            if (!conversationsResponseMessage.IsSuccessStatusCode)
            {
                LogWarning("GETCONVERSATIONS", $"Could not retrieve conversations for Xperience contact '{contact.ContactGUID}'.{Environment.NewLine}Response status: {conversationsResponseMessage.StatusCode}.{Environment.NewLine}Error details:{Environment.NewLine}{conversationsResponseBody}");
                return new List<string>();
            }

            return conversationsResponse["conversations"].Select(c => (string)c["id"]).ToList();
            
        }


        private void LogWarning(string eventCode, string message)
        {
            eventLogService.LogWarning("INTERCOM", eventCode, message);
        }
    }
}
