using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace AARPG.Core.UI.NPCStats{
	public class NPCStatsWindow : UIPanel{
		private readonly BestiaryEntry bestiaryEntry;
		private UIBestiaryEntryInfoPage display;

		public NPCStatsWindow(int netID) : base(){
			//Invisible panel
			base.BackgroundColor = Color.Transparent;
			base.BorderColor = Color.Transparent;
			SetPadding(0);
			MarginTop = MarginLeft = MarginRight = MarginBottom = 0;

			//Make a copy of what the final result would be, rather than access the entry itself
			var clone = BestiaryEntry.Enemy(netID);
			var npc = new NPC();
			npc.SetDefaults(netID);
			bestiaryEntry = new BestiaryEntry();
			bestiaryEntry.Info.AddRange(
				from info in new List<IBestiaryInfoElement>(clone.Info)
				where info is NPCNetIdBestiaryInfoElement or NamePlateInfoElement or NPCPortraitInfoElement
				select info);
			bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(npc.GetBestiaryCreditId(), quickUnlock: true);
			bestiaryEntry.Icon = clone.Icon;
		}

		public override void OnInitialize(){
			display = new UIBestiaryEntryInfoPage();
			display.FillInfoForEntry(bestiaryEntry, default);

			display.Left.Set(0, 0);
			display.Top.Set(0, 0);
			display.Width.Set(0, 0.45f);
			display.Height.Set(0, 0.9f);
			Append(display);
		}
	}
}
