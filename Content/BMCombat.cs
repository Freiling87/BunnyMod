using RogueLibsCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
			PlayfieldObject_00();
			SpawnerMain_00();
		}
		public void Initialize_Names()
		{
			string t = vNameType.Interface;

			CustomName headshot = RogueLibs.CreateCustomName("Headshot", t, new CustomNameInfo("Headshot"));
		}

		#region Bullet
		public void Bullet_00()
		{
			Type t = typeof(Bullet);
			Type g = GetType();

			Prefix(t, "FindParticleStoppingPoint", g, "Bullet_FindParticleStoppingPoint", new Type[0] { });
			Prefix(t, "FindStoppingPoint", g, "Bullet_FindStoppingPoint", new Type[0] { });
			Postfix(t, "SetupBullet", g, "Bullet_SetupBullet", new Type[0] { });
		}
		public static bool Bullet_FindParticleStoppingPoint(Bullet __instance, ref Vector2 __result, RaycastHit2D[] ___hitsAlloc) // Replacement
		{
			Vector2 vector = Vector2.zero;

			if (__instance.agent == null)
			{
				if (__instance.objectReal != null)
				{
					switch(__instance.objectReal.direction)
					{
						case "N":
							__instance.tr.rotation = Quaternion.Euler(0f, 0f, 0f);
							break;
						case "NE":
							__instance.tr.rotation = Quaternion.Euler(0f, 0f, 315f);
							break;
						case "E":
							__instance.tr.rotation = Quaternion.Euler(0f, 0f, 270f);
							break;
						case "SE":
							__instance.tr.rotation = Quaternion.Euler(0f, 0f, 225f);
							break;
						case "S":
							__instance.tr.rotation = Quaternion.Euler(0f, 0f, 180f);
							break;
						case "SW":
							__instance.tr.rotation = Quaternion.Euler(0f, 0f, 135f);
							break;
						case "W":
							__instance.tr.rotation = Quaternion.Euler(0f, 0f, 90f);
							break;
						case "NW":
							__instance.tr.rotation = Quaternion.Euler(0f, 0f, 45f);
							break;
					}

					__instance.dirHelper.localPosition = new Vector3(0f, 0.32f, 0f);
					vector = __instance.dirHelper.position - __instance.tr.position;
					__instance.tr.rotation = Quaternion.Euler(0f, 0f, 0f);
				}
			}
			else if ((!__instance.agent.localPlayer || __instance.agent.outOfControl) && !__instance.agent.objectAgent)
			{
				if (__instance.agent.outOfControl && __instance.agent.isPlayer != 0)
					vector = __instance.agent.opponent.tr.position - __instance.agent.tr.position;
				else if (__instance.agent.isPlayer != 0)
					vector = __instance.agent.gun.holdAttackAimPoint - __instance.agent.tr.position;
				else if (__instance.agent.oma.mindControlled)
				{
					__instance.movement.RotateToAngleTransform(__instance.agent.tr.eulerAngles.z - 90f);
					__instance.dirHelper.localPosition = new Vector3(0f, 0.32f, 0f);
					vector = __instance.dirHelper.position - __instance.agent.tr.position;
				}
				else if (GC.serverPlayer)
					vector = __instance.agent.opponent.tr.position - __instance.agent.tr.position;
				else
					vector = __instance.agent.gun.holdAttackAimPoint - __instance.agent.tr.position;
			}
			else if (__instance.agent.controllerType == "Keyboard")
			{
				if (GC.sessionDataBig.trackpadMode)
				{
					__instance.movement.RotateToAngleTransform(__instance.agent.tr.eulerAngles.z - 90f);
					__instance.dirHelper.localPosition = new Vector3(0f, 0.32f, 0f);
					vector = __instance.dirHelper.position - __instance.agent.tr.position;
				}
				else
					vector = __instance.agent.agentCamera.actualCamera.ScreenCamera.ScreenToWorldPoint(Input.mousePosition) - __instance.agent.tr.position;
			}
			else
			{
				if (__instance.agent.isPlayer > 0 && !__instance.agent.outOfControl && __instance.agent.localPlayer)
				{
					if (__instance.agent.target.AttackTowardTarget())
						vector = __instance.agent.target.tr.position - __instance.agent.tr.position;
					else
					{
						__instance.tr.rotation = Quaternion.Euler(0f, 0f, __instance.agent.gun.FindWeaponAngleGamepad() - 90f);
						__instance.dirHelper.localPosition = new Vector3(0f, 0.32f, 0f);
						vector = __instance.dirHelper.position - __instance.agent.tr.position;
					}
				}
				else if ((__instance.agent.isPlayer <= 0 || __instance.agent.outOfControl || __instance.agent.localPlayer) && !GC.serverPlayer)
					_ = __instance.agent.localPlayer;

				if (__instance.agent.objectAgent)
				{
					switch (__instance.agent.assignedObjectReal.direction)
					{
						case "N":
							__instance.tr.rotation = Quaternion.Euler(0f, 0f, 0f);
							break;
						case "NE":
							__instance.tr.rotation = Quaternion.Euler(0f, 0f, 315f);
							break;
						case "E":
							__instance.tr.rotation = Quaternion.Euler(0f, 0f, 270f);
							break;
						case "SE":
							__instance.tr.rotation = Quaternion.Euler(0f, 0f, 225f);
							break;
						case "S":
							__instance.tr.rotation = Quaternion.Euler(0f, 0f, 180f);
							break;
						case "SW":
							__instance.tr.rotation = Quaternion.Euler(0f, 0f, 135f);
							break;
						case "W":
							__instance.tr.rotation = Quaternion.Euler(0f, 0f, 90f);
							break;
						case "NW":
							__instance.tr.rotation = Quaternion.Euler(0f, 0f, 45f);
							break;
					}
					
					__instance.dirHelper.localPosition = new Vector3(0f, 0.32f, 0f);
					vector = __instance.dirHelper.position - __instance.tr.position;
					__instance.tr.rotation = Quaternion.Euler(0f, 0f, 0f);
				}
			}

			int num2 = 0;
			Vector2 vector2 = Vector2.zero;
			
			if (__instance.agent != null)
			{
				vector2 = __instance.agent.tr.position;
				num2 = Physics2D.RaycastNonAlloc(vector2, vector.normalized, ___hitsAlloc, 13.44f, __instance.myLayerMask);
			}
			else if (__instance.objectReal != null)
			{
				vector2 = __instance.objectReal.tr.position;
				num2 = Physics2D.RaycastNonAlloc(vector2, vector.normalized, ___hitsAlloc, 13.44f, __instance.myLayerMask);
			}
			
			float num3 = 13.44f;

			BMLog("\tTrait Check");

			if (__instance.agent.statusEffects.hasTrait(cTrait.Sniper))
				num3 = 20.00f;
			else if (__instance.agent.statusEffects.hasTrait(cTrait.Sniper_2))
				num3 = 50.00f;

			BMLog("\tnum3 = " + num3);

			Vector2 ___result = Vector2.zero;
			__instance.stoppingPointObject = null;
			Window window = null;
			
			for (int i = 0; i < num2; i++)
			{
				RaycastHit2D raycastHit2D = ___hitsAlloc[i];
			
				if (raycastHit2D.collider.CompareTag("Wall"))
				{
					if (Vector2.Distance(vector2, raycastHit2D.collider.transform.position) < num3)
					{
						num3 = raycastHit2D.distance;
						___result = raycastHit2D.collider.transform.position;
						__instance.stoppingPointObject = null;
					}
				}
				else if (raycastHit2D.collider.CompareTag("ObjectRealSprite"))
				{
					bool flag = true;
					ObjectReal objectReal = null;

					if (raycastHit2D.collider.name.Contains("ExtraSprite"))
						__instance.objectReal = raycastHit2D.collider.transform.parent.transform.parent.GetComponent<ObjectReal>();
					else
						objectReal = raycastHit2D.collider.transform.parent.GetComponent<ObjectReal>();
					
					if (__instance.objectReal == objectReal)
						flag = false;
					
					if (__instance.bulletType == bulletStatus.FireExtinguisher || __instance.bulletType == bulletStatus.Water || __instance.bulletType == bulletStatus.Water2)
						flag = false;
					
					if (objectReal.bulletsCanPass)
					{
						flag = false;

						if (objectReal.objectName == "Window")
						{
							if (__instance.bulletType == bulletStatus.LeafBlower || __instance.bulletType == bulletStatus.FireExtinguisher || __instance.bulletType == bulletStatus.ResearchGun || __instance.bulletType == bulletStatus.Water || __instance.bulletType == bulletStatus.Water2)
								flag = true;
							else
							{
								Window window2 = (Window)objectReal;
						
								if (!window2.hitWindowOnce)
								{
									window = window2;
									flag = true;
								}
							}
						}
					}

					if (flag && Vector2.Distance(vector2, raycastHit2D.collider.transform.position) < num3)
					{
						num3 = raycastHit2D.distance;
						___result = raycastHit2D.collider.transform.position;
						__instance.stoppingPointObject = objectReal;
					}
				}
			}

			if (window == __instance.stoppingPointObject && window != null)
			{
				window.hitWindowOnce = true;
				__instance.RayHit(window);
			}
			
			__instance.stoppingPointDistance = num3;

			if (___result == Vector2.zero)
				___result = -Vector2.one;

			return false;
		}
		public static bool Bullet_FindStoppingPoint(Bullet __instance, ref Vector2 __result, RaycastHit2D[] ___hitsAlloc) // Replacement
		{
			BMLog("Bullet_FindStoppingPoint");

			Vector2 vector = Vector2.zero;

			if ((!__instance.agent.localPlayer || __instance.agent.outOfControl) && !__instance.agent.objectAgent)
			{
				if (__instance.agent.outOfControl && __instance.agent.isPlayer != 0)
					vector = __instance.agent.opponent.tr.position - __instance.agent.tr.position;
				else if (__instance.agent.isPlayer != 0)
					vector = __instance.agent.gun.holdAttackAimPoint - __instance.agent.tr.position;
				else if (__instance.agent.oma.mindControlled)
				{
					__instance.movement.RotateToAngleTransform(__instance.agent.tr.eulerAngles.z - 90f);
					__instance.dirHelper.localPosition = new Vector3(0f, 0.32f, 0f);
					vector = __instance.dirHelper.position - __instance.agent.tr.position;
				}
				else if (GC.serverPlayer)
					vector = __instance.agent.opponent.tr.position - __instance.agent.tr.position;
				else
					vector = __instance.agent.melee.attackObject.tr.position - __instance.agent.tr.position;
			}
			else if (__instance.agent.controllerType == "Keyboard")
			{
				if (GC.sessionDataBig.trackpadMode)
				{
					__instance.movement.RotateToAngleTransform(__instance.agent.tr.eulerAngles.z - 90f);
					__instance.dirHelper.localPosition = new Vector3(0f, 0.32f, 0f);
					vector = __instance.dirHelper.position - __instance.agent.tr.position;
				}
				else
					vector = __instance.agent.agentCamera.actualCamera.ScreenCamera.ScreenToWorldPoint(Input.mousePosition) - __instance.agent.tr.position;
			}
			else if (__instance.agent.target.AttackTowardTarget())
				vector = __instance.agent.target.tr.position - __instance.agent.tr.position;
			else
			{
				__instance.tr.rotation = Quaternion.Euler(0f, 0f, __instance.agent.gun.FindWeaponAngleGamepad() - 90f);
				__instance.dirHelper.localPosition = new Vector3(0f, 0.32f, 0f);
				vector = __instance.dirHelper.position - __instance.agent.tr.position;
			}
			
			int num = Physics2D.RaycastNonAlloc(__instance.agent.tr.position, vector.normalized, ___hitsAlloc, 13.44f, __instance.myLayerMask);

			float num2 = 13.44f;

			BMLog("\tTrait Check");

			if (__instance.agent.statusEffects.hasTrait(cTrait.Sniper))
				num2 = 20.00f;
			else if (__instance.agent.statusEffects.hasTrait(cTrait.Sniper_2))
				num2 = 50.00f;

			BMLog("\tnum2 = " + num2);

			__result = Vector2.zero;
			__instance.stoppingPointObject = null;
			Window window = null;

			for (int i = 0; i < num; i++)
			{
				RaycastHit2D raycastHit2D = ___hitsAlloc[i];
				bool flag = true;

				if (GC.serverPlayer && !__instance.agent.localPlayer && __instance.agent.isPlayer != 0)
					flag = false;

				if (raycastHit2D.collider.CompareTag("Wall"))
				{
					if (Vector2.Distance(__instance.agent.tr.position, raycastHit2D.collider.transform.position) < num2)
					{
						num2 = raycastHit2D.distance;
						__result = raycastHit2D.collider.transform.position;
						__instance.stoppingPointObject = null;
					}
				}
				else if (raycastHit2D.collider.CompareTag("ObjectRealSprite"))
				{
					bool flag2 = true;
					ObjectReal objectReal = null;

					if (raycastHit2D.collider.name.Contains("ExtraSprite"))
						__instance.objectReal = raycastHit2D.collider.transform.parent.transform.parent.GetComponent<ObjectReal>();
					else
						objectReal = raycastHit2D.collider.transform.parent.GetComponent<ObjectReal>();
					
					if (!GC.serverPlayer && !__instance.agent.localPlayer)
						flag = false;
					
					if (objectReal.bulletsCanPass)
					{
						flag2 = false;
					
						if (objectReal.objectName == "Window")
						{
							if (__instance.bulletType == bulletStatus.GhostBlaster)
							{
								if (!((Window)objectReal).hitWindowOnce)
									flag2 = true;
							}
							else
							{
								Window window2 = (Window)objectReal;
						
								if (!window2.hitWindowOnce)
								{
									window = window2;
									flag2 = true;
								}
							}
						}
					}

					if (flag2 && Vector2.Distance(__instance.agent.tr.position, raycastHit2D.collider.transform.position) < num2)
					{
						num2 = raycastHit2D.distance;
						__result = raycastHit2D.collider.transform.position;
						__instance.stoppingPointObject = objectReal;
					}
				}
				else if (raycastHit2D.collider.CompareTag("AgentSprite") && raycastHit2D.collider.gameObject != __instance.agent.agentSpriteTransform.gameObject)
				{
					GameObject go = raycastHit2D.collider.GetComponent<AgentColliderBox>().objectSprite.go;
					Agent agent = go.GetComponent<ObjectSprite>().agent;

					if (GC.multiplayerMode)
					{
						if (GC.serverPlayer && __instance.agent.localPlayer && agent.isPlayer > 0 && !agent.localPlayer && agent != __instance.agent)
							flag = false;
					
						if (GC.serverPlayer && __instance.agent.isPlayer == 0 && agent.isPlayer > 0 && !agent.localPlayer && agent != __instance.agent)
							flag = false;
						
						if (!GC.serverPlayer && __instance.agent.isPlayer == 0 && !agent.localPlayer && agent != __instance.agent)
							flag = false;
						
						if (!GC.serverPlayer && __instance.agent.isPlayer > 0 && !__instance.agent.localPlayer && !agent.localPlayer && agent != __instance.agent)
							flag = false;
						
						if (!GC.serverPlayer && __instance.agent.localPlayer && agent.isPlayer > 0 && !agent.localPlayer && agent != __instance.agent)
							flag = false;
						
						if (!GC.serverPlayer && __instance.agent.isPlayer != 0 && !__instance.agent.localPlayer && agent.isPlayer != 0 && !agent.localPlayer && agent != __instance.agent)
							flag = false;
					}

					if ((!agent.bulletsCanPass || agent.ghost) && agent != __instance.agent && agent.DontHitAlignedCheck(__instance.agent) && Vector2.Distance(__instance.agent.tr.position, go.transform.position) < num2)
					{
						num2 = raycastHit2D.distance;
						__result = go.transform.position;
						__instance.stoppingPointObject = go.GetComponent<ObjectSprite>().playFieldObject;
					}
				}

				if (!flag)
					__instance.stoppingPointObject = null;
			}

			if (window == __instance.stoppingPointObject && window != null)
			{
				window.hitWindowOnce = true;
				__instance.RayHit(window);
			}
			
			__instance.stoppingPointDistance = num2;
			
			return false;
		}
		public static void Bullet_SetupBullet(Bullet __instance) // Postfix
		{
			if (GC.challenges.Contains(cChallenge.ScaryGuns))
			{
				__instance.damage = Mathf.Max(1, (int)(__instance.damage * UnityEngine.Random.Range(0.25f, 5f)));
				__instance.speed = Mathf.Min(65, __instance.speed * 3);
			}
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

			bool playerDamagedByNpc = false;
			
			if (flag2)
			{
				if (damagerAgent.isPlayer != 0 && !damagerAgent.localPlayer)
					playerDamagedByNpc = true;
			
				if (instanceIsAgent && damagedAgent.isPlayer != 0 && damagerAgent.isPlayer != 0 && !GC.pvp)
					flag6 = false;
			}
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
						if (damagedAgent.statusEffects.hasTrait("ResistBullets"))
							dmg /= 1.5f;

						if (damagedAgent.statusEffects.hasTrait("ResistBulletsSmall"))
							dmg /= 1.2f;

						if (damagedAgent.statusEffects.hasTrait("ResistBulletsTrait2"))
							dmg /= 2f;
						else if (damagedAgent.statusEffects.hasTrait("ResistBulletsTrait"))
							dmg /= 1.5f;

						bool headShot = false;

						if (damagerAgent.statusEffects.hasTrait(cTrait.DoubleTapper) || damagerAgent.statusEffects.hasTrait(cTrait.DoubleTapper_2) &&
							!damagedAgent.movement.HasLOSObjectBehind(damagerAgent) && 
							!damagedAgent.movement.HasLOSAgent(damagerAgent) &&
							Vector2.Distance(damagerAgent.tr.position, damagedAgent.tr.position) <= 0.64f)
							headShot = true;

						if (damagerAgent.statusEffects.hasTrait(cTrait.Sniper) || damagerAgent.statusEffects.hasTrait(cTrait.Sniper_2) &&
							(!damagedAgent.movement.HasLOSAgent(damagerAgent) || damagerAgent.hiddenInObject || damagerAgent.invisible)&&
							Vector2.Distance(damagerAgent.tr.position, damagedAgent.tr.position) >= 5.12f)
							headShot = true;

						if (headShot)
						{
							if (damagedAgent.sleeping)
							{
								dmg = 200f;
								damagedAgent.agentHitboxScript.wholeBodyMode = 0;
								damagerAgent.melee.successfullySleepKilled = true;

								damagedAgent.statusEffects.CreateBuffText(cBuffText.Headshot, damagedAgent.objectNetID);
							}
							else if (
								(dmg != 200f && !damagedAgent.dead) ||
								(((damagedAgent.mostRecentGoalCode != goalType.Battle && damagedAgent.mostRecentGoalCode != goalType.Flee) || damagedAgent.frozen) &&
								!damagedAgent.movement.HasLOSObjectBehind(damagerAgent) &&
								!damagedAgent.sleeping &&
								dmg != 200f &&
								!damagedAgent.dead))
							{
								damagedAgent.agentHelperTr.localPosition = new Vector3(-0.64f, 0f, 0f);

								if (!GC.tileInfo.IsOverlapping(damagedAgent.agentHelperTr.position, "Wall"))
								{
									damagedAgent.agentHelperTr.localPosition = Vector3.zero;
									damagedAgent.statusEffects.CreateBuffText(cBuffText.Headshot, damagedAgent.objectNetID);

									if (damagerAgent.statusEffects.hasStatusEffect("InvisibleLimited") || 
										(damagerAgent.statusEffects.hasStatusEffect("Invisible") && damagerAgent.statusEffects.hasSpecialAbility("InvisibleLimitedItem")) ||
										damagerAgent.hiddenInObject)
									{
										dmg *= 10f;
										damagerAgent.melee.successfullyBackstabbed = true;
										GC.OwnCheck(damagerAgent, damagedAgent.go, "Normal", 0);
									}
									else
										dmg *= 2f;
								}
							}
							else if (damagerAgent.statusEffects.hasStatusEffect(vStatusEffect.InvisibleTemporary))
							{
								bool alreadyDead = false;

								if (instanceIsAgent && damagedAgent.dead)
									alreadyDead = true;

								if (!playerDamagedByNpc && !alreadyDead && !damagerAgent.statusEffects.hasTrait(cTrait.Sniper) && !damagerAgent.statusEffects.hasTrait(cTrait.Sniper_2))
									damagerAgent.statusEffects.RemoveInvisibleLimited();
							}
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
			Prefix(typeof(SpawnerMain), "SpawnBullet", GetType(), "SpawnerMain_SpawnBullet", new Type[4] { typeof(Vector3), typeof(bulletStatus), typeof(PlayfieldObject), typeof(int) });
		}
		public static bool SpawnerMain_SpawnBullet(Vector3 bulletPos, bulletStatus bulletType, PlayfieldObject myPlayfieldObject, int bulletNetID, SpawnerMain __instance, ref Bullet __result) // Prefix
		{
			if (!GC.challenges.Contains(cChallenge.ScaryGuns)
				|| bulletType != bulletStatus.Normal || bulletType != bulletStatus.Shotgun || bulletType != bulletStatus.Revolver)
				return true;

			BMLog("SpawnerMain_SpawnBullet: bulletType = " + bulletType);

			Agent agent = null;
			Item item = null;
			ObjectReal objectReal = null;
			bool isFromAgent = false;
			bool isFromItem = false;
			float bulletScale = 0.333333f;

			if (myPlayfieldObject != null)
			{
				if (myPlayfieldObject.playfieldObjectType == "Agent")
				{
					isFromAgent = true;
					agent = myPlayfieldObject.playfieldObjectAgent;
				}

				if (myPlayfieldObject.playfieldObjectType == "Item")
				{
					isFromItem = true;
					item = myPlayfieldObject.playfieldObjectItem;
				}
				else
					objectReal = myPlayfieldObject.playfieldObjectReal;
			}

			switch (bulletType)
			{
				case bulletStatus.Normal:
					__result = __instance.bulletNormalPrefab_S.Spawn(bulletPos);
					break;
				case bulletStatus.Shotgun:
					__result = __instance.bulletShotgunPrefab_S.Spawn(bulletPos);
					break;
				case bulletStatus.Revolver:
					__result = __instance.bulletRevolverPrefab_S.Spawn(bulletPos);
					break;
				default:
					__result = __instance.bulletPrefab_S.Spawn(bulletPos);
					break;
			}

			__result.streamBullet = false;

			__result.DoEnable();
			__result.bulletType = bulletType;
			__result.spr = __result.bulletSprite;
			__result.bulletNetID = bulletNetID;

			if (isFromAgent)
			{
				__result.agent = agent;
				__result.cameFromCollider = agent.agentColliderNormal;

				__result.cameFromWeapon = agent.inventory.equippedWeapon.invItemName;
			}
			else if (isFromItem)
			{
				__result.item = item;
				__result.explosionOwner = item.owner;
				__result.cameFromCollider = item.itemHitboxCollider;
				__result.cameFromWeapon = myPlayfieldObject.objectName;
			}
			else
			{
				__result.objectReal = objectReal;
				__result.cameFromCollider = objectReal.objectHitbox;
				__result.cameFromWeapon = myPlayfieldObject.objectName;

				if (objectReal.hasObjectAgent)
					__result.agent = objectReal.objectAgent;
			}

			__result.SetupBullet();

			GameObject gameObject9 = null;

			try
			{
				gameObject9 = __result.tr.Find("P_BulletTrail").gameObject;
				gameObject9.transform.localPosition = new Vector3(0f, -0.32f, 0f);
				gameObject9.transform.localScale = new Vector3(bulletScale, bulletScale, bulletScale);
			}
			catch
			{
				try
				{
					if (__result.particles == null)
					{
						string str = "BulletTrail";
						gameObject9 = __result.tr.Find("P_" + str).gameObject;
					}
					else
						gameObject9 = __result.particles;

					gameObject9.transform.SetParent(__result.tr);
					gameObject9.transform.localPosition = new Vector3(0f, -0.32f, 0f);
					gameObject9.transform.localEulerAngles = Vector3.zero;
					gameObject9.transform.localScale = new Vector3(bulletScale, bulletScale, bulletScale);
				}
				catch
				{
					string effectType = "BulletTrail";
					gameObject9 = __instance.SpawnParticleEffect(effectType, bulletPos, __result.tr.eulerAngles.z);
					gameObject9.transform.SetParent(__result.tr);
					gameObject9.transform.localPosition = new Vector3(0f, -0.32f, 0f);
					gameObject9.transform.localEulerAngles = Vector3.zero;
					gameObject9.transform.localScale = new Vector3(bulletScale, bulletScale, bulletScale);
				}
			}

			__result.particles = gameObject9;
			ParticleEffect component8 = __result.particles.GetComponent<ParticleEffect>();
			component8.hasAttachedToObject = true;
			component8.attachedToObject = __result;
			component8.attachedToObjectTr = __result.tr;
			component8.ps.Play();
			__result.particleEffectGetsDeattached = true;

			try
			{
				__result.lightTemp.fancyLightRenderer.enabled = true;
				__result.lightTemp.fancyLight.bulletChanges = true;
			}
			catch
			{
				Debug.LogError("Bullet has no LightTemp: " + __result);
			}

			__result.particles = gameObject9;

			MeshRenderer component9 = __result.spr.GetComponent<MeshRenderer>();

			if (GC.challenges.Contains("RogueVision"))
			{
				if (__result.agent != null)
				{
					if (!__result.agent.rogueVisionInvisible)
					{
						component9.enabled = true;

						if (__result.lightTemp != null)
							__result.lightTemp.tr.Find("LightFancy").GetComponent<MeshRenderer>().enabled = true;

						if (__result.lightReal != null)
							__result.lightReal.gameObject.SetActive(true);

						if (__result.particles != null)
							__result.particles.gameObject.SetActive(true);
					}
					else
					{
						component9.enabled = false;

						if (__result.lightTemp != null)
							__result.lightTemp.tr.Find("LightFancy").GetComponent<MeshRenderer>().enabled = false;

						if (__result.lightReal != null)
							__result.lightReal.gameObject.SetActive(false);

						if (__result.particles != null)
							__result.particles.gameObject.SetActive(false);
					}
				}
			}
			else if (!component9.enabled)
			{
				component9.enabled = true;
				if (__result.lightTemp != null)
					__result.lightTemp.tr.Find("LightFancy").GetComponent<MeshRenderer>().enabled = true;

				if (__result.lightReal != null)
					__result.lightReal.gameObject.SetActive(true);

				if (__result.particles != null)
					__result.particles.gameObject.SetActive(true);
			}
			return false;
		}
		#endregion
	}
}
