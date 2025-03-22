//using ExampleMod.Items;
//using ExampleMod.NPCs;
//using ExampleMod.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
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
using BasicMod.Items;
using BasicMod.Tiles;
using BasicMod.Tiles.Ores;
using Terraria.GameContent.Biomes;
using System.Collections;

namespace BasicMod
{
	public class BasicWorld : ModWorld
	{

		public static bool downedTesla;

		public static bool SwatEvent = false;
		public static int SwatKillCount = 0;
		public static int MaxSwatKillCount = 150;
		public static bool downedSwatMilitia;
		public static bool downedSwatMilitiaHard;
		public static int worldMeter;
		public static int bleckTiles;

		public static ArrayList boomerangIds;
		public static ArrayList orbIds;




		public override void Initialize()
		{
			downedSwatMilitia = false;
			downedSwatMilitiaHard = false;
			downedTesla = false;
			worldMeter = 0;

			boomerangIds = new ArrayList(new int[] { 1324, 1825, 4818, 3030, 1918, 670, 561, 1513, 1122, 4760, 4764, 191, 284, 55, 119, mod.ItemType("Lunarang"), mod.ItemType("Beemerang"), mod.ItemType("MagicalBoomerang"), mod.ItemType("Skelerang") });
			orbIds = new ArrayList(new int[] { mod.ItemType("AttackOrb"), mod.ItemType("SupportOrb") });


			}

		public override TagCompound Save()
		{
			return new TagCompound
			{
				{"downedTesla", downedTesla},
				{"downedSwatMilitia", downedSwatMilitia},
				{"downedSwatMilitiaHard", downedSwatMilitiaHard},
				{"worldMeter", worldMeter}

			};
		}

		public override void Load(TagCompound tag)
		{
			downedTesla = tag.GetBool("downedTesla");
			downedSwatMilitia = tag.GetBool("downedSwatMilitia");
			downedSwatMilitiaHard = tag.GetBool("downedSwatMilitiaHard");
			worldMeter = tag.GetInt("worldMeter");

		}

		public override void NetSend(BinaryWriter writer)
		{
			BitsByte flags = new BitsByte();
			flags[0] = downedTesla;
			flags[4] = SwatEvent;
			flags[5] = downedSwatMilitia;
			flags[6] = downedSwatMilitiaHard;
			/**
			flags[1] = downedAncient;
			flags[2] = downedhydra;
			flags[3] = downedRuneGhost;
			flags[4] = downedB4;
			flags[5] = downedSwatMilitia;
			flags[6] = downedSwatMilitiaHard;
			flags[7] = downedTyrant;
			**/
			writer.Write(flags);
			/**
			flags = new BitsByte();
			flags[0] = hasSummonedFortressBoss;
			flags[1] = downedFortressBoss;
			flags[2] = downedBear;
			flags[3] = downedBlade;
			flags[4] = SwatEvent;
			flags[5] = downedNoetnap;
			writer.Write(flags);
			writer.Write(SwatKillCount);
			**/
			writer.Write(SwatKillCount);
			writer.Write(worldMeter);
		}

		public override void NetReceive(BinaryReader reader)
		{
			BitsByte flags = reader.ReadByte();
			downedTesla = flags[0];
			SwatEvent = flags[4];
			downedSwatMilitia = flags[5];
			downedSwatMilitiaHard = flags[6];
			/**
			downedAncient = flags[1];
			downedhydra = flags[2];
			downedRuneGhost = flags[3];
			downedB4 = flags[4];
			downedSwatMilitia = flags[5];
			downedSwatMilitiaHard = flags[6]; a
			downedSwatMilitiaHard = flags[7];
			flags = reader.ReadByte();
			hasSummonedFortressBoss = flags[0];
			downedFortressBoss = flags[1];
			downedBear = flags[2];
			downedBlade = flags[3];
			SwatEvent = flags[4];
			downedNoetnap = flags[5];
			SwatKillCount = reader.ReadInt32();
			**/
			SwatKillCount = reader.ReadInt32();
			worldMeter = reader.ReadInt32();
		}


		public override void PreUpdate()
		{
			RunWorldMeter();
			RunSwatEvent();
		}

		private void RunSwatEvent()
        {
			MaxSwatKillCount = 150;
			if (SwatKillCount >= MaxSwatKillCount)
			{
				SwatEvent = false;
				downedSwatMilitia = true;
				if (Main.netMode == NetmodeID.Server)
					NetMessage.SendData(MessageID.WorldData); // Immediately inform clients of new world state.
				string key = "Mods.BasicMod.SwatDefeat";
				Color messageColor = Color.Orange;
				if (Main.netMode == 2) // Server
				{
					NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
				}
				else if (Main.netMode == 0) // Single Player
				{
					Main.NewText(Language.GetTextValue(key), messageColor);
				}
				SwatKillCount = 0;
			}
		}

