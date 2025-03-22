using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BasicMod.NPCs
{
	public class AngryStar : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[npc.type] = 2;
		}

		public override void SetDefaults()
		{
			npc.width = 45;
			npc.height = 45;

			npc.damage = 40;
			npc.defense = 15;
			npc.lifeMax = 500;

			npc.noTileCollide = false;
			npc.noGravity = true;
			npc.HitSound = SoundID.NPCHit4;
			npc.DeathSound = SoundID.NPCDeath14;
			npc.value = 6000f;
			npc.knockBackResist = 0f;
			npc.aiStyle = -1;
			npc.buffImmune[BuffID.Confused] = false;
			//banner = npc.type;
			//bannerItem = mod.ItemType("VelocichopperBanner");
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			if (npc.life < 0)
            {
				for (int i = 0; i < 25; i++)
				{
					Vector2 totalVel = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * 5f;
					int num62 = Gore.NewGore(npc.Center, totalVel, 16);
					Gore gore5 = Main.gore[num62];
					Gore gore2 = gore5;
					gore2.velocity *= 0.66f;
					gore5 = Main.gore[num62];
					gore2 = gore5;
					gore2.velocity += npc.velocity * 0.3f;
				}
			} else {
				for (int i = 0; i < 3; i++)
				{
					Vector2 totalVel = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * 2f;
					int num62 = Gore.NewGore(npc.Center, totalVel, 16);
					Gore gore5 = Main.gore[num62];
					Gore gore2 = gore5;
					gore2.velocity *= 0.66f;
					gore5 = Main.gore[num62];
					gore2 = gore5;
					gore2.velocity += npc.velocity * 0.3f;
				}
			}
			
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{

			if (Main.hardMode)
            {
				return SpawnCondition.OverworldNight.Chance * 0.075f;
            }
			return 0f;
		}

		private float shootAttack
		{
			get => npc.ai[3];
			set => npc.ai[3] = value;
		}


		protected float speed = 1.5f;
		protected float acceleration = 0.1f;
		protected float speedY = 1.5f;
		protected float accelerationY = 0.08f;

		int maxTileHover = 30;
		int minTileHover = 12;

		public int Reload_Timer = 0;
		public int reloadTime = 360;

		float trueSpeed = 5f;

		float maxShootTime = 900;

		float minShootTime = 500;

		public override void AI()
		{
			CreateDusts();
			if (shootAttack > minShootTime)
			{
				npc.velocity = Vector2.Zero;
				npc.rotation += 0.5f;
				shootAttack--;

				if (shootAttack % 10f == 0 && Main.netMode != 1)
				{
					Vector2 shootPosition = npc.Center;
					Vector2 totalVel = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * trueSpeed;
					Projectile.NewProjectile(shootPosition, totalVel, mod.ProjectileType("AngryStarMiniStar"), npc.damage / 3, 3f, Main.myPlayer);
					npc.netUpdate = true;
				}
				return;
			}
			else
			{
				npc.rotation = 0f;
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
				Vector2 shootPosition = npc.Center;
				Vector2 totalVel = Vector2.Normalize(Main.player[npc.target].Center - shootPosition) * trueSpeed;
				totalVel = Utils.RotatedByRandom(totalVel, MathHelper.PiOver4 / 3);

				//Projectile.NewProjectile(npc.Center.X + (npc.width/2 * npc.direction), npc.position.Y + npc.height, 5.00f * npc.direction, 5.00f, ProjectileID.SmokeBomb, npc.damage, 3f, Main.myPlayer);
				Projectile.NewProjectile(shootPosition, totalVel, mod.ProjectileType("AngryStarMiniStar"), npc.damage / 2, 3f, Main.myPlayer);

				Reload_Timer = 0;
				npc.netUpdate = true;
			}

			float distance = Math.Abs(npc.position.X - Main.player[npc.target].position.X) + Math.Abs(npc.position.Y - Main.player[npc.target].position.Y);

			if (distance < 500 && shootAttack <= 0)
			{
				shootAttack = maxShootTime;
				npc.velocity = Vector2.Zero;
			}

			if (shootAttack > 0)
			{
				shootAttack--;
			}


		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(Reload_Timer);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			Reload_Timer = reader.ReadInt32();
		}

		private void CreateDusts()
		{
			int spinning = (shootAttack > minShootTime && shootAttack < maxShootTime) ? 2 : 1;

			if (Main.rand.Next(16 / spinning) == 0)
			{
				Vector2 value3 = Vector2.UnitX.RotatedByRandom(1.5707963705062866).RotatedBy(npc.velocity.ToRotation());
				int num61 = Dust.NewDust(npc.position, npc.width, npc.height, 58, npc.velocity.X * 0.5f * spinning, npc.velocity.Y * 0.5f * spinning, 150, default(Color), 1.2f);
				Main.dust[num61].velocity = value3 * 0.66f;
				Main.dust[num61].position = npc.Center + value3 * 12f;
			}
			if (Main.rand.Next(48 / spinning) == 0)
			{
				int num62 = Gore.NewGore(npc.Center, new Vector2(npc.velocity.X * 0.2f, npc.velocity.Y * 0.2f), 16);
				Gore gore5 = Main.gore[num62];
				Gore gore2 = gore5;
				gore2.velocity *= 0.66f;
				gore5 = Main.gore[num62];
				gore2 = gore5;
				gore2.velocity += npc.velocity * 0.3f;
			}
		}

		public override void FindFrame(int frameHeight)
		{
			// This makes the sprite flip horizontally in conjunction with the npc.direction.
			npc.spriteDirection = npc.direction;
			if (Main.GameUpdateCount % 25 == 0)
			{
				npc.frameCounter++;
			}
			if (npc.frameCounter < 1)
			{
				npc.frame.Y = 0 * frameHeight;
			}
			else if (npc.frameCounter < 2)
			{
				npc.frame.Y = 1 * frameHeight;
			}
			else
			{
				npc.frameCounter = 0;
			}
		}

		public override void NPCLoot()
		{
			Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.FallenStar, Main.rand.Next(2,5));
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

	public class AngryStarMiniStar : ModProjectile
	{
		public override void SetDefaults()
		{
			projectile.width = 10;               //The width of projectile hitbox
			projectile.height = 10;              //The height of projectile hitbox
			projectile.aiStyle = -1;             //The ai style of the projectile, please reference the source code of Terraria
			projectile.friendly = false;         //Can the projectile deal damage to enemies?
			projectile.hostile = true;         //Can the projectile deal damage to the player?
			projectile.timeLeft = 600;          //The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			projectile.alpha = 0;             //The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
			projectile.light = 0.5f;            //How much light emit around the projectile
			projectile.tileCollide = true;          //Can the projectile collide with tiles?
		}

        public override void AI()
        {
			if (Main.rand.Next(20) == 0)
			{
				Vector2 value3 = Vector2.UnitX.RotatedByRandom(1.5707963705062866).RotatedBy(projectile.velocity.ToRotation());
				int num61 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 58, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 150, default(Color), 1.2f);
				Main.dust[num61].velocity = value3 * 0.66f;
				Main.dust[num61].position = projectile.Center + value3 * 12f;
			}

			projectile.rotation += 0.5f;
		}

        public override void Kill(int timeLeft)
		{
			// This code and the similar code above in OnTileCollide spawn dust from the tiles collided with. SoundID.Item10 is the bounce sound you hear.
			Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
		}
	}
}