using System.Linq;
using BunnyMod.Content.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class Warlord : CustomTrait
	{
		private const string name = nameof(Warlord);

		[RLSetup]
		[UsedImplicitly]
		public static void Setup()
		{
			// TODO move this somewhere else, add localization
			RogueLibs.CreateCustomName(cDialogue.WarlordSubmission, vNameType.Dialogue, new CustomNameInfo("Okay, okay! You're the boss now!"));
			
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<Warlord>()
					.Localize<Warlord>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(true)
							.SetCantSwap(false)
							.SetCharacterCreationCost(32)
							.SetEnabled(true)
					);

			BMTraitsManager.RegisterTrait<Warlord>(new BMTraitInfo(name, traitBuilder));
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }

		public static bool Agent_CanShakeDown_Prefix(ref bool result, Agent instance)
		{
			if (instance.HasTrait<Warlord>())
			{
				result = true;
				return false;
			}
			return true;
		}
		
		public static bool ObjectMult_RemoveShakedownPerson_Prefix(ObjectMult instance)
		{
			// Intercept ShakedownFail XP loss for Warlord if not Extortionist
			return !instance.agent.HasTrait<Warlord>() || instance.agent.HasTrait(vTrait.Extortionist) || instance.agent.HasTrait(vTrait.Extortionist_2);
		}

		// TODO implement this
		public static void StatusEffects_ChangeHealth_ShakedownHook(StatusEffects instance)
		{
			Agent hurtAgent = instance.agent;
			Agent shakedowningAgent = GameController.gameController.playerAgentList.First(agent => agent.HasTrait<Warlord>());
			if (shakedowningAgent != null)
			{
				hurtAgent.relationships.SetRel(shakedowningAgent, nameof(relStatus.Submissive));
				BMHeaderTools.SayDialogue(hurtAgent, cDialogue.WarlordSubmission, vNameType.Dialogue);
			}
		}
	}
}