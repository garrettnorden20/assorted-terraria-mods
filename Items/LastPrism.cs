using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Enums;
using Terraria.GameContent.Shaders;
using IL.Terraria.Graphics.Effects;
using System.IO;

namespace BasicMod.Items
{
	public class ExampleLastPrism : ModItem
	{
		// You can use a vanilla texture for your item by using the format: "Terraria/Item_<Item ID>".
		public override string Texture => "Terraria/Item_" + ItemID.LastPrism;
		public static Color OverrideColor = new Color(122, 173, 255);

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Example Last Prism");
			Tooltip.SetDefault(@"A slightly different laser-firing Prism
Ignores NPC immunity frames and fires 10 beams at once instead of 6.");
		}

		public override void SetDefaults()
		{
			// Start by using CloneDefaults to clone all the basic item properties from the vanilla Last Prism.
			// For example, this copies sprite size, use style, sell price, and the item being a magic weapon.
			item.CloneDefaults(ItemID.LastPrism);
			item.mana = 4;
			item.damage = 42;
			item.shoot = ModContent.ProjectileType<ExampleLastPrismHoldout>();
			item.shootSpeed = 30f;

			// Change the item's draw color so that it is visually distinct from the vanilla Last Prism.
			item.color = OverrideColor;
		}

		public override void AddRecipes()
		{
		}

