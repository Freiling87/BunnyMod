﻿using HarmonyLib;

namespace BunnyMod.Content.Patches
{
	[HarmonyPatch(declaringType: typeof(GameResources))]
	public static class GameResourcesPatches
	{
		/*[HarmonyPostfix, HarmonyPatch(methodName: nameof(GameResources.SetupDics))]
		private static void SetupDics_Postfix(GameResources __instance)
		{
			__instance.facialHairDic.Add("TestFacialHair", __instance.facialHairList[10]);
			__instance.facialHairDic.Add("TestFacialHairSE", __instance.facialHairList[11]);
		}*/
	}
}