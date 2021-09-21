using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using BunnyMod.Content.Traits;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;

namespace BunnyMod.Content
{
	public class BMItems
	{
		[RLSetup]
		public static void InitializeNames()
		{
			string t;

			t = vNameType.Dialogue;
			_ = RogueLibs.CreateCustomName(cDialogue.CantDrinkAlcohol, t, new CustomNameInfo("Today, I choose not to drink."));
			_ = RogueLibs.CreateCustomName(cDialogue.CantEatMeat, t, new CustomNameInfo("Meat is murder!"));
			_ = RogueLibs.CreateCustomName(cDialogue.CantEatNonvegetarian, t, new CustomNameInfo("No! Me want meat!"));
			_ = RogueLibs.CreateCustomName(cDialogue.CantUseDrug, t, new CustomNameInfo("Nope, my body is a temple!"));
			_ = RogueLibs.CreateCustomName(cDialogue.CantUseLoud, t, new CustomNameInfo("I can't use that! It's too loooooud."));
			_ = RogueLibs.CreateCustomName(cDialogue.CantUseLoud_2, t, new CustomNameInfo("But that'll hurt my delicate little ears!"));
			_ = RogueLibs.CreateCustomName(cDialogue.CantUseBlunt, t, new CustomNameInfo("I need a sharper tool for this work."));
			_ = RogueLibs.CreateCustomName(cDialogue.CantUseSharp, t, new CustomNameInfo("Mommy says I can't use sharp things!"));
			_ = RogueLibs.CreateCustomName(cDialogue.CantUseSharp_2, t, new CustomNameInfo("I swore to draw no blood... unless I remove this trait first."));
			_ = RogueLibs.CreateCustomName(cDialogue.CantWearArmor, t, new CustomNameInfo("I'm too fuckin' fat to wear this!"));
			_ = RogueLibs.CreateCustomName(cDialogue.CantWearHeadgear, t, new CustomNameInfo("Owie! This is too tight for my big, fat, stupid, ugly head!"));
		}


		#region InvItem

		public void InvItem_00()
		{
			Postfix(typeof(InvItem), "SetupDetails", GetType(), "InvItem_SetupDetails", new Type[1] { typeof(bool) });
			Prefix(typeof(InvItem), "UseItem", GetType(), "InvItem_UseItem", new Type[0] { });
		}

		public static void InvItem_SetupDetails(bool notNew, InvItem __instance) // Postfix
		{
			string name = __instance.invItemName;

			if (__instance.Categories.Count == 0)
				__instance.Categories.Add("NullCatcher");

			if (__instance.Categories.Contains("Alcohol")) { }

			if (__instance.Categories.Contains("Drugs")) { }

			if (__instance.Categories.Contains("Food"))
			{
				if (vItem.nonVegetarian.Contains(name))
					__instance.Categories.Add("NonVegetarian");
				else if (vItem.vegetarian.Contains(name))
					__instance.Categories.Add("Vegetarian");
			}

			if (__instance.Categories.Contains("Weapons"))
			{
				if (vItem.blunt.Contains(name))
					__instance.Categories.Add("Blunt");
				if (vItem.explosive.Contains(name))
					__instance.Categories.Add("Explosive");
				if (vItem.loud.Contains(name) && !__instance.contents.Contains("Silencer"))
					__instance.Categories.Add("Loud");
				if (vItem.piercing.Contains(name))
					__instance.Categories.Add("Piercing");
			}

			return;
		}

