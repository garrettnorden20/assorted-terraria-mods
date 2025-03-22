using BasicMod.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BasicMod.Items.MeleeWeapons.Slashers
{
	public class HellishGreatslasher : SlasherItem
	{
		public override string Texture => "Terraria/Item_" + ItemID.FieryGreatsword;
		public override void SetStaticDefaults()
		{

		}

		public override void SetDefaults()
		{
			item.CloneDefaults(ItemID.FieryGreatsword);
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
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.useTime = 35; // must be greater than swing delay (or not, appears to be nonissue)
			item.useAnimation = 35;
			item.channel = false; // turn this off to make it not automatically swing next
			item.melee = true;
			item.noMelee = true; // Important because the spear is actually a projectile instead of an item. This prevents the melee hitbox of this item.
			item.noUseGraphic = true; // Important, it's kind of wired if people see two spears at one time. This prevents the melee animation of this item.
			item.autoReuse = false; // keep false, use channel instead. // autouse overrides swingDelay, in a sense, and makes it use useTime.


			// channel true, autoreuse false - swings automatically in continous motion, but if swingDelay is greater, it cuts it off
			// channel false, autoreuse false - works fine, one swing at a time

			// both these IGNORE swingdelay. I believe they create a new projectile most of the time, or in certain conditions, so dangerous to use them.
			// channel true, autoreuse true - works fine, swings automatically in continous motion SHORT
			// channel false, autoreuse true - technically continous motion, but i believe it ignores swingTime and uses useTIme instead for duration between swings. perhaps this is better? ignore for now

			item.UseSound = SoundID.Item1;
			item.shoot = mod.ProjectileType("HellishGreatslasherProjectile");
		}


		// alternate between swinging upwards and downwards (specifically, this is useful if autoreuse is not being used)
		bool swingDownwards = false;

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.HellstoneBar, 12);
			recipe.AddTile(TileID.Hellforge);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}

	}

	public class HellishGreatslasherProjectile : Slasher
	{
		public HellishGreatslasherProjectile()
		{
			// set values
			swingRange = MathHelper.PiOver2;

			swingDelay = 34;

			rotateNum1 = 0.90f;
			rotateNum2 = 0.10f;

			dist = 60; // how far away the hitbox is from the player.

			hitboxWidth = 114;
			hitboxHeight = 114;

			offsetX = -1 * (hitboxWidth / 2); // how much to offset the X of the hitbox, this should be half and -5 of hitboxwidth
			offsetY = -1 * (hitboxHeight / 2); // how much to offset the Y of the hitbox, this should be half of hitbox height
		}
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Hellish Greatslasher");
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

        public override void createDusts(Rectangle box)
        {
			if (Main.rand.Next(2) == 0)
			{
				Dust.NewDust(box.TopLeft(), box.Width, box.Height, 6, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 0, default(Color), 2.4f);
			}
		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            base.OnHitNPC(target, damage, knockback, crit);
			target.AddBuff(BuffID.OnFire, 180);
			Main.NewText("working");
		}


    }


}