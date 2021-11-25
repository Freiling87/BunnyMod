using BunnyMod.Extensions;
using Google2u;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Traits.T_Magic_Chronomantic_Dilation
{
	public class RATS2 : CustomTrait
	{
		private const string name = nameof(RATS2);

	[RLSetup]
		[UsedImplicitly]
		private static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<RATS2>()
					.Localize<RATS2>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(false)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(true)
							.SetCantSwap(true)
							.SetCharacterCreationCost(12)
							.SetEnabled(true)
					);

			/*
			 * TODO
			 * RATS_2.Recommendations.AddRange(new[] { vTrait.Butterfingerer, cSpecialAbility.ChronomanticDilation, vTrait.IncreasedCritChance, vTrait.Kneecapper, vTrait.UnCrits });
			 */
			BMTraitsManager.RegisterTrait<RATS2>(new BMTraitInfo(name, traitBuilder));
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
		
		public static int GetLuckMultiplier(string luckType, Agent agent)
		{
			if (luckType == "CritChance"
					|| luckType == nameof(StatusEffectNameDB.rowIds.ChanceToSlowEnemies)
					|| luckType == nameof(StatusEffectNameDB.rowIds.ChanceAttacksDoZeroDamage)
					|| luckType == nameof(StatusEffectNameDB.rowIds.ChanceToKnockWeapons)
					|| luckType == "GunAim")
			{
				if (agent.HasTrait<RATS2>())
				{
					return 2;
				}
			}
			return 0;
		}
	}
}