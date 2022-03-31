using BepInEx.Logging;
using BunnyMod.Content.Logging;
using BunnyMod.Content.Traits;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;
using Random = UnityEngine.Random;
using System.Collections;
using System.Reflection;
using System;
using System.Collections.Generic;
using Light2D;
using System.Linq;
using Object = UnityEngine.Object;
using BunnyMod.Content.Custom;
using BunnyMod.Content.Extensions;

namespace BunnyMod.Content.Patches
{
	[HarmonyPatch(declaringType: typeof(LoadLevel))]
	public static class P_LoadLevel
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		public static GameController GC => GameController.gameController;

		/// <summary>
		/// Roamer Traits
		/// </summary>
		/// <param name="__result"></param>
		/// <param name="__instance"></param>
		/// <returns></returns>
		[HarmonyPostfix, HarmonyPatch(methodName: "SetupMore3_3")]
		public static IEnumerator SetupMore3_3_Postfix(IEnumerator __result, LoadLevel __instance) 
		{
			logger.LogDebug("LoadLevel_SetupMore3_3_Postfix");

			while (__result.MoveNext())
				yield return __result.Current;

			int level = GC.sessionDataBig.curLevel;

			for (int agentSearch = 0; agentSearch < GC.agentList.Count; agentSearch++)
			{
				Agent agent = GC.agentList[agentSearch];

				if (agent.isPlayer > 0)
				{
					if (agent.HasTrait<Haunted>())
						C_Spawns.SpawnRoamerSquad(agent, 4, vAgent.Ghost, __instance, nameof(relStatus.Hostile), 1);

					if (agent.HasTrait<BodyGuarded>())
						C_Spawns.SpawnEmployees(agent, 2, vAgent.Goon, __instance, nameof(relStatus.Aligned));
					else if (agent.HasTrait<BodyGuarded2>())
						C_Spawns.SpawnEmployees(agent, 2, vAgent.Supergoon, __instance, nameof(relStatus.Aligned));

					if (level >= 10)
						if (agent.HasTrait<MobDebt>())
							C_Spawns.SpawnRoamerSquad(agent, (int)((float)level * 1.66f), vAgent.Mobster, __instance, nameof(relStatus.Hostile), 4);

					// To SORCE
					if (level >= 13)
						if (GC.challenges.Contains(cChallenge.LitterallyTheWorst) || GC.challenges.Contains(cChallenge.FloralerFlora))
							for (int i = 0; i <= level - 11; i++)
								GC.spawnerMain.SpawnButlerBot();

					if (agent.HasTrait<MookMasher>())
						C_Spawns.SpawnRoamerSquad(agent, level * 2, vAgent.Goon, __instance, nameof(relStatus.Hostile), 4);

					if (agent.HasTrait<Reinforcements>())
						C_Spawns.SpawnRoamerSquad(agent, 4, vAgent.ResistanceLeader, __instance, nameof(relStatus.Aligned), 1);
					else if (agent.HasTrait<Reinforcements2>())
						C_Spawns.SpawnRoamerSquad(agent, 8, vAgent.ResistanceLeader, __instance, nameof(relStatus.Aligned), 1);
				}
			}
		}

		/// <summary>
		/// Hey, You! Approach behaviors
		/// </summary>
		/// <returns></returns>
		[HarmonyPrefix, HarmonyPatch(methodName: nameof(LoadLevel.SetupMore4))] 
		private static bool SetupMore4_Prefix()
		{
			return true; // Deactivated, feature on hold

			logger.LogDebug("LoadLevel.SetupMore4");

			List<int> gangsAssigned = new List<int>();

			foreach (Agent agent in GC.agentList)
			{
				logger.LogDebug("Detected " + agent.agentName.PadLeft(12) + " #" + GC.agentList.IndexOf(agent).ToString().PadRight(2) + ", member of gang #" +
					agent.gang + ", which has " + agent.gangMembers.Count + " members. He is/not a leader: " + agent.gangLeader);

				// Assign to Gangs to allow Begging/Mugging behaviors
				if ((agent.agentName == vAgent.Blahd || agent.agentName == vAgent.Crepe) && agent.gang != 0 && agent.gangMembers.Count > 1 &&
					!gangsAssigned.Contains(agent.gang))
				{
					agent.gangLeader = true;
					gangsAssigned.Add(agent.gang);

					logger.LogDebug("Added Leader to Gang " + agent.gang + ": " + agent.agentName.PadLeft(12) + " #" +
						GC.agentList.IndexOf(agent).ToString().PadRight(2));
				}
				else if (agent.agentName == vAgent.SlumDweller) 
				{
					if (GC.percentChance(33))
					{
						Agent.gangCount++;
						agent.gang = Agent.gangCount;
						agent.gangMembers.Add(agent);
						agent.gangLeader = true;
						gangsAssigned.Add(agent.gang);

						logger.LogDebug("Added Hobo to Gang " + agent.gang + ": " + agent.agentName.PadLeft(12) + " #" +
							GC.agentList.IndexOf(agent).ToString().PadRight(2));
					}
				}
			}

			return true;
		}
	}
}