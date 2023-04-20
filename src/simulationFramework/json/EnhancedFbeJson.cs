using Newtonsoft.Json;

namespace NRUSharp.simulationFramework.json{
    public record EnhancedFbeJson : BaseFbeParamsJson{
        [JsonProperty(PropertyName = "q")] public string Q{ get; set; }
    }
}