using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace BasicMod
{
	public class ModRecipies : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Recipe Test Item");
			Tooltip.SetDefault("You need help with creating this!");
		}

		public override string Texture => "Terraria/Item_" + ItemID.ChlorophyteShotbow;

		public override void SetDefaults()
		{
			item.width = 26;
			item.height = 26;
			item.rare = ItemRarityID.Blue;
		}

		//Using our custom recipe type
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Wood, 8);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(ItemID.Blowpipe);
			recipe.AddRecipe();
		}
	}
}
