using RogueLibsCore;
using System.Collections.Generic;
using BepInEx.Logging;
using BunnyMod.Content.Logging;
using UnityEngine;
using Random = UnityEngine.Random;
using BunnyMod.Content.Traits;

namespace BunnyMod.Content.Custom
{
	public static class C_Combat
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		private static GameController GC => GameController.gameController;

		public static bool CanAgentMeleeHitGhost(Agent agent)
		{
			if (agent.statusEffects.hasTrait(cTrait.BlessedStrikes_2) || agent.statusEffects.hasTrait(cTrait.InfernalStrikes_2))
				return true;
			else if (agent.inventory.equippedWeapon.invItemName == vItem.Fist)
				return (agent.statusEffects.hasTrait(cTrait.BlessedStrikes) || agent.statusEffects.hasTrait(cTrait.InfernalStrikes));

			return false;
		}

		public static float GetBulletRange(Agent agent)
		{
			float maxBulletRange = 13.44f;

			if (agent.statusEffects.hasTrait(cTrait.Ballistician))
				maxBulletRange = 50f;
			else if (agent.statusEffects.hasTrait(cTrait.Ballistician_2))
				maxBulletRange = 100f;
			else if (agent.statusEffects.hasTrait(cTrait.Sniper))
				maxBulletRange = 25f;

			return maxBulletRange;
		}

		public static float GetGlobalKnockBackMultiplier()
		{
			float baseAmt = 1f;

			if (GC.challenges.Contains(vChallenge.BigKnockback))
				baseAmt = 1.50f;
			else if (GC.challenges.Contains(cChallenge.SaveTheWalls))
				baseAmt = 0.50f;
			else if (GC.challenges.Contains(cChallenge.BoringPhysics))
				baseAmt = 0.10f;
			else if (GC.challenges.Contains(cChallenge.WallWallopWorld))
				baseAmt = 5.00f;

			return baseAmt;
		}
	}
}
