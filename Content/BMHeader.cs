using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using RogueLibsCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using BunnyMod.Content.Traits;
using BunnyMod.Properties;
using UnityEngine;

namespace BunnyMod.Content
{
	[BepInPlugin(ModInfo.BepInExPluginId, ModInfo.Title, ModInfo.Version)]
	[BepInProcess("StreetsOfRogue.exe")]
	[BepInDependency(RogueLibs.GUID, RogueLibs.CompiledVersion)]
	public class BMHeader : BaseUnityPlugin
	{
		public const bool debugMode = true;

		public static ManualLogSource ConsoleMessage;
		public static BaseUnityPlugin MainInstance;

		public void Awake()
		{
			MainInstance = this;
			ConsoleMessage = Logger;

			new Harmony(ModInfo.BepInExPluginId).PatchAll(GetType().Assembly);

			RogueLibs.LoadFromAssembly();
			BMTraitsManager.FinalizeTraits();
		}

		public static void Log(string logMessage) =>
			ConsoleMessage.LogMessage(logMessage);
	}

	public static class BMHeaderTools
	{
		private static GameController GC => GameController.gameController;
		private static void BMLog(string logMessage) => BMHeader.Log(logMessage);

		public static void AddDictionary(Dictionary<PlayfieldObject, bool> dict, PlayfieldObject objectReal, bool defaultValue)
		{
			BMLog("AddDictionaryBool");

			// May need to force types here

			if (!dict.ContainsKey(objectReal))
				dict.Add(objectReal, defaultValue);
			else
				dict[objectReal] = defaultValue;
		}

		public static void AddDictionary(Dictionary<PlayfieldObject, string> dict, PlayfieldObject objectReal, string defaultValue)
		{
			BMLog("AddDictionary");

			// May need to force types here

			if (!dict.ContainsKey(objectReal))
				dict.Add(objectReal, defaultValue);
			else
				dict[objectReal] = defaultValue;
		}

		public static T GetMethodWithoutOverrides<T>(this MethodInfo method, object callFrom)
				where T : Delegate
		{
			IntPtr ptr = method.MethodHandle.GetFunctionPointer();
			return (T) Activator.CreateInstance(typeof(T), callFrom, ptr);
		}

		public static void InvokeRepeating(object instance, string method, float delay, float interval)
		{
			MethodInfo methodAccessed = AccessTools.Method(instance.GetType(), method);
			_ = InvokeRepeatingAsync(instance, methodAccessed, (int) Mathf.Floor(delay * 1000), (int) Mathf.Floor(interval * 1000));
		}

		private static async Task InvokeRepeatingAsync(object instance, MethodInfo method, int delay, int interval)
		{
			await Task.Delay(delay);

			while (true)
			{
				method.Invoke(instance, new object[0]);
				await Task.Delay(interval);
			}
		}

		public static T RandomFromList<T>(List<T> list)
		{
			System.Random rnd = new System.Random();

			return list[rnd.Next(list.Count)];
		}

		public static void Set(this object obj, params Func<string, object>[] hash)
		{
			foreach (Func<string, object> member in hash)
			{
				var propertyName = member.Method.GetParameters()[0].Name;
				var propertyValue = member(string.Empty);
				obj.GetType()
						.GetProperty(propertyName)
						.SetValue(obj, propertyValue, null);
			}
		}

		public static void SayDialogue(Agent agent, string customNameInfo, string vNameType)
		{
			string text = GC.nameDB.GetName(customNameInfo, vNameType);
			agent.Say(text);
		}

		public static void SayDialogue(ObjectReal objectReal, string customNameInfo, string vNameType)
		{
			string text = GC.nameDB.GetName(customNameInfo, vNameType);
			objectReal.Say(text);
		}
	}

	public enum DamageType
	{
		burnedFingers,
		brokenWindow
	}

	public static class cAgent // Custom Agents
	{
		public const string
				Cook = "Cook",
				Commando = "Commando",
				Dancer = "Dancer",
				DiscoMonarch = "DiscoMonarch",
				Driver = "Driver", // "Have you seen my truck?"
				Fence = "Fence",
				Fixer = "Fixer",
				GoGanger = "GoGanger", // Roller Skates + melee, hireable with cocaine
				Hippie = "Hippie",
				Hitman = "Hitman",
				Janitor = "Janitor",
				Junkie = "Junkie",
				Mechanic = "Mechanic",
				Mercenary = "Mercenary",
				Priest = "Priest",
				Paramedic = "Paramedic",
				PostalWorker = "PostalWorker",
				RentACop = "RentACop",
				ResistanceBreacher = "ResistanceBreacher",
				ResistanceDoctor = "ReistanceDoctor",
				ResistanceHacker = "ResistanceHacker",
				ResistanceInfiltrator = "ResistanceInfiltrator",
				ResistanceSoldier = "ResistanceSoldier",
				Retiree = "Retiree",
				SecretPolice = "SecretPolice",
				ServiceDrone = "ServiceDrone",
				Sniper = "Sniper",
				Spy = "Spy",
				SWAT = "SWAT",
				Teamster = "Teamster",
				Tourist = "Tourist",
				Trapper = "Trapper",
				UndercoverCop = "UndercoverCop",
				Wildfolk = "Wildfolk";
	}
	public static class cBuffText
	{
		public const string
				Headshot = "Headshot";
	}
	public static class cButtonText
	{
		public const string
				CamerasCaptureWanted = "CamerasCaptureWanted",
				CamerasCaptureGuilty = "CamerasCaptureGuilty",
				DispenseIce = "DispenseIce",
				FountainSteal = "FountainSteal",
				FountainWishFabulousWealth = "FountainWishFabulousWealth",
				FountainWishFameAndGlory = "FountainWishFameAndGlory",
				FountainWishGoodHealth = "FountainWishGoodHealth",
				FountainWishTrueFriendship = "FountainWishTrueFriendship",
				FountainWishWorldPeace = "FountainWishWorldPeace",
				GrillFudPaid = "GrillFudPaid",
				HideInContainer = "HideInContainer",
				OpenContainer = "OpenContainer",
				SlotMachineHackJackpot = "SlotMachineHackJackpot",
				SlotMachinePlay1 = "Play1",
				SlotMachinePlay100 = "Play100",
				StealItem = "StealItem";
	}
	public static class cChallenge // Custom Mutators
	{
		public const string				
				Fire_Hide = "Fire_Hide",
				Fire_Show = "Fire_Show",
				GasolineHumidity = "FastFires",
				Mildfire = "WeakFires",
				NapalmSprings = "StubbornFires",
				
				Gameplay_Hide = "Gameplay_Hide",
				Gameplay_Show = "Gameplay_Show",
				ScaryGuns = "ScaryGuns",
				
				Knockback_Hide = "Knockback_Hide",
				Knockback_Show = "Knockback_Show",
				BoringPhysics = "BoringPhysics",
				SaveTheWalls = "SaveTheWalls",
				WallWallopWorld = "WallWallopWorld",
				
				AnCapistan = "AnCapistan",
				DiscoCityDanceoff = "DiscoCityDanceoff",
				PoliceState = "PoliceState",

				QuestCount_Hide = "QuestCount_Hide",
				QuestCount_Show = "QuestCount_Show",
				SingleMinded = "SingleMinded",
				RushinRevolution = "RushinRevolution",
				Workhorse = "Workhorse",
				
				QuestRewards_Hide = "QuestRewards_Hide",
				QuestRewards_Show = "QuestRewards_Show",
				DoublePlyRewards = "DoublePlyRewards",
				UnpaidInternship = "UnpaidInternship",
				
				Tethers_Hide = "Tethers_Hide",
				Tethers_Show = "Tethers_Show",
				HomesickAndTired = "HomesickandTired",
				RoaminOrgy = "RoaminOrgy";

