using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace BasicMod.Projectiles.TeslaProjs
{
	// ATTACK 3
	class TeslaTriangle : ModProjectile
	{
		//public override string Texture => "Terraria/Projectile_" + ProjectileID.WoodenArrowFriendly;
		public override void SetStaticDefaults()
		{
			Main.projFrames[projectile.type] = 4; // this is when using the squiggly one
		}
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
			
			projectile.ignoreWater = true;
			
			//
			**/
			projectile.friendly = false;
			projectile.hostile = true;
			projectile.light = 1;

			projectile.tileCollide = false;
			projectile.timeLeft = 300;
			projectile.aiStyle = 0;

		}

		public override void AI()
		{
			Player owner = Main.player[projectile.owner];
			projectile.rotation = projectile.velocity.ToRotation();// + MathHelper.PiOver2;

			projectile.ai[0] += 1f; // Use a timer 

			if (projectile.alpha > 1)
			{
				projectile.alpha = projectile.alpha - 17; // make it less transparent
			}

			if (projectile.ai[0] % 20 == 0)
			{
				Dust.NewDust(projectile.position, projectile.width, projectile.height, 202);
			}


			// cycling through frame anims
			if (++projectile.frameCounter >= 4)
			{
				projectile.frameCounter = 0;
				if (++projectile.frame >= 4)
				{
					projectile.frame = 0;
				}
			}


			// get current speed (length of velocity)
			float speed = projectile.velocity.Length();

			projectile.velocity = 1.005f * speed * Vector2.Normalize(projectile.velocity);

			

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
