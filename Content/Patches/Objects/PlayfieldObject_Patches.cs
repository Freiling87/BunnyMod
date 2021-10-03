using System;
using BunnyMod.Content.ObjectBehaviour;
using HarmonyLib;

namespace BunnyMod.Content.Patches
{
	[HarmonyPatch(declaringType: typeof(PlayfieldObject))]
	public static class PlayfieldObject_Patches
	{
		[HarmonyPrefix, HarmonyPatch(methodName: nameof(PlayfieldObject.playerHasUsableItem), argumentTypes: new[] { typeof(InvItem) })]
		private static bool PlayerHasUsableItem_Prefix(PlayfieldObject __instance, InvItem myItem, ref bool __result)
		{
			ObjectControllerManager.GetController(__instance)?.HandlePlayerHasUsableItem(__instance, myItem, ref __result);
			return !__result; // stop further execution if the controller determined that the item is usable
		}

		[HarmonyPostfix, HarmonyPatch(methodName: nameof(PlayfieldObject.FinishedOperating), argumentTypes: new Type[] { })]
		private static void FinishedOperating_Postfix(PlayfieldObject __instance)
		{
			ObjectControllerManager.GetController(__instance)?.HandleFinishedOperating(__instance);
		}
	}
}