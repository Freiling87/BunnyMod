using BepInEx.Logging;
using BunnyMod.Content.Logging;
using BunnyMod.Content.Traits;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;
using Random = UnityEngine.Random;

namespace BunnyMod.Content
{
	public class CustomNameDump
	{
		[RLSetup]
		public void Names_Behavior()
		{
			string nameType = vNameType.Interface;
			_ = RogueLibs.CreateCustomName("Hobo_GiveMoney1", nameType, new CustomNameInfo("Give Money"));
			_ = RogueLibs.CreateCustomName("Hobo_GiveMoney2", nameType, new CustomNameInfo("Give Money"));
			_ = RogueLibs.CreateCustomName("Hobo_GiveMoney3", nameType, new CustomNameInfo("Give Money"));
			_ = RogueLibs.CreateCustomName("Hobo_GiveItem", nameType, new CustomNameInfo("Give Item"));
			_ = RogueLibs.CreateCustomName("Gangbanger_GiveMoney", nameType, new CustomNameInfo("Give Money"));

			nameType = vNameType.Dialogue;
			_ = RogueLibs.CreateCustomName("Hobo_CantAfford", nameType, new CustomNameInfo("Come on, don't hold out on me!"));
			_ = RogueLibs.CreateCustomName("Hobo_Donate_Aligned", nameType, new CustomNameInfo("I knew it, you're Jesus! Hey Jesus, get this CIA microchip outta my head! Wabadoobaflooba! Yaba! Haba!"));
			_ = RogueLibs.CreateCustomName("Hobo_Donate_Annoyed", nameType, new CustomNameInfo("...Okay."));
			_ = RogueLibs.CreateCustomName("Hobo_Donate_Friendly", nameType, new CustomNameInfo("Hey, thanks buddy!"));
			_ = RogueLibs.CreateCustomName("Hobo_Donate_Hostile", nameType, new CustomNameInfo("Fuck you, asshole!"));
			_ = RogueLibs.CreateCustomName("Hobo_Donate_Loyal", nameType, new CustomNameInfo("Oh, hell yeah!"));
			_ = RogueLibs.CreateCustomName("Hobo_Donate_Neutral", nameType, new CustomNameInfo("Oh, uh... thanks, I guess?"));
			_ = RogueLibs.CreateCustomName("Hobo_DontWant", nameType, new CustomNameInfo("Nah, what else you got?"));
			_ = RogueLibs.CreateCustomName("Hobo_Mug", nameType, new CustomNameInfo("Can you spare a few bucks, friendo?"));
			_ = RogueLibs.CreateCustomName("Gangbanger_CantAfford", nameType, new CustomNameInfo("Sucks to be you!"));
			_ = RogueLibs.CreateCustomName("Gangbanger_Mug", nameType, new CustomNameInfo("The cash, hand it over!"));
			_ = RogueLibs.CreateCustomName("GangbangerB_Mug", nameType, new CustomNameInfo("Empty your pockets, bitch!"));
		}

		[RLSetup]
		public void Names_Combat()
		{
			string t = vNameType.StatusEffect;
			_ = RogueLibs.CreateCustomName("Headshot", t, new CustomNameInfo("Headshot"));
		}

		[RLSetup]
		public void Names_Items()
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
	}
}
