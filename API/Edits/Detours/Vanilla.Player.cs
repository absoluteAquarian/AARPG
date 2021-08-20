using AARPG.Core.NPCs;
using Terraria;

namespace AARPG.API.Edits.Detours{
	internal static partial class Vanilla{
		internal static void Player_KillMe(On.Terraria.Player.orig_KillMe orig, Player self, Terraria.DataStructures.PlayerDeathReason damageSource, double dmg, int hitDirection, bool pvp){
			orig(self, damageSource, dmg, hitDirection, pvp);

			if(damageSource.SourceNPCIndex >= 0){
				//Revert the NPC's given name to what it should be
				NPC source = Main.npc[damageSource.SourceNPCIndex];

				if(source.TryGetGlobalNPC(out StatNPC stat) && stat.stats is not null){
					//NPC's given name has the level removed.  Add it back
					source.GivenName = $"[Lv. {stat.stats.level}] " + source.GivenName;
				}
			}
		}
	}
}
