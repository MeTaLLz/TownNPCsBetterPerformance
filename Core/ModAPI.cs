namespace TownNPCsFreeze
{
    public static class ModAPI
    {
        private static bool _enabled = true;
        private static bool _initialized = false;

        /// <summary>
        /// Initializes the API.
        /// </summary>
        internal static void Initialize()
        {
            if (_initialized) return;
            _initialized = true;
        }

        /// <summary>
        /// Completely enables or disables all mod functionality.
        /// </summary>
        public static void SetEnabled(bool enabled)
        {
            if (_enabled == enabled) return;

            _enabled = enabled;

            if (!_enabled)
            {
                ChatLogger.Log(ColorHelper.DarkRed, "Mod functionality disabled");
                FreezeCore.ThawAll();
                FreezeCore.ThawExcluded();
                InvincibleHandler.RemoveAll();
                return;
            }

            ChatLogger.Log(ColorHelper.DarkGreen, "Mod functionality enabled");

            BossFreezeHandler.SyncState();

            if (ConfigCache.BossFreeze && BossFreezeHandler.IsBossFightActive())
            {
                BossFreezeHandler.ForceFreeze();
            }
            else if (ConfigCache.DistantFreeze)
            {
                DistantFreezeHandler.ProcessDistantFreeze();
            }
            else
            {
                FreezeCore.ThawAll();
            }
        }

        /// <summary>
        /// Returns whether the mod is currently enabled.
        /// </summary>
        public static bool IsEnabled() => _enabled;

        /// <summary>
        /// Registers a town NPC type that should never be frozen.
        /// </summary>
        public static void RegisterExcludedNPC(int npcType)
        {
            if (!_initialized) Initialize();
            ExclusionHandler.RegisterCompatibilityExclusion(npcType);
        }

        /// <summary>
        /// Unregisters a previously registered town NPC exclusion.
        /// </summary>
        public static void UnregisterExcludedNPC(int npcType)
        {
            if (!_initialized) Initialize();
            ExclusionHandler.UnregisterCompatibilityExclusion(npcType);
        }
    }
}