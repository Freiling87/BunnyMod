﻿using RogueLibsCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;
using System.Collections;

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
			InitializeNames();

			Agent_00();
			AgentInteractions_00();
			ObjectMult_00();
			Relationships_00();
			StatusEffects_00();
		}
		public static void InitializeNames()
		{
			string t = vNameType.Dialogue;
			_ = RogueLibs.CreateCustomName(cDialogue.VeiledThreatsAnnoyed, t, new CustomNameInfo("Did you just... threaten me?"));
			_ = RogueLibs.CreateCustomName(cDialogue.WarlordSubmission, t, new CustomNameInfo("Okay, okay! You're the boss now!"));
		}

		#region Custom
		public static void AnnoyWitnesses(Agent perp, Agent victim)
		{
			for (int i = 0; i < GC.agentList.Count; i++)
			{
				Agent bystander = GC.agentList[i];

				if (Vector2.Distance(bystander.tr.position, perp.tr.position) < bystander.LOSRange / perp.hardToSeeFromDistance && 
					bystander != perp && bystander != victim && !bystander.zombified && !bystander.ghost && !bystander.oma.hidden && 
					(!perp.aboveTheLaw || !bystander.enforcer || victim.enforcer) && 
					perp.prisoner == bystander.prisoner && !perp.invisible && !victim.noEnforcerAlert)
				{
					string perpRel = bystander.relationships.GetRel(perp);
					string victimRel = bystander.relationships.GetRel(victim);

					if (victimRel != vRelationship.Hostile && victimRel != vRelationship.Annoyed)
					{
						if (perpRel == vRelationship.Neutral || perpRel == vRelationship.Friendly)
						{
							if (bystander.relationships.GetRelationship(perp).hasLOS)
							{
								relStatus victimRel2 = bystander.relationships.GetRelCode(victim);
								relStatus perpRel2 = bystander.relationships.GetRelCode(perp);

								if ((victimRel2 == relStatus.Aligned && perpRel2 != relStatus.Aligned) || 
									(victimRel2 == relStatus.Loyal && perpRel2 != relStatus.Aligned && perpRel2 != relStatus.Loyal))
								{
									bystander.sawBiteList.Add(victim);
									bystander.relationships.SetRelHate(perp, 5);
								}
								else
								{
									bystander.sawBiteList.Add(victim);
									bystander.relationships.SetStrikes(perp, 2);
								}
							}
						}
						else if (perpRel == vRelationship.Annoyed && bystander.relationships.GetRelationship(perp).hasLOS)
						{
							bystander.sawBiteList.Add(victim);
							bystander.relationships.SetRelHate(perp, 5);
						}
					}
				}
			}
		}
		public static void AnnoyWitnessesVictimless(Agent perp)
		{
			for (int i = 0; i < GC.agentList.Count; i++)
			{
				Agent bystander = GC.agentList[i];

				if (Vector2.Distance(bystander.tr.position, perp.tr.position) < bystander.LOSRange / perp.hardToSeeFromDistance &&
					bystander != perp && !bystander.zombified && !bystander.ghost && !bystander.oma.hidden &&
					(!perp.aboveTheLaw || !bystander.enforcer) &&
					perp.prisoner == bystander.prisoner && !perp.invisible)
				{
					string perpRel = bystander.relationships.GetRel(perp);

					if (perpRel == vRelationship.Neutral || perpRel == vRelationship.Friendly)
					{
						if (bystander.relationships.GetRelationship(perp).hasLOS)
						{
							relStatus perpRel2 = bystander.relationships.GetRelCode(perp);

							if (perpRel2 != relStatus.Aligned || perpRel2 != relStatus.Loyal)
								bystander.relationships.SetStrikes(perp, 2);
						}
					}
					else if (perpRel == vRelationship.Annoyed && bystander.relationships.GetRelationship(perp).hasLOS)
					{
						bystander.relationships.SetRelHate(perp, 5);
					}
				}
			}
		}
		public static void SpawnEmployees(Agent playerAgent, int numberToSpawn, string agentType, LoadLevel __instance, string relationship) // Non-Patch
		{
			BMLog("SpawnEmployees");

			for (int i = 0; i <numberToSpawn; i++)
				GC.spawnerMain.SpawnAgent(GC.tileInfo.FindLocationNearLocation(playerAgent.tr.position, playerAgent, 0.32f, 0.96f, true, false), playerAgent, agentType, "FriendPhone", playerAgent);
		}
		public static void SpawnRoamerSquad(Agent playerAgent, int numberToSpawn, string agentType, LoadLevel __instance, string relationship, int splitIntoGroupSize) // Non-Patch
		{
			BMLog("LoadLevel_SpawnRoamerSquad");

			List<Agent> spawnedAgentList = new List<Agent>();
			//playerAgent.gangStalking = Agent.gangCount;
			Vector2 pos = Vector2.zero;

			numberToSpawn = (int)((float)numberToSpawn * __instance.levelSizeModifier);

			for (int i = 0; i < numberToSpawn; i++)
			{
				if (i % splitIntoGroupSize == 0)
					Agent.gangCount++; // Splits spawn into groups

				Vector2 vector = Vector2.zero;
				int attempts = 0;

				if (i == 0)
				{
					do
					{
						vector = GC.tileInfo.FindRandLocationGeneral(0.32f);
						attempts++;
					}
					while ((vector == Vector2.zero || Vector2.Distance(vector, GC.playerAgent.tr.position) < 20f) && attempts < 300);

					pos = vector;
				}
				else
					vector = GC.tileInfo.FindLocationNearLocation(pos, null, 0.32f, 1.28f, true, true);

				if (vector != Vector2.zero && attempts < 300)
				{
					Agent agent = GC.spawnerMain.SpawnAgent(vector, null, agentType);
					agent.movement.RotateToAngleTransform((float)Random.Range(0, 360));
					agent.gang = Agent.gangCount;
					agent.modLeashes = 0;

					if (agentType == vAgent.Ghost)
						agent.alwaysRun = true;

					agent.wontFlee = true;
					agent.agentActive = true;
					//agent.statusEffects.AddStatusEffect("InvisiblePermanent");
					agent.oma.mustBeGuilty = true;
					spawnedAgentList.Add(agent);

					// Align agents in group to each other
					if (spawnedAgentList.Count > 1)
						for (int j = 0; j < spawnedAgentList.Count; j++)
							if (spawnedAgentList[j] != agent)
							{
								agent.relationships.SetRelInitial(spawnedAgentList[j], vRelationship.Aligned);
								spawnedAgentList[j].relationships.SetRelInitial(agent, vRelationship.Aligned);
							}

					agent.relationships.SetRel(playerAgent, relationship);
					playerAgent.relationships.SetRel(agent, relationship);

					switch (relationship.ToString())
					{
						case vRelationship.Annoyed:
							agent.relationships.SetRelHate(playerAgent, 1);
							playerAgent.relationships.SetRelHate(agent, 1);

							break;
						case vRelationship.Hostile:
							agent.relationships.SetRelHate(playerAgent, 5);
							playerAgent.relationships.SetRelHate(agent, 5);
							
							break;
					}

					if (agentType == vAgent.ResistanceLeader && BMTraits.IsPlayerTraitActive(cTrait.Reinforcements_2))
					{
						InvItem invItem = new InvItem();
						invItem.invItemName = GC.Choose<string>(vItem.Revolver, vItem.MachineGun);
						invItem.ItemSetup(false);
						invItem.invItemCount = invItem.rewardCount;
						agent.inventory.AddItemAtEmptySlot(invItem, true, false);
						agent.inventory.equippedWeapon = invItem;

						agent.inventory.startingHeadPiece = vArmorHead.SoldierHelmet;
					}
					else if (agentType == vAgent.ResistanceLeader && BMTraits.IsPlayerTraitActive(cTrait.Reinforcements))
					{
						InvItem invItem = new InvItem();
						invItem.invItemName = GC.Choose<string>(vItem.Pistol, vItem.Knife);
						invItem.ItemSetup(false);
						invItem.invItemCount = invItem.rewardCount;
						agent.inventory.AddItemAtEmptySlot(invItem, true, false);
						agent.inventory.equippedWeapon = invItem;

						agent.inventory.startingHeadPiece = vArmorHead.HardHat;
					}

					agent.SetDefaultGoal(vAgentGoal.WanderLevel);
				}
			}
		}
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
			// Triggers at level gen. Not instantiator, may be reusing an existing Agent from last level. Saves overhead on procgen appearances/inv/etc.
			// VeiledThreats tracking bool

			if (veiledThreatsUsed.ContainsKey(__instance))
				veiledThreatsUsed[__instance] = false;
			else
				veiledThreatsUsed.Add(__instance, false);
		}
		public static Dictionary<Agent, bool> veiledThreatsUsed = new Dictionary<Agent, bool>();
		#endregion
		#region AgentInteractions
		public void AgentInteractions_00()
		{
			Type t = typeof(AgentInteractions);
			Type g = GetType();

			Prefix(t, "AddButton", g, "AgentInteractions_AddButton", new Type[3] { typeof(string), typeof(int), typeof(string) });
			//Prefix(typeof(AgentInteractions), "DetermineButtons", GetType(), "AgentInteractions_DetermineButtons", new Type[5] { typeof(Agent), typeof(Agent), typeof(List<string>), typeof(List<string>), typeof(List<int>) });
			//Prefix(typeof(AgentInteractions), "PressedButton", GetType(), "AgentInteractions_PressedButton", new Type[4] { typeof(Agent), typeof(Agent), typeof(string), typeof(int) });
			Prefix(t, "Shakedown", g, "AgentInteractions_Shakedown", new Type[2] { typeof(Agent), typeof(Agent) });
			Prefix(t, "Threaten", g, "AgentInteractions_Threaten", new Type[2] { typeof(Agent), typeof(Agent) });
			Prefix(t, "ThreatenKey", g, "AgentInteractions_ThreatenKey", new Type[2] { typeof(Agent), typeof(Agent) });
			Prefix(t, "ThreatenKeyAndSafeCombination", g, "AgentInteractions_ThreatenKeyAndSafeCombination", new Type[2] { typeof(Agent), typeof(Agent) });
			Prefix(t, "ThreatenLeaveTown", g, "AgentInteractions_ThreatenLeaveTown", new Type[2] { typeof(Agent), typeof(Agent) });
			Prefix(t, "ThreatenMayor", g, "AgentInteractions_ThreatenMayor", new Type[2] { typeof(Agent), typeof(Agent) });
			Prefix(t, "ThreatenMayorBadge", g, "AgentInteractions_ThreatenMayorBadge", new Type[2] { typeof(Agent), typeof(Agent) });
			Prefix(t, "ThreatenMoney", g, "AgentInteractions_ThreatenMoney", new Type[2] { typeof(Agent), typeof(Agent) });
			Prefix(t, "ThreatenSafeCombination", g, "AgentInteractions_ThreatenSafeCombination", new Type[2] { typeof(Agent), typeof(Agent) });
			//Postfix(typeof(AgentInteractions), "UseItemOnObject", GetType(), "AgentInteractions_UseItemOnObject", new Type[6] { typeof(Agent), typeof(Agent), typeof(InvItem), typeof(int), typeof(string), typeof(string) });
		}
		public static void AgentInteractions_AddButton(string buttonName, int moneyCost, string extraCost, AgentInteractions __instance, ref Agent ___mostRecentInteractingAgent) // Prefix
		{
			if (extraCost.EndsWith("-30"))
				extraCost.Replace("-30", "-" + ToolCost(___mostRecentInteractingAgent, 30));
			else if (extraCost.EndsWith("-20"))
				extraCost.Replace("-20", "-" + ToolCost(___mostRecentInteractingAgent, 20));
		}
		public static void AgentInteractions_AddButton_4(string buttonName, int moneyCost, string extraCost) // Prefix
		{
			BMLog("Adding Button: buttonName = " + buttonName + "; moneyCost = " + moneyCost + "; extraCost = " + extraCost);
		}
		public static bool AgentInteractions_DetermineButtons(Agent agent, Agent interactingAgent, List<string> buttons1, List<string> buttonsExtra1, List<int> buttonPrices1, AgentInteractions __instance) // Prefix
		{
			BMLog("AgentInteractions_DetermineButtons: agent = " + agent.agentName + agent.agentID + "; Gang: " + agent.gang + "; GangMugging: " + interactingAgent.gangMugging);

			if (agent.agentName == "Hobo")
			{
				GC.audioHandler.Play(agent, "AgentTalk");

				if (agent.gang == interactingAgent.gangMugging && agent.gang != 0)
				{
					BMLog("AgentInteractions_DetermineButtons: Adding Buttons");

					__instance.AddButton("Hobo_GiveMoney1", agent.determineMoneyCost("Hobo_GiveMoney1"));
					__instance.AddButton("Hobo_GiveMoney2", agent.determineMoneyCost("Hobo_GiveMoney2"));
					__instance.AddButton("Hobo_GiveMoney3", agent.determineMoneyCost("Hobo_GiveMoney3"));
					__instance.AddButton("Hobo_GiveItem", "(Choose)");
				}
				else
					BMHeaderTools.SayDialogue(agent, "Interact", vNameType.Dialogue);
			}
			else if (agent.agentName == "Gangbanger" || agent.agentName == "GangbangerB")
			{
				GC.audioHandler.Play(agent, "AgentTalk");

				if (agent.gang == interactingAgent.gangMugging && agent.gang != 0)
					__instance.AddButton("Gangbanger_GiveMoney", agent.determineMoneyCost("Mug_Gangbanger"));
				else
					BMHeaderTools.SayDialogue(agent, "Interact", vNameType.Dialogue);
			}
			else if (agent.agentName == "Junkie")
			{
				GC.audioHandler.Play(agent, "AgenTalk");

				if (agent.gang == interactingAgent.gangMugging && agent.gang != 0)
				{
					__instance.AddButton("Junkie_GiveMoney", agent.determineMoneyCost("Mug_Junkie"));
					__instance.AddButton("Junkie_GiveItem", agent.determineMoneyCost("Junkie_GiveItem"));
				}
				else
					BMHeaderTools.SayDialogue(agent, "Interact", vNameType.Dialogue);
			}

			return true;
		}
		public static bool AgentInteractions_PressedButton(Agent agent, Agent interactingAgent, string buttonText, int buttonPrice, AgentInteractions __instance) // Prefix
		{
			BMLog("AgentInteractions_PressedButton: " + agent.agentName + " / " + buttonText);

			if (agent.agentName == "Hobo")
			{
				if (buttonText == "Hobo_GiveMoney1" || buttonText == "Hobo_GiveMoney2" || buttonText == "Hobo_GiveMoney3")
				{
					if (agent.moneySuccess(buttonPrice))
						BMBehaviors.Hobo_MugMoney(agent, interactingAgent, buttonPrice, BMBehaviors.Hobo_relStatusAfterDonation(agent, interactingAgent, buttonPrice).ToString("f"), buttonText);
					else
						BMHeaderTools.SayDialogue(agent, "Hobo_CantAfford", vNameType.Dialogue);

					agent.StopInteraction();

					return false; // Double-check that these aren't skipping anything important
				}
				else if (buttonText == "Hobo_GiveItem")
				{
					agent.ShowUseOn("Hobo_Donate");

					return false;
				}
			}
			else if ((agent.agentName == "Gangbanger" || agent.agentName == "GangbangerB"))
			{
				if (buttonText == "GangBanger_GiveMoney")
				{
					if (agent.moneySuccess(buttonPrice))
						__instance.MugMoney(agent, interactingAgent);
					else
						BMHeaderTools.SayDialogue(agent, "Gangbanger_CantAfford", vNameType.Dialogue);

					agent.StopInteraction();

					return false; // Double-check that these aren't skipping anything important
				}
			}

			return true;
		}
		public static bool AgentInteractions_Shakedown(Agent agent, Agent interactingAgent, AgentInteractions __instance) // Prefix
		{
			// Veiled Threats

			if (interactingAgent.statusEffects.hasTrait(cTrait.VeiledThreats) && !veiledThreatsUsed[agent])
			{
				if (!interactingAgent.gc.serverPlayer)
				{
					interactingAgent.objectMult.ObjectAction(agent.objectNetID, "Shakedown");

					return false;
				}

				int myChance = agent.relationships.FindThreat(interactingAgent, true);

				if (agent.gc.percentChance(myChance) || agent.health <= agent.healthMax * 0.4f || agent.relationships.GetRel(interactingAgent) == "Aligned" || agent.slaveOwners.Contains(interactingAgent))
				{
					int num = 0;

					for (int i = 0; i < agent.gc.agentList.Count; i++)
						if (agent.gc.agentList[i].oma.shookDown)
							num++;
					
					if (num >= 3)
						BMHeaderTools.SayDialogue(agent, "ThreatenedShakedownLowCash", vNameType.Dialogue);
					else
						BMHeaderTools.SayDialogue(agent, "ThreatenedShakedown", vNameType.Dialogue);
					
					int shakedownAmount = interactingAgent.objectMult.FindShakedownAmount(num);
					
					if (interactingAgent.localPlayer)
					{
						interactingAgent.objectMult.AddShakedownPerson(agent, shakedownAmount, false);
						agent.gc.audioHandler.Play(interactingAgent, "Success");
					}
					else
						interactingAgent.objectMult.AddShakedownPerson(agent, shakedownAmount, true);
					
					agent.shookDownAgent = interactingAgent;
					agent.oma.shookDown = true;

					if (agent.relationships.GetRel(interactingAgent) != "Aligned" && agent.relationships.GetRel(interactingAgent) != "Loyal")
						agent.relationships.SetRel(interactingAgent, "Submissive");

					__instance.BecomeSubmissiveWithAlliesAllInChunk(agent, interactingAgent);

					return false;
				}

				agent.StopInteraction();
				BMHeaderTools.SayDialogue(agent, cDialogue.VeiledThreatsAnnoyed, vNameType.Dialogue);
				agent.relationships.SetRel(interactingAgent, vRelationship.Annoyed);
				agent.relationships.SetRelHate(interactingAgent, 2);
				veiledThreatsUsed[agent] = true;

				return false;
			}

			return true;
		}
		public static bool AgentInteractions_Threaten(Agent agent, Agent interactingAgent) // Prefix
		{
			// Veiled Threats

			if (interactingAgent.statusEffects.hasTrait(cTrait.VeiledThreats) && !veiledThreatsUsed[agent])
			{
				if (!GC.serverPlayer)
				{
					interactingAgent.objectMult.ObjectAction(agent.objectNetID, "Threaten");

					return false;
				}

				int myChance = agent.relationships.FindThreat(interactingAgent, true);

				if (agent.gc.percentChance(myChance))
				{
					for (int i = 0; i < agent.inventory.InvItemList.Count; i++)
					{
						InvItem invItem = agent.inventory.InvItemList[i];

						if (invItem.questItem)
						{
							agent.inventory.DestroyItem(invItem);
							interactingAgent.inventory.AddItemAtEmptySlot(invItem, false, false);
							BMHeaderTools.SayDialogue(agent, "ThreatenedScared", vNameType.Dialogue);
							agent.killForQuest.boughtItem = true;
							agent.cantSleep = true;
							agent.oma.didAsk = true;
							GC.audioHandler.Play(interactingAgent, "SelectItem");
						}
					}

					return false;
				}

				agent.StopInteraction();
				BMHeaderTools.SayDialogue(agent, cDialogue.VeiledThreatsAnnoyed, vNameType.Dialogue);
				agent.relationships.SetRel(interactingAgent, vRelationship.Annoyed);
				agent.relationships.SetRelHate(interactingAgent, 2);
				agent.oma.didAsk = true;
				// GC.EnforcerAlertAttack(interactingAgent, agent, 7.4f);

				return false;
			}

			return true;
		}
		public static bool AgentInteractions_ThreatenKey(Agent agent, Agent interactingAgent, AgentInteractions __instance) // Prefix
		{
			// Veiled Threats

			if (interactingAgent.statusEffects.hasTrait(cTrait.VeiledThreats) & !veiledThreatsUsed[agent])
			{
				if (!GC.serverPlayer)
				{
					interactingAgent.objectMult.ObjectAction(agent.objectNetID, "ThreatenKey");

					return false;
				}

				int myChance = agent.relationships.FindThreat(interactingAgent, true);

				if (GC.percentChance(myChance))
				{
					for (int i = 0; i < agent.inventory.InvItemList.Count; i++)
					{
						InvItem invItem = agent.inventory.InvItemList[i];

						if (invItem.invItemName == "Key")
						{
							agent.inventory.DestroyItem(invItem);
							interactingAgent.inventory.AddItemAtEmptySlot(invItem, false, false);

							for (int j = 0; j < GC.objectRealList.Count; j++)
								if (GC.objectRealList[j].objectName == "Door" && GC.objectRealList[j].startingChunk == agent.startingChunk && GC.objectRealList[j].owner == agent.ownerID)
									((Door)GC.objectRealList[j]).boughtKeyInChunk = true;
							
							if (agent.relationships.GetRel(interactingAgent) != "Aligned" && agent.relationships.GetRel(interactingAgent) != "Loyal")
								agent.relationships.SetRel(interactingAgent, "Submissive");
							
							__instance.BecomeSubmissiveWithAlliesAllInChunk(agent, interactingAgent);
							BMHeaderTools.SayDialogue(agent, "ThreatenedScared", vNameType.Dialogue);
							GC.audioHandler.Play(interactingAgent, "SelectItem");
						}
					}

					return false;
				}

				agent.StopInteraction();
				BMHeaderTools.SayDialogue(agent, cDialogue.VeiledThreatsAnnoyed, vNameType.Dialogue);
				agent.relationships.SetRel(interactingAgent, vRelationship.Annoyed);
				agent.relationships.SetRelHate(interactingAgent, 3);

				return false;
			}

			return true;
		}
		public static bool AgentInteractions_ThreatenKeyAndSafeCombination(Agent agent, Agent interactingAgent, AgentInteractions __instance) // Prefix
		{
			// Veiled Threats

			if (interactingAgent.statusEffects.hasTrait(cTrait.VeiledThreats) && !veiledThreatsUsed[agent])
			{
				if (!GC.serverPlayer)
				{
					interactingAgent.objectMult.ObjectAction(agent.objectNetID, "ThreatenKeyAndSafeCombination");
					
					return false;
				}

				int myChance = agent.relationships.FindThreat(interactingAgent, true);
				
				if (agent.gc.percentChance(myChance))
				{
					for (int i = 0; i < agent.inventory.InvItemList.Count; i++)
					{
						InvItem invItem = agent.inventory.InvItemList[i];
				
						if (invItem.invItemName == "Key" || invItem.invItemName == "SafeCombination")
						{
							agent.inventory.DestroyItem(invItem);
							interactingAgent.inventory.AddItemAtEmptySlot(invItem, false, false);
						}
					}

					for (int j = 0; j < agent.gc.objectRealList.Count; j++)
					{
						if (agent.gc.objectRealList[j].objectName == "Door" && agent.gc.objectRealList[j].startingChunk == agent.startingChunk && agent.gc.objectRealList[j].owner == agent.ownerID)
							((Door)agent.gc.objectRealList[j]).boughtKeyInChunk = true;
					
						if (agent.gc.objectRealList[j].chestReal && agent.gc.objectRealList[j].startingChunk == agent.startingChunk && agent.gc.objectRealList[j].owner == agent.ownerID)
							agent.gc.objectRealList[j].allowedToSteal = true;
					}

					if (agent.relationships.GetRel(interactingAgent) != "Aligned" && agent.relationships.GetRel(interactingAgent) != "Loyal")
						agent.relationships.SetRel(interactingAgent, "Submissive");
					
					__instance.BecomeSubmissiveWithAlliesAllInChunk(agent, interactingAgent);
					BMHeaderTools.SayDialogue(agent, "ThreatenedScared", vNameType.Dialogue);
					GC.audioHandler.Play(interactingAgent, "SelectItem");

					return false;
				}

				agent.StopInteraction();
				BMHeaderTools.SayDialogue(agent, cDialogue.VeiledThreatsAnnoyed, vNameType.Dialogue);
				agent.relationships.SetRel(interactingAgent, vRelationship.Annoyed);
				agent.relationships.SetRelHate(interactingAgent, 2);

				return false;
			}

			return true;
		}
		public static bool AgentInteractions_ThreatenLeaveTown(Agent agent, Agent interactingAgent) // Prefix
		{
			// Veiled Threats

			if (interactingAgent.statusEffects.hasTrait(cTrait.VeiledThreats) && !veiledThreatsUsed[agent])
			{
				if (interactingAgent.gc.serverPlayer)
				{
					int myChance = agent.relationships.FindThreat(interactingAgent, true);

					if (agent.slaveOwners.Count > 0)
					{
						BMHeaderTools.SayDialogue(agent, "AskLeaveTownSlave", vNameType.Dialogue);

						return false;
					}

					if (!agent.gc.percentChance(myChance))
					{
						agent.StopInteraction();
						BMHeaderTools.SayDialogue(agent, cDialogue.VeiledThreatsAnnoyed, vNameType.Dialogue);
						agent.relationships.SetRel(interactingAgent, vRelationship.Annoyed);
						agent.relationships.SetRelHate(interactingAgent, 2);
						agent.oma.didAsk = true;

						return false;
					}

					BMHeaderTools.SayDialogue(agent, "ThreatenedScaredLeaveTown", vNameType.Dialogue);
					agent.wantsToExit = true;
					float num = Vector2.Distance(agent.transform.position, agent.gc.startingPoint.tr.position);
					float num2 = Vector2.Distance(agent.transform.position, agent.gc.exitPoint.tr.position);

					if (num < num2)
						agent.wantsToExitLocation = agent.gc.startingPoint;
					else
						agent.wantsToExitLocation = agent.gc.exitPoint;
					
					agent.oma.didAsk = true;
					
					if (agent.killForQuest != null)
					{
						agent.killForQuest.playerTally[interactingAgent.isPlayer - 1]++;
						agent.killForQuest.player = interactingAgent;
					}
					
					string rel = agent.relationships.GetRel(interactingAgent);
					
					if (rel == "Friendly" || rel == "Loyal" || rel == "Aligned")
					{
						agent.relationships.SetRel(interactingAgent, "Neutral");
					
						return false;
					}
				}
				else
					interactingAgent.objectMult.ObjectAction(agent.objectNetID, "ThreatenLeaveTown");

				return false;
			}

			return true;
		}
		public static bool AgentInteractions_ThreatenMayor(Agent agent, Agent interactingAgent) // Prefix
		{
			// Veiled Threats

			if (interactingAgent.statusEffects.hasTrait(cTrait.VeiledThreats) && !veiledThreatsUsed[agent])
			{
				if (!interactingAgent.gc.serverPlayer)
				{
					interactingAgent.objectMult.ObjectAction(agent.objectNetID, "ThreatenMayor");

					return false;
				}

				int myChance = agent.relationships.FindThreat(interactingAgent, true);

				if (agent.gc.percentChance(myChance))
				{
					for (int i = 0; i < agent.inventory.InvItemList.Count; i++)
					{
						InvItem invItem = agent.inventory.InvItemList[i];

						if (invItem.invItemName == "MayorHat")
						{
							agent.inventory.DestroyItem(invItem);
							agent.objectMult.DestroyInvItemByName(invItem);
							interactingAgent.inventory.AddItemAtEmptySlot(invItem, false, false);
							BMHeaderTools.SayDialogue(agent, "ThreatenedScared", vNameType.Dialogue);
							agent.gc.audioHandler.Play(interactingAgent, "SelectItem");
							agent.gc.ending.SetMayorTurnover("ThreatenedMayor");
						}
					}

					return false;
				}

				agent.StopInteraction();
				BMHeaderTools.SayDialogue(agent, cDialogue.VeiledThreatsAnnoyed, vNameType.Dialogue);
				agent.relationships.SetRel(interactingAgent, vRelationship.Annoyed);
				agent.relationships.SetRelHate(interactingAgent, 2);

				return false;
			}

			return true;
		}
		public static bool AgentInteractions_ThreatenMayorBadge(Agent agent, Agent interactingAgent, AgentInteractions __instance) // Prefix
		{
			// Veiled Threats

			if (interactingAgent.statusEffects.hasTrait(cTrait.VeiledThreats) && !veiledThreatsUsed[agent])
			{
				if (!interactingAgent.gc.serverPlayer)
				{
					interactingAgent.objectMult.ObjectAction(agent.objectNetID, "ThreatenMayorBadge");

					return false;
				}

				int myChance = agent.relationships.FindThreat(interactingAgent, true);

				if (agent.gc.percentChance(myChance))
				{
					for (int i = 0; i < agent.inventory.InvItemList.Count; i++)
					{
						InvItem invItem = agent.inventory.InvItemList[i];

						if (invItem.invItemName == "MayorBadge")
						{
							agent.inventory.DestroyItem(invItem);
							interactingAgent.inventory.AddItemAtEmptySlot(invItem, false, false);
							BMHeaderTools.SayDialogue(agent, "ThreatenedScared", vNameType.Dialogue);
							agent.gc.audioHandler.Play(interactingAgent, "SelectItem");

							for (int j = 0; j < agent.gc.agentList.Count; j++)
							{
								Agent agent2 = agent.gc.agentList[j];

								if (agent2.agentName == "Mayor" && agent2.startingChunkRealDescription == "MayorOffice")
								{
									__instance.BecomeFriendlyWithAlliesAllInChunk(agent, interactingAgent);

									break;
								}
							}
						}
					}

					return false;
				}

				agent.StopInteraction();
				BMHeaderTools.SayDialogue(agent, cDialogue.VeiledThreatsAnnoyed, vNameType.Dialogue);
				agent.relationships.SetRel(interactingAgent, vRelationship.Annoyed);
				agent.relationships.SetRelHate(interactingAgent, 2);

				return false;
			}

			return true;
		}
		public static bool AgentInteractions_ThreatenMoney(Agent agent, Agent interactingAgent, AgentInteractions __instance) // Prefix
		{
			// Veiled Threats

			if (interactingAgent.statusEffects.hasTrait(cTrait.VeiledThreats) && !veiledThreatsUsed[agent])
			{
				if (!interactingAgent.gc.serverPlayer)
				{
					interactingAgent.objectMult.ObjectAction(agent.objectNetID, "ThreatenMoney");

					return false;
				}

				int myChance = agent.relationships.FindThreat(interactingAgent, true);

				if (agent.gc.percentChance(myChance) || agent.relationships.GetRel(interactingAgent) == "Aligned" || agent.relationships.GetRel(interactingAgent) == "Submissive")
				{
					for (int i = 0; i < agent.inventory.InvItemList.Count; i++)
					{
						InvItem invItem = agent.inventory.InvItemList[i];
					
						if (invItem.invItemName == "Money")
						{
							agent.inventory.DestroyItem(invItem);
							interactingAgent.inventory.AddItemAtEmptySlot(invItem, false, false);
							BMHeaderTools.SayDialogue(agent, "ThreatenedScared", vNameType.Dialogue);
							agent.gc.audioHandler.Play(interactingAgent, "SelectItem");
						}
					}

					return false;
				}
				agent.StopInteraction();
				BMHeaderTools.SayDialogue(agent, cDialogue.VeiledThreatsAnnoyed, vNameType.Dialogue);
				agent.relationships.SetRel(interactingAgent, vRelationship.Annoyed);
				agent.relationships.SetRelHate(interactingAgent, 2);

				return false;
			}

			return true;
		}
		public static bool AgentInteractions_ThreatenSafeCombination(Agent agent, Agent interactingAgent, AgentInteractions __instance) // Prefix
		{
			// Veiled Threats

			if (interactingAgent.statusEffects.hasTrait(cTrait.VeiledThreats) && !veiledThreatsUsed[agent])
			{
				if (!interactingAgent.gc.serverPlayer)
				{
					interactingAgent.objectMult.ObjectAction(agent.objectNetID, "ThreatenSafeCombination");

					return false;
				}

				int myChance = agent.relationships.FindThreat(interactingAgent, true);

				if (agent.gc.percentChance(myChance))
				{
					for (int i = 0; i < agent.inventory.InvItemList.Count; i++)
					{
						InvItem invItem = agent.inventory.InvItemList[i];
				
						if (invItem.invItemName == "SafeCombination")
						{
							agent.inventory.DestroyItem(invItem);
							interactingAgent.inventory.AddItemAtEmptySlot(invItem, false, false);
						
							for (int j = 0; j < agent.gc.objectRealList.Count; j++)
								if (agent.gc.objectRealList[j].chestReal && agent.gc.objectRealList[j].startingChunk == agent.startingChunk && agent.gc.objectRealList[j].owner == agent.ownerID)
									agent.gc.objectRealList[j].allowedToSteal = true;
							
							if (agent.relationships.GetRel(interactingAgent) != "Aligned" && agent.relationships.GetRel(interactingAgent) != "Loyal")
								agent.relationships.SetRel(interactingAgent, "Submissive");
							
							__instance.BecomeSubmissiveWithAlliesAllInChunk(agent, interactingAgent);
							BMHeaderTools.SayDialogue(agent, "ThreatenedScared", vNameType.Dialogue);
							agent.gc.audioHandler.Play(interactingAgent, "SelectItem");
						}
					}

					return false;
				}

				agent.StopInteraction();
				BMHeaderTools.SayDialogue(agent, cDialogue.VeiledThreatsAnnoyed, vNameType.Dialogue);
				agent.relationships.SetRel(interactingAgent, vRelationship.Annoyed);
				agent.relationships.SetRelHate(interactingAgent, 2);

				return false;
			}

			return true;
		}
		public static void AgentInteractions_UseItemOnObject(Agent agent, Agent interactingAgent, InvItem item, int slotNum, string combineType, string useOnType, ref bool __result) // Postfix
		{
			BMLog("AgentInteractions_UseItemOnObject: " + item.invItemName);

			if (useOnType == "Hobo_Donate")
			{
				string itemName = item.invItemName;

				if (itemName == "Banana" || itemName == "BananaPeel" || itemName == "Beer" || itemName == "Cigarettes" || itemName == "Fud" || itemName == "Sugar" || itemName == "Whiskey")
					BMBehaviors.Hobo_AcceptDonation(agent, interactingAgent, item);
				else
				{
					BMHeaderTools.SayDialogue(agent, "Hobo_DontWant", vNameType.Dialogue);
					GC.audioHandler.Play(interactingAgent, "CantDo");
				}

				__result = true;
			}
		}
		#endregion
		#region ObjectMult
		public void ObjectMult_00()
		{
			Type t = typeof(ObjectMult);
			Type g = GetType();

			Prefix(t, "RemoveShakedownPerson", g, "ObjectMult_RemoveShakedownPerson", new Type[2] { typeof(Agent), typeof(int) });
		}
		public static bool ObjectMult_RemoveShakedownPerson(Agent shookDownAgent, int shakedownAmount, ObjectMult __instance) // Prefix
		{
			// Intercept ShakedownFail XP loss for Warlord if not Extortionist

			if (__instance.agent.statusEffects.hasTrait(cTrait.Warlord) && 
				!(__instance.agent.statusEffects.hasTrait(vTrait.Extortionist) || (__instance.agent.statusEffects.hasTrait(vTrait.Extortionist_2))))
				return false;

			return true;
		}
		#endregion
		#region Relationships
		public void Relationships_00()
		{
			Type t = typeof(Relationships);
			Type g = GetType();

			Postfix(t, "EnforcerAlert", g, "Relationships_EnforcerAlert", new Type[5] { typeof(Agent), typeof(float), typeof(Vector2), typeof(int), typeof(Agent) });
			Prefix(t, "OwnCheck", g, "Relationships_OwnCheck", new Type[7] { typeof(Agent), typeof(GameObject), typeof(int), typeof(string), typeof(bool), typeof(int), typeof(Fire) });
			Postfix(t, "SetupRelationshipOriginal", g, "Relationships_SetupRelationshipOriginal", new Type[1] { typeof(Agent) });
		}
		public static void Relationships_EnforcerAlert(Agent criminal, float noiseDist, Vector2 noisePos, int numStrikes, Agent victim, Relationships __instance) // Postfix
		{
			if (GC.challenges.Contains(cChallenge.PoliceState))
				if (__instance.GetRel(criminal) == vRelationship.Hostile)
					criminal.statusEffects.AddTrait(vTrait.Wanted);
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
			else if (BMTraits.isPlayerInitialRelationshipTraitActive && ___agent.isPlayer != 0)
			{
				if (__instance.GetRel(otherAgent) == vRelationship.Neutral)
				{
					string newRel = vRelationship.Neutral;
					bool enforcer = otherAgent.enforcer || otherAgent.statusEffects.hasTrait(vTrait.TheLaw) || vAgent.LawEnforcement.Contains(otherAgent.agentName);
					bool criminal = otherAgent.objectMultAgent.mustBeGuilty || vAgent.Criminal.Contains(otherAgent.agentName);

					if (___agent.statusEffects.hasTrait(cTrait.ObjectivelyUnpleasant) || 
						(___agent.statusEffects.hasTrait(cTrait.GenerallyUnpleasant) && GC.percentChance(20)) ||
						(___agent.statusEffects.hasTrait(cTrait.Priors) && enforcer) ||
						(___agent.statusEffects.hasTrait(cTrait.BootLicker) && criminal))
					{
						if (GC.percentChance(4))
							newRel = vRelationship.Hostile;
						else
							newRel = vRelationship.Annoyed;
					}
					
					if ((___agent.statusEffects.hasTrait(cTrait.Priors) && criminal) ||
						(___agent.statusEffects.hasTrait(cTrait.BootLicker) && enforcer))
						newRel = vRelationship.Friendly;
					
					if (___agent.statusEffects.hasTrait(cTrait.Polarizing))
					{
						if (GC.percentChance(50))
							newRel = vRelationship.Annoyed;
						else
							newRel = vRelationship.Friendly;
					}
					else if (___agent.statusEffects.hasTrait(cTrait.Polarizing_2))
					{
						int roll = Random.Range(1, 100);

						if (roll <= 25)
							newRel = vRelationship.Hostile;
						else if (roll <= 50)
							newRel = vRelationship.Annoyed;
						else if (roll <= 67)
							newRel = vRelationship.Friendly;
						else if (roll <= 88)
							newRel = vRelationship.Loyal;
						else if (roll <= 100)
							newRel = vRelationship.Aligned;
					}

					if ((___agent.statusEffects.hasTrait(cTrait.Domineering) && GC.percentChance(4)) || 
						(___agent.statusEffects.hasTrait(cTrait.Domineering_2) && GC.percentChance(8)))
						newRel = vRelationship.Submissive;

					if (otherAgent.agentName == vAgent.ResistanceLeader)
						newRel = vRelationship.Aligned;
					else if (otherAgent.agentName == vAgent.Mobster && ___agent.statusEffects.hasTrait(cTrait.MobDebt))
					{
						if (GC.levelTheme == 0 || GC.levelTheme == 1)
							newRel = vRelationship.Friendly;
						else if (GC.levelTheme == 2)
							newRel = vRelationship.Neutral;
						else if (GC.levelTheme == 3)
							newRel = vRelationship.Annoyed;
						else if (GC.levelTheme == 4 || GC.levelTheme == 5)
							newRel = vRelationship.Hostile;
					}

					if (newRel != vRelationship.Neutral)
					{
						__instance.SetRelInitial(otherAgent, newRel);
						otherAgent.relationships.SetRelInitial(___agent, newRel);

						if (newRel == vRelationship.Annoyed)
						{
							otherAgent.relationships.SetStrikes(___agent, 2);
							__instance.SetStrikes(otherAgent, 2);
						}
						else if (newRel == vRelationship.Hostile)
						{
							otherAgent.relationships.SetStrikes(___agent, 5);
							__instance.SetStrikes(otherAgent, 5);
						}
					}
				}
			}
		}
		#endregion
		#region StatusEffects
		public void StatusEffects_00()
		{
			Type t = typeof(StatusEffects);
			Type g = GetType();

			Postfix(t, "AddTrait", g, "StatusEffects_AddTrait", new Type[3] { typeof(string), typeof(bool), typeof(bool) });
			Postfix(t, "AgentIsRival", g, "StatusEffects_AgentIsRival", new Type[1] { typeof(Agent) });
			Prefix(t, "BecomeHidden", g, "StatusEffects_BecomeHidden", new Type[1] { typeof(ObjectReal) });
			Postfix(t, "BecomeNotHidden", g, "StatusEffects_BecomeNotHidden_Postfix", new Type[0]);
			Prefix(t, "ChangeHealth", g, "StatusEffects_ChangeHealth", new Type[6] { typeof(float), typeof(PlayfieldObject), typeof(NetworkInstanceId), typeof(float), typeof(string), typeof(byte) });
			Postfix(t, "RemoveTrait", g, "StatusEffects_RemoveTrait", new Type[2] { typeof(string), typeof(bool) });
		}
		public static void StatusEffects_AddTrait(string traitName, bool isStarting, bool justRefresh, StatusEffects __instance) // Postfix
		{
			// Fatass
			// Sniper

			Agent agent = __instance.agent;

			if (traitName == cTrait.Fatass)
			{
				agent.SetEndurance(agent.enduranceStatMod + 1);
				agent.SetSpeed(agent.speedStatMod - 1);
			}
			else if (traitName == cTrait.Sniper || traitName == cTrait.Sniper_2)
			{
				__instance.SpecialAbilityInterfaceCheck();
				// I think this is triggered once, and then continues as long as you have it.
			}
		}
		public static void StatusEffects_AgentIsRival(Agent myAgent, StatusEffects __instance, ref bool __result) // Postfix
		{
			// All custom Social traits with Rivalry & XP bonus

			Agent otherAgent = __instance.agent;

			if ((myAgent.statusEffects.hasTrait(cTrait.MobDebt) && otherAgent.agentName == vAgent.Mobster) ||
				(myAgent.statusEffects.hasTrait(cTrait.BootLicker) && (vAgent.Criminal.Contains(otherAgent.agentName) || otherAgent.objectMultAgent.mustBeGuilty)) ||
				(myAgent.statusEffects.hasTrait(cTrait.Priors) && (vAgent.LawEnforcement.Contains(otherAgent.agentName) || otherAgent.enforcer || otherAgent.statusEffects.hasTrait(vTrait.TheLaw))))
				__result = true;
		}
		public static bool StatusEffects_BecomeHidden(ObjectReal hiddenInObject, StatusEffects __instance) // Replacement
		{
			// Sniper/Doubletapper hidden object non-collision
			// Underdark Citizen Enemy Manhole unhide

			BMLog("StatusEffects_BecomeHidden");

			int a = 0;

			Agent agent = __instance.agent;
			agent.oma.hidden = true;
			string audio = vAudioClip.Hide;

				BMLog("\tA" + a++);

			if (!(hiddenInObject is null))
			{
				BMLog("\tB" + a++);

				string objName = hiddenInObject.name;
				agent.hiddenInObject = hiddenInObject;
				hiddenInObject.agentHiding = agent;
				string hide = "North";

				switch (objName)
				{
					case vObject.Bathtub:
						audio = vAudioClip.AgentJoin;
						hide = "Conditional";
						break;
					case vObject.Bush:
						audio = vAudioClip.Hide;
						hide = "North";
						break;
					case vObject.Plant:
						audio = vAudioClip.Hide;
						hide = "South";
						break;
					case vObject.PoolTable:
						audio = vAudioClip.AgentKnockOut;
						hide = "North";
						break;
					case vObject.TableBig:
						audio = vAudioClip.AgentKnockout2;
						hide = "North";
						break;
					case vObject.TrashCan:
						audio = vAudioClip.Hoist;
						hide = "South";
						break;
				}
				
				if (hide == "North")
					agent.tr.position = new Vector2(hiddenInObject.tr.position.x, hiddenInObject.tr.position.y + 0.24f);
				else if (hide == "South")
					agent.tr.position = new Vector2(hiddenInObject.tr.position.x, hiddenInObject.tr.position.y - 0.24f);
				else if (hide == "Conditional")
				{
					agent.tr.position = new Vector2(hiddenInObject.tr.position.x, hiddenInObject.tr.position.y + 0.24f);
					// Placeholder, detect walls here and do case-by-case
				}

				agent.rb.velocity = Vector2.zero;

				BMLog("\tB" + a++);

				if (objName == vObject.Manhole || objName == vObject.Plant || objName == vObject.PoolTable || objName == vObject.TrashCan || objName == vObject.Bathtub || objName == vObject.TableBig)
				{
					agent.EnableHitboxes(false);
					agent.agentItemColliderTr.gameObject.SetActive(false);
				}
			}

			BMLog("\tA" + a++);

			if (!GC.consoleVersion)
				agent.EnableMouseboxes(false);

			BMLog("\tA" + a++);

			agent.agentHitboxScript.shadow.GetComponent<MeshRenderer>().enabled = false;
			agent.SetInvisible(true);
			agent.objectSprite.RefreshRenderer();
			agent.objectMult.BecomeHidden();

			GC.audioHandler.Play(agent, audio);

			BMLog("\tA" + a++);

			if (GC.loadComplete)
				for (int i = 0; i < GC.activeBrainAgentList.Count; i++)
				{
					Agent iAgent = GC.activeBrainAgentList[i];

					if (iAgent != agent)
					{
						Relationship relationship = iAgent.relationships.GetRelationship(agent);
					
						if (relationship.HasLOS(""))
							relationship.sawBecomeHidden = true;
					}
				}

			BMLog("\tA" + a++);

			if (agent.isPlayer != 0 && agent.localPlayer)
			{
				agent.blockWalking = true;
				__instance.StartCoroutine(StatusEffects_WaitForAgentUnhide(__instance));
			}

			BMLog("\tA" + a++);

			if (!(hiddenInObject is null))
				if (BMTraits.IsPlayerTraitActive(cTrait.UnderdarkCitizen) && agent.isPlayer == 0 && hiddenInObject.objectName == vObject.Manhole)
					agent.statusEffects.BecomeNotHidden();

			//if (hiddenInObject is Bathtub || hiddenInObject is Plant || hiddenInObject is PoolTable || hiddenInObject is TableBig)
			//	agent.agentCollider.enabled = false;

			return false;
		}
		public static void StatusEffects_BecomeNotHidden_Postfix(StatusEffects __instance) // Postfix
		{
			__instance.agent.agentCollider.enabled = true;
			__instance.agent.EnableHitboxes(true);
		}
		public static bool StatusEffects_ChangeHealth(float healthNum, PlayfieldObject damagerObject, NetworkInstanceId cameFromClient, float clientFinalHealthNum, string damagerObjectName, byte extraVar, StatusEffects __instance, ref HealthBar ___healthBar) // Replacement
		{
			// Warlord submission

			Agent hurtAgent = __instance.agent;

			if ((hurtAgent.teleporting && !hurtAgent.skillPoints.justGainedLevel) ||
				(hurtAgent.ghost && hurtAgent.skillPoints.justGainedLevel) ||
				(hurtAgent.dead && hurtAgent.skillPoints.justGainedLevel && !hurtAgent.teleporting && !hurtAgent.suicided && !hurtAgent.finishedLevel && !hurtAgent.finishedLevelImmediate && !hurtAgent.finishedLevelImmediateMult && !hurtAgent.finishedLevelRealMult && !hurtAgent.oma.finishedLevel) ||
				((hurtAgent.finishedLevel || hurtAgent.finishedLevelImmediate || hurtAgent.finishedLevelImmediateMult || hurtAgent.finishedLevelRealMult || hurtAgent.oma.finishedLevel) && !hurtAgent.suicided && healthNum < 0f) ||
				(hurtAgent.butlerBot || hurtAgent.hologram || (hurtAgent.mechEmpty && healthNum < 0f)) || 
				(GC.cinematic && GC.loadLevel.LevelContainsMayor()))
				return false;
			
			float dmg = healthNum;
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

				if ((hurtAgent.oma.superSpecialAbility || __instance.hasTrait(vTrait.Juggernaut)) && hurtAgent.chargingForward && !hurtAgent.suiciding && !hurtAgent.suicided)
					healthNum = (float)((int)(healthNum * 0.4f));
				
				if (hurtAgent.oma.hidden && !__instance.GotStatusEffectDamage())
					__instance.BecomeNotHidden();
				
				if (damagerObject != null)
				{
					flag2 = true;
					__instance.ExitBox(true);

					if (!hurtAgent.oma.superSpecialAbility && !__instance.hasTrait(vTrait.Slinky))
						__instance.RemoveInvisibleLimited();
				}

				if (hurtAgent.isPlayer > 0)
					GC.playerControl.Vibrate(hurtAgent.isPlayer, Mathf.Clamp(-healthNum / 100f + 0.05f, 0f, 0.25f), Mathf.Clamp(-healthNum / 132f + 0.05f, 0f, 0.2f));
				
				if (hurtAgent.justHitByAgent2 != null && hurtAgent.justHitByAgent2 != hurtAgent)
				{
					Relationship relationship = hurtAgent.relationships.GetRelationship(hurtAgent.justHitByAgent2);
					Relationship relationship2 = hurtAgent.justHitByAgent2.relationships.GetRelationship(hurtAgent);

					if (relationship != null && relationship2 != null)
					{
						if (!hurtAgent.justHitByAgent2.objectAgent && hurtAgent.deathMethod != "Bite" && !hurtAgent.oma.mindControlled && !hurtAgent.justHitByAgent2.oma.mindControlled)
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
								if (hurtAgent.isPlayer > 0 && hurtAgent.justHitByAgent2.isPlayer > 0 && !GC.pvp)
								{
									healthNum = 0f;
									flag = true;
								}
								else
								{
									if ((relationship.relTypeCode == relStatus.Aligned || relationship.relTypeCode == relStatus.Loyal) && hurtAgent.isPlayer == 0 && !hurtAgent.dead && healthNum != -200f)
									{
										healthNum = (float)Mathf.Clamp((int)(healthNum / 3f), -1000, -1);
										flag3 = true;
									}
									if ((relationship2.relTypeCode == relStatus.Aligned || relationship2.relTypeCode == relStatus.Loyal) && hurtAgent.justHitByAgent2.isPlayer == 0 && !hurtAgent.dead && healthNum != -200f && !flag3)
										healthNum = (float)Mathf.Clamp((int)(healthNum / 3f), -1000, -1);
								}
							}
						}

						if (!hurtAgent.suicided)
						{
							if (__instance.hasTrait("ChanceAttacksDoZeroDamage2"))
							{
								int myChance = hurtAgent.DetermineLuck(10, vTrait.UnCrits, true);

								if (GC.percentChance(myChance))
									healthNum = 0f;
							}
							else if (__instance.hasTrait(vTrait.UnCrits))
							{
								int myChance2 = hurtAgent.DetermineLuck(5, vTrait.UnCrits, true);

								if (GC.percentChance(myChance2))
									healthNum = 0f;
							}
						}

						if (GC.serverPlayer)
						{
							if (hurtAgent.hasStealingFromAgent && hurtAgent.justHitByAgent2 == hurtAgent.stealingFromAgent)
								hurtAgent.SetDefaultGoal(vGoal.WanderFar);
							
							if (healthNum < 0f && (relationship.relTypeCode == relStatus.Aligned || relationship.relTypeCode == relStatus.Submissive || (relationship.relTypeCode == relStatus.Loyal && hurtAgent.hasEmployer)) && !hurtAgent.oma.bodyGuarded && hurtAgent.deathMethod != "Poison")
							{
								relationship.damageDone += -(int)healthNum;

								if (!hurtAgent.justHitByAgent2.objectAgent)
								{
									if (hurtAgent.justHitByAgent2.mostRecentGoalCode == goalType.Battle)
										relationship.hitNumberOfTimesInCombat++;
									else
										relationship.hitNumberOfTimes++;
								}

								if ((relationship.damageDone >= 10 && (relationship.hitNumberOfTimes > 4 || relationship.hitNumberOfTimesInCombat > 12) && hurtAgent.isPlayer == 0 && (hurtAgent.isPlayer != 0 || hurtAgent.justHitByAgent2.isPlayer != 0)) || (hurtAgent.killForQuest != null && hurtAgent.isPlayer == 0 && (hurtAgent.justHitByAgent2.isPlayer != 0 || hurtAgent.justHitByAgent2.hasEmployer)) || (hurtAgent.isBigQuestObject && (hurtAgent.bigQuestType == "Gangbanger" || hurtAgent.bigQuestType == "GangbangerB" || hurtAgent.bigQuestType == "Vampire" || hurtAgent.bigQuestType == "Assassin" || hurtAgent.bigQuestType == "MechPilot")) || (hurtAgent.oma.didAsk && (hurtAgent.justHitByAgent2.isPlayer != 0 || hurtAgent.justHitByAgent2.hasEmployer)) || hurtAgent.justHitByAgent2.oma.mindControlled)
								{
									hurtAgent.relationships.SetRel(hurtAgent.justHitByAgent2, "Hateful");
									hurtAgent.relationships.SetRelHate(hurtAgent.justHitByAgent2, 5);

									if (hurtAgent.justHitByAgent2.oma.mindControlled)
									{
										hurtAgent.justHitByAgent2.relationships.SetRel(hurtAgent, "Hateful");
										hurtAgent.justHitByAgent2.relationships.SetRelHate(hurtAgent, 5);
									}

									if (hurtAgent.justHitByAgent2.isPlayer > 0)
										hurtAgent.justHitByAgent2.relationships.SetRel(hurtAgent, "Hateful");
									
									if (!hurtAgent.isMayor)
									{
										List<Agent> agentList = GC.agentList;

										for (int i = 0; i < agentList.Count; i++)
										{
											Agent iAgent = agentList[i];

											if (Vector2.Distance(iAgent.curPosition, hurtAgent.curPosition) < iAgent.LOSRange / hurtAgent.hardToSeeFromDistance)
											{
												relStatus relCode = iAgent.relationships.GetRelCode(hurtAgent);

												if (relCode == relStatus.Aligned && iAgent.movement.HasLOSObject360(hurtAgent))
												{
													iAgent.relationships.SetRel(hurtAgent.justHitByAgent2, "Hateful");
													iAgent.relationships.SetRelHate(hurtAgent.justHitByAgent2, 5);

													if (hurtAgent.justHitByAgent2.isPlayer > 0)
														hurtAgent.justHitByAgent2.relationships.SetRel(iAgent, "Hateful");
												}

												if (relCode == relStatus.Loyal)
												{
													relStatus relCode2 = iAgent.relationships.GetRelCode(hurtAgent.justHitByAgent2);

													if (relCode2 != relStatus.Aligned && relCode2 != relStatus.Loyal && iAgent.movement.HasLOSObject360(hurtAgent))
													{
														iAgent.relationships.SetRel(hurtAgent.justHitByAgent2, "Hateful");
														iAgent.relationships.SetRelHate(hurtAgent.justHitByAgent2, 5);
													
														if (hurtAgent.justHitByAgent2.isPlayer > 0)
															hurtAgent.justHitByAgent2.relationships.SetRel(iAgent, "Hateful");
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
					if (hurtAgent.isPlayer > 0 && hurtAgent.localPlayer)
					{
						float f = healthNum;

						if (num2 != 0f)
							f = num2;
						
						GC.alienFX.PlayerHurt(hurtAgent);
						
						if (hurtAgent.critted && (!hurtAgent.dead || hurtAgent.fakeDead))
							GC.spawnerMain.SpawnStatusText(hurtAgent, "PlayerHealthDownCrit", Mathf.Abs(f).ToString(), cameFromClient);
						else if (!hurtAgent.dead || hurtAgent.fakeDead)
							GC.spawnerMain.SpawnStatusText(hurtAgent, "PlayerHealthDown", Mathf.Abs(f).ToString(), cameFromClient);
					}
					else
					{
						float f2 = healthNum;

						if (clientFinalHealthNum != -999f)
							f2 = clientFinalHealthNum;
						
						if (hurtAgent.critted && (!hurtAgent.dead || hurtAgent.fakeDead))
							GC.spawnerMain.SpawnStatusText(hurtAgent, "NPCHealthDownCrit", Mathf.Abs(f2).ToString(), cameFromClient);
						else if (!hurtAgent.dead || hurtAgent.fakeDead)
							GC.spawnerMain.SpawnStatusText(hurtAgent, "NPCHealthDown", Mathf.Abs(f2).ToString(), cameFromClient);
					}
				}

				hurtAgent.critted = false;

				if (hurtAgent.health + healthNum > 0f || hurtAgent.dead)
				{
					bool flag5 = false;

					if (flag2 && damagerObject.bulletType == bulletStatus.GhostBlaster)
					{
						hurtAgent.objectSprite.Flash("GhostBlaster");

						flag5 = true;
					}

					if (!flag5)
					{
						hurtAgent.objectSprite.Flash();

						if (GC.multiplayerMode && flag2 && damagerObjectName != "Bullet" && damagerObjectName != "Melee" && damagerObjectName != "Explosion")
							hurtAgent.objectMult.AgentFlash();
					}
				}

				if (GC.levelType == "Tutorial" && hurtAgent.isPlayer > 0)
				{
					GC.tutorial.LoseHealth();
				
					if (hurtAgent.health < 0f)
						hurtAgent.health = 1f;
				}

				if (hurtAgent.isPlayer > 0)
					__instance.timeSinceLastDamaged = 0f;
				
				if (hurtAgent.arenaBattler && !hurtAgent.arenaBattleStarted && GC.serverPlayer)
				{
					hurtAgent.arenaBattleStarted = true;

					for (int j = 0; j < hurtAgent.gc.objectRealList.Count; j++)
					{
						ObjectReal objectReal = hurtAgent.gc.objectRealList[j];
					
						if (objectReal.startingChunk == hurtAgent.startingChunk && objectReal.objectName == "EventTriggerFloor")
						{
							EventTriggerFloor eventTriggerFloor = (EventTriggerFloor)objectReal;
						
							if (eventTriggerFloor.triggerState == "Initial")
								eventTriggerFloor.triggerState = "FightsOver";
						}
					}
				}

				if (hurtAgent.challengedToFight > 0 && hurtAgent.justHitByAgent2 != null)
				{
					if (hurtAgent.justHitByAgent2.isPlayer != 0)
					{
						if (hurtAgent.challengedToFight == 1 || (hurtAgent.challengedToFight == 2 && (damagerObjectName == "Bullet" || damagerObjectName == "Explosion" || damagerObjectName == "Fire")))
						{
							hurtAgent.challengedToFight = 0;
							hurtAgent.challengedToFightAgentID = 0;
							hurtAgent.challengedToFightAgent = null;
							hurtAgent.SetFollowing(null);
							BMHeaderTools.SayDialogue(hurtAgent, "NotFair", vNameType.Dialogue);
						}
					}
					else
					{
						relStatus relCode3 = hurtAgent.justHitByAgent2.relationships.GetRelCode(hurtAgent.challengedToFightAgent);

						if (relCode3 == relStatus.Aligned || relCode3 == relStatus.Loyal)
						{
							hurtAgent.challengedToFight = 0;
							hurtAgent.challengedToFightAgentID = 0;
							hurtAgent.challengedToFightAgent = null;
							hurtAgent.SetFollowing(null);
							BMHeaderTools.SayDialogue(hurtAgent, "NotFair", vNameType.Dialogue);
						}
					}
				}
				if (hurtAgent.localPlayer && hurtAgent.isPlayer != 0)
				{
					if (hurtAgent.inventory.HasItem("CourierPackage") && !__instance.hasTrait("NoBrokenPackages") && (!hurtAgent.oma.superSpecialAbility || !(hurtAgent.agentName == "Courier")) && !__instance.GotNonHitDamage() && hurtAgent.deathMethod != "FellInHole")
					{
						hurtAgent.courierPackageDamage -= healthNum;

						if (hurtAgent.courierPackageDamage >= 20f)
						{
							hurtAgent.inventory.DestroyItem(hurtAgent.inventory.FindItem("CourierPackage"));
							InvItem invItem = new InvItem();
							invItem.invItemName = "CourierPackageBroken";
							invItem.SetupDetails(false);
							GC.audioHandler.Play(hurtAgent, "CourierPackageBreak");
							GC.spawnerMain.SpawnStatusText(hurtAgent, "OutOfAmmo", "CourierPackage", "Item");
							hurtAgent.inventory.DontPlayPickupSounds(true);
							hurtAgent.inventory.AddItemOrDrop(invItem);
							hurtAgent.inventory.DontPlayPickupSounds(false);
						}
					}
				
					if (hurtAgent.mindControlling && !__instance.GotNonHitDamage())
						hurtAgent.relationships.StopAgentsUnderMindControl();
				}
			}
			else if (healthNum > 0f)
			{
				if (__instance.zombieResurrecting || healthNum == 1000f)
					flag = true;
				
				if (!flag)
				{
					if (healthNum == hurtAgent.healthMax && !hurtAgent.finishedLevel)
					{
						if (hurtAgent.skillPoints.justGainedLevel)
							GC.spawnerMain.SpawnStatusText(hurtAgent, "HealthUpSlower", "FullHealth", "StatusEffect", cameFromClient);
						else
						{
							GC.alienFX.GainHealth(hurtAgent);
							GC.spawnerMain.SpawnStatusText(hurtAgent, "HealthUp", "FullHealth", "StatusEffect", cameFromClient);
						}
					}
					else if (!hurtAgent.finishedLevel)
					{
						GC.alienFX.GainHealth(hurtAgent);
						GC.spawnerMain.SpawnStatusText(hurtAgent, "HealthUp", healthNum.ToString(), cameFromClient);
					}
				}
			}

			if (healthNum <= 0f && (!__instance.GotStatusEffectDamage() || hurtAgent.health + healthNum <= 0f))
			{
				if (hurtAgent.isPlayer == 0 && hurtAgent.interactingAgent != null)
					hurtAgent.StopInteraction();
				
				if (__instance.hasStatusEffect(vStatusEffect.Dizzy))
					__instance.RemoveStatusEffect(vStatusEffect.Dizzy);
			
				if (__instance.hasStatusEffect(vStatusEffect.Electrocuted))
					__instance.RemoveStatusEffect(vStatusEffect.Electrocuted);
			}

			if (clientFinalHealthNum != -999f)
			{
				if (clientFinalHealthNum > 0f && hurtAgent.health < 0f)
					hurtAgent.health = 0f;
				
				hurtAgent.health += clientFinalHealthNum;
			}
			else
				hurtAgent.health += healthNum;
			
			bool flag6 = false;
			
			if (hurtAgent.oma.bodyGuarded && hurtAgent.employer != null && hurtAgent.employer.oma.superSpecialAbility && hurtAgent.employer.agentName == vAgent.Bouncer)
				flag6 = true;
			
			if (hurtAgent.oma.bodyGuarded && hurtAgent.bodyguardedWantsRevenge && (hurtAgent.health <= hurtAgent.healthMax * 0.3f || flag6) && hurtAgent.health > 0f && hurtAgent.isPlayer == 0 && GC.serverPlayer && hurtAgent.justHitByAgent2 != null && (hurtAgent.justHitByAgent2.isPlayer != 0 || hurtAgent.justHitByAgent2.hasEmployer))
				hurtAgent.BodyguardedSorryMutiny();
			
			if (hurtAgent.health <= hurtAgent.healthMax * 0.4f && hurtAgent.health > 0f && hurtAgent.isPlayer == 0 && GC.serverPlayer && hurtAgent.CanShakeDown() && hurtAgent.justHitByAgent2 != null && (hurtAgent.justHitByAgent2.isPlayer != 0 || hurtAgent.justHitByAgent2.hasEmployer))
			{
				int k = 0;

				while (k < GC.playerAgentList.Count)
				{
					Agent shakedowningAgent = GC.playerAgentList[k];
					bool canMakeSubmissive = false;

					if (hurtAgent.justHitByAgent2.isPlayer != 0 && hurtAgent.justHitByAgent2 != shakedowningAgent && !hurtAgent.justHitByAgent2.statusEffects.hasTrait(vTrait.Extortionist) && !hurtAgent.justHitByAgent2.statusEffects.hasTrait(vTrait.Extortionist_2) && Vector2.Distance(shakedowningAgent.tr.position, hurtAgent.justHitByAgent2.tr.position) < 13.44f)
						canMakeSubmissive = true;
					else if (hurtAgent.justHitByAgent2.isPlayer == 0 && !hurtAgent.justHitByAgent2.hasEmployer)
					{
						relStatus relCode4 = hurtAgent.justHitByAgent2.relationships.GetRelCode(shakedowningAgent);
					
						if ((relCode4 == relStatus.Aligned || relCode4 == relStatus.Loyal) && Vector2.Distance(shakedowningAgent.tr.position, hurtAgent.justHitByAgent2.tr.position) < 13.44f)
							canMakeSubmissive = true;
					}

					if (shakedowningAgent.statusEffects.hasTrait(cTrait.Warlord))
						canMakeSubmissive = true;

					if ((shakedowningAgent == hurtAgent.justHitByAgent2 || shakedowningAgent == hurtAgent.justHitByAgent2.employer || canMakeSubmissive) && 
						(shakedowningAgent.statusEffects.hasTrait(vTrait.Extortionist) || shakedowningAgent.statusEffects.hasTrait(vTrait.Extortionist_2)))
					{
						hurtAgent.relationships.SetRel(shakedowningAgent, vRelationship.Submissive);
						hurtAgent.agentInteractions.BecomeSubmissiveWithAlliesAllInChunk(hurtAgent, shakedowningAgent);
						int num3 = 0;

						for (int l = 0; l < GC.agentList.Count; l++)
							if (GC.agentList[l].oma.shookDown)
								num3++;
						
						int shakedownAmount = shakedowningAgent.objectMult.FindShakedownAmount(num3);
						shakedowningAgent.objectMult.AddShakedownPerson(hurtAgent, shakedownAmount, true);
						hurtAgent.oma.shookDown = true;
						hurtAgent.shookDownAgent = shakedowningAgent;

						if (num3 >= 3)
						{
							BMHeaderTools.SayDialogue(hurtAgent, "ThreatenedShakedownLowCash", vNameType.Dialogue);
						
							break;
						}

						BMHeaderTools.SayDialogue(hurtAgent, "SubmitShakedowner", vNameType.Dialogue);

						break;
					}
					else if ((shakedowningAgent == hurtAgent.justHitByAgent2 || shakedowningAgent == hurtAgent.justHitByAgent2.employer || canMakeSubmissive) && 
						shakedowningAgent.statusEffects.hasTrait(cTrait.Warlord))
					{
						hurtAgent.relationships.SetRel(shakedowningAgent, vRelationship.Submissive);

						BMHeaderTools.SayDialogue(hurtAgent, cDialogue.WarlordSubmission, vNameType.Dialogue);

						break;
					}
					else
						k++;
				}
			}

			bool flag8 = false;

			if (cameFromClient != NetworkInstanceId.Invalid && healthNum < 0f && hurtAgent.isPlayer == 0)
			{
				Agent component = NetworkServer.FindLocalObject(cameFromClient).GetComponent<Agent>();
				hurtAgent.justHitByAgent3 = true;

				if (!component.invisible)
					hurtAgent.relationships.AddRelHate(component, Mathf.Clamp((int)Mathf.Abs(healthNum), 5, 200));

				hurtAgent.justHitByAgent3 = false;

				if (extraVar == 1)
					flag8 = true;
			}

			if (hurtAgent.deathMethod == "Cyanide") { } // Sic
			
			if (!flag || healthNum != 0f)
				hurtAgent.objectMult.ChangeHealth(dmg, damagerObject, cameFromClient, healthNum);
			
			hurtAgent.SetBrainActive(true);
			
			if (__instance.hasTrait(vTrait.ImOuttie) || __instance.hasTrait(vTrait.ImOuttie_2))
				hurtAgent.FindSpeed();
			
			if (hurtAgent.health > hurtAgent.healthMax && !__instance.hasTrait("Supercharge"))
				hurtAgent.health = hurtAgent.healthMax;
			
			if (hurtAgent.health <= hurtAgent.healthMax && __instance.hasTrait("Supercharge"))
				__instance.RemoveTrait("Supercharge");
			
			if (hurtAgent.isPlayer > 0 && hurtAgent.localPlayer)
				___healthBar.StartChange();
			
			if (hurtAgent.mechEmpty)
			{
				try
				{
					GC.sessionData.mechHealth[hurtAgent.mechTiedToPlayer] = (int)hurtAgent.health;
				}
				catch { }
			}

			if ((hurtAgent.isPlayer == 0 || !hurtAgent.localPlayer) && hurtAgent.agentHealthBar != null)
				hurtAgent.agentHealthBar.ChangeHealth();
			
			if (hurtAgent.isPlayer > 0 && hurtAgent.localPlayer && (!hurtAgent.dead || hurtAgent.fakeDead) && healthNum != -200f && healthNum < 0f)
				GC.stats.AddToStat(hurtAgent, "DamageTaken", -(int)healthNum);
			
			if (healthNum < 0f)
				__instance.UseQuickEscapeTeleporter(false);
			
			bool flag9 = true;
			bool flag10 = false;

			if (GC.multiplayerMode)
			{
				if (hurtAgent.isPlayer != 0 && 
					!hurtAgent.localPlayer)
					flag9 = false;
				
				if (!GC.serverPlayer && 
					hurtAgent.isPlayer == 0)
				{
					flag9 = true;
					flag10 = true;
				}
			}

			bool flag11 = false;
			bool flag12 = false;
			
			if (flag9)
			{
				if (hurtAgent.dead && hurtAgent.fakeDead)
					flag11 = false;
				else if (hurtAgent.dead)
					flag11 = true;
				
				if (hurtAgent.ghost && hurtAgent.isPlayer > 0 && hurtAgent.health > -40f)
					flag11 = true;
			}

			if (hurtAgent.sleepKnockedOut && hurtAgent.agentHitboxScript.wholeBodyAnimType == "Sleep")
				hurtAgent.agentHitboxScript.PlayWholeBodyAnim("Dead");
			
			if (hurtAgent.health <= 0f && !flag11 && flag9 && !hurtAgent.mechEmpty && hurtAgent.agentName != "MechEmpty")
			{
				if (hurtAgent.mechFilled && !hurtAgent.suiciding && !hurtAgent.suicided)
				{
					if (!hurtAgent.fellInHole && !__instance.clientMechTransforming)
					{
						if (GC.serverPlayer)
						{
							__instance.MechTransformBackStart(true);
							hurtAgent.objectMult.SpecialAbility("MechTransformBackDead", null);
			
							return false;
						}

						hurtAgent.objectMult.SpecialAbility("MechTransformBackStartDead", null);
						__instance.clientMechTransforming = true;
					}

					return false;
				}

				bool flag13 = false;

				if (flag2 && damagerObject.isBullet && damagerObject.playfieldObjectBullet.rubber)
					flag13 = true;
				
				if (extraVar == 2 || flag13)
				{
					hurtAgent.knockedOut = true;
					hurtAgent.knockedOutLocal = true;
					flag13 = true;
				}
				
				if (hurtAgent.oma.bodyGuarded)
				{
					hurtAgent.zombified = false;
					hurtAgent.zombieWhenDead = false;
				
					if (!__instance.hasStatusEffect(vStatusEffect.Resurrection) && !hurtAgent.fellInHoleWhileDead)
						GC.quests.AddBigQuestPoints(hurtAgent.employer, hurtAgent, "BodyguardKill");
				}

				if ((hurtAgent.KnockedOut() || hurtAgent.arrested || (hurtAgent.FellInHole() && !hurtAgent.oma.bodyGuarded)) && __instance.hasStatusEffect(vStatusEffect.Resurrection) && hurtAgent.isPlayer == 0)
				{
					if (hurtAgent.FellInHole())
						__instance.RemoveStatusEffect(vStatusEffect.Resurrection, false);
					else
						__instance.RemoveStatusEffect(vStatusEffect.Resurrection);
				}

				if (__instance.hasStatusEffect(vStatusEffect.Resurrection) && hurtAgent.possessing)
					for (int m = 0; m < __instance.StatusEffectList.Count; m++)
						if (__instance.StatusEffectList[m].statusEffectName == vStatusEffect.Resurrection && !__instance.StatusEffectList[m].infiniteTime)
						{
							__instance.RemoveStatusEffect(vStatusEffect.Resurrection);

							break;
						}

				if ((__instance.hasStatusEffect(vStatusEffect.Resurrection) || __instance.hasStatusEffect("ExtraLife") || hurtAgent.possessing) && !GC.mainGUI.menuGUI.endedGame && !flag10 && !hurtAgent.suicided)
					hurtAgent.resurrect = true;
				
				if (!hurtAgent.resurrect)
				{
					if (!hurtAgent.zombified && hurtAgent.zombieWhenDead && hurtAgent.isPlayer == 0 && !hurtAgent.inhuman && hurtAgent.agentName != "Alien" && !hurtAgent.FellInHole())
					{
						if (hurtAgent.justHitByAgent2 != null && hurtAgent.justHitByAgent2.zombified)
							hurtAgent.zombifiedByAgent = hurtAgent.justHitByAgent2;
						
						hurtAgent.zombified = true;
						hurtAgent.resurrect = true;
						
						if (hurtAgent.nonQuestObjectMarker != null)
						{
							bool flag14 = true;
						
							for (int n = 0; n < GC.playerAgentList.Count; n++)
								if (GC.playerAgentList[n].agentName == "Zombie")
									flag14 = false;
							
							if (flag14)
								Object.Destroy(hurtAgent.nonQuestObjectMarker.gameObject);
						}
					}

					if (hurtAgent.zombified && hurtAgent.isPlayer != 0 && !hurtAgent.suicided)
						if (!hurtAgent.resurrect && hurtAgent.justHitByAgent2 != null && !hurtAgent.zombified && hurtAgent.justHitByAgent2.statusEffects.hasTrait("ResurrectionAllies") && !hurtAgent.justHitByAgent2.zombified && GC.percentChance(30))
							hurtAgent.resurrect = true;
				}
				else if (hurtAgent.zombified && hurtAgent.employer != null)
					hurtAgent.zombifiedByAgent = hurtAgent.employer;
				
				if (hurtAgent.transforming && hurtAgent.isPlayer > 0 && !flag10 && !hurtAgent.mechFilled && !__instance.hasSpecialAbility("MechTransform") && !hurtAgent.wasTransforming)
				{
					Debug.Log("Remove Werewolf (Dead)");
				
					if (GC.serverPlayer)
					{
						__instance.RemoveStatusEffect("WerewolfEffect");
						hurtAgent.objectMult.SpecialAbility("WerewolfTransformBackDead", null);
					}
					else
						hurtAgent.objectMult.SpecialAbility("WerewolfTransformBackStartDead", null);
				}

				if (!flag2 && !hurtAgent.dead && !hurtAgent.KnockedOut() && hurtAgent.deathMethod == "" && hurtAgent.deathKiller == "")
				{
					hurtAgent.deathMethod = "";
					hurtAgent.deathKiller = "";
				}
				
				hurtAgent.movement.StopChargingForward();
				
				if (hurtAgent.isPlayer > 0)
					__instance.RechargeSpecialAbilityImmediate();
				
				if (hurtAgent.KnockedOut())
				{
					if (!GC.audioHandler.soundsPlaying.Contains("ChloroformAgent") && !hurtAgent.arrested)
					{
						if (flag13)
							GC.audioHandler.Play(hurtAgent, vAudioClip.AgentDie);
						else
							GC.audioHandler.Play(hurtAgent, vAudioClip.AgentKnockOut);
					}
					if (!hurtAgent.arrested && !flag10 && !hurtAgent.FellInHole() && !hurtAgent.disappeared)
						GC.spawnerMain.SpawnStateIndicator(hurtAgent, "DizzyStars");
					
					hurtAgent.health = 0f;
					
					if (hurtAgent.sleeping)
						hurtAgent.sleepKnockedOut = true;
					
					if ((GC.coopMode || GC.fourPlayerMode) && hurtAgent.isPlayer > 0)
					{
						for (int num8 = 0; num8 < GC.playerAgentList.Count; num8++)
						{
							Agent agent7 = GC.playerAgentList[num8];

							if (!(agent7 != hurtAgent) || (agent7.dead && !agent7.resurrect) || agent7.ghost || !agent7.fakeDead) { } // Sic
						}
					}
				}
				else
				{
					if (hurtAgent.agentName == vAgent.Robot)
						GC.audioHandler.Play(hurtAgent, vAudioClip.RobotDeath);
					else if (!hurtAgent.FellInHole() || hurtAgent.oma.bodyGuarded)
						GC.audioHandler.Play(hurtAgent, vAudioClip.AgentDie);
					
					if (hurtAgent.isPlayer != 0)
					{
						GC.audioHandler.Play(hurtAgent, vAudioClip.PlayerDeath);
						__instance.playedPlayerDeath = true;
					}
				}

				if (GC.levelType == vLevelType.Tutorial)
					GC.tutorial.KillAgent(hurtAgent);
				
				if (cameFromClient == NetworkInstanceId.Invalid && flag2 && damagerObject.CompareTag("Bullet") && !damagerObject.nonLethal && hurtAgent.tickEndDamage == 0)
				{
					hurtAgent.rb.velocity = Vector2.zero;
					hurtAgent.rb.angularVelocity = 0f;
					hurtAgent.movement.KnockBackBullet(damagerObject.go, Mathf.Abs(healthNum) * 30f, false, damagerObject);
				}

				__instance.ExitBox(true);
				__instance.RemoveInvisibleLimited();
				bool flag15 = false;
				
				if (hurtAgent.justHitByAgent2 && hurtAgent.justHitByAgent2.agentName == "Custom")
				{
					hurtAgent.deathKiller = hurtAgent.justHitByAgent2.agentRealName;
					flag15 = true;
				}
				
				if (!flag15 && hurtAgent.killedByAgentIndirect != null && hurtAgent.killedByAgentIndirect.agentName == "Custom")
					hurtAgent.deathKiller = hurtAgent.killedByAgentIndirect.agentRealName;
				
				Agent mindControlAgent = null;
				
				if (hurtAgent.oma.mindControlled)
					mindControlAgent = hurtAgent.mindControlAgent;
				
				if ((!GC.coopMode && !GC.fourPlayerMode && !GC.multiplayerMode) || hurtAgent.isPlayer == 0 || hurtAgent.resurrect)
				{
					if (!GC.serverPlayer)
						hurtAgent.killedOnClient = true;
				
					__instance.SetupDeath(damagerObject, flag10);
					hurtAgent.pathing = 0;
					hurtAgent.movement.PathStop();
					hurtAgent.SetBrainActive(false);
					hurtAgent.brainUpdate.slowAIWait = 0;
					
					if (GC.serverPlayer)
					{
						if (hurtAgent.brain.Goals.Count > 0)
							hurtAgent.brain.RemoveAllSubgoals(hurtAgent.brain.Goals[0]);
					
						if (hurtAgent.brain.Goals.Count > 0)
							hurtAgent.brain.Goals[0].Terminate();
						
						hurtAgent.brain.Goals.Clear();
					}

					hurtAgent.inCombat = false;
					hurtAgent.inFleeCombat = false;
					
					if (hurtAgent.isPlayer > 0)
						hurtAgent.interactionHelper.gameObject.SetActive(false);
					
					if (!hurtAgent.KnockedOut())
						GC.spawnerMain.SpawnStateIndicator(hurtAgent, "NoAnim");
					
					hurtAgent.relationships.SetAllRel2("Dead");
					
					if (!hurtAgent.oma.bodyGuarded)
					{
						hurtAgent.job = "";
						hurtAgent.jobCode = jobType.None;
						hurtAgent.jobBig = "";
						hurtAgent.jobBigCode = jobType.None;
						hurtAgent.SetFollowing(null);
					
						if (!hurtAgent.resurrect)
							hurtAgent.SetEmployerNullLate();
					}

					hurtAgent.SetTraversable("");
					
					if (hurtAgent.lightReal != null)
						hurtAgent.lightReal.tr.localPosition = new Vector3(0f, -0.2f, 0f);
					
					bool flag16 = true;
					
					if (hurtAgent.justHitByAgent2 || hurtAgent.killedByAgentIndirect)
					{
						if (hurtAgent.justHitByAgent2)
							hurtAgent.killedByAgent = hurtAgent.justHitByAgent2;
						else if (hurtAgent.killedByAgentIndirect)
							hurtAgent.killedByAgent = hurtAgent.killedByAgentIndirect;
						
						Agent playerGettingPoints = null;
						
						if (hurtAgent.killedByAgent.isPlayer > 0)
							playerGettingPoints = hurtAgent.killedByAgent;
						else if (hurtAgent.killedByAgent.employer != null)
						{
							if (hurtAgent.killedByAgent.employer.isPlayer > 0)
								playerGettingPoints = hurtAgent.killedByAgent.employer;
						}
						else if (hurtAgent.recentMindControlAgent != null)
							mindControlAgent = hurtAgent.recentMindControlAgent;
						
						flag16 = __instance.IsInnocent(playerGettingPoints);
					}

					if (!flag10)
					{
						bool flag17 = false;

						if (mindControlAgent != null)
						{
							GC.stats.AddToStat(mindControlAgent, "IndirectlyKilled", 1);
						
							if (__instance.AgentIsRival(mindControlAgent))
								mindControlAgent.skillPoints.AddPoints("IndirectlyKillRival");
							else if (flag16)
								mindControlAgent.skillPoints.AddPoints("IndirectlyKillInnocent");
							else
								mindControlAgent.skillPoints.AddPoints("IndirectlyKill");
						}
						else if (hurtAgent.killedByAgentIndirect != null && hurtAgent.killedByAgentIndirect != hurtAgent)
						{
							if (hurtAgent.killedByAgentIndirect.zombified && hurtAgent.agentName == "Zombie")
								flag17 = true;
							
							hurtAgent.killedByAgent = hurtAgent.killedByAgentIndirect;
							
							if (hurtAgent.isPlayer == 0 && hurtAgent.killedByAgent.isPlayer != 0 && !flag17)
							{
								GC.stats.AddToStat(hurtAgent.killedByAgent, "IndirectlyKilled", 1);
							
								if (flag16)
									GC.stats.AddToStat(hurtAgent.killedByAgent, "InnocentsKilled", 1);
								
								hurtAgent.relationships.AddToKillStat(hurtAgent.killedByAgent);
							}

							if (hurtAgent.isPlayer == 0 && (!hurtAgent.resurrect || hurtAgent.zombified) && !flag17 && !hurtAgent.playersReceivedPoints.Contains(hurtAgent.killedByAgent) && hurtAgent.relationships.GetRel(hurtAgent.killedByAgent) != "Aligned")
							{
								hurtAgent.playersReceivedPoints.Add(hurtAgent.killedByAgent);

								if (__instance.AgentIsRival(hurtAgent.killedByAgent))
									hurtAgent.killedByAgent.skillPoints.AddPoints("IndirectlyKillRival");
								else if (flag16)
									hurtAgent.killedByAgent.skillPoints.AddPoints("IndirectlyKillInnocent");
								else
									hurtAgent.killedByAgent.skillPoints.AddPoints("IndirectlyKill");
							}
						}
						else if (hurtAgent.justHitByAgent2 != null && hurtAgent.justHitByAgent2 != hurtAgent)
						{
							hurtAgent.killedByAgent = hurtAgent.justHitByAgent2;

							if (hurtAgent.killedByAgent.zombified && hurtAgent.agentName == "Zombie")
								flag17 = true;
							
							if (hurtAgent.isPlayer == 0 && hurtAgent.killedByAgent.isPlayer != 0 && !flag17 && !hurtAgent.arrested)
							{
								if (hurtAgent.KnockedOut())
									GC.stats.AddToStat(hurtAgent.killedByAgent, "KnockedOut", 1);
								else
								{
									GC.stats.AddToStat(hurtAgent.killedByAgent, "Killed", 1);
							
									if (flag16)
										GC.stats.AddToStat(hurtAgent.killedByAgent, "InnocentsKilled", 1);
									
									hurtAgent.relationships.AddToKillStat(hurtAgent.killedByAgent);
								}
							}

							if (hurtAgent.isPlayer == 0 && (!hurtAgent.resurrect || hurtAgent.zombified) && !flag17 && !hurtAgent.playersReceivedPoints.Contains(hurtAgent.killedByAgent))
							{
								hurtAgent.playersReceivedPoints.Add(hurtAgent.killedByAgent);

								if (hurtAgent.KnockedOut())
								{
									if (hurtAgent.deathMethod == "Arrested")
									{
										if (flag16)
										{
											hurtAgent.killedByAgent.skillPoints.AddPoints("ArrestedPointsInnocent");
											GC.quests.AddBigQuestPoints(hurtAgent.killedByAgent, "ArrestInnocent");
											GC.quests.AddBigQuestPoints(hurtAgent.killedByAgent, hurtAgent, "Neutralize");
										}
										else
										{
											if (hurtAgent.relationships.GetRel(hurtAgent.killedByAgent) != "Aligned")
												hurtAgent.killedByAgent.skillPoints.AddPoints("ArrestedPoints");
											
											GC.quests.AddBigQuestPoints(hurtAgent.killedByAgent, "ArrestGuilty");
											GC.quests.AddBigQuestPoints(hurtAgent.killedByAgent, hurtAgent, "Neutralize");
										}
									}
									else if (__instance.AgentIsRival(hurtAgent.killedByAgent))
									{
										if (hurtAgent.relationships.GetRel(hurtAgent.killedByAgent) != "Aligned")
											hurtAgent.killedByAgent.skillPoints.AddPoints("KnockOutPointsRival");
										
										GC.quests.AddBigQuestPoints(hurtAgent.killedByAgent, "KillGuilty");
										GC.quests.AddBigQuestPoints(hurtAgent.killedByAgent, hurtAgent, "Neutralize");
									}
									else if (flag16)
									{
										if (hurtAgent.relationships.GetRel(hurtAgent.killedByAgent) != "Aligned")
											hurtAgent.killedByAgent.skillPoints.AddPoints("KnockOutPointsInnocent");
										
										GC.quests.AddBigQuestPoints(hurtAgent.killedByAgent, "KillInnocent");
										GC.quests.AddBigQuestPoints(hurtAgent.killedByAgent, hurtAgent, "Neutralize");
									}
									else
									{
										if (hurtAgent.relationships.GetRel(hurtAgent.killedByAgent) != "Aligned")
											hurtAgent.killedByAgent.skillPoints.AddPoints("KnockOutPoints");
										
										GC.quests.AddBigQuestPoints(hurtAgent.killedByAgent, "KillGuilty");
										GC.quests.AddBigQuestPoints(hurtAgent.killedByAgent, hurtAgent, "Neutralize");
									}
								}
								else
								{
									_ = hurtAgent.deathMethod == "Bite"; // Sic

									if (__instance.AgentIsRival(hurtAgent.killedByAgent))
									{
										if (hurtAgent.relationships.GetRel(hurtAgent.killedByAgent) != "Aligned")
											hurtAgent.killedByAgent.skillPoints.AddPoints("KillPointsRival");
										
										GC.quests.AddBigQuestPoints(hurtAgent.killedByAgent, "KillGuilty");
										GC.quests.AddBigQuestPoints(hurtAgent.killedByAgent, hurtAgent, "Dead");
										GC.quests.AddBigQuestPoints(hurtAgent.killedByAgent, hurtAgent, "Neutralize");
										GC.quests.AddBigQuestPoints(hurtAgent.killedByAgent, hurtAgent, "BloodlustKill");
									}
									else if (flag16)
									{
										if (hurtAgent.relationships.GetRel(hurtAgent.killedByAgent) != "Aligned")
											hurtAgent.killedByAgent.skillPoints.AddPoints("KillPointsInnocent");
										
										GC.quests.AddBigQuestPoints(hurtAgent.killedByAgent, "KillInnocent");
										GC.quests.AddBigQuestPoints(hurtAgent.killedByAgent, hurtAgent, "Dead");
										GC.quests.AddBigQuestPoints(hurtAgent.killedByAgent, hurtAgent, "Neutralize");
										GC.quests.AddBigQuestPoints(hurtAgent.killedByAgent, hurtAgent, "BloodlustKill");
									}
									else
									{
										if (hurtAgent.relationships.GetRel(hurtAgent.killedByAgent) != "Aligned")
											hurtAgent.killedByAgent.skillPoints.AddPoints("KillPoints");
										
										GC.quests.AddBigQuestPoints(hurtAgent.killedByAgent, "KillGuilty");
										GC.quests.AddBigQuestPoints(hurtAgent.killedByAgent, hurtAgent, "Dead");
										GC.quests.AddBigQuestPoints(hurtAgent.killedByAgent, hurtAgent, "Neutralize");
										GC.quests.AddBigQuestPoints(hurtAgent.killedByAgent, hurtAgent, "BloodlustKill");
									}
								}
							}
						}

						if (hurtAgent.killForQuest != null && hurtAgent.justHitByAgent2 != null && hurtAgent.justHitByAgent2.isPlayer > 0)
						{
							try
							{
								hurtAgent.killForQuest.playerTally[hurtAgent.justHitByAgent2.isPlayer - 1]++;
							}
							catch
							{
								Debug.LogError(string.Concat("Agent killForQuest playerTally Error: ", hurtAgent, " - ", hurtAgent.killForQuest, " - ", hurtAgent.justHitByAgent2));
							}
						}

						if (hurtAgent.justHitByAgent2 != null && hurtAgent.isPlayer == 0 && (!hurtAgent.justHitByAgent2.resurrect || hurtAgent.justHitByAgent2.zombified) && !flag17)
						{
							if (hurtAgent.justHitByAgent2.zombified && hurtAgent.agentName == "Zombie")
								flag17 = true;
							
							if (!flag17)
							{
								Agent agent9 = null;
								Agent agent10 = null;
							
								if (hurtAgent.justHitByAgent2.mindControlAgent != null)
								{
									agent9 = hurtAgent.justHitByAgent2.mindControlAgent;
								
									if (hurtAgent.killedByAgent != null && hurtAgent.killedByAgent.mindControlAgent != null)
										agent10 = hurtAgent.killedByAgent.mindControlAgent;
								}
								else if (hurtAgent.justHitByAgent2.employer != null)
								{
									hurtAgent.justHitByAgent2.killsForEmployer++;
									agent9 = hurtAgent.justHitByAgent2.employer;
								
									if (hurtAgent.killedByAgent != null && hurtAgent.killedByAgent.employer != null)
										agent10 = hurtAgent.killedByAgent.employer;
								}
								if (agent9 != null)
								{
									GC.stats.AddToStat(agent9, "IndirectlyKilled", 1);
								
									if (__instance.AgentIsRival(hurtAgent.killedByAgent))
									{
										if (agent10 != null && !hurtAgent.playersReceivedPoints.Contains(agent10))
										{
											agent10.skillPoints.AddPoints("IndirectlyKillRival");
											hurtAgent.playersReceivedPoints.Add(agent10);
										}
									}
									else if (flag16)
									{
										if (!hurtAgent.playersReceivedPoints.Contains(agent9))
										{
											agent9.skillPoints.AddPoints("IndirectlyKillInnocent");
											hurtAgent.playersReceivedPoints.Add(agent9);
										}
									}
									else if (!hurtAgent.playersReceivedPoints.Contains(agent9))
									{
										agent9.skillPoints.AddPoints("IndirectlyKill");
										hurtAgent.playersReceivedPoints.Add(agent9);
									}

									hurtAgent.relationships.AddToKillStat(agent9);
								}
							}

							__instance.ActivateKillProfiter();
						}

						if (!hurtAgent.KnockedOut())
							__instance.RealKillRepercussions(flag10);
						
						if (GC.serverPlayer)
						{
							if (hurtAgent.agentName == "Gangbanger" && hurtAgent.killedByAgent != null)
							{
								if (hurtAgent.killedByAgent.isPlayer != 0)
								{
									GC.sessionData.gangbangersKilled++;
						
									if (GC.sessionData.gangbangersKilled >= GC.unlocks.gangbangerUnlockAmount)
										GC.unlocks.DoUnlock("GangbangerB", "Agent");
									
									if (GC.multiplayerMode)
										GC.playerAgent.objectMult.CallRpcUnlockProgressAll("GangbangerB", "Agent");
								}
							}
							else if (hurtAgent.agentName == "Ghost")
							{
								GC.unlocks.DoUnlock("Werewolf", "Agent");

								if (GC.multiplayerMode)
									GC.playerAgent.objectMult.CallRpcUnlockAll("Werewolf", "Agent");
							}
							else if (hurtAgent.agentName == "Assassin")
							{
								if (hurtAgent.isPlayer == 0)
								{
									GC.unlocks.DoUnlock("AssassinsEveryLevel", "Challenge");
							
									if (GC.multiplayerMode)
										GC.playerAgent.objectMult.CallRpcUnlockAll("AssassinsEveryLevel", "Challenge");
								}
							}
							else if ((hurtAgent.originalWerewolf || hurtAgent.oma.secretWerewolf) && hurtAgent.killedByAgent != null)
							{
								if (hurtAgent.killedByAgent.isPlayer != 0)
								{
									GC.unlocks.DoUnlockProgress("ManyWerewolf", "Challenge", 1);

									if (GC.multiplayerMode)
										GC.playerAgent.objectMult.CallRpcUnlockProgressAll("ManyWerewolf", "Challenge");
								}
							}
							else if (hurtAgent.agentName == "OfficeDrone")
							{
								if (hurtAgent.oma.offeredOfficeDrone)
									GC.ChangeStockPrice("Negative2");
								else
									GC.ChangeStockPrice("Negative");
							}
						}
					}

					_ = hurtAgent.killedByAgent != null; // Sic

					if (hurtAgent.protectingAgent != null)
						hurtAgent.protectingAgent.hasProtector = false;
				}
				else
				{
					Debug.Log("SETUPDEATH " + hurtAgent);
					__instance.SetupDeath(damagerObject, flag10);

					if (!hurtAgent.KnockedOut())
					{
						if (GC.arcadeMode)
							__instance.StartCoroutine(__instance.WaitForArcadeContinue());
						else
							__instance.StartCoroutine(__instance.WaitForGhostRespawn());
					}
				}
			}
			else if (((hurtAgent.KnockedOut() && hurtAgent.health <= -10f) || (hurtAgent.FellInHole() && hurtAgent.health <= 0f) || hurtAgent.burnt) && flag9)
			{
				Debug.Log("Death Type 2: " + hurtAgent);

				if (!GC.serverPlayer && hurtAgent.isPlayer != 0 && hurtAgent.localPlayer)
					hurtAgent.objectMult.CallCmdDieAfterKnockout();
				
				if (hurtAgent.KnockedOut() && GC.levelType == vLevelType.Tutorial)
					GC.tutorial.tutorialPeopleKilled++;
				
				hurtAgent.knockedOut = false;
				hurtAgent.knockedOutLocal = false;
				hurtAgent.sleepKnockedOut = false;
				
				if (hurtAgent.oma.bodyGuarded)
				{
					hurtAgent.zombified = false;
					hurtAgent.zombieWhenDead = false;
				}
				
				if (GC.levelType == vLevelType.Tutorial)
					flag12 = true;
				
				if (!hurtAgent.dead)
					__instance.RealKillRepercussions(flag10);
				
				GC.spawnerMain.SpawnStateIndicator(hurtAgent, "NoAnim");
				hurtAgent.relationships.SetAllRel2("Dead");

				if (!hurtAgent.oma.bodyGuarded)
				{
					hurtAgent.job = "";
					hurtAgent.jobCode = jobType.None;
					hurtAgent.jobBig = "";
					hurtAgent.jobBigCode = jobType.None;
					hurtAgent.SetFollowing(null);
				
					if (!hurtAgent.resurrect)
						hurtAgent.SetEmployerNullLate();
				}

				if (hurtAgent.isMayor)
					GC.ending.SetMayorTurnover("Dead");
				
				hurtAgent.SetTraversable("");
				hurtAgent.agentHitboxScript.eyesWBGO.SetActive(true);
				
				if (hurtAgent.isPlayer > 0)
				{
					if (hurtAgent.possessing)
						hurtAgent.resurrect = true;
				
					__instance.SetupDeath(damagerObject, flag10);
					
					if (!hurtAgent.resurrect)
					{
						if (GC.coopMode || GC.multiplayerMode || (GC.fourPlayerMode && !GC.arcadeMode))
						{
							if (hurtAgent.FellInHole())
								__instance.RespawnAsGhost();
							else
								__instance.StartCoroutine(__instance.WaitForGhostRespawn());
						}
						else if (GC.arcadeMode)
						{
							Debug.Log("SETUPDEATH 2" + hurtAgent);
							__instance.StartCoroutine(__instance.WaitForArcadeContinue());
						}
					}
				}
				else
				{
					hurtAgent.Say("", true);

					if (!hurtAgent.zombified && hurtAgent.zombieWhenDead && hurtAgent.isPlayer == 0 && !hurtAgent.inhuman && hurtAgent.agentName != "Alien" && !hurtAgent.FellInHole() && !hurtAgent.burnt)
					{
						if (hurtAgent.justHitByAgent2 != null && hurtAgent.justHitByAgent2.zombified)
							hurtAgent.zombifiedByAgent = hurtAgent.justHitByAgent2;
					
						hurtAgent.zombified = true;
						hurtAgent.resurrect = true;
						
						if (hurtAgent.nonQuestObjectMarker != null)
						{
							bool flag18 = true;
						
							for (int num9 = 0; num9 < GC.playerAgentList.Count; num9++)
								if (GC.playerAgentList[num9].agentName == "Zombie")
									flag18 = false;
							
							if (flag18)
								Object.Destroy(hurtAgent.nonQuestObjectMarker.gameObject);
						}

						__instance.StartCoroutine(__instance.ResurrectSequence());
					}
					else if (hurtAgent.resurrect && hurtAgent.isPlayer == 0 && !hurtAgent.FellInHole())
						__instance.StartCoroutine(__instance.ResurrectSequence());
				}

				hurtAgent.agentHitboxScript.wholeBodyMode = 0;
				hurtAgent.agentHitboxScript.SetWBSprites();
			}

			if (flag9)
			{
				if (hurtAgent.frozen && !hurtAgent.dead && healthNum <= 0f && hurtAgent.deathMethod != "Poison" && hurtAgent.deathMethod != "Nicotine" && hurtAgent.deathMethod != "Acid" && hurtAgent.deathMethod != "Cyanide")
					__instance.RemoveStatusEffect("Frozen");
				
				if (hurtAgent.frozen && hurtAgent.resurrect)
				{
					__instance.RemoveStatusEffect("Frozen");
					hurtAgent.frozen = false;
					hurtAgent.objectSprite.agentColorDirty = true;
				}

				if (hurtAgent.ghost && hurtAgent.dead)
				{
					if (GC.multiplayerMode)
						hurtAgent.objectMult.Gib(2);
				
					__instance.GhostGib();
				}
				else if (hurtAgent.frozen && hurtAgent.dead && !hurtAgent.KnockedOut() && !hurtAgent.FellInHole() && !hurtAgent.disappeared && !hurtAgent.resurrect && !hurtAgent.inhuman && !hurtAgent.oma.bodyGuarded)
				{
					if (GC.serverPlayer || (!GC.serverPlayer && (hurtAgent.localPlayer || flag10)))
					{
						if (GC.multiplayerMode)
							hurtAgent.objectMult.Gib(1);
					
						__instance.IceGib();
					}
				}
				else if ((hurtAgent.health <= -20f || healthNum <= -20f || (dmg <= -20f && healthNum <= 0f) || ((GC.levelType == "Tutorial" && healthNum <= -10f) || flag12) || hurtAgent.bloodyMessed) && hurtAgent.dead && !hurtAgent.KnockedOut() && !hurtAgent.resurrect && !hurtAgent.FellInHole() && !hurtAgent.inhuman && !hurtAgent.disappeared && !flag8 && !hurtAgent.oma.bodyGuarded)
				{
					bool flag19 = true;

					if (hurtAgent.justHitByAgent2 != null && hurtAgent.justHitByAgent2.melee != null && (hurtAgent.justHitByAgent2.melee.successfullyBackstabbed || hurtAgent.justHitByAgent2.melee.successfullySleepKilled) && !hurtAgent.justHitByAgent2.statusEffects.hasTrait("BloodyMess"))
					{
						flag19 = false;
						hurtAgent.health = 0f;
					}
					
					if (flag19 && (GC.serverPlayer || (!GC.serverPlayer && (hurtAgent.localPlayer || flag10) && !__instance.hasTrait(vStatusEffect.Resurrection))))
					{
						if (GC.multiplayerMode && !__instance.dontDoBloodExplosion)
							hurtAgent.objectMult.Gib(0);
					
						__instance.NormalGib();
					}
				}

				if (hurtAgent.frozen && hurtAgent.dead)
				{
					hurtAgent.frozen = false;
					hurtAgent.objectSprite.agentColorDirty = true;
				}
			}

			hurtAgent.bloodyMessed = false;
			hurtAgent.slippedOnBananaPeel = false;
			hurtAgent.justHitByAgent2 = null;

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
		public static IEnumerator StatusEffects_WaitForAgentUnhide(StatusEffects __instance) // Non-Patch
		{
			Vector2 playerPos = new Vector2(__instance.agent.tr.position.x, __instance.agent.tr.position.y + 0.16f);
			bool closeToPlayerPos = true;

			do
			{
				if (Vector2.Distance(__instance.agent.tr.position, playerPos) > 0.32f)
					closeToPlayerPos = false;
				else
					yield return null;
			}
			while (closeToPlayerPos && __instance.agent.oma.hidden);
			
			if (__instance.agent.oma.hidden)
				__instance.BecomeNotHidden();
			
			yield break;
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
