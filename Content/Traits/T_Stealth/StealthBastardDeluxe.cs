using BunnyMod.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Traits.T_Stealth
{
	public class StealthBastardDeluxe : CustomTrait
	{
		private const string name = nameof(StealthBastardDeluxe);

		[RLSetup]
		[UsedImplicitly]
		private static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<StealthBastardDeluxe>()
					.Localize<StealthBastardDeluxe>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(true)
							.SetCantSwap(false)
							.SetCharacterCreationCost(4)
							.SetEnabled(true)
					);

			// TODO conflict: vTrait.Loud, vTrait.Bulky
			BMTraitsManager.RegisterTrait<StealthBastardDeluxe>(new BMTraitInfo(name, traitBuilder));
		}
		
		public static float GetBrokenWindowHealthCostFactor(Agent agent)
		{
			return agent.HasTrait<StealthBastardDeluxe>() ? 0f : 1f;
		}

		public static bool CanHideInObject(ObjectReal objectInstance)
		{
			return objectInstance is Bathtub || objectInstance is Plant || objectInstance is PoolTable || objectInstance is TableBig;
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}