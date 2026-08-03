using System.Collections.Generic;
using Terraria;

namespace TownNPCsFreeze
{
    public static class ConfigCache
    {
        public static bool DistantGhost { get; private set; } = true;
        public static int LoadDistanceX { get; private set; } = 90;
        public static int LoadDistanceY { get; private set; } = 65;
        public static bool BossGhost { get; private set; } = false;

        public static bool MakeInvincible { get; private set; } = false;

        public static bool WrathOfTheGodsCompatibility { get; private set; } = true;
        public static bool CalamityCompatibility { get; private set; } = true;

        public static int GhostInterval { get; private set; } = 12;
        public static int TeleportInterval { get; private set; } = 12;
        public static int SyncInterval { get; private set; } = 12;
        public static int InvincibleInterval { get; private set; } = 60;

        public static bool LogToChat { get; private set; } = false;

        public static bool ExcludeTravelingMerchant { get; private set; } = true;

        public static HashSet<int> ExcludedNPCs { get; } = [];
        public static bool IsValid { get; private set; } = false;

        public static void Update(TNBPConfig config)
        {
            if (config == null) return;

            var oldExclusions = new HashSet<int>(ExcludedNPCs);

            DistantGhost = config.DistantGhost;
            LoadDistanceX = config.LoadDistanceX;
            LoadDistanceY = config.LoadDistanceY;
            BossGhost = config.BossGhost;
            LogToChat = config.LogToChat;
            MakeInvincible = config.MakeInvincible;
            WrathOfTheGodsCompatibility = config.WrathOfTheGodsCompatibility;
            CalamityCompatibility = config.CalamityCompatibility;
            GhostInterval = config.GhostInterval;
            TeleportInterval = config.TeleportInterval;
            SyncInterval = config.SyncInterval;
            InvincibleInterval = config.InvincibleInterval;
            ExcludeTravelingMerchant = config.ExcludeTravelingMerchant;

            ExcludedNPCs.Clear();
            foreach (var def in config.ExcludedNPCs)
                ExcludedNPCs.Add(def.Type);

            LogExclusionChanges(oldExclusions);

            IsValid = true;
                        
            DistantGhostManager.UpdateConfig();
        }

        private static void LogExclusionChanges(HashSet<int> oldExclusions)
        {
            if (!LogToChat) return;

            foreach (int npcType in ExcludedNPCs)
            {
                if (!oldExclusions.Contains(npcType))
                {
                    string npcName = Lang.GetNPCNameValue(npcType);
                    ModLogger.Log("ExcludedAdded", npcName, npcType);
                }
            }

            foreach (int npcType in oldExclusions)
            {
                if (!ExcludedNPCs.Contains(npcType))
                {
                    string npcName = Lang.GetNPCNameValue(npcType);
                    ModLogger.Log("ExcludedRemoved", npcName, npcType);
                }
            }
        }
    }
}