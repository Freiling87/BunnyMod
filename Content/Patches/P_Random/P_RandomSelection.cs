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
	[HarmonyPatch(declaringType:typeof(RandomSelection))]
	public static class P_RandomSelection
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		public static GameController GC => GameController.gameController;

		[HarmonyPrefix,HarmonyPatch(methodName:nameof(RandomSelection.RandomSelect), argumentTypes:new[] { typeof(string), typeof(string) })]
		public static bool RandomSelect_Prefix(string rName, string rCategory, ref string __result)
		{
			if (rName.StartsWith("FireSpewerSpawnChance") && BMChallenges.IsChallengeFromListActive(cChallenge.WallsFlammable))
			{
				__result = "No";

				return false;
			}

			return true;
		}
	}
}
