using BunnyMod.ObjectBehaviour.Controllers;
using HarmonyLib;
using UnityEngine;

namespace BunnyMod.Patches.Objects
{
	[HarmonyPatch(declaringType: typeof(Hole))]
	public static class P_Hole
	{
		[HarmonyPrefix, HarmonyPatch(methodName: nameof(Hole.EnterRange), argumentTypes: new[] { typeof(GameObject) })]
		private static bool EnterRange_Prefix(Hole __instance, GameObject myObject)
		{
			return HoleController.Hole_EnterRange_Prefix(__instance, myObject);
		}
	}
}