using System;
using BunnyMod.ObjectBehaviour.Controllers;
using HarmonyLib;

namespace BunnyMod.Patches.Objects
{
	[HarmonyPatch(declaringType: typeof(Fountain))]
	public static class P_Fountain
	{
		[HarmonyPostfix, HarmonyPatch(methodName: nameof(Fountain.SetVars), argumentTypes: new Type[] { })]
		private static void SetVars_Postfix(Fountain __instance)
		{
			FountainController.SetVars(__instance);
		}

		// TODO patch Fountain::Interact(Agent) - allow stealing from fountain after tossing a coin
	}
}