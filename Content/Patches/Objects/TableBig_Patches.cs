using System;
using HarmonyLib;

namespace BunnyMod.Patches.Objects
{
	[HarmonyPatch(declaringType: typeof(TableBig))]
	public static class TableBig_Patches
	{
		[HarmonyPostfix, HarmonyPatch(methodName: nameof(TableBig.SetVars), argumentTypes: new Type[] { })]
		private static void SetVars_Postfix(TableBig __instance)
		{
			__instance.interactable = true;
		}
	}
}