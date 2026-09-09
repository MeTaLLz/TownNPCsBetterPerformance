using Terraria;
using Terraria.ModLoader;

namespace TownNPCsFreeze
{
    public class UpdateSystem : ModSystem
    {
        private int _distantFreezeCounter;
        private int _teleportCounter;
        private int _syncCounter;
        private int _invincibleCounter;
        private int _bossFightCounter;

        public override void Load()
        {
            ModAPI.Initialize();
            DelegateRegistry.Initialize();

            HomeTeleportHandler.SetDelegates(
                DelegateRegistry.TeleportHome,
                DelegateRegistry.IsInGoodRestingSpot,
                DelegateRegistry.FindGoodRestingSpot
            );
            HomeSyncHandler.SetDelegates(DelegateRegistry.UpdateNetworkCode);

            On_NPC.UpdateNPC += On_NPC_UpdateNPC;
            
            var config = ModContent.GetInstance<TNBPConfig>();
            ConfigCache.Update(config);
            
            DistantFreezeHandler.SyncWithConfig();
            BossFreezeHandler.SyncWithConfig();
            InvincibleHandler.SyncWithConfig();
        }

        public override void Unload()
        {
            On_NPC.UpdateNPC -= On_NPC_UpdateNPC;
        }

        public override void OnWorldUnload()
        {
            FreezeCore.ThawAll();
            FreezeCore.ThawExcluded();
        }

        private void On_NPC_UpdateNPC(On_NPC.orig_UpdateNPC orig, NPC self, int i)
        {
            if (!ModAPI.IsEnabled())
            {
                orig(self, i);
                return;
            }

            if (self.active && self.ai[3] == ModConstants.FreezeFlag)
                return;

            orig(self, i);
        }

        public override void PostUpdateEverything()
        {
            if (!ModAPI.IsEnabled()) return;

            ProcessBossFreeze();
            ProcessDistantFreeze();
            ProcessHomeTeleport();
            ProcessHomeSync();
            ProcessInvincible();
        }

        private void ProcessDistantFreeze()
        {
            if (!ConfigCache.DistantFreeze) return;
            if (!TickInterval(ref _distantFreezeCounter, ConfigCache.FreezeInterval)) 
                return;
            DistantFreezeHandler.ProcessDistantFreeze();
        }

        private void ProcessBossFreeze()
        {
            if (!ConfigCache.BossFreeze) return;
            if (!TickInterval(ref _bossFightCounter, ConfigCache.FreezeInterval)) 
                return;
            BossFreezeHandler.ProcessBossFreeze();
        }

        private void ProcessHomeTeleport()
        {
            if (!TickInterval(ref _teleportCounter, ConfigCache.TeleportInterval)) 
                return;
            HomeTeleportHandler.ProcessTeleportFrozen();
        }

        private void ProcessHomeSync()
        {
            if (!TickInterval(ref _syncCounter, ConfigCache.SyncInterval)) 
                return;
            HomeSyncHandler.SyncHomes();
        }

        private void ProcessInvincible()
        {
            if (!TickInterval(ref _invincibleCounter, ConfigCache.InvincibleInterval)) 
                return;
            InvincibleHandler.ApplyToAll();
        }

        private static bool TickInterval(ref int counter, int interval)
        {
            counter++;
            if (counter < interval) return false;
            counter = 0;
            return true;
        }
    }
}