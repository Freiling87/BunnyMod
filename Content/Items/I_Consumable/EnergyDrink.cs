using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using BunnyMod.Content.Traits;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;

namespace BunnyMod.Content.Items.I_Consumable
{
	[ItemCategories(RogueCategories.Food, RogueCategories.Usable)]
	public class EnergyDrink : CustomItem
	{
		[RLSetup]
		public static void Setup()
		{
			RogueLibs.CreateCustomItem<EnergyDrink>()
					.WithDescription(new CustomNameInfo(""))
					.WithName(new CustomNameInfo(""))
					.WithSprite(Properties.Resources.EnergyDrink)
					.WithUnlock(new ItemUnlock
					{
						CharacterCreationCost = 3,
						LoadoutCost = 3,
						Prerequisites = { },
						UnlockCost = 3
					});
		}

		public override void SetupDetails()
		{
			Item.goesInToolbar = true;
			Item.healthChange = 5;
			Item.initCount = 1;
			Item.itemType = ItemTypes.Consumable;
			Item.itemValue = 20;
			Item.rewardCount = 3;
			Item.stackable = true;
		}

		[IgnoreChecks("FullHealth")]
		public bool UseItem()
		{
			if (Owner.electronic)
			{
				Owner.SayDialogue("");
				gc.audioHandler.Play(Owner, vAudioClip.CantDo);

				return false;
			}

			Owner.AddEffect<ChronomanticDilation>(15);
			gc.audioHandler.Play(Owner, vAudioClip.UseDrink);
			Count--;

			return true;
		}
	}
}
