using BepInEx;
using HarmonyLib;
using RogueLibsCore;
using System;
using System.Reflection;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.IO;


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
            Logger.LogInfo("BunnyMod loaded");

            this.PatchPrefix(typeof(ObjectReal), "Start", GetType(), "Start_ObjectReal");

            this.PatchPostfix(typeof(Bathtub), "Interact", GetType(), "Interact_Bathtub");
            this.PatchPostfix(typeof(Bathtub), "SetVars", GetType(), "SetVars_Bathtub");

            this.PatchPrefix(typeof(Stove), "DamagedObject", GetType(), "DamagedObject_Stove");
            this.PatchPostfix(typeof(Stove), "DetermineButtons", GetType(), "DetermineButtons_Stove");
            this.PatchPrefix(typeof(Stove), "FinishedOperating", GetType(), "FinishedOperating_Stove");
            this.PatchPrefix(typeof(Stove), "Interact", GetType(), "Interact_Stove");
            this.PatchPrefix(typeof(Stove), "MakeNonFunctional", GetType(), "MakeNonFunctional_Stove");
            this.PatchPostfix(typeof(Stove), "ObjectAction", GetType(), "ObjectAction_Stove");
            this.PatchPrefix(typeof(Stove), "ObjectUpdate", GetType(), "ObjectUpdate_Stove");
            this.PatchPrefix(typeof(Stove), "playerHasUsableItem", GetType(), "PlayerHasUsableItem_Stove");
            this.PatchPostfix(typeof(Stove), "PressedButton", GetType(), "PressedButton_Stove");
            this.PatchPostfix(typeof(Stove), "RevertAllVars", GetType(), "RevertAllVars_Stove");
            this.PatchPostfix(typeof(Stove), "SetVars", GetType(), "SetVars_Stove");
            this.PatchPostfix(typeof(Stove), "Start", GetType(), "Start_Stove");

        }
        public static bool Start(ObjectReal __instance)
		{
            if (__instance is Stove stove)
                VariablesStove.Add(stove, new VariablesStove());

            return true;
		}
        public void FixedUpdate()
        {
            // You will need to check if any of the stoves were destroyed

            List<Stove> removal = new List<Stove>();

            foreach (KeyValuePair<Stove, VariablesStove> pair in VariablesStove)
                if (pair.Key.isBroken())
                    removal.Add(pair.Key);

            foreach (Stove stove in removal)
                VariablesStove.Remove(stove);
        }
        #endregion
		#region Bathtub
        public static void Interact_BathTub(Agent agent, Bathtub __instance)
		{
            __instance.Interact(agent);

            agent.statusEffects.BecomeHidden(__instance);

            __instance.StopInteraction();
        }
		public static void SetVars_Bathtub(Bathtub __instance)
        {
            __instance.interactable = true;
        }
        #endregion
        #region Stove
        public static Dictionary<Stove, VariablesStove> VariablesStove = new Dictionary<Stove, VariablesStove>();

        public static IEnumerator AboutToExplode_Stove(Stove __instance) //Non-patch
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

            MethodInfo removeObjectAgent = AccessTools.DeclaredMethod(typeof(ObjectReal).BaseType, "RemoveObjectAgent");
            removeObjectAgent.GetMethodWithoutOverrides<Action>(__instance).Invoke();

            __instance.RemoveObjectAgent();
            __instance.cantMakeFollowersAttack = true;

			yield return new WaitForSeconds(3f);
            if (!__instance.destroying)
                __instance.DestroyMe(VariablesStove[__instance].savedDamagerObject);

			yield break;
        }//√
        public static void AnimationSequence_Stove(Stove __instance) //Non-patch
		{
            if (!__instance.destroying && __instance.activeObject && !__instance.notInOriginalLocation && __instance.spawnedShadow && __instance.onCamera)
            {
                VariablesStove[__instance].animationCountdown -= Time.deltaTime;
                if (VariablesStove[__instance].animationCountdown <= 0f)
                {
                    if (VariablesStove[__instance].animationFrame == 0)
                    {
                        __instance.ChangeSpriteByID(VariablesStove[__instance].animateSpriteID2);
                        VariablesStove[__instance].animationFrame = 1;
                    }
                    else
                    {
                        __instance.ChangeSpriteByID(VariablesStove[__instance].animateSpriteID);
                        VariablesStove[__instance].animationFrame = 0;
                    }
                    VariablesStove[__instance].animationCountdown = 0.5f;
                }
            }
        }//√
		public static void Countdown_Stove(Stove __instance) // Non-Patch
		{
            if (!__instance.gc.cinematic)
            {
                string myText = string.Concat(__instance.timeCountdownClock);

                if (__instance.timeCountdownClock > 0 && !__instance.destroyed && !__instance.destroying)
                    __instance.gc.spawnerMain.SpawnStatusText(__instance, "Countdown", myText);

                __instance.timeCountdownClock--;

                if (__instance.timeCountdownClock == 0 || __instance.timeCountdownClock == -1 || __instance.destroyed)
                    __instance.CancelInvoke();
            }
        } // Possibly un-used, according to DNSpy // √
        public static bool DamagedObject_Stove(PlayfieldObject damagerObject, float damageAmount, Stove __instance)
		{
            MethodInfo damagedObject = AccessTools.DeclaredMethod(typeof(Stove).BaseType, "damagedObject");
            damagedObject.GetMethodWithoutOverrides<Action<PlayfieldObject, float>>(__instance).Invoke(damagerObject, damageAmount);

            if (damageAmount >= 15f && !__instance.startedFlashing)
            {
                VariablesStove[__instance].savedDamagerObject = damagerObject;
                __instance.StartCoroutine(AboutToExplode_Stove(__instance));
            }

            if (damageAmount >= (float)__instance.damageThreshold)
            {
                VariablesStove[__instance].savedDamagerObject = damagerObject;
                __instance.DestroyMe(damagerObject);
            }

            return false;
        }//√
        public static void DetermineButtons_Stove(Stove __instance)
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
            //if (__instance.interactingAgent.inventory.HasItem("ToolKit"))
            //{
            //    if (!__instance.startedFlashing)
            //    {
            //        __instance.buttons.Add("UseWrenchToDetonate");
            //        __instance.buttonsExtra.Add(" (" + __instance.interactingAgent.inventory.FindItem("ToolKit").invItemCount + ") -0");
            //        return;
            //    }
            //}
            else
                __instance.interactingAgent.SayDialogue("CantOperateGenerator");
        }//√
        public static bool FinishedOperating_Stove(Stove __instance)
		{
            MethodInfo finishedOperating_base = AccessTools.DeclaredMethod(typeof(Stove).BaseType, "finishedOperating");
            finishedOperating_base.GetMethodWithoutOverrides<Action>(__instance).Invoke();

            string invItemName = __instance.operatingItem.invItemName;

            if (invItemName == "Wrench" || invItemName == "ToolKit")
            {
                UseWrenchToDetonate_Stove(__instance);
                __instance.StopInteraction();
            }

            return false;
        }// This one may need to be on ObjectReal instead, since it's called by PlayfieldObject elsewhere //√
        public static void Interact_Stove(Agent agent, Stove __instance)
		{
            MethodInfo interact = AccessTools.DeclaredMethod(typeof(ObjectReal).BaseType, "Interact");
            interact.GetMethodWithoutOverrides<Action<Agent>>(__instance).Invoke(agent);

            if (__instance.timer > 0f || __instance.startedFlashing) //May not have flashing in this
                __instance.StopInteraction();

            __instance.ShowObjectButtons();
        }// Patch this one to patch ObjectReal? If I understood correctly. //√
        public static bool MakeNonFunctional_Stove(PlayfieldObject damagerObject, Stove __instance)
		{
            if (damagerObject != null && __instance.interactable)
            {
                MethodInfo makeNonFunctional = AccessTools.DeclaredMethod(typeof(ObjectReal).BaseType, "makeNonFunctional");
                makeNonFunctional.GetMethodWithoutOverrides<Action<PlayfieldObject>>(__instance).Invoke(damagerObject);

                __instance.timer = 10f;
                __instance.timeCountdownClock = (int)__instance.timer;
                __instance.InvokeRepeating("Countdown", 0.01f, 1f);
                __instance.interactable = false;
                VariablesStove[__instance].savedDamagerObject = damagerObject;
                VariablesStove[__instance].countdownCauser = VariablesStove[__instance].savedDamagerObject;
            }
            return false;
        }//√
        public static void ObjectAction_Stove(string myAction, string extraString, float extraFloat, Agent causerAgent, PlayfieldObject extraObject, Stove __instance, ref bool __noMoreObjectActions)
		{
            if (!__noMoreObjectActions && myAction == "UseWrenchToDetonate")
                UseWrenchToDetonate_Stove(__instance);

            __noMoreObjectActions = false;
        }//√
        public static bool ObjectUpdate_Stove(Stove __instance)
		{
            AnimationSequence_Stove(__instance);

			if (__instance.timer > 0f)
			{
				__instance.timer -= Time.deltaTime;

				if (__instance.timer <= 0f)
				{
					if (__instance.startedFlashing)
					{
						__instance.DestroyMe(VariablesStove[__instance].savedDamagerObject);
						return false;
					}
                    VariablesStove[__instance].noOwnCheckCountdown = true;
                    VariablesStove[__instance].savedDamagerObject = VariablesStove[__instance].countdownCauser;
					__instance.DestroyMe(VariablesStove[__instance].countdownCauser);
				}
			}
            return false;
		}//√
        public static bool PlayerHasUsableItem_Stove(InvItem myItem, Stove __instance) => // Non-patch
            (myItem.invItemName == "Wrench" || myItem.invItemName == "ToolKit") && __instance.timer == 0f && !__instance.startedFlashing;//√
        public static void PressedButton_Stove(string buttonText, int buttonPrice, Stove __instance)
		{
            MethodInfo pressedButton = AccessTools.DeclaredMethod(typeof(Stove).BaseType, "PressedButton");
            pressedButton.GetMethodWithoutOverrides<Action<string, int>>(__instance).Invoke(buttonText, buttonPrice);

            if (buttonText == "UseWrenchToDetonate")
            {
                __instance.StartCoroutine(__instance.Operating(__instance.interactingAgent, __instance.interactingAgent.inventory.FindItem("Wrench"), 2f, true, "Tampering"));
                return;
            }
            __instance.StopInteraction();
        }//√
		public static void SetVars_Stove(Stove __instance)
        {
            __instance.canExplosiveStimulate = true;
            __instance.dontDestroyImmediateOnClient = true;
            __instance.hasUpdate = true;
            __instance.interactable = true;
            __instance.animates = true;
        }//√
        public static void RevertAllVars_Stove(Stove __instance)
        {
            VariablesStove[__instance].mustSpawnExplosionOnClients = false;
            VariablesStove[__instance].animateSpriteID = 0;
            VariablesStove[__instance].animateSpriteID2 = 0;
            VariablesStove[__instance].savedDamagerObject = null;
            __instance.GetComponent<Animator>().enabled = false;
            VariablesStove[__instance].noOwnCheckCountdown = false;
            VariablesStove[__instance].countdownCauser = null;
            __instance.objectSprite.transform.Find("RealSprite").transform.localPosition = Vector3.zero;
            __instance.objectSprite.transform.Find("RealSprite").transform.localScale = Vector3.one;
            __instance.CancelInvoke();
        }//√
        public static void UseWrenchToDetonate_Stove(Stove __instance) // Non-Patch
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
        }//√
        #endregion
    }

    public class VariablesStove
    {
        public int animateSpriteID;
        public int animateSpriteID2;
        public float animationCountdown;
        public int animationFrame;
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


