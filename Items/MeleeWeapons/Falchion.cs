using BasicMod.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BasicMod.Items.MeleeWeapons
{
	public class Falchion : ModItem
	{
		public override string Texture => "Terraria/Item_" + ItemID.TrueExcalibur;
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Launches stars with every thrust.");

		}

		public override void SetDefaults()
		{
			item.damage = 30;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.useAnimation = 34;
			item.useTime = 34;
			item.shootSpeed = 3.7f;
			item.knockBack = 8f;
			item.width = 32;
			item.height = 32;
			item.scale = 1f;
			item.rare = ItemRarityID.Pink;
			item.value = Item.sellPrice(silver: 10);
			item.channel = true;

			item.melee = true;
			item.noMelee = true; // Important because the spear is actually a projectile instead of an item. This prevents the melee hitbox of this item.
			item.noUseGraphic = true; // Important, it's kind of wired if people see two spears at one time. This prevents the melee animation of this item.
			item.autoReuse = false; // Most spears don't autoReuse, but it's possible when used in conjunction with CanUseItem()

			item.UseSound = SoundID.Item1;
			item.shoot = mod.ProjectileType("FalchionProjectile");
		}

		public override bool CanUseItem(Player player)
		{
			// Ensures no more than one spear can be thrown out, use this when using autoReuse
			return player.ownedProjectileCounts[item.shoot] < 1;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			// TODO: add ingredient droppable from new enemy?
			recipe.AddIngredient(ModContent.ItemType<Ores.SeleniumBar>(), 12);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}

	public class FalchionProjectile : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Falchion");
			
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



		Vector2 mousePosition;

		public float currentRotation = 0;
		public float currentRotationTarget = 0;

		bool swingDownwards = false; // initial swing is upwards

		public float AI_Timer;
		public float swingDelay = 25;

		public float swingRange = MathHelper.PiOver2;

		// bottom 2 must add to 1
		public float rotateNum1 = 0.92f;
		public float rotateNum2 = 0.08f;


		// somehow, projectile.ai[0] controls movement, or being attached. dont touch it.
		public override void AI()
		{

			// update player
			Player projOwner = Main.player[projectile.owner];
			projOwner.heldProj = projectile.whoAmI;
			
			projOwner.itemTime = projOwner.itemAnimation;

			AI_Timer++;
			

			if (AI_Timer >= swingDelay) // at the beginning of every new swing
			{
				AI_Timer = 0; // reset the timer

				if (projOwner.channel) // if channeling
				{
					
					projOwner.itemTime = (int)(swingDelay + 1); // set these two to survive until at least the next swing
					projOwner.itemAnimation = (int)(swingDelay + 1);

					projectile.ai[1] = 0; // will run initial swing code

					swingDownwards ^= true; // swaps upward to downward, and vice versa

				}
				else // if not channeling, kill the proj. note that it only checks to destroy proj at beginning of each swing
                {
					
					projectile.Kill();
				}

			}

			if (projectile.ai[1] == 0 && projectile.owner == Main.myPlayer) // swing initializer. runs at start of every swing, also checks for multiplayer support
            {

				mousePosition = Main.MouseWorld - projOwner.Center;

				// change player's direction
				int newDirection = Main.MouseWorld.X > projOwner.Center.X ? 1 : -1;
				projOwner.ChangeDir(newDirection);
				projectile.direction = newDirection;


				// adjustment that helps center the sweetspot on the mouse
				if (projOwner.direction < 0)
				{
					mousePosition = mousePosition.RotatedBy(MathHelper.PiOver4 * -1);
				} else
                {
					mousePosition = mousePosition.RotatedBy(MathHelper.PiOver4);
				}

				float mouseRotation = (mousePosition.ToRotation());

				if (swingDownwards)
				{

					currentRotation = mouseRotation - swingRange; // start swing behind the mouse

					currentRotationTarget = mouseRotation + swingRange; // end swing ahead of the mouse

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
				} else // swing upwards
                {

					currentRotation = mouseRotation + swingRange;

					currentRotationTarget = mouseRotation - swingRange;

					if (projOwner.direction < 0) // if facing left, swap the rotation and the target(this makes sure it swings upward)
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
				}
			}
			projectile.ai[1] = 1; // ensure initializer wont run again until next swing


			// the actual turning function: the two numbers MUST add to 1.
			// making the second number larger makes the swing much faster.

			currentRotation = currentRotation * rotateNum1 + rotateNum2 * currentRotationTarget;
			projectile.rotation = currentRotation;

			updatePlayerItemRotation(projOwner, currentRotation);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.immune[projectile.owner] = (int)((int)swingDelay - AI_Timer);
		}

		private void updatePlayerItemRotation(Player projOwner, float rotation)
        {
			if (projOwner.direction < 0)
			{
				projOwner.itemRotation = currentRotation - swingRange;
			} else
            {
				projOwner.itemRotation = currentRotation;
			}
		}


		// these dictate how far away the hitbox should be
		private float width = 40;
		private float height = 40;

		private int offsetX = -35; // how much to offset the X of the hitbox
		private int offsetY = -40; // how much to offset the Y of the hitbox

		private int hitboxWidth = 75;
		private int hitboxHeight = 75;

		public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
			Player projOwner = Main.player[projectile.owner];
			//Main.NewText(hitbox.X + " " + hitbox.Y);

			if (swingDownwards)
            {
				if (projOwner.direction < 0) // if facing left
				{
					hitbox.X = (int)(hitbox.X + (Math.Sqrt((double)height * height + width * width)) * Math.Cos(currentRotation + swingRange)) + offsetX;
					hitbox.Y = (int)(hitbox.Y + (Math.Sqrt((double)height * height + width * width)) * Math.Sin(currentRotation + swingRange)) + offsetY;

				}
				else
				{
					hitbox.X = (int)(hitbox.X + (Math.Sqrt((double)height * height + width * width)) * Math.Cos(currentRotation)) + offsetX;
					hitbox.Y = (int)(hitbox.Y + (Math.Sqrt((double)height * height + width * width)) * Math.Sin(currentRotation)) + offsetY;

				}
			} else // if swinging upwards
            {
				if (projOwner.direction < 0) // if facing left
				{
					hitbox.X = (int)(hitbox.X + (Math.Sqrt((double)height * height + width * width)) * Math.Cos(currentRotation)) + offsetX;
					hitbox.Y = (int)(hitbox.Y + (Math.Sqrt((double)height * height + width * width)) * Math.Sin(currentRotation)) + offsetY;

				}
				else
				{
					hitbox.X = (int)(hitbox.X + (Math.Sqrt((double)height * height + width * width)) * Math.Cos(currentRotation + swingRange)) + offsetX;
					hitbox.Y = (int)(hitbox.Y + (Math.Sqrt((double)height * height + width * width)) * Math.Sin(currentRotation + swingRange)) + offsetY;

				}
			}
			
			// changes the hitbox size. don't confuse with above fields
			hitbox.Width = hitboxWidth;
			hitbox.Height = hitboxHeight;
			//Main.NewText("now" + hitbox.X + " " + hitbox.Y);
		}

	}
}
