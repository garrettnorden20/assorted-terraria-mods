using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BasicMod.NPCs
{
	public class ZombieArcher : ModNPC
	{
		//public override string Texture => "Terraria/NPC_" + NPCID.SkeletonArcher;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Zombie Archer");
			Main.npcFrameCount[npc.type] = Main.npcFrameCount[NPCID.SkeletonArcher];
		}

		public override void SetDefaults()
		{
			
			npc.width = 18;
			npc.height = 40;
			npc.damage = 30;
			npc.defense = 8;
			npc.lifeMax = 115;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath2;
			npc.value = 60f;
			npc.knockBackResist = 0.3f;
			npc.aiStyle = 3;
			aiType = NPCID.SkeletonArcher;
			animationType = NPCID.SkeletonArcher;
			banner = Item.NPCtoBanner(NPCID.SkeletonArcher);
			bannerItem = Item.BannerToItem(banner);
			//npc.type = 110;
		}

        public override void HitEffect(int hitDirection, double damage)
        {
			if (npc.life > 0)
			{
				for (int num432 = 0; (double)num432 < damage / (double)npc.lifeMax * 100.0; num432++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f);
				}
				if (npc.type == 186 && Main.rand.Next(5) == 0)
				{
					Gore.NewGore(npc.position, npc.velocity, 242);
				}
				if (npc.type == 187)
				{
					for (int num433 = 0; (double)num433 < damage / (double)npc.lifeMax * 200.0; num433++)
					{
						Dust.NewDust(npc.position, npc.width, 24, 4, hitDirection, -1f, 125, new Color(0, 80, 255, 100));
					}
				}
				return;
			}
			Gore.NewGore(new Vector2(npc.position.X, npc.position.Y + 20f), npc.velocity, 4);
			Gore.NewGore(new Vector2(npc.position.X, npc.position.Y + 20f), npc.velocity, 4);
			Gore.NewGore(new Vector2(npc.position.X, npc.position.Y + 34f), npc.velocity, 5);
			Gore.NewGore(new Vector2(npc.position.X, npc.position.Y + 34f), npc.velocity, 5);

		}

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (Main.hardMode)
			{
				return SpawnCondition.OverworldNight.Chance * 0.08f;
			}
			return 0f;
		}

		public override void NPCLoot()
		{
			/**
			BasicWorld.SwatKillCount += 5;
			if (Main.netMode == NetmodeID.Server)
				NetMessage.SendData(MessageID.WorldData); // Immediately inform clients of new world state.
			**/
			
		}

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
			//target.AddBuff(mod.BuffType("Zoomed"), 120);
        }
    }
}
