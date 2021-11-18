using System.Collections;
using BepInEx.Logging;
using BunnyMod.Extensions;
using BunnyMod.Logging;
using BunnyMod.ObjectBehaviour.Controllers.Data;
using BunnyMod.Traits;
using Google2u;
using JetBrains.Annotations;
using RogueLibsCore;
using UnityEngine;

namespace BunnyMod.ObjectBehaviour.Controllers
{
	// TODO think about creating an animation-sequence for WrenchDetonate.

	public class StoveController : IObjectController<Stove>
	{
		private const string WrenchDetonate_ButtonText = "UseWrenchToDetonate";
		private const string WrenchDetonate_BarType = nameof(InterfaceNameDB.rowIds.Tampering);
		private const int WrenchDetonate_ToolCost = 30;

		private const string GrillFud_ButtonText = "GrillFudPaid";
		private const string GrillFud_BarType = nameof(InterfaceNameDB.rowIds.Grilling);
		private const int GrillFud_CashCost = 5;

		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		private static readonly ObjectDataAccessor<StoveData> dataAccessor = new ObjectDataAccessor<StoveData>();

		[RLSetup, UsedImplicitly]
		private static void Initialize()
		{
			StoveController controller = new StoveController();
			ObjectControllerManager.RegisterObjectController(controller);
			ObjectControllerManager.RegisterDataController<Stove>(dataAccessor);
		}

		public static void SetVars(Stove stove)
		{
			stove.canExplosiveStimulate = true;
			stove.dontDestroyImmediateOnClient = true;
			stove.hasUpdate = true;
			stove.interactable = true;
		}

		private static void RegisterDamagedBy(PlayfieldObject stove, PlayfieldObject damagerObject)
		{
			dataAccessor.GetObjectData(stove).savedDamagerObject = damagerObject;
		}

		private static void TriggerExplosion(ObjectReal stove)
		{
			GameController gc = GameController.gameController;
			if (gc.serverPlayer && !stove.spawnedExplosion)
			{
				stove.spawnedExplosion = true;
				StoveData stoveData = dataAccessor.GetObjectData(stove);
				PlayfieldObject damagerObject = stoveData.savedDamagerObject;
				Explosion explosion = gc.spawnerMain.SpawnExplosion(damagerObject, stove.tr.position, "FireBomb", false, -1, false,
						stove.FindMustSpawnExplosionOnClients(damagerObject));

				if (stoveData.noOwnCheckCountdown)
				{
					explosion.noOwnCheck = true;
				}
			}
			gc.audioHandler.Stop(stove, "GeneratorHiss");
		}

		private static void HandleWrenchDetonate(ObjectReal stove)
		{
			logger.LogDebug($"Detonating stove '{stove.name}' with wrench.");
			GameController gc = GameController.gameController;
			Agent agent = stove.interactingAgent;
			InvDatabase agentInventory = agent.inventory;

			if (gc.serverPlayer)
			{
				stove.MakeNonFunctional(agent);
				agentInventory.SubtractFromItemCount(agentInventory.FindItem(ItemNameDB.rowIds.Wrench), WrenchDetonate_ToolCost);
				agent.skillPoints.AddPoints(nameof(InterfaceNameDB.rowIds.TamperGeneratorPoints));
			}
			else
			{
				stove.functional = false;
				agent.objectMult.ObjectAction(stove.objectNetID, nameof(InterfaceNameDB.rowIds.UseWrenchToDetonate));
			}
			gc.playerAgent.SetCheckUseWithItemsAgain(stove);
		}

		private static void HandleGrillFud(ObjectReal stove)
		{
			logger.LogDebug($"Grilling Fud on stove '{stove.name}'");
			GameController gc = GameController.gameController;
			Agent agent = stove.interactingAgent;
			InvDatabase agentInventory = agent.inventory;
			InvItem rawFudItem = agentInventory.FindItem(ItemNameDB.rowIds.Fud);
			int cookedCount = rawFudItem.invItemCount;

			gc.spawnerMain.SpawnNoise(stove.curPosition, 1f, null, null, stove.lastHitByAgent);
			gc.audioHandler.Play(stove, "Grill");

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
		}

