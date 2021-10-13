using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AARPG.Core.Utility{
	public static class NPCUtils{
		public static void SaveNPCToTag(TagCompound tag, short type){
			if(type < NPCID.Count){
				tag["mod"] = "Terraria";
				tag["id"] = type;
			}else{
				var npc = ModContent.GetModNPC(type);
				tag["mod"] = npc.Mod.Name;
				tag["name"] = npc.FullName;
			}
		}

		public static short LoadNPCFromTag(TagCompound tag, List<TagCompound> unloadedMods){
			string mod = tag.GetString("mod");
			if(mod == "Terraria")
				return tag.GetShort("id");

			string name = tag.GetString("name");

			if(ModLoader.TryGetMod(mod, out _) && ModContent.TryFind(name, out ModNPC npc))
				return (short)npc.Type;

			unloadedMods.Add(new(){
				["mod"] = mod,
				["name"] = name
			});

			return -1;
		}
	}
}
