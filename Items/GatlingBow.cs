using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace BasicMod.Items
{
	public class GatlingBow : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("TutorialSword"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			Tooltip.SetDefault("An arrow hose.");
		}

		public override void SetDefaults()
		{
			/**
			
			item.melee = true;
			item.width = 40;
			item.height = 40;
			item.useTime = 25;
			item.useAnimation = 20;
			item.useStyle = 1;
			item.knockBack = 6;
			item.value = 10000;
			item.rare = 2;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			
			**/
			item.CloneDefaults(ItemID.WoodenBow);
			item.shoot = mod.ProjectileType("HoseLaser");
			item.damage = 8;
			item.shootSpeed = 8;
			item.useAmmo = AmmoID.None;
			item.autoReuse = true; // means autoswing/autofire
            item.useTime = 1;
			item.UseSound = SoundID.Item12;
			item.useAnimation = 3;

		}

		
		public override bool AltFunctionUse(Player player)
		{
			return true;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack) //This lets you modify the firing of the item
		{
			if (player.altFunctionUse == 2)
			{
				Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, Main.myPlayer);
			}
			else
			{
				Vector2 muzzleOffset = Vector2.Normalize(new Vector2(speedX, speedY)) * 25f;
				if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
				{
					position += muzzleOffset;
				}

				int spread = 20; //The angle of random spread.
								 //float spreadMult = 0.1f; //Multiplier for bullet spread, set it higher and it will make for some outrageous spread.
				float spreadMult = (float)Main.rand.Next(1, 5) / 10.0f;
				for (int i = 0; i < 5; i++)
				{

					float vX = speedX + (float)Main.rand.Next(-spread, spread + 1) * spreadMult;
					float vY = speedY + (float)Main.rand.Next(-spread, spread + 1) * spreadMult;
					Projectile.NewProjectile(position.X, position.Y, vX, vY, type, damage, knockBack, Main.myPlayer);
					ConsumeAmmo(player);
				}
			}
			return false;

		}


        public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.DirtBlock, 1);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}