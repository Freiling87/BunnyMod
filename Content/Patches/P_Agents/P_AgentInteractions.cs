using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using BepInEx.Logging;
using BunnyMod.Content.Logging;
using BunnyMod.Content.Traits;
using HarmonyLib;

namespace BunnyMod.Content.Patches
{
	[HarmonyPatch(declaringType: typeof(AgentInteractions))]
	public static class P_AgentInteractions
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();

		/*[HarmonyPrefix,
		 HarmonyPatch(methodName: nameof(AgentInteractions.DetermineButtons),
				 argumentTypes: new[] { typeof(Agent), typeof(Agent), typeof(List<string>), typeof(List<string>), typeof(List<int>) })]
		private static void DetermineButtons_Prefix(Agent agent, Agent interactingAgent,
				List<string> buttons1, List<string> buttonsExtra1, List<int> buttonPrices1,
				AgentInteractions __instance)
		{
			logger.LogDebug(
					$"DetermineButtons(agent = {agent.agentName}{agent.agentID}, agent.gang = {agent.gang}, interactingAgent.gangMugging = {interactingAgent.gangMugging})");

			bool isHobo = agent.agentName == nameof(AgentNameDB.rowIds.Hobo);
			bool isGangbanger = agent.agentName == nameof(AgentNameDB.rowIds.Gangbanger) || agent.agentName == nameof(AgentNameDB.rowIds.GangbangerB);
			bool isJunkie = agent.agentName == cAgent.Junkie;

			if (isHobo || isGangbanger || isJunkie)
			{
				GameController.gameController.audioHandler.Play(agent, "AgentTalk");
				if (agent.gang == interactingAgent.gangMugging && agent.gang != 0)
				{
					logger.LogDebug($"DetermineButtons: Adding Buttons");

					if (isHobo)
					{
						__instance.AddButton("Hobo_GiveMoney1", agent.determineMoneyCost("Hobo_GiveMoney1"));
						__instance.AddButton("Hobo_GiveMoney2", agent.determineMoneyCost("Hobo_GiveMoney2"));
						__instance.AddButton("Hobo_GiveMoney3", agent.determineMoneyCost("Hobo_GiveMoney3"));
						__instance.AddButton("Hobo_GiveItem", "(Choose)");						
					}
					else if (isGangbanger)
					{
						__instance.AddButton("Gangbanger_GiveMoney", agent.determineMoneyCost("Mug_Gangbanger"));
					}
					else {
						__instance.AddButton("Junkie_GiveMoney", agent.determineMoneyCost("Mug_Junkie"));
						__instance.AddButton("Junkie_GiveItem", agent.determineMoneyCost("Junkie_GiveItem"));
					}
				}
				else
				{
					BMHeaderTools.SayDialogue(agent, "Interact", vNameType.Dialogue);
				}
			}
		}*/

		/*[HarmonyPrefix,
		 HarmonyPatch(methodName: nameof(AgentInteractions.PressedButton), argumentTypes: new[] { typeof(Agent), typeof(Agent), typeof(string), typeof(int) })]
		private static bool PressedButton_Prefix(Agent agent, Agent interactingAgent, string buttonText, int buttonPrice, AgentInteractions __instance)
		{
			logger.LogDebug($"PressedButton(agent = {agent.agentName}, buttonText = {buttonText})");

			switch (agent.agentName)
			{
				case nameof(AgentNameDB.rowIds.Hobo):
					switch (buttonText)
					{
						case "Hobo_GiveMoney1":
						case "Hobo_GiveMoney2":
						case "Hobo_GiveMoney3":
						{
							if (agent.moneySuccess(buttonPrice))
							{
								BMBehaviors.Hobo_MugMoney(agent, interactingAgent, buttonPrice,
										BMBehaviors.Hobo_relStatusAfterDonation(agent, interactingAgent, buttonPrice).ToString("f"), buttonText);
							}
							else
							{
								BMHeaderTools.SayDialogue(agent, "Hobo_CantAfford", vNameType.Dialogue);
							}
							agent.StopInteraction();
							return false; // Double-check that this isn't skipping anything important
						}
						case "Hobo_GiveItem":
							agent.ShowUseOn("Hobo_Donate");
							return false; // Double-check that this isn't skipping anything important
					}
					break;
				case nameof(AgentNameDB.rowIds.Gangbanger):
				case nameof(AgentNameDB.rowIds.GangbangerB):
					if (buttonText == "GangBanger_GiveMoney")
					{
						if (agent.moneySuccess(buttonPrice))
						{
							__instance.MugMoney(agent, interactingAgent);
						}
						else
						{
							BMHeaderTools.SayDialogue(agent, "Gangbanger_CantAfford", vNameType.Dialogue);
						}
						agent.StopInteraction();

						return false; // Double-check that this isn't skipping anything important
					}
					break;
			}
			return true;
		}*/

