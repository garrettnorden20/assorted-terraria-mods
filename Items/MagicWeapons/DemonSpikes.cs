using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BasicMod.Items.MagicWeapons
{
    public class DemonSpikes : ModItem
    {
        public override string Texture => "Terraria/Item_" + ItemID.WoodenBow;
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Shoots two arrows for every shot.");
        }

        public override void SetDefaults()
        {
            item.damage = 40; // Sets the item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
            item.magic = true;
            item.width = 38; // hitbox width of the item
            item.height = 6; // hitbox height of the item

            item.useTime = 2; // The item's use time in ticks (60 ticks == 1 second.)
            item.useAnimation = 30; // The length of the item's use animation in ticks (60 ticks == 1 second.)
            item.reuseDelay = 50; // time between bursts

            item.useStyle = ItemUseStyleID.HoldingOut; // how you use the item (swinging, holding out, etc)
            item.noMelee = true; //so the item's animation doesn't do damage
            item.knockBack = 0; // Sets the item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
            item.value = 90000; // how much the item sells for (measured in copper)
            item.rare = ItemRarityID.LightRed;// the color that the item's name will be in-game
            item.UseSound = SoundID.Item43; // USE FOR BOWS
            item.autoReuse = false; // if you can hold click to automatically use it again
            item.shootSpeed = 10f; // the speed of the projectile (measured in pixels per frame)
            item.shoot = mod.ProjectileType("Spike");
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            
            //int multiplier = item.useAnimation / (player.itemAnimation+1);
            int multiplier = item.useAnimation/item.useTime - ((player.itemAnimation + 1)/item.useTime) + 1;
            Main.NewText(multiplier + "=" + item.useTime + " - " + (player.itemAnimation + 1) + " / " + item.useTime);

            float positionOffsetX = 45;
            float positionOffsetY = 0;

            Projectile.NewProjectile(position + new Vector2(positionOffsetX * multiplier * player.direction, positionOffsetY), Vector2.Zero, type, damage, knockBack, player.whoAmI);

            /**
            for (int i = 1; i < 5; i++)
            {
                Projectile.NewProjectile(position + new Vector2(positionOffsetX * i, positionOffsetY), Vector2.Zero, type, damage, knockBack, player.whoAmI);
            }
            **/
            return false;
        }

    }

    public class Spike : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 5;
        }
        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 70;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.magic = true;
            projectile.alpha = 10;
            projectile.light = 0.75f;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 120;

        }

        int retractTime = 10;
        int frameRetract;
        public override void AI()
        {
            projectile.ai[1] += 1f; // timer

            if (projectile.ai[1] == 1) // on startup
            {
                frameRetract = 10 / 5;
            }


            // one in whatever chance to spawn a dust (green)
            if (false)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 75);
            }


            // cycling through frame anims
            projectile.frameCounter++;
            if (projectile.frameCounter >= 5 && projectile.frame < 4 && projectile.timeLeft > retractTime)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }

            if (projectile.frameCounter >= frameRetract && projectile.frame > 0 && projectile.timeLeft <= retractTime)
            {
                projectile.frame--;
                projectile.frameCounter = 0;
            }

        }
    }
}
