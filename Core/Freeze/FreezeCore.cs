using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace TownNPCsFreeze
{
    public static class FreezeCore
    {
        public static void SetFreeze(NPC npc, bool freeze)
        {
            if (freeze)
                ApplyFreeze(npc);
            else if (npc.ai[3] == ModConstants.FreezeFlag)
                ApplyThaw(npc);
                
            if (NetmodeHelper.IsServer)
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
        }

        public static void FreezeAll()
        {
            if (NetmodeHelper.IsMultiplayerClient) return;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.townNPC && !ExclusionHandler.IsExcluded(npc) && npc.ai[3] != ModConstants.FreezeFlag)
                    SetFreeze(npc, true);
            }
        }

        public static void ThawAll()
        {
            if (NetmodeHelper.IsMultiplayerClient) return;

            int count = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.townNPC && npc.ai[3] == ModConstants.FreezeFlag && !ExclusionHandler.IsExcluded(npc))
                {
                    SetFreeze(npc, false);
                    count++;
                }
            }
        }

        public static void ThawExcluded()
        {
            if (NetmodeHelper.IsMultiplayerClient) return;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.townNPC && npc.ai[3] == ModConstants.FreezeFlag && ExclusionHandler.IsExcluded(npc))
                {
                    SetFreeze(npc, false);
                }
            }
        }

        public static void ThawByType(int npcType)
        {
            if (NetmodeHelper.IsMultiplayerClient) return;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.type == npcType && npc.ai[3] == ModConstants.FreezeFlag)
                {
                    SetFreeze(npc, false);
                }
            }
        }

        public static void ApplyFreeze(NPC npc)
        {
            npc.ai[3] = ModConstants.FreezeFlag;
            npc.velocity = Vector2.Zero;
            npc.oldVelocity = Vector2.Zero;
            npc.noTileCollide = true;
            npc.noGravity = true;
            npc.immortal = true;
            npc.dontTakeDamage = true;
            npc.dontTakeDamageFromHostiles = true;
            npc.chaseable = false;
            npc.Opacity = 0.5f;
            // npc.hide = true;
            npc.netUpdate = true;
            
            ChatLogger.Log("{0} ({1}) frozen", npc.GivenName, Lang.GetNPCNameValue(npc.type));

            if (NetmodeHelper.IsServer)
            {
                PacketSender.SendVisualSync(npc);
            }
        }

        public static void ApplyThaw(NPC npc)
        {
            npc.ai[3] = 0f;
            npc.noTileCollide = false;
            npc.noGravity = false;

            if (!ConfigCache.MakeInvincible)
            {
                npc.immortal = false;
            }

            npc.dontTakeDamage = false;
            npc.dontTakeDamageFromHostiles = false;
            npc.chaseable = true;
            npc.Opacity = 1f;
            // npc.hide = false;
            npc.netUpdate = true;
            
            ChatLogger.Log("{0} ({1}) thawed", npc.GivenName, Lang.GetNPCNameValue(npc.type));

            if (NetmodeHelper.IsServer)
            {
                PacketSender.SendVisualSync(npc);
            }
        }
    }
}