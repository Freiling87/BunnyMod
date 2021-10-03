using BunnyMod.Content.Extensions;
using BunnyMod.Content.Traits;
using Google2u;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.ObjectBehaviour
{
	public class SecurityCamController : ObjectControllerInterface<SecurityCam>
	{
		private const string CamerasCaptureWanted_ButtonText = "CamerasCaptureWanted";
		private const string CamerasCaptureWanted_TargetType = "Wanted";

		private const string CamerasCaptureGuilty_ButtonText = "CamerasCaptureGuilty";
		private const string CamerasCaptureGuilty_TargetType = "Guilty";

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

		public static void HandleSuccessfulManualShutdown(SecurityCam securityCam)
		{
			securityCam.interactingAgent.skillPoints.AddPoints(nameof(InterfaceNameDB.rowIds.TamperPoliceBoxPoints));
		}

		public void HandleObjectUpdate(SecurityCam objectInstance) { }

		public void HandlePlayerHasUsableItem(SecurityCam objectInstance, InvItem itemToTest, ref bool result) { }

		public void HandlePressedButton(SecurityCam objectInstance, string buttonText, int buttonPrice)
		{
			throw new System.NotImplementedException();
		}

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

		public void HandleObjectAction(SecurityCam objectInstance, string action, ref bool noMoreObjectActions)
		{
			throw new System.NotImplementedException();
		}

		public void HandleDamagedObject(SecurityCam objectInstance, PlayfieldObject damagerObject, float damageAmount) { }

		public void HandleMakeNonFunctional(SecurityCam objectInstance, PlayfieldObject damagerObject) { }

		public void HandleDestroyMe(SecurityCam objectInstance, PlayfieldObject damagerObject) { }

		public void HandleDestroyMe3(SecurityCam objectInstance) { }
	}
}