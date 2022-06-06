using BepInEx.Logging;
using BunnyMod.Content.Logging;
using BunnyMod.Content.Traits;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;
using Random = UnityEngine.Random;

namespace BunnyMod.Content.Patches
{
	[HarmonyPatch(declaringType:typeof(InvDatabase))]
	class P_InvDatabase
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		public static GameController GC => GameController.gameController;

		[HarmonyPrefix, HarmonyPatch(methodName:nameof(InvDatabase.SubtractFromItemCount), argumentTypes: new[] { typeof(int), typeof(int), typeof(bool) })]
		public static bool SubtractFromItemCount_c_Prefix(int slotNum, ref int amount, bool toolbarMove, InvDatabase __instance) 
		{
			logger.LogDebug("InvDatabase_SubtractFromItemCount_c:");
			logger.LogDebug("\tslotNum = " + slotNum);
			logger.LogDebug("\tamount = " + amount);
			logger.LogDebug("\ttoolbarMove = " + toolbarMove);

			if (vItem.tools.Contains(__instance.InvItemList[slotNum].invItemName))
			{
				if (__instance.agent.statusEffects.hasTrait(cTrait.TamperTantrum_2))
					amount = 0;
				else if (__instance.agent.statusEffects.hasTrait(cTrait.TamperTantrum))
					amount /= 2;
			}
			return true;
		} // TODO: is the ref int here correct?

		[HarmonyPrefix, HarmonyPatch(methodName:nameof(InvDatabase.SubtractFromItemCount), argumentTypes: new[] { typeof(InvItem), typeof(int), typeof(bool) })]
		public static bool SubtractFromItemCount_d_Prefix(InvItem invItem, ref int amount, bool toolbarMove, InvDatabase __instance) 
		{
			logger.LogDebug("InvDatabase_SubtractFromItemCount_d:");
			logger.LogDebug("\tInvItem = " + invItem.invItemName);
			logger.LogDebug("\tamount = " + amount);
			logger.LogDebug("\ttoolbarMove = " + toolbarMove);

			if (vItem.tools.Contains(invItem.invItemName))
			{
				if (__instance.agent.statusEffects.hasTrait(cTrait.TamperTantrum_2))
					amount = 0;
				else if (__instance.agent.statusEffects.hasTrait(cTrait.TamperTantrum))
					amount /= 2;
			}
			return true;
		}
	}
}
