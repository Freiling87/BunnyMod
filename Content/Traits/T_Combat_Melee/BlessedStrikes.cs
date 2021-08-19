using BunnyMod.Content.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class BlessedStrikes : CustomTrait
	{
		private const string name = nameof(BlessedStrikes);
		
		[RLSetup]
		[UsedImplicitly]
		public static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<BlessedStrikes>()
					.Localize<BlessedStrikes>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(true)
							.SetCantSwap(false)
							.SetCharacterCreationCost(2)
							.SetEnabled(true)
					);

			BMTraitsManager.RegisterTrait<BlessedStrikes>(new BMTraitInfo(name, traitBuilder)
					.WithUpgrade(typeof(BlessedStrikes2))
					.WithConflictGroup(ETraitConflictGroup.StrikerTraits)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}