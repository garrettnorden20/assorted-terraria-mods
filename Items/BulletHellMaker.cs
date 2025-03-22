﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace BasicMod.Items
{
	public class BulletHellMaker : ModItem
	{

		public override string Texture => "Terraria/Item_" + ItemID.MagnetSphere;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("bullet hell");
			Tooltip.SetDefault("lmao");
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
			//item.CloneDefaults(ItemID.WoodenBow);

			item.ranged = true;
			item.mana = 0;
			item.damage = 200;
			item.shootSpeed = 8;
			item.autoReuse = true;
			item.useTime = 2;
			item.useAnimation = 2;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.useAmmo = AmmoID.None;
			item.shoot = mod.ProjectileType("BulletHellBullet");
			item.width = 24;
			item.height = 28;
			item.noMelee = true;
			item.channel = true; // something to do with holding item
			item.knockBack = 8; // doesn't matter, will be overriden in LeafProj anyway
			item.rare = ItemRarityID.Purple;
			item.value = Item.sellPrice(gold: 50);
			item.UseSound = SoundID.Item9; // ????


		}
		int bulletPattern = 1;

		Random rand = new Random();
		double deg = 0;
		double degChange = 5;
		int trueVelocity = 8;
		double gapDifference = 0;
		double GAP_DIFFERENCE_MAX = 30;
		double GAP_DIFFERENCE_MIN = -30;
		int timer = 0;

		bool countUp = false;
		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack) //This lets you modify the firing of the item
		{
			if (player.altFunctionUse == 2) // reset values
            {
				if (bulletPattern == 2) item.useTime = 30;
				else item.useTime = 2;
				deg = 0;
				timer = 0;
				degChange = 5;
				trueVelocity = 8;
                gapDifference = GAP_DIFFERENCE_MAX;
				return false;
			}

			switch (bulletPattern)
			{
				case 1:
					//alternateGapDifference(); // use this later
					//Main.NewText(gapDifference);
					Player p = Main.player[Main.myPlayer];
					position = p.position;

					deg = (deg + degChange + gapDifference + rand.Next(0, 5)) % 360; // degreeConstant; //The degrees, you can multiply projectile.ai[1] to make it orbit faster, may be choppy depending on the value
					double rad = deg * (Math.PI / 180); //Convert degrees to radians
					double dist = 100; //Distance away from the player

					Vector2 projectile = new Vector2();
					projectile.X = p.Center.X - (int)(Math.Cos(rad) * dist);
					projectile.Y = p.Center.Y - (int)(Math.Sin(rad) * dist);

					Vector2 velocity = Vector2.Subtract(projectile, p.position);
					velocity.Normalize();
					velocity = velocity * trueVelocity;

					speedX = velocity.X;
					speedY = velocity.Y;

					//type = rand.Next(1, 900);
					return true;
					break; 

				case 2:
					timer++;
					if (timer % 3 == 0 && item.useTime > 2)
                    {
						item.useTime--;
                    }

					p = Main.player[Main.myPlayer];
					position = p.position;

					deg = (rand.Next(0, 361) % 360); // degreeConstant; //The degrees, you can multiply projectile.ai[1] to make it orbit faster, may be choppy depending on the value
					rad = deg * (Math.PI / 180); //Convert degrees to radians
					dist = 100; //Distance away from the player

					projectile = new Vector2();
					projectile.X = p.Center.X - (int)(Math.Cos(rad) * dist);
					projectile.Y = p.Center.Y - (int)(Math.Sin(rad) * dist);

					velocity = Vector2.Subtract(projectile, p.position);
					velocity.Normalize();
					velocity = velocity * trueVelocity;

					speedX = velocity.X;
					speedY = velocity.Y;
					
					int spread = 10; //The angle of random spread.
					float spreadMult = 0.1f; //Multiplier for bullet spread, set it higher and it will make for some outrageous spread.
					for (int i = 0; i < 20; i++)
                    {
						float vX = speedX + (float)Main.rand.Next(-spread, spread + 1) * spreadMult;
						float vY = speedY + (float)Main.rand.Next(-spread, spread + 1) * spreadMult;
						velocity = new Vector2(vX, vY);
						Projectile.NewProjectile(position, (velocity * (float)(1.0 + rand.NextDouble())), type, damage, knockBack, Main.myPlayer);
					}
					return false;
					break; // optional

				
				default: return false;
			}

			


		}

        private void alternateGapDifference()
        {
			if (gapDifference <= GAP_DIFFERENCE_MIN)
			{
				countUp = true;
			}
			else if (gapDifference >= GAP_DIFFERENCE_MAX)
			{
				countUp = false;
			}
			if (countUp)
			{
				gapDifference += 0.5;
			}
			else
			{
				gapDifference -= 0.5;
			}
		}

        public override bool AltFunctionUse(Player player)
		{
			return true;
		}

		/**
		 * Player p = Main.player[Main.myPlayer];
			position = p.position;

			deg = (deg + degChange) % 360; // degreeConstant; //The degrees, you can multiply projectile.ai[1] to make it orbit faster, may be choppy depending on the value
			double rad = deg * (Math.PI / 180); //Convert degrees to radians
			double dist = 100; //Distance away from the player

			Vector2 projectile = new Vector2();
			projectile.X = p.Center.X - (int)(Math.Cos(rad) * dist);
			projectile.Y = p.Center.Y - (int)(Math.Sin(rad) * dist);

			Vector2 velocity = Vector2.Subtract(projectile, p.position);
			velocity.Normalize();
			velocity = velocity * trueVelocity;

			speedX = velocity.X;
			speedY = velocity.Y;

			return true;
		**/


		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.DirtBlock, 1);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
 