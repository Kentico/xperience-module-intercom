using System;

using CMS.Helpers;

using Newtonsoft.Json;

namespace Kentico.Xperience.Intercom
{
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
            var createdAt = DateTimeHelper.UnixTimeStampToDateTime(CreatedAt).ToString("r");

            return $"{createdAt} | {Author}:{Environment.NewLine}{HTMLHelper.StripTags(Body)}";
        }
    }
}