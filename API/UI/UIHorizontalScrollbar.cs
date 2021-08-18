using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.UI;

namespace AARPG.API.UI{
	public class UIHorizontalScrollbar : UIElement{
		private float viewPosition;
		private float viewSize = 1f;
		private float maxViewSize = 20f;
		private bool isDragging;
		private float dragXOffset;
		private bool isHoveringOverHandle;
		private readonly Asset<Texture2D> texture;
		private readonly Asset<Texture2D> innerTexture;

		public UIHorizontalScrollbar(){
			Height.Set(20f, 0f);
			MaxHeight.Set(20f, 0f);
			texture = Main.Assets.Request<Texture2D>("Images/UI/Scrollbar");
			innerTexture = Main.Assets.Request<Texture2D>("Images/UI/ScrollbarInner");
			PaddingLeft = 5f;
			PaddingRight = 5f;
		}

		public float ViewPosition{
			get => viewPosition;
			set => viewPosition = MathHelper.Clamp(value, 0f, maxViewSize - viewSize);
		}

		public void SetView(float viewSize, float maxViewSize){
			viewSize = MathHelper.Clamp(viewSize, 0f, maxViewSize);
			viewPosition = MathHelper.Clamp(viewPosition, 0f, maxViewSize - viewSize);
			this.viewSize = viewSize;
			this.maxViewSize = maxViewSize;
		}

		private Rectangle GetHandleRectangle(){
			CalculatedStyle innerDimensions = GetInnerDimensions();
			if(maxViewSize == 0f && viewSize == 0f){
				viewSize = 1f;
				maxViewSize = 1f;
			}

			return new Rectangle(
				(int)(innerDimensions.X + innerDimensions.Width * (viewPosition / maxViewSize)) - 3,
				(int)innerDimensions.Y,
				(int)(innerDimensions.Width * (viewSize / maxViewSize)) + 7,
				20);
		}

		private void DrawBar(SpriteBatch spriteBatch, Rectangle dimensions){
			Texture2D textureInst = texture.Value;

			//Draw top
			spriteBatch.Draw(textureInst, new Rectangle(dimensions.X - 6, dimensions.Y, 6, dimensions.Height), new Rectangle(0, 0, 6, textureInst.Height), Color.White);
			//Draw middle
			spriteBatch.Draw(textureInst, new Rectangle(dimensions.X, dimensions.Y, dimensions.Width, dimensions.Height), new Rectangle(8, 0, 4, textureInst.Height), Color.White);
			//Draw bottom
			spriteBatch.Draw(textureInst, new Rectangle(dimensions.X + dimensions.Width, dimensions.Y, 6, dimensions.Height), new Rectangle(textureInst.Width - 6, 0, 6, textureInst.Height), Color.White);
		}

		private void DrawHandle(SpriteBatch spriteBatch, Rectangle dimensions){
			Texture2D texture = innerTexture.Value;
			Color color = Color.White * ((isDragging || isHoveringOverHandle) ? 1f : 0.85f);

			//Draw top
			spriteBatch.Draw(texture, new Rectangle(dimensions.X - 8, dimensions.Y + 3, 8, dimensions.Height - 6), new Rectangle(0, 0, 8, texture.Height), color);
			//Draw middle
			spriteBatch.Draw(texture, new Rectangle(dimensions.X, dimensions.Y + 3, dimensions.Width, dimensions.Height - 6), new Rectangle(8, 0, 4, texture.Height), color);
			//Draw bottom
			spriteBatch.Draw(texture, new Rectangle(dimensions.X + dimensions.Width, dimensions.Y + 3, 8, dimensions.Height - 6), new Rectangle(texture.Width - 8, 0, 8, texture.Height), color);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch){
			CalculatedStyle dimensions = GetDimensions();
			CalculatedStyle innerDimensions = GetInnerDimensions();

			if(isDragging){
				float num = UserInterface.ActiveInstance.MousePosition.X - innerDimensions.X - dragXOffset;
				viewPosition = MathHelper.Clamp(num / innerDimensions.Width * maxViewSize, 0f, maxViewSize - viewSize);
			}

			// play tick sound on hover
			Rectangle handleRectangle = GetHandleRectangle();
			Vector2 mousePosition = UserInterface.ActiveInstance.MousePosition;
			bool isHoveringOverHandle = this.isHoveringOverHandle;
			this.isHoveringOverHandle = handleRectangle.Contains(new Point((int)mousePosition.X, (int)mousePosition.Y));

			if(!isHoveringOverHandle && this.isHoveringOverHandle && Main.hasFocus)
				SoundEngine.PlaySound(SoundID.MenuTick);

			// Draw Scrollbar
			DrawBar(spriteBatch, dimensions.ToRectangle());
			DrawHandle(spriteBatch, handleRectangle);
		}

		public override void MouseDown(UIMouseEvent evt){
			base.MouseDown(evt);
			if(evt.Target == this){
				Rectangle handleRectangle = GetHandleRectangle();
				if(handleRectangle.Contains(new Point((int)evt.MousePosition.X, (int)evt.MousePosition.Y))){
					isDragging = true;
					dragXOffset = evt.MousePosition.X - handleRectangle.X;
				}else{
					CalculatedStyle innerDimensions = GetInnerDimensions();
					float num = UserInterface.ActiveInstance.MousePosition.X - innerDimensions.X - (handleRectangle.Width >> 1);
					viewPosition = MathHelper.Clamp(num / innerDimensions.Width * maxViewSize, 0f, maxViewSize - viewSize);
				}
			}
		}

		public override void MouseUp(UIMouseEvent evt){
			base.MouseUp(evt);
			isDragging = false;
		}
	}
}
