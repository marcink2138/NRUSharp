using Newtonsoft.Json;

namespace NRUSharp.simulationFramework.json{
    public record RandomMutingFbeJson : BaseFbeParamsJson{
        [JsonProperty(PropertyName = "max_muted_periods")]
        public string MaxMutedPeriods{ get; set; }

        [JsonProperty(PropertyName = "max_transmission_periods")]
        public string MaxTransmissionPeriod{ get; set; }
    }
}