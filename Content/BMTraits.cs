using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using BepInEx;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;

using Random = UnityEngine.Random;
using UnityEngine.Networking;
using System.Linq;

/*
    IsActive is the state of the unlock in the Rewards/Traits/Mutators menu
    if IsActive is set to true, you'll be able to find that unlock while playing; if set to false you won't

    Available determines whether the unlock will be available in the menu and in the game
    if Available is set to false, you won't find anything about that unlock in the game; if set to true you can find it in the Rewards/Traits/Mutators menu
    For upgrade traits, set Available to false.
 */

namespace BunnyMod.Content
{
	#region Enums

    #endregion

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

            // Agent
            Prefix(typeof(Agent), "CanShakeDown", GetType(), "Agent_CanShakeDown", new Type[0] { });

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

            // PlayerControl
            Postfix(typeof(PlayerControl), "Update", GetType(), "PlayerControl_Update", new Type[0] { });

            // PlayfieldObject
            Postfix(typeof(PlayfieldObject), "DetermineLuck", GetType(), "PlayfieldObject_DetermineLuck", new Type[3] { typeof(int), typeof(string), typeof(bool) });

            // Relationships
            Postfix(typeof(Relationships), "SetupRelationshipOriginal", GetType(), "Relationships_SetupRelationshipOriginal", new Type[1] { typeof(Agent) });

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
            #region Consumables
            CustomTrait Carnivore = RogueLibs.CreateCustomTrait(cTraits.Carnivore, true,
                new CustomNameInfo(cTraits.Carnivore),
                new CustomNameInfo("'Meeeeeeat,' you grunt enthusiastically."));
            Carnivore.Available = true;
            Carnivore.AvailableInCharacterCreation = true;
            Carnivore.CanRemove = true;
            Carnivore.CanSwap = false;
            Carnivore.Conflicting.AddRange(new string[] { vTraits.BananaLover, vTraits.OilLessEssential, vTraits.OilReliant, cTraits.Vegetarian });
            Carnivore.CostInCharacterCreation = -1;
            Carnivore.IsActive = true;
            Carnivore.Upgrade = null;

            CustomTrait DAREdevil = RogueLibs.CreateCustomTrait(cTraits.DAREdevil, true,
                new CustomNameInfo(cTraits.DAREdevil),
                new CustomNameInfo("You have injected zero marijuanas. Crack is Whack. Smokers are Jokers. Needles are for... beetles."));
            DAREdevil.Available = true;
            DAREdevil.AvailableInCharacterCreation = true;
            DAREdevil.CanRemove = true;
            DAREdevil.CanSwap = false;
            DAREdevil.Conflicting.AddRange(new string[] { vTraits.Addict, cTraits.FriendOfBill, cTraits.Teetotaller});
            DAREdevil.CostInCharacterCreation = -3;
            DAREdevil.IsActive = true;
            DAREdevil.Upgrade = null;

            CustomTrait FriendOfBill = RogueLibs.CreateCustomTrait(cTraits.FriendOfBill, true,
                new CustomNameInfo("Friend Of Bill"),
                new CustomNameInfo("You are taking things one day at a time. But every day sucks when you can't get drunk anymore."));
            FriendOfBill.Available = true;
            FriendOfBill.AvailableInCharacterCreation = true;
            FriendOfBill.CanRemove = true;
            FriendOfBill.CanSwap = false;
            FriendOfBill.Conflicting.AddRange(new string[] { vTraits.Addict, cTraits.DAREdevil, cTraits.Teetotaller});
            FriendOfBill.CostInCharacterCreation = -1;
            FriendOfBill.IsActive = true;
            FriendOfBill.Upgrade = null;

            CustomTrait Teetotaller = RogueLibs.CreateCustomTrait(cTraits.Teetotaller, true,
                new CustomNameInfo(cTraits.Teetotaller),
                new CustomNameInfo("Wow, you're really boring. You don't do drugs *or* alcohol. What do you even do?"));
            Teetotaller.Available = true;
            Teetotaller.AvailableInCharacterCreation = true;
            Teetotaller.CanRemove = true;
            Teetotaller.CanSwap = false;
            Teetotaller.Conflicting.AddRange(new string[] { vTraits.Addict, cTraits.DAREdevil, vTraits.Electronic, cTraits.FriendOfBill, vTraits.OilLessEssential, vTraits.OilReliant });
            Teetotaller.CostInCharacterCreation = -4;
            Teetotaller.IsActive = true;
            Teetotaller.Upgrade = null;

            CustomTrait Vegetarian = RogueLibs.CreateCustomTrait(cTraits.Vegetarian, true,
                new CustomNameInfo(cTraits.Vegetarian),
                new CustomNameInfo("You are one of those people."));
            Vegetarian.Available = true;
            Vegetarian.AvailableInCharacterCreation = true;
            Vegetarian.CanRemove = true;
            Vegetarian.CanSwap = true;
            Vegetarian.Conflicting.AddRange(new string[] { vTraits.Jugularious, vTraits.StrictCannibal, cTraits.Carnivore, vTraits.Electronic, vTraits.FleshFeast, vTraits.OilLessEssential, vTraits.OilReliant, vTraits.Zombiism });
            Vegetarian.CostInCharacterCreation = -1;
            Vegetarian.IsActive = true;
            Vegetarian.Available = false;
            Vegetarian.Upgrade = null;
            #endregion
            #region Equipment
            CustomTrait AfraidOfLoudNoises = RogueLibs.CreateCustomTrait(cTraits.AfraidOfLoudNoises, true,
                new CustomNameInfo("Afraid of Loud Noises"),
                new CustomNameInfo("The recoil bruised my shouldah. The brass shell casings disoriented me as they flew past my face. The smell of sulfur and destruction made me sick. The explosions - loud like a bowomb - gave me a temporary case of PTSD. For at least an hour after firing the gun just a few times, I was anxious and irritable. And it's such small portions!"));
            AfraidOfLoudNoises.Available = true;
            AfraidOfLoudNoises.AvailableInCharacterCreation = true;
            AfraidOfLoudNoises.CanRemove = true;
            AfraidOfLoudNoises.CanSwap = true;
            AfraidOfLoudNoises.Conflicting.AddRange(new string[] { cTraits.DrawNoBlood, vTraits.NearHarmless, vTraits.StubbyFingers, vTraits.SausageFingers });
            AfraidOfLoudNoises.CostInCharacterCreation = -4;
            AfraidOfLoudNoises.IsActive = true;
            AfraidOfLoudNoises.Upgrade = null;

            CustomTrait DrawNoBlood = RogueLibs.CreateCustomTrait(cTraits.DrawNoBlood, true,
                new CustomNameInfo("Draw No Blood"),
                new CustomNameInfo("You have taken an oath to draw no blood. Guess you'll have to smash skulls really carefully, then. You cannot use bullet-based guns, sharp weapons, or most explosives."));
            DrawNoBlood.Available = true;
            DrawNoBlood.AvailableInCharacterCreation = true;
            DrawNoBlood.CanRemove = true;
            DrawNoBlood.CanSwap = false;
            DrawNoBlood.Conflicting.AddRange(new string[] { cTraits.AfraidOfLoudNoises, vTraits.NearHarmless, vTraits.Jugularious, vTraits.FleshFeast, vTraits.StubbyFingers, vTraits.SausageFingers });
            DrawNoBlood.CostInCharacterCreation = -5;
            DrawNoBlood.IsActive = true;
            DrawNoBlood.Upgrade = null;

            CustomTrait FatHead = RogueLibs.CreateCustomTrait(cTraits.FatHead, true,
                new CustomNameInfo("Fat Head"),
                new CustomNameInfo("You have a big, fat, ugly head. You can't wear hats of any kind. No one will lend you their headphones or sunglasses, because your big, fat, dumb, ugly head will break them. Your self-esteem is pretty much in the toilet."));
            FatHead.Available = true;
            FatHead.AvailableInCharacterCreation = true;
            FatHead.CanRemove = true;
            FatHead.CanSwap = false;
            FatHead.Conflicting.AddRange(new string[] { vTraits.Diminutive });
            FatHead.CostInCharacterCreation = -1;
            FatHead.IsActive = true;
            FatHead.Upgrade = null;
            #endregion
            #region Luck
            CustomTrait Charmed = RogueLibs.CreateCustomTrait(cTraits.Charmed, true,
                new CustomNameInfo("Charmed & Dangerous"),
                new CustomNameInfo("You once found a fourteen-leaf clover."));
            Charmed.Available = true;
            Charmed.AvailableInCharacterCreation = true;
            Charmed.CanRemove = false;
            Charmed.CanSwap = true;
            Charmed.Conflicting.AddRange(new string[] { cTraits.Charmed_2, cTraits.Cursed, cTraits.Cursed_2 });
            Charmed.CostInCharacterCreation = 3;
            Charmed.IsActive = true;
            Charmed.Upgrade = cTraits.Charmed_2;

