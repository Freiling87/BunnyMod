using BunnyMod.Content.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class ManaBattery2 : CustomTrait
	{
		private const string name = nameof(ManaBattery2);

		[RLSetup]
		[UsedImplicitly]
		private static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<ManaBattery2>()
					.Localize<ManaBattery2>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(false)
							.SetAvailableInCharacterCreation(false)
							.SetCantLose(true)
							.SetCantSwap(true)
							.SetCharacterCreationCost(4)
							.SetEnabled(true)
					);

			/*
			 * TODO
			 * ManaBattery_2.SpecialAbilities.AddRange(cSpecialAbility.Magic);
			 */
			BMTraitsManager.RegisterTrait<ManaBattery2>(new BMTraitInfo(name, traitBuilder)
					.WithConflictGroup(ETraitConflictGroup.Archmage_ManaBattery)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}