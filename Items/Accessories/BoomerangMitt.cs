using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;

namespace BasicMod.Items.Accessories
{
	public class BoomerangMitt : ModItem
	{
		public override string Texture => "Terraria/Item_" + ItemID.DivingGear;
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("A glove that enhances your throwing abilities." +
				"\nWhen using a boomerangs, fires another and increases damage.");
		}

		public override void SetDefaults()
		{
			item.width = 24;
			item.height = 28;
			item.value = 100;
			item.rare = ItemRarityID.Blue;
			item.accessory = true;
			item.defense = 1;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<BasicModPlayer>().boomerangMitt = true;
		}

		/**
		public override void AddRecipes()
		{
		}
		**/
	}
}