using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;
using Random = UnityEngine.Random;
using BepInEx.Logging;

namespace BunnyMod.Content
{
	public class BMBehaviors
	{
		private static readonly string loggerName = $"BunnyMod_{MethodBase.GetCurrentMethod().DeclaringType?.Name}";
		private static ManualLogSource Logger => _logger ?? (_logger = BepInEx.Logging.Logger.CreateLogSource(loggerName));
		private static ManualLogSource _logger;

		private static GameController GC => GameController.gameController;

		[RLSetup]
		public void Initialize_Names()
		{
			#region ButtonText
			string nameType = "Interface";
			_ = RogueLibs.CreateCustomName("Hobo_GiveMoney1", nameType, new CustomNameInfo("Give Money"));
			_ = RogueLibs.CreateCustomName("Hobo_GiveMoney2", nameType, new CustomNameInfo("Give Money"));
			_ = RogueLibs.CreateCustomName("Hobo_GiveMoney3", nameType, new CustomNameInfo("Give Money"));
			_ = RogueLibs.CreateCustomName("Hobo_GiveItem", nameType, new CustomNameInfo("Give Item"));
			_ = RogueLibs.CreateCustomName("Gangbanger_GiveMoney", nameType, new CustomNameInfo("Give Money"));
			#endregion
			#region Dialogue
			nameType = "Dialogue";
			_ = RogueLibs.CreateCustomName("Hobo_CantAfford", nameType, new CustomNameInfo("Come on, don't hold out on me!"));
			_ = RogueLibs.CreateCustomName("Hobo_Donate_Aligned", nameType, new CustomNameInfo("I knew it, you're Jesus! Hey Jesus, get this CIA microchip outta my head! Wabadoobaflooba! Yaba! Haba!"));
			_ = RogueLibs.CreateCustomName("Hobo_Donate_Annoyed", nameType, new CustomNameInfo("...Okay."));
			_ = RogueLibs.CreateCustomName("Hobo_Donate_Friendly", nameType, new CustomNameInfo("Hey, thanks buddy!"));
			_ = RogueLibs.CreateCustomName("Hobo_Donate_Hostile", nameType, new CustomNameInfo("Fuck you, asshole!"));
			_ = RogueLibs.CreateCustomName("Hobo_Donate_Loyal", nameType, new CustomNameInfo("Oh, hell yeah!"));
			_ = RogueLibs.CreateCustomName("Hobo_Donate_Neutral", nameType, new CustomNameInfo("Oh, uh... thanks, I guess?"));
			_ = RogueLibs.CreateCustomName("Hobo_DontWant", nameType, new CustomNameInfo("Nah, what else you got?"));
			_ = RogueLibs.CreateCustomName("Hobo_Mug", nameType, new CustomNameInfo("Can you spare a few bucks, friendo?"));
			_ = RogueLibs.CreateCustomName("Gangbanger_CantAfford", nameType, new CustomNameInfo("Sucks to be you!"));
			_ = RogueLibs.CreateCustomName("Gangbanger_Mug", nameType, new CustomNameInfo("The cash, hand it over!"));
			_ = RogueLibs.CreateCustomName("GangbangerB_Mug", nameType, new CustomNameInfo("Empty your pockets, bitch!"));
			#endregion
		}

		#region Custom
		public static void Hobo_AcceptDonation(Agent hobo, Agent interactingAgent, int moneyValue)
		{
			Logger.LogDebug("Hobo_AcceptDonation: " + hobo.agentID + " receiving $" + moneyValue);

			// TODO: Write Hobo_AcceptDonation(Money)
		}

		public static void Hobo_AcceptDonation(Agent hobo, Agent interactingAgent, InvItem invItem)
		{
			Logger.LogDebug("Hobo_AcceptDonation: " + hobo.agentID + " receiving " + invItem.invItemName);

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
				Logger.LogDebug("Unacceptable item donated to " + hobo.agentName + hobo.agentID);
				moneyValue = invItem.itemValue;
			}

			string newRelationship = Hobo_relStatusAfterDonation(hobo, interactingAgent, moneyValue).ToString("f");

			Logger.LogDebug("Hobo_AcceptDonation: item = " + item + ";  moneyValue = " + moneyValue + "; newRelationship = " + newRelationship);

			Hobo_MugItem(hobo, interactingAgent, item, newRelationship);
		}

		public static void Hobo_MugItem(Agent agent, Agent interactingAgent, string itemName, string relStatus)
		{
			Logger.LogDebug("Hobo_MugItem");

			if (GC.serverPlayer)
			{
				for (int i = 0; i < agent.gangMembers.Count; i++)
					agent.gangMembers[i].hasMugged = true;

				for (int j = 0; j < GC.playerAgentList.Count; j++)
					GC.playerAgentList[j].gangMugging = 0;

				agent.objectMult.SetGangMuggingOff();
				agent.doingMugging = -1;
				BMHeaderTools.SayDialogue(agent, "Bought", vNameType.Dialogue); // ←
				GC.audioHandler.Play(interactingAgent, "SelectItem");

				agent.relationships.SetRel(interactingAgent, relStatus);

				return;
			}
			interactingAgent.objectMult.ObjectAction(agent.objectNetID, "MugItem");
		}

		public static void Hobo_MugMoney(Agent agent, Agent interactingAgent, int moneyValue, string relStatus, string transactionType)
		{
			Logger.LogDebug("Hobo_MugMoney");

			if (GC.serverPlayer)
			{
				for (int i = 0; i < agent.gangMembers.Count; i++)
					agent.gangMembers[i].hasMugged = true;

				for (int j = 0; j < GC.playerAgentList.Count; j++)
					GC.playerAgentList[j].gangMugging = 0;

				agent.objectMult.SetGangMuggingOff();
				agent.doingMugging = -1;

				BMHeaderTools.SayDialogue(agent, "Hobo_Donate_" + relStatus, vNameType.Dialogue); //
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
			Logger.LogDebug("Hobo_relStatusAfterDonation: moneyValue = " + moneyValue);

			int[] reactionPercentages = new int[6] { 0, 0, 0, 0, 0, 0 };
			List<relStatus> reactionOutcomes = new List<relStatus>
				{ relStatus.Hostile, relStatus.Annoyed, relStatus.Neutral, relStatus.Friendly, relStatus.Loyal, relStatus.Aligned };

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
	}
}