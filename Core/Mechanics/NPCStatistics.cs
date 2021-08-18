using AARPG.Core.JSON;
using AARPG.Core.NPCs;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;
using Terraria;
using Terraria.ModLoader.IO;

namespace AARPG.Core.Mechanics{
	/// <summary>
	/// An object representing statistics for an NPC
	/// </summary>
	[DataContract]
	[JsonConverter(typeof(NPCStatisticsConverter))]
	public class NPCStatistics : Statistics{
		public Modifier scaleModifier = Modifier.Default;

		public Modifier valueModifier = Modifier.Default;

		public void ApplyTo(NPC npc){
			healthModifier.ApplyModifier(ref npc.lifeMax);
			npc.life = npc.lifeMax;
			defenseModifier.ApplyModifier(ref npc.defDefense);
			
			if(npc.TryGetGlobalNPC<StatNPC>(out var statNPC))
				enduranceModifier.ApplyModifier(ref statNPC.endurance);

			scaleModifier.ApplyModifier(ref npc.scale);
			valueModifier.ApplyModifier(ref npc.value);
		}

		public override TagCompound SaveToTag(){
			var tag = base.SaveToTag();
			tag.Add("mod.scale", scaleModifier.ToTag());
			tag.Add("mod.value", valueModifier.ToTag());
			return tag;
		}

		public override void LoadFromTag(TagCompound tag){
			base.LoadFromTag(tag);
			scaleModifier = Modifier.FromTag(tag.GetCompound("mod.scale"));
			valueModifier = Modifier.FromTag(tag.GetCompound("mod.value"));
		}
	}
}
