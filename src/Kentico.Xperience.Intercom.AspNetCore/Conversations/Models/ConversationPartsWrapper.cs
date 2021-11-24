using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

namespace Kentico.Xperience.Intercom
{
    internal class ConversationPartsWrapper
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("conversation_parts")]
        public List<ConversationPart> ConversationParts { get; set; }

        public override string ToString()
        {
            return String.Join($"{Environment.NewLine}{Environment.NewLine}", ConversationParts.Where(c => !String.IsNullOrWhiteSpace(c.Body) && String.Equals("comment", c.PartType, StringComparison.OrdinalIgnoreCase)));
        }
    }
}