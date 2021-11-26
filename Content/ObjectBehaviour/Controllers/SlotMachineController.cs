using System;
using System.Collections;
using BepInEx.Logging;
using BunnyMod.Extensions;
using BunnyMod.Logging;
using BunnyMod.ObjectBehaviour.Controllers.Data;
using Google2u;
using JetBrains.Annotations;
using RogueLibsCore;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BunnyMod.ObjectBehaviour.Controllers
{
	public class SlotMachineController : IObjectController<SlotMachine>
	{
		private const string SlotMachineHackJackpot_ButtonText = "SlotMachineHackJackpot";
		private const int SlotMachineHackJackpot_Payout = 10;

		private const float JackpotEventDuration = 10f;
		private const float JackpotNoiseInterval = 1f;
		private const int JackpotNumNoises = 10;
		private const int JackpotTargetNumCashDrops = 50;

		private static readonly string[] JackpotDialogue =
		{
				cDialogue.SlotMachineJackpot_1,
				cDialogue.SlotMachineJackpot_2,
				cDialogue.SlotMachineJackpot_3,
				cDialogue.SlotMachineJackpot_4,
				cDialogue.SlotMachineJackpot_5
		};

		private const string Play1_ButtonText = "Play1";
		private const string Play100_ButtonText = "Play100";

		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		private static readonly ObjectDataAccessor<SlotMachineData> dataAccessor = new ObjectDataAccessor<SlotMachineData>();

		[RLSetup, UsedImplicitly]
		private static void Initialize()
		{
			SlotMachineController controller = new SlotMachineController();
			ObjectControllerManager.RegisterObjectController(controller);
			ObjectControllerManager.RegisterDataController<SlotMachine>(dataAccessor);
		}

		private static void TriggerJackpot(SlotMachine slotMachine, int payout)
		{
			SlotMachineData slotMachineData = dataAccessor.GetObjectData(slotMachine);
			slotMachine.interactable = false;
			slotMachine.StartCoroutine(JackpotPlayEvent(slotMachine, slotMachineData, payout));
		}

		private static IEnumerator JackpotPlayEvent(SlotMachine slotMachine, SlotMachineData slotMachineData, int payout)
		{
			GameController gc = GameController.gameController;
			Agent interactingAgent = slotMachine.interactingAgent;

			float noiseVolume = slotMachineData.isHacked ? 5f : 2f;
			int jackpotDialogueLength = JackpotDialogue.Length;
			float dialogueInterval = JackpotEventDuration / jackpotDialogueLength;
			int cashDropAmount;
			int lastCashDropAmount; // spit out all the money that couldn't be split evenly on the last drop
			int cashDropCount;
			if (payout < JackpotTargetNumCashDrops)
			{
				cashDropAmount = 1;
				lastCashDropAmount = 1;
				cashDropCount = payout;
			}
			else
			{
				int avgDrop = payout / JackpotTargetNumCashDrops;
				cashDropAmount = (payout - avgDrop) / (JackpotTargetNumCashDrops - 1);
				lastCashDropAmount = payout - (cashDropAmount * (JackpotTargetNumCashDrops - 1));
				cashDropCount = JackpotTargetNumCashDrops;
			}
			float cashDropInterval = JackpotEventDuration / cashDropCount;

			float nextNoiseTimeRemaining = 0;
			int noiseSpawnsRemaining = JackpotNumNoises;
			float nextDialogueTimeRemaining = dialogueInterval;
			int dialogueRemaining = jackpotDialogueLength;
			float nextCashDropTimeRemaining = cashDropInterval;
			int cashDropsRemaining = cashDropCount;

			while (noiseSpawnsRemaining > 0 || dialogueRemaining > 0 || cashDropsRemaining > 0)
			{
				if (!slotMachine.functional || slotMachine.destroyed)
				{
					break;
				}

				if (nextNoiseTimeRemaining <= 0 && noiseSpawnsRemaining > 0)
				{
					JackpotMakeNoise(slotMachine, noiseVolume, slotMachineData.isHacked, interactingAgent);
					nextNoiseTimeRemaining = JackpotNoiseInterval;
					noiseSpawnsRemaining--;
				}
				if (nextDialogueTimeRemaining <= 0 && dialogueRemaining > 0)
				{
					BMHeaderTools.SayDialogue(slotMachine, JackpotDialogue[jackpotDialogueLength - dialogueRemaining], vNameType.Dialogue);
					nextDialogueTimeRemaining = dialogueInterval;
					dialogueRemaining--;
				}
				if (nextCashDropTimeRemaining <= 0 && cashDropsRemaining > 0)
				{
					slotMachine.PlayAnim(vAnimation.MachineOperate, interactingAgent);
					JackpotSpitOutMoney(slotMachine, cashDropsRemaining == 1 ? lastCashDropAmount : cashDropAmount);
					gc.audioHandler.Play(slotMachine, vAudioClip.Win);
					nextCashDropTimeRemaining = cashDropInterval;
					cashDropsRemaining--;
				}

				float nextEventTimeRemaining = Math.Min(Math.Min(nextNoiseTimeRemaining, nextDialogueTimeRemaining), nextCashDropTimeRemaining);
				nextNoiseTimeRemaining -= nextEventTimeRemaining;
				nextDialogueTimeRemaining -= nextEventTimeRemaining;
				nextCashDropTimeRemaining -= nextEventTimeRemaining;
				yield return new WaitForSeconds(nextEventTimeRemaining);
			}

			OnJackpotEventOver(slotMachine, slotMachineData);
		}

		private static void JackpotMakeNoise(PlayfieldObject slotMachine, float volume, bool wasHacked, Agent causerAgent)
		{
			GameController gc = GameController.gameController;
			gc.audioHandler.Play(slotMachine, vAudioClip.Boombox1);
			gc.spawnerMain.SpawnStateIndicator(slotMachine, "MusicNotes");
			Vector3 noisePos = slotMachine.tr.position;
			gc.spawnerMain.SpawnNoise(noisePos, volume, slotMachine, "Attract");
			if (wasHacked)
			{
				gc.spawnerMain.SpawnNoise(noisePos, 3f, slotMachine, "Attract", causerAgent, false).distraction = true;
			}
		}

		private static void JackpotSpitOutMoney(ObjectReal slotMachine, int amount)
		{
			if (GameController.gameController.serverPlayer)
			{
				InvItem cashItem = new InvItem { invItemName = nameof(ItemNameDB.rowIds.Money) };
				cashItem.ItemSetup(false);
				cashItem.invItemCount = amount;

				float randA = Random.Range(-0.16f, 0.16f);
				float randB = Random.Range(0.16f, 0.32f);
				Vector3 position = slotMachine.tr.position;
				switch (slotMachine.direction)
				{
					case "N":
						position = new Vector3(position.x + randA, position.y + randB, position.z);
						break;
					case "E":
						position = new Vector3(position.x + randB, position.y + randA, position.z);
						break;
					case "S":
						position = new Vector3(position.x + randA, position.y - randB, position.z);
						break;
					case "W":
						position = new Vector3(position.x - randB, position.y + randA, position.z);
						break;
				}
				GameController.gameController.spawnerMain.SpillItem(position, cashItem);
			}
			else
			{
				slotMachine.interactingAgent.objectMult.ObjectAction(slotMachine.objectNetID, nameof(JackpotMakeNoise), amount);
			}
		}

		private static void OnJackpotEventOver(SlotMachine slotMachine, SlotMachineData slotMachineData)
		{
			if (slotMachine.functional && !slotMachine.destroyed)
			{
				if (slotMachine.stateIndicator != null)
				{
					slotMachine.stateIndicator.StateIndicatorOff();
				}
				GameController.gameController.audioHandler.StopObjectSounds(slotMachine);
				if (GameController.gameController.serverPlayer)
				{
					GameController.gameController.playerAgent.objectMult.ObjectAction(slotMachine.objectNetID, "StopMusicClient");
				}
			}

			slotMachineData.isPlayingSound = false;
			if (!GameController.gameController.challenges.Contains(vChallenge.NoLimits))
			{
				slotMachine.MakeAllMachinesNonFunctional();
				// TODO: Have proprietor come and cut you off from playing here, but give you a free cocktail.
			}
		}

		/// <summary>
		/// Handles custom gambling logic
		/// </summary>
		/// <param name="slotMachine">instance of the slot machine</param>
		/// <param name="gambleAmount">cash amount to gamble with</param>
		/// <returns>true if gambling event was handled by the controller</returns>
		public static bool HandleGamble(SlotMachine slotMachine, int gambleAmount)
		{
			// TODO turn this into a mutator
			{
				if (!slotMachine.moneySuccess(gambleAmount))
				{
					slotMachine.StopInteraction();
					return true;
				}
				slotMachine.IncreaseNumPlays();
				slotMachine.objectInvDatabase.money.invItemCount += gambleAmount;
				int advantage = slotMachine.interactingAgent.DetermineLuck(slotMachine.advantage, "SlotMachine", true);
				int result = 1000 - Mathf.Clamp(Random.Range(1, 1000) + advantage, 1, 1000);

				if (result == 0 || slotMachine.interactingAgent.HasTrait<Debug>())
				{
					logger.LogDebug($"\tGambling result: {result} -> Jackpot!");
					TriggerJackpot(slotMachine, gambleAmount * 64);
				}
				else if (result > 128)
				{
					logger.LogDebug($"\tGambling result: {result} -> No Payout");
					BMHeaderTools.SayDialogue(slotMachine.interactingAgent, "SlotMachineLost", vNameType.Dialogue);
					GameController.gameController.audioHandler.Play(slotMachine, "Fail");
				}
				else
				{
					int payoutMult = 2;
					if (result <= 8) payoutMult = 16;
					else if (result <= 32) payoutMult = 8;
					else if (result <= 64) payoutMult = 4;
					int payout = gambleAmount * payoutMult;
					logger.LogDebug($"\tGambling result: {result} | payoutMult: {payoutMult} | payout: {payout}");

					slotMachine.interactingAgent.inventory.AddItem("Money", payout);
					slotMachine.objectInvDatabase.SubtractFromItemCount(slotMachine.objectInvDatabase.money, payout);
					BMHeaderTools.SayDialogue(slotMachine.interactingAgent, "SlotMachineWon", vNameType.Dialogue);
					slotMachine.PlayAnim("MachineOperate", slotMachine.interactingAgent);
					for (; payoutMult > 0; payoutMult--)
					{
						GameController.gameController.audioHandler.Play(slotMachine, "Win");
					}
				}

				slotMachine.StopInteraction();
				return true;
			}
		}

		public void HandleRevertAllVars(SlotMachine objectInstance) { }

		public void HandleObjectUpdate(SlotMachine objectInstance) { }

		public void HandlePlayerHasUsableItem(SlotMachine objectInstance, InvItem itemToTest, ref bool result) { }

		public void HandlePressedButton(SlotMachine objectInstance, string buttonText, int buttonPrice)
		{
			switch (buttonText)
			{
				case Play1_ButtonText:
					objectInstance.Gamble(1);
					return;
				case Play100_ButtonText:
					objectInstance.Gamble(100);
					return;
				case SlotMachineHackJackpot_ButtonText:
					dataAccessor.GetObjectData(objectInstance).isHacked = true;
					TriggerJackpot(objectInstance, SlotMachineHackJackpot_Payout);
					return;
			}

			// TODO make sure that we don't mind the vanilla code calling "StopInteraction" here.
		}

		public void HandleDetermineButtons(SlotMachine objectInstance)
		{
			if (objectInstance.interactingAgent.interactionHelper.interactingFar)
			{
				objectInstance.AddButton(SlotMachineHackJackpot_ButtonText);
			}
			else
			{
				objectInstance.AddButton(Play1_ButtonText, price: 1);
				objectInstance.AddButton(Play100_ButtonText, price: 100);
			}
		}

		public void HandleFinishedOperating(SlotMachine objectInstance) { }

		public static bool HandleInteractFar_Prefix(SlotMachine slotMachine, Agent agent)
		{
			if (dataAccessor.GetObjectData(slotMachine).isPlayingSound)
			{
				BMHeaderTools.SayDialogue(agent, cDialogue.MachineBusy, vNameType.Dialogue);
				GameController.gameController.audioHandler.Play(slotMachine, vAudioClip.CantDo);
				return false;
			}
			return true;
		}

		public static bool HandleInteract_Prefix(SlotMachine slotMachine, Agent agent)
		{
			if (dataAccessor.GetObjectData(slotMachine).isPlayingSound)
			{
				BMHeaderTools.SayDialogue(agent, cDialogue.MachineBusy, vNameType.Dialogue);
				GameController.gameController.audioHandler.Play(slotMachine, vAudioClip.CantDo);
				return false;
			}
			return true;
		}

		public void HandleInteract(SlotMachine objectInstance, Agent agent) { }

		public void HandleObjectAction(SlotMachine objectInstance, string action, ref bool noMoreObjectActions, string extraString, float extraFloat, Agent causerAgent, PlayfieldObject extraObject)
		{
			if (action == nameof(JackpotSpitOutMoney))
			{
				JackpotSpitOutMoney(objectInstance, (int) extraFloat);
			}
		}

		public void HandleDamagedObject(SlotMachine objectInstance, PlayfieldObject damagerObject, float damageAmount) { }

		public void HandleMakeNonFunctional(SlotMachine objectInstance, PlayfieldObject damagerObject) { }

		public void HandleDestroyMe(SlotMachine objectInstance, PlayfieldObject damagerObject) { }

		public void HandleDestroyMe3(SlotMachine objectInstance) { }
	}
}