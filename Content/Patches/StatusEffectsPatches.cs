using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx.Logging;
using BTHarmonyUtils.TranspilerUtils;
using BunnyMod.Content.Logging;
using BunnyMod.Content.Abilities.A_Magic;
using BunnyMod.Content.Traits;
using Google2u;
using HarmonyLib;
using RogueLibsCore;
using UnityEngine;
using UnityEngine.Networking;

namespace BunnyMod.Content.Patches
{
	[HarmonyPatch(declaringType: typeof(StatusEffects))]
	public static class StatusEffectsPatches
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();

		[HarmonyPostfix, HarmonyPatch(methodName: nameof(StatusEffects.AgentIsRival), argumentTypes: new[] { typeof(Agent) })]
		private static void AgentIsRival_Postfix(Agent myAgent, StatusEffects __instance, ref bool __result)
		{
			// All custom Social traits with Rivalry & XP bonus

			Agent otherAgent = __instance.agent;

			if (MobDebt.AgentIsRival(myAgent, otherAgent) || BootLicker.AgentIsRival(myAgent, otherAgent) || Priors.AgentIsRival(myAgent, otherAgent))
			{
				__result = true;
			}
		}

		#region BecomeHidden, BecomeNotHidden
		private static float BecomeHidden_GetPositionOffset(PlayfieldObject hiddenInObject)
		{
			switch (hiddenInObject.objectName)
			{
				case nameof(ObjectNameDB.rowIds.Bathtub):
					// TODO detect walls here and do case-by-case
					return 0.24f;
				case nameof(ObjectNameDB.rowIds.Plant):
				case nameof(ObjectNameDB.rowIds.TrashCan):
					return -0.24f;
				case nameof(ObjectNameDB.rowIds.Bush):
				case nameof(ObjectNameDB.rowIds.PoolTable):
				case nameof(ObjectNameDB.rowIds.TableBig):
				default:
					return 0.24f;
			}
		}

		private static bool BecomeHidden_ShouldDisableHitbox(PlayfieldObject hiddenInObject)
		{
			switch (hiddenInObject.objectName)
			{
				case nameof(ObjectNameDB.rowIds.Bathtub):
				case nameof(ObjectNameDB.rowIds.Plant):
				case nameof(ObjectNameDB.rowIds.TrashCan):
				case nameof(ObjectNameDB.rowIds.PoolTable):
				case nameof(ObjectNameDB.rowIds.TableBig):
					return true;
				default:
					return false;
			}
		}

		private static string BecomeHidden_GetClipName(PlayfieldObject hiddenInObject)
		{
			switch (hiddenInObject.objectName)
			{
				case nameof(ObjectNameDB.rowIds.Bathtub):
					return vAudioClip.AgentJoin;
				case nameof(ObjectNameDB.rowIds.PoolTable):
					return vAudioClip.AgentKnockOut;
				case nameof(ObjectNameDB.rowIds.TableBig):
					return vAudioClip.AgentKnockout2;
				case nameof(ObjectNameDB.rowIds.TrashCan):
					return vAudioClip.Hoist;
				case nameof(ObjectNameDB.rowIds.Bush):
				case nameof(ObjectNameDB.rowIds.Plant):
				default:
					return vAudioClip.Hide;
			}
		}

		[HarmonyPostfix, HarmonyPatch(methodName: nameof(StatusEffects.BecomeHidden), argumentTypes: new[] { typeof(ObjectReal) })]
		private static void BecomeHidden_Postfix(ObjectReal hiddenInObject, StatusEffects __instance)
		{
			if (!(hiddenInObject is null))
			{
				Agent agent = __instance.agent;
				Vector3 objectPosition = hiddenInObject.tr.position;
				agent.tr.position = new Vector2(objectPosition.x, objectPosition.y + BecomeHidden_GetPositionOffset(hiddenInObject));
				if (BecomeHidden_ShouldDisableHitbox(hiddenInObject))
				{
					agent.EnableHitboxes(false);
					agent.agentItemColliderTr.gameObject.SetActive(false);
				}

				UnderdarkCitizen.Handle_StatusEffects_BecomeHidden(__instance, hiddenInObject);
			}
		}

