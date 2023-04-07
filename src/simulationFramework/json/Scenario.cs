using System.Collections.Generic;
using Newtonsoft.Json;

namespace NRUSharp.simulationFramework.json{
    public record Scenario(
        [JsonProperty(PropertyName = "standard_fbe")]
        List<StandardFbeJson> StandardFbe,
        [JsonProperty(PropertyName = "fixed_muting_fbe")]
        List<FixedMutingFbeJson> FixedMutingFbe,
        [JsonProperty(PropertyName = "random_muting_fbe")]
        List<RandomMutingFbeJson> RandomMutingFbe,
        [JsonProperty(PropertyName = "floating_fbe")]
        List<FloatingFbeJson> FloatingFbe,
        [JsonProperty(PropertyName = "greedy_enhanced_fbe")]
        List<GreedyEnhancedFbeJson> GreedyEnhancedFbe,
        [JsonProperty(PropertyName = "enhanced_fbe")]
        List<EnhancedFbeJson> EnhancedFbe
    );
}