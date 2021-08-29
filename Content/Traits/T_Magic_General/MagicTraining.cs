using BunnyMod.Content.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class MagicTraining : CustomTrait
	{
		private const string name = nameof(MagicTraining);

		[RLSetup]
		[UsedImplicitly]
		private static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<MagicTraining>()
					.Localize<MagicTraining>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(true)
							.SetCantSwap(true)
							.SetCharacterCreationCost(5)
							.SetEnabled(true)
					);

			/*
			 * TODO
			 * MagicTraining.Recommendations.AddRange(cSpecialAbility.Magic);
			 * MagicTraining.SpecialAbilities.AddRange(cSpecialAbility.Magic);
			 */
			BMTraitsManager.RegisterTrait<MagicTraining>(new BMTraitInfo(name, traitBuilder)
					.WithUpgrade(typeof(MagicTraining2))
					.WithConflictGroup(ETraitConflictGroup.Archmage_MagicTraining)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}