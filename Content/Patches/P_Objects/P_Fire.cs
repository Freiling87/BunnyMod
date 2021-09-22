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
using Object = UnityEngine.Object;

namespace BunnyMod.Content.Patches
{
	[HarmonyPatch(declaringType:typeof(Fire))]
	public static class P_Fire
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		public static GameController GC => GameController.gameController;

		[HarmonyPrefix,HarmonyPatch(methodName:nameof(Fire.UpdateFire))]
		public static bool UpdateFire_Prefix(Fire __instance)
		{
			float lifetime = 10f;
			float spreadTime = 5f;
			float generationCap = 6;

			if (BMChallenges.IsChallengeFromListActive(cChallenge.AffectsFires))
			{
				if (GC.challenges.Contains(cChallenge.NapalmSprings))
				{
					lifetime = 20f;
					spreadTime = 15f;
					generationCap = 99f;
				}
				else if (GC.challenges.Contains(cChallenge.Mildfire))
				{
					lifetime = 6f;
					spreadTime = 3f;
					generationCap = 2;
				}

				if (GC.challenges.Contains(cChallenge.GasolineHumidity))
					spreadTime = lifetime - 2f;
			}

			if (GC.serverPlayer)
			{
				if (!__instance.hasBurningObject && !__instance.noObject)
				{
					__instance.timeLeft = 0f;

					if (!__instance.destroying)
					{
						try
						{
							__instance.DestroyMe();
						}
						catch
						{
							Debug.LogError(string.Concat(new object[] { "Fire DestroyMe Error 1: ", __instance, " - ", __instance.UID }));
						}
					}
				}

				if (__instance.timeLeft <= 0f)
					goto IL_460;

				if (__instance.burningObjectAgent && __instance.hasObjectAgent)
				{
					try
					{
						__instance.objectAgent.tr.position = __instance.tr.position;
					}
					catch { }
				}

				__instance.timeLeft -= Time.deltaTime;
				__instance.timeToOilSpread -= Time.deltaTime;

				if (__instance.timeLeft <= spreadTime && !__instance.fireHasSpread && __instance.canSpread)
				{
					try
					{
						if (__instance.generation < generationCap && GC.serverPlayer)
							__instance.StartCoroutine(__instance.FireSpread());

						__instance.fireHasSpread = true;
					}
					catch
					{
						Debug.LogError(string.Concat(new object[] { "Fire Spread Error 1: ", __instance, " - ", __instance.UID }));
					}
				}

				if (__instance.timeLeft <= spreadTime && __instance.neverGoOut)
					__instance.timeLeft = lifetime;

				if (__instance.timeLeft <= spreadTime && __instance.oilFireTime > 0)
				{
					__instance.oilFireTime--;
					__instance.timeLeft = lifetime;
				}

				if (__instance.timeLeft <= 0f && !__instance.destroying)
				{
					try
					{
						__instance.DestroyMe();

						goto IL_460;
					}
					catch
					{
						Debug.LogError(string.Concat(new object[] { "Fire DestroyMe Error 2: ", __instance, " - ", __instance.UID }));

						if (__instance.particles != null)
						{
							__instance.particles.GetComponent<ParticleSystem>().Stop();
							__instance.particles.transform.SetParent(GC.particleEffectsNest.transform);
						}

						GC.firesList.Remove(__instance);
						Object.Destroy(__instance.gameObject);

						return false;
					}
				}

				if (__instance.timeToOilSpread > 0f || __instance.hasSpreadOil)
					goto IL_460;

				try
				{
					__instance.timeToOilSpread = __instance.timeToOilSpreadBase;
					__instance.FireSpreadOil();
					__instance.hasSpreadOil = true;

					goto IL_460;
				}
				catch
				{
					Debug.LogError(string.Concat(new object[] { "Fire Spread Error 2: ", __instance, " - ", __instance.UID }));

					if (__instance.particles != null)
					{
						__instance.particles.GetComponent<ParticleSystem>().Stop();
						__instance.particles.transform.SetParent(GC.particleEffectsNest.transform);
					}

					GC.firesList.Remove(__instance);
					Object.Destroy(__instance.gameObject);

					return false;
				}
			}

			if (!__instance.clientDestroyed && __instance.destroying)
			{
				try
				{
					__instance.clientDestroyed = true;
					__instance.DestroyMe();

					goto IL_460;
				}
				catch
				{
					Debug.LogError(string.Concat(new object[] { "Fire DestroyMe Error 3: ", __instance, " - ", __instance.UID }));

					goto IL_460;
				}
			}

			if (!GC.serverPlayer && !__instance.destroying && __instance.burningObject != null)
			{
				try
				{
					if (__instance.burningObjectAgent || __instance.burningObjectItem)
						__instance.tr.position = new Vector3(__instance.burningObject.transform.position.x,
							__instance.burningObject.transform.position.y - 0.1f, __instance.burningObject.transform.position.z);
					else
						__instance.tr.position = __instance.burningObject.transform.position;
				}
				catch
				{
					Debug.LogError(string.Concat(new object[] { "Fire Transform Error: ", __instance, " - ", __instance.burningObject }));
				}
			}

		IL_460:
			if (__instance.tr.eulerAngles != Vector3.zero)
				__instance.tr.eulerAngles = Vector3.zero;

			return false;
		}
	}
}
