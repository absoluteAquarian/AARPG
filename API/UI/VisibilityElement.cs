using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.UI;

namespace AARPG.API.UI{
	// TODO: doesn't work properly
	public class VisibilityElement<T> : UIElement where T : UIElement{
		public readonly T Actual;

		public bool Visible;

		public readonly bool UpdateWhenNotVisible;

		public VisibilityElement(T actual, bool updateWhenNotVisible){
			Actual = actual;
			UpdateWhenNotVisible = updateWhenNotVisible;

			Visible = true;

			SetPadding(0);
			MarginTop = MarginLeft = MarginRight = MarginBottom = 0;
		}

		public override void OnInitialize(){
			if(Actual.Parent is not null && Actual.Parent != this)
				throw new InvalidOperationException("Child UIElement is expected to not be appended to another UIElement during initialization");

			Append(Actual);
		}

		public override void Recalculate(){
			if(Parent is not null){
				Width = new(0, 1f);
				Height = new(0, 1f);
				Left = new(0, 0);
				Top = new(0, 0);
			}
			base.Recalculate();
		}

		public override void Update(GameTime gameTime){
			if(Visible || UpdateWhenNotVisible)
				base.Update(gameTime);
		}

		public override void Draw(SpriteBatch spriteBatch){
			if(Visible)
				base.Draw(spriteBatch);
		}
	}
}
