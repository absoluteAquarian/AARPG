using AARPG.Core.JSON;
using AARPG.Core.Mechanics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AARPG.Core.Systems{
	public static partial class NPCStatisticsRegistry{
		public static class Conditions{
			public static bool PostBoss1() => NPC.downedBoss1;
			public static bool PostBoss2() => NPC.downedBoss2;
			public static bool PostBoss3() => NPC.downedBoss3;
			public static bool PostWoF() => Main.hardMode;
			public static bool PostMech() => NPC.downedMechBossAny;
			public static bool PostPlant() => NPC.downedPlantBoss;
			public static bool PostGolem() => NPC.downedGolemBoss;
			public static bool PostCultist() => NPC.downedAncientCultist;
			public static bool PostMoonLord() => NPC.downedMoonlord;
			public static bool AnyTownNPCActive() => Main.npc.Any(n => n.whoAmI < Main.maxNPCs && n.active && n.townNPC);
		}

		internal static Dictionary<string, Func<bool>> conditions;

		private static void AddRequirement(ref Func<bool> existing, Func<bool> newRequirement){
			if(existing is null)
				existing = newRequirement;
			else
				existing += newRequirement;
		}

		private static void RegisterEntries(){
			using Stream jsonStream = CoreMod.Instance.GetFileStream("Data/npcstats.json");
			using StreamReader jsonReader = new StreamReader(jsonStream);
			string json = jsonReader.ReadToEnd();

			NPCStatisticsDatabaseJSON database = JsonConvert.DeserializeObject<NPCStatisticsDatabaseJSON>(json, new JsonSerializerSettings(){
				MissingMemberHandling = MissingMemberHandling.Ignore,
				ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
				FloatFormatHandling = FloatFormatHandling.DefaultValue,
				PreserveReferencesHandling = PreserveReferencesHandling.All
			});

			foreach(var entry in database.Database){
				Func<bool> requirement = null;
				if(entry.RequirementKeys is not null){
					string[] keys = entry.RequirementKeys.Split(';');
					
					//Remove the trailing "," on the last one if it's there
					if(keys[^1].EndsWith(','))
						keys[^1] = keys[^1][..^1];

					foreach(var key in keys)
						AddRequirement(ref requirement, conditions.TryGetValue(key, out var condition)
							? condition
							: throw new ArgumentException($"Unknown condition requested: {key}"));
				}

				CreateEntry(entry.ID, entry.NamePrefix, entry.Weight, entry.Stats, requirement);

				CoreMod.Instance.Logger.Debug($"Added entry for NPC \"{Lang.GetNPCNameValue(entry.ID)}\", Name: {entry.NamePrefix ?? "null"}");
			}

			CreateEntry(NPCID.GreenSlime, null, 3f, new NPCStatistics(){
				level = 1,
				xp = 1
			});
			CreateEntry(NPCID.GreenSlime, "Bulky", 1f, new NPCStatistics(){
				level = 2,
				xp = 2,
				healthModifier = Modifier.MultOnly(1.1f),
				scaleModifier = Modifier.MultOnly(1.2f)
			});
			CreateEntry(NPCID.GreenSlime, "Small", 1.8f, new NPCStatistics(){
				level = 1,
				xp = 1,
				healthModifier = Modifier.MultOnly(0.95f),
				scaleModifier = Modifier.MultOnly(0.9f)
			});
		}
	}
}
