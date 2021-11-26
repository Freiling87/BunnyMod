using System;
using BunnyMod.ObjectBehaviour.Controllers;
using HarmonyLib;

namespace BunnyMod.Patches.Objects
{
	[HarmonyPatch(declaringType: typeof(VendorCart))]
	public static class P_VendorCart
	{
		[HarmonyPostfix, HarmonyPatch(methodName: nameof(VendorCart.SetVars), argumentTypes: new Type[] { })]
		private static void SetVars_Postfix(VendorCart __instance)
		{
			VendorCartController.HandleSetVars(__instance);
		}
	}
}