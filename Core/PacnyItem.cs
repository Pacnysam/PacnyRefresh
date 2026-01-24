using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Terraria.Localization;

namespace PacnyRefresh.Core
{
    public class PacnyItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public float critDamageMod = 0f;

        public float materialMult = 0;
        public float rottime = 0;

        public override void ModifyWeaponCrit(Item item, Player player, ref float crit)
        {
            float updatedCritMod = (2 + item.GetGlobalItem<PacnyItem>().critDamageMod) * Main.LocalPlayer.GetModPlayer<PacnyPlayer>().critDamageMult;
            if (updatedCritMod <= 1 && Helper.IsWeapon(item))
                crit *= 0;
        }

        /*public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            float updatedCritMod = (2 + item.GetGlobalItem<PacnyItem>().critDamageMod) * Main.LocalPlayer.GetModPlayer<PacnyPlayer>().critDamageMult;

            if (updatedCritMod != 2 && updatedCritMod > 1 && Helper.IsWeapon(item))
            {
                TooltipLine critLine = tooltips.Find(n => n.Name == "CritChance");
                TooltipLine damageLine = tooltips.Find(n => n.Name == "Damage");
                int index = tooltips.IndexOf(critLine);

                if (critLine == null)
                    index = tooltips.IndexOf(damageLine);

                if (critLine != null || damageLine != null)
                    tooltips.Insert(index + 1, new TooltipLine(Mod, "CritMult", Language.GetTextValue("Mods.PacnyRefresh.Mechanics.CritMult", updatedCritMod)));
                //tooltips.Insert(index + 1, new TooltipLine(Mod, "CritMult", $"{updatedCritMod}x critical strike multiplier"));
            }
        }*/

        public override void SetDefaults(Item item)
        {
            switch (item.type)
            {
                case ItemID.JungleSpores:
                    {
                        item.width = 22;
                        item.height = 16;
                        break;
                    }
                case ItemID.CursedFlame:
                    {
                        item.width = 16;
                        item.height = 20;
                        break;
                    }
                case ItemID.Ichor:
                    {
                        item.width = 16;
                        item.height = 24;
                        break;
                    }
            }
        }

        public override void PostUpdate(Item item)
        {
            if (item.timeSinceItemSpawned % 180 == 160)
            {
                materialMult = 1f;
            }
            else 
            {
                if (materialMult > 0)
                    materialMult -= 0.01f;
            }
            materialMult = Math.Clamp(materialMult, 0f, 1f);

            rottime += (float)Math.PI / 60;
            if (rottime >= Math.PI * 2) rottime = 0;

            if (!Main.dedServ)
            {
                switch (item.type)
                {
                    case ItemID.CursedFlame:
                        {
                            if (Main.rand.NextBool((int)(materialMult * 15), 100))
                            {
                                Dust dust = Dust.NewDustDirect(item.position, item.width, item.height, DustID.FireworkFountain_Green, 0, 0, 0, default, Main.rand.NextFloat(0.45f, 0.65f));
                                dust.position += dust.velocity * 0.85f;
                                dust.velocity *= 0.85f;
                                dust.noGravity = true;
                            }
                            break;
                        }
                    case ItemID.Ichor:
                        {
                            if (Main.rand.NextBool((int)(materialMult * 20), 100))
                            {
                                Dust dust = Dust.NewDustDirect(item.Left, item.width, item.height / 2, DustID.Ichor, 0, Main.rand.NextFloat(-3f, -1.5f), 160, default, Main.rand.NextFloat(0.55f, 0.7f));
                                dust.position += dust.velocity * 1.15f;
                                dust.velocity *= 0.35f;
                                dust.fadeIn = 1.25f * materialMult;
                                dust.noGravity = true;
                                dust.noLight = true;
                                dust.noLightEmittence = true;
                            }
                            break;
                        }
                }
            }
        }

        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (item.type == ItemID.CursedFlame || item.type == ItemID.Ichor) 
            {
                Texture2D tex = TextureAssets.Item[item.type].Value;
                spriteBatch.Draw(tex, position, frame, Color.White, 0, origin, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(tex, position, frame, ColorHelper.AdditiveWhite() * 0.3f, 0, origin, scale, SpriteEffects.None, 0f);
                return false;
            }
            return base.PreDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }

        public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            if (item.type == ItemID.CursedFlame || item.type == ItemID.Ichor)
            {
                float rottime = Main.item[whoAmI].GetGlobalItem<PacnyItem>().rottime * 1.75f;
                float multiplier = Main.item[whoAmI].GetGlobalItem<PacnyItem>().materialMult * 0.5f;
                Vector2 squash = new((float)Math.Sin(rottime) * multiplier, (float)Math.Cos(rottime) * multiplier);

                Texture2D tex = TextureAssets.Item[item.type].Value;
                spriteBatch.Draw(tex, item.Center - Main.screenPosition, null, lightColor, rotation, tex.Size()/2, scale * (Vector2.One + squash), SpriteEffects.None, 0f);
                spriteBatch.Draw(tex, item.Center - Main.screenPosition, null, ColorHelper.AdditiveWhite() * 0.3f, rotation, tex.Size() / 2, scale * (Vector2.One + squash), SpriteEffects.None, 0f);
                return false;
            }
            return base.PreDrawInWorld(item, spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
        }
    }
}
