using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BasicMod.Items.Ammo.Darts
{
	public class ChlorophyteDart : ModItem
	{
		public override string Texture => "Terraria/Item_" + ItemID.PoisonDart;
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Homes in on enemies.");
		}

		public override void SetDefaults()
		{
			item.damage = 24;
			item.ranged = true;
			item.width = 8;
			item.height = 8;
			item.maxStack = 999;
			item.consumable = true;             //You need to set the item consumable so that the ammo would automatically consumed
			item.knockBack = 3f;
			item.value = 10;
			item.rare = ItemRarityID.Lime;
			item.shoot = mod.ProjectileType("ChlorophyteDartProjectile");  //The projectile shoot when your weapon using this ammo
			item.shootSpeed = 1f;                  //The speed of the projectile
			item.ammo = AmmoID.Dart;            //The ammo class this ammo belongs to.
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.ChlorophyteBar);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this, 150);
			recipe.AddRecipe();
		}
	}

	public class ChlorophyteDartProjectile : ModProjectile
	{
		public override string Texture => "Terraria/Projectile_" + ProjectileID.PoisonDartBlowgun;

		public override void SetDefaults()
		{
			projectile.width = 14;               //The width of projectile hitbox
			projectile.height = 14;              //The height of projectile hitbox
			projectile.aiStyle = -1;             //The ai style of the projectile, please reference the source code of Terraria
			projectile.friendly = true;         //Can the projectile deal damage to enemies?
			projectile.hostile = false;         //Can the projectile deal damage to the player?
			projectile.ranged = true;           //Is the projectile shoot by a ranged weapon?
			projectile.timeLeft = 600;          //The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			projectile.alpha = 255;             //The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
			projectile.tileCollide = true;          //Can the projectile collide with tiles?
			projectile.extraUpdates = 1;
			//aiType = ProjectileID.ChlorophyteArrow;           //Act exactly like default Bullet
		}

		public override void AI()
        {
			//Main.PlaySound(6, (int)base.position.X, (int)base.position.Y);
			if (Main.rand.NextBool())
			{
				Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 40);
			}
			projectile.ai[0] += 1f;
			if (projectile.ai[0] > 3f)
			{
				projectile.alpha = 0;
			}
			if (projectile.ai[0] >= 20f)
			{
				projectile.ai[0] = 20f;

				Main.NewText("3ok");
				projectile.velocity.Y += 0.075f;
			}

			projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + 1.57f;
			
			if (projectile.velocity.Y > 16f)
			{
				projectile.velocity.Y = 16f;
			}

			/**
			if (projectile.alpha < 170)
			{
				for (int num164 = 0; num164 < 10; num164++)
				{
					float x2 = projectile.position.X - projectile.velocity.X / 10f * (float)num164;
					float y2 = projectile.position.Y - projectile.velocity.Y / 10f * (float)num164;
					int num165 = Dust.NewDust(new Vector2(x2, y2), 1, 1, 75);
					Main.dust[num165].alpha = projectile.alpha;
					Main.dust[num165].position.X = x2;
					Main.dust[num165].position.Y = y2;
					Main.dust[num165].velocity *= 0f;
					Main.dust[num165].noGravity = true;
				}
			}
			**/
			float num166 = (float)Math.Sqrt(projectile.velocity.X * projectile.velocity.X + projectile.velocity.Y * projectile.velocity.Y);
			float num167 = projectile.localAI[0];
			if (num167 == 0f)
			{
				projectile.localAI[0] = num166;
				num167 = num166;
			}
			if (projectile.alpha > 0)
			{
				projectile.alpha -= 25;
			}
			if (projectile.alpha < 0)
			{
				projectile.alpha = 0;
			}
			float num168 = projectile.position.X;
			float num169 = projectile.position.Y;
			float num170 = 300f;
			bool flag4 = false;
			int num171 = 0;
			if (projectile.ai[1] == 0f)
			{
				for (int num172 = 0; num172 < 200; num172++)
				{
					if (Main.npc[num172].CanBeChasedBy(this) && (projectile.ai[1] == 0f || projectile.ai[1] == (float)(num172 + 1)))
					{
						float num173 = Main.npc[num172].position.X + (float)(Main.npc[num172].width / 2);
						float num174 = Main.npc[num172].position.Y + (float)(Main.npc[num172].height / 2);
						float num175 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num173) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num174);
						if (num175 < num170 && Collision.CanHit(new Vector2(projectile.position.X + (float)(projectile.width / 2), projectile.position.Y + (float)(projectile.height / 2)), 1, 1, Main.npc[num172].position, Main.npc[num172].width, Main.npc[num172].height))
						{
							num170 = num175;
							num168 = num173;
							num169 = num174;
							flag4 = true;
							num171 = num172;
						}
					}
				}
				if (flag4)
				{
					projectile.ai[1] = num171 + 1;
				}
				flag4 = false;
			}
			if (projectile.ai[1] > 0f)
			{
				int num176 = (int)(projectile.ai[1] - 1f);
				if (Main.npc[num176].active && Main.npc[num176].CanBeChasedBy(this, ignoreDontTakeDamage: true) && !Main.npc[num176].dontTakeDamage)
				{
					float num177 = Main.npc[num176].position.X + (float)(Main.npc[num176].width / 2);
					float num178 = Main.npc[num176].position.Y + (float)(Main.npc[num176].height / 2);
					if (Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num177) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num178) < 1000f)
					{
						flag4 = true;
						num168 = Main.npc[num176].position.X + (float)(Main.npc[num176].width / 2);
						num169 = Main.npc[num176].position.Y + (float)(Main.npc[num176].height / 2);
					}
				}
				else
				{
					projectile.ai[1] = 0f;
				}
			}
			if (!projectile.friendly)
			{
				flag4 = false;
			}
			if (flag4)
			{
				float num179 = num167;
				Vector2 vector9 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
				float num180 = num168 - vector9.X;
				float num181 = num169 - vector9.Y;
				float num182 = (float)Math.Sqrt(num180 * num180 + num181 * num181);
				num182 = num179 / num182;
				num180 *= num182;
				num181 *= num182;
				int num183 = 8;
				projectile.velocity.X = (projectile.velocity.X * (float)(num183 - 1) + num180) / (float)num183;
				projectile.velocity.Y = (projectile.velocity.Y * (float)(num183 - 1) + num181) / (float)num183;
			}

		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			return true;
		}

		public override void Kill(int timeLeft)
		{
			// This code and the similar code above in OnTileCollide spawn dust from the tiles collided with. SoundID.Item10 is the bounce sound you hear.
			Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
			Main.PlaySound(SoundID.Item10, projectile.position);
		}

	}
}
