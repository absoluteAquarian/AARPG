using AARPG.API.GraphicsLib;
using AARPG.Core.Players;
using AARPG.Core.Utility.Extensions;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;

namespace AARPG.Core.Mechanics{
	public sealed class Experience{
		public static class Sizes{
			public const int OrbSmallYellow =      1;
			public const int OrbSmallGreen =       5;
			public const int OrbSmallBlue =       10;
			public const int OrbMediumYellow =    50;
			public const int OrbMediumGreen =    100;
			public const int OrbMediumBlue =     500;
			public const int OrbLargeYellow =   1000;
			public const int OrbLargeGreen =    5000;
			public const int OrbLargeBlue =    10000;
		}

		public readonly int value;

		public Vector2 center;
		public Vector2 velocity;

		public readonly int target;

		private readonly Queue<Vector2> oldCenters;
		private Color[] collectedTrail;

		public bool active;
		public bool collected;
		private bool oldCollected;

		public const int ExtraUpdates = 7;

		public readonly float rotation;

		public Experience(int xp, Vector2 startPosition, Vector2 startVelocity, int targetPlayer){
			value = xp;

			Vector2 size = GetSize();
			if(size == Vector2.Zero)
				throw new Exception("Invalid xp count: " + xp);

			center = startPosition;
			velocity = startVelocity;

			target = targetPlayer;

			oldCenters = new();

			active = true;

			rotation = Main.rand.NextFloat(MathHelper.TwoPi);
		}

		public Vector2 GetSize(){
			if(value is Sizes.OrbSmallYellow or Sizes.OrbSmallGreen or Sizes.OrbSmallBlue)
				return new Vector2(6);
			if(value is Sizes.OrbMediumYellow or Sizes.OrbMediumGreen or Sizes.OrbMediumBlue)
				return new Vector2(8);
			if(value is Sizes.OrbLargeYellow or Sizes.OrbLargeGreen or Sizes.OrbLargeBlue)
				return new Vector2(10);
			return Vector2.Zero;
		}

		public Color GetTrailColor(){
			if(value is Sizes.OrbSmallYellow or Sizes.OrbMediumYellow or Sizes.OrbLargeYellow)
				return Color.Yellow;
			if(value is Sizes.OrbSmallGreen or Sizes.OrbMediumGreen or Sizes.OrbLargeGreen)
				return Color.LimeGreen;
			if(value is Sizes.OrbSmallBlue or Sizes.OrbMediumBlue or Sizes.OrbLargeBlue)
				return Color.Blue;
			return Color.Transparent;
		}

		public Rectangle GetSourceRectangle()
			=> value switch{
				Sizes.OrbSmallYellow =>  new Rectangle( 0,  0,  6,  6),
				Sizes.OrbSmallGreen =>   new Rectangle( 8,  0,  6,  6),
				Sizes.OrbSmallBlue =>    new Rectangle(16,  0,  6,  6),
				Sizes.OrbMediumYellow => new Rectangle( 0,  8,  8,  8),
				Sizes.OrbMediumGreen =>  new Rectangle(10,  8,  8,  8),
				Sizes.OrbMediumBlue =>   new Rectangle(20,  8,  8,  8),
				Sizes.OrbLargeYellow =>  new Rectangle( 0, 18, 10, 10),
				Sizes.OrbLargeGreen =>   new Rectangle(12, 18, 10, 10),
				Sizes.OrbLargeBlue =>    new Rectangle(24, 18, 10, 10),
				_ => Rectangle.Empty
			};

		public void Update(){
			if(!active)
				return;

			oldCollected = collected;

			//Home in on the player, unless they're dead
			Player player = Main.player[target];

			if(!player.active || player.dead)
				collected = true;

			if(collected){
				//Make the trail shrink
				if(oldCenters.Count == 0)
					active = false;
				else
					for(int i = 0; i < ExtraUpdates + 1; i++)
						oldCenters.Dequeue();

				return;
			}

			for(int i = 0; i < ExtraUpdates + 1; i++)
				InnerUpdate();
		}

		private void InnerUpdate(){
			Player player = Main.player[target];

			Vector2 direction = player.DirectionFrom(center);

			velocity = velocity.RotateTowards(direction, MathHelper.ToRadians(270) / 60f / (ExtraUpdates + 1));

			if(Vector2.DistanceSquared(center, player.Center) >= Vector2.DistanceSquared(center + velocity / (ExtraUpdates + 1), player.Center)){
				velocity += Vector2.Normalize(velocity) * 5f / 60f / (ExtraUpdates + 1);

				const float vel = 30 * 16;
				if(velocity.LengthSquared() > vel * vel)
					velocity = Vector2.Normalize(velocity) * vel;
			}else
				velocity *= 1f - 3.57f / 60f / (ExtraUpdates + 1);

			if(oldCenters.Count < 30 * (ExtraUpdates + 1))
				oldCenters.Enqueue(center);
			else{
				oldCenters.Dequeue();
				oldCenters.Enqueue(center);
			}

			center += velocity / (ExtraUpdates + 1);

			if(!collected && player.Hitbox.Contains(center.ToPoint())){
				var statPlayer = player.GetModPlayer<StatPlayer>();

				statPlayer.stats.UpdateXP(player, value);

				statPlayer.xpCollectColor = GetTrailColor();
				statPlayer.xpCollectFlashTimer = StatPlayer.XPCollectTimerMax;

				collected = true;
			}
		}

		public void DrawTrail(){
			if(!active)
				return;

			Vector2 size = GetSize();
			if(size == Vector2.Zero)
				return;

			//No trail yet
			if(oldCenters.Count == 0)
				return;

			Color color = GetTrailColor();
			Color[] colors = new Color[oldCenters.Count + 1];

			//If collected, just grab colors from the old array
			if(!oldCollected || collectedTrail is null || collectedTrail.Length == oldCenters.Count + 1){
				colors[^1] = color;
				int i = 0;
				foreach(var old in oldCenters){
					colors[i] = Color.Lerp(color, Color.Transparent, 1f - i / (float)(oldCenters.Count + 1));
					i++;
				}

				collectedTrail = colors;
			}else
				Array.Copy(collectedTrail, collectedTrail.Length - oldCenters.Count - 1, colors, 0, oldCenters.Count + 1);

			Vector2[] points = new Vector2[oldCenters.Count + 1];
			points[^1] = center;
			oldCenters.CopyTo(points, 0);

			PrimitiveDrawing.DrawLineStrip(points, colors);
		}
	}
}
