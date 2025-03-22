using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace BasicMod.Projectiles
{
	// The following laser shows a channeled ability, after charging up the laser will be fired
	// Using custom drawing, dust effects, and custom collision checks for tiles
	public class MagicShield : ModProjectile
	{

		private const int AI_State_Slot = 0;

		private const int State_Absorbing = 0;
		private const int State_Release = 1;
		private const int State_Attack_2 = 2;

		public float AI_State
		{
			get => projectile.ai[AI_State_Slot];
			set => projectile.ai[AI_State_Slot] = value;
		}


		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;    //The length of old position to be recorded
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;        //The recording mode
			drawOriginOffsetY = -4;
			drawOffsetX = 6;
		}

		public override void SetDefaults()
		{
			projectile.width = 36;
			projectile.height = 36;
			projectile.friendly = true;
			projectile.penetrate = -1;
			projectile.tileCollide = false;
			projectile.magic = true;
			projectile.hide = false;
		}


		// Set custom immunity time on hitting an NPC
		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.immune[projectile.owner] = 5;
		}


		// The AI of the projectile
		public override void AI()
		{
			Player player = Main.player[projectile.owner];
			projectile.velocity = Vector2.Zero;
			projectile.ai[1] += 1;
			if (projectile.ai[1] == 1)
            {
				savedProjs = new ArrayList();
				AI_State = State_Absorbing;
				projToShoot = 0;
            }


			UpdatePlayer(player);
			
			if (AI_State == State_Absorbing)
            {
				//Main.NewText("else if State_Absorbing");
				if (projectile.ai[1] % 2 == 0)
				{
					FindEnemyProjectile();
				}
				ReleaseCase(player);
			} 
			
			else if (AI_State == State_Release)
            {
				
				if (projectile.ai[1] % 4 == 0)
				{
					//Main.NewText("unleash");
					UnleashProjectiles(player);
				}
			}


			SpawnDusts(player);
			CastLights();



			// By separating large AI into methods it becomes very easy to see the flow of the AI in a broader sense
			// First we update player variables that are needed to channel the laser
			// Then we run our charging laser logic
			// If we are fully charged, we proceed to update the laser's position
			// Finally we spawn some effects like dusts and light




			// If laser is not charged yet, stop the AI here.


		}

		private void SpawnDusts(Player player)
		{
			for (int i = 0; i < 1; ++i)
			{
				Dust.NewDust(projectile.position, projectile.width, projectile.height, 204);

			}
		}

		private void ReleaseCase(Player player)
		{
			// Kill the projectile if the player stops channeling
			if (!player.channel)
			{
				AI_State = State_Release;

			}
			else
			{
				// Do we still have enough mana? If not, we kill the projectile because we cannot use it anymore
				if (Main.time % 10 < 1 && !player.CheckMana(player.inventory[player.selectedItem].mana, true))
				{
					projectile.Kill();
				}
			}
		}

		// note; only gets enemy projectiles, not "projectiles" which are actually NPCs. 
		private void FindEnemyProjectile()
        {
			for (int i = 0; i < Main.projectile.Length; i++)
            {
				if (Main.projectile[i].hostile  &&  Vector2.Distance(Main.projectile[i].position, projectile.position) < 40f)
                {
					//Main.NewText( i + " Added Proj: Owner: " + Main.projectile[i].owner + " Identity: " + Main.projectile[i].identity);
					savedProjs.Add(Main.projectile[i].type);
					projToShoot++;


					for (int n = 0; n < 20; n++)
					{
						Dust.NewDust(projectile.position, projectile.width, projectile.height, 31);

					}


					Main.projectile[i].Kill();

					
				}
            }

        }

		private void UnleashProjectiles(Player player)
        {
			if (projToShoot > 30)
            {
				projToShoot = 30;
            }
			if (projToShoot != 0)
            {
				//Main.NewText("if projCloneCOunter: " + savedProjs.Count);

				int type = (int)savedProjs[0];
				savedProjs.RemoveAt(0);
				if (projectile.owner == Main.myPlayer)
				{
					int projLoc = Projectile.NewProjectile(projectile.position, projectileDirection * trueVelocity, type, (int)(50f * player.magicDamageMult), 1, Main.myPlayer);
					Main.projectile[projLoc].friendly = true;
					Main.projectile[projLoc].hostile = false;
					projectile.netUpdate = true;
				}
				
				projToShoot--;

            } else // end it
            {
				AI_State = State_Absorbing;
				savedProjs.Clear();
				projectile.Kill();
			}

			/**

			foreach (int i in savedProjClone)
            {
				int projLoc = Projectile.NewProjectile(projectile.position, projectileDirection * trueVelocity, i, 20, 1, Main.myPlayer);
				Main.projectile[projLoc].friendly = true;
				Main.projectile[projLoc].hostile = false;
			}
			savedProjs.Clear();
			**/
        }


		Vector2 projectileDirection;
		float trueVelocity = 8f;
		ArrayList savedProjs;
		object[] savedProjClone;
		int projToShoot;


		private void UpdatePlayer(Player player)
		{
			if (projectile.owner == Main.myPlayer)
			{
				Vector2 diff = Main.MouseWorld - player.Center;
				diff.Normalize();
				projectile.position = (player.Center - new Vector2(16, 18)) + (diff * 70f);
				projectileDirection = diff;
				projectile.direction = Main.MouseWorld.X > player.position.X ? 1 : -1;
				projectile.netUpdate = true;
			}
			int dir = projectile.direction;
			player.ChangeDir(dir); // Set player direction to where we are shooting
			player.heldProj = projectile.whoAmI; // Update player's held projectile
			player.itemTime = 2; // Set item time to 2 frames while we are used
			player.itemAnimation = 2; // Set item animation time to 2 frames while we are used
			player.itemRotation = projectileDirection.ToRotation();
			if (Main.MouseWorld.X < player.position.X)
            {
				player.itemRotation = projectileDirection.ToRotation() + MathHelper.Pi;

			}
			projectile.rotation = projectileDirection.ToRotation();

			Main.NewText(player.itemRotation);
		}


		// DOES AFTERIMAGE
		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			//Redraw the projectile with the color not influenced by light
			Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
			for (int k = 0; k < projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
				Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - k) / ((float)projectile.oldPos.Length * 2f)); // progressively make the afterimages more trans
				spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
			}
			return true;
		}
		private void CastLights()
		{
			// Cast a light along the line of the laser
			//DelegateMethods.v3_1 = new Vector3(0.8f, 0.8f, 1f);
			//Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * (Distance - MOVE_DISTANCE), 26, DelegateMethods.CastLight);
		}

		public override bool ShouldUpdatePosition() => false;

		/*
		 * Update CutTiles so the laser will cut tiles (like grass)
		 */
		public override void CutTiles()
		{
			//DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
			//Vector2 unit = projectile.velocity;
			//Utils.PlotTileLine(projectile.Center, projectile.Center + unit * Distance, (projectile.width + 16) * projectile.scale, DelegateMethods.CutTiles);
		}


		/**
		private class SavedProjectile
		{
			public int projType;

			public NPC curEnemyTarget; // added

			public int cooldown;

			public DrillBeam()
			{
				this.curTileTarget = Point16.NegativeOne;
				this.cooldown = 0;
				this.curEnemyTarget = null;
			}
		}
		**/
	}
}
