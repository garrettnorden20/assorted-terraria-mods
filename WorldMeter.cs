using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BasicMod
{
    class WorldMeter
    {
        public static int MeterMin = -1000;
        public static int MeterMax = 1000;

        public static void ChangeMeter(int change)
        {
            if (!isWithinConstraints(change))
            {
                return;
            }

            BasicWorld.worldMeter += change;

            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.WorldData); // Immediately inform clients of new world state. This is like updating all the flags for us automatically.
            }
               

        }

        private static bool isWithinConstraints(int change)
        {
            if (BasicWorld.worldMeter + change >= MeterMax || BasicWorld.worldMeter + change <= MeterMin)
            {
                return false;
            }
            return true;
        }

    }

    class MeterPlayer : ModPlayer
    {

        public override bool Shoot(Item item, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (item.type == ItemID.Clentaminator)
            {
                WorldMeter.ChangeMeter(2);
            }
            return base.Shoot(item, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
        }
    }
}
