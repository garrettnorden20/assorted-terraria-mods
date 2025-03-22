using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace BasicMod.Projectiles
{
    class LeafProj : ModProjectile
    {
        private double degreeConstant = 1.5;

        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 5;
        }
        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.WoodenArrowFriendly);
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.ranged = true;
            projectile.alpha = 10;
            projectile.light = 0.75f;
            projectile.arrow = false;
            projectile.tileCollide = false;
            projectile.penetrate = 10;
            projectile.timeLeft = 600;
            projectile.usesLocalNPCImmunity = true; // ignores enemy invicniblity frames
            projectile.localNPCHitCooldown = -1; // this guarentees there's no enemy invinciviltiy frames
           
        }

        bool homingIn = false; // once its released, this becomes true forever
        NPC nearest = null;
        Boolean lightening;
        Vector2 mousePosition;
        Random rand = new Random();

        public override void AI()
        {
            projectile.ai[1] += 1f; // timer

            //Making player variable "p" set as the projectile's owner
            Player p = Main.player[projectile.owner];
            
            if (projectile.ai[1] == 1) // on startup
            {

                projectile.ai[1] = 90f;
                projectile.rotation = 0;
                nearest = FindNearest(p.position, null); // get nearest NPC
            }


            // one in whatever chance to spawn a dust (green)
            if (rand.Next(0,91) == 0)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 75);
            }


            // cycling through frame anims
            if (++projectile.frameCounter >= 5)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= 5)
                {
                    projectile.frame = 0;
                }
            }

            // "pulsing" effects
            if (projectile.alpha == 20)
            {
                lightening = true;
            }
            if (projectile.alpha == 140)
            {
                lightening = false;
            }
            if (lightening == true)
            {
                projectile.alpha += 20;
            }
            if (lightening == false)
            {
                projectile.alpha -= 20;
            }

            //Factors for calculations
            double deg = (double)projectile.ai[1] * 3; // degreeConstant; //The degrees, you can multiply projectile.ai[1] to make it orbit faster, may be choppy depending on the value
            double rad = deg * (Math.PI / 180); //Convert degrees to radians
            double dist = 100; //Distance away from the player


            if (p.releaseUseItem == true || homingIn == true) // mouse left-click released, or had been at some point
            {
                if (homingIn == false)
                { // immediatly after left click, and only then
                    homingIn = true;

                    projectile.knockBack = 0; // low knockback when launched

                    // for mouse
                    mousePosition = Main.MouseWorld;
                    Vector2 direction = Vector2.Normalize(Vector2.Subtract(mousePosition, projectile.position));
                    projectile.velocity = projectile.velocity.Length() * direction;
                    projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2; // face direction of travel
                }
                projectile.rotation = projectile.velocity.ToRotation();
                
                if (projectile.velocity.Length() < 18f) // speed up if velocity less than this
                {
                    projectile.velocity = projectile.velocity * 1.1f;
                }
                /**
                if (nearest != null && nearest.active == true)
                {
                    // update velocity each frame to follow the NPC
                    Vector2 direction = Vector2.Normalize(Vector2.Subtract(nearest.position, projectile.position));
                    projectile.velocity = projectile.velocity.Length() * direction;
                    projectile.rotation = projectile.velocity.ToRotation(); // MathHelper.PiOver2; // face direction of travel
                }
                **/
                   
                

            }
            else // this is when they rotate in wall formation
            {
                projectile.knockBack = 15; // knockback high when wall form
                nearest = FindNearest(p.position, null); // get nearest NPC

                projectile.rotation = (float)rad + (3.0f * MathHelper.PiOver2);


                /*Position the player based on where the player is, the Sin/Cos of the angle times the /
                /distance for the desired distance away from the player minus the projectile's width   /
                /and height divided by two so the center of the projectile is at the right place.     */
                projectile.position.X = -5 + p.Center.X - (int)(Math.Cos(rad) * dist) - projectile.width / 2;
                projectile.position.Y = 7 + p.Center.Y - (int)(Math.Sin(rad) * dist) - projectile.height / 2;

                //projectile.direction = (int)projectile.velocity.ToRotation(); // face forward

                //Increase the counter/angle in degrees by 1 point, you can change the rate here too, but the orbit may look choppy depending on the value
            }

            

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