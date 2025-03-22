using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BasicMod.Items.RangedWeapons.DartWeapons
{
	public class DartSniper : ModItem
	{
		public override string Texture => "Terraria/Item_" + ItemID.SniperRifle;
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Shoots an incredibly powerful dart with high crit.");
		}

		public override void SetDefaults()
		{
			item.damage = 140; // Sets the item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
			item.ranged = true; // sets the damage type to ranged
			item.width = 38; // hitbox width of the item
			item.height = 6; // hitbox height of the item
			item.useTime = 34; // The item's use time in ticks (60 ticks == 1 second.)
			item.useAnimation = 34; // The length of the item's use animation in ticks (60 ticks == 1 second.)
			item.useStyle = ItemUseStyleID.HoldingOut; // how you use the item (swinging, holding out, etc)
			item.noMelee = true; //so the item's animation doesn't do damage
			item.knockBack = 7; // Sets the item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
			item.value = 10000; // how much the item sells for (measured in copper)
			item.rare = ItemRarityID.Yellow;// the color that the item's name will be in-game
			item.UseSound = SoundID.Item99; // The sound that this item plays when used.
			item.autoReuse = false; // if you can hold click to automatically use it again
			item.shootSpeed = 25f; // the speed of the projectile (measured in pixels per frame)
			item.useAmmo = AmmoID.Dart; // The "ammo Id" of the ammo item that this weapon uses. Note that this is not an item Id, but just a magic value.
			item.shoot = 10;
			item.crit = 30;
		}

		public override void AddRecipes()
		{

		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			return true;
		}
	}

}
