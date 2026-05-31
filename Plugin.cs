namespace RespawnTokenCustomizer
{
    using System;
    using Exiled.API.Features;
    using HarmonyLib;
    using RespawnTokenCustomizer.Configuration;
    using RespawnTokenCustomizer.Patches;
    using RespawnTokenCustomizer.Runtime;
    using ExiledPlugin = Exiled.API.Features.Plugin<RespawnTokenCustomizer.Configuration.Config>;
    using ServerEvents = Exiled.Events.Handlers.Server;

    public sealed class Plugin : ExiledPlugin
    {
        private const string HarmonyIdPrefix = "respawntokencustomizer";

        private Harmony harmony;
        private FactionTokenRuntimeState runtimeState;
        private RespawnTokenService tokenService;

        public static Plugin Instance { get; private set; }

        public override string Author => "Ferox";

        public override string Name => "Respawn Token Customizer";

        public override string Prefix => "respawn_token_customizer";

        public override Version Version => new Version(1, 4, 0);

        public override Version RequiredExiledVersion => new Version(9, 14, 1);

        internal RespawnTokenService TokenService => tokenService;

        public override void OnEnabled()
        {
            Instance = this;
            runtimeState = new FactionTokenRuntimeState();
            tokenService = new RespawnTokenService(this, runtimeState);
            harmony = new Harmony($"{HarmonyIdPrefix}.{DateTime.UtcNow.Ticks}");

            try
            {
                bool earnedTokenPatchEnabled = EnsureEarnedTokenPatch();
                MiniWaveUnlockPatch.Patch(harmony);

                if (Config.EarnedTokenPoolMode == EarnedTokenPoolMode.PerFaction && !earnedTokenPatchEnabled)
                    Log.Warn("Respawn Token Customizer will use shared earned-token behavior because the per-faction patch could not be applied.");

                tokenService.ResetForRound();
            }
            catch (Exception exception)
            {
                Cleanup();
                Log.Error($"Respawn Token Customizer failed to enable: {exception}");
                throw;
            }

            ServerEvents.WaitingForPlayers += OnWaitingForPlayers;
            ServerEvents.RoundStarted += OnRoundStarted;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Cleanup();
            base.OnDisabled();
        }

        internal void ReloadRuntime()
        {
            EnsureEarnedTokenPatch();
            tokenService?.Reload();
        }

        private bool EnsureEarnedTokenPatch()
        {
            if (Config.EarnedTokenPoolMode != EarnedTokenPoolMode.PerFaction)
                return true;

            if (RespawnTokensEarnedPatch.IsPatched)
                return true;

            return RespawnTokensEarnedPatch.Patch(harmony, warnOnFailure: true);
        }

        private void OnWaitingForPlayers()
        {
            tokenService?.ResetForRound();
        }

        private void OnRoundStarted()
        {
            tokenService?.ResetForRound();
        }

        private void Cleanup()
        {
            ServerEvents.WaitingForPlayers -= OnWaitingForPlayers;
            ServerEvents.RoundStarted -= OnRoundStarted;
            harmony?.UnpatchAll(harmony.Id);
            RespawnTokensEarnedPatch.Reset();
            MiniWaveUnlockPatch.Reset();
            harmony = null;
            tokenService = null;
            runtimeState = null;
            Instance = null;
        }
    }
}
