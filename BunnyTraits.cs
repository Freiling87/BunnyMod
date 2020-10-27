using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using BepInEx;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;

/*
    IsActive is the state of the unlock in the Rewards/Traits/Mutators menu
    Available determines whether the unlock will be available in the menu and in the game
    if Available is set to false, you won't find anything about that unlock in the game; if set to true you can find it in the Rewards/Traits/Mutators menu
    if IsActive is set to true, you'll be able to find that unlock while playing; if set to false you won't
 */

namespace BunnyMod
{
	class BunnyTraits
	{
		#region Generic
		public void Awake()
		{
            Initialize_Names();
            Initialize_Traits();

            BunnyHeader.MainInstance.PatchPrefix(typeof(ItemFunctions), "UseItem", GetType(), "ItemFunctions_UseItem", new Type[2] { typeof(InvItem), typeof(Agent) });

            BunnyHeader.MainInstance.PatchPostfix(typeof(PlayfieldObject), "DetermineLuck", GetType(), "PlayfieldObject_DetermineLuck", new Type[3] { typeof(int), typeof(string), typeof(bool) });
        }
        public static void Initialize_Names()
		{
        }
        public static void Initialize_Traits_Inactive()
		{
            CustomTrait Alcoholic = RogueLibs.CreateCustomTrait("Alcoholic", true,
                new CustomNameInfo("Alcoholic"),
                new CustomNameInfo("")); //
            Alcoholic.AvailableInCharacterCreation = false; //
            Alcoholic.CostInCharacterCreation = -6;
            Alcoholic.IsActive = false; //
            Alcoholic.Available = false; //
            Alcoholic.Upgrade = null;

            CustomTrait ArtificialInsermonation = RogueLibs.CreateCustomTrait("ArtificialInsermonation", true,
                new CustomNameInfo("Artificial Insermonation"),
                new CustomNameInfo("Activate an Altar to deliver a Sermon, randomly improving relations with NPCs within earshot. They may donate tithes."));
            ArtificialInsermonation.AvailableInCharacterCreation = false; //
            ArtificialInsermonation.CostInCharacterCreation = 2;
            ArtificialInsermonation.IsActive = false; //
            ArtificialInsermonation.Available = false; //
            ArtificialInsermonation.Upgrade = "ArtificialInsermonation_2";

            CustomTrait ArtificialInsermonation_2 = RogueLibs.CreateCustomTrait("ArtificialInsermonation_2", true,
                new CustomNameInfo("Artificial Insermonation +"),
                new CustomNameInfo("Improved relationships and tithes from Sermonizing."));
            ArtificialInsermonation_2.AvailableInCharacterCreation = false; //
            ArtificialInsermonation_2.CostInCharacterCreation = 2;
            ArtificialInsermonation_2.IsActive = false; //
            ArtificialInsermonation_2.Available = false; //
            ArtificialInsermonation_2.Upgrade = null;

            CustomTrait CheekyTrappy = RogueLibs.CreateCustomTrait("CheekyTrappy", true,
                new CustomNameInfo("Cheeky Trappy"),
                new CustomNameInfo("All hidden traps are visible to you. NPCs will no longer avoid your traps."));
            CheekyTrappy.AvailableInCharacterCreation = false; //
            CheekyTrappy.CostInCharacterCreation = 1;
            CheekyTrappy.IsActive = false; //
            CheekyTrappy.Available = false; //
            CheekyTrappy.Upgrade = null;

            CustomTrait DeathToSnitches = RogueLibs.CreateCustomTrait("DeathToSnitches", true,
                new CustomNameInfo("Death To Snitches"),
                new CustomNameInfo("Cops will ignore your Pusher attempts. You may attempt to sell to Cops, but failure will turn them hostile."));
            DeathToSnitches.AvailableInCharacterCreation = false; //
            DeathToSnitches.IsActive = false; //
            DeathToSnitches.Available = false; //
            DeathToSnitches.Upgrade = null;
            //TODO: Unlock DeathToSnitches when Pusher gained

            CustomTrait DrawNoBlood = RogueLibs.CreateCustomTrait("DrawNoBlood", true,
                new CustomNameInfo("Draw No Blood"),
                new CustomNameInfo("You have taken an oath. You cannot use bullet-based guns or sharp weapons."));
            DrawNoBlood.AvailableInCharacterCreation = false; //
            DrawNoBlood.CanRemove = false;
            DrawNoBlood.CanSwap = false;
            DrawNoBlood.CostInCharacterCreation = -3;
            DrawNoBlood.IsActive = false; //
            DrawNoBlood.Available = false; //
            DrawNoBlood.Upgrade = null;

            CustomTrait HungryBoy = RogueLibs.CreateCustomTrait("HungryBoy", true,
                new CustomNameInfo("Hungry Boy"),
                new CustomNameInfo("Less healing from food and alcohol."));
            HungryBoy.AvailableInCharacterCreation = false; //
            HungryBoy.CostInCharacterCreation = -3;
            HungryBoy.IsActive = false; //
            HungryBoy.Available = false; //
            HungryBoy.Upgrade = null;

            CustomTrait OneHappyTamper = RogueLibs.CreateCustomTrait("OneHappyTamper", true,
                new CustomNameInfo("One Happy Tamper"),
                new CustomNameInfo("Owners will allow you to tamper with their belongings."));
            OneHappyTamper.AvailableInCharacterCreation = false; //
            OneHappyTamper.CostInCharacterCreation = 2;
            OneHappyTamper.IsActive = false; //
            OneHappyTamper.Available = false; //
            OneHappyTamper.Upgrade = null;

            CustomTrait PursuitOfTrappiness = RogueLibs.CreateCustomTrait("PursuitOfTrappiness", true,
                new CustomNameInfo("Pursuit Of Trappiness"),
                new CustomNameInfo("Un-Aligned NPCs take additional damage from Traps."));
            PursuitOfTrappiness.AvailableInCharacterCreation = false; //
            PursuitOfTrappiness.CostInCharacterCreation = 2;
            PursuitOfTrappiness.IsActive = false; //
            PursuitOfTrappiness.Available = false; //
            PursuitOfTrappiness.Upgrade = null;

            CustomTrait Pusher = RogueLibs.CreateCustomTrait("Pusher", true,
                new CustomNameInfo("Pusher"),
                new CustomNameInfo("You can interact with most NPCs to attempt to sell them any drug item you have (or simplified, just Sugar). If you fail, they become annoyed (Upper Crusters will call the cops immediately). Cops who witness a dealing attempt will go Hostile. If you succeed at a sale, they have a chance to become Hooked. After a certain interval of withdrawal, NPCs will gain the Jonesing status. They'll seek you out in the level and beg you for a particular drug. If you go too long without selling them the requested drug, they'll go hostile, but selling them other types of drugs will keep them at bay for a while. When Jonesing, they will freely give you keys and safe combos if you ask. Jonesing NPCs may also attack other drug dealers, doctors, or scientists if they can't track you down."));//
            Pusher.AvailableInCharacterCreation = false; //
            Pusher.CanRemove = false;
            Pusher.CanSwap = false;
            Pusher.CostInCharacterCreation = 6;
            Pusher.IsActive = false; //
            Pusher.Available = false; //
            Pusher.Upgrade = "Pusher_2";
            //TODO: Unlock DeathToSnitches when Pusher gained

            CustomTrait Pusher_2 = RogueLibs.CreateCustomTrait("Pusher_2", true,
                new CustomNameInfo("Pusher +"),
                new CustomNameInfo("Increased chances of success when pushing drugs."));//
            Pusher_2.AvailableInCharacterCreation = false; //
            Pusher_2.IsActive = false; //
            Pusher_2.Available = false; //
            Pusher_2.Upgrade = null;

            CustomTrait ReturnToBonke = RogueLibs.CreateCustomTrait("ReturnToBonke", true,
                new CustomNameInfo("Return to Bonke"),
                new CustomNameInfo("Chance to inflict Dizziness when striking an NPC with a blunt weapon."));
            ReturnToBonke.AvailableInCharacterCreation = false; //
            ReturnToBonke.CostInCharacterCreation = 3;
            ReturnToBonke.IsActive = false; //
            ReturnToBonke.Available = false; //
            ReturnToBonke.Upgrade = null;

            CustomTrait SuffersToolsGladly = RogueLibs.CreateCustomTrait("SuffersToolsGladly", true,
                new CustomNameInfo("Suffers Tools Gladly"),
                new CustomNameInfo("Your tools take less wear from tampering."));
            SuffersToolsGladly.AvailableInCharacterCreation = false; //
            SuffersToolsGladly.CostInCharacterCreation = 1;
            SuffersToolsGladly.IsActive = false; //
            SuffersToolsGladly.Available = false; //
            SuffersToolsGladly.Upgrade = "SticklerForTools_2";

            CustomTrait SuffersToolsGladly_2 = RogueLibs.CreateCustomTrait("SuffersToolsGladly_2", true,
                new CustomNameInfo("Suffers Tools Gladly +"),
                new CustomNameInfo("Your tools take zero wear when used in tampering."));
            SuffersToolsGladly_2.AvailableInCharacterCreation = false;
            SuffersToolsGladly_2.IsActive = false;//
            SuffersToolsGladly_2.Available = false; //
            SuffersToolsGladly_2.Upgrade = null;

            CustomTrait TrapperKeeper = RogueLibs.CreateCustomTrait("TrapperKeeper", true,
                new CustomNameInfo("Trapper Keeper"),
                new CustomNameInfo("Interact with Traps to add them to your inventory. 100% chance to disarm Door Detonators."));
            TrapperKeeper.AvailableInCharacterCreation = false; //
            TrapperKeeper.CostInCharacterCreation = 2;
            TrapperKeeper.IsActive = false; //
            TrapperKeeper.Available = false; //
            TrapperKeeper.Upgrade = null;

            CustomTrait UndeadBane = RogueLibs.CreateCustomTrait("UndeadBane", true,
                new CustomNameInfo("Undead Bane"),
                new CustomNameInfo("The undead fear and hate you. They're probably just jealous. All Vampires, Zombies & Ghosts are hostile on sight."));
            UndeadBane.AvailableInCharacterCreation = false; //
            UndeadBane.CostInCharacterCreation = -4;
            UndeadBane.IsActive = false; //
            UndeadBane.Available = false; //
            UndeadBane.Upgrade = null;

            CustomTrait VeiledThreats = RogueLibs.CreateCustomTrait("VeiledThreats", true,
                new CustomNameInfo("Veiled Threats"),
                new CustomNameInfo("When you attempt to Bribe, Extort, Mug, or Threaten, a failure will turn the target Annoyed instead of Hostile."));
            VeiledThreats.AvailableInCharacterCreation = false; //
            VeiledThreats.CostInCharacterCreation = 3;
            VeiledThreats.IsActive = false; //
            VeiledThreats.Available = false; //
            VeiledThreats.Upgrade = null;

            CustomTrait Whiffist = RogueLibs.CreateCustomTrait("Whiffist", true,
                new CustomNameInfo("Whiffist"),
                new CustomNameInfo("Small chance for Melee or Thrown attacks to miss you completely."));
            Whiffist.AvailableInCharacterCreation = false; //
            Whiffist.CostInCharacterCreation = 3;
            Whiffist.IsActive = false; //
            Whiffist.Available = false; //
            Whiffist.Upgrade = "Whiffist_2";
        }
        public static void Initialize_Traits()
        {
            CustomTrait Carnivore = RogueLibs.CreateCustomTrait("Carnivore", true,
                new CustomNameInfo("Carnivore"),
                new CustomNameInfo("'Meeeeeeat,' you grunt enthusiastically."));
            Carnivore.Available = true;
            Carnivore.AvailableInCharacterCreation = true;
            Carnivore.CanRemove = true;
            Carnivore.CanSwap = false;
            Carnivore.Conflicting.AddRange(new string[] { "BananaLover", "Vegetarian" });
            Carnivore.CostInCharacterCreation = -1;
            Carnivore.IsActive = true;
            Carnivore.Upgrade = null;

            CustomTrait Charmed = RogueLibs.CreateCustomTrait("Charmed", true,
                new CustomNameInfo("Charmed & Dangerous"),
                new CustomNameInfo("You once found a fourteen-leaf clover."));
            Charmed.Available = true;
            Charmed.AvailableInCharacterCreation = true;
            Charmed.CanRemove = false;
            Charmed.CanSwap = true;
            Charmed.Conflicting.AddRange(new string[] { "Charmed_2", "Cursed", "Cursed_2" });
            Charmed.CostInCharacterCreation = 3;
            Charmed.IsActive = true;
            Charmed.Upgrade = "Charmed_2";

            CustomTrait Charmed_2 = RogueLibs.CreateCustomTrait("Charmed_2", true,
                new CustomNameInfo("Charmed to the Teeth"),
                new CustomNameInfo("You are *really* lucky. Anyone who's been at the urinal next to you can attest."));
            Charmed_2.Available = true;
            Charmed_2.AvailableInCharacterCreation = true;
            Charmed_2.CanRemove = false;
            Charmed_2.CanSwap = true;
            Charmed_2.Conflicting.AddRange(new string[] { "Charmed", "Cursed", "Cursed_2" });
            Charmed_2.CostInCharacterCreation = 6;
            Charmed_2.Upgrade = null;

            CustomTrait Cursed = RogueLibs.CreateCustomTrait("Cursed", true,
                new CustomNameInfo("Your own Cursed Enemy"),
                new CustomNameInfo("You pissed in some old Gypsy lady's cereal, and you still refuse to apologize. She didn't like that."));
            Cursed.Available = true;
            Cursed.AvailableInCharacterCreation = true;
            Cursed.CanRemove = true;
            Cursed.CanSwap = false;
            Cursed.Conflicting.AddRange(new string[] { "Cursed_2", "Charmed", "Charmed_2" });
            Cursed.CostInCharacterCreation = -2;
            Cursed.IsActive = false;
            Cursed.Upgrade = null;

            CustomTrait Cursed_2 = RogueLibs.CreateCustomTrait("Cursed_2", true,
            new CustomNameInfo("First in Cursed"),
            new CustomNameInfo("You bought up an old Indian graveyard, and there you built a black cat sanctuary and mirror-breakery. Not your best choice."));
            Cursed_2.Available = true;
            Cursed_2.AvailableInCharacterCreation = true;
            Cursed_2.CanRemove = true;
            Cursed_2.CanSwap = false;
            Cursed_2.Conflicting.AddRange(new string[] { "Cursed", "Charmed", "Charmed_2" });
            Cursed_2.CostInCharacterCreation = -4;
            Cursed_2.IsActive = false;
            Cursed_2.Upgrade = null;

            CustomTrait DAREdevil = RogueLibs.CreateCustomTrait("DAREdevil", true,
                new CustomNameInfo("DAREdevil"),
                new CustomNameInfo("You have injected zero marijuanas. Crack is Whack. Smokers are Jokers. Needles are for... beetles."));
            DAREdevil.Available = true;
            DAREdevil.AvailableInCharacterCreation = true;
            DAREdevil.CanRemove = true;
            DAREdevil.CanSwap = false;
            DAREdevil.Conflicting.AddRange(new string[] { "Addict", "FriendOfBill", "Teetotaller" });
            DAREdevil.CostInCharacterCreation = -3;
            DAREdevil.IsActive = true;
            DAREdevil.Upgrade = null;

            CustomTrait FriendOfBill = RogueLibs.CreateCustomTrait("FriendofBill", true,
                new CustomNameInfo("Friend Of Bill"),
                new CustomNameInfo("You are taking things one day at a time. But every day sucks when you can't get drunk anymore."));
            FriendOfBill.Available = true;
            FriendOfBill.AvailableInCharacterCreation = true;
            FriendOfBill.CanRemove = true;
            FriendOfBill.CanSwap = false;
            FriendOfBill.Conflicting.AddRange(new string[] { "Addict", "DAREdevil", "Teetotaller" });
            FriendOfBill.CostInCharacterCreation = -1;
            FriendOfBill.IsActive = true;
            FriendOfBill.Upgrade = null;

			CustomTrait StealthBastardDeluxe = RogueLibs.CreateCustomTrait("StealthBastardDeluxe", true,
				new CustomNameInfo("Stealth Bastard Deluxe"),
				new CustomNameInfo("Allows you to hide in Bathtubs, Plants, Pool Tables, and Big Tables. [Bug: If you get stuck between it and the wall, you might clip through the wall]"));
			StealthBastardDeluxe.Available = true;
			StealthBastardDeluxe.AvailableInCharacterCreation = true;
			StealthBastardDeluxe.CanRemove = false;
			StealthBastardDeluxe.CanRemove = true;
			StealthBastardDeluxe.Conflicting.AddRange(new string[] { "Loud" });
			StealthBastardDeluxe.CostInCharacterCreation = 4;
			StealthBastardDeluxe.IsActive = true;
			StealthBastardDeluxe.Upgrade = null;

			CustomTrait Teetotaller = RogueLibs.CreateCustomTrait("Teetotaller", true,
                new CustomNameInfo("Teetotaller"),
                new CustomNameInfo("Wow, you're really boring. You don't do drugs *or* alcohol. What do you even do?"));
            Teetotaller.Available = true;
            Teetotaller.AvailableInCharacterCreation = true;
            Teetotaller.CanRemove = true;
            Teetotaller.CanSwap = false;
            Teetotaller.Conflicting.AddRange(new string[] { "Addict", "DAREdevil", "FriendOfBill" });
            Teetotaller.CostInCharacterCreation = -4;
            Teetotaller.IsActive = true;
            Teetotaller.Upgrade = null;

            CustomTrait Vegetarian = RogueLibs.CreateCustomTrait("Vegetarian", true,
                new CustomNameInfo("Vegetarian"),
                new CustomNameInfo("You are one of those people."));
            Vegetarian.Available = true;
            Vegetarian.AvailableInCharacterCreation = true;
            Vegetarian.CanRemove = true;
            Vegetarian.CanSwap = true;
            Vegetarian.Conflicting.AddRange(new string[] { "BloodRestoresHealth", "CannibalizeRestoresHealth", "Carnivore", "Zombify" });
            Vegetarian.CostInCharacterCreation = -1;
            Vegetarian.IsActive = false;
            Vegetarian.Available = false;
            Vegetarian.Upgrade = null;
        }
		#endregion

