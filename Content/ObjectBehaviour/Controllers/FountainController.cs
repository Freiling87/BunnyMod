using System.Collections.Generic;
using BunnyMod.Content.Extensions;
using Google2u;
using JetBrains.Annotations;
using RogueLibsCore;
using UnityEngine;

namespace BunnyMod.Content.ObjectBehaviour
{
	public class FountainController : ObjectControllerInterface<Fountain>
	{
		private const string FountainSteal_ButtonText = "FountainSteal";
		private const string FountainSteal_BarType = nameof(InterfaceNameDB.rowIds.Tampering);

		// TODO unused button-texts:
		/*
		 * FountainWishFabulousWealth = "FountainWishFabulousWealth",
		 * FountainWishFameAndGlory = "FountainWishFameAndGlory",
		 * FountainWishGoodHealth = "FountainWishGoodHealth",
		 * FountainWishTrueFriendship = "FountainWishTrueFriendship",
		 * FountainWishWorldPeace = "FountainWishWorldPeace",
		 *
		 * RogueLibs.CreateCustomName(cButtonText.FountainWishFabulousWealth, t, new CustomNameInfo("Wish for fabulous wealth"));
		 * RogueLibs.CreateCustomName(cButtonText.FountainWishFameAndGlory, t, new CustomNameInfo("Wish for fame & glory"));
		 * RogueLibs.CreateCustomName(cButtonText.FountainWishGoodHealth, t, new CustomNameInfo("Wish for good health"));
		 * RogueLibs.CreateCustomName(cButtonText.FountainWishTrueFriendship, t, new CustomNameInfo("Wish for true friendship"));
		 * RogueLibs.CreateCustomName(cButtonText.FountainWishWorldPeace, t, new CustomNameInfo("Wish for world peace"));
		 */

		private static readonly HashSet<Fountain> FountainsStolenFrom = new HashSet<Fountain>();

		[RLSetup, UsedImplicitly]
		public static void Setup()
		{
			// TODO create proper localization system for these buttons
			RogueLibs.CreateCustomName(FountainSteal_ButtonText, vNameType.Interface, new CustomNameInfo("Steal money"));
		}
		
		static FountainController()
		{
			ObjectControllerManager.ChangeLevelEvent += OnChangeLevel;
		}

		private static void OnChangeLevel()
		{
			FountainsStolenFrom.Clear();
		}

		public static void SetVars(Fountain fountain)
		{
			fountain.damageThreshold = 50;
			fountain.damageAccumulates = false;
			fountain.cantMakeFollowersAttack = true;
			FountainsStolenFrom.Remove(fountain);
		}

		private static void HandleFountainSteal(Fountain fountain)
		{
			GameController gc = GameController.gameController;
			Agent agent = fountain.interactingAgent;
			InvDatabase agentInventory = agent.inventory;

			InvItem fountainMoneyItem = new InvItem()
			{
					invItemName = nameof(ItemNameDB.rowIds.Money),
					invItemCount = Random.Range(10, 30)
			};
			fountainMoneyItem.ItemSetup(false);
			fountainMoneyItem.ShowPickingUpText(agent);
			agentInventory.AddItem(fountainMoneyItem);
			fountain.objectInvDatabase.DestroyAllItems();
			fountain.interactable = false;
			FountainsStolenFrom.Add(fountain);

			agent.statusEffects.AddStatusEffect(StatusEffectNameDB.rowIds.FeelingUnlucky, true, true);
			if (agent.HasTrait(StatusEffectNameDB.rowIds.OperateSecretly))
			{
				gc.audioHandler.Play(fountain, vAudioClip.JumpOutWater);
			}
			else
			{
				gc.spawnerMain.SpawnExplosion(fountain, fountain.curPosition, vExplosion.Water);
			}
		}

		public void HandleObjectUpdate(Fountain objectInstance) { }
		public void HandlePlayerHasUsableItem(Fountain objectInstance, InvItem itemToTest, ref bool result)
		{
		}

		public void HandlePressedButton(Fountain objectInstance, string buttonText, int buttonPrice)
		{
			GameController gc = GameController.gameController;
			Agent agent = objectInstance.interactingAgent;
			if (buttonText == FountainSteal_ButtonText)
			{
				objectInstance.StartCoroutine(objectInstance.Operating(agent, null, 2f, false, FountainSteal_BarType));
				gc.audioHandler.Play(objectInstance, vAudioClip.JumpIntoWater);

				if (!agent.HasTrait(StatusEffectNameDB.rowIds.OperateSecretly))
				{
					gc.spawnerMain.SpawnNoise(objectInstance.tr.position, 0.4f, agent, "Normal", agent);
					gc.audioHandler.Play(objectInstance, vAudioClip.Operating);
					gc.spawnerMain.SpawnStateIndicator(objectInstance, "HighVolume");
					gc.OwnCheck(agent, objectInstance.go, "Normal", 2);
					BMAgents.AnnoyWitnessesVictimless(agent);
				}

				objectInstance.StopInteraction();
			}
		}

		public void HandleDetermineButtons(Fountain objectInstance)
		{
			if (!FountainsStolenFrom.Contains(objectInstance))
			{
				objectInstance.AddButton(FountainSteal_ButtonText);
			}
		}

		public void HandleFinishedOperating(Fountain objectInstance)
		{
			if (objectInstance.operatingBarType == FountainSteal_BarType)
			{
				HandleFountainSteal(objectInstance);
				objectInstance.StopInteraction();
			}
		}

		public void HandleInteract(Fountain objectInstance, Agent agent) { }
		public void HandleObjectAction(Fountain objectInstance, string action, ref bool noMoreObjectActions) { }
		public void HandleDamagedObject(Fountain objectInstance, PlayfieldObject damagerObject, float damageAmount) { }
		public void HandleMakeNonFunctional(Fountain objectInstance, PlayfieldObject damagerObject) { }
		public void HandleDestroyMe(Fountain objectInstance, PlayfieldObject damagerObject) { }
		public void HandleDestroyMe3(Fountain objectInstance) { }
	}
}