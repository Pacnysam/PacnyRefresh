using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using static Terraria.ModLoader.ModContent;
using static PacnyRefresh.Core.Helper;

namespace PacnyRefresh.Core.Mechanics
{
    public class SummonTagNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public int tagDamage = 0;
        public float tagDamageMult = 0f;

        public int tagCritChance = 0;

        //public float tagHealing = 0f;

        public override void ResetEffects(NPC npc)
        {
            tagDamage = 0;
            tagDamageMult = 0f;
            tagCritChance = 0;
            //tagHealing = 0f;
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (projectile.npcProj || projectile.trap || !projectile.IsMinionOrSentryRelated)
                return;
            var projTagMult = ProjectileID.Sets.SummonTagDamageMultiplier[projectile.type];

            if (tagDamage != 0)
                modifiers.FlatBonusDamage += tagDamage * projTagMult;
            if (tagDamageMult != 0)
                modifiers.ScalingBonusDamage += tagDamageMult * projTagMult;

            if (tagCritChance > 0 && Main.rand.Next(100) < tagCritChance)
            {
                modifiers.SetCrit();
            }
        }
    }
}
