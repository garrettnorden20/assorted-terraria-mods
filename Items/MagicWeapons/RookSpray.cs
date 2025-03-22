using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BasicMod.Items.MagicWeapons
{
	public class RookSpray : ModItem
	{
		public override string Texture => "Terraria/Item_" + ItemID.SilverAxe;
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Throws some rooks.");
		}

		public override void SetDefaults()
		{
			item.damage = 38; // Sets the item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
			item.magic = true; // sets the damage type to ranged
			item.mana = 17;
			item.width = 38; // hitbox width of the item
			item.height = 6; // hitbox height of the item
			item.useTime = 60; // The item's use time in ticks (60 ticks == 1 second.)
			item.useAnimation = 60; // The length of the item's use animation in ticks (60 ticks == 1 second.)
			item.useStyle = ItemUseStyleID.SwingThrow; // how you use the item (swinging, holding out, etc)
			item.noMelee = true; //so the item's animation doesn't do damage
			item.noUseGraphic = true;
			item.knockBack = 4; // Sets the item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
			item.value = 10000; // how much the item sells for (measured in copper)
			item.rare = ItemRarityID.Lime;// the color that the item's name will be in-game
			item.UseSound = SoundID.Item9; // The sound that this item plays when used.
			item.autoReuse = false; // if you can hold click to automatically use it again
			item.shootSpeed = 14f; // the speed of the projectile (measured in pixels per frame)

			item.shoot = mod.ProjectileType("RookProjectile");
		}
		/**
		public override void AddRecipes()
		{
		}
		**/

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			int numberProjectiles = 8 + Main.rand.Next(5); // 3 or 4 shots
			for (int i = 0; i < numberProjectiles; i++)
			{
				Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(30)); // 30 degree spread.
																												// If you want to randomize the speed to stagger the projectiles
				float scale = 1f - (Main.rand.NextFloat() * .3f);
				perturbedSpeed = perturbedSpeed * scale; 
				Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, knockBack, player.whoAmI);
			}
			return false; // return false because we don't want tmodloader to shoot projectile
		}

	}

	public class RookProjectile : ModProjectile
	{
		public override void SetDefaults()
		{
			projectile.width = 18;               //The width of projectile hitbox
			projectile.height = 28;              //The height of projectile hitbox
			projectile.aiStyle = -1;             //The ai style of the projectile, please reference the source code of Terraria
			projectile.friendly = true;         //Can the projectile deal damage to enemies?
			projectile.hostile = false;         //Can the projectile deal damage to the player?
			projectile.magic = true;           //Is the projectile shoot by a ranged weapon?
			projectile.timeLeft = 270;          //The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			projectile.alpha = 0;             //The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
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
			if (projectile.alpha > 50)
			{
				projectile.alpha -= 10;
			}

			projectile.velocity.X = projectile.velocity.X * 0.99f;
			projectile.velocity.Y = projectile.velocity.Y + 0.2f; // 0.1f for arrow gravity, 0.4f for knife gravity

			
			if (projectile.velocity.Y > 16f) // This check implements "terminal velocity". We don't want the projectile to keep getting faster and faster. Past 16f this projectile will travel through blocks, so this check is useful.
			{
				projectile.velocity.Y = 16f;
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
				projectile.velocity.Y = -oldVelocity.Y * 0.5f;
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
}
