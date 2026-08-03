using System;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace TownNPCsFreeze
{
    public class UpdateSystem : ModSystem
    {
        private int _distantGhostCounter;
        private int _teleportCounter;
        private int _syncCounter;
        private int _invincibleCounter;
        private int _bossFightCounter;

        private static Action<NPC, int> _updateNetworkCodeDelegate;

        public override void Load()
        {
            ModAPI.Initialize();
            HomeTeleportManager.InitDelegates();
            On_NPC.UpdateNPC += On_NPC_UpdateNPC;

            InitUpdateNetworkCodeDelegate();
            HomeSyncManager.Init(_updateNetworkCodeDelegate);
            LoadConfig();
        }

        public override void Unload()
        {
            On_NPC.UpdateNPC -= On_NPC_UpdateNPC;
        }

        public override void PostUpdateEverything()
        {
            if (!ModAPI.IsEnabled()) return;

            ProcessBossGhost();
            ProcessDistantGhost();
            ProcessHomeSync();
            ProcessHomeTeleport();
            ProcessInvincible();
        }

        private static void InitUpdateNetworkCodeDelegate()
        {
            var method = typeof(NPC).GetMethod("UpdateNetworkCode",
                BindingFlags.NonPublic | BindingFlags.Instance);
            if (method != null)
                _updateNetworkCodeDelegate = (Action<NPC, int>)Delegate.CreateDelegate(
                    typeof(Action<NPC, int>), method);
        }

        private static void LoadConfig()
        {
            var config = ModContent.GetInstance<TNBPConfig>();
            ConfigCache.Update(config);
            InvincibleManager.UpdateConfig();
        }

        private void ProcessHomeSync()
        {
            if (!TickInterval(ref _syncCounter, ConfigCache.SyncInterval)) return;
            HomeSyncManager.SyncHomes();
        }

        private void ProcessDistantGhost()
        {
            if (!ConfigCache.DistantGhost) return;
            if (!TickInterval(ref _distantGhostCounter, ConfigCache.GhostInterval)) return;
            DistantGhostManager.ProcessDistantGhost();
        }

        private void ProcessHomeTeleport()
        {
            if (!TickInterval(ref _teleportCounter, ConfigCache.TeleportInterval)) return;
            HomeTeleportManager.ProcessGhostTeleport();
        }

        private void ProcessInvincible()
        {
            if (!TickInterval(ref _invincibleCounter, ConfigCache.InvincibleInterval)) return;
            InvincibleManager.ApplyToAll();
        }

        private void ProcessBossGhost()
        {
            if (!ConfigCache.BossGhost) return;
            if (!TickInterval(ref _bossFightCounter, ConfigCache.GhostInterval)) return;
            BossGhostManager.ProcessBossGhost();
        }

        private static bool TickInterval(ref int counter, int interval)
        {
            counter++;
            if (counter < interval) return false;
            counter = 0;
            return true;
        }

        private void On_NPC_UpdateNPC(On_NPC.orig_UpdateNPC orig, NPC self, int i)
        {
            if (!ModAPI.IsEnabled())
            {
                orig(self, i);
                return;
            }

            if (self.active && self.ai[3] == ModConstants.GhostFlag)
                return;

            orig(self, i);
        }
    }
}