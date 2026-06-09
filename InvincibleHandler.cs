using Terraria;
using Terraria.ModLoader;
using static TownNPCsFreeze.ModInstanceManager;

namespace TownNPCsFreeze
{
    public class InvincibleHandler : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public override bool PreAI(NPC npc)
        {
            if (Config.MakeInvincible && npc.townNPC && !npc.boss)
                npc.dontTakeDamage = true;
            
            return true;
        }

        public override bool? CanBeHitByItem(NPC npc, Player player, Item item)
        {
            if (Config.MakeInvincible && npc.townNPC && !npc.boss)
                return false;
            return null;
        }

        public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
        {
            if (Config.MakeInvincible && npc.townNPC && !npc.boss)
                return false;
            return null;
        }

        public override bool CanBeHitByNPC(NPC npc, NPC attacker)
        {
            if (Config.MakeInvincible && npc.townNPC && !npc.boss)
                return false;
            return base.CanBeHitByNPC(npc, attacker);
        }
    }
}