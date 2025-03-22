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
using BasicMod;

namespace BasicMod
{
	// This file shows the very basics of using ModPlayer classes since BasicModPlayer can be a bit overwhelming.
	// ModPlayer classes provide a way to attach data to Players and act on that data. 
	// This example will hopefully provide you with an understanding of the basic building blocks of how ModPlayer works. 
	// This example will teach the most commonly sought after effect: "How to do X if the player has Y?"
	// X in this example will be "Apply a debuff to enemies."
	// Y in this example will be "Wearing an accessory."
	// After studying this example, you can change X to other effects by changing the "hook" you use or the code within the hook you use. For example, you could use OnHitByNPC and call Projectile.NewProjectile within that hook to change X to "When the player is hit by NPC, spawn Projectiles".
	// We can change Y to other conditions as well. For example, you could give the player the effect by having a "potion" ModItem give a ModBuff that sets the ModPlayer variable in ModBuff.Update
	// Another example would be an armor set effect. Simply use the ModItem.UpdateArmorSet hook 

	public class BasicModPlayer : ModPlayer
	{
		public bool boomerangMitt;
		public bool fuzzyMittens;
		public bool woolenGlove;
		public bool ammoStash;
		public bool rageBand;
		public bool rageBandNextHit;
		public float enemiesKilled;


		// Here we declare the frostBurnSummon variable which will represent whether this player has the effect or not.

		// ResetEffects is used to reset effects back to their default value. Terraria resets all effects every frame back to defaults so we will follow this design. (You might think to set a variable when an item is equipped and unassign the value when the item in unequipped, but Terraria is not designed that way.)
		public override void ResetEffects()
		{
			//FrostBurnSummon = false;
			boomerangMitt = false;
			fuzzyMittens = false;
			woolenGlove = false;
			ammoStash = false;
			rageBand = false;

			SoulSeekerCaps();
		}

		public override void clientClone(ModPlayer clientClone)
		{
			BasicModPlayer clone = clientClone as BasicModPlayer;
			// Here we would make a backup clone of values that are only correct on the local players Player instance.
			// Some examples would be RPG stats from a GUI, Hotkey states, and Extra Item Slots
			clone.enemiesKilled = enemiesKilled;
		}

		public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
		{
			ModPacket packet = mod.GetPacket();
			packet.Write((byte)BasicModMessageType.BasicModPlayerSyncPlayer);
			packet.Write((byte)player.whoAmI);
			packet.Write(enemiesKilled);
			packet.Send(toWho, fromWho);
		}

		public override void SendClientChanges(ModPlayer clientPlayer)
		{
			// Here we would sync something like an RPG stat whenever the player changes it.
			BasicModPlayer clone = clientPlayer as BasicModPlayer;
			if (clone.enemiesKilled != enemiesKilled)
			{
				// Send a Mod Packet with the changes.
				var packet = mod.GetPacket();
				packet.Write((byte)BasicModMessageType.EnemiesKilled);
				packet.Write((byte)player.whoAmI);
				packet.Write(enemiesKilled);
				packet.Send();
			}
		}

		public override TagCompound Save()
		{
			// Read https://github.com/tModLoader/tModLoader/wiki/Saving-and-loading-using-TagCompound to better understand Saving and Loading data.
			return new TagCompound {
				// {"somethingelse", somethingelse}, // To save more data, add additional lines
				{"enemiesKilled", enemiesKilled},
			};
			//note that C# 6.0 supports indexer initializers
			//return new TagCompound {
			//	["score"] = score
			//};
		}

		public override void Load(TagCompound tag)
		{
			enemiesKilled = tag.GetFloat("enemiesKilled");
			// nonStopParty was added after the initial ExampleMod release. Read https://github.com/tModLoader/tModLoader/wiki/Saving-and-loading-using-TagCompound#mod-version-updates for information about how to handle version updates in your mod without messing up current users of your mod.
		}

		public override void LoadLegacy(BinaryReader reader)
		{
			int loadVersion = reader.ReadInt32();
		}

		// Here we use a "hook" to actually let our frostBurnSummon status take effect. This hook is called anytime a player owned projectile hits an enemy. 
		public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
		{

			/**
			// frostBurnSummon, as its name suggests, applies frostBurn to enemy NPC but only for Summon projectiles.
			// In this if statement we check several conditions. We first check to make sure the projectile that hit the NPC is either a minion projectile or a projectile that minions shoot.
			// We then check that frostBurnSummon is set to true. The last check for not noEnchantments is because some projectiles don't allow enchantments and we want to honor that restriction.
			if ((proj.minion || ProjectileID.Sets.MinionShot[proj.type]) && FrostBurnSummon && !proj.noEnchantments)
			{
				// If all those checks pass, we apply FrostBurn for some random duration.
				target.AddBuff(BuffID.Frostburn, 60 * Main.rand.Next(5, 15), false);
			}
			**/
		}

