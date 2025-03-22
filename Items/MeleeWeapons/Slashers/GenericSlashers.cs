using BasicMod.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BasicMod.Items.MeleeWeapons.Slashers
{

	public static class GenericSlasher
    {
		public static float damageMult = 1.2f;
		public static float kbMult = 1.1f;
		public static float useTimeMult = 1.3f;
		public static int materialAmount = 10;

		// wooden sword, copper, etc
		public static int smallDist = 35;
		public static int smallLength = 50;
	}
	public class WoodenSlasher : SlasherItem
	{
		public override string Texture => "BasicMod/Items/MeleeWeapons/Slashers/GenericSlashersImages/WoodenSlasher";

		public override void SetDefaults()
		{
			Item copy = new Item();
			copy.SetDefaults(ItemID.WoodenSword);

			item.damage = (int)(copy.damage * GenericSlasher.damageMult);
			item.useTime = (int)(copy.useAnimation * GenericSlasher.useTimeMult);
			item.useAnimation = (int)(copy.useAnimation * GenericSlasher.useTimeMult);
			item.knockBack = (int)(copy.knockBack * GenericSlasher.kbMult);

			item.shoot = mod.ProjectileType("WoodenSlasherProjectile");
			item.autoReuse = false;

			item.value = copy.value;
			item.rare = copy.rare;

			item.melee = true;
			item.UseSound = copy.UseSound;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.height = copy.height;
			item.width = copy.width;

		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Wood, GenericSlasher.materialAmount);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}

	public class WoodenSlasherProjectile : Slasher
	{
		public override string Texture => "BasicMod/Items/MeleeWeapons/Slashers/GenericSlashersImages/WoodenSlasher";
		public WoodenSlasherProjectile()
		{
			Item copy = new Item();
			copy.SetDefaults(ItemID.WoodenSword);

			swingDelay = (int)(copy.useAnimation * GenericSlasher.useTimeMult) - 1;

			rotateNum1 = 0.80f;
			rotateNum2 = 1f - rotateNum1;

			dist = GenericSlasher.smallDist;

			hitboxWidth = GenericSlasher.smallLength;
			hitboxHeight = hitboxWidth;
			offsetX = -1 * (hitboxWidth / 2);
			offsetY = -1 * (hitboxHeight / 2);
		}
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Wooden Slasher");
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
	}

	public class CopperSlasher : SlasherItem
	{
		public override string Texture => "BasicMod/Items/MeleeWeapons/Slashers/GenericSlashersImages/CopperSlasher";

		public override void SetDefaults()
		{
			Item copy = new Item();
			copy.SetDefaults(ItemID.CopperBroadsword);

			item.damage = (int)(copy.damage * GenericSlasher.damageMult);
			item.useTime = (int)(copy.useTime * GenericSlasher.useTimeMult);
			item.useAnimation = (int)(copy.useTime * GenericSlasher.useTimeMult);
			item.knockBack = (int)(copy.knockBack * GenericSlasher.kbMult);

			item.shoot = mod.ProjectileType("CopperSlasherProjectile");
			item.autoReuse = false;

			item.value = copy.value;
			item.rare = copy.rare;

			item.melee = true;
			item.UseSound = copy.UseSound;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.height = copy.height;
			item.width = copy.width;
			
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.CopperBar, GenericSlasher.materialAmount);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}

	public class CopperSlasherProjectile : Slasher
	{
		public override string Texture => "BasicMod/Items/MeleeWeapons/Slashers/GenericSlashersImages/CopperSlasher";
		public CopperSlasherProjectile()
		{
			Item copy = new Item();
			copy.SetDefaults(ItemID.CopperBroadsword);

			swingDelay = (int)(copy.useTime * GenericSlasher.useTimeMult) - 1;

			rotateNum1 = 0.80f;
			rotateNum2 = 1f - rotateNum1;

			dist = GenericSlasher.smallDist;

			hitboxWidth = GenericSlasher.smallLength;
			hitboxHeight = hitboxWidth;
			offsetX = -1 * (hitboxWidth / 2);
			offsetY = -1 * (hitboxHeight / 2);
		}
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Copper Slasher");
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
	}

	public class TinSlasher : SlasherItem
	{
		public override string Texture => "BasicMod/Items/MeleeWeapons/Slashers/GenericSlashersImages/TinSlasher";

		public override void SetDefaults()
		{
			Item copy = new Item();
			copy.SetDefaults(ItemID.TinBroadsword);

			item.damage = (int)(copy.damage * GenericSlasher.damageMult);
			item.useTime = (int)(copy.useTime * GenericSlasher.useTimeMult);
			item.useAnimation = (int)(copy.useTime * GenericSlasher.useTimeMult);
			item.knockBack = (int)(copy.knockBack * GenericSlasher.kbMult);

			item.shoot = mod.ProjectileType("TinSlasherProjectile");
			item.autoReuse = false;

			item.value = copy.value;
			item.rare = copy.rare;

			item.melee = true;
			item.UseSound = copy.UseSound;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.height = copy.height;
			item.width = copy.width;

		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.TinBar, GenericSlasher.materialAmount);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}

	public class TinSlasherProjectile : Slasher
	{
		public override string Texture => "BasicMod/Items/MeleeWeapons/Slashers/GenericSlashersImages/TinSlasher";
		public TinSlasherProjectile()
		{
			Item copy = new Item();
			copy.SetDefaults(ItemID.TinBroadsword);

			swingDelay = (int)(copy.useTime * GenericSlasher.useTimeMult) - 1;

			rotateNum1 = 0.80f;
			rotateNum2 = 1f - rotateNum1;

			dist = GenericSlasher.smallDist;

			hitboxWidth = GenericSlasher.smallLength;
			hitboxHeight = hitboxWidth;
			offsetX = -1 * (hitboxWidth / 2);
			offsetY = -1 * (hitboxHeight / 2);
		}
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Tin Slasher");
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
	}

	public class IronSlasher : SlasherItem
	{
		public override string Texture => "BasicMod/Items/MeleeWeapons/Slashers/GenericSlashersImages/IronSlasher";

		public override void SetDefaults()
		{
			Item copy = new Item();
			copy.SetDefaults(ItemID.IronBroadsword);

			item.damage = (int)(copy.damage * GenericSlasher.damageMult);
			item.useTime = (int)(copy.useTime * GenericSlasher.useTimeMult);
			item.useAnimation = (int)(copy.useTime * GenericSlasher.useTimeMult);
			item.knockBack = (int)(copy.knockBack * GenericSlasher.kbMult);

			item.shoot = mod.ProjectileType("IronSlasherProjectile");
			item.autoReuse = false;

			item.value = copy.value;
			item.rare = copy.rare;

			item.melee = true;
			item.UseSound = copy.UseSound;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.height = copy.height;
			item.width = copy.width;

		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.IronBar, GenericSlasher.materialAmount);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}

	public class IronSlasherProjectile : Slasher
	{
		public override string Texture => "BasicMod/Items/MeleeWeapons/Slashers/GenericSlashersImages/IronSlasher";
		public IronSlasherProjectile()
		{
			Item copy = new Item();
			copy.SetDefaults(ItemID.IronBroadsword);

			swingDelay = (int)(copy.useTime * GenericSlasher.useTimeMult) - 1;

			rotateNum1 = 0.80f;
			rotateNum2 = 1f - rotateNum1;

			dist = GenericSlasher.smallDist;

			hitboxWidth = GenericSlasher.smallLength;
			hitboxHeight = hitboxWidth;
			offsetX = -1 * (hitboxWidth / 2);
			offsetY = -1 * (hitboxHeight / 2);
		}
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Iron Slasher");
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
	}

	public class LeadSlasher : SlasherItem
	{
		public override string Texture => "BasicMod/Items/MeleeWeapons/Slashers/GenericSlashersImages/LeadSlasher";

		public override void SetDefaults()
		{
			Item copy = new Item();
			copy.SetDefaults(ItemID.LeadBroadsword);

			item.damage = (int)(copy.damage * GenericSlasher.damageMult);
			item.useTime = (int)(copy.useTime * GenericSlasher.useTimeMult);
			item.useAnimation = (int)(copy.useTime * GenericSlasher.useTimeMult);
			item.knockBack = (int)(copy.knockBack * GenericSlasher.kbMult);

			item.shoot = mod.ProjectileType("LeadSlasherProjectile");
			item.autoReuse = false;

			item.value = copy.value;
			item.rare = copy.rare;

			item.melee = true;
			item.UseSound = copy.UseSound;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.height = copy.height;
			item.width = copy.width;

		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.LeadBar, GenericSlasher.materialAmount);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
	
	public class LeadSlasherProjectile : Slasher
	{
		public override string Texture => "BasicMod/Items/MeleeWeapons/Slashers/GenericSlashersImages/LeadSlasher";
		public LeadSlasherProjectile()
		{
			Item copy = new Item();
			copy.SetDefaults(ItemID.LeadBroadsword);

			swingDelay = (int)(copy.useTime * GenericSlasher.useTimeMult) - 1;

			rotateNum1 = 0.80f;
			rotateNum2 = 1f - rotateNum1;

			dist = GenericSlasher.smallDist;

			hitboxWidth = GenericSlasher.smallLength;
			hitboxHeight = hitboxWidth;
			offsetX = -1 * (hitboxWidth / 2);
			offsetY = -1 * (hitboxHeight / 2);
		}
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Lead Slasher");
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
	}

	public class SilverSlasher : SlasherItem
	{
		public override string Texture => "BasicMod/Items/MeleeWeapons/Slashers/GenericSlashersImages/SilverSlasher";

		public override void SetDefaults()
		{
			Item copy = new Item();
			copy.SetDefaults(ItemID.SilverBroadsword);

			item.damage = (int)(copy.damage * GenericSlasher.damageMult);
			item.useTime = (int)(copy.useTime * GenericSlasher.useTimeMult);
			item.useAnimation = (int)(copy.useTime * GenericSlasher.useTimeMult);
			item.knockBack = (int)(copy.knockBack * GenericSlasher.kbMult);

			item.shoot = mod.ProjectileType("SilverSlasherProjectile");
			item.autoReuse = false;

			item.value = copy.value;
			item.rare = copy.rare;

			item.melee = true;
			item.UseSound = copy.UseSound;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.height = copy.height;
			item.width = copy.width;

		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.SilverBar, GenericSlasher.materialAmount);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}

	public class SilverSlasherProjectile : Slasher
	{
		public override string Texture => "BasicMod/Items/MeleeWeapons/Slashers/GenericSlashersImages/SilverSlasher";
		public SilverSlasherProjectile()
		{
			Item copy = new Item();
			copy.SetDefaults(ItemID.SilverBroadsword);

			swingDelay = (int)(copy.useTime * GenericSlasher.useTimeMult) - 1;

			rotateNum1 = 0.80f;
			rotateNum2 = 1f - rotateNum1;

			dist = GenericSlasher.smallDist;

			hitboxWidth = GenericSlasher.smallLength;
			hitboxHeight = hitboxWidth;
			offsetX = -1 * (hitboxWidth / 2);
			offsetY = -1 * (hitboxHeight / 2);
		}
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Silver Slasher");
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
	}

	public class TungstenSlasher : SlasherItem
	{
		public override string Texture => "BasicMod/Items/MeleeWeapons/Slashers/GenericSlashersImages/TungstenSlasher";

		public override void SetDefaults()
		{
			Item copy = new Item();
			copy.SetDefaults(ItemID.TungstenBroadsword);

			item.damage = (int)(copy.damage * GenericSlasher.damageMult);
			item.useTime = (int)(copy.useTime * GenericSlasher.useTimeMult);
			item.useAnimation = (int)(copy.useTime * GenericSlasher.useTimeMult);
			item.knockBack = (int)(copy.knockBack * GenericSlasher.kbMult);

			item.shoot = mod.ProjectileType("TungstenSlasherProjectile");
			item.autoReuse = false;

			item.value = copy.value;
			item.rare = copy.rare;

			item.melee = true;
			item.UseSound = copy.UseSound;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.height = copy.height;
			item.width = copy.width;

		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.TungstenBar, GenericSlasher.materialAmount);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}

	public class TungstenSlasherProjectile : Slasher
	{
		public override string Texture => "BasicMod/Items/MeleeWeapons/Slashers/GenericSlashersImages/TungstenSlasher";
		public TungstenSlasherProjectile()
		{
			Item copy = new Item();
			copy.SetDefaults(ItemID.TungstenBroadsword);

			swingDelay = (int)(copy.useTime * GenericSlasher.useTimeMult) - 1;

			rotateNum1 = 0.80f;
			rotateNum2 = 1f - rotateNum1;

			dist = GenericSlasher.smallDist;

			hitboxWidth = GenericSlasher.smallLength;
			hitboxHeight = hitboxWidth;
			offsetX = -1 * (hitboxWidth / 2);
			offsetY = -1 * (hitboxHeight / 2);
		}
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Tungsten Slasher");
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
	}

	public class GoldSlasher : SlasherItem
	{
		public override string Texture => "BasicMod/Items/MeleeWeapons/Slashers/GenericSlashersImages/GoldSlasher";

		public override void SetDefaults()
		{
			Item copy = new Item();
			copy.SetDefaults(ItemID.GoldBroadsword);

			item.damage = (int)(copy.damage * GenericSlasher.damageMult);
			item.useTime = (int)(copy.useTime * GenericSlasher.useTimeMult);
			item.useAnimation = (int)(copy.useTime * GenericSlasher.useTimeMult);
			item.knockBack = (int)(copy.knockBack * GenericSlasher.kbMult);

			item.shoot = mod.ProjectileType("GoldSlasherProjectile");
			item.autoReuse = false;

			item.value = copy.value;
			item.rare = copy.rare;

			item.melee = true;
			item.UseSound = copy.UseSound;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.height = copy.height;
			item.width = copy.width;

		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.GoldBar, GenericSlasher.materialAmount);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}

	public class GoldSlasherProjectile : Slasher
	{
		public override string Texture => "BasicMod/Items/MeleeWeapons/Slashers/GenericSlashersImages/GoldSlasher";
		public GoldSlasherProjectile()
		{
			Item copy = new Item();
			copy.SetDefaults(ItemID.GoldBroadsword);

			swingDelay = (int)(copy.useTime * GenericSlasher.useTimeMult) - 1;

			rotateNum1 = 0.80f;
			rotateNum2 = 1f - rotateNum1;

			dist = GenericSlasher.smallDist;

			hitboxWidth = GenericSlasher.smallLength;
			hitboxHeight = hitboxWidth;
			offsetX = -1 * (hitboxWidth / 2);
			offsetY = -1 * (hitboxHeight / 2);
		}
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Gold Slasher");
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
	}

	public class PlatinumSlasher : SlasherItem
	{
		public override string Texture => "BasicMod/Items/MeleeWeapons/Slashers/GenericSlashersImages/PlatinumSlasher";

		public override void SetDefaults()
		{
			Item copy = new Item();
			copy.SetDefaults(ItemID.PlatinumBroadsword);

			item.damage = (int)(copy.damage * GenericSlasher.damageMult);
			item.useTime = (int)(copy.useTime * GenericSlasher.useTimeMult);
			item.useAnimation = (int)(copy.useTime * GenericSlasher.useTimeMult);
			item.knockBack = (int)(copy.knockBack * GenericSlasher.kbMult);

			item.shoot = mod.ProjectileType("PlatinumSlasherProjectile");
			item.autoReuse = false;

			item.value = copy.value;
			item.rare = copy.rare;

			item.melee = true;
			item.UseSound = copy.UseSound;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.height = copy.height;
			item.width = copy.width;

		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.PlatinumBar, GenericSlasher.materialAmount);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}

	public class PlatinumSlasherProjectile : Slasher
	{
		public override string Texture => "BasicMod/Items/MeleeWeapons/Slashers/GenericSlashersImages/PlatinumSlasher";
		public PlatinumSlasherProjectile()
		{
			Item copy = new Item();
			copy.SetDefaults(ItemID.PlatinumBroadsword);

			swingDelay = (int)(copy.useTime * GenericSlasher.useTimeMult) - 1;

			rotateNum1 = 0.80f;
			rotateNum2 = 1f - rotateNum1;

			dist = GenericSlasher.smallDist;

			hitboxWidth = GenericSlasher.smallLength;
			hitboxHeight = hitboxWidth;
			offsetX = -1 * (hitboxWidth / 2);
			offsetY = -1 * (hitboxHeight / 2);
		}
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Platinum Slasher");
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
	}
}