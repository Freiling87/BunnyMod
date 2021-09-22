using RogueLibsCore;
using System.Collections.Generic;
using BepInEx.Logging;
using BunnyMod.Content.Logging;
using UnityEngine;
using Random = UnityEngine.Random;
using BunnyMod.Content.Traits;

namespace BunnyMod.Content.Custom
{
	public static class C_LevelGen
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		private static GameController GC => GameController.gameController;

		public static int GangCount(int vanilla)
		{
			logger.LogDebug("SetGangCount");

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
			logger.LogDebug("GetWallBorderTypeFromMutator: '" + GetWallMutator() + "'");

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
			logger.LogDebug("GetWallTypeFromMutator: '" + GetWallMutator() + "'");

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
	}
}
