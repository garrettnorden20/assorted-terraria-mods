using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BasicMod.Items.MagicWeapons
{
	public class BladeFlurry : ModItem
	{
		public override string Texture => "Terraria/Item_" + ItemID.SilverAxe;
		public override void SetDefaults()
		{
			item.damage = 45;
			item.magic = true;
			item.mana = 3;
			item.width = 38;
			item.height = 6;
			item.useTime = 6;
			item.useAnimation = 6;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.noUseGraphic = true;
			item.noMelee = true;
			item.knockBack = 5;
			item.value = 10000;
			item.rare = ItemRarityID.Lime;
			item.UseSound = SoundID.Item9;
			item.autoReuse = true;
			item.shootSpeed = 18f;

			item.shoot = mod.ProjectileType("BladeFlurryProjectile");
		}

		public override void AddRecipes()
		{
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(35));
			speedX = perturbedSpeed.X;
			speedY = perturbedSpeed.Y;
			Projectile.NewProjectile(position, new Vector2(speedX, speedY) * Main.rand.NextFloat(1.2f, 1.8f), type, damage, knockBack, player.whoAmI, Main.rand.Next(8));
			return false;
		}
	}

	public class BladeFlurryProjectile : ModProjectile
	{
		//public override string Texture => "Terraria/Projectile_" + ProjectileID.WoodenArrowFriendly;
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 3;    //The length of old position to be recorded
			ProjectileID.Sets.TrailingMode[projectile.type] = 1;        //The recording mode
		}
		public override void SetDefaults()
		{
			projectile.penetrate = 3;
			projectile.width = 32;
			projectile.height = 32;
			projectile.aiStyle = -1;
			projectile.friendly = true;
			projectile.hostile = false;
			projectile.magic = true;
			projectile.timeLeft = 180;
			projectile.alpha = 255;             //The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
			projectile.light = 1f;
			projectile.tileCollide = false;
		}
		private float SwordNum
		{
			get => projectile.ai[0];
			set => projectile.ai[0] = value;
		}

		private float AI_Timer
		{
			get => projectile.ai[1];
			set => projectile.ai[1] = value;
		}

		int totalSwords = 6;

		float dist = 220; // distance from player
		float minDist = 70;

		float innerRotationMult = 2f; // higher means higher individual star rotation speed

		float curRot = 0;


		float trueVel = 20f;

		public override void AI()
		{
			if (AI_Timer == 1)
			{
				projectile.alpha = 5;
			}
			if (AI_Timer > 140)
			{
				projectile.alpha += 2;
			}

			projectile.velocity *= 0.95f;

			//projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver4;
			projectile.rotation += (0.5f + AI_Timer/900) * (float)projectile.direction;

			AI_Timer++;

			
		}

		public Color GetColor()
		{
			switch (SwordNum)
			{
				case 0:
					return Color.Red;
				case 1:
					return Color.DarkSalmon;
				case 2:
					return Color.Cyan;
				case 3:
					return Color.Yellow;
				case 4:
					return Color.Indigo;
				case 5:
					return Color.Violet;
				case 6:
					return Color.Lime;
				case 7:
					return Color.Orange;
				default:
					return Color.Pink;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{

			SpriteEffects spriteEffects = SpriteEffects.None;
			if (projectile.spriteDirection == -1)
			{
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
			Texture2D texture = Main.projectileTexture[projectile.type];
			int frameWidth = Main.projectileTexture[projectile.type].Width / 14;
			int startX = frameWidth * (int)SwordNum;
			Vector2 drawOrigin = new Vector2(frameWidth * 0.5f, 32 * 0.5f);
			drawOrigin = Vector2.Zero;
			Rectangle sourceRectangle = new Rectangle(startX, 0, frameWidth, texture.Height);
			
			drawOrigin = sourceRectangle.Size() / 2f;
			drawOrigin.X = (float)(projectile.spriteDirection == 1 ? sourceRectangle.Width - 20 : 20);

			Color drawColor = projectile.GetAlpha(lightColor);
			drawColor = GetColor();
			float col = 200;
			drawColor.MultiplyRGB(new Color(col, col, col));
			drawColor.A = 0;

			Main.spriteBatch.Draw(texture,
				projectile.position - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY),
				sourceRectangle, drawColor, projectile.rotation, drawOrigin, projectile.scale, spriteEffects, 0f);


			//Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
			for (int k = 0; k < projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, projectile.gfxOffY);
				Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
				//spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
				//Main.spriteBatch.Draw(texture, drawPos, sourceRectangle, color, projectile.rotation, Vector2.Zero, projectile.scale, spriteEffects, 0f);


			}


			return false;
		}
	}
}
