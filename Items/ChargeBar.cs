using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections;
using System;

namespace BasicMod.Items
{
    class ChargeBar : ModItem
    {
		public override string Texture => "Terraria/Item_" + ItemID.LifeFruit;
		public override void SetDefaults()
		{
			item.width = 34;
			item.height = 34;
			item.accessory = true;
			item.value = 150000;
			item.rare = ItemRarityID.Pink;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			// To assign the player the frostBurnSummon effect, we can't do player.frostBurnSummon = true because Player doesn't have frostBurnSummon. Be sure to remember to call the GetModPlayer method to retrieve the ModPlayer instance attached to the specified Player.
			//player.GetModPlayer<ChargePlayer>().FrostBurnSummon = true;
		}
	}
}
