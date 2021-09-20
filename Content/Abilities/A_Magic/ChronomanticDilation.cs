using RogueLibsCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Logging;
using System.Reflection;
using BunnyMod.Content.Abilities.A_Magic;

namespace BunnyMod.Content.Abilities
{
	class ChronomanticDilation : CustomAbility, IAbilityRechargeable
	{
		private static readonly string loggerName = $"BunnyMod_{MethodBase.GetCurrentMethod().DeclaringType?.Name}";
		private static ManualLogSource Logger => _logger ?? (_logger = BepInEx.Logging.Logger.CreateLogSource(loggerName));
		private static ManualLogSource _logger;

		#region Main
		[RLSetup]
		public static void Setup()
		{
			RogueLibs.CreateCustomAbility<ChronomanticDilation>()
				.WithDescription(new CustomNameInfo
				{
					[LanguageCode.English] = "Chronomantic Dilation",
					[LanguageCode.Russian] = "Хрономантический сдвиг",
				})
				.WithName(new CustomNameInfo
				{
					[LanguageCode.English] = "Cast: Toggle to slow down everything but yourself. Toggle to deactivate.\nMiscast: Speed up everything but yourself.",
					[LanguageCode.Russian] = "Заклинание: Нажмите кнопку Супер-способности, чтобы замедлить всё, кроме себя. Нажмите повторно, чтобы выключить замедление.\nНеудача: Ускоряет всё кроме вас.",
				})
				.WithSprite(Properties.Resources.ChronomanticDilation)
				.WithUnlock(new AbilityUnlock
				{
					CharacterCreationCost = 10,
					IsAvailable = true,
					IsAvailableInCC = true,
					UnlockCost = 10,
				});
		}
		public override void OnAdded()
		{
			throw new NotImplementedException();
		}
		public override void OnPressed()
		{
			if (MSA_CD_IsWindingUp(Item.agent) || MSA_CD_IsMiscast(Item.agent))
				MSA_CD_DialogueCantDo(Item.agent);
			else if (MSA_CD_IsCast(Item.agent))
				MSA_CD_StartDecast(Item.agent);
			else if (MSA_CD_RollMiscast(Item.agent, (float)((Shared.CalcMaxMana(Item.agent) - Item.invItemCount) / 100f)))
				MSA_CD_StartMiscast(Item.agent, MSA_CD_RollTimescale(Item.agent, true));
			else
				MSA_CD_StartCast(Item.agent, MSA_CD_RollTimescale(Item.agent, false));
		}
		public override void SetupDetails()
		{
			Item.cantDrop = true;
			Item.Categories.Add("Usable");
			Item.Categories.Add("NPCsCantPickup");
			Item.dontAutomaticallySelect = true;
			Item.dontSelectNPC = true;
			Item.otherDamage = 0; // Bitwise variable field, see Extension method class below
			Item.isWeapon = false;
			Item.initCount = 100;
			Item.rechargeAmountInverse = 100;
			Item.maxAmmo = 100;
			Item.stackable = true;
			Item.thiefCantSteal = true;
		}
		public void OnRecharging(AbilityRechargingArgs e)
		{
			e.UpdateDelay = 1f;

			if (MSA_CD_IsCast(Item.agent))
			{
				Item.invItemCount -= MSA_CD_RollManaCost(Item.agent);

				if (Item.invItemCount < 0)
					MSA_CD_StartMiscast(Item.agent, MSA_CD_RollTimescale(Item.agent, true));
			}
			else if (Item.invItemCount < Shared.CalcMaxMana(Item.agent) && Item.agent.statusEffects.CanRecharge())
			{
				Item.invItemCount += Math.Min(Shared.CalcMaxMana(Item.agent) - Item.invItemCount, 5);

				if (Item.invItemCount == Shared.CalcMaxMana(Item.agent) && !MSA_CD_IsMiscast(Item.agent))
					MSA_CD_StartRecharge(Item.agent, true);
			}
		}
		#endregion

		public static float baseTimeScale;

		#region Mechanics
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

			Logger.LogDebug("ChronomancyRollTimescale: " + timescale);

