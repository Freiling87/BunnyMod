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

namespace BunnyMod.Content.Patches
{
	[HarmonyPatch(declaringType: typeof(Agent))]
	public static class P_Agent
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		public static GameController GC => GameController.gameController;

		[HarmonyPrefix, HarmonyPatch(methodName:nameof(Agent.AgentOnCamera))]
		public static bool AgentOnCamera_Prefix(Agent __instance, ref bool __result) 
		{
			if (__instance.isPlayer != 0)
			{
				__instance.onCamera = true;

				__result = true;
				return false;
			}

			if (GC.serverPlayer)
			{
				if ((!__instance.brain.active && !__instance.oma._dead && !__instance.frozen && !__instance.wasPossessed2 && GC.serverPlayer &&
					GC.loadCompleteReally && !GC.loadLevel.recentlyStartedLevel && !__instance.oma.mindControlled) || __instance.objectAgent)
				{
					__instance.onCamera = false;

					__result = false;
					return false;
				}
			}
			else if ((!__instance.brain.active && !__instance.dead && !__instance.frozen && !__instance.wasPossessed2 && GC.serverPlayer &&
				GC.loadCompleteReally && !GC.loadLevel.recentlyStartedLevel) || __instance.objectAgent)
			{
				__instance.onCamera = false;

				__result = false;
				return false;
			}

			Vector2 v = __instance.tr.position;
			Vector2 vector = GC.playerAgent.agentCamera.originalCamera.WorldToViewportPoint(v);
			float x = vector.x / BMInterface.GetZoomLevel();
			float y = vector.y / BMInterface.GetZoomLevel();

			if (x > -0.1f && x < 1.1f &&
				y > -0.1f && y < 1.1f)
			{
				__instance.onCamera = true;

				__result = true;
				return false;
			}

			if (GC.coopMode || GC.fourPlayerMode)
			{
				vector = GC.playerAgent2.agentCamera.originalCamera.WorldToViewportPoint(v);
				x = vector.x / BMInterface.GetZoomLevel();
				y = vector.y / BMInterface.GetZoomLevel();

				if (x > -0.1f && x < 1.1f &&
					y > -0.1f && y < 1.1f)
				{
					__instance.onCamera = true;

					__result = true;
					return false;
				}

				if (GC.fourPlayerMode)
				{
					vector = GC.playerAgent3.agentCamera.originalCamera.WorldToViewportPoint(v);
					x = vector.x / BMInterface.GetZoomLevel();
					y = vector.y / BMInterface.GetZoomLevel();

					if (x > -0.1f && x < 1.1f &&
						y > -0.1f && y < 1.1f)
					{
						__instance.onCamera = true;

						__result = true;
						return false;
					}
					if (!GC.sessionDataBig.threePlayer)
					{
						vector = GC.playerAgent4.agentCamera.originalCamera.WorldToViewportPoint(v);
						x = vector.x / BMInterface.GetZoomLevel();
						y = vector.y / BMInterface.GetZoomLevel();

						if (x > -0.1f && x < 1.1f &&
							y > -0.1f && y < 1.1f)
						{
							__instance.onCamera = true;

							__result = true;
							return false;
						}
					}
				}
			}

			__instance.onCamera = false;

			__result = false;
			return false;
		}

		[HarmonyPostfix, HarmonyPatch(methodName: "Awake")]
		public static void Awake_Postfix(Agent __instance) 
		{
			__instance.wasOnCamera = false;
		}

		[HarmonyPrefix, HarmonyPatch(methodName: nameof(Agent.CanShakeDown))]
		private static bool CanShakeDown_Prefix(ref bool __result, Agent __instance)
		{
			logger.LogDebug($"called {nameof(CanShakeDown_Prefix)}");
			return Warlord.Agent_CanShakeDown_Prefix(ref __result, __instance);
		}

		[HarmonyPostfix, HarmonyPatch(methodName: nameof(Agent.FindOperatingTime), argumentTypes: new[] { typeof(PlayfieldObject), typeof(float) })]
		private static void FindOperatingTime_Postfix(PlayfieldObject operatingOnObject, float timeToUnlock, Agent __instance, ref float __result)
		{
			__result *= __instance.gc.selectedTimeScale;
		}

		[HarmonyPostfix, HarmonyPatch(methodName: nameof(Agent.SetupAgentStats), argumentTypes: new[] { typeof(string) })]
		private static void SetupAgentStats_Postfix(string transformationType, Agent __instance)
		{
			Reinforcements.SetupAgentStats(__instance);
			Reinforcements2.SetupAgentStats(__instance);

			if (__instance.agentName == cAgent.ResistanceSoldier)
			{
				__instance.SetStrength(2);
				__instance.SetEndurance(1);
				__instance.SetAccuracy(3);
				__instance.SetSpeed(3);

				__instance.modMeleeSkill = 2;
				__instance.modGunSkill = 2;
				__instance.modToughness = 2;
				__instance.modVigilant = 1;

				__instance.statusEffects.AddTrait("RegenerateHealthWhenLow");

				__instance.AddJob("Recruit", 0);
				__instance.AddDesire("Supplies");
				__instance.AddDesire("Technology");
				__instance.AddDesire("Weapons");

				__instance.agentHitboxScript.legsColor = new Color32(66, 101, 61, byte.MaxValue);

				__instance.agentCategories.Clear();
				__instance.agentCategories.Add("Guns");
				__instance.agentCategories.Add("Melee");
				__instance.agentCategories.Add("Defense");
				__instance.setInitialCategories = true;
			}
		}

		[HarmonyPostfix, HarmonyPatch(methodName: "Start")]
		private static void Start_Postfix(Agent __instance)
		{
			// Triggers at level gen. Not instantiator, may be reusing an existing Agent from last level. Saves overhead on procgen appearances/inv/etc.

			// VeiledThreats tracking bool
			VeiledThreats.ResetForAgent(__instance);
		}
	}
}