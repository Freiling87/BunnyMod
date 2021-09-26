using HarmonyLib;

namespace BunnyMod.Content.Patches
{
	[HarmonyPatch(declaringType: typeof(Quests))]
	public static class P_Quests
	{
		[HarmonyPrefix, HarmonyPatch(methodName: nameof(Quests.GetLevelExitChallengeItem))]
		private static bool GetLevelExitChallengeItem_Prefix(Quests __instance, ref InvItem __result)
		{
			GameController gc = GameController.gameController;
			string itemName = null;
			if (gc.challenges.Contains(cChallenge.UnpaidInternship))
			{
				itemName = gc.Choose(vItem.Banana, vItem.HamSandwich);
			}else if (gc.challenges.Contains(cChallenge.DoublePlyRewards))
			{
				itemName = gc.Choose(vItem.FreeItemVoucher, vItem.HiringVoucher);
			}

			if (itemName != null)
			{
				InvItem invItem = new InvItem()
				{
						invItemName = itemName,
						invItemCount = 1
				};
				invItem.SetupDetails(false);
				__instance.questItemsGiven.Add(itemName);
				__result = invItem;
				return false;
			}

			return true;
		}
	}
}