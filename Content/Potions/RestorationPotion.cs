using Microsoft.Xna.Framework.Graphics;
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
using static Terraria.ModLoader.ModContent;
using static PacnyRefresh.Core.Helper;

namespace PacnyRefresh.Content.Potions
{
    public class RestorePotionSystem : ModSystem
    {
        public override void PostAddRecipes()
        {
            for (int i = 0; i < Recipe.numRecipes; i++)
            {
                Recipe recipe = Main.recipe[i];

                if (recipe.HasIngredient(ItemID.Mushroom) && recipe.HasIngredient(ItemID.GlowingMushroom) && recipe.HasIngredient(ItemID.PinkGel) && recipe.HasIngredient(ItemID.Bottle) && recipe.HasResult(ItemID.RestorationPotion))
                {
                    recipe.DisableRecipe();
                }
            }
        }
    }
    public class RestorationPotionItem : GlobalItem
    {
        const int LesserRestoreAmount = 75;
        const int RestoreAmount = 150;

        public override void Load() => On_Item.SetDefaults1 += LesserRestorationUnremover;
        public override void Unload() => On_Item.SetDefaults1 -= LesserRestorationUnremover;
        private void LesserRestorationUnremover(On_Item.orig_SetDefaults1 orig, Item self, int type)
        {
            if (type == ItemID.LesserRestorationPotion)
            {
                self.type = ItemID.LesserRestorationPotion;
                self.UseSound = SoundID.Item3;
                self.healLife = LesserRestoreAmount;
                self.useStyle = ItemUseStyleID.DrinkLiquid;
                self.useTurn = true;
                self.useAnimation = 17;
                self.useTime = 17;
                self.maxStack = Item.CommonMaxStack;
                self.consumable = true;
                self.width = 14;
                self.height = 24;
                self.potion = true;
                self.value = 1500;
                self.rare = ItemRarityID.Blue;
            }
            else
                orig(self, type);
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(item, tooltips);
            
            foreach (TooltipLine line in tooltips.Where(x => x.Mod == "Terraria" && x.Name == "HealLife"))
                line.Text = Language.GetTextValue("Mods.PacnyRefresh.VanillaItemTooltips.RestorationPotion", item.healLife);

            if (item.type == ItemID.RestorationPotion)
            {
                foreach (TooltipLine line in tooltips.Where(x => x.Mod == "Terraria" && x.Name == "Tooltip0"))
                    line.Hide();
            }
        }
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.LesserRestorationPotion || entity.type == ItemID.RestorationPotion;
        public override void SetDefaults(Item entity)
        {
            base.SetDefaults(entity);
            
            if (entity.type == ItemID.RestorationPotion)
                entity.healLife = RestoreAmount;
        }
        public override void UpdateInventory(Item item, Player player)
        {
            switch(item.type)
            {
                case ItemID.LesserRestorationPotion:
                    item.healLife = LesserRestoreAmount;
                    break;
                case ItemID.RestorationPotion:
                    item.healLife = RestoreAmount;
                    break;
                }
        }
        public override bool CanUseItem(Item item, Player player)
        {
            return item.type switch
            {
                ItemID.LesserRestorationPotion or ItemID.RestorationPotion => player.FindBuffIndex(BuffID.PotionSickness) == -1,
                _ => base.CanUseItem(item, player),
            };
        }
        public override bool? UseItem(Item item, Player player)
        {
            switch (item.type)
            {
                case ItemID.LesserRestorationPotion:
                case ItemID.RestorationPotion:
                    int heal = item.healLife;
                    item.healLife = 0;
                    player.AddBuff(BuffID.PotionSickness, (int)player.PotionDelayModifier.ApplyTo(Helper.TimeToTicks(60)));
                    player.AddBuff(BuffType<RestorationBuff>(), Helper.TimeToTicks(heal/10f));
                    break;
            }
            return base.UseItem(item, player);
        }
        public override void AddRecipes()
        {
            Recipe.Create(ItemID.LesserRestorationPotion, 2)
                .AddRecipeGroup("PacnyRefresh:EvilMushroom")
                .AddIngredient(ItemID.PinkGel, 1)
                .AddIngredient(ItemID.Bottle, 2)
                .AddTile(TileID.Bottles)
                .Register();
            Recipe.Create(ItemID.RestorationPotion, 2)
                .AddIngredient(ItemID.LesserRestorationPotion, 2)
                .AddIngredient(ItemID.PixieDust, 1)
                .AddIngredient(ItemID.CrystalShard, 1)
                .AddTile(TileID.Bottles)
                .Register();
        }
    }

    public class RestorationBuff : ModBuff
    {
        public override string Texture => CoolBuffTex(base.Texture);
        public override void Update(Player player, ref int buffIndex)
        {
            if (player.buffTime[buffIndex] % 30 == 0)
                player.Heal(5);
        }
    }
}
