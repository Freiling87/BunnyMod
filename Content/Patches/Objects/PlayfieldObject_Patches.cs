using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx.Logging;
using BTHarmonyUtils.TranspilerUtils;
using BunnyMod.Logging;
using BunnyMod.ObjectBehaviour;
using BunnyMod.Traits;
using HarmonyLib;

namespace BunnyMod.Patches.Objects
{
	[HarmonyPatch(declaringType: typeof(PlayfieldObject))]
	public static class PlayfieldObject_Patches
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		
		[HarmonyPrefix, HarmonyPatch(methodName: nameof(PlayfieldObject.playerHasUsableItem), argumentTypes: new[] { typeof(InvItem) })]
		private static bool PlayerHasUsableItem_Prefix(PlayfieldObject __instance, InvItem myItem, ref bool __result)
		{
			ObjectControllerManager.GetObjectController(__instance)?.HandlePlayerHasUsableItem(__instance, myItem, ref __result);
			return !__result; // stop further execution if the controller determined that the item is usable
		}

		[HarmonyPostfix, HarmonyPatch(methodName: nameof(PlayfieldObject.FinishedOperating), argumentTypes: new Type[] { })]
		private static void FinishedOperating_Postfix(PlayfieldObject __instance)
		{
			ObjectControllerManager.GetObjectController(__instance)?.HandleFinishedOperating(__instance);
		}

		[HarmonyPrefix, HarmonyPatch(methodName: nameof(PlayfieldObject.ShowObjectButtons), argumentTypes: new Type[] { })]
		private static void ShowObjectButtons_Prefix(PlayfieldObject __instance)
		{
			BMTraitController.CorrectButtonCosts(__instance);
		}

		[HarmonyTranspiler, HarmonyPatch(methodName: nameof(PlayfieldObject.DetermineLuck), argumentTypes: new[] { typeof(int), typeof(string), typeof(bool) })]
		private static IEnumerable<CodeInstruction> DetermineLuck_Transpiler(IEnumerable<CodeInstruction> codeInstructions)
		{
			List<CodeInstruction> instructions = codeInstructions.ToList();

			MethodInfo method_controller_GetLuckBonus = SymbolExtensions.GetMethodInfo(() => BMTraitController.GetLuckBonus(0, null, null));

			CodeReplacementPatch luckBonusPatch = new CodeReplacementPatch(
					expectedMatches: 19,
					prefixInstructionSequence: new List<CodeInstruction>
					{
							new CodeInstruction(OpCodes.Ldc_R4),
							new CodeInstruction(OpCodes.Stloc_S, 1),
							new CodeInstruction(OpCodes.Ldc_R4),
							new CodeInstruction(OpCodes.Stloc_S, 2),
							new CodeInstruction(OpCodes.Ldc_R4),
							new CodeInstruction(OpCodes.Stloc_S, 3),
							new CodeInstruction(OpCodes.Ldc_R4),
							new CodeInstruction(OpCodes.Stloc_S, 4),
							new CodeInstruction(OpCodes.Ldc_R4),
							new CodeInstruction(OpCodes.Stloc_S, 5),
					},
					insertInstructionSequence: new List<CodeInstruction>
					{
							new CodeInstruction(OpCodes.Ldloc_S, 5), // num6
							new CodeInstruction(OpCodes.Ldarg_2), // num6, luckType
							new CodeInstruction(OpCodes.Ldarg_0), // num6, luckType, playFieldObject
							new CodeInstruction(OpCodes.Call, method_controller_GetLuckBonus), // luckBonus
							new CodeInstruction(OpCodes.Ldloc_0), // luckBonus, num
							new CodeInstruction(OpCodes.Add), // summed
							new CodeInstruction(OpCodes.Stloc_0)
					}
			);
			luckBonusPatch.ApplySafe(instructions, logger);
			return instructions;
		}
	}
}