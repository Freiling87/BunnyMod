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
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace BunnyMod.Content.Patches.P_LevelGen
{
	[HarmonyPatch(declaringType: typeof(SpawnerFloor))]
	public static class P_SpawnerFloor
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		public static GameController GC => GameController.gameController;

		[HarmonyPrefix, HarmonyPatch(methodName:nameof(SpawnerFloor.spawn), argumentTypes: new[] { typeof(string) })]
		public static bool spawn_Prefix(string floorName, SpawnerFloor __instance, ref tk2dTileMap ___tilemapFloors, ref tk2dTileMap ___tilemapFloors3,
				ref tk2dTileMap ___tilemapFloors4) 
		{
			// Floor Mods
			// Seems to affect only Home Base

			if (BMLevelGen.GetActiveFloorMod() == null)
				return true;

			floorName = BMLevelGen.GetFloorTile();

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
						if (GC.levelTheme != 1 && GC.levelTheme != 2 &&
								(GC.levelTheme < 3 || !(__instance.transform.parent.GetComponent<Chunk>().description == "Mall")) &&
								(!__instance.transform.parent.GetComponent<Chunk>().userCreated ||
										!(__instance.transform.parent.GetComponent<Chunk>().description != "Hideout")))
						{
							Object.Destroy(__instance.gameObject);
							return false;
						}

						num5 = 628;
						__instance.tileInfo.tileArray[num, num2].conveyorBelt = true;
						list.Add(__instance.tileInfo.tileArray[num, num2]);
						list2.Add(Object.Instantiate<GameObject>(GC.spawnerMain.conveyorBeltImagePrefab, new Vector2((float)num * 0.64f, (float)num2 * 0.64f),
								Quaternion.Euler(0f, 0f, 0f)));
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
						if (GC.levelTheme != 1 && (!__instance.transform.parent.GetComponent<Chunk>().userCreated ||
								!(__instance.transform.parent.GetComponent<Chunk>().description != "Hideout")))
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
						if (GC.levelTheme != 1 && GC.levelTheme != 2 && GC.levelTheme != 4 && GC.levelTheme != 5 &&
								(!__instance.transform.parent.GetComponent<Chunk>().userCreated ||
										!(__instance.transform.parent.GetComponent<Chunk>().description != "Hideout")))
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
				GC.spawnerMain.StartCoroutine(GC.spawnerMain.SpawnHoleLate(null, __instance.transform.position, __instance.transform.localScale,
						__instance.transform.rotation, true, false));
			else if (floorName == "Water" || floorName == "Canal" || floorName == "Pool")
			{
				Chunk component2 = __instance.transform.parent.GetComponent<Chunk>();

				if (component2 == null)
					component2 = __instance.transform.parent.transform.parent.GetComponent<Chunk>();

				Water water = GC.spawnerMain.SpawnWater(null, __instance.transform.position, __instance.transform.localScale, __instance.transform.rotation,
						true, component2);

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

					GameObject gameObject = GC.spawnerMain.SpawnConveyorBelt(null, __instance.transform.position, __instance.transform.localScale,
							__instance.transform.rotation, text, true, component3).gameObject;
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
						GameObject gameObject2 =
								Object.Instantiate<GameObject>(GC.spawnerMain.ownerPropertyColliderPrefab, position, Quaternion.Euler(0f, 0f, 0f));
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
	}
}
