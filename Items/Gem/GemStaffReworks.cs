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
using static PacnyRefresh.Core.Helper;
using static PacnyRefresh.Core.ColorHelper;
using Mono.Cecil;
using PacnyRefresh.Core;
using ReLogic.Content;
using PacnyRefresh.Effects.Dusts;

namespace PacnyRefresh.Items.Gem
{
    public class GemStaffSystem : ModSystem
    {
        public override void PostSetupContent()
        {
            foreach (int item in GemStaffReworkItem.gemStaves)
            {
                TextureAssets.Item[item] = Request<Texture2D>($"PacnyRefresh/Items/Gem/{ItemID.Search.GetName(item)}");
            }
        }

        public override void Unload()
        {
            foreach (int item in GemStaffReworkItem.gemStaves)
            {
                TextureAssets.Item[item] = Request<Texture2D>($"Terraria/Images/Item_{item}");
            }
        }

        public override void PostAddRecipes()
        {
            for (int i = 0; i < Recipe.numRecipes; i++)
            {
                Recipe recipe = Main.recipe[i];

                if (recipe.HasIngredient(ItemID.GoldBar) && recipe.HasIngredient(ItemID.Ruby) && recipe.HasTile(TileID.Anvils) && recipe.HasResult(ItemID.RubyStaff))
                {
                    recipe.RemoveIngredient(ItemID.Ruby);
                    recipe.AddIngredient(ItemID.FallenStar, 3);
                    recipe.AddIngredient(ItemID.Ruby, 8);
                }
                if (recipe.HasIngredient(ItemID.PlatinumBar) && recipe.HasIngredient(ItemID.Diamond) && recipe.HasTile(TileID.Anvils) && recipe.HasResult(ItemID.DiamondStaff))
                {
                    recipe.RemoveIngredient(ItemID.Diamond);
                    recipe.AddIngredient(ItemID.FallenStar, 3);
                    recipe.AddIngredient(ItemID.Diamond, 8);
                }
                if (recipe.HasIngredient(ItemID.FossilOre) && recipe.HasIngredient(ItemID.Amber) && recipe.HasTile(TileID.Anvils) && recipe.HasResult(ItemID.AmberStaff))
                {
                    recipe.DisableRecipe();
                }
            }
            Recipe amberStaffCorro = Recipe.Create(ItemID.AmberStaff, 1)
                .AddIngredient(ItemID.FossilOre, 15)
                .AddIngredient(ItemID.Obsidian, 10)
                .AddIngredient(ItemID.ShadowScale, 5)
                .AddIngredient(ItemID.Amber, 8)
                .AddTile(TileID.Anvils)
                .Register();

            Recipe amberStaffCrim = Recipe.Create(ItemID.AmberStaff, 1)
            .AddIngredient(ItemID.FossilOre, 15)
            .AddIngredient(ItemID.Obsidian, 10)
            .AddIngredient(ItemID.TissueSample, 5)
            .AddIngredient(ItemID.Amber, 8)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }

    public class GemStaffPlayer : ModPlayer
    {
        public int sapphireHits;

        public override void PostUpdateMiscEffects()
        {
            if (sapphireHits > 3)
                sapphireHits = 0;
        }
    }

