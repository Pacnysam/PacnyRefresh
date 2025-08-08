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
            binaryWriter.Write(projectile.extraUpdates);
            binaryWriter.Write(counter);
            binaryWriter.Write(gravity);
            binaryWriter.Write(gravityDelay);
            binaryWriter.Write(critDamageMod);
        }

        public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
        {
            projectile.extraUpdates = binaryReader.ReadInt32();
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
    }
}
