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

	}
}