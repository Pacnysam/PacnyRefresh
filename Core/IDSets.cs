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
    public static class ProjectileSets
    {
        public static bool[] summonSpeedImmune = ProjectileID.Sets.Factory.CreateNamedSet("SummonSpeedImmune")
            .RegisterBoolSet(false, ProjectileID.Spazmamini, ProjectileID.DeadlySphere);
    }
    public static class BuffSets
    {
        public static bool[] Cooldown = BuffID.Sets.Factory.CreateNamedSet("Cooldown")
            .RegisterBoolSet(false, BuffID.PotionSickness, BuffID.ManaSickness, BuffID.ChaosState);
    }
}
