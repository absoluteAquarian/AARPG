using AARPG.Core.UI.NPCStats;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace AARPG.Core.Systems{
	public class InterfaceSystem : ModSystem{
		public static bool LeftClick => curMouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released;

		private static MouseState oldMouse;
		private static MouseState curMouse;

		internal static NPCStatsState debugNPCStats;
		internal static UserInterface debugNPCInterface;

		public static void LoadStatic(){
			if(!Main.dedServ){
				debugNPCInterface = new();
				debugNPCStats = new();

				debugNPCInterface.SetState(debugNPCStats);
				debugNPCStats.Activate();
			}
		}

		public static void UnloadStatic(){
			debugNPCInterface = null;
			debugNPCStats = null;
		}

		public override void UpdateUI(GameTime gameTime){
			oldMouse = curMouse;
			curMouse = Mouse.GetState();

			if(debugNPCStats.Active)
				debugNPCInterface.Update(gameTime);
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers){
			int idx = layers.FindIndex(layer => layer.Name == "Vanilla: Mouse Text");
			if(idx >= 0){
				layers.Insert(idx + 1, new LegacyGameInterfaceLayer("AARPG: Debug NPC Stats",
					() => {
						if(debugNPCStats.Active)
							debugNPCInterface.Draw(Main.spriteBatch, new GameTime());
						return true;
					}, InterfaceScaleType.UI));
			}
		}
	}
}
