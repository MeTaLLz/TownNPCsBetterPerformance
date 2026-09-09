using System.IO;
using Microsoft.Xna.Framework;
using Terraria;

namespace TownNPCsFreeze
{
    public static class PacketHandler
    {
        public static void HandlePacket(BinaryReader reader)
        {
            switch (reader.ReadByte())
            {
                case 1: HandleLocalAISync(reader); break;
                case 2: HandleVisualSync(reader); break;
                case 99: HandleChatMessage(reader); break;
            }
        }

        // Home teleport
        private static void HandleLocalAISync(BinaryReader reader)
        {
            int npcWhoAmI = reader.ReadInt32();
            NPC npc = Main.npc[npcWhoAmI];
            if (npc != null && NetmodeHelper.IsMultiplayerClient)
            {
                npc.localAI[0] = reader.ReadInt32();
                npc.localAI[1] = reader.ReadInt32();
                npc.localAI[2] = reader.ReadInt32();
                npc.localAI[3] = reader.ReadInt32();
            }
        }

        // Sprite visibility
        private static void HandleVisualSync(BinaryReader reader)
        {
            int npcWhoAmI = reader.ReadInt32();
            NPC npc = Main.npc[npcWhoAmI];
            if (npc != null && NetmodeHelper.IsMultiplayerClient)
            {
                npc.Opacity = reader.ReadSingle();
                float x = reader.ReadSingle();
                float y = reader.ReadSingle();
                npc.position.X = x;
                npc.position.Y = y;
                npc.netOffset = Vector2.Zero;
            }
        }

        // Server message
        private static void HandleChatMessage(BinaryReader reader)
        {
            string message = reader.ReadString();
            byte r = reader.ReadByte();
            byte g = reader.ReadByte();
            byte b = reader.ReadByte();

            if (NetmodeHelper.IsMultiplayerClient)
            {
                Main.NewText(message, new Color(r, g, b));
            }
        }
    }
}