using BepInEx.Logging;
using BunnyMod.Content.Logging;
using BunnyMod.Content.Traits;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;
using Random = UnityEngine.Random;
using System.Collections.Generic;

namespace BunnyMod.Content.Patches.P_Items
{
	[HarmonyPatch(declaringType: typeof(ItemFunctions))]
	public static class P_ItemFunctions
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		public static GameController GC => GameController.gameController;

		[HarmonyPostfix, HarmonyPatch(methodName: nameof(ItemFunctions.DetermineHealthChange), argumentTypes: new[] { typeof(InvItem), typeof(Agent) })]
		public static void DetermineHealthChange_Postfix(InvItem item, Agent agent, ref int __result) 
		{
			List<string> cats = item.Categories;
			StatusEffects traits = agent.statusEffects;
			if
			(
					(cats.Contains("Alcohol") && (traits.hasTrait(cTrait.FriendOfBill) || traits.hasTrait(cTrait.Teetotaller))) ||
					(cats.Contains("Drugs") && (traits.hasTrait(cTrait.DAREdevil) || traits.hasTrait(cTrait.Teetotaller))) ||
					(cats.Contains("Vegetarian") && traits.hasTrait(cTrait.Carnivore)) ||
					(cats.Contains("NonVegetarian") && traits.hasTrait(cTrait.Vegetarian))
			)
				__result = 0;
			if (traits.hasTrait(cTrait.Fatass))
				__result = (int)((float)__result * 1.5f);
		}

		[HarmonyPrefix, HarmonyPatch(methodName: nameof(ItemFunctions.UseItem), argumentTypes: new[] { typeof(InvItem), typeof(Agent) })]
		public static bool UseItem_Prefix(InvItem item, Agent agent, ItemFunctions __instance) 
		{
			if (item.itemType == "Consumable")
			{
				if (vItem.alcohol.Contains(item.invItemName) &&
						((agent.statusEffects.hasTrait(cTrait.FriendOfBill) || agent.statusEffects.hasTrait(cTrait.Teetotaller))))
				{
					BMHeaderTools.SayDialogue(agent, cDialogue.CantDrinkAlcohol, vNameType.Dialogue);
					goto terminus;
				}
				else if (vItem.drugs.Contains(item.invItemName) &&
						(agent.statusEffects.hasTrait(cTrait.DAREdevil) || agent.statusEffects.hasTrait(cTrait.Teetotaller)))
				{
					BMHeaderTools.SayDialogue(agent, cDialogue.CantUseDrug, vNameType.Dialogue);
					goto terminus;
				}
				else if (vItem.nonVegetarian.Contains(item.invItemName) && agent.statusEffects.hasTrait(cTrait.Vegetarian))
				{
					BMHeaderTools.SayDialogue(agent, cDialogue.CantEatMeat, vNameType.Dialogue);
					goto terminus;
				}
				else if (vItem.vegetarian.Contains(item.invItemName) && agent.statusEffects.hasTrait(cTrait.Carnivore))
				{
					BMHeaderTools.SayDialogue(agent, cDialogue.CantEatNonvegetarian, vNameType.Dialogue);
					goto terminus;
				}
			}
			else
			{
				if (vItem.loud.Contains(item.invItemName) && agent.statusEffects.hasTrait(cTrait.AfraidOfLoudNoises))
				{
					BMHeaderTools.SayDialogue(agent, cDialogue.CantUseLoud, "Dialogue");
					goto terminus;
				}

				if (vItem.piercing.Contains(item.invItemName) && agent.statusEffects.hasTrait(cTrait.DrawNoBlood))
				{
					BMHeaderTools.SayDialogue(agent, cDialogue.CantUseSharp_2, vNameType.Dialogue);
					goto terminus;
				}
			}

			return true;

		terminus:

			GC.audioHandler.Play(agent, "CantDo");

			return false;
		}
	}
}
