using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using BasicMod.Projectiles;
using Microsoft.Xna.Framework.Graphics;

namespace BasicMod.Items.MeleeWeapons.Boomerangs
{
	public class MagicalBoomerang : ModItem
	{
		public override string Texture => "Terraria/Item_" + ItemID.EnchantedBoomerang;
		public override void SetStaticDefaults()
		{

			Tooltip.SetDefault("An enhanced Enchanted Boomerang. Homes in on foes.");
		}
		public override void SetDefaults()
		{
			item.damage = 80;
			item.melee = true;
			item.width = 30;
			item.height = 30;
			item.useTime = 12;
			item.useAnimation = 12;
			item.noUseGraphic = true;
			item.useStyle = 1;
			item.knockBack = 7;
			item.value = 8;
			item.rare = ItemRarityID.Orange;
			item.shootSpeed = 16f;
			item.shoot = mod.ProjectileType("MagicalBoomerangProjectile");
			item.UseSound = SoundID.Item1;
			item.autoReuse = false;
			item.noMelee = true;
		}

		public int boomerangCount = 0;
		public int maxBooms = 3;


		public override bool CanUseItem(Player player)       //this make that you can shoot only 1 boomerang at once
		{
			//TODO: Check for boomerang mitt

			boomerangCount = 0;
			for (int i = 0; i < 1000; ++i)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == item.shoot)
				{
					boomerangCount++;
					
				}
				if (boomerangCount > maxBooms)
                {
					return false;
				}
			}
			return true;
		}

		public override void AddRecipes()
		{
            ModRecipe recipe = new ModRecipe(mod);
            // TODO: add ingredient droppable from new enemy?
            recipe.AddIngredient(ItemID.PixieDust, 10);
			recipe.AddIngredient(ItemID.EnchantedBoomerang);
            recipe.SetResult(this);
            recipe.AddRecipe();
		}
	}

	class MagicalBoomerangProjectile : ModProjectile
	{
		public override string Texture => "Terraria/Projectile_" + ProjectileID.EnchantedBoomerang;

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 3;    //The length of old position to be recorded
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;        //The recording mode
		}

		public override void SetDefaults()
		{
			projectile.width = 30;
			projectile.height = 30;
			projectile.aiStyle = -1;
			projectile.friendly = true;
			projectile.melee = true;
			projectile.magic = false;
			projectile.penetrate = -1;
			projectile.timeLeft = 600;
			projectile.light = 0.6f;

		}


		bool homeIn = true; // to make it stop homing, try turning this off in OnHit

		float vel = 16f;
		float turnAccel = 2f;

		float homingVel = 16f;
		float homingTurnAccel = 0.8f;

		public override void AI()
		{
			// dusts
			if (Main.rand.Next(5) == 0)
			{
				int num27 = Main.rand.Next(3);
				if (num27 == 0)
                {
					num27 = 15;
                } else if (num27 == 1)
				{
					num27 = 57;
				} else
                {
					num27 = 58;
                }
				Dust.NewDust(projectile.position, projectile.width, projectile.height, num27, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 150, default(Color), 0.7f);
			}
			if (projectile.ai[0] == 0f)
			{
				projectile.ai[1] += 1f;

				if (homeIn)
				{
					if (Main.rand.Next(2) == 0)
					{
						int num29 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 57, 0f, 0f, 255, default(Color), 0.75f);
						Dust dust12 = Main.dust[num29];
						Dust dust2 = dust12;
						dust2.velocity *= 0.1f;
						Main.dust[num29].noGravity = true;
					}
					if (projectile.velocity.X > 0f)
					{
						projectile.spriteDirection = 1;
					}
					else if (projectile.velocity.X < 0f)
					{
						projectile.spriteDirection = -1;
					}
					float num30 = projectile.position.X;
					float num31 = projectile.position.Y;
					float num32 = 1000f;
					bool flag = false; // has a target
					if (projectile.ai[1] > 10f)
					{
						for (int num33 = 0; num33 < 200; num33++)
						{ // cycles through potential npcs
							if (Main.npc[num33].CanBeChasedBy(projectile))
							{
								float num34 = Main.npc[num33].position.X + (float)(Main.npc[num33].width / 2);
								float num35 = Main.npc[num33].position.Y + (float)(Main.npc[num33].height / 2);
								float num36 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num34) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num35);
								if (num36 < 800f && Collision.CanHit(new Vector2(projectile.position.X + (float)(projectile.width / 2), projectile.position.Y + (float)(projectile.height / 2)), 1, 1, Main.npc[num33].position, Main.npc[num33].width, Main.npc[num33].height))
								{
									num32 = num36;
									num30 = num34;
									num31 = num35;
									flag = true;
								}
							}
						}
					}
					if (!flag)
					{ // if it doesn't have a target (do normal thing)
						num30 = projectile.position.X + (float)(projectile.width / 2) + projectile.velocity.X * 100f;
						num31 = projectile.position.Y + (float)(projectile.height / 2) + projectile.velocity.Y * 100f;
						if (projectile.ai[1] >= 30f)
						{
							projectile.ai[0] = 1f;
							projectile.ai[1] = 0f;
							projectile.netUpdate = true;
						}
					}

					// similar code to bottom - but instead of homing on the player, it's on an enemy, right here.
					float num37 = homingVel; // topspeed of homing
					float num38 = homingTurnAccel; // turning accaleration of homing

					Vector2 vector = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
					float num39 = num30 - vector.X;
					float num40 = num31 - vector.Y;
					float num41 = (float)Math.Sqrt(num39 * num39 + num40 * num40);
					float num42 = num41;
					num41 = num37 / num41;
					num39 *= num41;
					num40 *= num41;
					if (projectile.velocity.X < num39)
					{
						projectile.velocity.X += num38;
						if (projectile.velocity.X < 0f && num39 > 0f)
						{
							projectile.velocity.X += num38 * 2f;
						}
					}
					else if (projectile.velocity.X > num39)
					{
						projectile.velocity.X -= num38;
						if (projectile.velocity.X > 0f && num39 < 0f)
						{
							projectile.velocity.X -= num38 * 2f;
						}
					}
					if (projectile.velocity.Y < num40)
					{
						projectile.velocity.Y += num38;
						if (projectile.velocity.Y < 0f && num40 > 0f)
						{
							projectile.velocity.Y += num38 * 2f;
						}
					}
					else if (projectile.velocity.Y > num40)
					{
						projectile.velocity.Y -= num38;
						if (projectile.velocity.Y > 0f && num40 < 0f)
						{
							projectile.velocity.Y -= num38 * 2f;
						}
					}
				}
				else if (projectile.ai[1] >= 30f)
				{
					projectile.ai[0] = 1f;
					projectile.ai[1] = 0f;
					projectile.netUpdate = true;
				}
			}
			else
			{
				projectile.tileCollide = false;
				float num43 = vel; // speed
				float num44 = turnAccel; // turning acceleration

				Vector2 vector2 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f); // position of boomerang
				float num45 = Main.player[projectile.owner].position.X + (float)(Main.player[projectile.owner].width / 2) - vector2.X; // x of player, minus the x of boom
				float num46 = Main.player[projectile.owner].position.Y + (float)(Main.player[projectile.owner].height / 2) - vector2.Y; // y of player, minus the y of boom

				// if too far away, kill
				float num47 = (float)Math.Sqrt(num45 * num45 + num46 * num46);
				if (num47 > 3000f)
				{
					projectile.Kill();
				}
				num47 = num43 / num47;
				num45 *= num47;
				num46 *= num47;

				if (projectile.velocity.X < num45)
				{
					projectile.velocity.X += num44;
					if (projectile.velocity.X < 0f && num45 > 0f)
					{
						projectile.velocity.X += num44;
					}
				}
				else if (projectile.velocity.X > num45)
				{
					projectile.velocity.X -= num44;
					if (projectile.velocity.X > 0f && num45 < 0f)
					{
						projectile.velocity.X -= num44;
					}
				}
				if (projectile.velocity.Y < num46)
				{
					projectile.velocity.Y += num44;
					if (projectile.velocity.Y < 0f && num46 > 0f)
					{
						projectile.velocity.Y += num44;
					}
				}
				else if (projectile.velocity.Y > num46)
				{
					projectile.velocity.Y -= num44;
					if (projectile.velocity.Y > 0f && num46 < 0f)
					{
						projectile.velocity.Y -= num44;
					}
				}

				if (Main.myPlayer == projectile.owner)
				{ // kills projectile on return to player
					Rectangle rectangle = new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height);
					Rectangle value2 = new Rectangle((int)Main.player[projectile.owner].position.X, (int)Main.player[projectile.owner].position.Y, Main.player[projectile.owner].width, Main.player[projectile.owner].height);
					if (rectangle.Intersects(value2))
					{
						projectile.Kill();
					}
				}
			}

			// rotations
			projectile.rotation += 0.5f * (float)projectile.direction;
		}

		public override void OnHitNPC(NPC targetNpc, int damage, float knockback, bool crit)
		{
			homeIn = false;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			//Redraw the projectile with the color not influenced by light
			Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
			for (int k = 0; k < projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
				Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
				spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
			}
			return true;
		}

		public override void Kill(int timeLeft)
		{
			// This code and the similar code above in OnTileCollide spawn dust from the tiles collided with. SoundID.Item10 is the bounce sound you hear.
			Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
			Main.PlaySound(SoundID.Item10, projectile.position);
		}

		// allows a rebound
		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
			Main.PlaySound(SoundID.Item10, projectile.position);
			
			if (projectile.velocity.X != oldVelocity.X)
			{
				projectile.velocity.X = -oldVelocity.X;
			}
			if (projectile.velocity.Y != oldVelocity.Y)
			{
				projectile.velocity.Y = -oldVelocity.Y;
			}
			projectile.ai[0] = 1;
			return false;
		}
	}
}
