using BunnyMod.Content.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class Carnivore : CustomTrait
	{
		private const string name = nameof(Carnivore);

		[RLSetup]
		[UsedImplicitly]
		public static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<Carnivore>()
					.Localize<Carnivore>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(false)
							.SetCantSwap(true)
							.SetCharacterCreationCost(-1)
							.SetEnabled(true)
					);

			// TODO add conflict with vTrait.LimitFood
			// TODO add conflict with vTrait.BananaLover
			BMTraitsManager.RegisterTrait<Carnivore>(new BMTraitInfo(name, traitBuilder)
					.WithConflictGroup(ETraitConflictGroup.LimitFood)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}