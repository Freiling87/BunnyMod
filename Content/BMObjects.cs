using System;
using Light2D;
using RogueLibsCore;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BunnyMod
{
	public class BMObjects
	{
		private static GameController GC => GameController.gameController;
		private static void BMLog(string logMessage) => BMHeader.Log(logMessage);

		#region Main

		public static void Awake()
		{
			Initialize_Names();

			SpawnerMain_00();
		}

		private static void Initialize_Names()
		{
			string t = vNameType.Dialogue;
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

		#region Base Classes & Spawners

		#region SpawnerMain

		private static void SpawnerMain_00()
		{
			Type t = typeof(SpawnerMain);
			Type g = typeof(BMObjects);

			
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
	}
}