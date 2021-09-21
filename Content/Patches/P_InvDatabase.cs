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

		[HarmonyPrefix, HarmonyPatch(methodName:nameof(InvDatabase.ChooseWeapon), argumentTypes:new[] { typeof(bool) })]
		public static bool ChooseWeapon_Prefix(bool noGuns, InvDatabase __instance) 
		{
			if (__instance.agent.isPlayer != 0 && BMTraitController.DoesPlayerHaveTraitFromList(__instance.agent, cTrait.LimitWeapons))
			{
				Agent agent = __instance.agent;

				if (__instance.agent.isPlayer > 0)
					__instance.UpdateWeaponList();

				InvItem invItem = null;
				InvItem invItem2 = null;
				float num = 0f;
				float num2 = 0f;
				bool flag = false;
				bool noWeapons = noGuns;
				bool flag3 = false;
				bool noLoud = false;

				if (agent.statusEffects.hasTrait(vTrait.StubbyFingers))
					noGuns = true;

				if (agent.challengedToFight > 0)
					noGuns = true;

				if (agent.statusEffects.hasTrait(vTrait.Pacifist))
				{
					noWeapons = true;
					flag3 = true;
				}

				if (agent.statusEffects.hasTrait(vTrait.SausageFingers))
				{
					noGuns = true;
					flag = true;
				}

				if (agent.statusEffects.hasTrait(vTrait.Harmless))
				{
					noGuns = true;
					flag = true;
				}

				if (agent.statusEffects.hasTrait(vTrait.NearHarmless))
				{
					noGuns = true;
					flag = true;
				}

				if (agent.statusEffects.hasTrait(cTrait.AfraidOfLoudNoises))
					noLoud = true;

				foreach (InvItem itemCandidate in __instance.InvItemList)
				{
					logger.LogDebug("\titem: " + itemCandidate.invItemName);
					//logger.LogDebug("\tLoud: " + vItem.loud.Contains(itemCandidate.invItemName)); // Works
					//foreach (string a in itemCandidate.contents)
					//    logger.LogDebug("\tContains: " + itemCandidate.contents[itemCandidate.contents.IndexOf(a)]); // Works

					if (itemCandidate.itemType == "WeaponProjectile" &&
							itemCandidate.hierarchy2 > num &&
							(!noGuns || itemCandidate.Categories.Contains("NotRealWeapons")) &&
							(!noWeapons || itemCandidate.Categories.Contains("NonViolent")) &&
							(!noLoud || !(vItem.loud.Contains(itemCandidate.invItemName) || itemCandidate.contents.Contains(vItem.Silencer))) &&
							itemCandidate.invItemCount > 0 &&
							(!itemCandidate.dontAutomaticallySelect || __instance.agent.isPlayer <= 0) &&
							(__instance.agent.isPlayer <= 0 || !(itemCandidate.itemType != __instance.lastEquippedWeaponType)) &&
							(!itemCandidate.dontSelectNPC || __instance.agent.isPlayer != 0))
					{
						invItem = itemCandidate;
						num = itemCandidate.hierarchy2;
					}

					if (itemCandidate.itemType == "WeaponMelee" && itemCandidate.hierarchy2 > num2 &&
							!flag &&
							(!flag3 || itemCandidate.Categories.Contains("NonViolent")) &&
							(!noLoud || (vItem.loud.Contains(itemCandidate.invItemName) && !itemCandidate.contents.Contains(vItem.Silencer))) &&
							itemCandidate.invItemCount > 0 &&
							(!itemCandidate.dontAutomaticallySelect || __instance.agent.isPlayer <= 0) &&
							(!itemCandidate.dontSelectNPC || __instance.agent.isPlayer != 0))
					{
						invItem2 = itemCandidate;
						num2 = itemCandidate.hierarchy2;
					}
				}

				if (invItem != null &&
						(__instance.agent.isPlayer <= 0 || !(__instance.equippedWeapon.itemType == "WeaponMelee") ||
								__instance.equippedWeapon == __instance.fist) && !__instance.equippedWeapon.Categories.Contains("NonViolent"))
					__instance.EquipWeapon(invItem);
				else if (invItem2 != null)
					__instance.EquipWeapon(invItem2);
				else
					__instance.EquipWeapon(__instance.fist);

				return false;
			}

			return true;
		}

		[HarmonyPrefix, HarmonyPatch(methodName:nameof(InvDatabase.DetermineIfCanUseWeapon), argumentTypes:new[] { typeof(InvItem) })]
		public static bool DetermineIfCanUseWeapon_Prefix(InvItem item, InvDatabase __instance, ref bool __result) 
		{
			//TODO: Verify non-equipped items like Time Bomb.

			//logger.LogDebug("InvDatabase_DetermineIfCanUseWeapon");
			//logger.LogDebug("\tItem: " + item.invItemName);
			//logger.LogDebug("\tSharp: " + vItem.piercing.Contains(item.invItemName));
			//logger.LogDebug("\tLoud: " + vItem.loud.Contains(item.invItemName));

			Agent agent = __instance.agent;
			bool fist = (item.invItemName == vItem.Fist);

			__result = (
							(!(item.itemType == "WeaponProjectile") || !agent.statusEffects.hasTrait(vTrait.StubbyFingers) ||
									item.Categories.Contains("NotRealWeapons")) &&
							(fist || item.Categories.Contains("NonViolent") || item.Categories.Contains("NotRealWeapons") ||
									!agent.statusEffects.hasTrait(vTrait.Pacifist)) &&
							(fist || !agent.statusEffects.hasTrait(vTrait.SausageFingers)) &&
							(fist || !agent.statusEffects.hasTrait(vTrait.Harmless)) &&
							(fist || !agent.statusEffects.hasTrait(vTrait.NearHarmless)) &&
							(!vItem.piercing.Contains(item.invItemName) || !agent.statusEffects.hasTrait(cTrait.DrawNoBlood)) &&
							(!vItem.loud.Contains(item.invItemName) || item.contents.Contains(vItem.Silencer) ||
									!agent.statusEffects.hasTrait(cTrait.AfraidOfLoudNoises))) ||
					(agent.isPlayer != 0 && !agent.localPlayer);

			return false;
		}

		[HarmonyPrefix, HarmonyPatch(methodName:nameof(InvDatabase.EquipArmor), argumentTypes:new[] { typeof(InvItem), typeof(bool) })]
		public static bool EquipArmor_Prefix(InvItem item, bool sfx, InvDatabase __instance) 
		{
			if (item.isArmor && __instance.agent.statusEffects.hasTrait(cTrait.Fatass))
			{
				BMHeaderTools.SayDialogue(__instance.agent, cDialogue.CantWearArmor, vNameType.Dialogue);
				GC.audioHandler.Play(__instance.agent, "CantDo");

				return false;
			}

			return true;
		}

		[HarmonyPrefix, HarmonyPatch(methodName:nameof(InvDatabase.EquipArmorHead), argumentTypes:new[] { typeof(InvItem), typeof(bool) })]
		public static bool EquipArmorHead_Prefix(InvItem item, bool sfx, InvDatabase __instance) 
		{
			if (item.isArmorHead && item != null && __instance.agent.statusEffects.hasTrait(cTrait.FatHead))
			{
				BMHeaderTools.SayDialogue(__instance.agent, cDialogue.CantWearHeadgear, vNameType.Dialogue);
				GC.audioHandler.Play(__instance.agent, "CantDo");

				return false;
			}

			return true;
		}

		[HarmonyPrefix, HarmonyPatch(methodName:nameof(InvDatabase.EquipWeapon), argumentTypes:new[] { typeof(InvItem), typeof(bool) })]
		public static bool EquipWeapon_Prefix(InvItem item, bool sfx, InvDatabase __instance) 
		{
			if (item == null)
				return false;

			Agent agent = __instance.agent;

			if (agent.statusEffects.hasTrait(cTrait.DrawNoBlood) && item.Categories.Contains("Piercing"))
			{
				BMHeaderTools.SayDialogue(agent, cDialogue.CantUseSharp, vNameType.Dialogue);
				GC.audioHandler.Play(__instance.agent, "CantDo");

				return false;
			}
			else if (agent.statusEffects.hasTrait(cTrait.AfraidOfLoudNoises) && item.Categories.Contains("Loud") && !item.contents.Contains("Silencer"))
			{
				BMHeaderTools.SayDialogue(agent, cDialogue.CantUseLoud, "Dialogue");
				GC.audioHandler.Play(__instance.agent, "CantDo");

				return false;
			}
			else if (agent.statusEffects.hasTrait(cTrait.SharpOnly) && item.Categories.Contains("Blunt"))
			{
				BMHeaderTools.SayDialogue(agent, cDialogue.CantUseBlunt, vNameType.Dialogue);
				GC.audioHandler.Play(__instance.agent, "CantDo");

				return false;
			}

			return true;
		}

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
