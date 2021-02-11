using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using BepInEx;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;

namespace BunnyMod
{
	public class BunnyBehaviors
	{
		#region Generic
		public void Awake()
		{
			Initialize_Names();

			BunnyHeader.MainInstance.PatchPrefix(typeof(AgentInteractions), "DetermineButtons", GetType(), "AgentInteractions_DetermineButtons", new Type[5] { typeof(Agent), typeof(Agent), typeof(List<string>), typeof(List<string>), typeof(List<string>) });

			BunnyHeader.MainInstance.PatchPrefix(typeof(LoadLevel), "SetupMore4", GetType(), "LoadLevel_SetupMore4", new Type[0] { });

			BunnyHeader.MainInstance.PatchPostfix(typeof(PlayfieldObject), "determineMoneyCost", GetType(), "PlayfieldObject_determineMoneyCost", new Type[2] { typeof(int), typeof(string) }); // Uncapitalized in source
		}
		public void FixedUpdate()
		{
		}
		public void Initialize_Names()
		{
			CustomName hobo_Mug = RogueLibs.CreateCustomName("Hobo_Mug", "Dialogue", new CustomNameInfo("Can you spare a few bucks, friendo?"));
			CustomName hobo_GiveMoney1 = RogueLibs.CreateCustomName("Hobo_GiveMoney1", "Button", new CustomNameInfo("Give Money"));
			CustomName hobo_GiveMoney2 = RogueLibs.CreateCustomName("Hobo_GiveMoney2", "Button", new CustomNameInfo("Give Money"));
			CustomName hobo_GiveMoney3 = RogueLibs.CreateCustomName("Hobo_GiveMoney3", "Button", new CustomNameInfo("Give Money"));
			CustomName hobo_GiveItem = RogueLibs.CreateCustomName("Hobo_GiveItem", "Button", new CustomNameInfo("Give Item"));
			CustomName gangbanger_Mug = RogueLibs.CreateCustomName("Gangbanger_Mug", "Dialogue", new CustomNameInfo("The cash, hand it over!"));
			CustomName gangbangerB_Mug = RogueLibs.CreateCustomName("GangbangerB_Mug", "Dialogue", new CustomNameInfo("Empty your pockets, bitch!"));
			CustomName gangbanger_GiveMoney = RogueLibs.CreateCustomName("Gangbanger_GiveMoney", "Button", new CustomNameInfo("Give Money"));
		}
		#endregion

		#region AgentInteractions
		public static bool AgentInteractions_DetermineButtons(Agent agent, Agent interactingAgent, List<string> buttons1, List<string> buttonsExtra1, List<int> buttonPrices1, AgentInteractions __instance) // Prefix
		{
			if (agent.agentName == "Hobo")
			{
				agent.gc.audioHandler.Play(agent, "AgentTalk");

				if (agent.gang == interactingAgent.gangMugging && agent.gang != 0)
				{
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
				agent.gc.audioHandler.Play(agent, "AgentTalk");

				if (agent.gang == interactingAgent.gangMugging && agent.gang != 0)
					__instance.AddButton("Gangbanger_GiveMoney", agent.determineMoneyCost("Mug_Gangbanger"));
				else
					agent.SayDialogue("Interact");
			}
			return true;
		}
		public static void AgentInteractions_PressedButton(Agent agent, Agent interactingAgent, string buttonText, int buttonPrice, AgentInteractions __instance)
		{

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
					// Do we want to limit this to roaming or stationary? Or just have all of them do it? Only testing will tell.

					Agent.gangCount++;
					agent.gang = Agent.gangCount;
					agent.gangLeader = true;
					gangsAssigned.Add(agent.gang);

					BunnyHeader.Log("Added Hobo to Gang " + agent.gang + ": " + agent.agentName.PadLeft(12) + " #" + ___gc.agentList.IndexOf(agent).ToString().PadRight(2));
				}

			}
		}
		#endregion
		#region PlayfieldObject
		public static int PlayfieldObject_determineMoneyCost(int moneyAmt, string transactionType, PlayfieldObject __instance, ref int __result) // Postfix // Uncapitalized in source
		{                              // ↑ [sic]
			Agent agent = (Agent)__instance;
			float num = __result;
			int levelMultiplier = Mathf.Clamp(__instance.gc.sessionDataBig.curLevelEndless, 1, 15);
			int gangsizeMultiplier = agent.gangMembers.Count;

			if (transactionType == "Mug_Gangbanger")
			{
				num = (float)(50 + levelMultiplier * 15);
			}
			else if (transactionType == "Mug_Hobo")
			{
				num = (float)(50 + levelMultiplier * 5);
			}

			if (agent.isPlayer == 0)
			{
				string rel = agent.relationships.GetRel(__instance.interactingAgent);

				if (rel == "Friendly")
					num *= 0.9f;
				else if (rel == "Loyal")
					num *= 0.8f;
				else if (rel == "Aligned")
					num *= 0.7f;
				else if (rel == "Submissive")
					num *= 0.6f;
			}

			if (__instance.gc.challenges.Contains("HighCost"))
				num *= 1.4f;

			if (__instance.gc.challenges.Contains("QuickGame"))
				num *= 0.8f;

			return __result;
		}
		#endregion
	}
}
