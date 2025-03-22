using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BasicMod.NPCs.SwatInvasion
{
	public class SwatDoorRam : ModNPC
	{
		//public override string Texture => "Terraria/NPC_" + NPCID.SkeletonArcher;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("SWAT Rammer");
			Main.npcFrameCount[npc.type] = Main.npcFrameCount[NPCID.GoblinPeon];
		}

		public override void SetDefaults()
		{

			npc.width = 18;
			npc.height = 40;
			npc.damage = 20;
			npc.defense = 2;
			npc.lifeMax = 115;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath2;
			npc.value = 60f;
			npc.knockBackResist = 0.5f;
			npc.aiStyle = 3;
			aiType = NPCID.ArmoredSkeleton;
			animationType = NPCID.GoblinPeon;
			//banner = Item.NPCtoBanner(NPCID.SkeletonArcher);
			//bannerItem = Item.BannerToItem(banner);
			//npc.type = 110;
		}

		/**
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			return SpawnCondition.OverworldNightMonster.Chance * 0.6f;
		}
		**/

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (BasicWorld.SwatEvent)
			{
				return 35f;
			}
			else
			{
				return 0f;
			}
		}

		public override void NPCLoot()
		{

			BasicWorld.SwatKillCount += 1;
			if (Main.netMode == NetmodeID.Server)
				NetMessage.SendData(MessageID.WorldData); // Immediately inform clients of new world state.
		}

        public override void PostAI()
        {
			int num178 = (int)((npc.position.X + (float)(npc.width / 2) + (float)(15 * npc.direction)) / 16f);
			int num179 = (int)((npc.position.Y + (float)npc.height - 15f) / 16f);
			if (Main.tile[num178, num179] == null)
			{
				Main.tile[num178, num179] = new Tile();
			}
			if (Main.tile[num178, num179 - 1] == null)
			{
				Main.tile[num178, num179 - 1] = new Tile();
			}
			if (Main.tile[num178, num179 - 2] == null)
			{
				Main.tile[num178, num179 - 2] = new Tile();
			}
			if (Main.tile[num178, num179 - 3] == null)
			{
				Main.tile[num178, num179 - 3] = new Tile();
			}
			if (Main.tile[num178, num179 + 1] == null)
			{
				Main.tile[num178, num179 + 1] = new Tile();
			}
			if (Main.tile[num178 + npc.direction, num179 - 1] == null)
			{
				Main.tile[num178 + npc.direction, num179 - 1] = new Tile();
			}
			if (Main.tile[num178 + npc.direction, num179 + 1] == null)
			{
				Main.tile[num178 + npc.direction, num179 + 1] = new Tile();
			}
			if (Main.tile[num178 - npc.direction, num179 + 1] == null)
			{
				Main.tile[num178 - npc.direction, num179 + 1] = new Tile();
			}
			Main.tile[num178, num179 + 1].halfBrick();
			if (Main.tile[num178, num179 - 1].nactive() && (TileLoader.IsClosedDoor(Main.tile[num178, num179 - 1]) || Main.tile[num178, num179 - 1].type == 388))
			{
				npc.ai[2] += 1f;
				npc.ai[3] = 0f;
				if (npc.ai[2] >= 60f)
				{
					if (!Main.bloodMoon && (npc.type == 3 || npc.type == 331 || npc.type == 332 || npc.type == 132 || npc.type == 161 || npc.type == 186 || npc.type == 187 || npc.type == 188 || npc.type == 189 || npc.type == 200 || npc.type == 223 || npc.type == 320 || npc.type == 321 || npc.type == 319))
					{
						npc.ai[1] = 0f;
					}
					npc.velocity.X = 0.5f * (float)(-npc.direction);
					int num180 = 5;
					if (Main.tile[num178, num179 - 1].type == 388)
					{
						num180 = 2;
					}
					npc.ai[1] += num180;
					npc.ai[2] = 0f;
					bool flag23 = false;
					if (npc.ai[1] >= 10f)
					{
						flag23 = true;
						npc.ai[1] = 10f;
					}
					WorldGen.KillTile(num178, num179 - 1, fail: true);
					if ((Main.netMode != 1 || !flag23) && flag23 && Main.netMode != 1)
					{
						WorldGen.KillTile(num178, num179 - 1);
						if (Main.netMode == 2)
						{
							NetMessage.SendData(17, -1, -1, null, 0, num178, num179 - 1);
						}
					}
				}
			}
		}
    }
}
