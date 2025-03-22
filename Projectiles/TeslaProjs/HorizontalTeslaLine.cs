using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.DataStructures;


namespace BasicMod.Projectiles.TeslaProjs
{
	// ATTACK LINES
	class HorizontalTeslaLine : ModProjectile
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
			projectile.friendly = false;
			projectile.hostile = true;
			projectile.tileCollide = false;
			projectile.timeLeft = 50;
			projectile.aiStyle = 0;

		}

		bool active = false;
		Vector2 start = Vector2.Zero;
		Vector2 end = Vector2.Zero;


		float Distance = 0;

		public override void AI()
		{
			if (projectile.ai[0] == 0)
			{
				start = projectile.position + new Vector2(-1.5f * Main.screenWidth, 0); // to left
				end = projectile.position + new Vector2(1.5f * Main.screenWidth, 0); // to right
				Distance = Vector2.Distance(start, end);
			}
			Player owner = Main.player[projectile.owner];
			//projectile.rotation = projectile.velocity.ToRotation();// + MathHelper.PiOver2;
			projectile.ai[0] += 1f; // Use a timer 

			if (projectile.ai[0] == 25)
			{
				active = true;
				Main.PlaySound(SoundID.DD2_LightningAuraZap, (int)projectile.position.X, (int)projectile.position.Y);
			}

			if (projectile.ai[0] == 50)
			{
				active = false;
				projectile.Kill();
			}


		}

		int smallWidth = 6;
		int largeWidth = 12;

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			int height = (int)(end.Y - start.Y);
			height = 3 * Main.screenHeight;
			height = (int)Distance;
			int width;
			// set your widths and colors here
			if (!active)
			{
				Utils.DrawLine(spriteBatch, start, end, Color.White, Color.Transparent, smallWidth);
				width = smallWidth;
			}
			else
			{
				Color col = new Color(85, 255, 255, 10);

				//Utils.DrawLine(spriteBatch, start, end, Color.DarkTurquoise, Color.Cyan, 12);
				Utils.DrawLine(spriteBatch, start, end, col, col, largeWidth);
				width = largeWidth;
			}

			/**
			// dust (too laggy)
			for (int d = 0; d < 140; d++)
			{
				Dust.NewDust(projectile.position, width, height, 202);
			}
			**/

			return false;
		}


		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			// We can only collide if we are at max charge, which is when the laser is actually fired
			if (!active) return false;

			// Run an AABB versus Line check to look for collisions, look up AABB collision first to see how it works
			// It will look for collisions on the given line using AABB
			return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start,
				end);
		}

		private void CastLights()
		{
			// Cast a light along the line of the laser
			DelegateMethods.v3_1 = new Vector3(0.8f, 0.8f, 1f);
			Utils.PlotTileLine(start, end, 10, DelegateMethods.CastLight);
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
