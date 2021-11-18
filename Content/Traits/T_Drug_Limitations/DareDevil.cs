using BunnyMod.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Traits.T_Drug_Limitations
{
	public class DareDevil : CustomTrait
	{
		private const string name = nameof(DareDevil);

		[RLSetup]
		[UsedImplicitly]
		private static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<DareDevil>()
					.Localize<DareDevil>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(false)
							.SetCantSwap(true)
							.SetCharacterCreationCost(-3)
							.SetEnabled(true)
					);

			// TODO conflict with vTrait.Addict
			BMTraitsManager.RegisterTrait<DareDevil>(new BMTraitInfo(name, traitBuilder)
					.WithConflictGroup(ETraitConflictGroup.LimitDrugs)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}