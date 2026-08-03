using Terraria;
using Terraria.ID;

namespace TownNPCsFreeze
{
    public static class BossGhostManager
    {
        private static bool _wasBossFightActive = false;
        private static bool _prevBossGhost = false;

        public static void SyncState()
        {
            _wasBossFightActive = IsBossFightActive() && ConfigCache.BossGhost;
        }

        public static void OnConfigChanged()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;
            if (!ConfigCache.IsValid) return;

            bool optionChanged = ConfigCache.BossGhost != _prevBossGhost;
            _prevBossGhost = ConfigCache.BossGhost;

            if (!optionChanged) return;

            if (ConfigCache.BossGhost && IsBossFightActive())
            {
                ForceFreeze();
            }
            else if (_wasBossFightActive)
            {
                ForceThaw();
            }
        }

        public static void ProcessBossGhost()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;
            if (!ConfigCache.IsValid) return;
            if (!ConfigCache.BossGhost) return;

            bool isBossFightActive = IsBossFightActive();

            if (isBossFightActive != _wasBossFightActive)
            {
                if (isBossFightActive)
                {
                    if (ConfigCache.LogToChat)
                        ModLogger.Log("BossFreezeStarted");
                    GhostCore.FreezeAllNPCs();
                }
                else
                {
                    if (ConfigCache.LogToChat)
                        ModLogger.Log("BossFreezeEnded");
                        
                    if (ConfigCache.DistantGhost)
                    {
                        DistantGhostManager.ProcessDistantGhost();
                    }
                    else
                    {
                        GhostCore.ThawAllNPCs();
                    }
                }

                _wasBossFightActive = isBossFightActive;
            }
        }

        public static void ForceFreeze()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;
            if (!ConfigCache.IsValid) return;
            if (!ConfigCache.BossGhost) return;
            if (!IsBossFightActive()) return;

            if (ConfigCache.LogToChat)
                ModLogger.Log("BossFreezeStarted");

            GhostCore.FreezeAllNPCs();
            
            _wasBossFightActive = true;
        }

        public static void ForceThaw()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;
            if (!ConfigCache.IsValid) return;

            if (ConfigCache.LogToChat)
                ModLogger.Log("BossFreezeEnded");

            if (ConfigCache.DistantGhost)
            {
                DistantGhostManager.ProcessDistantGhost();
            }
            else
            {
                GhostCore.ThawAllNPCs();
            }
            
            _wasBossFightActive = false;
        }

        public static bool IsBossFightActive()
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.boss)
                    return true;
            }
            return false;
        }

        public static void SetPrevBossGhost(bool value) => _prevBossGhost = value;
    }
}