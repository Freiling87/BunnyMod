﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using BepInEx;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;

/*
    IsActive is the state of the unlock in the Rewards/Traits/Mutators menu
    if IsActive is set to true, you'll be able to find that unlock while playing; if set to false you won't

    Available determines whether the unlock will be available in the menu and in the game
    if Available is set to false, you won't find anything about that unlock in the game; if set to true you can find it in the Rewards/Traits/Mutators menu
    For upgrade traits, set Available to false.
 */

namespace BunnyMod
{
    public enum DamageType
	{
        burnedFingers,
        brokenWindow
	}

    public class BunnyTraits
    {
        #region Main
        public void Awake()
        {
            Initialize_Names();
            Initialize_Traits_Active();

            BunnyHeader.MainInstance.PatchPrefix(typeof(AgentInteractions), "AddButton", GetType(), "AgentInteractions_AddButton", new Type[3] { typeof(string), typeof(int), typeof(string) });

            BunnyHeader.MainInstance.PatchPostfix(typeof(InvDatabase), "DetermineIfCanUseWeapon", GetType(), "InvDatabase_DetermineIfCanUseWeapon", new Type[1] { typeof(InvItem) });
            BunnyHeader.MainInstance.PatchPrefix(typeof(InvDatabase), "EquipArmor", GetType(), "InvDatabase_EquipArmor", new Type[2] { typeof(InvItem), typeof(bool) });
            BunnyHeader.MainInstance.PatchPrefix(typeof(InvDatabase), "EquipArmorHead", GetType(), "InvDatabase_EquipArmorHead", new Type[2] { typeof(InvItem), typeof(bool) });
            BunnyHeader.MainInstance.PatchPrefix(typeof(InvDatabase), "EquipWeapon", GetType(), "InvDatabase_EquipWeapon", new Type[2] { typeof(InvItem), typeof(bool) });
            BunnyHeader.MainInstance.PatchPrefix(typeof(InvDatabase), "SubtractFromItemCount", GetType(), "InvDatabase_SubtractFromItemCount_a", new Type[3] { typeof(int), typeof(int), typeof(bool) });
            BunnyHeader.MainInstance.PatchPrefix(typeof(InvDatabase), "SubtractFromItemCount", GetType(), "InvDatabase_SubtractFromItemCount_b", new Type[3] { typeof(InvItem), typeof(int), typeof(bool) });

            BunnyHeader.MainInstance.PatchPostfix(typeof(InvItem), "SetupDetails", GetType(), "InvItem_SetupDetails", new Type[1] { typeof(bool) });
            BunnyHeader.MainInstance.PatchPrefix(typeof(InvItem), "UseItem", GetType(), "InvItem_UseItem", new Type[0] { });

            BunnyHeader.MainInstance.PatchPostfix(typeof(ItemFunctions), "DetermineHealthChange", GetType(), "ItemFunctions_DetermineHealthChange", new Type[2] { typeof(InvItem), typeof(Agent) });
            BunnyHeader.MainInstance.PatchPrefix(typeof(ItemFunctions), "UseItem", GetType(), "ItemFunctions_UseItem", new Type[2] { typeof(InvItem), typeof(Agent) });

            BunnyHeader.MainInstance.PatchPostfix(typeof(PlayfieldObject), "DetermineLuck", GetType(), "PlayfieldObject_DetermineLuck", new Type[3] { typeof(int), typeof(string), typeof(bool) });

            BunnyHeader.MainInstance.PatchPostfix(typeof(StatusEffects), "AddTrait", GetType(), "StatusEffects_AddTrait", new Type[3] { typeof(string), typeof(bool), typeof(bool) });
            BunnyHeader.MainInstance.PatchPostfix(typeof(StatusEffects), "RemoveTrait", GetType(), "StatusEffects_RemoveTrait", new Type[2] { typeof(string), typeof(bool) });
        }
        public static void Initialize_Names()
        {
        }
        public static void Initialize_Traits_Inactive()
        {
            #region Combat
            CustomTrait ReturnToBonke = RogueLibs.CreateCustomTrait("ReturnToBonke", true,
                new CustomNameInfo("Return to Bonke"),
                new CustomNameInfo("Chance to inflict Dizziness when striking an NPC with a blunt weapon."));
            ReturnToBonke.AvailableInCharacterCreation = false; //
            ReturnToBonke.CostInCharacterCreation = 3;
            ReturnToBonke.IsActive = false; //
            ReturnToBonke.Available = false; //
            ReturnToBonke.Upgrade = null;

            CustomTrait Whiffist = RogueLibs.CreateCustomTrait("Whiffist", true,
                new CustomNameInfo("Whiffist"),
                new CustomNameInfo("Small chance for Melee or Thrown attacks to miss you completely."));
            Whiffist.AvailableInCharacterCreation = false; //
            Whiffist.CostInCharacterCreation = 3;
            Whiffist.IsActive = false; //
            Whiffist.Available = false; //
            Whiffist.Upgrade = "Whiffist_2";
            #endregion
            #region Consumables
            CustomTrait Alcoholic = RogueLibs.CreateCustomTrait("Alcoholic", true,
                new CustomNameInfo("Alcoholic"),
                new CustomNameInfo("Alcoholic? What? This must be a mistake. You can stop drinking any time you want. You just don't want to."));
            Alcoholic.Available = true;
            Alcoholic.AvailableInCharacterCreation = true;
            Alcoholic.CanRemove = true;
            Alcoholic.CanSwap = false;
            Alcoholic.CostInCharacterCreation = -5;
            Alcoholic.IsActive = true;
            Alcoholic.Upgrade = null;
            // TODO: Allow consumption at full health

            CustomTrait HungryBoy = RogueLibs.CreateCustomTrait("HungryBoy", true,
                new CustomNameInfo("Hungry Boy"),
                new CustomNameInfo("Less healing from food and alcohol."));
            HungryBoy.AvailableInCharacterCreation = false; //
            HungryBoy.CostInCharacterCreation = -3;
            HungryBoy.IsActive = false; //
            HungryBoy.Available = false; //
            HungryBoy.Upgrade = null;
            #endregion
            #region Class - Dealer
            CustomTrait DeathToSnitches = RogueLibs.CreateCustomTrait("DeathToSnitches", true,
                new CustomNameInfo("Death To Snitches"),
                new CustomNameInfo("Cops will ignore your Pusher attempts. You may attempt to sell to Cops, but failure will turn them hostile."));
            DeathToSnitches.AvailableInCharacterCreation = false; //
            DeathToSnitches.IsActive = false; //
            DeathToSnitches.Available = false; //
            DeathToSnitches.Upgrade = null;
            //TODO: Unlock DeathToSnitches when Pusher gained

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
            #endregion
            #region Equipment
            CustomTrait Fatass = RogueLibs.CreateCustomTrait("Fatass", true,
                new CustomNameInfo("Fatass"),
                new CustomNameInfo("Becoming a fat fuck was not a decision you took lightly. In fact, you don't do anything lightly. You move slower and can't wear Armor, but you *really* enjoy food. If Stomping is your thing, it increases that damage too."));
            Fatass.Available = true;
            Fatass.AvailableInCharacterCreation = true;
            Fatass.CanRemove = true;
            Fatass.CanSwap = false;
            Fatass.Conflicting.AddRange(new string[] { "Diminutive", "DontTriggerFloorHazards", "Electronic", "KnockbackMore" });
            Fatass.CostInCharacterCreation = -2;
            Fatass.IsActive = true;
            Fatass.Recommendations.AddRange(new string[] { "Stomp" });
            Fatass.Upgrade = null;
            #endregion
            #region Karma
            CustomTrait CodeOfHonor = RogueLibs.CreateCustomTrait("CodeOfHonor", true,
                new CustomNameInfo("Code of Honor"),
                new CustomNameInfo("You have sworn to protect the innocent, and generally just be a good guy. You lose XP for dishonorable acts."));
            CodeOfHonor.Available = false; //
            CodeOfHonor.AvailableInCharacterCreation = false; //
            CodeOfHonor.CostInCharacterCreation = -6;
            CodeOfHonor.IsActive = false; // 
            CodeOfHonor.Upgrade = null;
			#endregion
            #region Class - Priest
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

            CustomTrait UndeadBane = RogueLibs.CreateCustomTrait("UndeadBane", true,
                new CustomNameInfo("Undead Bane"),
                new CustomNameInfo("The undead fear and hate you. They're probably just jealous. All Vampires, Zombies & Ghosts are hostile on sight."));
            UndeadBane.AvailableInCharacterCreation = false; //
            UndeadBane.CostInCharacterCreation = -4;
            UndeadBane.IsActive = false; //
            UndeadBane.Available = false; //
            UndeadBane.Upgrade = null;
            #endregion
            #region Social
            CustomTrait VeiledThreats = RogueLibs.CreateCustomTrait("VeiledThreats", true,
                new CustomNameInfo("Veiled Threats"),
                new CustomNameInfo("When you attempt to Bribe, Extort, Mug, or Threaten, a failure will turn the target Annoyed instead of Hostile."));
            VeiledThreats.AvailableInCharacterCreation = false; //
            VeiledThreats.CostInCharacterCreation = 3;
            VeiledThreats.IsActive = false; //
            VeiledThreats.Available = false; //
            VeiledThreats.Upgrade = null;
            #endregion
            #region Tampering
            CustomTrait OneHappyTamper = RogueLibs.CreateCustomTrait("OneHappyTamper", true,
                new CustomNameInfo("One Happy Tamper"),
                new CustomNameInfo("Owners will allow you to tamper with their belongings."));
            OneHappyTamper.AvailableInCharacterCreation = false; //
            OneHappyTamper.CostInCharacterCreation = 2;
            OneHappyTamper.IsActive = false; //
            OneHappyTamper.Available = false; //
            OneHappyTamper.Upgrade = null;
            #endregion
            #region Trapping
            CustomTrait CheekyTrappy = RogueLibs.CreateCustomTrait("CheekyTrappy", true,
                new CustomNameInfo("Cheeky Trappy"),
                new CustomNameInfo("All hidden traps are visible to you. NPCs will no longer avoid your traps."));
            CheekyTrappy.AvailableInCharacterCreation = false; //
            CheekyTrappy.CostInCharacterCreation = 1;
            CheekyTrappy.IsActive = false; //
            CheekyTrappy.Available = false; //
            CheekyTrappy.Upgrade = null;

            CustomTrait PursuitOfTrappiness = RogueLibs.CreateCustomTrait("PursuitOfTrappiness", true,
                new CustomNameInfo("Pursuit Of Trappiness"),
                new CustomNameInfo("Un-Aligned NPCs take additional damage from Traps."));
            PursuitOfTrappiness.AvailableInCharacterCreation = false; //
            PursuitOfTrappiness.CostInCharacterCreation = 2;
            PursuitOfTrappiness.IsActive = false; //
            PursuitOfTrappiness.Available = false; //
            PursuitOfTrappiness.Upgrade = null;

            CustomTrait TrapperKeeper = RogueLibs.CreateCustomTrait("TrapperKeeper", true,
                new CustomNameInfo("Trapper Keeper"),
                new CustomNameInfo("Interact with Traps to add them to your inventory. 100% chance to disarm Door Detonators."));
            TrapperKeeper.AvailableInCharacterCreation = false; //
            TrapperKeeper.CostInCharacterCreation = 2;
            TrapperKeeper.IsActive = false; //
            TrapperKeeper.Available = false; //
            TrapperKeeper.Upgrade = null;
            #endregion
            #region Miscellaneous
            CustomTrait EagleEyed = RogueLibs.CreateCustomTrait("EagleEyed", true,
                new CustomNameInfo("Eagle Eyed"),
                new CustomNameInfo("You can see further than normal. Hell, you can see further than *abnormal*."));
            EagleEyed.Available = true;
            EagleEyed.AvailableInCharacterCreation = true;
            EagleEyed.CanRemove = false;
            EagleEyed.CanSwap = true;
            EagleEyed.CostInCharacterCreation = 3;
            EagleEyed.IsActive = true;
            EagleEyed.Upgrade = "EagleEyed_2";

            CustomTrait EagleEyed_2 = RogueLibs.CreateCustomTrait("EagleEyed_2", true,
                new CustomNameInfo("Eagle Eyed +"),
                new CustomNameInfo("You can see *really* far. You might have been a good sniper or pilot, but you spent most of your early life peeping into windows."));
            EagleEyed_2.Available = true;
            EagleEyed_2.AvailableInCharacterCreation = false;
            EagleEyed_2.CanRemove = false;
            EagleEyed_2.CanSwap = false;
            EagleEyed_2.CostInCharacterCreation = 6;
            EagleEyed_2.IsActive = true;
            EagleEyed_2.Upgrade = null;

            CustomTrait Myopic = RogueLibs.CreateCustomTrait("Myopic", true,
                new CustomNameInfo("Myopic"),
                new CustomNameInfo("You can't see too far."));
            Myopic.Available = true;
            Myopic.AvailableInCharacterCreation = true;
            Myopic.CanRemove = true;
            Myopic.CanSwap = true;
            Myopic.CostInCharacterCreation = -3;
            Myopic.IsActive = true;
            Myopic.Upgrade = null;
            #endregion
        }
		public static void Initialize_Traits_Active()
        {
            #region Consumables
            CustomTrait Carnivore = RogueLibs.CreateCustomTrait("Carnivore", true,
                new CustomNameInfo("Carnivore"),
                new CustomNameInfo("'Meeeeeeat,' you grunt enthusiastically."));
            Carnivore.Available = true;
            Carnivore.AvailableInCharacterCreation = true;
            Carnivore.CanRemove = true;
            Carnivore.CanSwap = false;
            Carnivore.Conflicting.AddRange(new string[] { "BananaLover", "OilRestoresHealth", "Vegetarian" });
            Carnivore.CostInCharacterCreation = -1;
            Carnivore.IsActive = true;
            Carnivore.Upgrade = null;

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

            CustomTrait Teetotaller = RogueLibs.CreateCustomTrait("Teetotaller", true,
                new CustomNameInfo("Teetotaller"),
                new CustomNameInfo("Wow, you're really boring. You don't do drugs *or* alcohol. What do you even do?"));
            Teetotaller.Available = true;
            Teetotaller.AvailableInCharacterCreation = true;
            Teetotaller.CanRemove = true;
            Teetotaller.CanSwap = false;
            Teetotaller.Conflicting.AddRange(new string[] { "Addict", "DAREdevil", "Electronic", "FriendOfBill", "OilRestoresHealth" });
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
            Vegetarian.Conflicting.AddRange(new string[] { "BloodRestoresHealth", "CannibalizeRestoresHealth", "Carnivore", "Electronic", "FleshFeast", "OilRestoresHealth", "Zombify" });
            Vegetarian.CostInCharacterCreation = -1;
            Vegetarian.IsActive = true;
            Vegetarian.Available = false;
            Vegetarian.Upgrade = null;
            #endregion
            #region Equipment
            CustomTrait AfraidOfLoudNoises = RogueLibs.CreateCustomTrait("AfraidOfLoudNoises", true,
                new CustomNameInfo("Afraid of Loud Noises"),
                new CustomNameInfo("The recoil bruised my shouldah. The brass shell casings disoriented me as they flew past my face. The smell of sulfur and destruction made me sick. The explosions - loud like a bowomb - gave me a temporary case of PTSD. For at least an hour after firing the gun just a few times, I was anxious and irritable. And it's such small portions!"));
            AfraidOfLoudNoises.Available = true;
            AfraidOfLoudNoises.AvailableInCharacterCreation = true;
            AfraidOfLoudNoises.CanRemove = true;
            AfraidOfLoudNoises.CanSwap = true;
            AfraidOfLoudNoises.Conflicting.AddRange(new string[] { "AttacksOneDamage", "CantUseGuns", "CantUseWeapons2", "DrawNoBlood" });
            AfraidOfLoudNoises.CostInCharacterCreation = -4;
            AfraidOfLoudNoises.IsActive = true;
            AfraidOfLoudNoises.Upgrade = null;

            CustomTrait DrawNoBlood = RogueLibs.CreateCustomTrait("DrawNoBlood", true,
                new CustomNameInfo("Draw No Blood"),
                new CustomNameInfo("You have taken an oath to draw no blood. Guess you'll have to smash skulls really carefully, then. You cannot use bullet-based guns, sharp weapons, or most explosives."));
            DrawNoBlood.Available = true;
            DrawNoBlood.AvailableInCharacterCreation = true;
            DrawNoBlood.CanRemove = true;
            DrawNoBlood.CanSwap = false;
            DrawNoBlood.Conflicting.AddRange(new string[] { "AttacksOneDamage", "AfraidOfLoudNoises", "BloodRestoresHealth", "FleshFeast", "CantUseGuns", "CantUseWeapons2" });
            DrawNoBlood.CostInCharacterCreation = -5;
            DrawNoBlood.IsActive = true;
            DrawNoBlood.Upgrade = null;

            CustomTrait FatHead = RogueLibs.CreateCustomTrait("FatHead", true,
                new CustomNameInfo("Fat Head"),
                new CustomNameInfo("You have a big, fat, ugly head. You can't wear hats of any kind. No one will lend you their headphones or sunglasses, because your big, fat, dumb, ugly head will break them. Your self-esteem is pretty much in the toilet."));
            FatHead.Available = true;
            FatHead.AvailableInCharacterCreation = true;
            FatHead.CanRemove = true;
            FatHead.CanSwap = false;
            FatHead.CostInCharacterCreation = -1;
            FatHead.IsActive = true;
            FatHead.Upgrade = null;
            #endregion
            #region Luck
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

            CustomTrait Charmed_2 = RogueLibs.CreateCustomTrait("Charmed_2", false,
                new CustomNameInfo("Charmed & Dangerous +"),
                new CustomNameInfo("You are *really* lucky. Anyone who's been at the urinal next to you can attest."));
            Charmed_2.Available = false;
            Charmed_2.AvailableInCharacterCreation = false;
            Charmed_2.CanRemove = false;
            Charmed_2.CanSwap = true;
            Charmed_2.Conflicting.AddRange(new string[] { "Charmed", "Cursed", "Cursed_2" });
            Charmed_2.CostInCharacterCreation = 6;
            Charmed_2.Upgrade = null;

            CustomTrait Cursed = RogueLibs.CreateCustomTrait("Cursed", true,
                new CustomNameInfo("Unlucky"),
                new CustomNameInfo("You pissed in some old Gypsy lady's cereal, and you still refuse to apologize. She didn't like that."));
            Cursed.Available = true;
            Cursed.AvailableInCharacterCreation = true;
            Cursed.CanRemove = true;
            Cursed.CanSwap = false;
            Cursed.Conflicting.AddRange(new string[] { "Cursed_2", "Charmed", "Charmed_2" });
            Cursed.CostInCharacterCreation = -2;
            Cursed.IsActive = true;
            Cursed.Upgrade = null;

            CustomTrait Cursed_2 = RogueLibs.CreateCustomTrait("Cursed_2", true,
                new CustomNameInfo("Unlucky +"),
                new CustomNameInfo("You bought up an old Indian graveyard, and there you built a black cat sanctuary and mirror-breakery. Not your best choice."));
            Cursed_2.Available = true;
            Cursed_2.AvailableInCharacterCreation = true;
            Cursed_2.CanRemove = true;
            Cursed_2.CanSwap = false;
            Cursed_2.Conflicting.AddRange(new string[] { "Cursed", "Charmed", "Charmed_2" });
            Cursed_2.CostInCharacterCreation = -4;
            Cursed_2.IsActive = true;
            Cursed_2.Upgrade = null;
            #endregion
            #region Magic - General
            CustomTrait Archmage = RogueLibs.CreateCustomTrait("Archmage", true,
                new CustomNameInfo("Archmage"),
                new CustomNameInfo("You are an unrivalled master of the magical arts. Basically cheat mode for magical abilities, added by request."));
            Archmage.Available = true;
            Archmage.AvailableInCharacterCreation = true;
            Archmage.CanRemove = false;
            Archmage.CanSwap = false;
            Archmage.Conflicting.AddRange(new string[] { "FocusedCasting", "FocusedCasting_2", "MagicTraining", "MagicTraining_2", "WildCasting", "WildCasting_2" });
            Archmage.CostInCharacterCreation = 100;
            Archmage.IsActive = true;
            Archmage.Recommendations.AddRange(new string[] { "ManaBattery", "ManaBattery_2", "ChronomanticDilation", "Cryomancy", "Electromancy", "PyromanticJet", "TelemanticBlink" });
            Archmage.SpecialAbilities.AddRange(new string[] { "ManaBattery", "ManaBattery_2", "ChronomanticDilation", "Cryomancy", "Electromancy", "PyromanticJet", "TelemanticBlink" });
            Archmage.Upgrade = null;

            CustomTrait FocusedCasting = RogueLibs.CreateCustomTrait("FocusedCasting", true,
                new CustomNameInfo("Focused Casting"),
                new CustomNameInfo("You've carefully refined your magic techniques to improve accuracy and reduce the chances of miscasting spells."));
            FocusedCasting.Available = true; 
            FocusedCasting.AvailableInCharacterCreation = true;
            FocusedCasting.CanRemove = false;
            FocusedCasting.CanSwap = false;
            FocusedCasting.Conflicting.AddRange(new string[] { "WildCasting", "WildCasting_2" });
            FocusedCasting.CostInCharacterCreation = 3;
            FocusedCasting.IsActive = true; 
            FocusedCasting.Recommendations.AddRange(new string[] { "ChronomanticDilation", "Cryomancy", "Electromancy", "PyromanticJet", "TelemanticBlink" });
            FocusedCasting.SpecialAbilities.AddRange(new string[] { "ChronomanticDilation", "Cryomancy", "Electromancy", "PyromanticJet", "TelemanticBlink" });
            FocusedCasting.Upgrade = "FocusedCasting_2";

            CustomTrait FocusedCasting_2 = RogueLibs.CreateCustomTrait("FocusedCasting_2", true,
                new CustomNameInfo("Focused Casting +"),
                new CustomNameInfo("You've even more carefully refined your techniques even more to improve accuracy and reduce the chances of miscasting spells EVEN MORE."));
            FocusedCasting_2.Available = false; 
            FocusedCasting_2.AvailableInCharacterCreation = false;
            FocusedCasting_2.CanRemove = false;
            FocusedCasting_2.CanSwap = false;
            FocusedCasting_2.Conflicting.AddRange(new string[] { "WildCasting", "WildCasting_2" });
            FocusedCasting_2.CostInCharacterCreation = 6;
            FocusedCasting_2.IsActive = true; 
            FocusedCasting_2.Upgrade = null;

            CustomTrait MagicTraining = RogueLibs.CreateCustomTrait("MagicTraining", true,
                new CustomNameInfo("Magic Training"),
                new CustomNameInfo("Improves your skills with any Magic Special Ability."));
            MagicTraining.Available = true;
            MagicTraining.AvailableInCharacterCreation = true;
            MagicTraining.CostInCharacterCreation = 5;
            MagicTraining.CanRemove = false;
            MagicTraining.CanSwap = false;
            MagicTraining.Conflicting.AddRange(new string[] { });
            MagicTraining.IsActive = true;
            MagicTraining.SpecialAbilities.AddRange(new string[] { "ChronomanticDilation", "Cryomancy", "Electromancy", "PyromanticJet", "TelemanticBlink" });
            MagicTraining.Upgrade = "MagicTraining_2";

            CustomTrait MagicTraining_2 = RogueLibs.CreateCustomTrait("MagicTraining_2", true,
                new CustomNameInfo("Magic Training +"),
                new CustomNameInfo("Further improves your skills with any Magic Special Ability."));
            MagicTraining_2.Available = false;
            MagicTraining_2.AvailableInCharacterCreation = false;
            MagicTraining_2.CostInCharacterCreation = 10;
            MagicTraining_2.CanRemove = false;
            MagicTraining_2.CanSwap = false;
            MagicTraining_2.Conflicting.AddRange(new string[] { });
            MagicTraining_2.IsActive = true;
            MagicTraining_2.SpecialAbilities.AddRange(new string[] { "ChronomanticDilation", "Cryomancy", "Electromancy", "PyromanticJet", "TelemanticBlink" });
            MagicTraining_2.Upgrade = null;

            CustomTrait ManaBattery = RogueLibs.CreateCustomTrait("ManaBattery", true,
                new CustomNameInfo("Mana Battery"),
                new CustomNameInfo("You can store more mana. Nifty."));
            ManaBattery.Available = true;
            ManaBattery.AvailableInCharacterCreation = true;
            ManaBattery.CostInCharacterCreation = 2;
            ManaBattery.CanRemove = false;
            ManaBattery.CanSwap = false;
            ManaBattery.Conflicting.AddRange(new string[] { });
            ManaBattery.IsActive = true;
            ManaBattery.SpecialAbilities.AddRange(new string[] { "Archmage", "ChronomanticDilation", "Cryomancy", "Electromancy", "PyromanticJet", "TelemanticBlink" });
            ManaBattery.Upgrade = "ManaBattery_2";

            CustomTrait ManaBattery_2 = RogueLibs.CreateCustomTrait("ManaBattery_2", true,
                new CustomNameInfo("Mana Battery +"),
                new CustomNameInfo("You can store even more mana. Even niftier."));
            ManaBattery_2.Available = true;
            ManaBattery_2.AvailableInCharacterCreation = false;
            ManaBattery_2.CostInCharacterCreation = 4;
            ManaBattery_2.CanRemove = false;
            ManaBattery_2.CanSwap = false;
            ManaBattery_2.Conflicting.AddRange(new string[] { });
            ManaBattery_2.IsActive = true;
            ManaBattery_2.SpecialAbilities.AddRange(new string[] { "Archmage", "ChronomanticDilation", "Cryomancy", "Electromancy", "PyromanticJet", "TelemanticBlink" });
            ManaBattery_2.Upgrade = null;

            CustomTrait WildCasting = RogueLibs.CreateCustomTrait("WildCasting", true,
                new CustomNameInfo("Wild Casting"),
                new CustomNameInfo("You don't need all that safety shit. You wanna cast some damn spells! Your spells are more powerful, but you have a greater chance of miscasting them."));
            WildCasting.Available = true; 
            WildCasting.AvailableInCharacterCreation = true;
            WildCasting.CanRemove = false;
            WildCasting.CanSwap = false;
            WildCasting.Conflicting.AddRange(new string[] { "FocusedCasting", "FocusedCasting_2" });
            WildCasting.CostInCharacterCreation = 3;
            WildCasting.IsActive = true; 
            WildCasting.Recommendations.AddRange(new string[] { "ChronomanticDilation", "Cryomancy", "Electromancy", "PyromanticJet", "TelemanticBlink" });
            WildCasting.SpecialAbilities.AddRange(new string[] { "ChronomanticDilation", "Cryomancy", "Electromancy", "PyromanticJet", "TelemanticBlink" });
            WildCasting.Upgrade = "WildCasting_2";

            CustomTrait WildCasting_2 = RogueLibs.CreateCustomTrait("WildCasting_2", true,
                new CustomNameInfo("Wild Casting +"),
                new CustomNameInfo("You're addicted to power. Your spells are ridiculously powerful... and risky."));
            WildCasting_2.Available = false; 
            WildCasting_2.AvailableInCharacterCreation = false;
            WildCasting_2.CanRemove = false;
            WildCasting_2.CanSwap = false;
            WildCasting_2.Conflicting.AddRange(new string[] { "FocusedCasting", "FocusedCasting_2" });
            WildCasting_2.CostInCharacterCreation = 6;
            WildCasting_2.IsActive = true; 
            WildCasting_2.Upgrade = null;
            #endregion
            #region Magic - Chronomantic Dilation
            CustomTrait RATS = RogueLibs.CreateCustomTrait("RATS", true,
                new CustomNameInfo("R.A.T.S. Mk VI"),
                new CustomNameInfo("Resistance-Tec Assisted Targeting System. The latest cybernetic enhancement to shooting accuracy, crit chance, & some combat traits with a double effect when time is slowed down."));
            RATS.Available = true;
            RATS.AvailableInCharacterCreation = true;
            RATS.CanRemove = false;
            RATS.CanSwap = true;
            RATS.Conflicting.AddRange(new string[] { "RATS_2" });
            RATS.CostInCharacterCreation = 3;
            RATS.IsActive = true;
            RATS.Recommendations.AddRange(new string[] { "ChronomanticDilation", "ChanceAttacksDoZeroDamage", "ChanceToSlowEnemies", "IncreasedCritChance", "KnockWeapons"});
            RATS.Upgrade = "RATS_2";

            CustomTrait RATS_2 = RogueLibs.CreateCustomTrait("RATS_2", true,
                new CustomNameInfo("R.A.T.S. Mk VII"),
                new CustomNameInfo("\"Various bug fixes and performance improvements.\" That's all the fucking update notes say. Also, they took out the headphone jack??"));
            RATS_2.Available = true;
            RATS_2.AvailableInCharacterCreation = true;
            RATS_2.CanRemove = false;
            RATS_2.CanSwap = false;
            RATS_2.Conflicting.AddRange(new string[] { "RATS" });
            RATS_2.CostInCharacterCreation = 12;
            RATS_2.IsActive = true;
            RATS_2.Upgrade = null;
			#endregion
			#region Magic - Telemantic Blink
            #endregion
			#region Stealth
			CustomTrait StealthBastardDeluxe = RogueLibs.CreateCustomTrait("StealthBastardDeluxe", true,
                new CustomNameInfo("Stealth Bastard Deluxe"),
                new CustomNameInfo("You can also through broken windows without taking a scratch. You can also hide in Bathtubs, Plants, Pool Tables, and Big Tables. [Bug: If you get stuck between it and the wall, you might clip through the wall]"));
            StealthBastardDeluxe.Available = true;
            StealthBastardDeluxe.AvailableInCharacterCreation = true;
            StealthBastardDeluxe.CanRemove = false;
            StealthBastardDeluxe.CanSwap = true;
            StealthBastardDeluxe.Conflicting.AddRange(new string[] { "Fatass", "Loud" });
            StealthBastardDeluxe.CostInCharacterCreation = 4;
            StealthBastardDeluxe.IsActive = true;
            StealthBastardDeluxe.Upgrade = null;
            #endregion
            #region Tampering
            CustomTrait TamperTantrum = RogueLibs.CreateCustomTrait("TamperTantrum", true,
                new CustomNameInfo("Tamper Tantrum"),
                new CustomNameInfo("Your tools take less wear from tampering."));
            TamperTantrum.Available = true;
            TamperTantrum.AvailableInCharacterCreation = true;
            TamperTantrum.CanRemove = false;
            TamperTantrum.CanSwap = true;
            TamperTantrum.CostInCharacterCreation = 2;
            TamperTantrum.IsActive = true;
            TamperTantrum.Upgrade = "TamperTantrum_2";

            CustomTrait TamperTantrum_2 = RogueLibs.CreateCustomTrait("TamperTantrum_2", false,
                new CustomNameInfo("Tamper Tantrum +"),
                new CustomNameInfo("Your tools take zero wear when used in tampering."));
            TamperTantrum_2.Available = false;
            TamperTantrum_2.AvailableInCharacterCreation = false;
            TamperTantrum_2.CanRemove = false;
            TamperTantrum_2.CanSwap = false;
            TamperTantrum_2.CostInCharacterCreation = 5;
            TamperTantrum_2.IsActive = true;
            TamperTantrum_2.Upgrade = null;
            #endregion
        }
        #endregion

