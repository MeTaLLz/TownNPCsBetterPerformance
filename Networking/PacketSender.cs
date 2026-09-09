using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace TownNPCsFreeze
{
    public static class PacketSender
    {
        // Home teleport
        public static void SendLocalAISync(NPC npc)
        {
            if (!NetmodeHelper.IsServer) return;
            
            ModPacket packet = ModInstance.Mod.GetPacket();
            packet.Write((byte)1);
            packet.Write(npc.whoAmI);
            packet.Write((int)npc.localAI[0]);
            packet.Write((int)npc.localAI[1]);
            packet.Write((int)npc.localAI[2]);
            packet.Write((int)npc.localAI[3]);
            packet.Send();
        }

        // Sprite visibility
        public static void SendVisualSync(NPC npc)
        {
            if (!NetmodeHelper.IsServer) return;
            
            ModPacket packet = ModInstance.Mod.GetPacket();
            packet.Write((byte)2);
            packet.Write(npc.whoAmI);
            packet.Write(npc.Opacity);
            packet.Write(npc.position.X);
            packet.Write(npc.position.Y);
            packet.Send();
        }

        // Server message
        public static void SendChatMessage(string message, Color color)
        {
            if (!NetmodeHelper.IsServer) return;
            
            ModPacket packet = ModInstance.Mod.GetPacket();
            packet.Write((byte)99);
            packet.Write(message);
            packet.Write(color.R);
            packet.Write(color.G);
            packet.Write(color.B);
            packet.Send();
        }
    }
}