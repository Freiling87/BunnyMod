using BunnyMod.Content.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class Wetworker : CustomTrait
	{
		private const string name = nameof(Wetworker);

		[RLSetup]
		[UsedImplicitly]
		private static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<Wetworker>()
					.Localize<Wetworker>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(true)
							.SetCantSwap(false)
							.SetCharacterCreationCost(8)
							.SetEnabled(true)
					);

			// TODO conflict with vTrait.Loud
			// TODO recommend vSpecialAbility.Camouflage
			BMTraitsManager.RegisterTrait<Wetworker>(new BMTraitInfo(name, traitBuilder));
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}