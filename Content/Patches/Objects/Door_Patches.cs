using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx.Logging;
using BTHarmonyUtils.TranspilerUtils;
using BunnyMod.Content.Logging;
using BunnyMod.Content.ObjectBehaviour;
using HarmonyLib;
using UnityEngine;

namespace BunnyMod.Content.Patches
{
	[HarmonyPatch(declaringType: typeof(Door))]
	public static class Door_Patches
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();

		private static CodeReplacementPatch GetSpawnNoisePatch()
		{
			MethodInfo spawnerMain_SpawnNoise = AccessTools.Method(typeof(SpawnerMain), nameof(SpawnerMain.SpawnNoise),
					new[] { typeof(Vector3), typeof(float), typeof(PlayfieldObject), typeof(string), typeof(Agent) });

			MethodInfo controller_HandleSpawnNoise =
					SymbolExtensions.GetMethodInfo(() => DoorController.HandleSpawnNoise(null, Vector3.zero, 0f, null, null, null));

			return new CodeReplacementPatch(
					expectedMatches: 1,
					targetInstructionSequence: new[]
					{
							new CodeInstruction(OpCodes.Callvirt, spawnerMain_SpawnNoise)
					},
					insertInstructionSequence: new[]
					{
							new CodeInstruction(OpCodes.Call, controller_HandleSpawnNoise)
					}
			);
		}

		[HarmonyTranspiler, HarmonyPatch(methodName: nameof(Door.OpenDoor), argumentTypes: new[] { typeof(Agent), typeof(bool) })]
		private static IEnumerable<CodeInstruction> OpenDoor_Transpiler(IEnumerable<CodeInstruction> codeInstructions)
		{
			List<CodeInstruction> instructions = codeInstructions.ToList();

			FieldInfo playfieldObject_gc = AccessTools.Field(typeof(PlayfieldObject), nameof(PlayfieldObject.gc));
			FieldInfo playfieldObject_go = AccessTools.Field(typeof(PlayfieldObject), nameof(PlayfieldObject.go));

			MethodInfo gameController_OwnCheck = AccessTools.Method(typeof(GameController), nameof(GameController.OwnCheck),
					new[] { typeof(Agent), typeof(GameObject), typeof(string), typeof(int) });

			MethodInfo shouldSkipOwnCheck_method = SymbolExtensions.GetMethodInfo(() => DoorController.ShouldSkipOwnCheck(null));

			CodeReplacementPatch spawnNoisePatch = GetSpawnNoisePatch();

			// in addition to patching the SpawnNoise call, we want to conditionally skip the ownCheck.
			CodeReplacementPatch ownCheckPatch = new CodeReplacementPatch(
					expectedMatches: 1,
					insertInstructionSequence: new[]
					{
							new CodeInstruction(OpCodes.Ldarg_0),
							new CodeInstruction(OpCodes.Call, shouldSkipOwnCheck_method),
							new CodeInstruction(OpCodes.Or)
					},
					postfixInstructionSequence: new[]
					{
							new CodeInstruction(OpCodes.Brtrue),

							new CodeInstruction(OpCodes.Ldarg_0),
							new CodeInstruction(OpCodes.Ldfld, playfieldObject_gc),
							new CodeInstruction(OpCodes.Ldarg_1),
							new CodeInstruction(OpCodes.Ldarg_0),
							new CodeInstruction(OpCodes.Ldfld, playfieldObject_go),
							new CodeInstruction(OpCodes.Ldstr), // "Door"
							new CodeInstruction(OpCodes.Ldc_I4_0),
							new CodeInstruction(OpCodes.Callvirt, gameController_OwnCheck) // this.gc.OwnCheck(arg1, this.go, "Door", 0)
					}
			);

			spawnNoisePatch.ApplySafe(instructions, logger);
			ownCheckPatch.ApplySafe(instructions, logger);
			return instructions;
		}

		[HarmonyTranspiler, HarmonyPatch(methodName: nameof(Door.CloseDoor), argumentTypes: new[] { typeof(Agent), typeof(bool) })]
		private static IEnumerable<CodeInstruction> CloseDoor_Transpiler(IEnumerable<CodeInstruction> codeInstructions)
		{
			List<CodeInstruction> instructions = codeInstructions.ToList();
			CodeReplacementPatch patch = GetSpawnNoisePatch();
			patch.ApplySafe(instructions, logger);
			return instructions;
		}

		[HarmonyPostfix, HarmonyPatch(methodName: "FreePrisonerPointsIfNotDead", argumentTypes: new[] { typeof(Agent), typeof(List<Agent>) })]
		private static void FreePrisonerPointsIfNotDead_Postfix(Agent myAgent, List<Agent> myFreedAgents)
		{
			DoorController.HandleFreePrisonerPointsIfNotDead(myAgent, myFreedAgents);
		}

		[HarmonyPrefix, HarmonyPatch(methodName: nameof(Door.delayedOwnCheck), argumentTypes: new[] { typeof(Agent) })]
		private static bool delayedOwnCheck_Prefix(Agent myAgent, ref IEnumerator __result)
		{
			if (DoorController.ShouldSkipOwnCheck(myAgent))
			{
				__result = EmptyCoroutine();
				return false;
			}
			return true;
		}

		private static IEnumerator EmptyCoroutine()
		{
			yield break;
		}
	}
}