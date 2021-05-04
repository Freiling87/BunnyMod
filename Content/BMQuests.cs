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
                invItem3.invItemCount = 0;
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
                    if (___agent.statusEffects.hasTrait(vTrait.TheLaw) && !___agent.oma.superSpecialAbility)
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
                case "HackPoints":
                    xpReward = 20;
                    break;
                case "IndirectlyKill":
                    xpReward = 30;
                    break;
                case "IndirectlyKillInnocent":
                    if (___agent.statusEffects.hasTrait(vTrait.TheLaw) && !___agent.oma.superSpecialAbility)
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
                    if (___agent.statusEffects.hasTrait(vTrait.TheLaw) && !___agent.oma.superSpecialAbility)
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
                    if (___agent.statusEffects.hasTrait(vTrait.TheLaw) && !___agent.oma.superSpecialAbility)
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
                case "NoDamageTaken":
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
                    if (___agent.statusEffects.hasTrait(vTrait.TheLaw) && !___agent.oma.superSpecialAbility && !___agent.statusEffects.hasTrait(vTrait.PromiseIllReturnIt))
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

                    if (___agent.statusEffects.hasTrait(vTrait.TheLaw) && !___agent.oma.superSpecialAbility && !___agent.statusEffects.hasTrait(vTrait.PromiseIllReturnIt))
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
    }
}