		private static IEnumerator CountdownCoroutine(ObjectReal stove)
		{
			yield return null;
			while (true)
			{
				if (stove.timeCountdownClock > 0 && !stove.destroyed && !stove.destroying)
				{
					GameController.gameController.spawnerMain.SpawnStatusText(stove, "Countdown", stove.timeCountdownClock.ToString());
				}
				stove.timeCountdownClock--;
				if (stove.timeCountdownClock <= 0 || stove.destroyed)
				{
					break;
				}
				yield return new WaitForSeconds(1f);
			}
		}

		private static IEnumerator AboutToExplode(ObjectReal stove)
		{
			logger.LogDebug($"Stove '{stove.name}' is about to explode, "
					+ $"lastHitByAgent: '{(stove.lastHitByAgent != null ? stove.lastHitByAgent.agentName : "null")}'");

			GameController gc = GameController.gameController;
			stove.interactable = false;
			stove.PlayObjectSpriteEffect( /* vanilla magic string */ "FlashingRepeatedly");
			if (stove.lastHitByAgent != null)
			{
				gc.spawnerMain.SpawnNoise(stove.tr.position, 1f, null, null, stove.lastHitByAgent);
				gc.OwnCheck(stove.lastHitByAgent, stove.go, "Normal", 0);
			}

			Vector3 particlePosition = stove.tr.position; // copy position
			particlePosition.y += 0.36f; // apply offset to copy
			stove.SpawnParticleEffect( /* vanilla magic string */ "Smoke", particlePosition);

			gc.audioHandler.Play(stove, /* vanilla magic string */ "GeneratorHiss");
			stove.RemoveObjectAgent();
			stove.cantMakeFollowersAttack = true;
			yield return new WaitForSeconds(3f);

			if (!stove.destroying)
			{
				gc.spawnerMain.SpawnExplosion(stove.lastHitByAgent, stove.curPosition, "FireBomb", false, -1, false, true);
			}
		}

		public void HandleRevertAllVars(Stove objectInstance)
		{
			Transform realSpriteTransform = objectInstance.objectSprite.transform.Find("RealSprite");
			realSpriteTransform.localPosition = Vector3.zero;
			realSpriteTransform.localScale = Vector3.one;
		}

		public void HandleObjectUpdate(Stove objectInstance)
		{
			if (objectInstance.timer > 0f)
			{
				objectInstance.timer -= Time.deltaTime;
				if (objectInstance.timer <= 0f)
				{
					StoveData stoveData = dataAccessor.GetObjectData(objectInstance);
					if (objectInstance.startedFlashing)
					{
						objectInstance.DestroyMe(stoveData.savedDamagerObject);
					}
					else
					{
						stoveData.noOwnCheckCountdown = true;
						stoveData.savedDamagerObject = stoveData.countdownCauser;
						objectInstance.DestroyMe(stoveData.countdownCauser);
					}
				}
			}
		}

		public void HandlePlayerHasUsableItem(Stove objectInstance, InvItem itemToTest, ref bool result)
		{
			bool rightItem = itemToTest.IsItem(ItemNameDB.rowIds.Wrench) || itemToTest.IsItem(ItemNameDB.rowIds.Fud);
			result |= rightItem && objectInstance.timer <= 0f && !objectInstance.startedFlashing;
		}

