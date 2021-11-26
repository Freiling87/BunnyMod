﻿using BunnyMod.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Traits.T_Combat_Melee
{
	public class InfernalStrikes2 : CustomTrait
	{
		private const string name = nameof(InfernalStrikes2);

		[RLSetup]
		[UsedImplicitly]
		private static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<InfernalStrikes2>()
					.Localize<InfernalStrikes2>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(false)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(true)
							.SetCantSwap(false)
							.SetCharacterCreationCost(12)
							.SetEnabled(true)
					);

			BMTraitsManager.RegisterTrait<InfernalStrikes2>(new BMTraitInfo(name, traitBuilder)
					.WithConflictGroup(ETraitConflictGroup.StrikerTraits)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}