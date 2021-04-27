using HarmonyLib;
using RogueLibsCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BunnyMod.Content
{
	public class BMObjects
    {
        public static GameController GC => GameController.gameController;
        public static bool Prefix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) => BMHeader.MainInstance.PatchPrefix(type, methodName, patchType, patchMethodName, types);
        public static bool Postfix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) => BMHeader.MainInstance.PatchPostfix(type, methodName, patchType, patchMethodName, types);
        public static void BMLog(string logMessage) => BMHeader.Log(logMessage);

        #region Main
        public void Awake()
        {
            Initialize_Names();

            ObjectReal_00();
            PlayfieldObject_00();

            AlarmButton_00();
            BathTub_00();
            Crate_00();
            Door_00();
            Elevator_00();
            FireHydrant_00();
            FlamingBarrel_00();
            Generator_00();
            Generator2_00();
            Hole_00();
            LaserEmitter_00();
            Manhole_00();
            Plant_00();
            PoliceBox_00();
            PoolTable_00();
            Refrigerator_00();
            SlotMachine_00();
            Stove_00();
            TableBig_00();
            Television_00();
            Toilet_00();
			TrashCan_00();
            VendorCart_00();
            Window_00();
        }
        public void FixedUpdate()
        {
            //TODO: Move this to occur at appropriate times rather than on update.

            List<Stove> stoveList = new List<Stove>();

            foreach (KeyValuePair<Stove, Stove_Remora> pair in Stove_Variables)
                if (pair.Key.isBroken())
                    stoveList.Add(pair.Key);

            foreach (Stove stove in stoveList)
            {
                Stove_Variables.Remove(stove);
                BMHeader.ConsoleMessage.LogInfo("Removed pair.Key from VariablesStove");
            }

            //List<Refrigerator> refrigeratorList = new List<Refrigerator>();

            //foreach (KeyValuePair<Refrigerator, Refrigerator_Remora> pair in Refrigerator_Variables)
            //    if (pair.Key.isBroken())
            //        refrigeratorList.Add(pair.Key);

            //foreach (Refrigerator Refrigerator in refrigeratorList)
            //{
            //    Refrigerator_Variables.Remove(Refrigerator);
            //    BunnyHeader.ConsoleMessage.LogInfo("Removed pair.Key from VariablesRefrigerator");
            //}
        }
        public static void Initialize_Names()
        {
            CustomName dispenseIce = RogueLibs.CreateCustomName("DispenseIce", "Interface", new CustomNameInfo("Dispense Ice"));

            CustomName openContainer = RogueLibs.CreateCustomName("OpenContainer", "Interface", new CustomNameInfo("Open container"));

            CustomName slotMachine_Play1 = RogueLibs.CreateCustomName("Play1", "Interface", new CustomNameInfo("Bet $1"));
            CustomName slotMachine_Play100 = RogueLibs.CreateCustomName("Play100", "Interface", new CustomNameInfo("Bet $100"));
        }
        #endregion

        #region Custom
        public static void CorrectButtonCosts(ObjectReal objectReal)
        {
            // TODO: This will only catch one tamper operation per object
            // Next you'd want to make a dictionary of buttonsExtra indexes and new versions of the labels 
            // You can't do it within the loop because it will break execution

            // TODO: Detect full button string, not buttonsExtra

            bool flag = false;
            string newLabel = "";

            foreach (string buttonLabel in objectReal.buttonsExtra)
            {
                //BMLog("Detected ButtonExtra: " + buttonLabel + " (Only non-blank when additional costs are applied to button)");

                if (buttonLabel.EndsWith("-30"))
                {
                    newLabel = buttonLabel.Replace("-30", "-" + BMAgents.ToolCost(objectReal.interactingAgent, 30));
                    flag = true;
                }
                else if (buttonLabel.EndsWith("-20"))
                {
                    newLabel = buttonLabel.Replace("-20", "-" + BMAgents.ToolCost(objectReal.interactingAgent, 20));
                    flag = true;
                }
                else if (buttonLabel == " - 7HP")
                {
                    newLabel = buttonLabel.Replace(" - 7HP", " - " + BMTraits.HealthCost(objectReal.interactingAgent, 7, DamageType.brokenWindow));
                    flag = true;
                }
                else if (buttonLabel == " - 15HP")
                {
                    newLabel = buttonLabel.Replace(" - 15HP", " - " + BMTraits.HealthCost(objectReal.interactingAgent, 15, DamageType.brokenWindow));
                    flag = true;
                }
                else if (buttonLabel.EndsWith("(Burn hands for 10 damage)"))
				{
                    newLabel = buttonLabel.Replace("(Burn hands for 10 damage)", "(Burn hands for " + BMTraits.HealthCost(objectReal.interactingAgent, 10, DamageType.burnedFingers) + " damage)");
                    flag = true;
				}

                if (flag)
                {
                    objectReal.buttonsExtra[objectReal.buttonsExtra.FindIndex(ind => ind.Equals(buttonLabel))] = newLabel;
                    break;
                }
            }
        }
		#endregion

		#region Base Classes
		#region ObjectReal
		public void ObjectReal_00()
		{
            Prefix(typeof(ObjectReal), "DestroyMe", GetType(), "ObjectReal_DestroyMe", new Type[1] { typeof(PlayfieldObject) });
            Prefix(typeof(ObjectReal), "DestroyMe3", GetType(), "ObjectReal_DestroyMe3", new Type[0] { });
            Postfix(typeof(ObjectReal), "DetermineButtons", GetType(), "ObjectReal_DetermineButtons", new Type[0]);
            Prefix(typeof(ObjectReal), "FinishedOperating", GetType(), "ObjectReal_FinishedOperating", new Type[0]);
            Prefix(typeof(ObjectReal), "Interact", GetType(), "ObjectReal_Interact", new Type[1] { typeof(Agent) });
            Prefix(typeof(ObjectReal), "MakeNonFunctional", GetType(), "ObjectReal_MakeNonFunctional", new Type[1] { typeof(PlayfieldObject) });
            Postfix(typeof(ObjectReal), "ObjectAction", GetType(), "ObjectReal_ObjectAction", new Type[5] { typeof(string), typeof(string), typeof(float), typeof(Agent), typeof(PlayfieldObject) });
            Prefix(typeof(ObjectReal), "ObjectUpdate", GetType(), "ObjectReal_ObjectUpdate", new Type[0]);
            Prefix(typeof(ObjectReal), "PressedButton", GetType(), "ObjectReal_PressedButton", new Type[2] { typeof(string), typeof(int) });
            Postfix(typeof(ObjectReal), "Start", GetType(), "ObjectReal_Start", new Type[0]);
        }
        public static bool ObjectReal_DestroyMe(PlayfieldObject damagerObject, ObjectReal __instance) // Prefix
        {
            BMHeader.ConsoleMessage.LogMessage(__instance.name + ": " + MethodBase.GetCurrentMethod().Name);

            if (__instance is Stove)
                Stove_Variables[(Stove)__instance].savedDamagerObject = damagerObject;

            return true;
        }
        public static bool ObjectReal_DestroyMe3(ObjectReal __instance) // Prefix
        {
            if (__instance is Stove)
            {
                if (GC.serverPlayer && !__instance.spawnedExplosion)
                {
                    __instance.spawnedExplosion = true;
                    Explosion explosion = GC.spawnerMain.SpawnExplosion(Stove_Variables[(Stove)__instance].savedDamagerObject, __instance.tr.position, "FireBomb", false, -1, false, __instance.FindMustSpawnExplosionOnClients(Stove_Variables[(Stove)__instance].savedDamagerObject));

                    if (Stove_Variables[(Stove)__instance].noOwnCheckCountdown)
                        explosion.noOwnCheck = true;
                }
                GC.audioHandler.Stop(__instance, "GeneratorHiss");
            }

            return true;
        }
        public static void ObjectReal_DetermineButtons(ObjectReal __instance) // Postfix
        {
            Agent agent = __instance.interactingAgent;

            if (__instance is FlamingBarrel)
            {
                if (__instance.ora.hasParticleEffect)
                {
                    if (agent.inventory.HasItem("Fud"))
                    {
                        __instance.buttons.Add("GrillFud");
                        __instance.buttonsExtra.Add(" (Burn hands for " + BMTraits.HealthCost(agent, 10, DamageType.burnedFingers) + " damage)");
                    }
                    else
                        agent.SayDialogue("CantGrillFud");
                }
                else
                {
                    if (agent.inventory.HasItem("CigaretteLighter"))
                        __instance.buttons.Add("LightBarbecue");
                    else
                        agent.SayDialogue("CantOperateBarbecue");
                }
            }
            else if (__instance is Manhole)
            {
                Manhole manhole = (Manhole)__instance;

                if (!manhole.opened && agent.inventory.HasItem("Crowbar"))
                {
                    __instance.buttons.Add("UseCrowbar");
                    __instance.buttonsExtra.Add(" (" + agent.inventory.FindItem("Crowbar").invItemCount + ") -" + BMAgents.ToolCost(agent, 15));
                }

                if (manhole.opened && agent.statusEffects.hasTrait(cTrait.UnderdarkCitizen))
                    __instance.buttons.Add("FlushYourself");
            }
            else if (__instance is Stove)
            {
                if (!__instance.startedFlashing)
                {
                    if (agent.inventory.HasItem("Wrench"))
                    {
                        __instance.buttons.Add("UseWrenchToDetonate");
                        __instance.buttonsExtra.Add(" (" + agent.inventory.FindItem("Wrench").invItemCount + ") -" + BMAgents.ToolCost(agent, 30));
                    }

                    if (agent.inventory.HasItem(vItem.Fud))
                    {
                        if (GC.challenges.Contains(cChallenge.AnCapistan))
						{
                            __instance.buttons.Add("GrillFudPaid");
                            __instance.buttonPrices.Add(5);
                        }
                        else
                            __instance.buttons.Add("GrillFud");
                    }
                }
            }
            else if (__instance is TrashCan)
            {
                __instance.buttons.Add("HideInTrashcan");
                __instance.buttons.Add("OpenChest");
            }
            else if (__instance is VendorCart)
                __instance.buttons.Add("VendorCart_Steal");
        }
        public static bool ObjectReal_FinishedOperating(ObjectReal __instance) // Replacement
        {
            BMHeader.ConsoleMessage.LogMessage(__instance.name + ": " + MethodBase.GetCurrentMethod().Name);

            MethodInfo finishedOperating_base = AccessTools.DeclaredMethod(typeof(PlayfieldObject), "FinishedOperating", new Type[0] { });
            finishedOperating_base.GetMethodWithoutOverrides<Action>(__instance).Invoke();

            if (__instance is FlamingBarrel)
                FlamingBarrel_GrilledFud((FlamingBarrel)__instance);
            else if (__instance is Manhole)
			{
                if (__instance.operatingItem.invItemName == vItem.Crowbar)
                    Manhole_UseCrowbar((Manhole)__instance);
			}
            else if (__instance is Stove)
            {
                if (__instance.operatingItem.invItemName == vItem.Wrench)
                    Stove_UseWrenchToDetonate((Stove)__instance);
                else if (__instance.operatingItem.invItemName == vItem.Fud)
                    Stove_GrilledFud((Stove)__instance);
            }
            else if (__instance is VendorCart)
                VendorCart_Steal((VendorCart)__instance);
            else if (!__instance.interactingAgent.interactionHelper.interactingFar)
            {
                string operatingBarType = __instance.operatingBarType;

                if (operatingBarType == "Collecting")
                    __instance.CollectPart();
            }

            return false;
        }
        public static bool ObjectReal_Interact(Agent agent, ObjectReal __instance) // Replacement
        {
            BMHeader.ConsoleMessage.LogMessage("ObjectReal_Interact: " + __instance.name);

            MethodInfo interact_base = AccessTools.DeclaredMethod(typeof(PlayfieldObject), "Interact", new Type[1] { typeof(Agent) });
            interact_base.GetMethodWithoutOverrides<Action<Agent>>(__instance).Invoke(agent);

            if (__instance is Bathtub || __instance is Plant || __instance is PoolTable || __instance is TableBig)
            {
                if (agent.statusEffects.hasTrait(cTrait.StealthBastardDeluxe))
                {
                    agent.SetInvisible(false);
                    agent.statusEffects.BecomeHidden(__instance);
                }

                __instance.StopInteraction();
            }
            else if (__instance is FlamingBarrel)
                __instance.ShowObjectButtons();
            else if (__instance is Manhole)
                __instance.ShowObjectButtons();
            else if (__instance is Stove)
            {
                if (__instance.timer > 0f || __instance.startedFlashing)
                    __instance.StopInteraction();

                __instance.ShowObjectButtons();
            }
            else if (__instance is VendorCart)
                __instance.ShowObjectButtons();

            __instance.playerInvDatabase = agent.GetComponent<InvDatabase>();

            return false;
        }
        public static bool ObjectReal_MakeNonFunctional(PlayfieldObject damagerObject, ObjectReal __instance) // Prefix
        {
            BMHeader.ConsoleMessage.LogMessage(__instance.name + ": " + MethodBase.GetCurrentMethod().Name);

            if (__instance is Stove)
            {
                if (damagerObject != null && __instance.interactable)
                {
                    GC.playerAgent.SetCheckUseWithItemsAgain(__instance);

                    if (!GC.serverPlayer)
                        GC.playerAgent.objectMult.ObjectAction(__instance.objectNetID, "MakeNonFunctional");

                    __instance.timer = 5f;
                    __instance.timeCountdownClock = (int)__instance.timer;
                    BunnyHeaderTools.InvokeRepeating(Stove_Variables[(Stove)__instance], "Countdown", 0.01f, 1f);

                    __instance.interactable = false;
                    Stove_Variables[(Stove)__instance].savedDamagerObject = damagerObject;
                    Stove_Variables[(Stove)__instance].countdownCauser = Stove_Variables[(Stove)__instance].savedDamagerObject;
                }
                return false;
            }
            return true;
        }
        public static void ObjectReal_ObjectAction(string myAction, string extraString, float extraFloat, Agent causerAgent, PlayfieldObject extraObject, ObjectReal __instance, ref bool ___noMoreObjectActions) // Postfix
        {
            BMHeader.ConsoleMessage.LogMessage(__instance.name + ": " + MethodBase.GetCurrentMethod().Name);

            if (__instance is Manhole)
            {
                if (myAction == "FlushYourself")
                    Manhole_FlushYourself((Manhole)__instance);
                else if (myAction == "UseCrowbar")
                    Manhole_UseCrowbar((Manhole)__instance);
            }
            else if (__instance is Stove)
            {
                if (!___noMoreObjectActions && myAction == "UseWrenchToDetonate")
                    Stove_UseWrenchToDetonate((Stove)__instance);

                ___noMoreObjectActions = false;
            }
            else if (__instance is VendorCart)
                if (myAction == "VendorCart_Steal")
				{
                    VendorCart_Steal((VendorCart)__instance);

                    ___noMoreObjectActions = true;
                }
        }
        public static bool ObjectReal_ObjectUpdate(ObjectReal __instance) // Prefix
        {
            if (__instance is Stove)
            {
                Stove_Remora remora = Stove_Variables[(Stove)__instance];

                if (__instance.timer > 0f)
                {
                    __instance.timer -= Time.deltaTime;

                    if (__instance.timer <= 0f)
                    {
                        if (__instance.startedFlashing)
                        {
                            __instance.DestroyMe(remora.savedDamagerObject);

                            return false;
                        }

                        remora.noOwnCheckCountdown = true;
                        remora.savedDamagerObject = remora.countdownCauser;
                        __instance.DestroyMe(remora.countdownCauser);
                    }
                }

                return false;
            }

            return true;
        }
        public static bool ObjectReal_PressedButton(string buttonText, int buttonPrice, ObjectReal __instance) // Replacement
        {
            BMHeader.ConsoleMessage.LogMessage(__instance.name + ": " + MethodBase.GetCurrentMethod().Name);

            MethodInfo pressedButton_Base = AccessTools.DeclaredMethod(typeof(PlayfieldObject), "PressedButton", new Type[2] { typeof(string), typeof(int) });
            pressedButton_Base.GetMethodWithoutOverrides<Action<string, int>>(__instance).Invoke(buttonText, buttonPrice);

            Agent agent = __instance.interactingAgent;
            int dodgyStrikes = 0;
            float dodgyVolume = 0f;

            if (buttonText == "CollectPart") // Vanilla
            {
                __instance.StartCoroutine(__instance.Operating(agent, null, 5f, true, "Collecting"));

                dodgyStrikes = 0;
                dodgyVolume = 1f;
            }
            else if (buttonText == "DispenseIce")
            {
                __instance.StartCoroutine(__instance.Operating(agent, agent.inventory.FindItem(vItem.Wrench), 2f, true, "Tampering"));

                dodgyStrikes = 0;
                dodgyVolume = 1f;
            }
            else if (buttonText == "FlushYourself" && __instance is Manhole)
                Manhole_FlushYourself((Manhole)__instance);
            else if (buttonText == "GrillFud")
                __instance.StartCoroutine(__instance.Operating(agent, agent.inventory.FindItem(vItem.Fud), 2f, true, "Grilling"));
            else if (buttonText == "GrillFudPaid" && __instance.moneySuccess(5))
                __instance.StartCoroutine(__instance.Operating(agent, agent.inventory.FindItem(vItem.Fud), 2f, true, "Grilling"));
            else if (buttonText == "HackExplode")
                __instance.HackExplode(agent);
            else if (buttonText == "HideInTrashcan")
                TrashCan_Hide((TrashCan)__instance, agent);
            else if (buttonText == "LightBarbecue")
            {
                __instance.StartFireInObject();
                __instance.StopInteraction();
            }
            else if (buttonText == "OpenChest")
                __instance.ShowChest();
            else if (buttonText == "UseCrowbar")
			{
                __instance.StartCoroutine(__instance.Operating(agent, agent.inventory.FindItem("Crowbar"), 2f, true, "Unlocking"));
                dodgyStrikes = 0;
                dodgyVolume = 1f;
            }
            else if (buttonText == "UseWrenchToDetonate")
            {
                __instance.StartCoroutine(__instance.Operating(agent, agent.inventory.FindItem("Wrench"), 2f, true, "Tampering"));
                dodgyStrikes = 0;
                dodgyVolume = 1f;
            }
            else if (buttonText == "VendorCart_Steal")
            {
                if (!agent.inventory.hasEmptySlot())
                {
                    agent.inventory.PlayerFullResponse(agent);
                    __instance.StopInteraction();

                    return false;
                }

                __instance.StartCoroutine(__instance.Operating(agent, null, 2f, false, "Tampering"));

                dodgyStrikes = 1;
                dodgyVolume = 0.2f;
            }

            if (dodgyVolume != 0f && !agent.statusEffects.hasTrait(vTrait.SneakyFingers) && __instance.functional)
			{
                GC.spawnerMain.SpawnNoise(__instance.tr.position, dodgyVolume, agent, "Normal", agent);
                GC.audioHandler.Play(__instance, "Hack");
                __instance.SpawnParticleEffect("Hack", __instance.tr.position);
                GC.spawnerMain.SpawnStateIndicator(__instance, "HighVolume");
                GC.OwnCheck(agent, __instance.go, "Normal", dodgyStrikes);
            }

            return false;
        }
        public static void ObjectReal_Start(ObjectReal __instance) // Postfix
        {
            //BunnyHeader.ConsoleMessage.LogMessage("ObjectReal_Start");

            if (__instance is Elevator elevator)
			{
                Elevator_Remora remora = new Elevator_Remora();
                Elevator_Variables[elevator] = remora;
                remora.elevatorHost = elevator;
			}
            else if (__instance is Stove stove)
            {
                Stove_Remora remora = new Stove_Remora();
                Stove_Variables[stove] = remora;
                remora.stoveHost = stove;
                Stove_Variables[(Stove)__instance].animateSpriteID = __instance.spr.GetSpriteIdByName("Stove");
                Stove_Variables[(Stove)__instance].animateSpriteID2 = __instance.spr.GetSpriteIdByName("Stove");
            }
            //else if (__instance is Refrigerator refrigerator)
            //{
            //    Refrigerator_Remora remora = new Refrigerator_Remora();
            //    Refrigerator_Variables[refrigerator] = remora;
            //    remora.refrigeratorHost = refrigerator;
            //}
        }
        #endregion
        #region PlayfieldObject
        public void PlayfieldObject_00()
		{
            Postfix(typeof(PlayfieldObject), "DetermineLuck", GetType(), "PlayfieldObject_DetermineLuck", new Type[3] { typeof(int), typeof(string), typeof(bool) });
            Prefix(typeof(PlayfieldObject), "Operating", GetType(), "PlayfieldObject_Operating", new Type[5] { typeof(Agent), typeof(InvItem), typeof(float), typeof(bool), typeof(string) });
            Prefix(typeof(PlayfieldObject), "playerHasUsableItem", GetType(), "PlayfieldObject_PlayerHasUsableItem", new Type[1] { typeof(InvItem) });
        }
        public static void PlayfieldObject_DetermineLuck(int originalLuck, string luckType, bool cancelStatusEffects, PlayfieldObject __instance, ref int __result) // Postfix
        {
            Agent agent = __instance.playfieldObjectAgent;

            int luckBonus = 0;
            int luckMultiplier = 0;
            bool RATStargetable = false;

            if (luckType == "FreeShopItem2")
                luckBonus = 10;
            else if (luckType == "DestroyGravestone")
                luckBonus = -5;
            else if (luckType == "TurnTables")
                luckBonus = 10;
            else if (luckType == "Joke")
                luckBonus = 10;
            else if (luckType == "CritChance")
            {
                luckBonus = 3;
                RATStargetable = true;
            }
            else if (luckType == "ChanceAttacksDoZeroDamage")
            {
                luckBonus = 4;
                RATStargetable = true;
            }
            else if (luckType == "DoorDetonator")
                luckBonus = 10;
            else if (luckType == "FreeShopItem")
                luckBonus = 10;
            else if (luckType == "FindThreat")
                luckBonus = 8;
            else if (luckType == "FindAskMayorHatPercentage")
                luckBonus = 8;
            else if (luckType == "ChanceToKnockWeapons")
            {
                luckBonus = 5;
                RATStargetable = true;
            }
            else if (luckType == "SlotMachine")
                luckBonus = 8;
            else if (luckType == "AttacksDamageAttacker")
                luckBonus = 10;
            else if (luckType == "Hack")
                luckBonus = 10;
            else if (luckType == "GunAim")
            {
                luckBonus = 5;
                RATStargetable = true;
            }
            else if (luckType == "SecurityCam")
                luckBonus = 10;
            else if (luckType == "FindAskPercentage")
                luckBonus = 8;
            else if (luckType == "ThiefToolsMayNotSubtract")
                luckBonus = 10;
            else if (luckType == "ChanceToSlowEnemies")
            {
                luckBonus = 4;
                RATStargetable = true;
            }

            if (agent.statusEffects.hasTrait(cTrait.Charmed))
                luckMultiplier = 1;
            else if (agent.statusEffects.hasTrait(cTrait.Charmed_2))
                luckMultiplier = 2;
            else if (agent.statusEffects.hasTrait(cTrait.Cursed))
                luckMultiplier = -1;
            else if (agent.statusEffects.hasTrait(cTrait.Cursed_2))
                luckMultiplier = -2;

            if (RATStargetable)
            {
                if (agent.statusEffects.hasTrait(cTrait.RATS))
                    luckMultiplier += 1;
                if (agent.statusEffects.hasTrait(cTrait.RATS_2))
                    luckMultiplier += 2;

                if (agent.isPlayer != 0 && agent.specialAbility == "ChronomanticDilation")
                    if (BMAbilities.MSA_CD_IsCast(agent))
                        luckMultiplier *= 2;
            }

            __result = Mathf.Clamp(__result + luckBonus * luckMultiplier, 0, 100);
        }
        public static bool PlayfieldObject_Operating(Agent myAgent, InvItem item, float timeToUnlock, bool makeNoise, string barType, PlayfieldObject __instance) // Prefix
        {
            try
            {
                BMLog("PlayfieldObject_Operating " + __instance.name + ": ");
                BMLog("\tAgent = " + myAgent.name);
                BMLog("\titem = " + item.invItemName);
                BMLog("\ttimeToUnlock = " + timeToUnlock);
                BMLog("\tmakeNoise = " + makeNoise);
                BMLog("\tbarType = " + barType);
            }
            catch
            {
                BMLog("Logging error");
            }

            return true;
        }
        public static bool PlayfieldObject_PlayerHasUsableItem(InvItem myItem, PlayfieldObject __instance) // Prefix
        {
            if (__instance is Stove)
            {
                Stove stove = (Stove)__instance;
                return (myItem.invItemName == "Wrench" || myItem.invItemName == "Fud")
                    && __instance.timer == 0f
                    && !stove.startedFlashing;
            }
            else
                return false;
        }
        #endregion

        #endregion

        #region Objects
        #region Alarm Button
        public void AlarmButton_00()
		{
            Prefix(typeof(AlarmButton), "DetermineButtons", GetType(), "AlarmButton_DetermineButtons", new Type[0] { });
            Prefix(typeof(AlarmButton), "DoLockdown", GetType(), "AlarmButton_DoLockdown", new Type[1] { typeof(bool) });
            Prefix(typeof(AlarmButton), "EndLockdown", GetType(), "AlarmButton_EndLockdown", new Type[0] { });
            Prefix(typeof(AlarmButton), "Interact", GetType(), "AlarmButton_Interact", new Type[1] { typeof(Agent) });
            Prefix(typeof(AlarmButton), "JustEndedLockdown", GetType(), "AlarmButton_JustEndedLockdown", new Type[0] { });
            Prefix(typeof(AlarmButton), "PressedButton", GetType(), "AlarmButton_PressedButton", new Type[2] { typeof(string), typeof(int) });
            Prefix(typeof(AlarmButton), "ToggleSwitch", GetType(), "AlarmButton_ToggleSwitch", new Type[2] { typeof(Agent), typeof(Agent) });
            //Pressedbutton
            //Toggleswitch
		}
        public static bool AlarmButton_DetermineButtons(AlarmButton __instance) // Replacement
		{
            BMLog("AlarmButton_DetermineButtons");

            MethodInfo determineButtons_base = AccessTools.DeclaredMethod(typeof(ObjectReal), "DetermineButtons", new Type[0] { });
            determineButtons_base.GetMethodWithoutOverrides<Action>(__instance).Invoke();

            if (__instance.interactingAgent.interactionHelper.interactingFar)
            {
                if (!__instance.hacked)
                    __instance.buttons.Add("AllAccessAlarmButton");

                if ((__instance.interactingAgent.oma.superSpecialAbility && __instance.interactingAgent.agentName == "Hacker") || __instance.interactingAgent.statusEffects.hasTrait("HacksBlowUpObjects"))
                    __instance.buttons.Add("HackExplode");
            }

            if (GC.challenges.Contains(cChallenge.AnCapistan) && !__instance.hacked)
			{
                if (!__instance.hacked)
                {
                    __instance.buttons.Add("AlarmButtonAncapistan");
                    __instance.buttonPrices.Add(25);
                }
                else
                    __instance.buttons.Add("AlarmButtonAncapistan");
            }

            return false;
        }
        public static bool AlarmButton_DoLockdown(bool closePanicRoomDoors, AlarmButton __instance) // Prefix
		{
            if (GC.challenges.Contains(cChallenge.AnCapistan))
            {
                if (AlarmButton.lockdownTimerCooldown > 0f)
                    return false;
                
                if (__instance.gc.serverPlayer)
                {
                    if (__instance.gc.lockdown)
                        AlarmButton.lockdownTimer = 7.5f;
                    else
                    {
                        AlarmButton.lockdownTimer = 7.5f;
                        AlarmButton.lockdownTimerBig = 40f;

                        if (!__instance.noLockdown)
                            __instance.gc.playerAgent.objectMult.ObjectAction(__instance.objectNetID, "DoLockdown");
                        
                        __instance.endLockDownGoing = true;
                        __instance.keepGoing = true;
                        __instance.firstTick = true;
                        __instance.mustKeepGoing = false;
                        __instance.InvokeRepeating("EndLockdownCoroutine", 0.01f, 0.1f);
                        
                        if (!__instance.noLockdown)
                            for (int i = 0; i < __instance.gc.agentList.Count; i++)
                                if (__instance.gc.agentList[i].pathing == 1)
                                {
                                    __instance.gc.agentList[i].pathfindingAI.rePath = true;
                                    __instance.gc.tileInfo.DoWandererRepath();
                                }
                    }
                }

                if (!__instance.noLockdown && __instance.gc.levelFeeling != "WarZone" && __instance.gc.levelFeeling != "Ooze" && __instance.gc.levelFeeling != "Riot" && __instance.gc.levelFeeling != "Zombies" && !__instance.gc.challenges.Contains("ZombieMutator") && !__instance.gc.lockdown)
                {
                    __instance.gc.audioHandler.Play(__instance.gc.playerAgent, "LockdownWallUp");

                    for (int j = 0; j < __instance.gc.lockdownWallList.Count; j++)
                        __instance.gc.lockdownWallList[j].SetWallUpAnim();
                }

                if (__instance.gc.serverPlayer && closePanicRoomDoors)
                {
                    int prison = __instance.gc.tileInfo.GetTileData(__instance.tr.position).prison;

                    for (int k = 0; k < __instance.gc.objectRealList.Count; k++)
                    {
                        ObjectReal objectReal = __instance.gc.objectRealList[k];

                        if (objectReal.startingChunk == __instance.startingChunk && objectReal.objectName == "Door" && prison != 0 && prison == __instance.gc.tileInfo.GetTileData(objectReal.tr.position).prison)
                        {
                            bool flag = true;

                            for (int l = 0; l < __instance.gc.agentList.Count; l++)
                            {
                                Agent agent = __instance.gc.agentList[l];

                                if (agent.curTileData.prison == prison && agent.curTileData.chunkID == __instance.startingChunk && (agent.ownerID != __instance.owner || agent.startingChunk != __instance.startingChunk) && !agent.upperCrusty)
                                    flag = false;
                            }

                            if (flag)
                                ((Door)objectReal).CloseDoor(null);
                        }
                    }
                }

                return false;
            }

            return true;
        }
        public static bool AlarmButton_EndLockdown(AlarmButton __instance) // Prefix
		{
            if (GC.challenges.Contains(cChallenge.AnCapistan))
            {
                if (AlarmButton.lockdownTimerCooldown <= 0f)
                    __instance.StartCoroutine(AlarmButton_LockdownCooldown(__instance));
                
                if (__instance.gc.levelTheme == 4 || __instance.gc.loadLevel.hasLockdownWalls)
                {
                    if (__instance.gc.serverPlayer)
                        __instance.gc.playerAgent.objectMult.ObjectAction(__instance.objectNetID, "EndLockdown");
                    
                    if (!__instance.noLockdown)
                        for (int i = 0; i < __instance.gc.agentList.Count; i++)
                            if (__instance.gc.agentList[i].pathing == 1)
                            {
                                __instance.gc.agentList[i].pathfindingAI.rePath = true;
                                __instance.gc.tileInfo.DoWandererRepath();
                            }
                    
                    if (__instance.gc.lockdown)
                        __instance.gc.audioHandler.Play(__instance.gc.playerAgent, "LockdownWallDown");
                    
                    for (int j = 0; j < __instance.gc.lockdownWallList.Count; j++)
                    {
                        LockdownWall lockdownWall = __instance.gc.lockdownWallList[j];
                    
                        if (lockdownWall.objectCollider.enabled)
                            lockdownWall.SetWallDownAnim();
                    }

                    for (int k = 0; k < __instance.gc.objectRealListWithDestroyed.Count; k++)
                    {
                        ObjectReal objectReal = __instance.gc.objectRealListWithDestroyed[k];
                    
                        if (objectReal.objectName == "AlarmButton" && objectReal != null)
                        {
                            int prison = __instance.gc.tileInfo.GetTileData(objectReal.tr.position).prison;
                        
                            for (int l = 0; l < __instance.gc.objectRealList.Count; l++)
                            {
                                ObjectReal objectReal2 = __instance.gc.objectRealList[l];
                            
                                if (objectReal2.startingChunk == objectReal.startingChunk && objectReal2.objectName == "Door" && prison != 0 && prison == __instance.gc.tileInfo.GetTileData(objectReal2.tr.position).prison)
                                    ((Door)objectReal2).OpenDoor(null);
                            }
                        }
                    }

                    __instance.StartCoroutine(AlarmButton_JustEndedLockdown(__instance));
                }

                return false;
            }

            return true;
        }
        public static bool AlarmButton_Interact(Agent agent, AlarmButton __instance) // Replacement
		{
            BMLog("AlarmButton_Interact");

			MethodInfo interact_base = AccessTools.DeclaredMethod(typeof(ObjectReal), "Interact", new Type[1] { typeof(Agent) });
			interact_base.GetMethodWithoutOverrides<Action<Agent>>(__instance).Invoke(agent);

			__instance.lastHitByAgent = __instance.interactingAgent;

            if (!__instance.isBroken())
            {
                if (GC.challenges.Contains(cChallenge.AnCapistan))
                {
                    __instance.ShowObjectButtons();

                    return false;
                }
                else
                {
                    if (agent.upperCrusty || __instance.hacked)
                        __instance.ToggleSwitch(__instance.interactingAgent, null);
                    else
                        __instance.Say("CantUseAlarmButton"); // base
                }
            }

            __instance.StopInteraction();

            return false;
        }
        public static IEnumerator AlarmButton_JustEndedLockdown(AlarmButton __instance) // Prefix
		{
            GC.justEndedLockdown = true;

            yield return new WaitForSeconds(0.4f);

            GC.justEndedLockdown = false;

            yield break;
        }
        public static IEnumerator AlarmButton_LockdownCooldown(AlarmButton __instance) // Non-Patch
		{
            AlarmButton.lockdownTimerCooldown = 5f;

            while (AlarmButton.lockdownTimerCooldown > 0f)
            {
                AlarmButton.lockdownTimerCooldown -= 0.1f;

                if (!GC.loadComplete)
                    AlarmButton.lockdownTimerCooldown = 0f;

                yield return new WaitForSeconds(0.1f);
            }

            AlarmButton.lockdownTimerCooldown = 0f;

            yield break;
        }
        public static bool AlarmButton_PressedButton(string buttonText, int buttonPrice, AlarmButton __instance) // Replacement
		{
            MethodInfo pressedButton_base = AccessTools.DeclaredMethod(typeof(ObjectReal), "Interact", new Type[2] { typeof(string), typeof(int) });
            pressedButton_base.GetMethodWithoutOverrides<Action<string, int>>(__instance).Invoke(buttonText, buttonPrice);

            if (buttonText == "AllAccessAlarmButton")
            {
                if ((!__instance.interactingAgent.oma.superSpecialAbility || !(__instance.interactingAgent.agentName == "Hacker")) && !__instance.interactingAgent.statusEffects.hasTrait("HacksBlowUpObjects"))
                    __instance.hackable = false;

                __instance.hacked = true;

                if (!__instance.gc.serverPlayer)
                    __instance.gc.playerAgent.objectMult.ObjectAction(__instance.objectNetID, "AllAccess");
            }
            else if (buttonText == "AlarmButtonAncapistan")
			{
                if (__instance.moneySuccess(buttonPrice))
                    __instance.ToggleSwitch(__instance.interactingAgent, null);
                else
                    __instance.interactingAgent.Say("I can't afford this. I should have worked harder!");
            }

            __instance.StopInteraction();

            return false;
        }
        public static IEnumerator AlarmButton_SetSwitchOn(AlarmButton __instance) // Non-Patch
		{
            __instance.switchOn = true;

            yield return new WaitForSeconds(0.5f);

            __instance.switchOn = false;

            yield break;
        }
        public static bool AlarmButton_ToggleSwitch(Agent causerAgent, Agent criminal, AlarmButton __instance) // Prefix
		{
            if (GC.challenges.Contains(cChallenge.AnCapistan))
			{
                if (GC.serverPlayer)
                {
                    if (causerAgent.isPlayer != 0)
                    {
                        if (causerAgent.hasProtector)
                        {
                            __instance.Say("AlarmAlreadyActive");
                            __instance.StopInteraction();
                        }
                        else if (causerAgent.spawnedSupercops >= 3 && !GC.challenges.Contains("NoLimits") && !GC.challenges.Contains(cChallenge.AnCapistan))
                        {
                            __instance.Say("AlarmLimitReached");
                            __instance.StopInteraction();
                        }
                        else
                        {
                            GC.audioHandler.Play(__instance, "AlarmButton");
                            __instance.SpawnEnforcer(causerAgent, criminal);
                        }
                    }
                    else if (!__instance.switchOn)
                    {
                        if (!__instance.switchOn)
                            __instance.StartCoroutine(AlarmButton_SetSwitchOn(__instance));
                        
                        if (!__instance.destroyed || __instance.destroying)
                            GC.audioHandler.Play(__instance, "AlarmButton");
                        
                        if (__instance.functional)
                        {
                            __instance.StartCoroutine(AlarmButton_WaitToBecomeActive(__instance));
                            __instance.SpawnEnforcer(causerAgent, criminal);
                        }
                        
                        if (!__instance.destroying)
                            causerAgent.usedAlarmButton = true;
                    }

                    __instance.DoLockdown(true);

                    return false;
                }
                else
                    __instance.interactingAgent.objectMult.ObjectAction(__instance.objectNetID, "ToggleSwitch");

                return false;
			}

            return true;
		}
        public static IEnumerator AlarmButton_WaitToBecomeActive(AlarmButton __instance) // Non-Patch
		{
            for (; ; )
                yield return null;

            yield break;
        }
        #endregion
        #region Bathtub
        public void BathTub_00()
		{
            Postfix(typeof(Bathtub), "SetVars", GetType(), "Bathtub_SetVars", new Type[0] { });
        }
        public static void Bathtub_SetVars(Bathtub __instance) // Postfix
        {
            BMLog("Bathtub_SetVars");

            if (BMTraits.IsPlayerTraitActive(cTrait.StealthBastardDeluxe))
			{
                BMLog("SBD trait detected");
                __instance.interactable = true;
            }

            //TODO: Closed Bath Curtain sprite?
            // See Generator.Start() for how to set animation sprites. Maybe just toggle sprite when used/unused.
        }
        #endregion
        #region Crate
        public void Crate_00()
		{
            Postfix(typeof(Crate), "DetermineButtons", GetType(), "Crate_DetermineButtons", new Type[0] { });
        }
        public static void Crate_DetermineButtons(Crate __instance) // Postfix
        {
            if (__instance.buttons.Any())
                CorrectButtonCosts(__instance);
        }
        #endregion
        #region Door
        public void Door_00()
		{
            Prefix(typeof(Door), "CloseDoor", GetType(), "Door_CloseDoor", new Type[2] { typeof(Agent), typeof(bool) });
            Postfix(typeof(Door), "DetermineButtons", GetType(), "Door_DetermineButtons", new Type[0] { });
            Prefix(typeof(Door), "OpenDoor", GetType(), "Door_OpenDoor", new Type[2] { typeof(Agent), typeof(bool) });
        }
        public static bool Door_CloseDoor(Agent myAgent, bool remote, Door __instance) // Replacement
		{
            if (GC.serverPlayer || GC.clientControlling || (__instance.immediateInteractions && (__instance.interactingAgent == GC.playerAgent || (myAgent != null && myAgent.oma.mindControlled))))
            {
                if (__instance.open && !__instance.destroyed)
                {
                    __instance.open = false;

                    if (myAgent != null)
                    {
                        if (myAgent.isPlayer > 0 && myAgent.localPlayer)
                            GC.audioHandler.Play(__instance, "DoorClose");
                        else
                        {
                            if (myAgent.oma.mindControlled && myAgent.mindControlAgent != null && myAgent.mindControlAgent.localPlayer)
                                GC.audioHandler.Play(__instance, "DoorClose");
                            else
                                GC.audioHandler.Play(__instance, "DoorCloseAI");
                        }
                    }
                    else
                        GC.audioHandler.Play(__instance, "DoorClose");

                    if (__instance.prisonDoor > 0 && __instance.locked)
                    {
                        __instance.graphUpdateScene.setTag = 5;
                        __instance.graphUpdateScene.Apply();
                    }
                    else if (__instance.locked)
                    {
                        __instance.graphUpdateScene.setTag = 4;
                        __instance.graphUpdateScene.Apply();
                    }

                    GC.tileInfo.DirtyWalls();
                    __instance.StartCoroutine(Door_CloseDoorAnim(__instance));
                    
                    if (__instance.objectShadow != null)
                        __instance.objectShadow.gameObject.SetActive(true);
                    
                    if (__instance.objectShadowCustom != null)
                        __instance.objectShadowCustom.gameObject.SetActive(true);
                    
                    __instance.GetComponent<BoxCollider2D>().enabled = true;
                    __instance.bulletsCanPass = false;
                    __instance.meleeCanPass = false;
                    __instance.lowInteractionPriority = false;
                    __instance.doorCooldown = 0.25f;

                    if (myAgent != null && myAgent.isPlayer > 0 && GC.serverPlayer && !__instance.outsideDoor && !myAgent.statusEffects.hasTrait(cTrait.StealthBastardDeluxe))
                        GC.spawnerMain.SpawnNoise(__instance.tr.position, 0.2f, null, null, myAgent);
                    
                    if (__instance.locked)
                    {
                        if (__instance.prisonObject > 0 && !__instance.prisonWallDown)
                            for (int i = 0; i < GC.agentList.Count; i++)
                            {
                                Agent agent = GC.agentList[i];
                                TileData tileData = GC.tileInfo.GetTileData(agent.tr.position);

                                if (tileData.chunkID == __instance.startingChunk && tileData.prison == __instance.prisonObject && tileData.prison > 0)
                                    agent.prisoner = __instance.prisonObject;
                            }
                        
                        if (__instance.prisonDoor > 0)
                        {
                            __instance.graphUpdateScene.setTag = 5;
                            __instance.PlaceInPrison();
                            __instance.graphUpdateScene.Apply();
                        }
                        else
                        {
                            __instance.graphUpdateScene.setTag = 4;
                            __instance.graphUpdateScene.Apply();
                        }
                    }
                    else if ((__instance.wasLocked || __instance.doorType == "DoorNoEntry") && __instance.outsideDoor)
                    {
                        __instance.graphUpdateScene.setTag = 8;
                        __instance.graphUpdateScene.Apply();
                    }

                    GC.tileInfo.ToggleDoorAtPosition(__instance.tr.position.x, __instance.tr.position.y, false, false);

                    try
                    {
                        if (__instance.lightObstacle == null)
                            __instance.lightObstacle = __instance.tr.Find("LightObstacle").gameObject;

                        if (__instance.hasLightObstacle && __instance.lightObstacle != null)
                            __instance.lightObstacle.SetActive(true);
                    }
                    catch { }
                    
                    GC.tileInfo.GetTileData(__instance.tr.position).wallSide = wallSideType.Door;
                    
                    if (GC.multiplayerMode && !GC.clientControlling)
                    {
                        if (!GC.serverPlayer)
                        {
                            if (myAgent != null)
                            {
                                if (myAgent.oma.mindControlled)
                                    GC.playerAgent.objectMult.ObjectAction(__instance.objectNetID, "CloseDoorMindControl", myAgent.objectNetID);
                                else
                                    myAgent.objectMult.ObjectAction(__instance.objectNetID, "CloseDoor");
                            }
                            else
                                GC.playerAgent.objectMult.ObjectAction(__instance.objectNetID, "CloseDoor");
                        }
                        else if (myAgent != null)
                            myAgent.objectMult.ObjectAction(__instance.objectNetID, "CloseDoorClient");
                        else
                            GC.playerAgent.objectMult.ObjectAction(__instance.objectNetID, "CloseDoorClient");
                    }

                    for (int j = 0; j < GC.playerAgentList.Count; j++)
                    {
                        Agent agent2 = GC.playerAgentList[j];
                    
                        if (agent2.localPlayer && agent2.interactionHelper.gameObject.activeSelf)
                            agent2.interactionHelper.StartCoroutine(agent2.interactionHelper.RefreshMe());
                    }

                    return false;
                }
            }
            else
            {
                if (myAgent != null)
                    myAgent.objectMult.ObjectAction(__instance.objectNetID, "CloseDoor");
                else
                    GC.playerAgent.objectMult.ObjectAction(__instance.objectNetID, "CloseDoor");
                
                __instance.StopInteraction();
            }

            return false;
        }
        public static IEnumerator Door_CloseDoorAnim(Door __instance) // Non-Patch
        {
            __instance.objectHitbox.GetComponent<MeshRenderer>().enabled = true;

            yield return new WaitForSeconds(0.02f);

            yield break;
        }
        public static void Door_DetermineButtons(Door __instance) // Postfix
        {
            if (__instance.buttons.Any())
                CorrectButtonCosts(__instance);
        }
        public static bool Door_OpenDoor(Agent myAgent, bool remote, Door __instance, bool ___usedTutorialKey) // Replacement
		{
            if (GC.serverPlayer || GC.clientControlling || (__instance.immediateInteractions && (__instance.interactingAgent == GC.playerAgent || (myAgent != null && myAgent.oma.mindControlled))))
            {
                if (__instance.hasDetonator)
                {
                    if (myAgent.ownerID != __instance.owner)
                        __instance.doorDetonatorAgent = myAgent;
                    
                    __instance.DoorDetonatorExplode();
                }
                else if (!__instance.open && !__instance.destroyed)
                {
                    __instance.open = true;

                    if (myAgent != null)
                    {
                        if (myAgent.isPlayer > 0)
                            __instance.noEntryCleared = true;
                    }
                    else
                        __instance.noEntryCleared = true;
                    
                    __instance.RemoveObjectAgent();
                    
                    if (myAgent != null)
                    {
                        if (myAgent.isPlayer > 0 && myAgent.localPlayer)
                            GC.audioHandler.Play(__instance, "DoorOpen");
                        else
                        {
                            if (myAgent.oma.mindControlled && myAgent.mindControlAgent != null && myAgent.mindControlAgent.localPlayer)
                                GC.audioHandler.Play(__instance, "DoorOpen");
                            else
                                GC.audioHandler.Play(__instance, "DoorOpenAI");
                        }
                    }
                    else if (GC.loadCompleteReally)
                        GC.audioHandler.Play(__instance, "DoorOpen");
                    
                    if (__instance.graphUpdateScene.setTag != 2)
                    {
                        __instance.graphUpdateScene.setTag = 2;
                        __instance.graphUpdateScene.Apply();
                    }
                    
                    GC.tileInfo.DirtyWalls();
                    GC.tileInfo.ToggleDoorAtPosition(__instance.tr.position.x, __instance.tr.position.y, false, true);
                    __instance.StartCoroutine(Door_OpenDoorAnim(__instance));

                    if (__instance.objectShadow != null)
                        __instance.objectShadow.gameObject.SetActive(false);
                    
                    if (__instance.objectShadowCustom != null)
                        __instance.objectShadowCustom.gameObject.SetActive(false);
                    
                    __instance.GetComponent<BoxCollider2D>().enabled = false;
                    __instance.objectSprite.sprH.color = ObjectSprite.clearColor;
                    __instance.bulletsCanPass = true;
                    __instance.meleeCanPass = true;
                    __instance.lowInteractionPriority = true;
                    __instance.doorCooldown = 0.25f;

                    if (GC.serverPlayer && !myAgent.statusEffects.hasTrait(cTrait.StealthBastardDeluxe))
                        GC.spawnerMain.SpawnNoise(__instance.tr.position, 0.2f, null, null, myAgent);
                    
                    bool flag3 = false;
                    
                    if (__instance.prisonDoor > 0)
                        flag3 = true;
                    
                    if (myAgent != null && !remote && !__instance.boughtKeyInChunk)
                    {
                        GC.OwnCheck(myAgent, __instance.go, "Door", 0);

                        if (__instance.prisonObject > 0 || __instance.doorType == "DoorNoEntry")
                        {
                            if (__instance.interactingAgent != null)
                            {
                                __instance.StartCoroutine(__instance.delayedOwnCheck(__instance.interactingAgent));
                                __instance.openingOwnCheck = true;
                            }
                            else if (myAgent != null && myAgent.oma.mindControlled)
                            {
                                __instance.StartCoroutine(__instance.delayedOwnCheck(myAgent));
                                __instance.openingOwnCheck = true;
                            }
                        }
                    }

                    if ((__instance.wasLocked || __instance.doorType == "DoorNoEntry") && __instance.outsideDoor)
                    {
                        __instance.graphUpdateScene.setTag = 8;
                        __instance.graphUpdateScene.Apply();
                    }

                    if (__instance.prisonObject > 0)
                    {
                        if (!__instance.prisonWallDown)
                            Door.freerAgent = myAgent;
                        
                        __instance.FreeFromPrison("Door");
                        
                        if (__instance.locked && __instance.extraVar != 10)
                            __instance.Unlock();
                    }

                    if (GC.levelType == "Tutorial" && __instance.interactingAgent == GC.playerAgent && __instance.wasLocked)
                    {
                        if (__instance.interactingAgent.inventory.HasItem("Key") && !___usedTutorialKey && (GC.tutorial.tutorialPart == "UseKeyOnDoor" || GC.tutorial.tutorialPart == "LoseHealth" || GC.tutorial.tutorialPart == "TranquilizeTarget"))
                        {
                            __instance.DestroyKeyItem("Key");
                            GC.tutorial.UseKeyOnDoor();
                            ___usedTutorialKey = true;
                        }
                    }
                    else if (__instance.wasLocked && !__instance.locked && ((__instance.prisonDoor > 0 && flag3) || __instance.startingChunkRealDescription == "Hotel"))
                        __instance.DestroyKeyItem("Key");

                    try
                    {
                        if (__instance.lightObstacle == null)
                            __instance.lightObstacle = __instance.tr.Find("LightObstacle").gameObject;
                        if (__instance.hasLightObstacle && __instance.lightObstacle.activeSelf)
                            __instance.lightObstacle.SetActive(false);
                    }
                    catch { }

                    GC.tileInfo.GetTileData(__instance.tr.position).wallSide = wallSideType.None;
                    __instance.StopInteraction();

                    if (__instance.fire != null && !__instance.fire.destroying)
                    {
                        __instance.fire.timeLeft = 0f;
                        __instance.fire.DestroyMe();
                    }
                    
                    for (int i = 0; i < GC.agentList.Count; i++)
                        if (GC.agentList[i].blockedByDoor == __instance)
                        {
                            GC.agentList[i].blockedByDoor = null;
                            GC.agentList[i].isBlockedByDoor = false;
                        }
                    
                    if (GC.multiplayerMode && !GC.clientControlling)
                    {
                        if (!GC.serverPlayer)
                        {
                            if (myAgent != null)
                            {
                                if (myAgent.oma.mindControlled)
                                    GC.playerAgent.objectMult.ObjectAction(__instance.objectNetID, "OpenDoorMindControl", myAgent.objectNetID);
                                else
                                    myAgent.objectMult.ObjectAction(__instance.objectNetID, "OpenDoor");
                            }
                            else
                                GC.playerAgent.objectMult.ObjectAction(__instance.objectNetID, "OpenDoor");
                        }
                        else if (myAgent != null)
                            myAgent.objectMult.ObjectAction(__instance.objectNetID, "OpenDoorClient");
                        else
                            GC.playerAgent.objectMult.ObjectAction(__instance.objectNetID, "OpenDoorClient");
                    }

                    if (GC.levelType == "HomeBase")
                    {
                        if (__instance.extraVar == 30)
                        {
                            for (int j = 0; j < GC.agentList.Count; j++)
                                if (GC.agentList[j].agentName == "ResistanceLeader" && GC.agentList[j].extraVar == 30)
                                {
                                    GC.agentList[j].Say(GC.nameDB.GetName("Tut_BathroomLine_1", "Dialogue"));
                                    GC.audioHandler.Play(GC.agentList[j], "AgentAnnoyed");
                                    GC.cinematics.wentInBathroom = true;
                                }
                        }
                        else if (__instance.extraVar == 31 && !GC.cinematics.playedResistanceLeaderIntro && GC.justFinishedTutorial)
                            GC.cinematics.ResistanceLeaderIntro();
                    }
                }
                for (int k = 0; k < GC.playerAgentList.Count; k++)
                {
                    Agent agent = GC.playerAgentList[k];
                    
                    if (agent.localPlayer && agent.interactionHelper.gameObject.activeSelf)
                        agent.interactionHelper.StartCoroutine(agent.interactionHelper.RefreshMe());
                }

                return false;
            }

            if (myAgent != null)
                myAgent.objectMult.ObjectAction(__instance.objectNetID, "OpenDoor");
            else
                GC.playerAgent.objectMult.ObjectAction(__instance.objectNetID, "OpenDoor");
            
            __instance.StopInteraction();

            return false;
        }
        public static IEnumerator Door_OpenDoorAnim(Door __instance) // Non-Patch
		{
            __instance.objectHitbox.GetComponent<MeshRenderer>().enabled = false;

            yield return new WaitForSeconds(0.02f);

            yield break;
        }
        #endregion
        #region Elevator
        public void Elevator_00()
		{
            Prefix(typeof(Elevator), "DetermineButtons", GetType(), "Elevator_DetermineButtons", new Type[0] { });
            Prefix(typeof(Elevator), "PressedButton", GetType(), "Elevator_PressedButton", new Type[1] { typeof(string) });
		}
        public static bool Elevator_DetermineButtons(Elevator __instance) // Prefix
		{
            if (GC.challenges.Contains(cChallenge.AnCapistan))
			{
                MethodInfo determineButtons_base = AccessTools.DeclaredMethod(typeof(ObjectReal), "DetermineButtons", new Type[0] { });
                determineButtons_base.GetMethodWithoutOverrides<Action>(__instance).Invoke();

                if (Elevator_Variables[__instance].ticketPurchased)
                    __instance.buttons.Add("ElevatorGoUp");
				else
                {
                    __instance.buttons.Add("Elevator_PurchaseTicket");
                    __instance.buttonPrices.Add(50);
                }

                return false;
			}

            return true;
		}
        public static bool Elevator_PressedButton(string buttonText, Elevator __instance, ref bool ___showingSecondButtonSet) // Prefix
		{
            if (GC.challenges.Contains(cChallenge.AnCapistan))
			{
                MethodInfo pressedButton_base = AccessTools.DeclaredMethod(typeof(ObjectReal), "PressedButton", new Type[1] { typeof(string) });
                pressedButton_base.GetMethodWithoutOverrides<Action<string>>(__instance).Invoke(buttonText);

                if (buttonText == "StartTutorial")
                {
                    GC.challenges.Clear();
                    GC.SetDailyRunText();
                    GC.sessionDataBig.coopMode = false;
                    GC.sessionDataBig.fourPlayerMode = false;
                    GC.sessionDataBig.threePlayer = false;
                    GC.sessionDataBig.newCharacter = "Hobo";
                    GC.loadLevel.RestartGame(101);

                    return false;
                }
                else if (buttonText == "Elevator_PurchaseTicket")
                    Elevator_PurchaseTicket(__instance);

                if (!(buttonText == "ElevatorGoUp"))
                {
                    __instance.StopInteraction();

                    return false;
                }

                Agent interactingAgent = __instance.interactingAgent;

                if (__instance.BigQuestRunning(interactingAgent) && !___showingSecondButtonSet)
                {
                    ___showingSecondButtonSet = true;
                    __instance.RefreshButtons();
                    __instance.SetObjectNameDisplay(interactingAgent);

                    return false;
                }

                __instance.StopInteraction();
                GC.exitPoint.TryToExit(interactingAgent);
                interactingAgent.mainGUI.invInterface.justPressedInteract = false;

                return false;
			}

            return true;
		}
        public static void Elevator_PurchaseTicket(Elevator __instance)
		{
            if (__instance.moneySuccess(50))
            {
                Elevator_Variables[__instance].ticketPurchased = true;
                //__instance.PlayAnim("MachineOperate", __instance.interactingAgent);
                GC.audioHandler.Play(__instance.interactingAgent, vAudioClip.ATMDeposit);
                __instance.Say("PAYMENT ACCEPTED - Thank you for using Evilator Co.!");
            }
            else
			{
                __instance.Say("PAYMENT DECLINED - Have a [ERR: Salutation not found (\"RejectPoorSwine\")] Day.");

                MethodInfo stopInteraction_base = AccessTools.DeclaredMethod(typeof(ObjectReal), "StopInteraction", new Type[0] { });
                stopInteraction_base.GetMethodWithoutOverrides<Action>(__instance).Invoke();

                return;
            }
        }
        public static Dictionary<Elevator, Elevator_Remora> Elevator_Variables = new Dictionary<Elevator, Elevator_Remora>();
        #endregion
        #region Fire Hydrant
        public void FireHydrant_00()
		{
            Prefix(typeof(FireHydrant), "DetermineButtons", GetType(), "FireHydrant_DetermineButtons", new Type[0] { });
            Prefix(typeof(FireHydrant), "PressedButton", GetType(), "FireHydrant_PressedButton", new Type[2] { typeof(string), typeof(int) });
		}
        public static bool FireHydrant_DetermineButtons(FireHydrant __instance) // Prefix
        {
            if (GC.challenges.Contains(cChallenge.AnCapistan))
            {
                MethodInfo determineButtons_base = AccessTools.DeclaredMethod(typeof(ObjectReal), "DetermineButtons", new Type[0] { });
                determineButtons_base.GetMethodWithoutOverrides<Action>(__instance).Invoke();

                if (__instance.interactingAgent.statusEffects.hasSpecialAbility("WaterCannon"))
                {

                    __instance.buttons.Add("RefillWaterCannon");
                    __instance.buttonPrices.Add(10);

                    return false;
                }

                __instance.interactingAgent.SayDialogue("CantUseFireHydrant");

                return false;
            }

            return true;
        }
        public static bool FireHydrant_PressedButton(string buttonText, int buttonPrice, FireHydrant __instance) // Prefix
		{
            if (GC.challenges.Contains(cChallenge.AnCapistan))
			{
                MethodInfo pressedButton_base = AccessTools.DeclaredMethod(typeof(PlayfieldObject), "PressedButton", new Type[2] { typeof(string), typeof(int) });
                pressedButton_base.GetMethodWithoutOverrides<Action<string, int>>(__instance).Invoke(buttonText, buttonPrice);

                if (buttonText == "RefillWaterCannon")
                {
                    if (__instance.moneySuccess(buttonPrice))
                    {
                        __instance.RefillWaterCannon();
                        __instance.StopInteraction();
                    }

                    return false;
                }

                __instance.StopInteraction();

                return false;
			}

            return true;
		}
        #endregion
        #region FlamingBarrel
        public void FlamingBarrel_00()
		{
            Postfix(typeof(FlamingBarrel), "SetVars", GetType(), "FlamingBarrel_SetVars", new Type[0] { });
        }
        public static void FlamingBarrel_GrilledFud(FlamingBarrel __instance) // Non-patch
        {
            InvItem rawFud = __instance.interactingAgent.inventory.FindItem("Fud");

            int numCooked = rawFud.invItemCount;

            __instance.interactingAgent.inventory.SubtractFromItemCount(rawFud, numCooked);

            InvItem hotFud = new InvItem();

            hotFud.invItemName = "HotFud";
            hotFud.invItemCount = numCooked;
            hotFud.SetupDetails(false);
            __instance.interactingAgent.inventory.AddItemOrDrop(hotFud);
            hotFud.ShowPickingUpText(__instance.interactingAgent);

            GC.spawnerMain.SpawnNoise(__instance.curPosition, 1f, null, null, __instance.lastHitByAgent);
            GC.audioHandler.Play(__instance, "Grill");
            FlamingBarrel_GrilledFudAfter(numCooked, __instance);
        }
        public static void FlamingBarrel_GrilledFudAfter(int myCount, FlamingBarrel __instance) // Non-patch
        {
            Agent agent = __instance.interactingAgent;

            GC.audioHandler.Play(__instance, "FireHit");

            if (agent.statusEffects.hasTrait(vTrait.ResistFire) || agent.statusEffects.hasTrait(vTrait.FireproofSkin) || agent.statusEffects.hasTrait(vTrait.FireproofSkin2))
                agent.Say("Mmmm, toasty. Just like the burning flesh on my fingers!");
			else
			{
                agent.statusEffects.ChangeHealth(-10f, __instance);
                agent.Say("God fucking damn it, I always fucking burn my fucking hands!");
            }

            return;
        }
        public static void FlamingBarrel_SetVars(FlamingBarrel __instance) // Postfix
        {
            __instance.interactable = true;
            __instance.fireDoesntDamage = true;
        }
        #endregion
        #region Generator 
        public void Generator_00()
		{
            Postfix(typeof(Generator), "DetermineButtons", GetType(), "Generator_DetermineButtons", new Type[0] { });
        }
        public static void Generator_DetermineButtons(Generator __instance) // Postfix
        {
            if (__instance.buttons.Any())
                CorrectButtonCosts(__instance);
        }
        #endregion
        #region Generator2
        public void Generator2_00()
        {
            Postfix(typeof(Generator2), "DetermineButtons", GetType(), "Generator2_DetermineButtons", new Type[0] { });
        }
        public static void Generator2_DetermineButtons(Generator2 __instance) // Postfix
        {
            if (__instance.buttons.Any())
                CorrectButtonCosts(__instance);
        }
        #endregion
        #region Hole
        public void Hole_00()
        {
            Prefix(typeof(Hole), "EnterRange", GetType(), "Hole_EnterRange", new Type[1] { typeof(GameObject) });
        }
        public static bool Hole_EnterRange(GameObject myObject, Hole __instance) // Prefix
		{
            if (GC.loadComplete)
                if (myObject.CompareTag("Agent"))
                {
                    Agent agent = myObject.GetComponent<Agent>();

                    if (__instance.GetComponent<ObjectMultHole>().objectHoleType == "Manhole" && agent.statusEffects.hasTrait(cTrait.UnderdarkCitizen) && !agent.statusEffects.hasStatusEffect(vStatusEffect.Giant))
                    {
                        Manhole_FlushYourself(agent, (Manhole)__instance.GetComponent<ObjectReal>());

                        return false;
                    }
                }

            return true;
		}
        #endregion
        #region LaserEmitter
        public void LaserEmitter_00()
        {
            Postfix(typeof(LaserEmitter), "DetermineButtons", GetType(), "LaserEmitter_DetermineButtons", new Type[0] { });
        }
        public static void LaserEmitter_DetermineButtons(LaserEmitter __instance) // Postfix
        {
            if (__instance.buttons.Any())
                CorrectButtonCosts(__instance);
        }
        #endregion
        #region Manhole
        public void Manhole_00()
        {
            Postfix(typeof(Manhole), "SetVars", GetType(), "Manhole_SetVars", new Type[0] { });
            Prefix(typeof(Manhole), "Start", GetType(), "Manhole_Start", new Type[0] { });
        }
        public static void Manhole_FlushYourself(Agent agent, ObjectReal __instance) // Non-Patch
        {
            List<ObjectReal> exits = new List<ObjectReal>();

            for (int i = 0; i < GC.objectRealList.Count; i++)
            {
                ObjectReal objectReal = GC.objectRealList[i];

                if (objectReal == __instance)
                    continue;
                else if (objectReal.objectName == vObject.Manhole)
                {
                    Manhole manhole = (Manhole)objectReal;

                    if (manhole.opened)
                        exits.Add(objectReal);
                }
                else if (objectReal.objectName == vObject.Toilet && (agent.statusEffects.hasTrait(vTrait.Diminutive) || agent.shrunk))
                    if (objectReal.destroyed)
                        exits.Add(objectReal);
            }

            ObjectReal exit = __instance;

            if (exits.Count > 0)
                exit = exits[Random.Range(0, exits.Count)];

            Vector3 exitSpot = Vector3.zero;

            if (exit is Manhole)
            {
                exitSpot = exit.curPosition;
                Vector2 offset = Random.insideUnitCircle.normalized;
                agent.Teleport((Vector2)exitSpot + offset, exit.objectName == vObject.Manhole, false);
                GC.spawnerMain.SpawnExplosion((PlayfieldObject)exit, exitSpot, "Water", false, -1, false, exit.FindMustSpawnExplosionOnClients(agent));

            }
            else if (exit is Toilet)
            {
                string direction = exit.direction;

                switch (direction)
                {
                    case "E":
                        exitSpot = new Vector3(exit.tr.position.x + 0.32f, exit.tr.position.y, exit.tr.position.z);

                        break;

                    case "N":
                        exitSpot = new Vector3(exit.tr.position.x, exit.tr.position.y + 0.32f, exit.tr.position.z);

                        break;

                    case "S":
                        exitSpot = new Vector3(exit.tr.position.x, exit.tr.position.y - 0.32f, exit.tr.position.z);

                        break;

                    case "W":
                        exitSpot = new Vector3(exit.tr.position.x - 0.32f, exit.tr.position.y, exit.tr.position.z);

                        break;
                }

                __instance.interactingAgent.Teleport(exitSpot, false, true);
                GC.spawnerMain.SpawnExplosion(__instance.interactingAgent, exit.tr.position, "Water", false, -1, false, __instance.FindMustSpawnExplosionOnClients(__instance.interactingAgent));
            }
        }
        public static void Manhole_FlushYourself(Manhole __instance) // Non-Patch
		{
            Agent agent = __instance.interactingAgent;

            List<ObjectReal> exits = new List<ObjectReal>();

            for (int i = 0; i < GC.objectRealList.Count; i++)
            {
                ObjectReal objectReal = GC.objectRealList[i];

                if (objectReal == __instance)
                    continue;
                else if (objectReal.objectName == vObject.Manhole)
                {
                    Manhole manhole = (Manhole)objectReal;

                    if (manhole.opened)
                        exits.Add(objectReal);
                }
                else if (objectReal.objectName == vObject.Toilet && (agent.statusEffects.hasTrait(vTrait.Diminutive) || agent.shrunk))
                    if (!objectReal.destroyed)
                        exits.Add(objectReal);
            }

            ObjectReal exit = __instance;

            if (exits.Count > 0)
                exit = exits[Random.Range(0, exits.Count)];

            GC.audioHandler.Play(agent, "ToiletTeleportIn");
            agent.toiletTeleporting = true;

            Vector3 exitSpot = Vector3.zero;

            if (exit is Manhole)
            {
                exitSpot = exit.curPosition;
                Vector2 offset = Random.insideUnitCircle.normalized;
                agent.Teleport((Vector2)exitSpot + offset, exit.objectName == vObject.Manhole, false);
                GC.spawnerMain.SpawnExplosion((PlayfieldObject)exit, exitSpot, "Water", false, -1, false, exit.FindMustSpawnExplosionOnClients(agent));

            }
            else if (exit is Toilet)
			{
                string direction = exit.direction;

                switch (direction)
                {
                    case "E":
                        exitSpot = new Vector3(exit.tr.position.x + 0.32f, exit.tr.position.y, exit.tr.position.z);

                        break;

                    case "N":
                        exitSpot = new Vector3(exit.tr.position.x, exit.tr.position.y + 0.32f, exit.tr.position.z);

                        break;

                    case "S":
                        exitSpot = new Vector3(exit.tr.position.x, exit.tr.position.y - 0.32f, exit.tr.position.z);

                        break;

                    case "W":
                        exitSpot = new Vector3(exit.tr.position.x - 0.32f, exit.tr.position.y, exit.tr.position.z);

                        break;
                }

                __instance.interactingAgent.Teleport(exitSpot, false, true);
                GC.spawnerMain.SpawnExplosion(__instance.interactingAgent, exit.tr.position, "Water", false, -1, false, __instance.FindMustSpawnExplosionOnClients(__instance.interactingAgent));
            }
        }
        public static IEnumerator Manhole_HoleAppearAfterLoad(Manhole __instance) // Non-Patch
		{
            if (GC.streamingWorld)
                while (!GC.streamingWorldController.StreamingLoaded(__instance.startingChunk, __instance.startingChunkReal))
                    yield return null;
            else
                while (!GC.loadCompleteEarly)
                    yield return null;
            
            __instance.HoleAppear();
            yield break;
        }
		public static void Manhole_SetVars(Manhole __instance) // Postfix
		{
            __instance.interactable = true;
		}
        public static IEnumerator Manhole_SetLightingLater(Manhole __instance) // Non-Patch
		{
            yield return null;
            yield return null;

            if (!__instance.gc.serverPlayer)
            {
                while (__instance.tr.Find("LightReal(Clone)") == null)
                    yield return null;
                
                __instance.lightReal = __instance.tr.Find("LightReal(Clone)").GetComponent<LightReal>();
            }

            __instance.lightReal.tr.localPosition = new Vector3(0f, -0.12f);

            yield break;
        }
        public static bool Manhole_Start(Manhole __instance) // Replacement
		{
            MethodInfo start_base = AccessTools.DeclaredMethod(typeof(ObjectReal), "Start", new Type[0] { });
            start_base.GetMethodWithoutOverrides<Action>(__instance).Invoke();

            if (GC.levelTheme != 3 && !GC.challenges.Contains(vChallenge.MixedUpLevels) && GC.levelFeeling != vLevelFeeling.WarZone && !BMTraits.IsPlayerTraitActive(cTrait.UnderdarkCitizen) && GC.serverPlayer)
            {
                __instance.objectName = "Manhole";
                __instance.RemoveMe();

                return false;
            }

            if (__instance.opened)
                __instance.objectSprite.meshRenderer.enabled = false;
            
            if (GC.lightingType != "None")
                __instance.StartCoroutine(Manhole_SetLightingLater(__instance));

            GC.tileInfo.GetTileData(__instance.tr.position).futureHole = true;
            
            if (GC.serverPlayer && GC.levelFeeling == "WarZone")
            {
                __instance.StartCoroutine(Manhole_HoleAppearAfterLoad(__instance));
            
                return false;
            }

            if (!GC.serverPlayer && __instance.normalHole)
                __instance.objectRealRealName = GC.nameDB.GetName("Hole", "Object");

            return false;
        }
        public static void Manhole_UseCrowbar(Manhole __instance) // Non-Patch
        {
            BMLog("Manhole_UseCrowbar");

            if (GC.serverPlayer)
            {
                Vector3 position = __instance.tr.position;
                position = new Vector3(__instance.tr.position.x, __instance.tr.position.y - 0.24f, __instance.tr.position.z);
                
                __instance.hole = GC.spawnerMain.SpawnHole(__instance, position, new Vector3(1.5f, 1.5f, 1f), Quaternion.identity, false, true);
                __instance.hole.ObjectHoleAppear("Manhole");
                GC.playerAgent.objectMult.ObjectAction(__instance.objectNetID, "HoleAppear");
                __instance.operatingAgent.inventory.SubtractFromItemCount(__instance.operatingItem, 15); // SubtractFromItemCount is Postfixed! ToolCost is already in there.
            }

            __instance.objectSprite.meshRenderer.enabled = false;
            __instance.opened = true;
            __instance.SetSDangerousToWalk(true);
            GC.audioHandler.Play(__instance, "ManholeOpen");

            if (GC.levelFeeling == "WarZone")
            {
                __instance.objectRealRealName = GC.nameDB.GetName("Hole", "Object");
                __instance.normalHole = true;
            }
        }
        #endregion
        #region Plant
        public void Plant_00()
        {
            Postfix(typeof(Plant), "SetVars", GetType(), "Plant_SetVars", new Type[0] { });
        }
        public static void Plant_SetVars(Plant __instance) // Postfix
        {
            if (BMTraits.IsPlayerTraitActive(cTrait.StealthBastardDeluxe))
                __instance.interactable = true;
            //__instance.lowInteractionPriority = true;
        }
        #endregion
        #region PoliceBox
        public void PoliceBox_00()
        {
            Postfix(typeof(PoliceBox), "DetermineButtons", GetType(), "PoliceBox_DetermineButtons", new Type[0] { });
        }
        public static void PoliceBox_DetermineButtons(PoliceBox __instance) // Postfix
        {
            if (__instance.buttons.Any())
                CorrectButtonCosts(__instance);
        }
        #endregion
        #region PoolTable
        public void PoolTable_00()
        {
            Postfix(typeof(PoolTable), "SetVars", GetType(), "PoolTable_SetVars", new Type[0] { });
        }
        public static void PoolTable_SetVars(PoolTable __instance) // Postfix
        {
            if (BMTraits.IsPlayerTraitActive(cTrait.StealthBastardDeluxe))
                __instance.interactable = true;
            //__instance.lowInteractionPriority = true;
        }
        #endregion
        #region Refrigerator
        public void Refrigerator_00()
        {
            //BunnyHeader.MainInstance.PatchPrefix(typeof(Refrigerator), "DetermineButtons", GetType(), "Refrigerator_DetermineButtons");
            //BunnyHeader.MainInstance.PatchPostfix(typeof(Refrigerator), "FinishedOperating", GetType(), "Refrigerator_FinishedOperating");
            //BunnyHeader.MainInstance.PatchPrefix(typeof(Refrigerator), "Interact", GetType(), "Refrigerator_Interact", new Type[1] { typeof(Agent) });
            //BunnyHeader.MainInstance.PatchPrefix(typeof(Refrigerator), "InteractFar", GetType(), "Refrigerator_InteractFar", new Type[1] { typeof(Agent) });
            //BunnyHeader.MainInstance.PatchPrefix(typeof(Refrigerator), "ObjectAction", GetType(), "Refrigerator_ObjectAction", new Type[5] { typeof(string), typeof(string), typeof(float), typeof(Agent), typeof(PlayfieldObject) });
            //BunnyHeader.MainInstance.PatchPrefix(typeof(Refrigerator), "PressedButton", GetType(), "Refrigerator_PressedButton", new Type[1] { typeof(string) });
        }
        //      public static IEnumerator Refrigerator_AboutToRun(Refrigerator __instance) // Non-Patch
        //      {
        //          BunnyHeader.ConsoleMessage.LogMessage(__instance.name + ": " + MethodBase.GetCurrentMethod().Name);

        //          __instance.interactable = false;

        //          __instance.PlayObjectSpriteEffect("FlashingRepeatedly");

        //          Vector3 particlePosition = new Vector3(__instance.tr.position.x, __instance.tr.position.y + 0.36f, __instance.tr.position.z);
        //          __instance.SpawnParticleEffect("Smoke", particlePosition);
        //          __instance.PlayAnim("MachineGoingToExplode", gc.playerAgent);
        //          __instance.RemoveObjectAgent();
        //          __instance.cantMakeFollowersAttack = true;

        //          yield return new WaitForSeconds(3f);
        //          if (!__instance.destroying)
        //              __instance.DestroyMe(Refrigerator_Variables[__instance].savedDamagerObject);

        //          yield break;
        //      }
        //      public static void Refrigerator_AnimationSequence(Refrigerator __instance) // Non-Patch
        //{
        //          BunnyHeader.ConsoleMessage.LogMessage(__instance.name + ": " + MethodBase.GetCurrentMethod().Name);

        //          Refrigerator_Remora remora = Refrigerator_Variables[(Refrigerator)__instance];

        //          if (!__instance.destroying && __instance.activeObject && !__instance.notInOriginalLocation && __instance.spawnedShadow && __instance.onCamera)
        //          {
        //              remora.animationCountdown -= Time.deltaTime;
        //              if (remora.animationCountdown <= 0f)
        //              {
        //                  if (remora.animationFrame == 0)
        //                  {
        //                      __instance.ChangeSpriteByID(remora.animateSpriteID2);
        //                      remora.animationFrame = 1;
        //                  }
        //                  else
        //                  {
        //                      __instance.ChangeSpriteByID(remora.animateSpriteID);
        //                      remora.animationFrame = 0;
        //                  }
        //                  remora.animationCountdown = 0.5f;
        //              }
        //          }
        //      }
        //      public static bool Refrigerator_DetermineButtons(Refrigerator __instance) // Postfix
        //{
        //          BunnyHeader.ConsoleMessage.LogMessage(__instance.name + ": " + MethodBase.GetCurrentMethod().Name);

        //          Agent agent = __instance.interactingAgent;

        //          MethodInfo DetermineButtons_Base = AccessTools.DeclaredMethod(typeof(ObjectReal), "DetermineButtons", new Type[0]{});
        //          DetermineButtons_Base.GetMethodWithoutOverrides<Action>(__instance).Invoke();

        //          if (Refrigerator_Variables[__instance].wasHacked)
        //	{
        //              __instance.buttons.Add("RefrigeratorRun");

        //              if ((__instance.interactingAgent.oma.superSpecialAbility && __instance.interactingAgent.agentName == "Hacker") || __instance.interactingAgent.statusEffects.hasTrait("HacksBlowUpObjects"))
        //                  __instance.buttons.Add("HackExplode");
        //          }

        //          if (!Refrigerator_Variables[__instance].wasHacked && __instance.interactingAgent.inventory.HasItem("Wrench"))
        //          {

        //              __instance.buttons.Add("DispenseIce");
        //              __instance.buttonsExtra.Add(" (" + __instance.interactingAgent.inventory.FindItem("Wrench").invItemCount + ") -" + BunnyTraits.ToolCost(__instance.interactingAgent));

        //              BunnyHeader.ConsoleMessage.LogMessage("Button error?");

        //		__instance.buttons.Add("OpenContainer");
        //          }
        //	else
        //              __instance.ShowChest();

        //          return false;
        //}
        //      public static void Refrigerator_FinishedOperating(Refrigerator __instance) // PostFix
        //{
        //          BunnyHeader.ConsoleMessage.LogMessage(__instance.name + ": " + MethodBase.GetCurrentMethod().Name);

        //          if (__instance.operatingItem.invItemName == "Wrench")
        //	{
        //              Refrigerator_IceDispense(__instance);
        //              __instance.StopInteraction();
        //	}
        //}
        //public static IEnumerator Refrigerator_IceDispense(Refrigerator __instance)
        //{
        //          BunnyHeader.ConsoleMessage.LogMessage(__instance.name + ": " + MethodBase.GetCurrentMethod().Name);

        //          //__instance.StopItemAudio();
        //          int num;

        //	for (int i = 0; i < 10; i = num + 1)
        //	{
        //		Refrigerator_IceShot(0, 0, null, __instance);
        //		yield return new WaitForSeconds(UnityEngine.Random.Range(0.3f, 0.6f));
        //		num = i;
        //	}

        //	Explosion explosion = gc.spawnerMain.SpawnExplosion(__instance, __instance.tr.position, "Normal", false, -1, false, true);
        //	explosion.agent = __instance.interactingAgent;
        //	explosion.realSource = __instance;
        //	__instance.DestroyMe();
        //	yield break;
        //}
        //public static void Refrigerator_IceShot(int bulletNetID, int bulletRotation, Agent fireworksOwner, Refrigerator __instance)
        //{
        //          BunnyHeader.ConsoleMessage.LogMessage(__instance.name + ": " + MethodBase.GetCurrentMethod().Name);

        //          //if (fireworksOwner != null)
        //          //{
        //          //	__instance.owner = fireworksOwner;
        //          //}
        //          Bullet bullet = gc.spawnerMain.SpawnBullet(__instance.tr.position, bulletStatus.FreezeRay, __instance);
        //	float angle;
        //	if (gc.serverPlayer)
        //	{
        //		angle = (float)UnityEngine.Random.Range(0, 359);
        //	}
        //	else
        //	{
        //		angle = (float)bulletRotation;
        //	}
        //	bullet.movement.RotateToAngleTransform(angle);
        //	bullet.movement.MoveForwardTransform(0.48f);
        //	gc.audioHandler.Play(__instance, "FireworksFire");
        //	if (gc.multiplayerMode && gc.serverPlayer)
        //	{
        //		bullet.bulletNetID = UnityEngine.Random.Range(0, 10000);
        //		NetworkInstanceId extraObjectID = NetworkInstanceId.Invalid;
        //		if (fireworksOwner != null) // Always true
        //		{
        //			extraObjectID = __instance.interactingAgent.objectNetID;
        //		}
        //		gc.playerAgent.objectMult.ObjectAction(__instance.objectNetID, "FireworksFire", angle.ToString(), (float)bulletNetID, extraObjectID);
        //	}
        //}
        //public static bool Refrigerator_Interact(Agent agent, Refrigerator __instance) // Replacement
        //      {
        //          BunnyHeader.ConsoleMessage.LogMessage(__instance.name + ": " + MethodBase.GetCurrentMethod().Name);

        //          MethodInfo interact_base = AccessTools.DeclaredMethod(typeof(ObjectReal), "Interact", new Type[1] { typeof(Agent) });
        //          interact_base.GetMethodWithoutOverrides<Action<Agent>>(__instance).Invoke(agent);

        //          if (__instance.timer > 0f || __instance.startedFlashing)
        //              __instance.StopInteraction();

        //          if (agent.inventory.HasItem("Wrench"))
        //              __instance.ShowObjectButtons();

        //          else
        //              __instance.ShowChest();

        //          return false;
        //      }
        //      public static bool Refrigerator_InteractFar(Agent agent, Refrigerator __instance) // Prefix
        //{
        //          BunnyHeader.ConsoleMessage.LogMessage(__instance.name + ": " + MethodBase.GetCurrentMethod().Name);

        //          if (__instance.timer > 0f || __instance.startedFlashing)
        //          {
        //              __instance.StopInteraction();
        //              return false;
        //          }
        //          Refrigerator_Variables[__instance].wasHacked = true;
        //          return true;
        //      }
        //      public static void Refrigerator_ObjectAction(string myAction, string extraString, float extraFloat, Agent causerAgent, PlayfieldObject extraObject, Refrigerator __instance, ref bool ___noMoreObjectActions) // Replacement 
        //{
        //          BunnyHeader.ConsoleMessage.LogMessage(__instance.name + ": " + MethodBase.GetCurrentMethod().Name);

        //          MethodInfo ObjectAction_Base = AccessTools.DeclaredMethod(typeof(ObjectReal), "ObjectAction", new Type[5] { typeof(string), typeof(string), typeof(float), typeof(Agent), typeof(PlayfieldObject) });
        //          ObjectAction_Base.GetMethodWithoutOverrides<Action<string, string, float, Agent, PlayfieldObject>>(__instance).Invoke(myAction, extraString, extraFloat, causerAgent, extraObject);

        //          if (!___noMoreObjectActions)
        //          {
        //              if (myAction == "OpenContainer")
        //                  __instance.ShowChest();
        //              if (!(myAction == "RefrigeratorRun"))
        //                  if (myAction == "RefrigeratorRunClients")
        //                      __instance.RefrigeratorRunClients();
        //              else
        //                  __instance.RefrigeratorRun(causerAgent);
        //          }
        //          ___noMoreObjectActions = false;
        //      }
        //      public static bool Refrigerator_PressedButton(string buttonText, Refrigerator __instance) // Replacement
        //{
        //          BunnyHeader.ConsoleMessage.LogMessage(__instance.name + ": " + MethodBase.GetCurrentMethod().Name);

        //          MethodInfo PressedButton_Base = AccessTools.DeclaredMethod(typeof(ObjectReal), "PressedButton", new Type[1] { typeof(string) });
        //          PressedButton_Base.GetMethodWithoutOverrides<Action<string>>(__instance).Invoke(buttonText);

        //          if (buttonText == "RefrigeratorRun" && __instance.interactingAgent.interactionHelper.interactingFar)
        //          {
        //              BunnyHeader.ConsoleMessage.LogMessage(__instance.name + ": " + "Remote Run");

        //              gc.audioHandler.Play(__instance.interactingAgent, "Success");
        //              __instance.RefrigeratorRun(__instance.interactingAgent);
        //              __instance.StopInteraction();
        //              return false;
        //          }
        //          else if (buttonText == "RefrigeratorRun" && !__instance.interactingAgent.interactionHelper.interactingFar)
        //          {
        //              BunnyHeader.ConsoleMessage.LogMessage(__instance.name + ": " + "Manual Run");

        //              gc.audioHandler.Play(__instance.interactingAgent, "Success");
        //              __instance.StartCoroutine(__instance.Operating(__instance.interactingAgent, __instance.interactingAgent.inventory.FindItem("Wrench"), 2f, true, "Tampering"));
        //              Refrigerator_IceDispense(__instance);
        //              return false;
        //              //__instance.RefrigeratorRun(__instance.interactingAgent);
        //              //__instance.StopInteraction();
        //              //TODO: I think 
        //          }
        //          else if (buttonText == "OpenContainer")
        //	{
        //              BunnyHeader.ConsoleMessage.LogMessage(__instance.name + ": " + "Show Chest");

        //              __instance.ShowChest();
        //              return false;
        //          }

        //          Refrigerator_Variables[__instance].wasHacked = false;
        //          __instance.StopInteraction(); // May need to remove this for second two options

        //          return false;
        //}
        //      public static void Refrigerator_UseWrenchToRun(Refrigerator __instance) // Non-Patch
        //{
        //          BunnyHeader.ConsoleMessage.LogMessage(__instance.name + ": " + MethodBase.GetCurrentMethod().Name);

        //          if (gc.serverPlayer)
        //          {
        //              __instance.MakeNonFunctional(__instance.interactingAgent);
        //              __instance.interactingAgent.inventory.SubtractFromItemCount(__instance.interactingAgent.inventory.FindItem("Wrench"), 30);
        //              __instance.interactingAgent.skillPoints.AddPoints("TamperGeneratorPoints");
        //              gc.playerAgent.SetCheckUseWithItemsAgain(__instance);
        //              return;
        //          }
        //          __instance.functional = false;
        //          gc.playerAgent.SetCheckUseWithItemsAgain(__instance);
        //          __instance.interactingAgent.objectMult.ObjectAction(__instance.objectNetID, "UseWrenchToRun");

        //          //TODO: See Item.FireworksFire if you want to turn this into randomly shooting Freeze Rays
        //      }
        //      public static Dictionary<Refrigerator, Refrigerator_Remora> Refrigerator_Variables = new Dictionary<Refrigerator, Refrigerator_Remora>();
        #endregion
        #region SatelliteDish
        public void SatelliteDish_00()
        {
            Postfix(typeof(SatelliteDish), "DetermineButtons", GetType(), "SatelliteDish_DetermineButtons", new Type[0] { });
        }
        public static void SatelliteDish_DetermineButtons(SatelliteDish __instance) // PostFix
        {
            if (__instance.buttons.Any())
                CorrectButtonCosts(__instance);
        }
        #endregion
        #region SecurityCam
        public void SecurityCam_00()
        {
            Prefix(typeof(SecurityCam), "MyUpdate", GetType(), "SecurityCam_MyUpdate", new Type[0] { });
            Postfix(typeof(SecurityCam), "SetVars", GetType(), "SecurityCam_SetVars", new Type[0] { });
        }
        public static bool SecurityCam_MyUpdate(ref IEnumerator __result, SecurityCam __instance, ref tk2dTileMap ___tilemapFloors2, ref bool ___agentsPreviouslyInView) // Prefix
        {
            BMLog("LoadLevel_FillFloors_Prefix");

            // Structure advised by Abbysssal for patch-replacing IEnumerators.
            if (GC.challenges.Contains(cChallenge.PoliceState))
            {
                __result = SecurityCam_MyUpdate_Replacement(__instance, ___tilemapFloors2, ___agentsPreviouslyInView);

                return false;
            }

            return true;
        }
        public static IEnumerator SecurityCam_MyUpdate_Replacement(SecurityCam __instance, tk2dTileMap ___tilemapFloors2, bool ___agentsPreviouslyInView) // Non-Patch
		{
            for (; ; )
            {
                if (__instance.functional && !__instance.destroyed && __instance.activeObject)
                {
                    __instance.agentsInView.Clear();

                    for (int i = 0; i < __instance.gc.activeBrainAgentList.Count; i++)
                    {
                        bool targetFound = false;
                        Agent agent = __instance.gc.activeBrainAgentList[i];

                        if (agent.brain.active && !agent.invisible && !agent.ghost && !agent.objectAgent && !agent.mechEmpty && !agent.dead && (agent.prisoner <= 0 || agent.ownerID != 0 || agent.isPlayer != 0) && !agent.underBox)
                        {
                            if (__instance.targets == "NonOwners")
                            {
                                if (((agent.ownerID != __instance.owner && agent.ownerID != 99) || (agent.startingChunk != __instance.startingChunk && (__instance.startingSector == 0 || agent.startingSector != __instance.startingSector))) ||
                                    agent.statusEffects.hasTrait(vTrait.Wanted))
                                    targetFound = true;
                            }
                            else if (__instance.targets == "Owners")
                            {
                                if (((agent.ownerID == __instance.owner || agent.ownerID == 99) && (agent.startingChunk == __instance.startingChunk || (__instance.startingSector != 0 && agent.startingSector == __instance.startingSector)))||
                                    agent.statusEffects.hasTrait(vTrait.Wanted))
                                    targetFound = true;
                            }
                            else if (__instance.targets == "Everyone")
                                targetFound = true;
                        }

                        if (targetFound && agent.curTileData.chunkID == __instance.startingChunk && agent.curTileData.floorMaterial != floorMaterialType.None)
                        {
                            float num = Vector2.Distance(agent.tr.position, __instance.tr.position);
                            
                            if (num > __instance.blindSpotDistance && num < 9f && __instance.HasLOSObjectNormal(agent, num) && agent.curOwnerTile == __instance.owner && !agent.statusEffects.hasTrait("InvisibleToCameras") && __instance.functional && !__instance.destroyed && !__instance.destroying)
                                __instance.agentsInView.Add(agent);
                        }
                    }

                    if (__instance.securityType == "Turret")
                    {
                        for (int j = 0; j < __instance.turrets.Count; j++)
                            if (__instance.agentsInView.Count > 0)
                            {
                                if (!__instance.turrets[j].camerasViewing.Contains(__instance.UID))
                                {
                                    __instance.turrets[j].camerasViewing.Add(__instance.UID);
                                    __instance.turrets[j].camerasViewingReal.Add(__instance);
                                }
                            }
                            else if (__instance.turrets[j].camerasViewing.Count > 0)
                            {
                                __instance.turrets[j].camerasViewing.Remove(__instance.UID);
                                __instance.turrets[j].camerasViewingReal.Remove(__instance);
                            }

                        if (__instance.agentsInView.Count > 0 && __instance.countdownToNoise <= 0f)
                            for (int k = 0; k < __instance.agentsInView.Count; k++)
                            {
                                __instance.ChangeDoorTags();

                                if (!__instance.gc.cinematic)
                                    __instance.gc.audioHandler.Play(__instance, "SecurityCamSpot");
                                
                                __instance.gc.spawnerMain.SpawnStateIndicator(__instance, "HighVolume");
                                __instance.gc.spawnerMain.SpawnNoise(__instance.agentsInView[k].tr.position, 4f, __instance, "Alarm", __instance.agentsInView[k]);
                                
                                if ((__instance.targets == "Everyone" || __instance.targets == "Owners") && (__instance.agentsInView[k].ownerID == __instance.owner || __instance.agentsInView[k].ownerID == 99) && __instance.agentsInView[k].startingChunk == __instance.startingChunk)
                                {
                                    __instance.agentsInView[k].relationships.SetRel(__instance.objectAgent, "Hateful");
                                    __instance.agentsInView[k].relationships.SetRelHate(__instance.objectAgent, 5);
                                }

                                __instance.countdownToNoise = 1f;
                            }
                        else if (__instance.agentsInView.Count == 0)
                            __instance.countdownToNoise = 0f;
                    }
                    else if (__instance.securityType == "Noise")
                    {
                        if (__instance.agentsInView.Count > 0 && __instance.countdownToNoise <= 0f)
                            for (int l = 0; l < __instance.agentsInView.Count; l++)
                            {
                                __instance.ChangeDoorTags();

                                if (!__instance.gc.cinematic)
                                    __instance.gc.audioHandler.Play(__instance, "SecurityCamSpot");
                                
                                __instance.gc.spawnerMain.SpawnStateIndicator(__instance, "HighVolume");
                                __instance.gc.spawnerMain.SpawnNoise(__instance.agentsInView[l].tr.position, 4f, __instance, "Alarm", __instance.agentsInView[l]);
                                
                                if ((__instance.targets == "Everyone" || __instance.targets == "Owners") && (__instance.agentsInView[l].ownerID == __instance.owner || __instance.agentsInView[l].ownerID == 99) && __instance.agentsInView[l].startingChunk == __instance.startingChunk)
                                {
                                    __instance.agentsInView[l].relationships.SetRel(__instance.objectAgent, "Hateful");
                                    __instance.agentsInView[l].relationships.SetRelHate(__instance.objectAgent, 5);
                                }

                                __instance.countdownToNoise = 1f;
                            }
                        else if (__instance.agentsInView.Count == 0)
                            __instance.countdownToNoise = 0f;
                    }

                    if (!___agentsPreviouslyInView && __instance.agentsInView.Count > 0)
                    {
                        ___agentsPreviouslyInView = true;
                        __instance.RemoveThenSpawnParticles("");
                    }
                    else if (___agentsPreviouslyInView && __instance.agentsInView.Count == 0)
                    {
                        ___agentsPreviouslyInView = false;
                        __instance.RemoveThenSpawnParticles("");
                    }
                }

                yield return new WaitForSeconds(0.1f);
            }

            yield break;
        }
        public static void SecurityCam_SetVars(SecurityCam __instance) // Postfix
		{
            __instance.shiftAmountS = -0.16f;
        }
        // If PoliceState, alert if any Guilty detected
        #endregion
        #region SlotMachine
        public void SlotMachine_00()
        {
            //BunnyHeader.MainInstance.PatchPrefix(typeof(SlotMachine), "DetermineButtons", GetType(), "SlotMachine_DetermineButtons", new Type[0] { });
            //BunnyHeader.MainInstance.PatchPrefix(typeof(SlotMachine), "Gamble", GetType(), "SlotMachine_Gamble", new Type[1] { typeof(int) });
            //BunnyHeader.MainInstance.PatchPrefix(typeof(SlotMachine), "PressedButton", GetType(), "SlotMachine_PressedButton", new Type[2] { typeof(string), typeof(int) });
        }
        public static bool SlotMachine_DetermineButtons(SlotMachine __instance) // Replacement
        {
            MethodInfo determineButtons_base = AccessTools.DeclaredMethod(typeof(ObjectReal), "DetermineButtons", new Type[0] { });
            determineButtons_base.GetMethodWithoutOverrides<Action>(__instance).Invoke();

            if (__instance.interactingAgent.interactionHelper.interactingFar)
            {
                if (__instance.advantage == 0)
                    __instance.buttons.Add("IncreaseSlotMachineOdds");

                if ((__instance.interactingAgent.oma.superSpecialAbility && __instance.interactingAgent.agentName == "Hacker") || __instance.interactingAgent.statusEffects.hasTrait("HacksBlowUpObjects"))
                    __instance.buttons.Add("HackExplode");
            }
            else
            {
                __instance.buttons.Add("Play1");
                __instance.buttonPrices.Add(1);

                __instance.buttons.Add("Play5");
                __instance.buttonPrices.Add(5);

                __instance.buttons.Add("Play20");
                __instance.buttonPrices.Add(20);

                __instance.buttons.Add("Play50");
                __instance.buttonPrices.Add(50);

                __instance.buttons.Add("Play100");
                __instance.buttonPrices.Add(100);
            }
            return false;
        }
        public static void SlotMachine_DropMoney(int amount, SlotMachine __instance) // Non-Patch
        {
            // Original
            __instance.interactingAgent.inventory.AddItem("Money", amount);
            __instance.objectInvDatabase.SubtractFromItemCount(__instance.objectInvDatabase.money, amount);
            //

            //InvItem money = new InvItem()
            //{

            //};

            //__instance.objectInvDatabase.DropItem(money, false, __instance.tr.position); // Need to slightly randomize this

        }
        public static bool SlotMachine_Gamble(int gambleAmt, SlotMachine __instance) // Replacement
        {
            if (!__instance.moneySuccess(gambleAmt))
            {
                __instance.StopInteraction();

                return false;
            }

            __instance.IncreaseNumPlays();
            __instance.objectInvDatabase.money.invItemCount += gambleAmt;

            int advantage = 45;
            advantage += __instance.advantage;

            advantage = __instance.interactingAgent.DetermineLuck(advantage, "SlotMachine", true);

            if (GC.percentChance(1))
            {
                SlotMachine_Jackpot(gambleAmt * 10, __instance);
            }
            else if (GC.percentChance(advantage - 1))
            {
                __instance.interactingAgent.inventory.AddItem("Money", gambleAmt * 2);
                __instance.objectInvDatabase.SubtractFromItemCount(__instance.objectInvDatabase.money, gambleAmt * 2);
                __instance.interactingAgent.SayDialogue("SlotMachineWon");
                __instance.PlayAnim("MachineOperate", __instance.interactingAgent);
                GC.audioHandler.Play(__instance, "Win");
            }
            else
            {
                __instance.interactingAgent.SayDialogue("SlotMachineLost");
                GC.audioHandler.Play(__instance, "Fail");
            }

            __instance.StopInteraction();

            return false;
        }
        public static async void SlotMachine_Jackpot(int payout, SlotMachine __instance) // Non-Patch
        {
            __instance.interactingAgent.SayDialogue("SlotMachineJackpot");

            for (int i = 10; i < payout; i += 10)
            {
                SlotMachine_DropMoney(payout / 10, __instance);

                __instance.PlayAnim("MachineOperate", __instance.interactingAgent);
                GC.audioHandler.Play(__instance, "Win");

                await Task.Delay(200);
            }
        }
        public static bool SlotMachine_PressedButton(string buttonText, int buttonPrice, SlotMachine __instance) // Replacement
        {
            MethodInfo pressedButton_base = AccessTools.DeclaredMethod(typeof(ObjectReal), "PressedButton", new Type[2] { typeof(string), typeof(int) });
            pressedButton_base.GetMethodWithoutOverrides<Action<string, int>>(__instance).Invoke(buttonText, buttonPrice);

            if (buttonText == "Play1")
                __instance.Gamble(1);
            else if (buttonText == "Play5")
                __instance.Gamble(5);
            else if (buttonText == "Play20")
                __instance.Gamble(20);
            else if (buttonText == "Play50")
                __instance.Gamble(50);
            else if (buttonText == "Play100")
                __instance.Gamble(100);
            else if (buttonText != "IncreaseSlotMachineOdds")
            {
                GC.audioHandler.Play(__instance.interactingAgent, "Success");
                __instance.IncreaseSlotMachineOdds(__instance.interactingAgent);
                __instance.StopInteraction();
            }
            else
                __instance.StopInteraction();

            return false;
        }
        #endregion
        #region Stove
        public void Stove_00()
        {
            Prefix(typeof(Stove), "DamagedObject", GetType(), "Stove_DamagedObject", new Type[2] { typeof(PlayfieldObject), typeof(float) });
            Prefix(typeof(Stove), "DestroyMe3", GetType(), "Stove_DestroyMe3", new Type[0] { });
            Postfix(typeof(Stove), "RevertAllVars", GetType(), "Stove_RevertAllVars", new Type[0] { });
            Postfix(typeof(Stove), "SetVars", GetType(), "Stove_SetVars", new Type[0] { });
        }
        public static IEnumerator Stove_AboutToExplode(Stove __instance) // Non-Patch
        {
            BMHeader.ConsoleMessage.LogMessage(__instance.name + ": Stove_AboutToExplode");

            __instance.interactable = false;

            __instance.PlayObjectSpriteEffect("FlashingRepeatedly");

            if (__instance.lastHitByAgent != null)
            {
                GC.spawnerMain.SpawnNoise(__instance.tr.position, 1f, null, null, __instance.lastHitByAgent);
                GC.OwnCheck(__instance.lastHitByAgent, __instance.go, "Normal", 0);
            }

            Vector3 particlePosition = new Vector3(__instance.tr.position.x, __instance.tr.position.y + 0.36f, __instance.tr.position.z);
            __instance.SpawnParticleEffect("Smoke", particlePosition);

            BMLog("Stove_AboutToExplode: lastHitByagent = " + __instance.lastHitByAgent.agentName);

            //__instance.PlayAnim("MachineGoingToExplode", __instance.lastHitByAgent); // 202103031538
            GC.audioHandler.Play(__instance, "GeneratorHiss");

            __instance.RemoveObjectAgent();
            __instance.cantMakeFollowersAttack = true;

            yield return new WaitForSeconds(3f);

            if (!__instance.destroying)
            {
                BMLog("Firebomb 1");
                GC.spawnerMain.SpawnExplosion(__instance.interactingAgent, __instance.curPosition, "FireBomb", false, -1, false, true);
                __instance.DestroyMe(Stove_Variables[__instance].savedDamagerObject);
            }

            yield break;
        }
        public static void Stove_AnimationSequence(Stove __instance) // Non-Patch // Deactivating, Attempt 202102261601 
        {
            // BunnyHeader.ConsoleMessage.LogMessage(__instance.name + ": " + MethodBase.GetCurrentMethod().Name); // Verbose

            Stove_Remora remora = Stove_Variables[__instance];

            if (!__instance.destroying && __instance.activeObject && !__instance.notInOriginalLocation && __instance.spawnedShadow && __instance.onCamera)
            {
                remora.animationCountdown -= Time.deltaTime;

                if (remora.animationCountdown <= 0f)
                {
                    if (remora.animationFrame == 0)
                    {
                        __instance.ChangeSpriteByID(remora.animateSpriteID2);
                        remora.animationFrame = 1;
                    }
                    else
                    {
                        __instance.ChangeSpriteByID(remora.animateSpriteID);
                        remora.animationFrame = 0;
                    }
                    remora.animationCountdown = 0.5f;
                }
            }
        }
        public static bool Stove_DamagedObject(PlayfieldObject damagerObject, float damageAmount, Stove __instance) // Replacement
        {
            BMHeader.ConsoleMessage.LogMessage(__instance.name + ": " + MethodBase.GetCurrentMethod().Name);

            MethodInfo damagedObject = AccessTools.DeclaredMethod(typeof(ObjectReal), "DamagedObject", new Type[2] { typeof(PlayfieldObject), typeof(float) });
            damagedObject.GetMethodWithoutOverrides<Action<PlayfieldObject, float>>(__instance).Invoke(damagerObject, damageAmount);

            if (damageAmount >= 15f && !__instance.startedFlashing)
            {
                BMHeader.ConsoleMessage.LogMessage("Stove_DamagedObject: Lemma 1");

                Stove_Variables[__instance].savedDamagerObject = damagerObject;
                __instance.StartCoroutine(Stove_AboutToExplode(__instance));
            }

            if (damageAmount >= __instance.damageThreshold)
            {
                BMHeader.ConsoleMessage.LogMessage("Stove_DamagedObject: Firebomb 2");

                Stove_Variables[__instance].savedDamagerObject = damagerObject;

                GC.spawnerMain.SpawnExplosion(__instance.interactingAgent, __instance.curPosition, "FireBomb", false, -1, false, true);
                __instance.DestroyMe(damagerObject);
            }

            return false;
        }
        public static bool Stove_DestroyMe3(Stove __instance) // Replacement
        {
            if (GC.serverPlayer || !__instance.objectBeingThrown)
            {
                GC.spawnerMain.SpawnFire(__instance.tossedBy, __instance.tr.position);
                GC.spawnerMain.SpawnExplosion(Stove_Variables[(Stove)__instance].savedDamagerObject, __instance.tr.position, "FireBomb", false, -1, false, __instance.FindMustSpawnExplosionOnClients(Stove_Variables[(Stove)__instance].savedDamagerObject));
            }
            return false;
        }
        public static void Stove_GrilledFud(Stove __instance) // Non-Patch 
        {
            BMLog("Grill GrilledFud 1");
            InvItem rawFud = __instance.interactingAgent.inventory.FindItem("Fud");
            int numCooked = rawFud.invItemCount;

            rawFud.invItemCount -= numCooked;

            if (rawFud.invItemCount <= 0)
                __instance.interactingAgent.inventory.DestroyItem(rawFud);

            InvItem hotFud = new InvItem();
            hotFud.invItemName = "HotFud";
            hotFud.SetupDetails(false);
            hotFud.invItemCount = numCooked;

            __instance.interactingAgent.inventory.AddItemOrDrop(hotFud);
            hotFud.ShowPickingUpText(__instance.interactingAgent);

            GC.spawnerMain.SpawnNoise(__instance.curPosition, 1f, null, null, __instance.lastHitByAgent);
            GC.audioHandler.Play(__instance, "Grill");
        }
        public static void Stove_RevertAllVars(Stove __instance) // Postfix
        {
            int logDepth = 1;

            BMHeader.ConsoleMessage.LogMessage("Stove_RevertAllVars " + logDepth++);

            Stove_Variables[__instance].mustSpawnExplosionOnClients = false;

            BMHeader.ConsoleMessage.LogMessage("Stove_RevertAllVars " + logDepth++);

            // Trying to deactivate this to determine if if will fix rotation.
            //Stove_Variables[__instance].animateSpriteID = 0;
            //Stove_Variables[__instance].animateSpriteID2 = 0;
            //__instance.GetComponent<Animator>().enabled = false; // 202103031538
            //

            BMHeader.ConsoleMessage.LogMessage("Stove_RevertAllVars " + logDepth++);

            Stove_Variables[__instance].savedDamagerObject = null;

            BMHeader.ConsoleMessage.LogMessage("Stove_RevertAllVars " + logDepth++);

            Stove_Variables[__instance].noOwnCheckCountdown = false;

            BMHeader.ConsoleMessage.LogMessage("Stove_RevertAllVars " + logDepth++);

            Stove_Variables[__instance].countdownCauser = null;

            BMHeader.ConsoleMessage.LogMessage("Stove_RevertAllVars " + logDepth++);

            __instance.objectSprite.transform.Find("RealSprite").transform.localPosition = Vector3.zero;
            __instance.objectSprite.transform.Find("RealSprite").transform.localScale = Vector3.one;

            BMHeader.ConsoleMessage.LogMessage("Stove_RevertAllVars " + logDepth++);

            __instance.CancelInvoke();
        }
        public static void Stove_SetVars(Stove __instance) // Postfix
        {
            __instance.canExplosiveStimulate = true;
            __instance.dontDestroyImmediateOnClient = true;
            __instance.hasUpdate = true;
            __instance.interactable = true;
        }
        public static void Stove_UseWrenchToDetonate(Stove __instance) // Non-Patch 
        {
            BMHeader.ConsoleMessage.LogMessage(__instance.name + ": " + MethodBase.GetCurrentMethod().Name);

            if (GC.serverPlayer)
            {
                __instance.MakeNonFunctional(__instance.interactingAgent);
                __instance.interactingAgent.inventory.SubtractFromItemCount(__instance.interactingAgent.inventory.FindItem("Wrench"), 30);
                __instance.interactingAgent.skillPoints.AddPoints("TamperGeneratorPoints");
                GC.playerAgent.SetCheckUseWithItemsAgain(__instance);
                return;
            }
            __instance.functional = false;
            GC.playerAgent.SetCheckUseWithItemsAgain(__instance);
            __instance.interactingAgent.objectMult.ObjectAction(__instance.objectNetID, "UseWrenchToDetonate");
        }
        public static Dictionary<Stove, Stove_Remora> Stove_Variables = new Dictionary<Stove, Stove_Remora>();
        #endregion
        #region TableBig
        public void TableBig_00()
        {
            Postfix(typeof(TableBig), "SetVars", GetType(), "TableBig_SetVars", new Type[0] { });

        }
        public static void TableBig_SetVars(TableBig __instance) // Postfix
        {
            if (BMTraits.IsPlayerTraitActive(cTrait.StealthBastardDeluxe))
                __instance.interactable = true;
            //__instance.lowInteractionPriority = true;
        }
        #endregion
        #region Television
        public void Television_00()
        {
            //BunnyHeader.MainInstance.PatchPostfix(typeof(Television), "SetVars", GetType(), "Television_SetVars");
        }
        public static void Television_SetVars(Television __instance) // Postfix
        {
        }
        #endregion
        #region Toilet
        public void Toilet_00()
        {
            Prefix(typeof(Toilet), "DetermineButtons", GetType(), "Toilet_DetermineButtons", new Type[0] { });
            Prefix(typeof(Toilet), "FlushYourself", GetType(), "Toilet_FlushYourself", new Type[0] { });
            Prefix(typeof(Toilet), "PressedButton", GetType(), "Toilet_PressedButton", new Type[2] { typeof(string), typeof(int) });
        }
        public static bool Toilet_DetermineButtons(Toilet __instance) // Prefix
		{
            if (GC.challenges.Contains(cChallenge.AnCapistan))
			{
                MethodInfo determineButtons_base = AccessTools.DeclaredMethod(typeof(ObjectReal), "DetermineButtons", new Type[0] { });
                determineButtons_base.GetMethodWithoutOverrides<Action>(__instance).Invoke();

                bool toiletWorking = false;

                if (GC.levelType != "HomeBase")
                {
                    if ((__instance.interactingAgent.statusEffects.hasTrait("Diminutive") || __instance.interactingAgent.statusEffects.hasStatusEffect("Shrunk")) && !__instance.interactingAgent.statusEffects.hasStatusEffect("Giant"))
                    {
                        __instance.buttons.Add("FlushYourself");
                        __instance.buttonPrices.Add(10);

                        toiletWorking = true;
                    }

                    if (__instance.hasPurgeStatusEffects())
                    {
                        __instance.buttons.Add("PurgeStatusEffects");
                        __instance.buttonPrices.Add(10);

                        toiletWorking = true;
                    }
                }
                
                if (!toiletWorking)
                    __instance.interactingAgent.SayDialogue("ToiletWontGo");

                return false;
			}

            return true;
		}
        public static bool Toilet_FlushYourself(Toilet __instance) // Replacement
        {
            if (__instance.interactingAgent.statusEffects.hasTrait(cTrait.UnderdarkCitizen))
			{
                if ((__instance.interactingAgent.statusEffects.hasTrait(vTrait.Diminutive) || __instance.interactingAgent.statusEffects.hasStatusEffect(vStatusEffect.Shrunk)) && !__instance.interactingAgent.statusEffects.hasStatusEffect(vStatusEffect.Giant))
                {
                    List<ObjectReal> exits = new List<ObjectReal>();

                    for (int i = 0; i < GC.objectRealList.Count; i++)
                    {
                        ObjectReal exitCandidate = GC.objectRealList[i];

                        if ((exitCandidate.objectName == vObject.Toilet || exitCandidate.objectName == vObject.Manhole) && 
                            exitCandidate != __instance && !exitCandidate.destroyed && exitCandidate.startingChunk != __instance.startingChunk)
                        {
                            if (exitCandidate.objectName == vObject.Manhole)
                            {
                                Manhole manhole = (Manhole)exitCandidate;

                                if (manhole.opened)
                                    exits.Add(exitCandidate);
                            }
							else
                                exits.Add(exitCandidate);
                        }
                    }

                    if (exits.Count == 0)
                        for (int j = 0; j < GC.objectRealList.Count; j++)
                        {
                            ObjectReal exitCandidate = GC.objectRealList[j];

                            if ((exitCandidate.objectName == vObject.Toilet || exitCandidate.objectName == vObject.Manhole) 
                                && exitCandidate != __instance && !exitCandidate.destroyed)
                            {
                                if (exitCandidate.objectName == vObject.Manhole)
                                {
                                    Manhole manhole = (Manhole)exitCandidate;

                                    if (manhole.opened)
                                        exits.Add(exitCandidate);
                                }
                                else
                                    exits.Add(exitCandidate);
                            }
                        }

                    ObjectReal exit = __instance;

                    if (exits.Count > 0)
                        exit = exits[Random.Range(0, exits.Count)];

                    Vector3 exitSpot = Vector3.zero;
                    string direction = exit.direction;

                    switch (direction)
                    {
                        case "E":
                            exitSpot = new Vector3(exit.tr.position.x + 0.32f, exit.tr.position.y, exit.tr.position.z);

                            break;

                        case "N":
                            exitSpot = new Vector3(exit.tr.position.x, exit.tr.position.y + 0.32f, exit.tr.position.z);

                            break;

                        case "S":
                            exitSpot = new Vector3(exit.tr.position.x, exit.tr.position.y - 0.32f, exit.tr.position.z);

                            break;

                        case "W":
                            exitSpot = new Vector3(exit.tr.position.x - 0.32f, exit.tr.position.y, exit.tr.position.z);

                            break;
                    }

                    GC.audioHandler.Play(__instance, "ToiletTeleportIn");
                    __instance.interactingAgent.toiletTeleporting = true;
                    __instance.interactingAgent.Teleport(exitSpot, false, true);
                    GC.spawnerMain.SpawnExplosion(__instance.interactingAgent, exit.tr.position, "Water", false, -1, false, __instance.FindMustSpawnExplosionOnClients(__instance.interactingAgent));
                }

                return false;
            }

            return true;
        }
        public static bool Toilet_PressedButton(string buttonText, int buttonPrice, Toilet __instance) // Prefix
		{
            if (GC.challenges.Contains(cChallenge.AnCapistan))
			{
                MethodInfo pressedButton_base = AccessTools.DeclaredMethod(typeof(ObjectReal), "PressedButton", new Type[2] { typeof(string), typeof(int) });
                pressedButton_base.GetMethodWithoutOverrides<Action<string, int>>(__instance).Invoke(buttonText, buttonPrice);

                if (buttonText == "FlushYourself" && __instance.moneySuccess(buttonPrice))
                    __instance.FlushYourself();
                else if (buttonText == "PurgeStatusEffects" && __instance.moneySuccess(buttonPrice))
                    __instance.PurgeStatusEffects();
                else
				{
                    __instance.interactingAgent.Say("I'm too poor (and therefore too lazy and unworthy) for this privilege!");
                    __instance.StopInteraction();
                }

                return false;
			}

            return true;
		}
		#endregion
		#region Trash Can
        public void TrashCan_00()
		{
            Prefix(typeof(TrashCan), "Interact", GetType(), "TrashCan_Interact", new Type[1] { typeof(Agent) });
		}
        public static void TrashCan_Hide(TrashCan __instance, Agent agent) // Non-Patch
		{
            agent.SetInvisible(false);
            agent.statusEffects.BecomeHidden(__instance);

            __instance.StopInteraction();
        }
        public static bool TrashCan_Interact (Agent agent, TrashCan __instance) // Prefix
		{
            if (agent.statusEffects.hasTrait(cTrait.StealthBastardDeluxe) && agent.statusEffects.hasTrait(vTrait.Diminutive))
			{
                MethodInfo interact_base = AccessTools.DeclaredMethod(typeof(ObjectReal), "Interact", new Type[1] { typeof(Agent) });
                interact_base.GetMethodWithoutOverrides<Action<Agent>>(__instance).Invoke(agent);

                __instance.ShowObjectButtons();
                
                return false;
			}

            return true;
		}
		#endregion
		#region VendorCart
        public void VendorCart_00()
		{
            Postfix(typeof(VendorCart), "SetVars", GetType(), "VendorCart_SetVars", new Type[0] { });
		}
        public static void VendorCart_SetVars(VendorCart __instance) // Postfix
		{
            __instance.interactable = true;
		}
        public static void VendorCart_Steal(VendorCart __instance) // Non-Patch
		{
            InvItem invItem = __instance.objectInvDatabase.InvItemList[0];

            invItem.ItemSetup(true);
            invItem.invItemCount = invItem.rewardCount;
            invItem.ShowPickingUpText(__instance.interactingAgent);
            __instance.interactingAgent.inventory.AddItem(invItem);
            __instance.objectInvDatabase.DestroyAllItems();
            __instance.StopInteraction();
		}
		#endregion
		#region Window
		public void Window_00()
        {
            Postfix(typeof(Window), "DetermineButtons", GetType(), "Window_DetermineButtons", new Type[0] { });
            Prefix(typeof(Window), "SlipThroughWindow", GetType(), "Window_SlipThroughWindow", new Type[1] { typeof(Agent) });
        }
        public static void Window_DetermineButtons(Window __instance) // Postfix
        {
            if (__instance.buttons.Any())
                CorrectButtonCosts(__instance);
        }
        public static bool Window_SlipThroughWindow(Agent myAgent, Window __instance) // Replacement
		{
            if (myAgent.statusEffects.hasTrait(vTrait.Bulky))
            {
                __instance.interactingAgent.SayDialogue("CantFit");
                GC.audioHandler.Play(myAgent, "CantDo");

                __instance.StopInteraction();

                return false;
            }

            if (!myAgent.statusEffects.hasTrait(cTrait.StealthBastardDeluxe))
                GC.audioHandler.Play(myAgent, "MeleeHitAgentCutSmallClients");

            __instance.StopInteraction();

            if (__instance.direction == "E" || __instance.direction == "W")
            {
                if (myAgent.tr.position.x < __instance.tr.position.x)
                    myAgent.tr.position = new Vector3(__instance.tr.position.x + 0.32f, __instance.tr.position.y, myAgent.tr.position.z);
                else
                    myAgent.tr.position = new Vector3(__instance.tr.position.x - 0.32f, __instance.tr.position.y, myAgent.tr.position.z);
            }
            else if (myAgent.tr.position.y < __instance.tr.position.y)
                myAgent.tr.position = new Vector3(__instance.tr.position.x, __instance.tr.position.y + 0.32f, myAgent.tr.position.z);
            else
                myAgent.tr.position = new Vector3(__instance.tr.position.x, __instance.tr.position.y - 0.32f, myAgent.tr.position.z);

            if (myAgent.statusEffects.hasTrait(cTrait.StealthBastardDeluxe))
                return false;

            float dmg = 15f;
            myAgent.deathMethod = "BrokenGlass";

            if (GC.challenges.Contains("LowHealth"))
                dmg = 7f;
            if (myAgent.health <= dmg)
                dmg = myAgent.health - 1f;

            myAgent.statusEffects.ChangeHealth(-dmg);
            GC.spawnerMain.SpawnNoise(myAgent.tr.position, 0.2f, null, null, myAgent);

            return false;
        }
		#endregion
		#endregion
	}
	#region Remorae
    public class Elevator_Remora
	{
        public static GameController GC => GameController.gameController;

        public Elevator elevatorHost; 

        public bool ticketPurchased = false;
    }
    public class Manhole_Remora
	{
        public static GameController GC => GameController.gameController;

        public Manhole manholeHost;

        public bool splashed = false;
    }
    public class Stove_Remora
    {
        public static GameController GC => GameController.gameController;

        public Stove stoveHost;

        public int animateSpriteID;
        public int animateSpriteID2;
        public float animationCountdown;
        public int animationFrame;
        public PlayfieldObject countdownCauser;
        public bool mustSpawnExplosionOnClients;
        public bool noOwnCheckCountdown;
        public PlayfieldObject savedDamagerObject;

        public void Countdown()
        {
            Stove stove = BMObjects.Stove_Variables.FirstOrDefault(x => x.Value == this).Key;

            string myText = string.Concat(stove.timeCountdownClock);

            if (stove.timeCountdownClock > 0 && !stove.destroyed && !stove.destroying)
                GC.spawnerMain.SpawnStatusText(stove, "Countdown", myText);

            stove.timeCountdownClock--;

            if (stove.timeCountdownClock == 0 || stove.timeCountdownClock == -1 || stove.destroyed)
                stove.CancelInvoke();
        }
    }
    public class Toilet_Remora
	{
        public static GameController GC => GameController.gameController;

        public Toilet toiletHost;

        public bool splashed = false;
    }
    //   public class Refrigerator_Remora
    //{
    //       public Refrigerator refrigeratorHost;

    //       public int animateSpriteID;
    //       public int animateSpriteID2;
    //       public float animationCountdown;
    //       public int animationFrame;
    //       public PlayfieldObject savedDamagerObject;
    //       public bool wasHacked = false;

    //       public void Countdown()
    //       {
    //           Refrigerator Refrigerator = BunnyObjects.Refrigerator_Variables.FirstOrDefault(x => x.Value == this).Key;

    //           string myText = string.Concat(Refrigerator.timeCountdownClock);

    //           if (Refrigerator.timeCountdownClock > 0 && !Refrigerator.destroyed && !Refrigerator.destroying)
    //               gc.spawnerMain.SpawnStatusText(Refrigerator, "Countdown", myText);

    //           Refrigerator.timeCountdownClock--;

    //           if (Refrigerator.timeCountdownClock == 0 || Refrigerator.timeCountdownClock == -1 || Refrigerator.destroyed)
    //               Refrigerator.CancelInvoke();
    //       }
    //   }
    #endregion
}
