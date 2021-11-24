using System;

using CMS.Helpers;

using Newtonsoft.Json;

namespace Kentico.Xperience.Intercom
{
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
}