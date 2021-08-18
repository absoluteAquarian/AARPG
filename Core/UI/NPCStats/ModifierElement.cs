using AARPG.API.UI;
using AARPG.Core.Mechanics;
using Microsoft.Xna.Framework;
using System;
using System.Reflection;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace AARPG.Core.UI.NPCStats{
	public class ModifierElement : UIPanel{
		private UIText name;
		private UIHorizontalSeparator separator;

		private UIText textAdd;
		private NewUITextBox promptAdd;
		private UIText textMult;
		private NewUITextBox promptMult;
		private UIText textFlat;
		private NewUITextBox promptFlat;

		private readonly string modifierName;
		public readonly FieldInfo statField;

		public Modifier modifier = Modifier.Default;

		public event Action<Modifier> OnModifierChange;

		public ModifierElement(string modifierName, FieldInfo statField) : base(){
			this.modifierName = modifierName;
			this.statField = statField;

			//Invisible panel
			base.BackgroundColor = Color.Transparent;
			base.BorderColor = Color.Transparent;
			SetPadding(0);
			MarginTop = MarginLeft = MarginRight = MarginBottom = 0;
		}

		public override void OnInitialize(){
			name = new UIText(modifierName);
			name.Left.Set(0, 0);
			name.Top.Set(0, 0);
			DepadChildThenAppend(name);

			separator = new UIHorizontalSeparator(highlightSideUp: true);
			separator.Width.Set(0, 1);
			separator.Left.Set(0, 0);
			separator.Top.Set(name.Top.Pixels + name.Height.Pixels + 8, 0);
			DepadChildThenAppend(separator);

			InitializeMemberPrompt(separator.Top.Pixels + separator.Height.Pixels + 14, "Add:", "0", ref textAdd, ref promptAdd, () => {
				if(float.TryParse(promptAdd.currentString, out float f) && f >= 0){
					modifier.add = f;

					if(modifier.add < 1E-6)
						promptAdd.SetText($"{modifier.add:#.######E+0}");  //Convert to scientific notation
					else
						promptAdd.SetText(modifier.add.ToString());
				}else{
					modifier.add = Modifier.Default.add;
					promptAdd.SetText(modifier.add.ToString());
				}

				OnModifierChange?.Invoke(modifier);
			});
			InitializeMemberPrompt(promptAdd.Top.Pixels + promptAdd.Height.Pixels + 8, "Mult:", "1", ref textMult, ref promptMult, () => {
				if(float.TryParse(promptMult.currentString, out float f) && f >= 0){
					modifier.mult = f;

					if(modifier.mult < 1E-6)
						promptMult.SetText($"{modifier.mult:#.######E+0}");  //Convert to scientific notation
					else
						promptMult.SetText(modifier.mult.ToString());
				}else{
					modifier.mult = Modifier.Default.mult;
					promptMult.SetText(modifier.mult.ToString());
				}

				OnModifierChange?.Invoke(modifier);
			});
			InitializeMemberPrompt(promptMult.Top.Pixels + promptMult.Height.Pixels + 8, "Flat:", "0", ref textFlat, ref promptFlat, () => {
				if(float.TryParse(promptFlat.currentString, out float f) && f >= 0){
					modifier.flat = f;

					if(modifier.add < 1E-6)
						promptFlat.SetText($"{modifier.flat:#.######E+0}");  //Convert to scientific notation
					else
						promptFlat.SetText(modifier.flat.ToString());
				}else{
					modifier.flat = Modifier.Default.flat;
					promptFlat.SetText(modifier.flat.ToString());
				}

				OnModifierChange?.Invoke(modifier);
			});

			Height.Set(promptFlat.Top.Pixels + promptFlat.Height.Pixels, 0);
		}

		private void InitializeMemberPrompt(float anchorY, string textContent, string defaultText, ref UIText text, ref NewUITextBox prompt, Action onLoseFocus){
			text = new UIText(textContent);
			text.Left.Set(0, 0);
			text.Top.Set(anchorY, 0);
			DepadChildThenAppend(text);

			prompt = new NewUITextBox("floating-point number...");
			prompt.Left.Set(0, 0);
			prompt.Top.Set(text.Top.Pixels + text.Height.Pixels + 8, 0);
			prompt.unfocusOnTab = false;
			prompt.OnUnfocus += onLoseFocus;
			prompt.SetText(defaultText);
			DepadChildThenAppend(prompt);
		}

		private void DepadChildThenAppend(UIElement child){
			child.SetPadding(0);
			child.MarginTop = child.MarginLeft = child.MarginRight = child.MarginBottom = 0;
			Append(child);
			child.Recalculate();
		}
	}
}
