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
			CustomName hobo_Mug = RogueLibs.CreateCustomName("Hobo_Mug", "Dialogue", new CustomNameInfo("Can you spare a few bucks, friendo?"));
			CustomName gangbanger_Mug = RogueLibs.CreateCustomName("GangBanger_Mug", "Dialogue", new CustomNameInfo("The cash, hand it over!"));
			CustomName gangbangerB_Mug = RogueLibs.CreateCustomName("GangBangerB_Mug", "Dialogue", new CustomNameInfo("Empty your pockets, bitch!"));
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
					agent.gangLeader = true;
					gangsAssigned.Add(agent.gang);

					BunnyHeader.Log("Added Leader to Gang " + agent.gang + ": " + agent.agentName.PadLeft(12) + " #" + ___gc.agentList.IndexOf(agent).ToString().PadRight(2));
				}
				else if (agent.agentName == "Hobo")
				{
					Agent.gangCount++;
					agent.gang = Agent.gangCount;
					agent.gangLeader = true;
					gangsAssigned.Add(agent.gang);

					BunnyHeader.Log("Added Hobo to Gang " + agent.gang + ": " + agent.agentName.PadLeft(12) + " #" + ___gc.agentList.IndexOf(agent).ToString().PadRight(2));
				}

			}
		}
		#endregion
	}
}
