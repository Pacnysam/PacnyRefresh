using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace PacnyRefresh.Core.Helpers
{
    public static class ColorHelper
    {
        public static Color Prefix(bool good = true) => good ? new(120, 190, 120) : new(190, 120, 120);

        public readonly struct Gradient(List<(Color, float)> points)
        {
            public Color GetColor(float position)
            {
                List<(Color, float)> colorPoints = [.. points.OrderBy(point => point.Item2)];

                if (colorPoints.First().Item2 > 0f) colorPoints.Insert(0, (colorPoints.First().Item1, 0f));
                if (colorPoints.Last().Item2 < 1f) colorPoints.Add(new(colorPoints.Last().Item1, 1f));

                for (int i = 0; i < colorPoints.Count - 1; i++)
                {
                    if (colorPoints.ElementAt(i + 1).Item2 > position)
                    {
                        float pos = Utils.Remap(position, colorPoints.ElementAt(i).Item2, colorPoints.ElementAt(i + 1).Item2, 0f, 1f);
                        return colorPoints.ElementAt(i).Item1.Lerp(colorPoints.ElementAt(i + 1).Item1, pos);
                    }
                }
                return colorPoints.Last().Item1;
            }
        }
        public static Gradient QuickGradient(List<Color> colors, bool loop = true)
        {
            List<(Color, float)> colorPoints = [];
            for (int i = 0; i <= colors.Count - 1; i++)
            {
                colorPoints.Add(new(colors[i], Utils.Remap(i, 0f, loop ? colors.Count : colors.Count - 1, 0f, 1f)));
            }
            if (loop) colorPoints.Add(new(colors[0], 1f));
            return new Gradient(colorPoints);
        }

        public static Color Lerp(this Color baseColor, Color targetColor, float amount) => Color.Lerp(baseColor, targetColor, amount);

        public static Color MultiplyAlpha(this Color color, float alpha) => new Color(color.R, color.G, color.B, (int)((color.A / 255f) * Math.Clamp(alpha, 0f, 1f) * 255));
        public static Color Alpha(this Color color, int alpha = 0) => color with { A = (byte)Math.Clamp(alpha, 0, 255) };
        public static Color Alpha(this Color color, float alpha) => color with { A = (byte)(Math.Clamp(alpha, 0f, 1f) * 255) };

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
                case 5: //ruby
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

        public static Gradient PrideFlag(bool loop = true) => QuickGradient([new Color(228, 3, 3), new Color(255, 140, 0), new Color(255, 237, 0), new Color(0, 138, 38), new Color(0, 76, 255), new Color(115, 41, 130)], loop);
        public static Gradient LesbianFlag(bool loop = true) => QuickGradient([new Color(214, 44, 0), new Color(255, 153, 86), Color.LightYellow, new Color(211, 98, 164), new Color(164, 1, 98)], loop);
        public static Gradient GayFlag(bool loop = true) => QuickGradient([new Color(27, 136, 107), new Color(144, 218, 181), Color.White, new Color(119, 165, 214), new Color(64, 37, 116)], loop);
        public static Gradient BiFlag(bool loop = true) => new(!loop ? [(new Color(214, 2, 112), 0.35f), (new Color(155, 79, 150), 0.5f), (new Color(0, 56, 168), 0.65f)] :
            [(new Color(107, 29, 140), 0f), (new Color(214, 2, 112), 0.1f), (new Color(214, 2, 112), 0.35f), (new Color(155, 79, 150), 0.5f), (new Color(0, 56, 168), 0.65f), (new Color(0, 56, 168), 0.9f), (new Color(107, 29, 140), 1f)]);
        public static Gradient TransFlag(bool loop = true) => QuickGradient([new Color(89, 206, 249), new Color(244, 170, 183), Color.White, new Color(244, 170, 183), new Color(89, 206, 249)], loop);
        public static Gradient AceFlag(bool loop = true) => QuickGradient([new Color(30, 30, 30), new Color(160, 160, 160), Color.White, new Color(154, 7, 121)], loop);

        /// <summary>
        /// Overhealth color (R:19,G:223,B:229,A:255).
        /// </summary>
        public static Color Overhealth => new(19, 223, 229, 255);
        /// <summary>
		/// SlimeBlue color (R:0,G:80,B:255,A:125-175).
		/// </summary>
        public static Color SlimeBlue => new Color(0, 80, 255, 255);
        /// <summary>
		/// SlimeBlueSimple color (R:112,G:172,B:244,A:255).
		/// </summary>
        public static Color SlimeBlueSimple => new Color(112, 172, 244, 255);
    }
}
