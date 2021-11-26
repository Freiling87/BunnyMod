using BunnyMod.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Traits.T_Combat_Ranged
{
	public class DoubleTapper : CustomTrait
	{
		private const string name = nameof(DoubleTapper);

		[RLSetup]
		[UsedImplicitly]
		private static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<DoubleTapper>()
					.Localize<DoubleTapper>()
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
			BMTraitsManager.RegisterTrait<DoubleTapper>(new BMTraitInfo(name, traitBuilder));
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}