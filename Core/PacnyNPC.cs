using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using static PacnyRefresh.Core.Helper;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;

namespace PacnyRefresh.Core
{
    public class PacnyNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public int damageModFlat = 0;
        public float critDamageModFlat = 0f;
        public int defenseModFlat = 0;
        public float defenseFactorMod = 1f;

        public float movementSpeed = 1f;

        public bool Slowed => movementSpeed < 1f;
        public bool stunned = false;

        public override void ResetEffects(NPC npc)
        {
            damageModFlat = 0;
            critDamageModFlat = 0f;
            defenseModFlat = 0;
            defenseFactorMod = 1f;

            movementSpeed = 1f;
            stunned = false;
        }
        
        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            modifiers.FlatBonusDamage += damageModFlat;
            modifiers.CritDamage += critDamageModFlat;
            modifiers.Defense.Flat += defenseModFlat;
            modifiers.DefenseEffectiveness *= defenseFactorMod;
        }

        public static bool CanBeStunned(NPC npc) => !npc.boss /* && npc.knockBackResist != 0f*/;

        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            if (stunned)
                return false;

            return true;
        }

        public override bool PreAI(NPC npc)
        {
            if (Main.netMode != NetmodeID.Server && stunned && !npc.boss/* && npc.knockBackResist != 0f*/)
            {
                npc.velocity = Vector2.Zero;
                return false;
            }
            return true;
        }

        public override void PostAI(NPC npc)
        {
            if (Slowed && !npc.boss/* && npc.knockBackResist != 0f*/)
            {
                npc.position -= npc.velocity * (1 - movementSpeed);
            }
        }
    }
}
