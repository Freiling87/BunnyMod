using System;
using HarmonyLib;

namespace BunnyMod.Patches.Objects
{
	[HarmonyPatch(declaringType: typeof(Plant))]
	public static class Plant_Patches
	{
		[HarmonyPostfix, HarmonyPatch(methodName: nameof(Plant.SetVars), argumentTypes: new Type[] { })]
		private static void SetVars_Postfix(Plant __instance)
		{
			__instance.interactable = true;
		}
	}
}