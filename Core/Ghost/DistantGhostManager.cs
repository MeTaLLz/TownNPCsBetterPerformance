using System;
using Terraria;
using Terraria.ID;

namespace TownNPCsFreeze
{
    public static class DistantGhostManager
    {
         private static int _thresholdPxX = 90 * 16;
        private static int _thresholdPxY = 65 * 16;

        private static int _prevLoadDistanceX = 90;
        private static int _prevLoadDistanceY = 65;

        public static void UpdateConfig()
        {
            _thresholdPxX = ConfigCache.LoadDistanceX * 16;
            _thresholdPxY = ConfigCache.LoadDistanceY * 16;
        }

        public static void OnConfigChanged()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;
            if (!ConfigCache.IsValid) return;

            bool distanceChanged = ConfigCache.LoadDistanceX != _prevLoadDistanceX ||
                                ConfigCache.LoadDistanceY != _prevLoadDistanceY;

            if (ConfigCache.DistantGhost)
            {
                if (distanceChanged)
                {
                    UpdateConfig();
                    ProcessDistantGhost();
                }
                else
                {
                    ProcessDistantGhost();
                }
            }
            else
            {
                if (ConfigCache.BossGhost && BossGhostManager.IsBossFightActive())
                {
                    BossGhostManager.ProcessBossGhost();
                }
                else
                {
                    GhostCore.ThawAllNPCs();
                }
            }

            _prevLoadDistanceX = ConfigCache.LoadDistanceX;
            _prevLoadDistanceY = ConfigCache.LoadDistanceY;
        }

        public static void ProcessDistantGhost()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;
            if (!ConfigCache.IsValid) return;
            if (!ConfigCache.DistantGhost) return;

            if (ConfigCache.BossGhost && BossGhostManager.IsBossFightActive())
                return;

            if (_thresholdPxX == 0 || _thresholdPxY == 0)
            {
                GhostCore.FreezeAllNPCs();
                return;
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || !npc.townNPC) 
                    continue;
                if (ExclusionManager.IsExcluded(npc)) 
                    continue;

                bool isGhost = npc.ai[3] == ModConstants.GhostFlag;
                bool playerInRange = IsAnyPlayerInRange(npc, _thresholdPxX, _thresholdPxY);

                if (!isGhost && !playerInRange)
                    GhostCore.SetGhost(npc, true);
                else if (isGhost && playerInRange)
                    GhostCore.SetGhost(npc, false);
            }
        }

        private static bool IsAnyPlayerInRange(NPC npc, int thresholdX, int thresholdY)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
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