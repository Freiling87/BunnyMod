using BunnyMod.Content.Extensions;
using Google2u;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class MobDebt : CustomTrait
	{
		private const string name = nameof(MobDebt);

		[RLSetup]
		[UsedImplicitly]
		public static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<MobDebt>()
					.Localize<MobDebt>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(false)
							.SetCantSwap(true)
							.SetCharacterCreationCost(-10)
							.SetEnabled(true)
					);

			BMTraitsManager.RegisterTrait<MobDebt>(new BMTraitInfo(name, traitBuilder));
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }

		public static relStatus? SetupInitialRelationship(Agent agent, Agent otherAgent, string currentRelationship)
		{
			if (currentRelationship != nameof(relStatus.Neutral)
					|| !otherAgent.HasTrait<MobDebt>() || agent.IsAgent(AgentNameDB.rowIds.Mobster))
			{
				// Trait not active or agent already has an opinion on otherAgent.
				return null;
			}

			switch (gc.levelTheme)
			{
				case 0:
				case 1:
					return relStatus.Friendly;
				case 2:
					return relStatus.Neutral;
				case 3:
					return relStatus.Annoyed;
				case 4:
				case 5:
					return relStatus.Hostile;
				default:
					return null;
			}
		}

		public static bool AgentIsRival(Agent agent, Agent otherAgent)
		{
			return agent.HasTrait<MobDebt>() && otherAgent.IsAgent(AgentNameDB.rowIds.Mobster);
		}
	}
}