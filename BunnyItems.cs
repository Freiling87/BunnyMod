using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using BepInEx;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;

namespace BunnyMod
{
    public class BunnyItems 
    {
		#region Generic
		public void Awake()
		{
            InitializeItems();

            #region Patches - Item Base
            BunnyHeader.MainInstance.PatchPostfix(typeof(ItemFunctions), "DetermineHealthChange", GetType(), "ItemFunctions_DetermineHealthChange", new Type[2] { typeof(InvItem), typeof(Agent) });

            BunnyHeader.MainInstance.PatchPostfix(typeof(Melee), "Attack", GetType(), "Melee_Attack", new Type[1] { typeof(bool) });
            #endregion
            #region Patches - Items

            #endregion
        }
        public static void InitializeItems_Inactive()
		{
            Sprite sprite_beerCan = RogueUtilities.ConvertToSprite(Properties.Resources.BeerCan);
            CustomItem BeerCan = RogueLibs.CreateCustomItem("BeerCan", sprite_beerCan, true,
                new CustomNameInfo("Beer Can"),
                new CustomNameInfo("Probably best just to get the money from recycling this."),
                item =>
                {
                    item.itemType = "WeaponThrown";
                    item.weaponCode = weaponType.WeaponThrown;
                    item.Categories.Add("Weapons");
                    item.Categories.Add("NonViolent");
                    item.isWeapon = true;
                    item.itemValue = 5;
                    item.initCount = 1;
                    item.rewardCount = 4;
                    item.stackable = true;
                    item.throwDamage = 1;
                    item.throwDistance = 4;
                    item.throwExtraDist = false;
                    item.dontAutomaticallySelect = true;
                    item.specialDamage = true;
                    item.shadowOffset = 4;
                });

            Sprite sprite_spear = RogueUtilities.ConvertToSprite(Properties.Resources.Spear);
            CustomItem Spear = RogueLibs.CreateCustomItem("Spear", sprite_spear, false,
                new CustomNameInfo("Spear"),
                new CustomNameInfo("You've probably seen Cavemen hunting your mom with these."),
                item =>
                {
                    item.itemType = "WeaponMelee";
                    item.weaponCode = weaponType.WeaponMelee;
                    item.Categories.Add("Weapons");
                    item.isWeapon = true;
                    item.itemValue = 30;
                    item.initCount = 80;
                    item.rewardCount = 200;
                    item.stackable = true;
                    item.meleeDamage = 8;
                    item.hitSoundType = "Cut";
                });
            Spear.CostInCharacterCreation = 1;
            Spear.CostInLoadout = 1;
            Spear.UnlockCost = 3;

            // Patch Melee.Attack() for these variables, see those for Knife.

            Sprite sprite_whiskeyBottle = RogueUtilities.ConvertToSprite(Properties.Resources.WhiskeyBottle);
            CustomItem WhiskeyBottle = RogueLibs.CreateCustomItem("WhiskeyBottle", sprite_whiskeyBottle, true,
                new CustomNameInfo("Whiskey Bottle"),
                new CustomNameInfo("Probably best just to get the money from recycling this."),
                item =>
                {
                    item.itemType = "WeaponThrown";
                    item.weaponCode = weaponType.WeaponThrown;
                    item.Categories.Add("Weapons");
                    item.Categories.Add("NonViolent");
                    item.isWeapon = true;
                    item.itemValue = 5;
                    item.initCount = 1;
                    item.rewardCount = 4;
                    item.stackable = true;
                    item.throwDamage = 5;
                    item.throwDistance = 8;
                    item.throwExtraDist = false;
                    item.dontAutomaticallySelect = true;
                    item.specialDamage = true;
                    item.shadowOffset = 4;
                });
            // TODO: See if there's a way to make this impact louder than a normal projectile
            // See Explosion.ExplosionHit() , with this.initialHitGlass = true;
        }
        public static void InitializeItems()
		{
        }
		#endregion

		#region ItemFunctions
		public static void ItemFunctions_DetermineHealthChange(InvItem item, Agent agent)
        {
            if (item.invItemName == "Beer")
            {
                agent.inventory.AddItem("BeerCan", 1);
            }
            if (item.invItemName == "Whiskey")
            {
                agent.inventory.AddItem("WhiskeyBottle", 1);
            }
        }
        #endregion
        #region Melee
        public static void Melee_Attack(bool specialAbility, Melee __instance)
        {
            InvItem invItem = (specialAbility ? __instance.agent.inventory.equippedSpecialAbility : __instance.agent.inventory.equippedWeapon) ?? __instance.agent.inventory.fist;

            bool flag2 = __instance.specialLunge; // TODO: Find out how to attach this to Spear

            if (invItem.invItemName == "Spear")
            {
                __instance.SetWeaponCooldown(2f);
                __instance.meleeContainerAnim.speed = 3f;
                __instance.meleeContainerAnim.Play("Melee-Knife", -1, 0f);
                __instance.meleeHitbox.boxColliderDefaultSizeX = 0.16f; //
                __instance.meleeHitbox.boxColliderDefaultSizeY = 0.36f; //
                __instance.meleeHitbox.boxColliderDefaultOffsetX = 0f; //
                __instance.meleeHitbox.boxColliderDefaultOffsetY = -0.02f; //
                //__instance.agent.movement.KnockForward(__instance.agent.tr.rotation, num3, true); //
                __instance.canMove = false; //
                __instance.realArm1.enabled = false; //
                if (!flag2)
                    __instance.gc.audioHandler.Play(__instance.agent, "SwingWeaponLarge");
                __instance.hitParticlesTr.localPosition = new Vector3(0.3f, 0f, 0f);
                __instance.animClass = "Stab";
            }
            if (flag2)
            {
                __instance.animClass += "Lunge";
            }
        }
        #endregion
        #region PlayfieldObject
        public static void PlayfieldObject_FindDamage() // 
        {
        }//I think this is for Spear & Beer Can
		#endregion
    }
}