using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BasicMod.Items.SupportOrbs
{
    public class AttackOrb : SupportOrb
    {
        public override string Texture => "BasicMod/Items/SupportOrbs/SupportOrb";
        public override void SetDefaults()
        {
            base.SetDefaults(); // run parent code
            item.useAnimation = 120; // only one that matters, useTime gets set to it
            item.mana = 60;
            item.width = 28;
            item.height = 52;
            item.rare = 6;
            item.UseSound = SoundID.Item44;
            item.shoot = mod.ProjectileType("AttackOrbProjectile");
        }
    }

    public class AttackOrbProjectile : SupportOrbProjectile
    {
        public override string Texture => "BasicMod/Items/SupportOrbs/SupportOrb";
        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.width = 24;
            projectile.height = 24;
        }

        public override void OnFinish(Player player)
        {
            CreateText(player, Color.Crimson, "Attack Increased!");
            player.AddBuff(BuffID.AmmoBox, 1800);
        }
    }

    public class AttackOrbBuff : SupportOrbBuff
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            DisplayName.SetDefault("Attack Orb");
            Description.SetDefault(""); // modified in method
        }

        public override void Update(Player player, ref int buffIndex)
        {
            float increase = 0.5f;
            player.meleeDamage += increase;
            player.rangedDamage += increase;
            player.meleeDamage += increase;
        }

        public override void ModifyBuffTip(ref string tip, ref int rare)
        {

        }
    }
}
