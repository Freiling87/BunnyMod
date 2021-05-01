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
            ScrollingMenu_00();
            CustomTrait DebugTrait = RogueLibs.CreateCustomTrait(cTrait.Debug, true,
                new CustomNameInfo(cTrait.Debug),
                new CustomNameInfo("Debug trait. If you see this, I hope you're me."));
            DebugTrait.Available = true;
            DebugTrait.AvailableInCharacterCreation = true;
            DebugTrait.CanRemove = false;
            DebugTrait.CanSwap = false;
            DebugTrait.CostInCharacterCreation = 0;
            DebugTrait.IsActive = true;
            #region Combat
            CustomTrait Ballistician = RogueLibs.CreateCustomTrait(cTrait.Ballistician, true,
                new CustomNameInfo("000: Ballistician"),
                new CustomNameInfo("You look around you, and all you see is trajectories.\n\nYour bullets fly faster, further & harder."));
            Ballistician.Available = true;
            Ballistician.AvailableInCharacterCreation = true;
            Ballistician.CanRemove = false;
            Ballistician.CanSwap = true;
            Ballistician.Conflicting.AddRange(new string[] { cTrait.Myopic, cTrait.Myopic2 });
            Ballistician.CostInCharacterCreation = 4;
            Ballistician.IsActive = true;
            Ballistician.Upgrade = cTrait.Ballistician_2;

            CustomTrait Ballistician2 = RogueLibs.CreateCustomTrait(cTrait.Ballistician_2, true,
                new CustomNameInfo("000: Ballistician +"),
                new CustomNameInfo("When you're shooting people, you like to yell, \"Pew Pew!\" Who's going to stop you, anyway?"));
            Ballistician2.Available = true;
            Ballistician2.AvailableInCharacterCreation = false;
            Ballistician2.CanRemove = false;
            Ballistician2.CanSwap = true;
            Ballistician2.Conflicting.AddRange(new string[] { cTrait.Myopic, cTrait.Myopic2 });
            Ballistician2.CostInCharacterCreation = 8;
            Ballistician2.IsActive = true;
            Ballistician2.Upgrade = null;

            CustomTrait DoubleTapper = RogueLibs.CreateCustomTrait(cTrait.DoubleTapper, true,
                new CustomNameInfo("Double-Tapper"),
                new CustomNameInfo("You're not some kind of monster. A quick bullet to the back of the head is more humane than the alternatives, isn't it?\n\nBullet attacks from behind within melee range do 2x damage. 10x if you're invisible or hidden."));
            DoubleTapper.Available = true;
            DoubleTapper.AvailableInCharacterCreation = true;
            DoubleTapper.CanRemove = false;
            DoubleTapper.CanSwap = true;
            DoubleTapper.Conflicting.AddRange(new string[] { vTrait.Loud });
            DoubleTapper.CostInCharacterCreation = 7;
            DoubleTapper.Recommendations.Add(vSpecialAbility.Camouflage);
            DoubleTapper.IsActive = true;
            DoubleTapper.Upgrade = null;

            CustomTrait Sniper = RogueLibs.CreateCustomTrait(cTrait.Sniper, true,
                new CustomNameInfo("Sniper"),
                new CustomNameInfo("One shot, a pretty good chance of one kill. They can run, but they'll only die tired, unless they get away.\n\n- Past a certain distance, bullet hits on unaware targets deal massive damage\n- Bullet range increased"));
            Sniper.Available = true;
            Sniper.AvailableInCharacterCreation = true;
            Sniper.CanRemove = false;
            Sniper.CanSwap = true;
            Sniper.Conflicting.AddRange(new string[] { cTrait.Myopic, cTrait.Myopic2 });
            Sniper.CostInCharacterCreation = 8;
            Sniper.Recommendations.Add(cTrait.EagleEyes);
            Sniper.IsActive = true;
            Sniper.Upgrade = null;
            #endregion
            #region Drug Limitations
            CustomTrait DAREdevil = RogueLibs.CreateCustomTrait(cTrait.DAREdevil, true,
                new CustomNameInfo("DAREdevil"),
                new CustomNameInfo("You have injected zero marijuanas. Crack is Whack. Smokers are Jokers. Needles are for... beetles."));
            DAREdevil.Available = true;
            DAREdevil.AvailableInCharacterCreation = true;
            DAREdevil.CanRemove = true;
            DAREdevil.CanSwap = false;
            DAREdevil.Conflicting.Add(vTrait.Addict);
            DAREdevil.Conflicting.AddRange(cTrait.LimitDrugs);
            DAREdevil.Conflicting.Remove(cTrait.DAREdevil);
            DAREdevil.CostInCharacterCreation = -3;
            DAREdevil.IsActive = true;
            DAREdevil.Upgrade = null;

            CustomTrait FriendOfBill = RogueLibs.CreateCustomTrait(cTrait.FriendOfBill, true,
                new CustomNameInfo("Friend Of Bill"),
                new CustomNameInfo("You are taking things one day at a time. But every day sucks when you can't get drunk anymore."));
            FriendOfBill.Available = true;
            FriendOfBill.AvailableInCharacterCreation = true;
            FriendOfBill.CanRemove = true;
            FriendOfBill.CanSwap = false;
            FriendOfBill.Conflicting.Add(vTrait.Addict);
            FriendOfBill.Conflicting.AddRange(cTrait.LimitDrugs);
            FriendOfBill.Conflicting.Remove(cTrait.FriendOfBill);
            FriendOfBill.CostInCharacterCreation = -1;
            FriendOfBill.IsActive = true;
            FriendOfBill.Upgrade = null;

            CustomTrait Teetotaller = RogueLibs.CreateCustomTrait(cTrait.Teetotaller, true,
                new CustomNameInfo("Teetotaller"),
                new CustomNameInfo("Wow, you're really boring. You don't do drugs *or* alcohol. What do you even do?"));
            Teetotaller.Available = true;
            Teetotaller.AvailableInCharacterCreation = true;
            Teetotaller.CanRemove = true;
            Teetotaller.CanSwap = false;
            Teetotaller.Conflicting.Add(vTrait.Addict);
            Teetotaller.Conflicting.AddRange(cTrait.LimitDrugs);
            Teetotaller.Conflicting.Remove(cTrait.Teetotaller);
            Teetotaller.CostInCharacterCreation = -4;
            Teetotaller.IsActive = true;
            Teetotaller.Upgrade = null;
            #endregion
            #region Food Limitations
            CustomTrait Carnivore = RogueLibs.CreateCustomTrait(cTrait.Carnivore, true,
                new CustomNameInfo("Carnivore"),
                new CustomNameInfo("'Meeeeeeat,' you grunt enthusiastically."));
            Carnivore.Available = true;
            Carnivore.AvailableInCharacterCreation = true;
            Carnivore.CanRemove = true;
            Carnivore.CanSwap = false;
            Carnivore.Conflicting.Add(vTrait.BananaLover);
            Carnivore.Conflicting.AddRange(cTrait.LimitFood);
            Carnivore.Conflicting.AddRange(vTrait.LimitFood);
            Carnivore.CostInCharacterCreation = -1;
            Carnivore.IsActive = true;
            Carnivore.Upgrade = null;

            CustomTrait Vegetarian = RogueLibs.CreateCustomTrait(cTrait.Vegetarian, true,
                new CustomNameInfo("Vegetarian"),
                new CustomNameInfo("You are one of those people."));
            Vegetarian.Available = true;
            Vegetarian.AvailableInCharacterCreation = true;
            Vegetarian.CanRemove = true;
            Vegetarian.CanSwap = true;
            Vegetarian.Conflicting.Add(vTrait.Zombiism);
            Vegetarian.Conflicting.AddRange(cTrait.LimitFood);
            Vegetarian.Conflicting.AddRange(vTrait.LimitFood);
            Vegetarian.CostInCharacterCreation = -1;
            Vegetarian.IsActive = true;
            Vegetarian.Upgrade = null;
            #endregion
            #region Equipment
            CustomTrait AfraidOfLoudNoises = RogueLibs.CreateCustomTrait(cTrait.AfraidOfLoudNoises, true,
                new CustomNameInfo("Afraid of Loud Noises"),
                new CustomNameInfo("The recoil bruised my shouldah. The brass shell casings disoriented me as they flew past my face. The smell of sulfur and destruction made me sick. The explosions - loud like a bowomb - gave me a temporary case of PTSD. For at least an hour after firing the gun just a few times, I was anxious and irritable. And it's such small portions!"));
            AfraidOfLoudNoises.Available = true;
            AfraidOfLoudNoises.AvailableInCharacterCreation = true;
            AfraidOfLoudNoises.CanRemove = true;
            AfraidOfLoudNoises.CanSwap = true;
            AfraidOfLoudNoises.Conflicting.AddRange(new string[] { cTrait.DrawNoBlood });
            AfraidOfLoudNoises.Conflicting.AddRange(vTrait.LimitWeapons);
            AfraidOfLoudNoises.CostInCharacterCreation = -4;
            AfraidOfLoudNoises.IsActive = true;
            AfraidOfLoudNoises.Upgrade = null;

            CustomTrait DrawNoBlood = RogueLibs.CreateCustomTrait(cTrait.DrawNoBlood, true,
                new CustomNameInfo("Draw No Blood"),
                new CustomNameInfo("You have taken an oath to draw no blood. Guess you'll have to smash skulls really carefully, then. You cannot use bullet-based guns, sharp weapons, or most explosives."));
            DrawNoBlood.Available = true;
            DrawNoBlood.AvailableInCharacterCreation = true;
            DrawNoBlood.CanRemove = true;
            DrawNoBlood.CanSwap = false;
            DrawNoBlood.Conflicting.AddRange(new string[] { cTrait.AfraidOfLoudNoises, vTrait.Jugularious, vTrait.FleshFeast });
            DrawNoBlood.Conflicting.AddRange(vTrait.LimitWeapons);
            DrawNoBlood.CostInCharacterCreation = -5;
            DrawNoBlood.IsActive = true;
            DrawNoBlood.Upgrade = null;

            CustomTrait FatHead = RogueLibs.CreateCustomTrait(cTrait.FatHead, true,
                new CustomNameInfo("Fat Head"),
                new CustomNameInfo("You have a big, fat, ugly head. You can't wear hats of any kind. No one will lend you their headphones or sunglasses, because your big, fat, dumb, ugly head will break them. Your self-esteem is pretty much in the toilet."));
            FatHead.Available = true;
            FatHead.AvailableInCharacterCreation = true;
            FatHead.CanRemove = true;
            FatHead.CanSwap = false;
            FatHead.Conflicting.AddRange(new string[] { vTrait.Diminutive });
            FatHead.CostInCharacterCreation = -1;
            FatHead.IsActive = true;
            FatHead.Upgrade = null;
            #endregion
            #region Experience Gain Rate
            CustomTrait DimBulb = RogueLibs.CreateCustomTrait(cTrait.DimBulb, true,
                new CustomNameInfo("Dim Bulb"),
                new CustomNameInfo("You have more money than sense. And buddy, you're broke.\n\n- XP gain set to 75%"));
            DimBulb.Available = true;
            DimBulb.AvailableInCharacterCreation = true;
            DimBulb.CanRemove = true;
            DimBulb.CanSwap = false;
			DimBulb.Conflicting.AddRange(cTrait.ExperienceRate);
            DimBulb.Conflicting.AddRange(vTrait.ExperienceRate);
            DimBulb.Conflicting.Remove(cTrait.DimBulb);
            DimBulb.CostInCharacterCreation = -10;
            DimBulb.IsActive = true;
            DimBulb.Upgrade = null;

            CustomTrait MoronTheMerrier = RogueLibs.CreateCustomTrait(cTrait.MoronTheMerrier, true,
                new CustomNameInfo("Moron the Merrier"),
                new CustomNameInfo("You are not playing with a full set of brain cells. You are not the sharpest tool in the dumb-person storage shed. The lights are on, but someone is dumb, and it's you. Are you understanding any of this?\n\n- XP gain set to 50%"));
            MoronTheMerrier.Available = true;
            MoronTheMerrier.AvailableInCharacterCreation = true;
            MoronTheMerrier.CanRemove = true;
            MoronTheMerrier.CanSwap = false;
            MoronTheMerrier.Conflicting.AddRange(cTrait.ExperienceRate);
            MoronTheMerrier.Conflicting.AddRange(vTrait.ExperienceRate);
            MoronTheMerrier.Conflicting.Remove(cTrait.MoronTheMerrier);
            MoronTheMerrier.CostInCharacterCreation = -20;
            MoronTheMerrier.IsActive = true;
            MoronTheMerrier.Upgrade = null;

            CustomTrait SmoothBrained = RogueLibs.CreateCustomTrait(cTrait.SmoothBrained, true,
                new CustomNameInfo("Smooth-Brained"),
                new CustomNameInfo("You emerged from your mother's womb fully formed, killing her in the process. The terrified doctor declared you a perfect child and in no need of improvement in any way, ever.\n\n- XP gain set to 0%"));
            SmoothBrained.Available = true;
            SmoothBrained.AvailableInCharacterCreation = true;
            SmoothBrained.CanRemove = true;
            SmoothBrained.CanSwap = false;
            SmoothBrained.Conflicting.AddRange(cTrait.ExperienceRate);
            SmoothBrained.Conflicting.AddRange(vTrait.ExperienceRate);
            SmoothBrained.Conflicting.Remove(cTrait.SmoothBrained);
            SmoothBrained.CostInCharacterCreation = -40;
            SmoothBrained.IsActive = true;
            SmoothBrained.Upgrade = null;

            CustomTrait VeryHardOnYourself = RogueLibs.CreateCustomTrait(cTrait.VeryHardOnYourself, true,
                new CustomNameInfo("Very Hard-On Yourself"),
                new CustomNameInfo("You were a rigid pianist for years: Any time you commited a boner or a cock-up, you would ejaculate in frustration. Your life was seemin' saturated, so you thought you'd come to the Resistance. They appreciate your perfectionism - the hardest job for you to swallow is a wrecked one.\n\nAny XP loss you incur is doubled.\n\nPenis"));
            VeryHardOnYourself.Available = true;
            VeryHardOnYourself.AvailableInCharacterCreation = true;
            VeryHardOnYourself.CanRemove = true;
            VeryHardOnYourself.CanSwap = false;
            VeryHardOnYourself.Conflicting.AddRange(new string[] { cTrait.SmoothBrained });
            VeryHardOnYourself.CostInCharacterCreation = -1;
            VeryHardOnYourself.Prerequisites.AddRange(new string[] { vTrait.TheLaw, vTrait.Extortionist });
            VeryHardOnYourself.IsActive = true;
            VeryHardOnYourself.Upgrade = null;
            #endregion
            #region Luck
            CustomTrait Charmed = RogueLibs.CreateCustomTrait(cTrait.Charmed, true,
                new CustomNameInfo("Charmed & Dangerous"),
                new CustomNameInfo("You once found a fourteen-leaf clover."));
            Charmed.Available = true;
            Charmed.AvailableInCharacterCreation = true;
            Charmed.CanRemove = false;
            Charmed.CanSwap = true;
            Charmed.Conflicting.AddRange(cTrait.Luck);
            Charmed.Conflicting.Remove(cTrait.Charmed);
            Charmed.CostInCharacterCreation = 3;
            Charmed.IsActive = true;
            Charmed.Upgrade = cTrait.Charmed_2;

            CustomTrait Charmed_2 = RogueLibs.CreateCustomTrait(cTrait.Charmed_2, false,
                new CustomNameInfo("Charmed & Dangerous +"),
                new CustomNameInfo("You are *really* lucky. Anyone who's been at the urinal next to you can attest."));
            Charmed_2.Available = true;
            Charmed_2.AvailableInCharacterCreation = false;
            Charmed_2.CanRemove = false;
            Charmed_2.CanSwap = true;
            Charmed_2.Conflicting.AddRange(cTrait.Luck);
            Charmed_2.Conflicting.Remove(cTrait.Charmed_2);
            Charmed_2.CostInCharacterCreation = 6;
            Charmed_2.Upgrade = null;

            CustomTrait Cursed = RogueLibs.CreateCustomTrait(cTrait.Cursed, true,
                new CustomNameInfo("Unlucky"),
                new CustomNameInfo("You pissed in some old Gypsy lady's cereal, and you still refuse to apologize. She didn't like that."));
            Cursed.Available = true;
            Cursed.AvailableInCharacterCreation = true;
            Cursed.CanRemove = true;
            Cursed.CanSwap = false;
            Cursed.Conflicting.AddRange(cTrait.Luck);
            Cursed.Conflicting.Remove(cTrait.Cursed);
            Cursed.CostInCharacterCreation = -2;
            Cursed.IsActive = true;
            Cursed.Upgrade = null;

            CustomTrait Cursed_2 = RogueLibs.CreateCustomTrait(cTrait.Cursed_2, true,
                new CustomNameInfo("Unlucky +"),
                new CustomNameInfo("You bought up an old Indian graveyard, and there you built a black cat sanctuary and mirror-breakery. Not your best choice."));
            Cursed_2.Available = true;
            Cursed_2.AvailableInCharacterCreation = true;
            Cursed_2.CanRemove = true;
            Cursed_2.CanSwap = false;
            Cursed_2.Conflicting.AddRange(cTrait.Luck);
            Cursed_2.Conflicting.Remove(cTrait.Cursed_2);
            Cursed_2.CostInCharacterCreation = -4;
            Cursed_2.IsActive = true;
            Cursed_2.Upgrade = null;
            #endregion
            #region Magic - General
            CustomTrait Archmage = RogueLibs.CreateCustomTrait(cTrait.Archmage, true,
                new CustomNameInfo("Archmage"),
                new CustomNameInfo("You are an unrivalled master of the magical arts. Basically cheat mode for magical abilities, added by request."));
            Archmage.Available = true;
            Archmage.AvailableInCharacterCreation = true;
            Archmage.CanRemove = false;
            Archmage.CanSwap = false;
            Archmage.Conflicting.AddRange(cTrait.Magic);
            Archmage.Conflicting.Remove(cTrait.Archmage);
            Archmage.CostInCharacterCreation = 100;
            Archmage.IsActive = true;
            Archmage.Recommendations.AddRange(cSpecialAbility.Magic);
            Archmage.SpecialAbilities.AddRange(cSpecialAbility.Magic);
            Archmage.Upgrade = null;

            CustomTrait FocusedCasting = RogueLibs.CreateCustomTrait(cTrait.FocusedCasting, true,
                new CustomNameInfo("Focused Casting"),
                new CustomNameInfo("You've carefully refined your magic techniques to improve accuracy and reduce the chances of miscasting spells."));
            FocusedCasting.Available = true;
            FocusedCasting.AvailableInCharacterCreation = true;
            FocusedCasting.CanRemove = false;
            FocusedCasting.CanSwap = false;
            FocusedCasting.Conflicting.AddRange(cTrait.MagicStyles);
            FocusedCasting.Conflicting.Remove(cTrait.FocusedCasting);
            FocusedCasting.CostInCharacterCreation = 3;
            FocusedCasting.IsActive = true;
            FocusedCasting.Recommendations.AddRange(cSpecialAbility.Magic);
            FocusedCasting.SpecialAbilities.AddRange(cSpecialAbility.Magic);
            FocusedCasting.Upgrade = cTrait.FocusedCasting_2;

            CustomTrait FocusedCasting_2 = RogueLibs.CreateCustomTrait(cTrait.FocusedCasting_2, true,
                new CustomNameInfo("Focused Casting +"),
                new CustomNameInfo("You've even more carefully refined your techniques even more to improve accuracy and reduce the chances of miscasting spells EVEN MORE."));
            FocusedCasting_2.Available = false;
            FocusedCasting_2.AvailableInCharacterCreation = false;
            FocusedCasting_2.CanRemove = false;
            FocusedCasting_2.CanSwap = false;
            FocusedCasting_2.Conflicting.AddRange(cTrait.MagicStyles);
            FocusedCasting_2.Conflicting.Remove(cTrait.FocusedCasting_2);
            FocusedCasting_2.CostInCharacterCreation = 6;
            FocusedCasting_2.IsActive = true;
            FocusedCasting_2.Upgrade = null;

            CustomTrait MagicTraining = RogueLibs.CreateCustomTrait(cTrait.MagicTraining, true,
                new CustomNameInfo("Magic Training"),
                new CustomNameInfo("Improves your skills with any Magic Special Ability."));
            MagicTraining.Available = true;
            MagicTraining.AvailableInCharacterCreation = true;
            MagicTraining.Conflicting.Add(cTrait.Archmage);
            MagicTraining.CostInCharacterCreation = 5;
            MagicTraining.CanRemove = false;
            MagicTraining.CanSwap = false;
            MagicTraining.IsActive = true;
            MagicTraining.Recommendations.AddRange(cSpecialAbility.Magic);
            MagicTraining.SpecialAbilities.AddRange(cSpecialAbility.Magic);
            MagicTraining.Upgrade = cTrait.MagicTraining_2;

            CustomTrait MagicTraining_2 = RogueLibs.CreateCustomTrait(cTrait.MagicTraining_2, true,
                new CustomNameInfo("Magic Training +"),
                new CustomNameInfo("Further improves your skills with any Magic Special Ability."));
            MagicTraining_2.Available = false;
            MagicTraining_2.AvailableInCharacterCreation = false;
            MagicTraining_2.Conflicting.Add(cTrait.Archmage);
            MagicTraining_2.CostInCharacterCreation = 10;
            MagicTraining_2.CanRemove = false;
            MagicTraining_2.CanSwap = false;
            MagicTraining_2.IsActive = true;
            MagicTraining_2.SpecialAbilities.AddRange(cSpecialAbility.Magic);
            MagicTraining_2.Upgrade = null;

            CustomTrait ManaBattery = RogueLibs.CreateCustomTrait(cTrait.ManaBattery, true,
                new CustomNameInfo("Mana Battery"),
                new CustomNameInfo("You can store more mana. Nifty."));
            ManaBattery.Available = true;
            ManaBattery.AvailableInCharacterCreation = true;
            ManaBattery.Conflicting.Add(cTrait.Archmage);
            ManaBattery.CostInCharacterCreation = 2;
            ManaBattery.CanRemove = false;
            ManaBattery.CanSwap = false;
            ManaBattery.Conflicting.AddRange(new string[] { });
            ManaBattery.IsActive = true;
            ManaBattery.SpecialAbilities.AddRange(cSpecialAbility.Magic);
            ManaBattery.Upgrade = cTrait.ManaBattery_2;

            CustomTrait ManaBattery_2 = RogueLibs.CreateCustomTrait(cTrait.ManaBattery_2, true,
                new CustomNameInfo("Mana Battery +"),
                new CustomNameInfo("You can store even more mana. Even niftier."));
            ManaBattery_2.Available = true;
            ManaBattery_2.AvailableInCharacterCreation = false;
            ManaBattery_2.Conflicting.Add(cTrait.Archmage);
            ManaBattery_2.CostInCharacterCreation = 4;
            ManaBattery_2.CanRemove = false;
            ManaBattery_2.CanSwap = false;
            ManaBattery_2.Conflicting.AddRange(new string[] { });
            ManaBattery_2.IsActive = true;
            ManaBattery_2.SpecialAbilities.AddRange(cSpecialAbility.Magic);
            ManaBattery_2.Upgrade = null;

            CustomTrait WildCasting = RogueLibs.CreateCustomTrait(cTrait.WildCasting, true,
                new CustomNameInfo("Wild Casting"),
                new CustomNameInfo("You don't need all that safety shit. You wanna cast some damn spells! Your spells are more powerful, but you have a greater chance of miscasting them."));
            WildCasting.Available = true;
            WildCasting.AvailableInCharacterCreation = true;
            WildCasting.CanRemove = false;
            WildCasting.CanSwap = false;
            WildCasting.Conflicting.AddRange(cTrait.MagicStyles);
            WildCasting.Conflicting.Remove(cTrait.WildCasting);
            WildCasting.CostInCharacterCreation = 3;
            WildCasting.IsActive = true;
            WildCasting.Recommendations.AddRange(cSpecialAbility.Magic);
            WildCasting.SpecialAbilities.AddRange(cSpecialAbility.Magic);
            WildCasting.Upgrade = cTrait.WildCasting_2;

            CustomTrait WildCasting_2 = RogueLibs.CreateCustomTrait(cTrait.WildCasting_2, true,
                new CustomNameInfo("Wild Casting +"),
                new CustomNameInfo("You're addicted to power. Your spells are ridiculously powerful... and risky."));
            WildCasting_2.Available = false;
            WildCasting_2.AvailableInCharacterCreation = false;
            WildCasting_2.CanRemove = false;
            WildCasting_2.CanSwap = false;
            WildCasting_2.Conflicting.AddRange(cTrait.MagicStyles);
            WildCasting_2.Conflicting.Remove(cTrait.WildCasting_2);
            WildCasting_2.CostInCharacterCreation = 6;
            WildCasting_2.IsActive = true;
            WildCasting_2.Upgrade = null;
            #endregion
            #region Magic - Chronomantic Dilation
            CustomTrait RATS = RogueLibs.CreateCustomTrait(cTrait.RATS, true,
                new CustomNameInfo("R.A.T.S. Mk VI"),
                new CustomNameInfo("Resistance-Tec Assisted Targeting System. The latest cybernetic enhancement to shooting accuracy, crit chance, & some combat traits with a double effect when time is slowed down."));
            RATS.Available = true;
            RATS.AvailableInCharacterCreation = true;
            RATS.CanRemove = false;
            RATS.CanSwap = true;
            RATS.Conflicting.AddRange(new string[] { cTrait.RATS_2 });
            RATS.CostInCharacterCreation = 3;
            RATS.IsActive = true;
            RATS.Recommendations.AddRange(new string[] { vTrait.Butterfingerer, cSpecialAbility.ChronomanticDilation, vTrait.IncreasedCritChance, vTrait.Kneecapper, vTrait.UnCrits });
            RATS.Upgrade = cTrait.RATS_2;

            CustomTrait RATS_2 = RogueLibs.CreateCustomTrait(cTrait.RATS_2, true,
                new CustomNameInfo("R.A.T.S. Mk VII"),
                new CustomNameInfo("\"Various bug fixes and performance improvements.\" That's all the fucking update notes say. Also, they took out the headphone jack??"));
            RATS_2.Available = true;
            RATS_2.AvailableInCharacterCreation = true;
            RATS_2.CanRemove = false;
            RATS_2.CanSwap = false;
            RATS_2.Conflicting.AddRange(new string[] { cTrait.RATS });
            RATS_2.CostInCharacterCreation = 12;
            RATS_2.IsActive = true;
            RATS_2.Recommendations.AddRange(new string[] { vTrait.Butterfingerer, cSpecialAbility.ChronomanticDilation, vTrait.IncreasedCritChance, vTrait.Kneecapper, vTrait.UnCrits });
            RATS_2.Upgrade = null;
			#endregion
			#region Miscellaneous
			CustomTrait EagleEyes = RogueLibs.CreateCustomTrait(cTrait.EagleEyes, true,
				new CustomNameInfo("Eagle Eyes"),
				new CustomNameInfo("You can see further than normal. Hell, you can see further than *abnormal*."));
			EagleEyes.Available = true;
			EagleEyes.AvailableInCharacterCreation = true;
			EagleEyes.CanRemove = false;
			EagleEyes.CanSwap = true;
            EagleEyes.Conflicting.AddRange(cTrait.VisionRange);
            EagleEyes.Conflicting.Remove(cTrait.EagleEyes);
			EagleEyes.CostInCharacterCreation = 3;
			EagleEyes.IsActive = true;
			EagleEyes.Upgrade = cTrait.EagleEyes_2;

			CustomTrait EagleEyes_2 = RogueLibs.CreateCustomTrait(cTrait.EagleEyes_2, true,
				new CustomNameInfo("Eagle Eyes +"),
				new CustomNameInfo("You might have been a good sniper, but you were kicked out of training when they didn't believe that you worked better without a rifle scope."));
			EagleEyes_2.Available = true;
			EagleEyes_2.AvailableInCharacterCreation = true;
			EagleEyes_2.CanRemove = false;
			EagleEyes_2.CanSwap = false;
            EagleEyes_2.Conflicting.AddRange(cTrait.VisionRange);
            EagleEyes_2.Conflicting.Remove(cTrait.EagleEyes_2);
            EagleEyes_2.CostInCharacterCreation = 6;
			EagleEyes_2.IsActive = true;
			EagleEyes_2.Upgrade = null;

            CustomTrait Myopic = RogueLibs.CreateCustomTrait(cTrait.Myopic, true,
				new CustomNameInfo("Myopic"),
				new CustomNameInfo("You can't see too far. In fact, you can't see far enough."));
			Myopic.Available = true;
			Myopic.AvailableInCharacterCreation = true;
			Myopic.CanRemove = true;
			Myopic.CanSwap = true;
            Myopic.Conflicting.AddRange(cTrait.VisionRange);
            Myopic.Conflicting.Remove(cTrait.Myopic);
            Myopic.CostInCharacterCreation = -4;
			Myopic.IsActive = true;
			Myopic.Upgrade = null;

            CustomTrait Myopic_2 = RogueLibs.CreateCustomTrait(cTrait.Myopic2, true,
				new CustomNameInfo("Ultramyopic"),
				new CustomNameInfo("You tend to keep people at arm's length, where you can't see them."));
			Myopic_2.Available = true;
			Myopic_2.AvailableInCharacterCreation = true;
			Myopic_2.CanRemove = true;
			Myopic_2.CanSwap = true;
            Myopic_2.Conflicting.AddRange(cTrait.VisionRange);
            Myopic_2.Conflicting.Remove(cTrait.Myopic2);
            Myopic_2.CostInCharacterCreation = -8;
			Myopic_2.IsActive = true;
			Myopic_2.Upgrade = null;
            #endregion
            #region Social
            CustomTrait Domineering = RogueLibs.CreateCustomTrait(cTrait.Domineering, true,
                new CustomNameInfo("Domineering"),
                new CustomNameInfo("There's just something about how you carry yourself. Maybe it's the way you walk, or maybe it's the way you demand obedience from the weak around you. People will occasionally be Submissive to you. Kinky!"));
            Domineering.Available = true;
            Domineering.AvailableInCharacterCreation = true;
            Domineering.CanRemove = true;
            Domineering.CanSwap = false;
            Domineering.Conflicting.Add(cTrait.Domineering_2);
            Domineering.CostInCharacterCreation = 2;
            Domineering.IsActive = true;
            Domineering.Upgrade = cTrait.Domineering_2;

            CustomTrait Domineering_2 = RogueLibs.CreateCustomTrait(cTrait.Domineering_2, true,
                new CustomNameInfo("Domineering +"),
                new CustomNameInfo("Some people make sure their social skills work for them. You crack the whip! You're finding more and more Subs everywhere you look."));
            Domineering_2.Available = true;
            Domineering_2.AvailableInCharacterCreation = false;
            Domineering_2.CanRemove = true;
            Domineering_2.CanSwap = false;
            Domineering_2.Conflicting.Add(cTrait.Domineering);
            Domineering_2.CostInCharacterCreation = 4;
            Domineering_2.IsActive = true;
            Domineering_2.Upgrade = null;

            CustomTrait GenerallyUnpleasant = RogueLibs.CreateCustomTrait(cTrait.GenerallyUnpleasant, true,
                new CustomNameInfo("Generally Unpleasant"),
                new CustomNameInfo("You have a certain way with people! It's... very annoying."));
            GenerallyUnpleasant.Available = true;
            GenerallyUnpleasant.AvailableInCharacterCreation = true;
            GenerallyUnpleasant.CanRemove = true;
            GenerallyUnpleasant.CanSwap = false;
            GenerallyUnpleasant.Conflicting.AddRange(new string[] { vTrait.Antisocial, vTrait.Charismatic, vTrait.FairGame, vTrait.FriendoftheCommonFolk, cTrait.GenerallyUnpleasant_2, vTrait.Malodorous, cTrait.Polarizing, cTrait.Polarizing_2, vTrait.Suspicious, vTrait.Wanted });
            GenerallyUnpleasant.CostInCharacterCreation = -3;
            GenerallyUnpleasant.IsActive = true;
            GenerallyUnpleasant.Upgrade = null;

            CustomTrait ObjectivelyUnpleasant = RogueLibs.CreateCustomTrait(cTrait.GenerallyUnpleasant_2, true,
                new CustomNameInfo("Objectively Unpleasant"),
                new CustomNameInfo("You chew with your mouth open. You rightfully have no friends in the world. You are scum. Everyone starts out Annoyed, including me."));
            ObjectivelyUnpleasant.Available = true;
            ObjectivelyUnpleasant.AvailableInCharacterCreation = true;
            ObjectivelyUnpleasant.CanRemove = true;
            ObjectivelyUnpleasant.CanSwap = false;
            ObjectivelyUnpleasant.Conflicting.AddRange(new string[] { vTrait.Antisocial, vTrait.Charismatic, vTrait.FairGame, vTrait.FriendoftheCommonFolk, cTrait.GenerallyUnpleasant, vTrait.Malodorous, cTrait.Polarizing, cTrait.Polarizing_2, vTrait.Suspicious, vTrait.Wanted });
            ObjectivelyUnpleasant.CostInCharacterCreation = -8;
            ObjectivelyUnpleasant.IsActive = true;
            ObjectivelyUnpleasant.Upgrade = null;

            CustomTrait Polarizing = RogueLibs.CreateCustomTrait(cTrait.Polarizing, true,
                new CustomNameInfo("Polarizing"),
                new CustomNameInfo("Everyone has an opinion on you, when they first meet you. Might be good or bad, but at least you feel noticed!"));
            Polarizing.Available = true;
            Polarizing.AvailableInCharacterCreation = false;
            Polarizing.CanRemove = false;
            Polarizing.CanSwap = true;
            Polarizing.Conflicting.AddRange(new string[] { vTrait.Antisocial, vTrait.Charismatic, vTrait.FairGame, vTrait.FriendoftheCommonFolk, cTrait.GenerallyUnpleasant, cTrait.GenerallyUnpleasant_2, vTrait.Malodorous, cTrait.Polarizing_2, vTrait.Suspicious, vTrait.Wanted });
            Polarizing.CostInCharacterCreation = 0;
            Polarizing.IsActive = true;
            Polarizing.Upgrade = cTrait.Polarizing_2;

            CustomTrait Polarizing_2 = RogueLibs.CreateCustomTrait(cTrait.Polarizing_2, true,
                new CustomNameInfo("Polarizing +"),
                new CustomNameInfo("People have *strong* opinions of you. Like me. I think you're just great."));
            Polarizing_2.Available = true;
            Polarizing_2.AvailableInCharacterCreation = true;
            Polarizing_2.CanRemove = false;
            Polarizing_2.CanSwap = true;
            Polarizing_2.Conflicting.AddRange(new string[] { vTrait.Antisocial, vTrait.Charismatic, vTrait.FairGame, vTrait.FriendoftheCommonFolk, cTrait.GenerallyUnpleasant, cTrait.GenerallyUnpleasant_2, vTrait.Malodorous, cTrait.Polarizing, vTrait.Suspicious, vTrait.Wanted });
            Polarizing_2.CostInCharacterCreation = 1;
            Polarizing_2.IsActive = true;
            Polarizing_2.Upgrade = null;

            CustomTrait Priors = RogueLibs.CreateCustomTrait(cTrait.Priors, true,
                new CustomNameInfo("Priors"),
                new CustomNameInfo("You have a long rap sheet, and the police know you by first name. On the upside, you've earned the respect of the City's criminals."));
            Priors.Available = true;
            Priors.AvailableInCharacterCreation = true;
            Priors.CanRemove = true;
            Priors.CanSwap = true;
            Priors.Conflicting.AddRange(new string[] { vTrait.Charismatic, vTrait.CopsDontCare, vTrait.CorruptionCosts, vTrait.FairGame, vTrait.Suspicious, vTrait.TheLaw, vTrait.Wanted });
            Priors.CostInCharacterCreation = -1;
            Priors.IsActive = true;
            Priors.Upgrade = null;

            CustomTrait VeiledThreats = RogueLibs.CreateCustomTrait(cTrait.VeiledThreats, true,
				new CustomNameInfo("000: Veiled Threats"),
				new CustomNameInfo("When you attempt to Bribe, Extort, Mug, or Threaten, a failure will turn the target Annoyed instead of Hostile."));
            VeiledThreats.Available = true;
            VeiledThreats.AvailableInCharacterCreation = true;
            VeiledThreats.CanRemove = false;
            VeiledThreats.CanSwap = true;
            VeiledThreats.Conflicting.AddRange(new string[] { });
            VeiledThreats.CostInCharacterCreation = 2;
            VeiledThreats.IsActive = true;
            VeiledThreats.Recommendations.AddRange(new string[] { vTrait.Extortionist, vTrait.Mugger });
            VeiledThreats.Upgrade = null;

            CustomTrait Warlord = RogueLibs.CreateCustomTrait(cTrait.Warlord, true,
                new CustomNameInfo("Warlord"),
                new CustomNameInfo("What is best in this game run, Onan? \n\nTo gib your enemies, to see them fall at your little rectangle legs -- to take their E_Horse and item drops and hear the lamentations of their... people with pixelated, possibly female presentation. That is best!\n\nYou can force anyone into submission."));
            Warlord.Available = true;
            Warlord.AvailableInCharacterCreation = true;
            Warlord.CanRemove = false;
            Warlord.CanSwap = true;
            Warlord.Conflicting.AddRange(new string[] { });
            Warlord.CostInCharacterCreation = 20;
            Warlord.IsActive = true;
            Warlord.Upgrade = null;
            #endregion
            #region Spawns
            CustomTrait Haunted = RogueLibs.CreateCustomTrait(cTrait.Haunted, true,
                new CustomNameInfo("000: Haunted"),
                new CustomNameInfo("You spent too long spelunking in an ancient treasure vault. The local ghosts were very unhappy with you, and you had their legal case dismissed. Now they're taking it into their own hands."));
            Haunted.Available = true;
            Haunted.AvailableInCharacterCreation = true;
            Haunted.CanRemove = true;
            Haunted.CanSwap = false;
            Haunted.CostInCharacterCreation = -2;
            Haunted.IsActive = true;
            Haunted.Upgrade = null;

            CustomTrait MobDebt = RogueLibs.CreateCustomTrait(cTrait.MobDebt, true,
                new CustomNameInfo("000: Mob Debt"),
                new CustomNameInfo("You found a sack of money, and the people to whom it belongs want it back with interest. Well, they certainly have your interest, but you've spent the principal. Pay it off at an ATM by Level 10, or else your kneecaps (and the rest of your body) will pay the price."));
            MobDebt.Available = true;
            MobDebt.AvailableInCharacterCreation = true;
            MobDebt.CanRemove = true;
            MobDebt.CanSwap = false;
            MobDebt.CostInCharacterCreation = -10;
            MobDebt.IsActive = true;
            MobDebt.Upgrade = null;

            CustomTrait MookMasher = RogueLibs.CreateCustomTrait(cTrait.MookMasher, true,
                new CustomNameInfo("000: Mook Masher"),
                new CustomNameInfo("The Mayor knows you're a threat, and you're coming for him. He could just destroy you, but as a villain, he prefers to send his henchmen at you in steadily increasing but manageable waves."));
            MookMasher.Available = true;
            MookMasher.AvailableInCharacterCreation = true;
            MookMasher.CanRemove = true;
            MookMasher.CanSwap = false;
            MookMasher.CostInCharacterCreation = -5;
            MookMasher.IsActive = true;
            MookMasher.Upgrade = null;

            CustomTrait Reinforcements = RogueLibs.CreateCustomTrait(cTrait.Reinforcements, true,
                new CustomNameInfo("000: Reinforcements"),
                new CustomNameInfo("You have worked to create an army for the Resistance. That army now patrols the City secretly, looking for the opportunity to aid the cause."));
            Reinforcements.Available = true;
            Reinforcements.AvailableInCharacterCreation = true;
            Reinforcements.CanRemove = false;
            Reinforcements.CanSwap = true;
            Reinforcements.Conflicting.AddRange(new string[] { cTrait.Reinforcements_2 });
            Reinforcements.CostInCharacterCreation = 5;
            Reinforcements.IsActive = true;
            Reinforcements.Upgrade = cTrait.Reinforcements_2;

            CustomTrait Reinforcements_2 = RogueLibs.CreateCustomTrait(cTrait.Reinforcements_2, true,
                new CustomNameInfo("000: Reinforcements +"),
                new CustomNameInfo("Your allies now have top-of-the-line equipment."));
            Reinforcements_2.Available = true;
            Reinforcements_2.AvailableInCharacterCreation = true;
            Reinforcements_2.CanRemove = false;
            Reinforcements_2.CanSwap = true;
            Reinforcements_2.Conflicting.AddRange(new string[] { cTrait.Reinforcements });
            Reinforcements_2.CostInCharacterCreation = 10;
            Reinforcements_2.IsActive = true;
            Reinforcements_2.Upgrade = null;
            #endregion
            #region Stealth
            CustomTrait StealthBastardDeluxe = RogueLibs.CreateCustomTrait(cTrait.StealthBastardDeluxe, true,
                new CustomNameInfo("Stealth Bastard Deluxe"),
                new CustomNameInfo("- Move through broken windows without taking a scratch.\n- Open and close doors silently.\n- Hide in Bathtubs, Plants, Pool Tables, and Big Tables.\n\n[Bug: If you get stuck between it and the wall, you might clip through the wall]"));
            StealthBastardDeluxe.Available = true;
            StealthBastardDeluxe.AvailableInCharacterCreation = true;
            StealthBastardDeluxe.CanRemove = false;
            StealthBastardDeluxe.CanSwap = true;
            StealthBastardDeluxe.Conflicting.AddRange(new string[] { vTrait.Loud, vTrait.Bulky });
            StealthBastardDeluxe.CostInCharacterCreation = 4;
            StealthBastardDeluxe.IsActive = true;
            StealthBastardDeluxe.Upgrade = null;

            CustomTrait UnderdarkCitizen = RogueLibs.CreateCustomTrait(cTrait.UnderdarkCitizen, true,
                new CustomNameInfo("000: Underdark Citizen"),
                new CustomNameInfo("You can navigate the city's sewers with ease. Their denizens no longer consider you an easy mark."));
            UnderdarkCitizen.Available = true;
            UnderdarkCitizen.AvailableInCharacterCreation = true;
            UnderdarkCitizen.CanRemove = false;
            UnderdarkCitizen.CanSwap = false;
            UnderdarkCitizen.Conflicting.AddRange(new string[] { vTrait.TheLaw });
            UnderdarkCitizen.CostInCharacterCreation = 2;
            UnderdarkCitizen.IsActive = true;
            UnderdarkCitizen.Upgrade = null;
            #endregion
            #region Tampering
            CustomTrait TamperTantrum = RogueLibs.CreateCustomTrait(cTrait.TamperTantrum, true,
                new CustomNameInfo("Tamper Tantrum"),
                new CustomNameInfo("Your tools take less wear from tampering."));
            TamperTantrum.Available = true;
            TamperTantrum.AvailableInCharacterCreation = true;
            TamperTantrum.CanRemove = false;
            TamperTantrum.CanSwap = true;
            TamperTantrum.Conflicting.AddRange(new string[] { });
            TamperTantrum.CostInCharacterCreation = 2;
            TamperTantrum.IsActive = true;
            TamperTantrum.Upgrade = cTrait.TamperTantrum_2;

            CustomTrait TamperTantrum_2 = RogueLibs.CreateCustomTrait(cTrait.TamperTantrum_2, false,
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
        public static bool isPlayerInitialRelationshipTraitActive = false;
        public static List<T> ConcatTraitLists<T>(params IEnumerable<T>[] enums)
            => enums.SelectMany(e => e).ToList();
        internal static string HealthCost(Agent agent, int baseDamage, DamageType type)
        {
            BMLog("HealthCost");

            if (type == DamageType.burnedFingers)
            {
                if (agent.statusEffects.hasTrait(vTrait.ResistFire) || agent.statusEffects.hasTrait(vTrait.FireproofSkin) || agent.statusEffects.hasTrait(vTrait.FireproofSkin2))
                    return "0";
            }
            else if (type == DamageType.brokenWindow)
            {
                if (agent.statusEffects.hasTrait(cTrait.StealthBastardDeluxe))
                    return "0";
                else if (agent.statusEffects.hasTrait(vTrait.Diminutive))
                    return (baseDamage / 2).ToString();
            }

            return baseDamage.ToString();
        }
        public static void setPlayerInitialRelationshipTraitActive()
		{
            foreach (Agent agent in GC.agentList)
                if (agent.isPlayer != 0)
                    foreach (string se in cTrait.RelationshipInitial)
                        if (agent.statusEffects.hasTrait(se))
						{
                            isPlayerInitialRelationshipTraitActive = true;

                            BMLog("SetPlayerIRTActive" + isPlayerInitialRelationshipTraitActive);

                            return;
                        }

            isPlayerInitialRelationshipTraitActive = false;

            BMLog("SetPlayerIRTActive" + isPlayerInitialRelationshipTraitActive);
		}
        public static bool IsPlayerTraitActive(string trait)
		{
            foreach (Agent agent in GC.agentList)
                if (agent.isPlayer != 0 && agent.statusEffects.hasTrait(trait))
                    return true;
                    
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
		#endregion

		#region ScrollingMenu
        public void ScrollingMenu_00()
		{
            Type t = typeof(ScrollingMenu);
            Type g = GetType();

            Prefix(t, "GetTraitsRemoveTrait", g, "ScrollingMenu_GetTraitsRemoveTrait", new Type[0] { });
		}
        public static bool ScrollingMenu_GetTraitsRemoveTrait (ScrollingMenu __instance) // Replacement
		{
            // Removed gate that prevents traits with less than -5 value being removed.

            __instance.customTraitList.Clear();

            for (int i = 0; i < __instance.agent.statusEffects.TraitList.Count; i++)
            {
                Trait trait = __instance.agent.statusEffects.TraitList[i];

                for (int j = 0; j < __instance.gc.sessionDataBig.unlocks.Count; j++)
                {
                    Unlock unlock = __instance.gc.sessionDataBig.unlocks[j];

                    if (unlock.unlockName == trait.traitName && unlock.unlockType == "Trait" && unlock.cost3 < 0 && (!unlock.cantLose || __instance.gc.challenges.Contains("NoLimits")))
                    {
                        __instance.customTraitList.Add(unlock);
                    }
                }
            }

            return false;
        }
		#endregion

	}
}
