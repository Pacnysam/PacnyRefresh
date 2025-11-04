using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.UI;
using Terraria.ModLoader;
using static Terraria.WorldGen;
using static Terraria.ModLoader.ModContent;
using Terraria.Localization;
using ReLogic.Content;


namespace PacnyRefresh.Core //most of this is snatched from starlight river and spirit, i (pacnysam) did not code any of this!
{
    public static partial class Helper
    {
        public static Rectangle ToRectangle(this Vector2 vector) => new Rectangle(0, 0, (int)vector.X, (int)vector.Y);
        public static Vector2 ScreenSize => new(Main.screenWidth, Main.screenHeight);

        public static bool OnScreen(Vector2 pos) => pos.X > -16 && pos.X < Main.screenWidth + 16 && pos.Y > -16 && pos.Y < Main.screenHeight + 16;

        public static bool OnScreen(Rectangle rect) => rect.Intersects(new Rectangle(0, 0, Main.screenWidth, Main.screenHeight));

        public static bool OnScreen(Vector2 pos, Vector2 size) => OnScreen(new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y));

        public static float Ratio(float width, float height) => height / width;

        public static Vector3 Vec3(this Vector2 vector) => new Vector3(vector.X, vector.Y, 0);

        public static Vector3 ScreenCoord(this Vector3 vector) => new(-1 + vector.X / Main.screenWidth * 2, (-1 + vector.Y / Main.screenHeight * 2f) * -1, 0);

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

        public static string EmptyTexString => "PacnyRefresh/Textures/Empty";
        public static Texture2D EmptyTex => Request<Texture2D>("PacnyRefresh/Textures/Empty").Value;

        public static string SetBonusKey => Language.GetTextValue(Main.ReversedUpDownArmorSetBonuses ? "Key.UP" : "Key.DOWN");
        public static string SetBonusSecondaryKey() => Language.GetTextValue(Main.ReversedUpDownArmorSetBonuses ? "Key.DOWN" : "Key.UP");

        public static float LerpFloat(float min, float max, float val)
        {
            float difference = max - min;
            return min + difference * val;
        }

        public static bool CheckCircularCollision(Vector2 center, int radius, Rectangle hitbox)
        {
            if (Vector2.Distance(center, hitbox.TopLeft()) <= radius) return true;
            if (Vector2.Distance(center, hitbox.TopRight()) <= radius) return true;
            if (Vector2.Distance(center, hitbox.BottomLeft()) <= radius) return true;
            return Vector2.Distance(center, hitbox.BottomRight()) <= radius;
        }

        public static bool CheckConicalCollision(Vector2 center, int radius, float angle, float width, Rectangle hitbox)
        {
            if (CheckPoint(center, radius, hitbox.TopLeft(), angle, width)) return true;
            if (CheckPoint(center, radius, hitbox.TopRight(), angle, width)) return true;
            if (CheckPoint(center, radius, hitbox.BottomLeft(), angle, width)) return true;
            return CheckPoint(center, radius, hitbox.BottomRight(), angle, width);
        }

        private static bool CheckPoint(Vector2 center, int radius, Vector2 check, float angle, float width)
        {
            float thisAngle = (center - check).ToRotation() % 6.28f;
            return Vector2.Distance(center, check) <= radius && thisAngle > angle - width && thisAngle < angle + width;
        }

        /*public static bool PointInTile(Vector2 point)
        {
            Point16 startCoords = new Point16((int)point.X / 16, (int)point.Y / 16);
            for(int x = -1; x <= 1; x++)
                for(int y = -1; y <= 1; y++)
                {                 
                    var thisPoint = startCoords + new Point16(x, y);

                    if (!WorldGen.InWorld(thisPoint.X, thisPoint.Y)) return false;

                        var tile = Framing.GetTileSafely(thisPoint);
                    if(tile. == 1)
                    {
                        var rect = new Rectangle(thisPoint.X * 16, thisPoint.Y * 16, 16, 16);
                        if (rect.Contains(point.ToPoint())) return true;
                    }
                }

            return false;
        }*/

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

