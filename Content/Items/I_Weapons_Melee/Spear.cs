using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using BunnyMod.Content.Traits;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;

namespace BunnyMod.Content.Items.I_Weapons_Melee
{
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
}
