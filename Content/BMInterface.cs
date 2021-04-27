using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using BepInEx;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;

using Random = UnityEngine.Random;
using UnityEngine.Networking;
using System.Linq;


namespace BunnyMod.Content
{
	public class BMInterface
	{
		public static GameController GC => GameController.gameController;
		public static bool Prefix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) => BMHeader.MainInstance.PatchPrefix(type, methodName, patchType, patchMethodName, types);
		public static bool Postfix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) => BMHeader.MainInstance.PatchPostfix(type, methodName, patchType, patchMethodName, types);
		public static void BMLog(string logMessage) => BMHeader.Log(logMessage);

		#region Main
		public void Awake()
		{
			PlayerControl_00();
		}
		#endregion

		#region Custom
		public static void ResetCameras()
		{
			float zoomLevel = GC.cameraScript.zoomLevel;

			if (BMTraits.IsPlayerTraitActive("EagleEyes"))
				zoomLevel = 0.75f;
			else if (BMTraits.IsPlayerTraitActive("EagleEyes_2"))
				zoomLevel = 0.50f;
			else if (BMTraits.IsPlayerTraitActive("Myopic"))
				zoomLevel = 1.50f;
			else if (BMTraits.IsPlayerTraitActive("Myopic_2"))
				zoomLevel = 2.00f;

			GC.cameraScript.zoomLevel = zoomLevel;
		}
		#endregion

		#region PlayerControl
		public void PlayerControl_00()
		{
			Postfix(typeof(PlayerControl), "Update", GetType(), "PlayerControl_Update", new Type[0] { });
		}
		public static void PlayerControl_Update() // Postfix
		{
			ResetCameras();
		}
		#endregion
	}
}
