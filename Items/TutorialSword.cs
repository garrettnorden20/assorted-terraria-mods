using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace BasicMod.Items
{
	public class TutorialSword : ModItem
	{
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("TutorialSword"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			Tooltip.SetDefault("Hello World!");
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
			item.shoot = mod.ProjectileType("BinaryArrow");
			**/
			item.CloneDefaults(ItemID.WoodenBow);
			item.damage = 200;
			item.shootSpeed = 6;
			item.autoReuse = false; 
			item.useTime = 20;
			item.useAnimation = 20;

			//item.useAmmo = AmmoID.Arrow;
			item.useAmmo = AmmoID.None;

			//item.shoot = mod.ProjectileType("ExplodeArrow");
			item.shoot = mod.ProjectileType("TeslaLine");


		}
		public override bool AltFunctionUse(Player player)
		{
			return true;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack) //This lets you modify the firing of the item
		{
			ChargePlayer mp = player.GetModPlayer<ChargePlayer>();

			/**
			if (mp.barPresent) // when the bar is up, and you press it
            {
				int timeDiff = Math.Abs((int)Main.GameUpdateCount - mp.oldTime);
				mp.oldTime = (int)Main.GameUpdateCount; // DO NOT use this, b/c it's updated for the next cycle. use diff instead
				mp.barPresent = false;
				
				if (timeDiff > -1 && timeDiff < mp.TOTAL_TIME) // if between the two times
                {
					Main.NewText("Between Times");
					// insert any "special conditions" for the timer
					if (timeDiff > 0.8 * mp.TOTAL_TIME)
                    {
						Main.NewText("Crit!");
						int spread = 20; //The angle of random spread.
						float spreadMult = 0.1f; //Multiplier for bullet spread, set it higher and it will make for some outrageous spread.
						for (int i = 0; i < 3; i++)
						{
							float vX = speedX + (float)Main.rand.Next(-spread, spread + 1) * spreadMult;
							float vY = speedY + (float)Main.rand.Next(-spread, spread + 1) * spreadMult;
							Projectile.NewProjectile(position.X, position.Y, vX, vY, type, damage, knockBack, Main.myPlayer);
						}
					}
					return true;

				} else // outside of the two times
                {
					Main.NewText("Missed Time!");
					return false;
				}
            } else // this will start the bar
            {
				mp.barPresent = true;
				return false;
            }
			**/

			return true;
		}
		/**
		 * if (player.altFunctionUse == 2)
			{	
				/**
				int spread = 90; //The angle of random spread.
				float spreadMult = 0.1f; //Multiplier for bullet spread, set it higher and it will make for some outrageous spread.
				for (int i = 0; i < 3; i++)
				{
					float vX = speedX + (float)Main.rand.Next(-spread, spread + 1) * spreadMult;
					float vY = speedY + (float)Main.rand.Next(-spread, spread + 1) * spreadMult;
					Projectile.NewProjectile(position.X, position.Y, vX, vY, type, damage, knockBack, Main.myPlayer);
				}
				return false;
			} else
            {
				/**
				if (player.controlUseItem) // if held down?
                {
					Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, Main.myPlayer);
				}
				Projectile.NewProjectile(position.X, position.Y, 0, 0, type, damage, knockBack, Main.myPlayer);
			}
			return false;
			
		}
		**/


		public override void AddRecipes() 
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.DirtBlock, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}

		
	}
}
public class ChargePlayer : ModPlayer
{
	// Fields
	public int oldTime = 0; // the timer to detect if you pressed it between
	public int TOTAL_TIME = 120;
	public bool barPresent = false;

	public override void OnEnterWorld(Player player)
	{
		barPresent = false;
	}
	public override void ResetEffects()
	{
		if (!barPresent)
		{
			oldTime = (int)Main.GameUpdateCount;
		}

		if ((int)Main.GameUpdateCount - oldTime >= TOTAL_TIME) // ran out of time
        {
			barPresent = false; // sets it to false
        }
	}
}