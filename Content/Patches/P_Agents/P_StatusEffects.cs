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
using System.Collections;
using BunnyMod.Content.Abilities;

namespace BunnyMod.Content.Patches
{
	[HarmonyPatch(declaringType: typeof(StatusEffects))]
	public static class P_StatusEffects
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		public static GameController GC => GameController.gameController;

		// TODO Fatass doesn't exist anymore (for now ?), move to CustomTrait::OnAdded
		[HarmonyPostfix,HarmonyPatch(methodName:nameof(StatusEffects.AddTrait), argumentTypes: new[] { typeof(string), typeof(bool), typeof(bool) })]
		public static void AddTrait_Postfix(string traitName, bool isStarting, bool justRefresh, StatusEffects __instance) // Postfix
		{
			Agent agent = __instance.agent;

			if (traitName == cTrait.Fatass)
			{
				agent.SetEndurance(agent.enduranceStatMod + 1);
				agent.SetSpeed(agent.speedStatMod - 1);
			}
		}

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
			MethodInfo audioHandler_Play = AccessTools.Method(typeof(AudioHandler), nameof(AudioHandler.Play), new[] { typeof(PlayfieldObject), typeof(string) });
			MethodInfo patches_GetClipName = AccessTools.Method(typeof(P_StatusEffects), nameof(BecomeHidden_GetClipName), new[] { typeof(PlayfieldObject) });

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

		// Non-Patch
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

