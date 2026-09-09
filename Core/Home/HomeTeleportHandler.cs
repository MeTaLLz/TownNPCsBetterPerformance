using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace TownNPCsFreeze
{
    public static class HomeTeleportHandler
    {
        private const float Inv16 = 1f / 16f;
        private static Action<NPC, int, int> _teleportHomeDelegate;
        private static Func<NPC, int, int, int, int, bool> _isInGoodRestingSpotDelegate;
        private static MethodInfo _findGoodRestingSpotMethod;

        public static void SetDelegates(
            Action<NPC, int, int> teleportHome,
            Func<NPC, int, int, int, int, bool> isInGoodRestingSpot,
            MethodInfo findGoodRestingSpot)
        {
            _teleportHomeDelegate = teleportHome;
            _isInGoodRestingSpotDelegate = isInGoodRestingSpot;
            _findGoodRestingSpotMethod = findGoodRestingSpot;
        }

        public static void ProcessTeleportFrozen()
        {
            if (NetmodeHelper.IsMultiplayerClient) return;

            bool isNightOrRain = !Main.dayTime || Main.raining || Main.eclipse || Main.slimeRain;
            if (!isNightOrRain) return;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.townNPC && npc.ai[3] == ModConstants.FreezeFlag)
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

            // Was teleported
            if (npc.localAI[0] == 1f)
                return;

            // Find good spot
            if (!FindIdealRestingSpot(npc, out int idealRestX, out int idealRestY))
                return;

            // Is in good spot
            if (_isInGoodRestingSpotDelegate(npc, (int)(npc.Center.X * Inv16), (int)(npc.Center.Y * Inv16), idealRestX, idealRestY))
                return;

            // Any player near
            if (IsPlayerNear(npc, idealRestX, idealRestY))
                return;

            Vector2 oldPos = npc.position;
            _teleportHomeDelegate(npc, idealRestX, idealRestY);

            if (oldPos != npc.position)
            {
                // Teleport home
                npc.localAI[0] = 1f;
                npc.localAI[1] = npc.homeTileX;
                npc.localAI[2] = npc.homeTileY;
                npc.localAI[3] = (int)(npc.position.X * Inv16);

                if (NetmodeHelper.IsServer)
                {
                    PacketSender.SendLocalAISync(npc);
                    // PacketSender.SendVisualSync(npc);
                }

                ChatLogger.Log(ColorHelper.LightBrown, "{0} ({1}) teleported home", npc.GivenName, Lang.GetNPCNameValue(npc.type));
            }
            else
            {
                // Was teleported
                npc.localAI[0] = 1f;
                if (NetmodeHelper.IsServer)
                    PacketSender.SendLocalAISync(npc);
            }

            npc.netUpdate = true;
            if (NetmodeHelper.IsServer)
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
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