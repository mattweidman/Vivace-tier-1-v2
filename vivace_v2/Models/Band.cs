using Newtonsoft.Json;
using System.Collections.Generic;

namespace vivace.Models
{
    public class Band : ModelVivace
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "users")]
        public IEnumerable<string> Users { get; set; }

        [JsonProperty(PropertyName = "songs")]
        public IEnumerable<string> Songs { get; set; }

        [JsonProperty(PropertyName = "events")]
        public IEnumerable<string> Events { get; set; }
    }
}
