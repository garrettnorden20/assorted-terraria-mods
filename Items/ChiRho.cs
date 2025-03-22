using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections;
using System;

namespace BasicMod.Items
{
    public class ChiRhoShield : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("The crusader's shield." +
                "\nDouble tap in any cardinal direction to do a dash!");
        }

        public override void SetDefaults()
        {
            item.damage = 50;
            item.knockBack = 6;

            item.defense = 8;
            item.accessory = true;
            item.rare = ItemRarityID.Blue;
            item.value = Item.sellPrice(silver: 60);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            ExampleDashPlayer mp = player.GetModPlayer<ExampleDashPlayer>();

            //If the dash is not active, immediately return so we don't do any of the logic for it
            if (!mp.DashActive)
                return;

            //This is where we set the afterimage effect.  You can replace these two lines with whatever you want to happen during the dash
            //Some examples include:  spawning dust where the player is, adding buffs, making the player immune, etc.
            //Here we take advantage of "player.eocDash" and "player.armorEffectDrawShadowEOCShield" to get the Shield of Cthulhu's afterimage effect
            player.eocDash = mp.DashTimer;
            player.armorEffectDrawShadowEOCShield = true;

            deactivateTilesBetter(player);

            Random rand = new Random();
            for (int i = 0; i < rand.Next(1, 4); i++) // x-(y-1) golden dusts every frame
            {
                Dust.NewDust(player.position, player.width, player.height, 169);

            }

            //If the dash has just started, apply the dash velocity in whatever direction we wanted to dash towards
            if (mp.DashTimer == ExampleDashPlayer.MAX_DASH_TIMER)
            {
                Vector2 newVelocity = player.velocity;

                if ((mp.DashDir == ExampleDashPlayer.DashLeft && player.velocity.X > -mp.DashVelocity) || (mp.DashDir == ExampleDashPlayer.DashRight && player.velocity.X < mp.DashVelocity))
                {
                    //X-velocity is set here
                    int dashDirection = mp.DashDir == ExampleDashPlayer.DashRight ? 1 : -1;
                    newVelocity.X = dashDirection * mp.DashVelocity;
                }

                player.velocity = newVelocity;

                // shoot projectile
                Vector2 mousePosition = Main.MouseWorld; // the mouse's position
                Vector2 projVelocity = new Vector2(mousePosition.X - player.position.X, mousePosition.Y - player.position.Y); // get vector between Player and Mouse
                projVelocity = Vector2.Normalize(projVelocity); // get unit vector for it (just direction)

                Projectile.NewProjectile(player.position, 1f * projVelocity, mod.ProjectileType("ChiRhoProj"), (int)player.meleeDamage * item.damage, 6, Main.myPlayer);
                // player.meleeDamage is a MULTIPLIER, not a solid value

            }

            //Decrement the timers
            mp.DashTimer--;
            mp.DashDelay--;

            if (mp.DashDelay == 0)
            {
                //The dash has ended.  Reset the fields
                mp.DashDelay = ExampleDashPlayer.MAX_DASH_DELAY;
                mp.DashTimer = ExampleDashPlayer.MAX_DASH_TIMER;
                mp.DashActive = false;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.DirtBlock, 20);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public ArrayList deActTiles = new ArrayList();

        public void deactivateTiles(Player p) // runs each frame
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


        public void deactivateTilesBetter(Player p) // runs each frame
        {
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
                            bool abovePlayer = j < (p.position.Y / 16f) - 1;
                            bool belowPlayer = j > (p.position.Y / 16f) + 2;
                            if (abovePlayer)
                            {
                                if (upDistance != 0)
                                {
                                    Main.tile[i, j].inActive(true);
                                }
                            } else if (belowPlayer)
                            {
                                if (downDistance != 0)
                                {
                                    Main.tile[i, j].inActive(true);
                                }
                            } else
                            {
                                Main.tile[i, j].inActive(true);
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

        //public void reactivateTiles 



        public class ExampleDashPlayer : ModPlayer
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
            //The initial velocity.  10 velocity is about 37.5 tiles/second or 50 mph
            public readonly float DashVelocity = 16f;
            //These two fields are the max values for the delay between dashes and the length of the dash in that order
            //The time is measured in frames
            public static readonly int MAX_DASH_DELAY = 50;
            public static readonly int MAX_DASH_TIMER = 35;

            public override void OnHitByNPC(NPC npc, int damage, bool crit)
            {
                if (DashActive)
                {

                    npc.AddBuff(BuffID.OnFire, 60 * Main.rand.Next(5, 15), false); // adds buff to the npc that hit you

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
                    if (item.type == ModContent.ItemType<ChiRhoShield>())
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
}