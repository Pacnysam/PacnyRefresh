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

namespace PacnyRefresh.Core
{
    public class PacnyProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public float critDamageMod = 0f;

        public float gravity = 0f;
        public int gravityDelay = 0;
        public int spawnTime = 0;
        public int counter = 0;

        public override void AI(Projectile projectile)
        {
            spawnTime--;
            gravityDelay--;
            counter++;

            if (gravity != 0f && gravityDelay <= 0)
            {
                projectile.velocity.Y += gravity;
            }
        }

        public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(counter);
            binaryWriter.Write(gravity);
            binaryWriter.Write(critDamageMod);
        }

        public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
        {
            counter = binaryReader.ReadInt32();
            gravity = binaryReader.ReadInt32();
            critDamageMod = binaryReader.ReadSingle();
        }

        public override bool? CanHitNPC(Projectile projectile, NPC target)
        {
            if (spawnTime > 0)
                return false;
            return base.CanHitNPC(projectile, target);
        }
    }
}
