using BunnyMod.Content.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class Archmage : CustomTrait
	{
		private const string name = nameof(Archmage);

		[RLSetup]
		[UsedImplicitly]
		public static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<Archmage>()
					.Localize<Archmage>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(true)
							.SetCantSwap(true)
							.SetCharacterCreationCost(32)
							.SetEnabled(true)
					);

			// TODO recommend cSpecialAbility.Magic
			// TODO .SpecialAbilities = cSpecialAbility.Magic
			BMTraitsManager.RegisterTrait<Archmage>(new BMTraitInfo(name, traitBuilder)
					.WithConflictGroup(
							ETraitConflictGroup.Archmage_FocusedCasting,
							ETraitConflictGroup.Archmage_MagicTraining,
							ETraitConflictGroup.Archmage_ManaBattery,
							ETraitConflictGroup.Archmage_WildCasting
					)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}