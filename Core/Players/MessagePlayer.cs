using AARPG.Core.NPCs;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace AARPG.Core.Players{
	public class MessagePlayer : ModPlayer{
		public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource){
			//Oops, an NPC's given name will have extraneous data here.  Remove that
			//[Lv. 1] Tiny Green Slime --> Tiny Green Slime
			if(damageSource.SourceNPCIndex >= 0){
				NPC source = Main.npc[damageSource.SourceNPCIndex];

				if(source.TryGetGlobalNPC<StatNPC>(out var stats))
					stats.ApplyNamePrefix(source, source.netID, prependLevel: false);
			}

			return true;
		}
	}
}
