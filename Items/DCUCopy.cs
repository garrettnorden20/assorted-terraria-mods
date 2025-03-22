using BasicMod.Mounts;
using Terraria.ID;
using Terraria.ModLoader;

namespace BasicMod.Items
{
	public class DCUCopy : ModItem
	{
		public override string Texture => "Terraria/Item_" + ItemID.DrillContainmentUnit;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Drill Containment Unit 2.0");
			Tooltip.SetDefault("Summons a flying drill mount. Can destroy walls.");
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 30;
			item.useTime = 20;
			item.useAnimation = 20;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.value = 30000;
			item.rare = ItemRarityID.Yellow;
			item.UseSound = SoundID.Item79;
			item.noMelee = true;
			item.mountType = ModContent.MountType<DrillContain>();
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.DirtBlock, 1);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}