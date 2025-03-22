using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using BasicMod.Projectiles;
using Microsoft.Xna.Framework.Graphics;

namespace BasicMod.Items.MeleeWeapons.Boomerangs
{
	public class Skelerang : ModItem
	{
		public override void SetStaticDefaults()
		{

			Tooltip.SetDefault("It seems to be moving.");
		}
		public override void SetDefaults()
		{
			item.damage = 28;
			item.melee = true;
			item.width = 32;
			item.height = 32;
			item.useTime = 18;
			item.useAnimation = 18;
			item.noUseGraphic = true;
			item.useStyle = 1;
			item.knockBack = 9;
			item.value = 10000;
			item.rare = ItemRarityID.Orange;
			item.shootSpeed = 13f;
			item.shoot = mod.ProjectileType("SkelerangProjectile");
			item.UseSound = SoundID.Item1;
			item.autoReuse = false;
			item.noMelee = true;
		}

		public override bool CanUseItem(Player player)       //this make that you can shoot only 1 boomerang at once
		{
			for (int i = 0; i < 1000; ++i)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == item.shoot)
				{
					return false;
				}
			}
			return true;
		}

	}

	class SkelerangProjectile : ModProjectile
	{

		public override void SetDefaults()
		{
			projectile.width = 20;
			projectile.height = 20;
			projectile.aiStyle = -1;
			projectile.friendly = true;
			projectile.melee = true;
			projectile.magic = false;
			projectile.penetrate = -1;
			projectile.timeLeft = 600;

		}


		bool homeIn = true; // to make it stop homing, try turning this off in OnHit

		float vel = 15f;
		float turnAccel = 1.5f;

		float homingVel = 16f;
		float homingTurnAccel = 0.8f;

		public override void AI()
		{
			if (projectile.ai[0] == 0f)
			{
				projectile.ai[1] += 1f;

				if (projectile.ai[1] >= 30f)
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
			Vector2 oldVelocity = projectile.velocity;
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
