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

		// TODO: Set this somewhere so it doesn't waste processor
		public static float BulletRange(Agent agent) =>
			agent.statusEffects.hasTrait(cTrait.Ballistician_2) ? 100f :
			agent.statusEffects.hasTrait(cTrait.Ballistician) || 
			agent.statusEffects.hasTrait(cTrait.Sniper_2) ? 50f :
			agent.statusEffects.hasTrait(cTrait.Sniper) ? 25f :
			13.44f;

		// TODO: Set this somewhere so it doesn't waste processor
		public static float GlobalKnockbackFactor() =>
			GC.challenges.Contains(cChallenge.BoringPhysics) ? 0.10f :
			GC.challenges.Contains(cChallenge.SaveTheWalls) ? 0.50f :
			GC.challenges.Contains(vChallenge.BigKnockback) ? 1.50f :
			GC.challenges.Contains(cChallenge.WallWallopWorld) ? 5.00f :
			1.00f;
	}
}