        public static float Counter(this Projectile projectile) => projectile.GetGlobalProjectile<PacnyProjectile>().counter;

        public static string TicksToTime(int ticks)
        {
            int sec = ticks / 60;
            return sec / 60 + ":" + (sec % 60 < 10 ? "0" + sec % 60 : "" + sec % 60);
        }

        public static int TimeToTicks(int hours, int min, int sec)
        {
            return hours * 216000 + min * 3600 + sec * 60;
        }
        public static int TimeToTicks(int min, int sec)
        {
            return min * 3600 + sec * 60;
        }
        public static int TimeToTicks(int sec)
        {
            return sec * 60;
        }

        public static float CompareAngle(float baseAngle, float targetAngle)
        {
            return (baseAngle - targetAngle + (float)Math.PI * 3) % MathHelper.TwoPi - (float)Math.PI;
        }

        public static float ConvertAngle(float angleIn)
        {
            return CompareAngle(0, angleIn) + (float)Math.PI;
        }

        public static string WrapString(string input, int length, DynamicSpriteFont font, float scale)
        {
            string output = "";
            string[] words = input.Split();

            string line = "";
            foreach (string str in words)
            {
                if (str == "NEWBLOCK")
                {
                    output += "\n\n";
                    line = "";
                    continue;
                }

                if (font.MeasureString(line).X * scale < length)
                {
                    output += " " + str;
                    line += " " + str;
                }
                else
                {
                    output += "\n" + str;
                    line = str;
                }
            }
            return output;
        }

        public static List<T> RandomizeList<T>(List<T> input)
        {
            int n = input.Count();
            while (n > 1)
            {
                n--;
                int k = Main.rand.Next(n + 1);
                T value = input[k];
                input[k] = input[n];
                input[n] = value;
            }
            return input;
        }

        public static Player FindNearestPlayer(Vector2 position)
        {
            Player player = null;

            for (int k = 0; k < Main.maxPlayers; k++)
                if (Main.player[k] != null && Main.player[k].active && (player == null || Vector2.DistanceSquared(position, Main.player[k].Center) < Vector2.DistanceSquared(position, player.Center)))
                    player = Main.player[k];
            return player;
        }

        public static float BezierEase(float time)
        {
            return time * time / (2f * (time * time - time) + 1f);
        }

        public static T[] FastUnion<T>(this T[] front, T[] back)
        {
            T[] combined = new T[front.Length + back.Length];

            Array.Copy(front, combined, front.Length);
            Array.Copy(back, 0, combined, front.Length, back.Length);

            return combined;
        }

        public static bool ConsumeItems(this Item[] inventory, Predicate<Item> predicate, int count)
        {
            var items = inventory.GetItems(predicate, count);

            // If the sum of items is less than the required amount, don't bother.
            if (items.Sum(i => inventory[i].stack) < count)
                return false;

            for (int i = 0; i < items.Count; i++)
            {
                Item item = inventory[items[i]];

                // If we're at the last item stack, and we're not going to consume the whole thing, just decrease its segmentTimer by the amount needed.
                if (i == items.Count - 1 && count < item.stack)
                {
                    item.stack -= count;
                }
                // Otherwise, delete the item and decrement segmentTimer as needed.
                else
                {
                    count -= item.stack;
                    item.TurnToAir();
                }
            }
            return true;
        }

