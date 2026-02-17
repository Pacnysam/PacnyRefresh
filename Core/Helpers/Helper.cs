using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using static Terraria.ModLoader.ModContent;
using static Terraria.WorldGen;


namespace PacnyRefresh.Core
{
    public static partial class Helper
    {
        public static Vector2 ScreenSize => new(Main.screenWidth, Main.screenHeight);
        public static float WorldTimer => Main.GlobalTimeWrappedHourly;
        public static bool IsVanitySet(Player player, int head, int body, int legs)
        {
            if (player.armor[0].type == head && player.armor[10].type == ItemID.None || player.armor[10].type == head &&
                player.armor[1].type == body && player.armor[11].type == ItemID.None || player.armor[11].type == body &&
                player.armor[2].type == legs && player.armor[12].type == ItemID.None || player.armor[12].type == legs) return true;
            return false;
        }
        public static bool IsVanitySet(Player player, int head, int body)
        {
            if (player.armor[0].type == head && player.armor[10].type == ItemID.None || player.armor[10].type == head &&
                player.armor[1].type == body && player.armor[11].type == ItemID.None || player.armor[11].type == body) return true;
            return false;
        }

        public static string EmptyTexString => "PacnyRefresh/Assets/Textures/Empty";
        public static Texture2D EmptyTex => Request<Texture2D>("PacnyRefresh/Assets/Textures/Empty").Value;

        public static string SetBonusKey => Language.GetTextValue(Main.ReversedUpDownArmorSetBonuses ? "Key.UP" : "Key.DOWN");
        public static string SetBonusSecondaryKey() => Language.GetTextValue(Main.ReversedUpDownArmorSetBonuses ? "Key.DOWN" : "Key.UP");
        public static float LavaLayer => Main.maxTilesY * 0.72f;

        public static bool CheckCircularCollision(Vector2 center, int radius, Rectangle hitbox)
        {
            if (Vector2.Distance(center, hitbox.TopLeft()) <= radius) return true;
            if (Vector2.Distance(center, hitbox.TopRight()) <= radius) return true;
            if (Vector2.Distance(center, hitbox.BottomLeft()) <= radius) return true;
            return Vector2.Distance(center, hitbox.BottomRight()) <= radius;
        }

        public static float Counter(this Projectile projectile) => projectile.GetGlobalProjectile<PacnyProjectile>().counter;
        //public static float Counter(this NPC npc) => npc.GetGlobalNPC<PacnyNPC>().counter;

        public static bool CanBeStunned(this NPC npc) => !npc.boss && !NPCID.Sets.ShouldBeCountedAsBoss[npc.type] /*&& !NPCSets.ccImmune[npc.type]*/
            && npc.aiStyle != NPCAIStyleID.Worm && (npc.knockBackResist != 0f /*|| NPCSets.ccSusceptibleException[npc.type]*/);

        public static bool ZoneLava(this Player player) => player.position.Y / 16 >= Main.maxTilesY * 0.72f;
        public static bool ZoneForest(this Player Player)
        {
            return !Player.ZoneJungle
                && !Player.ZoneDungeon
                && !Player.ZoneCorrupt
                && !Player.ZoneCrimson
                && !Player.ZoneHallow
                && !Player.ZoneSnow
                && !Player.ZoneUndergroundDesert
                && !Player.ZoneGlowshroom
                && !Player.ZoneMeteor
                && !Player.ZoneBeach
                && !Player.ZoneDesert
                && Player.ZoneOverworldHeight;
        }

        public static int TimeToTicks(float hours, float min, float sec) => (int)((hours * 216000) + (min * 3600) + (sec * 60));
        public static int TimeToTicks(float min, float sec) => (int)((min * 3600) + (sec * 60));
        public static int TimeToTicks(float sec) => (int)(sec * 60);

        public static float Opacity(this Gore gore) => 1f - gore.alpha / 255f;

        public static bool Toggle(this ref bool input) => input = !input;
        public static float RandNeg(this float num) => Main.rand.NextBool() ? num : -num;
        public static int RandNeg(this int num) => Main.rand.NextBool() ? num : -num;

        public static void WritePoint16(this BinaryWriter writer, Point16 point) { writer.Write(point.X); writer.Write(point.Y); }
        public static Point16 ReadPoint16(this BinaryReader reader) => new(reader.ReadInt16(), reader.ReadInt16());

        public static float BezierEase(float time)
        {
            return time * time / (2f * (time * time - time) + 1f);
        }

        public static bool IsWeapon(this Item item) => item.type != ItemID.None && item.stack > 0 && (item.damage > 0 || item.useAmmo > 0 && item.useAmmo != AmmoID.Solution);

        public static PlayerDeathReason QuickDeathReason(string key, Player player, int variants = 0)
        {
            NetworkText text = NetworkText.FromKey("Mods.GoldLeaf.DeathReasons." + key + ((variants == 0) ? "" : ".Variant" + (1 + Main.rand.Next(variants))), player.name);
            return PlayerDeathReason.ByCustomReason(text);
        }

        public static void ReduceBuffTime(this Player player, int buffType, int timeChange)
        {
            int buffTime = player.buffTime[player.FindBuffIndex(buffType)] - timeChange;
            player.ClearBuff(buffType);
            if (buffTime > 2) player.AddBuff(buffType, buffTime);
        }

        public static bool IsTargetValid(NPC npc) => npc.active && !npc.friendly && !npc.immortal && !npc.dontTakeDamage && npc.lifeMax > 5;
        public static bool IsValid(this NPC npc) => IsTargetValid(npc);

        public static bool TileNearby(Point position, int distance, int type/*, Predicate<Tile> predicate*/)
        {
            for (int i = Math.Clamp(position.X - distance, 0, Main.maxTilesX); i <= position.X + distance || i > Main.maxTilesX; i++)
            {
                for (int j = Math.Clamp(position.Y - distance, 0, Main.maxTilesY); j <= position.Y + distance || j > Main.maxTilesY; j++)
                {
                    if (Vector2.Distance(new Vector2(position.X, position.Y), new Vector2(i, j)) <= distance && Main.tile[i, j].TileType == type)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool IsValidDebuff(Player player, int buffindex)
        {
            int bufftype = player.buffType[buffindex];
            bool vitalbuff = (BuffSets.Cooldown[bufftype] || !BuffSets.IsRemovable[bufftype] || BuffSets.Cosmetic[bufftype]);
            return player.buffTime[buffindex] > 2 && Main.debuff[bufftype] && !Main.buffNoTimeDisplay[bufftype] && !Main.vanityPet[bufftype] && !vitalbuff;
        }

        public static bool IsValidDebuff(int bufftype, int time)
        {
            bool vitalbuff = (BuffSets.Cooldown[bufftype] || !BuffSets.IsRemovable[bufftype] || BuffSets.Cosmetic[bufftype]);
            return time > 2 && Main.debuff[bufftype] && !Main.buffNoTimeDisplay[bufftype] && !Main.vanityPet[bufftype] && !vitalbuff;
        }

        public static string CoolBuffTex(string input) => GetInstance<VisualConfig>().CoolBuffs ? input + "Cool" : input;
    }
}

