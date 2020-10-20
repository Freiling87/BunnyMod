using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using BepInEx;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;

namespace BunnyMod
{
	[BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    [BepInProcess("StreetsOfRogue.exe")]
    [BepInDependency(RogueLibs.pluginGuid, "2.0")]

    public class ObjectPatches : BaseUnityPlugin
    {
		#region Mod Info
		public const string pluginGuid = "freiling87.streetsofrogue.bunnymod";
        public const string pluginName = "Bunny Mod";
        public const string pluginVersion = "1.0.0.0";
		#endregion
		#region Generic
		public void Awake()
		{
            #region Names
            CustomName name_BurnedHands = RogueLibs.CreateCustomName("BurnedHands", "Dialogue", new CustomNameInfo("God damn it, I always burn my fucking hands!"));
			#endregion
			#region Patches
			this.PatchPrefix(typeof(PlayfieldObject), "playerHasUsableItem", GetType(), "PlayfieldObject_PlayerHasUsableItem");

            this.PatchPrefix(typeof(ObjectReal), "DestroyMe", GetType(), "ObjectReal_DestroyMe");
            this.PatchPostfix(typeof(ObjectReal), "DetermineButtons", GetType(), "ObjectReal_DetermineButtons");
            this.PatchPrefix(typeof(ObjectReal), "FinishedOperating", GetType(), "ObjectReal_FinishedOperating");
            this.PatchPrefix(typeof(ObjectReal), "Interact", GetType(), "ObjectReal_Interact");
            this.PatchPrefix(typeof(ObjectReal), "MakeNonFunctional", GetType(), "ObjectReal_MakeNonFunctional");
            this.PatchPostfix(typeof(ObjectReal), "ObjectAction", GetType(), "ObjectReal_ObjectAction");
            this.PatchPrefix(typeof(ObjectReal), "ObjectUpdate", GetType(), "ObjectReal_ObjectUpdate");
            this.PatchPrefix(typeof(ObjectReal), "PressedButton", GetType(), "ObjectReal_PressedButton");
            this.PatchPostfix(typeof(ObjectReal), "Start", GetType(), "ObjectReal_Start");

            this.PatchPostfix(typeof(Bathtub), "SetVars", GetType(), "Bathtub_SetVars");

            this.PatchPostfix(typeof(FlamingBarrel), "SetVars", GetType(), "FlamingBarrel_SetVars");

            this.PatchPostfix(typeof(Plant), "SetVars", GetType(), "Plant_SetVars");

            this.PatchPostfix(typeof(PoolTable), "SetVars", GetType(), "PoolTable_SetVars");

            this.PatchPrefix(typeof(Stove), "DamagedObject", GetType(), "Stove_DamagedObject");
            this.PatchPostfix(typeof(Stove), "RevertAllVars", GetType(), "Stove_RevertAllVars");
            this.PatchPostfix(typeof(Stove), "SetVars", GetType(), "Stove_SetVars");

            this.PatchPostfix(typeof(TableBig), "SetVars", GetType(), "TableBig_SetVars");
			#endregion
		}
		public void FixedUpdate()
        {
            // You will need to check if any of the stoves were destroyed

            List<Stove> removal = new List<Stove>();

            foreach (KeyValuePair<Stove, VariablesStove> pair in VariablesStove)
                if (pair.Key.isBroken())
				{
                    removal.Add(pair.Key);
                    Logger.LogInfo("Added pair.Key from VariablesStove to removal list");
                }

            foreach (Stove stove in removal)
            {
                VariablesStove.Remove(stove);
                Logger.LogInfo("Removed pair.Key from VariablesStove");
            }
        } //TODO: Where should this be patched?
		#endregion
		#region PlayfieldObject
		public static bool PlayfieldObject_PlayerHasUsableItem(InvItem myItem, PlayfieldObject __instance, ref bool __result)
        {
            if (__instance is Stove)
			{
                Stove stove = (Stove)__instance;
                return (myItem.invItemName == "Wrench") 
                    && __instance.timer == 0f 
                    && !stove.startedFlashing; 
            }
            else
			{
                __result = true;
                return false;
			}
        }    
        #endregion
        #region ObjectReal
        public static bool ObjectReal_DestroyMe(PlayfieldObject damagerObject, ObjectReal __instance)
		{
            if (__instance is Stove)
			{
                VariablesStove[(Stove)__instance].savedDamagerObject = damagerObject;
            }
            return true;
		}
        public static void ObjectReal_DetermineButtons(ObjectReal __instance)
        {
            if (__instance is FlamingBarrel)
			{
                if (__instance.ora.hasParticleEffect)
				{
                    if (__instance.interactingAgent.inventory.HasItem("Fud"))
                    {
                        __instance.buttons.Add("GrillFud");
                        return;
                    }
                    __instance.interactingAgent.SayDialogue("CantGrillFud");
                }
				else
				{
                    if (__instance.interactingAgent.inventory.HasItem("CigaretteLighter"))
					{
                        __instance.buttons.Add("LightBarbecue");
                        return;
					}
                    __instance.interactingAgent.SayDialogue("CantOperateBarbecue");
                    return;
				}
			}
            if (__instance is Stove)
			{
                if (__instance.interactingAgent.inventory.HasItem("Wrench"))
                {
                    if (!__instance.startedFlashing)
                    {
                        __instance.buttons.Add("UseWrenchToDetonate");
                        __instance.buttonsExtra.Add(" (" + __instance.interactingAgent.inventory.FindItem("Wrench").invItemCount + ") -30");
                        return;
                    }
                }
                else
                    __instance.interactingAgent.SayDialogue("CantOperateGenerator");
            }
        }
        public static bool ObjectReal_FinishedOperating(ObjectReal __instance)
        {
            if (__instance is FlamingBarrel)
			{
                FlamingBarrel_GrilledFud((FlamingBarrel)__instance);
                __instance.StopInteraction();
			}
            if (__instance is Stove)
            {
                if (__instance.operatingItem.invItemName == "Wrench")
                {
                    Stove_UseWrenchToDetonate((Stove)__instance);

                    MethodInfo stopInteraction_base = AccessTools.DeclaredMethod(typeof(PlayfieldObject), "StopInteraction", new Type[0]);
                    stopInteraction_base.GetMethodWithoutOverrides<Action>(__instance).Invoke();

                    return false;
                }
            }

            MethodInfo finishedOperating_base = AccessTools.DeclaredMethod(typeof(PlayfieldObject), "FinishedOperating");
            finishedOperating_base.GetMethodWithoutOverrides<Action>(__instance).Invoke();

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
        public static bool ObjectReal_Interact(Agent agent, ObjectReal __instance)
        {
            MethodInfo interact_base = AccessTools.DeclaredMethod(typeof(PlayfieldObject), "Interact");
            interact_base.GetMethodWithoutOverrides<Action<Agent>>(__instance).Invoke(agent);

            __instance.playerInvDatabase = agent.GetComponent<InvDatabase>();

            if (__instance is Bathtub || __instance is Plant || __instance is PoolTable)
            {
                //TODO: Disable objects' "move toward wall" behavior when generating a chunk.
                //TODO: Alternatively, make them non-blocking to movement when a player is inside it.

                agent.statusEffects.BecomeHidden(__instance);

                //Possible fixes for stuck hider, but you will need to find out how to undo them:
                //agent.agentItemColliderTr.gameObject.SetActive(false);
                //__instance.onShadowLayer = true;
                //__instance.colliderSize = "InPrefab";
                // Then patch StatusEffects.BecomeNotHidden() to reenable whatever you disable to make it passable.

                MethodInfo stopinteraction_base = AccessTools.DeclaredMethod(typeof(PlayfieldObject), "StopInteraction", new Type[0]);
                stopinteraction_base.GetMethodWithoutOverrides<Action>(__instance).Invoke();
            }
            if (__instance is FlamingBarrel)
			{
                __instance.ShowObjectButtons();
			}
            if (__instance is Stove)
            {
                if (__instance.timer > 0f || __instance.startedFlashing)
                    __instance.StopInteraction();

                __instance.ShowObjectButtons();
            }

            return false;
        }
        public static bool ObjectReal_MakeNonFunctional(PlayfieldObject damagerObject, ObjectReal __instance)
        {
            if (__instance is Stove)
			{
                if (damagerObject != null && __instance.interactable)
                {
                    __instance.gc.playerAgent.SetCheckUseWithItemsAgain(__instance);
                    if (!__instance.gc.serverPlayer)
                    {
                        __instance.gc.playerAgent.objectMult.ObjectAction(__instance.objectNetID, "MakeNonFunctional");
                    }

                    __instance.timer = 10f;
                    __instance.timeCountdownClock = (int)__instance.timer;
                    __instance.InvokeRepeating("Countdown", 0.01f, 1f);
                    __instance.interactable = false;
                    VariablesStove[(Stove)__instance].savedDamagerObject = damagerObject;
                    VariablesStove[(Stove)__instance].countdownCauser = VariablesStove[(Stove)__instance].savedDamagerObject;
                }
                return false;
            }
            return true;
        }
        public static void ObjectReal_ObjectAction(string myAction, string extraString, float extraFloat, Agent causerAgent, PlayfieldObject extraObject, ObjectReal __instance, ref bool ___noMoreObjectActions)
        {
            if (__instance is Stove)
			{
                if (!___noMoreObjectActions && myAction == "UseWrenchToDetonate")
                {
                    Stove_UseWrenchToDetonate((Stove)__instance);
                }
                ___noMoreObjectActions = false;
            }
        }
        public static bool ObjectReal_ObjectUpdate(ObjectReal __instance)
        {
            if (__instance is Stove)
			{
                if (__instance.timer > 0f)
                {
                    __instance.timer -= Time.deltaTime;

                    if (__instance.timer <= 0f)
                    {
                        if (__instance.startedFlashing)
                        {
                            __instance.DestroyMe(VariablesStove[(Stove)__instance].savedDamagerObject);
                            return false;
                        }
                        VariablesStove[(Stove)__instance].noOwnCheckCountdown = true;
                        VariablesStove[(Stove)__instance].savedDamagerObject = VariablesStove[(Stove)__instance].countdownCauser;
                        __instance.DestroyMe(VariablesStove[(Stove)__instance].countdownCauser);
                    }
                }
                return false;
            }
            return true;
        }
        public static bool ObjectReal_PressedButton(string buttonText, int buttonPrice, ObjectReal __instance)
        {
            MethodInfo pressedButton_Base = AccessTools.DeclaredMethod(typeof(PlayfieldObject), "PressedButton", new Type[1]); //TODO: Verify if index necessary here
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
                __instance.StartCoroutine(__instance.Operating(__instance.interactingAgent, null, 2f, true, "Grilling"));
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
        public static void ObjectReal_Start(ObjectReal __instance)
		{
            if (__instance is Stove stove)
                VariablesStove.Add(stove, new VariablesStove());
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
        public static void FlamingBarrel_GrilledFud(FlamingBarrel __instance)
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
        public static void FlamingBarrel_GrilledFudAfter(int myCount, FlamingBarrel __instance)
		{
            __instance.gc.audioHandler.Play(__instance, "FireHit");
            __instance.interactingAgent.Damage(__instance);
            __instance.interactingAgent.statusEffects.ChangeHealth((float)5, __instance);
            __instance.interactingAgent.SayDialogue("BurnedHands");
            return;
		}
        public static void FlamingBarrel_SetVars(FlamingBarrel __instance)
		{
            __instance.interactable = true;
            __instance.fireDoesntDamage = true;
		}
        #endregion
        #region Plant
        public static void Plant_SetVars(Plant __instance)
        {
            __instance.interactable = true;
        }
        #endregion
        #region PoolTable
        public static void PoolTable_SetVars(PoolTable __instance)
        {
            __instance.interactable = true;
        }
        #endregion
        #region Stove
        public static Dictionary<Stove, VariablesStove> VariablesStove = new Dictionary<Stove, VariablesStove>();

        public static IEnumerator Stove_AboutToExplode(Stove __instance)
		{
            __instance.interactable = false;
            __instance.PlayObjectSpriteEffect("FlashingRepeatedly");
            
            if (__instance.lastHitByAgent != null)
            {
                __instance.gc.spawnerMain.SpawnNoise(__instance.tr.position, 1f, null, null, __instance.lastHitByAgent);
                __instance.gc.OwnCheck(__instance.lastHitByAgent, __instance.go, "Normal", 0);
            }

            Vector3 particlePosition = new Vector3(__instance.tr.position.x, __instance.tr.position.y + 0.36f, __instance.tr.position.z);
            __instance.SpawnParticleEffect("Smoke", particlePosition);
            __instance.gc.audioHandler.Play(__instance, "GeneratorHiss");
            __instance.RemoveObjectAgent();
            __instance.cantMakeFollowersAttack = true;

			yield return new WaitForSeconds(3f);
            if (!__instance.destroying)
                __instance.DestroyMe(VariablesStove[__instance].savedDamagerObject);

			yield break;
        }
        public static bool Stove_DamagedObject(PlayfieldObject damagerObject, float damageAmount, Stove __instance)
		{
            MethodInfo damagedObject = AccessTools.DeclaredMethod(typeof(ObjectReal), "DamagedObject");
            damagedObject.GetMethodWithoutOverrides<Action<PlayfieldObject, float>>(__instance).Invoke(damagerObject, damageAmount);

            if (damageAmount >= 15f && !__instance.startedFlashing)
            {
                VariablesStove[__instance].savedDamagerObject = damagerObject;
                __instance.StartCoroutine(Stove_AboutToExplode(__instance));
            }

            if (damageAmount >= (float)__instance.damageThreshold)
            {
                VariablesStove[__instance].savedDamagerObject = damagerObject;
                __instance.DestroyMe(damagerObject);
            }
            //TODO: Ensure Flames for all destruction types.
            return false;
        }
        public static void Stove_SetVars(Stove __instance)
        {
            __instance.canExplosiveStimulate = true;
            __instance.dontDestroyImmediateOnClient = true;
            __instance.hasUpdate = true;
            __instance.interactable = true;
        }
        public static void Stove_RevertAllVars(Stove __instance)
        {
            VariablesStove[__instance].mustSpawnExplosionOnClients = false;
            VariablesStove[__instance].savedDamagerObject = null;
            VariablesStove[__instance].noOwnCheckCountdown = false;
            VariablesStove[__instance].countdownCauser = null;
            __instance.objectSprite.transform.Find("RealSprite").transform.localPosition = Vector3.zero;
            __instance.objectSprite.transform.Find("RealSprite").transform.localScale = Vector3.one;
            __instance.CancelInvoke();
        }
        public static void Stove_UseWrenchToDetonate(Stove __instance)
        {
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
        #endregion
        #region TableBig
        public static void TableBig_SetVars(TableBig __instance)
        {
            __instance.interactable = true;
        }
        #endregion
    }

    public class VariablesStove
    {
        public PlayfieldObject countdownCauser;
        public bool mustSpawnExplosionOnClients;
        public bool noOwnCheckCountdown;
        public PlayfieldObject savedDamagerObject;
    }

    public static class SSS
    {
        public static T GetMethodWithoutOverrides<T>(this MethodInfo method, object callFrom)
            where T : Delegate
        {
            IntPtr ptr = method.MethodHandle.GetFunctionPointer();
            return (T)Activator.CreateInstance(typeof(T), callFrom, ptr);
        }
    }
}