            CustomTrait Charmed_2 = RogueLibs.CreateCustomTrait(cTraits.Charmed_2, false,
                new CustomNameInfo("Charmed & Dangerous +"),
                new CustomNameInfo("You are *really* lucky. Anyone who's been at the urinal next to you can attest."));
            Charmed_2.Available = false;
            Charmed_2.AvailableInCharacterCreation = false;
            Charmed_2.CanRemove = false;
            Charmed_2.CanSwap = true;
            Charmed_2.Conflicting.AddRange(new string[] { cTraits.Charmed, cTraits.Cursed, cTraits.Cursed_2 });
            Charmed_2.CostInCharacterCreation = 6;
            Charmed_2.Upgrade = null;

            CustomTrait Cursed = RogueLibs.CreateCustomTrait(cTraits.Cursed, true,
                new CustomNameInfo("Unlucky"),
                new CustomNameInfo("You pissed in some old Gypsy lady's cereal, and you still refuse to apologize. She didn't like that."));
            Cursed.Available = true;
            Cursed.AvailableInCharacterCreation = true;
            Cursed.CanRemove = true;
            Cursed.CanSwap = false;
            Cursed.Conflicting.AddRange(new string[] { cTraits.Charmed, cTraits.Charmed_2, cTraits.Cursed_2 });
            Cursed.CostInCharacterCreation = -2;
            Cursed.IsActive = true;
            Cursed.Upgrade = null;

            CustomTrait Cursed_2 = RogueLibs.CreateCustomTrait(cTraits.Cursed_2, true,
                new CustomNameInfo("Unlucky +"),
                new CustomNameInfo("You bought up an old Indian graveyard, and there you built a black cat sanctuary and mirror-breakery. Not your best choice."));
            Cursed_2.Available = true;
            Cursed_2.AvailableInCharacterCreation = true;
            Cursed_2.CanRemove = true;
            Cursed_2.CanSwap = false;
            Cursed_2.Conflicting.AddRange(new string[] { cTraits.Cursed, cTraits.Charmed, cTraits.Charmed_2 });
            Cursed_2.CostInCharacterCreation = -4;
            Cursed_2.IsActive = true;
            Cursed_2.Upgrade = null;
            #endregion
            #region Magic - General
            CustomTrait Archmage = RogueLibs.CreateCustomTrait(cTraits.Archmage, true,
                new CustomNameInfo("Archmage"),
                new CustomNameInfo("You are an unrivalled master of the magical arts. Basically cheat mode for magical abilities, added by request."));
            Archmage.Available = true;
            Archmage.AvailableInCharacterCreation = true;
            Archmage.CanRemove = false;
            Archmage.CanSwap = false;
            Archmage.Conflicting.AddRange(new string[] { cTraits.FocusedCasting, cTraits.FocusedCasting_2, cTraits.MagicTraining, cTraits.MagicTraining_2, cTraits.ManaBattery, cTraits.ManaBattery_2, cTraits.WildCasting, cTraits.WildCasting_2 });
            Archmage.CostInCharacterCreation = 100;
            Archmage.IsActive = true;
            Archmage.Recommendations.AddRange(new string[] { "ChronomanticDilation", "PyromanticJet", "TelemanticBlink" });
            Archmage.SpecialAbilities.AddRange(new string[] { "ChronomanticDilation", "PyromanticJet", "TelemanticBlink" });
            Archmage.Upgrade = null;

            CustomTrait FocusedCasting = RogueLibs.CreateCustomTrait(cTraits.FocusedCasting, true,
                new CustomNameInfo("Focused Casting"),
                new CustomNameInfo("You've carefully refined your magic techniques to improve accuracy and reduce the chances of miscasting spells."));
            FocusedCasting.Available = true;
            FocusedCasting.AvailableInCharacterCreation = true;
            FocusedCasting.CanRemove = false;
            FocusedCasting.CanSwap = false;
            FocusedCasting.Conflicting.AddRange(new string[] { cTraits.WildCasting, cTraits.WildCasting_2 });
            FocusedCasting.CostInCharacterCreation = 3;
            FocusedCasting.IsActive = true;
            FocusedCasting.Recommendations.AddRange(new string[] { "ChronomanticDilation", "PyromanticJet", "TelemanticBlink" });
            FocusedCasting.SpecialAbilities.AddRange(new string[] { "ChronomanticDilation", "PyromanticJet", "TelemanticBlink" });
            FocusedCasting.Upgrade = cTraits.FocusedCasting_2;

            CustomTrait FocusedCasting_2 = RogueLibs.CreateCustomTrait(cTraits.FocusedCasting_2, true,
                new CustomNameInfo("Focused Casting +"),
                new CustomNameInfo("You've even more carefully refined your techniques even more to improve accuracy and reduce the chances of miscasting spells EVEN MORE."));
            FocusedCasting_2.Available = false;
            FocusedCasting_2.AvailableInCharacterCreation = false;
            FocusedCasting_2.CanRemove = false;
            FocusedCasting_2.CanSwap = false;
            FocusedCasting_2.Conflicting.AddRange(new string[] { cTraits.WildCasting, cTraits.WildCasting_2 });
            FocusedCasting_2.CostInCharacterCreation = 6;
            FocusedCasting_2.IsActive = true;
            FocusedCasting_2.Upgrade = null;

            CustomTrait MagicTraining = RogueLibs.CreateCustomTrait(cTraits.MagicTraining, true,
                new CustomNameInfo("Magic Training"),
                new CustomNameInfo("Improves your skills with any Magic Special Ability."));
            MagicTraining.Available = true;
            MagicTraining.AvailableInCharacterCreation = true;
            MagicTraining.CostInCharacterCreation = 5;
            MagicTraining.CanRemove = false;
            MagicTraining.CanSwap = false;
            MagicTraining.Conflicting.AddRange(new string[] { cTraits.MagicTraining_2 });
            MagicTraining.IsActive = true;
            MagicTraining.SpecialAbilities.AddRange(new string[] { "ChronomanticDilation", "PyromanticJet", "TelemanticBlink" });
            MagicTraining.Upgrade = cTraits.MagicTraining_2;

            CustomTrait MagicTraining_2 = RogueLibs.CreateCustomTrait(cTraits.MagicTraining_2, true,
                new CustomNameInfo("Magic Training +"),
                new CustomNameInfo("Further improves your skills with any Magic Special Ability."));
            MagicTraining_2.Available = false;
            MagicTraining_2.AvailableInCharacterCreation = false;
            MagicTraining_2.CostInCharacterCreation = 10;
            MagicTraining_2.CanRemove = false;
            MagicTraining_2.CanSwap = false;
            MagicTraining_2.Conflicting.AddRange(new string[] { cTraits.MagicTraining });
            MagicTraining_2.IsActive = true;
            MagicTraining_2.SpecialAbilities.AddRange(new string[] { "ChronomanticDilation", "PyromanticJet", "TelemanticBlink" });
            MagicTraining_2.Upgrade = null;

            CustomTrait ManaBattery = RogueLibs.CreateCustomTrait(cTraits.ManaBattery, true,
                new CustomNameInfo("Mana Battery"),
                new CustomNameInfo("You can store more mana. Nifty."));
            ManaBattery.Available = true;
            ManaBattery.AvailableInCharacterCreation = true;
            ManaBattery.CostInCharacterCreation = 2;
            ManaBattery.CanRemove = false;
            ManaBattery.CanSwap = false;
            ManaBattery.Conflicting.AddRange(new string[] { });
            ManaBattery.IsActive = true;
            ManaBattery.SpecialAbilities.AddRange(new string[] { "ChronomanticDilation", "PyromanticJet", "TelemanticBlink" });
            ManaBattery.Upgrade = cTraits.ManaBattery_2;

