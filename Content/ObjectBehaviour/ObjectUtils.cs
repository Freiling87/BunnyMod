using BunnyMod.Content.Extensions;
using Google2u;
using JetBrains.Annotations;
using UnityEngine;

namespace BunnyMod.Content.ObjectBehaviour
{
	public static class ObjectUtils
	{
		public static void SpawnSuspiciousNoise(int strikes, float volume, Agent causer, ObjectReal targetObject)
		{
			if (causer.HasTrait(StatusEffectNameDB.rowIds.OperateSecretly) || !targetObject.functional || volume <= 0f)
			{
				return;
			}

			GameController gc = GameController.gameController;
			Vector3 noisePosition = targetObject.tr.position;

			gc.spawnerMain.SpawnNoise(noisePosition, volume, causer, "Normal", causer);
			gc.audioHandler.Play(targetObject, "Hack");
			targetObject.SpawnParticleEffect("Hack", noisePosition);
			gc.spawnerMain.SpawnStateIndicator(targetObject, "HighVolume");
			gc.OwnCheck(causer, targetObject.go, "Normal", strikes);
		}

		public static void HideInObject(Agent agent, ObjectReal targetObject)
		{
			agent.SetInvisible(false);
			agent.statusEffects.BecomeHidden(targetObject);
			targetObject.StopInteraction();
		}

		[UsedImplicitly]
		public static bool DidStopInteraction(PlayfieldObject playfieldObject)
		{
			return playfieldObject.interactingAgent == null;
		}
	}
}