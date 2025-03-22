using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System.Collections.Generic;

namespace BasicMod.GlobalNPCs
{
    public class InvasionGlobalNPC : GlobalNPC
    {
        /**
        public static int[] invaders = {
            ModContent.NPCType<NPCs.SwatSniper>(),
            //ModContent.NPCType<NPCs.ZombieArcher>()
            ModContent.NPCType<NPCs.SwatCopter>(),
            ModContent.NPCType<NPCs.SwatDoorRam>(),
            ModContent.NPCType<NPCs.SwatCruiseMissile>()
        };
        **/

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            /**
            //If the custom invasion is up and the invasion has reached the spawn pos
            if (BasicWorld.
            
            
            
            
            
            Event && (Main.invasionX == (double)Main.spawnTileX))
            {
                //Clear pool so that only the stuff you want spawns
                pool.Clear();

                //key = NPC ID | value = spawn weight
                //pool.add(key, value)

                //For every ID inside the invader array in our CustomInvasion file
                //foreach (int i in invaders)
                //{
                  //  pool.Add(i, 1f); //Add it to the pool with the same weight of 1
                //}
                pool.Add(ModContent.NPCType<NPCs.SwatDoorRam>(), 1f);
                pool.Add(ModContent.NPCType<NPCs.SwatSniper>(), 0.3f);
                pool.Add(ModContent.NPCType<NPCs.SwatCruiseMissile>(), 0.2f);

                if (!NPC.AnyNPCs(mod.NPCType("SwatCopter")))
                {
                    pool.Add(ModContent.NPCType<NPCs.SwatCopter>(), 0.2f);
                }
            }
            **/
        }

        //Adding to the AI of an NPC
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            //Main.NewText(spawnRate + " " + maxSpawns);
            if (BasicWorld.SwatEvent)
            {
                spawnRate = 5;
                maxSpawns = 15;
                /**
                if (NPC.AnyNPCs(mod.NPCType("TheGreatTyrannosaurus")))
                {
                    spawnRate = 0;
                    maxSpawns = 0;
                }
                else
                {
                    spawnRate = 10;
                    maxSpawns = 10;
                }
                **/
            }
            
        }

        public override void PostAI(NPC npc)
        {
            /**
            //Changes NPCs so they do not despawn when invasion up and invasion at spawn
            if (BasicWorld.SwatEvent && (Main.invasionX == (double)Main.spawnTileX))
            {
                npc.timeLeft = 1000;
            }
            **/
        }

        public override void NPCLoot(NPC npc)
        {
            /**
            int swatDoorRamID = ModContent.NPCType<NPCs.SwatDoorRam>();
            //When an NPC (from the invasion list) dies, add progress by decreasing size
            if (BasicWorld.SwatEvent)
            {
                switch (npc.type)
                {
                    case swatDoorRamID:
                    case NPCID.SkeletonSniper:
                    case NPCID.SkeletonCommando:
                        Console.WriteLine("Case 1");
                        BasicWorld.SwatKillCount += 2;
                        break;
                    case 2:
                        Console.WriteLine("Case 2");
                        break;
                    default:
                        Console.WriteLine("Default case");
                        break;
                }
                //Gets IDs of invaders from CustomInvasion file
                foreach (int invader in invaders)
                {
                    //If npc type equal to invader's ID decrement size to progress invasion
                    if (npc.type == invader)
                    {
                        BasicWorld.SwatKillCount += 5;
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.WorldData); // Immediately inform clients of new world state.
                    }

                }
            }
            **/
        }

    }
}