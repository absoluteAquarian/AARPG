using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace AARPG.Core.Utility.Extensions{
	public static partial class ExtensionMethods{
		public static Vector3 ScreenCoord(this Vector2 vector){
			//"vector" is a point on the screen... given the zoom is 1x
			//Let's correct that
			Vector2 screenCenter = new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f);
			Vector2 diff = vector - screenCenter;
			diff *= Main.GameZoomTarget;
			vector = screenCenter + diff;

			return new Vector3(-1 + vector.X / Main.screenWidth * 2, (-1 + vector.Y / Main.screenHeight * 2f) * -1, 0);
		}

		public static float DirectionToAngle(this Vector2 source, Vector2 destination)
			=> (destination - source).ToRotation();

		public static Vector2 RotateTowards(this Vector2 source, Vector2 destination, float maxRadiansRotation){
			float angleSource = source.ToRotation();
			float angleDestination = destination.ToRotation();

			//  +-------+
			//  | S |   |
			//  |---+---|
			//  | D |   |
			//  +-------+
			if(angleSource < -MathHelper.PiOver2 && angleDestination > MathHelper.PiOver2)
				angleSource += MathHelper.TwoPi;
			//  +-------+
			//  | D |   |
			//  |---+---|
			//  | S |   |
			//  +-------+
			else if(angleSource > MathHelper.PiOver2 && angleDestination < -MathHelper.PiOver2)
				angleDestination += MathHelper.TwoPi;
			//  +-------+
			//  | S |   |
			//  |---+---|
			//  |   | D |
			//  +-------+
			else if(angleSource < -MathHelper.PiOver2 && angleDestination < MathHelper.PiOver2 && angleDestination > 0)
				angleSource += MathHelper.TwoPi;
			//  +-------+
			//  |   | D |
			//  |---+---|
			//  | S |   |
			//  +-------+
			else if(angleSource > MathHelper.PiOver2 && angleDestination > -MathHelper.PiOver2 && angleDestination < 0)
				angleDestination += MathHelper.TwoPi;

			float diff = angleDestination - angleSource;
			if(diff > MathHelper.Pi)
				diff = MathHelper.TwoPi - diff;

			if(diff > maxRadiansRotation)
				diff = maxRadiansRotation;
			else if(diff < -maxRadiansRotation)
				diff = -maxRadiansRotation;

			return source.RotatedBy(diff);
		}
	}
}
