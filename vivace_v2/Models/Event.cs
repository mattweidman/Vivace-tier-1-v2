﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace vivace.Models
{
    public class Event : ModelVivace
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "band")]
        public string Band { get; set; }

        [JsonProperty(PropertyName = "users")]
        public IEnumerable<string> Users { get; set; }

        [JsonProperty(PropertyName = "songs")]
        public IEnumerable<string> Songs { get; set; }

        [JsonProperty(PropertyName = "currentsong")]
        public string CurrentSong { get; set; }
    }
}
