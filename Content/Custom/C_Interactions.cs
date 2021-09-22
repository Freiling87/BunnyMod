using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RogueLibsCore;
using System.Collections.Generic;
using BepInEx.Logging;
using BunnyMod.Content.Logging;
using UnityEngine;
using Random = UnityEngine.Random;
using BunnyMod.Content.Traits;

namespace BunnyMod.Content.Custom
{
	public static class C_Interactions
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		private static GameController GC => GameController.gameController;

		public static void Hobo_AcceptDonation(Agent hobo, Agent interactingAgent, int moneyValue)
		{
			logger.LogDebug("Hobo_AcceptDonation: " + hobo.agentID + " receiving $" + moneyValue);

			// TODO: Write Hobo_AcceptDonation(Money)
		}

		public static void Hobo_AcceptDonation(Agent hobo, Agent interactingAgent, InvItem invItem)
		{
			logger.LogDebug("Hobo_AcceptDonation: " + hobo.agentID + " receiving " + invItem.invItemName);

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
				logger.LogDebug("Unacceptable item donated to " + hobo.agentName + hobo.agentID);
				moneyValue = invItem.itemValue;
			}

			string newRelationship = Hobo_relStatusAfterDonation(hobo, interactingAgent, moneyValue).ToString("f");

			logger.LogDebug("Hobo_AcceptDonation: item = " + item + ";  moneyValue = " + moneyValue + "; newRelationship = " + newRelationship);

			Hobo_MugItem(hobo, interactingAgent, item, newRelationship);
		}

		public static void Hobo_MugItem(Agent agent, Agent interactingAgent, string itemName, string relStatus)
		{
			logger.LogDebug("Hobo_MugItem");

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
			logger.LogDebug("Hobo_MugMoney");

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
			logger.LogDebug("Hobo_relStatusAfterDonation: moneyValue = " + moneyValue);

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
	}
}
