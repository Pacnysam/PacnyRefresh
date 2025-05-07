using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace PacnyRefresh.Core
{
    public class PacnyProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public float critDamageMod = 0f;

        public float gravity = 0f;
        public int gravityDelay = 0;

        public override void AI(Projectile projectile)
        {
            if (gravity != 0f)
            {
                projectile.velocity.Y += gravity;
            }
        }
    }
}
