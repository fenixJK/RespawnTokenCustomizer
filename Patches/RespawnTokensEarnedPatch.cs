namespace RespawnTokenCustomizer.Patches
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Exiled.API.Features;
    using HarmonyLib;
    using LabApi.Events.Arguments.ServerEvents;
    using LabServerEvents = LabApi.Events.Handlers.ServerEvents;
    using Mirror;
    using PlayerRoles;
    using Respawning;
    using Respawning.Waves;
    using Respawning.Waves.Generic;
    using RespawnTokenCustomizer.Configuration;

    internal static class RespawnTokensEarnedPatch
    {
        public static bool IsPatched { get; private set; }

        public static bool Patch(Harmony harmony)
        {
            if (harmony is null)
                throw new ArgumentNullException(nameof(harmony));

            MethodInfo target = AccessTools.Method(typeof(RespawnTokensManager), "OnPointsModified", new[] { typeof(Faction), typeof(float) });
            MethodInfo prefix = typeof(RespawnTokensEarnedPatch).GetMethod(nameof(Prefix), BindingFlags.Static | BindingFlags.NonPublic);

            if (target is null || prefix is null)
            {
                Log.Warn("Respawn Token Customizer could not find RespawnTokensManager.OnPointsModified. Earned-token pool overrides are disabled for this server build.");
                IsPatched = false;
                return false;
            }

            try
            {
                harmony.Patch(target, prefix: new HarmonyMethod(prefix));
                IsPatched = true;
                return true;
            }
            catch (Exception exception)
            {
                Log.Warn($"Respawn Token Customizer could not patch earned-token behavior. Earned-token pool overrides are disabled for this server build. {exception}");
                IsPatched = false;
                return false;
            }
        }

        private static bool Prefix(Faction faction, float newValue)
        {
            Plugin plugin = Plugin.Instance;

            if (plugin is null || !plugin.Config.OverrideEarnedTokenPool)
                return true;

            if (!NetworkServer.active || !WaveManager.TryGet(faction, out SpawnableWaveBase spawnWave) || !(spawnWave is ILimitedWave limitedWave))
                return false;

            FactionTokenSettings settings = plugin.TokenService.GetSettings(faction);
            int earnedLimit = Math.Max(0, settings.EarnableTokens);

            while (plugin.TokenService.GetEarned(faction) < earnedLimit && TryAchieveMilestone(faction, newValue))
            {
                limitedWave.RespawnTokens++;
                plugin.TokenService.MarkEarned(faction);
                WaveUpdateMessage.ServerSendUpdate(spawnWave, UpdateMessageFlags.Tokens);
            }

            if (plugin.TokenService.CalculateRemainingEarnableTokens() == 0)
                SendMaxUpdates();

            WaveUpdateMessage.ServerSendUpdate(spawnWave, UpdateMessageFlags.Tokens);
            return false;
        }

        private static bool TryAchieveMilestone(Faction faction, float influence)
        {
            if (!RespawnTokensManager.Milestones.TryGetValue(faction, out List<RespawnTokensManager.Milestone> milestones))
                return false;

            for (int index = 0; index < milestones.Count; index++)
            {
                RespawnTokensManager.Milestone milestone = milestones[index];

                if (milestone.Achieved || milestone.Threshold > influence)
                    continue;

                AchievingMilestoneEventArgs achieving = new AchievingMilestoneEventArgs(faction, milestone.Threshold, index);
                LabServerEvents.OnAchievingMilestone(achieving);

                if (!achieving.IsAllowed)
                    return false;

                milestone.Achieved = true;
                LabServerEvents.OnAchievedMilestone(new AchievedMilestoneEventArgs(faction, milestone.Threshold, index));
                return true;
            }

            return false;
        }

        private static void SendMaxUpdates()
        {
            foreach (SpawnableWaveBase wave in WaveManager.Waves)
            {
                if (!(wave is IMiniWave))
                    WaveUpdateMessage.ServerSendUpdate(wave, UpdateMessageFlags.Max);
            }
        }
    }
}
