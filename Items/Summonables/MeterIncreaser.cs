using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BasicMod.Items.Summonables
{
    public class MeterIncreaser : ModItem
    {
        public override string Texture => "Terraria/Item_" + ItemID.PumpkinMoonMedallion;
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 52;
            item.maxStack = 20;
            item.rare = 6;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = 4;
            item.UseSound = SoundID.Item44;
            item.consumable = true;
        }

        public override bool CanUseItem(Player player)
        {
            return true;
        }

        int changeValue = 10;

        public override bool UseItem(Player player)
        {
            /**
            int num = CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), Color.Black, "Dark");
            CombatText combatText = Main.combatText[num];
            combatText.scale = 6f;

            if (Main.netMode == 2 && num != 100)
            {
                NetMessage.SendData(81, -1, -1, null, (int)combatText.color.PackedValue, combatText.position.X, combatText.position.Y);
            }
            **/

            WorldMeter.ChangeMeter(5);


            Main.dayTime = !Main.dayTime;
            Main.moonPhase = 6;
            Main.time = 0;


            return true;

        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var countdownTooltip = new TooltipLine(mod, "WorldMeter", $"It says {BasicWorld.worldMeter}s on it...");
            countdownTooltip.overrideColor = Color.OrangeRed;
            tooltips.Add(countdownTooltip);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(1006, 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}