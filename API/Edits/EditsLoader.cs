namespace AARPG.API.Edits{
	internal static class EditsLoader{
		public static void Load(){
			On.Terraria.NPC.SetDefaultsFromNetId += Detours.Vanilla.NPC_SetDefaultsFromNetId;
		}
	}
}
