using RogueLibsCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BunnyMod.Content
{
	public class BMAbilities
	{
		public static GameController GC => GameController.gameController;

		public static bool Prefix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) =>
			BMHeader.MainInstance.PatchPrefix(type, methodName, patchType, patchMethodName, types);

		public static bool Postfix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) =>
			BMHeader.MainInstance.PatchPostfix(type, methodName, patchType, patchMethodName, types);

		public static void BMLog(string logMessage) => BMHeader.Log(logMessage);

		public void Awake()
		{
			InitializeAbilities();

			Agent_00();
			StatusEffects_00();
		}

		public static void InitializeAbilities()
		{
			MSA_CD_Initialize();
			MSA_PJ_Initialize();
			MSA_TB_Initialize();
		}

		public static void InitializeNames()
		{
			string t;

			t = vNameType.Dialogue;
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_CD_CantDo1, t,
				new CustomNameInfo("I need to take a \"time out!\" Get it? But seriously, my heart will stop."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_CD_CantDo2, t, new CustomNameInfo("I'm gonna take the blue pill for a sec."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_CD_CantDo3, t, new CustomNameInfo("I think this is giving me dementia."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_CD_CantDo4, t,
				new CustomNameInfo("Slow your roll! If I create a spacetime singularity people are gonna be mad."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_CD_Cast1, t, new CustomNameInfo("Stop right there! Okay, slowing down is cool too."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_CD_Cast2, t, new CustomNameInfo("Swallow this, clock-suckers!"));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_CD_Cast3, t, new CustomNameInfo("Nothing can slow me down!"));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_CD_Cast4, t, new CustomNameInfo("Freeze! In time, not in ice."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_CD_Decast1, t, new CustomNameInfo("Back to boring normal time."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_CD_Decast2, t, new CustomNameInfo("I guess there is a spoon."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_CD_Decast3, t, new CustomNameInfo("There can't always be Morpheus. Sometimes you get Lesspheus."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_CD_Decast4, t,
				new CustomNameInfo("You can only dilate time so much, or else everything just falls out."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_CD_Miscast1, t, new CustomNameInfo("Iii ttthhhiiinnnkkk Iii mmmeeesssssseeeddd uuuppp..."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_CD_Miscast2, t, new CustomNameInfo("Bullet Time? More like Bullshit Time!"));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_CD_Miscast3, t, new CustomNameInfo("(Slow Motion Noises)"));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_CD_Miscast4, t,
				new CustomNameInfo("I dilated time too much, and it turned into a chronological prolapse!"));
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
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_PJ_Recharge2, t,
				new CustomNameInfo("I'm here to burn things and chew bubblegum. I'm not out of gum, but I'm still gonna do a lot of burning."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_PJ_Recharge3, t, new CustomNameInfo("(Laughs maniacally)"));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_PJ_Recharge4, t, new CustomNameInfo("Why are the innocent so fun to burn?"));

			_ = RogueLibs.CreateCustomName(cDialogue.MSA_TB_CantDo1, t,
				new CustomNameInfo("I need to give it a rest or my head will explode. I've seen it happen."));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_TB_CantDo2, t, new CustomNameInfo("Slow down! Haven't you seen The Fly?"));
			_ = RogueLibs.CreateCustomName(cDialogue.MSA_TB_CantDo3, t,
				new CustomNameInfo("Don't abuse Spacetime too much, or you'll blink out of existence."));
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

		#region Magic General

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
			return plane.Raycast(ray, out float enter) ? (Vector2) ray.GetPoint(enter) : default;
		}

		#endregion

		#region Chronomantic Dilation

		#region Chronomantic Dilation - Bits

		public static bool MSA_CD_IsCast(Agent agent)
		{
			try
			{
				return (agent.inventory.equippedSpecialAbility.otherDamage & 0b_0001) != 0;
			}
			catch
			{
				return false;
			}
		}

		public static bool MSA_CD_IsMiscast(Agent agent) =>
			(agent.inventory.equippedSpecialAbility.otherDamage & 0b_0010) != 0;

		public static bool MSA_CD_IsWindingUp(Agent agent) =>
			(agent.inventory.equippedSpecialAbility.otherDamage & 0b_0100) != 0;

		public static void MSA_CD_LogVariables(Agent agent)
		{
			BMLog("ChronomancyIsCast: " + MSA_CD_IsCast(agent));
			BMLog("ChronomancyIsMiscast: " + MSA_CD_IsMiscast(agent));
			BMLog("ChronomancyIsWindindUp: " + MSA_CD_IsWindingUp(agent));
		}

		public static void MSA_CD_SetCast(Agent agent, bool value)
		{
			//BunnyHeader.Log("SetCast " + value);

			if (value) agent.inventory.equippedSpecialAbility.otherDamage |= 0b_0001;
			else agent.inventory.equippedSpecialAbility.otherDamage &= ~0b_0001;
		}

		public static void MSA_CD_SetMiscast(Agent agent, bool value)
		{
			//BunnyHeader.Log("SetMiscast " + value);

			if (value) agent.inventory.equippedSpecialAbility.otherDamage |= 0b_0010;
			else agent.inventory.equippedSpecialAbility.otherDamage &= ~0b_0010;
		}

		public static void MSA_CD_SetWindingUp(Agent agent, bool value)
		{
			//BunnyHeader.Log("SetWindingUp" + value);

			if (value) agent.inventory.equippedSpecialAbility.otherDamage |= 0b_0100;
			else agent.inventory.equippedSpecialAbility.otherDamage &= ~0b_0100;
		}

		#endregion

		#region Chronomantic Dilation - Dialogue

		public static void MSA_CD_DialogueCantDo(Agent agent)
		{
			if (GC.percentChance(75))
				return;

			GC.audioHandler.Play(agent, "CantDo");

			List<string> dialogue = new List<string>()
			{
				cDialogue.MSA_CD_CantDo1,
				cDialogue.MSA_CD_CantDo2,
				cDialogue.MSA_CD_CantDo3,
				cDialogue.MSA_CD_CantDo4,
			};

			BMHeaderTools.SayDialogue(agent, BMHeaderTools.RandomFromList(dialogue), vNameType.Dialogue);
		}

		public static void MSA_CD_DialogueCast(Agent agent)
		{
			agent.SpawnParticleEffect("ExplosionMindControl", agent.curPosition);
			GameController.gameController.audioHandler.Play(agent, "MakeOffering");

			if (GC.percentChance(75))
				return;

			List<string> dialogue = new List<string>()
			{
				cDialogue.MSA_CD_Cast1,
				cDialogue.MSA_CD_Cast2,
				cDialogue.MSA_CD_Cast3,
				cDialogue.MSA_CD_Cast4,
			};

			BMHeaderTools.SayDialogue(agent, BMHeaderTools.RandomFromList(dialogue), vNameType.Dialogue);
		}

		public static void MSA_CD_DialogueDecast(Agent agent)
		{
			GameController.gameController.audioHandler.Play(agent, "MakeOffering");

			if (GC.percentChance(75))
				return;

			List<string> dialogue = new List<string>()
			{
				cDialogue.MSA_CD_Decast1,
				cDialogue.MSA_CD_Decast2,
				cDialogue.MSA_CD_Decast3,
				cDialogue.MSA_CD_Decast4,
			};

			BMHeaderTools.SayDialogue(agent, BMHeaderTools.RandomFromList(dialogue), vNameType.Dialogue);
		}

		public static void MSA_CD_DialogueMiscast(Agent agent)
		{
			agent.SpawnParticleEffect("ExplosionEMP", agent.curPosition);
			GC.audioHandler.Play(agent, "ToiletTeleportIn");

			if (GC.percentChance(75))
				return;

			List<string> dialogue = new List<string>()
			{
				cDialogue.MSA_CD_Miscast1,
				cDialogue.MSA_CD_Miscast2,
				cDialogue.MSA_CD_Miscast3,
				cDialogue.MSA_CD_Miscast4,
			};

			BMHeaderTools.SayDialogue(agent, BMHeaderTools.RandomFromList(dialogue), vNameType.Dialogue);
		}

		public static void MSA_CD_DialogueRecharge(Agent agent)
		{
			agent.statusEffects.CreateBuffText("Recharged", agent.objectNetID);
			GC.audioHandler.Play(agent, "Recharge");

			if (GC.percentChance(75))
				return;

			List<string> dialogue = new List<string>()
			{
				cDialogue.MSA_CD_Recharge1,
				cDialogue.MSA_CD_Recharge2,
				cDialogue.MSA_CD_Recharge3,
				cDialogue.MSA_CD_Recharge4,
			};

			BMHeaderTools.SayDialogue(agent, BMHeaderTools.RandomFromList(dialogue), vNameType.Dialogue);
		}

		#endregion

		public static float baseTimeScale;

		public static void MSA_CD_Initialize()
		{
			Sprite spriteChronomanticDilation = RogueUtilities.ConvertToSprite(Properties.Resources.ChronomanticDilation);

			CustomAbility chronomanticDilation = RogueLibs.CreateCustomAbility(cSpecialAbility.ChronomanticDilation, spriteChronomanticDilation, true,
				new CustomNameInfo("Chronomantic Dilation", "", "", "", "", "Хрономантический сдвиг", "", ""),
				new CustomNameInfo("Cast: Toggle to slow down everything but yourself. Toggle to deactivate.\nMiscast: Speed up everything but yourself.", "",
					"", "", "",
					"Заклинание: Нажмите кнопку Супер-способности, чтобы замедлить всё, кроме себя. Нажмите повторно, чтобы выключить замедление.\nНеудача: Ускоряет всё кроме вас.",
					"", ""),
				delegate(InvItem item)
				{
					item.cantDrop = true;
					item.Categories.Add("Usable");
					item.Categories.Add("NPCsCantPickup");
					item.dontAutomaticallySelect = true;
					item.dontSelectNPC = true;
					item.otherDamage = 0; // Bitwise variable field, see Extension method class below
					item.isWeapon = false;
					item.initCount = 100;
					item.rechargeAmountInverse = 100;
					item.maxAmmo = 100;
					item.stackable = true;
					item.thiefCantSteal = true;
				});

			chronomanticDilation.Available = true;
			chronomanticDilation.AvailableInCharacterCreation = true;
			chronomanticDilation.CostInCharacterCreation = 10;

			chronomanticDilation.OnPressed = delegate(InvItem item, Agent agent)
			{
				if (MSA_CD_IsWindingUp(agent) || MSA_CD_IsMiscast(agent))
					MSA_CD_DialogueCantDo(agent);
				else if (MSA_CD_IsCast(agent))
					MSA_CD_StartDecast(agent);
				else if (MSA_CD_RollMiscast(agent, (float) ((CalcMaxMana(agent) - item.invItemCount) / 100f)))
					MSA_CD_StartMiscast(agent, MSA_CD_RollTimescale(agent, true));
				else
					MSA_CD_StartCast(agent, MSA_CD_RollTimescale(agent, false));
			};

			chronomanticDilation.Recharge = (item, agent) =>
			{
				if (MSA_CD_IsCast(agent))
				{
					item.invItemCount -= MSA_CD_RollManaCost(agent);

					if (item.invItemCount < 0)
						MSA_CD_StartMiscast(agent, MSA_CD_RollTimescale(agent, true));
				}
				else if (item.invItemCount < CalcMaxMana(agent) && agent.statusEffects.CanRecharge())
				{
					item.invItemCount += Math.Min(CalcMaxMana(agent) - item.invItemCount, 5);

					if (item.invItemCount == CalcMaxMana(agent) && !MSA_CD_IsMiscast(agent))
						MSA_CD_StartRecharge(agent, true);
				}
			};
			chronomanticDilation.RechargeInterval = (item, myAgent) =>
				item.invItemCount > 0 ? new WaitForSeconds(1f) : null;
		}

		public static int MSA_CD_RollManaCost(Agent agent)
		{
			int manaCost = 15;

			if (agent.statusEffects.hasTrait(cTrait.FocusedCasting))
				manaCost -= 2;
			else if (agent.statusEffects.hasTrait(cTrait.FocusedCasting_2))
				manaCost -= 4;
			else if (agent.statusEffects.hasTrait(cTrait.WildCasting))
				manaCost += UnityEngine.Random.Range(-2, 2);
			else if (agent.statusEffects.hasTrait(cTrait.WildCasting_2))
				manaCost = UnityEngine.Random.Range(-5, 5);
			else if (agent.statusEffects.hasTrait(cTrait.Archmage))
				manaCost = 0;

			return manaCost;
		}

		public static bool MSA_CD_RollMiscast(Agent agent, float percentMod)
		{
			float risk = 1.0f + percentMod;

			if (agent.statusEffects.hasTrait(cTrait.Archmage))
				return false;
			else if (agent.statusEffects.hasTrait(cTrait.FocusedCasting))
				risk -= 0.25f;
			else if (agent.statusEffects.hasTrait(cTrait.FocusedCasting_2))
				risk -= 0.50f;
			else if (agent.statusEffects.hasTrait(cTrait.WildCasting))
				risk += 0.75f;
			else if (agent.statusEffects.hasTrait(cTrait.WildCasting_2))
				risk += 1.50f;

			if (agent.statusEffects.hasTrait(cTrait.MagicTraining))
				risk *= (4 / 5);
			else if (agent.statusEffects.hasTrait(cTrait.MagicTraining_2))
				risk *= (3 / 5);

			return (UnityEngine.Random.Range(0f, 100f) <= risk);
		}

		public static float MSA_CD_RollTimescale(Agent agent, bool MisCast)
		{
			float timescale = 1.000f;

			if (agent.statusEffects.hasTrait(cTrait.Archmage))
				return 4.000f;

			if (!MisCast)
			{
				timescale = 2.000f;

				if (agent.statusEffects.hasTrait(cTrait.FocusedCasting))
					timescale += 0.250f;
				else if (agent.statusEffects.hasTrait(cTrait.FocusedCasting_2))
					timescale += 0.500f;
				else if (agent.statusEffects.hasTrait(cTrait.WildCasting))
					timescale += 0.750f;
				else if (agent.statusEffects.hasTrait(cTrait.WildCasting_2))
					timescale += 1.500f;

				if (agent.statusEffects.hasTrait(cTrait.MagicTraining))
					timescale += 0.500f;
				else if (agent.statusEffects.hasTrait(cTrait.MagicTraining_2))
					timescale += 1.000f;
			}
			else if (MisCast)
			{
				timescale = 0.250f;

				if (agent.statusEffects.hasTrait(cTrait.FocusedCasting))
					timescale += 0.375f;
				else if (agent.statusEffects.hasTrait(cTrait.FocusedCasting_2))
					timescale += 0.500f;
				else if (agent.statusEffects.hasTrait(cTrait.WildCasting))
					timescale -= 0.063f;
				else if (agent.statusEffects.hasTrait(cTrait.WildCasting_2))
					timescale -= 0.188f;

				if (agent.statusEffects.hasTrait(cTrait.MagicTraining))
					timescale += 0.125f;
				else if (agent.statusEffects.hasTrait(cTrait.MagicTraining_2))
					timescale += 0.250f;
			}

			BMLog("ChronomancyRollTimescale: " + timescale);

			return timescale;
		}

		public static void MSA_CD_StartCast(Agent agent, float speedupfactor)
		{
			MSA_CD_SetCast(agent, true);
			MSA_CD_DialogueCast(agent);

			GC.selectedTimeScale = baseTimeScale / speedupfactor;
			GC.mainTimeScale = baseTimeScale / speedupfactor;
			agent.speedMax = agent.FindSpeed() * (int) speedupfactor;

			BMLog("Timescale: " + GC.mainTimeScale.ToString());
		}

		public static async void MSA_CD_StartDecast(Agent agent)
		{
			agent.speedMax = agent.FindSpeed();

			MSA_CD_SetCast(agent, false); // Needs to occur before delays or Overcast occurs erroneously

			GC.selectedTimeScale = baseTimeScale;
			GC.mainTimeScale = baseTimeScale;

			MSA_CD_DialogueDecast(agent);

			await Task.Delay(1000);

			await MSA_CD_StartWindingUp(agent);

			BMLog("Timescale: " + GC.mainTimeScale.ToString());
		}

		public static async void MSA_CD_StartMiscast(Agent agent, float slowdownFactor)
		{
			BMLog("ChronomancyStartMiscast: " + slowdownFactor);

			MSA_CD_DialogueMiscast(agent);

			if (MSA_CD_IsCast(agent))
				MSA_CD_SetCast(agent, false);

			MSA_CD_SetMiscast(agent, true);

			MSA_CD_StartWindingUp(agent); // TODO: Ensure that this duration is equal to miscast duration

			GC.selectedTimeScale = baseTimeScale / slowdownFactor;
			GC.mainTimeScale = baseTimeScale / slowdownFactor;
			agent.speedMax = (int) ((float) agent.FindSpeed() * slowdownFactor);

			agent.inventory.buffDisplay.specialAbilitySlot.MakeNotUsable();

			BMLog("Timescale: " + GC.mainTimeScale.ToString());

			await Task.Delay(5000);

			MSA_CD_SetMiscast(agent, false);
			MSA_CD_StartDecast(agent);
		}

		public static void MSA_CD_StartRecharge(Agent agent, bool routine)
		{
			BMLog("ChronomancyStartRecharge");

			if (!routine)
				MSA_CD_DialogueRecharge(agent);

			if (MSA_CD_IsWindingUp(agent))
				MSA_CD_SetWindingUp(agent, false);

			if (MSA_CD_IsMiscast(agent))
			{
				//TODO: Eliminate redundancies between Recharge and DeCast

				MSA_CD_SetMiscast(agent, false);

				GC.selectedTimeScale = baseTimeScale;
				GC.mainTimeScale = baseTimeScale;
				agent.speedMax = agent.FindSpeed();

				agent.inventory.buffDisplay.specialAbilitySlot.MakeUsable();
			}

			BMLog("Timescale: " + GC.mainTimeScale.ToString());
		}

		public static async Task MSA_CD_StartWindingUp(Agent agent)
		{
			MSA_CD_SetWindingUp(agent, true);

			float duration = 4000f;

			if (agent.statusEffects.hasTrait(cTrait.Archmage))
				duration = 1000f;
			else if (agent.statusEffects.hasTrait(cTrait.WildCasting))
				duration -= 1000f;
			else if (agent.statusEffects.hasTrait(cTrait.WildCasting_2))
				duration -= 2000f;

			if (agent.statusEffects.hasTrait(cTrait.MagicTraining))
				duration -= 1000f;
			else if (agent.statusEffects.hasTrait(cTrait.MagicTraining_2))
				duration -= 2000f;

			await Task.Delay((int) duration);

			while (MSA_CD_IsMiscast(agent))
				await Task.Delay(1000);

			MSA_CD_StartRecharge(agent, true);
		}

		#endregion

		#region Pyromantic Jet

		#region Pyromantic Jet - Bits

		public static bool MSA_PJ_IsBurnedOut(Agent agent)
		{
			BMLog("PyromancyIsBurnedOut: " + ((agent.inventory.equippedSpecialAbility.otherDamage & 0b_0001) != 0));

			return (agent.inventory.equippedSpecialAbility.otherDamage & 0b_0001) != 0;
		}

		public static bool MSA_PJ_IsCoolingDown(Agent agent)
		{
			BMLog("PyromancyIsCoolingDown: " + ((agent.inventory.equippedSpecialAbility.otherDamage & 0b_0010) != 0));

			return (agent.inventory.equippedSpecialAbility.otherDamage & 0b_0010) != 0;
		}

		public static bool MSA_PJ_IsMiscast(Agent agent)
		{
			BMLog("PyromancyIsMiscast: " + ((agent.inventory.equippedSpecialAbility.otherDamage & 0b_0100) != 0));

			return (agent.inventory.equippedSpecialAbility.otherDamage & 0b_0100) != 0;
		}

		public static void MSA_PJ_LogBooleans(Agent agent)
		{
			BMLog("PyromancyIsBurnedOut: " + MSA_PJ_IsBurnedOut(agent));
			BMLog("PyromancyIsCoolingDown: " + MSA_PJ_IsCoolingDown(agent));
			BMLog("PyromancyIsMiscast: " + MSA_PJ_IsMiscast(agent));
		}

		public static void MSA_PJ_SetBurnedOut(Agent agent, bool value)
		{
			BMLog("PyromancySetBurnedOut " + value);

			if (value) agent.inventory.equippedSpecialAbility.otherDamage |= 0b_0001;
			else agent.inventory.equippedSpecialAbility.otherDamage &= ~0b_0001;
		}

		public static void MSA_PJ_SetCoolingDown(Agent agent, bool value)
		{
			BMLog("PyromancySetCoolingDown " + value);

			if (value) agent.inventory.equippedSpecialAbility.otherDamage |= 0b_0010;
			else agent.inventory.equippedSpecialAbility.otherDamage &= ~0b_0010;
		}

		public static void MSA_PJ_SetMiscast(Agent agent, bool value)
		{
			BMLog("PyromancySetMiscast " + value);

			if (value) agent.inventory.equippedSpecialAbility.otherDamage |= 0b_0100;
			else agent.inventory.equippedSpecialAbility.otherDamage &= ~0b_0100;
		}

		#endregion

		#region Pyromantic Jet - Dialogue

		public static void MSA_PJ_DialogueCantDo(Agent agent)
		{
			GC.audioHandler.Play(agent, "CantDo");

			if (GC.percentChance(75))
				return;

			List<string> dialogue = new List<string>()
			{
				cDialogue.MSA_PJ_CantDo1,
				cDialogue.MSA_PJ_CantDo2,
				cDialogue.MSA_PJ_CantDo3,
				cDialogue.MSA_PJ_CantDo4,
			};

			BMHeaderTools.SayDialogue(agent, BMHeaderTools.RandomFromList(dialogue), vNameType.Dialogue);
		}

		public static void MSA_PJ_DialogueCast(Agent agent)
		{
			if (GC.percentChance(99))
				return;

			GC.audioHandler.Play(agent, vAudioClip.AgentLaugh);

			List<string> dialogue = new List<string>()
			{
				cDialogue.MSA_PJ_Cast1,
				cDialogue.MSA_PJ_Cast2,
				cDialogue.MSA_PJ_Cast3,
				cDialogue.MSA_PJ_Cast4,
			};

			BMHeaderTools.SayDialogue(agent, BMHeaderTools.RandomFromList(dialogue), vNameType.Dialogue);
		}

		public static void MSA_PJ_DialogueMiscast(Agent agent)
		{
			if (GC.percentChance(75))
				return;

			List<string> dialogue = new List<string>()
			{
				cDialogue.MSA_PJ_Miscast1,
				cDialogue.MSA_PJ_Miscast2,
				cDialogue.MSA_PJ_Miscast3,
				cDialogue.MSA_PJ_Miscast4,
			};

			BMHeaderTools.SayDialogue(agent, BMHeaderTools.RandomFromList(dialogue), vNameType.Dialogue);
		}

		public static void MSA_PJ_DialogueRecharge(Agent agent)
		{
			agent.statusEffects.CreateBuffText("Recharged", agent.objectNetID);
			GC.audioHandler.Play(agent, "Recharge");

			if (GC.percentChance(75))
				return;

			List<string> dialogue = new List<string>()
			{
				cDialogue.MSA_PJ_Recharge1,
				cDialogue.MSA_PJ_Recharge2,
				cDialogue.MSA_PJ_Recharge3,
				cDialogue.MSA_PJ_Recharge4,
			};

			BMHeaderTools.SayDialogue(agent, BMHeaderTools.RandomFromList(dialogue), vNameType.Dialogue);
		}

		#endregion

		public static void MSA_PJ_Initialize()
		{
			Sprite spritePyromanticJet = RogueUtilities.ConvertToSprite(Properties.Resources.PyromanticJet);

			CustomAbility pyromanticJet = RogueLibs.CreateCustomAbility(cSpecialAbility.PyromanticJet, spritePyromanticJet, true,
				new CustomNameInfo("Pyromantic Jet", "", "", "", "", "Огненная струя", "", ""),
				new CustomNameInfo("Cast: Hold to shoot flames.\nMiscast: Excrete Napalm from all your pores. What did you eat??", "", "", "", "",
					"Заклинание: Удерживайте кнопку Супер-способности, чтобы выстрелить пламенем.\nНеудача: Из вас течёт Напалм.. что вы ели??", "", ""),
				delegate(InvItem item)
				{
					item.cantDrop = true;
					item.Categories.Add("NPCsCantPickUp");
					item.dontAutomaticallySelect = true;
					item.dontSelectNPC = true;
					item.gunKnockback = 0;
					item.rapidFire = true;
					item.initCount = 100;
					item.itemType = "WeaponProjectile";
					item.rapidFire = true;
					item.rechargeAmountInverse = 100;
					item.shadowOffset = 2;
					item.stackable = true;
					item.thiefCantSteal = true;
				});

			pyromanticJet.Available = true;
			pyromanticJet.AvailableInCharacterCreation = true;
			pyromanticJet.CostInCharacterCreation = 6;

			int pjTimeHeld = 0;

			pyromanticJet.OnHeld = delegate(InvItem item, Agent agent, ref float time)
			{
				pjTimeHeld++;

				if (pjTimeHeld >= MSA_PJ_RollFireRate(agent) && !MSA_PJ_IsBurnedOut(agent) && !MSA_PJ_IsCoolingDown(agent) && !MSA_PJ_IsMiscast(agent))
				{
					pjTimeHeld = 0;

					if (MSA_PJ_RollMiscast(agent, 0))
						MSA_PJ_StartMiscast(agent, 20);
					else
					{
						MSA_PJ_StartCast(agent);

						item.invItemCount -= MSA_PJ_RollManaCost(agent);

						if (item.invItemCount <= 0)
							MSA_PJ_StartBurnout(agent);
					}
				}
				else
					MSA_PJ_DialogueCantDo(agent);
			};

			pyromanticJet.OnReleased = delegate(InvItem item, Agent agent)
			{
				if (pjTimeHeld > 0 && !MSA_PJ_IsBurnedOut(agent) && !MSA_PJ_IsCoolingDown(agent) && !MSA_PJ_IsMiscast(agent))
					MSA_PJ_StartCoolingDown(agent);
			};

			pyromanticJet.Recharge = (item, agent) =>
			{
				if (item.invItemCount < CalcMaxMana(agent) && agent.statusEffects.CanRecharge())
				{
					item.invItemCount++;

					if (item.invItemCount == CalcMaxMana(agent))
						MSA_PJ_CompleteRecharge(agent, true);
				}
			};

			pyromanticJet.RechargeInterval = (item, myAgent) =>
				item.invItemCount > 0 ? new WaitForSeconds(0.1f) : null;
		}

		public static float MSA_PJ_RollFireRate(Agent agent)
		{
			float divisor = 5.000f;

			StatusEffects se = agent.statusEffects;

			if (se.hasTrait(cTrait.FocusedCasting))
				divisor -= 0.250f;
			else if (se.hasTrait(cTrait.FocusedCasting_2))
				divisor -= 0.500f;
			else if (se.hasTrait(cTrait.WildCasting))
				divisor -= 0.750f;
			else if (se.hasTrait(cTrait.WildCasting_2))
				divisor -= 1.500f;

			if (se.hasTrait(cTrait.MagicTraining))
				divisor -= 0.250f;
			else if (se.hasTrait(cTrait.MagicTraining_2))
				divisor -= 0.500f;

			return divisor;
		}

		public static int MSA_PJ_RollManaCost(Agent agent)
		{
			if (agent.statusEffects.hasTrait(cTrait.Archmage))
				return 0;

			float min = 3.000f;
			float max = 5.000f;

			if (agent.statusEffects.hasTrait(cTrait.FocusedCasting))
				max -= 0.500f;
			else if (agent.statusEffects.hasTrait(cTrait.FocusedCasting_2))
				max -= 1.000f;
			else if (agent.statusEffects.hasTrait(cTrait.WildCasting))
			{
				min -= 1.500f;
				max += 1.500f;
			}
			else if (agent.statusEffects.hasTrait(cTrait.WildCasting_2))
			{
				min -= 3.000f;
				max += 3.000f;
			}

			if (agent.statusEffects.hasTrait(cTrait.MagicTraining))
			{
				min -= 0.250f;
				max -= 0.250f;
			}
			else if (agent.statusEffects.hasTrait(cTrait.MagicTraining_2))
			{
				min -= 0.500f;
				max -= 0.500f;
			}

			return (int) UnityEngine.Random.Range(min, max);
		}

		public static bool MSA_PJ_RollMiscast(Agent agent, float modifier)
		{
			float risk = 1.000f + modifier;

			if (agent.statusEffects.hasTrait(cTrait.Archmage))
				return false;
			else if (agent.statusEffects.hasTrait(cTrait.FocusedCasting))
				risk -= 0.500f;
			else if (agent.statusEffects.hasTrait(cTrait.FocusedCasting_2))
				risk -= 0.750f;
			else if (agent.statusEffects.hasTrait(cTrait.WildCasting))
				risk += 0.250f;
			else if (agent.statusEffects.hasTrait(cTrait.WildCasting_2))
				risk += 0.500f;

			if (agent.statusEffects.hasTrait(cTrait.MagicTraining))
				risk *= 0.75f;
			else if (agent.statusEffects.hasTrait(cTrait.MagicTraining_2))
				risk *= 0.50f;

			return risk >= Random.Range(0f, 100f);
		}

		public static void MSA_PJ_StartBurnout(Agent agent)
		{
			if (!agent.statusEffects.hasTrait(cTrait.WildCasting) && !agent.statusEffects.hasTrait(cTrait.WildCasting_2))
			{
				GC.audioHandler.Play(agent, vAudioClip.Win);
				MSA_PJ_SetBurnedOut(agent, true);
			}

			Task.Delay(5000);

			MSA_PJ_SetBurnedOut(agent, false); // 202103301952
		}

		public static void MSA_PJ_StartCast(Agent agent)
		{
			BMLog("PyromancyStartCast");

			Bullet bullet = GC.spawnerMain.SpawnBullet(agent.gun.tr.position, bulletStatus.Fire, agent);

			if (agent.controllerType == "Keyboard" && !GC.sessionDataBig.trackpadMode)
				bullet.movement.RotateToMouseTr(agent.agentCamera.actualCamera);
			else if (agent.target.AttackTowardTarget())
				bullet.tr.rotation = Quaternion.Euler(0f, 0f, agent.target.transform.eulerAngles.z);
			else
				bullet.tr.rotation = Quaternion.Euler(0f, 0f, agent.gun.FindWeaponAngleGamepad() - 90f);

			if (GC.sessionDataBig.autoAim != "Off")
			{
				int myChance = 25; // Placeholder, find the real numbers later. For now, suck it, Auto-aimers B)
				if (GC.percentChance(myChance))
					bullet.movement.AutoAim(agent, agent.movement.FindAimTarget(true), bullet);
			}

			bullet.speed = 6;

			if (agent.statusEffects.hasTrait(cTrait.MagicTraining))
				bullet.speed += 1;
			else if (agent.statusEffects.hasTrait(cTrait.MagicTraining_2))
				bullet.speed += 2;

			if (agent.statusEffects.hasTrait(cTrait.WildCasting))
				bullet.speed += 3;
			else if (agent.statusEffects.hasTrait(cTrait.WildCasting_2) || agent.statusEffects.hasTrait(cTrait.Archmage))
				bullet.speed += 6;

			MSA_PJ_DialogueCast(agent);
		}

		public static async Task MSA_PJ_StartCoolingDown(Agent agent)
		{
			BMLog("PyromancyStartCoolingDown");

			if (MSA_PJ_IsCoolingDown(agent) == false)
			{
				MSA_PJ_SetCoolingDown(agent, true);

				float duration = 2000f;

				if (agent.statusEffects.hasTrait(cTrait.MagicTraining))
					duration -= 500f;
				else if (agent.statusEffects.hasTrait(cTrait.MagicTraining_2))
					duration -= 1000f;

				if (agent.statusEffects.hasTrait(cTrait.WildCasting))
					duration -= 1250f;
				else if (agent.statusEffects.hasTrait(cTrait.WildCasting_2))
					duration -= 2000f;

				if (agent.statusEffects.hasTrait(cTrait.Archmage))
					duration = 0f;

				duration = Mathf.Max(0f, duration);

				await Task.Delay((int) duration);

				MSA_PJ_SetCoolingDown(agent, false);
			}
		}

		public static void MSA_PJ_StartMiscast(Agent agent, int degree)
		{
			BMLog("PyromancyStartMiscast");

			MSA_PJ_DialogueMiscast(agent);

			GC.spawnerMain.SpawnExplosion(agent, agent.curPosition, "FireBomb");

			MSA_PJ_StartBurnout(agent);
		}

		public static void MSA_PJ_CompleteRecharge(Agent agent, bool routine)
		{
			BMLog("PyromancyStartRecharge");

			if (!routine)
				MSA_PJ_DialogueRecharge(agent);

			if (MSA_PJ_IsBurnedOut(agent))
				MSA_PJ_SetBurnedOut(agent, false);

			if (MSA_PJ_IsMiscast(agent))
				MSA_PJ_SetMiscast(agent, false);

			agent.inventory.buffDisplay.specialAbilitySlot.MakeUsable();
		}

		#endregion

		#region Telemantic Blink

		#region Telemantic Blink - Bits

		public static bool MSA_TB_IsReturning(Agent agent)
		{
			//BunnyHeader.Log("TelemancyIsReturning: " + ((agent.inventory.equippedSpecialAbility.otherDamage & 0b_0001) != 0));

			return (agent.inventory.equippedSpecialAbility.otherDamage & 0b_0001) != 0;
		}

		public static bool MSA_TB_IsMiscast(Agent agent)
		{
			//BunnyHeader.Log("TelemancyIsMiscast: " + ((agent.inventory.equippedSpecialAbility.otherDamage & 0b_0010) != 0));

			return (agent.inventory.equippedSpecialAbility.otherDamage & 0b_0010) != 0;
		}

		public static void MSA_TB_LogBits(Agent agent)
		{
			BMLog("TelemancyIsMiscast: " + MSA_TB_IsMiscast(agent));
			BMLog("TelemancyIsReturning: " + MSA_TB_IsReturning(agent));
		}

		public static void MSA_TB_SetReturning(Agent agent, bool value)
		{
			BMLog("TelemancySetReturning: " + value);

			if (value) agent.inventory.equippedSpecialAbility.otherDamage |= 0b_0001;
			else agent.inventory.equippedSpecialAbility.otherDamage &= ~0b_0001;
		}

		public static void MSA_TB_SetMiscast(Agent agent, bool value)
		{
			BMLog("TelemancySetMiscast: " + value);

			if (value) agent.inventory.equippedSpecialAbility.otherDamage |= 0b_0010;
			else agent.inventory.equippedSpecialAbility.otherDamage &= ~0b_0010;
		}

		#endregion

		#region Telemantic Blink - Dialogue

		public static void MSA_TB_DialogueCantDo(Agent agent)
		{
			GC.audioHandler.Play(agent, "CantDo");

			if (GC.percentChance(75))
				return;

			List<string> dialogue = new List<string>()
			{
				cDialogue.MSA_TB_CantDo1,
				cDialogue.MSA_TB_CantDo2,
				cDialogue.MSA_TB_CantDo3,
				cDialogue.MSA_TB_CantDo4,
			};

			BMHeaderTools.SayDialogue(agent, BMHeaderTools.RandomFromList(dialogue), vNameType.Dialogue);
		}

		public static void MSA_TB_DialogueCast(Agent agent)
		{
			agent.SpawnParticleEffect("Spawn", agent.curPosition);
			GameController.gameController.audioHandler.Play(agent, "Spawn");

			if (GC.percentChance(75))
				return;

			List<string> dialogue = new List<string>()
			{
				cDialogue.MSA_TB_Cast1,
				cDialogue.MSA_TB_Cast2,
				cDialogue.MSA_TB_Cast3,
				cDialogue.MSA_TB_Cast4,
			};

			BMHeaderTools.SayDialogue(agent, BMHeaderTools.RandomFromList(dialogue), vNameType.Dialogue);
		}

		public static void MSA_TB_DialogueFullyCharged(Agent agent) // TODO
		{
			agent.SpawnParticleEffect("ExplosionEMP", agent.curPosition);
			GC.audioHandler.Play(agent, "Hypnotize");
		}

		public static void MSA_TB_DialogueMiscast(Agent agent)
		{
			GC.audioHandler.Play(agent, "ZombieSpitFire");

			if (GC.percentChance(75))
				return;

			List<string> dialogue = new List<string>()
			{
				cDialogue.MSA_TB_Miscast1,
				cDialogue.MSA_TB_Miscast2,
				cDialogue.MSA_TB_Miscast3,
				cDialogue.MSA_TB_Miscast4,
			};

			BMHeaderTools.SayDialogue(agent, BMHeaderTools.RandomFromList(dialogue), vNameType.Dialogue);
		}

		public static void MSA_TB_DialogueRecharge(Agent agent)
		{
			agent.statusEffects.CreateBuffText("Recharged", agent.objectNetID);
			GC.audioHandler.Play(agent, "Recharge");

			if (GC.percentChance(75))
				return;

			List<string> dialogue = new List<string>()
			{
				cDialogue.MSA_TB_Recharge1,
				cDialogue.MSA_TB_Recharge2,
				cDialogue.MSA_TB_Recharge3,
				cDialogue.MSA_TB_Recharge4,
			};

			BMHeaderTools.SayDialogue(agent, BMHeaderTools.RandomFromList(dialogue), vNameType.Dialogue);
		}

		#endregion

		public static void MSA_TB_Initialize()
		{
			Sprite spriteTelemanticBlink = RogueUtilities.ConvertToSprite(Properties.Resources.TelemanticBlink);

			CustomAbility telemanticBlink = RogueLibs.CreateCustomAbility(cSpecialAbility.TelemanticBlink, spriteTelemanticBlink, true,
				new CustomNameInfo("Telemantic Blink", "", "", "", "", "Телепортация", "", ""),
				new CustomNameInfo(
					"Cast: Hold to charge, point cursor at destination. Charge increases accuracy but also miscast chance.\nMiscast: Have a small stroke.", "",
					"", "", "",
					"Заклинание: Удерживайте кнопку Супер-способности, наведите курсор на место в которое хотите переместится. Удерживание кнопки Супер-способности повышает точность, но и шанс на неудачный каст.\nНеудача: Небольшой удар.",
					"", ""),
				delegate(InvItem item)
				{
					item.cantDrop = true;
					item.Categories.Add("NPCsCantPickup");
					item.dontAutomaticallySelect = true;
					item.dontSelectNPC = true;
					item.isWeapon = false;
					item.initCount = 100;
					item.rechargeAmountInverse = 100;
					item.maxAmmo = 100;
					item.stackable = true;
					item.thiefCantSteal = true;
				});

			telemanticBlink.Conflicting.AddRange(new string[] { "CantTeleport" });

			telemanticBlink.Available = true;
			telemanticBlink.AvailableInCharacterCreation = true;
			telemanticBlink.CostInCharacterCreation = 8;

			int tbHeldCounter = 0; // Seconds ability held to charge
			int tbNetCharge = 0; // Net total of post-ability charge level
			bool tbFullChargeIndicated = false; // Whether particle effect showing full charge is active.

			telemanticBlink.OnPressed = delegate(InvItem item, Agent agent)
			{
				if (!MSA_TB_IsMiscast(agent) && !MSA_TB_IsReturning(agent) && item.invItemCount > 0)
					agent.SpawnParticleEffect("ExplosionMindControl", agent.curPosition);
				else
					MSA_TB_DialogueCantDo(agent);
			};

			telemanticBlink.OnHeld = delegate(InvItem item, Agent agent, ref float timeHeld)
			{
				if (!MSA_TB_IsMiscast(agent) && !MSA_TB_IsReturning(agent) && item.invItemCount > 0 && tbNetCharge < 100)
				{
					if (timeHeld * MSA_TB_RollChargeRate(agent) >= tbHeldCounter)
					{
						tbHeldCounter++;

						BMLog("Telemancy OnHeld: HeldCounter = " + tbHeldCounter + "; timeHeld = " + timeHeld);

						int manaCost = Mathf.Min(MSA_TB_RollManaCost(agent), 100 - tbNetCharge);

						item.invItemCount -= manaCost;
						tbNetCharge += manaCost;

						if (tbNetCharge == 100)
							MSA_TB_DialogueFullyCharged(agent);
					}
				}
				else if (tbNetCharge == 100 & !tbFullChargeIndicated)
				{
					agent.SpawnParticleEffect("ExplosionEMP", agent.curPosition);
					tbFullChargeIndicated = true;
				}
			};

			telemanticBlink.OnReleased = delegate(InvItem item, Agent agent)
			{
				if (tbNetCharge > 0 && !MSA_TB_IsReturning(agent) && !MSA_TB_TryMiscast(agent, tbNetCharge))
				{
					MSA_TB_StartCast(agent, Math.Max(100, tbNetCharge));
					MSA_TB_StartReturn(agent, MSA_TB_RollReturnDuration(agent));
					tbHeldCounter = 0;
					tbNetCharge = 0;
				}

				tbFullChargeIndicated = false;
			};

			telemanticBlink.Recharge = (item, agent) =>
			{
				if (tbNetCharge == 0 && item.invItemCount < CalcMaxMana(agent) && agent.statusEffects.CanRecharge())
				{
					item.invItemCount = Math.Min(CalcMaxMana(agent), item.invItemCount + MSA_TB_RollRechargeRate(agent));

					if (item.invItemCount == CalcMaxMana(agent))
						MSA_TB_StartRecharge(agent, true);
				}
			};

			telemanticBlink.RechargeInterval = (item, myAgent) =>
				item.invItemCount > 0 ? new WaitForSeconds(1f) : null;
		}

		public static int MSA_TB_RollChargeRate(Agent agent)
		{
			float rate = 1.000f;

			if (agent.statusEffects.hasTrait(cTrait.Archmage))
				return 10;
			else if (agent.statusEffects.hasTrait(cTrait.MagicTraining))
				rate *= 1.250f;
			else if (agent.statusEffects.hasTrait(cTrait.MagicTraining_2))
				rate *= 1.500f;

			if (agent.statusEffects.hasTrait(cTrait.FocusedCasting))
				rate *= 1.250f;
			else if (agent.statusEffects.hasTrait(cTrait.FocusedCasting_2))
				rate *= 1.500f;
			else if (agent.statusEffects.hasTrait(cTrait.WildCasting))
				rate *= 2.000f;
			else if (agent.statusEffects.hasTrait(cTrait.WildCasting_2))
				rate *= 3.000f;

			return (int) rate;
		}

		public static Vector2 MSA_TB_RollDestination(Agent agent, float minError, float maxError)
		{
			TileInfo tileInfo = GC.tileInfo;
			Vector2 currentPosition = agent.curPosition;
			Vector2 targetPosition;

			for (int i = 0; i < 50; i++)
			{
				float distance = UnityEngine.Random.Range(minError, maxError);

				targetPosition = MouseIngamePosition() + (distance * UnityEngine.Random.insideUnitCircle.normalized);

				TileData tileData = tileInfo.GetTileData(targetPosition);

				if ((tileData.solidObject) ||
					(tileInfo.WallExist(tileData)) ||
					(tileInfo.GetWallMaterial(targetPosition.x, targetPosition.y) == wallMaterialType.Border)) // Removed Conveyor, Water, Hole
					continue;

				return targetPosition;
			}
			return currentPosition;
		}

		public static int MSA_TB_RollManaCost(Agent agent)
		{
			BMLog("TelemancyRollManaCost");

			float min = 25.000f;
			float max = 33.000f;

			if (agent.statusEffects.hasTrait(cTrait.Archmage))
				return 0;
			else if (agent.statusEffects.hasTrait(cTrait.MagicTraining))
			{
				min *= 0.875f;
				max *= 0.875f;
			}
			else if (agent.statusEffects.hasTrait(cTrait.MagicTraining_2))
			{
				min *= 0.750f;
				max *= 0.750f;
			}

			if (agent.statusEffects.hasTrait(cTrait.FocusedCasting))
			{
				min *= 0.750f;
				max *= 0.750f;
			}
			else if (agent.statusEffects.hasTrait(cTrait.FocusedCasting_2))
			{
				min *= 0.500f;
				max *= 0.500f;
			}
			else if (agent.statusEffects.hasTrait(cTrait.WildCasting))
			{
				min *= 0.500f;
				max *= 1.250f;
			}
			else if (agent.statusEffects.hasTrait(cTrait.WildCasting_2))
			{
				min *= 0.250f;
				max *= 1.500f;
			}

			return (int) UnityEngine.Random.Range(min, max);
		}

		public static int MSA_TB_RollRechargeRate(Agent agent)
		{
			float min = 5.00f;
			float max = 10.00f;

			if (agent.statusEffects.hasTrait(cTrait.Archmage))
				return 1000;
			if (agent.statusEffects.hasTrait(cTrait.MagicTraining))
			{
				min *= 1.500f;
				max *= 1.500f;
			}
			else if (agent.statusEffects.hasTrait(cTrait.MagicTraining_2))
			{
				min *= 2.250f;
				max *= 2.250f;
			}

			if (agent.statusEffects.hasTrait(cTrait.FocusedCasting))
			{
				min *= 2.000f;
				max *= 1.250f;
			}
			else if (agent.statusEffects.hasTrait(cTrait.FocusedCasting_2))
			{
				min *= 3.000f;
				max *= 1.750f;
			}
			else if (agent.statusEffects.hasTrait(cTrait.WildCasting))
			{
				min *= 0.500f;
				max *= 3.000f;
			}
			else if (agent.statusEffects.hasTrait(cTrait.WildCasting_2))
			{
				min *= 0.250f;
				max *= 4.000f;
			}

			return (int) UnityEngine.Random.Range(min, max);
		}

		public static int MSA_TB_RollReturnDuration(Agent agent)
		{
			float duration = 4000f;

			if (agent.statusEffects.hasTrait(cTrait.Archmage))
				return 0;
			else if (agent.statusEffects.hasTrait(cTrait.MagicTraining))
				duration *= 0.500f;
			else if (agent.statusEffects.hasTrait(cTrait.MagicTraining_2))
				duration *= 0.250f;

			if (agent.statusEffects.hasTrait(cTrait.FocusedCasting))
				duration *= 0.875f;
			else if (agent.statusEffects.hasTrait(cTrait.FocusedCasting_2))
				duration *= 0.750f;
			else if (agent.statusEffects.hasTrait(cTrait.WildCasting))
				duration *= 0.250f;
			else if (agent.statusEffects.hasTrait(cTrait.WildCasting_2))
				duration *= 0.000f;

			return (int) duration;
		}

		public static void MSA_TB_StartCast(Agent agent, float charge)
		{
			float maxError = 200.000f;

			if (agent.statusEffects.hasTrait(cTrait.FocusedCasting))
				maxError -= 25.000f;
			else if (agent.statusEffects.hasTrait(cTrait.FocusedCasting_2))
				maxError -= 50.000f;
			else if (agent.statusEffects.hasTrait(cTrait.WildCasting))
				maxError += 25.000f;
			else if (agent.statusEffects.hasTrait(cTrait.WildCasting_2))
				maxError += 50.000f;

			maxError = (maxError - charge) / 20f;

			agent.Teleport(MSA_TB_RollDestination(agent, 0, maxError), false, true);
			agent.rb.velocity = Vector2.zero;

			MSA_TB_DialogueCast(agent);
		}

		public static void MSA_TB_StartRecharge(Agent agent, bool routine)
		{
			if (true) // Not checking for routine/nonroutine yet.
				MSA_TB_DialogueRecharge(agent);

			agent.inventory.buffDisplay.specialAbilitySlot.MakeUsable();
		}

		public static async void MSA_TB_StartReturn(Agent agent, int mSecs)
		{
			MSA_TB_SetReturning(agent, true);

			await Task.Delay(mSecs);

			MSA_TB_SetReturning(agent, false);
		}

		public static bool MSA_TB_TryMiscast(Agent agent, float netCharge)
		{
			if (agent.statusEffects.hasTrait(cTrait.Archmage))
				return false;
			else if (agent.statusEffects.hasTrait(cTrait.MagicTraining))
				netCharge *= 0.750f;
			else if (agent.statusEffects.hasTrait(cTrait.MagicTraining_2))
				netCharge *= 0.500f;

			if (agent.statusEffects.hasTrait(cTrait.FocusedCasting))
				netCharge *= 0.500f;
			else if (agent.statusEffects.hasTrait(cTrait.FocusedCasting_2))
				netCharge *= 0.250f;
			else if (agent.statusEffects.hasTrait(cTrait.WildCasting))
				netCharge *= 1.500f;
			else if (agent.statusEffects.hasTrait(cTrait.WildCasting_2))
				netCharge *= 2.000f;

			netCharge -= UnityEngine.Random.Range(0f, 100f);
			netCharge /= 25;

			int severity = (int) Mathf.Clamp(netCharge, 0, 4);

			if (severity == 0)
				return false;

			bool failTeleport = false;
			bool lightAndSound = false;
			bool randomTeleport = false;
			bool loseItem = false;
			bool confusion = false;
			bool dizziness = false;

			if (severity == 1)
			{
				lightAndSound = true;
				confusion = true;
			}
			else if (severity == 2)
			{
				failTeleport = true;
				randomTeleport = true;
			}
			else if (severity == 3)
			{
				loseItem = true;
				dizziness = true;
			}
			else if (severity >= 4)
			{
				failTeleport = true;
				loseItem = true;
				lightAndSound = true;
				randomTeleport = true;
			}

			BMLog("TelemancyTryMiscast: stroke");
			agent.statusEffects.ChangeHealth(-severity * 5);
			MSA_TB_DialogueMiscast(agent);

			if (randomTeleport)
			{
				BMLog("TelemancyTryMiscast: randomTeleport");

				agent.statusEffects.UseQuickEscapeTeleporter(false);
				failTeleport = true;
			}

			if (lightAndSound)
			{
				BMLog("TelemancyTryMiscast: lightAndSound");

				GC.spawnerMain.SpawnNoise(agent.curPosition, 1f, null, null, agent);
				agent.SpawnParticleEffect("ExplosionEMP", agent.tr.position);
			}

			if (confusion)
				agent.statusEffects.AddStatusEffect("Confused", severity * 5);

			if (dizziness)
				agent.statusEffects.AddStatusEffect("Dizzy", severity * 3);

			if (loseItem)
			{
				BMLog("TelemancyTryMiscast: LoseItem");

				// To pick random item: compare from NPC Thief stealing? That would exclude quest items and your money supply, at least.

				// TODO
				// A Shortcut might be telling the game that the item has fallen into a hole. It would have the AV effects already set up. But might not be that simple.
			}

			agent.inventory.buffDisplay.specialAbilitySlot.MakeNotUsable();

			if (!failTeleport)
				return false;

			return true;
		}

		#endregion

		#region Agent

		public void Agent_00()
		{
			Postfix(typeof(Agent), "FindOperatingTime", GetType(), "Agent_FindOperatingTime", new Type[2] { typeof(PlayfieldObject), typeof(float) });
		}

		public static void Agent_FindOperatingTime(PlayfieldObject operatingOnObject, float timeToUnlock, Agent __instance, ref float __result) // Postfix
		{
			__result *= GC.selectedTimeScale;
		}

		#endregion

		#region StatusEffects

		public void StatusEffects_00()
		{
			Postfix(typeof(StatusEffects), "GiveSpecialAbility", GetType(), "StatusEffects_GiveSpecialAbility", new Type[1] { typeof(String) });
		}

		public static void StatusEffects_GiveSpecialAbility(string abilityName, StatusEffects __instance) // Postfix
		{
			if (__instance.agent.inventory.equippedSpecialAbility != null)
			{
				InvItem ability = __instance.agent.inventory.equippedSpecialAbility;
				Agent agent = __instance.agent;

				string[] magicAbilities =
				{
					cSpecialAbility.ChronomanticDilation,
					cSpecialAbility.PyromanticJet,
					cSpecialAbility.TelemanticBlink
				};

				if (magicAbilities.Contains(abilityName))
				{
					ability.otherDamage = 0; // Bitwise variables

					ability.initCount = CalcMaxMana(agent);
					ability.maxAmmo = CalcMaxMana(agent);
					ability.rechargeAmountInverse = CalcMaxMana(agent);
				}
			}
		}

		#endregion
	}
}