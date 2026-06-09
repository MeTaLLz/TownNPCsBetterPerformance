using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TownNPCsFreeze
{
    public class TownNPCsFreeze : Mod
    {
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            byte packetType = reader.ReadByte();
            
            switch (packetType)
            {
                case 1:
                    HandleLocalAISync(reader);
                    break;
                case 2:
                    HandleVisualSync(reader);
                    break;
                case 99:
                    HandleLogMessage(reader);
                    break;
            }
        }

        private void HandleLocalAISync(BinaryReader reader)
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

        private void HandleVisualSync(BinaryReader reader)
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

        private void HandleLogMessage(BinaryReader reader)
        {
            string message = reader.ReadString();
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                Main.NewText(message);
            }
        }
    }
}