		public void HandlePressedButton(Stove objectInstance, string buttonText, int buttonPrice)
		{
			Agent agent = objectInstance.interactingAgent;
			switch (buttonText)
			{
				case WrenchDetonate_ButtonText:
					objectInstance.StartCoroutine(objectInstance.Operating(agent, agent.inventory.FindItem(ItemNameDB.rowIds.Wrench), 2f, true,
							WrenchDetonate_BarType));
					ObjectUtils.SpawnSuspiciousNoise(0, 1f, agent, objectInstance);
					break;
				case GrillFud_ButtonText:
					if (objectInstance.moneySuccess(5))
					{
						objectInstance.StartCoroutine(objectInstance.Operating(agent, agent.inventory.FindItem(ItemNameDB.rowIds.Fud), 2f, true,
								GrillFud_BarType));
					}
					break;
			}
		}

		public void HandleDetermineButtons(Stove objectInstance)
		{
			if (objectInstance.startedFlashing)
			{
				return;
			}

			Agent agent = objectInstance.interactingAgent;
			if (agent.inventory.HasItem(ItemNameDB.rowIds.Wrench))
			{
				int invItemCount = agent.inventory.FindItem(ItemNameDB.rowIds.Wrench).invItemCount;
				int toolCost = BMTraitController.ApplyToolCostModifiers(agent, WrenchDetonate_ToolCost);
				objectInstance.AddButton(
						text: WrenchDetonate_ButtonText,
						extraText: $" ({invItemCount}) -{toolCost}"
				);
			}

			if (agent.inventory.HasItem(ItemNameDB.rowIds.Fud))
			{
				if (GameController.gameController.challenges.Contains(cChallenge.AnCapistan))
				{
					objectInstance.AddButton(
							text: GrillFud_ButtonText,
							price: GrillFud_CashCost
					);
				}
			}
		}

		public void HandleFinishedOperating(Stove objectInstance)
		{
			if (objectInstance.operatingBarType == WrenchDetonate_BarType)
			{
				HandleWrenchDetonate(objectInstance);
				objectInstance.StopInteraction();
			}
			else if (objectInstance.operatingBarType == GrillFud_BarType)
			{
				HandleGrillFud(objectInstance);
				objectInstance.StopInteraction();
			}
		}

		public void HandleInteract(Stove objectInstance, Agent agent)
		{
			if (objectInstance.timer > 0f || objectInstance.startedFlashing)
			{
				objectInstance.StopInteraction();
				return;
			}

			objectInstance.ShowObjectButtons();
		}

		public void HandleObjectAction(Stove objectInstance, string action, ref bool noMoreObjectActions, string extraString, float extraFloat, Agent causerAgent, PlayfieldObject extraObject)
		{
			if (!noMoreObjectActions && action == WrenchDetonate_ButtonText)
			{
				HandleWrenchDetonate(objectInstance);
			}
			noMoreObjectActions = false; // ??? why
		}

		public void HandleDamagedObject(Stove objectInstance, PlayfieldObject damagerObject, float damageAmount)
		{
			if (damageAmount >= 15f && !objectInstance.startedFlashing)
			{
				objectInstance.StartCoroutine(AboutToExplode(objectInstance));
			}

			if (damageAmount >= objectInstance.damageThreshold)
			{
				GameController.gameController.spawnerMain.SpawnExplosion(damagerObject, objectInstance.curPosition, "FireBomb", false, -1, false, true);
			}
		}

		public void HandleMakeNonFunctional(Stove objectInstance, PlayfieldObject damagerObject)
		{
			if (damagerObject == null || !objectInstance.interactable)
			{
				return;
			}

			objectInstance.timer = 5f;
			objectInstance.timeCountdownClock = (int) objectInstance.timer;
			objectInstance.interactable = false;
			StoveData stoveData = dataAccessor.GetObjectData(objectInstance);
			stoveData.savedDamagerObject = damagerObject;
			stoveData.countdownCauser = damagerObject;
			objectInstance.StartCoroutine(CountdownCoroutine(objectInstance));
		}

		public void HandleDestroyMe(Stove objectInstance, PlayfieldObject damagerObject)
		{
			RegisterDamagedBy(objectInstance, damagerObject);
		}

		public void HandleDestroyMe3(Stove objectInstance)
		{
			TriggerExplosion(objectInstance);
		}
	}
}