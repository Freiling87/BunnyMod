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
            Item_00();
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
            if (__instance.agent.isPlayer != 0 && BMTraits.DoesPlayerHaveTraitFromList(__instance.agent, cTrait.LimitWeapons))
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
		#region Item
        public void Item_00()
		{
            Prefix(typeof(Item), "SpecialDamage", GetType(), "Item_SpecialDamage", new Type[1] { typeof(PlayfieldObject) });
		}
        public static List<string> customThrowables = new List<string>()
        {
            cItem.Brick,
            cItem.BeerCan,
            cItem.Lunchbox,
            cItem.ManholeCover,
            cItem.Sawblade,
            cItem.ThrowingKnife,
            cItem.WhiskeyBottle
        };
        public static bool Item_SpecialDamage(PlayfieldObject damagerObject, Item __instance)
		{
            string itemName = __instance.invItem.invItemName;

            if (customThrowables.Contains(itemName) && damagerObject.CompareTag("Agent"))
			{
                Agent agent = (Agent)damagerObject;

                if (itemName == cItem.BeerCan)
                    GC.audioHandler.Play(agent, vAudioClip.BulletHitObject);
                else if (itemName == cItem.ManholeCover)
                    GC.audioHandler.Play(agent, vAudioClip.MeleeHitAgentLarge);
                else if (itemName == cItem.Sawblade)
                    GC.audioHandler.Play(agent, vAudioClip.SawBladeHit);
                else if (itemName == cItem.ThrowingKnife)
                    GC.audioHandler.Play(agent, vAudioClip.MeleeHitAgentCutSmall2);
                else if (itemName == cItem.WhiskeyBottle)
                    GC.audioHandler.Play(agent, vAudioClip.WindowDamage);
                else
                    GC.audioHandler.Play(agent, vAudioClip.MeleeHitAgentLarge);

                GC.spawnerMain.SpawnParticleEffect("ObjectDestroyed", __instance.tr.position, __instance.tr.eulerAngles.z);

                if (agent.inhuman || agent.mechFilled || agent.mechEmpty)
                {
                    GC.spawnerMain.SpawnParticleEffect("BloodHitYellow", agent.tr.position, __instance.tr.eulerAngles.z);
                    GC.playerAgent.objectMultPlayfield.SpawnParticleEffect("BloodHitYellow", agent.tr.position, __instance.tr.eulerAngles.z, false, agent);
                }
                else
                {
                    GC.spawnerMain.SpawnParticleEffect("BloodHit", agent.tr.position, __instance.tr.eulerAngles.z);
                    GC.playerAgent.objectMultPlayfield.SpawnParticleEffect("BloodHit", agent.tr.position, __instance.tr.eulerAngles.z, false, agent);
                }

                __instance.DestroyMeFromClient();

                return false;
			}

            return true;
		}
        #endregion
        #region ItemFunctions
        public void ItemFunctions_00()
        {
            Postfix(typeof(ItemFunctions), "DetermineHealthChange", GetType(), "ItemFunctions_DetermineHealthChange", new Type[2] { typeof(InvItem), typeof(Agent) });
            Prefix(typeof(ItemFunctions), "UseItem", GetType(), "ItemFunctions_UseItem_Prefix", new Type[2] { typeof(InvItem), typeof(Agent) });
            Postfix(typeof(ItemFunctions), "UseItem", GetType(), "ItemFunctions_UseItem_Postfix", new Type[2] { typeof(InvItem), typeof(InvItem) });
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
        public static bool ItemFunctions_UseItem_Prefix(InvItem item, Agent agent, ItemFunctions __instance) // Prefix
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
        public static void ItemFunctions_UseItem_Postfix(InvItem item, Agent agent, ItemFunctions __instance) // Postfix
		{
            if (item.invItemName == vItem.Whiskey)
                item.database.AddItemOrDrop(cItem.WhiskeyBottle, 1);
		}
        #endregion
    }

    [ItemCategories(RogueCategories.Technology, RogueCategories.Usable)]
    public class BearTrapProcessor : CustomItem, IItemCombinable
    {
        [RLSetup]
        public static void Setup()
        {
            RogueLibs.CreateCustomItem<BearTrapProcessor>()
                .WithDescription(new CustomNameInfo("You can stick pretty much anything in here, and a Bear Trap will pop out once it reaches a value threshold of 100."))
                .WithName(new CustomNameInfo(cItem.BearTrapProcessor))
                .WithSprite(Properties.Resources.BearTrapProcessor)
                .WithUnlock(new ItemUnlock
                {
                    CharacterCreationCost = 4,
                    IsAvailable = true,
                    IsAvailableInCC = true,
                    IsAvailableInItemTeleporter = true,
                    LoadoutCost = 4,
                    Prerequisites = { vItem.FoodProcessor, vItem.Beartrap },
                    UnlockCost = 4
                });
        }
        public override void SetupDetails()
        {
            Item.cantBeCloned = true;
            Item.hasCharges = true;
            Item.initCount = 0;
            Item.itemValue = 200;
            Item.itemType = ItemTypes.Combine;
            Item.rewardCount = 0;
            Item.stackable = true;
            Item.thiefCantSteal = true;
        }
        public bool CombineFilter(InvItem other) =>
            other.contents.Count > 0 &&
            other.itemValue > 0 &&
            other.invItemName != vItem.Money &&
            !other.cantDrop &&
            !other.questItem;
        public bool CombineItems(InvItem other)
        {
            if (!CombineFilter(other)) return false;

            if (Owner.agentInvDatabase.equippedWeapon == other)
                Owner.agentInvDatabase.UnequipWeapon();
            else if (Owner.agentInvDatabase.equippedArmor == other)
                Owner.agentInvDatabase.UnequipArmor();
            else if (Owner.agentInvDatabase.equippedArmorHead == other)
                Owner.agentInvDatabase.UnequipArmorHead();

            if (other.isArmor || other.isArmorHead || other.hasCharges ||
                (other.isWeapon && other.itemType != ItemTypes.WeaponThrown))
                Owner.agentInvDatabase.DestroyItem(other);
            else
                Owner.agentInvDatabase.SubtractFromItemCount(other, 1);

            int unitsMade = 0;
            int totalOutput = Item.invItemCount;
            totalOutput += Owner.determineMoneyCost(other, other.itemValue, vItem.BombProcessor);

            while (totalOutput >= 100)
            {
                totalOutput -= 100;
                unitsMade++;
            }

            Item.invItemCount = totalOutput;

            for (int j = 0; j < unitsMade; j++)
            {
                InvItem sugar = new InvItem();
                sugar.invItemName = vItem.Beartrap;
                sugar.invItemCount = 1;
                sugar.contents.Clear();
                sugar.SetupDetails(false);

                Owner.inventory.AddItemOrDrop(sugar);
            }

            gc.audioHandler.Play(Owner, vAudioClip.CombineItem);

            return true;
        }
        public CustomTooltip CombineTooltip(InvItem other) => default;
        public CustomTooltip CombineCursorText(InvItem other) => default;
    }

    [ItemCategories(RogueCategories.Weapons, RogueCategories.NotRealWeapons, "Trash")]
    public class BeerCan : CustomItem
    {
        [RLSetup]
        public static void Setup()
        {
            RogueLibs.CreateCustomItem<BeerCan>()
                .WithName(new CustomNameInfo("Beer Can"))
                .WithDescription(new CustomNameInfo("No, you can't crack someone's skull with it. It won't break windows. And the worst part: there's no beer in it."))
                .WithSprite(Properties.Resources.BeerCan)
                .WithUnlock(new ItemUnlock
                {
                    CharacterCreationCost = 1,
                    IsAvailable = true,
                    IsAvailableInCC = true,
                    IsAvailableInItemTeleporter = true,
                    LoadoutCost = 1,
                    Prerequisites = { vItem.Beer },
                    UnlockCost = 1
                });
        }
        public override void SetupDetails()
        {
            Item.cantBeCloned = true;
            Item.hasCharges = false;
            Item.initCount = 10;
            Item.isWeapon = false;
            Item.itemType = ItemTypes.WeaponThrown;
            Item.itemValue = 3;
            Item.rewardCount = 10;
            Item.stackable = true;
            Item.throwDistance = 4;
            Item.throwExtraDist = true;
            Item.throwDamage = 0;
            Item.weaponCode = weaponType.WeaponThrown;
        }
    }

    [ItemCategories(RogueCategories.Weapons, RogueCategories.NotRealWeapons, "Trash")]
    public class Brick : CustomItem
    {
        [RLSetup]
        public static void Setup()
        {
            RogueLibs.CreateCustomItem<Brick>()
                .WithName(new CustomNameInfo("Brick"))
                .WithDescription(new CustomNameInfo("Look around you. That wall? Brick. That fireplace? Brick. Everything is made of brick. Brick is the fundamental 'building block' of the universe. I think. I didn't do a lot of research on this."))
                .WithSprite(Properties.Resources.Brick)
                .WithUnlock(new ItemUnlock
                {
                    CharacterCreationCost = 2,
                    IsAvailable = true,
                    IsAvailableInCC = true,
                    IsAvailableInItemTeleporter = true,
                    LoadoutCost = 2,
                    Prerequisites = { },
                    UnlockCost = 2
                });
        }
        public override void SetupDetails()
        {
            Item.cantBeCloned = true;
            Item.hasCharges = false;
            Item.initCount = 10;
            Item.isWeapon = false;
            Item.itemType = ItemTypes.WeaponThrown;
            Item.itemValue = 5;
            Item.rewardCount = 10;
            Item.stackable = true;
            Item.throwDistance = 6;
            Item.throwExtraDist = true;
            Item.throwDamage = 8;
            Item.weaponCode = weaponType.WeaponThrown;
        }
    }

    [ItemCategories(RogueCategories.Weapons, RogueCategories.NotRealWeapons, RogueCategories.Melee, "Trash")]
    public class BrokenBottle : CustomItem
    {
        [RLSetup]
        public static void Setup()
        {
            RogueLibs.CreateCustomItem<BrokenBottle>()
                .WithName(new CustomNameInfo("Broken Bottle"))
                .WithDescription(new CustomNameInfo("Hmm, what could you do with this? Recycle it? Make an art installation? It's probably too sharp for a windchime. Think about it."))
                .WithSprite(Properties.Resources.BrokenBottle)
                .WithUnlock(new ItemUnlock
                {
                    CharacterCreationCost = 1,
                    IsAvailable = true,
                    IsAvailableInCC = true,
                    IsAvailableInItemTeleporter = true,
                    LoadoutCost = 1,
                    Prerequisites = { cItem.WhiskeyBottle },
                    UnlockCost = 1
                });
        }
        public override void SetupDetails()
        {
            Item.cantBeCloned = true;
            Item.hitSoundType = vAudioClip.MeleeHitAgentCutSmall2;
            Item.hasCharges = false;
            Item.initCount = 40;
            Item.isWeapon = false;
            Item.itemType = ItemTypes.WeaponMelee;
            Item.itemValue = 1;
            Item.meleeDamage = 4;
            Item.rewardCount = 40;
            Item.stackable = false;
            Item.weaponCode = weaponType.WeaponMelee;
        }
    }

    [ItemCategories(RogueCategories.Food, RogueCategories.Usable)]
    public class EnergyDrink : CustomItem
	{
        [RLSetup]
        public static void Setup()
		{
            RogueLibs.CreateCustomItem<EnergyDrink>()
                .WithDescription(new CustomNameInfo(""))
                .WithName(new CustomNameInfo(""))
                .WithSprite(Properties.Resources.EnergyDrink)
                .WithUnlock(new ItemUnlock
                {
                    CharacterCreationCost = 3,
                    LoadoutCost = 3,
                    Prerequisites = { },
                    UnlockCost = 3
                });
		}

		public override void SetupDetails()
		{
            Item.goesInToolbar = true;
            Item.healthChange = 5;
            Item.initCount = 1;
            Item.itemType = ItemTypes.Consumable;
            Item.itemValue = 20;
            Item.rewardCount = 3;
            Item.stackable = true;
		}

        [IgnoreChecks("FullHealth")]
        public bool UseItem()
		{
            if (Owner.electronic)
			{
                Owner.SayDialogue("");
                gc.audioHandler.Play(Owner, vAudioClip.CantDo);
     
                return false;
			}

            Owner.AddEffect<ChronomanticDilation>(15);
            gc.audioHandler.Play(Owner, vAudioClip.UseDrink);
            Count--;

            return true;
		}
	}

    [ItemCategories(RogueCategories.Passive, "Blessed")]
    public class HolySymbol : CustomItem
	{
        [RLSetup]
        public static void Setup()
		{
            RogueLibs.CreateCustomItem<HolySymbol>()
                .WithDescription(new CustomNameInfo("Shiny!\n\nHoly!\n\nThat's it!"))
                .WithName(new CustomNameInfo("Holy Symbol"))
                .WithSprite(Properties.Resources.HolySymbol)
                .WithUnlock(new ItemUnlock
                {
                    CharacterCreationCost = 3,
                    LoadoutCost = 3,
                    UnlockCost = 5,
                    Prerequisites = { }
                });
		}
		public override void SetupDetails()
		{
            Item.itemType = ItemTypes.Tool;
            Item.itemValue = 80;
            Item.cantBeCloned = true;
            Item.goesInToolbar = false;
        }
	}

    [ItemCategories(RogueCategories.Weapons, RogueCategories.NotRealWeapons, "Blessed")]
    public class HolyWaterFlask : CustomItem
    {
        [RLSetup]
        public static void Setup()
        {
            RogueLibs.CreateCustomItem<BeerCan>()
                .WithName(new CustomNameInfo("Holy Water Flask"))
                .WithDescription(new CustomNameInfo("It's soapy water. An old man mumbled over it. Vampires and Zombies seem to take it seriously though, so whatever."))
                .WithSprite(Properties.Resources.HolyWaterFlask)
                .WithUnlock(new ItemUnlock
                {
                    CharacterCreationCost = 1,
                    IsAvailable = true,
                    IsAvailableInCC = true,
                    IsAvailableInItemTeleporter = true,
                    LoadoutCost = 1,
                    Prerequisites = { },
                    UnlockCost = 1
                });
        }
        public override void SetupDetails()
        {
            Item.cantBeCloned = false;
            Item.hasCharges = false;
            Item.initCount = 3;
            Item.isWeapon = false;
            Item.itemType = ItemTypes.WeaponThrown;
            Item.itemValue = 15;
            Item.rewardCount = 3;
            Item.stackable = true;
            Item.throwDistance = 7;
            Item.throwExtraDist = true;
            Item.throwDamage = 0;
            Item.weaponCode = weaponType.WeaponThrown;
        }
    }

    [ItemCategories(RogueCategories.Weapons, RogueCategories.NotRealWeapons)]
    public class ManholeCover : CustomItem
    {
        [RLSetup]
        public static void Setup()
        {
            RogueLibs.CreateCustomItem<ManholeCover>()
                .WithName(new CustomNameInfo("Manhole Cover"))
                .WithDescription(new CustomNameInfo("Wow, this seems a little large for a manhole, but I guess some people do go kinda crazy.\n\nOh, you mean those things in the ground..."))
                .WithSprite(Properties.Resources.ManholeCover)
                .WithUnlock(new ItemUnlock
                {
                    CharacterCreationCost = 2,
                    IsAvailable = true,
                    IsAvailableInCC = true,
                    IsAvailableInItemTeleporter = true,
                    LoadoutCost = 2,
                    Prerequisites = { },
                    UnlockCost = 2
                });
        }
        public override void SetupDetails()
        {
            Item.cantBeCloned = true;
            Item.hasCharges = false;
            Item.initCount = 1;
            Item.isWeapon = false;
            Item.itemType = ItemTypes.WeaponThrown;
            Item.itemValue = 25;
            Item.stackable = false;
            Item.throwDistance = 6;
            Item.throwExtraDist = true;
            Item.throwDamage = 20;
            Item.weaponCode = weaponType.WeaponThrown;
        }
    }

    [ItemCategories(RogueCategories.Weapons)]
    public class Sawblade : CustomItem
    {
        [RLSetup]
        public static void Setup()
        {
            RogueLibs.CreateCustomItem<Sawblade>()
                .WithName(new CustomNameInfo("Sawblade"))
                .WithDescription(new CustomNameInfo("Sawblade? Yeah, but Blade: Trinity kinda sucked."))
                .WithSprite(Properties.Resources.Sawblade)
                .WithUnlock(new ItemUnlock
                {
                    CharacterCreationCost = 3,
                    IsAvailable = true,
                    IsAvailableInCC = true,
                    IsAvailableInItemTeleporter = true,
                    LoadoutCost = 3,
                    Prerequisites = { },
                    UnlockCost = 3
                });
        }
        public override void SetupDetails()
        {
            Item.cantBeCloned = false;
            Item.hasCharges = false;
            Item.initCount = 1;
            Item.isWeapon = true;
            Item.itemType = ItemTypes.WeaponThrown;
            Item.itemValue = 50;
            Item.rewardCount = 3;
            Item.stackable = true;
            Item.throwDistance = 8;
            Item.throwExtraDist = true;
            Item.throwDamage = 25;
        }
    }

    [ItemCategories(RogueCategories.Weapons, RogueCategories.Melee)]
    public class Spear : CustomItem
	{
        [RLSetup]
        public static void Setup()
		{
            RogueLibs.CreateCustomItem<Spear>()
                .WithDescription(new CustomNameInfo("Make sure the pointy end goes toward the bad guy and you're all set. Can you handle that?"))
                .WithName(new CustomNameInfo(cItem.Spear))
                .WithSprite(Properties.Resources.Spear)
                .WithUnlock(new ItemUnlock
                {
                    CharacterCreationCost = 2,
                    IsAvailable = true,
                    IsAvailableInCC = true,
                    IsAvailableInItemTeleporter = true,
                    LoadoutCost = 2,
                    Prerequisites = { vItem.Knife },
                    UnlockCost = 3
                });
		}
		public override void SetupDetails()
        {
            Item.hasCharges = false;
            Item.hitSoundType = vAudioClip.MeleeHitAgentCutLarge;
            Item.initCount = 100;
            Item.isWeapon = true;
            Item.itemType = ItemTypes.WeaponMelee;
            Item.itemValue = 60;
            Item.meleeDamage = 8;
            Item.stackable = false;
            Item.weaponCode = weaponType.WeaponMelee;
		}
	}

    [ItemCategories(RogueCategories.Drugs, RogueCategories.Technology, RogueCategories.Usable)]
    public class SugarProcessor : CustomItem, IItemCombinable
	{
        [RLSetup]
        public static void Setup()
		{
            RogueLibs.CreateCustomItem<SugarProcessor>()
                .WithDescription(new CustomNameInfo("You can stick pretty much anything in here, and a bag of Sugar will pop out once it reaches a value threshold of 100."))
                .WithName(new CustomNameInfo(cItem.SugarProcessor))
                .WithSprite(Properties.Resources.SugarProcessor)
                .WithUnlock(new ItemUnlock
                {
                    CharacterCreationCost = 4,
                    IsAvailable = true,
                    IsAvailableInCC = true,
                    IsAvailableInItemTeleporter = true,
                    LoadoutCost = 4,
                    Prerequisites = { vItem.FoodProcessor, vItem.Sugar },
                    UnlockCost = 4
                });
		}
		public override void SetupDetails()
		{
            Item.cantBeCloned = true;
            Item.hasCharges = true;
            Item.initCount = 0;
            Item.itemValue = 200;
            Item.itemType = ItemTypes.Combine;
            Item.rewardCount = 0;
            Item.stackable = true;
            Item.thiefCantSteal = true;
		}
        public bool CombineFilter(InvItem other) => 
            other.contents.Count > 0 &&
            other.itemValue > 0 &&
            other.invItemName != vItem.Money &&
            !other.cantDrop &&
            !other.questItem;
        public bool CombineItems(InvItem other)
		{
            if (!CombineFilter(other)) return false;

            if (Owner.agentInvDatabase.equippedWeapon == other)
                Owner.agentInvDatabase.UnequipWeapon();
            else if (Owner.agentInvDatabase.equippedArmor == other)
                Owner.agentInvDatabase.UnequipArmor();
            else if (Owner.agentInvDatabase.equippedArmorHead == other)
                Owner.agentInvDatabase.UnequipArmorHead();
            
            if (other.isArmor || other.isArmorHead || other.hasCharges || 
                (other.isWeapon && other.itemType != ItemTypes.WeaponThrown))
                Owner.agentInvDatabase.DestroyItem(other);
            else
                Owner.agentInvDatabase.SubtractFromItemCount(other, 1);

            int unitsMade = 0;
            int totalOutput = Item.invItemCount;
            totalOutput += Owner.determineMoneyCost(other, other.itemValue, vItem.BombProcessor) / 2;

            while (totalOutput >= 100)
            {
                totalOutput -= 100;
                unitsMade++;
            }

            Item.invItemCount = totalOutput;

            for (int j = 0; j < unitsMade; j++)
            {
                InvItem sugar = new InvItem();
                sugar.invItemName = vItem.Sugar;
                sugar.invItemCount = 1;
                sugar.contents.Clear();
                sugar.SetupDetails(false);

                Owner.inventory.AddItemOrDrop(sugar);
            }

            gc.audioHandler.Play(Owner, vAudioClip.CombineItem);

            return true;
        }
        public CustomTooltip CombineTooltip(InvItem other) => default;
        public CustomTooltip CombineCursorText(InvItem other) => default;
	}

    [ItemCategories(RogueCategories.Weapons, RogueCategories.NotRealWeapons, "Trash")]
    public class WhiskeyBottle : CustomItem
	{
        [RLSetup]
        public static void Setup()
		{
            RogueLibs.CreateCustomItem<WhiskeyBottle>()
                .WithName(new CustomNameInfo("Whiskey Bottle"))
                .WithDescription(new CustomNameInfo("No, it's just the bottle, you see. Stop shaking it. No, don't put your tongue in it, you degenerate! Even I don't know where that's been. Just throw it at people like a normal person."))
                .WithSprite(Properties.Resources.WhiskeyBottle)
                .WithUnlock(new ItemUnlock
                {
                    CharacterCreationCost = 3,
                    IsAvailable = true,
                    IsAvailableInCC = true,
                    IsAvailableInItemTeleporter = true,
                    LoadoutCost = 3,
                    Prerequisites = { vItem.Whiskey },
                    UnlockCost = 3
                });
		}
		public override void SetupDetails()
		{
            Item.cantBeCloned = true;
            Item.hasCharges = false;
            Item.initCount = 5;
            Item.isWeapon = false;
            Item.itemType = ItemTypes.WeaponThrown;
            Item.itemValue = 5;
            Item.rewardCount = 5;
            Item.stackable = true;
            Item.throwDistance = 8;
            Item.throwExtraDist = true;
            Item.throwDamage = 5;
		}
	}
}