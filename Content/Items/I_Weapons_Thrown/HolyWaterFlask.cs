using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using BunnyMod.Content.Traits;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;

namespace BunnyMod.Content.Items.I_Weapons_Thrown
{
	[ItemCategories(RogueCategories.Weapons, RogueCategories.NotRealWeapons, "Blessed")]
	public class HolyWaterFlask : CustomItem
	{
		[RLSetup]
		public static void Setup()
		{
			RogueLibs.CreateCustomItem<BeerCan>()
					.WithName(new CustomNameInfo("Holy Water Flask"))
					.WithDescription(
							new CustomNameInfo(
									"It's soapy water. An old man mumbled over it. Vampires and Zombies seem to take it seriously though, so whatever."))
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
}
