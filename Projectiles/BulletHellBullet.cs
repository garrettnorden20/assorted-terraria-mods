using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace BasicMod.Projectiles
{
	// ATTACK 1
	class BulletHellBullet : ModProjectile
	{
		//public override string Texture => "Terraria/Projectile_" + ProjectileID.WoodenArrowFriendly;
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
			projectile.tileCollide = false;
			projectile.timeLeft = 600;
			projectile.aiStyle = 0;

		}

		float posAngle;

		int amplitude = 4;
		float periodLength = 15; // bigger number means longer period
		public override void AI()
		{
			Player owner = Main.player[projectile.owner];
			projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
			projectile.ai[0] += 1f; // Use a timer 
			float currentTime = projectile.ai[0] / periodLength;

			if (projectile.ai[0] < 5)
            {
				return;
            }

			Vector2 mathPos = projectile.position - owner.position; // this is our main vector - this subtraction is done because coordinates are measured
			// from the top left of the screen.
			if (projectile.ai[0] == 5) // if done at 0, the projectile may not be far away enough to detect the angle 
            {
				posAngle = (float)Math.Acos(Vector2.Dot(mathPos, Vector2.UnitX)/mathPos.Length());
				// this operation gets the angle between the unit vector and our main. returns angle between 0 and pi. 
				// Note that for each arrow, posAngle must remain the same during its entire lifespan. 

				if (mathPos.Y > 0)  // because of how arccos works, make sure if mathPos.Y is below player, the angle is negative (positive means down in Terraria)
				{
					posAngle *= -1;
				}

				//Main.NewText("Angle: " + posAngle);
				//Main.NewText("MathPos: " + mathPos);
			}

			
			float curveMult = (amplitude * (float)Math.Cos(currentTime)); // this effectively is the "hypotoneus" of our second vector. by setting it to 
			// cos or sin with a function of time, you can make it alternate up and down along its path. put this into desmos to see the path if you fired it straight

			Vector2 offsetVector = new Vector2(curveMult * (float)Math.Cos(MathHelper.PiOver2 - posAngle), curveMult * (float)Math.Sin(MathHelper.PiOver2 - posAngle));
			// the vector to be added to our position. it is always orthogonal to the position vector. If the curve multi was our hypotneous,
			// then the X and Y parts of this vector are effectively the legs.

			Vector2 newMathPos = mathPos + offsetVector; // add the two together 

			

			projectile.position = owner.position + newMathPos; // now make the position vector the way Terraria likes it



			// get current speed (length of velocity)
			float speed = projectile.velocity.Length();
			//projectile.velocity = speed * Vector2.Normalize(new Vector2(projectile.velocity.X + (3 * (float)Math.Cos(currentDegree)), projectile.velocity.Y));

			//projectile.velocity = 1.005f * speed * Vector2.Normalize(projectile.velocity);

			//projectile.rotation = projectile.position.ToRotation() + MathHelper.PiOver2;

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