		#region ItemFunctions
        public static bool ItemFunctions_UseItem(InvItem item, Agent agent)
		{
            string itemName = item.invItemName;
            List<string> alcohol = new List<string>() { "Beer", "Cocktail", "Whiskey" };
            List<string> drugs = new List<string>() { "Antidote", "Cigarettes", "Cocaine", "CritterUpper", "CyanidePill", "ElectroPill", "Giantizer", "KillerThrower", "RagePoison", "Shrinker", "Steroids", "Syringe" };
            List<string> meats = new List<string>() { "BaconCheeseburger", "HamSandwich" };
            List<string> nonMeats = new List<string>() { "Banana", "Fud", "HotFud" };

            if (nonMeats.Contains(itemName) && agent.statusEffects.hasTrait("Carnivore"))
            {
                agent.Say("No! Me want meat!");
                BunnyHeader.gc.audioHandler.Play(agent, "CantDo");
                return false;
            }
            if (drugs.Contains(itemName) && (agent.statusEffects.hasTrait("DAREdevil") || agent.statusEffects.hasTrait("Teetotaller")))
			{
                agent.Say("Nope, my body is a temple!");
                BunnyHeader.gc.audioHandler.Play(agent, "CantDo");
                return false;
            }
            if (alcohol.Contains(itemName) && (agent.statusEffects.hasTrait("FriendOfBill") || agent.statusEffects.hasTrait("Teetotaller")))
			{
                agent.Say("Today, I choose not to drink.");
                BunnyHeader.gc.audioHandler.Play(agent, "CantDo");
                return false;
            }
            if (meats.Contains(itemName) && agent.statusEffects.hasTrait("Vegetarian"))
			{
                agent.Say("Meat is murder!");
                BunnyHeader.gc.audioHandler.Play(agent, "CantDo");
                return false;
			}
            return true;
		}
		#endregion
		#region PlayfieldObject
		public static void PlayfieldObject_DetermineLuck(int originalLuck, string luckType, bool cancelStatusEffects, PlayfieldObject __instance, ref int __result) // Postfix
		{
            int modifier = 0;

            if (luckType == "FreeShopItem2")
                modifier = 10;
            if (luckType == "DestroyGravestone")
                modifier = -5;
            if (luckType == "TurnTables")
                modifier = 10;
            if (luckType == "Joke")
                modifier = 10;
            if (luckType == "CritChance")
                modifier = 3;
            if (luckType == "ChanceAttacksDoZeroDamage")
                modifier = 4;
            if (luckType == "DoorDetonator")
                modifier = 10;
            if (luckType == "FreeShopItem")
                modifier = 10;
            if (luckType == "FindThreat")
                modifier = 8;
            if (luckType == "FindAskMayorHatPercentage")
                modifier = 8;
            if (luckType == "ChanceToKnockWeapons")
                modifier = 5;
            if (luckType == "SlotMachine")
                modifier = 8;
            if (luckType == "AttacksDamageAttacker")
                modifier = 10;
            if (luckType == "Hack")
                modifier = 10;
            if (luckType == "GunAim")
                modifier = 5;
            if (luckType == "SecurityCam")
                modifier = 10;
            if (luckType == "FindAskPercentage")
                modifier = 8;
            if (luckType == "ThiefToolsMayNotSubtract")
                modifier = 10;
            if (luckType == "CanceToSlowEnemies")
                modifier = 4;

            if (__instance.playfieldObjectAgent.statusEffects.hasTrait("Charmed"))
                __result += modifier * 1;
            else if (__instance.playfieldObjectAgent.statusEffects.hasTrait("Charmed_2"))
                __result += modifier * 2;
            else if (__instance.playfieldObjectAgent.statusEffects.hasTrait("Cursed"))
                __result += modifier * -1;
            else if (__instance.playfieldObjectAgent.statusEffects.hasTrait("Cursed_2"))
                __result += modifier * -2;

            if (__result > 100)
                __result = 100;
            else if (__result < 0)
                __result = 0;
        }
		#endregion
	}
}