		// Because this weapon fires a holdout projectile, it needs to block usage if its projectile already exists.
		public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<ExampleLastPrismHoldout>()] <= 0;
	}

	public class ExampleLastPrismHoldout : ModProjectile
	{
		public override string Texture => "Terraria/Projectile_" + ProjectileID.LastPrism;

		// The vanilla Last Prism is an animated item with 5 frames of animation. We copy that here.
		private const int NumAnimationFrames = 5;

		// This controls how many individual beams are fired by the Prism.
		public const int NumBeams = 6;

		// This value controls how many frames it takes for the Prism to reach "max charge". 60 frames = 1 second.
		public const float MaxCharge = 180f;

		// This value controls how many frames it takes for the beams to begin dealing damage. Before then they can't hit anything.
		public const float DamageStart = 30f;

		// This value controls how sluggish the Prism turns while being used. Vanilla Last Prism is 0.08f.
		// Higher values make the Prism turn faster.
		private const float AimResponsiveness = 0.08f;

		// This value controls how frequently the Prism emits sound once it's firing.
		private const int SoundInterval = 20;

		// These values place caps on the mana consumption rate of the Prism.
		// When first used, the Prism consumes mana once every MaxManaConsumptionDelay frames.
		// Every time mana is consumed, the pace becomes one frame faster, meaning mana consumption smoothly increases.
		// When capped out, the Prism consumes mana once every MinManaConsumptionDelay frames.
		private const float MaxManaConsumptionDelay = 15f;
		private const float MinManaConsumptionDelay = 5f;

		// This property encloses the internal AI variable projectile.ai[0]. It makes the code easier to read.
		private float FrameCounter
		{
			get => projectile.ai[0];
			set => projectile.ai[0] = value;
		}

		// This property encloses the internal AI variable projectile.ai[1].
		private float NextManaFrame
		{
			get => projectile.ai[1];
			set => projectile.ai[1] = value;
		}

		// This property encloses the internal AI variable projectile.localAI[0].
		// localAI is not automatically synced over the network, but that does not cause any problems in this case.
		private float ManaConsumptionRate
		{
			get => projectile.localAI[0];
			set => projectile.localAI[0] = value;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Example Last Prism");
			Main.projFrames[projectile.type] = NumAnimationFrames;

			// Signals to Terraria that this projectile requires a unique identifier beyond its index in the projectile array.
			// This prevents the issue with the vanilla Last Prism where the beams are invisible in multiplayer.
			ProjectileID.Sets.NeedsUUID[projectile.type] = true;
		}

		public override void SetDefaults()
		{
			// Use CloneDefaults to clone all basic projectile statistics from the vanilla Last Prism.
			projectile.CloneDefaults(ProjectileID.LastPrism);
		}

		public override void AI()
		{
			Player player = Main.player[projectile.owner];
			Vector2 rrp = player.RotatedRelativePoint(player.MountedCenter, true);

			// Update the Prism's damage every frame so that it is dynamically affected by Mana Sickness.
			UpdateDamageForManaSickness(player);

			// Update the frame counter.
			FrameCounter += 1f;

			// Update projectile visuals and sound.
			UpdateAnimation();
			PlaySounds();

			// Update the Prism's position in the world and relevant variables of the player holding it.
			UpdatePlayerVisuals(player, rrp);

			// Update the Prism's behavior: project beams on frame 1, consume mana, and despawn if out of mana.
			if (projectile.owner == Main.myPlayer)
			{
				// Slightly re-aim the Prism every frame so that it gradually sweeps to point towards the mouse.
				UpdateAim(rrp, player.HeldItem.shootSpeed);

				// player.CheckMana returns true if the mana cost can be paid. Since the second argument is true, the mana is actually consumed.
				// If mana shouldn't consumed this frame, the || operator short-circuits its evaluation player.CheckMana never executes.
				bool manaIsAvailable = !ShouldConsumeMana() || player.CheckMana(player.HeldItem.mana, true, false);

				// The Prism immediately stops functioning if the player is Cursed (player.noItems) or "Crowd Controlled", e.g. the Frozen debuff.
				// player.channel indicates whether the player is still holding down the mouse button to use the item.

				//bool stillInUse = player.channel && manaIsAvailable && !player.noItems && !player.CCed;
				bool stillInUse = FrameCounter < 180f && !player.CCed;

				// Spawn in the Prism's lasers on the first frame if the player is capable of using the item.
				if (stillInUse && FrameCounter == 1f)
				{
					FireBeams();
				}

				// If the Prism cannot continue to be used, then destroy it immediately.
				else if (!stillInUse)
				{
					projectile.Kill();
				}
			}

			// This ensures that the Prism never times out while in use.
			projectile.timeLeft = 2;
		}

		private void UpdateDamageForManaSickness(Player player)
		{
			float ownerCurrentMagicDamage = player.allDamage + (player.magicDamage - 1f);
			projectile.damage = (int)(player.HeldItem.damage * ownerCurrentMagicDamage);
		}

		private void UpdateAnimation()
		{
			projectile.frameCounter++;

			// As the Prism charges up and focuses the beams, its animation plays faster.
			int framesPerAnimationUpdate = FrameCounter >= MaxCharge ? 2 : FrameCounter >= (MaxCharge * 0.66f) ? 3 : 4;

			// If necessary, change which specific frame of the animation is displayed.
			if (projectile.frameCounter >= framesPerAnimationUpdate)
			{
				projectile.frameCounter = 0;
				if (++projectile.frame >= NumAnimationFrames)
				{
					projectile.frame = 0;
				}
			}
		}

		private void PlaySounds()
		{
			// The Prism makes sound intermittently while in use, using the vanilla projectile variable soundDelay.
			if (projectile.soundDelay <= 0)
			{
				projectile.soundDelay = SoundInterval;

				// On the very first frame, the sound playing is skipped. This way it doesn't overlap the starting hiss sound.
				if (FrameCounter > 1f)
				{
					Main.PlaySound(SoundID.Item15, projectile.position);
				}
			}
		}

		private void UpdatePlayerVisuals(Player player, Vector2 playerHandPos)
		{
			// Place the Prism directly into the player's hand at all times.
			projectile.Center = playerHandPos + new Vector2(0, -400);
			// The beams emit from the tip of the Prism, not the side. As such, rotate the sprite by pi/2 (90 degrees).
			projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;
			projectile.spriteDirection = projectile.direction;

			// The Prism is a holdout projectile, so change the player's variables to reflect that.
			// Constantly resetting player.itemTime and player.itemAnimation prevents the player from switching items or doing anything else.
			player.ChangeDir((Main.MouseWorld - player.position).X > 0 ? 1 : -1);
			player.heldProj = projectile.whoAmI;
			player.itemTime = 2;
			player.itemAnimation = 2;

			// If you do not multiply by projectile.direction, the player's hand will point the wrong direction while facing left.
			//player.itemRotation = (projectile.velocity * projectile.direction).ToRotation();
		}

		private bool ShouldConsumeMana()
		{
			// If the mana consumption timer hasn't been initialized yet, initialize it and consume mana on frame 1.
			if (ManaConsumptionRate == 0f)
			{
				NextManaFrame = ManaConsumptionRate = MaxManaConsumptionDelay;
				return true;
			}

			// Should mana be consumed this frame?
			bool consume = FrameCounter == NextManaFrame;

			// If mana is being consumed this frame, update the rate of mana consumption and write down the next frame mana will be consumed.
			if (consume)
			{
				// MathHelper.Clamp(X,A,B) guarantees that A <= X <= B. If X is outside the range, it will be set to A or B accordingly.
				ManaConsumptionRate = MathHelper.Clamp(ManaConsumptionRate - 1f, MinManaConsumptionDelay, MaxManaConsumptionDelay);
				NextManaFrame += ManaConsumptionRate;
			}
			return consume;
		}

		private void UpdateAim(Vector2 source, float speed)
		{
			// Get the player's current aiming direction as a normalized vector.
			Vector2 aim = Vector2.Normalize(Main.MouseWorld - source);
			if (aim.HasNaNs())
			{
				aim = -Vector2.UnitY;
			}

			// Change a portion of the Prism's current velocity so that it points to the mouse. This gives smooth movement over time.
			aim = Vector2.Normalize(Vector2.Lerp(Vector2.Normalize(projectile.velocity), aim, AimResponsiveness));
			aim *= speed;

			if (aim != projectile.velocity)
			{
				projectile.netUpdate = true;
			}
			//projectile.velocity = aim;
			projectile.velocity = new Vector2(0, 1);
		}

		private void FireBeams()
		{
			// If for some reason the beam velocity can't be correctly normalized, set it to a default value.
			Vector2 beamVelocity = Vector2.Normalize(projectile.velocity);
			if (beamVelocity.HasNaNs())
			{
				beamVelocity = -Vector2.UnitY;
			}

			// This UUID will be the same between all players in multiplayer, ensuring that the beams are properly anchored on the Prism on everyone's screen.
			int uuid = Projectile.GetByUUID(projectile.owner, projectile.whoAmI);

			int damage = projectile.damage;
			float knockback = projectile.knockBack;
			for (int b = 0; b < NumBeams; ++b)
			{
				Projectile.NewProjectile(projectile.Center, beamVelocity, ModContent.ProjectileType<ExampleLastPrismBeam>(), damage, knockback, projectile.owner, b, uuid);
			}

			// After creating the beams, mark the Prism as having an important network event. This will make Terraria sync its data to other players ASAP.
			projectile.netUpdate = true;
		}

		// Because the Prism is a holdout projectile and stays glued to its user, it needs custom drawcode.
		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			SpriteEffects effects = projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			Texture2D texture = Main.projectileTexture[projectile.type];
			int frameHeight = texture.Height / Main.projFrames[projectile.type];
			int spriteSheetOffset = frameHeight * projectile.frame;
			Vector2 sheetInsertPosition = (projectile.Center + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition).Floor();

			// The Prism is always at full brightness, regardless of the surrounding light. This is equivalent to it being its own glowmask.
			// It is drawn in a non-white color to distinguish it from the vanilla Last Prism.
			Color drawColor = ExampleLastPrism.OverrideColor;
			spriteBatch.Draw(texture, sheetInsertPosition, new Rectangle?(new Rectangle(0, spriteSheetOffset, texture.Width, frameHeight)), drawColor, projectile.rotation, new Vector2(texture.Width / 2f, frameHeight / 2f), projectile.scale, effects, 0f);
			return false;
		}
	}

	public class ExampleLastPrismBeam : ModProjectile
	{
		// A helpful math constant for performing beam angling calculations.
		private const float PiBeamDivisor = MathHelper.Pi / ExampleLastPrismHoldout.NumBeams;

		// How much more damage the beams do when the Prism is fully charged. Damage smoothly scales up to this multiplier.
		private const float MaxDamageMultiplier = 1.5f;

		// Beams increase their scale from 0 to this value as the Prism charges up.
		private const float MaxBeamScale = 1.8f;

		// Beams reduce their spread to zero as the Prism charges up. This controls the maximum spread.
		private const float MaxBeamSpread = 1.5f;

		// The maximum possible range of the beam. Don't set this too high or it will cause significant lag.
		private const float MaxBeamLength = 800f;

		// The width of the beam in pixels for the purposes of tile collision.
		// This should generally be left at 1, otherwise the beam tends to stop early when touching tiles.
		private const float BeamTileCollisionWidth = 1f;

		// The width of the beam in pixels for the purposes of entity hitbox collision.
		// This gets scaled with the beam's scale value, so as the beam visually grows its hitbox gets wider as well.
		private const float BeamHitboxCollisionWidth = 22f;

		// The number of sample points to use when performing a collision hitscan for the beam.
		// More points theoretically leads to a higher quality result, but can cause more lag. 3 tends to be enough.
		private const int NumSamplePoints = 3;

		// How quickly the beam adjusts to sudden changes in length.
		// Every frame, the beam replaces this ratio of its current length with its intended length.
		// Generally you shouldn't need to change this.
		// Setting it too low will make the beam lazily pass through walls before being blocked by them.
		private const float BeamLengthChangeFactor = 0.75f;

		// The charge percentage required on the host prism for the beam to begin visual effects (e.g. impact dust).
		private const float VisualEffectThreshold = 0.1f;

		// Each Last Prism beam draws two lasers separately: an inner beam and an outer beam. This controls their opacity.
		private const float OuterBeamOpacityMultiplier = 0.75f;
		private const float InnerBeamOpacityMultiplier = 0.1f;

		// The maximum brightness of the light emitted by the beams. Brightness scales from 0 to this value as the Prism's charge increases.
		private const float BeamLightBrightness = 0.75f;

		// These variables control the beam's potential coloration.
		// As a value, hue ranges from 0f to 1f, both of which are pure red. The laser beams vary from 0.57 to 0.75, which winds up being a blue-to-purple gradient.
		// Saturation ranges from 0f to 1f and controls how greyed out the color is. 0 is fully grayscale, 1 is vibrant, intense color.
		// Lightness ranges from 0f to 1f and controls how dark or light the color is. 0 is pitch black. 1 is pure white.
		private const float BeamColorHue = 0.57f;
		private const float BeamHueVariance = 0.18f;
		private const float BeamColorSaturation = 0.66f;
		private const float BeamColorLightness = 0.53f;

		// This property encloses the internal AI variable projectile.ai[0]. It makes the code easier to read.
		private float BeamID
		{
			get => projectile.ai[0];
			set => projectile.ai[0] = value;
		}

		// This property encloses the internal AI variable projectile.ai[1].
		private float HostPrismIndex
		{
			get => projectile.ai[1];
			set => projectile.ai[1] = value;
		}

		// This property encloses the internal AI variable projectile.localAI[1].
		// Normally, localAI is not synced over the network. This beam manually syncs this variable using SendExtraAI and ReceiveExtraAI.
		private float BeamLength
		{
			get => projectile.localAI[1];
			set => projectile.localAI[1] = value;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Example Prism Beam");
		}

		public override void SetDefaults()
		{
			projectile.width = 18;
			projectile.height = 18;
			projectile.magic = true;
			projectile.penetrate = -1;
			projectile.alpha = 255;
			// The beam itself still stops on tiles, but its invisible "source" projectile ignores them.
			// This prevents the beams from vanishing if the player shoves the Prism into a wall.
			projectile.tileCollide = false;

			// Using local NPC immunity allows each beam to strike independently from one another.
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 10;
		}

		// Send beam length over the network to prevent hitbox-affecting and thus cascading desyncs in multiplayer.
		public override void SendExtraAI(BinaryWriter writer) => writer.Write(BeamLength);
		public override void ReceiveExtraAI(BinaryReader reader) => BeamLength = reader.ReadSingle();

		public override void AI()
		{
			// If something has gone wrong with either the beam or the host Prism, destroy the beam.
			Projectile hostPrism = Main.projectile[(int)HostPrismIndex];
			if (projectile.type != ModContent.ProjectileType<ExampleLastPrismBeam>() || !hostPrism.active || hostPrism.type != ModContent.ProjectileType<ExampleLastPrismHoldout>())
			{
				projectile.Kill();
				return;
			}

			// Grab some variables from the host Prism.
			Vector2 hostPrismDir = Vector2.Normalize(hostPrism.velocity);
			float chargeRatio = MathHelper.Clamp(hostPrism.ai[0] / ExampleLastPrismHoldout.MaxCharge, 0f, 1f); // hostPrism.ai[0] is the hostPrism's frame counter

			// Update the beam's damage every frame based on charge and the host Prism's damage.
			projectile.damage = (int)(hostPrism.damage * GetDamageMultiplier(chargeRatio));

			// The beam cannot strike enemies until the host Prism is at a certain charge level.
			projectile.friendly = hostPrism.ai[0] > ExampleLastPrismHoldout.DamageStart;

			// This offset is used to make each individual beam orient differently based on its Beam ID.
			float beamIdOffset = BeamID - ExampleLastPrismHoldout.NumBeams / 2f + 0.5f;
			float beamSpread;
			float spinRate;
			float beamStartSidewaysOffset;
			float beamStartForwardsOffset;

			// Variables scale smoothly while the host Prism is charging up.
			if (chargeRatio < 1f)
			{
				projectile.scale = MathHelper.Lerp(0f, MaxBeamScale, chargeRatio);
				beamSpread = MathHelper.Lerp(0f, MaxBeamSpread, chargeRatio);
				beamStartSidewaysOffset = MathHelper.Lerp(20f, 6f, chargeRatio);
				beamStartForwardsOffset = MathHelper.Lerp(-21f, -17f, chargeRatio);

				// For the first 2/3 of charge time, the opacity scales up from 0% to 40%.
				// Spin rate increases slowly during this time.
				if (chargeRatio <= 0.66f)
				{
					float phaseRatio = chargeRatio * 1.5f;
					projectile.Opacity = MathHelper.Lerp(0f, 0.4f, phaseRatio);
					spinRate = MathHelper.Lerp(20f, 16f, phaseRatio);
				}

				// For the last 1/3 of charge time, the opacity scales up from 40% to 100%.
				// Spin rate increases dramatically during this time.
				else
				{
					float phaseRatio = (chargeRatio - 0.66f) * 3f;
					projectile.Opacity = MathHelper.Lerp(0.4f, 1f, phaseRatio);
					spinRate = MathHelper.Lerp(16f, 12f, chargeRatio);
				}
			}

			// If the host Prism is already at max charge, don't calculate anything. Just use the max values.
			else
			{
				projectile.scale = MaxBeamScale;
				projectile.Opacity = 1f;
				beamSpread = MaxBeamSpread;
				spinRate = 12f;
				beamStartSidewaysOffset = 6f;
				beamStartForwardsOffset = -17f;
			}

			// The amount to which the angle changes reduces over time so that the beams look like they are focusing.
			float deviationAngle = ((hostPrism.ai[0]) + beamIdOffset * spinRate) / (spinRate * ExampleLastPrismHoldout.NumBeams) * MathHelper.TwoPi;
			Main.NewText("" + deviationAngle);

			// This trigonometry calculates where the beam is supposed to be pointing.
			Vector2 unitRot = Vector2.UnitY.RotatedBy(deviationAngle);
			Vector2 yVec = new Vector2(4f, beamStartSidewaysOffset);
			float hostPrismAngle = hostPrism.velocity.ToRotation();
			Vector2 beamSpanVector = (unitRot * yVec).RotatedBy(hostPrismAngle);
			float sinusoidYOffset = unitRot.Y * PiBeamDivisor * beamSpread;

			// Calculate the beam's emanating position. Start with the Prism's center.
			projectile.Center = hostPrism.Center;
			// Add a fixed offset to align with the Prism's sprite sheet.
			projectile.position += hostPrismDir * 16f + new Vector2(0f, -hostPrism.gfxOffY);
			// Add the forwards offset, measured in pixels.
			projectile.position += hostPrismDir * beamStartForwardsOffset;
			// Add the sideways offset vector, which is calculated for the current angle of the beam and scales with the beam's sideways offset.
			projectile.position += beamSpanVector;

			// Set the beam's velocity to point towards its current spread direction and sanity check it. It should have magnitude 1.
			projectile.velocity = hostPrismDir.RotatedBy(sinusoidYOffset);
			if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
			{
				projectile.velocity = -Vector2.UnitY;
			}
			projectile.rotation = projectile.velocity.ToRotation();

			// Update the beam's length by performing a hitscan collision check.
			float hitscanBeamLength = PerformBeamHitscan(hostPrism, chargeRatio >= 1f);
			BeamLength = MathHelper.Lerp(BeamLength, hitscanBeamLength, BeamLengthChangeFactor);

			// This Vector2 stores the beam's hitbox statistics. X = beam length. Y = beam width.
			Vector2 beamDims = new Vector2(projectile.velocity.Length() * BeamLength, projectile.width * projectile.scale);

			// Only produce dust and cause water ripples if the beam is above a certain charge level.
			Color beamColor = GetOuterBeamColor();
			if (chargeRatio >= VisualEffectThreshold)
			{
				ProduceBeamDust(beamColor);

			}

			// Make the beam cast light along its length. The brightness of the light scales with the charge.
			// v3_1 is an unnamed decompiled variable which is the color of the light cast by DelegateMethods.CastLight.
			DelegateMethods.v3_1 = beamColor.ToVector3() * BeamLightBrightness * chargeRatio;
			Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * BeamLength, beamDims.Y, new Utils.PerLinePoint(DelegateMethods.CastLight));
		}

		// Uses a simple polynomial (x^3) to get sudden but smooth damage increase near the end of the charge-up period.
		private float GetDamageMultiplier(float chargeRatio)
		{
			float f = chargeRatio * chargeRatio * chargeRatio;
			return MathHelper.Lerp(1f, MaxDamageMultiplier, f);
		}

		private float PerformBeamHitscan(Projectile prism, bool fullCharge)
		{
			// By default, the hitscan interpolation starts at the projectile's center.
			// If the host Prism is fully charged, the interpolation starts at the Prism's center instead.
			Vector2 samplingPoint = projectile.Center;
			if (fullCharge)
			{
				samplingPoint = prism.Center;
			}

			// Overriding that, if the player shoves the Prism into or through a wall, the interpolation starts at the player's center.
			// This last part prevents the player from projecting beams through walls under any circumstances.
			Player player = Main.player[projectile.owner];
			if (!Collision.CanHitLine(player.Center, 0, 0, prism.Center, 0, 0))
			{
				samplingPoint = player.Center;
			}

			// Perform a laser scan to calculate the correct length of the beam.
			// Alternatively, if you want the beam to ignore tiles, just set it to be the max beam length with the following line.
			// return MaxBeamLength;
			float[] laserScanResults = new float[NumSamplePoints];
			Collision.LaserScan(samplingPoint, projectile.velocity, BeamTileCollisionWidth * projectile.scale, MaxBeamLength, laserScanResults);
			float averageLengthSample = 0f;
			for (int i = 0; i < laserScanResults.Length; ++i)
			{
				averageLengthSample += laserScanResults[i];
			}
			averageLengthSample /= NumSamplePoints;

			return averageLengthSample;
		}

		// Determines whether the specified target hitbox is intersecting with the beam.
		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			// If the target is touching the beam's hitbox (which is a small rectangle vaguely overlapping the host Prism), that's good enough.
			if (projHitbox.Intersects(targetHitbox))
			{
				return true;
			}

			// Otherwise, perform an AABB line collision check to check the whole beam.
			float _ = float.NaN;
			Vector2 beamEndPos = projectile.Center + projectile.velocity * BeamLength;
			return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, beamEndPos, BeamHitboxCollisionWidth * projectile.scale, ref _);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			// If the beam doesn't have a defined direction, don't draw anything.
			if (projectile.velocity == Vector2.Zero)
			{
				return false;
			}

			Texture2D texture = Main.projectileTexture[projectile.type];
			Vector2 centerFloored = projectile.Center.Floor() + projectile.velocity * projectile.scale * 10.5f;
			Vector2 drawScale = new Vector2(projectile.scale);

			// Reduce the beam length proportional to its square area to reduce block penetration.
			float visualBeamLength = BeamLength - 14.5f * projectile.scale * projectile.scale;

			DelegateMethods.f_1 = 1f; // f_1 is an unnamed decompiled variable whose function is unknown. Leave it at 1.
			Vector2 startPosition = centerFloored - Main.screenPosition;
			Vector2 endPosition = startPosition + projectile.velocity * visualBeamLength;

			// Draw the outer beam.
			DrawBeam(spriteBatch, texture, startPosition, endPosition, drawScale, GetOuterBeamColor() * OuterBeamOpacityMultiplier * projectile.Opacity);

			// Draw the inner beam, which is half size.
			drawScale *= 0.5f;
			DrawBeam(spriteBatch, texture, startPosition, endPosition, drawScale, GetInnerBeamColor() * InnerBeamOpacityMultiplier * projectile.Opacity);

			// Returning false prevents Terraria from trying to draw the projectile itself.
			return false;
		}

		private void DrawBeam(SpriteBatch spriteBatch, Texture2D texture, Vector2 startPosition, Vector2 endPosition, Vector2 drawScale, Color beamColor)
		{
			Utils.LaserLineFraming lineFraming = new Utils.LaserLineFraming(DelegateMethods.RainbowLaserDraw);

			// c_1 is an unnamed decompiled variable which is the render color of the beam drawn by DelegateMethods.RainbowLaserDraw.
			DelegateMethods.c_1 = beamColor;
			Utils.DrawLaser(spriteBatch, texture, startPosition, endPosition, drawScale, lineFraming);
		}

		private Color GetOuterBeamColor()
		{
			// This hue calculation produces a unique color for each beam based on its Beam ID.
			float hue = (BeamID / ExampleLastPrismHoldout.NumBeams) % BeamHueVariance + BeamColorHue;

			// Main.hslToRgb converts Hue, Saturation, Lightness into a Color for general purpose use.
			Color c = Main.hslToRgb(hue, BeamColorSaturation, BeamColorLightness);

			// Manually reduce the opacity of the color so beams can overlap without completely overwriting each other.
			c.A = 64;
			return c;
		}

		// Inner beams are always pure white so that they act as a "blindingly bright" center to each laser.
		private Color GetInnerBeamColor() => Color.White;

		private void ProduceBeamDust(Color beamColor)
		{
			// Create one dust per frame a small distance from where the beam ends.
			const int type = 15;
			Vector2 endPosition = projectile.Center + projectile.velocity * (BeamLength - 14.5f * projectile.scale);

			// Main.rand.NextBool is used to give a 50/50 chance for the angle to point to the left or right.
			// This gives the dust a 50/50 chance to fly off on either side of the beam.
			float angle = projectile.rotation + (Main.rand.NextBool() ? 1f : -1f) * MathHelper.PiOver2;
			float startDistance = Main.rand.NextFloat(1f, 1.8f);
			float scale = Main.rand.NextFloat(0.7f, 1.1f);
			Vector2 velocity = angle.ToRotationVector2() * startDistance;
			Dust dust = Dust.NewDustDirect(endPosition, 0, 0, type, velocity.X, velocity.Y, 0, beamColor, scale);
			dust.color = beamColor;
			dust.noGravity = true;

			// If the beam is currently large, make the dust faster and larger to match.
			if (projectile.scale > 1f)
			{
				dust.velocity *= projectile.scale;
				dust.scale *= projectile.scale;
			}
		}

		// Automatically iterates through every tile the laser is overlapping to cut grass at all those locations.
		public override void CutTiles()
		{
			// tilecut_0 is an unnamed decompiled variable which tells CutTiles how the tiles are being cut (in this case, via a projectile).
			DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
			Utils.PerLinePoint cut = new Utils.PerLinePoint(DelegateMethods.CutTiles);
			Vector2 beamStartPos = projectile.Center;
			Vector2 beamEndPos = beamStartPos + projectile.velocity * BeamLength;

			// PlotTileLine is a function which performs the specified action to all tiles along a drawn line, with a specified width.
			// In this case, it is cutting all tiles which can be destroyed by projectiles, for example grass or pots.
			Utils.PlotTileLine(beamStartPos, beamEndPos, projectile.width * projectile.scale, cut);
		}
	}
}
