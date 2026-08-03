using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace TownNPCsFreeze
{
    public static class ExclusionManager
    {
        private static readonly HashSet<int> _compatibilityExclusions = [];

        public static bool RegisterCompatibilityExclusion(int npcType)
        {
            if (!_compatibilityExclusions.Add(npcType))
                return false;

            ModLogger.Log("ExcludedAdded", Lang.GetNPCNameValue(npcType), npcType);
            return true;
        }

        public static bool UnregisterCompatibilityExclusion(int npcType)
        {
            if (!_compatibilityExclusions.Remove(npcType))
                return false;

            ModLogger.Log("ExcludedRemoved", Lang.GetNPCNameValue(npcType), npcType);
            return true;
        }

        public static bool IsExcluded(NPC npc)
        {
            if (IsAlwaysExcluded(npc))
                return true;

            return _compatibilityExclusions.Contains(npc.type) ||
                   ConfigCache.ExcludedNPCs.Contains(npc.type);
        }

        private static bool IsAlwaysExcluded(NPC npc)
        {
            if (npc.type == NPCID.TravellingMerchant && ConfigCache.ExcludeTravelingMerchant)
                return true;

            return false;
        }
    }
}