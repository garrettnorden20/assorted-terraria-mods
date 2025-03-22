using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using BasicMod.Projectiles;

namespace BasicMod.Items.MeleeWeapons
{
	public class TestingBoomerang : ModItem
	{
		public override string Texture => "Terraria/Item_" + ItemID.InletPump;
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("its a test");
		}
		public override void SetDefaults()
		{
			item.damage = 20;
			item.melee = true;
			item.width = 30;
			item.height = 30;
			item.useTime = 15;
			item.useAnimation = 15;
			item.noUseGraphic = true;
			item.useStyle = 1;
			item.knockBack = 7;
			item.value = 8;
			item.rare = ItemRarityID.Orange;
			item.shootSpeed = 10f;
			item.shoot = mod.ProjectileType("ArcShell");
			item.UseSound = SoundID.Item1;
			item.autoReuse = false;
		}
		public override bool CanUseItem(Player player)       //this make that you can shoot only 1 boomerang at once
		{
			/**
			for (int i = 0; i < 1000; ++i)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == item.shoot)
				{
					return false;
				}
			}
			**/
			return true;
		}

		public override void AddRecipes()
		{
			/**
            ModRecipe recipe = new ModRecipe(mod);
            // TODO: add ingredient droppable from new enemy?
            recipe.AddIngredient(ModContent.ItemType<Ores.SeleniumBar>(), 10);
            recipe.SetResult(this);
            recipe.AddRecipe();
            **/
		}
	}


	class ArcShell : ModProjectile
	{
		public override string Texture => "Terraria/Projectile_" + ProjectileID.Bananarang;

		public override void SetDefaults()
		{
			projectile.width = 30;
			projectile.height = 30;
			projectile.aiStyle = -1;
			projectile.friendly = true;
			projectile.melee = true;
			projectile.magic = false;
			projectile.penetrate = -1;
			projectile.timeLeft = 300;
			projectile.light = 0.5f;

		}

		Vector2 startPos = Vector2.Zero;
		Vector2 target = Vector2.Zero;
		Vector2 arcPoint = Vector2.Zero;
		float speed = 10;
		float arcHeight = 1;
		public override void AI()
		{
			projectile.ai[0] += 1f;
			if (projectile.owner == Main.myPlayer && projectile.ai[0] == 1)
			{
				projectile.velocity = Vector2.Zero;
				startPos = projectile.position;
				target = Main.MouseWorld;

				projectile.netUpdate = true;
			}

			float x0 = startPos.X;
			float x1 = target.X;
			float dist = x1 - x0;
			float nextX = MathHelper.Lerp(projectile.position.X, x1, 0.1f);
			float baseY = MathHelper.Lerp(startPos.Y, target.Y, (nextX - x0) / dist);
			float arc = arcHeight * (nextX - x0) * (nextX - x1) / (-0.25f * dist * dist);
			Vector2 nextPos = new Vector2(nextX, baseY + arc);

			// Rotate to face the next position, and then move there
			projectile.position = nextPos;

			Main.NewText("player: " + Main.player[projectile.owner].position.X + "," + Main.player[projectile.owner].position.Y);
			Main.NewText("arc: " + projectile.position.X + "," + projectile.position.Y);
			// Do something when we reach the target
			//if (nextPos == targetPos) Arrived();


		}
	}

	class TestingProjectile : ModProjectile
	{
		public override string Texture => "Terraria/Projectile_" + ProjectileID.Bananarang;

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
			projectile.light = 0.5f;

		}


		bool homeIn = true; // to make it stop homing, try turning this off in OnHit

		float vel = 10f;
		float turnAccel = 3f;

		float homingVel = 10f;
		float homingTurnAccel = 0.8f;

		public override void AI()
		{
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
			projectile.rotation += 0.4f * (float)projectile.direction;
		}

		public override void OnHitNPC(NPC targetNpc, int damage, float knockback, bool crit)
		{
			homeIn = false;
		}
	}
}
