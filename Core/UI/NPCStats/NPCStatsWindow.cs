using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace AARPG.Core.UI.NPCStats{
	public class NPCStatsWindow : UIPanel{
		private readonly BestiaryEntry bestiaryEntry;
		private UIElement entryPortrait;

		public NPCStatsWindow(int netID) : base(){
			//Invisible panel
			base.BackgroundColor = Color.Transparent;
			base.BorderColor = Color.Transparent;
			SetPadding(0);
			MarginTop = MarginLeft = MarginRight = MarginBottom = 0;

			bestiaryEntry = Main.BestiaryDB.FindEntryByNPCID(netID);
			
			//Invalid entry
			if(bestiaryEntry.Info.Count == 0)
				throw new ArgumentException("Unknown NPC net ID detected: " + netID);
		}

		public override void OnInitialize(){
			foreach(var info in bestiaryEntry.Info){
				if(info is NPCPortraitInfoElement portrait){
					entryPortrait = portrait.ProvideUIElement(new BestiaryUICollectionInfo(){
						OwnerEntry = bestiaryEntry,
						UnlockState = BestiaryEntryUnlockState.CanShowPortraitOnly_1
					});

					break;
				}
			}

			if(entryPortrait is null)
				throw new InvalidOperationException("Invalid bestiary entry");

			entryPortrait.Left.Set(0, 0);
			entryPortrait.Top.Set(0, 0);
			Append(entryPortrait);
		}
	}
}
