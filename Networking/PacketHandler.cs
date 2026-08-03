using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace TownNPCsFreeze
{
    public static class PacketHandler
    {
        public static void HandlePacket(BinaryReader reader, int whoAmI)
        {
            switch (reader.ReadByte())
            {
                case 1: HandleLocalAISync(reader); break;
                case 2: HandleVisualSync(reader); break;
                case 99: HandleLogMessage(reader); break;
            }
        }

        private static void HandleLocalAISync(BinaryReader reader)
        {
            int npcWhoAmI = reader.ReadInt32();
            NPC npc = Main.npc[npcWhoAmI];
            if (npc != null && Main.netMode == NetmodeID.MultiplayerClient)
            {
                npc.localAI[0] = reader.ReadInt32();
                npc.localAI[1] = reader.ReadInt32();
                npc.localAI[2] = reader.ReadInt32();
                npc.localAI[3] = reader.ReadInt32();
            }
        }

        private static void HandleVisualSync(BinaryReader reader)
        {
            int npcWhoAmI = reader.ReadInt32();
            NPC npc = Main.npc[npcWhoAmI];
            if (npc != null && Main.netMode == NetmodeID.MultiplayerClient)
            {
                npc.hide = reader.ReadBoolean();
                float x = reader.ReadSingle();
                float y = reader.ReadSingle();
                npc.position.X = x;
                npc.position.Y = y;
                npc.netOffset = Vector2.Zero;
            }
        }

        private static void HandleLogMessage(BinaryReader reader)
        {
            string message = reader.ReadString();
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                Main.NewText(message);
            }
        }
    }
}