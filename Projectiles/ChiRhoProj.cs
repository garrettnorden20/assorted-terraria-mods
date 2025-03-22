using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace BasicMod.Projectiles
{
	// if kills nearest while pierce is active, it's still trying to go after nearest
    class ChiRhoProj : ModProjectile
    {
		NPC nearest = null;
		Random rand = new Random();

		public override void SetDefaults()
		{
			//projectile.CloneDefaults(ProjectileID.TerraBeam);
			projectile.width = 16;
			projectile.height = 16;
			projectile.alpha = 100;
			projectile.penetrate = 3;
			projectile.friendly = true;
			projectile.melee = true;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
			projectile.timeLeft = 600;
			projectile.aiStyle = 0;


		}

        public override void AI()
        {	
			for (int i = 0; i < rand.Next(1, 2); i++) // x-(y-1) golden dusts every frame
			{
				Dust.NewDust(projectile.position, projectile.width, projectile.height, 74);

			}

			projectile.ai[0] += 1f; // Use a timer 
			if (projectile.ai[0] < 60f) // slowdown for 1 second
			{
				projectile.rotation += (0.6f * (float)projectile.direction) * ((100f - (projectile.ai[0])) / 100f); // angular deacceleration
				projectile.velocity = projectile.velocity * 0.98f;
			}
			if (projectile.ai[0] == 60f) // find the thing to home in on
            {
				nearest = FindNearest(projectile.position, null);
            }

			if (projectile.ai[0] > 60f && projectile.ai[0] < 80f) // stop, face direction
			{	
				projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver4; // face direction of travel
			}
			if (projectile.ai[0] >= 80f)
			{
				if (projectile.velocity.Length() < 18f) // speed up if velocity less than this
				{
					projectile.velocity = projectile.velocity * 1.1f;
				}
				if (nearest != null && nearest.active == true)
                {
					// update velocity each frame to follow the NPC
					Vector2 direction = Vector2.Normalize(Vector2.Subtract(nearest.position, projectile.position));
					projectile.velocity = projectile.velocity.Length() * direction;
					projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver4; // face direction of travel
				}
			} 
			
		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			/**
            if (target.life - damage <= 0)
            {
				//nearest = FindNearest(projectile.position, target); // go to next NPC
				nearest = null;
            }
			**/
        }

        public NPC FindNearest(Vector2 pos, NPC avoid) // avoid is one you already hit
		{
			NPC nearest = null;
			float oldDist = 1001;
			float newDist = 1000;
			for (int i = 0; i < Terraria.Main.npc.Length - 1; i++) //Do once for each NPC in the world
			{
				if (Terraria.Main.npc[i] == avoid)//Don't target the one you want to avoid
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
