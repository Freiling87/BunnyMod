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
			BunnyHeader.MainInstance.PatchPostfix(typeof(Bathtub), "SetVars", GetType(), "Bathtub_SetVars");

            BunnyHeader.MainInstance.PatchPostfix(typeof(FlamingBarrel), "SetVars", GetType(), "FlamingBarrel_SetVars");

            BunnyHeader.MainInstance.PatchPostfix(typeof(Plant), "SetVars", GetType(), "Plant_SetVars");

            BunnyHeader.MainInstance.PatchPostfix(typeof(PoolTable), "SetVars", GetType(), "PoolTable_SetVars");

            BunnyHeader.MainInstance.PatchPrefix(typeof(Refrigerator), "DetermineButtons", GetType(), "Refrigerator_DetermineButtons");
            BunnyHeader.MainInstance.PatchPostfix(typeof(Refrigerator), "FinishedOperating", GetType(), "Refrigerator_FinishedOperating");
            BunnyHeader.MainInstance.PatchPrefix(typeof(Refrigerator), "Interact", GetType(), "Refrigerator_Interact", new Type[1] { typeof(Agent) });
            BunnyHeader.MainInstance.PatchPrefix(typeof(Refrigerator), "InteractFar", GetType(), "Refrigerator_InteractFar", new Type[1] { typeof(Agent) });
            BunnyHeader.MainInstance.PatchPrefix(typeof(Refrigerator), "ObjectAction", GetType(), "Refrigerator_ObjectAction", new Type[5] { typeof(string), typeof(string), typeof(float), typeof(Agent), typeof(PlayfieldObject) });
            BunnyHeader.MainInstance.PatchPrefix(typeof(Refrigerator), "PressedButton", GetType(), "Refrigerator_PressedButton", new Type[1] { typeof(string) });
            BunnyHeader.MainInstance.PatchPostfix(typeof(Refrigerator), "Start", GetType(), "Refrigerator_Start");

            BunnyHeader.MainInstance.PatchPrefix(typeof(Stove), "DamagedObject", GetType(), "Stove_DamagedObject", new Type[2] { typeof(PlayfieldObject), typeof(float) });
            BunnyHeader.MainInstance.PatchPostfix(typeof(Stove), "RevertAllVars", GetType(), "Stove_RevertAllVars");
            BunnyHeader.MainInstance.PatchPostfix(typeof(Stove), "SetVars", GetType(), "Stove_SetVars");

            BunnyHeader.MainInstance.PatchPostfix(typeof(TableBig), "SetVars", GetType(), "TableBig_SetVars");

            //BunnyHeader.MainInstance.PatchPostfix(typeof(Television), "SetVars", GetType(), "Television_SetVars");
			#endregion
        }
        public void FixedUpdate()
        {
            List<Stove> stoveList = new List<Stove>();

            foreach (KeyValuePair<Stove, Stove_Remora> pair in Stove_Variables)
                if (pair.Key.isBroken())
                    stoveList.Add(pair.Key);

            foreach (Stove stove in stoveList)
            {
                Stove_Variables.Remove(stove);
                BunnyHeader.ConsoleMessage.LogInfo("Removed pair.Key from VariablesStove");
            }

            List<Refrigerator> refrigeratorList = new List<Refrigerator>();

            foreach (KeyValuePair<Refrigerator, Refrigerator_Remora> pair in Refrigerator_Variables)
                if (pair.Key.isBroken())
                    refrigeratorList.Add(pair.Key);

            foreach (Refrigerator Refrigerator in refrigeratorList)
            {
                Refrigerator_Variables.Remove(Refrigerator);
                BunnyHeader.ConsoleMessage.LogInfo("Removed pair.Key from VariablesRefrigerator");
            }
        }
        public static void Initialize_Names()
		{
		}
        #endregion
        #region ObjectReal
        public static bool ObjectReal_DestroyMe(PlayfieldObject damagerObject, ObjectReal __instance) // Prefix
        {
            BunnyHeader.ConsoleMessage.LogMessage("ObjectReal_DestroyMe");

            if (__instance is Stove)
                Stove_Variables[(Stove)__instance].savedDamagerObject = damagerObject;
            
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
                        __instance.buttonsExtra.Add(" (" + __instance.interactingAgent.inventory.FindItem("Wrench").invItemCount + ") -" + BunnyTraits.ToolCost(__instance.interactingAgent));
                    }
                    if (__instance.interactingAgent.inventory.HasItem("Fud"))
					{
                        __instance.buttons.Add("GrillFud");
                    }
                        
                }
                return;
            }
        }
        public static bool ObjectReal_FinishedOperating(ObjectReal __instance) // Replacement
        {
            BunnyHeader.ConsoleMessage.LogMessage("ObjectReal_FinishedOperating");

            MethodInfo finishedOperating_base = AccessTools.DeclaredMethod(typeof(PlayfieldObject), "FinishedOperating");
            finishedOperating_base.GetMethodWithoutOverrides<Action>(__instance).Invoke();

			#region Patch
			if (__instance is FlamingBarrel)
            {
                FlamingBarrel_GrilledFud((FlamingBarrel)__instance);
            }
            if (__instance is Stove)
            {
                if (__instance.operatingItem.invItemName == "Wrench")
                    Stove_UseWrenchToDetonate((Stove)__instance);

                if (__instance.operatingItem.invItemName == "Fud")
                    Stove_GrilledFud((Stove)__instance);
            }
            #endregion

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
            BunnyHeader.ConsoleMessage.LogMessage("ObjectReal_Interact");
            //TODO: Try StopInteraction() as a default, with other options as return.

            MethodInfo interact_base = AccessTools.DeclaredMethod(typeof(PlayfieldObject), "Interact");
            interact_base.GetMethodWithoutOverrides<Action<Agent>>(__instance).Invoke(agent);

            if (__instance is Bathtub || __instance is Plant || __instance is PoolTable || __instance is TableBig)
            {
                if (agent.statusEffects.hasTrait("StealthBastardDeluxe"))
                    agent.statusEffects.BecomeHidden(__instance);
                __instance.StopInteraction();
            }
            else if (__instance is FlamingBarrel)
            {
                __instance.ShowObjectButtons();
            }
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
            BunnyHeader.ConsoleMessage.LogMessage("ObjectReal_MakeNonFunctional");

            if (__instance is Stove)
            {
                if (damagerObject != null && __instance.interactable)
                {
                    __instance.gc.playerAgent.SetCheckUseWithItemsAgain(__instance);

                    if (!__instance.gc.serverPlayer)
                        __instance.gc.playerAgent.objectMult.ObjectAction(__instance.objectNetID, "MakeNonFunctional");

                    __instance.timer = 10f;
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
            BunnyHeader.ConsoleMessage.LogMessage("ObjectReal_ObjectAction");

            if (__instance is Stove)
            {
                if (!___noMoreObjectActions && myAction == "UseWrenchToDetonate")
                    Stove_UseWrenchToDetonate((Stove)__instance);
                
                ___noMoreObjectActions = false;
            }
        }
        public static bool ObjectReal_ObjectUpdate(ObjectReal __instance) // Prefix
        {
            //ConsoleMessage.LogMessage("ObjectReal_ObjectUpdate"); //Verbose

            if (__instance is Stove)
            {
                Stove_Remora remora = Stove_Variables[(Stove)__instance];

                Stove_AnimationSequence((Stove)__instance); //Untested

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
            BunnyHeader.ConsoleMessage.LogMessage("ObjectReal_PressedButton");

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
                __instance.StartCoroutine(__instance.Operating(__instance.interactingAgent, __instance.interactingAgent.inventory.FindItem("Fud"), 2f, true, "Grilling"));
                return false;
            }
            if (buttonText == "UseWrenchToDetonate")
            {
                __instance.StartCoroutine(__instance.Operating(__instance.interactingAgent, __instance.interactingAgent.inventory.FindItem("Wrench"), 2f, true, "Tampering"));
                return false;
            }
            #endregion Patch
            if (!(buttonText == "CollectPart"))
            {
                return false;
            }

            __instance.StartCoroutine(__instance.Operating(__instance.interactingAgent, null, 5f, true, "Collecting"));

            if (!__instance.interactingAgent.statusEffects.hasTrait("OperateSecretly") && __instance.functional)
            {
                __instance.gc.spawnerMain.SpawnNoise(__instance.tr.position, 1f, __instance.interactingAgent, "Normal", __instance.interactingAgent);
                __instance.gc.audioHandler.Play(__instance, "Hack");
                __instance.SpawnParticleEffect("Hack", __instance.tr.position);
                __instance.gc.spawnerMain.SpawnStateIndicator(__instance, "HighVolume");
                __instance.gc.OwnCheck(__instance.interactingAgent, __instance.go, "Normal", 0);
            }

            return false;
        }
        public static void ObjectReal_Start(ObjectReal __instance) // Postfix
        {
            BunnyHeader.ConsoleMessage.LogMessage("ObjectReal_Start");

            if (__instance is Stove stove)
			{
                Stove_Remora remora = new Stove_Remora();
                Stove_Variables[stove] = remora;
                remora.stoveHost = stove;
                // Untested:
                Stove_Variables[(Stove)__instance].animateSpriteID = __instance.spr.GetSpriteIdByName("Stove");
                Stove_Variables[(Stove)__instance].animateSpriteID2 = __instance.spr.GetSpriteIdByName("Stove");
            }
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
        #region FlamingBarrel
        public static void FlamingBarrel_GrilledFud(FlamingBarrel __instance) // Non-patch
		{
            InvItem rawFud = __instance.interactingAgent.inventory.FindItem("Fud");
            int num = rawFud.invItemCount;
            rawFud.invItemCount -= num;

            if (rawFud.invItemCount <= 0)
            {
                __instance.interactingAgent.inventory.DestroyItem(rawFud);
            }

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
        #region Plant
        public static void Plant_SetVars(Plant __instance) // Postfix
        {
            __instance.interactable = true;
            __instance.lowInteractionPriority = true;
        }
        #endregion
        #region Refrigerator
        public static IEnumerator Refrigerator_AboutToRun(Refrigerator __instance) // Non-Patch
        {
            BunnyHeader.ConsoleMessage.LogMessage(MethodBase.GetCurrentMethod().Name);

            __instance.interactable = false;

            __instance.PlayObjectSpriteEffect("FlashingRepeatedly");

            Vector3 particlePosition = new Vector3(__instance.tr.position.x, __instance.tr.position.y + 0.36f, __instance.tr.position.z);
            __instance.SpawnParticleEffect("Smoke", particlePosition);
            __instance.PlayAnim("MachineGoingToExplode", __instance.gc.playerAgent);
            __instance.RemoveObjectAgent();
            __instance.cantMakeFollowersAttack = true;

            yield return new WaitForSeconds(3f);
            if (!__instance.destroying)
                __instance.DestroyMe(Refrigerator_Variables[__instance].savedDamagerObject);

            yield break;
        }
        public static void Refrigerator_AnimationSequence(Refrigerator __instance) // Non-Patch
		{
            Refrigerator_Remora remora = Refrigerator_Variables[(Refrigerator)__instance];

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
        public static bool Refrigerator_DetermineButtons(Refrigerator __instance) // Postfix
		{
            BunnyHeader.ConsoleMessage.LogMessage(MethodBase.GetCurrentMethod().Name);

            MethodInfo DetermineButtons_Base = AccessTools.DeclaredMethod(typeof(ObjectReal), "DetermineButtons");
            DetermineButtons_Base.GetMethodWithoutOverrides<Action>(__instance).Invoke();

            if (__instance.interactingAgent.interactionHelper != null)
			{
                if (__instance.interactingAgent.interactionHelper.interactingFar)
                    __instance.buttons.Add("RefrigeratorRun");

                if ((__instance.interactingAgent.oma.superSpecialAbility && __instance.interactingAgent.agentName == "Hacker") || __instance.interactingAgent.statusEffects.hasTrait("HacksBlowUpObjects"))
                    __instance.buttons.Add("HackExplode");
            }
            else if (__instance.interactingAgent.inventory.HasItem("Wrench"))
            {
                __instance.buttons.Add("RefrigeratorRun");
                __instance.buttonsExtra.Add(" (" + __instance.interactingAgent.inventory.FindItem("Wrench").invItemCount + ") -" + BunnyTraits.ToolCost(__instance.interactingAgent));

                CustomName showChest = RogueLibs.CreateCustomName("ShowChest", "ButtonText",
                    new CustomNameInfo("Open container"));
                __instance.buttons.Add("ShowChest");
            }
            else
                __instance.ShowChest();
            return false;
		}
        public static void Refrigerator_FinishedOperating(Refrigerator __instance) // PostFix
		{
            BunnyHeader.ConsoleMessage.LogMessage(MethodBase.GetCurrentMethod().Name);

            if (__instance.interactingAgent.inventory.HasItem("Wrench"))
                __instance.ShowObjectButtons();
		}
  //      public static IEnumerator Refrigerator_IceDispense(Refrigerator __instance)
  //      {
  //          __instance.StopItemAudio();
  //          int num;
  //          for (int i = 0; i < 10; i = num + 1)
  //          {
  //              __instance.FireworksFire(0, 0, null);
  //              yield return new WaitForSeconds(Random.Range(0.3f, 0.6f));
  //              num = i;
  //          }
  //          Explosion explosion = __instance.gc.spawnerMain.SpawnExplosion(__instance, __instance.tr.position, "Normal", false, -1, false, true);
  //          explosion.agent = __instance.owner;
  //          explosion.realSource = __instance;
  //          __instance.DestroyMeFromClient();
  //          yield break;
  //      }
  //      public static void Refrigerator_IceShot(int bulletNetID, int bulletRotation, Agent fireworksOwner, Refrigerator __instance)
		//{
  //          if (fireworksOwner != null)
  //          {
  //              __instance.owner = fireworksOwner;
  //          }
  //          Bullet bullet = __instance.gc.spawnerMain.SpawnBullet(__instance.tr.position, bulletStatus.FreezeRay, __instance);
  //          float angle;
  //          if (__instance.gc.serverPlayer)
  //          {
  //              angle = (float)Random.Range(0, 359);
  //          }
  //          else
  //          {
  //              angle = (float)bulletRotation;
  //          }
  //          bullet.movement.RotateToAngleTransform(angle);
  //          bullet.movement.MoveForwardTransform(0.48f);
  //          __instance.gc.audioHandler.Play(__instance, "FireworksFire");
  //          if (__instance.gc.multiplayerMode && __instance.gc.serverPlayer)
  //          {
  //              bullet.bulletNetID = Random.Range(0, 10000);
  //              NetworkInstanceId extraObjectID = NetworkInstanceId.Invalid;
  //              if (__instance.owner != null)
  //              {
  //                  extraObjectID = __instance.owner.objectNetID;
  //              }
  //              __instance.gc.playerAgent.objectMult.ObjectAction(__instance.objectNetID, "FireworksFire", angle.ToString(), (float)bulletNetID, extraObjectID);
  //          }
  //      }
        public static bool Refrigerator_Interact(Agent agent, Refrigerator __instance) // Replacement
        {
            BunnyHeader.ConsoleMessage.LogMessage(MethodBase.GetCurrentMethod().Name);

            MethodInfo interact_base = AccessTools.DeclaredMethod(typeof(ObjectReal), "Interact", new Type[1] { typeof(Agent) });
            interact_base.GetMethodWithoutOverrides<Action<Agent>>(__instance).Invoke(__instance.interactingAgent);

            if (__instance.timer > 0f || __instance.startedFlashing)
                __instance.StopInteraction();

            if (__instance.interactingAgent.inventory.HasItem("Wrench"))
                __instance.ShowObjectButtons();

            else
                __instance.ShowChest();

            return false;
        }
        public static bool Refrigerator_InteractFar(Agent agent, Refrigerator __instance) // Prefix
		{
            BunnyHeader.ConsoleMessage.LogMessage(MethodBase.GetCurrentMethod().Name);

            if (__instance.timer > 0f || __instance.startedFlashing)
            {
                __instance.StopInteraction();
                return false;
            }
            return true;
        }
        public static void Refrigerator_ObjectAction(string myAction, string extraString, float extraFloat, Agent causerAgent, PlayfieldObject extraObject, Refrigerator __instance, ref bool ___noMoreObjectActions) // Replacement 
		{
            MethodInfo ObjectAction_Base = AccessTools.DeclaredMethod(typeof(ObjectReal), "ObjectAction", new Type[5] { typeof(string), typeof(string), typeof(float), typeof(Agent), typeof(PlayfieldObject) });
            ObjectAction_Base.GetMethodWithoutOverrides<Action<string, string, float, Agent, PlayfieldObject>>(__instance).Invoke(myAction, extraString, extraFloat, causerAgent, extraObject);

            if (!___noMoreObjectActions)
            {
                if (myAction == "ShowChest")
                    __instance.ShowChest();
                if (!(myAction == "RefrigeratorRun"))
                    if (myAction == "RefrigeratorRunClients")
                        __instance.RefrigeratorRunClients();
                else
                    __instance.RefrigeratorRun(causerAgent);
            }
            ___noMoreObjectActions = false;
        }
        public static bool Refrigerator_PressedButton(string buttonText, Refrigerator __instance) // Replacement
		{
            BunnyHeader.ConsoleMessage.LogMessage(MethodBase.GetCurrentMethod().Name);

            MethodInfo PressedButton_Base = AccessTools.DeclaredMethod(typeof(ObjectReal), "PressedButton", new Type[1] { typeof(string) });
            PressedButton_Base.GetMethodWithoutOverrides<Action<string>>(__instance).Invoke(buttonText);


            if (buttonText == "RefrigeratorRun" && __instance.interactingAgent.interactionHelper.interactingFar)
            {
                __instance.gc.audioHandler.Play(__instance.interactingAgent, "Success");
                __instance.RefrigeratorRun(__instance.interactingAgent);
                __instance.StopInteraction();
                return false;
            }
            else if (buttonText == "RefrigeratorRun" && !__instance.interactingAgent.interactionHelper.interactingFar)
            {
                //Manual tampering here
            }
            else if (buttonText == "ShowChest")
                __instance.ShowChest();

            __instance.StopInteraction(); // May need to remove this for second two options

            return false;
		}
        public static void Refrigerator_Start(Refrigerator __instance) // Postfix
		{
            Refrigerator_Remora remora = new Refrigerator_Remora();
            Refrigerator_Variables[__instance] = remora;
            remora.refrigeratorHost = __instance;
            // Untested:
            Refrigerator_Variables[(Refrigerator)__instance].animateSpriteID = __instance.spr.GetSpriteIdByName("Refrigerator");
            Refrigerator_Variables[(Refrigerator)__instance].animateSpriteID2 = __instance.spr.GetSpriteIdByName("Refrigerator");
        }
        public static void Refrigerator_UseWrenchToRun(Refrigerator __instance) // Non-Patch
		{
            BunnyHeader.ConsoleMessage.LogMessage(MethodBase.GetCurrentMethod().Name);

            if (__instance.gc.serverPlayer)
            {
                __instance.MakeNonFunctional(__instance.interactingAgent);
                __instance.interactingAgent.inventory.SubtractFromItemCount(__instance.interactingAgent.inventory.FindItem("Wrench"), 30);
                __instance.interactingAgent.skillPoints.AddPoints("TamperGeneratorPoints");
                __instance.gc.playerAgent.SetCheckUseWithItemsAgain(__instance);
                return;
            }
            __instance.functional = false;
            __instance.gc.playerAgent.SetCheckUseWithItemsAgain(__instance);
            __instance.interactingAgent.objectMult.ObjectAction(__instance.objectNetID, "UseWrenchToRun");

            //TODO: See Item.FireworksFire if you want to turn this into randomly shooting Freeze Rays
        }
        public static Dictionary<Refrigerator, Refrigerator_Remora> Refrigerator_Variables = new Dictionary<Refrigerator, Refrigerator_Remora>();
        #endregion
        #region PoolTable
        public static void PoolTable_SetVars(PoolTable __instance)
        {
            __instance.interactable = true;
            __instance.lowInteractionPriority = true;
        }
        #endregion
        #region Stove
        public static IEnumerator Stove_AboutToExplode(Stove __instance) // Non-Patch
		{
            BunnyHeader.ConsoleMessage.LogMessage("Stove_AboutToExplode");

            __instance.interactable = false;

            __instance.PlayObjectSpriteEffect("FlashingRepeatedly");
            
            if (__instance.lastHitByAgent != null)
            {
                __instance.gc.spawnerMain.SpawnNoise(__instance.tr.position, 1f, null, null, __instance.lastHitByAgent);
                __instance.gc.OwnCheck(__instance.lastHitByAgent, __instance.go, "Normal", 0);
            }

            Vector3 particlePosition = new Vector3(__instance.tr.position.x, __instance.tr.position.y + 0.36f, __instance.tr.position.z);
            __instance.SpawnParticleEffect("Smoke", particlePosition);
            __instance.PlayAnim("MachineGoingToExplode", __instance.lastHitByAgent); // Orig. gc.playerAgent
            __instance.gc.audioHandler.Play(__instance, "GeneratorHiss");
            __instance.RemoveObjectAgent();
            __instance.cantMakeFollowersAttack = true;

			yield return new WaitForSeconds(3f);
            if (!__instance.destroying)
                __instance.DestroyMe(Stove_Variables[__instance].savedDamagerObject);

			yield break;
        }
        public static void Stove_AnimationSequence(Stove __instance) // Non-Patch
		{
            Stove_Remora remora = Stove_Variables[(Stove)__instance];

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
            BunnyHeader.ConsoleMessage.LogMessage("Stove_DamagedObject");

            MethodInfo damagedObject = AccessTools.DeclaredMethod(typeof(ObjectReal), "DamagedObject");
            damagedObject.GetMethodWithoutOverrides<Action<PlayfieldObject, float>>(__instance).Invoke(damagerObject, damageAmount);

            if (damageAmount >= 15f && !__instance.startedFlashing)
            {
                Stove_Variables[__instance].savedDamagerObject = damagerObject;
                __instance.StartCoroutine(Stove_AboutToExplode(__instance));
            }

            if (damageAmount >= (float)__instance.damageThreshold)
            {
                Stove_Variables[__instance].savedDamagerObject = damagerObject;
                __instance.DestroyMe(damagerObject);
            }
            //TODO: Consider flame spit instead of flame particle
            return false;
        }
        public static void Stove_GrilledFud(Stove __instance) // Non-Patch 
        {
            InvItem rawFud = __instance.interactingAgent.inventory.FindItem("Fud");
            
            int numCooked = rawFud.invItemCount;

            rawFud.invItemCount -= numCooked;

            if (rawFud.invItemCount <= 0)
                __instance.interactingAgent.inventory.DestroyItem(rawFud);

            InvItem cookedFud = new InvItem();
            cookedFud.invItemName = "HotFud";
            cookedFud.SetupDetails(false);
            cookedFud.invItemCount = numCooked;
            __instance.interactingAgent.inventory.AddItemOrDrop(cookedFud);
            cookedFud.ShowPickingUpText(__instance.interactingAgent);

            __instance.gc.audioHandler.Play(__instance, "Grill");
            Stove_GrilledFudAfter(numCooked, __instance);
        }
        public static void Stove_GrilledFudAfter(int myCount, Stove __instance) // Non-Patch 
        {
            return;
        }
        public static void Stove_RevertAllVars(Stove __instance) // Postfix
        {
            Stove_Variables[__instance].mustSpawnExplosionOnClients = false;
            // Untested:
            Stove_Variables[__instance].animateSpriteID = 0;
            Stove_Variables[__instance].animateSpriteID2 = 0;
            __instance.GetComponent<Animator>().enabled = false;
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
            __instance.animates = true;
            __instance.canExplosiveStimulate = true;
            __instance.dontDestroyImmediateOnClient = true;
            __instance.hasUpdate = true;
            __instance.interactable = true;
        }
        public static void Stove_UseWrenchToDetonate(Stove __instance) // Non-Patch 
        {
            BunnyHeader.ConsoleMessage.LogMessage("Stove_UseWrenchToDetonate");

            if (__instance.gc.serverPlayer)
            {
                __instance.MakeNonFunctional(__instance.interactingAgent);
                __instance.interactingAgent.inventory.SubtractFromItemCount(__instance.interactingAgent.inventory.FindItem("Wrench"), 30);
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
		public static void TableBig_SetVars(TableBig __instance)
        {
            __instance.interactable = true;
            __instance.lowInteractionPriority = true;
        }
        #endregion
        #region Television
        public static void Television_SetVars(Television __instance)
        {
        }
        #endregion
    }
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
    public class Refrigerator_Remora
	{
        public Refrigerator refrigeratorHost;

        public int animateSpriteID;
        public int animateSpriteID2;
        public float animationCountdown;
        public int animationFrame;
        public PlayfieldObject savedDamagerObject;

        public void Countdown()
        {
            Refrigerator Refrigerator = BunnyObjects.Refrigerator_Variables.FirstOrDefault(x => x.Value == this).Key;

            string myText = string.Concat(Refrigerator.timeCountdownClock);

            if (Refrigerator.timeCountdownClock > 0 && !Refrigerator.destroyed && !Refrigerator.destroying)
                Refrigerator.gc.spawnerMain.SpawnStatusText(Refrigerator, "Countdown", myText);

            Refrigerator.timeCountdownClock--;

            if (Refrigerator.timeCountdownClock == 0 || Refrigerator.timeCountdownClock == -1 || Refrigerator.destroyed)
                Refrigerator.CancelInvoke();
        }
    }
}
