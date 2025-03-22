using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent.Dyes;
using Terraria.GameContent.UI;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;
using BasicMod.UI;
using Terraria.Graphics;
using System.Collections;

namespace BasicMod
{
	public class BasicMod : Mod
	{
		public static ModHotKey WallToggle;

		internal UserInterface MyInterface;
		//internal PotionUI MyUI;
		//internal BarUI MyUI;
		internal BarUI MyUI;

		public override void Load()
		{
			WallToggle = RegisterHotKey("Toggle Drill Containemnt Unit", "P");

			if (!Main.dedServ)
			{
				MyInterface = new UserInterface();

				//MyUI = new PotionUI();
				MyUI = new BarUI();
				//MyUI = new BiomeEnterUI();

				MyUI.Activate(); // Activate calls Initialize() on the UIState if not initialized, then calls OnActivate and then calls Activate on every child element
				MyInterface?.SetState(MyUI);
			}

			ModTranslation text = CreateTranslation("SwatDefeat");
			text.SetDefault("You drove the Swatsaurs to extinction!");
			AddTranslation(text);

			text = CreateTranslation("SwatEventStart");
			text.SetDefault("The Swat Militia is coming!");
			AddTranslation(text);

			text = CreateTranslation("SwatHardStart");
			text.SetDefault("The Swat Militia ready for a rematch!");
			AddTranslation(text);

			text = CreateTranslation("SwatMilitia");
			AddTranslation(text);

			text = CreateTranslation("SwatMilitiaCleared");
			text.SetDefault("Cleared ");
			AddTranslation(text);
		}

		public override void Unload()
		{
			// Unload static references
			// You need to clear static references to assets (Texture2D, SoundEffects, Effects). 
			// In addition to that, if you want your mod to completely unload during unload, you need to clear static references to anything referencing your Mod class
			WallToggle = null;
		}

		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			BasicModMessageType msgType = (BasicModMessageType)reader.ReadByte();
			switch (msgType)
			{
				case BasicModMessageType.StartSwatEvent:
					BasicWorld.SwatEvent = true;
					BasicWorld.SwatKillCount = 0;
					if (Main.netMode == NetmodeID.Server)
					{
						NetMessage.SendData(MessageID.WorldData); // Immediately inform clients of new world state.
					}

					break;
				
			}
		}

		public override void UpdateMusic(ref int music, ref MusicPriority priority)
		{
			if (Main.myPlayer == -1 || Main.gameMenu || !Main.LocalPlayer.active)
			{
				return;
			}
			// Make sure your logic here goes from lowest priority to highest so your intended priority is maintained.
			if (Main.LocalPlayer.GetModPlayer<ModPlayerBiome>().ZoneExample)
			{
				music = GetSoundSlot(SoundType.Music, "Sounds/Music/BleckMusic");
				priority = MusicPriority.BiomeLow;
			}
		}

		private GameTime _lastUpdateUiGameTime;

