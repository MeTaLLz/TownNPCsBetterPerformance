using Terraria;
using Terraria.ModLoader;

namespace TownNPCsFreeze
{
    public class InvincibleManager : GlobalNPC
    {
        private static bool _invincibleEnabled = false;
        private static bool _prevInvincibleEnabled = false;

        public static void UpdateConfig()
        {
            _prevInvincibleEnabled = _invincibleEnabled;
            _invincibleEnabled = ConfigCache.MakeInvincible;
        }

        public static void ApplyToAll()
        {
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
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.townNPC && !npc.boss)
                    npc.immortal = false;
            }
        }

        public static void OnConfigChanged()
        {
            if (_prevInvincibleEnabled && !_invincibleEnabled)
                RemoveAll();
        }
    }
}