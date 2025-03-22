using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BasicMod.Items.RangedWeapons.DartWeapons
{
	public class MultiDart : ModItem
	{
		public override string Texture => "Terraria/Item_" + ItemID.ChlorophyteShotbow;
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Shoots multiple darts at once.");
		}

		public override void SetDefaults()
		{
			item.damage = 40; // Sets the item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
			item.ranged = true; // sets the damage type to ranged
			item.width = 38; // hitbox width of the item
			item.height = 6; // hitbox height of the item
			item.useTime = 28; // The item's use time in ticks (60 ticks == 1 second.)
			item.useAnimation = 28; // The length of the item's use animation in ticks (60 ticks == 1 second.)
			item.useStyle = ItemUseStyleID.HoldingOut; // how you use the item (swinging, holding out, etc)
			item.noMelee = true; //so the item's animation doesn't do damage
			item.knockBack = 4; // Sets the item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
			item.value = 20000; // how much the item sells for (measured in copper)
			item.rare = ItemRarityID.Green;// the color that the item's name will be in-game
			item.UseSound = SoundID.Item36; // The sound that this item plays when used.
			item.autoReuse = false; // if you can hold click to automatically use it again
			item.shootSpeed = 9f; // the speed of the projectile (measured in pixels per frame)
			item.useAmmo = AmmoID.Dart; // The "ammo Id" of the ammo item that this weapon uses. Note that this is not an item Id, but just a magic value.
			item.shoot = 10;
		}

		public override void AddRecipes()
		{

		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			//Main.NewText("bruv");
			int numberProjectiles = 2 + Main.rand.Next(1); // 2 or 3 shots
			for (int i = 0; i < numberProjectiles; i++)
			{
				Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(9)); // 30 degree spread.
																												// If you want to randomize the speed to stagger the projectiles
																												// float scale = 1f - (Main.rand.NextFloat() * .3f);
																												// perturbedSpeed = perturbedSpeed * scale; 
				Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, knockBack, player.whoAmI);
			}
			return true; // return false because we don't want tmodloader to shoot projectile
		}
	}
}