		public override void UpdateUI(GameTime gameTime)
		{
			_lastUpdateUiGameTime = gameTime;
			if (MyInterface?.CurrentState != null)
			{
				MyInterface.Update(gameTime);
			}
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			BasicWorld modWorld = (BasicWorld)GetModWorld("BasicWorld");
			if (BasicWorld.SwatEvent)
			{
				int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
				LegacyGameInterfaceLayer orionProgress = new LegacyGameInterfaceLayer("Swat Militia",
					delegate
					{
						DrawSwatEvent(Main.spriteBatch);
						return true;
					},
					InterfaceScaleType.UI);
				layers.Insert(index, orionProgress);
			}

			int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
			if (mouseTextIndex != -1)
			{
				layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
					"MyMod: MyInterface",
					delegate
					{
						if (_lastUpdateUiGameTime != null && MyInterface?.CurrentState != null)
						{
							//MyInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
						}
						return true;
					},
					   InterfaceScaleType.UI));
			}
		}

		// adding negative vector flips it
		float increaser = 0;
		bool countDown = false;
		float increaserAmt = 0.05f;
        public override void ModifyTransformMatrix(ref SpriteViewMatrix Transform)
        {
			/**
			if (increaser > 5)
            {
				countDown = true;
            } 
			else if (increaser < 0)
            {
				countDown = false;
            }

			if (countDown)
            {
				increaser -= increaserAmt;
            } else
            {
				increaser += increaserAmt;
			}

			Transform.Zoom += new Vector2 (5 - increaser, increaser);
			**/
        }

        public void DrawSwatEvent(SpriteBatch spriteBatch)
		{
			if (BasicWorld.SwatEvent && !Main.gameMenu)
			{
				float scaleMultiplier = 0.5f + 1 * 0.5f;
				float alpha = 0.5f;
				Texture2D progressBg = Main.colorBarTexture;
				Texture2D progressColor = Main.colorBarTexture;
				Texture2D orionIcon = GetTexture("Items/Summonables/SwatEgg");
				const string orionDescription = "Swat Militia";
				Color descColor = new Color(39, 86, 134);

				Color waveColor = new Color(255, 241, 51);
				Color barrierColor = new Color(255, 241, 51);

				try
				{
					//draw the background for the waves counter
					const int offsetX = 20;
					const int offsetY = 20;
					int width = (int)(200f * scaleMultiplier);
					int height = (int)(46f * scaleMultiplier);
					Rectangle waveBackground = Utils.CenteredRectangle(new Vector2(Main.screenWidth - offsetX - 100f, Main.screenHeight - offsetY - 23f), new Vector2(width, height));
					Utils.DrawInvBG(spriteBatch, waveBackground, new Color(63, 65, 151, 255) * 0.785f);

					//draw wave text

					string waveText = Language.GetTextValue("Mods.BasicMod.SwatMilitiaCleared") + (int)(((float)BasicWorld.SwatKillCount / 150f) * 100) + "%"; // IMP
					Utils.DrawBorderString(spriteBatch, waveText, new Vector2(waveBackground.X + waveBackground.Width / 2, waveBackground.Y), Color.White, scaleMultiplier, 0.5f, -0.1f);

					//draw the progress bar

					if (BasicWorld.SwatKillCount == 0)
					{
					}
					// Main.NewText(MathHelper.Clamp((modWorld.SwatKillCount/modWorld.MaxSwatKillCount), 0f, 1f));
					Rectangle waveProgressBar = Utils.CenteredRectangle(new Vector2(waveBackground.X + waveBackground.Width * 0.5f, waveBackground.Y + waveBackground.Height * 0.75f), new Vector2(progressColor.Width, progressColor.Height));
					Rectangle waveProgressAmount = new Rectangle(0, 0, (int)(progressColor.Width * MathHelper.Clamp(((float)BasicWorld.SwatKillCount / 150f), 0f, 1f)), progressColor.Height);
					Vector2 offset = new Vector2((waveProgressBar.Width - (int)(waveProgressBar.Width * scaleMultiplier)) * 0.5f, (waveProgressBar.Height - (int)(waveProgressBar.Height * scaleMultiplier)) * 0.5f);

					spriteBatch.Draw(progressBg, waveProgressBar.Location.ToVector2() + offset, null, Color.White * alpha, 0f, new Vector2(0f), scaleMultiplier, SpriteEffects.None, 0f);
					spriteBatch.Draw(progressBg, waveProgressBar.Location.ToVector2() + offset, waveProgressAmount, waveColor, 0f, new Vector2(0f), scaleMultiplier, SpriteEffects.None, 0f);

					//draw the icon with the event description

					//draw the background
					const int internalOffset = 6;
					Vector2 descSize = new Vector2(154, 40) * scaleMultiplier;
					Rectangle barrierBackground = Utils.CenteredRectangle(new Vector2(Main.screenWidth - offsetX - 100f, Main.screenHeight - offsetY - 19f), new Vector2(width, height));
					Rectangle descBackground = Utils.CenteredRectangle(new Vector2(barrierBackground.X + barrierBackground.Width * 0.5f, barrierBackground.Y - internalOffset - descSize.Y * 0.5f), descSize);
					Utils.DrawInvBG(spriteBatch, descBackground, descColor * alpha);

					//draw the icon
					int descOffset = (descBackground.Height - (int)(32f * scaleMultiplier)) / 2;
					Rectangle icon = new Rectangle(descBackground.X + descOffset, descBackground.Y + descOffset, (int)(32 * scaleMultiplier), (int)(32 * scaleMultiplier));
					spriteBatch.Draw(orionIcon, icon, Color.White);

					//draw text

					Utils.DrawBorderString(spriteBatch, Language.GetTextValue("BasicMod.SwatMilitia"), new Vector2(barrierBackground.X + barrierBackground.Width * 0.5f, barrierBackground.Y - internalOffset - descSize.Y * 0.5f), Color.White, 0.80f, 0.3f, 0.4f);
				}
				catch (Exception e)
				{
					ErrorLogger.Log(e.ToString());
				}
			}
		}

		public float zoomIncrease = 1.0f;

	}


	internal enum BasicModMessageType : byte
	{
		EnemiesKilled,
		BasicModPlayerSyncPlayer,
		StartSwatEvent,
		UpdateWorldMeter
	}
}