using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using static Terraria.ModLoader.ModContent;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.Audio;
using ReLogic.Content;

namespace PacnyRefresh.Core
{
    [ReinitializeDuringResizeArrays]
    public static class ItemSets
    {
        public static (Asset<Texture2D>, Color, bool)[] Glowmask = ItemID.Sets.Factory.CreateNamedSet("Glowmask")
            .Description("Bool automatically draws an in world glowmask")
            .RegisterCustomSet<(Asset<Texture2D>, Color, bool)>((null, Color.White, false));
    }
    public static partial class ProjectileSets
    {
        public static bool[] summonSpeedImmune = ProjectileID.Sets.Factory.CreateNamedSet("SummonSpeedImmune")
            .RegisterBoolSet(false, ProjectileID.Spazmamini, ProjectileID.DeadlySphere);
    }
    public static class BuffSets
    {
        public static bool[] Cooldown = BuffID.Sets.Factory.CreateNamedSet("Cooldown")
            .RegisterBoolSet(false, BuffID.PotionSickness, BuffID.ManaSickness, BuffID.ChaosState, BuffID.BrainOfConfusionBuff);

        public static bool[] Cosmetic = BuffID.Sets.Factory.CreateNamedSet("Cosmetic")
            .RegisterBoolSet(false, BuffID.Slimed, BuffID.GelBalloonBuff, BuffID.Lovestruck, BuffID.Stinky, BuffID.Wet);

        public static bool[] NoCleanseTooltip = BuffID.Sets.Factory.CreateNamedSet("NoCleanseTooltip")
            .Description("Does not display tooltip for being cleansable")
            .RegisterBoolSet(false, BuffID.StarInBottle);

        public static bool[] IsRemovable = BuffID.Sets.Factory.CreateNamedSet("IsRemovable")
            .Description("Can be removed by safety blanket and similar methods")
            .RegisterBoolSet(true, BuffID.MoonLeech, BuffID.TheTongue, BuffID.Obstructed, BuffID.Horrified, BuffID.Hunger, BuffID.NeutralHunger, BuffID.Starving, BuffID.StarInBottle);
    }
}
