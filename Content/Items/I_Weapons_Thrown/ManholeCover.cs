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
	[ItemCategories(RogueCategories.Weapons, RogueCategories.NotRealWeapons)]
	public class ManholeCover : CustomItem
	{
		[RLSetup]
		public static void Setup()
		{
			RogueLibs.CreateCustomItem<ManholeCover>()
					.WithName(new CustomNameInfo("Manhole Cover"))
					.WithDescription(new CustomNameInfo(
							"Wow, this seems a little large for a manhole, but I guess some people do go kinda crazy.\n\nOh, you mean those things in the ground..."))
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
}
