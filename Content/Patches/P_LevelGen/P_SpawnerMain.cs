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
using Light2D;

namespace BunnyMod.Content.Patches.P_LevelGen
{
	[HarmonyPatch(declaringType: typeof(SpawnerMain))]
	public static class P_SpawnerMain
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		public static GameController GC => GameController.gameController;

		// TODO: Transpiler
		[HarmonyPrefix,HarmonyPatch(methodName:nameof(SpawnerMain.GetLightColor), argumentTypes:new[] { typeof(string) })]
		public static bool GetLightColor_Prefix(string lightRealName, SpawnerMain __instance, ref Color __result, Color32 ___defaultColor) 
		{
			string challenge = BMChallenges.GetActiveChallengeFromList(cChallenge.AffectsLights);

			LightReal lightReal = new LightReal();

			if (challenge == null)
				return true;
			else if (challenge == cChallenge.DiscoCityDanceoff)
				lightReal.lightReal2Color = BMLevelGen.discoColors.RandomElement<Color32>();
			else if (challenge == cChallenge.GreenLiving)
				lightReal.lightReal2Color = BMLevelGen.homeColor;
			else if (challenge == cChallenge.Panoptikopolis)
				lightReal.lightReal2Color = BMLevelGen.whiteColor;

			__result = lightReal.lightReal2Color;
			return false;

			#region Vanilla

			if (lightRealName == "ArenaRingLight")
				lightReal.lightReal2Color = BMLevelGen.arenaRingColor;
			else if (lightRealName == "BankLight")
				lightReal.lightReal2Color = BMLevelGen.whiteColor;
			else if (lightRealName == "BlueLight")
				lightReal.lightReal2Color = BMLevelGen.blueColor;
			else if (lightRealName == "CyanGreenLight")
				lightReal.lightReal2Color = BMLevelGen.cyanGreenColor;
			else if (lightRealName == "CyanLight")
				lightReal.lightReal2Color = BMLevelGen.cyanColor;
			else if (lightRealName == "DefaultLight")
				lightReal.lightReal2Color = BMLevelGen.defaultColor;
			else if (lightRealName == "FarmLight")
				lightReal.lightReal2Color = BMLevelGen.homeColor;
			else if (lightRealName == "FireStationLight")
				lightReal.lightReal2Color = BMLevelGen.fireStationColor;
			else if (lightRealName == "GraveyardLight")
				lightReal.lightReal2Color = BMLevelGen.cyanColor;
			if (lightRealName == "GreenLight")
				lightReal.lightReal2Color = BMLevelGen.greenColor;
			else if (lightRealName == "HomeLight")
			{
				if (GC.levelTheme == 4)
					lightReal.lightReal2Color = BMLevelGen.homeColorUptown;
				else if (GC.levelTheme == 5)
					lightReal.lightReal2Color = BMLevelGen.homeColorMayorVillage;
				else
					lightReal.lightReal2Color = BMLevelGen.homeColor;
			}
			else if (lightRealName == "HospitalLight")
			{
				if (GC.levelTheme == 5)
					lightReal.lightReal2Color = BMLevelGen.homeColorMayorVillage;
				else
					lightReal.lightReal2Color = BMLevelGen.whiteColor;
			}
			else if (lightRealName == "KitchenLight")
				lightReal.lightReal2Color = BMLevelGen.whiteColor;
			if (lightRealName == "LabLight")
				lightReal.lightReal2Color = BMLevelGen.labColor;
			else if (lightRealName == "LakeLight")
				lightReal.lightReal2Color = BMLevelGen.lakeColor;
			else if (lightRealName == "LightBlueLight")
			{
				if (GC.levelTheme == 5)
					lightReal.lightReal2Color = BMLevelGen.lightBlueColorMayorVillage;
				else
					lightReal.lightReal2Color = BMLevelGen.lightBlueColor;
			}
			else if (lightRealName == "MallLight")
				lightReal.lightReal2Color = BMLevelGen.mallColor;
			else if (lightRealName == "OfficeLight")
				lightReal.lightReal2Color = BMLevelGen.whiteColor;
			else if (lightRealName == "PinkLight")
				lightReal.lightReal2Color = BMLevelGen.pinkColor;
			if (lightRealName == "PinkWhiteLight")
				lightReal.lightReal2Color = BMLevelGen.pinkWhiteColor;
			else if (lightRealName == "PoolLight")
			{
				if (GC.levelTheme == 5)
					lightReal.lightReal2Color = BMLevelGen.poolColorLighter;
				else
					lightReal.lightReal2Color = BMLevelGen.poolColor;
			}
			else if (lightRealName == "PrivateClubLight")
				lightReal.lightReal2Color = BMLevelGen.privateClubColor;
			else if (lightRealName == "PurpleLight")
				lightReal.lightReal2Color = BMLevelGen.purpleColor;
			if (lightRealName == "RedLight")
				lightReal.lightReal2Color = BMLevelGen.redColor;
			else if (lightRealName == "TVStationLight")
			{
				lightReal.lightReal2Color = BMLevelGen.mallColor;
			}
			else if (lightRealName == "WhiteLight")
				lightReal.lightReal2Color = BMLevelGen.whiteColor;
			else if (lightRealName == "ZooLight")
				lightReal.lightReal2Color = BMLevelGen.zooColor;

			#endregion

			__result = lightReal.lightReal2Color;

			return false;
		}
	}
}
