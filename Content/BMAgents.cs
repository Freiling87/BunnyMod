using RogueLibsCore;
using System.Collections.Generic;
using BepInEx.Logging;
using BunnyMod.Content.Logging;
using UnityEngine;
using Random = UnityEngine.Random;
using BunnyMod.Content.Traits;

namespace BunnyMod.Content
{
	public static class BMAgents
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();

		private static GameController GC => GameController.gameController;

		// TODO I'd like to move this entire region somewhere else, but haven't made my mind up yet where it should go
		#region Custom

		// TODO unused.
		public static void AnnoyWitnesses(Agent perp, Agent victim)
		{
			foreach (Agent bystander in GC.agentList)
			{
				if (Vector2.Distance(bystander.tr.position, perp.tr.position) < bystander.LOSRange / perp.hardToSeeFromDistance &&
						bystander != perp && bystander != victim && !bystander.zombified && !bystander.ghost && !bystander.oma.hidden &&
						(!perp.aboveTheLaw || !bystander.enforcer || victim.enforcer) &&
						perp.prisoner == bystander.prisoner && !perp.invisible && !victim.noEnforcerAlert)
				{
					string perpRel = bystander.relationships.GetRel(perp);
					string victimRel = bystander.relationships.GetRel(victim);

					if (victimRel != nameof(relStatus.Hostile) && victimRel != nameof(relStatus.Annoyed))
					{
						if (perpRel == nameof(relStatus.Neutral) || perpRel == nameof(relStatus.Friendly))
						{
							if (bystander.relationships.GetRelationship(perp).hasLOS)
							{
								relStatus victimRel2 = bystander.relationships.GetRelCode(victim);
								relStatus perpRel2 = bystander.relationships.GetRelCode(perp);

								if ((victimRel2 == relStatus.Aligned && perpRel2 != relStatus.Aligned) ||
										(victimRel2 == relStatus.Loyal && perpRel2 != relStatus.Aligned && perpRel2 != relStatus.Loyal))
								{
									bystander.sawBiteList.Add(victim);
									bystander.relationships.SetRelHate(perp, 5);
								}
								else
								{
									bystander.sawBiteList.Add(victim);
									bystander.relationships.SetStrikes(perp, 2);
								}
							}
						}
						else if (perpRel == nameof(relStatus.Annoyed) && bystander.relationships.GetRelationship(perp).hasLOS)
						{
							bystander.sawBiteList.Add(victim);
							bystander.relationships.SetRelHate(perp, 5);
						}
					}
				}
			}
		}

		public static void AnnoyWitnessesVictimless(Agent perp)
		{
			foreach (Agent bystander in GC.agentList)
			{
				if (Vector2.Distance(bystander.tr.position, perp.tr.position) < bystander.LOSRange / perp.hardToSeeFromDistance &&
						bystander != perp && !bystander.zombified && !bystander.ghost && !bystander.oma.hidden &&
						(!perp.aboveTheLaw || !bystander.enforcer) &&
						perp.prisoner == bystander.prisoner && !perp.invisible)
				{
					string perpRel = bystander.relationships.GetRel(perp);

					if (perpRel == nameof(relStatus.Neutral) || perpRel == nameof(relStatus.Friendly))
					{
						if (bystander.relationships.GetRelationship(perp).hasLOS)
						{
							relStatus perpRel2 = bystander.relationships.GetRelCode(perp);

							// TODO something isn't right here, condition always evaluates to true
							if (perpRel2 != relStatus.Aligned || perpRel2 != relStatus.Loyal)
								bystander.relationships.SetStrikes(perp, 2);
						}
					}
					else if (perpRel == nameof(relStatus.Annoyed) && bystander.relationships.GetRelationship(perp).hasLOS)
					{
						bystander.relationships.SetRelHate(perp, 5);
					}
				}
			}
		}

		public static void SpawnEmployees(Agent playerAgent, int numberToSpawn, string agentType, LoadLevel __instance, string relationship) // Non-Patch
		{
			logger.LogDebug("SpawnEmployees");

			for (int i = 0; i < numberToSpawn; i++)
				GC.spawnerMain.SpawnAgent(GC.tileInfo.FindLocationNearLocation(playerAgent.tr.position, playerAgent, 0.32f, 0.96f, true, false), playerAgent,
						agentType, "FriendPhone", playerAgent);
		}

		public static void SpawnRoamerSquad(Agent playerAgent, int numberToSpawn, string agentType, LoadLevel __instance, string relationship,
				int splitIntoGroupSize) // Non-Patch
		{
			logger.LogDebug("LoadLevel_SpawnRoamerSquad");

			List<Agent> spawnedAgentList = new List<Agent>();
			//playerAgent.gangStalking = Agent.gangCount;
			Vector2 pos = Vector2.zero;

			numberToSpawn = (int) ((float) numberToSpawn * __instance.levelSizeModifier);

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
					agent.movement.RotateToAngleTransform((float) Random.Range(0, 360));
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

		#endregion

		// TODO Fatass doesn't exist anymore (for now ?), move to CustomTrait::OnAdded
		public static void StatusEffects_AddTrait(string traitName, bool isStarting, bool justRefresh, StatusEffects __instance) // Postfix
		{
			Agent agent = __instance.agent;

			if (traitName == BMTraits_Old.Fatass)
			{
				agent.SetEndurance(agent.enduranceStatMod + 1);
				agent.SetSpeed(agent.speedStatMod - 1);
			}
		}

		// TODO Fatass doesn't exist anymore (for now ?), move to CustomTrait::OnRemoved
		public static void StatusEffects_RemoveTrait(string traitName, bool onlyLocal, StatusEffects __instance) // Postfix
		{
			Agent agent = __instance.agent;

			if (traitName == BMTraits_Old.Fatass)
			{
				//TODO: CharacterCreation.CreatePointTallyText() for stat mods
				agent.SetEndurance(agent.enduranceStatMod - 1);
				agent.SetSpeed(agent.speedStatMod + 1);
			}
		}
	}
}