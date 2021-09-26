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
	public class Brick : CustomItem
	{
		[RLSetup]
		public static void Setup()
		{
			RogueLibs.CreateCustomItem<Brick>()
					.WithName(new CustomNameInfo("Brick"))
					.WithDescription(new CustomNameInfo(
							"Look around you. That wall? Brick. That fireplace? Brick. Everything is made of brick. Brick is the fundamental 'building block' of the universe. I think. I didn't do a lot of research on this."))
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
}
