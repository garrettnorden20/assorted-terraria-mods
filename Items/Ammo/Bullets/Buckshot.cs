using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BasicMod.Items.Ammo.Bullets
{
	public class Buckshot : ModItem
	{
		public override string Texture => "Terraria/Item_" + ItemID.EmptyBullet;
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Deals high damage and knockback but has short range");
		}

		public override void SetDefaults()
		{
			item.damage = 14;
			item.ranged = true;
			item.width = 8;
			item.height = 8;
			item.maxStack = 999;
			item.consumable = true;             //You need to set the item consumable so that the ammo would automatically consumed
			item.knockBack = 4.5f;
			item.value = 10;
			item.rare = ItemRarityID.Green;
			item.shoot = mod.ProjectileType("BuckshotProjectile");  //The projectile shoot when your weapon using this ammo
			item.shootSpeed = 4f;                  //The speed of the projectile
			item.ammo = AmmoID.Bullet;            //The ammo class this ammo belongs to.
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Grenade, 1);
			recipe.AddIngredient(ItemID.MusketBall, 70);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this, 70);
			recipe.AddRecipe();
		}
	}

	public class BuckshotProjectile : ModProjectile
	{
		public override string Texture => "Terraria/Projectile_" + ProjectileID.ExplosiveBullet;

		public override void SetDefaults()
		{
			projectile.width = 4;               //The width of projectile hitbox
			projectile.height = 4;              //The height of projectile hitbox
			projectile.aiStyle = 0;             //The ai style of the projectile, please reference the source code of Terraria
			projectile.friendly = true;         //Can the projectile deal damage to enemies?
			projectile.hostile = false;         //Can the projectile deal damage to the player?
			projectile.ranged = true;           //Is the projectile shoot by a ranged weapon?
			projectile.timeLeft = 30;          //The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			projectile.alpha = 255;             //The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
			projectile.light = 0.5f;            //How much light emit around the projectile
			projectile.tileCollide = true;          //Can the projectile collide with tiles?
			aiType = ProjectileID.Bullet;           //Act exactly like default Bullet
		}

        public override void PostAI()
        {
			if (projectile.alpha > 17)
            {
				projectile.alpha -= 17;
			}
            base.PostAI();
			projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
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
