using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;
using static PacnyRefresh.Core.Helper;
using System;
using Terraria.ModLoader.IO;
using System.IO;
using System.Collections.Generic;

namespace PacnyRefresh.Core
{
    public class MinionSpeedPlayer : ModPlayer
    {
        public float summonSpeed = 0f;
        public float minionSpeed = 0f;
        public float sentrySpeed = 0f;
        public float familiarSpeed = 0f;

        public override void PostUpdateMiscEffects()
        {
            minionSpeed += summonSpeed;
            sentrySpeed += summonSpeed;
            familiarSpeed += summonSpeed;
        }

        public override void ResetEffects()
        {
            minionSpeed = 0f;
            sentrySpeed = 0f;
            familiarSpeed = 0f;
            summonSpeed = 0f;
        }
    }

    public class MinionSpeedProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            return entity.minion || entity.sentry;
        }

        private float minionSpeedCounter;
        private float sentrySpeedCounter;
        //private float familiarSpeedCounter;
        private int extraUpdateCache;

        public override bool PreAI(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];

            if (player != null && !ProjectileSets.summonSpeedImmune[projectile.type])
            {
                for (int k = 0; k < extraUpdateCache; extraUpdateCache--)
                {
                    if (projectile.extraUpdates > 0)
                        projectile.extraUpdates--;
                }

                if (projectile.minion)
                {
                    minionSpeedCounter += player.GetModPlayer<MinionSpeedPlayer>().minionSpeed;

                    for (; minionSpeedCounter >= 1; minionSpeedCounter--)
                    {
                        projectile.extraUpdates++; extraUpdateCache++;
                    }
                }
                if (projectile.sentry)
                {
                    sentrySpeedCounter += player.GetModPlayer<MinionSpeedPlayer>().sentrySpeed;

                    for (; sentrySpeedCounter >= 1; sentrySpeedCounter--)
                    {
                        projectile.extraUpdates++; extraUpdateCache++;
                    }
                }
                /*if (familiar)
                {
                    familiarSpeedCounter += player.GetModPlayer<MinionSpeedPlayer>().sentrySpeed;

                    for (; familiarSpeedCounter >= 1; familiarSpeedCounter--)
                    {
                        projectile.extraUpdates++; extraUpdateCache++;
                    }
                }*/
            }

            return true;
        }
    }
}
