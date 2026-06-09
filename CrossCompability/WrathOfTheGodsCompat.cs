using Terraria.ModLoader;
using static TownNPCsFreeze.ModInstanceManager;

namespace TownNPCsFreeze.CrossCompatibility
{
    public class WrathOfTheGodsCompat : ModSystem
    {
        public override void Load()
        {
            UpdateExclusions();
        }

        public static void UpdateExclusions()
        {
            if (Config == null) return;
            
            if (ModLoader.TryGetMod("NoxusBoss", out Mod noxusBoss))
            {
                if (noxusBoss.TryFind("Solyn", out ModNPC solyn))
                    ModAPI.UnregisterExcludedNPC(solyn.Type);
                
                if (noxusBoss.TryFind("BattleSolyn", out ModNPC battleSolyn))
                    ModAPI.UnregisterExcludedNPC(battleSolyn.Type);
            }
            
            if (Config.WrathOfTheGodsCompatibility)
            {
                if (ModLoader.TryGetMod("NoxusBoss", out Mod noxusBoss2))
                {
                    if (noxusBoss2.TryFind("Solyn", out ModNPC solyn))
                        ModAPI.RegisterExcludedNPC(solyn.Type);
                    
                    if (noxusBoss2.TryFind("BattleSolyn", out ModNPC battleSolyn))
                        ModAPI.RegisterExcludedNPC(battleSolyn.Type);
                }
            }
        }
    }
}