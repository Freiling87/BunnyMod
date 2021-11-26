using BunnyMod.Extensions;
using BunnyMod.ObjectBehaviour.Controllers.Data;
using Google2u;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.ObjectBehaviour.Controllers
{
	public class ElevatorController : IObjectController<Elevator>
	{
		private const string ElevatorPurchaseTicket_ButtonText = "ElevatorPurchaseTicket";
		private const int ElevatorPurchaseTicket_ButtonPrice = 50;
		
		private static readonly ObjectDataAccessor<ElevatorData> dataAccessor = new ObjectDataAccessor<ElevatorData>();

		[RLSetup, UsedImplicitly]
		private static void Initialize()
		{
			ElevatorController controller = new ElevatorController();
			ObjectControllerManager.RegisterObjectController(controller);
			ObjectControllerManager.RegisterDataController<Elevator>(dataAccessor);
		}

		private static void HandlePurchaseTicket(Elevator elevator, int price)
		{
			GameController gc = GameController.gameController;
			Agent agent = elevator.interactingAgent;
			if (elevator.moneySuccess(price))
			{
				dataAccessor.GetObjectData(elevator).isTicketPurchased = true;
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
				if (!dataAccessor.GetObjectData(elevator).isTicketPurchased)
				{
					// One simply does not use the elevator before paying the fee
					elevator.RemoveButton(nameof(InterfaceNameDB.rowIds.ElevatorGoUp));
				}
			}
		}

		public void HandleRevertAllVars(Elevator objectInstance) { }
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
				if (!dataAccessor.GetObjectData(objectInstance).isTicketPurchased)
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
		public void HandleObjectAction(Elevator objectInstance, string action, ref bool noMoreObjectActions, string extraString, float extraFloat, Agent causerAgent, PlayfieldObject extraObject) { }
		public void HandleDamagedObject(Elevator objectInstance, PlayfieldObject damagerObject, float damageAmount) { }
		public void HandleMakeNonFunctional(Elevator objectInstance, PlayfieldObject damagerObject) { }
		public void HandleDestroyMe(Elevator objectInstance, PlayfieldObject damagerObject) { }
		public void HandleDestroyMe3(Elevator objectInstance) { }
	}
}