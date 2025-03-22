using BasicMod.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


// To be inherited

namespace BasicMod.Items.MeleeWeapons.Slashers
{
	public class Slasher : ModProjectile
	{
		/**
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Slasher");
			drawOffsetX = -45;
			drawOriginOffsetY = -45;
			drawOriginOffsetX = 16;
		}

		public override void SetDefaults()
		{
			projectile.width = 1;
			projectile.height = 1;
			projectile.aiStyle = 19;
			projectile.penetrate = -1;
			projectile.scale = 1.3f;
			projectile.alpha = 0;

			projectile.hide = true;
			projectile.ownerHitCheck = true;
			projectile.melee = true;
			projectile.tileCollide = false;
			projectile.friendly = true;
		}
		**/
		Rectangle newHitbox;

		Vector2 mousePosition;
		
		bool swingDownwards = false; // initial swing is upwards
		bool initializer = true; // whether to run the intiializer on this iteration
		
		public float AI_Timer;

		private float currentRotation = 0;

		public float initialRotation = 0;
		private float currentRotationTarget = 0;
		
		public float swingDelay = 25;

		public float swingRange = MathHelper.PiOver2;

		// bottom 2 must add to 1
		public float rotateNum1 = 0.92f;
		public float rotateNum2 = 0.08f;

		// TEMP FIELDS
		public int numProjs = 5;
		public float projVelocity = 1f;
		public float eachProjRange;
		public float totalProjRange;

		// AI if each projectile is a new instance seamlessly put together.
		public override void AI()
		{


			if (projectile.ai[1] == 1) // makes it so that swinging downwards is true
			{
				swingDownwards = true;
				projectile.ai[1] = 0; // dont run again
			}

			// update player
			Player projOwner = Main.player[projectile.owner];
			projOwner.heldProj = projectile.whoAmI;

			/** this was supposed to fix if useTime is less than swingDelay, but now appears to be a nonissue.
			if (projOwner.itemAnimationMax < swingDelay)
            {
				projOwner.itemAnimationMax = (int)(swingDelay + 1);
				projOwner.itemAnimation = projOwner.itemAnimationMax;

			}
			projOwner.itemTime = projOwner.itemAnimation;
			**/

			//projOwner.itemTime = 2;
			//projOwner.itemAnimation = 2;

			if (AI_Timer >= swingDelay) // at the end of a swing
			{
				projectile.Kill();
			}


			if (AI_Timer == 0 & projectile.owner == Main.myPlayer)
			{
				swingDelay = swingDelay - (int)((1 - projOwner.meleeSpeed) * swingDelay); // melee speed defaults to 1.0, faster is things llike 0.8
				projVelocity = projVelocity + ((1 - projOwner.meleeSpeed) * projVelocity);

				mousePosition = Main.MouseWorld - projOwner.Center;

				// change player's direction
				int newDirection = Main.MouseWorld.X > projOwner.Center.X ? 1 : -1;
				projOwner.ChangeDir(newDirection);
				projectile.direction = newDirection;


				// adjustment that helps center the sweetspot on the mouse
				if (projOwner.direction < 0)
				{
					mousePosition = mousePosition.RotatedBy(MathHelper.PiOver4 * -1);
				}
				else
				{
					mousePosition = mousePosition.RotatedBy(MathHelper.PiOver4);
				}

				float mouseRotation = (mousePosition.ToRotation());

				if (swingDownwards)
				{
					currentRotation = mouseRotation - swingRange; // start swing behind the mouse
					currentRotationTarget = mouseRotation + swingRange; // end swing ahead of the mouse
				}
				else // swing upwards
				{
					currentRotation = mouseRotation + swingRange;
					currentRotationTarget = mouseRotation - swingRange;
				}

				if (projOwner.direction < 0) // if facing left, swap the rotation and the target (this makes sure it swings upward)
				{
					float temp = currentRotation;
					currentRotation = currentRotationTarget;
					currentRotationTarget = temp;
				}
				else // if facing right, do this. i dont really know why.
				{
					currentRotation -= swingRange;
					currentRotationTarget -= swingRange;
				}

				initialRotation = currentRotation;


				eachProjRange = (currentRotationTarget - initialRotation) / numProjs;
				totalProjRange = eachProjRange;
				
				projectile.netUpdate = true;
			}

			// the actual turning function: the two numbers MUST add to 1.
			// making the second number larger makes the swing much faster.

			currentRotation = currentRotation * rotateNum1 + rotateNum2 * currentRotationTarget;
			projectile.rotation = currentRotation;


			CustomBehavior(swingDownwards, projOwner);

			createDusts(newHitbox);

			updatePlayerItemRotation(projOwner, currentRotation);

			AI_Timer++;

		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(AI_Timer);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			AI_Timer = reader.ReadSingle();
		}
		public virtual void CustomBehavior(bool swingDownwards, Player projOwner)
		{
		}

