using Newtonsoft.Json;
using System.Collections.Generic;

namespace vivace.Models
{
    public class Song : ModelVivace
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "parts")]
        public IEnumerable<string> Parts { get; set; }
    }
}
