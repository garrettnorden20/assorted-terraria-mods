using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BasicMod.NPCs.SwatInvasion
{
    public class SwatCruiseMissile : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Swat Missile");
        }

        public override void SetDefaults()
        {
            npc.width = 16;
            npc.height = 16;
            npc.alpha = 255;
            npc.damage = 90;
            npc.defense = 20;
            npc.lifeMax = 500;

            npc.noGravity = true;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.value = 6000f;
            npc.knockBackResist = 0.5f;
            npc.aiStyle = -1;
            //aiType = 86;
            //animationType = 3;
            npc.buffImmune[BuffID.Confused] = false;
            //banner = npc.type;
            //bannerItem = mod.ItemType("VelocichopperBanner");
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
			if (BasicWorld.SwatEvent)
			{
				return 11f;
			}
			else
			{
			    return 0;
			}

        }


        public int AI_Timer = 0;
        public int numTurns = 0;
        float trueVel = 5f;

        public override void AI()
        {

            //npc.timeLeft = 10000;

            //Main.NewText("In AI" + AI_Timer);
            Player player = Main.player[npc.target];
            npc.TargetClosest(true);

            if (AI_Timer == 1)
            {
                npc.rotation = MathHelper.Pi;
                //Main.NewText("In AI 0 Method" + AI_Timer);
                npc.position = player.position - new Vector2(Main.rand.NextFloat(-1000f, 1000f), 1000f);
                npc.alpha = 0;
                npc.velocity = new Vector2(0, trueVel);
            }

            float distance = Vector2.Distance(npc.position, player.position);


            if ((AI_Timer == 2 || AI_Timer % 5 == 0) && player.position.Y - 100f > npc.position.Y && distance < 500f && numTurns <= 5) // player is below
            {
                Vector2 velocity = Vector2.Normalize((new Vector2(player.Center.X, player.Center.Y) - npc.position)) * trueVel;
                npc.velocity = velocity;
                npc.rotation = velocity.ToRotation() + MathHelper.PiOver2;
                numTurns++;
            }

            //if (AI_Timer >  2 && npc.velocity.Length() < 0.5f)
            if (AI_Timer > 2 && (Collision.TileCollision(npc.position, npc.velocity, npc.width, npc.height) == Vector2.Zero || npc.velocity.Y < 0.1f))
            {
                //Collision.
                Main.NewText("Col" + AI_Timer);
                Explode(player, npc.damage, false);
            }


            if (AI_Timer > 2 && AI_Timer % 5 == 0)
            {
                CreateDust();
            }

            AI_Timer++;


            /**
            if (AI_Timer > 481)
            {
                bombTimer++;
                npc.direction = rushDirection;
                npc.velocity = new Vector2(10 * rushDirection, 0f);
                if ((npc.Center.X > player.Center.X + 1200 && rushDirection == 1) || (npc.Center.X < player.Center.X - 1200 && rushDirection == -1) && Main.netMode != 1)
                {
                    AI_Timer = 0;
                    npc.netUpdate = true;
                }
                if (bombTimer > bombReload && Main.netMode != 1)
                {
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0, 0, mod.ProjectileType("SwatBomb"), damage, 3f, Main.myPlayer);
                    bombTimer = 0;
                }
            }
            else if (AI_Timer > 480)
            {
                if (npc.Center.X < player.Center.X)
                {
                    rushDirection = 1;
                }
                else
                {
                    rushDirection = -1;
                }
            }
            else if (AI_Timer > 420)
            {
                Vector2 moveTo = new Vector2(player.Center.X - (900f * npc.direction), player.Center.Y + -300f) - npc.Center;
                npc.velocity = (moveTo) * .03f;
            }
            else if (AI_Timer > attackTime)
            {
                npc.velocity = new Vector2(0, 0f);

                Reload_Timer++;
                if (Reload_Timer > reloadTime && Main.netMode != 1)
                {
                    int Xvar = Main.rand.Next(-50, 50);

                    int Yvar = 50 - Xvar;

                    Projectile.NewProjectile(npc.Center.X + (100f * npc.direction), npc.Center.Y, 5.00f * (1 + Xvar * .01f) * npc.direction, 5.00f * (1 + Yvar * .01f), 110, damage, 3f, Main.myPlayer);

                    Reload_Timer = 0;
                }
            }
            else if (AI_Timer > attackTime - 120)
            {
                npc.velocity = new Vector2(0, 0f);
            }
            else
            {
                Vector2 moveTo = new Vector2(player.Center.X - (300f * npc.direction), player.Center.Y + -300f) - npc.Center;
                npc.velocity = (moveTo) * .03f;
            }
            **/
        }


        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            Explode(target, damage, crit);
        }


        private void Explode(Player target, int damage, bool crit)
        {
            Player player = Main.player[npc.target];
            npc.width = 150;
            npc.height = 150;
            npc.position.X = npc.position.X - (float)(npc.width / 2);
            npc.position.Y = npc.position.Y - (float)(npc.height / 2);
            Projectile.NewProjectile(npc.Center, new Vector2(0, 0), mod.ProjectileType("SwatBombExplosion"), npc.damage, 2, player.whoAmI);
            Main.PlaySound(SoundID.Item62, npc.position);
            for (int i = 0; i < 100; i++)
            {
                int dustIndex = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 31, 0f, 0f, 100, default(Color), 2f);
                Main.dust[dustIndex].velocity *= 1.4f;
            }
            // Fire Dust spawn
            for (int i = 0; i < 160; i++)
            {
                int dustIndex = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 6, 0f, 0f, 100, default(Color), 3f);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].velocity *= 5f;
                dustIndex = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 6, 0f, 0f, 100, default(Color), 2f);
                Main.dust[dustIndex].velocity *= 3f;
            }
            npc.active = false;
        }

        private void CreateDust()
        {
            for (int n = 0; n < 1; n++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 31);

            }

        }
    }

}