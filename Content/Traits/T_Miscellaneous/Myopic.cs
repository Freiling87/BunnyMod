using BunnyMod.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Traits.T_Miscellaneous
{
	public class Myopic : CustomTrait
	{
		private const string name = nameof(Myopic);

		[RLSetup]
		[UsedImplicitly]
		private static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<Myopic>()
					.Localize<Myopic>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(false)
							.SetCantSwap(false)
							.SetCharacterCreationCost(-4)
							.SetEnabled(true)
					);

			BMTraitsManager.RegisterTrait<Myopic>(new BMTraitInfo(name, traitBuilder)
					.WithConflictGroup(
							ETraitConflictGroup.VisionRange,
							ETraitConflictGroup.Myopic_Ballistician,
							ETraitConflictGroup.Myopic_Sniper
					)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}