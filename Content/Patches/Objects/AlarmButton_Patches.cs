using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx.Logging;
using BTHarmonyUtils.TranspilerUtils;
using BunnyMod.Content.Logging;
using BunnyMod.Content.ObjectBehaviour;
using HarmonyLib;

namespace BunnyMod.Content.Patches
{
	[HarmonyPatch(declaringType: typeof(AlarmButton))]
	public static class AlarmButton_Patches
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		
		[HarmonyTranspiler, HarmonyPatch(methodName: nameof(AlarmButton.Interact), argumentTypes: new[] { typeof(Agent) })]
		private static IEnumerable<CodeInstruction> Interact_Transpiler(IEnumerable<CodeInstruction> instructionsEnumerable, ILGenerator generator)
		{
			List<CodeInstruction> instructions = instructionsEnumerable.ToList();
			MethodInfo objectReal_Interact = AccessTools.Method(typeof(ObjectReal), nameof(ObjectReal.Interact), new [] {typeof(Agent) });
			MethodInfo controller_DoShowObjectButtons = SymbolExtensions.GetMethodInfo(() => AlarmButtonController.DoShowObjectButtons(null));
			Label continueLabel = generator.DefineLabel();
			CodeReplacementPatch patch = new CodeReplacementPatch(
					expectedMatches: 1,
					prefixInstructionSequence: new List<CodeInstruction>
					{
							new CodeInstruction(OpCodes.Ldarg_0),
							new CodeInstruction(OpCodes.Ldarg_1),
							new CodeInstruction(OpCodes.Call, objectReal_Interact)
					},
					insertInstructionSequence: new List<CodeInstruction>
					{
							new CodeInstruction(OpCodes.Ldarg_0),
							new CodeInstruction(OpCodes.Call, controller_DoShowObjectButtons),
							new CodeInstruction(OpCodes.Brfalse_S, continueLabel),
							new CodeInstruction(OpCodes.Ret),
							new CodeInstruction(OpCodes.Nop) { labels = new List<Label> { continueLabel } }
					}
			);
			patch.ApplySafe(instructions, logger);
			return instructions;
		}

		// TODO I'm not happy with this being here.
		[HarmonyPrefix, HarmonyPatch(methodName: nameof(AlarmButton.ToggleSwitch), argumentTypes: new[] { typeof(Agent), typeof(Agent) })]
		private static bool ToggleSwitch_Prefix(AlarmButton __instance, Agent causerAgent, Agent criminal)
		{
			GameController gc = GameController.gameController;
			if (gc.challenges.Contains(cChallenge.AnCapistan) && gc.serverPlayer && causerAgent.isPlayer != 0 && !causerAgent.hasProtector)
			{
				gc.audioHandler.Play(__instance, "AlarmButton");
				__instance.SpawnEnforcer(causerAgent, criminal);
				__instance.DoLockdown(true);
				return false;
			}
			return true;
		}
	}
}