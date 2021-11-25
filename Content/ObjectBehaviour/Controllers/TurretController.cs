using BunnyMod.Extensions;
using Google2u;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.ObjectBehaviour.Controllers
{
	public class TurretController : IObjectController<Turret>
	{
		private const string TurretsAttackWanted_ButtonText = "TurretsAttackWanted"; // TODO localization
		private const string TurretsAttackWanted_TargetType = "Wanted";

		private const string TurretsAttackGuilty_ButtonText = "TurretsAttackGuilty"; // TODO localization
		private const string TurretsAttackGuilty_TargetType = "Guilty";

		[RLSetup, UsedImplicitly]
		private static void Initialize()
		{
			TurretController controller = new TurretController();
			ObjectControllerManager.RegisterObjectController(controller);
		}

		public static void Start_Postfix(Turret turret)
		{
			if (turret.owner == 85) // TODO magic id
			{
				turret.targets = GameController.gameController.challenges.Contains(cChallenge.PoliceState)
						? TurretsAttackGuilty_TargetType
						: TurretsAttackWanted_TargetType;
			}
		}

		[UsedImplicitly]
		public static bool IsOpponent(Agent agent, Turret turret)
		{
			if (GameController.gameController.challenges.Contains(cChallenge.PoliceState))
			{
				if (agent.HasTrait(StatusEffectNameDB.rowIds.Wanted) || agent.objectMultAgent.mustBeGuilty)
				{
					return true;
				}
			}

			switch (turret.targets)
			{
				case "Owners":
					return agent.IsEnforcer() && turret.owner == 85; // TODO magic id
				case TurretsAttackWanted_TargetType:
					return agent.HasTrait(StatusEffectNameDB.rowIds.Wanted);
				case TurretsAttackGuilty_TargetType:
					return agent.IsGuilty();
			}

			return false;
		}

		/// <summary>
		/// This method is called when a button is pressed.
		/// Any button logic handled here does not close the interaction menu (unless you call StopInteraction)
		/// </summary>
		/// <param name="turret">Turret that is being interacted with</param>
		/// <param name="buttonText">text of the pressed button</param>
		/// <returns>true if the button press was handled by this method, otherwise false</returns>
		public static bool HandlePressedButton_KeepMenu(Turret turret, string buttonText)
		{
			if (buttonText == TurretsAttackWanted_ButtonText)
			{
				HandlePressedButton(turret, buttonText, TurretsAttackWanted_TargetType);
				return true;
			}
			if (buttonText == TurretsAttackGuilty_ButtonText)
			{
				HandlePressedButton(turret, buttonText, TurretsAttackGuilty_TargetType);
				return true;
			}
			return false;
		}

		// this is here to deduplicate button handling
		private static void HandlePressedButton(Turret turret, string buttonText, string targetType)
		{
			turret.targets = targetType;
			if (!turret.gc.serverPlayer)
			{
				turret.interactingAgent.objectMult.ObjectAction(turret.objectNetID, buttonText);
			}
			turret.RefreshButtons();
		}

		public void HandleRevertAllVars(Turret objectInstance) { }
		public void HandleObjectUpdate(Turret objectInstance) { }
		public void HandlePlayerHasUsableItem(Turret objectInstance, InvItem itemToTest, ref bool result) { }
		public void HandlePressedButton(Turret objectInstance, string buttonText, int buttonPrice) { }

		public void HandleDetermineButtons(Turret objectInstance)
		{
			Agent agent = objectInstance.interactingAgent;
			if (agent.interactionHelper.interactingFar)
			{
				objectInstance.AddButton(
						text: TurretsAttackWanted_ButtonText,
						extraText: objectInstance.targets == TurretsAttackWanted_TargetType ? " *" : ""
				);
				objectInstance.AddButton(
						text: TurretsAttackGuilty_ButtonText,
						extraText: objectInstance.targets == TurretsAttackGuilty_TargetType ? " *" : ""
				);
			}
		}

		public void HandleFinishedOperating(Turret objectInstance) { }
		public void HandleInteract(Turret objectInstance, Agent agent) { }

		public void HandleObjectAction(Turret objectInstance, string action, ref bool noMoreObjectActions, string extraString, float extraFloat, Agent causerAgent, PlayfieldObject extraObject)
		{
			if (action == TurretsAttackGuilty_ButtonText)
			{
				objectInstance.targets = TurretsAttackGuilty_TargetType;
			}
			else if (action == TurretsAttackWanted_ButtonText)
			{
				objectInstance.targets = TurretsAttackWanted_TargetType;
			}
		}

		public void HandleDamagedObject(Turret objectInstance, PlayfieldObject damagerObject, float damageAmount) { }
		public void HandleMakeNonFunctional(Turret objectInstance, PlayfieldObject damagerObject) { }
		public void HandleDestroyMe(Turret objectInstance, PlayfieldObject damagerObject) { }
		public void HandleDestroyMe3(Turret objectInstance) { }
	}
}