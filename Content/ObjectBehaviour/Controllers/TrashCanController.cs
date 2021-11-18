using BunnyMod.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.ObjectBehaviour.Controllers
{
	public class TrashCanController : IObjectController<TrashCan>
	{
		private const string HideInContainer_ButtonText = "HideInContainer";
		private const string OpenContainer_ButtonText = "OpenContainer";
		
		[RLSetup, UsedImplicitly]
		private static void Initialize()
		{
			TrashCanController controller = new TrashCanController();
			ObjectControllerManager.RegisterObjectController(controller);
		}
		
		public void HandleRevertAllVars(TrashCan objectInstance) { }
		public void HandleObjectUpdate(TrashCan objectInstance) { }
		public void HandlePlayerHasUsableItem(TrashCan objectInstance, InvItem itemToTest, ref bool result) { }

		public void HandlePressedButton(TrashCan objectInstance, string buttonText, int buttonPrice)
		{
			Agent agent = objectInstance.interactingAgent;
			switch (buttonText)
			{
				case HideInContainer_ButtonText:
					ObjectUtils.HideInObject(agent, objectInstance);
					break;
				case OpenContainer_ButtonText:
					objectInstance.ShowChest();
					break;
			}
		}

		public void HandleDetermineButtons(TrashCan objectInstance)
		{
			objectInstance.AddButton(text: HideInContainer_ButtonText);
			objectInstance.AddButton(text: OpenContainer_ButtonText);
		}

		public void HandleFinishedOperating(TrashCan objectInstance) { }
		public void HandleInteract(TrashCan objectInstance, Agent agent) { }
		public void HandleObjectAction(TrashCan objectInstance, string action, ref bool noMoreObjectActions, string extraString, float extraFloat, Agent causerAgent, PlayfieldObject extraObject) { }
		public void HandleDamagedObject(TrashCan objectInstance, PlayfieldObject damagerObject, float damageAmount) { }
		public void HandleMakeNonFunctional(TrashCan objectInstance, PlayfieldObject damagerObject) { }
		public void HandleDestroyMe(TrashCan objectInstance, PlayfieldObject damagerObject) { }
		public void HandleDestroyMe3(TrashCan objectInstance) { }
	}
}