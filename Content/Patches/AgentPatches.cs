using BepInEx.Logging;
using BunnyMod.Content.Traits;
using HarmonyLib;
using UnityEngine;

namespace BunnyMod.Content.Patches
{
	[HarmonyPatch(declaringType: typeof(Agent))]
	public static class AgentPatches
	{
		private static readonly string loggerName = $"BunnyMod_{nameof(AgentPatches)}";
		private static ManualLogSource Logger => _logger ?? (_logger = BepInEx.Logging.Logger.CreateLogSource(loggerName));
		private static ManualLogSource _logger;

		[HarmonyPrefix, HarmonyPatch(methodName: nameof(Agent.CanShakeDown))]
		private static bool CanShakeDown_Prefix(ref bool __result, Agent __instance)
		{
			Logger.LogDebug($"called {nameof(CanShakeDown_Prefix)}");
			return Warlord.Agent_CanShakeDown_Prefix(ref __result, __instance);
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