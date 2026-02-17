using Microsoft.Xna.Framework;
using PacnyRefresh.Core;
using PacnyRefresh.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace PacnyRefresh.Content.Gores
{
    public class StarGore : ModGore
    {
        public override void OnSpawn(Gore gore, IEntitySource source)
        {
            gore.position -= new Vector2(22, 23) * gore.scale;
            gore.numFrames = 3;
            gore.behindTiles = false;
            gore.timeLeft = 30;
            gore.scale *= 0.1f;
            ChildSafety.SafeGore[gore.type] = true;
        }

        public override Color? GetAlpha(Gore gore, Color lightColor)
        {
            Color color1 = new(255, 241, 51);
            Color color2 = new(81, 166, 243);

            Color color = Color.Lerp(color1, color2, Math.Clamp((30 - gore.timeLeft) / 25f, 0f, 1f)).MultiplyAlpha(gore.alpha / 255f - 0.3f);
            return color * gore.Opacity();
        }

        public override bool Update(Gore gore)
        {
            gore.velocity *= 0.95f;
            gore.position += gore.velocity;
            gore.position.Y += 0.035f * (MathHelper.TwoPi * gore.velocity.Length()) * -(3 - gore.frame);

            gore.alpha += 4;

            if (--gore.timeLeft < 10)
            {
                gore.scale = (gore.scale - 1) / 3f + 1;
            }
            else
            {
                gore.scale = MathHelper.Lerp(gore.scale, 1, 0.075f);
            }

            gore.rotation += MathHelper.TwoPi * gore.velocity.Length() * 0.05f * Math.Sign(gore.velocity.X + 0.0001f);//MathHelper.Lerp(gore.rotation, gore.rotation * 0.45f, 0.075f);

            if (gore.alpha > 245)
                gore.active = false;
            return false;
        }
    }
}
