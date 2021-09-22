using BepInEx.Logging;
using BunnyMod.Content.Logging;
using BunnyMod.Content.Traits;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;
using Random = UnityEngine.Random;

namespace BunnyMod.Content.Patches.P_Items
{
	[HarmonyPatch(declaringType:typeof(InvItem))]
	public static class P_InvItem
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		public static GameController GC => GameController.gameController;

		[HarmonyPostfix, HarmonyPatch(methodName:nameof(InvItem.SetupDetails), argumentTypes: new[] { typeof(bool) })]
		public static void SetupDetails_Postfix(bool notNew, InvItem __instance) 
		{
			string name = __instance.invItemName;

			if (__instance.Categories.Count == 0)
				__instance.Categories.Add("NullCatcher");

			if (__instance.Categories.Contains("Alcohol")) { }

			if (__instance.Categories.Contains("Drugs")) { }

			if (__instance.Categories.Contains("Food"))
			{
				if (vItem.nonVegetarian.Contains(name))
					__instance.Categories.Add("NonVegetarian");
				else if (vItem.vegetarian.Contains(name))
					__instance.Categories.Add("Vegetarian");
			}

			if (__instance.Categories.Contains("Weapons"))
			{
				if (vItem.blunt.Contains(name))
					__instance.Categories.Add("Blunt");
				if (vItem.explosive.Contains(name))
					__instance.Categories.Add("Explosive");
				if (vItem.loud.Contains(name) && !__instance.contents.Contains("Silencer"))
					__instance.Categories.Add("Loud");
				if (vItem.piercing.Contains(name))
					__instance.Categories.Add("Piercing");
			}

			return;
		}

		[HarmonyPrefix, HarmonyPatch(methodName:nameof(InvItem.UseItem))]
		public static bool UseItem_Prefix(InvItem __instance) 
		{
			Agent agent = __instance.agent;
			List<string> cats = __instance.Categories;
			bool cantDoFlag = false;

			if (cats.Contains("Alcohol") && (agent.statusEffects.hasTrait(cTrait.FriendOfBill) || agent.statusEffects.hasTrait(cTrait.Teetotaller)))
			{
				BMHeaderTools.SayDialogue(agent, cDialogue.CantDrinkAlcohol, vNameType.Dialogue);
				cantDoFlag = true;
			}
			else if (cats.Contains("Drugs") && (agent.statusEffects.hasTrait(cTrait.DAREdevil) || agent.statusEffects.hasTrait(cTrait.Teetotaller)))
			{
				BMHeaderTools.SayDialogue(agent, cDialogue.CantUseDrug, vNameType.Dialogue);
				cantDoFlag = true;
			}
			else if (cats.Contains("NonVegetarian") && agent.statusEffects.hasTrait(cTrait.Vegetarian))
			{
				BMHeaderTools.SayDialogue(agent, cDialogue.CantEatMeat, vNameType.Dialogue);
				cantDoFlag = true;
			}
			else if (cats.Contains("Vegetarian") && agent.statusEffects.hasTrait(cTrait.Carnivore))
			{
				BMHeaderTools.SayDialogue(agent, cDialogue.CantEatNonvegetarian, vNameType.Dialogue);
				cantDoFlag = true;
			}
			else if (cats.Contains("Loud") && agent.statusEffects.hasTrait(cTrait.AfraidOfLoudNoises))
			{
				BMHeaderTools.SayDialogue(agent, cDialogue.CantUseLoud, "Dialogue");
				cantDoFlag = true;
			}
			else if (cats.Contains("Piercing") && agent.statusEffects.hasTrait(cTrait.DrawNoBlood))
			{
				BMHeaderTools.SayDialogue(agent, cDialogue.CantUseSharp_2, vNameType.Dialogue);
				cantDoFlag = true;
			}

			if (cantDoFlag)
			{
				GC.audioHandler.Play(agent, "CantDo");

				return false;
			}

			return true;
		}
	}
}
