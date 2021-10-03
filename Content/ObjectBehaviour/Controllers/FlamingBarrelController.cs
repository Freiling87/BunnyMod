using BunnyMod.Content.Extensions;
using BunnyMod.Content.Traits;
using Google2u;
using RogueLibsCore;

namespace BunnyMod.Content.ObjectBehaviour
{
	public class FlamingBarrelController : ObjectControllerInterface<FlamingBarrel>
	{
		private const string GrillFud_ButtonText = "GrillFud";
		private const string GrillFud_BarType = nameof(InterfaceNameDB.rowIds.Grilling);
		private const int GrillFud_HealthCost = 10;

		private const string LightBarbecue_ButtonText = "LightBarbecue";

		public static void SetVars(FlamingBarrel flamingBarrel)
		{
			flamingBarrel.interactable = true;
			flamingBarrel.fireDoesntDamage = true;
		}

		private static void HandleGrillFud(ObjectReal barrel)
		{
			GameController gc = GameController.gameController;
			Agent agent = barrel.interactingAgent;
			InvDatabase agentInventory = agent.inventory;
			InvItem rawFudItem = agentInventory.FindItem(ItemNameDB.rowIds.Fud);
			int cookedCount = rawFudItem.invItemCount;

			gc.spawnerMain.SpawnNoise(barrel.curPosition, 1f, null, null, barrel.lastHitByAgent);
			gc.audioHandler.Play(barrel, "Grill");

			// Handle items
			{
				agentInventory.SubtractFromItemCount(rawFudItem, cookedCount);

				InvItem hotFudItem = new InvItem
				{
						invItemName = nameof(ItemNameDB.rowIds.HotFud),
						invItemCount = cookedCount
				};
				hotFudItem.SetupDetails(false);
				hotFudItem.ShowPickingUpText(agent);
				agentInventory.AddItemOrDrop(hotFudItem);
			}

			// Apply grill damage
			{
				gc.audioHandler.Play(barrel, "FireHit");
				if (agent.HasTrait(StatusEffectNameDB.rowIds.ResistFire)
						|| agent.HasTrait(StatusEffectNameDB.rowIds.FireproofSkin)
						|| agent.HasTrait(StatusEffectNameDB.rowIds.FireproofSkin2))
				{
					BMHeaderTools.SayDialogue(agent, cDialogue.FlamingBarrelCookNoDamage, vNameType.Dialogue);
				}
				else
				{
					agent.ChangeHealth(-GrillFud_HealthCost, barrel);
					BMHeaderTools.SayDialogue(agent, cDialogue.FlamingBarrelCookDamage, vNameType.Dialogue);
				}
			}
		}

		public void HandleObjectUpdate(FlamingBarrel objectInstance) { }
		public void HandlePlayerHasUsableItem(FlamingBarrel objectInstance, InvItem itemToTest, ref bool result) { }

		public void HandlePressedButton(FlamingBarrel objectInstance, string buttonText, int buttonPrice)
		{
			Agent agent = objectInstance.interactingAgent;
			switch (buttonText)
			{
				case GrillFud_ButtonText:
					objectInstance.StartCoroutine(objectInstance.Operating(agent, agent.inventory.FindItem(ItemNameDB.rowIds.Fud), 2f, true, GrillFud_BarType));
					break;
				case LightBarbecue_ButtonText:
					objectInstance.StartFireInObject();
					objectInstance.StopInteraction();
					break;
			}
		}

		public void HandleDetermineButtons(FlamingBarrel objectInstance)
		{
			Agent agent = objectInstance.interactingAgent;
			if (objectInstance.ora.hasParticleEffect)
			{
				if (agent.inventory.HasItem(ItemNameDB.rowIds.Fud))
				{
					objectInstance.AddButton(
							text: GrillFud_ButtonText,
							extraText:
							$" (Burn hands for {BMTraitController.HealthCost(agent, GrillFud_HealthCost, DamageType.burnedFingers)} damage)" // TODO localization ??
					);
				}
				else
				{
					BMHeaderTools.SayDialogue(agent, /* Vanilla magic-string */ "CantGrillFud", vNameType.Dialogue);
				}
			}
			else
			{
				if (agent.inventory.HasItem(ItemNameDB.rowIds.CigaretteLighter))
				{
					objectInstance.AddButton(text: LightBarbecue_ButtonText);
				}
				else
				{
					BMHeaderTools.SayDialogue(agent, /* Vanilla magic-string */ "CantOperateBarbecue", vNameType.Dialogue);
				}
			}
		}

		public void HandleFinishedOperating(FlamingBarrel objectInstance)
		{
			if (objectInstance.operatingBarType == GrillFud_BarType)
			{
				HandleGrillFud(objectInstance);
				objectInstance.StopInteraction();
			}
		}

		public void HandleInteract(FlamingBarrel objectInstance, Agent agent)
		{
			objectInstance.ShowObjectButtons();
		}

		public void HandleObjectAction(FlamingBarrel objectInstance, string action, ref bool noMoreObjectActions) { }
		public void HandleDamagedObject(FlamingBarrel objectInstance, PlayfieldObject damagerObject, float damageAmount) { }
		public void HandleMakeNonFunctional(FlamingBarrel objectInstance, PlayfieldObject damagerObject) { }
		public void HandleDestroyMe(FlamingBarrel objectInstance, PlayfieldObject damagerObject) { }
		public void HandleDestroyMe3(FlamingBarrel objectInstance) { }
	}
}