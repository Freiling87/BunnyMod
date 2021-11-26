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
using UnityEngine;

namespace BunnyMod.Patches.Objects
{
	[HarmonyPatch(declaringType: typeof(Lamp))]
	public static class P_Lamp
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		
		[HarmonyTranspiler, HarmonyPatch(methodName: "Start", argumentTypes: new Type[] { })]
		private static IEnumerable<CodeInstruction> Start_Transpiler(IEnumerable<CodeInstruction> codeInstructions)
		{
			List<CodeInstruction> instructions = codeInstructions.ToList();

			MethodInfo spawnerMain_SpawnLightTemp = AccessTools.Method(typeof(SpawnerMain), nameof(SpawnerMain.SpawnLightTemp),
					new[] { typeof(Vector3), typeof(PlayfieldObject), typeof(string) });

			MethodInfo controller_spawnLightTemp = SymbolExtensions.GetMethodInfo(() => LampController.HandleSpawnLightTemp(null, Vector3.zero, null, null));

			CodeReplacementPatch patch = new CodeReplacementPatch(
					expectedMatches: 1,
					targetInstructionSequence: new[]
					{
							new CodeInstruction(OpCodes.Callvirt, spawnerMain_SpawnLightTemp)
					},
					insertInstructionSequence: new[]
					{
							new CodeInstruction(OpCodes.Call, controller_spawnLightTemp) 
					}
			);
			patch.ApplySafe(instructions, logger);
			return instructions;
		}
	}
}