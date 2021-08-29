using BunnyMod.Content.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class FocusedCasting : CustomTrait
	{
		private const string name = nameof(FocusedCasting);

		[RLSetup]
		[UsedImplicitly]
		private static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<FocusedCasting>()
					.Localize<FocusedCasting>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(true)
							.SetCantSwap(true)
							.SetCharacterCreationCost(3)
							.SetEnabled(true)
					);

			/*
			 * TODO
			 * FocusedCasting.Recommendations.AddRange(cSpecialAbility.Magic);
			 * FocusedCasting.SpecialAbilities.AddRange(cSpecialAbility.Magic);
			 */
			BMTraitsManager.RegisterTrait<FocusedCasting>(new BMTraitInfo(name, traitBuilder)
					.WithUpgrade(typeof(FocusedCasting2))
					.WithConflictGroup(
							ETraitConflictGroup.Archmage_FocusedCasting,
							ETraitConflictGroup.MagicCasting
					)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}