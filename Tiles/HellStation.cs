using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace BasicMod.Tiles
{
	public class HellStation : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileID.Sets.HasOutlines[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style4x2); //this style already takes care of direction for us
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
			TileObjectData.addTile(Type);
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("HellStation");
			AddMapEntry(new Color(200, 200, 200), name);
			//dustType = ModContent.DustType<Sparkle>();
			disableSmartCursor = true;
			adjTiles = new int[] { TileID.Beds };
		}

		public override bool HasSmartInteract()
		{
			return true;
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = 1;
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Item.NewItem(i * 16, j * 16, 64, 32, ModContent.ItemType<Items.Placeable.HellStationIcon>());
		}

		public override bool NewRightClick(int i, int j)
		{
			Main.NewText("Right Clicked");
			Player player = Main.LocalPlayer;

			Tile tile = Main.tile[i, j];

			i = i - tile.frameX / 18;
			j = j + 2;
			i += tile.frameX >= 72 ? 5 : 2;
			if (tile.frameY % 38 != 0)
			{
				j--;
			}

			Point center = new Point(i, j+1); // adjust the +1 to fix
			Vector2 newDiggerLoc = center.ToVector2();
			Projectile.NewProjectile(newDiggerLoc.ToWorldCoordinates(), Vector2.Zero, mod.ProjectileType("HellDigger"), 0, 0);

			/**
			int spawnX = i - tile.frameX / 18;
			int spawnY = j + 2;
			spawnX += tile.frameX >= 72 ? 5 : 2;
			if (tile.frameY % 38 != 0)
			{
				spawnY--;
			}
			player.FindSpawn();
			if (player.SpawnX == spawnX && player.SpawnY == spawnY)
			{
				player.RemoveSpawn();
				Main.NewText("Spawn point removed!", 255, 240, 20, false);
			}
			else if (Player.CheckSpawn(spawnX, spawnY))
			{
				player.ChangeSpawn(spawnX, spawnY);
				Main.NewText("Spawn point set!", 255, 240, 20, false);
			}
			**/
			return true;
		}

		public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;
			player.noThrow = 2;
			player.showItemIcon = true;
			player.showItemIcon2 = ModContent.ItemType<Items.Placeable.HellStationIcon>();
		}
	}
}