            CustomTrait ManaBattery_2 = RogueLibs.CreateCustomTrait(cTraits.ManaBattery_2, true,
                new CustomNameInfo("Mana Battery +"),
                new CustomNameInfo("You can store even more mana. Even niftier."));
            ManaBattery_2.Available = true;
            ManaBattery_2.AvailableInCharacterCreation = false;
            ManaBattery_2.CostInCharacterCreation = 4;
            ManaBattery_2.CanRemove = false;
            ManaBattery_2.CanSwap = false;
            ManaBattery_2.Conflicting.AddRange(new string[] { });
            ManaBattery_2.IsActive = true;
            ManaBattery_2.SpecialAbilities.AddRange(new string[] { "ChronomanticDilation", "PyromanticJet", "TelemanticBlink" });
            ManaBattery_2.Upgrade = null;

            CustomTrait WildCasting = RogueLibs.CreateCustomTrait(cTraits.WildCasting, true,
                new CustomNameInfo("Wild Casting"),
                new CustomNameInfo("You don't need all that safety shit. You wanna cast some damn spells! Your spells are more powerful, but you have a greater chance of miscasting them."));
            WildCasting.Available = true;
            WildCasting.AvailableInCharacterCreation = true;
            WildCasting.CanRemove = false;
            WildCasting.CanSwap = false;
            WildCasting.Conflicting.AddRange(new string[] { cTraits.FocusedCasting, cTraits.FocusedCasting_2 });
            WildCasting.CostInCharacterCreation = 3;
            WildCasting.IsActive = true;
            WildCasting.Recommendations.AddRange(new string[] { "ChronomanticDilation", "PyromanticJet", "TelemanticBlink" });
            WildCasting.SpecialAbilities.AddRange(new string[] { "ChronomanticDilation", "PyromanticJet", "TelemanticBlink" });
            WildCasting.Upgrade = cTraits.WildCasting_2;

            CustomTrait WildCasting_2 = RogueLibs.CreateCustomTrait("WildCasting_2", true,
                new CustomNameInfo("Wild Casting +"),
                new CustomNameInfo("You're addicted to power. Your spells are ridiculously powerful... and risky."));
            WildCasting_2.Available = false;
            WildCasting_2.AvailableInCharacterCreation = false;
            WildCasting_2.CanRemove = false;
            WildCasting_2.CanSwap = false;
            WildCasting_2.Conflicting.AddRange(new string[] { cTraits.FocusedCasting, cTraits.FocusedCasting_2});
            WildCasting_2.CostInCharacterCreation = 6;
            WildCasting_2.IsActive = true;
            WildCasting_2.Upgrade = null;
            #endregion
            #region Magic - Chronomantic Dilation
            CustomTrait RATS = RogueLibs.CreateCustomTrait(cTraits.RATS, true,
                new CustomNameInfo("R.A.T.S. Mk VI"),
                new CustomNameInfo("Resistance-Tec Assisted Targeting System. The latest cybernetic enhancement to shooting accuracy, crit chance, & some combat traits with a double effect when time is slowed down."));
            RATS.Available = true;
            RATS.AvailableInCharacterCreation = true;
            RATS.CanRemove = false;
            RATS.CanSwap = true;
            RATS.Conflicting.AddRange(new string[] { cTraits.RATS_2 });
            RATS.CostInCharacterCreation = 3;
            RATS.IsActive = true;
            RATS.Recommendations.AddRange(new string[] { vTraits.Butterfingerer, "ChronomanticDilation", vTraits.IncreasedCritChance, vTraits.Kneecapper, vTraits.UnCrits });
            RATS.Upgrade = cTraits.RATS_2;

            CustomTrait RATS_2 = RogueLibs.CreateCustomTrait(cTraits.RATS_2, true,
                new CustomNameInfo("R.A.T.S. Mk VII"),
                new CustomNameInfo("\"Various bug fixes and performance improvements.\" That's all the fucking update notes say. Also, they took out the headphone jack??"));
            RATS_2.Available = true;
            RATS_2.AvailableInCharacterCreation = true;
            RATS_2.CanRemove = false;
            RATS_2.CanSwap = false;
            RATS_2.Conflicting.AddRange(new string[] { cTraits.RATS });
            RATS_2.CostInCharacterCreation = 12;
            RATS_2.IsActive = true;
            RATS_2.Recommendations.AddRange(new string[] { vTraits.Butterfingerer, "ChronomanticDilation", vTraits.IncreasedCritChance, vTraits.Kneecapper, vTraits.UnCrits });
            RATS_2.Upgrade = null;
			#endregion
			#region Miscellaneous
			CustomTrait EagleEyes = RogueLibs.CreateCustomTrait(cTraits.EagleEyes, true,
				new CustomNameInfo("Eagle Eyes"),
				new CustomNameInfo("You can see further than normal. Hell, you can see further than *abnormal*."));
			EagleEyes.Available = true;
			EagleEyes.AvailableInCharacterCreation = true;
			EagleEyes.CanRemove = false;
			EagleEyes.CanSwap = true;
            EagleEyes.Conflicting.AddRange(new string[] { cTraits.EagleEyes_2, cTraits.Myopic, cTraits.Myopic2});
			EagleEyes.CostInCharacterCreation = 3;
			EagleEyes.IsActive = true;
			EagleEyes.Upgrade = cTraits.EagleEyes_2;

			CustomTrait EagleEyes_2 = RogueLibs.CreateCustomTrait(cTraits.EagleEyes_2, true,
				new CustomNameInfo("Eagle Eyes +"),
				new CustomNameInfo("You might have been a good sniper, but you were kicked out of training when they didn't believe that you worked better without a rifle scope."));
			EagleEyes_2.Available = true;
			EagleEyes_2.AvailableInCharacterCreation = true;
			EagleEyes_2.CanRemove = false;
			EagleEyes_2.CanSwap = false;
            EagleEyes_2.Conflicting.AddRange(new string[] { cTraits.EagleEyes, cTraits.Myopic, cTraits.Myopic2 });
			EagleEyes_2.CostInCharacterCreation = 6;
			EagleEyes_2.IsActive = true;
			EagleEyes_2.Upgrade = null;

            CustomTrait Myopic = RogueLibs.CreateCustomTrait(cTraits.Myopic, true,
				new CustomNameInfo("Myopic"),
				new CustomNameInfo("You can't see too far. In fact, you can't see far enough."));
			Myopic.Available = true;
			Myopic.AvailableInCharacterCreation = true;
			Myopic.CanRemove = true;
			Myopic.CanSwap = true;
            Myopic.Conflicting.AddRange(new string[] { cTraits.EagleEyes, cTraits.EagleEyes_2, cTraits.Myopic2 });
			Myopic.CostInCharacterCreation = -4;
			Myopic.IsActive = true;
			Myopic.Upgrade = null;

            CustomTrait Myopic_2 = RogueLibs.CreateCustomTrait(cTraits.Myopic2, true,
				new CustomNameInfo("Ultramyopic"),
				new CustomNameInfo("You tend to keep people at arm's length, where you can't see them."));
			Myopic_2.Available = true;
			Myopic_2.AvailableInCharacterCreation = true;
			Myopic_2.CanRemove = true;
			Myopic_2.CanSwap = true;
            Myopic_2.Conflicting.AddRange(new string[] { cTraits.EagleEyes, cTraits.EagleEyes_2, cTraits.Myopic });
			Myopic_2.CostInCharacterCreation = -8;
			Myopic_2.IsActive = true;
			Myopic_2.Upgrade = null;
            #endregion
            #region Social
            CustomTrait Domineering = RogueLibs.CreateCustomTrait(cTraits.Domineering, true,
                new CustomNameInfo(cTraits.Domineering),
                new CustomNameInfo("There's just something about how you carry yourself. Maybe it's the way you walk, or maybe it's the way you demand obedience from the weak around you. People will occasionally be Submissive to you. Kinky!"));
            Domineering.Available = true;
            Domineering.AvailableInCharacterCreation = true;
            Domineering.CanRemove = true;
            Domineering.CanSwap = false;
            Domineering.Conflicting.AddRange(new string[] { cTraits.Domineering_2 });
            Domineering.CostInCharacterCreation = 2;
            Domineering.IsActive = true;
            Domineering.Upgrade = cTraits.Domineering_2;

