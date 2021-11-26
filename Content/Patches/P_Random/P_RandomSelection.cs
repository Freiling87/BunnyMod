using BepInEx.Logging;
using BunnyMod.Logging;
using HarmonyLib;

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
