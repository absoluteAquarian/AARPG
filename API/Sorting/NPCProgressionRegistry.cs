using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using static AARPG.API.Sorting.SortingProgression;

namespace AARPG.API.Sorting{
	public static class NPCProgressionRegistry{
		public static class ZoneHelpers{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool SurfaceOrSky(Player player)
				=> player.ZoneOverworldHeight || player.ZoneSkyHeight;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool SurfaceOrSkyOrUnderground(Player player)
				=> player.ZoneOverworldHeight || player.ZoneSkyHeight;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool UndergroundOrCaverns(Player player)
				=> player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool NoEvents(Player player)
				// TODO: figure out what shows/hides the invasion progression thing
				=> (Main.invasionType == InvasionID.None)
					&& !Main.snowMoon && !Main.pumpkinMoon
					&& !Main.eclipse
					&& !player.ZoneTowerNebula && !player.ZoneTowerSolar && !player.ZoneTowerStardust && !player.ZoneTowerVortex;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool SurfacePurity(Player player)
				=> SurfaceOrSky(player) && NoEvents(player) && player.ZonePurity;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool SurfaceSnow(Player player)
				=> SurfaceOrSky(player) && NoEvents(player) && player.ZoneSnow;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool SurfaceCorruption(Player player)
				=> SurfaceOrSkyOrUnderground(player) && NoEvents(player) && player.ZoneCorrupt;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool SurfaceCrimson(Player player)
				=> SurfaceOrSkyOrUnderground(player) && NoEvents(player) && player.ZoneCrimson;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool SurfaceJungle(Player player)
				=> SurfaceOrSky(player) && NoEvents(player) && player.ZoneJungle;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool SurfaceHallow(Player player)
				=> SurfaceOrSkyOrUnderground(player) && NoEvents(player) && player.ZoneHallow && Main.hardMode;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool SurfaceDesert(Player player)
				=> SurfaceOrSky(player) && NoEvents(player) && player.ZoneDesert;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool SurfaceMushroom(Player player)
				=> SurfaceOrSky(player) && NoEvents(player) && player.ZoneGlowshroom;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool DepthsJungle(Player player)
				=> UndergroundOrCaverns(player) && NoEvents(player) && player.ZoneJungle;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool DepthsCorruption(Player player)
				=> player.ZoneRockLayerHeight && NoEvents(player) && player.ZoneCorrupt && Main.hardMode;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool DepthsCrimson(Player player)
				=> player.ZoneRockLayerHeight && NoEvents(player) && player.ZoneCrimson && Main.hardMode;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool DepthsSnow(Player player)
				=> UndergroundOrCaverns(player) && NoEvents(player) && player.ZoneSnow;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool DepthsDesert(Player player)
				=> UndergroundOrCaverns(player) && NoEvents(player) && player.ZoneUndergroundDesert;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool DepthsMushroom(Player player)
				=> UndergroundOrCaverns(player) && NoEvents(player) && player.ZoneGlowshroom;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool Underworld(Player player)
				=> player.ZoneUnderworldHeight && NoEvents(player);
		}

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
			NPCID.BlackRecluse,
			NPCID.BlackRecluseWall,
			NPCID.WallCreeper,
			NPCID.WallCreeperWall,
			NPCID.JungleCreeper,
			NPCID.JungleCreeperWall,
			NPCID.BloodCrawler,
			NPCID.BloodCrawlerWall,
			NPCID.DesertScorpionWalk,
			NPCID.DesertScorpionWall,
			NPCID.Nymph,
			NPCID.RayGunner,
			NPCID.SolarSpearman,
			NPCID.MothronSpawn,
			NPCID.LihzahrdCrawler,
			NPCID.Nutcracker,
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

		public static Dictionary<short, List<SortingProgression>> idsToProgressions;

		internal static void Load(){
			idsByProgression = new();

			InitializeRegistry();

			//Ensure that each progression enum has an entry
			foreach(var progression in Enum.GetValues<SortingProgression>())
				if(!idsByProgression.ContainsKey(progression))
					throw new Exception($"Progression value \"{nameof(SortingProgression)}.{progression}\" does not have an entry");

			idsToProgressions = new();
			for(short i = 0; i < NPCLoader.NPCCount; i++)
				idsToProgressions[i] = idsByProgression.Where(kvp => kvp.Value.Contains(i)).Select(kvp => kvp.Key).ToList();
		}

		internal static void Unload(){
			idsByProgression = null;
			idsToProgressions = null;
		}

