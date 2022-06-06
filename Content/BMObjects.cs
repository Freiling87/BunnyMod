using HarmonyLib;
using Light2D;
using RogueLibsCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BunnyMod.Content.Traits;
using UnityEngine;
using Random = UnityEngine.Random;
using BunnyMod.Content.Custom;

namespace BunnyMod.Content
{
	public class BMObjects
	{
		public static GameController GC => GameController.gameController;

		public static bool Prefix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) =>
				BMHeader.MainInstance.PatchPrefix(type, methodName, patchType, patchMethodName, types);
		public static bool Postfix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) =>
				BMHeader.MainInstance.PatchPostfix(type, methodName, patchType, patchMethodName, types);
		public static void BMLog(string logMessage) => BMHeader.Log(logMessage);

		public static MethodInfo ObjectReal_DamagedObject_base = AccessTools.DeclaredMethod(typeof(ObjectReal), "DamagedObject", new Type[2] { typeof(PlayfieldObject), typeof(float) });
		public static MethodInfo ObjectReal_DetermineButtons_base = AccessTools.DeclaredMethod(typeof(ObjectReal), "DetermineButtons", new Type[0] { });
		public static MethodInfo ObjectReal_Interact_base = AccessTools.DeclaredMethod(typeof(ObjectReal), "Interact", new Type[1] { typeof(Agent) });
		public static MethodInfo ObjectReal_ObjectAction_base = AccessTools.DeclaredMethod(typeof(ObjectReal), "ObjectAction", new Type[5] { typeof(string), typeof(string), typeof(float), typeof(Agent), typeof(PlayfieldObject) });
		public static MethodInfo ObjectReal_PressedButton_base = AccessTools.DeclaredMethod(typeof(ObjectReal), "PressedButton", new Type[2] { typeof(string), typeof(int) });
		public static MethodInfo ObjectReal_Start_base = AccessTools.DeclaredMethod(typeof(ObjectReal), "Start", new Type[0] { });
		public static MethodInfo PlayfieldObject_StopInteraction_base = AccessTools.DeclaredMethod(typeof(PlayfieldObject), nameof(PlayfieldObject.StopInteraction), new Type[0] { });
		public static MethodInfo PlayfieldObject_FinishedOperating_base = AccessTools.DeclaredMethod(typeof(PlayfieldObject), "FinishedOperating", new Type[0] { });
		public static MethodInfo PlayfieldObject_Interact_base = AccessTools.DeclaredMethod(typeof(PlayfieldObject), "Interact", new Type[1] { typeof(Agent) });
		public static MethodInfo PlayfieldObject_PressedButton_base = AccessTools.DeclaredMethod(typeof(PlayfieldObject), "PressedButton", new Type[2] { typeof(string), typeof(int) });

		#region Main

		public void Awake()
		{
			Initialize_Names();

			ObjectReal_00();
			PlayfieldObject_00();

			AlarmButton_00();
			ATMMachine_00();
			BathTub_00();
			Crate_00();
			Door_00();
			Elevator_00();
			FireHydrant_00();
			FlamingBarrel_00();
			Generator_00();
			Generator2_00();
			Lamp_00();
			LaserEmitter_00();
			Plant_00();
			PoliceBox_00();
			PoolTable_00();
			Refrigerator_00();
			SecurityCam_00();
			SlotMachine_00();
			Stove_00();
			TableBig_00();
			Television_00();
			TrashCan_00();
			Turret_00();
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
			string t;

			t = vNameType.Dialogue;
			_ = RogueLibs.CreateCustomName(cDialogue.FlamingBarrelCookDamage, t, new CustomNameInfo("God fucking damn it, I always fucking burn my fucking hands!"));
			_ = RogueLibs.CreateCustomName(cDialogue.FlamingBarrelCookNoDamage, t, new CustomNameInfo("Mmmm, toasty. Just like the burning flesh on my fingers!"));
			_ = RogueLibs.CreateCustomName(cDialogue.MachineBusy, t, new CustomNameInfo("It's busy doing... machine things."));
			_ = RogueLibs.CreateCustomName(cDialogue.SlotMachineJackpot_1, t, new CustomNameInfo("Chauvelin Automated Vice, Inc. presents: Jackpot!"));
			_ = RogueLibs.CreateCustomName(cDialogue.SlotMachineJackpot_2, t, new CustomNameInfo("Winner Winner, Chicken Dinner!"));
			_ = RogueLibs.CreateCustomName(cDialogue.SlotMachineJackpot_3, t, new CustomNameInfo("NOTE: You are not actually winning a Chicken Dinner, it's an expression."));
			_ = RogueLibs.CreateCustomName(cDialogue.SlotMachineJackpot_4, t, new CustomNameInfo("Yep... still going."));
			_ = RogueLibs.CreateCustomName(cDialogue.SlotMachineJackpot_5, t, new CustomNameInfo("Jackpot. Happy for ya."));

			t = vNameType.Interface;
			_ = RogueLibs.CreateCustomName(cButtonText.DispenseIce, t, new CustomNameInfo("Dispense ice"));
			_ = RogueLibs.CreateCustomName(cButtonText.HideInContainer, t, new CustomNameInfo("Hide in container"));
			_ = RogueLibs.CreateCustomName(cButtonText.OpenContainer, t, new CustomNameInfo("Open container"));
			_ = RogueLibs.CreateCustomName(cButtonText.SlotMachineHackJackpot, t, new CustomNameInfo("Penny-Slot Jackpot Promotion"));
			_ = RogueLibs.CreateCustomName(cButtonText.SlotMachinePlay1, t, new CustomNameInfo("Play"));
			_ = RogueLibs.CreateCustomName(cButtonText.SlotMachinePlay100, t, new CustomNameInfo("Play"));
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
					newLabel = buttonLabel.Replace("-30", "-" + BMTraitController.ApplyToolCostModifiers(objectReal.interactingAgent, 30));
					flag = true;
				}
				else if (buttonLabel.EndsWith("-20"))
				{
					newLabel = buttonLabel.Replace("-20", "-" + BMTraitController.ApplyToolCostModifiers(objectReal.interactingAgent, 20));
					flag = true;
				}
				else if (buttonLabel == " - 7HP")
				{
					newLabel = buttonLabel.Replace(" - 7HP", " - " + BMTraitController.HealthCost(objectReal.interactingAgent, 7, DamageType.brokenWindow));
					flag = true;
				}
				else if (buttonLabel == " - 15HP")
				{
					newLabel = buttonLabel.Replace(" - 15HP", " - " + BMTraitController.HealthCost(objectReal.interactingAgent, 15, DamageType.brokenWindow));
					flag = true;
				}
				else if (buttonLabel.EndsWith("(Burn hands for 10 damage)"))
				{
					newLabel = buttonLabel.Replace("(Burn hands for 10 damage)",
							"(Burn hands for " + BMTraitController.HealthCost(objectReal.interactingAgent, 10, DamageType.burnedFingers) + " damage)");
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

		#region Base Classes & Spawners

		#region ObjectReal

		public void ObjectReal_00()
		{
			Prefix(typeof(ObjectReal), "FinishedOperating", GetType(), "ObjectReal_FinishedOperating", new Type[0]);
			Prefix(typeof(ObjectReal), "Interact", GetType(), "ObjectReal_Interact", new Type[1] { typeof(Agent) });
			Prefix(typeof(ObjectReal), "MakeNonFunctional", GetType(), "ObjectReal_MakeNonFunctional", new Type[1] { typeof(PlayfieldObject) });
			Postfix(typeof(ObjectReal), "ObjectAction", GetType(), "ObjectReal_ObjectAction",
					new Type[5] { typeof(string), typeof(string), typeof(float), typeof(Agent), typeof(PlayfieldObject) });
			Prefix(typeof(ObjectReal), "ObjectUpdate", GetType(), "ObjectReal_ObjectUpdate", new Type[0]);
			Prefix(typeof(ObjectReal), "PressedButton", GetType(), "ObjectReal_PressedButton", new Type[2] { typeof(string), typeof(int) });
			Postfix(typeof(ObjectReal), "Start", GetType(), "ObjectReal_Start", new Type[0]);
		}

		public static bool ObjectReal_FinishedOperating(ObjectReal __instance) // Replacement
		{
			PlayfieldObject_FinishedOperating_base.GetMethodWithoutOverrides<Action>(__instance).Invoke();

			string opItem = "";

			if (!(__instance.operatingItem is null))
				opItem = __instance.operatingItem.invItemName;

			if (__instance is FlamingBarrel)
				FlamingBarrel_GrilledFud((FlamingBarrel) __instance);
			else if (__instance is Fountain)
				Fountain_Steal((Fountain) __instance);
			else if (__instance is Stove)
			{
				if (opItem == vItem.Wrench)
					Stove_UseWrenchToDetonate((Stove) __instance);
				else if (opItem == vItem.Fud)
					Stove_GrilledFud((Stove) __instance);
			}
			else if (__instance is VendorCart)
				VendorCart_Steal((VendorCart) __instance);
			else if (!__instance.interactingAgent.interactionHelper.interactingFar && __instance.operatingBarType == "Collecting")
				__instance.CollectPart();
			else
				return false;

			__instance.StopInteraction();

			return false;
		}

		public static bool ObjectReal_Interact(Agent agent, ObjectReal __instance) // Replacement
		{
			BMHeader.ConsoleMessage.LogMessage("ObjectReal_Interact: " + __instance.name);

			PlayfieldObject_Interact_base.GetMethodWithoutOverrides<Action<Agent>>(__instance).Invoke(agent);

			if (__instance is Bathtub || __instance is Plant || __instance is PoolTable || __instance is TableBig)
			{
				if (agent.HasTrait<StealthBastardDeluxe>())
				{
					agent.SetInvisible(false);
					agent.statusEffects.BecomeHidden(__instance);
				}

				__instance.StopInteraction();
			}
			else if (__instance is FlamingBarrel)
				__instance.ShowObjectButtons();
			else if (__instance is Stove)
			{
				if (__instance.timer > 0f || __instance.startedFlashing)
					__instance.StopInteraction();

				__instance.ShowObjectButtons();
			}
			else if (__instance is VendorCart && VendorCartStolenFrom[__instance] == false)
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
					__instance.timeCountdownClock = (int) __instance.timer;
					BMHeaderTools.InvokeRepeating(Stove_Variables[(Stove) __instance], "Countdown", 0.01f, 1f);

					__instance.interactable = false;
					Stove_Variables[(Stove) __instance].savedDamagerObject = damagerObject;
					Stove_Variables[(Stove) __instance].countdownCauser = Stove_Variables[(Stove) __instance].savedDamagerObject;
				}
				return false;
			}
			return true;
		}

		public static void ObjectReal_ObjectAction(string myAction, string extraString, float extraFloat, Agent causerAgent, PlayfieldObject extraObject,
				ObjectReal __instance, ref bool ___noMoreObjectActions) // Postfix
		{
			if (__instance is Stove)
			{
				if (!___noMoreObjectActions && myAction == "UseWrenchToDetonate")
					Stove_UseWrenchToDetonate((Stove) __instance);

				___noMoreObjectActions = false;
			}
		}

		public static bool ObjectReal_ObjectUpdate(ObjectReal __instance) // Prefix
		{
			if (__instance is Stove)
			{
				Stove_Remora remora = Stove_Variables[(Stove) __instance];

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

			PlayfieldObject_PressedButton_base.GetMethodWithoutOverrides<Action<string, int>>(__instance).Invoke(buttonText, buttonPrice);

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

			else if (buttonText == "GrillFud")
				__instance.StartCoroutine(__instance.Operating(agent, agent.inventory.FindItem(vItem.Fud), 2f, true, "Grilling"));
			else if (buttonText == cButtonText.GrillFudPaid && __instance.moneySuccess(5))
				__instance.StartCoroutine(__instance.Operating(agent, agent.inventory.FindItem(vItem.Fud), 2f, true, "Grilling"));
			else if (buttonText == "HackExplode")
				__instance.HackExplode(agent);
			else if (buttonText == cButtonText.HideInContainer)
				TrashCan_Hide((TrashCan) __instance, agent);
			else if (buttonText == "LightBarbecue")
			{
				__instance.StartFireInObject();
				__instance.StopInteraction();
			}
			else if (buttonText == cButtonText.OpenContainer)
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
			else if (buttonText == cButtonText.StealItem)
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
				Stove_Variables[(Stove) __instance].animateSpriteID = __instance.spr.GetSpriteIdByName("Stove");
				Stove_Variables[(Stove) __instance].animateSpriteID2 = __instance.spr.GetSpriteIdByName("Stove");
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
			Postfix(typeof(PlayfieldObject), "DetermineLuck", GetType(), "PlayfieldObject_DetermineLuck",
					new Type[3] { typeof(int), typeof(string), typeof(bool) });
			Prefix(typeof(PlayfieldObject), "Operating", GetType(), "PlayfieldObject_Operating",
					new Type[5] { typeof(Agent), typeof(InvItem), typeof(float), typeof(bool), typeof(string) });
			Prefix(typeof(PlayfieldObject), "playerHasUsableItem", GetType(), "PlayfieldObject_PlayerHasUsableItem", new Type[1] { typeof(InvItem) });
		}

		/// <summary>
		/// Removed RATS bonuses out of this patch into a transpiler
		/// Verify that both postfixes play nicely together.
		/// </summary>
		/// <param name="originalLuck"></param>
		/// <param name="luckType"></param>
		/// <param name="cancelStatusEffects"></param>
		/// <param name="__instance"></param>
		/// <param name="__result"></param>
		public static void PlayfieldObject_DetermineLuck(int originalLuck, string luckType, bool cancelStatusEffects, PlayfieldObject __instance,
				ref int __result) // Postfix
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
			else if (luckType == vTrait.UnCrits)
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
			else if (luckType == vTrait.Butterfingerer)
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

			// RATS was removed from here, to Roguewarts mod.

			if (agent.HasTrait<Charmed>())
				luckMultiplier += 1;
			else if (agent.HasTrait<Charmed2>())
				luckMultiplier += 2;
			else if (agent.HasTrait<Cursed>())
				luckMultiplier += -1;
			else if (agent.HasTrait<Cursed2>())
				luckMultiplier += -2;

			__result = Mathf.Clamp(__result + (luckBonus * luckMultiplier), 0, 100);
		}

		public static bool PlayfieldObject_Operating(Agent myAgent, InvItem item, float timeToUnlock, bool makeNoise, string barType,
				PlayfieldObject __instance) // Prefix
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
				Stove stove = (Stove) __instance;
				return (myItem.invItemName == "Wrench" || myItem.invItemName == "Fud")
						&& __instance.timer == 0f
						&& !stove.startedFlashing;
			}
			else
				return false;
		}

		#endregion

		#region Objects

		#region Alarm Button

		#region ATMMachine

		public void ATMMachine_00()
		{
			Type t = typeof(ATMMachine);
			Type g = GetType();

			Prefix(t, "DetermineButtons", g, "ATMMachine_DetermineButtons", new Type[0] { });
		}

		public static bool ATMMachine_DetermineButtons(ATMMachine __instance) // Replacement
		{
			// Priors enabled to pay off cops

			ObjectReal_DetermineButtons_base.GetMethodWithoutOverrides<Action>(__instance).Invoke();

			if (__instance.interactingAgent.interactionHelper.interactingFar)
			{
				if (!__instance.didSpitOutMoney)
					__instance.buttons.Add("SpitOutMoney");

				if ((__instance.interactingAgent.oma.superSpecialAbility && __instance.interactingAgent.agentName == "Hacker") ||
						__instance.interactingAgent.statusEffects.hasTrait("HacksBlowUpObjects"))
				{
					__instance.buttons.Add("HackExplode");

					return false;
				}
			}
			else
			{
				bool flag = false;

				if (__instance.readyForDelivery &&
						(__instance.interactingAgent.inventory.HasItem("CourierPackage")
								|| __instance.interactingAgent.inventory.HasItem("CourierPackageBroken")))
				{
					__instance.buttons.Add("DeliverPackage");
					__instance.buttonPrices.Add(0);
					__instance.buttonsExtra.Add("");
				}

				if (!__instance.gc.challenges.Contains("Sandbox") &&
						!__instance.gc.challenges.Contains("SpeedRun") &&
						!__instance.gc.challenges.Contains("SpeedRun2") &&
						!__instance.gc.customCampaign && !__instance.gc.wasLevelEditing)
				{
					__instance.buttons.Add("StoreItem");
					__instance.buttonPrices.Add(0);
					__instance.buttonsExtra.Add("");
					flag = true;
				}

				if (!__instance.specialInvDatabase.isEmpty() &&
						!__instance.gc.challenges.Contains("SpeedRun") &&
						!__instance.gc.challenges.Contains("SpeedRun2") &&
						!__instance.gc.customCampaign &&
						!__instance.gc.wasLevelEditing)
				{
					__instance.buttons.Add("RetrieveStoredItem");
					__instance.buttonPrices.Add(0);
					__instance.buttonsExtra.Add("");
					__instance.buttons.Add("DestroyItem");
					__instance.buttonPrices.Add(0);
					__instance.buttonsExtra.Add("");
					flag = true;
				}

				if (__instance.interactingAgent.statusEffects.hasStatusEffect("InDebt") ||
						__instance.interactingAgent.statusEffects.hasStatusEffect("InDebt2") ||
						__instance.interactingAgent.statusEffects.hasStatusEffect("InDebt3"))
				{
					__instance.buttons.Add("PayBackDebt");
					__instance.buttonPrices.Add(__instance.interactingAgent.CalculateDebt());
					__instance.buttonsExtra.Add("");
					flag = true;
				}

				if (__instance.interactingAgent.statusEffects.hasStatusEffect("OweCops1") ||
						__instance.interactingAgent.statusEffects.hasStatusEffect("OweCops2") ||
						__instance.interactingAgent.HasTrait<Priors>())
				{
					__instance.buttons.Add("PayCops");

					if (__instance.interactingAgent.statusEffects.hasStatusEffect("OweCops1"))
						__instance.buttonPrices.Add(__instance.determineMoneyCost("PayCops1"));
					else if (__instance.interactingAgent.statusEffects.hasStatusEffect("OweCops2") ||
							__instance.interactingAgent.HasTrait<Priors>())
						__instance.buttonPrices.Add(__instance.determineMoneyCost("PayCops2"));

					__instance.buttonsExtra.Add("");
					flag = true;
				}

				if (__instance.interactingAgent.bigQuest == "Hobo" && !__instance.gc.loadLevel.LevelContainsMayor())
				{
					__instance.buttons.Add("PutMoneyTowardHome");
					__instance.buttonPrices.Add(__instance.determineMoneyCost("PutMoneyTowardHome"));
					__instance.buttonsExtra.Add("");
					flag = true;
				}

				__instance.ShowCollectAlienPartButton();

				if (__instance.CanCollectAlienPart())
					flag = true;

				if (!flag)
					BMHeaderTools.SayDialogue(__instance.interactingAgent, "NoReasonToUse", vNameType.Dialogue);
			}

			return false;
		}

		public static void ATMMachine_PayCops(ATMMachine __instance) // Postfix
		{
			if (__instance.interactingAgent.HasTrait<Priors>())
			{
				if (!__instance.moneySuccess(__instance.determineMoneyCost("PayCops2")))
				{
					__instance.StopInteraction();

					return;
				}

				__instance.DidPayCops(__instance.interactingAgent);
			}
		}

		#endregion

		#region Bathtub

		public void BathTub_00()
		{
			Postfix(typeof(Bathtub), "SetVars", GetType(), "Bathtub_SetVars", new Type[0] { });
		}

		public static void Bathtub_SetVars(Bathtub __instance) // Postfix
		{
			__instance.interactable = true;

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
			Type t = typeof(Door);
			Type g = GetType();

			Prefix(t, "CloseDoor", g, "Door_CloseDoor", new Type[2] { typeof(Agent), typeof(bool) });
			Postfix(t, "DetermineButtons", g, "Door_DetermineButtons", new Type[0] { });
			Postfix(t, "FreePrisonerPointsIfNotDead", g, "Door_FreePrisonerPointsIfNotDead", new Type[2] { typeof(Agent), typeof(List<Agent>) });
			Prefix(t, "OpenDoor", g, "Door_OpenDoor", new Type[2] { typeof(Agent), typeof(bool) });
			Prefix(t, "Unlock", g, "Door_Unlock", new Type[0] { });
		}

		public static bool Door_CloseDoor(Agent myAgent, bool remote, Door __instance) // Replacement
		{
			if (GC.serverPlayer || GC.clientControlling || (__instance.immediateInteractions &&
					(__instance.interactingAgent == GC.playerAgent || (myAgent != null && myAgent.oma.mindControlled))))
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

					if (myAgent != null && myAgent.isPlayer > 0 && GC.serverPlayer && !__instance.outsideDoor &&
							!myAgent.HasTrait<StealthBastardDeluxe>())
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
			BMLog("Door_OpenDoor");

			if (GC.serverPlayer || GC.clientControlling || (__instance.immediateInteractions &&
					(__instance.interactingAgent == GC.playerAgent || myAgent != null && myAgent.oma.mindControlled)))
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

					BMLog("\tCheckpoint 1");

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

					if (GC.serverPlayer && myAgent != null && !myAgent.HasTrait<StealthBastardDeluxe>())
						GC.spawnerMain.SpawnNoise(__instance.tr.position, 0.2f, null, null, myAgent);

					if (myAgent != null && !remote && !__instance.boughtKeyInChunk && !myAgent.HasTrait<StealthBastardDeluxe>())
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
						if (__instance.interactingAgent.inventory.HasItem("Key") && !___usedTutorialKey && (GC.tutorial.tutorialPart == "UseKeyOnDoor" ||
								GC.tutorial.tutorialPart == "LoseHealth" || GC.tutorial.tutorialPart == "TranquilizeTarget"))
						{
							__instance.DestroyKeyItem("Key");
							GC.tutorial.UseKeyOnDoor();
							___usedTutorialKey = true;
						}
					}
					else if (__instance.wasLocked && !__instance.locked &&
							((__instance.prisonDoor > 0 && __instance.prisonDoor > 0) || __instance.startingChunkRealDescription == "Hotel"))
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
			BMLog("Door_OpenDoorAnim");

			__instance.objectHitbox.GetComponent<MeshRenderer>().enabled = false;

			yield return new WaitForSeconds(0.02f);

			yield break;
		}

		public static bool Door_Unlock(Door __instance, bool ___destroyingDoor) // Replacement
		{
			// Stealth Bastard Deluxe silent door opening

			if (GC.serverPlayer || GC.clientControlling)
			{
				__instance.locked = false;

				if (__instance.prisonDoor > 0 && !__instance.open)
				{
					__instance.graphUpdateScene.setTag = 2;
					__instance.graphUpdateScene.Apply();
				}

				if (__instance.interactingAgent != null && !__instance.boughtKeyInChunk &&
						!__instance.interactingAgent.HasTrait<StealthBastardDeluxe>())
					__instance.StartCoroutine(__instance.delayedOwnCheck(__instance.interactingAgent));

				if (!___destroyingDoor)
				{
					if (__instance.direction == "N" || __instance.direction == "S")
					{
						if (__instance.doorType == "Hard")
							__instance.SetObjectSprite("DoorHard");
						else
							__instance.SetObjectSprite("Door");
					}
					else if (__instance.direction == "E" || __instance.direction == "W")
					{
						if (__instance.doorType == "Hard")
							__instance.SetObjectSprite("DoorHardNS");
						else
							__instance.SetObjectSprite("DoorNS");
					}

					if (__instance.doorType == "Hard")
						__instance.ChangeObjectRealName(GC.nameDB.GetName(__instance.objectName, "Object"), GC.nameDB.GetName("DoorHard", "Object"));
					else
						__instance.ChangeObjectRealName(GC.nameDB.GetName(__instance.objectName, "Object"));
				}

				GC.playerAgent.SetCheckUseWithItemsAgain(__instance);
				GC.playerAgent.objectMult.ObjectAction(__instance.objectNetID, "AllowImmediateInteractions");

				if (GC.serverPlayer && GC.multiplayerMode)
				{
					GC.playerAgent.objectMult.ObjectAction(__instance.objectNetID, "UnlockFromHost");

					return false;
				}
			}
			else if (!___destroyingDoor)
			{
				if (__instance.interactingAgent != null)
				{
					__instance.interactingAgent.objectMult.ObjectAction(__instance.objectNetID, "Unlock");

					return false;
				}

				GC.playerAgent.objectMult.ObjectAction(__instance.objectNetID, "Unlock");
			}

			return false;
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

			if (agent.statusEffects.hasTrait(vTrait.ResistFire) || agent.statusEffects.hasTrait(vTrait.FireproofSkin) ||
					agent.statusEffects.hasTrait(vTrait.FireproofSkin2))
				BMHeaderTools.SayDialogue(agent, cDialogue.FlamingBarrelCookNoDamage, vNameType.Dialogue);
			else
			{
				agent.statusEffects.ChangeHealth(-10f, __instance);
				BMHeaderTools.SayDialogue(agent, cDialogue.FlamingBarrelCookDamage, vNameType.Dialogue);
			}

			return;
		}

		public static void FlamingBarrel_SetVars(FlamingBarrel __instance) // Postfix
		{
			__instance.interactable = true;
			__instance.fireDoesntDamage = true;
		}

		#endregion

		#region Fountain



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

		#region Plant

		public void Plant_00()
		{
			Postfix(typeof(Plant), "SetVars", GetType(), "Plant_SetVars", new Type[0] { });
		}

		public static void Plant_SetVars(Plant __instance) // Postfix
		{
			__instance.interactable = true;
			//__instance.lowInteractionPriority = true;
		}

		#endregion

		#region PoliceBox

		public void PoliceBox_00()
		{
			Type t = typeof(PoliceBox);
			Type g = GetType();

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
			__instance.interactable = true;
			//__instance.lowInteractionPriority = true;
		}

		#endregion

		#region Refrigerator

		public void Refrigerator_00()
		{
			Type t = typeof(Refrigerator);
			Type g = GetType();
		}

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

		#region SlotMachine

		public void SlotMachine_00()
		{
			Type t = typeof(SlotMachine);
			Type g = GetType();

			Prefix(t, "DetermineButtons", g, "SlotMachine_DetermineButtons", new Type[0] { });
			Prefix(t, "Gamble", g, "SlotMachine_Gamble", new Type[1] { typeof(int) });
			Prefix(t, "Interact", g, "SlotMachine_Interact", new Type[1] { typeof(Agent) });
			Prefix(t, "InteractFar", g, "SlotMachine_InteractFar", new Type[1] { typeof(Agent) });
			Prefix(t, "PressedButton", g, "SlotMachine_PressedButton", new Type[2] { typeof(string), typeof(int) });
			Postfix(t, "SetVars", g, "SlotMachine_SetVars", new Type[0] { });
		}

		public static Dictionary<PlayfieldObject, bool> SlotMachineHacked = new Dictionary<PlayfieldObject, bool>();
		public static Dictionary<PlayfieldObject, bool> SlotMachinePlayingSound = new Dictionary<PlayfieldObject, bool>();

		public static bool SlotMachine_DetermineButtons(SlotMachine __instance) // Replacement
		{
			ObjectReal_DetermineButtons_base.GetMethodWithoutOverrides<Action>(__instance).Invoke();

			if (__instance.interactingAgent.interactionHelper.interactingFar)
			{
				if (__instance.advantage == 0)
					__instance.buttons.Add("IncreaseSlotMachineOdds");

				__instance.buttons.Add(cButtonText.SlotMachineHackJackpot);

				if ((__instance.interactingAgent.oma.superSpecialAbility && __instance.interactingAgent.agentName == "Hacker") ||
						__instance.interactingAgent.statusEffects.hasTrait("HacksBlowUpObjects"))
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

		public static bool SlotMachine_Gamble(int gambleAmt, SlotMachine __instance) // Replacement
		{
			if (!__instance.moneySuccess(gambleAmt))
			{
				__instance.StopInteraction();

				return false;
			}

			__instance.IncreaseNumPlays();
			__instance.objectInvDatabase.money.invItemCount += gambleAmt;
			int advantage = __instance.interactingAgent.DetermineLuck(__instance.advantage, "SlotMachine", true);
			int payoutMult;
			int result = 1000 - Mathf.Clamp(Random.Range(1, 1000) + advantage, 1, 1000);

			BMLog("\tresult: " + result);

			if (result == 0)
				payoutMult = 64;
			else if (result <= 8)
				payoutMult = 16;
			else if (result <= 32)
				payoutMult = 8;
			else if (result <= 64)
				payoutMult = 4;
			else if (result <= 128)
				payoutMult = 2;
			else
				payoutMult = 0;

			BMLog("\tpayoutMult: " + payoutMult);

			int payout = gambleAmt * payoutMult;

			BMLog("\tpayout: " + payout);

			if (payoutMult == 64 || __instance.interactingAgent.HasTrait<Debug>())
				SlotMachine_Jackpot(payout, __instance);
			else if (payoutMult > 0)
			{
				__instance.interactingAgent.inventory.AddItem("Money", payout);
				__instance.objectInvDatabase.SubtractFromItemCount(__instance.objectInvDatabase.money, payout);
				BMHeaderTools.SayDialogue(__instance.interactingAgent, "SlotMachineWon", vNameType.Dialogue);
				__instance.PlayAnim("MachineOperate", __instance.interactingAgent);

				while (payoutMult > 0)
				{
					GC.audioHandler.Play(__instance, "Win");
					payoutMult--;
				}
			}
			else
			{
				BMHeaderTools.SayDialogue(__instance.interactingAgent, "SlotMachineLost", vNameType.Dialogue);
				GC.audioHandler.Play(__instance, "Fail");
			}

			__instance.StopInteraction();

			return false;
		}

		public static bool SlotMachine_IncreaseSlotMachineOdds(Agent causerAgent, SlotMachine __instance) // Replacement
		{
			if (GC.serverPlayer)
			{
				__instance.advantage = 50;

				if ((!causerAgent.oma.superSpecialAbility || !(causerAgent.agentName == "Hacker")) && !causerAgent.statusEffects.hasTrait("HacksBlowUpObjects"))
				{
					__instance.hackable = false;

					return false;
				}
			}
			else
				__instance.interactingAgent.objectMult.ObjectAction(__instance.objectNetID, "IncreaseSlotMachineOdds");

			return false;
		}

		public static bool SlotMachine_Interact(Agent agent, SlotMachine __instance) // Prefix
		{
			if (SlotMachinePlayingSound[__instance])
			{
				BMHeaderTools.SayDialogue(agent, cDialogue.MachineBusy, vNameType.Dialogue);
				GC.audioHandler.Play(__instance, vAudioClip.CantDo);

				return false;
			}

			return true;
		}

		public static bool SlotMachine_InteractFar(Agent agent, SlotMachine __instance) // Prefix
		{
			if (SlotMachinePlayingSound[__instance])
			{
				BMHeaderTools.SayDialogue(agent, cDialogue.MachineBusy, vNameType.Dialogue);
				GC.audioHandler.Play(__instance, vAudioClip.CantDo);

				return false;
			}

			return true;
		}

		public static async void SlotMachine_Jackpot(int payout, SlotMachine __instance) // Non-Patch
		{
			float volume = SlotMachineHacked[__instance] ? 5f : 2f;
			int cashDropIncrement = payout / 50;
			__instance.StartCoroutine(SlotMachine_JackpotPlayTime(__instance));
			__instance.StartCoroutine(SlotMachine_PlayingNoise(__instance));
			__instance.interactable = false;

			for (int i = 0; i <= 100; i++)
			{
				if (i % 2 == 0 && i != 0)
				{
					__instance.PlayAnim(vAnimation.MachineOperate, __instance.interactingAgent);
					SlotMachine_SpitOutMoney(cashDropIncrement, __instance);
					GC.audioHandler.Play(__instance, vAudioClip.Win);
				}

				if (i % 10 == 0) // Starts at 0
				{
					GC.audioHandler.Play(__instance, vAudioClip.Boombox1);
					GC.spawnerMain.SpawnStateIndicator(__instance, "MusicNotes");
					GC.spawnerMain.SpawnNoise(__instance.tr.position, volume, __instance, "Attract");
				}

				if (i % 20 == 0 && i != 0)
						// BMHeaderTools.SayDialogue(__instance, cDialogue.SlotMachineJackpot_ + (i / 20).ToString(), vNameType.Dialogue); // Testing
					BMHeaderTools.SayDialogue(__instance, cDialogue.SlotMachineJackpot_1, vNameType.Dialogue);

				await Task.Delay(100);
			}

			if (!GC.challenges.Contains(vChallenge.NoLimits))
			{
				__instance.MakeAllMachinesNonFunctional();
				// TODO: Have proprietor come and cut you off from playing here, but give you a free cocktail.
			}
		}

		public static IEnumerator SlotMachine_JackpotPlayTime(SlotMachine __instance) // Non-Patch
		{
			yield return new WaitForSeconds(10f);

			if (__instance.functional && !__instance.destroyed)
			{
				if (__instance.stateIndicator != null)
					__instance.stateIndicator.StateIndicatorOff();

				GC.audioHandler.StopObjectSounds(__instance);

				if (GC.serverPlayer)
					GC.playerAgent.objectMult.ObjectAction(__instance.objectNetID, "StopMusicClient");
			}

			SlotMachinePlayingSound[__instance] = false;

			yield break;
		}

		public static IEnumerator SlotMachine_PlayingNoise(SlotMachine __instance) // Non-Patch
		{
			float jukeboxTimer = 1f;

			while (SlotMachinePlayingSound[__instance])
			{
				jukeboxTimer -= Time.deltaTime;

				if (jukeboxTimer <= 0f)
				{
					if (SlotMachineHacked[__instance])
						GC.spawnerMain.SpawnNoise(__instance.tr.position, 3f, __instance, "Attract", __instance.interactingAgent, false).distraction = true;

					jukeboxTimer = 1f;
				}

				yield return null;
			}

			yield break;
		}

		public static bool SlotMachine_PressedButton(string buttonText, int buttonPrice, SlotMachine __instance) // Replacement
		{
			ObjectReal_PressedButton_base.GetMethodWithoutOverrides<Action<string, int>>(__instance).Invoke(buttonText, buttonPrice);

			if (buttonText == cButtonText.SlotMachinePlay1)
				__instance.Gamble(1);
			else if (buttonText == "Play5")
				__instance.Gamble(5);
			else if (buttonText == "Play20")
				__instance.Gamble(20);
			else if (buttonText == "Play50")
				__instance.Gamble(50);
			else if (buttonText == cButtonText.SlotMachinePlay100)
				__instance.Gamble(100);
			else if (buttonText == "IncreaseSlotMachineOdds")
			{
				GC.audioHandler.Play(__instance.interactingAgent, "Success");
				__instance.IncreaseSlotMachineOdds(__instance.interactingAgent);
				__instance.StopInteraction();
			}
			else if (buttonText == cButtonText.SlotMachineHackJackpot)
			{
				SlotMachineHacked[__instance] = true;
				SlotMachine_Jackpot(10, __instance);
			}
			else
				__instance.StopInteraction();

			return false;
		}

		public static void SlotMachine_SetVars(SlotMachine __instance) // Postfix
		{
			BMHeaderTools.AddDictionary(SlotMachinePlayingSound, __instance, false);
			BMHeaderTools.AddDictionary(SlotMachineHacked, __instance, false);
		}

		public static void SlotMachine_SpitOutMoney(int amount, SlotMachine __instance) // Non-Patch
		{
			if (GC.serverPlayer)
			{
				InvItem invItem = new InvItem();
				invItem.invItemName = vItem.Money;
				invItem.ItemSetup(false);

				invItem.invItemCount = amount;
				Vector3 position = __instance.tr.position;

				float randA = Random.Range(-0.16f, 0.16f);
				float randB = Random.Range(0.16f, 0.32f);

				switch (__instance.direction)
				{
					case "N":
						position = new Vector3(__instance.tr.position.x + randA, __instance.tr.position.y + randB, __instance.tr.position.z);

						break;
					case "E":
						position = new Vector3(__instance.tr.position.x + randB, __instance.tr.position.y + randA, __instance.tr.position.z);

						break;

					case "S":
						position = new Vector3(__instance.tr.position.x + randA, __instance.tr.position.y - randB, __instance.tr.position.z);

						break;
					case "W":
						position = new Vector3(__instance.tr.position.x - randB, __instance.tr.position.y + randA, __instance.tr.position.z);

						break;
				}

				GC.spawnerMain.SpillItem(position, invItem);

				return;
			}

			__instance.interactingAgent.objectMult.ObjectAction(__instance.objectNetID, "SpitOutMoney");
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

			ObjectReal_DamagedObject_base.GetMethodWithoutOverrides<Action<PlayfieldObject, float>>(__instance).Invoke(damagerObject, damageAmount);

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
				GC.spawnerMain.SpawnExplosion(Stove_Variables[(Stove) __instance].savedDamagerObject, __instance.tr.position, "FireBomb", false, -1, false,
						__instance.FindMustSpawnExplosionOnClients(Stove_Variables[(Stove) __instance].savedDamagerObject));
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
		{ }

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

		public static bool TrashCan_Interact(Agent agent, TrashCan __instance) // Prefix
		{
			if (agent.HasTrait<StealthBastardDeluxe>() && agent.statusEffects.hasTrait(vTrait.Diminutive))
			{
				ObjectReal_Interact_base.GetMethodWithoutOverrides<Action<Agent>>(__instance).Invoke(agent);

				__instance.ShowObjectButtons();

				return false;
			}

			return true;
		}

		#endregion

		#region VendorCart

		public void VendorCart_00()
		{
			Type t = typeof(VendorCart);
			Type g = GetType();

			Postfix(typeof(VendorCart), "SetVars", GetType(), "VendorCart_SetVars", new Type[0] { });
		}

		public static Dictionary<PlayfieldObject, bool> VendorCartStolenFrom = new Dictionary<PlayfieldObject, bool>();

		public static void VendorCart_SetVars(VendorCart __instance) // Postfix
		{
			__instance.interactable = true;
			BMHeaderTools.AddDictionary(VendorCartStolenFrom, __instance, false);
		}

		public static void VendorCart_Steal(VendorCart __instance) // Non-Patch
		{
			InvItem invItem = __instance.objectInvDatabase.InvItemList[0];

			invItem.ItemSetup(true);
			invItem.invItemCount = GC.Choose<int>(1, 1, 2, 2, 2, 2, 3, 3);
			invItem.ShowPickingUpText(__instance.interactingAgent);
			__instance.interactingAgent.inventory.AddItem(invItem);
			__instance.objectInvDatabase.DestroyAllItems();
			VendorCartStolenFrom[__instance] = true;
			__instance.interactable = false;
			__instance.StopInteraction();
		}

		#endregion

		#region Window

		public void Window_00()
		{
			Type t = typeof(Window);
			Type g = GetType();

			Postfix(t, "DetermineButtons", g, "Window_DetermineButtons", new Type[0] { });
			Prefix(t, "SlipThroughWindow", g, "Window_SlipThroughWindow", new Type[1] { typeof(Agent) });
			Postfix(t, "SetVars", g, "Window_SetVars", new Type[0] { });
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
				BMHeaderTools.SayDialogue(__instance.interactingAgent, "CantFit", vNameType.Dialogue);
				GC.audioHandler.Play(myAgent, "CantDo");

				__instance.StopInteraction();

				return false;
			}

			if (!myAgent.HasTrait<StealthBastardDeluxe>())
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

			if (myAgent.HasTrait<StealthBastardDeluxe>())
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

		public static void Window_SetVars(Window __instance) // Postfix
		{
			__instance.breakForAthleteQuest = true;
		}
		#endregion
	}

	#region Remorae

	public class Elevator_Remora
	{
		public static GameController GC => GameController.gameController;

		public Elevator elevatorHost;

		public bool ticketPurchased = false;
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