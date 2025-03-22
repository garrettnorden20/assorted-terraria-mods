using BasicMod.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BasicMod.Items.MeleeWeapons.Slashers
{
	public class LevinSword : SlasherItem
	{
		public override void SetStaticDefaults()
		{

		}

		public override void SetDefaults()
		{
			item.CloneDefaults(ItemID.InfluxWaver);
			/**
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
			
			**/
			//item.knockBack = 8f;
			item.damage = 100;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.useTime = 35; // this doesnt determine cooldown between swings while autoreuse. must be greater than swing delay (or not, appears to be nonissue
			item.useAnimation = 35;
			item.channel = false; // turn this off to make it not automatically swing next
			item.melee = true;
			item.noMelee = true; // Important because the spear is actually a projectile instead of an item. This prevents the melee hitbox of this item.
			item.noUseGraphic = true; // Important, it's kind of wired if people see two spears at one time. This prevents the melee animation of this item.

			item.autoReuse = true; // autouse overrides swingDelay, in a sense, and makes it use useTime.

			item.UseSound = SoundID.Item1;
			item.shoot = mod.ProjectileType("LevinSwordProjectile");
		}

		public override void AddRecipes()
		{
			// drop from Tesla Coil
		}

	}

	public class LevinSwordProjectile : Slasher
	{
		public LevinSwordProjectile()
		{
			// set values
			swingRange = MathHelper.PiOver2;

			swingDelay = 34;

			rotateNum1 = 0.85f;
			rotateNum2 = 0.15f;

			dist = 50; // how far away the hitbox is from the player.


			hitboxWidth = 80;
			hitboxHeight = 80;

			offsetX = -1 * (hitboxWidth / 2); // how much to offset the X of the hitbox, this should be half and -5 of hitboxwidth
			offsetY = -1 * (hitboxHeight / 2); // how much to offset the Y of the hitbox, this should be half of hitbox height

			numProjs = 4;

			projVelocity = 8f;

		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("LevinSword");
			//ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;    //The length of old position to be recorded
			//ProjectileID.Sets.TrailingMode[projectile.type] = 0;        //The recording mode
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

		public override void CustomBehavior(bool swingDownwards, Player projOwner)
		{
			if ((!swingDownwards && projOwner.direction < 0) || (swingDownwards && projOwner.direction > 0))
			{
				if (projectile.rotation > initialRotation + totalProjRange)
				{
					//Main.NewText("in proj fir");
					Projectile.NewProjectile(projOwner.Center, projVelocity * (projectile.rotation + MathHelper.PiOver4).ToRotationVector2(), mod.ProjectileType("LevinSwordBeam"), (int)(projectile.damage * 0.75f), 2, projOwner.whoAmI, 0f);
					totalProjRange += eachProjRange;
				}
			}
			else
			{
				if (projectile.rotation < initialRotation + totalProjRange)
				{
					//Main.NewText("in proj fir");
					Projectile.NewProjectile(projOwner.Center, projVelocity * (projectile.rotation + MathHelper.PiOver4).ToRotationVector2(), mod.ProjectileType("LevinSwordBeam"), (int)(projectile.damage * 0.75f), 2, projOwner.whoAmI, 0f);
					totalProjRange += eachProjRange;
				}
			}

		}

        public override void createDusts(Rectangle box)
        {
			if (Main.rand.Next(2) == 0)
			{
				Vector2 randomDust = Main.rand.NextVector2Unit() * 3f;
				//var dust = Dust.NewDustDirect(box.TopLeft(), box.Width, box.Height, 226, randomDust.X, randomDust.Y, 255, default, 1.5f);
				Dust.NewDust(box.TopLeft(), box.Width, box.Height, 133, randomDust.X * 0.25f, randomDust.Y * 0.25f, 0, Color.Yellow, 1f);
			}
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			/**
			//Redraw the projectile with the color not influenced by light
			Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
			for (int k = 0; k < projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, projectile.gfxOffY) + new Vector2(Main.rand.Next(-16, 16), Main.rand.Next(-16, 16)); // makes this awesome vibrato

				Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - k) / ((float)projectile.oldPos.Length * 2f)); // progressively make the afterimages more trans
				spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, projectile.rotation, Vector2.Zero, projectile.scale, SpriteEffects.None, 0f);
				// + Main.rand.NextFloat(-MathHelper.PiOver4 / 3, MathHelper.PiOver4 / 3)
				// rotation +Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2) is a cool effect, but doesn't line up well with swings.
			}
			**/
			return true;
		}

	}

	public class LevinSwordBeam : ModProjectile
    {
		public override void SetStaticDefaults()
		{
			Main.projFrames[projectile.type] = 4;
		}
		public override void SetDefaults()
		{
			projectile.CloneDefaults(ProjectileID.WoodenArrowFriendly);

			projectile.alpha = 100;
			projectile.friendly = true;
			projectile.melee = true;
		
			projectile.light = 1;

			projectile.timeLeft = 300;
			projectile.aiStyle = -1;

		}

		public override void AI()
		{
			Player owner = Main.player[projectile.owner];
			projectile.rotation = projectile.velocity.ToRotation();// + MathHelper.PiOver2;

			projectile.ai[0] += 1f; // Use a timer 

			if (projectile.alpha > 1)
			{
				projectile.alpha = projectile.alpha - 17; // make it less transparent
			}

			if (projectile.ai[0] % 20 == 0)
			{
				Dust.NewDust(projectile.position, projectile.width, projectile.height, 202);
			}


			// cycling through frame anims
			if (++projectile.frameCounter >= 4)
			{
				projectile.frameCounter = 0;
				if (++projectile.frame >= 4)
				{
					projectile.frame = 0;
				}
			}


			// get current speed (length of velocity)
			float speed = projectile.velocity.Length();

			projectile.velocity = 1.005f * speed * Vector2.Normalize(projectile.velocity);



		}

		public override void Kill(int timeLeft)
		{
			// This code and the similar code above in OnTileCollide spawn dust from the tiles collided with. SoundID.Item10 is the bounce sound you hear.
			Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
			Main.PlaySound(SoundID.Item10, projectile.position);
		}
	}
}