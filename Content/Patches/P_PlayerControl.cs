using BepInEx.Logging;
using BunnyMod.Content.Logging;
using BunnyMod.Content.Traits;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;
using Random = UnityEngine.Random;


namespace BunnyMod.Content.Patches
{
	[HarmonyPatch(declaringType:typeof(PlayerControl))]
	public static class P_PlayerControl
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		public static GameController GC => GameController.gameController;

		[HarmonyPostfix, HarmonyPatch(methodName:"Update")]
		public static void PlayerControl_Update(PlayerControl __instance)
		{
			GC.cameraScript.zoomLevel = BMInterface.GetZoomLevel();
			__instance.myCamera.zoomLevel = BMInterface.GetZoomLevel();
		}
	}
}
