using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using BasicMod.Buffs;
using BasicMod;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace BasicMod.Mounts
{

    public class DrillContain : ModMountData
    {
        //float maxLengthBeam = 224f;
        //float maxRadius = 65.6f;
        float maxLengthBeam = 400f;
        float maxLengthAttackBeam = 448f;
        float maxRadius = 65.6f;


        private class DrillBeam
        {
            public Point16 curTileTarget;

            public NPC curEnemyTarget; // added

            public int cooldown;

            public DrillBeam()
            {
                this.curTileTarget = Point16.NegativeOne;
                this.cooldown = 0;
                this.curEnemyTarget = null;
            }
        }

        private class DrillMountData
        {
            // int maxNumBeams = 4;
            int maxNumBeams = 40;


            public float diodeRotationTarget;

            public float diodeRotation;

            public float outerRingRotation;

            public DrillBeam[] beams;

            public int beamCooldown;

            public Vector2 crosshairPosition;

            public DrillMountData()
            {
                this.beams = new DrillBeam[maxNumBeams];
                for (int i = 0; i < this.beams.Length; i++)
                {
                    this.beams[i] = new DrillBeam();
                }
            }
        }
        public override void SetDefaults()
        {
            mountData.spawnDust = 226;
            mountData.buff = ModContent.BuffType<DCUBuff>();
            mountData.heightBoost = 16;
            mountData.flightTimeMax = 320;
            mountData.fatigueMax = 320;
            mountData.fallDamage = 1f;
            mountData.usesHover = false;
            mountData.swimSpeed = 4f;
            mountData.runSpeed = 6f;
            mountData.dashSpeed = 4f;
            mountData.acceleration = 0.16f;
            mountData.jumpHeight = 10;
            mountData.jumpSpeed = 4f;
            mountData.blockExtraJumps = true;
            mountData.emitsLight = true;
            mountData.lightColor = new Vector3(0.3f, 0.3f, 0.4f);
            mountData.totalFrames = 1;
            int[] array = new int[mountData.totalFrames];
            for (int num3 = 0; num3 < array.Length; num3++)
            {
                array[num3] = 4;
            }
            mountData.playerYOffsets = array;
            mountData.xOffset = 1;
            mountData.bodyFrame = 3;
            mountData.yOffset = 4;
            mountData.playerHeadOffset = 18;
            mountData.standingFrameCount = 1;
            mountData.standingFrameDelay = 12;
            mountData.standingFrameStart = 0;
            mountData.runningFrameCount = 1;
            mountData.runningFrameDelay = 12;
            mountData.runningFrameStart = 0;
            mountData.flyingFrameCount = 1;
            mountData.flyingFrameDelay = 12;
            mountData.flyingFrameStart = 0;
            mountData.inAirFrameCount = 1;
            mountData.inAirFrameDelay = 12;
            mountData.inAirFrameStart = 0;
            mountData.idleFrameCount = 0;
            mountData.idleFrameDelay = 12;
            mountData.idleFrameStart = 8;
            mountData.swimFrameCount = 0;
            mountData.swimFrameDelay = 12;
            mountData.swimFrameStart = 0;
            if (Main.netMode != 2)
            {
                mountData.backTexture = Main.drillMountTexture[0];
                mountData.backTextureGlow = Main.drillMountTexture[3];
                mountData.backTextureExtra = null;
                mountData.backTextureExtraGlow = null;
                mountData.frontTexture = Main.drillMountTexture[1];
                mountData.frontTextureGlow = Main.drillMountTexture[4];
                mountData.frontTextureExtra = Main.drillMountTexture[2];
                mountData.frontTextureExtraGlow = Main.drillMountTexture[5];
                mountData.textureWidth = mountData.frontTexture.Width;
                mountData.textureHeight = mountData.frontTexture.Height;
            }
            Mount.drillTextureSize = new Vector2(80f, 80f);
            Vector2 value = new Vector2(mountData.textureWidth, mountData.textureHeight / mountData.totalFrames);

        }

        public override void UpdateEffects(Player mountedPlayer)
        {
            mountedPlayer.autoJump = mountedPlayer.mount.AutoJump;
            if (mountedPlayer.ownedProjectileCounts[453] < 1)
            {
                mountedPlayer.mount._abilityActive = false;
            }
        }

        // Since only a single instance of ModMountData ever exists, we can use player.mount._mountSpecificData to store additional data related to a specific mount.
        // Using something like this for gameplay effects would require ModPlayer syncing, but this example is purely visual.

        public override void SetMount(Player player, ref bool skipDust)
        {
            player.mount._mountSpecificData = new DrillMountData();
        }

        public void UpdateDrill(Player mountedPlayer, bool controlUp, bool controlDown)
        {
            //Main.NewText("update drill new mthod");
            DrillMountData drillMountData = (DrillMountData)mountedPlayer.mount._mountSpecificData;
            for (int i = 0; i < drillMountData.beams.Length; i++)
            {
                //Main.NewText("update drill new mthod INSIDE BEAMD LOOP");
                DrillBeam drillBeam = drillMountData.beams[i];
                if (drillBeam.cooldown > 1)
                {
                    drillBeam.cooldown--;
                }
                else if (drillBeam.cooldown == 1)
                {
                    drillBeam.cooldown = 0;
                    drillBeam.curTileTarget = Point16.NegativeOne;
                    drillBeam.curEnemyTarget = null;
                }
            }
            drillMountData.diodeRotation = drillMountData.diodeRotation * 0.85f + 0.15f * drillMountData.diodeRotationTarget; // to slowly turn it?
            if (drillMountData.beamCooldown > 0)
            {
                drillMountData.beamCooldown--;
            }
        }

        float rotationCounter = 0f;
        public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 Position, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
        {

            if (playerDrawData == null)
            {
                return false;
            }
            //Player player = Main.player[Main.myPlayer];
            Player player = drawPlayer;
            DrillPlayer drillPlayer = drawPlayer.GetModPlayer<DrillPlayer>();


            DrillMountData drillMountData = (DrillMountData)player.mount._mountSpecificData;
            rotation = 0f;
            switch (drawType)
            {
                case 0:
                    //Main.NewText("inside case 0");
                    rotation = drillMountData.outerRingRotation - rotation;
                    break;
                case 3:
                    //Main.NewText("inside case 3");

                    if (player.direction < 0) // if facing left, add pi to correct the draw
                    {
                        rotation = MathHelper.Pi;
                    }
                    rotation = drillMountData.diodeRotation - rotation - drawPlayer.fullRotation; // flipping error here?
                    //Main.NewText("diodeRotation: " + drillMountData.diodeRotation + ", " + rotation + ", " + drawPlayer.fullRotation);

                    //rotationCounter = rotationCounter + 0.1f;
                    //rotation = rotationCounter;
                    //drillMountData.diodeRotation = rotation;

                    break;
            }
            //Vector2 drawOrigin = this.Origin;
            drawScale = 1f;
            //SpriteEffects spriteEffects;



            DrawData item;
            item = new DrawData(texture, Position, frame, drawColor, rotation, drawOrigin, drawScale, spriteEffects, 0);
            item.shader = Mount.currentShader;
            playerDrawData.Add(item);
            if (glowTexture != null)
            {
                //Main.NewText("inside??? case");
                item = new DrawData(glowTexture, Position, frame, glowColor * ((float)(int)drawColor.A / 255f), rotation, drawOrigin, drawScale, spriteEffects, 0);
                item.shader = Mount.currentShader;
            }
            playerDrawData.Add(item);


            DrillMountData drillMountData2 = (DrillMountData)player.mount._mountSpecificData;
            Rectangle value = new Rectangle(0, 0, 1, 1);
            Vector2 vector = Mount.drillDiodePoint1.RotatedBy(drillMountData2.diodeRotation); // to draw line from diode
            Vector2 vector2 = Mount.drillDiodePoint2.RotatedBy(drillMountData2.diodeRotation);
            for (int i = 0; i < drillMountData2.beams.Length; i++)
            {
                DrillBeam drillBeam = drillMountData2.beams[i];
                if (!drillPlayer.doEnemies && (drillBeam.curTileTarget == Point16.NegativeOne))
                {
                    continue;
                }
                if (drillPlayer.doEnemies && (drillBeam.curEnemyTarget == null))
                {
                    continue;
                }

                for (int j = 0; j < 2; j++)
                {
                    Vector2 value2;
                    if (drillPlayer.doEnemies)
                    {
                        value2 = drillBeam.curEnemyTarget.position - Main.screenPosition - Position;
                    } else
                    {
                        value2 = new Vector2(drillBeam.curTileTarget.X * 16 + 8, drillBeam.curTileTarget.Y * 16 + 8) - Main.screenPosition - Position;
                    }
                    Vector2 vector3;
                    Color color;
                    if (j == 0)
                    {
                        vector3 = vector;
                        color = (drillPlayer.doEnemies) ? Color.Crimson : Color.CornflowerBlue;

                    }
                    else
                    {
                        vector3 = vector2;
                        color = (drillPlayer.doEnemies) ? Color.Red : Color.LightGreen;
                    }
                    color.A = 128;
                    color *= 0.5f;
                    Vector2 v = value2 - vector3;
                    float num6 = v.ToRotation();
                    float y = v.Length();
                    item = new DrawData(scale: new Vector2(2f, y), texture: Main.magicPixel, position: vector3 + Position, sourceRect: value, color: color, rotation: num6 - (float)Math.PI / 2f, origin: Vector2.Zero, effect: SpriteEffects.None, inactiveLayerDepth: 0);
                    item.ignorePlayerRotation = true;
                    item.shader = Mount.currentShader;
                    playerDrawData.Add(item);
                }
            }
            return false;
        }
        // skip Dismount?

        public override void UseAbility(Player mountedPlayer, Vector2 mousePosition, bool toggleOn)
        {

            //Main.NewText("useability");
            Player player = mountedPlayer;
            Hover(mountedPlayer);
            UpdateDrill(player, player.controlUp, player.controlDown);

            if (!player.controlUseItem) // end if not pressing anything
            {
                return;

            }
            NewAimAbility(mountedPlayer, mousePosition);
            UsedDrill(mountedPlayer);
            if (Main.myPlayer == mountedPlayer.whoAmI)
            {
                if (!toggleOn)
                {
                    player.mount._abilityActive = false;
                }
                else if (!player.mount._abilityActive)
                {
                    if (mountedPlayer.whoAmI == Main.myPlayer)
                    {
                        //Main.NewText("well inside useability");
                        float num = Main.screenPosition.X + (float)Main.mouseX;
                        float num2 = Main.screenPosition.Y + (float)Main.mouseY;
                        Projectile.NewProjectile(ai0: num - mountedPlayer.position.X, ai1: num2 - mountedPlayer.position.Y, X: num, Y: num2, SpeedX: 0f, SpeedY: 0f, Type: 453, Damage: 0, KnockBack: 0f, Owner: mountedPlayer.whoAmI);
                    }
                    player.mount._abilityActive = true;
                }
            }
            else
            {
                player.mount._abilityActive = toggleOn;
            }
            /**
			if (player.controlUseItem)
            {
				Main.NewText("well inside useability");
				float num = Main.screenPosition.X + (float)Main.mouseX;
				float num2 = Main.screenPosition.Y + (float)Main.mouseY;
				Projectile.NewProjectile(ai0: num - mountedPlayer.position.X, ai1: num2 - mountedPlayer.position.Y, X: num, Y: num2, SpeedX: 0f, SpeedY: 0f, Type: 453, Damage: 0, KnockBack: 0f, Owner: mountedPlayer.whoAmI);
			}
			**/
            //base.UseAbility(mountedPlayer, mousePosition, toggleOn);
            //return true;
        }

        public override bool UpdateFrame(Player mountedPlayer, int state, Vector2 velocity)
        {
            /**
			if (!MountLoader.UpdateFrame(mountedPlayer, state, velocity))
			{
				return;
			}
			**/
            //Main.NewText("top of updateframe");
            Player player = mountedPlayer;
            if (player.mount._frameState != state)
            {
                player.mount._frameState = state;
                player.mount._frameCounter = 0f;
            }
            if (state != 0)
            {
                player.mount._idleTime = 0;
            }
            if (player.mount._data.emitsLight)
            {
                Point point = mountedPlayer.Center.ToTileCoordinates();
                Lighting.AddLight(point.X, point.Y, player.mount._data.lightColor.X, player.mount._data.lightColor.Y, player.mount._data.lightColor.Z);
            }
            if (state != 0 && state != 1)
            {
                return false;
            }
            Vector2 position = default(Vector2);
            position.X = mountedPlayer.position.X;
            position.Y = mountedPlayer.position.Y + (float)mountedPlayer.height;
            int num7 = (int)(position.X / 16f);
            _ = position.Y / 16f;
            float num8 = 0f;
            float num9 = mountedPlayer.width;
            while (num9 > 0f)
            {
                float num10 = (float)((num7 + 1) * 16) - position.X;
                if (num10 > num9)
                {
                    num10 = num9;
                }
                num8 += Collision.GetTileRotation(position) * num10;
                num9 -= num10;
                position.X += num10;
                num7++;
            }
            float num11 = num8 / (float)mountedPlayer.width - mountedPlayer.fullRotation;
            float num12 = 0f;
            float num13 = (float)Math.PI / 20f;
            if (num11 < 0f)
            {
                num12 = ((!(num11 > 0f - num13)) ? (0f - num13) : num11);
            }
            else if (num11 > 0f)
            {
                num12 = ((!(num11 < num13)) ? num13 : num11);
            }
            if (num12 != 0f)
            {
                mountedPlayer.fullRotation += num12;
                if (mountedPlayer.fullRotation > (float)Math.PI / 4f)
                {
                    mountedPlayer.fullRotation = (float)Math.PI / 4f;
                }
                if (mountedPlayer.fullRotation < -(float)Math.PI / 4f)
                {
                    mountedPlayer.fullRotation = -(float)Math.PI / 4f;
                }
            }
            //Main.NewText("bottom of updateframe");
            return true;
        }


        public void UsedDrill(Player mountedPlayer)
        {

            //Main.NewText("inside usedDrull");
            Player player = mountedPlayer;
            DrillPlayer drillPlayer = mountedPlayer.GetModPlayer<DrillPlayer>();
            /**
			if (!player.mount._abilityActive)
			{
				return;
			}
			**/
            if (!player.controlUseItem)
            {
                return;
            }

            DrillMountData drillMountData = (DrillMountData)player.mount._mountSpecificData;

            //drillMountData.beamCooldown = 0; // added

            if (drillMountData.beamCooldown != 0)
            {
                //Main.NewText("got f'ed at beamcooldown num1");
                return;
            }
            for (int i = 0; i < drillMountData.beams.Length; i++)
            {
                if (drillPlayer.doEnemies)
                {
                    //Main.NewText("Inside usedrill");
                    DrillBeam drillBeam = drillMountData.beams[i];
                    if (drillBeam.cooldown != 0)
                    {
                        //Main.NewText("got f'ed at beamcooldown num2");
                        continue;
                    }
                    NPC nearest = DrillSmartCursorEnemy(mountedPlayer, drillMountData);
                    //NPC nearest = FindNearest(Main.MouseWorld, null, mountedPlayer, maxLengthBeam);
                    if (nearest == null)
                    {
                        break;
                    }
                    drillBeam.curEnemyTarget = nearest;
                    //Main.NewText(nearest.ToString());
                    Vector2 vector = nearest.position;
                    nearest.StrikeNPC(100, 5, 0);

                    
                    float num = (vector - mountedPlayer.Center).ToRotation();
                    for (int j = 0; j < 2; j++)
                    {
                        float num2 = num + ((Main.rand.Next(2) == 1) ? (-1f) : 1f) * ((float)Math.PI / 2f);
                        float num3 = (float)Main.rand.NextDouble() * 2f + 2f;
                        Vector2 vector2 = new Vector2((float)Math.Cos(num2) * num3, (float)Math.Sin(num2) * num3);
                        int num4 = Dust.NewDust(vector, 0, 0, 230, vector2.X, vector2.Y);
                        Main.dust[num4].noGravity = true;
                        Main.dust[num4].customData = mountedPlayer;
                    }
                    drillBeam.cooldown = Mount.drillPickTime;
                }
                else
                {
                    DrillBeam drillBeam = drillMountData.beams[i];
                    if (drillBeam.cooldown != 0)
                    {
                        //Main.NewText("got f'ed at beamcooldown num2");
                        continue;
                    }
                    Point16 point = DrillSmartCursor(mountedPlayer, drillMountData);
                    if (!(point != Point16.NegativeOne))
                    {
                        break;
                    }
                    drillBeam.curTileTarget = point;
                    //int pickPower = Mount.drillPickPower;
                    int pickPower = 600;
                    bool flag = mountedPlayer.whoAmI == Main.myPlayer;
                    if (flag)
                    {
                        //Main.NewText("should be in boys");
                        bool flag2 = true;
                        if (WorldGen.InWorld(point.X, point.Y) && Main.tile[point.X, point.Y] != null && Main.tile[point.X, point.Y].type == 26 && !Main.hardMode)
                        {
                            flag2 = false;
                            mountedPlayer.Hurt(PlayerDeathReason.ByOther(4), mountedPlayer.statLife / 2, -mountedPlayer.direction);
                        }
                        if (mountedPlayer.noBuilding)
                        {
                            flag2 = false;
                        }
                        if (flag2)
                        {
                            // WHERE THE MAGIC HAPPENS
                            //Main.NewText("GET PICKED");
                            if (drillPlayer.doWalls)
                            {
                                WorldGen.KillWall(point.X, point.Y, false);
                            }
                            else
                            {
                                mountedPlayer.PickTile(point.X, point.Y, pickPower);
                            }
                        }
                    }
                    Vector2 vector = new Vector2((float)(point.X << 4) + 8f, (float)(point.Y << 4) + 8f);
                    float num = (vector - mountedPlayer.Center).ToRotation();
                    for (int j = 0; j < 2; j++)
                    {
                        float num2 = num + ((Main.rand.Next(2) == 1) ? (-1f) : 1f) * ((float)Math.PI / 2f);
                        float num3 = (float)Main.rand.NextDouble() * 2f + 2f;
                        Vector2 vector2 = new Vector2((float)Math.Cos(num2) * num3, (float)Math.Sin(num2) * num3);
                        int num4 = Dust.NewDust(vector, 0, 0, 230, vector2.X, vector2.Y);
                        Main.dust[num4].noGravity = true;
                        Main.dust[num4].customData = mountedPlayer;
                    }
                    if (flag)
                    {
                        Tile.SmoothSlope(point.X, point.Y);
                    }
                    drillBeam.cooldown = Mount.drillPickTime;
                    //drillBeam.cooldown = 0; //added
                    break;
                }
            }
            drillMountData.beamCooldown = Mount.drillBeamCooldownMax;
        }

        public void NewAimAbility(Player mountedPlayer, Vector2 mousePosition1)
        {
            Vector2 mousePosition = Main.MouseWorld;
            Vector2 v = ClampToDeadZone(mountedPlayer, mousePosition) - mountedPlayer.Center;
            DrillMountData drillMountData = (DrillMountData)mountedPlayer.mount._mountSpecificData;
            //drillMountData.diodeRotationTarget = v.ToRotation();

            float num = v.ToRotation();
            if (num < 0f)
            {
                num += (float)Math.PI * 2f;
            }
            drillMountData.diodeRotationTarget = num;
            //Main.NewText("Aim Ability Old Rotation" + num);
            float num2 = drillMountData.diodeRotation % ((float)Math.PI * 2f);
            if (num2 < 0f)
            {
                num2 += (float)Math.PI * 2f;
            }
            if (num2 < num)
            {
                if (num - num2 > (float)Math.PI)
                {
                    num2 += (float)Math.PI * 2f;
                }
            }
            else if (num2 - num > (float)Math.PI)
            {
                num2 -= (float)Math.PI * 2f;
            }
            drillMountData.diodeRotation = num2;
            drillMountData.crosshairPosition = mousePosition;
            //Main.NewText("Aim Ability Rotation" + drillMountData.diodeRotation);
        }

        private Vector2 ClampToDeadZone(Player mountedPlayer, Vector2 position)
        {
            int num;
            int num2;

            num = (int)Mount.drillTextureSize.Y;
            num2 = (int)Mount.drillTextureSize.X;

            Vector2 center = mountedPlayer.Center;
            position -= center;
            if (position.X > (float)(-num2) && position.X < (float)num2 && position.Y > (float)(-num) && position.Y < (float)num)
            {
                float num3 = (float)num2 / Math.Abs(position.X);
                float num4 = (float)num / Math.Abs(position.Y);
                if (num3 > num4)
                {
                    position *= num4;
                }
                else
                {
                    position *= num3;
                }
            }
            return position + center;
        }


        public bool Hover(Player mountedPlayer)
        {
            //Main.NewText("top Hover");
            DrillMountData drillMountData = (DrillMountData)mountedPlayer.mount._mountSpecificData;
            if (mountedPlayer.velocity.Y == 0f)
            {
                mountedPlayer.velocity.Y = 0.001f;
            }
            //if (true) // added
            if (mountedPlayer.mount._frameState == 2 || mountedPlayer.mount._frameState == 4)
            {
                //Main.NewText("top Hover in frame check");
                bool flag = true;
                float num = 1f;
                float num2 = mountedPlayer.gravity / Player.defaultGravity;
                if (mountedPlayer.slowFall)
                {
                    num2 /= 3f;
                }
                if (num2 < 0.25f)
                {
                    num2 = 0.25f;
                }
                //float num3 = mountedPlayer.mount._fatigue / mountedPlayer.mount._fatigueMax;
                float num3 = 0f;
                float num4 = 4f * num3;
                float num5 = 4f * num3;
                if (num4 == 0f)
                {
                    num4 = -0.001f;
                }
                if (num5 == 0f)
                {
                    num5 = -0.001f;
                }
                float currYVel = mountedPlayer.velocity.Y;
                float oldYVel = currYVel;
                if ((mountedPlayer.controlUp || mountedPlayer.controlJump) && flag)
                {

                    num4 = -8f;
                    currYVel -= mountedPlayer.mount._data.acceleration * num;
                    //Main.NewText("Go Up!!!: " + currYVel);
                    //Main.NewText("mount accel!!!: " + mountedPlayer.mount._data.acceleration);
                }
                else if (mountedPlayer.controlDown)
                {
                    currYVel += mountedPlayer.mount._data.acceleration * num;
                    num5 = 8f;
                    //Main.NewText("Go Down!!!: " + currYVel);
                    //Main.NewText("mount accel!!!: " + mountedPlayer.mount._data.acceleration);
                }
                else
                {
                    //currYVel *= 0.97f; // fadded
                    _ = mountedPlayer.jump;
                }
                if (currYVel < num4)
                {
                    currYVel = ((!(num4 - currYVel < mountedPlayer.mount._data.acceleration)) ? (currYVel + mountedPlayer.mount._data.acceleration * num) : num4);

                    //currYVel = -8f;
                }
                else if (currYVel > num5)
                {
                    currYVel = ((!(currYVel - num5 < mountedPlayer.mount._data.acceleration)) ? (currYVel - mountedPlayer.mount._data.acceleration * num) : num5);

                    //currYVel = 8f;
                }
                mountedPlayer.velocity.Y = currYVel;
                //Main.NewText("YVel Before: " + oldYVel + " After: " + currYVel);
                mountedPlayer.fallStart = (int)(mountedPlayer.position.Y / 16f);
            }
            else if (mountedPlayer.velocity.Y == 0f)
            {
                mountedPlayer.velocity.Y = 0.001f;
            }
            //else
            if (true) // added
            {
                float num9 = mountedPlayer.velocity.X / mountedPlayer.mount._data.dashSpeed;
                if ((double)num9 > 0.95)
                {
                    num9 = 0.95f;
                }
                if ((double)num9 < -0.95)
                {
                    num9 = -0.95f;
                }
                mountedPlayer.fullRotation = (float)Math.PI / 4f * num9 / 2f;
                DrillMountData obj = (DrillMountData)mountedPlayer.mount._mountSpecificData;
                float outerRingRotation = obj.outerRingRotation;
                outerRingRotation += mountedPlayer.velocity.X / 80f;
                if (outerRingRotation > (float)Math.PI)
                {
                    outerRingRotation -= (float)Math.PI * 2f;
                }
                else if (outerRingRotation < -(float)Math.PI)
                {
                    outerRingRotation += (float)Math.PI * 2f;
                }
                obj.outerRingRotation = outerRingRotation;
            }
            //Main.NewText("bottom Hover");
            return true;
        }

        private Point16 DrillSmartCursor(Player mountedPlayer, DrillMountData data)
        {
            //Main.NewText("DrillSmartCursor");
            DrillPlayer drillPlayer = mountedPlayer.GetModPlayer<DrillPlayer>();

            Vector2 value = ((mountedPlayer.whoAmI != Main.myPlayer) ? data.crosshairPosition : (Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY)));
            Vector2 center = mountedPlayer.Center;
            Vector2 value2 = value - center;
            float num = value2.Length();

            //value below is max length;
            if (num > maxLengthBeam)
            {
                num = maxLengthBeam;
            }
            num += 32f;
            value2.Normalize();
            Vector2 end = center + value2 * num;
            Point16 tilePoint = new Point16(-1, -1);

            // the 65 below is the radius in which it can detect blocks (cone around you)
            if (!Utils.PlotTileLine(center, end, maxRadius, delegate (int x, int y)
            {
                tilePoint = new Point16(x, y);
                for (int i = 0; i < data.beams.Length; i++)
                {
                    if (data.beams[i].curTileTarget == tilePoint) // make sure two beams dont target the same thing
                    {
                        return true;
                    }
                }
                if (drillPlayer.doWalls) // if walls
                {
                    return (Main.tile[x, y].wall == 0) ? true : false;

                }
                else // if tiles
                {
                    if (!WorldGen.CanKillTile(x, y))
                    {
                        return true;
                    }
                    return (Main.tile[x, y] == null || Main.tile[x, y].inActive() || !Main.tile[x, y].active()) ? true : false;
                }
            }))
            {
                return tilePoint;
            }
            return new Point16(-1, -1);
        }

        private NPC DrillSmartCursorEnemy(Player mountedPlayer, DrillMountData data)
        {
            //Main.NewText("DrillSmartCursor");
            DrillPlayer drillPlayer = mountedPlayer.GetModPlayer<DrillPlayer>();

            Vector2 value = ((mountedPlayer.whoAmI != Main.myPlayer) ? data.crosshairPosition : (Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY)));
            Vector2 center = mountedPlayer.Center;
            Vector2 value2 = value - center;
            float num = value2.Length();

            //value below is max length;
            if (num > maxLengthAttackBeam)
            {
                num = maxLengthAttackBeam;
            }
            num += 32f;
            value2.Normalize();
            Vector2 end = center + value2 * num;
            Point16 tilePoint = new Point16(-1, -1);
            NPC nearest = null;

            if (!Utils.PlotTileLine(center, end, maxRadius, delegate (int x, int y)
            {

                tilePoint = new Point16(x, y);

                for (int i = 0; i < data.beams.Length; i++)
                {
                    if (data.beams[i].curTileTarget == tilePoint)
                    {
                        return true;
                    }
                }
                nearest = FindNearest(tilePoint.ToWorldCoordinates(), null, mountedPlayer, 16f);
                if (nearest != null)
                {
                    //Main.NewText("we are in nearest not null");
                    return false;
                }
                return true;
            }))
            {
                return nearest;
            }
            return null;

        }

        private NPC FindNearest(Vector2 pos, NPC avoid, Player mountedPlayer, float max) // avoid is one you already hit
        {

            NPC nearest = null;
            float oldDist = 1001;
            float newDist = 1000;
           
            if (Terraria.Main.npc == null)
            {
                return null;
            }
            for (int i = 0; i < Terraria.Main.npc.Length - 1; i++) //Do once for each NPC in the world
            {
                if (Terraria.Main.npc[i] == avoid)//Don't target the one you want to avoid
                    continue;
                if (Terraria.Main.npc[i].friendly == true)//Don't target town NPCs
                    continue;
                if (Terraria.Main.npc[i].active == false)//Don't target dead NPCs
                    continue;
                if (Terraria.Main.npc[i].damage == 0)//Don't target non-aggressive NPCs
                    continue;
                if (!Collision.CanHit(pos, mountedPlayer.width, mountedPlayer.height, Main.npc[i].position, Main.npc[i].width, Main.npc[i].height))
                {
                    continue;
                }
                if (nearest == null) //if no NPCs have made it past the previous few checks, and its below max
                    nearest = Terraria.Main.npc[i]; //become the nearest NPC 
                else
                {
                    oldDist = Vector2.Distance(pos, nearest.position);//Check the distance to the nearest NPC that's earlier in the loop
                    newDist = Vector2.Distance(pos, Terraria.Main.npc[i].position);//Check the distance to the current NPC in the loop
                    if (newDist < oldDist)//If closer than the previous NPC in the loop
                        nearest = Terraria.Main.npc[i];//Become the nearest NPC
                }
            }
            return (Vector2.Distance(pos, nearest.position) < max) ? nearest : null;
           // return nearest; //return the npc that is nearest to the vector 'pos'
        }


    }

    public class DrillPlayer : ModPlayer
    {
        int MAX_TOGGLE_TIMER = 15; // how long before you can toggle it again
        int toggleTime = 0;
        public bool doWalls = false;
        public bool doEnemies = false;
        public override void PreUpdate()
        {
            if (player.mount._type == mod.MountType("DrillContain"))
            {
                player.gravity = 0;
                
                
                if (toggleTime == 0)
                {
                    if (player.controlQuickHeal)
                    {
                        if (!doEnemies)
                        {
                            doEnemies = true;
                            Main.NewText("Attack enemies enabled.");
                        }
                        else
                        {
                            doEnemies = false;
                            Main.NewText("Attack enemies .");
                        }
                        toggleTime = MAX_TOGGLE_TIMER;
                    }
                }
                /**
                if (player.controlQuickHeal)
                {
                    if (!doEnemies)
                    {
                        doEnemies = true;
                        Main.NewText("Attack enemies enabled.");
                    }
                    else
                    {
                        doEnemies = false;
                        Main.NewText("Attack enemies disabled.");
                    }
                    toggleTime = MAX_TOGGLE_TIMER;
                }
                **/

                player.noItems = true;

                //if (player.releaseUseItem)
                if (player.controlUseItem)
                {
                    //player.mount.UseAbility(player, Vector2.Zero, toggleOn: true);

                }
                player.mount.UseAbility(player, Vector2.Zero, toggleOn: true);


                player.releaseUseItem = false;


            }

            if (toggleTime > 0)
            {
                toggleTime--;
            }
            base.PreUpdate();
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (BasicMod.WallToggle.JustPressed)
            {
                //Main.NewText("inside walltoggle");
                if (toggleTime == 0)
                {
                    if (!doWalls)
                    {
                        doWalls = true;
                    }
                    else
                    {
                        doWalls = false;
                    }
                    toggleTime = MAX_TOGGLE_TIMER;
                }
            }
        }

    }
}