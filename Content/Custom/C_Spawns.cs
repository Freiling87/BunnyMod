using RogueLibsCore;
using System.Collections.Generic;
using BepInEx.Logging;
using BunnyMod.Content.Logging;
using UnityEngine;
using Random = UnityEngine.Random;
using BunnyMod.Content.Traits;

namespace BunnyMod.Content.Custom
{
	public static class C_Spawns
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		private static GameController GC => GameController.gameController;

		public static void SpawnEmployees(Agent playerAgent, int numberToSpawn, string agentType, LoadLevel __instance, string relationship)
		{
			logger.LogDebug("SpawnEmployees");

			for (int i = 0; i < numberToSpawn; i++)
				GC.spawnerMain.SpawnAgent(GC.tileInfo.FindLocationNearLocation(playerAgent.tr.position, playerAgent, 0.32f, 0.96f, true, false), playerAgent,
						agentType, "FriendPhone", playerAgent);
		}

		public static void SpawnRoamerSquad(Agent playerAgent, int numberToSpawn, string agentType, LoadLevel __instance, string relationship, int splitIntoGroupSize)
		{
			logger.LogDebug("LoadLevel_SpawnRoamerSquad");

			List<Agent> spawnedAgentList = new List<Agent>();
			//playerAgent.gangStalking = Agent.gangCount;
			Vector2 pos = Vector2.zero;

			numberToSpawn = (int)((float)numberToSpawn * __instance.levelSizeModifier);

			for (int i = 0; i < numberToSpawn; i++)
			{
				if (i % splitIntoGroupSize == 0)
					Agent.gangCount++; // Splits spawn into groups

				Vector2 vector = Vector2.zero;
				int attempts = 0;

				if (i == 0)
				{
					do
					{
						vector = GC.tileInfo.FindRandLocationGeneral(0.32f);
						attempts++;
					} while ((vector == Vector2.zero || Vector2.Distance(vector, GC.playerAgent.tr.position) < 20f) && attempts < 300);

					pos = vector;
				}
				else
					vector = GC.tileInfo.FindLocationNearLocation(pos, null, 0.32f, 1.28f, true, true);

				if (vector != Vector2.zero && attempts < 300)
				{
					Agent agent = GC.spawnerMain.SpawnAgent(vector, null, agentType);
					agent.movement.RotateToAngleTransform((float)Random.Range(0, 360));
					agent.gang = Agent.gangCount;
					agent.modLeashes = 0;

					if (agentType == vAgent.Ghost)
						agent.alwaysRun = true;

					agent.wontFlee = true;
					agent.agentActive = true;
					//agent.statusEffects.AddStatusEffect("InvisiblePermanent");
					agent.oma.mustBeGuilty = true;
					spawnedAgentList.Add(agent);

					// Align agents in group to each other
					if (spawnedAgentList.Count > 1)
						for (int j = 0; j < spawnedAgentList.Count; j++)
							if (spawnedAgentList[j] != agent)
							{
								agent.relationships.SetRelInitial(spawnedAgentList[j], nameof(relStatus.Aligned));
								spawnedAgentList[j].relationships.SetRelInitial(agent, nameof(relStatus.Aligned));
							}

					agent.relationships.SetRel(playerAgent, relationship);
					playerAgent.relationships.SetRel(agent, relationship);

					switch (relationship.ToString())
					{
						case nameof(relStatus.Annoyed):
							agent.relationships.SetRelHate(playerAgent, 1);
							playerAgent.relationships.SetRelHate(agent, 1);

							break;
						case nameof(relStatus.Hostile):
							agent.relationships.SetRelHate(playerAgent, 5);
							playerAgent.relationships.SetRelHate(agent, 5);

							break;
					}

					if (agentType == vAgent.ResistanceLeader)
					{
						if (playerAgent.HasTrait<Reinforcements2>())
						{
							Reinforcements2.EquipReinforcement(agent);
						}
						else if (playerAgent.HasTrait<Reinforcements>())
						{
							Reinforcements.EquipReinforcement(agent);
						}
					}

					agent.SetDefaultGoal(vAgentGoal.WanderLevel);
				}
			}
		}
	}
}