					ability.initCount = BMAbilityController.CalcMaxMana(agent);
					ability.maxAmmo = BMAbilityController.CalcMaxMana(agent);
					ability.rechargeAmountInverse = BMAbilityController.CalcMaxMana(agent);
				}
			}
		}

		[HarmonyPostfix, HarmonyPatch(methodName: nameof(StatusEffects.ChangeHealth), argumentTypes: new[] { typeof(float), typeof(PlayfieldObject), typeof(NetworkInstanceId), typeof(float), typeof(string), typeof(byte) })]
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

		// TODO Fatass doesn't exist anymore (for now ?), move to CustomTrait::OnRemoved
		[HarmonyPostfix, HarmonyPatch(methodName: nameof(StatusEffects.RemoveTrait), argumentTypes: new[] { typeof(string), typeof(bool) })]
		public static void RemoveTrait_Postfix(string traitName, bool onlyLocal, StatusEffects __instance) 
		{
			Agent agent = __instance.agent;

			if (traitName == cTrait.Fatass)
			{
				//TODO: CharacterCreation.CreatePointTallyText() for stat mods
				agent.SetEndurance(agent.enduranceStatMod - 1);
				agent.SetSpeed(agent.speedStatMod + 1);
			}
		}

		#region SpecialAbilityInterfaceCheck2
		// Non-Patch
		public static IEnumerator SpecialAbilityInterfaceCheck2_Enumerator(StatusEffects __instance)
		{
			// Sniper Headshot indicators

			#region Vanilla

			__instance.startedSpecialAbilityInterfaceCheck = true;

			if (__instance.startedSpecialAbilityInterfaceCheck)
			{
				do
				{
					if (GC.loadComplete &&
						__instance.agent.specialAbilityIndicator != null &&
						!__instance.agent.disappearedArcade &&
						__instance.agent.inventory.buffDisplay.specialAbilitySlot != null &&
						!__instance.agent.ghost)
					{
						string specialAbility = __instance.agent.specialAbility;

						if (specialAbility == vSpecialAbility.Bite)
						{
							bool flag = false;

							if ((__instance.agent.health < __instance.agent.healthMax || __instance.agent.oma.superSpecialAbility ||
									__instance.hasTrait("BiteFullHealth")) &&
								__instance.agent.bitingAgent == null &&
								__instance.CanShowSpecialAbilityIndicator())
							{
								Agent agent = (Agent)__instance.FindSpecialAbilityObject();

								if (agent != null)
								{
									__instance.agent.specialAbilityIndicator.ShowIndicator(agent, "Bite");
									flag = true;
								}
							}

							if (!flag)
								__instance.agent.specialAbilityIndicator.Revert();
						}
						else if (specialAbility == vSpecialAbility.ChloroformHankie)
						{
							bool targetAvailable = false;

							if (__instance.agent.inventory.equippedSpecialAbility.invItemCount == 0 && __instance.CanShowSpecialAbilityIndicator())
							{
								Agent targetAgent = __instance.CanSpecialAttack(false);

								if (targetAgent != null)
								{
									__instance.agent.specialAbilityIndicator.ShowIndicator(targetAgent, "Chloroform");
									targetAvailable = true;
								}

								__instance.agent.inventory.buffDisplay.specialAbilitySlot.MakeUsable();
							}

							if (!targetAvailable)
								__instance.agent.specialAbilityIndicator.Revert();
						}
						else if (specialAbility == vSpecialAbility.Handcuffs)
						{
							bool flag3 = false;

							if (__instance.agent.arrestingAgent == null && __instance.CanShowSpecialAbilityIndicator())
							{
								Agent agent3 = (Agent)__instance.FindSpecialAbilityObject();

								if (agent3 != null)
								{
									if (agent3.statusEffects.IsInnocent(__instance.agent) && !__instance.agent.oma.superSpecialAbility)
										__instance.agent.specialAbilityIndicator.ShowIndicator(agent3, "Handcuffs", "Innocent");
									else
										__instance.agent.specialAbilityIndicator.ShowIndicator(agent3, "Handcuffs", "Guilty");

									flag3 = true;
								}
							}

							if (!flag3)
								__instance.agent.specialAbilityIndicator.Revert();
						}
						else if (specialAbility == vSpecialAbility.Enslave)
						{
							bool flag4 = false;

							if (__instance.agent.arrestingAgent == null && __instance.CanShowSpecialAbilityIndicator())
							{
								Agent agent4 = (Agent)__instance.FindSpecialAbilityObject();

								if (agent4 != null)
								{
									__instance.agent.specialAbilityIndicator.ShowIndicator(agent4, "Enslave");
									flag4 = true;
								}
							}

							if (!flag4)
								__instance.agent.specialAbilityIndicator.Revert();
						}
						else if (specialAbility == vSpecialAbility.Toss)
						{
							bool flag5 = false;

							if (__instance.agent.hoistingObject == null && __instance.CanShowSpecialAbilityIndicator())
							{
								ObjectReal objectReal = (ObjectReal)__instance.FindSpecialAbilityObject();

								if (objectReal != null)
								{
									flag5 = true;
									__instance.agent.specialAbilityIndicator.ShowIndicator(objectReal, "Toss");
								}
							}

							if (!flag5)
								__instance.agent.specialAbilityIndicator.Revert();
						}
						else if (specialAbility == vSpecialAbility.Cannibalize)
						{
							bool flag6 = false;

							if (__instance.agent.bitingAgent == null && __instance.CanShowSpecialAbilityIndicator())
							{
								Agent agent5 = (Agent)__instance.FindSpecialAbilityObject();

								if (agent5 != null && (__instance.agent.health < __instance.agent.healthMax ||
									(agent5.agentName == __instance.agent.oma.bigQuestTarget1 && __instance.agent.oma.bigQuestObjectCount == 0)))
								{
									__instance.agent.specialAbilityIndicator.ShowIndicator(agent5, "Cannibalize");
									flag6 = true;
								}
							}

							if (!flag6)
								__instance.agent.specialAbilityIndicator.Revert();
						}
						else if (specialAbility == vSpecialAbility.Possess)
						{
							bool flag7 = false;

							if (!__instance.agent.possessing && __instance.CanShowSpecialAbilityIndicator())
							{
								Agent agent6 = (Agent)__instance.FindSpecialAbilityObject();

								if (agent6 != null)
								{
									flag7 = true;
									__instance.agent.specialAbilityIndicator.ShowIndicator(agent6, "Possess");
								}
							}

							if (!flag7)
								__instance.agent.specialAbilityIndicator.Revert();
						}
						else if (specialAbility == vSpecialAbility.MechTransformation)
						{
							bool flag8 = false;

							if (__instance.agent.arrestingAgent == null && __instance.agent.inventory.equippedSpecialAbility.invItemCount == 0 &&
								__instance.CanShowSpecialAbilityIndicator())
							{
								Agent agent7 = (Agent)__instance.FindSpecialAbilityObject();

								if (agent7 != null)
								{
									__instance.agent.specialAbilityIndicator.ShowIndicator(agent7, "MechTransform");
									flag8 = true;
								}
							}

							if (!flag8)
								__instance.agent.specialAbilityIndicator.Revert();
						}
						else if (specialAbility == "InvisibleLimitedItem")
						{
							if (__instance.agent.invisible)
								__instance.agent.inventory.buffDisplay.specialAbilitySlot.MakeNotUsable();
							else if (__instance.agent.inventory.equippedSpecialAbility.invItemCount > 0)
								__instance.agent.inventory.buffDisplay.specialAbilitySlot.MakeNotUsable();
							else if (__instance.CanShowSpecialAbilityIndicator())
							{
								bool flag9 = true;

								for (int i = 0; i < GC.activeBrainAgentList.Count; i++)
								{
									Agent agent8 = GC.activeBrainAgentList[i];

									if (agent8 != __instance.agent && !agent8.dead)
									{
										Relationship relationship = agent8.relationships.GetRelationship(__instance.agent);

										if (relationship.HasLOS("") && relationship.relType != "Aligned" && relationship.relType != "Loyal" &&
											agent8.employer != __instance.agent && !agent8.oma.hidden)
										{
											__instance.agent.inventory.buffDisplay.specialAbilitySlot.MakeNotUsable();
											flag9 = false;

											break;
										}
									}
								}

								if (flag9)
								{
									for (int j = 0; j < GC.objectRealList.Count; j++)
									{
										if (GC.objectRealList[j].objectName == "SecurityCam")
										{
											SecurityCam securityCam = (SecurityCam)GC.objectRealList[j];

											if (securityCam.agentsInView.Contains(__instance.agent) && !securityCam.destroyed && securityCam.functional)
											{
												__instance.agent.inventory.buffDisplay.specialAbilitySlot.MakeNotUsable();
												flag9 = false;

												break;
											}
										}
									}
								}

								if (flag9)
									__instance.agent.inventory.buffDisplay.specialAbilitySlot.MakeUsable();
							}
						}
					}

					yield return new WaitForSeconds(0.1f);
				} while (__instance.startedSpecialAbilityInterfaceCheck);
			}

			#endregion

			if ((__instance.agent.HasTrait<Sniper>() || __instance.agent.HasTrait<Sniper2>()) &&
				__instance.agent.agentInvDatabase.equippedWeapon.invItemName == vItem.Revolver)
			{
				// Test for reduced range if hidden

				foreach (Agent targetAgent in GC.agentList)
				{
					if (!targetAgent.dead &&
						!targetAgent.ghost &&
						__instance.agent.movement.HasLOSAgent360(targetAgent) &&
						__instance.agent.movement.GetDistance(__instance.agent.gameObject, targetAgent.gameObject) > 6f)
					{
						__instance.agent.specialAbilityIndicator.ShowIndicator(targetAgent, "Chloroform"); // Placeholder for new sprite
					}
				}
			}

			yield break;
		}

		[HarmonyPrefix, HarmonyPatch(methodName: nameof(StatusEffects.SpecialAbilityInterfaceCheck))]
		public static bool SpecialAbilityInterfaceCheck2_Prefix(StatusEffects __instance, ref IEnumerator __result) 
		{
			__result = SpecialAbilityInterfaceCheck2_Enumerator(__instance);

			return false;
		}
		#endregion
	}
}