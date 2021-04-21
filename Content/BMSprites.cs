using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using BepInEx;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;

namespace BunnyMod.Content
{
	class BMSprites
	{
		public static GameController GC => GameController.gameController;
		public static bool Prefix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) => BMHeader.MainInstance.PatchPrefix(type, methodName, patchType, patchMethodName, types);
		public static bool Postfix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) => BMHeader.MainInstance.PatchPostfix(type, methodName, patchType, patchMethodName, types);
		public static void BMLog(string logMessage) => BMHeader.Log(logMessage);

		public void Awake()
		{
			CharacterCreation_00();
			CharacterSelect_00();
			GameResources_00();
		}
		public void FixedUpdate()
		{
		}
		public void Initialize_Names()
		{
		}
		public static void Initialize_Sprites()
		{
		}

		#region CharacterCreation
		public void CharacterCreation_00()
		{
			Postfix(typeof(CharacterCreation), "Awake", GetType(), "CharacterCreation_Awake", new Type[0] { });
		}
		public static void CharacterCreation_Awake(CharacterCreation __instance) // Postfix
		{
			__instance.facialHairTypes.Add("TestFacialHair");
			__instance.facialHairTypes.Add("TestFacialHair");
		}
		#endregion
		#region CharacterSelect
		public void CharacterSelect_00()
		{

		}
		public static void CharacterSelect_FakeStart(CharacterSelect __instance) // Postfix
		{
			__instance.facialHairTypes.Add("TestFacialHair");
		}
		#endregion
		#region GameResources
		public void GameResources_00()
		{
			Postfix(typeof(GameResources), "SetupDics", GetType(), "GameResources_SetupDics", new Type[0] { });
		}
		public static void GameResources_SetupDics(GameResources __instance) // Postfix
		{
			__instance.facialHairDic.Add("TestFacialHair", __instance.facialHairList[10]);
			__instance.facialHairDic.Add("TestFacialHairSE", __instance.facialHairList[11]);

		}
		#endregion
	}
}
