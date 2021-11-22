using System;
using System.Collections.Generic;
using System.Linq;

using CMS.Helpers;

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
            var createdAt = IntercomConversationService.UnixTimeStampToDateTime(CreatedAt).ToString("r");

            return $"{createdAt} | {Source}{Environment.NewLine}{Environment.NewLine}{ConversationParts}";
        }
    }


    internal class ConversationSource
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("author")]
        public Author Author { get; set; }

        public override string ToString()
        {
            return $"{Author}:{Environment.NewLine}{HTMLHelper.StripTags(Body)}";
        }
    }


    internal class Author
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        public override string ToString()
        {
            return String.IsNullOrEmpty(Email) 
                    ? HTMLHelper.StripTags(Name) 
                    : HTMLHelper.StripTags($"{Name} ({Email})");
        }
    }


    internal class ConversationPartsWrapper
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("conversation_parts")]
        public List<ConversationPart> ConversationParts { get; set; }

        public override string ToString()
        {
            return String.Join($"{Environment.NewLine}{Environment.NewLine}", ConversationParts.Where(c => String.Equals("comment", c.PartType, StringComparison.OrdinalIgnoreCase)));
        }
    }


    internal class ConversationPart
    {
        [JsonProperty("created_at")]
        public long CreatedAt { get; set; }

        [JsonProperty("part_type")]
        public string PartType { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("author")]
        public Author Author { get; set; }

        public override string ToString()
        {
            var createdAt = IntercomConversationService.UnixTimeStampToDateTime(CreatedAt).ToString("r");

            return $"{createdAt} | {Author}:{Environment.NewLine}{HTMLHelper.StripTags(Body)}";
        }
    }
}