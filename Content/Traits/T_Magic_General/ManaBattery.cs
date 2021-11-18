using BunnyMod.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Traits.T_Magic_General
{
	public class ManaBattery : CustomTrait
	{
		private const string name = nameof(ManaBattery);

		[RLSetup]
		[UsedImplicitly]
		private static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<ManaBattery>()
					.Localize<ManaBattery>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(true)
							.SetCantSwap(true)
							.SetCharacterCreationCost(2)
							.SetEnabled(true)
					);

			/*
			 * TODO
			 * ManaBattery.SpecialAbilities.AddRange(cSpecialAbility.Magic);
			 */
			BMTraitsManager.RegisterTrait<ManaBattery>(new BMTraitInfo(name, traitBuilder)
					.WithUpgrade(typeof(ManaBattery2))
					.WithConflictGroup(ETraitConflictGroup.Archmage_ManaBattery)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}