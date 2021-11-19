using System.Collections.Generic;
using System.Linq;
using BunnyMod.Extensions;
using BunnyMod.Traits.T_Stealth;
using Google2u;
using JetBrains.Annotations;
using RogueLibsCore;
using UnityEngine;

namespace BunnyMod.ObjectBehaviour.Controllers
{
	public class ToiletController : IObjectController<Toilet>
	{
		private const int AnCapistan_ButtonCost = 10;

		[RLSetup, UsedImplicitly]
		private static void Initialize()
		{
			ToiletController controller = new ToiletController();
			ObjectControllerManager.RegisterObjectController(controller);
		}

		public static bool FlushYourself_Prefix(Toilet toilet)
		{
			Agent agent = toilet.interactingAgent;
			if (!agent.HasTrait<UnderdarkCitizen>())
			{
				return true;
			}

			if ((agent.HasTrait(StatusEffectNameDB.rowIds.Diminutive) || agent.HasTrait(StatusEffectNameDB.rowIds.Shrunk))
					&& !agent.HasTrait(StatusEffectNameDB.rowIds.Giant))
			{
				List<ObjectReal> exits = GameController.gameController.objectRealList
						.Where(exitCandidate => UnderdarkCitizen.CanFlushToObject(toilet, exitCandidate, false))
						.ToList();

				if (exits.Count == 0)
				{
					exits = GameController.gameController.objectRealList
							.Where(exitCandidate => UnderdarkCitizen.CanFlushToObject(toilet, exitCandidate, true))
							.ToList();
				}

				ObjectReal exit = exits.Count <= 0 ? toilet : exits[Random.Range(0, exits.Count)];
				Vector3 exitSpot = exit.tr.position;
				switch (exit.direction)
				{
					case "E":
						exitSpot += new Vector3(0.32f, 0f, 0f);
						break;
					case "N":
						exitSpot += new Vector3(0f, 0.32f, 0f);
						break;
					case "S":
						exitSpot += new Vector3(0f, -0.32f, 0f);
						break;
					case "W":
						exitSpot += new Vector3(-0.32f, 0f, 0f);
						break;
				}

				GameController.gameController.audioHandler.Play(toilet, "ToiletTeleportIn");
				agent.toiletTeleporting = true;
				agent.Teleport(exitSpot, false, true);
				GameController.gameController.spawnerMain.SpawnExplosion(agent, exit.tr.position, "Water", false, -1, false, toilet.FindMustSpawnExplosionOnClients(agent));
			}
			return false;
		}

		public static void HandleDetermineButtons_Postfix(Toilet instance)
		{
			if (GameController.gameController.challenges.Contains(cChallenge.AnCapistan))
			{
				instance.NormalizeButtons();
				int buttonCount = instance.buttons.Count;
				for (int i = 0; i < buttonCount; i++)
				{
					instance.buttonPrices[i] = AnCapistan_ButtonCost;
				}
			}
		}

		public static bool HandlePressedButton_Prefix(Toilet instance, int buttonPrice)
		{
			if (GameController.gameController.challenges.Contains(cChallenge.AnCapistan))
			{
				if (!instance.moneySuccess(buttonPrice))
				{
					BMHeaderTools.SayDialogue(instance.interactingAgent, cDialogue.CantAffordToilet, vNameType.Dialogue);
					instance.StopInteraction();
					return false;
				}
			}
			return true;
		}

		public void HandleRevertAllVars(Toilet objectInstance) { }
		public void HandleObjectUpdate(Toilet objectInstance) { }
		public void HandlePlayerHasUsableItem(Toilet objectInstance, InvItem itemToTest, ref bool result) { }
		public void HandlePressedButton(Toilet objectInstance, string buttonText, int buttonPrice) { }
		public void HandleDetermineButtons(Toilet objectInstance) { }
		public void HandleFinishedOperating(Toilet objectInstance) { }
		public void HandleInteract(Toilet objectInstance, Agent agent) { }
		public void HandleObjectAction(Toilet objectInstance, string action, ref bool noMoreObjectActions, string extraString, float extraFloat, Agent causerAgent, PlayfieldObject extraObject) { }
		public void HandleDamagedObject(Toilet objectInstance, PlayfieldObject damagerObject, float damageAmount) { }
		public void HandleMakeNonFunctional(Toilet objectInstance, PlayfieldObject damagerObject) { }
		public void HandleDestroyMe(Toilet objectInstance, PlayfieldObject damagerObject) { }
		public void HandleDestroyMe3(Toilet objectInstance) { }
	}
}