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
	[HarmonyPatch(declaringType:typeof(RandomOther))]
	public static class P_RandomOther
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		public static GameController GC => GameController.gameController;

		[HarmonyPostfix,HarmonyPatch(methodName:nameof(RandomOther.fillOther))]
		public static void RandomOther_fillOther(ref RandomSelection ___component, ref RandomList ___rList) // Postfix
		{
			logger.LogDebug("RandomOther_fillOther");
			// Pay special attention to this. If this is only called at Game Start, you need to find another place post-mutator to mod this.

			if (GC.challenges.Contains(cChallenge.ShantyTown) || GC.challenges.Contains(cChallenge.GreenLiving))
			{
				___rList = ___component.CreateRandomList("FireSpewerSpawnChance1", "Others", "Other");
				___component.CreateRandomElement(___rList, "No", 5);

				___rList = ___component.CreateRandomList("FireSpewerSpawnChance2", "Others", "Other");
				___component.CreateRandomElement(___rList, "No", 5);

				___rList = ___component.CreateRandomList("FireSpewerSpawnChance3", "Others", "Other");
				___component.CreateRandomElement(___rList, "No", 5);

				___rList = ___component.CreateRandomList("FireSpewerSpawnChance4", "Others", "Other");
				___component.CreateRandomElement(___rList, "No", 5);

				___rList = ___component.CreateRandomList("FireSpewerSpawnChance5", "Others", "Other");
				___component.CreateRandomElement(___rList, "No", 5);
			}
		}
	}
}
