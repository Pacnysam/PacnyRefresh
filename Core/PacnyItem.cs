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

namespace PacnyRefresh.Core
{
    public class PacnyItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public float materialMult = 0;
        public float rottime = 0;

        public override void SetDefaults(Item item)
        {
            switch (item.type)
            { 
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
            if (item.timeSinceItemSpawned % 180 == 120)
            {
                materialMult = 1f;
            }

            if (materialMult > 0)
                materialMult -= 0.01f;
            if (materialMult < 0)
                materialMult = 0;

            rottime += (float)Math.PI / 60;
            if (rottime >= Math.PI * 2) rottime = 0;
        }

        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (item.type == ItemID.CursedFlame || item.type == ItemID.Ichor) 
            {
                Texture2D tex = TextureAssets.Item[item.type].Value;
                spriteBatch.Draw(tex, position, frame, Color.White, 0, origin, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(tex, position, frame, ColorHelper.AdditiveWhite * 0.3f, 0, origin, scale, SpriteEffects.None, 0f);
                return false;
            }
            return base.PreDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }

        public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            if (item.type == ItemID.CursedFlame || item.type == ItemID.Ichor)
            {
                Texture2D tex = TextureAssets.Item[item.type].Value;
                spriteBatch.Draw(tex, item.Center - Main.screenPosition, null, lightColor, rotation + ((float)Math.Sin(Main.item[whoAmI].GetGlobalItem<PacnyItem>().rottime * 3f) * (materialMult * 0.25f)), tex.Size()/2, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(tex, item.Center - Main.screenPosition, null, ColorHelper.AdditiveWhite * 0.3f, rotation + ((float)Math.Sin(Main.item[whoAmI].GetGlobalItem<PacnyItem>().rottime * 3f) * (materialMult * 0.25f)), tex.Size() / 2, scale, SpriteEffects.None, 0f);
                return false;
            }
            return base.PreDrawInWorld(item, spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
        }
    }
}
