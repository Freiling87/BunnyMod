using System;
using BunnyMod.ObjectBehaviour.Controllers;
using HarmonyLib;

namespace BunnyMod.Patches.Objects
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