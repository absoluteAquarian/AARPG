using AARPG.Core.JSON;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Terraria.ModLoader.IO;

namespace AARPG.Core.Mechanics{
	/// <summary>
	/// A structure representing a modifier to some value
	/// </summary>
	[DataContract]
	[JsonConverter(typeof(ModifierConverter))]
	public struct Modifier{
		/// <summary>
		/// The additive factor.  Applied before <seealso cref="mult"/> is used
		/// <para>
		/// <c>result = ((input + add) * mult) + flat</c>
		/// </para>
		/// </summary>
		[DataMember(Name = "add")]
		[DefaultValue(0f)]
		public float add;
		/// <summary>
		/// The multiplicative factor.  Applied after <seealso cref="add"/> is used and before <seealso cref="flat"/> is used
		/// <para>
		/// <c>result = ((input + add) * mult) + flat</c>
		/// </para>
		/// </summary>
		[DataMember(Name = "mult")]
		[DefaultValue(1f)]
		public float mult;
		/// <summary>
		/// The flat additive factor.  Applied after <seealso cref="mult"/> is used
		/// <para>
		/// <c>result = ((input + add) * mult) + flat</c>
		/// </para>
		/// </summary>
		[DataMember(Name = "flat")]
		[DefaultValue(0f)]
		public float flat;

		public static readonly Modifier Default = new Modifier(add: 0, mult: 1, flat: 0);

		public Modifier(float add, float mult, float flat){
			this.add = add;
			this.mult = mult;
			this.flat = flat;
		}

		public void ApplyModifier(ref int stat)
			=> stat = (int)((stat + add) * mult + flat);

		public void ApplyModifier(ref float stat)
			=> stat = (stat + add) * mult + flat;

		public void ApplyModifier(ref double stat)
			=> stat = (stat + add) * mult + flat;

		public static Modifier operator &(Modifier first, Modifier second)
			=> new Modifier(first.add + second.add, first.mult * second.mult, first.flat + second.flat);

		public static bool operator ==(Modifier first, Modifier second)
			=> first.add == second.add && first.mult == second.mult && first.flat == second.flat;

		public static bool operator !=(Modifier first, Modifier second)
			=> !(first == second);

		public override bool Equals(object obj)
			=> obj is Modifier modifier && this == modifier;

		public override int GetHashCode()
			=> HashCode.Combine(add, mult, flat);

		public override string ToString() => $"Add: {add}, Mult: {mult}, Flat: {flat}";

		public static Modifier AddOnly(float add) => new Modifier(add, 1, 0);
		public static Modifier MultOnly(float mult) => new Modifier(0, mult, 0);
		public static Modifier FlatOnly(float flat) => new Modifier(0, 1, flat);

		public TagCompound ToTag()
			=> new(){
				["add"] = add,
				["mult"] = mult,
				["flat"] = flat
			};

		public static Modifier FromTag(TagCompound tag)
			=> tag is null
				? Default
				: new(tag.Get<float>("add"), tag.Get<float>("mult"), tag.Get<float>("flat"));
	}
}
