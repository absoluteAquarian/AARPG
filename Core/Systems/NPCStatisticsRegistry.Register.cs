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
			string projectFolder = Path.Combine(Platform.Get<IPathService>().GetStoragePath("Terraria"), "ModLoader", "Beta", "Mod Sources", nameof(AARPG), "Data");
			CreateProgressionJSONs(projectFolder);

			CoreMod.Instance.Logger.Debug("Loading internally stored JSONs...");

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
					if(entry.RequirementKeys is not null)
						requirement = CreateProgressionFunction(entry.RequirementKeys);

					CreateEntry(id, entry.NamePrefix, entry.Weight, entry.Stats, requirement);

					//CoreMod.Instance.Logger.Debug($"Added statistics entry for NPC \"{Lang.GetNPCNameValue(id)}\", Prefix: {entry.NamePrefix ?? "none"}");
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

		private static void CreateProgressionJSONs(string projectFolder){
			//If the Mod Sources path exists, update the directory for the jsons
			if(!Directory.Exists(projectFolder) || CoreMod.Release){
				if(!CoreMod.Release)
					CoreMod.Instance.Logger.Debug("AARPG project directory not found: " + projectFolder);

				return;
			}

			CoreMod.Instance.Logger.Debug("AARPG project directory found.  Attempting to update Data folder...");

			string pathsFile = Path.Combine(projectFolder, "paths.txt");
			List<string> existingFiles = new(File.ReadAllLines(pathsFile));
			List<string> files = new();
			bool entryWasCreated = false;

			ModNPC m = null;
			var progressionDict = NPCProgressionRegistry.idsToProgressions;
			foreach(string file in existingFiles
				.Concat(NPCProgressionRegistry.idsToProgressions.Keys.Select(k => k < NPCID.Count
					? "Vanilla/" + NPCID.Search.GetName(k)
					: (m = NPCLoader.GetNPC(k)).Mod.Name + "/" + m.FullName[(m.FullName.IndexOf('.') + 1)..]))){
				NPCStatisticsDatabaseJSON json;
				int id = -1;

				string fullPath = Path.Combine(projectFolder, file) + ".json";

				string modName = Path.GetDirectoryName(file);
				Directory.CreateDirectory(Path.Combine(projectFolder, modName));

				string npcName = Path.GetFileName(file);

				bool isVanillaID = modName == "Vanilla" && NPCID.Search.TryGetId(npcName, out id);
				bool isModdedID = !isVanillaID && ModContent.TryFind(file.Replace('/', '.'), out m);
				if(isModdedID)
					id = (short)m.Type;

				if(File.Exists(fullPath)){
					json = JsonConvert.DeserializeObject<NPCStatisticsDatabaseJSON>(File.ReadAllText(fullPath), new JsonSerializerSettings(){
						MissingMemberHandling = MissingMemberHandling.Ignore,
						DefaultValueHandling = DefaultValueHandling.Populate
					});

					files.Add(file + ".json");
				}else if((isVanillaID || isModdedID) && progressionDict.TryGetValue((short)id, out var progressions)){
					NPCStatistics stats = null;
					json = new(){
						Database = progressions
							.Select(p => (stats = GenerateStats(id, p)) is null ? null : new NPCStatisticsDatabaseEntryJSON(){
								NamePrefix = null,
								Weight = 1f,
								Stats = stats,
								RequirementKeys = Enum.GetName(p)
							})
							.Where(j => j is not null)
							.ToList()
					};

					if(json.Database.Count > 0){
						//Create the file in the project
						using(StreamWriter writer = new StreamWriter(File.Open(fullPath, FileMode.CreateNew)))
							writer.Write(JsonConvert.SerializeObject(json, Formatting.Indented, new JsonSerializerSettings(){
								MissingMemberHandling = MissingMemberHandling.Ignore,
								DefaultValueHandling = DefaultValueHandling.Populate
							}));

						files.Add(file + ".json");

						entryWasCreated = true;
					}
				}
			}

			//Update the paths file if any new entries were created
			if(entryWasCreated || existingFiles.Count != files.Count){
				File.WriteAllLines(pathsFile, files);

				CoreMod.Instance.Logger.Warn("New JSON entries have been created.  Re-building the mod will be required!");
			}
		}

		private static NPCStatistics GenerateStats(int type, SortingProgression progression){
			var stats = GenerateBossStats(type);
			if(stats is not null){
				CoreMod.Instance.Logger.Debug($"Generated stats for vanilla NPC \"{Lang.GetNPCNameValue(type)}\"");

				return stats;
			}

			// TODO: generate a dictionary containing the base "levels" per progression entry for normal NPCs
			return null;
		}

		private static NPCStatistics GenerateBossStats(int type)
			=> type switch{
				NPCID.KingSlime => new(){
					level = 10,
					xp = 500
				},
				NPCID.EyeofCthulhu => new(){
					level = 12,
					xp = 650
				},
				NPCID.EaterofWorldsHead => new(){
					level = 18,
					xp = 1200
				},
				NPCID.EaterofWorldsBody => new(){
					level = 18,
					xp = 0
				},
				NPCID.EaterofWorldsTail => new(){
					level = 18,
					xp = 0
				},
				NPCID.BrainofCthulhu => new(){
					level = 18,
					xp = 1200
				},
				NPCID.DD2DarkMageT1 => new(){
					level = 15,
					xp = 900
				},
				NPCID.QueenBee => new(){
					level = 25,
					xp = 3500
				},
				NPCID.SkeletronHead => new(){
					level = 30,
					xp = 4550
				},
				NPCID.SkeletronHand => new(){
					level = 30,
					xp = 0
				},
				NPCID.WallofFlesh => new(){
					level = 35,
					xp = 2000
				},
				NPCID.WallofFleshEye => new(){
					level = 35,
					xp = 2000
				},
				NPCID.GoblinSummoner => new(){
					level = 40,
					xp = 3500
				},
				NPCID.BloodNautilus => new(){
					level = 38,
					xp = 2750
				},
				NPCID.QueenSlimeBoss => new(){
					level = 45,
					xp = 6000
				},
				NPCID.TheDestroyer => new(){
					level = 55,
					xp = 8000
				},
				NPCID.PirateCaptain => new(){
					level = 51,
					xp = 2500
				},
				NPCID.PirateShip => new(){
					level = 53,
					xp = 6000,
				},
				NPCID.PirateShipCannon => new(){
					level = 53,
					xp = 0,
				},
				NPCID.DD2OgreT2 => new(){
					level = 60,
					xp = 9000
				},
				NPCID.Spazmatism => new(){
					level = 55,
					xp = 4000
				},
				NPCID.Retinazer => new(){
					level = 55,
					xp = 4000
				},
				NPCID.SkeletronPrime => new(){
					level = 55,
					xp = 8000
				},
				NPCID.PrimeCannon => new(){
					level = 55,
					xp = 0
				},
				NPCID.PrimeLaser => new(){
					level = 55,
					xp = 0
				},
				NPCID.PrimeSaw => new(){
					level = 55,
					xp = 0
				},
				NPCID.PrimeVice => new(){
					level = 55,
					xp = 0
				},
				NPCID.Plantera => new(){
					level = 75,
					xp = 12000
				},
				NPCID.Mothron => new(){
					level = 77,
					xp = 12500
				},
				NPCID.MourningWood => new(){
					level = 80,
					xp = 15750
				},
				NPCID.Pumpking => new(){
					level = 82,
					xp = 17500
				},
				NPCID.Everscream => new(){
					level = 80,
					xp = 15750
				},
				NPCID.SantaNK1 => new(){
					level = 81,
					xp = 16000
				},
				NPCID.IceQueen => new(){
					level = 82,
					xp = 17500
				},
				NPCID.HallowBoss => new(){
					level = 86,
					xp = 20000
				},
				NPCID.Golem => new(){
					level = 89,
					xp = 22750
				},
				NPCID.GolemFistLeft => new(){
					level = 89,
					xp = 0
				},
				NPCID.GolemFistRight => new(){
					level = 89,
					xp = 0
				},
				NPCID.GolemHead => new(){
					level = 89,
					xp = 0
				},
				NPCID.GolemHeadFree => new(){
					level = 89,
					xp = 0
				},
				NPCID.MartianSaucerCore => new(){
					level = 85,
					xp = 19000
				},
				NPCID.MartianSaucerCannon => new(){
					level = 85,
					xp = 0
				},
				NPCID.MartianSaucerTurret => new(){
					level = 85,
					xp = 0
				},
				NPCID.MartianSaucer => new(){
					level = 85,
					xp = 0
				},
				NPCID.DD2DarkMageT3 => new(){
					level = 82,
					xp = 14000
				},
				NPCID.DD2OgreT3 => new(){
					level = 83,
					xp = 15000
				},
				NPCID.DD2Betsy => new(){
					level = 90,
					xp = 23000
				},
				NPCID.DukeFishron => new(){
					level = 90,
					xp = 25000
				},
				NPCID.CultistBoss => new(){
					level = 95,
					xp = 30000
				},
				NPCID.LunarTowerSolar => new(){
					level = 97,
					xp = 8000
				},
				NPCID.LunarTowerNebula => new(){
					level = 97,
					xp = 8000
				},
				NPCID.LunarTowerStardust => new(){
					level = 97,
					xp = 8000
				},
				NPCID.LunarTowerVortex => new(){
					level = 97,
					xp = 8000
				},
				NPCID.MoonLordCore => new(){
					level = 100,
					xp = 40000
				},
				NPCID.MoonLordHead => new(){
					level = 100,
					xp = 0
				},
				NPCID.MoonLordHand => new(){
					level = 100,
					xp = 0
				},
				NPCID.MoonLordFreeEye => new(){
					level = 100,
					xp = 0
				},
				_ => null
			};
	}
}
