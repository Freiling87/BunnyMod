using BunnyMod.Content.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class Vegetarian : CustomTrait
	{
		private const string name = nameof(Vegetarian);

		[RLSetup]
		[UsedImplicitly]
		private static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<Vegetarian>()
					.Localize<Vegetarian>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(false)
							.SetCantSwap(false)
							.SetCharacterCreationCost(-1)
							.SetEnabled(true)
					);

			// TODO add conflict with vTrait.Zombiism
			// TODO add conflict with vTrait.LimitFood
			BMTraitsManager.RegisterTrait<Vegetarian>(new BMTraitInfo(name, traitBuilder)
					.WithConflictGroup(ETraitConflictGroup.LimitFood)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}