		public static List<string> AffectsBullets = new List<string>()
		{
				ScaryGuns,
		};
		public static List<string> AffectsFires = new List<string>()
		{
				GasolineHumidity,
				NapalmSprings,
				Mildfire,
		};
		public static List<string> Knockback = new List<string>()
		{
				BoringPhysics,
				SaveTheWalls,
				WallWallopWorld
		};
		public static List<string> Leashes = new List<string>()
		{
				RoaminOrgy,
				HomesickAndTired
		};
		public static List<string> QuestCount = new List<string>()
		{
				RushinRevolution,
				SingleMinded,
				Workhorse
		};

	}
	public static class cDialogue // Custom Dialogue
	{
		public const string
				CantAffordAlarmButton = "AlarmButtonCantAfford",
				CantAffordElevator = "CantAffordElevator",
				CantAffordToilet = "ToiletCantAfford",
				CantUseLoud = "CantUseLoud",
				CantUseLoud_2 = "CantUseLoud_2",
				CantEatMeat = "Vegetarian",
				CantEatNonvegetarian = "CantEatNonvegetarian",
				CantUseDrug = "CantUseDrug",
				CantUseBlunt = "CantUseBlunt",
				CantUseSharp = "CantUseSharp",
				CantUseSharp_2 = "CantUseSharp_2",
				CantWearArmor = "CantWearArmor",
				CantWearHeadgear = "CantWearHeadgear",
				CantDrinkAlcohol = "CantDrinkAlcohol",
				FlamingBarrelCookDamage = "FlamingBarrelCookDamage",
				FlamingBarrelCookNoDamage = "FlamingBarrelCookNoDamage",
				MachineBusy = "MachineBusy",
				MSA_CD_CantDo1 = "MSA_CD_CantDo1",
				MSA_CD_CantDo2 = "MSA_CD_CantDo2",
				MSA_CD_CantDo3 = "MSA_CD_CantDo3",
				MSA_CD_CantDo4 = "MSA_CD_CantDo4",
				MSA_CD_Cast1 = "MSA_CD_Cast1",
				MSA_CD_Cast2 = "MSA_CD_Cast2",
				MSA_CD_Cast3 = "MSA_CD_Cast3",
				MSA_CD_Cast4 = "MSA_CD_Cast4",
				MSA_CD_Decast1 = "MSA_CS_Decast1",
				MSA_CD_Decast2 = "MSA_CS_Decast2",
				MSA_CD_Decast3 = "MSA_CS_Decast3",
				MSA_CD_Decast4 = "MSA_CS_Decast4",
				MSA_CD_Miscast1 = "MSA_CD_Miscast1",
				MSA_CD_Miscast2 = "MSA_CD_Miscast2",
				MSA_CD_Miscast3 = "MSA_CD_Miscast3",
				MSA_CD_Miscast4 = "MSA_CD_Miscast4",
				MSA_CD_Recharge1 = "MSA_CD_Recharge1",
				MSA_CD_Recharge2 = "MSA_CD_Recharge2",
				MSA_CD_Recharge3 = "MSA_CD_Recharge3",
				MSA_CD_Recharge4 = "MSA_CD_Recharge4",
				MSA_PJ_CantDo1 = "MSA_PJ_CantDo1",
				MSA_PJ_CantDo2 = "MSA_PJ_CantDo2",
				MSA_PJ_CantDo3 = "MSA_PJ_CantDo3",
				MSA_PJ_CantDo4 = "MSA_PJ_CantDo4",
				MSA_PJ_Cast1 = "MSA_PJ_Cast1",
				MSA_PJ_Cast2 = "MSA_PJ_Cast2",
				MSA_PJ_Cast3 = "MSA_PJ_Cast3",
				MSA_PJ_Cast4 = "MSA_PJ_Cast4",
				MSA_PJ_Miscast1 = "MSA_PJ_Miscast1",
				MSA_PJ_Miscast2 = "MSA_PJ_Miscast2",
				MSA_PJ_Miscast3 = "MSA_PJ_Miscast3",
				MSA_PJ_Miscast4 = "MSA_PJ_Miscast4",
				MSA_PJ_Recharge1 = "MSA_PJ_Recharge1",
				MSA_PJ_Recharge2 = "MSA_PJ_Recharge2",
				MSA_PJ_Recharge3 = "MSA_PJ_Recharge3",
				MSA_PJ_Recharge4 = "MSA_PJ_Recharge4",
				MSA_TB_CantDo1 = "MSA_TB_CantDo1",
				MSA_TB_CantDo2 = "MSA_TB_CantDo2",
				MSA_TB_CantDo3 = "MSA_TB_CantDo3",
				MSA_TB_CantDo4 = "MSA_TB_CantDo4",
				MSA_TB_Cast1 = "MSA_TB_Cast1",
				MSA_TB_Cast2 = "MSA_TB_Cast2",
				MSA_TB_Cast3 = "MSA_TB_Cast3",
				MSA_TB_Cast4 = "MSA_TB_Cast4",
				MSA_TB_Miscast1 = "MSA_TB_Miscast1",
				MSA_TB_Miscast2 = "MSA_TB_Miscast2",
				MSA_TB_Miscast3 = "MSA_TB_Miscast3",
				MSA_TB_Miscast4 = "MSA_TB_Miscast4",
				MSA_TB_Recharge1 = "MSA_TB_Recharge1",
				MSA_TB_Recharge2 = "MSA_TB_Recharge2",
				MSA_TB_Recharge3 = "MSA_TB_Recharge3",
				MSA_TB_Recharge4 = "MSA_TB_Recharge4",
				PurchaseElevator = "PurchaseElevator",
				SlotMachineJackpot_ = "SlotMachineJackpot_", // For concatenation into following
				SlotMachineJackpot_1 = "SlotMachineJackpot_1",
				SlotMachineJackpot_2 = "SlotMachineJackpot_2",
				SlotMachineJackpot_3 = "SlotMachineJackpot_3",
				SlotMachineJackpot_4 = "SlotMachineJackpot_4",
				SlotMachineJackpot_5 = "SlotMachineJackpot_5",
				VeiledThreatsAnnoyed = "ThreatenAnnoyed",
				WarlordSubmission = "WarlordSubmission";
	}
	public static class cItem // Custom Items
	{
		public const string
			BearTrapProcessor = "Bear Trap Processor",
			BeerCan = "Beer Can",
			Brick = "Brick",
			BrokenBottle = "Broken Bottle",
			EnergyDrink = "Energy Drink",
			FancyHat = "Fancy Hat",
			FireMine = "Fire Mine",
			HolySymbol = "Holy Symbol",
			HolyWaterFlask = "Holy Water Flask",
			KillArmortizer = "Kill Armortizer",
			Lunchbox = "Lunchbox",
			ManholeCover = "Manhole Cover",
			MultiTool = "Multi-Tool",
			Repairerator = "Repairerator",
			RiotHelmet = "Riot Helmet",
			Sawblade = "Sawblade",
			Spear = "Spear",
			StunGun = "Stun-Gun",
			SugarProcessor = "Sugar Processor",
			ThrowingKnife = "ThrowingKnives",
			WhiskeyBottle = "Whiskey Bottle",
			WireCutters = "Wire Cutters";
	}
	public static class cObject
	{
		public static List<string> HideableSBD = new List<string>()
		{
				vObject.Bathtub,
				vObject.Plant,
				vObject.PoolTable,
				vObject.TableBig,
				vObject.TrashCan
		};
	}
	public static class cSkillPoints // Custom Skill Points
	{
		public const string
				AngeredMany = "AngeredMany",
				BQMalusDowntown = "BQMalusDowntown",
				BQMalusFloor = "BQMalusFloor",
				BQMalusGame = "BQMalusGame",
				BQMalusIndustrial = "BQMalusIndustrial",
				BQMalusPark = "BQMalusPark",
				BQMalusSlums = "BQMalusSlums",
				BQMalusUptown = "BQMalusUptown",
				ElectabilityMalus = "ElectabilityMalus",
				FreePrisonerFailure = "FreePrisonerFailure",
				StoleNone = "StoleNone", // TODO does nothing
				TookLotsOfDamage = "TookLotsOfDamage";
	}
	public static class cTrait // Custom Traits
	{
		public const string
				AfraidOfLoudNoises = "AfraidOfLoudNoises",
				Archmage = "Archmage",
				Ballistician = "Ballistician",
				Ballistician_2 = "Ballistician2",
				BlessedStrikes = "BlessedStrikes",
				BlessedStrikes_2 = "BlessedStrikes2",
				BodyGuarded = "BodyGuarded",
				BodyGuarded_2 = "BodyGuarded2",
				Brainiac = "Brainiac",
				BootLicker = "BootLicker",
				Carnivore = "Carnivore",
				Charmed = "Charmed",
				Charmed_2 = "Charmed2",
				Cursed = "Cursed",
				Cursed_2 = "Cursed2",
				DAREdevil = "DAREdevil",
				Debug = "00_Debug",
				DimBulb = "DimBulb",
				Domineering = "Domineering",
				Domineering_2 = "Domineering2",
				DoubleTapper = "DoubleTapper",
				DoubleTapper_2 = "DoubleTapper2",
				DrawNoBlood = "DrawNoBlood",
				EagleEyes = "EagleEyes",
				EagleEyes_2 = "EagleEyes2",
				Fatass = "Fatass",
				FatHead = "FatHead",
				FocusedCasting = "FocusedCasting",
				FocusedCasting_2 = "FocusedCasting2",
				FriendOfBill = "FriendOfBill",
				GenerallyUnpleasant = "GenerallyUnpleasant",
				Haunted = "Haunted",
				InfernalStrikes = "InfernalStrikes",
				InfernalStrikes_2 = "InfernalStrikes2",
				MagicTraining = "MagicTraining",
				MagicTraining_2 = "MagicTraining2",
				ManaBattery = "ManaBattery",
				ManaBattery_2 = "ManaBattery2",
				MobDebt = "MobDebt",
				MookMasher = "MookMasher",
				MoronTheMerrier = "MoronTheMerrier",
				Myopic = "Myopic",
				Myopic2 = "Myopic2",
				ObjectivelyUnpleasant = "ObjectivelyUnpleasant",
				Polarizing = "Polarizing",
				Polarizing_2 = "Polarizing2",
				Priors = "Priors",
				RATS = "RATS",
				RATS_2 = "RATS2",
				Reinforcements = "Reinforcements",
				Reinforcements_2 = "Reinforcements2",
				SharpOnly = "SharpOnly",
				SmoothBrained = "SmoothBrained",
				Sniper = "Sniper",
				Sniper_2 = "Sniper2",
				StealthBastardDeluxe = "StealthBastardDeluxe",
				TamperTantrum = "TamperTantrum",
				TamperTantrum_2 = "TamperTantrum2",
				Teetotaller = "Teetotaller",
				UnderdarkCitizen = "UnderdarkCitizen",
				Vegetarian = "Vegetarian",
				VeiledThreats = "VeiledThreats",
				VeryHardOnYourself = "VeryHardOnYourself",
				Warlord = "Warlord",
				WildCasting = "WildCasting",
				WildCasting_2 = "WildCasting2";

		public static List<string> BulletRange = new List<string>()
		{
				Ballistician,
				Ballistician_2,
				Sniper
		};

		public static List<string> ExperienceRate = new List<string>()
		{
				DimBulb,
				MoronTheMerrier,
				SmoothBrained
		};

		public static List<string> LimitArmor = new List<string>()
				{ };

		public static List<string> LimitDrugs = new List<string>()
		{
				DAREdevil,
				FriendOfBill,
				Teetotaller,
		};

		public static List<string> LimitFood = new List<string>()
		{
				Carnivore,
				Vegetarian
		};

		public static List<string> LimitHeadgear = new List<string>()
		{
				FatHead
		};

		public static List<string> LimitWeapons = new List<string>()
		{
				AfraidOfLoudNoises,
				DrawNoBlood,
		};

		public static List<string> Luck = new List<string>()
		{
				Charmed,
				Charmed_2,
				Cursed,
				Cursed_2,
		};

		public static List<string> Magic = new List<string>()
		{
				Archmage,
				FocusedCasting,
				FocusedCasting_2,
				MagicTraining,
				MagicTraining_2,
				ManaBattery,
				ManaBattery_2,
				WildCasting,
				WildCasting_2,
		};

		public static List<string> MagicStyles = new List<string>()
		{
				Archmage,
				FocusedCasting,
				FocusedCasting_2,
				WildCasting,
				WildCasting_2,
		};

		public static List<string> RelationshipInitial = new List<string>()
		{
				BootLicker,
				Domineering,
				Domineering_2,
				GenerallyUnpleasant,
				ObjectivelyUnpleasant,
				Polarizing,
				Polarizing_2,
				Priors,
		};

		public static List<string> Spawns = new List<string>()
		{
				Haunted,
				MobDebt,
				MookMasher,
				Reinforcements,
				Reinforcements_2,
		};

		public static List<string> VisionRange = new List<string>()
		{
				EagleEyes,
				EagleEyes_2,
				Myopic,
				Myopic2,
		};
	}
	public static class cStatuseffect // Custom Status Effects
	{
		public const string
				ElevatorAccess = "ElevatorAccess";
	}
	public static class vAgent // Vanilla Agent Classes
	{
		public const string
				Alien = "Alien",
				Assassin = "Assassin",
				Athlete = "Athlete",
				Bartender = "Bartender",
				Blahd = "Gangbanger",
				Bouncer = "Bouncer",
				Cannibal = "Cannibal",
				Clerk = "Clerk",
				Comedian = "Comedian",
				Cop = "Cop",
				CopBot = "CopBot",
				Courier = "Courier",
				Crepe = "GangbangerB",
				Demolitionist = "Demolitionist",
				Doctor = "Doctor",
				DrugDealer = "DrugDealer",
				Firefighter = "Firefighter",
				Ghost = "Ghost",
				Goon = "Guard",
				Gorilla = "Gorilla",
				Hacker = "Hacker",
				InvestmentBanker = "Businessman",
				Mayor = "Mayor",
				MechPilot = "MechPilot",
				Mobster = "Mafia",
				Musician = "Musician",
				OfficeDrone = "OfficeDrone",
				ResistanceLeader = "ResistanceLeader",
				Robot = "Robot",
				RobotPlayer = "RobotPlayer",
				Scientist = "Scientist",
				ShapeShifter = "ShapeShifter",
				Shopkeeper = "Shopkeeper",
				Slave = "Slave",
				Slavemaster = "Slavemaster",
				SlumDweller = "Hobo",
				Soldier = "Soldier",
				SuperCop = "Cop2",
				Supergoon = "Guard2",
				Thief = "Thief",
				UpperCruster = "UpperCruster",
				Vampire = "Vampire",
				Werewolf = "WerewolfB",
				Worker = "Worker",
				Wrestler = "Wrestler",
				Zombie = "Zombie";

		public static List<string> Criminal = new List<string>()
		{
				Blahd,
				Crepe,
				DrugDealer,
				Mobster,
				Thief,
		};

		public static List<string> Evil = new List<string>()
		{
				ShapeShifter,
				Vampire,
				Zombie
		};

		public static List<string> LawEnforcement = new List<string>()
		{
				Cop,
				CopBot,
				SuperCop,
		};

		public static List<string> Nonhuman = new List<string>()
		{
				Alien,
				CopBot,
				Gorilla,
				Ghost,
				Robot,
				RobotPlayer,
				ShapeShifter,
				Werewolf,
				Vampire,
				Zombie,
		};

		public static List<string> Supernatural = new List<string>()
		{
				Ghost,
				Werewolf,
				Vampire,
				Zombie,
		};

