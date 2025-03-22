using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;

namespace BasicMod.Items.Accessories
{
	public class RageBand : ModItem
	{
		public override string Texture => "Terraria/Item_" + ItemID.Blindfold;
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("A used headband filled with anger." +
				"\nIncreases damage and knockback of next melee attack when hit.");
		}

		public override void SetDefaults()
		{
			item.width = 24;
			item.height = 28;
			item.value = 100;
			item.rare = ItemRarityID.Blue;
			item.accessory = true;
			item.defense = 4;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<BasicModPlayer>().rageBand = true;
		}

	}
}