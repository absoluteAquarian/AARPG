namespace AARPG.API.Edits.Detours{
	internal static partial class Vanilla{
		public static int NetIDOverride;

		public static void NPC_SetDefaultsFromNetId(On.Terraria.NPC.orig_SetDefaultsFromNetId orig, Terraria.NPC self, int id, Terraria.NPCSpawnParams spawnparams){
			//Due to some bad API code, NPCLoader.SetDefaults isn't given the netID properly, so let's do that properly
			NetIDOverride = id;

			orig(self, id, spawnparams);

			NetIDOverride = 0;
		}
	}
}
