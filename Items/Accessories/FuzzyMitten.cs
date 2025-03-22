using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;

namespace BasicMod.Items.Accessories
{
	public class FuzzyMitten : ModItem
	{
		public override string Texture => "Terraria/Item_" + ItemID.TitanGlove;
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("A pair of warm mittens!" +
				"\nAllows you to throw snowballs with each ranged attack.");
		}

		public override void SetDefaults()
		{
			item.width = 24;
			item.height = 28;
			item.value = 100;
			item.rare = ItemRarityID.Blue;
			item.accessory = true;
			item.defense = 1;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<BasicModPlayer>().fuzzyMittens = true;
		}

		/**
		public override void AddRecipes()
		{
		}
		**/
	}
}