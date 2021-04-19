using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using BepInEx;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;
using Random = UnityEngine.Random;

namespace BunnyMod.Content
{
	public class BMBehaviors
	{
		public static GameController GC => GameController.gameController;
		public static bool Prefix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) => BMHeader.MainInstance.PatchPrefix(type, methodName, patchType, patchMethodName, types);
		public static bool Postfix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) => BMHeader.MainInstance.PatchPostfix(type, methodName, patchType, patchMethodName, types);
		public static void BMLog(string logMessage) => BMHeader.Log(logMessage);

		public void Awake()
		{
			Initialize_Names();

			// AgentInteractions
			//Prefix(typeof(AgentInteractions), "AddButton", GetType(), "AgentInteractions_AddButton_4", new Type[3] { typeof(string), typeof(int), typeof(string) });
			//Prefix(typeof(AgentInteractions), "DetermineButtons", GetType(), "AgentInteractions_DetermineButtons", new Type[5] { typeof(Agent), typeof(Agent), typeof(List<string>), typeof(List<string>), typeof(List<int>) });
			//Prefix(typeof(AgentInteractions), "PressedButton", GetType(), "AgentInteractions_PressedButton", new Type[4] { typeof(Agent), typeof(Agent), typeof(string), typeof(int) });
			//Postfix(typeof(AgentInteractions), "UseItemOnObject", GetType(), "AgentInteractions_UseItemOnObject", new Type[6] { typeof(Agent), typeof(Agent), typeof(InvItem), typeof(int), typeof(string), typeof(string) });

			// BrainUpdate
			Prefix(typeof(BrainUpdate), "GoalArbitrate", GetType(), "BrainUpdate_GoalArbitrate", new Type[0] { });

			// LoadLevel
			//Prefix(typeof(LoadLevel), "SetupMore4", GetType(), "LoadLevel_SetupMore4", new Type[0] { });

			// PlayfieldObject
			//Postfix(typeof(PlayfieldObject), "determineMoneyCost", GetType(), "PlayfieldObject_determineMoneyCost", new Type[2] { typeof(int), typeof(string) }); // Uncapitalized in source
		}
		public void FixedUpdate()
		{
		}
		public void Initialize_Names()
		{
			#region ButtonText
			string nameType = "Interface";
			CustomName hobo_GiveMoney1 = RogueLibs.CreateCustomName("Hobo_GiveMoney1", nameType, new CustomNameInfo("Give Money"));
			CustomName hobo_GiveMoney2 = RogueLibs.CreateCustomName("Hobo_GiveMoney2", nameType, new CustomNameInfo("Give Money"));
			CustomName hobo_GiveMoney3 = RogueLibs.CreateCustomName("Hobo_GiveMoney3", nameType, new CustomNameInfo("Give Money"));
			CustomName hobo_GiveItem = RogueLibs.CreateCustomName("Hobo_GiveItem", nameType, new CustomNameInfo("Give Item"));
			CustomName gangbanger_GiveMoney = RogueLibs.CreateCustomName("Gangbanger_GiveMoney", nameType, new CustomNameInfo("Give Money"));
			#endregion
			#region Dialogue
			nameType = "Dialogue";
			CustomName hobo_CantAfford = RogueLibs.CreateCustomName("Hobo_CantAfford", nameType, new CustomNameInfo("Come on, don't hold out on me!"));
			CustomName hobo_Donate_Aligned = RogueLibs.CreateCustomName("Hobo_Donate_Aligned", nameType, new CustomNameInfo("I knew it, you're Jesus! Hey Jesus, get this CIA microchip outta my head! Wabadoobaflooba! Yaba! Haba!"));
			CustomName hobo_Donate_Annoyed = RogueLibs.CreateCustomName("Hobo_Donate_Annoyed", nameType, new CustomNameInfo("...Okay."));
			CustomName hobo_Donate_Friendly = RogueLibs.CreateCustomName("Hobo_Donate_Friendly", nameType, new CustomNameInfo("Hey, thanks buddy!"));
			CustomName hobo_Donate_Hostile = RogueLibs.CreateCustomName("Hobo_Donate_Hostile", nameType, new CustomNameInfo("Fuck you, asshole!"));
			CustomName hobo_Donate_Loyal = RogueLibs.CreateCustomName("Hobo_Donate_Loyal", nameType, new CustomNameInfo("Oh, hell yeah!"));
			CustomName hobo_Donate_Neutral = RogueLibs.CreateCustomName("Hobo_Donate_Neutral", nameType, new CustomNameInfo("Oh, uh... thanks, I guess?"));
			CustomName hobo_DontWant = RogueLibs.CreateCustomName("Hobo_DontWant", nameType, new CustomNameInfo("Nah, what else you got?"));
			CustomName hobo_Mug = RogueLibs.CreateCustomName("Hobo_Mug", nameType, new CustomNameInfo("Can you spare a few bucks, friendo?"));
			CustomName gangbanger_CantAfford = RogueLibs.CreateCustomName("Gangbanger_CantAfford", nameType, new CustomNameInfo("Sucks to be you!"));
			CustomName gangbanger_Mug = RogueLibs.CreateCustomName("Gangbanger_Mug", nameType, new CustomNameInfo("The cash, hand it over!"));
			CustomName gangbangerB_Mug = RogueLibs.CreateCustomName("GangbangerB_Mug", nameType, new CustomNameInfo("Empty your pockets, bitch!"));
			#endregion
		}

		#region Custom
		public static void Hobo_AcceptDonation(Agent hobo, Agent interactingAgent, int moneyValue)
		{
			BMLog("Hobo_AcceptDonation: " + hobo.agentID + " receiving $" + moneyValue);

			// TODO: Write Hobo_AcceptDonation(Money)
		}
		public static void Hobo_AcceptDonation(Agent hobo, Agent interactingAgent, InvItem invItem)
		{
			BMLog("Hobo_AcceptDonation: " + hobo.agentID + " receiving " + invItem.invItemName);

			int moneyValue;
			string item = invItem.invItemName;

			if (item == "BananaPeel")
				moneyValue = -1;
			else if (item == "Banana")
				moneyValue = 0;
			else if (item == "Fud")
				moneyValue = 5;
			else if (item == "Beer" || item == "Cigarettes")
				moneyValue = 10;
			else if (item == "Whiskey")
				moneyValue = 20;
			else if (item == "Sugar")
				moneyValue = 50;
			else
			{
				BMLog("Unacceptable item donated to " + hobo.agentName + hobo.agentID);
				moneyValue = invItem.itemValue;
			}

			string newRelationship = Hobo_relStatusAfterDonation(hobo, interactingAgent, moneyValue).ToString("f");

			BMLog("Hobo_AcceptDonation: item = " + item + ";  moneyValue = " + moneyValue + "; newRelationship = " + newRelationship);

			Hobo_MugItem(hobo, interactingAgent, item, newRelationship);
		}
		public static void Hobo_MugItem(Agent agent, Agent interactingAgent, string itemName, string relStatus)
		{
			BMLog("Hobo_MugItem");

			if (GC.serverPlayer)
			{
				for (int i = 0; i < agent.gangMembers.Count; i++)
					agent.gangMembers[i].hasMugged = true;

				for (int j = 0; j < GC.playerAgentList.Count; j++)
					GC.playerAgentList[j].gangMugging = 0;

				agent.objectMult.SetGangMuggingOff();
				agent.doingMugging = -1;
				agent.SayDialogue("Bought"); // ←
				GC.audioHandler.Play(interactingAgent, "SelectItem");

				agent.relationships.SetRel(interactingAgent, relStatus);

				return;
			}
			interactingAgent.objectMult.ObjectAction(agent.objectNetID, "MugItem");
		}
		public static void Hobo_MugMoney(Agent agent, Agent interactingAgent, int moneyValue, string relStatus, string transactionType)
		{
			BMLog("Hobo_MugMoney");

			if (GC.serverPlayer)
			{
				for (int i = 0; i < agent.gangMembers.Count; i++)
					agent.gangMembers[i].hasMugged = true;

				for (int j = 0; j < GC.playerAgentList.Count; j++)
					GC.playerAgentList[j].gangMugging = 0;

				agent.objectMult.SetGangMuggingOff();
				agent.doingMugging = -1;

				agent.SayDialogue("Hobo_Donate_" + relStatus); //
				InvItem invItem = new InvItem();
				invItem.invItemName = "Money";
				invItem.invItemCount = agent.determineMoneyCost("Hobo_GiveMoney1"); // ←
				invItem.ItemSetup(true);
				agent.inventory.AddItem(invItem);

				agent.relationships.SetRel(interactingAgent, relStatus);

				return;
			}
			interactingAgent.objectMult.ObjectAction(agent.objectNetID, "MugMoney"); // ←
		}
		public static relStatus Hobo_relStatusAfterDonation(Agent hobo, Agent interactingAgent, int moneyValue)
		{
			BMLog("Hobo_relStatusAfterDonation: moneyValue = " + moneyValue);

			int[] reactionPercentages = new int[6] { 0, 0, 0, 0, 0, 0 };
			List<relStatus> reactionOutcomes = new List<relStatus> { relStatus.Hostile, relStatus.Annoyed, relStatus.Neutral, relStatus.Friendly, relStatus.Loyal, relStatus.Aligned };

			if (moneyValue == -1)
				reactionPercentages = new int[] { 100, 0, 0, 0, 0, 0 };
			if (moneyValue == 0)
				reactionPercentages = new int[] { 10, 55, 35, 0, 0, 0 };
			else if (moneyValue == 5)
				reactionPercentages = new int[] { 0, 5, 25, 65, 5, 0 };
			else if (moneyValue == 10)
				reactionPercentages = new int[] { 0, 0, 5, 65, 25, 5 };
			else if (moneyValue == 20)
				reactionPercentages = new int[] { 0, 0, 0, 35, 55, 10 };
			else if (moneyValue == 50)
				reactionPercentages = new int[] { 0, 0, 0, 0, 0, 100 };

			int[] reactionsWeighted = new int[7] { 0, 0, 0, 0, 0, 0, 0 }; // 0th 0 is floor for for-loop

			for (int i = 1; i <= 6; i++) // 0th 0 used here
				reactionsWeighted[i] = reactionsWeighted[i - 1] + reactionPercentages[i];

			int roll = Mathf.Clamp(UnityEngine.Random.Range(1, 100), 1, 100);
			int outcome = 1;

			for (int j = 1; j <= 6; j++)
			{
				if (roll >= reactionsWeighted[j])
					outcome = j;
				else
					break;
			}

			return reactionOutcomes[outcome];
		}
		#endregion

		#region AgentInteractions
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
					agent.SayDialogue("Interact");
			}
			if (agent.agentName == "Gangbanger" || agent.agentName == "GangbangerB")
			{
				GC.audioHandler.Play(agent, "AgentTalk");

				if (agent.gang == interactingAgent.gangMugging && agent.gang != 0)
					__instance.AddButton("Gangbanger_GiveMoney", agent.determineMoneyCost("Mug_Gangbanger"));
				else
					agent.SayDialogue("Interact");
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
						Hobo_MugMoney(agent, interactingAgent, buttonPrice, Hobo_relStatusAfterDonation(agent, interactingAgent, buttonPrice).ToString("f"), buttonText);
					else
						agent.SayDialogue("Hobo_CantAfford");

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
						agent.SayDialogue("Gangbanger_CantAfford");

					agent.StopInteraction();
					return false; // Double-check that these aren't skipping anything important
				}
			}
			return true;
		}
		public static void AgentInteractions_UseItemOnObject(Agent agent, Agent interactingAgent, InvItem item, int slotNum, string combineType, string useOnType, ref bool __result) // Postfix
		{
			BMLog("AgentInteractions_UseItemOnObject: " + item.invItemName);

			if (useOnType == "Hobo_Donate")
			{
				string itemName = item.invItemName;

				if (itemName == "Banana" || itemName == "BananaPeel" || itemName == "Beer" || itemName == "Cigarettes" || itemName == "Fud" || itemName == "Sugar" || itemName == "Whiskey" )
					Hobo_AcceptDonation(agent, interactingAgent, item);
				else
				{
					agent.SayDialogue("Hobo_DontWant");
					GC.audioHandler.Play(interactingAgent, "CantDo");
				}

				__result = true;
			}
		}
		#endregion
		#region BrainUpdate
		public static bool BrainUpdate_GoalArbitrate(BrainUpdate __instance, Agent ___agent, float ___curDesirability, float ___curDesirability2, Danger ___topDanger, TileInfo ___tileInfo, GoalWalkToExit ___tempGoalWalkToExit, float ___highestDesirability, GoalBattle ___tempGoalBattle, GoalFlee ___tempGoalFlee, GoalFleeDanger ___tempGoalFleeDanger, GoalTattle ___tempGoalTattle, GoalFollow ___tempGoalFollow, GoalWaitForDangerEnd ___tempGoalWaitForDangerEnd, GoalInteract ___tempGoalInteract, GoalDoJob ___tempGoalDoJob, GoalNoiseReact ___tempGoalNoiseReact, GoalSteal ___tempGoalSteal, GoalRobotFollow ___tempGoalRobotFollow, GoalPatrol ___tempGoalPatrol, GoalInvestigate ___tempGoalInvestigate, GoalGoGet ___tempGoalGoGet, GoalFindFire ___tempGoalFindFire, GoalCuriousObject ___tempGoalCuriousObject, GoalCommitArson ___tempGoalCommitArson, GoalCannibalize ___tempGoalCannibalize, GoalBite ___tempGoalBite, GoalRobotClean ___tempGoalRobotClean, Brain ___brain, bool ___mustBeZero, List<Danger> ___dangerCombinedList) // Replacement
		{
			___curDesirability = 0f;
			___curDesirability2 = 0f;
			___highestDesirability = 0f;
			goalType goalType = goalType.None;
			___tempGoalBattle.battlingAgent = null;
			___tempGoalFlee.fleeingAgent = null;
			___tempGoalFleeDanger.danger = null;
			___tempGoalFleeDanger.dangerPos = Vector3.zero;
			___tempGoalFleeDanger.dangerType = "";
			___tempGoalFleeDanger.dangerTypeCode = dangerStatus.None;
			___tempGoalTattle.agent = null;
			___tempGoalFollow.followingAgent = null;
			___tempGoalInteract.interactingAgent = null;
			___tempGoalWalkToExit.myObject = null;
			___tempGoalDoJob.curJob = "";
			___tempGoalDoJob.curJobCode = jobType.None;
			___tempGoalDoJob.workingForAgent = null;
			___tempGoalDoJob.assignedPos = Vector3.zero;
			___tempGoalDoJob.assignedAgent = null;
			___tempGoalDoJob.assignedObject = null;
			___tempGoalNoiseReact.noise = null;
			___tempGoalNoiseReact.noisePosition = Vector3.zero;
			___tempGoalWaitForDangerEnd.danger = null;
			___tempGoalPatrol.patrolPoint = null;
			___tempGoalPatrol.patrolPointInterest = null;
			___tempGoalCuriousObject.interest = null;
			___tempGoalCuriousObject.playfieldObject = null;
			___tempGoalInvestigate.investigatePosition = Vector3.zero;
			___tempGoalFindFire.fire = null;
			___tempGoalCommitArson.arsonObject = null;
			___tempGoalSteal.stealingFromAgent = null;
			___tempGoalBite.bitingTarget = null;
			___tempGoalCannibalize.cannibalizingTarget = null;
			___tempGoalGoGet.item = null;
			___tempGoalGoGet.invItem = null;
			___tempGoalRobotFollow.followingAgent = null;
			___tempGoalRobotClean.cleaningObject = null;

			if (___brain.Goals[0].goalStateCode == goalStatus.Completed)
			{
				___brain.Goals[0].Terminate();
				___brain.Goals.RemoveAt(0);
			}

			if (___brain.Goals.Count == 0)
			{
				Goal item = new Goal();
				___brain.Goals.Add(item);
			}
			
			if (___highestDesirability < 100000f)
			{
				int teamSize = 1;
				int otherTeamSize = 1;
				float num = 0f;
				float num2 = 0f;
				float num3 = 0f;
				float num4 = 0f;
				float num5 = 0f;
				bool flag = false;
				Relationship relationship = null;
				Agent agent = null;
				float num6 = 0f;
				float tetherDistance = 0f;
				List<Agent> hateList = ___agent.relationships.hateList;
				int count = hateList.Count;

				for (int i = 0; i < count; i++)
				{
					agent = hateList[i];

					if (agent.brain.active && agent.go.activeSelf && ___agent.agentID != agent.agentID && !agent.objectMultAgent._dead && !agent.ghost && ___agent.prisoner == agent.prisoner)
					{
						relationship = ___agent.relationships.GetRelationship(agent);

						if (relationship.relTypeCode == relStatus.Hostile && (relationship.lastSawPosition != Vector3.zero || ___agent.justHitByAgent.agentID == agent.agentID))
						{
							if (!flag)
							{
								flag = true;
								teamSize = ___agent.relationships.GetTeamSize(true);
								otherTeamSize = agent.relationships.GetTeamSize(true);
							}

							if (___agent.justHitByAgent.agentID == agent.agentID)
								relationship.lastSawPosition = agent.curPosition;

							if (agent.prisoner == ___agent.prisoner && agent.startingChunk == ___agent.startingChunk && agent.brainUpdate.slowAIWait > 0)
								agent.brainUpdate.slowAIWait = 0;

							num5 = relationship.distance;
							num6 = relationship.relHate;
							tetherDistance = 0f;
							num = ___agent.relationships.AssessBattle(agent, teamSize, otherTeamSize, num5, num6, relationship);
							num2 = ___agent.relationships.AssessFlee(agent, teamSize, otherTeamSize, num5, num6, relationship);

							if (___agent.mustFlee && ___agent.slaveOwners.Count == 0 && !___agent.angryAtMayor)
								num2 = 999999f;
							
							if (___agent.wontFlee && !___agent.shrunk)
								num2 = 0f;
							
							if (___agent.zombified)
								num2 = 0f;
							
							if (agent.objectAgent)
								num2 = 0f;
							
							if (___agent.hasStealingFromAgent && agent.agentID == ___agent.stealingFromAgent.agentID && num6 < 15f)
							{
								if (___agent.defaultGoalCode == goalType.Steal)
								{
									___agent.SetDefaultGoal("WanderFar");
									___agent.stoleStuff = true;
								}

								num2 = 999999f;
							}

							#region Home Chunk Tether Distance Check
							if (___agent.modLeashes == 1 && relationship.brainCyclesSinceLastHomeCheck > 10)
							{
								relationship.brainCyclesSinceLastHomeCheck = 0;
								float nearestDistanceToTether = 0f;
							
								if (___agent.hasStartingChunkReal)
								{
									if (__instance.levelTypeNormal)
										nearestDistanceToTether = Vector2.Distance(agent.curPosition, ___agent.startingChunkReal.chunkLeash1Tr);
								
									if (___agent.startingChunkReal.shape == 5 || ___agent.startingChunkReal.shape == 6)
										nearestDistanceToTether = Math.Min(nearestDistanceToTether, Vector2.Distance(agent.curPosition, ___agent.startingChunkReal.chunkLeash2Tr));

									if (___agent.startingChunkReal.shape == 5 || ___agent.startingChunkReal.shape == 6)
										nearestDistanceToTether = Math.Min(nearestDistanceToTether, Vector2.Distance(agent.curPosition, ___agent.startingChunkReal.chunkLeash3Tr));

									if (___agent.startingChunkReal.shape == 6)
										nearestDistanceToTether = Math.Min(nearestDistanceToTether, Vector2.Distance(agent.curPosition, ___agent.startingChunkReal.chunkLeash4Tr));
								}

								if (nearestDistanceToTether == 0f)
									nearestDistanceToTether = Vector2.Distance(agent.curPosition, ___agent.startingPosition);
								
								if (__instance.chunkStatic)
									nearestDistanceToTether = 0f;
								
								relationship.lastHomeCheckDistance = nearestDistanceToTether;
								tetherDistance = nearestDistanceToTether;
							}
							else
							{
								tetherDistance = relationship.lastHomeCheckDistance;
								relationship.brainCyclesSinceLastHomeCheck++;
							}
							#endregion

							if (___agent.hasEmployer && !agent.objectAgent && agent.isPlayer == 0)
								try
								{
									if (agent.brain.Goals[0].goalCode == goalType.Flee)
										tetherDistance = Vector2.Distance(___agent.curPosition, ___agent.employer.curPosition);
									
									if (((agent.brain.Goals[0].goalCode != goalType.Battle && agent.brain.Goals[0].goalCode != goalType.Flee && agent.brain.Goals[0].goalCode != goalType.Bite && agent.brain.Goals[0].goalCode != goalType.CommitArson && agent.brain.Goals[0].goalCode != goalType.Tattle) || agent.frozen || agent.electrocuted || agent.dizzy) && agent != ___agent.assignedAgent && ___agent.justHitByAgent.agentID != agent.agentID)
									{
										num = 0f;
										num2 = 0f;
									}
									
									if (___agent.oma.bodyGuarded)
									{
										if (agent.brain.Goals[0].goalCode != goalType.Battle)
										{
											num = 0f;
											num2 = 0f;
										}
										else if (___agent.relationships.GetRelationship(agent).relHate <= 5f)
											num = 0f;
									}
								}
								catch
								{
									try
									{
										if ((agent.frozen || agent.electrocuted || agent.dizzy) && agent != ___agent.assignedAgent && ___agent.justHitByAgent.agentID != agent.agentID)
										{
											num = 0f;
											num2 = 0f;
										}
									}
									catch { }
								}
							
							if (___agent.zombified && !agent.objectAgent && !___agent.hasEmployer && num5 > 6.72f)
								num = 0f;
							
							if (agent.objectAgent && agent.assignedObjectReal != null && (agent.assignedObjectReal.meleeCanPass || agent.assignedObjectReal.destroyed))
							{
								num = 0f;
								num2 = 0f;
							}
							
							if (GC.lockdown && GC.tileInfo.DifferentLockdownZones(___agent.curTileData, agent.curTileData))
							{
								num = 0f;
								num2 = 0f;
							}
							
							bool flag2 = false;
							___curDesirability = 0f;
							___curDesirability2 = 0f;
							
							if (num >= num3 && num5 < 21.6f)
							{
								if (___brain.Goals[0].goalCode != goalType.Battle && (relationship.HasLOS("") || ___agent.justHitByAgent.agentID == agent.agentID) && num5 < 13.44f)
								{
									___curDesirability = num;
									flag2 = true;
								}
								else if (___brain.Goals[0].goalCode == goalType.Battle || ___agent.doorFollow)
								{
									___curDesirability = num;
									flag2 = true;
								}
								else if (___agent.attackCooldown > 0f)
								{
									___curDesirability = num;
									flag2 = true;
								}
							}

							if (num2 >= num4 && num5 < 21.6f)
							{
								if (___brain.Goals[0].goalCode != goalType.Flee && (relationship.HasLOS("") || ___agent.justHitByAgent.agentID == agent.agentID) && num5 < 13.44f)
									___curDesirability2 = num2;
								else if (___brain.Goals[0].goalCode == goalType.Flee)
									___curDesirability2 = num2;
								else if (___agent.attackCooldown > 0f)
									___curDesirability2 = num2;
								else if (___agent.hasStealingFromAgent && agent.agentID == ___agent.stealingFromAgent.agentID && num6 < 15f)
									___curDesirability2 = num2;
							}

							if (___agent.enraged && (___curDesirability > 0f || ___curDesirability2 > 0f))
							{
								___curDesirability = 100f;
								___curDesirability2 = 0f;
							}
							
							if (___agent.shrunk && !agent.shrunk && (___curDesirability > 0f || ___curDesirability2 > 0f))
							{
								___curDesirability = 0f;
								___curDesirability2 = 100f;
							}
							
							if (!___agent.shrunk && agent.shrunk && (___curDesirability > 0f || ___curDesirability2 > 0f) && flag2)
							{
								___curDesirability = 100f;
								___curDesirability2 = 0f;
							}
							
							if (___agent.rescueForQuest != null && agent.objectAgent && flag2)
							{
								___curDesirability = 100f;
								___curDesirability2 = 0f;
							}
							
							if (___agent.prisoner > 0)
							{
								___curDesirability = 100f;
								___curDesirability2 = 0f;
							}

							string agentName = agent.agentName;
							
							if (agent.killerRobot)
							{
								___curDesirability = 0f;
								___curDesirability2 = 100f;
							}
							
							if (agent.hasGettingArrestedByAgent || agent.hasGettingBitByAgent)
							{
								___curDesirability = 0f;
								___curDesirability2 = 0f;
							}
							
							if (__instance.noCowards)
							{
								bool flag3 = false;

								if (___agent.upperCrusty && !___agent.usedAlarmButton)
								{
									flag3 = true;
									bool flag4 = false;
								
									for (int j = 0; j < GC.objectRealListActive.Count; j++)
										if (GC.objectRealListActive[j].objectName == "AlarmButton")
										{
											flag4 = true;
											break;
										}
									
									if (!flag4)
										flag3 = false;
								}

								if (!flag3)
									___curDesirability2 = 0f;
							}

							if (___curDesirability >= ___highestDesirability)
							{
								bool isFollowingAndClose = false;
							
								if (tetherDistance < 20f || ___agent.guardingMayor)
									isFollowingAndClose = true;
								
								if (___agent.hasEmployer)
								{
									isFollowingAndClose = true;
								
									if (tetherDistance > 8f && agent.mostRecentGoalCode == goalType.Flee)
										isFollowingAndClose = false;
								}

								if (___agent.attackCooldown > 0f)
									isFollowingAndClose = true;
								
								if (___agent.jobCode == jobType.Follow && ___agent.hasFollowing && Vector2.Distance(___agent.curPosition, ___agent.following.curPosition) > 8f)
									isFollowingAndClose = false;
								
								if (agent.onFireServer)
									isFollowingAndClose = true;
								
								if (isFollowingAndClose && num > num3)
								{
									num3 = num;
									Agent battlingAgent = agent;
									___highestDesirability = ___curDesirability;
									___tempGoalBattle.battlingAgent = battlingAgent;
									goalType = goalType.Battle;
								}
							}

							if (___curDesirability2 >= ___highestDesirability && ___curDesirability2 >= ___curDesirability)
							{
								bool flag6 = true;

								if (___agent.jobCode == jobType.Follow && ___agent.hasFollowing && Vector2.Distance(___agent.curPosition, ___agent.following.curPosition) > 8f)
									flag6 = false;
								
								if (___curDesirability2 == 100f)
									flag6 = true;
								
								if (flag6 && num2 > num4)
								{
									num4 = num2;
									Agent fleeingAgent = agent;
									___highestDesirability = ___curDesirability2;
									___tempGoalFlee.fleeingAgent = fleeingAgent;
									goalType = goalType.Flee;
								}
							}
						}
					}
				}
			}

			if ((___agent.prisoner == 0 || ___agent.ownerID > 0) && !___agent.sleeping && __instance.levelTypeNormal && !___agent.zombified && !___agent.ghost && !___agent.hasDangerMajorMinor && !___agent.oma.bodyGuarded)
			{
				___curDesirability = 0f;
				___mustBeZero = false;

				if (___brain.Goals[0].goalCode == goalType.FleeDanger)
				{
					GoalFleeDanger goalFleeDanger = (GoalFleeDanger)___brain.Goals[0];

					if ((goalFleeDanger.dangerTypeCode == dangerStatus.Minor || goalFleeDanger.dangerTypeCode == dangerStatus.MajorMinor || goalFleeDanger.dangerTypeCode == dangerStatus.Major) && Vector2.Distance(___agent.curPosition, goalFleeDanger.dangerPos) < 21.6f)
					{
						bool flag7 = false;
						bool flag8 = false;

						if (goalFleeDanger.danger.agent != null)
						{
							if (goalFleeDanger.danger.agent.isPlayer != 0)
								flag7 = true;
							else if (goalFleeDanger.danger.agent.hasEmployer)
								flag8 = true;
						}

						if (goalFleeDanger.dangerTypeCode == dangerStatus.MajorMinor && ((___agent.hasEmployer && (___agent.employer == goalFleeDanger.danger.agent || flag7 || flag8)) || ___agent.questGiverQuest != null || ___agent.questEnderQuest != null || ___agent.rescueForQuest != null))
						{
							___curDesirability = 0f;
							___mustBeZero = true;
						}
						else
							___curDesirability = 20f;
						
						if (goalFleeDanger.danger.agent != null && goalFleeDanger.agent.invisible)
						{
							___curDesirability = 0f;
							___mustBeZero = true;
						}
					}

					if ((goalFleeDanger.dangerTypeCode == dangerStatus.Prison || goalFleeDanger.dangerTypeCode == dangerStatus.Haunting) && !___agent.retreatedIndoors)
					{
						if (___agent.agentName == "Gorilla")
							___curDesirability = 20f;
						else
							___curDesirability = 250f;
					}

					if (goalFleeDanger.danger.dangerSizeCode == dangerSizeType.Spooked)
						___curDesirability = 20f;
				}

				___topDanger = null;
				List<Danger> dangerListNoCollider = GC.dangerListNoCollider;

				for (int k = 0; k < dangerListNoCollider.Count; k++)
				{
					Danger danger = dangerListNoCollider[k];

					if (danger.usingBox)
					{
						if (___agent.curPosX > danger.curPosition.x - danger.dangerHalfWidth && ___agent.curPosX < danger.curPosition.x + danger.dangerHalfWidth && ___agent.curPosY > danger.curPosition.y - danger.dangerHalfHeight && ___agent.curPosY < danger.curPosition.y + danger.dangerHalfHeight)
							__instance.AssessDanger(danger);
					}
					else
					{
						try
						{
							if (Vector2.Distance(___agent.curPosition, danger.playfieldObject.curPosition) < danger.dangerRadius / 2f)
								__instance.AssessDanger(danger);
						}
						catch { }
					}
				}

				List<Danger> list = ___dangerCombinedList;

				for (int l = 0; l < list.Count; l++)
				{
					Danger danger2 = list[l];
					__instance.AssessDanger(danger2);
				}

				if (___topDanger != null)
				{
					if (___topDanger.dangerTypeCode == dangerStatus.Prison && ___topDanger.prison == ___agent.prisonEscape && ___topDanger.startingChunk == ___agent.startingChunk && ___agent.prisoner == 0 && ___brain.Goals[0].goalCode != goalType.Follow && !___agent.hasEmployer && ___agent.rescueForQuest == null)
					{
						if (___agent.agentName == "Gorilla")
							___curDesirability = 20f;
						else
							___curDesirability = 250f;
						
						List<Quest> mainQuestList = GC.quests.mainQuestList;
						
						for (int m = 0; m < mainQuestList.Count; m++)
						{
							Quest quest = mainQuestList[m];
						
							if (quest.questStatus != "NotAccepted" && quest.questType == "Rescue" && quest.questTarget1 == ___agent)
								___curDesirability = 7f;
						}

						if (___agent.retreatedIndoors)
							___curDesirability = 0f;
					}
					else if (___topDanger.dangerTypeCode == dangerStatus.Haunting && ___topDanger.startingChunk == ___agent.startingChunk && ___agent.prisoner == 0 && ___brain.Goals[0].goalCode != goalType.Follow && !___agent.hasEmployer && ___agent.rescueForQuest == null)
					{
						if (___agent.agentName == "Gorilla")
							___curDesirability = 20f;
						else
							___curDesirability = 250f;
						
						if (___agent.defaultGoalCode != goalType.WanderFar)
						{
							___agent.SetDefaultGoal("WanderFar");
							___agent.SetPreviousDefaultGoal(___agent.defaultGoal);
						}
						
						___agent.haunted = true;
						List<Quest> mainQuestList2 = GC.quests.mainQuestList;
						
						for (int n = 0; n < mainQuestList2.Count; n++)
						{
							Quest quest2 = mainQuestList2[n];

							if (quest2.questStatus != "NotAccepted" && quest2.questType == "Rescue" && quest2.questTarget1 == ___agent)
								___curDesirability = 7f;
						}
					}
					else if (___topDanger.dangerSizeCode == dangerSizeType.AnnoyedAgent)
					{
						if (___agent.hasEmployer)
							___curDesirability = 7f;
						else
							___curDesirability = 30f;
					}
					else if (___topDanger.dangerSizeCode == dangerSizeType.Spooked)
						___curDesirability = 20f;
					else if (___topDanger.dangerTypeCode != dangerStatus.Prison && ___topDanger.dangerTypeCode != dangerStatus.Haunting && ___topDanger.dangerTypeCode != dangerStatus.Minor && ___topDanger.dangerTypeCode != dangerStatus.ReallyMinor && ___topDanger.dangerSizeCode != dangerSizeType.AnnoyedAgent)
						___curDesirability = 30f;
					else if ((___topDanger.dangerTypeCode == dangerStatus.Minor || ___topDanger.dangerTypeCode == dangerStatus.ReallyMinor) && ___topDanger.agent != null && ___agent.modToughness == 0 && ___agent.slaveOwners.Count == 0 && ___agent.relationships.GetRelCode(___topDanger.agent) != relStatus.Aligned && ___agent.relationships.GetRelCode(___topDanger.agent) != relStatus.Loyal && ___agent.relationships.GetRelCode(___topDanger.agent) != relStatus.Hostile && ___agent.relationships.GetRelCode(___topDanger.agent) != relStatus.Submissive && ___agent.questGiverQuest == null && ___agent.questEnderQuest == null && ___agent.killForQuest == null && ___agent.rescueForQuest == null && ___brain.Goals[0].goalCode != goalType.Follow && !___agent.hasEmployer)
					{
						TileData curTileData = ___agent.curTileData;

						if (curTileData.chunkID == ___agent.startingChunk && curTileData.owner > 0)
						{
							curTileData = ___topDanger.agent.curTileData;

							if (curTileData.owner > 0 && ___agent.prisoner == ___topDanger.agent.prisoner)
								___curDesirability = 15f;
						}
						else
							___curDesirability = 15f;
					}
				}

				if (___mustBeZero)
					___curDesirability = 0f;
				
				if (___curDesirability >= ___highestDesirability)
				{
					___highestDesirability = ___curDesirability;
				
					if (___topDanger != null)
					{
						___tempGoalFleeDanger.danger = ___topDanger;
						___tempGoalFleeDanger.dangerType = ___topDanger.dangerType;
						___tempGoalFleeDanger.dangerTypeCode = ___topDanger.dangerTypeCode;
						___tempGoalFleeDanger.dangerPos = ___topDanger.tr.position;
					}
					else if (___brain.Goals[0].goalCode == goalType.FleeDanger)
					{
						GoalFleeDanger goalFleeDanger2 = (GoalFleeDanger)___brain.Goals[0];
						___tempGoalFleeDanger.danger = goalFleeDanger2.danger;
						___tempGoalFleeDanger.dangerType = goalFleeDanger2.dangerType;
						___tempGoalFleeDanger.dangerTypeCode = goalFleeDanger2.dangerTypeCode;
						___tempGoalFleeDanger.dangerPos = goalFleeDanger2.dangerPos;
					}
					
					goalType = goalType.FleeDanger;
				}
			}

			if (___agent.gang != 0 && ___agent.doingMugging == -1 && !___agent.hasEmployer)
			{
				float num12 = 9999f;
				Agent canWarn = null;
				List<Agent> gangMembers = ___agent.gangMembers;
				Agent canWarn2 = null;
				float num13 = 0f;

				for (int num14 = 0; num14 < gangMembers.Count; num14++)
				{
					Agent agent2 = gangMembers[num14];

					if (agent2.gang == ___agent.gang && !agent2.objectMultAgent._dead && agent2.agentID != ___agent.agentID && !agent2.hasEmployer)
					{
						float num15 = Vector2.Distance(___agent.curPosition, agent2.curPosition);

						if (agent2.isMayor)
						{
							canWarn2 = agent2;
							num13 = num15;
						}

						if (num15 < num12)
						{
							num12 = num15;
							canWarn = agent2;
						}
					}
				}

				if (num12 != 9999f && ___agent.challengedToFight == 0)
				{
					float num16 = 3f;

					if (___agent.mostRecentGoalCode == goalType.Follow)
						num16 = 2f;
					
					if (num12 > num16)
						___agent.canWarn = canWarn;
				
					if (num13 > 5f)
						___agent.canWarn = canWarn2;
				}
			}

			if (___agent.canWarn != null && ___highestDesirability < 10f)
			{
				___curDesirability = 10f;
			
				if (___curDesirability >= ___highestDesirability)
				{
					___highestDesirability = ___curDesirability;
					___tempGoalFollow.followingAgent = ___agent.canWarn;
					goalType = goalType.Follow;
				}
			}

			if (___agent.listeningToAgentsJoke != null && ___highestDesirability < 10f)
			{
				___curDesirability = 10f;

				if (___curDesirability >= ___highestDesirability)
				{
					___highestDesirability = ___curDesirability;
					___tempGoalFollow.followingAgent = ___agent.listeningToAgentsJoke;
					goalType = goalType.Follow;
				}
			}

			if (___agent.defaultGoalCode == goalType.Steal && ___highestDesirability < 10f)
			{
				___curDesirability = 10f;
				relStatus relCode = ___agent.relationships.GetRelCode(___agent.stealingFromAgent);

				if (___agent.stealingFromAgent.invisible || ___agent.stealingFromAgent.disappeared || ___agent.slaveOwners.Count > 0 || ___agent.hasEmployer || relCode == relStatus.Aligned || relCode == relStatus.Loyal || relCode == relStatus.Friendly || ___agent.hasGettingArrestedByAgent || ___agent.hasGettingBitByAgent)
				{
					___curDesirability = 0f;
					___agent.SetDefaultGoal("WanderFar");
				}

				if (___curDesirability >= ___highestDesirability)
				{
					___highestDesirability = ___curDesirability;
					___tempGoalSteal.stealingFromAgent = ___agent.stealingFromAgent;
					goalType = goalType.Steal;
				}
			}

			if (___agent.defaultGoalCode == goalType.Bite && ___highestDesirability < 10f)
			{
				___curDesirability = 10f;
				relStatus relCode2 = ___agent.relationships.GetRelCode(___agent.bitingTarget);

				if (___agent.bitingTarget.invisible || ___agent.bitingTarget.disappeared || ___agent.slaveOwners.Count > 0 || relCode2 == relStatus.Aligned || relCode2 == relStatus.Loyal || relCode2 == relStatus.Friendly || ___agent.hasGettingArrestedByAgent)
				{
					___curDesirability = 0f;
					___agent.SetDefaultGoal(___agent.previousDefaultGoal);
				}

				if (___curDesirability >= ___highestDesirability)
				{
					___highestDesirability = ___curDesirability;
					___tempGoalBite.bitingTarget = ___agent.bitingTarget;
					goalType = goalType.Bite;
				}
			}

			if (___agent.defaultGoalCode == goalType.Cannibalize && ___highestDesirability < 10f)
			{
				___curDesirability = 10f;

				if (___agent.cannibalizingTarget.disappeared || ___agent.slaveOwners.Count > 0)
				{
					___curDesirability = 0f;
					___agent.SetDefaultGoal(___agent.previousDefaultGoal);
				}

				if (___curDesirability >= ___highestDesirability)
				{
					___highestDesirability = ___curDesirability;
					___tempGoalCannibalize.cannibalizingTarget = ___agent.cannibalizingTarget;
					goalType = goalType.Cannibalize;
				}
			}

			if (___agent.defaultGoalCode == goalType.GoGet && ___highestDesirability < 10f)
			{
				___curDesirability = 10f;

				if (___agent.goGettingTarget == null)
				{
					___curDesirability = 0f;
					___agent.SetDefaultGoal(___agent.previousDefaultGoal);
				}
				else if (___agent.goGettingTarget.fellInHole || !___agent.goGettingTarget.go.activeSelf)
				{
					___curDesirability = 0f;
					___agent.SetDefaultGoal(___agent.previousDefaultGoal);
				}

				if (___curDesirability >= ___highestDesirability)
				{
					___highestDesirability = ___curDesirability;
					___tempGoalGoGet.item = ___agent.goGettingTarget;
					goalType = goalType.GoGet;
				}
			}

			List<Agent> playerAgentList = GC.playerAgentList;

			for (int num17 = 0; num17 < playerAgentList.Count; num17++)
			{
				Agent agent3 = playerAgentList[num17];
				bool flag9 = false;

				if (GC.multiplayerMode && ___agent.interactingAgent == agent3 && !agent3.localPlayer)
					flag9 = true;
				
				bool flag10 = false;
				
				if (___agent.gang != 0)
					for (int num18 = 0; num18 < ___agent.gangMembers.Count; num18++)
					{
						Agent agent4 = ___agent.gangMembers[num18];
				
						if (agent4.agentID != ___agent.agentID && agent4.interactingAgent != null && !agent4.objectMultAgent._dead)
						{
							flag10 = true;
							agent3 = agent4.interactingAgent;
						}
					}

				if (agent3.interactionHelper.interactionObject == ___agent.agentSpriteTransform.gameObject || flag9 || flag10)
				{
					if (___agent.questGiverQuest != null)
						___curDesirability = 1000f;
					else
						___curDesirability = 9.2f;
					
					if (___curDesirability >= ___highestDesirability)
					{
						___highestDesirability = ___curDesirability;
						___tempGoalInteract.interactingAgent = agent3;
						goalType = goalType.Interact;
					}
				}
			}

			if (___agent.wantsToExit && ___highestDesirability < 2000f)
			{
				___curDesirability = 2000f;
			
				if (___curDesirability >= ___highestDesirability)
				{
					___highestDesirability = ___curDesirability;
				
					if (___agent.rescueForQuest != null)
					{
						if (___agent.rescueForQuest.extraVar == 0)
							___tempGoalWalkToExit.myObject = GC.startingPoint;
						else
							___tempGoalWalkToExit.myObject = GC.exitPoint;
					
						___tempGoalWalkToExit.escorting = true;
					}
					else if (___agent.slaveWantsToExit)
					{
						float num19 = Vector2.Distance(___agent.transform.position, GC.startingPoint.tr.position);
						float num20 = Vector2.Distance(___agent.transform.position, GC.exitPoint.tr.position);

						if (num19 < num20)
							___tempGoalWalkToExit.myObject = GC.startingPoint;
						else
							___tempGoalWalkToExit.myObject = GC.exitPoint;
						
						___tempGoalWalkToExit.escorting = true;
					}
					else if (___agent.killForQuest != null)
					{
						if (___agent.killForQuest.questStatusCode == questStatusType.Done)
						{
							___tempGoalWalkToExit.myObject = GC.exitPoint;
							___tempGoalWalkToExit.escorting = false;
						}
						else
						{
							if (___agent.wantsToExitLocation == null)
								___tempGoalWalkToExit.myObject = GC.startingPoint;
							else
								___tempGoalWalkToExit.myObject = ___agent.wantsToExitLocation;
							
							___tempGoalWalkToExit.escorting = true;
						}
					}
					else
					{
						___tempGoalWalkToExit.myObject = GC.exitPoint;
						___tempGoalWalkToExit.escorting = false;
					}

					goalType = goalType.WalkToExit;
				}
			}

			bool flag11 = false;

			if (___agent.slaveOwners.Count == 1 && ___agent.challengedToFight == 0)
			{
				Agent agent5 = ___agent.slaveOwners[0];

				if (agent5.isPlayer == 0)
				{
					if ((agent5.mostRecentGoalCode == goalType.Flee || agent5.mostRecentGoalCode == goalType.FleeDanger) && !agent5.objectMultAgent._dead)
					{
						flag11 = true;
						___agent.SetFollowing(agent5);
					}
					else if (___agent.hasFollowing && agent5 != ___agent.following)
						___agent.SetFollowing(null);
				}
			}

			if ((___agent.jobCode == jobType.Follow || ___agent.jobCode == jobType.None || flag11) && ___agent.jobBigCode == jobType.None && ___agent.hasFollowing && ___highestDesirability < 200f && (___agent.following.prisoner == ___agent.prisoner || ___agent.prisonEscape != 0))
			{
				if (___agent.rescueForQuest != null)
					___curDesirability = 200f;
				else if (___agent.relationships.GetRelCode(___agent.following) == relStatus.Hostile)
					___curDesirability = 10f;
				else
					___curDesirability = 8f;
				
				if (___curDesirability >= ___highestDesirability)
				{
					___highestDesirability = ___curDesirability;
					___tempGoalFollow.followingAgent = ___agent.following;
					goalType = goalType.Follow;
				}
			}

			if ((___agent.jobCode != jobType.None || (___agent.jobBigCode != jobType.None && ___agent.jobCode == jobType.None)) && ___highestDesirability < 10f)
			{
				___curDesirability = 10f;

				if (___agent.jobCode == jobType.None)
				{
					___agent.job = ___agent.jobBig;
					___agent.jobCode = ___agent.jobBigCode;
				}

				if (___curDesirability >= ___highestDesirability)
				{
					___highestDesirability = ___curDesirability;
					___tempGoalDoJob.workingForAgent = ___agent.employer;
					___tempGoalDoJob.curJob = ___agent.job;
					___tempGoalDoJob.curJobCode = ___agent.jobCode;
					___tempGoalDoJob.assignedPos = ___agent.assignedPos;
					___tempGoalDoJob.assignedAgent = ___agent.assignedAgent;
					___tempGoalDoJob.assignedObject = ___agent.assignedObject;
					goalType = goalType.DoJob;
				}
			}

			if (___agent.firefighter && !___agent.retreatedIndoors && ___highestDesirability < 9.8f)
			{
				___curDesirability = 0f;
				Fire fire = null;
				float num21 = 10000f;

				if (GC.firesList.Count > 0)
				{
					InvItem equippedSpecialAbility = ___agent.inventory.equippedSpecialAbility;

					if (equippedSpecialAbility != null && equippedSpecialAbility.invItemCount != 0)
						for (int num22 = 0; num22 < GC.firesList.Count; num22++)
						{
							Fire fire2 = GC.firesList[num22];

							if (!fire2.neverGoOut && !fire2.itemFire && !fire2.destroying)
							{
								float num23 = Vector2.Distance(fire2.tr.position, ___agent.tr.position);

								if (num23 < num21 && !GC.tileInfo.DifferentLockdownZones(___agent.curTileData, fire2.curTileData))
								{
									num21 = num23;
									fire = fire2;
									___curDesirability = 9.8f;
								}
							}
						}
				}

				if (___curDesirability >= ___highestDesirability)
				{
					___highestDesirability = ___curDesirability;
					___tempGoalFindFire.fire = fire;
					goalType = goalType.FindFire;
				}
			}

			if (___agent.arsonist && !___agent.retreatedIndoors)
			{
				___curDesirability = 0f;

				if (false || ___agent.activeArsonist)
				{
					bool flag12 = false;

					if (___agent.inventory.HasItem("MolotovCocktail"))
					{
						if (___agent.curArsonTarget != null)
						{
							if (___agent.curArsonTarget.ora._destroyed)
								___agent.curArsonTarget = null;
							else
								flag12 = true;
						}

						Agent agent6 = null;
						float num24 = 10000f;

						for (int num25 = 0; num25 < GC.playerAgentList.Count; num25++)
							if (GC.playerAgentList[num25].bigQuest == "Firefighter")
							{
								Agent agent7 = GC.playerAgentList[num25];
								float num26 = Vector2.Distance(___agent.curPosition, agent7.curPosition);

								if (num26 < num24)
								{
									num24 = num26;
									agent6 = agent7;
								}
							}

						if (!flag12)
						{
							List<ObjectReal> list2 = new List<ObjectReal>();

							for (int num27 = 0; num27 < GC.objectRealListActive.Count; num27++)
							{
								ObjectReal objectReal = GC.objectRealListActive[num27];

								if (!objectReal.fireProof && !objectReal.cantBeDestroyed && objectReal.damageAccumulates && !objectReal.notRealObject && !GC.tileInfo.DifferentLockdownZones(___agent.curTileData, objectReal.curTileData) && objectReal.objectName != "Door" && objectReal.objectName != "Window" && !objectReal.blocksView && objectReal.prisonObject == 0 && objectReal.curTileData.privateProperty2 == 0 && !objectReal.canStartFireInObject && !___agent.arsonTargets.Contains(objectReal))
								{
									bool flag13 = false;

									if (___agent.arsonTargets.Count > 0 && ___agent.arsonTargets[___agent.arsonTargets.Count - 1].objectName == objectReal.objectName)
										flag13 = true;
									
									if (!flag13 && Vector2.Distance(___agent.curPosition, objectReal.curPosition) > 1.28f && objectReal.fire == null)
									{
										bool flag14 = false;

										if (!___agent.didArson)
										{
											if (Vector2.Distance(objectReal.curPosition, agent6.curPosition) < 8f)
												flag14 = true;
										}
										else
											flag14 = true;
										
										if (objectReal.owner != 0 && objectReal.startingChunk != 0)
										{
											bool flag15 = false;

											for (int num28 = 0; num28 < GC.agentList.Count; num28++)
											{
												Agent agent8 = GC.agentList[num28];

												if (agent8.ownerID == objectReal.owner && agent8.startingChunk == objectReal.owner && !agent8.objectMultAgent._dead)
												{
													flag15 = true;
													break;
												}
											}

											if (!flag15)
												flag14 = false;
										}

										if (flag14)
											list2.Add(objectReal);
									}
								}
							}

							if (list2.Count > 0)
							{
								flag12 = true;
								___agent.curArsonTarget = list2[Random.Range(0, list2.Count)];
								___agent.arsonTargets.Add(___agent.curArsonTarget);

								if (___agent.curArsonTarget.objectAgent == null)
									___agent.curArsonTarget.SpawnObjectAgent();

								Debug.Log(string.Concat(new object[] { "Molotov Target: ", ___agent, " - ", ___agent.curArsonTarget }));
							}
						}
					}

					if (flag12)
						___curDesirability = 10000f;
					
					if (___curDesirability >= ___highestDesirability)
					{
						___highestDesirability = ___curDesirability;
						___tempGoalCommitArson.arsonObject = ___agent.curArsonTarget;
						goalType = goalType.CommitArson;
					}
				}
			}

			if (___highestDesirability < 9.5f)
			{
				float num29 = 0f;
				float num30 = 9999f;
				Noise noise = null;
				float curNoiseVol = 0f;
				bool flag16 = false;
				Noise noise2 = null;
				bool flag17 = false;

				if (___brain.Goals[0].goalCode == goalType.NoiseReact)
				{
					noise2 = ___agent.mostRecentNoise;
					flag17 = true;
				}
				else if (___agent.mostRecentGoalCode == goalType.NoiseReact && ___agent.mostRecentNoise.distraction)
					noise2 = ___agent.mostRecentNoise;
				
				if (noise2 != null && (noise2.isActiveAndEnabled || flag17))
				{
					float num31 = Vector2.Distance(___agent.curPosition, noise2.curPosition);
					float num32 = num31 * 100f * Mathf.Pow(___agent.curNoiseVol + 1f, 1.5f);
					bool flag18 = false;
					List<NoiseCrime> noiseCrimes = noise2.noiseCrimes;

					for (int num33 = 0; num33 < noiseCrimes.Count; num33++)
						if (noiseCrimes[num33].victim == ___agent)
						{
							num32 = num31 * 100f * Mathf.Pow(___agent.curNoiseVol + 1f, 1.5f);
							num32 *= 200f;
							num30 = num31;
							flag18 = true;
						}

					if (noise2.type == "DoorHelper" || noise2.type == "Alarm")
						num32 *= 10f;
					
					if (flag18)
						___curDesirability = 9.5f;
					else
						___curDesirability = 9f;
					
					noise = noise2;
					num29 = num32;
					curNoiseVol = ___agent.curNoiseVol;
				}

				List<Noise> noiseList = GC.noiseList;

				for (int num34 = 0; num34 < noiseList.Count; num34++)
				{
					Noise noise3 = noiseList[num34];
					float num31 = Vector2.Distance(___agent.curPosition, noise3.curPosition);

					if (noise3.agent != ___agent && ___brain.Goals[0].goalCode != goalType.Battle && noise3.state == 0 && !___agent.hasFollowing)
					{
						bool flag19 = true;

						if (___agent.pathing == 1)
						{
							if ((noise3.volume == 0f || noise3.volume == 0.2f || noise3.volume == 0.5f) && noise3.type != "BulletHitAgent")
								flag19 = false;
							
							if (___agent.jobCode == jobType.Follow)
								flag19 = false;
							
							if (___agent.canWarn != null)
								flag19 = false;
							
							if (___agent.curTileData.tracks && noise3.volume <= 1f)
								flag19 = false;
						}

						if (noise3.agentsHeard.Contains(___agent))
							flag19 = false;
						
						if ((noise3.volume == 0f || noise3.type != "BulletHitAgent") && ___agent.statusEffects.hasStatusEffect("HearingBlocked"))
							flag19 = false;
						
						if (noise3.volume == 0.2f && ___agent.mostRecentGoalCode != goalType.Guard)
							flag19 = false;
						
						if (noise3.type == "Trespassing" && ___agent.jobCode == jobType.Follow)
							flag19 = false;
						
						if (noise3.targetAgent != null && noise3.targetAgent != ___agent)
							flag19 = false;
						
						if (noise3.type == "LoudGuy" && noise3.playfieldObject != null && ___agent.jobCode == jobType.Follow && noise3.playfieldObject.playfieldObjectType == "Agent" && ((Agent)noise3.playfieldObject).isPlayer > 0)
							flag19 = false;
						
						if (___agent.dancing && noise3.volume <= 1f && noise3.type != "BulletHitAgent")
							flag19 = false;
						
						if (___agent.investigation == 1)
							flag19 = false;

						if (___agent.angryAtMayor)
						{
							bool flag20 = false;

							for (int num35 = 0; num35 < GC.agentList.Count; num35++)
								if (GC.agentList[num35].isMayor && GC.agentList[num35].dead)
								{
									flag20 = true;
									break;
								}
							
							if (!flag20)
								flag19 = false;
						}

						if (___agent.arenaBattler)
							flag19 = false;
						
						if (noise3.type == "Flee" && ___agent.zombified)
							flag19 = false;
						
						if (noise3.type == "Joke" && ((___agent.curTileData.chunkID != noise3.curTileData.chunkID && noise3.curTileData.owner != 0) || ___agent.curTileData.owner != noise3.curTileData.owner || (___agent.prisoner != noise3.curTileData.prison && !noise3.curTileData.prisonOpened)) && !___agent.answeringDoor)
							flag19 = false;
						
						bool flag21 = false;
						
						if (noise3.type == "DoorHelper")
						{
							if (___agent.curTileData.chunkID != noise3.curTileData.chunkID || ___agent.curTileData.owner != noise3.curTileData.owner || noise3.doorHelperAgent != ___agent || ___agent.prisoner != 0)
							{
								flag19 = false;
								flag21 = true;
							}
						}
						else if (noise3.type == "WindowHelper")
						{
							if (!GC.tileInfo.IsIndoors(___agent.curPosition) && noise3.windowHelperAgent != ___agent)
								flag19 = false;
							else if ((___agent.curTileData.chunkID != noise3.curTileData.chunkID || ___agent.curTileData.owner != noise3.curTileData.owner) && noise3.windowHelperAgent != ___agent)
								flag19 = false;
							else if (noise3.windowHelperAgent != ___agent)
								flag19 = false;
							else if (noise3.windowHelperAgent == ___agent && noise3.outsideWindowNoise != null)
							{
								Noise outsideWindowNoise = noise3.outsideWindowNoise;
								float num36 = Vector2.Distance(___agent.curPosition, outsideWindowNoise.curPosition);
								float noiseDistance = noise3.noiseDistance;
							
								if (num36 >= noiseDistance)
									flag19 = false;
							}
						}
						else if (noise3.type == "Alarm" && (___agent.startingChunk != noise3.playfieldObject.startingChunk || (___agent.ownerID != noise3.curTileData.owner && ___agent.ownerID != 99) || (___agent.startingSector != 0 && ___agent.startingSector != noise3.playfieldObject.startingSector)))
							flag19 = false;
						
						if (___agent.sleeping && noise3.type != "WindowHelper" && (!(noise3.type == "DoorHelper") || !flag21) && noise3.volume < 3f && !___agent.statusEffects.hasStatusEffect("HearingBlocked"))
							flag19 = (noise3.curPosition.x < 4999f && noise3.curTileData.owner > 0);
						else if (___agent.sleeping && noise3.type == "WindowHelper" && noise3.type2 != "RapOnWindow" && noise3.volume < 3f)
							flag19 = false;
						
						if (flag19 && (num31 < Mathf.Clamp(noise3.noiseDistance, 0f, ___agent.hearingRange) || (noise3.bulletMadeNoise && num31 < 10.8f)))
						{
							float num32 = num31 * 100f * Mathf.Pow(noise3.volume + 1f, 1.5f);
							bool flag22 = false;
							List<NoiseCrime> noiseCrimes2 = noise3.noiseCrimes;
						
							for (int num37 = 0; num37 < noiseCrimes2.Count; num37++)
								if (noiseCrimes2[num37].victim == ___agent && (!___agent.investigatingCrime || num31 != num30))
								{
									num32 *= 10f;
									flag22 = true;
								}
							
							if (noise3.type == "DoorHelper")
							{
								num32 *= 10f;

								if (noise != null && noise.type == "Joke")
									num32 = 0f;
							}

							if (noise3.type == "Alarm")
							{
								num32 *= 10f;
							
								if (noise != null && noise.type == "Joke")
									num32 = 0f;
							}

							if (noise3.type == "Joke")
							{
								num32 *= 10f;
							
								if (noise != null && noise.type == "DoorHelper")
									num32 = num29 + 1f;
							}

							if (num32 >= num29)
							{
								if (flag22)
									___curDesirability = 9.5f;
								else
									___curDesirability = 9f;
							
								noise = noise3;
								num29 = num32;
								curNoiseVol = noise3.volume;
							}

							if (noise3.type == "WindowHelper" && noise3.windowHelperAgent == ___agent && !flag16 && ___brain.Goals[0].goalCode != goalType.NoiseReact)
							{
								___curDesirability = 9f;
								noise = noise3;
								num29 = 9999999f;
								curNoiseVol = noise3.volume;
							}
							
							if (noise3.type == "WindowHelper" && noise3.windowHelperAgent == ___agent && noise3.closestWindow != null && !flag16 && ___brain.Goals[0].goalCode != goalType.NoiseReact)
							{
								___curDesirability = 9f;
								noise = noise3;
								num29 = 99999990f;
								flag16 = true;
								curNoiseVol = noise3.volume;
							}
						}
					}
				}

				if (___curDesirability >= ___highestDesirability && noise != null)
				{
					___highestDesirability = ___curDesirability;
					___tempGoalNoiseReact.noise = noise;
					___tempGoalNoiseReact.noisePosition = noise.curPosition;
					goalType = goalType.NoiseReact;
					___agent.curNoiseVol = curNoiseVol;
					___agent.mostRecentNoise = noise;
				}
			}

			if (GC.dangerList.Count > 0)
			{
				___curDesirability = 0f;
				Danger danger3 = null;
				List<Danger> dangerList = GC.dangerList;

				for (int num38 = 0; num38 < dangerList.Count; num38++)
				{
					Danger danger4 = dangerList[num38];

					if ((danger4.dangerTypeCode == dangerStatus.Gas || danger4.dangerTypeCode == dangerStatus.Fire || danger4.dangerTypeCode == dangerStatus.BadMusic1 || danger4.dangerTypeCode == dangerStatus.BadMusic2 || danger4.dangerTypeCode == dangerStatus.Water) && danger4.startingChunk == ___agent.startingChunk)
					{
						bool flag23 = true;

						if (danger4.dangerTypeCode == dangerStatus.Fire && danger4.playfieldObject != null && !___tileInfo.IsIndoors(danger4.playfieldObject.tr.position))
							flag23 = false;
						
						if (danger4.dangerTypeCode == dangerStatus.Gas && (___agent.statusEffects.hasStatusEffect("ResistGas") || ___agent.preventStatusEffects || ___agent.hasEmployer || (___agent.firefighter && (___agent.mostRecentGoalCode == goalType.FindFire || ___agent.mostRecentGoalCode == goalType.Battle))))
							flag23 = false;
						
						if (danger4.dangerTypeCode == dangerStatus.Water && ___agent.hasEmployer)
							flag23 = false;
						
						if (danger4.dangerTypeCode == dangerStatus.BadMusic1)
						{
							if (___agent.ownerID != 0)
								flag23 = false;
						}
						else if (danger4.dangerTypeCode == dangerStatus.BadMusic2 && ___agent.ownerID != 0)
							flag23 = false;
						
						if (flag23)
						{
							___curDesirability = 6f;
							danger3 = danger4;
						}
					}
				}

				if (___curDesirability >= ___highestDesirability)
				{
					___highestDesirability = ___curDesirability;
					___tempGoalWaitForDangerEnd.danger = danger3;
					goalType = goalType.WaitForDangerEnd;
				}
			}

			if (___agent.defaultGoalCode == goalType.Guard && !___agent.hasFled && ___highestDesirability < 5f)
			{
				___curDesirability = 5f;

				if (___curDesirability >= ___highestDesirability)
				{
					___highestDesirability = ___curDesirability;
					goalType = goalType.Guard;
				}
			}

			if (___agent.defaultGoalCode == goalType.Patrol && !___agent.hasFled && ___highestDesirability < 5f)
			{
				___curDesirability = 5f;
				PatrolPointInterest patrolPointInterest = null;
				List<PatrolPointInterest> patrolPointInterests = ___agent.patrolPointInterests;

				for (int num39 = 0; num39 < patrolPointInterests.Count; num39++)
				{
					PatrolPointInterest patrolPointInterest2 = patrolPointInterests[num39];

					if (patrolPointInterest2.patrolPoint.pointNum == ___agent.curPatrolPoint)
						patrolPointInterest = patrolPointInterest2;
				}

				if (___curDesirability >= ___highestDesirability && patrolPointInterest != null && patrolPointInterest.patrolPoint != null)
				{
					___highestDesirability = ___curDesirability;
					___tempGoalPatrol.patrolPointInterest = patrolPointInterest;
					___tempGoalPatrol.patrolPoint = patrolPointInterest.patrolPoint;
					goalType = goalType.Patrol;
				}
			}

			if ((___agent.defaultGoalCode == goalType.None || ___agent.defaultGoalCode == goalType.CuriousObject) && ___agent.interests.Count > 0)
			{
				___curDesirability = 0f;
				float num40 = 0f;
				Interest interest = null;

				if (___agent.interest == null)
				{
					if (___highestDesirability < 3f)
					{
						List<Interest> interests = ___agent.interests;

						for (int num41 = 0; num41 < interests.Count; num41++)
						{
							Interest interest2 = interests[num41];
							bool flag24 = true;

							if (interest2.playfieldObject.CompareTag("Agent") && !((Agent)interest2.playfieldObject).objectMultAgent._dead)
								flag24 = false;

							if (flag24)
							{
								float num42 = Vector2.Distance(___agent.curPosition, interest2.playfieldObject.curPosition);

								if (interests.Count == 1)
									interest2.interestValue = interest2.interestValueMax;
								
								float num43 = 30f * interest2.interestValue / (num42 * 100f);
								
								if (num43 > num40)
								{
									___curDesirability = Mathf.Clamp(num43, 0f, 3f);
									interest = interest2;
									num40 = num43;
								}
							}
						}
					}
				}
				else
				{
					___curDesirability = 3f;
					interest = ___agent.interest;
				}

				if (___curDesirability >= ___highestDesirability && interest != null && interest.playfieldObject != null)
				{
					___highestDesirability = ___curDesirability;
					___tempGoalCuriousObject.interest = interest;
					___tempGoalCuriousObject.playfieldObject = interest.playfieldObject;
					goalType = goalType.CuriousObject;
				}
			}

			if (___agent.investigation > 0 && ___highestDesirability < 0.9f)
			{
				___curDesirability = 0.9f;

				if (___curDesirability >= ___highestDesirability)
				{
					___highestDesirability = ___curDesirability;
					___tempGoalInvestigate.investigatePosition = ___agent.investigatePosition;
					goalType = goalType.Investigate;
				}
			}

			if (___agent.defaultGoalCode == goalType.GetElectionResults && ___highestDesirability < 2f)
			{
				___curDesirability = 2f;

				if (___curDesirability >= ___highestDesirability)
				{
					___highestDesirability = ___curDesirability;
					goalType = goalType.GetElectionResults;
				}
			}

			if (___agent.defaultGoalCode == goalType.Wander && ___highestDesirability < 2f)
			{
				___curDesirability = 2f;

				if (___curDesirability >= ___highestDesirability)
				{
					___highestDesirability = ___curDesirability;
					goalType = goalType.Wander;
				}
			}

			___agent.dontWalkGang = false;

			if ((___agent.defaultGoalCode == goalType.WanderFar && ___highestDesirability < 2f) || (___agent.enraged && ___curDesirability < 7f))
			{
				if (___agent.enraged)
					___curDesirability = 7f;
				else
					___curDesirability = 2f;
			
				if (___curDesirability >= ___highestDesirability)
				{
					___highestDesirability = ___curDesirability;
					goalType = goalType.WanderFar;
				}
			}

			if (___agent.defaultGoalCode == goalType.WanderInOwnedProperty && ___highestDesirability < 2f)
			{
				___curDesirability = 2f;
			
				if (___curDesirability >= ___highestDesirability)
				{
					___highestDesirability = ___curDesirability;
					goalType = goalType.WanderInOwnedProperty;
				}
			}

			if (___agent.defaultGoalCode == goalType.Sleep && ___highestDesirability < 1.1f && !___agent.hasTalkText)
			{
				if (___agent.cantSleep && ___agent.modLeashes == 1)
					___agent.SetDefaultGoal("WanderInOwnedProperty");
				else if (___agent.cantSleep)
					___agent.SetDefaultGoal("WanderInOwnedProperty");
				else
				{
					___curDesirability = 1.1f;
				
					if (___curDesirability >= ___highestDesirability)
					{
						___highestDesirability = ___curDesirability;
						goalType = goalType.Sleep;
					}
				}
			}

			if (___agent.defaultGoalCode == goalType.Sit && ___highestDesirability < 1.1f)
			{
				___curDesirability = 1.1f;

				if (___curDesirability >= ___highestDesirability)
				{
					___highestDesirability = ___curDesirability;
					goalType = goalType.Sit;
				}
			}

			if (___agent.defaultGoalCode == goalType.Dance && ___highestDesirability < 1.1f)
			{
				___curDesirability = 1.1f;

				if (___curDesirability >= ___highestDesirability)
				{
					___highestDesirability = ___curDesirability;
					goalType = goalType.Dance;
				}
			}

			if (___agent.defaultGoalCode == goalType.IceSkate && ___highestDesirability < 1.1f)
			{
				___curDesirability = 1.1f;

				if (___curDesirability >= ___highestDesirability)
				{
					___highestDesirability = ___curDesirability;
					goalType = goalType.IceSkate;
				}
			}

			if (___agent.defaultGoalCode == goalType.Swim && ___highestDesirability < 1.1f)
			{
				___curDesirability = 1.1f;

				if (___curDesirability >= ___highestDesirability)
				{
					___highestDesirability = ___curDesirability;
					goalType = goalType.Swim;
				}
			}

			if (___agent.defaultGoalCode == goalType.Joke && ___highestDesirability < 1.1f)
			{
				___curDesirability = 1.1f;

				if (___curDesirability >= ___highestDesirability)
				{
					___highestDesirability = ___curDesirability;
					goalType = goalType.Joke;
				}
			}

			if (___agent.defaultGoalCode == goalType.ListenToJokeNPC && ___highestDesirability < 1.1f)
			{
				___curDesirability = 1.1f;

				if (___curDesirability >= ___highestDesirability)
				{
					___highestDesirability = ___curDesirability;
					goalType = goalType.ListenToJokeNPC;
				}
			}

			if (___agent.Tripped() || ___agent.dizzy || ___agent.electrocuted || ___agent.hasGettingArrestedByAgent || ___agent.relationships.joiningParty)
			{
				___curDesirability = 1000000f;

				if (___curDesirability >= ___highestDesirability)
				{
					___highestDesirability = ___curDesirability;
					goalType = goalType.DoNothing;
				}
			}

			if (___agent.killerRobot)
			{
				___curDesirability = 10000000f;

				if (___curDesirability >= ___highestDesirability)
				{
					___highestDesirability = ___curDesirability;
					___tempGoalRobotFollow.followingAgent = null;
					___tempGoalRobotFollow.followingAgent = GC.playerAgent;

					if (playerAgentList.Count > 1)
					{
						float num44 = 1000000f;
						Agent agent9 = GC.playerAgent;

						for (int num45 = 0; num45 < playerAgentList.Count; num45++)
						{
							Agent agent10 = playerAgentList[num45];

							if (Vector2.Distance(agent10.curPosition, ___agent.curPosition) < num44 && !agent10.objectMultAgent._dead && !agent10.ghost && !agent10.finishedLevel)
							{
								agent9 = agent10;
								num44 = Vector2.Distance(agent9.curPosition, ___agent.curPosition);
								___tempGoalRobotFollow.followingAgent = agent9;
							}
						}
					}

					List<Agent> agentList = GC.agentList;
					int num46 = 0;

					while (num46 < agentList.Count)
					{
						Agent agent11 = agentList[num46];

						if (agent11.hologram && !agent11.disappeared)
						{
							if (Vector2.Distance(agent11.curPosition, ___agent.curPosition) < Vector2.Distance(___tempGoalRobotFollow.followingAgent.curPosition, ___agent.curPosition))
							{
								___tempGoalRobotFollow.followingAgent = agent11;

								break;
							}

							break;
						}
						else
							num46++;
					}

					if (___tempGoalRobotFollow.followingAgent != null)
					{
						___agent.SetOpponent(___tempGoalRobotFollow.followingAgent);
						goalType = goalType.RobotFollow;
					}
				}
			}

			if (___agent.butlerBot)
			{
				___curDesirability = 10000000f;

				if (___curDesirability >= ___highestDesirability)
				{
					___highestDesirability = ___curDesirability;
					___tempGoalRobotClean.cleaningObject = null;
					bool flag25 = true;
				
					if (___agent.mostRecentCleaningFocus != null && ___agent.mostRecentCleaningFocus.isAgent)
					{
						Agent playfieldObjectAgent = ___agent.mostRecentCleaningFocus.playfieldObjectAgent;
					
						flag25 = (!playfieldObjectAgent.objectMultAgent._dead || playfieldObjectAgent.resurrect || playfieldObjectAgent.FellInHole() || playfieldObjectAgent.disappeared || playfieldObjectAgent.deadTime < 3f || playfieldObjectAgent.oma.bodyGuarded || GC.tileInfo.DifferentLockdownZones(___agent.curTileData, playfieldObjectAgent.curTileData) || GC.tileInfo.IsIndoors(playfieldObjectAgent.curPosition, false));
					}

					if (flag25)
					{
						float num47 = 1000000f;
						Agent agent12 = null;

						for (int num48 = 0; num48 < GC.agentList.Count; num48++)
						{
							Agent agent13 = GC.agentList[num48];

							if (agent13.isPlayer == 0 && !agent13.resurrect && agent13.objectMultAgent._dead && !agent13.FellInHole() && !agent13.disappeared && agent13.deadTime >= 3f && !agent13.oma.bodyGuarded && !GC.tileInfo.DifferentLockdownZones(___agent.curTileData, agent13.curTileData) && !GC.tileInfo.IsIndoors(agent13.curPosition, false))
							{
								float num49 = Vector2.Distance(agent13.curPosition, ___agent.curPosition);

								if (num49 < num47)
								{
									bool flag26 = true;

									if (GC.butlerBotList.Count > 1)
										for (int num50 = 0; num50 < GC.butlerBotList.Count; num50++)
											if (GC.butlerBotList[num50].mostRecentCleaningFocus == agent13)
												flag26 = false;
									
									if (agent13.QuestInvolvementFull())
										flag26 = false;
									
									if (flag26)
									{
										num47 = num49;
										agent12 = agent13;
									}
								}
							}
						}

						if (agent12 != null)
						{
							___agent.mostRecentCleaningFocus = agent12;
							___tempGoalRobotClean.cleaningObject = agent12;
						}
					}
					else
						___tempGoalRobotClean.cleaningObject = ___agent.mostRecentCleaningFocus;
					
					goalType = goalType.RobotClean;
				}
			}

			if (___highestDesirability == 0f)
			{
				___curDesirability = 1f;

				if (___curDesirability >= ___highestDesirability)
				{
					___highestDesirability = ___curDesirability;
					goalType = goalType.Idle;
				}
			}

			if (GC.cinematic || ___agent.cinematic || (GC.levelType == "Tutorial" && ___agent.agentName == "ResistanceLeader"))
			{
				___curDesirability = 1000000f;

				if (___curDesirability >= ___highestDesirability)
				{
					___highestDesirability = ___curDesirability;
					goalType = goalType.Cinematic;
				}
			}

			if ((___agent.mostRecentGoalCode == goalType.NoiseReact && goalType != goalType.NoiseReact) || (___agent.mostRecentGoalCode == goalType.DoJob && goalType != goalType.DoJob))
				__instance.CancelSearchStaticStateIndicator();

			if (___agent.mostRecentGoalCode == goalType.Battle && goalType != goalType.Battle)
			{
				if (!___agent.objectMultAgent._dead && ___agent.agentName == "Assassin" && ___agent.gang > 0)
					___agent.countdownToInvisible = 10;
			}
			else if (___agent.countdownToInvisible > 0)
			{
				___agent.countdownToInvisible--;
			
				if (___agent.countdownToInvisible == 0)
				{
					if (!___agent.objectMultAgent._dead && !___agent.zombified && goalType != goalType.Battle && goalType != goalType.DoNothing && goalType != goalType.Interact && goalType != goalType.Cinematic && ___agent.slaveOwners.Count == 0)
						___agent.statusEffects.AddStatusEffect("InvisiblePermanent", false);
					else if (!___agent.objectMultAgent._dead)
						___agent.countdownToInvisible = 1;
				}
			}

			switch (goalType)
			{
				case goalType.Battle:
					if (goalType != ___brain.Goals[0].goalCode)
					{
						if (___agent.mostRecentGoalCode != goalType.Battle && ___agent.mostRecentGoalCode != goalType.Flee)
						{
							if (!___tempGoalBattle.battlingAgent.objectAgent && (!___agent.justGotUp || !___agent.battlingBeforeTrip) && (((!___agent.oma.rioter || ___agent.isPlayer != 0) && !___agent.warZoneAgent) || ___tempGoalBattle.battlingAgent.isPlayer != 0) && (((!___tempGoalBattle.battlingAgent.oma.rioter || ___tempGoalBattle.battlingAgent.isPlayer != 0) && !___tempGoalBattle.battlingAgent.warZoneAgent) || ___agent.isPlayer != 0))
							{
								bool flag27 = true;

								if (___agent.zombified)
								{
									flag27 = false;
								
									if (___agent.agentName == "Zombie" && GC.challenges.Contains("ZombiesWelcome"))
										flag27 = true;
								}

								if (flag27)
								{
									GC.spawnerMain.SpawnStateIndicator(___agent, "StartBattle");
								
									if (___tempGoalBattle.battlingAgent != null)
										___agent.relationships.HurtFriendCheck2(___tempGoalBattle.battlingAgent);
								}
							}

							___agent.battlingBeforeTrip = false;
							___agent.relationships.GetRelationship(___tempGoalBattle.battlingAgent).canBeSetBack = false;

							if (___agent.isPlayer == 0 && !___tempGoalBattle.battlingAgent.objectAgent && (!___agent.justGotUp || !___agent.battlingBeforeTrip) && ___agent.SayDialogue("StartBattle") == "")
								___agent.Say("");
							
							if (___agent.agentName == "Assassin" && ___agent.gang > 0)
								___agent.statusEffects.RemoveStatusEffect("InvisiblePermanent", false);
							
							if (___agent.relationships.GetRelationship(___tempGoalBattle.battlingAgent).hasLOS360)
								GC.spawnerMain.SpawnNoise(___tempGoalBattle.battlingAgent.tr.position, 1f, ___agent, null, ___tempGoalBattle.battlingAgent).playfieldObject = ___agent;
							
							if (___tempGoalBattle.battlingAgent.isPlayer > 0)
							{
								Relationship relationship2 = ___agent.relationships.GetRelationship(___tempGoalBattle.battlingAgent);
							
								if (!relationship2.didAnger)
									___agent.relationships.AddToAngerStat(___tempGoalBattle.battlingAgent, relationship2);
								
								if (!relationship2.didHostile)
								{
									relationship2.didHostile = true;
									GC.stats.AddToStat(___tempGoalBattle.battlingAgent, "Hostiled", 1);
								}
							}
						}

						if ((___agent.oma.secretWerewolf || ___agent.originalWerewolf) && ___agent.isPlayer == 0 && !___agent.zombified && (___agent.agentName != "WerewolfB" || ___agent.specialAbility == "WerewolfTransform"))
						{
							bool flag28 = false;

							if (___agent.inventory.equippedSpecialAbility != null)
							{
								if (___agent.oma.secretWerewolf)
									flag28 = true;
								else if (___agent.inventory.equippedSpecialAbility.invItemCount == 0)
									flag28 = true;
							}
							else
								flag28 = true;
							
							if (flag28)
							{
								___agent.statusEffects.WerewolfTransformStart();
								___agent.objectMult.SpecialAbility("WerewolfTransform", null);
							}
						}

						__instance.CancelSearchStaticStateIndicator();
						__instance.SwitchGoal(new GoalBattle
						{
							battlingAgent = ___tempGoalBattle.battlingAgent
						});
						___agent.agentActive = true;
						___agent.cantSleep = true;
						___agent.SetRunBackToPosition(true);
					}
					else if (((GoalBattle)___brain.Goals[0]).battlingAgent != ___tempGoalBattle.battlingAgent)
					{
						__instance.SwitchGoal(new GoalBattle
						{
							battlingAgent = ___tempGoalBattle.battlingAgent
						});
					}

					break;

				case goalType.Flee:
					if (goalType != ___brain.Goals[0].goalCode)
					{
						if (___agent.mostRecentGoalCode != goalType.Flee && ___agent.isPlayer == 0 && !___tempGoalFlee.fleeingAgent.objectAgent && !___agent.hasStealingFromAgent && ___agent.SayDialogue("FleeBattle") == "")
							___agent.Say("");

						if ((___agent.oma.secretWerewolf || ___agent.originalWerewolf) && ___agent.isPlayer == 0 && !___agent.zombified && (___agent.agentName != "WerewolfB" || ___agent.specialAbility == "WerewolfTransform") && ___agent.inventory.equippedSpecialAbility.invItemCount == 0)
						{
							___agent.statusEffects.WerewolfTransformStart();
							___agent.objectMult.SpecialAbility("WerewolfTransform", null);
						}

						__instance.CancelSearchStaticStateIndicator();
						__instance.SwitchGoal(new GoalFlee
						{
							fleeingAgent = ___tempGoalFlee.fleeingAgent
						});
						___agent.agentActive = true;
						___agent.cantSleep = true;
						___agent.SetRunBackToPosition(true);
						___agent.hasFled = true;

						if (___agent.defaultGoalCode != goalType.WanderFar && ___agent.ownerID != 0 && !___agent.shrunk && !___agent.oma.shookDown && (!___tempGoalFlee.fleeingAgent.statusEffects.hasTrait("Shakedowner") || !___agent.CanShakeDown()))
							___agent.SetDefaultGoal("WanderFar");
					}
					else if (((GoalFlee)___brain.Goals[0]).fleeingAgent != ___tempGoalFlee.fleeingAgent)
						__instance.SwitchGoal(new GoalFlee
						{
							fleeingAgent = ___tempGoalFlee.fleeingAgent
						});
					
					break;
				case goalType.FleeDanger:
					if (goalType != ___brain.Goals[0].goalCode)
					{
						if (___agent.mostRecentGoalCode != goalType.FleeDanger && ___agent.isPlayer == 0 && ___agent.SayDialogue("FleeDanger") == "")
							___agent.Say("");
						
						__instance.CancelSearchStaticStateIndicator();
						__instance.SwitchGoal(new GoalFleeDanger
						{
							danger = ___tempGoalFleeDanger.danger,
							dangerType = ___tempGoalFleeDanger.dangerType,
							dangerTypeCode = ___tempGoalFleeDanger.dangerTypeCode,
							dangerPos = ___tempGoalFleeDanger.dangerPos
						});
						___agent.agentActive = true;
						___agent.cantSleep = true;
						___agent.SetRunBackToPosition(true);
					}
					else if (((GoalFleeDanger)___brain.Goals[0]).danger != ___tempGoalFleeDanger.danger)
					{
						__instance.SwitchGoal(new GoalFleeDanger

						{
							danger = ___tempGoalFleeDanger.danger,
							dangerType = ___tempGoalFleeDanger.dangerType,
							dangerTypeCode = ___tempGoalFleeDanger.dangerTypeCode,
							dangerPos = ___tempGoalFleeDanger.dangerPos
						});
					}

					break;
				case goalType.Follow:
					if (goalType != ___brain.Goals[0].goalCode)
					{
						__instance.CancelSearchStaticStateIndicator();
						__instance.SwitchGoal(new GoalFollow
						{
							followingAgent = ___tempGoalFollow.followingAgent
						});
						___agent.agentActive = true;
						___agent.SetRunBackToPosition(true);
					}
					else if (((GoalFollow)___brain.Goals[0]).followingAgent != ___tempGoalFollow.followingAgent)
					{
						__instance.SwitchGoal(new GoalFollow
						{
							followingAgent = ___tempGoalFollow.followingAgent
						});
					}

					break;
				case goalType.Interact:
					if (goalType != ___brain.Goals[0].goalCode)
					{
						__instance.CancelSearchStaticStateIndicator();
						__instance.SwitchGoal(new GoalInteract
						{
							interactingAgent = ___tempGoalInteract.interactingAgent
						});
						___agent.agentActive = true;
					}
					else if (((GoalInteract)___brain.Goals[0]).interactingAgent != ___tempGoalInteract.interactingAgent)
					{
						__instance.SwitchGoal(new GoalInteract
						{
							interactingAgent = ___tempGoalInteract.interactingAgent
						});
					}

					break;
				case goalType.WalkToExit:
					if (goalType != ___brain.Goals[0].goalCode)
					{
						__instance.CancelSearchStaticStateIndicator();
						__instance.SwitchGoal(new GoalWalkToExit
						{
							myObject = ___tempGoalWalkToExit.myObject,
							escorting = ___tempGoalWalkToExit.escorting,
							myFinalDestPosition = ___tempGoalWalkToExit.myFinalDestPosition
						});
						___agent.agentActive = false;
					}

					break;
				case goalType.DoJob:
					if (goalType != ___brain.Goals[0].goalCode)
					{
						__instance.CancelSearchStaticStateIndicator();
						__instance.SwitchGoal(new GoalDoJob
						{
							workingForAgent = ___tempGoalDoJob.workingForAgent,
							curJob = ___tempGoalDoJob.curJob,
							curJobCode = ___tempGoalDoJob.curJobCode,
							assignedPos = ___tempGoalDoJob.assignedPos,
							assignedAgent = ___tempGoalDoJob.assignedAgent,
							assignedObject = ___tempGoalDoJob.assignedObject
						});
						___agent.agentActive = true;
						___agent.SetRunBackToPosition(true);
					}
					else
					{
						GoalDoJob goalDoJob = (GoalDoJob)___brain.Goals[0];

						if (goalDoJob.workingForAgent != ___tempGoalDoJob.workingForAgent)
							__instance.SwitchGoal(new GoalDoJob
							{
								workingForAgent = ___tempGoalDoJob.workingForAgent,
								curJob = ___tempGoalDoJob.curJob,
								curJobCode = ___tempGoalDoJob.curJobCode,
								assignedPos = ___tempGoalDoJob.assignedPos,
								assignedAgent = ___tempGoalDoJob.assignedAgent,
								assignedObject = ___tempGoalDoJob.assignedObject
							});
						else if (goalDoJob.curJobCode != ___tempGoalDoJob.curJobCode)
							__instance.SwitchGoal(new GoalDoJob
							{
								workingForAgent = ___tempGoalDoJob.workingForAgent,
								curJob = ___tempGoalDoJob.curJob,
								curJobCode = ___tempGoalDoJob.curJobCode,
								assignedPos = ___tempGoalDoJob.assignedPos,
								assignedAgent = ___tempGoalDoJob.assignedAgent,
								assignedObject = ___tempGoalDoJob.assignedObject
							});
						else if (goalDoJob.assignedPos != ___tempGoalDoJob.assignedPos)
							__instance.SwitchGoal(new GoalDoJob
							{
								workingForAgent = ___tempGoalDoJob.workingForAgent,
								curJob = ___tempGoalDoJob.curJob,
								curJobCode = ___tempGoalDoJob.curJobCode,
								assignedPos = ___tempGoalDoJob.assignedPos,
								assignedAgent = ___tempGoalDoJob.assignedAgent,
								assignedObject = ___tempGoalDoJob.assignedObject
							});
						else if (goalDoJob.assignedAgent != ___tempGoalDoJob.assignedAgent)
							__instance.SwitchGoal(new GoalDoJob
							{
								workingForAgent = ___tempGoalDoJob.workingForAgent,
								curJob = ___tempGoalDoJob.curJob,
								curJobCode = ___tempGoalDoJob.curJobCode,
								assignedPos = ___tempGoalDoJob.assignedPos,
								assignedAgent = ___tempGoalDoJob.assignedAgent,
								assignedObject = ___tempGoalDoJob.assignedObject
							});
						else if (goalDoJob.assignedObject != ___tempGoalDoJob.assignedObject)
							__instance.SwitchGoal(new GoalDoJob
							{
								workingForAgent = ___tempGoalDoJob.workingForAgent,
								curJob = ___tempGoalDoJob.curJob,
								curJobCode = ___tempGoalDoJob.curJobCode,
								assignedPos = ___tempGoalDoJob.assignedPos,
								assignedAgent = ___tempGoalDoJob.assignedAgent,
								assignedObject = ___tempGoalDoJob.assignedObject
							});
					}

					break;
				case goalType.NoiseReact:
					if (goalType != ___brain.Goals[0].goalCode)
					{
						__instance.CancelSearchStaticStateIndicator();
						__instance.SwitchGoal(new GoalNoiseReact
						{
							noise = ___tempGoalNoiseReact.noise,
							noisePosition = ___tempGoalNoiseReact.noisePosition
						});
						___agent.agentActive = true;
						___agent.goBackToPosition = true;
					}
					else if (((GoalNoiseReact)___brain.Goals[0]).noise != ___tempGoalNoiseReact.noise)
						__instance.SwitchGoal(new GoalNoiseReact
						{
							noise = ___tempGoalNoiseReact.noise,
							noisePosition = ___tempGoalNoiseReact.noisePosition
						});
					
					if (___agent.gang != 0)
					{
						List<Agent> gangMembers2 = ___agent.gangMembers;
					
						for (int num51 = 0; num51 < gangMembers2.Count; num51++)
						{
							Agent agent14 = gangMembers2[num51];
						
							if (agent14.gang == ___agent.gang && !agent14.objectMultAgent._dead && agent14 != ___agent && (agent14.mostRecentGoalCode == goalType.WanderFar || agent14.mostRecentGoalCode == goalType.GetElectionResults) && !___agent.investigatingCrime)
								___agent.dontWalkGang = true;
						}
					}

					break;
				case goalType.WaitForDangerEnd:
					if (goalType != ___brain.Goals[0].goalCode)
					{
						__instance.CancelSearchStaticStateIndicator();
						__instance.SwitchGoal(new GoalWaitForDangerEnd
						{
							danger = ___tempGoalWaitForDangerEnd.danger
						});
						___agent.agentActive = true;
					}

					break;
				case goalType.Patrol:
					if (goalType != ___brain.Goals[0].goalCode)
					{
						__instance.CancelSearchStaticStateIndicator();
						__instance.SwitchGoal(new GoalPatrol
						{
							patrolPoint = ___tempGoalPatrol.patrolPoint,
							patrolPointInterest = ___tempGoalPatrol.patrolPointInterest
						});
						___agent.agentActive = false;
					}
					else if (((GoalPatrol)___brain.Goals[0]).patrolPointInterest != ___tempGoalPatrol.patrolPointInterest)
						__instance.SwitchGoal(new GoalPatrol
						{
							patrolPoint = ___tempGoalPatrol.patrolPoint,
							patrolPointInterest = ___tempGoalPatrol.patrolPointInterest
						});

					break;
				case goalType.CuriousObject:
					if (goalType != ___brain.Goals[0].goalCode)
					{
						__instance.CancelSearchStaticStateIndicator();
						__instance.SwitchGoal(new GoalCuriousObject
						{
							interest = ___tempGoalCuriousObject.interest,
							playfieldObject = ___tempGoalCuriousObject.playfieldObject
						});
						___agent.agentActive = false;
					}
					else if (((GoalCuriousObject)___brain.Goals[0]).interest != ___tempGoalCuriousObject.interest)
					{
						__instance.SwitchGoal(new GoalCuriousObject
						{
							interest = ___tempGoalCuriousObject.interest,
							playfieldObject = ___tempGoalCuriousObject.playfieldObject
						});
					}

					break;
				case goalType.Investigate:
					if (goalType != ___brain.Goals[0].goalCode)
					{
						__instance.CancelSearchStaticStateIndicator();
						__instance.SwitchGoal(new GoalInvestigate
						{
							investigatePosition = ___tempGoalInvestigate.investigatePosition
						});
						___agent.agentActive = true;
					}
					else if (((GoalInvestigate)___brain.Goals[0]).investigatePosition != ___tempGoalInvestigate.investigatePosition)
						__instance.SwitchGoal(new GoalInvestigate
						{
							investigatePosition = ___tempGoalInvestigate.investigatePosition
						});

					break;
				case goalType.Wander:
					if (goalType != ___brain.Goals[0].goalCode)
					{
						__instance.CancelSearchStaticStateIndicator();
						GoalWander newGoal = new GoalWander();
						__instance.SwitchGoal(newGoal);
						___agent.agentActive = false;
					}

					break;
				case goalType.Sleep:
					if (goalType != ___brain.Goals[0].goalCode)
					{
						__instance.CancelSearchStaticStateIndicator();
						GoalSleep newGoal2 = new GoalSleep();
						__instance.SwitchGoal(newGoal2);
						___agent.agentActive = false;

						if (Vector2.Distance(___agent.curPosition, ___agent.originalLocation) > 0.4f)
							___agent.agentActive = true;
					}

					break;
				case goalType.DoNothing:
					if (goalType != ___brain.Goals[0].goalCode)
					{
						__instance.CancelSearchStaticStateIndicator();
						GoalDoNothing newGoal3 = new GoalDoNothing();
						__instance.SwitchGoal(newGoal3);
						___agent.agentActive = true;
					}

					break;
				case goalType.RobotFollow:
					if (goalType != ___brain.Goals[0].goalCode)
					{
						__instance.CancelSearchStaticStateIndicator();
						__instance.SwitchGoal(new GoalRobotFollow
						{
							followingAgent = ___tempGoalRobotFollow.followingAgent
						});
						___agent.agentActive = true;
					}
					else if (((GoalRobotFollow)___brain.Goals[0]).followingAgent != ___tempGoalRobotFollow.followingAgent)
						__instance.SwitchGoal(new GoalRobotFollow
						{
							followingAgent = ___tempGoalRobotFollow.followingAgent
						});
					
					break;
				case goalType.Idle:
					if (goalType != ___brain.Goals[0].goalCode)
					{
						__instance.CancelSearchStaticStateIndicator();
						GoalIdle newGoal4 = new GoalIdle();
						__instance.SwitchGoal(newGoal4);
						___agent.agentActive = false;
					}

					break;
				case goalType.Cinematic:
					if (goalType != ___brain.Goals[0].goalCode)
					{
						__instance.CancelSearchStaticStateIndicator();
						GoalCinematic newGoal5 = new GoalCinematic();
						__instance.SwitchGoal(newGoal5);
						___agent.agentActive = false;
					}

					break;
				case goalType.Guard:
					if (goalType != ___brain.Goals[0].goalCode)
					{
						__instance.CancelSearchStaticStateIndicator();
						GoalGuard newGoal6 = new GoalGuard();
						__instance.SwitchGoal(newGoal6);
						___agent.agentActive = false;

						if (Vector2.Distance(___agent.curPosition, ___agent.startingPosition) > 0.4f)
							___agent.agentActive = true;
					}

					break;
				case goalType.WanderInOwnedProperty:
					if (goalType != ___brain.Goals[0].goalCode)
					{
						__instance.CancelSearchStaticStateIndicator();
						GoalWanderInOwnedProperty newGoal7 = new GoalWanderInOwnedProperty();
						__instance.SwitchGoal(newGoal7);
						___agent.agentActive = false;
					}

					break;
				case goalType.WanderFar:
					if (goalType != ___brain.Goals[0].goalCode)
					{
						__instance.CancelSearchStaticStateIndicator();

						if (___agent.gangLeader)
							for (int num52 = 0; num52 < ___agent.gangMembers.Count; num52++)
							{
								Agent agent15 = ___agent.gangMembers[num52];

								if (agent15.gang == ___agent.gang && agent15.agentID != ___agent.agentID && !agent15.hasEmployer)
									agent15.SetFollowing(null);
							}
						
						GoalWanderFar newGoal8 = new GoalWanderFar();
						__instance.SwitchGoal(newGoal8);

						if (___agent.killForQuest == null && ___agent.rescueForQuest == null && (!(___agent.agentName == "Assassin") || ___agent.gang <= 0) && !___agent.alwaysActiveWanderFar)
							___agent.agentActive = false;
						else
							___agent.agentActive = true;
					}

					if (___agent.gang != 0)
					{
						List<Agent> gangMembers3 = ___agent.gangMembers;

						for (int num53 = 0; num53 < gangMembers3.Count; num53++)
						{
							Agent agent16 = gangMembers3[num53];

							if (agent16.gang == ___agent.gang && !agent16.objectMultAgent._dead && agent16 != ___agent && agent16.mostRecentGoalCode == goalType.NoiseReact && !agent16.investigatingCrime)
								___agent.dontWalkGang = true;
						}
					}

					break;
				case goalType.Dance:
					if (goalType != ___brain.Goals[0].goalCode)
					{
						__instance.CancelSearchStaticStateIndicator();
						GoalDance newGoal9 = new GoalDance();
						__instance.SwitchGoal(newGoal9);
						___agent.agentActive = false;

						if (Vector2.Distance(___agent.curPosition, ___agent.startingPosition) > 0.4f)
							___agent.agentActive = true;
					}

					break;
				case goalType.Joke:
					if (goalType != ___brain.Goals[0].goalCode)
					{
						__instance.CancelSearchStaticStateIndicator();
						GoalJoke newGoal10 = new GoalJoke();
						__instance.SwitchGoal(newGoal10);
						___agent.agentActive = false;

						if (Vector2.Distance(___agent.curPosition, ___agent.startingPosition) > 0.4f)
							___agent.agentActive = true;
					}

					break;
				case goalType.ListenToJokeNPC:
					if (goalType != ___brain.Goals[0].goalCode)
					{
						__instance.CancelSearchStaticStateIndicator();
						GoalListenToJokeNPC newGoal11 = new GoalListenToJokeNPC();
						__instance.SwitchGoal(newGoal11);
						___agent.agentActive = false;

						if (Vector2.Distance(___agent.curPosition, ___agent.startingPosition) > 0.4f)
							___agent.agentActive = true;
					}

					break;
				case goalType.Sit:
					if (goalType != ___brain.Goals[0].goalCode)
					{
						__instance.CancelSearchStaticStateIndicator();
						GoalSit newGoal12 = new GoalSit();
						__instance.SwitchGoal(newGoal12);
						___agent.agentActive = false;

						if (Vector2.Distance(___agent.curPosition, ___agent.originalLocation) > 0.4f)
							___agent.agentActive = true;
					}

					break;
				case goalType.Steal:
					if (goalType != ___brain.Goals[0].goalCode)
					{
						__instance.CancelSearchStaticStateIndicator();
						__instance.SwitchGoal(new GoalSteal
						{
							stealingFromAgent = ___tempGoalSteal.stealingFromAgent
						});
						___agent.agentActive = true;
					}

					break;
				case goalType.IceSkate:
					if (goalType != ___brain.Goals[0].goalCode)
					{
						__instance.CancelSearchStaticStateIndicator();
						GoalIceSkate newGoal13 = new GoalIceSkate();
						__instance.SwitchGoal(newGoal13);
						___agent.agentActive = false;
					}

					break;
				case goalType.Swim:
					if (goalType != ___brain.Goals[0].goalCode)
					{
						__instance.CancelSearchStaticStateIndicator();
						GoalSwim newGoal14 = new GoalSwim();
						__instance.SwitchGoal(newGoal14);
						___agent.agentActive = false;
					}

					break;
				case goalType.FindFire:
					if (goalType != ___brain.Goals[0].goalCode)
					{
						__instance.CancelSearchStaticStateIndicator();
						__instance.SwitchGoal(new GoalFindFire
						{
							fire = ___tempGoalFindFire.fire
						});
						___agent.agentActive = true;
					}
					else if (((GoalFindFire)___brain.Goals[0]).fire != ___tempGoalFindFire.fire)
						__instance.SwitchGoal(new GoalFindFire
						{
							fire = ___tempGoalFindFire.fire
						});

					break;
				case goalType.RobotClean:
					if (goalType != ___brain.Goals[0].goalCode)
					{
						__instance.CancelSearchStaticStateIndicator();
						__instance.SwitchGoal(new GoalRobotClean
						{
							cleaningObject = ___tempGoalRobotClean.cleaningObject
						});
						___agent.agentActive = true;
					}
					else
					{
						if (((GoalRobotClean)___brain.Goals[0]).cleaningObject != ___tempGoalRobotClean.cleaningObject)
							__instance.SwitchGoal(new GoalRobotClean
							{
								cleaningObject = ___tempGoalRobotClean.cleaningObject
							});

						___agent.agentActive = true;
					}

					break;
				case goalType.Bite:
					if (goalType != ___brain.Goals[0].goalCode)
					{
						__instance.CancelSearchStaticStateIndicator();
						__instance.SwitchGoal(new GoalBite
						{
							bitingTarget = ___tempGoalBite.bitingTarget
						});
						___agent.agentActive = true;
					}

					break;
				case goalType.Cannibalize:
					if (goalType != ___brain.Goals[0].goalCode)
					{
						__instance.CancelSearchStaticStateIndicator();
						__instance.SwitchGoal(new GoalCannibalize
						{
							cannibalizingTarget = ___tempGoalCannibalize.cannibalizingTarget
						});
						___agent.agentActive = true;
					}

					break;
				case goalType.GoGet:
					if (goalType != ___brain.Goals[0].goalCode)
					{
						__instance.CancelSearchStaticStateIndicator();
						__instance.SwitchGoal(new GoalGoGet
						{
							item = ___tempGoalGoGet.item
						});
						___agent.agentActive = true;
					}

					break;
				case goalType.GetElectionResults:
					if (goalType != ___brain.Goals[0].goalCode)
					{
						__instance.CancelSearchStaticStateIndicator();
						GoalGetElectionResults newGoal15 = new GoalGetElectionResults();
						__instance.SwitchGoal(newGoal15);
						___agent.agentActive = true;
					}

					if (___agent.gang != 0)
					{
						List<Agent> gangMembers4 = ___agent.gangMembers;

						for (int num54 = 0; num54 < gangMembers4.Count; num54++)
						{
							Agent agent17 = gangMembers4[num54];

							if (agent17.gang == ___agent.gang && !agent17.objectMultAgent._dead && agent17 != ___agent && agent17.mostRecentGoalCode == goalType.NoiseReact && !agent17.investigatingCrime)
								___agent.dontWalkGang = true;
						}
					}

					break;
				case goalType.CommitArson:
					if (goalType != ___brain.Goals[0].goalCode)
					{
						__instance.CancelSearchStaticStateIndicator();
						__instance.SwitchGoal(new GoalCommitArson
						{
							arsonObject = ___tempGoalCommitArson.arsonObject
						});
						___agent.agentActive = true;
					}
					else if (((GoalCommitArson)___brain.Goals[0]).arsonObject != ___tempGoalCommitArson.arsonObject)
						__instance.SwitchGoal(new GoalCommitArson
						{
							arsonObject = ___tempGoalCommitArson.arsonObject
						});

					break;
			}

			___agent.justHitByAgent = GC.dummyAgent;
			___agent.hasJustHitByAgent = false;

			return true;
		}
		#endregion
		#region LoadLevel
		public static void LoadLevel_SetupMore4(LoadLevel __instance) // Prefix
		{
			BMLog("LoadLevel.SetupMore4");

			List<int> gangsAssigned = new List<int>();

			foreach (Agent agent in GC.agentList)
			{
				BMLog("Detected " + agent.agentName.PadLeft(12) + " #" + GC.agentList.IndexOf(agent).ToString().PadRight(2) + ", member of gang #" + agent.gang + ", which has " + agent.gangMembers.Count + " members. He is/not a leader: " + agent.gangLeader);

				if ((agent.agentName == "Gangbanger" || agent.agentName == "GangbangerB") && agent.gang != 0 && agent.gangMembers.Count > 1 && !gangsAssigned.Contains(agent.gang))
				{
					agent.gangLeader = true;
					gangsAssigned.Add(agent.gang);

					BMLog("Added Leader to Gang " + agent.gang + ": " + agent.agentName.PadLeft(12) + " #" + GC.agentList.IndexOf(agent).ToString().PadRight(2));
				}
				else if (agent.agentName == "Hobo")
				{
					if (GC.percentChance(33))
					{
						Agent.gangCount++;
						agent.gang = Agent.gangCount;
						agent.gangMembers.Add(agent);
						agent.gangLeader = true;
						gangsAssigned.Add(agent.gang);

						BMLog("Added Hobo to Gang " + agent.gang + ": " + agent.agentName.PadLeft(12) + " #" + GC.agentList.IndexOf(agent).ToString().PadRight(2));
					}
				}
			}
		}
		#endregion
		#region PlayfieldObject
		public static void PlayfieldObject_determineMoneyCost(int moneyAmt, string transactionType, PlayfieldObject __instance, ref int __result) // Postfix // Uncapitalized in source
		{                               // ↑ [sic]
			BMLog("PlayfieldObject_determineMoneyCost: transactionType = " + transactionType +"; PFO = " + __instance.name);

			Agent agent = (Agent)__instance;
			float num = __result;
			int levelMultiplier = Mathf.Clamp(GC.sessionDataBig.curLevelEndless, 1, 15);
			int gangsizeMultiplier = agent.gangMembers.Count;

			BMLog("PlayfieldObject_DetermineMoneyCost: num = " + num + "; LevelMult = " + levelMultiplier + "; gangsizeMult = " + gangsizeMultiplier);

			if (transactionType == "Mug_Gangbanger")
				num = (float)(levelMultiplier * 10 + gangsizeMultiplier * 15);
			else if (transactionType == "Hobo_GiveMoney1")
				num = 05f;
			else if (transactionType == "Hobo_GiveMoney2")
				num = 20f;
			else if (transactionType == "Hobo_GiveMoney3")
				num = 50f;
			else
				BMLog("Bad string passed to PlayfieldObject_determineMoneyCost");

			__result = (int)num;

			BMLog("PlayfieldObject_determineMoneyCost: result = " + __result);
		}
		#endregion
	}
}
