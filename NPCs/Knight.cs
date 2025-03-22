using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BasicMod.NPCs
{
	// This ModNPC serves as an example of a complete AI example.
	public class Knight : ModNPC
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Flutter Slime"); // Automatic from .lang files
			Main.npcFrameCount[npc.type] = 5; // make sure to set this for your modnpcs.
		}

		public override void SetDefaults()
		{
			
			npc.width = 18;
			npc.height = 40;
			npc.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
			npc.damage = 7;
			npc.scale = 1;
			npc.defense = 2;
			npc.lifeMax = 25;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			//npc.alpha = 175;
			//npc.color = new Color(0, 80, 255, 100);
			npc.value = 25f;
			npc.buffImmune[BuffID.Poisoned] = true;
			npc.buffImmune[BuffID.Confused] = false; // npc default to being immune to the Confused debuff. Allowing confused could be a little more work depending on the AI. npc.confused is true while the npc is confused.
			//aiType = NPCID.Zombie;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			// we would like this npc to spawn in the overworld.
			return 0f;
		}

		private const int Frame_Move_1 = 0;
		private const int Frame_Move_2 = 1;
		private const int Frame_Move_3 = 2;
		private const int Frame_Move_4 = 3;

		

		float accelSpeed = 0.1f;
		float maxSpeed = 1.6f;

		int jumps = 0;
		float playerChaseThreshold = 100;
		float chaseReturnThreshold = 500;
		
		public void AI2()
		{
			
			bool stopped = false;
			if (npc.velocity.X == 0f)
			{
				stopped = true;
			} else
            {
				jumps = 0;
            }
			if (npc.justHit)
			{
				stopped = false;
			}


			if (jumps > 2) // has been jumping in place and not getting closer
			{
				npc.direction = -1 * npc.direction; // try other direction
				jumps = 0;
				if (npc.ai[3] < playerChaseThreshold + 1) // if chasing the player, make it not
                {
					npc.ai[3] = playerChaseThreshold + 1;
                }
			}

			Vector2 targetPosition = new Vector2(0, 0);

			if (npc.ai[3] >= chaseReturnThreshold) // reset back to Player state
			{
				npc.ai[3] = 0;
			}
			if (npc.ai[3] < playerChaseThreshold) // on Player state
			{
				npc.TargetClosest(true);
				npc.FaceTarget();
				targetPosition = Main.player[npc.target].position; // chase the player
			}
			
			if (npc.ai[3] >= playerChaseThreshold && npc.ai[3] < chaseReturnThreshold) // on Wander state
            {
				if (npc.ai[3] == playerChaseThreshold) // flip direction when it first hits threshold
				{
					//Teleport();
					//float distance = Math.Abs(npc.position.X - Main.player[npc.target].position.X) + Math.Abs(npc.position.Y - Main.player[npc.target].position.Y);
					//Vector2 dir = (Main.player[npc.target].position - npc.position);
					//npc.ai[3] = 0;
					//npc.velocity += Vector2.Normalize(dir) * 12f;
					//return;
					npc.direction = -1 * npc.direction;
				}
				targetPosition = (npc.direction == -1) ? new Vector2(0, 0) : new Vector2(Main.maxTilesX * 16, 0); // set the target position to far left or far right
            }
			//Main.NewText(jumps + " dir: " + npc.direction + " targ: " + targetPosition.X);


			// RUNNING TOWARD TARGET
			if (targetPosition.X < npc.position.X && npc.velocity.X > -1 * maxSpeed) // // IF the target is to my left AND I'm not at max "left" velocity
			{
				npc.velocity.X -= accelSpeed; // accelerate to the left
			}
			else if (targetPosition.X < npc.position.X && npc.velocity.X < -1 * maxSpeed) // too fast
			{
				npc.velocity.X += accelSpeed; // de
			}

			if (targetPosition.X > npc.position.X && npc.velocity.X < maxSpeed) // AND I'm not at max "right" velocity
			{
				npc.velocity.X += accelSpeed; // accelerate to the right
			}
			else if (targetPosition.X > npc.position.X && npc.velocity.X > maxSpeed) // too fast
			{
				npc.velocity.X -= accelSpeed; // de
			}

			int num38 = 60;
			bool flag4 = false;


			// changes ai[3]
			if (npc.velocity.Y == 0f && ((npc.velocity.X > 0f && npc.direction < 0) || (npc.velocity.X < 0f && npc.direction > 0)))
			{ // no vertical, but moving in way opposite their direction
				flag4 = true;
			}
			if (npc.position.X == npc.oldPosition.X || npc.ai[3] >= (float)num38 || flag4)
			{
				npc.ai[3] += 1f;
			}
			else if ((double)Math.Abs(npc.velocity.X) > 0.9 && npc.ai[3] > 0f)
			{
				npc.ai[3] -= 1f;
			}
			if (npc.ai[3] > (float)(num38 * 10))
			{
				npc.ai[3] = 0f;
			}
			if (npc.justHit)
			{
				npc.ai[3] = 0f;
			}
			if (npc.ai[3] == (float)num38)
			{
				npc.netUpdate = true;
			}


			bool flag22 = false;
			if (npc.velocity.Y == 0f)
			{
				int num168 = (int)(npc.position.Y + (float)npc.height + 7f) / 16; // y tile coord of its bottom
				int num169 = (int)npc.position.X / 16; // x tile coord of its left side
				int num170 = (int)(npc.position.X + (float)npc.width) / 16; // x tile coord of its right side
				for (int num171 = num169; num171 <= num170; num171++)
				{
					if (Main.tile[num171, num168] == null)
					{
						return;
					}
					if (Main.tile[num171, num168].nactive() && Main.tileSolid[Main.tile[num171, num168].type])
					{
						flag22 = true;
						break;
					}
				}
			}
			if (npc.velocity.Y >= 0f)
			{ // falling
				int xVelDir = 0;
				if (npc.velocity.X < 0f)
				{
					xVelDir = -1;
				}
				if (npc.velocity.X > 0f)
				{
					xVelDir = 1;
				}
				Vector2 position3 = npc.position;
				position3.X += npc.velocity.X;
				int num173 = (int)((position3.X + (float)(npc.width / 2) + (float)((npc.width / 2 + 1) * xVelDir)) / 16f); // x coord of left edge if moving left, right edge if moving right
				int num174 = (int)((position3.Y + (float)npc.height - 1f) / 16f); // y coord of bottom edge
				if (Main.tile[num173, num174] == null)
				{
					Main.tile[num173, num174] = new Tile();
				}
				if (Main.tile[num173, num174 - 1] == null)
				{
					Main.tile[num173, num174 - 1] = new Tile();
				}
				if (Main.tile[num173, num174 - 2] == null)
				{
					Main.tile[num173, num174 - 2] = new Tile();
				}
				if (Main.tile[num173, num174 - 3] == null)
				{
					Main.tile[num173, num174 - 3] = new Tile();
				}
				if (Main.tile[num173, num174 + 1] == null)
				{
					Main.tile[num173, num174 + 1] = new Tile();
				}
				if (Main.tile[num173 - xVelDir, num174 - 3] == null)
				{
					Main.tile[num173 - xVelDir, num174 - 3] = new Tile();
				}
				if ((float)(num173 * 16) < position3.X + (float)npc.width && (float)(num173 * 16 + 16) > position3.X && ((Main.tile[num173, num174].nactive() && !Main.tile[num173, num174].topSlope() && !Main.tile[num173, num174 - 1].topSlope() && Main.tileSolid[Main.tile[num173, num174].type] && !Main.tileSolidTop[Main.tile[num173, num174].type]) || (Main.tile[num173, num174 - 1].halfBrick() && Main.tile[num173, num174 - 1].nactive())) && (!Main.tile[num173, num174 - 1].nactive() || !Main.tileSolid[Main.tile[num173, num174 - 1].type] || Main.tileSolidTop[Main.tile[num173, num174 - 1].type] || (Main.tile[num173, num174 - 1].halfBrick() && (!Main.tile[num173, num174 - 4].nactive() || !Main.tileSolid[Main.tile[num173, num174 - 4].type] || Main.tileSolidTop[Main.tile[num173, num174 - 4].type]))) && (!Main.tile[num173, num174 - 2].nactive() || !Main.tileSolid[Main.tile[num173, num174 - 2].type] || Main.tileSolidTop[Main.tile[num173, num174 - 2].type]) && (!Main.tile[num173, num174 - 3].nactive() || !Main.tileSolid[Main.tile[num173, num174 - 3].type] || Main.tileSolidTop[Main.tile[num173, num174 - 3].type]) && (!Main.tile[num173 - xVelDir, num174 - 3].nactive() || !Main.tileSolid[Main.tile[num173 - xVelDir, num174 - 3].type]))
				{
					float num175 = num174 * 16; // vector y coord of bottom edge
					if (Main.tile[num173, num174].halfBrick())
					{
						num175 += 8f;
					}
					if (Main.tile[num173, num174 - 1].halfBrick())
					{
						num175 -= 8f;
					}
					if (num175 < position3.Y + (float)npc.height)
					{
						float num176 = position3.Y + (float)npc.height - num175;
						float num177 = 16.1f;
						if (num176 <= num177)
						{
							npc.gfxOffY += npc.position.Y + (float)npc.height - num175;
							npc.position.Y = num175 - (float)npc.height;
							if (num176 < 9f)
							{
								npc.stepSpeed = 1f;
							}
							else
							{
								npc.stepSpeed = 2f;
							}
						}
					}
				}
			}
			if (flag22)
			{ // if on solid ground
				int num178 = (int)((npc.position.X + (float)(npc.width / 2) + (float)(15 * npc.direction)) / 16f); // x of tile directly in front of them
				int num179 = (int)((npc.position.Y + (float)npc.height - 15f) / 16f); // y of bottommost tile they're occupying (but not what they're walking on)
				if (Main.tile[num178, num179] == null)
				{
					Main.tile[num178, num179] = new Tile();
				}
				if (Main.tile[num178, num179 - 1] == null)
				{
					Main.tile[num178, num179 - 1] = new Tile();
				}
				if (Main.tile[num178, num179 - 2] == null)
				{
					Main.tile[num178, num179 - 2] = new Tile();
				}
				if (Main.tile[num178, num179 - 3] == null)
				{
					Main.tile[num178, num179 - 3] = new Tile();
				}
				if (Main.tile[num178, num179 + 1] == null)
				{
					Main.tile[num178, num179 + 1] = new Tile();
				}
				if (Main.tile[num178 + npc.direction, num179 - 1] == null)
				{
					Main.tile[num178 + npc.direction, num179 - 1] = new Tile();
				}
				if (Main.tile[num178 + npc.direction, num179 + 1] == null)
				{
					Main.tile[num178 + npc.direction, num179 + 1] = new Tile();
				}
				if (Main.tile[num178 - npc.direction, num179 + 1] == null)
				{
					Main.tile[num178 - npc.direction, num179 + 1] = new Tile();
				}
				Main.tile[num178, num179 + 1].halfBrick();
				if (true)
				{
					int num181 = npc.spriteDirection;
					if (npc.type == 425)
					{
						num181 *= -1;
					}
					if ((npc.velocity.X < 0f && num181 == -1) || (npc.velocity.X > 0f && num181 == 1))
					{ // facing direction of movement
						if (npc.height >= 32 && Main.tile[num178, num179 - 2].nactive() && Main.tileSolid[Main.tile[num178, num179 - 2].type])
						{ // if two blocks tall, and tile (1 in front and 2 up) is active and solid
							if (Main.tile[num178, num179 - 3].nactive() && Main.tileSolid[Main.tile[num178, num179 - 3].type])
							{ // same as above, but 3 up
								npc.velocity.Y = -8f; // jump up
								npc.netUpdate = true;
							}
							else
							{
								npc.velocity.Y = -7f; // jump up not as high
								npc.netUpdate = true;
							}
						}
						else if (Main.tile[num178, num179 - 1].nactive() && Main.tileSolid[Main.tile[num178, num179 - 1].type])
						{ // a small jump, if tile 1x1 up and forward is there
							npc.velocity.Y = -6f;
							npc.netUpdate = true;
						}
						else if (npc.position.Y + (float)npc.height - (float)(num179 * 16) > 20f && Main.tile[num178, num179].nactive() && !Main.tile[num178, num179].topSlope() && Main.tileSolid[Main.tile[num178, num179].type])
						{
							npc.velocity.Y = -5f;
							npc.netUpdate = true;
						}
						else if (npc.directionY < 0 && npc.type != 67 && (!Main.tile[num178, num179 + 1].nactive() || !Main.tileSolid[Main.tile[num178, num179 + 1].type]) && (!Main.tile[num178 + npc.direction, num179 + 1].nactive() || !Main.tileSolid[Main.tile[num178 + npc.direction, num179 + 1].type]))
						{
							// jump across a pit like that
							npc.velocity.Y = -8f;
							npc.velocity.X *= 1.5f;
							npc.netUpdate = true;
						}
						if (npc.velocity.Y == 0f && stopped && npc.ai[3] == 1f)
						{
							npc.velocity.Y = -5f;
						}
						jumps++;
					}
				}
			}
			if (Main.netMode == 1 || npc.type != 120 || !(npc.ai[3] >= (float)num38))
			{
				return;
			}
		}

		public override void AI()
		{
			npc.ai[3]++;

			bool stopped = false;
			if (npc.velocity.X == 0f)
			{
				stopped = true;
			}
			else
			{
				jumps = 0;
			}
			if (npc.justHit)
			{
				stopped = false;
			}


			if (jumps > 2) // has been jumping in place and not getting closer
			{
				jumps = 0;
				if (npc.ai[3] < playerChaseThreshold + 1) // if chasing the player, make it not
				{
					npc.ai[3] = playerChaseThreshold;
				} else
                {
					npc.direction = -1 * npc.direction; // try other direction
				}
			}

			Vector2 targetPosition = new Vector2(0, 0);

			if (npc.ai[3] >= chaseReturnThreshold) // reset back to Player state
			{
				npc.ai[3] = 0;
			}
			if (npc.ai[3] < playerChaseThreshold) // on Player state
			{
				npc.TargetClosest(true);
				npc.FaceTarget();
				targetPosition = Main.player[npc.target].position; // chase the player

				float distance = Math.Abs(npc.position.X - Main.player[npc.target].position.X) + Math.Abs(npc.position.Y - Main.player[npc.target].position.Y);

				if (distance < 200f && npc.ai[2] > 200)
                {
					Vector2 dir = (Main.player[npc.target].position - npc.position);
					npc.velocity += Vector2.Normalize(dir) * 6f;
					npc.ai[2] = 0;
				} else if (distance < 500f)
                {
					npc.ai[2]++;
                }
				Main.NewText(npc.ai[2] + "");
			}

			if (npc.ai[3] >= playerChaseThreshold && npc.ai[3] < chaseReturnThreshold) // on Wander state
			{
				if (npc.ai[3] == playerChaseThreshold) // flip direction when it first hits threshold
				{
					//Teleport();
					//float distance = Math.Abs(npc.position.X - Main.player[npc.target].position.X) + Math.Abs(npc.position.Y - Main.player[npc.target].position.Y);
					//Vector2 dir = (Main.player[npc.target].position - npc.position);
					//npc.ai[3] = 0;
					//npc.velocity += Vector2.Normalize(dir) * 12f;
					//return;
					npc.direction = -1 * npc.direction;
				}
				targetPosition = (npc.direction == -1) ? new Vector2(0, 0) : new Vector2(Main.maxTilesX * 16, 0); // set the target position to far left or far right
			}
			//Main.NewText(jumps + " dir: " + npc.direction + " targ: " + targetPosition.X);


			// RUNNING TOWARD TARGET
			if (targetPosition.X < npc.position.X && npc.velocity.X > -1 * maxSpeed) // // IF the target is to my left AND I'm not at max "left" velocity
			{
				npc.velocity.X -= accelSpeed; // accelerate to the left
			}
			else if (targetPosition.X < npc.position.X && npc.velocity.X < -1 * maxSpeed) // too fast
			{
				npc.velocity.X += accelSpeed; // de
			}

			if (targetPosition.X > npc.position.X && npc.velocity.X < maxSpeed) // AND I'm not at max "right" velocity
			{
				npc.velocity.X += accelSpeed; // accelerate to the right
			}
			else if (targetPosition.X > npc.position.X && npc.velocity.X > maxSpeed) // too fast
			{
				npc.velocity.X -= accelSpeed; // de
			}

			int num38 = 60;
			bool flag4 = false;


			// changes ai[3]
			if (npc.velocity.Y == 0f && ((npc.velocity.X > 0f && npc.direction < 0) || (npc.velocity.X < 0f && npc.direction > 0)))
			{ // no vertical, but moving in way opposite their direction
				flag4 = true;
			}
			if (npc.position.X == npc.oldPosition.X || npc.ai[3] >= (float)num38 || flag4)
			{
				npc.ai[3] += 1f;
			}
			else if ((double)Math.Abs(npc.velocity.X) > 0.9 && npc.ai[3] > 0f)
			{
				npc.ai[3] -= 1f;
			}
			if (npc.ai[3] > (float)(num38 * 10))
			{
				npc.ai[3] = 0f;
			}
			if (npc.justHit)
			{
				npc.ai[3] = 0f;
			}
			if (npc.ai[3] == (float)num38)
			{
				npc.netUpdate = true;
			}


			bool flag22 = false;
			if (npc.velocity.Y == 0f)
			{
				int num168 = (int)(npc.position.Y + (float)npc.height + 7f) / 16; // y tile coord of its bottom
				int num169 = (int)npc.position.X / 16; // x tile coord of its left side
				int num170 = (int)(npc.position.X + (float)npc.width) / 16; // x tile coord of its right side
				for (int num171 = num169; num171 <= num170; num171++)
				{
					if (Main.tile[num171, num168] == null)
					{
						return;
					}
					if (Main.tile[num171, num168].nactive() && Main.tileSolid[Main.tile[num171, num168].type])
					{
						flag22 = true;
						break;
					}
				}
			}
			if (npc.velocity.Y >= 0f)
			{ // falling
				int xVelDir = 0;
				if (npc.velocity.X < 0f)
				{
					xVelDir = -1;
				}
				if (npc.velocity.X > 0f)
				{
					xVelDir = 1;
				}
				Vector2 position3 = npc.position;
				position3.X += npc.velocity.X;
				int num173 = (int)((position3.X + (float)(npc.width / 2) + (float)((npc.width / 2 + 1) * xVelDir)) / 16f); // x coord of left edge if moving left, right edge if moving right
				int num174 = (int)((position3.Y + (float)npc.height - 1f) / 16f); // y coord of bottom edge
				if (Main.tile[num173, num174] == null)
				{
					Main.tile[num173, num174] = new Tile();
				}
				if (Main.tile[num173, num174 - 1] == null)
				{
					Main.tile[num173, num174 - 1] = new Tile();
				}
				if (Main.tile[num173, num174 - 2] == null)
				{
					Main.tile[num173, num174 - 2] = new Tile();
				}
				if (Main.tile[num173, num174 - 3] == null)
				{
					Main.tile[num173, num174 - 3] = new Tile();
				}
				if (Main.tile[num173, num174 + 1] == null)
				{
					Main.tile[num173, num174 + 1] = new Tile();
				}
				if (Main.tile[num173 - xVelDir, num174 - 3] == null)
				{
					Main.tile[num173 - xVelDir, num174 - 3] = new Tile();
				}
				if ((float)(num173 * 16) < position3.X + (float)npc.width && (float)(num173 * 16 + 16) > position3.X && ((Main.tile[num173, num174].nactive() && !Main.tile[num173, num174].topSlope() && !Main.tile[num173, num174 - 1].topSlope() && Main.tileSolid[Main.tile[num173, num174].type] && !Main.tileSolidTop[Main.tile[num173, num174].type]) || (Main.tile[num173, num174 - 1].halfBrick() && Main.tile[num173, num174 - 1].nactive())) && (!Main.tile[num173, num174 - 1].nactive() || !Main.tileSolid[Main.tile[num173, num174 - 1].type] || Main.tileSolidTop[Main.tile[num173, num174 - 1].type] || (Main.tile[num173, num174 - 1].halfBrick() && (!Main.tile[num173, num174 - 4].nactive() || !Main.tileSolid[Main.tile[num173, num174 - 4].type] || Main.tileSolidTop[Main.tile[num173, num174 - 4].type]))) && (!Main.tile[num173, num174 - 2].nactive() || !Main.tileSolid[Main.tile[num173, num174 - 2].type] || Main.tileSolidTop[Main.tile[num173, num174 - 2].type]) && (!Main.tile[num173, num174 - 3].nactive() || !Main.tileSolid[Main.tile[num173, num174 - 3].type] || Main.tileSolidTop[Main.tile[num173, num174 - 3].type]) && (!Main.tile[num173 - xVelDir, num174 - 3].nactive() || !Main.tileSolid[Main.tile[num173 - xVelDir, num174 - 3].type]))
				{
					float num175 = num174 * 16; // vector y coord of bottom edge
					if (Main.tile[num173, num174].halfBrick())
					{
						num175 += 8f;
					}
					if (Main.tile[num173, num174 - 1].halfBrick())
					{
						num175 -= 8f;
					}
					if (num175 < position3.Y + (float)npc.height)
					{
						float num176 = position3.Y + (float)npc.height - num175;
						float num177 = 16.1f;
						if (num176 <= num177)
						{
							npc.gfxOffY += npc.position.Y + (float)npc.height - num175;
							npc.position.Y = num175 - (float)npc.height;
							if (num176 < 9f)
							{
								npc.stepSpeed = 1f;
							}
							else
							{
								npc.stepSpeed = 2f;
							}
						}
					}
				}
			}
			if (flag22)
			{ // if on solid ground
				int num178 = (int)((npc.position.X + (float)(npc.width / 2) + (float)(15 * npc.direction)) / 16f); // x of tile directly in front of them
				int num179 = (int)((npc.position.Y + (float)npc.height - 15f) / 16f); // y of bottommost tile they're occupying (but not what they're walking on)
				if (Main.tile[num178, num179] == null)
				{
					Main.tile[num178, num179] = new Tile();
				}
				if (Main.tile[num178, num179 - 1] == null)
				{
					Main.tile[num178, num179 - 1] = new Tile();
				}
				if (Main.tile[num178, num179 - 2] == null)
				{
					Main.tile[num178, num179 - 2] = new Tile();
				}
				if (Main.tile[num178, num179 - 3] == null)
				{
					Main.tile[num178, num179 - 3] = new Tile();
				}
				if (Main.tile[num178, num179 + 1] == null)
				{
					Main.tile[num178, num179 + 1] = new Tile();
				}
				if (Main.tile[num178 + npc.direction, num179 - 1] == null)
				{
					Main.tile[num178 + npc.direction, num179 - 1] = new Tile();
				}
				if (Main.tile[num178 + npc.direction, num179 + 1] == null)
				{
					Main.tile[num178 + npc.direction, num179 + 1] = new Tile();
				}
				if (Main.tile[num178 - npc.direction, num179 + 1] == null)
				{
					Main.tile[num178 - npc.direction, num179 + 1] = new Tile();
				}
				Main.tile[num178, num179 + 1].halfBrick();
				if (true)
				{
					int num181 = npc.spriteDirection;
					if (npc.type == 425)
					{
						num181 *= -1;
					}
					if ((npc.velocity.X < 0f && num181 == -1) || (npc.velocity.X > 0f && num181 == 1))
					{ // facing direction of movement
						if (npc.height >= 32 && Main.tile[num178, num179 - 2].nactive() && Main.tileSolid[Main.tile[num178, num179 - 2].type])
						{ // if two blocks tall, and tile (1 in front and 2 up) is active and solid
							if (Main.tile[num178, num179 - 3].nactive() && Main.tileSolid[Main.tile[num178, num179 - 3].type])
							{ // same as above, but 3 up
								npc.velocity.Y = -8f; // jump up
								npc.netUpdate = true;
							}
							else
							{
								npc.velocity.Y = -7f; // jump up not as high
								npc.netUpdate = true;
							}
						}
						else if (Main.tile[num178, num179 - 1].nactive() && Main.tileSolid[Main.tile[num178, num179 - 1].type])
						{ // a small jump, if tile 1x1 up and forward is there
							npc.velocity.Y = -6f;
							npc.netUpdate = true;
						}
						else if (npc.position.Y + (float)npc.height - (float)(num179 * 16) > 20f && Main.tile[num178, num179].nactive() && !Main.tile[num178, num179].topSlope() && Main.tileSolid[Main.tile[num178, num179].type])
						{
							npc.velocity.Y = -5f;
							npc.netUpdate = true;
						}
						else if (npc.directionY < 0 && npc.type != 67 && (!Main.tile[num178, num179 + 1].nactive() || !Main.tileSolid[Main.tile[num178, num179 + 1].type]) && (!Main.tile[num178 + npc.direction, num179 + 1].nactive() || !Main.tileSolid[Main.tile[num178 + npc.direction, num179 + 1].type]))
						{
							// jump across a pit like that
							npc.velocity.Y = -8f;
							npc.velocity.X *= 1.5f;
							npc.netUpdate = true;
						}
						if (npc.velocity.Y == 0f && stopped && npc.ai[3] == 1f)
						{
							npc.velocity.Y = -5f;
						}
						jumps++;
					}
				}
			}
			if (Main.netMode == 1 || npc.type != 120 || !(npc.ai[3] >= (float)num38))
			{
				return;
			}
		}


        public override void ModifyHitByItem(Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
			if (npc.ai[3] > 100)
			{
				Main.NewText("what");
				int dir = (npc.Center.X < player.Center.X) ? -1 : 1;
				npc.velocity.X = 0;
				npc.velocity += new Vector2(dir * 6f, 0);
				for (int n = 0; n < 20; n++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 31);

				}
				npc.ai[3] = 0;
				damage = 0;
				knockback = 0;
				npc.alpha = 200;
			}
		}

        private void Teleport() {
			Main.NewTextMultiline("here!");
			int playerXtile = (int)Main.player[npc.target].position.X / 16;
			int playerYtile = (int)Main.player[npc.target].position.Y / 16;
			int npcXtile = (int)npc.position.X / 16;
			int npcYtile = (int)npc.position.Y / 16;
			int randomVariance = 20;
			int iterations = 0;
			bool toofar = false;
			if (Math.Abs(npc.position.X - Main.player[npc.target].position.X) + Math.Abs(npc.position.Y - Main.player[npc.target].position.Y) > 2000f)
			{
				iterations = 100;
				toofar = true;
			}
			while (!toofar && iterations < 100)
			{
				iterations++;
				int randX = Main.rand.Next(playerXtile - randomVariance, playerXtile + randomVariance);
				for (int randY = Main.rand.Next(playerYtile - randomVariance, playerYtile + randomVariance); randY < playerYtile + randomVariance; randY++)
				{
					if ((randY < playerYtile - 4 || randY > playerYtile + 4 || randX < playerXtile - 4 || randX > playerXtile + 4) && (randY < npcYtile - 1 || randY > npcYtile + 1 || randX < npcXtile - 1 || randX > npcXtile + 1) && Main.tile[randX, randY].nactive())
					{
						bool passChecks = true;
						if (npc.type == 32 && Main.tile[randX, randY - 1].wall == 0)
						{ // dark caster
							passChecks = false;
						}
						else if (Main.tile[randX, randY - 1].lava())
						{
							passChecks = false;
						}
						if (passChecks && Main.tileSolid[Main.tile[randX, randY].type] && !Collision.SolidTiles(randX - 1, randX + 1, randY - 4, randY - 1))
						{
							npc.position.X = randX * 16 - npc.width / 2;
							npc.position.Y = randY * 16 - npc.height;
							npc.netUpdate = true;
							npc.ai[3] = 0f;
							Main.NewTextMultiline("WOW!");
						}
					}
				}
			}
		}
		// Here in FindFrame, we want to set the animation frame our npc will use depending on what it is doing.
		// We set npc.frame.Y to x * frameHeight where x is the xth frame in our spritesheet, counting from 0. For convenience, I have defined some consts above.
		public override void FindFrame(int frameHeight)
		{

			/**
			// This makes the sprite flip horizontally in conjunction with the npc.direction.
			npc.spriteDirection = npc.direction;

			// For the most part, our animation matches up with our states.
			if (AI_State == State_Asleep)
			{
				// npc.frame.Y is the goto way of changing animation frames. npc.frame starts from the top left corner in pixel coordinates, so keep that in mind.
				npc.frame.Y = Frame_Asleep * frameHeight;
			}
			else if (AI_State == State_Notice)
			{
				npc.frameCounter++;
				if (npc.frameCounter < 5)
				{
					npc.frame.Y = Frame_Move_1 * frameHeight;
				}
				else if (npc.frameCounter < 10)
				{
					npc.frame.Y = Frame_Move_2 * frameHeight;
				}
				else if (npc.frameCounter < 15)
				{
					npc.frame.Y = Frame_Move_3 * frameHeight;
				}
				else if (npc.frameCounter < 20)
				{
					npc.frame.Y = Frame_Move_4 * frameHeight;
				}
				else
				{
					npc.frameCounter = 0;
				}
			}
			**/
			npc.spriteDirection = npc.direction;

			npc.frameCounter++;
			if (npc.frameCounter < 5)
			{
				npc.frame.Y = Frame_Move_1 * frameHeight;
			}
			else if (npc.frameCounter < 10)
			{
				npc.frame.Y = Frame_Move_2 * frameHeight;
			}
			else if (npc.frameCounter < 15)
			{
				npc.frame.Y = Frame_Move_3 * frameHeight;
			}
			else if (npc.frameCounter < 20)
			{
				npc.frame.Y = Frame_Move_4 * frameHeight;
			}
			else
			{
				npc.frameCounter = 0;
			}

		}
	}
}
