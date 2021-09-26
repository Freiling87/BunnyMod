using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using BunnyMod.Content.Traits;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;

namespace BunnyMod.Content.Items.I_Passive
{
	[ItemCategories(RogueCategories.Passive, "Blessed")]
	public class HolySymbol : CustomItem
	{
		[RLSetup]
		public static void Setup()
		{
			RogueLibs.CreateCustomItem<HolySymbol>()
					.WithDescription(new CustomNameInfo("Shiny!\n\nHoly!\n\nThat's it!"))
					.WithName(new CustomNameInfo("Holy Symbol"))
					.WithSprite(Properties.Resources.HolySymbol)
					.WithUnlock(new ItemUnlock
					{
						CharacterCreationCost = 3,
						LoadoutCost = 3,
						UnlockCost = 5,
						Prerequisites = { }
					});
		}

		public override void SetupDetails()
		{
			Item.itemType = ItemTypes.Tool;
			Item.itemValue = 80;
			Item.cantBeCloned = true;
			Item.goesInToolbar = false;
		}
	}
}
