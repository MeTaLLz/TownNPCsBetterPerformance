using Terraria;
using Terraria.ModLoader;

namespace TownNPCsFreeze
{
    public class InvincibleHandler : GlobalNPC
    {
        private static bool _invincibleEnabled = false;

        public static void SyncWithConfig()
        {
            bool wasEnabled = _invincibleEnabled;
            _invincibleEnabled = ConfigCache.MakeInvincible;
            
            if (wasEnabled && !_invincibleEnabled)
                RemoveAll();
        }

        public static void ApplyToAll()
        {
            if (NetmodeHelper.IsMultiplayerClient) return;
            
            if (!_invincibleEnabled) return;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.townNPC && !npc.boss)
                    npc.immortal = true;
            }
        }

        public static void RemoveAll()
        {
            if (NetmodeHelper.IsMultiplayerClient) return;
            
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.townNPC && !npc.boss)
                    npc.immortal = false;
            }
        }
    }
}