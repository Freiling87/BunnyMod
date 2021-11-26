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
	[HarmonyPatch(declaringType: typeof(Manhole))]
	public static class Manhole_Patches
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		
		[HarmonyPostfix, HarmonyPatch(methodName: nameof(Manhole.SetVars), argumentTypes: new Type[] { })]
		private static void SetVars_Postfix(Manhole __instance)
		{
			ManholeController.SetVars(__instance);
		}

		[HarmonyTranspiler, HarmonyPatch(methodName: "Start", argumentTypes: new Type[] { })]
		private static IEnumerable<CodeInstruction> Start_Transpiler(IEnumerable<CodeInstruction> codeInstructions)
		{
			List<CodeInstruction> instructions = codeInstructions.ToList();
			MethodInfo playfieldObject_set_objectName = AccessTools.PropertySetter(typeof(PlayfieldObject), nameof(PlayfieldObject.objectName));
			MethodInfo controller_ShouldSpawnManhole = SymbolExtensions.GetMethodInfo(() => ManholeController.ShouldSpawnManhole());
			
			CodeReplacementPatch patch = new CodeReplacementPatch(
					expectedMatches: 1,
					insertInstructionSequence: new[]
					{
							new CodeInstruction(OpCodes.Call, controller_ShouldSpawnManhole),
							new CodeInstruction(OpCodes.Not),
							new CodeInstruction(OpCodes.And) 
					},
					postfixInstructionSequence: new[]
					{
							new CodeInstruction(OpCodes.Brfalse_S),
							new CodeInstruction(OpCodes.Ldarg_0),
							new CodeInstruction(OpCodes.Ldstr), // "Manhole"
							new CodeInstruction(OpCodes.Call, playfieldObject_set_objectName), 
					}
			);
			patch.ApplySafe(instructions, logger);
			return instructions;
		}
	}
}