﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace vivace.Models
{
    [DataContract]
    public class Event : ModelVivace
    {
        [DataMember, JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [DataMember, JsonProperty(PropertyName = "band")]
        public string Band { get; set; }

        [DataMember, JsonProperty(PropertyName = "users")]
        public IEnumerable<string> Users { get; set; }

        [DataMember, JsonProperty(PropertyName = "songs")]
        public IEnumerable<string> Songs { get; set; }

        [DataMember, JsonProperty(PropertyName = "currentsong")]
        public string CurrentSong { get; set; }
    }
}
