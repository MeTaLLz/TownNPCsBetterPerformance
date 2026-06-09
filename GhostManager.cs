using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using static TownNPCsFreeze.ModInstanceManager;

namespace TownNPCsFreeze
{
    public static class GhostManager
    {
        private const float GhostFlag = -9999f;    
        private static int _cachedThresholdX;
        private static int _cachedThresholdY;
        private static HashSet<int> _configExcludedCache = new();
        private static HashSet<int> _oldConfigExcludedCache = new();
        private static int _frameCounter;
        private static HashSet<int> _excludedNPCs = new();

        public static bool RegisterExcludedNPC(int npcType)
        {
            if (_excludedNPCs.Contains(npcType))
                return false;
            
            _excludedNPCs.Add(npcType);
            
            string npcName = Lang.GetNPCNameValue(npcType);
            ModLogger.Log("ExcludedAdded", npcName, npcType);
            
            return true;
        }

        public static bool UnregisterExcludedNPC(int npcType)
        {
            if (!_excludedNPCs.Contains(npcType))
                return false;
            
            _excludedNPCs.Remove(npcType);
            
            string npcName = Lang.GetNPCNameValue(npcType);
            ModLogger.Log("ExcludedRemoved", npcName, npcType);
            
            return true;
        }

        private static bool IsExcludedFromGhost(NPC npc)
        {
            // Check config and API exclusions
            if (_excludedNPCs.Contains(npc.type)) return true;
            
            if (_configExcludedCache.Contains(npc.type)) return true;
            
            return false;
        }

        public static void ProcessDistantGhost()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            
            if (Config == null) return;
            if (Config.ConfigRefreshInterval <= 0) return;
            
            // Update cached config values periodically
            _frameCounter++;
            if (_frameCounter >= Config.ConfigRefreshInterval)
            {
                _frameCounter = 0;
                _cachedThresholdX = Config.LoadDistanceX;
                _cachedThresholdY = Config.LoadDistanceY;
            }

            int thresholdX = _cachedThresholdX;
            int thresholdY = _cachedThresholdY;

            // If any axis is zero, freeze all NPCs
            if (thresholdX == 0 || thresholdY == 0)
            {
                FreezeAllNPCs();
                return;
            }

            float inv16 = 1f / 16f;
            
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || !npc.townNPC) continue;
                if (IsExcludedFromGhost(npc)) continue;

                bool isGhost = npc.ai[3] == GhostFlag;
                bool playerInRange = IsAnyPlayerInRange(npc, thresholdX, thresholdY, inv16);
                
                // Freeze if far from player, unfreeze if close
                if (!isGhost && !playerInRange)
                    SetGhost(npc, true);
                else if (isGhost && playerInRange)
                    SetGhost(npc, false);
            }
        }

        private static void FreezeAllNPCs()
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.townNPC && !IsExcludedFromGhost(npc) && npc.ai[3] != GhostFlag)
                    SetGhost(npc, true);
            }
        }

        private static bool IsAnyPlayerInRange(NPC npc, int thresholdX, int thresholdY, float inv16)
        {
            for (int p = 0; p < Main.maxPlayers; p++)
            {
                Player pl = Main.player[p];
                if (pl?.active == true && !pl.dead)
                {
                    // Convert pixels to tiles and check axis-aligned rectangle
                    float dx = Math.Abs((pl.Center.X - npc.Center.X) * inv16);
                    if (dx > thresholdX) continue;
                    
                    float dy = Math.Abs((pl.Center.Y - npc.Center.Y) * inv16);
                    if (dy <= thresholdY) return true;
                }
            }
            return false;
        }

        public static void RestoreAllGhost()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;

            int count = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.townNPC && npc.ai[3] == GhostFlag && !IsExcludedFromGhost(npc))
                {
                    SetGhost(npc, false);
                    count++;
                }
            }
            if (count > 0) ModLogger.Log("RestoreAll", count);
        }

            public static void UnfreezeExcluded()
            {
                if (Main.netMode == NetmodeID.MultiplayerClient) return;

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && npc.townNPC && npc.ai[3] == GhostFlag && IsExcludedFromGhost(npc))
                    {
                        SetGhost(npc, false);
                    }
                }
            }

        public static void SetGhost(NPC npc, bool ghost)
        {
            if (ghost)
            {
                // Freeze
                npc.ai[3] = GhostFlag;
                npc.velocity = Vector2.Zero;
                npc.oldVelocity = Vector2.Zero;
                npc.frameCounter = 0;
                npc.noTileCollide = true;
                npc.immortal = true;
                npc.dontTakeDamage = true;
                npc.hide = true;
                npc.netUpdate = true;
                ModLogger.Log("Frozen", npc.GivenName, Lang.GetNPCNameValue(npc.type));
                
                if (Main.netMode == NetmodeID.Server)
                    PacketHelper.SendVisualSync(npc);
            }
            else if (npc.ai[3] == GhostFlag)
            {
                // Unfreeze
                npc.ai[3] = 0f;
                npc.noTileCollide = false;
                npc.immortal = false;
                npc.dontTakeDamage = false;
                npc.hide = false;
                npc.netUpdate = true;
                npc.localAI[0] = 0f;
                npc.localAI[1] = 0f;
                npc.localAI[2] = 0f;
                npc.localAI[3] = 0f;
                PacketHelper.SendLocalAISync(npc);
                ModLogger.Log("Thawed", npc.GivenName, Lang.GetNPCNameValue(npc.type));
                
                if (Main.netMode == NetmodeID.Server)
                    PacketHelper.SendVisualSync(npc);
            }

            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
        }

        public static void RefreshConfigCache()
        {
            if (Config == null) return;
            _cachedThresholdX = Config.LoadDistanceX;
            _cachedThresholdY = Config.LoadDistanceY;
            _frameCounter = 0;
            
            _configExcludedCache.Clear();
            foreach (var def in Config.ExcludedNPCs)
                _configExcludedCache.Add(def.Type);
        }
    }
}