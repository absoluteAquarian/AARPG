using AARPG.API.UI;
using AARPG.Core.Mechanics;
using AARPG.Core.Systems;
using System.Linq;
using System.Reflection;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace AARPG.Core.UI.NPCStats{
	public class NPCStatsState : UIState{
		private NPCStatisticsRegistry.Entry currentEntry;

		private UIDragablePanel panel;
		private NPCStatsWindow npcWindow;

		//private VisibilityElement<BetterUIList> statsMenu;
		private BetterUIList statsMenu;
		private UIScrollbar scrollBar;

		public bool Active => !CoreMod.Release && ToggleActive;

		private bool toggleActive;
		internal bool ToggleActive{
			get => toggleActive;
			set{
				if(toggleActive != value){
					if(!value)
						panel?.RemoveChild(statsMenu);
					else
						panel.Append(statsMenu);

					toggleActive = value;
				}
			}
		}

		public override void OnInitialize(){
			const int width = 650;
			const int height = 500;

			panel = new UIDragablePanel("NPC Statistics");
			panel.Width.Set(width, 0);
			panel.Height.Set(height, 0);
			panel.Left.Set(panel.lastPos.X, 0);
			panel.Top.Set(panel.lastPos.Y, 0);
			panel.OnMenuClose += () => ToggleActive = false;
			Append(panel);

			UIVerticalSeparator separator = new UIVerticalSeparator();
			separator.Height.Set(0, 0.8f);
			separator.Left.Set(0, 0.5f);
			separator.Top.Set(0, 0.15f);
			panel.Append(separator);

			//Initialize NPC Stats window
			scrollBar = new UIScrollbar();
			scrollBar.Top.Set(0, 0.1f);
			scrollBar.Left.Set(0, 0.95f);
			scrollBar.Width.Set(20, 0);
			scrollBar.Height.Set(0, 0.825f);

			var horizontalScrollBar = new UIHorizontalScrollbar();
			horizontalScrollBar.Top.Set(0, 0.95f);
			horizontalScrollBar.Left.Set(20, 0);
			horizontalScrollBar.Width.Set(0, 0.825f);
			horizontalScrollBar.Height.Set(20, 0);

			statsMenu = new BetterUIList();
			statsMenu.SetScrollbar(scrollBar);
			statsMenu.Append(scrollBar);
			statsMenu.SetHorizontalScrollbar(horizontalScrollBar);
			statsMenu.Append(horizontalScrollBar);
			statsMenu.ListPadding = 10;
			statsMenu.Top.Set(0, 0.1f);
			statsMenu.Left.Set(0, 0.575f);
			statsMenu.Width.Set(0, 0.475f);
			statsMenu.Height.Set(0, 0.825f);
			panel.Append(statsMenu);
			/*
			statsMenu = new VisibilityElement<BetterUIList>(subMenu, updateWhenNotVisible: false){
				Visible = false
			};
			statsMenu.Append(subMenu);
			*/

			//Add the non-modifier fields
			NonModifierStatElement<int> levelElement = new("Level:", "positive integer...", "0", typeof(NPCStatistics).GetField("level"));
			levelElement.OnDataChange += data => currentEntry.stats.level = data;
			levelElement.CheckInputValidity += (string input, out int value) => int.TryParse(input, out value) && value >= 0;
			levelElement.GetTextOnInvalidInput += () => "0";
			levelElement.GetTextOnValidInput += data => data.ToString();
			statsMenu.Add(levelElement);

			NonModifierStatElement<int> xpElement = new("XP on Kill:", "positive integer...", "0", typeof(NPCStatistics).GetField("level"));
			xpElement.OnDataChange += data => currentEntry.stats.xp = data;
			xpElement.CheckInputValidity += (string input, out int value) => int.TryParse(input, out value) && value >= 0;
			xpElement.GetTextOnInvalidInput += () => "0";
			xpElement.GetTextOnValidInput += data => data.ToString();
			statsMenu.Add(xpElement);

			//Add the modifiers
			var modifiers = typeof(NPCStatistics).GetFields(BindingFlags.Public | BindingFlags.Instance).Where(f => f.FieldType == typeof(Modifier));
			foreach(var mod in modifiers){
				ModifierElement element = new ModifierElement(mod.Name + ":", mod);
				element.OnModifierChange += modifier => element.statField.SetValue(currentEntry.stats, modifier);
				statsMenu.Add(element);
			}

			//Add toggles for the default conditions
			foreach(var key in NPCStatisticsRegistry.conditions.Keys){
				//If the name is too long, ignore it and log the problem to the log file
				if(key.Length > 35){
					CoreMod.Instance.Logger.Warn($"NPC Statistics Condition \"{key}\" had a name which exceeded 35 characters");
					continue;
				}

				EntryConditionElement element = new EntryConditionElement(key);
				statsMenu.Add(element);
			}
		}

		public void SetNPCWindow(NPCStatisticsRegistry.Entry entry){
			if(npcWindow is not null)
				npcWindow.Remove();

			currentEntry = entry;

			//Initialize NPC Window
			npcWindow = new NPCStatsWindow(currentEntry.sourceID);
			npcWindow.Left.Set(20, 0);
			npcWindow.Top.Set(0.05f, 0);
			npcWindow.Height.Set(0, 0.825f);
			panel.Append(npcWindow);
			npcWindow.Activate();

			//statsMenu.Visible = true;
			ToggleActive = true;
		}
	}
}
