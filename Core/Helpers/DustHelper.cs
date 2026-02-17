using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.UI;
using Terraria.ModLoader;
using static Terraria.WorldGen;
using static Terraria.ModLoader.ModContent;
using Terraria.Localization;
using ReLogic.Content;
using System.IO;
using System.Diagnostics.Metrics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Newtonsoft.Json.Linq;


namespace PacnyRefresh.Core.Helpers
{
    public static class DustHelper
    {
        public static float Opacity(this Dust dust) => 1f - dust.alpha / 255f;
    }
}

