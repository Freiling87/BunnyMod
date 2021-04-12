using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using BepInEx;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;

using Random = UnityEngine.Random;

/*
    IsActive is the state of the unlock in the Rewards/Traits/Mutators menu
    if IsActive is set to true, you'll be able to find that unlock while playing; if set to false you won't

    Available determines whether the unlock will be available in the menu and in the game
    if Available is set to false, you won't find anything about that unlock in the game; if set to true you can find it in the Rewards/Traits/Mutators menu
    For upgrade traits, set Available to false.
 */

namespace BunnyMod.Content
{
    public enum DamageType
	{
        burnedFingers,
        brokenWindow
	}

    public class BMTraits
    {
        public static GameController GC => GameController.gameController;
        public static bool Prefix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) => BMHeader.MainInstance.PatchPrefix(type, methodName, patchType, patchMethodName, types);
        public static bool Postfix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) => BMHeader.MainInstance.PatchPostfix(type, methodName, patchType, patchMethodName, types);
        public static void BMLog(string logMessage) => BMHeader.Log(logMessage);

        #region Main
        public void Awake()
        {
            Initialize_Names();
            Initialize_Traits();

            // AgentInteractions
            Prefix(typeof(AgentInteractions), "AddButton", GetType(), "AgentInteractions_AddButton", new Type[3] { typeof(string), typeof(int), typeof(string) });

            // InvDatabase
            Postfix(typeof(InvDatabase), "DetermineIfCanUseWeapon", GetType(), "InvDatabase_DetermineIfCanUseWeapon", new Type[1] { typeof(InvItem) });
            Prefix(typeof(InvDatabase), "EquipArmor", GetType(), "InvDatabase_EquipArmor", new Type[2] { typeof(InvItem), typeof(bool) });
            Prefix(typeof(InvDatabase), "EquipArmorHead", GetType(), "InvDatabase_EquipArmorHead", new Type[2] { typeof(InvItem), typeof(bool) });
            Prefix(typeof(InvDatabase), "EquipWeapon", GetType(), "InvDatabase_EquipWeapon", new Type[2] { typeof(InvItem), typeof(bool) });
            Prefix(typeof(InvDatabase), "SubtractFromItemCount", GetType(), "InvDatabase_SubtractFromItemCount_a", new Type[3] { typeof(int), typeof(int), typeof(bool) });
            Prefix(typeof(InvDatabase), "SubtractFromItemCount", GetType(), "InvDatabase_SubtractFromItemCount_b", new Type[3] { typeof(InvItem), typeof(int), typeof(bool) });

            // InvItem
            Postfix(typeof(InvItem), "SetupDetails", GetType(), "InvItem_SetupDetails", new Type[1] { typeof(bool) });
            Prefix(typeof(InvItem), "UseItem", GetType(), "InvItem_UseItem", new Type[0] { });

            // ItemFunctions
            Postfix(typeof(ItemFunctions), "DetermineHealthChange", GetType(), "ItemFunctions_DetermineHealthChange", new Type[2] { typeof(InvItem), typeof(Agent) });
            Prefix(typeof(ItemFunctions), "UseItem", GetType(), "ItemFunctions_UseItem", new Type[2] { typeof(InvItem), typeof(Agent) });

            // LoadLevel
            Postfix(typeof(LoadLevel), "SetupMore3_3", GetType(), "LoadLevel_SetupMore3_3_Postfix", new Type[0] { });

            // MeleeHitbox
            //Postfix(typeof(MeleeHitbox), "HitObject", GetType(), "MeleeHitbox_HitObject", new Type[2] { typeof(GameObject), typeof(bool) });
            //Postfix(typeof(MeleeHitbox), "MeleeHitEffect", GetType(), "MeleeHitbox_MeleeHitEffect", new Type[1] { typeof(GameObject) });

            // PlayerControl
            Postfix(typeof(PlayerControl), "Update", GetType(), "PlayerControl_Update", new Type[0] { });

            // PlayfieldObject
            Postfix(typeof(PlayfieldObject), "DetermineLuck", GetType(), "PlayfieldObject_DetermineLuck", new Type[3] { typeof(int), typeof(string), typeof(bool) });

            // StatusEffects
            Postfix(typeof(StatusEffects), "AddTrait", GetType(), "StatusEffects_AddTrait", new Type[3] { typeof(string), typeof(bool), typeof(bool) });
            Postfix(typeof(StatusEffects), "BecomeHidden", GetType(), "StatusEffects_BecomeHidden", new Type[1] { typeof(ObjectReal)});
            Postfix(typeof(StatusEffects), "RemoveTrait", GetType(), "StatusEffects_RemoveTrait", new Type[2] { typeof(string), typeof(bool) });
        }
        public static void Initialize_Names()
        {
        }
        public static void Initialize_Traits()
        {
			#region Codes of Conduct
			//CustomTrait CodeOfHonor = RogueLibs.CreateCustomTrait("CodeOfHonor", true,
			//    new CustomNameInfo("Code of Honor"),
			//    new CustomNameInfo("You have sworn to protect the innocent, and generally just be a good guy. You lose XP for dishonorable acts."));
			//CodeOfHonor.Available = false; //
			//CodeOfHonor.AvailableInCharacterCreation = false; //
			//CodeOfHonor.CostInCharacterCreation = -6;
			//CodeOfHonor.IsActive = false; // 
			//CodeOfHonor.Upgrade = null;
			#endregion
			#region Combat
			//CustomTrait ReturnToBonke = RogueLibs.CreateCustomTrait("ReturnToBonke", true,
			//    new CustomNameInfo("Return to Bonke"),
			//    new CustomNameInfo("Chance to inflict Dizziness when striking an NPC with a blunt weapon."));
			//ReturnToBonke.AvailableInCharacterCreation = false; //
			//ReturnToBonke.CostInCharacterCreation = 3;
			//ReturnToBonke.IsActive = false; //
			//ReturnToBonke.Available = false; //
			//ReturnToBonke.Upgrade = null;

			CustomTrait SpectralStrikes = RogueLibs.CreateCustomTrait("SpectralStrikes", true,
				new CustomNameInfo("Spectral Strikes"),
				new CustomNameInfo("Your unarmed attacks can damage Ghosts."));
			SpectralStrikes.AvailableInCharacterCreation = true;
			SpectralStrikes.CostInCharacterCreation = 1;
			SpectralStrikes.IsActive = true;
			SpectralStrikes.Available = true;
			SpectralStrikes.Upgrade = "SpectralStrikes_2";

            CustomTrait SpectralStrikes_2 = RogueLibs.CreateCustomTrait("SpectralStrikes_2", true,
                new CustomNameInfo("Spectral Strikes +"),
                new CustomNameInfo("Your melee attacks can damage Ghosts."));
            SpectralStrikes_2.AvailableInCharacterCreation = true;
            SpectralStrikes_2.CostInCharacterCreation = 2;
            SpectralStrikes_2.IsActive = true;
            SpectralStrikes_2.Available = true;
            SpectralStrikes_2.Upgrade = null;

            //CustomTrait Whiffist = RogueLibs.CreateCustomTrait("Whiffist", true,
            //    new CustomNameInfo("Whiffist"),
            //    new CustomNameInfo("Small chance for Melee or Thrown attacks to miss you completely."));
            //Whiffist.AvailableInCharacterCreation = false; //
            //Whiffist.CostInCharacterCreation = 3;
            //Whiffist.IsActive = false; //
            //Whiffist.Available = false; //
            //Whiffist.Upgrade = "Whiffist_2";
            #endregion
            #region Consumables
            //CustomTrait Alcoholic = RogueLibs.CreateCustomTrait("Alcoholic", true,
            //    new CustomNameInfo("Alcoholic"),
            //    new CustomNameInfo("Alcoholic? What? This must be a mistake. You can stop drinking any time you want. You just don't want to."));
            //Alcoholic.Available = true;
            //Alcoholic.AvailableInCharacterCreation = true;
            //Alcoholic.CanRemove = true;
            //Alcoholic.CanSwap = false;
            //Alcoholic.CostInCharacterCreation = -5;
            //Alcoholic.IsActive = true;
            //Alcoholic.Upgrade = null;
            //// TODO: Allow consumption at full health

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

            //CustomTrait HungryBoy = RogueLibs.CreateCustomTrait("HungryBoy", true,
            //    new CustomNameInfo("Hungry Boy"),
            //    new CustomNameInfo("Less healing from food and alcohol."));
            //HungryBoy.AvailableInCharacterCreation = false; //
            //HungryBoy.CostInCharacterCreation = -3;
            //HungryBoy.IsActive = false; //
            //HungryBoy.Available = false; //
            //HungryBoy.Upgrade = null;

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
            Vegetarian.IsActive = false;
            Vegetarian.Available = false;
            Vegetarian.Upgrade = null;
            #endregion
            #region Economic
            //CustomTrait DeathToSnitches = RogueLibs.CreateCustomTrait("DeathToSnitches", true,
            //    new CustomNameInfo("Death To Snitches"),
            //    new CustomNameInfo("Cops & Upper-Crusters will ignore your Pusher attempts. You may attempt to sell to Cops, but failure will turn them hostile."));
            //DeathToSnitches.AvailableInCharacterCreation = false; //
            //DeathToSnitches.IsActive = false; //
            //DeathToSnitches.Available = false; //
            //DeathToSnitches.Upgrade = null;
            ////TODO: Unlock DeathToSnitches when Pusher gained

            //CustomTrait Pusher = RogueLibs.CreateCustomTrait("Pusher", true,
            //    new CustomNameInfo("Pusher"),
            //    new CustomNameInfo("You can interact with most NPCs to attempt to sell them any drug item you have (or simplified, just Sugar). If you fail, they become annoyed (Upper Crusters will call the cops immediately). Cops who witness a dealing attempt will go Hostile. If you succeed at a sale, they have a chance to become Hooked. After a certain interval of withdrawal, NPCs will gain the Jonesing status. They'll seek you out in the level and beg you for a particular drug. If you go too long without selling them the requested drug, they'll go hostile, but selling them other types of drugs will keep them at bay for a while. When Jonesing, they will freely give you keys and safe combos if you ask. Jonesing NPCs may also attack other drug dealers, doctors, or scientists if they can't track you down."));//
            //Pusher.AvailableInCharacterCreation = false; //
            //Pusher.CanRemove = false;
            //Pusher.CanSwap = false;
            //Pusher.CostInCharacterCreation = 6;
            //Pusher.IsActive = false; //
            //Pusher.Available = false; //
            //Pusher.Upgrade = "Pusher_2";
            ////TODO: Unlock DeathToSnitches when Pusher gained

            //CustomTrait Pusher_2 = RogueLibs.CreateCustomTrait("Pusher_2", true,
            //    new CustomNameInfo("Pusher +"),
            //    new CustomNameInfo("Increased chances of success when pushing drugs."));//
            //Pusher_2.AvailableInCharacterCreation = false; //
            //Pusher_2.IsActive = false; //
            //Pusher_2.Available = false; //
            //Pusher_2.Upgrade = null;
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
            DrawNoBlood.IsActive = false;
            DrawNoBlood.Upgrade = null;

            //CustomTrait Fatass = RogueLibs.CreateCustomTrait("Fatass", true,
            //    new CustomNameInfo("Fatass"),
            //    new CustomNameInfo("Becoming a fat fuck was not a decision you took lightly. In fact, you don't do anything lightly. You move slower and can't wear Armor, but you *really* enjoy food. If Stomping is your thing, it increases that damage too."));
            //Fatass.Available = true;
            //Fatass.AvailableInCharacterCreation = true;
            //Fatass.CanRemove = true;
            //Fatass.CanSwap = false;
            //Fatass.Conflicting.AddRange(new string[] { "Diminutive", "DontTriggerFloorHazards", "Electronic", "KnockbackMore" });
            //Fatass.CostInCharacterCreation = -2;
            //Fatass.IsActive = true;
            //Fatass.Recommendations.AddRange(new string[] { "Stomp" });
            //Fatass.Upgrade = null;

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
            Cursed.IsActive = false;
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
            Cursed_2.IsActive = false;
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
            RATS.Recommendations.AddRange(new string[] { "ChronomanticDilation", "ChanceAttacksDoZeroDamage", "ChanceToSlowEnemies", "IncreasedCritChance", "KnockWeapons" });
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
			#region Miscellaneous
			CustomTrait EagleEyes = RogueLibs.CreateCustomTrait("EagleEyes", true,
				new CustomNameInfo("Eagle Eyes"),
				new CustomNameInfo("You can see further than normal. Hell, you can see further than *abnormal*."));
			EagleEyes.Available = true;
			EagleEyes.AvailableInCharacterCreation = true;
			EagleEyes.CanRemove = false;
			EagleEyes.CanSwap = true;
            EagleEyes.Conflicting.AddRange(new string[] { "EagleEyes_2", "Myopic", "Myopic_2" });
			EagleEyes.CostInCharacterCreation = 3;
			EagleEyes.IsActive = true;
			EagleEyes.Upgrade = "EagleEyes_2";

			CustomTrait EagleEyes_2 = RogueLibs.CreateCustomTrait("EagleEyes_2", true,
				new CustomNameInfo("Eagle Eyes +"),
				new CustomNameInfo("You can see *really* far. You might have been a good sniper or pilot, but you spent most of your early life peeping into windows."));
			EagleEyes_2.Available = true;
			EagleEyes_2.AvailableInCharacterCreation = true;
			EagleEyes_2.CanRemove = false;
			EagleEyes_2.CanSwap = false;
            EagleEyes_2.Conflicting.AddRange(new string[] { "EagleEyes", "Myopic", "Myopic_2" });
			EagleEyes_2.CostInCharacterCreation = 6;
			EagleEyes_2.IsActive = true;
			EagleEyes_2.Upgrade = null;

            CustomTrait Haunted = RogueLibs.CreateCustomTrait("Haunted", true,
                new CustomNameInfo("Haunted"),
                new CustomNameInfo("You spent too long spelunking in an ancient treasure vault. The local ghosts were very unhappy with you, and you had their legal case dismissed. Now they're taking it into their own hands."));
            Haunted.Available = true;
            Haunted.AvailableInCharacterCreation = true;
            Haunted.CanRemove = true;
            Haunted.CanSwap = false;
            Haunted.Conflicting.AddRange(new string[] { });
            Haunted.CostInCharacterCreation = -2;
            Haunted.IsActive = true;
            Haunted.Upgrade = null;

            CustomTrait MobDebt = RogueLibs.CreateCustomTrait("MobDebt", true,
                new CustomNameInfo("Mob Debt"),
                new CustomNameInfo("You found a sack of money, and the people to whom it belongs want it back. With interest. You have $1,000, but need to pay back double the amount by Level 10, or else your kneecaps (and the rest of your body) will pay the price."));
            MobDebt.Available = true;
            MobDebt.AvailableInCharacterCreation = true;
            MobDebt.CanRemove = true;
            MobDebt.CanSwap = false;
            MobDebt.Conflicting.AddRange(new string[] { });
            MobDebt.CostInCharacterCreation = -10;
            MobDebt.IsActive = true;
            MobDebt.Upgrade = null;

            CustomTrait MookMasher = RogueLibs.CreateCustomTrait("MookMasher", true,
                new CustomNameInfo("Mook Masher"),
                new CustomNameInfo("The Mayor knows you're a threat, and you're coming for him. He could just destroy you, but as a villain, he prefers to send his henchmen at you in steadily increasing but manageable waves."));
            MookMasher.Available = true;
            MookMasher.AvailableInCharacterCreation = true;
            MookMasher.CanRemove = true;
            MookMasher.CanSwap = false;
            MookMasher.Conflicting.AddRange(new string[] { });
            MookMasher.CostInCharacterCreation = -5;
            MookMasher.IsActive = true;
            MookMasher.Upgrade = null;

            CustomTrait Myopic = RogueLibs.CreateCustomTrait("Myopic", true,
				new CustomNameInfo("Myopic"),
				new CustomNameInfo("You can't see too far."));
			Myopic.Available = true;
			Myopic.AvailableInCharacterCreation = true;
			Myopic.CanRemove = true;
			Myopic.CanSwap = true;
            Myopic.Conflicting.AddRange(new string[] { "EagleEyes", "EagleEyes_2", "Myopic_2" });
			Myopic.CostInCharacterCreation = -4;
			Myopic.IsActive = true;
			Myopic.Upgrade = null;

			CustomTrait Myopic_2 = RogueLibs.CreateCustomTrait("Myopic_2", true,
				new CustomNameInfo("Ultramyopic"),
				new CustomNameInfo("You tend to keep people at arm's length, where you can't see them."));
			Myopic_2.Available = true;
			Myopic_2.AvailableInCharacterCreation = true;
			Myopic_2.CanRemove = true;
			Myopic_2.CanSwap = true;
            Myopic_2.Conflicting.AddRange(new string[] { "EagleEyes", "EagleEyes_2", "Myopic" });
			Myopic_2.CostInCharacterCreation = -8;
			Myopic_2.IsActive = true;
			Myopic_2.Upgrade = null;

            CustomTrait Reinforcements = RogueLibs.CreateCustomTrait("Reinforcements", true,
                new CustomNameInfo("Reinforcements"),
                new CustomNameInfo("You have worked to create an army for the Resistance. That army now patrols the City secretly, looking for the opportunity to aid the cause."));
            Reinforcements.Available = true;
            Reinforcements.AvailableInCharacterCreation = true;
            Reinforcements.CanRemove = false;
            Reinforcements.CanSwap = true;
            Reinforcements.Conflicting.AddRange(new string[] { });
            Reinforcements.CostInCharacterCreation = 5;
            Reinforcements.IsActive = true;
            Reinforcements.Upgrade = "Reinforcements_2";

            CustomTrait Reinforcements_2 = RogueLibs.CreateCustomTrait("Reinforcements_2", true,
                new CustomNameInfo("Reinforcements +"),
                new CustomNameInfo("Your allies now have top-of-the-line equipment."));
            Reinforcements_2.Available = true;
            Reinforcements_2.AvailableInCharacterCreation = true;
            Reinforcements_2.CanRemove = false;
            Reinforcements_2.CanSwap = true;
            Reinforcements_2.Conflicting.AddRange(new string[] { });
            Reinforcements_2.CostInCharacterCreation = 10;
            Reinforcements_2.IsActive = true;
            Reinforcements_2.Upgrade = "Reinforcements_2";
            #endregion
            #region Social
            //CustomTrait ArtificialInsermonation = RogueLibs.CreateCustomTrait("ArtificialInsermonation", true,
            //    new CustomNameInfo("Artificial Insermonation"),
            //    new CustomNameInfo("Activate an Altar to deliver a Sermon, randomly improving relations with NPCs within earshot. They may donate tithes."));
            //ArtificialInsermonation.AvailableInCharacterCreation = false; //
            //ArtificialInsermonation.CostInCharacterCreation = 2;
            //ArtificialInsermonation.IsActive = false; //
            //ArtificialInsermonation.Available = false; //
            //ArtificialInsermonation.Upgrade = "ArtificialInsermonation_2";

            //CustomTrait ArtificialInsermonation_2 = RogueLibs.CreateCustomTrait("ArtificialInsermonation_2", true,
            //    new CustomNameInfo("Artificial Insermonation +"),
            //    new CustomNameInfo("Improved relationships and tithes from Sermonizing."));
            //ArtificialInsermonation_2.AvailableInCharacterCreation = false; //
            //ArtificialInsermonation_2.CostInCharacterCreation = 2;
            //ArtificialInsermonation_2.IsActive = false; //
            //ArtificialInsermonation_2.Available = false; //
            //ArtificialInsermonation_2.Upgrade = null;

            //CustomTrait UndeadBane = RogueLibs.CreateCustomTrait("UndeadBane", true,
            //    new CustomNameInfo("Undead Bane"),
            //    new CustomNameInfo("The undead fear and hate you. They're probably just jealous. All Vampires, Zombies & Ghosts are hostile on sight."));
            //UndeadBane.AvailableInCharacterCreation = false; //
            //UndeadBane.CostInCharacterCreation = -4;
            //UndeadBane.IsActive = false; //
            //UndeadBane.Available = false; //
            //UndeadBane.Upgrade = null;

            //CustomTrait VeiledThreats = RogueLibs.CreateCustomTrait("VeiledThreats", true,
            //    new CustomNameInfo("Veiled Threats"),
            //    new CustomNameInfo("When you attempt to Bribe, Extort, Mug, or Threaten, a failure will turn the target Annoyed instead of Hostile."));
            //VeiledThreats.AvailableInCharacterCreation = false; //
            //VeiledThreats.CostInCharacterCreation = 3;
            //VeiledThreats.IsActive = false; //
            //VeiledThreats.Available = false; //
            //VeiledThreats.Upgrade = null;
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

            CustomTrait UnderdarkCitizen = RogueLibs.CreateCustomTrait("UnderdarkCitizen", true,
                new CustomNameInfo("Underdark Citizen"),
                new CustomNameInfo("You can navigate the city's sewers with ease. Their denizens no longer consider you an easy mark."));
            UnderdarkCitizen.Available = true;
            UnderdarkCitizen.AvailableInCharacterCreation = true;
            UnderdarkCitizen.CanRemove = false;
            UnderdarkCitizen.CanSwap = false;
            UnderdarkCitizen.Conflicting.AddRange(new string[] { });
            UnderdarkCitizen.CostInCharacterCreation = 2;
            UnderdarkCitizen.IsActive = true;
            UnderdarkCitizen.Upgrade = null;
            #endregion
            #region Tampering
            //CustomTrait OneHappyTamper = RogueLibs.CreateCustomTrait("OneHappyTamper", true,
            //    new CustomNameInfo("One Happy Tamper"),
            //    new CustomNameInfo("Owners will allow you to tamper with their belongings."));
            //OneHappyTamper.AvailableInCharacterCreation = false; //
            //OneHappyTamper.CostInCharacterCreation = 2;
            //OneHappyTamper.IsActive = false; //
            //OneHappyTamper.Available = false; //
            //OneHappyTamper.Upgrade = null;

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
            #region Trapping
            //CustomTrait CheekyTrappy = RogueLibs.CreateCustomTrait("CheekyTrappy", true,
            //    new CustomNameInfo("Cheeky Trappy"),
            //    new CustomNameInfo("All hidden traps are visible to you. NPCs will no longer avoid your traps."));
            //CheekyTrappy.AvailableInCharacterCreation = false; //
            //CheekyTrappy.CostInCharacterCreation = 1;
            //CheekyTrappy.IsActive = false; //
            //CheekyTrappy.Available = false; //
            //CheekyTrappy.Upgrade = null;

            //CustomTrait PursuitOfTrappiness = RogueLibs.CreateCustomTrait("PursuitOfTrappiness", true,
            //    new CustomNameInfo("Pursuit Of Trappiness"),
            //    new CustomNameInfo("Un-Aligned NPCs take additional damage from Traps."));
            //PursuitOfTrappiness.AvailableInCharacterCreation = false; //
            //PursuitOfTrappiness.CostInCharacterCreation = 2;
            //PursuitOfTrappiness.IsActive = false; //
            //PursuitOfTrappiness.Available = false; //
            //PursuitOfTrappiness.Upgrade = null;

            //CustomTrait TrapperKeeper = RogueLibs.CreateCustomTrait("TrapperKeeper", true,
            //    new CustomNameInfo("Trapper Keeper"),
            //    new CustomNameInfo("Interact with Traps to add them to your inventory. 100% chance to disarm Door Detonators."));
            //TrapperKeeper.AvailableInCharacterCreation = false; //
            //TrapperKeeper.CostInCharacterCreation = 2;
            //TrapperKeeper.IsActive = false; //
            //TrapperKeeper.Available = false; //
            //TrapperKeeper.Upgrade = null;
            #endregion
        }
        #endregion

        #region Custom
        internal static string HealthCost(Agent agent, int baseDamage, DamageType type)
        {
            BMLog("HealthCost");

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
        public static bool IsTraitActive(string trait)
		{
            BMLog("IsTraitActive");

            foreach (Agent agent in GC.playerAgentList)
                foreach (Trait se in agent.statusEffects.TraitList)
				{
                    BMLog("Trait: '" + se.traitName + "'");

                    if (se.traitName == "StealthBastardDeluxe")
                        return true;
                }
                    
            return false;
		}
        public static void MoralCodeEvents(Agent agent, string action)
		{
            //TODO: Look in Quests and ObjectMult for pointsType
            // Event will call SkillPoints.AddPoints(EventType, 0=Good, 1=Evil)
            // Then AddPointsLate will set values depending on EventType, and flip polarity depending on the int passed.)
            // May be easiest to branch away from AddPointsLate though, so you don't have to mess with it.
            // If you do an IL injection, do it at 787
        }
        public static void ResetCameras()
		{
            float zoomLevel = GC.cameraScript.zoomLevel;

            if (IsTraitActive("EagleEyes"))
                zoomLevel = 0.60f;
            else if (IsTraitActive("EagleEyes_2"))
                zoomLevel = 0.40f;
            else if (IsTraitActive("Myopic"))
                zoomLevel = 1.50f;
            else if (IsTraitActive("Myopic_2"))
                zoomLevel = 2.00f;

            GC.cameraScript.zoomLevel = zoomLevel;
        }
        public static int ToolCost(Agent agent, int baseCost)
        {
            if (agent.statusEffects.hasTrait("TamperTantrum"))
                return (baseCost / 2);

            if (agent.statusEffects.hasTrait("TamperTantrum_2"))
                return 0;

            return baseCost;
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
                GC.audioHandler.Play(__instance.agent, "CantDo");

                return false;
            }

            return true;
		}
        public static bool InvDatabase_EquipArmorHead(InvItem item, bool sfx, InvDatabase __instance) // Prefix
		{
            if (item.isArmorHead && item != null && __instance.agent.statusEffects.hasTrait("FatHead"))
            {
                __instance.agent.Say("Ow, I can feel it squeezing my big, stupid, dumb, ugly head!");
                GC.audioHandler.Play(__instance.agent, "CantDo");

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
                GC.audioHandler.Play(__instance.agent, "CantDo");

                return false;
            }
            else if (agent.statusEffects.hasTrait("AfraidOfLoudNoises") && item.Categories.Contains("Loud") && !item.contents.Contains("Silencer"))
            {
                agent.Say("I can't use that! It's too loooooud.");
                GC.audioHandler.Play(__instance.agent, "CantDo");

                return false;
            }
            else if (agent.statusEffects.hasTrait("NoBlunt") && item.Categories.Contains("Blunt"))
			{
                agent.Say("I need something sharper.");
                GC.audioHandler.Play(__instance.agent, "CantDo");

                return false;
            }

            return true;
        }
        public static bool InvDatabase_SubtractFromItemCount_a(int slotNum, ref int amount, bool toolbarMove, InvDatabase __instance) // Prefix
		{
            if (BMHeader.tools.Contains(__instance.InvItemList[slotNum].invItemName))
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
            if (BMHeader.tools.Contains(invItem.invItemName))
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
                if (BMHeader.nonVegetarian.Contains(name))
                    __instance.Categories.Add("NonVegetarian");
                else if (BMHeader.vegetarian.Contains(name))
                    __instance.Categories.Add("Vegetarian");
            }
            if (__instance.Categories.Contains("Weapons"))
            {
                if (BMHeader.blunt.Contains(name))
                    __instance.Categories.Add("Blunt");
                if (BMHeader.explosive.Contains(name))
                    __instance.Categories.Add("Explosive");
                if (BMHeader.loud.Contains(name) && !__instance.contents.Contains("Silencer"))
                    __instance.Categories.Add("Loud");
                if (BMHeader.piercing.Contains(name))
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
                GC.audioHandler.Play(agent, "CantDo");
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
                if (BMHeader.alcohol.Contains(item.invItemName) && ((agent.statusEffects.hasTrait("FriendOfBill") || agent.statusEffects.hasTrait("Teetotaller"))))
                {
                    agent.Say("Today, I choose not to drink.");
                    goto terminus;
                }

                if (BMHeader.drugs.Contains(item.invItemName) && (agent.statusEffects.hasTrait("DAREdevil") || agent.statusEffects.hasTrait("Teetotaller")))
                {
                    agent.Say("Nope, my body is a temple!");
                    goto terminus;
                }

                if (BMHeader.nonVegetarian.Contains(item.invItemName) && agent.statusEffects.hasTrait("Vegetarian"))
                {
                    agent.Say("Meat is murder!");
                    goto terminus;
                }
                else if (BMHeader.vegetarian.Contains(item.invItemName) && agent.statusEffects.hasTrait("Carnivore"))
                {
                    agent.Say("No! Me want meat!");
                    goto terminus;
                }
            }
			else
            {
                if (BMHeader.loud.Contains(item.invItemName) && agent.statusEffects.hasTrait("AfraidOfLoudNoises"))
                {
                    agent.Say("But that'll hurt my little ears!");
                    goto terminus;
                }

                if (BMHeader.piercing.Contains(item.invItemName) && agent.statusEffects.hasTrait("DrawNoBlood"))
                {
                    agent.Say("I swore to draw no blood. Unless I remove this trait first.");
                    goto terminus;
                }
            }

            return true;

            terminus:

            GC.audioHandler.Play(agent, "CantDo");

            return false;
        }
        #endregion
        #region LevelEditor
        public static void LevelEditor_CreateLevelFeatureListDowntown()
        {
        }
        public static void LevelEditor_CreateLevelFeatureListIndustrial()
		{
		}
        public static void LevelEditor_CreateLevelFeatureListMayorVillage()
		{
		}
        public static void LevelEditor_CreateLevelFeatureListPark()
		{
        }
        public static void LevelEditor_CreateLevelFeatureListSlums()
        {
        }
        public static void LevelEditor_CreateLevelFeatureListUptown()
		{
		}
		#endregion
		#region LoadLevel
		#endregion
		#region MeleeHitbox
		public static void MeleeHitbox_HitObject(GameObject hitObject, bool fromClient, MeleeHitbox __instance) // 
		{
			if (!__instance.myMelee.agent.statusEffects.hasTrait("SpectralStrikes"))
				return;

			InvItem invItem = null;

			try
			{
				invItem = __instance.myMelee.agent.inventory.equippedWeapon;

				if (__instance.myMelee.agent.inventory.equippedWeapon.itemType == "WeaponProjectile")
				{
					invItem = __instance.myMelee.agent.inventory.fist;
				}
			}
			catch
			{
			}

			if ((!__instance.ObjectListContains(hitObject) && __instance.myMelee.canDamage && __instance.canHitMore) || fromClient)
			{
				if (hitObject.CompareTag("AgentSprite"))
				{
					__instance.objectList.Add(hitObject);
					Agent agent3 = hitObject.GetComponent<ObjectSprite>().agent;

					if (__instance.myMelee.agent != agent3 && agent3.ghost && !agent3.fellInHole && !GC.cinematic && __instance.HasLOSMelee(agent3))
					{
						__instance.objectList.Add(agent3.melee.meleeHitbox.gameObject);

						if (__instance.myMelee.invItem.meleeNoHit && !agent3.dead)
						{
							Relationship relationship = agent3.relationships.GetRelationship(__instance.myMelee.agent);

							if (!agent3.movement.HasLOSObjectBehind(__instance.myMelee.agent) || agent3.sleeping || __instance.myMelee.agent.isPlayer == 0 || __instance.myMelee.agent.invisible || (__instance.myMelee.invItem.invItemName == "StealingGlove" && __instance.myMelee.agent.oma.superSpecialAbility))
								__instance.canHitMore = false;
							else
								if (GC.serverPlayer)
								GC.spawnerMain.SpawnNoise(__instance.myMelee.agent.tr.position, 0f, null, null, __instance.myMelee.agent);
						}

						bool flag3 = !__instance.myMelee.invItem.meleeNoHit && __instance.myMelee.agent.DontHitAlignedCheck(agent3);

						if (flag3)
						{
							agent3.melee.meleeHitbox.objectList.Add(__instance.gameObject);
							agent3.melee.meleeHitbox.objectList.Add(__instance.myMelee.agent.sprTr.gameObject);

							if (__instance.myMelee.agent.zombified && agent3.isPlayer == 0 && !agent3.oma.bodyGuarded)
								agent3.zombieWhenDead = true;

							if (agent3.isPlayer == 0 && __instance.myMelee.agent.isPlayer != 0 && !agent3.dead && agent3.agentName != "Zombie" && !agent3.inhuman && !agent3.mechEmpty && !agent3.mechFilled && __instance.myMelee.agent.localPlayer && !agent3.statusEffects.hasStatusEffect("Invincible"))
							{
								if (__instance.myMelee.agent.statusEffects.hasTrait("FleshFeast2"))
									__instance.myMelee.agent.statusEffects.ChangeHealth(6f);
								else if (__instance.myMelee.agent.statusEffects.hasTrait("FleshFeast"))
									__instance.myMelee.agent.statusEffects.ChangeHealth(3f);
							}

							if (GC.serverPlayer || agent3.health > 0f || agent3.dead)
								agent3.Damage(__instance.myMelee, fromClient);

							__instance.myMelee.agent.relationships.FollowerAlert(agent3);

							if (agent3.statusEffects.hasTrait("AttacksDamageAttacker2") && __instance.myMelee.agent.ghost)
							{
								int myChance = agent3.DetermineLuck(20, "AttacksDamageAttacker", true);
								if (GC.percentChance(myChance))
								{
									__instance.myMelee.agent.lastHitByAgent = agent3;
									__instance.myMelee.agent.justHitByAgent2 = agent3;
									__instance.myMelee.agent.lastHitByAgent = agent3;
									__instance.myMelee.agent.deathMethod = "AttacksDamageAttacker";
									__instance.myMelee.agent.deathKiller = agent3.agentName;
									__instance.myMelee.agent.statusEffects.ChangeHealth(-10f);
								}
							}
							else if (agent3.statusEffects.hasTrait("AttacksDamageAttacker") && __instance.myMelee.agent.ghost)
							{
								int myChance2 = agent3.DetermineLuck(20, "AttacksDamageAttacker", true);

								if (GC.percentChance(myChance2))
								{
									__instance.myMelee.agent.lastHitByAgent = agent3;
									__instance.myMelee.agent.justHitByAgent2 = agent3;
									__instance.myMelee.agent.lastHitByAgent = agent3;
									__instance.myMelee.agent.deathMethod = "AttacksDamageAttacker";
									__instance.myMelee.agent.deathKiller = agent3.agentName;
									__instance.myMelee.agent.statusEffects.ChangeHealth(-5f);
								}
							}

							if (agent3.justDied && __instance.myMelee.agent.isPlayer > 0 && !GC.coopMode && !GC.fourPlayerMode && !GC.multiplayerMode && GC.sessionDataBig.slowMotionCinematic && GC.percentChance(25))
							{
								if (GC.challenges.Contains("LowHealth"))
									if (GC.percentChance(50))
										GC.StartCoroutine(GC.SetSecondaryTimeScale(0.1f, 0.13f));
									else
										GC.StartCoroutine(GC.SetSecondaryTimeScale(0.1f, 0.13f));
							}

							float num = 0f;

							if (__instance.myMelee.successfullySleepKilled || __instance.myMelee.successfullyBackstabbed)
								num = 0f;
							else if ((!agent3.dead || agent3.justDied) && !agent3.disappeared)
								num = (float)Mathf.Clamp(agent3.damagedAmount * 20, 80, 9999);
							else if (!agent3.disappeared)
								num = 80f;

							if (__instance.myMelee.agent.statusEffects.hasTrait("CauseBiggerKnockback"))
								num *= 2f;

							Vector3 position = agent3.tr.position;
							Vector2 velocity = agent3.rb.velocity;

							if (!agent3.disappeared && !fromClient)
								agent3.movement.KnockBackBullet(__instance.myMelee.meleeContainerTr.gameObject, num, true, __instance.myMelee.agent);

							bool flag4 = false;

							if (agent3.hasEmployer && agent3.employer.statusEffects.hasSpecialAbility("ProtectiveShell") && agent3.employer.objectMult.chargingSpecialLunge)
								flag4 = true;

							if (agent3.statusEffects.hasSpecialAbility("ProtectiveShell") && agent3.objectMult.chargingSpecialLunge)
								flag4 = true;

							if (flag4)
							{
								bool flag5 = true;

								if (GC.multiplayerMode && GC.serverPlayer)
								{
									if (agent3.isPlayer != 0 && !agent3.localPlayer && __instance.myMelee.agent.isPlayer == 0)
										flag5 = false;
									if (__instance.myMelee.agent.isPlayer != 0 && !__instance.myMelee.agent.localPlayer && agent3.isPlayer == 0)
										flag5 = false;
								}

								if (flag5)
								{
									__instance.myMelee.agent.movement.KnockBackBullet(agent3.gameObject, 240f, true, agent3);

									if (GC.serverPlayer && __instance.myMelee.agent.isPlayer == 0 && invItem.invItemName != "Fist" && !agent3.warZoneAgent)
									{
										int myChance3 = agent3.DetermineLuck(15, "ChanceToKnockWeapons", true);

										if (GC.percentChance(myChance3))
										{
											InvItem invItem2 = __instance.myMelee.agent.inventory.FindItem(invItem.invItemName);
											__instance.myMelee.agent.inventory.DestroyItem(invItem2);
											GC.spawnerMain.SpillItem(__instance.tr.position, invItem2);
											GC.spawnerMain.SpawnStatusText(__instance.myMelee.agent, "OutOfAmmo", invItem2.invItemName, "Item");

											if (!GC.serverPlayer && (__instance.myMelee.agent.isPlayer != 0 || __instance.myMelee.agent.mindControlAgent == GC.playerAgent))
												__instance.myMelee.agent.objectMultPlayfield.SpawnStatusText("OutOfAmmo", invItem2.invItemName, "Item", __instance.myMelee.agent.objectNetID, "", "");

											__instance.myMelee.agent.statusEffects.CreateBuffText("DroppedWeapon", __instance.myMelee.agent.objectNetID);
											__instance.myMelee.agent.dontPickUpWeapons = true;
										}
									}
								}
							}

							if (!GC.serverPlayer && (__instance.myMelee.agent.localPlayer || __instance.myMelee.agent.mindControlAgent == GC.playerAgent))
							{
								__instance.myMelee.agent.objectMultPlayfield.TempDisableNetworkTransform(agent3);
								Quaternion localRotation = __instance.myMelee.meleeHelperTr.localRotation;
								__instance.myMelee.meleeHelperTr.rotation = __instance.myMelee.meleeContainerTr.rotation;
								__instance.myMelee.meleeHelperTr.position = __instance.myMelee.meleeContainerTr.position;
								__instance.myMelee.meleeHelperTr.localPosition = new Vector3(__instance.myMelee.meleeHelperTr.localPosition.x, __instance.myMelee.meleeHelperTr.localPosition.y + 10f, __instance.myMelee.meleeHelperTr.localPosition.z);
								Vector3 position2 = __instance.myMelee.meleeHelperTr.position;
								__instance.myMelee.meleeHelperTr.localPosition = Vector3.zero;
								__instance.myMelee.meleeHelperTr.localRotation = localRotation;

								if (!__instance.myMelee.agent.testingNewClientLerps)
								{
									if (__instance.myMelee.agent.isPlayer != 0)
										__instance.myMelee.agent.objectMult.CallCmdMeleeHitAgent(agent3.objectNetID, position2, (int)num, position, agent3.rb.velocity);
									else
										GC.playerAgent.objectMult.CallCmdMeleeHitAgentNPC(__instance.myMelee.agent.objectNetID, agent3.objectNetID, position2, (int)num, position, agent3.rb.velocity);
								}
							}
							else if (GC.multiplayerMode && GC.serverPlayer)
								__instance.myMelee.agent.objectMult.CallRpcMeleeHitObjectFake(agent3.objectNetID);

							if ((__instance.myMelee.agent.isPlayer > 0 && __instance.myMelee.agent.localPlayer) || (agent3.isPlayer > 0 && agent3.localPlayer))
							{
								if (agent3.justDied)
									GC.ScreenShake(0.25f, (float)Mathf.Clamp(15 * agent3.damagedAmount, 160, 500), Vector2.zero, __instance.myMelee.agent);
								else
									GC.ScreenShake(0.2f, (float)Mathf.Clamp(15 * agent3.damagedAmount, 0, 500), Vector2.zero, __instance.myMelee.agent);
							}

							GC.alienFX.PlayerHitEnemy(__instance.myMelee.agent);
							__instance.myMelee.agent.combat.meleeJustHitCooldown = __instance.myMelee.agent.combat.meleeJustHitTimeStart;
							__instance.myMelee.agent.combat.meleeJustHitCloseCooldown = __instance.myMelee.agent.combat.meleeJustHitCloseTimeStart;

							if (GC.serverPlayer)
							{
								if (__instance.myMelee.successfullyBackstabbed)
									GC.spawnerMain.SpawnNoise(__instance.tr.position, 0.7f, null, null, __instance.myMelee.agent);
								else if (!__instance.myMelee.successfullySleepKilled)
									GC.spawnerMain.SpawnNoise(__instance.tr.position, 1f, null, null, __instance.myMelee.agent);
							}

							__instance.MeleeHitEffect(hitObject);

							GC.playerControl.Vibrate(__instance.myMelee.agent.isPlayer, Mathf.Clamp((float)agent3.damagedAmount / 100f + 0.05f, 0f, 0.25f), Mathf.Clamp((float)agent3.damagedAmount / 132f + 0.05f, 0f, 0.2f));

							if (GC.levelType == "Tutorial")
							{
								GC.tutorial.MeleeTarget(agent3);

								return;
							}
						}
					}
				}
			}
		}
		public static void MeleeHitbox_MeleeHitEffect(GameObject hitObject, MeleeHitbox __instance) // Postfix
		{
			// Gate to weapon/fist and trait

			if (hitObject.GetComponent<ObjectSprite>().agent.ghost && hitObject.CompareTag("AgentSprite"))
			{
				InvItem invItem = null;

				try
				{
					invItem = __instance.myMelee.agent.inventory.equippedWeapon;

					if (__instance.myMelee.agent.inventory.equippedWeapon.itemType == "WeaponProjectile")
						invItem = __instance.myMelee.agent.inventory.fist;
				}
				catch
				{
				}

				if (__instance.myMelee.fakeHitAgent)
					return;

				Agent agent = hitObject.GetComponent<ObjectSprite>().agent;

				if (__instance.myMelee.recentFakeHitObjects.Contains(agent.go))
					return;

				if (agent.hologram || agent.objectAgent)
					return;

				bool flag = false;

				if (invItem.hitSoundType == "Cut")
				{
					if (agent.damagedAmount < 12)
						GC.audioHandler.Play(__instance.myMelee.agent, "MeleeHitAgentCutSmall");
					else
						GC.audioHandler.Play(__instance.myMelee.agent, "MeleeHitAgentCutLarge");

					flag = true;
				}

				if (agent.damagedAmount < 10)
				{
					if (!flag)
					{
						string hitSoundType = invItem.hitSoundType;

						if (!(hitSoundType == "Normal"))
						{
							if (!(hitSoundType == "WerewolfSlash"))
								GC.audioHandler.Play(__instance.myMelee.agent, "MeleeHitAgentSmall");
							else
								GC.audioHandler.Play(__instance.myMelee.agent, "WerewolfSlash");
						}
						else
							GC.audioHandler.Play(__instance.myMelee.agent, "MeleeHitAgentSmall");
					}

					if (agent.damagedAmount > 0)
					{
						if (agent.inhuman || agent.mechFilled || agent.mechEmpty)
							GC.spawnerMain.SpawnParticleEffect("BloodHitYellow", agent.tr.position, __instance.myMelee.meleeContainerTr.eulerAngles.z - 90f);
						else
							GC.spawnerMain.SpawnParticleEffect("BloodHit", agent.tr.position, __instance.myMelee.meleeContainerTr.eulerAngles.z - 90f);
					}
					else
						GC.spawnerMain.SpawnParticleEffect("ObjectDestroyed", agent.tr.position, __instance.myMelee.meleeContainerTr.eulerAngles.z - 90f);

					if ((__instance.myMelee.agent.isPlayer > 0 && __instance.myMelee.agent.localPlayer) || (agent.isPlayer > 0 && agent.localPlayer))
					{
						GC.FreezeFrames(1);

						return;
					}
				}
				else if (agent.damagedAmount < 15)
				{
					if (!flag)
					{
						string hitSoundType = invItem.hitSoundType;

						if (!(hitSoundType == "Normal"))
						{
							if (!(hitSoundType == "WerewolfSlash"))
								GC.audioHandler.Play(__instance.myMelee.agent, "MeleeHitAgentLarge");
							else
								GC.audioHandler.Play(__instance.myMelee.agent, "WerewolfSlash");
						}
						else
							GC.audioHandler.Play(__instance.myMelee.agent, "MeleeHitAgentLarge");
					}

					if (agent.inhuman || agent.mechFilled || agent.mechEmpty)
						GC.spawnerMain.SpawnParticleEffect("BloodHitYellowMed", agent.tr.position, __instance.myMelee.meleeContainerTr.eulerAngles.z - 90f);
					else
						GC.spawnerMain.SpawnParticleEffect("BloodHitMed", agent.tr.position, __instance.myMelee.meleeContainerTr.eulerAngles.z - 90f);

					if ((__instance.myMelee.agent.isPlayer > 0 && __instance.myMelee.agent.localPlayer) || (agent.isPlayer > 0 && agent.localPlayer))
					{
						GC.FreezeFrames(2);

						return;
					}
				}
				else
				{
					if (!flag)
					{
						string hitSoundType = invItem.hitSoundType;

						if (!(hitSoundType == "Normal"))
						{
							if (!(hitSoundType == "WerewolfSlash"))
								GC.audioHandler.Play(__instance.myMelee.agent, "MeleeHitAgentLarge");
							else
								GC.audioHandler.Play(__instance.myMelee.agent, "WerewolfSlash");
						}
						else
							GC.audioHandler.Play(__instance.myMelee.agent, "MeleeHitAgentLarge");

						GC.audioHandler.Play(__instance.myMelee.agent, "MeleeHitAgentLarge");
					}
					if (agent.inhuman || agent.mechFilled || agent.mechEmpty)
						GC.spawnerMain.SpawnParticleEffect("BloodHitYellowLarge", agent.tr.position, __instance.myMelee.meleeContainerTr.eulerAngles.z - 90f);
					else
						GC.spawnerMain.SpawnParticleEffect("BloodHitLarge", agent.tr.position, __instance.myMelee.meleeContainerTr.eulerAngles.z - 90f);

					if ((__instance.myMelee.agent.isPlayer > 0 && __instance.myMelee.agent.localPlayer) || (agent.isPlayer > 0 && agent.localPlayer))
					{
						GC.FreezeFrames(3);

						return;
					}
				}
			}
		}
		#endregion
		#region PlayerControl
		public static void PlayerControl_Update() // Postfix
		{
            ResetCameras();
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
                    if (BMAbilities.MSA_CD_IsCast(agent))
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
        public static void StatusEffects_BecomeHidden(ObjectReal hiddenInObject, StatusEffects __instance) // Postfix
		{
            Agent agent = __instance.agent;

            if (IsTraitActive("UnderdarkCitizen") && agent.isPlayer == 0)
			{
                agent.statusEffects.BecomeNotHidden();
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
