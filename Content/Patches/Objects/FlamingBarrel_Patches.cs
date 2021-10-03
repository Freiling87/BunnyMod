using System;
using BunnyMod.Content.ObjectBehaviour;
using HarmonyLib;

namespace BunnyMod.Content.Patches
{
	[HarmonyPatch(declaringType: typeof(FlamingBarrel))]
	public static class FlamingBarrel_Patches
	{
		[HarmonyPostfix, HarmonyPatch(methodName: nameof(FlamingBarrel.SetVars), argumentTypes: new Type[] { })]
		private static void SetVars_Postfix(FlamingBarrel __instance)
		{
			FlamingBarrelController.SetVars(__instance);
		}
	}
}