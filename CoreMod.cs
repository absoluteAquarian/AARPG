using AARPG.API.Edits;
using AARPG.Core.Mechanics;
using AARPG.Core.Systems;
using AARPG.Core.Utility.Extensions;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AARPG{
	public class CoreMod : Mod{
		public static CoreMod Instance => ModContent.GetInstance<CoreMod>();

		public const bool Release = false;

		public override void Load(){
			PlayerStatistics.InitializeXPRequirements();

			EditsLoader.Load();

			NPCStatisticsRegistry.Load();

			InterfaceSystem.LoadStatic();
		}

		public override void PostSetupContent(){
			NPCStatisticsRegistry.PostSetupContent();
		}

		public override void Unload(){
			InterfaceSystem.UnloadStatic();

			NPCStatisticsRegistry.Unload();
		}

		public override object Call(params object[] args){
			if(args is null)
				throw new ArgumentNullException(nameof(args));

			if(args.Length < 2)
				throw new ArgumentException("Too few arguments were provided");

			if(args[0] is not string function)
				throw new ArgumentException("Expected a function name for the first argument");

			void CheckArgsLength(int expected, params string[] argNames){
				if(args.Length != expected)
					throw new ArgumentOutOfRangeException($"Expected {expected} arguments for Mod.Call(\"{function}\", {string.Join(",", argNames)}), got {args.Length} arguments instead");
			}

			void CheckArg<TExpected>(int argSlot, out TExpected validInstance, string additionalArgs = null){
				if(additionalArgs != null)
					additionalArgs = ", " + additionalArgs;

				if(args[argSlot] is not TExpected expected)
					throw new ArgumentException($"Argument {argSlot} for Mod.Call(\"{function}\"{additionalArgs ?? ""}) must be of type {typeof(TExpected).FullNameUpgraded()}");

				validInstance = expected;
			}

			switch(function){
				case "Register NPC Stats Condition":
					CheckArgsLength(3, "string accessKey, Func<bool> condition");
					CheckArg(1, out string key);
					CheckArg(2, out Func<bool> condition);

					if(NPCStatisticsRegistry.conditions.ContainsKey(key))
						throw new ArgumentException($"NPC Statistics Registry already has an entry for \"{key}\"");

					NPCStatisticsRegistry.conditions.Add(key, condition);
					return true;
				default:
					throw new ArgumentException($"Unknown function: \"{function}\"");
			}
		}
	}
}