		public static bool InvItem_UseItem(InvItem __instance) // Prefix
		{
			Agent agent = __instance.agent;
			List<string> cats = __instance.Categories;
			bool cantDoFlag = false;

			if (cats.Contains("Alcohol") && (agent.statusEffects.hasTrait(cTrait.FriendOfBill) || agent.statusEffects.hasTrait(cTrait.Teetotaller)))
			{
				BMHeaderTools.SayDialogue(agent, cDialogue.CantDrinkAlcohol, vNameType.Dialogue);
				cantDoFlag = true;
			}
			else if (cats.Contains("Drugs") && (agent.statusEffects.hasTrait(cTrait.DAREdevil) || agent.statusEffects.hasTrait(cTrait.Teetotaller)))
			{
				BMHeaderTools.SayDialogue(agent, cDialogue.CantUseDrug, vNameType.Dialogue);
				cantDoFlag = true;
			}
			else if (cats.Contains("NonVegetarian") && agent.statusEffects.hasTrait(cTrait.Vegetarian))
			{
				BMHeaderTools.SayDialogue(agent, cDialogue.CantEatMeat, vNameType.Dialogue);
				cantDoFlag = true;
			}
			else if (cats.Contains("Vegetarian") && agent.statusEffects.hasTrait(cTrait.Carnivore))
			{
				BMHeaderTools.SayDialogue(agent, cDialogue.CantEatNonvegetarian, vNameType.Dialogue);
				cantDoFlag = true;
			}
			else if (cats.Contains("Loud") && agent.statusEffects.hasTrait(cTrait.AfraidOfLoudNoises))
			{
				BMHeaderTools.SayDialogue(agent, cDialogue.CantUseLoud, "Dialogue");
				cantDoFlag = true;
			}
			else if (cats.Contains("Piercing") && agent.statusEffects.hasTrait(cTrait.DrawNoBlood))
			{
				BMHeaderTools.SayDialogue(agent, cDialogue.CantUseSharp_2, vNameType.Dialogue);
				cantDoFlag = true;
			}

			if (cantDoFlag)
			{
				GC.audioHandler.Play(agent, "CantDo");

				return false;
			}

			return true;
		}

		#endregion

		#region Item

		public void Item_00()
		{
			Prefix(typeof(Item), "SpecialDamage", GetType(), "Item_SpecialDamage", new Type[1] { typeof(PlayfieldObject) });
		}

		public static List<string> customThrowables = new List<string>()
		{
				cItem.Brick,
				cItem.BeerCan,
				cItem.Lunchbox,
				cItem.ManholeCover,
				cItem.Sawblade,
				cItem.ThrowingKnife,
				cItem.WhiskeyBottle
		};

		public static bool Item_SpecialDamage(PlayfieldObject damagerObject, Item __instance)
		{
			string itemName = __instance.invItem.invItemName;

			if (customThrowables.Contains(itemName) && damagerObject.CompareTag("Agent"))
			{
				Agent agent = (Agent) damagerObject;

				if (itemName == cItem.BeerCan)
					GC.audioHandler.Play(agent, vAudioClip.BulletHitObject);
				else if (itemName == cItem.ManholeCover)
					GC.audioHandler.Play(agent, vAudioClip.MeleeHitAgentLarge);
				else if (itemName == cItem.Sawblade)
					GC.audioHandler.Play(agent, vAudioClip.SawBladeHit);
				else if (itemName == cItem.ThrowingKnife)
					GC.audioHandler.Play(agent, vAudioClip.MeleeHitAgentCutSmall2);
				else if (itemName == cItem.WhiskeyBottle)
					GC.audioHandler.Play(agent, vAudioClip.WindowDamage);
				else
					GC.audioHandler.Play(agent, vAudioClip.MeleeHitAgentLarge);

				GC.spawnerMain.SpawnParticleEffect("ObjectDestroyed", __instance.tr.position, __instance.tr.eulerAngles.z);

				if (agent.inhuman || agent.mechFilled || agent.mechEmpty)
				{
					GC.spawnerMain.SpawnParticleEffect("BloodHitYellow", agent.tr.position, __instance.tr.eulerAngles.z);
					GC.playerAgent.objectMultPlayfield.SpawnParticleEffect("BloodHitYellow", agent.tr.position, __instance.tr.eulerAngles.z, false, agent);
				}
				else
				{
					GC.spawnerMain.SpawnParticleEffect("BloodHit", agent.tr.position, __instance.tr.eulerAngles.z);
					GC.playerAgent.objectMultPlayfield.SpawnParticleEffect("BloodHit", agent.tr.position, __instance.tr.eulerAngles.z, false, agent);
				}

				__instance.DestroyMeFromClient();

				return false;
			}

			return true;
		}

