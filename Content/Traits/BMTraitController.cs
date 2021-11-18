using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
			return Mathf.FloorToInt(baseCost * costFactor);
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

		public static string HealthCost(Agent agent, int baseDamage, DamageType type)
		{
			logger.LogDebug($"{MethodBase.GetCurrentMethod().Name} ( agent = '{agent.agentName}', baseDamage = '{baseDamage}', damageType = '{type}'");
			float healthCostFactor = GetHealthCostFactor(agent, type);
			return Mathf.FloorToInt(baseDamage * healthCostFactor).ToString();
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