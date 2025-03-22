using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace BasicMod.NPCs
{
	// This ModNPC serves as an example of a complete AI example.
	public class TeslaBoss : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Tesla Tower"); 
			Main.npcFrameCount[npc.type] = 12; // make sure to set this for your modnpcs.
		}

		public override void SetDefaults()
		{
			npc.noTileCollide = true;
			npc.noGravity = true;
			npc.boss = true;
			npc.width = 120;
			npc.height = 180;
			npc.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
			npc.damage = 40;
			npc.defense = 15;
			npc.lifeMax = 40000; // 50,000
			npc.HitSound = SoundID.NPCHit4;
			npc.DeathSound = SoundID.NPCDeath1;
			npc.value = 25f;
			npc.netAlways = true; // makes it always update, even off screen?
			npc.knockBackResist = 0; // no knockback on it
			npc.buffImmune[BuffID.Poisoned] = true;
			npc.buffImmune[BuffID.Confused] = false; // npc default to being immune to the Confused debuff. Allowing confused could be a little more work depending on the AI. npc.confused is true while the npc is confused.
			npc.lavaImmune = true;
			npc.npcSlots = 30; // sort of disables other spawns
			music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/TeslaCoil");
			musicPriority = MusicPriority.BossMedium;


			// important future fields
			// dontTakeDamage, takenDamageMultiplier

		}



		/**
		public override void OnSpawn() //optional
		{

		}
		**/
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			// boss so shouldn't come naturally
			//return SpawnCondition.OverworldDaySlime.Chance * 0.1f;
			return 0.0f;
		}

        public override void NPCLoot()
        {
			if (!BasicWorld.downedTesla)
			{
				BasicWorld.downedTesla = true;
				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData); // Immediately inform clients of new world state
				}
			}
		}

        private const int MaxTime_ChooseWaiting = 60;
		private const int MaxTime_InitialSleep = 120;
		private const int MaxTime_Phase2 = 600;
		
		private int MaxTime_Attack_1 = 300;
		private int MaxTime_Attack_2 = 300;
		private int MaxTime_Attack_3 = 300;
		private int MaxTime_Attack_4 = 500;
		private int MaxTime_Attack_5 = 540;

		bool chooseNext = false;
		Player currentTarget = null;

		int phase = 0;
		float phaseTwoThreshold = 0.6f;

		int primaryAttackTimeInterval;
		int secondaryAttackTimeInterval;

		// These const ints are for the benefit of the programmer. Organization is key to making an AI that behaves properly without driving you crazy.
		// Here I lay out what I will use each of the 4 npc.ai slots for.
		private const int AI_State_Slot = 0;
		private const int AI_Timer_Slot = 1;
		//private const int AI_Flutter_Time_Slot = 2;
		private const int AI_Unused_Slot_3 = 3;

		// npc.localAI will also have 4 float variables available to use. With ModNPC, using just a local class member variable would have the same effect.
		private const int Local_AI_Unused_Slot_0 = 0;
		private const int Local_AI_Unused_Slot_1 = 1;
		private const int Local_AI_Unused_Slot_2 = 2;
		private const int Local_AI_Unused_Slot_3 = 3;

		// Here I define some values I will use with the State slot. Using an ai slot as a means to store "state" can simplify things greatly. Think flowchart.
		private const int State_Asleep = 0;
		private const int State_Attack_1 = 1;
		private const int State_Attack_2 = 2;
		private const int State_Attack_3 = 3;
		private const int State_Attack_4 = 4;
		private const int State_Attack_5 = 5; // maybe do this attack when reaches half-health?

		int totalnumAttacks = 5;
		int attacksSoFar = 0;
		List<int> attackOrder = new List<int>();
		bool inPhaseTwoAttack;

		// This is a property (https://msdn.microsoft.com/en-us/library/x9fsa0sw.aspx), it is very useful and helps keep out AI code clear of clutter.
		// Without it, every instance of "AI_State" in the AI code below would be "npc.ai[AI_State_Slot]". 
		// Also note that without the "AI_State_Slot" defined above, this would be "npc.ai[0]".
		// This is all to just make beautiful, manageable, and clean code.
		public float AI_State
		{
			get => npc.ai[AI_State_Slot];
			set => npc.ai[AI_State_Slot] = value;
		}

		public float AI_Timer
		{
			get => npc.ai[AI_Timer_Slot];
			set => npc.ai[AI_Timer_Slot] = value;
		}

		public override void AI()
		{
			CheckForDespawn();
			AddEffects();
			
			#region Phase2
			if (npc.life < npc.lifeMax * phaseTwoThreshold && phase == 0 && chooseNext == true) // special thing done once for reaching phase2
            {
				if (AI_Timer > MaxTime_Phase2) 
				{
					inPhaseTwoAttack = false;
					//Main.NewText("Phase2 Start Finish");
					npc.takenDamageMultiplier = 1f;
					AI_Timer = 0;
					phase = 1; // set to next phase
					chooseNext = true;

				}
				else if (AI_Timer == 0) 
				{
					//Main.NewText("Phase2 Start Start");
					Talk("ERROR: LETHAL FORCE ENGAGING");

					inPhaseTwoAttack = true;
					npc.takenDamageMultiplier = 0.1f;
					
					MaxTime_Attack_1 = 240;
					MaxTime_Attack_2 = 240;
					MaxTime_Attack_3 = 240;
					MaxTime_Attack_4 = 500;
					MaxTime_Attack_5 = 360;

					ResetPhase2();
					npc.TargetClosest(true); // sets the boss to target the closest NPC
					currentTarget = Main.player[npc.target]; // sets the field to the current target
					AI_Timer++;

				}
				else // all other code for the actual Attack can go here
				{
					//Main.NewText("Phase2 Continue");
					AttackPhase2(npc);
					AI_Timer++;
				}
			}
            #endregion

			else if (chooseNext) // when the next move/state needs to be decided randomly
            {	if (AI_Timer > MaxTime_ChooseWaiting) { // take time to choose next attack
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Teleport();
						//AI_State = Main.rand.Next(1,6); // determines which Attack will come next; min inclusive, max exclusive // TODO: TESTING TESTING ERROR FIX
						orderAttacks();
						AI_State = attackOrder[attacksSoFar % totalnumAttacks];
						attacksSoFar++;
						npc.netUpdate = true;
					}
					AI_Timer = 0;
					chooseNext = false;
				} else
                {
					AI_Timer++; // continue to wait
				}
				
			}
			// The npc starts in the asleep state, until time to awaken
			else if (AI_State == State_Asleep)
			{
				if (AI_Timer > MaxTime_InitialSleep) // if it has waited enough time, end the sleep state
                {
					Talk("THREAT DETECTED: ENGAGING");
					npc.takenDamageMultiplier = 1f;
					AI_Timer = 0;
					AI_State = State_Attack_1; // sets it to Attack next AI turn
                } else
                {
					npc.takenDamageMultiplier = 0.1f;
					AI_Timer++; // continue to wait

                }
			}
            #region Attack 1
            // In this state, attack_1 is happening
            else if (AI_State == State_Attack_1)
			{
				if (AI_Timer > MaxTime_Attack_1) // the end of Attack 1 state - in next loop, it chooses next
				{
					//Main.NewText("Attack 1 Finish");
					AI_Timer = 0;
					chooseNext = true;
				}
				else if (AI_Timer == 0) // for events at the very beginning of the Attack_1 State
                {
					//Main.NewText("Attack 1 Start");
					ResetAttack1(npc);
					npc.TargetClosest(true); // sets the boss to target the closest NPC
					currentTarget = Main.player[npc.target]; // sets the field to the current target
					AI_Timer++;
					// TODO: draw crosshair?
				}
				else // all other code for the actual Attack can go here
                {
					Attack1(npc);
					AI_Timer++;
				}
				
			}
            #endregion
            
			#region Attack 2
            // in this state, attack_2 is happening
            else if (AI_State == State_Attack_2)
            {
				if (AI_Timer > MaxTime_Attack_2) // the end of Attack 2 state - in next loop, it chooses next
				{
					//Main.NewText("Attack 2 Finish");
					AI_Timer = 0;
					chooseNext = true;
					
				}
				else if (AI_Timer == 0) // for events at the very beginning of the Attack_2 State
				{
					//Main.NewText("Attack 2 Start");
					ResetAttack2();
					npc.TargetClosest(true); // sets the boss to target the closest NPC
					currentTarget = Main.player[npc.target]; // sets the field to the current target
					AI_Timer++;
					// TODO: draw crosshair?
				}
				else // all other code for the actual Attack can go here
				{
					Attack2(npc);
					AI_Timer++;
				}
			}
            #endregion

            #region Attack 3
            else if (AI_State == State_Attack_3)
			{
				if (AI_Timer > MaxTime_Attack_3) // the end of Attack 3 state - in next loop, it chooses next
				{
					//Main.NewText("Attack 3 Finish");
					AI_Timer = 0;
					chooseNext = true;

				}
				else if (AI_Timer == 0) // for events at the very beginning of the Attack_3 State
				{
					//Main.NewText("Attack 3 Start");
					ResetAttack3();
					npc.TargetClosest(true); // sets the boss to target the closest NPC
					currentTarget = Main.player[npc.target]; // sets the field to the current target
					AI_Timer++;

				}
				else // all other code for the actual Attack can go here
				{
					Attack3(npc);
					AI_Timer++;
				}
			}
            #endregion

            #region Attack 4
            else if (AI_State == State_Attack_4)
			{
				if (AI_Timer > MaxTime_Attack_4) // the end of Attack 4 state - in next loop, it chooses next
				{
					//Main.NewText("Attack 4 Finish");
					AI_Timer = 0;
					chooseNext = true;

				}
				else if (AI_Timer == 0) // for events at the very beginning of the Attack_3 State
				{
					//Main.NewText("Attack 4 Start");
					ResetAttack4();
					npc.TargetClosest(true); // sets the boss to target the closest NPC
					currentTarget = Main.player[npc.target]; // sets the field to the current target
					AI_Timer++;

				}
				else // all other code for the actual Attack can go here
				{
					Attack4(npc);
					AI_Timer++;
				}
			}
			#endregion

			#region Attack 5
			else if (AI_State == State_Attack_5)
			{
				if (AI_Timer > MaxTime_Attack_5) // the end of Attack 4 state - in next loop, it chooses next
				{
					//Main.NewText("Attack 5 Finish");
					AI_Timer = 0;
					chooseNext = true;

				}
				else if (AI_Timer == 0) // for events at the very beginning of the Attack_3 State
				{
					//Main.NewText("Attack 5 Start");
					ResetAttack5();
					npc.TargetClosest(true); // sets the boss to target the closest NPC
					currentTarget = Main.player[npc.target]; // sets the field to the current target
					AI_Timer++;

				}
				else // all other code for the actual Attack can go here
				{
					Attack5(npc);
					AI_Timer++;
				}
			}
			#endregion
		}

		private void orderAttacks()
        {
			if (attacksSoFar == 0)
            {
				for (int i = 1; i < totalnumAttacks + 1; i++)
                {
					attackOrder.Add(i);
                }

			}
			if (attacksSoFar % totalnumAttacks == 0)
            {
				Shuffle();
            }
        }


		private Random rng = new Random();
		private void Shuffle()
		{
			int n = attackOrder.Count;
			while (n > 1)
			{
				n--;
				int k = rng.Next(n + 1);
				int value = attackOrder[k];
				attackOrder[k] = attackOrder[n];
				attackOrder[n] = value;
			}
			for (int i = 0; i < attackOrder.Count; i++)
			{
				//Main.NewText(attackOrder[i]);
			}
		}

		private void Attack1(NPC npc)
        {
			if (AI_Timer % 3 == 0 && (Main.netMode != NetmodeID.MultiplayerClient)) { // first condition determines how many bullets will be shot essentially
				Vector2 position = npc.Center;

				if (phase == 1) // if boss is at second phase
                {
					alternateGapDifference(); // begin to alternate the gap difference
				}
				
				deg = (deg + degChange + gapDifference + rand.Next(0, 5)) % 360; // degreeConstant; //The degrees, you can multiply projectile.ai[1] to make it orbit faster, may be choppy depending on the value
				double rad = deg * (Math.PI / 180); //Convert degrees to radians
				double dist = 100; //Distance away from the player

				Vector2 projectile = new Vector2();
				projectile.X = npc.Center.X - (int)(Math.Cos(rad) * dist);
				projectile.Y = npc.Center.Y - (int)(Math.Sin(rad) * dist);

				Vector2 velocity = Vector2.Subtract(projectile, position);
				velocity.Normalize();
				velocity = velocity * trueVelocity;

				float speedX = velocity.X;
				float speedY = velocity.Y;

				Projectile.NewProjectile(position, velocity, mod.ProjectileType("TeslaHellBullet"), (int)primaryAttackDamage, 1);
				// make player owner?

				npc.netUpdate = true;
				
			}


		}

		float primaryAttackDamage;
		float secondaryAttackDamage;

		// Attack1 values
		double deg = 0;
		double degChange = 9;
		int trueVelocity = 4;
		double gapDifference = 0;
		double GAP_DIFFERENCE_MAX = 30;
		double GAP_DIFFERENCE_MIN = -30;
		bool countUp;
		Random rand = new Random();
		
		private void ResetAttack1(NPC npc)
        {
			if ((Main.netMode != NetmodeID.MultiplayerClient))
            {
				deg = rand.Next(361); // random starting angle
				npc.netUpdate = true;
			}
			if (phase == 0)
            {
				trueVelocity = 6;
			} else
            {
				trueVelocity = 7;
			}
			degChange = 15;
			gapDifference = 0;
			countUp = false;

			primaryAttackDamage = npc.damage * 0.5f;
		}

		private void alternateGapDifference()
		{
			if (gapDifference <= GAP_DIFFERENCE_MIN)
			{
				countUp = true;
			}
			else if (gapDifference >= GAP_DIFFERENCE_MAX)
			{
				countUp = false;
			}
			if (countUp)
			{
				gapDifference += 0.5;
			}
			else
			{
				gapDifference -= 0.5;
			}
		}


		private void Attack2(NPC npc)
		{
			if (AI_Timer % 20 == 0 && (Main.netMode != NetmodeID.MultiplayerClient))
			{
				Vector2 position = npc.Center;

				Vector2 playerPos = currentTarget.position - position;

				Vector2 velocity = trueVelocity * Vector2.Normalize(playerPos);  // gets the direction to aim at the player

				int spread = 20; //The angle of random spread.
				float spreadMult = 0.1f; //Multiplier for bullet spread, set it higher and it will make for some outrageous spread.
				for (int i = 0; i < 4; i++)
				{
					//float vX = speedX + (float)Main.rand.Next(-spread, spread + 1) * spreadMult;
					//float vY = speedY + (float)Main.rand.Next(-spread, spread + 1) * spreadMult;

					float vX = velocity.X + (float)Main.rand.Next(-spread, spread + 1) * spreadMult;
					float vY = velocity.Y + (float)Main.rand.Next(-spread, spread + 1) * spreadMult;
					Projectile.NewProjectile(position.X, position.Y, vX, vY, mod.ProjectileType("TeslaBolt"), (int)primaryAttackDamage, 1, Main.myPlayer);
				}

				if (phase == 1)
                {
					Projectile.NewProjectile(position, velocity, mod.ProjectileType("TeslaTriangleHoming"), (int)secondaryAttackDamage, 1, Main.myPlayer);
				}

				npc.netUpdate = true;

			}
		}

		

		private void ResetAttack2()
        {
			deg = 0;
			degChange = 5;
			trueVelocity = 7;

			primaryAttackDamage = npc.damage * 0.5f;
			secondaryAttackDamage = npc.damage * 0.75f;
		}


		int spread;
		private void Attack3(NPC npc)
        {
			if (AI_Timer % 8 == 0 && (Main.netMode != NetmodeID.MultiplayerClient))
			{
				Vector2 position = npc.Center;

				Vector2 playerPos = currentTarget.position - position;

				Vector2 velocity = Vector2.Normalize(playerPos);

				for (int i = 0; i < 1; i++)
				{
					if (phase == 1)
					{
						velocity = Rotate(velocity, 2f * spread) * trueVelocity;
						Projectile.NewProjectile(position, velocity, mod.ProjectileType("TeslaTriangle"), (int)primaryAttackDamage, 1, Main.myPlayer);
						velocity = Vector2.Normalize(playerPos);

						velocity = Rotate(velocity, -2f * spread) * trueVelocity;
						Projectile.NewProjectile(position, velocity, mod.ProjectileType("TeslaTriangle"), (int)primaryAttackDamage, 1, Main.myPlayer);
						velocity = Vector2.Normalize(playerPos);

						velocity = Rotate(velocity, 3f * spread) * trueVelocity;
						Projectile.NewProjectile(position, velocity, mod.ProjectileType("TeslaTriangle"), (int)primaryAttackDamage, 1, Main.myPlayer);
						velocity = Vector2.Normalize(playerPos);

						velocity = Rotate(velocity, -3f * spread) * trueVelocity;
						Projectile.NewProjectile(position, velocity, mod.ProjectileType("TeslaTriangle"), (int)primaryAttackDamage, 1, Main.myPlayer);
						velocity = Vector2.Normalize(playerPos);

						velocity = Rotate(velocity, -4f * spread) * trueVelocity;
						Projectile.NewProjectile(position, velocity, mod.ProjectileType("TeslaTriangle"), (int)primaryAttackDamage, 1, Main.myPlayer);
						velocity = Vector2.Normalize(playerPos);
					}

					velocity = Rotate(velocity, spread) * trueVelocity;
					Projectile.NewProjectile(position, velocity, mod.ProjectileType("TeslaTriangle"), (int)primaryAttackDamage, 1, Main.myPlayer);
					velocity = Vector2.Normalize(playerPos);

					velocity = velocity * trueVelocity;
					Projectile.NewProjectile(position, velocity, mod.ProjectileType("TeslaTriangle"), (int)primaryAttackDamage, 1, Main.myPlayer);
					velocity = Vector2.Normalize(playerPos);

					velocity = Rotate(velocity, -spread) * trueVelocity;
					Projectile.NewProjectile(position, velocity, mod.ProjectileType("TeslaTriangle"), (int)primaryAttackDamage, 1, Main.myPlayer);

					
				}

				npc.netUpdate = true;

			}
		}

		private void ResetAttack3()
		{
			if (phase == 0)
            {
				trueVelocity = 7;
				spread = 35;
			} else
            {
				spread = 45;
				trueVelocity = 7;
			}
			deg = 0;
			degChange = 5;
			
			primaryAttackDamage = npc.damage * 0.5f;

		}


		Vector2 laserPositionLeft = Main.screenPosition;
		Vector2 laserPositionRight = Main.screenPosition + new Vector2(Main.screenWidth, 0);
		int laserIncrement = 12;
		bool isGoingLeft = true;
		int numBullets;

		// attacks with vertical lasers; starts from left to right, then switches 
		private void Attack4(NPC npc)
		{
			if ((Main.netMode != NetmodeID.MultiplayerClient))
			{
				if (AI_Timer == (MaxTime_Attack_4 / 2))
				{ // if attack is halfway done
					isGoingLeft = !isGoingLeft; // flips the bool
				}
				if (AI_Timer % primaryAttackTimeInterval == 0)
				{
					if (isGoingLeft)
					{
						laserPositionLeft = laserPositionLeft + new Vector2(laserIncrement, 0);
						Projectile.NewProjectile(laserPositionLeft, Vector2.Zero, mod.ProjectileType("TeslaLine"), (int)primaryAttackDamage, 10, Main.myPlayer);
					}
					else
					{
						laserPositionRight = laserPositionRight - new Vector2(laserIncrement, 0);
						Projectile.NewProjectile(laserPositionRight, Vector2.Zero, mod.ProjectileType("TeslaLine"), (int)primaryAttackDamage, 10, Main.myPlayer);
					}

				}
				if (AI_Timer % secondaryAttackTimeInterval == 0) // shooting secondary bullets
                {
					Vector2 position = npc.Center;

					Vector2 playerPos = currentTarget.position - position;

					Vector2 velocity = trueVelocity * Vector2.Normalize(playerPos);  // gets the direction to aim at the player

					int spread = 30; //The angle of random spread.
					float spreadMult = 0.1f; //Multiplier for bullet spread, set it higher and it will make for some outrageous spread.
					for (int i = 0; i < numBullets; i++)
					{
						float vX = velocity.X + (float)Main.rand.Next(-spread, spread + 1) * spreadMult;
						float vY = velocity.Y + (float)Main.rand.Next(-spread, spread + 1) * spreadMult;
						Vector2 finVelocity = new Vector2(vX, vY);

						Projectile.NewProjectile(position, Vector2.Normalize(finVelocity) * trueVelocity, mod.ProjectileType("TeslaTriangle"), (int)secondaryAttackDamage, 1, Main.myPlayer);
					}
				}
				npc.netUpdate = true;
			}
		}

		private void ResetAttack4()
		{
			if (phase == 0)
            {
				// individual slices, not beam
				primaryAttackTimeInterval = 20;
				laserIncrement = 120;
				secondaryAttackTimeInterval = 60;
				numBullets = 3;
			} else
            {
				// one large beam
				primaryAttackTimeInterval = 2;
				laserIncrement = 12;
				secondaryAttackTimeInterval = 70;
				numBullets = 5;
			}
			
			laserPositionLeft = Main.screenPosition;
			laserPositionRight = Main.screenPosition + new Vector2(Main.screenWidth, 0);
			trueVelocity = 5;

			primaryAttackDamage = npc.damage * 1.5f;
			secondaryAttackDamage = npc.damage * 0.5f;

			// maybe add isGoingLeft? does it need to be reset

		}

		// teleport spam
		private void Attack5(NPC npc)
		{
			if (AI_Timer % 40 == 0 && (Main.netMode != NetmodeID.MultiplayerClient))
			{
				Teleport();
				Vector2 position = npc.Center;

				Vector2 direction = Vector2.UnitY;

				while (deg <= 360)
				{

					direction = Rotate(direction, (float)deg) * trueVelocity;
					Projectile.NewProjectile(position, direction, mod.ProjectileType("TeslaTriangle"), (int)primaryAttackDamage, 1, Main.myPlayer);

					direction = Vector2.UnitY;
					deg += 60;
				}
				deg = 0;

				npc.netUpdate = true;

			}
		}

		private void ResetAttack5()
		{
			deg = 0;
			degChange = 5;
			trueVelocity = 6;

			primaryAttackDamage = npc.damage * 0.5f;
		}

		public Vector2 Rotate(Vector2 v, float degrees)
		{
			float radians = degrees * (float)(Math.PI / 180);
			float sin = (float)Math.Sin(radians);
			float cos = (float)Math.Cos(radians);

			float tx = v.X;
			float ty = v.Y;
			v.X = (cos * tx) - (sin * ty);
			v.Y = (sin * tx) + (cos * ty);

			return v;
		}



		bool laserLineSwitcher;
		// laser grid attack
		private void AttackPhase2(NPC npc)
        {
			if ((Main.netMode != NetmodeID.MultiplayerClient))
			{
				if (AI_Timer % primaryAttackTimeInterval == 0 && AI_Timer > secondaryAttackTimeInterval) 
				{
					npc.takenDamageMultiplier = 1f;

					laserLineSwitcher = !laserLineSwitcher;
					if (laserLineSwitcher)
					{
						Vector2 laserPositionLeftIncr = laserPositionLeft;
						while (laserPositionLeftIncr.X < laserPositionRight.X)
						{
							laserPositionLeftIncr = laserPositionLeftIncr + new Vector2(laserIncrement, 0);
							Projectile.NewProjectile(laserPositionLeftIncr, Vector2.Zero, mod.ProjectileType("TeslaLine"), (int)primaryAttackDamage, 10, Main.myPlayer);
						}
					}
					else
					{
						Vector2 laserPositionLeftIncr = laserPositionLeft + new Vector2(laserIncrement * 0.5f, 0);
						while (laserPositionLeftIncr.X < laserPositionRight.X)
						{
							laserPositionLeftIncr = laserPositionLeftIncr + new Vector2(laserIncrement, 0);
							Projectile.NewProjectile(laserPositionLeftIncr, Vector2.Zero, mod.ProjectileType("TeslaLine"), (int)primaryAttackDamage, 10, Main.myPlayer);
						}
					}

				}
				npc.netUpdate = true;
			}
	
		}

		private void ResetPhase2()
		{
			primaryAttackTimeInterval = 60;
			secondaryAttackTimeInterval = 120;

			laserPositionLeft = Main.screenPosition;
			laserPositionRight = Main.screenPosition + new Vector2(Main.screenWidth, 0);
			laserIncrement = 120;
			laserLineSwitcher = false;
			trueVelocity = 5;

			primaryAttackDamage = npc.damage * 0.75f;
		}


		int teleportDistance = 750;
		int minDistance = 200;
		int tileWidth = 8;
		int tileHeight = 11;
		int randX;
		int randY;

		Point teleportVecTile = new Point(0, 0);
		Vector2 teleportVec = Vector2.Zero;
		int iterations = 0;
		int maxIterations = 15;

		// Teleports to a random location; tries several times to pick empty air, but will eventually resort to inside tiles if no air
		public void Teleport()
        {
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				iterations = 0; // failsafe if no open air
				do
				{
					iterations++;

					// set up new random coordinates for next loop
					//teleportVec = new Vector2(rand.Next(-teleportDistance, teleportDistance), rand.Next(-teleportDistance, teleportDistance));
					
					randX = rand.Next(-teleportDistance, teleportDistance);
					randY = rand.Next(-teleportDistance, teleportDistance);

					// constraints to ensure it does not appear too close to player
					if (randX >= 0 && randX < minDistance)
					{
						randX = minDistance;
					}
					if (randX < 0 && randX > -minDistance)
					{
						randX = -minDistance;
					}
					if (randY >= 0 && randY < minDistance)
					{
						randY = minDistance;
					}
					if (randY < 0 && randY > -minDistance)
					{
						randY = -minDistance;
					}

					teleportVec = new Vector2(randX, randY);

					teleportVec = currentTarget.position + teleportVec;
					teleportVecTile = teleportVec.ToTileCoordinates();

				} while (iterations < maxIterations && Collision.SolidTiles(teleportVecTile.X, teleportVecTile.X + tileWidth, teleportVecTile.Y, teleportVecTile.Y + tileHeight) == true);
				// if have tried 10 times, or if the area is air, then stop the loop

				Main.TeleportEffect(npc.getRect(), 3, 0, 50f); // generates sound + dust; TODO: replace with individual dust and sounds 

				npc.position = teleportVec; // the actual "teleportation"

				if (phase == 1)
                {
					Projectile.NewProjectile(npc.Center, Vector2.Zero, mod.ProjectileType("TeslaLine"), (int)(npc.damage * 0.75f), 10, Main.myPlayer);
					Projectile.NewProjectile(npc.Center, Vector2.Zero, mod.ProjectileType("HorizontalTeslaLine"), (int)(npc.damage * 0.75f), 10, Main.myPlayer);
				}
				
				npc.netUpdate = true;
				
			}
        }






		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.625f * bossLifeScale);
			npc.damage = (int)(npc.damage * 0.6f);
		}

		private void Talk(string message)
		{
			if (Main.netMode != NetmodeID.Server)
			{
				Main.NewText(message, 150, 250, 150);
			}
			else
			{
				NetworkText text = NetworkText.FromLiteral(message);
				NetMessage.BroadcastChatMessage(text, new Color(150, 250, 150));
			}
		}

		public void CheckForDespawn()
        {
			npc.TargetClosest(false);
			Player player = Main.player[npc.target];
			if (npc.target < 0 || npc.target == 255 || player.dead || !player.active)
			{
				npc.TargetClosest(false);
				npc.direction = 1;
				npc.velocity.Y = npc.velocity.Y + 1.5f;
				if (npc.timeLeft > 15)
				{
					npc.timeLeft = 15;
					return;
				}
			}
		}


		public void AddEffects()
		{
			// Lighting
			Lighting.AddLight(npc.Center, Color.White.ToVector3() * 0.78f);

			// Dusts

			for (int d = 0; d < 2; d++)
			{
				Dust.NewDust(npc.position + new Vector2(0, npc.height + 2), npc.width, 1, 6, 0, 5f); // exhaust fire
																									 //Dust.NewDust(npc.position + new Vector2(0, npc.height), npc.width, 1, 109, 0, 2f); // smoke
			}


			if (inPhaseTwoAttack)
            {
				Vector2 randomVec = Utils.RandomVector2(new Terraria.Utilities.UnifiedRandom(), -3f, 3f);
				randomVec = Vector2.Normalize(randomVec) * 15f;
				Dust.NewDust(npc.position + new Vector2(npc.width * 0.406f, npc.height * 0.0425f), (int)(npc.width * 0.176f), (int)(npc.height * 0.17f), 74, randomVec.X, randomVec.Y); // acid green diamond

			}
			else if (AI_Timer % 4 == 0 && phase == 1)
			{
				Vector2 randomVec = Utils.RandomVector2(new Terraria.Utilities.UnifiedRandom(), -3f, 3f);
				randomVec = Vector2.Normalize(randomVec) * 1.5f;
				Dust.NewDust(npc.position + new Vector2(npc.width * 0.406f, npc.height * 0.0425f), (int)(npc.width * 0.176f), (int)(npc.height * 0.17f), 74, randomVec.X, randomVec.Y); // acid green diamond
			}
			else if (AI_Timer % 5 == 0) // diamond dust phase 0
			{
				//Dust.NewDust(npc.position + new Vector2(npc.width * 0.406f, npc.height * 0.0425f), (int)(npc.width * 0.176f), (int)(npc.height * 0.17f), 74); // acid green diamond
				Vector2 randomVec = Utils.RandomVector2(new Terraria.Utilities.UnifiedRandom(), -3f, 3f);
				randomVec = Vector2.Normalize(randomVec) * 4f;
				Dust.NewDust(npc.position + new Vector2(npc.width * 0.406f, npc.height * 0.0425f), (int)(npc.width * 0.176f), (int)(npc.height * 0.17f), 74, randomVec.X, randomVec.Y); // acid green diamond

			}
			

		}
			int currFrame = 0;
			
		public override void FindFrame(int frameHeight)
        {
			SelectFrameCycle();
			
			if (AI_Timer % frameCycle == 0) // change frames 5 times a second by dafult, changes by phase
			{
				npc.frame.Y = frameHeight * currFrame;
				currFrame++;
				if (currFrame > 11)
                {
					currFrame = 0;
                }
			}
		}

		int frameCycle;
		private void SelectFrameCycle()
        {
			if (inPhaseTwoAttack)
            {
				frameCycle = 2;
            } else if (phase == 1){
				frameCycle = 4;
            } else
            {
				frameCycle = 8;
            }
        }

        // Our texture is 32x32 with 2 pixels of padding vertically, so 34 is the vertical spacing.  These are for my benefit and the numbers could easily be used directly in the code below, but this is how I keep code organized.
        private const int Frame_Asleep = 0;
		private const int Frame_Notice = 1;
		private const int Frame_Falling = 2;
		private const int Frame_Flutter_1 = 3;
		private const int Frame_Flutter_2 = 4;
		private const int Frame_Flutter_3 = 5;

		// Here in FindFrame, we want to set the animation frame our npc will use depending on what it is doing.
		// We set npc.frame.Y to x * frameHeight where x is the xth frame in our spritesheet, counting from 0. For convenience, I have defined some consts above.
		
		/**
		public override void FindFrame(int frameHeight)
		{

			// This makes the sprite flip horizontally in conjunction with the npc.direction.
			npc.spriteDirection = npc.direction;

			// For the most part, our animation matches up with our states.
			if (AI_State == State_Asleep)
			{
				// npc.frame.Y is the goto way of changing animation frames. npc.frame starts from the top left corner in pixel coordinates, so keep that in mind.
				npc.frame.Y = Frame_Asleep * frameHeight;
			}
			else if (AI_State == State_Notice)
			{
				// Going from Notice to Asleep makes our npc look like it's crouching to jump.
				if (AI_Timer < 10)
				{
					npc.frame.Y = Frame_Notice * frameHeight;
				}
				else
				{
					npc.frame.Y = Frame_Asleep * frameHeight;
				}
			}
			else if (AI_State == State_Jump)
			{
				npc.frame.Y = Frame_Falling * frameHeight;
			}
			else if (AI_State == State_Hover)
			{
				// Here we have 3 frames that we want to cycle through.
				npc.frameCounter++;
				if (npc.frameCounter < 10)
				{
					npc.frame.Y = Frame_Flutter_1 * frameHeight;
				}
				else if (npc.frameCounter < 20)
				{
					npc.frame.Y = Frame_Flutter_2 * frameHeight;
				}
				else if (npc.frameCounter < 30)
				{
					npc.frame.Y = Frame_Flutter_3 * frameHeight;
				}
				else
				{
					npc.frameCounter = 0;
				}
			}
			else if (AI_State == State_Fall)
			{
				npc.frame.Y = Frame_Falling * frameHeight;
			}
		}
		**/
	}

}
