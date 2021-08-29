using BunnyMod.Content.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class AfraidOfLoudNoises : CustomTrait
	{
		private const string name = nameof(AfraidOfLoudNoises);

		[RLSetup]
		[UsedImplicitly]
		private static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<AfraidOfLoudNoises>()
					.Localize<AfraidOfLoudNoises>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(false)
							.SetCantSwap(false)
							.SetCharacterCreationCost(-4)
							.SetEnabled(true)
					);

			BMTraitsManager.RegisterTrait<AfraidOfLoudNoises>(new BMTraitInfo(name, traitBuilder)
					.WithConflictGroup(ETraitConflictGroup.AfraidOfLoudNoises_DrawNoBlood, ETraitConflictGroup.LimitWeapons)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}