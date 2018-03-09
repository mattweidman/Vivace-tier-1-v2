using Newtonsoft.Json;
using System.Collections.Generic;

namespace vivace.Models
{
    public class User
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "bands")]
        public IEnumerable<string> Bands { get; set; }

        [JsonProperty(PropertyName = "events")]
        public IEnumerable<string> Events { get; set; }
    }
}