        /// <summary>
        /// Gets a list of item indeces from an inventory matching a predicate.
        /// </summary>
        /// <param name="inventory">The pool of items to search.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="stopCountingAt">The number of items to search before stopping.</param>
        /// <returns>The items matching the predicate.</returns>
        public static List<int> GetItems(this Item[] inventory, Predicate<Item> predicate, int stopCountingAt = int.MaxValue)
        {
            var indeces = new List<int>();
            for (int i = 0; i < inventory.Length; i++)
            {
                if (stopCountingAt <= 0)
                    break;
                if (predicate(inventory[i]))
                {
                    indeces.Add(i);
                    stopCountingAt -= inventory[i].stack;
                }
            }
            return indeces;
        }
        public static bool HasItem(Player player, int type, int count)
        {
            int items = 0;

            for (int k = 0; k < player.inventory.Length; k++)
            {
                Item item = player.inventory[k];
                if (item.type == type) items += item.stack;
            }

            return items >= count;
        }

        /*public static bool HasItems(player player, int[,] types, bool mustHaveAll) 
        {
            int[] items;

            for (int k = 0; k < types.Length|| k < 3; k++)
            {
                if (HasItem(player, types[k, 0], types[k, 1])) 
                {
                    if (!mustHaveAll) return true;
                    
                }
            }
            return false;
        }*/

