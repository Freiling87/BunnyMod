using System.Collections.Generic;
using System.Linq;
using BunnyMod.Extensions;
using BunnyMod.Traits;
using BunnyMod.Traits.T_Stealth;
using Google2u;
using JetBrains.Annotations;
using RogueLibsCore;
using UnityEngine;

namespace BunnyMod.ObjectBehaviour.Controllers
{
	public class ManholeController : IObjectController<Manhole>
	{
		private const string FlushYourself_ButtonText = "FlushYourself";

		private const string UseCrowbar_ButtonText = "UseCrowbar";
		private const string UseCrowbar_BarType = nameof(InterfaceNameDB.rowIds.Unlocking);
		private const int UseCrowbar_ToolCost = 15;

		[RLSetup, UsedImplicitly]
		private static void Initialize()
		{
			ManholeController controller = new ManholeController();
			ObjectControllerManager.RegisterObjectController(controller);
		}
		
		public static void SetVars(Manhole manhole)
		{
			manhole.interactable = true;
		}

		[UsedImplicitly]
		public static bool ShouldSpawnManhole()
		{
			return BMTraitController.IsPlayerTraitActive<UnderdarkCitizen>();
		}

		public static void HandleFlushYourself(ObjectReal manhole, Agent agent)
		{
			GameController gc = GameController.gameController;
			bool canGoToToilets = agent.HasTrait(StatusEffectNameDB.rowIds.Diminutive) || agent.shrunk;
			List<ObjectReal> exits = gc.objectRealList
					.Where(thing => thing != manhole)
					.Where(thing =>
							thing is Manhole anotherManhole && anotherManhole.opened
							|| canGoToToilets && thing is Toilet && !thing.destroyed
					)
					.ToList();
			ObjectReal exit = exits.Count > 0
					? exits[Random.Range(0, exits.Count - 1)]
					: manhole;

			gc.audioHandler.Play(agent, "ToiletTeleportIn");
			agent.toiletTeleporting = true;

			if (exit is Manhole)
			{
				Vector3 exitSpot = exit.curPosition;
				agent.Teleport((Vector2) exitSpot + Random.insideUnitCircle.normalized, true, false);
				gc.spawnerMain.SpawnExplosion(exit, exitSpot, "Water", false, -1, false, exit.FindMustSpawnExplosionOnClients(agent));
			}
			else if (exit is Toilet)
			{
				Vector3 exitSpot = exit.tr.position;
				const float exitOffset = 0.32f;
				switch (exit.direction)
				{
					case "N":
						exitSpot.y += exitOffset;
						break;
					case "S":
						exitSpot.y -= exitOffset;
						break;
					case "E":
						exitSpot.x += exitOffset;
						break;
					case "W":
						exitSpot.x -= exitOffset;
						break;
				}
				agent.Teleport(exitSpot, false, true);
				gc.spawnerMain.SpawnExplosion(agent, exit.tr.position, "Water", false, -1, false, exit.FindMustSpawnExplosionOnClients(agent));
			}
		}

		private static void HandleUseCrowbar(Manhole manhole)
		{
			GameController gc = GameController.gameController;
			if (gc.serverPlayer)
			{
				Vector3 position = manhole.tr.position;
				position.y -= 0.24f;

				manhole.hole = gc.spawnerMain.SpawnHole(manhole, position, new Vector3(1.5f, 1.5f, 1f), Quaternion.identity, false, true);
				manhole.hole.ObjectHoleAppear(nameof(ObjectNameDB.rowIds.Manhole));
				gc.playerAgent.objectMult.ObjectAction(manhole.objectNetID, "HoleAppear");
				manhole.operatingAgent.inventory.SubtractFromItemCount(manhole.operatingItem, UseCrowbar_ToolCost);
			}

			manhole.objectSprite.meshRenderer.enabled = false;
			manhole.opened = true;
			manhole.SetSDangerousToWalk(true);
			gc.audioHandler.Play(manhole, "ManholeOpen");

			if (gc.levelFeeling == "WarZone")
			{
				manhole.objectRealRealName = gc.nameDB.GetName(nameof(ObjectNameDB.rowIds.Hole), "Object");
				manhole.normalHole = true;
			}
		}

		public void HandleRevertAllVars(Manhole objectInstance) { }

		public void HandleObjectUpdate(Manhole objectInstance) { }
		public void HandlePlayerHasUsableItem(Manhole objectInstance, InvItem itemToTest, ref bool result) { }

		public void HandlePressedButton(Manhole objectInstance, string buttonText, int buttonPrice)
		{
			Agent agent = objectInstance.interactingAgent;
			switch (buttonText)
			{
				case FlushYourself_ButtonText:
					HandleFlushYourself(objectInstance, agent);
					break;
				case UseCrowbar_ButtonText:
					objectInstance.StartCoroutine(objectInstance.Operating(agent, agent.inventory.FindItem(ItemNameDB.rowIds.Crowbar), 2f, true,
							UseCrowbar_BarType));
					ObjectUtils.SpawnSuspiciousNoise(0, 1f, agent, objectInstance);
					break;
			}
		}

		public void HandleDetermineButtons(Manhole objectInstance)
		{
			Agent agent = objectInstance.interactingAgent;
			if (!objectInstance.opened)
			{
				if (agent.inventory.HasItem(ItemNameDB.rowIds.Crowbar))
				{
					int crowbarCount = agent.inventory.FindItem(ItemNameDB.rowIds.Crowbar).invItemCount;
					int toolCost = BMTraitController.ApplyToolCostModifiers(agent, UseCrowbar_ToolCost);
					objectInstance.AddButton(
							text: UseCrowbar_ButtonText,
							extraText: $" ({crowbarCount}) -{toolCost}"
					);
				}
			}
			else
			{
				if (agent.HasTrait<UnderdarkCitizen>())
				{
					objectInstance.AddButton(text: FlushYourself_ButtonText);
				}
			}
		}

		public void HandleFinishedOperating(Manhole objectInstance)
		{
			if (objectInstance.operatingBarType == UseCrowbar_BarType)
			{
				HandleUseCrowbar(objectInstance);
				objectInstance.StopInteraction();
			}
		}

		public void HandleInteract(Manhole objectInstance, Agent agent)
		{
			objectInstance.ShowObjectButtons();
		}

		public void HandleObjectAction(Manhole objectInstance, string action, ref bool noMoreObjectActions, string extraString, float extraFloat, Agent causerAgent, PlayfieldObject extraObject)
		{
			if (action == FlushYourself_ButtonText)
			{
				HandleFlushYourself(objectInstance, objectInstance.interactingAgent);
			}
			else if (action == UseCrowbar_ButtonText)
			{
				HandleUseCrowbar(objectInstance);
			}
		}

		public void HandleDamagedObject(Manhole objectInstance, PlayfieldObject damagerObject, float damageAmount) { }
		public void HandleMakeNonFunctional(Manhole objectInstance, PlayfieldObject damagerObject) { }
		public void HandleDestroyMe(Manhole objectInstance, PlayfieldObject damagerObject) { }
		public void HandleDestroyMe3(Manhole objectInstance) { }
	}
}