using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using BepInEx;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;
using UnityEngine.Networking;
using System.Threading.Tasks;

namespace BunnyMod
{
    public class BunnyObjects
    {
        #region Generic
        public void Awake()
        {
            Initialize_Names();

            #region Patches - Object Base
            BunnyHeader.MainInstance.PatchPrefix(typeof(ObjectReal), "DestroyMe", GetType(), "ObjectReal_DestroyMe", new Type[1] { typeof(PlayfieldObject) });
            BunnyHeader.MainInstance.PatchPrefix(typeof(ObjectReal), "DestroyMe3", GetType(), "ObjectReal_DestroyMe3", new Type[0] { });
            BunnyHeader.MainInstance.PatchPostfix(typeof(ObjectReal), "DetermineButtons", GetType(), "ObjectReal_DetermineButtons");
            BunnyHeader.MainInstance.PatchPrefix(typeof(ObjectReal), "FinishedOperating", GetType(), "ObjectReal_FinishedOperating");
            BunnyHeader.MainInstance.PatchPrefix(typeof(ObjectReal), "Interact", GetType(), "ObjectReal_Interact", new Type[1] { typeof(Agent) });
            BunnyHeader.MainInstance.PatchPrefix(typeof(ObjectReal), "MakeNonFunctional", GetType(), "ObjectReal_MakeNonFunctional", new Type[1] { typeof(PlayfieldObject) });
            BunnyHeader.MainInstance.PatchPostfix(typeof(ObjectReal), "ObjectAction", GetType(), "ObjectReal_ObjectAction", new Type[5] { typeof(string), typeof(string), typeof(float), typeof(Agent), typeof(PlayfieldObject) });
            BunnyHeader.MainInstance.PatchPrefix(typeof(ObjectReal), "ObjectUpdate", GetType(), "ObjectReal_ObjectUpdate");
            BunnyHeader.MainInstance.PatchPrefix(typeof(ObjectReal), "PressedButton", GetType(), "ObjectReal_PressedButton", new Type[2] { typeof(string), typeof(int) });
            BunnyHeader.MainInstance.PatchPostfix(typeof(ObjectReal), "Start", GetType(), "ObjectReal_Start");

            //this.PatchPrefix(typeof(PlayfieldObject), "FindDamage", GetType(), "PlayfieldObject_FindDamage", new Type[] { typeof(PlayfieldObject), typeof(bool), typeof(bool), typeof(bool) }); 
            BunnyHeader.MainInstance.PatchPrefix(typeof(PlayfieldObject), "playerHasUsableItem", GetType(), "PlayfieldObject_PlayerHasUsableItem", new Type[1] { typeof(InvItem) });

            BunnyHeader.MainInstance.PatchPostfix(typeof(StatusEffects), "BecomeHidden", GetType(), "StatusEffects_BecomeHidden", new Type[1] { typeof(ObjectReal) });
            BunnyHeader.MainInstance.PatchPostfix(typeof(StatusEffects), "BecomeNotHidden", GetType(), "StatusEffects_BecomeNotHidden");
			#endregion
			#region Patches - Objects
			BunnyHeader.MainInstance.PatchPostfix(typeof(Bathtub), "SetVars", GetType(), "Bathtub_SetVars", new Type[0] { });

            BunnyHeader.MainInstance.PatchPostfix(typeof(Crate), "DetermineButtons", GetType(), "Crate_DetermineButtons", new Type[0] { });

            BunnyHeader.MainInstance.PatchPostfix(typeof(Door), "DetermineButtons", GetType(), "Door_DetermineButtons", new Type[0] { });

            BunnyHeader.MainInstance.PatchPostfix(typeof(FlamingBarrel), "SetVars", GetType(), "FlamingBarrel_SetVars", new Type[0] { });

            BunnyHeader.MainInstance.PatchPostfix(typeof(Generator), "DetermineButtons", GetType(), "Generator_DetermineButtons", new Type[0] { });

            BunnyHeader.MainInstance.PatchPostfix(typeof(Generator2), "DetermineButtons", GetType(), "Generator2_DetermineButtons", new Type[0] { });

            BunnyHeader.MainInstance.PatchPostfix(typeof(LaserEmitter), "DetermineButtons", GetType(), "LaserEmitter_DetermineButtons", new Type[0] { });

            BunnyHeader.MainInstance.PatchPostfix(typeof(Plant), "SetVars", GetType(), "Plant_SetVars", new Type[0] { });

            BunnyHeader.MainInstance.PatchPostfix(typeof(PoliceBox), "DetermineButtons", GetType(), "PoliceBox_DetermineButtons", new Type[0] { });

            BunnyHeader.MainInstance.PatchPostfix(typeof(PoolTable), "SetVars", GetType(), "PoolTable_SetVars", new Type[0] { });

            //BunnyHeader.MainInstance.PatchPrefix(typeof(Refrigerator), "DetermineButtons", GetType(), "Refrigerator_DetermineButtons");
            //BunnyHeader.MainInstance.PatchPostfix(typeof(Refrigerator), "FinishedOperating", GetType(), "Refrigerator_FinishedOperating");
            //BunnyHeader.MainInstance.PatchPrefix(typeof(Refrigerator), "Interact", GetType(), "Refrigerator_Interact", new Type[1] { typeof(Agent) });
            //BunnyHeader.MainInstance.PatchPrefix(typeof(Refrigerator), "InteractFar", GetType(), "Refrigerator_InteractFar", new Type[1] { typeof(Agent) });
            //BunnyHeader.MainInstance.PatchPrefix(typeof(Refrigerator), "ObjectAction", GetType(), "Refrigerator_ObjectAction", new Type[5] { typeof(string), typeof(string), typeof(float), typeof(Agent), typeof(PlayfieldObject) });
            //BunnyHeader.MainInstance.PatchPrefix(typeof(Refrigerator), "PressedButton", GetType(), "Refrigerator_PressedButton", new Type[1] { typeof(string) });

            //BunnyHeader.MainInstance.PatchPrefix(typeof(SlotMachine), "DetermineButtons", GetType(), "SlotMachine_DetermineButtons", new Type[0] { });
            //BunnyHeader.MainInstance.PatchPrefix(typeof(SlotMachine), "Gamble", GetType(), "SlotMachine_Gamble", new Type[1] { typeof(int) });
            //BunnyHeader.MainInstance.PatchPrefix(typeof(SlotMachine), "PressedButton", GetType(), "SlotMachine_PressedButton", new Type[2] { typeof(string), typeof(int) });

            BunnyHeader.MainInstance.PatchPostfix(typeof(SatelliteDish), "DetermineButtons", GetType(), "SatelliteDish_DetermineButtons", new Type[0] { });

            BunnyHeader.MainInstance.PatchPrefix(typeof(Stove), "DamagedObject", GetType(), "Stove_DamagedObject", new Type[2] { typeof(PlayfieldObject), typeof(float) });
            BunnyHeader.MainInstance.PatchPrefix(typeof(Stove), "DestroyMe3", GetType(), "Stove_DestroyMe3", new Type[0] { });
            BunnyHeader.MainInstance.PatchPostfix(typeof(Stove), "RevertAllVars", GetType(), "Stove_RevertAllVars", new Type[0] { });
            BunnyHeader.MainInstance.PatchPostfix(typeof(Stove), "SetVars", GetType(), "Stove_SetVars", new Type[0] { });

            BunnyHeader.MainInstance.PatchPostfix(typeof(TableBig), "SetVars", GetType(), "TableBig_SetVars", new Type[0] { });

            //BunnyHeader.MainInstance.PatchPostfix(typeof(Television), "SetVars", GetType(), "Television_SetVars");
			#endregion
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
                BunnyHeader.ConsoleMessage.LogInfo("Removed pair.Key from VariablesStove");
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
            CustomName dispenseIce = RogueLibs.CreateCustomName("DispenseIce", "ButtonText", new CustomNameInfo("Dispense Ice"));

            CustomName openContainer = RogueLibs.CreateCustomName("OpenContainer", "ButtonText", new CustomNameInfo("Open container"));

            CustomName slotMachine_Play1 = RogueLibs.CreateCustomName("Play1", "ButtonText", new CustomNameInfo("Bet $1"));
            CustomName slotMachine_Play100 = RogueLibs.CreateCustomName("Play100", "ButtonText", new CustomNameInfo("Bet $100"));

            CustomName stove_DontTouchAngry = RogueLibs.CreateCustomName("Stove_DontTouchAngry", "Dialogue", new CustomNameInfo("Yeah, just make yourself at home, asshole!"));
            CustomName stove_NotThrilled = RogueLibs.CreateCustomName("Stove_NotThrilled", "Dialogue", new CustomNameInfo("Oh, you're uh... using my stove."));
        }
        #endregion

