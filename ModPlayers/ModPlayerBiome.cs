using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.World.Generation;
using System.Collections;
using System;

namespace BasicMod
{
    class ModPlayerBiome : ModPlayer
    {
		public bool ZoneExample;
		public override void UpdateBiomes()
		{
			ZoneExample = BasicWorld.bleckTiles > 200;
		}

		public override bool CustomBiomesMatch(Player other)
		{
			ModPlayerBiome modOther = other.GetModPlayer<ModPlayerBiome>();
			return ZoneExample == modOther.ZoneExample;
			// If you have several Zones, you might find the &= operator or other logic operators useful:
			// bool allMatch = true;
			// allMatch &= ZoneExample == modOther.ZoneExample;
			// allMatch &= ZoneModel == modOther.ZoneModel;
			// return allMatch;
			// Here is an example just using && chained together in one statemeny 
			// return ZoneExample == modOther.ZoneExample && ZoneModel == modOther.ZoneModel;
		}

		public override void CopyCustomBiomesTo(Player other)
		{
			ModPlayerBiome modOther = other.GetModPlayer<ModPlayerBiome>();
			modOther.ZoneExample = ZoneExample;
		}

		public override void SendCustomBiomes(BinaryWriter writer)
		{
			BitsByte flags = new BitsByte();
			flags[0] = ZoneExample;
			writer.Write(flags);
		}

		public override void ReceiveCustomBiomes(BinaryReader reader)
		{
			BitsByte flags = reader.ReadByte();
			ZoneExample = flags[0];
		}

		public override void UpdateBiomeVisuals()
		{
		}

		public override Texture2D GetMapBackgroundImage()
		{
			if (ZoneExample)
			{
				return mod.GetTexture("Backgrounds/ExampleBiomeMapBackground");
			}
			return null;
		}
	}

	public class ExampleUgBgStyle : ModUgBgStyle
	{
		public override bool ChooseBgStyle()
		{
			return Main.LocalPlayer.GetModPlayer<ModPlayerBiome>().ZoneExample;
		}

		public override void FillTextureArray(int[] textureSlots)
		{
			textureSlots[0] = mod.GetBackgroundSlot("Backgrounds/BleckBG02");
			textureSlots[1] = mod.GetBackgroundSlot("Backgrounds/BleckBG13");
			textureSlots[2] = mod.GetBackgroundSlot("Backgrounds/BleckBG02");
			textureSlots[3] = mod.GetBackgroundSlot("Backgrounds/BleckBG13");
		}
	}
}
