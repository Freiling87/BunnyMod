using BunnyMod.Content.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class Charmed2 : CustomTrait
	{
		private const string name = nameof(Charmed2);

		[RLSetup]
		[UsedImplicitly]
		public static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<Charmed2>()
					.Localize<Charmed2>()
					.WithUnlock(new TraitUnlock(name, false) // TODO trait disabled
							.SetAvailable(false)
							.SetAvailableInCharacterCreation(false)
							.SetCantLose(true)
							.SetCantSwap(false)
							.SetCharacterCreationCost(10)
							.SetEnabled(false)
					);

			BMTraitsManager.RegisterTrait<Charmed2>(new BMTraitInfo(name, traitBuilder)
					.WithConflictGroup(ETraitConflictGroup.Luck)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}