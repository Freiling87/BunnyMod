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
	[ItemCategories(RogueCategories.Weapons, RogueCategories.NotRealWeapons, "Trash")]
	public class BeerCan : CustomItem
	{
		[RLSetup]
		public static void Setup()
		{
			RogueLibs.CreateCustomItem<BeerCan>()
					.WithName(new CustomNameInfo("Beer Can"))
					.WithDescription(
							new CustomNameInfo(
									"No, you can't crack someone's skull with it. It won't break windows. And the worst part: there's no beer in it."))
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
}
