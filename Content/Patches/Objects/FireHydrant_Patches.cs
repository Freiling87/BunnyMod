using System;
using BunnyMod.Content.ObjectBehaviour;
using HarmonyLib;

namespace BunnyMod.Content.Patches
{
	[HarmonyPatch(declaringType: typeof(FireHydrant))]
	public static class FireHydrant_Patches
	{
		[HarmonyPrefix, HarmonyPatch(methodName: nameof(FireHydrant.PressedButton), argumentTypes: new[] { typeof(string), typeof(int) })]
		private static bool PressedButton_Prefix(FireHydrant __instance, string buttonText, int buttonPrice)
		{
			return FireHydrantController.FireHydrant_PressedButton_Prefix(__instance, buttonText, buttonPrice);
		}

		[HarmonyPostfix, HarmonyPatch(methodName: nameof(FireHydrant.DetermineButtons), argumentTypes: new Type[] { })]
		private static void DetermineButtons_Postfix(FireHydrant __instance)
		{
			FireHydrantController.FireHydrant_DetermineButtons_Postfix(__instance);
		}
	}
}