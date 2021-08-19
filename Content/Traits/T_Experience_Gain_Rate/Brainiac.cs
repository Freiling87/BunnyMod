using BunnyMod.Content.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class Brainiac : CustomTrait
	{
		private const string name = nameof(Brainiac);

		[RLSetup]
		[UsedImplicitly]
		public static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<Brainiac>()
					.Localize<Brainiac>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(true)
							.SetCantSwap(true)
							.SetCharacterCreationCost(40)
							.SetEnabled(true)
					);

			BMTraitsManager.RegisterTrait<Brainiac>(new BMTraitInfo(name, traitBuilder)
					.WithConflictGroup(ETraitConflictGroup.ExperienceRate)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}