		public static List<string> Undead = new List<string>()
		{
				Ghost,
				Vampire,
				Zombie
		};
	}
	public static class vAgentGoal // Vanilla Agent Goals
	{
		public const string
				CuriousObject = "CuriousObject",
				Dance = "Dance",
				Guard = "Guard",
				IceSkate = "IceSkate",
				Idle = "Idle",
				TellJoke = "Joke",
				ListenToJoke = "ListenToJokeNPC",
				Null = "None",
				Patrol = "Patrol",
				Sit = "Sit",
				Sleep = "Sleep",
				Swim = "Swim",
				Wander = "Wander",
				WanderLevel = "WanderFar",
				WanderOwned = "WanderInOwnedProperty";
	}
	public static class vAgentGroup // Vanilla Agent Groups
	{
		public const string
				ArenaBattlers = "ArenaBattler",
				BlueCollars = "BlueCollars",
				BusinessOwners = "BusinessOwners",
				Cabin = "ParkHome",
				Cave = "Cave",
				Creatures = "Creatures",
				Downtown = "Downtown",
				Entertainers = "Entertainers",
				Fighters = "Fighters",
				Gangbangers = "GangbangerType",
				Guards = "GuardType",
				Hideout = "Hideout",
				HideoutIndustrial = "HideoutIndustrial",
				Hooligans = "Hooligans",
				HooligansNoGangbangers = "HooligansNoGangbangers",
				Hostages = "Kidnapped",
				LabSpecimens = "LabSpecimens",
				LawEnforcement = "LawWorkers",
				Park = "Park",
				Slums = "SlumsAgent",
				Uptown = "Uptown",
				UptownHome = "UptownHome",
				WhiteCollars = "WhiteCollars";
	}
	public static class vAmbience // Vanilla Ambient Audio Loops
	{
		public const string
				AirConditioner = "AirFiltrationAmbience",
				BathHouse = "SpaAmbience",
				Casino = "CasinoAmbience",
				Cave = "CavernAmbience",
				Computer = "ComputerAmbience",
				ConveyorBelt = "EscalatorAmbience",
				Generator = "GeneratorAmbience",
				GeneratorOverclocked = "OverclockedGeneratorAmbience",
				Graveyard = "GraveyardAmbience",
				LampPost = "LampPostAmbience",
				Laser = "LaserAmbience",
				Park = "ParkAmbience",
				Television = "TVAmbience";
	}
	public static class vAnimation // Vanilla Object Animations
	{
		public const string
				MachineOperate = "MachineOperate";
	}
	public static class vArmor // Vanilla Armor
	{
		public const string
				BraceletOfStrength = "BraceletStrength",
				BulletproofVest = "BulletproofVest",
				Codpiece = "CodPiece",
				FireproofSuit = "FireproofSuit",
				MayorBadge = "MayorBadge",
				MoodRing = "MoodRing",
				Null = "";
	}
	public static class vArmorHead // Vanilla Headgear
	{
		public const string
				BlueHat = "HatBlue",
				CopHat = "CopHat",
				DoctorHeadLamp = "DoctorHeadLamp",
				FireHelmet = "FireHelmet",
				GasMask = "GasMask",
				HackerGlasses = "HackerGlasses",
				HardHat = "HardHat",
				Headphones = "Headphones",
				MafiaHat = "Fedora",
				MayorHat = "MayorHat",
				Null = "",
				RedHat = "HatRed",
				SlaveHelmet = "SlaveHelmet",
				SoldierHelmet = "SoldierHelmet",
				Sunglasses = "Sunglasses",
				SuperCopHat = "Cop2Hat",
				ThiefHat = "ThiefHat";
	}
	public static class vAudioClip // Vanilla Audio Clips
	{
		public const string
				AddTrait = "AddTrait",
				AgentAlert = "AgentAlert",
				AgentAnnoyed = "AgentAnnoyed",
				AgentArrest = "AgentArrest",
				AgentDie = "AgentDie",
				AgentDie1 = "AgentDie1",
				AgentDie2 = "AgentDie2",
				AgentDie3 = "AgentDie3",
				AgentEnslave = "AgentEnslave",
				AgentFlee = "AgentFlee",
				AgentGiant1 = "AgentGiant1",
				AgentGiant2 = "AgentGiant2",
				AgentGiant3 = "AgentGiant3",
				AgentGib = "AgentGib",
				AgentInvestigate = "AgentInvestigate",
				AgentInvestigate2 = "AgentInvestigate2",
				AgentInvestigate3 = "AgentInvestigate3",
				AgentJoin = "AgentJoin",
				AgentJoin2 = "AgentJoin2",
				AgentJoin3 = "AgentJoin3",
				AgentJoke = "AgentJoke",
				AgentKnockOut = "AgentKnockOut",
				AgentKnockout2 = "AgentKnockout2",
				AgentKnockout3 = "AgentKnockout3",
				AgentLaugh = "AgentLaugh",
				AgentOK = "AgentOK",
				AgentOK2 = "AgentOK2",
				AgentRevive = "AgentRevive",
				AgentReviveZombie = "AgentReviveZombie",
				AgentRuckus = "AgentRuckus",
				AgentRuckus2 = "AgentRuckus2",
				AgentRuckus3 = "AgentRuckus3",
				AgentShrink = "AgentShrink",
				AgentTalk = "AgentTalk",
				AgentTalk2 = "AgentTalk2",
				AgentTalk3 = "AgentTalk3",
				AgentTalk4 = "AgentTalk4",
				AgentTalk5 = "AgentTalk5",
				AirFiltrationAmbience = "AirFiltrationAmbience",
				AlarmButton = "AlarmButton",
				AmmoOut1 = "AmmoOut1",
				AmmoOut2 = "AmmoOut2",
				AmmoOutLaserGun = "AmmoOutLaserGun",
				ArmedMine = "ArmedMine",
				ArmorBreak = "ArmorBreak",
				ATMDeposit = "ATMDeposit",
				BearTrapSnap = "BearTrapSnap",
				BeginCombine = "BeginCombine",
				BigUnlock = "BigUnlock",
				BiteCannibal = "BiteCannibal",
				BiteCannibal2 = "BiteCannibal2",
				BiteCannibal3 = "BiteCannibal3",
				BloodSuck = "BloodSuck",
				Boombox1 = "Boombox1",
				Boombox2 = "Boombox2",
				BulletHitAgent = "BulletHitAgent",
				BulletHitIndestructibleObject = "BulletHitIndestructibleObject",
				BulletHitObject = "BulletHitObject",
				BulletHitWall = "BulletHitWall",
				BushDestroy = "BushDestroy",
				ButlerBotClean = "ButlerBotClean",
				BuyItem = "BuyItem",
				BuyUnlock = "BuyUnlock",
				CannibalFinish = "CannibalFinish",
				CantDo = "CantDo",
				CasinoAmbience = "CasinoAmbience",
				CavernAmbience = "CavernAmbience",
				ChainsawSwing = "ChainsawSwing",
				ChainsawSwing2 = "ChainsawSwing2",
				ChargeIntoSolidWall = "ChargeIntoSolidWall",
				ChargeLand = "ChargeLand",
				ChargeLaunch = "ChargeLaunch",
				ChargePrepare = "ChargePrepare",
				ChestOpen = "ChestOpen",
				ChloroformAgent = "ChloroformAgent",
				ChloroformAgent2 = "ChloroformAgent2",
				ChooseDisaster = "ChooseDisaster",
				ChooseReward = "ChooseReward",
				ClickButton = "ClickButton",
				CloseMenu = "CloseMenu",
				ClubMusic = "ClubMusic",
				CombineItem = "CombineItem",
				ComputerAmbience = "ComputerAmbience",
				ConveyorBelt = "ConveyorBelt",
				CopBotCam = "CopBotCam",
				CopBotDetect = "CopBotDetect",
				Countdown = "Countdown",
				CountdownEnd = "CountdownEnd",
				Credits = "Credits",
				DeliverPackage = "DeliverPackage",
				Depossess = "Depossess",
				DialogueTextCrawl1 = "DialogueTextCrawl1",
				DialogueTextCrawl2 = "DialogueTextCrawl2",
				DialogueTextCrawl3 = "DialogueTextCrawl3",
				DialogueTextCrawl4 = "DialogueTextCrawl4",
				DialogueTextCrawl5 = "DialogueTextCrawl5",
				DialogueTextCrawl6 = "DialogueTextCrawl6",
				DialogueTextCrawl7 = "DialogueTextCrawl7",
				DialogueTextCrawl8 = "DialogueTextCrawl8",
				Dizzy = "Dizzy",
				DoorClose = "DoorClose",
				DoorCloseAI = "DoorCloseAI",
				DoorKnock = "DoorKnock",
				DoorOpen = "DoorOpen",
				DoorOpenAI = "DoorOpenAI",
				DropItem = "DropItem",
				ElectroZap = "ElectroZap",
				ElectroZap2 = "ElectroZap2",
				EnterMech = "EnterMech",
				EquipArmor = "EquipArmor",
				EquipWeapon = "EquipWeapon",
				EscalatorAmbience = "EscalatorAmbience",
				ExitMech = "ExitMech",
				ExplodeDizzy = "ExplodeDizzy",
				ExplodeGiant = "ExplodeGiant",
				ExplodeMindControl = "ExplodeMindControl",
				ExplodeMindControl2 = "ExplodeMindControl2",
				ExplodeMindControl3 = "ExplodeMindControl3",
				ExplodeMonkeyBarrel = "ExplodeMonkeyBarrel",
				ExplodeSlime = "ExplodeSlime",
				ExplodeWarp = "ExplodeWarp",
				ExplodeWater = "ExplodeWater",
				Explosion = "Explosion",
				ExplosionEMP = "ExplosionEMP",
				Fail = "Fail",
				FallInHole = "FallInHole",
				FireballHitAgent = "FireballHitAgent",
				FireConstant = "FireConstant",
				FireExtinguisherEnd = "FireExtinguisherEnd",
				FireExtinguisherFire = "FireExtinguisherFire",
				FireExtinguisherLoop = "FireExtinguisherLoop",
				FireHit = "FireHit",
				FireHitShort = "FireHitShort",
				FireHydrantBreak = "FireHydrantBreak",
				FirePersist = "FirePersist",
				FireSpewerFire = "FireSpewerFire",
				FireworksFire1 = "FireworksFire1",
				FireworksFire2 = "FireworksFire2",
				FireworksFire3 = "FireworksFire3",
				FireworksLoop = "FireworksLoop",
				FlameThrowerEnd = "FlameThrowerEnd",
				FlamethrowerFireOld = "FlamethrowerFire-Old",
				FlamethrowerFire = "FlamethrowerFire",
				FlameThrowerLoop = "FlameThrowerLoop",
				FlamingBarrelCrackle = "FlamingBarrelCrackle",
				FloorClear = "FloorClear",
				FloorTrap = "FloorTrap",
				Freeze = "Freeze",
				FreezeRayFire = "FreezeRayFire",
				FreezeRayFireOld = "FreezeRayFireOld",
				GasConstant = "GasConstant",
				GasSpawn = "GasSpawn",
				GeneratorAmbience = "GeneratorAmbience",
				GeneratorHiss = "GeneratorHiss",
				GhostGibberEnd = "GhostGibberEnd",
				GhostGibberFire = "GhostGibberFire",
				GhostGibberLoop = "GhostGibberLoop",
				GoInvisible = "GoInvisible",
				GraveyardAmbience = "GraveyardAmbience",
				Grill = "Grill",
				GrillOperate = "GrillOperate",
				Hack = "Hack",
				Hack2 = "Hack2",
				Heal = "Heal",
				Hide = "Hide",
				HideInterface = "HideInterface",
				HighVolume = "HighVolume",
				Hoist = "Hoist",
				Home_Base_v2 = "Home_Base_v2",
				Hypnotize = "Hypnotize",
				IceBreak = "IceBreak",
				InstallMalware = "InstallMalware",
				Intro_Hit = "Intro_Hit",
				Intro_Loop = "Intro_Loop",
				Intro_Whoosh = "Intro_Whoosh",
				ItemFallInHole = "ItemFallInHole",
				ItemFallInHole2 = "ItemFallInHole2",
				ItemFallInHoleSmall = "ItemFallInHoleSmall",
				ItemHitItem = "ItemHitItem",
				Jukebox = "Jukebox",
				Jump = "Jump",
				JumpIntoWater = "JumpIntoWater",
				JumpIntoWater2 = "JumpIntoWater2",
				JumpIntoWater3 = "JumpIntoWater3",
				JumpOutWater = "JumpOutWater",
				KillerPlantBite = "KillerPlantBite",
				KillerPlantSnap = "KillerPlantSnap",
				KillerPlantSnap2 = "KillerPlantSnap2",
				KillerPlantSnapOther = "KillerPlantSnapOther",
				LampDestroy = "LampDestroy",
				LamppostAmbience = "LamppostAmbience",
				Land = "Land",
				LaserAmbience = "LaserAmbience",
				LaserGunFire = "LaserGunFire",
				LaserGunFire2 = "LaserGunFire2",
				LaserGunFire3 = "LaserGunFire3",
				LaserGunFireEmpty = "LaserGunFireEmpty",
				LeafBlowerEnd = "LeafBlowerEnd",
				LeafBlowerFire = "LeafBlowerFire",
				LeafBlowerLoop = "LeafBlowerLoop",
				Level1_1 = "Level1_1",
				Level1_2 = "Level1_2",
				Level1_3 = "Level1_3",
				Level2_1 = "Level2_1",
				Level2_2 = "Level2_2",
				Level2_3 = "Level2_3",
				Level3_1 = "Level3_1",
				Level3_2 = "Level3_2",
				Level3_3 = "Level3_3",
				Level4_1 = "Level4_1",
				Level4_2 = "Level4_2",
				Level4_3 = "Level4_3",
				Level5_1 = "Level5_1",
				Level5_2 = "Level5_2",
				Level5_3 = "Level5_3",
				Level6 = "Level6",
				LevelIntro = "LevelIntro",
				LevelUp = "LevelUp",
				LevelWarning = "LevelWarning",
				LockdownWallDown = "LockdownWallDown",
				LockdownWallUp = "LockdownWallUp",
				LoseStatusEffect = "LoseStatusEffect",
				LungePrepare = "LungePrepare",
				LungeSwing = "LungeSwing",
				LungeSwing2 = "LungeSwing2",
				MachineGunFire = "MachineGunFire",
				MakeOffering = "MakeOffering",
				ManholeOpen = "ManholeOpen",
				MeleeHitAgentCutLarge = "MeleeHitAgentCutLarge",
				MeleeHitAgentCutSmall = "MeleeHitAgentCutSmall",
				MeleeHitAgentCutSmall2 = "MeleeHitAgentCutSmall2",
				MeleeHitAgentLarge = "MeleeHitAgentLarge",
				MeleeHitAgentSmall = "MeleeHitAgentSmall",
				MeleeHitAgentSmall2 = "MeleeHitAgentSmall2",
				MeleeHitAgentSmall3 = "MeleeHitAgentSmall3",
				MeleeHitMelee = "MeleeHitMelee",
				MeleeHitMelee2Old = "MeleeHitMelee2-Old",
				MeleeHitMelee2 = "MeleeHitMelee2",
				MeleeHitMelee3Old = "MeleeHitMelee3-Old",
				MeleeHitMelee3 = "MeleeHitMelee3",
				MeleeHitMelee4 = "MeleeHitMelee4",
				MeleeHitMelee5 = "MeleeHitMelee5",
				MeleeHitMeleeBlade = "MeleeHitMeleeBlade",
				MeleeHitMeleeBlade2 = "MeleeHitMeleeBlade2",
				MeleeHitMeleeBlade3 = "MeleeHitMeleeBlade3",
				MeleeHitMeleeBlade4 = "MeleeHitMeleeBlade4",
				MeleeWeaponBreak = "MeleeWeaponBreak",
				MenuMove = "MenuMove",
				MindControlEnd = "MindControlEnd",
				MindControlFire = "MindControlFire",
				MindControlFire2 = "MindControlFire2",
				MindControlFire3 = "MindControlFire3",
				MindControlSuccess = "MindControlSuccess",
				MindControlSuccess2 = "MindControlSuccess2",
				MindControlSuccess3 = "MindControlSuccess3",
				MineCart = "MineCart",
				MolotovCocktailBreak = "MolotovCocktailBreak",
				MovieScreen = "MovieScreen",
				ObjectDestroy = "ObjectDestroy",
				ObjectOnFire = "ObjectOnFire",
				OpenMenu = "OpenMenu",
				OperateTrapDoor = "OperateTrapDoor",
				Operating = "Operating",
				OperatingArrest = "OperatingArrest",
				OperatingElectronicsSuccess = "OperatingElectronicsSuccess",
				OverclockedGeneratorAmbience = "OverclockedGeneratorAmbience",
				Paralyzer = "Paralyzer",
				ParkAmbience = "ParkAmbience",
				PistolFire = "PistolFire",
				PlayerDeath = "PlayerDeath",
				PoisonDamage = "PoisonDamage",
				Poisoned = "Poisoned",
				PolluteWater = "PolluteWater",
				Possess = "Possess",
				PourOil = "PourOil",
				PourOil2 = "PourOil2",
				PourOil3 = "PourOil3",
				PourOil4 = "PourOil4",
				PourOil5 = "PourOil5",
				PowerBox = "PowerBox",
				PushTrap = "PushTrap",
				PushTrapHit = "PushTrapHit",
				PushTrapHitWall = "PushTrapHitWall",
				PushTrapRetract = "PushTrapRetract",
				QuestAccept = "QuestAccept",
				QuestComplete = "QuestComplete",
				QuestCompleteBig = "QuestCompleteBig",
				QuestFail = "QuestFail",
				RadiationBlast = "RadiationBlast",
				RapOnWindow = "RapOnWindow",
				Recharge = "Recharge",
				RefillWaterCannon = "RefillWaterCannon",
				ResearchGunEnd = "ResearchGunEnd",
				ResearchGunFire = "ResearchGunFire",
				ResearchGunLoop = "ResearchGunLoop",
				RevolverFire1 = "RevolverFire1",
				RevolverFire2 = "RevolverFire2",
				RevolverFire3 = "RevolverFire3",
				Rimshot = "Rimshot",
				RobotDeath = "RobotDeath",
				RobotWalk = "RobotWalk",
				RobotWalk2 = "RobotWalk2",
				RobotWalk3 = "RobotWalk3",
				RobotWalk4 = "RobotWalk4",
				RobotWalkLoop = "RobotWalkLoop",
				RocketLauncherFire = "RocketLauncherFire",
				RocketLauncherHitIndestructible = "RocketLauncherHitIndestructible",
				SatelliteAdjust = "SatelliteAdjust",
				SatelliteBroadcast = "SatelliteBroadcast",
				SawBladeHit = "SawBladeHit",
				SawBladeHit2 = "SawBladeHit2",
				SawBladeRun = "SawBladeRun",
				SecurityCamSpot = "SecurityCamSpot",
				SecurityCamTurn = "SecurityCamTurn",
				SecurityShutdown = "SecurityShutdown",
				SelectCharacter = "SelectCharacter",
				SelectCharacter2 = "SelectCharacter2",
				SelectCharacter3 = "SelectCharacter3",
				SelectCharacter4 = "SelectCharacter4",
				SelectItem = "SelectItem",
				ShotgunFire = "ShotgunFire",
				ShowInterface = "ShowInterface",
				ShrinkRayFire = "ShrinkRayFire",
				SilencedBulletHitObject = "SilencedBulletHitObject",
				SilencedGun = "SilencedGun",
				SilencedGun2 = "SilencedGun2",
				SilencedGun3 = "SilencedGun3",
				Singe = "Singe",
				SlideWhistle = "SlideWhistle",
				Slip = "Slip",
				SlipLand = "SlipLand",
				SpaAmbience = "SpaAmbience",
				SpeechEnding = "SpeechEnding",
				SpeechMain = "SpeechMain",
				SpinRecordFail = "SpinRecordFail",
				SpinRecordSuccess = "SpinRecordSuccess",
				SpitFireball = "SpitFireball",
				SpitMeat = "SpitMeat",
				SplitScreenSwitch = "SplitScreenSwitch",
				StatusEffectShift = "StatusEffectShift",
				Stomp = "Stomp",
				Stomp2 = "Stomp2",
				Stomp3 = "Stomp3",
				Success = "Success",
				SwingWeaponFist = "SwingWeaponFist",
				SwingWeaponLarge = "SwingWeaponLarge",
				SwingWeaponSmall = "SwingWeaponSmall",
				TakeMoney = "TakeMoney",
				TakePicture = "TakePicture",
				TaserFire = "TaserFire",
				TaserFireFail = "TaserFireFail",
				TaserHitAgent = "TaserHitAgent",
				Teleport = "Teleport",
				ThrowItem = "ThrowItem",
				ThrowMoneyInWell = "ThrowMoneyInWell",
				TitleScreen = "TitleScreen",
				TitleScreenLogo = "TitleScreenLogo",
				ToiletPurge = "ToiletPurge",
				ToiletTeleportIn = "ToiletTeleportIn",
				ToiletTeleportOut = "ToiletTeleportOut",
				Track_A_Tutorial_Start = "Track_A_Tutorial_Start",
				Track_Hype_v4 = "Track_Hype_v4",
				Train = "Train",
				TranquilizerFire = "TranquilizerFire",
				TranquilizerHitAgent = "TranquilizerHitAgent",
				TrapDoorClose = "TrapDoorClose",
				TreeDestroy = "TreeDestroy",
				TreeHit = "TreeHit",
				TreeHit2 = "TreeHit2",
				TripLaser = "TripLaser",
				TripLaser2 = "TripLaser2",
				TVAmbience = "TVAmbience",
				UnHide = "UnHide",
				UseBlindenizer = "UseBlindenizer",
				UseBooUrn = "UseBooUrn",
				UseCigaretteLighter = "UseCigaretteLighter",
				UseCigarettes = "UseCigarettes",
				UseCocaine = "UseCocaine",
				UseCologne = "UseCologne",
				UseDeliveryApp = "UseDeliveryApp",
				UseDrink = "UseDrink",
				UseEarwarpWhistle = "UseEarwarpWhistle",
				UseExplosiveStimulator = "UseExplosiveStimulator",
				UseFireworks = "UseFireworks",
				UseFood = "UseFood",
				UseItemTeleporter = "UseItemTeleporter",
				UseItemTeleporter2 = "UseItemTeleporter2",
				UseItemTeleporter3 = "UseItemTeleporter3",
				UseMemoryEraser = "UseMemoryEraser",
				UseNecronomicon = "UseNecronomicon",
				UseSyringe = "UseSyringe",
				UseWalkieTalkie = "UseWalkieTalkie",
				WallDestroy = "WallDestroy",
				WallDestroyGlass = "WallDestroyGlass",
				WallDestroyGlass1 = "WallDestroyGlass1",
				WallDestroyGlass2 = "WallDestroyGlass2",
				WallDestroyGlass3 = "WallDestroyGlass3",
				WaterBlastEnd = "WaterBlastEnd",
				WaterBlastFire = "WaterBlastFire",
				WaterBlastLoop = "WaterBlastLoop",
				WaterCannonEnd = "WaterCannonEnd",
				WaterCannonFire = "WaterCannonFire",
				WaterCannonLoop = "WaterCannonLoop",
				WaterFire = "WaterFire",
				WaterHit = "WaterHit",
				WaterPistolFire = "WaterPistolFire",
				WaterPistolHitAgent = "WaterPistolHitAgent",
				WerewolfSlash1Old = "WerewolfSlash1-Old",
				WerewolfSlash1 = "WerewolfSlash1",
				WerewolfSlash2Old = "WerewolfSlash2-Old",
				WerewolfSlash2 = "WerewolfSlash2",
				WerewolfTransform = "WerewolfTransform",
				WerewolfTransformBack = "WerewolfTransformBack",
				Whoosh2 = "Whoosh2",
				Win = "Win",
				WindowDamage = "WindowDamage",
				WithdrawalDamage = "WithdrawalDamage",
				WithdrawalStart = "WithdrawalStart",
				ZombieSpitCharge = "ZombieSpitCharge",
				ZombieSpitFire = "ZombieSpitFire";
	}
	public static class vChallenge // Vanilla Mutators
	{
		public const string
				AssassinsEveryLevel = "AssassinsEveryLevel",
				BigKnockback = "BigKnockbackForAll",
				CoolWithCannibals = "CannibalsDontAttack",
				DoctorsMoreImportant = "DoctorsMoreImportant",
				EveryoneHatesYou = "EveryoneHatesYou",
				ExplodingBodies = "ExplodingBodies",
				FullHealth = "FullHealth",
				GorillaTown = "GorillaTown",
				HalfHealth = "HalfHealth",
				HighCost = "HighCost",
				InfiniteAmmo = "InfiniteAmmo",
				InfiniteAmmoNormalWeapons = "InfiniteAmmoNormalWeapons",
				InfiniteMeleeDurability = "InfiniteMeleeDurability",
				LowHealth = "LowHealth",
				ManyWerewolf = "ManyWerewolf",
				MixedUpLevels = "MixedUpLevels",
				MoneyRewards = "MoneyRewards",
				NoCops = "NoCops",
				NoCowards = "NoCowards",
				NoGuns = "NoGuns",
				NoLimits = "NoLimits",
				NoMelee = "NoMelee",
				RocketLaunchers = "RocketLaunchers",
				RogueVision = "RogueVision",
				SlowDown = "SlowDown",
				SpeedUp = "SpeedUp",
				SupercopLand = "SupercopsReplaceCops",
				TimeLimit = "TimeLimit",
				TimeLimit2 = "TimeLimit2",
				TimeLimitQuestsGiveMoreTime = "TimeLimitQuestsGiveMoreTime",
				ZombieMutator = "ZombieMutator",
				ZombiesWelcome = "ZombiesWelcome";

