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
    public class BMItems
    {
        public static GameController GC => GameController.gameController;
        public static bool Prefix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) => BMHeader.MainInstance.PatchPrefix(type, methodName, patchType, patchMethodName, types);
        public static bool Postfix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) => BMHeader.MainInstance.PatchPostfix(type, methodName, patchType, patchMethodName, types);
        public static void BMLog(string logMessage) => BMHeader.Log(logMessage);

        #region Generic
        public void Awake()
        {
            InitializeItems();
            InitializeNames();

            InvDatabase_00();
            InvItem_00();
            ItemFunctions_00();
        }
        public static void InitializeItems()
        {
            // Not adding here until RL sprites are fixed, for the most part.
        }
        public static void InitializeNames()
        {
			string t;

			t = vNameType.Dialogue;
            _ = RogueLibs.CreateCustomName(cDialogue.CantDrinkAlcohol, t, new CustomNameInfo("Today, I choose not to drink."));
            _ = RogueLibs.CreateCustomName(cDialogue.CantEatMeat, t, new CustomNameInfo("Meat is murder!"));
            _ = RogueLibs.CreateCustomName(cDialogue.CantEatNonvegetarian, t, new CustomNameInfo("No! Me want meat!"));
            _ = RogueLibs.CreateCustomName(cDialogue.CantUseDrug, t, new CustomNameInfo("Nope, my body is a temple!"));
            _ = RogueLibs.CreateCustomName(cDialogue.CantUseLoud, t, new CustomNameInfo("I can't use that! It's too loooooud."));
			_ = RogueLibs.CreateCustomName(cDialogue.CantUseLoud_2, t, new CustomNameInfo("But that'll hurt my delicate little ears!"));
            _ = RogueLibs.CreateCustomName(cDialogue.CantUseBlunt, t, new CustomNameInfo("I need a sharper tool for this work."));
            _ = RogueLibs.CreateCustomName(cDialogue.CantUseSharp, t, new CustomNameInfo("Mommy says I can't use sharp things!"));
            _ = RogueLibs.CreateCustomName(cDialogue.CantUseSharp_2, t, new CustomNameInfo("I swore to draw no blood... unless I remove this trait first."));
            _ = RogueLibs.CreateCustomName(cDialogue.CantWearArmor, t, new CustomNameInfo("I'm too fuckin' fat to wear this!"));
            _ = RogueLibs.CreateCustomName(cDialogue.CantWearHeadgear, t, new CustomNameInfo("Owie! This is too tight for my big, fat, stupid, ugly head!"));
        }
        #endregion

        #region InvDatabase
        public void InvDatabase_00()
        {
            Type t = typeof(InvDatabase);
            Type g = GetType();

            Prefix(t, "ChooseWeapon", g, "InvDatabase_ChooseWeapon", new Type[1] { typeof(bool) });
            Prefix(t, "DetermineIfCanUseWeapon", g, "InvDatabase_DetermineIfCanUseWeapon", new Type[1] { typeof(InvItem) });
            Prefix(t, "EquipArmor", g, "InvDatabase_EquipArmor", new Type[2] { typeof(InvItem), typeof(bool) });
            Prefix(t, "EquipArmorHead", g, "InvDatabase_EquipArmorHead", new Type[2] { typeof(InvItem), typeof(bool) });
            Prefix(t, "EquipWeapon", g, "InvDatabase_EquipWeapon", new Type[2] { typeof(InvItem), typeof(bool) });
            Prefix(t, "SubtractFromItemCount", g, "InvDatabase_SubtractFromItemCount_c", new Type[3] { typeof(int), typeof(int), typeof(bool) });
            Prefix(t, "SubtractFromItemCount", g, "InvDatabase_SubtractFromItemCount_d", new Type[3] { typeof(InvItem), typeof(int), typeof(bool) });
        }
        public static bool InvDatabase_ChooseWeapon(bool noGuns, InvDatabase __instance) // Prefix
		{
            BMLog("InvDatabase_Chooseweapon");

            if (BMTraits.DoesPlayerHaveTraitFromList(__instance.agent, cTrait.LimitWeapons))
			{
                BMLog("\tTrait found");

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
                    BMLog("\titem: " + itemCandidate.invItemName);
                    //BMLog("\tLoud: " + vItem.loud.Contains(itemCandidate.invItemName)); // Works
                    //foreach (string a in itemCandidate.contents)
                    //    BMLog("\tContains: " + itemCandidate.contents[itemCandidate.contents.IndexOf(a)]); // Works

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

                if (invItem != null && (__instance.agent.isPlayer <= 0 || !(__instance.equippedWeapon.itemType == "WeaponMelee") || __instance.equippedWeapon == __instance.fist) && !__instance.equippedWeapon.Categories.Contains("NonViolent"))
                    __instance.EquipWeapon(invItem);
                else if (invItem2 != null)
                    __instance.EquipWeapon(invItem2);
                else 
                    __instance.EquipWeapon(__instance.fist);

                return false;
			}

            return true;
		}
        public static bool InvDatabase_DetermineIfCanUseWeapon(InvItem item, InvDatabase __instance, ref bool __result) // Replacement
        {
            //TODO: Verify non-equipped items like Time Bomb.

            //BMLog("InvDatabase_DetermineIfCanUseWeapon");
            //BMLog("\tItem: " + item.invItemName);
            //BMLog("\tSharp: " + vItem.piercing.Contains(item.invItemName));
            //BMLog("\tLoud: " + vItem.loud.Contains(item.invItemName));

            Agent agent = __instance.agent;
            bool fist = (item.invItemName == vItem.Fist);

            __result = (
                (!(item.itemType == "WeaponProjectile") || !agent.statusEffects.hasTrait(vTrait.StubbyFingers) || item.Categories.Contains("NotRealWeapons")) && 
                (fist || item.Categories.Contains("NonViolent") || item.Categories.Contains("NotRealWeapons") || !agent.statusEffects.hasTrait(vTrait.Pacifist)) && 
                (fist || !agent.statusEffects.hasTrait(vTrait.SausageFingers)) && 
                (fist || !agent.statusEffects.hasTrait(vTrait.Harmless)) && 
                (fist || !agent.statusEffects.hasTrait(vTrait.NearHarmless)) &&
                (!vItem.piercing.Contains(item.invItemName) || !agent.statusEffects.hasTrait(cTrait.DrawNoBlood)) &&
                (!vItem.loud.Contains(item.invItemName) || item.contents.Contains(vItem.Silencer) || !agent.statusEffects.hasTrait(cTrait.AfraidOfLoudNoises)) ) || 
                (agent.isPlayer != 0 && !agent.localPlayer);

            return false;
        }
        public static bool InvDatabase_EquipArmor(InvItem item, bool sfx, InvDatabase __instance) // Prefix
        {
            if (item.isArmor && __instance.agent.statusEffects.hasTrait(cTrait.Fatass))
            {
                BMHeaderTools.SayDialogue(__instance.agent, cDialogue.CantWearArmor, vNameType.Dialogue);
                GC.audioHandler.Play(__instance.agent, "CantDo");

                return false;
            }

            return true;
        }
        public static bool InvDatabase_EquipArmorHead(InvItem item, bool sfx, InvDatabase __instance) // Prefix
        {
            if (item.isArmorHead && item != null && __instance.agent.statusEffects.hasTrait(cTrait.FatHead))
            {
                BMHeaderTools.SayDialogue(__instance.agent, cDialogue.CantWearHeadgear, vNameType.Dialogue);
                GC.audioHandler.Play(__instance.agent, "CantDo");

                return false;
            }

            return true;
        }
        public static bool InvDatabase_EquipWeapon(InvItem item, bool sfx, InvDatabase __instance) // Prefix
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
        public static bool InvDatabase_SubtractFromItemCount_c(int slotNum, ref int amount, bool toolbarMove, InvDatabase __instance) // Prefix
        {
            BMLog("InvDatabase_SubtractFromItemCount_c:");
            BMLog("\tslotNum = " + slotNum);
            BMLog("\tamount = " + amount);
            BMLog("\ttoolbarMove = " + toolbarMove);

            if (vItem.tools.Contains(__instance.InvItemList[slotNum].invItemName))
            {
                if (__instance.agent.statusEffects.hasTrait(cTrait.TamperTantrum_2))
                    amount = 0;
                else if (__instance.agent.statusEffects.hasTrait(cTrait.TamperTantrum))
                    amount /= 2;
            }
            return true;
        }
        public static bool InvDatabase_SubtractFromItemCount_d(InvItem invItem, ref int amount, bool toolbarMove, InvDatabase __instance) // Prefix
        {
            BMLog("InvDatabase_SubtractFromItemCount_d:");
            BMLog("\tInvItem = " + invItem.invItemName);
            BMLog("\tamount = " + amount);
            BMLog("\ttoolbarMove = " + toolbarMove);

            if (vItem.tools.Contains(invItem.invItemName))
            {
                if (__instance.agent.statusEffects.hasTrait(cTrait.TamperTantrum_2))
                    amount = 0;
                else if (__instance.agent.statusEffects.hasTrait(cTrait.TamperTantrum))
                    amount /= 2;
            }
            return true;
        }
        #endregion
        #region InvItem
        public void InvItem_00()
        {
            Postfix(typeof(InvItem), "SetupDetails", GetType(), "InvItem_SetupDetails", new Type[1] { typeof(bool) });
            Prefix(typeof(InvItem), "UseItem", GetType(), "InvItem_UseItem", new Type[0] { });
        }
        public static void InvItem_SetupDetails(bool notNew, InvItem __instance) // Postfix
        {
            string name = __instance.invItemName;

            if (__instance.Categories.Count == 0)
                __instance.Categories.Add("NullCatcher");

            if (__instance.Categories.Contains("Alcohol"))
            {
            }

            if (__instance.Categories.Contains("Drugs"))
            {
            }

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
        public static bool InvItem_UseItem(InvItem __instance) // Prefix
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
        #endregion
        #region ItemFunctions
        public void ItemFunctions_00()
        {
            Postfix(typeof(ItemFunctions), "DetermineHealthChange", GetType(), "ItemFunctions_DetermineHealthChange", new Type[2] { typeof(InvItem), typeof(Agent) });
            Prefix(typeof(ItemFunctions), "UseItem", GetType(), "ItemFunctions_UseItem", new Type[2] { typeof(InvItem), typeof(Agent) });
        }
        public static void ItemFunctions_DetermineHealthChange(InvItem item, Agent agent, ref int __result) // Postfix
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
        public static bool ItemFunctions_UseItem(InvItem item, Agent agent, ItemFunctions __instance) // Prefix ***new
        {
            if (item.itemType == "Consumable")
            {
                if (vItem.alcohol.Contains(item.invItemName) && ((agent.statusEffects.hasTrait(cTrait.FriendOfBill) || agent.statusEffects.hasTrait(cTrait.Teetotaller))))
                {
                    BMHeaderTools.SayDialogue(agent, cDialogue.CantDrinkAlcohol, vNameType.Dialogue);
                    goto terminus;
                }
                else if (vItem.drugs.Contains(item.invItemName) && (agent.statusEffects.hasTrait(cTrait.DAREdevil) || agent.statusEffects.hasTrait(cTrait.Teetotaller)))
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
        #endregion
    }
}