using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BasicMod.Items.MeleeWeapons.Slashers
{
	public class Visceria : SlasherItem
	{
		public override string Texture => "BasicMod/Items/MeleeWeapons/Slashers/ShroomSlasher";

		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Spawns homing blood shots on hit.");
		}
		public override void SetDefaults()
		{

			item.damage = 52;
			item.reuseDelay = 300;
			item.useTime = 40;
			item.useAnimation = 40;
			item.knockBack = 5f;

			item.shoot = mod.ProjectileType("VisceriaProjectile");
			item.autoReuse = true;
			item.height = 52;
			item.width = 60;

			item.value = 140000;
			item.rare = ItemRarityID.Lime;

			item.melee = true;
			item.UseSound = SoundID.Item1;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.noMelee = true;
			item.noUseGraphic = true;


		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.ShroomiteBar, 12);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}

	public class VisceriaProjectile : Slasher
	{
		public override string Texture => "BasicMod/Items/MeleeWeapons/Slashers/ShroomSlasher";
		public VisceriaProjectile()
		{
			swingDelay = 39;

			rotateNum1 = 0.9f;
			rotateNum2 = 1f - rotateNum1;

			dist = 55;

			hitboxWidth = 90;
			hitboxHeight = hitboxWidth;
			offsetX = -1 * (hitboxWidth / 2);
			offsetY = -1 * (hitboxHeight / 2);
		}
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Visceria");
		}

		public override void SetDefaults()
		{
			projectile.width = 1;
			projectile.height = 1;
			projectile.aiStyle = 19;
			projectile.penetrate = -1;
			projectile.scale = 1.3f;
			projectile.alpha = 0;

			projectile.hide = true;
			projectile.ownerHitCheck = true;
			projectile.melee = true;
			projectile.tileCollide = false;
			projectile.friendly = true;

		}
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            base.OnHitNPC(target, damage, knockback, crit);

			Projectile.NewProjectile(target.Center, Vector2.Zero, mod.ProjectileType("BloodShot"), (int)(damage * 0.5f), 1, projectile.owner, 0f, target.whoAmI);
		}


    }

	public class BloodShot : ModProjectile
    {
		public override string Texture => "Terraria/Projectile_" + ProjectileID.WoodenArrowFriendly;

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;    //The length of old position to be recorded
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;        //The recording mode
		}
		public override void SetDefaults()
		{
			//projectile.CloneDefaults(ProjectileID.TerraBeam);
			projectile.width = 16;
			projectile.height = 16;
			projectile.alpha = 50;
			projectile.friendly = true;
			projectile.alpha = 255;
			projectile.melee = true;
			projectile.tileCollide = true;
			projectile.ignoreWater = true;
			projectile.timeLeft = 90;
			projectile.aiStyle = 0;


		}

		float trueVelocity = 14f;
		NPC nearest = null;

		public override void AI()
		{
			projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

			/**
			for (int i = 0; i < rand.Next(1, 2); i++) // x-(y-1) golden dusts every frame
			{
				Dust.NewDust(projectile.position, projectile.width, projectile.height, 74);

			}
			**/
			for (int i = 0; i < 3; i++)
            {
				Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Blood, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.2f, 0, default, 1.5f);
			}

			projectile.ai[0] += 1f; // Use a timer 
			if (projectile.ai[0] == 1f) // find the thing to home in on
			{
				nearest = FindNearest(projectile.position, projectile.ai[1]);
				if (nearest == null)
				{
					projectile.Kill();
				}
			}
			if (nearest != null && nearest.active == true)
			{
				// update velocity each frame to follow the NPC
				Vector2 direction = Vector2.Normalize(Vector2.Subtract(nearest.position, projectile.position));
				projectile.velocity = trueVelocity * direction;
			} else
            {
				projectile.Kill();
            }

		}

		public override void Kill(int timeLeft)
		{
			// This code and the similar code above in OnTileCollide spawn dust from the tiles collided with. SoundID.Item10 is the bounce sound you hear.
			Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
			Main.PlaySound(SoundID.Item10, projectile.position);
		}

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
			for (int d = 0; d < 50; d++)
			{
				Vector2 vel = Main.rand.NextVector2Unit() * 5f;
				Dust.NewDust(target.Center, target.width, target.height, 5, vel.X, vel.Y); // blood
			}
		}

        public override bool? CanHitNPC(NPC target)
        {
			return (target.whoAmI != projectile.ai[1]);
        }

        public NPC FindNearest(Vector2 pos, float avoid) // avoid is one you already hit
		{
			NPC nearest = null;
			float oldDist = 1001;
			float newDist = 1000;
			for (int i = 0; i < Terraria.Main.npc.Length - 1; i++) //Do once for each NPC in the world
			{
				if (i == (int)avoid)//Don't target the one you want to avoid
					continue;
				if (Terraria.Main.npc[i].friendly == true)//Don't target town NPCs
					continue;
				if (Terraria.Main.npc[i].active == false)//Don't target dead NPCs
					continue;
				if (Terraria.Main.npc[i].damage == 0)//Don't target non-aggressive NPCs
					continue;
				if (!Collision.CanHit(pos, projectile.width, projectile.height, Main.npc[i].position, Main.npc[i].width, Main.npc[i].height))
				{
					continue;
				}
				if (nearest == null) //if no NPCs have made it past the previous few checks
					nearest = Terraria.Main.npc[i]; //become the nearest NPC 
				else
				{
					oldDist = Vector2.Distance(pos, nearest.position);//Check the distance to the nearest NPC that's earlier in the loop
					newDist = Vector2.Distance(pos, Terraria.Main.npc[i].position);//Check the distance to the current NPC in the loop
					if (newDist < oldDist)//If closer than the previous NPC in the loop
						nearest = Terraria.Main.npc[i];//Become the nearest NPC
				}
			}
			return nearest; //return the npc that is nearest to the vector 'pos'
		}
	}
}
