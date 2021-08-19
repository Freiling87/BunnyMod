using BunnyMod.Content.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class SmoothBrained : CustomTrait
	{
		private const string name = nameof(SmoothBrained);

		[RLSetup]
		[UsedImplicitly]
		public static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<SmoothBrained>()
					.Localize<SmoothBrained>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(false)
							.SetCantSwap(true)
							.SetCharacterCreationCost(-40)
							.SetEnabled(true)
					);

			// TODO conflict vTrait.ExperienceRate
			BMTraitsManager.RegisterTrait<SmoothBrained>(new BMTraitInfo(name, traitBuilder)
					.WithConflictGroup(ETraitConflictGroup.ExperienceRate, ETraitConflictGroup.SmoothBrained_VeryHardOnYourself)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}