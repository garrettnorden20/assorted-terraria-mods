using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BasicMod.Items.Ammo
{
	public class SharpDart : ModItem
	{
		public override string Texture => "Terraria/Item_" + ItemID.PoisonDart;
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Pierces enemies");
		}

		public override void SetDefaults()
		{
			item.damage = 10;
			item.ranged = true;
			item.width = 8;
			item.height = 8;
			item.maxStack = 999;
			item.consumable = true;
			item.knockBack = 2f;
			item.value = 10;
			item.rare = ItemRarityID.Green;
			item.shoot = mod.ProjectileType("SharpDartProjectile");
			item.shootSpeed = 3f;
			item.ammo = AmmoID.Dart;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(mod.ItemType("Dart"), 150);
			recipe.AddIngredient(ItemID.Stinger, 150);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this, 150);
			recipe.AddRecipe();
		}
	}

	public class SharpDartProjectile : ModProjectile
	{
		public override string Texture => "Terraria/Projectile_" + ProjectileID.PoisonDartBlowgun;

		public override void SetDefaults()
		{
			projectile.width = 14;     
			projectile.height = 14;    
			projectile.aiStyle = 1;    
			projectile.friendly = true;
			projectile.hostile = false;
			projectile.ranged = true;  
			projectile.timeLeft = 600; 
			projectile.alpha = 255;    
			projectile.light = 0.5f;            
			projectile.tileCollide = true;          
			aiType = ProjectileID.PoisonDartBlowgun;
			projectile.penetrate = 4;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			return true;
		}

        public override void PostAI()
        {
			int num = Main.rand.Next(2);
			for (int i = 0; i < num; i++) // x-(y-1) golden dusts every frame
			{
				Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Smoke, 0, 0, 0, default, 0.5f);

			}
		}

        public override void Kill(int timeLeft)
		{
			// This code and the similar code above in OnTileCollide spawn dust from the tiles collided with. SoundID.Item10 is the bounce sound you hear.
			Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
			Main.PlaySound(SoundID.Item10, projectile.position);
		}

	}
}
