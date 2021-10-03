using System;
using BunnyMod.Content.ObjectBehaviour;
using HarmonyLib;

namespace BunnyMod.Content.Patches
{
	[HarmonyPatch(declaringType: typeof(Stove))]
	public static class Stove_Patches
	{
		[HarmonyPostfix, HarmonyPatch(methodName: nameof(Stove.SetVars), argumentTypes: new Type[] { })]
		private static void SetVars_Postfix(Stove __instance)
		{
			StoveController.SetVars(__instance);
		}

		[HarmonyPostfix, HarmonyPatch(methodName: nameof(Stove.RevertAllVars), argumentTypes: new Type[] { })]
		private static void RevertAllVars_Postfix(Stove __instance)
		{
			StoveController.RevertAllVars(__instance);
		}
	}
}