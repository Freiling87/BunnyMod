using BunnyMod.Content.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class Myopic2 : CustomTrait
	{
		private const string name = nameof(Myopic2);

		[RLSetup]
		[UsedImplicitly]
		public static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<Myopic2>()
					.Localize<Myopic2>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(false)
							.SetCantSwap(false)
							.SetCharacterCreationCost(-8)
							.SetEnabled(true)
					);

			BMTraitsManager.RegisterTrait<Myopic2>(new BMTraitInfo(name, traitBuilder)
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