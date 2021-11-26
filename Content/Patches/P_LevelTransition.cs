using System;
using BunnyMod.ObjectBehaviour;
using HarmonyLib;

namespace BunnyMod.Patches
{
	[HarmonyPatch(declaringType: typeof(LevelTransition))]
	public static class P_LevelTransition
	{
		[HarmonyPrefix, HarmonyPatch(methodName: nameof(LevelTransition.ChangeLevel), argumentTypes: new Type[] { })]
		private static void ChangeLevel_Prefix()
		{
			ObjectControllerManager.OnLevelChange();
		}
	}
}