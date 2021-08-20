using System.Text.RegularExpressions;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace AARPG.Core.Players{
	public class MessagePlayer : ModPlayer{
		public static readonly Regex NPCLevelIndicatorRegex = new Regex(@"\[[^\]]+\] (.+)", RegexOptions.Compiled);

		public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource){
			//Oops, an NPC's given name will have extraneous data here.  Remove that
			//[Lv. 1] Tiny Green Slime --> Tiny Green Slime
			if(damageSource.SourceNPCIndex >= 0){
				NPC source = Main.npc[damageSource.SourceNPCIndex];

				Match match = NPCLevelIndicatorRegex.Match(source.GivenName);
				if(match.Success){
					//Replace the NPC's given name to not include the level
					source.GivenName = match.Groups[0].Value;
				}
			}

			return true;
		}
	}
}
