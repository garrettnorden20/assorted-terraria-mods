using BasicMod.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BasicMod.Items.MeleeWeapons.Slashers
{
	public class KatanaSlasher : SlasherItem
	{
		public override string Texture => "Terraria/Item_" + ItemID.Katana;
		public override void SetStaticDefaults()
		{

		}

		public override void SetDefaults()
		{
			item.CloneDefaults(ItemID.TerraBlade);
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
			item.damage = 25;
			item.knockBack = 2.5f;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.useTime = 17; // this doesnt determine cooldown between swings while autoreuse. must be greater than swing delay
			item.useAnimation = 17;
			item.channel = false; // turn this off to make it not automatically swing next
			item.melee = true;
			item.noMelee = true; // Important because the spear is actually a projectile instead of an item. This prevents the melee hitbox of this item.
			item.noUseGraphic = true; // Important, it's kind of wired if people see two spears at one time. This prevents the melee animation of this item.
			
			item.autoReuse = true; // autouse overrides swingDelay, in a sense, and makes it use useTime.

			item.UseSound = SoundID.Item1;
			item.shoot = mod.ProjectileType("KatanaSlasherProjectile");
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Wood, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}

	}

	public class KatanaSlasherProjectile : Slasher
	{
		public KatanaSlasherProjectile()
		{
			// set values

			swingRange = MathHelper.PiOver2;

			swingDelay = 16;

			rotateNum1 = 0.70f;
			rotateNum2 = 0.30f;

			dist = 60; // how far away the hitbox is from the player.

			
			hitboxWidth = 100;
			hitboxHeight = 100;

			offsetX = -1 * (hitboxWidth / 2); // how much to offset the X of the hitbox, this should be half of hitboxwidth
			offsetY = -1 * (hitboxHeight / 2); // how much to offset the Y of the hitbox, this should be half of hitbox height

		}
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Katana Slasher");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 3;    //The length of old position to be recorded
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;        //The recording mode
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


		float curRot = 0;
		/**
		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			/**
			//Redraw the projectile with the color not influenced by light
			Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
			for (int k = 0; k < projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, projectile.gfxOffY);
				Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - k) / ((float)projectile.oldPos.Length * 2f)); // progressively make the afterimages more trans
				spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, projectile.rotation + Main.rand.NextFloat(-MathHelper.PiOver4 / 3, MathHelper.PiOver4 / 3), Vector2.Zero, projectile.scale, SpriteEffects.None, 0f);
				// rotation +Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2) is a cool effect, but doesn't line up well with swings.
			}
			return true;
			**/
			//Redraw the projectile with the color not influenced by light


			// rotating afterimages. could be cool, just not for this

			/**
			float dist = 100;
			Player projOwner = Main.player[projectile.owner];
			curRot++;
			for (int k = 0; k <= 5; k++)
			{
				float rot = MathHelper.ToRadians(curRot + k*(360f/6f));

				float x = ((float)(projOwner.Center.X + dist * Math.Cos(rot)));
				float y = ((float)(projOwner.Center.Y + dist * Math.Sin(rot)));

				Vector2 drawPos = new Vector2(x, y) - Main.screenPosition + new Vector2(0f, projectile.gfxOffY);

				Color color = projectile.GetAlpha(lightColor) * (0.5f); // progressively make the afterimages more trans
				spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, projectile.rotation, Vector2.Zero, projectile.scale, SpriteEffects.None, 0f);
				// rotation +Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2) is a cool effect, but doesn't line up well with swings.
			}
			return true;
			return true;
		}
		**/



	}


}