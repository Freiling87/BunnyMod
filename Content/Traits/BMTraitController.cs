using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using BepInEx.Logging;
using BunnyMod.Extensions;
using BunnyMod.Logging;
using BunnyMod.Traits.T_Stealth;
using BunnyMod.Traits.T_Tampering;
using Google2u;
using RogueLibsCore;
using UnityEngine;

namespace BunnyMod.Traits
{
	/// <summary>
	/// Provides methods for interfacing with BM Custom Traits
	/// </summary>
	public static class BMTraitController
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();

		public static int ApplyToolCostModifiers(Agent agent, int baseCost)
		{
			logger.LogDebug($"{MethodBase.GetCurrentMethod().Name} ( agent = '{agent.agentName}', baseCost = '{baseCost}'");
			float costFactor = TamperTantrum.GetToolCostFactor(agent)
					* TamperTantrum2.GetToolCostFactor(agent);
			return Mathf.RoundToInt(baseCost * costFactor);
		}

		private static float GetHealthCostFactor(Agent agent, DamageType type)
		{
			switch (type)
			{
				case DamageType.burnedFingers:
					if (agent.HasTrait(StatusEffectNameDB.rowIds.ResistFire) || agent.HasTrait(StatusEffectNameDB.rowIds.FireproofSkin)
							|| agent.HasTrait(StatusEffectNameDB.rowIds.FireproofSkin2))
					{
						return 0f;
					}
					return 1f;
				case DamageType.brokenWindow:
					return StealthBastardDeluxe.GetBrokenWindowHealthCostFactor(agent)
							* (agent.HasTrait(StatusEffectNameDB.rowIds.Diminutive) ? 0.5f : 1f);
				default:
					return 1f;
			}
		}

		public static int HealthCost(Agent agent, int baseDamage, DamageType type)
		{
			logger.LogDebug($"{MethodBase.GetCurrentMethod().Name} ( agent = '{agent.agentName}', baseDamage = '{baseDamage}', damageType = '{type}'");
			float healthCostFactor = GetHealthCostFactor(agent, type);
			return Mathf.RoundToInt(baseDamage * healthCostFactor);
		}

		private static DamageType? GetDamageTypeForButtonText(string buttonText)
		{
			switch (buttonText)
			{
				case nameof(InterfaceNameDB.rowIds.SlipThroughWindow):
					return DamageType.brokenWindow;
			}
			return null;
		}

		public static void CorrectButtonCosts(PlayfieldObject playfieldObject)
		{
			playfieldObject.NormalizeButtons();
			int buttonsCount = playfieldObject.buttons.Count;
			for (int i = 0; i < buttonsCount; i++)
			{
				string buttonText = playfieldObject.buttons[i];
				switch (buttonText)
				{
					// Actions affected by tool-cost modifiers
					case nameof(InterfaceNameDB.rowIds.UseCrowbar):
					case nameof(InterfaceNameDB.rowIds.UseWrenchToDetonate):
					case nameof(InterfaceNameDB.rowIds.UseWrenchToDeactivate):
					case nameof(InterfaceNameDB.rowIds.UseWrenchToAdjustSatellite):
					{
						Match match = Regex.Match(playfieldObject.buttonsExtra[i], "^(.*)(-?[0-9]+)$");
						if (match.Success && match.Groups.Count > 2)
						{
							int toolCost = int.Parse(match.Groups[2].Value);
							toolCost = ApplyToolCostModifiers(playfieldObject.interactingAgent, toolCost);
							playfieldObject.buttonsExtra[i] = match.Groups[1].Value + toolCost;
						}
						break;
					}

					// Actions affected by health-cost modifiers
					case nameof(InterfaceNameDB.rowIds.SlipThroughWindow):
					{
						DamageType? damageType = GetDamageTypeForButtonText(buttonText);
						if (damageType == null)
						{
							logger.LogError($"CorrectButtonCosts health-cost modifiers triggered for buttonText: '{buttonText}' but no damageType was found");
							break;
						}

						Match match = Regex.Match(playfieldObject.buttonsExtra[i], "^(.*)([0-9]+)HP$");
						if (match.Success && match.Groups.Count > 2)
						{
							int healthCost = int.Parse(match.Groups[2].Value);
							healthCost = HealthCost(playfieldObject.interactingAgent, healthCost, damageType.Value);
							playfieldObject.buttonsExtra[i] = match.Groups[1].Value + healthCost + "HP";
						}
						break;
					}
				}
			}
		}

		public static bool IsPlayerTraitActive<TraitType>()
		{
			return GameController.gameController.agentList.Any(agent => agent.isPlayer != 0 && agent.HasTrait<TraitType>());
		}

		public static bool IsPlayerTraitActive(string trait)
		{
			return GameController.gameController.agentList.Any(agent => agent.isPlayer != 0 && agent.HasTrait(trait));
		}

		// TODO to be removed soon (tm)
		public static bool DoesPlayerHaveTraitFromList(Agent agent, List<string> traits)
		{
			logger.LogDebug($"{MethodBase.GetCurrentMethod().Name} ( agent = '{agent.agentName}', traits = '{string.Join(", ", traits)}'");
			return traits.Any(agent.HasTrait);
		}
	}
}