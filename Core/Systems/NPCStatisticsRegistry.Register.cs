using AARPG.API.Sorting;
using AARPG.Core.JSON;
using AARPG.Core.Mechanics;
using Newtonsoft.Json;
using ReLogic.OS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AARPG.Core.Systems{
	public static partial class NPCStatisticsRegistry{
		internal static Dictionary<string, Func<short, bool>> conditions;

		private static void AddRequirement(ref Func<short, bool> existing, Func<short, bool> newRequirement){
			if(existing is null)
				existing = newRequirement;
			else
				existing += newRequirement;
		}

		private static void RegisterEntries(){
			CreateProgressionJSONs();

			using Stream pathsStream = CoreMod.Instance.GetFileStream("Data/paths.txt");
			using StreamReader pathsReader = new StreamReader(pathsStream);
			string[] paths = pathsReader.ReadToEnd().Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

			pathsReader.Dispose();
			pathsStream.Dispose();

			foreach(var path in paths){
				Stream jsonStream = CoreMod.Instance.GetFileStream("Data/" + path);
				StreamReader jsonReader = new StreamReader(jsonStream);
				string json = jsonReader.ReadToEnd();

				int nameIDStart = path.IndexOf('/');
				if(nameIDStart == -1){
					CoreMod.Instance.Logger.Warn($"Registry path was invalid: \"{path}\"");
					goto disposeStreams;
				}

				string source = path[..nameIDStart];
				
				nameIDStart++;

				string thingName = Path.ChangeExtension(path[nameIDStart..], null);

				int id;
				if(source == "Vanilla"){
					if(!NPCID.Search.TryGetId(thingName, out id)){
						CoreMod.Instance.Logger.Warn($"Registry path \"{path}\" had an invalid vanilla type identifier: \"{thingName}\"");
						goto disposeStreams;
					}
				}else if(ModLoader.TryGetMod(source, out Mod mod)){
					if(!mod.TryFind(thingName, out ModNPC modNPC)){
						CoreMod.Instance.Logger.Warn($"Registry path \"{path}\" had an invalid mod type identifier: \"{thingName}\"");
						goto disposeStreams;
					}

					id = modNPC.Type;
				}else{
					//Mod doesn't exist or wasn't loaded.  Just ignore it
					goto disposeStreams;
				}

				NPCStatisticsDatabaseJSON database = JsonConvert.DeserializeObject<NPCStatisticsDatabaseJSON>(json, new JsonSerializerSettings(){
					MissingMemberHandling = MissingMemberHandling.Ignore,
					ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
					FloatFormatHandling = FloatFormatHandling.DefaultValue,
					PreserveReferencesHandling = PreserveReferencesHandling.All
				});

				foreach(var entry in database.Database){
					Func<short, bool> requirement = null;
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

					CreateEntry(id, entry.NamePrefix, entry.Weight, entry.Stats, requirement);

					CoreMod.Instance.Logger.Debug($"Added entry for NPC \"{Lang.GetNPCNameValue(id)}\", Name: {entry.NamePrefix ?? "null"}");
				}

disposeStreams:
				jsonReader.Dispose();
				jsonStream.Dispose();
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

		private static void CreateProgressionJSONs(){
			//If the Mod Sources path exists, update the directory for the jsons
			string folder = Path.Combine(Platform.Get<IPathService>().GetStoragePath("ModLoader"), "Beta", "Mod Sources", nameof(AARPG), "Data");
			if(!Directory.Exists(folder))
				return;

			ModNPC m;
			var progressionDict = NPCProgressionRegistry.idsToProgressions;
			foreach(string file in File.ReadAllLines(Path.Combine(folder, "paths.txt"))
				.Concat(NPCProgressionRegistry.idsToProgressions.Keys.Select(k => k < NPCID.Count
					? "Vanilla/" + NPCID.Search.GetName(k)
					: (m = NPCLoader.GetNPC(k)).Mod.Name + "/" + m.FullName[(m.FullName.IndexOf('.') + 1)..]))){
				NPCStatisticsDatabaseJSON json;
				string fullPath = Path.Combine(folder, file) + ".json";
				string modName = Path.GetDirectoryName(file);
				string npcName = Path.GetFileName(file);

				if(File.Exists(fullPath)){
					json = JsonConvert.DeserializeObject<NPCStatisticsDatabaseJSON>(File.ReadAllText(fullPath), new JsonSerializerSettings(){
						MissingMemberHandling = MissingMemberHandling.Ignore,
						DefaultValueHandling = DefaultValueHandling.Populate
					});
				}else if((modName == "Vanilla" && NPCID.Search.TryGetId(npcName, out int id) && progressionDict.TryGetValue((short)id, out var progressions)) || (ModContent.TryFind(file.Replace('/', '.'), out m) && progressionDict.TryGetValue((short)(id = m.Type), out progressions))){
					json = new(){
						Database = progressions.Select(p => new NPCStatisticsDatabaseEntryJSON(){
							NamePrefix = null,
							Weight = 1f,
							Stats = GenerateStats(id, p),
							RequirementKeys = Enum.GetName(p)
						}).ToList()
					};
				}
			}
		}

		private static NPCStatistics GenerateStats(int type, SortingProgression progression){
			
		}

		private static NPCStatistics GenerateBossStats(int type){
			NPC npc = new NPC();
			npc.SetDefaults(type);

			//Modded bosses should have their entries be set manually
			if(!npc.boss || type >= NPCID.Count)
				return null;

			return type switch{
				NPCID.KingSlime => new(){
					level = 10,
					xp = 
				}
			};
		}
	}
}
