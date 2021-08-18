using System;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AARPG.Core.Utility{
	public static class TagUtils{
		public static TagCompound SaveAsTag(this DamageClass damage)
			=> new(){
				["mod"] = damage.Mod?.Name ?? "Terraria",
				["name"] = damage.Name
			};

		public static DamageClass LoadDamageClass(TagCompound tag){
			string mod = tag.GetString("mod");
			string type = tag.GetString("name");

			if(mod is null || tag is null)
				throw new ArgumentException("Invalid tag: no Mod or Name specified");

			if(mod == "Terraria"){
				if(type == DamageClass.NoScaling.Name)
					return DamageClass.NoScaling;
				if(type == DamageClass.Generic.Name)
					return DamageClass.Generic;
				if(type == DamageClass.Melee.Name)
					return DamageClass.Melee;
				if(type == DamageClass.Ranged.Name)
					return DamageClass.Ranged;
				if(type == DamageClass.Magic.Name)
					return DamageClass.Magic;
				if(type == DamageClass.Summon.Name)
					return DamageClass.Summon;
				if(type == DamageClass.Throwing.Name)
					return DamageClass.Throwing;

				throw new ArgumentException($"Invalid damage class detected ({mod}:{type})");
			}else if(ModLoader.TryGetMod(mod, out Mod inst)){
				//Return "null" if the damage class no longer exists
				return inst.TryFind<DamageClass>(type, out var damage) ? damage : null;
			}

			//Return "null" to indicate that the mod couldn't be loaded
			return null;
		}
	}
}
