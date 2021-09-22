using BepInEx.Logging;
using BunnyMod.Content.Logging;
using BunnyMod.Content.Traits;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;
using Random = UnityEngine.Random;

namespace BunnyMod.Content.Patches
{
	[HarmonyPatch(declaringType: typeof(Movement))]
	public static class P_Movement
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		public static GameController GC => GameController.gameController;

		[HarmonyPostfix, HarmonyPatch(methodName: nameof(Movement.FindKnockBackStrength), argumentTypes:new [] {typeof(float)})]
		public static void FindKnockBackStrength_Postfix(float strength, ref float __result)
		{
			if (BMChallenges.IsChallengeFromListActive(cChallenge.Knockback))
				__result *= BMCombat.GetGlobalKnockBackMultiplier();
		}
	}
}