        #region Custom
        public static void CorrectButtonCosts(ObjectReal objectReal)
		{
            // TODO: This will only catch one tamper operation per object
            // Next you'd want to make a dictionary of buttonsExtra indexes and new versions of the labels 
            // You can't do it within the loop because it will break execution

            bool flag = false;
            string newLabel = "";

            foreach (string buttonLabel in objectReal.buttonsExtra)
			{
                BunnyHeader.Log("Detected ButtonExtra: " + buttonLabel);

                if (buttonLabel.EndsWith("-30"))
				{
                    newLabel = buttonLabel.Replace("-30", "-" + BunnyTraits.ToolCost(objectReal.interactingAgent, 30));
                    flag = true;
                }
                else if (buttonLabel.EndsWith("-20"))
				{
                    newLabel = buttonLabel.Replace("-20", "-" + BunnyTraits.ToolCost(objectReal.interactingAgent, 20));
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

		#region ObjectReal
		public static bool ObjectReal_DestroyMe(PlayfieldObject damagerObject, ObjectReal __instance) // Prefix
        {
            BunnyHeader.ConsoleMessage.LogMessage(__instance.name + ": " + MethodBase.GetCurrentMethod().Name);

            if (__instance is Stove)
                Stove_Variables[(Stove)__instance].savedDamagerObject = damagerObject;
            
            return true;
        }
        public static bool ObjectReal_DestroyMe3(ObjectReal __instance) // Prefix
		{
            if (__instance is Stove)
			{
                if (__instance.gc.serverPlayer && !__instance.spawnedExplosion)
                {
                    Debug.Log("Spawn Stove Explosion");

                    __instance.spawnedExplosion = true;
                    Explosion explosion = __instance.gc.spawnerMain.SpawnExplosion(Stove_Variables[(Stove)__instance].savedDamagerObject, __instance.tr.position, "FireBomb", false, -1, false, __instance.FindMustSpawnExplosionOnClients(Stove_Variables[(Stove)__instance].savedDamagerObject));

                    if (Stove_Variables[(Stove)__instance].noOwnCheckCountdown)
                        explosion.noOwnCheck = true;
                }
                __instance.gc.audioHandler.Stop(__instance, "GeneratorHiss");
            }

            return true;
		}
        public static void ObjectReal_DetermineButtons(ObjectReal __instance) // Postfix
        {
            //ConsoleMessage.LogMessage("ObjectReal_DetermineButtons");

            if (__instance is FlamingBarrel)
            {
                if (__instance.ora.hasParticleEffect)
                {
                    if (__instance.interactingAgent.inventory.HasItem("Fud"))
                    {
                        __instance.buttons.Add("GrillFud");
                        __instance.buttonsExtra.Add(" (Burn hands for 10 damage)");
                    }
                    else
                        __instance.interactingAgent.SayDialogue("CantGrillFud");

                    return;
                }
                else
                {
                    if (__instance.interactingAgent.inventory.HasItem("CigaretteLighter"))
                        __instance.buttons.Add("LightBarbecue");
                    else
                        __instance.interactingAgent.SayDialogue("CantOperateBarbecue");

                    return;
                }
            }
            if (__instance is Stove)
            {
                if (!__instance.startedFlashing)
                {
                    if (__instance.interactingAgent.inventory.HasItem("Wrench"))
                    {
                        __instance.buttons.Add("UseWrenchToDetonate");
                        __instance.buttonsExtra.Add(" (" + __instance.interactingAgent.inventory.FindItem("Wrench").invItemCount + ") -" + BunnyTraits.ToolCost(__instance.interactingAgent, 30));
                    }

                    if (__instance.interactingAgent.inventory.HasItem("Fud"))
                        __instance.buttons.Add("GrillFud");
                }
                return;
            }
        }
        public static bool ObjectReal_FinishedOperating(ObjectReal __instance) // Replacement
        {
            BunnyHeader.ConsoleMessage.LogMessage(__instance.name + ": " + MethodBase.GetCurrentMethod().Name);

            MethodInfo finishedOperating_base = AccessTools.DeclaredMethod(typeof(PlayfieldObject), "FinishedOperating");
            finishedOperating_base.GetMethodWithoutOverrides<Action>(__instance).Invoke();

			if (__instance is FlamingBarrel)
			{
                FlamingBarrel_GrilledFud((FlamingBarrel)__instance);
                __instance.StopInteraction(); // Attempt 202102261630 
            }
            else  if (__instance is Stove)
            {
                if (__instance.operatingItem.invItemName == "Wrench")
				{
                    Stove_UseWrenchToDetonate((Stove)__instance);
                    __instance.StopInteraction(); // Attempt 202102261630
                }

                if (__instance.operatingItem.invItemName == "Fud")
                {
                    Stove_GrilledFud((Stove)__instance);
                    __instance.StopInteraction(); // Attempt 202102261630
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
            BunnyHeader.ConsoleMessage.LogMessage("ObjectReal_Interact: " + __instance.name);

            MethodInfo interact_base = AccessTools.DeclaredMethod(typeof(PlayfieldObject), "Interact");
            interact_base.GetMethodWithoutOverrides<Action<Agent>>(__instance).Invoke(agent);

            if (__instance is Bathtub || __instance is Plant || __instance is PoolTable || __instance is TableBig)
            {
                if (agent.statusEffects.hasTrait("StealthBastardDeluxe"))
				{
                    agent.SetInvisible(false); // Attempt to fix camo bug
                    agent.statusEffects.BecomeHidden(__instance);
                }
                    
                __instance.StopInteraction();
            }
            else if (__instance is FlamingBarrel)
                __instance.ShowObjectButtons();
            else if (__instance is Stove)
            {
                BunnyHeader.Log("Stove 1");

                __instance.gc.spawnerMain.SpawnNoise(__instance.tr.position, 1f, __instance, null, __instance.interactingAgent);

                Agent noticingOwner = Stove_OwnerWatching(agent, (Stove)__instance);
                Agent interactingAgent = __instance.interactingAgent;
                string relationship = agent.relationships.GetRel(interactingAgent);

                if (__instance.timer > 0f || __instance.startedFlashing)
                    __instance.StopInteraction();

                BunnyHeader.Log("Stove 6");

                if (!(noticingOwner is null))
				{
                    BunnyHeader.Log("ObjectReal_Interact: noticingOwner " + noticingOwner.agentName + "; RelStatus: \"" + relationship +"\"");

                    BunnyHeader.Log("Stove 7a");

                    if (relationship == "Annoyed" || relationship == "Neutral" || relationship == "Hostile")
					{
                        BunnyHeader.Log("Stove 8a");

                        noticingOwner.SayDialogue("Stove_DontTouchAngry");
                        noticingOwner.relationships.AddStrikes(agent, 1);

                        //possibly replace with gc.owncheck

                        __instance.StopInteraction();

                        return false;
                    }
                    else if (relationship == "Friendly" || relationship == "Aligned" || relationship == "Loyal" || relationship == "Submissive")
					{
                        BunnyHeader.Log("Stove 8b");

                        noticingOwner.SayDialogue("Stove_NotThrilled");
					}
                    else
                        BunnyHeader.Log("Stove 8c: Relationship null or not matched.");
				}
				else
                    BunnyHeader.Log("Stove 7b: Owner null");

                __instance.ShowObjectButtons();
            }

            __instance.playerInvDatabase = agent.GetComponent<InvDatabase>();

            return false;
        }
        public static bool ObjectReal_MakeNonFunctional(PlayfieldObject damagerObject, ObjectReal __instance) // Prefix
        {
            BunnyHeader.ConsoleMessage.LogMessage(__instance.name + ": " + MethodBase.GetCurrentMethod().Name);

            if (__instance is Stove)
            {
                if (damagerObject != null && __instance.interactable)
                {
                    __instance.gc.playerAgent.SetCheckUseWithItemsAgain(__instance);

                    if (!__instance.gc.serverPlayer)
                        __instance.gc.playerAgent.objectMult.ObjectAction(__instance.objectNetID, "MakeNonFunctional");

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
            BunnyHeader.ConsoleMessage.LogMessage(__instance.name + ": " + MethodBase.GetCurrentMethod().Name);

            if (__instance is Stove)
            {
                if (!___noMoreObjectActions && myAction == "UseWrenchToDetonate")
                    Stove_UseWrenchToDetonate((Stove)__instance);
                
                ___noMoreObjectActions = false;
            }
        }
        public static bool ObjectReal_ObjectUpdate(ObjectReal __instance) // Prefix
        {
            //ConsoleMessage.LogMessage("ObjectReal_ObjectUpdate"); // Verbose when enabled

            if (__instance is Stove)
            {
                Stove_Remora remora = Stove_Variables[(Stove)__instance];

                //Stove_AnimationSequence((Stove)__instance); // Attempt 202102261601

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
            BunnyHeader.ConsoleMessage.LogMessage(__instance.name + ": " + MethodBase.GetCurrentMethod().Name);

            MethodInfo pressedButton_Base = AccessTools.DeclaredMethod(typeof(PlayfieldObject), "PressedButton", new Type[2] { typeof(string), typeof(int) });
            pressedButton_Base.GetMethodWithoutOverrides<Action<string, int>>(__instance).Invoke(buttonText, buttonPrice);

            if (buttonText == "HackExplode")
            {
                __instance.HackExplode(__instance.interactingAgent);
                return false;
            }
            #region Patch
            if (buttonText == "LightBarbecue")
            {
                __instance.StartFireInObject();
                __instance.StopInteraction();

                return false;
            }
            if (buttonText == "GrillFud")
            {
                __instance.StartCoroutine(__instance.Operating(__instance.interactingAgent, __instance.interactingAgent.inventory.FindItem("Fud"), 1f, true, "Grilling"));

                if (!__instance.interactingAgent.statusEffects.hasTrait("OperateSecretly") && __instance.functional)
                {
                    __instance.gc.spawnerMain.SpawnNoise(__instance.tr.position, 1f, __instance.interactingAgent, "Normal", __instance.interactingAgent);
                    __instance.gc.audioHandler.Play(__instance, "GrillOperate");
                    //__instance.SpawnParticleEffect("Hack", __instance.tr.position);
                    __instance.gc.spawnerMain.SpawnStateIndicator(__instance, "HighVolume");
                    __instance.gc.OwnCheck(__instance.interactingAgent, __instance.go, "Normal", 0);
                }

                return false;
            }
            if (buttonText == "UseWrenchToDetonate")
            {
                __instance.StartCoroutine(__instance.Operating(__instance.interactingAgent, __instance.interactingAgent.inventory.FindItem("Wrench"), 2f, true, "Tampering"));

                if (!__instance.interactingAgent.statusEffects.hasTrait("OperateSecretly") && __instance.functional)
                {
                    __instance.gc.spawnerMain.SpawnNoise(__instance.tr.position, 1f, __instance.interactingAgent, "Normal", __instance.interactingAgent);
                    //__instance.gc.audioHandler.Play(__instance, "Hack");
                    __instance.SpawnParticleEffect("Hack", __instance.tr.position);
                    __instance.gc.spawnerMain.SpawnStateIndicator(__instance, "HighVolume");
                    __instance.gc.OwnCheck(__instance.interactingAgent, __instance.go, "Normal", 0);
                }
            }
            if (buttonText == "DispenseIce")
			{
                __instance.StartCoroutine(__instance.Operating(__instance.interactingAgent, __instance.interactingAgent.inventory.FindItem("Wrench"), 2f, true, "Tampering"));

                if (!__instance.interactingAgent.statusEffects.hasTrait("OperateSecretly") && __instance.functional)
                {
                    __instance.gc.spawnerMain.SpawnNoise(__instance.tr.position, 1f, __instance.interactingAgent, "Normal", __instance.interactingAgent);
                    //__instance.gc.audioHandler.Play(__instance, "Hack");
                    __instance.SpawnParticleEffect("Hack", __instance.tr.position);
                    __instance.gc.spawnerMain.SpawnStateIndicator(__instance, "HighVolume");
                    __instance.gc.OwnCheck(__instance.interactingAgent, __instance.go, "Normal", 0);
                }
            }
            #endregion Patch
            if (buttonText == "CollectPart")
			{
                __instance.StartCoroutine(__instance.Operating(__instance.interactingAgent, null, 5f, true, "Collecting"));

                if (!__instance.interactingAgent.statusEffects.hasTrait("OperateSecretly") && __instance.functional)
                {
                    __instance.gc.spawnerMain.SpawnNoise(__instance.tr.position, 1f, __instance.interactingAgent, "Normal", __instance.interactingAgent);
                    __instance.gc.audioHandler.Play(__instance, "Hack");
                    __instance.SpawnParticleEffect("Hack", __instance.tr.position);
                    __instance.gc.spawnerMain.SpawnStateIndicator(__instance, "HighVolume");
                    __instance.gc.OwnCheck(__instance.interactingAgent, __instance.go, "Normal", 0);
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
            int num = rawFud.invItemCount;

            //rawFud.invItemCount -= num;

            //if (rawFud.invItemCount <= 0)
            //{
            //    __instance.interactingAgent.inventory.DestroyItem(rawFud);
            //}

            __instance.interactingAgent.inventory.SubtractFromItemCount(rawFud, num); // Hopefully addresses negaFud bug

            InvItem cookedFud = new InvItem();
            cookedFud.invItemName = "HotFud";
            cookedFud.SetupDetails(false);
            cookedFud.invItemCount = num;
            __instance.interactingAgent.inventory.AddItemOrDrop(cookedFud);
            cookedFud.ShowPickingUpText(__instance.interactingAgent);

            __instance.gc.audioHandler.Play(__instance, "Grill");
            FlamingBarrel_GrilledFudAfter(num, __instance);
        }
        public static void FlamingBarrel_GrilledFudAfter(int myCount, FlamingBarrel __instance) // Non-patch
		{
            __instance.gc.audioHandler.Play(__instance, "FireHit");
            __instance.interactingAgent.statusEffects.ChangeHealth(-10f, __instance);
            __instance.interactingAgent.Say("God fucking damn it, I always fucking burn my fucking hands!");
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
		#region LaserEmitter
		public static void LaserEmitter_DetermineButtons(LaserEmitter __instance) // Postfix
		{
            if (__instance.buttons.Any())
                CorrectButtonCosts(__instance);
		}
		#endregion
		#region Plant
		public static void Plant_SetVars(Plant __instance) // Postfix
        {
            __instance.interactable = true;
            __instance.lowInteractionPriority = true;
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
            __instance.interactable = true;
            __instance.lowInteractionPriority = true;
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
		//          __instance.PlayAnim("MachineGoingToExplode", __instance.gc.playerAgent);
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

		//	Explosion explosion = __instance.gc.spawnerMain.SpawnExplosion(__instance, __instance.tr.position, "Normal", false, -1, false, true);
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
		//          Bullet bullet = __instance.gc.spawnerMain.SpawnBullet(__instance.tr.position, bulletStatus.FreezeRay, __instance);
		//	float angle;
		//	if (__instance.gc.serverPlayer)
		//	{
		//		angle = (float)UnityEngine.Random.Range(0, 359);
		//	}
		//	else
		//	{
		//		angle = (float)bulletRotation;
		//	}
		//	bullet.movement.RotateToAngleTransform(angle);
		//	bullet.movement.MoveForwardTransform(0.48f);
		//	__instance.gc.audioHandler.Play(__instance, "FireworksFire");
		//	if (__instance.gc.multiplayerMode && __instance.gc.serverPlayer)
		//	{
		//		bullet.bulletNetID = UnityEngine.Random.Range(0, 10000);
		//		NetworkInstanceId extraObjectID = NetworkInstanceId.Invalid;
		//		if (fireworksOwner != null) // Always true
		//		{
		//			extraObjectID = __instance.interactingAgent.objectNetID;
		//		}
		//		__instance.gc.playerAgent.objectMult.ObjectAction(__instance.objectNetID, "FireworksFire", angle.ToString(), (float)bulletNetID, extraObjectID);
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

		//              __instance.gc.audioHandler.Play(__instance.interactingAgent, "Success");
		//              __instance.RefrigeratorRun(__instance.interactingAgent);
		//              __instance.StopInteraction();
		//              return false;
		//          }
		//          else if (buttonText == "RefrigeratorRun" && !__instance.interactingAgent.interactionHelper.interactingFar)
		//          {
		//              BunnyHeader.ConsoleMessage.LogMessage(__instance.name + ": " + "Manual Run");

		//              __instance.gc.audioHandler.Play(__instance.interactingAgent, "Success");
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

		//          if (__instance.gc.serverPlayer)
		//          {
		//              __instance.MakeNonFunctional(__instance.interactingAgent);
		//              __instance.interactingAgent.inventory.SubtractFromItemCount(__instance.interactingAgent.inventory.FindItem("Wrench"), 30);
		//              __instance.interactingAgent.skillPoints.AddPoints("TamperGeneratorPoints");
		//              __instance.gc.playerAgent.SetCheckUseWithItemsAgain(__instance);
		//              return;
		//          }
		//          __instance.functional = false;
		//          __instance.gc.playerAgent.SetCheckUseWithItemsAgain(__instance);
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

            if (__instance.gc.percentChance(1))
			{
                SlotMachine_Jackpot(gambleAmt * 10, __instance);
            }
            else if (__instance.gc.percentChance(advantage - 1 ))
            {
                __instance.interactingAgent.inventory.AddItem("Money", gambleAmt * 2);
                __instance.objectInvDatabase.SubtractFromItemCount(__instance.objectInvDatabase.money, gambleAmt * 2);
                __instance.interactingAgent.SayDialogue("SlotMachineWon");
                __instance.PlayAnim("MachineOperate", __instance.interactingAgent);
                __instance.gc.audioHandler.Play(__instance, "Win");
            }
            else
			{
                __instance.interactingAgent.SayDialogue("SlotMachineLost");
                __instance.gc.audioHandler.Play(__instance, "Fail");
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
                __instance.gc.audioHandler.Play(__instance, "Win");

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
                __instance.gc.audioHandler.Play(__instance.interactingAgent, "Success");
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
            // TODO: This one is why it's blinking but not breaking.

            BunnyHeader.ConsoleMessage.LogMessage(__instance.name + ": Stove_AboutToExplode");

            __instance.interactable = false;

            __instance.PlayObjectSpriteEffect("FlashingRepeatedly");
            
            if (__instance.lastHitByAgent != null)
            {
                __instance.gc.spawnerMain.SpawnNoise(__instance.tr.position, 1f, null, null, __instance.lastHitByAgent);
                __instance.gc.OwnCheck(__instance.lastHitByAgent, __instance.go, "Normal", 0);
            }

            Vector3 particlePosition = new Vector3(__instance.tr.position.x, __instance.tr.position.y + 0.36f, __instance.tr.position.z);
            __instance.SpawnParticleEffect("Smoke", particlePosition);

            BunnyHeader.Log("Stove_AboutToExplode: lastHitByagent = " + __instance.lastHitByAgent.agentName);

            __instance.PlayAnim("MachineGoingToExplode", __instance.gc.playerAgent);
            __instance.gc.audioHandler.Play(__instance, "GeneratorHiss");

            __instance.RemoveObjectAgent();
            __instance.cantMakeFollowersAttack = true;

			yield return new WaitForSeconds(3f);

            if (!__instance.destroying)
			{
                BunnyHeader.Log("Firebomb 1");
                __instance.gc.spawnerMain.SpawnExplosion(__instance.interactingAgent, __instance.curPosition, "FireBomb", false, -1, false, true);
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
            BunnyHeader.ConsoleMessage.LogMessage(__instance.name + ": " + MethodBase.GetCurrentMethod().Name);

            MethodInfo damagedObject = AccessTools.DeclaredMethod(typeof(ObjectReal), "DamagedObject");
            damagedObject.GetMethodWithoutOverrides<Action<PlayfieldObject, float>>(__instance).Invoke(damagerObject, damageAmount);

            if (damageAmount >= 15f && !__instance.startedFlashing)
            {
                BunnyHeader.ConsoleMessage.LogMessage("Stove_DamagedObject: Lemma 1");

                Stove_Variables[__instance].savedDamagerObject = damagerObject;
                __instance.StartCoroutine(Stove_AboutToExplode(__instance));
            }

            if (damageAmount >= __instance.damageThreshold)
            {
                BunnyHeader.ConsoleMessage.LogMessage("Stove_DamagedObject: Firebomb 2");

                Stove_Variables[__instance].savedDamagerObject = damagerObject;

                __instance.gc.spawnerMain.SpawnExplosion(__instance.interactingAgent, __instance.curPosition, "FireBomb", false, -1, false, true);
                __instance.DestroyMe(damagerObject);
            }

            return false;
        }
        public static bool Stove_DestroyMe3(Stove __instance) // Replacement
		{
            if (__instance.gc.serverPlayer || !__instance.objectBeingThrown)
            {
                __instance.gc.spawnerMain.SpawnFire(__instance.tossedBy, __instance.tr.position);
                __instance.gc.spawnerMain.SpawnExplosion(Stove_Variables[(Stove)__instance].savedDamagerObject, __instance.tr.position, "FireBomb", false, -1, false, __instance.FindMustSpawnExplosionOnClients(Stove_Variables[(Stove)__instance].savedDamagerObject));
            }
            return false;
        }
        public static Agent Stove_FindOwner(Stove stove) // Non-Patch
		{
            BunnyHeader.Log("Stove_FindOwner: startingChunk = " + stove.startingChunk+ "; OwnerID = " + stove.owner);

            // relationships.owncheck

            for (int i = 0; i < stove.gc.agentList.Count; i++)
            {
                Agent agent = stove.gc.agentList[i];

                BunnyHeader.Log("Stove_FindOwner: Checking Agent " + i + "; OwnerID = " + agent.ownerID);

                if (agent.startingChunk == stove.startingChunk && agent.ownerID == stove.owner)
				{
                    BunnyHeader.Log("Stove_FindOwner: Found Stove Owner: " + agent.agentName);

                    return agent;
                }
                    
            }

            return null;
        }
        public static void Stove_GrilledFud(Stove __instance) // Non-Patch 
        {
            BunnyHeader.ConsoleMessage.LogMessage(__instance.name + ": " + MethodBase.GetCurrentMethod().Name);

            InvItem rawFud = __instance.interactingAgent.inventory.FindItem("Fud");
            
            int numCooked = rawFud.invItemCount;
            //rawFud.invItemCount -= numCooked;
            __instance.interactingAgent.inventory.ChangeItemCount(rawFud, -numCooked);

            InvItem cookedFud = new InvItem()
            {
                invItemName = "HotFud",
                invItemCount = numCooked,
            };
            cookedFud.SetupDetails(false);

            __instance.interactingAgent.inventory.AddItemOrDrop(cookedFud);
            cookedFud.ShowPickingUpText(__instance.interactingAgent);

            __instance.gc.audioHandler.Play(__instance, "Grill");
            __instance.gc.spawnerMain.SpawnNoise(__instance.curPosition, 1f, null, null, __instance.lastHitByAgent);
        }
        public static void Stove_RevertAllVars(Stove __instance) // Postfix
        {
            BunnyHeader.ConsoleMessage.LogMessage(__instance.name + ": " + MethodBase.GetCurrentMethod().Name);

            Stove_Variables[__instance].mustSpawnExplosionOnClients = false;

            // Trying to deactivate this to determine if if will fix rotation.
            //Stove_Variables[__instance].animateSpriteID = 0;
            //Stove_Variables[__instance].animateSpriteID2 = 0;
            //__instance.GetComponent<Animator>().enabled = false;
            //

            Stove_Variables[__instance].savedDamagerObject = null;
            Stove_Variables[__instance].noOwnCheckCountdown = false;
            Stove_Variables[__instance].countdownCauser = null;
            // Untested:
            __instance.objectSprite.transform.Find("RealSprite").transform.localPosition = Vector3.zero;
            __instance.objectSprite.transform.Find("RealSprite").transform.localScale = Vector3.one;
            //
            __instance.CancelInvoke();
        }
        public static void Stove_SetVars(Stove __instance) // Postfix
        {
            // __instance.animates = true; // Attempt 202102261601
            __instance.canExplosiveStimulate = true;
            __instance.dontDestroyImmediateOnClient = true;
            __instance.hasUpdate = true;
            __instance.interactable = true;
        }
        public static Agent Stove_OwnerWatching(Agent interactingAgent, Stove stove) // Non-Patch
		{
            BunnyHeader.Log("Stove_OwnerWatching: " + stove.name + ": Stove_UnfriendlyOwnerWatching");

            Agent agent = Stove_FindOwner(stove);

            if (agent != null && stove.interactingAgent != null)
            {
                relStatus relTypeCode = agent.relationships.RelList2[interactingAgent.agentID].relTypeCode;

                BunnyHeader.Log("Stove_OwnerWatching: " + stove.name + ": Owner " + relTypeCode);

                if (agent.movement.HasLOSAgent360(stove.interactingAgent) && !agent.dead && !agent.zombified)
                    return agent;
            }

            return null;
        }
        public static void Stove_UseWrenchToDetonate(Stove __instance) // Non-Patch 
        {
            BunnyHeader.ConsoleMessage.LogMessage(__instance.name + ": " + MethodBase.GetCurrentMethod().Name);

            if (__instance.gc.serverPlayer)
            {
                __instance.MakeNonFunctional(__instance.interactingAgent);
                __instance.interactingAgent.inventory.SubtractFromItemCount(__instance.interactingAgent.inventory.FindItem("Wrench"), 30); //TODO
                __instance.interactingAgent.skillPoints.AddPoints("TamperGeneratorPoints");
                __instance.gc.playerAgent.SetCheckUseWithItemsAgain(__instance);
                return;
            }
            __instance.functional = false;
            __instance.gc.playerAgent.SetCheckUseWithItemsAgain(__instance);
            __instance.interactingAgent.objectMult.ObjectAction(__instance.objectNetID, "UseWrenchToDetonate");
        }
        public static Dictionary<Stove, Stove_Remora> Stove_Variables = new Dictionary<Stove, Stove_Remora>();
		#endregion
		#region TableBig
		public static void TableBig_SetVars(TableBig __instance) // Postfix
        {
            __instance.interactable = true;
            __instance.lowInteractionPriority = true;
        }
        #endregion
        #region Television
        public static void Television_SetVars(Television __instance) // Postfix
        {
        }
        #endregion
    }
	#region Remorae
	public class Stove_Remora
    {
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
            Stove stove = BunnyObjects.Stove_Variables.FirstOrDefault(x => x.Value == this).Key;

            string myText = string.Concat(stove.timeCountdownClock);

            if (stove.timeCountdownClock > 0 && !stove.destroyed && !stove.destroying)
                stove.gc.spawnerMain.SpawnStatusText(stove, "Countdown", myText);

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
 //               Refrigerator.gc.spawnerMain.SpawnStatusText(Refrigerator, "Countdown", myText);

 //           Refrigerator.timeCountdownClock--;

 //           if (Refrigerator.timeCountdownClock == 0 || Refrigerator.timeCountdownClock == -1 || Refrigerator.destroyed)
 //               Refrigerator.CancelInvoke();
 //       }
 //   }
	#endregion
}
