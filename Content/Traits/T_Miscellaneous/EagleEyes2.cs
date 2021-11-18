using BunnyMod.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Traits.T_Miscellaneous
{
	public class EagleEyes2 : CustomTrait
	{
		private const string name = nameof(EagleEyes2);

		[RLSetup]
		[UsedImplicitly]
		private static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<EagleEyes2>()
					.Localize<EagleEyes2>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true) // TODO set to false after testing
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(true)
							.SetCantSwap(true)
							.SetCharacterCreationCost(6)
							.SetEnabled(true)
					);

			BMTraitsManager.RegisterTrait<EagleEyes2>(new BMTraitInfo(name, traitBuilder)
					.WithConflictGroup(ETraitConflictGroup.VisionRange)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}