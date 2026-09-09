using Terraria;

namespace TownNPCsFreeze
{
    public static class BossFreezeHandler
    {
        private static bool _wasBossFightActive = false;
        private static bool _prevBossFreeze = false;

        public static void SyncWithConfig()
        {
            if (NetmodeHelper.IsMultiplayerClient) return;
            if (!ConfigCache.IsValid) return;

            bool wasEnabled = _prevBossFreeze;
            _prevBossFreeze = ConfigCache.BossFreeze;

            // Apply if changed
            if (wasEnabled != ConfigCache.BossFreeze)
            {
                if (ConfigCache.BossFreeze && IsBossFightActive())
                    SetFreeze(true);
                else if (_wasBossFightActive)
                    SetFreeze(false);
            }
            // Re-apply if didn't change and boss freeze is active
            else if (ConfigCache.BossFreeze && IsBossFightActive())
            {
                FreezeCore.FreezeAll();
            }
        }

        public static void ProcessBossFreeze()
        {
            if (NetmodeHelper.IsMultiplayerClient) return;
            if (!ConfigCache.IsValid) return;
            if (!ConfigCache.BossFreeze) return;

            bool isBossFightActive = IsBossFightActive();

            if (isBossFightActive != _wasBossFightActive)
                SetFreeze(isBossFightActive);

            // For future replace. Current realisation does not take into account 
            // that new town NPCs may appear during boss freeze.
            /*
            if (isBossFightActive)
            {
                FreezeCore.FreezeAll();
                _wasBossFightActive = true;
            }
            else if (_wasBossFightActive)
            {
                SetFreeze(false);
            }
            */
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

        public static void SyncState()
        {
            _prevBossFreeze = ConfigCache.BossFreeze;
            _wasBossFightActive = IsBossFightActive() && ConfigCache.BossFreeze;
        }

        private static void SetFreeze(bool freeze)
        {
            if (freeze)
            {
                ChatLogger.Log(ColorHelper.Coral, "Boss freeze started");
                FreezeCore.FreezeAll();
                _wasBossFightActive = true;
            }
            else
            {
                ChatLogger.Log(ColorHelper.Coral, "Boss freeze ended");

                if (ConfigCache.DistantFreeze)
                {
                    DistantFreezeHandler.ProcessDistantFreeze();
                }
                else
                {
                    FreezeCore.ThawAll();
                }

                _wasBossFightActive = false;
            }
        }

        public static void ForceFreeze()
        {
            if (NetmodeHelper.IsMultiplayerClient) return;
            if (!ConfigCache.IsValid) return;
            if (!ConfigCache.BossFreeze) return;
            if (!IsBossFightActive()) return;

            SetFreeze(true);
        }

        public static void ForceThaw()
        {
            if (NetmodeHelper.IsMultiplayerClient) return;
            if (!ConfigCache.IsValid) return;

            SetFreeze(false);
        }
    }
}