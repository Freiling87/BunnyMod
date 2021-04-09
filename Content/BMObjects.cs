﻿using HarmonyLib;
using RogueLibsCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace BunnyMod.Content
{
	public class BMObjects
    {
        public static GameController GC => GameController.gameController;
        public static bool Prefix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) => BMHeader.MainInstance.PatchPrefix(type, methodName, patchType, patchMethodName, types);
        public static bool Postfix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) => BMHeader.MainInstance.PatchPostfix(type, methodName, patchType, patchMethodName, types);

        #region Generic
        public void Awake()
        {
            Initialize_Names();

            // ObjectReal
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

            // PlayfieldObject
            //this.PatchPrefix(typeof(PlayfieldObject), "FindDamage", GetType(), "PlayfieldObject_FindDamage", new Type[] { typeof(PlayfieldObject), typeof(bool), typeof(bool), typeof(bool) }); 
            Prefix(typeof(PlayfieldObject), "Operating", GetType(), "PlayfieldObject_Operating", new Type[5] { typeof(Agent), typeof(InvItem), typeof(float), typeof(bool), typeof(string) });
            Prefix(typeof(PlayfieldObject), "playerHasUsableItem", GetType(), "PlayfieldObject_PlayerHasUsableItem", new Type[1] { typeof(InvItem) });

            // StatusEffects
            Postfix(typeof(StatusEffects), "BecomeHidden", GetType(), "StatusEffects_BecomeHidden", new Type[1] { typeof(ObjectReal) });
            Postfix(typeof(StatusEffects), "BecomeNotHidden", GetType(), "StatusEffects_BecomeNotHidden", new Type[0]);



            // Bathtub
            Postfix(typeof(Bathtub), "SetVars", GetType(), "Bathtub_SetVars", new Type[0] { });
            
            // Crate
            Postfix(typeof(Crate), "DetermineButtons", GetType(), "Crate_DetermineButtons", new Type[0] { });

            // Door
            Postfix(typeof(Door), "DetermineButtons", GetType(), "Door_DetermineButtons", new Type[0] { });

            // Flaming Barrel
            Postfix(typeof(FlamingBarrel), "SetVars", GetType(), "FlamingBarrel_SetVars", new Type[0] { });

            // Generator
            Postfix(typeof(Generator), "DetermineButtons", GetType(), "Generator_DetermineButtons", new Type[0] { });

            // Generator (Overclocked)
            Postfix(typeof(Generator2), "DetermineButtons", GetType(), "Generator2_DetermineButtons", new Type[0] { });

            // Hole
            Prefix(typeof(Hole), "EnterRange", GetType(), "Hole_EnterRange", new Type[1] { typeof(GameObject) });

            // Laser Emitter
            Postfix(typeof(LaserEmitter), "DetermineButtons", GetType(), "LaserEmitter_DetermineButtons", new Type[0] { });

            // Manhole
            Postfix(typeof(Manhole), "SetVars", GetType(), "Manhole_SetVars", new Type[0] { });

            // Plant
            Postfix(typeof(Plant), "SetVars", GetType(), "Plant_SetVars", new Type[0] { });

            // Police Box
            Postfix(typeof(PoliceBox), "DetermineButtons", GetType(), "PoliceBox_DetermineButtons", new Type[0] { });

            // Pool Table
            Postfix(typeof(PoolTable), "SetVars", GetType(), "PoolTable_SetVars", new Type[0] { });

			//// Refrigerator
			//BunnyHeader.MainInstance.PatchPrefix(typeof(Refrigerator), "DetermineButtons", GetType(), "Refrigerator_DetermineButtons");
			//BunnyHeader.MainInstance.PatchPostfix(typeof(Refrigerator), "FinishedOperating", GetType(), "Refrigerator_FinishedOperating");
			//BunnyHeader.MainInstance.PatchPrefix(typeof(Refrigerator), "Interact", GetType(), "Refrigerator_Interact", new Type[1] { typeof(Agent) });
			//BunnyHeader.MainInstance.PatchPrefix(typeof(Refrigerator), "InteractFar", GetType(), "Refrigerator_InteractFar", new Type[1] { typeof(Agent) });
			//BunnyHeader.MainInstance.PatchPrefix(typeof(Refrigerator), "ObjectAction", GetType(), "Refrigerator_ObjectAction", new Type[5] { typeof(string), typeof(string), typeof(float), typeof(Agent), typeof(PlayfieldObject) });
			//BunnyHeader.MainInstance.PatchPrefix(typeof(Refrigerator), "PressedButton", GetType(), "Refrigerator_PressedButton", new Type[1] { typeof(string) });

			//// Slot Machine
			//BunnyHeader.MainInstance.PatchPrefix(typeof(SlotMachine), "DetermineButtons", GetType(), "SlotMachine_DetermineButtons", new Type[0] { });
			//BunnyHeader.MainInstance.PatchPrefix(typeof(SlotMachine), "Gamble", GetType(), "SlotMachine_Gamble", new Type[1] { typeof(int) });
			//BunnyHeader.MainInstance.PatchPrefix(typeof(SlotMachine), "PressedButton", GetType(), "SlotMachine_PressedButton", new Type[2] { typeof(string), typeof(int) });

            // Satellite Dish
			Postfix(typeof(SatelliteDish), "DetermineButtons", GetType(), "SatelliteDish_DetermineButtons", new Type[0] { });

            // Stove
            Prefix(typeof(Stove), "DamagedObject", GetType(), "Stove_DamagedObject", new Type[2] { typeof(PlayfieldObject), typeof(float) });
            Prefix(typeof(Stove), "DestroyMe3", GetType(), "Stove_DestroyMe3", new Type[0] { });
            Postfix(typeof(Stove), "RevertAllVars", GetType(), "Stove_RevertAllVars", new Type[0] { });
            Postfix(typeof(Stove), "SetVars", GetType(), "Stove_SetVars", new Type[0] { });

            // Table (Big)
            Postfix(typeof(TableBig), "SetVars", GetType(), "TableBig_SetVars", new Type[0] { });

            // Television
            //BunnyHeader.MainInstance.PatchPostfix(typeof(Television), "SetVars", GetType(), "Television_SetVars");

            // Window
            Postfix(typeof(Window), "DetermineButtons", GetType(), "Window_DetermineButtons", new Type[0] { });
            Prefix(typeof(Window), "SlipThroughWindow", GetType(), "Window_SlipThroughWindow", new Type[1] { typeof(Agent) });
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
                BMHeader.Log("Detected ButtonExtra: " + buttonLabel + " (Only non-blank when additional costs are applied to button)");

                if (buttonLabel.EndsWith("-30"))
                {
                    newLabel = buttonLabel.Replace("-30", "-" + BMTraits.ToolCost(objectReal.interactingAgent, 30));
                    flag = true;
                }
                else if (buttonLabel.EndsWith("-20"))
                {
                    newLabel = buttonLabel.Replace("-20", "-" + BMTraits.ToolCost(objectReal.interactingAgent, 20));
                    flag = true;
                }
                else if (buttonLabel == " - 7HP")
                {
                    BMHeader.Log("a");
                    newLabel = buttonLabel.Replace(" - 7HP", " - " + BMTraits.HealthCost(objectReal.interactingAgent, 7, DamageType.brokenWindow));
                    flag = true;
                }
                else if (buttonLabel == " - 15HP")
                {
                    BMHeader.Log("b");
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
                    BMHeader.Log("c");
                    objectReal.buttonsExtra[objectReal.buttonsExtra.FindIndex(ind => ind.Equals(buttonLabel))] = newLabel;
                    break;
                }
            }
        }
        #endregion

        #region ObjectReal
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
                    __instance.buttonsExtra.Add(" (" + agent.inventory.FindItem("Crowbar").invItemCount + ") -" + BMTraits.ToolCost(agent, 15));
                }

                if (manhole.opened && agent.statusEffects.hasTrait("UnderdarkCitizen"))
                    __instance.buttons.Add("FlushYourself");
            }
            else if (__instance is Stove)
            {
                if (!__instance.startedFlashing)
                {
                    if (agent.inventory.HasItem("Wrench"))
                    {
                        __instance.buttons.Add("UseWrenchToDetonate");
                        __instance.buttonsExtra.Add(" (" + agent.inventory.FindItem("Wrench").invItemCount + ") -" + BMTraits.ToolCost(agent, 30));
                    }

                    if (agent.inventory.HasItem("Fud"))
                        __instance.buttons.Add("GrillFud");
                }
            }
        }
        public static bool ObjectReal_FinishedOperating(ObjectReal __instance) // Replacement
        {
            BMHeader.ConsoleMessage.LogMessage(__instance.name + ": " + MethodBase.GetCurrentMethod().Name);

            MethodInfo finishedOperating_base = AccessTools.DeclaredMethod(typeof(PlayfieldObject), "FinishedOperating");
            finishedOperating_base.GetMethodWithoutOverrides<Action>(__instance).Invoke();

            if (__instance is FlamingBarrel)
            {
                FlamingBarrel_GrilledFud((FlamingBarrel)__instance);
                __instance.StopInteraction();
            }
            else if (__instance is Manhole)
			{
                if (__instance.operatingItem.invItemName == "Crowbar")
				{
                    Manhole_UseCrowbar((Manhole)__instance);
                    __instance.StopInteraction();
				}
			}
            else if (__instance is Stove)
            {
                if (__instance.operatingItem.invItemName == "Wrench")
                {
                    Stove_UseWrenchToDetonate((Stove)__instance);
                    __instance.StopInteraction();
                }

                if (__instance.operatingItem.invItemName == "Fud")
                {
                    Stove_GrilledFud((Stove)__instance);
                    __instance.StopInteraction();
                }
            }

            if (!__instance.interactingAgent.interactionHelper.interactingFar)
            {
                string operatingBarType = __instance.operatingBarType;

                if (operatingBarType == "Collecting")
                {
                    __instance.CollectPart();
                    __instance.StopInteraction();
                }
            }
            return false;
        }
        public static bool ObjectReal_Interact(Agent agent, ObjectReal __instance) // Replacement
        {
            BMHeader.ConsoleMessage.LogMessage("ObjectReal_Interact: " + __instance.name);

            MethodInfo interact_base = AccessTools.DeclaredMethod(typeof(PlayfieldObject), "Interact");
            interact_base.GetMethodWithoutOverrides<Action<Agent>>(__instance).Invoke(agent);

            if (__instance is Bathtub || __instance is Plant || __instance is PoolTable || __instance is TableBig)
            {
                if (agent.statusEffects.hasTrait("StealthBastardDeluxe"))
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

            if (buttonText == "CollectPart") // Vanilla
            {
                __instance.StartCoroutine(__instance.Operating(__instance.interactingAgent, null, 5f, true, "Collecting"));

                if (!__instance.interactingAgent.statusEffects.hasTrait("OperateSecretly") && __instance.functional)
                {
                    GC.spawnerMain.SpawnNoise(__instance.tr.position, 1f, __instance.interactingAgent, "Normal", __instance.interactingAgent);
                    GC.audioHandler.Play(__instance, "Hack");
                    __instance.SpawnParticleEffect("Hack", __instance.tr.position);
                    GC.spawnerMain.SpawnStateIndicator(__instance, "HighVolume");
                    GC.OwnCheck(__instance.interactingAgent, __instance.go, "Normal", 0);
                }
            }
            else if (buttonText == "DispenseIce")
            {
                __instance.StartCoroutine(__instance.Operating(__instance.interactingAgent, __instance.interactingAgent.inventory.FindItem("Wrench"), 2f, true, "Tampering"));

                if (!__instance.interactingAgent.statusEffects.hasTrait("OperateSecretly") && __instance.functional)
                {
                    GC.spawnerMain.SpawnNoise(__instance.tr.position, 1f, __instance.interactingAgent, "Normal", __instance.interactingAgent);
                    //gc.audioHandler.Play(__instance, "Hack");
                    __instance.SpawnParticleEffect("Hack", __instance.tr.position);
                    GC.spawnerMain.SpawnStateIndicator(__instance, "HighVolume");
                    GC.OwnCheck(__instance.interactingAgent, __instance.go, "Normal", 0);
                }
            }
            else if (buttonText == "FlushYourself" && __instance is Manhole)
                Manhole_FlushYourself((Manhole)__instance);
            else if (buttonText == "GrillFud")
                __instance.StartCoroutine(__instance.Operating(__instance.interactingAgent, __instance.interactingAgent.inventory.FindItem("Fud"), 2f, true, "Grilling"));
            else if (buttonText == "HackExplode") // Vanilla
                __instance.HackExplode(__instance.interactingAgent);
            else if (buttonText == "LightBarbecue")
            {
                __instance.StartFireInObject();
                __instance.StopInteraction();
            }
            else if (buttonText == "UseCrowbar")
                __instance.StartCoroutine(__instance.Operating(__instance.interactingAgent, __instance.interactingAgent.inventory.FindItem("Crowbar"), 2f, true, "Unlocking"));
            else if (buttonText == "UseWrenchToDetonate")
            {
                __instance.StartCoroutine(__instance.Operating(__instance.interactingAgent, __instance.interactingAgent.inventory.FindItem("Wrench"), 2f, true, "Tampering"));

                if (!__instance.interactingAgent.statusEffects.hasTrait("OperateSecretly") && __instance.functional)
                {
                    GC.spawnerMain.SpawnNoise(__instance.tr.position, 1f, __instance.interactingAgent, "Normal", __instance.interactingAgent);
                    __instance.SpawnParticleEffect("Hack", __instance.tr.position);
                    GC.spawnerMain.SpawnStateIndicator(__instance, "HighVolume");
                    GC.OwnCheck(__instance.interactingAgent, __instance.go, "Normal", 0);
                }
            }
            return false;
        }
        public static void ObjectReal_Start(ObjectReal __instance) // Postfix
        {
            //BunnyHeader.ConsoleMessage.LogMessage("ObjectReal_Start");

            if (__instance is Stove stove)
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
        public static bool PlayfieldObject_Operating(Agent myAgent, InvItem item, float timeToUnlock, bool makeNoise, string barType, PlayfieldObject __instance) // Prefix
        {
            try
            {
                BMHeader.Log("PlayfieldObject_Operating " + __instance.name + ": ");
                BMHeader.Log("    Agent = " + myAgent.name);
                BMHeader.Log("    item = " + item.invItemName);
                BMHeader.Log("    timeToUnlock = " + timeToUnlock);
                BMHeader.Log("    makeNoise = " + makeNoise);
                BMHeader.Log("    barType = " + barType);
            }
            catch
            {
                BMHeader.Log("Logging error");
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
        #region StatusEffects
        public static void StatusEffects_BecomeHidden(ObjectReal hiddenInObject, StatusEffects __instance)
        {
            if (hiddenInObject is Bathtub || hiddenInObject is Plant || hiddenInObject is PoolTable || hiddenInObject is TableBig)
                __instance.agent.agentCollider.enabled = false;
            return;
        }
        public static void StatusEffects_BecomeNotHidden(StatusEffects __instance)
        {
            __instance.agent.agentCollider.enabled = true;
        }
        #endregion

        #region Bathtub
        public static void Bathtub_SetVars(Bathtub __instance)
        {
            if (BMTraits.IsTraitActive("StealthBastardDeluxe"))
                __instance.interactable = true;

            //TODO: Closed Bath Curtain sprite?
            // See Generator.Start() for how to set animation sprites. Maybe just toggle sprite when used/unused.
        }
        #endregion
        #region Crate
        public static void Crate_DetermineButtons(Crate __instance) // Postfix
        {
            if (__instance.buttons.Any())
                CorrectButtonCosts(__instance);
        }
        #endregion
        #region Door
        public static void Door_DetermineButtons(Door __instance) // Postfix
        {
            if (__instance.buttons.Any())
                CorrectButtonCosts(__instance);
        }
        #endregion
        #region FlamingBarrel
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

            if (agent.statusEffects.hasTrait("ResistFire") || agent.statusEffects.hasTrait("FireproofSkin") || agent.statusEffects.hasTrait("FireproofSkin2"))
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
        public static void Generator_DetermineButtons(Generator __instance) // Postfix
        {
            if (__instance.buttons.Any())
                CorrectButtonCosts(__instance);
        }
        #endregion
        #region Generator2
        public static void Generator2_DetermineButtons(Generator2 __instance) // Postfix
        {
            if (__instance.buttons.Any())
                CorrectButtonCosts(__instance);
        }
		#endregion
		#region Hole
		public static bool Hole_EnterRange(GameObject myObject, Hole __instance) // Prefix
		{
            if (GC.loadComplete)
                if (myObject.CompareTag("Agent"))
                {
                    Agent agent = myObject.GetComponent<Agent>();

                    if (__instance.GetComponent<ObjectMultHole>().objectHoleType == "Manhole" && agent.statusEffects.hasTrait("UnderdarkCitizen"))
                    {
                        Manhole_FlushYourself(agent);

                        return false;
                    }
                }

            return true;
		}
		#endregion
		#region LaserEmitter
		public static void LaserEmitter_DetermineButtons(LaserEmitter __instance) // Postfix
        {
            if (__instance.buttons.Any())
                CorrectButtonCosts(__instance);
        }
        #endregion
        #region Manhole
        public static void Manhole_FlushYourself(Agent agent) // Non-Patch
        {
            List<ObjectReal> exits = new List<ObjectReal>();

            for (int i = 0; i < GC.objectRealList.Count; i++)
            {
                ObjectReal objectReal = GC.objectRealList[i];

                if (objectReal.objectName == "Manhole")
                {
                    Manhole manhole = (Manhole)objectReal;

                    if (manhole.opened)
                        exits.Add(objectReal);
                }
            }

            Vector2 exit = exits[UnityEngine.Random.Range(0, exits.Count)].curPosition;
            Vector2 offset = UnityEngine.Random.insideUnitCircle.normalized;

            GC.audioHandler.Play(agent, "ToiletTeleportIn");
            agent.toiletTeleporting = true;
            agent.Teleport(exit + offset, true, false);
        }
		public static void Manhole_FlushYourself(Manhole __instance) // Non-Patch
		{
			Agent agent = __instance.interactingAgent;

			if ((agent.statusEffects.hasTrait("UnderdarkCitizen")) && !agent.statusEffects.hasStatusEffect("Giant"))
			{
				List<ObjectReal> list = new List<ObjectReal>();
				float furthestManholeDistance = 0f;

				for (int i = 0; i < GC.objectRealList.Count; i++)
				{
					ObjectReal objectReal = GC.objectRealList[i];

					if (objectReal.objectName == "Manhole" && objectReal != __instance)
					{
						Manhole manhole = (Manhole)objectReal;

						if (manhole.opened)
						{
							list.Add(objectReal);
							float distance = Vector2.Distance(__instance.tr.position, objectReal.tr.position);

							if (distance > furthestManholeDistance)
								furthestManholeDistance = distance;
						}
					}
				}

				ObjectReal destinationManhole = __instance;

				if (list.Count > 0)
					destinationManhole = list[UnityEngine.Random.Range(0, list.Count)];

				Vector2 outHole = destinationManhole.tr.position;
                Vector2 offset = UnityEngine.Random.insideUnitCircle.normalized;

                GC.audioHandler.Play(__instance, "ToiletTeleportIn");
				agent.toiletTeleporting = true;
				agent.Teleport(outHole + offset, true, false);
			}
		}
		public static void Manhole_SetVars(Manhole __instance) // Postfix
		{
            __instance.interactable = true;
		}
        public static void Manhole_UseCrowbar(Manhole __instance) // Non-Patch
        {
            BMHeader.Log("Manhole_UseCrowbar");

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
        public static void Plant_SetVars(Plant __instance) // Postfix
        {
            if (BMTraits.IsTraitActive("StealthBastardDeluxe"))
                __instance.interactable = true;
            //__instance.lowInteractionPriority = true;
        }
        #endregion
        #region PoliceBox
        public static void PoliceBox_DetermineButtons(PoliceBox __instance)
        {
            if (__instance.buttons.Any())
                CorrectButtonCosts(__instance);
        }
        #endregion
        #region PoolTable
        public static void PoolTable_SetVars(PoolTable __instance) // Postfix
        {
            if (BMTraits.IsTraitActive("StealthBastardDeluxe"))
                __instance.interactable = true;
            //__instance.lowInteractionPriority = true;
        }
        #endregion
        #region Refrigerator
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

        //          MethodInfo DetermineButtons_Base = AccessTools.DeclaredMethod(typeof(ObjectReal), "DetermineButtons");
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
        public static void SatelliteDish_DetermineButtons(SatelliteDish __instance) // PostFix
        {
            if (__instance.buttons.Any())
                CorrectButtonCosts(__instance);
        }
        #endregion
        #region SlotMachine
        public static bool SlotMachine_DetermineButtons(SlotMachine __instance) // Replacement
        {
            MethodInfo determineButtons_base = AccessTools.DeclaredMethod(typeof(ObjectReal), "DetermineButtons");
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
            MethodInfo pressedButton_base = AccessTools.DeclaredMethod(typeof(ObjectReal), "PressedButton");
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

            BMHeader.Log("Stove_AboutToExplode: lastHitByagent = " + __instance.lastHitByAgent.agentName);

            //__instance.PlayAnim("MachineGoingToExplode", __instance.lastHitByAgent); // 202103031538
            GC.audioHandler.Play(__instance, "GeneratorHiss");

            __instance.RemoveObjectAgent();
            __instance.cantMakeFollowersAttack = true;

            yield return new WaitForSeconds(3f);

            if (!__instance.destroying)
            {
                BMHeader.Log("Firebomb 1");
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

            MethodInfo damagedObject = AccessTools.DeclaredMethod(typeof(ObjectReal), "DamagedObject");
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
            BMHeader.Log("Grill GrilledFud 1");
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
		public static void TableBig_SetVars(TableBig __instance) // Postfix
        {
            if (BMTraits.IsTraitActive("StealthBastardDeluxe"))
                __instance.interactable = true;
            //__instance.lowInteractionPriority = true;
        }
        #endregion
        #region Television
        public static void Television_SetVars(Television __instance) // Postfix
        {
        }
        #endregion
        #region Window
        public static void Window_DetermineButtons(Window __instance) // Postfix
        {
            if (__instance.buttons.Any())
                CorrectButtonCosts(__instance);
        }
        public static bool Window_SlipThroughWindow(Agent myAgent, Window __instance) // Replacement
		{
            if (myAgent.statusEffects.hasTrait("BigCollider"))
            {
                __instance.interactingAgent.SayDialogue("CantFit");
                GC.audioHandler.Play(myAgent, "CantDo");

                __instance.StopInteraction();

                return false;
            }

            if (!myAgent.statusEffects.hasTrait("StealthBastardDeluxe"))
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

            if (myAgent.statusEffects.hasTrait("StealthBastardDeluxe"))
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
	}
	#region Remorae
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