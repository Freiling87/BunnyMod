using RogueLibsCore;
using System;
using System.Collections.Generic;
using UnityEngine;
using BunnyMod;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;
using BunnyMod.Content.Logging;
using BepInEx.Logging;

namespace BunnyMod.Content.Custom
{
	public static class C_Challenges
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		private static GameController GC => GameController.gameController;

		public static string GetActiveChallengeFromList(List<string> challengeList)
		{
			foreach (string mutator in challengeList)
				if (GC.challenges.Contains(mutator))
					return mutator;

			return null;
		}

		public static bool IsChallengeFromListActive(List<string> challengeList)
		{
			foreach (string mutator in challengeList)
				if (GC.challenges.Contains(mutator))
					return true;

			return false;
		}
	}
}
