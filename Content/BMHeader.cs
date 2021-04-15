using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;
using System.Threading.Tasks;
using BunnyMod.Content;

namespace BunnyMod
{
	[BepInPlugin(pluginGuid, pluginName, pluginVersion)]
	[BepInProcess("StreetsOfRogue.exe")]
	[BepInDependency(RogueLibs.pluginGuid, "2.0")]

	public class BMHeader : BaseUnityPlugin
	{
		public const string pluginGuid = "freiling87.streetsofrogue.bunnymod";
		public const string pluginName = "Bunny Mod";
		public const string pluginVersion = "1.4.0";

		public static ManualLogSource ConsoleMessage;
		public static GameController GC => GameController.gameController;
		public static BaseUnityPlugin MainInstance;

		public void Awake()
		{
			MainInstance = this;
			ConsoleMessage = Logger;

			new BMAbilities().Awake();
			//new BunnyBehaviors().Awake();
			//new BMDisasters().Awake();
			//new BunnyItems().Awake();
			new BMLevelGen().Awake();
			new BMMutators().Awake();
			new BMObjects().Awake();
			//new BunnySprites().Awake();
			new BMTraits().Awake();
		}
		#region Category Lists
		public static List<string> alcohol = new List<string>() { "Beer", "Cocktail", "Whiskey" };
		public static List<string> drugs = new List<string>() { "Antidote", "Cigarettes", "Cocaine", "CritterUpper", "CyanidePill", "ElectroPill", "Giantizer", "KillerThrower", "RagePoison", "Shrinker", "Steroids", "Syringe" };
		public static List<string> nonVegetarian = new List<string>() { "BaconCheeseburger", "HamSandwich" };
		public static List<string> vegetarian = new List<string>() { "Banana", "Fud", "HotFud" };

		public static List<string> blunt = new List<string>() { };
		public static List<string> explosive = new List<string>() { };
		public static List<string> heavy = new List<string>() { "Axe", "BaseballBat", "BearTrap", "BombTrigger", "BulletproofVest", "FireExtinguisher", "FireproofSuit", "Flamethrower", "GhostBlaster", "LandMine", "MachineGun", "Revolver", "RocketLauncher", "Shotgun", "Sledgehammer", "Wrench" };
		public static List<string> loud = new List<string>() { "BoomBox", "DizzyGrenade", "DoorDetonator", "EMPGrenade", "ExplosiveStimulator", "FireExtinguisher", "Fireworks", "GhostBlaster", "Grenade", "HearingBlocker", "Leafblower", "LandMine", "MachineGun", "MolotovCocktail", "Pistol", "RemoteBomb", "Revolver", "RocketLauncher", "Shotgun", "TimeBomb", "WarpGrenade" };
		public static List<string> piercing = new List<string>() { "Axe", "BearTrap", "Grenade", "Knife", "LandMine", "MachineGun", "Pistol", "Revolver", "RocketLauncher", "Shotgun", "Shuriken", "Sword" };
		public static List<string> tools = new List<string> { "Crowbar", "Wrench" };
		#endregion
		public static void Log (string logMessage) =>
			ConsoleMessage.LogMessage(logMessage);
	}

