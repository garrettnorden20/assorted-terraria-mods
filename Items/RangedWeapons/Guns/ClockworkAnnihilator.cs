using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BasicMod.Items.RangedWeapons.Guns
{
	public class ClockworkAnnihilator : ModItem
	{
		public override string Texture => "Terraria/Item_" + ItemID.ClockworkAssaultRifle;
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Kills like clockwork.");
		}

		public override void SetDefaults()
		{
			item.damage = 35; // Sets the item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
			item.ranged = true; // sets the damage type to ranged
			item.width = 38; // hitbox width of the item
			item.height = 6; // hitbox height of the item

			item.useTime = 3; // The item's use time in ticks (60 ticks == 1 second.)
			item.useAnimation = 15; // The length of the item's use animation in ticks (60 ticks == 1 second.)
			item.reuseDelay = 18; // time between bursts

			item.useStyle = ItemUseStyleID.HoldingOut; // how you use the item (swinging, holding out, etc)
			item.noMelee = true; //so the item's animation doesn't do damage
			item.knockBack = 0; // Sets the item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
			item.value = 90000; // how much the item sells for (measured in copper)
			item.rare = ItemRarityID.LightRed;// the color that the item's name will be in-game
			item.UseSound = SoundID.Item31; // The sound that this item plays when used.
			item.autoReuse = true; // if you can hold click to automatically use it again
			item.shootSpeed = 10f; // the speed of the projectile (measured in pixels per frame)
			item.useAmmo = AmmoID.Bullet; // The "ammo Id" of the ammo item that this weapon uses. Note that this is not an item Id, but just a magic value.
			item.shoot = 10;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.ClockworkAssaultRifle, 1);
			recipe.AddIngredient(ItemID.Cog, 12);
			recipe.AddIngredient(ItemID.SoulofFright, 3);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			float radians = MathHelper.ToRadians(20);
			Vector2 velocity = new Vector2(speedX, speedY);

			Projectile.NewProjectile(position, (velocity.RotatedBy(-1 * radians)).RotatedByRandom(MathHelper.ToRadians(5)), type, damage, knockBack, player.whoAmI);
			Projectile.NewProjectile(position, (velocity.RotatedBy(radians)).RotatedByRandom(MathHelper.ToRadians(5)), type, damage, knockBack, player.whoAmI);
			Projectile.NewProjectile(position, velocity.RotatedByRandom(MathHelper.ToRadians(5)), type, damage, knockBack, player.whoAmI);

			return false;
		}

		public override bool ConsumeAmmo(Player player)
		{
			// Because of how the game works, player.itemAnimation will be 11, 7, and finally 3. (useAnimation - 1, then - useTime until less than 0.) 
			// We can get the Clockwork Assault Riffle Effect by not consuming ammo when itemAnimation is lower than the first shot.
			return !(player.itemAnimation < item.useAnimation - 2);
		}
	}
}
