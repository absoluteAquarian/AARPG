using AARPG.Core.Utility;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AARPG.Core.Mechanics{
	/// <summary>
	/// An object representing the statistics for the player
	/// </summary>
	public class PlayerStatistics : Statistics{
		public struct ModifierData{
			public Modifier modifier;
			public int crit;
		}

		private struct UnloadedDamageClass{
			public string mod;
			public string name;

			public Modifier modifier;

			public int crit;
		}

		private static int[] xpRequirementsPerLevel;

		public static int MaxLevel{ get; private set; } = -1;

		public static RefReadOnlyArray<int> XPRequirementsPerLevel => new RefReadOnlyArray<int>(xpRequirementsPerLevel);

		internal static void InitializeXPRequirements(){
			//Already initialized if this is true
			if(MaxLevel > -1)
				return;

			MaxLevel = 100;
			xpRequirementsPerLevel = new int[MaxLevel + 1];

			for(int i = 1; i < MaxLevel + 1; i++){
				//Log is stronger at low values of "i" and Pow is stronger at high values of "i"
				double lowLvlGrowth = Math.Log(i, 1.05);
				double highLvlGrowth = 13 * Math.Pow(i, 1.37);

				xpRequirementsPerLevel[i - 1] = (int)(lowLvlGrowth + highLvlGrowth);
			}

			xpRequirementsPerLevel[MaxLevel] = -1;
		}

		public long XpTotal{ get; private set; }

		//Modifiers are constant and should not be reset
		private Dictionary<DamageClass, Ref<ModifierData>> attackModifiersByClass = new();
		private List<UnloadedDamageClass> unloadedClasses = new();

		public Modifier runAccelerationModifier = Modifier.Default;
		public Modifier maxRunSpeedModifier = Modifier.Default;
		public Modifier itemUseModifier = Modifier.Default;

		internal void DeInitialize(){
			attackModifiersByClass?.Clear();
			attackModifiersByClass = null;

			unloadedClasses?.Clear();
			unloadedClasses = null;
		}

		public ref ModifierData GetModifier(DamageClass damage){
			if(!attackModifiersByClass.TryGetValue(damage, out var data)){
				//Create a new instance and cache it if it doesn't exist already
				attackModifiersByClass.Add(damage, data = new Ref<ModifierData>(){
					Value = new(){
						modifier = Modifier.Default,
						crit = 0
					}
				});
			}

			return ref data.Value;
		}

		public void UpdateXP(Player statsOwner, int xp){
			this.xp += xp;
			XpTotal += xp;

			bool lvlup = false;
			while(this.xp >= xpRequirementsPerLevel[level] && level < MaxLevel){
				lvlup = true;

				//Level up!
				ApplyGenericLevelUpBoosts();

				this.xp -= xpRequirementsPerLevel[level];
				level++;
			}

			if(lvlup){
				CombatText.NewText(new Rectangle((int)statsOwner.Top.X - 30, (int)statsOwner.Top.Y - 35, 60, 20), CombatText.DamagedHostileCrit, "LEVEL UP!", dramatic: true);

				int[] dustTypes = new int[]{ DustID.Confetti, DustID.Confetti_Blue, DustID.Confetti_Green, DustID.Confetti_Pink, DustID.Confetti_Yellow };
				for(int i = 0; i < 80; i++){
					Vector2 randVel = new Vector2(Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-6f, -0.25f));

					Dust.NewDustPerfect(statsOwner.Center, Main.rand.Next(dustTypes), randVel, Scale: Main.rand.NextFloat(0.85f, 1.2f));
				}

				MiscUtils.SendMessage($"Player \"{statsOwner.name}\" has reached Lv. {level}!");
			}
		}

		private void ApplyGenericLevelUpBoosts(){
			healthModifier.add += 1;
			defenseModifier.add += 0.2f;
			enduranceModifier.add += 0.002f;
			runAccelerationModifier.mult += 0.02f / 60f;
			maxRunSpeedModifier.add += 0.03f;
			itemUseModifier.mult += 0.002f;
		}

		public override TagCompound SaveToTag(){
			var tag = base.SaveToTag();
			tag.Add("xptotal", XpTotal);
			tag.Add("mod.runaccel", runAccelerationModifier.ToTag());
			tag.Add("mod.maxrun", maxRunSpeedModifier.ToTag());
			tag.Add("mod.itemuse", itemUseModifier.ToTag());

			List<TagCompound> damageClassThings = new();
			//Add the modifiers
			foreach(var thing in attackModifiersByClass){
				damageClassThings.Add(new(){
					["class"] = thing.Key.SaveAsTag(),
					["mod"] = thing.Value.Value.modifier.ToTag(),
					["crit"] = thing.Value.Value.crit
				});
			}
			//... and any unloaded ones so the data is still "there"
			foreach(var unloaded in unloadedClasses){
				damageClassThings.Add(new(){
					["class"] = new TagCompound(){
						["mod"] = unloaded.mod,
						["name"] = unloaded.name
					},
					["mod"] = unloaded.modifier.ToTag(),
					["crit"] = unloaded.crit
				});
			}
			tag.Add("mod.attack", damageClassThings);

			return tag;
		}

		public override void LoadFromTag(TagCompound tag){
			base.LoadFromTag(tag);

			XpTotal = tag.GetLong("xptotal");

			runAccelerationModifier = Modifier.FromTag(tag.GetCompound("mod.runaccel"));
			maxRunSpeedModifier = Modifier.FromTag(tag.GetCompound("mod.maxrun"));
			itemUseModifier = Modifier.FromTag(tag.GetCompound("mod.itemuse"));

			attackModifiersByClass.Clear();
			if(tag.GetList<TagCompound>("mod.attack") is List<TagCompound> list){
				foreach(var entry in list){
					var classTag = entry.GetCompound("class");
					DamageClass damage = TagUtils.LoadDamageClass(classTag);
					Modifier modifier = Modifier.FromTag(entry.GetCompound("mod"));
					int crit = entry.GetInt("crit");

					if(damage is null){
						//Unable to load the class.  Cache its data, then continue
						unloadedClasses.Add(new(){
							mod = classTag.GetString("mod"),
							name = classTag.GetString("name"),
							modifier = modifier,
							crit = crit
						});
						continue;
					}

					attackModifiersByClass.Add(damage, new(){
						Value = new(){
							modifier = modifier,
							crit = crit
						}
					});
				}
			}
		}
	}
}
