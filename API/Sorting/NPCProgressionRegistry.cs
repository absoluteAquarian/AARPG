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
			//All zombies and Demon Eyes
			idsByProgression[SortingProgression.PreHardmodeSurfaceNight].AddRange(CreateNetIDsFromBaseTypes(NPCID.Zombie, NPCID.BaldZombie, NPCID.PincushionZombie, NPCID.SlimedZombie, NPCID.SwampZombie, NPCID.TwiggyZombie, NPCID.CataractEye, NPCID.SleepyEye, NPCID.DialatedEye, NPCID.GreenEye, NPCID.PurpleEye, NPCID.DemonEye, NPCID.FemaleZombie));

			idsByProgression[SortingProgression.PreHardmodeSnowSurface] = new(){
				NPCID.IceSlime
			};

			idsByProgression[SortingProgression.PreHardmodeSnowSurfaceNight] = new(){
				NPCID.ZombieEskimo,
				NPCID.ArmedZombieEskimo
			};

			idsByProgression[SortingProgression.DesertSurface] = new(){
				NPCID.SandSlime,
				NPCID.Vulture,
				NPCID.Antlion
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

			idsByProgression[SortingProgression.PreHardmodeMushroomBiomeSurface] = new(){
				NPCID.AnomuraFungus,
				NPCID.FungiBulb,
				NPCID.MushiLadybug,
				NPCID.ZombieMushroomHat,
				NPCID.ZombieMushroom
			};

			idsByProgression[SortingProgression.PreHardmodeMushroomBiomeDepths] = new(){
				NPCID.SporeBat,
				NPCID.SporeSkeleton
			};

			idsByProgression[SortingProgression.KingSlime] = new(){
				NPCID.KingSlime,
				NPCID.SlimeSpiked,
				NPCID.BlueSlime
			};

			idsByProgression[SortingProgression.CthulhuEye] = new(){
				NPCID.EyeofCthulhu
			};

			idsByProgression[SortingProgression.BloodMoon] = new(){
				NPCID.TheGroom,
				NPCID.TheBride,
				NPCID.BloodZombie,
				NPCID.Drippler,
				NPCID.EyeballFlyingFish,
				NPCID.ZombieMerman
			};

			idsByProgression[SortingProgression.JungleSurface] = new(){
				NPCID.JungleSlime,
				NPCID.JungleBat,
				NPCID.Piranha,
				NPCID.Snatcher
			};

			idsByProgression[SortingProgression.JungleSurfaceNight] = new(){
				NPCID.JungleBat,
				NPCID.Piranha,
				NPCID.Snatcher,
				NPCID.DoctorBones
			};
			//All zombies and Demon Eyes
			idsByProgression[SortingProgression.JungleSurfaceNight].AddRange(CreateNetIDsFromBaseTypes(NPCID.Zombie, NPCID.BaldZombie, NPCID.PincushionZombie, NPCID.SlimedZombie, NPCID.SwampZombie, NPCID.TwiggyZombie, NPCID.CataractEye, NPCID.SleepyEye, NPCID.DialatedEye, NPCID.GreenEye, NPCID.PurpleEye, NPCID.DemonEye, NPCID.FemaleZombie));

			idsByProgression[SortingProgression.Ocean] = new(){
				NPCID.PinkJellyfish,
				NPCID.Crab,
				NPCID.Squid,
				NPCID.SeaSnail,
				NPCID.Shark
			};

			idsByProgression[SortingProgression.DesertDepths] = new(){
				NPCID.Antlion,
				NPCID.WalkingAntlion,
				NPCID.LarvaeAntlion,
				NPCID.FlyingAntlion,
				NPCID.GiantFlyingAntlion,
				NPCID.GiantWalkingAntlion,
				NPCID.SandSlime,
				NPCID.TombCrawlerHead
			};

			idsByProgression[SortingProgression.EvilBoss] = new(){
				NPCID.EaterofWorldsHead,
				NPCID.EaterofWorldsBody,
				NPCID.EaterofWorldsTail,
				NPCID.BrainofCthulhu,
				NPCID.Creeper
			};

			idsByProgression[SortingProgression.GoblinArmy] = new(){
				NPCID.GoblinArcher,
				NPCID.GoblinPeon,
				NPCID.GoblinScout,
				NPCID.GoblinSorcerer,
				NPCID.GoblinThief,
				NPCID.GoblinWarrior
			};

			idsByProgression[SortingProgression.DD2Army] = new(){
				NPCID.DD2GoblinT1,
				NPCID.DD2GoblinBomberT1,
				NPCID.DD2JavelinstT1,
				NPCID.DD2WyvernT1,
				NPCID.DD2DarkMageT1,
				NPCID.DD2SkeletonT1
			};

			idsByProgression[SortingProgression.SkyBiome] = new(){
				NPCID.Harpy
			};

			idsByProgression[SortingProgression.JungleDepths] = new(){
				NPCID.ManEater,
				NPCID.JungleBat,
				NPCID.SpikedJungleSlime,
				NPCID.Piranha,
				NPCID.LacBeetle
			};
			idsByProgression[SortingProgression.JungleDepths].AddRange(CreateNetIDsFromBaseTypes(NPCID.Hornet, NPCID.HornetFatty, NPCID.HornetHoney, NPCID.HornetLeafy, NPCID.HornetSpikey, NPCID.HornetStingy));

			idsByProgression[SortingProgression.JungleDepthsNight] = new(){
				NPCID.DoctorBones
			};
			idsByProgression[SortingProgression.JungleDepthsNight].AddRange(idsByProgression[SortingProgression.JungleDepths]);

			idsByProgression[SortingProgression.QueenBee] = new(){
				NPCID.QueenBee,
				NPCID.Bee,
				NPCID.BeeSmall
			};

			idsByProgression[SortingProgression.SpiderBiome] = new(){
				NPCID.WallCreeper
			};

			idsByProgression[SortingProgression.Skeletron] = new(){
				NPCID.SkeletronHead
			};

			idsByProgression[SortingProgression.PreHardmodeHell] = new(){
				NPCID.Hellbat,
				NPCID.LavaSlime,
				NPCID.FireImp,
				NPCID.Demon,
				NPCID.VoodooDemon,
				NPCID.BoneSerpentHead
			};

			idsByProgression[SortingProgression.WallOfFlesh] = new(){
				NPCID.WallofFlesh,
				NPCID.WallofFleshEye
			};
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
