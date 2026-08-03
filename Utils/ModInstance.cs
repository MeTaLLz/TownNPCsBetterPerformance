using Terraria.ModLoader;

namespace TownNPCsFreeze
{
    public static class ModInstance
    {   
        private static TownNPCsFreeze _mod;
        public static TownNPCsFreeze Mod => _mod ??= ModContent.GetInstance<TownNPCsFreeze>();
    }
}