		public static List<string> AddsLawEnforcement = new List<string>()
		{
				SupercopLand,
		};

		public static List<string> AddsNonhumans = new List<string>()
		{
				CoolWithCannibals,
				ZombieMutator,
				ZombiesWelcome,
		};

		public static List<string> RemovesLawEnforcement = new List<string>()
		{
				NoCops,
		};

		public static List<string> Zombies = new List<string>()
		{
				ZombieMutator,
				ZombiesWelcome,
		};
	}
	public static class vChunkType // Vanilla Chunks
	{
		#region All Chunk Types

		public const string
				Apartments = "Apartments",
				Arcade = "Arcade",
				Arena = "Arena",
				Armory = "Armory",
				Bank = "Bank",
				Bar = "Bar",
				Bathhouse = "Bathhouse",
				Bathroom = "Bathroom",
				Cabin = "Cabin",
				Casino = "Casino",
				Cave = "Cave",
				Church = "Church",
				CityPark = "CityPark",
				ConfiscationCenter = "ConfiscationCenter",
				DanceClub = "DanceClub",
				DeportationCenter = "DeportationCenter",
				DrugDen = "DrugDen",
				Farm = "Farm",
				FireStation = "FireStation",
				GatedCommunity = "GatedCommunity",
				Generic = "Generic",
				Graveyard = "Graveyard",
				Greenhouse = "Greenhouse",
				HedgeMaze = "HedgeMaze",
				Hideout = "Hideout",
				Hospital = "Hospital",
				Hotel = "Hotel",
				House = "House",
				HouseUptown = "HouseUptown",
				IceRink = "IceRink",
				Lab = "Lab",
				Mall = "Mall",
				Mansion = "Mansion",
				MayorHouse = "MayorHouse",
				MayorOffice = "MayorOffice",
				MilitaryOutpost = "MilitaryOutpost",
				MovieTheater = "MovieTheater",
				MusicHall = "MusicHall",
				None = "None",
				OfficeBuilding = "OfficeBuilding",
				Pit = "Pit",
				PodiumPark = "PodiumPark",
				PoliceOutpost = "PoliceOutpost",
				PoliceStation = "PoliceStation",
				Prison = "Prison",
				PrivateClub = "PrivateClub",
				Shack = "Shack",
				Shop = "Shop",
				SlaveShop = "SlaveShop",
				TVStation = "TVStation",
				Zoo = "Zoo";

