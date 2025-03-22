using Terraria.ID;
using Terraria.ModLoader;

namespace BasicMod.Items.Materials
{
	public class TeslaRemnant : ModItem
	{
		public override string Texture => "Terraria/Item_" + ItemID.Sapphire;
		public override void SetStaticDefaults()
		{
			ItemID.Sets.SortingPriorityMaterials[item.type] = 59; // influences the inventory sort order. 59 is PlatinumBar, higher is more valuable.
			Tooltip.SetDefault("An electrified power source of a fearsome machine.");
		}

		public override void SetDefaults()
		{
			item.rare = ItemRarityID.Cyan;
			item.maxStack = 999;
			item.createTile = ModContent.TileType<Tiles.Ores.SeleniumOreTile>();
			item.width = 12;
			item.height = 12;
			item.value = 3000;
		}
	}
}
