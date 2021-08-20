using AARPG.Core.Mechanics;
using AARPG.Core.NPCs;

namespace AARPG.API.Edits.Detours{
	internal static partial class Vanilla{
		public static int NetIDOverride{ get; private set; }

		public static bool TransformingNPC{ get; private set; }

		public static NPCStatistics PreTransformStats{ get; private set; }

		public static string PreTransformNamePrefix{ get; private set; }

		public static void NPC_SetDefaultsFromNetId(On.Terraria.NPC.orig_SetDefaultsFromNetId orig, Terraria.NPC self, int id, Terraria.NPCSpawnParams spawnparams){
			//Due to some bad API code, NPCLoader.SetDefaults isn't given the netID properly, so let's do that properly
			NetIDOverride = id;

			orig(self, id, spawnparams);

			NetIDOverride = 0;
		}

		public static void NPC_Transform(On.Terraria.NPC.orig_Transform orig, Terraria.NPC self, int newType){
			//Force the NPC's StatNPC to not update its registry entry
			TransformingNPC = true;

			if(self.TryGetGlobalNPC(out StatNPC stat)){
				PreTransformStats = stat.stats;
				PreTransformNamePrefix = stat.namePrefix;
			}

			orig(self, newType);

			PreTransformStats = null;
			PreTransformNamePrefix = null;

			TransformingNPC = false;
		}
	}
}
