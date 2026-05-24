namespace RespawnTokenCustomizer.Configuration
{
    using System.Collections.Generic;
    using System.ComponentModel;

    public sealed class FactionTokenSettings
    {
        [Description("How many main-wave respawn tokens this faction starts each round with.")]
        public int StartingTokens { get; set; } = 1;

        [Description("How many extra main-wave respawn tokens this faction can earn from objective milestones during a round.")]
        public int EarnableTokens { get; set; } = 3;

        [Description("Influence thresholds that grant earned tokens. Extra thresholds are generated if EarnableTokens is higher than this list.")]
        public List<int> MilestoneThresholds { get; set; } = new List<int>
        {
            40,
            80,
            150,
            200,
        };
    }
}
