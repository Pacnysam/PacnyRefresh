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

        public override void SetDefaults(Item entity)
        {
            if (entity.type == ItemID.BlueMoon)
            {
                entity.shoot = ProjectileType<BlueMoonP>();
                //entity.damage = 23;
                entity.width = 34; entity.height = 32;
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

        ref float Charge => ref Projectile.ai[0];
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

            throwRange = Math.Clamp(Charge * 4, 220, 360);
            swingDistance = 10 + (int)(Charge / 2);
            swingSpeed = 2.5f + (Charge / 20);

            if (Charge == CHARGETIME && Main.myPlayer == Projectile.owner)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath7, Projectile.position);

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
                if (Main.myPlayer == Projectile.owner)
                {
                    timesSlammed++;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ProjectileType<WaterBurst>(), 0, 0, player.whoAmI);

                    for (float k = 0; k < Main.rand.Next(9, 11); k++)
                    {
                        Vector2 newVelocity = new(Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-18f, 9f));

                        if (State != (int)FlailStates.Dropping)
                        {
                            newVelocity = new Vector2(Projectile.velocity.X * 0.3f, Projectile.velocity.Y) + new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-7f, 5f));
                        }

                        var proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, newVelocity, ProjectileID.WaterBolt, (int)(Projectile.damage * 0.65f), 0, player.whoAmI, 3);
                        proj.GetGlobalProjectile<PacnyProjectile>().gravity = 0.55f;
                        proj.DamageType = DamageClass.Melee;
                    }
                }
                Helper.AddScreenshake(Main.LocalPlayer, 18, Projectile.Center);

                SoundEngine.PlaySound(new SoundStyle("PacnyRefresh/Sounds/HeavyThump") { Volume = 0.5f, PitchVariance = 0.5f }, Projectile.position);
                SoundEngine.PlaySound(new SoundStyle("PacnyRefresh/Sounds/JellyfishMiniDeath") { Volume = 1.5f, PitchVariance = 0.6f }, Projectile.position);
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

            /*Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle frame = new(0, texture.Height / Main.projFrames[Type] * Projectile.frame, texture.Width, (texture.Height / Main.projFrames[Type]) - 2);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;*/
            return true;

            /*Vector2 drawOrigin = new(tex.Width() * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Main.spriteBatch.Draw(texture, drawPos, null, Color.White * (1.0f - (0.08f * k)), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            }

            Texture2D glowTex = Request<Texture2D>("GoldLeaf/Items/Nightshade/VampireBatGlowOutline").Value;
            Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, ColorHelper.AdditiveWhite * 0.5f, Projectile.rotation, new Vector2(0, -8) + glowTex.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
            return true;*/
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
            if (++Projectile.frameCounter >= 4)
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
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame)), lightColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame)), ColorHelper.AdditiveWhite * 0.3f, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Request<Texture2D>("PacnyRefresh/Items/Dungeon/WaterBurstGlow").Value, Projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame)), ColorHelper.AdditiveWhite * 0.3f, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
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
