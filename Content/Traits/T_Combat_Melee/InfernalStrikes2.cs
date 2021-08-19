using BunnyMod.Content.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class InfernalStrikes2 : CustomTrait
	{
		private const string name = nameof(InfernalStrikes2);

		[RLSetup]
		[UsedImplicitly]
		public static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<InfernalStrikes2>()
					.Localize<InfernalStrikes2>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(false)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(true)
							.SetCantSwap(false)
							.SetCharacterCreationCost(12)
							.SetEnabled(true)
					);

			BMTraitsManager.RegisterTrait<InfernalStrikes2>(new BMTraitInfo(name, traitBuilder)
					.WithConflictGroup(ETraitConflictGroup.StrikerTraits)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}