		public virtual void createDusts(Rectangle box)
        {

        }

		public virtual Terraria.Audio.LegacySoundStyle getSound()
        {
			return SoundID.Item1;
		}


        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
			// fixes wrong hit direction when facing left
			Player projOwner = Main.player[projectile.owner];
			hitDirection = projOwner.direction;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			Player projOwner = Main.player[projectile.owner];
			target.immune[projectile.owner] = (int)((int)swingDelay + 1 - AI_Timer);
		}

		private void updatePlayerItemRotation(Player projOwner, float rotation)
		{
			if (projOwner.direction < 0)
			{
				projOwner.itemRotation = currentRotation - swingRange;
			}
			else
			{
				projOwner.itemRotation = currentRotation;
			}
		}


		// these dictate how far away the hitbox should be
		public float dist = 30;

		public int offsetX = -35; // how much to offset the X of the hitbox
		public int offsetY = -40; // how much to offset the Y of the hitbox

		public int hitboxWidth = 75;
		public int hitboxHeight = 75;

		public override void ModifyDamageHitbox(ref Rectangle hitbox)
		{
			Player projOwner = Main.player[projectile.owner];
			//Main.NewText(hitbox.X + " " + hitbox.Y);

			if (swingDownwards)
			{
				if (projOwner.direction < 0) // if facing left
				{
					hitbox.X = (int)(hitbox.X + dist * Math.Cos(currentRotation + swingRange)) + offsetX;
					hitbox.Y = (int)(hitbox.Y + dist * Math.Sin(currentRotation + swingRange)) + offsetY;

				}
				else
				{
					hitbox.X = (int)(hitbox.X + dist * Math.Cos(currentRotation)) + offsetX;
					hitbox.Y = (int)(hitbox.Y + dist * Math.Sin(currentRotation)) + offsetY;

				}
			}
			else // if swinging upwards
			{
				if (projOwner.direction < 0) // if facing left
				{
					hitbox.X = (int)(hitbox.X + dist * Math.Cos(currentRotation)) + offsetX;
					hitbox.Y = (int)(hitbox.Y + dist * Math.Sin(currentRotation)) + offsetY;

				}
				else
				{
					hitbox.X = (int)(hitbox.X + dist * Math.Cos(currentRotation + swingRange)) + offsetX;
					hitbox.Y = (int)(hitbox.Y + dist * Math.Sin(currentRotation + swingRange)) + offsetY;

				}
			}

			// changes the hitbox size. don't confuse with above fields
			hitbox.Width = hitboxWidth;
			hitbox.Height = hitboxHeight;
			//Main.NewText("now" + hitbox.X + " " + hitbox.Y);
			newHitbox = hitbox;
		}

	}

	public class SlasherItem : ModItem
    {
		bool swingDownwards = false;
		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			if (swingDownwards)
			{
				Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, 0, 1); // swingDownwards
			}
			else
			{
				Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, 0, 0); // swingUpwards
			}
			swingDownwards ^= true; // flip
			return false;
		}

		public override bool CanUseItem(Player player)
		{
			if (player.ownedProjectileCounts[item.shoot] >= 1)
            {
				return false;
            }
			return base.CanUseItem(player);
		}
	}
}
