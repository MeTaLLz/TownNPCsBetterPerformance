using Terraria.ModLoader;

namespace TownNPCsFreeze.CrossCompatibility
{
    public class CalamityCompat : ModSystem
    {
        public override void Load()
        {
            UpdateExclusions();
        }

        public static void UpdateExclusions()
        {
            if (!ConfigCache.IsValid) return;
            
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamity))
            {
                if (calamity.TryFind("ShadySalesman", out ModNPC shadySalesman))
                    ModAPI.UnregisterExcludedNPC(shadySalesman.Type);
            }
            
            if (ConfigCache.CalamityCompatibility)
            {
                if (ModLoader.TryGetMod("CalamityMod", out Mod calamity2))
                {
                    if (calamity2.TryFind("ShadySalesman", out ModNPC shadySalesman))
                        ModAPI.RegisterExcludedNPC(shadySalesman.Type);
                }
            }
        }
    }
}