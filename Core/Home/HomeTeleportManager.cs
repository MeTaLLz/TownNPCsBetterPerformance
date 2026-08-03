using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace TownNPCsFreeze
{
    public static class HomeTeleportManager
    {
        private const float Inv16 = 1f / 16f;
        private static Action<NPC, int, int> _teleportHomeDelegate;
        private static Func<NPC, int, int, int, int, bool> _isInGoodRestingSpotDelegate;
        private static MethodInfo _findGoodRestingSpotMethod;

        public static void InitDelegates()
        {
            _teleportHomeDelegate = CreateDelegate<Action<NPC, int, int>>(
                "AI_007_TownEntities_TeleportToHome", [typeof(int), typeof(int)]);

            _isInGoodRestingSpotDelegate = CreateDelegate<Func<NPC, int, int, int, int, bool>>(
                "AI_007_TownEntities_IsInAGoodRestingSpot", [typeof(int), typeof(int), typeof(int), typeof(int)]);

            _findGoodRestingSpotMethod = typeof(NPC).GetMethod("AI_007_FindGoodRestingSpot",
                BindingFlags.NonPublic | BindingFlags.Instance, null,
                [typeof(int), typeof(int), typeof(int).MakeByRefType(), typeof(int).MakeByRefType()], null);
        }

        public static void ProcessGhostTeleport()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;

            bool isNightOrRain = !Main.dayTime || Main.raining || Main.eclipse || Main.slimeRain;
            if (!isNightOrRain) return;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.townNPC && npc.ai[3] == ModConstants.GhostFlag)
                    TryTeleportHome(npc);
            }
        }

        private static void TryTeleportHome(NPC npc)
        {
            if (npc.homeless || npc.homeTileX <= 0 || npc.homeTileY <= 0)
                return;

            bool homeChanged = (int)npc.localAI[1] != npc.homeTileX || (int)npc.localAI[2] != npc.homeTileY;
            bool positionChanged = (int)npc.localAI[3] != (int)(npc.position.X * Inv16);

            if (homeChanged || positionChanged)
                npc.localAI[0] = 0f;

            if (npc.localAI[0] == 1f)
                return;

            if (!FindIdealRestingSpot(npc, out int idealRestX, out int idealRestY))
                return;

            if (_isInGoodRestingSpotDelegate(npc, (int)(npc.Center.X * Inv16), (int)(npc.Center.Y * Inv16), idealRestX, idealRestY))
                return;

            if (IsPlayerNear(npc, idealRestX, idealRestY))
                return;

            Vector2 oldPos = npc.position;
            _teleportHomeDelegate(npc, idealRestX, idealRestY);

            if (oldPos != npc.position)
            {
                npc.localAI[0] = 1f;
                npc.localAI[1] = npc.homeTileX;
                npc.localAI[2] = npc.homeTileY;
                npc.localAI[3] = (int)(npc.position.X * Inv16);

                if (Main.netMode == NetmodeID.Server)
                {
                    PacketSender.SendLocalAISync(npc);
                    PacketSender.SendVisualSync(npc);
                }

                if (ConfigCache.LogToChat)
                    ModLogger.Log("TeleportedHome", npc.GivenName, Lang.GetNPCNameValue(npc.type));
            }
            else
            {
                npc.localAI[0] = 1f;
                if (Main.netMode == NetmodeID.Server)
                    PacketSender.SendLocalAISync(npc);
            }

            npc.netUpdate = true;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
        }

        private static T CreateDelegate<T>(string methodName, Type[] paramTypes) where T : Delegate
        {
            var method = typeof(NPC).GetMethod(methodName,
                BindingFlags.NonPublic | BindingFlags.Instance, null, paramTypes, null);
            return (T)Delegate.CreateDelegate(typeof(T), method);
        }

        private static bool FindIdealRestingSpot(NPC npc, out int restX, out int restY)
        {
            object[] args = [
                (int)(npc.Center.X * Inv16),
                (int)(npc.Center.Y * Inv16),
                0, 0
            ];

            _findGoodRestingSpotMethod.Invoke(npc, args);
            restX = (int)args[2];
            restY = (int)args[3];
            return true;
        }

        private static bool IsPlayerNear(NPC npc, int idealRestX, int idealRestY)
        {
            int npcHalfW = NPC.sWidth / 2 + NPC.safeRangeX;
            int npcHalfH = NPC.sHeight / 2 + NPC.safeRangeY;
            int homeX = idealRestX * 16 + 8;
            int homeY = idealRestY * 16 + 8;

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (!player.active || player.dead) continue;

                int playerHalfW = player.width / 2;
                int playerHalfH = player.height / 2;

                if (Math.Abs(player.Center.X - npc.Center.X) < npcHalfW + playerHalfW &&
                    Math.Abs(player.Center.Y - npc.Center.Y) < npcHalfH + playerHalfH)
                    return true;

                if (Math.Abs(player.Center.X - homeX) < npcHalfW + playerHalfW &&
                    Math.Abs(player.Center.Y - homeY) < npcHalfH + playerHalfH)
                    return true;
            }
            return false;
        }
    }
}