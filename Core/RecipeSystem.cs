using System;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using static Terraria.ModLoader.ModContent;
using static PacnyRefresh.Core.Helper;

namespace PacnyRefresh.Core
{
    public class RecipeSystem : ModSystem
    {
        public override void PreWorldGen()
        {
            learnedRecipes.Clear();
        }

        public override void PostAddRecipes()
        {
            for (int i = 0; i < Recipe.numRecipes; i++)
            {
                Recipe recipe = Main.recipe[i];
                
                if (recipe.createItem.dye > 0 || recipe.createItem.hairDye > -1)
                    recipe.AddOnCraftCallback(RecipeCallbacks.DyeMinor);

                if (recipe.TryGetIngredient(ItemID.FallenStar, out Item star))
                {
                    recipe.AddOnCraftCallback(RecipeCallbacks.Star);
                }
                if (recipe.HasTile(TileID.Anvils))
                {
                    recipe.AddOnCraftCallback(RecipeCallbacks.Anvil);
                }
                if (recipe.HasTile(TileID.Solidifier) || recipe.TryGetIngredient(ItemID.Gel, out Item gel) && (!recipe.HasRecipeGroup(RecipeGroup.recipeGroupIDs["Wood"]) || !recipe.HasIngredient(ItemID.Torch)))
                {
                    recipe.AddOnCraftCallback(RecipeCallbacks.Slime);
                }
            }       
        }

        public override void AddRecipeGroups()
        {
            RecipeGroup BaseGroup(object GroupName, int[] Items, bool Prefix = true) //yoinked from spirit
            {
                string Name = Prefix? Language.GetTextValue("LegacyMisc.37") + " " : "";

                Name += GroupName switch
                {
                    //modcontent items
                    int i => Lang.GetItemNameValue((int)GroupName),
                    //vanilla item ids
                    short s => Lang.GetItemNameValue((short)GroupName),
                    //custom group names
                    _ => GroupName.ToString(),
                };
                return new RecipeGroup(() => Name, Items);
            }
            void RegisterVarietyGroup(string name, int[] items, bool prefix = false)
            {
                string groupName = "";

                if (items.Length < 2)
                    return;
                for (int i = 0; i < items.Length - 1; i++)
                {
                    ItemID.Search.TryGetName(items[i], out string itemName);
                    ModItem currentItem = GetModItem(items[i]);
                    string localizedName = currentItem != null ? currentItem.DisplayName.Value : Language.GetTextValue("ItemName." + itemName);
                    if (i + 2 == items.Length)
                    {
                        ItemID.Search.TryGetName(items[i + 1], out string nextItemName);
                        ModItem nextItem = GetModItem(items[i + 1]);
                        string nextLocalizedName = nextItem != null ? nextItem.DisplayName.Value : Language.GetTextValue("ItemName." + nextItemName);

                        groupName += Language.GetTextValue("Mods.PacnyRefresh.RecipeGroups.Or", localizedName, nextLocalizedName);
                        break;
                    }
                    groupName += localizedName + ", ";
                }
                RecipeGroup.RegisterGroup(name, BaseGroup(groupName, items, prefix));
            }

            #region ores
            RegisterVarietyGroup("PacnyRefresh:CopperOre", [ItemID.CopperOre, ItemID.TinOre]);
            RegisterVarietyGroup("PacnyRefresh:IronOre", [ItemID.SilverOre, ItemID.TungstenOre]);
            RegisterVarietyGroup("PacnyRefresh:SilverOre", [ItemID.SilverOre, ItemID.TungstenOre]);
            RegisterVarietyGroup("PacnyRefresh:GoldOre", [ItemID.GoldOre, ItemID.PlatinumOre]);

            RegisterVarietyGroup("PacnyRefresh:EvilOre", [ItemID.DemoniteOre, ItemID.CrimtaneOre]);

            RegisterVarietyGroup("PacnyRefresh:CobaltOre", [ItemID.CobaltOre, ItemID.PalladiumOre]);
            RegisterVarietyGroup("PacnyRefresh:MythrilOre", [ItemID.MythrilOre, ItemID.OrichalcumOre]);
            RegisterVarietyGroup("PacnyRefresh:AdamantiteOre", [ItemID.AdamantiteOre, ItemID.TitaniumOre]);
            #endregion ores

            #region bars
            RegisterVarietyGroup("PacnyRefresh:CopperBar", [ItemID.CopperBar, ItemID.TinBar]);
            RegisterVarietyGroup("PacnyRefresh:SilverBar", [ItemID.SilverBar, ItemID.TungstenBar]);
            RegisterVarietyGroup("PacnyRefresh:GoldBar", [ItemID.GoldBar, ItemID.PlatinumBar]);

            RegisterVarietyGroup("PacnyRefresh:EvilBar", [ItemID.DemoniteBar, ItemID.CrimtaneBar]);

            RegisterVarietyGroup("PacnyRefresh:CobaltBar", [ItemID.CobaltBar, ItemID.PalladiumBar]);
            RegisterVarietyGroup("PacnyRefresh:MythrilBar", [ItemID.MythrilBar, ItemID.OrichalcumBar]);
            RegisterVarietyGroup("PacnyRefresh:AdamantiteBar", [ItemID.AdamantiteBar, ItemID.TitaniumBar]);
            #endregion bars

            RegisterVarietyGroup("PacnyRefresh:JellyfishBait", [ItemID.PinkJellyfish, ItemID.BlueJellyfish, ItemID.GreenJellyfish]);
            RegisterVarietyGroup("PacnyRefresh:EvilYoyo", [ItemID.CorruptYoyo, ItemID.CrimsonYoyo]);

            RegisterVarietyGroup("PacnyRefresh:EvilMushroom", [ItemID.VileMushroom, ItemID.ViciousMushroom]);
            RegisterVarietyGroup("PacnyRefresh:EvilMaterial", [ItemID.RottenChunk, ItemID.Vertebrae]);
            RegisterVarietyGroup("PacnyRefresh:EvilBossMaterial", [ItemID.ShadowScale, ItemID.TissueSample]);
        }

        

        public static List<int> learnedRecipes = [];

        public static void LearnRecipie(Item item)
        {
            if (!learnedRecipes.Contains(item.type))
            {
                learnedRecipes.Add(item.type);
            }
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["learnedRecipies"] = learnedRecipes;
        }
        public override void LoadWorldData(TagCompound tag)
        {
            learnedRecipes = (List<int>)tag.GetList<int>("learnedRecipies");
        }
    }
}
