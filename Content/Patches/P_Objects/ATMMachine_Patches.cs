using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx.Logging;
using BTHarmonyUtils.TranspilerUtils;
using BunnyMod.Logging;
using BunnyMod.ObjectBehaviour.Controllers;
using HarmonyLib;

namespace BunnyMod.Patches.Objects
{
	[HarmonyPatch(declaringType: typeof(ATMMachine))]
	public static class ATMMachine_Patches
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		
		[HarmonyTranspiler, HarmonyPatch(methodName: nameof(ATMMachine.DetermineButtons), argumentTypes: new Type[] { })]
		private static IEnumerable<CodeInstruction> DetermineButtons_Transpiler(IEnumerable<CodeInstruction> instructionsEnumerable)
		{
			List<CodeInstruction> instructions = instructionsEnumerable.ToList();
			MethodInfo addButtonsIfPossible_method = SymbolExtensions.GetMethodInfo(() => ATMMachineController.AddButtonsIfPossible(null));

			CodeReplacementPatch patch = new CodeReplacementPatch(
					expectedMatches: 1,
					insertInstructionSequence: new List<CodeInstruction>
					{
							new CodeInstruction(OpCodes.Ldarg_0),
							new CodeInstruction(OpCodes.Call, addButtonsIfPossible_method),
							new CodeInstruction(OpCodes.Ldloc_0),
							new CodeInstruction(OpCodes.Or),
							new CodeInstruction(OpCodes.Stloc_0) // local_0 = local_0 || AddButtonsIfPossible(this)
					},
					postfixInstructionSequence: new List<CodeInstruction>
					{
							new CodeInstruction(OpCodes.Ldloc_0),
							new CodeInstruction(OpCodes.Brtrue_S)
					}
			);
			patch.ApplySafe(instructions, logger);
			return instructions;
		}

		[HarmonyPostfix, HarmonyPatch(methodName: nameof(ATMMachine.PayCops), argumentTypes: new Type[] { })]
		private static void PayCops_Postfix(ATMMachine __instance)
		{
			ATMMachineController.HandlePayCops(__instance);
		}
	}
}