    public class GemStaffReworkItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public static readonly int[] gemStaves = [ItemID.AmethystStaff, ItemID.TopazStaff, ItemID.SapphireStaff, ItemID.EmeraldStaff, ItemID.RubyStaff, ItemID.DiamondStaff, ItemID.AmberStaff];

        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return gemStaves.Contains(entity.type);
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (gemStaves.Contains(item.type)) 
            {
                string[] text =
                [
                    Language.GetTextValue("Mods.PacnyRefresh.VanillaItemTooltips.GemStaves.Type" + (Array.IndexOf(gemStaves, item.type) + 1))
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

        public override void SetStaticDefaults()
        {
            foreach (int item in gemStaves)
            {
                ItemSets.Glowmask[item] = (Request<Texture2D>($"PacnyRefresh/Items/Gem/{ItemID.Search.GetName(item)}Glow", AssetRequestMode.ImmediateLoad), Color.White with { A = 160 }, true);
            }
        }

        public override void SetDefaults(Item item)
        {
            switch (item.type)
            {
                case ItemID.AmethystStaff:
                    {
                        item.width = 30;
                        item.height = 30;

                        item.damage = 15;
                        item.ArmorPenetration = 10;
                        item.crit = 4;

                        item.mana = 6;

                        item.UseSound = SoundID.DD2_EtherianPortalSpawnEnemy with { Pitch = 0.8f, PitchVariance = 0.3f };
                        break;
                    }
                case ItemID.TopazStaff:
                    {
                        item.width = 32;
                        item.height = 32;

                        item.damage = 14;

                        item.GetGlobalItem<PacnyItem>().critDamageMod = 0.5f;

                        item.mana = 7;

                        item.UseSound = SoundID.DD2_EtherianPortalSpawnEnemy with { Pitch = 0.8f, PitchVariance = 0.3f };
                        break;
                    }
                case ItemID.SapphireStaff:
                    {
                        item.width = 34;
                        item.height = 34;

                        item.damage = 16;
                        item.crit = 6;
                        item.shootSpeed = 28f;

                        item.mana = 8;

                        item.UseSound = SoundID.DD2_EtherianPortalSpawnEnemy with { Pitch = 0.8f, PitchVariance = 0.3f };
                        break;
                    }
                case ItemID.EmeraldStaff:
                    {
                        item.width = 38;
                        item.height = 34;

                        item.damage = 17;
                        item.shootSpeed = 14.5f;
                        item.useTime = 42;
                        item.useAnimation = 42;

                        item.GetGlobalItem<PacnyItem>().critDamageMod = 0.25f;

                        item.mana = 8;

                        item.UseSound = SoundID.DD2_EtherianPortalSpawnEnemy with { Pitch = 0.8f, PitchVariance = 0.3f };
                        break;
                    }
                case ItemID.RubyStaff:
                    {
                        item.width = 34;
                        item.height = 34;

                        item.reuseDelay = 15;

                        item.damage = 18;
                        item.shootSpeed = 1.5f;

                        item.rare = ItemRarityID.Green;

                        item.mana = 9;

                        item.UseSound = SoundID.Item102;
                        break;
                    }
                case ItemID.DiamondStaff:
                    {
                        item.width = 36;
                        item.height = 36;

                        item.damage = 7;
                        item.useTime = 10;
                        item.useAnimation = 20;

                        item.UseSound = SoundID.DD2_BookStaffCast with { Pitch = 0.2f };
                        break;
                    }
                case ItemID.AmberStaff:
                    {
                        item.width = 34;
                        item.height = 34;

                        item.useTime = 5;
                        item.useAnimation = 15;
                        item.reuseDelay = 45;

                        item.damage = 19;
                        item.GetGlobalItem<PacnyItem>().critDamageMod = -0.5f;

                        item.UseSound = SoundID.DD2_EtherianPortalSpawnEnemy with { Volume = 1.2f, Pitch = 0.6f, PitchVariance = 0.4f };

                        item.mana = 12;

                        item.rare = ItemRarityID.Orange;
                        break;
                    }
            }
        }

        public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            float offset = -1f;
            switch (type)
            {
                case ProjectileID.AmethystBolt:
                    {
                        offset = 0f;
                        break;
                    }
                case ProjectileID.TopazBolt:
                    {
                        offset = -3f;
                        break;
                    }
                case ProjectileID.SapphireBolt:
                    {
                        offset = -3.5f;
                        break;
                    }
                case ProjectileID.EmeraldBolt:
                    {
                        offset = -8f;
                        break;
                    }
                case ProjectileID.RubyBolt:
                    {
                        offset = -2f;
                        break;
                    }
                case ProjectileID.DiamondBolt:
                    {
                        offset = -7.5f;
                        break;
                    }
                case ProjectileID.AmberBolt:
                    {
                        offset = -1.5f;
                        break;
                    }
            }

            Vector2 muzzleOffset = Vector2.Normalize(velocity) * (item.Size.Length() + offset);

            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
        }

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int dustType = 0;

            switch (item.type)
            {
                case ItemID.AmethystStaff:
                    {
                        dustType = DustID.GemAmethyst;
                        break;
                    }
                case ItemID.TopazStaff:
                    {
                        dustType = DustID.GemTopaz;
                        break;
                    }
                case ItemID.SapphireStaff:
                    {
                        dustType = DustID.GemSapphire;
                        break;
                    }
                case ItemID.EmeraldStaff:
                    {
                        dustType = DustID.GemEmerald;
                        break;
                    }
                case ItemID.RubyStaff:
                    {
                        dustType = DustID.GemRuby;
                        break;
                    }
                case ItemID.DiamondStaff:
                    {
                        dustType = DustID.GemDiamond;
                        break;
                    }
                case ItemID.AmberStaff:
                    {
                        dustType = DustID.AmberBolt;
                        break;
                    }
            }

            for (float k = 0; k < Math.PI * 2; k += Main.rand.NextFloat(0.1f, 0.24f))
            {
                Dust dust = Dust.NewDustPerfect(position + new Vector2(0, Main.rand.NextFloat(10, 15)).RotatedBy(k), dustType, Vector2.One.RotatedBy(k) * 3f, 0, Color.White, Main.rand.NextFloat(0.75f, 1.05f));
                dust.velocity *= -0.5f;
                dust.noGravity = true;
            }

            if (item.type == ItemID.TopazStaff)
            {
                int p = Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(Main.rand.Next(-9, 9))) * 0.85f, type, (int)(damage * 0.7f), knockback, player.whoAmI);
                Main.projectile[p].scale *= 0.6f;
            }
            if (item.type == ItemID.EmeraldStaff)
            {
                int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                Main.projectile[p].velocity.Y -= 3.8f;
                SoundEngine.PlaySound(SoundID.Item110, position);
                return false;
            }

