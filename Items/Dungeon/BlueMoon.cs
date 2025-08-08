using System;
using static Terraria.ModLoader.ModContent;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Threading;
using System.IO;
using Terraria.ModLoader.IO;
using System.Linq;
using PacnyRefresh.Core;
using PacnyRefresh.Core.Bases.Projectiles;
using ReLogic.Content;
using Terraria.Graphics.CameraModifiers;

namespace PacnyRefresh.Items.Dungeon
{
    public class BlueMoonItem : GlobalItem
    {
        /*private static Asset<Texture2D> glowTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>("PacnyRefresh/Items/Dungeon/BlueMoonGlow");
        }*/

        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ItemID.BlueMoon;
        }
        public override void SetDefaults(Item entity)
        {
            if (entity.type == ItemID.BlueMoon)
            {
                entity.shoot = ProjectileType<BlueMoonP>();
                entity.damage = 24;
                entity.knockBack = 7.25f;
                entity.width = 34; entity.height = 32;
            }
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            string[] text =
                [
                    Language.GetTextValue("Mods.PacnyRefresh.VanillaItemTooltips.BlueMoon")
                ];

            TooltipLine knockbackTooltip = tooltips.Find(n => n.Name == "Knockback");

            if (knockbackTooltip != null)
            {
                int index = tooltips.IndexOf(knockbackTooltip);
                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i] != string.Empty)
                        tooltips.Insert(index + 1, new TooltipLine(Mod, "BlueMoonTooltip", text[i]));
                }
            }
        }
    }
    
    public class BlueMoonP : BaseFlailProjectile 
    {
        public BlueMoonP() : base(330, 2.2f, 24f, 2.5f, false, 40) { }
        
        private static Asset<Texture2D> glowTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>("PacnyRefresh/Items/Dungeon/BlueMoonPGlow");
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        ref float Charge => ref Projectile.ai[1];
        int timesSlammed = 0;
        const int CHARGETIME = 75;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(timesSlammed);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            timesSlammed = reader.ReadInt32();
        }

        public override void SetDefaults() 
        {
            Projectile.CloneDefaults(ProjectileID.BlueMoon);
            Projectile.width = 38; Projectile.height = 38;
            Projectile.aiStyle = 0;
            Projectile.scale = 1f;
        }
        
        public override void RealAI()
        {
            Projectile.rotation = Projectile.AngleFrom(Main.player[Projectile.owner].MountedCenter) + 1.57f;

            if (State == (int)FlailStates.Swinging || State == (int)FlailStates.Throwing || State == (int)FlailStates.Returning || State == (int)FlailStates.Dropping)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.DungeonWater, Projectile.velocity.X * 0.4f, Projectile.velocity.Y * 0.4f, 100, Color.White, 1f);
                dust.noGravity = true;
                dust.velocity.X /= 2f;
                dust.velocity.Y /= 2f;
            }
        }

        public override void SpinEffect(Player player)
        {
            Charge++;

            if (Charge > 100) Charge = 100;

            throwRange = Math.Clamp(Charge * 4, 180, 360);
            swingDistance = 10 + (int)(Charge / 2);
            swingSpeed = 2.5f + (Charge / 20);

            if (Charge == CHARGETIME && Main.myPlayer == Projectile.owner)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath7, Projectile.position);

                Projectile.damage = (int)(Projectile.damage * 1.5f);

                for (float k = 0; k < 6.28f; k += 6.28f / 40)
                {
                    Dust dust = Dust.NewDustPerfect(player.MountedCenter, DustID.DungeonWater, Vector2.One.RotatedBy(k) * 3.6f, 0, ColorHelper.AdditiveWhite, 2.5f);
                    dust.noGravity = true;
                }
            }
        }

        public override void ThrowEffect(Player player)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                SoundEngine.PlaySound(new SoundStyle("PacnyRefresh/Sounds/Dash") { Pitch = -0.4f, PitchVariance = 0.4f }, Projectile.position);
            }
        }

        public override void DropSlamEffect(Player player)
        {
            WaterShockwave(player);
        }

        public override void TileStrikeEffect(Player player)
        {
            WaterShockwave(player);
        }

        private void WaterShockwave(Player player) 
        {
            if (timesSlammed < 1 && Charge >= CHARGETIME && State != (int)FlailStates.Returning && State != (int)FlailStates.ReturningFinal)
            {
                timesSlammed++;
                Projectile.netUpdate = true;

                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ProjectileType<WaterBurst>(), 0, 0, player.whoAmI);

                    for (float k = 0; k < Main.rand.Next(9, 11); k++)
                    {
                        Vector2 newVelocity = new(Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-18f, 9f));

                        if (State != (int)FlailStates.Dropping)
                        {
                            newVelocity = new Vector2(Projectile.velocity.X * 0.45f, Projectile.velocity.Y * 1.45f) + new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-8f, 5.5f));
                        }

                        var proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, newVelocity, ProjectileType<WaterBoltGravity>(), (int)(Projectile.damage * 0.5f), 0, player.whoAmI, 3);
                        proj.DamageType = DamageClass.Melee;
                        proj.netUpdate = true;
                    }
                }
                
                if (!Main.dedServ)
                {
                    //Helper.AddScreenshake(Main.LocalPlayer, 18, Projectile.Center);
                    Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.One), 25f * GetInstance<GraphicsConfig>().ShakeIntensity, 6.5f, 30, 1500));
                    Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, new Vector2(Projectile.velocity.Y, Projectile.velocity.X).SafeNormalize(Vector2.One), 13.5f * GetInstance<GraphicsConfig>().ShakeIntensity, 12, 25, 1500));

                    SoundEngine.PlaySound(new SoundStyle("PacnyRefresh/Sounds/HeavyThump") { Volume = 0.5f, PitchVariance = 0.5f }, Projectile.position);
                    SoundEngine.PlaySound(new SoundStyle("PacnyRefresh/Sounds/JellyfishMiniDeath") { Volume = 1.65f, Pitch = - 0.3f, PitchVariance = 0.6f }, Projectile.position);
                }
            }
            else
            {
                SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            }
        }

        

        /*public override void ApexEffect(Player player)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ProjectileID.WaterBolt, Projectile.damage, Projectile.knockBack, player.whoAmI);
        }*/

        public override bool PreDraw(ref Color lightColor)
        {
            if (Charge >= CHARGETIME && timesSlammed < 1)
            {
                Vector2 drawOrigin = new(glowTex.Width() * 0.5f/* * 0.07f*/, Projectile.height * 0.5f/* * 0.07f*/);
                for (int k = 0; k < Projectile.oldPos.Length; k++)
                {
                    //Color color = new(0, 24 - (k * 2), 180 - (k * 5.5f)) { A = 0 };
                    Color color = new(0, 65 - (k * 4), 180 - (k * 8)) { A = 0 };
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                    Main.spriteBatch.Draw(glowTex.Value, drawPos, null, color * (0.55f - (k * 0.075f)), Projectile.oldRot[k], drawOrigin, (Projectile.scale * (1.15f - (k * 0.075f))) /* * 0.07f*/, SpriteEffects.None, 0f);
                }
            }
            return true;
        }
    }

    public class WaterBurst : ModProjectile 
    {
        public override void SetDefaults()
        {
            Projectile.damage = 0;

            Projectile.width = 134;
            Projectile.height = 104;
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 9;
        }

        public override void AI()
        {
            if (++Projectile.frameCounter >= 3)
            {
                if (Projectile.frame >= 9)
                    Projectile.Kill();

                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            Vector2 drawOrigin = new(texture.Width * 0.5f, Projectile.height * 0.5f);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame)), lightColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame)), ColorHelper.AdditiveWhite * 0.3f, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            Main.EntitySpriteDraw(Request<Texture2D>("PacnyRefresh/Items/Dungeon/WaterBurstGlow").Value, Projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame)), ColorHelper.AdditiveWhite * 0.3f, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }

    public class WaterBoltGravity : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.WaterBolt;
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.WaterBolt);
            AIType = ProjectileID.WaterBolt;
        }
        public override void AI()
        {
            Projectile.velocity.Y += 0.55f;
        }
    }

    public class BlueMoonSystem : ModSystem 
    {
        public override void PostSetupContent()
        {
            TextureAssets.Item[ItemID.BlueMoon] = Request<Texture2D>("PacnyRefresh/Items/Dungeon/BlueMoon");
        }

        public override void Unload()
        {
            TextureAssets.Item[ItemID.BlueMoon] = Request<Texture2D>($"Terraria/Images/Item_{ItemID.BlueMoon}");
        }
    }
}
