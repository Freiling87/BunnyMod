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

namespace BunnyMod.Content.Patches.P_LevelGen
{
	[HarmonyPatch(declaringType:typeof(SpawnerObject))]
	public static class P_SpawnerObject
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		public static GameController GC => GameController.gameController;

		[HarmonyPrefix, HarmonyPatch(methodName:nameof(SpawnerObject.spawn), argumentTypes:new[] { typeof(string) })]
		public static bool spawn_Prefix(ref string objectRealName)
		{
			logger.LogDebug("SpawnerObject_spawn:");
			logger.LogDebug("\tobjectRealName = '" + objectRealName + "'");

			if (BMChallenges.IsChallengeFromListActive(cChallenge.WallsFlammable) && objectRealName == vObject.FireSpewer)
				objectRealName = vObject.SecurityCam;

			return true;
		}
	}
}
