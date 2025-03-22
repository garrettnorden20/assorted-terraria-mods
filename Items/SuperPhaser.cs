using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections;
using System;

namespace BasicMod.Items
{
    public class SuperPhaser : ModItem
    {
        public override string Texture => "Terraria/Item_" + ItemID.CobaltBreastplate;
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Phase through anything." +
                "\nDouble tap in any cardinal direction to do a dash!");
        }

        public override void SetDefaults()
        {
            //item.damage = 50;
            //item.knockBack = 6;

            item.defense = 8;
            item.accessory = true;
            item.rare = ItemRarityID.Blue;
            item.value = Item.sellPrice(silver: 60);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            ExampleSuperPhaserDashPlayer mp = player.GetModPlayer<ExampleSuperPhaserDashPlayer>();
            player.maxRunSpeed = 200f;

            if (!mp.DashActive)
            {
                return;
            }

            // if player lets go of these buttons, the dash is over
            if ((mp.DashDir == ExampleSuperPhaserDashPlayer.DashRight && player.releaseRight == true) ||
                (mp.DashDir == ExampleSuperPhaserDashPlayer.DashLeft && player.releaseLeft == true))
            {

                if (mp.MinDashTime <= 0) // has been going for the minimum amount of time
                {
                    mp.DashActive = false;
                }

            }

            if (!mp.DashActive || mp.DashTimer == 0)
            {
                //The dash has ended.  Reset the fields
                mp.DashDelay = ExampleSuperPhaserDashPlayer.MAX_DASH_DELAY;
                mp.DashTimer = ExampleSuperPhaserDashPlayer.MAX_DASH_TIMER;
                mp.MinDashTime = ExampleSuperPhaserDashPlayer.MIN_DASH_TIME;
                mp.DashActive = false;

                // clean up tiles from before
                reactivateTiles(player);
                return;
            }


            //This is where we set the afterimage effect.  You can replace these two lines with whatever you want to happen during the dash
            //Some examples include:  spawning dust where the player is, adding buffs, making the player immune, etc.
            //Here we take advantage of "player.eocDash" and "player.armorEffectDrawShadowEOCShield" to get the Shield of Cthulhu's afterimage effect
            player.eocDash = mp.DashTimer;
            player.armorEffectDrawShadowEOCShield = true;




            // MAIN DEACTIVATE TILES HERE
            if (mp.DashTimer > 0)
            {
                deactivateTilesBetter(player);
            }


            Random rand = new Random();
            for (int i = 0; i < 1; i++) // dust every frame
            {
                Dust.NewDust(player.position, player.width, player.height / 2, 198); // white puffies?

            }

            //If the dash has just started, apply the dash velocity in whatever direction we wanted to dash towards
            if (true || mp.DashTimer == ExampleSuperPhaserDashPlayer.MAX_DASH_TIMER)
            {
                Vector2 newVelocity = player.velocity;

                if ((mp.DashDir == ExampleSuperPhaserDashPlayer.DashLeft && player.velocity.X > -mp.DashVelocity) || (mp.DashDir == ExampleSuperPhaserDashPlayer.DashRight && player.velocity.X < mp.DashVelocity))
                {
                    //X-velocity is set here
                    int dashDirection = mp.DashDir == ExampleSuperPhaserDashPlayer.DashRight ? 1 : -1;
                    newVelocity.X = dashDirection * mp.DashVelocity;
                }

                player.velocity = newVelocity;

            }

            //Decrement the timers
            mp.DashTimer--;
            mp.DashDelay--;
            mp.MinDashTime--;


        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.DirtBlock, 20);
            recipe.SetResult(this);
            recipe.AddRecipe();

        }

        //public ArrayList deActTiles = new ArrayList();

        public void deactivateTiles(Player p) // OLD DONT USE
        {
            //Main.NewText("start of deactive");
            int deRadius = 3; // the distance away you want blocks to disappear
            int additionalBuffer = 2; // for checking if out of bounds, to make back to real blocks

            int upDistance = (p.controlJump) ? 1 : 0; // if player is pressing up, then blocks to the up will begin to deactive
            int downDistance = (p.controlDown) ? 1 : 0;

            int minTileX = (int)(p.position.X / 16f - (float)deRadius) - additionalBuffer;
            int maxTileX = (int)(p.position.X / 16f + (float)deRadius) + additionalBuffer;
            int minTileY = (int)((p.position.Y / 16f) - upDistance);
            int maxTileY = (int)((p.position.Y / 16f) + 2 + downDistance);
            if (minTileX < 0)
            {
                minTileX = 0;
            }
            if (maxTileX > Main.maxTilesX)
            {
                maxTileX = Main.maxTilesX;
            }
            if (minTileY < 0)
            {
                minTileY = 0;
            }
            if (maxTileY > Main.maxTilesY)
            {
                maxTileY = Main.maxTilesY;
            }
            for (int i = minTileX; i <= maxTileX; i++)
            {
                for (int j = minTileY; j <= maxTileY; j++)
                {
                    float diffX = Math.Abs((float)i - p.position.X / 16f);
                    float diffY = Math.Abs((float)j - p.position.Y / 16f);

                    double distanceToTile = 0.0;
                    distanceToTile = Math.Sqrt((double)(diffX * diffX + diffY * diffY));

                    //Main.NewText("BOTH LOOPS");
                    if (distanceToTile < (double)deRadius) // make the tile inactive if distance is small
                    {
                        //Main.NewText("PAST DIFF CHECK");
                        if (Main.tile[i, j] != null && Main.tile[i, j].active())
                        {
                            //Main.NewText("BEFORE KILL");

                            Main.tile[i, j].inActive(true); // sets it to unactive
                            if (!Main.tile[i, j].active() && Main.netMode != NetmodeID.SinglePlayer)
                            {
                                NetMessage.SendData(MessageID.TileChange, -1, -1, null, 0, (float)i, (float)j, 0f, 0, 0, 0);
                            }



                            Point po = new Point(i, j);
                            Dust.NewDust(po.ToWorldCoordinates(), 8, 8, 175);



                        }
                    }
                    else // make the tile actile again if it's farther away from player
                    {
                        if (Main.tile[i, j] != null && Main.tile[i, j].inActive())
                        {
                            Main.tile[i, j].inActive(false);
                            if (Main.tile[i, j].active() && Main.netMode != NetmodeID.SinglePlayer)
                            {
                                NetMessage.SendData(MessageID.TileChange, -1, -1, null, 0, (float)i, (float)j, 0f, 0, 0, 0);
                            }

                        }
                    }
                }
            }



        }

        ArrayList tilesList = new ArrayList();
        public void deactivateTilesBetter(Player p) // runs each frame
        {
            // basically, one array surrounding player; in the blocks within distance deRadius, will dissapear, but those outside that radius 
            // reappear

            //Main.NewText("start of deactive");
            int deRadius = 3; // the distance away you want blocks to disappear
            int additionalBuffer = 2; // for checking if out of bounds, to make back to real blocks

            int upDistance = (p.controlJump) ? deRadius : 0; // if player is pressing up, then blocks to the up will begin to deactive
            int downDistance = (p.controlDown) ? deRadius : 0;

            int minTileX = (int)(p.position.X / 16f - (float)deRadius) - additionalBuffer;
            int maxTileX = (int)(p.position.X / 16f + (float)deRadius) + additionalBuffer;
            int minTileY = (int)((p.position.Y / 16f) - deRadius - additionalBuffer);
            int maxTileY = (int)((p.position.Y / 16f) + 2 + deRadius + additionalBuffer);
            if (minTileX < 0)
            {
                minTileX = 0;
            }
            if (maxTileX > Main.maxTilesX)
            {
                maxTileX = Main.maxTilesX;
            }
            if (minTileY < 0)
            {
                minTileY = 0;
            }
            if (maxTileY > Main.maxTilesY)
            {
                maxTileY = Main.maxTilesY;
            }
            for (int i = minTileX; i <= maxTileX; i++)
            {
                for (int j = minTileY; j <= maxTileY; j++)
                {
                    float diffX = Math.Abs((float)i - p.position.X / 16f);
                    float diffY = Math.Abs((float)j - p.position.Y / 16f);

                    double distanceToTile = 0.0;
                    distanceToTile = Math.Sqrt((double)(diffX * diffX + diffY * diffY));

                    //Main.NewText("BOTH LOOPS");
                    if (distanceToTile < (double)deRadius) // make the tile inactive if distance is small
                    {
                        //Main.NewText("PAST DIFF CHECK");
                        if (Main.tile[i, j] != null && Main.tile[i, j].active())
                        {
                            bool abovePlayer = j < (p.position.Y / 16f) - 1; // if the j is currently above the player
                            bool belowPlayer = j > (p.position.Y / 16f) + 2; // if the j is currently below the player
                            bool willDeactivate = false;
                            if (abovePlayer)
                            {
                                if (upDistance != 0) // if j is above player, and Space is currently pressed (character ascending)
                                {
                                    //Main.tile[i, j].inActive(true);
                                    willDeactivate = true;
                                }
                            }
                            else if (belowPlayer)
                            {
                                if (downDistance != 0) // if j is below player, and Down is currently pressed (character descending)
                                {
                                    //Main.tile[i, j].inActive(true);
                                    willDeactivate = true;
                                }
                            }
                            else
                            {
                                //Main.tile[i, j].inActive(true);
                                willDeactivate = true;
                            }
                            if (willDeactivate)
                            {
                                Main.tile[i, j].inActive(true);
                                //int[] curr = { i, j };
                                //tilesList.Add(curr); // add the point to a list

                            }

                            //Main.tile[i, j].inActive(true); // sets it to unactive
                            if (!Main.tile[i, j].active() && Main.netMode != NetmodeID.SinglePlayer)
                            {
                                NetMessage.SendData(MessageID.TileChange, -1, -1, null, 0, (float)i, (float)j, 0f, 0, 0, 0);
                            }



                            Point po = new Point(i, j);
                            Dust.NewDust(po.ToWorldCoordinates(), 8, 8, 175);

                        }
                    }
                    else // make the tile active again if it's farther away from player
                    {
                        if (Main.tile[i, j] != null && Main.tile[i, j].inActive())
                        {
                            Main.tile[i, j].inActive(false);
                            if (Main.tile[i, j].active() && Main.netMode != NetmodeID.SinglePlayer)
                            {
                                NetMessage.SendData(MessageID.TileChange, -1, -1, null, 0, (float)i, (float)j, 0f, 0, 0, 0);
                            }
                            //int[] curr = { i, j };
                            //tilesList.Remove(curr); // try to remove it from the list



                        }
                    }
                }
            }
        }

        public void reactivateTiles(Player p)
        {

            {
                // basically, one array surrounding player; in the blocks within distance deRadius, will dissapear, but those outside that radius 
                // reappear

                //Main.NewText("start of deactive");
                int deRadius = 3; // the distance away you want blocks to disappear
                int additionalBuffer = 2; // for checking if out of bounds, to make back to real blocks

                int upDistance = (p.controlJump) ? deRadius : 0; // if player is pressing up, then blocks to the up will begin to deactive
                int downDistance = (p.controlDown) ? deRadius : 0;

                int minTileX = (int)(p.position.X / 16f - (float)deRadius) - additionalBuffer;
                int maxTileX = (int)(p.position.X / 16f + (float)deRadius) + additionalBuffer;
                int minTileY = (int)((p.position.Y / 16f) - deRadius - additionalBuffer);
                int maxTileY = (int)((p.position.Y / 16f) + 2 + deRadius + additionalBuffer);
                if (minTileX < 0)
                {
                    minTileX = 0;
                }
                if (maxTileX > Main.maxTilesX)
                {
                    maxTileX = Main.maxTilesX;
                }
                if (minTileY < 0)
                {
                    minTileY = 0;
                }
                if (maxTileY > Main.maxTilesY)
                {
                    maxTileY = Main.maxTilesY;
                }
                for (int i = minTileX; i <= maxTileX; i++)
                {
                    for (int j = minTileY; j <= maxTileY; j++)
                    {
                        float diffX = Math.Abs((float)i - p.Center.X / 16f);
                        float diffY = Math.Abs((float)j - p.Center.Y / 16f);

                        double distanceToTile = 0.0;
                        distanceToTile = Math.Sqrt((double)(diffX * diffX + diffY * diffY));

                        //Main.NewText("BOTH LOOPS");
                        if (distanceToTile < (double)1) // make the tile inactive if distance is small
                        {
                            //Main.NewText("PAST DIFF CHECK");
                            if (Main.tile[i, j] != null && Main.tile[i, j].active())
                            {
                                bool abovePlayer = j < (p.position.Y / 16f) - 1; // if the j is currently above the player
                                bool belowPlayer = j > (p.position.Y / 16f) + 2; // if the j is currently below the player
                                bool willDeactivate = false;
                                if (abovePlayer)
                                {
                                    if (upDistance != 0) // if j is above player, and Space is currently pressed (character ascending)
                                    {
                                        //Main.tile[i, j].inActive(true);
                                        willDeactivate = true;
                                    }
                                }
                                else if (belowPlayer)
                                {
                                    if (downDistance != 0) // if j is below player, and Down is currently pressed (character descending)
                                    {
                                        //Main.tile[i, j].inActive(true);
                                        willDeactivate = true;
                                    }
                                }
                                else
                                {
                                    //Main.tile[i, j].inActive(true);
                                    willDeactivate = true;
                                }
                                if (willDeactivate)
                                {
                                    Main.tile[i, j].inActive(true);
                                    //int[] curr = { i, j };
                                    //tilesList.Add(curr); // add the point to a list

                                }

                                //Main.tile[i, j].inActive(true); // sets it to unactive
                                if (!Main.tile[i, j].active() && Main.netMode != NetmodeID.SinglePlayer)
                                {
                                    NetMessage.SendData(MessageID.TileChange, -1, -1, null, 0, (float)i, (float)j, 0f, 0, 0, 0);
                                }



                                Point po = new Point(i, j);
                                Dust.NewDust(po.ToWorldCoordinates(), 8, 8, 175);

                            }
                        }
                        else // make the tile active again if it's farther away from player
                        {
                            if (Main.tile[i, j] != null && Main.tile[i, j].inActive())
                            {
                                Main.tile[i, j].inActive(false);
                                if (Main.tile[i, j].active() && Main.netMode != NetmodeID.SinglePlayer)
                                {
                                    NetMessage.SendData(MessageID.TileChange, -1, -1, null, 0, (float)i, (float)j, 0f, 0, 0, 0);
                                }
                                //int[] curr = { i, j };
                                //tilesList.Remove(curr); // try to remove it from the list



                            }
                        }
                    }
                }
            }



        }
    }



    public class ExampleSuperPhaserDashPlayer : ModPlayer
    {
        //These indicate what direction is what in the timer arrays used
        public static readonly int DashDown = 0;
        public static readonly int DashUp = 1;
        public static readonly int DashRight = 2;
        public static readonly int DashLeft = 3;

        //The direction the player is currently dashing towards.  Defaults to -1 if no dash is ocurring.
        public int DashDir = -1;

        //The fields related to the dash accessory
        public bool DashActive = false;
        public int DashDelay = MAX_DASH_DELAY;
        public int DashTimer = MAX_DASH_TIMER;
        public int MinDashTime = MIN_DASH_TIME;
        //The initial velocity.  10 velocity is about 37.5 tiles/second or 50 mph
        public readonly float DashVelocity = 16f;
        //These two fields are the max values for the delay between dashes and the length of the dash in that order
        //The time is measured in frames
        public static readonly int MAX_DASH_DELAY = 40;
        public static readonly int MAX_DASH_TIMER = 18000;
        public static readonly int MIN_DASH_TIME = 10;

        public override void OnHitByNPC(NPC npc, int damage, bool crit)
        {
            if (DashActive)
            {

                //npc.AddBuff(BuffID.OnFire, 60 * Main.rand.Next(5, 15), false); // adds buff to the npc that hit you

            }
            base.OnHitByNPC(npc, damage, crit);
        }

        public override void ResetEffects()
        {
            //ResetEffects() is called not long after player.doubleTapCardinalTimer's values have been set

            //Check if the ExampleDashAccessory is equipped and also check against this priority:
            // If the Shield of Cthulhu, Master Ninja Gear, Tabi and/or Solar Armour set is equipped, prevent this accessory from doing its dash effect
            //The priority is used to prevent undesirable effects.
            //Without it, the player is able to use the ExampleDashAccessory's dash as well as the vanilla ones
            bool dashAccessoryEquipped = false;

            //This is the loop used in vanilla to update/check the not-vanity accessories
            for (int i = 3; i < 8 + player.extraAccessorySlots; i++)
            {
                Item item = player.armor[i];

                //Set the flag for the ExampleDashAccessory being equipped if we have it equipped OR immediately return if any of the accessories are
                // one of the higher-priority ones
                if (item.type == ModContent.ItemType<SuperPhaser>())
                    dashAccessoryEquipped = true;
                else if (item.type == ItemID.EoCShield || item.type == ItemID.MasterNinjaGear || item.type == ItemID.Tabi)
                    return;
            }

            //If we don't have the ExampleDashAccessory equipped or the player has the Solor armor set equipped, return immediately
            //Also return if the player is currently on a mount, since dashes on a mount look weird, or if the dash was already activated
            if (!dashAccessoryEquipped || player.setSolar || player.mount.Active || DashActive)
                return;

            //When a directional key is pressed and released, vanilla starts a 15 tick (1/4 second) timer during which a second press activates a dash
            //If the timers are set to 15, then this is the first press just processed by the vanilla logic.  Otherwise, it's a double-tap

            else if (player.controlRight && player.releaseRight && player.doubleTapCardinalTimer[DashRight] < 15)
                DashDir = DashRight;
            else if (player.controlLeft && player.releaseLeft && player.doubleTapCardinalTimer[DashLeft] < 15)
                DashDir = DashLeft;
            else
                return;  //No dash was activated, return

            DashActive = true;

            //Main.NewText("dashing i be");
            //Here you'd be able to set an effect that happens when the dash first activates
            //Some examples include:  the larger smoke effect from the Master Ninja Gear and Tabi
        }
    }
}