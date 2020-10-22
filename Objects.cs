using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;

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
        public const string pluginVersion = "1.1.0";
        #endregion
        #region Generic
        public static ManualLogSource ConsoleMessage;

        public void Awake()
        {
            ConsoleMessage = Logger;
            RoguePatcher patcher = new RoguePatcher(this, GetType());
            #region Items
            #region BeerCan
            Sprite sprite_beerCan = RogueUtilities.ConvertToSprite(Properties.Resources.BeerCan);

            CustomItem BeerCan = RogueLibs.CreateCustomItem("BeerCan", sprite_beerCan, true,
                new CustomNameInfo("Beer Can"),
                new CustomNameInfo("Probably best just to get the money from recycling this."),
                item =>
                {
                    item.itemType = "WeaponThrown";
                    item.weaponCode = weaponType.WeaponThrown;
                    item.Categories.Add("Weapons");
                    item.Categories.Add("NonViolent");
                    item.isWeapon = true;
                    item.itemValue = 5;
                    item.initCount = 1;
                    item.rewardCount = 4;
                    item.stackable = true;
                    item.throwDamage = 1;
                    item.throwDistance = 4;
                    item.throwExtraDist = false;
                    item.dontAutomaticallySelect = true;
                    item.specialDamage = true;
                    item.shadowOffset = 4;
                });
			#endregion
			#region Spear
			Sprite sprite_spear = RogueUtilities.ConvertToSprite(Properties.Resources.Spear);

			CustomItem Spear = RogueLibs.CreateCustomItem("Spear", sprite_spear, false,
				new CustomNameInfo("Spear"),
				new CustomNameInfo("You've probably seen Cavemen hunting your mom with these."),
				item =>
				{
					item.itemType = "WeaponMelee";
                    item.weaponCode = weaponType.WeaponMelee;
                    item.Categories.Add("Weapons");
                    item.isWeapon = true;
                    item.itemValue = 30;
                    item.initCount = 80;
                    item.rewardCount = 200;
                    item.stackable = true;
                    item.meleeDamage = 8;
                    item.hitSoundType = "Cut";
				});
            Spear.UnlockCost = 3;
            Spear.CostInCharacterCreation = 1;
            Spear.CostInLoadout = 1;

            //(Melee)Spear.animClass = "Stab";
            // Patch Melee.Attack() for these variables, see those for Knife.
			#endregion
			#endregion
			#region Names
			CustomName name_BurnedHands = RogueLibs.CreateCustomName("BurnedHands", "Dialogue", new CustomNameInfo("God fucking damn it, I always fucking burn my fucking hands!"));
			#endregion
			#region Patches
			#region Patches - Generic
            //TODO: Switch to using simpler patcher.Prefix method, now that the point of its arguments are clear

			this.PatchPrefix(typeof(ObjectReal), "DestroyMe", GetType(), "ObjectReal_DestroyMe", new Type[1] { typeof(PlayfieldObject) });
            this.PatchPostfix(typeof(ObjectReal), "DetermineButtons", GetType(), "ObjectReal_DetermineButtons");
            this.PatchPrefix(typeof(ObjectReal), "FinishedOperating", GetType(), "ObjectReal_FinishedOperating");
            this.PatchPostfix(typeof(ObjectReal), "Interact", GetType(), "ObjectReal_Interact", new Type[1] { typeof(Agent) });
            this.PatchPrefix(typeof(ObjectReal), "MakeNonFunctional", GetType(), "ObjectReal_MakeNonFunctional", new Type[1] { typeof(PlayfieldObject) });
            this.PatchPostfix(typeof(ObjectReal), "ObjectAction", GetType(), "ObjectReal_ObjectAction", new Type[5] { typeof(string), typeof(string), typeof(float), typeof(Agent), typeof(PlayfieldObject) });
            this.PatchPrefix(typeof(ObjectReal), "ObjectUpdate", GetType(), "ObjectReal_ObjectUpdate");
            this.PatchPrefix(typeof(ObjectReal), "PressedButton", GetType(), "ObjectReal_PressedButton", new Type[2] { typeof(string), typeof(int) });
            this.PatchPostfix(typeof(ObjectReal), "Start", GetType(), "ObjectReal_Start");

            //this.PatchPrefix(typeof(PlayfieldObject), "FindDamage", GetType(), "PlayfieldObject_FindDamage", new Type[] { typeof(PlayfieldObject), typeof(bool), typeof(bool), typeof(bool) }); 
            this.PatchPrefix(typeof(PlayfieldObject), "playerHasUsableItem", GetType(), "PlayfieldObject_PlayerHasUsableItem", new Type[1] { typeof(InvItem) });

            this.PatchPostfix(typeof(StatusEffects), "BecomeHidden", GetType(), "StatusEffects_BecomeHidden", new Type[1] { typeof(ObjectReal) });
            this.PatchPostfix(typeof(StatusEffects), "BecomeNotHidden", GetType(), "StatusEffects_BecomeNotHidden");
			#endregion
			#region Patches - Objects
			this.PatchPostfix(typeof(Bathtub), "SetVars", GetType(), "Bathtub_SetVars");

            this.PatchPostfix(typeof(FlamingBarrel), "SetVars", GetType(), "FlamingBarrel_SetVars");

            this.PatchPostfix(typeof(Plant), "SetVars", GetType(), "Plant_SetVars");

            this.PatchPostfix(typeof(PoolTable), "SetVars", GetType(), "PoolTable_SetVars");

            this.PatchPrefix(typeof(Stove), "DamagedObject", GetType(), "Stove_DamagedObject", new Type[2] { typeof(PlayfieldObject), typeof(float) });
            this.PatchPostfix(typeof(Stove), "RevertAllVars", GetType(), "Stove_RevertAllVars");
            this.PatchPostfix(typeof(Stove), "SetVars", GetType(), "Stove_SetVars");

            this.PatchPostfix(typeof(TableBig), "SetVars", GetType(), "TableBig_SetVars");
			#endregion
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
            List<Stove> removal = new List<Stove>();

            foreach (KeyValuePair<Stove, VariablesStove> pair in Stove_Variables)
                if (pair.Key.isBroken())
                    removal.Add(pair.Key);

            foreach (Stove stove in removal)
            {
                Stove_Variables.Remove(stove);
                ConsoleMessage.LogInfo("Removed pair.Key from VariablesStove");
            }
        }
        #endregion
        #region ObjectReal
        public static bool ObjectReal_DestroyMe(PlayfieldObject damagerObject, ObjectReal __instance)
        {
            ConsoleMessage.LogMessage("ObjectReal_DestroyMe");

            if (__instance is Stove)
                Stove_Variables[(Stove)__instance].savedDamagerObject = damagerObject;
            
            return true;
        }
        public static void ObjectReal_DetermineButtons(ObjectReal __instance)
        {
            //ConsoleMessage.LogMessage("ObjectReal_DetermineButtons");

            if (__instance is FlamingBarrel)
            {
                if (__instance.ora.hasParticleEffect)
                {
                    if (__instance.interactingAgent.inventory.HasItem("Fud"))
                        __instance.buttons.Add("GrillFud");
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
                        __instance.buttonsExtra.Add(" (" + __instance.interactingAgent.inventory.FindItem("Wrench").invItemCount + ") -30");
                        // TODO: It was adding this to Grill Fud. Hopefully reordering it will fix it?
                    }
                    if (__instance.interactingAgent.inventory.HasItem("Fud"))
                        __instance.buttons.Add("GrillFud");
                }
                return;
            }
        }
        public static bool ObjectReal_FinishedOperating(ObjectReal __instance)
        {
            ConsoleMessage.LogMessage("ObjectReal_FinishedOperating");

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
                if (__instance.operatingBarType == "Collecting")
                    __instance.CollectPart();

            __instance.StopInteraction();
            return false;
        }
        public static void ObjectReal_Interact(Agent agent, ObjectReal __instance)
        {
            ConsoleMessage.LogMessage("ObjectReal_Interact");
            //TODO: Try StopInteraction() as a default, with other options as return.

            if (__instance is Bathtub || __instance is Plant || __instance is PoolTable || __instance is TableBig)
            {
                //TODO: Disable objects' "move toward wall" behavior when generating a chunk.
                //TODO: Alternatively, make them non-blocking to movement when a player is inside it.

                agent.statusEffects.BecomeHidden(__instance);

                __instance.StopInteraction();
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
                        __instance.gc.playerAgent.objectMult.ObjectAction(__instance.objectNetID, "MakeNonFunctional");

                    __instance.timer = 10f;
                    __instance.timeCountdownClock = (int)__instance.timer;
                    __instance.InvokeRepeating("Countdown", 0.01f, 1f);
                    __instance.interactable = false;
                    Stove_Variables[(Stove)__instance].savedDamagerObject = damagerObject;
                    Stove_Variables[(Stove)__instance].countdownCauser = Stove_Variables[(Stove)__instance].savedDamagerObject;
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
                    Stove_UseWrenchToDetonate((Stove)__instance);
                
                ___noMoreObjectActions = false;
            }
        }
        public static bool ObjectReal_ObjectUpdate(ObjectReal __instance)
        {
            //ConsoleMessage.LogMessage("ObjectReal_ObjectUpdate"); //Verbose

            if (__instance is Stove)
            {
                if (__instance.timer > 0f)
                {
                    __instance.timer -= Time.deltaTime;

                    if (__instance.timer <= 0f)
                    {
                        if (__instance.startedFlashing)
                        {
                            __instance.DestroyMe(Stove_Variables[(Stove)__instance].savedDamagerObject);
                            return false;
                        }
                        Stove_Variables[(Stove)__instance].noOwnCheckCountdown = true;
                        Stove_Variables[(Stove)__instance].savedDamagerObject = Stove_Variables[(Stove)__instance].countdownCauser;
                        __instance.DestroyMe(Stove_Variables[(Stove)__instance].countdownCauser);
                    }
                }
                return false;
            }
            return true;
        }
        public static bool ObjectReal_PressedButton(string buttonText, int buttonPrice, ObjectReal __instance)
        {
            ConsoleMessage.LogMessage("ObjectReal_PressedButton");

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
            ConsoleMessage.LogMessage("ObjectReal_Start");

            if (__instance is Stove stove)
                Stove_Variables.Add(stove, new VariablesStove());
        }
        #endregion
        #region PlayfieldObject
        public static void PlayfieldObject_FindDamage()//I think this is for Spear & Beer Can
		{

		}
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
                //__instance.agent.agentCollider.gameObject.SetActive(false); Nope, this makes the character invisible but still trapped.
                
                return;
		}
        public static void StatusEffects_BecomeNotHidden(StatusEffects __instance)
		{
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
            __instance.interactingAgent.statusEffects.ChangeHealth((float)-10, __instance);
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
                __instance.DestroyMe(Stove_Variables[__instance].savedDamagerObject);

			yield break;
        }
        public static bool Stove_DamagedObject(PlayfieldObject damagerObject, float damageAmount, Stove __instance)
		{
            ConsoleMessage.LogMessage("Stove_DamagedObject");

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
            //TODO: Ensure Flames for all destruction types.
            //TODO: Consider flame spit instead of flame particle
            return false;
        }
        public static void Stove_GrilledFud(Stove __instance)
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
            Stove_GrilledFudAfter(num, __instance);
        }
        public static void Stove_GrilledFudAfter(int myCount, Stove __instance)
        {
            return;
        }
        public static void Stove_RevertAllVars(Stove __instance)
        {
            Stove_Variables[__instance].mustSpawnExplosionOnClients = false;
            Stove_Variables[__instance].savedDamagerObject = null;
            Stove_Variables[__instance].noOwnCheckCountdown = false;
            Stove_Variables[__instance].countdownCauser = null;
            __instance.objectSprite.transform.Find("RealSprite").transform.localPosition = Vector3.zero;
            __instance.objectSprite.transform.Find("RealSprite").transform.localScale = Vector3.one;
            __instance.CancelInvoke();
        }
        public static void Stove_SetVars(Stove __instance)
        {
            __instance.canExplosiveStimulate = true;
            __instance.dontDestroyImmediateOnClient = true;
            __instance.hasUpdate = true;
            __instance.interactable = true;
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
        public static Dictionary<Stove, VariablesStove> Stove_Variables = new Dictionary<Stove, VariablesStove>();
        #endregion
        #region TableBig
        public static void TableBig_SetVars(TableBig __instance)
        {
            __instance.interactable = true;
        }
		#endregion
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


