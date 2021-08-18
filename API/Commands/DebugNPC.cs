using AARPG.Core.Systems;
using AARPG.Core.UI.NPCStats;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace AARPG.API.Commands{
	[Autoload(!CoreMod.Release)]
	public class DebugNPC : ModCommand{
		public override string Command => "debugnpc";

		public override CommandType Type => CommandType.Chat;

		public override string Usage => "[c/ff6a00:Usage: /debugnpc <net ID> <stats name>]";

		public override string Description => "Opens the NPC Statistics debug menu";

		public override void Action(CommandCaller caller, string input, string[] args){
			if(args.Length < 2){
				caller.Reply("Too few arguments specified", Color.Red);
				return;
			}

			if(args.Length > 2){
				caller.Reply("Too many arguments specified", Color.Red);
				return;
			}

			if(!int.TryParse(args[0], out int netID)){
				caller.Reply("Expected an integer argument for the net ID.", Color.Red);
				return;
			}

			string name = args[1];
			if(name == "null")
				name = null;

			if(!NPCStatisticsRegistry.TryGetEntry(netID, name, out var entry)){
				caller.Reply($"NPC \"{Lang.GetNPCNameValue(netID)}\" does not have {(name is null ? "a default entry" : $"an entry named \"{name}\"")}", Color.Red);
				return;
			}

			InterfaceSystem.debugNPCStats.SetNPCWindow(entry);
		}
	}
}
