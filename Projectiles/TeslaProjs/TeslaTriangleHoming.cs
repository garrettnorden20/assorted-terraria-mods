using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace BasicMod.Projectiles.TeslaProjs
{
	// ATTACK 2
	class TeslaTriangleHoming : ModProjectile
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
			projectile.timeLeft = 600;
			projectile.aiStyle = 0;

		}


		Player nearest = null;

		public override void AI()
		{
			projectile.rotation = projectile.velocity.ToRotation();// + MathHelper.PiOver2;

			projectile.ai[0] += 1f; // Use a timer 

			if (projectile.alpha > 1)
			{
				projectile.alpha = projectile.alpha - 17; // make it less transparent
			}

			if (projectile.ai[0] % 3 == 0)
			{
				Dust.NewDust(projectile.position, projectile.width, projectile.height, 100);
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
			if (projectile.ai[0] == 30f) // find the thing to home in on
			{
				nearest = FindNearest(projectile.position);
			}

			if (projectile.ai[0] > 30f && projectile.ai[0] < 90f)
			{
				if (nearest != null && nearest.active == true)
				{
					// update velocity each frame to follow the NPC
					Vector2 direction = Vector2.Normalize(Vector2.Subtract(nearest.position, projectile.position));
					projectile.velocity = projectile.velocity.Length() * direction;
					projectile.rotation = projectile.velocity.ToRotation();
				}
			}

			float speed = projectile.velocity.Length();

			projectile.velocity = 1.0f * speed * Vector2.Normalize(projectile.velocity);



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

		public Player FindNearest(Vector2 pos) // avoid is one you already hit
		{
			Player nearest = null;
			float oldDist = 1001;
			float newDist = 1000;
			for (int i = 0; i < Terraria.Main.player.Length - 1; i++) //Do once for each player in the world
			{
				if (Terraria.Main.player[i].active == false)//Don't target dead players
					continue;
				if (!Collision.CanHit(pos, projectile.width, projectile.height, Main.player[i].position, Main.player[i].width, Main.player[i].height))
				{
					continue;
				}
				if (nearest == null) //if no players have made it past the previous few checks
					nearest = Terraria.Main.player[i]; //become the nearest player 
				else
				{
					oldDist = Vector2.Distance(pos, nearest.position);//Check the distance to the nearest player that's earlier in the loop
					newDist = Vector2.Distance(pos, Terraria.Main.player[i].position);//Check the distance to the current player in the loop
					if (newDist < oldDist)//If closer than the previous player in the loop
						nearest = Terraria.Main.player[i];//Become the nearest player
				}
			}
			return nearest; //return the player that is nearest to the vector 'pos'
		}
	}
}
