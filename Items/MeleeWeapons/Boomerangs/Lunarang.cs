using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using BasicMod.Projectiles;

namespace BasicMod.Items.MeleeWeapons.Boomerangs
{
    public class Lunarang : ModItem
    {
        public override string Texture => "Terraria/Item_" + ItemID.Bananarang;
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("A boomerang from the stars.");
        }
        public override void SetDefaults()
        {
            item.damage = 40;
            item.melee = true;
            item.width = 30;
            item.height = 30;
            item.useTime = 15;
            item.useAnimation = 15;
            item.noUseGraphic = true;
            item.useStyle = 1;
            item.knockBack = 2;
            item.value = 8;
            item.rare = 6;
            item.shootSpeed = 12f;
            item.shoot = mod.ProjectileType("LunarangProjectile");
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
        }
        public override bool CanUseItem(Player player)       //this make that you can shoot only 1 boomerang at once
        {
            for (int i = 0; i < 1000; ++i)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == item.shoot)
                {
                    return false;
                }
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            // TODO: add ingredient droppable from new enemy?
            recipe.AddIngredient(ModContent.ItemType<Ores.SeleniumBar>(), 10);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }

    public class LunarangProjectile : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.Bananarang;

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.aiStyle = 3;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.magic = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
            projectile.light = 0.5f;
            projectile.extraUpdates = 1;

        }

        public override void OnHitNPC(NPC targetNpc, int damage, float knockback, bool crit)
        {
            Player player = Main.player[projectile.owner];
            Vector2 target = targetNpc.position;

            Vector2 position;
            float speedX = 10f;
            float speedY = 10f;
            float ceilingLimit = target.Y;
            for (int i = 0; i < 3; i++)
            {
                position = player.Center + new Vector2((-(float)Main.rand.Next(0, 401) * player.direction), -600f);
                position.Y -= (100 * i);
                Vector2 heading = target - position;
                if (heading.Y < 0f)
                {
                    heading.Y *= -1f;
                }
                if (heading.Y < 20f)
                {
                    heading.Y = 20f;
                }
                heading.Normalize();
                heading *= new Vector2(speedX, speedY).Length();
                speedX = heading.X;
                speedY = heading.Y + Main.rand.Next(-40, 41) * 0.02f;
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, 503, (int)(damage * 1.5f), 5, player.whoAmI, 0f, ceilingLimit);
            }
        }

        public override void PostAI()
        {
            for (int i = 0; i < 1; ++i)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 204);

            }
            base.PostAI();
        }

    }
}
