using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;

namespace BasicMod.Items.Accessories
{
	public class AmmoStash : ModItem
	{
		public override string Texture => "Terraria/Item_" + ItemID.EndlessMusketPouch;
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("A stash of ammo, just in case." +
				"\nGives a 20% chance to not consume ammo.");
		}

		public override void SetDefaults()
		{
			item.width = 24;
			item.height = 28;
			item.value = 100;
			item.rare = ItemRarityID.Blue;
			item.accessory = true;
			item.defense = 5;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<BasicModPlayer>().ammoStash = true;
		}

	}
}