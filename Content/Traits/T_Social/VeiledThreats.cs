using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using BTHarmonyUtils.TranspilerUtils;
using BunnyMod.Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Traits.T_Social
{
	public class VeiledThreats : CustomTrait
	{
		private const string name = nameof(VeiledThreats);

		private static readonly HashSet<Agent> veiledThreadsUsedOn = new HashSet<Agent>();

		[RLSetup]
		[UsedImplicitly]
		private static void Setup()
		{
			// TODO move this somewhere else, add localization
			RogueLibs.CreateCustomName(cDialogue.VeiledThreatsAnnoyed, vNameType.Dialogue, new CustomNameInfo("Did you just... threaten me?"));
			
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<VeiledThreats>()
					.Localize<VeiledThreats>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(true)
							.SetCantSwap(false)
							.SetCharacterCreationCost(5)
							.SetEnabled(true)
					);

			// TODO recommend: vTrait.Extortionist, vTrait.Mugger
			BMTraitsManager.RegisterTrait<VeiledThreats>(new BMTraitInfo(name, traitBuilder)
					.WithConflictGroup(ETraitConflictGroup.Myopic_Ballistician)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }

		public static void ResetForAgent(Agent agent)
		{
			if (veiledThreadsUsedOn.Contains(agent))
			{
				veiledThreadsUsedOn.Remove(agent);
			}
		}

		private static bool AlreadyUsedOnAgent(Agent agent)
		{
			return veiledThreadsUsedOn.Contains(agent);
		}

		public static CodeReplacementPatch ThreatenFailureHook(ILGenerator generator) =>
				GetInteractionPatch(generator, nameof(HandleThreatenFailed));

		public static CodeReplacementPatch ThreatenAskFailureHook(ILGenerator generator) =>
				GetInteractionPatch(generator, nameof(HandleThreatenAskFailed));

		private static bool HandleThreatenFailed(Agent agent, Agent interactingAgent)
		{
			if (!interactingAgent.HasTrait<VeiledThreats>() || AlreadyUsedOnAgent(agent))
			{
				return true;
			}

			BMHeaderTools.SayDialogue(agent, cDialogue.VeiledThreatsAnnoyed, vNameType.Dialogue);
			agent.relationships.SetRel(interactingAgent, nameof(relStatus.Annoyed));
			agent.relationships.SetRelHate(interactingAgent, 2);
			veiledThreadsUsedOn.Add(agent);
			return false;
		}

		private static bool HandleThreatenAskFailed(Agent agent, Agent interactingAgent)
		{
			if (!interactingAgent.HasTrait<VeiledThreats>() || AlreadyUsedOnAgent(agent))
			{
				return true;
			}

			BMHeaderTools.SayDialogue(agent, cDialogue.VeiledThreatsAnnoyed, vNameType.Dialogue);
			agent.relationships.SetRel(interactingAgent, nameof(relStatus.Annoyed));
			agent.relationships.SetRelHate(interactingAgent, 2);
			agent.oma.didAsk = true;
			veiledThreadsUsedOn.Add(agent);
			return false;
		}

		private static CodeReplacementPatch GetInteractionPatch(ILGenerator generator, string handler)
		{
			Label continueLabel = generator.DefineLabel();

			MethodInfo handlerMethod = AccessTools.Method(typeof(VeiledThreats), handler, new[] { typeof(Agent), typeof(Agent) });
			MethodInfo playfieldObject_stopInteraction = AccessTools.Method(typeof(PlayfieldObject), nameof(PlayfieldObject.StopInteraction));
			MethodInfo agent_sayDialogue = AccessTools.Method(typeof(Agent), nameof(Agent.SayDialogue), new[] { typeof(string), typeof(bool) });

			/* Look for
			 *  agent.StopInteraction();
			 *  <INSERT CODE HERE>
			 *  agent.SayDialogue("ThreatenedAngry", true);
			 *
			 * where
			 *  <INSERT CODE HERE>
			 *  =
			 *   if (VeiledThreats.HandleThreatenFailed(agent, interactingAgent))
			 *   {
			 *     return;
			 *   }
			 */
			return new CodeReplacementPatch(
					expectedMatches: 1,
					prefixInstructionSequence: new List<CodeInstruction>
					{
							new CodeInstruction(OpCodes.Ldarg_1), // agent
							new CodeInstruction(OpCodes.Callvirt, playfieldObject_stopInteraction) // agent.StopInteraction()
					},
					insertInstructionSequence: new List<CodeInstruction>
					{
							new CodeInstruction(OpCodes.Ldarg_1), // agent
							new CodeInstruction(OpCodes.Ldarg_2), // interactingAgent
							new CodeInstruction(OpCodes.Call, handlerMethod), // HandleThreatenFailed(agent, interactingAgent)
							new CodeInstruction(OpCodes.Brtrue_S, continueLabel), // skip the return statement if HandleThreatenFailed returned false
							new CodeInstruction(OpCodes.Ret), // exit method early
							new CodeInstruction(OpCodes.Nop) { labels = new List<Label> { continueLabel } } // Nop instruction to attach continueLabel to
					},
					postfixInstructionSequence: new List<CodeInstruction>
					{
							new CodeInstruction(OpCodes.Ldarg_1), // agent
							new CodeInstruction(OpCodes.Ldstr, "ThreatenedAngry"), // "ThreatenedAngry"
							new CodeInstruction(OpCodes.Ldc_I4_1), // true
							new CodeInstruction(OpCodes.Callvirt, agent_sayDialogue), // agent.SayDialogue("ThreatenedAngry", true)
							new CodeInstruction(OpCodes.Pop)
					});
		}
	}
}