using Terraria.ModLoader;

namespace TownNPCsFreeze
{
    public static class ModInstanceManager
    {
        private static TNPSConfig _config;
        public static TNPSConfig Config => _config ??= ModContent.GetInstance<TNPSConfig>();
        
        private static TownNPCsFreeze _mod;
        public static TownNPCsFreeze Mod => _mod ??= ModContent.GetInstance<TownNPCsFreeze>();
    }
}