		public static bool CanUseEntriesAtProgressionStage(SortingProgression progression, short spawningNPC, Player player)
			=> progression switch{
				PreHardmodeSurface => ZoneHelpers.SurfacePurity(player) && !Main.hardMode && Main.dayTime,
				PreHardmodeSurfaceNight => ZoneHelpers.SurfacePurity(player) && !Main.hardMode && !Main.dayTime,
				PreHardmodeSnowSurface => ZoneHelpers.SurfaceSnow(player) && !Main.hardMode && Main.dayTime,
				PreHardmodeSnowSurfaceNight => ZoneHelpers.SurfaceSnow(player) && !Main.hardMode && !Main.dayTime,
				DesertSurface => ZoneHelpers.SurfaceDesert(player) && !Main.hardMode,
				PreHardmodeSnowDepths => ZoneHelpers.DepthsSnow(player) && !Main.hardMode,
				RainEvent => Main.IsItRaining && !Main.hardMode,
				PreHardmodeUnderground => ZoneHelpers.NoEvents(player) && player.ZoneDirtLayerHeight && !Main.hardMode,
				SandstormEvent => Sandstorm.Happening && ZoneHelpers.SurfaceDesert(player) && !Main.hardMode,
				PreHardmodeCorruption => ZoneHelpers.SurfaceCorruption(player) && !Main.hardMode,
				PreHardmodeCrimson => ZoneHelpers.SurfaceCrimson(player) && !Main.hardMode,
				PreHardmodeCaverns => ZoneHelpers.NoEvents(player) && player.ZoneRockLayerHeight && !Main.hardMode,
				PreHardmodeMushroomBiomeSurface => ZoneHelpers.SurfaceMushroom(player) && !Main.hardMode,
				PreHardmodeMushroomBiomeDepths => ZoneHelpers.DepthsMushroom(player) && !Main.hardMode,
				MiniBiomeGraveyard => ZoneHelpers.NoEvents(player) && player.ZoneGraveyard && !Main.hardMode,
				KingSlime => idsByProgression[KingSlime].Contains(spawningNPC),
				CthulhuEye => idsByProgression[CthulhuEye].Contains(spawningNPC),
				BloodMoon => ZoneHelpers.SurfaceOrSky(player) && Main.bloodMoon && !Main.hardMode,
				JungleSurface => ZoneHelpers.SurfaceJungle(player) && !Main.hardMode && Main.dayTime,
				JungleSurfaceNight => ZoneHelpers.SurfaceJungle(player) && !Main.hardMode && !Main.dayTime,
				MiniBiomeBeeHive => CanUseEntriesAtProgressionStage(JungleDepths, spawningNPC, player),
				Ocean => ZoneHelpers.NoEvents(player) && player.ZoneOverworldHeight && player.ZoneBeach,
				DesertDepths => true,
				MiniBiomeGranite => true,
				MiniBiomeMarble => true,
				EvilBoss => true,
				MiniBiomeMeteorite => true,
				GoblinArmy => true,
				DD2Army => true,
				SkyBiome => true,
				JungleDepths => true,
				JungleDepthsNight => true,
				QueenBee => true,
				SpiderBiome => true,
				Skeletron => true,
				Dungeon => true,
				PreHardmodeHell => true,
				WallOfFlesh => true,
				HardmodeSurface => true,
				HardmodeMiniBiomeGraveyard => true,
				HardmodeSurfaceNight => true,
				HardmodeUnderground => true,
				HardmodeMushroomBiome => true,
				HardmodeSnowSurface => true,
				HardmodeDesertSurface => true,
				HallowSurface => true,
				HallowSurfaceNight => true,
				HardmodeCaverns => true,
				HardmodeMiniBiomeMarble => true,
				HardmodeSkyBiome => true,
				HardmodeSnowDepths => true,
				HardmodeCorruption => true,
				HardmodeCrimson => true,
				HallowDepths => true,
				GoblinArmyHardmode => true,
				HardmodeSpiderBiome => true,
				HardmodeDesertDepths => true,
				BloodMoonHardmode => true,
				HardmodeHell => true,
				SnowmanArmy => true,
				HardmodeRainEvent => true,
				HardmodeSandstormEvent => true,
				HardmodeCorruptionDepths => true,
				HardmodeCrimsonDepths => true,
				SolarEclipse => true,
				HardmodeJungleSurface => true,
				HardmodeJungleSurfaceNight => true,
				QueenSlime => true,
				Destroyer => true,
				PostMechHell => true,
				HardmodeJungleDepths => true,
				HardmodeJungleDepthsNight => true,
				PirateArmy => true,
				Twins => true,
				DD2ArmyTier2 => true,
				SkeletronPrime => true,
				SolarEclipsePostAllMechs => true,
				Plantera => true,
				PostPlanteraDungeon => true,
				SolarEclipsePostPlantera => true,
				PumpkinMoon => true,
				FrostMoon => true,
				Empress => true,
				LihzahrdTemple => true,
				Golem => true,
				MartianArmy => true,
				DD2ArmyTier3 => true,
				DukeFishron => true,
				Cultist => true,
				SolarPillar => true,
				NebulaPillar => true,
				VortexPillar => true,
				StardustPillar => true,
				MoonLord => true,
				_ => throw new Exception("Unknown progression enum value: " + progression)
			};

