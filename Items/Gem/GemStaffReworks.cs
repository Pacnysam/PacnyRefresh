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
using System;
using System.Threading;
using System.IO;
using Terraria.ModLoader.IO;
using System.Linq;
using Mono.Cecil;

namespace PacnyRefresh.Items.Gem
{
    /*public class GemStaffReworkItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        private readonly int[] gemStaves = [ItemID.AmethystStaff, ItemID.TopazStaff, ItemID.SapphireStaff, ItemID.EmeraldStaff, ItemID.RubyStaff, ItemID.DiamondStaff, ItemID.AmberStaff];

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (gemStaves.Contains(item.type) && GetInstance<GameplayConfig>().OreStaffReworks) 
            {
                string[] text =
                [
                    Language.GetTextValue("Mods.GoldLeaf.Items.Vanilla.GemStaves.Type" + (Array.IndexOf(gemStaves, item.type) + 1))
                ];

                TooltipLine gemStaffTooltip = tooltips.Find(n => n.Name == "UseMana");

                if (gemStaffTooltip != null)
                {
                    int index = tooltips.IndexOf(gemStaffTooltip);
                    for (int i = 0; i < text.Length; i++)
                    {
                        if (text[i] != string.Empty)
                            tooltips.Insert(index + 1, new TooltipLine(Mod, "GemStaff", text[i]));
                    }
                }
            }
        }

        public override void SetDefaults(Item item)
        {
            if (GetInstance<GameplayConfig>().OreStaffReworks) 
            {
                switch (item.type)
                {
                    case ItemID.AmethystStaff:
                        {
                            item.damage = 14;
                            item.ArmorPenetration = 10;
                            item.crit = 8;
                            break;
                        }
                    case ItemID.TopazStaff:
                        {
                            item.damage = 13;
                            break;
                        }
                    case ItemID.SapphireStaff:
                        {
                            item.damage = 17;
                            item.crit = 6;
                            break;
                        }
                    case ItemID.EmeraldStaff:
                        {
                            item.damage = 16;
                            item.shootSpeed = 14.5f;
                            item.useTime = 42;
                            item.useAnimation = 42;
                            break;
                        }
                    case ItemID.RubyStaff:
                        {
                            item.damage = 18;
                            item.crit = 3;
                            break;
                        }
                    case ItemID.DiamondStaff:
                        {
                            item.damage = 10;
                            item.useTime = 10;
                            item.useAnimation = 20;
                            break;
                        }
                    case ItemID.AmberStaff:
                        {
                            item.damage = 21;
                            item.crit = 2;
                            break;
                        }
                }
            }
        }

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (item.type == ItemID.TopazStaff && GetInstance<GameplayConfig>().OreStaffReworks)
            {
                int p = Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(Main.rand.Next(-9, 9))) * 0.85f, type, (int)(damage * 0.7f), knockback, player.whoAmI);
                Main.projectile[p].scale *= 0.6f;
            }
            if (item.type == ItemID.EmeraldStaff && GetInstance<GameplayConfig>().OreStaffReworks)
            {
                int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                Main.projectile[p].velocity.Y -= 3.8f;
                return false;
            }
            return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }

        public class GemStaffReworkProjectile : GlobalProjectile
        {
            public override bool InstancePerEntity => true;

            public override void SetDefaults(Projectile entity)
            {
                if (GetInstance<GameplayConfig>().OreStaffReworks)
                {
                    switch (entity.type)
                    {
                        case ProjectileID.AmethystBolt:
                        case ProjectileID.TopazBolt:
                            {
                                entity.timeLeft = 300;
                                break;
                            }
                        case ProjectileID.EmeraldBolt:
                            {
                                entity.timeLeft = 180;
                                break;
                            }
                        case ProjectileID.DiamondBolt:
                            {
                                entity.extraUpdates = 10;
                                entity.penetrate = 2;
                                break;
                            }
                    }
                }
            }

            public override bool PreAI(Projectile projectile)
            {
                if (projectile.type == ProjectileID.EmeraldBolt && GetInstance<GameplayConfig>().OreStaffReworks)
                {
                    projectile.velocity.Y += 0.35f;
                }
                return base.PreAI(projectile);
            }

            public override void PostAI(Projectile projectile)
            {
                if (projectile.type == ProjectileID.AmethystBolt && GetInstance<GameplayConfig>().OreStaffReworks) 
                {
                    projectile.velocity = projectile.velocity.Length() * Vector2.Lerp(projectile.velocity, projectile.DirectionTo(Main.MouseWorld) * projectile.velocity.Length() * 0.5f, 0.2f).SafeNormalize(Vector2.Normalize(projectile.velocity));
                    projectile.netUpdate = true;
                }
                if (projectile.type == ProjectileID.EmeraldBolt && projectile.timeLeft % 6 == 0 && GetInstance<GameplayConfig>().OreStaffReworks)
                {
                    int emerald = Projectile.NewProjectile(projectile.GetSource_FromAI(), new Vector2(projectile.Center.X, projectile.Center.Y + Main.rand.NextFloat(-4, 4)), Vector2.Zero, ProjectileType<FallingEmerald>(), (int)(projectile.damage * 0.55f), projectile.knockBack * 0.3f, projectile.owner);
                    Main.projectile[emerald].DamageType = DamageClass.Magic;
                }
            }

            public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
            {
                if (projectile.type == ProjectileID.TopazBolt && GetInstance<GameplayConfig>().OreStaffReworks)
                {
                    if (projectile.velocity.X != oldVelocity.X)
                    {
                        projectile.velocity.X = -oldVelocity.X;
                    }

                    if (projectile.velocity.Y != oldVelocity.Y)
                    {
                        projectile.velocity.Y = -oldVelocity.Y;
                    }
                    return false;
                }
                return base.OnTileCollide(projectile, oldVelocity);
            }

            public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
            {
                if (GetInstance<GameplayConfig>().OreStaffReworks) 
                {
                    switch (projectile.type)
                    {
                        case ProjectileID.AmethystBolt:
                            {
                                ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.NightsEdge,
                                        new ParticleOrchestraSettings { PositionInWorld = projectile.Center },
                                        projectile.owner);
                                break;
                            }
                        case ProjectileID.SapphireBolt:
                            {
                                if (hit.Crit)
                                {
                                    int explosion = Projectile.NewProjectile(projectile.GetSource_FromAI(), projectile.Center, Vector2.Zero, ProjectileType<SapphireBurst>(), projectile.damage * 2, projectile.knockBack / 2, projectile.owner, 80f);
                                    Main.projectile[explosion].DamageType = DamageClass.Magic;
                                    SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, projectile.Center);
                                    SoundEngine.PlaySound(SoundID.NPCDeath7, projectile.Center);
                                }
                                break;
                            }
                        case ProjectileID.RubyBolt:
                            {
                                if (hit.Crit)
                                {
                                    Item.NewItem(new EntitySource_DropAsItem(Projectile), target.Center, ItemType<HeartTiny>());
                                    //NewItemPerfect(target.Center, new Vector2(0, -2f), ItemType<HeartTiny>());
                                }
                                break;
                            }
                        case ProjectileID.DiamondBolt:
                            {
                                if (hit.Crit)
                                {
                                    projectile.damage -= 1;
                                    projectile.penetrate += 1;
                                    projectile.CritChance += 6;
                                }
                                break;
                            }
                        case ProjectileID.AmberBolt:
                            {
                                if (hit.Crit)
                                {
                                    target.AddBuff(BuffType<AmberStun>(), 120);
                                    SoundEngine.PlaySound(SoundID.Item150, projectile.Center);
                                    SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, projectile.Center);
                                    ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.ChlorophyteLeafCrystalShot,
                                            new ParticleOrchestraSettings { PositionInWorld = target.Center },
                                            target.whoAmI);
                                }
                                break;
                            }
                    }
                }
            }
        }
    }
    */
}
