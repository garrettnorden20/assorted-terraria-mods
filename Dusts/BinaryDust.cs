using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace BasicMod.Dusts
{
	public class BinaryDust : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.noGravity = true;
			dust.scale = 1.25f;

		}
		
	}
}