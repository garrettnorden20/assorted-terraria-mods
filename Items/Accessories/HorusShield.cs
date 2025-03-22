using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace BasicMod.Items.Accessories
{
	public class HorusShield : ModItem
	{
		//public override string Texture => "Terraria/Item_" + ItemID.MagmaStone;
		public override void SetStaticDefaults() 
		{
			Tooltip.SetDefault("Imbued with the power of an ancient god." +
				"\nGrants immunity to knockback and fire blocks." +
				"\nGrants immunity to most debuffs." + 
				"\nInflicts many status effects on nearby enemies." +
				"\nIncreases flight time and ascent speed greatly.");
		}

		public override void SetDefaults()
		{
			item.width = 24;
			item.height = 28;
			item.value = 100;
			item.rare = ItemRarityID.Lime;
			item.accessory = true;
			item.defense = 8;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			SetBuff(player.Center);

			player.buffImmune[BuffID.Bleeding] = true;
			player.buffImmune[BuffID.BrokenArmor] = true;
			player.buffImmune[BuffID.Burning] = true;
			player.buffImmune[BuffID.Confused] = true;
			player.buffImmune[BuffID.Cursed] = true;
			player.buffImmune[BuffID.Darkness] = true;
			player.buffImmune[BuffID.Poisoned] = true;
			player.buffImmune[BuffID.Silenced] = true;
			player.buffImmune[BuffID.Slow] = true;
			player.buffImmune[BuffID.Weak] = true;
			player.buffImmune[BuffID.Chilled] = true;
			player.buffImmune[BuffID.OnFire] = true;

			player.noKnockback = true;
			player.jumpSpeedBoost += 3.5f; // makes jump height taller/faster
			player.wingTimeMax += 120;

			player.fireWalk = true;


		}
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(mod.ItemType("CalamitousPresence"));
			recipe.AddIngredient(ItemID.AnkhShield);
			recipe.AddTile(TileID.TinkerersWorkbench);
			recipe.SetResult(this);
			recipe.AddRecipe();

		}

		public void SetBuff(Vector2 pos) // avoid is one you already hit
		{
			float setDist = 800;
			float newDist = 0;
			for (int i = 0; i < Terraria.Main.npc.Length - 1; i++) //Do once for each NPC in the world
			{
				if (Terraria.Main.npc[i].friendly == true)//Don't target town NPCs
					continue;
				if (Terraria.Main.npc[i].active == false)//Don't target dead NPCs
					continue;
				if (Terraria.Main.npc[i].damage == 0)//Don't target non-aggressive NPCs
					continue;
				else
				{
					newDist = Vector2.Distance(pos, Terraria.Main.npc[i].position);//Check the distance to the current NPC
					if (newDist < setDist)
					{ //If closer than the required distance 

						//Terraria.Main.npc[i].AddBuff(BuffID.OnFire, 20);
						Terraria.Main.npc[i].AddBuff(BuffID.CursedInferno, 20);
						Terraria.Main.npc[i].AddBuff(BuffID.ShadowFlame, 20);
						Terraria.Main.npc[i].AddBuff(BuffID.Ichor, 20);
						Terraria.Main.npc[i].AddBuff(BuffID.Frostburn, 20);
						Terraria.Main.npc[i].AddBuff(BuffID.Venom, 20);
					}
				}
			}
		}
	}
}