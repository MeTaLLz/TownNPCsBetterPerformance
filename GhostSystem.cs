using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static TownNPCsFreeze.ModInstanceManager;

namespace TownNPCsFreeze
{
    public class GhostSystem : ModSystem
    {
        private const float GhostFlag = -9999f;
        
        private int _distantGhostCounter = 0;
        private int _teleportCounter = 0;
        private int _syncCounter = 0;
        
        // Cache last known home positions to detect changes
        private Dictionary<int, Point> _lastHome = new();
        private Dictionary<int, bool> _lastHomeless = new();
        
        private static Action<NPC, int> _updateNetworkCodeDelegate;

        public override void Load()
        {
            ModAPI.Initialize();
            On_NPC.UpdateNPC += On_NPC_UpdateNPC;
            HomeManager.InitDelegates();
            
            var method = typeof(NPC).GetMethod("UpdateNetworkCode", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            
            if (method != null)
                _updateNetworkCodeDelegate = (Action<NPC, int>)Delegate.CreateDelegate(
                    typeof(Action<NPC, int>), method);
        }

        public override void Unload()
        {
            On_NPC.UpdateNPC -= On_NPC_UpdateNPC;
        }

        private void On_NPC_UpdateNPC(On_NPC.orig_UpdateNPC orig, NPC self, int i)
        {
            // When API says mod is disabled, run vanilla AI without freezing
            if (!ModAPI.IsEnabled())
            {
                orig(self, i); 
                return;
            }
            
            // Frozen NPCs skip AI completely
            if (self.active && self.ai[3] == GhostFlag)
                return;
            orig(self, i);
        }

        private void SyncFrozenNPCs()
        {
            if (Main.netMode != NetmodeID.Server || _updateNetworkCodeDelegate == null)
                return;
            
            _syncCounter++;
            if (_syncCounter < Config.UpdateInterval)
                return;
            _syncCounter = 0;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || !npc.townNPC || npc.ai[3] != GhostFlag)
                    continue;

                // Check if home data changed since last sync
                _lastHome.TryGetValue(i, out Point lastHome);
                _lastHomeless.TryGetValue(i, out bool lastHomeless);
                
                bool homeChanged = lastHome.X != npc.homeTileX || lastHome.Y != npc.homeTileY;
                bool homelessChanged = lastHomeless != npc.homeless;
                
                if (!homeChanged && !homelessChanged)
                    continue;

                // Update cache and trigger network sync
                _lastHome[i] = new Point(npc.homeTileX, npc.homeTileY);
                _lastHomeless[i] = npc.homeless;
                
                _updateNetworkCodeDelegate(npc, i);
                
                if (Config.LogToChat)
                    ModLogger.Log("SyncHome", npc.GivenName, Lang.GetNPCNameValue(npc.type), npc.homeTileX, npc.homeTileY, npc.homeless);
            }
        }

        public override void PostUpdateEverything()
        {
            if (!ModAPI.IsEnabled()) return;
            SyncFrozenNPCs();

            if (Config.DistantGhost)
            {
                _distantGhostCounter++;
                if (_distantGhostCounter >= Config.UpdateInterval)
                {
                    _distantGhostCounter = 0;
                    GhostManager.ProcessDistantGhost();
                }
            }

            _teleportCounter++;
            if (_teleportCounter >= Config.UpdateInterval)
            {
                _teleportCounter = 0;
                HomeManager.ProcessGhostTeleport();
            }
        }
    }
}