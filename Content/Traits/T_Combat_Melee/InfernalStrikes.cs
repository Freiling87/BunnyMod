using BunnyMod.Content.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class InfernalStrikes : CustomTrait
	{
		private const string name = nameof(InfernalStrikes);

		[RLSetup]
		[UsedImplicitly]
		public static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<InfernalStrikes>()
					.Localize<InfernalStrikes>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(true)
							.SetCantSwap(false)
							.SetCharacterCreationCost(6)
							.SetEnabled(true)
					);

			BMTraitsManager.RegisterTrait<InfernalStrikes>(new BMTraitInfo(name, traitBuilder)
					.WithUpgrade(typeof(InfernalStrikes2))
					.WithConflictGroup(ETraitConflictGroup.StrikerTraits)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}