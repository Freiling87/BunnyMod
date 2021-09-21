using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using BunnyMod.Content.Traits;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;

namespace BunnyMod.Content.Items.I_Combineable
{
	[ItemCategories(RogueCategories.Drugs, RogueCategories.Technology, RogueCategories.Usable)]
	public class SugarProcessor : CustomItem, IItemCombinable
	{
		[RLSetup]
		public static void Setup()
		{
			RogueLibs.CreateCustomItem<SugarProcessor>()
					.WithDescription(new CustomNameInfo(
							"You can stick pretty much anything in here, and a bag of Sugar will pop out once it reaches a value threshold of 100."))
					.WithName(new CustomNameInfo(cItem.SugarProcessor))
					.WithSprite(Properties.Resources.SugarProcessor)
					.WithUnlock(new ItemUnlock
					{
						CharacterCreationCost = 4,
						IsAvailable = true,
						IsAvailableInCC = true,
						IsAvailableInItemTeleporter = true,
						LoadoutCost = 4,
						Prerequisites = { vItem.FoodProcessor, vItem.Sugar },
						UnlockCost = 4
					});
		}

		public override void SetupDetails()
		{
			Item.cantBeCloned = true;
			Item.hasCharges = true;
			Item.initCount = 0;
			Item.itemValue = 200;
			Item.itemType = ItemTypes.Combine;
			Item.rewardCount = 0;
			Item.stackable = true;
			Item.thiefCantSteal = true;
		}

		public bool CombineFilter(InvItem other) =>
				other.contents.Count > 0 &&
				other.itemValue > 0 &&
				other.invItemName != vItem.Money &&
				!other.cantDrop &&
				!other.questItem;

		public bool CombineItems(InvItem other)
		{
			if (!CombineFilter(other)) return false;

			if (Owner.agentInvDatabase.equippedWeapon == other)
				Owner.agentInvDatabase.UnequipWeapon();
			else if (Owner.agentInvDatabase.equippedArmor == other)
				Owner.agentInvDatabase.UnequipArmor();
			else if (Owner.agentInvDatabase.equippedArmorHead == other)
				Owner.agentInvDatabase.UnequipArmorHead();

			if (other.isArmor || other.isArmorHead || other.hasCharges ||
					(other.isWeapon && other.itemType != ItemTypes.WeaponThrown))
				Owner.agentInvDatabase.DestroyItem(other);
			else
				Owner.agentInvDatabase.SubtractFromItemCount(other, 1);

			int unitsMade = 0;
			int totalOutput = Item.invItemCount;
			totalOutput += Owner.determineMoneyCost(other, other.itemValue, vItem.BombProcessor) / 2;

			while (totalOutput >= 100)
			{
				totalOutput -= 100;
				unitsMade++;
			}

			Item.invItemCount = totalOutput;

			for (int j = 0; j < unitsMade; j++)
			{
				InvItem sugar = new InvItem();
				sugar.invItemName = vItem.Sugar;
				sugar.invItemCount = 1;
				sugar.contents.Clear();
				sugar.SetupDetails(false);

				Owner.inventory.AddItemOrDrop(sugar);
			}

			gc.audioHandler.Play(Owner, vAudioClip.CombineItem);

			return true;
		}

		public CustomTooltip CombineTooltip(InvItem other) => default;
		public CustomTooltip CombineCursorText(InvItem other) => default;
	}
}
