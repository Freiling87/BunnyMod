using BunnyMod.Content.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class Priors : CustomTrait
	{
		private const string name = nameof(Priors);

		[RLSetup]
		[UsedImplicitly]
		private static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<Priors>()
					.Localize<Priors>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(false)
							.SetCantSwap(false)
							.SetCharacterCreationCost(3)
							.SetEnabled(true)
					);

			// TODO conflict: vTrait.Charismatic, vTrait.TheLaw
			BMTraitsManager.RegisterTrait<Priors>(new BMTraitInfo(name, traitBuilder)
					.WithConflictGroup(ETraitConflictGroup.PoliceStanding)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
		
		public static relStatus? SetupInitialRelationship(Agent agent, Agent otherAgent, string currentRelationship)
		{
			if (currentRelationship != nameof(relStatus.Neutral) || !otherAgent.HasTrait<Priors>())
			{
				// Trait not active or agent already has an opinion on otherAgent.
				return null;
			}

			if (agent.IsEnforcer())
			{
				return gc.percentChance(4) ? relStatus.Hostile : relStatus.Annoyed;
			}
			if (agent.IsCriminal())
			{
				return relStatus.Friendly;
			}
			return null;
		}

		public static bool AgentIsRival(Agent agent, Agent otherAgent)
		{
			return agent.HasTrait<Priors>() && otherAgent.IsEnforcer();
		}
	}
}