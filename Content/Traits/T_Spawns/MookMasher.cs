using BunnyMod.Content.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class MookMasher : CustomTrait
	{
		private const string name = nameof(MookMasher);

		[RLSetup]
		[UsedImplicitly]
		public static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<MookMasher>()
					.Localize<MookMasher>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(false)
							.SetCantSwap(true)
							.SetCharacterCreationCost(-5)
							.SetEnabled(true)
					);

			BMTraitsManager.RegisterTrait<MookMasher>(new BMTraitInfo(name, traitBuilder));
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}