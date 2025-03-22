using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace BasicMod.Projectiles
{
    class Icicle : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 1;
            drawOriginOffsetY = -5;
            drawOffsetX = 6;
        }
        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.WoodenArrowFriendly);
            projectile.height = 50;
            projectile.width = 50;
 
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.ranged = false;
            projectile.magic = true;
            projectile.alpha = 10;
            projectile.arrow = false;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 300;
            
        }

        bool falling = false;
        Random rand = new Random();
        int fallTime = 25;

        public override void AI()
        {
            projectile.rotation = MathHelper.TwoPi;
            projectile.ai[1] += 1f; // timer

            //Making player variable "p" set as the projectile's owner
            Player p = Main.player[projectile.owner];

            if (projectile.ai[1] % 3 == 0)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Ice);
            }

            if (projectile.ai[1] == 1) // on startup
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Ice);
                }
            }

            if (projectile.ai[1] < fallTime)
            {
                projectile.velocity = Vector2.Zero;
            }

            if (projectile.ai[1] == fallTime)
            {
                falling = true;
                projectile.velocity = new Vector2(0, 3);
            }

            if (projectile.ai[1] > fallTime)
            {
                if (projectile.velocity.Length() < 10f && projectile.ai[1] % 3 == 0)
                {
                    projectile.velocity = projectile.velocity * 1.1f;
                }
            }
           



        }


        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (falling == false)
            {
                return false;
            }
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int i = 0; i < 100; i++)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Ice);
               
            }
            Main.PlaySound(SoundID.Item50, (int)projectile.position.X, (int)projectile.position.Y);

            return true;
        }
    }
}