using HarmonyLib;

namespace BunnyMod.Patches
{
	[HarmonyPatch(declaringType: typeof(GameResources))]
	public static class P_GameResources
	{
		/*[HarmonyPostfix, HarmonyPatch(methodName: nameof(GameResources.SetupDics))]
		private static void SetupDics_Postfix(GameResources __instance)
		{
			__instance.facialHairDic.Add("TestFacialHair", __instance.facialHairList[10]);
			__instance.facialHairDic.Add("TestFacialHairSE", __instance.facialHairList[11]);
		}*/
	}
}