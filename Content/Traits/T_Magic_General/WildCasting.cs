using BunnyMod.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Traits.T_Magic_General
{
	public class WildCasting : CustomTrait
	{
		private const string name = nameof(WildCasting);

		[RLSetup]
		[UsedImplicitly]
		private static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<WildCasting>()
					.Localize<WildCasting>()
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
			 * WildCasting.Recommendations.AddRange(cSpecialAbility.Magic);
			 * WildCasting.SpecialAbilities.AddRange(cSpecialAbility.Magic);
			 */
			BMTraitsManager.RegisterTrait<WildCasting>(new BMTraitInfo(name, traitBuilder)
					.WithUpgrade(typeof(WildCasting2))
					.WithConflictGroup(
							ETraitConflictGroup.Archmage_WildCasting,
							ETraitConflictGroup.MagicCasting
					)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}