        #region Custom
        public static void MoralCodeEvents(Agent agent, string action)
		{
            //TODO: Look in Quests and ObjectMult for pointsType
            // Event will call SkillPoints.AddPoints(EventType, 0=Good, 1=Evil)
            // Then AddPointsLate will set values depending on EventType, and flip polarity depending on the int passed.)
            // May be easiest to branch away from AddPointsLate though, so you don't have to mess with it.
            // If you do an IL injection, do it at 787
        }
        internal static string HealthCost(Agent agent, int baseDamage, DamageType type)
        {
            BunnyHeader.Log("HealthCost");

            if (type == DamageType.burnedFingers)
			{
                if (agent.statusEffects.hasTrait("ResistFire") || agent.statusEffects.hasTrait("FireproofSkin") || agent.statusEffects.hasTrait("FireproofSkin2"))
                    return "0";
            }
            else if (type == DamageType.brokenWindow)
			{
                if (agent.statusEffects.hasTrait("StealthBastardDeluxe"))
                    return "0";
                else if (agent.statusEffects.hasTrait("Diminutive"))
                    return (baseDamage / 2).ToString();
            }

            return baseDamage.ToString();
        }
        public static string ToolCost(Agent agent, int baseCost)
        {
            if (agent.statusEffects.hasTrait("TamperTantrum"))
                return (baseCost / 2).ToString();

            if (agent.statusEffects.hasTrait("TamperTantrum_2"))
                return "0";

            return baseCost.ToString();
        }
        #endregion

