using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace TownNPCsFreeze
{
    public static class HomeSyncManager
    {
        private static readonly Dictionary<int, Point> _lastHome = [];
        private static readonly Dictionary<int, bool> _lastHomeless = [];
        private static Action<NPC, int> _updateNetworkCodeDelegate;

        public static void Init(Action<NPC, int> delegateMethod)
        {
            _updateNetworkCodeDelegate = delegateMethod;
        }

        public static void SyncHomes()
        {
            if (Main.netMode != NetmodeID.Server || _updateNetworkCodeDelegate == null)
                return;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || !npc.townNPC || npc.ai[3] != ModConstants.GhostFlag)
                    continue;

                if (!_lastHome.TryGetValue(i, out Point lastHome))
                {
                    UpdateHomeCache(i, npc);
                    continue;
                }

                if (!IsHomeChanged(npc, lastHome))
                    continue;

                UpdateHomeCache(i, npc);
                _updateNetworkCodeDelegate(npc, i);

                if (ConfigCache.LogToChat)
                    ModLogger.Log("SyncHome", npc.GivenName, Lang.GetNPCNameValue(npc.type),
                        npc.homeTileX, npc.homeTileY);
            }
        }

        private static bool IsHomeChanged(NPC npc, Point lastHome)
        {
            bool homelessChanged = _lastHomeless.TryGetValue(npc.whoAmI, out bool lastHomeless) &&
                                lastHomeless != npc.homeless;

            return lastHome.X != npc.homeTileX ||
                lastHome.Y != npc.homeTileY ||
                homelessChanged;
        }

        private static void UpdateHomeCache(int index, NPC npc)
        {
            _lastHome[index] = new Point(npc.homeTileX, npc.homeTileY);
            _lastHomeless[index] = npc.homeless;
        }
    }
}