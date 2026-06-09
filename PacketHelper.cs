using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TownNPCsFreeze
{
    public static class PacketHelper
    {
        public static void SendLocalAISync(NPC npc)
        {
            if (Main.netMode != NetmodeID.Server)
                return;
            
            ModPacket packet = ModInstanceManager.Mod.GetPacket();
            packet.Write((byte)1);
            packet.Write(npc.whoAmI);
            packet.Write((int)npc.localAI[0]);
            packet.Write((int)npc.localAI[1]);
            packet.Write((int)npc.localAI[2]);
            packet.Write((int)npc.localAI[3]);
            packet.Send();
        }

        public static void SendVisualSync(NPC npc)
        {
            if (Main.netMode != NetmodeID.Server)
                return;
            
            ModPacket packet = ModInstanceManager.Mod.GetPacket();
            packet.Write((byte)2);
            packet.Write(npc.whoAmI);
            packet.Write(npc.hide);
            packet.Write(npc.position.X);
            packet.Write(npc.position.Y);
            packet.Send();
        }
    }
}