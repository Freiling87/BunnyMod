using BunnyMod.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Traits.T_Combat_Melee
{
	public class BlessedStrikes : CustomTrait
	{
		private const string name = nameof(BlessedStrikes);

		[RLSetup]
		[UsedImplicitly]
		private static void Setup()
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