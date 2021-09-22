using BepInEx.Logging;
using BunnyMod.Content.Logging;
using BunnyMod.Content.Traits;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;
using Random = UnityEngine.Random;
using System.Collections;
using System.Reflection;
using System;

namespace BunnyMod.Content.Patches
{
	[HarmonyPatch(declaringType:typeof(SkillPoints))]
	public static class P_SkillPoints
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		public static GameController GC => GameController.gameController;

		// TODO soon to be replaced with transpiler - (i.e. currently not working)
		[HarmonyPrefix, HarmonyPatch(methodName:nameof(SkillPoints.AddPointsLate), argumentTypes: new[] { typeof(string), typeof(int) })]
		public static bool AddPointsLate_Prefix(string pointsType, int extraNum, ref IEnumerator __result, SkillPoints __instance,
				ref Agent ___agent) 
		{
			logger.LogDebug("SkillPoints_AddPointsLate");

			__result = AddPointsLate_Enumerator(pointsType, extraNum, __instance, ___agent);

			return false;
		}

		// Non-Patch
		private static IEnumerator AddPointsLate_Enumerator(string pointsType, int extraNum, SkillPoints __instance,
				Agent ___agent) 
		{
			logger.LogDebug("SkillPoints_AddPointsLate_IEnumerator:");
			logger.LogDebug("\tpointsType = " + pointsType);
			logger.LogDebug("\textraNum = " + extraNum);

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
					if (___agent.statusEffects.hasTrait(vTrait.TheLaw) && !___agent.oma.superSpecialAbility ||
							___agent.HasTrait<VeryHardOnYourself>()) // TODO is new
					{
						if (___agent.statusEffects.hasTrait(vTrait.Crooked))
							xpReward = 0;
						else if (___agent.statusEffects.hasTrait(vTrait.Crooked2))
							xpReward = -40;
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
				case cSkillPoints.BQMalusDowntown: // TODO is new
					xpReward = -500;
					break;
				case cSkillPoints.BQMalusFloor: // TODO is new
					xpReward = 300;
					break;
				case cSkillPoints.BQMalusGame: // TODO is new
					xpReward = -1000;
					break;
				case cSkillPoints.BQMalusIndustrial: // TODO is new
					xpReward = -500;
					break;
				case cSkillPoints.BQMalusPark: // TODO is new
					xpReward = -500;
					break;
				case cSkillPoints.BQMalusSlums: // TODO is new
					xpReward = -500;
					break;
				case cSkillPoints.BQMalusUptown: // TODO is new
					xpReward = -500;
					break;
				case "CompleteMission":
					xpReward = 300;

					if (GC.challenges.Contains(cChallenge.UnpaidInternship)) // TODO is new
						xpReward *= 2;

					break;
				case "CompleteMissionFindBombs":
					xpReward = 700;
					break;
				case "CompleteMissionReduced":
					xpReward = 150;

					if (GC.challenges.Contains(cChallenge.UnpaidInternship)) // TODO is new
						xpReward *= 2;

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
				case cSkillPoints.ElectabilityMalus: // TODO is new
					xpReward = -100;
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
				case cSkillPoints.FreePrisonerFailure: // TODO is new
					xpReward = -50 * extraNum;
					break;
				case "HackPoints":
					xpReward = 20;
					break;
				case "IndirectlyKill":
					xpReward = 30;
					break;
				case "IndirectlyKillInnocent":
					if ((___agent.statusEffects.hasTrait(vTrait.TheLaw) && !___agent.oma.superSpecialAbility) ||
							___agent.HasTrait<VeryHardOnYourself>()) // TODO is new
					{
						if (___agent.statusEffects.hasTrait(vTrait.Crooked))
							xpReward = 0;
						else if (___agent.statusEffects.hasTrait(vTrait.Crooked2))
							xpReward = -15;
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
					if ((___agent.statusEffects.hasTrait(vTrait.TheLaw) && !___agent.oma.superSpecialAbility) ||
							___agent.HasTrait<VeryHardOnYourself>()) // TODO is new
					{
						if (___agent.statusEffects.hasTrait(vTrait.Crooked2))
							xpReward = 0;
						else if (___agent.statusEffects.hasTrait(vTrait.Crooked))
							xpReward = -20;
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
					if ((___agent.statusEffects.hasTrait(vTrait.TheLaw) && !___agent.oma.superSpecialAbility) ||
							___agent.HasTrait<VeryHardOnYourself>()) // TODO is new
					{
						if (___agent.statusEffects.hasTrait(vTrait.Crooked))
							xpReward = 0;
						else if (___agent.statusEffects.hasTrait(vTrait.Crooked2))
							xpReward = -20;
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
				case cSkillPoints.AngeredMany: // TODO is new
					xpReward *= -100; // TODO '*' by mistake?
					break;
				case "NoDamageTaken":
					xpReward = 100;
					break;
				case cSkillPoints.TookLotsOfDamage: // TODO is new
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
					if ((___agent.statusEffects.hasTrait(vTrait.TheLaw) && !___agent.oma.superSpecialAbility &&
									!___agent.statusEffects.hasTrait(vTrait.PromiseIllReturnIt)) ||
							___agent.HasTrait<VeryHardOnYourself>()) // TODO is new
					{
						if (___agent.statusEffects.hasTrait(vTrait.Crooked))
							xpReward = 0;
						else if (___agent.statusEffects.hasTrait(vTrait.Crooked2))
							xpReward = -10;
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
					xpReward = extraNum * 10;
					break;
				case "StealPointsNegative":
					xpReward = extraNum * 10;

					if ((___agent.statusEffects.hasTrait(vTrait.TheLaw) && !___agent.oma.superSpecialAbility &&
									!___agent.statusEffects.hasTrait(vTrait.PromiseIllReturnIt)) ||
							___agent.HasTrait<VeryHardOnYourself>()) // TODO is new
					{
						xpReward *= -1;

						if (___agent.statusEffects.hasTrait(vTrait.Crooked))
							xpReward /= 2;
						else if (___agent.statusEffects.hasTrait(vTrait.Crooked2))
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
					xpReward = 100; // TODO missing a '0' here?
					break;
			}

			if (xpReward != 0)
			{
				float xpModifier = 1.00f;

				if (___agent.HasTrait<SmoothBrained>()) // TODO is new
					xpModifier = 0.00f;
				else if (xpReward > 0)
				{
					if (___agent.statusEffects.hasTrait(vTrait.Studious))
						xpModifier = 1.30f;
					else if (___agent.statusEffects.hasTrait(vTrait.SuperStudious))
						xpModifier = 1.50f;
					else if (___agent.HasTrait<DimBulb>()) // TODO is new
						xpModifier = 0.75f;
					else if (___agent.HasTrait<MoronTheMerrier>()) // TODO is new
						xpModifier = 0.50f;
					else if (___agent.HasTrait<Brainiac>()) // TODO is new
						xpModifier = 4.00f;
				}
				else if (___agent.HasTrait<VeryHardOnYourself>()) // TODO is new
					xpModifier = 2.00f;

				xpReward = (int)(xpReward * xpModifier);

				float floorXpAcceleration = 0.075f;
				int cityFloor = Mathf.Clamp(GC.sessionDataBig.curLevelEndless, 1, 16);

				if (GC.sessionDataBig.challenges.Contains("QuickGame"))
				{
					floorXpAcceleration = 0.1125f;
					cityFloor = Mathf.Clamp(GC.sessionDataBig.curLevelEndless, 1, 11);
				}

				xpReward = (int)(xpReward * (1f + (cityFloor - 1) * floorXpAcceleration));
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

					// TODO this is a Coroutine, you'll have to StartCoroutine it. - although that doesn't really matter since this will be replaced with a transpiler
					MethodInfo CancelJustGainedLevel = AccessTools.DeclaredMethod(typeof(SkillPoints), "CancelJustGainedLevel");
					CancelJustGainedLevel.GetMethodWithoutOverrides<Action>(__instance).Invoke();

					if (GC.unlocks.CanDoUnlocks())
					{
						if (___agent.statusEffects.hasTrait(vTrait.SuperStudious))
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

					if (___agent.statusEffects.hasTrait(vTrait.PotentialtoNotSuck))
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

						if (GC.sessionData.skillLevel[___agent.isPlayer] % 2 == 0 && (agent.strengthStatMod != 3 || agent.enduranceStatMod != 3 ||
								agent.accuracyStatMod != 3 || agent.speedStatMod != 3))
						{
							string randStatMod;
							bool bonusStat;

							do
							{
								randStatMod = GC.Choose("Strength", "Endurance", "Accuracy", "Speed");
								bonusStat = true;

								if (randStatMod == "Strength" && agent.strengthStatMod == 3)
									bonusStat = false;
								else if (randStatMod == "Endurance" && agent.enduranceStatMod == 3)
									bonusStat = false;
								else if (randStatMod == "Accuracy" && agent.accuracyStatMod == 3)
									bonusStat = false;
								else if (randStatMod == "Speed" && agent.speedStatMod == 3)
									bonusStat = false;
							} while (!bonusStat);

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

			logger.LogDebug("\txpReward = " + xpReward);
		}
	}
}
