using BunnyMod.Content.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class ObjectivelyUnpleasant : CustomTrait
	{
		private const string name = nameof(ObjectivelyUnpleasant);

		[RLSetup]
		[UsedImplicitly]
		public static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<ObjectivelyUnpleasant>()
					.Localize<ObjectivelyUnpleasant>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(false)
							.SetCantSwap(true)
							.SetCharacterCreationCost(-8)
							.SetEnabled(true)
					);

			// TODO conflict: vTrait.Antisocial, vTrait.Charismatic, vTrait.FairGame, vTrait.FriendoftheCommonFolk, vTrait.Malodorous, vTrait.Suspicious, vTrait.Wanted
			BMTraitsManager.RegisterTrait<ObjectivelyUnpleasant>(new BMTraitInfo(name, traitBuilder)
					.WithConflictGroup(
							ETraitConflictGroup.Charisma,
							ETraitConflictGroup.BootLicker_Unpleasant
					)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
		
		public static relStatus? SetupInitialRelationship(Agent agent, Agent otherAgent, string currentRelationship)
		{
			if (currentRelationship != nameof(relStatus.Neutral) || !otherAgent.HasTrait<ObjectivelyUnpleasant>())
			{
				// Trait not active or agent already has an opinion on otherAgent.
				return null;
			}

			return gc.percentChance(4) ? relStatus.Hostile : relStatus.Annoyed;
		}
	}
}