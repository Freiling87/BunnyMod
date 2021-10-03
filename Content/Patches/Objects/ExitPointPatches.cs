using HarmonyLib;

namespace BunnyMod.Content.Patches
{
	[HarmonyPatch(declaringType: typeof(ExitPoint))]
	public static class ExitPointPatches
	{
		[HarmonyPrefix, HarmonyPatch(methodName: nameof(ExitPoint.DetermineIfCanExit))]
		private static bool DetermineIfCanExit_Prefix(ref bool __result)
		{
			if (GameController.gameController.challenges.Contains(cChallenge.RushinRevolution))
			{
				__result = true;
				return false;
			}
			return true;
		}
	}
}