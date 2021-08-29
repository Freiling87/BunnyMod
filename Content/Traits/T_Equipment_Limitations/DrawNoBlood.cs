using BunnyMod.Content.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class DrawNoBlood : CustomTrait
	{
		private const string name = nameof(DrawNoBlood);

		[RLSetup]
		[UsedImplicitly]
		private static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<DrawNoBlood>()
					.Localize<DrawNoBlood>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(false)
							.SetCantSwap(true)
							.SetCharacterCreationCost(-5)
							.SetEnabled(true)
					);

			// TODO conflict vTrait.Jugularious, vTrait.FleshFeast
			BMTraitsManager.RegisterTrait<DrawNoBlood>(new BMTraitInfo(name, traitBuilder)
					.WithConflictGroup(ETraitConflictGroup.AfraidOfLoudNoises_DrawNoBlood, ETraitConflictGroup.LimitWeapons)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}