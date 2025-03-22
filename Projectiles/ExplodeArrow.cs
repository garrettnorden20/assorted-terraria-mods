using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace BasicMod.Projectiles
{
	// if kills nearest while pierce is active, it's still trying to go after nearest
	class ExplodeArrow : ModProjectile
	{

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
			projectile.timeLeft = 300;
			projectile.aiStyle = 0;

		}

		public override void AI()
		{
			Player owner = Main.player[projectile.owner];
			projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
			projectile.ai[0] += 1f; // Use a timer 
			if (owner.altFunctionUse == 2)
			{
				for (int i = 0; i < 2; i++) // x-(y-1) golden dusts every frame
				{
					Dust.NewDust(projectile.position, projectile.width, projectile.height, 74);

				}
				Vector2 dir = Vector2.Normalize(projectile.velocity);
				float magnitude = projectile.velocity.Length() * 2;
				for (int i = 0; i < 360; i = i + 10)
				{
					Projectile.NewProjectile(projectile.position, magnitude * Rotate(dir, i), ProjectileID.HellfireArrow,
					(int)owner.rangedDamage * projectile.damage, 0, Main.myPlayer);
				}
				projectile.active = false;
			}
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
