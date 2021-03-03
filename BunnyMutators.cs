using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using RogueLibsCore;
using UnityEngine;
using UnityEngine.Networking;
using Pathfinding;

namespace BunnyMod
{
	public class BunnyMutators
	{
		public void Awake()
		{
			InitializeMutators();

			BunnyHeader.MainInstance.PatchPostfix(typeof(Bullet), "SetupBullet", GetType(), "Bullet_SetupBullet", new Type[0] { });

			BunnyHeader.MainInstance.PatchPrefix(typeof(PoolsScene), "SpawnWall", GetType(), "PoolsScene_SpawnWall", new Type[7] { typeof(bool), typeof(wallMaterialType), typeof(Vector3), typeof(TileData), typeof(int), typeof(int), typeof(bool)});
		}
		public static void InitializeMutators()
		{
			#region Map Modification
			CustomMutator shantyTown = RogueLibs.CreateCustomMutator("Shantytown", true,
				new CustomNameInfo("Shantytown"),
				new CustomNameInfo("Hard mode for Firefighters, easy mode for arsonists. Fun mode for psychopaths."));
			shantyTown.Available = true;
			shantyTown.Conflicting.AddRange(new string[] { "SteelCity" });

			CustomMutator cityOfSteel = RogueLibs.CreateCustomMutator("CityOfSteel", true,
				new CustomNameInfo("CityOfSteel"),
				new CustomNameInfo("A gleaming city of steel. It all smells like steel cleaner, which I guess you can get used to."));
			cityOfSteel.Available = true;
			cityOfSteel.Conflicting.AddRange(new string[] { "Shantytown" });
			#endregion

			CustomMutator scarierGuns = RogueLibs.CreateCustomMutator("Scarier Guns", true,
				new CustomNameInfo("Scarier Guns"),
				new CustomNameInfo("Bullets act a little more realistically. Get good, or get dead."));
			scarierGuns.Available = true;
		}

		#region Bullet
		public static void Bullet_SetupBullet(Bullet __instance) // Postfix
		{
			if (BunnyHeader.gc.challenges.Contains("ScarierGuns"))
			{
				__instance.damage *= 2;
				__instance.speed *= 2;
			}
		}
		#endregion
		#region PoolsScene
		public static void PoolsScene_SpawnWall(bool isFront, wallMaterialType myWallMaterial, Vector3 myPos, TileData myTileData, int streamingSize, int streamingOffset, bool buildingStreamingChunk) // Prefix
		{
			if (BunnyHeader.gc.challenges.Contains("ShantyTown"))
			{
				if (myWallMaterial == wallMaterialType.Normal || myWallMaterial == wallMaterialType.Steel)
					myWallMaterial = wallMaterialType.Wood;
				else if (myWallMaterial == wallMaterialType.Bars)
					myWallMaterial = wallMaterialType.BarbedWire;
			}
			else if (BunnyHeader.gc.challenges.Contains("CityOfSteel"))
			{
				if (myWallMaterial == wallMaterialType.Normal || myWallMaterial == wallMaterialType.Wood)
					myWallMaterial = wallMaterialType.Steel;
				else if (myWallMaterial == wallMaterialType.BarbedWire)
					myWallMaterial = wallMaterialType.Bars;
			}
		}
		#endregion
	}
}
