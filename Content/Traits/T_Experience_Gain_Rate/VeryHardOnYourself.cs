using BunnyMod.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Traits.T_Experience_Gain_Rate
{
	public class VeryHardOnYourself : CustomTrait
	{
		private const string name = nameof(VeryHardOnYourself);

		[RLSetup]
		[UsedImplicitly]
		private static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<VeryHardOnYourself>()
					.Localize<VeryHardOnYourself>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(false)
							.SetCantSwap(true)
							.SetCharacterCreationCost(-6)
							.SetEnabled(true)
					);

			// TODO prerequisite vTrait.TheLaw, vTrait.Extortionist
			BMTraitsManager.RegisterTrait<VeryHardOnYourself>(new BMTraitInfo(name, traitBuilder)
					.WithConflictGroup(ETraitConflictGroup.SmoothBrained_VeryHardOnYourself)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}