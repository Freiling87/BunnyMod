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

		public static MethodInfo ObjectReal_DamagedObject_base =
				AccessTools.DeclaredMethod(typeof(ObjectReal), "DamagedObject", new Type[2] { typeof(PlayfieldObject), typeof(float) });

		public static MethodInfo ObjectReal_DetermineButtons_base = AccessTools.DeclaredMethod(typeof(ObjectReal), "DetermineButtons", new Type[0] { });
		public static MethodInfo ObjectReal_Interact_base = AccessTools.DeclaredMethod(typeof(ObjectReal), "Interact", new Type[1] { typeof(Agent) });

		public static MethodInfo ObjectReal_ObjectAction_base = AccessTools.DeclaredMethod(typeof(ObjectReal), "ObjectAction",
				new Type[5] { typeof(string), typeof(string), typeof(float), typeof(Agent), typeof(PlayfieldObject) });

		public static MethodInfo ObjectReal_PressedButton_base =
				AccessTools.DeclaredMethod(typeof(ObjectReal), "PressedButton", new Type[2] { typeof(string), typeof(int) });

		public static MethodInfo ObjectReal_Start_base = AccessTools.DeclaredMethod(typeof(ObjectReal), "Start", new Type[0] { });

		public static MethodInfo PlayfieldObject_StopInteraction_base =
				AccessTools.DeclaredMethod(typeof(PlayfieldObject), nameof(PlayfieldObject.StopInteraction), new Type[0] { });

		public static MethodInfo PlayfieldObject_FinishedOperating_base =
				AccessTools.DeclaredMethod(typeof(PlayfieldObject), "FinishedOperating", new Type[0] { });

		public static MethodInfo PlayfieldObject_Interact_base = AccessTools.DeclaredMethod(typeof(PlayfieldObject), "Interact", new Type[1] { typeof(Agent) });

		public static MethodInfo PlayfieldObject_PressedButton_base =
				AccessTools.DeclaredMethod(typeof(PlayfieldObject), "PressedButton", new Type[2] { typeof(string), typeof(int) });

		#region Main

		public void Awake()
		{
			Initialize_Names();

			PlayfieldObject_00();
			SpawnerMain_00();

			Crate_00();
			Door_00();
			Generator_00();
			Generator2_00();
			LaserEmitter_00();
			Plant_00();
			PoliceBox_00();
			SecurityCam_00();
			SlotMachine_00();
			TableBig_00();
			Television_00();
			Toilet_00();
			TrashCan_00();
			Turret_00();
			VendorCart_00();
			Window_00();
		}

		public static void Initialize_Names()
		{
			string t;

			t = vNameType.Dialogue;
			_ = RogueLibs.CreateCustomName(cDialogue.CantAffordAlarmButton, t, new CustomNameInfo("I can't afford this. I should have worked harder!"));
			_ = RogueLibs.CreateCustomName(cDialogue.CantAffordElevator, t,
					new CustomNameInfo("PAYMENT DECLINED - Have a [ERR: Salutation not found (\"RejectPoorSwine\")] Day."));
			_ = RogueLibs.CreateCustomName(cDialogue.CantAffordToilet, t,
					new CustomNameInfo("I'm too poor (and therefore too lazy and unworthy) for this privilege!"));
			_ = RogueLibs.CreateCustomName(cDialogue.FlamingBarrelCookDamage, t,
					new CustomNameInfo("God fucking damn it, I always fucking burn my fucking hands!"));
			_ = RogueLibs.CreateCustomName(cDialogue.FlamingBarrelCookNoDamage, t,
					new CustomNameInfo("Mmmm, toasty. Just like the burning flesh on my fingers!"));
			_ = RogueLibs.CreateCustomName(cDialogue.MachineBusy, t, new CustomNameInfo("It's busy doing... machine things."));
			_ = RogueLibs.CreateCustomName(cDialogue.PurchaseElevator, t, new CustomNameInfo("PAYMENT ACCEPTED - Thank you for using Evilator Co.!"));
			_ = RogueLibs.CreateCustomName(cDialogue.SlotMachineJackpot_1, t, new CustomNameInfo("Chauvelin Automated Vice, Inc. presents: Jackpot!"));
			_ = RogueLibs.CreateCustomName(cDialogue.SlotMachineJackpot_2, t, new CustomNameInfo("Winner Winner, Chicken Dinner!"));
			_ = RogueLibs.CreateCustomName(cDialogue.SlotMachineJackpot_3, t,
					new CustomNameInfo("NOTE: You are not actually winning a Chicken Dinner, it's an expression."));
			_ = RogueLibs.CreateCustomName(cDialogue.SlotMachineJackpot_4, t, new CustomNameInfo("Yep... still going."));
			_ = RogueLibs.CreateCustomName(cDialogue.SlotMachineJackpot_5, t, new CustomNameInfo("Jackpot. Happy for ya."));

			t = vNameType.Interface;
			_ = RogueLibs.CreateCustomName(cButtonText.CamerasCaptureGuilty, t, new CustomNameInfo("Detect Guilty"));
			_ = RogueLibs.CreateCustomName(cButtonText.CamerasCaptureWanted, t, new CustomNameInfo("Detect Wanted"));
			_ = RogueLibs.CreateCustomName(cButtonText.DispenseIce, t, new CustomNameInfo("Dispense ice"));
			_ = RogueLibs.CreateCustomName(cButtonText.GrillFudPaid, t, new CustomNameInfo("Grill Fud"));
			_ = RogueLibs.CreateCustomName(cButtonText.HideInContainer, t, new CustomNameInfo("Hide in container"));
			_ = RogueLibs.CreateCustomName(cButtonText.OpenContainer, t, new CustomNameInfo("Open container"));
			_ = RogueLibs.CreateCustomName(cButtonText.SlotMachineHackJackpot, t, new CustomNameInfo("Penny-Slot Jackpot Promotion"));
			_ = RogueLibs.CreateCustomName(cButtonText.SlotMachinePlay1, t, new CustomNameInfo("Play"));
			_ = RogueLibs.CreateCustomName(cButtonText.SlotMachinePlay100, t, new CustomNameInfo("Play"));
			_ = RogueLibs.CreateCustomName(cButtonText.StealItem, t, new CustomNameInfo("Steal item"));
		}

		#endregion

		#region Custom

		// TODO replace this
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

		#region PlayfieldObject

		public void PlayfieldObject_00()
		{
			Postfix(typeof(PlayfieldObject), "DetermineLuck", GetType(), nameof(PlayfieldObject_DetermineLuck),
					new Type[3] { typeof(int), typeof(string), typeof(bool) });
		}

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

			if (RATStargetable)
			{
				if (agent.HasTrait<RATS>())
					luckMultiplier += 1;
				if (agent.HasTrait<RATS2>())
					luckMultiplier += 2;

				if (agent.isPlayer != 0 && agent.specialAbility == "ChronomanticDilation")
					if (BMAbilities.MSA_CD_IsCast(agent))
						luckMultiplier *= 2;
			}

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

		#endregion

		#region SpawnerMain

		public void SpawnerMain_00()
		{
			Type t = typeof(SpawnerMain);
			Type g = GetType();

			Postfix(t, nameof(SpawnerMain.spawnObjectReal), g, nameof(SpawnerMain_spawnObjectReal),
					new Type[6] { typeof(Vector3), typeof(PlayfieldObject), typeof(string), typeof(string), typeof(WorldDataObject), typeof(int) });
		}

		// TODO split this into Challenge classes FloralerFlora and LitterallyTheWorst
		public static void SpawnerMain_spawnObjectReal(Vector3 objectPos, PlayfieldObject objectSource, string objectType, string myDir,
				WorldDataObject worldDataObjects, int worldDataElementPosition, SpawnerMain __instance) // Postfix
		{
			// LitterallyTheWorst
			// FloralerFlora

			BMLog("SpawnerMain_spawnObjectReal");
			BMLog("\tobjectType: " + objectType);

			Vector2 loc = objectPos;
			int chance = 100;

			if (GC.challenges.Contains(cChallenge.FloralerFlora) || BMHeader.debugMode)
				switch (objectType)
				{
					case vObject.Bush:
						while (GC.percentChance(chance))
						{
							GC.spawnerMain.SpawnWreckagePileObject(new Vector2(loc.x + Random.Range(-0.64f, 0.64f), loc.y + Random.Range(-0.64f, 0.64f)),
									vObject.Bush, false);
							chance -= 20;
						}

						break;

					case vObject.KillerPlant:
						while (GC.percentChance(chance))
						{
							GC.spawnerMain.SpawnWreckagePileObject(new Vector2(loc.x + Random.Range(-0.64f, 0.64f), loc.y + Random.Range(-0.64f, 0.64f)),
									vObject.Bush, false);
							chance -= 20;
						}

						break;

					case vObject.Plant:
						while (GC.percentChance(chance))
						{
							GC.spawnerMain.SpawnWreckagePileObject(new Vector2(loc.x + Random.Range(-0.32f, 0.32f), loc.y + Random.Range(-0.32f, 0.32f)),
									vObject.Bush, false);
							chance -= 66;
						}

						break;

					case vObject.Tree:
						while (GC.percentChance(chance))
						{
							GC.spawnerMain.SpawnWreckagePileObject(new Vector2(loc.x + Random.Range(-0.64f, 0.64f), loc.y + Random.Range(-0.64f, 0.64f)),
									vObject.Bush, false);
							chance -= 10;
						}

						break;
				}

			if (GC.challenges.Contains(cChallenge.LitterallyTheWorst) || BMHeader.debugMode)
				switch (objectType)
				{
					case vObject.ATMMachine:
						while (GC.percentChance(chance))
						{
							GC.spawnerMain.SpawnWreckagePileObject(new Vector2(loc.x + Random.Range(-0.48f, 0.48f), loc.y + Random.Range(-0.48f, 0.48f)),
									vObject.MovieScreen, false); // Was vObject.Lamp. Also try A/C if MovieScreen doesn't work out.
							chance -= 10;
						}

						break;

					case vObject.Barbecue:
						while (GC.percentChance(chance))
						{
							GC.spawnerMain.SpawnWreckagePileObject(new Vector2(loc.x + Random.Range(-0.24f, 0.24f), loc.y + Random.Range(-0.12f, 0.12f)),
									vObject.Bush, true);
							chance -= 25;
						}

						break;

					case vObject.Boulder:
						while (GC.percentChance(1))
							GC.spawnerMain.SpawnItem(new Vector2(loc.x + Random.Range(-0.48f, 0.48f), loc.y + Random.Range(-0.48f, 0.48f)), vItem.Rock);

						while (GC.percentChance(chance))
						{
							GC.spawnerMain.SpawnWreckagePileObject(new Vector2(loc.x + Random.Range(-0.48f, 0.48f), loc.y + Random.Range(-0.48f, 0.48f)),
									vObject.FlamingBarrel, false);
							chance -= 15;
						}

						break;

					case vObject.BoulderSmall:
						while (GC.percentChance(1))
							GC.spawnerMain.SpawnItem(new Vector2(loc.x + Random.Range(-0.16f, 0.16f), loc.y + Random.Range(-0.16f, 0.16f)), vItem.Rock);

						while (GC.percentChance(chance))
						{
							GC.spawnerMain.SpawnWreckagePileObject(new Vector2(loc.x + Random.Range(-0.16f, 0.16f), loc.y + Random.Range(-0.16f, 0.16f)),
									vObject.FlamingBarrel, false);
							chance -= 10;
						}

						break;

					case vObject.FlamingBarrel:
						while (GC.percentChance(chance))
						{
							GC.spawnerMain.SpawnWreckagePileObject(new Vector2(loc.x + Random.Range(-0.08f, 0.08f), loc.y + Random.Range(-0.08f, 0.08f)),
									vObject.Bush, true);
							chance -= 50;
						}

						break;

					case vObject.Toilet:
						while (GC.percentChance(chance))
						{
							GC.spawnerMain.SpawnWreckagePileObject(new Vector2(loc.x + Random.Range(-0.16f, 0.16f), loc.y + Random.Range(-0.24f, 0.24f)),
									vObject.FlamingBarrel, false);
							chance -= 100;
						}

						break;

					case vObject.TrashCan:
						while (GC.percentChance(1))
							GC.spawnerMain.SpawnItem(new Vector2(loc.x + Random.Range(-0.32f, 0.32f), loc.y + Random.Range(-0.32f, 0.32f)), vItem.BananaPeel);

						while (GC.percentChance(chance))
						{
							GC.spawnerMain.SpawnWreckagePileObject(new Vector2(loc.x + Random.Range(-0.48f, 0.48f), loc.y + Random.Range(-0.48f, 0.48f)),
									cObject.WreckageMisc.RandomElement(), GC.percentChance(10));
							chance -= 15;
						}

						break;
				}
		}

		#endregion

		#endregion

		#region Objects

		#region Crate

		public void Crate_00()
		{
			Postfix(typeof(Crate), "DetermineButtons", GetType(), "Crate_DetermineButtons", new Type[0] { });
		}

		// TODO get rid of this / move this elsewhere
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

			Postfix(t, "DetermineButtons", g, nameof(Door_DetermineButtons), new Type[0] { });
		}

		// TODO get rid of this / move this elsewhere
		public static void Door_DetermineButtons(Door __instance) // Postfix
		{
			if (__instance.buttons.Any())
				CorrectButtonCosts(__instance);
		}

		#endregion

		#region Generator

		public void Generator_00()
		{
			Postfix(typeof(Generator), "DetermineButtons", GetType(), "Generator_DetermineButtons", new Type[0] { });
		}

		// TODO get rid of this / move this elsewhere
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

		// TODO get rid of this / move this elsewhere
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

		// TODO get rid of this / move this elsewhere
		public static void LaserEmitter_DetermineButtons(LaserEmitter __instance) // Postfix
		{
			if (__instance.buttons.Any())
				CorrectButtonCosts(__instance);
		}

		#endregion

		#region PoliceBox

		public void PoliceBox_00()
		{
			Type t = typeof(PoliceBox);
			Type g = GetType();

			Postfix(typeof(PoliceBox), "DetermineButtons", GetType(), "PoliceBox_DetermineButtons", new Type[0] { });
		}

		// TODO get rid of this / move this elsewhere
		public static void PoliceBox_DetermineButtons(PoliceBox __instance) // Postfix
		{
			if (__instance.buttons.Any())
				CorrectButtonCosts(__instance);
		}

		#endregion

		#region SatelliteDish

		public void SatelliteDish_00()
		{
			Postfix(typeof(SatelliteDish), "DetermineButtons", GetType(), "SatelliteDish_DetermineButtons", new Type[0] { });
		}

		// TODO get rid of this / move this elsewhere
		public static void SatelliteDish_DetermineButtons(SatelliteDish __instance) // PostFix
		{
			if (__instance.buttons.Any())
				CorrectButtonCosts(__instance);
		}

		#endregion

		#region SecurityCam

		public void SecurityCam_00()
		{
			Type t = typeof(SecurityCam);
			Type g = GetType();

			Prefix(t, "PressedButton", g, nameof(SecurityCam_PressedButton), new Type[2] { typeof(string), typeof(int) });
			Prefix(t, "ObjectAction", g, nameof(SecurityCam_ObjectAction), new Type[5] { typeof(string), typeof(string), typeof(float), typeof(Agent), typeof(PlayfieldObject) });
			Postfix(t, "StartLate", g, nameof(SecurityCam_StartLate), new Type[0] { });
		}

		public static bool SecurityCam_PressedButton(string buttonText, int buttonPrice, SecurityCam __instance) // Replacement
		{
			ObjectReal_PressedButton_base.GetMethodWithoutOverrides<Action<string, int>>(__instance).Invoke(buttonText, buttonPrice); // same

			switch (buttonText)
			{
				case "AttemptTurnOffSecurityCam": // same
					__instance.StartCoroutine(__instance.Operating(__instance.interactingAgent, null, 2f, true, "TurningOffSecurityCam")); // same
					return false; // same
				case "CamerasCaptureEveryone": // same
				{
					__instance.targets = "Everyone"; // same
					if (!__instance.gc.serverPlayer) // same
						__instance.interactingAgent.objectMult.ObjectAction(__instance.objectNetID, "CamerasCaptureEveryone"); // same
					__instance.RefreshButtons(); // same
					return false; // same
				}
				case cButtonText.CamerasCaptureGuilty:
				{
					__instance.targets = "Guilty";
					if (!__instance.gc.serverPlayer)
						__instance.interactingAgent.objectMult.ObjectAction(__instance.objectNetID, cButtonText.CamerasCaptureGuilty);
					__instance.RefreshButtons();
					return false;
				}
				case cButtonText.CamerasCaptureWanted:
				{
					__instance.targets = "Wanted";
					if (!__instance.gc.serverPlayer)
						__instance.interactingAgent.objectMult.ObjectAction(__instance.objectNetID, cButtonText.CamerasCaptureWanted);
					__instance.RefreshButtons();
					return false;
				}
				case "CamerasCaptureNonOwners":
				{
					__instance.targets = "NonOwners";
					if (!__instance.gc.serverPlayer)
						__instance.interactingAgent.objectMult.ObjectAction(__instance.objectNetID, "CamerasCaptureNonOwners");
					__instance.RefreshButtons();
					return false;
				}
				case "CamerasCaptureOwners":
				{
					__instance.targets = "Owners";
					if (!__instance.gc.serverPlayer)
						__instance.interactingAgent.objectMult.ObjectAction(__instance.objectNetID, "CamerasCaptureOwners");
					__instance.RefreshButtons();
					return false;
				}
				case "TurnCameraOff":
				{
					if (__instance.gc.serverPlayer)
						__instance.MakeNonFunctional(null);
					else
						__instance.interactingAgent.objectMult.ObjectAction(__instance.objectNetID, "TurnCameraOff");
					__instance.RefreshButtons();
					return false;
				}
				case "TurnCameraOn":
				{
					__instance.MakeFunctional();
					if (!__instance.gc.serverPlayer)
						__instance.interactingAgent.objectMult.ObjectAction(__instance.objectNetID, "TurnCameraOn");
					__instance.RefreshButtons();
					return false;
				}
				default:
					__instance.StopInteraction();
					return false;
			}
		}

		public static bool SecurityCam_ObjectAction(string myAction, string extraString, float extraFloat, Agent causerAgent, PlayfieldObject extraObject,
				SecurityCam __instance, ref bool ___noMoreObjectActions) // Replacement
		{
			ObjectReal_ObjectAction_base.GetMethodWithoutOverrides<Action<string, string, float, Agent, PlayfieldObject>>(__instance)
					.Invoke(myAction, extraString, extraFloat, causerAgent, extraObject);

			if (!___noMoreObjectActions)
			{
				if (myAction == "CamerasCaptureEveryone")
					__instance.targets = "Everyone";
				else if (myAction == cButtonText.CamerasCaptureGuilty)
					__instance.targets = "Guilty";
				else if (myAction == "CamerasCaptureNonOwners")
					__instance.targets = "NonOwners";
				else if (myAction == "CamerasCaptureOwners")
					__instance.targets = "Owners";
				else if (myAction == cButtonText.CamerasCaptureWanted)
					__instance.targets = "Wanted";
				else if (myAction == "FailToDisable")
					__instance.FailToDisable();
				else if (myAction == "RemoveParticles")
				{
					__instance.functional = false;
					__instance.RemoveParticles(true, false);
				}
				else if (myAction == "RemoveThenSpawnParticles")
				{
					__instance.functional = true;
					__instance.RemoveThenSpawnParticles(extraString);
				}
				else if (myAction == "SpawnParticles")
				{
					__instance.functional = true;
					__instance.SpawnParticles(false);
				}
				else if (myAction == "TurnCameraOff")
					__instance.MakeNonFunctional(null);
				else if (myAction == "TurnCameraOn")
					__instance.MakeFunctional();
			}

			___noMoreObjectActions = false;

			return false;
		}

		public static void SecurityCam_StartLate(SecurityCam __instance) // Postfix
		{
			if (__instance.owner == 85)
			{
				if (GC.challenges.Contains(cChallenge.PoliceState))
					__instance.targets = "Guilty";
				else
					__instance.targets = "Wanted";
			}
		}

		#endregion

		#region SlotMachine

		public void SlotMachine_00()
		{
			Type t = typeof(SlotMachine);
			Type g = GetType();

			Prefix(t, "DetermineButtons", g, nameof(SlotMachine_DetermineButtons), new Type[0] { });
			Prefix(t, "Gamble", g, nameof(SlotMachine_Gamble), new Type[1] { typeof(int) });
			Prefix(t, "Interact", g, nameof(SlotMachine_Interact), new Type[1] { typeof(Agent) });
			Prefix(t, "InteractFar", g, nameof(SlotMachine_InteractFar), new Type[1] { typeof(Agent) });
			Prefix(t, "PressedButton", g, nameof(SlotMachine_PressedButton), new Type[2] { typeof(string), typeof(int) });
			Postfix(t, "SetVars", g, nameof(SlotMachine_SetVars), new Type[0] { });
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
			SlotMachinePlayingSound[__instance] = false;
			SlotMachineHacked[__instance] = false;
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
				ObjectReal_DetermineButtons_base.GetMethodWithoutOverrides<Action>(__instance).Invoke();

				bool toiletWorking = false;

				if (GC.levelType != "HomeBase")
				{
					if ((__instance.interactingAgent.statusEffects.hasTrait("Diminutive") ||
									__instance.interactingAgent.statusEffects.hasStatusEffect("Shrunk")) &&
							!__instance.interactingAgent.statusEffects.hasStatusEffect("Giant"))
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
					BMHeaderTools.SayDialogue(__instance.interactingAgent, "ToiletWontGo", vNameType.Dialogue);

				return false;
			}

			return true;
		}

		public static bool Toilet_FlushYourself(Toilet __instance) // Replacement
		{
			if (__instance.interactingAgent.HasTrait<UnderdarkCitizen>())
			{
				if ((__instance.interactingAgent.statusEffects.hasTrait(vTrait.Diminutive) ||
								__instance.interactingAgent.statusEffects.hasStatusEffect(vStatusEffect.Shrunk)) &&
						!__instance.interactingAgent.statusEffects.hasStatusEffect(vStatusEffect.Giant))
				{
					List<ObjectReal> exits = new List<ObjectReal>();

					for (int i = 0; i < GC.objectRealList.Count; i++)
					{
						ObjectReal exitCandidate = GC.objectRealList[i];

						if (exitCandidate != __instance && !exitCandidate.destroyed && exitCandidate.startingChunk != __instance.startingChunk)
						{
							if (exitCandidate is Manhole)
							{
								Manhole manhole = (Manhole) exitCandidate;

								if (manhole.opened)
									exits.Add(exitCandidate);
							}
							else if (exitCandidate is Toilet)
								exits.Add(exitCandidate);
						}
					}

					if (exits.Count == 0)
						for (int j = 0; j < GC.objectRealList.Count; j++)
						{
							ObjectReal exitCandidate = GC.objectRealList[j];

							if (exitCandidate != __instance && !exitCandidate.destroyed)
							{
								if (exitCandidate is Manhole)
								{
									Manhole manhole = (Manhole) exitCandidate;

									if (manhole.opened)
										exits.Add(exitCandidate);
								}
								else if (exitCandidate is Toilet)
									exits.Add(exitCandidate);
							}
						}

					ObjectReal exit = __instance;

					if (exits.Count > 0)
						exit = exits[Random.Range(0, exits.Count - 1)];

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
					GC.spawnerMain.SpawnExplosion(__instance.interactingAgent, exit.tr.position, "Water", false, -1, false,
							__instance.FindMustSpawnExplosionOnClients(__instance.interactingAgent));
				}

				return false;
			}

			return true;
		}

		public static bool Toilet_PressedButton(string buttonText, int buttonPrice, Toilet __instance) // Prefix
		{
			if (GC.challenges.Contains(cChallenge.AnCapistan))
			{
				ObjectReal_PressedButton_base.GetMethodWithoutOverrides<Action<string, int>>(__instance).Invoke(buttonText, buttonPrice);

				if (buttonText == "FlushYourself" && __instance.moneySuccess(buttonPrice))
					__instance.FlushYourself();
				else if (buttonText == "PurgeStatusEffects" && __instance.moneySuccess(buttonPrice))
					__instance.PurgeStatusEffects();
				else
				{
					BMHeaderTools.SayDialogue(__instance.interactingAgent, cDialogue.CantAffordToilet, vNameType.Dialogue);
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

		#region Turret

		public void Turret_00()
		{
			Type t = typeof(Turret);
			Type g = GetType();

			Postfix(t, nameof(Turret.isOpponent), g, "Turret_IsOpponent", new Type[2] { typeof(Agent), typeof(bool) });
		}

		public static void Turret_IsOpponent(Agent myAgent, bool brainMustBeActive, Turret __instance, ref bool __result) // Postfix
		{
			// Public security cams
			// Police State

			if ((!brainMustBeActive || myAgent.brain.active) &&
					!myAgent.invisible &&
					!myAgent.ghost &&
					!myAgent.objectAgent &&
					(myAgent.prisoner <= 0 || myAgent.ownerID != 0) &&
					!myAgent.statusEffects.hasTrait("InvisibleToCameras"))
			{
				if (__instance.targets == "Wanted")
				{
					if (myAgent.statusEffects.hasTrait(vTrait.Wanted))
						__result = true;
				}

				if (GC.challenges.Contains(cChallenge.PoliceState)) // Can always override others
					if (myAgent.statusEffects.hasTrait(vTrait.Wanted) || myAgent.objectMultAgent.mustBeGuilty)
						__result = true;
			}
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
			VendorCartStolenFrom[__instance] = false;
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

		#endregion
	}
}