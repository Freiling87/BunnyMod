using BunnyMod.Content.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class BootLicker : CustomTrait
	{
		private const string name = nameof(BootLicker);

		[RLSetup]
		[UsedImplicitly]
		public static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<BootLicker>()
					.Localize<BootLicker>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(false)
							.SetCantSwap(true)
							.SetCharacterCreationCost(-1)
							.SetEnabled(true)
					);

			// TODO conflict: vTrait.Charismatic, vTrait.CorruptionCosts, vTrait.Wanted, vTrait.TheLaw
			BMTraitsManager.RegisterTrait<BootLicker>(new BMTraitInfo(name, traitBuilder)
					.WithConflictGroup(
							ETraitConflictGroup.PoliceStanding,
							ETraitConflictGroup.BootLicker_Unpleasant
					)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }

		public static relStatus? SetupInitialRelationship(Agent agent, Agent otherAgent, string currentRelationship)
		{
			if (currentRelationship != nameof(relStatus.Neutral) || !otherAgent.HasTrait<BootLicker>())
			{
				// Trait not active or agent already has an opinion on otherAgent.
				return null;
			}

			if (agent.IsEnforcer())
			{
				return relStatus.Friendly;
			}
			if (agent.IsCriminal())
			{
				return gc.percentChance(4) ? relStatus.Hostile : relStatus.Annoyed;
			}
			return null;
		}
		
		public static bool AgentIsRival(Agent agent, Agent otherAgent)
		{
			return agent.HasTrait<BootLicker>() && otherAgent.IsCriminal();
		}
	}
}