using System;

using Newtonsoft.Json;

namespace Kentico.Xperience.Intercom
{
    internal class IntercomConversation
    {
        [JsonProperty("source")]
        public ConversationSource Source { get; set; }

        [JsonProperty("created_at")]
        public int CreatedAt { get; set; }

        [JsonProperty("conversation_parts")]
        public ConversationPartsWrapper ConversationParts { get; set; }

        public override string ToString()
        {
            var createdAt = DateTimeHelper.UnixTimeStampToDateTime(CreatedAt).ToString("r");

            return $"{createdAt} | {Source}{Environment.NewLine}{Environment.NewLine}{ConversationParts}";
        }
    }
}