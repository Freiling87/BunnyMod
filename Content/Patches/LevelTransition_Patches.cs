using System;
using BunnyMod.Content.ObjectBehaviour;
using HarmonyLib;

namespace BunnyMod.Content.Patches
{
	[HarmonyPatch(declaringType: typeof(LevelTransition))]
	public static class LevelTransition_Patches
	{
		[HarmonyPrefix, HarmonyPatch(methodName: nameof(LevelTransition.ChangeLevel), argumentTypes: new Type[] { })]
		private static void ChangeLevel_Prefix()
		{
			ObjectControllerManager.TriggerChangeLevelEvent();
		}
	}
}