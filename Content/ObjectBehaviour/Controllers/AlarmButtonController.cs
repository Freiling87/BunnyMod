using BunnyMod.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.ObjectBehaviour.Controllers
{
	public class AlarmButtonController : IObjectController<AlarmButton>
	{
		private const string AlarmButtonAncapistan_ButtonText = "AlarmButtonAncapistan";
		private const int AlarmButtonAncapistan_CashCost = 25;

		[RLSetup, UsedImplicitly]
		private static void Initialize()
		{
			AlarmButtonController controller = new AlarmButtonController();
			ObjectControllerManager.RegisterObjectController(controller);
		}
		
		[UsedImplicitly]
		public static bool DoShowObjectButtons(AlarmButton alarmButton)
		{
			if (!alarmButton.isBroken() && GameController.gameController.challenges.Contains(cChallenge.AnCapistan))
			{
				alarmButton.ShowObjectButtons();
				return true;
			}
			return false;
		}

		public void HandleRevertAllVars(AlarmButton objectInstance) { }
		public void HandleObjectUpdate(AlarmButton objectInstance) { }
		public void HandlePlayerHasUsableItem(AlarmButton objectInstance, InvItem itemToTest, ref bool result) { }

		public void HandlePressedButton(AlarmButton objectInstance, string buttonText, int buttonPrice)
		{
			Agent agent = objectInstance.interactingAgent;
			switch (buttonText)
			{
				case AlarmButtonAncapistan_ButtonText:
					if (objectInstance.moneySuccess(buttonPrice))
					{
						objectInstance.ToggleSwitch(agent, null);
					}
					else
					{
						BMHeaderTools.SayDialogue(agent, cDialogue.CantAffordAlarmButton, vNameType.Dialogue);
					}
					break;
			}
		}

		public void HandleDetermineButtons(AlarmButton objectInstance)
		{
			GameController gc = GameController.gameController;
			// not sure how I feel about Challenge logic being here - but not sure where else to put it either
			if (gc.challenges.Contains(cChallenge.AnCapistan) && !objectInstance.hacked)
			{
				objectInstance.AddButton(
						text: AlarmButtonAncapistan_ButtonText,
						price: AlarmButtonAncapistan_CashCost
				);
			}
		}

		public void HandleFinishedOperating(AlarmButton objectInstance) { }
		public void HandleInteract(AlarmButton objectInstance, Agent agent) { }
		public void HandleObjectAction(AlarmButton objectInstance, string action, ref bool noMoreObjectActions, string extraString, float extraFloat, Agent causerAgent, PlayfieldObject extraObject) { }
		public void HandleDamagedObject(AlarmButton objectInstance, PlayfieldObject damagerObject, float damageAmount) { }
		public void HandleMakeNonFunctional(AlarmButton objectInstance, PlayfieldObject damagerObject) { }
		public void HandleDestroyMe(AlarmButton objectInstance, PlayfieldObject damagerObject) { }
		public void HandleDestroyMe3(AlarmButton objectInstance) { }
	}
}