using System;

using CMS.Helpers;

using Newtonsoft.Json;

namespace Kentico.Xperience.Intercom
{
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
}