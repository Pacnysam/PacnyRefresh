using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using static Terraria.ModLoader.ModContent;
using static Terraria.WorldGen;


namespace PacnyRefresh.Core
{
    public static partial class Helper
    {
        public static Vector2 ScreenSize => new(Main.screenWidth, Main.screenHeight);
        public static float WorldTimer => Main.GlobalTimeWrappedHourly;
        public static bool IsVanitySet(Player player, int head, int body, int legs)
        {
            if (player.armor[0].type == head && player.armor[10].type == ItemID.None || player.armor[10].type == head &&
                player.armor[1].type == body && player.armor[11].type == ItemID.None || player.armor[11].type == body &&
                player.armor[2].type == legs && player.armor[12].type == ItemID.None || player.armor[12].type == legs) return true;
            return false;
        }
        public static bool IsVanitySet(Player player, int head, int body)
        {
            if (player.armor[0].type == head && player.armor[10].type == ItemID.None || player.armor[10].type == head &&
                player.armor[1].type == body && player.armor[11].type == ItemID.None || player.armor[11].type == body) return true;
            return false;
        }

        public static string EmptyTexString => "PacnyRefresh/Assets/Textures/Empty";
        public static Texture2D EmptyTex => Request<Texture2D>("PacnyRefresh/Assets/Textures/Empty").Value;

        public static string SetBonusKey => Language.GetTextValue(Main.ReversedUpDownArmorSetBonuses ? "Key.UP" : "Key.DOWN");
        public static string SetBonusSecondaryKey() => Language.GetTextValue(Main.ReversedUpDownArmorSetBonuses ? "Key.DOWN" : "Key.UP");
        public static float LavaLayer => Main.maxTilesY * 0.72f;

        public static bool CheckCircularCollision(Vector2 center, int radius, Rectangle hitbox)
        {
            if (Vector2.Distance(center, hitbox.TopLeft()) <= radius) return true;
            if (Vector2.Distance(center, hitbox.TopRight()) <= radius) return true;
            if (Vector2.Distance(center, hitbox.BottomLeft()) <= radius) return true;
            return Vector2.Distance(center, hitbox.BottomRight()) <= radius;
        }

        public static float Counter(this Projectile projectile) => projectile.GetGlobalProjectile<PacnyProjectile>().counter;
        //public static float Counter(this NPC npc) => npc.GetGlobalNPC<PacnyNPC>().counter;

        public static bool CanBeStunned(this NPC npc) => !npc.boss && !NPCID.Sets.ShouldBeCountedAsBoss[npc.type] /*&& !NPCSets.ccImmune[npc.type]*/
            && npc.aiStyle != NPCAIStyleID.Worm && (npc.knockBackResist != 0f /*|| NPCSets.ccSusceptibleException[npc.type]*/);

        public static bool ZoneLava(this Player player) => player.position.Y / 16 >= Main.maxTilesY * 0.72f;
        public static bool ZoneForest(this Player Player)
        {
            return !Player.ZoneJungle
                && !Player.ZoneDungeon
                && !Player.ZoneCorrupt
                && !Player.ZoneCrimson
                && !Player.ZoneHallow
                && !Player.ZoneSnow
                && !Player.ZoneUndergroundDesert
                && !Player.ZoneGlowshroom
                && !Player.ZoneMeteor
                && !Player.ZoneBeach
                && !Player.ZoneDesert
                && Player.ZoneOverworldHeight;
        }

        public static int TimeToTicks(float hours, float min, float sec) => (int)((hours * 216000) + (min * 3600) + (sec * 60));
        public static int TimeToTicks(float min, float sec) => (int)((min * 3600) + (sec * 60));
        public static int TimeToTicks(float sec) => (int)(sec * 60);

        public static float Opacity(this Gore gore) => 1f - gore.alpha / 255f;

        public static bool Toggle(this ref bool input) => input = !input;
        public static float RandNeg(this float num) => Main.rand.NextBool() ? num : -num;
        public static int RandNeg(this int num) => Main.rand.NextBool() ? num : -num;

        public static void WritePoint16(this BinaryWriter writer, Point16 point) { writer.Write(point.X); writer.Write(point.Y); }
        public static Point16 ReadPoint16(this BinaryReader reader) => new(reader.ReadInt16(), reader.ReadInt16());

