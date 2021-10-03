using BunnyMod.Content.ObjectBehaviour;
using HarmonyLib;
using UnityEngine;

namespace BunnyMod.Content.Patches
{
	[HarmonyPatch(declaringType: typeof(Hole))]
	public static class Hole_Patches
	{
		[HarmonyPrefix, HarmonyPatch(methodName: nameof(Hole.EnterRange), argumentTypes: new[] { typeof(GameObject) })]
		private static bool EnterRange_Prefix(Hole __instance, GameObject myObject)
		{
			return HoleController.Hole_EnterRange_Prefix(__instance, myObject);
		}
	}
}