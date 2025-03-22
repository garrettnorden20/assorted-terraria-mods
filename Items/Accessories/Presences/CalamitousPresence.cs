using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace BasicMod.Items.Accessories.Presences
{
	public class CalamitousPresence : ModItem
	{
		public override string Texture => "Terraria/Item_" + ItemID.MagmaStone;
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Brutal and stained energy flows from you..." +
				"\nInflicts many status effects on nearby enemies.");
		}

		public override void SetDefaults()
		{
			item.width = 24;
			item.height = 28;
			item.value = 100;
			item.rare = ItemRarityID.Pink;
			item.accessory = true;
			item.defense = 6;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			SetBuff(player.Center);

		}
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(mod.ItemType("DreadfulPresence"));
			recipe.AddIngredient(ItemID.SoulofFright);
			recipe.AddIngredient(ItemID.SoulofMight);
			recipe.AddIngredient(ItemID.SoulofSight);
			recipe.AddIngredient(ItemID.Ichor, 10);
			recipe.AddTile(TileID.TinkerersWorkbench);
			recipe.SetResult(this);
			recipe.AddRecipe();


			recipe = new ModRecipe(mod);
			recipe.AddIngredient(mod.ItemType("DreadfulPresence"));
			recipe.AddIngredient(ItemID.SoulofFright);
			recipe.AddIngredient(ItemID.SoulofMight);
			recipe.AddIngredient(ItemID.SoulofSight);
			recipe.AddIngredient(ItemID.CursedFlame, 10);
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