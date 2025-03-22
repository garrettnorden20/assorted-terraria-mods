using Terraria;
using Terraria.ModLoader;

namespace BasicMod.Buffs
{
	public class Zoomed : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Zoomed");
			Description.SetDefault("You're zoomed in!");
			Main.buffNoTimeDisplay[Type] = true;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
		}
	}
}
