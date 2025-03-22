using BasicMod.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BasicMod.Items.MeleeWeapons.Slashers
{
	public class EnchantedSlasher : SlasherItem
	{
		public override string Texture => "Terraria/Item_" + ItemID.EnchantedSword;
		public override void SetStaticDefaults()
		{

		}

		public override void SetDefaults()
		{
			item.CloneDefaults(ItemID.EnchantedSword);
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
			item.useTime = 26; // must be greater than swing delay (or not, appears to be nonissue)
			item.useAnimation = 26;
			item.channel = false; // turn this off to make it not automatically swing next
			item.melee = true;
			item.noMelee = true; // Important because the spear is actually a projectile instead of an item. This prevents the melee hitbox of this item.
			item.noUseGraphic = true; // Important, it's kind of wired if people see two spears at one time. This prevents the melee animation of this item.
			item.autoReuse = true; // keep false, use channel instead. // autouse overrides swingDelay, in a sense, and makes it use useTime.

			item.UseSound = SoundID.Item1;
			item.shoot = mod.ProjectileType("EnchantedSlasherProjectile");
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.FallenStar, 10);
			recipe.AddIngredient(ItemID.EnchantedBoomerang, 1);
			recipe.AddIngredient(mod.ItemType("WoodenSlasher"), 1);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}

	}

	public class EnchantedSlasherProjectile : Slasher
	{
		public EnchantedSlasherProjectile()
		{
			// set values
			swingRange = MathHelper.PiOver2;

			swingDelay = 25;

			rotateNum1 = 0.85f;
			rotateNum2 = 0.15f;

			dist = 35; // how far away the hitbox is from the player.

			hitboxWidth = 50;
			hitboxHeight = 50;

			offsetX = -1 * (hitboxWidth / 2); // how much to offset the X of the hitbox, this should be half and -5 of hitboxwidth
			offsetY = -1 * (hitboxHeight / 2); // how much to offset the Y of the hitbox, this should be half of hitbox height

			numProjs = 2;
		}


		float projVel = 9.5f;


		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Enchanted Slasher");
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
					Projectile.NewProjectile(projOwner.Center, projVel * (projectile.rotation + MathHelper.PiOver4).ToRotationVector2(), ProjectileID.EnchantedBeam, (int)(projectile.damage * 0.75f), 2, projOwner.whoAmI, 0f);
					totalProjRange += eachProjRange;
				}
			}
			else
			{
				if (projectile.rotation < initialRotation + totalProjRange)
				{
					//Main.NewText("in proj fir");
					Projectile.NewProjectile(projOwner.Center, projVel * (projectile.rotation + MathHelper.PiOver4).ToRotationVector2(), ProjectileID.EnchantedBeam, (int)(projectile.damage * 0.75f), 2, projOwner.whoAmI, 0f);
					totalProjRange += eachProjRange;
				}
			}

		}




	}


}