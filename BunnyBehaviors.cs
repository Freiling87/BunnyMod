using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using BepInEx;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;

namespace BunnyMod
{
	/*See:
	 * int Agent.DoingMugging
	 * AgentInteractions.MugMoney - if player, else ObjectAction
	 * Relationships.ProtectOwnedLight
	 * AgentInteractions.ThreatenMoney
	 */

	public class BunnyBehaviors
	{
		#region Generic
		public void Awake()
		{
			Initialize_Names();

			BunnyHeader.MainInstance.PatchPrefix(typeof(LoadLevel), "SetupMore4", GetType(), "LoadLevel_SetupMore4", new Type[0] { });

		}
		public void FixedUpdate()
		{
		}
		public void Initialize_Names()
		{
			// Need Gangbanger_Mug and GangbangerB_Mug
		}
		#endregion

		#region LoadLevel
		public static void LoadLevel_SetupMore4(LoadLevel __instance, ref GameController ___gc) // Prefix
		{
			BunnyHeader.Log("LoadLevel.SetupMore4");

			List<int> gangsAssigned = new List<int>();

			foreach (Agent agent in ___gc.agentList)
			{
				BunnyHeader.Log("Detected " + agent.agentName.PadLeft(12) + " #" + ___gc.agentList.IndexOf(agent).ToString().PadRight(2) + ", member of gang #" + agent.gang + ", which has " + agent.gangMembers.Count + " members. He is/not a leader: " + agent.gangLeader);

				if ((agent.agentName == "Gangbanger" || agent.agentName == "GangbangerB") && agent.gang != 0 && agent.gangMembers.Count > 1 && !gangsAssigned.Contains(agent.gang))
				{
					BunnyHeader.Log("Added Leader to Gang " + agent.gang + ": " + agent.agentName.PadLeft(12) + " #" + ___gc.agentList.IndexOf(agent).ToString().PadRight(2));

					agent.gangLeader = true;
					gangsAssigned.Add(agent.gang);
				}
			}

			// Find a way to iterate through gangs instead?
		}
		#endregion
	}
}
