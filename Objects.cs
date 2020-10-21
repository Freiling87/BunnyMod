﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using BepInEx;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;
using BepInEx.Logging;

namespace BunnyMod
{
	[BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    [BepInProcess("StreetsOfRogue.exe")]
    [BepInDependency(RogueLibs.pluginGuid, "2.0")]

    public class Objects : BaseUnityPlugin
    {
		#region Mod Info
		public const string pluginGuid = "freiling87.streetsofrogue.bunnymod";
        public const string pluginName = "Bunny Mod";
        public const string pluginVersion = "1.0.0.0";
        #endregion
        #region Generic
        public static ManualLogSource ConsoleMessage;

        public void Awake()
        {
            ConsoleMessage = Logger;
            #region Names
            CustomName name_BurnedHands = RogueLibs.CreateCustomName("BurnedHands", "Dialogue", new CustomNameInfo("God fucking damn it, I always fucking burn my fucking hands!"));
            #endregion
            #region Patches

            this.PatchPrefix(typeof(ObjectReal), "DestroyMe", GetType(), "ObjectReal_DestroyMe");
            this.PatchPostfix(typeof(ObjectReal), "DetermineButtons", GetType(), "ObjectReal_DetermineButtons");
            this.PatchPrefix(typeof(ObjectReal), "FinishedOperating", GetType(), "ObjectReal_FinishedOperating");
            this.PatchPrefix(typeof(ObjectReal), "Interact", GetType(), "ObjectReal_Interact");
            this.PatchPrefix(typeof(ObjectReal), "MakeNonFunctional", GetType(), "ObjectReal_MakeNonFunctional");
            this.PatchPostfix(typeof(ObjectReal), "ObjectAction", GetType(), "ObjectReal_ObjectAction");
            this.PatchPrefix(typeof(ObjectReal), "ObjectUpdate", GetType(), "ObjectReal_ObjectUpdate");
            this.PatchPrefix(typeof(ObjectReal), "PressedButton", GetType(), "ObjectReal_PressedButton");
            this.PatchPostfix(typeof(ObjectReal), "Start", GetType(), "ObjectReal_Start");

            this.PatchPrefix(typeof(PlayfieldObject), "playerHasUsableItem", GetType(), "PlayfieldObject_PlayerHasUsableItem");

            this.PatchPostfix(typeof(StatusEffects), "BecomeHidden", GetType(), "StatusEffects_BecomeHidden");
            this.PatchPostfix(typeof(StatusEffects), "BecomeUnhidden", GetType(), "StatusEffects_BecomeUnhidden");

            this.PatchPostfix(typeof(Bathtub), "SetVars", GetType(), "Bathtub_SetVars");

            this.PatchPostfix(typeof(FlamingBarrel), "SetVars", GetType(), "FlamingBarrel_SetVars");

            this.PatchPostfix(typeof(Plant), "SetVars", GetType(), "Plant_SetVars");

            this.PatchPostfix(typeof(PoolTable), "SetVars", GetType(), "PoolTable_SetVars");

            this.PatchPrefix(typeof(Stove), "DamagedObject", GetType(), "Stove_DamagedObject");
            this.PatchPostfix(typeof(Stove), "RevertAllVars", GetType(), "Stove_RevertAllVars");
            this.PatchPostfix(typeof(Stove), "SetVars", GetType(), "Stove_SetVars");

            this.PatchPostfix(typeof(TableBig), "SetVars", GetType(), "TableBig_SetVars");
            #endregion
            #region Traits
            CustomTrait Alcoholic = RogueLibs.CreateCustomTrait("Alcoholic", true,
                new CustomNameInfo("Alcoholic"),
                new CustomNameInfo("")); //
            Alcoholic.CostInCharacterCreation = -6;
            Alcoholic.IsActive = false; //
            Alcoholic.Upgrade = null;

            CustomTrait ArtificialInsermonation = RogueLibs.CreateCustomTrait("ArtificialInsermonation", true,
                new CustomNameInfo("Artificial Insermonation"),
                new CustomNameInfo("Activate an Altar to deliver a Sermon, randomly improving relations with NPCs within earshot. They may donate tithes."));
            ArtificialInsermonation.CostInCharacterCreation = 2;
            ArtificialInsermonation.IsActive = false; //
            ArtificialInsermonation.Upgrade = "ArtificialInsermonation_2";

            CustomTrait ArtificialInsermonation_2 = RogueLibs.CreateCustomTrait("ArtificialInsermonation_2", true,
                new CustomNameInfo("Artificial Insermonation +"),
                new CustomNameInfo("Improved relationships and tithes from Sermonizing."));
            ArtificialInsermonation_2.CostInCharacterCreation = 2;
            ArtificialInsermonation_2.IsActive = false; //
            ArtificialInsermonation_2.Upgrade = null;

            CustomTrait Charmed = RogueLibs.CreateCustomTrait("Charmed", true,
                new CustomNameInfo("Charmed"),
                new CustomNameInfo("A permanent slight increase to your Luck."));
            Charmed.CanRemove = false;
            Charmed.CanSwap = false;
            Charmed.Conflicting.AddRange(new string[] { "Cursed" });
            Charmed.CostInCharacterCreation = 2;
            Charmed.IsActive = false; //
            Charmed.Upgrade = null;

            CustomTrait CheekyTrappy = RogueLibs.CreateCustomTrait("CheekyTrappy", true,
                new CustomNameInfo("Cheeky Trappy"),
                new CustomNameInfo("All hidden traps are visible to you. NPCs will no longer avoid your traps."));
            CheekyTrappy.CostInCharacterCreation = 1;
            CheekyTrappy.IsActive = false; //
            CheekyTrappy.Upgrade = null;

            CustomTrait Cursed = RogueLibs.CreateCustomTrait("Cursed", true,
                new CustomNameInfo("Cursed"),
                new CustomNameInfo("A permanent slight decrease to your Luck."));
            Cursed.CanRemove = false;
            Cursed.CanSwap = false;
            Cursed.Conflicting.AddRange(new string[] { "Charmed" });
            Cursed.CostInCharacterCreation = -1;
            Cursed.IsActive = false; //
            Cursed.Upgrade = null;

            CustomTrait DeathToSnitches = RogueLibs.CreateCustomTrait("DeathToSnitches", true,
                new CustomNameInfo("Death To Snitches"),
                new CustomNameInfo("Cops will ignore your Pusher attempts. You may attempt to sell to Cops, but failure will turn them hostile."));
            DeathToSnitches.AvailableInCharacterCreation = false;
            DeathToSnitches.IsActive = false; //
            DeathToSnitches.Upgrade = null;
            //TODO: Unlock DeathToSnitches when Pusher gained

            CustomTrait DrawNoBlood = RogueLibs.CreateCustomTrait("DrawNoBlood", true,
                new CustomNameInfo("Draw No Blood"),
                new CustomNameInfo("You have taken an oath. You cannot use bullet-based guns or sharp weapons."));
            DrawNoBlood.CanRemove = false;
            DrawNoBlood.CanSwap = false;
            DrawNoBlood.CostInCharacterCreation = -3;
            DrawNoBlood.IsActive = false; //
            DrawNoBlood.Upgrade = null;

            CustomTrait DAREdevil = RogueLibs.CreateCustomTrait("DAREdevil", true,
                new CustomNameInfo("DARE-Devil"),
                new CustomNameInfo("You cannot consume drugs."));
            DAREdevil.CanRemove = false;
            DAREdevil.CanSwap = false;
            DAREdevil.CostInCharacterCreation = -2;
            DAREdevil.IsActive = false; //
            DAREdevil.Upgrade = null;

            CustomTrait FriendOfBill = RogueLibs.CreateCustomTrait("FriendofBill", true,
                new CustomNameInfo("Friend Of Bill"),
                new CustomNameInfo("You cannot consume Alchohol."));
            FriendOfBill.CanRemove = false;
            FriendOfBill.CanSwap = false;
            FriendOfBill.CostInCharacterCreation = -2;
            FriendOfBill.IsActive = false; //
            FriendOfBill.Upgrade = null;

            CustomTrait HungryBoy = RogueLibs.CreateCustomTrait("HungryBoy", true,
                new CustomNameInfo("Hungry Boy"),
                new CustomNameInfo("Less healing from food and alcohol."));
            HungryBoy.CostInCharacterCreation = -3;
            HungryBoy.IsActive = false; //
            HungryBoy.Upgrade = null;

            CustomTrait OneHappyTamper = RogueLibs.CreateCustomTrait("OneHappyTamper", true,
                new CustomNameInfo("One Happy Tamper"),
                new CustomNameInfo("Owners will allow you to tamper with their belongings."));
            OneHappyTamper.CostInCharacterCreation = 2;
            OneHappyTamper.IsActive = false; //
            OneHappyTamper.Upgrade = null;

            CustomTrait PursuitOfTrappiness = RogueLibs.CreateCustomTrait("PursuitOfTrappiness", true,
                new CustomNameInfo("Pursuit Of Trappiness"),
                new CustomNameInfo("Un-Aligned NPCs take additional damage from Traps."));
            PursuitOfTrappiness.CostInCharacterCreation = 2;
            PursuitOfTrappiness.IsActive = false; //
            PursuitOfTrappiness.Upgrade = null;

            CustomTrait Pusher = RogueLibs.CreateCustomTrait("Pusher", true,
                new CustomNameInfo("Pusher"),
                new CustomNameInfo("You can interact with most NPCs to attempt to sell them any drug item you have (or simplified, just Sugar). If you fail, they become annoyed (Upper Crusters will call the cops immediately). Cops who witness a dealing attempt will go Hostile. If you succeed at a sale, they have a chance to become Hooked. After a certain interval of withdrawal, NPCs will gain the Jonesing status. They'll seek you out in the level and beg you for a particular drug. If you go too long without selling them the requested drug, they'll go hostile, but selling them other types of drugs will keep them at bay for a while. When Jonesing, they will freely give you keys and safe combos if you ask. Jonesing NPCs may also attack other drug dealers, doctors, or scientists if they can't track you down."));//
            Pusher.CanRemove = false;
            Pusher.CanSwap = false;
            Pusher.CostInCharacterCreation = 6;
            Pusher.IsActive = false; //
            Pusher.Upgrade = "Pusher_2";
            //TODO: Unlock DeathToSnitches when Pusher gained

            CustomTrait Pusher_2 = RogueLibs.CreateCustomTrait("Pusher_2", true,
                new CustomNameInfo("Pusher +"),
                new CustomNameInfo("Increased chances of success when pushing drugs."));//
            Pusher_2.AvailableInCharacterCreation = false;
            Pusher_2.IsActive = false; //
            Pusher_2.Upgrade = null;

            CustomTrait ReturnToBonke = RogueLibs.CreateCustomTrait("ReturnToBonke", true,
                new CustomNameInfo("Return to Bonke"),
                new CustomNameInfo("Chance to inflict Dizziness when striking an NPC with a blunt weapon."));
            ReturnToBonke.CostInCharacterCreation = 3;
            ReturnToBonke.IsActive = false; //
            ReturnToBonke.Upgrade = null;

            CustomTrait StealthBastardDeluxe = RogueLibs.CreateCustomTrait("StealthBastardDeluxe", true,
                new CustomNameInfo("Stealth Bastard Deluxe"),
                new CustomNameInfo("Allows you to hide in Bathtubs, Plants, Pool Tables, and Big Tables.")); 
            StealthBastardDeluxe.Conflicting.AddRange(new string[] { "Loud" });
            StealthBastardDeluxe.CostInCharacterCreation = 4;
            StealthBastardDeluxe.IsActive = false; //
            StealthBastardDeluxe.Upgrade = null;

            CustomTrait SuffersToolsGladly = RogueLibs.CreateCustomTrait("SuffersToolsGladly", true,
                new CustomNameInfo("Suffers Tools Gladly"),
                new CustomNameInfo("Your tools take less wear from tampering."));
            SuffersToolsGladly.CostInCharacterCreation = 1;
            SuffersToolsGladly.IsActive = false; //
            SuffersToolsGladly.Upgrade = "SticklerForTools_2";

            CustomTrait SuffersToolsGladly_2 = RogueLibs.CreateCustomTrait("SuffersToolsGladly_2", true,
                new CustomNameInfo("Suffers Tools Gladly +"),
                new CustomNameInfo("Your tools take zero wear when used in tampering."));
            SuffersToolsGladly_2.AvailableInCharacterCreation = false;
            SuffersToolsGladly_2.IsActive = false;//
            SuffersToolsGladly_2.Upgrade = null;

            CustomTrait Teetotaller = RogueLibs.CreateCustomTrait("Teetotaller", true,
                new CustomNameInfo("Teetotaller"),
                new CustomNameInfo("You cannot consume drugs or alcohol."));
            Teetotaller.CostInCharacterCreation = -4;
            Teetotaller.IsActive = false; //
            Teetotaller.Upgrade = null;

            CustomTrait TrapperKeeper = RogueLibs.CreateCustomTrait("TrapperKeeper", true,
                new CustomNameInfo("Trapper Keeper"),
                new CustomNameInfo("Interact with Traps to add them to your inventory. 100% chance to disarm Door Detonators."));
            TrapperKeeper.CostInCharacterCreation = 2;
            TrapperKeeper.IsActive = false; //
            TrapperKeeper.Upgrade = null;

            CustomTrait UndeadBane = RogueLibs.CreateCustomTrait("UndeadBane", true,
                new CustomNameInfo("Undead Bane"),
                new CustomNameInfo("The undead fear and hate you. They're probably just jealous. All Vampires, Zombies & Ghosts are hostile on sight."));
            UndeadBane.CostInCharacterCreation = -4;
            UndeadBane.IsActive = false; //
            UndeadBane.Upgrade = null;

            CustomTrait VeiledThreats = RogueLibs.CreateCustomTrait("VeiledThreats", true,
                new CustomNameInfo("Veiled Threats"),
                new CustomNameInfo("When you attempt to Bribe, Extort, Mug, or Threaten, a failure will turn the target Annoyed instead of Hostile."));
            VeiledThreats.CostInCharacterCreation = 3;
            VeiledThreats.IsActive = false; //
            VeiledThreats.Upgrade = null;

            CustomTrait Whiffist = RogueLibs.CreateCustomTrait("Whiffist", true,
                new CustomNameInfo("Whiffist"),
                new CustomNameInfo("Small chance for Melee or Thrown attacks to miss you completely."));
            Whiffist.CostInCharacterCreation = 3;
            Whiffist.IsActive = false; //
            Whiffist.Upgrade = "Whiffist_2";
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
            ConsoleMessage.LogMessage("ObjectReal_FinishedOperating");

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

            if (__instance is Bathtub || __instance is Plant || __instance is PoolTable || __instance is TableBig)
            {
                //TODO: Disable objects' "move toward wall" behavior when generating a chunk.
                //TODO: Alternatively, make them non-blocking to movement when a player is inside it.

                agent.statusEffects.BecomeHidden(__instance);

                //Possible fixes for stuck hider, but you will need to find out how to undo them:
                //agent.agentItemColliderTr.gameObject.SetActive(false);
                //__instance.onShadowLayer = true;
                //__instance.colliderSize = "InPrefab";
                //__instance.originalBoxColliderActive = false;
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
            ConsoleMessage.LogMessage("ObjectReal_MakeNonFunctional");

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
            ConsoleMessage.LogMessage("ObjectReal_ObjectAction");

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
            ConsoleMessage.LogMessage("ObjectReal_ObjectUpdate");

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
            ConsoleMessage.LogMessage("ObjectReal_PressedButton");

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
        public static void ObjectReal_Start(ObjectReal __instance)
        {
            if (__instance is Stove stove)
                VariablesStove.Add(stove, new VariablesStove());
        }
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
		#region StatusEffects
        public static void StatusEffects_BecomeHidden(ObjectReal hiddenInObject, StatusEffects __instance)
		{
            if (hiddenInObject is Bathtub || hiddenInObject is Plant || hiddenInObject is PoolTable || hiddenInObject is TableBig)
			{
                __instance.agent.agentItemColliderTr.gameObject.SetActive(false);
			}
		}
        public static void StatusEffects_BecomeNotHidden(StatusEffects __instance)
		{
            __instance.agent.agentItemColliderTr.gameObject.SetActive(true);
		}
		#endregion

		#region Objects
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
            ConsoleMessage.LogMessage("Stove_AboutToExplode");
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
            ConsoleMessage.LogMessage("Stove_DamagedObject");

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
            //TODO: Consider flame spit instead of flame particle
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
            ConsoleMessage.LogMessage("Stove_UseWrenchToDetonate");

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
		#endregion
		#region Traits

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