		[HarmonyTranspiler, HarmonyPatch(methodName: nameof(StatusEffects.BecomeHidden), argumentTypes: new[] { typeof(ObjectReal) })]
		private static IEnumerable<CodeInstruction> BecomeHidden_Transpiler(IEnumerable<CodeInstruction> instructionsEnumerable, ILGenerator generator)
		{
			List<CodeInstruction> instructions = instructionsEnumerable.ToList();

			FieldInfo statusEffetcs_gc = AccessTools.Field(typeof(StatusEffects), "gc");
			FieldInfo gameController_audiHandler = AccessTools.Field(typeof(GameController), nameof(GameController.audioHandler));
			FieldInfo statusEffects_agent = AccessTools.Field(typeof(StatusEffects), nameof(StatusEffects.agent));
			MethodInfo audioHandler_Play =
					AccessTools.Method(typeof(AudioHandler), nameof(AudioHandler.Play), new[] { typeof(PlayfieldObject), typeof(string) });
			MethodInfo patches_GetClipName =
					AccessTools.Method(typeof(StatusEffectsPatches), nameof(BecomeHidden_GetClipName), new[] { typeof(PlayfieldObject) });

			/*
			 * Replace:
			 *  this.gc.audioHandler.Play(this.agent, "Hide");
			 * With:
			 *  this.gc.audioHandler.Play(this.agent, StatusEffectsPatches.BecomeHidden_GetClipName(hiddenInObject))
			 */
			CodeReplacementPatch agentPositionPatch = new CodeReplacementPatch(
					expectedMatches: 1,
					prefixInstructionSequence: new List<CodeInstruction>
					{
							new CodeInstruction(OpCodes.Ldarg_0),
							new CodeInstruction(OpCodes.Ldfld, statusEffetcs_gc),
							new CodeInstruction(OpCodes.Ldfld, gameController_audiHandler), // this.gc.audioHandler

							new CodeInstruction(OpCodes.Ldarg_0),
							new CodeInstruction(OpCodes.Ldfld, statusEffects_agent) // this.agent
					},
					targetInstructionSequence: new List<CodeInstruction>
					{
							new CodeInstruction(OpCodes.Ldstr, "Hide") // string: "Hide"
					},
					insertInstructionSequence: new List<CodeInstruction>
					{
							new CodeInstruction(OpCodes.Ldarg_1),
							new CodeInstruction(OpCodes.Call, patches_GetClipName) // StatusEffectsPatches.BecomeHidden_GetClipName(hiddenInObject)
					},
					postfixInstructionSequence: new List<CodeInstruction>
					{
							new CodeInstruction(OpCodes.Callvirt, audioHandler_Play) // this.gc.audioHandler.Play(this.agent, "Hide")
					}
			);
			agentPositionPatch.ApplySafe(instructions, logger);

			return instructions;
		}

		private static void BecomeHidden_Transpiler_Marker(StatusEffects _this, ObjectReal hiddenInObject)
		{
			TranspilerMarkers.TargetSequenceStart();
			// TODO - can't access private field (ffs!)
		}

		[HarmonyPostfix, HarmonyPatch(methodName: nameof(StatusEffects.BecomeNotHidden))]
		private static void BecomeNotHidden_Postfix(StatusEffects __instance)
		{
			__instance.agent.agentCollider.enabled = true;
			__instance.agent.EnableHitboxes(true);
		}
		#endregion

		[HarmonyPostfix, HarmonyPatch(methodName: nameof(StatusEffects.GiveSpecialAbility), argumentTypes: new[] { typeof(string) })]
		private static void GiveSpecialAbility_Postfix(string abilityName, StatusEffects __instance)
		{
			if (__instance.agent.inventory.equippedSpecialAbility != null)
			{
				InvItem ability = __instance.agent.inventory.equippedSpecialAbility;
				Agent agent = __instance.agent;

				string[] magicAbilities =
				{
					cSpecialAbility.ChronomanticDilation,
					cSpecialAbility.PyromanticJet,
					cSpecialAbility.TelemanticBlink
				};

				if (magicAbilities.Contains(abilityName))
				{
					ability.otherDamage = 0; // Bitwise variables

					ability.initCount = Shared.CalcMaxMana(agent);
					ability.maxAmmo = Shared.CalcMaxMana(agent);
					ability.rechargeAmountInverse = Shared.CalcMaxMana(agent);
				}
			}
		}

		[HarmonyPostfix,
		 HarmonyPatch(methodName: nameof(StatusEffects.ChangeHealth), argumentTypes: new[] { typeof(float), typeof(PlayfieldObject), typeof(NetworkInstanceId), typeof(float), typeof(string), typeof(byte) })]
		private static void ChangeHealth_Postfix(float healthNum, PlayfieldObject damagerObject, NetworkInstanceId cameFromClient, float clientFinalHealthNum, string damagerObjectName, byte extraVar, StatusEffects __instance)
		{
			// TODO change to transpiler (see todo in Warlord)
			Agent hurtAgent = __instance.agent;
			if (hurtAgent.health <= hurtAgent.healthMax * 0.4f && hurtAgent.health > 0f && hurtAgent.isPlayer == 0
					&& GameController.gameController.serverPlayer && hurtAgent.CanShakeDown() && hurtAgent.justHitByAgent2 != null
					&& (hurtAgent.justHitByAgent2.isPlayer != 0 || hurtAgent.justHitByAgent2.hasEmployer))
			{
				Agent shakedowningAgent = GameController.gameController.playerAgentList.First(agent => agent.HasTrait<Warlord>());
				if (shakedowningAgent != null)
				{
					hurtAgent.relationships.SetRel(shakedowningAgent, nameof(relStatus.Submissive));
					BMHeaderTools.SayDialogue(hurtAgent, cDialogue.WarlordSubmission, vNameType.Dialogue);
				}
			}
		}
	}
}