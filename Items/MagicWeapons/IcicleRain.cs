using BasicMod.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BasicMod.Items.MagicWeapons
{
	public class IcicleRain : ModItem
	{
		public override string Texture => "Terraria/Item_" + ItemID.WandofSparking;
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Summon an icicle to fall on your foes.");
		}

		public override void SetDefaults()
		{
			item.damage = 20;
			item.noMelee = true;
			item.magic = true;
			item.mana = 5;
			item.rare = ItemRarityID.Blue;
			item.width = 28;
			item.height = 30;
			item.useTime = 25;
			item.useAnimation = 25;
			item.UseSound = SoundID.Item9;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.shootSpeed = 14f;
			item.shoot = ModContent.ProjectileType<Icicle>();
			item.value = Item.sellPrice(silver: 3);
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.IceBlock, 15);
			recipe.AddIngredient(ItemID.Shiverthorn, 1);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}


		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack) //This lets you modify the firing of the item
		{
			Vector2 mouseLocation = Main.MouseWorld;
			position = mouseLocation;
			speedX = 0;
			speedY = 0;


			return true;
		}

	}
}
