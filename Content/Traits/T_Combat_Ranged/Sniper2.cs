using BunnyMod.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Traits.T_Combat_Ranged
{
	public class Sniper2 : CustomTrait
	{
		private const string name = nameof(Sniper2);

		[RLSetup]
		[UsedImplicitly]
		private static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<Sniper2>()
					.Localize<Sniper2>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(false)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(true)
							.SetCantSwap(true)
							.SetCharacterCreationCost(8)
							.SetEnabled(true)
					);

			BMTraitsManager.RegisterTrait<Sniper2>(new BMTraitInfo(name, traitBuilder)
					.WithConflictGroup(ETraitConflictGroup.Myopic_Sniper)
			);
		}

		public override void OnAdded()
		{
			StatusEffects.SpecialAbilityInterfaceCheck();
		}

		public override void OnRemoved() { }
	}
}