using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BasicMod.Tiles
{
	public class BleckTile : ModTile
	{
		//Note: ExampleSand requires ExampleSandProjectile to work.
		//This is how the block works:
		//The block is placed on another solid block. When the block below it is destroyed , the original block gets destroyed and spawns a projectile
		//That projectile spawns dust. When that projectile hits another tile, it will create the sand tile again.

		//ExampleSand (the item) is just used for placing the tile, this isn't needed and can be placed in other ways

		public override void SetDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileBrick[Type] = true;
			//Main.tileMergeDirt[Type] = false;
			Main.tileBlockLight[Type] = true;
			//TileID.Sets.Falling[Type] = true;
			AddMapEntry(new Color(0, 0, 0));
		}

	}

	public class BleckTileItem : ModItem
	{
		public override string Texture => "Terraria/Item_" + ItemID.PalladiumOre;
		public override void SetStaticDefaults()
		{
			ItemID.Sets.SortingPriorityMaterials[item.type] = 58;
		}

		public override void SetDefaults()
		{
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.useTurn = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.autoReuse = true;
			item.maxStack = 999;
			item.consumable = true;
			item.createTile = ModContent.TileType<Tiles.BleckTile>();
			item.width = 12;
			item.height = 12;
			item.value = 3000;
		}
	}
}