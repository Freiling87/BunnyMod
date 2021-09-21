using BepInEx.Logging;
using BunnyMod.Content.Extensions;
using BunnyMod.Content.Logging;
using BunnyMod.Content.Traits;
using Google2u;
using HarmonyLib;
using UnityEngine;

namespace BunnyMod.Content.Patches
{
	[HarmonyPatch(declaringType: typeof(Relationships))]
	public static class RelationshipsPatches
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();

		[HarmonyPostfix,
		 HarmonyPatch(methodName: nameof(Relationships.EnforcerAlert),
				 argumentTypes: new[] { typeof(Agent), typeof(float), typeof(Vector2), typeof(int), typeof(Agent) })]
		private static void EnforcerAlert_Postfix(Agent criminal, float noiseDist, Vector2 noisePos, int numStrikes, Agent victim, Relationships __instance)
		{
			// TODO move logic to PoliceState challenge
			if (GameController.gameController.challenges.Contains(cChallenge.PoliceState))
			{
				if (__instance.GetRel(criminal) == nameof(relStatus.Hostile))
				{
					criminal.statusEffects.AddTrait(vTrait.Wanted);
				}
			}
		}

		[HarmonyPostfix, HarmonyPatch(methodName: nameof(Relationships.SetupRelationshipOriginal), argumentTypes: new[] { typeof(Agent) })]
		private static void SetupRelationshipOriginal_Postfix(Agent otherAgent, Relationships __instance, ref Agent ___agent)
		{
			// Relationships defines how ___agent feels about otherAgent
			// Don't delete this comment, it gets confusing-er-than-shit

			if (GameController.gameController.levelType == nameof(InterfaceNameDB.rowIds.HomeBase))
			{
				__instance.SetRelInitial(otherAgent, nameof(relStatus.Aligned));
				otherAgent.relationships.SetRelInitial(___agent, nameof(relStatus.Aligned));
				return;
			}

			string currentRelationship = __instance.GetRel(otherAgent);
			if (currentRelationship == null)
			{
				logger.LogWarning("SetupRelationshipOriginal - currentRelationship was null!");
				return;
			}

			relStatus? newRelationship;
			if (___agent.IsAgent(AgentNameDB.rowIds.ResistanceLeader) && otherAgent.isPlayer > 0)
			{
				newRelationship = relStatus.Aligned;
			}
			else
			{
				// Order matters here, the first non-Null relStatus will be used.
				newRelationship =
						MobDebt.SetupInitialRelationship(___agent, otherAgent, currentRelationship)
						?? Domineering.SetupInitialRelationship(___agent, otherAgent, currentRelationship)
						?? Domineering2.SetupInitialRelationship(___agent, otherAgent, currentRelationship)
						?? Polarizing.SetupInitialRelationship(___agent, otherAgent, currentRelationship)
						?? Polarizing2.SetupInitialRelationship(___agent, otherAgent, currentRelationship)
						?? BootLicker.SetupInitialRelationship(___agent, otherAgent, currentRelationship)
						?? Priors.SetupInitialRelationship(___agent, otherAgent, currentRelationship)
						?? GenerallyUnpleasant.SetupInitialRelationship(___agent, otherAgent, currentRelationship)
						?? ObjectivelyUnpleasant.SetupInitialRelationship(___agent, otherAgent, currentRelationship);
			}

			if (newRelationship != null)
			{
				string relationshipString = newRelationship.Value.ToString();
				__instance.SetRelInitial(otherAgent, relationshipString);
				otherAgent.relationships.SetRelInitial(___agent, relationshipString);
				if (newRelationship.Value == relStatus.Annoyed)
				{
					otherAgent.relationships.SetStrikes(___agent, 2);
					__instance.SetStrikes(otherAgent, 2);
				}
				else if (newRelationship.Value == relStatus.Hostile)
				{
					otherAgent.relationships.SetStrikes(___agent, 5);
					__instance.SetStrikes(otherAgent, 5);
				}
			}
		}

		// TODO OwnCheck Prefix - PoliceState sets all Objects NoStrikesIfDestroyed to false
	}
}