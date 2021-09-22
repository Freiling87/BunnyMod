using BepInEx.Logging;
using BunnyMod.Content.Logging;
using BunnyMod.Content.Traits;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;
using Random = UnityEngine.Random;

namespace BunnyMod.Content.Patches
{
	[HarmonyPatch(declaringType: typeof(MeleeHitbox))]
	public static class P_MeleeHitbox
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		public static GameController GC => GameController.gameController;

		[HarmonyPrefix, HarmonyPatch(methodName: nameof(MeleeHitbox.HitObject), argumentTypes: new[] { typeof(GameObject)})]
		public static bool HitObject_Prefix(GameObject hitObject, bool fromClient, MeleeHitbox __instance) 
		{
			// Spectral Strikes

			InvItem invItem = null;
			Agent hittingAgent = __instance.myMelee.agent;

			try
			{
				invItem = hittingAgent.inventory.equippedWeapon;

				if (hittingAgent.inventory.equippedWeapon.itemType == "WeaponProjectile")
					invItem = hittingAgent.inventory.fist;
			}
			catch { }

			if ((!__instance.ObjectListContains(hitObject) && __instance.myMelee.canDamage && __instance.canHitMore) || fromClient)
			{
				if (GC.multiplayerMode && GC.serverPlayer && !hittingAgent.localPlayer && !fromClient && hittingAgent.isPlayer != 0)
				{
					bool flag = true;

					if (hitObject.CompareTag("ObjectRealSprite"))
					{
						ObjectReal objectReal;

						if (hitObject.name.Contains("ExtraSprite"))
							objectReal = hitObject.transform.parent.transform.parent.GetComponent<ObjectReal>();
						else
							objectReal = hitObject.GetComponent<ObjectSprite>().objectReal;

						if (objectReal.objectName == "Window")
						{
							Window window = (Window)objectReal;

							if (window.FindDamage(__instance.myMelee, false, true, fromClient) >= 30)
								__instance.justHitWindow = true;
							else
							{
								if (window.hitWindowOnce)
									return false;

								__instance.justHitWindow = true;
							}
						}

						if (!objectReal.objectSprite.meshRenderer.enabled || !objectReal.notRealObject || !objectReal.OnFloor || !objectReal.meleeCanPass ||
							!objectReal.tempNoMeleeHits)
							return false;
					}

					if (!hitObject.CompareTag("ObjectRealSprite") && !hitObject.CompareTag("AgentSprite") && !hitObject.CompareTag("ItemImage") &&
						!hitObject.CompareTag("Wall"))
						flag = false;
					else if (hitObject.CompareTag("AgentSprite"))
					{
						Agent agent = hitObject.GetComponent<ObjectSprite>().agent;

						if (agent == hittingAgent)
							flag = false;

						if (agent.localPlayer && hittingAgent.isPlayer > 0 && !hittingAgent.localPlayer)
							flag = false;
					}

					if (flag)
					{
						__instance.FakeHit(hitObject);

						return false;
					}
				}

				#region Sorted by target type

				if (hitObject.CompareTag("MeleeHitbox"))
				{
					if (hittingAgent.statusEffects.hasTrait("BlocksSometimesHit2"))
					{
						if (GC.percentChance(60))
							hitObject = hitObject.GetComponent<MeleeColliderBox>().meleeHitbox.myMelee.agent.agentHitboxScript.go;
					}
					else if (hittingAgent.statusEffects.hasTrait("BlocksSometimesHit"))
					{
						if (GC.percentChance(30))
							hitObject = hitObject.GetComponent<MeleeColliderBox>().meleeHitbox.myMelee.agent.agentHitboxScript.go;
					}
					else
					{
						Agent agent2 = hitObject.GetComponent<MeleeColliderBox>().meleeHitbox.myMelee.agent;

						if (agent2.statusEffects.hasTrait("BlocksSometimesHit2"))
						{
							if (GC.percentChance(60))
								hitObject = hittingAgent.agentHitboxScript.go;
						}
						else if (agent2.statusEffects.hasTrait("BlocksSometimesHit") && GC.percentChance(30))
							hitObject = hittingAgent.agentHitboxScript.go;
					}
				}

				if (hitObject.CompareTag("ObjectRealSprite") && !__instance.myMelee.invItem.meleeNoHit)
				{
					ObjectReal objectReal2;

					if (hitObject.name.Contains("ExtraSprite"))
						objectReal2 = hitObject.transform.parent.transform.parent.GetComponent<ObjectReal>();
					else
						objectReal2 = hitObject.GetComponent<ObjectSprite>().objectReal;

					if (objectReal2.extraSprites.Count > 0)
						for (int i = 0; i < objectReal2.extraSprites.Count; i++)
							__instance.objectList.Add(objectReal2.extraSprites[i].gameObject);
					else
						__instance.objectList.Add(hitObject);

					if (!GC.serverPlayer && !hittingAgent.localPlayer && hittingAgent.mindControlAgent != GC.playerAgent &&
						objectReal2.objectSprite.meshRenderer.enabled && !objectReal2.notRealObject && !objectReal2.OnFloor && !objectReal2.meleeCanPass &&
						!objectReal2.tempNoMeleeHits)
					{
						__instance.FakeHit(hitObject);

						return false;
					}

					if (__instance.HasLOSMelee(objectReal2) && objectReal2.objectMeshRenderer.enabled)
					{
						bool flag2 = false;

						if (hittingAgent.statusEffects.hasTrait("HitObjectsNoNoise"))
							flag2 = true;

						if (objectReal2.objectName == "Window")
						{
							if (objectReal2.FindDamage(__instance.myMelee, false, true, fromClient) >= 30)
								__instance.justHitWindow = true;

							if (flag2)
							{
								Window component = objectReal2.GetComponent<Window>();
								component.StartCoroutine(component.TempNoNoise());
							}
						}

						if (!objectReal2.notRealObject && !objectReal2.OnFloor && (!objectReal2.meleeCanPass || __instance.justHitWindow) &&
							!objectReal2.tempNoMeleeHits)
						{
							objectReal2.Damage(__instance.myMelee, fromClient);

							if (!objectReal2.noDamageNoise && !__instance.myMelee.successfullySleepKilled && !flag2)
								GC.spawnerMain.SpawnNoise(objectReal2.FindDamageNoisePos(objectReal2.tr.position), (float)objectReal2.noiseHitVol, null, null,
									hittingAgent);

							if (hittingAgent != null && GC.serverPlayer)
								GC.OwnCheck(hittingAgent, hitObject, "Normal", 1);

							if (__instance.justHitWindow)
								__instance.justHitWindow = false;

							if (__instance.myMelee.hitParticlesTr != null && __instance.myMelee.meleeContainerTr != null)
								GC.spawnerMain.SpawnParticleEffect("ObjectDestroyed", __instance.myMelee.hitParticlesTr.position,
									__instance.myMelee.meleeContainerTr.eulerAngles.z - 90f);

							if (objectReal2.destroying || objectReal2.justDamaged)
							{
								if (hittingAgent.isPlayer > 0 && hittingAgent.localPlayer && !objectReal2.noDestroyEffects)
								{
									GC.ScreenShake(0.2f, 80f, hittingAgent.tr.position, hittingAgent);
									GC.FreezeFrames(1);
								}
							}
							else if (hittingAgent.isPlayer > 0 && hittingAgent.localPlayer)
							{
								GC.ScreenShake(0.1f, 80f, hittingAgent.tr.position, hittingAgent);
								GC.FreezeFrames(0);
							}

							GC.alienFX.HitObject(hittingAgent);
							__instance.MeleeHitEffect(hitObject);
							GC.playerControl.Vibrate(hittingAgent.isPlayer, Mathf.Clamp((float)objectReal2.damagedAmount / 100f + 0.05f, 0f, 0.25f),
								Mathf.Clamp((float)objectReal2.damagedAmount / 132f + 0.05f, 0f, 0.2f));

							if (!GC.serverPlayer && (hittingAgent.isPlayer > 0 || hittingAgent.mindControlAgent == GC.playerAgent))
							{
								if (hittingAgent.isPlayer != 0)
								{
									hittingAgent.objectMult.CallCmdMeleeHitObjectReal(objectReal2.objectNetID);

									return false;
								}

								GC.playerAgent.objectMult.CallCmdMeleeHitObjectRealNPC(hittingAgent.objectNetID, objectReal2.objectNetID);

								return false;
							}
							else if (GC.serverPlayer && GC.multiplayerMode)
							{
								hittingAgent.objectMult.CallRpcMeleeHitObjectFake(objectReal2.objectNetID);

								return false;
							}
						}
					}
				}
				else if (hitObject.CompareTag("AgentSprite"))
				{
					__instance.objectList.Add(hitObject);
					Agent hitAgent = hitObject.GetComponent<ObjectSprite>().agent;

					if (GC.multiplayerMode)
						if ((GC.serverPlayer && hittingAgent.localPlayer && hitAgent.isPlayer > 0 && !hitAgent.localPlayer) ||
							(GC.serverPlayer && hittingAgent.isPlayer == 0 && hitAgent.isPlayer > 0 && !hitAgent.localPlayer) ||
							(GC.multiplayerMode && GC.serverPlayer && hittingAgent.isPlayer == 0 && hitAgent.isPlayer == 0 &&
								hittingAgent.mindControlAgent != null && hittingAgent.mindControlAgent != GC.playerAgent && !hitAgent.dead) ||
							(GC.multiplayerMode && GC.serverPlayer && hittingAgent.isPlayer == 0 && hitAgent.isPlayer == 0 &&
								hitAgent.mindControlAgent != null && hitAgent.mindControlAgent != GC.playerAgent && !hitAgent.dead) ||
							(!GC.serverPlayer && hittingAgent.isPlayer == 0 && !hitAgent.localPlayer && hittingAgent != hitAgent &&
								((hittingAgent.mindControlAgent != GC.playerAgent && hitAgent.mindControlAgent != GC.playerAgent) || hitAgent.dead)) ||
							(!GC.serverPlayer && hittingAgent.isPlayer > 0 && !hittingAgent.localPlayer && !hitAgent.localPlayer) ||
							(!GC.serverPlayer && (hittingAgent.localPlayer || hittingAgent.mindControlAgent == GC.playerAgent) && hitAgent.isPlayer > 0 &&
								!hitAgent.localPlayer) ||
							(!GC.serverPlayer && hittingAgent.isPlayer != 0 && !hittingAgent.localPlayer && hitAgent.isPlayer != 0 && !hitAgent.localPlayer))
						{
							__instance.FakeHit(hitObject);

							return false;
						}

					if (hittingAgent != hitAgent &&
						(!hitAgent.ghost || BMCombat.CanAgentMeleeHitGhost(hittingAgent)) &&
						!hitAgent.fellInHole && !GC.cinematic && __instance.HasLOSMelee(hitAgent))
					{
						__instance.objectList.Add(hitAgent.melee.meleeHitbox.gameObject);

						if (__instance.myMelee.invItem.meleeNoHit && !hitAgent.dead)
						{
							Relationship relationship = hitAgent.relationships.GetRelationship(hittingAgent);

							if (!hitAgent.movement.HasLOSObjectBehind(hittingAgent) || hitAgent.sleeping || hittingAgent.isPlayer == 0 ||
								hittingAgent.invisible || (__instance.myMelee.invItem.invItemName == "StealingGlove" && hittingAgent.oma.superSpecialAbility))
							{
								__instance.canHitMore = false;

								if (__instance.myMelee.invItem.invItemName == "ChloroformHankie")
								{
									if (!fromClient)
										__instance.HitAftermath(hitObject, fromClient, hitAgent, __instance.myMelee.invItem.invItemName);
								}
								else if (__instance.myMelee.invItem.invItemName == "StealingGlove")
								{
									if (hittingAgent.oma.superSpecialAbility)
										__instance.HitAftermath(hitObject, fromClient, hitAgent, __instance.myMelee.invItem.invItemName);
									else if (hittingAgent.movement.HasLOSObject(hitAgent, "360", false, true))
									{
										hitAgent.agentHelperTr.localPosition = new Vector3(-0.64f, 0f, 0f);

										if ((!GC.tileInfo.IsOverlapping(hitAgent.agentHelperTr.position, "Wall") || hitAgent.sleeping) && !fromClient)
											__instance.HitAftermath(hitObject, fromClient, hitAgent, __instance.myMelee.invItem.invItemName);
									}
								}
							}
							else
							{
								if (__instance.myMelee.invItem.invItemName == "StealingGlove" && relationship.relType != "Aligned" &&
									relationship.relType != "Submissive" && hittingAgent.movement.HasLOSObject(hitAgent, "360", false, true) && !fromClient)
									__instance.HitAftermath(hitObject, fromClient, hitAgent, "StealingGloveFail");

								if (GC.serverPlayer)
									GC.spawnerMain.SpawnNoise(hittingAgent.tr.position, 0f, null, null, hittingAgent);
							}

							if ((__instance.myMelee.invItem.invItemName == "ChloroformHankie" || __instance.myMelee.invItem.invItemName == "Handcuffs") &&
								relationship.relType != "Aligned" && relationship.relType != "Submissive" && !fromClient)
								__instance.HitAftermath(hitObject, fromClient, hitAgent, "ChloroformHankieFail");
						}

						bool flag3 = !__instance.myMelee.invItem.meleeNoHit && hittingAgent.DontHitAlignedCheck(hitAgent);

						if (flag3)
						{
							hitAgent.melee.meleeHitbox.objectList.Add(__instance.gameObject);
							hitAgent.melee.meleeHitbox.objectList.Add(hittingAgent.sprTr.gameObject);

							if (hittingAgent.zombified && hitAgent.isPlayer == 0 && !hitAgent.oma.bodyGuarded)
								hitAgent.zombieWhenDead = true;

							if (hitAgent.isPlayer == 0 && hittingAgent.isPlayer != 0 && !hitAgent.dead && hitAgent.agentName != "Zombie" && !hitAgent.inhuman &&
								!hitAgent.mechEmpty && !hitAgent.mechFilled && hittingAgent.localPlayer &&
								!hitAgent.statusEffects.hasStatusEffect("Invincible"))
							{
								if (hittingAgent.statusEffects.hasTrait("FleshFeast2"))
									hittingAgent.statusEffects.ChangeHealth(6f);
								else if (hittingAgent.statusEffects.hasTrait("FleshFeast"))
									hittingAgent.statusEffects.ChangeHealth(3f);
							}

							if (GC.serverPlayer || hitAgent.health > 0f || hitAgent.dead)
								hitAgent.Damage(__instance.myMelee, fromClient);

							hittingAgent.relationships.FollowerAlert(hitAgent);

							if (hitAgent.statusEffects.hasTrait("AttacksDamageAttacker2") &&
								(!hittingAgent.ghost || BMCombat.CanAgentMeleeHitGhost(hittingAgent)))
							{
								int myChance = hitAgent.DetermineLuck(20, "AttacksDamageAttacker", true);

								if (GC.percentChance(myChance))
								{
									hittingAgent.lastHitByAgent = hitAgent;
									hittingAgent.justHitByAgent2 = hitAgent;
									hittingAgent.lastHitByAgent = hitAgent;
									hittingAgent.deathMethod = "AttacksDamageAttacker";
									hittingAgent.deathKiller = hitAgent.agentName;
									hittingAgent.statusEffects.ChangeHealth(-10f);
								}
							}
							else if (hitAgent.statusEffects.hasTrait("AttacksDamageAttacker") &&
								(!hittingAgent.ghost || BMCombat.CanAgentMeleeHitGhost(hittingAgent)))
							{
								int myChance2 = hitAgent.DetermineLuck(20, "AttacksDamageAttacker", true);

								if (GC.percentChance(myChance2))
								{
									hittingAgent.lastHitByAgent = hitAgent;
									hittingAgent.justHitByAgent2 = hitAgent;
									hittingAgent.lastHitByAgent = hitAgent;
									hittingAgent.deathMethod = "AttacksDamageAttacker";
									hittingAgent.deathKiller = hitAgent.agentName;
									hittingAgent.statusEffects.ChangeHealth(-5f);
								}
							}

							if (hitAgent.justDied && hittingAgent.isPlayer > 0 && !GC.coopMode && !GC.fourPlayerMode && !GC.multiplayerMode &&
								GC.sessionDataBig.slowMotionCinematic && GC.percentChance(25))
							{
								if (GC.challenges.Contains("LowHealth"))
								{
									if (GC.percentChance(50))
										GC.StartCoroutine(GC.SetSecondaryTimeScale(0.1f, 0.13f));
								}
								else
									GC.StartCoroutine(GC.SetSecondaryTimeScale(0.1f, 0.13f));
							}

							float num = 0f;

							if (__instance.myMelee.successfullySleepKilled || __instance.myMelee.successfullyBackstabbed)
								num = 0f;
							else if ((!hitAgent.dead || hitAgent.justDied) && !hitAgent.disappeared)
								num = (float)Mathf.Clamp(hitAgent.damagedAmount * 20, 80, 9999);
							else if (!hitAgent.disappeared)
								num = 80f;

							if (hittingAgent.statusEffects.hasTrait("CauseBiggerKnockback"))
								num *= 2f;

							Vector3 position = hitAgent.tr.position;

							if (!hitAgent.disappeared && !fromClient)
								hitAgent.movement.KnockBackBullet(__instance.myMelee.meleeContainerTr.gameObject, num, true, hittingAgent);

							bool flag4 = false;

							if (hitAgent.hasEmployer && hitAgent.employer.statusEffects.hasSpecialAbility("ProtectiveShell") &&
								hitAgent.employer.objectMult.chargingSpecialLunge)
								flag4 = true;

							if (hitAgent.statusEffects.hasSpecialAbility("ProtectiveShell") && hitAgent.objectMult.chargingSpecialLunge)
								flag4 = true;

							if (flag4)
							{
								bool flag5 = true;

								if (GC.multiplayerMode && GC.serverPlayer)
								{
									if (hitAgent.isPlayer != 0 && !hitAgent.localPlayer && hittingAgent.isPlayer == 0)
										flag5 = false;

									if (hittingAgent.isPlayer != 0 && !hittingAgent.localPlayer && hitAgent.isPlayer == 0)
										flag5 = false;
								}

								if (flag5)
								{
									hittingAgent.movement.KnockBackBullet(hitAgent.gameObject, 240f, true, hitAgent);

									if (GC.serverPlayer && hittingAgent.isPlayer == 0 && invItem.invItemName != "Fist" && !hitAgent.warZoneAgent)
									{
										int myChance3 = hitAgent.DetermineLuck(15, "ChanceToKnockWeapons", true);

										if (GC.percentChance(myChance3))
										{
											InvItem invItem2 = hittingAgent.inventory.FindItem(invItem.invItemName);
											hittingAgent.inventory.DestroyItem(invItem2);
											GC.spawnerMain.SpillItem(__instance.tr.position, invItem2);
											GC.spawnerMain.SpawnStatusText(hittingAgent, "OutOfAmmo", invItem2.invItemName, "Item");

											if (!GC.serverPlayer && (hittingAgent.isPlayer != 0 || hittingAgent.mindControlAgent == GC.playerAgent))
												hittingAgent.objectMultPlayfield.SpawnStatusText("OutOfAmmo", invItem2.invItemName, "Item",
													hittingAgent.objectNetID, "", "");

											hittingAgent.statusEffects.CreateBuffText("DroppedWeapon", hittingAgent.objectNetID);
											hittingAgent.dontPickUpWeapons = true;
										}
									}
								}
							}

							if (!GC.serverPlayer && (hittingAgent.localPlayer || hittingAgent.mindControlAgent == GC.playerAgent))
							{
								hittingAgent.objectMultPlayfield.TempDisableNetworkTransform(hitAgent);
								Quaternion localRotation = __instance.myMelee.meleeHelperTr.localRotation;
								__instance.myMelee.meleeHelperTr.rotation = __instance.myMelee.meleeContainerTr.rotation;
								__instance.myMelee.meleeHelperTr.position = __instance.myMelee.meleeContainerTr.position;
								__instance.myMelee.meleeHelperTr.localPosition = new Vector3(__instance.myMelee.meleeHelperTr.localPosition.x,
									__instance.myMelee.meleeHelperTr.localPosition.y + 10f, __instance.myMelee.meleeHelperTr.localPosition.z);
								Vector3 position2 = __instance.myMelee.meleeHelperTr.position;
								__instance.myMelee.meleeHelperTr.localPosition = Vector3.zero;
								__instance.myMelee.meleeHelperTr.localRotation = localRotation;

								if (!hittingAgent.testingNewClientLerps)
								{
									if (hittingAgent.isPlayer != 0)
										hittingAgent.objectMult.CallCmdMeleeHitAgent(hitAgent.objectNetID, position2, (int)num, position,
											hitAgent.rb.velocity);
									else
										GC.playerAgent.objectMult.CallCmdMeleeHitAgentNPC(hittingAgent.objectNetID, hitAgent.objectNetID, position2, (int)num,
											position, hitAgent.rb.velocity);
								}
							}
							else if (GC.multiplayerMode && GC.serverPlayer)
								hittingAgent.objectMult.CallRpcMeleeHitObjectFake(hitAgent.objectNetID);

							if ((hittingAgent.isPlayer > 0 && hittingAgent.localPlayer) || (hitAgent.isPlayer > 0 && hitAgent.localPlayer))
							{
								if (hitAgent.justDied)
									GC.ScreenShake(0.25f, (float)Mathf.Clamp(15 * hitAgent.damagedAmount, 160, 500), Vector2.zero, hittingAgent);
								else
									GC.ScreenShake(0.2f, (float)Mathf.Clamp(15 * hitAgent.damagedAmount, 0, 500), Vector2.zero, hittingAgent);
							}

							GC.alienFX.PlayerHitEnemy(hittingAgent);
							hittingAgent.combat.meleeJustHitCooldown = hittingAgent.combat.meleeJustHitTimeStart;
							hittingAgent.combat.meleeJustHitCloseCooldown = hittingAgent.combat.meleeJustHitCloseTimeStart;

							if (GC.serverPlayer)
							{
								if (__instance.myMelee.successfullyBackstabbed)
									GC.spawnerMain.SpawnNoise(__instance.tr.position, 0.7f, null, null, hittingAgent);
								else if (!__instance.myMelee.successfullySleepKilled)
									GC.spawnerMain.SpawnNoise(__instance.tr.position, 1f, null, null, hittingAgent);
							}

							__instance.MeleeHitEffect(hitObject);
							GC.playerControl.Vibrate(hittingAgent.isPlayer, Mathf.Clamp((float)hitAgent.damagedAmount / 100f + 0.05f, 0f, 0.25f),
								Mathf.Clamp((float)hitAgent.damagedAmount / 132f + 0.05f, 0f, 0.2f));

							if (GC.levelType == "Tutorial")
							{
								GC.tutorial.MeleeTarget(hitAgent);

								return false;
							}
						}
					}
				}
				else if (hitObject.CompareTag("ItemImage"))
				{
					__instance.objectList.Add(hitObject);
					Item component2 = hitObject.transform.parent.GetComponent<Item>();

					if (!GC.serverPlayer && !hittingAgent.localPlayer && hittingAgent.mindControlAgent != GC.playerAgent)
					{
						__instance.FakeHit(hitObject);

						return false;
					}

					if (!component2.justSpilled && !__instance.myMelee.invItem.meleeNoHit && component2.itemObject == null &&
						__instance.HasLOSMelee(component2))
					{
						Vector3 position3 = component2.tr.position;

						if (!fromClient)
							component2.movement.KnockBackBullet(__instance.myMelee.meleeContainerTr.gameObject, 250f, true, hittingAgent);

						component2.Damage(__instance.myMelee, fromClient);

						if (component2.invItem.reactOnTouch)
							component2.TouchMe(hittingAgent, "MeleeHitbox");

						component2.thrower = hittingAgent;

						if (component2.go.activeSelf)
							component2.StartCoroutine(component2.HitCauserCoroutine(hittingAgent));

						if (GC.serverPlayer && !__instance.myMelee.successfullySleepKilled && !hittingAgent.statusEffects.hasTrait("HitObjectsNoNoise"))
							GC.spawnerMain.SpawnNoise(__instance.tr.position, 1f, null, null, hittingAgent);

						if (component2.startingOwner != 0 && GC.serverPlayer && !hittingAgent.statusEffects.hasTrait("NoStealPenalty"))
							GC.OwnCheck(hittingAgent, hitObject.transform.parent.gameObject, "Normal", 1);

						Physics2D.IgnoreCollision(component2.myCollider2D, hittingAgent.myCollider2D, true);
						Physics2D.IgnoreCollision(component2.myCollider2D, hittingAgent.agentItemCollider, true);
						__instance.MeleeHitEffect(hitObject);
						GC.playerControl.Vibrate(hittingAgent.isPlayer, Mathf.Clamp((float)__instance.myMelee.invItem.meleeDamage / 100f + 0.05f, 0f, 0.25f),
							Mathf.Clamp((float)__instance.myMelee.invItem.meleeDamage / 132f + 0.05f, 0f, 0.2f));

						if (!GC.serverPlayer && (hittingAgent.localPlayer || hittingAgent.mindControlAgent == GC.playerAgent))
						{
							hittingAgent.objectMultPlayfield.TempDisableNetworkTransform(component2);
							Quaternion localRotation2 = __instance.myMelee.meleeHelperTr.localRotation;
							__instance.myMelee.meleeHelperTr.rotation = __instance.myMelee.meleeContainerTr.rotation;
							__instance.myMelee.meleeHelperTr.position = __instance.myMelee.meleeContainerTr.position;
							__instance.myMelee.meleeHelperTr.localPosition = new Vector3(__instance.myMelee.meleeHelperTr.localPosition.x,
								__instance.myMelee.meleeHelperTr.localPosition.y + 10f, __instance.myMelee.meleeHelperTr.localPosition.z);
							Vector3 position4 = __instance.myMelee.meleeHelperTr.position;
							__instance.myMelee.meleeHelperTr.localPosition = Vector3.zero;
							__instance.myMelee.meleeHelperTr.localRotation = localRotation2;

							if (hittingAgent.isPlayer != 0)
							{
								hittingAgent.objectMult.CallCmdMeleeHitItem(component2.objectNetID, position4, 250, position3, component2.rb.velocity);

								return false;
							}

							GC.playerAgent.objectMult.CallCmdMeleeHitItemNPC(hittingAgent.objectNetID, component2.objectNetID, position4, 250, position3,
								component2.rb.velocity);

							return false;
						}
						else if (GC.multiplayerMode)
						{
							bool serverPlayer = GC.serverPlayer;

							return false;
						}
					}
				}
				else if (hitObject.CompareTag("MeleeHitbox"))
				{
					Melee melee = hitObject.GetComponent<MeleeColliderBox>().meleeHitbox.myMelee;
					__instance.objectList.Add(melee.meleeHitbox.gameObject);

					if ((__instance.myMelee.invItem.invItemName == "StealingGlove" || __instance.myMelee.invItem.invItemName == "ChloroformHankie") ||
						(melee.invItem.invItemName == "StealingGlove" || melee.invItem.invItemName == "ChloroformHankie"))
						return false;

					Agent agent4 = hitObject.GetComponent<MeleeColliderBox>().meleeHitbox.myMelee.agent;

					if ((GC.serverPlayer && hittingAgent.isPlayer == 0 && agent4.isPlayer > 0 && !agent4.localPlayer) ||
						(!GC.serverPlayer && hittingAgent.isPlayer == 0 && !agent4.localPlayer && hittingAgent != agent4 &&
							((agent4.mindControlAgent != GC.playerAgent && hittingAgent.mindControlAgent != GC.playerAgent) || agent4.dead)))
						return false;

					if ((GC.multiplayerMode && GC.serverPlayer && hittingAgent.isPlayer == 0 && agent4.isPlayer == 0 && hittingAgent.mindControlAgent != null &&
							hittingAgent.mindControlAgent != GC.playerAgent && !agent4.dead) ||
						(GC.multiplayerMode && GC.serverPlayer && hittingAgent.isPlayer == 0 && agent4.isPlayer == 0 && agent4.mindControlAgent != null &&
							agent4.mindControlAgent != GC.playerAgent && !agent4.dead))
					{
						__instance.FakeHit(hitObject);
						return false;
					}

					if ((!GC.serverPlayer && hittingAgent.isPlayer > 0 && !hittingAgent.localPlayer && !agent4.localPlayer) ||
						(!hittingAgent.DontHitAlignedCheck(agent4)))
						return false;

					if (hittingAgent != agent4 && hittingAgent.agentSpriteTransform.localScale.x != 3f && agent4.agentSpriteTransform.localScale.x != 3f &&
						hittingAgent.agentSpriteTransform.localScale.x > 0.34f && agent4.agentSpriteTransform.localScale.x > 0.34f &&
						!__instance.myMelee.invItem.meleeNoHit && __instance.HasLOSMelee(melee.agent))
					{
						melee.meleeHitbox.objectList.Add(__instance.gameObject);
						__instance.objectList.Add(agent4.sprTr.gameObject);
						melee.meleeHitbox.objectList.Add(hittingAgent.sprTr.gameObject);
						int num2 = hittingAgent.FindDamageTestOnly(hitObject.GetComponent<MeleeColliderBox>().meleeHitbox.myMelee);
						int num3 = agent4.FindDamageTestOnly(__instance.myMelee);
						int num4 = 0;
						int num5;

						if (melee.specialLunge)
							num5 = Mathf.Clamp(300, 100, 200);
						else
							num5 = Mathf.Clamp(num2 * 10, 100, 200);

						if (__instance.myMelee.specialLunge && !fromClient)
							num4 = Mathf.Clamp(300, 100, 200);
						else if (!fromClient)
							num4 = Mathf.Clamp(num3 * 10, 100, 200);

						Vector3 position5 = agent4.tr.position;
						bool flag6 = true;

						if (GC.multiplayerMode && GC.serverPlayer)
						{
							if (agent4.isPlayer != 0 && !agent4.localPlayer && hittingAgent.isPlayer == 0)
								flag6 = false;

							if (hittingAgent.isPlayer != 0 && !hittingAgent.localPlayer && agent4.isPlayer == 0)
								flag6 = false;
						}

						if (flag6)
						{
							hittingAgent.movement.KnockBackBullet(melee.meleeContainerTr.gameObject, (float)num5, true, melee.agent);

							if (!fromClient)
								agent4.movement.KnockBackBullet(__instance.myMelee.meleeContainerTr.gameObject, (float)num4, true, hittingAgent);
						}

						if (!GC.serverPlayer)
						{
							Agent agent5 = null;
							Agent agent6 = null;

							if (hittingAgent.localPlayer || hittingAgent.mindControlAgent == GC.playerAgent)
							{
								agent5 = hittingAgent;
								agent6 = agent4;
							}
							else if (agent4.localPlayer || agent4.mindControlAgent == GC.playerAgent)
							{
								agent5 = agent4;
								agent6 = hittingAgent;
							}

							if (agent5 != null)
							{
								agent5.objectMultPlayfield.TempDisableNetworkTransform(agent6);
								Quaternion localRotation3 = agent5.melee.meleeHelperTr.localRotation;
								agent5.melee.meleeHelperTr.rotation = agent5.melee.meleeContainerTr.rotation;
								agent5.melee.meleeHelperTr.position = agent5.melee.meleeContainerTr.position;
								agent5.melee.meleeHelperTr.localPosition = new Vector3(agent5.melee.meleeHelperTr.localPosition.x,
									agent5.melee.meleeHelperTr.localPosition.y + 10f, agent5.melee.meleeHelperTr.localPosition.z);
								Vector3 position6 = agent5.melee.meleeHelperTr.position;
								Debug.Log(string.Concat("MeleeHitMelee KnockToPosition: ", position6, " - ", position5));
								agent5.melee.meleeHelperTr.localPosition = Vector3.zero;
								agent5.melee.meleeHelperTr.localRotation = localRotation3;

								if (agent5.isPlayer != 0)
									agent5.objectMult.CallCmdMeleeHitMelee(agent6.objectNetID, position6, num5, position5, agent6.rb.velocity);
								else
									GC.playerAgent.objectMult.CallCmdMeleeHitMeleeNPC(agent5.objectNetID, agent6.objectNetID, position6, num5, position5,
										agent6.rb.velocity);
							}
						}
						else if (GC.multiplayerMode && GC.serverPlayer)
							hittingAgent.objectMult.CallRpcMeleeHitObjectFake(agent4.objectNetID);

						if (!hittingAgent.ghost &&
							(!agent4.ghost || BMCombat.CanAgentMeleeHitGhost(hittingAgent)))
						{
							if (invItem.invItemName != "Fist" && agent4.inventory.equippedWeapon.invItemName == "Fist")
							{
								agent4.lastHitByAgent = hittingAgent;
								agent4.justHitByAgent2 = hittingAgent;
								agent4.lastHitByAgent = hittingAgent;
								agent4.deathMethodItem = invItem.invItemName;
								agent4.deathMethodObject = invItem.invItemName;
								agent4.deathMethod = invItem.invItemName;
								agent4.deathKiller = hittingAgent.agentName;

								if (!GC.serverPlayer || hittingAgent.localPlayer || hittingAgent.isPlayer == 0)
								{
									if (hittingAgent.zombified && agent4.isPlayer == 0 && !agent4.oma.bodyGuarded)
										agent4.zombieWhenDead = true;

									agent4.statusEffects.ChangeHealth(-1f);
								}
							}
							else if (invItem.invItemName == "Fist" && agent4.inventory.equippedWeapon.invItemName != "Fist")
							{
								hittingAgent.lastHitByAgent = agent4;
								hittingAgent.justHitByAgent2 = agent4;
								hittingAgent.lastHitByAgent = agent4;
								agent4.deathMethodItem = invItem.invItemName;
								agent4.deathMethodObject = invItem.invItemName;
								hittingAgent.deathMethod = agent4.inventory.equippedWeapon.invItemName;
								hittingAgent.deathKiller = agent4.agentName;

								if (!GC.serverPlayer || hittingAgent.localPlayer || hittingAgent.isPlayer == 0)
								{
									if (agent4.zombified && hittingAgent.isPlayer == 0 && !hittingAgent.oma.bodyGuarded)
										hittingAgent.zombieWhenDead = true;

									hittingAgent.statusEffects.ChangeHealth(-1f);
								}
							}
						}

						hittingAgent.inventory.DepleteMelee(5);
						agent4.inventory.DepleteMelee(5);
						__instance.MeleeHitEffect(hitObject);

						if ((hittingAgent.isPlayer > 0 && hittingAgent.localPlayer) || (melee.agent.isPlayer > 0 && melee.agent.localPlayer))
						{
							GC.ScreenShake(0.2f, 80f, hittingAgent.tr.position, hittingAgent);
							GC.FreezeFrames(1);
						}

						GC.alienFX.BlockAttack(hittingAgent);
						GC.alienFX.BlockAttack(melee.agent);

						if (!hittingAgent.killerRobot && !melee.agent.killerRobot)
						{
							GC.EnforcerAlertAttack(hittingAgent, melee.agent, 7.4f);
							GC.EnforcerAlertAttack(melee.agent, hittingAgent, 7.4f);
						}

						GC.playerControl.Vibrate(hittingAgent.isPlayer, Mathf.Clamp((float)__instance.myMelee.invItem.meleeDamage / 100f + 0.05f, 0f, 0.25f),
							Mathf.Clamp((float)__instance.myMelee.invItem.meleeDamage / 132f + 0.05f, 0f, 0.2f));
						hittingAgent.combat.meleeJustBlockedCooldown = hittingAgent.combat.meleeJustBlockedTimeStart;
						agent4.combat.meleeJustBlockedCooldown = agent4.combat.meleeJustBlockedTimeStart;

						if (GC.serverPlayer)
							GC.spawnerMain.SpawnNoise(__instance.tr.position, 1f, null, null, hittingAgent);

						if (__instance.myMelee.hitParticlesTr != null && __instance.myMelee.meleeContainerTr != null)
							GC.spawnerMain.SpawnParticleEffect("ObjectDestroyed", __instance.myMelee.hitParticlesTr.position,
								__instance.myMelee.meleeContainerTr.eulerAngles.z - 90f);

						if ((hittingAgent.statusEffects.hasTrait("ChanceToKnockWeapons") || hittingAgent.statusEffects.hasTrait("KnockWeapons")) &&
							GC.serverPlayer && agent4.isPlayer == 0 && agent4.inventory.equippedWeapon.invItemName != "Fist" && !agent4.warZoneAgent)
						{
							int myChance4 = 15;
							bool knockWeapons = hittingAgent.statusEffects.hasTrait("KnockWeapons");

							if (!knockWeapons)
								myChance4 = hittingAgent.DetermineLuck(15, "ChanceToKnockWeapons", true);

							if (GC.percentChance(myChance4) || knockWeapons)
							{
								InvItem invItem3 = agent4.inventory.FindItem(agent4.inventory.equippedWeapon.invItemName);
								agent4.inventory.DestroyItem(invItem3);
								GC.spawnerMain.SpillItem(__instance.tr.position, invItem3);
								GC.spawnerMain.SpawnStatusText(agent4, "OutOfAmmo", invItem3.invItemName, "Item");

								if (!GC.serverPlayer)
									agent4.objectMultPlayfield.SpawnStatusText("OutOfAmmo", invItem3.invItemName, "Item", hittingAgent.objectNetID, "", "");

								agent4.statusEffects.CreateBuffText("DroppedWeapon");
								agent4.dontPickUpWeapons = true;
							}
						}

						if ((agent4.statusEffects.hasTrait("ChanceToKnockWeapons") || agent4.statusEffects.hasTrait("KnockWeapons")) && GC.serverPlayer &&
							hittingAgent.isPlayer == 0 && invItem.invItemName != "Fist" && !agent4.warZoneAgent)
						{
							int myChance5 = 15;

							if (!agent4.statusEffects.hasTrait("KnockWeapons"))
								myChance5 = agent4.DetermineLuck(15, "ChanceToKnockWeapons", true);

							if (GC.percentChance(myChance5) || agent4.statusEffects.hasTrait("KnockWeapons"))
							{
								InvItem invItem4 = hittingAgent.inventory.FindItem(invItem.invItemName);
								hittingAgent.inventory.DestroyItem(invItem4);
								GC.spawnerMain.SpillItem(__instance.tr.position, invItem4);
								GC.spawnerMain.SpawnStatusText(hittingAgent, "OutOfAmmo", invItem4.invItemName, "Item");

								if (!GC.serverPlayer && (hittingAgent.isPlayer != 0 || hittingAgent.mindControlAgent == GC.playerAgent))
									hittingAgent.objectMultPlayfield.SpawnStatusText("OutOfAmmo", invItem4.invItemName, "Item", hittingAgent.objectNetID, "",
										"");

								hittingAgent.statusEffects.CreateBuffText("DroppedWeapon", hittingAgent.objectNetID);
								hittingAgent.dontPickUpWeapons = true;

								return false;
							}
						}
					}
				}
				else if (hitObject.CompareTag("BulletHitbox"))
				{
					__instance.objectList.Add(hitObject);

					if (hittingAgent.isPlayer > 0)
					{
						bool localPlayer = hittingAgent.localPlayer;

						return false;
					}
				}
				else if (hitObject.CompareTag("Wall"))
				{
					__instance.objectList.Add(hitObject);

					if (!__instance.hitWall && !__instance.myMelee.invItem.meleeNoHit)
					{
						__instance.hitWall = true;
						bool flag8 = false;
						int damage = hittingAgent.FindDamage(__instance.myMelee, true, fromClient);

						if (hittingAgent.agentSpriteTransform.localScale.x > 1f)
						{
							damage = 200;
							TileData tileData = GC.tileInfo.GetTileData(hitObject.transform.position);

							if (tileData.wallMaterial != wallMaterialType.Steel && tileData.wallMaterial != wallMaterialType.Glass)
								__instance.hitWall = false;

							if (__instance.hitWall2 == 0)
								__instance.hitWall2 = 1;
						}

						bool streamingWorld = GC.streamingWorld;
						int num7 = 30;
						int num8 = 50;
						int num9 = 50;
						int num10 = 50;
						int num11 = 200;

						if (GC.challenges.Contains("WallsEasilySmashed"))
						{
							num7 = 5;
							num8 = 10;
							num9 = 50;
							num10 = 50;
							num11 = 200;
						}

						if (hittingAgent.statusEffects.hasTrait("MeleeDestroysWalls2") && invItem.invItemName != "Fist")
						{
							num7 = 0;
							num8 = 0;
							num9 = 0;
							num10 = 0;
							num11 = 0;
						}
						else if (hittingAgent.statusEffects.hasTrait("MeleeDestroysWalls") && invItem.invItemName != "Fist")
						{
							num7 = 0;
							num8 = 0;
							num9 = 0;
							num10 = 50;
							num11 = 200;
						}

						if (damage >= num7)
						{
							TileData tileData2 = GC.tileInfo.GetTileData(hitObject.transform.position);

							if ((tileData2.wallMaterial == wallMaterialType.Wood && damage >= num7) ||
								(tileData2.wallMaterial == wallMaterialType.Normal && damage >= num8) ||
								(tileData2.wallMaterial == wallMaterialType.Steel && damage >= num11) ||
								(tileData2.wallMaterial == wallMaterialType.Border && damage >= num10) ||
								(tileData2.wallMaterial == wallMaterialType.Glass && damage >= num9) ||
								(damage == num11 && tileData2.wallMaterial != wallMaterialType.Steel) ||
								(damage == num10 && tileData2.wallMaterial != wallMaterialType.Border && tileData2.wallMaterial != wallMaterialType.Steel &&
									tileData2.wallMaterial != wallMaterialType.LockdownWall))
							{
								Door.freerAgent = hittingAgent;
								GC.tileInfo.DestroyWallTileAtPosition(hitObject.transform.position.x, hitObject.transform.position.y, Vector3.zero, true,
									hittingAgent, false, true, false, hittingAgent, false);
								GC.audioHandler.Play(hittingAgent, "WallDestroy");

								if (hitObject.name.Contains("Glass"))
									GC.audioHandler.Play(hittingAgent, "WallDestroyGlass");

								hitObject.layer = 1;

								if (!hitObject.name.Contains("Border"))
									GC.stats.AddDestructionQuestPoints();

								if (hittingAgent.isPlayer > 0 && hittingAgent.localPlayer)
									GC.stats.AddToStat(hittingAgent, "Destruction", 1);

								GC.ScreenShake(0.25f, 160f, hittingAgent.tr.position, hittingAgent);

								if (__instance.hitWall2 == 1)
								{
									GC.FreezeFrames(3);
									__instance.hitWall2 = 2;
								}
								else if (damage != 200)
									GC.FreezeFrames(3);

								flag8 = true;

								if (invItem.invItemName != "Fist")
								{
									if (__instance.myMelee.depletedDuringThisAttack && invItem.invItemCount > 0)
									{
										__instance.myMelee.depletedDuringThisAttack = false;
										invItem.invItemCount += __instance.myMelee.depletedDuringThisAttackAmount;
									}

									if (hittingAgent.statusEffects.hasTrait("MeleeDestroysWalls2"))
										hittingAgent.inventory.DepleteMelee(10);
									else
										hittingAgent.inventory.DepleteMelee(20);
								}
							}
						}

						if (__instance.myMelee.hitParticlesTr != null && __instance.myMelee.meleeContainerTr != null)
							GC.spawnerMain.SpawnParticleEffect("ObjectDestroyed", __instance.myMelee.hitParticlesTr.position,
								__instance.myMelee.meleeContainerTr.eulerAngles.z - 90f);

						if (!__instance.myMelee.successfullySleepKilled && !hittingAgent.statusEffects.hasTrait("HitObjectsNoNoise") && (GC.serverPlayer ||
							(!GC.serverPlayer && (hittingAgent.localPlayer || hittingAgent.mindControlAgent == GC.playerAgent))))
						{
							GC.spawnerMain.SpawnNoise(hittingAgent.tr.position, 1f, null, null, hittingAgent);

							if (flag8)
								GC.spawnerMain.SpawnNoise(hitObject.transform.position, 1f, null, null, hittingAgent);
						}

						if (flag8 && hittingAgent != null)
							GC.OwnCheck(hittingAgent, hitObject, "Normal", 0);

						GC.audioHandler.Play(hittingAgent, "BulletHitWall");
						GC.playerControl.Vibrate(hittingAgent.isPlayer, Mathf.Clamp((float)__instance.myMelee.invItem.meleeDamage / 100f + 0.05f, 0f, 0.25f),
							Mathf.Clamp((float)__instance.myMelee.invItem.meleeDamage / 132f + 0.05f, 0f, 0.2f));

						if (!GC.serverPlayer && (hittingAgent.localPlayer || hittingAgent.mindControlAgent == GC.playerAgent))
						{
							if (hittingAgent.isPlayer != 0)
							{
								hittingAgent.objectMult.CallCmdMeleeHitWall(hitObject.transform.position);

								return false;
							}

							GC.playerAgent.objectMult.CallCmdMeleeHitWallNPC(hittingAgent.objectNetID, hitObject.transform.position);
						}
					}
				}

				#endregion
			}

			return false;
		}

