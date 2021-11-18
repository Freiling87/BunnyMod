using System;
using BunnyMod.ObjectBehaviour.Controllers;
using HarmonyLib;

namespace BunnyMod.Patches.Objects
{
	[HarmonyPatch(declaringType: typeof(Stove))]
	public static class Stove_Patches
	{
		[HarmonyPostfix, HarmonyPatch(methodName: nameof(Stove.SetVars), argumentTypes: new Type[] { })]
		private static void SetVars_Postfix(Stove __instance)
		{
			StoveController.SetVars(__instance);
		}
	}
}