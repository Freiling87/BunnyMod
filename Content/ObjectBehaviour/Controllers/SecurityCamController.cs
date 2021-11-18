using BunnyMod.Extensions;
using BunnyMod.Traits.T_Social;
using Google2u;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.ObjectBehaviour.Controllers
{
	public class SecurityCamController : IObjectController<SecurityCam>
	{
		private const string CamerasCaptureWanted_ButtonText = "CamerasCaptureWanted";
		private const string CamerasCaptureWanted_TargetType = "Wanted";

		private const string CamerasCaptureGuilty_ButtonText = "CamerasCaptureGuilty";
		private const string CamerasCaptureGuilty_TargetType = "Guilty";

		[RLSetup, UsedImplicitly]
		private static void Initialize()
		{
			SecurityCamController controller = new SecurityCamController();
			ObjectControllerManager.RegisterObjectController(controller);
		}
		
		public static void StartLate(SecurityCam camera)
		{
			if (camera.owner == 85) // TODO magic id
			{
				camera.targets = GameController.gameController.challenges.Contains(cChallenge.PoliceState)
						? CamerasCaptureGuilty_TargetType
						: CamerasCaptureWanted_TargetType;
			}
		}

		/// <summary>
		/// Expands the camera detection system.
		/// Called between the vanilla code evaluating the detection flag for an agent and the first use of that flag
		/// </summary>
		/// <param name="camera">Camera that is checking for detected agents</param>
		/// <param name="agent">agent that is currently being checked</param>
		/// <param name="currentlyDetecting">the detection status according to vanilla code</param>
		/// <returns>the new detection status</returns>
		[UsedImplicitly]
		public static bool IsCameraDetectingAgent(SecurityCam camera, Agent agent, bool currentlyDetecting)
		{
			if (currentlyDetecting)
			{
				return true;
			}

			switch (camera.targets)
			{
				case "Owners": /* vanilla magic string */
					return agent.IsEnforcer() && camera.owner == 85; // TODO magic ID
				case CamerasCaptureWanted_TargetType:
					return agent.HasTrait(StatusEffectNameDB.rowIds.Wanted);
				case CamerasCaptureGuilty_TargetType:
					return agent.objectMultAgent.mustBeGuilty || agent.HasTrait(StatusEffectNameDB.rowIds.Wanted) || agent.HasTrait<Priors>();
			}
			return false;
		}

		/// <summary>
		/// This method is called when a button is pressed.
		/// Any button logic handled here does not close the interaction menu (unless you call StopInteraction)
		/// </summary>
		/// <param name="camera">Camera that is being interacted with</param>
		/// <param name="buttonText">text of the pressed button</param>
		/// <returns>true if the button press was handled by this method, otherwise false</returns>
		public static bool HandlePressedButton_KeepMenu(SecurityCam camera, string buttonText)
		{
			if (buttonText == CamerasCaptureWanted_ButtonText)
			{
				HandlePressedButton(camera, CamerasCaptureWanted_ButtonText, CamerasCaptureWanted_TargetType);
				return true;
			}
			if (buttonText == CamerasCaptureGuilty_ButtonText)
			{
				HandlePressedButton(camera, CamerasCaptureGuilty_ButtonText, CamerasCaptureGuilty_TargetType);
				return true;
			}
			return false;
		}

		// this is here to deduplicate button handling
		private static void HandlePressedButton(SecurityCam camera, string buttonText, string targetType)
		{
			camera.targets = targetType;
			if (!camera.gc.serverPlayer)
			{
				camera.interactingAgent.objectMult.ObjectAction(camera.objectNetID, buttonText);
			}
			camera.RefreshButtons();
		}

		public static void HandleSuccessfulManualShutdown(SecurityCam securityCam)
		{
			securityCam.interactingAgent.skillPoints.AddPoints(nameof(InterfaceNameDB.rowIds.TamperPoliceBoxPoints));
		}

		public void HandleRevertAllVars(SecurityCam objectInstance) { }

		public void HandleObjectUpdate(SecurityCam objectInstance) { }

		public void HandlePlayerHasUsableItem(SecurityCam objectInstance, InvItem itemToTest, ref bool result) { }

		// beware: SecurityCam buttons should not close the interaction menu, but `StopInteraction` is hard-coded into the PressedButton method (which is ridiculous)
		//  because of this, you should use 
		public void HandlePressedButton(SecurityCam objectInstance, string buttonText, int buttonPrice) { }

		public void HandleDetermineButtons(SecurityCam objectInstance)
		{
			Agent agent = objectInstance.interactingAgent;
			if (agent.interactionHelper.interactingFar)
			{
				objectInstance.AddButton(
						text: CamerasCaptureWanted_ButtonText,
						extraText: objectInstance.targets == CamerasCaptureWanted_TargetType ? " *" : ""
				);
				objectInstance.AddButton(
						text: CamerasCaptureGuilty_ButtonText,
						extraText: objectInstance.targets == CamerasCaptureGuilty_TargetType ? " *" : ""
				);
			}
		}

		// beware: SecurityCam wants to be annoying and *does not* call ObjectReal::FinishedOperating -> this method won't be called
		public void HandleFinishedOperating(SecurityCam objectInstance) { }

		public void HandleInteract(SecurityCam objectInstance, Agent agent) { }

		public void HandleObjectAction(SecurityCam objectInstance, string action, ref bool noMoreObjectActions, string extraString, float extraFloat, Agent causerAgent, PlayfieldObject extraObject)
		{
			if (action == CamerasCaptureGuilty_ButtonText)
			{
				objectInstance.targets = CamerasCaptureGuilty_TargetType;
			}
			else if (action == CamerasCaptureWanted_ButtonText)
			{
				objectInstance.targets = CamerasCaptureWanted_TargetType;
			}
		}

		public void HandleDamagedObject(SecurityCam objectInstance, PlayfieldObject damagerObject, float damageAmount) { }

		public void HandleMakeNonFunctional(SecurityCam objectInstance, PlayfieldObject damagerObject) { }

		public void HandleDestroyMe(SecurityCam objectInstance, PlayfieldObject damagerObject) { }

		public void HandleDestroyMe3(SecurityCam objectInstance) { }
	}
}