using Newtonsoft.Json;

namespace NRUSharp.simulationFramework.json{
    public record BaseFbeParamsJson{
        [field: JsonProperty(PropertyName = "name")]
        public string Name{ get; set; }

        [field: JsonProperty(PropertyName = "offset")]
        public string Offset{ get; set; }

        [JsonProperty(PropertyName = "ffp")] 
        public string Ffp{ get; set; }

        [JsonProperty(PropertyName = "cot")] 
        public string Cot{ get; set; }

        [JsonProperty(PropertyName = "station_number")]
        private string StationNumber{ get; set; }
    }
}