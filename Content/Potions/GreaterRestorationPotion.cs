using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using PacnyRefresh.Content.Underground.Items.GemStaves;
using PacnyRefresh.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static PacnyRefresh.Core.Helper;
using static Terraria.ModLoader.ModContent;

namespace PacnyRefresh.Content.Potions
{
    public class GreaterRestorationPotion : ModItem
    {
        const int GreaterRestoreAmount = 225;
        public override bool IsLoadingEnabled(Mod mod) => false;
        public override void SetStaticDefaults() 
        { 
            Item.ResearchUnlockCount = 30;
        }
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.RestorationPotion);
            
            Item.healLife = GreaterRestoreAmount;
            Item.rare = ItemRarityID.Orange;

            Item.consumable = true;
            Item.value = Item.sellPrice(silver: 12, copper: 50);

            Item.width = 20;
            Item.height = 28;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(3)
                .AddIngredient(ItemID.RestorationPotion, 3)
                .AddIngredient(ItemID.Ectoplasm)
                .AddTile(TileID.Bottles)
                .Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach (TooltipLine line in tooltips.Where(x => x.Mod == "Terraria" && x.Name == "HealLife"))
                line.Text = Language.GetTextValue("Mods.PacnyRefresh.VanillaItemTooltips.RestorationPotion", Main.LocalPlayer.GetHealLife(Item) - (Main.LocalPlayer.GetHealLife(Item) % 5));
        }

        public override void UpdateInventory(Player player) => Item.healLife = GreaterRestoreAmount;
        public override bool CanUseItem(Player player) => player.FindBuffIndex(BuffID.PotionSickness) == -1;
        public override bool? UseItem(Player player)
        {
            int heal = player.GetHealLife(Item) - (player.GetHealLife(Item) % 5);
            Item.healLife = 0;

            player.AddBuff(BuffID.PotionSickness, (int)player.PotionDelayModifier.ApplyTo(Helper.TimeToTicks(60)));
            player.AddBuff(BuffType<RestorationBuff>(), Helper.TimeToTicks(heal / 10f));
            return true;
        }
    }
}
