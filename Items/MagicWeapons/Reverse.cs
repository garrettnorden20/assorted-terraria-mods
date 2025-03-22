using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BasicMod.Items.MagicWeapons
{
	public class Reverse : ModItem
	{
		public override string Texture => "Terraria/Item_" + ItemID.EmptyBullet;
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Unleashes a flurry of feathers");
		}

		public override void SetDefaults()
		{
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.useTime = 5;
			item.useAnimation = 5;
			item.autoReuse = true;
			item.damage = 40;
			item.magic = true;
			item.width = 8;
			item.height = 8;
			item.knockBack = 2.5f;
			item.value = 10;
			item.rare = ItemRarityID.Green;
			item.shoot = mod.ProjectileType("ReverseProjectile");  //The projectile shoot when your weapon using this ammo
			item.shootSpeed = 16f;                  //The speed of the projectile
			//item.ammo = AmmoID.Bullet;            //The ammo class this ammo belongs to.
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.MagicDagger, 1);
			recipe.AddIngredient(mod.ItemType("TeslaRemnant"), 5);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			Projectile.NewProjectile(position, new Vector2(speedX * -1, speedY * -1), type, damage, knockBack, player.whoAmI, 1);
			Projectile.NewProjectile(position, new Vector2(speedX * -1, speedY * -1), type, damage, knockBack, player.whoAmI, -1);

			return false;
        }
    }

	public class ReverseProjectile : ModProjectile
	{
		public override string Texture => "Terraria/Projectile_" + ProjectileID.MagicDagger;

		public override void SetDefaults()
		{
			projectile.width = 4;               //The width of projectile hitbox
			projectile.height = 4;              //The height of projectile hitbox
			projectile.aiStyle = -1;             //The ai style of the projectile, please reference the source code of Terraria
			projectile.friendly = true;         //Can the projectile deal damage to enemies?
			projectile.hostile = false;         //Can the projectile deal damage to the player?
			projectile.ranged = true;           //Is the projectile shoot by a ranged weapon?
			projectile.timeLeft = 300;          //The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			projectile.alpha = 50;             //The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
			projectile.light = 0.5f;            //How much light emit around the projectile
			projectile.tileCollide = false;          //Can the projectile collide with tiles?
													 //aiType = ProjectileID.Bullet;           //Act exactly like default Bullet
			projectile.extraUpdates = 1;
		}

		private float AI_Timer
		{
			get => projectile.ai[1];
			set => projectile.ai[1] = value;
		}

		float initRot = 0;
		Vector2 initVel = Vector2.Zero;
		float degreeTurn = 0;

		float randomDegChange;
		float randomVelChange;
		public override void AI()
        {
			
			if (AI_Timer == 0)
            {
				initVel = projectile.velocity;
				initRot = projectile.velocity.ToRotation();

				if (projectile.ai[0] == 1)
                {
					randomDegChange = Main.rand.Next(0, 10);
				} else
                {
					randomDegChange = Main.rand.Next(-10, 10);
				}
				
				randomVelChange = Main.rand.NextFloat(-1f, 1f);
            }


			degreeTurn += 14;
			if (degreeTurn > 185)
            {
				degreeTurn = 185;
            }
			
						
			if (AI_Timer < 25)
            {
				projectile.velocity += Vector2.Normalize(initVel.RotatedBy(MathHelper.ToRadians((degreeTurn * projectile.ai[0]) + randomDegChange))) * 8f;
				projectile.velocity = Vector2.Normalize(projectile.velocity) * (8f + randomVelChange);
			} else
            {
				if (projectile.velocity.Length() < 12f)
                {
					projectile.velocity *= 1.01f;
				}
				
			}

			if (AI_Timer > 60)
            {
				projectile.tileCollide = true;
            }

			projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
			AI_Timer++;
        }


        public override void Kill(int timeLeft)
		{
			if (timeLeft <= 0)
			{
				for (int i = 0; i < 5; i++)
				{
					Vector2 randomDust = Main.rand.NextVector2Unit() * 3f;
					//var dust = Dust.NewDustDirect(box.TopLeft(), box.Width, box.Height, 226, randomDust.X, randomDust.Y, 255, default, 1.5f);
					Dust dust = Dust.NewDustDirect(projectile.Center, projectile.width, projectile.height, 60, randomDust.X * 0.5f, randomDust.Y * 0.5f, 0, default, 1.5f);
					dust.noGravity = true;
				}
			}
			// This code and the similar code above in OnTileCollide spawn dust from the tiles collided with. SoundID.Item10 is the bounce sound you hear.
			Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
			Main.PlaySound(SoundID.Item10, projectile.position);
		}

	}
}
