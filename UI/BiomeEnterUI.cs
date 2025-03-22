using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace BasicMod.UI
{
	// ExampleUIs visibility is toggled by typing "/coin" in chat. (See CoinCommand.cs)
	// ExampleUI is a simple UI example showing how to use UIPanel, UIImageButton, and even a custom UIElement.
	internal class BiomeEnterUI : UIState
	{
		private UIText panel;
		private String defaultText = " ";

		// In OnInitialize, we place various UIElements onto our UIState (this class).
		// UIState classes have width and height equal to the full screen, because of this, usually we first define a UIElement that will act as the container for our UI.
		// We then place various other UIElement onto that container UIElement positioned relative to the container UIElement.
		public override void OnInitialize()
		{
			/**
			UIPanel panel = new UIPanel();
			panel.Width.Set(300, 0);
			panel.Height.Set(300, 0);
			panel.HAlign = panel.VAlign = 0.5f; // 1
			panel.BackgroundColor = null;
			Append(panel);
			**/
			panel = new UIText(defaultText);
			panel.Width.Set(300, 0);
			panel.Height.Set(300, 0);
			panel.HAlign = panel.VAlign = 0.5f;
			panel.Top.Set(25, 0);
			Append(panel);
		}

		public override void Update(GameTime gameTime)
		{
			Player p = Main.player[Main.myPlayer]; // must stay here and not be field, unless you reinitialize it each time
			String textToSet = defaultText;
			if (p.ZoneDesert)
            {
				//textToSet = "The Desert";
            }

            panel.TextColor = new Color(255, 124, 255, 0);
			panel.SetText(textToSet);
			Recalculate();
		}

	}
}
