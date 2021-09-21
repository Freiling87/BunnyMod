using RogueLibsCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BunnyMod.Content.Abilities
{
	/// <summary>
	/// Provides methods for interfacing with BM Custom Abilities
	/// </summary>
	public class BMAbilityController
	{
		public static int CalcMaxMana(Agent agent)
		{
			if (agent.statusEffects.hasTrait(cTrait.ManaBattery))
				return 150;
			else if (agent.statusEffects.hasTrait(cTrait.ManaBattery_2))
				return 200;
			else if (agent.statusEffects.hasTrait(cTrait.Archmage))
				return 10000;
			else
				return 100;
		}

		public static Vector2 MouseIngamePosition()
		{
			Plane plane = new Plane(new Vector3(0, 0, 1), new Vector3(0, 0, 0));
			Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
			return plane.Raycast(ray, out float enter) ? (Vector2)ray.GetPoint(enter) : default;
		}

		public static void InitializeNames()
		{
			string t;

			t = vNameType.Dialogue;
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_CD_CantDo1, t, new CustomNameInfo("I need to take a \"time out!\" Get it? But seriously, my heart will stop."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_CD_CantDo2, t, new CustomNameInfo("I'm gonna take the blue pill for a sec."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_CD_CantDo3, t, new CustomNameInfo("I think this is giving me dementia."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_CD_CantDo4, t, new CustomNameInfo("Slow your roll! If I create a spacetime singularity people are gonna be mad."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_CD_Cast1, t, new CustomNameInfo("Stop right there! Okay, slowing down is cool too."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_CD_Cast2, t, new CustomNameInfo("Swallow this, clock-suckers!"));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_CD_Cast3, t, new CustomNameInfo("Nothing can slow me down!"));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_CD_Cast4, t, new CustomNameInfo("Freeze! In time, not in ice."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_CD_Decast1, t, new CustomNameInfo("Back to boring normal time."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_CD_Decast2, t, new CustomNameInfo("I guess there is a spoon."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_CD_Decast3, t, new CustomNameInfo("There can't always be Morpheus. Sometimes you get Lesspheus."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_CD_Decast4, t, new CustomNameInfo("You can only dilate time so much, or else everything just falls out."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_CD_Miscast1, t, new CustomNameInfo("Iii ttthhhiiinnnkkk Iii mmmeeesssssseeeddd uuuppp..."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_CD_Miscast2, t, new CustomNameInfo("Bullet Time? More like Bullshit Time!"));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_CD_Miscast3, t, new CustomNameInfo("(Slow Motion Noises)"));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_CD_Miscast4, t, new CustomNameInfo("I dilated time too much, and it turned into a chronological prolapse!"));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_CD_Recharge1, t, new CustomNameInfo("It's Slowing-down-time... time!"));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_CD_Recharge2, t, new CustomNameInfo("Not many time puns left. Uh... clock-a-doodle-doo?"));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_CD_Recharge3, t, new CustomNameInfo("All wound up and ready to run. Like a clock."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_CD_Recharge4, t, new CustomNameInfo("I've got a need for relative speed!"));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_PJ_CantDo1, t, new CustomNameInfo("I'm burned out."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_PJ_CantDo2, t, new CustomNameInfo("I don't feel like exploding right now."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_PJ_CantDo3, t, new CustomNameInfo("I need to eat more beans!"));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_PJ_CantDo4, t, new CustomNameInfo("Gimme a sec, I need to pop all these blisters from the heat."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_PJ_Cast1, t, new CustomNameInfo("Die! Burn! Die! Die!"));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_PJ_Cast2, t, new CustomNameInfo("Burn, baby, burn!"));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_PJ_Cast3, t, new CustomNameInfo("BURN-ie would have won!"));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_PJ_Cast4, t, new CustomNameInfo("You're fired! Hahaha!"));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_PJ_Miscast1, t, new CustomNameInfo("Not very stoked right now."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_PJ_Miscast2, t, new CustomNameInfo("Haha my skin is melting lol XDDD"));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_PJ_Miscast3, t, new CustomNameInfo("Flame off! Flame off!"));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_PJ_Miscast4, t, new CustomNameInfo("I shidded an farded an bursteded into flames."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_PJ_Recharge1, t, new CustomNameInfo("Ready to burn!"));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_PJ_Recharge2, t, new CustomNameInfo("I'm here to burn things and chew bubblegum. I'm not out of gum, but I'm still gonna do a lot of burning."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_PJ_Recharge3, t, new CustomNameInfo("(Laughs maniacally)"));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_PJ_Recharge4, t, new CustomNameInfo("Why are the innocent so fun to burn?"));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_TB_CantDo1, t, new CustomNameInfo("I need to give it a rest or my head will explode. I've seen it happen."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_TB_CantDo2, t, new CustomNameInfo("Slow down! Haven't you seen The Fly?"));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_TB_CantDo3, t, new CustomNameInfo("Don't abuse Spacetime too much, or you'll blink out of existence."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_TB_CantDo4, t, new CustomNameInfo("Let me stay here for a sec."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_TB_Cast1, t, new CustomNameInfo("Vwip!"));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_TB_Cast2, t, new CustomNameInfo("Nothing personal, kid."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_TB_Cast3, t, new CustomNameInfo("If you blink, I blink. And I'm gone when you open your eyes."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_TB_Cast4, t, new CustomNameInfo("Man, some smoke grenades would make this a lot cooler."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_TB_Miscast1, t, new CustomNameInfo("I smell burning toast."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_TB_Miscast2, t, new CustomNameInfo("Blurgh (Drool)"));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_TB_Miscast3, t, new CustomNameInfo("I pink I bust hab a stwoke."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_TB_Miscast4, t, new CustomNameInfo("My head a splode."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_TB_Recharge1, t, new CustomNameInfo("Who needs Scotty? I'll beam my damn self up."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_TB_Recharge2, t, new CustomNameInfo("All charged up and ready to blink!"));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_TB_Recharge3, t, new CustomNameInfo("Where do you want me?"));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_TB_Recharge4, t, new CustomNameInfo("Let's get outta here."));
		}

	}
}
