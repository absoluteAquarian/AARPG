using AARPG.API.UI;
using Microsoft.Xna.Framework;
using System;
using System.Reflection;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace AARPG.Core.UI.NPCStats{
	public class NonModifierStatElement<T> : UIPanel where T : struct{
		private UIText text;
		private NewUITextBox prompt;

		public T data;

		public readonly FieldInfo statField;

		public event Action<T> OnDataChange;
		public delegate bool InputIsValidDelegate(string input, out T value);
		public event InputIsValidDelegate CheckInputValidity;
		public Func<string> GetTextOnInvalidInput;
		public Func<T, string> GetTextOnValidInput;

		private readonly string textHeader;
		private readonly string hintText;
		private readonly string defaultText;

		public NonModifierStatElement(string textHeader, string hintText, string defaultText, FieldInfo statField){
			this.textHeader = textHeader;
			this.hintText = hintText;
			this.defaultText = defaultText;
			this.statField = statField;

			//Invisible panel
			base.BackgroundColor = Color.Transparent;
			base.BorderColor = Color.Transparent;
			SetPadding(0);
			MarginTop = MarginLeft = MarginRight = MarginBottom = 0;
		}

		public override void OnInitialize(){
			text = new UIText(textHeader);
			text.Left.Set(0, 0);
			text.Top.Set(0, 0);
			DepadChildThenAppend(text);

			prompt = new NewUITextBox(hintText);
			prompt.Left.Set(0, 0);
			prompt.Top.Set(text.Top.Pixels + text.Height.Pixels + 8, 0);
			prompt.unfocusOnTab = false;
			prompt.OnUnfocus += () => {
				T value = default;
				if(CheckInputValidity?.Invoke(prompt.currentString, out value) ?? false){
					data = value;

					string text = GetTextOnValidInput?.Invoke(data);
					if(text is not null)
						prompt.SetText(text);
				}else{
					data = default;
					prompt.SetText(GetTextOnInvalidInput?.Invoke() ?? "");
				}

				OnDataChange?.Invoke(data);
			};
			prompt.SetText(defaultText ?? "");
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
