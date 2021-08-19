using BunnyMod.Content.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class FocusedCasting2 : CustomTrait
	{
		private const string name = nameof(FocusedCasting2);

		[RLSetup]
		[UsedImplicitly]
		public static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<FocusedCasting2>()
					.Localize<FocusedCasting2>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(false)
							.SetAvailableInCharacterCreation(false)
							.SetCantLose(true)
							.SetCantSwap(true)
							.SetCharacterCreationCost(6)
							.SetEnabled(true)
					);

			BMTraitsManager.RegisterTrait<FocusedCasting2>(new BMTraitInfo(name, traitBuilder)
					.WithConflictGroup(
							ETraitConflictGroup.Archmage_FocusedCasting,
							ETraitConflictGroup.MagicCasting
					)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}