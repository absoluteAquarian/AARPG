using AARPG.API.DataStructures;
using AARPG.Core.Mechanics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace AARPG.Core.Systems{
	public class ExperienceTracker : ModSystem{
		public struct HeapIndex{
			public readonly int index;
			public Experience xp;

			internal HeapIndex(int index, Experience xp){
				this.index = index;
				this.xp = xp;
			}
		}

		private static Heap<Experience> heap;

		public override void OnWorldLoad(){
			heap?.Dispose();
			heap = new(1000);
		}

		public override void PostUpdateNPCs(){
			var iter = heap.GetEnumerator();

			List<int> toFree = new();

			while(iter.MoveNext()){
				var xp = iter.Current as Experience;

				xp.Update();
				
				if(!xp.active)
					toFree.Add((iter as Heap<Experience>.HeapEnumerator).CurrentIndex);
			}

			foreach(var freed in toFree)
				heap.FreeEntries(freed, 1);
		}

		public override void PostDrawTiles(){
			var batch = Main.spriteBatch;
			batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

			//Draw the orbs
			var texture = CoreMod.Instance.Assets.Request<Texture2D>("Core/Mechanics/Experience").Value;
			foreach(Experience xp in heap){
				var size = xp.GetSize();
				if(size == Vector2.Zero || xp.collected)
					continue;

				var source = xp.GetSourceRectangle();

				batch.Draw(texture, xp.center - Main.screenPosition, source, Color.White, xp.rotation, size / 2f, 1f, SpriteEffects.None, 0);
			}
			batch.End();

			//Draw the trails
			foreach(Experience xp in heap)
				xp.DrawTrail();
		}

		public static Experience GetHeapElement(HeapIndex index)
			=> heap[index.index];

		public static HeapIndex SpawnExperience(int xp, Vector2 location, Vector2 velocity, int targetPlayer){
			Experience thing = new Experience(xp, location, velocity, targetPlayer);

			return new HeapIndex(heap.AllocateEntries(new[]{ thing }), thing);
		}
	}
}
