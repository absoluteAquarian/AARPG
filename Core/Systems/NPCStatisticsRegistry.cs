using AARPG.Core.Mechanics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Utilities;

namespace AARPG.Core.Systems{
	/// <summary>
	/// The central database for NPC statistics
	/// </summary>
	public static partial class NPCStatisticsRegistry{
		public class Entry{
			public string namePrefix;

			public NPCStatistics stats;

			public Func<bool> requirement;

			public readonly int sourceID;
			public readonly float tableWeight;

			internal Entry(int type, string name, float weight, NPCStatistics stats){
				sourceID = type;
				namePrefix = name;
				tableWeight = weight;
				this.stats = stats;
			}
		}

		private static Dictionary<int, List<Entry>> registry;

		internal static Entry GetEntry(int netID, string name){
			if(registry.TryGetValue(netID, out var list)){
				foreach(var entry in list)
					if(entry.namePrefix == name)
						return entry;
			}

			return null;
		}

		internal static bool TryGetEntry(int netID, string name, out Entry entry){
			entry = GetEntry(netID, name);
			return entry is not null;
		}

		internal static void Load(){
			registry = new();

			conditions = new(){
				["post-eye"] = Conditions.PostBoss1,
				["post-evil"] = Conditions.PostBoss2,
				["post-skel"] = Conditions.PostBoss3,
				["pre-wof"] = Conditions.PreWoF,
				["post-wof"] = Conditions.PostWoF,
				["post-mech"] = Conditions.PostMech,
				["post-plant"] = Conditions.PostPlant,
				["post-golem"] = Conditions.PostGolem,
				["post-cultist"] = Conditions.PostCultist,
				["post-moonlord"] = Conditions.PostMoonLord,
				["anytownies"] = Conditions.AnyTownNPCActive
			};
		}

		internal static void PostSetupContent(){
			RegisterEntries();
		}

		internal static void Unload(){
			registry?.Clear();
			registry = null;

			conditions?.Clear();
			conditions = null;
		}

		public static Entry GetRandomStats(int type){
			if(!registry.TryGetValue(type, out var list))
				return null;

			var validEntries = list.Where(e => e.requirement?.Invoke() ?? true).ToList();
			if(validEntries.Count == 0)
				return null;

			WeightedRandom<Entry> wRand = new(Main.rand);
			foreach(var entry in validEntries)
				wRand.Add(entry, entry.tableWeight);

			return wRand.Get();
		}

		public static bool HasStats(int type)
			=> registry.ContainsKey(type);

		public static void CreateEntry(int type, string namePrefix, float weight, NPCStatistics stats, Func<bool> requirement = null){
			if(namePrefix == "null")
				namePrefix = null;

			if(!registry.TryGetValue(type, out var list))
				registry.Add(type, list = new List<Entry>());

			if(list.Any(e => e.namePrefix == namePrefix))
				throw new ArgumentException($"{(namePrefix is null ? "A default entry" : $"An entry named \"{namePrefix}\"")} has already been registered for the NPC \"{Lang.GetNPCNameValue(type)}\"");

			var entry = new Entry(type, namePrefix, weight, stats){
				requirement = requirement
			};

			list.Add(entry);
		}
	}
}
