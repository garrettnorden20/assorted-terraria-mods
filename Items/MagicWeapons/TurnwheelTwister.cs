using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BasicMod.Items.MagicWeapons
{
	public class TurnwheelTwister : ModItem
	{
		public override string Texture => "Terraria/Item_" + ItemID.SilverAxe;
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Summons bullet-shooting cogs.");
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
			item.autoReuse = true; // if you can hold click to automatically use it again
			item.shootSpeed = 8f; // the speed of the projectile (measured in pixels per frame)

			item.shoot = mod.ProjectileType("TurnwheelTwisterProjectile");
		}

		public override void AddRecipes()
		{
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			return true;
		}

	}

	public class TurnwheelTwisterProjectile : ModProjectile
	{
		//public override string Texture => "Terraria/Projectile_" + ProjectileID.PoisonDartBlowgun;
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;    //The length of old position to be recorded
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;        //The recording mode
		}

		public override void SetDefaults()
		{
			projectile.penetrate = -1;
			projectile.width = 40;               //The width of projectile hitbox
			projectile.height = 40;              //The height of projectile hitbox
			projectile.aiStyle = -1;             //The ai style of the projectile, please reference the source code of Terraria
			projectile.friendly = true;         //Can the projectile deal damage to enemies?
			projectile.hostile = false;         //Can the projectile deal damage to the player?
			projectile.magic = true;           //Is the projectile shoot by a ranged weapon?
			projectile.timeLeft = 270;          //The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			projectile.alpha = 255;             //The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
			projectile.light = 0.5f;            //How much light emit around the projectile
			projectile.tileCollide = true;          //Can the projectile collide with tiles?
													//aiType = ProjectileID.PoisonDartBlowgun;           //Act exactly like default Bullet
		}

		public float AI_Timer
		{
			get => projectile.ai[0];
			set => projectile.ai[0] = value;
		}

		public override void AI()
		{
			Player owner = Main.player[projectile.owner];
			//projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;
			projectile.ai[0] += 1f; // Use a timer 
			if (projectile.alpha > 100)
            {
				projectile.alpha -= 10;
            }

			if (AI_Timer > 25)
            {
				projectile.velocity *= .98f;
			}

			if (AI_Timer == 60)
			{
				projectile.rotation += MathHelper.PiOver4;
				if ((Main.netMode != NetmodeID.MultiplayerClient))
                {
					ShootBullets();
					projectile.netUpdate = true;
				}
				projectile.Kill();
				
			}

			if (projectile.velocity.Length() > 2.5f)
			{
				Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Smoke, projectile.velocity.X * 0.4f, projectile.velocity.Y * 0.4f, 0, default, 1f);
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D text = mod.GetTexture("Items/MagicWeapons/TurnwheelTwisterProjectile");


			Vector2 drawOrigin = new Vector2(text.Width * 0.5f, text.Height * 0.5f);
			Vector2 drawPos = projectile.position - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);

			spriteBatch.Draw(text, drawPos, null, lightColor, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
			return false;
		}

		private void ShootBullets()
        {
			float rotIncrement = MathHelper.ToRadians(35);

			float currRotation = projectile.rotation;

			Vector2 velocity = currRotation.ToRotationVector2() * 10f;

			for (int i = 0; i < 9; i++)
            {
				Projectile.NewProjectile(projectile.Center, velocity.RotatedBy(i * (rotIncrement)), mod.ProjectileType("TurnwheelTwisterProjectile2"), projectile.damage, 1, Main.myPlayer);
			}

		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{

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
			return false;
		}

		public override void Kill(int timeLeft)
		{
			// This code and the similar code above in OnTileCollide spawn dust from the tiles collided with. SoundID.Item10 is the bounce sound you hear.
			for (int i = 0; i < 10; i++)
            {
				Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Smoke, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 0, default, 1f);
			} 
			Main.PlaySound(SoundID.Item10, projectile.position);
		}

	}

	public class TurnwheelTwisterProjectile2 : ModProjectile
	{
		public override string Texture => "Terraria/Item_" + ItemID.Cog;
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;    //The length of old position to be recorded
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;        //The recording mode
		}

		public override void SetDefaults()
		{
			projectile.penetrate = 3;
			projectile.width = 20;               //The width of projectile hitbox
			projectile.height = 20;              //The height of projectile hitbox
			projectile.aiStyle = 0;             //The ai style of the projectile, please reference the source code of Terraria
			projectile.friendly = true;         //Can the projectile deal damage to enemies?
			projectile.hostile = false;         //Can the projectile deal damage to the player?
			projectile.magic = true;           //Is the projectile shoot by a ranged weapon?
			projectile.timeLeft = 160;          //The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			projectile.alpha = 255;             //The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
			projectile.light = 0.5f;            //How much light emit around the projectile
			projectile.tileCollide = true;          //Can the projectile collide with tiles?
													//aiType = ProjectileID.PoisonDartBlowgun;           //Act exactly like default Bullet
		}

		public float AI_Timer
		{
			get => projectile.ai[0];
			set => projectile.ai[0] = value;
		}

        public override void PostAI()
        {
			projectile.rotation += MathHelper.ToRadians(16);
			Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Smoke, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 0, default, 1f);
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D text = Main.itemTexture[ItemID.Cog];


			Vector2 drawOrigin = new Vector2(text.Width * 0.5f, text.Height * 0.5f);
			Vector2 drawPos = projectile.position - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);

			spriteBatch.Draw(text, drawPos, null, lightColor, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
			return false;
		}

		public override void Kill(int timeLeft)
		{
			// This code and the similar code above in OnTileCollide spawn dust from the tiles collided with. SoundID.Item10 is the bounce sound you hear.
			for (int i = 0; i < 10; i++)
			{
				Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Smoke, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 0, default, 1f);
			}
			Main.PlaySound(SoundID.Item10, projectile.position);
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			//If collide with tile, reduce the penetrate.
			//So the projectile can reflect at most 5 times
			projectile.penetrate--;
			if (projectile.penetrate <= 0)
			{
				projectile.Kill();
			}
			else
			{
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
			}
			return false;
		}

	}

}

