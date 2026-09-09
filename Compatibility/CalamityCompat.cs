using Terraria.ModLoader;
using System;

namespace TownNPCsFreeze.Compatibility
{
    public class CalamityCompat : ModSystem
    {
        public override void PostSetupContent()
        {
            try
            {
                UpdateExclusions();
            }
            catch (Exception ex)
            {
                FileLogger.Warn("CalamityCompat", $"Failed to update exclusions: {ex.Message}");
            }
        }

        public static void UpdateExclusions()
        {
            if (!ConfigCache.IsValid) return;

            if (!ModLoader.TryGetMod("CalamityMod", out Mod calamity))
                return;

            try
            {
                // Re-register
                UnregisterIfExists(calamity, "ShadySalesman");

                if (ConfigCache.CalamityCompatibility)
                {
                    RegisterIfExists(calamity, "ShadySalesman");
                }
            }
            catch (Exception ex)
            {
                FileLogger.Warn("CalamityCompat", $"Error updating exclusions: {ex.Message}");
            }
        }

        private static void RegisterIfExists(Mod mod, string npcName)
        {
            if (mod.TryFind(npcName, out ModNPC npc))
                ModAPI.RegisterExcludedNPC(npc.Type);
        }

        private static void UnregisterIfExists(Mod mod, string npcName)
        {
            if (mod.TryFind(npcName, out ModNPC npc))
                ModAPI.UnregisterExcludedNPC(npc.Type);
        }
    }
}

// Excluded ShadySalesman because he cannot despawn during the day due to disabled UpdateNPC.