	public static class BunnyHeaderTools
	{
		public static T GetMethodWithoutOverrides<T>(this MethodInfo method, object callFrom)
			where T : Delegate
		{
			IntPtr ptr = method.MethodHandle.GetFunctionPointer();
			return (T)Activator.CreateInstance(typeof(T), callFrom, ptr);
		}
		public static void InvokeRepeating(object instance, string method, float delay, float interval)
		{
			MethodInfo methodAccessed = AccessTools.Method(instance.GetType(), method);
			Task task = InvokeRepeating2(instance, methodAccessed, (int)Mathf.Floor(delay * 1000), (int)Mathf.Floor(interval * 1000));
		}
		private static async Task InvokeRepeating2(object instance, MethodInfo method, int delay, int interval)
		{
			await Task.Delay(delay);

			while (true)
			{
				method.Invoke(instance, new object[0]);
				await Task.Delay(interval);
			}
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
			};
		}
	}


    public enum DamageType
    {
        burnedFingers,
        brokenWindow
    }
    public static class cAbilities // Custom Abilities
	{
        public const string
            ChronomanticDilation = "ChronomanticDilation",
            PyromanticJet = "PyromanticJet",
            TelemanticBlink = "TelemanticBlink";
    }
    public static class cMutators // Custom Mutators
	{
        public const string
            CityOfSteel = "CityOfSteel",
            Claustrophobia = "Claustrophobia",
            Megalopolis = "Megalopolis",
            ScaryGuns = "ScaryGuns",
            ShantyTown = "ShantyTown",
            SingleMinded = "SingleMinded",
            ACityForAnts = "ACityForAnts",
            Ultrapolis = "Ultrapolis",
            Workhorse = "Workhorse",
            ZeroQuests = "ZeroQuests";
	}
	public static class cTraits // Custom Traits
	{
		public const string
			AfraidOfLoudNoises = "AfraidOfLoudNoises",
			Archmage = "Archmage",
			Carnivore = "Carnivore",
			Charmed = "Charmed",
			Charmed_2 = "Charmed_2",
			Cursed = "Cursed",
			Cursed_2 = "Cursed_2",
			DAREdevil = "DAREdevil",
			Domineering = "Domineering",
			Domineering_2 = "Domineering_2",
			DrawNoBlood = "DrawNoBlood",
			EagleEyes = "EagleEyes",
			EagleEyes_2 = "EagleEyes_2",
			Fatass = "Fatass",
			FatHead = "FatHead",
			FocusedCasting = "FocusedCasting",
			FocusedCasting_2 = "FocusedCasting_2",
			FriendOfBill = "FriendOfBill",
			GenerallyUnpleasant = "GenerallyUnpleasant",
			GenerallyUnpleasant_2 = "GenerallyUnpleasant_2",
			Haunted = "Haunted",
			MagicTraining = "MagicTraining",
			MagicTraining_2 = "MagicTraining_2",
			ManaBattery = "ManaBattery",
			ManaBattery_2 = "ManaBattery_2",
			MobDebt = "MobDebt",
			MobDebt_2 = "MobDebt_2",
			MookMasher = "MookMasher",
			Myopic = "Myopic",
			Myopic2 = "Myopic_2",
			Polarizing = "Polarizing",
			Polarizing_2 = "Polarizing_2",
			Priors = "Priors",
			RATS = "RATS",
			RATS_2 = "RATS_2",
			Reinforcements = "Reinforcements",
			Reinforcements_2 = "Reinforcements_2",
			StealthBastardDeluxe = "StealthBastardDeluxe",
			TamperTantrum = "TamperTantrum",
			TamperTantrum_2 = "TamperTantrum_2",
			Teetotaller = "Teetotaller",
			UnderdarkCitizen = "UnderdarkCitizen",
			Vegetarian = "Vegetarian",
			VeiledThreats = "VeiledThreats",
			Warlord = "Warlord",
			WildCasting = "WildCasting",
			WildCasting_2 = "WildCasting_2";
	}
	public static class vTraits // Vanilla Traits
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
            FairGame = "EveryoneHatesZombie",
            FastFood = "CannibalizeFaster",
            FeatureAct = "JokesMoreSuccessful/JokesAlwaysSuccessful",
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
            Studious2 = "MoreSkillPoints",
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
            UpperCrusty = "UpperCrusty",
            VeinTapper = "BiteGainMoreHealth",
            ViciousChameleon = "FailedAttacksDontEndCamouflage",
            VocallyChallenged = "CantSpeakEnglish",
            WallWalloper = "MeleeDestroysWalls",
            WallsWorstNightmare = "MoreKnockingThroughWalls",
            Wanted = "Wanted",
            WerewolfAWereness = "WerewolfAwareness",
            WrongBuilding = "OwnersNotHostile",
            Zombiism = "Zombify";
    }

}
