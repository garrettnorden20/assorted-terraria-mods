﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace BasicMod.Items.MeleeWeapons.Flails
{
	public class Hookshot : ModItem
	{
		public override string Texture => "Terraria/Item_" + ItemID.BlueMoon;
		public override void SetDefaults()
		{
			item.width = 22;
			item.height = 20;
			item.value = Item.sellPrice(silver: 5);
			item.rare = ItemRarityID.White;
			item.noMelee = true;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.useAnimation = 40;
			item.useTime = 40;
			item.knockBack = 1f;
			item.damage = 5;
			item.noUseGraphic = true;
			item.shoot = mod.ProjectileType("HookshotProjectile");
			item.shootSpeed = 20f;
			item.UseSound = SoundID.Item1;
			item.melee = true;
			item.crit = 9;
			item.channel = true;
		}

		public override void AddRecipes()
		{
			var recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Chain);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
	public class HookshotProjectile : ModProjectile
	{
		// The folder path to the flail chain sprite
		private const string ChainTexturePath = "BasicMod/Items/MeleeWeapons/Flails/ExampleFlailProjectileChain";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Taser Spoke"); // Set the projectile name to Example Flail Ball
		}

		public override void SetDefaults()
		{
			projectile.width = 22;
			projectile.height = 22;
			projectile.friendly = true;
			projectile.penetrate = -1; // Make the flail infinitely penetrate like other flails
			projectile.melee = true;
			//	projectile.aiStyle = 15; // The vanilla flails all use aiStyle 15, but we must not use it since we want to customize the range and behavior.
		}

		int AI_Timer = 0;
		public bool tasing = false;
		public bool alreadyTased = false;

		public int taseTimer = 0;
		public int maxTaseTime = 60;
		public float taseCooldown = 5; // frames between each damage instance

		public float maxLength = 350f;

		NPC targetting = null;

		// This AI code is adapted from the aiStyle 15. We need to re-implement this to customize the behavior of our flail
		public override void AI()
		{
			AI_Timer++;
			if (AI_Timer == 1)
			{
				Main.NewText(projectile.velocity.ToRotation());
			}

			// velocity on below dust could be important
			//var dust = Dust.NewDustDirect(targetting.position, targetting.width, targetting.height, 172, projectile.velocity.X * 0.4f, projectile.velocity.Y * 0.4f, 100, default, 1.5f);


			var player = Main.player[projectile.owner];
			HookPlayer hookPlayer = player.GetModPlayer<HookPlayer>();

			// If owner player dies, remove the flail.
			if (player.dead)
			{
				projectile.Kill();
				return;
			}

			// This prevents the item from being able to be used again prior to this projectile dying
			player.itemAnimation = 10;
			player.itemTime = 10;

			// Here we turn the player and projectile based on the relative positioning of the player and projectile.
			int newDirection = projectile.Center.X > player.Center.X ? 1 : -1;
			player.ChangeDir(newDirection);
			projectile.direction = newDirection;

			var vectorToPlayer = player.MountedCenter - projectile.Center;
			float currentChainLength = vectorToPlayer.Length();

			// Here is what various ai[] values mean in this AI code:
			// ai[0] == 0: Just spawned/being thrown out
			// ai[0] == 1: Flail has hit a tile or has reached maxChainLength, and is now in the swinging mode
			// ai[1] == 1 or !projectile.tileCollide: projectile is being forced to retract

			// ai[0] == 0 means the projectile has neither hit any tiles yet or reached maxChainLength
			if (projectile.ai[0] == 0f)
			{
				// This is how far the chain would go measured in pixels
				float maxChainLength = maxLength;
				projectile.tileCollide = true;

				if (currentChainLength > maxChainLength)
				{
					// If we reach maxChainLength, we change behavior.
					projectile.ai[0] = 1f;
					projectile.ai[1] = 1f;
					projectile.netUpdate = true;
				}
				/**
				else if (!player.channel)
				{
					// Once player lets go of the use button, let gravity take over and let air friction slow down the projectile
					if (projectile.velocity.Y < 0f)
						projectile.velocity.Y *= 0.9f;

					projectile.velocity.Y += 1f;
					projectile.velocity.X *= 0.9f;
				}
				**/
			}
			else if (projectile.ai[0] == 1f)
			{
				//if (tasing)
				if (hookPlayer.hooking)
				{

					// Spawn some dust visuals
					Vector2 randomDust = Main.rand.NextVector2Unit() * 3f;
					var dust = Dust.NewDustDirect(targetting.position, targetting.width, targetting.height, 226, randomDust.X, randomDust.Y, 255, default, 1.5f);
					dust.noGravity = true;
					dust.velocity /= 2f;

					//projectile.friendly = true;
					projectile.velocity = Vector2.Zero;
					projectile.position = targetting.Center;

					

					taseTimer++;
					//if ((!targetting.active || taseTimer > maxTaseTime) && Main.netMode != 1)
					if (!targetting.active && Main.netMode != 1)
					{
						//tasing = false;
						hookPlayer.hooking = false;
					} else
                    {
						player.velocity = Vector2.Normalize(Main.npc[hookPlayer.npcIndex].Center - player.position) * 12f;
					}
					

				}
				else
				{
					// When ai[0] == 1f, the projectile has either hit a tile or has reached maxChainLength, so now we retract the projectile
					float elasticFactorA = 20f / player.meleeSpeed; // on way back
					float elasticFactorB = 8f / player.meleeSpeed;
					//float maxStretchLength = 300f; // This is the furthest the flail can stretch before being forced to retract. Make sure that this is a bit less than maxChainLength so you don't accidentally reach maxStretchLength on the initial throw.

					if (projectile.ai[1] == 1f)
					{

					}

					if (projectile.tileCollide)
						projectile.netUpdate = true;

					projectile.tileCollide = false;
					if (currentChainLength < 20f)
						projectile.Kill();

					if (!projectile.tileCollide)
						elasticFactorB *= 2f;

					int restingChainLength = 60;

					if (currentChainLength > restingChainLength || !projectile.tileCollide)
					{
						var elasticAcceleration = vectorToPlayer * elasticFactorA / currentChainLength - projectile.velocity;
						elasticAcceleration *= elasticFactorB / elasticAcceleration.Length();
						projectile.velocity *= 0.98f;
						projectile.velocity += elasticAcceleration;
					}
					/**
					// If there is tension in the chain, or if the projectile is being forced to retract, give the projectile some velocity towards the player
					if (currentChainLength > restingChainLength || !projectile.tileCollide)
					{
						var elasticAcceleration = vectorToPlayer * elasticFactorA / currentChainLength - projectile.velocity;
						elasticAcceleration *= elasticFactorB / elasticAcceleration.Length();
						projectile.velocity *= 0.98f;
						projectile.velocity += elasticAcceleration;
					}
					else
					{
						// Otherwise, friction and gravity allow the projectile to rest.
						if (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y) < 6f)
						{
							projectile.velocity.X *= 0.96f;
							projectile.velocity.Y += 0.2f;
						}
						if (player.velocity.X == 0f)
							projectile.velocity.X *= 0.96f;
					}
					**/
					//var elasticAcceleration = vectorToPlayer * elasticFactorA / currentChainLength - projectile.velocity;
					//elasticAcceleration *= elasticFactorB / elasticAcceleration.Length();
					//projectile.velocity *= 0.98f;
					//projectile.velocity += elasticAcceleration;
				}
			}

			// Here we set the rotation based off of the direction to the player tweaked by the velocity, giving it a little spin as the flail turns around each swing 
			projectile.rotation = vectorToPlayer.ToRotation(); //projectile.velocity.X * 0.1f;

			// Here is where a flail like Flower Pow could spawn additional projectiles or other custom behaviors
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.immune[projectile.owner] = 5; // set invincibility frames

			if (!alreadyTased)
			{
				HookPlayer hookPlayer = Main.player[projectile.owner].GetModPlayer<HookPlayer>();
				hookPlayer.hooking = true;
				hookPlayer.npcIndex = target.whoAmI;
				//tasing = true;
				projectile.ai[0] = 1f;
				projectile.ai[1] = 1f;
				targetting = target;
				alreadyTased = true;
			}

		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			// This custom OnTileCollide code makes the projectile bounce off tiles at 1/5th the original speed, and plays sound and spawns dust if the projectile was going fast enough.
			bool shouldMakeSound = false;

			if (oldVelocity.X != projectile.velocity.X)
			{
				if (Math.Abs(oldVelocity.X) > 4f)
				{
					shouldMakeSound = true;
				}

				projectile.position.X += projectile.velocity.X;
				projectile.velocity.X = -oldVelocity.X * 0.2f;
			}

			if (oldVelocity.Y != projectile.velocity.Y)
			{
				if (Math.Abs(oldVelocity.Y) > 4f)
				{
					shouldMakeSound = true;
				}

				projectile.position.Y += projectile.velocity.Y;
				projectile.velocity.Y = -oldVelocity.Y * 0.2f;
			}

			// ai[0] == 1 is used in AI to represent that the projectile has hit a tile since spawning
			projectile.ai[0] = 1f;

			if (shouldMakeSound)
			{
				// if we should play the sound..
				projectile.netUpdate = true;
				Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
				// Play the sound
				Main.PlaySound(SoundID.Dig, (int)projectile.position.X, (int)projectile.position.Y);
			}

			return false;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			var player = Main.player[projectile.owner];

			Vector2 mountedCenter = player.MountedCenter;
			Texture2D chainTexture = ModContent.GetTexture(ChainTexturePath);

			var drawPosition = projectile.Center;
			var remainingVectorToPlayer = mountedCenter - drawPosition;

			float rotation = remainingVectorToPlayer.ToRotation() - MathHelper.PiOver2;

			if (projectile.alpha == 0)
			{
				int direction = -1;

				if (projectile.Center.X < mountedCenter.X)
					direction = 1;

				player.itemRotation = (float)Math.Atan2(remainingVectorToPlayer.Y * direction, remainingVectorToPlayer.X * direction);
			}

			// This while loop draws the chain texture from the projectile to the player, looping to draw the chain texture along the path
			while (true)
			{
				float length = remainingVectorToPlayer.Length();

				// Once the remaining length is small enough, we terminate the loop
				if (length < 25f || float.IsNaN(length))
					break;

				// drawPosition is advanced along the vector back to the player by 12 pixels
				// 12 comes from the height of ExampleFlailProjectileChain.png and the spacing that we desired between links
				drawPosition += remainingVectorToPlayer * 12 / length;
				remainingVectorToPlayer = mountedCenter - drawPosition;

				// Finally, we draw the texture at the coordinates using the lighting information of the tile coordinates of the chain section
				Color color = Lighting.GetColor((int)drawPosition.X / 16, (int)(drawPosition.Y / 16f));
				spriteBatch.Draw(chainTexture, drawPosition - Main.screenPosition, null, color, rotation, chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
			}

			return true;
		}
	}

	public class HookPlayer : ModPlayer
	{
		public int npcIndex;
		public bool hooking;

		public override void ResetEffects()
		{
			npcIndex = 0;
			//hooking = false;
		}
		public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
		{
			if (hooking && npc.whoAmI == npcIndex)
			{
				damage = 0;
				player.ApplyDamageToNPC(npc, (int)(player.meleeDamageMult * 50), 15f, player.direction, false);
				player.velocity = Vector2.Zero;
				hooking = false;
			}
		}
	}
}
