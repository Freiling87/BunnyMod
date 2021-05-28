using HarmonyLib;
using RogueLibsCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BunnyMod.Content
{
	public class BMQuests
	{
		public static GameController GC => GameController.gameController;
		public static bool Prefix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) => BMHeader.MainInstance.PatchPrefix(type, methodName, patchType, patchMethodName, types);
		public static bool Postfix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) => BMHeader.MainInstance.PatchPostfix(type, methodName, patchType, patchMethodName, types);
		public static void BMLog(string logMessage) => BMHeader.Log(logMessage);

		public void Awake()
		{
            ExitPoint_00();
            Quests_00();
            SkillPoints_00();
            StatsScreen_00();
		}

		#region ExitPoint
		public void ExitPoint_00()
        {
            Prefix(typeof(ExitPoint), "DetermineIfCanExit", GetType(), "ExitPoint_DetermineIfCanExit", new Type[0] { });
        }
        public static bool ExitPoint_DetermineIfCanExit(ref bool __result, ExitPoint __instance) // Prefix
		{
			if (GC.challenges.Contains(cChallenge.RushinRevolution))
			{
				__result = true;
				return false;
			}

			return true;
		}
        #endregion
        #region Quests
        public void Quests_00()
        {
			Type t = typeof(Quests);
			Type g = GetType();

            Prefix(t, "GetLevelExitChallengeItem", g, "Quests_GetLevelExitChallengeItem", new Type[0] { });
        }
        public static bool Quests_GetLevelExitChallengeItem(Quests __instance, ref InvItem __result) // Prefix
		{
            // DoublePlyRewards
            // UnpaidInternship

            bool flag = false;
            string item = "";

            if (GC.challenges.Contains(cChallenge.UnpaidInternship))
            {
                item = GC.Choose<string> (vItem.Banana,vItem.HamSandwich);
                flag = true;
            }
            else if (GC.challenges.Contains(cChallenge.DoublePlyRewards))
			{
                item = GC.Choose<string>(vItem.FreeItemVoucher, vItem.HiringVoucher);
                flag = true;
            }

            if (flag)
			{
                InvItem invItem3 = new InvItem();
                invItem3.invItemName = item;
                invItem3.SetupDetails(false);
                invItem3.invItemCount = 1;
                __instance.questItemsGiven.Add(item);

                __result = invItem3;
                return false;
            }

            return true;
        }
		#endregion
		#region SkillPoints
		public void SkillPoints_00()
        {
            Prefix(typeof(SkillPoints), "AddPointsLate", GetType(), "SkillPoints_AddPointsLate", new Type[2] { typeof(string), typeof(int) });
        }
        public static bool SkillPoints_AddPointsLate(string pointsType, int extraNum, ref IEnumerator __result, SkillPoints __instance, ref Agent ___agent) // Prefix
        {
            BMLog("SkillPoints_AddPointsLate");

            __result = SkillPoints_AddPointsLate_IEnumerator(pointsType, extraNum, __result, __instance, ___agent);

            return false;
        }
        private static IEnumerator SkillPoints_AddPointsLate_IEnumerator(string pointsType, int extraNum, IEnumerator __result, SkillPoints __instance, Agent ___agent) // Non-Patch
        {
            BMLog("SkillPoints_AddPointsLate_IEnumerator:");
            BMLog("\tpointsType = " + pointsType);
            BMLog("\textraNum = " + extraNum);

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
                    if ((___agent.statusEffects.hasTrait(vTrait.TheLaw) && !___agent.oma.superSpecialAbility) ||
                        ___agent.statusEffects.hasTrait(cTrait.VeryHardOnYourself))
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
                case cSkillPoints.BQMalusDowntown:
                    xpReward = -500;
                    break;
                case cSkillPoints.BQMalusFloor:
                    xpReward = 300;
                    break;
                case cSkillPoints.BQMalusGame:
                    xpReward = -1000;
                    break;
                case cSkillPoints.BQMalusIndustrial:
                    xpReward = -500;
                    break;
                case cSkillPoints.BQMalusPark:
                    xpReward = -500;
                    break;
                case cSkillPoints.BQMalusSlums:
                    xpReward = -500;
                    break;
                case cSkillPoints.BQMalusUptown:
                    xpReward = -500;
                    break;
                case "CompleteMission":
                    xpReward = 300;

                    if (GC.challenges.Contains(cChallenge.UnpaidInternship))
                        xpReward *= 2;

                    break;
                case "CompleteMissionFindBombs":
                    xpReward = 700;
                    break;
                case "CompleteMissionReduced":
                    xpReward = 150;

                    if (GC.challenges.Contains(cChallenge.UnpaidInternship))
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
                case cSkillPoints.ElectabilityMalus:
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
                case cSkillPoints.FreePrisonerFailure:
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
                        ___agent.statusEffects.hasTrait(cTrait.VeryHardOnYourself))
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
                        ___agent.statusEffects.hasTrait(cTrait.VeryHardOnYourself))
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
                        ___agent.statusEffects.hasTrait(cTrait.VeryHardOnYourself))
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
                case cSkillPoints.AngeredMany:
                    xpReward *= -100;
                    break;
                case "NoDamageTaken":
                    xpReward = 100;
                    break;
                case cSkillPoints.TookLotsOfDamage:
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
                    if ((___agent.statusEffects.hasTrait(vTrait.TheLaw) && !___agent.oma.superSpecialAbility && !___agent.statusEffects.hasTrait(vTrait.PromiseIllReturnIt)) ||
                        ___agent.statusEffects.hasTrait(cTrait.VeryHardOnYourself))
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

                    if ((___agent.statusEffects.hasTrait(vTrait.TheLaw) && !___agent.oma.superSpecialAbility && !___agent.statusEffects.hasTrait(vTrait.PromiseIllReturnIt)) ||
                        ___agent.statusEffects.hasTrait(cTrait.VeryHardOnYourself))
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
                    xpReward = 100;
                    break;
            }

            if (xpReward != 0)
            {
                float xpModifier = 1.00f;

                if (___agent.statusEffects.hasTrait(cTrait.SmoothBrained))
                    xpModifier = 0.00f;
                else if (xpReward > 0)
                {
                    if (___agent.statusEffects.hasTrait(vTrait.Studious))
                        xpModifier = 1.30f;
                    else if (___agent.statusEffects.hasTrait(vTrait.SuperStudious))
                        xpModifier = 1.50f;
                    else if (___agent.statusEffects.hasTrait(cTrait.DimBulb))
                        xpModifier = 0.75f;
                    else if (___agent.statusEffects.hasTrait(cTrait.MoronTheMerrier))
                        xpModifier = 0.50f;
                    else if (___agent.statusEffects.hasTrait(cTrait.Brainiac))
                        xpModifier = 4.00f;
                }
                else if (xpReward < 0)
                    if (___agent.statusEffects.hasTrait(cTrait.VeryHardOnYourself))
                        xpModifier = 2.00f;

                xpReward = (int)((float)xpReward * xpModifier);

                float floorXpAcceleration = 0.075f;
                int cityFloor = Mathf.Clamp(GC.sessionDataBig.curLevelEndless, 1, 16);

                if (GC.sessionDataBig.challenges.Contains("QuickGame"))
                {
                    floorXpAcceleration = 0.1125f;
                    cityFloor = Mathf.Clamp(GC.sessionDataBig.curLevelEndless, 1, 11);
                }

                xpReward = (int)((float)xpReward * (1f + (float)(cityFloor - 1) * floorXpAcceleration));
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

                    MethodInfo CancelJustGainedLevel = AccessTools.DeclaredMethod(typeof(SkillPoints), "CancelJustGainedLevel", new Type[0] { });
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

                        if (GC.sessionData.skillLevel[___agent.isPlayer] % 2 == 0 && (agent.strengthStatMod != 3 || agent.enduranceStatMod != 3 || agent.accuracyStatMod != 3 || agent.speedStatMod != 3))
                        {
                            string randStatMod;
                            bool bonusStat;

                            do
                            {
                                randStatMod = GC.Choose<string>("Strength", "Endurance", "Accuracy", "Speed");
                                bonusStat = true;

                                if (randStatMod == "Strength" && agent.strengthStatMod == 3)
                                    bonusStat = false;
                                else if (randStatMod == "Endurance" && agent.enduranceStatMod == 3)
                                    bonusStat = false;
                                else if (randStatMod == "Accuracy" && agent.accuracyStatMod == 3)
                                    bonusStat = false;
                                else if (randStatMod == "Speed" && agent.speedStatMod == 3)
                                    bonusStat = false;
                            }
                            while (!bonusStat);

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

            BMLog("\txpReward = " + xpReward);

            yield break;
        }
		#endregion
		#region StatsScreen
        public void StatsScreen_00()
		{
            Type t = typeof(StatsScreen);
            Type g = GetType();

            Prefix(t, "DoStatsScreenUnlocks", g, "StatsScreen_DoStatsScreenUnlocks", new Type[1] { typeof(int) });
		}
        public static bool StatsScreen_DoStatsScreenUnlocks(int agentNum, StatsScreen __instance) // Replacement
		{
            // Limited strictly to VeryHardOnYourself, to add negative XP rewards

            bool veryHardOnYourself = __instance.agent.statusEffects.hasTrait(cTrait.VeryHardOnYourself);

            bool quickGame = GC.challenges.Contains("QuickGame");
            bool bigQuestThemeComplete = false;
            bool bigQuestRunComplete = false;

            if (GC.quests.CanHaveBigQuest(__instance.agent))
            {
                if (GC.quests.BigQuestBasedOnTotal(__instance.agent))
                {
                    if (GC.quests.CheckIfBigQuestCompleteFloor(__instance.agent) && __instance.agent.health > 0f && (!(__instance.agent.bigQuest == "Firefighter") || __instance.agent.needArsonist > 1))
                        __instance.agent.skillPoints.AddPoints("BigQuestBonusFloor");
                    else if (veryHardOnYourself)
                        __instance.agent.skillPoints.AddPoints(cSkillPoints.BQMalusFloor);
                    
                    if (((!quickGame && (GC.sessionDataBig.curLevel == 3 || GC.sessionDataBig.curLevel == 6 || GC.sessionDataBig.curLevel == 9 || GC.sessionDataBig.curLevel == 12 || GC.sessionDataBig.curLevel == 15)) || 
                        (quickGame && (GC.sessionDataBig.curLevel == 2 || GC.sessionDataBig.curLevel == 4 || GC.sessionDataBig.curLevel == 6 || GC.sessionDataBig.curLevel == 8 || GC.sessionDataBig.curLevel == 10))) && 
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
                    else if (veryHardOnYourself)
                    {
                        __instance.agent.skillPoints.AddPoints(cSkillPoints.BQMalusFloor);

                        if (GC.sessionData.bigQuestStatusGame[__instance.agent.isPlayer - 1] != "QuestWaitForNextGame" && GC.sessionData.bigQuestStatusGame[__instance.agent.isPlayer - 1] != "QuestStartFromBeginning")
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
                else if (!bigQuestThemeComplete && __instance.agent.health > 0f && veryHardOnYourself)
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
                else if (veryHardOnYourself)
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
                else if (GC.stats.stoleItems[agentNum] == 0)
                    __instance.agent.skillPoints.AddPoints(cSkillPoints.StoleNone);

                if (GC.stats.angered[agentNum] == 0)
                    __instance.agent.skillPoints.AddPoints("NoAngerLevel");
                else if (GC.stats.angered[agentNum] >= (GC.agentCount * 4 / 5))
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
                else if (GC.stats.damageTaken[agentNum] >= 400)
                    __instance.agent.skillPoints.AddPoints(cSkillPoints.TookLotsOfDamage);
                
                if (GC.stats.timeTaken[agentNum] < 120f)
                    __instance.agent.skillPoints.AddPoints("TimeBonus");
				#endregion
				#region Unlocks
				GC.unlocks.DoUnlock("CompleteAnyLevel", "Extra");
                
                if (!GC.challenges.Contains("NoSlums") && !GC.challenges.Contains("NoIndustrial") && !GC.challenges.Contains("NoPark") && !GC.challenges.Contains("NoDowntown") && !GC.challenges.Contains("NoUptown") && !GC.challenges.Contains("RandomLevelThemes") && !GC.challenges.Contains("QuickGame"))
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
                    
                    if (!GC.challenges.Contains("NoSlums") && !GC.challenges.Contains("NoIndustrial") && !GC.challenges.Contains("NoPark") && !GC.challenges.Contains("NoDowntown") && !GC.challenges.Contains("NoUptown") && !GC.challenges.Contains("RandomLevelThemes"))
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

                            if (agent2.isPlayer == 0 && !agent2.dead && __instance.agent.employer == null && !__instance.agent.objectAgent && agent2.relationships.GetRel(agent) != "Hateful")
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
                if (veryHardOnYourself)
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
		#endregion
	}
}
