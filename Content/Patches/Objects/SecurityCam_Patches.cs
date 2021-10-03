using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx.Logging;
using BTHarmonyUtils;
using BTHarmonyUtils.TranspilerUtils;
using BunnyMod.Content.Logging;
using BunnyMod.Content.ObjectBehaviour;
using HarmonyLib;
using JetBrains.Annotations;

namespace BunnyMod.Content.Patches
{
	[HarmonyPatch(declaringType: typeof(SecurityCam))]
	public static class SecurityCam_Patches
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();

		[HarmonyTranspiler, HarmonyPatch(methodName: nameof(SecurityCam.FinishedOperating), argumentTypes: new Type[] { })]
		private static IEnumerable<CodeInstruction> FinishedOperating_Transpiler(IEnumerable<CodeInstruction> codeInstructions)
		{
			List<CodeInstruction> instructions = codeInstructions.ToList();
			MethodInfo gc_percentChance = AccessTools.Method(typeof(GameController), nameof(GameController.percentChance), new[] { typeof(int) });
			MethodInfo controller_HandleSuccess = SymbolExtensions.GetMethodInfo(() => SecurityCamController.HandleSuccessfulManualShutdown(null));
			CodeReplacementPatch patch = new CodeReplacementPatch(
					expectedMatches: 1,
					prefixInstructionSequence: new[]
					{
							new CodeInstruction(OpCodes.Callvirt, gc_percentChance),
							new CodeInstruction(OpCodes.Brfalse_S)
					},
					insertInstructionSequence: new[]
					{
							new CodeInstruction(OpCodes.Ldarg_0),
							new CodeInstruction(OpCodes.Call, controller_HandleSuccess)
					}
			);
			patch.ApplySafe(instructions, logger);
			return instructions;
		}

		// this particular piece of petrifying posture is unfortunately required for target methods calculated at runtime
		[HarmonyPatch] // HarmonyPatch is required for 'PatchAll' to find this, but no further arguments are allowed by harmony if 'HarmonyTargetMethod' is present
		private static class SecurityCam_MyUpdate_MoveNext_Patches
		{
			[HarmonyTargetMethod, UsedImplicitly]
			private static MethodInfo Find_MyUpdateMoveNext_MethodInfo()
			{
				return PatcherUtils.FindIEnumeratorMoveNext(AccessTools.Method(typeof(SecurityCam), "MyUpdate", new Type[] { }));
			}

			[HarmonyTranspiler, UsedImplicitly]
			private static IEnumerable<CodeInstruction> MyUpdateMoveNext_Transpiler(IEnumerable<CodeInstruction> codeInstructions)
			{
				List<CodeInstruction> instructions = codeInstructions.ToList();

				/*
				 * this patch expands the detection system
				 * it calls SecurityCamController::IsCameraDetectingAgent after the vanilla detection is finished
				 *   / before the detection flag is used for the first time
				 */
				MethodInfo controller_IsCameraDetectingAgent_method = SymbolExtensions.GetMethodInfo(() => SecurityCamController.IsCameraDetectingAgent(null, null, false));
				FieldInfo agent_curTileData_field = AccessTools.Field(typeof(Agent), nameof(Agent.curTileData));
				FieldInfo tileData_chunkID_field = AccessTools.Field(typeof(TileData), nameof(TileData.chunkID));
				CodeReplacementPatch cameraDetectionPatch = new CodeReplacementPatch(
						expectedMatches: 1,
						insertInstructionSequence: new[]
						{
								new CodeInstruction(OpCodes.Ldarg_0), // this (SecurityCam)
								new CodeInstruction(OpCodes.Ldloc_S, 4), // local#4 is agent to check (Agent)
								new CodeInstruction(OpCodes.Ldloc_3), // local#3 is flag for agent detection (bool)
								new CodeInstruction(OpCodes.Call, controller_IsCameraDetectingAgent_method),
								new CodeInstruction(OpCodes.Stloc_3), // set the detection flag
						},
						postfixInstructionSequence: new[]
						{
								new CodeInstruction(OpCodes.Ldloc_3),
								new CodeInstruction(OpCodes.Brfalse),
								new CodeInstruction(OpCodes.Ldloc_S, 4),
								new CodeInstruction(OpCodes.Ldfld, agent_curTileData_field),
								new CodeInstruction(OpCodes.Ldfld, tileData_chunkID_field)
						}
				);
				cameraDetectionPatch.ApplySafe(instructions, logger);
				return instructions;
			}
		}
	}
}