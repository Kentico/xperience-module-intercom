using Newtonsoft.Json;

namespace Kentico.Xperience.Intercom.Admin
{
    internal class IntercomWhereEqualsFilter
    {
        [JsonProperty("query")]
        public IntercomSimpleQuery Query { get; set; }


        public IntercomWhereEqualsFilter(string field, string value)
        {
            Query = new IntercomSimpleQuery
            {
                Field = field,
                Operator = "=",
                Value = value
            };
        }
    }
}
