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

namespace PacnyRefresh.Core
{
    public class PacnyPlayer : ModPlayer
    {
        public float itemSpeed;

        public float meleeCritDamageMult = 1f;
        public float rangedCritDamageMult = 1f;
        public float magicCritDamageMult = 1f;
        public float summonCritDamageMult = 1f;
        public float critDamageMult = 1f;

        public int summonCritChance = 0;

        public override float UseTimeMultiplier(Item Item) => itemSpeed;
        public override float UseAnimationMultiplier(Item item) => itemSpeed;

        public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (item.DamageType.CountsAsClass(DamageClass.Summon) && ((summonCritChance > 0 && Main.rand.NextBool(summonCritChance, 100)) || summonCritChance > 100))
            {
                modifiers.SetCrit();
            }

            if (item.DamageType == DamageClass.Melee) critDamageMult *= meleeCritDamageMult;
            if (item.DamageType == DamageClass.Ranged) critDamageMult *= rangedCritDamageMult;
            if (item.DamageType == DamageClass.Magic) critDamageMult *= magicCritDamageMult;
            if (item.DamageType == DamageClass.Summon) critDamageMult *= summonCritDamageMult;

            modifiers.CritDamage += (item.GetGlobalItem<PacnyItem>().critDamageMod);
            modifiers.CritDamage *= critDamageMult;
        }
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            int totalSummonCritChance = summonCritChance + proj.GetGlobalProjectile<PacnyProjectile>().summonCritChance;
            if (proj.DamageType.CountsAsClass(DamageClass.Summon) && ((summonCritChance > 0 && Main.rand.NextBool(totalSummonCritChance, 100)) || summonCritChance > 100))
            {
                modifiers.SetCrit();
            }

            modifiers.CritDamage += (proj.GetGlobalProjectile<PacnyProjectile>().critDamageMod);
            modifiers.CritDamage *= critDamageMult;
        }

        public delegate void DoubleTapDelegate(Player player);
        public static event DoubleTapDelegate DoubleTapEvent;
        public static event DoubleTapDelegate DoubleTapPrimaryEvent;
        public static event DoubleTapDelegate DoubleTapSecondaryEvent;

        public void DoubleTap(Player player, int keyDir)
        {
            DoubleTapEvent?.Invoke(player);

            if ((Main.ReversedUpDownArmorSetBonuses && keyDir == 1) || (!Main.ReversedUpDownArmorSetBonuses && keyDir == 0))
                DoubleTapPrimaryEvent?.Invoke(player);

            if ((Main.ReversedUpDownArmorSetBonuses && keyDir == 0) || (!Main.ReversedUpDownArmorSetBonuses && keyDir == 1))
                DoubleTapSecondaryEvent?.Invoke(player);
        }

        public delegate void ResetEffectsDelegate(PacnyPlayer player);
        public static event ResetEffectsDelegate ResetEffectsEvent;
        public override void ResetEffects()
        {
            ResetEffectsEvent?.Invoke(this);

            itemSpeed = 1;
            critDamageMult = 1f;
            meleeCritDamageMult = rangedCritDamageMult = magicCritDamageMult = summonCritDamageMult = 1f;
            summonCritChance = 0;
        }

        public override void Load()
        {
            On_Player.KeyDoubleTap += DoubleTapKey;
        }

        public override void Unload()
        {
            On_Player.KeyDoubleTap -= DoubleTapKey;

            ResetEffectsEvent = null;
        }

        private static void DoubleTapKey(On_Player.orig_KeyDoubleTap orig, Player self, int keyDir)
        {
            orig(self, keyDir);

            self.GetModPlayer<PacnyPlayer>().DoubleTap(self, keyDir);
        }
    }
}