		/*[HarmonyPostfix,
		 HarmonyPatch(methodName: nameof(AgentInteractions.UseItemOnObject),
				 argumentTypes: new[] { typeof(Agent), typeof(Agent), typeof(InvItem), typeof(int), typeof(string), typeof(string) })]
		private static void UseItemOnObject_Postfix(Agent agent, Agent interactingAgent, InvItem item, int slotNum, string combineType, string useOnType,
				ref bool __result)
		{
			logger.LogDebug($"UseItemOnObject(item = {item.invItemName})");

			if (useOnType == "Hobo_Donate")
			{
				string itemName = item.invItemName;
				if (itemName == nameof(ItemNameDB.rowIds.Banana) || itemName == nameof(ItemNameDB.rowIds.BananaPeel)
						|| itemName == nameof(ItemNameDB.rowIds.Beer) || itemName == nameof(ItemNameDB.rowIds.Cigarettes)
						|| itemName == nameof(ItemNameDB.rowIds.Fud) || itemName == "Sugar" // ? sugar exists ?
						|| itemName == nameof(ItemNameDB.rowIds.Whiskey))
				{
					BMBehaviors.Hobo_AcceptDonation(agent, interactingAgent, item);
				}
				else
				{
					BMHeaderTools.SayDialogue(agent, "Hobo_DontWant", vNameType.Dialogue);
					GameController.gameController.audioHandler.Play(interactingAgent, "CantDo");
				}

				__result = true;
			}
		}*/

		[HarmonyPrefix, HarmonyPatch(methodName: nameof(AgentInteractions.AddButton), argumentTypes: new[] { typeof(string), typeof(int), typeof(string) })]
		private static void AddButton_Prefix(string buttonName, int moneyCost, ref string extraCost, Agent ___mostRecentInteractingAgent)
		{
			TamperTantrum.AgentInteractions_AddButton_Prefix(buttonName, ref extraCost, ___mostRecentInteractingAgent);
			TamperTantrum2.AgentInteractions_AddButton_Prefix(buttonName, ref extraCost, ___mostRecentInteractingAgent);
		}

		[HarmonyTranspiler, HarmonyPatch(methodName: nameof(AgentInteractions.Shakedown), argumentTypes: new[] { typeof(Agent), typeof(Agent) })]
		private static IEnumerable<CodeInstruction> Shakedown_Transpiler(IEnumerable<CodeInstruction> instructionsEnumerable, ILGenerator generator)
		{
			List<CodeInstruction> instructions = instructionsEnumerable.ToList();
			VeiledThreats.ThreatenFailureHook(generator).ApplySafe(instructions, logger);
			return instructions;
		}

		[HarmonyTranspiler, HarmonyPatch(methodName: nameof(AgentInteractions.Threaten), argumentTypes: new[] { typeof(Agent), typeof(Agent) })]
		private static IEnumerable<CodeInstruction> Threaten_Transpiler(IEnumerable<CodeInstruction> instructionsEnumerable, ILGenerator generator)
		{
			List<CodeInstruction> instructions = instructionsEnumerable.ToList();
			VeiledThreats.ThreatenAskFailureHook(generator).ApplySafe(instructions, logger);
			return instructions;
		}

