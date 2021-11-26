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
	[HarmonyPatch(declaringType: typeof(TrashCan))]
	public static class P_TrashCan
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		
		[HarmonyTranspiler, HarmonyPatch(methodName: nameof(TrashCan.Interact), argumentTypes: new[] { typeof(Agent) })]
		private static IEnumerable<CodeInstruction> Interact_Transpiler(IEnumerable<CodeInstruction> codeInstructions)
		{
			List<CodeInstruction> instructions = codeInstructions.ToList();

			MethodInfo method_playfieldObject_ShowChest = AccessTools.DeclaredMethod(typeof(PlayfieldObject), nameof(PlayfieldObject.ShowChest), new Type[] { });

			MethodInfo method_controller_HandleInteract = SymbolExtensions.GetMethodInfo(() => TrashCanController.HandleInteract(null, null));

			CodeReplacementPatch patch = new CodeReplacementPatch(
					expectedMatches: 1,
					targetInstructionSequence: new[]
					{
							new CodeInstruction(OpCodes.Ldarg_0), 
							new CodeInstruction(OpCodes.Call, method_playfieldObject_ShowChest)
					},
					insertInstructionSequence: new[]
					{
							new CodeInstruction(OpCodes.Ldarg_1),
							new CodeInstruction(OpCodes.Ldarg_0), 
							new CodeInstruction(OpCodes.Call, method_controller_HandleInteract) 
					}
			);
			patch.ApplySafe(instructions, logger);
			return instructions;
		}
	}
}