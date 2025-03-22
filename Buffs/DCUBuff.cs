using Terraria;
using Terraria.ModLoader;

namespace BasicMod.Buffs
{
	public class DCUBuff : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Drill Mount");
			Description.SetDefault("Riding in a flying drill");
			Main.buffNoTimeDisplay[Type] = true;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.mount.SetMount(ModContent.MountType<Mounts.DrillContain>(), player);
			player.buffTime[buffIndex] = 10;
		}
	}
}
