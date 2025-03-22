using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.World.Generation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using BasicMod.Tiles;

namespace Terraria.GameContent.Biomes
{
	public class BleckBiome : MicroBiome
	{
		private struct Hub
		{
			public Vector2 Position;

			public Hub(Vector2 position)
			{
				this.Position = position;
			}

			public Hub(float x, float y)
			{
				this.Position = new Vector2(x, y);
			}
		}

		private class Cluster : List<Hub>
		{
		}

		private class ClusterGroup : List<Cluster>
		{
			public int Width;

			public int Height;

			private void SearchForCluster(bool[,] hubMap, List<Point> pointCluster, int x, int y, int level = 2)
			{
				pointCluster.Add(new Point(x, y));
				hubMap[x, y] = false;
				level--;
				if (level != -1)
				{
					if (x > 0 && hubMap[x - 1, y])
					{
						this.SearchForCluster(hubMap, pointCluster, x - 1, y, level);
					}
					if (x < hubMap.GetLength(0) - 1 && hubMap[x + 1, y])
					{
						this.SearchForCluster(hubMap, pointCluster, x + 1, y, level);
					}
					if (y > 0 && hubMap[x, y - 1])
					{
						this.SearchForCluster(hubMap, pointCluster, x, y - 1, level);
					}
					if (y < hubMap.GetLength(1) - 1 && hubMap[x, y + 1])
					{
						this.SearchForCluster(hubMap, pointCluster, x, y + 1, level);
					}
				}
			}

			private void AttemptClaim(int x, int y, int[,] clusterIndexMap, List<List<Point>> pointClusters, int index)
			{
				int num = clusterIndexMap[x, y];
				if (num == -1 || num == index)
				{
					return;
				}
				int num2 = ((WorldGen.genRand.Next(2) == 0) ? (-1) : index);
				foreach (Point item in pointClusters[num])
				{
					clusterIndexMap[item.X, item.Y] = num2;
				}
			}

			public void Generate(int width, int height)
			{
				this.Width = width;
				this.Height = height;
				base.Clear();
				bool[,] array = new bool[width, height];
				int num = (width >> 1) - 1;
				int num2 = (height >> 1) - 1;
				int num3 = (num + 1) * (num + 1);
				Point point = new Point(num, num2);
				for (int i = point.Y - num2; i <= point.Y + num2; i++)
				{
					float num4 = (float)num / (float)num2 * (float)(i - point.Y);
					int num5 = Math.Min(num, (int)Math.Sqrt((float)num3 - num4 * num4));
					for (int j = point.X - num5; j <= point.X + num5; j++)
					{
						array[j, i] = WorldGen.genRand.Next(2) == 0;
					}
				}
				List<List<Point>> list = new List<List<Point>>();
				for (int k = 0; k < array.GetLength(0); k++)
				{
					for (int l = 0; l < array.GetLength(1); l++)
					{
						if (array[k, l] && WorldGen.genRand.Next(2) == 0)
						{
							List<Point> list2 = new List<Point>();
							this.SearchForCluster(array, list2, k, l);
							if (list2.Count > 2)
							{
								list.Add(list2);
							}
						}
					}
				}
				int[,] array2 = new int[array.GetLength(0), array.GetLength(1)];
				for (int m = 0; m < array2.GetLength(0); m++)
				{
					for (int n = 0; n < array2.GetLength(1); n++)
					{
						array2[m, n] = -1;
					}
				}
				for (int num6 = 0; num6 < list.Count; num6++)
				{
					foreach (Point item in list[num6])
					{
						array2[item.X, item.Y] = num6;
					}
				}
				for (int num7 = 0; num7 < list.Count; num7++)
				{
					foreach (Point item2 in list[num7])
					{
						int x = item2.X;
						int y = item2.Y;
						if (array2[x, y] == -1)
						{
							break;
						}
						int index = array2[x, y];
						if (x > 0)
						{
							this.AttemptClaim(x - 1, y, array2, list, index);
						}
						if (x < array2.GetLength(0) - 1)
						{
							this.AttemptClaim(x + 1, y, array2, list, index);
						}
						if (y > 0)
						{
							this.AttemptClaim(x, y - 1, array2, list, index);
						}
						if (y < array2.GetLength(1) - 1)
						{
							this.AttemptClaim(x, y + 1, array2, list, index);
						}
					}
				}
				foreach (List<Point> item3 in list)
				{
					item3.Clear();
				}
				for (int num8 = 0; num8 < array2.GetLength(0); num8++)
				{
					for (int num9 = 0; num9 < array2.GetLength(1); num9++)
					{
						if (array2[num8, num9] != -1)
						{
							list[array2[num8, num9]].Add(new Point(num8, num9));
						}
					}
				}
				foreach (List<Point> item4 in list)
				{
					if (item4.Count < 4)
					{
						item4.Clear();
					}
				}
				foreach (List<Point> item5 in list)
				{
					Cluster cluster = new Cluster();
					if (item5.Count <= 0)
					{
						continue;
					}
					foreach (Point item6 in item5)
					{
						cluster.Add(new Hub((float)item6.X + (WorldGen.genRand.NextFloat() - 0.5f) * 0.5f, (float)item6.Y + (WorldGen.genRand.NextFloat() - 0.5f) * 0.5f));
					}
					base.Add(cluster);
				}
			}
		}