		private void RunWorldMeter()
        {
			//Main.NewText(worldMeter + "");
        }

		public override void TileCountsAvailable(int[] tileCounts)
		{
			// Here we count various tiles towards ZoneExample
			bleckTiles = tileCounts[ModContent.TileType<Tiles.BleckTile>()];

		}


		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
		{
			// Because world generation is like layering several images ontop of each other, we need to do some steps between the original world generation steps.

			// The first step is an Ore. Most vanilla ores are generated in a step called "Shinies", so for maximum compatibility, we will also do this.
			// First, we find out which step "Shinies" is.
			int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Pots"));
			if (ShiniesIndex != -1)
			{
				// Next, we insert our step directly after the original "Shinies" step. 
				// ExampleModOres is a method seen below.
				
				//tasks.Insert(ShiniesIndex + 1, new PassLegacy("Darkness Biome", DarkBiome));
			}

			int DesertIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));
			if (DesertIndex != -1)
			{

				tasks.Insert(DesertIndex + 1, new PassLegacy("Post Terrain", delegate (GenerationProgress progress) {
					progress.Message = "Bleck";
					int num598 = Main.maxTilesX / 2;
					int num599 = WorldGen.genRand.Next(num598) / 4;
					num599 += num598 / 8;
					int x11 = num598 + num599; // flip neg
					int num600 = 0;
					while (!Biomes<BleckBiome>.Place(new Point(x11, (int)(Main.rockLayer * 1.1)), WorldGen.structures))
					{
						num599 = WorldGen.genRand.Next(num598) / 2;
						num599 += num598 / 8;
						x11 = num598 + num599;
						if (++num600 > 1000)
						{

						}
					}

				}));
			}

			int LivingTreesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Living Trees"));
			if (LivingTreesIndex != -1)
			{
				/**
				tasks.Insert(LivingTreesIndex + 1, new PassLegacy("Post Terrain", delegate (GenerationProgress progress) {
					// We can inline the world generation code like this, but if exceptions happen within this code 
					// the error messages are difficult to read, so making methods is better. This is called an anonymous method.
					progress.Message = "Charging Up...";
					//MakeCharges();
				}));
				**/

				tasks.Insert(LivingTreesIndex + 2, new PassLegacy("Post Terrain", delegate (GenerationProgress progress) {
					// We can inline the world generation code like this, but if exceptions happen within this code 
					// the error messages are difficult to read, so making methods is better. This is called an anonymous method.
					progress.Message = "Building Igloos";
					MakeIgloos();
					
				}));
			}

			int HellforgeIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Hellforge"));
			if (HellforgeIndex != -1)
			{
				// Next, we insert our step directly after the original "Shinies" step. 
				// ExampleModOres is a method seen below.

				tasks.Insert(HellforgeIndex + 1, new PassLegacy("Make Moon", PlaceMoon));
			}


