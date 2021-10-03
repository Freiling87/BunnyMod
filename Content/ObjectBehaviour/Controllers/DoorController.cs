using System.Collections.Generic;
using System.Linq;
using BunnyMod.Content.Traits;
using JetBrains.Annotations;
using RogueLibsCore;
using UnityEngine;

namespace BunnyMod.Content.ObjectBehaviour
{
	public static class DoorController
	{
		[UsedImplicitly]
		public static bool ShouldSkipOwnCheck(Agent agent)
		{
			return agent != null && agent.HasTrait<StealthBastardDeluxe>();
		}
		
		public static void HandleSpawnNoise(SpawnerMain spawnerMain,
				Vector3 noisePos, float noiseVolume, PlayfieldObject sourceObject, string noiseType, Agent searchAgentSource)
		{
			if (searchAgentSource != null && searchAgentSource.HasTrait<StealthBastardDeluxe>())
			{
				return; // StealthBastardDeluxe mitigates Door-Noises
			}
			spawnerMain.SpawnNoise(noisePos, noiseVolume, sourceObject, noiseType, searchAgentSource);
		}

		public static void HandleFreePrisonerPointsIfNotDead(Agent agent, IEnumerable<Agent> freedAgents)
		{
			if (agent.HasTrait<VeryHardOnYourself>())
			{
				int deadPrisoners = freedAgents.Count(agent2 => agent2.dead && !agent2.teleporting);
				agent.skillPoints.AddPoints(cSkillPoints.FreePrisonerFailure, deadPrisoners);
			}
		}
	}
}