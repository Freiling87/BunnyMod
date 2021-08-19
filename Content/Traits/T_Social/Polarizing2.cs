using BunnyMod.Content.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;
using UnityEngine;

namespace BunnyMod.Content.Traits
{
	public class Polarizing2 : CustomTrait
	{
		private const string name = nameof(Polarizing2);

		[RLSetup]
		[UsedImplicitly]
		public static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<Polarizing2>()
					.Localize<Polarizing2>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(false)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(true)
							.SetCantSwap(false)
							.SetCharacterCreationCost(1)
							.SetEnabled(true)
					);

			// TODO conflict: vTrait.Antisocial, vTrait.Charismatic, vTrait.FairGame, vTrait.FriendoftheCommonFolk, vTrait.Malodorous, vTrait.Suspicious, vTrait.Wanted
			BMTraitsManager.RegisterTrait<Polarizing2>(new BMTraitInfo(name, traitBuilder)
					.WithConflictGroup(ETraitConflictGroup.Charisma)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }

		public static relStatus? SetupInitialRelationship(Agent agent, Agent otherAgent, string currentRelationship)
		{
			if (currentRelationship != nameof(relStatus.Neutral) || !otherAgent.HasTrait<Polarizing2>())
			{
				// Trait not active or agent already has an opinion on otherAgent.
				return null;
			}

			int roll = Random.Range(0, 100);
			return roll < 25 ? relStatus.Hostile
					: roll < 50 ? relStatus.Annoyed
					: roll < 67 ? relStatus.Friendly
					: roll < 88 ? relStatus.Loyal
					: relStatus.Aligned;
		}
	}
}