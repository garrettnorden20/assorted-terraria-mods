using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;
using BasicMod.Items.SupportOrbs;
using Terraria;
using Microsoft.Xna.Framework;

namespace BasicMod.Items.Accessories
{
	public class OrbHastener : ModItem
	{
		public override string Texture => "Terraria/Item_" + ItemID.GhostBanner;
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("A magical, gleaming artifact." +
				"\n60% increased support orb use speed.");
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
			player.GetModPlayer<SupportOrbPlayer>().orbTimeMult += .60f;
		}
	}
}