using System.Collections.Generic;
using BepInEx.Logging;
using BunnyMod.Content.Extensions;
using BunnyMod.Content.Traits;
using Google2u;

namespace BunnyMod.Content.ObjectBehaviour
{
	public static class StoveController
	{
		private const int wrenchTamperCost = 30;
		
		private static readonly string loggerName = $"BunnyMod_{nameof(StoveController)}";
		private static ManualLogSource Logger => _logger ?? (_logger = BepInEx.Logging.Logger.CreateLogSource(loggerName));
		private static ManualLogSource _logger;

		private static readonly Dictionary<Stove, StoveData> stoveDictionary = new Dictionary<Stove, StoveData>();

		private static StoveData GetStoveData(Stove stove)
		{
			if (!stoveDictionary.ContainsKey(stove))
			{
				Logger.LogError($"stoveDictionary is missing stove #{stove.objectRealID}");
				return stoveDictionary[stove] = new StoveData();
			}
			return stoveDictionary[stove];
		}

		public static void RegisterDamagedBy(Stove stove, PlayfieldObject damagerObject)
		{
			GetStoveData(stove).savedDamagerObject = damagerObject;
		}

		public static void TriggerExplosion(Stove stove)
		{
			GameController gc = GameController.gameController;
			if (gc.serverPlayer && !stove.spawnedExplosion)
			{
				stove.spawnedExplosion = true;
				StoveData stoveData = GetStoveData(stove);
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

		public static void AddButtons(Stove stove)
		{
			if (stove.startedFlashing)
			{
				return;
			}

			Agent agent = stove.interactingAgent;
			if (agent.inventory.HasItem(ItemNameDB.rowIds.Wrench))
			{
				int invItemCount = agent.inventory.FindItem(ItemNameDB.rowIds.Wrench).invItemCount;
				int toolCost = BMTraitController.ApplyToolCostModifiers(agent, wrenchTamperCost);
				stove.AddButton(
						text: "UseWrenchToDetonate",
						extraText: $" ({invItemCount}) -{toolCost}"
				);
			}
		}
	}
}