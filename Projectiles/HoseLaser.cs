using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace BasicMod.Projectiles
{
    class HoseLaser : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.RayGunnerLaser);
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.timeLeft = 120;
            projectile.ranged = true;
            projectile.alpha = 240;
            projectile.light = 0.1f;
            projectile.usesLocalNPCImmunity = true; // ignores enemy invicniblity frames
            projectile.localNPCHitCooldown = -1; // this guarentees there's no enemy invinciviltiy frames
            // projectile.alpha = 255, completely transparent, because in AI it's supposed to tick down quickly
            projectile.timeLeft = 50;
            projectile.tileCollide = true;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2; // set it right way
            projectile.ai[0] += 1f; // Use a timer 
            if (projectile.ai[0] <= 60) // in first 60 frames
            {
                if (Main.rand.Next(0, 31) == 50) // 1/20 chance
                {
                    Dust.NewDust(projectile.position, projectile.width, projectile.height, 217); // create a single dust
                }
            }
            if (projectile.alpha > 1)
            {
                projectile.alpha = projectile.alpha - 17; // make it less transparent
            }

        }

        public override void Kill(int timeLeft)
        {
            /**
            for (int i = 0; i < 3; i++) // x-(y-1) golden dusts every frame
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 217);

            }
            int minTileX = (int)(projectile.position.X / 16f);
            int maxTileY = (int)(projectile.position.Y / 16f);
            if (minTileX < 0)
            {
                minTileX = 0;
            }
            if (minTileX > Main.maxTilesX)
            {
                minTileX = Main.maxTilesX;
            }
            if (maxTileY < 0)
            {
                maxTileY = 0;
            }
            if (maxTileY > Main.maxTilesY)
            {
                maxTileY = Main.maxTilesY;
            }
            WorldGen.KillTile(minTileX, maxTileY);
            **/
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            /**
            //WorldGen.KillTile((int)projectile.position.X, (int)projectile.position.Y);
            int minTileX = (int)(projectile.position.X / 16f);
            int maxTileY = (int)(projectile.position.Y / 16f);
            if (minTileX < 0)
            {
                minTileX = 0;
            }
            if (minTileX > Main.maxTilesX)
            {
                minTileX = Main.maxTilesX;
            }
            if (maxTileY < 0)
            {
                maxTileY = 0;
            }
            if (maxTileY > Main.maxTilesY)
            {
                maxTileY = Main.maxTilesY;
            }
            WorldGen.KillTile(minTileX, maxTileY);
            **/


            //Annihilation();
            Hellevator();
            return true;
        }

        public void Annihilation()
        {
            int explosionRadius = 1;

            int minTileX = (int)(projectile.position.X / 16f - (float)explosionRadius);
            int maxTileX = (int)(projectile.position.X / 16f + (float)explosionRadius);
            int minTileY = (int)(projectile.position.Y / 16f - (float)explosionRadius);
            int maxTileY = (int)(projectile.position.Y / 16f + (float)explosionRadius);
            if (minTileX < 0)
            {
                minTileX = 0;
            }
            if (maxTileX > Main.maxTilesX)
            {
                maxTileX = Main.maxTilesX;
            }
            if (minTileY < 0)
            {
                minTileY = 0;
            }
            if (maxTileY > Main.maxTilesY)
            {
                maxTileY = Main.maxTilesY;
            }
            bool canKillWalls = false;
            for (int x = minTileX; x <= maxTileX; x++)
            {
                for (int y = minTileY; y <= maxTileY; y++)
                {
                    float diffX = Math.Abs((float)x - projectile.position.X / 16f);
                    float diffY = Math.Abs((float)y - projectile.position.Y / 16f);
                    double distance = Math.Sqrt((double)(diffX * diffX + diffY * diffY));
                    if (distance < (double)explosionRadius && Main.tile[x, y] != null && Main.tile[x, y].wall == 0)
                    {
                        canKillWalls = true;
                        break;
                    }
                }
            }
            for (int i = minTileX; i <= maxTileX; i++)
            {
                for (int j = minTileY; j <= maxTileY; j++)
                {
                    float diffX = Math.Abs((float)i - projectile.position.X / 16f);
                    float diffY = Math.Abs((float)j - projectile.position.Y / 16f);
                    double distanceToTile = Math.Sqrt((double)(diffX * diffX + diffY * diffY));
                    if (distanceToTile < (double)explosionRadius)
                    {
                        bool canKillTile = true;
                        if (Main.tile[i, j] != null && Main.tile[i, j].active())
                        {
                            canKillTile = true;

                            if (!TileLoader.CanExplode(i, j))
                            {
                                canKillTile = false;
                            }
                            if (canKillTile)
                            {
                                WorldGen.KillTile(i, j, false, false, false);
                                if (!Main.tile[i, j].active() && Main.netMode != NetmodeID.SinglePlayer)
                                {
                                    NetMessage.SendData(MessageID.TileChange, -1, -1, null, 0, (float)i, (float)j, 0f, 0, 0, 0);
                                }
                            }
                        }
                        if (canKillTile)
                        {
                            for (int x = i - 1; x <= i + 1; x++)
                            {
                                for (int y = j - 1; y <= j + 1; y++)
                                {
                                    if (Main.tile[x, y] != null && Main.tile[x, y].wall > 0 && canKillWalls && WallLoader.CanExplode(x, y, Main.tile[x, y].wall))
                                    {
                                        WorldGen.KillWall(x, y, false);
                                        if (Main.tile[x, y].wall == 0 && Main.netMode != NetmodeID.SinglePlayer)
                                        {
                                            NetMessage.SendData(MessageID.TileChange, -1, -1, null, 2, (float)x, (float)y, 0f, 0, 0, 0);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        public void Hellevator()
        {
            int explosionRadius = 2;

            int minTileX = (int)(projectile.position.X / 16f - (float)explosionRadius);
            int maxTileX = (int)(projectile.position.X / 16f + (float)explosionRadius);
            int minTileY = (int)(projectile.position.Y / 16f - (float)explosionRadius);
            int maxTileY = Main.maxTilesY;
            if (minTileX < 0)
            {
                minTileX = 0;
            }
            if (maxTileX > Main.maxTilesX)
            {
                maxTileX = Main.maxTilesX;
            }
            if (minTileY < 0)
            {
                minTileY = 0;
            }
            if (maxTileY > Main.maxTilesY)
            {
                maxTileY = Main.maxTilesY;
            }
            bool canKillWalls = false;
            for (int y = minTileY; y <= maxTileY; y++)
            {
                for (int x = minTileX; x <= maxTileX; x++)
                {
                    float diffX = Math.Abs((float)x - projectile.position.X / 16f);
                    float diffY = Math.Abs((float)y - projectile.position.Y / 16f);
                    double distance = Math.Sqrt((double)(diffX * diffX + diffY * diffY));
                    if (distance < (double)explosionRadius && Main.tile[x, y] != null && Main.tile[x, y].wall == 0)
                    {
                        canKillWalls = true;
                        break;
                    }
                }
            }
            for (int j = minTileY; j <= maxTileY; j++)
            {
                    for (int i = minTileX; i <= maxTileX; i++)
                    {
                        bool canKillTile = true;
                        if (Main.tile[i, j] != null && Main.tile[i, j].active())
                        {
                            canKillTile = true;

                            if (!TileLoader.CanExplode(i, j))
                            {
                                canKillTile = false;
                            }
                            if (canKillTile)
                            {
                                WorldGen.KillTile(i, j, false, false, false);
                                if (!Main.tile[i, j].active() && Main.netMode != NetmodeID.SinglePlayer)
                                {
                                    NetMessage.SendData(MessageID.TileChange, -1, -1, null, 0, (float)i, (float)j, 0f, 0, 0, 0);
                                }
                            }
                        }
                        if (canKillTile)
                        {
                            for (int x = i - 1; x <= i + 1; x++)
                            {
                                for (int y = j - 1; y <= j + 1; y++)
                                {
                                    if (Main.tile[x, y] != null && Main.tile[x, y].wall > 0 && canKillWalls && WallLoader.CanExplode(x, y, Main.tile[x, y].wall))
                                    {
                                        WorldGen.KillWall(x, y, false);
                                        if (Main.tile[x, y].wall == 0 && Main.netMode != NetmodeID.SinglePlayer)
                                        {
                                            NetMessage.SendData(MessageID.TileChange, -1, -1, null, 2, (float)x, (float)y, 0f, 0, 0, 0);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
}