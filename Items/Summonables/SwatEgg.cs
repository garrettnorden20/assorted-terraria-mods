using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BasicMod.Items.Summonables
{
    public class SwatEgg : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Summons the Swat Militia" + "\nThey never died out!");
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
            if (BasicWorld.SwatEvent)
            {
                return false;
            }
            return true;
        }

        public override bool UseItem(Player player)
        {
            string key = "Mods.BasicMod.SwatEventStart";
            Color messageColor = Color.Orange;
            if (Main.netMode == 2) // Server
            {
                NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
            }
            else if (Main.netMode == 0) // Single Player
            {
                Main.NewText(Language.GetTextValue(key), messageColor);
            }

            if (Main.netMode == 0)
            {
                Main.PlaySound(SoundID.Roar, player.position, 0);
                BasicWorld.SwatEvent = true;
            }
            if (Main.netMode == 1 && player.whoAmI == Main.myPlayer)
            {
                ModPacket packet = mod.GetPacket();
                packet.Write((byte)BasicModMessageType.StartSwatEvent);
                packet.Send();
            }

            return true;
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