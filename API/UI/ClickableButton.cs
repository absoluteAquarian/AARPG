﻿using AARPG.Core.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace AARPG.API.UI{
	public class ClickableButton : UITextPanel<string>{
		public bool Hovering;

		public bool LeftClick => Hovering && InterfaceSystem.LeftClick;

		public ClickableButton(string text) : base(text){ }

		public override void Update(GameTime gameTime){
			Hovering = ContainsPoint(Main.MouseScreen);

			if(Hovering)
				BackgroundColor = new Color(93, 114, 201);
			else
				BackgroundColor = new Color(63, 82, 151) * 0.7f;
		}
	}
}
