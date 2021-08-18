using AARPG.Core.Mechanics;
using AARPG.Core.Systems;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AARPG.Core.NPCs{
	public class StatNPC : GlobalNPC{
		public NPCStatistics stats;

		public float endurance;

		public override bool CloneNewInstances => true;

		public override bool InstancePerEntity => true;

		//AppliesToEntity is called by NPCLoader.SetDefaults... Perfect!
		public override bool AppliesToEntity(NPC entity, bool lateInstantiation){
			var idOverride = API.Edits.Detours.Vanilla.NetIDOverride;
			return lateInstantiation && NPCStatisticsRegistry.HasStats(idOverride != 0 ? idOverride : entity.type);
		}

		public override void SetDefaults(NPC npc){
			//Use "netID" instead of "type" to support the different slime/zombie IDs
			var idOverride = API.Edits.Detours.Vanilla.NetIDOverride;

			var entry = NPCStatisticsRegistry.GetRandomStats(idOverride != 0 ? idOverride : npc.type);

			if(entry is not null){
				stats = entry.stats;
				stats?.ApplyTo(npc);

				string namePrefix = entry.namePrefix is null ? "" : entry.namePrefix + " ";
				npc.GivenName = $"[Lv. {stats.level}] {namePrefix}{Lang.GetNPCNameValue(idOverride != 0 ? idOverride : npc.type)}";
			}
		}

		public override void ModifyHitByItem(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit){
			ApplyEndurance(ref damage);
		}

		public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection){
			ApplyEndurance(ref damage);
		}

		public override void ModifyHitNPC(NPC npc, NPC target, ref int damage, ref float knockback, ref bool crit){
			if(target.TryGetGlobalNPC<StatNPC>(out var statNPC))
				statNPC.ApplyEndurance(ref damage);
		}

		private void ApplyEndurance(ref int damage)
			=> damage = Math.Max(1, (int)(damage * (1f - Math.Min(0.9999f, endurance))));
	}
}