		private void PlaceSand(ClusterGroup clusters, Point start, Vector2 scale)
		{
			int num = (int)(scale.X * (float)clusters.Width);
			int num2 = (int)(scale.Y * (float)clusters.Height);
			int num3 = 5;
			int num4 = start.Y + (num2 >> 1);
			float num5 = 0f;
			short[] array = new short[num + num3 * 2];
			for (int i = -num3; i < num + num3; i++)
			{
				for (int j = 150; j < num4; j++)
				{
					if (WorldGen.SolidOrSlopedTile(i + start.X, j))
					{
						num5 += (float)(j - 1);
						array[i + num3] = (short)(j - 1);
						break;
					}
				}
			}
			float num6 = num5 / (float)(num + num3 * 2);
			int num7 = 0;
			for (int k = -num3; k < num + num3; k++)
			{
				float value = Math.Abs((float)(k + num3) / (float)(num + num3 * 2)) * 2f - 1f;
				value = MathHelper.Clamp(value, -1f, 1f);
				if (k % 3 == 0)
				{
					num7 = Utils.Clamp(num7 + GenBase._random.Next(-1, 2), -10, 10);
				}
				float num8 = (float)Math.Sqrt(1f - value * value * value * value);
				int num9 = num4 - (int)(num8 * ((float)num4 - num6)) + num7;
				int val = num4 - (int)(((float)num4 - num6) * (num8 - 0.15f / (float)Math.Sqrt(Math.Max(0.01, (double)Math.Abs(8f * value) - 0.1)) + 0.25f));
				val = Math.Min(num4, val);
				if (Math.Abs(value) < 0.8f)
				{
					float num10 = Utils.SmoothStep(0.5f, 0.8f, Math.Abs(value));
					num10 = num10 * num10 * num10;
					int val2 = 10 + (int)(num6 - num10 * 20f) + num7;
					val2 = Math.Min(val2, num9);
					int num11 = 50;
					for (int l = num11; (float)l < num6; l++)
					{
						int num12 = k + start.X;
						if (GenBase._tiles[num12, l].active() && (GenBase._tiles[num12, l].type == 189 || GenBase._tiles[num12, l].type == 196)) // cloud, raincloud
						{
							num11 = l + 5;
						}
					}
					for (int m = num11; m < val2; m++)
					{
						int num13 = k + start.X;
						int num14 = m;
						GenBase._tiles[num13, num14].active(active: false);
						GenBase._tiles[num13, num14].wall = 0;
					}
					array[k + num3] = (short)val2;
				}
				/** this code relates it to the surface, so dont use it if you want it completely 
				for (int num15 = num4 - 1; num15 >= num9; num15--)
				{
					int num16 = k + start.X;
					int num17 = num15;
					Tile tile = GenBase._tiles[num16, num17];
					tile.liquid = 0;
					Tile testTile = GenBase._tiles[num16, num17 + 1];
					Tile testTile2 = GenBase._tiles[num16, num17 + 2];
					tile.type = (ushort)((WorldGen.SolidTile(testTile) && WorldGen.SolidTile(testTile2)) ? (ushort)ModContent.TileType<BleckTile>() : (ushort)ModContent.TileType<BleckTile>()); // used to be sand, hardened sand
					if (num15 > num9 + 5)
					{
						tile.wall = 0 // tile.wall = 187;
					}
					tile.active(active: true);
					if (tile.wall != 187)
					{
						tile.wall = 0 // tile.wall = 0;
					}
					if (num15 < val)
					{
						if (num15 > num9 + 5)
						{
							tile.wall = 0 // tile.wall = 187;
						}
						tile.active(active: false);
					}
					WorldGen.SquareWallFrame(num16, num17);
				}
				**/
			}
		}

