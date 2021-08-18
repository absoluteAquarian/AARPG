using System.ComponentModel;
using Terraria.ModLoader.IO;

namespace AARPG.Core.Mechanics{
	/// <summary>
	/// An object representing the XP, Level and other stat modifiers that could be shared between NPCs and players
	/// </summary>
	public class Statistics{
		public int level;
		public int xp;

		public Modifier healthModifier = Modifier.Default;

		public Modifier defenseModifier = Modifier.Default;

		public Modifier enduranceModifier = Modifier.Default;

		public virtual TagCompound SaveToTag()
			=> new(){
				["level"] = level,
				["xp"] = xp,
				["mod.hp"] = healthModifier.ToTag(),
				["mod.defense"] = defenseModifier.ToTag(),
				["mod.endure"] = enduranceModifier.ToTag()
			};

		public virtual void LoadFromTag(TagCompound tag){
			level = tag.GetInt("level");
			xp = tag.GetInt("xp");
			healthModifier = Modifier.FromTag(tag.GetCompound("mod.hp"));
			defenseModifier = Modifier.FromTag(tag.GetCompound("mod.defense"));
			enduranceModifier = Modifier.FromTag(tag.GetCompound("mod.endure"));
		}
	}
}
