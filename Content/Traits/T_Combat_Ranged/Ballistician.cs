using BunnyMod.Content.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class Ballistician : CustomTrait
	{
		private const string name = nameof(Ballistician);

		[RLSetup]
		[UsedImplicitly]
		private static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<Ballistician>()
					.Localize<Ballistician>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(true)
							.SetCantSwap(false)
							.SetCharacterCreationCost(4)
							.SetEnabled(true)
					);

			BMTraitsManager.RegisterTrait<Ballistician>(new BMTraitInfo(name, traitBuilder)
					.WithUpgrade(typeof(Ballistician2))
					.WithConflictGroup(ETraitConflictGroup.Myopic_Ballistician)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}