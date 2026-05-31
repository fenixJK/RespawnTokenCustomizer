namespace RespawnTokenCustomizer.Patches
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Exiled.API.Features;
    using HarmonyLib;
    using Mirror;
    using Respawning;
    using Respawning.Waves;
    using Respawning.Waves.Generic;
    using RespawnTokenCustomizer.Configuration;

    internal static class MiniWaveUnlockPatch
    {
        public static bool IsPatched { get; private set; }

        public static bool Patch(Harmony harmony)
        {
            if (harmony is null)
                throw new ArgumentNullException(nameof(harmony));

            MethodInfo postfix = typeof(MiniWaveUnlockPatch).GetMethod(nameof(Postfix), BindingFlags.Static | BindingFlags.NonPublic);
            List<MethodInfo> targets = new List<MethodInfo>
            {
                AccessTools.Method(typeof(NtfMiniWave).BaseType, "Unlock", new[] { typeof(bool) }),
                AccessTools.Method(typeof(ChaosMiniWave).BaseType, "Unlock", new[] { typeof(bool) }),
            };

            if (postfix is null || targets.Exists(target => target is null))
            {
                Log.Warn("Respawn Token Customizer could not find miniwave Unlock methods. Miniwave unlock-token config is disabled for this server build.");
                IsPatched = false;
                return false;
            }

            try
            {
                foreach (MethodInfo target in targets)
                {
                    harmony.Patch(target, postfix: new HarmonyMethod(postfix));
                }

                IsPatched = true;
                return true;
            }
            catch (Exception exception)
            {
                Log.Warn($"Respawn Token Customizer could not patch miniwave unlock behavior. Miniwave unlock-token config is disabled for this server build. {exception}");
                IsPatched = false;
                return false;
            }
        }

        public static void Reset()
        {
            IsPatched = false;
        }

        private static void Postfix(IMiniWave __instance, bool ignoreConfig)
        {
            Plugin plugin = Plugin.Instance;

            if (plugin is null || __instance is null || !(__instance is SpawnableWaveBase wave))
                return;

            if (!ignoreConfig && NetworkServer.active && !wave.Configuration.IsEnabled)
                return;

            int unlockTokens = plugin.TokenService.GetMiniWaveUnlockTokens(__instance);

            if (__instance is ILimitedWave limitedWave)
            {
                limitedWave.RespawnTokens = unlockTokens;

                WaveUpdateMessage.ServerSendUpdate(wave, UpdateMessageFlags.Tokens);
            }
        }
    }
}
