namespace RespawnTokenCustomizer.Configuration
{
    using System.ComponentModel;

    public sealed class MiniWaveTokenSettings
    {
        [Description("How many miniwave respawn tokens this faction starts each round with. Vanilla is 0.")]
        public int StartingTokens { get; set; } = 0;

        [Description("How many miniwave respawn tokens are given when this miniwave unlocks. Vanilla is 1.")]
        public int UnlockTokens { get; set; } = 1;
    }
}
