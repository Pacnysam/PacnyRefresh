using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using static Terraria.ModLoader.ModContent;
using static PacnyRefresh.Core.Helper;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.UI;
using ReLogic.Content;

namespace PacnyRefresh.Core
{
    public class GlowmaskItem : GlobalItem
    {
        public override bool InstancePerEntity => false;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return ItemSets.Glowmask[entity.type].Item1 != null;
        }
        public override void PostDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            if (ItemSets.Glowmask[item.type].Item3)
                spriteBatch.Draw(ItemSets.Glowmask[item.type].Item1.Value, item.Center - Main.screenPosition, null, ItemSets.Glowmask[item.type].Item2, rotation, ItemSets.Glowmask[item.type].Item1.Size() / 2, scale, SpriteEffects.None, 0f);
        }
    }

    public class ItemGlowLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.ArmOverItem);

        protected override void Draw(ref PlayerDrawSet drawinfo)
        {
            Player player = drawinfo.drawPlayer;
            if (player.JustDroppedAnItem || ItemSets.Glowmask[player.HeldItem.type].Item1 == null)
                return;

            if (player.heldProj >= 0 && drawinfo.shadow == 0f && !drawinfo.heldProjOverHand)
                drawinfo.projectileDrawPosition = drawinfo.DrawDataCache.Count;

            Item heldItem = drawinfo.heldItem;
            int itemType = heldItem.type;
            Main.instance.LoadItem(itemType);
            float adjustedItemScale = player.GetAdjustedItemScale(heldItem);
            Texture2D tex = ItemSets.Glowmask[player.HeldItem.type].Item1.Value;
            Vector2 position = new((int)(drawinfo.ItemLocation.X - Main.screenPosition.X), (int)(drawinfo.ItemLocation.Y - Main.screenPosition.Y));
            Rectangle itemDrawFrame = player.GetItemDrawFrame(itemType);
            drawinfo.itemColor = Lighting.GetColor((int)(drawinfo.Position.X + player.width * 0.5) / 16, (int)((drawinfo.Position.Y + player.height * 0.5) / 16));

            if (player.shroomiteStealth && heldItem.DamageType == DamageClass.Ranged)
            {
                float stealth = player.stealth;
                if ((double)stealth < 0.03)
                {
                    stealth = 0.03f;
                }
                float num3 = (1f + stealth * 10f) / 11f;
                drawinfo.itemColor = new Color((byte)(drawinfo.itemColor.R * stealth), (byte)(drawinfo.itemColor.G * stealth), (byte)(drawinfo.itemColor.B * num3), (byte)(drawinfo.itemColor.A * stealth));
            }
            if (player.setVortex && heldItem.DamageType == DamageClass.Ranged)
            {
                drawinfo.itemColor = drawinfo.itemColor.MultiplyRGBA(new Color(Vector4.Lerp(Vector4.One, new Vector4(0f, 0.12f, 0.16f, 0f), 1f - Math.Clamp(player.stealth, 0.03f, 1f))));
            }

            bool usingItem = player.itemAnimation > 0 && heldItem.useStyle != ItemUseStyleID.None;
            bool hasHoldStyle = heldItem.holdStyle != 0 && !player.pulley;
            if (!player.CanVisuallyHoldItem(heldItem))
                hasHoldStyle = false;

            if (drawinfo.shadow != 0f || player.frozen || !(usingItem || hasHoldStyle) || player.dead || heldItem.noUseGraphic || player.wet && heldItem.noWet && !ItemID.Sets.WaterTorches[itemType] || player.happyFunTorchTime && player.inventory[player.selectedItem].createTile == TileID.Torches && player.itemAnimation == 0)
                return;

            Vector2 vector = Vector2.Zero;
            Vector2 origin = new(itemDrawFrame.Width * 0.5f - itemDrawFrame.Width * 0.5f * player.direction, itemDrawFrame.Height);

            if (heldItem.useStyle == ItemUseStyleID.DrinkLiquid && player.itemAnimation > 0)
            {
                Vector2 vector2 = new(0.5f, 0.4f);
                origin = itemDrawFrame.Size() * vector2;
            }
            if (player.gravDir == -1f)
                origin.Y = itemDrawFrame.Height - origin.Y;

            origin += vector;
            float itemRotation = player.itemRotation;

            if (heldItem.useStyle == ItemUseStyleID.GolfPlay)
            {
                ref float x = ref position.X;
                float num6 = x;
                _ = player.direction;
                x = num6 - 0f;
                itemRotation -= (float)Math.PI / 2f * player.direction;
                origin.Y = 2f;
                origin.X += 2 * player.direction;
            }

            ItemSlot.GetItemLight(ref drawinfo.itemColor, heldItem);
            Color color = ItemSets.Glowmask[itemType].Item2;
            DrawData drawdata;

            if (heldItem.useStyle == ItemUseStyleID.Shoot)
            {
                if (Item.staff[itemType])
                {
                    float num9 = player.itemRotation + 0.785f * player.direction;
                    float num10 = 0f;
                    float num11 = 0f;
                    Vector2 origin5 = new(0f, itemDrawFrame.Height);
                    if (player.gravDir == -1f)
                    {
                        if (player.direction == -1)
                        {
                            num9 += 1.57f;
                            origin5 = new Vector2(itemDrawFrame.Width, 0f);
                            num10 -= itemDrawFrame.Width;
                        }
                        else
                        {
                            num9 -= 1.57f;
                            origin5 = Vector2.Zero;
                        }
                    }
                    else if (player.direction == -1)
                    {
                        origin5 = new Vector2(itemDrawFrame.Width, itemDrawFrame.Height);
                        num10 -= itemDrawFrame.Width;
                    }
                    ItemLoader.HoldoutOrigin(player, ref origin5);
                    drawdata = new DrawData(tex, new Vector2((int)(drawinfo.ItemLocation.X - Main.screenPosition.X + origin5.X + num10), (int)(drawinfo.ItemLocation.Y - Main.screenPosition.Y + num11)), itemDrawFrame, color, num9, origin5, adjustedItemScale, drawinfo.itemEffect);
                    drawinfo.DrawDataCache.Add(drawdata);
                    return;
                }
                Vector2 vector9 = new(0f, itemDrawFrame.Height / 2);
                Vector2 vector10 = Main.DrawPlayerItemPos(player.gravDir, itemType);
                int num12 = (int)vector10.X;
                vector9.Y = vector10.Y;
                Vector2 origin7 = new(-num12, itemDrawFrame.Height / 2);
                if (player.direction == -1)
                {
                    origin7 = new Vector2(itemDrawFrame.Width + num12, itemDrawFrame.Height / 2);
                }
                drawdata = new DrawData(tex, new Vector2((int)(drawinfo.ItemLocation.X - Main.screenPosition.X + vector9.X), (int)(drawinfo.ItemLocation.Y - Main.screenPosition.Y + vector9.Y)), itemDrawFrame, color, player.itemRotation, origin7, adjustedItemScale, drawinfo.itemEffect);
                drawinfo.DrawDataCache.Add(drawdata);
                return;
            }
            if (player.gravDir == -1f)
            {
                drawdata = new DrawData(tex, position, itemDrawFrame, color, itemRotation, origin, adjustedItemScale, drawinfo.itemEffect);
                drawinfo.DrawDataCache.Add(drawdata);
                return;
            }
            drawdata = new DrawData(tex, position, itemDrawFrame, color, itemRotation, origin, adjustedItemScale, drawinfo.itemEffect);
            drawinfo.DrawDataCache.Add(drawdata);
        }
    }
}
