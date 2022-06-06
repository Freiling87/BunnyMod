using RogueLibsCore;
using System.Collections.Generic;
using BepInEx.Logging;
using BunnyMod.Content.Logging;
using UnityEngine;
using Random = UnityEngine.Random;
using BunnyMod.Content.Traits;

namespace BunnyMod.Content.Custom
{
	public static class C_Combat
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		private static GameController GC => GameController.gameController;

		// TODO: Set this somewhere so it doesn't waste processor
		public static float GlobalKnockbackFactor() =>
			GC.challenges.Contains(cChallenge.BoringPhysics) ? 0.10f :
			GC.challenges.Contains(cChallenge.SaveTheWalls) ? 0.50f :
			GC.challenges.Contains(vChallenge.BigKnockback) ? 1.50f :
			GC.challenges.Contains(cChallenge.WallWallopWorld) ? 5.00f :
			1.00f;
	}
}
