using BunnyMod.Content.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class Haunted : CustomTrait
	{
		private const string name = nameof(Haunted);

		[RLSetup]
		[UsedImplicitly]
		public static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<Haunted>()
					.Localize<Haunted>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(false)
							.SetCantSwap(true)
							.SetCharacterCreationCost(-2)
							.SetEnabled(true)
					);

			BMTraitsManager.RegisterTrait<Haunted>(new BMTraitInfo(name, traitBuilder));
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}