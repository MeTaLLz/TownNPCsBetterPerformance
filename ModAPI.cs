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
        /// Completely enables or disables all mod functionality. (Just in case)
        /// </summary>
        public static void SetEnabled(bool enabled)
        {
            if (_enabled == enabled) return;
            
            _enabled = enabled;
            
            if (!_enabled)
            {
                GhostManager.RestoreAllGhost();
            }
            else
            {
                GhostManager.ProcessDistantGhost();
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
            GhostManager.RegisterExcludedNPC(npcType);
        }

        /// <summary>
        /// Unregisters a previously registered town NPC exclusion.
        /// </summary>
        public static void UnregisterExcludedNPC(int npcType)
        {
            if (!_initialized) Initialize();
            GhostManager.UnregisterExcludedNPC(npcType);
        }
    }
}