using AARPG.Core.Mechanics;
using AARPG.Core.Players;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace AARPG.API.Commands{
	[Autoload(!CoreMod.Release)]
	public class ZerLevel : ModCommand{
		public override string Command => "zlvl";

		public override CommandType Type => CommandType.Chat;

		public override string Usage => "[c/ff6a00:Usage: /zlvl]";

		public override string Description => "Resets your level and XP to zero";

		public override void Action(CommandCaller caller, string input, string[] args){
			if(args.Length > 0){
				caller.Reply("Command expected no arguments", Color.Red);
				return;
			}

			StatPlayer plr = caller.Player.GetModPlayer<StatPlayer>();
			plr.stats = new PlayerStatistics();

			caller.Reply("RPG stats reset");
		}
	}
}