		#endregion

		#region ItemFunctions

		public void ItemFunctions_00()
		{
			Postfix(typeof(ItemFunctions), "DetermineHealthChange", GetType(), "ItemFunctions_DetermineHealthChange",
					new Type[2] { typeof(InvItem), typeof(Agent) });
			Prefix(typeof(ItemFunctions), "UseItem", GetType(), "ItemFunctions_UseItem", new Type[2] { typeof(InvItem), typeof(Agent) });
		}

		public static void ItemFunctions_DetermineHealthChange(InvItem item, Agent agent, ref int __result) // Postfix
		{
			List<string> cats = item.Categories;
			StatusEffects traits = agent.statusEffects;
			if
			(
					(cats.Contains("Alcohol") && (traits.hasTrait(cTrait.FriendOfBill) || traits.hasTrait(cTrait.Teetotaller))) ||
					(cats.Contains("Drugs") && (traits.hasTrait(cTrait.DAREdevil) || traits.hasTrait(cTrait.Teetotaller))) ||
					(cats.Contains("Vegetarian") && traits.hasTrait(cTrait.Carnivore)) ||
					(cats.Contains("NonVegetarian") && traits.hasTrait(cTrait.Vegetarian))
			)
				__result = 0;
			if (traits.hasTrait(cTrait.Fatass))
				__result = (int) ((float) __result * 1.5f);
		}

		public static bool ItemFunctions_UseItem(InvItem item, Agent agent, ItemFunctions __instance) // Prefix ***new
		{
			if (item.itemType == "Consumable")
			{
				if (vItem.alcohol.Contains(item.invItemName) &&
						((agent.statusEffects.hasTrait(cTrait.FriendOfBill) || agent.statusEffects.hasTrait(cTrait.Teetotaller))))
				{
					BMHeaderTools.SayDialogue(agent, cDialogue.CantDrinkAlcohol, vNameType.Dialogue);
					goto terminus;
				}
				else if (vItem.drugs.Contains(item.invItemName) &&
						(agent.statusEffects.hasTrait(cTrait.DAREdevil) || agent.statusEffects.hasTrait(cTrait.Teetotaller)))
				{
					BMHeaderTools.SayDialogue(agent, cDialogue.CantUseDrug, vNameType.Dialogue);
					goto terminus;
				}
				else if (vItem.nonVegetarian.Contains(item.invItemName) && agent.statusEffects.hasTrait(cTrait.Vegetarian))
				{
					BMHeaderTools.SayDialogue(agent, cDialogue.CantEatMeat, vNameType.Dialogue);
					goto terminus;
				}
				else if (vItem.vegetarian.Contains(item.invItemName) && agent.statusEffects.hasTrait(cTrait.Carnivore))
				{
					BMHeaderTools.SayDialogue(agent, cDialogue.CantEatNonvegetarian, vNameType.Dialogue);
					goto terminus;
				}
			}
			else
			{
				if (vItem.loud.Contains(item.invItemName) && agent.statusEffects.hasTrait(cTrait.AfraidOfLoudNoises))
				{
					BMHeaderTools.SayDialogue(agent, cDialogue.CantUseLoud, "Dialogue");
					goto terminus;
				}

				if (vItem.piercing.Contains(item.invItemName) && agent.statusEffects.hasTrait(cTrait.DrawNoBlood))
				{
					BMHeaderTools.SayDialogue(agent, cDialogue.CantUseSharp_2, vNameType.Dialogue);
					goto terminus;
				}
			}

			return true;

			terminus:

			GC.audioHandler.Play(agent, "CantDo");

			return false;
		}

		#endregion
	}










}