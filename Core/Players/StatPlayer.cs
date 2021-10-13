using AARPG.Core.Mechanics;
using AARPG.Core.NPCs;
using AARPG.Core.Systems;
using AARPG.Core.Utility;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AARPG.Core.Players{
	public class StatPlayer : ModPlayer{
		public PlayerStatistics stats;

		public Dictionary<short, int> downedCountsByID;

		private List<TagCompound> unloaded;

		internal int xpCollectFlashTimer = -1;
		internal const int XPCollectTimerMax = 18;
		internal Color xpCollectColor;

		public override void SaveData(TagCompound tag){
			tag["stats"] = stats.SaveToTag();

			List<TagCompound> tags = new();
			foreach(var kvp in downedCountsByID){
				TagCompound entry = new();
				NPCUtils.SaveNPCToTag(entry, kvp.Key);
				entry["count"] = kvp.Value;
			}

			tags.AddRange(unloaded);

			tag["downedCounts"] = tags;
		}

		public override void Initialize(){
			stats?.DeInitialize();
			stats = new PlayerStatistics();

			downedCountsByID = new();
			unloaded = new();
		}

		public override void LoadData(TagCompound tag){
			if(tag.GetCompound("stats") is TagCompound statTag)
				stats.LoadFromTag(statTag);

			if(tag.GetList<TagCompound>("downedCounts") is List<TagCompound> list){
				foreach(var entry in list){
					short type = NPCUtils.LoadNPCFromTag(entry, unloaded);

					if(type == -1)
						continue;

					downedCountsByID.Add(type, entry.GetInt("count"));
				}
			}
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

		private int lastUpdate = -1;

		public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright){
			if(lastUpdate != Main.GameUpdateCount && xpCollectFlashTimer >= 0){
				xpCollectFlashTimer--;
				if(xpCollectFlashTimer <= 0)
					xpCollectColor = default;
			}
			
			if(drawInfo.shadow != 0)
				return;

			Color current = new Color(r, g, b, a);
			if(xpCollectFlashTimer >= 0 && xpCollectColor != default){
				current = Color.Lerp(current, xpCollectColor, xpCollectFlashTimer / (float)XPCollectTimerMax);
				var vector = current.ToVector4();
				r = vector.X;
				g = vector.Y;
				b = vector.Z;
				a = vector.W;
			}

			lastUpdate = (int)Main.GameUpdateCount;
		}
	}
}
