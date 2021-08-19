using BunnyMod.Content.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class BlessedStrikes2 : CustomTrait
	{
		private const string name = nameof(BlessedStrikes2);

		[RLSetup]
		[UsedImplicitly]
		public static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<BlessedStrikes2>()
					.Localize<BlessedStrikes2>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(false)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(true)
							.SetCantSwap(false)
							.SetCharacterCreationCost(5)
							.SetEnabled(true)
					);

			BMTraitsManager.RegisterTrait<BlessedStrikes2>(new BMTraitInfo(name, traitBuilder)
					.WithConflictGroup(ETraitConflictGroup.StrikerTraits)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}