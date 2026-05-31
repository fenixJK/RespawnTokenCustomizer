namespace RespawnTokenCustomizer.Configuration
{
    using System.Collections.Generic;
    using System.ComponentModel;

    public sealed class FactionTokenSettings
    {
        [Description("How many main-wave respawn tokens this faction starts each round with.")]
        public int StartingTokens { get; set; } = 1;

        [Description("How many extra main-wave respawn tokens this faction can earn in PerFaction mode.")]
        public int EarnableTokens { get; set; } = 3;

        [Description("Influence thresholds that grant earned tokens. Used in both Shared and PerFaction modes.")]
        public List<int> MilestoneThresholds { get; set; } = new List<int>
        {
            40,
            80,
            150,
            200,
        };
    }
}
