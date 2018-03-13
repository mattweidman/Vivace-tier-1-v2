using Newtonsoft.Json;
using System.Collections.Generic;

namespace vivace.Models
{
    public class Part : ModelVivace
    {
        [JsonProperty(PropertyName = "instrument")]
        public string Instrument { get; set; }

        [JsonProperty(PropertyName = "song")]
        public string Song { get; set; }

        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }
    }
}
