﻿using AARPG.API.Sorting;
using AARPG.Core.Mechanics;
using AARPG.Core.Players;
using AARPG.Core.Systems;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AARPG.Core.NPCs{
	public class StatNPC : GlobalNPC{
		public NPCStatistics stats;

		public float endurance;

		internal string namePrefix;

		internal bool DelayedStatAssignment{ get; private set; }

		public override bool CloneNewInstances => true;

		public override bool InstancePerEntity => true;

		//AppliesToEntity is called by NPCLoader.SetDefaults... Perfect!
		public override bool AppliesToEntity(NPC entity, bool lateInstantiation){
			int idOverride = API.Edits.Detours.Vanilla.NetIDOverride;
			int netID = idOverride != 0 ? idOverride : entity.type;
			return lateInstantiation && (NPCStatisticsRegistry.HasStats(netID) || NPCProgressionRegistry.NonSeparableWormNPCToHead.ContainsKey(netID));
		}

		public override void SetDefaults(NPC npc){
			//Use "netID" instead of "type" to support the different net types (e.g. slimes and zombies)
			int idOverride = API.Edits.Detours.Vanilla.NetIDOverride;
			int netID = idOverride != 0 ? idOverride : npc.type;

			if(API.Edits.Detours.Vanilla.TransformingNPC && NPCProgressionRegistry.TransformingNPCs.Contains(netID)){
				//Transforming NPCs should carry the stats
				stats = API.Edits.Detours.Vanilla.PreTransformStats;
			}else if(NPCProgressionRegistry.NonSeparableWormNPCToHead.TryGetValue(netID, out _)){
				//Use the stats from the head NPC, unless this NPC isn't being spawned by the head
				//Indicate that the NPC needs stats, but getting them should be delayed
				DelayedStatAssignment = true;
				return;
			}else{
				NPCStatisticsRegistry.Entry entry = NPCStatisticsRegistry.GetRandomStats(netID);
				stats = entry?.stats;
				namePrefix = entry.namePrefix is null ? "" : (entry.namePrefix + " ");
			}

			ApplyStatsAndNamePrefix(npc, netID);
		}

		public override void PostAI(NPC npc){
			if(!npc.active || !DelayedStatAssignment)
				return;

			DelayedStatAssignment = false;

			if(NPCProgressionRegistry.NonSeparableWormNPCToHead.TryGetValue(npc.netID, out int headType)){
				//All vanilla worms set the "head" NPC's whoAmI to npc.ai[3] and npc.realLife
				if(npc.realLife >= 0 && npc.realLife == npc.ai[3]){
					NPC headNPC = Main.npc[npc.realLife];

					if(headNPC.active && headNPC.type == headType && headNPC.TryGetGlobalNPC(out StatNPC headStats)){
						stats = headStats.stats;

						ApplyStatsAndNamePrefix(npc, npc.netID);
					}
				}
			}
		}

		private void ApplyStatsAndNamePrefix(NPC npc, int netID){
			if(stats is not null){
				stats.ApplyTo(npc);
				
				string lvl = $"[Lv. {stats.level}]";
				string netName = Lang.GetNPCNameValue(netID);

				//Ensure that names like "The Groom" end up as "The Large Groom" instead of "Large The Groom"
				if(netName.StartsWith("The "))
					npc.GivenName = $"{lvl} The {namePrefix}{netName[4..]}";
				else
					npc.GivenName = $"{lvl} {namePrefix}{netName}";
			}
		}

		public override void ModifyHitByItem(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit){
			ApplyEndurance(ref damage);
		}

		public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection){
			ApplyEndurance(ref damage);
		}

		public override void ModifyHitNPC(NPC npc, NPC target, ref int damage, ref float knockback, ref bool crit){
			if(target.TryGetGlobalNPC<StatNPC>(out var statNPC))
				statNPC.ApplyEndurance(ref damage);
		}

		private void ApplyEndurance(ref int damage)
			=> damage = Math.Max(1, (int)(damage * (1f - Math.Min(0.9999f, endurance))));

		public override void HitEffect(NPC npc, int hitDirection, double damage){
			if(npc.life <= 0){
				for(int i = 0; i < Main.maxPlayers; i++){
					Player player = Main.player[i];

					if(!player.active || player.dead || !npc.playerInteraction[i])
						continue;

					StatPlayer statPlayer = player.GetModPlayer<StatPlayer>();

					if(npc.TryGetGlobalNPC<StatNPC>(out var statNPC) && statNPC.stats is not null && !npc.SpawnedFromStatue){
						bool hasCount = statPlayer.downedCountsByID.TryGetValue((short)npc.netID, out int count);
						//Killing more of the same boss gives the player less and less XP
						int xp = statNPC.stats.xp;
						if(hasCount)
							xp = (int)(xp * 1f / count);

						//Spawn the experience
						while(xp > 0){
							int toSpawn;
							if(xp >= 10000){
								toSpawn = 10000;
								xp -= 10000;
							}else if(xp >= 5000){
								toSpawn = 5000;
								xp -= 5000;
							}else if(xp >= 1000){
								toSpawn = 1000;
								xp -= 1000;
							}else if(xp >= 500){
								toSpawn = 500;
								xp -= 500;
							}else if(xp >= 100){
								toSpawn = 100;
								xp -= 100;
							}else if(xp >= 50){
								toSpawn = 50;
								xp -= 50;
							}else if(xp >= 10){
								toSpawn = 10;
								xp -= 10;
							}else if(xp >= 5){
								toSpawn = 5;
								xp -= 5;
							}else{
								toSpawn = 1;
								xp--;
							}

							ExperienceTracker.SpawnExperience(toSpawn, npc.Center, Vector2.UnitX.RotatedByRandom(MathHelper.Pi) * 6f, player.whoAmI);
						}

						if(npc.boss){
							if(!hasCount)
								statPlayer.downedCountsByID.Add((short)npc.netID, 0);

							statPlayer.downedCountsByID[(short)npc.netID]++;
						}
					}
				}
			}
		}
	}
}