        public static float BezierEase(float time)
        {
            return time * time / (2f * (time * time - time) + 1f);
        }

        public static bool IsWeapon(this Item item) => item.type != ItemID.None && item.stack > 0 && (item.damage > 0 || item.useAmmo > 0 && item.useAmmo != AmmoID.Solution);

        public static PlayerDeathReason QuickDeathReason(string key, Player player, int variants = 0)
        {
            NetworkText text = NetworkText.FromKey("Mods.GoldLeaf.DeathReasons." + key + ((variants == 0) ? "" : ".Variant" + (1 + Main.rand.Next(variants))), player.name);
            return PlayerDeathReason.ByCustomReason(text);
        }

        public static void ReduceBuffTime(this Player player, int buffType, int timeChange)
        {
            int buffTime = player.buffTime[player.FindBuffIndex(buffType)] - timeChange;
            player.ClearBuff(buffType);
            if (buffTime > 2) player.AddBuff(buffType, buffTime);
        }

        public static bool IsTargetValid(NPC npc) => npc.active && !npc.friendly && !npc.immortal && !npc.dontTakeDamage && npc.lifeMax > 5;
        public static bool IsValid(this NPC npc) => IsTargetValid(npc);

        public static bool TileNearby(Point position, int distance, int type/*, Predicate<Tile> predicate*/)
        {
            for (int i = Math.Clamp(position.X - distance, 0, Main.maxTilesX); i <= position.X + distance || i > Main.maxTilesX; i++)
            {
                for (int j = Math.Clamp(position.Y - distance, 0, Main.maxTilesY); j <= position.Y + distance || j > Main.maxTilesY; j++)
                {
                    if (Vector2.Distance(new Vector2(position.X, position.Y), new Vector2(i, j)) <= distance && Main.tile[i, j].TileType == type)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool IsValidDebuff(Player player, int buffindex)
        {
            int bufftype = player.buffType[buffindex];
            bool vitalbuff = (BuffSets.Cooldown[bufftype] || !BuffSets.IsRemovable[bufftype] || BuffSets.Cosmetic[bufftype]);
            return player.buffTime[buffindex] > 2 && Main.debuff[bufftype] && !Main.buffNoTimeDisplay[bufftype] && !Main.vanityPet[bufftype] && !vitalbuff;
        }

        public static bool IsValidDebuff(int bufftype, int time)
        {
            bool vitalbuff = (BuffSets.Cooldown[bufftype] || !BuffSets.IsRemovable[bufftype] || BuffSets.Cosmetic[bufftype]);
            return time > 2 && Main.debuff[bufftype] && !Main.buffNoTimeDisplay[bufftype] && !Main.vanityPet[bufftype] && !vitalbuff;
        }

        public static string CoolBuffTex(string input) => (Main.AssetSourceController.ActiveResourcePackList.EnabledPacks.Select(x => x.Name).Contains("Cool Buffs") ? input + "Cool" : input);
    }

    public static class DustHelper
    {
        public static void DrawStar(Vector2 position, int dustType, float pointAmount = 5, float mainSize = 1, float dustDensity = 1, float dustSize = 1f, float pointDepthMult = 1f, float pointDepthMultOffset = 0.5f, bool noGravity = false, float randomAmount = 0, float rotationAmount = -1)
        {
            float rot;
            if (rotationAmount < 0) { rot = Main.rand.NextFloat(0, (float)Math.PI * 2); } else { rot = rotationAmount; }

            float density = 1 / dustDensity * 0.1f;

            for (float k = 0; k < 6.28f; k += density)
            {
                float rand = 0;
                if (randomAmount > 0) { rand = Main.rand.NextFloat(-0.01f, 0.01f) * randomAmount; }

                float x = (float)Math.Cos(k + rand);
                float y = (float)Math.Sin(k + rand);
                float mult = Math.Abs(k * (pointAmount / 2) % (float)Math.PI - (float)Math.PI / 2) * pointDepthMult + pointDepthMultOffset;//triangle wave function
                Dust.NewDustPerfect(position, dustType, new Vector2(x, y).RotatedBy(rot) * mult * mainSize, 0, default, dustSize).noGravity = noGravity;
            }
        }

        public static void DrawCircle(Vector2 position, int dustType, float mainSize = 1, float RatioX = 1, float RatioY = 1, float dustDensity = 1, float dustSize = 1f, float randomAmount = 0, float rotationAmount = 0, bool nogravity = false)
        {
            float rot;
            if (rotationAmount < 0) { rot = Main.rand.NextFloat(0, (float)Math.PI * 2); } else { rot = rotationAmount; }

            float density = 1 / dustDensity * 0.1f;

            for (float k = 0; k < 6.28f; k += density)
            {
                float rand = 0;
                if (randomAmount > 0) { rand = Main.rand.NextFloat(-0.01f, 0.01f) * randomAmount; }

                float x = (float)Math.Cos(k + rand) * RatioX;
                float y = (float)Math.Sin(k + rand) * RatioY;
                if (dustType == 222 || dustType == 130 || nogravity)
                {
                    Dust.NewDustPerfect(position, dustType, new Vector2(x, y).RotatedBy(rot) * mainSize, 0, default, dustSize).noGravity = true;
                }
                else
                {
                    Dust.NewDustPerfect(position, dustType, new Vector2(x, y).RotatedBy(rot) * mainSize, 0, default, dustSize);
                }
            }
        }
        public static void DrawTriangle(Vector2 position, int dustType, float size, float dustDensity = 1f, float dustSize = 2f, float rotationAmount = -1, bool noGravity = true)
        {
            float rot;
            if (rotationAmount < 0) { rot = Main.rand.NextFloat(0, (float)Math.PI * 2); } else { rot = rotationAmount; }
            float density = 1 / dustDensity * 0.1f;
            float x = 1;
            float y = 0;
            for (float k = 0; k < 6.3f; k += density)
            {
                if (k % 2.093333f <= density)
                {
                    x = (float)Math.Cos(k);
                    y = (float)Math.Sin(k);
                }
                Vector2 offsetVect = new Vector2(x, y);
                offsetVect = offsetVect.RotatedBy(2.093333f);
                offsetVect *= k % 2.093333f / 2.093333f * 2f;
                Dust.NewDustPerfect(position, dustType, (new Vector2(x, y) + offsetVect).RotatedBy(rot) * size, 0, default, dustSize).noGravity = noGravity;
                //not the cleanest, but im tired of trying, ive legit been at this for 2 hours. Maybe im missing something really obvious, but hardcode a fucking hoy
                offsetVect = new Vector2(x, y);
                offsetVect = offsetVect.RotatedBy(-1.046667);
                offsetVect *= k % 2.093333f / 2.093333f;
                Dust.NewDustPerfect(position, dustType, (new Vector2(x, y) + offsetVect).RotatedBy(rot) * size, 0, default, dustSize).noGravity = noGravity;
            }
        }
        public static void DrawDiamond(Vector2 position, int dustType, float size, float dustDensity = 1f, float dustSize = 2f, float rotationAmount = -1, bool noGravity = true)
        {
            float rot;
            if (rotationAmount < 0) { rot = Main.rand.NextFloat(0, (float)Math.PI * 2); } else { rot = rotationAmount; }
            float density = 1 / dustDensity * 0.1f;
            float x = 1;
            float y = 0;
            for (float k = 0; k < 6.3f; k += density)
            {
                if (k % 1.57f <= density)
                {
                    x = (float)Math.Cos(k);
                    y = (float)Math.Sin(k);
                }
                Vector2 offsetVect = new Vector2(x, y);
                offsetVect = offsetVect.RotatedBy(1.57f);
                offsetVect *= k % 1.57f / 1.57f;
                Dust.NewDustPerfect(position, dustType, (new Vector2(x, y) + offsetVect).RotatedBy(rot) * size, 0, default, dustSize).noGravity = noGravity;
                //not the cleanest, but im tired of trying, ive legit been at this for 2 hours. Maybe im missing something really obvious, but hardcode a fucking hoy
                offsetVect = new Vector2(x, y);
                offsetVect = offsetVect.RotatedBy(-1.57f);
                offsetVect *= k % 1.57f / 1.57f;
                Dust.NewDustPerfect(position, dustType, (new Vector2(x, y) + offsetVect).RotatedBy(rot) * size, 0, default, dustSize).noGravity = noGravity;
            }
        }

        public static void DrawDustImage(Vector2 position, int dustType, float size, string imagePath, float dustSize = 1f, bool noGravity = true, float rot = 0.34f)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                float rotation = Main.rand.NextFloat(0 - rot, rot);
                Texture2D glyphTexture = Request<Texture2D>(imagePath).Value;
                Color[] data = new Color[glyphTexture.Width * glyphTexture.Height];
                glyphTexture.GetData(data);
                for (int i = 0; i < glyphTexture.Width; i += 2)
                {
                    for (int j = 0; j < glyphTexture.Height; j += 2)
                    {
                        Color alpha = data[j * glyphTexture.Width + i];
                        if (alpha == new Color(0, 0, 0))
                        {
                            double dustX = i - glyphTexture.Width / 2;
                            double dustY = j - glyphTexture.Height / 2;
                            dustX *= size;
                            dustY *= size;
                            Dust.NewDustPerfect(position, dustType, new Vector2((float)dustX, (float)dustY).RotatedBy(rotation)).noGravity = noGravity;
                        }
                    }
                }
            }
        }

        public static void DrawDustImageRainbow(Vector2 position, float size, string imagePath, float dustSize = 1f, bool noGravity = true, float rot = 0.34f)
        {
            int red = Main.rand.Next(60, 255);
            int green = Main.rand.Next(60, 255);
            int blue = Main.rand.Next(60, 255);
            Color color = new Color(red, green, blue);
            if (Main.netMode != NetmodeID.Server)
            {
                float rotation = Main.rand.NextFloat(0 - rot, rot);
                Texture2D glyphTexture = Request<Texture2D>(imagePath).Value;
                Color[] data = new Color[glyphTexture.Width * glyphTexture.Height];
                glyphTexture.GetData(data);
                for (int i = 0; i < glyphTexture.Width; i += 2)
                {
                    for (int j = 0; j < glyphTexture.Height; j += 2)
                    {
                        Color alpha = data[j * glyphTexture.Width + i];
                        if (alpha == new Color(0, 0, 0))
                        {
                            double dustX = i - glyphTexture.Width / 2;
                            double dustY = j - glyphTexture.Height / 2;
                            dustX *= size;
                            dustY *= size;
                            Vector2 dir = new Vector2((float)dustX, (float)dustY).RotatedBy(rotation);
                            Dust.NewDustPerfect(position, 267, dir, 0, color, dustSize).noGravity = noGravity;
                        }
                    }
                }
            }
        }

        public static void DrawElectricity(Vector2 point1, Vector2 point2, int dusttype, float scale = 1, int armLength = 30, Color color = default, float density = 0.05f)
        {
            int nodeCount = (int)Vector2.Distance(point1, point2) / armLength;
            Vector2[] nodes = new Vector2[nodeCount + 1];

            nodes[nodeCount] = point2; //adds the end as the last point

            for (int k = 1; k < nodes.Count(); k++)
            {
                //Sets all intermediate nodes to their appropriate randomized dot product positions
                nodes[k] = Vector2.Lerp(point1, point2, k / (float)nodeCount) +
                    (k == nodes.Count() - 1 ? Vector2.Zero : Vector2.Normalize(point1 - point2).RotatedBy(1.58f) * Main.rand.NextFloat(-armLength / 2, armLength / 2));

                //Spawns the dust between each node
                Vector2 prevPos = k == 1 ? point1 : nodes[k - 1];
                for (float i = 0; i < 1; i += density)
                {
                    Dust d = Dust.NewDustPerfect(Vector2.Lerp(prevPos, nodes[k], i), dusttype, Vector2.Zero, 0, color, scale);
                    d.noGravity = true;
                }
            }
        }

        public static int TileDust(Tile tile, ref int dusttype)
        {
            switch (tile.TileType)
            {
                case TileID.Stone: dusttype = DustID.Stone; break;
                case TileID.Sand: case TileID.Sandstone: dusttype = 32; break;
                case TileID.Granite: dusttype = DustID.Granite; break;
                case TileID.Marble: dusttype = DustID.Marble; break;
                case TileID.Grass: case TileID.JungleGrass: dusttype = DustID.Grass; break;
                case TileID.MushroomGrass: case TileID.MushroomBlock: dusttype = 96; break;

                default:
                    if (TileID.Sets.Crimson[tile.TileType])
                        dusttype = DustID.Blood;
                    if (TileID.Sets.Corrupt[tile.TileType])
                        dusttype = 14;
                    if (TileID.Sets.Ices[tile.TileType] || TileID.Sets.IcesSnow[tile.TileType])
                        dusttype = DustID.Ice;
                    if (TileID.Sets.Snow[tile.TileType] || tile.TileType == TileID.Cloud || tile.TileType == TileID.RainCloud)
                        dusttype = 51;

                    ModTile modtile = TileLoader.GetTile(tile.TileType);
                    if (modtile != null)
                        dusttype = modtile.DustType;
                    break;
            }

            return dusttype;

        }
    }

