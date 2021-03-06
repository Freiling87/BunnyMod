﻿using System;
using System.Collections.Generic;
using System.Linq;
using RogueLibsCore;
using UnityEngine;
using System.Collections;
using HarmonyLib;
using System.Reflection;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;
using Light2D;

namespace BunnyMod.Content
{
	public class BMLevelGen
	{
		public static GameController GC => GameController.gameController;
		public static bool Prefix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) => BMHeader.MainInstance.PatchPrefix(type, methodName, patchType, patchMethodName, types);
		public static bool Postfix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) => BMHeader.MainInstance.PatchPostfix(type, methodName, patchType, patchMethodName, types);
		public static void BMLog(string logMessage) => BMHeader.Log(logMessage);

		public void Awake()
		{
			BasicFloor_00();
			BasicWall_00();
			LoadLevel_00();
			RandomWalls_00();
			SpawnerFloor_00();
			SpawnerMain_00();
			SpawnerObject_00();
			TileInfo_00();
		}
		#region Custom
		public static int GangCount(int vanilla)
		{
			BMLog("SetGangCount");

			if (GC.challenges.Contains(cChallenge.HoodlumsWonderland))
				vanilla = 12;

			return vanilla;
		}
		public static int GenPopCount(int vanilla)
		{
			if (GC.challenges.Contains(cChallenge.GhostTown))
				vanilla *= 0;
			else if (GC.challenges.Contains(cChallenge.HordeAlmighty))
				vanilla *= 2;
			else if (GC.challenges.Contains(cChallenge.LetMeSeeThatThrong))
				vanilla *= 4;
			else if (GC.challenges.Contains(cChallenge.SwarmWelcome))
				vanilla *= 8;

			return vanilla;
		}
		public static string GetFloorTile()
		{
			string curMutator = "";

			foreach (string mutator in cChallenge.AffectsFloors)
				if (GC.challenges.Contains(mutator))
					curMutator = mutator;

			switch (curMutator)
			{
				case cChallenge.ArcologyEcology:
					return vFloor.Grass;
				case cChallenge.DiscoCityDanceoff:
					return vFloor.DanceFloor;
				default:
					return null;
			}
		}
		public static string GetFloorTileGroup()
		{
			string curMutator = "";

			foreach (string mutator in cChallenge.AffectsFloors)
				if (GC.challenges.Contains(mutator))
					curMutator = mutator;

			switch (curMutator)
			{
				case cChallenge.ArcologyEcology:
					return vFloorTileGroup.Park;
				case cChallenge.DiscoCityDanceoff:
					return vFloorTileGroup.Uptown;
				default:
					return vFloorTileGroup.Building;
			}
		}
		public static string GetWallMutator()
		{
			foreach (string mutator in GC.challenges)
				if (cChallenge.Walls.Contains(mutator))
					return mutator;

			return null;
		}
		public static wallMaterialType GetBorderWallMaterialFromMutator()
		{
			BMLog("GetWallBorderTypeFromMutator: '" + GetWallMutator() + "'");

			switch (GetWallMutator())
			{
				case cChallenge.CityOfSteel:
					return wallMaterialType.Steel;

				case cChallenge.GreenLiving:
					return wallMaterialType.Wood;

				case cChallenge.Panoptikopolis:
					return wallMaterialType.Glass;

				case cChallenge.ShantyTown:
					return wallMaterialType.Wood;

				case cChallenge.SpelunkyDory:
					return wallMaterialType.Border;

				default:
					return wallMaterialType.Border;
			}
		}
		public static string GetWallTypeFromMutator()
		{
			BMLog("GetWallTypeFromMutator: '" + GetWallMutator() + "'");

			switch (GetWallMutator())
			{
				case cChallenge.CityOfSteel:
					return vWall.Steel;

				case cChallenge.GreenLiving:
					return vWall.Hedge;

				case cChallenge.Panoptikopolis:
					return vWall.Glass;

				case cChallenge.ShantyTown:
					return vWall.Wood;

				case cChallenge.SpelunkyDory:
					return vWall.Cave;
			}

			return null;
		}
		public static bool IsNextToLake(Vector2 spot) =>
			GC.tileInfo.GetTileData(new Vector2(spot.x, spot.y + 0.64f)).lake ||
			GC.tileInfo.GetTileData(new Vector2(spot.x + 0.64f, spot.y + 0.64f)).lake ||
			GC.tileInfo.GetTileData(new Vector2(spot.x + 0.64f, spot.y + 0.64f)).lake ||
			GC.tileInfo.GetTileData(new Vector2(spot.x + 0.64f, spot.y)).lake ||
			GC.tileInfo.GetTileData(new Vector2(spot.x, spot.y - 0.64f)).lake ||
			GC.tileInfo.GetTileData(new Vector2(spot.x - 0.64f, spot.y - 0.64f)).lake ||
			GC.tileInfo.GetTileData(new Vector2(spot.x - 0.64f, spot.y - 0.64f)).lake ||
			GC.tileInfo.GetTileData(new Vector2(spot.x - 0.64f, spot.y)).lake;
		public static string GetActiveFloorMod()
		{
			foreach (string mutator in cChallenge.Walls)
				if (GC.challenges.Contains(mutator))
					return mutator;

			return null;
		}
		public static bool IsWallModActive()
		{
			foreach (string mutator in cChallenge.Walls)
				if (GC.challenges.Contains(mutator))
					return true;

			return false;
		}
		public static int LevelSizeModifier(int vanilla)
		{
			if (GC.challenges.Contains(cChallenge.ACityForAnts))
				vanilla = 4;
			else if (GC.challenges.Contains(cChallenge.Claustropolis))
				vanilla = 12;
			else if (GC.challenges.Contains(cChallenge.Megalopolis))
				vanilla = 48;
			else if (GC.challenges.Contains(cChallenge.Ultrapolis))
				vanilla = 64;

			return vanilla;
		}
		public static int LevelSizeRatio()
		{
			return LevelSizeModifier(30) / 30;
		}
		public static int MafiaCount(int vanilla)
		{
			return vanilla;
		}
		public static int QuestCount(int vanilla)
		{
			if (GC.challenges.Contains(cChallenge.RushinRevolution))
				vanilla = 0;
			else if (GC.challenges.Contains(cChallenge.SingleMinded))
				vanilla = 1;
			else if (GC.challenges.Contains(cChallenge.Workhorse))
				vanilla = 4;

			return vanilla;
		}
		#endregion

		#region BasicFloor
		public void BasicFloor_00()
		{
			Type t = typeof(BasicFloor);
			Type g = GetType();

			Prefix(t, "Spawn", g, "BasicFloor_Spawn", new Type[5] { typeof(SpawnerBasic), typeof(string), typeof(Vector2), typeof(Vector2), typeof(Chunk) });
		}
		public static bool BasicFloor_Spawn(SpawnerBasic spawner, ref string floorName, Vector2 myPos, Vector2 myScale, Chunk startingChunkReal) // Prefix
		{
			// Modifies Interior Floors only

			if (GetActiveFloorMod() != null)
			{
				if (vFloor.Natural.Contains(floorName))
				{
					if (GC.challenges.Contains(cChallenge.GreenLiving))
						floorName = vFloor.Grass;
					else if (GC.challenges.Contains(cChallenge.SpelunkyDory))
						floorName = vFloor.CaveFloor;
				}
				else if (vFloor.Rugs.Contains(floorName))
				{
					if (GC.challenges.Contains(cChallenge.DiscoCityDanceoff)) // Overrides some non-exclusive challenges
						floorName = vFloor.CasinoFloor;
					else if (GC.challenges.Contains(cChallenge.CityOfSteel))
						floorName = vFloor.MetalPlates;
					else if (GC.challenges.Contains(cChallenge.GreenLiving))
						floorName = vFloor.Grass;
					else if (GC.challenges.Contains(cChallenge.Panoptikopolis))
						floorName = vFloor.ClearFloor;
					else if (GC.challenges.Contains(cChallenge.SpelunkyDory))
						floorName = vFloor.DirtFloor;
				}
				else if (vFloor.Constructed.Contains(floorName))
				{
					if (GC.challenges.Contains(cChallenge.DiscoCityDanceoff)) // Overrides some non-exclusive challenges
						floorName = vFloor.BathroomTile;
					else if (GC.challenges.Contains(cChallenge.CityOfSteel))
						floorName = vFloor.MetalFloor;
					else if (GC.challenges.Contains(cChallenge.GreenLiving))
						floorName = vFloor.DirtFloor;
					else if (GC.challenges.Contains(cChallenge.Panoptikopolis))
						floorName = vFloor.CleanTiles;
					else if (GC.challenges.Contains(cChallenge.ShantyTown))
						floorName = vFloor.DrugDenFloor;
					else if (GC.challenges.Contains(cChallenge.SpelunkyDory))
						floorName = vFloor.CaveFloor;
				}
				else if (vFloor.Raised.Contains(floorName))
				{
					if (GC.challenges.Contains(cChallenge.DiscoCityDanceoff)) // Overrides some non-exclusive challenges
						floorName = vFloor.DanceFloorRaised;
					else if (GC.challenges.Contains(cChallenge.CityOfSteel))
						floorName = vFloor.SolidPlates;
					else if (GC.challenges.Contains(cChallenge.GreenLiving))
						floorName = vFloor.CaveFloor;
					else if (GC.challenges.Contains(cChallenge.Panoptikopolis))
						floorName = vFloor.CleanTilesRaised;
					else if (GC.challenges.Contains(cChallenge.ShantyTown))
						floorName = vFloor.DirtyTiles;
					else if (GC.challenges.Contains(cChallenge.SpelunkyDory))
						floorName = vFloor.Grass;
				}
			}

			return true;
		}
		#endregion
		#region BasicWall
		public void BasicWall_00()
		{
			Type t = typeof(BasicWall);
			Type g = GetType();

			Prefix(t, "Spawn", g, "BasicWall_Spawn_Prefix", new Type[5] { typeof(SpawnerBasic), typeof(string), typeof(Vector2), typeof(Vector2), typeof(Chunk) });
			Postfix(t, "Spawn", g, "BasicWall_Spawn_Postfix", new Type[5] { typeof(SpawnerBasic), typeof(string), typeof(Vector2), typeof(Vector2), typeof(Chunk) });
		}
		public static void BasicWall_Spawn_Prefix(SpawnerBasic spawner, ref string wallName, Vector2 myPos, Vector2 myScale, Chunk startingChunkReal) // Prefix
		{
			if (IsWallModActive())
				wallName = GetWallTypeFromMutator();
		}
		public static void BasicWall_Spawn_Postfix(SpawnerBasic spawner, string wallName, Vector2 myPos, Vector2 myScale, Chunk startingChunkReal) // Postfix
		{
			// FloralerFlora Hedge Wall leaves spawn

			if (wallName == vWall.Hedge && (GC.challenges.Contains(cChallenge.FloralerFlora)))
			{
				int chance = 100;

				while (GC.percentChance(chance))
				{
					GC.spawnerMain.SpawnWreckagePileObject(new Vector2(myPos.x + Random.Range(-0.48f, 0.48f), myPos.y + Random.Range(-0.48f, 0.48f)), vObject.Bush, false);
					chance -= 20;
				}
			}

		}
		#endregion
		#region LoadLevel
		public void LoadLevel_00()
		{
			Type t = typeof(LoadLevel);
			Type g = GetType();

			Prefix(t, "CreateInitialMap", g, "LoadLevel_CreateInitialMap", new Type[0] { });
			Prefix(t, "FillFloors", g, "LoadLevel_FillFloors_Prefix", new Type[0] { });
			Prefix(t, "FillMapChunks", g, "LoadLevel_FillMapChunks_Prefix", new Type[0] { });
			Prefix(t, "loadStuff2", g, "LoadLevel_loadStuff2_Prefix", new Type[0] { });
			Prefix(t, "LoadBasicLEvel", g, "LoadLevel_SetupBasicLevel", new Type[0] { });
			Prefix(t, "SetupMore3_3", g, "LoadLevel_SetupMore3_3_Prefix", new Type[0] { });
			Postfix(t, "SetupMore3_3", g, "LoadLevel_SetupMore3_3_Postfix", new Type[0] { });
			Postfix(t, "SetupMore5_2", g, "LoadLevel_SetupMore5_2", new Type[0] { });
			Prefix(t, "SetupRels", g, "LoadLevel_SetupRels", new Type[0] { });
		}
		public static bool LoadLevel_CreateInitialMap(LoadLevel __instance, ref bool ___placedKey1, ref bool ___placedKey2, ref bool ___placedKey3) // Replacement
		{
			// Set Custom Quest Count
			// Set Custom Map Size 

			BMLog("LoadLevel_CreateInitialMap");

			__instance.levelSizeMax = 30;
			__instance.levelSizeAxis = 10;

			if (GC.customLevel && !GC.customRandomLevel)
			{
				int tilesFilled = 0;

				foreach (LevelEditorTileChunk levelEditorTileChunk in __instance.customLevel.chunkTiles)
				{
					if (levelEditorTileChunk.tileFilled)
					{
						tilesFilled++;
						__instance.levelSize++;
						int x = (int)levelEditorTileChunk.posX;
						int y = (int)(9f - levelEditorTileChunk.posY);

						if (levelEditorTileChunk.tileName == "Randomized11" || levelEditorTileChunk.tileName == "Randomized12" || levelEditorTileChunk.tileName == "Randomized22")
						{
							__instance.mapChunkArray[x, y].randomTile = true;
							__instance.mapChunkArray[x, y].customLevelChunk = false;
						}
						else if (levelEditorTileChunk.tileName == "Empty11" || levelEditorTileChunk.tileName == "Empty12" || levelEditorTileChunk.tileName == "Empty22")
						{
							__instance.mapChunkArray[x, y].randomTile = false;
							__instance.mapChunkArray[x, y].customLevelChunk = false;
						}
						else
						{
							__instance.mapChunkArray[x, y].randomTile = false;
							__instance.mapChunkArray[x, y].customLevelChunk = true;
						}

						__instance.mapChunkArray[x, y].chunkID = tilesFilled;
						__instance.mapChunkArray[x, y].chunkMainTile = levelEditorTileChunk.chunkMainTile;
						__instance.mapChunkArray[x, y].specificQuest = levelEditorTileChunk.specificQuest;
						__instance.mapChunkArray[x, y].chunkName = levelEditorTileChunk.tileName;
						__instance.mapChunkArray[x, y].chunkShape = levelEditorTileChunk.chunkShape;

						if (levelEditorTileChunk.directionX == 0)
							__instance.mapChunkArray[x, y].chunkDirX = "Normal";
						else
							__instance.mapChunkArray[x, y].chunkDirX = "Reverse";

						if (levelEditorTileChunk.directionY == 0)
							__instance.mapChunkArray[x, y].chunkDirY = "Normal";
						else
							__instance.mapChunkArray[x, y].chunkDirY = "Reverse";

						__instance.mapChunkArray[x, y].chunkRotate = levelEditorTileChunk.rotation;

						Debug.Log(string.Concat(new object[] { "Fill Custom Level Tile: ", __instance.mapChunkArray[x, y].chunkName, " - ", levelEditorTileChunk.directionX, " - ", __instance.mapChunkArray[x, y].chunkDirX, " - ", levelEditorTileChunk.rotation }));
					}
				}

				for (int x = 0; x < __instance.levelSizeAxis; x++)
					for (int y = 0; y < __instance.levelSizeAxis; y++)
						if (__instance.mapChunkArray[x, y].chunkID != 0)
						{
							if (x > 0)
							{
								if (__instance.mapChunkArray[x - 1, y].chunkID != 0)
									__instance.mapChunkArray[x, y].exitW = true;
								else
									__instance.mapChunkArray[x, y].exitW = false;
							}
							else
								__instance.mapChunkArray[x, y].exitW = false;

							if (x < __instance.levelSizeAxis - 1)
							{
								if (__instance.mapChunkArray[x + 1, y].chunkID != 0)
									__instance.mapChunkArray[x, y].exitE = true;
								else
									__instance.mapChunkArray[x, y].exitE = false;
							}
							else
								__instance.mapChunkArray[x, y].exitE = false;

							if (y > 0)
							{
								if (__instance.mapChunkArray[x, y - 1].chunkID != 0)
									__instance.mapChunkArray[x, y].exitN = true;
								else
									__instance.mapChunkArray[x, y].exitN = false;
							}
							else
								__instance.mapChunkArray[x, y].exitN = false;

							if (y < __instance.levelSizeAxis - 1)
							{
								if (__instance.mapChunkArray[x, y + 1].chunkID != 0)
									__instance.mapChunkArray[x, y].exitS = true;
								else
									__instance.mapChunkArray[x, y].exitS = false;
							}
							else
								__instance.mapChunkArray[x, y].exitS = false;
						}

				Debug.Log("LEVEL SIZE: " + tilesFilled);
				BMLog("Here B");

				switch (GC.sessionDataBig.curLevelEndless)
				{
					case 1:
						__instance.levelSizeMax = 20;
						GC.quests.questTriesTotal = 2;
						break;
					case 2:
						__instance.levelSizeMax = 30;
						GC.quests.questTriesTotal = 4;
						break;
					case 3:
						__instance.levelSizeMax = 24;
						GC.quests.questTriesTotal = 2;
						break;
					case 4:
						__instance.levelSizeMax = 20;
						GC.quests.questTriesTotal = 2;
						break;
					case 5:
						__instance.levelSizeMax = 30;
						GC.quests.questTriesTotal = 4;
						break;
					case 6:
						__instance.levelSizeMax = 24;
						GC.quests.questTriesTotal = 2;
						break;
					case 7:
						__instance.levelSizeMax = 20;
						GC.quests.questTriesTotal = 2;
						break;
					case 8:
						__instance.levelSizeMax = 30;
						GC.quests.questTriesTotal = 4;
						break;
					case 9:
						__instance.levelSizeMax = 24;
						GC.quests.questTriesTotal = 2;
						break;
					case 10:
						__instance.levelSizeMax = 20;
						GC.quests.questTriesTotal = 2;
						break;
					case 11:
						__instance.levelSizeMax = 30;
						GC.quests.questTriesTotal = 4;
						break;
					case 12:
						__instance.levelSizeMax = 24;
						GC.quests.questTriesTotal = 3;
						break;
					case 13:
						__instance.levelSizeMax = 20;
						GC.quests.questTriesTotal = 2;
						break;
					case 14:
						__instance.levelSizeMax = 30;
						GC.quests.questTriesTotal = 4;
						break;
					case 15:
						__instance.levelSizeMax = 24;
						GC.quests.questTriesTotal = 3;
						break;
					case 16:
						__instance.levelSizeMax = 30;
						break;
				}

				if (GC.fourPlayerMode && GC.quests.questTriesTotal < 3)
					GC.quests.questTriesTotal = 3;

				if (!__instance.customLevel.randomizeQuests)
				{
					GC.quests.questTriesTotal = 4;

					return false;
				}
			}
			else if (GC.levelShape == 0)
			{
				if (!__instance.quickGame)
					switch (GC.sessionDataBig.curLevelEndless)
					{
						case 1:
							__instance.levelSizeMax = 20;
							GC.quests.questTriesTotal = 2;
							break;
						case 2:
							__instance.levelSizeMax = 30;
							GC.quests.questTriesTotal = 4;
							break;
						case 3:
							__instance.levelSizeMax = 24;
							GC.quests.questTriesTotal = 2;
							break;
						case 4:
							__instance.levelSizeMax = 20;
							GC.quests.questTriesTotal = 2;
							break;
						case 5:
							__instance.levelSizeMax = 30;
							GC.quests.questTriesTotal = 4;
							break;
						case 6:
							__instance.levelSizeMax = 24;
							GC.quests.questTriesTotal = 2;
							break;
						case 7:
							__instance.levelSizeMax = 20;
							GC.quests.questTriesTotal = 2;
							break;
						case 8:
							__instance.levelSizeMax = 30;
							GC.quests.questTriesTotal = 4;
							break;
						case 9:
							__instance.levelSizeMax = 24;
							GC.quests.questTriesTotal = 2;
							break;
						case 10:
							__instance.levelSizeMax = 20;
							GC.quests.questTriesTotal = 2;
							break;
						case 11:
							__instance.levelSizeMax = 30;
							GC.quests.questTriesTotal = 4;
							break;
						case 12:
							__instance.levelSizeMax = 24;
							GC.quests.questTriesTotal = 3;
							break;
						case 13:
							__instance.levelSizeMax = 20;
							GC.quests.questTriesTotal = 2;
							break;
						case 14:
							__instance.levelSizeMax = 30;
							GC.quests.questTriesTotal = 4;
							break;
						case 15:
							__instance.levelSizeMax = 24;
							GC.quests.questTriesTotal = 3;
							break;
						case 16:
							__instance.levelSizeMax = 30;
							break;
					}
				else if (__instance.quickGame)
					switch (GC.sessionDataBig.curLevelEndless)
					{
						case 1:
							__instance.levelSizeMax = 20;
							GC.quests.questTriesTotal = 2;
							break;
						case 2:
							__instance.levelSizeMax = 30;
							GC.quests.questTriesTotal = 4;
							break;
						case 3:
							__instance.levelSizeMax = 20;
							GC.quests.questTriesTotal = 2;
							break;
						case 4:
							__instance.levelSizeMax = 24;
							GC.quests.questTriesTotal = 2;
							break;
						case 5:
							__instance.levelSizeMax = 20;
							GC.quests.questTriesTotal = 2;
							break;
						case 6:
							__instance.levelSizeMax = 30;
							GC.quests.questTriesTotal = 4;
							break;
						case 7:
							__instance.levelSizeMax = 20;
							GC.quests.questTriesTotal = 2;
							break;
						case 8:
							__instance.levelSizeMax = 24;
							GC.quests.questTriesTotal = 2;
							break;
						case 9:
							__instance.levelSizeMax = 30;
							GC.quests.questTriesTotal = 4;
							break;
						case 10:
							__instance.levelSizeMax = 24;
							GC.quests.questTriesTotal = 2;
							break;
						case 11:
							__instance.levelSizeMax = 30;
							break;
					}

				if (GC.levelTheme == 2 && __instance.levelSizeMax < 24)
					__instance.levelSizeMax = 24;

				if (GC.levelTheme == 3)
				{
					if (__instance.levelSizeMax == 20)
						__instance.levelSizeMax = 40;
					else if (__instance.levelSizeMax == 30)
						__instance.levelSizeMax = 50;
					else if (__instance.levelSizeMax == 24)
						__instance.levelSizeMax = 44;
				}

				if (GC.levelTheme == 4)
				{
					if (__instance.levelSizeMax == 20)
						__instance.levelSizeMax = 30;
					else if (__instance.levelSizeMax == 30)
						__instance.levelSizeMax = 40;
					else if (__instance.levelSizeMax == 24)
						__instance.levelSizeMax = 34;
				}

				if (GC.levelTheme == 5)
					__instance.levelSizeMax = 30;

				if (GC.oneTimeVersion)
				{
					if (GC.levelTheme == 3)
						__instance.levelSizeMax = 50;
					else
						__instance.levelSizeMax = 30;

					GC.quests.questTriesTotal = 4;
				}

				if (GC.arcadeMode && GC.quests.questTriesTotal == 4)
					GC.quests.questTriesTotal = 3;

				if (GC.fourPlayerMode && GC.quests.questTriesTotal < 3)
					GC.quests.questTriesTotal = 3;

				GC.quests.questTriesTotal = QuestCount(GC.quests.questTriesTotal);

				if (__instance.squareMap)
				{
					__instance.mapChunkArray[2, 2].chunkID = 1;
					__instance.mapChunkArray[3, 2].chunkID = 2;
					__instance.mapChunkArray[4, 2].chunkID = 3;
					__instance.mapChunkArray[5, 2].chunkID = 4;
					__instance.mapChunkArray[6, 2].chunkID = 5;
					__instance.mapChunkArray[7, 2].chunkID = 6;
					__instance.levelNorth = 2;
					__instance.levelSouth = 2;
					__instance.levelEast = 2;
					__instance.levelWest = 2;
					__instance.levelSize = 6;
				}
				else
				{
					__instance.mapChunkArray[5, 5].chunkID = 1;
					__instance.levelSize++;
				}

				int randA = Random.Range(3, 7);
				int randB = Random.Range(3, 7);

				if (GC.levelTheme == 3)
				{
					for (int k = 0; k < 10; k++)
					{
						int num6 = k;
						int num7 = randA;
						MapChunk mapChunk = __instance.mapChunkArray[num6, num7];
						mapChunk.chunkID = __instance.levelSize + 1;

						if (k == 0)
							mapChunk.special = "CanalEndW";
						else if (k == 9)
							mapChunk.special = "CanalEndE";
						else
							mapChunk.special = "Canal";

						if (num7 > 5)
							__instance.levelSouth = num7;
						else if (num7 < 5)
							__instance.levelNorth = num7;

						if (num7 != 9)
						{
							__instance.mapChunkArray[num6, num7].exitN = true;
							__instance.mapChunkArray[num6, num7 + 1].exitS = true;
						}

						if (num7 != 0)
						{
							__instance.mapChunkArray[num6, num7].exitS = true;
							__instance.mapChunkArray[num6, num7 - 1].exitN = true;
						}

						if (num6 != 9)
						{
							__instance.mapChunkArray[num6, num7].exitE = true;
							__instance.mapChunkArray[num6 + 1, num7].exitW = true;
						}

						if (num6 != 0)
						{
							__instance.mapChunkArray[num6, num7].exitW = true;
							__instance.mapChunkArray[num6 - 1, num7].exitE = true;
						}

						__instance.levelSize++;
					}

					for (int l = 0; l < 10; l++)
					{
						int num8 = randB;
						int num9 = l;
						MapChunk mapChunk2 = __instance.mapChunkArray[num8, num9];
						mapChunk2.chunkID = __instance.levelSize + 1;

						if (l == randA)
							mapChunk2.special = "CanalCross";
						else if (l == 0)
							mapChunk2.special = "CanalEndN";
						else if (l == 9)
							mapChunk2.special = "CanalEndS";
						else
							mapChunk2.special = "CanalNS";

						if (num8 > 5)
							__instance.levelEast = num8;
						else if (num8 < 5)
							__instance.levelWest = num8;

						if (num9 != 9)
						{
							__instance.mapChunkArray[num8, num9].exitN = true;
							__instance.mapChunkArray[num8, num9 + 1].exitS = true;
						}
						if (num9 != 0)
						{
							__instance.mapChunkArray[num8, num9].exitS = true;
							__instance.mapChunkArray[num8, num9 - 1].exitN = true;
						}
						if (num8 != 9)
						{
							__instance.mapChunkArray[num8, num9].exitE = true;
							__instance.mapChunkArray[num8 + 1, num9].exitW = true;
						}
						if (num8 != 0)
						{
							__instance.mapChunkArray[num8, num9].exitW = true;
							__instance.mapChunkArray[num8 - 1, num9].exitE = true;
						}

						__instance.levelSize++;
					}
				}

				__instance.levelSizeMax = LevelSizeModifier(__instance.levelSizeMax);

				Debug.Log("LEVEL SIZE: " + __instance.levelSizeMax);
				BMLog("Here A");

				if (__instance.squareMap)
					for (int m = 2; m < 8; m++)
						for (int n = 2; n < 7; n++)
						{
							string chunkExit = "S";

							if (__instance.PlaceChunk(chunkExit, m, n))
							{
								__instance.levelSize++;
								_ = __instance.mapChunkArray[m, n];
							}
						}
				else
				{
					int num10 = 0;

					while (__instance.levelSize < __instance.levelSizeMax)
					{
						int randC = Random.Range(1, 9);
						int randD = Random.Range(1, 9);

						string chunkExit2 = GC.Choose<string>("N", "S", new string[]
						{
							"E",
							"W"
						});

						if (__instance.PlaceChunk(chunkExit2, randC, randD))
						{
							__instance.levelSize++;
							num10 = 0;
							_ = __instance.mapChunkArray[randC, randD];
						}
						else
							num10++;
					}
				}

				if (__instance.levelNorth == 0)
					__instance.levelNorth = 1;

				if (__instance.levelSouth == 9)
					__instance.levelSouth = 8;

				bool flag = false;
				bool flag2 = false;
				List<int> list = new List<int>();
				List<int> list2 = new List<int>();
				List<int> list3 = new List<int>();
				List<int> list4 = new List<int>();

				for (int num13 = 1; num13 < __instance.levelSizeMax + 1; num13++)
					for (int num14 = 0; num14 < __instance.levelSizeAxis; num14++)
						for (int num15 = 0; num15 < __instance.levelSizeAxis; num15++)
							if (__instance.mapChunkArray[num14, num15].chunkID == num13 && (GC.levelTheme != 3 || randB != num14))
							{
								if (num15 == __instance.levelNorth && !flag2)
								{
									list3.Add(num14);
									list4.Add(num15);
								}
								else if (num15 == __instance.levelSouth && !flag)
								{
									list.Add(num14);
									list2.Add(num15);
								}
							}

				if (!__instance.LevelContainsMayor())
				{
					int index = Random.Range(0, list3.Count);
					__instance.mapChunkArray[list3[index], list4[index]].exitN = true;
					MapChunk mapChunk3 = __instance.mapChunkArray[list3[index], list4[index] - 1];
					mapChunk3.chunkID = __instance.levelSize + 1;
					mapChunk3.exitS = true;
					mapChunk3.special = "LevelEnd";
					__instance.levelSize++;
				}

				int index2 = Random.Range(0, list.Count);
				__instance.mapChunkArray[list[index2], list2[index2]].exitS = true;
				MapChunk mapChunk4 = __instance.mapChunkArray[list[index2], list2[index2] + 1];
				mapChunk4.chunkID = __instance.levelSize + 1;
				mapChunk4.exitN = true;
				mapChunk4.special = "LevelStart";
				__instance.levelSize++;

				if (GC.levelTheme == 3)
				{
					int num16 = -1;
					int num17 = -10;
					int num18 = 0;

					for (int num19 = 0; num19 < 2; num19++)
					{
						do
						{
							int num20 = Random.Range(0, 10);

							if (__instance.mapChunkArray[num20, randA - 1].chunkID != 0 && __instance.mapChunkArray[num20, randA + 1].chunkID != 0 && Mathf.Abs(num20 - num17) > 2 && __instance.mapChunkArray[num20, randA].special == "Canal")
								num16 = num20;

							num18++;
						}
						while (num16 == -1 && num18 < 100);

						if (num16 == -1)
						{
							num18 = 0;

							do
							{
								int num21 = Random.Range(0, 10);

								if (__instance.mapChunkArray[num21, randA - 1].chunkID != 0 && Mathf.Abs(num21 - num17) > 2 && __instance.mapChunkArray[num21, randA].special == "Canal")
									num16 = num21;

								num18++;
							}
							while (num16 == -1 && num18 < 100);
						}

						if (num16 == -1)
						{
							num18 = 0;

							do
							{
								int num22 = Random.Range(0, 10);

								if (__instance.mapChunkArray[num22, randA + 1].chunkID != 0 && Mathf.Abs(num22 - num17) > 2 && __instance.mapChunkArray[num22, randA].special == "Canal")
									num16 = num22;

								num18++;
							}
							while (num16 == -1 && num18 < 100);
						}

						if (num16 == -1)
						{
							num18 = 0;

							do
							{
								int num23 = Random.Range(0, 10);

								if (Mathf.Abs(num23 - num17) > 2 && __instance.mapChunkArray[num23, randA].special == "Canal")
									num16 = num23;

								num18++;
							}
							while (num16 == -1 && num18 < 100);
						}

						if (num16 != -1)
							__instance.mapChunkArray[num16, randA].special = "CanalBridge";

						if (num17 == -10)
							num17 = num16;

						num16 = -1;
						num18 = 0;
					}

					int num24 = -1;
					int num25 = -10;
					num18 = 0;

					for (int num26 = 0; num26 < 2; num26++)
					{
						do
						{
							int num27 = Random.Range(0, 10);

							if (__instance.mapChunkArray[randB - 1, num27].chunkID != 0 && __instance.mapChunkArray[randB + 1, num27].chunkID != 0 && Mathf.Abs(num27 - num25) > 2 && __instance.mapChunkArray[randB, num27].special == "CanalNS")
								num24 = num27;

							num18++;
						}
						while (num24 == -1 && num18 < 100);

						if (num24 == -1)
						{
							num18 = 0;

							do
							{
								int num28 = Random.Range(0, 10);

								if (__instance.mapChunkArray[randB - 1, num28].chunkID != 0 && Mathf.Abs(num28 - num25) > 2 && __instance.mapChunkArray[randB, num28].special == "CanalNS")
									num24 = num28;

								num18++;
							}
							while (num24 == -1 && num18 < 100);
						}

						if (num24 == -1)
						{
							num18 = 0;

							do
							{
								int num29 = Random.Range(0, 10);

								if (__instance.mapChunkArray[randB + 1, num29].chunkID != 0 && Mathf.Abs(num29 - num25) > 2 && __instance.mapChunkArray[randB, num29].special == "CanalNS")
									num24 = num29;

								num18++;
							}
							while (num24 == -1 && num18 < 100);
						}

						if (num24 == -1)
						{
							num18 = 0;

							do
							{
								int num30 = Random.Range(0, 10);

								if (Mathf.Abs(num30 - num25) > 2 && __instance.mapChunkArray[randB, num30].special == "CanalNS")
									num24 = num30;

								num18++;
							}
							while (num24 == -1 && num18 < 100);
						}

						if (num24 != -1)
							__instance.mapChunkArray[randB, num24].special = "CanalBridgeNS";

						if (num25 == -10)
							num25 = num24;

						num24 = -1;
						num18 = 0;
					}
					return false;
				}
			}
			else if (GC.levelShape == 2)
			{
				bool flag3 = false;

				while (!flag3)
				{
					__instance.mapChunkArray = new MapChunk[__instance.levelSizeAxis, __instance.levelSizeAxis];

					for (int num31 = 0; num31 < __instance.levelSizeAxis; num31++)
						for (int num32 = 0; num32 < __instance.levelSizeAxis; num32++)
							__instance.mapChunkArray[num31, num32] = new MapChunk();

					int num33 = 5;
					int num34 = 9;
					int num35 = 0;
					__instance.levelSize = 0;
					__instance.mapChunkArray[num33, num34].chunkID = 1;
					__instance.mapChunkArray[num33, num34].exitN = true;
					__instance.mapChunkArray[num33, num34].special = "LevelStart";
					__instance.levelSize++;

					while (__instance.levelSize < __instance.levelSizeMax && num35 < 100 && !flag3)
					{
						num35++;

						string text = GC.Choose<string>("N", "N", "S", "E", "E", "W", "W");

						if (__instance.levelSize == 1)
							text = "N";

						if (text == "N" && num34 == 1)
							flag3 = true;

						if (!flag3 && __instance.PlaceChunk(text, num33, num34))
						{
							if (text == "N")
								num34--;
							else if (text == "S")
								num34++;
							else if (text == "E")
								num33++;
							else if (text == "W")
								num33--;

							__instance.levelSize++;
						}
					}
				}

				bool flag4 = false;
				int num36 = __instance.levelSize + 1;

				for (int num37 = 1; num37 < __instance.levelSizeMax + 1; num37++)
					for (int num38 = 0; num38 < __instance.levelSizeAxis; num38++)
						for (int num39 = 0; num39 < __instance.levelSizeAxis; num39++)
							if (__instance.mapChunkArray[num38, num39].chunkID == num37 && __instance.mapChunkArray[num38, num39].chunkID == __instance.levelSize && !flag4)
							{
								__instance.mapChunkArray[num38, num39].exitN = true;
								__instance.mapChunkArray[num38, num39 - 1].chunkID = __instance.levelSize + 1;
								__instance.mapChunkArray[num38, num39 - 1].exitS = true;
								__instance.mapChunkArray[num38, num39 - 1].special = "LevelEnd";
								num36 = __instance.levelSize + 1;
								__instance.levelSize++;
								flag4 = true;
							}

				int num40 = -1;
				int num41 = -1;
				int num42 = -1;
				int num43 = -1;
				int num44 = -1;
				int num45 = -1;
				bool flag5;
				int num46;

				do
				{
					flag5 = true;
					num46 = GC.Choose<int>(1, 2, new int[]
					{
						2,
						3,
						3
					});

					if (num46 == 2)
						num40 = __instance.levelSize / 2 + Random.Range(-3, 4);
					else if (num46 == 3)
					{
						num40 = __instance.levelSize / 3 + Random.Range(-2, 3);
						num41 = __instance.levelSize * 2 / 3 + Random.Range(-2, 3);

						if (Mathf.Abs(num40 - num41) < 3)
							flag5 = false;
					}
				}
				while (!flag5);

				if (num40 != -1)
				{
					num42 = Random.Range(2, num40);
					num44 = Random.Range(2, num40);

					if (num44 == num42)
						num44 = -1;
				}

				if (num41 != -1)
				{
					num43 = Random.Range(num40 + 1, num41);
					num45 = Random.Range(num40 + 1, num41);

					if (num45 == num43)
						num45 = -1;
				}

				int num47;
				int num48;

				if (num41 != -1)
				{
					num47 = Random.Range(num41 + 1, num36);
					num48 = Random.Range(num41 + 1, num36);

					if (num48 == num47)
						num48 = -1;
				}
				else if (num40 != -1)
				{
					num47 = Random.Range(num40 + 1, num36);
					num48 = Random.Range(num40 + 1, num36);

					if (num48 == num47)
						num48 = -1;
				}
				else
				{
					num47 = Random.Range(2, num36);
					num48 = Random.Range(2, num36);

					if (num48 == num47)
						num48 = -1;
				}

				Debug.Log(string.Concat(new object[] { "Num Sectors: ", num46, " - Door 1: ", num40, " - Door 2: ", num41, " - End: ", num36, " - Branch 1: ", num42, " - Branch 2: ", num43, " - ranch3: ", num47, " - Branch 4: ", num44, " - Branch 5: ", num45, " - Branch 6: ", num48 }));

				for (int num49 = 0; num49 < __instance.levelSizeAxis; num49++)
					for (int num50 = 0; num50 < __instance.levelSizeAxis; num50++)
					{
						if (__instance.mapChunkArray[num49, num50].chunkID < num40 && __instance.mapChunkArray[num49, num50].chunkID != 0)
							__instance.mapChunkArray[num49, num50].sector = 1;
						else if (__instance.mapChunkArray[num49, num50].chunkID > num40 && __instance.mapChunkArray[num49, num50].chunkID < num41 && __instance.mapChunkArray[num49, num50].chunkID != 0)
							__instance.mapChunkArray[num49, num50].sector = 2;
						else if (__instance.mapChunkArray[num49, num50].chunkID > num40 && __instance.mapChunkArray[num49, num50].chunkID > num41 && __instance.mapChunkArray[num49, num50].chunkID < num36 && __instance.mapChunkArray[num49, num50].chunkID != 0)
							__instance.mapChunkArray[num49, num50].sector = 3;
						else if (__instance.mapChunkArray[num49, num50].chunkID == num40)
						{
							__instance.mapChunkArray[num49, num50].sector = 1;
							__instance.mapChunkArray[num49, num50].functionType = "Door";
						}
						else if (__instance.mapChunkArray[num49, num50].chunkID == num41)
						{
							__instance.mapChunkArray[num49, num50].sector = 2;
							__instance.mapChunkArray[num49, num50].functionType = "Door";
						}
						else if (__instance.mapChunkArray[num49, num50].chunkID == num36)
							__instance.mapChunkArray[num49, num50].sector = 3;
					}

				for (int num51 = 0; num51 < __instance.levelSizeAxis; num51++)
					for (int num52 = 0; num52 < __instance.levelSizeAxis; num52++)
						if (__instance.mapChunkArray[num51, num52].chunkID == num42 || __instance.mapChunkArray[num51, num52].chunkID == num43 || __instance.mapChunkArray[num51, num52].chunkID == num47 || __instance.mapChunkArray[num51, num52].chunkID == num44 || __instance.mapChunkArray[num51, num52].chunkID == num45 || __instance.mapChunkArray[num51, num52].chunkID == num48)
						{
							int num53 = 0;
							int num35 = 0;
							int num33 = num51;
							int num34 = num52;
							bool foundExit = false;
							int randE = Random.Range(1, 5);

							while (num53 < randE && num35 < 100)
							{
								num35++;

								string text2 = GC.Choose<string>("N", "S", new string[]
								{
									"E",
									"W"
								});

								if (__instance.PlaceChunk(text2, num33, num34))
								{
									if (text2 == "N")
										num34--;
									else if (text2 == "S")
										num34++;
									else if (text2 == "E")
										num33++;
									else if (text2 == "W")
										num33--;

									num53++;
									__instance.levelSize++;

									if (__instance.mapChunkArray[num51, num52].chunkID == num42 || __instance.mapChunkArray[num51, num52].chunkID == num44)
										__instance.mapChunkArray[num33, num34].sector = 1;
									else if (__instance.mapChunkArray[num51, num52].chunkID == num43 || __instance.mapChunkArray[num51, num52].chunkID == num45)
										__instance.mapChunkArray[num33, num34].sector = 2;
									else if (__instance.mapChunkArray[num51, num52].chunkID == num47 || __instance.mapChunkArray[num51, num52].chunkID == num48)
										__instance.mapChunkArray[num33, num34].sector = 3;

									if (num53 == randE)
										__instance.PlaceKeyOrTreasure(num53, foundExit, num33, num34);
								}
							}

							if (num53 == 0)
							{
								if (__instance.mapChunkArray[num51, num52].chunkID == num42)
									num42 = -1;

								if (__instance.mapChunkArray[num51, num52].chunkID == num43)
									num43 = -1;

								if (__instance.mapChunkArray[num51, num52].chunkID == num47)
									num47 = -1;

								if (__instance.mapChunkArray[num51, num52].chunkID == num44)
									num44 = -1;

								if (__instance.mapChunkArray[num51, num52].chunkID == num45)
									num45 = -1;

								if (__instance.mapChunkArray[num51, num52].chunkID == num48)
									num48 = -1;
							}
							else if (num35 == 100)
								__instance.PlaceKeyOrTreasure(num53, foundExit, num33, num34);
						}

				for (int num55 = 0; num55 < __instance.levelSizeAxis; num55++)
					for (int num56 = 0; num56 < __instance.levelSizeAxis; num56++)
					{
						if (!___placedKey1 && __instance.mapChunkArray[num55, num56].chunkID == num40)
							__instance.mapChunkArray[num55, num56].functionType = "";

						if (!___placedKey2 && __instance.mapChunkArray[num55, num56].chunkID == num41)
							__instance.mapChunkArray[num55, num56].functionType = "";

						if (!___placedKey3)
							_ = __instance.mapChunkArray[num55, num56].chunkID;
					}
			}

			return false;
		}
		public static bool LoadLevel_FillFloors_Prefix(LoadLevel __instance, ref IEnumerator __result, ref tk2dTileMap ___tilemapFloors2) // Prefix
		{
			if (BMChallenges.IsChallengeFromListActive(cChallenge.AffectsFloors))
			{
				__result = LoadLevel_FillFloors_Replacement(__instance, ___tilemapFloors2);

				return false;
			}

			return true;
		}
		public static IEnumerator LoadLevel_FillFloors_Replacement(LoadLevel __instance, tk2dTileMap ___tilemapFloors2) // Non-Patch
		{
			// Attempt at FloorMod. No visible effect yet.

			float maxChunkTime = 0.02f;
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			int triesCount = 0;
			int num;

			for (int i2 = 0; i2 < __instance.levelSizeAxis; i2 = num + 1)
			{
				for (int j2 = 0; j2 < __instance.levelSizeAxis; j2 = num + 1)
				{
					num = triesCount;
					triesCount = num + 1;
					int num2 = i2 * 16;
					int num3 = i2 * 16 + 16;
					int num4 = 160 - j2 * 16;
					int num5 = 160 - j2 * 16 - 16;

					for (int k = num2; k < num3; k++)
					{
						for (int l = num4; l > num5; l--)
						{
							__instance.tileInfo.tileArray[k, l - 1].chunkID = __instance.mapChunkArray[i2, j2].chunkID;
							string floorTileGroup = vFloorTileGroup.Building; // Homebase is default

							if (BMChallenges.IsChallengeFromListActive(cChallenge.AffectsFloors))
								floorTileGroup = vFloorTileGroup.Industrial; // No effect?
							else if (GC.levelShape == 0 && GC.levelType != "HomeBase")
							{
								if (GC.levelTheme == 0)
									floorTileGroup = vFloorTileGroup.Slums;
								else if (GC.levelTheme == 1)
									floorTileGroup = vFloorTileGroup.Industrial;
								else if (GC.levelTheme == 2)
									floorTileGroup = vFloorTileGroup.Park;
								else if (GC.levelTheme == 3)
									floorTileGroup = vFloorTileGroup.Downtown;
								else if (GC.levelTheme == 4)
									floorTileGroup = vFloorTileGroup.Uptown;
								else if (GC.levelTheme == 5)
									floorTileGroup = vFloorTileGroup.MayorVillage;
							}

							int tile = int.Parse(GC.rnd.RandomSelect(floorTileGroup, "RandomFloorsWalls"));

							___tilemapFloors2.SetTile(k, l - 1, 0, tile);
						}
					}

					if (Time.realtimeSinceStartup - realtimeSinceStartup > maxChunkTime)
					{
						yield return null;
						realtimeSinceStartup = Time.realtimeSinceStartup;
					}

					Random.InitState(__instance.randomSeedNum + triesCount);
					num = j2;
				}

				num = i2;
			}

			__instance.allChunksFilled = true;

			yield break;
		}
		public static bool LoadLevel_FillMapChunks_Prefix(LoadLevel __instance, ref IEnumerator __result, ref tk2dTileMap ___tilemapWalls, ref tk2dTileMap ___tilemapFloors2) // Replacement
		{
			__result = LoadLevel_FillMapChunks_Replacement(__instance, ___tilemapWalls, ___tilemapFloors2);

			return false;
		}
		public static IEnumerator LoadLevel_FillMapChunks_Replacement(LoadLevel __instance, tk2dTileMap ___tilemapWalls, tk2dTileMap ___tilemapFloors2) // Non-Patch
		{
			// Exterior Floors (confirmed)
			// This works in two different places, which work on different levels.

			int log = 0;
			BMLog("LoadLevel_FillMapChunks_Replacement");

			float maxChunkTime = 0.02f;
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			int triesCount = 0;
			int num;

			for (int i = 0; i < __instance.levelSizeAxis; i = num + 1)
			{
				for (int j = 0; j < __instance.levelSizeAxis; j = num + 1)
				{
					num = triesCount;
					triesCount = num + 1;

					if (__instance.mapChunkArray[i, j].chunkID != 0) // Slums, 
					{
						int num2 = i * 16;
						int num3 = i * 16 + 16;
						int num4 = 160 - j * 16;
						int num5 = 160 - j * 16 - 16;

						for (int k = num2; k < num3; k++)
						{
							for (int l = num4; l > num5; l--)
							{
								___tilemapWalls.ClearTile(k, l - 1, 0);
								__instance.tileInfo.tileArray[k, l - 1].chunkID = __instance.mapChunkArray[i, j].chunkID;
								string tilemapGroup = vFloorTileGroup.Building;

								if (BMChallenges.IsChallengeFromListActive(cChallenge.AffectsFloors))
									tilemapGroup = GetFloorTileGroup(); // Works on: Slums,
								else if (GC.levelShape == 0 && GC.levelType != "HomeBase")
								{
									if (GC.levelTheme == 0)
										tilemapGroup = vFloorTileGroup.Slums;
									else if (GC.levelTheme == 1)
										tilemapGroup = vFloorTileGroup.Industrial;
									else if (GC.levelTheme == 2)
										tilemapGroup = vFloorTileGroup.Park;
									else if (GC.levelTheme == 3)
										tilemapGroup = vFloorTileGroup.Downtown;
									else if (GC.levelTheme == 4)
										tilemapGroup = vFloorTileGroup.Uptown;
									else if (GC.levelTheme == 5)
										tilemapGroup = vFloorTileGroup.MayorVillage;
								}

								int tile = int.Parse(GC.rnd.RandomSelect(tilemapGroup, "RandomFloorsWalls"));
								___tilemapFloors2.SetTile(k, l - 1, 0, tile);
							}
						}
					}
					else if (!GC.holeLevel) // Park, 
					{
						__instance.mapChunkArray[i, j].filled = true;
						int num6 = i * 16;
						int num7 = i * 16 + 16;
						int num8 = 160 - j * 16;
						int num9 = 160 - j * 16 - 16;

						for (int m = num6; m < num7; m++)
						{
							for (int n = num8; n > num9; n--)
							{
								if (m != 0 && n != 160 && m != (__instance.levelSizeAxis - 1) * 16 + 16 - 1 && n != 160 - (__instance.levelSizeAxis - 1) * 16 - 16 + 1)
								{
									int wallMaterialOffset = 0;
									int wallMaterialOffsetTop = 0;

									switch (GC.levelTheme)
									{
										case 0:
											wallMaterialOffset = 72;
											wallMaterialOffsetTop = 140;
											break;
										case 1:
											wallMaterialOffset = 244;
											wallMaterialOffsetTop = 1015;
											break;
										case 2:
											wallMaterialOffset = 220;
											wallMaterialOffsetTop = 1085;
											break;
										case 3:
											wallMaterialOffset = 228;
											wallMaterialOffsetTop = 1155;
											break;
										case 4:
											wallMaterialOffset = 236;
											wallMaterialOffsetTop = 1225;
											break;
										case 5:
											wallMaterialOffset = 300;
											wallMaterialOffsetTop = 1673;
											break;
									}

									___tilemapWalls.SetTile(m, n - 1, 0, 0);
									TileData tileData = __instance.tileInfo.tileArray[m, n - 1];
									tileData.wallMaterialOffset = wallMaterialOffset;
									tileData.wallMaterialOffsetTop = wallMaterialOffsetTop;
									tileData.wallFrontVariation = true;
									tileData.wallMaterial = wallMaterialType.Border;
									string tilemapGroup = vFloorTileGroup.Building;

									if (BMChallenges.IsChallengeFromListActive(cChallenge.AffectsFloors))
										tilemapGroup = vFloorTileGroup.MayorVillage; // Works on: Park, 
									else if (GC.levelShape == 0 && GC.levelType != "HomeBase")
									{
										if (GC.levelTheme == 0)
											tilemapGroup = vFloorTileGroup.Slums;
										else if (GC.levelTheme == 1)
											tilemapGroup = vFloorTileGroup.Industrial;
										else if (GC.levelTheme == 2)
											tilemapGroup = vFloorTileGroup.Park;
										else if (GC.levelTheme == 3)
											tilemapGroup = vFloorTileGroup.Downtown;
										else if (GC.levelTheme == 4)
											tilemapGroup = vFloorTileGroup.Uptown;
										else if (GC.levelTheme == 5)
											tilemapGroup = vFloorTileGroup.MayorVillage;
									}

									int tile2 = int.Parse(GC.rnd.RandomSelect(tilemapGroup, "RandomFloorsWalls"));
									___tilemapFloors2.SetTile(m, n - 1, 0, tile2);
									tileData.chunkID = __instance.mapChunkArray[i, j].chunkID;
								}
							}
						}
					}

					if (Time.realtimeSinceStartup - realtimeSinceStartup > maxChunkTime)
					{
						yield return null;

						realtimeSinceStartup = Time.realtimeSinceStartup;
					}

					Random.InitState(__instance.randomSeedNum + triesCount);
					num = j;
				}

				num = i;
			}

			if (GC.levelType == "Tutorial" || GC.levelType == "HomeBase")
				__instance.allChunksFilled = true;
			else 
			{
				BMLog("\tA");
				MethodInfo FillMapChunks2_Private = AccessTools.Method(typeof(LoadLevel), "FillMapChunks2", new Type[0] { });
				IEnumerator FillMapChunks2_Private_IEnumerator = (IEnumerator)FillMapChunks2_Private.Invoke(__instance, new object[0]);
				__instance.StartCoroutine(FillMapChunks2_Private_IEnumerator);
				BMLog("\tB");
			}

			yield break;
		}
		public static bool LoadLevel_loadStuff2_Prefix (LoadLevel __instance) // Prefix
		{
			BMLog("LoadLevel_loadStuff2_Prefix");

			if (GC.challenges.Contains(cChallenge.SkywayDistrict))
				GC.canalHoles = true;
			else
				GC.canalHoles = false;

			return true;
		}
		public static bool LoadLevel_SetupBasicLevel (LoadLevel __instance, tk2dTileMap ___tilemapWalls, tk2dTileMap ___tilemapFloors2) // Replacement
		{
			// WallMod Borders

			for (int i = 0; i < __instance.levelSizeAxis; i++)
				for (int j = 0; j < __instance.levelSizeAxis; j++)
					if (__instance.mapChunkArray[i, j].chunkID != 0)
						for (int k = i * 16; k < i * 16 + 16; k++)
							for (int l = 160 - j * 16; l > 160 - j * 16 - 16; l--)
							{
								___tilemapWalls.ClearTile(k, l - 1, 0);
								__instance.tileInfo.tileArray[k, l - 1].chunkID = __instance.mapChunkArray[i, j].chunkID;
								int tile = Random.Range(0, 0);
								___tilemapFloors2.SetTile(k, l - 1, 0, tile);
							}
					else
					{
						__instance.mapChunkArray[i, j].filled = true;
						int num = i * 16;
						int num2 = i * 16 + 16;
						int num3 = 160 - j * 16;
						int num4 = 160 - j * 16 - 16;

						for (int m = num; m < num2; m++)
							for (int n = num3; n > num4; n--)
								if (m != 0 && n != 160 && m != (__instance.levelSizeAxis - 1) * 16 + 16 - 1 && n != 160 - (__instance.levelSizeAxis - 1) * 16 - 16 + 1)
								{
									int wallMaterialOffset = 0;
									int wallMaterialOffsetTop = 0;
						
									switch (GC.levelTheme)
									{
										case 0:
											wallMaterialOffset = 72;
											wallMaterialOffsetTop = 140;
											break;
										case 1:
											wallMaterialOffset = 244;
											wallMaterialOffsetTop = 1015;
											break;
										case 2:
											wallMaterialOffset = 220;
											wallMaterialOffsetTop = 1085;
											break;
										case 3:
											wallMaterialOffset = 228;
											wallMaterialOffsetTop = 1155;
											break;
										case 4:
											wallMaterialOffset = 236;
											wallMaterialOffsetTop = 1225;
											break;
										case 5:
											wallMaterialOffset = 300;
											wallMaterialOffsetTop = 1673;
											break;
									}

									TileData tileData = __instance.tileInfo.tileArray[m, n - 1];
									___tilemapWalls.SetTile(m, n - 1, 0, 0);
									tileData.wallMaterialOffset = wallMaterialOffset;
									tileData.wallMaterialOffsetTop = wallMaterialOffsetTop;
									tileData.wallFrontVariation = true;
									tileData.wallMaterial = GetBorderWallMaterialFromMutator();
									int tile2 = Random.Range(0, 0);
									___tilemapFloors2.SetTile(m, n - 1, 0, tile2);
									tileData.chunkID = __instance.mapChunkArray[i, j].chunkID;
								}
					}

			return false;
		}
		public static bool LoadLevel_SetupMore3_3_Prefix(LoadLevel __instance, ref tk2dTileMap ___tilemapFloors4, ref Minimap ___minimap, ref IEnumerator __result) // Prefix
		{
			BMLog("LoadLevel_SetupMore3_3_Prefix");

			// Structure advised by Abbysssal for patch-replacing IEnumerators.
			__result = LoadLevel_SetupMore3_3_Replacement(__instance, ___tilemapFloors4, ___minimap);

			return false;
		}
		public static IEnumerator LoadLevel_SetupMore3_3_Replacement(LoadLevel __instance, tk2dTileMap ___tilemapFloors4, Minimap ___minimap) // Non-Patch
		{
			// Level Features
			// Level Roamers

			BMLog("LoadLevel_SetupMore3_3_Replacement");

			Random.InitState(__instance.randomSeedNum);
			float maxChunkTime = 0.02f;
			float chunkStartTime = Time.realtimeSinceStartup;
			int randomCount = 0;

			if (GC.serverPlayer && !__instance.memoryTest && GC.levelType != "Attract")
			{
				if (GC.sessionDataBig.curLevel > 0 && GC.staticChunk == "")
					__instance.levelSizeModifier = (float)__instance.levelSizeMax / 30f;

				if (GC.levelType != "HomeBase" && GC.levelType != "Tutorial" && GC.staticChunk == "")
					GC.levelFeelingsScript.StartLevelFeelings();

				if (GC.sessionDataBig.curLevel > 0 && GC.staticChunk == "" && GC.levelType != "Tutorial" && GC.levelType != "HomeBase")
				{
					#region Features
					#region Lakes
					__instance.hasLakes = false;

					if (GC.levelTheme == 2 || 
						(GC.customLevel && __instance.customLevel.levelFeatures.Contains("Lake")) ||
						(GC.challenges.Contains("MixedUpLevels") && GC.percentChance(33) && GC.levelTheme != 3) ||
						GC.challenges.Contains(cChallenge.LakeItOrLeaveIt))
					{
						Debug.Log("Loading Lakes");

						List<Water> lakeList = new List<Water>();
						string lakeType = "Water";

						if (__instance.WaterToIce())
							lakeType = "Ice";

						int numObjects = (int)((float)Random.Range(5, 10) * __instance.levelSizeModifier);
						int bigTries = 0;
						bool findingFactoryLake = false;
						Chunk factoryChunk = null;

						if (lakeType == "Water")
							for (int i = 0; i < __instance.levelChunks.Count; i++)
								if (__instance.levelChunks[i].description == "Hideout" || __instance.levelChunks[i].description == "Factory")
								{
									findingFactoryLake = true;
									factoryChunk = __instance.levelChunks[i];
								}

						int num2;

						for (int i = 0; i < numObjects; i = num2 + 1)
						{
							Vector2 spot = Vector2.zero;
							int attempts = 0;

							do
							{
								spot = GC.tileInfo.FindRandLocationGeneral(0.64f);

								if (findingFactoryLake && 
									(((spot.x <= factoryChunk.chunkEdgeE || 
									spot.x >= factoryChunk.chunkEdgeE + __instance.chunkSize) && 
									(spot.x >= factoryChunk.chunkEdgeW || 
									spot.x <= factoryChunk.chunkEdgeW - __instance.chunkSize)) || 
									spot.y <= factoryChunk.chunkEdgeS - __instance.chunkSize || 
									spot.y >= factoryChunk.chunkEdgeN + __instance.chunkSize) && 
									(((spot.y <= factoryChunk.chunkEdgeN || 
									spot.y >= factoryChunk.chunkEdgeN + __instance.chunkSize) && 
									(spot.y >= factoryChunk.chunkEdgeS || 
									spot.y <= factoryChunk.chunkEdgeS - __instance.chunkSize)) || 
									spot.x <= factoryChunk.chunkEdgeW - __instance.chunkSize || 
									spot.x >= factoryChunk.chunkEdgeE + __instance.chunkSize))
									spot = Vector2.zero;

								attempts++;
								TileData tileData = GC.tileInfo.GetTileData(spot);

								if (tileData.chunkID == GC.startingPoint.startingChunk ||
									(GC.exitPoint != null && tileData.chunkID == GC.exitPoint.startingChunk))
									spot = Vector2.zero;

								for (int l = -4; l < 5; l++)
									for (int m = -4; m < 5; m++)
										if (GC.tileInfo.GetTileDataInt(tileData.posX + l, tileData.posY + m).wallMaterial != wallMaterialType.None)
										{
											spot = Vector2.zero;

											break;
										}
							}
							while ((spot == Vector2.zero || Vector2.Distance(spot, GC.playerAgent.tr.position) < 8f) && attempts < 100);

							if (spot != Vector2.zero)
							{
								GC.tileInfo.lakeN = 0f;
								GC.tileInfo.lakeS = 9999f;
								GC.tileInfo.lakeE = 0f;
								GC.tileInfo.lakeW = 9999f;
								Random.InitState(__instance.randomSeedNum + i + randomCount);
								int expandLimit = Random.Range(7, 12);
								GC.tileInfo.CreateLake(spot, 0, expandLimit, lakeType);
								Vector2 vector2 = new Vector2((GC.tileInfo.lakeE + GC.tileInfo.lakeW) / 2f, (GC.tileInfo.lakeN + GC.tileInfo.lakeS) / 2f);
								Vector2 v = new Vector2((GC.tileInfo.lakeE - GC.tileInfo.lakeW) / 0.64f + 0.32f, (GC.tileInfo.lakeN - GC.tileInfo.lakeS) / 0.64f + 0.32f);

								if (lakeType == "Water")
								{
									Water water = GC.spawnerMain.SpawnWater(null, vector2, v, __instance.transform.rotation, true, null); // __instance.transform.rotation
									water.lakeNTile = GC.tileInfo.lakeNTile;
									water.lakeSTile = GC.tileInfo.lakeSTile;
									water.lakeETile = GC.tileInfo.lakeETile;
									water.lakeWTile = GC.tileInfo.lakeWTile;
									lakeList.Add(water);

									if (findingFactoryLake)
									{
										water.factory = factoryChunk;

										if (GC.oneTimeVersion)
											water.SpreadPoisonWait("Giant");
										else
											water.SpreadPoisonWait(GC.playerAgent.statusEffects.ChooseRandomStatusEffectLake());
									}
									else if (GC.challenges.Contains(cChallenge.ThePollutionSolution) && GC.percentChance(80))
										water.SpreadPoisonWait(GC.playerAgent.statusEffects.ChooseRandomStatusEffectLake());
								}

								if (GC.lightingType == "Full" || GC.lightingType == "Med")
								{
									LightReal lightReal = GC.spawnerMain.SpawnLightReal(vector2, null, 2);
									lightReal.name = "LakeLight";
									lightReal.tr.localScale = new Vector3(8f, 8f, 1f);
									lightReal.GetComponent<LightSprite>().Color = new Color32(0, 213, 255, 85);
									lightReal.GetComponent<LightSprite>().Sprite = lightReal.pointLightSpecial;
									GC.tileInfo.lakeLightLocations.Add(vector2);
								}

								if (findingFactoryLake)
									findingFactoryLake = false;
							}

							num2 = bigTries;
							bigTries = num2 + 1;

							if (bigTries >= 100)
								break;

							if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
							{
								yield return null;

								chunkStartTime = Time.realtimeSinceStartup;
							}

							Random.InitState(__instance.randomSeedNum + i);
							num2 = i;
						}

						if (lakeType == "Water")
						{
							for (int n = 1; n < __instance.levelSizeAxis * 16 - 1; n++)
								for (int num3 = 1; num3 < __instance.levelSizeAxis * 16 - 1; num3++)
									if (GC.tileInfo.tileArray[n, num3].water)
										GC.tileInfo.SetupFloorTile(n, num3, 4);
						}
						else if (lakeType == "Ice")
							for (int num4 = 1; num4 < __instance.levelSizeAxis * 16 - 1; num4++)
								for (int num5 = 1; num5 < __instance.levelSizeAxis * 16 - 1; num5++)
									if (GC.tileInfo.tileArray[num4, num5].ice)
										GC.tileInfo.SetupFloorTile(num4, num5, 4);

						if (lakeType == "Water")
						{
							int num6 = 0;

							while ((float)num6 < (float)lakeList.Count / 2.5f)
							{
								Water water2 = lakeList[num6];
								List<Vector3> list = new List<Vector3>();

								for (int num7 = 1; num7 < __instance.levelSizeAxis * 16 - 1; num7++)
									for (int num8 = 1; num8 < __instance.levelSizeAxis * 16 - 1; num8++)
										if (num7 >= water2.lakeWTile && num7 <= water2.lakeETile && num8 >= water2.lakeSTile && num8 <= water2.lakeNTile)
										{
											int tile = ___tilemapFloors4.GetTile(num7, num8, 0);

											if (tile == 695 || tile == 699 || tile == 701 || tile == 702)
												list.Add(new Vector3((float)num7, (float)num8, 0f));
										}

								Vector3 vector3 = Vector3.zero;

								if (list.Count != 0 && water2.factory == null)
									vector3 = list[Random.Range(0, list.Count)];

								Vector3 zero = Vector3.zero;

								if (vector3 != Vector3.zero)
								{
									int tile2 = ___tilemapFloors4.GetTile((int)vector3.x, (int)vector3.y, 0);
									int num9 = (int)vector3.x;
									int num10 = (int)vector3.y;

									if (tile2 == 688)
										zero = new Vector3((float)num9, (float)(num10 + 1), 0f);

									if (tile2 == 689)
										zero = new Vector3((float)num9, (float)(num10 - 1), 0f);

									if (tile2 == 690)
										zero = new Vector3((float)(num9 + 1), (float)num10, 0f);

									if (tile2 == 691)
										zero = new Vector3((float)(num9 + 1), (float)(num10 - 1), 0f);

									if (tile2 == 692)
										zero = new Vector3((float)num9, (float)(num10 + 1), 0f);

									if (tile2 == 693)
										zero = new Vector3((float)(num9 + 1), (float)num10, 0f);

									if (tile2 == 694)
										zero = new Vector3((float)(num9 + 1), (float)(num10 + 1), 0f);

									if (tile2 == 695)
										zero = new Vector3((float)(num9 + 1), (float)num10, 0f);

									if (tile2 == 696)
										zero = new Vector3((float)(num9 - 1), (float)num10, 0f);

									if (tile2 == 697)
										zero = new Vector3((float)(num9 - 1), (float)(num10 - 1), 0f);

									if (tile2 == 698)
										zero = new Vector3((float)num9, (float)(num10 - 1), 0f);

									if (tile2 == 699)
										zero = new Vector3((float)num9, (float)(num10 - 1), 0f);

									if (tile2 == 700)
										zero = new Vector3((float)(num9 - 1), (float)(num10 - 1), 0f);

									if (tile2 == 701)
										zero = new Vector3((float)(num9 - 1), (float)num10, 0f);

									if (tile2 == 702)
										zero = new Vector3((float)num9, (float)(num10 + 1), 0f);
								}

								if (zero != Vector3.zero)
								{
									zero = new Vector3(zero.x * 0.64f, zero.y * 0.64f, 0f);

									if (!GC.tileInfo.IsOverlapping(zero, "ObjectReal"))
									{
										WaterPump waterPump = (WaterPump)GC.spawnerMain.spawnObjectReal(zero, null, "WaterPump");

										if (GC.tileInfo.tileArray[(int)vector3.x, (int)vector3.y].poisonWaterFake)
											waterPump.RemoveMe();
										else
										{
											waterPump.lake = water2;
											water2.waterPump = waterPump;
											water2.SpreadPoisonStart(null, false);
										}
									}
								}

								num6++;
							}
						}

						for (int num11 = 0; num11 < __instance.levelSizeAxis * 16; num11++)
							for (int num12 = 0; num12 < __instance.levelSizeAxis * 16; num12++)
								if (GC.tileInfo.tileArray[num11, num12].lake)
									GC.tileInfo.lakeTiles.Add(new Vector2((float)num11, (float)num12));
					}
					#endregion
					#region Power Boxes
					int earlyCops = 0;
					bool hasPowerBoxes = false;
					bool hasExtraPowerBoxes = false;

					if (((GC.sessionDataBig.curLevel >= 2 || GC.levelTheme != 0) && GC.levelTheme != 2) ||
						(GC.levelTheme == 3 && GC.debugMode) ||
						(GC.customLevel && __instance.customLevel.levelFeatures.Contains("PowerBox")) || 
						GC.challenges.Contains(cChallenge.PowerWhelming))
						hasPowerBoxes = true;

					if (GC.quests.CanHaveBigQuest(null))
					{
						for (int num13 = 0; num13 < 5; num13++)
							if (GC.sessionData.bigQuest[num13] == "RobotPlayer")
							{
								hasPowerBoxes = true;
								hasExtraPowerBoxes = true;
							}

						if (!hasExtraPowerBoxes)
							for (int num14 = 0; num14 < GC.playerAgentList.Count; num14++)
								if (GC.playerAgentList[num14].bigQuest == "RobotPlayer")
								{
									hasPowerBoxes = true;
									hasExtraPowerBoxes = true;
								}
					}

					if (hasPowerBoxes)
					{
						Debug.Log("Loading Power Boxes");
						int bigTries = GC.Choose<int>(0, 1, new int[0]);

						if (hasExtraPowerBoxes)
							bigTries = GC.Choose<int>(2, 3, new int[0]);

						int attempts;

						for (int numObjects = 0; numObjects < bigTries; numObjects = attempts + 1)
						{
							Vector2 vector4 = Vector2.zero;
							int attempts2 = 0;

							do
							{
								vector4 = GC.tileInfo.FindRandLocationGeneral(2f);

								for (int num16 = 0; num16 < GC.objectRealList.Count; num16++)
									if (GC.objectRealList[num16].objectName == "PowerBox" && Vector2.Distance(GC.objectRealList[num16].tr.position, vector4) < 28f)
										vector4 = Vector2.zero;

								if (vector4 != Vector2.zero)
								{
									if (GC.tileInfo.WaterNearby(vector4))
										vector4 = Vector2.zero;

									if (GC.tileInfo.IceNearby(vector4))
										vector4 = Vector2.zero;

									if (GC.tileInfo.BridgeNearby(vector4))
										vector4 = Vector2.zero;
								}

								attempts2++;
							}
							while ((vector4 == Vector2.zero || Vector2.Distance(vector4, GC.playerAgent.tr.position) < 5f || Vector2.Distance(vector4, GC.elevatorDown.tr.position) < 20f) && attempts2 < 100);

							if (vector4 != Vector2.zero)
								GC.spawnerMain.spawnObjectReal(vector4, null, "PowerBox");

							if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
							{
								yield return null;
								chunkStartTime = Time.realtimeSinceStartup;
							}

							Random.InitState(__instance.randomSeedNum + numObjects);
							attempts = numObjects;
						}
					}
					#endregion
					#region Slime Barrels
					bool hasSlimeBarrels = false;

					if ((GC.levelTheme == 2 || (GC.challenges.Contains("MixedUpLevels") && GC.percentChance(33))) ||
						(GC.customLevel && __instance.customLevel.levelFeatures.Contains("SlimeBarrel")) ||
						GC.challenges.Contains(cChallenge.ThePollutionSolution))
						hasSlimeBarrels = true;

					if (hasSlimeBarrels)
					{
						Debug.Log("Loading Slime Barrels");
						int bigTries = Random.Range(2, 6);
						Water myLake = null;

						for (int i = 0; i < GC.watersList.Count; i++)
							if (GC.watersList[i].factory != null)
								myLake = GC.watersList[i];

						if (myLake != null)
						{
							int i2;

							for (int i = 0; i < bigTries; i = i2 + 1)
							{
								Vector2 vector5 = Vector2.zero;
								int j = 0;

								do
								{
									vector5 = GC.tileInfo.FindRandLocationGeneral(0.64f);
									j++;
								}
								while ((vector5 == Vector2.zero || Vector2.Distance(vector5, GC.playerAgent.tr.position) < 5f || Vector2.Distance(vector5, myLake.tr.position) > 7f) && j < 300);

								if (vector5 != Vector2.zero)
									GC.spawnerMain.spawnObjectReal(vector5, null, "SlimeBarrel");

								if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
								{
									yield return null;
									chunkStartTime = Time.realtimeSinceStartup;
								}

								Random.InitState(__instance.randomSeedNum + i);
								i2 = i;
							}
						}
					}
					#endregion
					#region Oil Spills
					bool hasOilSpills = false;

					if (GC.levelTheme == 1 || 
						(GC.challenges.Contains("MixedUpLevels") && GC.percentChance(33)) ||
						(GC.customLevel && __instance.customLevel.levelFeatures.Contains("OilSpill")) ||
						GC.challenges.Contains(cChallenge.ThePollutionSolution))
						hasOilSpills = true;

					if (hasOilSpills && GC.serverPlayer)
					{
						Debug.Log("Loading Oil Spills");
						int bigTries = (int)((float)Random.Range(6, 10) * __instance.levelSizeModifier);
						int num2;

						for (int numObjects = 0; numObjects < bigTries; numObjects = num2 + 1)
						{
							Vector2 pos = Vector2.zero;
							pos = GC.tileInfo.FindRandLocationGeneral();
							GC.tileInfo.SpillLiquidLarge(pos, "Oil", false, 0, false);

							if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
							{
								yield return null;
								chunkStartTime = Time.realtimeSinceStartup;
							}

							Random.InitState(__instance.randomSeedNum + numObjects);
							num2 = numObjects;
						}
					}
					#endregion
					#region Vending Machines
					if (true)
					{
						Debug.Log("Loading Vending Machines");
						Random.InitState(GC.loadLevel.randomSeedNum + GC.sessionDataBig.curLevelEndless + 12);
						List<string> machines = new List<string>();
						machines.Add("AmmoDispenser");
						machines.Add("ATMMachine");
						machines.Add("AugmentationBooth");
						machines.Add("CapsuleMachine");
						machines.Add("CloneMachine");
						machines.Add("PawnShopMachine");

						if (!GC.arcadeMode)
							machines.Add("LoadoutMachine");

						for (int i = 0; i < GC.objectRealList.Count; i++)
						{
							string objectName = GC.objectRealList[i].objectName;

							if (objectName == "PawnShopMachine" || objectName == "AmmoDispenser" || objectName == "CloneMachine" || objectName == "CapsuleMachine" || objectName == "LoadoutMachine" || objectName == "ATMMachine" || objectName == "AugmentationBooth")
								machines.Remove(objectName);
						}

						int count = machines.Count;
						List<int> list3 = new List<int>();

						for (int i = 0; i < count; i++)
						{
							Vector2 spot = Vector2.zero;
							int i2 = 0;

							do
							{
								spot = GC.tileInfo.FindRandLocationNearWall(0.64f);

								if (spot != Vector2.zero)
								{
									TileData tileData2 = GC.tileInfo.GetTileData(spot);

									if (GC.tileInfo.GetTileData(new Vector2(spot.x, spot.y + 0.64f)).owner == 0 && GC.tileInfo.GetTileData(new Vector2(spot.x + 0.64f, spot.y)).owner == 0 && GC.tileInfo.GetTileData(new Vector2(spot.x, spot.y - 0.64f)).owner == 0 && GC.tileInfo.GetTileData(new Vector2(spot.x - 0.64f, spot.y)).owner == 0)
										spot = Vector2.zero;

									if (!GC.tileInfo.IsOverlapping(new Vector2(spot.x, spot.y + 0.64f), "Wall") && !GC.tileInfo.IsOverlapping(new Vector2(spot.x, spot.y - 0.64f), "Wall") && !GC.tileInfo.IsOverlapping(new Vector2(spot.x + 0.64f, spot.y), "Wall") && !GC.tileInfo.IsOverlapping(new Vector2(spot.x - 0.64f, spot.y), "Wall"))
										spot = Vector2.zero;

									if (GC.tileInfo.IsOverlapping(spot, "ObjectRealSprite", 0.64f))
										spot = Vector2.zero;

									if (list3.Contains(tileData2.chunkID))
										spot = Vector2.zero;

									if (GC.tileInfo.DestroyIfBetweenWalls(spot))
										spot = Vector2.zero;
								}

								i2++;
							}
							while ((spot == Vector2.zero || Vector2.Distance(spot, GC.playerAgent.tr.position) < 2f) && i2 < 100);

							if (spot != Vector2.zero)
							{
								Random.InitState(__instance.randomSeedNum + 1);
								string text = machines[Random.Range(0, machines.Count)];
								ObjectReal objectReal = GC.spawnerMain.spawnObjectReal(spot, null, text);
								machines.Remove(text);
								bool flag5 = false;

								if (GC.tileInfo.GetTileData(new Vector2(spot.x, spot.y + 0.64f)).owner != 0 && GC.tileInfo.GetTileData(new Vector2(spot.x, spot.y + 0.64f)).wallMaterial != wallMaterialType.None)
								{
									objectReal.direction = "S";
									flag5 = true;
								}
								else if (GC.tileInfo.GetTileData(new Vector2(spot.x, spot.y - 0.64f)).owner != 0 && GC.tileInfo.GetTileData(new Vector2(spot.x, spot.y - 0.64f)).wallMaterial != wallMaterialType.None)
								{
									objectReal.direction = "N";
									flag5 = true;
								}
								else if (GC.tileInfo.GetTileData(new Vector2(spot.x + 0.64f, spot.y)).owner != 0 && GC.tileInfo.GetTileData(new Vector2(spot.x + 0.64f, spot.y)).wallMaterial != wallMaterialType.None)
								{
									objectReal.direction = "W";
									flag5 = true;
								}
								else if (GC.tileInfo.GetTileData(new Vector2(spot.x - 0.64f, spot.y)).owner != 0 && GC.tileInfo.GetTileData(new Vector2(spot.x - 0.64f, spot.y)).wallMaterial != wallMaterialType.None)
								{
									objectReal.direction = "E";
									flag5 = true;
								}

								if (!flag5)
								{
									if (GC.tileInfo.IsOverlapping(new Vector2(spot.x, spot.y + 0.64f), "Wall"))
										objectReal.direction = "S";
									else if (GC.tileInfo.IsOverlapping(new Vector2(spot.x, spot.y - 0.64f), "Wall"))
										objectReal.direction = "N";
									else if (GC.tileInfo.IsOverlapping(new Vector2(spot.x + 0.64f, spot.y), "Wall"))
										objectReal.direction = "W";
									else if (GC.tileInfo.IsOverlapping(new Vector2(spot.x - 0.64f, spot.y), "Wall"))
										objectReal.direction = "E";
								}

								string text2 = objectReal.direction;

								if (text2 == "S" || !objectReal.fourDirection)
									text2 = "";

								objectReal.ChangeSprite(text + text2);
								objectReal.startingObjectSprite = text + text2;
								objectReal.ShiftTowardWalls(objectReal.direction);
								TileData tileData3 = GC.tileInfo.GetTileData(spot);
								list3.Add(tileData3.chunkID);

								if (i < count - 1 && machines.Count != 0)
								{
									string a = "";
									Vector2 zero2 = Vector2.zero;
									Vector2 zero3 = Vector2.zero;

									if (GC.tileInfo.GetTileData(new Vector2(spot.x, spot.y + 0.64f)).wallMaterial != wallMaterialType.None)
									{
										zero2 = new Vector2(spot.x + 1.28f, spot.y);
										zero3 = new Vector2(spot.x - 1.28f, spot.y);
										a = "N";
									}
									else if (GC.tileInfo.GetTileData(new Vector2(spot.x, spot.y - 0.64f)).wallMaterial != wallMaterialType.None)
									{
										zero2 = new Vector2(spot.x + 1.28f, spot.y);
										zero3 = new Vector2(spot.x - 1.28f, spot.y);
										a = "S";
									}
									else if (GC.tileInfo.GetTileData(new Vector2(spot.x + 0.64f, spot.y)).wallMaterial != wallMaterialType.None)
									{
										zero2 = new Vector2(spot.x, spot.y + 1.28f);
										zero3 = new Vector2(spot.x, spot.y - 1.28f);
										a = "E";
									}
									else if (GC.tileInfo.GetTileData(new Vector2(spot.x - 0.64f, spot.y)).wallMaterial != wallMaterialType.None)
									{
										zero2 = new Vector2(spot.x, spot.y + 1.28f);
										zero3 = new Vector2(spot.x, spot.y - 1.28f);
										a = "W";
									}

									GC.tileInfo.GetTileData(zero2);
									bool flag6 = true;

									if ((GC.tileInfo.GetTileData(new Vector2(zero2.x, zero2.y + 0.64f)).wallMaterial == wallMaterialType.None && a == "N") ||
										(GC.tileInfo.GetTileData(new Vector2(zero2.x, zero2.y - 0.64f)).wallMaterial != wallMaterialType.None && a == "N") ||
										(GC.tileInfo.GetTileData(new Vector2(zero2.x + 0.64f, zero2.y - 0.64f)).wallMaterial != wallMaterialType.None && a == "N") ||
										(GC.tileInfo.GetTileData(new Vector2(zero2.x - 0.64f, zero2.y - 0.64f)).wallMaterial != wallMaterialType.None && a == "N") ||
										(GC.tileInfo.GetTileData(new Vector2(zero2.x + 0.64f, zero2.y)).wallMaterial == wallMaterialType.None && a == "E") ||
										(GC.tileInfo.GetTileData(new Vector2(zero2.x - 0.64f, zero2.y)).wallMaterial != wallMaterialType.None && a == "E") ||
										(GC.tileInfo.GetTileData(new Vector2(zero2.x - 0.64f, zero2.y + 0.64f)).wallMaterial != wallMaterialType.None && a == "E") ||
										(GC.tileInfo.GetTileData(new Vector2(zero2.x - 0.64f, zero2.y - 0.64f)).wallMaterial != wallMaterialType.None && a == "E") ||
										(GC.tileInfo.GetTileData(new Vector2(zero2.x, zero2.y - 0.64f)).wallMaterial == wallMaterialType.None && a == "S") ||
										(GC.tileInfo.GetTileData(new Vector2(zero2.x, zero2.y + 0.64f)).wallMaterial != wallMaterialType.None && a == "S") ||
										(GC.tileInfo.GetTileData(new Vector2(zero2.x + 0.64f, zero2.y + 0.64f)).wallMaterial != wallMaterialType.None && a == "S") ||
										(GC.tileInfo.GetTileData(new Vector2(zero2.x - 0.64f, zero2.y + 0.64f)).wallMaterial != wallMaterialType.None && a == "S") ||
										(GC.tileInfo.GetTileData(new Vector2(zero2.x - 0.64f, zero2.y)).wallMaterial == wallMaterialType.None && a == "W") ||
										(GC.tileInfo.GetTileData(new Vector2(zero2.x + 0.64f, zero2.y)).wallMaterial != wallMaterialType.None && a == "W") ||
										(GC.tileInfo.GetTileData(new Vector2(zero2.x + 0.64f, zero2.y + 0.64f)).wallMaterial != wallMaterialType.None && a == "W") ||
										(GC.tileInfo.GetTileData(new Vector2(zero2.x + 0.64f, zero2.y - 0.64f)).wallMaterial != wallMaterialType.None && a == "W"))
										flag6 = false;

									if (GC.tileInfo.IsOverlapping(zero2, "Anything"))
										flag6 = false;

									if (GC.tileInfo.IsOverlapping(zero2, "ObjectRealSprite", 0.64f))
										flag6 = false;

									if (flag6 && zero2 != Vector2.zero)
									{
										Random.InitState(__instance.randomSeedNum + 2);
										text = machines[Random.Range(0, machines.Count)];
										ObjectReal objectReal2 = GC.spawnerMain.spawnObjectReal(zero2, null, text);
										machines.Remove(text);
										objectReal2.direction = objectReal.direction;
										text2 = objectReal2.direction;

										if (text2 == "S" || !objectReal2.fourDirection)
											text2 = "";

										objectReal2.ChangeSprite(text + text2);
										objectReal2.startingObjectSprite = text + text2;
										objectReal2.ShiftTowardWalls(objectReal2.direction);
										i++;
									}
									else
									{
										GC.tileInfo.GetTileData(zero3);
										flag6 = true;

										if ((GC.tileInfo.GetTileData(new Vector2(zero3.x, zero3.y + 0.64f)).wallMaterial == wallMaterialType.None && a == "N") ||
											(GC.tileInfo.GetTileData(new Vector2(zero3.x, zero3.y - 0.64f)).wallMaterial != wallMaterialType.None && a == "N") ||
											(GC.tileInfo.GetTileData(new Vector2(zero3.x + 0.64f, zero3.y - 0.64f)).wallMaterial != wallMaterialType.None && a == "N") ||
											(GC.tileInfo.GetTileData(new Vector2(zero3.x - 0.64f, zero3.y - 0.64f)).wallMaterial != wallMaterialType.None && a == "N") ||
											(GC.tileInfo.GetTileData(new Vector2(zero3.x + 0.64f, zero3.y)).wallMaterial == wallMaterialType.None && a == "E") ||
											(GC.tileInfo.GetTileData(new Vector2(zero3.x - 0.64f, zero3.y)).wallMaterial != wallMaterialType.None && a == "E") ||
											(GC.tileInfo.GetTileData(new Vector2(zero3.x - 0.64f, zero3.y + 0.64f)).wallMaterial != wallMaterialType.None && a == "E") ||
											(GC.tileInfo.GetTileData(new Vector2(zero3.x - 0.64f, zero3.y - 0.64f)).wallMaterial != wallMaterialType.None && a == "E") ||
											(GC.tileInfo.GetTileData(new Vector2(zero3.x, zero3.y - 0.64f)).wallMaterial == wallMaterialType.None && a == "S") ||
											(GC.tileInfo.GetTileData(new Vector2(zero3.x, zero3.y + 0.64f)).wallMaterial != wallMaterialType.None && a == "S") ||
											(GC.tileInfo.GetTileData(new Vector2(zero3.x + 0.64f, zero3.y + 0.64f)).wallMaterial != wallMaterialType.None && a == "S") ||
											(GC.tileInfo.GetTileData(new Vector2(zero3.x - 0.64f, zero3.y + 0.64f)).wallMaterial != wallMaterialType.None && a == "S") ||
											(GC.tileInfo.GetTileData(new Vector2(zero3.x - 0.64f, zero3.y)).wallMaterial == wallMaterialType.None && a == "W") ||
											(GC.tileInfo.GetTileData(new Vector2(zero3.x + 0.64f, zero3.y)).wallMaterial != wallMaterialType.None && a == "W") ||
											(GC.tileInfo.GetTileData(new Vector2(zero3.x + 0.64f, zero3.y + 0.64f)).wallMaterial != wallMaterialType.None && a == "W") ||
											(GC.tileInfo.GetTileData(new Vector2(zero3.x + 0.64f, zero3.y - 0.64f)).wallMaterial != wallMaterialType.None && a == "W"))
											flag6 = false;

										if (GC.tileInfo.IsOverlapping(zero3, "Anything"))
											flag6 = false;

										if (GC.tileInfo.IsOverlapping(zero3, "ObjectRealSprite", 0.64f))
											flag6 = false;

										if (flag6 && zero3 != Vector2.zero)
										{
											Random.InitState(__instance.randomSeedNum + 3);
											text = machines[Random.Range(0, machines.Count)];
											ObjectReal objectReal3 = GC.spawnerMain.spawnObjectReal(zero3, null, text);
											machines.Remove(text);
											objectReal3.direction = objectReal.direction;
											text2 = objectReal3.direction;

											if (text2 == "S" || !objectReal3.fourDirection)
												text2 = "";

											objectReal3.ChangeSprite(text + text2);
											objectReal3.startingObjectSprite = text + text2;
											objectReal3.ShiftTowardWalls(objectReal3.direction);
											i++;
										}
									}
								}
							}
						}
					}
					#endregion
					#region Trash Cans
					bool hasTrashCans = true;

					if ((GC.customLevel && __instance.customLevel.levelFeatures.Contains("TrashCan")) ||
						GC.challenges.Contains(cChallenge.ArcologyEcology) || GC.challenges.Contains(cChallenge.PoliceState) || GC.challenges.Contains(cChallenge.MACITS))
						hasTrashCans = true;

					if (GC.challenges.Contains(cChallenge.AnCapistan))
						hasTrashCans = false;

					if (hasTrashCans)
					{
						Debug.Log("Loading Trash Cans");
						int bigTries = (int)((float)Random.Range(6, 12) * __instance.levelSizeModifier);
						List<int> spawnedInChunks = new List<int>();
						int num2;

						for (int i = 0; i < bigTries; i = num2 + 1)
						{
							Vector2 spot = Vector2.zero;
							int i2 = 0;

							do
							{
								spot = GC.tileInfo.FindRandLocationNearWall(0.64f);

								if (spot != Vector2.zero)
								{
									TileData tileData4 = GC.tileInfo.GetTileData(spot);

									if (GC.tileInfo.GetTileData(new Vector2(spot.x, spot.y + 0.64f)).owner == 0 &&
										GC.tileInfo.GetTileData(new Vector2(spot.x + 0.64f, spot.y)).owner == 0 &&
										GC.tileInfo.GetTileData(new Vector2(spot.x, spot.y - 0.64f)).owner == 0 &&
										GC.tileInfo.GetTileData(new Vector2(spot.x - 0.64f, spot.y)).owner == 0)
										spot = Vector2.zero;

									if (!GC.tileInfo.IsOverlapping(new Vector2(spot.x, spot.y + 0.64f), "Wall") &&
										!GC.tileInfo.IsOverlapping(new Vector2(spot.x, spot.y - 0.64f), "Wall") &&
										!GC.tileInfo.IsOverlapping(new Vector2(spot.x + 0.64f, spot.y), "Wall") &&
										!GC.tileInfo.IsOverlapping(new Vector2(spot.x - 0.64f, spot.y), "Wall"))
										spot = Vector2.zero;

									if (GC.tileInfo.IsOverlapping(spot, "ObjectRealSprite", 0.64f))
										spot = Vector2.zero;

									if (spawnedInChunks.Contains(tileData4.chunkID))
										spot = Vector2.zero;

									if (GC.tileInfo.DestroyIfBetweenWalls(spot))
										spot = Vector2.zero;
								}

								i2++;
							}
							while ((spot == Vector2.zero || Vector2.Distance(spot, GC.playerAgent.tr.position) < 5f) && i2 < 100);

							if (spot != Vector2.zero)
							{
								GC.spawnerMain.spawnObjectReal(spot, null, vObject.TrashCan).ShiftTowardWalls();

								TileData tileData5 = GC.tileInfo.GetTileData(spot);
								spawnedInChunks.Add(tileData5.chunkID);
								bool flag8 = true;
								Random.InitState(__instance.randomSeedNum + i + ++randomCount);

								if (i < bigTries - 1 && GC.percentChance(25) && flag8)
								{
									string a2 = "";
									Vector2 zero4 = Vector2.zero;
									Vector2 zero5 = Vector2.zero;

									if (GC.tileInfo.GetTileData(new Vector2(spot.x, spot.y + 0.64f)).wallMaterial != wallMaterialType.None)
									{
										zero4 = new Vector2(spot.x + 1.28f, spot.y);
										zero5 = new Vector2(spot.x - 1.28f, spot.y);
										a2 = "N";
									}
									else if (GC.tileInfo.GetTileData(new Vector2(spot.x, spot.y - 0.64f)).wallMaterial != wallMaterialType.None)
									{
										zero4 = new Vector2(spot.x + 1.28f, spot.y);
										zero5 = new Vector2(spot.x - 1.28f, spot.y);
										a2 = "S";
									}
									else if (GC.tileInfo.GetTileData(new Vector2(spot.x + 0.64f, spot.y)).wallMaterial != wallMaterialType.None)
									{
										zero4 = new Vector2(spot.x, spot.y + 1.28f);
										zero5 = new Vector2(spot.x, spot.y - 1.28f);
										a2 = "E";
									}
									else if (GC.tileInfo.GetTileData(new Vector2(spot.x - 0.64f, spot.y)).wallMaterial != wallMaterialType.None)
									{
										zero4 = new Vector2(spot.x, spot.y + 1.28f);
										zero5 = new Vector2(spot.x, spot.y - 1.28f);
										a2 = "W";
									}

									GC.tileInfo.GetTileData(zero4);
									bool flag9 = true;

									if ((GC.tileInfo.GetTileData(new Vector2(zero4.x, zero4.y + 0.64f)).wallMaterial == wallMaterialType.None && a2 == "N") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x, zero4.y - 0.64f)).wallMaterial != wallMaterialType.None && a2 == "N") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x + 0.64f, zero4.y - 0.64f)).wallMaterial != wallMaterialType.None && a2 == "N") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x - 0.64f, zero4.y - 0.64f)).wallMaterial != wallMaterialType.None && a2 == "N") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x + 0.64f, zero4.y)).wallMaterial == wallMaterialType.None && a2 == "E") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x - 0.64f, zero4.y)).wallMaterial != wallMaterialType.None && a2 == "E") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x - 0.64f, zero4.y + 0.64f)).wallMaterial != wallMaterialType.None && a2 == "E") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x - 0.64f, zero4.y - 0.64f)).wallMaterial != wallMaterialType.None && a2 == "E") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x, zero4.y - 0.64f)).wallMaterial == wallMaterialType.None && a2 == "S") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x, zero4.y + 0.64f)).wallMaterial != wallMaterialType.None && a2 == "S") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x + 0.64f, zero4.y + 0.64f)).wallMaterial != wallMaterialType.None && a2 == "S") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x - 0.64f, zero4.y + 0.64f)).wallMaterial != wallMaterialType.None && a2 == "S") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x - 0.64f, zero4.y)).wallMaterial == wallMaterialType.None && a2 == "W") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x + 0.64f, zero4.y)).wallMaterial != wallMaterialType.None && a2 == "W") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x + 0.64f, zero4.y + 0.64f)).wallMaterial != wallMaterialType.None && a2 == "W") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x + 0.64f, zero4.y - 0.64f)).wallMaterial != wallMaterialType.None && a2 == "W"))
										flag9 = false;

									if (GC.tileInfo.IsOverlapping(zero4, "Anything"))
										flag9 = false;

									if (GC.tileInfo.IsOverlapping(zero4, "ObjectRealSprite", 0.64f))
										flag9 = false;

									if (flag9 && zero4 != Vector2.zero)
									{
										GC.spawnerMain.spawnObjectReal(zero4, null, "TrashCan").ShiftTowardWalls();
										num2 = i;
										i = num2 + 1;
									}
									else
									{
										GC.tileInfo.GetTileData(zero5);
										flag9 = true;

										if ((GC.tileInfo.GetTileData(new Vector2(zero5.x, zero5.y + 0.64f)).wallMaterial == wallMaterialType.None && a2 == "N") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x, zero5.y - 0.64f)).wallMaterial != wallMaterialType.None && a2 == "N") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x + 0.64f, zero5.y - 0.64f)).wallMaterial != wallMaterialType.None && a2 == "N") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x - 0.64f, zero5.y - 0.64f)).wallMaterial != wallMaterialType.None && a2 == "N") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x + 0.64f, zero5.y)).wallMaterial == wallMaterialType.None && a2 == "E") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x - 0.64f, zero5.y)).wallMaterial != wallMaterialType.None && a2 == "E") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x - 0.64f, zero5.y + 0.64f)).wallMaterial != wallMaterialType.None && a2 == "E") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x - 0.64f, zero5.y - 0.64f)).wallMaterial != wallMaterialType.None && a2 == "E") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x, zero5.y - 0.64f)).wallMaterial == wallMaterialType.None && a2 == "S") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x, zero5.y + 0.64f)).wallMaterial != wallMaterialType.None && a2 == "S") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x + 0.64f, zero5.y + 0.64f)).wallMaterial != wallMaterialType.None && a2 == "S") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x - 0.64f, zero5.y + 0.64f)).wallMaterial != wallMaterialType.None && a2 == "S") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x - 0.64f, zero5.y)).wallMaterial == wallMaterialType.None && a2 == "W") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x + 0.64f, zero5.y)).wallMaterial != wallMaterialType.None && a2 == "W") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x + 0.64f, zero5.y + 0.64f)).wallMaterial != wallMaterialType.None && a2 == "W") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x + 0.64f, zero5.y - 0.64f)).wallMaterial != wallMaterialType.None && a2 == "W"))
											flag9 = false;

										if (GC.tileInfo.IsOverlapping(zero5, "Anything"))
											flag9 = false;

										if (GC.tileInfo.IsOverlapping(zero5, "ObjectRealSprite", 0.64f))
											flag9 = false;

										if (flag9 && zero5 != Vector2.zero)
										{
											GC.spawnerMain.spawnObjectReal(zero5, null, "TrashCan").ShiftTowardWalls();
											num2 = i;
											i = num2 + 1;
										}
									}
								}
							}

							if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
							{
								yield return null;
								chunkStartTime = Time.realtimeSinceStartup;
							}

							Random.InitState(__instance.randomSeedNum + i);
							num2 = i;
						}
					}
					#endregion
					#region Mod - SpelunkyDory / Cave Wall Outcroppings (Shelved)
					if (false) // Cave Wall isn't in the enum used in the wall generation code, so I am not sure how to make this work right.
					{
						Debug.Log("Loading SpelunkyDory Cave Wall Outcroppings");
						int bigTries = (int)((float)Random.Range(48, 64) * __instance.levelSizeModifier);
						List<int> spawnedCount = new List<int>();
						int itemCountIterator;

						for (int numObjects = 0; numObjects < bigTries; numObjects = itemCountIterator + 1)
						{
							Vector2 spotCandidate = Vector2.zero;
							int spotsTried = 0;

							do
							{
								spotCandidate = GC.tileInfo.FindRandLocationNearWall(0.64f);

								if (spotCandidate != Vector2.zero)
								{
									TileData spotTileData = GC.tileInfo.GetTileData(spotCandidate);

									//if (GC.tileInfo.GetTileData(new Vector2(spotCandidate.x, spotCandidate.y + 0.64f)).owner == 0 &&
									//	GC.tileInfo.GetTileData(new Vector2(spotCandidate.x + 0.64f, spotCandidate.y)).owner == 0 &&
									//	GC.tileInfo.GetTileData(new Vector2(spotCandidate.x, spotCandidate.y - 0.64f)).owner == 0 &&
									//	GC.tileInfo.GetTileData(new Vector2(spotCandidate.x - 0.64f, spotCandidate.y)).owner == 0)
									//	spotCandidate = Vector2.zero;

									if (!GC.tileInfo.IsOverlapping(new Vector2(spotCandidate.x, spotCandidate.y + 0.64f), "Wall") &&
										!GC.tileInfo.IsOverlapping(new Vector2(spotCandidate.x, spotCandidate.y - 0.64f), "Wall") &&
										!GC.tileInfo.IsOverlapping(new Vector2(spotCandidate.x + 0.64f, spotCandidate.y), "Wall") &&
										!GC.tileInfo.IsOverlapping(new Vector2(spotCandidate.x - 0.64f, spotCandidate.y), "Wall"))
										spotCandidate = Vector2.zero;

									if (GC.tileInfo.IsOverlapping(spotCandidate, "ObjectRealSprite", 0.64f))
										spotCandidate = Vector2.zero;

									if (spawnedCount.Contains(spotTileData.chunkID))
										spotCandidate = Vector2.zero;

									if (GC.tileInfo.DestroyIfBetweenWalls(spotCandidate))
										spotCandidate = Vector2.zero;
								}

								spotsTried++;
							}
							while ((spotCandidate == Vector2.zero || Vector2.Distance(spotCandidate, GC.playerAgent.tr.position) < 5f) && spotsTried < 100);

							if (spotCandidate != Vector2.zero)
							{
								GC.tileInfo.BuildWallTileAtPosition(spotCandidate.x, spotCandidate.y, wallMaterialType.Normal);
								TileData spotTileData = GC.tileInfo.GetTileData(spotCandidate);
								spawnedCount.Add(spotTileData.chunkID);
								Random.InitState(__instance.randomSeedNum + numObjects + ++randomCount);

								if (numObjects < bigTries - 1 && GC.percentChance(25))
								{
									string adjoiningWall = "";
									Vector2 leftOfSpot = Vector2.zero;
									Vector2 rightOfSpot = Vector2.zero;

									if (GC.tileInfo.GetTileData(new Vector2(spotCandidate.x, spotCandidate.y + 0.64f)).wallMaterial != wallMaterialType.None)
									{
										leftOfSpot = new Vector2(spotCandidate.x + 1.28f, spotCandidate.y);
										rightOfSpot = new Vector2(spotCandidate.x - 1.28f, spotCandidate.y);
										adjoiningWall = "N";
									}
									else if (GC.tileInfo.GetTileData(new Vector2(spotCandidate.x, spotCandidate.y - 0.64f)).wallMaterial != wallMaterialType.None)
									{
										leftOfSpot = new Vector2(spotCandidate.x + 1.28f, spotCandidate.y);
										rightOfSpot = new Vector2(spotCandidate.x - 1.28f, spotCandidate.y);
										adjoiningWall = "S";
									}
									else if (GC.tileInfo.GetTileData(new Vector2(spotCandidate.x + 0.64f, spotCandidate.y)).wallMaterial != wallMaterialType.None)
									{
										leftOfSpot = new Vector2(spotCandidate.x, spotCandidate.y + 1.28f);
										rightOfSpot = new Vector2(spotCandidate.x, spotCandidate.y - 1.28f);
										adjoiningWall = "E";
									}
									else if (GC.tileInfo.GetTileData(new Vector2(spotCandidate.x - 0.64f, spotCandidate.y)).wallMaterial != wallMaterialType.None)
									{
										leftOfSpot = new Vector2(spotCandidate.x, spotCandidate.y + 1.28f);
										rightOfSpot = new Vector2(spotCandidate.x, spotCandidate.y - 1.28f);
										adjoiningWall = "W";
									}

									GC.tileInfo.GetTileData(leftOfSpot);
									bool isSpotAcceptable = true;

									if ((GC.tileInfo.GetTileData(new Vector2(leftOfSpot.x, leftOfSpot.y + 0.64f)).wallMaterial == wallMaterialType.None && adjoiningWall == "N") ||
										(GC.tileInfo.GetTileData(new Vector2(leftOfSpot.x, leftOfSpot.y - 0.64f)).wallMaterial != wallMaterialType.None && adjoiningWall == "N") ||
										(GC.tileInfo.GetTileData(new Vector2(leftOfSpot.x + 0.64f, leftOfSpot.y - 0.64f)).wallMaterial != wallMaterialType.None && adjoiningWall == "N") ||
										(GC.tileInfo.GetTileData(new Vector2(leftOfSpot.x - 0.64f, leftOfSpot.y - 0.64f)).wallMaterial != wallMaterialType.None && adjoiningWall == "N") ||
										(GC.tileInfo.GetTileData(new Vector2(leftOfSpot.x + 0.64f, leftOfSpot.y)).wallMaterial == wallMaterialType.None && adjoiningWall == "E") ||
										(GC.tileInfo.GetTileData(new Vector2(leftOfSpot.x - 0.64f, leftOfSpot.y)).wallMaterial != wallMaterialType.None && adjoiningWall == "E") ||
										(GC.tileInfo.GetTileData(new Vector2(leftOfSpot.x - 0.64f, leftOfSpot.y + 0.64f)).wallMaterial != wallMaterialType.None && adjoiningWall == "E") ||
										(GC.tileInfo.GetTileData(new Vector2(leftOfSpot.x - 0.64f, leftOfSpot.y - 0.64f)).wallMaterial != wallMaterialType.None && adjoiningWall == "E") ||
										(GC.tileInfo.GetTileData(new Vector2(leftOfSpot.x, leftOfSpot.y - 0.64f)).wallMaterial == wallMaterialType.None && adjoiningWall == "S") ||
										(GC.tileInfo.GetTileData(new Vector2(leftOfSpot.x, leftOfSpot.y + 0.64f)).wallMaterial != wallMaterialType.None && adjoiningWall == "S") ||
										(GC.tileInfo.GetTileData(new Vector2(leftOfSpot.x + 0.64f, leftOfSpot.y + 0.64f)).wallMaterial != wallMaterialType.None && adjoiningWall == "S") ||
										(GC.tileInfo.GetTileData(new Vector2(leftOfSpot.x - 0.64f, leftOfSpot.y + 0.64f)).wallMaterial != wallMaterialType.None && adjoiningWall == "S") ||
										(GC.tileInfo.GetTileData(new Vector2(leftOfSpot.x - 0.64f, leftOfSpot.y)).wallMaterial == wallMaterialType.None && adjoiningWall == "W") ||
										(GC.tileInfo.GetTileData(new Vector2(leftOfSpot.x + 0.64f, leftOfSpot.y)).wallMaterial != wallMaterialType.None && adjoiningWall == "W") ||
										(GC.tileInfo.GetTileData(new Vector2(leftOfSpot.x + 0.64f, leftOfSpot.y + 0.64f)).wallMaterial != wallMaterialType.None && adjoiningWall == "W") ||
										(GC.tileInfo.GetTileData(new Vector2(leftOfSpot.x + 0.64f, leftOfSpot.y - 0.64f)).wallMaterial != wallMaterialType.None && adjoiningWall == "W"))
										isSpotAcceptable = false;

									if (GC.tileInfo.IsOverlapping(leftOfSpot, "Anything"))
										isSpotAcceptable = false;

									if (GC.tileInfo.IsOverlapping(leftOfSpot, "ObjectRealSprite", 0.64f))
										isSpotAcceptable = false;

									if (isSpotAcceptable && leftOfSpot != Vector2.zero)
									{
										GC.tileInfo.BuildWallTileAtPosition(spotCandidate.x, spotCandidate.y, wallMaterialType.Normal);
										itemCountIterator = numObjects;
										numObjects = itemCountIterator + 1;
									}
									else
									{
										GC.tileInfo.GetTileData(rightOfSpot);
										isSpotAcceptable = true;

										if ((GC.tileInfo.GetTileData(new Vector2(rightOfSpot.x, rightOfSpot.y + 0.64f)).wallMaterial == wallMaterialType.None && adjoiningWall == "N") ||
											(GC.tileInfo.GetTileData(new Vector2(rightOfSpot.x, rightOfSpot.y - 0.64f)).wallMaterial != wallMaterialType.None && adjoiningWall == "N") ||
											(GC.tileInfo.GetTileData(new Vector2(rightOfSpot.x + 0.64f, rightOfSpot.y - 0.64f)).wallMaterial != wallMaterialType.None && adjoiningWall == "N") ||
											(GC.tileInfo.GetTileData(new Vector2(rightOfSpot.x - 0.64f, rightOfSpot.y - 0.64f)).wallMaterial != wallMaterialType.None && adjoiningWall == "N") ||
											(GC.tileInfo.GetTileData(new Vector2(rightOfSpot.x + 0.64f, rightOfSpot.y)).wallMaterial == wallMaterialType.None && adjoiningWall == "E") ||
											(GC.tileInfo.GetTileData(new Vector2(rightOfSpot.x - 0.64f, rightOfSpot.y)).wallMaterial != wallMaterialType.None && adjoiningWall == "E") ||
											(GC.tileInfo.GetTileData(new Vector2(rightOfSpot.x - 0.64f, rightOfSpot.y + 0.64f)).wallMaterial != wallMaterialType.None && adjoiningWall == "E") ||
											(GC.tileInfo.GetTileData(new Vector2(rightOfSpot.x - 0.64f, rightOfSpot.y - 0.64f)).wallMaterial != wallMaterialType.None && adjoiningWall == "E") ||
											(GC.tileInfo.GetTileData(new Vector2(rightOfSpot.x, rightOfSpot.y - 0.64f)).wallMaterial == wallMaterialType.None && adjoiningWall == "S") ||
											(GC.tileInfo.GetTileData(new Vector2(rightOfSpot.x, rightOfSpot.y + 0.64f)).wallMaterial != wallMaterialType.None && adjoiningWall == "S") ||
											(GC.tileInfo.GetTileData(new Vector2(rightOfSpot.x + 0.64f, rightOfSpot.y + 0.64f)).wallMaterial != wallMaterialType.None && adjoiningWall == "S") ||
											(GC.tileInfo.GetTileData(new Vector2(rightOfSpot.x - 0.64f, rightOfSpot.y + 0.64f)).wallMaterial != wallMaterialType.None && adjoiningWall == "S") ||
											(GC.tileInfo.GetTileData(new Vector2(rightOfSpot.x - 0.64f, rightOfSpot.y)).wallMaterial == wallMaterialType.None && adjoiningWall == "W") ||
											(GC.tileInfo.GetTileData(new Vector2(rightOfSpot.x + 0.64f, rightOfSpot.y)).wallMaterial != wallMaterialType.None && adjoiningWall == "W") ||
											(GC.tileInfo.GetTileData(new Vector2(rightOfSpot.x + 0.64f, rightOfSpot.y + 0.64f)).wallMaterial != wallMaterialType.None && adjoiningWall == "W") ||
											(GC.tileInfo.GetTileData(new Vector2(rightOfSpot.x + 0.64f, rightOfSpot.y - 0.64f)).wallMaterial != wallMaterialType.None && adjoiningWall == "W"))
											isSpotAcceptable = false;

										if (GC.tileInfo.IsOverlapping(rightOfSpot, "Anything"))
											isSpotAcceptable = false;

										if (GC.tileInfo.IsOverlapping(rightOfSpot, "ObjectRealSprite", 0.64f))
											isSpotAcceptable = false;

										if (isSpotAcceptable && rightOfSpot != Vector2.zero)
										{
											GC.spawnerMain.spawnObjectReal(rightOfSpot, null, "TrashCan").ShiftTowardWalls();
											itemCountIterator = numObjects;
											numObjects = itemCountIterator + 1;
										}
									}
								}
							}

							if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
							{
								yield return null;
								chunkStartTime = Time.realtimeSinceStartup;
							}

							Random.InitState(__instance.randomSeedNum + numObjects);
							itemCountIterator = numObjects;
						}

						spawnedCount = null;
					}
					#endregion
					#region Mod - SurveillanceSociety / Public Security Cams & Turrets
					if (GC.challenges.Contains(cChallenge.SurveillanceSociety))
					{
						BMLog("Loading Public Security Cams");

						int bigTries = (int)((float)Random.Range(8, 12) * __instance.levelSizeModifier);
						List<int> spawnedInChunks = new List<int>();
						int num2;

						for (int numObjects = 0; numObjects < bigTries; numObjects = num2 + 1)
						{
							Vector2 spotCandidate = Vector2.zero;
							int spotsTried = 0;

							do
							{
								spotCandidate = GC.tileInfo.FindRandLocationNearWall(0.64f);

								if (spotCandidate != Vector2.zero)
								{
									TileData spotTileData = GC.tileInfo.GetTileData(spotCandidate);

									//  We want these to generate in public.
									//if (GC.tileInfo.GetTileData(new Vector2(spotNearWall.x, spotNearWall.y + 0.64f)).owner == 0 &&
									//	GC.tileInfo.GetTileData(new Vector2(spotNearWall.x + 0.64f, spotNearWall.y)).owner == 0 &&
									//	GC.tileInfo.GetTileData(new Vector2(spotNearWall.x, spotNearWall.y - 0.64f)).owner == 0 &&
									//	GC.tileInfo.GetTileData(new Vector2(spotNearWall.x - 0.64f, spotNearWall.y)).owner == 0)
									//	spotNearWall = Vector2.zero;

									// What is the purpose of this one if we're using FindRandLocationNearWall(0.64f) above??
									if (!GC.tileInfo.IsOverlapping(new Vector2(spotCandidate.x, spotCandidate.y + 0.64f), "Wall") &&
										!GC.tileInfo.IsOverlapping(new Vector2(spotCandidate.x, spotCandidate.y - 0.64f), "Wall") &&
										!GC.tileInfo.IsOverlapping(new Vector2(spotCandidate.x + 0.64f, spotCandidate.y), "Wall") &&
										!GC.tileInfo.IsOverlapping(new Vector2(spotCandidate.x - 0.64f, spotCandidate.y), "Wall"))
									{
										spotCandidate = Vector2.zero;
									}

									if (GC.tileInfo.IsOverlapping(spotCandidate, "ObjectRealSprite", 0.64f))
										spotCandidate = Vector2.zero;

									if (spawnedInChunks.Contains(spotTileData.chunkID))
										spotCandidate = Vector2.zero;

									if (GC.tileInfo.DestroyIfBetweenWalls(spotCandidate))
										spotCandidate = Vector2.zero;
								}

								spotsTried++;
							}
							while ((spotCandidate == Vector2.zero || Vector2.Distance(spotCandidate, GC.playerAgent.tr.position) < 5f) && spotsTried < 100);

							if (spotCandidate != Vector2.zero)
							{
								ObjectReal securityCam = GC.spawnerMain.spawnObjectReal(spotCandidate, null, vObject.SecurityCam);
								securityCam.ShiftTowardWalls();

								securityCam.owner = 85;
								SecurityCam securityCamCast = (SecurityCam)securityCam;
								securityCamCast.securityType = "Noise";
								securityCamCast.targets = "Wanted";
								TileData spotTileData = GC.tileInfo.GetTileData(spotCandidate);
								spawnedInChunks.Add(spotTileData.chunkID);
								Random.InitState(__instance.randomSeedNum + numObjects + ++randomCount);

								if (numObjects < bigTries - 1 && GC.percentChance(25))
								{
									string wallEdge = "";
									Vector2 leftOfSpot = Vector2.zero;
									Vector2 rightOfSpot = Vector2.zero;

									if (GC.tileInfo.GetTileData(new Vector2(spotCandidate.x, spotCandidate.y + 0.64f)).wallMaterial != wallMaterialType.None)
									{
										leftOfSpot = new Vector2(spotCandidate.x + 1.28f, spotCandidate.y);
										rightOfSpot = new Vector2(spotCandidate.x - 1.28f, spotCandidate.y);
										wallEdge = "N";
										securityCam.direction = "S";
									}
									else if (GC.tileInfo.GetTileData(new Vector2(spotCandidate.x, spotCandidate.y - 0.64f)).wallMaterial != wallMaterialType.None)
									{
										leftOfSpot = new Vector2(spotCandidate.x + 1.28f, spotCandidate.y);
										rightOfSpot = new Vector2(spotCandidate.x - 1.28f, spotCandidate.y);
										wallEdge = "S";
										securityCam.direction = "N";
									}
									else if (GC.tileInfo.GetTileData(new Vector2(spotCandidate.x + 0.64f, spotCandidate.y)).wallMaterial != wallMaterialType.None)
									{
										leftOfSpot = new Vector2(spotCandidate.x, spotCandidate.y + 1.28f);
										rightOfSpot = new Vector2(spotCandidate.x, spotCandidate.y - 1.28f);
										wallEdge = "E";
										securityCam.direction = "W";
									}
									else if (GC.tileInfo.GetTileData(new Vector2(spotCandidate.x - 0.64f, spotCandidate.y)).wallMaterial != wallMaterialType.None)
									{
										leftOfSpot = new Vector2(spotCandidate.x, spotCandidate.y + 1.28f);
										rightOfSpot = new Vector2(spotCandidate.x, spotCandidate.y - 1.28f);
										wallEdge = "W";
										securityCam.direction = "E";
									}

									GC.tileInfo.GetTileData(leftOfSpot);
									bool isSpotAcceptable = true;

									#region Refactor - for readability, not performance 
									//// Proceeding clockwise:
									//Vector2 neighborN = new Vector2(leftOfSpot.x, leftOfSpot.y + 0.64f);
									//Vector2 neighborNE = new Vector2(leftOfSpot.x + 0.64f, leftOfSpot.y + 0.64f);
									//Vector2 neighborE = new Vector2(leftOfSpot.x + 0.64f, leftOfSpot.y);
									//Vector2 neighborSE = new Vector2(leftOfSpot.x + 0.64f, leftOfSpot.y - 0.64f);
									//Vector2 neighborS = new Vector2(leftOfSpot.x, leftOfSpot.y - 0.64f);
									//Vector2 neibhorSW = new Vector2(leftOfSpot.x - 0.64f, leftOfSpot.y - 0.64f);
									//Vector2 neighborW = new Vector2(leftOfSpot.x - 0.64f, leftOfSpot.y);
									//Vector2 neighborNW = new Vector2(leftOfSpot.x - 0.64f, leftOfSpot.y + 0.64f);

									//switch (wallEdge)
									//{
									//	case "N":
									//		if ((GC.tileInfo.GetTileData(neighborN).wallMaterial == wallMaterialType.None) ||
									//			(GC.tileInfo.GetTileData(neighborS).wallMaterial != wallMaterialType.None) ||
									//			(GC.tileInfo.GetTileData(neighborSE).wallMaterial != wallMaterialType.None) ||
									//			(GC.tileInfo.GetTileData(neibhorSW).wallMaterial != wallMaterialType.None))
									//			isSpotAcceptable = false;

									//		break;

									//	case "E":
									//		if ((GC.tileInfo.GetTileData(neighborE).wallMaterial == wallMaterialType.None) ||
									//			(GC.tileInfo.GetTileData(neighborW).wallMaterial != wallMaterialType.None) ||
									//			(GC.tileInfo.GetTileData(neighborNW).wallMaterial != wallMaterialType.None) ||
									//			(GC.tileInfo.GetTileData(neibhorSW).wallMaterial != wallMaterialType.None))
									//			isSpotAcceptable = false;

									//		break;

									//	case "S":
									//		if ((GC.tileInfo.GetTileData(neighborS).wallMaterial == wallMaterialType.None) ||
									//			(GC.tileInfo.GetTileData(neighborN).wallMaterial != wallMaterialType.None) ||
									//			(GC.tileInfo.GetTileData(neighborNE).wallMaterial != wallMaterialType.None) ||
									//			(GC.tileInfo.GetTileData(neighborNW).wallMaterial != wallMaterialType.None))
									//			isSpotAcceptable = false;

									//		break;

									//	case "W":
									//		if ((GC.tileInfo.GetTileData(neighborW).wallMaterial == wallMaterialType.None) ||
									//			(GC.tileInfo.GetTileData(neighborE).wallMaterial != wallMaterialType.None) ||
									//			(GC.tileInfo.GetTileData(neighborNE).wallMaterial != wallMaterialType.None) ||
									//			(GC.tileInfo.GetTileData(neighborSE).wallMaterial != wallMaterialType.None))
									//			isSpotAcceptable = false;

									//		break;
									//}
									#endregion

									if ((GC.tileInfo.GetTileData(new Vector2(leftOfSpot.x, leftOfSpot.y + 0.64f)).wallMaterial == wallMaterialType.None && wallEdge == "N") ||
										(GC.tileInfo.GetTileData(new Vector2(leftOfSpot.x, leftOfSpot.y - 0.64f)).wallMaterial != wallMaterialType.None && wallEdge == "N") ||
										(GC.tileInfo.GetTileData(new Vector2(leftOfSpot.x + 0.64f, leftOfSpot.y - 0.64f)).wallMaterial != wallMaterialType.None && wallEdge == "N") ||
										(GC.tileInfo.GetTileData(new Vector2(leftOfSpot.x - 0.64f, leftOfSpot.y - 0.64f)).wallMaterial != wallMaterialType.None && wallEdge == "N") ||
										(GC.tileInfo.GetTileData(new Vector2(leftOfSpot.x + 0.64f, leftOfSpot.y)).wallMaterial == wallMaterialType.None && wallEdge == "E") ||
										(GC.tileInfo.GetTileData(new Vector2(leftOfSpot.x - 0.64f, leftOfSpot.y)).wallMaterial != wallMaterialType.None && wallEdge == "E") ||
										(GC.tileInfo.GetTileData(new Vector2(leftOfSpot.x - 0.64f, leftOfSpot.y + 0.64f)).wallMaterial != wallMaterialType.None && wallEdge == "E") ||
										(GC.tileInfo.GetTileData(new Vector2(leftOfSpot.x - 0.64f, leftOfSpot.y - 0.64f)).wallMaterial != wallMaterialType.None && wallEdge == "E") ||
										(GC.tileInfo.GetTileData(new Vector2(leftOfSpot.x, leftOfSpot.y - 0.64f)).wallMaterial == wallMaterialType.None && wallEdge == "S") ||
										(GC.tileInfo.GetTileData(new Vector2(leftOfSpot.x, leftOfSpot.y + 0.64f)).wallMaterial != wallMaterialType.None && wallEdge == "S") ||
										(GC.tileInfo.GetTileData(new Vector2(leftOfSpot.x + 0.64f, leftOfSpot.y + 0.64f)).wallMaterial != wallMaterialType.None && wallEdge == "S") ||
										(GC.tileInfo.GetTileData(new Vector2(leftOfSpot.x - 0.64f, leftOfSpot.y + 0.64f)).wallMaterial != wallMaterialType.None && wallEdge == "S") ||
										(GC.tileInfo.GetTileData(new Vector2(leftOfSpot.x - 0.64f, leftOfSpot.y)).wallMaterial == wallMaterialType.None && wallEdge == "W") ||
										(GC.tileInfo.GetTileData(new Vector2(leftOfSpot.x + 0.64f, leftOfSpot.y)).wallMaterial != wallMaterialType.None && wallEdge == "W") ||
										(GC.tileInfo.GetTileData(new Vector2(leftOfSpot.x + 0.64f, leftOfSpot.y + 0.64f)).wallMaterial != wallMaterialType.None && wallEdge == "W") ||
										(GC.tileInfo.GetTileData(new Vector2(leftOfSpot.x + 0.64f, leftOfSpot.y - 0.64f)).wallMaterial != wallMaterialType.None && wallEdge == "W"))
										isSpotAcceptable = false;

									if (GC.tileInfo.IsOverlapping(leftOfSpot, "Anything"))
										isSpotAcceptable = false;

									if (GC.tileInfo.IsOverlapping(leftOfSpot, "ObjectRealSprite", 0.64f))
										isSpotAcceptable = false;

									if (isSpotAcceptable && leftOfSpot != Vector2.zero)
									{
										ObjectReal turret1 = GC.spawnerMain.spawnObjectReal(leftOfSpot, null, vObject.Turret);
										turret1.ShiftTowardWalls();

										turret1.direction = securityCam.direction;
										turret1.owner = 85;
										num2 = numObjects;
										numObjects = num2 + 1;
									}
									else
									{
										GC.tileInfo.GetTileData(rightOfSpot);
										isSpotAcceptable = true;

										if ((GC.tileInfo.GetTileData(new Vector2(rightOfSpot.x, rightOfSpot.y + 0.64f)).wallMaterial == wallMaterialType.None && wallEdge == "N") ||
											(GC.tileInfo.GetTileData(new Vector2(rightOfSpot.x, rightOfSpot.y - 0.64f)).wallMaterial != wallMaterialType.None && wallEdge == "N") ||
											(GC.tileInfo.GetTileData(new Vector2(rightOfSpot.x + 0.64f, rightOfSpot.y - 0.64f)).wallMaterial != wallMaterialType.None && wallEdge == "N") ||
											(GC.tileInfo.GetTileData(new Vector2(rightOfSpot.x - 0.64f, rightOfSpot.y - 0.64f)).wallMaterial != wallMaterialType.None && wallEdge == "N") ||
											(GC.tileInfo.GetTileData(new Vector2(rightOfSpot.x + 0.64f, rightOfSpot.y)).wallMaterial == wallMaterialType.None && wallEdge == "E") ||
											(GC.tileInfo.GetTileData(new Vector2(rightOfSpot.x - 0.64f, rightOfSpot.y)).wallMaterial != wallMaterialType.None && wallEdge == "E") ||
											(GC.tileInfo.GetTileData(new Vector2(rightOfSpot.x - 0.64f, rightOfSpot.y + 0.64f)).wallMaterial != wallMaterialType.None && wallEdge == "E") ||
											(GC.tileInfo.GetTileData(new Vector2(rightOfSpot.x - 0.64f, rightOfSpot.y - 0.64f)).wallMaterial != wallMaterialType.None && wallEdge == "E") ||
											(GC.tileInfo.GetTileData(new Vector2(rightOfSpot.x, rightOfSpot.y - 0.64f)).wallMaterial == wallMaterialType.None && wallEdge == "S") ||
											(GC.tileInfo.GetTileData(new Vector2(rightOfSpot.x, rightOfSpot.y + 0.64f)).wallMaterial != wallMaterialType.None && wallEdge == "S") ||
											(GC.tileInfo.GetTileData(new Vector2(rightOfSpot.x + 0.64f, rightOfSpot.y + 0.64f)).wallMaterial != wallMaterialType.None && wallEdge == "S") ||
											(GC.tileInfo.GetTileData(new Vector2(rightOfSpot.x - 0.64f, rightOfSpot.y + 0.64f)).wallMaterial != wallMaterialType.None && wallEdge == "S") ||
											(GC.tileInfo.GetTileData(new Vector2(rightOfSpot.x - 0.64f, rightOfSpot.y)).wallMaterial == wallMaterialType.None && wallEdge == "W") ||
											(GC.tileInfo.GetTileData(new Vector2(rightOfSpot.x + 0.64f, rightOfSpot.y)).wallMaterial != wallMaterialType.None && wallEdge == "W") ||
											(GC.tileInfo.GetTileData(new Vector2(rightOfSpot.x + 0.64f, rightOfSpot.y + 0.64f)).wallMaterial != wallMaterialType.None && wallEdge == "W") ||
											(GC.tileInfo.GetTileData(new Vector2(rightOfSpot.x + 0.64f, rightOfSpot.y - 0.64f)).wallMaterial != wallMaterialType.None && wallEdge == "W"))
											isSpotAcceptable = false;

										if (GC.tileInfo.IsOverlapping(rightOfSpot, "Anything"))
											isSpotAcceptable = false;

										if (GC.tileInfo.IsOverlapping(rightOfSpot, "ObjectRealSprite", 0.64f))
											isSpotAcceptable = false;

										if (isSpotAcceptable && rightOfSpot != Vector2.zero)
										{
											ObjectReal turret2 = GC.spawnerMain.spawnObjectReal(rightOfSpot, null, vObject.Turret);
											turret2.ShiftTowardWalls();

											turret2.direction = securityCam.direction;
											turret2.owner = 85;
											securityCamCast.turrets.Add((Turret)turret2);
											securityCamCast.securityType = "Turret";
											num2 = numObjects;
											numObjects = num2 + 1;
										}
									}
								}
							}

							if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
							{
								yield return null;
								chunkStartTime = Time.realtimeSinceStartup;
							}

							Random.InitState(__instance.randomSeedNum + numObjects);
							num2 = numObjects;
						}
					}
					#endregion
					#region Mod - LitterallyTheWorst / Litter
					if ((GC.customLevel && __instance.customLevel.levelFeatures.Contains(cLevelFeature.Litter)) ||
						GC.challenges.Contains(cChallenge.LitterallyTheWorst))
					{
						Debug.Log("Loading Litter");

						int numObjects = (int)((5 - GC.levelTheme) * 20 * __instance.levelSizeModifier);
						int num2;

						for (int i = 0; i < numObjects; i = num2 + 1)
						{
							Vector2 spot = Vector2.zero;
							int i2 = 0;

							do
							{
								spot = GC.tileInfo.FindRandLocationGeneral(0f); // Vanilla 2f

								i2++;
							}
							while (spot == Vector2.zero && i2 < 100);

							if (spot != Vector2.zero)
								GC.spawnerMain.SpawnWreckagePileObject(spot, GC.Choose<string>(vObject.Shelf, vObject.Lamp, vObject.Counter, vObject.VendorCart), false);
								
							if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
							{
								yield return null;
								chunkStartTime = Time.realtimeSinceStartup;
							}

							Random.InitState(__instance.randomSeedNum + i);
							num2 = i;
						}
					}
					#endregion
					#region Mod - BadNeighborhoods / Broken Windows
					if ((GC.challenges.Contains("MixedUpLevels") && GC.percentChance(33)) ||
						(GC.customLevel && __instance.customLevel.levelFeatures.Contains(cLevelFeature.BrokenWindows)) ||
						GC.challenges.Contains(cChallenge.BadNeighborhoods))
					{
						BMLog("Breaking Windows");

						List<Window> breakUs = new List<Window>();

						foreach (ObjectReal objReal in GC.objectRealList)
							if (objReal is Window window && GC.percentChance(2))
								breakUs.Add(window);

						if (breakUs.Count > 0)
							foreach (Window window in breakUs)
								window.DamagedObject(window, 0f);
					}
					#endregion
					#region Vendor Carts
					bool hasVendorCarts = false;

					if (GC.levelTheme == 2 ||
						(GC.customLevel && __instance.customLevel.levelFeatures.Contains("VendorCart")) ||
						GC.challenges.Contains(cChallenge.CartOfTheDeal))
						hasVendorCarts = true;

					if (GC.levelFeeling == "HarmAtIntervals" || GC.levelFeeling == "Lockdown" || GC.levelFeeling == "WarZone")
						hasVendorCarts = false;

					if (hasVendorCarts)
					{
						Debug.Log("Loading Vendor Carts");
						int bigTries = 2;
						int i2;

						for (int i = 0; i < bigTries; i = i2 + 1)
						{
							Vector2 vector8 = Vector2.zero;
							int num23 = 0;

							do
							{
								vector8 = GC.tileInfo.FindRandLocationGeneral(2f);

								for (int num24 = 0; num24 < GC.objectRealList.Count; num24++)
									if (GC.objectRealList[num24].objectName == "VendorCart" && Vector2.Distance(GC.objectRealList[num24].tr.position, vector8) < 28f)
										vector8 = Vector2.zero;

								if (vector8 != Vector2.zero)
								{
									if (GC.tileInfo.WaterNearby(vector8))
										vector8 = Vector2.zero;

									if (GC.tileInfo.WaterNearby2(vector8))
										vector8 = Vector2.zero;

									if (GC.tileInfo.IceNearby(vector8))
										vector8 = Vector2.zero;

									if (GC.tileInfo.BridgeNearby(vector8))
										vector8 = Vector2.zero;
								}

								num23++;
							}
							while ((vector8 == Vector2.zero || Vector2.Distance(vector8, GC.playerAgent.tr.position) < 10f) && num23 < 100);

							if (vector8 != Vector2.zero && num23 < 100)
							{
								ObjectReal objectReal4 = GC.spawnerMain.spawnObjectReal(vector8, null, "VendorCart");
								Vector2 vector9 = Vector2.zero;
								int num25 = 0;

								do
								{
									vector9 = GC.tileInfo.FindLocationNearLocation(vector8, null, 0.64f, 1.28f, true, true);
									num25++;
								}
								while (vector9 == Vector2.zero && num25 < 500);

								if (num25 >= 500 && vector9 == Vector2.zero)
									objectReal4.RemoveMe();
								else if (vector9 != Vector2.zero)
								{
									string agentType = "Shopkeeper";

									if (i == 1)
										agentType = "Bartender";

									Agent agent = GC.spawnerMain.SpawnAgent(vector9, null, agentType);
									agent.SetDefaultGoal("Guard");
									agent.startingAngle = 270;
									objectReal4.owner = 255;
									agent.ownerID = 255;
								}
							}

							if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
							{
								yield return null;
								chunkStartTime = Time.realtimeSinceStartup;
							}

							Random.InitState(__instance.randomSeedNum + i);
							i2 = i;
						}
					}
					#endregion
					#region PoliceBoxes
					bool hasPoliceBoxes = false;

					if (GC.levelTheme == 3 || GC.levelTheme == 4 || __instance.hasLockdownWalls || 
						(GC.challenges.Contains("MixedUpLevels") && GC.percentChance(20)) ||
						GC.challenges.Contains(cChallenge.PoliceState) || GC.challenges.Contains(cChallenge.MACITS) ||
						(GC.customLevel && __instance.customLevel.levelFeatures.Contains("PoliceBox")))
						hasPoliceBoxes = true;
					
					if (GC.challenges.Contains(cChallenge.AnCapistan))
						hasPoliceBoxes = false;

					if (hasPoliceBoxes)
					{
						Debug.Log("Loading Police Boxes");

						if (GC.levelTheme == 4 || GC.challenges.Contains(cChallenge.PoliceState))
						{
							int i2;

							for (int i = 0; i < GC.tileInfo.lockdownZones.Count; i = i2 + 1)
							{
								Vector2 vector10 = Vector2.zero;
								int num26 = 0;

								do
								{
									vector10 = GC.tileInfo.FindRandLocationGeneral(2f);

									for (int num27 = 0; num27 < GC.objectRealList.Count; num27++)
										if (GC.objectRealList[num27].objectName == "PoliceBox" && Vector2.Distance(GC.objectRealList[num27].tr.position, vector10) < 14f)
											vector10 = Vector2.zero;

									if (vector10 != Vector2.zero)
									{
										if (GC.tileInfo.WaterNearby(vector10))
											vector10 = Vector2.zero;

										if (GC.tileInfo.IceNearby(vector10))
											vector10 = Vector2.zero;

										if (GC.tileInfo.BridgeNearby(vector10))
											vector10 = Vector2.zero;

										if (GC.tileInfo.GetTileData(vector10).lockdownZone != GC.tileInfo.lockdownZones[i])
											vector10 = Vector2.zero;
									}

									num26++;
								}
								while ((vector10 == Vector2.zero || Vector2.Distance(vector10, GC.playerAgent.tr.position) < 5f) && num26 < 500);

								if (vector10 != Vector2.zero)
									GC.spawnerMain.spawnObjectReal(vector10, null, "PoliceBox");

								if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
								{
									yield return null;
									chunkStartTime = Time.realtimeSinceStartup;
								}

								Random.InitState(__instance.randomSeedNum + i);
								i2 = i;
							}
						}
						else
						{
							int bigTries = (int)((float)Random.Range(6, 10) * __instance.levelSizeModifier);
							int num2;

							for (int numObjects = 0; numObjects < bigTries; numObjects = num2 + 1)
							{
								Vector2 vector11 = Vector2.zero;
								int attempts = 0;

								do
								{
									vector11 = GC.tileInfo.FindRandLocationGeneral(2f);

									for (int num29 = 0; num29 < GC.objectRealList.Count; num29++)
										if (GC.objectRealList[num29].objectName == "PoliceBox" && Vector2.Distance(GC.objectRealList[num29].tr.position, vector11) < 14f)
											vector11 = Vector2.zero;

									if (vector11 != Vector2.zero)
									{
										if (GC.tileInfo.WaterNearby(vector11))
											vector11 = Vector2.zero;

										if (GC.tileInfo.IceNearby(vector11))
											vector11 = Vector2.zero;

										if (GC.tileInfo.BridgeNearby(vector11))
											vector11 = Vector2.zero;
									}

									attempts++;
								}
								while ((vector11 == Vector2.zero || Vector2.Distance(vector11, GC.playerAgent.tr.position) < 5f) && attempts < 100);

								if (vector11 != Vector2.zero)
									GC.spawnerMain.spawnObjectReal(vector11, null, "PoliceBox");

								if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
								{
									yield return null;
									chunkStartTime = Time.realtimeSinceStartup;
								}

								Random.InitState(__instance.randomSeedNum + numObjects);
								num2 = numObjects;
							}
						}
					}
					#endregion
					#region AlarmButtons
					bool hasAlarmButtons = false;

					if (GC.levelTheme == 3 || GC.levelTheme == 4 || hasPoliceBoxes ||
						GC.challenges.Contains(cChallenge.PoliceState) || GC.challenges.Contains(cChallenge.MACITS) ||
						(GC.customLevel && __instance.customLevel.levelFeatures.Contains("AlarmButton")))
						hasAlarmButtons = true;

					if (GC.challenges.Contains(cChallenge.AnCapistan))
						hasAlarmButtons = false;

					if (hasAlarmButtons)
					{
						Debug.Log("Loading Alarm Buttons");
						int bigTries = (int)((float)Random.Range(6, 10) * __instance.levelSizeModifier);
						int i2;

						for (int i = 0; i < bigTries; i = i2 + 1)
						{
							Vector2 vector12 = Vector2.zero;
							int attempts = 0;

							do
							{
								vector12 = GC.tileInfo.FindRandLocationGeneral(2f);

								for (int num31 = 0; num31 < GC.objectRealList.Count; num31++)
									if (GC.objectRealList[num31].objectName == "AlarmButton" && Vector2.Distance(GC.objectRealList[num31].tr.position, vector12) < 14f)
										vector12 = Vector2.zero;

								if (vector12 != Vector2.zero)
								{
									if (GC.tileInfo.WaterNearby(vector12))
										vector12 = Vector2.zero;

									if (GC.tileInfo.IceNearby(vector12))
										vector12 = Vector2.zero;

									if (GC.tileInfo.BridgeNearby(vector12))
										vector12 = Vector2.zero;
								}

								attempts++;
							}
							while ((vector12 == Vector2.zero || Vector2.Distance(vector12, GC.playerAgent.tr.position) < 5f) && attempts < 100);

							if (vector12 != Vector2.zero)
								GC.spawnerMain.spawnObjectReal(vector12, null, "AlarmButton");

							if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
							{
								yield return null;
								chunkStartTime = Time.realtimeSinceStartup;
							}

							Random.InitState(__instance.randomSeedNum + i);
							i2 = i;
						}
					}
					#endregion
					#region Manholes - Warzone
					if (GC.levelFeeling == vLevelFeeling.WarZone)
					{
						Debug.Log("Loading Manholes (War Zone)");
						int bigTries = (int)((float)Random.Range(4, 8) * __instance.levelSizeModifier);
						int num2;

						for (int numObjects = 0; numObjects < bigTries; numObjects = num2 + 1)
						{
							Vector2 vector13 = Vector2.zero;
							int num32 = 0;

							do
							{
								vector13 = GC.tileInfo.FindRandLocationGeneral(1f);

								for (int num33 = 0; num33 < GC.objectRealList.Count; num33++)
									if (GC.objectRealList[num33].objectName == "Manhole" && Vector2.Distance(GC.objectRealList[num33].tr.position, vector13) < 14f)
										vector13 = Vector2.zero;

								if (vector13 != Vector2.zero)
								{
									if (GC.tileInfo.WaterNearby(vector13))
										vector13 = Vector2.zero;

									if (GC.tileInfo.IceNearby(vector13))
										vector13 = Vector2.zero;

									if (GC.tileInfo.BridgeNearby(vector13))
										vector13 = Vector2.zero;
								}

								num32++;
							}
							while ((vector13 == Vector2.zero || Vector2.Distance(vector13, GC.elevatorDown.tr.position) < 20f || Vector2.Distance(vector13, GC.elevatorUp.tr.position) < 20f) && num32 < 100);

							if (vector13 != Vector2.zero && Vector2.Distance(vector13, GC.elevatorDown.tr.position) >= 20f && Vector2.Distance(vector13, GC.elevatorUp.tr.position) >= 20f)
								GC.spawnerMain.spawnObjectReal(vector13, null, "Manhole");

							if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
							{
								yield return null;
								chunkStartTime = Time.realtimeSinceStartup;
							}

							Random.InitState(__instance.randomSeedNum + numObjects);
							num2 = numObjects;
						}
					}
					#endregion
					#region Manholes - Normal
					bool hasManholes = false;

					if ((GC.levelTheme == 3 || 
						(GC.challenges.Contains("MixedUpLevels") && GC.percentChance(33)) ||
						(GC.customLevel && __instance.customLevel.levelFeatures.Contains("Manhole"))))
						hasManholes = true;

					if (GC.challenges.Contains(cChallenge.AnCapistan) || 
						GC.levelFeeling == vLevelFeeling.WarZone)
						hasManholes = false;

					if (BMTraits.IsPlayerTraitActive(cTrait.UnderdarkCitizen)) // Trait-specific. Normal ones below.
					{
						Debug.Log("Loading Underdark Manholes");
						int bigTries = (int)((float)Random.Range(8, 12) * __instance.levelSizeModifier);
						int i2;

						for (int i = 0; i < bigTries; i = i2 + 1)
						{
							Vector2 spot = Vector2.zero;
							int spotsTried = 0;

							do
							{
								spot = GC.tileInfo.FindRandLocationGeneral(2f);

								for (int j = 0; j < GC.objectRealList.Count; j++)
									if (GC.objectRealList[j].objectName == vObject.Manhole && Vector2.Distance(GC.objectRealList[j].tr.position, spot) < 14f)
										spot = Vector2.zero;

								if (spot != Vector2.zero)
								{
									if (GC.tileInfo.WaterNearby(spot))
										spot = Vector2.zero;

									if (GC.tileInfo.IceNearby(spot))
										spot = Vector2.zero;

									if (GC.tileInfo.BridgeNearby(spot))
										spot = Vector2.zero;
								}

								spotsTried++;
							}
							while ((spot == Vector2.zero || Vector2.Distance(spot, GC.playerAgent.tr.position) < 5f) && spotsTried < 100);

							if (spot != Vector2.zero && Vector2.Distance(spot, GC.playerAgent.tr.position) >= 5f)
								GC.spawnerMain.spawnObjectReal(spot, null, vObject.Manhole);

							if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
							{
								yield return null;
								chunkStartTime = Time.realtimeSinceStartup;
							}

							Random.InitState(__instance.randomSeedNum + i);
							i2 = i;
						}

						int numObjects = (int)((float)Random.Range(2, 4) * __instance.levelSizeModifier);
						List<Manhole> manholeList = new List<Manhole>();

						for (int i = 0; i < GC.objectRealList.Count; i++)
							if (GC.objectRealList[i].objectName == vObject.Manhole)
								manholeList.Add((Manhole)GC.objectRealList[i]);

						BMLog("UDManhole List count: " + manholeList.Count());

						// Hidden Agent Placement
						if (manholeList.Count > 0)
							for (int i = 0; i < numObjects; i = i2 + 1)
							{
								int attemptsToAddHiddenAgentToManhole = 0;
								Manhole manhole;
								bool NoHiddenAgentMatch;

								do
								{
									Random.InitState(__instance.randomSeedNum + i + ++randomCount);
									manhole = manholeList[Random.Range(0, manholeList.Count)];
									NoHiddenAgentMatch = true;

									for (int j = 0; j < GC.agentList.Count; j++)
										if (GC.agentList[j].oma.hidden && Vector2.Distance(manhole.tr.position, GC.agentList[j].tr.position) < 10f)
										{
											attemptsToAddHiddenAgentToManhole++;
											NoHiddenAgentMatch = false;
										}

									attemptsToAddHiddenAgentToManhole++;
								}
								while (attemptsToAddHiddenAgentToManhole < 50 && !NoHiddenAgentMatch);

								if (NoHiddenAgentMatch)
								{
									string text3 = vAgent.Thief;

									Agent agent2 = GC.spawnerMain.SpawnAgent(manhole.tr.position, manhole, text3);
									agent2.SetDefaultGoal("Idle");
									agent2.statusEffects.BecomeHidden(manhole);
									agent2.oma.mustBeGuilty = true;
								}

								if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
								{
									yield return null;
									chunkStartTime = Time.realtimeSinceStartup;
								}

								Random.InitState(__instance.randomSeedNum + i);
								i2 = i;
							}

						manholeList = null;
					} // Custom
					else if (hasManholes) // Vanilla
					{
						Debug.Log("Loading Manholes");
						int bigTries = (int)((float)Random.Range(8, 12) * __instance.levelSizeModifier);
						int num2;

						for (int i = 0; i < bigTries; i = num2 + 1)
						{
							Vector2 locationCandidate = Vector2.zero;
							int num34 = 0;

							do
							{
								locationCandidate = GC.tileInfo.FindRandLocationGeneral(2f);

								for (int j = 0; j < GC.objectRealList.Count; j++)
									if (GC.objectRealList[j].objectName == "Manhole" && Vector2.Distance(GC.objectRealList[j].tr.position, locationCandidate) < 14f)
										locationCandidate = Vector2.zero;

								if (locationCandidate != Vector2.zero)
								{
									if (GC.tileInfo.WaterNearby(locationCandidate))
										locationCandidate = Vector2.zero;

									if (GC.tileInfo.IceNearby(locationCandidate))
										locationCandidate = Vector2.zero;

									if (GC.tileInfo.BridgeNearby(locationCandidate))
										locationCandidate = Vector2.zero;
								}

								num34++;
							}
							while ((locationCandidate == Vector2.zero || Vector2.Distance(locationCandidate, GC.playerAgent.tr.position) < 5f) && num34 < 100);

							if (locationCandidate != Vector2.zero && Vector2.Distance(locationCandidate, GC.playerAgent.tr.position) >= 5f)
								GC.spawnerMain.spawnObjectReal(locationCandidate, null, "Manhole");

							if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
							{
								yield return null;
								chunkStartTime = Time.realtimeSinceStartup;
							}

							Random.InitState(__instance.randomSeedNum + i);
							num2 = i;
						}

						int numObjects = (int)((float)Random.Range(2, 4) * __instance.levelSizeModifier);
						List<Manhole> manholeList = new List<Manhole>();

						for (int num36 = 0; num36 < GC.objectRealList.Count; num36++)
							if (GC.objectRealList[num36].objectName == "Manhole")
								manholeList.Add((Manhole)GC.objectRealList[num36]);

						if (manholeList.Count > 0)
							for (int i = 0; i < numObjects; i = num2 + 1)
							{
								int num37 = 0;
								Manhole manhole;
								bool flag13;

								do
								{
									Random.InitState(__instance.randomSeedNum + i + ++randomCount);
									manhole = manholeList[Random.Range(0, manholeList.Count)];
									flag13 = true;

									for (int num38 = 0; num38 < GC.agentList.Count; num38++)
										if (GC.agentList[num38].oma.hidden && Vector2.Distance(manhole.tr.position, GC.agentList[num38].tr.position) < 10f)
										{
											num37++;
											flag13 = false;
										}

									num37++;
								}
								while (num37 < 50 && !flag13);

								if (flag13)
								{
									string text3 = GC.Choose<string>("Thief", "Thief", "Thief", "Cannibal");

									if ((!(text3 == "Thief") || !GC.challenges.Contains("ThiefNoSteal")) && 
										(!(text3 == "Cannibal") || !GC.challenges.Contains("CannibalsDontAttack")))
									{
										Agent agent2 = GC.spawnerMain.SpawnAgent(manhole.tr.position, manhole, text3);
										agent2.SetDefaultGoal("Idle");
										agent2.statusEffects.BecomeHidden(manhole);
										agent2.oma.mustBeGuilty = true;
									}
								}

								if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
								{
									yield return null;
									chunkStartTime = Time.realtimeSinceStartup;
								}

								Random.InitState(__instance.randomSeedNum + i);
								num2 = i;
							}

						manholeList = null;
					} // Vanilla
					#endregion
					#region Fire Hydrants
					bool hasFireHydrants = false;

					if (GC.levelTheme != 2 || 
						(GC.challenges.Contains("MixedUpLevels") && GC.percentChance(33)) ||
						(GC.customLevel && __instance.customLevel.levelFeatures.Contains("FireHydrant")))
						hasFireHydrants = true;

					if (GC.challenges.Contains(cChallenge.AnCapistan))
						hasFireHydrants = false;

					if (hasFireHydrants)
					{
						Debug.Log("Loading Fire Hydrants");
						int numObjects = (int)((float)Random.Range(6, 10) * __instance.levelSizeModifier);
						int i2;

						for (int i = 0; i < numObjects; i = i2 + 1)
						{
							Vector2 vector15 = Vector2.zero;
							int num39 = 0;

							do
							{
								vector15 = GC.tileInfo.FindRandLocationGeneral(2f);

								for (int num40 = 0; num40 < GC.objectRealList.Count; num40++)
									if (GC.objectRealList[num40].objectName == "FireHydrant" && Vector2.Distance(GC.objectRealList[num40].tr.position, vector15) < 14f)
										vector15 = Vector2.zero;

								if (vector15 != Vector2.zero)
								{
									if (GC.tileInfo.WaterNearby(vector15))
										vector15 = Vector2.zero;

									if (GC.tileInfo.IceNearby(vector15))
										vector15 = Vector2.zero;

									if (GC.tileInfo.BridgeNearby(vector15))
										vector15 = Vector2.zero;
								}

								num39++;
							}
							while ((vector15 == Vector2.zero || Vector2.Distance(vector15, GC.playerAgent.tr.position) < 5f) && num39 < 100);

							if (vector15 != Vector2.zero)
								GC.spawnerMain.spawnObjectReal(vector15, null, "FireHydrant");

							if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
							{
								yield return null;
								chunkStartTime = Time.realtimeSinceStartup;
							}

							Random.InitState(__instance.randomSeedNum + i);
							i2 = i;
						}
					}
					#endregion
					#region Exploding & Slime Barrels
					if (GC.levelTheme == 1 ||
						(GC.customLevel && __instance.customLevel.levelFeatures.Contains("ExplodingSlimeBarrel")) ||
						GC.challenges.Contains(cChallenge.ThePollutionSolution) ||
						(GC.challenges.Contains("MixedUpLevels") && GC.percentChance(33)))
					{
						Debug.Log("Loading Exploding and Slime Barrels");
						int numObjects = (int)((float)Random.Range(11, 16) * __instance.levelSizeModifier);
						int i2;

						for (int i = 0; i < numObjects; i = i2 + 1)
						{
							Vector2 vector16 = Vector2.zero;
							int num41 = 0;

							do
							{
								vector16 = GC.tileInfo.FindRandLocationGeneral(2f);
								num41++;
							}
							while ((vector16 == Vector2.zero || Vector2.Distance(vector16, GC.playerAgent.tr.position) < 5f) && num41 < 100);

							string objectType = GC.Choose<string>("ExplodingBarrel", "SlimeBarrel", new string[0]);

							if (vector16 != Vector2.zero)
								GC.spawnerMain.spawnObjectReal(vector16, null, objectType);

							if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
							{
								yield return null;
								chunkStartTime = Time.realtimeSinceStartup;
							}

							Random.InitState(__instance.randomSeedNum + i);
							i2 = i;
						}
					}
					#endregion
					#region Flaming Barrels
					bool hasFlamingBarrels = false;

					if (GC.levelTheme < 3 || 
						(GC.challenges.Contains("MixedUpLevels") && GC.percentChance(33)) || 
						(GC.customLevel && __instance.customLevel.levelFeatures.Contains("FlamingBarrel")) ||
						GC.challenges.Contains(cChallenge.AnCapistan))
						hasFlamingBarrels = true;

					if (GC.challenges.Contains(cChallenge.PoliceState) || GC.challenges.Contains(cChallenge.MACITS))
						hasFlamingBarrels = false;

					if (hasFlamingBarrels)
					{
						Debug.Log("Loading Flaming Barrels");
						int numObjects = (int)((float)Random.Range(6, 10) * __instance.levelSizeModifier);
						int i2;

						for (int i = 0; i < numObjects; i = i2 + 1)
						{
							Vector2 vector17 = Vector2.zero;
							int num42 = 0;

							do
							{
								vector17 = GC.tileInfo.FindRandLocationGeneral(2f);

								if (vector17 != Vector2.zero)
								{
									if (GC.tileInfo.WaterNearby(vector17))
										vector17 = Vector2.zero;

									if (GC.tileInfo.IceNearby(vector17))
										vector17 = Vector2.zero;

									if (GC.tileInfo.BridgeNearby(vector17))
										vector17 = Vector2.zero;
								}

								num42++;
							}
							while ((vector17 == Vector2.zero || Vector2.Distance(vector17, GC.playerAgent.tr.position) < 5f) && num42 < 100);

							if (vector17 != Vector2.zero)
								GC.spawnerMain.spawnObjectReal(vector17, null, vObject.FlamingBarrel);

							if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
							{
								yield return null;
								chunkStartTime = Time.realtimeSinceStartup;
							}

							Random.InitState(__instance.randomSeedNum + i);
							i2 = i;
						}
					}
					#endregion
					#region Flame Grates
					//if (GC.challenges.Contains(cChallenge.TransitExperiment))
					//	__instance.hasFlameGrates = false;

					if (__instance.hasFlameGrates)
					{
						Debug.Log("Loading Flame Grates");
						int numObjects = (int)((float)Random.Range(6, 10) * __instance.levelSizeModifier);
						int num2;

						for (int bigTries = 0; bigTries < numObjects; bigTries = num2 + 1)
						{
							Vector2 vector18 = Vector2.zero;
							int num43 = 0;
							bool flag17;

							do
							{
								vector18 = GC.tileInfo.FindRandLocationGeneral(1f);
								num43++;
								flag17 = false;

								if (vector18 != Vector2.zero && (GC.tileInfo.GetTileData(new Vector3(vector18.x, vector18.y, 0f)).spillOil ||
									GC.tileInfo.GetTileData(new Vector3(vector18.x, vector18.y + 0.64f, 0f)).spillOil ||
									GC.tileInfo.GetTileData(new Vector3(vector18.x, vector18.y - 0.64f, 0f)).spillOil ||
									GC.tileInfo.GetTileData(new Vector3(vector18.x + 0.64f, vector18.y, 0f)).spillOil ||
									GC.tileInfo.GetTileData(new Vector3(vector18.x - 0.64f, vector18.y, 0f)).spillOil ||
									GC.tileInfo.GetTileData(new Vector3(vector18.x + 0.64f, vector18.y + 0.64f, 0f)).spillOil ||
									GC.tileInfo.GetTileData(new Vector3(vector18.x + 0.64f, vector18.y - 0.64f, 0f)).spillOil ||
									GC.tileInfo.GetTileData(new Vector3(vector18.x - 0.64f, vector18.y + 0.64f, 0f)).spillOil ||
									GC.tileInfo.GetTileData(new Vector3(vector18.x - 0.64f, vector18.y - 0.64f, 0f)).spillOil))
									flag17 = true;

								GC.tileInfo.GetTileData(vector18);
							}
							while ((vector18 == Vector2.zero || Vector2.Distance(vector18, GC.playerAgent.tr.position) < 7f || flag17) && num43 < 100);

							num43 = 0;

							if (vector18 == Vector2.zero)
							{
								Random.InitState(__instance.randomSeedNum + bigTries + ++randomCount);

								do
								{
									vector18 = GC.tileInfo.FindRandLocationGeneral();
									num43++;
								}
								while ((vector18 == Vector2.zero || Vector2.Distance(vector18, GC.playerAgent.tr.position) < 5f) && num43 < 50);
							}

							if (vector18 != Vector2.zero)
								GC.spawnerMain.spawnObjectReal(vector18, null, "FlameGrate");

							if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
							{
								yield return null;
								chunkStartTime = Time.realtimeSinceStartup;
							}

							Random.InitState(__instance.randomSeedNum + bigTries);
							num2 = bigTries;
						}
					}
					#endregion
					#region Barbecues
					bool hasBarbecues = false;

					if (GC.levelTheme == 2 ||
						(GC.challenges.Contains("MixedUpLevels") && GC.percentChance(33)) ||
						(GC.customLevel && __instance.customLevel.levelFeatures.Contains("Barbecue")))
						hasBarbecues = true;

					if (hasBarbecues)
					{
						Debug.Log("Loading Barbecues");
						int numObjects = (int)((float)Random.Range(3, 5) * __instance.levelSizeModifier);
						int num2;

						for (int bigTries = 0; bigTries < numObjects; bigTries = num2 + 1)
						{
							Vector2 vector19 = Vector2.zero;
							int num44 = 0;

							do
							{
								vector19 = GC.tileInfo.FindRandLocationGeneral(1f);

								for (int num45 = 0; num45 < GC.objectRealList.Count; num45++)
									if (GC.objectRealList[num45].objectName == "Barbecue" && Vector2.Distance(GC.objectRealList[num45].tr.position, vector19) < 14f)
										vector19 = Vector2.zero;

								num44++;
							}
							while ((vector19 == Vector2.zero || Vector2.Distance(vector19, GC.playerAgent.tr.position) < 5f) && num44 < 100);

							if (vector19 != Vector2.zero)
								GC.spawnerMain.spawnObjectReal(vector19, null, "Barbecue");

							if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
							{
								yield return null;
								chunkStartTime = Time.realtimeSinceStartup;
							}

							Random.InitState(__instance.randomSeedNum + bigTries);
							num2 = bigTries;
						}
					}
					#endregion
					#region Mod - BroughtBackFountain / Fountains
					if (__instance.customLevel.levelFeatures.Contains(cLevelFeature.Fountains) || GC.challenges.Contains(cChallenge.BroughtBackFountain))
					{
						Debug.Log("Loading Fountains");
						int numObjects = Mathf.Clamp(3 * LevelSizeRatio(), 1, 5);
						int i2;

						for (int i = 0; i < numObjects; i = i2 + 1)
						{
							Vector2 vector20 = Vector2.zero;
							int num46 = 0;

							do
							{
								vector20 = GC.tileInfo.FindRandLocationGeneral(2f);

								for (int num47 = 0; num47 < GC.objectRealList.Count; num47++)
									if (GC.objectRealList[num47].objectName == "Fountain" && Vector2.Distance(GC.objectRealList[num47].tr.position, vector20) < (14f * LevelSizeRatio()))
										vector20 = Vector2.zero;

								num46++;
							}
							while ((vector20 == Vector2.zero || Vector2.Distance(vector20, GC.playerAgent.tr.position) < 5f) && num46 < 100);

							if (vector20 != Vector2.zero)
								GC.spawnerMain.spawnObjectReal(vector20, null, "Fountain");

							if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
							{
								yield return null;
								chunkStartTime = Time.realtimeSinceStartup;
							}

							Random.InitState(__instance.randomSeedNum + i);
							i2 = i;
						}
					}
					#endregion
					#region Mod - DiscoCityDanceoff / Jukeboxes & Speakers
					if (GC.challenges.Contains(cChallenge.DiscoCityDanceoff))
					{
						Debug.Log("Loading Disco Shit");
						int bigTries = (int)((float)Random.Range(6, 12) * __instance.levelSizeModifier);
						List<int> spawnedInChunks = new List<int>();
						int i2;

						for (int i = 0; i < bigTries; i = i2 + 1)
						{
							Vector2 vector7 = Vector2.zero;
							int num22 = 0;

							do
							{
								vector7 = GC.tileInfo.FindRandLocationNearWall(0.64f);

								if (vector7 != Vector2.zero)
								{
									TileData tileData4 = GC.tileInfo.GetTileData(vector7);

									if (GC.tileInfo.GetTileData(new Vector2(vector7.x, vector7.y + 0.64f)).owner == 0 &&
										GC.tileInfo.GetTileData(new Vector2(vector7.x + 0.64f, vector7.y)).owner == 0 &&
										GC.tileInfo.GetTileData(new Vector2(vector7.x, vector7.y - 0.64f)).owner == 0 &&
										GC.tileInfo.GetTileData(new Vector2(vector7.x - 0.64f, vector7.y)).owner == 0)
										vector7 = Vector2.zero;

									if (!GC.tileInfo.IsOverlapping(new Vector2(vector7.x, vector7.y + 0.64f), "Wall") &&
										!GC.tileInfo.IsOverlapping(new Vector2(vector7.x, vector7.y - 0.64f), "Wall") &&
										!GC.tileInfo.IsOverlapping(new Vector2(vector7.x + 0.64f, vector7.y), "Wall") &&
										!GC.tileInfo.IsOverlapping(new Vector2(vector7.x - 0.64f, vector7.y), "Wall"))
										vector7 = Vector2.zero;

									if (GC.tileInfo.IsOverlapping(vector7, "ObjectRealSprite", 0.64f))
										vector7 = Vector2.zero;

									if (spawnedInChunks.Contains(tileData4.chunkID))
										vector7 = Vector2.zero;

									if (GC.tileInfo.DestroyIfBetweenWalls(vector7))
										vector7 = Vector2.zero;
								}

								num22++;
							}
							while ((vector7 == Vector2.zero || Vector2.Distance(vector7, GC.playerAgent.tr.position) < 5f) && num22 < 100);

							if (vector7 != Vector2.zero)
							{
								GC.spawnerMain.spawnObjectReal(vector7, null, vObject.Speaker).ShiftTowardWalls();

								TileData tileData5 = GC.tileInfo.GetTileData(vector7);
								spawnedInChunks.Add(tileData5.chunkID);
								Random.InitState(__instance.randomSeedNum + i + ++randomCount);

								if (i < bigTries - 1 && GC.percentChance(25))
								{
									string a2 = "";
									Vector2 zero4 = Vector2.zero;
									Vector2 zero5 = Vector2.zero;

									if (GC.tileInfo.GetTileData(new Vector2(vector7.x, vector7.y + 0.64f)).wallMaterial != wallMaterialType.None)
									{
										zero4 = new Vector2(vector7.x + 1.28f, vector7.y);
										zero5 = new Vector2(vector7.x - 1.28f, vector7.y);
										a2 = "N";
									}
									else if (GC.tileInfo.GetTileData(new Vector2(vector7.x, vector7.y - 0.64f)).wallMaterial != wallMaterialType.None)
									{
										zero4 = new Vector2(vector7.x + 1.28f, vector7.y);
										zero5 = new Vector2(vector7.x - 1.28f, vector7.y);
										a2 = "S";
									}
									else if (GC.tileInfo.GetTileData(new Vector2(vector7.x + 0.64f, vector7.y)).wallMaterial != wallMaterialType.None)
									{
										zero4 = new Vector2(vector7.x, vector7.y + 1.28f);
										zero5 = new Vector2(vector7.x, vector7.y - 1.28f);
										a2 = "E";
									}
									else if (GC.tileInfo.GetTileData(new Vector2(vector7.x - 0.64f, vector7.y)).wallMaterial != wallMaterialType.None)
									{
										zero4 = new Vector2(vector7.x, vector7.y + 1.28f);
										zero5 = new Vector2(vector7.x, vector7.y - 1.28f);
										a2 = "W";
									}

									GC.tileInfo.GetTileData(zero4);
									bool flag9 = true;

									if ((GC.tileInfo.GetTileData(new Vector2(zero4.x, zero4.y + 0.64f)).wallMaterial == wallMaterialType.None && a2 == "N") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x, zero4.y - 0.64f)).wallMaterial != wallMaterialType.None && a2 == "N") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x + 0.64f, zero4.y - 0.64f)).wallMaterial != wallMaterialType.None && a2 == "N") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x - 0.64f, zero4.y - 0.64f)).wallMaterial != wallMaterialType.None && a2 == "N") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x + 0.64f, zero4.y)).wallMaterial == wallMaterialType.None && a2 == "E") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x - 0.64f, zero4.y)).wallMaterial != wallMaterialType.None && a2 == "E") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x - 0.64f, zero4.y + 0.64f)).wallMaterial != wallMaterialType.None && a2 == "E") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x - 0.64f, zero4.y - 0.64f)).wallMaterial != wallMaterialType.None && a2 == "E") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x, zero4.y - 0.64f)).wallMaterial == wallMaterialType.None && a2 == "S") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x, zero4.y + 0.64f)).wallMaterial != wallMaterialType.None && a2 == "S") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x + 0.64f, zero4.y + 0.64f)).wallMaterial != wallMaterialType.None && a2 == "S") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x - 0.64f, zero4.y + 0.64f)).wallMaterial != wallMaterialType.None && a2 == "S") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x - 0.64f, zero4.y)).wallMaterial == wallMaterialType.None && a2 == "W") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x + 0.64f, zero4.y)).wallMaterial != wallMaterialType.None && a2 == "W") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x + 0.64f, zero4.y + 0.64f)).wallMaterial != wallMaterialType.None && a2 == "W") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x + 0.64f, zero4.y - 0.64f)).wallMaterial != wallMaterialType.None && a2 == "W"))
										flag9 = false;

									if (GC.tileInfo.IsOverlapping(zero4, "Anything"))
										flag9 = false;

									if (GC.tileInfo.IsOverlapping(zero4, "ObjectRealSprite", 0.64f))
										flag9 = false;

									if (flag9 && zero4 != Vector2.zero)
									{
										GC.spawnerMain.spawnObjectReal(zero4, null, vObject.Speaker).ShiftTowardWalls();
										i2 = i;
										i = i2 + 1;
									}
									else
									{
										GC.tileInfo.GetTileData(zero5);
										flag9 = true;

										if ((GC.tileInfo.GetTileData(new Vector2(zero5.x, zero5.y + 0.64f)).wallMaterial == wallMaterialType.None && a2 == "N") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x, zero5.y - 0.64f)).wallMaterial != wallMaterialType.None && a2 == "N") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x + 0.64f, zero5.y - 0.64f)).wallMaterial != wallMaterialType.None && a2 == "N") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x - 0.64f, zero5.y - 0.64f)).wallMaterial != wallMaterialType.None && a2 == "N") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x + 0.64f, zero5.y)).wallMaterial == wallMaterialType.None && a2 == "E") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x - 0.64f, zero5.y)).wallMaterial != wallMaterialType.None && a2 == "E") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x - 0.64f, zero5.y + 0.64f)).wallMaterial != wallMaterialType.None && a2 == "E") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x - 0.64f, zero5.y - 0.64f)).wallMaterial != wallMaterialType.None && a2 == "E") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x, zero5.y - 0.64f)).wallMaterial == wallMaterialType.None && a2 == "S") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x, zero5.y + 0.64f)).wallMaterial != wallMaterialType.None && a2 == "S") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x + 0.64f, zero5.y + 0.64f)).wallMaterial != wallMaterialType.None && a2 == "S") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x - 0.64f, zero5.y + 0.64f)).wallMaterial != wallMaterialType.None && a2 == "S") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x - 0.64f, zero5.y)).wallMaterial == wallMaterialType.None && a2 == "W") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x + 0.64f, zero5.y)).wallMaterial != wallMaterialType.None && a2 == "W") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x + 0.64f, zero5.y + 0.64f)).wallMaterial != wallMaterialType.None && a2 == "W") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x + 0.64f, zero5.y - 0.64f)).wallMaterial != wallMaterialType.None && a2 == "W"))
											flag9 = false;

										if (GC.tileInfo.IsOverlapping(zero5, "Anything"))
											flag9 = false;

										if (GC.tileInfo.IsOverlapping(zero5, "ObjectRealSprite", 0.64f))
											flag9 = false;

										if (flag9 && zero5 != Vector2.zero)
										{
											GC.spawnerMain.spawnObjectReal(zero5, null, vObject.Jukebox).ShiftTowardWalls();
											i2 = i;
											i = i2 + 1;
										}
									}
								}
							}

							if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
							{
								yield return null;
								chunkStartTime = Time.realtimeSinceStartup;
							}

							Random.InitState(__instance.randomSeedNum + i);
							i2 = i;
						}

						spawnedInChunks = null;
					}
					#endregion
					#region Mod - DiscoCityDanceoff / Turntables
					if (GC.challenges.Contains(cChallenge.DiscoCityDanceoff))
					{
						Debug.Log("Loading Disco Turntables");
						int bigTries = 6;

						int i2;

						for (int i = 0; i < bigTries; i = i2 + 1)
						{
							Vector2 vector4 = Vector2.zero;
							int num15 = 0;

							do
							{
								vector4 = GC.tileInfo.FindRandLocationGeneral(2f);

								for (int num16 = 0; num16 < GC.objectRealList.Count; num16++)
									if (GC.objectRealList[num16].objectName == vObject.Turntables && Vector2.Distance(GC.objectRealList[num16].tr.position, vector4) < 28f)
										vector4 = Vector2.zero;

								if (vector4 != Vector2.zero)
								{
									if (GC.tileInfo.WaterNearby(vector4))
										vector4 = Vector2.zero;

									if (GC.tileInfo.IceNearby(vector4))
										vector4 = Vector2.zero;

									if (GC.tileInfo.BridgeNearby(vector4))
										vector4 = Vector2.zero;
								}

								num15++;
							}
							while ((vector4 == Vector2.zero || Vector2.Distance(vector4, GC.playerAgent.tr.position) < 5f || Vector2.Distance(vector4, GC.elevatorDown.tr.position) < 20f) && num15 < 100);

							if (vector4 != Vector2.zero)
								GC.spawnerMain.spawnObjectReal(vector4, null, vObject.Turntables);

							if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
							{
								yield return null;
								chunkStartTime = Time.realtimeSinceStartup;
							}

							Random.InitState(__instance.randomSeedNum + i);
							i2 = i;
						}
					}
					#endregion
					#region Trees
					if (GC.levelTheme == 2 || 
						(GC.challenges.Contains("MixedUpLevels") && GC.percentChance(33)) ||
						(GC.customLevel && __instance.customLevel.levelFeatures.Contains("Tree")) ||
						(GC.challenges.Contains(cChallenge.ArcologyEcology)))
					{
						Debug.Log("Loading Trees");

						int numObjects = (int)((float)Random.Range(30, 40) * __instance.levelSizeModifier);
						int num2;

						for (int bigTries = 0; bigTries < numObjects; bigTries = num2 + 1)
						{
							Vector2 vector21 = Vector2.zero;
							int num48 = 0;

							do
							{
								vector21 = GC.tileInfo.FindRandLocationGeneral(0.64f);
								num48++;
							}
							while ((vector21 == Vector2.zero || Vector2.Distance(vector21, GC.playerAgent.tr.position) < 5f) && num48 < 100);

							if (vector21 != Vector2.zero && (GC.tileInfo.GetTileData(new Vector2(vector21.x, vector21.y + 0.64f)).lake ||
								GC.tileInfo.GetTileData(new Vector2(vector21.x + 0.64f, vector21.y + 0.64f)).lake ||
								GC.tileInfo.GetTileData(new Vector2(vector21.x + 0.64f, vector21.y + 0.64f)).lake ||
								GC.tileInfo.GetTileData(new Vector2(vector21.x + 0.64f, vector21.y)).lake ||
								GC.tileInfo.GetTileData(new Vector2(vector21.x, vector21.y - 0.64f)).lake ||
								GC.tileInfo.GetTileData(new Vector2(vector21.x - 0.64f, vector21.y - 0.64f)).lake ||
								GC.tileInfo.GetTileData(new Vector2(vector21.x - 0.64f, vector21.y - 0.64f)).lake ||
								GC.tileInfo.GetTileData(new Vector2(vector21.x - 0.64f, vector21.y)).lake))
								vector21 = Vector2.zero;

							if (vector21 != Vector2.zero)
								GC.spawnerMain.spawnObjectReal(vector21, null, "Tree");

							if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
							{
								yield return null;
								chunkStartTime = Time.realtimeSinceStartup;
							}

							Random.InitState(__instance.randomSeedNum + bigTries);
							num2 = bigTries;
						}
					}
					#endregion
					#region Boulders
					if (GC.levelTheme == 2 || 
						(GC.challenges.Contains("MixedUpLevels") && GC.percentChance(33)) ||
						GC.customLevel && __instance.customLevel.levelFeatures.Contains("Boulder") ||
						GC.challenges.Contains(cChallenge.ArcologyEcology) || GC.challenges.Contains(cChallenge.SpelunkyDory))
					{
						Debug.Log("Loading Boulders");
						int numObjects = (int)((float)Random.Range(10, 20) * __instance.levelSizeModifier);
						int num2;

						for (int bigTries = 0; bigTries < numObjects; bigTries = num2 + 1)
						{
							Vector2 vector22 = Vector2.zero;
							int num49 = 0;

							do
							{
								vector22 = GC.tileInfo.FindRandLocationGeneral(1.28f);
								num49++;
							}
							while ((vector22 == Vector2.zero || Vector2.Distance(vector22, GC.playerAgent.tr.position) < 5f) && num49 < 100);

							if (vector22 != Vector2.zero && (GC.tileInfo.GetTileData(new Vector2(vector22.x, vector22.y + 0.64f)).lake ||
								GC.tileInfo.GetTileData(new Vector2(vector22.x + 0.64f, vector22.y + 0.64f)).lake ||
								GC.tileInfo.GetTileData(new Vector2(vector22.x + 0.64f, vector22.y + 0.64f)).lake ||
								GC.tileInfo.GetTileData(new Vector2(vector22.x + 0.64f, vector22.y)).lake ||
								GC.tileInfo.GetTileData(new Vector2(vector22.x, vector22.y - 0.64f)).lake ||
								GC.tileInfo.GetTileData(new Vector2(vector22.x - 0.64f, vector22.y - 0.64f)).lake ||
								GC.tileInfo.GetTileData(new Vector2(vector22.x - 0.64f, vector22.y - 0.64f)).lake ||
								GC.tileInfo.GetTileData(new Vector2(vector22.x - 0.64f, vector22.y)).lake))
								vector22 = Vector2.zero;

							if (vector22 != Vector2.zero)
								GC.spawnerMain.spawnObjectReal(vector22, null, "Boulder");

							if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
							{
								yield return null;
								chunkStartTime = Time.realtimeSinceStartup;
							}

							Random.InitState(__instance.randomSeedNum + bigTries);
							num2 = bigTries;
						}

						numObjects = (int)((float)Random.Range(10, 20) * __instance.levelSizeModifier);

						for (int bigTries = 0; bigTries < numObjects; bigTries = num2 + 1)
						{
							Vector2 vector23 = Vector2.zero;
							int num50 = 0;

							do
							{
								vector23 = GC.tileInfo.FindRandLocationGeneral(0.64f);
								num50++;
							}
							while ((vector23 == Vector2.zero || Vector2.Distance(vector23, GC.playerAgent.tr.position) < 5f) && num50 < 100);

							if (vector23 != Vector2.zero)
								GC.spawnerMain.spawnObjectReal(vector23, null, "BoulderSmall");

							if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
							{
								yield return null;
								chunkStartTime = Time.realtimeSinceStartup;
							}

							Random.InitState(__instance.randomSeedNum + bigTries);
							num2 = bigTries;
						}
					}
					#endregion
					#region Bushes
					if (GC.levelTheme == 2 || 
						(GC.challenges.Contains("MixedUpLevels") && GC.percentChance(33)) ||
						(GC.customLevel && __instance.customLevel.levelFeatures.Contains("Bush")) || 
						GC.challenges.Contains(cChallenge.ArcologyEcology))
					{
						Debug.Log("Loading Bushes");
						Random.InitState(__instance.randomSeedNum);
						int numObjects = (int)((float)Random.Range(20, 30) * __instance.levelSizeModifier);
						int num2;

						for (int i = 0; i < numObjects; i = num2 + 1)
						{
							Vector2 vector24 = Vector2.zero;
							int num51 = 0;

							do
							{
								vector24 = GC.tileInfo.FindRandLocationGeneral(0.64f);

								for (int num52 = 0; num52 < GC.objectRealList.Count; num52++)
									if (GC.objectRealList[num52].objectName == "VendorCart" && Vector2.Distance(GC.objectRealList[num52].tr.position, vector24) < 4f)
										vector24 = Vector2.zero;

								num51++;
							}
							while ((vector24 == Vector2.zero || Vector2.Distance(vector24, GC.playerAgent.tr.position) < 5f) && num51 < 100);

							num51 = 0;

							if (vector24 == Vector2.zero)
							{
								Random.InitState(__instance.randomSeedNum + i + ++randomCount);

								do
								{
									vector24 = GC.tileInfo.FindRandLocationGeneral();
									num51++;
								}
								while ((vector24 == Vector2.zero || Vector2.Distance(vector24, GC.playerAgent.tr.position) < 5f) && num51 < 50);
							}

							if (vector24 != Vector2.zero)
								GC.tileInfo.CreateBushCluster(vector24, 0);

							if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
							{
								yield return null;
								chunkStartTime = Time.realtimeSinceStartup;
							}

							Random.InitState(__instance.randomSeedNum + i);
							num2 = i;
						}

						int bigTries = (int)((float)Random.Range(4, 6) * __instance.levelSizeModifier);
						List<Bush> bushList = new List<Bush>();

						for (int num53 = 0; num53 < GC.objectRealList.Count; num53++)
							if (GC.objectRealList[num53].objectName == "Bush")
								bushList.Add((Bush)GC.objectRealList[num53]);

						for (int i = 0; i < bigTries; i = num2 + 1)
						{
							int num54 = 0;
							Random.InitState(__instance.randomSeedNum + i + ++randomCount);
							Bush bush;
							bool flag22;

							do
							{
								bush = bushList[Random.Range(0, bushList.Count)];
								flag22 = true;

								for (int num55 = 0; num55 < GC.agentList.Count; num55++)
									if (GC.agentList[num55].oma.hidden && Vector2.Distance(bush.tr.position, GC.agentList[num55].tr.position) < 10f)
									{
										num54++;
										flag22 = false;
									}

								if (Vector2.Distance(bush.tr.position, GC.playerAgent.tr.position) < 10f)
									flag22 = false;

								num54++;
							}
							while (num54 < 50 && !flag22);

							if (flag22 && !GC.challenges.Contains("CannibalsDontAttack") && GC.levelFeeling != "HarmAtIntervals")
							{
								Agent agent3 = GC.spawnerMain.SpawnAgent(bush.tr.position, bush, "Cannibal");
								agent3.SetDefaultGoal("Idle");
								agent3.statusEffects.BecomeHidden(null);
								agent3.hiddenInBush = bush;
								bush.TurnOnShake();
								agent3.noEnforcerAlert = true;
								agent3.oma.mustBeGuilty = true;
							}

							if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
							{
								yield return null;
								chunkStartTime = Time.realtimeSinceStartup;
							}

							Random.InitState(__instance.randomSeedNum + i);
							num2 = i;
						}

						bushList = null;
					}
					#endregion
					#region Land Mines
					if (GC.levelTheme == 2 || 
						(GC.challenges.Contains("MixedUpLevels") && GC.percentChance(33)) ||
						(GC.customLevel && __instance.customLevel.levelFeatures.Contains("Mine")))
					{
						Debug.Log("Loading Mines");

						if (GC.levelTheme == 2)
						{
							int num2;

							for (int bigTries = 0; bigTries < __instance.levelChunks.Count; bigTries = num2 + 1)
							{
								if (__instance.levelChunks[bigTries].description == "MilitaryOutpost")
								{
									int numObjects = Random.Range(5, 10);

									for (int i = 0; i < numObjects; i = num2 + 1)
									{
										Vector2 vector25 = Vector2.zero;
										int num56 = 0;

										do
										{
											vector25 = GC.tileInfo.FindRandLocationGeneral(0.64f);
											num56++;

											if
											(
												(
													(
														(
															vector25.x <= __instance.levelChunks[bigTries].chunkEdgeE ||
															vector25.x >= __instance.levelChunks[bigTries].chunkEdgeE + __instance.chunkSize
														)
														&&
														(
															vector25.x >= __instance.levelChunks[bigTries].chunkEdgeW ||
															vector25.x <= __instance.levelChunks[bigTries].chunkEdgeW - __instance.chunkSize
														)
													) ||
													vector25.y <= __instance.levelChunks[bigTries].chunkEdgeS - __instance.chunkSize ||
													vector25.y >= __instance.levelChunks[bigTries].chunkEdgeN + __instance.chunkSize
												)
												&&
												(
													(
														(
															vector25.y <= __instance.levelChunks[bigTries].chunkEdgeN ||
															vector25.y >= __instance.levelChunks[bigTries].chunkEdgeN + __instance.chunkSize
														)
														&&
														(
															vector25.y >= __instance.levelChunks[bigTries].chunkEdgeS ||
															vector25.y <= __instance.levelChunks[bigTries].chunkEdgeS - __instance.chunkSize
														)
													)
													||
													vector25.x <= __instance.levelChunks[bigTries].chunkEdgeW - __instance.chunkSize ||
													vector25.x >= __instance.levelChunks[bigTries].chunkEdgeE + __instance.chunkSize
												)
											)
												vector25 = Vector2.zero;

											for (int num57 = 0; num57 < GC.objectRealList.Count; num57++)
												if (GC.objectRealList[num57].objectName == "Mine" && Vector2.Distance(GC.objectRealList[num57].tr.position, vector25) < 1.92f)
													vector25 = Vector2.zero;
										}
										while ((vector25 == Vector2.zero || Vector2.Distance(vector25, GC.playerAgent.tr.position) < 5f) && num56 < 100);

										if (vector25 != Vector2.zero)
											GC.spawnerMain.spawnObjectReal(vector25, null, "Mine");

										if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
										{
											yield return null;
											chunkStartTime = Time.realtimeSinceStartup;
										}

										Random.InitState(__instance.randomSeedNum + i);
										num2 = i;
									}
								}
								num2 = bigTries;
							}
						}
						else
						{
							int bigTries = (int)((float)Random.Range(5, 10) * __instance.levelSizeModifier);
							int num2;

							for (int numObjects = 0; numObjects < bigTries; numObjects = num2 + 1)
							{
								Vector2 vector26 = Vector2.zero;
								int num58 = 0;

								do
								{
									vector26 = GC.tileInfo.FindRandLocationGeneral(0.64f);

									for (int num59 = 0; num59 < GC.objectRealList.Count; num59++)
										if (GC.objectRealList[num59].objectName == "Mine" && Vector2.Distance(GC.objectRealList[num59].tr.position, vector26) < 1.92f)
											vector26 = Vector2.zero;

									num58++;
								}
								while ((vector26 == Vector2.zero || Vector2.Distance(vector26, GC.playerAgent.tr.position) < 5f) && num58 < 100);

								if (vector26 != Vector2.zero)
									GC.spawnerMain.spawnObjectReal(vector26, null, "Mine");

								if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
								{
									yield return null;
									chunkStartTime = Time.realtimeSinceStartup;
								}

								Random.InitState(__instance.randomSeedNum + numObjects);
								num2 = numObjects;
							}
						}
					}
					#endregion
					#region Bear Traps
					if (GC.levelTheme == 2 || 
						(GC.challenges.Contains("MixedUpLevels") && GC.percentChance(33)) ||
						(GC.customLevel && __instance.customLevel.levelFeatures.Contains("BearTrap")))
					{
						Debug.Log("Loading Bear Traps");
						int bigTries = (int)((float)Random.Range(10, 20) * __instance.levelSizeModifier);
						int num2;

						for (int numObjects = 0; numObjects < bigTries; numObjects = num2 + 1)
						{
							Vector2 vector27 = Vector2.zero;
							int num60 = 0;

							do
							{
								vector27 = GC.tileInfo.FindRandLocationGeneral(0.64f);

								for (int num61 = 0; num61 < GC.itemList.Count; num61++)
									if (GC.itemList[num61].objectName == "BearTrapPark" && Vector2.Distance(GC.itemList[num61].tr.position, vector27) < 4f)
										vector27 = Vector2.zero;

								num60++;
							}
							while ((vector27 == Vector2.zero || Vector2.Distance(vector27, GC.playerAgent.tr.position) < 5f) && num60 < 100);

							if (vector27 != Vector2.zero)
							{
								Item item = GC.spawnerMain.SpawnItem(vector27, "BearTrapPark");
								item.interactable = false;
								item.SetCantPickUp(true);
								item.dangerous = true;
								item.danger = GC.spawnerMain.SpawnDanger(item, "Major", "Normal");
								item.danger.noFlee = true;
								item.dangerPenaltyAmount = 100000;
								item.danger.avoidInCombat = false;
								GC.objectModifyEnvironmentList.Add(item);
								GC.tileInfo.GetTileData(vector27).dangerousToWalk = true;
							}

							if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
							{
								yield return null;
								chunkStartTime = Time.realtimeSinceStartup;
							}

							Random.InitState(__instance.randomSeedNum + numObjects);
							num2 = numObjects;
						}
					}
					#endregion
					#region Lamps
					bool hasLamps = true;

					if (GC.customLevel)
						hasLamps = __instance.customLevel.levelFeatures.Contains("Lamp");

					if (GC.challenges.Contains(cChallenge.AnCapistan))
						hasLamps = false;

					if (hasLamps)
					{
						Debug.Log("Loading Lamps");
						int bigTries = (int)((float)Random.Range(18, 22) * __instance.levelSizeModifier);
						List<int> spawnedInChunks = new List<int>();
						int num2;

						for (int numObjects = 0; numObjects < bigTries; numObjects = num2 + 1)
						{
							Vector2 vector28 = Vector2.zero;
							int num62 = 0;

							do
							{
								vector28 = GC.tileInfo.FindRandLocationGeneral(2f);

								if (vector28 != Vector2.zero)
								{
									TileData tileData6 = GC.tileInfo.GetTileData(vector28);
									int num63 = 0;

									for (int num64 = 0; num64 < spawnedInChunks.Count; num64++)
										if (spawnedInChunks[num64] == tileData6.chunkID)
											num63++;

									if (num63 >= 3)
										vector28 = Vector2.zero;
								}

								if (vector28 != Vector2.zero)
								{
									if (GC.tileInfo.WaterNearby(vector28))
										vector28 = Vector2.zero;

									if (GC.tileInfo.IceNearby(vector28))
										vector28 = Vector2.zero;

									if (GC.tileInfo.BridgeNearby(vector28))
										vector28 = Vector2.zero;
								}

								num62++;
							}
							while ((vector28 == Vector2.zero || Vector2.Distance(vector28, GC.playerAgent.tr.position) < 5f) && num62 < 100);

							if (vector28 != Vector2.zero)
							{
								GC.spawnerMain.spawnObjectReal(vector28, null, "Lamp");
								TileData tileData7 = GC.tileInfo.GetTileData(vector28);
								spawnedInChunks.Add(tileData7.chunkID);
							}

							if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
							{
								yield return null;
								chunkStartTime = Time.realtimeSinceStartup;
							}

							Random.InitState(__instance.randomSeedNum + numObjects);
							num2 = numObjects;
						}

						spawnedInChunks = null;
					}
					#endregion
					#endregion
					#region Ambience
					Debug.Log("Loading Ambient Objects");

					for (int num65 = 0; num65 < __instance.levelChunks.Count; num65++)
					{
						Chunk chunk = __instance.levelChunks[num65];
						string ambience = "";
						string description = chunk.description;
						 
						switch(description)
						{
							case vChunkType.Casino:
								ambience = vAmbience.Casino; // This was left out of vanilla.
								break;
							case vChunkType.Cave:
								ambience = vAmbience.Cave;
								break;
							case vChunkType.CityPark:
								ambience = vAmbience.Park;
								break;
							case vChunkType.Bathhouse:
								ambience = vAmbience.BathHouse;
								break;
							case vChunkType.Graveyard:
								ambience = vAmbience.Graveyard;
								break;
							default:
								if (GC.challenges.Contains(cChallenge.ArcologyEcology) || GC.challenges.Contains(cChallenge.GreenLiving))
									ambience = vAmbience.Park;
								else if (GC.challenges.Contains(cChallenge.SpelunkyDory))
									ambience = vAmbience.Cave;
								else if (GC.challenges.Contains(cChallenge.GhostTown))
									ambience = vAmbience.Graveyard;
								break;
						}

						if (ambience != "")
						{
							if (chunk.shape == 6)
							{
								Vector2 v2 = new Vector2(
									(chunk.chunkLeash1Tr.x + chunk.chunkLeash2Tr.x + chunk.chunkLeash3Tr.x + chunk.chunkLeash4Tr.x) / 4f, 
									(chunk.chunkLeash1Tr.y + chunk.chunkLeash2Tr.y + chunk.chunkLeash3Tr.y + chunk.chunkLeash4Tr.y) / 4f);
								GC.spawnerMain.spawnObjectReal(v2, null, "AmbientObject").GetComponent<AmbientObject>().ambientAudioForObject = ambience + "_Huge";
							}
							else if (chunk.shape == 5)
								GC.spawnerMain.spawnObjectReal(chunk.chunkLeash1Tr, null, "AmbientObject").GetComponent<AmbientObject>().ambientAudioForObject = ambience + "_Long";
							else
								GC.spawnerMain.spawnObjectReal(chunk.chunkLeash1Tr, null, "AmbientObject").GetComponent<AmbientObject>().ambientAudioForObject = ambience;
						}
					}
					#endregion
					#region Mayor
					__instance.LevelContainsMayor();
					bool hasMayor = false;

					if (__instance.LevelContainsMayor())
						hasMayor = true;

					if (GC.customLevel)
						hasMayor = __instance.customLevel.levelFeatures.Contains("Mayor");

					if (hasMayor)
					{
						Debug.Log("Loading Mayor");
						bool findingFactoryLake = false;
						bool removeAgentsInBuilding = false;
						Agent mayorAgent = null;
						List<string> list4 = new List<string>();

						for (int num66 = 0; num66 < __instance.levelChunks.Count; num66++)
						{
							if (__instance.levelChunks[num66].description == "MayorHouse" && !list4.Contains("Sleeping"))
								list4.Add("Sleeping");

							if (__instance.levelChunks[num66].description == "DanceClub" && !list4.Contains("Dancing"))
								list4.Add("Dancing");

							if (__instance.levelChunks[num66].description == "Bar" && !list4.Contains("Drinking"))
								list4.Add("Drinking");

							if (__instance.levelChunks[num66].description == "MusicHall" && !list4.Contains("WatchingShow"))
								list4.Add("WatchingShow");

							if (__instance.levelChunks[num66].description == "MayorOffice" && !list4.Contains("WalkingInOffice"))
								list4.Add("WalkingInOffice");

							if (__instance.levelChunks[num66].description == "Bathhouse" && !list4.Contains("Swimming"))
								list4.Add("Swimming");
						}

						list4.Add("Wandering");

						for (int num67 = 0; num67 < list4.Count; num67++)
							Debug.Log("Potential Location: " + list4[num67]);

						string lakeType = list4[Random.Range(0, list4.Count)];
						Debug.Log("CHOSENLOCATION: " + lakeType);
						bool flag27 = false;

						if (lakeType == "Sleeping")
							for (int num68 = 0; num68 < GC.objectRealList.Count; num68++)
							{
								ObjectReal objectReal5 = GC.objectRealList[num68];

								if (objectReal5.objectName == "Bed" && objectReal5.startingChunkRealDescription == "MayorHouse")
								{
									Vector2 v3 = objectReal5.tr.position;
									string agentType2 = "Mayor";
									Agent agent4 = GC.spawnerMain.SpawnAgent(v3, null, agentType2);
									flag27 = true;
									mayorAgent = agent4;
									agent4.startingChunk = objectReal5.startingChunk;
									agent4.initialStartingChunk = agent4.startingChunk;
									agent4.startingChunkReal = objectReal5.startingChunkReal;
									agent4.startingChunkRealDescription = objectReal5.startingChunkRealDescription;
									agent4.SetDefaultGoal("Sleep");
									agent4.cantBump = true;
									agent4.ownerID = 1;
									findingFactoryLake = true;
									removeAgentsInBuilding = true;

									break;
								}
							}
						else if (lakeType == "Dancing")
						{
							List<EventTriggerFloor> list5 = new List<EventTriggerFloor>();

							for (int num69 = 0; num69 < GC.objectRealList.Count; num69++)
							{
								ObjectReal objectReal6 = GC.objectRealList[num69];

								if (objectReal6.objectName == "EventTriggerFloor")
								{
									EventTriggerFloor eventTriggerFloor = (EventTriggerFloor)objectReal6;

									if (eventTriggerFloor.triggerType == "MayorDancePosition")
									{
										list5.Add(eventTriggerFloor);
										Vector2 v4 = objectReal6.tr.position;
										string agentType3 = "Mayor";
										Agent agent5 = GC.spawnerMain.SpawnAgent(v4, null, agentType3);
										flag27 = true;
										mayorAgent = agent5;
										agent5.startingChunk = objectReal6.startingChunk;
										agent5.initialStartingChunk = agent5.startingChunk;
										agent5.startingChunkReal = objectReal6.startingChunkReal;
										agent5.startingChunkRealDescription = objectReal6.startingChunkRealDescription;
										agent5.SetDefaultGoal("Dance");
										agent5.cantBump = true;
										findingFactoryLake = true;

										break;
									}
								}
							}

							if (list5.Count > 0)
							{
								EventTriggerFloor eventTriggerFloor2 = list5[Random.Range(0, list5.Count)];
							}
						}
						else if (lakeType == "Drinking")
							for (int num70 = 0; num70 < GC.objectRealList.Count; num70++)
							{
								ObjectReal objectReal7 = GC.objectRealList[num70];

								if (objectReal7.objectName == "BarStool" && objectReal7.startingChunkRealDescription == "Bar")
								{
									Vector2 v5 = objectReal7.tr.position;
									string agentType4 = "Mayor";
									Agent agent6 = GC.spawnerMain.SpawnAgent(v5, null, agentType4);
									flag27 = true;
									mayorAgent = agent6;
									agent6.startingChunk = objectReal7.startingChunk;
									agent6.initialStartingChunk = agent6.startingChunk;
									agent6.startingChunkReal = objectReal7.startingChunkReal;
									agent6.startingChunkRealDescription = objectReal7.startingChunkRealDescription;
									agent6.cantBump = true;
									findingFactoryLake = true;

									break;
								}
							}
						else if (lakeType == "WatchingShow")
							for (int num71 = 0; num71 < GC.objectRealList.Count; num71++)
							{
								ObjectReal objectReal8 = GC.objectRealList[num71];

								if (objectReal8.objectName == "Chair2" && objectReal8.startingChunkRealDescription == "MusicHall")
								{
									Vector2 v6 = objectReal8.tr.position;
									string agentType5 = "Mayor";
									Agent agent7 = GC.spawnerMain.SpawnAgent(v6, null, agentType5);
									flag27 = true;
									mayorAgent = agent7;
									agent7.startingChunk = objectReal8.startingChunk;
									agent7.initialStartingChunk = agent7.startingChunk;
									agent7.startingChunkReal = objectReal8.startingChunkReal;
									agent7.startingChunkRealDescription = objectReal8.startingChunkRealDescription;
									agent7.SetDefaultGoal("ListenToJokeNPC");
									agent7.cantBump = true;
									findingFactoryLake = true;

									break;
								}
							}
						else if (lakeType == "Swimming")
						{
							List<EventTriggerFloor> list6 = new List<EventTriggerFloor>();

							for (int num72 = 0; num72 < GC.objectRealList.Count; num72++)
							{
								ObjectReal objectReal9 = GC.objectRealList[num72];

								if (objectReal9.objectName == "EventTriggerFloor")
								{
									EventTriggerFloor eventTriggerFloor3 = (EventTriggerFloor)objectReal9;

									if (eventTriggerFloor3.triggerType == "MayorSwimPosition")
									{
										list6.Add(eventTriggerFloor3);
										Vector2 v7 = objectReal9.tr.position;
										string agentType6 = "Mayor";
										Agent agent8 = GC.spawnerMain.SpawnAgent(v7, null, agentType6);
										flag27 = true;
										mayorAgent = agent8;
										agent8.startingChunk = objectReal9.startingChunk;
										agent8.initialStartingChunk = agent8.startingChunk;
										agent8.startingChunkReal = objectReal9.startingChunkReal;
										agent8.startingChunkRealDescription = objectReal9.startingChunkRealDescription;
										agent8.SetDefaultGoal("Swim");
										agent8.agentHitboxScript.SetupBodyStrings();
										agent8.cantBump = true;
										findingFactoryLake = true;

										break;
									}
								}
							}

							if (list6.Count > 0)
							{
								EventTriggerFloor eventTriggerFloor4 = list6[Random.Range(0, list6.Count)];
							}
						}
						else if (lakeType == "WalkingInOffice")
						{
							List<EventTriggerFloor> list7 = new List<EventTriggerFloor>();

							for (int num73 = 0; num73 < GC.objectRealList.Count; num73++)
							{
								ObjectReal objectReal10 = GC.objectRealList[num73];

								if (objectReal10.objectName == "EventTriggerFloor")
								{
									EventTriggerFloor eventTriggerFloor5 = (EventTriggerFloor)objectReal10;

									if (eventTriggerFloor5.triggerType == "MayorOfficePosition")
									{
										list7.Add(eventTriggerFloor5);
										Vector2 v8 = objectReal10.tr.position;
										string agentType7 = "Mayor";
										Agent agent9 = GC.spawnerMain.SpawnAgent(v8, null, agentType7);
										flag27 = true;
										mayorAgent = agent9;
										agent9.startingChunk = objectReal10.startingChunk;
										agent9.initialStartingChunk = agent9.startingChunk;
										agent9.startingChunkReal = objectReal10.startingChunkReal;
										agent9.startingChunkRealDescription = objectReal10.startingChunkRealDescription;
										agent9.SetDefaultGoal("CuriousObject");
										agent9.relationships.SetupInterests();
										agent9.cantBump = true;
										agent9.ownerID = 1;
										findingFactoryLake = true;

										break;
									}
								}
							}
						}

						if (lakeType == "Wandering" || !flag27)
						{
							Vector2 myLocation = Vector2.zero;
							int num74 = 0;

							do
							{
								myLocation = GC.tileInfo.FindRandLocationGeneral(0.32f);
								num74++;
							}
							while ((myLocation == Vector2.zero || Vector2.Distance(myLocation, GC.playerAgent.tr.position) < 10f) && num74 < 300);

							string agentType8 = "Mayor";
							Agent myAgent = GC.spawnerMain.SpawnAgent(myLocation, null, agentType8);
							mayorAgent = myAgent;
							myAgent.movement.RotateToAngleTransform((float)Random.Range(0, 360));
							myAgent.SetDefaultGoal("WanderFar");
							myLocation = Vector2.zero;
							num74 = 0;

							do
							{
								myLocation = GC.tileInfo.FindLocationNearLocation(myAgent.tr.position, null, 0.32f, 1.28f, true, true);
								num74++;
							}
							while (myLocation == Vector2.zero && num74 < 300);

							if (myLocation != Vector2.zero)
							{
								Agent.gangCount++;
								myAgent.gang = Agent.gangCount;
								myAgent.modLeashes = 0;
								Random.InitState(__instance.randomSeedNum + ++randomCount);
								int bigTries = Random.Range(4, 6);
								int num2;

								for (int numObjects = 0; numObjects < bigTries; numObjects = num2 + 1)
								{
									agentType8 = "Cop2";

									if (__instance.replaceCopWithGangbanger)
										agentType8 = "Gangbanger";

									Agent agent10 = GC.spawnerMain.SpawnAgent(myLocation, null, agentType8);
									agent10.movement.RotateToAngleTransform((float)Random.Range(0, 360));
									agent10.SetDefaultGoal("WanderFar");
									agent10.gang = Agent.gangCount;
									agent10.modVigilant = 0;
									agent10.guardingMayor = true;
									agent10.modLeashes = 1;
									agent10.specialWalkSpeed = myAgent.speedMax;
									agent10.oma.mustBeGuilty = true;

									for (int num75 = 0; num75 < GC.agentList.Count; num75++)
									{
										Agent agent11 = GC.agentList[num75];

										if (agent11.gang == myAgent.gang)
										{
											agent10.relationships.SetRelInitial(agent11, "Aligned");
											agent11.relationships.SetRelInitial(agent10, "Aligned");

											if (!agent11.gangMembers.Contains(agent10))
												agent11.gangMembers.Add(agent10);

											if (!agent10.gangMembers.Contains(agent11))
												agent10.gangMembers.Add(agent11);
										}
									}

									if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
									{
										yield return null;
										chunkStartTime = Time.realtimeSinceStartup;
									}

									Random.InitState(__instance.randomSeedNum + numObjects);
									num2 = numObjects;
								}
							}

							myLocation = default(Vector2);
							myAgent = null;
						}

						if (removeAgentsInBuilding)
						{
							List<Agent> list8 = new List<Agent>();

							for (int num76 = 0; num76 < GC.agentList.Count; num76++)
								if (GC.agentList[num76].startingChunk == mayorAgent.startingChunk && !GC.agentList[num76].isMayor && !list8.Contains(GC.agentList[num76]))
									list8.Add(GC.agentList[num76]);

							while (list8.Count > 0)
							{
								list8[0].DestroyMe();
								list8.Remove(list8[0]);
							}
						}

						if (findingFactoryLake)
							for (int num77 = 0; num77 < 5; num77++)
							{
								string agentType9 = "Cop2";

								if (__instance.replaceCopWithGangbanger)
									agentType9 = "Gangbanger";

								int num78 = 0;
								Vector2 vector29 = Vector2.zero;

								do
								{
									vector29 = GC.tileInfo.FindLocationNearLocation(mayorAgent.tr.position, null, 0.32f, 1.28f, true, false);

									if (!GC.tileInfo.IsIndoors(vector29))
										vector29 = Vector2.zero;

									num78++;
								}
								while (vector29 == Vector2.zero && num78 < 300);

								if (vector29 != Vector2.zero)
								{
									Agent agent12 = GC.spawnerMain.SpawnAgent(vector29, null, agentType9);
									agent12.movement.RotateToAngleTransform((float)Random.Range(0, 360));
									agent12.startingChunk = mayorAgent.startingChunk;
									agent12.initialStartingChunk = agent12.startingChunk;
									agent12.startingChunkReal = mayorAgent.startingChunkReal;
									agent12.startingChunkRealDescription = mayorAgent.startingChunkRealDescription;
									agent12.SetDefaultGoal("CuriousObject");
									agent12.relationships.SetupInterests();
									agent12.guardingMayor = true;
									agent12.modLeashes = 1;
									agent12.oma.mustBeGuilty = true;

									if (lakeType == "WalkingInOffice")
									{
										agent12.ownerID = 1;
										agent12.SetFollowing(mayorAgent);
									}
									else if (lakeType == "Sleeping")
									{
										agent12.ownerID = 1;
										agent12.oma.modProtectsProperty = 2;
									}
									else if (lakeType == "Swimming")
									{
										agent12.SetDefaultGoal("Swim");
										agent12.agentHitboxScript.SetupBodyStrings();
										agent12.SetFollowing(mayorAgent);
									}

									if (num77 < 3)
										agent12.modVigilant = 0;
								}
							}

						for (int num79 = 0; num79 < GC.agentList.Count; num79++)
						{
							Agent agent13 = GC.agentList[num79];

							if (agent13.startingChunkRealDescription == "MayorHouse" || agent13.startingChunkRealDescription == "MayorOffice")
								agent13.oma.mustBeGuilty = true;
						}

						mayorAgent = null;
						lakeType = null;
					}
					#endregion
					#region Roamers
					if (GC.levelFeeling != vLevelFeeling.RadiationBlasts)
					{
						#region Generic
						bool hasGenericRoamers = true;

						if (GC.sessionDataBig.curLevel >= 1)
							hasGenericRoamers = true;

						if (GC.customLevel)
							hasGenericRoamers = __instance.customLevel.levelFeatures.Contains(vAgent.SlumDweller);

						if (hasGenericRoamers && GC.levelFeeling != vLevelFeeling.Riot && GC.levelFeeling != vLevelFeeling.Lockdown && GC.levelFeeling != vLevelFeeling.WarZone)
						{
							Debug.Log("Loading Roamers");

							int bigTries = (int)((float)Random.Range(16, 20) * __instance.levelSizeModifier);
							bigTries = GenPopCount(bigTries);
							int num2;

							for (int numObjects = 0; numObjects < bigTries; numObjects = num2 + 1)
							{
								Vector2 trySpot = Vector2.zero;
								int num80 = 0;

								do
								{
									trySpot = GC.tileInfo.FindRandLocationGeneral(0.32f);
									num80++;
								}
								while ((trySpot == Vector2.zero || Vector2.Distance(trySpot, GC.playerAgent.tr.position) < 10f) && num80 < 300);

								if (trySpot != Vector2.zero)
								{
									string roamerAgent = "Hobo";
									Random.InitState(__instance.randomSeedNum + numObjects + ++randomCount);

									if (GC.challenges.Contains("MixedUpLevels") && GC.percentChance(33))
									{
										roamerAgent = GC.Choose<string>("Hobo", "Worker", "ParkAgent", "Agents", "UpperCruster");

										if (GC.challenges.Contains(cChallenge.LetMeSeeThatThrong) && GC.percentChance(1))
											roamerAgent = "Thief";
										else if (GC.challenges.Contains(cChallenge.SwarmWelcome) && GC.percentChance(3))
											roamerAgent = "Thief";
										else if (GC.percentChance(10))
											roamerAgent = "Thief";

										if (GC.challenges.Contains("ZombiesWelcome") && GC.percentChance(10))
											roamerAgent = "Zombie";

										if (GC.challenges.Contains("CannibalsDontAttack") && GC.percentChance(10))
											roamerAgent = "Cannibal";

										if (GC.challenges.Contains("DoctorsMoreImportant") && GC.percentChance(10))
											roamerAgent = "Doctor";
									}
									else
									{
										if (GC.levelTheme == 0)
										{
											if (GC.challenges.Contains(cChallenge.AnCapistan))
											{
												if (GC.challenges.Contains(cChallenge.LetMeSeeThatThrong) && GC.percentChance(3))
													roamerAgent = "Thief";
												else if (GC.challenges.Contains(cChallenge.SwarmWelcome) && GC.percentChance(5))
													roamerAgent = "Thief";
												else if (GC.percentChance(10))
													roamerAgent = "Thief";

												if (GC.challenges.Contains("ZombiesWelcome") && GC.percentChance(10))
													roamerAgent = "Zombie";

												if (GC.challenges.Contains("CannibalsDontAttack") && GC.percentChance(10))
													roamerAgent = "Cannibal";

												if (GC.challenges.Contains("DoctorsMoreImportant") && GC.percentChance(5))
													roamerAgent = "Doctor";

												else if (GC.percentChance(10))
													roamerAgent = GC.Choose<string>(vAgent.DrugDealer, vAgent.Crepe, vAgent.Blahd, vAgent.Thief) ;
											}
											else if (GC.challenges.Contains(cChallenge.PoliceState))
											{
												if (GC.percentChance(10))
													roamerAgent = "Firefighter";
												else
													roamerAgent = "Hobo";

												if (GC.challenges.Contains("DoctorsMoreImportant") && GC.percentChance(10))
													roamerAgent = "Doctor";
											}
											else
											{
												if (hasPoliceBoxes)
												{
													if (GC.percentChance(10))
														roamerAgent = "Firefighter";
													else
														roamerAgent = GC.Choose<string>("Hobo", "UpperCruster");
												}

												if (GC.challenges.Contains(cChallenge.LetMeSeeThatThrong) && GC.percentChance(1))
													roamerAgent = "Thief";
												else if (GC.challenges.Contains(cChallenge.SwarmWelcome) && GC.percentChance(3))
													roamerAgent = "Thief";
												else if (GC.percentChance(10))
													roamerAgent = "Thief";

												if (GC.challenges.Contains("ZombiesWelcome") && GC.percentChance(10))
													roamerAgent = "Zombie";

												if (GC.challenges.Contains("CannibalsDontAttack") && GC.percentChance(10))
													roamerAgent = "Cannibal";

												if (GC.challenges.Contains("DoctorsMoreImportant") && GC.percentChance(10))
													roamerAgent = "Doctor";
											}
										}
										else if (GC.levelTheme == 1)
										{
											if (GC.challenges.Contains(cChallenge.AnCapistan))
											{
												roamerAgent = GC.Choose<string> (vAgent.SlumDweller, vAgent.SlumDweller, vAgent.Worker);

												if (GC.challenges.Contains(cChallenge.LetMeSeeThatThrong) && GC.percentChance(1))
													roamerAgent = "Thief";
												else if (GC.challenges.Contains(cChallenge.SwarmWelcome) && GC.percentChance(3))
													roamerAgent = "Thief";
												else if (GC.percentChance(10))
													roamerAgent = "Thief";

												if (GC.challenges.Contains("ZombiesWelcome") && GC.percentChance(10))
													roamerAgent = "Zombie";

												if (GC.challenges.Contains("CannibalsDontAttack") && GC.percentChance(10))
													roamerAgent = "Cannibal";

												if (GC.challenges.Contains("DoctorsMoreImportant") && GC.percentChance(10))
													roamerAgent = "Doctor";
											}
											else if (GC.challenges.Contains(cChallenge.PoliceState))
											{
												if (GC.percentChance(10))
													roamerAgent = "Firefighter";
												else
													roamerAgent = GC.Choose<string>("Hobo", "Worker", "UpperCruster");

												if (GC.challenges.Contains("DoctorsMoreImportant") && GC.percentChance(10))
													roamerAgent = "Doctor";
											}
											else
											{
												if (GC.percentChance(10))
													roamerAgent = "Firefighter";
												else if (hasPoliceBoxes)
													roamerAgent = GC.Choose<string>("Hobo", "Worker", "UpperCruster");
												else
													roamerAgent = GC.Choose<string>("Hobo", "Worker");

												if (GC.challenges.Contains(cChallenge.LetMeSeeThatThrong) && GC.percentChance(1))
													roamerAgent = "Thief";
												else if (GC.challenges.Contains(cChallenge.SwarmWelcome) && GC.percentChance(3))
													roamerAgent = "Thief";
												else if (GC.percentChance(10))
													roamerAgent = "Thief";

												if (GC.challenges.Contains("ZombiesWelcome") && GC.percentChance(10))
													roamerAgent = "Zombie";

												if (GC.challenges.Contains("CannibalsDontAttack") && GC.percentChance(10))
													roamerAgent = "Cannibal";

												if (GC.challenges.Contains("DoctorsMoreImportant") && GC.percentChance(10))
													roamerAgent = "Doctor";
											}
										}
										else if (GC.levelTheme == 2)
										{
											if (GC.challenges.Contains(cChallenge.AnCapistan))
											{
												roamerAgent = GC.Choose<string>(GC.rnd.RandomSelect("ParkAgent", "Agents"), vAgent.SlumDweller);

												if (GC.challenges.Contains(cChallenge.LetMeSeeThatThrong) && GC.percentChance(3))
													roamerAgent = "Thief";
												else if (GC.challenges.Contains(cChallenge.SwarmWelcome) && GC.percentChance(5))
													roamerAgent = "Thief";
												else if (GC.percentChance(10))
													roamerAgent = "Thief";

												if (GC.challenges.Contains("ZombiesWelcome") && GC.percentChance(10))
													roamerAgent = "Zombie";

												if (GC.challenges.Contains("CannibalsDontAttack") && GC.percentChance(10))
													roamerAgent = "Cannibal";

												if (GC.challenges.Contains("DoctorsMoreImportant") && GC.percentChance(10))
													roamerAgent = "Doctor";
											}
											else if (GC.challenges.Contains(cChallenge.PoliceState))
											{
												roamerAgent = GC.Choose<string>(GC.rnd.RandomSelect("ParkAgent", "Agents"), "UpperCruster");

												if (GC.challenges.Contains("DoctorsMoreImportant") && GC.percentChance(10))
													roamerAgent = "Doctor";
											}
											else
											{
												if (hasPoliceBoxes)
													GC.Choose<string>(GC.rnd.RandomSelect("ParkAgent", "Agents"), "UpperCruster");
												else
													roamerAgent = GC.rnd.RandomSelect("ParkAgent", "Agents");

												if (GC.challenges.Contains(cChallenge.LetMeSeeThatThrong) && GC.percentChance(1))
													roamerAgent = "Thief";
												else if (GC.challenges.Contains(cChallenge.SwarmWelcome) && GC.percentChance(3))
													roamerAgent = "Thief";
												else if (GC.percentChance(10))
													roamerAgent = "Thief";

												if (GC.challenges.Contains("ZombiesWelcome") && GC.percentChance(10))
													roamerAgent = "Zombie";

												if (GC.challenges.Contains("CannibalsDontAttack") && GC.percentChance(10))
													roamerAgent = "Cannibal";

												if (GC.challenges.Contains("DoctorsMoreImportant") && GC.percentChance(10))
													roamerAgent = "Doctor";
											}
										}
										else if (GC.levelTheme == 3)
										{
											if (GC.challenges.Contains(cChallenge.AnCapistan))
											{
												roamerAgent = GC.Choose<string>("Hobo", "Hobo", "UpperCruster", "UpperCruster");

												if (GC.challenges.Contains(cChallenge.LetMeSeeThatThrong) && GC.percentChance(3))
													roamerAgent = "Thief";
												else if (GC.challenges.Contains(cChallenge.SwarmWelcome) && GC.percentChance(5))
													roamerAgent = "Thief";
												else if (GC.percentChance(10))
													roamerAgent = "Thief";

												if (GC.challenges.Contains("ZombiesWelcome") && GC.percentChance(10))
													roamerAgent = "Zombie";

												if (GC.challenges.Contains("CannibalsDontAttack") && GC.percentChance(10))
													roamerAgent = "Cannibal";

												if (GC.challenges.Contains("DoctorsMoreImportant") && GC.percentChance(10))
													roamerAgent = "Doctor";

												if (GC.percentChance(3))
													roamerAgent = "Vampire";
											}
											else if (GC.challenges.Contains(cChallenge.PoliceState))
											{
												if (GC.percentChance(8))
													roamerAgent = "Firefighter";
												else
													roamerAgent = GC.Choose<string>("Hobo", "UpperCruster", "UpperCruster");

												if (GC.challenges.Contains("ZombiesWelcome") && GC.percentChance(10))
													roamerAgent = "Zombie";

												if (GC.challenges.Contains("CannibalsDontAttack") && GC.percentChance(10))
													roamerAgent = "Cannibal";

												if (GC.challenges.Contains("DoctorsMoreImportant") && GC.percentChance(10))
													roamerAgent = "Doctor";

												if (GC.percentChance(3))
													roamerAgent = "Vampire";
											}
											else
											{
												if (GC.percentChance(8))
													roamerAgent = "Firefighter";
												else
													roamerAgent = GC.Choose<string>("Hobo", "UpperCruster", "UpperCruster");

												if (GC.challenges.Contains(cChallenge.LetMeSeeThatThrong) && GC.percentChance(1))
													roamerAgent = "Thief";
												else if (GC.challenges.Contains(cChallenge.SwarmWelcome) && GC.percentChance(3))
													roamerAgent = "Thief";
												else if (GC.percentChance(10))
													roamerAgent = "Thief";

												if (GC.challenges.Contains("DoctorsMoreImportant") && GC.percentChance(10))
													roamerAgent = "Doctor";

												if (GC.percentChance(3))
													roamerAgent = "Vampire";

											}
										}
										else if (GC.levelTheme == 4)
										{
											if (GC.challenges.Contains(cChallenge.AnCapistan))
											{
												roamerAgent = GC.Choose<string>("UpperCruster", vAgent.SlumDweller);

												if (GC.challenges.Contains("ZombiesWelcome") && GC.percentChance(10))
													roamerAgent = "Zombie";

												if (GC.challenges.Contains("CannibalsDontAttack") && GC.percentChance(10))
													roamerAgent = "Cannibal";

												if (GC.challenges.Contains("DoctorsMoreImportant") && GC.percentChance(10))
													roamerAgent = "Doctor";

												if (GC.percentChance(3))
													roamerAgent = "Vampire";
											}
											else if (GC.challenges.Contains(cChallenge.PoliceState))
											{
												if (GC.percentChance(8))
													roamerAgent = "Firefighter";
												else
													roamerAgent = GC.Choose<string>("UpperCruster", "UpperCruster");

												if (GC.challenges.Contains("DoctorsMoreImportant") && GC.percentChance(10))
													roamerAgent = "Doctor";

												if (GC.percentChance(3))
													roamerAgent = "Vampire";
											}
											else
											{
												if (GC.percentChance(8))
													roamerAgent = "Firefighter";
												else
													roamerAgent = GC.Choose<string>("UpperCruster", "UpperCruster", new string[0]);

												if (GC.challenges.Contains("ZombiesWelcome") && GC.percentChance(10))
													roamerAgent = "Zombie";

												if (GC.challenges.Contains("CannibalsDontAttack") && GC.percentChance(10))
													roamerAgent = "Cannibal";

												if (GC.challenges.Contains("DoctorsMoreImportant") && GC.percentChance(10))
													roamerAgent = "Doctor";

												if (GC.percentChance(3))
													roamerAgent = "Vampire";
											}
										}
										else if (GC.levelTheme == 5)
										{
											if (GC.challenges.Contains(cChallenge.AnCapistan))
											{
												roamerAgent = vAgent.UpperCruster;

												if (GC.challenges.Contains("ZombiesWelcome") && GC.percentChance(10))
													roamerAgent = "Zombie";

												if (GC.challenges.Contains("CannibalsDontAttack") && GC.percentChance(10))
													roamerAgent = "Cannibal";

												if (GC.challenges.Contains("DoctorsMoreImportant") && GC.percentChance(10))
													roamerAgent = "Doctor";
											}
											else if (GC.challenges.Contains(cChallenge.PoliceState))
											{
												if (GC.percentChance(10))
													roamerAgent = "Firefighter";
												else
													roamerAgent = GC.Choose<string>("UpperCruster", "UpperCruster");

												if (GC.challenges.Contains("DoctorsMoreImportant") && GC.percentChance(10))
													roamerAgent = "Doctor";
											}
											else
											{
												if (GC.percentChance(10))
													roamerAgent = "Firefighter";
												else
													roamerAgent = GC.Choose<string>("UpperCruster", "UpperCruster");

												if (GC.challenges.Contains("ZombiesWelcome") && GC.percentChance(10))
													roamerAgent = "Zombie";

												if (GC.challenges.Contains("CannibalsDontAttack") && GC.percentChance(10))
													roamerAgent = "Cannibal";

												if (GC.challenges.Contains("DoctorsMoreImportant") && GC.percentChance(10))
													roamerAgent = "Doctor";
											}
										}
									}

									Agent spawnedAgent = GC.spawnerMain.SpawnAgent(trySpot, null, roamerAgent);
									spawnedAgent.movement.RotateToAngleTransform((float)Random.Range(0, 360));

									if (roamerAgent == vAgent.UpperCruster)
									{
										if (GC.percentChance(20))
										{
											trySpot = Vector2.zero;
											num80 = 0;
											Random.InitState(__instance.randomSeedNum + numObjects + ++randomCount);

											do
											{
												trySpot = GC.tileInfo.FindLocationNearLocation(spawnedAgent.tr.position, null, 0.32f, 1.28f, true, true);
												num80++;
											}
											while (trySpot == Vector2.zero && num80 < 300);

											if (trySpot != Vector2.zero && num80 < 300)
											{
												roamerAgent = vAgent.Slave;
												Agent agent15 = GC.spawnerMain.SpawnAgent(trySpot, null, roamerAgent);
												agent15.movement.RotateToAngleTransform((float)Random.Range(0, 360));
												agent15.relationships.SetRelInitial(spawnedAgent, "Submissive");
												spawnedAgent.relationships.SetRelInitial(agent15, "Aligned");
												agent15.slaveOwners.Add(spawnedAgent);
												spawnedAgent.slavesOwned.Add(agent15);
												Agent.gangCount++;
												spawnedAgent.gang = Agent.gangCount;
												agent15.gang = Agent.gangCount;
												agent15.modLeashes = 0;
												spawnedAgent.gangMembers.Add(agent15);
												agent15.gangMembers.Add(spawnedAgent);
												agent15.SetDefaultGoal("WanderFar");
											}
										}
									}
									else if (roamerAgent == vAgent.Thief && GC.percentChance(50))
										spawnedAgent.losCheckAtIntervals = true;
								}

								if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
								{
									yield return null;
									chunkStartTime = Time.realtimeSinceStartup;
								}

								Random.InitState(__instance.randomSeedNum + numObjects);
								num2 = numObjects;
							}
						}
						#endregion
						#region Musician
						bool hasMusician = false;

						if ((GC.levelTheme == 3 || GC.levelTheme == 4 || GC.levelTheme == 5 || (GC.challenges.Contains("MixedUpLevels") && GC.percentChance(33))) && GC.percentChance(33))
							hasMusician = true;

						if (GC.customLevel)
							hasMusician = __instance.customLevel.levelFeatures.Contains("Musician");

						if (GC.challenges.Contains(cChallenge.DiscoCityDanceoff))
							hasMusician = true;

						if (hasMusician && GC.levelFeeling != "Riot" && GC.levelFeeling != "HarmAtIntervals" && GC.levelFeeling != "Lockdown" && GC.levelFeeling != "WarZone")
						{
							Debug.Log("Loading Musician");
							int bigTries = 1;

							if (GC.challenges.Contains(cChallenge.DiscoCityDanceoff))
								bigTries = 4 * LevelSizeRatio();

							int num2;

							for (int numObjects = 0; numObjects < bigTries; numObjects = num2 + 1)
							{
								Vector2 myLocation = Vector2.zero;
								int num81 = 0;

								do
								{
									myLocation = GC.tileInfo.FindRandLocationGeneral(0.32f);
									num81++;
								}
								while ((myLocation == Vector2.zero || Vector2.Distance(myLocation, GC.playerAgent.tr.position) < 10f) && num81 < 300);

								if (myLocation != Vector2.zero && num81 < 300)
								{
									string agentType10 = "Musician";
									Agent mayorAgent = GC.spawnerMain.SpawnAgent(myLocation, null, agentType10);
									mayorAgent.movement.RotateToAngleTransform((float)Random.Range(0, 360));
									mayorAgent.SetDefaultGoal("WanderFar");
									myLocation = Vector2.zero;
									mayorAgent.mustSpillMoney = true;
									num81 = 0;
									Random.InitState(__instance.randomSeedNum + numObjects + ++randomCount);

									do
									{
										myLocation = GC.tileInfo.FindLocationNearLocation(mayorAgent.tr.position, null, 0.32f, 1.28f, true, true);
										num81++;
									}
									while (myLocation == Vector2.zero && num81 < 300);

									if (myLocation != Vector2.zero)
									{
										Agent.gangCount++;
										mayorAgent.gang = Agent.gangCount;
										mayorAgent.modLeashes = 0;
										Random.InitState(__instance.randomSeedNum + numObjects + ++randomCount);
										int i = Random.Range(2, 4);

										for (int j = 0; j < i; j = num2 + 1)
										{
											if (GC.challenges.Contains(cChallenge.DiscoCityDanceoff))
												agentType10 = vAgent.Musician;
											else if ((!GC.challenges.Contains("QuickGame") && GC.sessionDataBig.curLevelEndless > 9) || (GC.challenges.Contains("QuickGame") && GC.sessionDataBig.curLevelEndless > 6))
												agentType10 = vAgent.Goon;
											else
												agentType10 = vAgent.Supergoon;

											Agent agent16 = GC.spawnerMain.SpawnAgent(myLocation, null, agentType10);
											agent16.movement.RotateToAngleTransform((float)Random.Range(0, 360));
											agent16.SetDefaultGoal("WanderFar");
											agent16.gang = Agent.gangCount;
											agent16.modLeashes = 0;
											agent16.modVigilant = 0;

											for (int num82 = 0; num82 < GC.agentList.Count; num82++)
											{
												Agent agent17 = GC.agentList[num82];

												if (agent17.gang == mayorAgent.gang)
												{
													agent16.relationships.SetRelInitial(agent17, "Aligned");
													agent17.relationships.SetRelInitial(agent16, "Aligned");

													if (!agent17.gangMembers.Contains(agent16))
														agent17.gangMembers.Add(agent16);

													if (!agent16.gangMembers.Contains(agent17))
														agent16.gangMembers.Add(agent17);
												}
											}

											if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
											{
												yield return null;
												chunkStartTime = Time.realtimeSinceStartup;
											}

											Random.InitState(__instance.randomSeedNum + j);
											num2 = j;
										}
									}

									mayorAgent = null;
								}

								myLocation = default(Vector2);
								num2 = numObjects;
							}
						}
						#endregion
						#region Cops
						bool hasCops = false;

						if ((GC.sessionDataBig.curLevel >= 2 || GC.levelTheme != 0) && GC.levelTheme != 2 && !GC.challenges.Contains("NoCops"))
							hasCops = true;

						if (GC.challenges.Contains(cChallenge.PoliceState) || (GC.levelTheme == 3 || GC.levelTheme == 4 || GC.levelTheme == 5) && !GC.challenges.Contains("NoCops") && GC.debugMode)
							hasCops = true;

						if (GC.customLevel)
							hasCops = __instance.customLevel.levelFeatures.Contains("Cop");

						if (GC.challenges.Contains(cChallenge.AnCapistan))
							hasCops = false;

						if (hasCops)
						{
							Debug.Log("Loading Cops");
							int bigTries = (int)((float)Random.Range(6, 10) * __instance.levelSizeModifier);
							bigTries -= earlyCops;
							int num2;

							for (int numObjects = 0; numObjects < bigTries; numObjects = num2 + 1)
							{
								Vector2 spot = Vector2.zero;
								int num83 = 0;

								do
								{
									spot = GC.tileInfo.FindRandLocationGeneral(0.32f);
									num83++;
								}
								while ((spot == Vector2.zero || Vector2.Distance(spot, GC.playerAgent.tr.position) < 20f) && num83 < 300);

								if (spot != Vector2.zero && num83 < 300)
								{
									string agentName = vAgent.Cop;

									if (GC.levelTheme == 4 || GC.levelTheme == 5 || GC.challenges.Contains(vChallenge.SupercopLand) || GC.challenges.Contains(cChallenge.PoliceState))
										agentName = vAgent.SuperCop;

									if (__instance.replaceCopWithGangbanger)
										agentName = "Gangbanger";

									Agent agent = GC.spawnerMain.SpawnAgent(spot, null, agentName);
									agent.movement.RotateToAngleTransform((float)Random.Range(0, 360));

									if ((agentName == "Cop" || agentName == "Cop2") && GC.levelFeeling == "Lockdown")
										agent.oma.modProtectsProperty = 1;

									if (GC.challenges.Contains(cChallenge.PoliceState))
									{
										foreach (Agent otherAgent in GC.agentList)
											if (!vAgent.LawEnforcement.Contains(otherAgent.agentName))
											{
												agent.relationships.SetRelInitial(otherAgent, vRelationship.Annoyed);

												agent.relationships.SetStrikes(otherAgent, 2);
											}
									}

									if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
									{
										yield return null;
										chunkStartTime = Time.realtimeSinceStartup;
									}

									Random.InitState(__instance.randomSeedNum + numObjects);
								}

								num2 = numObjects;
							}
						}
						#endregion
						#region Extra Cops
						bool extraCops = false;

						if (GC.sessionData.nextLevelExtraCops)
						{
							extraCops = true;
							GC.sessionData.nextLevelExtraCops = false;
						}

						if (GC.challenges.Contains(cChallenge.AnCapistan))
							extraCops = false;

						if (extraCops)
						{
							Debug.Log("Loading Extra Cops");
							int bigTries = (int)((float)Random.Range(6, 10) * __instance.levelSizeModifier);
							bigTries -= earlyCops;
							int num2;

							for (int numObjects = 0; numObjects < bigTries; numObjects = num2 + 1)
							{
								Vector2 vector32 = Vector2.zero;
								int num84 = 0;

								do
								{
									vector32 = GC.tileInfo.FindRandLocationGeneral(0.32f);
									num84++;
								}
								while ((vector32 == Vector2.zero || Vector2.Distance(vector32, GC.playerAgent.tr.position) < 20f) && num84 < 300);

								if (vector32 != Vector2.zero && num84 < 300)
								{
									string agentType11 = "Cop2";
									Agent agent19 = GC.spawnerMain.SpawnAgent(vector32, null, agentType11);
									agent19.movement.RotateToAngleTransform((float)Random.Range(0, 360));

									if (GC.levelFeeling == "Lockdown")
										agent19.oma.modProtectsProperty = 1;

									if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
									{
										yield return null;
										chunkStartTime = Time.realtimeSinceStartup;
									}

									Random.InitState(__instance.randomSeedNum + numObjects);
								}

								num2 = numObjects;
							}
						}

						for (int num85 = 0; num85 < GC.objectRealList.Count; num85++)
						{
							ObjectReal objectReal11 = GC.objectRealList[num85];

							if (objectReal11.objectName == "PowerBox")
							{
								if (hasExtraPowerBoxes)
									__instance.SpawnCopNearLocation(objectReal11.tr.position, 1.28f, 2.56f, GC.Choose<int>(2, 3, new int[0]));
								else
									__instance.SpawnCopNearLocation(objectReal11.tr.position, 1.28f, 2.56f);
							}
							else if (objectReal11.objectName == "ATMMachine")
								__instance.SpawnCopNearLocation(objectReal11.tr.position, 0.64f, 2.56f);
						}
						#endregion
						#region Cop Bots
						bool hasCopBots = false;

						if (GC.challenges.Contains(cChallenge.PoliceState) || GC.levelTheme == 4 && !GC.challenges.Contains(vChallenge.NoCops))
							hasCopBots = true;

						if (GC.customLevel)
							hasCopBots = __instance.customLevel.levelFeatures.Contains(vAgent.CopBot);

						if (GC.challenges.Contains(cChallenge.AnCapistan))
							hasCops = false;

						if (hasCopBots)
						{
							Debug.Log("Loading CopBots");
							int bigTries = (int)((float)Random.Range(4, 6) * __instance.levelSizeModifier);
							string lakeType = "Normal";
							int num2;

							for (int numObjects = 0; numObjects < bigTries; numObjects = num2 + 1)
							{
								Vector2 vector33 = Vector2.zero;
								int num86 = 0;

								do
								{
									vector33 = GC.tileInfo.FindRandLocationGeneral(0.32f);

									for (int num87 = 0; num87 < GC.agentList.Count; num87++)
										if (GC.agentList[num87].agentName == "CopBot" && Vector2.Distance(GC.agentList[num87].tr.position, vector33) < 14f)
											vector33 = Vector2.zero;

									num86++;
								}
								while ((vector33 == Vector2.zero || Vector2.Distance(vector33, GC.playerAgent.tr.position) < 20f) && num86 < 500);

								if (vector33 != Vector2.zero && num86 < 500)
								{
									string agentType12 = "CopBot";
									Agent agent20 = GC.spawnerMain.SpawnAgent(vector33, null, agentType12);
									agent20.movement.RotateToAngleTransform((float)Random.Range(0, 360));
									agent20.oma.securityType = agent20.oma.convertSecurityTypeToInt(lakeType);

									if (lakeType == "Normal")
										lakeType = "ID";
									else if (lakeType == "ID")
										lakeType = "Weapons";
									else if (lakeType == "Weapons")
										lakeType = "Normal";

									if (GC.levelFeeling == "Lockdown")
										agent20.oma.modProtectsProperty = 1;

									if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
									{
										yield return null;
										chunkStartTime = Time.realtimeSinceStartup;
									}

									Random.InitState(__instance.randomSeedNum + numObjects);
								}

								num2 = numObjects;
							}

							lakeType = null;
						}
						#endregion
						#region Zombies
						bool hasZombies = false;

						if (GC.challenges.Contains(vChallenge.ZombieMutator))
							hasZombies = true;

						if (GC.customLevel)
							hasZombies = __instance.customLevel.levelFeatures.Contains("Zombie");

						if (GC.challenges.Contains(cChallenge.PoliceState))
							hasZombies = false;

						if (hasZombies)
						{
							Debug.Log("Loading Zombies");
							int bigTries = (int)((float)Random.Range(25, 36) * __instance.levelSizeModifier);
							int num2;

							for (int numObjects = 0; numObjects < bigTries; numObjects = num2 + 1)
							{
								Vector2 vector34 = Vector2.zero;
								int num88 = 0;

								do
								{
									vector34 = GC.tileInfo.FindRandLocationGeneral(0.32f);
									num88++;
								}
								while ((vector34 == Vector2.zero || Vector2.Distance(vector34, GC.playerAgent.tr.position) < 20f) && num88 < 200);

								if (vector34 != Vector2.zero && num88 < 200)
								{
									string agentType13 = "Zombie";
									Agent agent21 = GC.spawnerMain.SpawnAgent(vector34, null, agentType13);
									agent21.movement.RotateToAngleTransform((float)Random.Range(0, 360));
									agent21.lowerItemChance = true;
									Object.Destroy(agent21.nonQuestObjectMarker);

									if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
									{
										yield return null;
										chunkStartTime = Time.realtimeSinceStartup;
									}

									Random.InitState(__instance.randomSeedNum + numObjects);
								}

								num2 = numObjects;
							}
						}
						#endregion
						#region Gangbangers
						bool hasGangbangers = false;
						bool removeAgentsInBuilding = false;
						bool findingFactoryLake = false;

						if ((GC.sessionDataBig.curLevel >= 2 && (GC.levelTheme == 0 || GC.levelTheme == 1 || GC.levelTheme == 2 || GC.levelTheme == 3)) || 
							(GC.sessionDataBig.curLevel >= 2 && GC.challenges.Contains("MixedUpLevels") && GC.percentChance(33)))
							hasGangbangers = true;

						if (GC.levelTheme == 2 || GC.levelTheme == 4)
						{
							for (int num89 = 0; num89 < GC.playerAgentList.Count; num89++)
							{
								if (GC.playerAgentList[num89].bigQuest == "Gangbanger")
									findingFactoryLake = true;
								else if (GC.playerAgentList[num89].bigQuest == "GangbangerB")
									removeAgentsInBuilding = true;
							}

							if (removeAgentsInBuilding || findingFactoryLake)
								hasGangbangers = true;
						}

						if (GC.customLevel)
							hasGangbangers = __instance.customLevel.levelFeatures.Contains("Gangbanger");

						if (GC.challenges.Contains(cChallenge.YoungMenInTheNeighborhood))
							hasGangbangers = true;

						if (GC.challenges.Contains(cChallenge.PoliceState))
							hasGangbangers = false;

						if (GC.challenges.Contains(cChallenge.AnCapistan))
							hasGangbangers = true;

						if (hasGangbangers && GC.levelFeeling != "HarmAtIntervals" && GC.levelFeeling != "Lockdown" && GC.levelFeeling != "WarZone" && GC.levelFeeling != "Riot")
						{
							Debug.Log("Loading Roving Gangs");

							int bigTries = GC.Choose<int>(0, 0, 0, 0, 1, 1, 2);
							bigTries = (int)(GangCount(bigTries) * (float)__instance.levelSizeMax / 30f);

							bool placedGangbangers = false;
							bool placedGangbangersB = false;

							if (removeAgentsInBuilding && findingFactoryLake)
								bigTries = 2;
							else if ((removeAgentsInBuilding || findingFactoryLake) && (bigTries == 0 || bigTries == 1))
								bigTries = GC.Choose<int>(1, 2, new int[0]);
							else if (GC.levelTheme == 2)
								bigTries = GC.Choose<int>(1, 1, 1, 1, 1, 2);

							int num2;

							for (int numObjects = 0; numObjects < bigTries; numObjects = num2 + 1)
							{
								string agentType14 = GC.Choose<string>("Gangbanger", "GangbangerB", new string[0]);

								if (removeAgentsInBuilding && numObjects == 0 && !placedGangbangers)
								{
									agentType14 = "Gangbanger";
									placedGangbangers = true;
								}
								else if (findingFactoryLake && numObjects == 0 && !placedGangbangersB)
								{
									agentType14 = "GangbangerB";
									placedGangbangersB = true;
								}
								else if ((placedGangbangers || placedGangbangersB) && numObjects == 1)
								{
									if (placedGangbangers)
										agentType14 = "GangbangerB";
									else
										agentType14 = "Gangbanger";
								}

								Agent.gangCount++;
								int num90 = Random.Range(3, 5);

								if (GC.levelTheme == 2)
									num90 = Random.Range(2, 4);

								Vector2 pos2 = Vector2.zero;

								for (int num91 = 0; num91 < num90; num91++)
								{
									Vector2 vector35 = Vector2.zero;
									int num92 = 0;

									if (num91 == 0)
									{
										do
										{
											vector35 = GC.tileInfo.FindRandLocationGeneral(0.32f);
											num92++;
										}
										while ((vector35 == Vector2.zero || Vector2.Distance(vector35, GC.playerAgent.tr.position) < 20f) && num92 < 300);

										pos2 = vector35;
									}
									else
										vector35 = GC.tileInfo.FindLocationNearLocation(pos2, null, 0.32f, 1.28f, true, true);

									if (vector35 != Vector2.zero && num92 < 300)
									{
										Agent agent22 = GC.spawnerMain.SpawnAgent(vector35, null, agentType14);
										agent22.movement.RotateToAngleTransform((float)Random.Range(0, 360));
										agent22.gang = Agent.gangCount;
										agent22.modLeashes = 0;

										for (int num93 = 0; num93 < GC.agentList.Count; num93++)
											if (GC.agentList[num93].gang == agent22.gang)
											{
												GC.agentList[num93].gangMembers.Add(agent22);

												if (!agent22.gangMembers.Contains(GC.agentList[num93]))
													agent22.gangMembers.Add(GC.agentList[num93]);
											}
									}
								}

								if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
								{
									yield return null;
									chunkStartTime = Time.realtimeSinceStartup;
								}

								Random.InitState(__instance.randomSeedNum + numObjects);
								num2 = numObjects;
							}
						}
						#endregion
						#region Mafia
						bool hasMafia = false;

						if (GC.levelTheme == 3 || (GC.sessionDataBig.curLevel >= 2 && GC.challenges.Contains("MixedUpLevels") && GC.percentChance(33)))
							hasMafia = true;

						if (GC.customLevel)
							hasMafia = __instance.customLevel.levelFeatures.Contains("Mafia");

						if (GC.challenges.Contains(cChallenge.MobTown) || GC.challenges.Contains(cChallenge.AnCapistan))
							hasMafia = true;

						if (GC.challenges.Contains(cChallenge.PoliceState))
							hasMafia = false;

						if (hasMafia && GC.levelFeeling != "HarmAtIntervals" && GC.levelFeeling != "Lockdown" && GC.levelFeeling != "WarZone")
						{
							Debug.Log("Loading Mafia");
							int bigTries = Random.Range(3, 5);
							bigTries = MafiaCount(bigTries);
							int num2;

							for (int numObjects = 0; numObjects < bigTries; numObjects = num2 + 1)
							{
								Agent.gangCount++;
								int num94 = Random.Range(3, 5);
								Vector2 pos3 = Vector2.zero;

								for (int num95 = 0; num95 < num94; num95++)
								{
									Vector2 vector36 = Vector2.zero;
									int num96 = 0;

									if (num95 == 0)
									{
										do
										{
											vector36 = GC.tileInfo.FindRandLocationGeneral(0.32f);
											num96++;
										}
										while ((vector36 == Vector2.zero || Vector2.Distance(vector36, GC.playerAgent.tr.position) < 20f) && num96 < 300);

										pos3 = vector36;
									}
									else
										vector36 = GC.tileInfo.FindLocationNearLocation(pos3, null, 0.32f, 1.28f, true, true);

									if (vector36 != Vector2.zero && num96 < 300)
									{
										Agent agent23 = GC.spawnerMain.SpawnAgent(vector36, null, "Mafia");
										agent23.movement.RotateToAngleTransform((float)Random.Range(0, 360));
										agent23.gang = Agent.gangCount;
										agent23.modLeashes = 0;

										if (num95 == 0 && numObjects == 0)
											agent23.gangLeader = true;

										for (int num97 = 0; num97 < GC.agentList.Count; num97++)
											if (GC.agentList[num97].gang == agent23.gang)
											{
												GC.agentList[num97].gangMembers.Add(agent23);

												if (!agent23.gangMembers.Contains(GC.agentList[num97]))
													agent23.gangMembers.Add(GC.agentList[num97]);
											}
									}
								}

								if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
								{
									yield return null;
									chunkStartTime = Time.realtimeSinceStartup;
								}

								Random.InitState(__instance.randomSeedNum + numObjects);
								num2 = numObjects;
							}
						}
						#endregion
					}
					#region Assassins
					for (int num98 = 0; num98 < GC.agentList.Count; num98++)
						if (GC.agentList[num98].isPlayer > 0 && (GC.agentList[num98].statusEffects.hasStatusEffect("InDebt3") || (GC.agentList[num98].isPlayer == 1 && GC.challenges.Contains("AssassinsEveryLevel"))))
							__instance.SpawnAssassins(GC.agentList[num98], GC.agentList[num98].CalculateDebt());
					#endregion
					#endregion
				}

				if (!__instance.loadedObjectAgents)
				{
					Debug.Log("Loading Object Agents");
					List<Agent> objectAgents = new List<Agent>();
					int num2;

					for (int earlyCops = 0; earlyCops < 50; earlyCops = num2 + 1)
					{
						Agent item2 = GC.spawnerMain.SpawnAgent(Vector3.zero, null, "ObjectAgent");
						objectAgents.Add(item2);

						if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
						{
							yield return null;

							chunkStartTime = Time.realtimeSinceStartup;
						}

						Random.InitState(__instance.randomSeedNum + earlyCops);
						num2 = earlyCops;
					}

					yield return null;

					for (int earlyCops = 0; earlyCops < objectAgents.Count; earlyCops = num2 + 1)
					{
						objectAgents[earlyCops].DestroyMe();

						if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
						{
							yield return null;

							chunkStartTime = Time.realtimeSinceStartup;
						}

						Random.InitState(__instance.randomSeedNum + earlyCops);
						num2 = earlyCops;
					}

					__instance.loadedObjectAgents = true;
					objectAgents = null;
				}

				if (!GC.multiplayerMode)
				{
					List<Agent> objectAgents = new List<Agent>();
					int num99 = 10;

					if (GC.levelFeeling == "WarZone")
						num99 = 40;

					int num100 = GC.poolsScene.agentCount - GC.poolsScene.agentIterator;
					int earlyCops = num99 - num100;

					if (earlyCops > 0)
					{
						Debug.Log("Loading Recycled Agents: " + earlyCops);
						int num2;

						for (int bigTries = 0; bigTries < earlyCops; bigTries = num2 + 1)
						{
							Agent item3 = GC.spawnerMain.SpawnAgent(Vector3.zero, null, "Hobo");
							objectAgents.Add(item3);

							if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
							{
								yield return null;

								chunkStartTime = Time.realtimeSinceStartup;
							}

							Random.InitState(__instance.randomSeedNum + bigTries);
							num2 = bigTries;
						}

						yield return null;

						for (int bigTries = 0; bigTries < objectAgents.Count; bigTries = num2 + 1)
						{
							objectAgents[bigTries].DestroyMe();

							if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
							{
								yield return null;

								chunkStartTime = Time.realtimeSinceStartup;
							}

							Random.InitState(__instance.randomSeedNum + bigTries);
							num2 = bigTries;
						}
					}

					objectAgents = null;
				}
			}
			else if (!__instance.memoryTest && !GC.streamingWorld)
			{
				GC.levelFeeling = GC.playerAgent.oma.convertIntToLevelFeeling(GC.playerAgent.oma.levelFeeling);
				int num2;

				for (int earlyCops = 0; earlyCops < GC.tileInfo.simpleObjects.Count; earlyCops = num2 + 1)
				{
					GC.spawnerMain.spawnObjectReal(GC.tileInfo.simpleObjectPositions[earlyCops], null, GC.tileInfo.simpleObjects[earlyCops]);

					if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
					{
						yield return null;

						chunkStartTime = Time.realtimeSinceStartup;
					}

					Random.InitState(__instance.randomSeedNum + earlyCops);
					num2 = earlyCops;
				}
			}

			if (!__instance.loadedPoolAgents && GC.streamingWorld && GC.streamingWorldController.usePools)
			{
				Debug.Log("Loading Pool Agents");
				__instance.spawningAgentPool = true;
				List<Agent> objectAgents = new List<Agent>();
				int earlyCopCount;

				for (int earlyCops = 0; earlyCops < 149; earlyCops = earlyCopCount + 1)
				{
					Agent earlyCop = GC.spawnerMain.SpawnAgent(Vector3.zero, null, "");

					if (!GC.serverPlayer)
						earlyCop.tr.SetParent(GC.agentsNest.transform);

					objectAgents.Add(earlyCop);

					if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
					{
						yield return null;

						chunkStartTime = Time.realtimeSinceStartup;
					}

					Random.InitState(__instance.randomSeedNum + earlyCops);
					earlyCopCount = earlyCops;
				}

				yield return null;

				for (int earlyCops = 0; earlyCops < objectAgents.Count; earlyCops = earlyCopCount + 1)
				{
					objectAgents[earlyCops].DestroyMe();

					if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
					{
						yield return null;

						chunkStartTime = Time.realtimeSinceStartup;
					}

					Random.InitState(__instance.randomSeedNum + earlyCops);
					earlyCopCount = earlyCops;
				}

				__instance.loadedPoolAgents = true;
				__instance.spawningAgentPool = false;
				objectAgents = null;
			}

			if (GC.streamingWorld)
				while (GC.streamingWorldController.settingUpPools)
					yield return null;

			for (int num101 = 0; num101 < __instance.levelChunks.Count; num101++)
				__instance.levelChunks[num101].chunkPos = __instance.levelChunks[num101].transform.position;

			if (!GC.serverPlayer)
				__instance.tileInfo.SetupFloorTiles(false, null);

			GC.tileInfo.SetupOrganicTiles(false);
			GC.tileInfo.SetupOrganicTiles2(false);
			___tilemapFloors4.Build();
			___minimap.UpdateMiniMap();
			__instance.Invoke("SetupMore4", 0.1f); // base.invoke

			yield break;
		}
		public static IEnumerator LoadLevel_SetupMore3_3_Postfix(IEnumerator __result, LoadLevel __instance) // Postfix
		{
			BMLog("LoadLevel_SetupMore3_3_Postfix");

			while (__result.MoveNext())
				yield return __result.Current;

			int level = GC.sessionDataBig.curLevel;

			for (int agentSearch = 0; agentSearch < GC.agentList.Count; agentSearch++)
			{
				Agent agent = GC.agentList[agentSearch];
				
				if (agent.isPlayer > 0)
				{
					if (agent.statusEffects.hasTrait(cTrait.Haunted))
						BMAgents.SpawnRoamerSquad(agent, 4, vAgent.Ghost, __instance, vRelationship.Hostile, 1);

					if (agent.statusEffects.hasTrait(cTrait.BodyGuarded))
						BMAgents.SpawnEmployees(agent, 2, vAgent.Goon, __instance, vRelationship.Aligned);
					else if (agent.statusEffects.hasTrait(cTrait.BodyGuarded_2))
						BMAgents.SpawnEmployees(agent, 2, vAgent.Supergoon, __instance, vRelationship.Aligned);

					if (level >= 10)
						if (agent.statusEffects.hasTrait(cTrait.MobDebt))
							BMAgents.SpawnRoamerSquad(agent, (int)((float)level * 1.66f), vAgent.Mobster, __instance, vRelationship.Hostile, 4);

					if (level >= 13)
						if (GC.challenges.Contains(cChallenge.LitterallyTheWorst) || GC.challenges.Contains(cChallenge.FloralerFlora))
							for (int i = 0; i <= level - 11; i++)
								GC.spawnerMain.SpawnButlerBot();

					if (agent.statusEffects.hasTrait(cTrait.MookMasher))
						BMAgents.SpawnRoamerSquad(agent, level * 2, vAgent.Goon, __instance, vRelationship.Hostile, 4);

					if (agent.statusEffects.hasTrait(cTrait.Reinforcements))
						BMAgents.SpawnRoamerSquad(agent, 4, vAgent.ResistanceLeader, __instance, vRelationship.Aligned, 1);
					else if (agent.statusEffects.hasTrait(cTrait.Reinforcements_2))
						BMAgents.SpawnRoamerSquad(agent, 8, vAgent.ResistanceLeader, __instance, vRelationship.Aligned, 1);
				}
			}
		}
		public static void LoadLevel_SetupMore5_2(LoadLevel __instance) // Postfix
		{
			BMLog("LoadLevel_SetupMore5_2");

			BMAbilities.baseTimeScale = GameController.gameController.selectedTimeScale;
		}
		public static bool LoadLevel_SetupRels() // Prefix
		{
			BMTraits.setPlayerInitialRelationshipTraitActive();

			return true;
		}
		#endregion
		#region RandomWalls
		public void RandomWalls_00()
			{
				Prefix(typeof(RandomWalls), "fillWalls", GetType(), "RandomWalls_fillWalls", new Type[0] { });
			}
		public static bool RandomWalls_fillWalls() // Prefix
		{
			string wallType = null;

			if (IsWallModActive())
				wallType = GetWallTypeFromMutator();

			if (wallType == null)
				return true;
			else
			{
				RandomSelection component = GameObject.Find("ScriptObject").GetComponent<RandomSelection>();
				RandomList rList;

				rList = component.CreateRandomList(vWallGroup.Normal, "Walls", "Wall");
				component.CreateRandomElement(rList, wallType, 3);

				rList = component.CreateRandomList(vWallGroup.Weak, "Walls", "Wall");
				component.CreateRandomElement(rList, wallType, 3);

				rList = component.CreateRandomList(vWallGroup.Strong, "Walls", "Wall");
				component.CreateRandomElement(rList, wallType, 3);

				rList = component.CreateRandomList(vWallGroup.Hideout, "Walls", "Wall");
				component.CreateRandomElement(rList, wallType, 3);
			}

			return false;
		}
		#endregion
		#region SpawnerFloor
		public void SpawnerFloor_00()
		{
			Type t = typeof(SpawnerFloor);
			Type g = GetType();

			Prefix(t, "spawn", g, "SpawnerFloor_spawn", new Type[1] { typeof(string) });
		}
		public static bool SpawnerFloor_spawn(string floorName, SpawnerFloor __instance, ref tk2dTileMap ___tilemapFloors, ref tk2dTileMap ___tilemapFloors3, ref tk2dTileMap ___tilemapFloors4) // Replacement
		{
			// Floor Mods
			// Seems to affect only Home Base

			if (GetActiveFloorMod() == null)
				return true;

			floorName = GetFloorTile();

			if (GC.levelTheme == 2 && floorName == "FlamePit")
				floorName = "Hole";
			
			Renderer component = __instance.transform.gameObject.GetComponent<Renderer>();
			float x = component.bounds.min.x;
			float x2 = component.bounds.max.x;
			float y = component.bounds.min.y;
			float y2 = component.bounds.max.y;
			int num = 0;
			int num2 = 0;
			List<TileData> list = new List<TileData>();
			List<GameObject> list2 = new List<GameObject>();
			List<Vector2> list3 = new List<Vector2>();
			
			if (floorName == "Water" && GC.loadLevel.WaterToIce())
				floorName = "Ice";
			
			for (float num3 = x + 0.32f; num3 < x2; num3 += 0.64f)
			{
				for (float num4 = y + 0.32f; num4 < y2; num4 += 0.64f)
				{
					int num5 = 0;
					num3 = Mathf.Round(num3 * 100f) / 100f;
					num4 = Mathf.Round(num4 * 100f) / 100f;
					___tilemapFloors.GetTileAtPosition(new Vector2(num3, num4), out num, out num2);

					if (num > 159 || num2 > 159)
					{
						Debug.LogError(__instance + " (Floor) out of bounds in " + __instance.transform.parent.GetComponent<Chunk>());
						__instance.spawned = true;
						Object.Destroy(__instance.gameObject);

						return false;
					}

					floorMaterialType floorMaterialType = floorMaterialType.None;

					if (floorName == "ArenaFloor")
					{
						num5 = 712;
						__instance.tileInfo.tileArray[num, num2].arenaFloor = true;
					}
					else if (floorName == "ArmoryFloor")
					{
						num5 = 172;
						floorMaterialType = floorMaterialType.ArmoryFloor;
					}
					else if (floorName == "BankFloor")
					{
						if (GC.levelTheme == 5)
							num5 = 1356;
						else
							num5 = 172;

						floorMaterialType = floorMaterialType.BankFloor;
					}
					else if (floorName == "Bathhouse")
					{
						num5 = 824;
						floorMaterialType = floorMaterialType.Bathhouse;
					}
					else if (floorName == "BathroomTile")
					{
						num5 = 46;
						floorMaterialType = floorMaterialType.BathroomTile;
					}
					else if (floorName == "BrickIndoor")
					{
						num5 = 1376;
						floorMaterialType = floorMaterialType.BrickIndoor;
					}
					else if (floorName == "Bridge")
					{
						floorMaterialType = floorMaterialType.Bridge;
						__instance.tileInfo.tileArray[num, num2].bridge = true;
					}
					else if (floorName == "Canal")
					{
						if (!GC.canalHoles)
						{
							num5 = 1004;
							__instance.tileInfo.tileArray[num, num2].water = true;
							__instance.floorLayer = 0;
							floorMaterialType = floorMaterialType.Canal;
						}
						else
						{
							num5 = 1144;
							TileData tileData = __instance.tileInfo.tileArray[num, num2];
							tileData.hole = true;
							___tilemapFloors.SetTile(num, num2, 0, 1);
							tileData.floorMaterialOffset = num5;
							tileData.floorMaterial = floorMaterialType.Hole;
							__instance.floorLayer = 0;
						}
					}
					else if (floorName == "CasinoFloor")
					{
						num5 = 152;
						floorMaterialType = floorMaterialType.CasinoFloor;
					}
					else if (floorName == "CaveFloor")
					{
						num5 = 736;
						floorMaterialType = floorMaterialType.CaveFloor;
					}
					else if (floorName == "Checkerboard")
					{
						num5 = 66;
						floorMaterialType = floorMaterialType.Checkerboard;
					}
					else if (floorName == "Checkerboard2")
					{
						num5 = 1356;
						floorMaterialType = floorMaterialType.Checkerboard2;
					}
					else if (floorName == "CityParkFloor")
					{
						if (GC.levelTheme == 3)
							num5 = 1416;
						else if (GC.levelTheme == 5)
							num5 = 1396;
						else
							num5 = 1044;

						floorMaterialType = floorMaterialType.CityParkFloor;
					}
					else if (floorName == "CleanTiles")
					{
						num5 = 1064;
						floorMaterialType = floorMaterialType.CleanTiles;
					}
					else if (floorName == "CleanTilesRaised")
					{
						num5 = 1104;
						floorMaterialType = floorMaterialType.CleanTilesRaised;
					}
					else if (floorName == "ClearFloor")
						floorMaterialType = floorMaterialType.ClearFloor;
					else if (floorName == "ClearFloor2")
						floorMaterialType = floorMaterialType.ClearFloor;
					if (floorName == "ConveyorBelt")
					{
						if (GC.levelTheme != 1 && GC.levelTheme != 2 && (GC.levelTheme < 3 || !(__instance.transform.parent.GetComponent<Chunk>().description == "Mall")) && (!__instance.transform.parent.GetComponent<Chunk>().userCreated || !(__instance.transform.parent.GetComponent<Chunk>().description != "Hideout")))
						{
							Object.Destroy(__instance.gameObject);
							return false;
						}

						num5 = 628;
						__instance.tileInfo.tileArray[num, num2].conveyorBelt = true;
						list.Add(__instance.tileInfo.tileArray[num, num2]);
						list2.Add(Object.Instantiate<GameObject>(GC.spawnerMain.conveyorBeltImagePrefab, new Vector2((float)num * 0.64f, (float)num2 * 0.64f), Quaternion.Euler(0f, 0f, 0f)));
						__instance.floorLayer = 2;
						floorMaterialType = floorMaterialType.ConveyorBelt;
					}
					else if (floorName == "DanceFloor")
					{
						num5 = 844;
						floorMaterialType = floorMaterialType.DanceFloor;
					}
					else if (floorName == "DanceFloorRaised")
					{
						num5 = 1124;
						floorMaterialType = floorMaterialType.DanceFloorRaised;
					}
					else if (floorName == "DirtFloor")
					{
						num5 = 756;
						floorMaterialType = floorMaterialType.DirtFloor;
					}
					else if (floorName == "DirtyTiles")
					{
						num5 = 1084;
						floorMaterialType = floorMaterialType.DirtyTiles;
					}
					else if (floorName == "DrugDenFloor")
					{
						num5 = 496;
						floorMaterialType = floorMaterialType.DrugDenFloor;
					}
					else if (floorName == "ElectronicPlates")
					{
						num5 = 1184;
						floorMaterialType = floorMaterialType.ElectronicPlates;
					}
					else if (floorName == "Facility")
					{
						num5 = 1224;
						floorMaterialType = floorMaterialType.Facility;
					}
					else if (floorName == "FactoryFloor")
					{
						num5 = 476;
						floorMaterialType = floorMaterialType.FactoryFloor;
					}
					else if (floorName == "FlamePit")
					{
						if (GC.levelTheme != 1 && (!__instance.transform.parent.GetComponent<Chunk>().userCreated || !(__instance.transform.parent.GetComponent<Chunk>().description != "Hideout")))
						{
							Object.Destroy(__instance.gameObject);

							return false;
						}

						if (GC.serverPlayer)
							GC.spawnerMain.spawnObjectReal(new Vector2((float)num * 0.64f, (float)num2 * 0.64f), null, "FlameGrate").extraVar = 1;

						__instance.floorLayer = -1;
						__instance.tileInfo.tileArray[num, num2].dangerousToWalk = true;
						floorMaterialType = floorMaterialType.FlamePit;
					}
					else if (floorName == "GreyTile")
					{
						num5 = 13;
						floorMaterialType = floorMaterialType.GreyTile;
					}
					else if (floorName == "Gym")
					{
						num5 = 864;
						floorMaterialType = floorMaterialType.Gym;
					}
					else if (floorName == "HideoutFloor")
					{
						num5 = 172;

						if (GC.levelTheme == 1)
							num5 = 476;

						floorMaterialType = floorMaterialType.HideoutFloor;
					}
					else if (floorName == "Hole")
					{
						if (GC.levelTheme != 1 && GC.levelTheme != 2 && GC.levelTheme != 4 && GC.levelTheme != 5 && (!__instance.transform.parent.GetComponent<Chunk>().userCreated || !(__instance.transform.parent.GetComponent<Chunk>().description != "Hideout")))
						{
							Object.Destroy(__instance.gameObject);

							return false;
						}

						num5 = 192;
						__instance.tileInfo.tileArray[num, num2].hole = true;
						__instance.floorLayer = 2;
						floorMaterialType = floorMaterialType.Hole;
					}
					else if (floorName == "HospitalFloor")
					{
						num5 = 436;
						floorMaterialType = floorMaterialType.HospitalFloor;
					}
					else if (floorName == "Ice")
					{
						num5 = 712;
						__instance.tileInfo.tileArray[num, num2].ice = true;
						__instance.tileInfo.tileArray[num, num2].organic = true;
						__instance.floorLayer = 2;
						floorMaterialType = floorMaterialType.Ice;
					}
					else if (floorName == "IceRink")
					{
						num5 = 984;
						__instance.tileInfo.tileArray[num, num2].ice = true;
						floorMaterialType = floorMaterialType.IceRink;
					}
					else if (floorName == "Mall")
					{
						num5 = 884;
						floorMaterialType = floorMaterialType.Mall;
					}
					else if (floorName == "MetalFloor")
					{
						num5 = 86;
						floorMaterialType = floorMaterialType.MetalFloor;
					}
					else if (floorName == "MetalPlates")
					{
						num5 = 1244;
						floorMaterialType = floorMaterialType.MetalPlates;
					}
					else if (floorName == "Muted")
					{
						num5 = 1264;
						floorMaterialType = floorMaterialType.Muted;
					}
					else if (floorName == "Normal")
					{
						if (GC.levelTheme == 3 || GC.levelTheme == 5)
							num5 = 924;
						else if (GC.levelTheme == 4)
							num5 = 1184;
						else
							num5 = 110;

						floorMaterialType = floorMaterialType.Normal;
					}
					else if (floorName == "OfficeFloor")
					{
						num5 = 456;
						floorMaterialType = floorMaterialType.OfficeFloor;
					}
					else if (floorName == "Pool")
					{
						num5 = 1024;
						__instance.tileInfo.tileArray[num, num2].water = true;
						__instance.floorLayer = 2;
						floorMaterialType = floorMaterialType.Pool;
						list3.Add(new Vector2((float)num, (float)num2));
					}
					else if (floorName == "PoliceStationFloor")
					{
						num5 = 172;
						floorMaterialType = floorMaterialType.PoliceStationFloor;
					}
					else if (floorName == "Posh")
					{
						num5 = 904;
						floorMaterialType = floorMaterialType.Posh;
					}
					else if (floorName == "PrisonFloor")
					{
						num5 = 536;
						floorMaterialType = floorMaterialType.PrisonFloor;
					}
					else if (floorName == vFloor.RugBlue)
					{
						num5 = 252;
						floorMaterialType = floorMaterialType.BlueRug;
					}
					else if (floorName == vFloor.RugDarkBlue)
					{
						num5 = 272;
						floorMaterialType = floorMaterialType.DarkBlueRug;
					}
					else if (floorName == vFloor.RugGreen)
					{
						num5 = 212;
						floorMaterialType = floorMaterialType.GreenRug;
					}
					else if (floorName == vFloor.RugPurple)
					{
						num5 = 556;
						floorMaterialType = floorMaterialType.PurpleRug;
					}
					else if (floorName == vFloor.RugRed)
					{
						num5 = 232;
						floorMaterialType = floorMaterialType.RedRug;
					}
					else if (floorName == "SmallTiles")
					{
						num5 = 1164;
						floorMaterialType = floorMaterialType.SmallTiles;
					}
					else if (floorName == "SolidPlates")
					{
						num5 = 1204;
						floorMaterialType = floorMaterialType.SolidPlates;
					}
					else if (floorName == "Water")
					{
						num5 = 688;
						__instance.tileInfo.tileArray[num, num2].water = true;
						__instance.tileInfo.tileArray[num, num2].organic = true;
						__instance.floorLayer = 2;
						floorMaterialType = floorMaterialType.Water;
						list3.Add(new Vector2((float)num, (float)num2));
					}
					else if (floorName == "WoodClean")
					{
						num5 = 924;
						floorMaterialType = floorMaterialType.WoodClean;
					}
					else if (floorName == "WoodSlats")
					{
						num5 = 944;
						floorMaterialType = floorMaterialType.WoodSlats;
					}
					else if (floorName != null && floorName.Length == 0)
					{
						num5 = 0;
						floorMaterialType = floorMaterialType.None;
					}

					if (floorName == "ClearFloor" || floorName == "ArenaFloor")
					{
						if (__instance.ownedByID != 55)
							__instance.ownedByID = 88;
						
						__instance.SetExtraFloorParams(num, num2, floorMaterialType);
					}
					else if (floorName == "ClearFloor2")
						__instance.SetExtraFloorParams(num, num2, floorMaterialType);
					else if (!(floorName == "Bridge"))
					{
						if (__instance.floorLayer == 0)
						{
							___tilemapFloors.SetTile(num, num2, 0, 1);
							__instance.tileInfo.tileArray[num, num2].floorMaterialOffset = num5;
							__instance.SetExtraFloorParams(num, num2, floorMaterialType);
						}
						else if (__instance.floorLayer == 1)
						{
							___tilemapFloors3.SetTile(num, num2, 0, 1);
							__instance.tileInfo.tileArray[num, num2].floorMaterialOffset3 = num5;
							__instance.tileInfo.tileArray[num, num2].floorMaterial3 = floorMaterialType;

							if (__instance.tileInfo.tileArray[num, num2].prison == 0)
								__instance.tileInfo.tileArray[num, num2].prison = __instance.prison;
						}
						else if (__instance.floorLayer == 2)
						{
							___tilemapFloors4.SetTile(num, num2, 0, 1);
							__instance.tileInfo.tileArray[num, num2].floorMaterialOffset4 = num5;
							__instance.tileInfo.tileArray[num, num2].floorMaterial4 = floorMaterialType;

							if (__instance.tileInfo.tileArray[num, num2].prison == 0)
								__instance.tileInfo.tileArray[num, num2].prison = __instance.prison;
						}
					}
				}
			}

			__instance.spawned = true;

			if (floorName == "Hole" || (floorName == "Canal" && GC.canalHoles))
				GC.spawnerMain.StartCoroutine(GC.spawnerMain.SpawnHoleLate(null, __instance.transform.position, __instance.transform.localScale, __instance.transform.rotation, true, false));
			else if (floorName == "Water" || floorName == "Canal" || floorName == "Pool")
			{
				Chunk component2 = __instance.transform.parent.GetComponent<Chunk>();

				if (component2 == null)
					component2 = __instance.transform.parent.transform.parent.GetComponent<Chunk>();

				Water water = GC.spawnerMain.SpawnWater(null, __instance.transform.position, __instance.transform.localScale, __instance.transform.rotation, true, component2);

				for (int i = 0; i < list3.Count; i++)
				{
					__instance.tileInfo.tileArray[(int)list3[i].x, (int)list3[i].y].waterReal = water;
					__instance.tileInfo.tileArray[(int)list3[i].x, (int)list3[i].y].waterRealID = water.UID;
				}
			}
			else if (!(floorName == "Ice") && !(floorName == "Bridge") && !(floorName == "CityParkFloor"))
			{
				if (floorName == "ConveyorBelt")
				{
					Chunk component3 = __instance.transform.parent.GetComponent<Chunk>();

					if (component3 == null)
						component3 = __instance.transform.parent.transform.parent.GetComponent<Chunk>();
				
					string text = __instance.direction;
					
					if (component3.chunkDirX == "Reverse" && !__instance.skipRotations)
					{
						if (text == "E")
							text = "W";
						else if (text == "W")
							text = "E";
					}

					if (component3.chunkDirY == "Reverse" && !__instance.skipRotations)
					{
						if (text == "N")
							text = "S";
						else if (text == "S")
							text = "N";
					}

					if (component3.chunkRotate == 1 && !__instance.skipRotations)
					{
						if (text == "E")
							text = "N";
						else if (text == "N")
							text = "W";
						else if (text == "S")
							text = "E";
						else if (text == "W")
							text = "S";
					}

					GameObject gameObject = GC.spawnerMain.SpawnConveyorBelt(null, __instance.transform.position, __instance.transform.localScale, __instance.transform.rotation, text, true, component3).gameObject;
					ConveyorBelt component4 = gameObject.GetComponent<ConveyorBelt>();

					for (int j = 0; j < list.Count; j++)
					{
						list[j].direction = text;
						list2[j].transform.SetParent(gameObject.transform);
						component4.imageList.Add(list2[j]);
					}
				}
				else
				{
					if (__instance.ownerID != 0 && __instance.ownedByID == 0)
						__instance.ownedByID = __instance.ownerID;
					else if (__instance.ownedByID == 0)
						__instance.ownedByID = 1;
					
					if (__instance.ownedByID != 0)
					{
						Vector3 position = new Vector3(__instance.transform.position.x, __instance.transform.position.y, -0.7f);
						GameObject gameObject2 = Object.Instantiate<GameObject>(GC.spawnerMain.ownerPropertyColliderPrefab, position, Quaternion.Euler(0f, 0f, 0f));
						BoxCollider2D component5 = gameObject2.GetComponent<BoxCollider2D>();
						
						if (__instance.transform.rotation.z > 0f)
							component5.size = new Vector2(__instance.transform.localScale.y * 0.64f, __instance.transform.localScale.x * 0.64f);
						else
							component5.size = new Vector2(__instance.transform.localScale.x * 0.64f, __instance.transform.localScale.y * 0.64f);
						
						gameObject2.transform.SetParent(GC.floorPenaltiesNest.transform);
						GC.ownerPropertyList.Add(gameObject2);
					}
				}
			}

			Object.Destroy(__instance.gameObject);

			return false;
		}
		#endregion
		#region SpawnerMain
		public void SpawnerMain_00()
		{
			Type t = typeof(SpawnerMain);
			Type g = GetType();

			Prefix(t, "GetLightColor", g, "SpawnerMain_GetLightColor", new Type[1] { typeof(string) });
		}
		#region Light Colors
		// https://colordesigner.io/color-mixer
		static Color32 arenaRingColor = new Color32(167, 76, 134, 200);
		static Color32 blueColor = new Color32(62, 62, 255, 200);
		static Color32 cyanColor = new Color32(0, 113, 159, 200);
		static Color32 cyanGreenColor = new Color32(0, 180, 143, 200);
		static Color32 defaultColor = new Color32(161, 161, 161, 105);
		static Color32 discoBlueColor = new Color32(64, 64, 255, 200);
		static Color32 discoGreenColor = new Color32(85, 170, 0, 200);
		static Color32 discoOrangeColor = new Color32(255, 188, 64, 200);
		static Color32 discoPurpleColor = new Color32(140, 52, 173, 200);
		static Color32 discoRedColor = new Color32(255, 85, 85, 200);
		static Color32 discoYellowColor = new Color32(255, 255, 85, 200);
		static Color32 fireStationColor = new Color32(125, 87, 248, 111);
		static Color32 greenColor = new Color32(0, 159, 60, 200);
		static Color32 homeColor = new Color32(199, 174, 120, 160);
		static Color32 homeColorMayorVillage = new Color32(212, 122, 244, 160);
		static Color32 homeColorUptown = new Color32(205, 173, 219, 85);
		static Color32 labColor = new Color32(64, 224, 255, 180);
		static Color32 lakeColor = new Color32(0, 213, 255, 85);
		static Color32 lightBlueColor = new Color32(124, 151, 189, 180);
		static Color32 lightBlueColorMayorVillage = new Color32(44, 106, 193, 180);
		static Color32 mallColor = new Color32(255, 255, 255, 80);
		static Color32 pinkColor = new Color32(159, 0, 148, 200);
		static Color32 pinkWhiteColor = new Color32(208, 163, 255, 120);
		static Color32 poolColor = new Color32(0, 213, 255, 85);
		static Color32 poolColorLighter = new Color32(144, 237, 255, 85);
		static Color32 privateClubColor = new Color32(163, 178, 110, 160);
		static Color32 purpleColor = new Color32(111, 0, 159, 200);
		static Color32 redColor = new Color32(159, 0, 0, 200);
		static Color32 whiteColor = new Color32(255, 255, 255, 120);
		static Color32 zooColor = new Color32(0, 255, 181, 85);
		static List<Color32> discoColors = new List<Color32>()
		{
			discoBlueColor,
			discoGreenColor,
			discoOrangeColor,
			discoPurpleColor,
			discoRedColor,
			discoYellowColor,
		};
		#endregion	
		public static bool SpawnerMain_GetLightColor(string lightRealName, SpawnerMain __instance, ref Color __result, Color32 ___defaultColor) // Prefix
		{
			string challenge = BMChallenges.GetActiveChallengeFromList(cChallenge.AffectsLights);

			LightReal lightReal = new LightReal();

			if (challenge == null)
				return true;
			else if (challenge == cChallenge.DiscoCityDanceoff)
				lightReal.lightReal2Color = discoColors.RandomElement<Color32>();
			else if (challenge == cChallenge.GreenLiving)
				lightReal.lightReal2Color = homeColor;
			else if (challenge == cChallenge.Panoptikopolis)
				lightReal.lightReal2Color = whiteColor;

			__result = lightReal.lightReal2Color;
			return false;

			#region Vanilla
			if (lightRealName == "ArenaRingLight")
				lightReal.lightReal2Color = arenaRingColor;
			else if (lightRealName == "BankLight")
				lightReal.lightReal2Color = whiteColor;
			else if (lightRealName == "BlueLight")
				lightReal.lightReal2Color = blueColor;
			else if (lightRealName == "CyanGreenLight")
				lightReal.lightReal2Color = cyanGreenColor;
			else if (lightRealName == "CyanLight")
				lightReal.lightReal2Color = cyanColor;
			else  if (lightRealName == "DefaultLight")
				lightReal.lightReal2Color = defaultColor;
			else if (lightRealName == "FarmLight")
				lightReal.lightReal2Color = homeColor;
			else if (lightRealName == "FireStationLight")
				lightReal.lightReal2Color = fireStationColor;
			else if (lightRealName == "GraveyardLight")
				lightReal.lightReal2Color = cyanColor;
			if (lightRealName == "GreenLight")
				lightReal.lightReal2Color = greenColor;
			else if (lightRealName == "HomeLight")
			{
				if (GC.levelTheme == 4)
					lightReal.lightReal2Color = homeColorUptown;
				else if (GC.levelTheme == 5)
					lightReal.lightReal2Color = homeColorMayorVillage;
				else
					lightReal.lightReal2Color = homeColor;
			}
			else if (lightRealName == "HospitalLight")
			{
				if (GC.levelTheme == 5)
					lightReal.lightReal2Color = homeColorMayorVillage;
				else
					lightReal.lightReal2Color = whiteColor;
			}
			else if (lightRealName == "KitchenLight")
				lightReal.lightReal2Color = whiteColor;
			if (lightRealName == "LabLight")
				lightReal.lightReal2Color = labColor;
			else if (lightRealName == "LakeLight")
				lightReal.lightReal2Color = lakeColor;
			else if (lightRealName == "LightBlueLight")
			{
				if (GC.levelTheme == 5)
					lightReal.lightReal2Color = lightBlueColorMayorVillage;
				else
					lightReal.lightReal2Color = lightBlueColor;
			}
			else if (lightRealName == "MallLight")
				lightReal.lightReal2Color = mallColor;
			else if (lightRealName == "OfficeLight")
				lightReal.lightReal2Color = whiteColor;
			else if (lightRealName == "PinkLight")
				lightReal.lightReal2Color = pinkColor;
			if (lightRealName == "PinkWhiteLight")
				lightReal.lightReal2Color = pinkWhiteColor;
			else if (lightRealName == "PoolLight")
			{
				if (GC.levelTheme == 5)
					lightReal.lightReal2Color = poolColorLighter;
				else
					lightReal.lightReal2Color = poolColor;
			}
			else if (lightRealName == "PrivateClubLight")
				lightReal.lightReal2Color = privateClubColor;
			else if (lightRealName == "PurpleLight")
				lightReal.lightReal2Color = purpleColor;
			if (lightRealName == "RedLight")
				lightReal.lightReal2Color = redColor;
			else if (lightRealName == "TVStationLight")
			{
				lightReal.lightReal2Color = mallColor;
			}
			else if (lightRealName == "WhiteLight")
				lightReal.lightReal2Color = whiteColor;
			else if (lightRealName == "ZooLight")
				lightReal.lightReal2Color = zooColor;
			#endregion

			__result = lightReal.lightReal2Color;

			return false;
		}
		#endregion
		#region SpawnerObject
		public void SpawnerObject_00()
		{
			Prefix(typeof(SpawnerObject), "spawn", GetType(), "SpawnerObject_spawn", new Type[1] { typeof(string) });
		}
		public static bool SpawnerObject_spawn(ref string objectRealName) // Prefix
		{
			BMLog("SpawnerObject_spawn:");
			BMLog("\tobjectRealName = '" + objectRealName + "'");

			if (BMChallenges.IsChallengeFromListActive(cChallenge.WallsFlammable) && objectRealName == vObject.FireSpewer)
				objectRealName = vObject.SecurityCam;

			return true;
		}
		#endregion
	}
}