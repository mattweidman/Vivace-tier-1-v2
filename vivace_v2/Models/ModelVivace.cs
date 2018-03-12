using Newtonsoft.Json;

namespace vivace.Models
{
    /// <summary>
    /// Superclass of models in Vivace
    /// </summary>
    public class ModelVivace
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }
}
