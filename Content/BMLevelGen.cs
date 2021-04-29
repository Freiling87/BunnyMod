using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RogueLibsCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;
using HarmonyLib;
using System.Reflection;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;
using Light2D;
using UnityEngine;
using System.Runtime;
using System.Runtime.CompilerServices;

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
			LoadLevel_00();
			RandomWalls_00();
			SpawnerFloor_00();
			SpawnerObject_00();
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
			if (GC.challenges.Contains(cChallenge.LetMeSeeThatThrong))
				vanilla *= 4;
			else if (GC.challenges.Contains(cChallenge.SwarmWelcome))
				vanilla *= 8;

			return vanilla;
		}
		public static string GetWallMutator()
		{
			foreach (string mutator in GC.challenges)
				if (cChallenge.Walls.Contains(mutator))
					return mutator;

			return null;
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

		#region LoadLevel
		public void LoadLevel_00()
		{
			Type t = typeof(LoadLevel);
			Type g = GetType();

			Prefix(t, "CreateInitialMap", g, "LoadLevel_CreateInitialMap", new Type[0] { });
			Prefix(t, "loadStuff2", g, "LoadLevel_loadStuff2_Prefix", new Type[0] { });
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
		// loadStuff2 [sic]
		public static bool LoadLevel_loadStuff2_Prefix (LoadLevel __instance, ref IEnumerator __result, ref List<ChunkData> ___chunkListBasic, ref tk2dTileMap ___tilemapFloors, ref tk2dTileMap ___tilemapFloors2, ref tk2dTileMap ___tilemapWalls, ref bool ___placedKey1, ref bool ___placedKey2, ref bool ___placedKey3, ref List<GameObject> ___chunkList, ref List <ChunkData> ___chunkListBasicBackup) // Prefix
		{
			BMLog("LoadLevel_LoadStuff2_Prefix");

			if (GC.challenges.Contains(cChallenge.SkywayDistrict))
				GC.canalHoles = true;
			else
				GC.canalHoles = false;

			return false;
		}
		// SetupMore3_3
		public static bool LoadLevel_SetupMore3_3_Prefix(LoadLevel __instance, ref tk2dTileMap ___tilemapFloors4, ref Minimap ___minimap, ref IEnumerator __result) // Prefix
		{
			BMLog("LoadLevel_SetupMore3_3_Prefix");

			// Structure advised by Abbysssal for patch-replacing IEnumerators.
			__result = LoadLevel_SetupMore3_3_Prefix_Replacement(__instance, ___tilemapFloors4, ___minimap);

			return false;
		}
		public static IEnumerator LoadLevel_SetupMore3_3_Prefix_Replacement(LoadLevel __instance, tk2dTileMap ___tilemapFloors4, Minimap ___minimap) // Non-Patch
		{
			// Level Features
			// Level Roamers

			BMLog("LoadLevel_SetupMore3_3_Prefix_Replacement");

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

					if (GC.levelTheme == 2 || (GC.challenges.Contains("MixedUpLevels") && GC.percentChance(33) && GC.levelTheme != 3))
						__instance.hasLakes = true;

					if (GC.customLevel)
						__instance.hasLakes = __instance.customLevel.levelFeatures.Contains("Lake");

					if (GC.challenges.Contains(cChallenge.ArcologyEcology))
						__instance.hasLakes = true;

					if (__instance.hasLakes)
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
							for (int k = 0; k < __instance.levelChunks.Count; k++)
								if (__instance.levelChunks[k].description == "Hideout" || __instance.levelChunks[k].description == "Factory")
								{
									findingFactoryLake = true;
									factoryChunk = __instance.levelChunks[k];
								}

						int num2;

						for (int i = 0; i < numObjects; i = num2 + 1)
						{
							Vector2 vector = Vector2.zero;
							int num = 0;

							do
							{
								vector = GC.tileInfo.FindRandLocationGeneral(0.64f);

								if (findingFactoryLake && (((vector.x <= factoryChunk.chunkEdgeE || vector.x >= factoryChunk.chunkEdgeE + __instance.chunkSize) && (vector.x >= factoryChunk.chunkEdgeW || vector.x <= factoryChunk.chunkEdgeW - __instance.chunkSize)) || vector.y <= factoryChunk.chunkEdgeS - __instance.chunkSize || vector.y >= factoryChunk.chunkEdgeN + __instance.chunkSize) && (((vector.y <= factoryChunk.chunkEdgeN || vector.y >= factoryChunk.chunkEdgeN + __instance.chunkSize) && (vector.y >= factoryChunk.chunkEdgeS || vector.y <= factoryChunk.chunkEdgeS - __instance.chunkSize)) || vector.x <= factoryChunk.chunkEdgeW - __instance.chunkSize || vector.x >= factoryChunk.chunkEdgeE + __instance.chunkSize))
									vector = Vector2.zero;

								num++;
								TileData tileData = GC.tileInfo.GetTileData(vector);

								if (tileData.chunkID == GC.startingPoint.startingChunk)
									vector = Vector2.zero;

								if (GC.exitPoint != null && tileData.chunkID == GC.exitPoint.startingChunk)
									vector = Vector2.zero;

								for (int l = -4; l < 5; l++)
									for (int m = -4; m < 5; m++)
										if (GC.tileInfo.GetTileDataInt(tileData.posX + l, tileData.posY + m).wallMaterial != wallMaterialType.None)
										{
											vector = Vector2.zero;

											break;
										}
							}
							while ((vector == Vector2.zero || Vector2.Distance(vector, GC.playerAgent.tr.position) < 8f) && num < 100);

							if (vector != Vector2.zero)
							{
								GC.tileInfo.lakeN = 0f;
								GC.tileInfo.lakeS = 9999f;
								GC.tileInfo.lakeE = 0f;
								GC.tileInfo.lakeW = 9999f;
								Random.InitState(__instance.randomSeedNum + i + randomCount);
								int expandLimit = Random.Range(7, 12);
								GC.tileInfo.CreateLake(vector, 0, expandLimit, lakeType);
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
								}

								if (GC.lightingType == "Full" || GC.lightingType == "Med")
								{
									LightReal lightReal = GC.spawnerMain.SpawnLightReal(vector2, null, 2);
									lightReal.name = "LakeLight";
									lightReal.tr.localScale = new Vector3(8f, 8f, 1f);
									lightReal.GetComponent<LightSprite>().Color = new Color32(0, 213, byte.MaxValue, 85);
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

						lakeList = null;
						lakeType = null;
						factoryChunk = null;
					}
					#endregion
					#region Power Boxes
					int earlyCops = 0;
					bool hasPowerBoxes = false;
					bool hasExtraPowerBoxes = false;

					if ((GC.sessionDataBig.curLevel >= 2 || GC.levelTheme != 0) && GC.levelTheme != 2)
						hasPowerBoxes = true;

					if (GC.levelTheme == 3 && GC.debugMode)
						hasPowerBoxes = true;

					if (GC.customLevel)
						hasPowerBoxes = __instance.customLevel.levelFeatures.Contains("PowerBox");

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

						int num2;

						for (int numObjects = 0; numObjects < bigTries; numObjects = num2 + 1)
						{
							Vector2 vector4 = Vector2.zero;
							int num15 = 0;

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

								num15++;
							}
							while ((vector4 == Vector2.zero || Vector2.Distance(vector4, GC.playerAgent.tr.position) < 5f || Vector2.Distance(vector4, GC.elevatorDown.tr.position) < 20f) && num15 < 100);

							if (vector4 != Vector2.zero)
								GC.spawnerMain.spawnObjectReal(vector4, null, "PowerBox");

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
					#region Slime Barrels
					bool hasSlimeBarrels = false;

					if (GC.levelTheme == 2 || (GC.challenges.Contains("MixedUpLevels") && GC.percentChance(33)))
						hasSlimeBarrels = true;

					if (GC.customLevel)
						hasSlimeBarrels = __instance.customLevel.levelFeatures.Contains("SlimeBarrel");

					if (GC.challenges.Contains(cChallenge.ArcologyEcology) || GC.challenges.Contains(cChallenge.PoliceState) || GC.challenges.Contains(cChallenge.MACITS))
						hasSlimeBarrels = false;
					else if (GC.challenges.Contains(cChallenge.AnCapistan))
						hasSlimeBarrels = true;

					if (hasSlimeBarrels)
					{
						Debug.Log("Loading Slime Barrels");
						int bigTries = Random.Range(2, 6);
						Water myLake = null;

						for (int num17 = 0; num17 < GC.watersList.Count; num17++)
							if (GC.watersList[num17].factory != null)
								myLake = GC.watersList[num17];

						if (myLake != null)
						{
							int num2;

							for (int numObjects = 0; numObjects < bigTries; numObjects = num2 + 1)
							{
								Vector2 vector5 = Vector2.zero;
								int num18 = 0;

								do
								{
									vector5 = GC.tileInfo.FindRandLocationGeneral(0.64f);
									num18++;
								}
								while ((vector5 == Vector2.zero || Vector2.Distance(vector5, GC.playerAgent.tr.position) < 5f || Vector2.Distance(vector5, myLake.tr.position) > 7f) && num18 < 300);

								if (vector5 != Vector2.zero)
									GC.spawnerMain.spawnObjectReal(vector5, null, "SlimeBarrel");

								if (Time.realtimeSinceStartup - chunkStartTime > maxChunkTime)
								{
									yield return null;
									chunkStartTime = Time.realtimeSinceStartup;
								}

								Random.InitState(__instance.randomSeedNum + numObjects);
								num2 = numObjects;
							}
						}
						myLake = null;
					}
					#endregion
					#region Oil Spills
					bool hasOilSpills = false;

					if (GC.levelTheme == 1 || (GC.challenges.Contains("MixedUpLevels") && GC.percentChance(33)))
						hasOilSpills = true;

					if (GC.customLevel)
						hasOilSpills = __instance.customLevel.levelFeatures.Contains("OilSpill");

					if (GC.challenges.Contains(cChallenge.ArcologyEcology) || GC.challenges.Contains(cChallenge.PoliceState) || GC.challenges.Contains(cChallenge.MACITS))
						hasOilSpills = false;
					else if (GC.challenges.Contains(cChallenge.AnCapistan))
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
					bool hasVendingMachines = true;

					if (GC.customLevel)
						hasVendingMachines = __instance.customLevel.levelFeatures.Contains("VendingMachine");

					if (GC.challenges.Contains(cChallenge.AnCapistan))
						hasVendingMachines = true;

					if (hasVendingMachines)
					{
						Debug.Log("Loading Vending Machines");
						Random.InitState(GC.loadLevel.randomSeedNum + GC.sessionDataBig.curLevelEndless + 12);
						List<string> list2 = new List<string>();
						list2.Add("PawnShopMachine");
						list2.Add("AmmoDispenser");
						list2.Add("CloneMachine");
						list2.Add("CapsuleMachine");
						list2.Add("ATMMachine");
						list2.Add("AugmentationBooth");

						if (!GC.arcadeMode)
							list2.Add("LoadoutMachine");

						for (int num19 = 0; num19 < GC.objectRealList.Count; num19++)
						{
							string objectName = GC.objectRealList[num19].objectName;

							if (objectName == "PawnShopMachine" || objectName == "AmmoDispenser" || objectName == "CloneMachine" || objectName == "CapsuleMachine" || objectName == "LoadoutMachine" || objectName == "ATMMachine" || objectName == "AugmentationBooth")
								list2.Remove(objectName);
						}

						int count = list2.Count;
						List<int> list3 = new List<int>();

						for (int num20 = 0; num20 < count; num20++)
						{
							Vector2 vector6 = Vector2.zero;
							int num21 = 0;

							do
							{
								vector6 = GC.tileInfo.FindRandLocationNearWall(0.64f);

								if (vector6 != Vector2.zero)
								{
									TileData tileData2 = GC.tileInfo.GetTileData(vector6);

									if (GC.tileInfo.GetTileData(new Vector2(vector6.x, vector6.y + 0.64f)).owner == 0 && GC.tileInfo.GetTileData(new Vector2(vector6.x + 0.64f, vector6.y)).owner == 0 && GC.tileInfo.GetTileData(new Vector2(vector6.x, vector6.y - 0.64f)).owner == 0 && GC.tileInfo.GetTileData(new Vector2(vector6.x - 0.64f, vector6.y)).owner == 0)
										vector6 = Vector2.zero;

									if (!GC.tileInfo.IsOverlapping(new Vector2(vector6.x, vector6.y + 0.64f), "Wall") && !GC.tileInfo.IsOverlapping(new Vector2(vector6.x, vector6.y - 0.64f), "Wall") && !GC.tileInfo.IsOverlapping(new Vector2(vector6.x + 0.64f, vector6.y), "Wall") && !GC.tileInfo.IsOverlapping(new Vector2(vector6.x - 0.64f, vector6.y), "Wall"))
										vector6 = Vector2.zero;

									if (GC.tileInfo.IsOverlapping(vector6, "ObjectRealSprite", 0.64f))
										vector6 = Vector2.zero;

									if (list3.Contains(tileData2.chunkID))
										vector6 = Vector2.zero;

									if (GC.tileInfo.DestroyIfBetweenWalls(vector6))
										vector6 = Vector2.zero;
								}

								num21++;
							}
							while ((vector6 == Vector2.zero || Vector2.Distance(vector6, GC.playerAgent.tr.position) < 2f) && num21 < 100);

							if (vector6 != Vector2.zero)
							{
								Random.InitState(__instance.randomSeedNum + 1);
								string text = list2[Random.Range(0, list2.Count)];
								ObjectReal objectReal = GC.spawnerMain.spawnObjectReal(vector6, null, text);
								list2.Remove(text);
								bool flag5 = false;

								if (GC.tileInfo.GetTileData(new Vector2(vector6.x, vector6.y + 0.64f)).owner != 0 && GC.tileInfo.GetTileData(new Vector2(vector6.x, vector6.y + 0.64f)).wallMaterial != wallMaterialType.None)
								{
									objectReal.direction = "S";
									flag5 = true;
								}
								else if (GC.tileInfo.GetTileData(new Vector2(vector6.x, vector6.y - 0.64f)).owner != 0 && GC.tileInfo.GetTileData(new Vector2(vector6.x, vector6.y - 0.64f)).wallMaterial != wallMaterialType.None)
								{
									objectReal.direction = "N";
									flag5 = true;
								}
								else if (GC.tileInfo.GetTileData(new Vector2(vector6.x + 0.64f, vector6.y)).owner != 0 && GC.tileInfo.GetTileData(new Vector2(vector6.x + 0.64f, vector6.y)).wallMaterial != wallMaterialType.None)
								{
									objectReal.direction = "W";
									flag5 = true;
								}
								else if (GC.tileInfo.GetTileData(new Vector2(vector6.x - 0.64f, vector6.y)).owner != 0 && GC.tileInfo.GetTileData(new Vector2(vector6.x - 0.64f, vector6.y)).wallMaterial != wallMaterialType.None)
								{
									objectReal.direction = "E";
									flag5 = true;
								}

								if (!flag5)
								{
									if (GC.tileInfo.IsOverlapping(new Vector2(vector6.x, vector6.y + 0.64f), "Wall"))
										objectReal.direction = "S";
									else if (GC.tileInfo.IsOverlapping(new Vector2(vector6.x, vector6.y - 0.64f), "Wall"))
										objectReal.direction = "N";
									else if (GC.tileInfo.IsOverlapping(new Vector2(vector6.x + 0.64f, vector6.y), "Wall"))
										objectReal.direction = "W";
									else if (GC.tileInfo.IsOverlapping(new Vector2(vector6.x - 0.64f, vector6.y), "Wall"))
										objectReal.direction = "E";
								}

								string text2 = objectReal.direction;

								if (text2 == "S" || !objectReal.fourDirection)
									text2 = "";

								objectReal.ChangeSprite(text + text2);
								objectReal.startingObjectSprite = text + text2;
								objectReal.ShiftTowardWalls(objectReal.direction);
								TileData tileData3 = GC.tileInfo.GetTileData(vector6);
								list3.Add(tileData3.chunkID);

								if (num20 < count - 1 && list2.Count != 0)
								{
									string a = "";
									Vector2 zero2 = Vector2.zero;
									Vector2 zero3 = Vector2.zero;

									if (GC.tileInfo.GetTileData(new Vector2(vector6.x, vector6.y + 0.64f)).wallMaterial != wallMaterialType.None)
									{
										zero2 = new Vector2(vector6.x + 1.28f, vector6.y);
										zero3 = new Vector2(vector6.x - 1.28f, vector6.y);
										a = "N";
									}
									else if (GC.tileInfo.GetTileData(new Vector2(vector6.x, vector6.y - 0.64f)).wallMaterial != wallMaterialType.None)
									{
										zero2 = new Vector2(vector6.x + 1.28f, vector6.y);
										zero3 = new Vector2(vector6.x - 1.28f, vector6.y);
										a = "S";
									}
									else if (GC.tileInfo.GetTileData(new Vector2(vector6.x + 0.64f, vector6.y)).wallMaterial != wallMaterialType.None)
									{
										zero2 = new Vector2(vector6.x, vector6.y + 1.28f);
										zero3 = new Vector2(vector6.x, vector6.y - 1.28f);
										a = "E";
									}
									else if (GC.tileInfo.GetTileData(new Vector2(vector6.x - 0.64f, vector6.y)).wallMaterial != wallMaterialType.None)
									{
										zero2 = new Vector2(vector6.x, vector6.y + 1.28f);
										zero3 = new Vector2(vector6.x, vector6.y - 1.28f);
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
										text = list2[Random.Range(0, list2.Count)];
										ObjectReal objectReal2 = GC.spawnerMain.spawnObjectReal(zero2, null, text);
										list2.Remove(text);
										objectReal2.direction = objectReal.direction;
										text2 = objectReal2.direction;

										if (text2 == "S" || !objectReal2.fourDirection)
											text2 = "";

										objectReal2.ChangeSprite(text + text2);
										objectReal2.startingObjectSprite = text + text2;
										objectReal2.ShiftTowardWalls(objectReal2.direction);
										num20++;
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
											text = list2[Random.Range(0, list2.Count)];
											ObjectReal objectReal3 = GC.spawnerMain.spawnObjectReal(zero3, null, text);
											list2.Remove(text);
											objectReal3.direction = objectReal.direction;
											text2 = objectReal3.direction;

											if (text2 == "S" || !objectReal3.fourDirection)
												text2 = "";

											objectReal3.ChangeSprite(text + text2);
											objectReal3.startingObjectSprite = text + text2;
											objectReal3.ShiftTowardWalls(objectReal3.direction);
											num20++;
										}
									}
								}
							}
						}
					}
					#endregion
					#region Trash Cans
					bool hasTrashCans = true;

					if (GC.customLevel)
						hasTrashCans = __instance.customLevel.levelFeatures.Contains("TrashCan");

					if (GC.challenges.Contains(cChallenge.ArcologyEcology) || GC.challenges.Contains(cChallenge.PoliceState) || GC.challenges.Contains(cChallenge.MACITS))
						hasTrashCans = true;
					else if (GC.challenges.Contains(cChallenge.AnCapistan))
						hasTrashCans = false;

					if (hasTrashCans)
					{
						Debug.Log("Loading Trash Cans");
						int bigTries = (int)((float)Random.Range(6, 12) * __instance.levelSizeModifier);
						List<int> spawnedInChunks = new List<int>();
						int num2;

						for (int numObjects = 0; numObjects < bigTries; numObjects = num2 + 1)
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
								GC.spawnerMain.spawnObjectReal(vector7, null, "TrashCan").ShiftTowardWalls();
								TileData tileData5 = GC.tileInfo.GetTileData(vector7);
								spawnedInChunks.Add(tileData5.chunkID);
								bool flag8 = true;
								Random.InitState(__instance.randomSeedNum + numObjects + ++randomCount);

								if (numObjects < bigTries - 1 && GC.percentChance(25) && flag8)
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
										GC.spawnerMain.spawnObjectReal(zero4, null, "TrashCan").ShiftTowardWalls();
										num2 = numObjects;
										numObjects = num2 + 1;
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

						spawnedInChunks = null;
					}
					#endregion
					#region Cave wall outcroppings for SpelunkyDory
					if (GC.challenges.Contains(cChallenge.SpelunkyDory))
					{
						Debug.Log("Loading SpelunkyDory Cave Wall Outcroppings");
						int bigTries = (int)((float)Random.Range(24, 24) * __instance.levelSizeModifier);
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
								GC.spawnerMain.spawnObjectReal(spotCandidate, null, vObject.Altar).ShiftTowardWalls(); // Using Altars to test this.
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
										GC.spawnerMain.spawnObjectReal(leftOfSpot, null, vObject.Altar).ShiftTowardWalls(); // Using Altars to test this.
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
					#region Public Security Cams for Police State
					if (GC.challenges.Contains(cChallenge.PoliceState))
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
										BMLog("PoliceState Security Cams: This one actually isn't useless // 202104261119");
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
								GC.spawnerMain.spawnObjectReal(spotCandidate, null, vObject.SecurityCam).ShiftTowardWalls();
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
									}
									else if (GC.tileInfo.GetTileData(new Vector2(spotCandidate.x, spotCandidate.y - 0.64f)).wallMaterial != wallMaterialType.None)
									{
										leftOfSpot = new Vector2(spotCandidate.x + 1.28f, spotCandidate.y);
										rightOfSpot = new Vector2(spotCandidate.x - 1.28f, spotCandidate.y);
										wallEdge = "S";
									}
									else if (GC.tileInfo.GetTileData(new Vector2(spotCandidate.x + 0.64f, spotCandidate.y)).wallMaterial != wallMaterialType.None)
									{
										leftOfSpot = new Vector2(spotCandidate.x, spotCandidate.y + 1.28f);
										rightOfSpot = new Vector2(spotCandidate.x, spotCandidate.y - 1.28f);
										wallEdge = "E";
									}
									else if (GC.tileInfo.GetTileData(new Vector2(spotCandidate.x - 0.64f, spotCandidate.y)).wallMaterial != wallMaterialType.None)
									{
										leftOfSpot = new Vector2(spotCandidate.x, spotCandidate.y + 1.28f);
										rightOfSpot = new Vector2(spotCandidate.x, spotCandidate.y - 1.28f);
										wallEdge = "W";
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
										GC.spawnerMain.spawnObjectReal(leftOfSpot, null, vObject.SecurityCam).ShiftTowardWalls();
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
											GC.spawnerMain.spawnObjectReal(rightOfSpot, null, vObject.Turret).ShiftTowardWalls();
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

						spawnedInChunks = null;
					}
					#endregion
					#region Vendor Carts
					bool hasVendorCarts = false;

					if (GC.levelTheme == 2)
						hasVendorCarts = true;

					if (GC.levelFeeling == "HarmAtIntervals" || GC.levelFeeling == "Lockdown" || GC.levelFeeling == "WarZone")
						hasVendorCarts = false;

					if (GC.customLevel)
						hasVendorCarts = __instance.customLevel.levelFeatures.Contains("VendorCart");

					if (GC.challenges.Contains(cChallenge.AnCapistan))
						hasVendorCarts = true;

					if (hasVendorCarts)
					{
						Debug.Log("Loading Vendor Carts");
						int bigTries = 2;
						int num2;

						for (int numObjects = 0; numObjects < bigTries; numObjects = num2 + 1)
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

									if (numObjects == 1)
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

							Random.InitState(__instance.randomSeedNum + numObjects);
							num2 = numObjects;
						}
					}
					#endregion
					#region PoliceBoxes
					bool hasPoliceBoxes = false;

					if (GC.levelTheme == 3 || GC.levelTheme == 4 || __instance.hasLockdownWalls || (GC.challenges.Contains("MixedUpLevels") && GC.percentChance(20)))
						hasPoliceBoxes = true;

					if (GC.challenges.Contains(cChallenge.PoliceState) || GC.challenges.Contains(cChallenge.MACITS))
						hasPoliceBoxes = true;
					else if (GC.challenges.Contains(cChallenge.AnCapistan))
						hasPoliceBoxes = false;

					if (GC.customLevel)
						hasPoliceBoxes = __instance.customLevel.levelFeatures.Contains("PoliceBox");

					if (hasPoliceBoxes)
					{
						Debug.Log("Loading Police Boxes");

						if (GC.levelTheme == 4 || GC.challenges.Contains(cChallenge.PoliceState))
						{
							int num2;

							for (int bigTries = 0; bigTries < GC.tileInfo.lockdownZones.Count; bigTries = num2 + 1)
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

										if (GC.tileInfo.GetTileData(vector10).lockdownZone != GC.tileInfo.lockdownZones[bigTries])
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

								Random.InitState(__instance.randomSeedNum + bigTries);
								num2 = bigTries;
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

					if (GC.challenges.Contains(cChallenge.PoliceState) || GC.levelTheme == 3 || GC.levelTheme == 4 || hasPoliceBoxes)
						hasAlarmButtons = true;

					if (GC.challenges.Contains(cChallenge.PoliceState) || GC.challenges.Contains(cChallenge.MACITS))
						hasAlarmButtons = true;
					else if (GC.challenges.Contains(cChallenge.AnCapistan))
						hasAlarmButtons = false;

					if (GC.customLevel)
						hasAlarmButtons = __instance.customLevel.levelFeatures.Contains("AlarmButton");

					if (hasAlarmButtons)
					{
						Debug.Log("Loading Alarm Buttons");
						int bigTries = (int)((float)Random.Range(6, 10) * __instance.levelSizeModifier);
						int num2;

						for (int numObjects = 0; numObjects < bigTries; numObjects = num2 + 1)
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

							Random.InitState(__instance.randomSeedNum + numObjects);
							num2 = numObjects;
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
					#region Manholes
					bool hasManholes = false;

					if ((GC.levelTheme == 3 || (GC.challenges.Contains("MixedUpLevels") && GC.percentChance(33))) && !(GC.levelFeeling == vLevelFeeling.WarZone))
						hasManholes = true;

					if (GC.challenges.Contains(cChallenge.AnCapistan))
						hasManholes = false;

					if (GC.customLevel)
						hasManholes = __instance.customLevel.levelFeatures.Contains("Manhole");

					if (BMTraits.IsPlayerTraitActive(cTrait.UnderdarkCitizen)) // Underdark Manholes; Vanilla below
					{
						Debug.Log("Loading Underdark Manholes");
						int bigTries = (int)((float)Random.Range(8, 12) * __instance.levelSizeModifier);
						int bigTryCounter;

						for (int i = 0; i < bigTries; i = bigTryCounter + 1)
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
							bigTryCounter = i;
						}

						int numObjects = (int)((float)Random.Range(2, 4) * __instance.levelSizeModifier);
						List<Manhole> manholeList = new List<Manhole>();

						for (int i = 0; i < GC.objectRealList.Count; i++)
							if (GC.objectRealList[i].objectName == vObject.Manhole)
								manholeList.Add((Manhole)GC.objectRealList[i]);

						BMLog("UDManhole List count: " + manholeList.Count());

						// Hidden Agent Placement
						if (manholeList.Count > 0)
							for (int i = 0; i < numObjects; i = bigTryCounter + 1)
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
								bigTryCounter = i;
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

					if (GC.levelTheme == 0 || GC.levelTheme == 1 || GC.levelTheme == 3 || GC.levelTheme == 4 || GC.levelTheme == 5 || (GC.challenges.Contains("MixedUpLevels") && GC.percentChance(33)))
						hasFireHydrants = true;

					if (GC.challenges.Contains(cChallenge.AnCapistan))
						hasFireHydrants = false;

					if (GC.customLevel)
						hasFireHydrants = __instance.customLevel.levelFeatures.Contains("FireHydrant");

					if (hasFireHydrants)
					{
						Debug.Log("Loading Fire Hydrants");
						int numObjects = (int)((float)Random.Range(6, 10) * __instance.levelSizeModifier);
						int num2;

						for (int bigTries = 0; bigTries < numObjects; bigTries = num2 + 1)
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

							Random.InitState(__instance.randomSeedNum + bigTries);
							num2 = bigTries;
						}
					}
					#endregion
					#region Exploding Slime Barrels
					bool hasExplodingSlimeBarrels = false;

					if (GC.levelTheme == 1)
						hasExplodingSlimeBarrels = true;

					if (GC.customLevel)
						hasExplodingSlimeBarrels = __instance.customLevel.levelFeatures.Contains("ExplodingSlimeBarrel");

					if (GC.challenges.Contains(cChallenge.ArcologyEcology) || GC.challenges.Contains(cChallenge.PoliceState) || GC.challenges.Contains(cChallenge.MACITS))
						hasExplodingSlimeBarrels = false;
					else if (GC.challenges.Contains(cChallenge.AnCapistan))
						hasExplodingSlimeBarrels = true;

					if (hasExplodingSlimeBarrels || (GC.challenges.Contains("MixedUpLevels") && GC.percentChance(33)))
					{
						Debug.Log("Loading Exploding and Slime Barrels");
						int numObjects = (int)((float)Random.Range(11, 16) * __instance.levelSizeModifier);
						int num2;

						for (int bigTries = 0; bigTries < numObjects; bigTries = num2 + 1)
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

							Random.InitState(__instance.randomSeedNum + bigTries);
							num2 = bigTries;
						}
					}
					#endregion
					#region Flaming Barrels
					bool hasFlamingBarrels = false;

					if (GC.levelTheme == 0 || GC.levelTheme == 1 || GC.levelTheme == 2 || (GC.challenges.Contains("MixedUpLevels") && GC.percentChance(33)))
						hasFlamingBarrels = true;

					if (GC.customLevel)
						hasFlamingBarrels = __instance.customLevel.levelFeatures.Contains("FlamingBarrel");

					if (GC.challenges.Contains(cChallenge.TransitExperiment) || GC.challenges.Contains(cChallenge.PoliceState) || GC.challenges.Contains(cChallenge.MACITS))
						hasFlamingBarrels = false;
					else if (GC.challenges.Contains(cChallenge.AnCapistan))
						hasFlamingBarrels = true;

					if (hasFlamingBarrels)
					{
						Debug.Log("Loading Flaming Barrels");
						int numObjects = (int)((float)Random.Range(6, 10) * __instance.levelSizeModifier);
						int num2;

						for (int bigTries = 0; bigTries < numObjects; bigTries = num2 + 1)
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
								GC.spawnerMain.spawnObjectReal(vector17, null, "FlamingBarrel");

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
					#region Flame Grates
					if (GC.challenges.Contains(cChallenge.TransitExperiment))
						__instance.hasFlameGrates = false;

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

					if (GC.levelTheme == 2 || (GC.challenges.Contains("MixedUpLevels") && GC.percentChance(33)))
						hasBarbecues = true;

					if (GC.customLevel)
						hasBarbecues = __instance.customLevel.levelFeatures.Contains("Barbecue");

					if (GC.challenges.Contains(cChallenge.TransitExperiment))
						hasBarbecues = false;

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
					#region Fountains
					if (GC.levelTheme != 2 && GC.challenges.Contains("MixedUpLevels"))
						GC.percentChance(33);

					if (false)
					{
						Debug.Log("Loading Fountains");
						int numObjects = 1;
						int num2;

						for (int bigTries = 0; bigTries < numObjects; bigTries = num2 + 1)
						{
							Vector2 vector20 = Vector2.zero;
							int num46 = 0;

							do
							{
								vector20 = GC.tileInfo.FindRandLocationGeneral(1f);

								for (int num47 = 0; num47 < GC.objectRealList.Count; num47++)
									if (GC.objectRealList[num47].objectName == "Fountain" && Vector2.Distance(GC.objectRealList[num47].tr.position, vector20) < 14f)
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

							Random.InitState(__instance.randomSeedNum + bigTries);
							num2 = bigTries;
						}
					}
					#endregion
					#region Trees
					bool hasTrees = false;

					if (GC.levelTheme == 2 || (GC.challenges.Contains("MixedUpLevels") && GC.percentChance(33)))
						hasTrees = true;

					if (GC.customLevel)
						hasTrees = __instance.customLevel.levelFeatures.Contains("Tree");

					if (GC.challenges.Contains(cChallenge.ArcologyEcology))
						hasTrees = true;

					if (hasTrees)
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
					bool hasBoulders = false;

					if (GC.levelTheme == 2 || (GC.challenges.Contains("MixedUpLevels") && GC.percentChance(33)))
						hasBoulders = true;

					if (GC.customLevel)
						hasBoulders = __instance.customLevel.levelFeatures.Contains("Boulder");

					if (GC.challenges.Contains(cChallenge.ArcologyEcology) || GC.challenges.Contains(cChallenge.SpelunkyDory))
						hasBoulders = true;

					if (hasBoulders)
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
					bool hasBushes = false;

					if (GC.levelTheme == 2 || (GC.challenges.Contains("MixedUpLevels") && GC.percentChance(33)))
						hasBushes = true;

					if (GC.customLevel)
						hasBushes = __instance.customLevel.levelFeatures.Contains("Bush");

					if (GC.challenges.Contains(cChallenge.ArcologyEcology))
						hasBushes = true;

					if (hasBushes)
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
					bool hasMines = false;

					if (GC.levelTheme == 2 || (GC.challenges.Contains("MixedUpLevels") && GC.percentChance(33)))
						hasMines = true;

					if (GC.customLevel)
						hasMines = __instance.customLevel.levelFeatures.Contains("Mine");

					if (hasMines)
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
					bool hasBearTraps = false;

					if (GC.levelTheme == 2 || (GC.challenges.Contains("MixedUpLevels") && GC.percentChance(33)))
						hasBearTraps = true;

					if (GC.customLevel)
						hasBearTraps = __instance.customLevel.levelFeatures.Contains("BearTrap");

					if (hasBearTraps)
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

						if (hasMusician && GC.levelFeeling != "Riot" && GC.levelFeeling != "HarmAtIntervals" && GC.levelFeeling != "Lockdown" && GC.levelFeeling != "WarZone")
						{
							Debug.Log("Loading Musician");
							int bigTries = 1;
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
											if ((!GC.challenges.Contains("QuickGame") && GC.sessionDataBig.curLevelEndless > 9) || (GC.challenges.Contains("QuickGame") && GC.sessionDataBig.curLevelEndless > 6))
												agentType10 = "Guard2";
											else
												agentType10 = "Guard";

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
						BMAgents.SpawnRoamerSquad(agent, 4, vAgent.Ghost, __instance, false, 1);

					if (level >= 10)
					{
						if (agent.statusEffects.hasTrait(cTrait.MobDebt))
							BMAgents.SpawnRoamerSquad(agent, (int)((float)level * 1.66f), vAgent.Mobster, __instance, false, 4);
						else if (agent.statusEffects.hasTrait(cTrait.MobDebt_2))
							BMAgents.SpawnRoamerSquad(agent, (int)((float)level * 1.33f), vAgent.Mobster, __instance, false, 4);
					}

					if (agent.statusEffects.hasTrait(cTrait.MookMasher))
						BMAgents.SpawnRoamerSquad(agent, level * 2, vAgent.Goon, __instance, false, 4);

					if (agent.statusEffects.hasTrait(cTrait.Reinforcements))
						BMAgents.SpawnRoamerSquad(agent, 3, vAgent.ResistanceLeader, __instance, true, 1);
					else if (agent.statusEffects.hasTrait(cTrait.Reinforcements_2))
						BMAgents.SpawnRoamerSquad(agent, 6, vAgent.ResistanceLeader, __instance, true, 1);
				}
			}
		}
		//
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
			Prefix(typeof(SpawnerFloor), "spawn", GetType(), "SpawnerFloor_spawn", new Type[1] { typeof(string) });
		}
		public static bool SpawnerFloor_spawn(string floorName, SpawnerFloor __instance, tk2dTileMap ___tilemapFloors, tk2dTileMap ___tilemapFloors3, tk2dTileMap ___tilemapFloors4) // Prefix
		{
			if (BMChallenges.IsChallengeFromListActive(cChallenge.AffectsFloors))
				floorName = vFloor.CasinoFloor;

			if (GC.levelTheme == 2 && floorName == vFloor.FlamePit)
				floorName = vFloor.Hole;

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

					switch (floorName)
					{
						case vFloor.ArenaFloor:
							num5 = 712;
							__instance.tileInfo.tileArray[num, num2].arenaFloor = true;
							break;
						case vFloor.ArmoryFloor:
							num5 = 172;
							floorMaterialType = floorMaterialType.ArmoryFloor;
							break;
						case vFloor.BankFloor:
							if (GC.levelTheme == 5)
								num5 = 1356;
							else
								num5 = 172;

							floorMaterialType = floorMaterialType.BankFloor;
							break;
						case vFloor.Bathhouse:
							num5 = 824;
							floorMaterialType = floorMaterialType.Bathhouse;
							break;
						case vFloor.BathroomTile:
							num5 = 46;
							floorMaterialType = floorMaterialType.BathroomTile;
							break;
						case vFloor.BrickIndoor:
							num5 = 1376;
							floorMaterialType = floorMaterialType.BrickIndoor;
							break;
						case vFloor.Bridge_Unused:
							floorMaterialType = floorMaterialType.Bridge;
							__instance.tileInfo.tileArray[num, num2].bridge = true;
							break;
						case vFloor.Canal:
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
							break;
						case vFloor.CasinoFloor:
							num5 = 152;
							floorMaterialType = floorMaterialType.CasinoFloor;
							break;
						case vFloor.CaveFloor:
							num5 = 736;
							floorMaterialType = floorMaterialType.CaveFloor;
							break;
						case vFloor.Checkerboard:
							num5 = 66;
							floorMaterialType = floorMaterialType.Checkerboard;
							break;
						case vFloor.Checkerboard2:
							num5 = 1356;
							floorMaterialType = floorMaterialType.Checkerboard2;
							break;
						case vFloor.CleanTiles:
							num5 = 1064;
							floorMaterialType = floorMaterialType.CleanTiles;
							break;
						case vFloor.CleanTilesRaised:
							num5 = 1104;
							floorMaterialType = floorMaterialType.CleanTilesRaised;
							break;
						case vFloor.ClearFloor:
							floorMaterialType = floorMaterialType.ClearFloor;
							break;
						case vFloor.ClearFloor2:
							floorMaterialType = floorMaterialType.ClearFloor;
							break;
						case vFloor.ConveyorBelt:
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
							break;
						case vFloor.DanceFloor:
							num5 = 844;
							floorMaterialType = floorMaterialType.DanceFloor;
							break;
						case vFloor.DanceFloorRaised:
							num5 = 1124;
							floorMaterialType = floorMaterialType.DanceFloorRaised;
							break;
						case vFloor.DirtFloor:
							num5 = 756;
							floorMaterialType = floorMaterialType.DirtFloor;
							break;
						case vFloor.DirtyTiles:
							num5 = 1084;
							floorMaterialType = floorMaterialType.DirtyTiles;
							break;
						case vFloor.DrugDenFloor:
							num5 = 496;
							floorMaterialType = floorMaterialType.DrugDenFloor;
							break;
						case vFloor.ElectronicPlates:
							num5 = 1184;
							floorMaterialType = floorMaterialType.ElectronicPlates;
							break;
						case vFloor.Facility:
							num5 = 1224;
							floorMaterialType = floorMaterialType.Facility;
							break;
						case vFloor.FactoryFloor:
							num5 = 476;
							floorMaterialType = floorMaterialType.FactoryFloor;
							break;
						case vFloor.FlamePit:
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
							break;
						case vFloor.Grass:
							if (GC.levelTheme == 3)
								num5 = 1416;
							else if (GC.levelTheme == 5)
								num5 = 1396;
							else
								num5 = 1044;

							floorMaterialType = floorMaterialType.CityParkFloor;
							break;
						case vFloor.GreyTile_Unused:
							num5 = 13;
							floorMaterialType = floorMaterialType.GreyTile;
							break;
						case vFloor.Gym:
							num5 = 864;
							floorMaterialType = floorMaterialType.Gym;
							break;
						case vFloor.HideoutFloor:
							if (GC.levelTheme == 1)
								num5 = 476;
							else
								num5 = 172;
							
							floorMaterialType = floorMaterialType.HideoutFloor;
							break;
						case vFloor.Hole:
							if (GC.levelTheme != 1 && GC.levelTheme != 2 && GC.levelTheme != 4 && GC.levelTheme != 5 && (!__instance.transform.parent.GetComponent<Chunk>().userCreated || !(__instance.transform.parent.GetComponent<Chunk>().description != "Hideout")))
							{
								Object.Destroy(__instance.gameObject);
								return false;
							}

							num5 = 192;
							__instance.tileInfo.tileArray[num, num2].hole = true;
							__instance.floorLayer = 2;
							floorMaterialType = floorMaterialType.Hole;
							break;
						case vFloor.HospitalFloor:
							num5 = 436;
							floorMaterialType = floorMaterialType.HospitalFloor;
							break;
						case vFloor.Ice:
							num5 = 712;
							__instance.tileInfo.tileArray[num, num2].ice = true;
							__instance.tileInfo.tileArray[num, num2].organic = true;
							__instance.floorLayer = 2;
							floorMaterialType = floorMaterialType.Ice;
							break;
						case vFloor.IceRink:
							num5 = 984;
							__instance.tileInfo.tileArray[num, num2].ice = true;
							floorMaterialType = floorMaterialType.IceRink;
							break;
						case vFloor.Mall:
							num5 = 884;
							floorMaterialType = floorMaterialType.Mall;
							break;
						case vFloor.MetalFloor:
							num5 = 86;
							floorMaterialType = floorMaterialType.MetalFloor;
							break;
						case vFloor.MetalPlates:
							num5 = 1244;
							floorMaterialType = floorMaterialType.MetalPlates;
							break;
						case vFloor.Muted:
							num5 = 1264;
							floorMaterialType = floorMaterialType.Muted;
							break;
						case vFloor.Normal:
							if (GC.levelTheme == 3 || GC.levelTheme == 5)
								num5 = 924;
							else if (GC.levelTheme == 4)
								num5 = 1184;
							else
								num5 = 110;

							floorMaterialType = floorMaterialType.Normal;
							break;
						case vFloor.OfficeFloor:
							num5 = 456;
							floorMaterialType = floorMaterialType.OfficeFloor;
							break;
						case vFloor.PoliceStationFloor:
							num5 = 172;
							floorMaterialType = floorMaterialType.PoliceStationFloor;
							break;
						case vFloor.Pool:
							num5 = 1024;
							__instance.tileInfo.tileArray[num, num2].water = true;
							__instance.floorLayer = 2;
							floorMaterialType = floorMaterialType.Pool;
							list3.Add(new Vector2((float)num, (float)num2));
							break;
						case vFloor.Posh:
							num5 = 904;
							floorMaterialType = floorMaterialType.Posh;
							break;
						case vFloor.PrisonFloor:
							num5 = 536;
							floorMaterialType = floorMaterialType.PrisonFloor;
							break;
						case vFloor.RugBlue:
							num5 = 252;
							floorMaterialType = floorMaterialType.BlueRug;
							break;
						case vFloor.RugDarkBlue:
							num5 = 272;
							floorMaterialType = floorMaterialType.DarkBlueRug;
							break;
						case vFloor.RugGreen:
							num5 = 212;
							floorMaterialType = floorMaterialType.GreenRug;
							break;
						case vFloor.RugPurple:
							num5 = 556;
							floorMaterialType = floorMaterialType.PurpleRug;
							break;
						case vFloor.RugRed:
							num5 = 232;
							floorMaterialType = floorMaterialType.RedRug;
							break;
						case vFloor.SmallTiles:
							num5 = 1164;
							floorMaterialType = floorMaterialType.SmallTiles;
							break;
						case vFloor.SolidPlates:
							num5 = 1204;
							floorMaterialType = floorMaterialType.SolidPlates;
							break;
						case vFloor.Water:
							num5 = 688;
							__instance.tileInfo.tileArray[num, num2].water = true;
							__instance.tileInfo.tileArray[num, num2].organic = true;
							__instance.floorLayer = 2;
							floorMaterialType = floorMaterialType.Water;
							list3.Add(new Vector2((float)num, (float)num2));
							break;
						case vFloor.WoodClean:
							num5 = 924;
							floorMaterialType = floorMaterialType.WoodClean;
							break;
						case vFloor.WoodSlats:
							num5 = 944;
							floorMaterialType = floorMaterialType.WoodSlats;
							break;
						default:
							if (floorName != null)
								if (floorName.Length == 0)
								{
									num5 = 0;
									floorMaterialType = floorMaterialType.None;
								}

							break;
					}
					
					if (floorName == vFloor.ClearFloor || floorName == vFloor.ArenaFloor)
					{
						if (__instance.ownedByID != 55)
							__instance.ownedByID = 88;
						
						__instance.SetExtraFloorParams(num, num2, floorMaterialType);
					}
					else if (floorName == vFloor.ClearFloor2)
						__instance.SetExtraFloorParams(num, num2, floorMaterialType);
					else if (!(floorName == vFloor.Bridge_Unused))
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
			else if (floorName == vFloor.Water || floorName == vFloor.Canal || floorName == vFloor.Pool)
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
			else if (!(floorName == vFloor.Ice) && !(floorName == vFloor.Bridge_Unused) && !(floorName == vFloor.Grass))
			{
				if (floorName == vFloor.ConveyorBelt)
				{
					Chunk component3 = __instance.transform.parent.GetComponent<Chunk>();

					if (component3 == null)
						component3 = __instance.transform.parent.transform.parent.GetComponent<Chunk>();
					
					string a = __instance.direction;
					string text;

					if (!(new string[] { "E", "N", "S", "W" }.Contains(a)))
						text = "S";
					else
						text = a;

					if (component3.chunkDirX == "Reverse" && !__instance.skipRotations)
					{
						if (text == "E")
							text = "W";
						if (text == "W")
							text = "E";
					}

					if (component3.chunkDirY == "Reverse" && !__instance.skipRotations)
					{
						if (text == "N")
							text = "S";
						if (text == "S")
							text = "N";
					}

					if (component3.chunkRotate == 1 && !__instance.skipRotations)
					{
						if (text == "E")
							text = "N";
						else if (text == "N")
							text = "w";
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
