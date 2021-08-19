using BunnyMod.Content.Extensions;
using Google2u;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class TamperTantrum : CustomTrait
	{
		private const string name = nameof(TamperTantrum);

		[RLSetup]
		[UsedImplicitly]
		public static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<TamperTantrum>()
					.Localize<TamperTantrum>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(true)
							.SetCantSwap(false)
							.SetCharacterCreationCost(2)
							.SetEnabled(true)
					);

			BMTraitsManager.RegisterTrait<TamperTantrum>(new BMTraitInfo(name, traitBuilder)
					.WithUpgrade(typeof(TamperTantrum2))
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }

		public static float GetToolCostFactor(Agent agent)
		{
			return agent.HasTrait<TamperTantrum>() ? 0.5f : 1f;
		}
		
		public static void AgentInteractions_AddButton_Prefix(string buttonName, ref string extraCost, Agent mostRecentInteractingAgent)
		{
			if (buttonName == nameof(InterfaceNameDB.rowIds.RemoveHelmetWrench) && extraCost.EndsWith("-30") && mostRecentInteractingAgent.HasTrait<TamperTantrum>())
			{
				extraCost = extraCost.Substring(0, extraCost.Length - 2) + "15";
			}
		}
	}
}