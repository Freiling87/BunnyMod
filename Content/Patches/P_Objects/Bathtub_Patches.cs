using System;
using HarmonyLib;

namespace BunnyMod.Patches.Objects
{
	[HarmonyPatch(declaringType: typeof(Bathtub))]
	public static class Bathtub_Patches
	{
		[HarmonyPostfix, HarmonyPatch(methodName: nameof(Bathtub.SetVars), argumentTypes: new Type[] { })]
		private static void SetVars_Postfix(Bathtub __instance)
		{
			__instance.interactable = true;

			//TODO: Closed Bath Curtain sprite?
			// See Generator.Start() for how to set animation sprites. Maybe just toggle sprite when used/unused.
		}
	}
}