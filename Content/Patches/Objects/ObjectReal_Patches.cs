using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx.Logging;
using BTHarmonyUtils.TranspilerUtils;
using BunnyMod.Content.Logging;
using BunnyMod.Content.ObjectBehaviour;
using BunnyMod.Content.Traits;
using HarmonyLib;
using RogueLibsCore;

namespace BunnyMod.Content.Patches
{
	[HarmonyPatch(declaringType: typeof(ObjectReal))]
	public static class ObjectReal_Patches
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();

		[HarmonyPostfix, HarmonyPatch(methodName: nameof(ObjectReal.ObjectUpdate), argumentTypes: new Type[] { })]
		private static void ObjectUpdate_Postfix(ObjectReal __instance)
		{
			ObjectControllerManager.GetController(__instance)?.HandleObjectUpdate(__instance);
		}

		[HarmonyPostfix, HarmonyPatch(methodName: nameof(ObjectReal.PressedButton), argumentTypes: new[] { typeof(string), typeof(int) })]
		private static void PressedButton_Postfix(ObjectReal __instance, string buttonText, int buttonPrice)
		{
			logger.LogDebug($"PressedButton on object: '{__instance.name}'");

			{
				// TODO DispenseIce is currently never used
				// if (buttonText == "DispenseIce")
				//   __instance.StartCoroutine(__instance.Operating(agent, agent.inventory.FindItem(vItem.Wrench), 2f, true, "Tampering"));
				//   ObjectUtils.SpawnSuspiciousNoise(0, 1f, agent, __instance);
			}

			ObjectControllerManager.GetController(__instance)?.HandlePressedButton(__instance, buttonText, buttonPrice);
		}

		[HarmonyPostfix, HarmonyPatch(methodName: nameof(ObjectReal.DetermineButtons), argumentTypes: new Type[] { })]
		private static void DetermineButtons_Postfix(ObjectReal __instance)
		{
			ObjectControllerManager.GetController(__instance)?.HandleDetermineButtons(__instance);
		}

		[HarmonyTranspiler, HarmonyPatch(methodName: nameof(ObjectReal.FinishedOperating), argumentTypes: new Type[] { })]
		private static IEnumerable<CodeInstruction> FinishedOperating_Transpiler(IEnumerable<CodeInstruction> instructionsEnumerable, ILGenerator generator)
		{
			List<CodeInstruction> instructions = instructionsEnumerable.ToList();

			MethodInfo playfieldObject_FinishedOperating =
					AccessTools.Method(typeof(PlayfieldObject), nameof(PlayfieldObject.FinishedOperating), new Type[] { });

			MethodInfo didStopInteraction_method = SymbolExtensions.GetMethodInfo(() => ObjectUtils.DidStopInteraction(null));
			Label continueLabel = generator.DefineLabel();
			CodeReplacementPatch patch = new CodeReplacementPatch(
					expectedMatches: 1,
					prefixInstructionSequence: new List<CodeInstruction>
					{
							new CodeInstruction(OpCodes.Ldarg_0),
							new CodeInstruction(OpCodes.Call, playfieldObject_FinishedOperating)
					},
					insertInstructionSequence: new List<CodeInstruction>
					{
							new CodeInstruction(OpCodes.Ldarg_0),
							new CodeInstruction(OpCodes.Call, didStopInteraction_method),
							new CodeInstruction(OpCodes.Brfalse_S, continueLabel),
							new CodeInstruction(OpCodes.Ret),
							new CodeInstruction(OpCodes.Nop) { labels = new List<Label> { continueLabel } }
					}
			);
			patch.ApplySafe(instructions, logger);
			return instructions;
		}

		[HarmonyPostfix, HarmonyPatch(methodName: nameof(ObjectReal.Interact), argumentTypes: new[] { typeof(Agent) })]
		private static void Interact_Postfix(ObjectReal __instance, Agent agent)
		{
			logger.LogDebug($"Interacting with objectReal: '{__instance.name}'");

			if (agent.HasTrait<StealthBastardDeluxe>() && StealthBastardDeluxe.CanHideInObject(__instance))
			{
				ObjectUtils.HideInObject(agent, __instance);
				return;
			}

			ObjectControllerManager.GetController(__instance)?.HandleInteract(__instance, agent);
		}

		[HarmonyPostfix,
		 HarmonyPatch(methodName: nameof(ObjectReal.ObjectAction),
				 argumentTypes: new[] { typeof(string), typeof(string), typeof(float), typeof(Agent), typeof(PlayfieldObject) })]
		private static void ObjectAction_Postfix(ObjectReal __instance,
				string myAction, string extraString, float extraFloat, Agent causerAgent, PlayfieldObject extraObject,
				ref bool ___noMoreObjectActions)
		{
			logger.LogDebug($"ObjectAction on object: '{__instance.name}', action: '{myAction}'");
			ObjectControllerManager.GetController(__instance)?.HandleObjectAction(__instance, myAction, ref ___noMoreObjectActions);
		}

		[HarmonyPostfix, HarmonyPatch(methodName: nameof(ObjectReal.DamagedObject), argumentTypes: new[] { typeof(PlayfieldObject), typeof(float) })]
		private static void DamagedObject_Postfix(ObjectReal __instance, PlayfieldObject damagerObject, float damageAmount)
		{
			logger.LogDebug($"Damaging object: '{__instance.name}'");
			ObjectControllerManager.GetController(__instance)?.HandleDamagedObject(__instance, damagerObject, damageAmount);
		}

		[HarmonyPostfix, HarmonyPatch(methodName: nameof(ObjectReal.MakeNonFunctional), argumentTypes: new[] { typeof(PlayfieldObject) })]
		private static void MakeNonFunctional_Postfix(ObjectReal __instance, PlayfieldObject damagerObject)
		{
			logger.LogDebug($"Making object nonFunctional. object: '{__instance.name}'");
			ObjectControllerManager.GetController(__instance)?.HandleMakeNonFunctional(__instance, damagerObject);
		}

		[HarmonyPrefix, HarmonyPatch(methodName: nameof(ObjectReal.DestroyMe), argumentTypes: new[] { typeof(PlayfieldObject) })]
		private static void DestroyMe_Prefix(PlayfieldObject damagerObject, ObjectReal __instance)
		{
			logger.LogDebug($"Destroying object: '{__instance.name}'");
			ObjectControllerManager.GetController(__instance)?.HandleDestroyMe(__instance, damagerObject);
		}

		[HarmonyPrefix, HarmonyPatch(methodName: nameof(ObjectReal.DestroyMe3), argumentTypes: new Type[] { })]
		private static void DestroyMe3_Prefix(ObjectReal __instance)
		{
			ObjectControllerManager.GetController(__instance)?.HandleDestroyMe3(__instance);
		}
	}
}