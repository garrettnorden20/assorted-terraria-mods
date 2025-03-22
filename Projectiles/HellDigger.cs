using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace BasicMod.Projectiles
{
    class HellDigger : ModProjectile
    {
        //public override string Texture => "Terraria/Projectile_" + ProjectileID.WoodenArrowFriendly;
        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.RayGunnerLaser);
            projectile.friendly = false;
            projectile.hostile = false;
            projectile.timeLeft = 3600;
            projectile.ranged = true;
            projectile.alpha = 0;
            projectile.usesLocalNPCImmunity = true; // ignores enemy invicniblity frames
            projectile.tileCollide = false;
            projectile.height = 16;
            projectile.width = 16;
            drawOriginOffsetY = -30;

        }

        public override void AI()
        {
            projectile.rotation = projectile.rotation + MathHelper.Pi;
            if (projectile.ai[1] == 0) // at start 
            {
                Main.NewText("inside AI if method");
            }
            if (projectile.position.ToTileCoordinates().Y <= 1) // if at very bottom
            {
                projectile.Kill(); // kill the projectile
            } else
            {
                projectile.timeLeft = 3600; // reset the time
            }
            projectile.velocity = new Vector2(0,0);
            if (projectile.ai[1] > 1 && projectile.ai[1] % 16 == 0) // every __ frames // old is 30
            {
                Hellevator(projectile.ai[1]);
                //projectile.position.Y += 16f;
            }
            projectile.position.Y += 1f;

            projectile.ai[1] += 1f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
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


        public void Hellevator(float timer)
        {
            //Main.NewText("start of hellevator");
            int explosionRadius = 2;

            Vector2 proposition = projectile.Center;

            int minTileX = (int)(proposition.X / 16f - (float)explosionRadius);
            int maxTileX = (int)(proposition.X / 16f + (float)explosionRadius);
            int minTileY = (int)(proposition.Y / 16f); // 1 to account for the tile happening
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

            for (int i = minTileX; i <= maxTileX; i++)
            {
                bool canKillTile = true;
                if (Main.tile[i, minTileY] != null && Main.tile[i, minTileY].active())
                {
                    canKillTile = true;

                    if (!TileLoader.CanExplode(i, minTileY))
                    {
                        canKillTile = false;
                    }
                    if (canKillTile)
                    {
                        //Main.NewText("inside kill tile");
                        WorldGen.KillTile(i, minTileY, false, false, false);
                        if (!Main.tile[i, minTileY].active() && Main.netMode != NetmodeID.SinglePlayer)
                        {
                            NetMessage.SendData(MessageID.TileChange, -1, -1, null, 0, (float)i, (float)minTileY, 0f, 0, 0, 0);
                        }
                    }
                }

            }
        }
    }
}
