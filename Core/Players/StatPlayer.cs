using AARPG.Core.Mechanics;
using AARPG.Core.NPCs;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AARPG.Core.Players{
	public class StatPlayer : ModPlayer{
		public PlayerStatistics stats;

		public override void SaveData(TagCompound tag){
			tag["stats"] = stats.SaveToTag();
		}

		public override void Initialize(){
			stats?.DeInitialize();
			stats = new PlayerStatistics();
		}

		public override void LoadData(TagCompound tag){
			if(tag.GetCompound("stats") is TagCompound statTag)
				stats.LoadFromTag(statTag);
		}

		public override void PostUpdateMiscEffects(){
			stats.healthModifier.ApplyModifier(ref Player.statLifeMax2);
			stats.defenseModifier.ApplyModifier(ref Player.statDefense);
			stats.enduranceModifier.ApplyModifier(ref Player.endurance);
		}

		public override void PostUpdateRunSpeeds(){
			stats.runAccelerationModifier.ApplyModifier(ref Player.runAcceleration);
			stats.maxRunSpeedModifier.ApplyModifier(ref Player.maxRunSpeed);
			stats.maxRunSpeedModifier.ApplyModifier(ref Player.accRunSpeed);
		}

		public override void ModifyWeaponCrit(Item item, ref int crit){
			ref var data = ref stats.GetModifier(item.DamageType);
			crit += data.crit;
		}

		public override void ModifyWeaponDamage(Item item, ref StatModifier damage, ref float flat){
			ref var data = ref stats.GetModifier(item.DamageType);
			damage += data.modifier.add;
			damage *= data.modifier.mult;
			flat += data.modifier.flat;
		}

		public override float UseSpeedMultiplier(Item item)
			=> 1f / stats.itemUseModifier.mult;

		public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit){
			if(target.life <= 0)
				OnKillNPC(target);
		}

		public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit){
			if(target.life <= 0)
				OnKillNPC(target);
		}

		private void OnKillNPC(NPC npc){
			if(npc.TryGetGlobalNPC<StatNPC>(out var statNPC) && statNPC.stats is not null && !npc.SpawnedFromStatue)
				stats.UpdateXP(Player, statNPC.stats.xp);
		}
	}
}