        #region AgentInteractions
        public static void AgentInteractions_AddButton(string buttonName, int moneyCost, string extraCost, AgentInteractions __instance, ref Agent ___mostRecentInteractingAgent) // Prefix
		{
            if (extraCost.EndsWith("-30"))
                extraCost.Replace("-30", "-" + ToolCost(___mostRecentInteractingAgent, 30));
            else if (extraCost.EndsWith("-20"))
                extraCost.Replace("-20", "-" + ToolCost(___mostRecentInteractingAgent, 20));
		}
        #endregion
        #region InvDatabase
        public static void InvDatabase_DetermineIfCanUseWeapon(InvItem item, InvDatabase __instance, ref bool __result) // Postfix
		{
            //TODO: Verify non-equipped items like Time Bomb.
            //TODO: Add Item.Categories for types above for mod compatibility
            //TODO: Convert all uses of Lists to Category checks

            if 
            (
                (__instance.agent.statusEffects.hasTrait("DrawNoBlood") && item.Categories.Contains("Piercing")) ||
                (__instance.agent.statusEffects.hasTrait("AfraidOfLoudNoises") && item.Categories.Contains("Loud") && !item.contents.Contains("Silencer")) ||
                (__instance.agent.statusEffects.hasTrait("NoBlunt") && item.Categories.Contains("Blunt"))
            )
                __result = false;

            // TODO: See also InvDatabase.ChooseWeapon
        }
        public static bool InvDatabase_EquipArmor(InvItem item, bool sfx, InvDatabase __instance) // Prefix
		{
            if (item.isArmor && __instance.agent.statusEffects.hasTrait("Fatass"))
            {
                __instance.agent.Say("I'm too fuckin' fat to wear this!");
                __instance.agent.gc.audioHandler.Play(__instance.agent, "CantDo");

                return false;
            }

            return true;
		}
        public static bool InvDatabase_EquipArmorHead(InvItem item, bool sfx, InvDatabase __instance) // Prefix
		{
            if (item.isArmorHead && item != null && __instance.agent.statusEffects.hasTrait("FatHead"))
            {
                __instance.agent.Say("Ow, I can feel it squeezing my big, stupid, dumb, ugly head!");
                __instance.agent.gc.audioHandler.Play(__instance.agent, "CantDo");

                return false;
            }

            return true;
        }
        public static bool InvDatabase_EquipWeapon(InvItem item, bool sfx, InvDatabase __instance) // Prefix
        {
            if (item == null)
                return false;

            Agent agent = __instance.agent;

            if (agent.statusEffects.hasTrait("DrawNoBlood") && item.Categories.Contains("Piercing"))
            {
                agent.Say("Mommy says I can't use sharp things!");
                __instance.agent.gc.audioHandler.Play(__instance.agent, "CantDo");

                return false;
            }
            else if (agent.statusEffects.hasTrait("AfraidOfLoudNoises") && item.Categories.Contains("Loud") && !item.contents.Contains("Silencer"))
            {
                agent.Say("I can't use that! It's too loooooud.");
                __instance.agent.gc.audioHandler.Play(__instance.agent, "CantDo");

                return false;
            }
            else if (agent.statusEffects.hasTrait("NoBlunt") && item.Categories.Contains("Blunt"))
			{
                agent.Say("I need something sharper.");
                __instance.agent.gc.audioHandler.Play(__instance.agent, "CantDo");

                return false;
            }

            return true;
        }
        public static bool InvDatabase_SubtractFromItemCount_a(int slotNum, ref int amount, bool toolbarMove, InvDatabase __instance) // Prefix
		{
            if (BunnyHeader.tools.Contains(__instance.InvItemList[slotNum].invItemName))
			{
                if (__instance.agent.statusEffects.hasTrait("TamperTantrum_2"))
                    amount = 0;
                else if (__instance.agent.statusEffects.hasTrait("TamperTantrum"))
                    amount /= 2;
            }
            return true;
		}
        public static bool InvDatabase_SubtractFromItemCount_b(InvItem invItem, ref int amount, bool toolbarMove, InvDatabase __instance) // Prefix
		{
            if (BunnyHeader.tools.Contains(invItem.invItemName))
            {
                if (__instance.agent.statusEffects.hasTrait("TamperTantrum_2"))
                    amount = 0;
                else if (__instance.agent.statusEffects.hasTrait("TamperTantrum"))
                    amount /= 2;
            }
            return true;
        }
		#endregion
		#region InvItem
        public static void InvItem_SetupDetails(bool notNew, InvItem __instance) // Postfix
        {
            string name = __instance.invItemName;

            if (__instance.Categories.Count == 0)
                __instance.Categories.Add("NullCatcher");

            if (__instance.Categories.Contains("Alcohol"))
			{
			}
            if (__instance.Categories.Contains("Drugs"))
			{
			}
            if (__instance.Categories.Contains("Food"))
            {
                if (BunnyHeader.nonVegetarian.Contains(name))
                    __instance.Categories.Add("NonVegetarian");
                else if (BunnyHeader.vegetarian.Contains(name))
                    __instance.Categories.Add("Vegetarian");
            }
            if (__instance.Categories.Contains("Weapons"))
            {
                if (BunnyHeader.blunt.Contains(name))
                    __instance.Categories.Add("Blunt");
                if (BunnyHeader.explosive.Contains(name))
                    __instance.Categories.Add("Explosive");
                if (BunnyHeader.loud.Contains(name) && !__instance.contents.Contains("Silencer"))
                    __instance.Categories.Add("Loud");
                if (BunnyHeader.piercing.Contains(name))
                    __instance.Categories.Add("Piercing");
            }
            return;
        }
        public static bool InvItem_UseItem(InvItem __instance) // Prefix
		{
            Agent agent = __instance.agent;
            List<string> cats = __instance.Categories;
            bool cantDoFlag = false;

            if (cats.Contains("Alcohol") && (agent.statusEffects.hasTrait("FriendOfBill") || agent.statusEffects.hasTrait("Teetotaller")))
            {
                agent.Say("Today, I choose not to drink.");
                cantDoFlag = true;
            }
            if (cats.Contains("Drugs") && (agent.statusEffects.hasTrait("DAREdevil") || agent.statusEffects.hasTrait("Teetotaller")))
            {
                agent.Say("Nope, my body is a temple!");
                cantDoFlag = true;
            }
            if (cats.Contains("NonVegetarian") && agent.statusEffects.hasTrait("Vegetarian"))
            {
                agent.Say("Meat is murder!");
                cantDoFlag = true;
            }
            if (cats.Contains("Vegetarian") && agent.statusEffects.hasTrait("Carnivore"))
            {
                agent.Say("No! Me want meat!");
                cantDoFlag = true;
            }
            if (cats.Contains("Loud") && agent.statusEffects.hasTrait("AfraidOfLoudNoises"))
			{
                agent.Say("But that'll hurt my little ears!");
                cantDoFlag = true;
            }
            if (cats.Contains("Piercing") && agent.statusEffects.hasTrait("DrawNoBlood"))
			{
                agent.Say("I swore to draw no blood. Unless I remove this trait first.");
                cantDoFlag = true;
            }
            if (cantDoFlag)
			{
                __instance.gc.audioHandler.Play(agent, "CantDo");
                return false;
			}

            return true;
        }
        #endregion
        #region ItemFunctions
        public static void ItemFunctions_DetermineHealthChange(InvItem item, Agent agent, ref int __result) // Postfix
		{
            List<string> cats = item.Categories;
            StatusEffects traits = agent.statusEffects;
            if
            (
                (cats.Contains("Alcohol") && (traits.hasTrait("FriendOfBill") || traits.hasTrait("Teetotaller")) ) ||
                (cats.Contains("Drugs") && (traits.hasTrait("DAREdevil") || traits.hasTrait("Teetotaller")) ) ||
                (cats.Contains("Vegetarian") && traits.hasTrait("Carnivore")) ||
                (cats.Contains("NonVegetarian") && traits.hasTrait("Vegetarian"))
            )
                __result = 0;
            if (traits.hasTrait("Fatass"))
                __result = (int)((float)__result * 1.5f);
		}
        public static bool ItemFunctions_UseItem(InvItem item, Agent agent, ItemFunctions __instance) // Prefix ***new
		{
            if (item.itemType == "Consumable")
			{
                if (BunnyHeader.alcohol.Contains(item.invItemName) && ((agent.statusEffects.hasTrait("FriendOfBill") || agent.statusEffects.hasTrait("Teetotaller"))))
                {
                    agent.Say("Today, I choose not to drink.");
                    goto terminus;
                }

                if (BunnyHeader.drugs.Contains(item.invItemName) && (agent.statusEffects.hasTrait("DAREdevil") || agent.statusEffects.hasTrait("Teetotaller")))
                {
                    agent.Say("Nope, my body is a temple!");
                    goto terminus;
                }

                if (BunnyHeader.nonVegetarian.Contains(item.invItemName) && agent.statusEffects.hasTrait("Vegetarian"))
                {
                    agent.Say("Meat is murder!");
                    goto terminus;
                }
                else if (BunnyHeader.vegetarian.Contains(item.invItemName) && agent.statusEffects.hasTrait("Carnivore"))
                {
                    agent.Say("No! Me want meat!");
                    goto terminus;
                }
            }
			else
            {
                if (BunnyHeader.loud.Contains(item.invItemName) && agent.statusEffects.hasTrait("AfraidOfLoudNoises"))
                {
                    agent.Say("But that'll hurt my little ears!");
                    goto terminus;
                }

                if (BunnyHeader.piercing.Contains(item.invItemName) && agent.statusEffects.hasTrait("DrawNoBlood"))
                {
                    agent.Say("I swore to draw no blood. Unless I remove this trait first.");
                    goto terminus;
                }
            }

            return true;

            terminus:

            item.gc.audioHandler.Play(agent, "CantDo");

            return false;
        }
		#endregion
		#region PlayfieldObject
		public static void PlayfieldObject_DetermineLuck(int originalLuck, string luckType, bool cancelStatusEffects, PlayfieldObject __instance, ref int __result) // Postfix
		{
            Agent agent = __instance.playfieldObjectAgent;

            int luckBonus = 0;
            int luckMultiplier = 0;
            bool RATStargetable = false;

            if (luckType == "FreeShopItem2")
                luckBonus = 10;
            else if (luckType == "DestroyGravestone")
                luckBonus = -5;
            else if (luckType == "TurnTables")
                luckBonus = 10;
            else if (luckType == "Joke")
                luckBonus = 10;
            else if (luckType == "CritChance")
			{
                luckBonus = 3;
                RATStargetable = true;
            }
            else if (luckType == "ChanceAttacksDoZeroDamage")
			{
                luckBonus = 4;
                RATStargetable = true;
            }
            else if (luckType == "DoorDetonator")
                luckBonus = 10;
            else if (luckType == "FreeShopItem")
                luckBonus = 10;
            else if (luckType == "FindThreat")
                luckBonus = 8;
            else if (luckType == "FindAskMayorHatPercentage")
                luckBonus = 8;
            else if (luckType == "ChanceToKnockWeapons")
			{
                luckBonus = 5;
                RATStargetable = true;
            }
            else if (luckType == "SlotMachine")
                luckBonus = 8;
            else if (luckType == "AttacksDamageAttacker")
                luckBonus = 10;
            else if (luckType == "Hack")
                luckBonus = 10;
            else if (luckType == "GunAim")
			{
                luckBonus = 5;
                RATStargetable = true;
            }
            else if (luckType == "SecurityCam")
                luckBonus = 10;
            else if (luckType == "FindAskPercentage")
                luckBonus = 8;
            else if (luckType == "ThiefToolsMayNotSubtract")
                luckBonus = 10;
            else if (luckType == "ChanceToSlowEnemies")
			{
                luckBonus = 4;
                RATStargetable = true;
            }

            if (agent.statusEffects.hasTrait("Charmed"))
                luckMultiplier = 1;
            else if (agent.statusEffects.hasTrait("Charmed_2"))
                luckMultiplier = 2;
            else if (agent.statusEffects.hasTrait("Cursed"))
                luckMultiplier = -1;
            else if (agent.statusEffects.hasTrait("Cursed_2"))
                luckMultiplier = -2;

            if (RATStargetable)
            {
                if (agent.statusEffects.hasTrait("RATS"))
                    luckMultiplier += 1;
                if (agent.statusEffects.hasTrait("RATS_2"))
                    luckMultiplier += 2;

                if (agent.isPlayer != 0 && agent.specialAbility == "ChronomanticDilation")
                    if (BunnyAbilities.MSA_CD_IsCast(agent))
                        luckMultiplier *= 2;
            }

            __result = Mathf.Clamp(__result + luckBonus * luckMultiplier, 0, 100);
        }
		#endregion
		#region StatusEffects
        public static void StatusEffects_AddTrait(string traitName, bool isStarting, bool justRefresh, StatusEffects __instance) // Postfix
		{
            Agent agent = __instance.agent;

            if (traitName == "Fatass")
			{
                agent.SetEndurance(agent.enduranceStatMod + 1);
                agent.SetSpeed(agent.speedStatMod - 1);
			}
		}
        public static void StatusEffects_RemoveTrait(string traitName, bool onlyLocal, StatusEffects __instance) // Postfix
		{
            Agent agent = __instance.agent;
            if (traitName == "Fatass")
			{
                //TODO: CharacterCreation.CreatePointTallyText() for stat mods
                agent.SetEndurance(agent.enduranceStatMod - 1);
                agent.SetSpeed(agent.speedStatMod + 1);
			}
		}
		#endregion
	}
}