        public static bool HasAccessory(Player player, int item, bool vanity)
        {
            int maxAccessoryIndex = 5 + player.extraAccessorySlots;
            int index = 3; if (vanity) index = 13;

            for (int i = index; i < index + maxAccessoryIndex; i++)
            {
                if (player.armor[i].type == item)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool HasAccessory(Player player, int item)
        {
            int maxAccessoryIndex = 5 + player.extraAccessorySlots;

            for (int i = 3; i < 3 + maxAccessoryIndex; i++)
            {
                if (player.armor[i].type == item)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool TryTakeItem(Player player, int type, int count)
        {
            if (HasItem(player, type, count))
            {
                int toTake = count;

                for (int k = 0; k < player.inventory.Length; k++)
                {
                    Item item = player.inventory[k];

                    if (item.type == type)
                    {
                        int stack = item.stack;
                        for (int i = 0; i < stack; i++)
                        {
                            item.stack--;
                            if (item.stack == 0) item.TurnToAir();

                            toTake--;
                            if (toTake <= 0) break;
                        }
                    }
                    if (toTake == 0) break;
                }

                return true;
            }
            else return false;
        }

        public static void NewItemSpecific(Vector2 position, Item item)
        {
            int targetIndex = 400;
            Main.item[400] = new Item(); //Vanilla seems to make sure to set the dummy here, so I will too.

            if (Main.netMode != NetmodeID.MultiplayerClient) //Main.Item index finder from vanilla
            {
                for (int j = 0; j < 400; j++)
                {
                    if (!Main.item[j].active && Main.timeItemSlotCannotBeReusedFor[j] == 0)
                    {
                        targetIndex = j;
                        break;
                    }
                }
            }

            if (targetIndex == 400 && Main.netMode != NetmodeID.MultiplayerClient) //some sort of vanilla failsafe if no safe index is found it seems?
            {
                int num2 = 0;
                for (int k = 0; k < 400; k++)
                {
                    if (Main.item[k].timeSinceItemSpawned - Main.timeItemSlotCannotBeReusedFor[k] > num2)
                    {
                        num2 = Main.item[k].timeSinceItemSpawned - Main.timeItemSlotCannotBeReusedFor[k];
                        targetIndex = k;
                    }
                }
            }

            Main.item[targetIndex] = item;
            Main.item[targetIndex].position = position;

            if (ItemSlot.Options.HighlightNewItems && item.type >= ItemID.None) //vanilla item highlight system
            {
                item.newAndShiny = true;
            }
            if (Main.netMode == NetmodeID.Server) //syncing
            {
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, targetIndex, 0, 0f, 0f, 0, 0, 0);
                item.FindOwner(item.whoAmI);
            }
            else if (Main.netMode == NetmodeID.SinglePlayer)
            {
                item.playerIndexTheItemIsReservedFor = Main.myPlayer;
            }
        }

        public static Item NewItemPerfect(Vector2 position, Vector2 velocity, int type, int stack = 1, bool noBroadcast = false, int prefixGiven = 0, bool noGrabDelay = false, bool reverseLookup = false)
        {
            int targetIndex = 400;
            Main.item[400] = new Item(); //Vanilla seems to make sure to set the dummy here, so I will too.

            if (Main.netMode != NetmodeID.MultiplayerClient) //Main.Item index finder from vanilla
            {
                if (reverseLookup)
                {
                    for (int i = 399; i >= 0; i--)
                    {
                        if (!Main.item[i].active && Main.timeItemSlotCannotBeReusedFor[i] == 0)
                        {
                            targetIndex = i;
                            break;
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < 400; j++)
                    {
                        if (!Main.item[j].active && Main.timeItemSlotCannotBeReusedFor[j] == 0)
                        {
                            targetIndex = j;
                            break;
                        }
                    }
                }
            }
            if (targetIndex == 400 && Main.netMode != NetmodeID.MultiplayerClient) //some sort of vanilla failsafe if no safe index is found it seems?
            {
                int num2 = 0;
                for (int k = 0; k < 400; k++)
                {
                    if (Main.item[k].timeSinceItemSpawned - Main.timeItemSlotCannotBeReusedFor[k] > num2)
                    {
                        num2 = Main.item[k].timeSinceItemSpawned - Main.timeItemSlotCannotBeReusedFor[k];
                        targetIndex = k;
                    }
                }
            }
            Main.timeItemSlotCannotBeReusedFor[targetIndex] = 0; //normal stuff
            Item item = Main.item[targetIndex];
            item.SetDefaults(type, false);
            item.Prefix(prefixGiven);
            item.position = position;
            item.velocity = velocity;
            item.active = true;
            item.timeSinceItemSpawned = 0;
            item.stack = stack;

            item.wet = Collision.WetCollision(item.position, item.width, item.height); //not sure what this is, from vanilla

            if (ItemSlot.Options.HighlightNewItems && item.type >= ItemID.None) //vanilla item highlight system
            {
                item.newAndShiny = true;
            }
            if (Main.netMode == NetmodeID.Server && !noBroadcast) //syncing
            {
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, targetIndex, noGrabDelay ? 1 : 0, 0f, 0f, 0, 0, 0);
                item.FindOwner(item.whoAmI);
            }
            else if (Main.netMode == NetmodeID.SinglePlayer)
            {
                item.playerIndexTheItemIsReservedFor = Main.myPlayer;
            }
            return item;
        }

        public static bool IsWeapon(this Item item) => item.type != ItemID.None && item.stack > 0 && item.useStyle > ItemUseStyleID.None && (item.damage > 0 || item.useAmmo > 0 && item.useAmmo != AmmoID.Solution);

        public static void DropItem(this Entity ent, int type, IEntitySource source, int stack = 1)
        {
            int i = Item.NewItem(source, ent.Hitbox, type, stack);
            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, i);
        }

        public static void DropItem(this Entity ent, int type, float chance, IEntitySource source, int stack = 1)
        {
            if (Main.rand.NextDouble() < chance)
            {
                int i = Item.NewItem(source, ent.Hitbox, type, stack);
                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, i);
            }
        }

        public static bool IsTargetValid(NPC npc) => npc.active && !npc.friendly && !npc.immortal && !npc.dontTakeDamage;

        public static double Distribution(int pos, int maxVal, float posOffset = 0.5f, float maxChance = 100f)
        {
            return -Math.Pow(20 * (pos - posOffset * maxVal) / maxVal, 2) + maxChance;
        }

        public static void OutlineRect(Rectangle rect, int tileType)
        {
            for (int i = 0; i < rect.Width; i++)
                PlaceTile(rect.X + i, rect.Y, tileType, true, true);

            for (int i = 0; i < rect.Width; i++)
                PlaceTile(rect.X + i, rect.Y + rect.Height, tileType, true, true);

            for (int i = 0; i < rect.Height; i++)
                PlaceTile(rect.X, rect.Y + i, tileType, true, true);

            for (int i = 0; i < rect.Height; i++)
                PlaceTile(rect.X + rect.Width, rect.Y + i, tileType, true, true);
        }

        public static bool IsValidDebuff(Player player, int buffindex)
        {
            int bufftype = player.buffType[buffindex];
            bool vitalbuff = bufftype == BuffID.PotionSickness || bufftype == BuffID.ManaSickness || bufftype == BuffID.ChaosState;
            return player.buffTime[buffindex] > 2 && Main.debuff[bufftype] && !Main.buffNoTimeDisplay[bufftype] && !Main.vanityPet[bufftype] && !vitalbuff;
        }

        public static bool IsValidDebuff(int bufftype, int time)
        {
            bool vitalbuff = bufftype == BuffID.PotionSickness || bufftype == BuffID.ManaSickness || bufftype == BuffID.ChaosState;
            return time > 2 && Main.debuff[bufftype] && !Main.buffNoTimeDisplay[bufftype] && !Main.vanityPet[bufftype] && !vitalbuff;
        }

        public static void AddScreenshake(Player player, float amount, Vector2 position)
        {
            position -= Main.screenPosition;
            Vector3 coords = new Vector3(position.X, -position.Y, 0).ScreenCoord();
            float mult = Vector3.Distance(coords, Vector3.Zero);

            if (!OnScreen(new Vector2(position.X, position.Y)))
            {
                amount /= mult;
                if (amount < 0) amount = 0;
            }

            player.GetModPlayer<GlobalPlayer>().ScreenShake += (int)(amount * GetInstance<GraphicsConfig>().ShakeIntensity);
        }
        public static void AddScreenshake(Player player, int amount)
        {
            player.GetModPlayer<GlobalPlayer>().ScreenShake += (int)(amount * GetInstance<GraphicsConfig>().ShakeIntensity);
        }
    }