            CustomTrait Domineering_2 = RogueLibs.CreateCustomTrait(cTraits.Domineering_2, true,
                new CustomNameInfo("Domineering +"),
                new CustomNameInfo("Some people make sure their social skills work for them. You crack the whip! You're finding more and more Subs everywhere you look."));
            Domineering_2.Available = true;
            Domineering_2.AvailableInCharacterCreation = true;
            Domineering_2.CanRemove = true;
            Domineering_2.CanSwap = false;
            Domineering_2.Conflicting.AddRange(new string[] { cTraits.Domineering });
            Domineering_2.CostInCharacterCreation = 4;
            Domineering_2.IsActive = true;
            Domineering_2.Upgrade = null;

            CustomTrait GenerallyUnpleasant = RogueLibs.CreateCustomTrait(cTraits.GenerallyUnpleasant, true,
                new CustomNameInfo("Generally Unpleasant"),
                new CustomNameInfo("You have a certain way with people! It's... very annoying."));
            GenerallyUnpleasant.Available = true;
            GenerallyUnpleasant.AvailableInCharacterCreation = true;
            GenerallyUnpleasant.CanRemove = true;
            GenerallyUnpleasant.CanSwap = false;
            GenerallyUnpleasant.Conflicting.AddRange(new string[] { vTraits.Antisocial, vTraits.Charismatic, vTraits.FairGame, vTraits.FriendoftheCommonFolk, cTraits.GenerallyUnpleasant_2, vTraits.Malodorous, cTraits.Polarizing, cTraits.Polarizing_2, vTraits.Suspicious, vTraits.Wanted });
            GenerallyUnpleasant.CostInCharacterCreation = -3;
            GenerallyUnpleasant.IsActive = true;
            GenerallyUnpleasant.Upgrade = null;

            CustomTrait ObjectivelyUnpleasant = RogueLibs.CreateCustomTrait(cTraits.GenerallyUnpleasant_2, true,
                new CustomNameInfo("Objectively Unpleasant"),
                new CustomNameInfo("You chew with your mouth open. You rightfully have no friends in the world. You are scum. Everyone starts out Annoyed, including me."));
            ObjectivelyUnpleasant.Available = true;
            ObjectivelyUnpleasant.AvailableInCharacterCreation = true;
            ObjectivelyUnpleasant.CanRemove = true;
            ObjectivelyUnpleasant.CanSwap = false;
            ObjectivelyUnpleasant.Conflicting.AddRange(new string[] { vTraits.Antisocial, vTraits.Charismatic, vTraits.FairGame, vTraits.FriendoftheCommonFolk, cTraits.GenerallyUnpleasant, vTraits.Malodorous, cTraits.Polarizing, cTraits.Polarizing_2, vTraits.Suspicious, vTraits.Wanted });
            ObjectivelyUnpleasant.CostInCharacterCreation = -8;
            ObjectivelyUnpleasant.IsActive = true;
            ObjectivelyUnpleasant.Upgrade = null;

            CustomTrait Polarizing = RogueLibs.CreateCustomTrait(cTraits.Polarizing, true,
                new CustomNameInfo(cTraits.Polarizing_2),
                new CustomNameInfo("Everyone has an opinion on you, when they first meet you. Might be good or bad, but at least you feel noticed!"));
            Polarizing.Available = true;
            Polarizing.AvailableInCharacterCreation = true;
            Polarizing.CanRemove = false;
            Polarizing.CanSwap = true;
            Polarizing.Conflicting.AddRange(new string[] { vTraits.Antisocial, vTraits.Charismatic, vTraits.FairGame, vTraits.FriendoftheCommonFolk, cTraits.GenerallyUnpleasant, cTraits.GenerallyUnpleasant_2, vTraits.Malodorous, cTraits.Polarizing_2, vTraits.Suspicious, vTraits.Wanted });
            Polarizing.CostInCharacterCreation = 0;
            Polarizing.IsActive = true;
            Polarizing.Upgrade = cTraits.Polarizing_2;

            CustomTrait Polarizing_2 = RogueLibs.CreateCustomTrait(cTraits.Polarizing_2, true,
                new CustomNameInfo("Polarizing +"),
                new CustomNameInfo("People have *strong* opinions of you. Like me. I think you're just great."));
            Polarizing_2.Available = true;
            Polarizing_2.AvailableInCharacterCreation = true;
            Polarizing_2.CanRemove = false;
            Polarizing_2.CanSwap = true;
            Polarizing_2.Conflicting.AddRange(new string[] { vTraits.Antisocial, vTraits.Charismatic, vTraits.FairGame, vTraits.FriendoftheCommonFolk, cTraits.GenerallyUnpleasant, cTraits.GenerallyUnpleasant_2, vTraits.Malodorous, cTraits.Polarizing, vTraits.Suspicious, vTraits.Wanted });
            Polarizing_2.CostInCharacterCreation = 1;
            Polarizing_2.IsActive = true;
            Polarizing_2.Upgrade = null;

            CustomTrait Priors = RogueLibs.CreateCustomTrait(cTraits.Priors, true,
                new CustomNameInfo("Priors"),
                new CustomNameInfo("You have a long rap sheet, and the police know you by first name. They are sick of your shit, and just looking for an excuse to beat you down."));
            Priors.Available = true;
            Priors.AvailableInCharacterCreation = true;
            Priors.CanRemove = true;
            Priors.CanSwap = true;
            Priors.Conflicting.AddRange(new string[] { vTraits.Charismatic, vTraits.CopsDontCare, vTraits.CorruptionCosts, vTraits.FairGame, vTraits.Suspicious, vTraits.TheLaw, vTraits.Wanted });
            Priors.CostInCharacterCreation = -2;
            Priors.IsActive = true;
            Priors.Upgrade = null;

            CustomTrait VeiledThreats = RogueLibs.CreateCustomTrait(cTraits.VeiledThreats, true,
				new CustomNameInfo("Veiled Threats"),
				new CustomNameInfo("When you attempt to Bribe, Extort, Mug, or Threaten, a failure will turn the target Annoyed instead of Hostile."));
            VeiledThreats.Available = true;
            VeiledThreats.AvailableInCharacterCreation = true;
            VeiledThreats.CanRemove = false;
            VeiledThreats.CanSwap = true;
            VeiledThreats.Conflicting.AddRange(new string[] { });
            VeiledThreats.CostInCharacterCreation = 2;
            VeiledThreats.IsActive = true;
            VeiledThreats.Recommendations.AddRange(new string[] { vTraits.Extortionist, vTraits.Mugger });
            VeiledThreats.Upgrade = null;

