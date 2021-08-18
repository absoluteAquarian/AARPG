namespace AARPG.API.Sorting{
	/// <summary>
	/// A loosely-ordered set of locations/conditions intended to be reached during progression
	/// </summary>
	public enum SortingProgression{
		/// <summary>
		/// Available while on the surface
		/// </summary>
		PreHardmodeSurface,
		/// <summary>
		/// Available while on the surface during nighttime
		/// </summary>
		PreHardmodeSurfaceNight,
		/// <summary>
		/// Available on the surface of the Snow biome
		/// </summary>
		PreHardmodeSnowSurface,
		/// <summary>
		/// Available on the surface of the Snow biome
		/// </summary>
		PreHardmodeSnowSurfaceNight,
		/// <summary>
		/// Available in the depths of the Snow biome
		/// </summary>
		PreHardmodeSnowDepths,
		/// <summary>
		/// Available during the Rain/Blizzard event
		/// </summary>
		RainEvent,
		/// <summary>
		/// Available while in the Underground layer
		/// </summary>
		PreHardmodeUnderground,
		/// <summary>
		/// Available during the Sandstorm event
		/// </summary>
		SandstormEvent,
		/// <summary>
		/// Available on the surface of the Corruption
		/// </summary>
		PreHardmodeCorruptionSurface,
		/// <summary>
		/// Available on the surface of the Crimson
		/// </summary>
		PreHardmodeCrimsonSurface,
		/// <summary>
		/// Available while in the Caverns layer
		/// </summary>
		PreHardmodeCaverns,
		/// <summary>
		/// Available in the Glowing Mushroom biome
		/// </summary>
		PreHardmodeMushroomBiome,
		/// <summary>
		/// Available while fighting King Slime
		/// </summary>
		KingSlime,
		/// <summary>
		/// Available while fighting the Eye of Cthulhu
		/// </summary>
		CthulhuEye,
		/// <summary>
		/// Available during the Blood Moon event
		/// </summary>
		BloodMoon,
		/// <summary>
		/// Available on the surface of the Desert
		/// </summary>
		DesertSurface,
		/// <summary>
		/// Available on the surface of the Jungle
		/// </summary>
		JungleSurface,
		/// <summary>
		/// Available in the Ocean
		/// </summary>
		Ocean,
		/// <summary>
		/// Available in the depths of the Desert
		/// </summary>
		DesertDepths,
		/// <summary>
		/// Available while fighting the Eater of Worlds or the Brain of Cthulhu
		/// </summary>
		EvilBoss,
		/// <summary>
		/// Available during the Goblin Army event
		/// </summary>
		GoblinArmy,
		/// <summary>
		/// Available during the Old One's Army event
		/// </summary>
		DD2Army,
		/// <summary>
		/// Available in the Sky/Space biome
		/// </summary>
		SkyBome,
		/// <summary>
		/// Available in the depths of the Jungle
		/// </summary>
		JungleDepths,
		/// <summary>
		/// Available while fighting Queen Bee
		/// </summary>
		QueenBee,
		/// <summary>
		/// Available while in a Spider biome
		/// </summary>
		SpiderBiome,
		/// <summary>
		/// Available in the Dungeon after defeating Skeletron
		/// </summary>
		Dungeon,
		/// <summary>
		/// Available while in the Underworld
		/// </summary>
		PreHardmodeHell,
		/// <summary>
		/// Available while on the surface during Hardmode
		/// </summary>
		HardmodeSurface,
		/// <summary>
		/// Available while on the surface during Hardmode and nighttime
		/// </summary>
		HardmodeSurfaceNight,
		/// <summary>
		/// Available while in the Underground layer during Hardmode
		/// </summary>
		HardmodeUnderground,
		/// <summary>
		/// Available while in the Caverns layer during Hardmode
		/// </summary>
		HardmodeCaverns,
		/// <summary>
		/// Available in the Glowing Mushroom biome during Hardmode
		/// </summary>
		HardmodeMushroomBiome,
		/// <summary>
		/// Available while in the Underworld during Hardmode
		/// </summary>
		HardmodeHell,
		/// <summary>
		/// Available on the surface of the Snow biome during Hardmode
		/// </summary>
		HardmodeSnowSurface,
		/// <summary>
		/// Available on the surface of the Hallow
		/// </summary>
		HallowSurface,
		/// <summary>
		/// Available in the Sky/Space biome during Hardmode
		/// </summary>
		HardmodeSkyBome,
		/// <summary>
		/// Available in the depths of the Snow biome during Hardmode
		/// </summary>
		HardmodeSnowDepths,
		/// <summary>
		/// Available in the depths of the Hallow
		/// </summary>
		HallowDepths,
		/// <summary>
		/// Available during the Goblin Army event while in Hardmode
		/// </summary>
		GoblinArmyHardmode,
		/// <summary>
		/// Available in the depths of the Corruption during Hardmode
		/// </summary>
		HardmodeCorruptionDepths,
		/// <summary>
		/// Available in the depths of the Crimson during Hardmode
		/// </summary>
		HardmodeCrimsonDepths,
		/// <summary>
		/// Available while in a Spider biome during Hardmode
		/// </summary>
		HardmodeSpiderBiome,
		/// <summary>
		/// Available during the Blood Moon event while in Hardmode
		/// </summary>
		BloodMoonHardmode,
		/// <summary>
		/// Available during the Snowman Legion event
		/// </summary>
		SnowmanArmy,
		/// <summary>
		/// Available during the Rain/Blizzard event while in Hardmode
		/// </summary>
		HardmodeRainEvent,
		/// <summary>
		/// Available during the Sandstorm event while in Hardmode
		/// </summary>
		HardmodeSandstormEvent,
		/// <summary>
		/// Available during the Solar Eclipse event
		/// </summary>
		SolarEclipse,
		/// <summary>
		/// Available on the surface of the Jungle during Hardmode
		/// </summary>
		JungleSurfaceHardmode,
		/// <summary>
		/// Available while fighting Queen Slime
		/// </summary>
		QueenSlime,
		/// <summary>
		/// Available while fighting The Destroyer
		/// </summary>
		Destroyer,
		/// <summary>
		/// Available in the depths of the Jungle during hardmode
		/// </summary>
		JungleDepthsHardmode,
		/// <summary>
		/// Available during the Pirate Invasion event
		/// </summary>
		PirateArmy,
		/// <summary>
		/// Available while fighting The Twins
		/// </summary>
		Twins,
		/// <summary>
		/// Available during the Old One's Army event after defeating any mech boss
		/// </summary>
		DD2ArmyTier2,
		/// <summary>
		/// Available while fighting Skeletron Prime
		/// </summary>
		SkeletronPrime,
		/// <summary>
		/// Available during the Solar Eclipse event after defeating all 3 mech bosses
		/// </summary>
		SolarEclipsePostAllMechs,
		/// <summary>
		/// Available while fighting Plantera
		/// </summary>
		Plantera,
		/// <summary>
		/// Available while in the Dungeon after defeating Plantera
		/// </summary>
		PostPlanteraDungeon,
		/// <summary>
		/// Available during the Solar Eclipse event after defeating Plantera
		/// </summary>
		SolarEclipsePostPlantera,
		/// <summary>
		/// Available while fighting the Empress of Light
		/// </summary>
		Empress,
		/// <summary>
		/// Available while in the Jungle Temple
		/// </summary>
		LihzahrdTemple,
		/// <summary>
		/// Available while fighting Golem
		/// </summary>
		Golem,
		/// <summary>
		/// Available during the Martian Invasion event
		/// </summary>
		MartianArmy,
		/// <summary>
		/// Available during the Old One's Army event after defeating Golem
		/// </summary>
		DD2ArmyTier3,
		/// <summary>
		/// Available while fighting Duke Fishron
		/// </summary>
		DukeFishron,
		/// <summary>
		/// Available while fighting the Lunatic Cultist
		/// </summary>
		Cultist,
		/// <summary>
		/// Available while fighting the Solar Pillar
		/// </summary>
		SolarPillar,
		/// <summary>
		/// Available while fighting the Nebula Pillar
		/// </summary>
		NebulaPillar,
		/// <summary>
		/// Available while fighting the Vortex Pillar
		/// </summary>
		VortexPillar,
		/// <summary>
		/// Available while fighting the Stardust Pillar
		/// </summary>
		StardustPillar,
		/// <summary>
		/// Available while fighting the Moon Lord
		/// </summary>
		MoonLord
	}
}
