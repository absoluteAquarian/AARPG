using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace AARPG.API.UI{
	//Shamelessly copied some of this code from CreativeTools
	//Maybe I could use it in TerraScience as well?
	public class UIDragablePanel : UIPanel{
		//Stores the offset from the top left of the UIPanel while dragging.
		private Vector2 Offset{ get; set; }

		public bool Dragging{ get; set; }

		public bool StopItemUse{ get; private set; }

		internal Vector2 lastPos = new Vector2(600, 400);

		public event Action OnMenuClose;
		internal UIPanel header;

		public UIDragablePanel(string headerText, bool stopItemUse = true){
			StopItemUse = stopItemUse;
			SetPadding(0);

			header = new UIPanel();
			header.SetPadding(0);
			header.Height.Set(30, 0f);
			header.BackgroundColor.A = 255;
			header.OnMouseDown += Header_MouseDown;
			header.OnMouseUp += Header_MouseUp;
			Append(header);

			var heading = new UIText(headerText, 0.9f){
				VAlign = 0.5f,
				MarginLeft = 16f
			};
			header.Append(heading);

			var closeButton = new UITextPanel<char>('X');
			closeButton.SetPadding(7);
			closeButton.Width.Set(40, 0);
			closeButton.Left.Set(-40, 1);
			closeButton.BackgroundColor.A = 255;
			closeButton.OnClick += (evt, element) => OnMenuClose?.Invoke();
			header.Append(closeButton);
		}

		public override void OnInitialize(){
			base.OnInitialize();
			header.Width = Width;
		}

		public override void Recalculate(){
			base.Recalculate();
			header.Width = Width;
		}

		public void Header_MouseDown(UIMouseEvent evt, UIElement element){
			base.MouseDown(evt);

			Offset = new Vector2(evt.MousePosition.X - Left.Pixels, evt.MousePosition.Y - Top.Pixels);
			Dragging = true;
		}

		public void Header_MouseUp(UIMouseEvent evt, UIElement element){
			base.MouseUp(evt);

			//A child element forced this to not move
			if(!Dragging)
				return;

			Dragging = false;

			Left.Set(evt.MousePosition.X - Offset.X, 0f);
			Top.Set(evt.MousePosition.Y - Offset.Y, 0f);

			Recalculate();
		}

		public override void Update(GameTime gameTime) {
			//Don't remove
			base.Update(gameTime);

			//Clicks on this UIElement dont cause the player to use current items. 
			if(ContainsPoint(Main.MouseScreen) && StopItemUse)
				Main.LocalPlayer.mouseInterface = true;

			if(Dragging){
				//Main.MouseScreen.X and Main.mouseX are the same.
				Left.Set(Main.mouseX - Offset.X, 0f);
				Top.Set(Main.mouseY - Offset.Y, 0f);
				Recalculate();

				lastPos = new Vector2(Left.Pixels, Top.Pixels);
			}

			//Here we check if the UIDragablePanel is outside the Parent UIElement rectangle. 
			//By doing this and some simple math, we can snap the panel back on screen if the user resizes his window or otherwise changes resolution.
			var parentSpace = Parent.GetDimensions().ToRectangle();

			if(!GetDimensions().ToRectangle().Intersects(parentSpace)){
				Left.Pixels = Utils.Clamp(Left.Pixels, 0, parentSpace.Right - Width.Pixels);
				Top.Pixels = Utils.Clamp(Top.Pixels, 0, parentSpace.Bottom - Height.Pixels);

				//Recalculate forces the UI system to do the positioning math again.
				Recalculate();
			}
		}
	}
}
