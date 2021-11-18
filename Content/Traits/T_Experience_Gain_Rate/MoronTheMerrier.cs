using BunnyMod.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Traits.T_Experience_Gain_Rate
{
	public class MoronTheMerrier : CustomTrait
	{
		private const string name = nameof(MoronTheMerrier);

		[RLSetup]
		[UsedImplicitly]
		private static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<MoronTheMerrier>()
					.Localize<MoronTheMerrier>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(false)
							.SetCantSwap(true)
							.SetCharacterCreationCost(-20)
							.SetEnabled(true)
					);
			
			BMTraitsManager.RegisterTrait<MoronTheMerrier>(new BMTraitInfo(name, traitBuilder)
					.WithConflictGroup(ETraitConflictGroup.ExperienceRate)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}