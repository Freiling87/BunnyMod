using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using BepInEx.Logging;
using RogueLibsCore;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BunnyMod.Abilities.A_Magic
{
	public class PyromanticJet : CustomAbility, IAbilityRechargeable
	{
		private static readonly string loggerName = $"BunnyMod_{MethodBase.GetCurrentMethod().DeclaringType?.Name}";
		private static ManualLogSource Logger => _logger ?? (_logger = BepInEx.Logging.Logger.CreateLogSource(loggerName));
		private static ManualLogSource _logger;

		#region Main
		[RLSetup]
		public static void Setup()
		{
			RogueLibs.CreateCustomAbility<PyromanticJet>()
				.WithDescription(new CustomNameInfo
				{
					[LanguageCode.English] = "Cast: Hold to shoot flames.\n\nMiscast: Excrete Napalm from all your pores. What did you eat??",
					[LanguageCode.Russian] = "Заклинание: Удерживайте кнопку Супер-способности, чтобы выстрелить пламенем.\n\nНеудача: Из вас течёт Напалм.. что вы ели??",
				})
				.WithName(new CustomNameInfo
				{
					[LanguageCode.English] = "Pyromantic Jet",
					[LanguageCode.Russian] = "Огненная струя",
				})
				.WithSprite(Properties.Resources.PyromanticJet)
				.WithUnlock(new AbilityUnlock
				{
					CharacterCreationCost = 6,
					IsAvailable = true,
					IsAvailableInCC = true,
					UnlockCost = 6,
				});
		}
		public override void OnAdded()
		{
		}
		public void OnHeld(AbilityHeldArgs e)
		{
			if (e.HeldTime >= MSA_PJ_RollFireRate(Item.agent) && !MSA_PJ_IsBurnedOut(Item.agent) && !MSA_PJ_IsCoolingDown(Item.agent) && !MSA_PJ_IsMiscast(Item.agent))
			{
				e.HeldTime = 0;

				if (MSA_PJ_RollMiscast(Item.agent, 0))
					MSA_PJ_StartMiscast(Item.agent, 20);
				else
				{
					MSA_PJ_StartCast(Item.agent);

					Item.invItemCount -= MSA_PJ_RollManaCost(Item.agent);

					if (Item.invItemCount <= 0)
						MSA_PJ_StartBurnout(Item.agent);
				}
			}
			else
				MSA_PJ_DialogueCantDo(Item.agent);
		}
		public override void OnPressed()
		{
			// TODO: Initial "Flame woof" sound effect
		}
		public void OnRecharging(AbilityRechargingArgs e)
		{
			e.UpdateDelay = 0.1f;

			if (Item.invItemCount < Shared.CalcMaxMana(Item.agent) && Item.agent.statusEffects.CanRecharge())
			{
				Item.invItemCount++;

				if (Item.invItemCount == Shared.CalcMaxMana(Item.agent))
					MSA_PJ_CompleteRecharge(Item.agent, true);
			}
		}
		public void OnReleased(AbilityReleasedArgs e)
		{
			// TODO: Make smoke come out of the player

			if (e.HeldTime > 0 && !MSA_PJ_IsBurnedOut(Item.agent) && !MSA_PJ_IsCoolingDown(Item.agent) && !MSA_PJ_IsMiscast(Item.agent))
				MSA_PJ_StartCoolingDown(Item.agent);
		}
		public override void SetupDetails()
		{
			base.SetupDetails();
			Item.cantDrop = true;
			Item.dontAutomaticallySelect = true;
			Item.dontSelectNPC = true;
			Item.gunKnockback = 0;
			Item.rapidFire = true;
			Item.initCount = 100;
			Item.itemType = "WeaponProjectile";
			Item.rapidFire = true;
			Item.rechargeAmountInverse = 100;
			Item.shadowOffset = 2;
			Item.stackable = true;
			Item.thiefCantSteal = true;
		}
		#endregion
		#region Bits
		public static bool MSA_PJ_IsBurnedOut(Agent agent)
		{
			Logger.LogDebug("PyromancyIsBurnedOut: " + ((agent.inventory.equippedSpecialAbility.otherDamage & 0b_0001) != 0));

			return (agent.inventory.equippedSpecialAbility.otherDamage & 0b_0001) != 0;
		}
		public static bool MSA_PJ_IsCoolingDown(Agent agent)
		{
			Logger.LogDebug("PyromancyIsCoolingDown: " + ((agent.inventory.equippedSpecialAbility.otherDamage & 0b_0010) != 0));

			return (agent.inventory.equippedSpecialAbility.otherDamage & 0b_0010) != 0;
		}
		public static bool MSA_PJ_IsMiscast(Agent agent)
		{
			Logger.LogDebug("PyromancyIsMiscast: " + ((agent.inventory.equippedSpecialAbility.otherDamage & 0b_0100) != 0));

			return (agent.inventory.equippedSpecialAbility.otherDamage & 0b_0100) != 0;
		}
		public static void MSA_PJ_LogBooleans(Agent agent)
		{
			Logger.LogDebug("PyromancyIsBurnedOut: " + MSA_PJ_IsBurnedOut(agent));
			Logger.LogDebug("PyromancyIsCoolingDown: " + MSA_PJ_IsCoolingDown(agent));
			Logger.LogDebug("PyromancyIsMiscast: " + MSA_PJ_IsMiscast(agent));
		}
		public static void MSA_PJ_SetBurnedOut(Agent agent, bool value)
		{
			Logger.LogDebug("PyromancySetBurnedOut " + value);

			if (value) agent.inventory.equippedSpecialAbility.otherDamage |= 0b_0001;
			else agent.inventory.equippedSpecialAbility.otherDamage &= ~0b_0001;
		}
		public static void MSA_PJ_SetCoolingDown(Agent agent, bool value)
		{
			Logger.LogDebug("PyromancySetCoolingDown " + value);

			if (value) agent.inventory.equippedSpecialAbility.otherDamage |= 0b_0010;
			else agent.inventory.equippedSpecialAbility.otherDamage &= ~0b_0010;
		}
		public static void MSA_PJ_SetMiscast(Agent agent, bool value)
		{
			Logger.LogDebug("PyromancySetMiscast " + value);

			if (value) agent.inventory.equippedSpecialAbility.otherDamage |= 0b_0100;
			else agent.inventory.equippedSpecialAbility.otherDamage &= ~0b_0100;
		}
		#endregion
		#region Dialogue
		public static void MSA_PJ_DialogueCantDo(Agent agent)
		{
			gc.audioHandler.Play(agent, "CantDo");

			if (gc.percentChance(75))
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
			if (gc.percentChance(99))
				return;

			gc.audioHandler.Play(agent, vAudioClip.AgentLaugh);

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
			if (gc.percentChance(75))
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
			gc.audioHandler.Play(agent, "Recharge");

			if (gc.percentChance(75))
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
		#region Mechanics
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

			return (int)Random.Range(min, max);
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
				gc.audioHandler.Play(agent, vAudioClip.Win);
				MSA_PJ_SetBurnedOut(agent, true);
			}

			Task.Delay(5000);

			MSA_PJ_SetBurnedOut(agent, false); // 202103301952
		}
		public static void MSA_PJ_StartCast(Agent agent)
		{
			Logger.LogDebug("PyromancyStartCast");

			Bullet bullet = gc.spawnerMain.SpawnBullet(agent.gun.tr.position, bulletStatus.Fire, agent);

			if (agent.controllerType == "Keyboard" && !gc.sessionDataBig.trackpadMode)
				bullet.movement.RotateToMouseTr(agent.agentCamera.actualCamera);
			else if (agent.target.AttackTowardTarget())
				bullet.tr.rotation = Quaternion.Euler(0f, 0f, agent.target.transform.eulerAngles.z);
			else
				bullet.tr.rotation = Quaternion.Euler(0f, 0f, agent.gun.FindWeaponAngleGamepad() - 90f);

			if (gc.sessionDataBig.autoAim != "Off")
			{
				int myChance = 25; // Placeholder, find the real numbers later. For now, suck it, Auto-aimers B)
				if (gc.percentChance(myChance))
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
			Logger.LogDebug("PyromancyStartCoolingDown");

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

				await Task.Delay((int)duration);

				MSA_PJ_SetCoolingDown(agent, false);
			}
		}
		public static void MSA_PJ_StartMiscast(Agent agent, int degree)
		{
			Logger.LogDebug("PyromancyStartMiscast");

			MSA_PJ_DialogueMiscast(agent);

			gc.spawnerMain.SpawnExplosion(agent, agent.curPosition, "FireBomb");

			MSA_PJ_StartBurnout(agent);
		}
		public static void MSA_PJ_CompleteRecharge(Agent agent, bool routine)
		{
			Logger.LogDebug("PyromancyStartRecharge");

			if (!routine)
				MSA_PJ_DialogueRecharge(agent);

			if (MSA_PJ_IsBurnedOut(agent))
				MSA_PJ_SetBurnedOut(agent, false);

			if (MSA_PJ_IsMiscast(agent))
				MSA_PJ_SetMiscast(agent, false);

			agent.inventory.buffDisplay.specialAbilitySlot.MakeUsable();
		}
		#endregion
	}
}
