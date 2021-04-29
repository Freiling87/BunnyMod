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
			AgentHealthBar_00();
			ObjectInfo_00();
			OperatingBar_00();
		}
		#endregion

		#region Custom
		public static void SetTraitZoom() =>
			GC.cameraScript.zoomLevel = GetZoomLevelFromTrait();
		public static float GetZoomLevelFromTrait()
		{
			if (BMTraits.IsPlayerTraitActive(cTrait.EagleEyes))
				return 0.75f;
			else if (BMTraits.IsPlayerTraitActive(cTrait.EagleEyes_2))
				return 0.50f;
			else if (BMTraits.IsPlayerTraitActive(cTrait.Myopic))
				return 1.50f;
			else if (BMTraits.IsPlayerTraitActive(cTrait.Myopic2))
				return 2.00f;

			return 1f;
		}
		#endregion

		#region AgentHealthBar
		public void AgentHealthBar_00()
		{
			Postfix(typeof(AgentHealthBar), "Awake", GetType(), "AgentHealthBar_Awake", new Type[0] { });
		}
		public static void AgentHealthBar_Awake(AgentHealthBar __instance) => // Postfix
			__instance.curScale = 0.72f / GetZoomLevelFromTrait();
		#endregion
		#region ObjectInfo 
		// The Arrows pointing to quest targets, etc.
		public void ObjectInfo_00()
		{
			Postfix(typeof(ObjectInfo), "OnEnable", GetType(), "ObjectInfo_OnEnable", new Type[0] { });
		}
		public static void ObjectInfo_OnEnable(ObjectInfo __instance) => // Postfix
			__instance.curScale = 0.72f / GetZoomLevelFromTrait();
		#endregion
		#region OperatingBar
		public void OperatingBar_00()
		{
			Postfix(typeof(OperatingBar), "Start", GetType(), "OperatingBar_Start", new Type[0] { });
		}
		public static void OperatingBar_Start(OperatingBar __instance) => // Postfix
			__instance.curScale = 0.72f / GetZoomLevelFromTrait();
		#endregion
		#region PlayerControl
		public void PlayerControl_00()
		{
		}
		#endregion
	}
}
