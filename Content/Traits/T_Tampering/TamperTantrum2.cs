using BunnyMod.Content.Extensions;
using Google2u;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class TamperTantrum2 : CustomTrait
	{
		private const string name = nameof(TamperTantrum2);

		[RLSetup]
		[UsedImplicitly]
		public static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<TamperTantrum2>()
					.Localize<TamperTantrum2>()
					.WithUnlock(new TraitUnlock(name, false)
							.SetAvailable(false)
							.SetAvailableInCharacterCreation(false)
							.SetCantLose(true)
							.SetCantSwap(true)
							.SetCharacterCreationCost(5)
							.SetEnabled(true)
					);

			BMTraitsManager.RegisterTrait<TamperTantrum2>(new BMTraitInfo(name, traitBuilder)
					.WithConflictGroup(ETraitConflictGroup.Myopic_Ballistician)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
		
		public static float GetToolCostFactor(Agent agent)
		{
			return agent.HasTrait<TamperTantrum2>() ? 0f : 1f;
		}
		
		public static void AgentInteractions_AddButton_Prefix(string buttonName, ref string extraCost, Agent mostRecentInteractingAgent)
		{
			if (buttonName == nameof(InterfaceNameDB.rowIds.RemoveHelmetWrench) && extraCost.EndsWith("-30") && mostRecentInteractingAgent.HasTrait<TamperTantrum2>())
			{
				extraCost = extraCost.Substring(0, extraCost.Length - 2) + "0";
			}
		}
	}
}