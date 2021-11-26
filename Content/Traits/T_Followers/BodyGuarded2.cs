using BunnyMod.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Traits.T_Followers
{
	public class BodyGuarded2 : CustomTrait
	{
		private const string name = nameof(BodyGuarded2);

		[RLSetup]
		[UsedImplicitly]
		private static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<BodyGuarded2>()
					.Localize<BodyGuarded2>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(false)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(true)
							.SetCantSwap(false)
							.SetCharacterCreationCost(12)
							.SetEnabled(true)
					);

			BMTraitsManager.RegisterTrait<BodyGuarded2>(new BMTraitInfo(name, traitBuilder));
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}