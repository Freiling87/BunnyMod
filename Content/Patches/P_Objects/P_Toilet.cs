using System;
using BunnyMod.ObjectBehaviour.Controllers;
using HarmonyLib;

namespace BunnyMod.Patches.Objects
{
	[HarmonyPatch(declaringType: typeof(Toilet))]
	public static class P_Toilet
	{
		[HarmonyPrefix, HarmonyPatch(methodName: nameof(Toilet.FlushYourself), argumentTypes: new Type[] { })]
		private static bool FlushYourself_Prefix(Toilet __instance)
		{
			return ToiletController.FlushYourself_Prefix(__instance);
		}

		[HarmonyPostfix, HarmonyPatch(methodName: nameof(Toilet.DetermineButtons), argumentTypes: new Type[] { })]
		private static void DetermineButtons_Postfix(Toilet __instance)
		{
			ToiletController.HandleDetermineButtons_Postfix(__instance);
		}

		[HarmonyPrefix, HarmonyPatch(methodName: nameof(Toilet.PressedButton), argumentTypes: new[] { typeof(string), typeof(int) })]
		private static bool PressedButton_Prefix(Toilet __instance, string buttonText, int buttonPrice)
		{
			return ToiletController.HandlePressedButton_Prefix(__instance, buttonPrice);
		}
	}
}