using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace TownNPCsFreeze
{
    [BackgroundColor(78, 60, 64, 216)]
    [SliderColor(121, 85, 71, 255)]
    public class TNBPConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Header("FreezeMode")]
        [DefaultValue(true)]
        [BackgroundColor(100, 75, 60, 255)]
        public bool DistantFreeze = true;

        [DefaultValue(ModConstants.DefaultLoadDistanceX)]
        [Range(0, 200)]
        [BackgroundColor(100, 75, 60, 255)]
        [Slider]
        public int LoadDistanceX = ModConstants.DefaultLoadDistanceX;

        [DefaultValue(ModConstants.DefaultLoadDistanceY)]
        [Range(0, 200)]
        [BackgroundColor(100, 75, 60, 255)]
        [Slider]
        public int LoadDistanceY = ModConstants.DefaultLoadDistanceY;

        [DefaultValue(false)]
        [BackgroundColor(100, 75, 60, 255)]
        public bool BossFreeze = false;

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

        [DefaultValue(true)]
        [BackgroundColor(100, 75, 60, 255)]
        [ReloadRequired]
        public bool TerrariaAmbienceFix = true;

        [Header("Advanced")]
        [DefaultValue(ModConstants.DefaultFreezeInterval)]
        [Range(1, 120)]
        [BackgroundColor(100, 75, 60, 255)]
        [Slider]
        public int FreezeInterval = ModConstants.DefaultFreezeInterval;

        [DefaultValue(ModConstants.DefaultTeleportInterval)]
        [Range(1, 120)]
        [BackgroundColor(100, 75, 60, 255)]
        [Slider]
        public int TeleportInterval = ModConstants.DefaultTeleportInterval;

        [DefaultValue(ModConstants.DefaultSyncInterval)]
        [Range(1, 120)]
        [BackgroundColor(100, 75, 60, 255)]
        [Slider]
        public int SyncInterval = ModConstants.DefaultSyncInterval;

        [DefaultValue(ModConstants.DefaultInvincibleInterval)]
        [Range(1, 120)]
        [BackgroundColor(100, 75, 60, 255)]
        [Slider]
        public int InvincibleInterval = ModConstants.DefaultInvincibleInterval;

        [DefaultValue(false)]
        [BackgroundColor(100, 75, 60, 255)]
        public bool LogToChat = false;

        [DefaultValue(true)]
        [BackgroundColor(100, 75, 60, 255)]
        public bool ModState = true;

        public override void OnChanged()
        {
            if (NetmodeHelper.IsMultiplayerClient) return;

            ConfigCache.Update(this);

            if (!ConfigCache.ModState)
            {
                ModAPI.SetEnabled(false);
                return;
            }

            InvincibleHandler.SyncWithConfig();
            DistantFreezeHandler.SyncWithConfig();
            BossFreezeHandler.SyncWithConfig();

            ModAPI.SetEnabled(true);
            FreezeCore.ThawExcluded();
        }
    }
}