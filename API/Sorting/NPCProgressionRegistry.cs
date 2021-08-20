using System;
using System.Collections.Generic;
using Terraria.ID;

namespace AARPG.API.Sorting{
	public static class NPCProgressionRegistry{
		//Copied from Terraria.ID.NPCID.cs
		private static readonly int[] NetIdMap = new int[65]{
			NPCID.CorruptSlime,
			NPCID.CorruptSlime,
			NPCID.BlueSlime,
			NPCID.BlueSlime,
			NPCID.BlueSlime,
			NPCID.BlueSlime,
			NPCID.BlueSlime,
			NPCID.BlueSlime,
			NPCID.BlueSlime,
			NPCID.BlueSlime,
			NPCID.EaterofSouls,
			NPCID.EaterofSouls,
			NPCID.AngryBones,
			NPCID.AngryBones,
			NPCID.ArmoredSkeleton,
			NPCID.Hornet,
			NPCID.Hornet,
			NPCID.MossHornet,
			NPCID.MossHornet,
			NPCID.MossHornet,
			NPCID.MossHornet,
			NPCID.Crimera,
			NPCID.Crimera,
			NPCID.Crimslime,
			NPCID.Crimslime,
			NPCID.Zombie,
			NPCID.Zombie,
			NPCID.BaldZombie,
			NPCID.BaldZombie,
			NPCID.PincushionZombie,
			NPCID.PincushionZombie,
			NPCID.SlimedZombie,
			NPCID.SlimedZombie,
			NPCID.SwampZombie,
			NPCID.SwampZombie,
			NPCID.TwiggyZombie,
			NPCID.TwiggyZombie,
			NPCID.CataractEye,
			NPCID.SleepyEye,
			NPCID.DialatedEye,
			NPCID.GreenEye,
			NPCID.PurpleEye,
			NPCID.DemonEye,
			NPCID.FemaleZombie,
			NPCID.FemaleZombie,
			NPCID.Skeleton,
			NPCID.Skeleton,
			NPCID.HeadacheSkeleton,
			NPCID.HeadacheSkeleton,
			NPCID.MisassembledSkeleton,
			NPCID.MisassembledSkeleton,
			NPCID.PantlessSkeleton,
			NPCID.PantlessSkeleton,
			NPCID.ZombieRaincoat,
			NPCID.ZombieRaincoat,
			NPCID.HornetFatty,
			NPCID.HornetFatty,
			NPCID.HornetHoney,
			NPCID.HornetHoney,
			NPCID.HornetLeafy,
			NPCID.HornetLeafy,
			NPCID.HornetSpikey,
			NPCID.HornetSpikey,
			NPCID.HornetStingy,
			NPCID.HornetStingy
		};

		//Used to indicate what NPCs should be ignored when assigning registry stats in SetDefaults due to them transforming from another NPC
		//NOTE: in the case of the Vampire, the NPC that's intially spawned is NPCID.Vampire
		internal static readonly List<int> TransformingNPCs = new(){
			NPCID.VampireBat,
			NPCID.Vampire,
			NPCID.BlackRecluseWall,
			NPCID.WallCreeperWall,
			NPCID.JungleCreeperWall,
			NPCID.BloodCrawlerWall,
			NPCID.DesertScorpionWall,
			NPCID.Nymph,
			NPCID.RayGunner,
			NPCID.SolarSpearman,
			NPCID.MothronSpawn,
			NPCID.LihzahrdCrawler,
			NPCID.NutcrackerSpinning
		};

