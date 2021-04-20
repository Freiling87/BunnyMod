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
			// LoadLevel
			Prefix(typeof(LoadLevel), "CanUseChunk", GetType(), "LoadLevel_CanUseChunk", new Type[6] { typeof(GameObject), typeof(ChunkData), typeof(bool), typeof(int), typeof(int), typeof(Vector3) });
			Prefix(typeof(LoadLevel), "CreateInitialMap", GetType(), "LoadLevel_CreateInitialMap", new Type[0] { });
			Prefix(typeof(LoadLevel), "FillFloors", GetType(), "LoadLevel_FillFloors_Prefix", new Type[0] { });
			Prefix(typeof(LoadLevel), "FillMapChunks", GetType(), "LoadLevel_FillMapChunks_Prefix", new Type[0] { });
			Prefix(typeof(LoadLevel), "FillMapChunks2", GetType(), "LoadLevel_FillMapChunks2_Prefix", new Type[0] { });
			Prefix(typeof(LoadLevel), "LoadStuff2", GetType(), "LoadLevel_LoadStuff2_Prefix", new Type[0] { });
			Prefix(typeof(LoadLevel), "SetupMore3_3", GetType(), "LoadLevel_SetupMore3_3_Prefix", new Type[0] { });
			Postfix(typeof(LoadLevel), "SetupMore3_3", GetType(), "LoadLevel_SetupMore3_3_Postfix", new Type[0] { });
			Postfix(typeof(LoadLevel), "SetupMore5_2", GetType(), "LoadLevel_SetupMore5_2", new Type[0] { });

			// RandomWalls
			Prefix(typeof(RandomWalls), "fillWalls", GetType(), "RandomWalls_fillWalls", new Type[0] { });

			// SpawnerFloor
			Prefix(typeof(SpawnerFloor), "spawn", GetType(), "SpawnerFloor_spawn", new Type[1] { typeof(string) });

			// SpawnerObject
			Prefix(typeof(SpawnerObject), "spawn", GetType(), "SpawnerObject_spawn", new Type[1] { typeof(string) });
		}
		#region Custom
		public static int LevelSizeMod(int vanilla)
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
			foreach (string mutator in cChallenge.WallsAndFloors)
				if (GC.challenges.Contains(mutator))
					return true;

			return false;
		}
		public static int ForceQuestCount(int vanilla)
		{
			if (GC.challenges.Contains(cChallenge.RushinRevolution))
				vanilla = 0;
			else if (GC.challenges.Contains(cChallenge.SingleMinded))
				vanilla = 1;
			else if (GC.challenges.Contains(cChallenge.Workhorse))
				vanilla = 4;

			return vanilla;
		}
		public static string GetFloorMutator()
		{
			foreach (string mutator in cChallenge.FloorsAndFeatures)
				if (GC.challenges.Contains(mutator))
					return mutator;

			foreach (string mutator in cChallenge.WallsAndFloors)
				if (GC.challenges.Contains(mutator))
					return mutator;

			return cChallenge.Null;
		}
		public static string GetFloorTileFromMutator()
		{
			switch (GetFloorMutator())
			{
				// Floor Mods

				case cChallenge.ArcologyEcology:
					return vFloor.Grass;

				case cChallenge.SunkenCity:
					return vFloor.Pool;

				case cChallenge.TransitExperiment:
					return vFloor.IceRink;

				// Wall & Floor Mods

				case cChallenge.CityOfSteel:
					return vFloor.MetalFloor;

				case cChallenge.GreenLiving:
					return vFloor.Grass;

				case cChallenge.Panoptikopolis:
					return vFloor.CleanTiles;

				case cChallenge.ShantyTown:
					return vFloor.DirtFloor;

				case cChallenge.SpelunkyDory:
					return vFloor.CaveFloor;

				default:
					return "";
			}
		}
		public static string GetFloorTileGroupFromMutator()
		{
			switch (GetFloorMutator())
			{
				// Floor Mods

				case cChallenge.ArcologyEcology:
					return vFloorTileGroup.Park;

				case cChallenge.SunkenCity:
					return vFloorTileGroup.Water;

				case cChallenge.TransitExperiment:
					return vFloorTileGroup.Ice;

				// Wall & Floor Mods

				case cChallenge.CityOfSteel:
					return vFloorTileGroup.Uptown;

				case cChallenge.GreenLiving:
					return vFloorTileGroup.Park;

				case cChallenge.Panoptikopolis:
					return vFloorTileGroup.Uptown;

				case cChallenge.ShantyTown:
					return vFloorTileGroup.UnknownPossiblyGeneric;

				case cChallenge.SpelunkyDory:
					return vFloorTileGroup.Industrial;

				default:
					return "";
			}
		}
		public static string GetWallMutator()
		{
			foreach (string mutator in GC.challenges)
				if (cChallenge.WallsAndFloors.Contains(mutator))
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
		public static int SetRoamerCount(int vanilla)
		{
			if (GC.challenges.Contains(cChallenge.GhostTown))
				vanilla *= 0;
			if (GC.challenges.Contains(cChallenge.LetMeSeeThatThrong))
				vanilla *= 4;
			else if (GC.challenges.Contains(cChallenge.SwarmWelcome))
				vanilla *= 8;

			return vanilla;
		}
		public static int SetGangCount(int vanilla)
		{
			BMLog("SetGangCount");

			if (GC.challenges.Contains(cChallenge.HoodlumsWonderland))
				vanilla = 12;

			return vanilla;
		}
		public static int SetMafiaCount(int vanilla)
		{


			return vanilla;
		}
		public static void SpawnRoamerSquad(Agent playerAgent, int numberToSpawn, string agentType, LoadLevel __instance, bool aligned, int splitIntoGroupSize) // Non-Patch
		{
			BMLog("LoadLevel_CanUseChunk");

			List<Agent> spawnedAgentList = new List<Agent>();
			//playerAgent.gangStalking = Agent.gangCount;
			Vector2 pos = Vector2.zero;

			numberToSpawn = (int)((float)numberToSpawn * __instance.levelSizeModifier);

			for (int i = 0; i < numberToSpawn; i++)
			{
				if (i % splitIntoGroupSize == 0)
					Agent.gangCount++; // Splits spawn into groups

				Vector2 vector = Vector2.zero;
				int attempts = 0;

				if (i == 0)
				{
					do
					{
						vector = GC.tileInfo.FindRandLocationGeneral(0.32f);
						attempts++;
					}
					while ((vector == Vector2.zero || Vector2.Distance(vector, GC.playerAgent.tr.position) < 20f) && attempts < 300);

					pos = vector;
				}
				else
					vector = GC.tileInfo.FindLocationNearLocation(pos, null, 0.32f, 1.28f, true, true);

				if (vector != Vector2.zero && attempts < 300)
				{
					Agent agent = GC.spawnerMain.SpawnAgent(vector, null, agentType);
					agent.movement.RotateToAngleTransform((float)Random.Range(0, 360));
					agent.gang = Agent.gangCount;
					agent.modLeashes = 0;

					if (agentType == vAgent.Ghost)
						agent.alwaysRun = true;

					agent.wontFlee = true;
					agent.agentActive = true;
					//agent.statusEffects.AddStatusEffect("InvisiblePermanent");
					//agent.oma.mustBeGuilty = true;
					spawnedAgentList.Add(agent);

					if (spawnedAgentList.Count > 1)
						for (int j = 0; j < spawnedAgentList.Count; j++)
							if (spawnedAgentList[j] != agent)
							{
								agent.relationships.SetRelInitial(spawnedAgentList[j], "Aligned");
								spawnedAgentList[j].relationships.SetRelInitial(agent, "Aligned");
							}

					if (aligned)
					{
						agent.relationships.SetRel(playerAgent, "Aligned");
						playerAgent.relationships.SetRel(agent, "Aligned");
					}
					else
					{
						agent.relationships.SetRel(playerAgent, "Hateful");
						playerAgent.relationships.SetRel(agent, "Hateful");
						agent.relationships.SetRelHate(playerAgent, 5);
						playerAgent.relationships.SetRelHate(agent, 5);
					}

					if (agentType == vAgent.ResistanceLeader && BMTraits.IsPlayerTraitActive(cTrait.Reinforcements_2))
					{
						InvItem invItem = new InvItem();
						invItem.invItemName = GC.Choose<string>(vItem.Revolver, vItem.MachineGun);
						invItem.ItemSetup(false);
						agent.inventory.AddItemAtEmptySlot(invItem, true, false);
						agent.inventory.equippedWeapon = invItem;

						agent.inventory.startingHeadPiece = vArmorHead.SoldierHelmet;
					}
					else if (agentType == vAgent.ResistanceLeader && BMTraits.IsPlayerTraitActive(cTrait.Reinforcements))
					{
						InvItem invItem = new InvItem();
						invItem.invItemName = GC.Choose<string>(vItem.Pistol, vItem.Knife);
						invItem.ItemSetup(false);
						agent.inventory.AddItemAtEmptySlot(invItem, true, false);
						agent.inventory.equippedWeapon = invItem;

						agent.inventory.startingHeadPiece = vArmorHead.HardHat;
					}

					agent.SetDefaultGoal(vAgentGoal.WanderLevel);
				}
			}
		}
		#endregion

		#region BasicSpawn

		#endregion
		#region LoadLevel
		public static bool LoadLevel_CanUseChunk(GameObject myChunkGo, ChunkData myChunkBasic, bool checkSessionData, int randChunkNum, int chunkShape, Vector3 myNewPos, LoadLevel __instance, bool __result) // Prefix
		{
			BMLog("LoadLevel_CanUseChunk");

			#region Generic
			Chunk chunk = null;
			string description;

			if (GC.basicSpawns)
				description = myChunkBasic.description;
			else
			{
				chunk = myChunkGo.GetComponent<Chunk>();
				description = chunk.description;
			}
			
			int sameChunkTypeCount = 0;
			
			if (checkSessionData && GC.sessionData.usedChunks.Contains(randChunkNum))
			{
				__result = false; 
				return false;
			}

			if (GC.basicSpawns)
				using (List<ChunkData>.Enumerator enumerator = __instance.usedChunksThisLevelBasic.GetEnumerator())
					while (enumerator.MoveNext())
					{
						ChunkData chunkData = enumerator.Current;

						if (description == chunkData.description && checkSessionData)
							sameChunkTypeCount++;
						if (description != "Generic" && description != "Lake" && myChunkBasic == chunkData)
						{
							__result = false; 
							return false;
						}
					}

			foreach (GameObject gameObject in __instance.usedChunksThisLevel)
			{
				Chunk component = gameObject.GetComponent<Chunk>();

				if (description == component.description && checkSessionData)
					sameChunkTypeCount++;

				if (description != "Generic" && description != "Lake" && chunk == component)
				{
					__result = false; 
					return false;
				}
			}

			#endregion
			#region Levels
			if (GC.levelTheme == (int)vLevelTheme.LevelTheme.Slums &&description != "Generic")
			{
				if ((GC.sessionDataBig.curLevelEndless == 1 && description == vChunkType.PoliceStation) ||
					vChunkType.SlumsProhibited.Contains(description) ||
					(sameChunkTypeCount >= 3 && vChunkType.SlumsLimitedTo3.Contains(description)) ||
					(sameChunkTypeCount >= 2 && vChunkType.SlumsLimitedTo2.Contains(description)) ||
					(sameChunkTypeCount >= 1 && vChunkType.SlumsLimitedTo1.Contains(description)))
					__result = false;

				return false;
			}
			else if (GC.levelTheme == (int)vLevelTheme.LevelTheme.Industrial && description != "Generic")
			{
				if (vChunkType.IndustrialProhibited.Contains(description) ||
						(sameChunkTypeCount >= 3 && vChunkType.IndustrialLimitedTo3.Contains(description)) ||
						(sameChunkTypeCount >= 2 && vChunkType.IndustrialLimitedTo2.Contains(description)) ||
						(sameChunkTypeCount >= 1 && vChunkType.IndustrialLimitedTo1.Contains(description)))
					__result = false; 

				return false;
			}
			else if (GC.levelTheme == (int)vLevelTheme.LevelTheme.Park)
			{
				if (vChunkType.ParkProhibited.Contains(description) ||
					(sameChunkTypeCount >= 3 && vChunkType.ParkLimitedTo3.Contains(description)) ||
					(sameChunkTypeCount >= 2 && vChunkType.ParkLimitedTo2.Contains(description)) ||
					(sameChunkTypeCount >= 1 && vChunkType.ParkLimitedTo1.Contains(description)))
				{
					__result = false; 
					return false;
				}

				if (chunkShape >= 0 && chunkShape <= 4)
				{
					bool flag = false;

					for (int i = 0; i < __instance.levelChunks.Count; i++)
						if (__instance.levelChunks[i].description != "Generic" && __instance.levelChunks[i].description != "Lake" && Vector2.Distance(__instance.levelChunks[i].chunkPos, myNewPos) <= __instance.chunkSize * 2f)
							flag = true;
					
					if (flag && description != "Generic" && description != "Lake")
					{
						__result = false; 
						return false;
					}
				}

				if (chunkShape == 5 || chunkShape == 6)
				{
					if (chunkShape == 6)
					{
						string a = GC.rnd.RandomSelect("Level3ChunkHuge", "RandomScenarios");

						if ((description == "Hideout" && a == "NotHideout") ||
							(description != "Hideout" && a == "Hideout"))
						{
							__result = false; 
							return false;
						}
					}

					__result = (
						description == "Hideout" || 
						description == "Graveyard" || 
						description == "MilitaryOutpost" || 
						description == "Cabin" || 
						description == "Greenhouse" || 
						description == "Cave" || 
						description == "Farm" || 
						description == "HedgeMaze");
					return false;
				}

				if (chunkShape == 1 || chunkShape == 2 || chunkShape == 3)
				{
					__result = true; 
					return false;
				}

				if (!(description == "Generic") && 
					!(description == "Lake") && 
					!(description == "Hideout") && 
					!(description == "MilitaryOutpost") && 
					!(description == "Cabin") && 
					!(description == "Greenhouse") && 
					!(description == "Cave") && 
					!(description == "Farm") && 
					!(description == "HedgeMaze") && 
					!(description == "Shop") && 
					!(description == "Bathroom") && 
					!(description == "Graveyard"))
				{
					__result = false; 
					return false;
				}

				if (description == "Lake")
				{
					bool flag2 = false;

					for (int j = 0; j < __instance.levelChunks.Count; j++)
						if ((__instance.levelChunks[j].description == "Lake" || __instance.levelChunks[j].description == "Island") && Vector2.Distance(__instance.levelChunks[j].chunkPos, myNewPos) <= __instance.chunkSize * 2f)
							flag2 = true;

					__result = !flag2;
					return false;
				}

				__result = true; 
				return false;
			}
			else if (GC.levelTheme == (int)vLevelTheme.LevelTheme.Downtown && description != "Generic")
			{
				if (vChunkType.DowntownProhibited.Contains(description) ||
					(sameChunkTypeCount >= 3 && description == "Temp") ||
					(sameChunkTypeCount >= 2 && vChunkType.DowntownLimitedTo2.Contains(description)) ||
					(sameChunkTypeCount >= 1 && vChunkType.DowntownLimitedTo1.Contains(description)))
					__result = false; 

				return false;
			}
			else if (GC.levelTheme == (int)vLevelTheme.LevelTheme.Uptown && description != "Generic")
			{
				if (vChunkType.UptownProhibited.Contains(description) ||
					(sameChunkTypeCount >= 5 && vChunkType.UptownLimitedTo5.Contains(description)) ||
					(sameChunkTypeCount >= 3 && vChunkType.UptownLimitedTo3.Contains(description)) ||
					(sameChunkTypeCount >= 2 && vChunkType.UptownLimitedTo2.Contains(description)) ||
					(sameChunkTypeCount >= 1 && !__instance.squareMap && vChunkType.UptownLimitedTo1.Contains(description)))
					__result = false; 

				return false;
			}
			else if (GC.levelTheme == (int)vLevelTheme.LevelTheme.MayorVillage && description != "Generic")
			{
				if (vChunkType.MayorVillageProhibited.Contains(description) ||
					(sameChunkTypeCount >= 2 && vChunkType.MayorVillageLimitedTo2.Contains(description)) ||
					(sameChunkTypeCount >= 1 && vChunkType.MayorVillageLimitedTo1.Contains(description)))
				{
					__result = false; 
					return false;
				}
			}
			#endregion

			__result = true; 
			return false;
		}
		public static bool LoadLevel_CreateInitialMap(LoadLevel __instance, ref bool ___placedKey1, ref bool ___placedKey2, ref bool ___placedKey3) // Replacement
		{
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

				GC.quests.questTriesTotal = ForceQuestCount(GC.quests.questTriesTotal);
				
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

				__instance.levelSizeMax = LevelSizeMod(__instance.levelSizeMax);

				Debug.Log("LEVEL SIZE: " + __instance.levelSizeMax);

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
			BMLog("LoadLevel_FillFloors_Prefix");

			// Structure advised by Abbysssal for patch-replacing IEnumerators.
			__result = LoadLevel_FillFloors_Replacement(__instance, ___tilemapFloors2);

			return false;
		}
		public static IEnumerator LoadLevel_FillFloors_Replacement(LoadLevel __instance, tk2dTileMap ___tilemapFloors2) // Replacement
		{
			BMLog("LoadLevel_FillFloors_Replacement");

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
						for (int l = num4; l > num5; l--)
						{
							__instance.tileInfo.tileArray[k, l - 1].chunkID = __instance.mapChunkArray[i2, j2].chunkID;
							int tile = 0;

							if (GC.levelShape == 0 && GC.levelType != "HomeBase")
							{
								if (BMChallenges.IsChallengeFromListActive(cChallenge.FloorsAndFeatures) || BMChallenges.IsChallengeFromListActive(cChallenge.WallsAndFloors))
									tile = int.Parse(GC.rnd.RandomSelect(GetFloorTileGroupFromMutator(), "RandomFloorsWalls"));
								else if (GC.levelTheme == 0)
									tile = int.Parse(GC.rnd.RandomSelect(vFloorTileGroup.Slums, "RandomFloorsWalls"));
								else if (GC.levelTheme == 1)
									tile = int.Parse(GC.rnd.RandomSelect(vFloorTileGroup.Industrial, "RandomFloorsWalls"));
								else if (GC.levelTheme == 2)
									tile = int.Parse(GC.rnd.RandomSelect(vFloorTileGroup.Park, "RandomFloorsWalls"));
								else if (GC.levelTheme == 3)
									tile = int.Parse(GC.rnd.RandomSelect(vFloorTileGroup.Downtown, "RandomFloorsWalls"));
								else if (GC.levelTheme == 4)
									tile = int.Parse(GC.rnd.RandomSelect(vFloorTileGroup.Uptown, "RandomFloorsWalls"));
								else if (GC.levelTheme == 5)
									tile = int.Parse(GC.rnd.RandomSelect(vFloorTileGroup.MayorVillage, "RandomFloorsWalls"));
							}
							else
								tile = int.Parse(GC.rnd.RandomSelect("FloorTilesBuilding", "RandomFloorsWalls"));
							
							___tilemapFloors2.SetTile(k, l - 1, 0, tile);
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
		public static bool LoadLevel_FillMapChunks_Prefix (LoadLevel __instance, ref IEnumerator __result, ref tk2dTileMap ___tilemapFloors2, ref tk2dTileMap ___tilemapWalls, ref List<GameObject> ___chunkList, ref List<ChunkData> ___chunkListBasic, ref List<ChunkData> ___chunkListBasicBackup) // Prefix
		{
			BMLog("LoadLevel_FillMapChunks_Prefix");

			// Structure advised by Abbysssal for patch-replacing IEnumerators.
			__result = LoadLevel_FillMapChunks_Replacement(__instance, ___tilemapFloors2, ___tilemapWalls, ___chunkList, ___chunkListBasic, ___chunkListBasicBackup);

			return false;
		}
		public static IEnumerator LoadLevel_FillMapChunks_Replacement(LoadLevel __instance, tk2dTileMap ___tilemapFloors2, tk2dTileMap ___tilemapWalls, List<GameObject> ___chunkList, List <ChunkData> ___chunkListBasic, List<ChunkData> ___chunkListBasicBackup) // Replacement
		{
			BMLog("LoadLevel_FillMapChunks_Replacement");

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

					if (__instance.mapChunkArray[i2, j2].chunkID != 0)
					{
						int num2 = i2 * 16;
						int num3 = i2 * 16 + 16;
						int num4 = 160 - j2 * 16;
						int num5 = 160 - j2 * 16 - 16;

						for (int k = num2; k < num3; k++)
							for (int l = num4; l > num5; l--)
							{
								___tilemapWalls.ClearTile(k, l - 1, 0);
								__instance.tileInfo.tileArray[k, l - 1].chunkID = __instance.mapChunkArray[i2, j2].chunkID;
								int tile = 0;

								if (GC.levelShape == 0 && GC.levelType != "HomeBase")
								{
									if (BMChallenges.IsChallengeFromListActive(cChallenge.FloorsAndFeatures) || BMChallenges.IsChallengeFromListActive(cChallenge.WallsAndFloors))
										tile = int.Parse(GC.rnd.RandomSelect(GetFloorTileGroupFromMutator(), "RandomFloorsWalls"));
									else if (GC.levelTheme == 0)
										tile = int.Parse(GC.rnd.RandomSelect(vFloorTileGroup.Slums, "RandomFloorsWalls"));
									else if (GC.levelTheme == 1)
										tile = int.Parse(GC.rnd.RandomSelect(vFloorTileGroup.Industrial, "RandomFloorsWalls"));
									else if (GC.levelTheme == 2)
										tile = int.Parse(GC.rnd.RandomSelect(vFloorTileGroup.Park, "RandomFloorsWalls"));
									else if (GC.levelTheme == 3)
										tile = int.Parse(GC.rnd.RandomSelect(vFloorTileGroup.Downtown, "RandomFloorsWalls"));
									else if (GC.levelTheme == 4)
										tile = int.Parse(GC.rnd.RandomSelect(vFloorTileGroup.Uptown, "RandomFloorsWalls"));
									else if (GC.levelTheme == 5)
										tile = int.Parse(GC.rnd.RandomSelect(vFloorTileGroup.MayorVillage, "RandomFloorsWalls"));
								}
								else
									tile = int.Parse(GC.rnd.RandomSelect("FloorTilesBuilding", "RandomFloorsWalls"));
								
								___tilemapFloors2.SetTile(k, l - 1, 0, tile);
							}
					}
					else if (!GC.holeLevel)
					{
						__instance.mapChunkArray[i2, j2].filled = true;
						int num6 = i2 * 16;
						int num7 = i2 * 16 + 16;
						int num8 = 160 - j2 * 16;
						int num9 = 160 - j2 * 16 - 16;

						for (int m = num6; m < num7; m++)
							for (int n = num8; n > num9; n--)
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
									int tile2 = 0;

									if (GC.levelShape == 0 && GC.levelType != "HomeBase")
									{
										if (BMChallenges.IsChallengeFromListActive(cChallenge.FloorsAndFeatures) || BMChallenges.IsChallengeFromListActive(cChallenge.WallsAndFloors))
											tile2 = int.Parse(GC.rnd.RandomSelect(GetFloorTileGroupFromMutator(), "RandomFloorsWalls"));
										else if (GC.levelTheme == 0)
											tile2 = int.Parse(GC.rnd.RandomSelect(vFloorTileGroup.Slums, "RandomFloorsWalls"));
										else if (GC.levelTheme == 1)
											tile2 = int.Parse(GC.rnd.RandomSelect(vFloorTileGroup.Industrial, "RandomFloorsWalls"));
										else if (GC.levelTheme == 2)
											tile2 = int.Parse(GC.rnd.RandomSelect(vFloorTileGroup.Park, "RandomFloorsWalls"));
										else if (GC.levelTheme == 3)
											tile2 = int.Parse(GC.rnd.RandomSelect(vFloorTileGroup.Downtown, "RandomFloorsWalls"));
										else if (GC.levelTheme == 4)
											tile2 = int.Parse(GC.rnd.RandomSelect(vFloorTileGroup.Uptown, "RandomFloorsWalls"));
										else if (GC.levelTheme == 5)
											tile2 = int.Parse(GC.rnd.RandomSelect(vFloorTileGroup.MayorVillage, "RandomFloorsWalls"));
									}
									else
										tile2 = int.Parse(GC.rnd.RandomSelect("FloorTilesBuilding", "RandomFloorsWalls"));
									
									___tilemapFloors2.SetTile(m, n - 1, 0, tile2);
									tileData.chunkID = __instance.mapChunkArray[i2, j2].chunkID;
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

			if (GC.levelType == "Tutorial" || GC.levelType == "HomeBase")
				__instance.allChunksFilled = true;
			else
				__instance.StartCoroutine(LoadLevel_FillMapChunks2_Replacement(__instance, ___chunkList, ___chunkListBasic, ___chunkListBasicBackup));

			yield break;
		}
		public static bool LoadLevel_FillMapChunks2_Prefix (LoadLevel __instance, ref IEnumerator __result, ref List<GameObject> ___chunkList, ref List <ChunkData> ___chunkListBasic, ref List <ChunkData> ___chunkListBasicBackup) // Prefix
		{
			BMLog("LoadLevel_FillMapChunks2_Prefix");

			// Structure advised by Abbysssal for patch-replacing IEnumerators.
			__result = LoadLevel_FillMapChunks2_Replacement(__instance, ___chunkList, ___chunkListBasic, ___chunkListBasicBackup);

			return false;
		}
		public static IEnumerator LoadLevel_FillMapChunks2_Replacement(LoadLevel __instance, List<GameObject> ___chunkList, List <ChunkData> ___chunkListBasic, List <ChunkData> ___chunkListBasicBackup) // Replacement
		{
			BMLog("LoadLevel_FillMapChunks2_Replacement");

			int longChunks = 0;
			int hugeChunks = 0;
			int longTries = 0;
			int hugeTries = 0;
			int questsAdded = 0;
			int maxQuests = GC.quests.totalQuests;
			int triesCount = 0;
			float maxChunkTime = 0.02f;
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			Random.InitState(__instance.randomSeedNum);

			while (!__instance.allChunksFilled)
			{
				int num = triesCount;
				triesCount = num + 1;
			
				__instance.allChunksFilled = true;
				
				for (int i = 0; i < __instance.levelSizeAxis; i++)
					for (int j = 0; j < __instance.levelSizeAxis; j++)
					{
						MapChunk mapChunk = __instance.mapChunkArray[i, j];

						if (mapChunk.chunkID != 0 && !mapChunk.filled && (!GC.customLevel || GC.customRandomLevel || mapChunk.chunkMainTile))
							__instance.allChunksFilled = false;
					}

				if (!__instance.allChunksFilled)
				{
					int num2 = Random.Range(0, __instance.levelSizeAxis);
					int num3 = Random.Range(0, __instance.levelSizeAxis);

					if (!__instance.mapChunkArray[num2, num3].filled)
					{
						MapChunk mapChunk2 = __instance.mapChunkArray[num2, num3];
						MapChunk mapChunk3 = null;
						MapChunk mapChunk4 = null;
						MapChunk mapChunk5 = null;

						if (mapChunk2.chunkID != 0 && !mapChunk2.filled && (!GC.customLevel || GC.customRandomLevel || mapChunk2.chunkMainTile))
						{
							mapChunk2.filled = true;
							int num4 = 0;
							string text = "Normal";
							string text2 = "Normal";
							int num5 = 0;
							int num6 = 1;
							bool exitN = mapChunk2.exitN;
							bool exitS = mapChunk2.exitS;
							bool exitE = mapChunk2.exitE;
							bool exitW = mapChunk2.exitW;
							int num7 = 0;
							int num8 = 0;
							int num9 = 0;
							int num10 = 0;
							int num11 = 0;
							int num12 = 0;
							int num13 = 0;
							int num14 = 0;

							if (mapChunk2.customLevelChunk || mapChunk2.randomTile)
							{
								if (mapChunk2.chunkShape == 5 || mapChunk2.chunkShape == 6)
								{
									num13 = 1;

									if (mapChunk2.chunkShape == 5)
									{
										if (mapChunk2.chunkRotate == 0)
											num14 = 1;
										else if (mapChunk2.chunkRotate == 1)
											num14 = 2;
									}
									else if (mapChunk2.chunkShape == 6)
										num14 = 4;
								}
							}
							else if (mapChunk2.special == "" && mapChunk2.functionType == "")
							{
								if ((hugeChunks == 0 || (GC.levelTheme == 3 && hugeChunks < 2) || (GC.levelTheme == 4 && hugeChunks < 4) || (GC.levelTheme == 5 && hugeChunks < 2)) && hugeTries < 50)
								{
									num13 = 1;
									num14 = 4;
									num = hugeTries;
									hugeTries = num + 1;
								}
								else if ((longChunks == 0 || (GC.levelTheme == 3 && longChunks < 2) || (GC.levelTheme == 4 && longChunks < 4) || (GC.levelTheme == 5 && longChunks < 2)) && longTries < 50)
								{
									num13 = 1;
									num14 = GC.Choose<int>(0, 1, 2, 3);
									num = longTries;
									longTries = num + 1;
								}
								else if (longChunks >= 3 && hugeChunks >= 2)
									num13 = 0;
								else
								{
									num13 = GC.Choose<int>(0, 0, 0, 1);

									if (longChunks < 3 && hugeChunks < 2)
										num14 = GC.Choose<int>(0, 1, 2, 3, 4);
									else if (longChunks < 3 && hugeChunks >= 2)
										num14 = GC.Choose<int>(0, 1, 2, 3);
									else if (longChunks >= 3 && hugeChunks < 2)
										num14 = 4;
								}
							}

							float num15 = 0f;
							float num16 = 0f;
							bool flag = false;

							if (num13 == 1 && ((num2 > 0 && num3 > 0 && num2 < __instance.levelSizeAxis - 1 && num3 < __instance.levelSizeAxis - 1 && mapChunk2.special == "") || mapChunk2.customLevelChunk || mapChunk2.randomTile))
							{
								if (num14 == 0)
								{
									mapChunk3 = __instance.mapChunkArray[num2, num3 + 1];

									if ((mapChunk2.exitS && !mapChunk3.filled && mapChunk3.special == "" && mapChunk3.functionType == "" && mapChunk2.sector == mapChunk3.sector) || mapChunk2.customLevelChunk || mapChunk2.randomTile)
									{
										num7 = num2;
										num8 = num3 + 1;
										mapChunk3.filled = true;
										__instance.SetMultiChunkID(mapChunk3.chunkID, mapChunk2.chunkID);
										mapChunk3.chunkID = mapChunk2.chunkID;
										num4 = 5;

										if (mapChunk2.customLevelChunk)
										{
											text = mapChunk2.chunkDirX;
											text2 = mapChunk2.chunkDirY;
											num5 = mapChunk2.chunkRotate;
										}
										else
										{
											text = GC.Choose<string>("Normal", "Reverse");
											text2 = GC.Choose<string>("Normal", "Reverse");
											num5 = 0;
										}

										num6 = 1;
										num15 = 0f;
										num16 = -__instance.halfChunkSize;

										if (GC.levelShape == 0)
										{
											mapChunk3.exitN = true;
											mapChunk3.exitS = true;
											mapChunk3.exitE = true;
											mapChunk3.exitW = true;
										}

										mapChunk2.blockedS = true;
										mapChunk3.blockedN = true;
										num = longChunks;
										longChunks = num + 1;
										flag = true;
									}
								}
								else if (num14 == 1)
								{
									mapChunk3 = __instance.mapChunkArray[num2, num3 - 1];

									if ((mapChunk2.exitN && !mapChunk3.filled && mapChunk3.special == "" && mapChunk3.functionType == "" && mapChunk2.sector == mapChunk3.sector) || mapChunk2.customLevelChunk || mapChunk2.randomTile)
									{
										num7 = num2;
										num8 = num3 - 1;
										mapChunk3.filled = true;
										__instance.SetMultiChunkID(mapChunk3.chunkID, mapChunk2.chunkID);
										mapChunk3.chunkID = mapChunk2.chunkID;
										num4 = 5;

										if (mapChunk2.customLevelChunk)
										{
											text = mapChunk2.chunkDirX;
											text2 = mapChunk2.chunkDirY;
											num5 = mapChunk2.chunkRotate;
										}
										else
										{
											text = GC.Choose<string>("Normal", "Reverse");
											text2 = GC.Choose<string>("Normal", "Reverse");
											num5 = 0;
										}

										num6 = 1;
										num15 = 0f;
										num16 = __instance.halfChunkSize;

										if (GC.levelShape == 0)
										{
											mapChunk3.exitN = true;
											mapChunk3.exitS = true;
											mapChunk3.exitE = true;
											mapChunk3.exitW = true;
										}

										mapChunk2.blockedN = true;
										mapChunk3.blockedS = true;
										num = longChunks;
										longChunks = num + 1;
										flag = true;
									}
								}
								else if (num14 == 2)
								{
									mapChunk3 = __instance.mapChunkArray[num2 + 1, num3];

									if ((mapChunk2.exitE && !mapChunk3.filled && mapChunk3.special == "" && mapChunk3.functionType == "" && mapChunk2.sector == mapChunk3.sector) || mapChunk2.customLevelChunk || mapChunk2.randomTile)
									{
										num7 = num2 + 1;
										num8 = num3;
										mapChunk3.filled = true;
										__instance.SetMultiChunkID(mapChunk3.chunkID, mapChunk2.chunkID);
										mapChunk3.chunkID = mapChunk2.chunkID;
										num4 = 5;

										if (mapChunk2.customLevelChunk)
										{
											text = mapChunk2.chunkDirX;
											text2 = mapChunk2.chunkDirY;
											num5 = mapChunk2.chunkRotate;
										}
										else
										{
											text = GC.Choose<string>("Normal", "Reverse");
											text2 = GC.Choose<string>("Normal", "Reverse");
											num5 = 1;
										}

										num6 = 1;
										num15 = __instance.halfChunkSize;
										num16 = 0f;

										if (GC.levelShape == 0)
										{
											mapChunk3.exitN = true;
											mapChunk3.exitS = true;
											mapChunk3.exitE = true;
											mapChunk3.exitW = true;
										}

										mapChunk2.blockedE = true;
										mapChunk3.blockedW = true;
										num = longChunks;
										longChunks = num + 1;
										flag = true;
									}
								}
								else if (num14 == 3)
								{
									mapChunk3 = __instance.mapChunkArray[num2 - 1, num3];

									if ((mapChunk2.exitW && !mapChunk3.filled && mapChunk3.special == "" && mapChunk3.functionType == "" && mapChunk2.sector == mapChunk3.sector) || mapChunk2.customLevelChunk || mapChunk2.randomTile)
									{
										num7 = num2 - 1;
										num8 = num3;
										mapChunk3.filled = true;
										__instance.SetMultiChunkID(mapChunk3.chunkID, mapChunk2.chunkID);
										mapChunk3.chunkID = mapChunk2.chunkID;
										num4 = 5;

										if (mapChunk2.customLevelChunk)
										{
											text = mapChunk2.chunkDirX;
											text2 = mapChunk2.chunkDirY;
											num5 = mapChunk2.chunkRotate;
										}
										else
										{
											text = GC.Choose<string>("Normal", "Reverse");
											text2 = GC.Choose<string>("Normal", "Reverse");
											num5 = 1;
										}

										num6 = 1;
										num15 = -__instance.halfChunkSize;
										num16 = 0f;

										if (GC.levelShape == 0)
										{
											mapChunk3.exitN = true;
											mapChunk3.exitS = true;
											mapChunk3.exitE = true;
											mapChunk3.exitW = true;
										}

										mapChunk2.blockedW = true;
										mapChunk3.blockedE = true;
										num = longChunks;
										longChunks = num + 1;
										flag = true;
									}
								}
								else if (num14 == 4)
								{
									if (mapChunk2.customLevelChunk || mapChunk2.randomTile)
									{
										mapChunk3 = __instance.mapChunkArray[num2 + 1, num3];
										mapChunk4 = __instance.mapChunkArray[num2, num3 - 1];
										mapChunk5 = __instance.mapChunkArray[num2 + 1, num3 - 1];
									}
									else
									{
										mapChunk3 = __instance.mapChunkArray[num2 + 1, num3];
										mapChunk4 = __instance.mapChunkArray[num2, num3 + 1];
										mapChunk5 = __instance.mapChunkArray[num2 + 1, num3 + 1];
									}
									if ((mapChunk2.exitS && !mapChunk3.filled && mapChunk3.special == "" && 
										mapChunk3.functionType == "" && 
										mapChunk2.sector == mapChunk3.sector && 
										!mapChunk4.filled && mapChunk4.special == "" && 
										mapChunk4.functionType == "" && 
										mapChunk2.sector == mapChunk4.sector && 
										!mapChunk5.filled && mapChunk5.special == "" && 
										mapChunk5.functionType == "" && 
										mapChunk2.sector == mapChunk5.sector) || mapChunk2.customLevelChunk || mapChunk2.randomTile)
									{
										if (mapChunk2.customLevelChunk || mapChunk2.randomTile)
										{
											num7 = num2 + 1;
											num8 = num3;
											num9 = num2;
											num10 = num3 - 1;
											num11 = num2 + 1;
											num12 = num3 - 1;
										}
										else
										{
											num7 = num2 + 1;
											num8 = num3;
											num9 = num2;
											num10 = num3 + 1;
											num11 = num2 + 1;
											num12 = num3 + 1;
										}

										__instance.SetMultiChunkID(mapChunk3.chunkID, mapChunk2.chunkID);
										__instance.SetMultiChunkID(mapChunk4.chunkID, mapChunk2.chunkID);
										__instance.SetMultiChunkID(mapChunk5.chunkID, mapChunk2.chunkID);
										mapChunk3.filled = true;
										mapChunk3.chunkID = mapChunk2.chunkID;
										mapChunk4.filled = true;
										mapChunk4.chunkID = mapChunk2.chunkID;
										mapChunk5.filled = true;
										mapChunk5.chunkID = mapChunk2.chunkID;
										num4 = 6;

										if (mapChunk2.customLevelChunk)
										{
											text = mapChunk2.chunkDirX;
											text2 = mapChunk2.chunkDirY;
											num5 = mapChunk2.chunkRotate;
										}
										else
										{
											text = GC.Choose<string>("Normal", "Reverse");
											text2 = GC.Choose<string>("Normal", "Reverse");
											num5 = GC.Choose<int>(0, 1);
										}

										num6 = 1;
										num15 = __instance.halfChunkSize;

										if (mapChunk2.customLevelChunk || mapChunk2.randomTile)
											num16 = __instance.halfChunkSize;
										else
											num16 = -__instance.halfChunkSize;
										
										if (GC.levelShape == 0)
										{
											mapChunk3.exitN = true;
											mapChunk3.exitS = true;
											mapChunk3.exitE = true;
											mapChunk3.exitW = true;
											mapChunk4.exitN = true;
											mapChunk4.exitS = true;
											mapChunk4.exitE = true;
											mapChunk4.exitW = true;
											mapChunk5.exitN = true;
											mapChunk5.exitS = true;
											mapChunk5.exitE = true;
											mapChunk5.exitW = true;
										}
										else
										{
											mapChunk2.exitE = true;
											mapChunk2.exitS = true;
											mapChunk3.exitW = true;
											mapChunk3.exitS = true;
											mapChunk4.exitN = true;
											mapChunk4.exitE = true;
											mapChunk5.exitN = true;
											mapChunk5.exitW = true;
										}

										mapChunk2.blockedS = true;
										mapChunk2.blockedE = true;
										mapChunk3.blockedS = true;
										mapChunk3.blockedW = true;
										mapChunk4.blockedN = true;
										mapChunk4.blockedE = true;
										mapChunk5.blockedN = true;
										mapChunk5.blockedW = true;
										num = hugeChunks;
										hugeChunks = num + 1;
										flag = true;
									}
								}

								if (num6 == 0)
									num13 = 0;
								else if (GC.levelShape == 0)
								{
									mapChunk2.exitN = true;
									mapChunk2.exitS = true;
									mapChunk2.exitE = true;
									mapChunk2.exitW = true;
								}
							}

							if (!flag && num13 != 0)
								mapChunk2.filled = false;
							else
							{
								if (num13 == 0)
								{
									if (mapChunk2.customLevelChunk)
									{
										num4 = 0;
										text = mapChunk2.chunkDirX;
										text2 = mapChunk2.chunkDirY;
										num5 = mapChunk2.chunkRotate;
									}
									else if (exitN && exitS && exitE && exitW)
									{
										num4 = 0;
										text = GC.Choose<string>("Normal", "Reverse");
										text2 = GC.Choose<string>("Normal", "Reverse");
										num5 = GC.Choose<int>(0, 1);
									}
									else if (!exitN && exitS && exitE && exitW)
									{
										if (GC.levelShape == 2)
											num4 = 1;
										else if (__instance.squareMap)
											num4 = GC.Choose<int>(0, 0, 1);
										else
											num4 = GC.Choose<int>(0, 0, 1);
										
										text = GC.Choose<string>("Normal", "Reverse");
										text2 = "Normal";
										
										if (num4 == 0)
										{
											mapChunk2.exitN = true;
											text2 = GC.Choose<string>("Normal", "Reverse");
											num5 = GC.Choose<int>(0, 1);
										}
									}
									else if (exitN && !exitS && exitE && exitW)
									{
										if (GC.levelShape == 2)
											num4 = 1;
										else if (__instance.squareMap)
											num4 = GC.Choose<int>(0, 0, 1);
										else
											num4 = GC.Choose<int>(0, 0, 1);
										
										text = GC.Choose<string>("Normal", "Reverse");
										text2 = "Reverse";
										
										if (num4 == 0)
										{
											mapChunk2.exitS = true;
											text2 = GC.Choose<string>("Normal", "Reverse");
											num5 = GC.Choose<int>(0, 1);
										}
									}
									else if (exitN && exitS && !exitE && exitW)
									{
										if (GC.levelShape == 2)
											num4 = 1;
										else if (__instance.squareMap)
											num4 = GC.Choose<int>(0, 0, 1);
										else
											num4 = GC.Choose<int>(0, 0, 1);
										
										text = GC.Choose<string>("Normal", "Reverse");
										text2 = "Reverse";
										num5 = 1;
										
										if (num4 == 0)
										{
											mapChunk2.exitE = true;
											text2 = GC.Choose<string>("Normal", "Reverse");
											num5 = GC.Choose<int>(0, 1);
										}
									}
									else if (exitN && exitS && exitE && !exitW)
									{
										if (GC.levelShape == 2)
											num4 = 1;
										else if (__instance.squareMap)
											num4 = GC.Choose<int>(0, 0, 1);
										else
											num4 = GC.Choose<int>(0, 0, 1);

										text = GC.Choose<string>("Normal", "Reverse");
										text2 = "Normal";
										num5 = 1;
										
										if (num4 == 0)
										{
											mapChunk2.exitW = true;
											text2 = GC.Choose<string>("Normal", "Reverse");
											num5 = GC.Choose<int>(0, 1);
										}
									}
									else if (!exitN && !exitS && exitE && exitW)
									{
										if (GC.levelShape == 2)
											num4 = 2;
										else if (__instance.squareMap)
											num4 = GC.Choose<int>(0, 0, 1);
										else
											num4 = 0;
										
										text = GC.Choose<string>("Normal", "Reverse");
										text2 = "Normal";
										num5 = 1;
										
										if (num4 == 0)
										{
											mapChunk2.exitN = true;
											mapChunk2.exitS = true;
											text2 = GC.Choose<string>("Normal", "Reverse");
											num5 = GC.Choose<int>(0, 1);
										}
									}
									else if (exitN && exitS && !exitE && !exitW)
									{
										if (GC.levelShape == 2)
											num4 = 2;
										else if (__instance.squareMap)
											num4 = GC.Choose<int>(0, 0, 1);
										else
											num4 = GC.Choose<int>(0, 0, 2);

										text = GC.Choose<string>("Normal", "Reverse");
										text2 = GC.Choose<string>("Normal", "Reverse");

										if (num4 == 0)
										{
											mapChunk2.exitE = true;
											mapChunk2.exitW = true;
											num5 = GC.Choose<int>(0, 1);
										}
									}
									else if (!exitN && exitS && exitE && !exitW)
									{
										if (GC.levelShape == 2)
											num4 = 3;
										else if (__instance.squareMap)
											num4 = 0;
										else
											num4 = GC.Choose<int>(0, 1, 3, 3);
										
										text = "Normal";
										text2 = "Normal";
										
										if (num4 == 0)
										{
											mapChunk2.exitN = true;
											mapChunk2.exitW = true;
											text = GC.Choose<string>("Normal", "Reverse");
											text2 = GC.Choose<string>("Normal", "Reverse");
											num5 = GC.Choose<int>(0, 1);
										}
										else if (num4 == 1)
										{
											mapChunk2.exitW = true;
											text = GC.Choose<string>("Normal", "Reverse");
										}
									}
									else if (!exitN && exitS && !exitE && exitW)
									{
										if (GC.levelShape == 2)
											num4 = 3;
										else if (__instance.squareMap)
											num4 = 0;
										else
											num4 = GC.Choose<int>(0, 1, 3, 3);
										
										text = "Reverse";
										text2 = "Normal";
										
										if (num4 == 0)
										{
											mapChunk2.exitN = true;
											mapChunk2.exitE = true;
											text = GC.Choose<string>("Normal", "Reverse");
											text2 = GC.Choose<string>("Normal", "Reverse");
											num5 = GC.Choose<int>(0, 1);
										}
										else if (num4 == 1)
										{
											mapChunk2.exitE = true;
											text = GC.Choose<string>("Normal", "Reverse");
										}
									}
									else if (exitN && !exitS && exitE && !exitW)
									{
										if (GC.levelShape == 2)
											num4 = 3;
										else if (__instance.squareMap)
											num4 = 0;
										else
											num4 = GC.Choose<int>(0, 1, 3, 3);

										text = "Normal";
										text2 = "Reverse";

										if (num4 == 0)
										{
											mapChunk2.exitS = true;
											mapChunk2.exitW = true;
											text = GC.Choose<string>("Normal", "Reverse");
											text2 = GC.Choose<string>("Normal", "Reverse");
											num5 = GC.Choose<int>(0, 1);
										}
										else if (num4 == 1)
										{
											mapChunk2.exitW = true;
											text = GC.Choose<string>("Normal", "Reverse");
										}
									}
									else if (exitN && !exitS && !exitE && exitW)
									{
										if (GC.levelShape == 2)
											num4 = 3;
										else if (__instance.squareMap)
											num4 = 0;
										else
											num4 = GC.Choose<int>(0, 1, 3, 3);
										
										text = "Reverse";
										text2 = "Reverse";
										
										if (num4 == 0)
										{
											mapChunk2.exitS = true;
											mapChunk2.exitE = true;
											text = GC.Choose<string>("Normal", "Reverse");
											text2 = GC.Choose<string>("Normal", "Reverse");
											num5 = GC.Choose<int>(0, 1);
										}
										else if (num4 == 1)
										{
											mapChunk2.exitE = true;
											text = GC.Choose<string>("Normal", "Reverse");
										}
									}
									else if (!exitN && exitS && !exitE && !exitW)
									{
										if (GC.levelShape == 2)
											num4 = 4;
										else if (__instance.squareMap)
											num4 = 0;
										else
											num4 = GC.Choose<int>(1, 2, 2, 3, 4, 4);

										text = GC.Choose<string>("Normal", "Reverse");
										text2 = "Normal";

										if (num4 == 1)
										{
											mapChunk2.exitE = true;
											mapChunk2.exitW = true;
										}
										else if (num4 == 2)
										{
											mapChunk2.exitN = true;
											text2 = GC.Choose<string>("Normal", "Reverse");
										}
										else if (num4 == 3)
										{
											if (text == "Normal")
												mapChunk2.exitE = true;
											else if (text == "Reverse")
												mapChunk2.exitW = true;
										}
									}
									else if (exitN && !exitS && !exitE && !exitW)
									{
										if (GC.levelShape == 2)
											num4 = 4;
										else if (__instance.squareMap)
											num4 = 0;
										else
											num4 = GC.Choose<int>(1, 2, 2, 3, 4, 4);
										
										text = GC.Choose<string>("Normal", "Reverse");
										text2 = "Reverse";
										
										if (num4 == 1)
										{
											mapChunk2.exitE = true;
											mapChunk2.exitW = true;
										}
										else if (num4 == 2)
										{
											mapChunk2.exitS = true;
											text2 = GC.Choose<string>("Normal", "Reverse");
										}
										else if (num4 == 3)
										{
											if (text == "Normal")
												mapChunk2.exitE = true;
											else if (text == "Reverse")
												mapChunk2.exitW = true;
										}
									}
									else if (!exitN && !exitS && exitE && !exitW)
									{
										if (GC.levelShape == 2)
											num4 = 4;
										else if (__instance.squareMap)
											num4 = 0;
										else
											num4 = GC.Choose<int>(1, 2, 2, 3, 4, 4);

										text = GC.Choose<string>("Normal", "Reverse");
										text2 = "Normal";
										num5 = 1;
										
										if (num4 == 1)
										{
											mapChunk2.exitN = true;
											mapChunk2.exitS = true;
										}
										else if (num4 == 2)
										{
											mapChunk2.exitW = true;
											text2 = GC.Choose<string>("Normal", "Reverse");
										}
										else if (num4 == 3)
										{
											if (text2 == "Normal")
												mapChunk2.exitS = true;
											else if (text2 == "Reverse")
												mapChunk2.exitN = true;
										}
									}
									else if (!exitN && !exitS && !exitE && exitW)
									{
										if (GC.levelShape == 2)
											num4 = 4;
										else if (__instance.squareMap)
											num4 = 0;
										else
											num4 = GC.Choose<int>(1, 2, 2, 3, 4, 4);
										
										text = GC.Choose<string>("Normal", "Reverse");
										text2 = "Reverse";
										num5 = 1;

										if (num4 == 1)
										{
											mapChunk2.exitN = true;
											mapChunk2.exitS = true;
										}
										else if (num4 == 2)
										{
											mapChunk2.exitE = true;
											text2 = GC.Choose<string>("Normal", "Reverse");
										}
										else if (num4 == 3)
										{
											if (text2 == "Normal")
												mapChunk2.exitS = true;
											else if (text2 == "Reverse")
												mapChunk2.exitN = true;
										}
									}
								}

								if (mapChunk2.special == "LevelStart")
								{
									if (__instance.LevelContainsMayor())
										num4 = 21;
									else
										num4 = 10;
									
									mapChunk2.exitN = true;
									mapChunk2.exitS = false;
									mapChunk2.exitE = false;
									mapChunk2.exitW = false;
									text = GC.Choose<string>("Normal", "Reverse");
									text2 = "Reverse";
									num5 = 0;
								}
								else if (mapChunk2.special == "LevelEnd")
								{
									num4 = 11;
									mapChunk2.exitN = false;
									mapChunk2.exitS = true;
									mapChunk2.exitE = false;
									mapChunk2.exitW = false;
									text = GC.Choose<string>("Normal", "Reverse");
									text2 = "Normal";
									num5 = 0;
								}
								else if (mapChunk2.special == "Canal")
								{
									num4 = 12;
									text = "Normal";
									text2 = "Normal";
									num5 = 0;
								}
								else if (mapChunk2.special == "CanalBridge")
								{
									num4 = 13;
									text = "Normal";
									text2 = "Normal";
									num5 = 0;
								}
								else if (mapChunk2.special == "CanalEndE")
								{
									num4 = 14;
									text = "Normal";
									text2 = "Normal";
									num5 = 0;
								}
								else if (mapChunk2.special == "CanalEndW")
								{
									num4 = 15;
									text = "Normal";
									text2 = "Normal";
									num5 = 0;
								}
								else if (mapChunk2.special == "CanalCross")
								{
									num4 = 16;
									text = "Normal";
									text2 = "Normal";
									num5 = 0;
								}
								else if (mapChunk2.special == "CanalNS")
								{
									num4 = 17;
									text = "Normal";
									text2 = "Normal";
									num5 = 0;
								}
								else if (mapChunk2.special == "CanalEndN")
								{
									num4 = 18;
									text = "Normal";
									text2 = "Normal";
									num5 = 0;
								}
								else if (mapChunk2.special == "CanalEndS")
								{
									num4 = 19;
									text = "Normal";
									text2 = "Normal";
									num5 = 0;
								}
								else if (mapChunk2.special == "CanalBridgeNS")
								{
									num4 = 20;
									text = "Normal";
									text2 = "Normal";
									num5 = 0;
								}

								if (GC.levelTheme == 2)
								{
									if (num4 >= 1 && num4 <= 4)
										num4 = 0;
								}
								else if (GC.levelTheme == 3)
								{
									if (num3 < 9)
									{
										if (__instance.mapChunkArray[num2, num3 + 1].special.Contains("Canal") && __instance.mapChunkArray[num2, num3].special == "")
											num4 = 0;
									}
									else if (num3 > 0 && __instance.mapChunkArray[num2, num3 - 1].special.Contains("Canal") && __instance.mapChunkArray[num2, num3].special == "")
										num4 = 0;
								
									if (num2 < 9)
									{
										if (__instance.mapChunkArray[num2 + 1, num3].special.Contains("Canal") && __instance.mapChunkArray[num2, num3].special == "")
											num4 = 0;
									}
									else if (num2 > 0 && __instance.mapChunkArray[num2 - 1, num3].special.Contains("Canal") && __instance.mapChunkArray[num2, num3].special == "")
									{
										num4 = 0;
									}
								}
								else if (GC.levelTheme == 4)
								{
									if (num4 >= 1 && num4 <= 4)
										num4 = 0;
								}
								else if (GC.levelTheme == 5 && num4 >= 1 && num4 <= 4)
									num4 = 0;
								
								GameObject gameObject = null;
								ChunkData chunkData = null;
								
								if (GC.basicSpawns)
									chunkData = ___chunkListBasic[0];
								
								bool flag2 = false;
								bool flag3 = false;
								Vector3 vector = new Vector2(__instance.chunkSize * (float)num2 - __instance.halfTileSize + __instance.halfChunkSize + num15, -__instance.chunkSize * (float)num3 + 102.4f + __instance.halfTileSize - __instance.halfChunkSize - __instance.tileSize + num16);
								int num17 = 0;
								bool flag5 = false;
								bool flag6 = false;
								bool flag7 = false;
								bool flag8 = false;

								while (!flag2)
								{
									List<ChunkData> list = ___chunkListBasic;

									if (num17 > 50 && GC.sessionDataBig.useOnlyChunkPackChunks)
										list = ___chunkListBasicBackup;
									
									if (mapChunk2.customLevelChunk)
									{
										bool flag9 = false;
									
										foreach (ChunkData chunkData2 in list)
											if (chunkData2.chunkName == mapChunk2.chunkName)
											{
												chunkData = chunkData2;
												flag9 = true;
										
												break;
											}
										
										if (!flag9)
											foreach (ChunkData chunkData3 in __instance.customLevel.chunkList)
												if (chunkData3.chunkName == mapChunk2.chunkName)
												{
													chunkData = chunkData3;
													flag9 = true;

													break;
												}
										
										flag2 = true;
									}
									else if (GC.customLevel && !GC.customRandomLevel && !mapChunk2.customLevelChunk && !mapChunk2.randomTile)
									{
										flag2 = true;
										flag3 = true;
									}
									else if (GC.basicSpawns)
									{
										int num18 = Random.Range(0, list.Count);
										ChunkData chunkData4 = list[num18];
										bool flag10 = false;

										if (GC.levelTheme == 4 || GC.challenges.Contains(cChallenge.PoliceState))
										{
											if (!__instance.placedDeportationCenter && !flag10)
											{
												List<ChunkData> list2 = new List<ChunkData>();
										
												for (int k = 0; k < list.Count; k++)
													if (list[k].description == vChunkType.DeportationCenter && num4 == list[k].shape)
														list2.Add(list[k]);
												
												if (list2.Count > 0)
												{
													chunkData4 = list2[Random.Range(0, list2.Count)];
													flag10 = true;
												}
											}
											if (!__instance.placedConfiscationCenter && !flag5 && !flag10)
											{
												flag5 = true;
												List<ChunkData> list3 = new List<ChunkData>();

												for (int l = 0; l < list.Count; l++)
													if (list[l].description == vChunkType.ConfiscationCenter && num4 == list[l].shape)
														list3.Add(list[l]);
												
												if (list3.Count > 0)
												{
													chunkData4 = list3[Random.Range(0, list3.Count)];
													flag10 = true;
												}
											}
										}
										else if (GC.levelTheme == 5)
										{
											if (!__instance.placedPodiumPark && __instance.LevelContainsMayor() && !flag6 && !flag10)
											{
												flag6 = true;
												List<ChunkData> list4 = new List<ChunkData>();

												for (int m = 0; m < list.Count; m++)
													if (list[m].description == vChunkType.PodiumPark && num4 == list[m].shape)
													{
														list4.Add(list[m]);
														text = "Normal";
														text2 = "Normal";

														if (num4 == 0 || num4 == 6)
															num5 = 0;
													}
												
												if (list4.Count > 0)
												{
													chunkData4 = list4[Random.Range(0, list4.Count)];
													flag10 = true;
												}
											}

											if (!__instance.placedMayorOffice && __instance.LevelContainsMayor() && !flag7 && !flag10)
											{
												flag7 = true;
												List<ChunkData> list5 = new List<ChunkData>();

												for (int n = 0; n < list.Count; n++)
													if (list[n].description == vChunkType.MayorOffice && num4 == list[n].shape)
														list5.Add(list[n]);
												
												if (list5.Count > 0)
												{
													chunkData4 = list5[Random.Range(0, list5.Count)];
													flag10 = true;
												}
											}

											if (!__instance.placedMayorHouse && __instance.LevelContainsMayor() && !flag8 && !flag10)
											{
												flag8 = true;
												List<ChunkData> list6 = new List<ChunkData>();

												for (int num19 = 0; num19 < list.Count; num19++)
													if (list[num19].description == vChunkType.MayorHouse && num4 == list[num19].shape)
														list6.Add(list[num19]);
												
												if (list6.Count > 0)
												{
													chunkData4 = list6[Random.Range(0, list6.Count)];
													flag10 = true;
												}
											}
										}
										
										if (flag10)
											flag2 = true;
										else if (chunkData4 == null)
											flag2 = false;
										else if (num17 < 500 && chunkData4.shape == num4 && __instance.CanUseChunk(null, chunkData4, true, num18, num4, vector) && chunkData4.functionType == mapChunk2.functionType)
											flag2 = true;
										else if (num17 >= 500 && chunkData4.shape == num4 && __instance.CanUseChunk(null, chunkData4, false, num18, num4, vector) && chunkData4.functionType == mapChunk2.functionType)
											flag2 = true;
										
										if (flag2)
										{
											chunkData = chunkData4;
										
											if (!GC.sessionData.usedChunks.Contains(num18))
												GC.sessionData.usedChunks.Add(num18);
											
											__instance.usedChunksThisLevelBasic.Add(chunkData);
										}

										num17++;
									}
									else
									{
										int num20 = Random.Range(0, ___chunkList.Count);
										GameObject gameObject2 = ___chunkList[num20];

										if (GC.levelTheme == 4 || GC.challenges.Contains(cChallenge.PoliceState))
										{
											if (num4 == 0 && !__instance.placedDeportationCenter)
											{
												for (int num21 = 0; num21 < ___chunkList.Count; num21++)
													if (___chunkList[num21].GetComponent<Chunk>().description == vChunkType.DeportationCenter)
														gameObject2 = ___chunkList[num21];
											}
											else if (num4 == 0 && !__instance.placedConfiscationCenter)
												for (int num22 = 0; num22 < ___chunkList.Count; num22++)
													if (___chunkList[num22].GetComponent<Chunk>().description == vChunkType.ConfiscationCenter)
														gameObject2 = ___chunkList[num22];
										}
										else if (GC.levelTheme == 5)
										{
											if (num4 == 0 && !__instance.placedPodiumPark && __instance.LevelContainsMayor())
											{
												for (int num23 = 0; num23 < ___chunkList.Count; num23++)
													if (___chunkList[num23].GetComponent<Chunk>().description == vChunkType.PodiumPark)
													{
														gameObject2 = ___chunkList[num23];
														text = "Normal";
														text2 = "Normal";
														num5 = 0;
													}
											}
											else if (num4 == 6 && !__instance.placedMayorOffice && __instance.LevelContainsMayor())
											{
												for (int num24 = 0; num24 < ___chunkList.Count; num24++)
													if (___chunkList[num24].GetComponent<Chunk>().description == vChunkType.MayorOffice)
														gameObject2 = ___chunkList[num24];
											}
											else if (num4 == 6 && !__instance.placedMayorHouse && __instance.LevelContainsMayor())
												for (int num25 = 0; num25 < ___chunkList.Count; num25++)
													if (___chunkList[num25].GetComponent<Chunk>().description == vChunkType.MayorHouse)
														gameObject2 = ___chunkList[num25];
										}

										Chunk component = gameObject2.GetComponent<Chunk>();

										if (gameObject2 == null)
											flag2 = false;
										else if (num17 < 500 && component.shape == num4 && __instance.CanUseChunk(gameObject2.gameObject, null, true, num20, num4, vector) && component.functionType == mapChunk2.functionType)
											flag2 = true;
										else if (num17 >= 500 && component.shape == num4 && __instance.CanUseChunk(gameObject2.gameObject, null, false, num20, num4, vector) && component.functionType == mapChunk2.functionType)
											flag2 = true;
										
										if (flag2)
										{
											gameObject = gameObject2;
										
											if (!GC.sessionData.usedChunks.Contains(num20))
												GC.sessionData.usedChunks.Add(num20);
											
											__instance.usedChunksThisLevel.Add(gameObject);
										}

										num17++;
									}
								}

								if (!flag3)
								{
									GameObject gameObject3 = null;
									Chunk chunk;

									if (GC.basicSpawns)
									{
										gameObject3 = Object.Instantiate<GameObject>(GC.readChunks.chunkPrefab, __instance.transform.position, __instance.transform.rotation);
										Chunk component2 = gameObject3.GetComponent<Chunk>();
										gameObject3.name = chunkData.chunkName;
										component2.shape = chunkData.shape;
										component2.description = chunkData.description;
										component2.functionType = chunkData.functionType;
										component2.spawnerList = chunkData.spawnerList;

										try
										{
											component2.userCreated = chunkData.userCreated;
										}
										catch { }

										try
										{
											component2.customCharacterList = chunkData.customCharacterList;
										}
										catch { }
										
										chunk = component2;

										for (int num26 = 0; num26 < component2.spawnerList.Count; num26++)
										{
											SpawnerBasic spawnerBasic = new SpawnerBasic();
											GC.readChunks.CopySpawnerData(component2.spawnerList[num26], spawnerBasic);
											SpawnerBasic spawnerBasic2 = spawnerBasic;
											GameController gameController = GC;
											num = gameController.spawnerCount;
											gameController.spawnerCount = num + 1;
											spawnerBasic2.spawnerID = num;
											GC.spawnerBasicList.Add(spawnerBasic);
											GC.loadLevel.spawnersBasic.Add(spawnerBasic);
											GC.loadLevel.startingChunkReals.Add(chunk);
											spawnerBasic.startingChunk = mapChunk2.chunkID;
										}
									}
									else
									{
										chunk = gameObject.GetComponent<Chunk>();
										gameObject3 = Object.Instantiate<GameObject>(gameObject, __instance.transform.position, __instance.transform.rotation);
									}

									Vector3 position = gameObject3.transform.position;
									Vector2 v = new Vector2(
										position.x + __instance.chunkSize * (float)num2 - __instance.halfTileSize + __instance.halfChunkSize, 
										position.y - __instance.chunkSize * (float)num3 + 102.4f + __instance.halfTileSize - __instance.halfChunkSize - __instance.tileSize);
									Vector2 v2 = new Vector2(
										position.x + __instance.chunkSize * (float)num7 - __instance.halfTileSize + __instance.halfChunkSize, 
										position.y - __instance.chunkSize * (float)num8 + 102.4f + __instance.halfTileSize - __instance.halfChunkSize - __instance.tileSize);
									Vector2 v3 = new Vector2(
										position.x + __instance.chunkSize * (float)num9 - __instance.halfTileSize + __instance.halfChunkSize, 
										position.y - __instance.chunkSize * (float)num10 + 102.4f + __instance.halfTileSize - __instance.halfChunkSize - __instance.tileSize);
									Vector2 v4 = new Vector2(
										position.x + __instance.chunkSize * (float)num11 - __instance.halfTileSize + __instance.halfChunkSize, 
										position.y - __instance.chunkSize * (float)num12 + 102.4f + __instance.halfTileSize - __instance.halfChunkSize - __instance.tileSize);
									gameObject3.transform.position = vector;
									gameObject3.name = string.Concat(new object[] { gameObject3.name, ".", num2, ".", num3, " (", mapChunk2.chunkID, ") ", mapChunk2.special, " | ", chunk.functionType, " | ", chunk.description });
									gameObject3.transform.SetParent(GC.chunksNest.transform);
									gameObject3.transform.localScale = new Vector3(0.01f, 0.01f, 1f);

									foreach (object obj in gameObject3.transform)
									{
										Transform transform = (Transform)obj;

										if (transform.GetComponent<Scenario>())
											transform.gameObject.SetActive(true);
									}

									Chunk component3 = gameObject3.GetComponent<Chunk>();
									Transform transform2 = gameObject3.transform;
									Vector3 position2 = transform2.position;
									__instance.levelChunks.Add(component3);
									component3.chunkID = mapChunk2.chunkID;
									component3.sector = mapChunk2.sector;
									component3.chunkX = num2;
									component3.chunkY = num3;
									component3.chunkPos = gameObject3.transform.position;

									if (GC.customLevel)
										component3.specificQuest = mapChunk2.specificQuest;
									
									if (num13 == 0)
									{
										component3.chunkLeash1 = Object.Instantiate<GameObject>(GC.chunkLeashPrefab, position2, Quaternion.identity, transform2);
										component3.chunkSize = "Normal";
									}
									else if (num14 < 4)
									{
										component3.chunkLeash1 = Object.Instantiate<GameObject>(GC.chunkLeashPrefab, gameObject3.transform.position, Quaternion.identity, transform2);
										component3.chunkLeash2 = Object.Instantiate<GameObject>(GC.chunkLeashPrefab, new Vector2(position2.x, position2.y + __instance.halfChunkSize), Quaternion.identity, transform2);
										component3.chunkLeash3 = Object.Instantiate<GameObject>(GC.chunkLeashPrefab, new Vector2(position2.x, position2.y - __instance.halfChunkSize), Quaternion.identity, transform2);
										component3.chunkSize = "Long";
									}
									else if (num14 == 4)
									{
										component3.chunkLeash1 = Object.Instantiate<GameObject>(GC.chunkLeashPrefab, new Vector2(position2.x + __instance.halfChunkSize, position2.y + __instance.halfChunkSize), Quaternion.identity, transform2);
										component3.chunkLeash2 = Object.Instantiate<GameObject>(GC.chunkLeashPrefab, new Vector2(position2.x + __instance.halfChunkSize, position2.y - __instance.halfChunkSize), Quaternion.identity, transform2);
										component3.chunkLeash3 = Object.Instantiate<GameObject>(GC.chunkLeashPrefab, new Vector2(position2.x - __instance.halfChunkSize, position2.y + __instance.halfChunkSize), Quaternion.identity, transform2);
										component3.chunkLeash4 = Object.Instantiate<GameObject>(GC.chunkLeashPrefab, new Vector2(position2.x - __instance.halfChunkSize, position2.y - __instance.halfChunkSize), Quaternion.identity, transform2);
										component3.chunkSize = "Huge";
									}
									
									if (num5 == 1)
									{
										gameObject3.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
										component3.chunkRotate = num5;
									}
						
									if (text == "Reverse")
									{
										gameObject3.transform.localScale = new Vector3(-0.01f, 0.01f, 1f);
										component3.chunkDirX = text;
									}
									
									if (text2 == "Reverse")
									{
										gameObject3.transform.localScale = new Vector3(gameObject3.transform.localScale.x, -0.01f, 1f);
										component3.chunkDirY = text2;
									}
									
									if (GC.levelShape == 2)
									{
										bool flag11 = num13 != 0 || GC.percentChance(30);

										if (flag11)
										{
											if (!mapChunk2.exitN)
											{
												GameObject gameObject4 = __instance.CreateSingleWall("WallHorizontal", gameObject3, v);
												gameObject4.transform.position = new Vector2(gameObject4.transform.position.x, gameObject4.transform.position.y + 4.8f);
											}
											else if (!mapChunk2.blockedN && !__instance.mapChunkArray[num2, num3 - 1].doorS)
											{
												GameObject gameObject5 = __instance.CreateSingleWall("WallHorizontalSplit", gameObject3, v);
												gameObject5.transform.position = new Vector2(gameObject5.transform.position.x, gameObject5.transform.position.y + 4.8f);
												__instance.mapChunkArray[num2, num3].doorN = true;
											}
										
											if (num13 != 0)
											{
												if (!mapChunk3.exitN)
												{
													GameObject gameObject6 = __instance.CreateSingleWall("WallHorizontal", gameObject3, v2);
													gameObject6.transform.position = new Vector2(gameObject6.transform.position.x, gameObject6.transform.position.y + 4.8f);
												}
												else if (!mapChunk3.blockedN && !__instance.mapChunkArray[num7, num8 - 1].doorS)
												{
													GameObject gameObject7 = __instance.CreateSingleWall("WallHorizontalSplit", gameObject3, v2);
													gameObject7.transform.position = new Vector2(gameObject7.transform.position.x, gameObject7.transform.position.y + 4.8f);
													__instance.mapChunkArray[num7, num8].doorN = true;
												}

												if (num14 == 4)
												{
													if (!mapChunk4.exitN)
													{
														GameObject gameObject8 = __instance.CreateSingleWall("WallHorizontal", gameObject3, v3);
														gameObject8.transform.position = new Vector2(gameObject8.transform.position.x, gameObject8.transform.position.y + 4.8f);
													}
													else if (!mapChunk4.blockedN && !__instance.mapChunkArray[num9, num10 - 1].doorS)
													{
														GameObject gameObject9 = __instance.CreateSingleWall("WallHorizontalSplit", gameObject3, v3);
														gameObject9.transform.position = new Vector2(gameObject9.transform.position.x, gameObject9.transform.position.y + 4.8f);
														__instance.mapChunkArray[num9, num10].doorN = true;
													}

													if (!mapChunk5.exitN)
													{
														GameObject gameObject10 = __instance.CreateSingleWall("WallHorizontal", gameObject3, v4);
														gameObject10.transform.position = new Vector2(gameObject10.transform.position.x, gameObject10.transform.position.y + 4.8f);
													}
													else if (!mapChunk5.blockedN && !__instance.mapChunkArray[num11, num12 - 1].doorS)
													{
														GameObject gameObject11 = __instance.CreateSingleWall("WallHorizontalSplit", gameObject3, v4);
														gameObject11.transform.position = new Vector2(gameObject11.transform.position.x, gameObject11.transform.position.y + 4.8f);
														__instance.mapChunkArray[num11, num12].doorN = true;
													}
												}
											}
										}

										flag11 = (num13 != 0 || GC.percentChance(30));

										if (flag11)
										{
											if (!mapChunk2.exitS)
											{
												GameObject gameObject12 = __instance.CreateSingleWall("WallHorizontal", gameObject3, v);
												gameObject12.transform.position = new Vector2(gameObject12.transform.position.x, gameObject12.transform.position.y - 4.8f);
											}
											else if (!mapChunk2.blockedS && !__instance.mapChunkArray[num2, num3 + 1].doorN)
											{
												GameObject gameObject13 = __instance.CreateSingleWall("WallHorizontalSplit", gameObject3, v);
												gameObject13.transform.position = new Vector2(gameObject13.transform.position.x, gameObject13.transform.position.y - 4.8f);
												__instance.mapChunkArray[num2, num3].doorS = true;
											}

											if (num13 != 0)
											{
												if (!mapChunk3.exitS)
												{
													GameObject gameObject14 = __instance.CreateSingleWall("WallHorizontal", gameObject3, v2);
													gameObject14.transform.position = new Vector2(gameObject14.transform.position.x, gameObject14.transform.position.y - 4.8f);
												}
												else if (!mapChunk3.blockedS && !__instance.mapChunkArray[num7, num8 + 1].doorN)
												{
													GameObject gameObject15 = __instance.CreateSingleWall("WallHorizontalSplit", gameObject3, v2);
													gameObject15.transform.position = new Vector2(gameObject15.transform.position.x, gameObject15.transform.position.y - 4.8f);
													__instance.mapChunkArray[num7, num8].doorS = true;
												}

												if (num14 == 4)
												{
													if (!mapChunk4.exitS)
													{
														GameObject gameObject16 = __instance.CreateSingleWall("WallHorizontal", gameObject3, v3);
														gameObject16.transform.position = new Vector2(gameObject16.transform.position.x, gameObject16.transform.position.y - 4.8f);
													}
													else if (!mapChunk4.blockedS && !__instance.mapChunkArray[num9, num10 + 1].doorN)
													{
														GameObject gameObject17 = __instance.CreateSingleWall("WallHorizontalSplit", gameObject3, v3);
														gameObject17.transform.position = new Vector2(gameObject17.transform.position.x, gameObject17.transform.position.y - 4.8f);
														__instance.mapChunkArray[num9, num10].doorS = true;
													}
													if (!mapChunk5.exitS)
													{
														GameObject gameObject18 = __instance.CreateSingleWall("WallHorizontal", gameObject3, v4);
														gameObject18.transform.position = new Vector2(gameObject18.transform.position.x, gameObject18.transform.position.y - 4.8f);
													}
													else if (!mapChunk5.blockedS && !__instance.mapChunkArray[num11, num12 + 1].doorN)
													{
														GameObject gameObject19 = __instance.CreateSingleWall("WallHorizontalSplit", gameObject3, v4);
														gameObject19.transform.position = new Vector2(gameObject19.transform.position.x, gameObject19.transform.position.y - 4.8f);
														__instance.mapChunkArray[num11, num12].doorS = true;
													}
												}
											}
										}

										flag11 = (num13 != 0 || GC.percentChance(30));

										if (flag11)
										{
											if (!mapChunk2.exitE)
											{
												GameObject gameObject20 = __instance.CreateSingleWall("WallVertical", gameObject3, v);
												gameObject20.transform.position = new Vector2(gameObject20.transform.position.x + 4.8f, gameObject20.transform.position.y);
											}
											else if (!mapChunk2.blockedE && !__instance.mapChunkArray[num2 + 1, num3].doorW)
											{
												GameObject gameObject21 = __instance.CreateSingleWall("WallVerticalSplit", gameObject3, v);
												gameObject21.transform.position = new Vector2(gameObject21.transform.position.x + 4.8f, gameObject21.transform.position.y);
												__instance.mapChunkArray[num2, num3].doorE = true;
											}

											if (num13 != 0)
											{
												if (!mapChunk3.exitE)
												{
													GameObject gameObject22 = __instance.CreateSingleWall("WallVertical", gameObject3, v2);
													gameObject22.transform.position = new Vector2(gameObject22.transform.position.x + 4.8f, gameObject22.transform.position.y);
												}
												else if (!mapChunk3.blockedE && !__instance.mapChunkArray[num7 + 1, num8].doorW)
												{
													GameObject gameObject23 = __instance.CreateSingleWall("WallVerticalSplit", gameObject3, v2);
													gameObject23.transform.position = new Vector2(gameObject23.transform.position.x + 4.8f, gameObject23.transform.position.y);
													__instance.mapChunkArray[num7, num8].doorE = true;
												}

												if (num14 == 4)
												{
													if (!mapChunk4.exitE)
													{
														GameObject gameObject24 = __instance.CreateSingleWall("WallVertical", gameObject3, v3);
														gameObject24.transform.position = new Vector2(gameObject24.transform.position.x + 4.8f, gameObject24.transform.position.y);
													}
													else if (!mapChunk4.blockedE && !__instance.mapChunkArray[num9 + 1, num10].doorW)
													{
														GameObject gameObject25 = __instance.CreateSingleWall("WallVerticalSplit", gameObject3, v3);
														gameObject25.transform.position = new Vector2(gameObject25.transform.position.x + 4.8f, gameObject25.transform.position.y);
														__instance.mapChunkArray[num9, num10].doorE = true;
													}

													if (!mapChunk5.exitE)
													{
														GameObject gameObject26 = __instance.CreateSingleWall("WallVertical", gameObject3, v4);
														gameObject26.transform.position = new Vector2(gameObject26.transform.position.x + 4.8f, gameObject26.transform.position.y);
													}
													else if (!mapChunk5.blockedE && !__instance.mapChunkArray[num11 + 1, num12].doorW)
													{
														GameObject gameObject27 = __instance.CreateSingleWall("WallVerticalSplit", gameObject3, v4);
														gameObject27.transform.position = new Vector2(gameObject27.transform.position.x + 4.8f, gameObject27.transform.position.y);
														__instance.mapChunkArray[num11, num12].doorE = true;
													}
												}
											}
										}

										flag11 = (num13 != 0 || GC.percentChance(30));

										if (flag11)
										{
											if (!mapChunk2.exitW)
											{
												GameObject gameObject28 = __instance.CreateSingleWall("WallVertical", gameObject3, v);
												gameObject28.transform.position = new Vector2(gameObject28.transform.position.x - 4.8f, gameObject28.transform.position.y);
											}
											else if (!mapChunk2.blockedW && !__instance.mapChunkArray[num2 - 1, num3].doorE)
											{
												GameObject gameObject29 = __instance.CreateSingleWall("WallVerticalSplit", gameObject3, v);
												gameObject29.transform.position = new Vector2(gameObject29.transform.position.x - 4.8f, gameObject29.transform.position.y);
												__instance.mapChunkArray[num2, num3].doorW = true;
											}

											if (num13 != 0)
											{
												if (!mapChunk3.exitW)
												{
													GameObject gameObject30 = __instance.CreateSingleWall("WallVertical", gameObject3, v2);
													gameObject30.transform.position = new Vector2(gameObject30.transform.position.x - 4.8f, gameObject30.transform.position.y);
												}
												else if (!mapChunk3.blockedW && !__instance.mapChunkArray[num7 - 1, num8].doorE)
												{
													GameObject gameObject31 = __instance.CreateSingleWall("WallVerticalSplit", gameObject3, v2);
													gameObject31.transform.position = new Vector2(gameObject31.transform.position.x - 4.8f, gameObject31.transform.position.y);
													__instance.mapChunkArray[num7, num8].doorW = true;
												}

												if (num14 == 4)
												{
													if (!mapChunk4.exitW)
													{
														GameObject gameObject32 = __instance.CreateSingleWall("WallVertical", gameObject3, v3);
														gameObject32.transform.position = new Vector2(gameObject32.transform.position.x - 4.8f, gameObject32.transform.position.y);
													}
													else if (!mapChunk4.blockedW && !__instance.mapChunkArray[num9 - 1, num10].doorE)
													{
														GameObject gameObject33 = __instance.CreateSingleWall("WallVerticalSplit", gameObject3, v3);
														gameObject33.transform.position = new Vector2(gameObject33.transform.position.x - 4.8f, gameObject33.transform.position.y);
														__instance.mapChunkArray[num9, num10].doorW = true;
													}

													if (!mapChunk5.exitW)
													{
														GameObject gameObject34 = __instance.CreateSingleWall("WallVertical", gameObject3, v4);
														gameObject34.transform.position = new Vector2(gameObject34.transform.position.x - 4.8f, gameObject34.transform.position.y);
													}
													else if (!mapChunk5.blockedW && !__instance.mapChunkArray[num11 - 1, num12].doorE)
													{
														GameObject gameObject35 = __instance.CreateSingleWall("WallVerticalSplit", gameObject3, v4);
														gameObject35.transform.position = new Vector2(gameObject35.transform.position.x - 4.8f, gameObject35.transform.position.y);
														__instance.mapChunkArray[num11, num12].doorW = true;
													}
												}
											}
										}
									}

									component3.chunkLeash1Tr = component3.chunkLeash1.transform.position;

									if (component3.chunkLeash2 != null)
										component3.chunkLeash2Tr = component3.chunkLeash2.transform.position;
									
									if (component3.chunkLeash3 != null)
										component3.chunkLeash3Tr = component3.chunkLeash3.transform.position;
									
									if (component3.chunkLeash4 != null)
										component3.chunkLeash4Tr = component3.chunkLeash4.transform.position;
									
									component3.chunkEdgeN = component3.chunkPos.y;
									component3.chunkEdgeS = component3.chunkPos.y;
									component3.chunkEdgeE = component3.chunkPos.x;
									component3.chunkEdgeW = component3.chunkPos.x;
									
									if (component3.shape < 5)
									{
										component3.chunkEdgeN += 5.12f;
										component3.chunkEdgeS -= 5.12f;
										component3.chunkEdgeE += 5.12f;
										component3.chunkEdgeW -= 5.12f;
									}
									else if (component3.shape == 5 && component3.chunkRotate == 0)
									{
										component3.chunkEdgeN += 10.24f;
										component3.chunkEdgeS -= 10.24f;
										component3.chunkEdgeE += 5.12f;
										component3.chunkEdgeW -= 5.12f;
									}
									else if (component3.shape == 5 && component3.chunkRotate == 1)
									{
										component3.chunkEdgeN += 10.24f;
										component3.chunkEdgeS -= 10.24f;
										component3.chunkEdgeE += 5.12f;
										component3.chunkEdgeW -= 5.12f;
									}
									else if (component3.shape == 6)
									{
										component3.chunkEdgeN += 10.24f;
										component3.chunkEdgeS -= 10.24f;
										component3.chunkEdgeE += 10.24f;
										component3.chunkEdgeW -= 10.24f;
									}

									if (GC.levelTheme == 4 || GC.challenges.Contains(cChallenge.PoliceState))
									{
										if (component3.description == vChunkType.DeportationCenter)
											__instance.placedDeportationCenter = true;
										else if (component3.description == vChunkType.ConfiscationCenter)
											__instance.placedConfiscationCenter = true;
									}
									else if (GC.levelTheme == 5)
									{
										if (component3.description == vChunkType.PodiumPark)
											__instance.placedPodiumPark = true;
										else if (component3.description == vChunkType.MayorOffice)
											__instance.placedMayorOffice = true;
										else if (component3.description == vChunkType.MayorHouse)
											__instance.placedMayorHouse = true;
									}

									if (questsAdded < maxQuests && component3.description != "Generic" && component3.description != "Graveyard" && component3.description != "Bathroom")
									{
										int num27 = 0;

										if (GC.debugMode && GC.sessionDataBig.curLevelEndless <= 3 && GC.levelTheme > 0)
											num27 = GC.levelTheme * 3;
										
										int num29;
										
										if (GC.serverPlayer)
										{
											int num28 = GC.sessionDataBig.curLevelEndless;
										
											if (GC.customLevel && __instance.customLevel.levelDifficulty != 0)
												num28 = __instance.customLevel.levelDifficulty;
											
											num29 = num28 + num27 + __instance.FindExtraPlayers();
											
											if (num29 > 10)
												num29 = 10;
											
											__instance.clientDifficulty = num29;
										}
										else
											num29 = __instance.clientDifficulty;

										string text3 = GC.rnd.RandomSelect(string.Concat(new object[] { "ScenarioDifficultyLevel", num29, "Quest_", questsAdded + 1 }), "Scenarios");

										if (text3 == "ScenarioDifficulty1")
											component3.difficultyLevel = 1;
										else if (text3 == "ScenarioDifficulty2")
											component3.difficultyLevel = 2;
										else if (text3 == "ScenarioDifficulty3")
											component3.difficultyLevel = 3;
										else if (text3 == "ScenarioDifficulty4")
										{
											component3.difficultyLevel = 3;
											text3 = "ScenarioDifficulty3";
										}
										else if (text3 == "ScenarioDifficulty5")
										{
											component3.difficultyLevel = 3;
											text3 = "ScenarioDifficulty3";
										}
										
										bool flag12 = false;
										
										foreach (object obj2 in gameObject3.transform)
										{
											Transform transform3 = (Transform)obj2;
										
											if (transform3.name == text3 && transform3.childCount > 0)
												flag12 = true;
										}

										if (flag12)
										{
											if (component3.shape == 6 && GC.percentChance(80) && !component3.quested)
											{
												component3.giveQuest = questsAdded + 1;
												num = questsAdded;
												questsAdded = num + 1;
											}
											else if (component3.shape == 5 && GC.percentChance(80) && !component3.quested)
											{
												component3.giveQuest = questsAdded + 1;
												num = questsAdded;
												questsAdded = num + 1;
											}
											else if (component3.shape < 5 && !component3.quested)
											{
												component3.giveQuest = questsAdded + 1;
												num = questsAdded;
												questsAdded = num + 1;
											}
										}
									}
								}
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
			}

			if (GC.holeLevel)
			{
				GameObject holeChunk = GC.chunkLoad.holeChunk;

				for (int num30 = 0; num30 < __instance.levelSizeAxis; num30++)
				{
					for (int num31 = 0; num31 < __instance.levelSizeAxis; num31++)
					{
						if (__instance.mapChunkArray[num30, num31].chunkID == 0)
						{
							GameObject gameObject36 = Object.Instantiate<GameObject>(holeChunk, __instance.transform.position, __instance.transform.rotation);
							gameObject36.transform.position = new Vector2(gameObject36.transform.position.x + __instance.chunkSize * (float)num30 - __instance.halfTileSize + __instance.halfChunkSize, gameObject36.transform.position.y - __instance.chunkSize * (float)num31 + 102.4f + __instance.halfTileSize - __instance.halfChunkSize - __instance.tileSize);
							gameObject36.name = "HoleChunk";
							gameObject36.transform.SetParent(GC.chunksNest.transform);
							gameObject36.transform.localScale = new Vector3(0.01f, 0.01f, 1f);
							gameObject36.transform.Find("Border").GetComponent<SpawnerWall>().chunkN = false;
							gameObject36.transform.Find("Border").GetComponent<SpawnerWall>().chunkS = false;
							gameObject36.transform.Find("Border").GetComponent<SpawnerWall>().chunkE = false;
							gameObject36.transform.Find("Border").GetComponent<SpawnerWall>().chunkW = false;

							try
							{
								if (__instance.mapChunkArray[num30 + 1, num31].chunkID != 0)
									gameObject36.transform.Find("Border").GetComponent<SpawnerWall>().chunkE = true;
							}
							catch { }

							try
							{
								if (__instance.mapChunkArray[num30 - 1, num31].chunkID != 0)
									gameObject36.transform.Find("Border").GetComponent<SpawnerWall>().chunkW = true;
							}
							catch { }

							try
							{
								if (__instance.mapChunkArray[num30, num31 + 1].chunkID != 0)
									gameObject36.transform.Find("Border").GetComponent<SpawnerWall>().chunkS = true;
							}
							catch { }

							try
							{
								if (__instance.mapChunkArray[num30, num31 - 1].chunkID != 0)
									gameObject36.transform.Find("Border").GetComponent<SpawnerWall>().chunkN = true;
							}
							catch { }

							try
							{
								if (__instance.mapChunkArray[num30 + 1, num31 - 1].chunkID != 0)
									gameObject36.transform.Find("Border").GetComponent<SpawnerWall>().chunkNE = true;
							}
							catch { }

							try
							{
								if (__instance.mapChunkArray[num30 - 1, num31 - 1].chunkID != 0)
									gameObject36.transform.Find("Border").GetComponent<SpawnerWall>().chunkNW = true;
							}
							catch { }

							try
							{
								if (__instance.mapChunkArray[num30 + 1, num31 + 1].chunkID != 0)
									gameObject36.transform.Find("Border").GetComponent<SpawnerWall>().chunkSE = true;
							}
							catch { }

							try
							{
								if (__instance.mapChunkArray[num30 - 1, num31 + 1].chunkID != 0)
									gameObject36.transform.Find("Border").GetComponent<SpawnerWall>().chunkSW = true;
							}
							catch { }

							gameObject36.name = string.Concat(new object[] { num30, ", ", num31, " - " });
						}
					}
				}
			}

			if (__instance.doReturnToLevelEditor)
			{
				__instance.StopAllCoroutines();
				GC.levelEditor.ReturnToLevelEditor();
			}

			yield break;
		}
		public static bool LoadLevel_LoadStuff2_Prefix (LoadLevel __instance, ref IEnumerator __result, ref List<ChunkData> ___chunkListBasic, ref tk2dTileMap ___tilemapFloors, ref tk2dTileMap ___tilemapFloors2, ref tk2dTileMap ___tilemapWalls, ref bool ___placedKey1, ref bool ___placedKey2, ref bool ___placedKey3, ref List<GameObject> ___chunkList, ref List <ChunkData> ___chunkListBasicBackup) // Prefix
		{
			BMLog("LoadLevel_LoadStuff2_Prefix");

			// Structure advised by Abbysssal for patch-replacing IEnumerators.
			__result = LoadLevel_LoadStuff2_Replacement(__instance, ___chunkListBasic, ___tilemapFloors, ___tilemapFloors2, ___tilemapWalls, ___placedKey1, ___placedKey2, ___placedKey3, ___chunkList, ___chunkListBasicBackup);

			return false;
		}
		public static IEnumerator LoadLevel_LoadStuff2_Replacement (LoadLevel __instance, List<ChunkData> ___chunkListBasic, tk2dTileMap ___tilemapFloors, tk2dTileMap ___tilemapFloors2, tk2dTileMap ___tilemapWalls, bool ___placedKey1, bool ___placedKey2, bool ___placedKey3, List <GameObject> ___chunkList, List <ChunkData> ___chunkListBasicBackup) // Prefix
		{
			BMLog("LoadLevel_LoadStuff2_Replacement");

			Debug.Log("LoadStuff2");
			__instance.startedLoadStuff2 = true;

			if (GC.loadPoolsScene)
			{
				while (GC.poolsScene == null)
					yield return null;
			
				while (!GC.poolsScene.poolLoaded)
					yield return null;
			}

			if (GC.sessionData.gotData && GC.serverPlayer)
				GC.sessionData.RetrieveSeeds();
			
			for (int k = 0; k < GC.sessionData.usedChunks.Count; k++)
				__instance.usedChunksBeforeLevelLoad.Add(GC.sessionData.usedChunks[k]);
			
			if (GC.levelType != "Tutorial")
				GC.tutorial.enabled = false;
			
			if (__instance.revertBackToBasicSpawns)
				GC.basicSpawns = true;
			
			if (GC.basicSpawns)
			{
				if (GC.levelType == "HomeBase" || GC.levelType == "Tutorial")
					GC.basicSpawns = false;
			
				__instance.revertBackToBasicSpawns = true;
			}

			if ((GC.sessionData.gameCode == "" || GC.sessionData.gameCode == null) && GC.serverPlayer)
				GC.sessionData.gameCode = DateTime.UtcNow.ToUniversalTime().ToString();
			
			if (__instance.randomSeedNum == 0)
			{
				if (GC.sessionDataBig.dailyRun)
				{
					__instance.randomSeedNum = GC.sessionDataBig.dailyRunSeed.GetHashCode();
					__instance.randomSeedLetter = GC.sessionDataBig.dailyRunSeed;
					Debug.Log("Letter Seed: " + GC.sessionDataBig.dailyRunSeed);
					Debug.Log("Num Seed: " + __instance.randomSeedNum);
					Random.InitState(__instance.randomSeedNum);
				}
				else if (GC.sessionDataBig.userSetSeed != "" && GC.sessionDataBig.userSetSeed != null)
				{
					__instance.randomSeedNum = GC.sessionDataBig.userSetSeed.GetHashCode();
					__instance.randomSeedLetter = GC.sessionDataBig.userSetSeed;
					Debug.Log("Letter Seed: " + GC.sessionDataBig.userSetSeed);
					Debug.Log("Num Seed: " + __instance.randomSeedNum);
					Random.InitState(__instance.randomSeedNum);
				}
				else if (GC.seedNum != 0)
				{
					__instance.randomSeedNum = GC.seedNum;
					Debug.Log("Num Seed: " + __instance.randomSeedNum);
					Random.InitState(__instance.randomSeedNum);
				}
				else if (GC.seedString != "")
				{
					__instance.randomSeedNum = GC.seedString.GetHashCode();

					if (GC.seedString == "abc")
						__instance.randomSeedNum = 96354;
					
					__instance.randomSeedLetter = GC.seedString;
					Debug.Log("Letter Seed: " + GC.seedString);
					Debug.Log("Num Seed: " + __instance.randomSeedNum);
					Random.InitState(__instance.randomSeedNum);
				}
				else
				{
					string text = "";

					for (int l = 0; l < 8; l++)
					{
						int num = Random.Range(0, 26);
						text += ((char)(97 + num)).ToString();
					}
					
					__instance.randomSeedLetter = text;
					__instance.randomSeedNum = text.GetHashCode();
					Debug.Log("Letter Seed: " + __instance.randomSeedLetter);
					Debug.Log("Num Seed: " + __instance.randomSeedNum);
					Random.InitState(__instance.randomSeedNum);
				}
			}
			else if (!GC.serverPlayer)
			{
				Debug.Log("Num Seed: " + __instance.randomSeedNum);
				Random.InitState(__instance.randomSeedNum);
				Debug.Log("HEEERE3");
			}
			else
			{
				__instance.randomSeedNum++;
				if (GC.oneTimeVersion)
				{
					__instance.randomSeedNum++;
				}
				Debug.Log("Letter Seed: " + __instance.randomSeedLetter);
				Debug.Log("Num Seed: " + __instance.randomSeedNum);
				Random.InitState(__instance.randomSeedNum);
				Debug.Log("HEEERE4");
			}

			if (GC.streamingWorld && GC.seedString == "abc")
			{
				__instance.randomSeedNum = 96355;
				__instance.randomSeedLetter = GC.seedString;
				Random.InitState(__instance.randomSeedNum);
			}

			Debug.Log(string.Concat(new object[] { "RANDOM SEED: ", __instance.randomSeedLetter, " - ", __instance.randomSeedNum }));
			Debug.Log(string.Concat(new object[] { "LEVEL NUM: ", GC.sessionDataBig.curLevel, " - ", GC.sessionDataBig.curLevelEndless, " (Endless)" }));
			Debug.Log(string.Concat(new object[] { "RANDOM NUMBER: ", Random.Range(0, 1000), " - ", GC.rnd.loadedRandomnessEarly.ToString() }));

			if (GC.challenges.Count > 0)
			{
				string text2 = "Mutators: ";

				for (int m = 0; m < GC.challenges.Count; m++)
				{
					text2 += GC.challenges[m];
				
					if (m != GC.challenges.Count - 1)
						text2 += ", ";
				}
				
				Debug.Log(text2);
			}

			if (!GC.rnd.loadedRandomnessEarly)
				GC.rnd.LoadRandomnessEarly();
			
			if (GC.sessionDataBig.challenges.Contains("RandomMutators") && GC.levelType != "Tutorial" && GC.levelType != "HomeBase")
				GC.menuGUI.FindRandomMutators();
			
			if (GC.streamingWorld)
				GC.staticChunk = "Streaming";
			
			if (GC.debugMode || GC.sessionDataBig.curLevel != 0)
			{
				GC.customRandomLevel = false;

				if (GC.customCampaign)
				{
					try
					{
						bool flag = false;

						if (!GC.serverPlayer)
							GC.sessionDataBig.curLevelEndless = GC.playerAgent.oma.curLevelEndless;

						Debug.Log(string.Concat(new object[] { "Trying to load custom level: ", __instance.customCampaign.levelList.Count, " - ", GC.sessionDataBig.curLevelEndless - 1 })); 

						for (int n = 0; n < __instance.customCampaign.levelList.Count; n++)
							if (__instance.customCampaign.levelList[n].levelName == __instance.customCampaign.levelNameList[GC.sessionDataBig.curLevelEndless - 1])
							{
								if (__instance.customCampaign.levelList[n].randomizeLevelContent)
								{
									__instance.customLevel = __instance.customCampaign.levelList[n];
									GC.customLevel = true;
									Debug.LogError("Loaded Custom Level: Random Content");
								}
								else
								{
									__instance.customLevel = __instance.customCampaign.levelList[n];
									GC.customLevel = true;
									Debug.Log(string.Concat(new object[] { "Loaded Custom Level: ", GC.sessionDataBig.curLevelEndless, " - ", __instance.customLevel.levelName }));
								}

								flag = true;
							}

						if (!flag)
							Debug.LogError("Couldn't load custom level (Error 1): " + GC.sessionDataBig.curLevelEndless);
					}
					catch
					{
						__instance.customLevel = null;
						GC.customLevel = false;
						Debug.LogError("Couldn't load custom level (Error 2): " + GC.sessionDataBig.curLevelEndless);
					}

					for (int num2 = 0; num2 < __instance.customCampaign.mutatorList.Count; num2++)
						if (!GC.challenges.Contains(__instance.customCampaign.mutatorList[num2]))
							GC.challenges.Add(__instance.customCampaign.mutatorList[num2]);
					
					GC.sessionDataBig.challenges = GC.challenges;
					GC.sessionDataBig.originalChallenges = GC.originalChallenges;
					GC.SetDailyRunText();
				}

				if (GC.customLevel)
				{
					if (__instance.customLevel.randomizeLevelContent)
						GC.customRandomLevel = true;
					
					for (int num3 = 0; num3 < __instance.customLevel.levelMutators.Count; num3++)
						if (!GC.challenges.Contains(__instance.customLevel.levelMutators[num3]))
							GC.challenges.Add(__instance.customLevel.levelMutators[num3]);
					
					GC.sessionDataBig.challenges = GC.challenges;
					GC.sessionDataBig.originalChallenges = GC.originalChallenges;
					GC.SetDailyRunText();
				}
			}

			Random.InitState(__instance.randomSeedNum);

			if (GC.sessionData.gotData || GC.debugMode || GC.sessionDataBig.startedGame || GC.levelEditing || GC.wasLevelEditing)
			{
				if (GC.levelType == "Attract")
				{
					GC.playerControl.SetCantPressGameplayButtons("Attract", 1, 0);
					GC.playerControl.SetCantPressGameplayButtons("Attract", 1, 1);
					GC.mainGUI.transform.Find("PressStart").gameObject.SetActive(true);
					GC.mainGUI.arcadeGUIScript.pressStartHolder.gameObject.SetActive(false);
					GC.mainGUI2.arcadeGUIScript.pressStartHolder.gameObject.SetActive(false);
					GC.mainGUI3.arcadeGUIScript.pressStartHolder.gameObject.SetActive(false);
					GC.mainGUI4.arcadeGUIScript.pressStartHolder.gameObject.SetActive(false);
					GC.nonClickableGUI.gameObject.SetActive(false);
					GC.nonClickableGUI2.gameObject.SetActive(false);
					GC.nonClickableGUI3.gameObject.SetActive(false);
					GC.nonClickableGUI4.gameObject.SetActive(false);
				}
				else if (GC.levelType == "Tutorial")
				{
					GameObject gameObject = Object.Instantiate<GameObject>(GC.chunkLoad.tutorialLevel, __instance.transform.position, __instance.transform.rotation);
					gameObject.transform.position = new Vector2(51.2f - __instance.halfTileSize, 51.2f - __instance.halfTileSize);
					gameObject.name = "ChunkStatic";
					gameObject.transform.parent = GC.chunksNest.transform;
					gameObject.transform.localScale = new Vector3(0.01f, 0.01f, 1f);
					__instance.staticChunk = gameObject.GetComponent<Chunk>();
					GC.levelTheme = 0;

					foreach (object obj in gameObject.transform)
					{
						Transform transform = (Transform)obj;

						if (transform.GetComponent<Scenario>())
							transform.gameObject.SetActive(true);
					}

					Chunk component = gameObject.GetComponent<Chunk>();
					component.chunkID = 0;
					component.chunkX = 1;
					component.chunkY = 1;
					__instance.mapChunkArray[6, 6].chunkID = 1;
					__instance.mapChunkArray[6, 5].chunkID = 1;
					__instance.mapChunkArray[6, 4].chunkID = 1;
					__instance.mapChunkArray[6, 3].chunkID = 1;
					__instance.mapChunkArray[5, 6].chunkID = 1;
					__instance.mapChunkArray[5, 5].chunkID = 1;
					__instance.mapChunkArray[5, 4].chunkID = 1;
					__instance.mapChunkArray[5, 3].chunkID = 1;
					__instance.mapChunkArray[4, 6].chunkID = 1;
					__instance.mapChunkArray[4, 5].chunkID = 1;
					__instance.mapChunkArray[4, 4].chunkID = 1;
					__instance.mapChunkArray[4, 3].chunkID = 1;
					__instance.mapChunkArray[3, 6].chunkID = 1;
					__instance.mapChunkArray[3, 5].chunkID = 1;
					__instance.mapChunkArray[3, 4].chunkID = 1;
					__instance.mapChunkArray[3, 3].chunkID = 1;
					__instance.SetupBasicLevel();

					__instance.StartCoroutine(LoadLevel_FillMapChunks_Replacement(__instance, ___tilemapFloors2, ___tilemapWalls, ___chunkList, ___chunkListBasic, ___chunkListBasicBackup));

					while (!__instance.allChunksFilled)
						yield return null;
					
					GC.cameraScript.zoomLevel = 0.49999997f;
					
					if (GC.coopMode && GC.sessionDataBig.splitScreenMode == "Split")
					{
						GC.cameraScriptS1.zoomLevel = 0.4f;
						GC.cameraScriptS2.zoomLevel = 0.4f;
					}
					else if (GC.fourPlayerMode && GC.sessionDataBig.splitScreenMode == "Split")
					{
						GC.cameraScriptS1.zoomLevel = 0.3f;
						GC.cameraScriptS2.zoomLevel = 0.3f;
						GC.cameraScriptS3.zoomLevel = 0.3f;
						GC.cameraScriptS4.zoomLevel = 0.3f;
					}
				}
				else if (GC.levelType == "HomeBase" || (!GC.debugMode && !GC.levelEditing && !GC.wasLevelEditing && GC.sessionDataBig.curLevel == 0) || (GC.debugMode && GC.startInHomeBase && GC.sessionDataBig.curLevel == 0))
				{
					GC.levelType = "HomeBase";
					GC.levelTheme = 0;

					if (!GC.serverPlayer)
					{
						GC.sessionDataBig.curLevel = GC.playerAgent.oma.curLevel;
						Debug.Log("CURLEVEL CLIENT ONLY_6: " + GC.sessionDataBig.curLevel);
						GC.sessionDataBig.curLevelEndless = GC.playerAgent.oma.curLevelEndless;
						GC.levelTheme = GC.playerAgent.oma.levelTheme;
					}
					
					GameObject gameObject2 = Object.Instantiate<GameObject>(GC.chunkLoad.homeBase, __instance.transform.position, __instance.transform.rotation);
					gameObject2.transform.position = new Vector2(51.2f - __instance.halfTileSize, 51.2f - __instance.halfTileSize);
					gameObject2.name = "ChunkStatic";
					gameObject2.transform.parent = GC.chunksNest.transform;
					gameObject2.transform.localScale = new Vector3(0.01f, 0.01f, 1f);
					__instance.staticChunk = gameObject2.GetComponent<Chunk>();
					
					foreach (object obj2 in gameObject2.transform)
					{
						Transform transform2 = (Transform)obj2;
					
						if (transform2.GetComponent<Scenario>())
							transform2.gameObject.SetActive(true);
					}

					Chunk component2 = gameObject2.GetComponent<Chunk>();
					component2.chunkID = 0;
					component2.chunkX = 1;
					component2.chunkY = 1;
					__instance.mapChunkArray[6, 6].chunkID = 1;
					__instance.mapChunkArray[6, 5].chunkID = 1;
					__instance.mapChunkArray[6, 4].chunkID = 1;
					__instance.mapChunkArray[6, 3].chunkID = 1;
					__instance.mapChunkArray[5, 6].chunkID = 1;
					__instance.mapChunkArray[5, 5].chunkID = 1;
					__instance.mapChunkArray[5, 4].chunkID = 1;
					__instance.mapChunkArray[5, 3].chunkID = 1;
					__instance.mapChunkArray[4, 6].chunkID = 1;
					__instance.mapChunkArray[4, 5].chunkID = 1;
					__instance.mapChunkArray[4, 4].chunkID = 1;
					__instance.mapChunkArray[4, 3].chunkID = 1;
					__instance.mapChunkArray[3, 6].chunkID = 1;
					__instance.mapChunkArray[3, 5].chunkID = 1;
					__instance.mapChunkArray[3, 4].chunkID = 1;
					__instance.mapChunkArray[3, 3].chunkID = 1;
					__instance.mapChunkArray[2, 4].chunkID = 1;
					__instance.mapChunkArray[7, 4].chunkID = 1;
					__instance.mapChunkArray[7, 6].chunkID = 1;

					__instance.SetupBasicLevel();
					__instance.StartCoroutine(LoadLevel_FillMapChunks_Replacement(__instance, ___tilemapFloors2, ___tilemapWalls, ___chunkList, ___chunkListBasic, ___chunkListBasicBackup));
					
					while (!__instance.allChunksFilled)
						yield return null;
					
					GC.cameraScript.zoomLevel = 0.49999997f;
					
					if (GC.coopMode && GC.sessionDataBig.splitScreenMode == "Split")
					{
						GC.cameraScriptS1.zoomLevel = 0.4f;
						GC.cameraScriptS2.zoomLevel = 0.4f;
					}
					else if (GC.fourPlayerMode && GC.sessionDataBig.splitScreenMode == "Split")
					{
						GC.cameraScriptS1.zoomLevel = 0.3f;
						GC.cameraScriptS2.zoomLevel = 0.3f;
						GC.cameraScriptS3.zoomLevel = 0.3f;
						GC.cameraScriptS4.zoomLevel = 0.3f;
					}
				}
				else if (GC.staticChunk == "")
				{
					if (!GC.serverPlayer)
					{
						if (GC.playerAgent.oma.curLevel == -1)
						{
							Debug.Log("Waiting for proper level number from server");

							while (GC.playerAgent.oma.curLevel == -1)
								yield return null;
						}

						GC.sessionDataBig.curLevel = GC.playerAgent.oma.curLevel;
						Debug.Log("CURLEVEL CLIENT ONLY_7: " + GC.sessionDataBig.curLevel);
						GC.sessionDataBig.curLevelEndless = GC.playerAgent.oma.curLevelEndless;
						GC.levelTheme = GC.playerAgent.oma.levelTheme;
					}
					else if (GC.sessionDataBig.curLevel == 0)
					{
						GC.sessionDataBig.curLevel = 1;
						Debug.Log("CURLEVEL SERVER ONLY_8: " + GC.sessionDataBig.curLevel);
						GC.sessionDataBig.curLevelEndless = 1;
						GC.sessionDataBig.curLevelActual = 1;
					}

					if (GC.serverPlayer)
					{
						if (GC.challenges.Contains("QuickGame"))
							__instance.quickGame = true;
					}
					else if (GC.challenges.Contains("QuickGame"))
						__instance.quickGame = true;
					else if (GC.playerAgent.oma.quickGame && !GC.challenges.Contains("QuickGame"))
					{
						GC.challenges.Add("QuickGame");
						GC.originalChallenges.Add("QuickGame");
						__instance.quickGame = true;
					}
					
					__instance.SetLevelTheme();
					Debug.Log(string.Concat(new object[] { "CREATING INITIAL MAP WITH SEED NUM: ", __instance.randomSeedNum, " - UsedChunks Count: ", GC.sessionData.usedChunks.Count }));
					Random.InitState(__instance.randomSeedNum);
					LoadLevel_CreateInitialMap(__instance, ref ___placedKey1, ref ___placedKey2, ref ___placedKey3);
					Debug.Log("Random Number After CreateInitialMap: " + Random.Range(0, 1000));
					__instance.StartCoroutine(LoadLevel_FillMapChunks_Replacement(__instance, ___tilemapFloors2, ___tilemapWalls, ___chunkList, ___chunkListBasic, ___chunkListBasicBackup));

					while (!__instance.allChunksFilled)
						yield return null;
					
					if (!GC.levelEditing)
					{
						if (GC.debugMode || GC.wasLevelEditing)
						{
							if (GC.consoleVersion)
								GC.cameraScript.zoomLevel = 0.9f;
							else
								GC.cameraScript.zoomLevel = 0.2f;
						}
						else
						{
							GC.cameraScript.zoomLevel = 0.49999997f;
						
							if (GC.coopMode && GC.sessionDataBig.splitScreenMode == "Split")
							{
								GC.cameraScriptS1.zoomLevel = 0.4f;
								GC.cameraScriptS2.zoomLevel = 0.4f;
							}
							else if (GC.fourPlayerMode && GC.sessionDataBig.splitScreenMode == "Split")
							{
								GC.cameraScriptS1.zoomLevel = 0.3f;
								GC.cameraScriptS2.zoomLevel = 0.3f;
								GC.cameraScriptS3.zoomLevel = 0.3f;
								GC.cameraScriptS4.zoomLevel = 0.3f;
							}
						}
					}
				}
				else if (GC.staticChunk != "")
				{
					GameObject original = null;

					for (int num4 = 0; num4 < GC.chunkLoad.chunkList.Count; num4++)
						if (GC.chunkLoad.chunkList[num4].name == GC.staticChunk)
						{
							original = GC.chunkLoad.chunkList[num4];
					
							break;
						}
					
					Chunk chunk = null;
					GameObject gameObject3 = null;
					int num5 = 0;
					
					if (!GC.levelEditing && GC.staticChunk != "Streaming")
					{
						if (GC.basicSpawns)
						{
							ChunkData chunkData = null;

							for (int num6 = 0; num6 < ___chunkListBasic.Count; num6++)
								if (___chunkListBasic[num6].chunkName == GC.staticChunk)
								{
									chunkData = ___chunkListBasic[num6];
							
									break;
								}
							
							gameObject3 = Object.Instantiate<GameObject>(GC.readChunks.chunkPrefab, __instance.transform.position, __instance.transform.rotation);
							Chunk component3 = gameObject3.GetComponent<Chunk>();
							gameObject3.name = chunkData.chunkName;
							component3.shape = chunkData.shape;
							num5 = component3.shape;
							component3.description = chunkData.description;
							component3.functionType = chunkData.functionType;
							component3.spawnerList = chunkData.spawnerList;

							try
							{
								component3.userCreated = chunkData.userCreated;
							}
							catch { }

							try
							{
								component3.customCharacterList = chunkData.customCharacterList;
							}
							catch { }

							chunk = component3;

							for (int num7 = 0; num7 < component3.spawnerList.Count; num7++)
							{
								SpawnerBasic spawnerBasic = new SpawnerBasic();
								GC.readChunks.CopySpawnerData(component3.spawnerList[num7], spawnerBasic);
								SpawnerBasic spawnerBasic2 = spawnerBasic;
								GameController gameController = GC;
								int num8 = gameController.spawnerCount;
								gameController.spawnerCount = num8 + 1;
								spawnerBasic2.spawnerID = num8;
								GC.spawnerBasicList.Add(spawnerBasic);
								GC.loadLevel.spawnersBasic.Add(spawnerBasic);
								GC.loadLevel.startingChunkReals.Add(chunk);
								spawnerBasic.startingChunk = 1;
							}
						}
						else
						{
							gameObject3 = Object.Instantiate<GameObject>(original, __instance.transform.position, __instance.transform.rotation);
							chunk = gameObject3.GetComponent<Chunk>();

							if (gameObject3.name.Contains("Long"))
								num5 = 5;
							else
								num5 = 6;
						}

						if (num5 == 5)
							gameObject3.transform.position = new Vector2(16f + __instance.halfTileSize, 20.16f);
						else if (num5 == 6)
							gameObject3.transform.position = new Vector2(20.16f, 20.16f);
						else
							gameObject3.transform.position = new Vector2(16f + __instance.halfTileSize, 11.84f);
						
						gameObject3.name = "ChunkStatic";
						gameObject3.transform.SetParent(GC.chunksNest.transform);
						gameObject3.transform.localScale = new Vector3(0.01f, 0.01f, 1f);
						__instance.staticChunk = gameObject3.GetComponent<Chunk>();
						
						foreach (object obj3 in gameObject3.transform)
						{
							Transform transform3 = (Transform)obj3;
						
							if (transform3.GetComponent<Scenario>())
								transform3.gameObject.SetActive(true);
						}

						chunk.chunkID = 0;
						chunk.chunkX = 1;
						chunk.chunkY = 1;
						int num9 = GC.Choose<int>(0, 1);
						string text3 = GC.Choose<string>("Normal", "Reverse");
						string text4 = GC.Choose<string>("Normal", "Reverse");
						
						if (GC.wasLevelEditingChunk)
						{
							num9 = 0;
							text3 = "Normal";
							text4 = "Normal";
						}
						if (num9 == 1)
						{
							gameObject3.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
							chunk.chunkRotate = num9;
						}
						if (text3 == "Reverse")
						{
							gameObject3.transform.localScale = new Vector3(-0.01f, 0.01f, 1f);
							chunk.chunkDirX = text3;
						}
						if (text4 == "Reverse")
						{
							gameObject3.transform.localScale = new Vector3(gameObject3.transform.localScale.x, -0.01f, 1f);
							chunk.chunkDirY = text4;
						}
					}

					__instance.SetLevelTheme();
					__instance.StartCoroutine(LoadLevel_FillFloors_Replacement(__instance, ___tilemapFloors2));

					while (!__instance.allChunksFilled)
						yield return null;
				}
			}

			__instance.SetLevelTheme();
			Random.InitState(__instance.randomSeedNum);
			GC.findFloorName();
			Debug.Log("RANDOM NUMBER 1.4: " + Random.Range(0, 1000));

			for (int num10 = 0; num10 < __instance.levelSizeAxis; num10++)
				for (int num11 = 0; num11 < __instance.levelSizeAxis; num11++)
					_ = __instance.mapChunkArray[num10, num11].chunkID;

			Debug.Log("RANDOM NUMBER 1.5: " + Random.Range(0, 1000));
			
			if (GC.basicSpawns)
				Debug.Log("Spawners Length: " + __instance.spawnersBasic.Count);
			else
				Debug.Log("Spawners Length: " + __instance.spawners.Count);
			
			Debug.Log("RANDOM NUMBER 1.6: " + Random.Range(0, 1000));
			
			if (!GC.basicSpawns)
			{
				foreach (GameObject gameObject4 in __instance.spawners)
					try
					{
						gameObject4.GetComponent<Spawner>().startingChunk = gameObject4.transform.parent.GetComponent<Chunk>().chunkID;
					}
					catch
					{
						try
						{
							gameObject4.GetComponent<Spawner>().startingChunk = gameObject4.transform.parent.transform.parent.GetComponent<Chunk>().chunkID;
						}
						catch
						{
							Debug.LogError("Spawner error in LoadLevel: " + gameObject4);
						}
					}

				foreach (GameObject gameObject5 in __instance.spawners)
				{
					Spawner component4 = gameObject5.GetComponent<Spawner>();

					if (component4.scenarioChunk == "")
						try
						{
							component4.scenarioChunk = component4.transform.parent.GetComponent<Scenario>().scenarioChunk;

							if (component4.scenarioChoice == "")
								component4.scenarioChoice = component4.transform.parent.GetComponent<Scenario>().name;
						}
						catch { }
				}
			}

			if (__instance.LevelContainsMayor())
				for (int num12 = 0; num12 < GC.sessionData.bigQuestsCompleted.Count; num12++)
					_ = GC.sessionData.bigQuestsCompleted[num12] == "Gangbanger";

			Random.InitState(__instance.randomSeedNum);
			Debug.Log("RANDOM NUMBER 2: " + Random.Range(0, 1000));
			int wallSpawners = 0;
			int floorSpawners = 0;
			int lightSpawners = 0;
			bool highFrameRate = true;
			float maxChunkTime = 0.02f;
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			__instance.memoryTest = false;

			if (GC.streamingWorld)
			{
				__instance.settingUpInitialStreamingObjects = true;

				foreach (KeyValuePair<string, ObjectVar> entry2 in GC.objectVars.objectVarDic)
				{
					ObjectVar myObjectVar = entry2.Value;
					int num8;

					for (int i = 0; i < myObjectVar.initialSpawns; i = num8 + 1)
					{
						ObjectReal component5 = GC.poolsScene.SpawnObjectReal(entry2.Key, __instance.gr.objectPrefabDic[entry2.Key], Vector3.zero).GetComponent<ObjectReal>();

						if (!component5.movingObject)
							component5.gameObject.isStatic = true;
						
						if (component5.transform.childCount != 0)
						{
							Transform child = component5.transform.GetChild(0);

							if (child.localPosition.z != -1E-06f)
								child.localPosition = new Vector3(child.localPosition.x, child.localPosition.y, -1E-06f);
						}

						if (myObjectVar.originalName != "" && myObjectVar.originalName != null)
							component5.objectRealRealName = GC.nameDB.GetName(myObjectVar.originalName, "Object");
						else
							component5.objectRealRealName = GC.nameDB.GetName(entry2.Key, "Object");

						component5.go.SetActive(false);

						if (Time.realtimeSinceStartup - realtimeSinceStartup > maxChunkTime)
						{
							yield return null;
						
							realtimeSinceStartup = Time.realtimeSinceStartup;
						}

						num8 = i;
					}

					myObjectVar = null;
					//entry2 = default(KeyValuePair<string, ObjectVar>); // TODO
				}

				Dictionary<string, ObjectVar>.Enumerator enumerator4 = default(Dictionary<string, ObjectVar>.Enumerator);
				GC.poolsScene.ResetPoolObjectReal();
				__instance.settingUpInitialStreamingObjects = false;
			}

			if (GC.basicSpawns)
			{
				if (highFrameRate)
				{
					List<SpawnerBasic> wallList = new List<SpawnerBasic>();
					List<Chunk> wallListChunks = new List<Chunk>();

					for (int num13 = 0; num13 < __instance.spawnersBasic.Count; num13++)
					{
						SpawnerBasic spawnerBasic3 = __instance.spawnersBasic[num13];
						Chunk item = __instance.startingChunkReals[num13];

						if (spawnerBasic3.spawnerType == "Wall")
						{
							wallList.Add(spawnerBasic3);
							wallListChunks.Add(item);
						}
					}

					for (int num14 = 0; num14 < wallList.Count; num14++)
					{
						SpawnerBasic value = wallList[num14];
						Chunk value2 = wallListChunks[num14];
						int index = Random.Range(num14, wallList.Count);
						wallList[num14] = wallList[index];
						wallList[index] = value;
						wallListChunks[num14] = wallListChunks[index];
						wallListChunks[index] = value2;
					}

					int num8;

					for (int i = 0; i < wallList.Count; i = num8 + 1)
					{
						SpawnerBasic spawnerBasic4 = wallList[i];
						Chunk chunk2 = wallListChunks[i];
						Random.InitState(__instance.randomSeedNum + i);
						GC.basicSpawn.DetermineSpawnType(spawnerBasic4, chunk2);
						num8 = wallSpawners;
						wallSpawners = num8 + 1;
						__instance.spawnersBasic.Remove(spawnerBasic4);
						__instance.startingChunkReals.Remove(chunk2);

						if (Time.realtimeSinceStartup - realtimeSinceStartup > maxChunkTime)
						{
							yield return null;

							realtimeSinceStartup = Time.realtimeSinceStartup;
						}

						num8 = i;
					}

					wallList = null;
					wallListChunks = null;
				}
				else
					for (int num15 = 0; num15 < __instance.spawnersBasic.Count; num15++)
					{
						SpawnerBasic spawnerBasic5 = __instance.spawnersBasic[num15];
						Chunk startingChunkReal = __instance.startingChunkReals[num15];

						if (spawnerBasic5.spawnerType == "Wall")
						{
							GC.basicSpawn.DetermineSpawnType(spawnerBasic5, startingChunkReal);
							int num8 = wallSpawners;
							wallSpawners = num8 + 1;
						}
					}
			}
			else if (highFrameRate)
			{
				List<Spawner> wallList2 = new List<Spawner>();

				for (int num16 = 0; num16 < __instance.spawners.Count; num16++)
				{
					Spawner component6 = __instance.spawners[num16].GetComponent<Spawner>();

					if (component6.spawnerType == "Wall")
						wallList2.Add(component6);
				}

				for (int num17 = 0; num17 < wallList2.Count; num17++)
				{
					Spawner value3 = wallList2[num17];
					int index2 = Random.Range(num17, wallList2.Count);
					wallList2[num17] = wallList2[index2];
					wallList2[index2] = value3;
				}

				int num8;

				for (int i = 0; i < wallList2.Count; i = num8 + 1)
				{
					Random.InitState(__instance.randomSeedNum + i);
					Spawner spawner = wallList2[i];
					spawner.DetermineSpawnType();
					num8 = wallSpawners;
					wallSpawners = num8 + 1;
					__instance.spawners.Remove(spawner.gameObject);

					if (Time.realtimeSinceStartup - realtimeSinceStartup > maxChunkTime)
					{
						yield return null;

						realtimeSinceStartup = Time.realtimeSinceStartup;
					}

					num8 = i;
				}
				wallList2 = null;
			}
			else
			{
				for (int num18 = 0; num18 < __instance.spawners.Count; num18++)
				{
					Spawner component7 = __instance.spawners[num18].GetComponent<Spawner>();

					if (component7.spawnerType == "Wall")
					{
						component7.DetermineSpawnType();
						int num8 = wallSpawners;
						wallSpawners = num8 + 1;
					}
				}
			}

			Random.InitState(__instance.randomSeedNum);
			Debug.Log(string.Concat(new object[] { "RANDOM NUMBER 2.5: ", Random.Range(0, 1000), " - ", wallSpawners, " - ", __instance.spawners.Count }));

			if (GC.basicSpawns)
			{
				if (highFrameRate)
				{
					List<SpawnerBasic> wallList = new List<SpawnerBasic>();
					List<Chunk> wallListChunks = new List<Chunk>();

					for (int num19 = 0; num19 < __instance.spawnersBasic.Count; num19++)
					{
						SpawnerBasic spawnerBasic6 = __instance.spawnersBasic[num19];
						Chunk item2 = __instance.startingChunkReals[num19];

						if (spawnerBasic6.spawnerType == "Floor")
						{
							wallList.Add(spawnerBasic6);
							wallListChunks.Add(item2);
						}
					}

					for (int num20 = 0; num20 < wallList.Count; num20++)
					{
						SpawnerBasic value4 = wallList[num20];
						Chunk value5 = wallListChunks[num20];
						int index3 = Random.Range(num20, wallList.Count);
						wallList[num20] = wallList[index3];
						wallList[index3] = value4;
						wallListChunks[num20] = wallListChunks[index3];
						wallListChunks[index3] = value5;
					}

					int num8;

					for (int i = 0; i < wallList.Count; i = num8 + 1)
					{
						SpawnerBasic spawnerBasic7 = wallList[i];
						Chunk chunk3 = wallListChunks[i];
						Random.InitState(__instance.randomSeedNum + i);
						GC.basicSpawn.DetermineSpawnType(spawnerBasic7, chunk3);
						num8 = floorSpawners;
						floorSpawners = num8 + 1;
						__instance.spawnersBasic.Remove(spawnerBasic7);
						__instance.startingChunkReals.Remove(chunk3);

						if (Time.realtimeSinceStartup - realtimeSinceStartup > maxChunkTime)
						{
							yield return null;

							realtimeSinceStartup = Time.realtimeSinceStartup;
						}

						num8 = i;
					}

					wallList = null;
					wallListChunks = null;
				}
				else
					for (int num21 = 0; num21 < __instance.spawnersBasic.Count; num21++)
					{
						SpawnerBasic spawnerBasic8 = __instance.spawnersBasic[num21];
						Chunk startingChunkReal2 = __instance.startingChunkReals[num21];

						if (spawnerBasic8.spawnerType == "Floor")
						{
							GC.basicSpawn.DetermineSpawnType(spawnerBasic8, startingChunkReal2);
							int num8 = floorSpawners;
							floorSpawners = num8 + 1;
						}
					}
			}
			else if (highFrameRate)
			{
				List<Spawner> wallList2 = new List<Spawner>();

				for (int num22 = 0; num22 < __instance.spawners.Count; num22++)
				{
					Spawner component8 = __instance.spawners[num22].GetComponent<Spawner>();

					if (component8.spawnerType == "Floor")
						wallList2.Add(component8);
				}

				for (int num23 = 0; num23 < wallList2.Count; num23++)
				{
					Spawner value6 = wallList2[num23];
					int index4 = Random.Range(num23, wallList2.Count);
					wallList2[num23] = wallList2[index4];
					wallList2[index4] = value6;
				}

				int num8;

				for (int i = 0; i < wallList2.Count; i = num8 + 1)
				{
					Spawner spawner2 = wallList2[i];
					Random.InitState(__instance.randomSeedNum + i);
					spawner2.DetermineSpawnType();
					num8 = floorSpawners;
					floorSpawners = num8 + 1;
					__instance.spawners.Remove(spawner2.gameObject);

					if (Time.realtimeSinceStartup - realtimeSinceStartup > maxChunkTime)
					{
						yield return null;

						realtimeSinceStartup = Time.realtimeSinceStartup;
					}

					num8 = i;
				}

				wallList2 = null;
			}
			else
				for (int num24 = 0; num24 < __instance.spawners.Count; num24++)
				{
					Spawner component9 = __instance.spawners[num24].GetComponent<Spawner>();

					if (component9.spawnerType == "Floor")
					{
						component9.DetermineSpawnType();
						int num8 = floorSpawners;
						floorSpawners = num8 + 1;
					}
				}
			
			if (((GC.levelTheme == 1 || GC.levelTheme == 3 || (GC.challenges.Contains("MixedUpLevels") && GC.percentChance(33))) && GC.staticChunk == "" && GC.levelType == "Normal") || GC.playerAgent.oma.hasTracks)
				__instance.hasTracks = true;
			
			if (GC.customLevel)
				for (int num25 = 0; num25 < __instance.customLevel.levelFeatures.Count; num25++)
					if (__instance.customLevel.levelFeatures[num25].Contains("Tracks"))
					{
						__instance.hasTracks = true;
			
						break;
					}

			if (__instance.hasTracks && !GC.levelEditing)
			{
				Random.InitState(__instance.randomSeedNum);
				GC.tileInfo.PlaceTracks();
			}
			
			Random.InitState(__instance.randomSeedNum);
			
			if (GC.levelTheme == 1 || (GC.challenges.Contains("MixedUpLevels") && GC.percentChance(33)))
				__instance.hasFlameGrates = true;
			
			if (GC.customLevel)
				__instance.hasFlameGrates = __instance.customLevel.levelFeatures.Contains("FlameGrate");
			
			Random.InitState(__instance.randomSeedNum);
			Debug.Log("RANDOM NUMBER 3.5: " + Random.Range(0, 1000));
			GC.rnd.LoadRandomness();
			
			if (GC.sessionData.syringeReferences.Count == 0)
			{
				GC.syringeColorsAvailable.Add("Red");
				GC.syringeColorsAvailable.Add("Blue");
				GC.syringeColorsAvailable.Add("Cyan");
				GC.syringeColorsAvailable.Add("Green");
				GC.syringeColorsAvailable.Add("Purple");
				GC.syringeColorsAvailable.Add("Pink");
				GC.syringeColorsAvailable.Add("White");
				GC.syringeColorsAvailable.Add("Yellow");
				GC.syringeColorsAvailable.Add("Orange");
				GC.syringeColorsAvailable.Add("Grey");
				GC.syringeColorsAvailable.Add("Black");
				GC.syringeColorsAvailable.Add("Brown");

				RandomList randomList = GC.randomSelection.randomListTable["SyringeContents"];
				
				for (int num26 = 0; num26 < randomList.elementListDefinition.Count; num26++)
				{
					RandomElement randomElement = randomList.elementListDefinition[num26];
					SyringeReference syringeReference = new SyringeReference();
					syringeReference.myEffect = randomElement.rName;
					int index5 = Random.Range(0, GC.syringeColorsAvailable.Count - 1);
					syringeReference.myColor = GC.syringeColorsAvailable[index5];
					GC.syringeReferences.Add(syringeReference);
					GC.sessionData.syringeReferences.Add(syringeReference);
					GC.syringeColorsAvailable.RemoveAt(index5);
				}
			}

			else if (GC.multiplayerMode)
			{
				RandomList randomList2 = GC.randomSelection.randomListTable["SyringeContents"];

				for (int num27 = 0; num27 < randomList2.elementListDefinition.Count; num27++)
					Random.Range(0, GC.syringeColorsAvailable.Count - 1);
			}

			Random.InitState(__instance.randomSeedNum);
			Debug.Log("RANDOM NUMBER 4: " + Random.Range(0, 1000));
			
			if (GC.basicSpawns)
			{
				if (highFrameRate)
				{
					List<SpawnerBasic> wallList = new List<SpawnerBasic>();
					List<Chunk> wallListChunks = new List<Chunk>();

					for (int num28 = 0; num28 < __instance.spawnersBasic.Count; num28++)
					{
						SpawnerBasic spawnerBasic9 = __instance.spawnersBasic[num28];
						Chunk item3 = __instance.startingChunkReals[num28];

						if (spawnerBasic9.spawnerType == "Light")
						{
							wallList.Add(spawnerBasic9);
							wallListChunks.Add(item3);
						}
					}

					for (int num29 = 0; num29 < wallList.Count; num29++)
					{
						SpawnerBasic value7 = wallList[num29];
						Chunk value8 = wallListChunks[num29];
						int index6 = Random.Range(num29, wallList.Count);
						wallList[num29] = wallList[index6];
						wallList[index6] = value7;
						wallListChunks[num29] = wallListChunks[index6];
						wallListChunks[index6] = value8;
					}

					int num8;

					for (int i = 0; i < wallList.Count; i = num8 + 1)
					{
						SpawnerBasic spawnerBasic10 = wallList[i];
						Chunk chunk4 = wallListChunks[i];
						Random.InitState(__instance.randomSeedNum + i);
						GC.basicSpawn.DetermineSpawnType(spawnerBasic10, chunk4);
						num8 = lightSpawners;
						lightSpawners = num8 + 1;
						__instance.spawnersBasic.Remove(spawnerBasic10);
						__instance.startingChunkReals.Remove(chunk4);

						if (Time.realtimeSinceStartup - realtimeSinceStartup > maxChunkTime)
						{
							yield return null;

							realtimeSinceStartup = Time.realtimeSinceStartup;
						}

						num8 = i;
					}

					wallList = null;
					wallListChunks = null;
				}
				else
					for (int num30 = 0; num30 < __instance.spawnersBasic.Count; num30++)
					{
						SpawnerBasic spawnerBasic11 = __instance.spawnersBasic[num30];
						Chunk startingChunkReal3 = __instance.startingChunkReals[num30];

						if (spawnerBasic11.spawnerType == "Light")
						{
							GC.basicSpawn.DetermineSpawnType(spawnerBasic11, startingChunkReal3);
							int num8 = lightSpawners;
							lightSpawners = num8 + 1;
						}
					}
			}
			else if (highFrameRate)
			{
				List<Spawner> wallList2 = new List<Spawner>();

				for (int num31 = 0; num31 < __instance.spawners.Count; num31++)
				{
					Spawner component10 = __instance.spawners[num31].GetComponent<Spawner>();

					if (component10.spawnerType == "Light")
						wallList2.Add(component10);
				}

				for (int num32 = 0; num32 < wallList2.Count; num32++)
				{
					Spawner value9 = wallList2[num32];
					int index7 = Random.Range(num32, wallList2.Count);
					wallList2[num32] = wallList2[index7];
					wallList2[index7] = value9;
				}
				
				int num8;
				
				for (int i = 0; i < wallList2.Count; i = num8 + 1)
				{
					Spawner spawner3 = wallList2[i];
					Random.InitState(__instance.randomSeedNum + i);
					spawner3.DetermineSpawnType();
					num8 = lightSpawners;
					lightSpawners = num8 + 1;
					__instance.spawners.Remove(spawner3.gameObject);
				
					if (Time.realtimeSinceStartup - realtimeSinceStartup > maxChunkTime)
					{
					
						yield return null;
						realtimeSinceStartup = Time.realtimeSinceStartup;
					}

					num8 = i;
				}

				wallList2 = null;
			}
			else
				for (int num33 = 0; num33 < __instance.spawners.Count; num33++)
				{
					Spawner component11 = __instance.spawners[num33].GetComponent<Spawner>();

					if (component11.spawnerType == "Light")
					{
						component11.DetermineSpawnType();
						int num8 = lightSpawners;
						lightSpawners = num8 + 1;
					}
				}

			Random.InitState(__instance.randomSeedNum);
			Debug.Log("RANDOM NUMBER 4.5: " + Random.Range(0, 1000));

			if (GC.serverPlayer || GC.levelType == "HomeBase")
			{
				if (GC.basicSpawns)
				{
					if (highFrameRate)
					{
						List<SpawnerBasic> wallList = new List<SpawnerBasic>();
						List<Chunk> wallListChunks = new List<Chunk>();

						for (int num34 = 0; num34 < __instance.spawnersBasic.Count; num34++)
						{
							SpawnerBasic spawnerBasic12 = __instance.spawnersBasic[num34];
							Chunk item4 = __instance.startingChunkReals[num34];

							if (spawnerBasic12.spawnerType != "Wall" && spawnerBasic12.spawnerType != "Floor" && spawnerBasic12.spawnerType != "Light")
							{
								wallList.Add(spawnerBasic12);
								wallListChunks.Add(item4);
							}
						}

						for (int num35 = 0; num35 < wallList.Count; num35++)
						{
							SpawnerBasic value10 = wallList[num35];
							Chunk value11 = wallListChunks[num35];
							int index8 = Random.Range(num35, wallList.Count);
							wallList[num35] = wallList[index8];
							wallList[index8] = value10;
							wallListChunks[num35] = wallListChunks[index8];
							wallListChunks[index8] = value11;
						}

						int num8;

						for (int i = 0; i < wallList.Count; i = num8 + 1)
						{
							SpawnerBasic spawnerBasic13 = wallList[i];
							Chunk chunk5 = wallListChunks[i];
							Random.InitState(__instance.randomSeedNum + i);
							GC.basicSpawn.DetermineSpawnType(spawnerBasic13, chunk5);
							__instance.spawnersBasic.Remove(spawnerBasic13);
							__instance.startingChunkReals.Remove(chunk5);

							if (Time.realtimeSinceStartup - realtimeSinceStartup > maxChunkTime)
							{
								yield return null;

								realtimeSinceStartup = Time.realtimeSinceStartup;
							}

							num8 = i;
						}

						wallList = null;
						wallListChunks = null;
					}
					else
						for (int num36 = 0; num36 < __instance.spawnersBasic.Count; num36++)
						{
							SpawnerBasic spawnerBasic14 = __instance.spawnersBasic[num36];
							Chunk startingChunkReal4 = __instance.startingChunkReals[num36];

							if (spawnerBasic14.spawnerType != "Wall" && spawnerBasic14.spawnerType != "Floor" && spawnerBasic14.spawnerType != "Light")
								GC.basicSpawn.DetermineSpawnType(spawnerBasic14, startingChunkReal4);
						}
				}
				else if (highFrameRate)
				{
					List<Spawner> wallList2 = new List<Spawner>();

					for (int num37 = 0; num37 < __instance.spawners.Count; num37++)
					{
						Spawner component12 = __instance.spawners[num37].GetComponent<Spawner>();

						if (component12.spawnerType != "Wall" && component12.spawnerType != "Floor" && component12.spawnerType != "Light")
							wallList2.Add(component12);
					}

					for (int num38 = 0; num38 < wallList2.Count; num38++)
					{
						Spawner value12 = wallList2[num38];
						int index9 = Random.Range(num38, wallList2.Count);
						wallList2[num38] = wallList2[index9];
						wallList2[index9] = value12;
					}

					int num8;

					for (int i = 0; i < wallList2.Count; i = num8 + 1)
					{
						Spawner spawner4 = wallList2[i];
						Random.InitState(__instance.randomSeedNum + i);

						if (__instance.memoryTest)
						{
							if (spawner4.spawnerType == "Agent")
								GC.spawnerMain.SpawnAgent(spawner4.transform.position, null, spawner4.name);
						}
						else
							spawner4.DetermineSpawnType();

						__instance.spawners.Remove(spawner4.gameObject);

						if (Time.realtimeSinceStartup - realtimeSinceStartup > maxChunkTime)
						{
							yield return null;

							if (__instance.memoryTest)
							{
								yield return null;
								yield return null;
							}

							realtimeSinceStartup = Time.realtimeSinceStartup;
						}

						num8 = i;
					}

					wallList2 = null;
				}
				else
					for (int num39 = 0; num39 < __instance.spawners.Count; num39++)
					{
						Spawner component13 = __instance.spawners[num39].GetComponent<Spawner>();

						if (component13.spawnerType != "Wall" && component13.spawnerType != "Floor" && component13.spawnerType != "Light")
							component13.DetermineSpawnType();
					}
			}

			__instance.spawnersBasic.Clear();
			__instance.startingChunkReals.Clear();
			Debug.Log("RANDOM NUMBER 5: " + Random.Range(0, 1000));
			
			if (!GC.streamingWorld)
			{
				for (int num40 = 0; num40 < __instance.levelSizeAxis; num40++)
					for (int num41 = 0; num41 < __instance.levelSizeAxis; num41++)
					{
						int num42 = num40 * 16;
						int num43 = num40 * 16 + 16;
						int num44 = 160 - num41 * 16;
						int num45 = 160 - num41 * 16 - 16;

						for (int num46 = num42; num46 < num43; num46++)
							for (int num47 = num44; num47 > num45; num47--)
								if (num46 == 0 || num47 == 160 || num46 == (__instance.levelSizeAxis - 1) * 16 + 16 - 1 || num47 == 160 - (__instance.levelSizeAxis - 1) * 16 - 16 + 1)
								{
									___tilemapWalls.ClearTile(num46, num47 - 1, 0);
									TileData tileData = __instance.tileInfo.tileArray[num46, num47 - 1];
									tileData.wallMaterialOffset = 0;
									tileData.wallMaterialOffsetTop = 0;
									tileData.wallMaterial = wallMaterialType.None;
									___tilemapFloors.SetTile(num46, num47 - 1, 0, 206);
									___tilemapFloors2.SetTile(num46, num47 - 1, 0, 206);
									tileData.hole = true;

									if (num47 == 160 - (__instance.levelSizeAxis - 1) * 16 - 16 + 1 && num46 != 0 && num46 != (__instance.levelSizeAxis - 1) * 16 + 16 - 1)
										___tilemapFloors.SetTile(num46, num47 - 1, 0, GC.Choose<int>(194, 208, 209, 210, 211));
									else
										___tilemapFloors.SetTile(num46, num47 - 1, 0, 193);
									
									tileData.floorMaterialOffset = 192;
									tileData.floorMaterial = floorMaterialType.Hole;
								}
					}

				if (GC.levelShape == 0)
					for (int num48 = 0; num48 < __instance.levelSizeAxis; num48++)
						for (int num49 = 0; num49 < __instance.levelSizeAxis; num49++)
						{
							int num50 = num48 * 16;
							int num51 = num48 * 16 + 16;
							int num52 = 160 - num49 * 16;
							int num53 = 160 - num49 * 16 - 16;

							for (int num54 = num50; num54 < num51; num54++)
								for (int num55 = num52; num55 > num53; num55--)
									if (num54 % 16 == 15 && num55 % 16 == 0)
									{
										if (__instance.tileInfo.tileArray[num54, num55 - 1].wallMaterial != wallMaterialType.None && 
											__instance.tileInfo.tileArray[num54 + 1, num55].wallMaterial != wallMaterialType.None && 
											__instance.tileInfo.tileArray[num54 - 1, num55 - 1].wallMaterial != wallMaterialType.None && 
											__instance.tileInfo.tileArray[num54 - 1, num55 - 2].wallMaterial != wallMaterialType.None && 
											__instance.tileInfo.tileArray[num54, num55 - 2].wallMaterial != wallMaterialType.None && 
											__instance.tileInfo.tileArray[num54 + 1, num55 - 1].wallMaterial == wallMaterialType.None && 
											__instance.tileInfo.tileArray[num54, num55].wallMaterial == wallMaterialType.None && 
											__instance.tileInfo.tileArray[num54, num55].wallMaterial == wallMaterialType.None && 
											__instance.tileInfo.tileArray[num54 - 1, num55].wallMaterial == wallMaterialType.None)
										{
											int tile = 0;


											if (BMChallenges.IsChallengeFromListActive(cChallenge.FloorsAndFeatures) || BMChallenges.IsChallengeFromListActive(cChallenge.WallsAndFloors))
												tile = int.Parse(GC.rnd.RandomSelect(GetFloorTileGroupFromMutator(), "RandomFloorsWalls"));
											else if (GC.levelTheme == 0)
												tile = int.Parse(GC.rnd.RandomSelect(vFloorTileGroup.Slums, "RandomFloorsWalls"));
											else if (GC.levelTheme == 1)
												tile = int.Parse(GC.rnd.RandomSelect(vFloorTileGroup.Industrial, "RandomFloorsWalls"));
											else if (GC.levelTheme == 2)
												tile = int.Parse(GC.rnd.RandomSelect(vFloorTileGroup.Park, "RandomFloorsWalls"));
											else if (GC.levelTheme == 3)
												tile = int.Parse(GC.rnd.RandomSelect(vFloorTileGroup.Downtown, "RandomFloorsWalls"));
											else if (GC.levelTheme == 4)
												tile = int.Parse(GC.rnd.RandomSelect(vFloorTileGroup.Uptown, "RandomFloorsWalls"));
											else if (GC.levelTheme == 5)
												tile = int.Parse(GC.rnd.RandomSelect(vFloorTileGroup.MayorVillage, "RandomFloorsWalls"));
											
											TileData tileData2 = __instance.tileInfo.tileArray[num54, num55 - 1];
											___tilemapWalls.ClearTile(num54, num55 - 1, 0);
											tileData2.wallMaterialOffset = 0;
											tileData2.wallMaterialOffsetTop = 0;
											tileData2.wallMaterial = wallMaterialType.None;
											___tilemapFloors2.SetTile(num54, num55 - 1, 0, tile);
											TileData tileData3 = __instance.tileInfo.tileArray[num54 + 1, num55];
											___tilemapWalls.ClearTile(num54 + 1, num55, 0);
											tileData3.wallMaterialOffset = 0;
											tileData3.wallMaterialOffsetTop = 0;
											tileData3.wallMaterial = wallMaterialType.None;
											___tilemapFloors2.SetTile(num54 + 1, num55, 0, tile);
										}
									}
									else if (
										num54 % 16 == 0 && num55 % 16 == 0 && 
										__instance.tileInfo.tileArray[num54, num55 - 1].wallMaterial != wallMaterialType.None && 
										__instance.tileInfo.tileArray[num54 + 1, num55 - 1].wallMaterial != wallMaterialType.None && 
										__instance.tileInfo.tileArray[num54 + 1, num55 - 2].wallMaterial != wallMaterialType.None && 
										__instance.tileInfo.tileArray[num54, num55 - 2].wallMaterial != wallMaterialType.None && 
										__instance.tileInfo.tileArray[num54 - 1, num55].wallMaterial != wallMaterialType.None && 
										__instance.tileInfo.tileArray[num54 - 1, num55 - 2].wallMaterial == wallMaterialType.None && 
										__instance.tileInfo.tileArray[num54 - 1, num55 - 1].wallMaterial == wallMaterialType.None && 
										__instance.tileInfo.tileArray[num54, num55].wallMaterial == wallMaterialType.None && 
										__instance.tileInfo.tileArray[num54 + 1, num55].wallMaterial == wallMaterialType.None)
									{
										int tile2 = 0;


										if (BMChallenges.IsChallengeFromListActive(cChallenge.FloorsAndFeatures) || BMChallenges.IsChallengeFromListActive(cChallenge.WallsAndFloors))
											tile2 = int.Parse(GC.rnd.RandomSelect(GetFloorTileGroupFromMutator(), "RandomFloorsWalls"));
										else if (GC.levelTheme == 0)
											tile2 = int.Parse(GC.rnd.RandomSelect(vFloorTileGroup.Slums, "RandomFloorsWalls"));
										else if (GC.levelTheme == 1)
											tile2 = int.Parse(GC.rnd.RandomSelect(vFloorTileGroup.Industrial, "RandomFloorsWalls"));
										else if (GC.levelTheme == 2)
											tile2 = int.Parse(GC.rnd.RandomSelect(vFloorTileGroup.Park, "RandomFloorsWalls"));
										else if (GC.levelTheme == 3)
											tile2 = int.Parse(GC.rnd.RandomSelect(vFloorTileGroup.Downtown, "RandomFloorsWalls"));
										else if (GC.levelTheme == 4)
											tile2 = int.Parse(GC.rnd.RandomSelect(vFloorTileGroup.Uptown, "RandomFloorsWalls"));
										else if (GC.levelTheme == 5)
											tile2 = int.Parse(GC.rnd.RandomSelect(vFloorTileGroup.MayorVillage, "RandomFloorsWalls"));
										
										TileData tileData4 = __instance.tileInfo.tileArray[num54, num55 - 1];
										___tilemapWalls.ClearTile(num54, num55 - 1, 0);
										tileData4.wallMaterialOffset = 0;
										tileData4.wallMaterialOffsetTop = 0;
										tileData4.wallMaterial = wallMaterialType.None;
										___tilemapFloors2.SetTile(num54, num55 - 1, 0, tile2);
										TileData tileData5 = __instance.tileInfo.tileArray[num54 - 1, num55];
										___tilemapWalls.ClearTile(num54 - 1, num55, 0);
										tileData5.wallMaterialOffset = 0;
										tileData5.wallMaterialOffsetTop = 0;
										tileData5.wallMaterial = wallMaterialType.None;
										___tilemapFloors2.SetTile(num54 - 1, num55, 0, tile2);
									}
						}
			}

			if ((GC.challenges.Contains(cChallenge.PoliceState) || GC.levelTheme == 4 || (GC.challenges.Contains("MixedUpLevels") && GC.percentChance(33))) && 
					GC.staticChunk == "" && GC.serverPlayer && GC.levelType == "Normal" && !GC.challenges.Contains(cChallenge.AnCapistan))
			{
				GC.tileInfo.PlaceLockdownWalls();
				__instance.hasLockdownWalls = true;
			}
			
			if (GC.customLevel)
				__instance.hasLockdownWalls = __instance.customLevel.levelFeatures.Contains("LockdownWall");
			
			Debug.Log("RANDOM NUMBER 6: " + Random.Range(0, 1000));
			
			if (GC.serverPlayer && !GC.streamingWorld)
			{
				__instance.tileInfo.SetupWallTiles(false, null);
				__instance.tileInfo.SetupFloorTiles(false, null);
				GameObject[] array = GameObject.FindGameObjectsWithTag("ChunkHelper");
				int num8;
			
				for (num8 = 0; num8 < array.Length; num8++)
					Object.Destroy(array[num8]);
				
				if (GC.lightingType == "Full")
					GC.poolsScene.NLWalls = false;
				else
					GC.poolsScene.NLWalls = true;
				
				for (int i = 0; i < GC.tileInfo.tileArray.GetLength(0); i = num8 + 1)
				{
					for (int j = 0; j < GC.tileInfo.tileArray.GetLength(1); j = num8 + 1)
					{
						TileData myTileData = GC.tileInfo.tileArray[i, j];
						GC.tileInfo.BuildWallObject(myTileData, null, false);

						if (Time.realtimeSinceStartup - realtimeSinceStartup > maxChunkTime)
						{
							yield return null;
						
							realtimeSinceStartup = Time.realtimeSinceStartup;
						}

						num8 = j;
					}

					num8 = i;
				}
			}

			Debug.Log("RANDOM NUMBER 7: " + Random.Range(0, 1000));
			__instance.StartCoroutine(__instance.SetupMore());

			yield break;

			yield break;
		}
		public static bool LoadLevel_SetupMore3_3_Prefix(LoadLevel __instance, ref tk2dTileMap ___tilemapFloors4, ref Minimap ___minimap, ref IEnumerator __result) // Replacement
		{
			BMLog("LoadLevel_SetupMore3_3_Prefix");

			// Structure advised by Abbysssal for patch-replacing IEnumerators.
			__result = LoadLevel_SetupMore3_3_Prefix_Replacement(__instance, ___tilemapFloors4, ___minimap);

			return false;
		}
		public static IEnumerator LoadLevel_SetupMore3_3_Prefix_Replacement(LoadLevel __instance, tk2dTileMap ___tilemapFloors4, Minimap ___minimap) // Non-Patch
		{
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

					if (GC.challenges.Contains(cChallenge.ArcologyEcology) || GC.challenges.Contains(cChallenge.PoliceState))
						hasSlimeBarrels = false;

					if (GC.challenges.Contains(cChallenge.AnCapistan))
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

					if (GC.challenges.Contains(cChallenge.ArcologyEcology) || GC.challenges.Contains(cChallenge.PoliceState))
						hasOilSpills = false;

					if (GC.challenges.Contains(cChallenge.AnCapistan))
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

					if (GC.challenges.Contains(cChallenge.ArcologyEcology) || GC.challenges.Contains(cChallenge.PoliceState))
						hasTrashCans = true;

					if (GC.challenges.Contains(cChallenge.AnCapistan))
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
					#region Public Security Cams for Police State
					if (GC.challenges.Contains(cChallenge.PoliceState))
					{
						Debug.Log("Loading Public Security Cams");

						int bigTries = (int)(15 * __instance.levelSizeModifier);
						List<int> spawnedInChunks = new List<int>();
						int numObjectsIterator;

						for (int numObjects = 0; numObjects < bigTries; numObjects = numObjectsIterator + 1)
						{
							Vector2 candidateSpot = Vector2.zero;
							int attempts = 0;

							do
							{
								candidateSpot = GC.tileInfo.FindRandLocationNearWall(0.64f);

								if (candidateSpot != Vector2.zero)
								{
									TileData spotTileData = GC.tileInfo.GetTileData(candidateSpot);

									if (GC.tileInfo.GetTileData(new Vector2(candidateSpot.x, candidateSpot.y + 0.64f)).owner == 0 &&
										GC.tileInfo.GetTileData(new Vector2(candidateSpot.x + 0.64f, candidateSpot.y)).owner == 0 &&
										GC.tileInfo.GetTileData(new Vector2(candidateSpot.x, candidateSpot.y - 0.64f)).owner == 0 &&
										GC.tileInfo.GetTileData(new Vector2(candidateSpot.x - 0.64f, candidateSpot.y)).owner == 0)
										candidateSpot = Vector2.zero;

									if (!GC.tileInfo.IsOverlapping(new Vector2(candidateSpot.x, candidateSpot.y + 0.64f), "Wall") &&
										!GC.tileInfo.IsOverlapping(new Vector2(candidateSpot.x, candidateSpot.y - 0.64f), "Wall") &&
										!GC.tileInfo.IsOverlapping(new Vector2(candidateSpot.x + 0.64f, candidateSpot.y), "Wall") &&
										!GC.tileInfo.IsOverlapping(new Vector2(candidateSpot.x - 0.64f, candidateSpot.y), "Wall"))
										candidateSpot = Vector2.zero;

									if (GC.tileInfo.IsOverlapping(candidateSpot, "ObjectRealSprite", 0.64f))
										candidateSpot = Vector2.zero;

									if (spawnedInChunks.Contains(spotTileData.chunkID))
										candidateSpot = Vector2.zero;

									if (GC.tileInfo.DestroyIfBetweenWalls(candidateSpot))
										candidateSpot = Vector2.zero;
								}

								attempts++;
							}
							while ((candidateSpot == Vector2.zero || Vector2.Distance(candidateSpot, GC.playerAgent.tr.position) < 5f) && attempts < 100);

							if (candidateSpot != Vector2.zero)
							{
								GC.spawnerMain.spawnObjectReal(candidateSpot, null, vObject.SecurityCam).ShiftTowardWalls();
								TileData tileData5 = GC.tileInfo.GetTileData(candidateSpot);
								spawnedInChunks.Add(tileData5.chunkID);
								Random.InitState(__instance.randomSeedNum + numObjects + ++randomCount);

								if (numObjects < bigTries - 1 && GC.percentChance(25))
								{
									string adjoiningWall = "";
									Vector2 zero4 = Vector2.zero;
									Vector2 zero5 = Vector2.zero;

									if (GC.tileInfo.GetTileData(new Vector2(candidateSpot.x, candidateSpot.y + 0.64f)).wallMaterial != wallMaterialType.None)
									{
										zero4 = new Vector2(candidateSpot.x + 1.28f, candidateSpot.y);
										zero5 = new Vector2(candidateSpot.x - 1.28f, candidateSpot.y);
										adjoiningWall = "N";
									}
									else if (GC.tileInfo.GetTileData(new Vector2(candidateSpot.x, candidateSpot.y - 0.64f)).wallMaterial != wallMaterialType.None)
									{
										zero4 = new Vector2(candidateSpot.x + 1.28f, candidateSpot.y);
										zero5 = new Vector2(candidateSpot.x - 1.28f, candidateSpot.y);
										adjoiningWall = "S";
									}
									else if (GC.tileInfo.GetTileData(new Vector2(candidateSpot.x + 0.64f, candidateSpot.y)).wallMaterial != wallMaterialType.None)
									{
										zero4 = new Vector2(candidateSpot.x, candidateSpot.y + 1.28f);
										zero5 = new Vector2(candidateSpot.x, candidateSpot.y - 1.28f);
										adjoiningWall = "E";
									}
									else if (GC.tileInfo.GetTileData(new Vector2(candidateSpot.x - 0.64f, candidateSpot.y)).wallMaterial != wallMaterialType.None)
									{
										zero4 = new Vector2(candidateSpot.x, candidateSpot.y + 1.28f);
										zero5 = new Vector2(candidateSpot.x, candidateSpot.y - 1.28f);
										adjoiningWall = "W";
									}

									GC.tileInfo.GetTileData(zero4);
									bool allClear = true;

									if ((GC.tileInfo.GetTileData(new Vector2(zero4.x, zero4.y + 0.64f)).wallMaterial == wallMaterialType.None && adjoiningWall == "N") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x, zero4.y - 0.64f)).wallMaterial != wallMaterialType.None && adjoiningWall == "N") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x + 0.64f, zero4.y - 0.64f)).wallMaterial != wallMaterialType.None && adjoiningWall == "N") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x - 0.64f, zero4.y - 0.64f)).wallMaterial != wallMaterialType.None && adjoiningWall == "N") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x + 0.64f, zero4.y)).wallMaterial == wallMaterialType.None && adjoiningWall == "E") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x - 0.64f, zero4.y)).wallMaterial != wallMaterialType.None && adjoiningWall == "E") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x - 0.64f, zero4.y + 0.64f)).wallMaterial != wallMaterialType.None && adjoiningWall == "E") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x - 0.64f, zero4.y - 0.64f)).wallMaterial != wallMaterialType.None && adjoiningWall == "E") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x, zero4.y - 0.64f)).wallMaterial == wallMaterialType.None && adjoiningWall == "S") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x, zero4.y + 0.64f)).wallMaterial != wallMaterialType.None && adjoiningWall == "S") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x + 0.64f, zero4.y + 0.64f)).wallMaterial != wallMaterialType.None && adjoiningWall == "S") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x - 0.64f, zero4.y + 0.64f)).wallMaterial != wallMaterialType.None && adjoiningWall == "S") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x - 0.64f, zero4.y)).wallMaterial == wallMaterialType.None && adjoiningWall == "W") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x + 0.64f, zero4.y)).wallMaterial != wallMaterialType.None && adjoiningWall == "W") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x + 0.64f, zero4.y + 0.64f)).wallMaterial != wallMaterialType.None && adjoiningWall == "W") ||
										(GC.tileInfo.GetTileData(new Vector2(zero4.x + 0.64f, zero4.y - 0.64f)).wallMaterial != wallMaterialType.None && adjoiningWall == "W"))
										allClear = false;

									if (GC.tileInfo.IsOverlapping(zero4, "Anything"))
										allClear = false;

									if (GC.tileInfo.IsOverlapping(zero4, "ObjectRealSprite", 0.64f))
										allClear = false;

									if (allClear && zero4 != Vector2.zero)
									{
										GC.spawnerMain.spawnObjectReal(zero4, null, vObject.SecurityCam).ShiftTowardWalls();
										numObjectsIterator = numObjects;
										numObjects = numObjectsIterator + 1;
									}
									else
									{
										GC.tileInfo.GetTileData(zero5);
										allClear = true;

										if ((GC.tileInfo.GetTileData(new Vector2(zero5.x, zero5.y + 0.64f)).wallMaterial == wallMaterialType.None && adjoiningWall == "N") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x, zero5.y - 0.64f)).wallMaterial != wallMaterialType.None && adjoiningWall == "N") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x + 0.64f, zero5.y - 0.64f)).wallMaterial != wallMaterialType.None && adjoiningWall == "N") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x - 0.64f, zero5.y - 0.64f)).wallMaterial != wallMaterialType.None && adjoiningWall == "N") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x + 0.64f, zero5.y)).wallMaterial == wallMaterialType.None && adjoiningWall == "E") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x - 0.64f, zero5.y)).wallMaterial != wallMaterialType.None && adjoiningWall == "E") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x - 0.64f, zero5.y + 0.64f)).wallMaterial != wallMaterialType.None && adjoiningWall == "E") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x - 0.64f, zero5.y - 0.64f)).wallMaterial != wallMaterialType.None && adjoiningWall == "E") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x, zero5.y - 0.64f)).wallMaterial == wallMaterialType.None && adjoiningWall == "S") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x, zero5.y + 0.64f)).wallMaterial != wallMaterialType.None && adjoiningWall == "S") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x + 0.64f, zero5.y + 0.64f)).wallMaterial != wallMaterialType.None && adjoiningWall == "S") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x - 0.64f, zero5.y + 0.64f)).wallMaterial != wallMaterialType.None && adjoiningWall == "S") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x - 0.64f, zero5.y)).wallMaterial == wallMaterialType.None && adjoiningWall == "W") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x + 0.64f, zero5.y)).wallMaterial != wallMaterialType.None && adjoiningWall == "W") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x + 0.64f, zero5.y + 0.64f)).wallMaterial != wallMaterialType.None && adjoiningWall == "W") ||
											(GC.tileInfo.GetTileData(new Vector2(zero5.x + 0.64f, zero5.y - 0.64f)).wallMaterial != wallMaterialType.None && adjoiningWall == "W"))
											allClear = false;

										if (GC.tileInfo.IsOverlapping(zero5, "Anything"))
											allClear = false;

										if (GC.tileInfo.IsOverlapping(zero5, "ObjectRealSprite", 0.64f))
											allClear = false;

										if (allClear && zero5 != Vector2.zero)
										{
											GC.spawnerMain.spawnObjectReal(zero5, null, vObject.SecurityCam).ShiftTowardWalls();
											numObjectsIterator = numObjects;
											numObjects = numObjectsIterator + 1;
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
							numObjectsIterator = numObjects;
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

					if (GC.challenges.Contains(cChallenge.PoliceState) || GC.levelTheme == 3 || GC.levelTheme == 4 || __instance.hasLockdownWalls || (GC.challenges.Contains("MixedUpLevels") && GC.percentChance(20)))
						hasPoliceBoxes = true;

					if (GC.challenges.Contains(cChallenge.AnCapistan))
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

					if (GC.challenges.Contains(cChallenge.AnCapistan))
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

						if (manholeList.Count > 0)
							for (int i = 0; i < numObjects; i = bigTryCounter + 1)
							{
								int attemptsToAddHiddenAgentToManhole = 0;
								Manhole manhole;
								bool NoHiddenAgentMatch;

								//Hidden Agent Placement
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
									string text3 = GC.Choose<string>("Thief", "Thief", new string[]
									{
										"Thief",
										"Cannibal"
									});

									if ((!(text3 == "Thief") || !GC.challenges.Contains("ThiefNoSteal")) && (!(text3 == "Cannibal") || !GC.challenges.Contains("CannibalsDontAttack")))
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
									string text3 = GC.Choose<string>("Thief", "Thief", new string[]
									{
										"Thief",
										"Cannibal"
									});

									if ((!(text3 == "Thief") || !GC.challenges.Contains("ThiefNoSteal")) && (!(text3 == "Cannibal") || !GC.challenges.Contains("CannibalsDontAttack")))
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

					if (GC.challenges.Contains(cChallenge.ArcologyEcology) || GC.challenges.Contains(cChallenge.PoliceState))
						hasExplodingSlimeBarrels = false;

					if (GC.challenges.Contains(cChallenge.AnCapistan))
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

					if (GC.challenges.Contains(cChallenge.TransitExperiment) || GC.challenges.Contains(cChallenge.PoliceState))
						hasFlamingBarrels = false;

					if (GC.challenges.Contains(cChallenge.AnCapistan))
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

						if (GC.challenges.Contains(cChallenge.ArcologyEcology))
							ambience = vAmbience.Park;
						else 
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
							bigTries = SetRoamerCount(bigTries);
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
							bigTries = (int)(SetGangCount(bigTries) * (float)__instance.levelSizeMax / 30f);

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
							bigTries = SetMafiaCount(bigTries);
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
						SpawnRoamerSquad(agent, 4, vAgent.Ghost, __instance, false, 1);

					if (level >= 10)
					{
						if (agent.statusEffects.hasTrait(cTrait.MobDebt))
							SpawnRoamerSquad(agent, (int)((float)level * 1.66f), vAgent.Mobster, __instance, false, 4);
						else if (agent.statusEffects.hasTrait(cTrait.MobDebt_2))
							SpawnRoamerSquad(agent, (int)((float)level * 1.33f), vAgent.Mobster, __instance, false, 4);
					}

					if (agent.statusEffects.hasTrait(cTrait.MookMasher))
						SpawnRoamerSquad(agent, level * 2, vAgent.Goon, __instance, false, 4);

					if (agent.statusEffects.hasTrait(cTrait.Reinforcements))
						SpawnRoamerSquad(agent, 3, vAgent.ResistanceLeader, __instance, true, 1);
					else if (agent.statusEffects.hasTrait(cTrait.Reinforcements_2))
						SpawnRoamerSquad(agent, 6, vAgent.ResistanceLeader, __instance, true, 1);
				}
			}
		}
		public static void LoadLevel_SetupMore5_2(LoadLevel __instance) // Postfix
		{
			BMLog("LoadLevel_SetupMore5_2");

			BMAbilities.baseTimeScale = GameController.gameController.selectedTimeScale;
		}
		#endregion
		#region RandomWalls
		public static bool RandomWalls_fillWalls() // Replacement
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
		#region SpawnerBasic

		#endregion
		#region SpawnerFloor
		public static bool SpawnerFloor_spawn(string floorName, SpawnerFloor __instance, tk2dTileMap ___tilemapFloors, tk2dTileMap ___tilemapFloors3, tk2dTileMap ___tilemapFloors4) // Prefix
		{
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
		public static bool SpawnerObject_spawn(string objectRealName) // Prefix
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
