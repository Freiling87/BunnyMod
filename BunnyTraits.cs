using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using BepInEx;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;

namespace BunnyMod
{
	class BunnyTraits
	{
		#region Generic
		public void Awake()
		{
            Initialize_Traits();
        }
        public static void Initialize_Traits()
		{
            CustomTrait Alcoholic = RogueLibs.CreateCustomTrait("Alcoholic", true,
                new CustomNameInfo("Alcoholic"),
                new CustomNameInfo("")); //
            Alcoholic.CostInCharacterCreation = -6;
            Alcoholic.IsActive = false; //
            Alcoholic.Available = false; //
            Alcoholic.Upgrade = null;

            CustomTrait ArtificialInsermonation = RogueLibs.CreateCustomTrait("ArtificialInsermonation", true,
                new CustomNameInfo("Artificial Insermonation"),
                new CustomNameInfo("Activate an Altar to deliver a Sermon, randomly improving relations with NPCs within earshot. They may donate tithes."));
            ArtificialInsermonation.CostInCharacterCreation = 2;
            ArtificialInsermonation.IsActive = false; //
            ArtificialInsermonation.Available = false; //
            ArtificialInsermonation.Upgrade = "ArtificialInsermonation_2";

            CustomTrait ArtificialInsermonation_2 = RogueLibs.CreateCustomTrait("ArtificialInsermonation_2", true,
                new CustomNameInfo("Artificial Insermonation +"),
                new CustomNameInfo("Improved relationships and tithes from Sermonizing."));
            ArtificialInsermonation_2.CostInCharacterCreation = 2;
            ArtificialInsermonation_2.IsActive = false; //
            ArtificialInsermonation_2.Available = false; //
            ArtificialInsermonation_2.Upgrade = null;

            CustomTrait Charmed = RogueLibs.CreateCustomTrait("Charmed", true,
                new CustomNameInfo("Charmed"),
                new CustomNameInfo("A permanent slight increase to your Luck."));
            Charmed.CanRemove = false;
            Charmed.CanSwap = false;
            Charmed.Conflicting.AddRange(new string[] { "Cursed" });
            Charmed.CostInCharacterCreation = 2;
            Charmed.IsActive = false; //
            Charmed.Available = false; //
            Charmed.Upgrade = null;

            CustomTrait CheekyTrappy = RogueLibs.CreateCustomTrait("CheekyTrappy", true,
                new CustomNameInfo("Cheeky Trappy"),
                new CustomNameInfo("All hidden traps are visible to you. NPCs will no longer avoid your traps."));
            CheekyTrappy.CostInCharacterCreation = 1;
            CheekyTrappy.IsActive = false; //
            CheekyTrappy.Available = false; //
            CheekyTrappy.Upgrade = null;

            CustomTrait Cursed = RogueLibs.CreateCustomTrait("Cursed", true,
                new CustomNameInfo("Cursed"),
                new CustomNameInfo("A permanent slight decrease to your Luck."));
            Cursed.CanRemove = false;
            Cursed.CanSwap = false;
            Cursed.Conflicting.AddRange(new string[] { "Charmed" });
            Cursed.CostInCharacterCreation = -1;
            Cursed.IsActive = false; //
            Cursed.Available = false; //
            Cursed.Upgrade = null;

            CustomTrait DeathToSnitches = RogueLibs.CreateCustomTrait("DeathToSnitches", true,
                new CustomNameInfo("Death To Snitches"),
                new CustomNameInfo("Cops will ignore your Pusher attempts. You may attempt to sell to Cops, but failure will turn them hostile."));
            DeathToSnitches.AvailableInCharacterCreation = false;
            DeathToSnitches.IsActive = false; //
            DeathToSnitches.Available = false; //
            DeathToSnitches.Upgrade = null;
            //TODO: Unlock DeathToSnitches when Pusher gained

            CustomTrait DrawNoBlood = RogueLibs.CreateCustomTrait("DrawNoBlood", true,
                new CustomNameInfo("Draw No Blood"),
                new CustomNameInfo("You have taken an oath. You cannot use bullet-based guns or sharp weapons."));
            DrawNoBlood.CanRemove = false;
            DrawNoBlood.CanSwap = false;
            DrawNoBlood.CostInCharacterCreation = -3;
            DrawNoBlood.IsActive = false; //
            DrawNoBlood.Available = false; //
            DrawNoBlood.Upgrade = null;

            CustomTrait DAREdevil = RogueLibs.CreateCustomTrait("DAREdevil", true,
                new CustomNameInfo("DARE-Devil"),
                new CustomNameInfo("You cannot consume drugs."));
            DAREdevil.CanRemove = false;
            DAREdevil.CanSwap = false;
            DAREdevil.CostInCharacterCreation = -2;
            DAREdevil.IsActive = false; //
            DAREdevil.Available = false; //
            DAREdevil.Upgrade = null;

            CustomTrait FriendOfBill = RogueLibs.CreateCustomTrait("FriendofBill", true,
                new CustomNameInfo("Friend Of Bill"),
                new CustomNameInfo("You cannot consume Alchohol."));
            FriendOfBill.CanRemove = false;
            FriendOfBill.CanSwap = false;
            FriendOfBill.CostInCharacterCreation = -2;
            FriendOfBill.IsActive = false; //
            FriendOfBill.Available = false; // 
            FriendOfBill.Upgrade = null;

            CustomTrait HungryBoy = RogueLibs.CreateCustomTrait("HungryBoy", true,
                new CustomNameInfo("Hungry Boy"),
                new CustomNameInfo("Less healing from food and alcohol."));
            HungryBoy.CostInCharacterCreation = -3;
            HungryBoy.IsActive = false; //
            HungryBoy.Available = false; //
            HungryBoy.Upgrade = null;

            CustomTrait OneHappyTamper = RogueLibs.CreateCustomTrait("OneHappyTamper", true,
                new CustomNameInfo("One Happy Tamper"),
                new CustomNameInfo("Owners will allow you to tamper with their belongings."));
            OneHappyTamper.CostInCharacterCreation = 2;
            OneHappyTamper.IsActive = false; //
            OneHappyTamper.Available = false; //
            OneHappyTamper.Upgrade = null;

            CustomTrait PursuitOfTrappiness = RogueLibs.CreateCustomTrait("PursuitOfTrappiness", true,
                new CustomNameInfo("Pursuit Of Trappiness"),
                new CustomNameInfo("Un-Aligned NPCs take additional damage from Traps."));
            PursuitOfTrappiness.CostInCharacterCreation = 2;
            PursuitOfTrappiness.IsActive = false; //
            PursuitOfTrappiness.Available = false; //
            PursuitOfTrappiness.Upgrade = null;

            CustomTrait Pusher = RogueLibs.CreateCustomTrait("Pusher", true,
                new CustomNameInfo("Pusher"),
                new CustomNameInfo("You can interact with most NPCs to attempt to sell them any drug item you have (or simplified, just Sugar). If you fail, they become annoyed (Upper Crusters will call the cops immediately). Cops who witness a dealing attempt will go Hostile. If you succeed at a sale, they have a chance to become Hooked. After a certain interval of withdrawal, NPCs will gain the Jonesing status. They'll seek you out in the level and beg you for a particular drug. If you go too long without selling them the requested drug, they'll go hostile, but selling them other types of drugs will keep them at bay for a while. When Jonesing, they will freely give you keys and safe combos if you ask. Jonesing NPCs may also attack other drug dealers, doctors, or scientists if they can't track you down."));//
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
            Pusher_2.AvailableInCharacterCreation = false;
            Pusher_2.IsActive = false; //
            Pusher_2.Available = false; //
            Pusher_2.Upgrade = null;

            CustomTrait ReturnToBonke = RogueLibs.CreateCustomTrait("ReturnToBonke", true,
                new CustomNameInfo("Return to Bonke"),
                new CustomNameInfo("Chance to inflict Dizziness when striking an NPC with a blunt weapon."));
            ReturnToBonke.CostInCharacterCreation = 3;
            ReturnToBonke.IsActive = false; //
            ReturnToBonke.Available = false; //
            ReturnToBonke.Upgrade = null;

            CustomTrait StealthBastardDeluxe = RogueLibs.CreateCustomTrait("StealthBastardDeluxe", true,
                new CustomNameInfo("Stealth Bastard Deluxe"),
                new CustomNameInfo("Allows you to hide in Bathtubs, Plants, Pool Tables, and Big Tables."));
            StealthBastardDeluxe.Conflicting.AddRange(new string[] { "Loud" });
            StealthBastardDeluxe.CostInCharacterCreation = 4;
            StealthBastardDeluxe.IsActive = false; //
            StealthBastardDeluxe.Available = false; //
            StealthBastardDeluxe.Upgrade = null;

            CustomTrait SuffersToolsGladly = RogueLibs.CreateCustomTrait("SuffersToolsGladly", true,
                new CustomNameInfo("Suffers Tools Gladly"),
                new CustomNameInfo("Your tools take less wear from tampering."));
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

            CustomTrait Teetotaller = RogueLibs.CreateCustomTrait("Teetotaller", true,
                new CustomNameInfo("Teetotaller"),
                new CustomNameInfo("You cannot consume drugs or alcohol."));
            Teetotaller.CostInCharacterCreation = -4;
            Teetotaller.IsActive = false; //
            Teetotaller.Available = false; //
            Teetotaller.Upgrade = null;

            CustomTrait TrapperKeeper = RogueLibs.CreateCustomTrait("TrapperKeeper", true,
                new CustomNameInfo("Trapper Keeper"),
                new CustomNameInfo("Interact with Traps to add them to your inventory. 100% chance to disarm Door Detonators."));
            TrapperKeeper.CostInCharacterCreation = 2;
            TrapperKeeper.IsActive = false; //
            TrapperKeeper.Available = false; //
            TrapperKeeper.Upgrade = null;

            CustomTrait UndeadBane = RogueLibs.CreateCustomTrait("UndeadBane", true,
                new CustomNameInfo("Undead Bane"),
                new CustomNameInfo("The undead fear and hate you. They're probably just jealous. All Vampires, Zombies & Ghosts are hostile on sight."));
            UndeadBane.CostInCharacterCreation = -4;
            UndeadBane.IsActive = false; //
            UndeadBane.Available = false; //
            UndeadBane.Upgrade = null;

            CustomTrait VeiledThreats = RogueLibs.CreateCustomTrait("VeiledThreats", true,
                new CustomNameInfo("Veiled Threats"),
                new CustomNameInfo("When you attempt to Bribe, Extort, Mug, or Threaten, a failure will turn the target Annoyed instead of Hostile."));
            VeiledThreats.CostInCharacterCreation = 3;
            VeiledThreats.IsActive = false; //
            VeiledThreats.Available = false; //
            VeiledThreats.Upgrade = null;

            CustomTrait Whiffist = RogueLibs.CreateCustomTrait("Whiffist", true,
                new CustomNameInfo("Whiffist"),
                new CustomNameInfo("Small chance for Melee or Thrown attacks to miss you completely."));
            Whiffist.CostInCharacterCreation = 3;
            Whiffist.IsActive = false; //
            Whiffist.Available = false; //
            Whiffist.Upgrade = "Whiffist_2";
        }
        #endregion
    }
}
