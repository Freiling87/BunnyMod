using BunnyMod.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Traits.T_Equipment_Limitations
{
	public class FatHead : CustomTrait
	{
		private const string name = nameof(FatHead);

		[RLSetup]
		[UsedImplicitly]
		private static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<FatHead>()
					.Localize<FatHead>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(false)
							.SetCantSwap(true)
							.SetCharacterCreationCost(-1)
							.SetEnabled(true)
					);

			// TODO conflict vTrait.Diminutive
			BMTraitsManager.RegisterTrait<FatHead>(new BMTraitInfo(name, traitBuilder));
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}