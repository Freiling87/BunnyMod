using System.Collections.Generic;
using System.Linq;
using BunnyMod.Extensions;
using BunnyMod.ObjectBehaviour.Controllers.Data;
using Google2u;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.ObjectBehaviour.Controllers
{
	public class VendorCartController : IObjectController<VendorCart>
	{
		private const string VendorCartSteal_ButtonText = "StealItem";
		private const string VendorCartSteal_BarType = nameof(InterfaceNameDB.rowIds.Tampering);

		private static readonly ObjectDataAccessor<VendorCartData> dataAccessor = new ObjectDataAccessor<VendorCartData>();

		[RLSetup, UsedImplicitly]
		private static void Initialize()
		{
			VendorCartController controller = new VendorCartController();
			ObjectControllerManager.RegisterObjectController(controller);
			ObjectControllerManager.RegisterDataController<VendorCart>(dataAccessor);
		}

		private static void HandleVendorCartSteal(PlayfieldObject vendorCart)
		{
			Agent agent = vendorCart.interactingAgent;
			InvDatabase agentInventory = agent.inventory;

			List<InvItem> vendorCartItemList = vendorCart.objectInvDatabase.InvItemList;
			foreach (InvItem invItem in vendorCartItemList.Where(invItem => invItem?.invItemName != null))
			{
				invItem.ItemSetup(true);
				invItem.invItemCount = GameController.gameController.Choose(1, 2, 2, 3);
				invItem.ShowPickingUpText(agent);
				agentInventory.AddItem(invItem);
			}

			dataAccessor.GetObjectData(vendorCart).wasStolenFrom = true;
			vendorCart.interactable = false;
			vendorCart.objectInvDatabase.DestroyAllItems();
		}

		public void HandleRevertAllVars(VendorCart objectInstance) { }
		public void HandleObjectUpdate(VendorCart objectInstance) { }
		public void HandlePlayerHasUsableItem(VendorCart objectInstance, InvItem itemToTest, ref bool result) { }

		public void HandlePressedButton(VendorCart objectInstance, string buttonText, int buttonPrice)
		{
			Agent agent = objectInstance.interactingAgent;
			switch (buttonText)
			{
				case VendorCartSteal_ButtonText:
					if (!agent.inventory.hasEmptySlot())
					{
						agent.inventory.PlayerFullResponse(agent);
						objectInstance.StopInteraction();
						break;
					}

					objectInstance.StartCoroutine(objectInstance.Operating(agent, null, 2f, false, VendorCartSteal_BarType));
					ObjectUtils.SpawnSuspiciousNoise(1, 0.2f, agent, objectInstance);
					break;
			}
		}

		public void HandleDetermineButtons(VendorCart objectInstance)
		{
			if (!dataAccessor.GetObjectData(objectInstance).wasStolenFrom)
			{
				objectInstance.AddButton(text: VendorCartSteal_ButtonText);
			}
		}

		public void HandleFinishedOperating(VendorCart objectInstance)
		{
			if (objectInstance.operatingBarType == VendorCartSteal_BarType)
			{
				HandleVendorCartSteal(objectInstance);
				objectInstance.StopInteraction();
			}
		}

		public void HandleInteract(VendorCart objectInstance, Agent agent)
		{
			if (dataAccessor.GetObjectData(objectInstance).wasStolenFrom)
			{
				objectInstance.StopInteraction();
				return;
			}

			objectInstance.ShowObjectButtons();
		}

		public void HandleObjectAction(VendorCart objectInstance, string action, ref bool noMoreObjectActions, string extraString, float extraFloat, Agent causerAgent, PlayfieldObject extraObject) { }
		public void HandleDamagedObject(VendorCart objectInstance, PlayfieldObject damagerObject, float damageAmount) { }
		public void HandleMakeNonFunctional(VendorCart objectInstance, PlayfieldObject damagerObject) { }
		public void HandleDestroyMe(VendorCart objectInstance, PlayfieldObject damagerObject) { }
		public void HandleDestroyMe3(VendorCart objectInstance) { }
	}
}