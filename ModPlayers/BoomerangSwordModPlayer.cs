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

// MeleeSpeed buffs are naturally applied - they affect shootspeed, so basically how far the boomerang will go
// MeleeDamage buffs are also naturally applied. So don't double it again

// TODO: fix boomerangs being included in this, fix the automatic autoswing, add dusts/other unique effects on a sword-by-sword basis

namespace BasicMod
{
    class BoomerangSwordGlobalItem : GlobalItem
    {
		public static float Default_Shoot_Speed = 9f;
		public static float Default_Damage_Modifier = 0.5f;

		public override bool AltFunctionUse(Item item, Player player)
		{
			if ((item.useStyle == ItemUseStyleID.SwingThrow && item.melee == true) && item.hammer == 0 && item.pick == 0 && item.axe == 0 && !BasicWorld.boomerangIds.Contains(item.type))
			{
				return true;
			}
			
			return base.AltFunctionUse(item, player);

		}
		public override bool CanUseItem(Item item, Player player)
        {
			for (int i = 0; i < 1000; ++i)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == mod.ProjectileType("BoomerangSwordProjectile"))
				{
					return false;
				}
			}
			
			if (item.useStyle == ItemUseStyleID.SwingThrow && item.melee == true && player.altFunctionUse == 2)
            {
				item.noUseGraphic = true;
				item.noMelee = true;
				item.shoot = mod.ProjectileType("BoomerangSwordProjectile");
				item.shootSpeed = Default_Shoot_Speed;
				item.useTime = item.useAnimation;

			} else if (item.shoot == mod.ProjectileType("BoomerangSwordProjectile"))
            {
				Item copy = new Item();
				copy.SetDefaults(item.type);

				item.noUseGraphic = false;
				item.noMelee = false;
				item.shootSpeed = copy.shootSpeed;
				item.shoot = copy.shoot;
			}
			return base.CanUseItem(item, player);
		}
        public override bool Shoot(Item item, Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (type == mod.ProjectileType("BoomerangSwordProjectile"))
            {
				Projectile.NewProjectile(player.Center.X, player.Center.Y - item.height, speedX, speedY, type, (int)(damage * Default_Damage_Modifier), knockBack, player.whoAmI, item.type);

				return false;
			}
			return base.Shoot(item, player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
        }

	}

	public class TesterSwordBoom : ModItem
	{
		public override string Texture => "Terraria/Item_" + ItemID.Bananarang;
		public override void SetStaticDefaults()
		{

			Tooltip.SetDefault("It seems to be moving.");
		}
		public override void SetDefaults()
		{
			item.damage = 28;
			item.melee = true;
			item.width = 32;
			item.height = 32;
			item.useTime = 18;
			item.useAnimation = 18;
			item.noUseGraphic = true;
			item.useStyle = 1;
			item.knockBack = 9;
			item.value = 10000;
			item.rare = ItemRarityID.Orange;
			item.shootSpeed = 8f;
			item.shoot = mod.ProjectileType("BoomerangSwordProjectile");
			item.UseSound = SoundID.Item1;
			item.autoReuse = false;
			item.noMelee = true;
		}

		public override bool CanUseItem(Player player)       //this make that you can shoot only 1 boomerang at once
		{
			for (int i = 0; i < 1000; ++i)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == item.shoot)
				{
					return false;
				}
			}
			return true;
		}

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, ItemID.WoodenSword);
			return false;
        }

    }

	public class BoomerangSwordProjectile : ModProjectile
    {
		public override string Texture => "Terraria/Projectile_" + ProjectileID.Bananarang;
		public override void SetStaticDefaults()
		{

		}
		public override void SetDefaults()
		{
			Item sword = new Item();
			sword.SetDefaults((int)projectile.ai[0]);
			
			projectile.width = sword.width;
			projectile.height = sword.height;
			projectile.penetrate = -1;
			projectile.melee = true;
			projectile.aiStyle = -1;
			projectile.friendly = true;
			projectile.melee = true;
			projectile.timeLeft = 600;
			projectile.tileCollide = false;

		}

		float vel = 15f; // vanilla always 13-16
		float turnAccel = 0.6f;
		float boomStatus = 0;

		int timer = 0;
		bool canTileCollide = false;

		public override void AI()
		{
			Item sword = new Item();
			sword.SetDefaults((int)projectile.ai[0]);
			float length = MathHelper.Max(sword.width, sword.height);
			float scale = sword.scale > 1 ? sword.scale : 1;
			projectile.width = (int)(length * scale);
			projectile.height = (int)(length * scale);

			timer++;
			if (timer > 2)
            {
				canTileCollide = true;
				projectile.tileCollide = true;
            }

			if (boomStatus == 0f)
			{
				projectile.ai[1] += 1f;

				if (projectile.ai[1] >= 30f)
				{
					boomStatus = 1f;
					projectile.ai[1] = 0f;
					projectile.netUpdate = true;
				}
			}
			else
			{
				projectile.tileCollide = false;
				float num43 = vel; // speed
				float num44 = turnAccel; // turning acceleration

				Vector2 vector2 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f); // position of boomerang
				float num45 = Main.player[projectile.owner].position.X + (float)(Main.player[projectile.owner].width / 2) - vector2.X; // x of player, minus the x of boom
				float num46 = Main.player[projectile.owner].position.Y + (float)(Main.player[projectile.owner].height / 2) - vector2.Y; // y of player, minus the y of boom

				// if too far away, kill
				float num47 = (float)Math.Sqrt(num45 * num45 + num46 * num46);
				if (num47 > 3000f)
				{
					projectile.Kill();
				}
				num47 = num43 / num47;
				num45 *= num47;
				num46 *= num47;

				if (projectile.velocity.X < num45)
				{
					projectile.velocity.X += num44;
					if (projectile.velocity.X < 0f && num45 > 0f)
					{
						projectile.velocity.X += num44;
					}
				}
				else if (projectile.velocity.X > num45)
				{
					projectile.velocity.X -= num44;
					if (projectile.velocity.X > 0f && num45 < 0f)
					{
						projectile.velocity.X -= num44;
					}
				}
				if (projectile.velocity.Y < num46)
				{
					projectile.velocity.Y += num44;
					if (projectile.velocity.Y < 0f && num46 > 0f)
					{
						projectile.velocity.Y += num44;
					}
				}
				else if (projectile.velocity.Y > num46)
				{
					projectile.velocity.Y -= num44;
					if (projectile.velocity.Y > 0f && num46 < 0f)
					{
						projectile.velocity.Y -= num44;
					}
				}

				if (Main.myPlayer == projectile.owner)
				{ // kills projectile on return to player
					Rectangle rectangle = new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height);
					Rectangle value2 = new Rectangle((int)Main.player[projectile.owner].position.X, (int)Main.player[projectile.owner].position.Y, Main.player[projectile.owner].width, Main.player[projectile.owner].height);
					if (rectangle.Intersects(value2))
					{
						projectile.Kill();
					}
				}
			}

			// rotations
			projectile.rotation += 0.5f * (float)projectile.direction;
		}

		public override void OnHitNPC(NPC targetNpc, int damage, float knockback, bool crit)
		{
			Vector2 oldVelocity = projectile.velocity;
			Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
			Main.PlaySound(SoundID.Item10, projectile.position);

			if (projectile.velocity.X != oldVelocity.X)
			{
				projectile.velocity.X = -oldVelocity.X;
			}
			if (projectile.velocity.Y != oldVelocity.Y)
			{
				projectile.velocity.Y = -oldVelocity.Y;
			}
			boomStatus = 1;
		}

		public override void Kill(int timeLeft)
		{
			// This code and the similar code above in OnTileCollide spawn dust from the tiles collided with. SoundID.Item10 is the bounce sound you hear.
			Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
			Main.PlaySound(SoundID.Item10, projectile.position);
		}

		// allows a rebound
		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (!canTileCollide)
            {
				return false;
            }
			Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
			Main.PlaySound(SoundID.Item10, projectile.position);

			if (projectile.velocity.X != oldVelocity.X)
			{
				projectile.velocity.X = -oldVelocity.X;
			}
			if (projectile.velocity.Y != oldVelocity.Y)
			{
				projectile.velocity.Y = -oldVelocity.Y;
			}
			boomStatus = 1;
			return false;
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
			Vector2 drawOrigin = new Vector2(Main.itemTexture[(int)projectile.ai[0]].Width * 0.5f, Main.itemTexture[(int)projectile.ai[0]].Height * 0.5f);
			Vector2 drawPos = projectile.position - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
			
			spriteBatch.Draw(Main.itemTexture[(int)projectile.ai[0]], drawPos, null, lightColor, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
			return false;
		}
    }
}
