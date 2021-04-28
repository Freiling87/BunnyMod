using RogueLibsCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

namespace BunnyMod.Content
{
	public class BMAgents
	{
		public static GameController GC => GameController.gameController;
		public static bool Prefix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) => BMHeader.MainInstance.PatchPrefix(type, methodName, patchType, patchMethodName, types);
		public static bool Postfix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) => BMHeader.MainInstance.PatchPostfix(type, methodName, patchType, patchMethodName, types);
		public static void BMLog(string logMessage) => BMHeader.Log(logMessage);

		public void Awake()
		{
			Agent_00();
			AgentInteractions_00();
			Relationships_00();
			StatusEffects_00();
		}

		#region Custom
		public static int ToolCost(Agent agent, int baseCost)
		{
			BMLog("ToolCost:");
			BMLog("\tagent = " + agent.name);
			BMLog("\tbaseCost = " + baseCost);

			if (agent.statusEffects.hasTrait(cTrait.TamperTantrum))
				return (baseCost / 2);

			if (agent.statusEffects.hasTrait(cTrait.TamperTantrum_2))
				return 0;

			return baseCost;
		}
		#endregion

		#region Agent
		public void Agent_00()
		{
			Prefix(typeof(Agent), "CanShakeDown", GetType(), "Agent_CanShakeDown", new Type[0] { });
			Postfix(typeof(Agent), "SetupAgentStats", GetType(), "Agent_SetupAgentStats", new Type[1] { typeof(string) });
			Postfix(typeof(Agent), "Start", GetType(), "Agent_Start", new Type[0] { });
		}
		public static bool Agent_CanShakeDown(ref bool __result) // Prefix
		{
			BMLog("Agent_CanShakeDown");

			if (BMTraits.IsPlayerTraitActive(cTrait.Warlord))
			{
				__result = true;

				return false;
			}

			return true;
		}
		public static void Agent_SetupAgentStats(string transformationType, Agent __instance) // Postfix
		{
			if (__instance.agentName == vAgent.ResistanceLeader)
			{
				if (BMTraits.IsPlayerTraitActive(cTrait.Reinforcements))
				{
					__instance.SetStrength(3);
					__instance.SetEndurance(2);
					__instance.inventory.startingHeadPiece = vArmorHead.HardHat;
				}
				else if (BMTraits.IsPlayerTraitActive(cTrait.Reinforcements_2))
				{
					__instance.SetStrength(3);
					__instance.SetEndurance(3);
					__instance.inventory.startingHeadPiece = vArmorHead.SoldierHelmet;
				}
			}
			else if (__instance.agentName == cAgent.ResistanceSoldier)
			{
				__instance.SetStrength(2);
				__instance.SetEndurance(1);
				__instance.SetAccuracy(3);
				__instance.SetSpeed(3);

				__instance.modMeleeSkill = 2;
				__instance.modGunSkill = 2;
				__instance.modToughness = 2;
				__instance.modVigilant = 1;

				__instance.statusEffects.AddTrait("RegenerateHealthWhenLow");

				__instance.AddJob("Recruit", 0);
				__instance.AddDesire("Supplies");
				__instance.AddDesire("Technology");
				__instance.AddDesire("Weapons");

				if (BMTraits.IsPlayerTraitActive(cTrait.Reinforcements))
				{
					__instance.inventory.startingHeadPiece = "HardHat";
					__instance.inventory.AddItemPlayerStart("Pistol", 0);
					__instance.inventory.AddItemPlayerStart("Knife", 100);
				}
				if (BMTraits.IsPlayerTraitActive(cTrait.Reinforcements_2))
				{
					__instance.inventory.startingHeadPiece = "SoldierHelmet";
					__instance.inventory.AddItemPlayerStart("MachineGun", 0);
					__instance.inventory.AddItemPlayerStart("Grenade", 5);
					__instance.inventory.AddItemPlayerStart("Knife", 100);
				}

				__instance.agentHitboxScript.legsColor = new Color32(66, 101, 61, byte.MaxValue);

				__instance.agentCategories.Clear();
				__instance.agentCategories.Add("Guns");
				__instance.agentCategories.Add("Melee");
				__instance.agentCategories.Add("Defense");
				__instance.setInitialCategories = true;
			}
		}
		public static void Agent_Start(Agent __instance) // Postfix
		{
			Agent_Remora remora = new Agent_Remora();
			Agent_Variables[__instance] = remora;
			remora.agentHost = __instance;
		}
		public static Dictionary<Agent, Agent_Remora> Agent_Variables = new Dictionary<Agent, Agent_Remora>();
		#endregion
		#region AgentInteractions
		public void AgentInteractions_00()
		{
			Prefix(typeof(AgentInteractions), "AddButton", GetType(), "AgentInteractions_AddButton", new Type[3] { typeof(string), typeof(int), typeof(string) });
		}
		public static void AgentInteractions_AddButton(string buttonName, int moneyCost, string extraCost, AgentInteractions __instance, ref Agent ___mostRecentInteractingAgent) // Prefix
		{
			if (extraCost.EndsWith("-30"))
				extraCost.Replace("-30", "-" + ToolCost(___mostRecentInteractingAgent, 30));
			else if (extraCost.EndsWith("-20"))
				extraCost.Replace("-20", "-" + ToolCost(___mostRecentInteractingAgent, 20));
		}
		#endregion
		#region Relationships
		public void Relationships_00()
		{
			Prefix(typeof(Relationships), "OwnCheck", GetType(), "Relationships_OwnCheck", new Type[7] { typeof(Agent), typeof(GameObject), typeof(int), typeof(string), typeof(bool), typeof(int), typeof(Fire) });
			Postfix(typeof(Relationships), "SetupRelationshipOriginal", GetType(), "Relationships_SetupRelationshipOriginal", new Type[1] { typeof(Agent) });
		}
		public static bool Relationships_OwnCheck(Agent otherAgent, GameObject affectedGameObject, int tagType, string ownCheckType, bool extraSprite, int strikes, Fire fire) // Prefix
		{
			// TODO: PoliceState sets all Objects NoStrikesIfDestroyed to false
			return true;
		}
		public static void Relationships_SetupRelationshipOriginal(Agent otherAgent, Relationships __instance, ref Agent ___agent) // Postfix
		{
			// This method sets how ___agent feels about otherAgent
			// Don't delete this comment, it gets confusing-er-than-shit

			if (GC.levelType == vLevelType.HomeBase)
			{
				__instance.SetRelInitial(otherAgent, vRelationship.Aligned);
				otherAgent.relationships.SetRelInitial(___agent, vRelationship.Aligned);
			}
			else
			{
				if (BMTraits.isPlayerInitialRelationshipTraitActive && ___agent.isPlayer != 0)
				{
					BMLog("Relationships_SetupRelationshipOriginal: ");
					BMLog("\tAgent = " + ___agent.name);
					BMLog("\totherAgent = " + otherAgent.name);
					BMLog("\tRelationship = '" + __instance.GetRel(otherAgent) + "'");

					if (__instance.GetRel(otherAgent) == vRelationship.Neutral)
					{
						int roll = Random.Range(0, 100);
						string newRel = vRelationship.Neutral;

						if ((___agent.statusEffects.hasTrait(cTrait.GenerallyUnpleasant) && roll <= 20) ||
							___agent.statusEffects.hasTrait(cTrait.GenerallyUnpleasant_2))
							newRel = vRelationship.Annoyed;
						else if (___agent.statusEffects.hasTrait(cTrait.Polarizing))
						{
							if (roll <= 50)
								newRel = vRelationship.Annoyed;
							else
								newRel = vRelationship.Friendly;
						}
						else if (___agent.statusEffects.hasTrait(cTrait.Polarizing_2))
						{
							if (roll <= 25)
								newRel = vRelationship.Hateful;
							else if (roll <= 50)
								newRel = vRelationship.Annoyed;
							else if (roll <= 67)
								newRel = vRelationship.Friendly;
							else if (roll <= 88)
								newRel = vRelationship.Loyal;
							else if (roll <= 100)
								newRel = vRelationship.Aligned;
						}

						roll = Random.Range(0, 100);

						if (___agent.statusEffects.hasTrait(cTrait.Domineering))
						{
							if (roll <= 5)
								newRel = vRelationship.Submissive;
						}
						else if (___agent.statusEffects.hasTrait(cTrait.Domineering_2))
						{
							if (roll <= 10)
								newRel = vRelationship.Submissive;
						}

						if (otherAgent.agentName == vAgent.ResistanceLeader)
							newRel = vRelationship.Aligned;

						if (newRel != vRelationship.Neutral)
						{
							__instance.SetRelInitial(otherAgent, newRel);
							otherAgent.relationships.SetRelInitial(___agent, newRel);

							if (newRel == vRelationship.Annoyed)
								otherAgent.relationships.SetStrikes(___agent, 2);
						}
					}
				}
			}
		}
		#endregion
		#region StatusEffects
		public void StatusEffects_00()
		{
			Postfix(typeof(StatusEffects), "AddTrait", GetType(), "StatusEffects_AddTrait", new Type[3] { typeof(string), typeof(bool), typeof(bool) });
			Postfix(typeof(StatusEffects), "BecomeHidden", GetType(), "StatusEffects_BecomeHidden", new Type[1] { typeof(ObjectReal) });
			Postfix(typeof(StatusEffects), "BecomeNotHidden", GetType(), "StatusEffects_BecomeNotHidden", new Type[0]);
			Prefix(typeof(StatusEffects), "ChangeHealth", GetType(), "StatusEffects_ChangeHealth", new Type[6] { typeof(float), typeof(PlayfieldObject), typeof(NetworkInstanceId), typeof(float), typeof(string), typeof(byte) });
			Postfix(typeof(StatusEffects), "RemoveTrait", GetType(), "StatusEffects_RemoveTrait", new Type[2] { typeof(string), typeof(bool) });
		}
		public static void StatusEffects_AddTrait(string traitName, bool isStarting, bool justRefresh, StatusEffects __instance) // Postfix
		{
			Agent agent = __instance.agent;

			if (traitName == cTrait.Fatass)
			{
				agent.SetEndurance(agent.enduranceStatMod + 1);
				agent.SetSpeed(agent.speedStatMod - 1);
			}
		}
		public static void StatusEffects_BecomeHidden(ObjectReal hiddenInObject, StatusEffects __instance) // Postfix
		{
			BMLog("StatusEffects_BecomeHidden");
			try { BMLog("\tObjectReal: " + hiddenInObject.name); }
			catch { BMLog("\tObjectReal: null"); }
			BMLog("\tAgent: " + __instance.agent.agentName);

			Agent agent = __instance.agent;

			if (!(hiddenInObject is null))
				if (BMTraits.IsPlayerTraitActive(cTrait.UnderdarkCitizen) && agent.isPlayer == 0 && hiddenInObject.objectName == vObject.Manhole)
					agent.statusEffects.BecomeNotHidden();

			if (hiddenInObject is Bathtub || hiddenInObject is Plant || hiddenInObject is PoolTable || hiddenInObject is TableBig)
				__instance.agent.agentCollider.enabled = false;
		}
		public static void StatusEffects_BecomeNotHidden(StatusEffects __instance)
		{
			__instance.agent.agentCollider.enabled = true;
		}
		public static bool StatusEffects_ChangeHealth(float healthNum, PlayfieldObject damagerObject, NetworkInstanceId cameFromClient, float clientFinalHealthNum, string damagerObjectName, byte extraVar, StatusEffects __instance, ref HealthBar ___healthBar) // Replacement
		{
			if ((__instance.agent.teleporting && !__instance.agent.skillPoints.justGainedLevel) ||
				(__instance.agent.ghost && __instance.agent.skillPoints.justGainedLevel) ||
				(__instance.agent.dead && __instance.agent.skillPoints.justGainedLevel && !__instance.agent.teleporting && !__instance.agent.suicided && !__instance.agent.finishedLevel && !__instance.agent.finishedLevelImmediate && !__instance.agent.finishedLevelImmediateMult && !__instance.agent.finishedLevelRealMult && !__instance.agent.oma.finishedLevel) ||
				((__instance.agent.finishedLevel || __instance.agent.finishedLevelImmediate || __instance.agent.finishedLevelImmediateMult || __instance.agent.finishedLevelRealMult || __instance.agent.oma.finishedLevel) && !__instance.agent.suicided && healthNum < 0f) ||
				(__instance.agent.butlerBot || __instance.agent.hologram || (__instance.agent.mechEmpty && healthNum < 0f)) || 
				(GC.cinematic && GC.loadLevel.LevelContainsMayor()))
				return false;
			
			float num = healthNum;
			bool flag = false;
			bool flag2 = false;

			if (healthNum < 0f)
			{
				float num2 = 0f;

				if (__instance.hasStatusEffect(vStatusEffect.Invincible) && !__instance.ignoreInvincible)
				{
					if (GC.debugMode)
						num2 = healthNum;

					healthNum = 0f;
				}

				if ((__instance.agent.oma.superSpecialAbility || __instance.hasTrait(vTrait.Juggernaut)) && __instance.agent.chargingForward && !__instance.agent.suiciding && !__instance.agent.suicided)
					healthNum = (float)((int)(healthNum * 0.4f));
				
				if (__instance.agent.oma.hidden && !__instance.GotStatusEffectDamage())
					__instance.BecomeNotHidden();
				
				if (damagerObject != null)
				{
					flag2 = true;
					__instance.ExitBox(true);

					if (!__instance.agent.oma.superSpecialAbility && !__instance.hasTrait(vTrait.Slinky))
						__instance.RemoveInvisibleLimited();
				}

				if (__instance.agent.isPlayer > 0)
					GC.playerControl.Vibrate(__instance.agent.isPlayer, Mathf.Clamp(-healthNum / 100f + 0.05f, 0f, 0.25f), Mathf.Clamp(-healthNum / 132f + 0.05f, 0f, 0.2f));
				
				if (__instance.agent.justHitByAgent2 != null && __instance.agent.justHitByAgent2 != __instance.agent)
				{
					Relationship relationship = __instance.agent.relationships.GetRelationship(__instance.agent.justHitByAgent2);
					Relationship relationship2 = __instance.agent.justHitByAgent2.relationships.GetRelationship(__instance.agent);

					if (relationship != null && relationship2 != null)
					{
						if (!__instance.agent.justHitByAgent2.objectAgent && __instance.agent.deathMethod != "Bite" && !__instance.agent.oma.mindControlled && !__instance.agent.justHitByAgent2.oma.mindControlled)
						{
							bool flag3 = false;
							bool flag4 = true;

							if (flag2)
							{
								if (damagerObject.playfieldObjectType == "Explosion" && ((Explosion)damagerObject).explosionType != vExplosion.Normal)
										flag4 = false;
								else if (damagerObject.playfieldObjectType == "Fire")
									flag4 = false;
							}
							if (flag4 && healthNum < 0f)
							{
								if (__instance.agent.isPlayer > 0 && __instance.agent.justHitByAgent2.isPlayer > 0 && !GC.pvp)
								{
									healthNum = 0f;
									flag = true;
								}
								else
								{
									if ((relationship.relTypeCode == relStatus.Aligned || relationship.relTypeCode == relStatus.Loyal) && __instance.agent.isPlayer == 0 && !__instance.agent.dead && healthNum != -200f)
									{
										healthNum = (float)Mathf.Clamp((int)(healthNum / 3f), -1000, -1);
										flag3 = true;
									}
									if ((relationship2.relTypeCode == relStatus.Aligned || relationship2.relTypeCode == relStatus.Loyal) && __instance.agent.justHitByAgent2.isPlayer == 0 && !__instance.agent.dead && healthNum != -200f && !flag3)
										healthNum = (float)Mathf.Clamp((int)(healthNum / 3f), -1000, -1);
								}
							}
						}

						if (!__instance.agent.suicided)
						{
							if (__instance.hasTrait("ChanceAttacksDoZeroDamage2"))
							{
								int myChance = __instance.agent.DetermineLuck(10, vTrait.UnCrits, true);

								if (GC.percentChance(myChance))
									healthNum = 0f;
							}
							else if (__instance.hasTrait(vTrait.UnCrits))
							{
								int myChance2 = __instance.agent.DetermineLuck(5, vTrait.UnCrits, true);

								if (GC.percentChance(myChance2))
									healthNum = 0f;
							}
						}

						if (GC.serverPlayer)
						{
							if (__instance.agent.hasStealingFromAgent && __instance.agent.justHitByAgent2 == __instance.agent.stealingFromAgent)
								__instance.agent.SetDefaultGoal(vGoal.WanderFar);
							
							if (healthNum < 0f && (relationship.relTypeCode == relStatus.Aligned || relationship.relTypeCode == relStatus.Submissive || (relationship.relTypeCode == relStatus.Loyal && __instance.agent.hasEmployer)) && !__instance.agent.oma.bodyGuarded && __instance.agent.deathMethod != "Poison")
							{
								relationship.damageDone += -(int)healthNum;

								if (!__instance.agent.justHitByAgent2.objectAgent)
								{
									if (__instance.agent.justHitByAgent2.mostRecentGoalCode == goalType.Battle)
										relationship.hitNumberOfTimesInCombat++;
									else
										relationship.hitNumberOfTimes++;
								}

								if ((relationship.damageDone >= 10 && (relationship.hitNumberOfTimes > 4 || relationship.hitNumberOfTimesInCombat > 12) && __instance.agent.isPlayer == 0 && (__instance.agent.isPlayer != 0 || __instance.agent.justHitByAgent2.isPlayer != 0)) || (__instance.agent.killForQuest != null && __instance.agent.isPlayer == 0 && (__instance.agent.justHitByAgent2.isPlayer != 0 || __instance.agent.justHitByAgent2.hasEmployer)) || (__instance.agent.isBigQuestObject && (__instance.agent.bigQuestType == "Gangbanger" || __instance.agent.bigQuestType == "GangbangerB" || __instance.agent.bigQuestType == "Vampire" || __instance.agent.bigQuestType == "Assassin" || __instance.agent.bigQuestType == "MechPilot")) || (__instance.agent.oma.didAsk && (__instance.agent.justHitByAgent2.isPlayer != 0 || __instance.agent.justHitByAgent2.hasEmployer)) || __instance.agent.justHitByAgent2.oma.mindControlled)
								{
									__instance.agent.relationships.SetRel(__instance.agent.justHitByAgent2, "Hateful");
									__instance.agent.relationships.SetRelHate(__instance.agent.justHitByAgent2, 5);

									if (__instance.agent.justHitByAgent2.oma.mindControlled)
									{
										__instance.agent.justHitByAgent2.relationships.SetRel(__instance.agent, "Hateful");
										__instance.agent.justHitByAgent2.relationships.SetRelHate(__instance.agent, 5);
									}

									if (__instance.agent.justHitByAgent2.isPlayer > 0)
										__instance.agent.justHitByAgent2.relationships.SetRel(__instance.agent, "Hateful");
									
									if (!__instance.agent.isMayor)
									{
										List<Agent> agentList = GC.agentList;

										for (int i = 0; i < agentList.Count; i++)
										{
											Agent agent = agentList[i];

											if (Vector2.Distance(agent.curPosition, __instance.agent.curPosition) < agent.LOSRange / __instance.agent.hardToSeeFromDistance)
											{
												relStatus relCode = agent.relationships.GetRelCode(__instance.agent);

												if (relCode == relStatus.Aligned && agent.movement.HasLOSObject360(__instance.agent))
												{
													agent.relationships.SetRel(__instance.agent.justHitByAgent2, "Hateful");
													agent.relationships.SetRelHate(__instance.agent.justHitByAgent2, 5);

													if (__instance.agent.justHitByAgent2.isPlayer > 0)
														__instance.agent.justHitByAgent2.relationships.SetRel(agent, "Hateful");
												}

												if (relCode == relStatus.Loyal)
												{
													relStatus relCode2 = agent.relationships.GetRelCode(__instance.agent.justHitByAgent2);

													if (relCode2 != relStatus.Aligned && relCode2 != relStatus.Loyal && agent.movement.HasLOSObject360(__instance.agent))
													{
														agent.relationships.SetRel(__instance.agent.justHitByAgent2, "Hateful");
														agent.relationships.SetRelHate(__instance.agent.justHitByAgent2, 5);
													
														if (__instance.agent.justHitByAgent2.isPlayer > 0)
															__instance.agent.justHitByAgent2.relationships.SetRel(agent, "Hateful");
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}

				if (healthNum != -200f && healthNum != -2000f && !flag)
				{
					if (__instance.agent.isPlayer > 0 && __instance.agent.localPlayer)
					{
						float f = healthNum;

						if (num2 != 0f)
							f = num2;
						
						GC.alienFX.PlayerHurt(__instance.agent);
						
						if (__instance.agent.critted && (!__instance.agent.dead || __instance.agent.fakeDead))
							GC.spawnerMain.SpawnStatusText(__instance.agent, "PlayerHealthDownCrit", Mathf.Abs(f).ToString(), cameFromClient);
						else if (!__instance.agent.dead || __instance.agent.fakeDead)
							GC.spawnerMain.SpawnStatusText(__instance.agent, "PlayerHealthDown", Mathf.Abs(f).ToString(), cameFromClient);
					}
					else
					{
						float f2 = healthNum;

						if (clientFinalHealthNum != -999f)
							f2 = clientFinalHealthNum;
						
						if (__instance.agent.critted && (!__instance.agent.dead || __instance.agent.fakeDead))
							GC.spawnerMain.SpawnStatusText(__instance.agent, "NPCHealthDownCrit", Mathf.Abs(f2).ToString(), cameFromClient);
						else if (!__instance.agent.dead || __instance.agent.fakeDead)
							GC.spawnerMain.SpawnStatusText(__instance.agent, "NPCHealthDown", Mathf.Abs(f2).ToString(), cameFromClient);
					}
				}

				__instance.agent.critted = false;

				if (__instance.agent.health + healthNum > 0f || __instance.agent.dead)
				{
					bool flag5 = false;

					if (flag2 && damagerObject.bulletType == bulletStatus.GhostBlaster)
					{
						__instance.agent.objectSprite.Flash("GhostBlaster");

						flag5 = true;
					}

					if (!flag5)
					{
						__instance.agent.objectSprite.Flash();

						if (GC.multiplayerMode && flag2 && damagerObjectName != "Bullet" && damagerObjectName != "Melee" && damagerObjectName != "Explosion")
							__instance.agent.objectMult.AgentFlash();
					}
				}

				if (GC.levelType == "Tutorial" && __instance.agent.isPlayer > 0)
				{
					GC.tutorial.LoseHealth();
				
					if (__instance.agent.health < 0f)
						__instance.agent.health = 1f;
				}

				if (__instance.agent.isPlayer > 0)
					__instance.timeSinceLastDamaged = 0f;
				
				if (__instance.agent.arenaBattler && !__instance.agent.arenaBattleStarted && GC.serverPlayer)
				{
					__instance.agent.arenaBattleStarted = true;

					for (int j = 0; j < __instance.agent.gc.objectRealList.Count; j++)
					{
						ObjectReal objectReal = __instance.agent.gc.objectRealList[j];
					
						if (objectReal.startingChunk == __instance.agent.startingChunk && objectReal.objectName == "EventTriggerFloor")
						{
							EventTriggerFloor eventTriggerFloor = (EventTriggerFloor)objectReal;
						
							if (eventTriggerFloor.triggerState == "Initial")
								eventTriggerFloor.triggerState = "FightsOver";
						}
					}
				}

				if (__instance.agent.challengedToFight > 0 && __instance.agent.justHitByAgent2 != null)
				{
					if (__instance.agent.justHitByAgent2.isPlayer != 0)
					{
						if (__instance.agent.challengedToFight == 1 || (__instance.agent.challengedToFight == 2 && (damagerObjectName == "Bullet" || damagerObjectName == "Explosion" || damagerObjectName == "Fire")))
						{
							__instance.agent.challengedToFight = 0;
							__instance.agent.challengedToFightAgentID = 0;
							__instance.agent.challengedToFightAgent = null;
							__instance.agent.SetFollowing(null);
							__instance.agent.SayDialogue("NotFair", true);
						}
					}
					else
					{
						relStatus relCode3 = __instance.agent.justHitByAgent2.relationships.GetRelCode(__instance.agent.challengedToFightAgent);

						if (relCode3 == relStatus.Aligned || relCode3 == relStatus.Loyal)
						{
							__instance.agent.challengedToFight = 0;
							__instance.agent.challengedToFightAgentID = 0;
							__instance.agent.challengedToFightAgent = null;
							__instance.agent.SetFollowing(null);
							__instance.agent.SayDialogue("NotFair", true);
						}
					}
				}
				if (__instance.agent.localPlayer && __instance.agent.isPlayer != 0)
				{
					if (__instance.agent.inventory.HasItem("CourierPackage") && !__instance.hasTrait("NoBrokenPackages") && (!__instance.agent.oma.superSpecialAbility || !(__instance.agent.agentName == "Courier")) && !__instance.GotNonHitDamage() && __instance.agent.deathMethod != "FellInHole")
					{
						__instance.agent.courierPackageDamage -= healthNum;

						if (__instance.agent.courierPackageDamage >= 20f)
						{
							__instance.agent.inventory.DestroyItem(__instance.agent.inventory.FindItem("CourierPackage"));
							InvItem invItem = new InvItem();
							invItem.invItemName = "CourierPackageBroken";
							invItem.SetupDetails(false);
							GC.audioHandler.Play(__instance.agent, "CourierPackageBreak");
							GC.spawnerMain.SpawnStatusText(__instance.agent, "OutOfAmmo", "CourierPackage", "Item");
							__instance.agent.inventory.DontPlayPickupSounds(true);
							__instance.agent.inventory.AddItemOrDrop(invItem);
							__instance.agent.inventory.DontPlayPickupSounds(false);
						}
					}
				
					if (__instance.agent.mindControlling && !__instance.GotNonHitDamage())
						__instance.agent.relationships.StopAgentsUnderMindControl();
				}
			}
			else if (healthNum > 0f)
			{
				if (__instance.zombieResurrecting || healthNum == 1000f)
					flag = true;
				
				if (!flag)
				{
					if (healthNum == __instance.agent.healthMax && !__instance.agent.finishedLevel)
					{
						if (__instance.agent.skillPoints.justGainedLevel)
							GC.spawnerMain.SpawnStatusText(__instance.agent, "HealthUpSlower", "FullHealth", "StatusEffect", cameFromClient);
						else
						{
							GC.alienFX.GainHealth(__instance.agent);
							GC.spawnerMain.SpawnStatusText(__instance.agent, "HealthUp", "FullHealth", "StatusEffect", cameFromClient);
						}
					}
					else if (!__instance.agent.finishedLevel)
					{
						GC.alienFX.GainHealth(__instance.agent);
						GC.spawnerMain.SpawnStatusText(__instance.agent, "HealthUp", healthNum.ToString(), cameFromClient);
					}
				}
			}

			if (healthNum <= 0f && (!__instance.GotStatusEffectDamage() || __instance.agent.health + healthNum <= 0f))
			{
				if (__instance.agent.isPlayer == 0 && __instance.agent.interactingAgent != null)
					__instance.agent.StopInteraction();
				
				if (__instance.hasStatusEffect(vStatusEffect.Dizzy))
					__instance.RemoveStatusEffect(vStatusEffect.Dizzy);
			
				if (__instance.hasStatusEffect(vStatusEffect.Electrocuted))
					__instance.RemoveStatusEffect(vStatusEffect.Electrocuted);
			}

			if (clientFinalHealthNum != -999f)
			{
				if (clientFinalHealthNum > 0f && __instance.agent.health < 0f)
					__instance.agent.health = 0f;
				
				__instance.agent.health += clientFinalHealthNum;
			}
			else
				__instance.agent.health += healthNum;
			
			bool flag6 = false;
			
			if (__instance.agent.oma.bodyGuarded && __instance.agent.employer != null && __instance.agent.employer.oma.superSpecialAbility && __instance.agent.employer.agentName == vAgent.Bouncer)
				flag6 = true;
			
			if (__instance.agent.oma.bodyGuarded && __instance.agent.bodyguardedWantsRevenge && (__instance.agent.health <= __instance.agent.healthMax * 0.3f || flag6) && __instance.agent.health > 0f && __instance.agent.isPlayer == 0 && GC.serverPlayer && __instance.agent.justHitByAgent2 != null && (__instance.agent.justHitByAgent2.isPlayer != 0 || __instance.agent.justHitByAgent2.hasEmployer))
				__instance.agent.BodyguardedSorryMutiny();
			
			if (__instance.agent.health <= __instance.agent.healthMax * 0.4f && __instance.agent.health > 0f && __instance.agent.isPlayer == 0 && GC.serverPlayer && __instance.agent.CanShakeDown() && __instance.agent.justHitByAgent2 != null && (__instance.agent.justHitByAgent2.isPlayer != 0 || __instance.agent.justHitByAgent2.hasEmployer))
			{
				int k = 0;

				while (k < GC.playerAgentList.Count)
				{
					Agent agent2 = GC.playerAgentList[k];
					bool flag7 = false;

					if (__instance.agent.justHitByAgent2.isPlayer != 0 && __instance.agent.justHitByAgent2 != agent2 && !__instance.agent.justHitByAgent2.statusEffects.hasTrait("Shakedowner") && !__instance.agent.justHitByAgent2.statusEffects.hasTrait("Shakedowner2") && Vector2.Distance(agent2.tr.position, __instance.agent.justHitByAgent2.tr.position) < 13.44f)
						flag7 = true;
					else if (__instance.agent.justHitByAgent2.isPlayer == 0 && !__instance.agent.justHitByAgent2.hasEmployer)
					{
						relStatus relCode4 = __instance.agent.justHitByAgent2.relationships.GetRelCode(agent2);
					
						if ((relCode4 == relStatus.Aligned || relCode4 == relStatus.Loyal) && Vector2.Distance(agent2.tr.position, __instance.agent.justHitByAgent2.tr.position) < 13.44f)
							flag7 = true;
					}
					if ((agent2 == __instance.agent.justHitByAgent2 || agent2 == __instance.agent.justHitByAgent2.employer || flag7) && (agent2.statusEffects.hasTrait("Shakedowner") || agent2.statusEffects.hasTrait("Shakedowner2")))
					{
						__instance.agent.relationships.SetRel(agent2, "Submissive");
						__instance.agent.agentInteractions.BecomeSubmissiveWithAlliesAllInChunk(__instance.agent, agent2);
						int num3 = 0;

						for (int l = 0; l < GC.agentList.Count; l++)
							if (GC.agentList[l].oma.shookDown)
								num3++;
						
						int shakedownAmount = agent2.objectMult.FindShakedownAmount(num3);
						agent2.objectMult.AddShakedownPerson(__instance.agent, shakedownAmount, true);
						__instance.agent.oma.shookDown = true;
						__instance.agent.shookDownAgent = agent2;

						if (num3 >= 3)
						{
							__instance.agent.SayDialogue("ThreatenedShakedownLowCash", true);
						
							break;
						}

						__instance.agent.SayDialogue("SubmitShakedowner", true);

						break;
					}
					else
						k++;
				}
			}

			bool flag8 = false;

			if (cameFromClient != NetworkInstanceId.Invalid && healthNum < 0f && __instance.agent.isPlayer == 0)
			{
				Agent component = NetworkServer.FindLocalObject(cameFromClient).GetComponent<Agent>();
				__instance.agent.justHitByAgent3 = true;

				if (!component.invisible)
					__instance.agent.relationships.AddRelHate(component, Mathf.Clamp((int)Mathf.Abs(healthNum), 5, 200));

				__instance.agent.justHitByAgent3 = false;

				if (extraVar == 1)
					flag8 = true;
			}

			if (__instance.agent.deathMethod == "Cyanide") { } // Sic
			
			if (!flag || healthNum != 0f)
				__instance.agent.objectMult.ChangeHealth(num, damagerObject, cameFromClient, healthNum);
			
			__instance.agent.SetBrainActive(true);
			
			if (__instance.hasTrait(vTrait.ImOuttie) || __instance.hasTrait(vTrait.ImOuttie_2))
				__instance.agent.FindSpeed();
			
			if (__instance.agent.health > __instance.agent.healthMax && !__instance.hasTrait("Supercharge"))
				__instance.agent.health = __instance.agent.healthMax;
			
			if (__instance.agent.health <= __instance.agent.healthMax && __instance.hasTrait("Supercharge"))
				__instance.RemoveTrait("Supercharge");
			
			if (__instance.agent.isPlayer > 0 && __instance.agent.localPlayer)
				___healthBar.StartChange();
			
			if (__instance.agent.mechEmpty)
			{
				try
				{
					GC.sessionData.mechHealth[__instance.agent.mechTiedToPlayer] = (int)__instance.agent.health;
				}
				catch { }
			}

			if ((__instance.agent.isPlayer == 0 || !__instance.agent.localPlayer) && __instance.agent.agentHealthBar != null)
				__instance.agent.agentHealthBar.ChangeHealth();
			
			if (__instance.agent.isPlayer > 0 && __instance.agent.localPlayer && (!__instance.agent.dead || __instance.agent.fakeDead) && healthNum != -200f && healthNum < 0f)
				GC.stats.AddToStat(__instance.agent, "DamageTaken", -(int)healthNum);
			
			if (healthNum < 0f)
				__instance.UseQuickEscapeTeleporter(false);
			
			bool flag9 = true;
			bool flag10 = false;

			if (GC.multiplayerMode)
			{
				if (__instance.agent.isPlayer != 0 && !__instance.agent.localPlayer)
					flag9 = false;
				
				if (!GC.serverPlayer && __instance.agent.isPlayer == 0)
				{
					flag9 = true;
					flag10 = true;
				}
			}

			bool flag11 = false;
			bool flag12 = false;
			
			if (flag9)
			{
				if (__instance.agent.dead && __instance.agent.fakeDead)
					flag11 = false;
				else if (__instance.agent.dead)
					flag11 = true;
				
				if (__instance.agent.ghost && __instance.agent.isPlayer > 0 && __instance.agent.health > -40f)
					flag11 = true;
			}

			if (__instance.agent.sleepKnockedOut && __instance.agent.agentHitboxScript.wholeBodyAnimType == "Sleep")
				__instance.agent.agentHitboxScript.PlayWholeBodyAnim("Dead");
			
			if (__instance.agent.health <= 0f && !flag11 && flag9 && !__instance.agent.mechEmpty && __instance.agent.agentName != "MechEmpty")
			{
				if (__instance.agent.mechFilled && !__instance.agent.suiciding && !__instance.agent.suicided)
				{
					if (!__instance.agent.fellInHole && !__instance.clientMechTransforming)
					{
						if (GC.serverPlayer)
						{
							__instance.MechTransformBackStart(true);
							__instance.agent.objectMult.SpecialAbility("MechTransformBackDead", null);
			
							return false;
						}

						__instance.agent.objectMult.SpecialAbility("MechTransformBackStartDead", null);
						__instance.clientMechTransforming = true;
					}

					return false;
				}

				bool flag13 = false;

				if (flag2 && damagerObject.isBullet && damagerObject.playfieldObjectBullet.rubber)
					flag13 = true;
				
				if (extraVar == 2 || flag13)
				{
					__instance.agent.knockedOut = true;
					__instance.agent.knockedOutLocal = true;
					flag13 = true;
				}
				
				if (__instance.agent.oma.bodyGuarded)
				{
					__instance.agent.zombified = false;
					__instance.agent.zombieWhenDead = false;
				
					if (!__instance.hasStatusEffect(vStatusEffect.Resurrection) && !__instance.agent.fellInHoleWhileDead)
						GC.quests.AddBigQuestPoints(__instance.agent.employer, __instance.agent, "BodyguardKill");
				}

				if ((__instance.agent.KnockedOut() || __instance.agent.arrested || (__instance.agent.FellInHole() && !__instance.agent.oma.bodyGuarded)) && __instance.hasStatusEffect(vStatusEffect.Resurrection) && __instance.agent.isPlayer == 0)
				{
					if (__instance.agent.FellInHole())
						__instance.RemoveStatusEffect(vStatusEffect.Resurrection, false);
					else
						__instance.RemoveStatusEffect(vStatusEffect.Resurrection);
				}

				if (__instance.hasStatusEffect(vStatusEffect.Resurrection) && __instance.agent.possessing)
					for (int m = 0; m < __instance.StatusEffectList.Count; m++)
						if (__instance.StatusEffectList[m].statusEffectName == vStatusEffect.Resurrection && !__instance.StatusEffectList[m].infiniteTime)
						{
							__instance.RemoveStatusEffect(vStatusEffect.Resurrection);

							break;
						}

				if ((__instance.hasStatusEffect(vStatusEffect.Resurrection) || __instance.hasStatusEffect("ExtraLife") || __instance.agent.possessing) && !GC.mainGUI.menuGUI.endedGame && !flag10 && !__instance.agent.suicided)
					__instance.agent.resurrect = true;
				
				if (!__instance.agent.resurrect)
				{
					if (!__instance.agent.zombified && __instance.agent.zombieWhenDead && __instance.agent.isPlayer == 0 && !__instance.agent.inhuman && __instance.agent.agentName != "Alien" && !__instance.agent.FellInHole())
					{
						if (__instance.agent.justHitByAgent2 != null && __instance.agent.justHitByAgent2.zombified)
							__instance.agent.zombifiedByAgent = __instance.agent.justHitByAgent2;
						
						__instance.agent.zombified = true;
						__instance.agent.resurrect = true;
						
						if (__instance.agent.nonQuestObjectMarker != null)
						{
							bool flag14 = true;
						
							for (int n = 0; n < GC.playerAgentList.Count; n++)
								if (GC.playerAgentList[n].agentName == "Zombie")
									flag14 = false;
							
							if (flag14)
								Object.Destroy(__instance.agent.nonQuestObjectMarker.gameObject);
						}
					}

					if (__instance.agent.zombified && __instance.agent.isPlayer != 0 && !__instance.agent.suicided)
						if (!__instance.agent.resurrect && __instance.agent.justHitByAgent2 != null && !__instance.agent.zombified && __instance.agent.justHitByAgent2.statusEffects.hasTrait("ResurrectionAllies") && !__instance.agent.justHitByAgent2.zombified && GC.percentChance(30))
							__instance.agent.resurrect = true;
				}
				else if (__instance.agent.zombified && __instance.agent.employer != null)
					__instance.agent.zombifiedByAgent = __instance.agent.employer;
				
				if (__instance.agent.transforming && __instance.agent.isPlayer > 0 && !flag10 && !__instance.agent.mechFilled && !__instance.hasSpecialAbility("MechTransform") && !__instance.agent.wasTransforming)
				{
					Debug.Log("Remove Werewolf (Dead)");
				
					if (GC.serverPlayer)
					{
						__instance.RemoveStatusEffect("WerewolfEffect");
						__instance.agent.objectMult.SpecialAbility("WerewolfTransformBackDead", null);
					}
					else
						__instance.agent.objectMult.SpecialAbility("WerewolfTransformBackStartDead", null);
				}

				if (!flag2 && !__instance.agent.dead && !__instance.agent.KnockedOut() && __instance.agent.deathMethod == "" && __instance.agent.deathKiller == "")
				{
					__instance.agent.deathMethod = "";
					__instance.agent.deathKiller = "";
				}
				
				__instance.agent.movement.StopChargingForward();
				
				if (__instance.agent.isPlayer > 0)
					__instance.RechargeSpecialAbilityImmediate();
				
				if (__instance.agent.KnockedOut())
				{
					if (!GC.audioHandler.soundsPlaying.Contains("ChloroformAgent") && !__instance.agent.arrested)
					{
						if (flag13)
							GC.audioHandler.Play(__instance.agent, vAudioClip.AgentDie);
						else
							GC.audioHandler.Play(__instance.agent, vAudioClip.AgentKnockOut);
					}
					if (!__instance.agent.arrested && !flag10 && !__instance.agent.FellInHole() && !__instance.agent.disappeared)
						GC.spawnerMain.SpawnStateIndicator(__instance.agent, "DizzyStars");
					
					__instance.agent.health = 0f;
					
					if (__instance.agent.sleeping)
						__instance.agent.sleepKnockedOut = true;
					
					if ((GC.coopMode || GC.fourPlayerMode) && __instance.agent.isPlayer > 0)
					{
						for (int num8 = 0; num8 < GC.playerAgentList.Count; num8++)
						{
							Agent agent7 = GC.playerAgentList[num8];

							if (!(agent7 != __instance.agent) || (agent7.dead && !agent7.resurrect) || agent7.ghost || !agent7.fakeDead) { } // Sic
						}
					}
				}
				else
				{
					if (__instance.agent.agentName == vAgent.Robot)
						GC.audioHandler.Play(__instance.agent, vAudioClip.RobotDeath);
					else if (!__instance.agent.FellInHole() || __instance.agent.oma.bodyGuarded)
						GC.audioHandler.Play(__instance.agent, vAudioClip.AgentDie);
					
					if (__instance.agent.isPlayer != 0)
					{
						GC.audioHandler.Play(__instance.agent, vAudioClip.PlayerDeath);
						__instance.playedPlayerDeath = true;
					}
				}

				if (GC.levelType == vLevelType.Tutorial)
					GC.tutorial.KillAgent(__instance.agent);
				
				if (cameFromClient == NetworkInstanceId.Invalid && flag2 && damagerObject.CompareTag("Bullet") && !damagerObject.nonLethal && __instance.agent.tickEndDamage == 0)
				{
					__instance.agent.rb.velocity = Vector2.zero;
					__instance.agent.rb.angularVelocity = 0f;
					__instance.agent.movement.KnockBackBullet(damagerObject.go, Mathf.Abs(healthNum) * 30f, false, damagerObject);
				}

				__instance.ExitBox(true);
				__instance.RemoveInvisibleLimited();
				bool flag15 = false;
				
				if (__instance.agent.justHitByAgent2 && __instance.agent.justHitByAgent2.agentName == "Custom")
				{
					__instance.agent.deathKiller = __instance.agent.justHitByAgent2.agentRealName;
					flag15 = true;
				}
				
				if (!flag15 && __instance.agent.killedByAgentIndirect != null && __instance.agent.killedByAgentIndirect.agentName == "Custom")
					__instance.agent.deathKiller = __instance.agent.killedByAgentIndirect.agentRealName;
				
				Agent agent8 = null;
				
				if (__instance.agent.oma.mindControlled)
					agent8 = __instance.agent.mindControlAgent;
				
				if ((!GC.coopMode && !GC.fourPlayerMode && !GC.multiplayerMode) || __instance.agent.isPlayer == 0 || __instance.agent.resurrect)
				{
					if (!GC.serverPlayer)
						__instance.agent.killedOnClient = true;
				
					__instance.SetupDeath(damagerObject, flag10);
					__instance.agent.pathing = 0;
					__instance.agent.movement.PathStop();
					__instance.agent.SetBrainActive(false);
					__instance.agent.brainUpdate.slowAIWait = 0;
					
					if (GC.serverPlayer)
					{
						if (__instance.agent.brain.Goals.Count > 0)
							__instance.agent.brain.RemoveAllSubgoals(__instance.agent.brain.Goals[0]);
					
						if (__instance.agent.brain.Goals.Count > 0)
							__instance.agent.brain.Goals[0].Terminate();
						
						__instance.agent.brain.Goals.Clear();
					}

					__instance.agent.inCombat = false;
					__instance.agent.inFleeCombat = false;
					
					if (__instance.agent.isPlayer > 0)
						__instance.agent.interactionHelper.gameObject.SetActive(false);
					
					if (!__instance.agent.KnockedOut())
						GC.spawnerMain.SpawnStateIndicator(__instance.agent, "NoAnim");
					
					__instance.agent.relationships.SetAllRel2("Dead");
					
					if (!__instance.agent.oma.bodyGuarded)
					{
						__instance.agent.job = "";
						__instance.agent.jobCode = jobType.None;
						__instance.agent.jobBig = "";
						__instance.agent.jobBigCode = jobType.None;
						__instance.agent.SetFollowing(null);
					
						if (!__instance.agent.resurrect)
							__instance.agent.SetEmployerNullLate();
					}

					__instance.agent.SetTraversable("");
					
					if (__instance.agent.lightReal != null)
						__instance.agent.lightReal.tr.localPosition = new Vector3(0f, -0.2f, 0f);
					
					bool flag16 = true;
					
					if (__instance.agent.justHitByAgent2 || __instance.agent.killedByAgentIndirect)
					{
						if (__instance.agent.justHitByAgent2)
							__instance.agent.killedByAgent = __instance.agent.justHitByAgent2;
						else if (__instance.agent.killedByAgentIndirect)
							__instance.agent.killedByAgent = __instance.agent.killedByAgentIndirect;
						
						Agent playerGettingPoints = null;
						
						if (__instance.agent.killedByAgent.isPlayer > 0)
							playerGettingPoints = __instance.agent.killedByAgent;
						else if (__instance.agent.killedByAgent.employer != null)
						{
							if (__instance.agent.killedByAgent.employer.isPlayer > 0)
								playerGettingPoints = __instance.agent.killedByAgent.employer;
						}
						else if (__instance.agent.recentMindControlAgent != null)
							agent8 = __instance.agent.recentMindControlAgent;
						
						flag16 = __instance.IsInnocent(playerGettingPoints);
					}

					if (!flag10)
					{
						bool flag17 = false;

						if (agent8 != null)
						{
							GC.stats.AddToStat(agent8, "IndirectlyKilled", 1);
						
							if (__instance.AgentIsRival(agent8))
								agent8.skillPoints.AddPoints("IndirectlyKillRival");
							else if (flag16)
								agent8.skillPoints.AddPoints("IndirectlyKillInnocent");
							else
								agent8.skillPoints.AddPoints("IndirectlyKill");
						}
						else if (__instance.agent.killedByAgentIndirect != null && __instance.agent.killedByAgentIndirect != __instance.agent)
						{
							if (__instance.agent.killedByAgentIndirect.zombified && __instance.agent.agentName == "Zombie")
								flag17 = true;
							
							__instance.agent.killedByAgent = __instance.agent.killedByAgentIndirect;
							
							if (__instance.agent.isPlayer == 0 && __instance.agent.killedByAgent.isPlayer != 0 && !flag17)
							{
								GC.stats.AddToStat(__instance.agent.killedByAgent, "IndirectlyKilled", 1);
							
								if (flag16)
									GC.stats.AddToStat(__instance.agent.killedByAgent, "InnocentsKilled", 1);
								
								__instance.agent.relationships.AddToKillStat(__instance.agent.killedByAgent);
							}

							if (__instance.agent.isPlayer == 0 && (!__instance.agent.resurrect || __instance.agent.zombified) && !flag17 && !__instance.agent.playersReceivedPoints.Contains(__instance.agent.killedByAgent) && __instance.agent.relationships.GetRel(__instance.agent.killedByAgent) != "Aligned")
							{
								__instance.agent.playersReceivedPoints.Add(__instance.agent.killedByAgent);

								if (__instance.AgentIsRival(__instance.agent.killedByAgent))
									__instance.agent.killedByAgent.skillPoints.AddPoints("IndirectlyKillRival");
								else if (flag16)
									__instance.agent.killedByAgent.skillPoints.AddPoints("IndirectlyKillInnocent");
								else
									__instance.agent.killedByAgent.skillPoints.AddPoints("IndirectlyKill");
							}
						}
						else if (__instance.agent.justHitByAgent2 != null && __instance.agent.justHitByAgent2 != __instance.agent)
						{
							__instance.agent.killedByAgent = __instance.agent.justHitByAgent2;

							if (__instance.agent.killedByAgent.zombified && __instance.agent.agentName == "Zombie")
								flag17 = true;
							
							if (__instance.agent.isPlayer == 0 && __instance.agent.killedByAgent.isPlayer != 0 && !flag17 && !__instance.agent.arrested)
							{
								if (__instance.agent.KnockedOut())
									GC.stats.AddToStat(__instance.agent.killedByAgent, "KnockedOut", 1);
								else
								{
									GC.stats.AddToStat(__instance.agent.killedByAgent, "Killed", 1);
							
									if (flag16)
										GC.stats.AddToStat(__instance.agent.killedByAgent, "InnocentsKilled", 1);
									
									__instance.agent.relationships.AddToKillStat(__instance.agent.killedByAgent);
								}
							}

							if (__instance.agent.isPlayer == 0 && (!__instance.agent.resurrect || __instance.agent.zombified) && !flag17 && !__instance.agent.playersReceivedPoints.Contains(__instance.agent.killedByAgent))
							{
								__instance.agent.playersReceivedPoints.Add(__instance.agent.killedByAgent);

								if (__instance.agent.KnockedOut())
								{
									if (__instance.agent.deathMethod == "Arrested")
									{
										if (flag16)
										{
											__instance.agent.killedByAgent.skillPoints.AddPoints("ArrestedPointsInnocent");
											GC.quests.AddBigQuestPoints(__instance.agent.killedByAgent, "ArrestInnocent");
											GC.quests.AddBigQuestPoints(__instance.agent.killedByAgent, __instance.agent, "Neutralize");
										}
										else
										{
											if (__instance.agent.relationships.GetRel(__instance.agent.killedByAgent) != "Aligned")
												__instance.agent.killedByAgent.skillPoints.AddPoints("ArrestedPoints");
											
											GC.quests.AddBigQuestPoints(__instance.agent.killedByAgent, "ArrestGuilty");
											GC.quests.AddBigQuestPoints(__instance.agent.killedByAgent, __instance.agent, "Neutralize");
										}
									}
									else if (__instance.AgentIsRival(__instance.agent.killedByAgent))
									{
										if (__instance.agent.relationships.GetRel(__instance.agent.killedByAgent) != "Aligned")
											__instance.agent.killedByAgent.skillPoints.AddPoints("KnockOutPointsRival");
										
										GC.quests.AddBigQuestPoints(__instance.agent.killedByAgent, "KillGuilty");
										GC.quests.AddBigQuestPoints(__instance.agent.killedByAgent, __instance.agent, "Neutralize");
									}
									else if (flag16)
									{
										if (__instance.agent.relationships.GetRel(__instance.agent.killedByAgent) != "Aligned")
											__instance.agent.killedByAgent.skillPoints.AddPoints("KnockOutPointsInnocent");
										
										GC.quests.AddBigQuestPoints(__instance.agent.killedByAgent, "KillInnocent");
										GC.quests.AddBigQuestPoints(__instance.agent.killedByAgent, __instance.agent, "Neutralize");
									}
									else
									{
										if (__instance.agent.relationships.GetRel(__instance.agent.killedByAgent) != "Aligned")
											__instance.agent.killedByAgent.skillPoints.AddPoints("KnockOutPoints");
										
										GC.quests.AddBigQuestPoints(__instance.agent.killedByAgent, "KillGuilty");
										GC.quests.AddBigQuestPoints(__instance.agent.killedByAgent, __instance.agent, "Neutralize");
									}
								}
								else
								{
									_ = __instance.agent.deathMethod == "Bite"; // Sic

									if (__instance.AgentIsRival(__instance.agent.killedByAgent))
									{
										if (__instance.agent.relationships.GetRel(__instance.agent.killedByAgent) != "Aligned")
											__instance.agent.killedByAgent.skillPoints.AddPoints("KillPointsRival");
										
										GC.quests.AddBigQuestPoints(__instance.agent.killedByAgent, "KillGuilty");
										GC.quests.AddBigQuestPoints(__instance.agent.killedByAgent, __instance.agent, "Dead");
										GC.quests.AddBigQuestPoints(__instance.agent.killedByAgent, __instance.agent, "Neutralize");
										GC.quests.AddBigQuestPoints(__instance.agent.killedByAgent, __instance.agent, "BloodlustKill");
									}
									else if (flag16)
									{
										if (__instance.agent.relationships.GetRel(__instance.agent.killedByAgent) != "Aligned")
											__instance.agent.killedByAgent.skillPoints.AddPoints("KillPointsInnocent");
										
										GC.quests.AddBigQuestPoints(__instance.agent.killedByAgent, "KillInnocent");
										GC.quests.AddBigQuestPoints(__instance.agent.killedByAgent, __instance.agent, "Dead");
										GC.quests.AddBigQuestPoints(__instance.agent.killedByAgent, __instance.agent, "Neutralize");
										GC.quests.AddBigQuestPoints(__instance.agent.killedByAgent, __instance.agent, "BloodlustKill");
									}
									else
									{
										if (__instance.agent.relationships.GetRel(__instance.agent.killedByAgent) != "Aligned")
											__instance.agent.killedByAgent.skillPoints.AddPoints("KillPoints");
										
										GC.quests.AddBigQuestPoints(__instance.agent.killedByAgent, "KillGuilty");
										GC.quests.AddBigQuestPoints(__instance.agent.killedByAgent, __instance.agent, "Dead");
										GC.quests.AddBigQuestPoints(__instance.agent.killedByAgent, __instance.agent, "Neutralize");
										GC.quests.AddBigQuestPoints(__instance.agent.killedByAgent, __instance.agent, "BloodlustKill");
									}
								}
							}
						}

						if (__instance.agent.killForQuest != null && __instance.agent.justHitByAgent2 != null && __instance.agent.justHitByAgent2.isPlayer > 0)
						{
							try
							{
								__instance.agent.killForQuest.playerTally[__instance.agent.justHitByAgent2.isPlayer - 1]++;
							}
							catch
							{
								Debug.LogError(string.Concat("Agent killForQuest playerTally Error: ", __instance.agent, " - ", __instance.agent.killForQuest, " - ", __instance.agent.justHitByAgent2));
							}
						}

						if (__instance.agent.justHitByAgent2 != null && __instance.agent.isPlayer == 0 && (!__instance.agent.justHitByAgent2.resurrect || __instance.agent.justHitByAgent2.zombified) && !flag17)
						{
							if (__instance.agent.justHitByAgent2.zombified && __instance.agent.agentName == "Zombie")
								flag17 = true;
							
							if (!flag17)
							{
								Agent agent9 = null;
								Agent agent10 = null;
							
								if (__instance.agent.justHitByAgent2.mindControlAgent != null)
								{
									agent9 = __instance.agent.justHitByAgent2.mindControlAgent;
								
									if (__instance.agent.killedByAgent != null && __instance.agent.killedByAgent.mindControlAgent != null)
										agent10 = __instance.agent.killedByAgent.mindControlAgent;
								}
								else if (__instance.agent.justHitByAgent2.employer != null)
								{
									__instance.agent.justHitByAgent2.killsForEmployer++;
									agent9 = __instance.agent.justHitByAgent2.employer;
								
									if (__instance.agent.killedByAgent != null && __instance.agent.killedByAgent.employer != null)
										agent10 = __instance.agent.killedByAgent.employer;
								}
								if (agent9 != null)
								{
									GC.stats.AddToStat(agent9, "IndirectlyKilled", 1);
								
									if (__instance.AgentIsRival(__instance.agent.killedByAgent))
									{
										if (agent10 != null && !__instance.agent.playersReceivedPoints.Contains(agent10))
										{
											agent10.skillPoints.AddPoints("IndirectlyKillRival");
											__instance.agent.playersReceivedPoints.Add(agent10);
										}
									}
									else if (flag16)
									{
										if (!__instance.agent.playersReceivedPoints.Contains(agent9))
										{
											agent9.skillPoints.AddPoints("IndirectlyKillInnocent");
											__instance.agent.playersReceivedPoints.Add(agent9);
										}
									}
									else if (!__instance.agent.playersReceivedPoints.Contains(agent9))
									{
										agent9.skillPoints.AddPoints("IndirectlyKill");
										__instance.agent.playersReceivedPoints.Add(agent9);
									}

									__instance.agent.relationships.AddToKillStat(agent9);
								}
							}

							__instance.ActivateKillProfiter();
						}

						if (!__instance.agent.KnockedOut())
							__instance.RealKillRepercussions(flag10);
						
						if (GC.serverPlayer)
						{
							if (__instance.agent.agentName == "Gangbanger" && __instance.agent.killedByAgent != null)
							{
								if (__instance.agent.killedByAgent.isPlayer != 0)
								{
									GC.sessionData.gangbangersKilled++;
						
									if (GC.sessionData.gangbangersKilled >= GC.unlocks.gangbangerUnlockAmount)
										GC.unlocks.DoUnlock("GangbangerB", "Agent");
									
									if (GC.multiplayerMode)
										GC.playerAgent.objectMult.CallRpcUnlockProgressAll("GangbangerB", "Agent");
								}
							}
							else if (__instance.agent.agentName == "Ghost")
							{
								GC.unlocks.DoUnlock("Werewolf", "Agent");

								if (GC.multiplayerMode)
									GC.playerAgent.objectMult.CallRpcUnlockAll("Werewolf", "Agent");
							}
							else if (__instance.agent.agentName == "Assassin")
							{
								if (__instance.agent.isPlayer == 0)
								{
									GC.unlocks.DoUnlock("AssassinsEveryLevel", "Challenge");
							
									if (GC.multiplayerMode)
										GC.playerAgent.objectMult.CallRpcUnlockAll("AssassinsEveryLevel", "Challenge");
								}
							}
							else if ((__instance.agent.originalWerewolf || __instance.agent.oma.secretWerewolf) && __instance.agent.killedByAgent != null)
							{
								if (__instance.agent.killedByAgent.isPlayer != 0)
								{
									GC.unlocks.DoUnlockProgress("ManyWerewolf", "Challenge", 1);

									if (GC.multiplayerMode)
										GC.playerAgent.objectMult.CallRpcUnlockProgressAll("ManyWerewolf", "Challenge");
								}
							}
							else if (__instance.agent.agentName == "OfficeDrone")
							{
								if (__instance.agent.oma.offeredOfficeDrone)
									GC.ChangeStockPrice("Negative2");
								else
									GC.ChangeStockPrice("Negative");
							}
						}
					}

					_ = __instance.agent.killedByAgent != null; // Sic

					if (__instance.agent.protectingAgent != null)
						__instance.agent.protectingAgent.hasProtector = false;
				}
				else
				{
					Debug.Log("SETUPDEATH " + __instance.agent);
					__instance.SetupDeath(damagerObject, flag10);

					if (!__instance.agent.KnockedOut())
					{
						if (GC.arcadeMode)
							__instance.StartCoroutine(__instance.WaitForArcadeContinue());
						else
							__instance.StartCoroutine(__instance.WaitForGhostRespawn());
					}
				}
			}
			else if (((__instance.agent.KnockedOut() && __instance.agent.health <= -10f) || (__instance.agent.FellInHole() && __instance.agent.health <= 0f) || __instance.agent.burnt) && flag9)
			{
				Debug.Log("Death Type 2: " + __instance.agent);

				if (!GC.serverPlayer && __instance.agent.isPlayer != 0 && __instance.agent.localPlayer)
					__instance.agent.objectMult.CallCmdDieAfterKnockout();
				
				if (__instance.agent.KnockedOut() && GC.levelType == vLevelType.Tutorial)
					GC.tutorial.tutorialPeopleKilled++;
				
				__instance.agent.knockedOut = false;
				__instance.agent.knockedOutLocal = false;
				__instance.agent.sleepKnockedOut = false;
				
				if (__instance.agent.oma.bodyGuarded)
				{
					__instance.agent.zombified = false;
					__instance.agent.zombieWhenDead = false;
				}
				
				if (GC.levelType == vLevelType.Tutorial)
					flag12 = true;
				
				if (!__instance.agent.dead)
					__instance.RealKillRepercussions(flag10);
				
				GC.spawnerMain.SpawnStateIndicator(__instance.agent, "NoAnim");
				__instance.agent.relationships.SetAllRel2("Dead");

				if (!__instance.agent.oma.bodyGuarded)
				{
					__instance.agent.job = "";
					__instance.agent.jobCode = jobType.None;
					__instance.agent.jobBig = "";
					__instance.agent.jobBigCode = jobType.None;
					__instance.agent.SetFollowing(null);
				
					if (!__instance.agent.resurrect)
						__instance.agent.SetEmployerNullLate();
				}

				if (__instance.agent.isMayor)
					GC.ending.SetMayorTurnover("Dead");
				
				__instance.agent.SetTraversable("");
				__instance.agent.agentHitboxScript.eyesWBGO.SetActive(true);
				
				if (__instance.agent.isPlayer > 0)
				{
					if (__instance.agent.possessing)
						__instance.agent.resurrect = true;
				
					__instance.SetupDeath(damagerObject, flag10);
					
					if (!__instance.agent.resurrect)
					{
						if (GC.coopMode || GC.multiplayerMode || (GC.fourPlayerMode && !GC.arcadeMode))
						{
							if (__instance.agent.FellInHole())
								__instance.RespawnAsGhost();
							else
								__instance.StartCoroutine(__instance.WaitForGhostRespawn());
						}
						else if (GC.arcadeMode)
						{
							Debug.Log("SETUPDEATH 2" + __instance.agent);
							__instance.StartCoroutine(__instance.WaitForArcadeContinue());
						}
					}
				}
				else
				{
					__instance.agent.Say("", true);

					if (!__instance.agent.zombified && __instance.agent.zombieWhenDead && __instance.agent.isPlayer == 0 && !__instance.agent.inhuman && __instance.agent.agentName != "Alien" && !__instance.agent.FellInHole() && !__instance.agent.burnt)
					{
						if (__instance.agent.justHitByAgent2 != null && __instance.agent.justHitByAgent2.zombified)
							__instance.agent.zombifiedByAgent = __instance.agent.justHitByAgent2;
					
						__instance.agent.zombified = true;
						__instance.agent.resurrect = true;
						
						if (__instance.agent.nonQuestObjectMarker != null)
						{
							bool flag18 = true;
						
							for (int num9 = 0; num9 < GC.playerAgentList.Count; num9++)
								if (GC.playerAgentList[num9].agentName == "Zombie")
									flag18 = false;
							
							if (flag18)
								Object.Destroy(__instance.agent.nonQuestObjectMarker.gameObject);
						}

						__instance.StartCoroutine(__instance.ResurrectSequence());
					}
					else if (__instance.agent.resurrect && __instance.agent.isPlayer == 0 && !__instance.agent.FellInHole())
						__instance.StartCoroutine(__instance.ResurrectSequence());
				}

				__instance.agent.agentHitboxScript.wholeBodyMode = 0;
				__instance.agent.agentHitboxScript.SetWBSprites();
			}

			if (flag9)
			{
				if (__instance.agent.frozen && !__instance.agent.dead && healthNum <= 0f && __instance.agent.deathMethod != "Poison" && __instance.agent.deathMethod != "Nicotine" && __instance.agent.deathMethod != "Acid" && __instance.agent.deathMethod != "Cyanide")
					__instance.RemoveStatusEffect("Frozen");
				
				if (__instance.agent.frozen && __instance.agent.resurrect)
				{
					__instance.RemoveStatusEffect("Frozen");
					__instance.agent.frozen = false;
					__instance.agent.objectSprite.agentColorDirty = true;
				}

				if (__instance.agent.ghost && __instance.agent.dead)
				{
					if (GC.multiplayerMode)
						__instance.agent.objectMult.Gib(2);
				
					__instance.GhostGib();
				}
				else if (__instance.agent.frozen && __instance.agent.dead && !__instance.agent.KnockedOut() && !__instance.agent.FellInHole() && !__instance.agent.disappeared && !__instance.agent.resurrect && !__instance.agent.inhuman && !__instance.agent.oma.bodyGuarded)
				{
					if (GC.serverPlayer || (!GC.serverPlayer && (__instance.agent.localPlayer || flag10)))
					{
						if (GC.multiplayerMode)
							__instance.agent.objectMult.Gib(1);
					
						__instance.IceGib();
					}
				}
				else if ((__instance.agent.health <= -20f || healthNum <= -20f || (num <= -20f && healthNum <= 0f) || ((GC.levelType == "Tutorial" && healthNum <= -10f) || flag12) || __instance.agent.bloodyMessed) && __instance.agent.dead && !__instance.agent.KnockedOut() && !__instance.agent.resurrect && !__instance.agent.FellInHole() && !__instance.agent.inhuman && !__instance.agent.disappeared && !flag8 && !__instance.agent.oma.bodyGuarded)
				{
					bool flag19 = true;

					if (__instance.agent.justHitByAgent2 != null && __instance.agent.justHitByAgent2.melee != null && (__instance.agent.justHitByAgent2.melee.successfullyBackstabbed || __instance.agent.justHitByAgent2.melee.successfullySleepKilled) && !__instance.agent.justHitByAgent2.statusEffects.hasTrait("BloodyMess"))
					{
						flag19 = false;
						__instance.agent.health = 0f;
					}
					
					if (flag19 && (GC.serverPlayer || (!GC.serverPlayer && (__instance.agent.localPlayer || flag10) && !__instance.hasTrait(vStatusEffect.Resurrection))))
					{
						if (GC.multiplayerMode && !__instance.dontDoBloodExplosion)
							__instance.agent.objectMult.Gib(0);
					
						__instance.NormalGib();
					}
				}

				if (__instance.agent.frozen && __instance.agent.dead)
				{
					__instance.agent.frozen = false;
					__instance.agent.objectSprite.agentColorDirty = true;
				}
			}

			__instance.agent.bloodyMessed = false;
			__instance.agent.slippedOnBananaPeel = false;
			__instance.agent.justHitByAgent2 = null;

			return false;
		}
		public static void StatusEffects_RemoveTrait(string traitName, bool onlyLocal, StatusEffects __instance) // Postfix
		{
			Agent agent = __instance.agent;
			if (traitName == cTrait.Fatass)
			{
				//TODO: CharacterCreation.CreatePointTallyText() for stat mods
				agent.SetEndurance(agent.enduranceStatMod - 1);
				agent.SetSpeed(agent.speedStatMod + 1);
			}
		}
		#endregion
	}
	public class Agent_Remora
	{
		public static GameController GC => GameController.gameController;

		public Agent agentHost;

		public bool hasElevatorPass = false;
	}
}
