using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.World.Generation;
using System.Collections;
using System;

namespace BasicMod
{

	public class ModPlayerGroundPound : ModPlayer
	{
        //These indicate what direction is what in the timer arrays used
        public static readonly int DashDown = 0;

        //The direction the player is currently dashing towards.  Defaults to -1 if no dash is ocurring.
        public int DashDir = -1;

        //The fields related to the dash accessory
        public bool DashActive = false;
        
		public int DashDelay = MAX_DASH_DELAY;
        public int DashTimer = MAX_DASH_TIMER;
        //The initial velocity.  10 velocity is about 37.5 tiles/second or 50 mph
        public readonly float DashVelocity = 25f;
		public readonly float MaxFallSpeed = 50f;
		//These two fields are the max values for the delay between dashes and the length of the dash in that order
		//The time is measured in frames
		
		public static readonly int MAX_DASH_DELAY = 60; // useless for now
		public static readonly int MAX_DASH_TIMER = 50; // cap at how much time you can spend airborne 


        public override void PreUpdateMovement()
        {
			ModPlayerGroundPound mp = player.GetModPlayer<ModPlayerGroundPound>();

			//If the dash is not active, immediately return so we don't do any of the logic for it
			if (!mp.DashActive)
				return;

			for (int num17 = 0; num17 < 2; num17++)
			{
				int num18 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 31, 0f, 0f, 100, default(Color), 2f);
				Main.dust[num18].position.X += Main.rand.Next(-5, 6);
				Main.dust[num18].position.Y += Main.rand.Next(-5, 6);
				Main.dust[num18].velocity *= 0.2f;
				Main.dust[num18].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
			}

			Main.NewText("dash active" + player.velocity.Y);

			//This is where we set the afterimage effect.  You can replace these two lines with whatever you want to happen during the dash
			//Some examples include:  spawning dust where the player is, adding buffs, making the player immune, etc.
			//Here we take advantage of "player.eocDash" and "player.armorEffectDrawShadowEOCShield" to get the Shield of Cthulhu's afterimage effect
			player.eocDash = mp.DashTimer;
			player.armorEffectDrawShadowEOCShield = true;
			player.maxFallSpeed = MaxFallSpeed;
			player.noKnockback = true;
			player.noFallDmg = true;

			player.immuneTime = mp.DashTimer;

			Vector2 newVelocity = player.velocity;
			
			//If the dash has just started, apply the dash velocity in whatever direction we wanted to dash towards
			if (mp.DashTimer == ModPlayerGroundPound.MAX_DASH_TIMER)
			{

				if ((mp.DashDir == ModPlayerGroundPound.DashDown && player.velocity.Y < mp.DashVelocity))
				{
					newVelocity.Y = mp.DashVelocity;
				}

				player.velocity = newVelocity;
			}

			

			//Decrement the timers
			mp.DashTimer--;
			mp.DashDelay--;

			if (player.velocity.Y < 0.5f || mp.DashTimer == 0)
			{
				if (player.velocity.Y < 0.5f)
                {
					groundPoundDamage();
				}
				Main.NewText("=ENDED");
				//The dash has ended.  Reset the fields
				//mp.DashDelay = ModPlayerGroundPound.MAX_DASH_DELAY;
				mp.DashTimer = ModPlayerGroundPound.MAX_DASH_TIMER;
				mp.DashActive = false;
				player.eocDash = 0;
				player.armorEffectDrawShadowEOCShield = false;
			}

			// add the constant speed effect
			newVelocity = player.velocity;
			if ((mp.DashDir == ModPlayerGroundPound.DashDown && player.velocity.Y < mp.DashVelocity))
			{
				newVelocity.Y = mp.DashVelocity;
			}

			player.velocity = newVelocity;
		}

