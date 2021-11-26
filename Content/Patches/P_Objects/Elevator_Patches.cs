using System;
using BunnyMod.ObjectBehaviour.Controllers;
using HarmonyLib;

namespace BunnyMod.Patches.Objects
{
	[HarmonyPatch(declaringType: typeof(Elevator))]
	public static class Elevator_Patches
	{
		[HarmonyPostfix, HarmonyPatch(methodName: nameof(Elevator.DetermineButtons), argumentTypes: new Type[] { })]
		private static void DetermineButtons_Postfix(Elevator __instance)
		{
			ElevatorController.Elevator_DetermineButtons_Postfix(__instance);
		}
	}
}