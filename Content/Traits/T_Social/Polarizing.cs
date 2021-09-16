using BunnyMod.Content.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class Polarizing : CustomTrait
	{
		private const string name = nameof(Polarizing);

		[RLSetup]
		[UsedImplicitly]
		private static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<Polarizing>()
					.Localize<Polarizing>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(false)
							.SetCantLose(true)
							.SetCantSwap(false)
							.SetCharacterCreationCost(0)
							.SetEnabled(true)
					);

			// TODO conflict: vTrait.Antisocial, vTrait.Charismatic, vTrait.FairGame, vTrait.FriendoftheCommonFolk, vTrait.Malodorous, vTrait.Suspicious, vTrait.Wanted
			BMTraitsManager.RegisterTrait<Polarizing>(new BMTraitInfo(name, traitBuilder)
					.WithUpgrade(typeof(Polarizing2))
					.WithConflictGroup(ETraitConflictGroup.Charisma)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
		
		public static relStatus? SetupInitialRelationship(Agent agent, Agent otherAgent, string currentRelationship)
		{
			if (currentRelationship != nameof(relStatus.Neutral) || !otherAgent.HasTrait<Polarizing>())
			{
				// Trait not active or agent already has an opinion on otherAgent.
				return null;
			}

			return gc.percentChance(50) ? relStatus.Annoyed : relStatus.Friendly;
		}
	}
}