		#endregion

		#region AnCapistan

		public static List<string> AnCapistanLimitedTo2 = new List<string>()
		{
				GatedCommunity,
				Prison,
				SlaveShop,
		};

		public static List<string> AnCapistanUnlimited = new List<string>()
		{
				DrugDen,
				Shack,
		};

		public static List<string> AnCapistanProhibited = new List<string>()
		{
				ConfiscationCenter,
				CityPark,
				DeportationCenter,
				PoliceOutpost,
				PoliceStation,
		};

		#endregion

		#region Downtown

		public static List<string> DowntownLimitedTo1 = new List<string>()
		{
				Arcade,
				Arena,
				// Bank, // [sic], but prohibited
				Bathroom,
				Church,
				CityPark,
				DanceClub,
				FireStation,
				Graveyard,
				Hideout,
				IceRink,
				Mall,
				MovieTheater,
				MusicHall,
				PoliceStation,
				Shop,
				SlaveShop,
		};

		public static List<string> DowntownLimitedTo2 = new List<string>()
		{
				Bar,
				Casino,
				Hotel,
		};

		public static List<string> DowntownProhibited = new List<string>()
		{
				Apartments,
				Armory,
				Bank,
				Bathhouse,
				Cabin,
				Cave,
				ConfiscationCenter,
				DeportationCenter,
				DrugDen,
				Farm,
				GatedCommunity,
				Greenhouse,
				HedgeMaze,
				Hospital,
				House,
				HouseUptown,
				Lab,
				Mansion,
				MayorHouse,
				MayorOffice,
				MilitaryOutpost,
				OfficeBuilding,
				Pit,
				PodiumPark,
				PoliceOutpost,
				Prison,
				PrivateClub,
				Shack,
				TVStation,
				Zoo,
		};

		public static List<string> DowntownUnlimited = new List<string>()
		{
				Generic,
				None,
		};

		#endregion

		#region Industrial

		public static List<string> IndustrialLimitedTo1 = new List<string>()
		{
				Bank,
				Bathroom,
				FireStation,
				Graveyard,
				Hospital,
				PoliceStation,
				Prison,
				Shop,
				SlaveShop,
		};

		public static List<string> IndustrialLimitedTo2 = new List<string>()
		{
				Armory,
				Bar,
				Casino,
				Hideout,
				"Factory",
				Lab,
		};

		public static List<string> IndustrialLimitedTo3 = new List<string>()
		{
				Apartments,
				DrugDen,
				House,
				OfficeBuilding,
				Shack,
		};

		public static List<string> IndustrialProhibited = new List<string>()
		{
				Arcade,
				Arena,
				Bathhouse,
				Cabin,
				Cave,
				Church,
				CityPark,
				ConfiscationCenter,
				DanceClub,
				DeportationCenter,
				Farm,
				GatedCommunity,
				Greenhouse,
				HedgeMaze,
				Hotel,
				HouseUptown,
				IceRink,
				Mall,
				Mansion,
				MayorHouse,
				MayorOffice,
				MilitaryOutpost,
				MovieTheater,
				MusicHall,
				Pit,
				PodiumPark,
				PoliceOutpost,
				PrivateClub,
				TVStation,
				Zoo,
		};

		public static List<string> IndustrialUnlimited = new List<string>()
		{
				Generic,
				None,
		};

		#endregion

		#region Mayor's Village

		public static List<string> MayorVillageLimitedTo1 = new List<string>()
		{
				Bank,
				Bar,
				Bathhouse,
				// Bathroom, // [sic]; also included in prohibited
				Church,
				DanceClub,
				FireStation,
				Hospital,
				MayorHouse,
				MayorOffice,
				MusicHall,
				Pit,
				PodiumPark,
				PoliceOutpost,
				// PoliceStation, // [sic]; also included in prohibited
				PrivateClub,
				Shop,
				SlaveShop,
				Zoo,
		};

		public static List<string> MayorVillageLimitedTo2 = new List<string>()
		{
				HouseUptown,
		};

		public static List<string> MayorVillageProhibited = new List<string>()
		{
				Apartments,
				Arcade,
				Arena,
				Armory,
				Bathroom,
				Cabin,
				Casino,
				Cave,
				CityPark,
				ConfiscationCenter,
				DeportationCenter,
				DrugDen,
				Farm,
				Graveyard,
				Greenhouse,
				HedgeMaze,
				Hideout,
				Hotel,
				House,
				IceRink,
				Lab,
				Mall,
				Mansion,
				MilitaryOutpost,
				MovieTheater,
				OfficeBuilding,
				PoliceStation,
				Prison,
				Shack,
				TVStation,
		};

		public static List<string> MayorVillageUnlimited = new List<string>()
		{
				GatedCommunity,
				Generic,
				None,
		};

		#endregion

		#region Park

		public static List<string> ParkLimitedTo1 = new List<string>()
		{
				Graveyard,
				HedgeMaze,
				Hideout,
		};

		public static List<string> ParkLimitedTo2 = new List<string>()
		{
				MilitaryOutpost,
		};

		public static List<string> ParkLimitedTo3 = new List<string>()
		{
				Cabin,
				Cave,
				Farm,
				Greenhouse,
		};

		public static List<string> ParkProhibited = new List<string>()
		{
				Arcade,
				Arena,
				Bathhouse,
				Church,
				CityPark,
				ConfiscationCenter,
				DanceClub,
				DeportationCenter,
				FireStation,
				GatedCommunity,
				Hotel,
				HouseUptown,
				IceRink,
				Mall,
				Mansion,
				MayorHouse,
				MayorOffice,
				MovieTheater,
				MusicHall,
				Pit,
				PodiumPark,
				PoliceOutpost,
				PrivateClub,
				TVStation,
				Zoo,
		};

		public static List<string> ParkUnlimited = new List<string>()
		{
				Bathroom,
				Generic,
				Shop,
		};

		#endregion

		#region Police State

		public static List<string> PoliceStateLimitedTo1 = new List<string>()
		{
				ConfiscationCenter,
				DeportationCenter,
				MilitaryOutpost,
				PoliceOutpost,
				PoliceStation,
				Prison,
		};

		public static List<string> PoliceStateLimitedTo2 = new List<string>()
		{
				PoliceOutpost,
				PoliceStation,
		};

		public static List<string> PoliceStateProhibited = new List<string>()
		{
				CityPark,
				DrugDen,
		};

		#endregion

		#region Slums

		public static List<string> SlumsLimitedTo1 = new List<string>()
		{
				Bank,
				Bathroom,
				Graveyard,
				Hideout,
				Hospital,
				PoliceStation,
				Prison,
				Shop,
				SlaveShop
		};

		public static List<string> SlumsLimitedTo2 = new List<string>()
		{
				Armory,
				Bar,
				Casino,
				Lab,
		};

		public static List<string> SlumsLimitedTo3 = new List<string>()
		{
				Apartments,
				DrugDen,
				House,
				OfficeBuilding,
				Shack,
		};

		public static List<string> SlumsProhibited = new List<string>()
		{
				Arcade,
				Arena,
				Bathhouse,
				Cabin,
				Cave,
				Church,
				CityPark,
				ConfiscationCenter,
				DanceClub,
				DeportationCenter,
				Farm,
				FireStation,
				GatedCommunity,
				Greenhouse,
				HedgeMaze,
				Hotel,
				HouseUptown,
				IceRink,
				Mall,
				Mansion,
				MayorHouse,
				MayorOffice,
				MilitaryOutpost,
				MovieTheater,
				MusicHall,
				Pit,
				PodiumPark,
				PoliceOutpost,
				PrivateClub,
				TVStation,
				Zoo,
		};

		public static List<string> SlumsUnlimited = new List<string>()
		{
				Generic,
				None,
		};

		#endregion

		#region Uptown

		public static List<string> UptownLimitedTo1 = new List<string>()
		{
				Bank,
				Bathhouse,
				Church,
				CityPark,
				ConfiscationCenter,
				DeportationCenter,
				FireStation,
				Graveyard,
				//Hideout, // [sic]
				IceRink,
				Mall,
				Mansion,
				Pit,
				PoliceStation,
				Shop,
				SlaveShop,
				TVStation,
				Zoo,
		};

		public static List<string> UptownLimitedTo2 = new List<string>()
		{
				Bathroom,
				GatedCommunity,
				MusicHall,
				PrivateClub,
		};

		public static List<string> UptownLimitedTo3 = new List<string>()
		{
				PoliceOutpost,
		};

		public static List<string> UptownLimitedTo5 = new List<string>()
		{
				HouseUptown,
		};

		public static List<string> UptownProhibited = new List<string>()
		{
				Apartments,
				Arcade,
				Arena,
				Armory,
				Bar,
				Cabin,
				Casino,
				Cave,
				DanceClub,
				DrugDen,
				Farm,
				HedgeMaze,
				Hideout,
				Hotel,
				House,
				MayorHouse,
				MayorOffice,
				MilitaryOutpost,
				MovieTheater,
				OfficeBuilding,
				PodiumPark,
				Prison,
				Shack,
		};

		public static List<string> UptownUnlimited = new List<string>()
		{
				Generic,
				Greenhouse,
				Hospital,
				Lab,
				None,
		};

