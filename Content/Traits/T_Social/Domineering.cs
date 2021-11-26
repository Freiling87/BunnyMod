using BunnyMod.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Traits.T_Social
{
	public class Domineering : CustomTrait
	{
		private const string name = nameof(Domineering);

		[RLSetup]
		[UsedImplicitly]
		private static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<Domineering>()
					.Localize<Domineering>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(false)
							.SetCantSwap(true)
							.SetCharacterCreationCost(2)
							.SetEnabled(true)
					);

			BMTraitsManager.RegisterTrait<Domineering>(new BMTraitInfo(name, traitBuilder)
					.WithUpgrade(typeof(Domineering2))
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
		
		public static relStatus? SetupInitialRelationship(Agent agent, Agent otherAgent, string currentRelationship)
		{
			if (currentRelationship != nameof(relStatus.Neutral) || !otherAgent.HasTrait<Domineering>())
			{
				// Trait not active or agent already has an opinion on otherAgent.
				return null;
			}

			if (gc.percentChance(4))
			{
				return relStatus.Submissive;
			}
			return null;
		}
	}
}