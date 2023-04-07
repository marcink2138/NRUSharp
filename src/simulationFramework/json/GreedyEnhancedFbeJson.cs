using Newtonsoft.Json;

namespace NRUSharp.simulationFramework.json{
    public record GreedyEnhancedFbeJson(
        [JsonProperty(PropertyName = "name")]
        string Name,
        [JsonProperty(PropertyName = "offset")]
        string Offset,
        [JsonProperty(PropertyName = "ffp")]
        string Ffp,
        [JsonProperty(PropertyName = "cot")]
        string Cot,
        [JsonProperty(PropertyName = "station_number")]
        string StationNumber,
        [JsonProperty(PropertyName = "q")]
        string Q
    );
}