		//The dictionary that's used to indicate what ID an NPC considers the "head" type in the case of worms
		//Does not contain the Eater of Worlds due to its splitting nature
		internal static readonly Dictionary<int, int> NonSeparableWormNPCToHead = new(){
			[NPCID.GiantWormBody] = NPCID.GiantWormHead,
			[NPCID.GiantWormTail] = NPCID.GiantWormHead,
			[NPCID.DevourerBody] = NPCID.DevourerHead,
			[NPCID.DevourerTail] = NPCID.DevourerHead,
			[NPCID.BoneSerpentBody] = NPCID.BoneSerpentHead,
			[NPCID.BoneSerpentTail] = NPCID.BoneSerpentHead,
			[NPCID.WyvernLegs] = NPCID.WyvernHead,
			[NPCID.WyvernBody] = NPCID.WyvernHead,
			[NPCID.WyvernBody2] = NPCID.WyvernHead,
			[NPCID.WyvernBody3] = NPCID.WyvernHead,
			[NPCID.WyvernTail] = NPCID.WyvernHead,
			[NPCID.DiggerBody] = NPCID.DiggerHead,
			[NPCID.DiggerTail] = NPCID.DiggerHead,
			[NPCID.LeechBody] = NPCID.LeechHead,
			[NPCID.LeechTail] = NPCID.LeechHead,
			[NPCID.TheDestroyerBody] = NPCID.TheDestroyer,
			[NPCID.TheDestroyerTail] = NPCID.TheDestroyer,
			[NPCID.StardustWormBody] = NPCID.StardustWormHead,
			[NPCID.StardustWormTail] = NPCID.StardustWormHead,
			[NPCID.SolarCrawltipedeBody] = NPCID.SolarCrawltipedeHead,
			[NPCID.SolarCrawltipedeTail] = NPCID.SolarCrawltipedeHead,
			[NPCID.CultistDragonBody1] = NPCID.CultistDragonHead,
			[NPCID.CultistDragonBody2] = NPCID.CultistDragonHead,
			[NPCID.CultistDragonBody3] = NPCID.CultistDragonHead,
			[NPCID.CultistDragonBody4] = NPCID.CultistDragonHead,
			[NPCID.CultistDragonTail] = NPCID.CultistDragonHead,
			[NPCID.DuneSplicerBody] = NPCID.DuneSplicerHead,
			[NPCID.DuneSplicerTail] = NPCID.DuneSplicerHead,
			[NPCID.TombCrawlerBody] = NPCID.TombCrawlerHead,
			[NPCID.TombCrawlerTail] = NPCID.TombCrawlerHead,
			[NPCID.BloodEelBody] = NPCID.BloodEelHead,
			[NPCID.BloodEelTail] = NPCID.BloodEelHead
		};

		public static Dictionary<SortingProgression, List<short>> idsByProgression;

		internal static void Load(){
			idsByProgression = new();

			InitializeRegistry();
		}

		internal static void Unload(){
			idsByProgression = null;
		}