		#endregion
	}
	public static class vColor // Vanilla Colors
	{
		public const string
				AlienSkin = "AlienSkin",
				Black = "Black",
				BlackSkin = "BlackSkin",
				Blonde = "Blonde",
				Blue = "Blue",
				Brown = "Brown",
				ButlerBotSkin = "ButlerBotSkin",
				CopBotSkin = "CopBotSkin",
				GoldSkin = "GoldSkin",
				GorillaSkin = "GorillaSkin",
				Green = "Green",
				Grey = "Grey",
				LightBlackSkin = "LightBlackSkin",
				MechSkin = "MechSkin",
				MixedSkin = "MixedSkin",
				Null = "",
				Orange = "Orange",
				PaleSkin = "PaleSkin",
				Pink = "Pink",
				PinkSkin = "PinkSkin",
				Purple = "Purple",
				Red = "Red",
				RobotPlayerSkin = "RobotPlayerSkin",
				RobotSkin = "RobotSkin",
				SuperPaleSkin = "SuperPaleSkin",
				WerewolfSkin = "WerewolfSkin",
				White = "White",
				WhiteSkin = "WhiteSkin",
				Yellow = "Yellow",
				ZombieSkin1 = "ZombieSkin1",
				ZombieSkin2 = "ZombieSkin2",
				ZombieSkin3 = "ZombieSkin3",
				ZombieSkin4 = "ZombieSkin4";
	}
	public static class vDirection // Vanilla Directions
	{
		public const string
				East = "E",
				North = "N",
				NorthEast = "NE",
				NorthWest = "NW",
				Null = "",
				South = "S",
				SouthEast = "SE",
				SouthWest = "SW",
				West = "W";
	}
	public static class vExperience // Vanilla Experience Types
	{
		public const string
				Arrest_100 = "ArrestedPoints",
				ArrestInnocent_10 = "ArrestedPointsInnocent",
				BQDowntown_500 = "BigQuestBonusDowntown",
				BQFloor_300 = "BigQuestBonusFloor",
				BQGame_1000 = "BigQuestBonusGame",
				BQIndustrial_500 = "BigQuestBonusIndustrial",
				BQSlums_500 = "BigQuestBonusSlums",
				BQUptown_500 = "BigQuestBonusUptown",
				CompleteMission_300 = "CompleteMission",
				CompleteFindBombs_700 = "CompleteMissionFindBombs",
				CompleteMission_150 = "CompleteMissionReduced",
				Destruction_200 = "Destruction",
				Destruction_1 = "DestructionPoints",
				Destruction_2 = "DestructionPoints2",
				DisarmDetonator_20 = "DisarmDetonatorPoints",
				Electability_100 = "ElectabilityBonus",
				Enslave_30 = "Enslaved",
				FindTreasure_100 = "FindTreasure",
				ExtinguishFire_5 = "FireExtinguishPoints",
				FreePrisoner_20 = "FreedPrisoner",
				FreeSlave_50 = "FreedSlave",
				Hack_20 = "HackPoints",
				KillIndirect_30 = "IndirectlyKill",
				KillIndirectInnocent_10 = "IndirectlyKillInnocent",
				KillIndirectRival_90 = "IndirectlyKillRival",
				Joke_30 = "Joke",
				KillRobot_1000 = "KilledRobot",
				Kill_50 = "KillPoints",
				KillInnocent_10 = "KillPointsInnocent",
				KillRival_150 = "KillPointsRival",
				Knockout_75 = "KnockOutPoints",
				KnockoutInnocent_10 = "KnockOutPointsInnocent",
				KnockoutRival_150 = "KnockOutPointsRival",
				Lockpick_20 = "LockpickPoints",
				ManySleeping_100 = "ManySleeping",
				Massacre_100 = "Massacre",
				LevelNoAnger_100 = "NoAngerLevel",
				NoDamageTaken_100 = "NoDamageTaken",
				NoDestruction_200 = "NoDestruction",
				NoGuns_200 = "NoGuns",
				NoKill_100 = "NoKillBonus",
				LevelNoKill_100 = "NoKillLevel",
				NotAlerted_100 = "NotAlerted",
				OnlyFists_200 = "OnlyFists",
				Pickpocket_15 = "PickpocketPoints",
				PoisonAir_20 = "PoisonAirPoints",
				RemoveSlaveHelmet_20 = "RemoveSlaveHelmetPoints",
				RemoveWindow_20 = "RemoveWindowPoints",
				ShakedownFail_100 = "ShakedownFailPoints",
				Shakedown_100 = "ShakedownPoints",
				Steal_10 = "StealPoints",
				StealNegative_10 = "StealPointsNegative",
				StoleLots_200 = "StoleLots",
				TamperGenerator_20 = "TamperGeneratorPoints",
				TamperLaserEmitter_20 = "TamperLaserEmitterPoints",
				TamperPoliceBox_20 = "TamperPoliceBoxPoints",
				TamperSatelliteDish_20 = "TamperSatelliteDishPoints",
				TimeBonus_100 = "TimeBonus",
				TwoPlayerWinner1_200 = "TwoPlayerWinner1",
				TwoPlayerWinner2_200 = "TwoPlayerWinner2",
				UnlockSafe_20 = "UnlockSafePoints",
				WonElection_100 = "WonElectionPoints";
	}
	public static class vExplosion // Vanilla Explosion Types
	{
		public const string
				Big = "Big",
				Dizzy = "Dizzy",
				EMP = "EMP",
				Molotov = "Firebomb",
				Huge = "Huge",
				NoiseOnly = "Knocker",
				MindControl = "MindControl",
				Normal = "Normal",
				Ooze = "Ooze",
				PowerSap = "PowerSap",
				Ridiculous = "Ridiculous",
				Slime = "Slime",
				Stomp = "Stomp",
				Warp = "Warp",
				Water = "Water";
	}
	public static class vEyesType // Vanilla Eye Types
	{
		public const string
				Cannibal = "EyesCannibal",
				Dead = "EyesDead",
				Narrow = "EyesNarrow",
				Normal = "Eyes",
				Null = "",
				Wide = "EyesWide",
				Zombie = "EyesZombie";
	}
	public static class vFacialHairType // Vanilla Facial Hair
	{
		public const string
				Beard = "Beard",
				CurlyMustache = "MustacheCircus",
				LongMustache = "MustacheRedneck",
				Mustache = "Mustache",
				Null = "None";
	}
	public static class vHairType // Vanilla Hair
	{
		public const string
				Afro = "Afro",
				AlienHead = "AlienHead",
				AssassinMask = "AssassinMask",
				Bald = "Bald",
				Balding = "Balding",
				BangsLong = "BangsLong",
				BangsMedium = "BangsMedium",
				ButlerBotHead = "ButlerBotHead",
				CopBotHead = "CopBotHead",
				Curtains = "Curtains",
				Cutoff = "Cutoff",
				FlatLong = "FlatLong",
				GorillaHead = "GorillaHead",
				HoboBeard = "HoboBeard",
				HologramHead = "HologramHead",
				Hoodie = "Hoodie",
				Leia = "Leia",
				MessyLong = "MessyLong",
				Military = "Military",
				Mohawk = "Mohawk",
				Normal = "Normal",
				NormalHigh = "NormalHigh",
				Null = "",
				Null2 = "None",
				Pompadour = "Pompadour",
				Ponytail = "Ponytail",
				PuffyLong = "PuffyLong",
				PuffyShort = "PuffyShort",
				RobotHead = "RobotHead",
				RobotPlayerHead = "RobotPlayerHead",
				Sidewinder = "Sidewinder",
				SlavemasterMask = "SlavemasterMask",
				Spiky = "Spiky",
				SpikyShort = "SpikyShort",
				Suave = "Suave",
				Wave = "Wave",
				WerewolfHead = "WerewolfHead";
	}
	public static class vItem // Vanilla Items
	{
		public const string
				AccuracyMod = "AccuracyMod",
				AmmoCapacityMod = "AmmoCapacityMod",
				AmmoProcessor = "AmmoProcessor",
				AmmoStealer = "AmmoStealer",
				Antidote = "Antidote",
				ArmorDurabilitySpray = "ArmorDurabilityDoubler",
				Axe = "Axe",
				BaconCheeseburger = "BaconCheeseburger",
				BalletShoes = "BalletShoes",
				Banana = "Banana",
				BananaPeel = "BananaPeel",
				BaseballBat = "BaseballBat",
				Beartrap = "BearTrap",
				BeartrapfromPark = "BearTrapPark",
				Beer = "Beer",
				BFG = "BFG",
				BigBomb = "BigBomb",
				Blindenizer = "Blindenizer",
				BloodBag = "BloodBag",
				Blowtorch = "Blowtorch",
				Blueprints = "Blueprints",
				BodySwapper = "BodySwapper",
				BodyVanisher = "BodyVanisher",
				BombProcessor = "BombMaker",
				BoomBox = "Boombox",
				BooUrn = "BooUrn",
				BraceletofStrength = "BraceletStrength",
				Briefcase = "Briefcase",
				BrokenCourierPackage = "CourierPackageBroken",
				BulletproofVest = "BulletproofVest",
				CardboardBox = "CardboardBox",
				Chainsaw = "Chainsaw",
				ChloroformHankie = "ChloroformHankie",
				CigaretteLighter = "CigaretteLighter",
				Cigarettes = "Cigarettes",
				CircuitBoard = "CircuitBoard",
				Cocktail = "Cocktail",
				Codpiece = "Codpiece",
				Cologne = "Cologne",
				CourierPackage = "CourierPackage",
				CritterUpper = "CritterUpper",
				Crowbar = "Crowbar",
				CubeOfLampey = "CubeOfLampey",
				CyanidePill = "CyanidePill",
				DeliveryApp = "DeliveryApp",
				DizzyGrenade = "GrenadeDizzy",
				DoorDetonator = "DoorDetonator",
				DrinkMixer = "DrinkMixer",
				EarWarpWhistle = "HearingBlocker",
				ElectroPill = "ElectroPill",
				ElectroTetherVest = "BodyguardTether",
				EMPGrenade = "GrenadeEMP",
				Evidence = "Evidence",
				Explodevice = "ExplosiveStimulator",
				FireExtinguisher = "FireExtinguisher",
				FireproofSuit = "FireproofSuit",
				Fireworks = "Fireworks",
				FirstAidKit = "FirstAidKit",
				Fist = "Fist",
				FiveLeafClover = "FiveLeafClover",
				Flamethrower = "Flamethrower",
				FlamingSword = "FlamingSword",
				Flask = "Flask",
				FoodProcessor = "FoodProcessor",
				Forcefield = "ForceField",
				FourLeafClover = "FourLeafClover",
				FreeItemVoucher = "FreeItemVoucher",
				FreezeRay = "FreezeRay",
				FriendPhone = "FriendPhone",
				Fud = "Fud",
				GasMask = "GasMask",
				GhostGibber = "GhostBlaster",
				Giantizer = "Giantizer",
				GrapplingHook = "GrapplingHook",
				Grenade = "Grenade",
				GuidedMissileLauncher = "GuidedMissileLauncher",
				HackingTool = "HackingTool",
				HamSandwich = "HamSandwich",
				HardDrive = "HardDrive",
				HardHat = "HardHat",
				Haterator = "Haterator",
				HiringVoucher = "HiringVoucher",
				HotFud = "HotFud",
				Hypnotizer = "Hypnotizer",
				HypnotizerII = "Hypnotizer2",
				IdentifyWand = "IdentifyWand",
				IncriminatingPhoto = "IncriminatingPhoto",
				ItemTeleporter = "ItemTeleporter",
				Jackhammer = "Jackhammer",
				Key = "Key",
				KeyCard = "KeyCard",
				KillAmmunizer = "KillProfiterAmmo",
				KillerThrower = "KillerThrower",
				KillHealthenizer = "KillProfiterHealth",
				KillProfiter = "KillProfiter",
				Knife = "Knife",
				KnockerGrenade = "GrenadeKnocker",
				KnockerMelee = "KnockerMelee",
				LandMine = "LandMine",
				Laptop = "Laptop",
				LaserBlazer = "LaserBlazer",
				LaserGun = "LaserGun",
				Leafblower = "LeafBlower",
				Lockpick = "Lockpick",
				MacGuffinMuffin = "MacguffinMuffin",
				MachineGun = "MachineGun",
				MagicLamp = "MagicLamp",
				MayorHat = "MayorHat",
				MayorsMansionGuestBadge = "MayorBadge",
				MechKey = "MechTransformItem",
				MeleeDurabilitySpray = "MeleeDurabilityDoubler",
				MemoryMutilator = "MemoryEraser",
				MindReaderDevice = "MindReaderDevice",
				MiniFridge = "MiniFridge",
				MolotovCocktail = "MolotovCocktail",
				Money = "Money",
				MonkeyBarrel = "MonkeyBarrel",
				MoodRing = "MoodRing",
				MusclyPill = "Steroids",
				Necronomicon = "Necronomicon",
				OilContainer = "OilContainer",
				ParalyzerTrap = "ParalyzerTrap",
				Pistol = "Pistol",
				PlasmaSword = "PlasmaSword",
				PoliceBaton = "PoliceBaton",
				PortableSellOMatic = "PortableSellOMatic",
				PossessionStone = "Depossessor",
				PowerDrill = "PowerDrill",
				PropertyDeed = "PropertyDeed",
				QuickEscapeTeleporter = "QuickEscapeTeleporter",
				RagePoison = "RagePoison",
				RateofFireMod = "RateOfFireMod",
				RecordofEvidence = "MayorEvidence",
				RemoteBomb = "RemoteBomb",
				RemoteBombTrigger = "BombTrigger",
				ResearchGun = "ResearchGun",
				ResurrectionShampoo = "ResurrectionShampoo",
				Revolver = "Revolver",
				Rock = "Rock",
				RocketLauncher = "RocketLauncher",
				RubberBulletsMod = "RubberBulletsMod",
				SafeBuster = "SafeBuster",
				SafeCombination = "SafeCombination",
				SafeCrackingTool = "SafeCrackingTool",
				Shotgun = "Shotgun",
				Shovel = "Shovel",
				Shrinker = "Shrinker",
				ShrinkRay = "ShrinkRay",
				Shuriken = "Shuriken",
				SignedBaseball = "SignedBaseball",
				Silencer = "Silencer",
				SixLeafClover = "SixLeafClover",
				SkeletonKey = "SkeletonKey",
				SlaveHelmet = "SlaveHelmet",
				SlaveHelmetRemote = "SlaveHelmetRemote",
				SlaveHelmetRemover = "SlaveHelmetRemover",
				Sledgehammer = "Sledgehammer",
				SniperRifle = "SniperRifle",
				SoldierHelmet = "SoldierHelmet",
				StickyGlove = "StealingGlove",
				StickyMine = "StickyMine",
				Sugar = "Cocaine",
				Sword = "Sword",
				Syringe = "Syringe",
				Taser = "Taser",
				Teleporter = "Teleporter",
				TimeBomb = "TimeBomb",
				Tooth = "Tooth",
				TranquilizerGun = "TranquilizerGun",
				Translator = "Translator",
				TripMine = "TripMine",
				VoodooDoll = "VoodooDoll",
				WalkieTalkie = "WalkieTalkie",
				WallBypasser = "WallBypasser",
				WarpGrenade = "GrenadeWarp",
				WarpZoner = "WarpZoner",
				WaterCannon = "WaterCannon",
				WaterPistol = "WaterPistol",
				Whiskey = "Whiskey",
				Will = "Will",
				WindowCutter = "WindowCutter",
				Wrench = "Wrench";

