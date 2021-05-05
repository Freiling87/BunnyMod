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

			t = vNameType.StatusEffect;
			_ = RogueLibs.CreateCustomName(cDialogue.AfraidOfLoudNoises, t, new CustomNameInfo("I can't use that! It's too loooooud."));
			_ = RogueLibs.CreateCustomName(cDialogue.AfraidOfLoudNoises2, t, new CustomNameInfo("But that'll hurt my delicate little ears!"));
			_ = RogueLibs.CreateCustomName(cDialogue.Carnivore, t, new CustomNameInfo("No! Me want meat!"));
            _ = RogueLibs.CreateCustomName(cDialogue.DAREdevil, t, new CustomNameInfo("Nope, my body is a temple!"));
            _ = RogueLibs.CreateCustomName(cDialogue.DrawNoBlood, t, new CustomNameInfo("Mommy says I can't use sharp things!"));
            _ = RogueLibs.CreateCustomName(cDialogue.DrawNoBlood2, t, new CustomNameInfo("I swore to draw no blood... unless I remove this trait first."));
            _ = RogueLibs.CreateCustomName(cDialogue.FatassCantWear, t, new CustomNameInfo("I'm too fuckin' fat to wear this!"));
            _ = RogueLibs.CreateCustomName(cDialogue.FatHeadCantWear, t, new CustomNameInfo("Owie! This is too tight for my big, fat, stupid, ugly head!"));
            _ = RogueLibs.CreateCustomName(cDialogue.FriendOfBill, t, new CustomNameInfo("Today, I choose not to drink."));
            _ = RogueLibs.CreateCustomName(cDialogue.SharpOnly, t, new CustomNameInfo("I need a sharper tool for this work."));
            _ = RogueLibs.CreateCustomName(cDialogue.Vegetarian, t, new CustomNameInfo("Meat is murder!"));
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

                foreach (InvItem invItem3 in __instance.InvItemList)
                {
                    BMLog("\titem: " + invItem3.invItemName);
                    BMLog("\tLoud: " + vItem.loud.Contains(invItem3.invItemName));
                    foreach (string a in invItem3.contents)
                        BMLog("\tContains: " + invItem3.contents[invItem3.contents.IndexOf(a)]);

                    if (invItem3.itemType == "WeaponProjectile" && 
                        invItem3.hierarchy2 > num && 
                        (!noGuns || invItem3.Categories.Contains("NotRealWeapons")) && 
                        (!noWeapons || invItem3.Categories.Contains("NonViolent")) && 
                        (!noLoud || (vItem.loud.Contains(invItem3.invItemName) && !invItem3.contents.Contains(vItem.Silencer))) &&
                        invItem3.invItemCount > 0 && 
                        (!invItem3.dontAutomaticallySelect || __instance.agent.isPlayer <= 0) && 
                        (__instance.agent.isPlayer <= 0 || !(invItem3.itemType != __instance.lastEquippedWeaponType)) && 
                        (!invItem3.dontSelectNPC || __instance.agent.isPlayer != 0))
                    {
                        invItem = invItem3;
                        num = invItem3.hierarchy2;
                    }
                
                    if (invItem3.itemType == "WeaponMelee" && invItem3.hierarchy2 > num2 && 
                        !flag && 
                        (!flag3 || invItem3.Categories.Contains("NonViolent")) &&
                        (!noLoud || (vItem.loud.Contains(invItem3.invItemName) && !invItem3.contents.Contains(vItem.Silencer))) &&
                        invItem3.invItemCount > 0 && 
                        (!invItem3.dontAutomaticallySelect || __instance.agent.isPlayer <= 0) && 
                        (!invItem3.dontSelectNPC || __instance.agent.isPlayer != 0))
                    {
                        invItem2 = invItem3;
                        num2 = invItem3.hierarchy2;
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

            BMLog("InvDatabase_DetermineIfCanUseWeapon");
            BMLog("\tItem: " + item.invItemName);
            BMLog("\tSharp: " + vItem.piercing.Contains(item.invItemName));
            BMLog("\tLoud: " + vItem.loud.Contains(item.invItemName));

            Agent agent = __instance.agent;
            bool fist = (item.invItemName == vItem.Fist);

            __result = (
                (!(item.itemType == "WeaponProjectile") || !agent.statusEffects.hasTrait(vTrait.StubbyFingers) || item.Categories.Contains("NotRealWeapons")) && 
                (fist || item.Categories.Contains("NonViolent") || item.Categories.Contains("NotRealWeapons") || !agent.statusEffects.hasTrait(vTrait.Pacifist)) && 
                (fist || !agent.statusEffects.hasTrait(vTrait.SausageFingers)) && 
                (fist || !agent.statusEffects.hasTrait(vTrait.Harmless)) && 
                (fist || !agent.statusEffects.hasTrait(vTrait.NearHarmless)) &&
                (!vItem.piercing.Contains(item.invItemName) || !agent.statusEffects.hasTrait(cTrait.DrawNoBlood)) &&
                (!vItem.loud.Contains(item.invItemName) || !agent.statusEffects.hasTrait(cTrait.AfraidOfLoudNoises)) ) || 
                (agent.isPlayer != 0 && !agent.localPlayer);

            return false;
        }
        public static bool InvDatabase_EquipArmor(InvItem item, bool sfx, InvDatabase __instance) // Prefix
        {
            if (item.isArmor && __instance.agent.statusEffects.hasTrait(cTrait.Fatass))
            {
                __instance.agent.SayDialogue(cDialogue.FatassCantWear);
                GC.audioHandler.Play(__instance.agent, "CantDo");

                return false;
            }

            return true;
        }
        public static bool InvDatabase_EquipArmorHead(InvItem item, bool sfx, InvDatabase __instance) // Prefix
        {
            if (item.isArmorHead && item != null && __instance.agent.statusEffects.hasTrait(cTrait.FatHead))
            {
                __instance.agent.SayDialogue(cDialogue.FatHeadCantWear);
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
                agent.SayDialogue(cDialogue.DrawNoBlood);
                GC.audioHandler.Play(__instance.agent, "CantDo");

                return false;
            }
            else if (agent.statusEffects.hasTrait(cTrait.AfraidOfLoudNoises) && item.Categories.Contains("Loud") && !item.contents.Contains("Silencer"))
            {
                agent.SayDialogue(cDialogue.AfraidOfLoudNoises);
                GC.audioHandler.Play(__instance.agent, "CantDo");

                return false;
            }
            else if (agent.statusEffects.hasTrait(cTrait.SharpOnly) && item.Categories.Contains("Blunt"))
            {
                agent.SayDialogue(cDialogue.SharpOnly);
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
                agent.SayDialogue(cDialogue.FriendOfBill);
                cantDoFlag = true;
            }
            else if (cats.Contains("Drugs") && (agent.statusEffects.hasTrait(cTrait.DAREdevil) || agent.statusEffects.hasTrait(cTrait.Teetotaller)))
            {
                agent.SayDialogue(cDialogue.DAREdevil);
                cantDoFlag = true;
            }
            else if (cats.Contains("NonVegetarian") && agent.statusEffects.hasTrait(cTrait.Vegetarian))
            {
                agent.SayDialogue(cDialogue.Vegetarian);
                cantDoFlag = true;
            }
            else if (cats.Contains("Vegetarian") && agent.statusEffects.hasTrait(cTrait.Carnivore))
            {
                agent.SayDialogue(cDialogue.Carnivore);
                cantDoFlag = true;
            }
            else if (cats.Contains("Loud") && agent.statusEffects.hasTrait(cTrait.AfraidOfLoudNoises))
            {
                agent.SayDialogue(cDialogue.AfraidOfLoudNoises2);
                cantDoFlag = true;
            }
            else if (cats.Contains("Piercing") && agent.statusEffects.hasTrait(cTrait.DrawNoBlood))
            {
                agent.SayDialogue(cDialogue.DrawNoBlood2);
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
                    agent.SayDialogue(cDialogue.FriendOfBill);
                    goto terminus;
                }
                else if (vItem.drugs.Contains(item.invItemName) && (agent.statusEffects.hasTrait(cTrait.DAREdevil) || agent.statusEffects.hasTrait(cTrait.Teetotaller)))
                {
                    agent.SayDialogue(cDialogue.DAREdevil);
                    goto terminus;
                }
                else if (vItem.nonVegetarian.Contains(item.invItemName) && agent.statusEffects.hasTrait(cTrait.Vegetarian))
                {
                    agent.SayDialogue(cDialogue.Vegetarian);
                    goto terminus;
                }
                else if (vItem.vegetarian.Contains(item.invItemName) && agent.statusEffects.hasTrait(cTrait.Carnivore))
                {
                    agent.SayDialogue(cDialogue.Carnivore);
                    goto terminus;
                }
            }
            else
            {
                if (vItem.loud.Contains(item.invItemName) && agent.statusEffects.hasTrait(cTrait.AfraidOfLoudNoises))
                {
                    agent.SayDialogue(cDialogue.AfraidOfLoudNoises2);
                    goto terminus;
                }

                if (vItem.piercing.Contains(item.invItemName) && agent.statusEffects.hasTrait(cTrait.DrawNoBlood))
                {
                    agent.SayDialogue(cDialogue.DrawNoBlood2);
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