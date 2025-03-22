using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BasicMod.Items.RangedWeapons.Bows
{

	class DeciduousBow : ModItem
	{
		public override string Texture => "Terraria/Item_" + ItemID.WoodenBow;
		public override void SetStaticDefaults()
		{

			Tooltip.SetDefault("Fires a spread of leaves.");
		}
		public override void SetDefaults()
		{
			item.damage = 43;
			item.ranged = true;
			item.width = 38;
			item.height = 6;

			item.useTime = 15;
			item.useAnimation = 15;

			item.useStyle = ItemUseStyleID.HoldingOut; // how you use the item (swinging, holding out, etc)
			item.noMelee = true; //so the item's animation doesn't do damage
			item.knockBack = 3; // Sets the item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
			item.value = 90000; // how much the item sells for (measured in copper)
			item.rare = ItemRarityID.LightRed;// the color that the item's name will be in-game
			item.UseSound = SoundID.Item5; // USE FOR BOWS
			item.autoReuse = true; // if you can hold click to automatically use it again
			item.shootSpeed = 10f; // the speed of the projectile (measured in pixels per frame)
			item.useAmmo = AmmoID.Arrow; // The "ammo Id" of the ammo item that this weapon uses. Note that this is not an item Id, but just a magic value.
			item.shoot = 1;
		}

		public override void AddRecipes()
		{
			// have Dryad sell it
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			float numberProjectiles = 4;
			float rotation = MathHelper.ToRadians(45);
			position += Vector2.Normalize(new Vector2(speedX, speedY)) * 45f;
			for (int i = 0; i < numberProjectiles; i++)
			{
				Vector2 perturbedSpeed = Vector2.Normalize(new Vector2(speedX, speedY)).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))); // Watch out for dividing by 0 if there is only 1 projectile.
				Projectile.NewProjectile(position, perturbedSpeed * 10f, mod.ProjectileType("DeciduousBowLeaf"), damage, knockBack, player.whoAmI);
			}
			return true;
		}
	}
	class DeciduousBowLeaf : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[projectile.type] = 5;
		}
		public override void SetDefaults()
		{
			projectile.width = 10;
			projectile.height = 10;
			projectile.friendly = true;
			projectile.ranged = true;
			projectile.alpha = 10;
			projectile.light = 0.75f;
			projectile.tileCollide = true;
			projectile.penetrate = -1;
			projectile.aiStyle = 0;
			projectile.timeLeft = 300;

		}

        public override void PostAI()
        {
			if (++projectile.frameCounter >= 5)
			{
				projectile.frameCounter = 0;
				if (++projectile.frame >= 5)
				{
					projectile.frame = 0;
				}
			}
			projectile.rotation = projectile.velocity.ToRotation();
		}

		public override void Kill(int timeLeft)
		{
			// This code and the similar code above in OnTileCollide spawn dust from the tiles collided with. SoundID.Item10 is the bounce sound you hear.
			Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
		}

	}
}
