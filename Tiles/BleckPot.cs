using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace BasicMod.Tiles
{
	public class BleckPot : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileCut[Type] = true;
			Main.tileSolid[Type] = false;
			Main.tileBrick[Type] = false;
			//Main.tileMergeDirt[Type] = false;
			Main.tileBlockLight[Type] = true;
			Main.tileFrameImportant[Type] = true;
			//TileID.Sets.Falling[Type] = true;
			AddMapEntry(new Color(200, 200, 200));

			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileID.Sets.HasOutlines[Type] = false;
			
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2); //this style already takes care of direction for us
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
			TileObjectData.addTile(Type);
			
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Pot");
			
			disableSmartCursor = true;
			adjTiles = new int[] { TileID.Beds };
		}

        public override bool CanExplode(int i, int j)
        {
			return true;
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
			return true;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
			Main.PlaySound(13, i * 16, j * 16);

			Item.NewItem(new Vector2(i * 16, j * 16), ItemID.UglySweater, 1);
		}
    }

	public class BleckPotItem : ModItem
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
			item.createTile = ModContent.TileType<Tiles.BleckPot>();
			item.width = 12;
			item.height = 12;
			item.value = 3000;
		}
	}
}