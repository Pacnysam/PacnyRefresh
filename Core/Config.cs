using Terraria.ModLoader.Config;

using PacnyRefresh.Core;
using static Terraria.ModLoader.ModContent;
using static PacnyRefresh.Core.Helper;
using System.ComponentModel;

namespace PacnyRefresh.Core
{
    /*public class GameplayConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;
    }*/

    public class VisualConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [ReloadRequired]
        [DefaultValue(false)]
        public bool CoolBuffs = false;

        [Range(0f, 1f)]
        [Slider]
        [DefaultValue(1f)]
        public float ShakeIntensity = 1;
    }
}
