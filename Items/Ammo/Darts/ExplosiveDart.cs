using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BasicMod.Items.Ammo.Darts
{
	public class ExplosiveDart : ModItem
	{
		public override string Texture => "Terraria/Item_" + ItemID.PoisonDart;
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Pierces enemies");
		}

		public override void SetDefaults()
		{
			item.damage = 10;
			item.ranged = true;
			item.width = 8;
			item.height = 8;
			item.maxStack = 999;
			item.consumable = true;
			item.knockBack = 2f;
			item.value = 10;
			item.rare = ItemRarityID.Green;
			item.shoot = mod.ProjectileType("ExplosiveDartProjectile");
			item.shootSpeed = 3f;
			item.ammo = AmmoID.Dart;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(mod.ItemType("Dart"), 150);
			recipe.AddIngredient(ItemID.Stinger, 150);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this, 150);
			recipe.AddRecipe();
		}
	}

	public class ExplosiveDartProjectile : ModProjectile
	{
		public override string Texture => "Terraria/Projectile_" + ProjectileID.PoisonDartBlowgun;

		public override void SetDefaults()
		{
			projectile.width = 14;
			projectile.height = 14;
			projectile.aiStyle = 1;
			projectile.friendly = true;
			projectile.hostile = false;
			projectile.ranged = true;
			projectile.timeLeft = 600;
			projectile.alpha = 255;
			projectile.light = 0.5f;
			projectile.tileCollide = true;
			aiType = ProjectileID.PoisonDartBlowgun;
		}

		public override void PostAI()
		{
			int num = Main.rand.Next(2);
			for (int i = 0; i < num; i++) // x-(y-1) golden dusts every frame
			{
				Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Smoke, 0, 0, 0, default, 0.5f);

			}
		}

		public override void Kill(int timeLeft)
		{
			if (timeLeft < 1)
            {
				return;
            }
			Player player = Main.player[projectile.owner];
			projectile.width = 50;
			projectile.height = 50;
			int numDust = 33;

			projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
			projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
			Projectile.NewProjectile(projectile.Center, new Vector2(0, 0), mod.ProjectileType("DartExplosion"), projectile.damage, projectile.knockBack, player.whoAmI);
			Main.PlaySound(SoundID.Item62, projectile.position);
			for (int i = 0; i < numDust; i++)
			{
				int dustIndex = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 31, 0f, 0f, 100, default(Color), 2f);
				Main.dust[dustIndex].velocity *= 1.4f;
			}
			// Fire Dust spawn
			for (int i = 0; i < numDust * 1.5; i++)
			{
				int dustIndex = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 6, 0f, 0f, 100, default(Color), 3f);
				Main.dust[dustIndex].noGravity = true;
				Main.dust[dustIndex].velocity *= 5f;
				dustIndex = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 6, 0f, 0f, 100, default(Color), 2f);
				Main.dust[dustIndex].velocity *= 3f;
			}
		}
	}

	public class DartExplosion : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dart Explosion");
		}

		public override void SetDefaults()
		{
			projectile.aiStyle = 1;
			aiType = ProjectileID.Bullet;
			projectile.width = 50;
			projectile.height = 50;
			projectile.friendly = true;
			projectile.hostile = false;
			projectile.penetrate = -1;

			projectile.tileCollide = false;
			projectile.timeLeft = 2;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			return false;
		}
	}
}
