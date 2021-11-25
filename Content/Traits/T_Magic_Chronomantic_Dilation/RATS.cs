using BunnyMod.Extensions;
using Google2u;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Traits.T_Magic_Chronomantic_Dilation
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

		public static int GetLuckMultiplier(string luckType, Agent agent)
		{
			if (luckType == "CritChance"
					|| luckType == nameof(StatusEffectNameDB.rowIds.ChanceToSlowEnemies)
					|| luckType == nameof(StatusEffectNameDB.rowIds.ChanceAttacksDoZeroDamage)
					|| luckType == nameof(StatusEffectNameDB.rowIds.ChanceToKnockWeapons)
					|| luckType == "GunAim")
			{
				if (agent.HasTrait<RATS>())
				{
					return 1;
				}
			}
			return 0;
		}
	}
}