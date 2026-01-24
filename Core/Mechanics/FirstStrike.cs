using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using static Terraria.ModLoader.ModContent;
using static PacnyRefresh.Core.Helper;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;
using Terraria.WorldBuilding;
using Terraria.ModLoader.IO;
using System.IO;
using System.Threading;
using Terraria.ID;

namespace PacnyRefresh.Core.Mechanics
{
    public class FirstStrikePlayer : ModPlayer
    {
        public delegate void FirstStrikeModificationDelegate(Player player, NPC target, NPC.HitModifiers hit);
        public static event FirstStrikeModificationDelegate ModifyFirstStrike;

        public delegate void FirstStrikeDelegate(Player player, NPC target, NPC.HitInfo hit, int damageDone);
        public static event FirstStrikeDelegate OnFirstStrike;

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
           if (!target.GetGlobalNPC<FirstStrikeNPC>().hasBeenStruck && IsTargetValid(target))
           {
                ModifyFirstStrike?.Invoke(Player, target, modifiers);
           }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!target.GetGlobalNPC<FirstStrikeNPC>().hasBeenStruck && IsTargetValid(target))
            {
                OnFirstStrike?.Invoke(Player, target, hit, damageDone);
                target.GetGlobalNPC<FirstStrikeNPC>().hasBeenStruck = true;
            }
        }
    }

    public class FirstStrikeNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        //public delegate void FirstStruckDelegate(NPC npc, NPC.HitInfo hit, Entity attacker);
        //public static event FirstStruckDelegate FirstStruckEvent;

        public bool hasBeenStruck = false;

        public override GlobalNPC Clone(NPC from, NPC to)
        {
            to.GetGlobalNPC(this).hasBeenStruck = from.GetGlobalNPC(this).hasBeenStruck;
            return base.Clone(from, to);
        }

        /*public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (!hasBeenStruck && IsTargetValid(npc))
            {
                FirstStruckEvent?.Invoke(npc, hit, player);
                hasBeenStruck = true;
            }
        }
        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (!hasBeenStruck && IsTargetValid(npc))
            {
                FirstStruckEvent?.Invoke(npc, hit, projectile);
                hasBeenStruck = true;
            }
        }*/

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(hasBeenStruck);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            hasBeenStruck = binaryReader.ReadBoolean();
        }
    }
}
