using BunnyMod.Content.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class EagleEyes : CustomTrait
	{
		private const string name = nameof(EagleEyes);

		[RLSetup]
		[UsedImplicitly]
		public static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<EagleEyes>()
					.Localize<EagleEyes>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(true)
							.SetCantSwap(false)
							.SetCharacterCreationCost(3)
							.SetEnabled(true)
					);

			BMTraitsManager.RegisterTrait<EagleEyes>(new BMTraitInfo(name, traitBuilder)
					.WithUpgrade(typeof(EagleEyes2))
					.WithConflictGroup(ETraitConflictGroup.VisionRange)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}