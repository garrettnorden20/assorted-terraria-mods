using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;

namespace BasicMod.Projectiles.RangedProjectiles
{
	
	class HomingPopperProjectile : ModProjectile
	{
		NPC nearest = null;
		Random rand = new Random();

		public override string Texture => "Terraria/Projectile_" + ProjectileID.WoodenArrowFriendly;

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;    //The length of old position to be recorded
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;        //The recording mode
		}
		public override void SetDefaults()
		{
			//projectile.CloneDefaults(ProjectileID.TerraBeam);
			projectile.width = 16;
			projectile.height = 16;
			projectile.alpha = 50;
			projectile.penetrate = 20;
			projectile.friendly = true;
			projectile.melee = true;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
			projectile.timeLeft = 600;
			projectile.aiStyle = 0;


		}

		ArrayList avoidList = new ArrayList();
		float trueVelocity = 20f;
		float stopTime = 60;
		bool fastMode = false;

		public override void AI()
		{
			projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
			
			/**
			for (int i = 0; i < rand.Next(1, 2); i++) // x-(y-1) golden dusts every frame
			{
				Dust.NewDust(projectile.position, projectile.width, projectile.height, 74);

			}
			**/

			projectile.ai[0] += 1f; // Use a timer 
			if (projectile.ai[0] < stopTime) // slowdown for 1 second
			{
				projectile.velocity = projectile.velocity * 0.98f;

			}
			if (projectile.ai[0] == stopTime) // find the thing to home in on
			{
				nearest = FindNearest(projectile.position, null);
				if (nearest == null)
				{
					projectile.Kill();
				}
				for (int n = 0; n < 10; n++)
				{
					Dust.NewDust(projectile.position, projectile.width, projectile.height, 31);

				}
			}
			if (projectile.ai[0] > stopTime)
			{
				fastMode = true;
				if (nearest != null && nearest.active == true)
				{
					// update velocity each frame to follow the NPC
					Vector2 direction = Vector2.Normalize(Vector2.Subtract(nearest.position, projectile.position));
					projectile.velocity = trueVelocity * direction;
				}

			}

		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			avoidList.Add(target);
			nearest = FindNearest(projectile.position, target); // go to next NPC
			if (nearest == null)
            {
				projectile.Kill();
            }
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			if (fastMode)
			{
				//Redraw the projectile with the color not influenced by light
				Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
				for (int k = 0; k < projectile.oldPos.Length; k++)
				{
					Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
					Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
					spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
				}
			}
			return true;
		}

		public override void Kill(int timeLeft)
		{
			// This code and the similar code above in OnTileCollide spawn dust from the tiles collided with. SoundID.Item10 is the bounce sound you hear.
			Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
			Main.PlaySound(SoundID.Item10, projectile.position);
		}

		public NPC FindNearest(Vector2 pos, NPC avoid) // avoid is one you already hit
		{
			NPC nearest = null;
			float oldDist = 1001;
			float newDist = 1000;
			for (int i = 0; i < Terraria.Main.npc.Length - 1; i++) //Do once for each NPC in the world
			{
				if (avoidList.Contains(Terraria.Main.npc[i]))//Don't target the one you want to avoid
					continue;
				if (Terraria.Main.npc[i].friendly == true)//Don't target town NPCs
					continue;
				if (Terraria.Main.npc[i].active == false)//Don't target dead NPCs
					continue;
				if (Terraria.Main.npc[i].damage == 0)//Don't target non-aggressive NPCs
					continue;
				if (!Collision.CanHit(pos, projectile.width, projectile.height, Main.npc[i].position, Main.npc[i].width, Main.npc[i].height))
				{
					continue;
				}
				if (nearest == null) //if no NPCs have made it past the previous few checks
					nearest = Terraria.Main.npc[i]; //become the nearest NPC 
				else
				{
					oldDist = Vector2.Distance(pos, nearest.position);//Check the distance to the nearest NPC that's earlier in the loop
					newDist = Vector2.Distance(pos, Terraria.Main.npc[i].position);//Check the distance to the current NPC in the loop
					if (newDist < oldDist)//If closer than the previous NPC in the loop
						nearest = Terraria.Main.npc[i];//Become the nearest NPC
				}
			}
			return nearest; //return the npc that is nearest to the vector 'pos'
		}
	}
}
