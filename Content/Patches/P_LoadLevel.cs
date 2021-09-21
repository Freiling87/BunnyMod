using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using RogueLibsCore;

namespace BunnyMod.Content.Patches
{
	[HarmonyPatch(declaringType: typeof(LoadLevel))]
	public static class P_LoadLevel
	{
		private static readonly string loggerName = $"BunnyMod_{MethodBase.GetCurrentMethod().DeclaringType?.Name}";
		private static ManualLogSource Logger => _logger ?? (_logger = BepInEx.Logging.Logger.CreateLogSource(loggerName));
		private static ManualLogSource _logger;

		public static GameController GC => GameController.gameController;

		[HarmonyPrefix, HarmonyPatch(methodName: nameof(LoadLevel.SetupMore4))] 
		private static bool SetupMore4_Prefix()
		{
			return true; // Deactivated, feature on hold

			Logger.LogDebug("LoadLevel.SetupMore4");

			List<int> gangsAssigned = new List<int>();

			foreach (Agent agent in GC.agentList)
			{
				Logger.LogDebug("Detected " + agent.agentName.PadLeft(12) + " #" + GC.agentList.IndexOf(agent).ToString().PadRight(2) + ", member of gang #" +
					agent.gang + ", which has " + agent.gangMembers.Count + " members. He is/not a leader: " + agent.gangLeader);

				// Assign to Gangs to allow Begging/Mugging behaviors
				if ((agent.agentName == vAgent.Blahd || agent.agentName == vAgent.Crepe) && agent.gang != 0 && agent.gangMembers.Count > 1 &&
					!gangsAssigned.Contains(agent.gang))
				{
					agent.gangLeader = true;
					gangsAssigned.Add(agent.gang);

					Logger.LogDebug("Added Leader to Gang " + agent.gang + ": " + agent.agentName.PadLeft(12) + " #" +
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

						Logger.LogDebug("Added Hobo to Gang " + agent.gang + ": " + agent.agentName.PadLeft(12) + " #" +
							GC.agentList.IndexOf(agent).ToString().PadRight(2));
					}
				}
			}

			return true;
		}
	}
}
