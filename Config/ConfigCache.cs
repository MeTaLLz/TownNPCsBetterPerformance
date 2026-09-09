using System.Collections.Generic;

namespace TownNPCsFreeze
{
    public static class ConfigCache
    {
        public static bool DistantFreeze { get; private set; } = true;
        public static bool BossFreeze { get; private set; } = false;

        public static int LoadDistanceX { get; private set; } = ModConstants.DefaultLoadDistanceX;
        public static int LoadDistanceY { get; private set; } = ModConstants.DefaultLoadDistanceY;

        public static bool MakeInvincible { get; private set; } = false;

        public static HashSet<int> ManualExclusions { get; } = [];
        public static bool ExcludeTravelingMerchant { get; private set; } = true;

        public static bool WrathOfTheGodsCompatibility { get; private set; } = true;
        public static bool CalamityCompatibility { get; private set; } = true;
        public static bool TerrariaAmbienceFix { get; private set; } = true;

        public static int FreezeInterval { get; private set; } = ModConstants.DefaultFreezeInterval;
        public static int TeleportInterval { get; private set; } = ModConstants.DefaultTeleportInterval;
        public static int SyncInterval { get; private set; } = ModConstants.DefaultSyncInterval;
        public static int InvincibleInterval { get; private set; } = ModConstants.DefaultInvincibleInterval;
        public static bool LogToChat { get; private set; } = false;
        public static bool ModState { get; private set; } = true;

        public static bool IsValid { get; private set; } = false;

        public static void Update(TNBPConfig config)
        {
            if (config == null) return;

            var oldManualExclusions = new HashSet<int>(ManualExclusions);

            DistantFreeze = config.DistantFreeze;
            BossFreeze = config.BossFreeze;

            LoadDistanceX = config.LoadDistanceX;
            LoadDistanceY = config.LoadDistanceY;

            MakeInvincible = config.MakeInvincible;

            ManualExclusions.Clear();
            foreach (var def in config.ExcludedNPCs)
                ManualExclusions.Add(def.Type);
            ExcludeTravelingMerchant = config.ExcludeTravelingMerchant;

            WrathOfTheGodsCompatibility = config.WrathOfTheGodsCompatibility;
            CalamityCompatibility = config.CalamityCompatibility;
            TerrariaAmbienceFix = config.TerrariaAmbienceFix;

            FreezeInterval = config.FreezeInterval;
            TeleportInterval = config.TeleportInterval;
            SyncInterval = config.SyncInterval;
            InvincibleInterval = config.InvincibleInterval;
            LogToChat = config.LogToChat;
            ModState = config.ModState;

            ExclusionHandler.LogManualChanges(oldManualExclusions);

            IsValid = true;
        }
    }
}