			/**
			// This second step that we add will go after "Traps" and follows the same pattern.
			int TrapsIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Traps"));
			if (TrapsIndex != -1)
			{
				tasks.Insert(TrapsIndex + 1, new PassLegacy("Example Mod Traps", ExampleModTraps));
			}

			
			**/
		}

		int[] nowMinX = new int[Main.maxTilesY];
		int[] nowMaxX = new int[Main.maxTilesY];


		ushort tileone = 0;
		ushort tiletwo = 0;

		private void DarkBiome(GenerationProgress progress)
        {
			tileone = (ushort)mod.TileType("BleckTile");
			tiletwo = (ushort)mod.TileType("BleckTile");

			progress.Message = "blahd";
			double snowTop = (int)Main.worldSurface;
			int num639 = WorldGen.genRand.Next(Main.maxTilesX);


			while ((float)num639 < (float)Main.maxTilesX * 0.55f || (float)num639 > (float)Main.maxTilesX * 0.7f)
			{
				num639 = WorldGen.genRand.Next(Main.maxTilesX);
			}

			num639 = Main.maxTilesX / 2;


			int num640 = WorldGen.genRand.Next(50, 90);
			float num641 = Main.maxTilesX / 4200;
			num640 += (int)((float)WorldGen.genRand.Next(20, 40) * num641);
			num640 += (int)((float)WorldGen.genRand.Next(20, 40) * num641);
			int num642 = num639 - num640;
			num640 = WorldGen.genRand.Next(50, 90);
			num640 += (int)((float)WorldGen.genRand.Next(20, 40) * num641);
			num640 += (int)((float)WorldGen.genRand.Next(20, 40) * num641);
			int num643 = num639 + num640;
			if (num642 < 0)
			{
				num642 = 0;
			}
			if (num643 > Main.maxTilesX)
			{
				num643 = Main.maxTilesX;
			}
			int num644 = 10;
			for (int num645 = (int)Main.rockLayer; num645 <= WorldGen.lavaLine - 140; num645++)
			{
				num642 += WorldGen.genRand.Next(-4, 4);
				num643 += WorldGen.genRand.Next(-3, 5);
				nowMinX[num645] = num642;
				nowMaxX[num645] = num643;
				for (int num646 = num642; num646 < num643; num646++)
				{
					if (num645 < WorldGen.lavaLine - 140)
					{
						if (Main.tile[num646, num645].wall != 0)
						{
							Main.tile[num646, num645].wall = 0;
						}
						switch (Main.tile[num646, num645].type)
						{
							case 0:
							case 2:
							case 23:
							case 40:
							case 53:
								Main.tile[num646, num645].type = tileone;
								break;
							case 1:
								Main.tile[num646, num645].type = tiletwo;
								break;
						}
					}
					else
					{
						num644 += WorldGen.genRand.Next(-3, 4);
						if (WorldGen.genRand.Next(3) == 0)
						{
							num644 += WorldGen.genRand.Next(-4, 5);
							if (WorldGen.genRand.Next(3) == 0)
							{
								num644 += WorldGen.genRand.Next(-6, 7);
							}
						}
						if (num644 < 0)
						{
							num644 = WorldGen.genRand.Next(3);
						}
						else if (num644 > 50)
						{
							num644 = 50 - WorldGen.genRand.Next(3);
						}
						for (int num647 = num645; num647 < num645 + num644; num647++)
						{
							if (Main.tile[num646, num647].wall == 2)
							{
								Main.tile[num646, num647].wall = 40;
							}
							switch (Main.tile[num646, num647].type)
							{
								case 0:
								case 2:
								case 23:
								case 40:
								case 53:
									//Main.tile[num646, num647].type = 147; // snow
									Main.tile[num646, num647].type = TileID.MartianConduitPlating; 
									break;
								case 1: // stone
									Main.tile[num646, num647].type = TileID.LunarBlockNebula; 
									break;
							}
						}
					}
				}
			}
		}

		private void ExampleModOres(GenerationProgress progress)
		{
			// progress.Message is the message shown to the user while the following code is running. Try to make your message clear. You can be a little bit clever, but make sure it is descriptive enough for troubleshooting purposes. 
			progress.Message = "Example Mod Ores";

			// Ores are quite simple, we simply use a for loop and the WorldGen.TileRunner to place splotches of the specified Tile in the world.
			// "6E-05" is "scientific notation". It simply means 0.00006 but in some ways is easier to read.
			for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-04); k++)
			{
				// The inside of this for loop corresponds to one single splotch of our Ore.
				// First, we randomly choose any coordinate in the world by choosing a random x and y value.
				int x = WorldGen.genRand.Next(0, Main.maxTilesX);
				int y = WorldGen.genRand.Next((int)WorldGen.worldSurface, Main.maxTilesY); // WorldGen.worldSurfaceLow is actually the highest surface tile. In practice you might want to use WorldGen.rockLayer or other WorldGen values.

				// Then, we call WorldGen.TileRunner with random "strength" and random "steps", as well as the Tile we wish to place. Feel free to experiment with strength and step to see the shape they generate.
				WorldGen.TileRunner(x, y, WorldGen.genRand.Next(6, 12), WorldGen.genRand.Next(4, 12), TileID.Hive);

				// Alternately, we could check the tile already present in the coordinate we are interested. Wrapping WorldGen.TileRunner in the following condition would make the ore only generate in Snow.
				// Tile tile = Framing.GetTileSafely(x, y);
				// if (tile.active() && tile.type == TileID.SnowBlock)
				// {
				// 	WorldGen.TileRunner(.....);
				// }
			}
		}

		/**
		private void ExampleModTraps(GenerationProgress progress)
		{
			progress.Message = "Example Mod Traps";

			// Computers are fast, so WorldGen code sometimes looks stupid.
			// Here, we want to place a bunch of tiles in the world, so we just repeat until success. It might be useful to keep track of attempts and check for attempts > maxattempts so you don't have infinite loops. 
			// The WorldGen.PlaceTile method returns a bool, but it is useless. Instead, we check the tile after calling it and if it is the desired tile, we know we succeeded.
			for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 6E-05); k++)
			{
				bool placeSuccessful = false;
				Tile tile;
				int tileToPlace = ModContent.TileType<ExampleCutTileTile>();
				while (!placeSuccessful)
				{
					int x = WorldGen.genRand.Next(0, Main.maxTilesX);
					int y = WorldGen.genRand.Next(0, Main.maxTilesY);
					WorldGen.PlaceTile(x, y, tileToPlace);
					tile = Main.tile[x, y];
					placeSuccessful = tile.active() && tile.type == tileToPlace;
				}
			}
		}

		

		private readonly int[,] _wellshape = {
			{0,0,3,1,4,0,0 },
			{0,3,1,1,1,4,0 },
			{3,1,1,1,1,1,4 },
			{5,5,5,6,5,5,5 },
			{5,5,5,6,5,5,5 },
			{5,5,5,6,5,5,5 },
			{2,1,5,6,5,1,2 },
			{1,1,5,5,5,1,1 },
			{1,1,5,5,5,1,1 },
			{0,1,5,5,5,1,0 },
			{0,1,5,5,5,1,0 },
			{0,1,5,5,5,1,0 },
			{0,1,5,5,5,1,0 },
			{0,1,5,5,5,1,0 },
			{0,1,5,5,5,1,0 },
			{0,1,5,5,5,1,0 },
			{0,1,5,5,5,1,0 },
			{0,1,1,1,1,1,0 },
		};
		private readonly int[,] _wellshapeWall = {
			{0,0,0,0,0,0,0 },
			{0,0,0,0,0,0,0 },
			{0,0,0,0,0,0,0 },
			{0,0,1,1,1,0,0 },
			{0,0,1,1,1,0,0 },
			{0,0,1,1,1,0,0 },
			{0,0,1,1,1,0,0 },
			{0,0,1,1,1,0,0 },
			{0,0,1,1,1,0,0 },
			{0,0,1,1,1,0,0 },
			{0,0,1,1,1,0,0 },
			{0,0,1,1,1,0,0 },
			{0,0,1,1,1,0,0 },
			{0,0,1,1,1,0,0 },
			{0,0,1,1,1,0,0 },
			{0,0,1,1,1,0,0 },
			{0,0,1,1,1,0,0 },
			{0,0,1,1,1,0,0 },
		};
		private readonly int[,] _wellshapeWater = {
			{0,0,0,0,0,0,0 },
			{0,0,0,0,0,0,0 },
			{0,0,0,0,0,0,0 },
			{0,0,0,0,0,0,0 },
			{0,0,0,0,0,0,0 },
			{0,0,0,0,0,0,0 },
			{0,0,0,0,0,0,0 },
			{0,0,1,1,1,0,0 },
			{0,0,1,1,1,0,0 },
			{0,0,1,1,1,0,0 },
			{0,0,1,1,1,0,0 },
			{0,0,1,1,1,0,0 },
			{0,0,1,1,1,0,0 },
			{0,0,1,1,1,0,0 },
			{0,0,1,1,1,0,0 },
			{0,0,1,1,1,0,0 },
			{0,0,1,1,1,0,0 },
			{0,0,0,0,0,0,0 },
		};

		public bool PlaceWell(int i, int j)
		{
			if (!WorldGen.SolidTile(i, j + 1))
			{
				return false;
			}
			if (Main.tile[i, j].active())
			{
				return false;
			}
			if (j < 150)
			{
				return false;
			}

			for (int y = 0; y < _wellshape.GetLength(0); y++)
			{
				for (int x = 0; x < _wellshape.GetLength(1); x++)
				{
					int k = i - 3 + x;
					int l = j - 6 + y;
					if (WorldGen.InWorld(k, l, 30))
					{
						Tile tile = Framing.GetTileSafely(k, l);
						switch (_wellshape[y, x])
						{
							case 1:
								tile.type = TileID.RedBrick;
								tile.active(true);
								break;
							case 2:
								tile.type = TileID.RedBrick;
								tile.active(true);
								tile.halfBrick(true);
								break;
							case 3:
								tile.type = TileID.RedBrick;
								tile.active(true);
								tile.slope(2);
								break;
							case 4:
								tile.type = TileID.RedBrick;
								tile.active(true);
								tile.slope(1);
								break;
							case 5:
								tile.active(false);
								break;
							case 6:
								tile.type = TileID.Rope;
								tile.active(true);
								break;
						}
						switch (_wellshapeWall[y, x])
						{
							case 1:
								tile.wall = WallID.RedBrick;
								break;
						}
						switch (_wellshapeWater[y, x])
						{
							case 1:
								tile.liquid = 255;
								break;
						}
					}
				}
			}
			return true;
		}
		**/


		private void PlaceMoon(GenerationProgress progress)
        {
			int numberToGenerate = 10;

			for (int k = 0; k < numberToGenerate; k++)
			{
				bool success = false;
				int attempts = 0;
				while (!success)
				{
					attempts++;
					if (attempts > 1000)
					{
						success = true;
						continue;
					}

					bool placementOK = true;

					int leftBound = (int)(Main.maxTilesX * 0.33f);
					int rightBound = (int)(Main.maxTilesX * 0.66f);
					int X = WorldGen.genRand.Next(leftBound, rightBound);
					int Y = (int)(Main.worldSurface * 0.35);
					int radius = 25;

					// all of below just checks each potential tile of the thing to see if it would overwrite cloud
					for (int l = X; l < X + radius; l++) // x bounds of thing
					{
						for (int m = Y - radius; m < Y + radius; m++) // y bounds of thing
						{
							if (Main.tile[l, m].active())
							{
								int type = (int)Main.tile[l, m].type;
								if (type == TileID.BlueDungeonBrick || type == TileID.GreenDungeonBrick || type == TileID.PinkDungeonBrick || type == TileID.Cloud || type == TileID.RainCloud || type == (ushort)ModContent.TileType<SeleniumOreTile>())
								{
									placementOK = false;
								}
							}
						}
					}
					if (placementOK)
					{
						success = MakeMoon(X, Y, radius);
					}
				}
			}
		}


		private bool MakeMoon(int X, int Y, int radius)
		{
			/**
			int leftBound = (int)(Main.maxTilesX * 0.33f);
			int rightBound = (int)(Main.maxTilesX * 0.66f);
			int X = WorldGen.genRand.Next(leftBound, rightBound);
			//int X = Main.spawnTileX - 30;
			int Y = 150;
			int radius = 25;
			**/
			for (int x = 0; x <= radius; x++)
			{
				for (int y = -radius; y <= radius; y++)
				{
					int ellipseFormula = ((3 * x) * (3 * x)) + (y * y); // put into desmos

					if ((Vector2.Distance(new Vector2(X, Y), new Vector2(x + X, y + Y)) <= radius) && ellipseFormula > (radius*radius))
					{
						Tile tile = Framing.GetTileSafely(x + X, y + Y);
						tile.type = (ushort)ModContent.TileType<SeleniumOreTile>();
						tile.active(true);
					}
				}
			}
			return true;

			
		}

		private void MakeCharges()
		{
			float widthScale = Main.maxTilesX / 2200f;
			int numberToGenerate = WorldGen.genRand.Next(2, (int)(3f * widthScale));
			//numCharges = 0;
			//chargePoints = new Point[numberToGenerate];
			for (int k = 0; k < numberToGenerate; k++)
			{
				bool success = false;
				int attempts = 0;
				while (!success)
				{
					attempts++;
					if (attempts > 1000)
					{
						success = true;
						continue;
					}
					int i = WorldGen.genRand.Next(300, Main.maxTilesX - 300); // random x within bounds of world (300, maxTiles-300). so not in oceans
					if (i <= Main.maxTilesX / 2 - 50 || i >= Main.maxTilesX / 2 + 50) // means if its not near spawn?
					{
						int j = 0;
						while (!Main.tile[i, j].active() && (double)j < Main.worldSurface) // while curr tile is not active and it's above the surface layer, keep going down
						{
							j++;
						}
						if (Main.tile[i, j].type == TileID.Dirt)
						{
							j--;
							if (j > 150) // don't place in sky islands, perhaps?
							{
								bool placementOK = true;

								// all of below just checks each potential tile of the thing to see if it would overwrite a dungeon brick.
								for (int l = i - 12; l < i + 12; l++) // x bounds of thing
								{
									for (int m = j - 6; m < j + 12; m++) // y bounds of thing
									{
										if (Main.tile[l, m].active())
										{
											int type = (int)Main.tile[l, m].type;
											if (type == TileID.BlueDungeonBrick || type == TileID.GreenDungeonBrick || type == TileID.PinkDungeonBrick || type == TileID.Cloud || type == TileID.RainCloud)
											{
												placementOK = false;
											}
										}
									}
								}
								if (placementOK)
								{
									success = PlaceCharge(i, j);
								}
							}
						}
					}
				}
			}
		}

		public bool PlaceCharge(int i, int j)
		{
			if (!WorldGen.SolidTile(i, j + 1)) // if tile directly below is not solid 
			{
				return false;
			}
			if (Main.tile[i, j].active()) // if the main tile is active
			{
				return false;
			}
			if (j < 150) // if it's near space of world
			{
				return false;
			}

			for (int y = 0; y < _chargeshape.GetLength(0); y++)
			{
				for (int x = 0; x < _chargeshape.GetLength(1); x++)
				{
					int k = i - 3 + x;
					int l = j - 6 + y;
					if (WorldGen.InWorld(k, l, 30))
					{
						Tile tile = Framing.GetTileSafely(k, l);
						switch (_chargeshape[y, x])
						{
							case 0:
								tile.active(false);
								break;

							case 1:
								tile.type = TileID.MartianConduitPlating;
								tile.active(true);
								break;
							/**
							case 2:
								tile.type = TileID.RedBrick;
								tile.active(true);
								tile.halfBrick(true);
								break;
							case 3:
								tile.type = TileID.RedBrick;
								tile.active(true);
								tile.slope(2);
								break;
							case 4:
								tile.type = TileID.RedBrick;
								tile.active(true);
								tile.slope(1);
								break;
							case 5:
								tile.active(false);
								break;
							
							case 6:
								tile.type = TileID.Rope;
								tile.active(true);
								break;
							**/
						}
						switch (_chargeshapeWall[y, x])
						{
							case 0:
								tile.wall = WallID.MartianConduit;
								break;
						}
						/**
						switch (_wellshapeWater[y, x])
						{
							case 1:
								tile.liquid = 255;
								break;
						}
						**/
					}
				}
			}
			
			return true;
		}

		private void MakeIgloos()
		{
			// s,m,l: 4200,6400,8400
			float widthScale = Main.maxTilesX / 2000;
			int numberToGenerate = WorldGen.genRand.Next(2, (int)(2f * widthScale));
			for (int k = 0; k < numberToGenerate; k++)
			{
				bool success = false;
				int attempts = 0;
				while (!success)
				{
					attempts++;
					if (attempts > 1000)
					{
						success = true;
						continue;
					}
					int i = WorldGen.genRand.Next(300, Main.maxTilesX - 300); // random x within bounds of world (300, maxTiles-300). so not in oceans
					if (i <= Main.maxTilesX / 2 - 50 || i >= Main.maxTilesX / 2 + 50) // means if its not near spawn?
					{
						int j = 0;
						while (!Main.tile[i, j].active() && (double)j < Main.worldSurface) // while curr tile is not active and it's above the surface layer, keep going down
						{
							j++;
						}
						if (Main.tile[i, j].type == TileID.SnowBlock)
						{
							j--;
							if (j > 150) // don't place in sky islands, perhaps?
							{
								bool placementOK = true;

								// all of below just checks each potential tile of the thing to see if it would overwrite a dungeon brick.
								for (int l = i - 12; l < i + 12; l++) // x bounds of thing
								{
									for (int m = j - 6; m < j + 12; m++) // y bounds of thing
									{
										if (Main.tile[l, m].active())
										{
											int type = (int)Main.tile[l, m].type;
											if (type == TileID.BlueDungeonBrick || type == TileID.GreenDungeonBrick || type == TileID.PinkDungeonBrick || type == TileID.Cloud || type == TileID.RainCloud)
											{
												placementOK = false;
											}
										}
									}
								}
								if (placementOK)
								{
									success = PlaceIgloo(i, j);
								}
							}
						}
					}
				}
			}
		}

		public bool PlaceIgloo(int i, int j)
		{
			Console.WriteLine(i + " " + j);
			if (!WorldGen.SolidTile(i, j + 1)) // if tile directly below is not solid 
			{
				return false;
			}
			if (Main.tile[i, j].active()) // if the main tile is active
			{
				return false;
			}
			if (j < 150) // if it's near space of world
			{
				return false;
			}

			int chestX = 0;
			int chestY = 0;
			// counted from top left
			for (int y = 0; y < _iglooshape.GetLength(0); y++)
			{
				for (int x = 0; x < _iglooshape.GetLength(1); x++)
				{
					// this makes it so the top left is shifted from just above the ground to higher above the ground, and in the middle. makes the structure line up better
					int k = i - (_iglooshape.GetLength(1)/2) + x;
					int l = j - (_iglooshape.GetLength(0)/2) + y;
					if (WorldGen.InWorld(k, l, 30))
					{
						Tile tile = Framing.GetTileSafely(k, l);

						if (y == 5 && x == 4)
                        {
							chestX = k;
							chestY = l;
						}

						switch (_iglooshape[y, x])
						{
							case 0:
								tile.active(false);
								break;

							case 1:
								tile.type = TileID.SnowBrick;
								tile.active(true);
								break;

							case 2:
								tile.type = TileID.IceBrick;
								tile.active(true);
								break;
							
							case 4:
								tile.active(false);
								WorldGen.PlaceChest(k, l);
								break;
								/**
								case 2:
									tile.type = TileID.RedBrick;
									tile.active(true);
									tile.halfBrick(true);
									break;
								case 3:
									tile.type = TileID.RedBrick;
									tile.active(true);
									tile.slope(2);
									break;
								case 4:
									tile.type = TileID.RedBrick;
									tile.active(true);
									tile.slope(1);
									break;
								case 5:
									tile.active(false);
									break;
								
								case 6:
									tile.type = TileID.Rope;
									tile.active(true);
									break;
								**/
						}
						switch (_iglooshapeWall[y, x])
						{
							case 0:
								tile.wall = WallID.IceFloeWallpaper;
								break;
							case 2:
								tile.wall = 0;
								break;
							case 9:
								tile.wall = 0;
								break;
						}
						/**
						switch (_wellshapeWater[y, x])
						{
							case 1:
								tile.liquid = 255;
								break;
						}
						**/
					}
				}
			}
			WorldGen.PlaceChest(chestX, chestY);

			return true;
		}

		private readonly int[,] _chargeshape = {
			{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
			{1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
			{1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
			{1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
			{1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
			{1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
			{1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
			{1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
			{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
		};


		private readonly int[,] _chargeshapeWall = {
			{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
			{1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
			{1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
			{1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
			{1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
			{1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
			{1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
			{1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
			{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
		};


		private readonly int[,] _iglooshape = {
			{0,0,2,2,2,2,2,2,0,0,0,0},
			{0,2,2,0,0,0,0,2,2,0,0,0},
			{2,2,0,0,0,0,0,0,2,2,2,2},
			{2,0,0,0,0,0,0,0,0,0,0,0},
			{2,0,0,0,0,0,0,0,0,0,0,0},
			{2,0,0,0,0,0,0,0,0,0,0,0},
			{1,1,1,1,1,1,1,1,1,1,1,1},
		};

		private readonly int[,] _iglooshapeWall = {
			{9,9,2,2,2,2,2,2,9,9,9,9},
			{9,2,2,0,0,0,0,2,2,9,9,9},
			{2,2,0,0,0,0,0,0,2,2,2,2},
			{2,0,0,0,0,0,0,0,0,0,9,9},
			{2,0,0,0,0,0,0,0,0,0,9,9},
			{2,0,0,0,0,0,0,0,0,0,9,9},
			{1,1,1,1,1,1,1,1,1,1,1,1},
		};

		// We can use PostWorldGen for world generation tasks that don't need to happen between vanilla world generation steps.
		public override void PostWorldGen()
		{
			// This is simply generating a line of Chlorophyte halfway down the world.
			//for (int i = 0; i < Main.maxTilesX; i++)
			//{
			//	Main.tile[i, Main.maxTilesY / 2].type = TileID.Chlorophyte;
			//}

			// Here we spawn Example Person just like the Guide.
			int num = NPC.NewNPC((Main.spawnTileX + 5) * 16, Main.spawnTileY * 16, NPCID.GingerbreadMan, 0, 0f, 0f, 0f, 0f, 255);
			Main.npc[num].homeTileX = Main.spawnTileX + 5;
			Main.npc[num].homeTileY = Main.spawnTileY;
			Main.npc[num].direction = 1;
			Main.npc[num].homeless = true;

			/**
			// Place some items in Ice Chests
			int[] itemsToPlaceInIceChests = { ModContent.ItemType<CarKey>(), ModContent.ItemType<ExampleLightPet>(), ItemID.PinkJellyfishJar };
			int itemsToPlaceInIceChestsChoice = 0;
			for (int chestIndex = 0; chestIndex < 1000; chestIndex++)
			{
				Chest chest = Main.chest[chestIndex];
				// If you look at the sprite for Chests by extracting Tiles_21.xnb, you'll see that the 12th chest is the Ice Chest. Since we are counting from 0, this is where 11 comes from. 36 comes from the width of each tile including padding. 
				if (chest != null && Main.tile[chest.x, chest.y].type == TileID.Containers && Main.tile[chest.x, chest.y].frameX == 11 * 36)
				{
					for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
					{
						if (chest.item[inventoryIndex].type == ItemID.None)
						{
							chest.item[inventoryIndex].SetDefaults(itemsToPlaceInIceChests[itemsToPlaceInIceChestsChoice]);
							itemsToPlaceInIceChestsChoice = (itemsToPlaceInIceChestsChoice + 1) % itemsToPlaceInIceChests.Length;
							// Alternate approach: Random instead of cyclical: chest.item[inventoryIndex].SetDefaults(Main.rand.Next(itemsToPlaceInIceChests));
							break;
						}
					}
				}
			}
			**/
		}




		/**
		public override void ResetNearbyTileEffects()
		{
			//ExamplePlayer modPlayer = Main.LocalPlayer.GetModPlayer<ExamplePlayer>();
			//modPlayer.voidMonolith = false;
			exampleTiles = 0;
		}

		public override void TileCountsAvailable(int[] tileCounts)
		{
			// Here we count various tiles towards ZoneExample
			exampleTiles = tileCounts[ModContent.TileType<ExampleBlock>()] + tileCounts[ModContent.TileType<ExampleSand>()];

			// We can also add to vanilla biome counts if appropriate. Here we are adding to the ZoneDesert since we have a sand tile in the mod.
			Main.sandTiles += tileCounts[ModContent.TileType<ExampleSand>()];
		}

		public override void PreUpdate()
		{
			// Update everything about spawning the traveling merchant from the methods we have in the Traveling Merchant's class
			ExampleTravelingMerchant.UpdateTravelingMerchant();
		}

		public override void PostUpdate()
		{
			if (Main.dayTime && VolcanoCountdown == 0)
			{
				if (VolcanoCooldown > 0)
				{
					VolcanoCooldown--;
				}
				if (VolcanoCooldown <= 0 && Main.rand.NextBool(VolcanoChance) && !ModContent.GetInstance<ExampleConfigServer>().DisableVolcanoes)
				{
					string key = "Mods.ExampleMod.VolcanoWarning";
					Color messageColor = Color.Orange;
					if (Main.netMode == NetmodeID.Server) // Server
					{
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
					}
					else if (Main.netMode == NetmodeID.SinglePlayer) // Single Player
					{
						Main.NewText(Language.GetTextValue(key), messageColor);
					}
					VolcanoCountdown = DefaultVolcanoCountdown;
					VolcanoCooldown = DefaultVolcanoCooldown;
				}
			}
			if (VolcanoCountdown > 0)
			{
				VolcanoCountdown--;
				if (VolcanoCountdown == 0)
				{
					VolcanoTremorTime = DefaultVolcanoTremorTime;
					// Since PostUpdate only happens in single and server, we need to inform the clients to shake if this is a server
					if (Main.netMode == NetmodeID.Server)
					{
						var netMessage = mod.GetPacket();
						netMessage.Write((byte)ExampleModMessageType.SetTremorTime);
						netMessage.Write(VolcanoTremorTime);
						netMessage.Send();
					}
					for (int playerIndex = 0; playerIndex < 255; playerIndex++)
					{
						if (Main.player[playerIndex].active)
						{
							Player player = Main.player[playerIndex];
							int speed = 12;
							float spawnX = Main.rand.Next(1000) - 500 + player.Center.X;
							float spawnY = -1000 + player.Center.Y;
							var baseSpawn = new Vector2(spawnX, spawnY);
							Vector2 baseVelocity = player.Center - baseSpawn;
							baseVelocity.Normalize();
							baseVelocity = baseVelocity * speed;
							var identities = new List<int>();
							for (int i = 0; i < VolcanoProjectiles; i++)
							{
								Vector2 spawn = baseSpawn;
								spawn.X = spawn.X + i * 30 - VolcanoProjectiles * 15;
								Vector2 velocity = baseVelocity;
								velocity = baseVelocity.RotatedBy(MathHelper.ToRadians(-VolcanoAngleSpread / 2 + VolcanoAngleSpread * i / (float)VolcanoProjectiles));
								velocity.X = velocity.X + 3 * Main.rand.NextFloat() - 1.5f;
								int projectile = Projectile.NewProjectile(spawn.X, spawn.Y, velocity.X, velocity.Y, Main.rand.Next(ProjectileID.MolotovFire, ProjectileID.MolotovFire3 + 1), 10, 10f, Main.myPlayer, 0f, 0f);
								Main.projectile[projectile].hostile = true;
								Main.projectile[projectile].Name = "Volcanic Rubble";
								identities.Add(Main.projectile[projectile].identity);
							}
							if (Main.netMode == NetmodeID.Server)
							{
								var netMessage = mod.GetPacket();
								netMessage.Write((byte)ExampleModMessageType.VolcanicRubbleMultiplayerFix);
								netMessage.Write(identities.Count);
								for (int i = 0; i < identities.Count; i++)
								{
									netMessage.Write(identities[i]);
								}
								netMessage.Send();
							}
						}
					}
				}
			}
		}
		**/


		/**
		// In ExampleMod, we use PostDrawTiles to draw the TEScoreBoard area. PostDrawTiles draws before players, npc, and projectiles, so it works well.
		public override void PostDrawTiles()
		{
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
			var screenRect = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);
			screenRect.Inflate(TEScoreBoard.drawBorderWidth, TEScoreBoard.drawBorderWidth);
			int scoreBoardType = ModContent.TileEntityType<TEScoreBoard>();
			foreach (var item in TileEntity.ByID)
			{
				if (item.Value.type == scoreBoardType)
				{
					var scoreBoard = item.Value as TEScoreBoard;
					Rectangle scoreBoardArea = scoreBoard.GetPlayArea();
					// We only want to draw while the area is visible. 
					if (screenRect.Intersects(scoreBoardArea))
					{
						scoreBoardArea.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);
						DrawBorderedRect(Main.spriteBatch, Color.LightBlue * 0.1f, Color.Blue * 0.3f, scoreBoardArea.TopLeft(), scoreBoardArea.Size(), TEScoreBoard.drawBorderWidth);
					}
				}
			}
			Main.spriteBatch.End();
		}

		// A helper method that draws a bordered rectangle. 
		public static void DrawBorderedRect(SpriteBatch spriteBatch, Color color, Color borderColor, Vector2 position, Vector2 size, int borderWidth)
		{
			spriteBatch.Draw(Main.magicPixel, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y), color);
			spriteBatch.Draw(Main.magicPixel, new Rectangle((int)position.X - borderWidth, (int)position.Y - borderWidth, (int)size.X + borderWidth * 2, borderWidth), borderColor);
			spriteBatch.Draw(Main.magicPixel, new Rectangle((int)position.X - borderWidth, (int)position.Y + (int)size.Y, (int)size.X + borderWidth * 2, borderWidth), borderColor);
			spriteBatch.Draw(Main.magicPixel, new Rectangle((int)position.X - borderWidth, (int)position.Y, (int)borderWidth, (int)size.Y), borderColor);
			spriteBatch.Draw(Main.magicPixel, new Rectangle((int)position.X + (int)size.X, (int)position.Y, (int)borderWidth, (int)size.Y), borderColor);
		}
		**/
	}
	
}
