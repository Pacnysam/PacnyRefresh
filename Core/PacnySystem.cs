using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using static Terraria.ModLoader.ModContent;

namespace PacnyRefresh.Core
{
    public class PacnySystem : ModSystem
    {
        public static float rottime = 0;
        public static int Timer;

        public override void PreUpdateWorld()
        {
            //World Rotation Timer
            Timer++;
            rottime += (float)Math.PI / 60;
            if (rottime >= Math.PI * 2) rottime = 0;
        }

        public override void PostSetupContent()
        {
            TextureAssets.Item[ItemID.JungleSpores] = Request<Texture2D>("PacnyRefresh/Content/Jungle/Items/JungleSpores");
            TextureAssets.Item[ItemID.CursedFlame] = Request<Texture2D>("PacnyRefresh/Content/Corruption/Items/CursedFlame");
            TextureAssets.Item[ItemID.Ichor] = Request<Texture2D>("PacnyRefresh/Content/Crimson/Items/Ichor");
        }

        public override void Unload()
        {
            TextureAssets.Item[ItemID.JungleSpores] = Request<Texture2D>($"Terraria/Images/Item_{ItemID.JungleSpores}");
            TextureAssets.Item[ItemID.CursedFlame] = Request<Texture2D>($"Terraria/Images/Item_{ItemID.CursedFlame}");
            TextureAssets.Item[ItemID.Ichor] = Request<Texture2D>($"Terraria/Images/Item_{ItemID.Ichor}");
        }
    }
}
