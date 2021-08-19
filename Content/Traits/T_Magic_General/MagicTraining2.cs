using BunnyMod.Content.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class MagicTraining2 : CustomTrait
	{
		private const string name = nameof(MagicTraining2);

		[RLSetup]
		[UsedImplicitly]
		public static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<MagicTraining2>()
					.Localize<MagicTraining2>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(false)
							.SetAvailableInCharacterCreation(false)
							.SetCantLose(true)
							.SetCantSwap(true)
							.SetCharacterCreationCost(10)
							.SetEnabled(true)
					);

			/*
			 * TODO
			 * MagicTraining_2.SpecialAbilities.AddRange(cSpecialAbility.Magic);
			 */
			BMTraitsManager.RegisterTrait<MagicTraining2>(new BMTraitInfo(name, traitBuilder)
					.WithConflictGroup(ETraitConflictGroup.Archmage_MagicTraining)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}