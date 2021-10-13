using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace AARPG.Core.UI.NPCStats{
	public class EntryConditionElement : UIElement{
		public bool ConditionSet{ get; private set; }

		public event Action<bool> OnConditionToggle;

		public readonly string conditionName;

		public EntryConditionElement(string conditionName){
			if(conditionName.Length > 35)
				throw new ArgumentException($"Condition name \"{conditionName}\" was too long");

			this.conditionName = conditionName;

			Width.Set(0, 1f);
			Height.Set(20, 0);

			SetPadding(0);
			MarginTop = MarginLeft = MarginRight = MarginBottom = 0;
		}

		public override void OnInitialize(){
			UIText text = new UIText(conditionName);
			text.Top.Set(5, 0);
			Append(text);

			UIToggleImage toggle = new UIToggleImage(Main.Assets.Request<Texture2D>("Images\\UI\\Settings_Toggle"), 13, 13, new Point(17, 1), new Point(1, 1));
			toggle.SetState(false);
			toggle.Left.Set(text.Width.GetValue(Width.Pixels) + 8, 0);
			toggle.Top.Set(6, 0);
			toggle.OnClick += (evt, element) => {
				ConditionSet = !ConditionSet;
				OnConditionToggle?.Invoke(ConditionSet);
			};
			Append(toggle);
		}
	}
}
