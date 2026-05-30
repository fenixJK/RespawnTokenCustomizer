namespace RespawnTokenCustomizer.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Features;
    using PlayerRoles;
    using Respawning;
    using Respawning.Waves;
    using Respawning.Waves.Generic;
    using RespawnTokenCustomizer.Configuration;
    using RespawnTokenCustomizer.Patches;

    internal sealed class RespawnTokenService
    {
        private readonly Plugin plugin;
        private readonly FactionTokenRuntimeState state;

        public RespawnTokenService(Plugin plugin, FactionTokenRuntimeState state)
        {
            this.plugin = plugin ?? throw new ArgumentNullException(nameof(plugin));
            this.state = state ?? throw new ArgumentNullException(nameof(state));
        }

        public void ResetForRound()
        {
            state.Reset();
            ApplySettings(resetCurrentTokens: true);
        }

        public void Reload()
        {
            ApplySettings(plugin.Config.ResetCurrentTokensOnReload);
        }

        public void ApplySettings(bool resetCurrentTokens)
        {
            ApplyFaction(Faction.FoundationStaff, plugin.Config.NineTailedFox, resetCurrentTokens);
            ApplyFaction(Faction.FoundationEnemy, plugin.Config.ChaosInsurgency, resetCurrentTokens);
            RespawnTokensManager.AvailableRespawnsLeft = CalculateRemainingEarnableTokens();
        }

        public int CalculateRemainingEarnableTokens()
        {
            if (!plugin.Config.OverrideEarnedTokenPool)
                return RespawnTokensManager.AvailableRespawnsLeft;

            return Math.Max(0, GetSettings(Faction.FoundationStaff).EarnableTokens - state.GetEarned(Faction.FoundationStaff)) +
                Math.Max(0, GetSettings(Faction.FoundationEnemy).EarnableTokens - state.GetEarned(Faction.FoundationEnemy));
        }

        public FactionTokenSettings GetSettings(Faction faction)
        {
            return faction == Faction.FoundationEnemy
                ? plugin.Config.ChaosInsurgency ?? new FactionTokenSettings()
                : plugin.Config.NineTailedFox ?? new FactionTokenSettings();
        }

        public int GetEarned(Faction faction)
        {
            return state.GetEarned(faction);
        }

        public void MarkEarned(Faction faction)
        {
            state.IncrementEarned(faction);
            RespawnTokensManager.AvailableRespawnsLeft = CalculateRemainingEarnableTokens();
        }

        public string BuildStatus()
        {
            return string.Join("\n", new[]
            {
                BuildFactionStatus("NTF", Faction.FoundationStaff),
                BuildFactionStatus("Chaos", Faction.FoundationEnemy),
                $"earned pool remaining={RespawnTokensManager.AvailableRespawnsLeft}",
                $"earned pool override patch active={RespawnTokensEarnedPatch.IsPatched}",
            });
        }

        private void ApplyFaction(Faction faction, FactionTokenSettings settings, bool resetCurrentTokens)
        {
            settings = Normalize(settings);
            RespawnTokensManager.Milestones[faction] = BuildMilestones(settings);

            if (!WaveManager.TryGet(faction, out SpawnableWaveBase wave) || !(wave is ILimitedWave limitedWave))
            {
                Log.Warn($"Respawn Token Customizer could not find limited wave for faction {faction}.");
                return;
            }

            limitedWave.InitialRespawnTokens = settings.StartingTokens;

            if (resetCurrentTokens)
                limitedWave.RespawnTokens = settings.StartingTokens;

            WaveUpdateMessage.ServerSendUpdate(wave, UpdateMessageFlags.Tokens | UpdateMessageFlags.Max);

            if (plugin.Config.Debug)
                Log.Debug($"Respawn Token Customizer applied {faction}: starting={settings.StartingTokens}, earnable={settings.EarnableTokens}, resetCurrent={resetCurrentTokens}.");
        }

        private string BuildFactionStatus(string label, Faction faction)
        {
            FactionTokenSettings settings = GetSettings(faction);
            string waveTokens = "unavailable";

            if (WaveManager.TryGet(faction, out SpawnableWaveBase wave) && wave is ILimitedWave limitedWave)
                waveTokens = $"{limitedWave.RespawnTokens}/{limitedWave.InitialRespawnTokens}";

            int achieved = RespawnTokensManager.Milestones.TryGetValue(faction, out List<RespawnTokensManager.Milestone> milestones)
                ? milestones.Count(milestone => milestone.Achieved)
                : 0;

            return $"{label}: wave tokens={waveTokens}, earned={state.GetEarned(faction)}/{Math.Max(0, settings.EarnableTokens)}, milestones achieved={achieved}";
        }

        private FactionTokenSettings Normalize(FactionTokenSettings settings)
        {
            settings ??= new FactionTokenSettings();
            settings.StartingTokens = Math.Max(0, settings.StartingTokens);
            settings.EarnableTokens = Math.Max(0, settings.EarnableTokens);
            settings.MilestoneThresholds ??= new List<int>();
            plugin.Config.ExtraMilestoneStep = Math.Max(1, plugin.Config.ExtraMilestoneStep);
            return settings;
        }

        private List<RespawnTokensManager.Milestone> BuildMilestones(FactionTokenSettings settings)
        {
            List<int> thresholds = settings.MilestoneThresholds
                .Where(threshold => threshold > 0)
                .Distinct()
                .OrderBy(threshold => threshold)
                .ToList();

            int requiredThresholds = settings.EarnableTokens;

            while (thresholds.Count < requiredThresholds)
            {
                int next = thresholds.Count == 0
                    ? plugin.Config.ExtraMilestoneStep
                    : thresholds[thresholds.Count - 1] + plugin.Config.ExtraMilestoneStep;

                thresholds.Add(next);
            }

            return thresholds
                .Take(requiredThresholds)
                .Select(threshold => new RespawnTokensManager.Milestone(threshold))
                .ToList();
        }
    }
}
