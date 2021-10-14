using AARPG.API.Sorting;
using AARPG.Core.Mechanics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;

namespace AARPG.Core.Systems{
	/// <summary>
	/// The central database for NPC statistics
	/// </summary>
	public static partial class NPCStatisticsRegistry{
		public class Entry{
			public string namePrefix;

			public NPCStatistics stats;

			public Func<short, bool> requirement;

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

			conditions = new(NPCProgressionRegistry.idsByProgression.Keys
				.Select(p => new KeyValuePair<string, Func<short, bool>>(Enum.GetName(p), CreateProgressionFunction(p))));
		}

		internal static Func<short, bool> CreateProgressionFunction(SortingProgression progression)
			=> npc => NPCProgressionRegistry.CanUseEntriesAtProgressionStage(progression, npc, Main.gameMenu ? null : Main.LocalPlayer);

		internal static Func<short, bool> CreateProgressionFunction(string jsonProgression){
			Func<short, bool> ret = null;
			
			foreach(var progression in jsonProgression.Split(';')){
				if(ret is null)
					ret = conditions[progression];
				else
					ret += conditions[progression];
			}

			return ret;
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

			var validEntries = list.Where(e => e.requirement?.Invoke((short)type) ?? true).ToList();
			if(validEntries.Count == 0)
				return null;

			WeightedRandom<Entry> wRand = new(Main.rand);
			foreach(var entry in validEntries)
				wRand.Add(entry, entry.tableWeight);

			return wRand.Get();
		}

		public static bool HasStats(int type)
			=> registry.ContainsKey(type);

		public static void CreateEntry(int type, string namePrefix, float weight, NPCStatistics stats, Func<short, bool> requirement = null){
			if(namePrefix == "null")
				namePrefix = null;

			if(!registry.TryGetValue(type, out var list))
				registry.Add(type, list = new List<Entry>());

			var entry = new Entry(type, namePrefix, weight, stats){
				requirement = requirement
			};

			list.Add(entry);
		}
	}
}
