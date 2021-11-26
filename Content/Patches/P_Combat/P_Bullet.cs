using BepInEx.Logging;
using BunnyMod.Content.Custom;
using BunnyMod.Content.Logging;
using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BunnyMod.Content.Patches
{
	[HarmonyPatch(declaringType: typeof(Bullet))]
	public static class P_Bullet
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		public static GameController GC => GameController.gameController;

		[HarmonyPostfix, HarmonyPatch(methodName: nameof(Bullet.SetupBullet))]
		public static void SetupBullet_Postfix(Bullet __instance)
		{
			if (__instance.bulletType == bulletStatus.Shotgun || __instance.bulletType == bulletStatus.Normal)
			{
				if (GC.challenges.Contains(cChallenge.ScaryGuns))
				{
					__instance.damage = Mathf.Max(1, (int)(__instance.damage * Random.Range(0.25f, 5f)));
					__instance.speed = Mathf.Min(65, __instance.speed * 3);
				}
				else if (__instance.agent.statusEffects.hasTrait(cTrait.Ballistician))
					__instance.speed = 40;
				// Lowest bad number: 40? Not sure, extreme range
				// Highest good number: 39
			}
		}

		[HarmonyPrefix, HarmonyPatch(methodName: nameof(Bullet.LateUpdateBullet))]
		public static bool LateUpdateBullet_Prefix(Bullet __instance, Transform ___bulletSpriteTr)
		{
			// Bullet Range mods
			// TODO: Transpiler method

			int logcounter = 0;

			logger.LogDebug("Bullet_LateUpdateBullet");
			logger.LogDebug("\tLog: " + logcounter++);

			if (Time.timeScale != 0f)
			{
				bool flag = true;
				Vector2 vector = __instance.tr.position;
				float maxBulletDistance;

				if (__instance.bulletType != bulletStatus.Fire &&
					__instance.bulletType != bulletStatus.Water &&
					__instance.bulletType != bulletStatus.Water2 &&
					__instance.bulletType != bulletStatus.LeafBlower &&
					__instance.bulletType != bulletStatus.ResearchGun &&
					__instance.bulletType != bulletStatus.FireExtinguisher)
					maxBulletDistance = C_Combat.BulletRange(__instance.agent);
				else
					maxBulletDistance = 13f;

				logger.LogDebug("\tLog: " + logcounter++);

				MethodInfo destroyMe_Base = AccessTools.DeclaredMethod(typeof(PlayfieldObject), "DestroyMe", new Type[0] { });

				if (__instance.agent != null && Vector2.Distance(__instance.agent.curPosition, vector) > maxBulletDistance)
					flag = false;

				logger.LogDebug("\tLog: " + logcounter++);

				if (!GC.splitScreen)
				{
					logger.LogDebug("\tLog A: " + logcounter++);

					if (!flag && Vector2.Distance(GC.cameraScript.containerTr.position, vector) > maxBulletDistance)
					{
						if (GC.activePooling)
							__instance.bulletSprite.SetSprite(__instance.bulletSprite.GetSpriteIdByName("Clear"));

						destroyMe_Base.GetMethodWithoutOverrides<Action>(__instance).Invoke();

						return false;
					}
				}
				else if (!flag)
				{
					logger.LogDebug("\tLog B: " + logcounter++);

					if (GC.coopMode)
					{
						if (Vector2.Distance(GC.cameraScriptS1.containerTr.position, vector) > maxBulletDistance &&
							Vector2.Distance(GC.cameraScriptS2.containerTr.position, vector) > maxBulletDistance)
						{
							if (GC.activePooling)
								__instance.bulletSprite.SetSprite(__instance.bulletSprite.GetSpriteIdByName("Clear"));

							destroyMe_Base.GetMethodWithoutOverrides<Action>(__instance).Invoke();

							return false;
						}
					}
					else if (GC.fourPlayerMode &&
						Vector2.Distance(GC.cameraScriptS1.containerTr.position, vector) > maxBulletDistance &&
						Vector2.Distance(GC.cameraScriptS2.containerTr.position, vector) > maxBulletDistance &&
						Vector2.Distance(GC.cameraScriptS3.containerTr.position, vector) > maxBulletDistance &&
						Vector2.Distance(GC.cameraScriptS4.containerTr.position, vector) > maxBulletDistance)
					{
						if (GC.activePooling)
							__instance.bulletSprite.SetSprite(__instance.bulletSprite.GetSpriteIdByName("Clear"));

						destroyMe_Base.GetMethodWithoutOverrides<Action>(__instance).Invoke();

						return false;
					}
				}

				logger.LogDebug("\tLog: " + logcounter++);

				Vector3 position = new Vector3(vector.x, vector.y, -2f + (vector.y - 1.28f) * 100f / 100000f);
				___bulletSpriteTr.position = position;
				__instance.timeSinceLaunch += Time.deltaTime;

				logger.LogDebug("\tLog: " + logcounter++);

				if (__instance.bulletType == bulletStatus.Fire ||
					__instance.bulletType == bulletStatus.Water ||
					__instance.bulletType == bulletStatus.Water2 ||
					__instance.bulletType == bulletStatus.LeafBlower ||
					__instance.bulletType == bulletStatus.ResearchGun ||
					__instance.bulletType == bulletStatus.FireExtinguisher)
				{
					logger.LogDebug("\tLog C: " + logcounter++);

					float num = 0.4f;
					float num2 = 0.35f;

					if (__instance.bulletType == bulletStatus.ResearchGun)
					{
						num = 0.2f;
						num2 = 0.15f;
					}
					else if (__instance.bulletType == bulletStatus.Water2)
					{
						num = 0.6f;
						num2 = 0.55f;
					}

					logger.LogDebug("\tLog: " + logcounter++);

					if (__instance.timeSinceLaunch >= num2 && !__instance.bulletFireStopped)
					{
						logger.LogDebug("\tLog D: " + logcounter++);

						__instance.bulletFireStopped = true;

						if (__instance.particles != null)
						{
							__instance.particles.GetComponent<ParticleSystem>().Stop();
							float num3 = __instance.myParticleScale;
							__instance.particles.transform.SetParent(GC.particleEffectsNest.transform);
							__instance.particles.transform.localScale = new Vector3(num3, num3, num3);

							return false;
						}
					}
					else if (__instance.timeSinceLaunch >= num)
					{
						logger.LogDebug("\tLogE: " + logcounter++);

						__instance.DestroyMe();

						return false;
					}
				}
				else if (__instance.bulletType == bulletStatus.GhostBlaster)
				{
					try
					{
						__instance.tr.position = __instance.agent.tr.position;
						__instance.agent.gun.visibleTime = 4f;

						if ((!__instance.agent.localPlayer || __instance.agent.outOfControl) && !__instance.agent.objectAgent)
						{
							if (__instance.agent.isPlayer != 0 && __instance.agent.outOfControl)
								__instance.movement.RotateToPositionOffsetTr(__instance.agent.opponent.tr.position);
							else if (__instance.agent.isPlayer != 0)
							{
								Vector3 vector2 = __instance.agent.gun.holdAttackAimPoint - __instance.tr.position;
								vector2.Normalize();
								float num4 = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f;
								Quaternion b = Quaternion.Euler(0f, 0f, num4 - 90f);
								__instance.tr.rotation = Quaternion.Slerp(__instance.tr.rotation, b, Time.deltaTime * 15f);
							}
							else if (__instance.agent.oma.mindControlled)
								__instance.movement.RotateToAngleTransform(__instance.agent.tr.eulerAngles.z - 90f);
							else if (GC.serverPlayer)
								__instance.movement.RotateToPositionOffsetTr(__instance.agent.opponent.tr.position);
							else
								__instance.movement.RotateToPositionOffsetTr(__instance.agent.melee.attackObject.tr.position);
						}
						else if (__instance.agent.controllerType == "Keyboard")
						{
							if (GC.sessionDataBig.trackpadMode)
								__instance.movement.RotateToAngleTransform(__instance.agent.tr.eulerAngles.z - 90f);
							else
								__instance.movement.RotateToMouseTr(__instance.agent.agentCamera.actualCamera);
						}
						else if (__instance.agent.target.AttackTowardTarget())
							__instance.tr.rotation = Quaternion.Euler(0f, 0f, __instance.agent.target.transform.eulerAngles.z);
						else
							__instance.tr.rotation = Quaternion.Euler(0f, 0f, __instance.agent.gun.FindWeaponAngleGamepad() - 90f);

						if (__instance.FindStoppingPoint() != Vector2.zero)
						{
							__instance.movement.MoveForwardTransform(__instance.stoppingPointDistance / 2f);
							__instance.tr.localScale = new Vector3(__instance.tr.localScale.x, __instance.stoppingPointDistance / 2f,
								__instance.tr.localScale.z);
							__instance.particles.GetComponent<ParticleSystem>().emissionRate = __instance.stoppingPointDistance * 150f;
							ParticleSystem.ShapeModule a = __instance.particles.GetComponent<ParticleSystem>().shape;
							a.length = __instance.stoppingPointDistance - 0.32f;
						}
						else
						{
							if ((!__instance.agent.localPlayer || __instance.agent.outOfControl) && !__instance.agent.objectAgent)
							{
								if (__instance.agent.isPlayer != 0 && __instance.agent.outOfControl)
									__instance.movement.RotateToPositionOffsetTr(__instance.agent.opponent.tr.position);
								else if (__instance.agent.isPlayer != 0)
								{
									Vector3 vector3 = __instance.agent.gun.holdAttackAimPoint - __instance.tr.position;
									vector3.Normalize();
									float num5 = Mathf.Atan2(vector3.y, vector3.x) * 57.29578f;
									Quaternion b2 = Quaternion.Euler(0f, 0f, num5 - 90f);
									__instance.tr.rotation = Quaternion.Slerp(__instance.tr.rotation, b2, Time.deltaTime * 15f);
								}
								else if (__instance.agent.oma.mindControlled)
									__instance.movement.RotateToAngleTransform(__instance.agent.tr.eulerAngles.z - 90f);
								else if (GC.serverPlayer)
									__instance.movement.RotateToPositionOffsetTr(__instance.agent.opponent.tr.position);
								else
									__instance.movement.RotateToPositionOffsetTr(__instance.agent.melee.attackObject.tr.position);
							}
							else if (__instance.agent.controllerType == "Keyboard")
							{
								if (GC.sessionDataBig.trackpadMode)
									__instance.movement.RotateToAngleTransform(__instance.agent.tr.eulerAngles.z - 90f);
								else
									__instance.movement.RotateToMouseTr(__instance.agent.agentCamera.actualCamera);
							}
							else if (__instance.agent.target.AttackTowardTarget())
								__instance.tr.rotation = Quaternion.Euler(0f, 0f, __instance.agent.target.transform.eulerAngles.z);
							else
								__instance.tr.rotation = Quaternion.Euler(0f, 0f, __instance.agent.gun.FindWeaponAngleGamepad() - 90f);

							__instance.movement.MoveForwardTransform(6.72f);
							__instance.tr.localScale = new Vector3(__instance.tr.localScale.x, 6.72f, __instance.tr.localScale.z);
							__instance.particles.GetComponent<ParticleSystem>().emissionRate = 2015.9999f;
							ParticleSystem.ShapeModule a = __instance.particles.GetComponent<ParticleSystem>().shape;
							a.length = 13.12f;
						}

						__instance.particles.transform.position = __instance.agent.tr.position;
						__instance.particles.transform.Translate(Vector3.forward * 0.32f, Space.Self);
						bool flag2 = false;
						__instance.timeToDamage += Time.deltaTime;

						if (__instance.timeToDamage >= 0.05f)
						{
							flag2 = true;
							__instance.timeToDamage = 0f;
						}

						if (GC.multiplayerMode && __instance.agent.isPlayer != 0)
						{
							__instance.timeToSendAimPoint += Time.deltaTime;

							if (__instance.timeToSendAimPoint >= 0.1f && (__instance.agent.isPlayer == 0 || __instance.agent.localPlayer) &&
								(GC.serverPlayer || __instance.agent.isPlayer != 0))
							{
								Vector3 v = Vector3.zero;

								if (__instance.agent.controllerType == "Keyboard")
								{
									if (GC.sessionDataBig.trackpadMode)
									{
										__instance.movement.RotateToAngleTransform(__instance.agent.tr.eulerAngles.z - 90f);
										__instance.dirHelper.localPosition = new Vector3(0f, 0.32f, 0f);
										v = __instance.dirHelper.position;
									}
									else
										v = __instance.agent.agentCamera.actualCamera.ScreenCamera.ScreenToWorldPoint(Input.mousePosition);
								}
								else if (__instance.agent.target.AttackTowardTarget())
									v = __instance.agent.target.tr.position;
								else
								{
									__instance.tr.rotation = Quaternion.Euler(0f, 0f, __instance.agent.gun.FindWeaponAngleGamepad() - 90f);
									__instance.dirHelper.localPosition = new Vector3(0f, 0.32f, 0f);
									v = __instance.dirHelper.position;
								}

								__instance.agent.objectMult.SendHoldAttackAimPoint(v);
								__instance.timeToSendAimPoint = 0f;
							}
						}

						if (__instance.stoppingPointObject != null && flag2)
							__instance.RayHit(__instance.stoppingPointObject);

						if (__instance.timeSinceLaunch >= 0.1f && (__instance.agent.isPlayer == 0 || __instance.agent.localPlayer) &&
							(GC.serverPlayer || __instance.agent.isPlayer != 0))
						{
							__instance.bulletFireStopped = true;
							__instance.agent.objectMult.StopHoldAttack();
							__instance.DestroyMe();
						}

						if (__instance.agent.gun.stopHoldAttack)
						{
							GC.audioHandler.Fade(__instance.agent, "GhostGibberFire");
							GC.audioHandler.Fade(__instance.agent, "GhostGibberLoop");
							__instance.agent.gun.stopHoldAttack = false;
							__instance.bulletFireStopped = true;
							__instance.timeSinceLaunch = 0f;
							__instance.timeToSendAimPoint = 0f;
							__instance.DestroyMe();
						}
					}
					catch
					{
						Debug.LogError("Ghost Gibber Error");
						__instance.DestroyMe();
					}
				}
			}

			return false;
		}
	}
}