    public static class DustHelper
    {
        public static void DrawStar(Vector2 position, int dustType, float pointAmount = 5, float mainSize = 1, float dustDensity = 1, float dustSize = 1f, float pointDepthMult = 1f, float pointDepthMultOffset = 0.5f, bool noGravity = false, float randomAmount = 0, float rotationAmount = -1)
        {
            float rot;
            if (rotationAmount < 0) { rot = Main.rand.NextFloat(0, (float)Math.PI * 2); } else { rot = rotationAmount; }

            float density = 1 / dustDensity * 0.1f;

            for (float k = 0; k < 6.28f; k += density)
            {
                float rand = 0;
                if (randomAmount > 0) { rand = Main.rand.NextFloat(-0.01f, 0.01f) * randomAmount; }

                float x = (float)Math.Cos(k + rand);
                float y = (float)Math.Sin(k + rand);
                float mult = Math.Abs(k * (pointAmount / 2) % (float)Math.PI - (float)Math.PI / 2) * pointDepthMult + pointDepthMultOffset;//triangle wave function
                Dust.NewDustPerfect(position, dustType, new Vector2(x, y).RotatedBy(rot) * mult * mainSize, 0, default, dustSize).noGravity = noGravity;
            }
        }

        public static void DrawCircle(Vector2 position, int dustType, float mainSize = 1, float RatioX = 1, float RatioY = 1, float dustDensity = 1, float dustSize = 1f, float randomAmount = 0, float rotationAmount = 0, bool nogravity = false)
        {
            float rot;
            if (rotationAmount < 0) { rot = Main.rand.NextFloat(0, (float)Math.PI * 2); } else { rot = rotationAmount; }

            float density = 1 / dustDensity * 0.1f;

            for (float k = 0; k < 6.28f; k += density)
            {
                float rand = 0;
                if (randomAmount > 0) { rand = Main.rand.NextFloat(-0.01f, 0.01f) * randomAmount; }

                float x = (float)Math.Cos(k + rand) * RatioX;
                float y = (float)Math.Sin(k + rand) * RatioY;
                if (dustType == 222 || dustType == 130 || nogravity)
                {
                    Dust.NewDustPerfect(position, dustType, new Vector2(x, y).RotatedBy(rot) * mainSize, 0, default, dustSize).noGravity = true;
                }
                else
                {
                    Dust.NewDustPerfect(position, dustType, new Vector2(x, y).RotatedBy(rot) * mainSize, 0, default, dustSize);
                }
            }
        }
        public static void DrawTriangle(Vector2 position, int dustType, float size, float dustDensity = 1f, float dustSize = 2f, float rotationAmount = -1, bool noGravity = true)
        {
            float rot;
            if (rotationAmount < 0) { rot = Main.rand.NextFloat(0, (float)Math.PI * 2); } else { rot = rotationAmount; }
            float density = 1 / dustDensity * 0.1f;
            float x = 1;
            float y = 0;
            for (float k = 0; k < 6.3f; k += density)
            {
                if (k % 2.093333f <= density)
                {
                    x = (float)Math.Cos(k);
                    y = (float)Math.Sin(k);
                }
                Vector2 offsetVect = new Vector2(x, y);
                offsetVect = offsetVect.RotatedBy(2.093333f);
                offsetVect *= k % 2.093333f / 2.093333f * 2f;
                Dust.NewDustPerfect(position, dustType, (new Vector2(x, y) + offsetVect).RotatedBy(rot) * size, 0, default, dustSize).noGravity = noGravity;
                //not the cleanest, but im tired of trying, ive legit been at this for 2 hours. Maybe im missing something really obvious, but hardcode a fucking hoy
                offsetVect = new Vector2(x, y);
                offsetVect = offsetVect.RotatedBy(-1.046667);
                offsetVect *= k % 2.093333f / 2.093333f;
                Dust.NewDustPerfect(position, dustType, (new Vector2(x, y) + offsetVect).RotatedBy(rot) * size, 0, default, dustSize).noGravity = noGravity;
            }
        }
        public static void DrawDiamond(Vector2 position, int dustType, float size, float dustDensity = 1f, float dustSize = 2f, float rotationAmount = -1, bool noGravity = true)
        {
            float rot;
            if (rotationAmount < 0) { rot = Main.rand.NextFloat(0, (float)Math.PI * 2); } else { rot = rotationAmount; }
            float density = 1 / dustDensity * 0.1f;
            float x = 1;
            float y = 0;
            for (float k = 0; k < 6.3f; k += density)
            {
                if (k % 1.57f <= density)
                {
                    x = (float)Math.Cos(k);
                    y = (float)Math.Sin(k);
                }
                Vector2 offsetVect = new Vector2(x, y);
                offsetVect = offsetVect.RotatedBy(1.57f);
                offsetVect *= k % 1.57f / 1.57f;
                Dust.NewDustPerfect(position, dustType, (new Vector2(x, y) + offsetVect).RotatedBy(rot) * size, 0, default, dustSize).noGravity = noGravity;
                //not the cleanest, but im tired of trying, ive legit been at this for 2 hours. Maybe im missing something really obvious, but hardcode a fucking hoy
                offsetVect = new Vector2(x, y);
                offsetVect = offsetVect.RotatedBy(-1.57f);
                offsetVect *= k % 1.57f / 1.57f;
                Dust.NewDustPerfect(position, dustType, (new Vector2(x, y) + offsetVect).RotatedBy(rot) * size, 0, default, dustSize).noGravity = noGravity;
            }
        }

