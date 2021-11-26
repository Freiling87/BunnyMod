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
}
