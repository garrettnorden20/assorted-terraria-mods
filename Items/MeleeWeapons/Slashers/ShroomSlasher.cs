using Microsoft.Xna.Framework;
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
	public class ShroomSlasher : SlasherItem
	{
		public override string Texture => "BasicMod/Items/MeleeWeapons/Slashers/ShroomSlasher";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sword of Shrooms");
			Tooltip.SetDefault("Swings create mushrooms that damage enemies.");
		}
		public override void SetDefaults()
		{

			item.damage = 72;
			item.reuseDelay = 300;
			item.useTime = 30;
			item.useAnimation = 30;
			item.knockBack = 5f;

			item.shoot = mod.ProjectileType("ShroomSlasherProjectile");
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

	public class ShroomSlasherProjectile : Slasher
	{
		public override string Texture => "BasicMod/Items/MeleeWeapons/Slashers/ShroomSlasher";
		public ShroomSlasherProjectile()
		{
			swingDelay = 29;

			rotateNum1 = 0.85f;
			rotateNum2 = 1f - rotateNum1;

			dist = 55;

			hitboxWidth = 90;
			hitboxHeight = hitboxWidth;
			offsetX = -1 * (hitboxWidth / 2);
			offsetY = -1 * (hitboxHeight / 2);


			numProjs = 11 ;
			projVelocity = 8f;
		}
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Shroom Slasher");
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

		public override void CustomBehavior(bool swingDownwards, Player projOwner)
		{
			float mushroomDistance = 100f;
			float mushroomMult = 0.5f;

			Vector2 dir = (projectile.rotation + MathHelper.PiOver4).ToRotationVector2();
			if ((!swingDownwards && projOwner.direction < 0) || (swingDownwards && projOwner.direction > 0))
			{
				if (projectile.rotation > initialRotation + totalProjRange)
				{
					//Main.NewText("in proj fir");
					Projectile.NewProjectile(projOwner.Center + (dir * mushroomDistance), projVelocity * dir, 131, (int)(projectile.damage * mushroomMult), 1, projOwner.whoAmI, 0f);
					totalProjRange += eachProjRange;
				}
			}
			else
			{
				if (projectile.rotation < initialRotation + totalProjRange)
				{
					//Main.NewText("in proj fir");
					Projectile.NewProjectile(projOwner.Center + (dir * mushroomDistance), projVelocity * dir, 131, (int)(projectile.damage * mushroomMult), 1, projOwner.whoAmI, 0f);
					totalProjRange += eachProjRange;
				}
			}
		}

	}
}
