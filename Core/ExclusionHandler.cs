using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace TownNPCsFreeze
{
    public static class ExclusionHandler
    {
        private static readonly HashSet<int> _autoExclusions = [];

        public static bool RegisterCompatibilityExclusion(int npcType)
        {
            if (!_autoExclusions.Add(npcType))
                return false;

            // if (!ConfigCache.ManualExclusions.Contains(npcType))
            //     ChatLogger.Log(ColorHelper.Aquamarine, "{0} ({1}) excluded from freeze (compat)", Lang.GetNPCNameValue(npcType), npcType);
            return true;
        }

        public static bool UnregisterCompatibilityExclusion(int npcType)
        {
            if (!_autoExclusions.Remove(npcType))
                return false;

            // ChatLogger.Log(ColorHelper.Aquamarine, "{0} ({1}) no longer excluded from freeze (compat)", Lang.GetNPCNameValue(npcType), npcType);
            return true;
        }

        public static void LogManualChanges(HashSet<int> oldExclusions)
        {
            if (!ConfigCache.LogToChat) return;

            foreach (int npcType in ConfigCache.ManualExclusions)
            {
                if (!oldExclusions.Contains(npcType))
                {
                    string npcName = Lang.GetNPCNameValue(npcType);
                    ChatLogger.Log(ColorHelper.Aquamarine, "{0} ({1}) excluded from freeze (manual)", npcName, npcType);
                }
            }

            foreach (int npcType in oldExclusions)
            {
                if (!ConfigCache.ManualExclusions.Contains(npcType))
                {
                    string npcName = Lang.GetNPCNameValue(npcType);
                    ChatLogger.Log(ColorHelper.Aquamarine, "{0} ({1}) no longer excluded from freeze (manual)", npcName, npcType);
                }
            }
        }

        public static bool IsExcluded(NPC npc)
        {
            if (IsExcludedByDefault(npc))
                return true;

            return _autoExclusions.Contains(npc.type) ||
                   ConfigCache.ManualExclusions.Contains(npc.type);
        }

        private static bool IsExcludedByDefault(NPC npc)
        {
            // Excluded TravellingMerchant because he cannot despawn during the night due to disabled UpdateNPC.
            if (npc.type == NPCID.TravellingMerchant && ConfigCache.ExcludeTravelingMerchant)
                return true;

            return false;
        }
    }
}