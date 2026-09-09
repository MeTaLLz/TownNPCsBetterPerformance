using Terraria.ModLoader;
using System;

namespace TownNPCsFreeze.Compatibility
{
    public class WoTGCompat : ModSystem
    {
        public override void PostSetupContent()
        {
            try
            {
                UpdateExclusions();
            }
            catch (Exception ex)
            {
                FileLogger.Warn("WoTGCompat", $"Failed to update exclusions: {ex.Message}");
            }
        }

        public static void UpdateExclusions()
        {
            if (!ConfigCache.IsValid) return;

            if (!ModLoader.TryGetMod("NoxusBoss", out Mod noxusBoss))
                return;

            try
            {
                // Re-register
                UnregisterIfExists(noxusBoss, "Solyn");
                UnregisterIfExists(noxusBoss, "BattleSolyn");

                if (ConfigCache.WrathOfTheGodsCompatibility)
                {
                    RegisterIfExists(noxusBoss, "Solyn");
                    RegisterIfExists(noxusBoss, "BattleSolyn");
                }
            }
            catch (Exception ex)
            {
                FileLogger.Warn("WoTGCompat", $"Error updating exclusions: {ex.Message}");
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

// Excluded both versions of Solyn because she can be frozen during mod boss fights.

// Maybe it will be necessary to disable the mod’s functionality during the fight with the Avatar of Emptiness while TilesAreUninteractable = true.