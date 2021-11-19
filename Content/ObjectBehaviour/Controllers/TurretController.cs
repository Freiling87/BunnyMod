using BunnyMod.Extensions;
using Google2u;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.ObjectBehaviour.Controllers
{
	public class TurretController : IObjectController<Turret>
	{
		[RLSetup, UsedImplicitly]
		private static void Initialize()
		{
			TurretController controller = new TurretController();
			ObjectControllerManager.RegisterObjectController(controller);
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

			return false;
		}

		public void HandleRevertAllVars(Turret objectInstance) { }
		public void HandleObjectUpdate(Turret objectInstance) { }
		public void HandlePlayerHasUsableItem(Turret objectInstance, InvItem itemToTest, ref bool result) { }
		public void HandlePressedButton(Turret objectInstance, string buttonText, int buttonPrice) { }
		public void HandleDetermineButtons(Turret objectInstance) { }
		public void HandleFinishedOperating(Turret objectInstance) { }
		public void HandleInteract(Turret objectInstance, Agent agent) { }
		public void HandleObjectAction(Turret objectInstance, string action, ref bool noMoreObjectActions, string extraString, float extraFloat, Agent causerAgent, PlayfieldObject extraObject) { }
		public void HandleDamagedObject(Turret objectInstance, PlayfieldObject damagerObject, float damageAmount) { }
		public void HandleMakeNonFunctional(Turret objectInstance, PlayfieldObject damagerObject) { }
		public void HandleDestroyMe(Turret objectInstance, PlayfieldObject damagerObject) { }
		public void HandleDestroyMe3(Turret objectInstance) { }
	}
}