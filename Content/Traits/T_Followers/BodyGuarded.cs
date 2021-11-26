using BunnyMod.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Traits.T_Followers
{
	public class BodyGuarded : CustomTrait
	{
		private const string name = nameof(BodyGuarded);

		[RLSetup]
		[UsedImplicitly]
		private static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<BodyGuarded>()
					.Localize<BodyGuarded>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(true)
							.SetCantSwap(false)
							.SetCharacterCreationCost(7)
							.SetEnabled(true)
					);

			BMTraitsManager.RegisterTrait<BodyGuarded>(new BMTraitInfo(name, traitBuilder)
					.WithUpgrade(typeof(BodyGuarded2))
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}