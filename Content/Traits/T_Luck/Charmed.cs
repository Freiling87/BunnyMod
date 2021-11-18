using BunnyMod.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Traits.T_Luck
{
	public class Charmed : CustomTrait
	{
		private const string name = nameof(Charmed);

		[RLSetup]
		[UsedImplicitly]
		private static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<Charmed>()
					.Localize<Charmed>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(true)
							.SetCantSwap(false)
							.SetCharacterCreationCost(5)
							.SetEnabled(true)
					);

			BMTraitsManager.RegisterTrait<Charmed>(new BMTraitInfo(name, traitBuilder)
					.WithUpgrade(typeof(Charmed2))
					.WithConflictGroup(ETraitConflictGroup.Luck)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}