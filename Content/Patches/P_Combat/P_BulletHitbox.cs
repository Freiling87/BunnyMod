using BepInEx.Logging;
using BunnyMod.Content.Logging;
using BunnyMod.Content.Traits;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;
using Random = UnityEngine.Random;

namespace BunnyMod.Content.Patches
{
	[HarmonyPatch(declaringType: typeof(BulletHitbox))]
	public static class P_BulletHitbox
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		public static GameController GC => GameController.gameController;

		[HarmonyPrefix, HarmonyPatch(methodName: nameof(BulletHitbox.HasLOSBullet), argumentTypes: new [] {typeof(PlayfieldObject)})]
		public static bool HasLOSBullet_Prefix(PlayfieldObject playfieldObject, BulletHitbox __instance, ref bool __result,	ref RaycastHit2D[] ___hitsAlloc) 
		{
			// Ballistician
			// Sniper

			if (__instance.myBullet.agent != null && BMTraitController.DoesPlayerHaveTraitFromList(__instance.myBullet.agent, cTrait.BulletRange))
			{
				float maxBulletRange = BMCombat.GetBulletRange(__instance.myBullet.agent);

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

		[HarmonyPrefix, HarmonyPatch(methodName: nameof(BulletHitbox.HitObject), argumentTypes: new[] { typeof(GameObject), typeof(bool) })]
		public static bool HitObject_Prefix(GameObject hitObject, bool fromClient, BulletHitbox __instance) 
		{
			// Vanilla

			Bullet myBullet = __instance.myBullet;
			myBullet.doingFakeHit = false;
			bool bulletShotByMindSlave = false;
			int playerNumber = 0;
			Agent agent = null;
			bulletStatus bulletType = myBullet.bulletType;
			Bullet bullet = myBullet;

			if (myBullet.agent != null)
			{
				agent = myBullet.agent;

				if (agent.localPlayer || (!GC.serverPlayer && agent.mindControlAgent == GC.playerAgent))
					bulletShotByMindSlave = true;

				playerNumber = agent.isPlayer;

				if (agent.mindControlAgent != null && agent.mindControlAgent != GC.playerAgent)
					playerNumber = -1;
			}

			if (fromClient && hitObject.CompareTag("AgentSprite"))
			{
				Agent hitAgent = hitObject.GetComponent<ObjectSprite>().agent;
				__instance.HitAftermath(hitAgent, playerNumber, bulletShotByMindSlave, fromClient);
			}

			if (myBullet.fakeDestroyed)
				return false;

			if (GC.multiplayerMode && GC.serverPlayer && !bulletShotByMindSlave && playerNumber != 0 && !fromClient)
			{
				bool isSpecialBulletType = true;

				if (!hitObject.CompareTag("ObjectRealSprite") && !hitObject.CompareTag("AgentSprite") && !hitObject.CompareTag("ItemImage") &&
					!hitObject.CompareTag("BulletHitbox"))
					return false;

				if (hitObject.CompareTag("AgentSprite"))
				{
					Agent hitAgent = hitObject.GetComponent<ObjectSprite>().agent;

					if (hitAgent == agent)
						isSpecialBulletType = false;

					if (hitAgent.localPlayer && playerNumber != 0 && !bulletShotByMindSlave)
						isSpecialBulletType = false;
				}
				else if (hitObject.CompareTag("BulletHitbox"))
					isSpecialBulletType = bulletType == bulletStatus.Water || bulletType == bulletStatus.Water2 || bulletType == bulletStatus.FireExtinguisher;

				if (isSpecialBulletType)
					myBullet.doingFakeHit = true;
			}

			if (!__instance.ObjectListContains(hitObject))
			{
				__instance.objectList.Add(hitObject);

				if (bulletType != bulletStatus.GhostBlaster)
				{
					if (hitObject.CompareTag("ObjectRealSprite") &&
						bulletType != bulletStatus.LeafBlower &&
						bulletType != bulletStatus.ResearchGun)
					{
						ObjectReal objectReal;

						if (hitObject.name.Contains("ExtraSprite"))
							objectReal = hitObject.transform.parent.transform.parent.GetComponent<ObjectReal>();
						else
							objectReal = hitObject.GetComponent<ObjectSprite>().objectReal;

						bool sniperShootFromCover = false;

						if (agent.statusEffects.hasTrait(cTrait.Sniper_2) &&
							agent.hiddenInObject != null &&
							agent.hiddenInObject == objectReal)
							sniperShootFromCover = true;

						if (!GC.serverPlayer && !bulletShotByMindSlave)
							myBullet.doingFakeHit = true;

						if ((__instance.HasLOSBullet(objectReal) || objectReal.isDoor || objectReal.isWindow) &&
							objectReal.spr.GetComponent<MeshRenderer>().enabled &&
							objectReal != myBullet.objectReal)
						{
							bool bulletBreakingWindow = false;

							if (objectReal.objectName == "Window")
							{
								Window window = (Window)objectReal;

								if (!window.hitWindowOnce)
								{
									bool WindowStopsBullet = true;

									if (agent != null &&
										agent.statusEffects.hasTrait("BulletsPassThroughObjects") &&
										bulletType != bulletStatus.Rocket &&
										bulletType != bulletStatus.MindControl)
										WindowStopsBullet = false;

									if (bulletType == bulletStatus.Tranquilizer)
									{
										myBullet.BulletHitEffect(hitObject);

										if (WindowStopsBullet)
											__instance.DestroyBullet(hitObject);
									}
									else if (bulletType == bulletStatus.Taser)
									{
										myBullet.BulletHitEffect(hitObject);

										if (WindowStopsBullet)
											__instance.DestroyBullet(hitObject);
									}
									else if (bulletType == bulletStatus.Dart)
									{
										myBullet.BulletHitEffect(hitObject);

										if (WindowStopsBullet)
											__instance.DestroyBullet(hitObject);
									}
									else if (bulletType == bulletStatus.Shrink)
									{
										myBullet.BulletHitEffect(hitObject);

										if (WindowStopsBullet)
											__instance.DestroyBullet(hitObject);
									}
									else if (bulletType == bulletStatus.FreezeRay)
									{
										myBullet.BulletHitEffect(hitObject);

										if (WindowStopsBullet)
											__instance.DestroyBullet(hitObject);
									}
									else if (bulletType == bulletStatus.WaterPistol)
									{
										myBullet.BulletHitEffect(hitObject);

										if (WindowStopsBullet)
											__instance.DestroyBullet(hitObject);
									}
									else if (bulletType == bulletStatus.Water2)
									{
										myBullet.BulletHitEffect(hitObject);

										if (WindowStopsBullet)
											__instance.DestroyBullet(hitObject);
									}
									else if (bulletType == bulletStatus.FireExtinguisher)
									{
										myBullet.BulletHitEffect(hitObject);

										if (WindowStopsBullet)
											__instance.DestroyBullet(hitObject);
									}
									else if (bulletType == bulletStatus.LeafBlower)
									{
										myBullet.BulletHitEffect(hitObject);

										if (WindowStopsBullet)
											__instance.DestroyBullet(hitObject);
									}
									else if (bulletType == bulletStatus.ResearchGun)
									{
										myBullet.BulletHitEffect(hitObject);

										if (WindowStopsBullet)
											__instance.DestroyBullet(hitObject);
									}
									else if (bulletType == bulletStatus.ZombieSpit)
									{
										myBullet.BulletHitEffect(hitObject);

										if (WindowStopsBullet)
											__instance.DestroyBullet(hitObject);
									}
									else if (!myBullet.doingFakeHit)
									{
										bulletBreakingWindow = true;
										window.hitWindowOnce = true;
									}
								}
							}

							bool bulletPassingObject = true;
							bool bulletIsFire = true;

							if (objectReal.bulletsCanPass && !bulletBreakingWindow)
								bulletPassingObject = false;

							if (objectReal.bulletsCanPass && bulletType != bulletStatus.Fire)
								bulletIsFire = false;

							if (!objectReal.notRealObject &&
								!objectReal.OnFloor &&
								(bulletPassingObject || bulletIsFire) &&
								!sniperShootFromCover) // Sniper_2
							{
								if (bulletType == bulletStatus.Tranquilizer)
								{
									myBullet.BulletHitEffect(hitObject);

									if (GC.serverPlayer)
										objectReal.DamagedObject(null, 0f);
								}
								else if (bulletType == bulletStatus.Taser)
								{
									myBullet.BulletHitEffect(hitObject);

									if (GC.serverPlayer)
										objectReal.DamagedObject(null, 0f);
								}
								else if (bulletType == bulletStatus.Dart)
								{
									myBullet.BulletHitEffect(hitObject);

									if (GC.serverPlayer)
										objectReal.DamagedObject(null, 0f);
								}
								else if (bulletType == bulletStatus.Shrink)
								{
									myBullet.BulletHitEffect(hitObject);

									if (GC.serverPlayer)
										objectReal.DamagedObject(null, 0f);
								}
								else if (bulletType == bulletStatus.FreezeRay)
								{
									myBullet.BulletHitEffect(hitObject);

									if (GC.serverPlayer)
										objectReal.DamagedObject(null, 0f);
								}
								else if (bulletType == bulletStatus.WaterPistol)
								{
									myBullet.BulletHitEffect(hitObject);

									if (GC.serverPlayer)
										objectReal.DamagedObject(null, 0f);
								}
								else if (bulletType == bulletStatus.Water)
								{
									myBullet.BulletHitEffect(hitObject);

									if (GC.serverPlayer)
										objectReal.DamagedObject(null, 0f);
								}
								else if (bulletType == bulletStatus.Water2)
								{
									myBullet.BulletHitEffect(hitObject);

									if (GC.serverPlayer)
										objectReal.DamagedObject(null, 0f);
								}
								else if (bulletType == bulletStatus.FireExtinguisher)
								{
									myBullet.BulletHitEffect(hitObject);

									if (GC.serverPlayer)
										objectReal.DamagedObject(null, 0f);
								}
								else if (bulletType == bulletStatus.LeafBlower)
									myBullet.BulletHitEffect(hitObject);
								else if (bulletType == bulletStatus.ResearchGun)
									myBullet.BulletHitEffect(hitObject);
								else if (bulletType == bulletStatus.ZombieSpit)
								{
									myBullet.BulletHitEffect(hitObject);

									if (GC.serverPlayer)
										objectReal.DamagedObject(null, 0f);
								}
								else if (bulletType != bulletStatus.Fire || !objectReal.fireDoesntDamage)
								{
									if (!myBullet.doingFakeHit)
									{
										objectReal.Damage(myBullet, fromClient);

										if (myBullet.gun != null && !objectReal.noDamageNoise && myBullet.gun.invItemName != "TranquilizerGun")
											GC.spawnerMain.SpawnNoise(objectReal.FindDamageNoisePos(objectReal.tr.position), (float)objectReal.noiseHitVol,
												null, null, myBullet.agent, true).bulletMadeNoise = true;

										if (myBullet.agent != null)
										{
											if (bulletType == bulletStatus.Fire)
											{
												if (objectReal.fireProof)
													GC.OwnCheck(myBullet.agent, hitObject, "Normal", 1);
												else
													GC.OwnCheck(myBullet.agent, hitObject, "Normal", 0);
											}
											else if (bulletType == bulletStatus.Shotgun)
											{
												if (!objectReal.shotgunHitsThisTick)
												{
													objectReal.shotgunHitsThisTick = true;
													objectReal.tickEndBullet = myBullet;
												}
											}
											else
												GC.OwnCheck(myBullet.agent, hitObject, "Normal", 1);
										}

										if ((objectReal.destroying || objectReal.justDamaged) && myBullet.agent != null &&
											(playerNumber != 0 && bulletShotByMindSlave))
										{
											GC.ScreenShake(0.2f, 80f, myBullet.tr.position, myBullet.agent);

											if (objectReal.destroying && !objectReal.noDestroyEffects)
												GC.FreezeFrames(1);
										}

										if (objectReal.destroying && (bulletType == bulletStatus.Fire || bulletType == bulletStatus.Fireball) &&
											!objectReal.fireProof)
											objectReal.burnt = true;
									}

									myBullet.BulletHitEffect(hitObject);
								}
							}

							if (!objectReal.bulletsCanPass)
							{
								if (!myBullet.doingFakeHit)
								{
									bool causeFire = false;

									if (myBullet.agent != null &&
										__instance.CanCauseFire())
										causeFire = true;

									if ((bulletType == bulletStatus.Fire || bulletType == bulletStatus.Fireball || causeFire) &&
										!objectReal.fireProof &&
										!objectReal.ora.onFire)
										GC.spawnerMain.SpawnFire(myBullet, objectReal.gameObject);
								}

								if ((bulletType == bulletStatus.Normal || bulletType == bulletStatus.Shotgun || bulletType == bulletStatus.Revolver ||
										bulletType == bulletStatus.Laser) &&
									!myBullet.fakeDestroyed &&
									!myBullet.playedBulletHitEffect)
								{
									if (bulletType == bulletStatus.Laser)
										GC.spawnerMain.SpawnParticleEffect("BulletHitLaser", myBullet.tr.position, myBullet.tr.eulerAngles.z);
									else
										GC.spawnerMain.SpawnParticleEffect("BulletHit", myBullet.tr.position, myBullet.tr.eulerAngles.z);
								}

								bool bulletDestroyedHere = true;

								if (myBullet.agent != null &&
									myBullet.agent.statusEffects.hasTrait("BulletsPassThroughObjects") &&
									bulletType != bulletStatus.Rocket &&
									bulletType != bulletStatus.MindControl &&
									objectReal.damageAccumulates)
									bulletDestroyedHere = false;

								if ((bulletType == bulletStatus.Water || bulletType == bulletStatus.Water2 || bulletType == bulletStatus.FireExtinguisher) &&
									objectReal.objectName != "Door" &&
									objectReal.objectName != "Window")
									bulletDestroyedHere = false;

								// Sniper_2
								if (myBullet.agent.statusEffects.hasTrait(cTrait.Sniper_2) &&
									myBullet.agent.hiddenInObject == objectReal)
									bulletDestroyedHere = false;

								if (bulletDestroyedHere)
									__instance.DestroyBullet(hitObject);
							}

							if (!myBullet.doingFakeHit && bulletIsFire)
							{
								bool canCauseFire = false;

								if (myBullet.agent != null && __instance.CanCauseFire())
									canCauseFire = true;

								if ((bulletType == bulletStatus.Fire || bulletType == bulletStatus.Fireball || canCauseFire) && !objectReal.fireProof &&
									!objectReal.ora.onFire)
									GC.spawnerMain.SpawnFire(myBullet, objectReal.gameObject);
							}

							if (bulletType == bulletStatus.Water ||
								bulletType == bulletStatus.Water2 ||
								bulletType == bulletStatus.FireExtinguisher ||
								bulletType == bulletStatus.FreezeRay)
							{
								if (objectReal.canStartFireInObject)
									objectReal.StopFireInObject(myBullet.agent);
								else if (objectReal.fire != null)
									__instance.PutOutFire(objectReal.fire, bulletShotByMindSlave);
							}

							if (!GC.serverPlayer && bulletShotByMindSlave)
							{
								if (myBullet.agent.localPlayer)
								{
									myBullet.agent.objectMult.CallCmdBulletHitObjectReal(objectReal.objectNetID, myBullet.bulletNetID);

									return false;
								}

								GC.playerAgent.objectMult.CallCmdBulletHitObjectRealNPC(myBullet.agent.objectNetID, objectReal.objectNetID,
									myBullet.bulletNetID);

								return false;
							}
						}
					}
					else if (hitObject.CompareTag("AgentSprite"))
					{
						Agent agent3 = hitObject.GetComponent<ObjectSprite>().agent;

						if (__instance.HasLOSBullet(agent3) && (!agent3.bulletsCanPass || bulletType == bulletStatus.Fire || bulletType == bulletStatus.Water ||
							bulletType == bulletStatus.Water2 || bulletType == bulletStatus.FireExtinguisher || bulletType == bulletStatus.LeafBlower ||
							bulletType == bulletStatus.ResearchGun) && !agent3.ghost && !agent3.fellInHole)
						{
							bool flag10 = true;

							if (myBullet.agent != null && bulletType != bulletStatus.ResearchGun)
								flag10 = myBullet.agent.DontHitAlignedCheck(agent3, bulletType, myBullet.statusEffect);

							if (agent3 != myBullet.agent && flag10)
							{
								if (GC.multiplayerMode)
								{
									if (GC.serverPlayer && bulletShotByMindSlave && agent3.isPlayer > 0 && !agent3.localPlayer && agent3 != myBullet.agent)
										myBullet.doingFakeHit = true;

									if (GC.serverPlayer && playerNumber == 0 && agent3.isPlayer > 0 && !agent3.localPlayer && agent3 != myBullet.agent)
										myBullet.doingFakeHit = true;

									if (myBullet.agent != null)
									{
										if (!GC.serverPlayer && playerNumber == 0 && !agent3.localPlayer && myBullet.agent.mindControlAgent != GC.playerAgent &&
											agent3 != myBullet.agent)
											myBullet.doingFakeHit = true;

										if (!GC.serverPlayer && playerNumber != 0 && !bulletShotByMindSlave && !agent3.localPlayer &&
											myBullet.agent.mindControlAgent != GC.playerAgent && agent3 != myBullet.agent)
											myBullet.doingFakeHit = true;

										if (!GC.serverPlayer && bulletShotByMindSlave && agent3.isPlayer > 0 && !agent3.localPlayer &&
											myBullet.agent.mindControlAgent != GC.playerAgent && agent3 != myBullet.agent)
											myBullet.doingFakeHit = true;

										if (!GC.serverPlayer && playerNumber != 0 && !bulletShotByMindSlave && agent3.isPlayer != 0 && !agent3.localPlayer &&
											myBullet.agent.mindControlAgent != GC.playerAgent && agent3 != myBullet.agent)
											myBullet.doingFakeHit = true;
									}
								}

								if ((bulletType == bulletStatus.Rocket || bulletType == bulletStatus.MindControl) && !myBullet.doingFakeHit)
								{
									agent3.hitByRocket = new Vector3(myBullet.tr.eulerAngles.x, myBullet.tr.eulerAngles.y, myBullet.tr.eulerAngles.z);

									if (GC.serverPlayer && agent3.localPlayer)
										myBullet.mustExplode = true;
									else if (!GC.serverPlayer && agent3.localPlayer && !bulletShotByMindSlave)
									{
										myBullet.mustExplode = true;
										myBullet.explosionFromNonLocalPlayer = true;
									}
									else if (GC.serverPlayer && !agent3.localPlayer && agent3.isPlayer != 0 && !bulletShotByMindSlave && playerNumber != 0)
										myBullet.mustExplode = true;
								}

								bool flag11 = false;

								if ((bulletType == bulletStatus.Fire || bulletType == bulletStatus.Water || bulletType == bulletStatus.Water2 ||
									bulletType == bulletStatus.FireExtinguisher || bulletType == bulletStatus.Fireball) && agent3.dead)
								{
									if (!myBullet.doingFakeHit && !fromClient)
										__instance.HitAftermath(agent3, playerNumber, bulletShotByMindSlave, fromClient);
								}
								else if (bulletType == bulletStatus.Tranquilizer)
								{
									if (!myBullet.doingFakeHit && !fromClient)
										__instance.HitAftermath(agent3, playerNumber, bulletShotByMindSlave, fromClient);

									if (!myBullet.fakeDestroyed)
										GC.audioHandler.Play(agent3, "TranquilizerHitAgent");
								}
								else if (bulletType == bulletStatus.Taser)
								{
									if (!myBullet.doingFakeHit && !fromClient)
										__instance.HitAftermath(agent3, playerNumber, bulletShotByMindSlave, fromClient);

									if (!myBullet.fakeDestroyed)
										GC.audioHandler.Play(agent3, "TaserHitAgent");
								}
								else if (bulletType == bulletStatus.Dart)
								{
									if (!myBullet.doingFakeHit && !fromClient)
										__instance.HitAftermath(agent3, playerNumber, bulletShotByMindSlave, fromClient);

									if (!myBullet.fakeDestroyed)
										GC.audioHandler.Play(agent3, "TranquilizerHitAgent");
								}
								else if (bulletType == bulletStatus.Shrink)
								{
									if (!myBullet.doingFakeHit && !fromClient)
										__instance.HitAftermath(agent3, playerNumber, bulletShotByMindSlave, fromClient);

									if (!myBullet.fakeDestroyed)
										GC.audioHandler.Play(agent3, "BulletHitAgent");
								}
								else if (bulletType == bulletStatus.FreezeRay)
								{
									if (!myBullet.doingFakeHit && !fromClient)
										__instance.HitAftermath(agent3, playerNumber, bulletShotByMindSlave, fromClient);

									if (!myBullet.fakeDestroyed)
									{
										if (agent3.preventStatusEffects)
											GC.audioHandler.Play(agent3, "BulletHitAgent");
										else
											GC.audioHandler.Play(agent3, "Freeze");
									}
								}
								else if (bulletType == bulletStatus.WaterPistol)
								{
									if (!myBullet.doingFakeHit && !fromClient)
										__instance.HitAftermath(agent3, playerNumber, bulletShotByMindSlave, fromClient);

									if (!myBullet.fakeDestroyed)
										GC.audioHandler.Play(agent3, "WaterPistolHitAgent");
								}
								else if (bulletType == bulletStatus.LeafBlower)
								{
									if (!myBullet.doingFakeHit && !fromClient)
										__instance.HitAftermath(agent3, playerNumber, bulletShotByMindSlave, fromClient);
								}
								else if (bulletType == bulletStatus.ResearchGun)
								{
									if (!myBullet.doingFakeHit && !fromClient)
										__instance.HitAftermath(agent3, playerNumber, bulletShotByMindSlave, fromClient);
								}
								else if (bulletType == bulletStatus.Water2)
								{
									if (!myBullet.doingFakeHit && !fromClient)
										__instance.HitAftermath(agent3, playerNumber, bulletShotByMindSlave, fromClient);
								}
								else if (bulletType == bulletStatus.FireExtinguisher)
								{
									if (!myBullet.doingFakeHit && !fromClient)
										__instance.HitAftermath(agent3, playerNumber, bulletShotByMindSlave, fromClient);
								}
								else if (bulletType == bulletStatus.ZombieSpit)
								{
									if (!myBullet.doingFakeHit && !fromClient)
										__instance.HitAftermath(agent3, playerNumber, bulletShotByMindSlave, fromClient);

									if (!myBullet.fakeDestroyed)
										GC.audioHandler.Play(agent3, "BulletHitAgent");
								}
								else if (bulletType == bulletStatus.Normal || bulletType == bulletStatus.Shotgun || bulletType == bulletStatus.Fire ||
									bulletType == bulletStatus.Water || bulletType == bulletStatus.Water2 || bulletType == bulletStatus.FireExtinguisher ||
									bulletType == bulletStatus.Fireball || bulletType == bulletStatus.Revolver || bulletType == bulletStatus.Laser)
								{
									if (!myBullet.doingFakeHit && !fromClient)
									{
										if (agent3.statusEffects.hasTrait("MeleeHoldDeflectsBullets") &&
											agent3.inventory.equippedWeapon != agent3.inventory.fist &&
											agent3.inventory.equippedWeapon.weaponCode == weaponType.WeaponMelee)
										{
											agent3.inventory.DepleteMelee(5);

											if (agent3.movement.HasLOSPositionNormal(myBullet.tr.position))
											{
												flag11 = true;

												if (bulletType == bulletStatus.Laser)
													GC.spawnerMain.SpawnParticleEffect("BulletHitLaser", agent3.tr.position, myBullet.tr.eulerAngles.z);
												else
													GC.spawnerMain.SpawnParticleEffect("BulletHit", agent3.tr.position, myBullet.tr.eulerAngles.z);

												if (agent3.inventory.equippedWeapon.hitSoundType == "Cut")
													GC.audioHandler.Play(agent3, "MeleeHitMeleeBlade");
												else
													GC.audioHandler.Play(agent3, "MeleeHitMelee");
											}
										}

										if (!flag11)
											__instance.HitAftermath(agent3, playerNumber, bulletShotByMindSlave, fromClient);
									}

									if (!myBullet.fakeDestroyed && !flag11)
									{
										if (bulletType == bulletStatus.Fire)
											GC.audioHandler.Play(agent3, "FireHitShort");
										else if (bulletType == bulletStatus.Water || bulletType == bulletStatus.Water2)
											GC.audioHandler.Play(agent3, "WaterHit");
										else if (bulletType == bulletStatus.FireExtinguisher)
											GC.audioHandler.Play(agent3, "FireHitShort");
										else if (bulletType == bulletStatus.Fireball)
											GC.audioHandler.Play(agent3, "FireballHitAgent");
										else
											GC.audioHandler.Play(agent3, "BulletHitAgent");
									}
								}

								if ((bulletType == bulletStatus.Normal || bulletType == bulletStatus.Shotgun || bulletType == bulletStatus.Revolver ||
									bulletType == bulletStatus.Laser) && agent3.tickEndDamage == 0 && !myBullet.fakeDestroyed && !flag11)
								{
									if (agent3.inhuman || agent3.mechFilled || agent3.mechEmpty)
										GC.spawnerMain.SpawnParticleEffect("BloodHitYellow", agent3.tr.position, myBullet.tr.eulerAngles.z);
									else
										GC.spawnerMain.SpawnParticleEffect("BloodHit", agent3.tr.position, myBullet.tr.eulerAngles.z);
								}

								if (bulletType != bulletStatus.Fire &&
									(!agent3.dead || bulletType != bulletStatus.FireExtinguisher && bulletType != bulletStatus.Water2) &&
									bulletType != bulletStatus.ResearchGun)
								{
									bool flag12 = true;

									if (myBullet.agent != null && myBullet.agent.statusEffects.hasTrait("BulletsPassThroughObjects") &&
										bulletType != bulletStatus.Rocket && bulletType != bulletStatus.MindControl)
										flag12 = false;

									if (flag12)
										__instance.DestroyBullet(hitObject);
								}

								if (!GC.serverPlayer && bulletShotByMindSlave && !myBullet.doingFakeHit && !flag11)
								{
									if (bulletType != bulletStatus.ResearchGun)
										myBullet.agent.objectMultPlayfield.TempDisableNetworkTransform(agent3);

									myBullet.dirHelper.localPosition = Vector3.zero;
									myBullet.dirHelper.localPosition = new Vector3(0f, 10f, 0f);
									Vector3 position = myBullet.dirHelper.position;
									myBullet.dirHelper.localPosition = Vector3.zero;
									int num2 = 30;

									if (agent3.dead && bulletType == bulletStatus.Fire)
										num2 = 0;
									else if (bulletType != bulletStatus.Normal && bulletType != bulletStatus.Shotgun && bulletType != bulletStatus.Fire &&
										bulletType != bulletStatus.Fireball && bulletType != bulletStatus.Revolver && bulletType != bulletStatus.Laser)
										num2 = 0;
									else if (agent3.statusEffects.hasTrait("KnockbackLess2"))
										num2 = 0;
									else if (bulletType == bulletStatus.Shotgun)
										num2 = 0;
									else if (bulletType == bulletStatus.ResearchGun)
										num2 = 0;
									else if (agent3.justDied)
										num2 = Mathf.Abs(agent3.lastDamageVal) * 30;

									if (myBullet.agent.statusEffects.hasTrait("CauseBiggerKnockback"))
										num2 *= 2;

									if (myBullet.agent.localPlayer)
									{
										myBullet.agent.objectMult.CallCmdBulletHitAgent(agent3.objectNetID, myBullet.bulletNetID, position, num2,
											agent3.tr.position, agent3.rb.velocity);

										return false;
									}

									GC.playerAgent.objectMult.CallCmdBulletHitAgentNPC(myBullet.agent.objectNetID, agent3.objectNetID, myBullet.bulletNetID,
										position, num2, agent3.tr.position, agent3.rb.velocity);

									return false;
								}
							}
						}
					}
					else if (hitObject.CompareTag("ItemImage"))
					{
						Item item = hitObject.GetComponent<ItemHitbox>().item;

						if (bulletType == bulletStatus.LeafBlower ||
							((bulletType == bulletStatus.Fire || bulletType == bulletStatus.Fireball || bulletType == bulletStatus.ResearchGun) &&
								item.invItem.canCatchFire))
						{
							if (!GC.serverPlayer && !bulletShotByMindSlave)
								myBullet.doingFakeHit = true;

							if (!myBullet.doingFakeHit)
							{
								if (bulletType == bulletStatus.LeafBlower)
								{
									if (item.startingOwner != 0)
										GC.OwnCheck(myBullet.agent, item.go, "Normal", 1);

									item.thrower = myBullet.agent;
									item.StartCoroutine(item.HitCauserCoroutine(myBullet.agent));

									if (__instance.HasLOSBullet(item))
										item.movement.KnockBackBullet(myBullet.go, 200f, false, myBullet);
								}
								else if ((bulletType == bulletStatus.Fire || bulletType == bulletStatus.Fireball) && item.invItem.canCatchFire)
								{
									if (item.startingOwner != 0)
										GC.OwnCheck(myBullet.agent, item.go, "Normal", 1);

									if (!item.burnt)
										GC.spawnerMain.SpawnFire(myBullet, item.go, item.tr.position, true);
								}
							}

							if (!GC.serverPlayer && bulletShotByMindSlave)
							{
								if (myBullet.agent.localPlayer)
								{
									myBullet.agent.objectMult.CallCmdBulletHitItem(item.objectNetID, myBullet.bulletNetID);

									return false;
								}

								GC.playerAgent.objectMult.CallCmdBulletHitItemNPC(myBullet.agent.objectNetID, item.objectNetID, myBullet.bulletNetID);

								return false;
							}
						}
					}
					else if (hitObject.CompareTag("Fire"))
					{
						if (bulletType == bulletStatus.Water || bulletType == bulletStatus.Water2 || bulletType == bulletStatus.FireExtinguisher ||
							bulletType == bulletStatus.FreezeRay)
						{
							__instance.PutOutFire(hitObject.GetComponent<Fire>(), bulletShotByMindSlave);

							return false;
						}
					}
					else if (hitObject.CompareTag("MeleeHitbox"))
					{
						if (hitObject.GetComponent<MeleeColliderBox>().meleeHitbox.myMelee.agent.statusEffects.hasTrait("MeleeDestroysBullets"))
						{
							myBullet.BulletHitEffect(hitObject);
							__instance.DestroyBullet(hitObject);

							return false;
						}
					}
					else if (hitObject.CompareTag("BulletHitbox"))
					{
						Bullet collidingBullet = hitObject.GetComponent<BulletHitbox>().myBullet;

						if (!myBullet.streamBullet)
						{
							if (myBullet.agent != null && myBullet.agent.statusEffects.hasTrait("BulletsDestroyOtherBullets") &&
								myBullet.agent != collidingBullet.agent && !collidingBullet.streamBullet)
							{
								bool flag13 = false;

								if (collidingBullet.agent != null && collidingBullet.agent.statusEffects.hasTrait("BulletsDestroyOtherBullets"))
									flag13 = true;

								if (bulletType != bulletStatus.Rocket || flag13)
								{
									myBullet.BulletHitEffect(hitObject);
									__instance.DestroyBullet(hitObject);
								}

								collidingBullet.BulletHitEffect(__instance.gameObject);
								collidingBullet.bulletHitboxScript.DestroyBullet(__instance.gameObject);
							}
						}
						else if (bulletType == bulletStatus.LeafBlower)
						{
							if (!collidingBullet.alteredSpeed && myBullet.agent != collidingBullet.agent && !collidingBullet.streamBullet)
							{
								collidingBullet.speed /= 2;
								collidingBullet.alteredSpeed = true;
							}
						}
						else if (bulletType == bulletStatus.Water2 &&
							(myBullet.agent.statusEffects.hasTrait("StrongerWaterCannon") || myBullet.agent.oma.superSpecialAbility ||
								collidingBullet.bulletType == bulletStatus.Fire || collidingBullet.bulletType == bulletStatus.Fireball) &&
							myBullet.agent != collidingBullet.agent)
						{
							collidingBullet.BulletHitEffect(__instance.gameObject);
							collidingBullet.bulletHitboxScript.DestroyBullet(__instance.gameObject);
						}

						if (collidingBullet.bulletType == bulletStatus.Water2 &&
							(collidingBullet.agent.statusEffects.hasTrait("StrongerWaterCannon") || collidingBullet.agent.oma.superSpecialAbility ||
								bulletType == bulletStatus.Fire || bulletType == bulletStatus.Fireball) && collidingBullet.agent != myBullet.agent)
						{
							myBullet.BulletHitEffect(__instance.gameObject);
							myBullet.bulletHitboxScript.DestroyBullet(__instance.gameObject);
						}

						if ((bulletType == bulletStatus.Water || bulletType == bulletStatus.FireExtinguisher) &&
							(collidingBullet.bulletType == bulletStatus.Fire || collidingBullet.bulletType == bulletStatus.Fireball))
						{
							collidingBullet.BulletHitEffect(__instance.gameObject);
							collidingBullet.bulletHitboxScript.DestroyBullet(__instance.gameObject);
						}

						if ((collidingBullet.bulletType == bulletStatus.Water || collidingBullet.bulletType == bulletStatus.FireExtinguisher) &&
							(bulletType == bulletStatus.Fire || bulletType == bulletStatus.Fireball))
						{
							myBullet.BulletHitEffect(__instance.gameObject);
							myBullet.bulletHitboxScript.DestroyBullet(__instance.gameObject);
						}

						if (collidingBullet.bulletType == bulletStatus.LeafBlower && !myBullet.alteredSpeed && myBullet.agent != collidingBullet.agent &&
							!collidingBullet.streamBullet)
						{
							myBullet.speed /= 2;
							myBullet.alteredSpeed = true;
						}
					}
				}

				return false;
			}

			return false;
		}

		[HarmonyPrefix, HarmonyPatch(methodName: "OnTriggerEnter2D", argumentTypes: new[] { typeof(Collider2D) })]
		public static bool OnTriggerEnter2D_Prefix(Collider2D other, BulletHitbox __instance) 
		{
			// Sniper +

			logger.LogDebug("BulletHitbox_OnTriggerEnter2D");

			if (other.CompareTag("ObjectRealSprite") && __instance.myBullet.agent != null &&
				__instance.myBullet.agent.statusEffects.hasTrait(cTrait.Sniper_2) && __instance.myBullet.agent.hiddenInObject != null)
			{
				try
				{
					logger.LogDebug("\tname: " + other.name);
				}
				catch { }
				try
				{
					logger.LogDebug("\thiddenIn: " + __instance.myBullet.agent.hiddenInObject.name);
				}
				catch { }

				logger.LogDebug("\tObject detected");
				ObjectReal obj = other.GetComponent<ObjectReal>();

				if (__instance.myBullet.agent.hiddenInObject == obj)
				{
					logger.LogDebug("\tObject bypassed");

					return false;
				}
			}

			return true;
		}

	}
}
