namespace RespawnTokenCustomizer.Runtime
{
    using System.Collections.Generic;
    using PlayerRoles;

    internal sealed class FactionTokenRuntimeState
    {
        private readonly Dictionary<Faction, int> earnedTokens = new Dictionary<Faction, int>();

        public void Reset()
        {
            earnedTokens.Clear();
        }

        public int GetEarned(Faction faction)
        {
            return earnedTokens.TryGetValue(faction, out int value) ? value : 0;
        }

        public void IncrementEarned(Faction faction)
        {
            earnedTokens[faction] = GetEarned(faction) + 1;
        }
    }
}
