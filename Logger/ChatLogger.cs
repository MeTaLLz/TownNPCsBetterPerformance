using Microsoft.Xna.Framework;
using Terraria;

namespace TownNPCsFreeze
{
    public static class ChatLogger
    {
        private const string Prefix = ModConstants.Prefix;

        public static void Log(string format, params object[] args)
        {
            Log(Color.White, format, args);
        }

        public static void Log(Color color, string format, params object[] args)
        {
            if (!ConfigCache.IsValid || !ConfigCache.LogToChat) return;

            string message = string.Format(format, args);
            string fullMessage = $"{Prefix} {message}";

            if (NetmodeHelper.IsSingleplayer || NetmodeHelper.IsMultiplayerClient)
            {
                Main.NewText(fullMessage, color);
            }
            else if (NetmodeHelper.IsServer)
            {
                // Don't log while mod is configuring on server
                // if (Main.gameMenu) 
                //     return;
                
                PacketSender.SendChatMessage(fullMessage, color);
            }
        }
    }
}