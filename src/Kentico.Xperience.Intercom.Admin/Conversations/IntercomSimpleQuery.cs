using Newtonsoft.Json;

namespace Kentico.Xperience.Intercom.Admin
{
    internal class IntercomSimpleQuery
    {
        [JsonProperty("field")]
        public string Field { get; set; }

        [JsonProperty("operator")]
        public string Operator { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
