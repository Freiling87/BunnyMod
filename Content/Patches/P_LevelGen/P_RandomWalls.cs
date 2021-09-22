using BepInEx.Logging;
using BunnyMod.Content.Logging;
using BunnyMod.Content.Traits;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;
using Random = UnityEngine.Random;
using System.Collections;
using System.Reflection;
using System;

namespace BunnyMod.Content.Patches
{
	[HarmonyPatch(declaringType:typeof(RandomWalls))]
	public static class P_RandomWalls
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		public static GameController GC => GameController.gameController;

		[HarmonyPrefix,HarmonyPatch(methodName:nameof(RandomWalls.fillWalls))]
		public static bool fillWalls_Prefix() 
		{
			string wallType = null;

			if (BMLevelGen.IsWallModActive())
				wallType = BMLevelGen.GetWallTypeFromMutator();

			if (wallType == null)
				return true;
			else
			{
				RandomSelection component = GameObject.Find("ScriptObject").GetComponent<RandomSelection>();
				RandomList rList;

				rList = component.CreateRandomList(vWallGroup.Normal, "Walls", "Wall");
				component.CreateRandomElement(rList, wallType, 3);

				rList = component.CreateRandomList(vWallGroup.Weak, "Walls", "Wall");
				component.CreateRandomElement(rList, wallType, 3);

				rList = component.CreateRandomList(vWallGroup.Strong, "Walls", "Wall");
				component.CreateRandomElement(rList, wallType, 3);

				rList = component.CreateRandomList(vWallGroup.Hideout, "Walls", "Wall");
				component.CreateRandomElement(rList, wallType, 3);
			}

			return false;
		}
	}
}
