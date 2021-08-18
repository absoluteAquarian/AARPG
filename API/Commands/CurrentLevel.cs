using AARPG.Core.Mechanics;
using AARPG.Core.Players;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace AARPG.API.Commands{
	[Autoload(!CoreMod.Release)]
	public class CurrentLevel : ModCommand{
		public override string Command => "lvl";

		public override CommandType Type => CommandType.Chat;

		public override string Usage => "[c/ff6a00:Usage: /lvl]";

		public override string Description => "Displays your current level and XP";

		public override void Action(CommandCaller caller, string input, string[] args){
			if(args.Length > 0){
				caller.Reply("Command expected no arguments", Color.Red);
				return;
			}

			StatPlayer plr = caller.Player.GetModPlayer<StatPlayer>();

			caller.Reply($"Stats: Lv. {plr.stats.level}, XP: {plr.stats.xp} / {PlayerStatistics.XPRequirementsPerLevel[plr.stats.level]} (Total: {plr.stats.XpTotal})");
		}
	}
}
