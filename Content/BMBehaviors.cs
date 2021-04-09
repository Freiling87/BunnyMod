using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using BepInEx;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;

namespace BunnyMod.Content
{
	public class BMBehaviors
	{
		public static GameController gc => GameController.gameController;

		#region Generic
		public void Awake()
		{
			Initialize_Names();

			BunnyHeader.MainInstance.PatchPrefix(typeof(AgentInteractions), "AddButton", GetType(), "AgentInteractions_AddButton_4", new Type[3] { typeof(string), typeof(int), typeof(string) });
			BunnyHeader.MainInstance.PatchPrefix(typeof(AgentInteractions), "DetermineButtons", GetType(), "AgentInteractions_DetermineButtons", new Type[5] { typeof(Agent), typeof(Agent), typeof(List<string>), typeof(List<string>), typeof(List<int>) });
			BunnyHeader.MainInstance.PatchPrefix(typeof(AgentInteractions), "PressedButton", GetType(), "AgentInteractions_PressedButton", new Type[4] { typeof(Agent), typeof(Agent), typeof(string), typeof(int) });
			BunnyHeader.MainInstance.PatchPostfix(typeof(AgentInteractions), "UseItemOnObject", GetType(), "AgentInteractions_UseItemOnObject", new Type[6] { typeof(Agent), typeof(Agent), typeof(InvItem), typeof(int), typeof(string), typeof(string) });

			BunnyHeader.MainInstance.PatchPrefix(typeof(LoadLevel), "SetupMore4", GetType(), "LoadLevel_SetupMore4", new Type[0] { });

			BunnyHeader.MainInstance.PatchPostfix(typeof(PlayfieldObject), "determineMoneyCost", GetType(), "PlayfieldObject_determineMoneyCost", new Type[2] { typeof(int), typeof(string) }); // Uncapitalized in source
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
		#endregion

		#region Custom
		public static void Hobo_AcceptDonation(Agent hobo, Agent interactingAgent, int moneyValue)
		{
			BunnyHeader.Log("Hobo_AcceptDonation: " + hobo.agentID + " receiving $" + moneyValue);

			// TODO: Write Hobo_AcceptDonation(Money)
		}
		public static void Hobo_AcceptDonation(Agent hobo, Agent interactingAgent, InvItem invItem)
		{
			BunnyHeader.Log("Hobo_AcceptDonation: " + hobo.agentID + " receiving " + invItem.invItemName);

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
				BunnyHeader.Log("Unacceptable item donated to " + hobo.agentName + hobo.agentID);
				moneyValue = invItem.itemValue;
			}

			string newRelationship = Hobo_relStatusAfterDonation(hobo, interactingAgent, moneyValue).ToString("f");

			BunnyHeader.Log("Hobo_AcceptDonation: item = " + item + ";  moneyValue = " + moneyValue + "; newRelationship = " + newRelationship);

			Hobo_MugItem(hobo, interactingAgent, item, newRelationship);
		}
		public static void Hobo_MugItem(Agent agent, Agent interactingAgent, string itemName, string relStatus)
		{
			BunnyHeader.Log("Hobo_MugItem");

			if (gc.serverPlayer)
			{
				for (int i = 0; i < agent.gangMembers.Count; i++)
					agent.gangMembers[i].hasMugged = true;

				for (int j = 0; j < gc.playerAgentList.Count; j++)
					gc.playerAgentList[j].gangMugging = 0;

				agent.objectMult.SetGangMuggingOff();
				agent.doingMugging = -1;
				agent.SayDialogue("Bought"); // ←
				gc.audioHandler.Play(interactingAgent, "SelectItem");

				agent.relationships.SetRel(interactingAgent, relStatus);

				return;
			}
			interactingAgent.objectMult.ObjectAction(agent.objectNetID, "MugItem");
		}
		public static void Hobo_MugMoney(Agent agent, Agent interactingAgent, int moneyValue, string relStatus, string transactionType)
		{
			BunnyHeader.Log("Hobo_MugMoney");

			if (gc.serverPlayer)
			{
				for (int i = 0; i < agent.gangMembers.Count; i++)
					agent.gangMembers[i].hasMugged = true;

				for (int j = 0; j < gc.playerAgentList.Count; j++)
					gc.playerAgentList[j].gangMugging = 0;

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
			BunnyHeader.Log("Hobo_relStatusAfterDonation: moneyValue = " + moneyValue);

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
			BunnyHeader.Log("Adding Button: buttonName = " + buttonName + "; moneyCost = " + moneyCost + "; extraCost = " + extraCost);
		}
		public static bool AgentInteractions_DetermineButtons(Agent agent, Agent interactingAgent, List<string> buttons1, List<string> buttonsExtra1, List<int> buttonPrices1, AgentInteractions __instance) // Prefix
		{
			BunnyHeader.Log("AgentInteractions_DetermineButtons: agent = " + agent.agentName + agent.agentID + "; Gang: " + agent.gang + "; GangMugging: " + interactingAgent.gangMugging);

			if (agent.agentName == "Hobo")
			{
				gc.audioHandler.Play(agent, "AgentTalk");

				if (agent.gang == interactingAgent.gangMugging && agent.gang != 0)
				{
					BunnyHeader.Log("AgentInteractions_DetermineButtons: Adding Buttons");

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
				gc.audioHandler.Play(agent, "AgentTalk");

				if (agent.gang == interactingAgent.gangMugging && agent.gang != 0)
					__instance.AddButton("Gangbanger_GiveMoney", agent.determineMoneyCost("Mug_Gangbanger"));
				else
					agent.SayDialogue("Interact");
			}
			return true;
		}
		public static bool AgentInteractions_PressedButton(Agent agent, Agent interactingAgent, string buttonText, int buttonPrice, AgentInteractions __instance) // Prefix
		{
			BunnyHeader.Log("AgentInteractions_PressedButton: " + agent.agentName + " / " + buttonText);

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
			BunnyHeader.Log("AgentInteractions_UseItemOnObject: " + item.invItemName);

			if (useOnType == "Hobo_Donate")
			{
				string itemName = item.invItemName;

				if (itemName == "Banana" || itemName == "BananaPeel" || itemName == "Beer" || itemName == "Cigarettes" || itemName == "Fud" || itemName == "Sugar" || itemName == "Whiskey" )
					Hobo_AcceptDonation(agent, interactingAgent, item);
				else
				{
					agent.SayDialogue("Hobo_DontWant");
					gc.audioHandler.Play(interactingAgent, "CantDo");
				}

				__result = true;
			}
		}
		#endregion
		#region LoadLevel
		public static void LoadLevel_SetupMore4(LoadLevel __instance, ref GameController ___gc) // Prefix
		{
			BunnyHeader.Log("LoadLevel.SetupMore4");

			List<int> gangsAssigned = new List<int>();

			foreach (Agent agent in ___gc.agentList)
			{
				BunnyHeader.Log("Detected " + agent.agentName.PadLeft(12) + " #" + ___gc.agentList.IndexOf(agent).ToString().PadRight(2) + ", member of gang #" + agent.gang + ", which has " + agent.gangMembers.Count + " members. He is/not a leader: " + agent.gangLeader);

				if ((agent.agentName == "Gangbanger" || agent.agentName == "GangbangerB") && agent.gang != 0 && agent.gangMembers.Count > 1 && !gangsAssigned.Contains(agent.gang))
				{
					agent.gangLeader = true;
					gangsAssigned.Add(agent.gang);

					BunnyHeader.Log("Added Leader to Gang " + agent.gang + ": " + agent.agentName.PadLeft(12) + " #" + ___gc.agentList.IndexOf(agent).ToString().PadRight(2));
				}
				else if (agent.agentName == "Hobo")
				{
					if (gc.percentChance(33))
					{
						Agent.gangCount++;
						agent.gang = Agent.gangCount;
						agent.gangMembers.Add(agent);
						agent.gangLeader = true;
						gangsAssigned.Add(agent.gang);

						BunnyHeader.Log("Added Hobo to Gang " + agent.gang + ": " + agent.agentName.PadLeft(12) + " #" + ___gc.agentList.IndexOf(agent).ToString().PadRight(2));
					}
				}
			}
		}
		#endregion
		#region PlayfieldObject
		public static void PlayfieldObject_determineMoneyCost(int moneyAmt, string transactionType, PlayfieldObject __instance, ref int __result) // Postfix // Uncapitalized in source
		{                               // ↑ [sic]
			BunnyHeader.Log("PlayfieldObject_determineMoneyCost: transactionType = " + transactionType +"; PFO = " + __instance.name);

			Agent agent = (Agent)__instance;
			float num = __result;
			int levelMultiplier = Mathf.Clamp(gc.sessionDataBig.curLevelEndless, 1, 15);
			int gangsizeMultiplier = agent.gangMembers.Count;

			BunnyHeader.Log("PlayfieldObject_DetermineMoneyCost: num = " + num + "; LevelMult = " + levelMultiplier + "; gangsizeMult = " + gangsizeMultiplier);

			if (transactionType == "Mug_Gangbanger")
				num = (float)(levelMultiplier * 10 + gangsizeMultiplier * 15);
			else if (transactionType == "Hobo_GiveMoney1")
				num = 05f;
			else if (transactionType == "Hobo_GiveMoney2")
				num = 20f;
			else if (transactionType == "Hobo_GiveMoney3")
				num = 50f;
			else
				BunnyHeader.Log("Bad string passed to PlayfieldObject_determineMoneyCost");

			__result = (int)num;

			BunnyHeader.Log("PlayfieldObject_determineMoneyCost: result = " + __result);
		}
		#endregion
	}
}
