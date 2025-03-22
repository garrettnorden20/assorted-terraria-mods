using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;

namespace BasicMod.Items.Accessories
{
	public class WoolenGlove : ModItem
	{
		public override string Texture => "Terraria/Item_" + ItemID.TitanGlove;
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("A pair of tough but cold gloves." +
				"\nHas a chance to inflict frostburn on melee attacks.");
		}

		public override void SetDefaults()
		{
			item.width = 24;
			item.height = 28;
			item.value = 100;
			item.rare = ItemRarityID.Blue;
			item.accessory = true;
			item.defense = 3;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<BasicModPlayer>().woolenGlove = true;
		}
	}
}