		public static List<string> alcohol = new List<string>()
		{
				Beer,
				Cocktail,
				Whiskey
		};

		public static List<string> drugs = new List<string>()
		{
				Antidote,
				Cigarettes,
				Sugar,
				CritterUpper,
				CyanidePill,
				ElectroPill,
				Giantizer,
				KillerThrower,
				RagePoison,
				Shrinker,
				MusclyPill,
				Syringe
		};

		public static List<string> nonVegetarian = new List<string>()
		{
				BaconCheeseburger,
				HamSandwich
		};

		public static List<string> vegetarian = new List<string>()
		{
				Beer,
				Banana,
				Cocktail,
				Fud,
				HotFud,
				Whiskey
		};

		public static List<string> blunt = new List<string>()
				{ };

		public static List<string> explosive = new List<string>()
				{ };

		public static List<string> heavy = new List<string>()
		{
				Axe,
				BaseballBat,
				Beartrap,
				BulletproofVest,
				Crowbar,
				FireExtinguisher,
				FireproofSuit,
				Flamethrower,
				GhostGibber,
				LandMine,
				MachineGun,
				Revolver,
				RocketLauncher,
				Shotgun,
				Sledgehammer,
				Wrench
		};

		public static List<string> loud = new List<string>()
		{
				BoomBox,
				DizzyGrenade,
				DoorDetonator,
				EMPGrenade,
				Explodevice,
				FireExtinguisher,
				Fireworks,
				GhostGibber,
				Grenade,
				EarWarpWhistle,
				Leafblower,
				LandMine,
				MachineGun,
				MolotovCocktail,
				Pistol,
				RemoteBomb,
				Revolver,
				RocketLauncher,
				Shotgun,
				TimeBomb,
				WarpGrenade
		};

		public static List<string> piercing = new List<string>()
		{
				Axe,
				Beartrap,
				Grenade,
				Knife,
				LandMine,
				MachineGun,
				Pistol,
				Revolver,
				RocketLauncher,
				Shotgun,
				Shuriken,
				Sword
		};

		public static List<string> tools = new List<string>()
		{
				Crowbar,
				Wrench,
		};
	}
	public static class vLevelFeeling // Vanilla Disasters
	{
		public const string
				BountyOnYourHead = "Bounty",
				FallingBombs = "DropBombs",
				HiddenBombs = "FindBombs",
				KillerRobot = "Killer",
				Lockdown = "Lockdown",
				Ooze = "Ooze",
				RadiationBlasts = "HarmAtIntervals",
				Riot = "Riot",
				ShiftingStatusEffects = "StatusEffectChange",
				WarZone = "WarZone",
				Zombies = "Zombies";
	}
	public static class vLevelTheme // Vanilla Floor Names
	{
		public const string
				Downtown = "Downtown",
				Industrial = "Industrial",
				MayorVillage = "MayorVillage",
				Park = "Park",
				Slums = "Slums",
				Uptown = "Uptown";