        public static void DrawDustImage(Vector2 position, int dustType, float size, string imagePath, float dustSize = 1f, bool noGravity = true, float rot = 0.34f)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                float rotation = Main.rand.NextFloat(0 - rot, rot);
                Texture2D glyphTexture = Request<Texture2D>(imagePath).Value;
                Color[] data = new Color[glyphTexture.Width * glyphTexture.Height];
                glyphTexture.GetData(data);
                for (int i = 0; i < glyphTexture.Width; i += 2)
                {
                    for (int j = 0; j < glyphTexture.Height; j += 2)
                    {
                        Color alpha = data[j * glyphTexture.Width + i];
                        if (alpha == new Color(0, 0, 0))
                        {
                            double dustX = i - glyphTexture.Width / 2;
                            double dustY = j - glyphTexture.Height / 2;
                            dustX *= size;
                            dustY *= size;
                            Dust.NewDustPerfect(position, dustType, new Vector2((float)dustX, (float)dustY).RotatedBy(rotation)).noGravity = noGravity;
                        }
                    }
                }
            }
        }

        public static void DrawDustImageRainbow(Vector2 position, float size, string imagePath, float dustSize = 1f, bool noGravity = true, float rot = 0.34f)
        {
            int red = Main.rand.Next(60, 255);
            int green = Main.rand.Next(60, 255);
            int blue = Main.rand.Next(60, 255);
            Color color = new Color(red, green, blue);
            if (Main.netMode != NetmodeID.Server)
            {
                float rotation = Main.rand.NextFloat(0 - rot, rot);
                Texture2D glyphTexture = Request<Texture2D>(imagePath).Value;
                Color[] data = new Color[glyphTexture.Width * glyphTexture.Height];
                glyphTexture.GetData(data);
                for (int i = 0; i < glyphTexture.Width; i += 2)
                {
                    for (int j = 0; j < glyphTexture.Height; j += 2)
                    {
                        Color alpha = data[j * glyphTexture.Width + i];
                        if (alpha == new Color(0, 0, 0))
                        {
                            double dustX = i - glyphTexture.Width / 2;
                            double dustY = j - glyphTexture.Height / 2;
                            dustX *= size;
                            dustY *= size;
                            Vector2 dir = new Vector2((float)dustX, (float)dustY).RotatedBy(rotation);
                            Dust.NewDustPerfect(position, 267, dir, 0, color, dustSize).noGravity = noGravity;
                        }
                    }
                }
            }
        }

        public static void DrawElectricity(Vector2 point1, Vector2 point2, int dusttype, float scale = 1, int armLength = 30, Color color = default, float density = 0.05f)
        {
            int nodeCount = (int)Vector2.Distance(point1, point2) / armLength;
            Vector2[] nodes = new Vector2[nodeCount + 1];

            nodes[nodeCount] = point2; //adds the end as the last point

            for (int k = 1; k < nodes.Count(); k++)
            {
                //Sets all intermediate nodes to their appropriate randomized dot product positions
                nodes[k] = Vector2.Lerp(point1, point2, k / (float)nodeCount) +
                    (k == nodes.Count() - 1 ? Vector2.Zero : Vector2.Normalize(point1 - point2).RotatedBy(1.58f) * Main.rand.NextFloat(-armLength / 2, armLength / 2));

                //Spawns the dust between each node
                Vector2 prevPos = k == 1 ? point1 : nodes[k - 1];
                for (float i = 0; i < 1; i += density)
                {
                    Dust d = Dust.NewDustPerfect(Vector2.Lerp(prevPos, nodes[k], i), dusttype, Vector2.Zero, 0, color, scale);
                    d.noGravity = true;
                }
            }
        }

        public static int TileDust(Tile tile, ref int dusttype)
        {
            switch (tile.TileType)
            {
                case TileID.Stone: dusttype = DustID.Stone; break;
                case TileID.Sand: case TileID.Sandstone: dusttype = 32; break;
                case TileID.Granite: dusttype = DustID.Granite; break;
                case TileID.Marble: dusttype = DustID.Marble; break;
                case TileID.Grass: case TileID.JungleGrass: dusttype = DustID.Grass; break;
                case TileID.MushroomGrass: case TileID.MushroomBlock: dusttype = 96; break;

                default:
                    if (TileID.Sets.Crimson[tile.TileType])
                        dusttype = DustID.Blood;
                    if (TileID.Sets.Corrupt[tile.TileType])
                        dusttype = 14;
                    if (TileID.Sets.Ices[tile.TileType] || TileID.Sets.IcesSnow[tile.TileType])
                        dusttype = DustID.Ice;
                    if (TileID.Sets.Snow[tile.TileType] || tile.TileType == TileID.Cloud || tile.TileType == TileID.RainCloud)
                        dusttype = 51;

                    ModTile modtile = TileLoader.GetTile(tile.TileType);
                    if (modtile != null)
                        dusttype = modtile.DustType;
                    break;
            }

            return dusttype;

        }
    }

    public static class ColorHelper
    {
        public static Color AdditiveWhite(byte alpha = 0) => new(255, 255, 255) { A = alpha };
        public static Color AdditiveWhite() => new(255, 255, 255) { A = 0 };

        public static Color GemColor(int gem)
        {
            switch (gem)
            {
                case 1: //amethyst
                case ItemID.Amethyst:
                    {
                        return new Color(193, 47, 246);
                    }
                case 2: //topaz
                case ItemID.Topaz:
                    {
                        return new Color(246, 188, 0);
                    }
                case 3: //sapphire
                case ItemID.Sapphire:
                    {
                        return new Color(86, 135, 255);
                    }
                case 4: //emerald
                case ItemID.Emerald:
                    {
                        return new Color(41, 206, 131);
                    }
                case 5: //rubyCounter
                case ItemID.Ruby:
                    {
                        return new Color(237, 26, 30);
                    }
                case 6: //diamond
                case ItemID.Diamond:
                    {
                        return Color.White;
                    }
                case 7: //amber
                case ItemID.Amber:
                    {
                        return new Color(244, 133, 27);
                    }
            }
            return Color.White;
        }

        public static Color RarityColor(int rarity)
        {
            switch (rarity)
            {
                case ItemRarityID.Gray:
                    {
                        return Colors.RarityTrash;
                    }
                case ItemRarityID.White:
                    {
                        return Color.White;
                    }
                case ItemRarityID.Blue:
                    {
                        return Colors.RarityBlue;
                    }
                case ItemRarityID.Green:
                    {
                        return Colors.RarityGreen;
                    }
                case ItemRarityID.Orange:
                    {
                        return Colors.RarityOrange;
                    }
                case ItemRarityID.LightRed:
                    {
                        return Colors.RarityRed;
                    }
                case ItemRarityID.Pink:
                    {
                        return Colors.RarityPink;
                    }
                case ItemRarityID.LightPurple:
                    {
                        return Colors.RarityPurple;
                    }
                case ItemRarityID.Lime:
                    {
                        return Colors.RarityLime;
                    }
                case ItemRarityID.Yellow:
                    {
                        return Colors.RarityYellow;
                    }
                case ItemRarityID.Cyan:
                    {
                        return Colors.RarityCyan;
                    }
                case ItemRarityID.Red:
                    {
                        return Colors.RarityDarkRed;
                    }
                case ItemRarityID.Purple:
                    {
                        return Colors.RarityDarkPurple;
                    }
                case ItemRarityID.Expert:
                    {
                        return Main.DiscoColor;
                    }
                case ItemRarityID.Master:
                    {
                        return Main.mcColor;
                    }
                case ItemRarityID.Quest:
                    {
                        return Colors.RarityAmber;
                    }
            }
            return Color.White;
        }

        /// <summary>
		/// SlimeBlue color (R:0,G:80,B:255,A:125-175).
		/// </summary>
        public static Color SlimeBlue => new(0, 80, 255, 255);
        /// <summary>
		/// SlimeBlueSimple color (R:112,G:172,B:244,A:255).
		/// </summary>
        public static Color SlimeBlueSimple => new(112, 172, 244, 255);
    }
}

