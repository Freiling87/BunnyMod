using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx.Logging;
using BTHarmonyUtils.MidFixPatch;
using BTHarmonyUtils.TranspilerUtils;
using BunnyMod.Logging;
using BunnyMod.ObjectBehaviour.Controllers;
using HarmonyLib;

namespace BunnyMod.Patches.Objects
{
	[HarmonyPatch(declaringType: typeof(Turret))]
	public static class Turret_Patches
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();

		[HarmonyPostfix, HarmonyPatch(methodName: "Start", argumentTypes: new Type[] { })]
		private static void Start_Postfix(Turret __instance)
		{
			TurretController.Start_Postfix(__instance);
		}
		
		[HarmonyTranspiler, HarmonyPatch(methodName: nameof(Turret.isOpponent), argumentTypes: new[] { typeof(Agent), typeof(bool) })]
		private static IEnumerable<CodeInstruction> IsOpponent_Transpiler(IEnumerable<CodeInstruction> codeInstructions, ILGenerator generator)
		{
			List<CodeInstruction> instructions = codeInstructions.ToList();

			MethodInfo method_controller_isOpponent = SymbolExtensions.GetMethodInfo(() => TurretController.IsOpponent(null, null));

			Label label_isNotOpponent = generator.DefineLabel();

			/* Goal is to hook our custom "IsOpponent" detection in next to the other target modes
			 * so we don't have to duplicate the previous conditions
			 */
			CodeReplacementPatch customTargetingPatch = new CodeReplacementPatch(
					expectedMatches: 1,
					insertInstructionSequence: new List<CodeInstruction>
					{
							new CodeInstruction(OpCodes.Ldarg_1), // [agent]
							new CodeInstruction(OpCodes.Ldarg_0), // [agent, turret]
							new CodeInstruction(OpCodes.Call, method_controller_isOpponent), // [bool]
							new CodeInstruction(OpCodes.Brfalse_S, label_isNotOpponent), // []
							new CodeInstruction(OpCodes.Ldc_I4_1), // [true]
							new CodeInstruction(OpCodes.Ret), // []
							new CodeInstruction(OpCodes.Nop) { labels = new List<Label> { label_isNotOpponent } }
					},
					postfixInstructionSequence: new List<CodeInstruction>
					{
							new CodeInstruction(OpCodes.Ldarg_0),
							new CodeInstruction(OpCodes.Call),
							new CodeInstruction(OpCodes.Ldstr, "NonOwners")
					}
			);
			customTargetingPatch.ApplySafe(instructions, logger);
			return instructions;
		}

		private static MidFixInstructionMatcher PressedButton_InstructionMatcher()
		{
			// Apply MidFix after base method was called.
			return new MidFixInstructionMatcher(
					expectedMatches: 1,
					prefixInstructionSequence: new[]
					{
							new CodeInstruction(OpCodes.Ldarg_0),
							new CodeInstruction(OpCodes.Ldarg_1),
							new CodeInstruction(OpCodes.Ldarg_2),
							new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(typeof(ObjectReal), nameof(ObjectReal.PressedButton), new[] { typeof(string), typeof(int) }))
					});
		}

		[BTHarmonyMidFix(nameof(PressedButton_InstructionMatcher)), HarmonyPatch(methodName: nameof(Turret.PressedButton), argumentTypes: new[] { typeof(string), typeof(int) })]
		private static bool PressedButton_MidFix(Turret __instance, string buttonText, int buttonPrice)
		{
			return !TurretController.HandlePressedButton_KeepMenu(__instance, buttonText); // skip rest of method if button press was handled
		}
	}
}