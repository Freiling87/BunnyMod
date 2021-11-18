using System;
using BunnyMod.ObjectBehaviour;
using HarmonyLib;

namespace BunnyMod.Patches.Objects
{
	[HarmonyPatch(declaringType: typeof(PlayfieldObject))]
	public static class PlayfieldObject_Patches
	{
		[HarmonyPrefix, HarmonyPatch(methodName: nameof(PlayfieldObject.playerHasUsableItem), argumentTypes: new[] { typeof(InvItem) })]
		private static bool PlayerHasUsableItem_Prefix(PlayfieldObject __instance, InvItem myItem, ref bool __result)
		{
			ObjectControllerManager.GetObjectController(__instance)?.HandlePlayerHasUsableItem(__instance, myItem, ref __result);
			return !__result; // stop further execution if the controller determined that the item is usable
		}

		[HarmonyPostfix, HarmonyPatch(methodName: nameof(PlayfieldObject.FinishedOperating), argumentTypes: new Type[] { })]
		private static void FinishedOperating_Postfix(PlayfieldObject __instance)
		{
			ObjectControllerManager.GetObjectController(__instance)?.HandleFinishedOperating(__instance);
		}
	}
}