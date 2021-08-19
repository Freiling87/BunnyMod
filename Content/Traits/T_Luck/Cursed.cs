using BunnyMod.Content.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class Cursed : CustomTrait
	{
		private const string name = nameof(Cursed);

		[RLSetup]
		[UsedImplicitly]
		public static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<Cursed>()
					.Localize<Cursed>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(false)
							.SetCantSwap(true)
							.SetCharacterCreationCost(-2)
							.SetEnabled(true)
					);

			BMTraitsManager.RegisterTrait<Cursed>(new BMTraitInfo(name, traitBuilder)
					.WithConflictGroup(ETraitConflictGroup.Luck)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}