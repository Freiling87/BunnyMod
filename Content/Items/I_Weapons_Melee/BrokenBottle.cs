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
	[ItemCategories(RogueCategories.Weapons, RogueCategories.NotRealWeapons, RogueCategories.Melee, "Trash")]
	public class BrokenBottle : CustomItem
	{
		[RLSetup]
		public static void Setup()
		{
			RogueLibs.CreateCustomItem<BrokenBottle>()
					.WithName(new CustomNameInfo("Broken Bottle"))
					.WithDescription(new CustomNameInfo(
							"Hmm, what could you do with this? Recycle it? Make an art installation? It's probably too sharp for a windchime. Think about it."))
					.WithSprite(Properties.Resources.BrokenBottle)
					.WithUnlock(new ItemUnlock
					{
						CharacterCreationCost = 1,
						IsAvailable = true,
						IsAvailableInCC = true,
						IsAvailableInItemTeleporter = true,
						LoadoutCost = 1,
						Prerequisites = { cItem.WhiskeyBottle },
						UnlockCost = 1
					});
		}

		public override void SetupDetails()
		{
			Item.cantBeCloned = true;
			Item.hitSoundType = vAudioClip.MeleeHitAgentCutSmall2;
			Item.hasCharges = false;
			Item.initCount = 40;
			Item.isWeapon = false;
			Item.itemType = ItemTypes.WeaponMelee;
			Item.itemValue = 1;
			Item.meleeDamage = 4;
			Item.rewardCount = 40;
			Item.stackable = false;
			Item.weaponCode = weaponType.WeaponMelee;
		}
	}
}
