using HarmonyLib;
using RogueLibsCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BunnyMod.Content
{
	public class BMCombat
	{
		public static GameController GC => GameController.gameController;
		public static bool Prefix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) => BMHeader.MainInstance.PatchPrefix(type, methodName, patchType, patchMethodName, types);
		public static bool Postfix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) => BMHeader.MainInstance.PatchPostfix(type, methodName, patchType, patchMethodName, types);
		public static void BMLog(string logMessage) => BMHeader.Log(logMessage);

		public void Awake()
		{
			Initialize_Names();

			Bullet_00();
			BulletHitbox_00();
			MeleeHitbox_00();
			Movement_00();
			PlayfieldObject_00();
			SpawnerMain_00();
		}
		public void Initialize_Names()
		{
			string t = vNameType.StatusEffect;
			_ = RogueLibs.CreateCustomName("Headshot", t, new CustomNameInfo("Headshot"));
		}

		#region Custom
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
				maxBulletRange = 100f;
			if (agent.statusEffects.hasTrait(cTrait.Sniper))
				maxBulletRange = 20f;

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
		#endregion

		#region Bullet
		public void Bullet_00()
		{
			Type t = typeof(Bullet);
			Type g = GetType();

			Postfix(t, "SetupBullet", g, "Bullet_SetupBullet", new Type[0] { });
			Prefix(t, "LateUpdateBullet", g, "Bullet_LateUpdateBullet", new Type[0] { });
		}
		public static void Bullet_SetupBullet(Bullet __instance) // Postfix
		{
			if (__instance.bulletType == bulletStatus.Shotgun || __instance.bulletType == bulletStatus.Normal)
			{
				if (GC.challenges.Contains(cChallenge.ScaryGuns))
				{
					__instance.damage = Mathf.Max(1, (int)(__instance.damage * Random.Range(0.25f, 5f)));
					__instance.speed = Mathf.Min(65, __instance.speed * 3);
				}
				else if (__instance.agent.statusEffects.hasTrait(cTrait.Ballistician))
					__instance.speed = 39;
				// Lowest bad number: 40? Not sure, extreme range
				// Highest good number: 37
			}
		}
		public static bool Bullet_LateUpdateBullet(Bullet __instance, Transform ___bulletSpriteTr) // Replacement
		{
			// Bullet Range mods

			if (Time.timeScale != 0f)
			{
				bool flag = true;
				Vector2 vector = __instance.tr.position;
				float maxBulletDistance;

				try { maxBulletDistance = GetBulletRange(__instance.agent); }
				catch { maxBulletDistance = 13f; }

				MethodInfo destroyMe_Base = AccessTools.DeclaredMethod(typeof(PlayfieldObject), "DestroyMe", new Type[0] { });

				if (__instance.agent != null && Vector2.Distance(__instance.agent.curPosition, vector) > maxBulletDistance)
					flag = false;
				
				if (!GC.splitScreen)
				{
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

				Vector3 position = new Vector3(vector.x, vector.y, -2f + (vector.y - 1.28f) * 100f / 100000f);
				___bulletSpriteTr.position = position;
				__instance.timeSinceLaunch += Time.deltaTime;

				if (__instance.bulletType == bulletStatus.Fire || 
					__instance.bulletType == bulletStatus.Water || 
					__instance.bulletType == bulletStatus.Water2 || 
					__instance.bulletType == bulletStatus.LeafBlower || 
					__instance.bulletType == bulletStatus.ResearchGun || 
					__instance.bulletType == bulletStatus.FireExtinguisher)
				{
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

					if (__instance.timeSinceLaunch >= num2 && !__instance.bulletFireStopped)
					{
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
							__instance.tr.localScale = new Vector3(__instance.tr.localScale.x, __instance.stoppingPointDistance / 2f, __instance.tr.localScale.z);
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
						
							if (__instance.timeToSendAimPoint >= 0.1f && (__instance.agent.isPlayer == 0 || __instance.agent.localPlayer) && (GC.serverPlayer || __instance.agent.isPlayer != 0))
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
						
						if (__instance.timeSinceLaunch >= 0.1f && (__instance.agent.isPlayer == 0 || __instance.agent.localPlayer) && (GC.serverPlayer || __instance.agent.isPlayer != 0))
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
		#endregion
		#region BulletHitbox
		public void BulletHitbox_00()
		{
			Type t = typeof(BulletHitbox);
			Type g = GetType();

			Prefix(t, "HasLOSBullet", g, "BulletHitbox_HasLOSBullet", new Type[1] { typeof(PlayfieldObject) });
			Prefix(t, "OnTriggerEnter2D", g, "BulletHitbox_OnTriggerEnter2D", new Type[1] { typeof(Collider2D) });
		}
		public static bool BulletHitbox_HasLOSBullet(PlayfieldObject playfieldObject, BulletHitbox __instance, ref bool __result, ref RaycastHit2D[] ___hitsAlloc) // Prefix
		{
			// Ballistician
			// Sniper

			if (__instance.myBullet.agent != null && BMTraits.DoesPlayerHaveTraitFromList(__instance.myBullet.agent, cTrait.BulletRange))
			{
				float maxBulletRange = GetBulletRange(__instance.myBullet.agent);

				__instance.myBullet.dirHelper.localPosition = new Vector3(0f, -0.16f, 0f);
				Vector3 position = __instance.myBullet.dirHelper.transform.position;
				Vector2 vector = playfieldObject.transform.position - position;
				int hitsInPath = Physics2D.RaycastNonAlloc(position, vector.normalized, ___hitsAlloc, maxBulletRange, __instance.myLayerMask);
				float closestHit = 1000000f;
				
				for (int i = 0; i < hitsInPath; i++)
				{
					RaycastHit2D raycastHit2D = ___hitsAlloc[i];

					if (raycastHit2D.collider.CompareTag("Wall") && Vector2.Distance(position, raycastHit2D.collider.transform.position) < closestHit)
						closestHit = Vector2.Distance(position, raycastHit2D.collider.transform.position);
				}

				__instance.myBullet.dirHelper.localPosition = new Vector3(0f, -0.32f, 0f);
				
				__result = Vector2.Distance(playfieldObject.tr.position, position) < closestHit;

				return false;
			}

			return true;
		}
		public static bool BulletHitbox_OnTriggerEnter2D(Collider2D other, BulletHitbox __instance) // Prefix
		{
			// GhillieSuit

			BMLog("BulletHitbox_OnTriggerEnter2D");
			try { BMLog("\tname: " + other.name); } catch { }

			Agent agent = __instance.myBullet.agent;

			if (other.CompareTag("ObjectRealSprite") && agent.statusEffects.hasTrait(cTrait.GhillieSuit))
			{
				BMLog("\tObject detected");
				ObjectReal obj = other.GetComponent<ObjectReal>();

				if (agent.hiddenInObject != null && agent.hiddenInObject == obj)
				{
					BMLog("\tObject bypassed");
					return false;
				}
			}

			return true;
		}
		#endregion
		#region MeleeHitbox
		public void MeleeHitbox_00()
		{
			Type t = typeof(MeleeHitbox);
			Type g = GetType();

			Prefix(t, "HitObject", g, "MeleeHitbox_HitObject", new Type[2] { typeof(GameObject), typeof(bool) });
			Prefix(t, "MeleeHitEffect", g, "MeleeHitbox_MeleeHitEffect", new Type[1] { typeof(GameObject) });
		}
		public static bool MeleeHitbox_HitObject(GameObject hitObject, bool fromClient, MeleeHitbox __instance) // Replacement
		{
			// Spectral Strikes

			InvItem invItem = null;
			Agent hittingAgent = __instance.myMelee.agent;

			try
			{
				invItem = hittingAgent.inventory.equippedWeapon;

				if (hittingAgent.inventory.equippedWeapon.itemType == "WeaponProjectile")
					invItem = hittingAgent.inventory.fist;
			}
			catch { }

			if ((!__instance.ObjectListContains(hitObject) && __instance.myMelee.canDamage && __instance.canHitMore) || fromClient)
			{
				if (GC.multiplayerMode && GC.serverPlayer && !hittingAgent.localPlayer && !fromClient && hittingAgent.isPlayer != 0)
				{
					bool flag = true;

					if (hitObject.CompareTag("ObjectRealSprite"))
					{
						ObjectReal objectReal;
					
						if (hitObject.name.Contains("ExtraSprite"))
							objectReal = hitObject.transform.parent.transform.parent.GetComponent<ObjectReal>();
						else
							objectReal = hitObject.GetComponent<ObjectSprite>().objectReal;
						
						if (objectReal.objectName == "Window")
						{
							Window window = (Window)objectReal;
						
							if (window.FindDamage(__instance.myMelee, false, true, fromClient) >= 30)
								__instance.justHitWindow = true;
							else
							{
								if (window.hitWindowOnce)
									return false;
							
								__instance.justHitWindow = true;
							}
						}

						if (!objectReal.objectSprite.meshRenderer.enabled || !objectReal.notRealObject || !objectReal.OnFloor || !objectReal.meleeCanPass || !objectReal.tempNoMeleeHits)
							return false;
					}

					if (!hitObject.CompareTag("ObjectRealSprite") && !hitObject.CompareTag("AgentSprite") && !hitObject.CompareTag("ItemImage") && !hitObject.CompareTag("Wall"))
						flag = false;
					else if (hitObject.CompareTag("AgentSprite"))
					{
						Agent agent = hitObject.GetComponent<ObjectSprite>().agent;
					
						if (agent == hittingAgent)
							flag = false;
						
						if (agent.localPlayer && hittingAgent.isPlayer > 0 && !hittingAgent.localPlayer)
							flag = false;
					}

					if (flag)
					{
						__instance.FakeHit(hitObject);
					
						return false;
					}
				}

				#region Sorted by target type
				if (hitObject.CompareTag("MeleeHitbox"))
				{
					if (hittingAgent.statusEffects.hasTrait("BlocksSometimesHit2"))
					{
						if (GC.percentChance(60))
							hitObject = hitObject.GetComponent<MeleeColliderBox>().meleeHitbox.myMelee.agent.agentHitboxScript.go;
					}
					else if (hittingAgent.statusEffects.hasTrait("BlocksSometimesHit"))
					{
						if (GC.percentChance(30))
							hitObject = hitObject.GetComponent<MeleeColliderBox>().meleeHitbox.myMelee.agent.agentHitboxScript.go;
					}
					else
					{
						Agent agent2 = hitObject.GetComponent<MeleeColliderBox>().meleeHitbox.myMelee.agent;
					
						if (agent2.statusEffects.hasTrait("BlocksSometimesHit2"))
						{
							if (GC.percentChance(60))
								hitObject = hittingAgent.agentHitboxScript.go;
						}
						else if (agent2.statusEffects.hasTrait("BlocksSometimesHit") && GC.percentChance(30))
							hitObject = hittingAgent.agentHitboxScript.go;
					}
				}

				if (hitObject.CompareTag("ObjectRealSprite") && !__instance.myMelee.invItem.meleeNoHit)
				{
					ObjectReal objectReal2;
				
					if (hitObject.name.Contains("ExtraSprite"))
						objectReal2 = hitObject.transform.parent.transform.parent.GetComponent<ObjectReal>();
					else
						objectReal2 = hitObject.GetComponent<ObjectSprite>().objectReal;
					
					if (objectReal2.extraSprites.Count > 0)
						for (int i = 0; i < objectReal2.extraSprites.Count; i++)
							__instance.objectList.Add(objectReal2.extraSprites[i].gameObject);
					else
						__instance.objectList.Add(hitObject);
					
					if (!GC.serverPlayer && !hittingAgent.localPlayer && hittingAgent.mindControlAgent != GC.playerAgent && objectReal2.objectSprite.meshRenderer.enabled && !objectReal2.notRealObject && !objectReal2.OnFloor && !objectReal2.meleeCanPass && !objectReal2.tempNoMeleeHits)
					{
						__instance.FakeHit(hitObject);
					
						return false;
					}

					if (__instance.HasLOSMelee(objectReal2) && objectReal2.objectMeshRenderer.enabled)
					{
						bool flag2 = false;
					
						if (hittingAgent.statusEffects.hasTrait("HitObjectsNoNoise"))
							flag2 = true;
						
						if (objectReal2.objectName == "Window")
						{
							if (objectReal2.FindDamage(__instance.myMelee, false, true, fromClient) >= 30)
								__instance.justHitWindow = true;
						
							if (flag2)
							{
								Window component = objectReal2.GetComponent<Window>();
								component.StartCoroutine(component.TempNoNoise());
							}
						}

						if (!objectReal2.notRealObject && !objectReal2.OnFloor && (!objectReal2.meleeCanPass || __instance.justHitWindow) && !objectReal2.tempNoMeleeHits)
						{
							objectReal2.Damage(__instance.myMelee, fromClient);
						
							if (!objectReal2.noDamageNoise && !__instance.myMelee.successfullySleepKilled && !flag2)
								GC.spawnerMain.SpawnNoise(objectReal2.FindDamageNoisePos(objectReal2.tr.position), (float)objectReal2.noiseHitVol, null, null, hittingAgent);
							
							if (hittingAgent != null && GC.serverPlayer)
								GC.OwnCheck(hittingAgent, hitObject, "Normal", 1);
							
							if (__instance.justHitWindow)
								__instance.justHitWindow = false;
							
							if (__instance.myMelee.hitParticlesTr != null && __instance.myMelee.meleeContainerTr != null)
								GC.spawnerMain.SpawnParticleEffect("ObjectDestroyed", __instance.myMelee.hitParticlesTr.position, __instance.myMelee.meleeContainerTr.eulerAngles.z - 90f);
							
							if (objectReal2.destroying || objectReal2.justDamaged)
							{
								if (hittingAgent.isPlayer > 0 && hittingAgent.localPlayer && !objectReal2.noDestroyEffects)
								{
									GC.ScreenShake(0.2f, 80f, hittingAgent.tr.position, hittingAgent);
									GC.FreezeFrames(1);
								}
							}
							else if (hittingAgent.isPlayer > 0 && hittingAgent.localPlayer)
							{
								GC.ScreenShake(0.1f, 80f, hittingAgent.tr.position, hittingAgent);
								GC.FreezeFrames(0);
							}
							
							GC.alienFX.HitObject(hittingAgent);
							__instance.MeleeHitEffect(hitObject);
							GC.playerControl.Vibrate(hittingAgent.isPlayer, Mathf.Clamp((float)objectReal2.damagedAmount / 100f + 0.05f, 0f, 0.25f), Mathf.Clamp((float)objectReal2.damagedAmount / 132f + 0.05f, 0f, 0.2f));
							
							if (!GC.serverPlayer && (hittingAgent.isPlayer > 0 || hittingAgent.mindControlAgent == GC.playerAgent))
							{
								if (hittingAgent.isPlayer != 0)
								{
									hittingAgent.objectMult.CallCmdMeleeHitObjectReal(objectReal2.objectNetID);
							
									return false;
								}

								GC.playerAgent.objectMult.CallCmdMeleeHitObjectRealNPC(hittingAgent.objectNetID, objectReal2.objectNetID);
								
								return false;
							}
							else if (GC.serverPlayer && GC.multiplayerMode)
							{
								hittingAgent.objectMult.CallRpcMeleeHitObjectFake(objectReal2.objectNetID);
								
								return false;
							}
						}
					}
				}
				else if (hitObject.CompareTag("AgentSprite"))
				{
					__instance.objectList.Add(hitObject);
					Agent hitAgent = hitObject.GetComponent<ObjectSprite>().agent;
					
					if (GC.multiplayerMode)
						if ((GC.serverPlayer && hittingAgent.localPlayer && hitAgent.isPlayer > 0 && !hitAgent.localPlayer) ||
							(GC.serverPlayer && hittingAgent.isPlayer == 0 && hitAgent.isPlayer > 0 && !hitAgent.localPlayer) ||
							(GC.multiplayerMode && GC.serverPlayer && hittingAgent.isPlayer == 0 && hitAgent.isPlayer == 0 && hittingAgent.mindControlAgent != null && hittingAgent.mindControlAgent != GC.playerAgent && !hitAgent.dead) ||
							(GC.multiplayerMode && GC.serverPlayer && hittingAgent.isPlayer == 0 && hitAgent.isPlayer == 0 && hitAgent.mindControlAgent != null && hitAgent.mindControlAgent != GC.playerAgent && !hitAgent.dead) ||
							(!GC.serverPlayer && hittingAgent.isPlayer == 0 && !hitAgent.localPlayer && hittingAgent != hitAgent && ((hittingAgent.mindControlAgent != GC.playerAgent && hitAgent.mindControlAgent != GC.playerAgent) || hitAgent.dead)) ||
							(!GC.serverPlayer && hittingAgent.isPlayer > 0 && !hittingAgent.localPlayer && !hitAgent.localPlayer) ||
							(!GC.serverPlayer && (hittingAgent.localPlayer || hittingAgent.mindControlAgent == GC.playerAgent) && hitAgent.isPlayer > 0 && !hitAgent.localPlayer) ||
							(!GC.serverPlayer && hittingAgent.isPlayer != 0 && !hittingAgent.localPlayer && hitAgent.isPlayer != 0 && !hitAgent.localPlayer))
						{
							__instance.FakeHit(hitObject);

							return false;
						}

					if (hittingAgent != hitAgent && 
						(!hitAgent.ghost || CanAgentMeleeHitGhost(hittingAgent)) &&
						!hitAgent.fellInHole && !GC.cinematic && __instance.HasLOSMelee(hitAgent))
					{
						__instance.objectList.Add(hitAgent.melee.meleeHitbox.gameObject);

						if (__instance.myMelee.invItem.meleeNoHit && !hitAgent.dead)
						{
							Relationship relationship = hitAgent.relationships.GetRelationship(hittingAgent);

							if (!hitAgent.movement.HasLOSObjectBehind(hittingAgent) || hitAgent.sleeping || hittingAgent.isPlayer == 0 || hittingAgent.invisible || (__instance.myMelee.invItem.invItemName == "StealingGlove" && hittingAgent.oma.superSpecialAbility))
							{
								__instance.canHitMore = false;

								if (__instance.myMelee.invItem.invItemName == "ChloroformHankie")
								{
									if (!fromClient)
										__instance.HitAftermath(hitObject, fromClient, hitAgent, __instance.myMelee.invItem.invItemName);
								}
								else if (__instance.myMelee.invItem.invItemName == "StealingGlove")
								{
									if (hittingAgent.oma.superSpecialAbility)
										__instance.HitAftermath(hitObject, fromClient, hitAgent, __instance.myMelee.invItem.invItemName);
									else if (hittingAgent.movement.HasLOSObject(hitAgent, "360", false, true))
									{
										hitAgent.agentHelperTr.localPosition = new Vector3(-0.64f, 0f, 0f);
								
										if ((!GC.tileInfo.IsOverlapping(hitAgent.agentHelperTr.position, "Wall") || hitAgent.sleeping) && !fromClient)
											__instance.HitAftermath(hitObject, fromClient, hitAgent, __instance.myMelee.invItem.invItemName);
									}
								}
							}
							else
							{
								if (__instance.myMelee.invItem.invItemName == "StealingGlove" && relationship.relType != "Aligned" && relationship.relType != "Submissive" && hittingAgent.movement.HasLOSObject(hitAgent, "360", false, true) && !fromClient)
									__instance.HitAftermath(hitObject, fromClient, hitAgent, "StealingGloveFail");
							
								if (GC.serverPlayer)
									GC.spawnerMain.SpawnNoise(hittingAgent.tr.position, 0f, null, null, hittingAgent);
							}
						
							if ((__instance.myMelee.invItem.invItemName == "ChloroformHankie" || __instance.myMelee.invItem.invItemName == "Handcuffs") && relationship.relType != "Aligned" && relationship.relType != "Submissive" && !fromClient)
								__instance.HitAftermath(hitObject, fromClient, hitAgent, "ChloroformHankieFail");
						}

						bool flag3 = !__instance.myMelee.invItem.meleeNoHit && hittingAgent.DontHitAlignedCheck(hitAgent);
						
						if (flag3)
						{
							hitAgent.melee.meleeHitbox.objectList.Add(__instance.gameObject);
							hitAgent.melee.meleeHitbox.objectList.Add(hittingAgent.sprTr.gameObject);
						
							if (hittingAgent.zombified && hitAgent.isPlayer == 0 && !hitAgent.oma.bodyGuarded)
								hitAgent.zombieWhenDead = true;
							
							if (hitAgent.isPlayer == 0 && hittingAgent.isPlayer != 0 && !hitAgent.dead && hitAgent.agentName != "Zombie" && !hitAgent.inhuman && !hitAgent.mechEmpty && !hitAgent.mechFilled && hittingAgent.localPlayer && !hitAgent.statusEffects.hasStatusEffect("Invincible"))
							{
								if (hittingAgent.statusEffects.hasTrait("FleshFeast2"))
									hittingAgent.statusEffects.ChangeHealth(6f);
								else if (hittingAgent.statusEffects.hasTrait("FleshFeast"))
									hittingAgent.statusEffects.ChangeHealth(3f);
							}
							
							if (GC.serverPlayer || hitAgent.health > 0f || hitAgent.dead)
								hitAgent.Damage(__instance.myMelee, fromClient);
							
							hittingAgent.relationships.FollowerAlert(hitAgent);

							if (hitAgent.statusEffects.hasTrait("AttacksDamageAttacker2") &&
								(!hittingAgent.ghost || CanAgentMeleeHitGhost(hittingAgent)))
							{
								int myChance = hitAgent.DetermineLuck(20, "AttacksDamageAttacker", true);
							
								if (GC.percentChance(myChance))
								{
									hittingAgent.lastHitByAgent = hitAgent;
									hittingAgent.justHitByAgent2 = hitAgent;
									hittingAgent.lastHitByAgent = hitAgent;
									hittingAgent.deathMethod = "AttacksDamageAttacker";
									hittingAgent.deathKiller = hitAgent.agentName;
									hittingAgent.statusEffects.ChangeHealth(-10f);
								}
							}
							else if (hitAgent.statusEffects.hasTrait("AttacksDamageAttacker") &&
								(!hittingAgent.ghost || CanAgentMeleeHitGhost(hittingAgent)))
							{
								int myChance2 = hitAgent.DetermineLuck(20, "AttacksDamageAttacker", true);
								
								if (GC.percentChance(myChance2))
								{
									hittingAgent.lastHitByAgent = hitAgent;
									hittingAgent.justHitByAgent2 = hitAgent;
									hittingAgent.lastHitByAgent = hitAgent;
									hittingAgent.deathMethod = "AttacksDamageAttacker";
									hittingAgent.deathKiller = hitAgent.agentName;
									hittingAgent.statusEffects.ChangeHealth(-5f);
								}
							}

							if (hitAgent.justDied && hittingAgent.isPlayer > 0 && !GC.coopMode && !GC.fourPlayerMode && !GC.multiplayerMode && GC.sessionDataBig.slowMotionCinematic && GC.percentChance(25))
							{
								if (GC.challenges.Contains("LowHealth"))
								{
									if (GC.percentChance(50))
										GC.StartCoroutine(GC.SetSecondaryTimeScale(0.1f, 0.13f));
								}
								else
									GC.StartCoroutine(GC.SetSecondaryTimeScale(0.1f, 0.13f));
							}

							float num = 0f;
							
							if (__instance.myMelee.successfullySleepKilled || __instance.myMelee.successfullyBackstabbed)
								num = 0f;
							else if ((!hitAgent.dead || hitAgent.justDied) && !hitAgent.disappeared)
								num = (float)Mathf.Clamp(hitAgent.damagedAmount * 20, 80, 9999);
							else if (!hitAgent.disappeared)
								num = 80f;
							
							if (hittingAgent.statusEffects.hasTrait("CauseBiggerKnockback"))
								num *= 2f;
							
							Vector3 position = hitAgent.tr.position;
							Vector2 velocity = hitAgent.rb.velocity;
							
							if (!hitAgent.disappeared && !fromClient)
								hitAgent.movement.KnockBackBullet(__instance.myMelee.meleeContainerTr.gameObject, num, true, hittingAgent);
							
							bool flag4 = false;
							
							if (hitAgent.hasEmployer && hitAgent.employer.statusEffects.hasSpecialAbility("ProtectiveShell") && hitAgent.employer.objectMult.chargingSpecialLunge)
								flag4 = true;
							
							if (hitAgent.statusEffects.hasSpecialAbility("ProtectiveShell") && hitAgent.objectMult.chargingSpecialLunge)
								flag4 = true;
							
							if (flag4)
							{
								bool flag5 = true;
							
								if (GC.multiplayerMode && GC.serverPlayer)
								{
									if (hitAgent.isPlayer != 0 && !hitAgent.localPlayer && hittingAgent.isPlayer == 0)
										flag5 = false;
								
									if (hittingAgent.isPlayer != 0 && !hittingAgent.localPlayer && hitAgent.isPlayer == 0)
										flag5 = false;
								}

								if (flag5)
								{
									hittingAgent.movement.KnockBackBullet(hitAgent.gameObject, 240f, true, hitAgent);
								
									if (GC.serverPlayer && hittingAgent.isPlayer == 0 && invItem.invItemName != "Fist" && !hitAgent.warZoneAgent)
									{
										int myChance3 = hitAgent.DetermineLuck(15, "ChanceToKnockWeapons", true);
									
										if (GC.percentChance(myChance3))
										{
											InvItem invItem2 = hittingAgent.inventory.FindItem(invItem.invItemName);
											hittingAgent.inventory.DestroyItem(invItem2);
											GC.spawnerMain.SpillItem(__instance.tr.position, invItem2);
											GC.spawnerMain.SpawnStatusText(hittingAgent, "OutOfAmmo", invItem2.invItemName, "Item");
										
											if (!GC.serverPlayer && (hittingAgent.isPlayer != 0 || hittingAgent.mindControlAgent == GC.playerAgent))
												hittingAgent.objectMultPlayfield.SpawnStatusText("OutOfAmmo", invItem2.invItemName, "Item", hittingAgent.objectNetID, "", "");
											
											hittingAgent.statusEffects.CreateBuffText("DroppedWeapon", hittingAgent.objectNetID);
											hittingAgent.dontPickUpWeapons = true;
										}
									}
								}
							}

							if (!GC.serverPlayer && (hittingAgent.localPlayer || hittingAgent.mindControlAgent == GC.playerAgent))
							{
								hittingAgent.objectMultPlayfield.TempDisableNetworkTransform(hitAgent);
								Quaternion localRotation = __instance.myMelee.meleeHelperTr.localRotation;
								__instance.myMelee.meleeHelperTr.rotation = __instance.myMelee.meleeContainerTr.rotation;
								__instance.myMelee.meleeHelperTr.position = __instance.myMelee.meleeContainerTr.position;
								__instance.myMelee.meleeHelperTr.localPosition = new Vector3(__instance.myMelee.meleeHelperTr.localPosition.x, __instance.myMelee.meleeHelperTr.localPosition.y + 10f, __instance.myMelee.meleeHelperTr.localPosition.z);
								Vector3 position2 = __instance.myMelee.meleeHelperTr.position;
								__instance.myMelee.meleeHelperTr.localPosition = Vector3.zero;
								__instance.myMelee.meleeHelperTr.localRotation = localRotation;

								if (!hittingAgent.testingNewClientLerps)
								{
									if (hittingAgent.isPlayer != 0)
										hittingAgent.objectMult.CallCmdMeleeHitAgent(hitAgent.objectNetID, position2, (int)num, position, hitAgent.rb.velocity);
									else
										GC.playerAgent.objectMult.CallCmdMeleeHitAgentNPC(hittingAgent.objectNetID, hitAgent.objectNetID, position2, (int)num, position, hitAgent.rb.velocity);
								}
							}
							else if (GC.multiplayerMode && GC.serverPlayer)
								hittingAgent.objectMult.CallRpcMeleeHitObjectFake(hitAgent.objectNetID);
							
							if ((hittingAgent.isPlayer > 0 && hittingAgent.localPlayer) || (hitAgent.isPlayer > 0 && hitAgent.localPlayer))
							{
								if (hitAgent.justDied)
									GC.ScreenShake(0.25f, (float)Mathf.Clamp(15 * hitAgent.damagedAmount, 160, 500), Vector2.zero, hittingAgent);
								else
									GC.ScreenShake(0.2f, (float)Mathf.Clamp(15 * hitAgent.damagedAmount, 0, 500), Vector2.zero, hittingAgent);
							}
							
							GC.alienFX.PlayerHitEnemy(hittingAgent);
							hittingAgent.combat.meleeJustHitCooldown = hittingAgent.combat.meleeJustHitTimeStart;
							hittingAgent.combat.meleeJustHitCloseCooldown = hittingAgent.combat.meleeJustHitCloseTimeStart;
							
							if (GC.serverPlayer)
							{
								if (__instance.myMelee.successfullyBackstabbed)
									GC.spawnerMain.SpawnNoise(__instance.tr.position, 0.7f, null, null, hittingAgent);
								else if (!__instance.myMelee.successfullySleepKilled)
									GC.spawnerMain.SpawnNoise(__instance.tr.position, 1f, null, null, hittingAgent);
							}

							__instance.MeleeHitEffect(hitObject);
							GC.playerControl.Vibrate(hittingAgent.isPlayer, Mathf.Clamp((float)hitAgent.damagedAmount / 100f + 0.05f, 0f, 0.25f), Mathf.Clamp((float)hitAgent.damagedAmount / 132f + 0.05f, 0f, 0.2f));
							
							if (GC.levelType == "Tutorial")
							{
								GC.tutorial.MeleeTarget(hitAgent);
							
								return false;
							}
						}
					}
				}
				else if (hitObject.CompareTag("ItemImage"))
				{
					__instance.objectList.Add(hitObject);
					Item component2 = hitObject.transform.parent.GetComponent<Item>();

					if (!GC.serverPlayer && !hittingAgent.localPlayer && hittingAgent.mindControlAgent != GC.playerAgent)
					{
						__instance.FakeHit(hitObject);
					
						return false;
					}

					if (!component2.justSpilled && !__instance.myMelee.invItem.meleeNoHit && component2.itemObject == null && __instance.HasLOSMelee(component2))
					{
						Vector3 position3 = component2.tr.position;
					
						if (!fromClient)
							component2.movement.KnockBackBullet(__instance.myMelee.meleeContainerTr.gameObject, 250f, true, hittingAgent);
						
						component2.Damage(__instance.myMelee, fromClient);
						
						if (component2.invItem.reactOnTouch)
							component2.TouchMe(hittingAgent, "MeleeHitbox");
						
						component2.thrower = hittingAgent;
						
						if (component2.go.activeSelf)
							component2.StartCoroutine(component2.HitCauserCoroutine(hittingAgent));
						
						if (GC.serverPlayer && !__instance.myMelee.successfullySleepKilled && !hittingAgent.statusEffects.hasTrait("HitObjectsNoNoise"))
							GC.spawnerMain.SpawnNoise(__instance.tr.position, 1f, null, null, hittingAgent);
						
						if (component2.startingOwner != 0 && GC.serverPlayer && !hittingAgent.statusEffects.hasTrait("NoStealPenalty"))
							GC.OwnCheck(hittingAgent, hitObject.transform.parent.gameObject, "Normal", 1);
						
						Physics2D.IgnoreCollision(component2.myCollider2D, hittingAgent.myCollider2D, true);
						Physics2D.IgnoreCollision(component2.myCollider2D, hittingAgent.agentItemCollider, true);
						__instance.MeleeHitEffect(hitObject);
						GC.playerControl.Vibrate(hittingAgent.isPlayer, Mathf.Clamp((float)__instance.myMelee.invItem.meleeDamage / 100f + 0.05f, 0f, 0.25f), Mathf.Clamp((float)__instance.myMelee.invItem.meleeDamage / 132f + 0.05f, 0f, 0.2f));
						
						if (!GC.serverPlayer && (hittingAgent.localPlayer || hittingAgent.mindControlAgent == GC.playerAgent))
						{
							hittingAgent.objectMultPlayfield.TempDisableNetworkTransform(component2);
							Quaternion localRotation2 = __instance.myMelee.meleeHelperTr.localRotation;
							__instance.myMelee.meleeHelperTr.rotation = __instance.myMelee.meleeContainerTr.rotation;
							__instance.myMelee.meleeHelperTr.position = __instance.myMelee.meleeContainerTr.position;
							__instance.myMelee.meleeHelperTr.localPosition = new Vector3(__instance.myMelee.meleeHelperTr.localPosition.x, __instance.myMelee.meleeHelperTr.localPosition.y + 10f, __instance.myMelee.meleeHelperTr.localPosition.z);
							Vector3 position4 = __instance.myMelee.meleeHelperTr.position;
							__instance.myMelee.meleeHelperTr.localPosition = Vector3.zero;
							__instance.myMelee.meleeHelperTr.localRotation = localRotation2;

							if (hittingAgent.isPlayer != 0)
							{
								hittingAgent.objectMult.CallCmdMeleeHitItem(component2.objectNetID, position4, 250, position3, component2.rb.velocity);
							
								return false;
							}

							GC.playerAgent.objectMult.CallCmdMeleeHitItemNPC(hittingAgent.objectNetID, component2.objectNetID, position4, 250, position3, component2.rb.velocity);
							
							return false;
						}
						else if (GC.multiplayerMode)
						{
							bool serverPlayer = GC.serverPlayer;
							
							return false;
						}
					}
				}
				else if (hitObject.CompareTag("MeleeHitbox"))
				{
					Melee melee = hitObject.GetComponent<MeleeColliderBox>().meleeHitbox.myMelee;
					__instance.objectList.Add(melee.meleeHitbox.gameObject);
					
					if ((__instance.myMelee.invItem.invItemName == "StealingGlove" || __instance.myMelee.invItem.invItemName == "ChloroformHankie") ||
						(melee.invItem.invItemName == "StealingGlove" || melee.invItem.invItemName == "ChloroformHankie") )
						return false;
					
					Agent agent4 = hitObject.GetComponent<MeleeColliderBox>().meleeHitbox.myMelee.agent;
					
					if ((GC.serverPlayer && hittingAgent.isPlayer == 0 && agent4.isPlayer > 0 && !agent4.localPlayer) ||
						(!GC.serverPlayer && hittingAgent.isPlayer == 0 && !agent4.localPlayer && hittingAgent != agent4 && ((agent4.mindControlAgent != GC.playerAgent && hittingAgent.mindControlAgent != GC.playerAgent) || agent4.dead)) )
						return false;
					
					if ((GC.multiplayerMode && GC.serverPlayer && hittingAgent.isPlayer == 0 && agent4.isPlayer == 0 && hittingAgent.mindControlAgent != null && hittingAgent.mindControlAgent != GC.playerAgent && !agent4.dead) ||
						(GC.multiplayerMode && GC.serverPlayer && hittingAgent.isPlayer == 0 && agent4.isPlayer == 0 && agent4.mindControlAgent != null && agent4.mindControlAgent != GC.playerAgent && !agent4.dead))
					{
						__instance.FakeHit(hitObject);
						return false;
					}

					if ((!GC.serverPlayer && hittingAgent.isPlayer > 0 && !hittingAgent.localPlayer && !agent4.localPlayer) || 
							(!hittingAgent.DontHitAlignedCheck(agent4)))
						return false;
					
					if (hittingAgent != agent4 && hittingAgent.agentSpriteTransform.localScale.x != 3f && agent4.agentSpriteTransform.localScale.x != 3f && hittingAgent.agentSpriteTransform.localScale.x > 0.34f && agent4.agentSpriteTransform.localScale.x > 0.34f && !__instance.myMelee.invItem.meleeNoHit && __instance.HasLOSMelee(melee.agent))
					{
						melee.meleeHitbox.objectList.Add(__instance.gameObject);
						__instance.objectList.Add(agent4.sprTr.gameObject);
						melee.meleeHitbox.objectList.Add(hittingAgent.sprTr.gameObject);
						int num2 = hittingAgent.FindDamageTestOnly(hitObject.GetComponent<MeleeColliderBox>().meleeHitbox.myMelee);
						int num3 = agent4.FindDamageTestOnly(__instance.myMelee);
						int num4 = 0;
						int num5;

						if (melee.specialLunge)
							num5 = Mathf.Clamp(300, 100, 200);
						else
							num5 = Mathf.Clamp(num2 * 10, 100, 200);
						
						if (__instance.myMelee.specialLunge && !fromClient)
							num4 = Mathf.Clamp(300, 100, 200);
						else if (!fromClient)
							num4 = Mathf.Clamp(num3 * 10, 100, 200);
						
						Vector3 position5 = agent4.tr.position;
						bool flag6 = true;
						
						if (GC.multiplayerMode && GC.serverPlayer)
						{
							if (agent4.isPlayer != 0 && !agent4.localPlayer && hittingAgent.isPlayer == 0)
								flag6 = false;
						
							if (hittingAgent.isPlayer != 0 && !hittingAgent.localPlayer && agent4.isPlayer == 0)
								flag6 = false;
						}

						if (flag6)
						{
							hittingAgent.movement.KnockBackBullet(melee.meleeContainerTr.gameObject, (float)num5, true, melee.agent);
							
							if (!fromClient)
								agent4.movement.KnockBackBullet(__instance.myMelee.meleeContainerTr.gameObject, (float)num4, true, hittingAgent);
						}

						if (!GC.serverPlayer)
						{
							Agent agent5 = null;
							Agent agent6 = null;

							if (hittingAgent.localPlayer || hittingAgent.mindControlAgent == GC.playerAgent)
							{
								agent5 = hittingAgent;
								agent6 = agent4;
							}
							else if (agent4.localPlayer || agent4.mindControlAgent == GC.playerAgent)
							{
								agent5 = agent4;
								agent6 = hittingAgent;
							}
							
							if (agent5 != null)
							{
								agent5.objectMultPlayfield.TempDisableNetworkTransform(agent6);
								Quaternion localRotation3 = agent5.melee.meleeHelperTr.localRotation;
								agent5.melee.meleeHelperTr.rotation = agent5.melee.meleeContainerTr.rotation;
								agent5.melee.meleeHelperTr.position = agent5.melee.meleeContainerTr.position;
								agent5.melee.meleeHelperTr.localPosition = new Vector3(agent5.melee.meleeHelperTr.localPosition.x, agent5.melee.meleeHelperTr.localPosition.y + 10f, agent5.melee.meleeHelperTr.localPosition.z);
								Vector3 position6 = agent5.melee.meleeHelperTr.position;
								Debug.Log(string.Concat("MeleeHitMelee KnockToPosition: ", position6, " - ", position5));
								agent5.melee.meleeHelperTr.localPosition = Vector3.zero;
								agent5.melee.meleeHelperTr.localRotation = localRotation3;

								if (agent5.isPlayer != 0)
									agent5.objectMult.CallCmdMeleeHitMelee(agent6.objectNetID, position6, num5, position5, agent6.rb.velocity);
								else
									GC.playerAgent.objectMult.CallCmdMeleeHitMeleeNPC(agent5.objectNetID, agent6.objectNetID, position6, num5, position5, agent6.rb.velocity);
							}
						}
						else if (GC.multiplayerMode && GC.serverPlayer)
							hittingAgent.objectMult.CallRpcMeleeHitObjectFake(agent4.objectNetID);

						if (!hittingAgent.ghost &&
							(!agent4.ghost || CanAgentMeleeHitGhost(hittingAgent)))
						{
							if (invItem.invItemName != "Fist" && agent4.inventory.equippedWeapon.invItemName == "Fist")
							{
								agent4.lastHitByAgent = hittingAgent;
								agent4.justHitByAgent2 = hittingAgent;
								agent4.lastHitByAgent = hittingAgent;
								agent4.deathMethodItem = invItem.invItemName;
								agent4.deathMethodObject = invItem.invItemName;
								agent4.deathMethod = invItem.invItemName;
								agent4.deathKiller = hittingAgent.agentName;

								if (!GC.serverPlayer || hittingAgent.localPlayer || hittingAgent.isPlayer == 0)
								{
									if (hittingAgent.zombified && agent4.isPlayer == 0 && !agent4.oma.bodyGuarded)
										agent4.zombieWhenDead = true;
									
									agent4.statusEffects.ChangeHealth(-1f);
								}
							}
							else if (invItem.invItemName == "Fist" && agent4.inventory.equippedWeapon.invItemName != "Fist")
							{
								hittingAgent.lastHitByAgent = agent4;
								hittingAgent.justHitByAgent2 = agent4;
								hittingAgent.lastHitByAgent = agent4;
								agent4.deathMethodItem = invItem.invItemName;
								agent4.deathMethodObject = invItem.invItemName;
								hittingAgent.deathMethod = agent4.inventory.equippedWeapon.invItemName;
								hittingAgent.deathKiller = agent4.agentName;

								if (!GC.serverPlayer || hittingAgent.localPlayer || hittingAgent.isPlayer == 0)
								{
									if (agent4.zombified && hittingAgent.isPlayer == 0 && !hittingAgent.oma.bodyGuarded)
										hittingAgent.zombieWhenDead = true;
									
									hittingAgent.statusEffects.ChangeHealth(-1f);
								}
							}
						}

						hittingAgent.inventory.DepleteMelee(5);
						agent4.inventory.DepleteMelee(5);
						__instance.MeleeHitEffect(hitObject);
						
						if ((hittingAgent.isPlayer > 0 && hittingAgent.localPlayer) || (melee.agent.isPlayer > 0 && melee.agent.localPlayer))
						{
							GC.ScreenShake(0.2f, 80f, hittingAgent.tr.position, hittingAgent);
							GC.FreezeFrames(1);
						}
						
						GC.alienFX.BlockAttack(hittingAgent);
						GC.alienFX.BlockAttack(melee.agent);
						
						if (!hittingAgent.killerRobot && !melee.agent.killerRobot)
						{
							GC.EnforcerAlertAttack(hittingAgent, melee.agent, 7.4f);
							GC.EnforcerAlertAttack(melee.agent, hittingAgent, 7.4f);
						}
						
						GC.playerControl.Vibrate(hittingAgent.isPlayer, Mathf.Clamp((float)__instance.myMelee.invItem.meleeDamage / 100f + 0.05f, 0f, 0.25f), Mathf.Clamp((float)__instance.myMelee.invItem.meleeDamage / 132f + 0.05f, 0f, 0.2f));
						hittingAgent.combat.meleeJustBlockedCooldown = hittingAgent.combat.meleeJustBlockedTimeStart;
						agent4.combat.meleeJustBlockedCooldown = agent4.combat.meleeJustBlockedTimeStart;
						
						if (GC.serverPlayer)
							GC.spawnerMain.SpawnNoise(__instance.tr.position, 1f, null, null, hittingAgent);
						
						if (__instance.myMelee.hitParticlesTr != null && __instance.myMelee.meleeContainerTr != null)
							GC.spawnerMain.SpawnParticleEffect("ObjectDestroyed", __instance.myMelee.hitParticlesTr.position, __instance.myMelee.meleeContainerTr.eulerAngles.z - 90f);
						
						if ((hittingAgent.statusEffects.hasTrait("ChanceToKnockWeapons") || hittingAgent.statusEffects.hasTrait("KnockWeapons")) && GC.serverPlayer && agent4.isPlayer == 0 && agent4.inventory.equippedWeapon.invItemName != "Fist" && !agent4.warZoneAgent)
						{
							int myChance4 = 15;
							bool knockWeapons = hittingAgent.statusEffects.hasTrait("KnockWeapons");
						
							if (!knockWeapons)
								myChance4 = hittingAgent.DetermineLuck(15, "ChanceToKnockWeapons", true);
							
							if (GC.percentChance(myChance4) || knockWeapons)
							{
								InvItem invItem3 = agent4.inventory.FindItem(agent4.inventory.equippedWeapon.invItemName);
								agent4.inventory.DestroyItem(invItem3);
								GC.spawnerMain.SpillItem(__instance.tr.position, invItem3);
								GC.spawnerMain.SpawnStatusText(agent4, "OutOfAmmo", invItem3.invItemName, "Item");
							
								if (!GC.serverPlayer)
									agent4.objectMultPlayfield.SpawnStatusText("OutOfAmmo", invItem3.invItemName, "Item", hittingAgent.objectNetID, "", "");
								
								agent4.statusEffects.CreateBuffText("DroppedWeapon");
								agent4.dontPickUpWeapons = true;
							}
						}

						if ((agent4.statusEffects.hasTrait("ChanceToKnockWeapons") || agent4.statusEffects.hasTrait("KnockWeapons")) && GC.serverPlayer && hittingAgent.isPlayer == 0 && invItem.invItemName != "Fist" && !agent4.warZoneAgent)
						{
							int myChance5 = 15;
						
							if (!agent4.statusEffects.hasTrait("KnockWeapons"))
								myChance5 = agent4.DetermineLuck(15, "ChanceToKnockWeapons", true);
							
							if (GC.percentChance(myChance5) || agent4.statusEffects.hasTrait("KnockWeapons"))
							{
								InvItem invItem4 = hittingAgent.inventory.FindItem(invItem.invItemName);
								hittingAgent.inventory.DestroyItem(invItem4);
								GC.spawnerMain.SpillItem(__instance.tr.position, invItem4);
								GC.spawnerMain.SpawnStatusText(hittingAgent, "OutOfAmmo", invItem4.invItemName, "Item");

								if (!GC.serverPlayer && (hittingAgent.isPlayer != 0 || hittingAgent.mindControlAgent == GC.playerAgent))
									hittingAgent.objectMultPlayfield.SpawnStatusText("OutOfAmmo", invItem4.invItemName, "Item", hittingAgent.objectNetID, "", "");
								
								hittingAgent.statusEffects.CreateBuffText("DroppedWeapon", hittingAgent.objectNetID);
								hittingAgent.dontPickUpWeapons = true;
								
								return false;
							}
						}
					}
				}
				else if (hitObject.CompareTag("BulletHitbox"))
				{
					__instance.objectList.Add(hitObject);

					if (hittingAgent.isPlayer > 0)
					{
						bool localPlayer = hittingAgent.localPlayer;
					
						return false;
					}
				}
				else if (hitObject.CompareTag("Wall"))
				{
					__instance.objectList.Add(hitObject);

					if (!__instance.hitWall && !__instance.myMelee.invItem.meleeNoHit)
					{
						__instance.hitWall = true;
						bool flag8 = false;
						int damage = hittingAgent.FindDamage(__instance.myMelee, true, fromClient);
					
						if (hittingAgent.agentSpriteTransform.localScale.x > 1f)
						{
							damage = 200;
							TileData tileData = GC.tileInfo.GetTileData(hitObject.transform.position);
						
							if (tileData.wallMaterial != wallMaterialType.Steel && tileData.wallMaterial != wallMaterialType.Glass)
								__instance.hitWall = false;
						
							if (__instance.hitWall2 == 0)
								__instance.hitWall2 = 1;
						}

						bool streamingWorld = GC.streamingWorld;
						int num7 = 30;
						int num8 = 50;
						int num9 = 50;
						int num10 = 50;
						int num11 = 200;
						
						if (GC.challenges.Contains("WallsEasilySmashed"))
						{
							num7 = 5;
							num8 = 10;
							num9 = 50;
							num10 = 50;
							num11 = 200;
						}

						if (hittingAgent.statusEffects.hasTrait("MeleeDestroysWalls2") && invItem.invItemName != "Fist")
						{
							num7 = 0;
							num8 = 0;
							num9 = 0;
							num10 = 0;
							num11 = 0;
						}
						else if (hittingAgent.statusEffects.hasTrait("MeleeDestroysWalls") && invItem.invItemName != "Fist")
						{
							num7 = 0;
							num8 = 0;
							num9 = 0;
							num10 = 50;
							num11 = 200;
						}

						if (damage >= num7)
						{
							TileData tileData2 = GC.tileInfo.GetTileData(hitObject.transform.position);

							if ((tileData2.wallMaterial == wallMaterialType.Wood && damage >= num7) || 
								(tileData2.wallMaterial == wallMaterialType.Normal && damage >= num8) || 
								(tileData2.wallMaterial == wallMaterialType.Steel && damage >= num11) || 
								(tileData2.wallMaterial == wallMaterialType.Border && damage >= num10) || 
								(tileData2.wallMaterial == wallMaterialType.Glass && damage >= num9) || 
								(damage == num11 && tileData2.wallMaterial != wallMaterialType.Steel) || 
								(damage == num10 && tileData2.wallMaterial != wallMaterialType.Border && tileData2.wallMaterial != wallMaterialType.Steel && tileData2.wallMaterial != wallMaterialType.LockdownWall))
							{
								Door.freerAgent = hittingAgent;
								GC.tileInfo.DestroyWallTileAtPosition(hitObject.transform.position.x, hitObject.transform.position.y, Vector3.zero, true, hittingAgent, false, true, false, hittingAgent, false);
								GC.audioHandler.Play(hittingAgent, "WallDestroy");

								if (hitObject.name.Contains("Glass"))
									GC.audioHandler.Play(hittingAgent, "WallDestroyGlass");
								
								hitObject.layer = 1;
								
								if (!hitObject.name.Contains("Border"))
									GC.stats.AddDestructionQuestPoints();
								
								if (hittingAgent.isPlayer > 0 && hittingAgent.localPlayer)
									GC.stats.AddToStat(hittingAgent, "Destruction", 1);
								
								GC.ScreenShake(0.25f, 160f, hittingAgent.tr.position, hittingAgent);
								
								if (__instance.hitWall2 == 1)
								{
									GC.FreezeFrames(3);
									__instance.hitWall2 = 2;
								}
								else if (damage != 200)
									GC.FreezeFrames(3);
								
								flag8 = true;
								
								if (invItem.invItemName != "Fist")
								{
									if (__instance.myMelee.depletedDuringThisAttack && invItem.invItemCount > 0)
									{
										__instance.myMelee.depletedDuringThisAttack = false;
										invItem.invItemCount += __instance.myMelee.depletedDuringThisAttackAmount;
									}
								
									if (hittingAgent.statusEffects.hasTrait("MeleeDestroysWalls2"))
										hittingAgent.inventory.DepleteMelee(10);
									else
										hittingAgent.inventory.DepleteMelee(20);
								}
							}
						}

						if (__instance.myMelee.hitParticlesTr != null && __instance.myMelee.meleeContainerTr != null)
							GC.spawnerMain.SpawnParticleEffect("ObjectDestroyed", __instance.myMelee.hitParticlesTr.position, __instance.myMelee.meleeContainerTr.eulerAngles.z - 90f);
						
						if (!__instance.myMelee.successfullySleepKilled && !hittingAgent.statusEffects.hasTrait("HitObjectsNoNoise") && (GC.serverPlayer || (!GC.serverPlayer && (hittingAgent.localPlayer || hittingAgent.mindControlAgent == GC.playerAgent))))
						{
							GC.spawnerMain.SpawnNoise(hittingAgent.tr.position, 1f, null, null, hittingAgent);
						
							if (flag8)
								GC.spawnerMain.SpawnNoise(hitObject.transform.position, 1f, null, null, hittingAgent);
						}

						if (flag8 && hittingAgent != null)
							GC.OwnCheck(hittingAgent, hitObject, "Normal", 0);
						
						GC.audioHandler.Play(hittingAgent, "BulletHitWall");
						GC.playerControl.Vibrate(hittingAgent.isPlayer, Mathf.Clamp((float)__instance.myMelee.invItem.meleeDamage / 100f + 0.05f, 0f, 0.25f), Mathf.Clamp((float)__instance.myMelee.invItem.meleeDamage / 132f + 0.05f, 0f, 0.2f));
						
						if (!GC.serverPlayer && (hittingAgent.localPlayer || hittingAgent.mindControlAgent == GC.playerAgent))
						{
							if (hittingAgent.isPlayer != 0)
							{
								hittingAgent.objectMult.CallCmdMeleeHitWall(hitObject.transform.position);
						
								return false;
							}

							GC.playerAgent.objectMult.CallCmdMeleeHitWallNPC(hittingAgent.objectNetID, hitObject.transform.position);
						}
					}
				}
				#endregion
			}

			return false;
		}
		public static bool MeleeHitbox_MeleeHitEffect(GameObject hitObject, MeleeHitbox __instance) // Prefix
		{
			// Spectral Strikes
			// This one is pared down to only relevant sections - not a replacement.

			Agent hittingAgent = __instance.myMelee.agent;

			if (CanAgentMeleeHitGhost(hittingAgent))
			{
				InvItem invItem = null;

				try
				{
					invItem = hittingAgent.inventory.equippedWeapon;

					if (hittingAgent.inventory.equippedWeapon.itemType == "WeaponProjectile")
						invItem = hittingAgent.inventory.fist;
				}
				catch { }
				
				if (hitObject.CompareTag("ObjectRealSprite"))
				{
					ObjectReal objectReal;
					
					if (hitObject.name.Contains("ExtraSprite"))
						objectReal = hitObject.transform.parent.transform.parent.GetComponent<ObjectReal>();
					else
						objectReal = hitObject.GetComponent<ObjectSprite>().objectReal;
					
					if (__instance.myMelee.recentFakeHitObjects.Contains(objectReal.go))
						return false;
					
					if (objectReal.damagedAmount < objectReal.damageThreshold && !objectReal.damageAccumulates)
					{
						if (objectReal.noMetallicSound)
						{
							GC.audioHandler.Play(objectReal, "BulletHitWall");
					
							return false;
						}

						GC.audioHandler.Play(objectReal, "BulletHitIndestructibleObject");
						
						return false;
					}
					else if (!objectReal.noDamageSound)
					{
						if (objectReal.specialDamageSound != "" && objectReal.specialDamageSound != null)
						{
							GC.audioHandler.Play(hittingAgent, objectReal.specialDamageSound);
						
							return false;
						}

						GC.audioHandler.Play(hittingAgent, "BulletHitObject");
						
						return false;
					}
				}
				else if (hitObject.CompareTag("AgentSprite"))
				{
					if (__instance.myMelee.fakeHitAgent)
						return false;
					
					Agent agent = hitObject.GetComponent<ObjectSprite>().agent;
					
					if (__instance.myMelee.recentFakeHitObjects.Contains(agent.go))
						return false;
					
					if (agent.hologram || agent.objectAgent)
						return false;
					
					bool flag = false;
					
					if (invItem.hitSoundType == "Cut")
					{
						if (agent.damagedAmount < 12)
							GC.audioHandler.Play(hittingAgent, "MeleeHitAgentCutSmall");
						else
							GC.audioHandler.Play(hittingAgent, "MeleeHitAgentCutLarge");
					
						flag = true;
					}

					if (agent.damagedAmount < 10)
					{
						if (!flag)
						{
							string hitSoundType = invItem.hitSoundType;
					
							if (!(hitSoundType == "Normal"))
							{
								if (!(hitSoundType == "WerewolfSlash"))
									GC.audioHandler.Play(hittingAgent, "MeleeHitAgentSmall");
								else
									GC.audioHandler.Play(hittingAgent, "WerewolfSlash");
							}
							else
								GC.audioHandler.Play(hittingAgent, "MeleeHitAgentSmall");
						}

						if (agent.damagedAmount > 0)
						{
							if (agent.inhuman || agent.mechFilled || agent.mechEmpty)
								GC.spawnerMain.SpawnParticleEffect("BloodHitYellow", agent.tr.position, __instance.myMelee.meleeContainerTr.eulerAngles.z - 90f);
							else
								GC.spawnerMain.SpawnParticleEffect("BloodHit", agent.tr.position, __instance.myMelee.meleeContainerTr.eulerAngles.z - 90f);
						}
						else
							GC.spawnerMain.SpawnParticleEffect("ObjectDestroyed", agent.tr.position, __instance.myMelee.meleeContainerTr.eulerAngles.z - 90f);
						
						if ((hittingAgent.isPlayer > 0 && hittingAgent.localPlayer) || (agent.isPlayer > 0 && agent.localPlayer))
						{
							GC.FreezeFrames(1);
						
							return false;
						}
					}
					else if (agent.damagedAmount < 15)
					{
						if (!flag)
						{
							string hitSoundType = invItem.hitSoundType;
							
							if (!(hitSoundType == "Normal"))
							{
								if (!(hitSoundType == "WerewolfSlash"))
									GC.audioHandler.Play(hittingAgent, "MeleeHitAgentLarge");
								else
									GC.audioHandler.Play(hittingAgent, "WerewolfSlash");
							}
							else
								GC.audioHandler.Play(hittingAgent, "MeleeHitAgentLarge");
						}

						if (agent.inhuman || agent.mechFilled || agent.mechEmpty)
							GC.spawnerMain.SpawnParticleEffect("BloodHitYellowMed", agent.tr.position, __instance.myMelee.meleeContainerTr.eulerAngles.z - 90f);
						else
							GC.spawnerMain.SpawnParticleEffect("BloodHitMed", agent.tr.position, __instance.myMelee.meleeContainerTr.eulerAngles.z - 90f);
						
						if ((hittingAgent.isPlayer > 0 && hittingAgent.localPlayer) || (agent.isPlayer > 0 && agent.localPlayer))
						{
							GC.FreezeFrames(2);
						
							return false;
						}
					}
					else
					{
						if (!flag)
						{
							string hitSoundType = invItem.hitSoundType;
							
							if (!(hitSoundType == "Normal"))
							{
								if (!(hitSoundType == "WerewolfSlash"))
									GC.audioHandler.Play(hittingAgent, "MeleeHitAgentLarge");
								else
									GC.audioHandler.Play(hittingAgent, "WerewolfSlash");
							}
							else
								GC.audioHandler.Play(hittingAgent, "MeleeHitAgentLarge");
							
							GC.audioHandler.Play(hittingAgent, "MeleeHitAgentLarge");
						}
						
						if (agent.inhuman || agent.mechFilled || agent.mechEmpty)
							GC.spawnerMain.SpawnParticleEffect("BloodHitYellowLarge", agent.tr.position, __instance.myMelee.meleeContainerTr.eulerAngles.z - 90f);
						else
							GC.spawnerMain.SpawnParticleEffect("BloodHitLarge", agent.tr.position, __instance.myMelee.meleeContainerTr.eulerAngles.z - 90f);
						
						if ((hittingAgent.isPlayer > 0 && hittingAgent.localPlayer) || (agent.isPlayer > 0 && agent.localPlayer))
						{
							GC.FreezeFrames(3);
						
							return false;
						}
					}
				}
				else if (hitObject.CompareTag("ItemImage"))
				{
					GC.audioHandler.Play(hittingAgent, "BulletHitWall");
					
					if (hittingAgent.isPlayer > 0 && hittingAgent.localPlayer)
					{
						GC.ScreenShake(0.1f, 80f, hittingAgent.tr.position, hittingAgent);
						GC.FreezeFrames(0);
					
						return false;
					}
				}
				else if (hitObject.CompareTag("MeleeHitbox"))
				{
					Agent agent2 = hitObject.GetComponent<MeleeColliderBox>().meleeHitbox.myMelee.agent;
					
					if (__instance.myMelee.recentFakeHitObjects.Contains(agent2.go))
						return false;
					
					if (invItem.hitSoundType == "Cut" || agent2.inventory.equippedWeapon.hitSoundType == "Cut")
					{
						GC.audioHandler.Play(hittingAgent, "MeleeHitMeleeBlade");
					
						return false;
					}

					GC.audioHandler.Play(hittingAgent, "MeleeHitMelee");
				}
				
				return false;
			}

			return true;
		}
		#endregion
		#region Movement
		public void Movement_00()
		{
			Type t = typeof(Movement);
			Type g = GetType();

			Postfix(t, "FindKnockBackStrength", g, "Movement_FindKnockBackStrength", new Type[1] { typeof(float) });
		}
		public static void Movement_FindKnockBackStrength(float strength, ref float __result) // Postfix
		{
			if (BMChallenges.IsChallengeFromListActive(cChallenge.Knockback))
				__result *= GetGlobalKnockBackMultiplier();
		}
		#endregion
		#region PlayfieldObject
		public void PlayfieldObject_00()
		{
			Prefix(typeof(PlayfieldObject), "FindDamage", GetType(), "PlayfieldObject_FindDamage", new Type[4] { typeof(PlayfieldObject), typeof(bool), typeof(bool), typeof(bool) });
		}
		public static bool PlayfieldObject_FindDamage(PlayfieldObject damagerObject, bool generic, bool testOnly, bool fromClient, PlayfieldObject __instance, ref int __result) // Replacement
		{
			// Not sure what Generic is, but it does not ever seem to be anything but false.

			// Ballistician
			// Double-Tapper
			// Infernal Strikes
			// Sniper
			// Spectral Strikes

			BMLog("PlayfieldObject_FindDamage");

			Agent damagedAgent = null;
			ObjectReal objectReal = null;
			bool instanceIsAgent = false;
			bool flag2 = false;
			bool instanceIsObject = false;
			bool flag4 = true;

			if (__instance.isAgent && !generic)
			{
				damagedAgent = (Agent)__instance;
				instanceIsAgent = true;
			}
			else if (__instance.isObjectReal && !generic)
			{
				objectReal = (ObjectReal)__instance;
				instanceIsObject = true;
			}
			
			Agent damagerAgent = null;
			float dmg = 0f;
			string type = "";
			bool isShotgunDamage = false;
			Item item = null;
			bool flag6 = true;
			bool isGhostBlasterDamage = false;
			bool flag8 = false;
			bool flag9 = false;

			#region Setup
			if (damagerObject.isAgent)
			{
				damagerAgent = damagerObject.GetComponent<Agent>();
				flag2 = true;

				if (damagerAgent.statusEffects.hasStatusEffect("Giant"))
					dmg = 30f;
				else if (damagerAgent.statusEffects.hasStatusEffect("ElectroTouch"))
				{
					dmg = 15f;
				
					if (instanceIsAgent)
					{
						if (damagedAgent.underWater || GC.tileInfo.GetTileData(damagedAgent.tr.position).spillWater)
						{
							if (damagedAgent.underWater)
								dmg *= 3f;
							else
								dmg *= 1.5f;
							
							if (damagerAgent.localPlayer && damagerAgent.isPlayer != 0)
								GC.unlocks.DoUnlockEarly("ElectrocuteInWater", "Extra");
						}
						else if (damagedAgent.underWater)
						{
							dmg *= 3f;

							if (damagerAgent.localPlayer && damagerAgent.isPlayer != 0)
								GC.unlocks.DoUnlockEarly("ElectrocuteInWater", "Extra");
						}

						if (!damagedAgent.dead && !testOnly)
						{
							damagedAgent.deathMethod = "ElectroTouch";
							damagedAgent.deathKiller = damagerAgent.agentName;
						}
					}
				}
				else if (damagerAgent.chargingForward)
				{
					if (instanceIsAgent)
					{
						if (!damagerAgent.oma.superSpecialAbility && !damagerAgent.statusEffects.hasTrait("ChargeMorePowerful"))
							dmg = 10f;
						else
							dmg = 20f;
						
						if (!damagedAgent.dead && !testOnly)
						{
							damagedAgent.deathMethod = "Charge";
							damagedAgent.deathKiller = damagerAgent.agentName;
						}
					}
					else
						dmg = 30f;
				}
				else if (damagerAgent == damagedAgent && damagedAgent.Tripped())
					dmg = 5f;
				else
					dmg = 30f;
				
				if (instanceIsAgent && damagedAgent.shrunk && !damagerAgent.shrunk)
				{
					dmg = 200f;
				
					if (!damagedAgent.dead && !testOnly)
					{
						damagedAgent.deathMethod = "Stomping";
						damagedAgent.deathKiller = damagerAgent.agentName;
					}
				}

				type = "TouchDamage";
			}
			else if (damagerObject.isBullet)
			{
				Bullet component = damagerObject.GetComponent<Bullet>();
				damagerAgent = component.agent;

				if (component.agent != null)
				{
					flag2 = true;

					if (instanceIsAgent && component.agent.objectAgent && component.bulletType == bulletStatus.Fire && damagedAgent.knockedByObject != null && damagedAgent.bouncy && damagedAgent.knockedByObject.playfieldObjectType == "Agent" && damagedAgent.lastHitByAgent != null)
						damagerAgent = damagedAgent.lastHitByAgent;
				}
				
				dmg = (float)component.damage;
				type = "Bullet";
				
				if (component.bulletType == bulletStatus.Fire || component.bulletType == bulletStatus.Fireball)
					type = "Fire";
				
				if (component.bulletType == bulletStatus.Shotgun && (__instance.tickEndObject == null || __instance.tickEndObject.bulletType == bulletStatus.Shotgun))
					isShotgunDamage = true;
				
				if (component.bulletType == bulletStatus.GhostBlaster)
					isGhostBlasterDamage = true;
				
				if (instanceIsAgent)
				{
					if (flag2)
					{
						if (!damagerAgent.objectAgent)
						{
							float num2 = (float)damagerAgent.accuracyStatMod;
							num2 += (float)component.moreAccuracy;
							dmg *= 0.6f + num2 / 5f;
							float x = damagerAgent.agentSpriteTransform.localScale.x;

							if (x <= 0.65f || x >= 0.67f)
								dmg *= x;

							if (!damagedAgent.dead && !testOnly)
							{
								damagedAgent.deathMethodItem = component.cameFromWeapon;
								damagedAgent.deathMethodObject = component.cameFromWeapon;
								damagedAgent.deathMethod = component.cameFromWeapon;

								if (!damagerAgent.objectAgent)
									damagedAgent.deathKiller = damagerAgent.agentName;
							}
						}
						else if (!damagedAgent.dead && !testOnly)
						{
							damagedAgent.deathMethodItem = component.cameFromWeapon;
							damagedAgent.deathMethodObject = component.cameFromWeapon;
							damagedAgent.deathMethod = component.cameFromWeapon;
							damagedAgent.deathKiller = "Nature";
						}
					}
					else if (!damagedAgent.dead && !testOnly)
					{
						if (component.bulletType == bulletStatus.Water || component.bulletType == bulletStatus.Water2)
						{
							damagedAgent.deathMethodItem = component.cameFromWeapon;
							damagedAgent.deathMethodObject = component.cameFromWeapon;
							damagedAgent.deathMethod = component.cameFromWeapon;
							damagedAgent.deathKiller = "Nature";
						}
						else
						{
							damagedAgent.deathMethodItem = component.cameFromWeapon;
							damagedAgent.deathMethodObject = damagerObject.objectName;
							damagedAgent.deathMethod = damagerObject.objectName;
							damagedAgent.deathKiller = "Nature";
						}
					}
				}
			}
			else if (damagerObject.isMelee)
			{
				Melee melee = damagerObject.playfieldObjectMelee;
				damagerAgent = melee.agent;
				flag2 = true;
				InvItem invItem;

				if (melee.invItem.weaponCode != weaponType.WeaponMelee)
					invItem = damagerAgent.inventory.fist;
				else
					invItem = melee.invItem;
				
				dmg = (float)invItem.meleeDamage;
				dmg *= 1f + (float)damagerAgent.strengthStatMod / 3f;
				float x2 = damagerAgent.agentSpriteTransform.localScale.x;
				dmg *= x2;
				type = "Melee";
				
				if (flag2 && instanceIsAgent)
				{
					if (!damagedAgent.dead && !testOnly)
					{
						damagedAgent.deathMethodItem = invItem.invItemName;
						damagedAgent.deathMethodObject = invItem.invItemName;
						damagedAgent.deathMethod = invItem.invItemName;
						damagedAgent.deathKiller = damagerAgent.agentName;
					}
				}
				else if (instanceIsAgent && !damagedAgent.dead && !testOnly)
				{
					damagedAgent.deathMethodItem = invItem.invItemName;
					damagedAgent.deathMethodObject = invItem.invItemName;
					damagedAgent.deathMethod = invItem.invItemName;
					damagedAgent.deathKiller = "Nature";
				}
			}
			else if (damagerObject.isExplosion)
			{
				Explosion explosion = damagerObject.playfieldObjectExplosion;
				damagerAgent = explosion.agent;

				if (explosion.agent != null)
				{
					flag2 = true;
				
					if (instanceIsAgent)
					{
						if (explosion.realSource != null && explosion.realSource.isItem && (!damagedAgent.movement.HasLOSAgent360(explosion.agent) || Vector2.Distance(damagedAgent.curPosition, explosion.agent.curPosition) > explosion.agent.LOSRange / damagedAgent.hardToSeeFromDistance))
							flag4 = false;
					
						if (explosion.sourceObject != null && explosion.sourceObject.isBullet && explosion.sourceObject.playfieldObjectBullet.cameFromWeapon == "Fireworks" && (!damagedAgent.movement.HasLOSAgent360(explosion.agent) || Vector2.Distance(damagedAgent.curPosition, explosion.agent.curPosition) > explosion.agent.LOSRange / damagedAgent.hardToSeeFromDistance))
							flag4 = false;
					}
				}

				dmg = (float)explosion.damage;
				type = "Explosion";
				
				if (flag2 && instanceIsAgent)
				{
					if (!damagedAgent.dead && !testOnly)
					{
						damagedAgent.deathMethod = "Explosion";

						if (damagerAgent != damagedAgent && !damagerAgent.objectAgent)
							damagedAgent.deathKiller = damagerAgent.agentName;
						else
							damagedAgent.deathKiller = "Self";
					}
				}
				else if (instanceIsAgent && !damagedAgent.dead && !testOnly)
				{
					damagedAgent.deathMethod = "Explosion";
					damagedAgent.deathKiller = "Nature";
				}
			}
			else if (damagerObject.isFire)
			{
				Fire fire = damagerObject.playfieldObjectFire;
				damagerAgent = fire.agent;

				if (fire.agent != null)
				{
					flag2 = true;
				
					if (instanceIsAgent && (!damagedAgent.movement.HasLOSAgent360(fire.agent) || Vector2.Distance(damagedAgent.curPosition, fire.agent.curPosition) > fire.agent.LOSRange / damagedAgent.hardToSeeFromDistance))
						flag4 = false;
				}

				dmg = (float)fire.damage;
				type = "Fire";
				
				if (instanceIsAgent)
				{
					if (flag2)
					{
						if (!damagedAgent.dead && !testOnly)
						{
							damagedAgent.deathMethod = "Fire";
				
							if (!damagerAgent.objectAgent)
								damagedAgent.deathKiller = damagerAgent.agentName;
						}
					}
					else if (!damagedAgent.dead && !testOnly)
					{
						damagedAgent.deathMethod = "Fire";
						damagedAgent.deathKiller = "Nature";
					}
				}
			}
			else if (damagerObject.isObjectReal)
			{
				ObjectReal objectReal2 = damagerObject.playfieldObjectReal;
				dmg = (float)objectReal2.hazardDamage;
				type = "Hazard";

				if (instanceIsAgent && damagedAgent.knockedByObject != null && damagedAgent.bouncy && damagedAgent.knockedByObject.playfieldObjectType == "Agent" && damagedAgent.lastHitByAgent != null)
				{
					damagerAgent = damagedAgent.lastHitByAgent;
					flag2 = true;
				}
				
				if (flag2 && instanceIsAgent)
				{
					if (!damagedAgent.dead && !testOnly)
					{
						damagedAgent.deathMethodItem = objectReal2.objectName;
						damagedAgent.deathMethodObject = objectReal2.objectName;
						damagedAgent.deathMethod = objectReal2.objectName;
				
						if (!damagerAgent.objectAgent)
							damagedAgent.deathKiller = damagerAgent.agentName;
					}
				}
				else if (instanceIsAgent)
				{
					if (!damagedAgent.dead && !testOnly)
					{
						damagedAgent.deathMethodItem = objectReal2.objectName;
						damagedAgent.deathMethodObject = objectReal2.objectName;
						damagedAgent.deathMethod = objectReal2.objectName;
						damagedAgent.deathKiller = "Nature";
					}
				}
				else if (instanceIsObject)
					dmg = 30f;
			}
			else if (damagerObject.isItem)
			{
				item = damagerObject.playfieldObjectItem;

				if (item.invItem.otherDamage > 0 && item.otherDamageMode)
				{
					if (item.hitCauser != null)
					{
						damagerAgent = item.hitCauser;
						flag2 = true;
					}
					else if (item.owner != null)
					{
						damagerAgent = item.owner;
						flag2 = true;
				
						if (instanceIsAgent && (!damagedAgent.movement.HasLOSAgent360(item.owner) || Vector2.Distance(damagedAgent.curPosition, item.owner.curPosition) > item.owner.LOSRange / damagedAgent.hardToSeeFromDistance))
							flag4 = false;
					}

					dmg = (float)item.invItem.otherDamage;
				}
				else if (item.invItem.touchDamage > 0 && __instance.playfieldObjectType == "Agent")
				{
					if (item.hitCauser != null)
					{
						damagerAgent = item.hitCauser;
						flag2 = true;
					}
					else if (item.owner != null)
					{
						damagerAgent = item.owner;
						flag2 = true;
					
						if (instanceIsAgent && (!damagedAgent.movement.HasLOSAgent360(item.owner) || Vector2.Distance(damagedAgent.curPosition, item.owner.curPosition) > item.owner.LOSRange / damagedAgent.hardToSeeFromDistance))
							flag4 = false;
					}
					
					if (item.invItem.touchDamage > 0)
						dmg = (float)item.invItem.touchDamage;
					else if (item.invItem.otherDamage > 0)
						dmg = (float)item.invItem.otherDamage;
					
					if (item.thrower != null)
						type = "Throw";
				}
				else
				{
					if (item.thrower != null && item.invItem.throwDamage != 0)
					{
						damagerAgent = item.thrower;
						flag2 = true;
					}
				
					dmg = (float)item.invItem.throwDamage;
					
					if (flag2 && item.invItem.invItemName == "TossItem" && (damagerAgent.oma.superSpecialAbility || damagerAgent.statusEffects.hasTrait("GoodThrower")))
						dmg *= 2f;
					
					type = "Throw";
				}

				if (!flag2 && item.thrower != null && item.thrower.statusEffects.hasTrait("KillerThrower"))
				{
					damagerAgent = item.thrower;
					flag2 = true;
					type = "Throw";
				}
				
				if (flag2 && instanceIsAgent)
				{
					if (!damagedAgent.dead && !testOnly)
					{
						damagedAgent.deathMethodItem = item.invItem.invItemName;
						damagedAgent.deathMethodObject = item.invItem.invItemName;
						damagedAgent.deathMethod = item.invItem.invItemName;
				
						if (!damagerAgent.objectAgent)
							damagedAgent.deathKiller = damagerAgent.agentName;
					}
				}
				else if (instanceIsAgent && !damagedAgent.dead && !testOnly)
				{
					damagedAgent.deathMethodItem = item.invItem.invItemName;
					damagedAgent.deathMethodObject = item.invItem.invItemName;
					damagedAgent.deathMethod = item.invItem.invItemName;
					damagedAgent.deathKiller = "Nature";
				}
			}
			#endregion

			bool playerDamagedByNpc = false;

			if (flag2)
			{
				if (damagerAgent.isPlayer != 0 && !damagerAgent.localPlayer)
					playerDamagedByNpc = true;
			
				if (instanceIsAgent && damagedAgent.isPlayer != 0 && damagerAgent.isPlayer != 0 && !GC.pvp)
					flag6 = false;
			}

			#region Hit types
			if (type == "Melee")
			{
				if (damagerAgent.statusEffects.hasTrait("Strength"))
					dmg *= 1.5f;
				
				if (damagerAgent.statusEffects.hasTrait("StrengthSmall"))
					dmg *= 1.25f;
				
				if (damagerAgent.statusEffects.hasTrait("Weak"))
					dmg *= 0.5f;
				
				if (damagerAgent.statusEffects.hasTrait("Withdrawal"))
					dmg *= 0.75f;

				#region Blessed / Infernal Strikes
				if (instanceIsAgent)
				{
					BMLog("\tdamagedAgent Name: " + damagedAgent.agentName);
					string invItemName = damagerAgent.inventory.equippedWeapon.invItemName;

					if (vAgent.Undead.Contains(damagedAgent.agentName) || vAgent.Evil.Contains(damagedAgent.agentName))
					{
						if (damagerAgent.statusEffects.hasTrait(cTrait.BlessedStrikes) && invItemName == vItem.Fist)
							dmg *= 1.50f;
						else if (damagerAgent.statusEffects.hasTrait(cTrait.BlessedStrikes_2) && invItemName == vItem.Fist || invItemName == vItem.BaseballBat)
							dmg *= 2.00f;
					}
					else if (!vAgent.Nonhuman.Contains(damagedAgent.agentName)) // Non-Undead, non-Robot
					{
						if (damagerAgent.statusEffects.hasTrait(cTrait.InfernalStrikes) && invItemName == vItem.Fist)
							dmg *= 1.25f;
						else if (damagerAgent.statusEffects.hasTrait(cTrait.InfernalStrikes_2) && invItemName == vItem.Fist || invItemName == vItem.Axe)
							dmg *= 1.50f;
					}
				}
				#endregion

				if (damagerAgent.melee.specialLunge)
				{
					if (damagerAgent.agentName == "WerewolfB")
						dmg *= 1.3f;
					else
						dmg *= 2f;
				}
				
				if (damagerAgent.inventory.equippedWeapon.invItemName == "Fist" || damagerAgent.inventory.equippedWeapon.itemType == "WeaponProjectile")
				{
					if (damagerAgent.statusEffects.hasTrait("StrongFists2"))
						dmg *= 1.8f;
					else if (damagerAgent.statusEffects.hasTrait("StrongFists"))
						dmg *= 1.4f;
				
					if (damagerAgent.statusEffects.hasTrait("CantAttack") && __instance.isAgent)
					{
						dmg = 0f;
						flag8 = true;
					}
					else if (damagerAgent.statusEffects.hasTrait("AttacksOneDamage") && __instance.isAgent)
					{
						dmg = 1f;
						flag9 = true;
					}
				}

				if (!playerDamagedByNpc && flag6)
				{
					if (damagerAgent.inventory.equippedArmor != null && !testOnly && (damagerAgent.inventory.equippedArmor.armorDepletionType == "MeleeAttack" && instanceIsAgent) && !damagedAgent.dead && !damagedAgent.mechEmpty && !damagedAgent.butlerBot)
						damagerAgent.inventory.DepleteArmor("Normal", Mathf.Clamp((int)(dmg / 2f), 0, 12));
				
					if (damagerAgent.inventory.equippedArmorHead != null && !testOnly && (damagerAgent.inventory.equippedArmorHead.armorDepletionType == "MeleeAttack" && instanceIsAgent) && !damagedAgent.dead && !damagedAgent.mechEmpty && !damagedAgent.butlerBot)
						damagerAgent.inventory.DepleteArmor("Head", Mathf.Clamp((int)(dmg / 2f), 0, 12));
				}

				if (instanceIsAgent)
				{
					float num3 = dmg / damagerAgent.agentSpriteTransform.localScale.x;
				
					if (!damagedAgent.dead && !testOnly && !playerDamagedByNpc && flag6 && !damagedAgent.butlerBot && !damagedAgent.mechEmpty)
						damagerAgent.inventory.DepleteMelee(Mathf.Clamp((int)num3, 0, 15), damagerObject.playfieldObjectMelee.invItem);
					
					if ((damagerAgent.statusEffects.hasTrait("SleepKiller") || damagerAgent.statusEffects.hasTrait("Backstabber")) && damagedAgent.sleeping)
					{
						dmg = 200f;
						damagedAgent.agentHitboxScript.wholeBodyMode = 0;
						damagerAgent.melee.successfullySleepKilled = true;
					
						if (damagerAgent.statusEffects.hasTrait("Backstabber"))
							damagedAgent.statusEffects.CreateBuffText("Backstab", damagedAgent.objectNetID);
					}
					else if ((damagerAgent.melee.mustDoBackstab && dmg != 200f && !damagedAgent.dead) || (damagerAgent.statusEffects.hasTrait("Backstabber") && ((damagedAgent.mostRecentGoalCode != goalType.Battle && damagedAgent.mostRecentGoalCode != goalType.Flee) || damagedAgent.frozen) && !damagedAgent.movement.HasLOSObjectBehind(damagerAgent) && !damagedAgent.sleeping && dmg != 200f && !damagedAgent.dead))
					{
						damagedAgent.agentHelperTr.localPosition = new Vector3(-0.64f, 0f, 0f);

						if (!GC.tileInfo.IsOverlapping(damagedAgent.agentHelperTr.position, "Wall"))
						{
							damagedAgent.agentHelperTr.localPosition = Vector3.zero;
							damagedAgent.statusEffects.CreateBuffText("Backstab", damagedAgent.objectNetID);
						
							if (damagerAgent.statusEffects.hasStatusEffect("InvisibleLimited") || (damagerAgent.statusEffects.hasStatusEffect("Invisible") && damagerAgent.statusEffects.hasSpecialAbility("InvisibleLimitedItem")))
							{
								dmg *= 10f;
								damagerAgent.melee.successfullyBackstabbed = true;
								GC.OwnCheck(damagerAgent, damagedAgent.go, "Normal", 0);
							}
							else
								dmg *= 2f;
						}
					}
					else if (damagerAgent.statusEffects.hasStatusEffect("InvisibleLimited"))
					{
						bool flag11 = false;

						if (instanceIsAgent && damagedAgent.dead)
							flag11 = true;
						
						if (!playerDamagedByNpc && !flag11 && !damagerAgent.oma.superSpecialAbility && !damagerAgent.statusEffects.hasTrait("FailedAttacksDontEndCamouflage"))
							damagerAgent.statusEffects.RemoveInvisibleLimited();
					}
				}
			}
			else if (type == "Bullet")
			{
				if (instanceIsAgent)
				{
					if (!isGhostBlasterDamage)
					{
						BMLog("\tBullet hit");

						if (damagerAgent.statusEffects.hasTrait(cTrait.Ballistician))
							dmg *= 1.25f;
						else if (damagerAgent.statusEffects.hasTrait(cTrait.Ballistician_2))
							dmg *= 1.50f;

						if (damagedAgent.statusEffects.hasTrait("ResistBullets"))
							dmg /= 1.5f;

						if (damagedAgent.statusEffects.hasTrait("ResistBulletsSmall"))
							dmg /= 1.2f;

						if (damagedAgent.statusEffects.hasTrait("ResistBulletsTrait2"))
							dmg /= 2.0f;
						else if (damagedAgent.statusEffects.hasTrait("ResistBulletsTrait"))
							dmg /= 1.5f;

						bool headShot = false;
						bool doubleTap = false;
						bool sniped = false;
						bool hidden = (!(damagerAgent.hiddenInBush is null) || !(damagerAgent.hiddenInObject is null));
						bool invisible = (damagerAgent.statusEffects.hasStatusEffect("InvisibleLimited") || (damagerAgent.statusEffects.hasStatusEffect("Invisible") && damagerAgent.statusEffects.hasSpecialAbility("InvisibleLimitedItem")) ||
											hidden);
						bool hasLOSBehind = damagedAgent.movement.HasLOSObjectBehind(damagerAgent);
						bool hasLOSAgent = damagedAgent.movement.HasLOSAgent(damagerAgent);
						float distance = Vector2.Distance(damagerAgent.tr.position, damagedAgent.tr.position);
						Bullet bullet = (Bullet)damagerObject;

						BMLog("\theadshot:\t" + headShot);
						BMLog("\tdoubleTap:\t" + doubleTap);
						BMLog("\tsniped:\t\t" + sniped);
						BMLog("\thidden:\t\t" + hidden);
						BMLog("\tinvisible:\t" + invisible);
						BMLog("\tdistance:\t" + distance);
						BMLog("\tHasLOSBehind:\t" + hasLOSBehind);
						BMLog("\tHasLOSAgent:\t" + hasLOSAgent);
						BMLog("\twepName: " + bullet.cameFromWeapon);

						if (damagerAgent.statusEffects.hasTrait(cTrait.DoubleTapper) &&
							((!hasLOSBehind && !hasLOSAgent) || hidden || invisible || damagedAgent.sleeping) &&
							distance <= 0.96f)
						{
							headShot = true;
							doubleTap = true;
						}

						if (damagerAgent.statusEffects.hasTrait(cTrait.Sniper) && bullet.cameFromWeapon == vItem.Revolver &&
							(((!hasLOSAgent || hidden || invisible || damagedAgent.sleeping) && distance >= 4.00f) ||
							distance >= 8.00f))
						{
							headShot = true;
							sniped = true;
						}

						if (headShot)
						{
							if (damagedAgent.sleeping)
							{
								dmg = 800f;
								damagedAgent.agentHitboxScript.wholeBodyMode = 0;
								damagerAgent.melee.successfullySleepKilled = true;

								damagedAgent.statusEffects.CreateBuffText(cBuffText.Headshot, damagedAgent.objectNetID);
							}
							else
							{
								damagedAgent.agentHelperTr.localPosition = new Vector3(-0.64f, 0f, 0f);
								 
								if (!GC.tileInfo.IsOverlapping(damagedAgent.agentHelperTr.position, "Wall"))
								{
									damagedAgent.agentHelperTr.localPosition = Vector3.zero;
									damagedAgent.statusEffects.CreateBuffText(cBuffText.Headshot, damagedAgent.objectNetID);

									if (doubleTap)
									{
										if (invisible)
										{
											dmg *= 10f;
											damagerAgent.melee.successfullyBackstabbed = true;
											GC.OwnCheck(damagerAgent, damagedAgent.go, "Normal", 0);
										}
										else
											dmg *= 2f; 
									}
									else if (sniped)
										dmg *= distance;

									BMLog("\tBullet damage: " + dmg);
								}
							}
							//else if (damagerAgent.statusEffects.hasStatusEffect(vStatusEffect.InvisibleTemporary))
							//{
							//	bool alreadyDead = false;

							//	if (instanceIsAgent && damagedAgent.dead)
							//		alreadyDead = true;

							//	if (!playerDamagedByNpc && !alreadyDead && !damagerAgent.statusEffects.hasTrait(cTrait.Sniper))
							//		damagerAgent.statusEffects.RemoveInvisibleLimited();
							//}
						}
					}
				}
			}
			else if (type == "Fire")
			{
				if (instanceIsAgent)
				{
					if (damagedAgent.statusEffects.hasTrait("ResistFire"))
						dmg /= 1.5f;
					
					if ((damagedAgent.oma.superSpecialAbility && damagedAgent.agentName == "Firefighter") || damagedAgent.statusEffects.hasTrait("FireproofSkin2"))
					{
						dmg = 0f;
						flag8 = true;
					}
					else if (damagedAgent.statusEffects.hasTrait("FireproofSkin"))
						dmg /= 1.5f;
				}
			}
			else if (type == "Throw")
			{
				if (flag2)
				{
					if (damagerAgent.statusEffects.hasTrait("GoodThrower"))
						dmg *= 2f;
					
					if (instanceIsAgent && damagerAgent.statusEffects.hasTrait("KillerThrower") && item.throwerReal == item.thrower)
					{
						if (damagedAgent != item.thrower)
						{
							if (damagedAgent.health >= 100f)
								dmg = 100f;
							else
								dmg = 200f;
						}
						else
							dmg = 20f;
					}
				}
			}
			else if (!(type == "Explosion"))
				_ = type == "Hazard";
			#endregion

			if (flag2 && instanceIsAgent && !testOnly)
			{
				if (damagerAgent.statusEffects.hasTrait("BloodyMess"))
					damagedAgent.bloodyMessed = true;
			
				if ((damagerAgent.invisible && !damagerAgent.oma.hidden) || damagerAgent.ghost)
				{
					damagerAgent.gc.spawnerMain.SpawnDanger(damagerAgent, "Targeted", "Spooked", damagedAgent);
					relStatus relCode = damagerAgent.relationships.GetRelCode(damagedAgent);
				
					if (relCode != relStatus.Aligned && relCode != relStatus.Loyal)
					{
						List<Agent> agentList = GC.agentList;
					
						for (int i = 0; i < agentList.Count; i++)
						{
							Agent agent3 = agentList[i];
						
							if (agent3.employer == damagerAgent)
							{
								relStatus relCode2 = agent3.relationships.GetRelCode(damagedAgent);
							
								if (relCode2 != relStatus.Aligned && relCode2 != relStatus.Loyal)
									agent3.relationships.SetRelHate(damagedAgent, 5);
								else if (relCode2 == relStatus.Aligned && agent3.relationships.GetRelCode(damagerAgent) == relStatus.Loyal)
								{
									agent3.relationships.SetRelHate(damagerAgent, 5);
									damagerAgent.agentInteractions.LetGo(agent3, damagerAgent);
								}
							}
						}
					}
				}
			}

			if (instanceIsAgent)
			{
				if (damagedAgent.statusEffects.hasTrait("NumbToPain"))
					dmg /= 3f;
				
				if (damagedAgent.statusEffects.hasTrait("ResistDamageSmall"))
					dmg /= 1.25f;
				
				if (damagedAgent.statusEffects.hasTrait("ResistDamageMed"))
					dmg /= 1.5f;
				
				if (damagedAgent.statusEffects.hasTrait("ResistDamageLarge"))
					dmg /= 2f;
				
				if (damagedAgent.statusEffects.hasTrait("Giant"))
					dmg /= 3f;
				
				if (damagedAgent.statusEffects.hasTrait("Shrunk"))
					dmg *= 3f;
				
				if (damagedAgent.statusEffects.hasTrait("Diminutive"))
					dmg *= 1.5f;
				
				if (damagedAgent.frozen)
					dmg *= 2f;
				
				if (damagedAgent.statusEffects.hasSpecialAbility("ProtectiveShell") && damagedAgent.objectMult.chargingSpecialLunge)
					dmg /= 8f;
				
				if (damagedAgent.hasEmployer && damagedAgent.employer.statusEffects.hasSpecialAbility("ProtectiveShell") && damagedAgent.employer.objectMult.chargingSpecialLunge)
					dmg /= 8f;
				
				if (damagedAgent.oma.mindControlled && damagedAgent.mindControlAgent != null && (damagedAgent.mindControlAgent.statusEffects.hasTrait("MindControlledResistDamage") || (damagedAgent.mindControlAgent.oma.superSpecialAbility && damagedAgent.mindControlAgent.agentName == "Alien")))
					dmg /= 1.5f;
				
				if (flag2 && flag6 && !damagerAgent.dead)
				{
					if (damagerAgent.statusEffects.hasTrait("MoreDamageWhenHealthLow") && damagerAgent.agentID != damagedAgent.agentID)
					{
						int num4 = (int)(damagerAgent.healthMax / 4f);
				
						if (damagerAgent.health <= (float)num4)
						{
							float num5 = damagerAgent.health / (float)num4;
							num5 = (1f - num5) * dmg * 1.5f;
							dmg += num5;
						}
					}
					else if (damagerAgent.statusEffects.hasTrait("MoreDamageWhenHealthLow2") && damagerAgent.agentID != damagedAgent.agentID)
					{
						int num6 = (int)(damagerAgent.healthMax / 2f);
						
						if (damagerAgent.health <= (float)num6)
						{
							float num7 = damagerAgent.health / (float)num6;
							num7 = (1f - num7) * dmg * 1.5f;
							dmg += num7;
						}
					}

					if (!testOnly && damagerAgent.agentID != damagedAgent.agentID)
					{
						int num8 = damagerAgent.critChance;
						num8 = damagerAgent.DetermineLuck(num8, "CritChance", true);

						if (Random.Range(0, 100) <= num8 - 1 && (!(GC.levelType == "Tutorial") || !(type == "Explosion")))
						{
							dmg *= 2f;
							damagedAgent.critted = true;
						}
						
						if (damagerAgent.statusEffects.hasTrait("ChanceToSlowEnemies2"))
						{
							int myChance = damagerAgent.DetermineLuck(20, "ChanceToSlowEnemies", true);
						
							if (GC.percentChance(myChance))
								damagedAgent.statusEffects.AddStatusEffect("Slow");
						}
						else if (damagerAgent.statusEffects.hasTrait("ChanceToSlowEnemies"))
						{
							int myChance2 = damagerAgent.DetermineLuck(8, "ChanceToSlowEnemies", true);
						
							if (GC.percentChance(myChance2))
								damagedAgent.statusEffects.AddStatusEffect("Slow");
						}
					}

					if (damagerAgent.statusEffects.hasTrait("MoreFollowersCauseMoreDamage") || damagerAgent.statusEffects.hasTrait("MoreFollowersCauseMoreDamage2"))
					{
						float num9 = 1.2f;

						if (damagerAgent.statusEffects.hasTrait("MoreFollowersCauseMoreDamage2"))
							num9 = 1.4f;
						
						float num10 = dmg;
						int num11 = 0;
						
						for (int j = 0; j < GC.agentList.Count; j++)
						{
							Agent agent4 = GC.agentList[j];
						
							if (agent4.hasEmployer && agent4.employer == damagerAgent && Vector2.Distance(agent4.tr.position, damagedAgent.tr.position) < 10.24f)
							{
								dmg += num10 * num9 - num10;
								num11++;
							
								if (num11 >= 3 && !GC.challenges.Contains("NoLimits"))
									break;
							}
						}
					}

					if (damagerAgent.oma.mindControlled && damagerAgent.mindControlAgent != null && (damagerAgent.mindControlAgent.statusEffects.hasTrait("MindControlledDamageMore") || (damagerAgent.mindControlAgent.oma.superSpecialAbility && damagerAgent.mindControlAgent.agentName == "Alien")))
						dmg *= 1.5f;
				}

				int num12 = 0;
				
				if (damagedAgent.inventory.equippedArmor != null && !testOnly && flag6)
				{
					InvItem equippedArmor = damagedAgent.inventory.equippedArmor;
				
					if (equippedArmor.armorDepletionType == "Everything")
						num12++;
					else if (equippedArmor.armorDepletionType == "Bullet" && type == "Bullet")
						num12++;
					else if (equippedArmor.armorDepletionType == "Fire" && type == "Fire")
						num12++;
					else if (equippedArmor.armorDepletionType == "FireAndEverything")
						num12++;
				}

				if (damagedAgent.inventory.equippedArmorHead != null && !testOnly && flag6)
				{
					InvItem equippedArmorHead = damagedAgent.inventory.equippedArmorHead;
				
					if (equippedArmorHead.armorDepletionType == "Everything")
						num12++;
					else if (equippedArmorHead.armorDepletionType == "Bullet" && type == "Bullet")
						num12++;
					else if (equippedArmorHead.armorDepletionType == "Fire" && type == "Fire")
						num12++;
					else if (equippedArmorHead.armorDepletionType == "FireAndEverything")
						num12++;
				}

				if (damagedAgent.inventory.equippedArmor != null && !testOnly && flag6)
				{
					InvItem equippedArmor2 = damagedAgent.inventory.equippedArmor;
				
					if (equippedArmor2.armorDepletionType == "Everything")
						damagedAgent.inventory.DepleteArmor("Normal", Mathf.Clamp((int)(dmg * 2f), 0, 12) / num12);
					else if (equippedArmor2.armorDepletionType == "Bullet" && type == "Bullet")
						damagedAgent.inventory.DepleteArmor("Normal", Mathf.Clamp((int)(dmg * 2f), 0, 12) / num12);
					else if (equippedArmor2.armorDepletionType == "Fire" && type == "Fire")
						damagedAgent.inventory.DepleteArmor("Normal", Mathf.Clamp((int)(dmg * 2f), 0, 12) / num12);
					else if (equippedArmor2.armorDepletionType == "FireAndEverything")
						damagedAgent.inventory.DepleteArmor("Normal", Mathf.Clamp((int)(dmg * 2f), 0, 12) / num12);
				}

				if (damagedAgent.inventory.equippedArmorHead != null && !testOnly && flag6)
				{
					InvItem equippedArmorHead2 = damagedAgent.inventory.equippedArmorHead;

					if (equippedArmorHead2.armorDepletionType == "Everything")
						damagedAgent.inventory.DepleteArmor("Head", Mathf.Clamp((int)(dmg * 2f), 0, 12) / num12);
					else if (equippedArmorHead2.armorDepletionType == "Bullet" && type == "Bullet")
						damagedAgent.inventory.DepleteArmor("Head", Mathf.Clamp((int)(dmg * 2f), 0, 12) / num12);
					else if (equippedArmorHead2.armorDepletionType == "Fire" && type == "Fire")
						damagedAgent.inventory.DepleteArmor("Head", Mathf.Clamp((int)(dmg * 2f), 0, 12) / num12);
					else if (equippedArmorHead2.armorDepletionType == "FireAndEverything")
						damagedAgent.inventory.DepleteArmor("Head", Mathf.Clamp((int)(dmg * 2f), 0, 12) / num12);
				}

				if (damagedAgent.statusEffects.hasTrait("MoreFollowersLessDamageToPlayer") || damagedAgent.statusEffects.hasTrait("MoreFollowersLessDamageToPlayer2"))
				{
					int num13 = 0;
					float num14 = 1.2f;
				
					if (damagedAgent.statusEffects.hasTrait("MoreFollowersLessDamageToPlayer2"))
						num14 = 1.4f;
					
					for (int k = 0; k < GC.agentList.Count; k++)
					{
						Agent agent5 = GC.agentList[k];
					
						if (agent5.hasEmployer && agent5.employer == damagedAgent && Vector2.Distance(agent5.tr.position, damagedAgent.tr.position) < 10.24f)
						{
							dmg /= num14;
							num13++;
						
							if (num13 >= 3 && !GC.challenges.Contains("NoLimits"))
								break;
						}
					}
				}

				if (!testOnly && flag4)
					damagedAgent.attackCooldown = 2f;
			}

			if (instanceIsObject && flag2 && (__instance.objectName == "Bars" || __instance.objectName == "BarbedWire"))
			{
				if (damagerAgent.statusEffects.hasTrait("MeleeDestroysWalls2"))
					dmg = 99f;
				else if (damagerAgent.statusEffects.hasTrait("MeleeDestroysWalls") && __instance.objectName == "BarbedWire")
					dmg = 99f;
			}
			
			if (dmg > 200f)
				dmg = 200f;
			
			int num15 = Mathf.Clamp((int)dmg, 1, 1000);
			
			if ((damagerObject.isItem && type == "Throw" && dmg == 0f) || flag8)
				num15 = 0;
			else if (flag9)
				num15 = 1;
			
			if (flag2 && instanceIsAgent && !testOnly)
			{
				if ((float)num15 < damagedAgent.health)
				{
					Relationship relationship = damagedAgent.relationships.GetRelationship(damagerAgent);
					relStatus myRel = relStatus.Neutral;
					bool flag12 = false;
			
					if (relationship != null)
					{
						myRel = relationship.relTypeCode;
						flag12 = relationship.sawBecomeHidden;
					}
					
					if ((!damagerAgent.invisible || flag12) && flag4)
					{
						if ((damagerAgent.isPlayer <= 0 || damagerAgent.localPlayer || damagerObject.isItem || damagerObject.isExplosion || damagerAgent.statusEffects.hasTrait("CantAttack")) && (!damagerObject.isExplosion || !damagerObject.noAngerOnHit) && !damagedAgent.mechEmpty)
						{
							damagedAgent.justHitByAgent3 = true;
							damagedAgent.relationships.AddRelHate(damagerAgent, Mathf.Clamp(num15, 5, 200));
							damagedAgent.justHitByAgent3 = false;
						}
					
						damagedAgent.relationships.PotentialAlignmentCheck(myRel);
					}
				}

				if (flag4)
					damagedAgent.SetJustHitByAgent(damagerAgent);
				
				damagedAgent.justHitByAgent2 = damagerAgent;
				damagedAgent.lastHitByAgent = damagerAgent;
				
				if (!damagerAgent.killerRobot && !damagedAgent.killerRobot)
				{
					relStatus relCode3 = damagerAgent.relationships.GetRelCode(damagedAgent);
				
					if (damagerObject.isExplosion)
					{
						Explosion explosion2 = (Explosion)damagerObject;
					
						if (explosion2.explosionType == "Huge" || explosion2.explosionType == "Ridiculous")
						{
							GC.EnforcerAlertAttack(damagerAgent, damagedAgent, 10.8f, explosion2.tr.position);
						
							if (damagerAgent.ownerID != 0 && relCode3 == relStatus.Hostile)
								GC.EnforcerAlertAttack(damagedAgent, damagerAgent, 10.8f, explosion2.tr.position);
						}
						else
						{
							GC.EnforcerAlertAttack(damagerAgent, damagedAgent, 10.8f, explosion2.tr.position);
						
							if (damagerAgent.ownerID != 0 && relCode3 == relStatus.Hostile)
								GC.EnforcerAlertAttack(damagedAgent, damagerAgent, 10.8f, explosion2.tr.position);
						}
					}
					else
					{
						GC.EnforcerAlertAttack(damagerAgent, damagedAgent, 7.4f);
						
						if (damagerAgent.ownerID != 0 && relCode3 == relStatus.Hostile)
							GC.EnforcerAlertAttack(damagedAgent, damagerAgent, 7.4f);
					}
				}

				damagedAgent.damagedAmount = num15;
				
				if (damagedAgent.agentName == "Slave")
					__instance.StartCoroutine(damagedAgent.agentInteractions.OwnCheckSlaveOwners(damagedAgent, damagerAgent));
				
				if (damagedAgent.isPlayer == 0 && !damagedAgent.hasEmployer && !damagedAgent.zombified && !damagedAgent.noEnforcerAlert)
					damagerAgent.oma.hasAttacked = true;
			}

			if (instanceIsObject)
			{
				if (flag2)
				{
					if (!testOnly)
					{
						objectReal.lastHitByAgent = damagerAgent;
						objectReal.damagedAmount = num15;
			
						if (objectReal.useForQuest != null || objectReal.destroyForQuest != null)
							GC.OwnCheck(damagerAgent, objectReal.gameObject, "Normal", 0);
					}

					if (!damagerAgent.objectAgent && damagerAgent.agentSpriteTransform.localScale.x > 1f)
						num15 = 99;
				}
				else if (!testOnly)
				{
					objectReal.lastHitByAgent = null;
					objectReal.damagedAmount = num15;
				}
			}

			if (!isShotgunDamage || instanceIsObject || fromClient)
			{
				__result = num15;
				return false;
			}
			
			__instance.tickEndDamage += num15;
			__instance.tickEndObject = damagerObject;
			__instance.tickEndRotation = damagerObject.tr.rotation;
			
			if (fromClient)
				__instance.tickEndDamageFromClient = true;
			else
				__instance.tickEndDamageFromClient = false;
			
			if (__instance.tickEndObject.isBullet)
				__instance.tickEndBullet = (Bullet)__instance.tickEndObject;
			
			__result = 9999;

			return false;
		}
		#endregion
		#region SpawnerMain
		public void SpawnerMain_00()
		{
		}
		#endregion
	}
}
