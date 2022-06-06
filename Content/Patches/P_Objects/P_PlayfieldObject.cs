using BepInEx.Logging;
using BunnyMod.Content.Logging;
using BunnyMod.Content.Traits;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;
using Random = UnityEngine.Random;
using System.Collections.Generic;

namespace BunnyMod.Content.Patches
{
	[HarmonyPatch(declaringType: typeof(PlayfieldObject))]
	public static class P_PlayfieldObject
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		private static GameController GC => GameController.gameController;

		[HarmonyPostfix, HarmonyPatch(methodName: nameof(PlayfieldObject.determineMoneyCost), argumentTypes: new[] { typeof(int), typeof(string) })]
		private static void determineMoneyCost_Postfix(int moneyAmt, string transactionType, PlayfieldObject __instance, ref int __result)
		{                // ↑ [sic]
			logger.LogDebug("PlayfieldObject_determineMoneyCost: transactionType = " + transactionType + "; PFO = " + __instance.name);

			Agent agent = (Agent)__instance;
			float num = __result;
			int levelMultiplier = Mathf.Clamp(GC.sessionDataBig.curLevelEndless, 1, 15);
			int gangsizeMultiplier = agent.gangMembers.Count;

			logger.LogDebug("PlayfieldObject_DetermineMoneyCost: num = " + num + "; LevelMult = " + levelMultiplier + "; gangsizeMult = " + gangsizeMultiplier);

			if (transactionType == "Mug_Gangbanger")
				num = (float)(levelMultiplier * 10 + gangsizeMultiplier * 15);
			else if (transactionType == "Hobo_GiveMoney1")
				num = 05f;
			else if (transactionType == "Hobo_GiveMoney2")
				num = 20f;
			else if (transactionType == "Hobo_GiveMoney3")
				num = 50f;
			else
				logger.LogDebug("Bad string passed to PlayfieldObject_determineMoneyCost");

			__result = (int)num;

			logger.LogDebug("PlayfieldObject_determineMoneyCost: result = " + __result);
		}
	}
}