		private static void InitializeRegistry(){
			//Create a registry of NPC ids based on where they're expected to appear during progression
			//This will help with the creation of "fallback stats" for the NPC Statistics Registry

			idsByProgression[SortingProgression.PreHardmodeSurface] = new(){
				NPCID.GreenSlime,
				NPCID.BlueSlime,
				NPCID.PurpleSlime,
				NPCID.Pinky,
				NPCID.SlimeMasked,
				NPCID.SlimeRibbonGreen,
				NPCID.SlimeRibbonRed,
				NPCID.SlimeRibbonWhite,
				NPCID.SlimeRibbonYellow,
				NPCID.WindyBalloon,
				NPCID.Dandelion
			};

			idsByProgression[SortingProgression.PreHardmodeSurfaceNight] = new(){
				NPCID.Raven,
				NPCID.ArmedZombie,
				NPCID.ArmedTorchZombie,
				NPCID.ArmedZombieCenx,
				NPCID.ArmedZombiePincussion,
				NPCID.ArmedZombieSlimed,
				NPCID.ArmedZombieSwamp,
				NPCID.ArmedZombieTwiggy,
				NPCID.ZombieDoctor,
				NPCID.ZombieSuperman,
				NPCID.ZombiePixie,
				NPCID.ZombieXmas,
				NPCID.ZombieSweater
			};
			idsByProgression[SortingProgression.PreHardmodeSurfaceNight].AddRange(CreateNetIDsFromBaseTypes(NPCID.Zombie, NPCID.BaldZombie, NPCID.PincushionZombie, NPCID.SlimedZombie, NPCID.SwampZombie, NPCID.TwiggyZombie, NPCID.CataractEye, NPCID.SleepyEye, NPCID.DialatedEye, NPCID.GreenEye, NPCID.PurpleEye, NPCID.DemonEye, NPCID.FemaleZombie));

			idsByProgression[SortingProgression.PreHardmodeSnowSurface] = new(){
				NPCID.IceSlime
			};

			idsByProgression[SortingProgression.PreHardmodeSnowSurfaceNight] = new(){
				NPCID.ZombieEskimo,
				NPCID.ArmedZombieEskimo
			};

			idsByProgression[SortingProgression.PreHardmodeSnowDepths] = new(){
				NPCID.IceBat,
				NPCID.SnowFlinx,
				NPCID.SpikedIceSlime,
				NPCID.UndeadViking,
				NPCID.UndeadMiner,
				NPCID.CyanBeetle,
				NPCID.Nymph
			};

			idsByProgression[SortingProgression.RainEvent] = new(){
				NPCID.FlyingFish,
				NPCID.UmbrellaSlime,
				NPCID.ZombieRaincoat
			};

			idsByProgression[SortingProgression.PreHardmodeUnderground] = new(){
				NPCID.GiantWormHead,
				NPCID.BlueSlime,
				NPCID.RedSlime,
				NPCID.YellowSlime,
				NPCID.Pinky,
				NPCID.BlueJellyfish
			};

			idsByProgression[SortingProgression.SandstormEvent] = new(){
				NPCID.Tumbleweed,
				NPCID.TombCrawlerHead
			};

			idsByProgression[SortingProgression.PreHardmodeCorruption] = new(){
				NPCID.DevourerHead
			};
			idsByProgression[SortingProgression.PreHardmodeCorruption].AddRange(CreateNetIDsFromBaseTypes(NPCID.EaterofSouls));

			idsByProgression[SortingProgression.PreHardmodeCrimson] = new(){
				NPCID.BloodCrawler,
				NPCID.FaceMonster
			};
			idsByProgression[SortingProgression.PreHardmodeCrimson].AddRange(CreateNetIDsFromBaseTypes(NPCID.Crimera));

			idsByProgression[SortingProgression.PreHardmodeCaverns] = new(){
				NPCID.BlackSlime,
				NPCID.MotherSlime,
				NPCID.BabySlime,
				NPCID.GiantWormHead,
				NPCID.SkeletonTopHat,
				NPCID.SkeletonAstonaut,
				NPCID.SkeletonAlien,
				NPCID.CaveBat,
				NPCID.BlueJellyfish,
				NPCID.Piranha,
				NPCID.Salamander,
				NPCID.Salamander2,
				NPCID.Salamander3,
				NPCID.Salamander4,
				NPCID.Salamander5,
				NPCID.Salamander6,
				NPCID.Salamander7,
				NPCID.Salamander8,
				NPCID.Salamander9,
				NPCID.Crawdad,
				NPCID.Crawdad2,
				NPCID.GiantShelly,
				NPCID.GiantShelly2,
				NPCID.UndeadMiner,
				NPCID.Tim,
				NPCID.LostGirl,
				NPCID.Pinky,
				NPCID.CochinealBeetle
			};
			idsByProgression[SortingProgression.PreHardmodeCaverns].AddRange(CreateNetIDsFromBaseTypes(NPCID.Skeleton, NPCID.HeadacheSkeleton, NPCID.MisassembledSkeleton, NPCID.PantlessSkeleton));
		}

		private static HashSet<short> CreateNetIDsFromBaseTypes(params short[] ids){
			HashSet<short> set = new();

			foreach(short id in ids){
				if(id < 0){
					//If the ID is a net ID, that's fine, just add it to the set and move on
					set.Add(id);
					continue;
				}else if(id == 0)
					throw new ArgumentException("ID array contained a zero value");

				set.Add(id);

				//Iterate through the "net IDs" array and find any that match this ID
				//NPCID.FromNetId(int id) returns NetIdMap[-id - 1] for negative IDs
				for(int i = 0; i < NetIdMap.Length; i++){
					int netID = NetIdMap[i];

					if(netID == id){
						//i = -id - 1
						//1 + i = -id
						//-1 - i = id
						set.Add((short)(-1 - i));
					}
				}
			}

			return set;
		}
	}
}