		private static void InitializeRegistry(){
			//Create a registry of NPC ids based on where they're expected to appear during progression
			//This will help with the creation of "fallback stats" for the NPC Statistics Registry

			idsByProgression[PreHardmodeSurface] = new(){
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

			idsByProgression[PreHardmodeSurfaceNight] = new(){
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
			idsByProgression[PreHardmodeSurfaceNight].AddZombieVariants();
			idsByProgression[PreHardmodeSurfaceNight].AddDemonEyeVariants();
			idsByProgression[PreHardmodeSurfaceNight].AddArmedZombies(includeHalloween: true, includeEskimo: false);

			idsByProgression[PreHardmodeSnowSurface] = new(){
				NPCID.IceSlime
			};

			idsByProgression[PreHardmodeSnowSurfaceNight] = new(){
				NPCID.ZombieEskimo,
				NPCID.ArmedZombieEskimo
			};

			idsByProgression[DesertSurface] = new(){
				NPCID.SandSlime,
				NPCID.Vulture,
				NPCID.Antlion
			};

			idsByProgression[PreHardmodeSnowDepths] = new(){
				NPCID.IceBat,
				NPCID.SnowFlinx,
				NPCID.SpikedIceSlime,
				NPCID.UndeadViking,
				NPCID.UndeadMiner,
				NPCID.CyanBeetle,
				NPCID.Nymph
			};

			idsByProgression[RainEvent] = new(){
				NPCID.FlyingFish,
				NPCID.UmbrellaSlime,
				NPCID.ZombieRaincoat
			};

			idsByProgression[PreHardmodeUnderground] = new(){
				NPCID.GiantWormHead,
				NPCID.BlueSlime,
				NPCID.RedSlime,
				NPCID.YellowSlime,
				NPCID.Pinky,
				NPCID.BlueJellyfish
			};

			idsByProgression[SandstormEvent] = new(){
				NPCID.Tumbleweed,
				NPCID.TombCrawlerHead
			};

			idsByProgression[PreHardmodeCorruption] = new(){
				NPCID.DevourerHead
			};
			idsByProgression[PreHardmodeCorruption].AddRange(CreateNetIDsFromBaseTypes(NPCID.EaterofSouls));

			idsByProgression[PreHardmodeCrimson] = new(){
				NPCID.BloodCrawler,
				NPCID.FaceMonster
			};
			idsByProgression[PreHardmodeCrimson].AddRange(CreateNetIDsFromBaseTypes(NPCID.Crimera));

			idsByProgression[PreHardmodeCaverns] = new(){
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
			idsByProgression[PreHardmodeCaverns].AddRange(CreateNetIDsFromBaseTypes(NPCID.Skeleton, NPCID.HeadacheSkeleton, NPCID.MisassembledSkeleton, NPCID.PantlessSkeleton));

			idsByProgression[PreHardmodeMushroomBiomeSurface] = new(){
				NPCID.AnomuraFungus,
				NPCID.FungiBulb,
				NPCID.MushiLadybug,
				NPCID.ZombieMushroomHat,
				NPCID.ZombieMushroom
			};

			idsByProgression[PreHardmodeMushroomBiomeDepths] = new(){
				NPCID.SporeBat,
				NPCID.SporeSkeleton
			};

			idsByProgression[MiniBiomeGraveyard] = new(){
				NPCID.MaggotZombie,
				NPCID.ZombieRaincoat,
				NPCID.ZombieEskimo,
				NPCID.TheGroom,
				NPCID.TheBride,
				NPCID.Raven,
				NPCID.Ghost,
				NPCID.ZombieMushroom,
				NPCID.ZombieMushroomHat
			};
			idsByProgression[MiniBiomeGraveyard].AddZombieVariants();
			idsByProgression[MiniBiomeGraveyard].AddDemonEyeVariants();
			idsByProgression[MiniBiomeGraveyard].AddArmedZombies(includeHalloween: true, includeEskimo: true);

			idsByProgression[KingSlime] = new(){
				NPCID.KingSlime,
				NPCID.SlimeSpiked,
				NPCID.BlueSlime
			};

			idsByProgression[CthulhuEye] = new(){
				NPCID.EyeofCthulhu
			};

			idsByProgression[BloodMoon] = new(){
				NPCID.TheGroom,
				NPCID.TheBride,
				NPCID.BloodZombie,
				NPCID.Drippler,
				NPCID.EyeballFlyingFish,
				NPCID.ZombieMerman
			};

			idsByProgression[JungleSurface] = new(){
				NPCID.JungleSlime,
				NPCID.JungleBat,
				NPCID.Piranha,
				NPCID.Snatcher
			};

			idsByProgression[JungleSurfaceNight] = new(){
				NPCID.JungleBat,
				NPCID.Piranha,
				NPCID.Snatcher,
				NPCID.DoctorBones
			};
			idsByProgression[JungleSurfaceNight].AddZombieVariants();
			idsByProgression[JungleSurfaceNight].AddDemonEyeVariants();
			idsByProgression[JungleSurfaceNight].AddArmedZombies(includeHalloween: true, includeEskimo: false);

			idsByProgression[MiniBiomeBeeHive] = new(){
				NPCID.Bee,
				NPCID.BeeSmall
			};
			idsByProgression[MiniBiomeBeeHive].AddHornetVariants();

			idsByProgression[Ocean] = new(){
				NPCID.PinkJellyfish,
				NPCID.Crab,
				NPCID.Squid,
				NPCID.SeaSnail,
				NPCID.Shark
			};

			idsByProgression[DesertDepths] = new(){
				NPCID.Antlion,
				NPCID.WalkingAntlion,
				NPCID.LarvaeAntlion,
				NPCID.FlyingAntlion,
				NPCID.GiantFlyingAntlion,
				NPCID.GiantWalkingAntlion,
				NPCID.SandSlime,
				NPCID.TombCrawlerHead
			};

			idsByProgression[MiniBiomeGranite] = new(){
				NPCID.GraniteFlyer,
				NPCID.GraniteGolem
			};

			idsByProgression[MiniBiomeMarble] = new(){
				NPCID.GreekSkeleton
			};

			idsByProgression[EvilBoss] = new(){
				NPCID.EaterofWorldsHead,
				NPCID.EaterofWorldsBody,
				NPCID.EaterofWorldsTail,
				NPCID.BrainofCthulhu,
				NPCID.Creeper
			};

			idsByProgression[MiniBiomeMeteorite] = new(){
				NPCID.MeteorHead
			};

			idsByProgression[GoblinArmy] = new(){
				NPCID.GoblinArcher,
				NPCID.GoblinPeon,
				NPCID.GoblinScout,
				NPCID.GoblinSorcerer,
				NPCID.GoblinThief,
				NPCID.GoblinWarrior
			};

			idsByProgression[DD2Army] = new(){
				NPCID.DD2GoblinT1,
				NPCID.DD2GoblinBomberT1,
				NPCID.DD2JavelinstT1,
				NPCID.DD2WyvernT1,
				NPCID.DD2DarkMageT1,
				NPCID.DD2SkeletonT1
			};

			idsByProgression[SkyBiome] = new(){
				NPCID.Harpy
			};

			idsByProgression[JungleDepths] = new(){
				NPCID.ManEater,
				NPCID.JungleBat,
				NPCID.SpikedJungleSlime,
				NPCID.Piranha,
				NPCID.LacBeetle
			};
			idsByProgression[JungleDepths].AddHornetVariants();

			idsByProgression[JungleDepthsNight] = new(){
				NPCID.DoctorBones
			};
			idsByProgression[JungleDepthsNight].AddRange(idsByProgression[JungleDepths]);

			idsByProgression[QueenBee] = new(){
				NPCID.QueenBee,
				NPCID.Bee,
				NPCID.BeeSmall
			};

			idsByProgression[SpiderBiome] = new(){
				NPCID.WallCreeper
			};

			idsByProgression[Skeletron] = new(){
				NPCID.SkeletronHead,
				NPCID.SkeletronHand
			};

			idsByProgression[Dungeon] = new(){
				NPCID.AngryBones,
				NPCID.AngryBonesBig,
				NPCID.AngryBonesBigHelmet,
				NPCID.AngryBonesBigMuscle,
				NPCID.DarkCaster,
				NPCID.CursedSkull,
				NPCID.DungeonSlime,
				NPCID.DungeonGuardian
			};

			idsByProgression[PreHardmodeHell] = new(){
				NPCID.Hellbat,
				NPCID.LavaSlime,
				NPCID.FireImp,
				NPCID.Demon,
				NPCID.VoodooDemon,
				NPCID.BoneSerpentHead
			};

			idsByProgression[WallOfFlesh] = new(){
				NPCID.WallofFlesh,
				NPCID.WallofFleshEye,
				NPCID.TheHungryII
			};

			idsByProgression[HardmodeSurface] = new(){
				NPCID.Mimic
			};
			idsByProgression[HardmodeSurface].AddRange(idsByProgression[PreHardmodeSurface]);

			idsByProgression[HardmodeMiniBiomeGraveyard] = new(){
				NPCID.HoppinJack
			};
			idsByProgression[HardmodeMiniBiomeGraveyard].AddRange(idsByProgression[MiniBiomeGraveyard]);

			idsByProgression[HardmodeSurfaceNight] = new(){
				NPCID.PossessedArmor,
				NPCID.WanderingEye,
				NPCID.Wraith,
				NPCID.Werewolf,
				NPCID.HoppinJack
			};
			idsByProgression[HardmodeSurfaceNight].AddRange(idsByProgression[PreHardmodeSurfaceNight]);

			idsByProgression[HardmodeUnderground] = new(){
				NPCID.DiggerHead,
				NPCID.PossessedArmor,
				NPCID.ToxicSludge,
				NPCID.GreenJellyfish,
				NPCID.Mimic
			};
			idsByProgression[HardmodeUnderground].AddRange(idsByProgression[PreHardmodeUnderground]);

			idsByProgression[HardmodeMushroomBiome] = new(){
				NPCID.FungoFish,
				NPCID.GiantFungiBulb
			};
			idsByProgression[HardmodeMushroomBiome].AddRange(idsByProgression[PreHardmodeMushroomBiomeSurface]);
			idsByProgression[HardmodeMushroomBiome].AddRange(idsByProgression[PreHardmodeMushroomBiomeDepths]);

			idsByProgression[HardmodeSnowSurface] = new();
			idsByProgression[HardmodeSnowSurface].AddRange(idsByProgression[PreHardmodeSnowSurface]);

			idsByProgression[HardmodeDesertSurface] = new(){
				NPCID.Mummy,
				NPCID.LightMummy,
				NPCID.DarkMummy,
				NPCID.BloodMummy
			};
			idsByProgression[HardmodeDesertSurface].AddRange(idsByProgression[DesertSurface]);

			idsByProgression[HallowSurface] = new(){
				NPCID.Pixie,
				NPCID.Unicorn,
				NPCID.LightMummy
			};

			idsByProgression[HallowSurfaceNight] = new(){
				NPCID.Gastropod
			};
			idsByProgression[HallowSurfaceNight].AddRange(idsByProgression[HallowSurface]);

			idsByProgression[HardmodeCaverns] = new(){
				NPCID.DiggerHead,
				NPCID.GiantBat,
				NPCID.AnglerFish,
				NPCID.GreenJellyfish,
				NPCID.RockGolem,
				NPCID.SkeletonArcher,
				NPCID.RuneWizard,
				NPCID.Mimic
			};
			idsByProgression[HardmodeCaverns].AddRange(CreateNetIDsFromBaseTypes(NPCID.ArmoredSkeleton));
			idsByProgression[HardmodeCaverns].AddRange(idsByProgression[PreHardmodeCaverns]);

			idsByProgression[HardmodeMiniBiomeMarble] = new(){
				NPCID.Medusa
			};
			idsByProgression[HardmodeMiniBiomeMarble].AddRange(idsByProgression[MiniBiomeMarble]);

			idsByProgression[HardmodeSkyBiome] = new(){
				NPCID.WyvernHead
			};
			idsByProgression[HardmodeSkyBiome].AddRange(idsByProgression[SkyBiome]);

			idsByProgression[HardmodeSnowDepths] = new(){
				NPCID.ArmoredViking,
				NPCID.IceTortoise,
				NPCID.IceElemental,
				NPCID.IcyMerman,
				NPCID.IceMimic,
				NPCID.PigronCorruption,
				NPCID.PigronCrimson,
				NPCID.PigronHallow
			};
			idsByProgression[HardmodeSnowDepths].AddRange(idsByProgression[PreHardmodeSnowDepths]);

			idsByProgression[HardmodeCorruption] = new(){
				NPCID.Corruptor,
				NPCID.CorruptSlime,
				NPCID.Slimeling,
				NPCID.Slimer,
				NPCID.DarkMummy,
				NPCID.Mimic
			};
			idsByProgression[HardmodeCorruption].AddRange(CreateNetIDsFromBaseTypes(NPCID.CorruptSlime));

			idsByProgression[HardmodeCrimson] = new(){
				NPCID.Herpling,
				NPCID.BloodJelly,
				NPCID.BloodFeeder,
				NPCID.BloodMummy,
				NPCID.Mimic
			};
			idsByProgression[HardmodeCrimson].AddRange(CreateNetIDsFromBaseTypes(NPCID.Crimslime));

			idsByProgression[HallowDepths] = new(){
				NPCID.IlluminantSlime,
				NPCID.IlluminantBat,
				NPCID.ChaosElemental,
				NPCID.EnchantedSword,
				NPCID.BigMimicHallow,
				NPCID.PigronHallow,
				NPCID.DesertGhoulHallow,
				NPCID.Mimic
			};

			idsByProgression[GoblinArmyHardmode] = new(){
				NPCID.GoblinSummoner
			};
			idsByProgression[GoblinArmyHardmode].AddRange(idsByProgression[GoblinArmy]);

			idsByProgression[HardmodeSpiderBiome] = new(){
				NPCID.BlackRecluse
			};

			idsByProgression[HardmodeDesertDepths] = new(){
				NPCID.DesertBeast,
				NPCID.DesertScorpionWalk,
				NPCID.DesertLamiaDark,
				NPCID.DesertLamiaLight,
				NPCID.DuneSplicerHead,
				NPCID.DesertGhoul,
				NPCID.DesertGhoulCorruption,
				NPCID.DesertGhoulCrimson,
				NPCID.DesertGhoulHallow,
				NPCID.DesertDjinn
			};
			idsByProgression[HardmodeDesertDepths].AddRange(idsByProgression[DesertDepths]);

			idsByProgression[BloodMoonHardmode] = new(){
				NPCID.Clown,
				NPCID.ChatteringTeethBomb,
				NPCID.GoblinShark,
				NPCID.BloodEelHead,
				NPCID.BloodNautilus,
				NPCID.BloodSquid
			};
			idsByProgression[BloodMoonHardmode].AddRange(idsByProgression[BloodMoon]);

			idsByProgression[HardmodeHell] = new(){
				NPCID.Mimic
			};
			idsByProgression[HardmodeHell].AddRange(idsByProgression[PreHardmodeHell]);

			idsByProgression[SnowmanArmy] = new(){
				NPCID.MisterStabby,
				NPCID.SnowmanGangsta,
				NPCID.SnowBalla
			};

			idsByProgression[HardmodeRainEvent] = new(){
				NPCID.AngryNimbus,
				NPCID.RainbowSlime,
				NPCID.IceGolem
			};
			idsByProgression[RainEvent].AddRange(idsByProgression[RainEvent]);

			idsByProgression[HardmodeSandstormEvent] = new(){
				NPCID.SandElemental,
				NPCID.DuneSplicerHead,
				NPCID.SandShark,
				NPCID.SandsharkCorrupt,
				NPCID.SandsharkCrimson,
				NPCID.SandsharkHallow
			};
			idsByProgression[HardmodeSandstormEvent].AddRange(idsByProgression[SandstormEvent]);

			idsByProgression[HardmodeCorruptionDepths] = new(){
				NPCID.DevourerHead,
				NPCID.SeekerHead,
				NPCID.CursedHammer,
				NPCID.Clinger,
				NPCID.Corruptor,
				NPCID.Slimer,
				NPCID.BigMimicCorruption,
				NPCID.PigronCorruption,
				NPCID.DesertGhoulCorruption,
				NPCID.Mimic
			};
			idsByProgression[HardmodeCorruptionDepths].AddRange(CreateNetIDsFromBaseTypes(NPCID.CorruptSlime));

			idsByProgression[HardmodeCrimsonDepths] = new(){
				NPCID.BloodJelly,
				NPCID.BloodFeeder,
				NPCID.CrimsonAxe,
				NPCID.IchorSticker,
				NPCID.FloatyGross,
				NPCID.BigMimicCrimson,
				NPCID.PigronCrimson,
				NPCID.DesertGhoulCrimson,
				NPCID.Mimic
			};

			idsByProgression[SolarEclipse] = new(){
				NPCID.Eyezor,
				NPCID.Frankenstein,
				NPCID.SwampThing,
				NPCID.Vampire,
				NPCID.CreatureFromTheDeep,
				NPCID.Fritz,
				NPCID.ThePossessed
			};

			idsByProgression[HardmodeJungleSurface] = new(){
				NPCID.Derpling,
				NPCID.GiantTortoise,
				NPCID.AnglerFish,
				NPCID.Arapaima,
				NPCID.AngryTrapper
			};
			idsByProgression[HardmodeJungleSurface].AddRange(idsByProgression[JungleSurface]);

			idsByProgression[HardmodeJungleSurfaceNight] = new(){
				NPCID.GiantTortoise,
				NPCID.GiantFlyingFox,
				NPCID.AnglerFish,
				NPCID.Arapaima,
				NPCID.AngryTrapper
			};
			idsByProgression[HardmodeJungleSurfaceNight].AddRange(idsByProgression[JungleSurfaceNight]);

			idsByProgression[QueenSlime] = new(){
				NPCID.QueenSlimeBoss,
				NPCID.QueenSlimeMinionBlue,
				NPCID.QueenSlimeMinionPink,
				NPCID.QueenSlimeMinionPurple
			};

			idsByProgression[Destroyer] = new(){
				NPCID.TheDestroyer,
				NPCID.TheDestroyerBody,
				NPCID.TheDestroyerTail,
				NPCID.Probe
			};

			idsByProgression[PostMechHell] = new(){
				NPCID.Lavabat,
				NPCID.RedDevil
			};
			idsByProgression[PostMechHell].AddRange(idsByProgression[HardmodeHell]);

			idsByProgression[HardmodeJungleDepths] = new(){
				NPCID.JungleCreeper,
				NPCID.Moth,
				NPCID.MossHornet,
				NPCID.AngryTrapper,
				NPCID.Arapaima,
				NPCID.GiantTortoise
			};
			idsByProgression[HardmodeJungleDepths].AddRange(idsByProgression[JungleDepths]);

			idsByProgression[HardmodeJungleDepthsNight] = new(){
				NPCID.JungleCreeper,
				NPCID.Moth,
				NPCID.MossHornet,
				NPCID.AngryTrapper,
				NPCID.Arapaima,
				NPCID.GiantTortoise
			};
			idsByProgression[HardmodeJungleDepthsNight].AddRange(idsByProgression[JungleDepthsNight]);

			idsByProgression[PirateArmy] = new(){
				NPCID.Parrot,
				NPCID.PirateCaptain,
				NPCID.PirateCorsair,
				NPCID.PirateCrossbower,
				NPCID.PirateDeadeye,
				NPCID.PirateDeckhand,
				NPCID.PirateGhost,
				NPCID.PirateShip,
				NPCID.PirateShipCannon
			};

			idsByProgression[Twins] = new(){
				NPCID.Spazmatism,
				NPCID.Retinazer
			};

			idsByProgression[DD2ArmyTier2] = new(){
				NPCID.DD2GoblinT2,
				NPCID.DD2GoblinBomberT2,
				NPCID.DD2JavelinstT2,
				NPCID.DD2WyvernT2,
				NPCID.DD2KoboldFlyerT2,
				NPCID.DD2KoboldWalkerT2,
				NPCID.DD2WitherBeastT2,
				NPCID.DD2OgreT2
			};

			idsByProgression[SkeletronPrime] = new(){
				NPCID.SkeletronPrime,
				NPCID.PrimeCannon,
				NPCID.PrimeLaser,
				NPCID.PrimeSaw,
				NPCID.PrimeVice
			};

			idsByProgression[SolarEclipsePostAllMechs] = new(){
				NPCID.Reaper
			};
			idsByProgression[SolarEclipsePostAllMechs].AddRange(idsByProgression[SolarEclipse]);

			idsByProgression[Plantera] = new(){
				NPCID.Plantera,
				NPCID.PlanterasTentacle
			};

			idsByProgression[PostPlanteraDungeon] = new(){
				NPCID.BlueArmoredBones,
				NPCID.BlueArmoredBonesMace,
				NPCID.BlueArmoredBonesNoPants,
				NPCID.BlueArmoredBonesSword,
				NPCID.RustyArmoredBonesAxe,
				NPCID.RustyArmoredBonesFlail,
				NPCID.RustyArmoredBonesSword,
				NPCID.RustyArmoredBonesSwordNoArmor,
				NPCID.HellArmoredBones,
				NPCID.HellArmoredBonesMace,
				NPCID.HellArmoredBonesSpikeShield,
				NPCID.HellArmoredBonesSword,
				NPCID.Paladin,
				NPCID.Necromancer,
				NPCID.NecromancerArmored,
				NPCID.RaggedCaster,
				NPCID.RaggedCasterOpenCoat,
				NPCID.DiabolistRed,
				NPCID.DiabolistWhite,
				NPCID.SkeletonCommando,
				NPCID.SkeletonSniper,
				NPCID.TacticalSkeleton,
				NPCID.GiantCursedSkull,
				NPCID.BoneLee,
				NPCID.DungeonSpirit
			};

			idsByProgression[SolarEclipsePostPlantera] = new(){
				NPCID.MothronEgg,
				NPCID.Mothron,
				NPCID.Butcher,
				NPCID.DeadlySphere,
				NPCID.DrManFly,
				NPCID.Nailhead,
				NPCID.Psycho
			};
			idsByProgression[SolarEclipsePostPlantera].AddRange(idsByProgression[SolarEclipsePostAllMechs]);

			idsByProgression[PumpkinMoon] = new(){
				NPCID.Scarecrow1,
				NPCID.Scarecrow2,
				NPCID.Scarecrow3,
				NPCID.Scarecrow4,
				NPCID.Scarecrow5,
				NPCID.Scarecrow6,
				NPCID.Scarecrow7,
				NPCID.Scarecrow8,
				NPCID.Scarecrow9,
				NPCID.Scarecrow10,
				NPCID.Splinterling,
				NPCID.Hellhound,
				NPCID.Poltergeist,
				NPCID.HeadlessHorseman,
				NPCID.MourningWood,
				NPCID.Pumpking
			};

			idsByProgression[FrostMoon] = new(){
				NPCID.PresentMimic,
				NPCID.Flocko,
				NPCID.GingerbreadMan,
				NPCID.ZombieElf,
				NPCID.ZombieElfBeard,
				NPCID.ZombieElfGirl,
				NPCID.ElfArcher,
				NPCID.Nutcracker,
				NPCID.Yeti,
				NPCID.ElfCopter,
				NPCID.Krampus,
				NPCID.Everscream,
				NPCID.SantaNK1,
				NPCID.IceQueen
			};

			idsByProgression[Empress] = new(){
				NPCID.HallowBoss
			};

			idsByProgression[LihzahrdTemple] = new(){
				NPCID.Lihzahrd,
				NPCID.FlyingSnake
			};

			idsByProgression[Golem] = new(){
				NPCID.Golem,
				NPCID.GolemFistLeft,
				NPCID.GolemFistRight
			};

			idsByProgression[MartianArmy] = new(){
				NPCID.MartianSaucerCannon,
				NPCID.MartianSaucerTurret,
				NPCID.MartianSaucerCore,
				NPCID.Scutlix,
				NPCID.ScutlixRider,
				NPCID.MartianWalker,
				NPCID.MartianDrone,
				NPCID.MartianTurret,
				NPCID.GigaZapper,
				NPCID.MartianEngineer,
				NPCID.MartianOfficer,
				NPCID.RayGunner,
				NPCID.GrayGrunt,
				NPCID.BrainScrambler
			};

			idsByProgression[DD2ArmyTier3] = new(){
				NPCID.DD2DarkMageT3,
				NPCID.DD2DrakinT3,
				NPCID.DD2GoblinBomberT3,
				NPCID.DD2GoblinT3,
				NPCID.DD2JavelinstT3,
				NPCID.DD2KoboldFlyerT3,
				NPCID.DD2KoboldWalkerT3,
				NPCID.DD2LightningBugT3,
				NPCID.DD2OgreT3,
				NPCID.DD2SkeletonT3,
				NPCID.DD2WitherBeastT3,
				NPCID.DD2WyvernT3,
				NPCID.DD2Betsy
			};

			idsByProgression[DukeFishron] = new(){
				NPCID.DukeFishron,
				NPCID.Sharkron,
				NPCID.Sharkron2
			};

			idsByProgression[Cultist] = new(){
				NPCID.CultistBoss,
				NPCID.CultistDragonHead,
				NPCID.AncientCultistSquidhead
			};

			idsByProgression[SolarPillar] = new(){
				NPCID.SolarCorite,
				NPCID.SolarCrawltipedeHead,
				NPCID.SolarSpearman,
				NPCID.SolarDrakomire,
				NPCID.SolarDrakomireRider,
				NPCID.SolarSolenian,
				NPCID.SolarSroller,
				NPCID.LunarTowerSolar
			};

			idsByProgression[NebulaPillar] = new(){
				NPCID.NebulaHeadcrab,
				NPCID.NebulaBeast,
				NPCID.NebulaBrain,
				NPCID.NebulaSoldier,
				NPCID.LunarTowerNebula
			};

			idsByProgression[VortexPillar] = new(){
				NPCID.VortexHornet,
				NPCID.VortexLarva,
				NPCID.VortexHornetQueen,
				NPCID.VortexRifleman,
				NPCID.VortexSoldier,
				NPCID.LunarTowerVortex
			};

			idsByProgression[StardustPillar] = new(){
				NPCID.StardustJellyfishBig,
				NPCID.StardustWormHead,
				NPCID.StardustCellBig,
				NPCID.StardustSoldier,
				NPCID.StardustSpiderBig,
				NPCID.StardustSpiderSmall,
				NPCID.LunarTowerStardust
			};

			idsByProgression[MoonLord] = new(){
				NPCID.MoonLordCore,
				NPCID.MoonLordLeechBlob
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

		private static void AddZombieVariants(this List<short> ids){
			ids.AddRange(CreateNetIDsFromBaseTypes(NPCID.Zombie, NPCID.BaldZombie, NPCID.PincushionZombie, NPCID.SlimedZombie, NPCID.SwampZombie, NPCID.TwiggyZombie, NPCID.FemaleZombie));
		}

		private static void AddDemonEyeVariants(this List<short> ids){
			ids.AddRange(CreateNetIDsFromBaseTypes(NPCID.CataractEye, NPCID.SleepyEye, NPCID.DialatedEye, NPCID.GreenEye, NPCID.PurpleEye, NPCID.DemonEye));
		}

		private static void AddArmedZombies(this List<short> ids, bool includeHalloween, bool includeEskimo){
			ids.Add(NPCID.ArmedTorchZombie);
			ids.Add(NPCID.ArmedZombie);
			ids.Add(NPCID.ArmedZombiePincussion);
			ids.Add(NPCID.ArmedZombieSlimed);
			ids.Add(NPCID.ArmedZombieSwamp);
			ids.Add(NPCID.ArmedZombieTwiggy);

			if(includeHalloween)
				ids.Add(NPCID.ArmedZombieCenx);

			if(includeEskimo)
				ids.Add(NPCID.ArmedZombieEskimo);
		}

		private static void AddHornetVariants(this List<short> ids){
			ids.AddRange(CreateNetIDsFromBaseTypes(NPCID.Hornet, NPCID.HornetFatty, NPCID.HornetHoney, NPCID.HornetLeafy, NPCID.HornetSpikey, NPCID.HornetStingy));
		}
	}
}
