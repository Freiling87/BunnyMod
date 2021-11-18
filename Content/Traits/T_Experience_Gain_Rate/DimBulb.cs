using BunnyMod.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Traits.T_Experience_Gain_Rate
{
	public class DimBulb : CustomTrait
	{
		private const string name = nameof(DimBulb);

		[RLSetup]
		[UsedImplicitly]
		private static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<DimBulb>()
					.Localize<DimBulb>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(false)
							.SetCantSwap(true)
							.SetCharacterCreationCost(-10)
							.SetEnabled(true)
					);

			BMTraitsManager.RegisterTrait<DimBulb>(new BMTraitInfo(name, traitBuilder)
					.WithConflictGroup(ETraitConflictGroup.ExperienceRate)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}