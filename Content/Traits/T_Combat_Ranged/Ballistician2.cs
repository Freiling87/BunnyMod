using BunnyMod.Content.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class Ballistician2 : CustomTrait
	{
		private const string name = nameof(Ballistician2);

		[RLSetup]
		[UsedImplicitly]
		private static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<Ballistician2>()
					.Localize<Ballistician2>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(false)
							.SetAvailableInCharacterCreation(false)
							.SetCantLose(true)
							.SetCantSwap(false)
							.SetCharacterCreationCost(8)
							.SetEnabled(true)
					);

			BMTraitsManager.RegisterTrait<Ballistician2>(new BMTraitInfo(name, traitBuilder)
					.WithConflictGroup(ETraitConflictGroup.Myopic_Ballistician)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}