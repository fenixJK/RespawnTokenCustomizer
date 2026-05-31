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

        [Description("Shared uses global main-wave earned-token and miniwave unlock values. PerFaction uses each faction's own values.")]
        public EarnedTokenPoolMode EarnedTokenPoolMode { get; set; } = EarnedTokenPoolMode.Shared;

        [Description("How many earned tokens are available in Shared mode. Both NTF and Chaos compete for this one pool.")]
        public int SharedEarnableTokens { get; set; } = 2;

        [Description("How many miniwave tokens are given on unlock in Shared mode. Both NTF and Chaos use this value.")]
        public int SharedMiniWaveUnlockTokens { get; set; } = 1;

        [Description("If true, running respawntokensreload also resets current live wave tokens to the configured starting tokens.")]
        public bool ResetCurrentTokensOnReload { get; set; } = false;

        [Description("When more milestone thresholds are needed, each generated threshold is this many influence points after the previous one.")]
        public int ExtraMilestoneStep { get; set; } = 50;

        [Description("Nine-Tailed Fox / Foundation Staff respawn token settings.")]
        public FactionRespawnSettings NineTailedFox { get; set; } = new FactionRespawnSettings();

        [Description("Chaos Insurgency / Foundation Enemy respawn token settings.")]
        public FactionRespawnSettings ChaosInsurgency { get; set; } = new FactionRespawnSettings();
    }
}
