using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BasicMod.NPCs.SwatInvasion
{
	public class SwatSniper : ModNPC
	{
		//public override string Texture => "Terraria/NPC_" + NPCID.SkeletonArcher;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("SwatSniper");
			Main.npcFrameCount[npc.type] = Main.npcFrameCount[NPCID.SkeletonSniper];
		}

		public override void SetDefaults()
		{

			npc.width = 18;
			npc.height = 40;
			npc.damage = 40;
			npc.defense = 20;
			npc.lifeMax = 300;
			npc.HitSound = SoundID.NPCHit2;
			npc.DeathSound = SoundID.NPCDeath2;
			npc.value = 60f;
			npc.knockBackResist = 0.5f;
			npc.aiStyle = 3;
			aiType = NPCID.SkeletonSniper;
			animationType = NPCID.SkeletonSniper;
			banner = Item.NPCtoBanner(NPCID.SkeletonSniper);
			bannerItem = Item.BannerToItem(banner);
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
				return 11f;
			}
			else
			{
				return 0f;
			}

		}



		public override void NPCLoot()
		{
			BasicWorld.SwatKillCount += 2;
			if (Main.netMode == NetmodeID.Server)
				NetMessage.SendData(MessageID.WorldData); // Immediately inform clients of new world state.

		}

		public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			//target.AddBuff(mod.BuffType("Zoomed"), 120);
		}

		bool thrownGrenade = false;
        public override void PostAI()
        {
			if (npc.velocity.X == 0f && Main.netMode != 1 && !thrownGrenade)
            {
				int proj = Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 8.00f * npc.direction, -8.00f, ProjectileID.SmokeBomb, npc.damage, 3f, Main.myPlayer);
				Main.projectile[proj].timeLeft = 90;
				thrownGrenade = true;
			}
		}
    }
}