		private void PlaceClusters(ClusterGroup clusters, Point start, Vector2 scale)
		{
			int num = (int)(scale.X * (float)clusters.Width);
			int num2 = (int)(scale.Y * (float)clusters.Height);
			Vector2 value = new Vector2(num, num2);
			Vector2 value2 = new Vector2(clusters.Width, clusters.Height);
			for (int i = -20; i < num + 20; i++)
			{
				for (int j = -20; j < num2 + 20; j++)
				{
					float num3 = 0f;
					int num4 = -1;
					float num5 = 0f;
					int num6 = i + start.X;
					int num7 = j + start.Y;
					Vector2 value3 = new Vector2(i, j) / value * value2;
					float num8 = (new Vector2(i, j) / value * 2f - Vector2.One).Length();
					for (int k = 0; k < clusters.Count; k++)
					{
						Cluster cluster = clusters[k];
						if (Math.Abs(cluster[0].Position.X - value3.X) > 10f || Math.Abs(cluster[0].Position.Y - value3.Y) > 10f)
						{
							continue;
						}
						float num9 = 0f;
						foreach (Hub item in cluster)
						{
							num9 += 1f / Vector2.DistanceSquared(item.Position, value3);
						}
						if (num9 > num3)
						{
							if (num3 > num5)
							{
								num5 = num3;
							}
							num3 = num9;
							num4 = k;
						}
						else if (num9 > num5)
						{
							num5 = num9;
						}
					}
					float num10 = num3 + num5;
					Tile tile = GenBase._tiles[num6, num7];
					bool flag = num8 >= 0.8f;
					if (num10 > 3.5f)
					{
						tile.ClearEverything();
						tile.wall = 0; // tile.wall = 187;
						tile.liquid = 0;
						if (num4 % 15 == 2)
						{
							tile.ResetToType((ushort)ModContent.TileType<BleckTile>()); // tile.ResetToType(404);
							tile.wall = 0; // tile.wall = 187;
							tile.active(active: true);
						}
						Tile.SmoothSlope(num6, num7);
					}
					else if (num10 > 1.8f)
					{
						tile.wall = 0; // tile.wall = 187;
						if (!flag || tile.active())
						{
							tile.ResetToType((ushort)ModContent.TileType<BleckTile>()); // tile.ResetToType(396);
							tile.wall = 0; // tile.wall = 187;
							tile.active(active: true);
							Tile.SmoothSlope(num6, num7);
						}
						tile.liquid = 0;
					}
					else if (num10 > 0.7f || !flag)
					{
						if (!flag || tile.active())
						{
							tile.ResetToType((ushort)ModContent.TileType<BleckTile>()); // tile.ResetToType(397);
							tile.active(active: true);
							Tile.SmoothSlope(num6, num7);
						}
						tile.liquid = 0;
						tile.wall = 0; // tile.wall = 216;
					}
					else
					{
						if (!(num10 > 0.25f))
						{
							continue;
						}
						float num11 = (num10 - 0.25f) / 0.45f;
						if (GenBase._random.NextFloat() < num11)
						{
							if (tile.active())
							{
								tile.ResetToType((ushort)ModContent.TileType<BleckTile>()); // tile.ResetToType(397);
								tile.active(active: true);
								Tile.SmoothSlope(num6, num7);
								tile.wall = 0; // tile.wall = 216;
							}
							tile.liquid = 0;
							tile.wall = 0; // tile.wall = 187;
						}
					}
				}
			}
		}

