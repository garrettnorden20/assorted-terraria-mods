using System;
using BasicMod.Dusts;
using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BasicMod.Items.Ammo.Arrows
{
	public class BinaryArrow : ModItem
	{
		public override void SetDefaults()
		{
			item.damage = 12;
			item.ranged = true;
			item.width = 10;
			item.height = 28;
			item.maxStack = 999;
			item.consumable = true;
			item.knockBack = 1f;
			item.value = 500;
			item.rare = 2;
			item.shoot = mod.ProjectileType("BinaryArrowProjectile");
			item.shootSpeed = 3f;
			item.ammo = AmmoID.Arrow;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			// TODO: add ingredient droppable from new enemy?
			recipe.AddIngredient(ItemID.WoodenArrow, 128);
			recipe.SetResult(this, 128);
			recipe.AddRecipe();
		}
	}

	class BinaryArrowProjectile : ModProjectile
	{
		public static int numberArrows;
		public override void SetDefaults()
		{
			projectile.CloneDefaults(ProjectileID.WoodenArrowFriendly);
			/**
			projectile.width = 10;
			projectile.height = 10;
			projectile.alpha = 100;
			projectile.penetrate = 3;
			projectile.friendly = true;
			projectile.melee = true;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
			
			//
			**/
			projectile.timeLeft = 120;
			projectile.aiStyle = -1;

		}

		public override void AI()
		{
			Player owner = Main.player[projectile.owner];
			projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;
			projectile.ai[0] += 1f; // Use a timer 


			if (projectile.ai[0] == 15 && projectile.ai[1] < 2) //getBinaryArrowCount(owner) < 129)
			{
				for (int i = 0; i < 20; i++) // x-(y-1) golden dusts every frame
				{
					Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<BinaryDust>());

				}
				Vector2 dir = Vector2.Normalize(projectile.velocity);
				float magnitude = projectile.velocity.Length();
				Projectile.NewProjectile(projectile.position, magnitude * Rotate(dir, 8), mod.ProjectileType("BinaryArrow"),
					(int)owner.rangedDamage * projectile.damage, 0, projectile.owner, 0, projectile.ai[1] + 1);
				Projectile.NewProjectile(projectile.position, magnitude * Rotate(dir, -8), mod.ProjectileType("BinaryArrow"),
					(int)owner.rangedDamage * projectile.damage, 0, projectile.owner, 0, projectile.ai[1] + 1);
			}
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 10; i++) // x-(y-1) golden dusts every frame
			{
				Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<BinaryDust>());

			}
			numberArrows--;
		}
		public override bool OnTileCollide(Vector2 oldVelocity)
		{

			return base.OnTileCollide(oldVelocity);
		}

		public static Vector2 Rotate(Vector2 v, float degrees)
		{
			float radians = degrees * (float)(Math.PI / 180);
			float sin = (float)Math.Sin(radians);
			float cos = (float)Math.Cos(radians);

			float tx = v.X;
			float ty = v.Y;
			v.X = (cos * tx) - (sin * ty);
			v.Y = (sin * tx) + (cos * ty);

			return v;
		}
	}
}
