using System.Collections.Generic;
using System.ComponentModel;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.Config;

namespace TownNPCsFreeze
{
    [BackgroundColor(78, 60, 64, 216)]
    [SliderColor(121, 85, 71, 255)]
    public class TNBPConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Header("GhostMode")]
        [DefaultValue(true)]
        [BackgroundColor(100, 75, 60, 255)]
        public bool DistantGhost = true;

        [DefaultValue(90)]
        [Range(0, 200)]
        [BackgroundColor(100, 75, 60, 255)]
        [Slider]
        public int LoadDistanceX = 90;

        [DefaultValue(65)]
        [Range(0, 200)]
        [BackgroundColor(100, 75, 60, 255)]
        [Slider]
        public int LoadDistanceY = 65;

        [DefaultValue(false)]
        [BackgroundColor(100, 75, 60, 255)]
        public bool BossGhost = false;

        [Header("Invincible")]
        [DefaultValue(false)]
        [BackgroundColor(100, 75, 60, 255)]
        public bool MakeInvincible = false;

        [Header("Exclusions")]
        [BackgroundColor(100, 75, 60, 255)]
        public List<NPCDefinition> ExcludedNPCs = [];

        [DefaultValue(true)]
        [BackgroundColor(100, 75, 60, 255)]
        public bool ExcludeTravelingMerchant = true;

        [Header("Compatibility")]
        [DefaultValue(true)]
        [BackgroundColor(100, 75, 60, 255)]
        [ReloadRequired]
        public bool WrathOfTheGodsCompatibility = true;

        [DefaultValue(true)]
        [BackgroundColor(100, 75, 60, 255)]
        [ReloadRequired]
        public bool CalamityCompatibility = true;

        [Header("Intervals")]
        [DefaultValue(12)]
        [Range(1, 120)]
        [BackgroundColor(100, 75, 60, 255)]
        [Slider]
        public int GhostInterval = 12;

        [DefaultValue(12)]
        [Range(1, 120)]
        [BackgroundColor(100, 75, 60, 255)]
        [Slider]
        public int TeleportInterval = 12;

        [DefaultValue(12)]
        [Range(1, 120)]
        [BackgroundColor(100, 75, 60, 255)]
        [Slider]
        public int SyncInterval = 12;

        [DefaultValue(60)]
        [Range(1, 120)]
        [BackgroundColor(100, 75, 60, 255)]
        [Slider]
        public int InvincibleInterval = 60;

        [Header("Debug")]
        [DefaultValue(false)]
        [BackgroundColor(100, 75, 60, 255)]
        public bool LogToChat = false;

        public override void OnChanged()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            ConfigCache.Update(this);
            
            InvincibleManager.UpdateConfig();
            InvincibleManager.OnConfigChanged();
            
            GhostCore.UnfreezeExcluded();

            DistantGhostManager.OnConfigChanged();
            BossGhostManager.OnConfigChanged();
        }
    }
}