using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;


namespace PacnyRefresh.Core.Bases.Projectiles
{
    public abstract class BaseFlailItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Melee;

            Item.channel = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = true;
            Item.noMelee = true;
                    
            Item.UseSound = SoundID.Item19;

            SafeSetDefaults();
        }

        public virtual void SafeSetDefaults() { }

        public override bool CanUseItem(Player Player)
        {
            return Player.ownedProjectileCounts[Item.shoot] == 0;
        }
    }
    
    public abstract class BaseFlailProjectile(float throwRange = 180, float fallSpeed = 1.2f, float throwSpeed = 18f, float swingSpeed = 10f, bool doRotation = true, float swingDistance = 30/*, float killDistance = 800 */) : ModProjectile 
    {
        public float throwRange = throwRange;
        public float fallSpeed = fallSpeed;
        public float throwSpeed = throwSpeed;
        public float swingSpeed = swingSpeed;
        public bool doRotation = doRotation;
        public float swingDistance = swingDistance;
        //public float killDistance = killDistance;

        public float State;
        public float DecelerationTimer;

        //public ref float State => ref Projectile.ai[0];
        //public ref float DecelerationTimer => ref Projectile.ai[1];

        public enum FlailStates : int
        {
            Swinging = 0,
            Throwing = 1,
            Returning = 2,
            LegacyThrown = 3,
            ReturningFinal = 4,
            StruckTileCanDrop = 5,
            Dropping = 6
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            
            Projectile.penetrate = -1;

            Projectile.netImportant = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(State);
            writer.Write(DecelerationTimer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            State = reader.Read();
            DecelerationTimer = reader.Read();
        }

        public virtual void ThrowEffect(Player player) { }
        public virtual void DropSlamEffect(Player player) { }
        public virtual void TileStrikeEffect(Player player) { }
        public virtual void SpinEffect(Player player) { }
        public virtual void ApexEffect(Player player) { }
        public virtual void RealAI() { }

        float MeleeSpeed => Main.player[Projectile.owner].GetAttackSpeed(DamageClass.Melee);

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (!player.active || player.dead || player.noItems || player.CCed || Vector2.Distance(Projectile.Center, player.Center) > 800f)
            {
                Projectile.Kill();
                return;
            }
            if (Main.myPlayer == Projectile.owner && Main.mapFullscreen)
            {
                Projectile.Kill();
                return;
            }

            Vector2 mountedCenter = player.MountedCenter;
            bool flag2 = false;
            int decelerationTime = 10;
            float throwSpeedFinal = throwSpeed * MeleeSpeed;
            float maxReturnSpeed = 3f * MeleeSpeed;
            float returningKillRange = 16f * MeleeSpeed;
            float maxReturnSpeedStruckTile = 6f * MeleeSpeed;
            float returnSpeedTileStruck = 48f * MeleeSpeed;

            float legacyReturnSpeed = 1f * MeleeSpeed; 
            float legacySpeed = 140f * MeleeSpeed; 
            int legacyMinDistance = 60; 

            //int iFrames = 10;
            //float num16 = throwSpeedFinal * (float)decelerationTime;
            //float num16 = throwRange;
            float maxRangeDropped = throwRange + 150f;
            Projectile.localNPCHitCooldown = 10;
            switch ((int)State)
            {
                case (int)FlailStates.Swinging:
                    {
                        flag2 = true;
                        if (Projectile.owner == Main.myPlayer)
                        {
                            Vector2 throwDirection = mountedCenter.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.UnitX * player.direction);
                            player.ChangeDir((throwDirection.X > 0f) ? 1 : (-1));
                            if (!player.channel)
                            {
                                State = (int)FlailStates.Throwing;
                                DecelerationTimer = 0f;
                                Projectile.velocity = throwDirection * throwSpeedFinal + player.velocity;
                                Projectile.Center = mountedCenter;
                                Projectile.netUpdate = true;
                                Projectile.ResetLocalNPCHitImmunity();
                                Projectile.localNPCHitCooldown = 10;

                                ThrowEffect(player);
                                break;
                            }
                        }
                        Projectile.localAI[1] += 1f;
                        Vector2 vector4 = new Vector2(player.direction).RotatedBy((float)Math.PI * swingSpeed * (Projectile.localAI[1] / 60f) * (float)player.direction);
                        vector4.Y *= 0.8f;
                        if (vector4.Y * player.gravDir > 0f)
                        {
                            vector4.Y *= 0.5f;
                        }
                        Projectile.Center = mountedCenter + vector4 * swingDistance;
                        Projectile.velocity = Vector2.Zero;
                        Projectile.localNPCHitCooldown = 15;
                        SpinEffect(player);
                        break;
                    }
                case (int)FlailStates.Throwing:
                    {
                        bool shouldReturn = DecelerationTimer++ >= throwRange;
                        shouldReturn |= Projectile.Distance(mountedCenter) >= throwRange;
                        if (player.controlUseItem)
                        {
                            State = (int)FlailStates.Dropping;
                            DecelerationTimer = 0f;
                            Projectile.netUpdate = true;
                            Projectile.velocity *= 0.2f;

                            ApexEffect(player);
                            break;
                        }
                        if (shouldReturn)
                        {
                            State = (int)FlailStates.Returning;
                            DecelerationTimer = 0f;
                            Projectile.netUpdate = true;
                            Projectile.velocity *= 0.3f;
                            
                            ApexEffect(player);
                        }
                        player.ChangeDir((player.Center.X < Projectile.Center.X) ? 1 : (-1));
                        Projectile.localNPCHitCooldown = 10;
                        break;
                    }
                case (int)FlailStates.Returning:
                    {
                        Vector2 vector = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);
                        if (Projectile.Distance(mountedCenter) <= returningKillRange)
                        {
                            Projectile.Kill();
                            return;
                        }
                        if (player.controlUseItem)
                        {
                            State = (int)FlailStates.Dropping;
                            DecelerationTimer = 0f;
                            Projectile.netUpdate = true;
                            Projectile.velocity *= 0.2f;
                        }
                        else
                        {
                            Projectile.velocity *= 0.98f;
                            Projectile.velocity = Projectile.velocity.MoveTowards(vector * returningKillRange, maxReturnSpeed);
                            player.ChangeDir((player.Center.X < Projectile.Center.X) ? 1 : (-1));
                        }
                        break;
                    }
                case (int)FlailStates.LegacyThrown:
                    {
                        if (!player.controlUseItem)
                        {
                            State = (int)FlailStates.ReturningFinal;
                            DecelerationTimer = 0f;
                            Projectile.netUpdate = true;
                            break;
                        }
                        float distanceToPlayer = Projectile.Distance(mountedCenter);
                        Projectile.tileCollide = DecelerationTimer == 1f;
                        bool flag4 = distanceToPlayer <= throwRange;
                        if (flag4 != Projectile.tileCollide)
                        {
                            Projectile.tileCollide = flag4;
                            DecelerationTimer = (Projectile.tileCollide ? 1 : 0);
                            Projectile.netUpdate = true;
                        }
                        if (distanceToPlayer > (float)legacyMinDistance)
                        {
                            if (distanceToPlayer >= throwRange)
                            {
                                Projectile.velocity *= 0.5f;
                                Projectile.velocity = Projectile.velocity.MoveTowards(Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero) * legacySpeed, legacySpeed);
                            }
                            Projectile.velocity *= 0.98f;
                            Projectile.velocity = Projectile.velocity.MoveTowards(Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero) * legacySpeed, legacyReturnSpeed);
                        }
                        else
                        {
                            if (Projectile.velocity.Length() < 8f)
                            {
                                Projectile.velocity.X *= 0.98f;
                                Projectile.velocity.Y += fallSpeed/2;
                            }
                            if (player.velocity.X == 0f)
                            {
                                Projectile.velocity.X *= 0.98f;
                            }
                        }
                        player.ChangeDir((player.Center.X < Projectile.Center.X) ? 1 : (-1));
                        break;
                    }
                case (int)FlailStates.ReturningFinal:
                    {
                        Projectile.tileCollide = false;
                        Vector2 vector2 = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);
                        if (Projectile.Distance(mountedCenter) <= returnSpeedTileStruck)
                        {
                            Projectile.Kill();
                            return;
                        }
                        Projectile.velocity *= 0.98f;
                        Projectile.velocity = Projectile.velocity.MoveTowards(vector2 * returnSpeedTileStruck, maxReturnSpeedStruckTile);
                        Vector2 target = Projectile.Center + Projectile.velocity;
                        Vector2 value = mountedCenter.DirectionFrom(target).SafeNormalize(Vector2.Zero);
                        if (Vector2.Dot(vector2, value) < 0f)
                        {
                            Projectile.Kill();
                            return;
                        }
                        player.ChangeDir((player.Center.X < Projectile.Center.X) ? 1 : (-1));
                        break;
                    }
                case (int)FlailStates.StruckTileCanDrop:
                    if (DecelerationTimer++ >= (decelerationTime + 5))
                    {
                        State = (int)FlailStates.Dropping;
                        DecelerationTimer = 0f;
                        Projectile.netUpdate = true;
                    }
                    else
                    {
                        Projectile.localNPCHitCooldown = 10;
                        Projectile.velocity.Y += fallSpeed;
                        Projectile.velocity.X *= 0.96f;
                        player.ChangeDir((player.Center.X < Projectile.Center.X) ? 1 : (-1));
                    }
                    break;
                case (int)FlailStates.Dropping:
                    if (!player.controlUseItem || Projectile.Distance(mountedCenter) > maxRangeDropped)
                    {
                        State = (int)FlailStates.ReturningFinal;
                        DecelerationTimer = 0f;
                        Projectile.netUpdate = true;
                        break;
                    }
                    if (!Projectile.shimmerWet)
                    {
                        Projectile.velocity.Y += fallSpeed;
                    }
                    Projectile.velocity.X *= 0.95f;
                    player.ChangeDir((player.Center.X < Projectile.Center.X) ? 1 : (-1));
                    break;
            }
            Projectile.direction = ((Projectile.velocity.X > 0f) ? 1 : (-1));
            Projectile.spriteDirection = Projectile.direction;
            Projectile.ownerHitCheck = flag2;
            if (doRotation)
            {
                if (Projectile.velocity.Length() > 1f)
                {
                    Projectile.rotation = Projectile.velocity.ToRotation() + Projectile.velocity.X * 0.1f;
                }
                else
                {
                    Projectile.rotation += Projectile.velocity.X * 0.1f;
                }
            }
            Projectile.timeLeft = 2;
            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2);
            player.itemRotation = Projectile.DirectionFrom(mountedCenter).ToRotation();
            if (Projectile.Center.X < mountedCenter.X)
            {
                player.itemRotation += (float)Math.PI;
            }
            player.itemRotation = MathHelper.WrapAngle(player.itemRotation);
            RealAI();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (State == (int)FlailStates.Swinging)
            {
                Vector2 mountedCenter = Main.player[Projectile.owner].MountedCenter;
                Vector2 vector = targetHitbox.ClosestPointInRect(mountedCenter) - mountedCenter;
                vector.Y /= 0.8f;
                float num = swingDistance + projHitbox.Width;
                return vector.Length() <= num;
            }
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            int iFrames = 10;
            int num2 = 0;
            Vector2 vector = Projectile.velocity;
            float num3 = 0.2f;
            if (State == (int)FlailStates.Throwing || State == (int)FlailStates.StruckTileCanDrop)
            {
                num3 = 0.4f;
            }
            if (State == (int)FlailStates.Dropping)
            {
                num3 = 0f;
            }
            if (oldVelocity.X != Projectile.velocity.X)
            {
                if (Math.Abs(oldVelocity.X) > 4f)
                {
                    num2 = 1;
                }
                Projectile.velocity.X = (0f - oldVelocity.X) * num3;
                Projectile.localAI[0] += 1f;
            }
            if (oldVelocity.Y != Projectile.velocity.Y)
            {
                if (Math.Abs(oldVelocity.Y) > 4f)
                {
                    num2 = 1;
                }
                Projectile.velocity.Y = (0f - oldVelocity.Y) * num3;
                Projectile.localAI[0] += 1f;
            }
            if (State == (int)FlailStates.Throwing)
            {
                State = (int)FlailStates.StruckTileCanDrop;
                Projectile.localNPCHitCooldown = iFrames;
                Projectile.netUpdate = true;
                Point scanAreaStart = Projectile.TopLeft.ToTileCoordinates();
                Point scanAreaEnd = Projectile.BottomRight.ToTileCoordinates();
                num2 = 2;
                Projectile.CreateImpactExplosion(2, Projectile.Center, ref scanAreaStart, ref scanAreaEnd, Projectile.width, out var causedShockwaves);
                Projectile.CreateImpactExplosion2_FlailTileCollision(Projectile.Center, causedShockwaves, vector);
                Projectile.position -= vector;
                TileStrikeEffect(Main.player[Projectile.owner]);
            }
            if (num2 > 0)
            {
                Projectile.netUpdate = true;
                for (int i = 0; i < num2; i++)
                {
                    Collision.HitTiles(Projectile.position, vector, Projectile.width, Projectile.height);
                }
                if (State == (int)FlailStates.Dropping)
                {
                    DropSlamEffect(Main.player[Projectile.owner]);
                }

                
            }
            if (State != (int)FlailStates.LegacyThrown && State != (int)FlailStates.Swinging && State != (int)FlailStates.StruckTileCanDrop && State != (int)FlailStates.Dropping && Projectile.localAI[0] >= 10f)
            {
                State = (int)FlailStates.ReturningFinal;
                Projectile.netUpdate = true;
            }
            //if (Projectile.wet)
            //{
            //  wetVelocity = Projectile.velocity;
            //}

            return false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (State == (int)FlailStates.Swinging)
                modifiers.FinalDamage *= 1.2f;
            if (State == (int)FlailStates.Throwing || State == (int)FlailStates.Returning)
                modifiers.FinalDamage *= 2f;

            if (State == (int)FlailStates.Swinging)
            {
                modifiers.Knockback *= 0.35f;
            }
            if (State == 6f)
            {
                modifiers.Knockback *= 0.5f;
            }
            modifiers.HitDirectionOverride = Math.Sign(target.Center.X - Main.player[Projectile.owner].Center.X);
        }

        public override bool PreDrawExtras()
        {
            Texture2D ChainTexture = Request<Texture2D>(Texture + "Chain").Value;

            Vector2 armPos = Main.GetPlayerArmPosition(Projectile);
            armPos.Y -= Main.player[Projectile.owner].gfxOffY;

            int numDraws = Math.Max((int)(Projectile.Distance(armPos) / ChainTexture.Height), 1);
            for (int i = 0; i < numDraws; i++)
            {
                Vector2 chaindrawpos = Vector2.Lerp(armPos, Projectile.Center, i / (float)numDraws);
                Color lightColor = Lighting.GetColor((int)chaindrawpos.X / 16, (int)chaindrawpos.Y / 16);

                Main.EntitySpriteDraw(ChainTexture, chaindrawpos - Main.screenPosition, null, lightColor, Projectile.AngleFrom(Main.player[Projectile.owner].MountedCenter) + 1.57f, ChainTexture.Size() / 2, 1f, SpriteEffects.None, 0);
            }
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle frame = new(0, texture.Height / Main.projFrames[Type] * Projectile.frame, texture.Width, (texture.Height / Main.projFrames[Type]) - 2);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
