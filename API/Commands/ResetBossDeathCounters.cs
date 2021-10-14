using AARPG.Core.Players;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace AARPG.API.Commands{
	[Autoload(!CoreMod.Release)]
	public class ResetBossDeathCounters : ModCommand{
		public override string Command => "rbc";

		public override CommandType Type => CommandType.Chat;

		public override string Usage => "[c/ff6a00:Usage: /rbc]";

		public override string Description => "Resets the XP penalties on all bosses for successive boss kills";

		public override void Action(CommandCaller caller, string input, string[] args){
			if(args.Length > 0){
				caller.Reply("Command expected no arguments", Color.Red);
				return;
			}

			StatPlayer plr = caller.Player.GetModPlayer<StatPlayer>();

			plr.downedCountsByID.Clear();

			caller.Reply("Boss XP penalties cleared!", Color.Green);
		}
	}
}
