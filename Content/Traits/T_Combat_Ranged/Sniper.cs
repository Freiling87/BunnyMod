using BunnyMod.Extensions;
using BunnyMod.Traits.T_Miscellaneous;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Traits.T_Combat_Ranged
{
	public class Sniper : CustomTrait
	{
		private const string name = nameof(Sniper);

		[RLSetup]
		[UsedImplicitly]
		private static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<Sniper>()
					.Localize<Sniper>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(true)
							.SetCantSwap(true)
							.SetCharacterCreationCost(4)
							.SetEnabled(true)
					);

			BMTraitsManager.RegisterTrait<Sniper>(new BMTraitInfo(name, traitBuilder)
					.WithUpgrade(typeof(Sniper2))
					.WithConflictGroup(ETraitConflictGroup.Myopic_Sniper)
					.WithRecommendation(typeof(EagleEyes))
			);
		}

		public override void OnAdded()
		{
			StatusEffects.SpecialAbilityInterfaceCheck();
		}

		public override void OnRemoved() { }
	}
}