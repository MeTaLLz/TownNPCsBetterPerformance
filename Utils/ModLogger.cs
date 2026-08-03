using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace TownNPCsFreeze
{
    public static class ModLogger
    {
        private const string Prefix = "Mods.TownNPCsFreeze.ModLogger.";

        public static void Log(string key, params object[] args)
        {
            if (!ConfigCache.IsValid || !ConfigCache.LogToChat) return;

            string fullKey = Prefix + key;
            string message = Language.GetTextValue(fullKey, args);
            string fullMessage = $"[TNBP] {message}";

            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(fullMessage);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = ModInstance.Mod.GetPacket();
                packet.Write((byte)99);
                packet.Write(fullMessage);
                packet.Send(0);
            }
            else if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                Main.NewText(fullMessage);
            }
        }
    }
}