        public override void Hurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit)
        {
            if (rageBand)
            {
				rageBandNextHit = true; // sets up next attack to do more damage
            }
        }

        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
           if (rageBandNextHit)
            {
				damage = (int)(damage * 1.5);
				knockback = knockback * 1.5f;
				rageBandNextHit = false;
            }
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			if (rageBandNextHit)
			{
				if (proj.melee)
                {
					damage = (int)(damage * 1.5);
				}
				rageBandNextHit = false;
			}
		}


        public override bool Shoot(Item item, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			Vector2 totalVel;
			if (fuzzyMittens)
            {
				// TODO: Maybe need online server check?
				totalVel = new Vector2(speedX, speedY);
				totalVel = Utils.RotatedByRandom(totalVel, MathHelper.PiOver4);
				Projectile.NewProjectile(position, totalVel, ProjectileID.SnowBallFriendly, (int)(10f * player.rangedDamageMult), 1, player.whoAmI);
            }
			if (boomerangMitt)
			{
				if (BasicWorld.boomerangIds.Contains(item.type))
				{
					damage = (int)(damage * 1.25f);
					totalVel = new Vector2(speedX, speedY);
					totalVel = Utils.RotatedByRandom(totalVel, MathHelper.PiOver4);
					Projectile.NewProjectile(position, totalVel, type, damage, knockBack, player.whoAmI);
				}
			}
			return true;
        }

        public override bool ConsumeAmmo(Item weapon, Item ammo)
        {
			if (ammoStash)
			{
				if (Main.rand.Next(5) == 0) // 20% to return false, 
				{
					return false;
                }
			}
			return true;
		}

        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
			if (woolenGlove)
            {
				if (Main.rand.Next(3) == 0)
                {
					target.AddBuff(BuffID.Frostburn, 60 * Main.rand.Next(2, 4), false);
				}
			}
        }

		float curRot = 0;
		public override void DrawEffects(PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
		{
			if (rageBandNextHit)
            {
				Dust dust = Dust.NewDustDirect(player.Center, 0, 0, ModContent.DustType<Dusts.SpiralDust>(), Scale: 1);
				dust.rotation = Main.rand.NextFloat(6.28f);
				dust.customData = player;
				dust.position = player.Center + Vector2.UnitX.RotatedBy(dust.rotation, Vector2.Zero) * dust.scale * 10;
			}





			if (player.statMana != player.statManaMax) // only show stars if less than full mana
            {
				float dist = 70; // distance from player
				float innerRotationMult = 2f; // higher means higher individual star rotation speed

				Texture2D texture = mod.GetTexture("Star");
			
				curRot = curRot + 0.6f; // higher number, higher rotating speed

				int playerStars = player.statMana / 20;
				if (player.statMana % 20 != 0) // correct for truncated mana
				{
					playerStars++;
				}
				for (int k = 0; k < playerStars; k++)
				{
					float rot = MathHelper.ToRadians(curRot + k * (360f / 10)); // or do playerstars

					float x = ((float)(player.Center.X + dist * Math.Cos(rot)));
					float y = ((float)(player.Center.Y + dist * Math.Sin(rot)));

					Vector2 drawPos = new Vector2(x, y) - Main.screenPosition;

					Color color;
					if (k == playerStars - 1 && (player.statMana % 20 != 0))
					{
						color = Color.White * ((player.statMana % 20f) / 20f);
					}
					else
					{
						color = Color.White;
					}
					DrawData data = new DrawData(texture, drawPos, null, color * 0.6f, MathHelper.PiOver2 + MathHelper.ToRadians(curRot), new Vector2(texture.Width / 2f, texture.Height / 2f), 0.55f, SpriteEffects.None, 0);
					Main.playerDrawData.Add(data);
				}
			}



			/**
			 * else if (k == playerStars - 1 && (player.statMana % 20 == 0))
				{
					color = Color.White * 0f;
				}
			DrawData draw = new DrawData(drawInfo.)
			Vector2 position = player.position;
			drawInfo.drawPlayer.position.X = player.position.X + 30;
			position.Y = player.position.Y + player.gfxOffY;
			Main.DrawPlayer(player, position, player.fullRotation, player.fullRotationOrigin, 0.5f + (float)Main.rand.Next(-10, 11) * 0.005f);
			position.X = player.position.X - player.shadowDodgeCount;
			this.DrawPlayer(player, position, player.fullRotation, player.fullRotationOrigin, 0.5f + (float)Main.rand.Next(-10, 11) * 0.005f);
			**/
		}

		private void SoulSeekerCaps()
        {
			if (!NPC.downedMoonlord)
            {
				enemiesKilled = (enemiesKilled > 150f) ? 150f : enemiesKilled;
            }
			if (!NPC.downedPlantBoss)
			{
				enemiesKilled = (enemiesKilled > 100f) ? 100f : enemiesKilled;
			}
			if (!Main.hardMode)
			{
				enemiesKilled = (enemiesKilled > 50f) ? 50f : enemiesKilled;
			}
		}



		// As a recap. Make a class variable, reset that variable in ResetEffects, and use that variable in the logic of whatever hooks you use.
	}

}
