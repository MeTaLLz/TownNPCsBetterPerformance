using System.Collections.Generic;
using System.ComponentModel;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.Config;

namespace TownNPCsFreeze
{
    [BackgroundColor(78, 60, 64, 216)]
    [SliderColor(121, 85, 71, 255)]
    public class TNPSConfig : ModConfig
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

        [Header("Invincible")]
        [DefaultValue(false)]
        [BackgroundColor(100, 75, 60, 255)]
        public bool MakeInvincible = false;

        [Header("Exclusions")]
        [BackgroundColor(100, 75, 60, 255)]
        public List<NPCDefinition> ExcludedNPCs = new();

        [Header("Compatibility")]
        [DefaultValue(true)]
        [BackgroundColor(100, 75, 60, 255)]
        [ReloadRequired]
        public bool WrathOfTheGodsCompatibility = true;

        [Header("Debug")]
        [DefaultValue(false)]
        [BackgroundColor(100, 75, 60, 255)]
        public bool LogToChat = false;

        [Header("Other")]
        [DefaultValue(10)]
        [Range(1, 60)]
        [BackgroundColor(100, 75, 60, 255)]
        [Slider]
        public int UpdateInterval = 10;

        [DefaultValue(15)]
        [Range(1, 60)]
        [BackgroundColor(100, 75, 60, 255)]
        [Slider]
        public int ConfigRefreshInterval = 15;

        private bool _prevDistantGhost;
        private int _prevLoadDistanceX;
        private int _prevLoadDistanceY;

        public override void OnChanged()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            bool distantChanged = DistantGhost != _prevDistantGhost;
            bool distanceXChanged = LoadDistanceX != _prevLoadDistanceX;
            bool distanceYChanged = LoadDistanceY != _prevLoadDistanceY;

            GhostManager.RefreshConfigCache();
            GhostManager.UnfreezeExcluded();

            if (distantChanged || distanceXChanged || distanceYChanged)
            {
                if (!DistantGhost)
                    GhostManager.RestoreAllGhost();
                else
                    GhostManager.ProcessDistantGhost();
            }

            _prevDistantGhost = DistantGhost;
            _prevLoadDistanceX = LoadDistanceX;
            _prevLoadDistanceY = LoadDistanceY;
        }
    }
}