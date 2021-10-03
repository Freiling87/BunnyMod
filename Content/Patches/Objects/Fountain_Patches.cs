using System;
using BunnyMod.Content.ObjectBehaviour;
using HarmonyLib;

namespace BunnyMod.Content.Patches
{
	[HarmonyPatch(declaringType: typeof(Fountain))]
	public static class Fountain_Patches
	{
		[HarmonyPostfix, HarmonyPatch(methodName: nameof(Fountain.SetVars), argumentTypes: new Type[] { })]
		private static void SetVars_Postfix(Fountain __instance)
		{
			FountainController.SetVars(__instance);
		}

		// TODO patch Fountain::Interact(Agent) - allow stealing from fountain after tossing a coin
	}
}