    public static class ColorHelper
    {
        public static Color AdditiveWhite(byte alpha = 0) => new(255, 255, 255) { A = alpha };
        public static Color AdditiveWhite() => new(255, 255, 255) { A = 0 };

        public static Color GemColor(int gem)
        {
            switch (gem)
            {
                case 1: //amethyst
                case ItemID.Amethyst:
                    {
                        return new Color(193, 47, 246);
                    }
                case 2: //topaz
                case ItemID.Topaz:
                    {
                        return new Color(246, 188, 0);
                    }
                case 3: //sapphire
                case ItemID.Sapphire:
                    {
                        return new Color(86, 135, 255);
                    }
                case 4: //emerald
                case ItemID.Emerald:
                    {
                        return new Color(41, 206, 131);
                    }
                case 5: //rubyCounter
                case ItemID.Ruby:
                    {
                        return new Color(237, 26, 30);
                    }
                case 6: //diamond
                case ItemID.Diamond:
                    {
                        return Color.White;
                    }
                case 7: //amber
                case ItemID.Amber:
                    {
                        return new Color(244, 133, 27);
                    }
            }
            return Color.White;
        }

        public static Color RarityColor(int rarity)
        {
            switch (rarity)
            {
                case ItemRarityID.Gray:
                    {
                        return Colors.RarityTrash;
                    }
                case ItemRarityID.White:
                    {
                        return Color.White;
                    }
                case ItemRarityID.Blue:
                    {
                        return Colors.RarityBlue;
                    }
                case ItemRarityID.Green:
                    {
                        return Colors.RarityGreen;
                    }
                case ItemRarityID.Orange:
                    {
                        return Colors.RarityOrange;
                    }
                case ItemRarityID.LightRed:
                    {
                        return Colors.RarityRed;
                    }
                case ItemRarityID.Pink:
                    {
                        return Colors.RarityPink;
                    }
                case ItemRarityID.LightPurple:
                    {
                        return Colors.RarityPurple;
                    }
                case ItemRarityID.Lime:
                    {
                        return Colors.RarityLime;
                    }
                case ItemRarityID.Yellow:
                    {
                        return Colors.RarityYellow;
                    }
                case ItemRarityID.Cyan:
                    {
                        return Colors.RarityCyan;
                    }
                case ItemRarityID.Red:
                    {
                        return Colors.RarityDarkRed;
                    }
                case ItemRarityID.Purple:
                    {
                        return Colors.RarityDarkPurple;
                    }
                case ItemRarityID.Expert:
                    {
                        return Main.DiscoColor;
                    }
                case ItemRarityID.Master:
                    {
                        return Main.mcColor;
                    }
                case ItemRarityID.Quest:
                    {
                        return Colors.RarityAmber;
                    }
            }
            return Color.White;
        }

        /// <summary>
        /// Overhealth color (R:19,G:223,B:229,A:255).
        /// </summary>
        public static Color Overhealth => new(19, 223, 229, 255);
        /// <summary>
		/// SlimeBlue color (R:0,G:80,B:255,A:125-175).
		/// </summary>
        public static Color SlimeBlue => new(0, 80, 255, 255);
        /// <summary>
		/// SlimeBlueSimple color (R:112,G:172,B:244,A:255).
		/// </summary>
        public static Color SlimeBlueSimple => new(112, 172, 244, 255);
    }
}

