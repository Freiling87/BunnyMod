using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;

namespace BunnyMod
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    [BepInProcess("StreetsOfRogue.exe")]
    [BepInDependency(RogueLibs.pluginGuid, "1.0")]
    public class Class1 : BaseUnityPlugin
    {
        public const string pluginGuid = "freiling87.streetsofrogue.bunnymod";
        public const string pluginName = "Bunny Mod";
        public const string pluginVersion = "1.0.0.0";

        public void Awake()
		{
            RoguePatcher patcher = new RoguePatcher(this, GetType());

            Logger.LogInfo("Hello world!");

            #region Screwdriver

            //Sprite sprite = RogueUtilities.ConvertToSprite(Properties.Resources.Screwdriver);
            CustomItem screwdriver = RogueLibs.CreateCustomItem("Screwdriver", sprite, false,
                new CustomNameInfo("Screwdriver",
                    null, null, null, null, null, null, null),
                new CustomNameInfo("You aren't really reading this description, are you? Please tell me you know what a screwdriver is.",
                    null, null, null, null, null, null, null),
                item =>
				{
                    item.itemType = "Tool";
                    item.weaponCode = weaponType.WeaponMelee;

                    item.itemValue = 20;
                    item.isWeapon = true;
                    item.meleeDamage = 3;
                    item.hitSoundType = "Normal";
                    item.goesInToolbar = true;
                    item.canFix = true;
                    item.equipped = true;
                    
				});
            screwdriver.Prerequisites.Add("Wrench");

            screwdriver.UnlockCost = 3;
            screwdriver.CostInCharacterCreation = 3;
            screwdriver.CostInLoadout = 3;

            //screwdriver.Categories.Add("Technology"); //
            //screwdriver.Categories.Add("Weapons");
            //screwdriver.Categories.Add("NotRealWeapons");
            //screwdriver.Categories.Add("Technology");

            #endregion
        }
	}
}