			return timescale;
		}
		public static void MSA_CD_StartCast(Agent agent, float speedupfactor)
		{
			MSA_CD_SetCast(agent, true);
			MSA_CD_DialogueCast(agent);

			gc.selectedTimeScale = baseTimeScale / speedupfactor;
			gc.mainTimeScale = baseTimeScale / speedupfactor;
			agent.speedMax = agent.FindSpeed() * (int)speedupfactor;

			Logger.LogDebug("Timescale: " + gc.mainTimeScale.ToString());
		}
		public static async void MSA_CD_StartDecast(Agent agent)
		{
			agent.speedMax = agent.FindSpeed();

			MSA_CD_SetCast(agent, false); // Needs to occur before delays or Overcast occurs erroneously

			gc.selectedTimeScale = baseTimeScale;
			gc.mainTimeScale = baseTimeScale;

			MSA_CD_DialogueDecast(agent);

			await Task.Delay(1000);

			await MSA_CD_StartWindingUp(agent);

			Logger.LogDebug("Timescale: " + gc.mainTimeScale.ToString());
		}
		public static async void MSA_CD_StartMiscast(Agent agent, float slowdownFactor)
		{
			Logger.LogDebug("ChronomancyStartMiscast: " + slowdownFactor);

			MSA_CD_DialogueMiscast(agent);

			if (MSA_CD_IsCast(agent))
				MSA_CD_SetCast(agent, false);

			MSA_CD_SetMiscast(agent, true);

			MSA_CD_StartWindingUp(agent); // TODO: Ensure that this duration is equal to miscast duration

			gc.selectedTimeScale = baseTimeScale / slowdownFactor;
			gc.mainTimeScale = baseTimeScale / slowdownFactor;
			agent.speedMax = (int)((float)agent.FindSpeed() * slowdownFactor);

			agent.inventory.buffDisplay.specialAbilitySlot.MakeNotUsable();

			Logger.LogDebug("Timescale: " + gc.mainTimeScale.ToString());

			await Task.Delay(5000);

			MSA_CD_SetMiscast(agent, false);
			MSA_CD_StartDecast(agent);
		}
		public static void MSA_CD_StartRecharge(Agent agent, bool routine)
		{
			Logger.LogDebug("ChronomancyStartRecharge");

			if (!routine)
				MSA_CD_DialogueRecharge(agent);

			if (MSA_CD_IsWindingUp(agent))
				MSA_CD_SetWindingUp(agent, false);

			if (MSA_CD_IsMiscast(agent))
			{
				//TODO: Eliminate redundancies between Recharge and DeCast

				MSA_CD_SetMiscast(agent, false);

				gc.selectedTimeScale = baseTimeScale;
				gc.mainTimeScale = baseTimeScale;
				agent.speedMax = agent.FindSpeed();

				agent.inventory.buffDisplay.specialAbilitySlot.MakeUsable();
			}

			Logger.LogDebug("Timescale: " + gc.mainTimeScale.ToString());
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

			await Task.Delay((int)duration);

			while (MSA_CD_IsMiscast(agent))
				await Task.Delay(1000);

			MSA_CD_StartRecharge(agent, true);
		}
		#endregion
		#region Bits
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
			// TODO: Restore logging if needed
			//Logger.LogDebug("ChronomancyIsCast: " + MSA_CD_IsCast(agent));
			//Logger.LogDebug("ChronomancyIsMiscast: " + MSA_CD_IsMiscast(agent));
			//Logger.LogDebug("ChronomancyIsWindindUp: " + MSA_CD_IsWindingUp(agent));
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
		#region Dialogue
		public static void MSA_CD_DialogueCantDo(Agent agent)
		{
			if (gc.percentChance(75))
				return;

			gc.audioHandler.Play(agent, "CantDo");

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

			if (gc.percentChance(75))
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

			if (gc.percentChance(75))
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
			gc.audioHandler.Play(agent, "ToiletTeleportIn");

			if (gc.percentChance(75))
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
			gc.audioHandler.Play(agent, "Recharge");

			if (gc.percentChance(75))
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
	}
}
