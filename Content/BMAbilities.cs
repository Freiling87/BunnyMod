using RogueLibsCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace BunnyMod.Content
{
	public class BMAbilities
	{
		public static GameController GC => GameController.gameController;
		public static bool Prefix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) => BMHeader.MainInstance.PatchPrefix(type, methodName, patchType, patchMethodName, types);
		public static bool Postfix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) => BMHeader.MainInstance.PatchPostfix(type, methodName, patchType, patchMethodName, types);
		public static void BMLog(string logMessage) => BMHeader.Log(logMessage);

		public void Awake()
		{
			InitializeAbilities();

			// Agent
			Postfix(typeof(Agent), "FindOperatingTime", GetType(), "Agent_FindOperatingTime", new Type[2] { typeof(PlayfieldObject), typeof(float) });

			// LoadLevel
			Postfix(typeof(LoadLevel), "SetupMore5_2", GetType(), "LoadLevel_SetupMore5_2", new Type[0] { });

			// StatusEffects
			Postfix(typeof(StatusEffects), "GiveSpecialAbility", GetType(), "StatusEffects_GiveSpecialAbility", new Type[1] { typeof(String) });
		}
		public static void InitializeAbilities()
		{
			MSA_CD_Initialize();
			MSA_PJ_Initialize();
			MSA_TB_Initialize();
		}

		#region Magic General
		public static int CalcMaxMana(Agent agent)
		{
			if (agent.statusEffects.hasTrait(cTraits.ManaBattery))
				return 150;
			else if (agent.statusEffects.hasTrait(cTraits.ManaBattery_2))
				return 200;
			else if (agent.statusEffects.hasTrait(cTraits.Archmage))
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
			GC.audioHandler.Play(agent, "CantDo");

			string[] dialogue =
			{
				"I need to take a \"time out!\" Get it? But seriously, my heart will stop.",
				"I'm gonna take the blue pill for a sec."
			};

			agent.Say(dialogue[UnityEngine.Random.Range(0, dialogue.Count() - 1)]);
		}
		public static void MSA_CD_DialogueCast(Agent agent) // TODO
		{
			string[] dialogue =
			{
				""
			};

			agent.Say(dialogue[UnityEngine.Random.Range(0, dialogue.Count() - 1)]);
		}
		public static void MSA_CD_DialogueMiscast(Agent agent)
		{
			string[] dialogue =
			{
				"Iii ttthhhiiinnnkkk Iii mmmeeesssssseeeddd uuuppp...",
				"Bullet Time? More like Bullshit Time!",
				"(Slow Motion Noises)",
				"Okay, maybe there is a spoon, I guess. Whatever."
			};

			agent.Say(dialogue[UnityEngine.Random.Range(0, dialogue.Count() - 1)]);
		}
		public static void MSA_CD_DialogueRecharge(Agent agent)
		{
			string[] dialogue =
			{
				"It's Slowing-down-time... Time!"
			};

			agent.Say(dialogue[UnityEngine.Random.Range(0, dialogue.Count() - 1)]);
		}
		#endregion
		public static float baseTimeScale;
		public static void MSA_CD_Initialize()
		{
			Sprite spriteChronomanticDilation = RogueUtilities.ConvertToSprite(Properties.Resources.ChronomanticDilation);

			CustomAbility chronomanticDilation = RogueLibs.CreateCustomAbility(cAbilities.ChronomanticDilation, spriteChronomanticDilation, true,
				new CustomNameInfo("Chronomantic Dilation"),
				new CustomNameInfo("Cast: Toggle to slow down everything but yourself. Toggle to deactivate.\nMiscast: Speed up everything but yourself."),
				delegate (InvItem item)
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

			chronomanticDilation.OnPressed = delegate (InvItem item, Agent agent)
			{
				if (MSA_CD_IsWindingUp(agent) || MSA_CD_IsMiscast(agent))
					MSA_CD_DialogueCantDo(agent);
				else if (MSA_CD_IsCast(agent))
					MSA_CD_StartDecast(agent);
				else if (MSA_CD_RollMiscast(agent, (float)((CalcMaxMana(agent) - item.invItemCount) / 100f)))
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

			if (agent.statusEffects.hasTrait(cTraits.FocusedCasting))
				manaCost -= 2;
			else if (agent.statusEffects.hasTrait(cTraits.FocusedCasting_2))
				manaCost -= 4;
			else if (agent.statusEffects.hasTrait(cTraits.WildCasting))
				manaCost += UnityEngine.Random.Range(-2, 2);
			else if (agent.statusEffects.hasTrait(cTraits.WildCasting_2))
				manaCost = UnityEngine.Random.Range(-5, 5);
			else if (agent.statusEffects.hasTrait(cTraits.Archmage))
				manaCost = 0;

			return manaCost;
		}
		public static bool MSA_CD_RollMiscast(Agent agent, float percentMod)
		{
			float risk = 1.0f + percentMod;

			if (agent.statusEffects.hasTrait(cTraits.Archmage))
				return false;
			else if (agent.statusEffects.hasTrait(cTraits.FocusedCasting))
				risk -= 0.25f;
			else if (agent.statusEffects.hasTrait(cTraits.FocusedCasting_2))
				risk -= 0.50f;
			else if (agent.statusEffects.hasTrait(cTraits.WildCasting))
				risk += 0.75f;
			else if (agent.statusEffects.hasTrait(cTraits.WildCasting_2))
				risk += 1.50f;

			if (agent.statusEffects.hasTrait(cTraits.MagicTraining))
				risk *= (4 / 5);
			else if (agent.statusEffects.hasTrait(cTraits.MagicTraining_2))
				risk *= (3 / 5);

			return (UnityEngine.Random.Range(0f, 100f) <= risk);
		}
		public static float MSA_CD_RollTimescale(Agent agent, bool MisCast)
		{
			float timescale = 1.000f;

			if (agent.statusEffects.hasTrait(cTraits.Archmage))
				return 4.000f;

			if (!MisCast)
			{
				timescale = 2.000f;

				if (agent.statusEffects.hasTrait(cTraits.FocusedCasting))
					timescale += 0.250f;
				else if (agent.statusEffects.hasTrait(cTraits.FocusedCasting_2))
					timescale += 0.500f;
				else if (agent.statusEffects.hasTrait(cTraits.WildCasting))
					timescale += 0.750f;
				else if (agent.statusEffects.hasTrait(cTraits.WildCasting_2))
					timescale += 1.500f;

				if (agent.statusEffects.hasTrait(cTraits.MagicTraining))
					timescale += 0.500f;
				else if (agent.statusEffects.hasTrait(cTraits.MagicTraining_2))
					timescale += 1.000f;
			}
			else if (MisCast)
			{
				timescale = 0.250f;

				if (agent.statusEffects.hasTrait(cTraits.FocusedCasting))
					timescale += 0.375f;
				else if (agent.statusEffects.hasTrait(cTraits.FocusedCasting_2))
					timescale += 0.500f;
				else if (agent.statusEffects.hasTrait(cTraits.WildCasting))
					timescale -= 0.063f;
				else if (agent.statusEffects.hasTrait(cTraits.WildCasting_2))
					timescale -= 0.188f;

				if (agent.statusEffects.hasTrait(cTraits.MagicTraining))
					timescale += 0.125f;
				else if (agent.statusEffects.hasTrait(cTraits.MagicTraining_2))
					timescale += 0.250f;
			}

			BMLog("ChronomancyRollTimescale: " + timescale);

			return timescale;
		}
		public static void MSA_CD_StartCast(Agent agent, float speedupfactor)
		{
			agent.SpawnParticleEffect("ExplosionMindControl", agent.curPosition);
			GameController.gameController.audioHandler.Play(agent, "MakeOffering");

			MSA_CD_SetCast(agent, true);

			GC.selectedTimeScale = baseTimeScale / speedupfactor;
			GC.mainTimeScale = baseTimeScale / speedupfactor;
			agent.speedMax = agent.FindSpeed() * (int)speedupfactor;

			BMLog("Timescale: " + GC.mainTimeScale.ToString());
		}
		public static async void MSA_CD_StartDecast(Agent agent)
		{
			GameController.gameController.audioHandler.Play(agent, "MakeOffering");

			agent.speedMax = agent.FindSpeed();

			MSA_CD_SetCast(agent, false); // Needs to occur before delays or Overcast occurs erroneously

			GC.selectedTimeScale = baseTimeScale;
			GC.mainTimeScale = baseTimeScale;

			await Task.Delay(1000);

			await MSA_CD_StartWindingUp(agent);

			BMLog("Timescale: " + GC.mainTimeScale.ToString());
		}
		public static async void MSA_CD_StartMiscast(Agent agent, float slowdownFactor)
		{
			BMLog("ChronomancyStartMiscast: " + slowdownFactor);

			agent.SpawnParticleEffect("ExplosionEMP", agent.curPosition);
			GC.audioHandler.Play(agent, "ToiletTeleportIn");
			MSA_CD_DialogueMiscast(agent);

			if (MSA_CD_IsCast(agent))
				MSA_CD_SetCast(agent, false);

			MSA_CD_SetMiscast(agent, true);

			MSA_CD_StartWindingUp(agent); // TODO: Ensure that this duration is equal to miscast duration

			GC.selectedTimeScale = baseTimeScale / slowdownFactor;
			GC.mainTimeScale = baseTimeScale / slowdownFactor;
			agent.speedMax = (int)((float)agent.FindSpeed() * slowdownFactor);

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
			{
				agent.statusEffects.CreateBuffText("Recharged", agent.objectNetID);
				GC.audioHandler.Play(agent, "Recharge");

				MSA_CD_DialogueRecharge(agent);
			}

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

			if (agent.statusEffects.hasTrait(cTraits.Archmage))
				duration = 1000f;
			else if (agent.statusEffects.hasTrait(cTraits.WildCasting))
				duration -= 1000f;
			else if (agent.statusEffects.hasTrait(cTraits.WildCasting_2))
				duration -= 2000f;

			if (agent.statusEffects.hasTrait(cTraits.MagicTraining))
				duration -= 1000f;
			else if (agent.statusEffects.hasTrait(cTraits.MagicTraining_2))
				duration -= 2000f;

			await Task.Delay((int)duration);

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

			string[] dialogue =
			{
				"I'm burned out.",
				"Hang on. I don't feel like exploding right now."
			};

			agent.Say(dialogue[UnityEngine.Random.Range(0, dialogue.Count() - 1)]);
		}
		public static void MSA_PJ_DialogueCast(Agent agent) // Not used yet
		{
			GC.audioHandler.Play(agent, "AgentLaugh");

			string[] dialogue =
			{
				"Die! Burn! Die! Die!",
				"Burn, baby, burn!",
				"BURN-ie 2024!",
				"Yer fired! Get it?"
			};

			agent.Say(dialogue[UnityEngine.Random.Range(0, dialogue.Count() - 1)]);
		}
		public static void MSA_PJ_DialogueMiscast(Agent agent)
		{
			string[] dialogue =
			{
				"Not very stoked right now.",
				"Haha my skin is melting lol XDDD",
				"Flame off! Flame off!",
				"I shidded an farded an bursteded into flames."
			};

			agent.Say(dialogue[UnityEngine.Random.Range(0, dialogue.Count() - 1)]);
		}
		public static void MSA_PJ_DialogueRecharge(Agent agent)
		{
			string[] dialogue =
			{
				"Ready to burn!",
				"I'm here to burn things and chew bubblegum. I'm not out of gum, but I'm still gonna do both.",
				"(Laughs maniacally)",
				"Why are the innocent so fun to burn?"
			};

			agent.Say(dialogue[UnityEngine.Random.Range(0, dialogue.Count() - 1)]);
		}
		#endregion
		public static void MSA_PJ_Initialize()
		{
			Sprite spritePyromanticJet = RogueUtilities.ConvertToSprite(Properties.Resources.PyromanticJet);

			CustomAbility pyromanticJet = RogueLibs.CreateCustomAbility(cAbilities.PyromanticJet, spritePyromanticJet, true,
				new CustomNameInfo("Pyromantic Jet"),
				new CustomNameInfo("Cast: Hold to shoot flames.\nMiscast: Excrete Napalm from all your pores. What did you eat?? "),
				delegate (InvItem item)
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
			pyromanticJet.CostInCharacterCreation = 8;

			int pjTimeHeld = 0;

			pyromanticJet.OnHeld = delegate (InvItem item, Agent agent, ref float time)
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
			};

			pyromanticJet.OnReleased = delegate (InvItem item, Agent agent)
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

			if (se.hasTrait(cTraits.FocusedCasting))
				divisor -= 0.250f;
			else if (se.hasTrait(cTraits.FocusedCasting_2))
				divisor -= 0.500f;
			else if (se.hasTrait(cTraits.WildCasting))
				divisor -= 0.750f;
			else if (se.hasTrait(cTraits.WildCasting_2))
				divisor -= 1.500f;

			if (se.hasTrait(cTraits.MagicTraining))
				divisor -= 0.250f;
			else if (se.hasTrait(cTraits.MagicTraining_2))
				divisor -= 0.500f;

			return divisor;
		}
		public static int MSA_PJ_RollManaCost(Agent agent)
		{
			if (agent.statusEffects.hasTrait(cTraits.Archmage))
				return 0;

			float min = 3.000f;
			float max = 5.000f;

			if (agent.statusEffects.hasTrait(cTraits.FocusedCasting))
				max -= 0.500f;
			else if (agent.statusEffects.hasTrait(cTraits.FocusedCasting_2))
				max -= 1.000f;
			else if (agent.statusEffects.hasTrait(cTraits.WildCasting))
			{
				min -= 1.500f;
				max += 1.500f;
			}
			else if (agent.statusEffects.hasTrait(cTraits.WildCasting_2))
			{
				min -= 3.000f;
				max += 3.000f;
			}

			if (agent.statusEffects.hasTrait(cTraits.MagicTraining))
			{
				min -= 0.250f;
				max -= 0.250f;
			}
			else if (agent.statusEffects.hasTrait(cTraits.MagicTraining_2))
			{
				min -= 0.500f;
				max -= 0.500f;
			}

			return (int)UnityEngine.Random.Range(min, max);
		}
		public static bool MSA_PJ_RollMiscast(Agent agent, float modifier)
		{
			float risk = 1.000f + modifier;

			if (agent.statusEffects.hasTrait(cTraits.Archmage))
				return false;
			else if (agent.statusEffects.hasTrait(cTraits.FocusedCasting))
				risk -= 0.500f;
			else if (agent.statusEffects.hasTrait(cTraits.FocusedCasting_2))
				risk -= 0.750f;
			else if (agent.statusEffects.hasTrait(cTraits.WildCasting))
				risk += 0.250f;
			else if (agent.statusEffects.hasTrait(cTraits.WildCasting_2))
				risk += 0.500f;

			if (agent.statusEffects.hasTrait(cTraits.MagicTraining))
				risk *= 0.75f;
			else if (agent.statusEffects.hasTrait(cTraits.MagicTraining_2))
				risk *= 0.50f;

			return risk >= UnityEngine.Random.Range(0f, 100f);
		}
		public static void MSA_PJ_StartBurnout(Agent agent)
		{
			if (!agent.statusEffects.hasTrait(cTraits.WildCasting) && !agent.statusEffects.hasTrait(cTraits.WildCasting_2))
			{
				GC.audioHandler.Play(agent, "MindControlEnd");
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

			if (agent.statusEffects.hasTrait(cTraits.MagicTraining))
				bullet.speed += 1;
			else if (agent.statusEffects.hasTrait(cTraits.MagicTraining_2))
				bullet.speed += 2;

			if (agent.statusEffects.hasTrait(cTraits.WildCasting))
				bullet.speed += 3;
			else if (agent.statusEffects.hasTrait(cTraits.WildCasting_2) || agent.statusEffects.hasTrait(cTraits.Archmage))
				bullet.speed += 6;
		}
		public static async Task MSA_PJ_StartCoolingDown(Agent agent)
		{
			BMLog("PyromancyStartCoolingDown");

			if (MSA_PJ_IsCoolingDown(agent) == false)
			{
				MSA_PJ_SetCoolingDown(agent, true);

				float duration = 2000f;

				if (agent.statusEffects.hasTrait(cTraits.MagicTraining))
					duration -= 500f;
				else if (agent.statusEffects.hasTrait(cTraits.MagicTraining_2))
					duration -= 1000f;

				if (agent.statusEffects.hasTrait(cTraits.WildCasting))
					duration -= 1250f;
				else if (agent.statusEffects.hasTrait(cTraits.WildCasting_2))
					duration -= 2000f;

				if (agent.statusEffects.hasTrait(cTraits.Archmage))
					duration = 0f;

				duration = Mathf.Max(0f, duration);

				await Task.Delay((int)duration);

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
			{
				agent.statusEffects.CreateBuffText("Recharged", agent.objectNetID);
				GC.audioHandler.Play(agent, "Recharge");
			}

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

			string[] dialogue =
			{
				"I need to give it a rest or my head will explode. I've seen it happen.",
				"Slow down! Haven't you seen The Fly?"
			};

			agent.Say(dialogue[UnityEngine.Random.Range(0, dialogue.Count() - 1)]);
		}
		public static void MSA_TB_DialogueCast(Agent agent)
		{
			if (GC.percentChance(75))
				return;

			agent.SpawnParticleEffect("Spawn", agent.curPosition);
			GameController.gameController.audioHandler.Play(agent, "Spawn");

			string[] dialogue =
			{
				"Vwip!",
				"Nothing personal, kid."
			};

			agent.Say(dialogue[UnityEngine.Random.Range(0, dialogue.Count() - 1)]);
		}
		public static void MSA_TB_DialogueFullyCharged(Agent agent) // TODO
		{
			agent.SpawnParticleEffect("ExplosionEMP", agent.curPosition);
			GC.audioHandler.Play(agent, "Hypnotize");
		}
		public static void MSA_TB_DialogueMiscast(Agent agent)
		{
			GC.audioHandler.Play(agent, "ZombieSpitFire");

			string[] dialogue =
			{
				"I smell burning toast.",
				"Blurgh. (Drool)",
				"I pink I bust hab a stwoke.",
				"My head a splode."
			};

			agent.Say(dialogue[UnityEngine.Random.Range(0, dialogue.Count() - 1)]);
		}
		public static void MSA_TB_DialogueRecharge(Agent agent)
		{
			string[] dialogue =
			{
				"Who needs Scotty? I'll beam my damn self up."
			};

			agent.Say(dialogue[UnityEngine.Random.Range(0, dialogue.Count() - 1)]);
		}
		#endregion
		public static void MSA_TB_Initialize()
		{
			Sprite spriteTelemanticBlink = RogueUtilities.ConvertToSprite(Properties.Resources.TelemanticBlink);

			CustomAbility telemanticBlink = RogueLibs.CreateCustomAbility(cAbilities.TelemanticBlink, spriteTelemanticBlink, true,
				new CustomNameInfo("Telemantic Blink"),
				new CustomNameInfo("Cast: Hold to charge, point cursor at destination. Charge increases accuracy but also miscast chance.\nMiscast: Have a small stroke."),
				delegate (InvItem item)
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

			telemanticBlink.OnPressed = delegate (InvItem item, Agent agent)
			{
				if (!MSA_TB_IsMiscast(agent) && !MSA_TB_IsReturning(agent) && item.invItemCount > 0)
					agent.SpawnParticleEffect("ExplosionMindControl", agent.curPosition);
				else
					MSA_TB_DialogueCantDo(agent);
			};

			telemanticBlink.OnHeld = delegate (InvItem item, Agent agent, ref float timeHeld)
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

			telemanticBlink.OnReleased = delegate (InvItem item, Agent agent)
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

			if (agent.statusEffects.hasTrait(cTraits.Archmage))
				return 10;
			else if (agent.statusEffects.hasTrait(cTraits.MagicTraining))
				rate *= 1.250f;
			else if (agent.statusEffects.hasTrait(cTraits.MagicTraining_2))
				rate *= 1.500f;

			if (agent.statusEffects.hasTrait(cTraits.FocusedCasting))
				rate *= 1.250f;
			else if (agent.statusEffects.hasTrait(cTraits.FocusedCasting_2))
				rate *= 1.500f;
			else if (agent.statusEffects.hasTrait(cTraits.WildCasting))
				rate *= 2.000f;
			else if (agent.statusEffects.hasTrait(cTraits.WildCasting_2))
				rate *= 3.000f;

			return (int)rate;
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

			if (agent.statusEffects.hasTrait(cTraits.Archmage))
				return 0;
			else if (agent.statusEffects.hasTrait(cTraits.MagicTraining))
			{
				min *= 0.875f;
				max *= 0.875f;
			}
			else if (agent.statusEffects.hasTrait(cTraits.MagicTraining_2))
			{
				min *= 0.750f;
				max *= 0.750f;
			}

			if (agent.statusEffects.hasTrait(cTraits.FocusedCasting))
			{
				min *= 0.750f;
				max *= 0.750f;
			}
			else if (agent.statusEffects.hasTrait(cTraits.FocusedCasting_2))
			{
				min *= 0.500f;
				max *= 0.500f;
			}
			else if (agent.statusEffects.hasTrait(cTraits.WildCasting))
			{
				min *= 0.500f;
				max *= 1.250f;
			}
			else if (agent.statusEffects.hasTrait(cTraits.WildCasting_2))
			{
				min *= 0.250f;
				max *= 1.500f;
			}

			return (int)UnityEngine.Random.Range(min, max);
		}
		public static int MSA_TB_RollRechargeRate(Agent agent)
		{
			float min = 5.00f;
			float max = 10.00f;

			if (agent.statusEffects.hasTrait(cTraits.Archmage))
				return 1000;
			if (agent.statusEffects.hasTrait(cTraits.MagicTraining))
			{
				min *= 1.500f;
				max *= 1.500f;
			}
			else if (agent.statusEffects.hasTrait(cTraits.MagicTraining_2))
			{
				min *= 2.250f;
				max *= 2.250f;
			}

			if (agent.statusEffects.hasTrait(cTraits.FocusedCasting))
			{
				min *= 2.000f;
				max *= 1.250f;
			}
			else if (agent.statusEffects.hasTrait(cTraits.FocusedCasting_2))
			{
				min *= 3.000f;
				max *= 1.750f;
			}
			else if (agent.statusEffects.hasTrait(cTraits.WildCasting))
			{
				min *= 0.500f;
				max *= 3.000f;
			}
			else if (agent.statusEffects.hasTrait(cTraits.WildCasting_2))
			{
				min *= 0.250f;
				max *= 4.000f;
			}

			return (int)UnityEngine.Random.Range(min, max);
		}
		public static int MSA_TB_RollReturnDuration(Agent agent)
		{
			float duration = 4000f;

			if (agent.statusEffects.hasTrait(cTraits.Archmage))
				return 0;
			else if (agent.statusEffects.hasTrait(cTraits.MagicTraining))
				duration *= 0.500f;
			else if (agent.statusEffects.hasTrait(cTraits.MagicTraining_2))
				duration *= 0.250f;

			if (agent.statusEffects.hasTrait(cTraits.FocusedCasting))
				duration *= 0.875f;
			else if (agent.statusEffects.hasTrait(cTraits.FocusedCasting_2))
				duration *= 0.750f;
			else if (agent.statusEffects.hasTrait(cTraits.WildCasting))
				duration *= 0.250f;
			else if (agent.statusEffects.hasTrait(cTraits.WildCasting_2))
				duration *= 0.000f;

			return (int)duration;
		}
		public static void MSA_TB_StartCast(Agent agent, float charge)
		{
			float maxError = 200.000f;

			if (agent.statusEffects.hasTrait(cTraits.FocusedCasting))
				maxError -=  25.000f;
			else if (agent.statusEffects.hasTrait(cTraits.FocusedCasting_2))
				maxError -=  50.000f;
			else if (agent.statusEffects.hasTrait(cTraits.WildCasting))
				maxError +=  25.000f;
			else if (agent.statusEffects.hasTrait(cTraits.WildCasting_2))
				maxError +=  50.000f;

			maxError = (maxError - charge) / 20f;

			agent.Teleport(MSA_TB_RollDestination(agent, 0, maxError), false, true);
			agent.rb.velocity = Vector2.zero;

			MSA_TB_DialogueCast(agent);
		}
		public static void MSA_TB_StartRecharge(Agent agent, bool routine)
		{
			if (true) // Not checking for routine/nonroutine yet.
			{
				agent.statusEffects.CreateBuffText("Recharged", agent.objectNetID);
				GC.audioHandler.Play(agent, "Recharge");
			}
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
			if (agent.statusEffects.hasTrait(cTraits.Archmage))
				return false;
			else if (agent.statusEffects.hasTrait(cTraits.MagicTraining))
				netCharge *= 0.750f;
			else if (agent.statusEffects.hasTrait(cTraits.MagicTraining_2))
				netCharge *= 0.500f;

			if (agent.statusEffects.hasTrait(cTraits.FocusedCasting))
				netCharge *= 0.500f;
			else if (agent.statusEffects.hasTrait(cTraits.FocusedCasting_2))
				netCharge *= 0.250f;
			else if (agent.statusEffects.hasTrait(cTraits.WildCasting))
				netCharge *= 1.500f;
			else if (agent.statusEffects.hasTrait(cTraits.WildCasting_2))
				netCharge *= 2.000f;

			netCharge -= UnityEngine.Random.Range(0f, 100f);
			netCharge /= 25;

			int severity = (int)Mathf.Clamp(netCharge, 0, 4);

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
		public static void Agent_FindOperatingTime(PlayfieldObject operatingOnObject, float timeToUnlock, Agent __instance, ref float __result) // Postfix
		{
			__result *= GC.selectedTimeScale;
		}
		#endregion
		#region LoadLevel
		public static void LoadLevel_SetupMore5_2(LoadLevel __instance) // Postfix
		{
			baseTimeScale = GameController.gameController.selectedTimeScale;
		}
		#endregion
		#region StatusEffects
		public static void StatusEffects_GiveSpecialAbility(string abilityName, StatusEffects __instance) // Postfix
		{
			if (__instance.agent.inventory.equippedSpecialAbility != null)
			{
				InvItem ability = __instance.agent.inventory.equippedSpecialAbility;
				Agent agent = __instance.agent;

				string[] magicAbilities = 
				{ 
					cAbilities.ChronomanticDilation, 
					cAbilities.PyromanticJet, 
					cAbilities.TelemanticBlink 
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
