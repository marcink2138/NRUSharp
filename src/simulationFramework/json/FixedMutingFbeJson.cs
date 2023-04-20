using Newtonsoft.Json;

namespace NRUSharp.simulationFramework.json{
    public record FixedMutingFbeJson : BaseFbeParamsJson{
        [JsonProperty(PropertyName = "muted_periods")]
        public string MutedPeriods{ get; set; }
    }
}