using System;
using Terraria;

namespace TownNPCsFreeze
{
    public static class DistantFreezeHandler
    {
        private const int LoadDistanceX = ModConstants.DefaultLoadDistanceX;
        private const int LoadDistanceY = ModConstants.DefaultLoadDistanceY;
        private const int TilesToPixelsMultiplier = 16;

        private static int _thresholdPxX = LoadDistanceX * TilesToPixelsMultiplier;
        private static int _thresholdPxY = LoadDistanceY * TilesToPixelsMultiplier;

        public static void SyncWithConfig()
        {
            if (NetmodeHelper.IsMultiplayerClient) 
                return;
            if (!ConfigCache.IsValid) 
                return;

            _thresholdPxX = ConfigCache.LoadDistanceX * TilesToPixelsMultiplier;
            _thresholdPxY = ConfigCache.LoadDistanceY * TilesToPixelsMultiplier;

            if (ConfigCache.DistantFreeze)
            {
                ProcessDistantFreeze();
            }
            else
            {
                if (ConfigCache.BossFreeze && BossFreezeHandler.IsBossFightActive())
                {
                    BossFreezeHandler.SyncWithConfig();
                }
                else
                {
                    FreezeCore.ThawAll();
                }
            }
        }

        public static void ProcessDistantFreeze()
        {
            if (NetmodeHelper.IsMultiplayerClient) return;
            if (!ConfigCache.IsValid) return;
            if (!ConfigCache.DistantFreeze) return;

            if (ConfigCache.BossFreeze && BossFreezeHandler.IsBossFightActive())
                return;

            if (_thresholdPxX == 0 || _thresholdPxY == 0)
            {
                FreezeCore.FreezeAll();
                return;
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || !npc.townNPC) 
                    continue;
                if (ExclusionHandler.IsExcluded(npc)) 
                    continue;

                bool isFreeze = npc.ai[3] == ModConstants.FreezeFlag;
                bool playerInRange = IsAnyPlayerInRange(npc, _thresholdPxX, _thresholdPxY);

                if (!isFreeze && !playerInRange)
                    FreezeCore.SetFreeze(npc, true);
                else if (isFreeze && playerInRange)
                    FreezeCore.SetFreeze(npc, false);
            }
        }

        private static bool IsAnyPlayerInRange(NPC npc, int thresholdX, int thresholdY)
        {
            if (NetmodeHelper.IsSingleplayer)
            {
                Player player = Main.LocalPlayer;
                if (player?.active == true && !player.dead)
                    return IsPlayerInRange(npc, player.Center.X, player.Center.Y, thresholdX, thresholdY);
                return false;
            }

            for (int p = 0; p < Main.maxPlayers; p++)
            {
                Player player = Main.player[p];
                if (player?.active == true && !player.dead)
                {
                    if (IsPlayerInRange(npc, player.Center.X, player.Center.Y, thresholdX, thresholdY))
                        return true;
                }
            }
            return false;
        }

        private static bool IsPlayerInRange(NPC npc, float plX, float plY, int thresholdX, int thresholdY)
        {
            float dx = Math.Abs(plX - npc.Center.X);
            if (dx > thresholdX) return false;

            float dy = Math.Abs(plY - npc.Center.Y);
            return dy <= thresholdY;
        }
    }
}