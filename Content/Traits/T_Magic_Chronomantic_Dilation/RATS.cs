using BunnyMod.Content.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class RATS : CustomTrait
	{
		private const string name = nameof(RATS);

		[RLSetup]
		[UsedImplicitly]
		private static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<RATS>()
					.Localize<RATS>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(true)
							.SetCantSwap(false)
							.SetCharacterCreationCost(3)
							.SetEnabled(true)
					);

			/*
			 * TODO
			 * RATS.Recommendations.AddRange(new[] { vTrait.Butterfingerer, cSpecialAbility.ChronomanticDilation, vTrait.IncreasedCritChance, vTrait.Kneecapper, vTrait.UnCrits });
			 */
			BMTraitsManager.RegisterTrait<RATS>(new BMTraitInfo(name, traitBuilder)
					.WithUpgrade(typeof(RATS2))
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}