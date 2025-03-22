using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BasicMod.Items.MagicWeapons
{
	public class RedFracture : ModItem
	{
		//public override string Texture => "Terraria/Item_" + ItemID.SilverAxe;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Red Fracture");

		}

		public override void SetDefaults()
		{
			item.damage = 38; // Sets the item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
			item.magic = true; // sets the damage type to ranged
			item.mana = 17;
			item.width = 38; // hitbox width of the item
			item.height = 6; // hitbox height of the item
			item.useTime = 35; // The item's use time in ticks (60 ticks == 1 second.)
			item.useAnimation = 35; // The length of the item's use animation in ticks (60 ticks == 1 second.)
			item.useStyle = ItemUseStyleID.SwingThrow; // how you use the item (swinging, holding out, etc)
			item.noMelee = true; //so the item's animation doesn't do damage
			item.knockBack = 4; // Sets the item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
			item.value = 10000; // how much the item sells for (measured in copper)
			item.rare = ItemRarityID.Lime;// the color that the item's name will be in-game
			item.UseSound = SoundID.Item9; // The sound that this item plays when used.
			item.autoReuse = false; // if you can hold click to automatically use it again
			item.shootSpeed = 25f; // the speed of the projectile (measured in pixels per frame)

			item.shoot = mod.ProjectileType("TargetterTarget");
		}

		public override void AddRecipes()
		{
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			return true;
		}

	}

	public class TargetterTarget : ModProjectile
	{
		public override string Texture => "Terraria/Projectile_" + ProjectileID.PoisonDartBlowgun;
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;    //The length of old position to be recorded
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;        //The recording mode
		}

		public override void SetDefaults()
		{
			projectile.width = 30;               //The width of projectile hitbox
			projectile.height = 30;              //The height of projectile hitbox
			projectile.aiStyle = 1;             //The ai style of the projectile, please reference the source code of Terraria
			projectile.friendly = true;         //Can the projectile deal damage to enemies?
			projectile.hostile = false;         //Can the projectile deal damage to the player?
			projectile.magic = true;           //Is the projectile shoot by a ranged weapon?
			projectile.timeLeft = 600;          //The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			projectile.alpha = 255;             //The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
			projectile.light = 0.5f;            //How much light emit around the projectile
			projectile.tileCollide = true;          //Can the projectile collide with tiles?
			//aiType = ProjectileID.PoisonDartBlowgun;           //Act exactly like default Bullet
		}

		public override void AI()
		{
			Player owner = Main.player[projectile.owner];
			//projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;
			projectile.ai[0] += 1f; // Use a timer 


			if (true)
			{
				int num27 = Main.rand.Next(3);
				if (num27 == 0)
				{
					num27 = 15;
				}
				else if (num27 == 1)
				{
					num27 = 57;
				}
				else
				{
					num27 = 58;
				}
				Dust.NewDust(projectile.position, projectile.width, projectile.height, num27, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 150, Color.Red, 1.5f);
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			/**
			//Redraw the projectile with the color not influenced by light
			Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
			for (int k = 0; k < projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
				Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
				spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
			}
			return true;
			**/
			return false;
		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			Player projOwner = Main.player[projectile.owner];
			for (int i = 0; i < 6; i++)
            {
				Projectile.NewProjectile(projOwner.position, Vector2.Zero, mod.ProjectileType("TargetSword"), projectile.damage, 1f, projectile.owner, i, target.whoAmI);
			}
			
        }

        public override void Kill(int timeLeft)
		{
			// This code and the similar code above in OnTileCollide spawn dust from the tiles collided with. SoundID.Item10 is the bounce sound you hear.
			Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
			Main.PlaySound(SoundID.Item10, projectile.position);
		}

	}

	public class TargetSword : ModProjectile
    {
		//public override string Texture => "Terraria/Projectile_" + ProjectileID.WoodenArrowFriendly;
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 3;    //The length of old position to be recorded
			ProjectileID.Sets.TrailingMode[projectile.type] = 1;        //The recording mode
		}
		public override void SetDefaults()
		{
			projectile.penetrate = -1;
			projectile.width = 10;               //The width of projectile hitbox
			projectile.height = 10;              //The height of projectile hitbox
			projectile.aiStyle = -1;             //The ai style of the projectile, please reference the source code of Terraria
			projectile.friendly = true;         //Can the projectile deal damage to enemies?
			projectile.hostile = false;         //Can the projectile deal damage to the player?
			projectile.magic = true;           //Is the projectile shoot by a ranged weapon?
			projectile.timeLeft = 600;          //The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			projectile.alpha = 255;             //The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
			projectile.light = 1f;            //How much light emit around the projectile
			projectile.tileCollide = false;          //Can the projectile collide with tiles
		}
		private float SwordNum
		{
			get => projectile.ai[0];
			set => projectile.ai[0] = value;
		}

		// This property encloses the internal AI variable projectile.ai[1].
		private float EnemyNum
		{
			get => projectile.ai[1];
			set => projectile.ai[1] = value;
		}


		int totalSwords = 6;

		float dist = 220; // distance from player
		float minDist = 70;

		float innerRotationMult = 2f; // higher means higher individual star rotation speed
		
		float curRot = 0;

		int AI_Timer = 0;

		float trueVel = 20f;

		public override void AI()
        {
			if (AI_Timer == 1)
            {
				projectile.alpha = 50;
            }

			NPC target = Main.npc[(int)EnemyNum];

			if (!target.active)
            {
				projectile.Kill();
            }

			if (AI_Timer < 60)
			{
				if (dist > minDist)
                {
					if (AI_Timer > 50)
                    {
						dist = dist + 5;
					} else
                    {
						dist = dist - 2;
					}
					
                } 

				projectile.friendly = false;
				curRot = curRot + 6f; // higher number, higher rotating speed

				float rot = MathHelper.ToRadians(curRot + SwordNum * (360f / totalSwords)); // or do playerstars

				float x = ((float)(target.Center.X + dist * Math.Cos(rot)));
				float y = ((float)(target.Center.Y + dist * Math.Sin(rot)));

				projectile.position = new Vector2(x, y);
				//projectile.rotation = rot + MathHelper.ToRadians(270);
				projectile.rotation = rot + MathHelper.PiOver4 + MathHelper.Pi;


			} else if (AI_Timer == 60)
            {
				projectile.friendly = true;
				projectile.penetrate = 1;
				Vector2 enemyDir = Vector2.Normalize(target.Center - projectile.position);
				projectile.velocity = trueVel * enemyDir;
            } else
            {
				Vector2 enemyDir = Vector2.Normalize(target.Center - projectile.position);

				projectile.velocity = trueVel * enemyDir;
				projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver4;
			}
			
			AI_Timer++;
			

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
			Rectangle sourceRectangle = new Rectangle(startX, 0, frameWidth, texture.Height);
			//Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
			//Vector2 origin = sourceRectangle.Size() / 2f;
			//origin.X = (float)(projectile.spriteDirection == 1 ? sourceRectangle.Width - 20 : 20);

			Color drawColor = projectile.GetAlpha(lightColor);

			Main.spriteBatch.Draw(texture,
				projectile.position - Main.screenPosition + new Vector2(0f, projectile.gfxOffY),
				sourceRectangle, drawColor, projectile.rotation, Vector2.Zero, projectile.scale, spriteEffects, 0f);


			//Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
			for (int k = 0; k < projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, projectile.gfxOffY);
				Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
				//spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
				Main.spriteBatch.Draw(texture,
				drawPos,
				sourceRectangle, color, projectile.rotation, Vector2.Zero, projectile.scale, spriteEffects, 0f);


			}


			return false;
		}



	}

}