            CustomTrait Warlord = RogueLibs.CreateCustomTrait(cTraits.Warlord, true,
                new CustomNameInfo(cTraits.Warlord),
                new CustomNameInfo("To crush your enemies, to see them fall at your feet -- to take their horses and goods and hear the lamentation of their women. That is best! You can convince anyone to accept your rule by force."));
            Warlord.Available = true;
            Warlord.AvailableInCharacterCreation = true;
            Warlord.CanRemove = false;
            Warlord.CanSwap = true;
            Warlord.Conflicting.AddRange(new string[] { });
            Warlord.CostInCharacterCreation = 16;
            Warlord.IsActive = true;
            Warlord.Upgrade = null;
            #endregion
            #region Spawns
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
                new CustomNameInfo("You found a sack of money, and the people to whom it belongs want it back with interest. Well, they certainly have your interest, but you've spent the principal. Pay it off at an ATM by Level 10, or else your kneecaps (and the rest of your body) will pay the price."));
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
            #region Stealth
            CustomTrait StealthBastardDeluxe = RogueLibs.CreateCustomTrait(cTraits.StealthBastardDeluxe, true,
                new CustomNameInfo("Stealth Bastard Deluxe"),
                new CustomNameInfo("You can also through broken windows without taking a scratch. You can also hide in Bathtubs, Plants, Pool Tables, and Big Tables. [Bug: If you get stuck between it and the wall, you might clip through the wall]"));
            StealthBastardDeluxe.Available = true;
            StealthBastardDeluxe.AvailableInCharacterCreation = true;
            StealthBastardDeluxe.CanRemove = false;
            StealthBastardDeluxe.CanSwap = true;
            StealthBastardDeluxe.Conflicting.AddRange(new string[] { vTraits.Loud });
            StealthBastardDeluxe.CostInCharacterCreation = 4;
            StealthBastardDeluxe.IsActive = true;
            StealthBastardDeluxe.Upgrade = null;

            CustomTrait UnderdarkCitizen = RogueLibs.CreateCustomTrait(cTraits.UnderdarkCitizen, true,
                new CustomNameInfo("Underdark Citizen"),
                new CustomNameInfo("You can navigate the city's sewers with ease. Their denizens no longer consider you an easy mark."));
            UnderdarkCitizen.Available = true;
            UnderdarkCitizen.AvailableInCharacterCreation = true;
            UnderdarkCitizen.CanRemove = false;
            UnderdarkCitizen.CanSwap = false;
            UnderdarkCitizen.Conflicting.AddRange(new string[] { vTraits.TheLaw });
            UnderdarkCitizen.CostInCharacterCreation = 2;
            UnderdarkCitizen.IsActive = true;
            UnderdarkCitizen.Upgrade = null;
            #endregion
            #region Tampering
            CustomTrait TamperTantrum = RogueLibs.CreateCustomTrait(cTraits.TamperTantrum, true,
                new CustomNameInfo("Tamper Tantrum"),
                new CustomNameInfo("Your tools take less wear from tampering."));
            TamperTantrum.Available = true;
            TamperTantrum.AvailableInCharacterCreation = true;
            TamperTantrum.CanRemove = false;
            TamperTantrum.CanSwap = true;
            TamperTantrum.Conflicting.AddRange(new string[] { });
            TamperTantrum.CostInCharacterCreation = 2;
            TamperTantrum.IsActive = true;
            TamperTantrum.Upgrade = cTraits.TamperTantrum_2;

            CustomTrait TamperTantrum_2 = RogueLibs.CreateCustomTrait(cTraits.TamperTantrum_2, false,
                new CustomNameInfo("Tamper Tantrum +"),
                new CustomNameInfo("Your tools take zero wear when used in tampering."));
            TamperTantrum_2.Available = false;
            TamperTantrum_2.AvailableInCharacterCreation = false;
            TamperTantrum_2.CanRemove = false;
            TamperTantrum_2.CanSwap = false;
            TamperTantrum_2.Conflicting.AddRange(new string[] { });
            TamperTantrum_2.CostInCharacterCreation = 5;
            TamperTantrum_2.IsActive = true;
            TamperTantrum_2.Upgrade = null;
            #endregion
        }
        #endregion
        #region Custom Methods
        public static List<T> ConcatTraitLists<T>(params IEnumerable<T>[] enums)
            => enums.SelectMany(e => e).ToList();
        internal static string HealthCost(Agent agent, int baseDamage, DamageType type)
        {
            BMLog("HealthCost");

            if (type == DamageType.burnedFingers)
            {
                if (agent.statusEffects.hasTrait(vTraits.ResistFire) || agent.statusEffects.hasTrait(vTraits.FireproofSkin) || agent.statusEffects.hasTrait(vTraits.FireproofSkin2))
                    return "0";
            }
            else if (type == DamageType.brokenWindow)
            {
                if (agent.statusEffects.hasTrait(cTraits.StealthBastardDeluxe))
                    return "0";
                else if (agent.statusEffects.hasTrait(vTraits.Diminutive))
                    return (baseDamage / 2).ToString();
            }

            return baseDamage.ToString();
        }
        public static bool IsTraitActive(string trait)
		{
            BMLog("IsTraitActive");

            foreach (Agent agent in GC.playerAgentList)
                if (agent.isPlayer != 0)
                    foreach (Trait se in agent.statusEffects.TraitList)
				    {
                        BMLog("Trait: '" + se.traitName + "'");

                        if (se.traitName == trait)
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
            if (agent.statusEffects.hasTrait(cTraits.TamperTantrum))
                return (baseCost / 2);

            if (agent.statusEffects.hasTrait(cTraits.TamperTantrum_2))
                return 0;

            return baseCost;
        }
		#endregion

		#region Agent
        public static bool Agent_CanShakeDown(ref bool __result) // Prefix
		{
            if (BMTraits.IsTraitActive(cTraits.Warlord))
			{
                __result = true;
                return false;
			}
            
            return true;
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
                (__instance.agent.statusEffects.hasTrait(cTraits.DrawNoBlood) && item.Categories.Contains("Piercing")) ||
                (__instance.agent.statusEffects.hasTrait(cTraits.AfraidOfLoudNoises) && item.Categories.Contains("Loud") && !item.contents.Contains("Silencer")) ||
                (__instance.agent.statusEffects.hasTrait(cTraits.DrawNoBlood) && item.Categories.Contains("Blunt"))
            )
                __result = false;

            // TODO: See also InvDatabase.ChooseWeapon
        }
        public static bool InvDatabase_EquipArmor(InvItem item, bool sfx, InvDatabase __instance) // Prefix
		{
            if (item.isArmor && __instance.agent.statusEffects.hasTrait(cTraits.Fatass))
            {
                __instance.agent.Say("I'm too fuckin' fat to wear this!");
                GC.audioHandler.Play(__instance.agent, "CantDo");

                return false;
            }

            return true;
		}
        public static bool InvDatabase_EquipArmorHead(InvItem item, bool sfx, InvDatabase __instance) // Prefix
		{
            if (item.isArmorHead && item != null && __instance.agent.statusEffects.hasTrait(cTraits.FatHead))
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

            if (agent.statusEffects.hasTrait(cTraits.DrawNoBlood) && item.Categories.Contains("Piercing"))
            {
                agent.Say("Mommy says I can't use sharp things!");
                GC.audioHandler.Play(__instance.agent, "CantDo");

                return false;
            }
            else if (agent.statusEffects.hasTrait(cTraits.AfraidOfLoudNoises) && item.Categories.Contains("Loud") && !item.contents.Contains("Silencer"))
            {
                agent.Say("I can't use that! It's too loooooud.");
                GC.audioHandler.Play(__instance.agent, "CantDo");

                return false;
            }
            else if (agent.statusEffects.hasTrait(cTraits.DrawNoBlood) && item.Categories.Contains("Blunt"))
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
                if (__instance.agent.statusEffects.hasTrait(cTraits.TamperTantrum_2))
                    amount = 0;
                else if (__instance.agent.statusEffects.hasTrait(cTraits.TamperTantrum))
                    amount /= 2;
            }
            return true;
		}
        public static bool InvDatabase_SubtractFromItemCount_b(InvItem invItem, ref int amount, bool toolbarMove, InvDatabase __instance) // Prefix
		{
            if (BMHeader.tools.Contains(invItem.invItemName))
            {
                if (__instance.agent.statusEffects.hasTrait(cTraits.TamperTantrum_2))
                    amount = 0;
                else if (__instance.agent.statusEffects.hasTrait(cTraits.TamperTantrum))
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
                    __instance.Categories.Add(cTraits.Vegetarian);
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

            if (cats.Contains("Alcohol") && (agent.statusEffects.hasTrait(cTraits.FriendOfBill) || agent.statusEffects.hasTrait(cTraits.Teetotaller)))
            {
                agent.Say("Today, I choose not to drink.");
                cantDoFlag = true;
            }
            if (cats.Contains("Drugs") && (agent.statusEffects.hasTrait(cTraits.DAREdevil) || agent.statusEffects.hasTrait(cTraits.Teetotaller)))
            {
                agent.Say("Nope, my body is a temple!");
                cantDoFlag = true;
            }
            if (cats.Contains("NonVegetarian") && agent.statusEffects.hasTrait(cTraits.Vegetarian))
            {
                agent.Say("Meat is murder!");
                cantDoFlag = true;
            }
            if (cats.Contains(cTraits.Vegetarian) && agent.statusEffects.hasTrait(cTraits.Carnivore))
            {
                agent.Say("No! Me want meat!");
                cantDoFlag = true;
            }
            if (cats.Contains("Loud") && agent.statusEffects.hasTrait(cTraits.AfraidOfLoudNoises))
			{
                agent.Say("But that'll hurt my little ears!");
                cantDoFlag = true;
            }
            if (cats.Contains("Piercing") && agent.statusEffects.hasTrait(cTraits.DrawNoBlood))
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
                (cats.Contains("Alcohol") && (traits.hasTrait(cTraits.FriendOfBill) || traits.hasTrait(cTraits.Teetotaller)) ) ||
                (cats.Contains("Drugs") && (traits.hasTrait(cTraits.DAREdevil) || traits.hasTrait(cTraits.Teetotaller)) ) ||
                (cats.Contains(cTraits.Vegetarian) && traits.hasTrait(cTraits.Carnivore)) ||
                (cats.Contains("NonVegetarian") && traits.hasTrait(cTraits.Vegetarian))
            )
                __result = 0;
            if (traits.hasTrait(cTraits.Fatass))
                __result = (int)((float)__result * 1.5f);
		}
        public static bool ItemFunctions_UseItem(InvItem item, Agent agent, ItemFunctions __instance) // Prefix ***new
		{
            if (item.itemType == "Consumable")
			{
                if (BMHeader.alcohol.Contains(item.invItemName) && ((agent.statusEffects.hasTrait(cTraits.FriendOfBill) || agent.statusEffects.hasTrait(cTraits.Teetotaller))))
                {
                    agent.Say("Today, I choose not to drink.");
                    goto terminus;
                }

                if (BMHeader.drugs.Contains(item.invItemName) && (agent.statusEffects.hasTrait(cTraits.DAREdevil) || agent.statusEffects.hasTrait(cTraits.Teetotaller)))
                {
                    agent.Say("Nope, my body is a temple!");
                    goto terminus;
                }

                if (BMHeader.nonVegetarian.Contains(item.invItemName) && agent.statusEffects.hasTrait(cTraits.Vegetarian))
                {
                    agent.Say("Meat is murder!");
                    goto terminus;
                }
                else if (BMHeader.vegetarian.Contains(item.invItemName) && agent.statusEffects.hasTrait(cTraits.Carnivore))
                {
                    agent.Say("No! Me want meat!");
                    goto terminus;
                }
            }
			else
            {
                if (BMHeader.loud.Contains(item.invItemName) && agent.statusEffects.hasTrait(cTraits.AfraidOfLoudNoises))
                {
                    agent.Say("But that'll hurt my little ears!");
                    goto terminus;
                }

                if (BMHeader.piercing.Contains(item.invItemName) && agent.statusEffects.hasTrait(cTraits.DrawNoBlood))
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

            if (agent.statusEffects.hasTrait(cTraits.Charmed))
                luckMultiplier = 1;
            else if (agent.statusEffects.hasTrait(cTraits.Charmed_2))
                luckMultiplier = 2;
            else if (agent.statusEffects.hasTrait(cTraits.Cursed))
                luckMultiplier = -1;
            else if (agent.statusEffects.hasTrait(cTraits.Cursed_2))
                luckMultiplier = -2;

            if (RATStargetable)
            {
                if (agent.statusEffects.hasTrait(cTraits.RATS))
                    luckMultiplier += 1;
                if (agent.statusEffects.hasTrait(cTraits.RATS_2))
                    luckMultiplier += 2;

                if (agent.isPlayer != 0 && agent.specialAbility == "ChronomanticDilation")
                    if (BMAbilities.MSA_CD_IsCast(agent))
                        luckMultiplier *= 2;
            }

            __result = Mathf.Clamp(__result + luckBonus * luckMultiplier, 0, 100);
        }
		#endregion
		#region Relationships
        public static void Relationships_SetupRelationshipOriginal(Agent otherAgent, Relationships __instance, ref Agent ___agent) // Postfix
		{
            if (__instance.GetRel(otherAgent) == "Neutral")
            {
                int roll = Random.Range(0, 100);
                string newRel = "";

                if (___agent.statusEffects.hasTrait(cTraits.GenerallyUnpleasant))
                {
                    if (roll <= 20)
                        newRel = "Annoyed";
                }
                else if (___agent.statusEffects.hasTrait(cTraits.GenerallyUnpleasant_2))
                    newRel = "Annoyed";
                else if (___agent.statusEffects.hasTrait(cTraits.Polarizing_2))
				{
                    if (roll <= 50)
                        newRel = "Annoyed";
                    else
                        newRel = "Friendly";
				}
                else if (___agent.statusEffects.hasTrait(cTraits.Polarizing_2))
				{
                    if (roll <= 25)
                        newRel = "Hostile";
                    else if (roll <= 50)
                        newRel = "Annoyed";
                    else if (roll <= 67)
                        newRel = "Friendly";
                    else if (roll <= 88)
                        newRel = "Loyal";
                    else if (roll <= 100)
                        newRel = "Aligned";
				}

                roll = Random.Range(0, 100);

                if (___agent.statusEffects.hasTrait(cTraits.Domineering))
				{
                    if (roll <= 10)
                        newRel = "Submissive";
				}
                else if (___agent.statusEffects.hasTrait(cTraits.Domineering_2))
				{
                    if (roll <= 20)
                        newRel = "Submissive";
				}

                if (newRel != "")
                    __instance.SetRelInitial(otherAgent, newRel);
            }
		}
        #endregion
        #region SkillPoints
        public static bool SkillPoints_AddPointsLate(string pointsType, int extraNum, ref IEnumerator __result, SkillPoints __instance, ref Agent ___agent) // Prefix
        {
            BMLog("SkillPoints_AddPointsLate");

            __result = SkillPoints_AddPointsLate_IEnumerator(pointsType, extraNum, __result, __instance, ___agent);

            return false;
        }
        private static IEnumerator SkillPoints_AddPointsLate_IEnumerator(string pointsType, int extraNum, IEnumerator __result, SkillPoints __instance, Agent ___agent)
        {
            BMLog("SkillPoints_AddPointsLate_IEnumerator:");
            BMLog("\tpointsType = " + pointsType);
            BMLog("\textraNum = " + extraNum);

			if (pointsType == "DestructionPoints" || pointsType == "DestructionPoints2" || pointsType == "FireExtinguishPoints")
				yield return null;
			else
				yield return new WaitForSeconds(0.3f);
			
			if (Time.timeScale == 0f && !GC.multiplayerMode)
			{
				while (Time.timeScale == 0f)
					yield return null;
			
				yield return new WaitForSeconds(0.2f);
			}
			
			int xpReward = 0;
			bool suppressAnimation = false;
			string text = pointsType;
			
			switch (text)
            {
                case "ArrestedPoints":
                    xpReward = 100;
                    break;
                case "ArrestedPointsInnocent":
                    if (___agent.statusEffects.hasTrait(vTraits.TheLaw) && !___agent.oma.superSpecialAbility)
					{
                        if (___agent.statusEffects.hasTrait(vTraits.Crooked))
                            xpReward = -40;
                        else if (___agent.statusEffects.hasTrait(vTraits.Crooked2))
                            xpReward = 0;
                        else
                            xpReward = -80;
                    }
					else
                        xpReward = 10;
                    break;
                case "BigQuestBonusDowntown":
                    xpReward = 500;
                    break;
                case "BigQuestBonusFloor":
                    xpReward = 300;
                    break;
                case "BigQuestBonusGame":
                    xpReward = 1000;
                    break;
                case "BigQuestBonusIndustrial":
                    xpReward = 500;
                    break;
                case "BigQuestBonusPark":
                    xpReward = 500;
                    break;
                case "BigQuestBonusSlums":
                    xpReward = 500;
                    break;
                case "BigQuestBonusUptown":
                    xpReward = 500;
                    break;
                case "CompleteMission":
                    switch (extraNum)
                    {
                        case 1:
                            xpReward = 300;
                            break;
                        case 2:
                            xpReward = 300;
                            break;
                        case 3:
                            xpReward = 300;
                            break;
                        case 4:
                            xpReward = 300;
                            break;
                        case 5:
                            xpReward = 300;
                            break;
                    }
                    break;
                case "CompleteMissionFindBombs":
                    xpReward = 700;
                    break;
                case "CompleteMissionReduced":
                    switch (extraNum)
                    {
                        case 1:
                            xpReward = 150;
                            break;
                        case 2:
                            xpReward = 150;
                            break;
                        case 3:
                            xpReward = 150;
                            break;
                        case 4:
                            xpReward = 150;
                            break;
                        case 5:
                            xpReward = 150;
                            break;
                    }
                    break;
                case "Destruction":
                    xpReward = 200;
                    break;
                case "DestructionPoints":
                    xpReward = 1;
                    suppressAnimation = true;
                    break;
                case "DestructionPoints2":
                    xpReward = 2;
                    suppressAnimation = true;
                    break;
                case "DisarmDetonatorPoints":
                    xpReward = 20;
                    break;
                case "ElectabilityBonus":
                    xpReward = 100;
                    break;
                case "Enslaved":
                    xpReward = 30;
                    break;
                case "FindTreasure":
                    xpReward = 100;
                    GC.stats.AddToStat(___agent, "TreasuresFound", 1);
                    break;
                case "FireExtinguishPoints":
                    xpReward = 5;
                    suppressAnimation = true;
                    break;
                case "FreedPrisoner":
                    xpReward = 20 * extraNum;
                    if (extraNum > 1)
                        pointsType = "FreedPrisoners";
                    break;
                case "FreedSlave":
                    xpReward = 50 * extraNum;
                    if (extraNum > 1)
                        pointsType = "FreedSlaves";
                    break;
                case "HackPoints":
                    xpReward = 20;
                    break;
                case "IndirectlyKill":
                    xpReward = 30;
                    break;
                case "IndirectlyKillInnocent":
                    if (___agent.statusEffects.hasTrait(vTraits.TheLaw) && !___agent.oma.superSpecialAbility)
					{
                        if (___agent.statusEffects.hasTrait(vTraits.Crooked))
                            xpReward = -15;
                        else if (___agent.statusEffects.hasTrait(vTraits.Crooked2))
                            xpReward = 0;
                        else
                            xpReward = -30;
                    }
                    else
                        xpReward = 10;
                    break;
                case "IndirectlyKillRival":
                    xpReward = 90;
                    break;
                case "Joke":
                    xpReward = 30 * extraNum;
                    break;
                case "KilledRobot":
                    xpReward = 1000;
                    break;
                case "KillPoints":
                    xpReward = 50;
                    break;
                case "KillPointsInnocent":
                    if (___agent.statusEffects.hasTrait(vTraits.TheLaw) && !___agent.oma.superSpecialAbility)
					{
                        if (___agent.statusEffects.hasTrait(vTraits.Crooked))
                            xpReward = -20;
                        else if (___agent.statusEffects.hasTrait(vTraits.Crooked2))
                            xpReward = 0;
                        else if (!___agent.oma.superSpecialAbility)
                            xpReward = -40;
                    }
                    else
                        xpReward = 10;
                    break;
                case "KillPointsRival":
                    xpReward = 150;
                    break;
                case "KnockOutPoints":
                    xpReward = 75;
                    break;
                case "KnockOutPointsInnocent":
                    if (___agent.statusEffects.hasTrait(vTraits.TheLaw) && !___agent.oma.superSpecialAbility)
					{
                        if (___agent.statusEffects.hasTrait(vTraits.Crooked))
                            xpReward = -20;
                        else if (___agent.statusEffects.hasTrait(vTraits.Crooked2))
                            xpReward = 0;
                        else
                            xpReward = -40;
                    }
                    else
                        xpReward = 10;
                    break;
                case "KnockOutPointsRival":
                    xpReward = 150;
                    break;
                case "LockpickPoints":
                    xpReward = 20;
                    break;
                case "ManySleeping":
                    xpReward = 100;
                    break;
                case "Massacre":
                    xpReward = 100;
                    break;
                case "NoAngerLevel":
                    xpReward = 100;
                    break;
                case "NoDamageTaken":
                    xpReward = 100;
                    break;
                case "NoDestruction":
                    xpReward = 200;
                    break;
                case "NoGuns":
                    xpReward = 200;
                    break;
                case "NoKillBonus":
                    xpReward = 100;
                    break;
                case "NoKillLevel":
                    xpReward = 100;
                    break;
                case "NotAlertedBonus":
                    xpReward = 100;
                    break;
                case "OnlyFists":
					xpReward = 200;
					break;
                case "PickpocketPoints":
                    if (___agent.statusEffects.hasTrait(vTraits.TheLaw) && !___agent.oma.superSpecialAbility && !___agent.statusEffects.hasTrait(vTraits.PromiseIllReturnIt))
                    {
                        if (___agent.statusEffects.hasTrait(vTraits.Crooked))
                            xpReward = -10;
                        else if (___agent.statusEffects.hasTrait(vTraits.Crooked2))
                            xpReward = 0;
                        else
                            xpReward = -15;
                    }
                    else
                        xpReward = 15;
                    break;
                case "PoisonAirPoints":
                    xpReward = 20;
                    break;
                case "RemoveSlaveHelmetPoints":
                    xpReward = 20;
                    break;
                case "RemoveWindowPoints":
					xpReward = 20;
					break;
                case "ShakedownFailPoints":
                    xpReward = -100;
                    break;
                case "ShakedownPoints":
                    xpReward = 100;
                    break;
                case "StealPoints":
                    switch (extraNum)
                    {
                        case 1:
                            xpReward = 10;
                            break;
                        case 2:
                            xpReward = 20;
                            break;
                        case 3:
                            xpReward = 30;
                            break;
                    }
                    break;
                case "StealPointsNegative":
                    xpReward = extraNum * 10;

                    if (___agent.statusEffects.hasTrait(vTraits.TheLaw) && !___agent.oma.superSpecialAbility && !___agent.statusEffects.hasTrait(vTraits.PromiseIllReturnIt))
                    {
                        xpReward *= -1;

                        if (___agent.statusEffects.hasTrait(vTraits.Crooked))
                            xpReward /= 2;
                        else if (___agent.statusEffects.hasTrait(vTraits.Crooked2))
                            xpReward = 0;
                    }
                    break;
                case "StoleLots":
                    xpReward = 200;
                    break;
                case "TamperGeneratorPoints":
                    xpReward = 20;
                    break;
                case "TamperLaserEmitterPoints":
                    xpReward = 20;
                    break;
                case "TamperPoliceBoxPoints":
                    xpReward = 20;
                    break;
                case "TamperSatelliteDishPoints":
                    xpReward = 20;
                    break;
                case "TimeBonus":
                    xpReward = 100;
                    break;
                case "TwoPlayerWinner1":
                    xpReward = 200;
                    break;
                case "TwoPlayerWinner2":
                    xpReward = 200;
                    break;
                case "UnlockSafePoints":
                    xpReward = 20;
                    break;
                case "WonElectionPoints":
                    xpReward = 100;
                    break;
            }
			
            if (xpReward != 0)
			{
				if (xpReward > 0)
				{
					if (___agent.statusEffects.hasTrait(vTraits.Studious))
						xpReward = (int)((float)xpReward * 1.3f);

					if (___agent.statusEffects.hasTrait(vTraits.Studious2))
						xpReward = (int)((float)xpReward * 1.5f);
				}

				float floorXpAcceleration = 0.075f;
				int cityFloor = Mathf.Clamp(GC.sessionDataBig.curLevelEndless, 1, 16);

				if (GC.sessionDataBig.challenges.Contains("QuickGame"))
				{
					floorXpAcceleration *= 1.5f;
					cityFloor = Mathf.Clamp(GC.sessionDataBig.curLevelEndless, 1, 11);
				}

				xpReward = (int)((float)xpReward * (1f + (float)(cityFloor - 1) * floorXpAcceleration));
				GC.sessionData.skillPoints[___agent.isPlayer] += xpReward;
				Color32 myColor;

				if (xpReward < 0)
				{
					myColor = new Color32(byte.MaxValue, 0, 0, byte.MaxValue);

					if (GC.sessionData.skillPoints[___agent.isPlayer] < __instance.findLevelThreshold(GC.sessionData.skillLevel[___agent.isPlayer] - 1))
						GC.sessionData.skillPoints[___agent.isPlayer] = __instance.findLevelThreshold(GC.sessionData.skillLevel[___agent.isPlayer] - 1);
				}
				else
					myColor = new Color32(byte.MaxValue, 216, 0, byte.MaxValue);
				
                if (GC.sessionData.skillPoints[___agent.isPlayer] >= __instance.findLevelThreshold(GC.sessionData.skillLevel[___agent.isPlayer]))
				{
					GC.audioHandler.Play(___agent, "LevelUp");

					if (___agent.isPlayer == 1)
						GC.alienFX.GainLevel();
					
                    GC.sessionData.skillLevel[___agent.isPlayer]++;
					__instance.levelsGained++;
					GC.sessionData.levelsGained[___agent.isPlayer]++;
					__instance.justGainedLevel = true;

                    //__instance.StartCoroutine(__instance.CancelJustGainedLevel()); // Original Private Method Inaccessible

                    MethodInfo CancelJustGainedLevel = AccessTools.DeclaredMethod(typeof(SkillPoints), "CancelJustGainedLevel", new Type[0] { });
                    CancelJustGainedLevel.GetMethodWithoutOverrides<Action>(__instance).Invoke();
					
                    if (GC.unlocks.CanDoUnlocks())
					{
						if (___agent.statusEffects.hasTrait(vTraits.Studious2))
						{
							GC.unlocks.AddNuggets(2);
							GC.spawnerMain.SpawnStatusText(___agent, "ItemPickupSlower", "Nuggets", "Item", "Add2Nuggets", "");
						}
						else
						{
							GC.unlocks.AddNuggets(3);
							GC.spawnerMain.SpawnStatusText(___agent, "ItemPickupSlower", "Nuggets", "Item", "Add3Nuggets", "");
						}
					}

					___agent.objectMult.SendChatAnnouncement("LevelUp", GC.sessionData.skillLevel[___agent.isPlayer].ToString(), "");

					if (___agent.localPlayer)
						___agent.skillBar.dTr.localScale = new Vector3(0f, ___agent.skillBar.dTr.localScale.y, ___agent.skillBar.dTr.localScale.z);
					
                    if (!___agent.finishedLevel)
						GC.spawnerMain.SpawnStatusText(___agent, "LevelUp", "LevelUp", "Interface");
					
                    if (___agent.statusEffects.hasTrait(vTraits.PotentialtoNotSuck))
					{
						Agent agent = ___agent;

						if (___agent.possessing || ___agent.transforming)
						{
							if (!GC.multiplayerMode)
							{
								if (___agent.isPlayer == 1)
									agent = GC.backupAgent1;
								
                                if (___agent.isPlayer == 2)
									agent = GC.backupAgent2;
								
                                if (___agent.isPlayer == 3)
									agent = GC.backupAgent3;
							
                                if (___agent.isPlayer == 4)
									agent = GC.backupAgent4;
							}
							else
							{
								if (___agent.playerColor == 1)
									agent = GC.backupAgent1;
								
                                if (___agent.playerColor == 2)
									agent = GC.backupAgent2;
								
                                if (___agent.playerColor == 3)
									agent = GC.backupAgent3;
							
                                if (___agent.playerColor == 4)
									agent = GC.backupAgent4;
							}
						}

						if (GC.sessionData.skillLevel[___agent.isPlayer] % 2 == 0 && (agent.strengthStatMod != 3 || agent.enduranceStatMod != 3 || agent.accuracyStatMod != 3 || agent.speedStatMod != 3))
						{
							string randStatMod;
							bool bonusStat;

							do
							{
								randStatMod = GC.Choose<string>("Strength", "Endurance", "Accuracy", "Speed");
								bonusStat = true;

								if (randStatMod == "Strength" && agent.strengthStatMod == 3)
									bonusStat = false;
								else if (randStatMod == "Endurance" && agent.enduranceStatMod == 3)
									bonusStat = false;
								else if (randStatMod == "Accuracy" && agent.accuracyStatMod == 3)
									bonusStat = false;
								else if (randStatMod == "Speed" && agent.speedStatMod == 3)
									bonusStat = false;
							}
							while (!bonusStat);

                            switch (randStatMod)
							{
                                case "Accuracy":
                                    agent.SetAccuracy(agent.accuracyStatMod + 1, true);
                                    GC.spawnerMain.SpawnStatusText(___agent, "BuffSpecial", "Accuracy", "BuffSpecial", "");

                                    break;
                                case "Endurance":
                                    agent.SetEndurance(agent.enduranceStatMod + 1, true);
                                    GC.spawnerMain.SpawnStatusText(___agent, "BuffSpecial", "Endurance", "BuffSpecial", "");

                                    break;
                                case "Speed":
                                    agent.SetSpeed(agent.speedStatMod + 1, true);
                                    GC.spawnerMain.SpawnStatusText(___agent, "BuffSpecial", "Speed", "BuffSpecial", "");

                                    break;
                                case "Strength":
                                    agent.SetStrength(agent.strengthStatMod + 1, true);
                                    GC.spawnerMain.SpawnStatusText(___agent, "BuffSpecial", "Strength", "BuffSpecial", "");

                                    break;
                            }
						}
					}

					if (___agent.health > 0f && !GC.menuGUI.demoOver)
					{
						if (___agent.possessing)
						{
							if (!GC.multiplayerMode)
							{
								if (___agent.isPlayer == 1)
									GC.backupAgent1.health = GC.backupAgent1.healthMax;
								
                                if (___agent.isPlayer == 2)
									GC.backupAgent2.health = GC.backupAgent2.healthMax;
								
                                if (___agent.isPlayer == 3)
									GC.backupAgent3.health = GC.backupAgent3.healthMax;
								
                                if (___agent.isPlayer == 4)
									GC.backupAgent4.health = GC.backupAgent4.healthMax;
							}
							else
							{
								if (___agent.playerColor == 1)
									GC.backupAgent1.health = GC.backupAgent1.healthMax;
								
                                if (___agent.playerColor == 2)
									GC.backupAgent2.health = GC.backupAgent2.healthMax;
								
                                if (___agent.playerColor == 3)
									GC.backupAgent3.health = GC.backupAgent3.healthMax;
								
                                if (___agent.playerColor == 4)
									GC.backupAgent4.health = GC.backupAgent4.healthMax;
							}

							GC.spawnerMain.SpawnStatusText(___agent, "HealthUpSlower", "FullHealth", "StatusEffect");
						}
						else
							___agent.statusEffects.ChangeHealth(___agent.healthMax);
					}
					else
						___agent.fullHealthAfterResurrect = true;
					
                    if (___agent.localPlayer && !suppressAnimation)
					{
						if (GC.fourPlayerMode)
							__instance.SpawnSkillPointsStatusText(pointsType, xpReward);
						else
						{
							___agent.skillBar.StartChange(true);
							___agent.skillBar.StartAnim(xpReward, pointsType, myColor);
						}
					}

					if (___agent.completingBigQuestLevel)
						GC.quests.SpawnBigQuestCompletedText2(___agent, true);
					else if (___agent.failingBigQuestLevel)
						GC.quests.SpawnBigQuestFailedText2(___agent, true);
				}
				else if (___agent.localPlayer && !suppressAnimation)
				{
					if (GC.fourPlayerMode)
					{
						__instance.SpawnSkillPointsStatusText(pointsType, xpReward);

						if (___agent.completingBigQuestLevel)
							GC.quests.SpawnBigQuestCompletedText2(___agent, false);
						else if (___agent.failingBigQuestLevel)
							GC.quests.SpawnBigQuestFailedText2(___agent, false);
					}
					else
					{
						___agent.skillBar.StartChange(false);
						___agent.skillBar.StartAnim(xpReward, pointsType, myColor);
					}
				}

				if (___agent.localPlayer)
					___agent.skillBar.UpdateSkillText();
			}

			yield break;
		}
		#endregion
		#region StatusEffects
		public static void StatusEffects_AddTrait(string traitName, bool isStarting, bool justRefresh, StatusEffects __instance) // Postfix
		{
            Agent agent = __instance.agent;

            if (traitName == cTraits.Fatass)
			{
                agent.SetEndurance(agent.enduranceStatMod + 1);
                agent.SetSpeed(agent.speedStatMod - 1);
			}
		}
        public static void StatusEffects_BecomeHidden(ObjectReal hiddenInObject, StatusEffects __instance) // Postfix
		{
            Agent agent = __instance.agent;

            if (IsTraitActive(cTraits.UnderdarkCitizen) && agent.isPlayer == 0)
			{
                agent.statusEffects.BecomeNotHidden();
			}
		}
        public static void StatusEffects_RemoveTrait(string traitName, bool onlyLocal, StatusEffects __instance) // Postfix
		{
            Agent agent = __instance.agent;
            if (traitName == cTraits.Fatass)
			{
                //TODO: CharacterCreation.CreatePointTallyText() for stat mods
                agent.SetEndurance(agent.enduranceStatMod - 1);
                agent.SetSpeed(agent.speedStatMod + 1);
			}
		}
		#endregion
	}
}
