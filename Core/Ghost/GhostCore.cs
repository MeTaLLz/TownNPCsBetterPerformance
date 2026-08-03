using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace TownNPCsFreeze
{
    public static class GhostCore
    {
        public static void SetGhost(NPC npc, bool ghost)
        {
            if (ghost)
                ApplyFreeze(npc);
            else if (npc.ai[3] == ModConstants.GhostFlag)
                ApplyThaw(npc);
                
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
        }

        public static void FreezeAllNPCs()
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.townNPC && !ExclusionManager.IsExcluded(npc) && npc.ai[3] != ModConstants.GhostFlag)
                    SetGhost(npc, true);
            }
        }

        public static void ThawAllNPCs()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) 
                return;

            int count = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.townNPC && npc.ai[3] == ModConstants.GhostFlag && !ExclusionManager.IsExcluded(npc))
                {
                    SetGhost(npc, false);
                    count++;
                }
            }
        }

        public static void UnfreezeExcluded()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) 
                return;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.townNPC && npc.ai[3] == ModConstants.GhostFlag && ExclusionManager.IsExcluded(npc))
                {
                    SetGhost(npc, false);
                }
            }
        }

        public static void UnfreezeByType(int npcType)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.type == npcType && npc.ai[3] == ModConstants.GhostFlag)
                {
                    SetGhost(npc, false);
                }
            }
        }

        public static void ApplyFreeze(NPC npc)
        {
            npc.ai[0] = 0f;
            npc.ai[1] = 0f;
            npc.ai[2] = 0f;
            npc.ai[3] = ModConstants.GhostFlag;
            npc.localAI[0] = 0f;
            npc.localAI[1] = 0f;
            npc.localAI[2] = 0f;
            npc.localAI[3] = 0f;
            npc.velocity = Vector2.Zero;
            npc.oldVelocity = Vector2.Zero;
            npc.noTileCollide = true;
            npc.noGravity = true;
            npc.immortal = true;
            npc.dontTakeDamage = true;
            npc.dontTakeDamageFromHostiles = true;
            npc.chaseable = false;
            npc.hide = true;
            npc.netUpdate = true;
            
            ModLogger.Log("Frozen", npc.GivenName, Lang.GetNPCNameValue(npc.type));

            if (Main.netMode == NetmodeID.Server)
            {
                PacketSender.SendVisualSync(npc);
            }
        }

        public static void ApplyThaw(NPC npc)
        {
            npc.ai[0] = 0f;
            npc.ai[1] = 0f;
            npc.ai[2] = 0f;
            npc.ai[3] = 0f;
            npc.localAI[0] = 0f;
            npc.localAI[1] = 0f;
            npc.localAI[2] = 0f;
            npc.localAI[3] = 0f;
            npc.noTileCollide = false;
            npc.noGravity = false;

            if (!ConfigCache.MakeInvincible)
            {
                npc.immortal = false;
            }

            npc.dontTakeDamage = false;
            npc.dontTakeDamageFromHostiles = false;
            npc.chaseable = true;
            npc.hide = false;
            npc.netUpdate = true;
            
            ModLogger.Log("Thawed", npc.GivenName, Lang.GetNPCNameValue(npc.type));

            if (Main.netMode == NetmodeID.Server)
            {
                PacketSender.SendVisualSync(npc);
            }
        }
    }
}