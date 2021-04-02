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
        }
        public static void InitializeItems()
		{

        }
		#endregion

		#region ItemFunctions
		public static void ItemFunctions_DetermineHealthChange(InvItem item, Agent agent) // Postfix
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
        public static void Melee_Attack(bool specialAbility, Melee __instance) // Postfix
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