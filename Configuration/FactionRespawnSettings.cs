namespace RespawnTokenCustomizer.Configuration
{
    using System.ComponentModel;

    public sealed class FactionRespawnSettings
    {
        [Description("Main-wave token settings for this faction.")]
        public FactionTokenSettings MainWave { get; set; } = new FactionTokenSettings();

        [Description("Miniwave token settings for this faction.")]
        public MiniWaveTokenSettings MiniWave { get; set; } = new MiniWaveTokenSettings();
    }
}
