using BunnyMod.Content.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class Cursed2 : CustomTrait
	{
		private const string name = nameof(Cursed2);

		[RLSetup]
		[UsedImplicitly]
		public static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<Cursed2>()
					.Localize<Cursed2>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(false)
							.SetCantSwap(true)
							.SetCharacterCreationCost(-4)
							.SetEnabled(true)
					);

			BMTraitsManager.RegisterTrait<Cursed2>(new BMTraitInfo(name, traitBuilder)
					.WithConflictGroup(ETraitConflictGroup.Luck)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}