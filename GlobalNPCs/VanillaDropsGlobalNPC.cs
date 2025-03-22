using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System.Collections.Generic;

namespace BasicMod.GlobalNPCs
{
    public class VanillaDropsGlobalNPC : GlobalNPC
    {
        public override void NPCLoot(NPC npc)
        {
            if (npc.type == NPCID.Skeleton)
            {
                if (Main.rand.Next(50) == 0)
                    Item.NewItem(npc.getRect(), mod.ItemType("Skelerang"));
            }
            
            if (npc.type == NPCID.SkeletronHead)
            {
                if (Main.rand.Next(4) == 0)
                    Item.NewItem(npc.getRect(), mod.ItemType("Skelerang"));
            }

            if (npc.type == NPCID.QueenBee)
            {
                if (Main.rand.Next(3) == 0)
                    Item.NewItem(npc.getRect(), mod.ItemType("Beemerang"));
            }
        }

        public override void PostAI(NPC npc)
        {
            if (npc.type == NPCID.Lihzahrd)
            {
                Main.NewText(npc.ai[0] + " " + npc.ai[1] + " " + npc.ai[2] + " " + npc.ai[3] + " " + npc.localAI[0] + npc.localAI[1] + npc.localAI[2] + npc.localAI[3]);
            }
            if (npc.type == mod.NPCType("Knight"))
            {
                Main.NewText(npc.ai[0] + " " + npc.ai[1] + " " + npc.ai[2] + " " + npc.ai[3] + " " + npc.localAI[0] + npc.localAI[1] + npc.localAI[2] + npc.localAI[3]);
            }
        }
    }
}