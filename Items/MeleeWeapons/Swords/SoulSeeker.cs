using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BasicMod.Items.MeleeWeapons.Swords
{
	public class SoulSeeker : ModItem
	{

		public override string Texture => "Terraria/Item_" + ItemID.Excalibur;
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Blade that grows stronger with every kill.");  //The (English) text shown below your weapon's name
		}

		public override void SetDefaults()
		{
			item.damage = 50; // The damage your item deals
			item.melee = true; // Whether your item is part of the melee class
			item.width = 40; // The item texture's width
			item.height = 40; // The item texture's height
			item.useTime = 30; // The time span of using the weapon. Remember in terraria, 60 frames is a second.
			item.useAnimation = 30; // The time span of the using animation of the weapon, suggest setting it the same as useTime.
			item.knockBack = 4.5f; // The force of knockback of the weapon. Maximum is 20
			item.value = Item.buyPrice(gold: 5); // The value of the weapon in copper coins
			item.rare = ItemRarityID.Green; // The rarity of the weapon, from -1 to 13. You can also use ItemRarityID.TheColorRarity
			item.UseSound = SoundID.Item1; // The sound when the weapon is being used
			item.autoReuse = true; // Whether the weapon can be used more than once automatically by holding the use button
			item.crit = 0; // The critical strike chance the weapon has. The player, by default, has 4 critical strike chance

			//The useStyle of the item. 
			//Use useStyle 1 for normal swinging or for throwing
			//use useStyle 2 for an item that drinks such as a potion,
			//use useStyle 3 to make the sword act like a shortsword
			//use useStyle 4 for use like a life crystal,
			//and use useStyle 5 for staffs or guns
			item.useStyle = ItemUseStyleID.SwingThrow; // 1 is the useStyle
		}

		public override void AddRecipes()
		{
			/**
			ModRecipe recipe = new ModRecipe(mod);
			// ItemType<ExampleItem>() is how to get the ExampleItem item, 10 is the amount of that item you need to craft the recipe
			recipe.AddIngredient(ModContent.ItemType<ExampleItem>(), 10);
			// You can use recipe.AddIngredient(ItemID.TheItemYouWantToUse, the amount of items needed); for a vanilla item.
			recipe.AddTile(ModContent.TileType<ExampleWorkbench>()); // Set the crafting tile to ExampleWorkbench
			recipe.SetResult(this); // Set the result to this item (ExampleSword)
			recipe.AddRecipe(); // When your done, add the recipe
			**/
			//TODO: recipe
		}

		public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			
		}

		public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
		{
			// Add the Onfire buff to the NPC for 1 second when the weapon hits an NPC
			// 60 frames = 1 second
			target.AddBuff(BuffID.Bleeding, 60);

			if (!target.SpawnedFromStatue && (target.life <= 0))
			{ // if target would be dead
				BasicModPlayer p = player.GetModPlayer<BasicModPlayer>();
				p.enemiesKilled += 0.1f;
				//Main.NewText("on kill: enemies killed " + p.enemiesKilled);
			}
		}

        public override void ModifyHitNPC(Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
        {
			for (int d = 0; d < 100; d++)
			{
				Dust.NewDust(target.Center, target.width, target.height, 5); // blood
			}

        }

        public override void ModifyWeaponDamage(Player player, ref float add, ref float mult, ref float flat)
        {
			BasicModPlayer p = player.GetModPlayer<BasicModPlayer>();
			flat += p.enemiesKilled;
			//Main.NewText("adding damage: enemies killed " + p.enemiesKilled);
		}




    }
}
