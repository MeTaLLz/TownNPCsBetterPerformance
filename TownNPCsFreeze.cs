using System.IO;
using Terraria.ModLoader;

namespace TownNPCsFreeze
{
    public class TownNPCsFreeze : Mod
    {
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            PacketHandler.HandlePacket(reader, whoAmI);
        }
    }
}