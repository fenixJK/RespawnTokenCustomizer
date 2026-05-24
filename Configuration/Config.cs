namespace RespawnTokenCustomizer.Configuration
{
    using System.ComponentModel;
    using Exiled.API.Interfaces;

    public sealed class Config : IConfig
    {
        [Description("Whether this plugin is enabled.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Whether debug messages should be printed to the server console.")]
        public bool Debug { get; set; } = false;

        [Description("If true, replaces the vanilla shared earned-token pool with per-faction earned-token limits below.")]
        public bool OverrideEarnedTokenPool { get; set; } = true;

        [Description("If true, running respawntokensreload also resets current live wave tokens to the configured starting tokens.")]
        public bool ResetCurrentTokensOnReload { get; set; } = false;

        [Description("When more milestone thresholds are needed, each generated threshold is this many influence points after the previous one.")]
        public int ExtraMilestoneStep { get; set; } = 50;

        [Description("Nine-Tailed Fox / Foundation Staff main-wave token settings.")]
        public FactionTokenSettings NineTailedFox { get; set; } = new FactionTokenSettings();

        [Description("Chaos Insurgency / Foundation Enemy main-wave token settings.")]
        public FactionTokenSettings ChaosInsurgency { get; set; } = new FactionTokenSettings();
    }
}
