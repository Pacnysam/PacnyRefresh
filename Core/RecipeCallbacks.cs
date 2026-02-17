using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PacnyRefresh.Content.Dusts;
using PacnyRefresh.Content.Gores;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static PacnyRefresh.Core.Helper;
using static Terraria.ModLoader.ModContent;

namespace PacnyRefresh.Core
{
    public static class RecipeCallbacks //TODO: remove all projectiles trom this
    {
        static readonly Player player = Main.LocalPlayer;

        /*public delegate void CraftEffectDelegate(Player player);
        public static event CraftEffectDelegate MajorOnCraftEvent;
        public static event CraftEffectDelegate MinorOnCraftEvent;

        public static void CraftEffects(Recipe recipe, Item item, List<Item> consumedItems, Item destinationStack)
        {
            if (player.GetModPlayer<GoldLeafPlayer>().craftTimer <= 0 && GetInstance<GraphicsConfig>().OnCraftEffects)
            {
                MajorOnCraftEvent?.Invoke(player);
            }
            MinorOnCraftEvent?.Invoke(player);
        }*/

        public static void Star(Recipe recipe, Item item, List<Item> consumedItems, Item destinationStack)
        {
            if (player.GetModPlayer<PacnyPlayer>().craftTimer <= 0) 
            {
                player.GetModPlayer<PacnyPlayer>().craftTimer = 15;

                Gore gore = Gore.NewGoreDirect(null, player.Top, Vector2.Zero, GoreType<StarGore>());
                gore.rotation = MathHelper.ToRadians(Main.rand.NextFloat(380, 780)).RandNeg();
                gore.velocity.X *= 0.65f;
                gore.velocity.Y = Main.rand.NextFloat(-8.5f, -6.5f);
                gore.frame = 1;
                gore.alpha -= Main.rand.Next(40, 70);

                for (int i = 0; i < 5; i++)
                {
                    Gore gore2 = Gore.NewGoreDirect(null, player.Top, Vector2.Zero, GoreType<StarGore>());
                    gore2.rotation = MathHelper.ToRadians(Main.rand.NextFloat(420, 820)).RandNeg();
                    gore2.velocity.X *= 1.45f;
                    gore2.velocity.Y = Main.rand.NextFloat(-4f, -1.5f);
                    gore2.frame = 0;
                    gore2.alpha += Main.rand.Next(-10, 15);
                }

                //DustHelper.DrawStar(player.MountedCenter, DustID.FireworkFountain_Yellow, 5, 2.6f, 1f, 0.55f, 0.6f, 0.5f, true, 0, -1);
                SoundEngine.PlaySound(SoundID.Item4);
            }
        }
        public static void Anvil(Recipe recipe, Item item, List<Item> consumedItems, Item destinationStack)
        {
            if (player.GetModPlayer<PacnyPlayer>().craftTimer <= 0)
            {
                player.GetModPlayer<PacnyPlayer>().craftTimer = 10;

                for (float k = 0; k < Main.rand.Next(4, 6); k++)
                {
                    Dust d = Dust.NewDustDirect(player.MountedCenter, 12, 0, DustID.Torch, 0f, Main.rand.NextFloat(-3.8f, -6.2f), 0, default, Main.rand.NextFloat(0.7f, 0.9f));
                }
                SoundEngine.PlaySound(SoundID.Item37);
            }
        }
        public static void Slime(Recipe recipe, Item item, List<Item> consumedItems, Item destinationStack)
        {
            if (player.GetModPlayer<PacnyPlayer>().craftTimer <= 0)
            {
                player.GetModPlayer<PacnyPlayer>().craftTimer = 10;

                for (int k = 0; k < 18; ++k)
                {
                    int dust = Dust.NewDust(player.position, player.width, player.height, DustType<SlimeDustBlue>(), Main.rand.NextFloat(-1.5f, 1.5f), Main.rand.NextFloat(-5f, -7f), 0, Color.White, Main.rand.NextFloat(0.8f, 1.2f));
                    Main.dust[dust].alpha = 175;
                    if (Main.rand.NextBool(2)) Main.dust[dust].alpha += 25;
                    if (Main.rand.NextBool(2)) Main.dust[dust].alpha += 25;
                }

                SoundStyle sound1 = new("PacnyRefresh/Assets/Sounds/JellyfishEggPop") { Volume = 0.65f, PitchVariance = 0.4f };
                SoundEngine.PlaySound(sound1);
            }
        }
        public static void DyeMinor(Recipe recipe, Item item, List<Item> consumedItems, Item destinationStack)
        {
            if (player.GetModPlayer<PacnyPlayer>().craftTimer <= 0)
            {
                for (int k = 0; k < 12; ++k)
                {
                    Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustType<SlimeDust>(), 0, Main.rand.NextFloat(-2.5f, -4f), 125, Color.White, Main.rand.NextFloat(1f, 1.6f));

                    if (item.dye > 0)
                        dust.shader = GameShaders.Armor.GetSecondaryShader(item.dye, Main.LocalPlayer);
                    if (item.hairDye > 0)
                        dust.shader = GameShaders.Armor.GetSecondaryShader(item.hairDye, Main.LocalPlayer);

                    if (Main.rand.NextBool()) dust.alpha += 50; if (Main.rand.NextBool()) dust.alpha += 25;
                }

                SoundEngine.PlaySound((Main.rand.NextBool() ? SoundID.Item86 : SoundID.Item87) with { MaxInstances = 2 });
            }
        }
    }
}