		[HarmonyTranspiler, HarmonyPatch(methodName: nameof(AgentInteractions.ThreatenKey), argumentTypes: new[] { typeof(Agent), typeof(Agent) })]
		private static IEnumerable<CodeInstruction> ThreatenKey_Transpiler(IEnumerable<CodeInstruction> instructionsEnumerable, ILGenerator generator)
		{
			List<CodeInstruction> instructions = instructionsEnumerable.ToList();
			VeiledThreats.ThreatenFailureHook(generator).ApplySafe(instructions, logger);
			return instructions;
		}

		[HarmonyTranspiler, HarmonyPatch(methodName: nameof(AgentInteractions.ThreatenKeyAndSafeCombination), argumentTypes: new[] { typeof(Agent), typeof(Agent) })]
		private static IEnumerable<CodeInstruction> ThreatenKeyAndSafeCombination_Transpiler(IEnumerable<CodeInstruction> instructionsEnumerable,
				ILGenerator generator)
		{
			List<CodeInstruction> instructions = instructionsEnumerable.ToList();
			VeiledThreats.ThreatenFailureHook(generator).ApplySafe(instructions, logger);
			return instructions;
		}

		[HarmonyTranspiler, HarmonyPatch(methodName: nameof(AgentInteractions.ThreatenLeaveTown), argumentTypes: new[] { typeof(Agent), typeof(Agent) })]
		private static IEnumerable<CodeInstruction> ThreatenLeaveTown_Transpiler(IEnumerable<CodeInstruction> instructionsEnumerable, ILGenerator generator)
		{
			List<CodeInstruction> instructions = instructionsEnumerable.ToList();
			VeiledThreats.ThreatenAskFailureHook(generator).ApplySafe(instructions, logger);
			return instructions;
		}

		[HarmonyTranspiler, HarmonyPatch(methodName: nameof(AgentInteractions.ThreatenMayor), argumentTypes: new[] { typeof(Agent), typeof(Agent) })]
		private static IEnumerable<CodeInstruction> ThreatenMayor_Transpiler(IEnumerable<CodeInstruction> instructionsEnumerable, ILGenerator generator)
		{
			List<CodeInstruction> instructions = instructionsEnumerable.ToList();
			VeiledThreats.ThreatenFailureHook(generator).ApplySafe(instructions, logger);
			return instructions;
		}

		[HarmonyTranspiler, HarmonyPatch(methodName: nameof(AgentInteractions.ThreatenMayorBadge), argumentTypes: new[] { typeof(Agent), typeof(Agent) })]
		private static IEnumerable<CodeInstruction> ThreatenMayorBadge_Transpiler(IEnumerable<CodeInstruction> instructionsEnumerable, ILGenerator generator)
		{
			List<CodeInstruction> instructions = instructionsEnumerable.ToList();
			VeiledThreats.ThreatenFailureHook(generator).ApplySafe(instructions, logger);
			return instructions;
		}

		[HarmonyTranspiler, HarmonyPatch(methodName: nameof(AgentInteractions.ThreatenMoney), argumentTypes: new[] { typeof(Agent), typeof(Agent) })]
		private static IEnumerable<CodeInstruction> ThreatenMoney_Transpiler(IEnumerable<CodeInstruction> instructionsEnumerable, ILGenerator generator)
		{
			List<CodeInstruction> instructions = instructionsEnumerable.ToList();
			VeiledThreats.ThreatenFailureHook(generator).ApplySafe(instructions, logger);
			return instructions;
		}

		[HarmonyTranspiler, HarmonyPatch(methodName: nameof(AgentInteractions.ThreatenSafeCombination), argumentTypes: new[] { typeof(Agent), typeof(Agent) })]
		private static IEnumerable<CodeInstruction> ThreatenSafeCombination_Transpiler(IEnumerable<CodeInstruction> instructionsEnumerable,
				ILGenerator generator)
		{
			List<CodeInstruction> instructions = instructionsEnumerable.ToList();
			VeiledThreats.ThreatenFailureHook(generator).ApplySafe(instructions, logger);
			return instructions;
		}
	}
}