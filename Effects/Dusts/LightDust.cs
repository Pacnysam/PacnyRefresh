using Microsoft.Xna.Framework;
using static Terraria.ModLoader.ModContent;
using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using PacnyRefresh.Core;
using ReLogic.Content;

namespace PacnyRefresh.Effects.Dusts
{
    public class LightDust : ModDust
    {
        private static Asset<Texture2D> tex;
        public override void Load()
        {
            tex = Request<Texture2D>(Texture);
        }

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.frame = new Rectangle(0, 0, 14, 14);
            dust.scale *= 0.35f;
            dust.alpha -= 15;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.velocity *= 0.95f;


            if (dust.alpha > 1) dust.alpha *= (int)1.03f;

            if (dust.alpha < 1)
            {
                //dust.scale += 0.045f;
                dust.alpha += 1;
                dust.scale *= 1.05f;
            }
            else
            {
                //dust.scale -= 0.025f;
                dust.scale *= 0.94f;
                dust.alpha += 7;
            }

            if (!dust.noLight)
                Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.3f);

            if (dust.alpha > 250)
            {
                dust.active = false;
            }
            return false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return Color.White * ((255 - dust.alpha) / 255f);
        }

        public override bool PreDraw(Dust dust)
        {
            Color color = dust.color;
            int brightness = (color.R + color.G + color.B) / 3;
            Color coreColor = new(color.R + brightness * 1.4f, color.G + brightness * 1.4f, color.B + brightness * 1.4f);
            color.A = 0; coreColor.A = 0;

            Main.spriteBatch.Draw(tex.Value, new Vector2(dust.position.X, dust.position.Y) - Main.screenPosition, dust.frame, Color.Black * ((175 - dust.alpha) / 255f) * 0.2f, dust.rotation, new Vector2(7, 7), dust.scale * 1.6f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex.Value, new Vector2(dust.position.X, dust.position.Y) - Main.screenPosition, dust.frame, color * ((100 - dust.alpha) / 255f) * 0.5f, dust.rotation, new Vector2(7, 7), dust.scale * 1.35f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex.Value, dust.position - Main.screenPosition, dust.frame, color, dust.rotation, new Vector2(7, 7), dust.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex.Value, dust.position - Main.screenPosition, dust.frame, coreColor * (1 - brightness / 255) /** ((225 - dust.alpha) / 255f) * 0.5f*/, dust.rotation, new Vector2(7, 7), dust.scale * 0.55f, SpriteEffects.None, 0);

            //Main.spriteBatch.Draw(Request<Texture2D>("PacnyRefresh/Effects/Dusts/LightDust").Value, dust.position - Main.screenPosition, dust.frame, dust.color * dust.alpha, GoldLeafWorld.rottime * 2, new Vector2(0, 0), dust.scale * 1.5f, SpriteEffects.None, 0);
            //Main.spriteBatch.Draw(Request<Texture2D>("PacnyRefresh/Effects/Dusts/LightDust").Value, dust.position - Main.screenPosition, dust.frame, dust.color * dust.alpha, GoldLeafWorld.rottime * -2, new Vector2(0, 0), dust.scale * 1.5f, SpriteEffects.None, 0);
            return false;
        }
    }

    public class LightDustFast : ModDust
    {
        public override string Texture => "PacnyRefresh/Effects/Dusts/LightDust";

        private static Asset<Texture2D> tex;
        public override void Load()
        {
            tex = Request<Texture2D>(Texture);
        }

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.frame = new Rectangle(0, 0, 14, 14);
            dust.scale *= 0.35f;
            dust.alpha -= 18;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.velocity *= 0.9f;


            if (dust.alpha > 1) dust.alpha *= (int)1.03f;

            if (dust.alpha < 1)
            {
                //dust.scale += 0.045f;
                dust.alpha += 2;
                dust.scale *= 1.1f;
            }
            else
            {
                //dust.scale -= 0.025f;
                dust.scale *= 0.89f;
                dust.alpha += 14;
            }

            if (!dust.noLight)
                Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.3f);

            if (dust.alpha > 250)
            {
                dust.active = false;
            }
            return false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return Color.White * ((255 - dust.alpha) / 255f);
        }

        public override bool PreDraw(Dust dust)
        {
            Color color = dust.color;
            int brightness = (color.R + color.G + color.B) / 3;
            Color coreColor = new(color.R + brightness * 1.4f, color.G + brightness * 1.4f, color.B + brightness * 1.4f);
            color.A = 0; coreColor.A = 0;

            Main.spriteBatch.Draw(tex.Value, new Vector2(dust.position.X, dust.position.Y) - Main.screenPosition, dust.frame, Color.Black * ((175 - dust.alpha) / 255f) * 0.2f, dust.rotation, new Vector2(7, 7), dust.scale * 1.6f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex.Value, new Vector2(dust.position.X, dust.position.Y) - Main.screenPosition, dust.frame, color * ((100 - dust.alpha) / 255f) * 0.5f, dust.rotation, new Vector2(7, 7), dust.scale * 1.35f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex.Value, dust.position - Main.screenPosition, dust.frame, color, dust.rotation, new Vector2(7, 7), dust.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex.Value, dust.position - Main.screenPosition, dust.frame, coreColor * (1 - brightness / 255) /** ((225 - dust.alpha) / 255f) * 0.5f*/, dust.rotation, new Vector2(7, 7), dust.scale * 0.55f, SpriteEffects.None, 0);

            return false;
        }
    }
}
