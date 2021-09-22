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
	[HarmonyPatch(declaringType:typeof(StatsScreen))]
	public static class P_StatsScreen
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		public static GameController GC => GameController.gameController;

		// TODO soon to be replaced with transpiler - (i.e. currently not working)
		[HarmonyPrefix, HarmonyPatch(methodName:nameof(StatsScreen.DoStatsScreenUnlocks), argumentTypes: new[] { typeof(int) })]
		public static bool DoStatsScreenUnlocks_Prefix(int agentNum, StatsScreen __instance)
		{
			// Limited strictly to VeryHardOnYourself, to add negative XP rewards

			bool veryHardOnYourself = __instance.agent.HasTrait<VeryHardOnYourself>();

			bool quickGame = GC.challenges.Contains("QuickGame");
			bool bigQuestThemeComplete = false;
			bool bigQuestRunComplete = false;

			if (GC.quests.CanHaveBigQuest(__instance.agent))
			{
				if (GC.quests.BigQuestBasedOnTotal(__instance.agent))
				{
					if (GC.quests.CheckIfBigQuestCompleteFloor(__instance.agent) && __instance.agent.health > 0f &&
							(!(__instance.agent.bigQuest == "Firefighter") || __instance.agent.needArsonist > 1))
						__instance.agent.skillPoints.AddPoints("BigQuestBonusFloor");
					else if (veryHardOnYourself) // TODO is new
						__instance.agent.skillPoints.AddPoints(cSkillPoints.BQMalusFloor); // TODO is new

					if (((!quickGame && (GC.sessionDataBig.curLevel == 3 || GC.sessionDataBig.curLevel == 6 || GC.sessionDataBig.curLevel == 9 ||
											GC.sessionDataBig.curLevel == 12 || GC.sessionDataBig.curLevel == 15)) ||
									(quickGame && (GC.sessionDataBig.curLevel == 2 || GC.sessionDataBig.curLevel == 4 || GC.sessionDataBig.curLevel == 6 ||
											GC.sessionDataBig.curLevel == 8 || GC.sessionDataBig.curLevel == 10))) &&
							GC.quests.CheckIfBigQuestCompleteTheme(__instance.agent, false))
						bigQuestThemeComplete = true;

					if (((!quickGame && GC.sessionDataBig.curLevel == 15) ||
									(quickGame && GC.sessionDataBig.curLevel == 10)) &&
							GC.quests.CheckIfBigQuestCompleteRun(__instance.agent, false) && !GC.challenges.Contains("Endless"))
						bigQuestRunComplete = true;
				}
				else if (__instance.agent.oma.bigQuestObjectCountTotal != 0)
				{
					if (GC.quests.CheckIfBigQuestCompleteFloor(__instance.agent))
					{
						if (__instance.agent.health > 0f)
							__instance.agent.skillPoints.AddPoints("BigQuestBonusFloor");

						if (GC.quests.CheckIfBigQuestCompleteTheme(__instance.agent, false))
							bigQuestThemeComplete = true;

						if (GC.quests.CheckIfBigQuestCompleteRun(__instance.agent, false) && !GC.challenges.Contains("Endless"))
							bigQuestRunComplete = true;
					}
					else if (veryHardOnYourself) // TODO is new | bad refactoring ? not having veryHardOnYourself breaks vanilla execution here
					{
						__instance.agent.skillPoints.AddPoints(cSkillPoints.BQMalusFloor);

						if (GC.sessionData.bigQuestStatusGame[__instance.agent.isPlayer - 1] != "QuestWaitForNextGame" &&
								GC.sessionData.bigQuestStatusGame[__instance.agent.isPlayer - 1] != "QuestStartFromBeginning")
							GC.sessionData.bigQuestStatusGame[__instance.agent.isPlayer - 1] = "QuestFailed";

						if (GC.sessionData.bigQuestStatusTheme[__instance.agent.isPlayer - 1] != "QuestWaitForNextArea")
							GC.sessionData.bigQuestStatusTheme[__instance.agent.isPlayer - 1] = "QuestFailed";
					}
				}
				else if (GC.quests.CheckIfBigQuestCompleteTheme(__instance.agent, false))
				{
					bigQuestThemeComplete = true;

					if (GC.quests.CheckIfBigQuestCompleteRun(__instance.agent, false) && !GC.challenges.Contains("Endless"))
						bigQuestRunComplete = true;
				}

				if (bigQuestThemeComplete && __instance.agent.health > 0f)
				{
					switch (GC.levelTheme)
					{
						case 0:
							__instance.agent.skillPoints.AddPoints("BigQuestBonusSlums");
							break;
						case 1:
							__instance.agent.skillPoints.AddPoints("BigQuestBonusIndustrial");
							break;
						case 2:
							__instance.agent.skillPoints.AddPoints("BigQuestBonusPark");
							break;
						case 3:
							__instance.agent.skillPoints.AddPoints("BigQuestBonusDowntown");
							break;
						case 4:
							__instance.agent.skillPoints.AddPoints("BigQuestBonusUptown");
							break;
					}
				}
				else if (!bigQuestThemeComplete && __instance.agent.health > 0f && veryHardOnYourself) // TODO is new
				{
					switch (GC.levelTheme)
					{
						case 0:
							__instance.agent.skillPoints.AddPoints(cSkillPoints.BQMalusSlums);
							break;
						case 1:
							__instance.agent.skillPoints.AddPoints(cSkillPoints.BQMalusIndustrial);
							break;
						case 2:
							__instance.agent.skillPoints.AddPoints(cSkillPoints.BQMalusPark);
							break;
						case 3:
							__instance.agent.skillPoints.AddPoints(cSkillPoints.BQMalusDowntown);
							break;
						case 4:
							__instance.agent.skillPoints.AddPoints(cSkillPoints.BQMalusUptown);
							break;
					}
				}

				if (GC.sessionData.bigQuestCountGame != null)
				{
					GC.sessionData.bigQuestCountGame[__instance.agent.isPlayer - 1] += __instance.agent.oma.bigQuestObjectCount;
					GC.sessionData.bigQuestCountTheme[__instance.agent.isPlayer - 1] += __instance.agent.oma.bigQuestObjectCount;
				}

				if (bigQuestRunComplete)
				{
					__instance.agent.finishedBigQuestRun = true;
					GC.sessionData.bigQuestStatusGame[__instance.agent.isPlayer - 1] = "QuestCompleted";

					if (!GC.customCampaign)
					{
						if (!GC.challenges.Contains("Sandbox") && !GC.unlocks.CharactersExceed())
						{
							GC.unlocks.DoUnlock(__instance.agent.bigQuest + "_BQ", "BigQuest");
							GC.unlocks.DoUnlock("CompleteAnyBigQuest", "Extra");
							__instance.StartCoroutine(GC.mainGUI.questNotification.Unlock("BigQuest", __instance.agent.bigQuest + "_BQ", __instance.agent));
						}

						__instance.agent.oma.superSpecialAbility = true;

						if (!GC.serverPlayer)
							__instance.agent.objectMult.CallCmdBecomeSuperSpecial();

						if (__instance.agent.agentName == "Cop")
						{
							__instance.agent.agentName = "Cop2";
							__instance.agent.inventory.defaultArmorHead.invItemName = "Cop2Hat";
						}
						else if (__instance.agent.agentName == "Guard")
							__instance.agent.agentName = "Guard2";
						else if (__instance.agent.agentName == "Hobo")
							__instance.agent.agentName = "UpperCruster";

						GC.sessionData.agentsCompletedBigQuest.Add(__instance.agent.isPlayer);

						if (__instance.agent.health > 0f)
							__instance.agent.skillPoints.AddPoints("BigQuestBonusGame");
					}
				}
				else if (veryHardOnYourself) // TODO is new
					__instance.agent.skillPoints.AddPoints(cSkillPoints.BQMalusGame);
			}

			if (__instance.agent.health > 0f)
			{
				bool noKills = false;
				bool onlyFists = false;

				#region XP

				if (!GC.stats.gunsUsed[agentNum])
					__instance.agent.skillPoints.AddPoints("NoGuns");

				if (!GC.stats.meleeUsed[agentNum] && !GC.stats.gunsUsed[agentNum])
				{
					__instance.agent.skillPoints.AddPoints("OnlyFists");
					onlyFists = true;
				}

				if (GC.stats.destruction[agentNum] >= 30)
					__instance.agent.skillPoints.AddPoints("Destruction");
				else if (GC.stats.destruction[agentNum] == 0)
					__instance.agent.skillPoints.AddPoints("NoDestruction");

				if (GC.stats.stoleItems[agentNum] >= 15)
					__instance.agent.skillPoints.AddPoints("StoleLots");
				else if (GC.stats.stoleItems[agentNum] == 0) // TODO is new
					__instance.agent.skillPoints.AddPoints(cSkillPoints.StoleNone);

				if (GC.stats.angered[agentNum] == 0)
					__instance.agent.skillPoints.AddPoints("NoAngerLevel");
				else if (GC.stats.angered[agentNum] >= (GC.agentCount * 4 / 5)) // TODO is new
					__instance.agent.skillPoints.AddPoints(cSkillPoints.AngeredMany);

				if (GC.stats.killed[agentNum] == 0 && GC.stats.indirectlyKilled[agentNum] == 0)
				{
					__instance.agent.skillPoints.AddPoints("NoKillLevel");
					noKills = true;
				}
				else if (GC.stats.killed[agentNum] >= 20)
					__instance.agent.skillPoints.AddPoints("Massacre");

				if (GC.stats.knockedOut[agentNum] >= 20)
					__instance.agent.skillPoints.AddPoints("ManySleeping");

				if (GC.stats.damageTaken[agentNum] == 0)
					__instance.agent.skillPoints.AddPoints("NoDamageTaken");
				else if (GC.stats.damageTaken[agentNum] >= 400) // TODO is new
					__instance.agent.skillPoints.AddPoints(cSkillPoints.TookLotsOfDamage);

				if (GC.stats.timeTaken[agentNum] < 120f)
					__instance.agent.skillPoints.AddPoints("TimeBonus");

				#endregion

				#region Unlocks

				GC.unlocks.DoUnlock("CompleteAnyLevel", "Extra");

				if (!GC.challenges.Contains("NoSlums") && !GC.challenges.Contains("NoIndustrial") && !GC.challenges.Contains("NoPark")
						&& !GC.challenges.Contains("NoDowntown") && !GC.challenges.Contains("NoUptown") && !GC.challenges.Contains("RandomLevelThemes")
						&& !GC.challenges.Contains("QuickGame"))
				{
					if (GC.sessionDataBig.curLevelEndless == 3)
					{
						GC.unlocks.DoUnlock("CompleteLevel1", "Extra");
						GC.unlocks.DoUnlock("NoSlums", "Challenge");
					}
					else if (GC.sessionDataBig.curLevelEndless == 6)
					{
						GC.unlocks.DoUnlock("CompleteLevel2", "Extra");
						GC.unlocks.DoUnlock("NoIndustrial", "Challenge");
					}
					else if (GC.sessionDataBig.curLevelEndless == 9)
					{
						GC.unlocks.DoUnlock("CompleteLevel3", "Extra");
						GC.unlocks.DoUnlock("NoPark", "Challenge");
					}
					else if (GC.sessionDataBig.curLevelEndless == 12)
					{
						GC.unlocks.DoUnlock("CompleteLevel4", "Extra");
						GC.unlocks.DoUnlock("NoDowntown", "Challenge");
					}
					else if (GC.sessionDataBig.curLevelEndless == 11)
						GC.unlocks.DoUnlock("MixedUpLevels", "Challenge");
					else if (GC.sessionDataBig.curLevelEndless == 13)
						GC.unlocks.DoUnlock("QuickGame", "Challenge");
					else if (GC.sessionDataBig.curLevelEndless == 14)
						GC.unlocks.DoUnlock("RandomLevelThemes", "Challenge");
					else if (GC.sessionDataBig.curLevelEndless == 15)
					{
						GC.unlocks.DoUnlock("CompleteLevel5", "Extra");
						GC.unlocks.DoUnlock("NoUptown", "Challenge");
					}
				}

				if (noKills && GC.stats.hostiled[agentNum] == 0)
					GC.unlocks.DoUnlock("Assassin", "Agent");

				if (GC.stats.destruction[agentNum] >= 75)
					GC.unlocks.DoUnlock("Athlete", "Agent");

				if (GC.stats.killed[agentNum] >= 20)
					GC.unlocks.DoUnlock("Cannibal", "Agent");

				if (__instance.agent.agentName != "Custom")
					GC.unlocks.DoUnlockProgress("ShapeShifter", "Agent", __instance.agent.agentName);

				if (!GC.sessionData.charactersExceed)
				{
					string text = __instance.agent.agentName;

					if (text == "Custom")
						text = __instance.agent.agentRealName;

					if (!GC.challenges.Contains("NoSlums") && !GC.challenges.Contains("NoIndustrial") && !GC.challenges.Contains("NoPark") &&
							!GC.challenges.Contains("NoDowntown") && !GC.challenges.Contains("NoUptown") && !GC.challenges.Contains("RandomLevelThemes"))
					{
						if (GC.sessionDataBig.curLevelEndless == 3 && !quickGame)
							GC.unlocks.DoUnlockProgress("Floor2", "Floor", text);
						else if (GC.sessionDataBig.curLevelEndless == 6 && !quickGame)
							GC.unlocks.DoUnlockProgress("Floor3", "Floor", text);
						else if (GC.sessionDataBig.curLevelEndless == 9 && !quickGame)
							GC.unlocks.DoUnlockProgress("Floor4", "Floor", text);
						else if (GC.sessionDataBig.curLevelEndless == 12 && !quickGame)
							GC.unlocks.DoUnlockProgress("Floor5", "Floor", text);
					}
				}

				if (GC.sessionDataBig.curLevelEndless == 15 && !quickGame && GC.challenges.Contains("TimeLimit"))
					GC.unlocks.DoUnlock("TimeLimit2", "Challenge");

				if (GC.challenges.Contains("TimeLimit"))
					GC.unlocks.DoUnlock("TimeLimitQuestsGiveMoreTime", "Challenge");

				if (onlyFists)
				{
					if (GC.sessionDataBig.curLevelEndless >= 1 && GC.sessionDataBig.curLevelEndless <= 3 && !quickGame)
					{
						GC.sessionData.usedOnlyFists[__instance.agent.isPlayer]++;

						if (GC.sessionData.usedOnlyFists[__instance.agent.isPlayer] == 3)
							GC.unlocks.DoUnlock("Wrestler", "Agent");
					}

					if (GC.sessionDataBig.curLevelEndless == 3 && !quickGame)
						GC.sessionData.usedOnlyFists[__instance.agent.isPlayer] = 0;
				}

				if (__instance.agent.agentName == "Vampire" && GC.sessionDataBig.curLevelEndless == 3 && !quickGame)
					GC.unlocks.DoUnlock("Zombie", "Agent");

				foreach (Agent agent in GC.playerAgentList)
				{
					bool flag6 = true;

					if (agent.localPlayer)
						for (int i = 0; i < GC.agentList.Count; i++)
						{
							Agent agent2 = GC.agentList[i];

							if (agent2.isPlayer == 0 && !agent2.dead && __instance.agent.employer == null && !__instance.agent.objectAgent &&
									agent2.relationships.GetRel(agent) != "Hateful")
							{
								flag6 = false;

								break;
							}
						}

					if (flag6)
					{
						GC.unlocks.DoUnlock("EveryoneHatesYou", "Challenge");

						break;
					}
				}

				#endregion
			}

			__instance.GetElectionScore(agentNum, true);

			if (__instance.currentLevelElectionScore > 0)
			{
				GC.sessionData.electionScore[__instance.agent.isPlayer]++;

				if (!GC.challenges.Contains("QuickGame") && GC.sessionData.electionScore[__instance.agent.isPlayer] > 15)
					GC.sessionData.electionScore[__instance.agent.isPlayer] = 15;
				else if (GC.challenges.Contains("QuickGame") && GC.sessionData.electionScore[__instance.agent.isPlayer] > 10)
					GC.sessionData.electionScore[__instance.agent.isPlayer] = 10;

				__instance.agent.skillPoints.AddPoints("ElectabilityBonus");

				return false;
			}
			else if (__instance.currentLevelElectionScore < 0)
			{
				if (veryHardOnYourself) // TODO is new
					__instance.agent.skillPoints.AddPoints(cSkillPoints.ElectabilityMalus);

				try
				{
					GC.sessionData.electionScore[__instance.agent.isPlayer]--;

					if (!GC.challenges.Contains("QuickGame") && GC.sessionData.electionScore[__instance.agent.isPlayer] < -15)
						GC.sessionData.electionScore[__instance.agent.isPlayer] = -15;
					else if (GC.challenges.Contains("QuickGame") && GC.sessionData.electionScore[__instance.agent.isPlayer] < -10)
						GC.sessionData.electionScore[__instance.agent.isPlayer] = -10;
				}
				catch { }
			}

			return false;
		}
	}
}
