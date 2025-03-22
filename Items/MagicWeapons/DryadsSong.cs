using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace BasicMod.Items.MagicWeapons
{
	public class DryadsSong : ModItem
	{

		//public override string Texture => "Terraria/Item_" + ItemID.MagnetSphere;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Song of the Last Dryad");
			Tooltip.SetDefault("Nature's power courses through you...");
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

			item.magic = true;
			item.mana = 20;
			item.damage = 200;
			item.shootSpeed = 10;
			item.autoReuse = true;
			item.useTime = 8;
			item.useAnimation = 60;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.useAmmo = AmmoID.None;
			item.shoot = mod.ProjectileType("LeafProj");
			item.width = 24;
			item.height = 28;
			item.noMelee = true;
			item.channel = true; // something to do with holding item
			item.knockBack = 8; // doesn't matter, will be overriden in LeafProj anyway
			item.rare = ItemRarityID.Purple;
			item.value = Item.sellPrice(gold: 50);
			item.UseSound = SoundID.Item9; // ????


		}
		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack) //This lets you modify the firing of the item
		{
				if (player.controlUseItem) // if held down? probably unnecessary
				{
					Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, Main.myPlayer);
				}
			return false;

		}



		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.FragmentNebula, 10);
			recipe.AddIngredient(ItemID.PurificationPowder, 5);
			recipe.AddIngredient(ItemID.LeafBlower, 1);
			recipe.AddTile(TileID.LunarCraftingStation); // Ancient Manipulator
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}