		private void AddTileVariance(ClusterGroup clusters, Point start, Vector2 scale)
		{
			int num = (int)(scale.X * (float)clusters.Width);
			int num2 = (int)(scale.Y * (float)clusters.Height);
			for (int i = -20; i < num + 20; i++)
			{
				for (int j = -20; j < num2 + 20; j++)
				{
					int num3 = i + start.X;
					int num4 = j + start.Y;
					Tile tile = GenBase._tiles[num3, num4];
					Tile testTile = GenBase._tiles[num3, num4 + 1];
					Tile testTile2 = GenBase._tiles[num3, num4 + 2];
					if (tile.type == 53 && (!WorldGen.SolidTile(testTile) || !WorldGen.SolidTile(testTile2)))
					{
						tile.type = 397;
					}
				}
			}
			for (int k = -20; k < num + 20; k++)
			{
				for (int l = -20; l < num2 + 20; l++)
				{
					int num5 = k + start.X;
					int num6 = l + start.Y;
					Tile tile2 = GenBase._tiles[num5, num6];
					if (!tile2.active() || tile2.type != 396)
					{
						continue;
					}
					bool flag = true;
					for (int num7 = -1; num7 >= -3; num7--)
					{
						if (GenBase._tiles[num5, num6 + num7].active())
						{
							flag = false;
							break;
						}
					}
					bool flag2 = true;
					for (int m = 1; m <= 3; m++)
					{
						if (GenBase._tiles[num5, num6 + m].active())
						{
							flag2 = false;
							break;
						}
					}
					if ((flag ^ flag2) && GenBase._random.Next(5) == 0)
					{
						WorldGen.PlaceTile(num5, num6 + ((!flag) ? 1 : (-1)), 165, mute: true, forced: true);
					}
					else if (flag && GenBase._random.Next(5) == 0)
					{
						WorldGen.PlaceTile(num5, num6 - 1, 187, mute: true, forced: true, -1, 29 + GenBase._random.Next(6));
					}
				}
			}
		}

		private bool FindStart(Point origin, Vector2 scale, int xHubCount, int yHubCount, out Point start)
		{
			start = new Point(0, 0);
			int width = (int)(scale.X * (float)xHubCount);
			int height = (int)(scale.Y * (float)yHubCount);
			origin.X -= width >> 1;
			int num2 = 220;
			for (int i = -20; i < width + 20; i++)
			{
				for (int j = (int)(Main.rockLayer * 1.1); j < Main.maxTilesY; j++) // avoids flying islands, goes down to find tile
				{
					if (!WorldGen.SolidTile(i + origin.X, j))
					{
						continue;
					}
					ushort type = GenBase._tiles[i + origin.X, j].type;
					if (type == 59 || type == 60) // mud/jungle tiles
					{
						return false; // don't override jungle!
					}
					if (j > num2)
					{
						num2 = j;
					}
					break;
				}
				if (WorldGen.UndergroundDesertLocation.Intersects(new Rectangle(origin.X, num2, width, height)))
				{
					return false;
				}
			}
			//WorldGen.UndergroundDesertLocation = new Rectangle(origin.X, num2, width, height);
			start = new Point(origin.X, num2);
			return true;
		}