		public enum LevelTheme : int
		{
			Slums = 0,
			Industrial = 1,
			Park = 2,
			Downtown = 3,
			Uptown = 4,
			MayorVillage = 5
		}
	}
	public static class vLevelType // Vanilla Level Types
	{
		public const string
				Attract = "Attract",
				HomeBase = "HomeBase",
				Normal = "Normal",
				Null = "",
				Tutorial = "Tutorial";
	}
	public static class vNameType // Vanilla Name Types
	{
		public const string
				Agent = "Agent",
				Dialogue = "Dialogue",
				Description = "Description",
				Interface = "Interface",
				Item = "Item",
				Object = "Object",
				StatusEffect = "StatusEffect",
				Unlock = "Unlock";
	}
	public static class vObject // Vanilla Objects
	{
		public const string
				AirConditioner = "AirConditioner",
				AlarmButton = "AlarmButton",
				Altar = "Altar",
				AmmoDispenser = "AmmoDispenser",
				ArcadeGame = "ArcadeGame",
				ATMMachine = "ATMMachine",
				AugmentationBooth = "AugmentationBooth",
				Barbecue = "Barbecue",
				BarStool = "BarStool",
				Bathtub = "Bathtub",
				Bed = "Bed",
				Boulder = "Boulder",
				BoulderSmall = "BoulderSmall",
				Bush = "Bush",
				CapsuleMachine = "CapsuleMachine",
				Chair = "Chair",
				Chair2 = "Chair2",
				ChestBasic = "ChestBasic",
				CloneMachine = "CloneMachine",
				Computer = "Computer",
				Counter = "Counter",
				Crate = "Crate",
				Desk = "Desk",
				Door = "Door",
				Elevator = "Elevator",
				EventTriggerFloor = "EventTriggerFloor",
				ExplodingBarrel = "ExplodingBarrel",
				FireHydrant = "FireHydrant",
				Fireplace = "Fireplace",
				FireSpewer = "FireSpewer",
				FlameGrate = "FlameGrate",
				FlamingBarrel = "FlamingBarrel",
				GasVent = "GasVent",
				Generator = "Generator",
				Generator2 = "Generator2",
				Gravestone = "Gravestone",
				Jukebox = "Jukebox",
				KillerPlant = "KillerPlant",
				Lamp = "Lamp",
				LaserEmitter = "LaserEmitter",
				LoadoutMachine = "LoadoutMachine",
				Manhole = "Manhole",
				Mine = "Mine",
				MovieScreen = "MovieScreen",
				PawnShopMachine = "PawnShopMachine",
				Plant = "Plant",
				Podium = "Podium",
				PoliceBox = "PoliceBox",
				PoolTable = "PoolTable",
				PowerBox = "PowerBox",
				Refrigerator = "Refrigerator",
				Safe = "Safe",
				SatelliteDish = "SatelliteDish",
				SecurityCam = "SecurityCam",
				Shelf = "Shelf",
				Sign = "Sign",
				SlimeBarrel = "SlimeBarrel",
				SlimePuddle = "SlimePuddle",
				SlotMachine = "SlotMachine",
				Speaker = "Speaker",
				Stove = "Stove",
				SwitchBasic = "SwitchBasic",
				SwitchFloor = "SwitchFloor",
				Table = "Table",
				TableBig = "TableBig",
				Television = "Television",
				Toilet = "Toilet",
				TrashCan = "TrashCan",
				Tree = "Tree",
				Tube = "Tube",
				Turntables = "Turntables",
				Turret = "Turret",
				VendorCart = "VendorCart",
				WaterPump = "WaterPump",
				Well = "Well",
				Window = "Window";
	}
	public static class vOwnerId // Vanilla Magic Number Owner IDs
	{
		public const int
				Unknown_0 = 0,
				Unknown_1 = 99,
				Unknown_2 = 255,
				AngersCops = 888;
	};
	public static class vQuest // Vanilla Quests
	{
		public const string
				Destroy = "Destroy",
				DestroyAll = "DestroyAll",
				DestroyAndRetrieve = "DestroyAndRetrieve",
				FreeSlave = "FreeSlave",
				Kill = "Kill",
				KillAll = "KillAll",
				KillAndRetrieve = "KillAndRetrieve",
				None = "None",
				PrisonBreak = "PrisonBreak",
				Random = "Random",
				Rescue = "Rescue",
				Retrieve = "Retrieve",
				UseAll = "UseAll";
	}
	public static class vQuestStatus // Vanilla Quest Statuses
	{
		public const string
				Accepted = "Accepted",
				Completed = "Completed",
				Done = "Done",
				Failed = "Failed",
				NotAccepted = "NotAccepted",
				Null = "";
	}
	public static class vSecurityType // Vanilla Security Types
	{
		public const string
				ID = "ID",
				Laptop = "Laptop",
				MayorTrigger = "MayorTrigger",
				Normal = "Normal",
				Null = "",
				Weapons = "Weapons";
	}
	public static class vSpecialAbility // Vanilla Special Abilities
	{
		public const string
				Bite = "Bite",
				Camouflage = "Camouflage",
				Cannibalize = "Cannibalize",
				Chaaarge = "Charge",
				ChloroformHankie = "ChloroformHankie",
				CryProfusely = "TutorialAbility",
				Enslave = "Enslave",
				Handcuffs = "Handcuffs",
				Joke = "Joke",
				Laptop = "Laptop",
				LaserGun = "LaserGun",
				MechTransformation = "MechTransform",
				MindControl = "MindControl",
				Possess = "Possess",
				PowerSap = "PowerSap",
				PrimalLunge = "Lunge",
				SharpLunge = "WerewolfLunge",
				StickyGlove = "StealingGlove",
				Stomp = "Stomp",
				Toss = "Toss",
				WaterCannon = "WaterCannon",
				WerewolfTransformation = "WerewolfTransform",
				ZombieSpit = "ZombieSpit";
	}
	public static class vStatusEffect // Vanilla Status Effects
	{
		public const string
				AbovetheLaw = "AboveTheLaw",
				AccuracyBoosted = "Accurate",
				Acid = "Acid",
				AllStatsDecreased = "DecreaseAllStats",
				AlwaysCrit = "AlwaysCrit",
				AmmoProcessor = "AmmoProcessor",
				BadVision = "BadVision",
				BoomBox = "Boombox",
				ChargeLevel1 = "ChargeLevel1",
				ChargeLevel2 = "ChargeLevel2",
				ChargeLevel3 = "ChargeLevel3",
				ChargeLevel4 = "ChargeLevel4",
				Confused = "Confused",
				CopDebt1 = "OweCops1",
				CopDebt2 = "OweCops2",
				CritterUpper = "CritterUpper",
				Cyanide = "Cyanide",
				DeliveringPackage = "DeliverPackage",
				Dizzy = "Dizzy",
				DNAConnection = "ZombieSpirit",
				Drunk = "Drunk",
				Electrocuted = "Electrocuted",
				ElectroTouch = "ElectroTouch",
				Fast = "Fast",
				FeelingGood = "FeelingGood",
				FeelingLucky = "FeelingLucky",
				FeelingUnlucky = "FeelingUnlucky",
				Frozen = "Frozen",
				Giant = "Giant",
				HearingBlocked = "HearingBlocked",
				IgnoreLasers = "IgnoreLasers",
				IncreaseAllStats = "IncreaseAllStats",
				InDebt1 = "InDebt",
				InDebt2 = "InDebt2",
				InDebt3 = "InDebt3",
				Invincible = "Invincible",
				Invisible = "Invisible",
				InvisiblePermanent = "InvisiblePermanent",
				InvisibleTemporary = "InvisibleLimited",
				KillerThrower = "KillerThrower",
				Loud = "Loud",
				MindControlling = "MindControlling",
				NiceSmelling = "NiceSmelling",
				Nicotine = "Nicotine",
				NumbtoPain = "NumbToPain",
				OnFire = "OnFire",
				Paralyzed = "Paralyzed",
				Poisoned = "Poisoned",
				Rage = "Enraged",
				RegenerateHealth = "RegenerateHealth",
				RegenerateHealthFaster = "RegenerateHealthFaster",
				ResistBulletsSmall = "ResistBulletsSmall",
				ResistDamageLarge = "ResistDamageLarge",
				ResistDamageMedium = "ResistDamageMed",
				ResistDamageSmall = "ResistDamageSmall",
				ResistFire = "ResistFire",
				Resurrection = "Resurrection",
				RevenueExtortion = "Revenue",
				Shrunk = "Shrunk",
				SlaveHelmetRemover = "SlaveHelmetRemover",
				Slow = "Slow",
				StableSystem = "BlockDebuffs",
				Strength = "Strength",
				SuperDizzy = "DizzyB",
				Tranquilized = "Tranquilized",
				WallBypasser = "WallBypasser",
				Weak = "Weak",
				Werewolf = "WerewolfEffect",
				Withdrawal = "Withdrawal";
	}
	public static class vSyringeEffect // Vanilla Syringe Effects
	{
		public const string
				Confused = "Confused",
				Fast = "Fast",
				Invincible = "Invincible",
				Invisible = "Invisible",
				Poison = "Poisoned",
				RegenerateHealth = "RegenerateHealth",
				Slow = "Slow",
				Strength = "Strength",
				SulfuricAcid = "Acid",
				Weak = "Weak";
	}
	public static class vTrait // Vanilla Traits
	{
		public const string
				AbovetheLaw = "AboveTheLaw",
				Accurate = "Accurate",
				Addict = "Addict",
				Aftershocked = "StompDamagesAgents",
				AmmoScavenger = "MoreAmmoInDroppedWeapons",
				Antisocial = "NoFollowers",
				ArmyofFive = "ZombieArmy",
				ArtoftheDeal = "ArtOfTheDeal",
				Backstabber = "Backstabber",
				BananaLover = "BananaLover",
				BenevolentOwner = "SlavesDontMutiny",
				BigBang = "BigRemoteBombExplosions",
				BigBullets = "BigBullets",
				BlahdBasher = "HatesBlahds",
				BlasterMaster = "ExplosionsBreakEverything",
				BlasterSurvivor = "ExplosionsDontDamageCauser",
				BlazingLasers = "LaserGunChargesFaster",
				BlendsInNicely = "HardToSeeFromDistance",
				BlockBreaker = "BlocksSometimesHit",
				BlockBullets = "MeleeHoldDeflectsBullets",
				BloodofSteel = "MusicianTakesLessHealth",
				Bloodlust = "Bloodlust",
				BloodyMess = "BloodyMess",
				BodySwapper = "PossessShorterCooldown",
				Bodyguard = "Bodyguard",
				BombBaker = "LowerCostRemoteBombs",
				BottomlessStomach = "BiteFullHealth",
				Bulky = "BigCollider",
				BulletBreaker = "BulletsDestroyOtherBullets",
				BulletSponge = "ResistBulletsTrait",
				BurningBullets = "BulletsCauseFire",
				Butterfingerer = "ChanceToKnockWeapons",
				CameraShy = "InvisibleToCameras",
				CardboardBoxLike = "StandingStillCausesCamouflage",
				Charismatic = "Likeable",
				ChipmunkTeeth = "BiteFaster",
				ClassSolidarity = "DontHitOwnKind",
				ClumsinessForgiven = "NoOwnCheckOnBreak",
				ComputerIlliterate = "NoTechSkill",
				ConfidentinCrowds = "MoreFollowersCauseMoreDamage",
				Confused = "Confused",
				CoolwithCannibals = "CannibalsNeutral",
				CopsDontCare = "CopsDontCare",
				CorruptionCosts = "MustPayCops",
				CovertCrook = "AgentsDontSeePickpocket",
				CrepeCrusher = "HatesCrepes",
				Crooked = "LessArrestXPLoss",
				Crooked2 = "NoArrestXPLoss",
				CyberNuke = "HacksBlowUpObjects",
				DeeperPockets = "PickpocketMoreMoney",
				DestructiveDeaths = "BiggerSlaveHelmetExplosions",
				Diminutive = "Diminutive",
				DisturbingFacialExpressions = "ScareEnemiesEasier",
				Dizzy = "Dizzy",
				DontMakeMeAngry = "MoreDamageWhenHealthLow",
				Drugalug = "IdentifyScience",
				Durabilitacious = "MeleeLastLonger",
				EggshellWalker = "JokesNeverCauseHate",
				Electronic = "Electronic",
				Extortionist = "Shakedowner",
				Extortionist_2 = "Shakedowner2",
				FairGame = "EveryoneHatesZombie",
				FastFood = "CannibalizeFaster",
				FeatureAct = "JokesMoreSuccessful/JokesAlwaysSuccessful", // TODO actually two traits
				FireproofSkin = "FireproofSkin",
				FireproofSkin2 = "FireproofSkin2",
				FleshFeast = "FleshFeast",
				FloatsLikeButterfly = "MeleeMobility",
				FoolproofHacks = "HackingMakesNoSound",
				FriendoftheCommonFolk = "GenericAgentsAligned",
				FriendoftheFamily = "MafiaAligned",
				Frozen = "Frozen",
				GPYesss = "MapFilled",
				GoodVibrations = "BiggerStompRadius",
				Graceful = "DontTriggerFloorHazards",
				Groupies = "BiggerMindControlRadius",
				Harmless = "CantAttack",
				HeartStopper = "EnemiesDieWhenFleeing",
				HomesicknessKiller = "AgentsFollowToNextLevel",
				HonorAmongThieves = "HonorAmongThieves",
				HonorableChallenges = "ChallengeAnyoneToFight",
				ImOuttie = "FastWhenHealthLow",
				ImOuttie_2 = "FastWhenHealthLow2",
				IdeologicalClash = "RandomPeopleSecretHate",
				ImpatientLunge = "FasterLunge",
				IncreasedCritChance = "IncreasedCritChance",
				InfectiousSpirit = "FollowersExtraHealth",
				InfernoAssailant = "FireExtinguishXP",
				InhumanStrength = "ZombiesStronger",
				IntrusionArtist = "ThiefToolsMayNotSubtract",
				JackofExtraTrades = "MoreTraitChoices",
				Juggernaut = "ChargeNoHealthLoss",
				Jugularious = "BloodRestoresHealth",
				KillerThrower = "KillerThrower",
				KillingTime = "WerewolfLastLonger",
				Kneecapper = "ChanceToSlowEnemies",
				KnockbackKing = "CauseBiggerKnockback",
				Knuckley = "StrongFists",
				Knuckley_2 = "StrongFists2",
				LeisurelyRide = "MoreTimeForDeliveries",
				LockandLoad = "ReloadWeaponsNewLevel",
				LonelinessKiller = "StartWithFollowers",
				LongLunge = "LongLunge",
				LongerStatusEffects = "StatusEffectsLonger",
				Loud = "Loud",
				LowCenterofGravity = "ChargeNoTrip",
				LowCostJobs = "LowcostJobs",
				Malodorous = "Unlikeable",
				MasterofDisaster = "DestructionXP",
				MedicalProfessional = "MedicalProfessional",
				ModernWarfarer = "RegenerateHealthWhenLow",
				Moocher = "CanBorrowMoney",
				Mugger = "Mugger",
				Naked = "Naked",
				NearHarmless = "AttacksOneDamage",
				NimbleFingers = "OperateQuickly",
				NoHarminDying = "NoDepossessHealthLoss",
				NoInFighting = "DontHitAligned",
				NoTeleports = "CantTeleport",
				OilLessEssential = "OilRestoresMoreHealth",
				OilReliant = "OilRestoresHealth",
				OntheHouse = "ChanceFreeShopItem",
				Pacifist = "CantUseWeapons",
				PeaBrained = "CantInteract",
				PenetratingBullets = "BulletsPassThroughObjects",
				PoorHandEyeCoordination = "OperateSlowly",
				PossessionsarePeachyKeen = "NoPossessHate",
				PotentialtoNotSuck = "IncreaseStatEvery2Levels",
				PowerLasers = "LaserMorePowerful",
				PowerWalkers = "MindControlledWalkThroughWalls",
				PricklySkin = "AttacksDamageAttacker",
				PromiseIllReturnIt = "NoStealPenalty",
				PuppetPower = "MindControlledDamageMore",
				QuickandDead = "ZombiesFaster",
				QuickTranq = "TranqDartsWorkFaster",
				Rampager = "BuffFromMultipleKills",
				RandomReverence = "RandomPeopleAligned",
				Rechargeable = "Rechargeable",
				ResearchGunRadiation = "ResearchGunStatusEffects",
				ResistBullets = "ResistBullets",
				ResistBulletsLarge = "ResistDamageLarge",
				ResistBulletsMedium = "ResistDamageMed",
				ResistBulletsSmall = "ResistBulletsSmall",
				ResistDamageSmall = "ResistDamageSmall",
				ResistFire = "ResistFire",
				ResistGas = "ResistGas",
				RestlessBeast = "WerewolfShorterCooldown",
				RiotCannon = "StrongerWaterCannon",
				RipandTear = "HigherWerewolfDamage",
				RollerSkates = "RollerSkates",
				RubberBullets = "BulletKnockouts",
				SafeinCrowds = "MoreFollowersLessDamageToPlayer",
				SafeStomp = "StompLessDamage",
				SapDamage = "MoreDamagePowerSap",
				SappyHealthy = "MoreHealthFromPowerSap",
				SausageFingers = "CantUseWeapons2",
				Savorer = "CannibalizeMoreHealth",
				ScientistSlayer = "HatesScientist",
				ScorchingSavior = "FightsFires",
				ScumbagSlaughterer = "MechHateTrait",
				SecretVandalizer = "HitObjectsNoNoise",
				ServeDrinks = "ServeDrinks",
				SharetheHealth = "HealthItemsGiveFollowersExtraHealth",
				ShopDrops = "VendorsDropShopItem",
				SkinnyNerdlinger = "KnockbackMore",
				SleepKiller = "SleepKiller",
				Slinky = "BumpsDontEndCamouflage",
				SlipperyTarget = "HardToShoot",
				SneakyBomber = "NPCsDontNoticeRemoteBombs",
				SneakyFingers = "OperateSecretly",
				Specist = "HatesGorilla",
				SpeedCoder = "HackImmediate",
				Sprinter = "FasterCharge",
				SteadfastSlaves = "SlavesFightForYou",
				SteadyHead = "WerewolfNoDizzy",
				StrictCannibal = "CannibalizeRestoresHealth",
				StubbyFingers = "CantUseGuns",
				Studious = "MoreSkillPoints",
				SuperStudious = "MoreSkillPoints2",
				SubduingSpree = "NoChloroformCooldown",
				Sucker = "BadTrader",
				SuperDizzy = "DizzyB",
				Surging = "MoreMoneyFromDeliveries",
				Suspicious = "Suspicious",
				SwiftSpitter = "FasterZombieSpitCharge",
				SwiftWolf = "HigherWerewolfSpeed",
				TableManners = "CannibalizeNoAnnoy",
				Tackler = "ChargeMorePowerful",
				TeamBuildingExpert = "MoreFollowers",
				TechExpert = "TechExpert",
				TeleportHappy = "TeleportAnytime",
				TheLaw = "TheLaw",
				TheyreJustKissing = "BiteNoAnnoy",
				ThickSkinnedPawn = "MindControlledResistDamage",
				ThroatofIron = "NoZombieSpitHealthLoss",
				TrustFunder = "MoneyAtLevelStart",
				UltimateButterfingerer = "KnockWeapons",
				UnCrits = "ChanceAttacksDoZeroDamage",
				UnCrits_2 = "ChanceAttacksDoZeroDamage2",
				UpperCrusty = "UpperCrusty",
				VeinTapper = "BiteGainMoreHealth",
				ViciousChameleon = "FailedAttacksDontEndCamouflage",
				VocallyChallenged = "CantSpeakEnglish",
				WallWalloper = "MeleeDestroysWalls",
				WallsWorstNightmare = "MoreKnockingThroughWalls",
				Wanted = "Naked",
				WerewolfAWereness = "WerewolfAwareness",
				WrongBuilding = "OwnersNotHostile",
				Zombiism = "Zombify";

		public static List<string> EnhanceDrugs = new List<string>()
		{
				Addict,
		};

		public static List<string> EnhanceFood = new List<string>()
		{
				BananaLover,
		};

		public static List<string> ExperienceRate = new List<string>()
		{
				Studious,
				SuperStudious,
		};

		public static List<string> LimitDrugs = new List<string>()
		{
				OilReliant
		};

		public static List<string> LimitFood = new List<string>()
		{
				Electronic,
				Jugularious,
				OilReliant,
				StrictCannibal,
		};

		public static List<string> LimitWeapons = new List<string>()
		{
				NearHarmless,
				Pacifist,
				StubbyFingers,
				SausageFingers
		};

		public static List<string> RelationshipInitialGeneral = new List<string>()
		{
				Charismatic,
				RandomReverence,
		};

		public static List<string> RelationshipInitialSpecific = new List<string>()
		{
				BlahdBasher,
				CoolwithCannibals,
				ClassSolidarity,
				CrepeCrusher,
				FriendoftheCommonFolk,
				FriendoftheFamily,
				ScientistSlayer,
				Specist,
				TheLaw,
		};
	}
}