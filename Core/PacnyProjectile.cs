using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using System.IO;
using Terraria.ModLoader.IO;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using PacnyRefresh.Core.Helpers;
using PacnyRefresh.Content.Gores;
using static Terraria.ModLoader.ModContent;

namespace PacnyRefresh.Core
{
    public class PacnyProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public float critDamageMod = 0f;

        public int summonCritChance = 0;

        public float gravity = 0f;
        public int gravityDelay = 0;
        public int spawnTime = 0;
        public int counter = 0;

        public override void AI(Projectile projectile)
        {
            spawnTime--;
            gravityDelay--;
            counter++;
            
            if ((projectile.type == ProjectileID.FallingStar) && counter % 15 == 0)
            {
                //DustHelper.DrawStar(projectile.Center, DustID.FireworkFountain_Blue, 5, 1.8f, 0.65f, 0.55f, 0.6f, 0.5f, true, 0, -1);
            }
            if (gravity != 0f && gravityDelay <= 0)
            {
                projectile.velocity.Y += gravity;
            }
        }
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_ItemUse realSource)
            {
                projectile.GetGlobalProjectile<PacnyProjectile>().critDamageMod += realSource.Item.GetGlobalItem<PacnyItem>().critDamageMod;
                projectile.netUpdate = true;
            }
            if (source is EntitySource_Parent parent && parent.Entity is Projectile proj)
            {
                projectile.GetGlobalProjectile<PacnyProjectile>().critDamageMod = proj.GetGlobalProjectile<PacnyProjectile>().critDamageMod;
            }
        }

        public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(counter);
            binaryWriter.Write(gravity);
            binaryWriter.Write(gravityDelay);
            binaryWriter.Write(critDamageMod);
        }

        public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
        {
            counter = binaryReader.ReadInt32();
            gravity = binaryReader.ReadInt32();
            gravityDelay = binaryReader.ReadInt32();
            critDamageMod = binaryReader.ReadSingle();
        }

        public override bool? CanHitNPC(Projectile projectile, NPC target)
        {
            if (spawnTime > 0)
                return false;
            return base.CanHitNPC(projectile, target);
        }
        public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
        {
            if (projectile.type == ProjectileID.FallingStar)
            {
                //DustHelper.DrawStar(projectile.Center, DustID.FireworkFountain_Yellow, 5, 2.6f, 1f, 0.55f, 0.6f, 0.5f, true, 0, -1);
                //DustHelper.DrawStar(projectile.Center, DustID.FireworkFountain_Blue, 5, 4.8f, 1.25f, 0.7f, 0.6f, 0.5f, true, 0, -1);
                /*for (int i = 0; i < 2; i++)
                {
                    Gore gore2 = Gore.NewGoreDirect(null, projectile.Center, Vector2.Zero, GoreType<ConstellationGore>());
                    gore2.rotation = MathHelper.ToRadians(Main.rand.NextFloat(240, 420)).RandNeg();
                    gore2.velocity.X *= 2f;
                    gore2.velocity.Y = Main.rand.NextFloat(-3f, 2f);
                    gore2.frame = 2;
                    gore2.alpha += Main.rand.Next(20, 50);
                }*/
                int bigStar = 4;
                for (int i = 0; i < 6; i++)
                {
                    Gore gore3 = Gore.NewGoreDirect(null, projectile.Center, Vector2.Zero, GoreType<StarGore>());
                    gore3.rotation = MathHelper.ToRadians(Main.rand.NextFloat(580, 780)).RandNeg();
                    gore3.velocity.X *= 1.85f;
                    gore3.velocity.Y = Main.rand.NextFloat(-2f, 1f);
                    gore3.frame = (byte)Main.rand.Next(2);
                    if (Main.rand.NextBool(bigStar))
                    {
                        gore3.frame = 2;
                        bigStar += 2;
                    }
                    gore3.alpha += Main.rand.Next(0, 30);
                }
            }

            return base.OnTileCollide(projectile, oldVelocity);
        }
    }
}
