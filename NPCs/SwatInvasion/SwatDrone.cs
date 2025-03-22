using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BasicMod.NPCs.SwatInvasion
{
    public class SwatDrone : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("SwatDrone");
            Main.npcFrameCount[npc.type] = 3;
        }

        public override void SetDefaults()
        {
            npc.width = 52;
            npc.height = 52;

            npc.damage = 40;
            npc.defense = 20;
            npc.lifeMax = 500;

            npc.noTileCollide = false;
            npc.noGravity = true;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.value = 6000f;
            npc.knockBackResist = 0.5f;
            npc.aiStyle = -1;
            //aiType = 86;
            //animationType = 3;
            npc.buffImmune[BuffID.Confused] = false;
            //banner = npc.type;
            //bannerItem = mod.ItemType("VelocichopperBanner");
        }

        public override void HitEffect(int hitDirection, double damage)
        {
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {

            if (BasicWorld.SwatEvent)
            {
                return 30f;
            }
            else
            {
                return 0f;
            }

        }


		protected float speed = 2f;
		protected float acceleration = 0.1f;
		protected float speedY = 1.5f;
		protected float accelerationY = 0.04f;


		private int AI_Timer = 0;

		int maxTileHover = 30;
		int minTileHover = 9;

		public int Reload_Timer = 0;
		public int reloadTime = 180;

		float trueSpeed = 5f;

		public override void AI()
		{
			AI_Timer++;

			if (AI_Timer % 3 == 0)
            {
				npc.frameCounter++;
            }


			bool flag33 = false;
			if (npc.justHit)
			{
				npc.ai[2] = 0f;
			}
			if (npc.ai[2] >= 0f)
			{
				int num379 = 16;
				bool flag34 = false;
				bool flag35 = false;
				if (npc.position.X > npc.ai[0] - (float)num379 && npc.position.X < npc.ai[0] + (float)num379)
				{
					flag34 = true;
				}
				else if (npc.velocity.X < 0f && npc.direction > 0 || npc.velocity.X > 0f && npc.direction < 0)
				{
					flag34 = true;
				}
				num379 += 24;
				if (npc.position.Y > npc.ai[1] - (float)num379 && npc.position.Y < npc.ai[1] + (float)num379)
				{
					flag35 = true;
				}
				if (flag34 && flag35)
				{
					npc.ai[2] += 1f;
					if (npc.ai[2] >= 30f && num379 == 16)
					{
						flag33 = true;
					}
					if (npc.ai[2] >= 60f)
					{
						npc.ai[2] = -200f;
						npc.direction *= -1;
						npc.velocity.X *= -1f;
						npc.collideX = false;
					}
				}
				else
				{
					npc.ai[0] = npc.position.X;
					npc.ai[1] = npc.position.Y;
					npc.ai[2] = 0f;
				}
				npc.TargetClosest(true);
			}
			else
			{
				npc.ai[2] += 1f;
				if (Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) > npc.position.X + (float)(npc.width / 2))
				{
					npc.direction = -1;
				}
				else
				{
					npc.direction = 1;
				}
			}
			int num380 = (int)((npc.position.X + (float)(npc.width / 2)) / 16f) + npc.direction * 2;
			int num381 = (int)((npc.position.Y + (float)npc.height) / 16f);
			bool flag36 = true;
			bool flag37 = false;
			int num382 = maxTileHover;
			for (int num404 = num381; num404 < num381 + num382; num404++)
			{
				if (Main.tile[num380, num404] == null)
				{
					Main.tile[num380, num404] = new Tile();
				}
				if (Main.tile[num380, num404].nactive() && Main.tileSolid[(int)Main.tile[num380, num404].type] || Main.tile[num380, num404].liquid > 0)
				{
					if (num404 <= num381 + minTileHover)
					{
						flag37 = true;
					}
					flag36 = false;
					break;
				}
			}
			if (flag33)
			{
				flag37 = false;
				flag36 = true;
			}
			if (flag36)
			{
				npc.velocity.Y += Math.Max(0.2f, 2.5f * accelerationY);
				if (npc.velocity.Y > Math.Max(2f, speedY))
				{
					npc.velocity.Y = Math.Max(2f, speedY);
				}
			}
			else
			{
				if (npc.directionY < 0 && npc.velocity.Y > 0f || flag37)
				{
					npc.velocity.Y -= 0.2f;
				}
				if (npc.velocity.Y < -4f)
				{
					npc.velocity.Y = -4f;
				}
			}
			if (npc.collideX)
			{
				npc.velocity.X = npc.oldVelocity.X * -0.4f;
				if (npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 1f)
				{
					npc.velocity.X = 1f;
				}
				if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -1f)
				{
					npc.velocity.X = -1f;
				}
			}
			if (npc.collideY)
			{
				npc.velocity.Y = npc.oldVelocity.Y * -0.25f;
				if (npc.velocity.Y > 0f && npc.velocity.Y < 1f)
				{
					npc.velocity.Y = 1f;
				}
				if (npc.velocity.Y < 0f && npc.velocity.Y > -1f)
				{
					npc.velocity.Y = -1f;
				}
			}
			if (npc.direction == -1 && npc.velocity.X > -speed)
			{
				npc.velocity.X -= acceleration;
				if (npc.velocity.X > speed)
				{
					npc.velocity.X -= acceleration;
				}
				else if (npc.velocity.X > 0f)
				{
					npc.velocity.X += acceleration / 2f;
				}
				if (npc.velocity.X < -speed)
				{
					npc.velocity.X = -speed;
				}
			}
			else if (npc.direction == 1 && npc.velocity.X < speed)
			{
				npc.velocity.X += acceleration;
				if (npc.velocity.X < -speed)
				{
					npc.velocity.X += acceleration;
				}
				else if (npc.velocity.X < 0f)
				{
					npc.velocity.X -= acceleration / 2f;
				}
				if (npc.velocity.X > speed)
				{
					npc.velocity.X = speed;
				}
			}
			if (npc.directionY == -1 && (double)npc.velocity.Y > -speedY)
			{
				npc.velocity.Y -= accelerationY;
				if ((double)npc.velocity.Y > speedY)
				{
					npc.velocity.Y -= accelerationY * 1.25f;
				}
				else if (npc.velocity.Y > 0f)
				{
					npc.velocity.Y += accelerationY * 0.75f;
				}
				if ((double)npc.velocity.Y < -speedY)
				{
					npc.velocity.Y = -speedY;
				}
			}
			else if (npc.directionY == 1 && (double)npc.velocity.Y < speedY)
			{
				npc.velocity.Y += accelerationY;
				if ((double)npc.velocity.Y < -speedY)
				{
					npc.velocity.Y += accelerationY * 1.25f;
				}
				else if (npc.velocity.Y < 0f)
				{
					npc.velocity.Y -= accelerationY * 0.75f;
				}
				if ((double)npc.velocity.Y > speedY)
				{
					npc.velocity.Y = speedY;
				}
			}

			Reload_Timer++;
			if (Reload_Timer > reloadTime && Main.netMode != 1 && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
			{
				Vector2 shootPosition = new Vector2(npc.Center.X + (npc.width / 2 * npc.direction), npc.position.Y + npc.height);
				Vector2 totalVel = Vector2.Normalize(Main.player[npc.target].Center - shootPosition) * trueSpeed;
				totalVel = Utils.RotatedByRandom(totalVel, MathHelper.PiOver4/3);

				//Projectile.NewProjectile(npc.Center.X + (npc.width/2 * npc.direction), npc.position.Y + npc.height, 5.00f * npc.direction, 5.00f, ProjectileID.SmokeBomb, npc.damage, 3f, Main.myPlayer);
				Projectile.NewProjectile(shootPosition, totalVel, ProjectileID.BulletSnowman, npc.damage, 3f, Main.myPlayer);

				Reload_Timer = 0;
			}



		}


        public override void FindFrame(int frameHeight)
        {
            // This makes the sprite flip horizontally in conjunction with the npc.direction.
            npc.spriteDirection = npc.direction;
            //npc.frameCounter++;
            if (npc.frameCounter < 1)
            {
                npc.frame.Y = 0 * frameHeight;
            }
            else if (npc.frameCounter < 2)
            {
                npc.frame.Y = 1 * frameHeight;
            }
            else if (npc.frameCounter < 3)
            {
                npc.frame.Y = 2 * frameHeight;
            }
            else if (npc.frameCounter < 4)
            {
                npc.frame.Y = 1 * frameHeight;
            }
            if (npc.frameCounter < 5)
            {
                npc.frame.Y = 0 * frameHeight;
            }
            else
            {
                npc.frameCounter = 0;
            }
        }

        public override void NPCLoot()
        {

            BasicWorld.SwatKillCount += 1;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData); // Immediately inform clients of new world state.
            /**
            // Counting Should be handled in GLobalNPC
            //
            QwertyWorld.SwatKillCount += 5;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData); // Immediately inform clients of new world state.
            **/

            /**
            if (Main.rand.Next(0, 100) == 1)
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("SwatTooth"));
            }
            if (Main.rand.Next(0, 100) == 1)
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("WornPrehistoricBow"));
            }
            if (Main.expertMode)
            {
                if (Main.rand.Next(0, 100) <= 15)
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("SwatVulcan"));
                }
            }
            else
            {
                if (Main.rand.Next(0, 100) <= 10)
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("SwatVulcan"));
                }
            }
            **/
        }

    }
}