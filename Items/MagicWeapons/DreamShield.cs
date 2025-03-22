using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using BasicMod.Projectiles;

namespace BasicMod.Items.MagicWeapons
{
	public class DreamShield : ModItem
	{
		//public override string Texture => "Terraria/Item_" + ItemID.Minishark;
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Hold to summon a shield that can attack and reflect projectiles.");
		}

		public override void SetDefaults()
		{
			item.damage = 20;
			item.noMelee = true;
			item.magic = true;
			item.channel = true; //Channel so that you can hold the weapon [Important]
			item.mana = 3;
			item.rare = ItemRarityID.Pink;
			item.width = 28;
			item.height = 30;
			item.useTime = 20;
			item.UseSound = SoundID.Item13;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.knockBack = 6f;
			item.shootSpeed = 14f;
			item.useAnimation = 20;
			item.shoot = ModContent.ProjectileType<MagicShield>();
			item.value = Item.sellPrice(silver: 3);
		}

		public override void AddRecipes()
		{
			/**
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(mod, "ExampleItem", 10);
			recipe.AddTile(mod, "ExampleWorkbench");
			recipe.SetResult(this);
			recipe.AddRecipe();
			**/
		}
	}
}