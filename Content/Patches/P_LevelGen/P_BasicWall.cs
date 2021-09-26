using BepInEx.Logging;
using BunnyMod.Content.Logging;
using BunnyMod.Content.Traits;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;
using Random = UnityEngine.Random;
using System.Collections;
using System.Reflection;
using System;

namespace BunnyMod.Content.Patches
{
	[HarmonyPatch(declaringType: typeof(BasicWall))]
	public static class P_BasicWall
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		public static GameController GC => GameController.gameController;

		[HarmonyPrefix,HarmonyPatch(methodName:nameof(BasicWall.Spawn), argumentTypes: new[] { typeof(SpawnerBasic), typeof(string), typeof(Vector2), typeof(Vector2), typeof(Chunk) })]
		public static void Spawn_Prefix(SpawnerBasic spawner, ref string wallName, Vector2 myPos, Vector2 myScale, Chunk startingChunkReal) 
		{
			if (BMLevelGen.IsWallModActive())
				wallName = BMLevelGen.GetWallTypeFromMutator();
		}

		[HarmonyPostfix,HarmonyPatch(methodName:nameof(BasicWall.Spawn), argumentTypes: new[] { typeof(SpawnerBasic), typeof(string), typeof(Vector2), typeof(Vector2), typeof(Chunk) })]
		public static void Spawn_Postfix(SpawnerBasic spawner, string wallName, Vector2 myPos, Vector2 myScale, Chunk startingChunkReal) 
		{
			// FloralerFlora Hedge Wall leaves spawn

			if (wallName == vWall.Hedge && (GC.challenges.Contains(cChallenge.FloralerFlora)))
			{
				int chance = 100;

				while (GC.percentChance(chance))
				{
					GC.spawnerMain.SpawnWreckagePileObject(new Vector2(myPos.x + Random.Range(-0.48f, 0.48f), myPos.y + Random.Range(-0.48f, 0.48f)),
							vObject.Bush, false);
					chance -= 20;
				}
			}
		}
	}
}
