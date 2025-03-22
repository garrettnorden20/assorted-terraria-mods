using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using BasicMod.Projectiles;

namespace BasicMod.Items.MeleeWeapons.Boomerangs
{
    public class Beemerang : ModItem
    {
        public override string Texture => "Terraria/Item_" + ItemID.BeesKnees;
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Summons bees when striking an enemy. " + "\nDefeated enemies explode into killer bees");
        }
        public override void SetDefaults()
        {
            item.damage = 28;
            item.melee = true;
            item.width = 30;
            item.height = 30;
            item.useTime = 16;
            item.useAnimation = 16;
            item.noUseGraphic = true;
            item.useStyle = 1;
            item.knockBack = 5;
            item.value = 80000;
            item.rare = ItemRarityID.Orange;
            item.shootSpeed = 10f;
            item.shoot = mod.ProjectileType("BeemerangProjectile");
            item.UseSound = SoundID.Item1;
            item.autoReuse = false;
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
    }

    class BeemerangProjectile : ModProjectile
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

            if (Main.myPlayer == projectile.owner)
            {
                if (targetNpc.life < 0)
                {
                    Projectile.NewProjectile(targetNpc.position, Main.rand.NextVector2Unit() * 1f, 566, (int)(damage * 1f), 5, player.whoAmI);
                }
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Projectile.NewProjectile(targetNpc.position, Main.rand.NextVector2Unit() * 5f, 181, (int)(damage * 0.5f), 5, player.whoAmI);
                    }
                }
                projectile.netUpdate = true;
            }


        }

        public override void PostAI()
        {
            /**
			for (int i = 0; i < 1; ++i)
			{
				Dust.NewDust(projectile.position, projectile.width, projectile.height, 204);

			}
			base.PostAI();
			**/
        }
    }
}
