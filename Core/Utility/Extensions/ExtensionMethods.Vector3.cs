using Microsoft.Xna.Framework;

namespace AARPG.Core.Utility.Extensions{
	public static partial class ExtensionMethods{
		public static Vector2 XY(this Vector3 vector)
			=> new Vector2(vector.X, vector.Y);
	}
}
