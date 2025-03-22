using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BasicMod.Items.MagicWeapons
{
	public class MysticHandaxe : ModItem
	{
		public override string Texture => "BasicMod/Items/MagicWeapons/MysticHandaxe";
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Throws axes in an arc above your head.");
		}
		public override void SetDefaults()
		{
			item.damage = 60;
			item.magic = true;
			item.mana = 15;
			item.width = 38;
			item.height = 6;
			item.useTime = 45;
			item.useAnimation = 45;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.knockBack = 8f;
			item.value = 50000;
			item.rare = ItemRarityID.Lime;
			item.UseSound = SoundID.Item9;
			item.autoReuse = true;
			item.shootSpeed = 18f;

			item.shoot = mod.ProjectileType("MysticHandaxeProjectile");
		}

		public override void AddRecipes()
		{
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			speedX = 3f * (speedX / Math.Abs(speedX));
			speedY = -10f;
			Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
			return false;
		}
	}

	public class MysticHandaxeProjectile : ModProjectile
	{
		public override string Texture => "BasicMod/Items/MagicWeapons/MysticHandaxe";

		int totalTrail = 30;
		int trailSkip = 5;
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[projectile.type] = totalTrail;    //The length of old position to be recorded
			ProjectileID.Sets.TrailingMode[projectile.type] = 1;        //The recording mode
		}
		public override void SetDefaults()
		{
			projectile.penetrate = -1;
			projectile.width = 80;
			projectile.height = 80;
			projectile.scale = 1f;
			projectile.aiStyle = -1;
			projectile.friendly = true;
			projectile.hostile = false;
			projectile.magic = true;
			projectile.timeLeft = 300;
			projectile.alpha = 50;
			projectile.light = 0.1f;
			projectile.tileCollide = false;
			projectile.extraUpdates = 1;
		}

		private float AI_Timer
		{
			get => projectile.ai[1];
			set => projectile.ai[1] = value;
		}

		List<float> rotPoints = new List<float>();

		public override void AI()
		{
			if (AI_Timer == 0)
			{
			}

			projectile.velocity.Y += 0.16f;

			//projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver4;
			projectile.rotation += (0.1f) * (float)projectile.direction;
			
			rotPoints.Insert(0, projectile.rotation);	
			if (rotPoints.Count > totalTrail)
            {
				rotPoints.RemoveAt(rotPoints.Count - 1);
			}

			AI_Timer++;

			/**
			 * //StarFury
			if (Main.rand.Next(16) == 0)
			{
				Vector2 value3 = Vector2.UnitX.RotatedByRandom(1.5707963705062866).RotatedBy(projectile.velocity.ToRotation());
				int num61 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 58, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 150, default(Color), 1.2f);
				Main.dust[num61].velocity = value3 * 0.66f;
				Main.dust[num61].position = projectile.Center + value3 * 12f;
			}
			if (Main.rand.Next(48) == 0)
			{
				int num62 = Gore.NewGore(projectile.Center, new Vector2(projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f), 16);
				Gore gore5 = Main.gore[num62];
				Gore gore2 = gore5;
				gore2.velocity *= 0.66f;
				gore5 = Main.gore[num62];
				gore2 = gore5;
				gore2.velocity += projectile.velocity * 0.3f;
			}
			**/
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.immune[projectile.owner] = 10;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
			Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
			for (int k = 0; k < rotPoints.Count; k = k+trailSkip)
			{
				Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
				Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
				float rotation;
				if (rotPoints[k] == 0f)
                {
					rotation = projectile.rotation;
				} else
                {
					rotation = rotPoints[k];
                }
				spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
			}
			return true;
		}
    }
}