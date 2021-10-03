using System.Collections.Generic;
using BunnyMod.Content.Extensions;
using Google2u;

namespace BunnyMod.Content.ObjectBehaviour
{
	public class ElevatorController : ObjectControllerInterface<Elevator>
	{
		private const string ElevatorPurchaseTicket_ButtonText = "ElevatorPurchaseTicket";
		private const int ElevatorPurchaseTicket_ButtonPrice = 50;

		private static readonly HashSet<Elevator> elevatorTicketsPurchased = new HashSet<Elevator>();

		static ElevatorController()
		{
			ObjectControllerManager.ChangeLevelEvent += OnChangeLevel;
		}

		private static void OnChangeLevel()
		{
			elevatorTicketsPurchased.Clear();
		}

		private static void HandlePurchaseTicket(Elevator elevator, int price)
		{
			GameController gc = GameController.gameController;
			Agent agent = elevator.interactingAgent;
			if (elevator.moneySuccess(price))
			{
				elevatorTicketsPurchased.Add(elevator);
				gc.audioHandler.Play(agent, vAudioClip.ATMDeposit);
				BMHeaderTools.SayDialogue(elevator, cDialogue.PurchaseElevator, vNameType.Dialogue);
			}
			else
			{
				BMHeaderTools.SayDialogue(elevator, cDialogue.CantAffordElevator, vNameType.Dialogue);
				elevator.StopInteraction();
			}
		}

		// this method is called after Elevator::DetermineButtons is called
		public static void Elevator_DetermineButtons_Postfix(Elevator elevator)
		{
			GameController gc = GameController.gameController;
			if (gc.challenges.Contains(cChallenge.AnCapistan))
			{
				if (!elevatorTicketsPurchased.Contains(elevator))
				{
					// One simply does not use the elevator before paying the fee
					elevator.RemoveButton(nameof(InterfaceNameDB.rowIds.ElevatorGoUp));
				}
			}
		}

		public void HandleObjectUpdate(Elevator objectInstance) { }
		public void HandlePlayerHasUsableItem(Elevator objectInstance, InvItem itemToTest, ref bool result) { }

		public void HandlePressedButton(Elevator objectInstance, string buttonText, int buttonPrice)
		{
			if (buttonText == ElevatorPurchaseTicket_ButtonText)
			{
				HandlePurchaseTicket(objectInstance, buttonPrice);
			}
		}

		public void HandleDetermineButtons(Elevator objectInstance)
		{
			GameController gc = GameController.gameController;
			if (gc.challenges.Contains(cChallenge.AnCapistan))
			{
				if (!elevatorTicketsPurchased.Contains(objectInstance))
				{
					objectInstance.AddButton(
							text: ElevatorPurchaseTicket_ButtonText,
							price: ElevatorPurchaseTicket_ButtonPrice
					);
				}
			}
		}

		public void HandleFinishedOperating(Elevator objectInstance) { }
		public void HandleInteract(Elevator objectInstance, Agent agent) { }
		public void HandleObjectAction(Elevator objectInstance, string action, ref bool noMoreObjectActions) { }
		public void HandleDamagedObject(Elevator objectInstance, PlayfieldObject damagerObject, float damageAmount) { }
		public void HandleMakeNonFunctional(Elevator objectInstance, PlayfieldObject damagerObject) { }
		public void HandleDestroyMe(Elevator objectInstance, PlayfieldObject damagerObject) { }
		public void HandleDestroyMe3(Elevator objectInstance) { }
	}
}