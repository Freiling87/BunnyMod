using System;
using HarmonyLib;

namespace BunnyMod.Content.Patches
{
	[HarmonyPatch(declaringType: typeof(PoolTable))]
	public static class PoolTable_Patches
	{
		[HarmonyPostfix, HarmonyPatch(methodName: nameof(PoolTable.SetVars), argumentTypes: new Type[] { })]
		private static void SetVars_Postfix(PoolTable __instance)
		{
			__instance.interactable = true;
		}
	}
}