		public override bool Place(Point origin, StructureMap structures)
		{
			float num = (float)Main.maxTilesX / 4200f;
			int num2 = (int)(80f * num);
			int num3 = (int)((GenBase._random.NextFloat() + 1f) * 80f * num);
			Vector2 scale = new Vector2(4f, 2f);
			if (!this.FindStart(origin, scale, num2, num3, out var start))
			{
				return false;
			}
			ClusterGroup clusterGroup = new ClusterGroup();
			clusterGroup.Generate(num2, num3);
			this.PlaceSand(clusterGroup, start, scale);
			this.PlaceClusters(clusterGroup, start, scale);
			this.AddTileVariance(clusterGroup, start, scale);
			int num4 = (int)(scale.X * (float)clusterGroup.Width);
			int num5 = (int)(scale.Y * (float)clusterGroup.Height);
			for (int i = -20; i < num4 + 20; i++)
			{
				for (int j = -20; j < num5 + 20; j++)
				{
					if (i + start.X > 0 && i + start.X < Main.maxTilesX - 1 && j + start.Y > 0 && j + start.Y < Main.maxTilesY - 1)
					{
						WorldGen.SquareWallFrame(i + start.X, j + start.Y);
						WorldUtils.TileFrame(i + start.X, j + start.Y, frameNeighbors: true);
					}
				}
			}
			GenPots();
			return true;
		}


		private void GenPots()
		{
			Main.tileSolid[137] = true;
			Main.tileSolid[130] = true;
			for (int num262 = 0; num262 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0008); num262++)
			{
				float num263 = (float)((double)num262 / ((double)(Main.maxTilesX * Main.maxTilesY) * 0.0008)); // ratio of how complete we are
				bool flag18 = false;
				int num264 = 0;
				while (!flag18)
				{
					int num265 = WorldGen.genRand.Next((int)WorldGen.worldSurfaceHigh, Main.maxTilesY - 10);
					if ((double)num263 > 0.93)
					{
						num265 = Main.maxTilesY - 150;
					}
					else if ((double)num263 > 0.75)
					{
						num265 = (int)WorldGen.worldSurfaceLow;
					}
					
					int num266 = WorldGen.genRand.Next(1, Main.maxTilesX);
					bool flag19 = false;
					for (int num267 = num265; num267 < Main.maxTilesY; num267++)
					{
						if (!flag19)
						{
							if (Main.tile[num266, num267].active() && Main.tileSolid[Main.tile[num266, num267].type] && !Main.tile[num266, num267 - 1].lava())
							{
								flag19 = true;
							}
						}
						else
						{
							int style = WorldGen.genRand.Next(0, 4);
							int num268 = 0;
							if (num267 < Main.maxTilesY - 5)
							{
								num268 = Main.tile[num266, num267 + 1].type;
							}
							
							if (PlaceBleckPot(num266, num267, (ushort)ModContent.TileType<BleckPot>()))
							{
								flag18 = true;
								break;
							}
							num264++;
							if (num264 >= 10000)
							{
								flag18 = true;
								break;
							}
						}
					}
				}
			}
		}

		private bool PlaceBleckPot(int x, int y, ushort type)
        {
			bool flag = true;
			for (int i = x; i < x + 2; i++)
			{
				for (int j = y - 1; j < y + 1; j++)
				{
					if (Main.tile[i, j] == null)
					{
						Main.tile[i, j] = new Tile();
					}
					if (Main.tile[i, j].active())
					{
						flag = false;
					}
				}
				if (Main.tile[i, y + 1] == null)
				{
					Main.tile[i, y + 1] = new Tile();
				}
				if (!Main.tile[i, y + 1].nactive() || Main.tile[i, y + 1].halfBrick() || Main.tile[i, y + 1].slope() != 0 || !Main.tileSolid[Main.tile[i, y + 1].type])
				{
					flag = false;
				}
			}
			if (flag)
			{
				//int num = WorldGen.genRand.Next(3) * 36;
				for (int k = 0; k < 2; k++)
				{
					for (int l = -1; l < 1; l++)
					{
						int num2 = k * 18;
						int num3 = (l + 1) * 18;
						Main.tile[x + k, y + l].active(active: true);
						Main.tile[x + k, y + l].frameX = (short)num2;
						Main.tile[x + k, y + l].frameY = (short)num3;
						Main.tile[x + k, y + l].type = type;
						Main.tile[x + k, y + l].halfBrick(halfBrick: false);
					}
				}
				return true;
			}
			return false;
		}
	}
}