        public override void ResetEffects()
        {
			if (player.mount.Active || DashActive || player.velocity.Y == 0 || player.gravControl)
                return;

            if (player.controlDown && player.releaseDown && player.doubleTapCardinalTimer[DashDown] < 15)
                DashDir = DashDown;
            else
                return;  //No dash was activated, return

            DashActive = true;

            Main.NewText("dash acvitavted!!!!");
			//Here you'd be able to set an effect that happens when the dash first activates
			//Some examples include:  the larger smoke effect from the Master Ninja Gear and Tabi
			for (int num17 = 0; num17 < 20; num17++)
			{
				int num18 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 31, 0f, 0f, 100, default(Color), 2f);
				Main.dust[num18].position.X += Main.rand.Next(-5, 6);
				Main.dust[num18].position.Y += Main.rand.Next(-5, 6);
				Main.dust[num18].velocity *= 0.2f;
				Main.dust[num18].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
			}
			int num19 = Gore.NewGore(new Vector2(player.position.X + (float)(player.width / 2) - 24f, player.position.Y + (float)(player.height / 2) - 34f), default(Vector2), Main.rand.Next(61, 64));
			Main.gore[num19].velocity.X = (float)Main.rand.Next(-50, 51) * 0.01f;
			Main.gore[num19].velocity.Y = (float)Main.rand.Next(-50, 51) * 0.01f;
			Main.gore[num19].velocity *= 0.4f;
			num19 = Gore.NewGore(new Vector2(player.position.X + (float)(player.width / 2) - 24f, player.position.Y + (float)(player.height / 2) - 14f), default(Vector2), Main.rand.Next(61, 64));
			Main.gore[num19].velocity.X = (float)Main.rand.Next(-50, 51) * 0.01f;
			Main.gore[num19].velocity.Y = (float)Main.rand.Next(-50, 51) * 0.01f;
			Main.gore[num19].velocity *= 0.4f;
		}


        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            if (DashActive)
            {
				damage = 0;
            }
        }



        float hitboxWidth = 150.0f;
		float hitboxHeight = 150.0f;
		private void groundPoundDamage()
		{
			Main.PlaySound(SoundID.Item56, (int)player.position.X, (int)player.position.Y);

			Rectangle rectangle = new Rectangle((int)((double)player.position.X + (double)player.velocity.X * 0.5 - hitboxWidth/2), (int)((double)player.position.Y + (double)player.velocity.Y * 0.5 - hitboxHeight/2), player.width + (int)hitboxWidth, player.height + (int)hitboxHeight);
			for (int i = 0; i < 200; i++)
			{
				if (Main.rand.NextBool())
					Dust.NewDust(rectangle.TopLeft() + new Vector2(0, (float)(rectangle.Height * 0.6)), rectangle.Width, (int)(rectangle.Height * 0.1), DustID.Smoke, 0, -3f);

				if (!Main.npc[i].active || Main.npc[i].dontTakeDamage || Main.npc[i].friendly)
				{
					continue;
				}
				NPC nPC = Main.npc[i];
				Rectangle rect = nPC.getRect();
				if (rectangle.Intersects(rect) && (nPC.noTileCollide || player.CanHit(nPC)))
				{
					float damage = 30f * player.meleeDamage;
					float knockback = 9f;
					bool crit = false;
					if (player.kbGlove)
					{
						knockback *= 2f;
					}
					if (player.kbBuff)
					{
						knockback *= 1.5f;
					}
					if (Main.rand.Next(100) < player.meleeCrit)
					{
						crit = true;
					}
					int dir = player.direction;
					if (player.Center.X < nPC.Center.X)
					{
						dir = 1;
					}
					if (player.Center.X > nPC.Center.X)
					{
						dir = -1;
					}
					if (player.whoAmI == Main.myPlayer)
					{
						player.ApplyDamageToNPC(nPC, (int)damage, knockback, dir, crit);
					}
					//player.eocDash = 10;
					//player.dashDelay = 30;
					//player.velocity.X = -num3 * 9;
					//player.velocity.Y = -4f;
					player.immune = true;
					player.immuneNoBlink = true;
					player.immuneTime = 16;
					//player.eocHit = i;
				}
			}
		}
    }
}