            if (item.type == ItemID.DiamondStaff)
            {
                int p = Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians((float)Math.Sin(PacnySystem.rottime * 8))) * 4f, type, (int)(damage * 0.7f), knockback, player.whoAmI);
            }
            else if (item.type == ItemID.AmberStaff)
            {
                SoundEngine.PlaySound(SoundID.DD2_SkyDragonsFuryShot with { Volume = 0.75f, Pitch = 0.2f, PitchVariance = 0.4f, MaxInstances = 3 }, position);
                Projectile p = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, Main.rand.NextFloat(-3, 3));
                p.scale = Main.rand.NextFloat(0.7f, 1.3f);
                return false;
            }
            else 
            {
                SoundEngine.PlaySound(SoundID.Item110, position);
            }
            return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }

        public class GemStaffReworkProjectile : GlobalProjectile
        {
            public override bool InstancePerEntity => true;
            public static readonly int[] gemBolts = [ProjectileID.AmethystBolt, ProjectileID.TopazBolt, ProjectileID.SapphireBolt, ProjectileID.EmeraldBolt, ProjectileID.RubyBolt, ProjectileID.DiamondBolt, ProjectileID.AmberBolt];

            public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
            {
                return gemBolts.Contains(entity.type);
            }

            public override void SetDefaults(Projectile entity)
            {
                switch (entity.type)
                {
                    case ProjectileID.AmethystBolt:
                        {
                            entity.netImportant = true;
                            entity.timeLeft = 300;
                            break;
                        }
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
                    case ProjectileID.RubyBolt:
                        {
                            entity.penetrate = 1;
                            break;
                        }
                    case ProjectileID.DiamondBolt:
                        {
                            entity.extraUpdates = 20;
                            entity.penetrate = 2;
                            entity.usesLocalNPCImmunity = true;
                            entity.localNPCHitCooldown = 10;
                            break;
                        }
                    case ProjectileID.AmberBolt:
                        {
                            entity.penetrate = 2;
                            entity.usesLocalNPCImmunity = true;
                            entity.localNPCHitCooldown = 10;
                            break;
                        }
                }
            }

            public override bool PreAI(Projectile projectile)
            {
                if (projectile.type == ProjectileID.SapphireBolt)
                {
                    projectile.velocity *= 0.95f;
                    if (projectile.velocity.Length() <= 0.1f)
                        projectile.Kill();
                }
                if (projectile.type == ProjectileID.EmeraldBolt)
                {
                    projectile.velocity.Y += 0.35f;
                }
                if (projectile.type == ProjectileID.RubyBolt)
                {
                    projectile.velocity *= 1.05f;
                    if (projectile.velocity.Length() > 15f)
                    {
                        projectile.velocity = Vector2.Normalize(projectile.velocity) * 15f;
                        projectile.extraUpdates = 1;
                    }
                }
                if (projectile.type == ProjectileID.AmberBolt)
                {
                    const int homingRange = 150;

                    float targetDistance = 8000f;
                    int target = -1;
                    for (int i = 0; i < 100; i++)
                    {
                        float range = Vector2.Distance(projectile.Center, Main.npc[i].Center);
                        if (range < targetDistance && range < homingRange && Main.npc[i].CanBeChasedBy(projectile, false))
                        {
                            target = i;
                            targetDistance = range;
                        }
                    }
                    if (target != -1 && Collision.CanHit(projectile.position, projectile.width, projectile.height, Main.npc[target].position, Main.npc[target].width, Main.npc[target].height))
                    {
                        projectile.velocity += Vector2.Normalize(Main.npc[target].Center - projectile.Center) * 3f;

                        if (projectile.velocity.Length() > 12.5f)
                            projectile.velocity = Vector2.Normalize(projectile.velocity) * 12.5f;
                    }

                    if (targetDistance > homingRange)
                    {
                        projectile.velocity.Y += (float)Math.Sin((PacnySystem.rottime + projectile.ai[0]) * 8) * Math.Clamp(projectile.GetGlobalProjectile<PacnyProjectile>().counter * 0.1f, 0f, 1.5f);
                    }
                    else
                        projectile.velocity.Y += (float)Math.Sin((PacnySystem.rottime + projectile.ai[0]) * 6) * 0.5f;
                }
                return base.PreAI(projectile);
            }
            public override void AI(Projectile projectile)
            {
                if (projectile.type == ProjectileID.RubyBolt)
                {
                    if (projectile.GetGlobalProjectile<PacnyProjectile>().counter % 8 == 0)
                    {
                        for (float k = 0; k < Math.PI * 2; k += (float)Math.PI / 30)
                        { 
                            Dust dust = Dust.NewDustPerfect(projectile.Center, DustID.GemRuby, Vector2.One.RotatedBy(k) * 0.9f, 0, Color.White, 1f); 
                            dust.noGravity = true;
                        }
                    }
                }
            }

            public override void OnKill(Projectile projectile, int timeLeft)
            {
                if (projectile.type == ProjectileID.RubyBolt)
                {
                    SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath, projectile.Center);

                    int i = Main.rand.Next(6, 8);
                    for (float k = 0; k < i; k++)
                    {
                        Projectile p = Projectile.NewProjectileDirect(projectile.GetSource_FromAI(), projectile.Center, new Vector2(Main.rand.NextFloat(-6, 6), Main.rand.NextFloat(-10, -1.5f)), ProjectileType<BasicRubyBolt>(), projectile.damage / 2, 0, projectile.owner, 0f, 0f);
                        p.GetGlobalProjectile<PacnyProjectile>().gravity = Main.rand.NextFloat(0.1f, 0.3f);
                        p.GetGlobalProjectile<PacnyProjectile>().spawnTime = 10;
                    }
                }
                else if (projectile.type != ProjectileID.DiamondBolt)
                {
                    SoundEngine.PlaySound(SoundID.Item118, projectile.Center);
                }
                if (projectile.type == ProjectileID.SapphireBolt)
                {
                    for (float i = 0; i < 12; ++i)
                    {
                        Dust dust = Dust.NewDustPerfect(projectile.Center, DustID.GemSapphire, new Vector2(0f, -3f).RotatedByRandom(MathHelper.Pi).RotatedBy(MathHelper.TwoPi * i / 8) * Vector2.One * (0.6f + Main.rand.NextFloat() * 0.35f), 0, Color.White, 1.5f);
                        dust.noGravity = true;
                        dust.fadeIn = Main.rand.NextFloat() * 2f;
                        var dust2 = Dust.CloneDust(dust);
                        dust2.scale *= 0.5f;
                        dust2.fadeIn *= 0.5f;
                    }
                }
                if (projectile.type == ProjectileID.AmberBolt)
                {
                    for (float i = 0; i < 12; ++i)
                    {
                        Dust dust = Dust.NewDustPerfect(projectile.Center, DustID.AmberBolt, new Vector2(0f, -3f).RotatedByRandom(MathHelper.Pi).RotatedBy(MathHelper.TwoPi * i / 8) * Vector2.One * (0.6f + Main.rand.NextFloat() * 0.35f), 0, Color.White, 1.5f);
                        dust.noGravity = true;
                        dust.fadeIn = Main.rand.NextFloat() * 2f;
                        var dust2 = Dust.CloneDust(dust);
                        dust2.scale *= 0.5f;
                        dust2.fadeIn *= 0.5f;
                    }
                }
            }

            public override void PostAI(Projectile projectile)
            {
                if (projectile.type == ProjectileID.AmethystBolt) 
                {
                    projectile.velocity = projectile.velocity.Length() * Vector2.Lerp(projectile.velocity, projectile.DirectionTo(Main.MouseWorld) * projectile.velocity.Length() * 0.5f, 0.2f).SafeNormalize(Vector2.Normalize(projectile.velocity));
                    projectile.netUpdate = true;
                }
                if (projectile.type == ProjectileID.EmeraldBolt && projectile.timeLeft % 5 == 0 && projectile.timeLeft > 90)
                {
                    int emerald = Projectile.NewProjectile(projectile.GetSource_FromAI(), new Vector2(projectile.Bottom.X, projectile.Bottom.Y + Main.rand.NextFloat(-4, 4)), Vector2.Zero, ProjectileType<FallingEmerald>(), (int)(projectile.damage * 0.55f), projectile.knockBack * 0.3f, projectile.owner);
                    Main.projectile[emerald].DamageType = DamageClass.Magic;
                }
            }

            public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
            {
                if (projectile.type == ProjectileID.TopazBolt)
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
                            if (Main.player[projectile.owner].GetModPlayer<GemStaffPlayer>().sapphireHits >= 3)
                            {
                                int explosion = Projectile.NewProjectile(projectile.GetSource_FromAI(), projectile.Center, Vector2.Zero, ProjectileType<SapphireBurst>(), projectile.damage * 2, projectile.knockBack / 2, projectile.owner, 80f);
                                Main.projectile[explosion].DamageType = DamageClass.Magic;
                                SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, projectile.Center);
                                SoundEngine.PlaySound(SoundID.NPCDeath7, projectile.Center);
                            }
                            Main.player[projectile.owner].GetModPlayer<GemStaffPlayer>().sapphireHits++;
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
                            projectile.damage /= 2;
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
    public class SapphireBurst : ModProjectile
    {
        public override string Texture => "PacnyRefresh/Textures/Empty";

        public float TimeFade => 1 - Projectile.timeLeft / 20f;
        public float Radius => BezierEase((20 - Projectile.timeLeft) / 20f) * Projectile.ai[0];
        int counter;

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 24;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            //Projectile.extraUpdates = 1;

            //Projectile.GetGlobalProjectile<GoldLeafProjectile>().throwingDamageType = DamageClass.Melee;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void OnSpawn(IEntitySource source)
        {
            ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.Excalibur,
                new ParticleOrchestraSettings { PositionInWorld = Projectile.Center },
                Projectile.owner);
        }

        public override bool PreAI()
        {
            counter++;
            return true;
        }

        float rot = 0;
        public override void AI()
        {
            Lighting.AddLight((int)(Projectile.position.X / 16), (int)(Projectile.position.Y / 16), GemColor(3).R / 255, GemColor(3).G / 255, GemColor(3).B / 255);

            if (counter <= 10 /*&& counter % 2 == 0*/)
            {
                for (int k = 0; k < 2; k++)
                {
                    rot += (float)Math.PI / 20;
                    if (rot >= Math.PI * 2) rot = 0;

                    float x = (float)Math.Cos(PacnySystem.rottime + rot + k * 3) * 1.6f;
                    float y = (float)Math.Sin(PacnySystem.rottime + rot + k * 3) * 1.6f;
                    Vector2 pos = new Vector2(x, y).RotatedBy(k * 2 * 6.28f);

                    ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.StardustPunch,
                        new ParticleOrchestraSettings { PositionInWorld = Projectile.Center, MovementVector = pos * (4.8f - counter * 0.2f) },
                        Projectile.owner);

                    //Dust d = Dust.NewDustPerfect(Projectile.Center, DustType<LightDust>(), pos * (3.2f - (counter * 0.1f)), 0, GemColor(3), 0.5f);
                }
            }

            if (counter > 12)
            {
                Projectile.damage = 0;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CheckCircularCollision(Projectile.Center, (int)Radius + 40, targetHitbox);
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D tex = Request<Texture2D>("PacnyRefresh/Textures/Flares/wavering").Value;
            Color color = GemColor(3) * (1.2f - counter * 0.05f);
            color.A = 0;

            Main.spriteBatch.Draw
            (
                tex,
                new Vector2
                (
                    Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f,
                    Projectile.position.Y - Main.screenPosition.Y + Projectile.height * 0.5f
                ),
                new Rectangle(0, 0, tex.Width, tex.Height),
                color,
                0f,
                tex.Size() * 0.5f,
                Projectile.scale * 0.16f + Radius * 0.009f,
                SpriteEffects.None,
                0f
            );
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.velocity += Vector2.Normalize(target.Center - Projectile.Center) * 4.6f * target.knockBackResist;

            target.immune[Projectile.owner] = 8;
        }
    }
    
    public class FallingEmerald : ModProjectile
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.Emerald;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 16;

            Projectile.friendly = true;
            Projectile.extraUpdates = 1;

            Projectile.DamageType = DamageClass.Melee;

            Projectile.GetGlobalProjectile<PacnyProjectile>().gravity = 0.08f;
            Projectile.GetGlobalProjectile<PacnyProjectile>().gravityDelay = 20;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox.Inflate(12, 0);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, Projectile.Center);

            for (float k = 0; k < 6.28f; k += 0.52f)
                Dust.NewDustPerfect(Projectile.Center, DustType<LightDust>(), Vector2.One.RotatedBy(k) * 0.45f, 0, GemColor(4), 0.35f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(tex.Width * 0.5f, Projectile.height * 0.5f);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin/* + new Vector2(0f, Projectile.gfxOffY)*/;
                Main.spriteBatch.Draw(tex, drawPos, null, ColorHelper.AdditiveWhite * (1.0f - 0.15f * k), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }
    }

    public class BasicRubyBolt : ModProjectile
    {
        public override string Texture => "PacnyRefresh/Textures/Empty";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.RubyBolt);
            //AIType = ProjectileID.RubyBolt;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.velocity *= 0.5f;
        }

        public override void AI()
        {
            //Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemRuby, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 0, GemColor(ItemID.Ruby), 0.6f);
            Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.GemRuby, Projectile.velocity * 0.6f, 0, GemColor(ItemID.Ruby), 1f);
            d.noGravity = true;
        }
    }

    public class AmberStun : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = false;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<PacnyNPC>().stunned = true;

            if (!npc.boss)
            {
                npc.velocity *= 0;
                npc.frame.Y = 0;
            }

            if (Main.rand.NextBool(20))
            {
                ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.AshTreeShake,
                    new ParticleOrchestraSettings { PositionInWorld = npc.Center },
                    npc.whoAmI);
            }
        }
    }
}
