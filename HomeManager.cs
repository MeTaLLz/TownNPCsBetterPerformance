using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using static TownNPCsFreeze.ModInstanceManager;

namespace TownNPCsFreeze
{
    public static class HomeManager
    {
        private static Action<NPC, int, int> _teleportHomeDelegate;
        private static Func<NPC, int, int, int, int, bool> _isInGoodRestingSpotDelegate;
        private static float _inv16 = 1f / 16f;

        public static void InitDelegates()
        {
            var teleportMethod = typeof(NPC).GetMethod("AI_007_TownEntities_TeleportToHome",
                BindingFlags.NonPublic | BindingFlags.Instance, null, [typeof(int), typeof(int)], null);
            _teleportHomeDelegate = (Action<NPC, int, int>)Delegate.CreateDelegate(typeof(Action<NPC, int, int>), teleportMethod);

            var isGoodMethod = typeof(NPC).GetMethod("AI_007_TownEntities_IsInAGoodRestingSpot",
                BindingFlags.NonPublic | BindingFlags.Instance, null, [typeof(int), typeof(int), typeof(int), typeof(int)], null);
            _isInGoodRestingSpotDelegate = (Func<NPC, int, int, int, int, bool>)Delegate.CreateDelegate(typeof(Func<NPC, int, int, int, int, bool>), isGoodMethod);
        }

        public static void ProcessGhostTeleport()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            // Vanilla conditions
            bool isNightOrRain = !Main.dayTime || Main.raining || Main.eclipse || Main.slimeRain;
            if (!isNightOrRain) return;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.townNPC && npc.ai[3] == -9999f)
                    TryTeleportHome(npc);
            }
        }

        private static void TryTeleportHome(NPC npc)
        {
            if (npc.homeless || npc.homeTileX <= 0 || npc.homeTileY <= 0) return;

            // Check if home or position changed since last teleport
            bool homeChanged = (int)npc.localAI[1] != npc.homeTileX || (int)npc.localAI[2] != npc.homeTileY;
            bool positionChanged = (int)npc.localAI[3] != (int)(npc.position.X * _inv16);
            
            if (homeChanged || positionChanged) npc.localAI[0] = 0f;
            if (npc.localAI[0] == 1f) return; // Already teleported during this freeze cycle

            int tileX = (int)(npc.Center.X * _inv16);
            int tileY = (int)(npc.Center.Y * _inv16);
            
            // Find ideal resting spot near home
            object[] findArgs = [tileX, tileY, 0, 0];
            typeof(NPC).GetMethod("AI_007_FindGoodRestingSpot", 
                BindingFlags.NonPublic | BindingFlags.Instance,
                null, [typeof(int), typeof(int), typeof(int).MakeByRefType(), typeof(int).MakeByRefType()], null)
                .Invoke(npc, findArgs);
            int idealRestX = (int)findArgs[2];
            int idealRestY = (int)findArgs[3];
            
            // Check if already in a good spot (no teleport needed)
            if (_isInGoodRestingSpotDelegate(npc, tileX, tileY, idealRestX, idealRestY)) return;

            // Check if any player is near the NPC or the target home
            int npcCenterX = (int)npc.Center.X;
            int npcCenterY = (int)npc.Center.Y;
            int npcHalfW = NPC.sWidth / 2 + NPC.safeRangeX;
            int npcHalfH = NPC.sHeight / 2 + NPC.safeRangeY;
            int homeX = idealRestX * 16 + 8;
            int homeY = idealRestY * 16 + 8;

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (!player.active || player.dead) continue;
                
                int playerCenterX = (int)player.Center.X;
                int playerCenterY = (int)player.Center.Y;
                int playerHalfW = player.width / 2;
                int playerHalfH = player.height / 2;
                
                if (Math.Abs(playerCenterX - npcCenterX) < npcHalfW + playerHalfW &&
                    Math.Abs(playerCenterY - npcCenterY) < npcHalfH + playerHalfH)
                    return; // Player near NPC
                    
                if (Math.Abs(playerCenterX - homeX) < npcHalfW + playerHalfW &&
                    Math.Abs(playerCenterY - homeY) < npcHalfH + playerHalfH)
                    return; // Player near home
            }

            // Perform the teleport
            Vector2 oldPos = npc.position;
            _teleportHomeDelegate(npc, idealRestX, idealRestY);
            
            if (oldPos != npc.position)
            {
                npc.localAI[0] = 1f;
                npc.localAI[1] = npc.homeTileX;
                npc.localAI[2] = npc.homeTileY;
                npc.localAI[3] = (int)(npc.position.X * _inv16);
                
                if (Main.netMode == NetmodeID.Server)
                {
                    PacketHelper.SendLocalAISync(npc);
                    PacketHelper.SendVisualSync(npc);
                }
                
                if (Config.LogToChat)
                    ModLogger.Log("TeleportedHome", npc.GivenName, Lang.GetNPCNameValue(npc.type));
            }
            else
            {
                // Teleport failed, prevent retry spam
                npc.localAI[0] = 1f;
                if (Main.netMode == NetmodeID.Server)
                    PacketHelper.SendLocalAISync(npc);
            }

            npc.netUpdate = true;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
        }
    }
}