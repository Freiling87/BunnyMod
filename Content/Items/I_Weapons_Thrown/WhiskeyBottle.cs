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
	public class WhiskeyBottle : CustomItem
	{
		[RLSetup]
		public static void Setup()
		{
			RogueLibs.CreateCustomItem<WhiskeyBottle>()
					.WithName(new CustomNameInfo("Whiskey Bottle"))
					.WithDescription(new CustomNameInfo(
							"No, it's just the bottle, you see. Stop shaking it. No, don't put your tongue in it, you degenerate! Even I don't know where that's been. Just throw it at people like a normal person."))
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
