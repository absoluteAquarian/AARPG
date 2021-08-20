namespace AARPG.API.Edits{
	internal static class EditsLoader{
		public static void Load(){
			On.Terraria.NPC.SetDefaultsFromNetId += Detours.Vanilla.NPC_SetDefaultsFromNetId;
			On.Terraria.NPC.Transform += Detours.Vanilla.NPC_Transform;

			On.Terraria.Player.KillMe += Detours.Vanilla.Player_KillMe;
		}
	}
}
