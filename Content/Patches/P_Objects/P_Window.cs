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
	[HarmonyPatch(declaringType: typeof(Window))]
	public static class P_Window
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();

		[HarmonyTranspiler, HarmonyPatch(methodName: nameof(Window.SlipThroughWindow), argumentTypes: new[] { typeof(Agent) })]
		private static IEnumerable<CodeInstruction> SlipThroughWindow_DamageSound_Transpiler(IEnumerable<CodeInstruction> codeInstructions, ILGenerator generator)
		{
			List<CodeInstruction> instructions = codeInstructions.ToList();

			Label label_shouldNotPlayDamageSound = generator.DefineLabel();

			MethodInfo method_shouldPlayDamageSound = SymbolExtensions.GetMethodInfo(() => WindowController.SlipThroughWindow_ShouldDamageAgent(null));

			List<CodeInstruction> playSound_matcherInstructions = new List<CodeInstruction>
			{
					new CodeInstruction(OpCodes.Ldarg_0),
					new CodeInstruction(OpCodes.Ldfld),
					new CodeInstruction(OpCodes.Ldfld),
					new CodeInstruction(OpCodes.Ldarg_1),
					new CodeInstruction(OpCodes.Ldstr, "MeleeHitAgentCutSmallClients"),
					new CodeInstruction(OpCodes.Callvirt)
			};

			CodeReplacementPatch beforeBranchPatch = new CodeReplacementPatch(
					expectedMatches: 1,
					postfixInstructionSequence: playSound_matcherInstructions,
					insertInstructionSequence: new List<CodeInstruction>
					{
							new CodeInstruction(OpCodes.Ldarg_1),
							new CodeInstruction(OpCodes.Call, method_shouldPlayDamageSound),
							new CodeInstruction(OpCodes.Brfalse_S, label_shouldNotPlayDamageSound)
					}
			);
			CodeReplacementPatch afterBranchPatch = new CodeReplacementPatch(
					expectedMatches: 1,
					prefixInstructionSequence: playSound_matcherInstructions,
					insertInstructionSequence: new List<CodeInstruction>
					{
							new CodeInstruction(OpCodes.Nop) { labels = new List<Label> { label_shouldNotPlayDamageSound } }
					}
			);

			beforeBranchPatch.ApplySafe(instructions, logger);
			afterBranchPatch.ApplySafe(instructions, logger);
			return instructions;
		}

		private static MidFixInstructionMatcher SlipThroughWindow_InstructionMatcher()
		{
			// Apply MidFix before agent receives damage
			return new MidFixInstructionMatcher(
					expectedMatches: 1,
					postfixInstructionSequence: new[]
					{
							new CodeInstruction(OpCodes.Ldarg_1),
							new CodeInstruction(OpCodes.Ldstr, "BrokenGlass")
					});
		}

		[BTHarmonyMidFix(nameof(SlipThroughWindow_InstructionMatcher)), HarmonyPatch(methodName: nameof(Window.SlipThroughWindow), argumentTypes: new[] { typeof(Agent) })]
		private static bool SlipThroughWindow_MidFix(Agent myAgent)
		{
			return WindowController.SlipThroughWindow_ShouldDamageAgent(myAgent);
		}

		[HarmonyPostfix, HarmonyPatch(methodName: nameof(Window.SetVars), argumentTypes: new Type[] { })]
		private static void SetVars_Postfix(Window __instance)
		{
			WindowController.SetVars_Postfix(__instance);
		}
	}
}