		[HarmonyPrefix, HarmonyPatch(methodName: nameof(MeleeHitbox.MeleeHitEffect), argumentTypes: new [] {typeof(GameObject)})]
		public static bool MeleeHitEffect_Prefix(GameObject hitObject, MeleeHitbox __instance) 
		{
			// Spectral Strikes
			// This one is pared down to only relevant sections - not a replacement.

			Agent hittingAgent = __instance.myMelee.agent;

			if (BMCombat.CanAgentMeleeHitGhost(hittingAgent))
			{
				InvItem invItem = null;

				try
				{
					invItem = hittingAgent.inventory.equippedWeapon;

					if (hittingAgent.inventory.equippedWeapon.itemType == "WeaponProjectile")
						invItem = hittingAgent.inventory.fist;
				}
				catch { }

				if (hitObject.CompareTag("ObjectRealSprite"))
				{
					ObjectReal objectReal;

					if (hitObject.name.Contains("ExtraSprite"))
						objectReal = hitObject.transform.parent.transform.parent.GetComponent<ObjectReal>();
					else
						objectReal = hitObject.GetComponent<ObjectSprite>().objectReal;

					if (__instance.myMelee.recentFakeHitObjects.Contains(objectReal.go))
						return false;

					if (objectReal.damagedAmount < objectReal.damageThreshold && !objectReal.damageAccumulates)
					{
						if (objectReal.noMetallicSound)
						{
							GC.audioHandler.Play(objectReal, "BulletHitWall");

							return false;
						}

						GC.audioHandler.Play(objectReal, "BulletHitIndestructibleObject");

						return false;
					}
					else if (!objectReal.noDamageSound)
					{
						if (objectReal.specialDamageSound != "" && objectReal.specialDamageSound != null)
						{
							GC.audioHandler.Play(hittingAgent, objectReal.specialDamageSound);

							return false;
						}

						GC.audioHandler.Play(hittingAgent, "BulletHitObject");

						return false;
					}
				}
				else if (hitObject.CompareTag("AgentSprite"))
				{
					if (__instance.myMelee.fakeHitAgent)
						return false;

					Agent agent = hitObject.GetComponent<ObjectSprite>().agent;

					if (__instance.myMelee.recentFakeHitObjects.Contains(agent.go))
						return false;

					if (agent.hologram || agent.objectAgent)
						return false;

					bool flag = false;

					if (invItem.hitSoundType == "Cut")
					{
						if (agent.damagedAmount < 12)
							GC.audioHandler.Play(hittingAgent, "MeleeHitAgentCutSmall");
						else
							GC.audioHandler.Play(hittingAgent, "MeleeHitAgentCutLarge");

						flag = true;
					}

					if (agent.damagedAmount < 10)
					{
						if (!flag)
						{
							string hitSoundType = invItem.hitSoundType;

							if (!(hitSoundType == "Normal"))
							{
								if (!(hitSoundType == "WerewolfSlash"))
									GC.audioHandler.Play(hittingAgent, "MeleeHitAgentSmall");
								else
									GC.audioHandler.Play(hittingAgent, "WerewolfSlash");
							}
							else
								GC.audioHandler.Play(hittingAgent, "MeleeHitAgentSmall");
						}

						if (agent.damagedAmount > 0)
						{
							if (agent.inhuman || agent.mechFilled || agent.mechEmpty)
								GC.spawnerMain.SpawnParticleEffect("BloodHitYellow", agent.tr.position,
									__instance.myMelee.meleeContainerTr.eulerAngles.z - 90f);
							else
								GC.spawnerMain.SpawnParticleEffect("BloodHit", agent.tr.position, __instance.myMelee.meleeContainerTr.eulerAngles.z - 90f);
						}
						else
							GC.spawnerMain.SpawnParticleEffect("ObjectDestroyed", agent.tr.position, __instance.myMelee.meleeContainerTr.eulerAngles.z - 90f);

						if ((hittingAgent.isPlayer > 0 && hittingAgent.localPlayer) || (agent.isPlayer > 0 && agent.localPlayer))
						{
							GC.FreezeFrames(1);

							return false;
						}
					}
					else if (agent.damagedAmount < 15)
					{
						if (!flag)
						{
							string hitSoundType = invItem.hitSoundType;

							if (!(hitSoundType == "Normal"))
							{
								if (!(hitSoundType == "WerewolfSlash"))
									GC.audioHandler.Play(hittingAgent, "MeleeHitAgentLarge");
								else
									GC.audioHandler.Play(hittingAgent, "WerewolfSlash");
							}
							else
								GC.audioHandler.Play(hittingAgent, "MeleeHitAgentLarge");
						}

						if (agent.inhuman || agent.mechFilled || agent.mechEmpty)
							GC.spawnerMain.SpawnParticleEffect("BloodHitYellowMed", agent.tr.position, __instance.myMelee.meleeContainerTr.eulerAngles.z - 90f);
						else
							GC.spawnerMain.SpawnParticleEffect("BloodHitMed", agent.tr.position, __instance.myMelee.meleeContainerTr.eulerAngles.z - 90f);

						if ((hittingAgent.isPlayer > 0 && hittingAgent.localPlayer) || (agent.isPlayer > 0 && agent.localPlayer))
						{
							GC.FreezeFrames(2);

							return false;
						}
					}
					else
					{
						if (!flag)
						{
							string hitSoundType = invItem.hitSoundType;

							if (!(hitSoundType == "Normal"))
							{
								if (!(hitSoundType == "WerewolfSlash"))
									GC.audioHandler.Play(hittingAgent, "MeleeHitAgentLarge");
								else
									GC.audioHandler.Play(hittingAgent, "WerewolfSlash");
							}
							else
								GC.audioHandler.Play(hittingAgent, "MeleeHitAgentLarge");

							GC.audioHandler.Play(hittingAgent, "MeleeHitAgentLarge");
						}

						if (agent.inhuman || agent.mechFilled || agent.mechEmpty)
							GC.spawnerMain.SpawnParticleEffect("BloodHitYellowLarge", agent.tr.position,
								__instance.myMelee.meleeContainerTr.eulerAngles.z - 90f);
						else
							GC.spawnerMain.SpawnParticleEffect("BloodHitLarge", agent.tr.position, __instance.myMelee.meleeContainerTr.eulerAngles.z - 90f);

						if ((hittingAgent.isPlayer > 0 && hittingAgent.localPlayer) || (agent.isPlayer > 0 && agent.localPlayer))
						{
							GC.FreezeFrames(3);

							return false;
						}
					}
				}
				else if (hitObject.CompareTag("ItemImage"))
				{
					GC.audioHandler.Play(hittingAgent, "BulletHitWall");

					if (hittingAgent.isPlayer > 0 && hittingAgent.localPlayer)
					{
						GC.ScreenShake(0.1f, 80f, hittingAgent.tr.position, hittingAgent);
						GC.FreezeFrames(0);

						return false;
					}
				}
				else if (hitObject.CompareTag("MeleeHitbox"))
				{
					Agent agent2 = hitObject.GetComponent<MeleeColliderBox>().meleeHitbox.myMelee.agent;

					if (__instance.myMelee.recentFakeHitObjects.Contains(agent2.go))
						return false;

					if (invItem.hitSoundType == "Cut" || agent2.inventory.equippedWeapon.hitSoundType == "Cut")
					{
						GC.audioHandler.Play(hittingAgent, "MeleeHitMeleeBlade");

						return false;
					}

					GC.audioHandler.Play(hittingAgent, "MeleeHitMelee");
				}

				return false;
			}

			return true;
		}
	}
}
