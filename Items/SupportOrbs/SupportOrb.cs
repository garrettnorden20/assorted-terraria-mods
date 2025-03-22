using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BasicMod.Items.SupportOrbs
{
	public class SupportOrbPlayer : ModPlayer
    {
		public float orbTimeMult; // 1 is base; multiplies the useAnimation, so 1.2 would be quicker, 0.8 slower
		public float orbEffectMult; // 1 is base; multiplies whatever effect there is
		public bool currentlyOrbing;

        public override void ResetEffects()
        {
			currentlyOrbing = false;
			orbTimeMult = 1f;
			orbEffectMult = 1f;
        }

        public override void PostUpdateRunSpeeds()
        {
            if (player.channel && BasicWorld.orbIds.Contains(player.inventory[player.selectedItem].type))
			{
				player.maxRunSpeed *= 0.5f;
				if (Math.Abs(player.velocity.X) > player.maxRunSpeed)
                {
					player.velocity.X = player.maxRunSpeed * (player.velocity.X/Math.Abs(player.velocity.X)); // slow the run down to temp maxRunSpeed, also fixes for direction

				}


			}
		}
    }

    public class SupportOrb : ModItem
    {
		public override string Texture => "BasicMod/Items/SupportOrbs/SupportOrb";
		public override void SetDefaults()
        {

			item.useAnimation = 120; // only one that matters, useTime gets set to it
			item.mana = 60;
			item.width = 28;
            item.height = 52;
            item.rare = 6; 
            item.UseSound = SoundID.Item44;
            item.shoot = mod.ProjectileType("SupportOrbProjectile");

			// Values that should not be changed
			item.noMelee = true;
			item.noUseGraphic = true;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.shootSpeed = 3.7f;
			item.channel = true;
		}
    }

    public class SupportOrbProjectile : ModProjectile
    {
		public override string Texture => "BasicMod/Items/SupportOrbs/SupportOrb";

		public override void SetDefaults()
		{
			projectile.width = 24;
			projectile.height = 24;
			
			// Do not change
			projectile.penetrate = -1;
			projectile.scale = 1.3f;
			projectile.alpha = 0;
			projectile.aiStyle = 19;
			projectile.hide = true;
			projectile.ownerHitCheck = true;
			projectile.tileCollide = false;
			projectile.friendly = true;
		}

		public float AI_Timer
		{
			get => projectile.ai[0];
			set => projectile.ai[0] = value;
		}

		public override void AI()
		{
			Player projOwner = Main.player[projectile.owner];
			SupportOrbPlayer mp = projOwner.GetModPlayer<SupportOrbPlayer>();

			if (AI_Timer == 0)
            {
				projOwner.itemAnimation = (int)(projOwner.itemAnimation * (1f/mp.orbTimeMult)); // apply quickness modifier
			}

			ProjectileSetUp(projOwner);

			mp.currentlyOrbing = true;

			projectile.position += projectile.velocity * 12f;
			
			if (!projOwner.channel) // if stop the channeling
			{
				projectile.Kill();
				projOwner.itemAnimation = 0;
				projOwner.itemTime = 0;
				return;
			}
			
			if (projOwner.itemAnimation == 0) {
				OnFinish(projOwner);
            }

			Rotations();

			DoDust();
			
			AI_Timer++;
		}

		public virtual void OnFinish(Player player)
        {
			//CreateText(player, Color.White, "Attack Increased!");
			//player.AddBuff(BuffID.WellFed, 600);
		}

		public void CreateText(Player player, Color color, String text)
        {
			int num = CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), color, text);
			CombatText combatText = Main.combatText[num];
			combatText.scale = 4f;

			if (Main.netMode == 2 && num != 100)
			{
				NetMessage.SendData(MessageID.CombatTextInt, -1, -1, null, (int)combatText.color.PackedValue, combatText.position.X, combatText.position.Y);
			}

		}

		public void DoDust()
        {
			if (Main.rand.NextBool(3))
			{
				Dust dust = Dust.NewDustDirect(projectile.position - (projectile.velocity * 5f), projectile.height, projectile.width, 204,
					projectile.velocity.X * .2f, projectile.velocity.Y * .2f, 200, Scale: 1.2f);
				dust.velocity += projectile.velocity * 0.3f;
				dust.velocity *= 0.2f;
			}
			if (Main.rand.NextBool(4))
			{
				Dust dust = Dust.NewDustDirect(projectile.position - (projectile.velocity * 5f), projectile.height, projectile.width, 204,
					0, 0, 254, Scale: 0.3f);
				dust.velocity += projectile.velocity * 0.5f;
				dust.velocity *= 0.5f;
			}
		}

		private void Rotations()
        {
			projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);
			if (projectile.spriteDirection == -1)
			{
				projectile.rotation -= MathHelper.ToRadians(90f);
			}
		}

		private void ProjectileSetUp(Player projOwner)
        {
			Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
			projectile.direction = projOwner.direction;
			projOwner.heldProj = projectile.whoAmI;
			projOwner.itemTime = projOwner.itemAnimation;
			projectile.position.X = ownerMountedCenter.X - (float)(projectile.width / 2);
			projectile.position.Y = ownerMountedCenter.Y - (float)(projectile.height / 2);
		}
	}

	public class SupportOrbBuff : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Support Orb");
			Description.SetDefault("You shouldn't be seeing this");
			
			// Don't change these
			canBeCleared = true;
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = false;
			Main.debuff[Type] = false; 
		}

		public override void Update(Player player, ref int buffIndex)
		{
		}
	}
}
