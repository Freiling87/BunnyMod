using BunnyMod.Content.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class Domineering2 : CustomTrait
	{
		private const string name = nameof(Domineering2);

		[RLSetup]
		[UsedImplicitly]
		private static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<Domineering2>()
					.Localize<Domineering2>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(false)
							.SetAvailableInCharacterCreation(false)
							.SetCantLose(false)
							.SetCantSwap(true)
							.SetCharacterCreationCost(4)
							.SetEnabled(true)
					);

			BMTraitsManager.RegisterTrait<Domineering2>(new BMTraitInfo(name, traitBuilder));
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
		
		public static relStatus? SetupInitialRelationship(Agent agent, Agent otherAgent, string currentRelationship)
		{
			if (currentRelationship != nameof(relStatus.Neutral) || !otherAgent.HasTrait<Domineering2>())
			{
				// Trait not active or agent already has an opinion on otherAgent.
				return null;
			}

			if (gc.percentChance(8))
			{
				return relStatus.Submissive;
			}
			return null;
		}
	}
}