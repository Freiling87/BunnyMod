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
            DoubleTapper.CostInCharacterCreation = 8;
            DoubleTapper.Recommendations.Add(vSpecialAbility.Camouflage);
            DoubleTapper.IsActive = true;
            DoubleTapper.Upgrade = null;

            CustomTrait Sniper = RogueLibs.CreateCustomTrait(cTrait.Sniper, true,
                new CustomNameInfo("Sniper"),
                new CustomNameInfo("One shot, a pretty good chance of one kill. They can run, but they'll only die tired, unless they get away.\n\n- Past a certain distance, Revolver hits on unaware targets deal massive damage\n- Bullet range increased by ~60%"));
            Sniper.Available = true;
            Sniper.AvailableInCharacterCreation = true;
            Sniper.CanRemove = false;
            Sniper.CanSwap = true;
            Sniper.Conflicting.AddRange(new string[] { cTrait.Myopic, cTrait.Myopic2 });
            Sniper.CostInCharacterCreation = 4;
            Sniper.Recommendations.Add(cTrait.EagleEyes);
            Sniper.IsActive = true;
            Sniper.Upgrade = null;
            #endregion
            #region Drug Limitations
            CustomTrait DAREdevil = RogueLibs.CreateCustomTrait(cTrait.DAREdevil, true,
                new CustomNameInfo("DAREdevil", "", "", "", "", "Наркотический отступник", "", ""),
                new CustomNameInfo("You have injected zero marijuanas. Crack is Whack. Smokers are Jokers. Needles are for... beetles.", "", "", "", "", "За свою жизнь вы не разу не употребили марихуану. Для вас крэк - трещина, курилщики - шутники, а иголки для ежей.", "", ""));
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
                new CustomNameInfo("Friend Of Bill", "", "", "", "", "Бывший алкоголик", "", ""),
                new CustomNameInfo("You are taking things one day at a time. But every day sucks when you can't get drunk anymore.", "", "", "", "", "Вы пьёте одну бутылку за другой, но каждый день без выпивки - это отстой!", "", ""));
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
                new CustomNameInfo("Teetotaller", "", "", "", "", "Трезвенник", "", ""),
                new CustomNameInfo("Wow, you're really boring. You don't do drugs *or* alcohol. What do you even do?", "", "", "", "", "Удивительно но вы действительно скучный. Вы не употребляете наркотики или алкоголь.. Вы вообще живой?", "", ""));
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
                new CustomNameInfo("Carnivore", "", "", "", "", "Плотоядное животное", "", ""),
                new CustomNameInfo("\"Meeeeeeat,\" you grunt enthusiastically.", "", "", "", "", "\"Мясооооо!\" Вы рычите эту фразу с большим энтузиазмом!", "", ""));
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
                new CustomNameInfo("Vegetarian", "", "", "", "", "Веган", "", ""),
                new CustomNameInfo("You are one of those people.", "", "", "", "", "Вы один из этих людей. Ну вы поняли.. вы.. веган!", "", ""));
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
            #region Equipment Limitations
            CustomTrait AfraidOfLoudNoises = RogueLibs.CreateCustomTrait(cTrait.AfraidOfLoudNoises, true,
                new CustomNameInfo("Afraid of Loud Noises", "", "", "", "", "Боязнь громких звуков", "", ""),
                new CustomNameInfo("The recoil bruised my shouldah. The brass shell casings disoriented me as they flew past my face. The smell of sulfur and destruction made me sick. The explosions - loud like a bowomb - gave me a temporary case of PTSD. For at least an hour after firing the gun just a few times, I was anxious and irritable. And it's such small portions!", "", "", "", "", "Отдача от оружия сильно ударяет меня в плечё. Пули пролетая мимо меня сильно дезоринтируют, а от запаха серы у меня проявляются рвотные рефлексы. Громкие взрывы вызывают у меня приступы ПТСР. Всё это я узнал когда выстрелил одну пулю из пистолета.. и это лишь одна пуля..", "", ""));
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
                new CustomNameInfo("Draw No Blood", "", "", "", "", "Кровянная клятва", "", ""),
                new CustomNameInfo("You have taken an oath to draw no blood. Guess you'll have to smash skulls really carefully, then.\n\n- Can't use bullet-based guns, sharp weapons, or most explosives.", "", "", "", "", "Вы приняли присягу не проливать ни капли крови.. Думаю вам надо будет крайней тщательно ломать черепа.\n\nВы не можете использовать огнестрельное оружие, колющее и режущее оружие, а также большинство взрывчатых веществ.", "", ""));
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
                new CustomNameInfo("Fat Head", "", "", "", "", "Жирная голова", "", ""),
                new CustomNameInfo("You have a big, fat, ugly head. You can't wear hats of any kind. No one will lend you their headphones or sunglasses, because your big, fat, dumb, ugly head will break them. Your self-esteem is pretty much in the toilet.", "", "", "", "", "У вас большая, жирная и уродливая голова. Вы даже не можете носить никакие головные уборы. Никто вам не одолжит солнцезащитные очки или наушники, вы их просто сломаете своей тупой, жирной головой. Неудивительно, что ваша самооценка ниже плинтуса.", "", ""));
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
            CustomTrait Brainiac = RogueLibs.CreateCustomTrait(cTrait.Brainiac, true,
                new CustomNameInfo("000: Brainiac"),
                new CustomNameInfo("You have a ton of potential, and pretty much nothing else. Hopefully you live past level 1. We're rooting for you, buddy.\n\n-XP gain set to 400%"));
            Brainiac.Available = true;
            Brainiac.AvailableInCharacterCreation = true;
            Brainiac.CanRemove = false;
            Brainiac.CanSwap = false;
            Brainiac.Conflicting.AddRange(cTrait.ExperienceRate);
            Brainiac.Conflicting.AddRange(vTrait.ExperienceRate);
            Brainiac.Conflicting.Remove(cTrait.Brainiac);
            Brainiac.CostInCharacterCreation = 40;
            Brainiac.IsActive = true;
            Brainiac.Upgrade = null;

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
                new CustomNameInfo("Charmed & Dangerous", "", "", "", "", "везучий", "", ""),
                new CustomNameInfo("You once found a fourteen-leaf clover.", "", "", "", "", "Как-то раз вы нашли 14 листьев клевера.", "", ""));
            Charmed.Available = true;
            Charmed.AvailableInCharacterCreation = true;
            Charmed.CanRemove = false;
            Charmed.CanSwap = true;
            Charmed.Conflicting.AddRange(cTrait.Luck);
            Charmed.Conflicting.Remove(cTrait.Charmed);
            Charmed.CostInCharacterCreation = 4;
            Charmed.IsActive = true;
            Charmed.Upgrade = cTrait.Charmed_2;

            CustomTrait Charmed_2 = RogueLibs.CreateCustomTrait(cTrait.Charmed_2, false,
                new CustomNameInfo("Charmed & Dangerous +", "", "", "", "", "везучий +", "", ""),
                new CustomNameInfo("You are *really* lucky. Anyone who's been at the toilet next to you can attest.", "", "", "", "", "Вам действительно повезло. Любой кто был у туалета рядом с вами это подтвердит.", "", ""));
            Charmed_2.Available = true;
            Charmed_2.AvailableInCharacterCreation = false;
            Charmed_2.CanRemove = false;
            Charmed_2.CanSwap = true;
            Charmed_2.Conflicting.AddRange(cTrait.Luck);
            Charmed_2.Conflicting.Remove(cTrait.Charmed_2);
            Charmed_2.CostInCharacterCreation = 8;
            Charmed_2.Upgrade = null;

            CustomTrait Cursed = RogueLibs.CreateCustomTrait(cTrait.Cursed, true,
                new CustomNameInfo("Unlucky", "", "", "", "", "Несчастный", "", ""),
                new CustomNameInfo("You pissed in some old Gypsy lady's cereal, and you still refuse to apologize. She didn't like that.", "", "", "", "", "Вы помочились в кашу старой заклинательницы и до сих пор не извинились. Ей это явно не нравится.", "", ""));
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
                new CustomNameInfo("Unlucky +", "", "", "", "", "Несчастный +", "", ""),
                new CustomNameInfo("You bought up an old Indian graveyard, and there you built a black cat sanctuary and mirror-breakery. Not your best choice.", "", "", "", "", "Вы купили старое индийское кладбище и построили там приют для чёрных кошек. Не самый лучший выбор.", "", ""));
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
                new CustomNameInfo("Archmage", "", "", "", "", "Архимаг", "", ""),
                new CustomNameInfo("You are an unrivalled master of the magical arts.\n\nBasically cheat mode for magical abilities, added by request.", "", "", "", "", "Вы непревзойдённый мастер магических исскуств.\n\nИ да это режим чит для магических способностей, добавлен по запросу.", "", ""));
            Archmage.Available = true;
            Archmage.AvailableInCharacterCreation = true;
            Archmage.CanRemove = false;
            Archmage.CanSwap = false;
            Archmage.Conflicting.AddRange(cTrait.Magic);
            Archmage.Conflicting.Remove(cTrait.Archmage);
            Archmage.CostInCharacterCreation = 32;
            Archmage.IsActive = true;
            Archmage.Recommendations.AddRange(cSpecialAbility.Magic);
            Archmage.SpecialAbilities.AddRange(cSpecialAbility.Magic);
            Archmage.Upgrade = null;

            CustomTrait FocusedCasting = RogueLibs.CreateCustomTrait(cTrait.FocusedCasting, true,
                new CustomNameInfo("Focused Casting", "", "", "", "", "Фокусировка кастов", "", ""),
                new CustomNameInfo("You've carefully refined your magic techniques to improve accuracy and reduce the chances of miscasting spells.", "", "", "", "", "Вы тщательно отточили свои магические приёмы, теперь вы стреляете точнее, а шанс на неудачный каст стал ниже.", "", ""));
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
                new CustomNameInfo("Focused Casting +", "", "", "", "", "Фокусировка кастов +", "", ""),
                new CustomNameInfo("You've even more carefully refined your techniques even more to improve accuracy and even more reduce the chances of miscasting spells EVEN MORE.", "", "", "", "", "Вы ещё более тщательно отточили свои магические приёмы, вы стали стрелять ещё точнее и шанс на неудачный каст стал ещё ниже.", "", ""));
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
                new CustomNameInfo("Magic Training", "", "", "", "", "Самоучитель по магии", "", ""),
                new CustomNameInfo("Improves your skills with any Magic Special Ability.", "", "", "", "", "Улучшает ваши навыки обращения с магией! Спец. способность.", "", ""));
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
                new CustomNameInfo("Magic Training +", "", "", "", "", "Самоучитель по магии +", "", ""),
                new CustomNameInfo("Further improves your skills with any Magic Special Ability.", "", "", "", "", "Теперь ещё больше улучшает навыки обращения с магией! Спец. способность.", "", ""));
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
                new CustomNameInfo("Mana Battery", "", "", "", "", "Аккумулятор маны", "", ""),
                new CustomNameInfo("You can store more mana. Nifty.", "", "", "", "", "Вы храните больше маны чем обычно, изящненько.", "", ""));
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
                new CustomNameInfo("Mana Battery +", "", "", "", "", "Аккумулятор маны +", "", ""),
                new CustomNameInfo("You can store even more mana. Even niftier.", "", "", "", "", "Вы можете хранить ещё больше маны, изящно!", "", ""));
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
                new CustomNameInfo("Wild Casting", "", "", "", "", "Бешеный Кастер", "", ""),
                new CustomNameInfo("You don't need all that safety shit. You wanna cast some damn spells! Your spells are more powerful, but you have a greater chance of miscasting them.", "", "", "", "", "Безопасность только для лохов! Вы хотите кастовать чёртовы мощные заклинания! Ваши заклинания более мощные, но и шанс на неудачный каст заклинания выше.", "", ""));
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
                new CustomNameInfo("Wild Casting +", "", "", "", "", "Бешеный Кастер +", "", ""),
                new CustomNameInfo("You're addicted to power. Your spells are ridiculously powerful... and risky.", "", "", "", "", "Вы зависимы от магии. Ваши заклинания рискованны, но при этом мощные и смешные.", "", ""));
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
                new CustomNameInfo("R.A.T.S. Mk VI", "", "", "", "", "С.С.П.П Мк. VI", "", ""),
                new CustomNameInfo("Resistance-Tec Assisted Targeting System. The latest cybernetic enhancement to shooting accuracy, crit chance, & some combat traits with a double effect when time is slowed down.", "", "", "", "", "Сопротивленческая Система Помощи Прицеливания. Совершенная кибернетеческая система улучшающая стрельбу, шанс крита, так же некоторые боевые особенности удваиваются, когда замедляется время.", "", ""));
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
                new CustomNameInfo("R.A.T.S. Mk VII", "", "", "", "", "С.С.П.П Мк. VII", "", ""),
                new CustomNameInfo("\"Various bug fixes and performance improvements.\" That's all the fucking update notes say. Also, they took out the headphone jack??", "", "", "", "", "\"Различные исправления багов и улучшение производительности.\" Это всё что сказано про это чёртово обновление! Они ещё и удалили разьём под наушники?!", "", ""));
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
                new CustomNameInfo("Eagle Eyes", "", "", "", "", "Орлиные глаза", "", ""),
                new CustomNameInfo("You can see further than normal. Hell, you can see further than *abnormal*.", "", "", "", "", "Вы можете видеть дальше, чем обычно. Чёрт! Это же ненормально!", "", ""));
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
                new CustomNameInfo("Eagle Eyes +", "", "", "", "", "Орлиные глаза +", "", ""),
                new CustomNameInfo("You might have been a good sniper, but you were kicked out of training when they didn't believe that you worked better without a rifle scope.", "", "", "", "", "Возможно вы были прекрасным снайпером, но вас выгнали на одной из тренировок, видимо они не поверили что вы стреляете лучше без прицела.", "", ""));
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
                new CustomNameInfo("Myopic", "", "", "", "", "Близорукость", "", ""),
                new CustomNameInfo("You can't see too far. In fact, you can't see far enough.", "", "", "", "", "Вы не можете видеть слишком далеко.", "", ""));
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
                new CustomNameInfo("Ultramyopic", "", "", "", "", "Супер близорукость", "", ""),
                new CustomNameInfo("You tend to keep people at arm's length, where you can't see them.", "", "", "", "", "Обычно вы держите людей на расстоянии вытянотой руки, жаль что вы их всё равно не видите.", "", ""));
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
                new CustomNameInfo("Domineering", "", "", "", "", "Доминирующий", "", ""),
                new CustomNameInfo("There's just something about how you carry yourself. Maybe it's the way you walk, or maybe it's the way you demand obedience from the weak around you. People will occasionally be Submissive to you. Kinky!", "", "", "", "", "Есть что-то в том, как ты себя ведешь. Может быть, что дело в том как вы ходите, а может в том, как вы требуете подчинения от слабых вокруг. Люди иногда будут повиноватся вам, но вы крайне эксцентричны!", "", ""));
            Domineering.Available = true;
            Domineering.AvailableInCharacterCreation = true;
            Domineering.CanRemove = true;
            Domineering.CanSwap = false;
            Domineering.Conflicting.Add(cTrait.Domineering_2);
            Domineering.CostInCharacterCreation = 2;
            Domineering.IsActive = true;
            Domineering.Upgrade = cTrait.Domineering_2;

            CustomTrait Domineering_2 = RogueLibs.CreateCustomTrait(cTrait.Domineering_2, true,
                new CustomNameInfo("Domineering +", "", "", "", "", "Доминирующий + ", "", ""),
                new CustomNameInfo("Some people make sure their social skills merely work for them, but you crack the whip! You're finding more and more Subs everywhere you look.", "", "", "", "", "Некоротые люди убеждаются, что ваши социальные навыки работают на них. Ты треснешь кнутом. Вы находите все больше и больше подводных камней куда бы вы ни посмотрели.", "", ""));
            Domineering_2.Available = true;
            Domineering_2.AvailableInCharacterCreation = false;
            Domineering_2.CanRemove = true;
            Domineering_2.CanSwap = false;
            Domineering_2.Conflicting.Add(cTrait.Domineering);
            Domineering_2.CostInCharacterCreation = 4;
            Domineering_2.IsActive = true;
            Domineering_2.Upgrade = null;

            CustomTrait GenerallyUnpleasant = RogueLibs.CreateCustomTrait(cTrait.GenerallyUnpleasant, true,
                new CustomNameInfo("Generally Unpleasant", "", "", "", "", "Вообще неприятный", "", ""),
                new CustomNameInfo("You have a certain way with people! A very annoying way.", "", "", "", "", "Вы крайне раздражительны, вы даже не умеете общатся с людьми! Это оооочень... раздражает.", "", ""));
            GenerallyUnpleasant.Available = true;
            GenerallyUnpleasant.AvailableInCharacterCreation = true;
            GenerallyUnpleasant.CanRemove = true;
            GenerallyUnpleasant.CanSwap = false;
            GenerallyUnpleasant.Conflicting.AddRange(new string[] { vTrait.Antisocial, vTrait.Charismatic, vTrait.FairGame, vTrait.FriendoftheCommonFolk, cTrait.GenerallyUnpleasant_2, vTrait.Malodorous, cTrait.Polarizing, cTrait.Polarizing_2, vTrait.Suspicious, vTrait.Wanted });
            GenerallyUnpleasant.CostInCharacterCreation = -3;
            GenerallyUnpleasant.IsActive = true;
            GenerallyUnpleasant.Upgrade = null;

            CustomTrait ObjectivelyUnpleasant = RogueLibs.CreateCustomTrait(cTrait.GenerallyUnpleasant_2, true,
                new CustomNameInfo("Objectively Unpleasant", "", "", "", "", "Обьективно неприятный", "", ""),
                new CustomNameInfo("You chew with your mouth open. You rightfully have no friends in the world. You are scum. Everyone starts out Annoyed, including me.", "", "", "", "", "Вы чавкаете во время еды. У вас в этом мире нету ни единого друга. Вы подонок. Все кто с вами встречаются раздражены, включая меня.", "", ""));
            ObjectivelyUnpleasant.Available = true;
            ObjectivelyUnpleasant.AvailableInCharacterCreation = true;
            ObjectivelyUnpleasant.CanRemove = true;
            ObjectivelyUnpleasant.CanSwap = false;
            ObjectivelyUnpleasant.Conflicting.AddRange(new string[] { vTrait.Antisocial, vTrait.Charismatic, vTrait.FairGame, vTrait.FriendoftheCommonFolk, cTrait.GenerallyUnpleasant, vTrait.Malodorous, cTrait.Polarizing, cTrait.Polarizing_2, vTrait.Suspicious, vTrait.Wanted });
            ObjectivelyUnpleasant.CostInCharacterCreation = -8;
            ObjectivelyUnpleasant.IsActive = true;
            ObjectivelyUnpleasant.Upgrade = null;

            CustomTrait Polarizing = RogueLibs.CreateCustomTrait(cTrait.Polarizing, true,
                new CustomNameInfo("Polarizing", "", "", "", "", "Популяризация", "", ""),
                new CustomNameInfo("Everyone has an opinion on you, when they first meet you. Might be good or bad, but at least you feel noticed!", "", "", "", "", "Каждый раз когда с вами встречается новый человек у него всегда будет о вас мнение, хорошее или плохое, зато вас заметили!", "", ""));
            Polarizing.Available = true;
            Polarizing.AvailableInCharacterCreation = false;
            Polarizing.CanRemove = false;
            Polarizing.CanSwap = true;
            Polarizing.Conflicting.AddRange(new string[] { vTrait.Antisocial, vTrait.Charismatic, vTrait.FairGame, vTrait.FriendoftheCommonFolk, cTrait.GenerallyUnpleasant, cTrait.GenerallyUnpleasant_2, vTrait.Malodorous, cTrait.Polarizing_2, vTrait.Suspicious, vTrait.Wanted });
            Polarizing.CostInCharacterCreation = 0;
            Polarizing.IsActive = true;
            Polarizing.Upgrade = cTrait.Polarizing_2;

            CustomTrait Polarizing_2 = RogueLibs.CreateCustomTrait(cTrait.Polarizing_2, true,
                new CustomNameInfo("Polarizing +", "", "", "", "", "Популяризация +", "", ""),
                new CustomNameInfo("People have *strong* opinions of you. Like me. I think you're just great.", "", "", "", "", "Люди хорошего мнения о вас. Даже я. Я думаю ты просто чудо.", "", ""));
            Polarizing_2.Available = true;
            Polarizing_2.AvailableInCharacterCreation = true;
            Polarizing_2.CanRemove = false;
            Polarizing_2.CanSwap = true;
            Polarizing_2.Conflicting.AddRange(new string[] { vTrait.Antisocial, vTrait.Charismatic, vTrait.FairGame, vTrait.FriendoftheCommonFolk, cTrait.GenerallyUnpleasant, cTrait.GenerallyUnpleasant_2, vTrait.Malodorous, cTrait.Polarizing, vTrait.Suspicious, vTrait.Wanted });
            Polarizing_2.CostInCharacterCreation = 1;
            Polarizing_2.IsActive = true;
            Polarizing_2.Upgrade = null;

            CustomTrait Priors = RogueLibs.CreateCustomTrait(cTrait.Priors, true,
                new CustomNameInfo("Priors", "", "", "", "", "Давные преступления", "", ""),
                new CustomNameInfo("You have a long rap sheet, and the police know you by first name. On the upside, you've earned the respect of the City's criminals.", "", "", "", "", "У вас длинный список преступлений и каждый коп знает вас в лицо. Они так устали от вас и вашего дерьма, что только и ищут повод, чтобы избить вас.", "", ""));
            Priors.Available = true;
            Priors.AvailableInCharacterCreation = true;
            Priors.CanRemove = true;
            Priors.CanSwap = true;
            Priors.Conflicting.AddRange(new string[] { vTrait.Charismatic, vTrait.CopsDontCare, vTrait.CorruptionCosts, vTrait.FairGame, vTrait.Suspicious, vTrait.TheLaw, vTrait.Wanted });
            Priors.CostInCharacterCreation = -1;
            Priors.IsActive = true;
            Priors.Upgrade = null;

            CustomTrait VeiledThreats = RogueLibs.CreateCustomTrait(cTrait.VeiledThreats, true,
                new CustomNameInfo("000: Veiled Threats", "", "", "", "", "Скрытое давление", "", ""),
                new CustomNameInfo("When you attempt to Bribe, Extort, Mug, or Threaten, a failure will turn the target Annoyed instead of Hostile.", "", "", "", "", "Когда вы пытаетесь угрожать, подкупить, грабить если ваша цель откажется, то она будет раздражена, а не враждебна.", "", ""));
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
                new CustomNameInfo("Warlord", "", "", "", "", "Варлорд", "", ""),
                new CustomNameInfo("What is best in this game run, Onan? \n\nTo gib your enemies, to see them fall at your little rectangle legs -- to take their E_Horse and item drops and hear the lamentations of their... people with pixelated, possibly female presentation. That is best!\n\nYou can force anyone into submission.", "", "", "", "", "Чтобы сокрушить своих врагов, чтобы увидеть их падения к вашим ногам - забрать их лошадей и их добро, услышать плачь их женщин. Это прекрасно! Вы можете убедить любого принять ваше правление с помощью силы.", "", ""));
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
                new CustomNameInfo("000: Haunted", "", "", "", "", "Призраки", "", ""),
                new CustomNameInfo("You spent too long spelunking in an ancient treasure vault. The local ghosts were very unhappy with you, and you had their legal case dismissed. Now they're taking it into their own hands.", "", "", "", "", "Вы потратили слишком много времени на спелеологию в древней сокровищнице. Местные призраки очень нерады вашему присутствию и вы перестали этим заниматся. Теперь призраки взяли всё в свои руки.", "", ""));
            Haunted.Available = true;
            Haunted.AvailableInCharacterCreation = true;
            Haunted.CanRemove = true;
            Haunted.CanSwap = false;
            Haunted.CostInCharacterCreation = -2;
            Haunted.IsActive = true;
            Haunted.Upgrade = null;

            CustomTrait MobDebt = RogueLibs.CreateCustomTrait(cTrait.MobDebt, true,
                new CustomNameInfo("000: Mob Debt", "", "", "", "", "Долг мафии", "", ""),
                new CustomNameInfo("You found a sack of money, and the people to whom it belongs want it back with interest. Well, they certainly have your interest, but you've spent the principal. Pay it off at an ATM by Level 10, or else your kneecaps (and the rest of your body) will pay the price.", "", "", "", "", "Вы нашли мешочек денег, однако его владельцы хотят его вернуть, как жаль что вы уже потратили основную часть этих денег, прийдётся возвращать с процентами. Оплатите сумму в банкомате на 10 этаже, иначе вместо вас будут платить ваши коленные чашечки.", "", ""));
            MobDebt.Available = true;
            MobDebt.AvailableInCharacterCreation = true;
            MobDebt.CanRemove = true;
            MobDebt.CanSwap = false;
            MobDebt.CostInCharacterCreation = -10;
            MobDebt.IsActive = true;
            MobDebt.Upgrade = null;

            CustomTrait MookMasher = RogueLibs.CreateCustomTrait(cTrait.MookMasher, true,
                new CustomNameInfo("000: Mook Masher", "", "", "", "", "Разрушитель сердец", "", ""),
                new CustomNameInfo("The Mayor knows you're a threat, and you're coming for him. He could just destroy you, but as a villain, he prefers to send his henchmen at you in steadily increasing but manageable waves.", "", "", "", "", "Когда вы пришли к Мэру, он знал что вы угроза. Он мог бы просто вас убить, но как любой злодей, он предпочтёл отправляться волны приспешников, волны постоянно растут, но они управляемые.", "", ""));
            MookMasher.Available = true;
            MookMasher.AvailableInCharacterCreation = true;
            MookMasher.CanRemove = true;
            MookMasher.CanSwap = false;
            MookMasher.CostInCharacterCreation = -5;
            MookMasher.IsActive = true;
            MookMasher.Upgrade = null;

            CustomTrait Reinforcements = RogueLibs.CreateCustomTrait(cTrait.Reinforcements, true,
                new CustomNameInfo("000: Reinforcements", "", "", "", "", "Оснаститель", "", ""),
                new CustomNameInfo("You have worked to create an army for the Resistance. That army now patrols the City secretly, looking for the opportunity to aid the cause.", "", "", "", "", "Вы работали над созданием армии для Сопротивления. На данный момент эта армия тайно патрулирует город, постоянно ищя возможность, чтобы помочь вашему общему делу.", "", ""));
            Reinforcements.Available = true;
            Reinforcements.AvailableInCharacterCreation = true;
            Reinforcements.CanRemove = false;
            Reinforcements.CanSwap = true;
            Reinforcements.Conflicting.AddRange(new string[] { cTrait.Reinforcements_2 });
            Reinforcements.CostInCharacterCreation = 4;
            Reinforcements.IsActive = true;
            Reinforcements.Upgrade = cTrait.Reinforcements_2;

            CustomTrait Reinforcements_2 = RogueLibs.CreateCustomTrait(cTrait.Reinforcements_2, true,
                new CustomNameInfo("000: Reinforcements +", "", "", "", "", "Оснаститель +", "", ""),
                new CustomNameInfo("Your allies now have top-of-the-line equipment.", "", "", "", "", "Ваши союзники теперь экипированны лучшим снаряжением.", "", ""));
            Reinforcements_2.Available = true;
            Reinforcements_2.AvailableInCharacterCreation = true;
            Reinforcements_2.CanRemove = false;
            Reinforcements_2.CanSwap = true;
            Reinforcements_2.Conflicting.AddRange(new string[] { cTrait.Reinforcements });
            Reinforcements_2.CostInCharacterCreation = 8;
            Reinforcements_2.IsActive = true;
            Reinforcements_2.Upgrade = null;
            #endregion
            #region Stealth
            CustomTrait StealthBastardDeluxe = RogueLibs.CreateCustomTrait(cTrait.StealthBastardDeluxe, true,
                new CustomNameInfo("Stealth Bastard Deluxe", "", "", "", "", "Скрытность воров максимальна", "", ""),
                new CustomNameInfo("- Move through broken windows without taking a scratch.\n- Open and close doors silently.\n- Hide in Bathtubs, Plants, Pool Tables, and Big Tables.", "", "", "", "", "Вы можете лазить через сломанные окна без единой царапины.\n\nВы можете прятатся в ванных, в растениях, за бильярдными столами и большими столами.", "", ""));
            StealthBastardDeluxe.Available = true;
            StealthBastardDeluxe.AvailableInCharacterCreation = true;
            StealthBastardDeluxe.CanRemove = false;
            StealthBastardDeluxe.CanSwap = true;
            StealthBastardDeluxe.Conflicting.AddRange(new string[] { vTrait.Loud, vTrait.Bulky });
            StealthBastardDeluxe.CostInCharacterCreation = 4;
            StealthBastardDeluxe.IsActive = true;
            StealthBastardDeluxe.Upgrade = null;

            CustomTrait UnderdarkCitizen = RogueLibs.CreateCustomTrait(cTrait.UnderdarkCitizen, true,
                new CustomNameInfo("000: Underdark Citizen", "", "", "", "", "Подземный житель", "", ""),
                new CustomNameInfo("You can navigate the city's sewers with ease. Their denizens no longer consider you an easy mark.", "", "", "", "", "Вы с лёгкостью оринтируетесь в канализации. Жители этих мест не считают вас лёгкой целью.", "", ""));
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
                new CustomNameInfo("Tamper Tantrum", "", "", "", "", "Вспышка Гнева", "", ""),
                new CustomNameInfo("Your tools take less wear from tampering.", "", "", "", "", "Ваши инструменты меньше изнашиваются от подделывания.", "", ""));
            TamperTantrum.Available = true;
            TamperTantrum.AvailableInCharacterCreation = true;
            TamperTantrum.CanRemove = false;
            TamperTantrum.CanSwap = true;
            TamperTantrum.Conflicting.AddRange(new string[] { });
            TamperTantrum.CostInCharacterCreation = 2;
            TamperTantrum.IsActive = true;
            TamperTantrum.Upgrade = cTrait.TamperTantrum_2;

            CustomTrait TamperTantrum_2 = RogueLibs.CreateCustomTrait(cTrait.TamperTantrum_2, false,
                new CustomNameInfo("Tamper Tantrum +", "", "", "", "", "Вспышка Гнева +", "", ""),
                new CustomNameInfo("Your tools take zero wear when used in tampering.", "", "", "", "", "Ваши инструменты не изнашиваются когда вы подделываете что-то.", "", ""));
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
        public static bool DoesPlayerHaveTraitFromList(Agent agent, List<string> traits)
		{
            BMLog("DoesPlayerHaveTraitFromList");

            foreach (string trait in traits)
                if (agent.statusEffects.hasTrait(trait))
                    return true;

            return false;
		}
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
