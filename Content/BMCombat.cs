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
			Bullet_00();
			PlayfieldObject_00();
			SpawnerMain_00();
		}

		#region Bullet
		public void Bullet_00()
		{
			Postfix(typeof(Bullet), "SetupBullet", GetType(), "Bullet_SetupBullet", new Type[0] { });
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
			Agent agent = null;
			ObjectReal objectReal = null;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = true;

			if (__instance.isAgent && !generic)
			{
				agent = (Agent)__instance;
				flag = true;
			}
			else if (__instance.isObjectReal && !generic)
			{
				objectReal = (ObjectReal)__instance;
				flag3 = true;
			}
			
			Agent agent2 = null;
			float num = 0f;
			string a = "";
			bool flag5 = false;
			Item item = null;
			bool flag6 = true;
			bool flag7 = false;
			bool flag8 = false;
			bool flag9 = false;

			if (damagerObject.isAgent)
			{
				agent2 = damagerObject.GetComponent<Agent>();
				flag2 = true;

				if (agent2.statusEffects.hasStatusEffect("Giant"))
					num = 30f;
				else if (agent2.statusEffects.hasStatusEffect("ElectroTouch"))
				{
					num = 15f;
				
					if (flag)
					{
						if (agent.underWater || GC.tileInfo.GetTileData(agent.tr.position).spillWater)
						{
							if (agent.underWater)
								num *= 3f;
							else
								num *= 1.5f;
							
							if (agent2.localPlayer && agent2.isPlayer != 0)
								GC.unlocks.DoUnlockEarly("ElectrocuteInWater", "Extra");
						}
						else if (agent.underWater)
						{
							num *= 3f;

							if (agent2.localPlayer && agent2.isPlayer != 0)
								GC.unlocks.DoUnlockEarly("ElectrocuteInWater", "Extra");
						}

						if (!agent.dead && !testOnly)
						{
							agent.deathMethod = "ElectroTouch";
							agent.deathKiller = agent2.agentName;
						}
					}
				}
				else if (agent2.chargingForward)
				{
					if (flag)
					{
						if (!agent2.oma.superSpecialAbility && !agent2.statusEffects.hasTrait("ChargeMorePowerful"))
							num = 10f;
						else
							num = 20f;
						
						if (!agent.dead && !testOnly)
						{
							agent.deathMethod = "Charge";
							agent.deathKiller = agent2.agentName;
						}
					}
					else
						num = 30f;
				}
				else if (agent2 == agent && agent.Tripped())
					num = 5f;
				else
					num = 30f;
				
				if (flag && agent.shrunk && !agent2.shrunk)
				{
					num = 200f;
				
					if (!agent.dead && !testOnly)
					{
						agent.deathMethod = "Stomping";
						agent.deathKiller = agent2.agentName;
					}
				}

				a = "TouchDamage";
			}
			else if (damagerObject.isBullet)
			{
				Bullet component = damagerObject.GetComponent<Bullet>();
				agent2 = component.agent;

				if (component.agent != null)
				{
					flag2 = true;

					if (flag && component.agent.objectAgent && component.bulletType == bulletStatus.Fire && agent.knockedByObject != null && agent.bouncy && agent.knockedByObject.playfieldObjectType == "Agent" && agent.lastHitByAgent != null)
						agent2 = agent.lastHitByAgent;
				}
				
				num = (float)component.damage;
				a = "Bullet";
				
				if (component.bulletType == bulletStatus.Fire || component.bulletType == bulletStatus.Fireball)
					a = "Fire";
				
				if (component.bulletType == bulletStatus.Shotgun && (__instance.tickEndObject == null || __instance.tickEndObject.bulletType == bulletStatus.Shotgun))
					flag5 = true;
				
				if (component.bulletType == bulletStatus.GhostBlaster)
					flag7 = true;
				
				if (flag)
				{
					if (flag2)
					{
						if (!agent2.objectAgent)
						{
							float num2 = (float)agent2.accuracyStatMod;
							num2 += (float)component.moreAccuracy;
							num *= 0.6f + num2 / 5f;
							float x = agent2.agentSpriteTransform.localScale.x;

							if (x <= 0.65f || x >= 0.67f)
								num *= x;

							if (!agent.dead && !testOnly)
							{
								agent.deathMethodItem = component.cameFromWeapon;
								agent.deathMethodObject = component.cameFromWeapon;
								agent.deathMethod = component.cameFromWeapon;

								if (!agent2.objectAgent)
									agent.deathKiller = agent2.agentName;
							}
						}
						else if (!agent.dead && !testOnly)
						{
							agent.deathMethodItem = component.cameFromWeapon;
							agent.deathMethodObject = component.cameFromWeapon;
							agent.deathMethod = component.cameFromWeapon;
							agent.deathKiller = "Nature";
						}
					}
					else if (!agent.dead && !testOnly)
					{
						if (component.bulletType == bulletStatus.Water || component.bulletType == bulletStatus.Water2)
						{
							agent.deathMethodItem = component.cameFromWeapon;
							agent.deathMethodObject = component.cameFromWeapon;
							agent.deathMethod = component.cameFromWeapon;
							agent.deathKiller = "Nature";
						}
						else
						{
							agent.deathMethodItem = component.cameFromWeapon;
							agent.deathMethodObject = damagerObject.objectName;
							agent.deathMethod = damagerObject.objectName;
							agent.deathKiller = "Nature";
						}
					}
				}
			}
			else if (damagerObject.isMelee)
			{
				Melee melee = damagerObject.playfieldObjectMelee;
				agent2 = melee.agent;
				flag2 = true;
				InvItem invItem;

				if (melee.invItem.weaponCode != weaponType.WeaponMelee)
					invItem = agent2.inventory.fist;
				else
					invItem = melee.invItem;
				
				num = (float)invItem.meleeDamage;
				num *= 1f + (float)agent2.strengthStatMod / 3f;
				float x2 = agent2.agentSpriteTransform.localScale.x;
				num *= x2;
				a = "Melee";
				
				if (flag2 && flag)
				{
					if (!agent.dead && !testOnly)
					{
						agent.deathMethodItem = invItem.invItemName;
						agent.deathMethodObject = invItem.invItemName;
						agent.deathMethod = invItem.invItemName;
						agent.deathKiller = agent2.agentName;
					}
				}
				else if (flag && !agent.dead && !testOnly)
				{
					agent.deathMethodItem = invItem.invItemName;
					agent.deathMethodObject = invItem.invItemName;
					agent.deathMethod = invItem.invItemName;
					agent.deathKiller = "Nature";
				}
			}
			else if (damagerObject.isExplosion)
			{
				Explosion explosion = damagerObject.playfieldObjectExplosion;
				agent2 = explosion.agent;

				if (explosion.agent != null)
				{
					flag2 = true;
				
					if (flag)
					{
						if (explosion.realSource != null && explosion.realSource.isItem && (!agent.movement.HasLOSAgent360(explosion.agent) || Vector2.Distance(agent.curPosition, explosion.agent.curPosition) > explosion.agent.LOSRange / agent.hardToSeeFromDistance))
							flag4 = false;
					
						if (explosion.sourceObject != null && explosion.sourceObject.isBullet && explosion.sourceObject.playfieldObjectBullet.cameFromWeapon == "Fireworks" && (!agent.movement.HasLOSAgent360(explosion.agent) || Vector2.Distance(agent.curPosition, explosion.agent.curPosition) > explosion.agent.LOSRange / agent.hardToSeeFromDistance))
							flag4 = false;
					}
				}

				num = (float)explosion.damage;
				a = "Explosion";
				
				if (flag2 && flag)
				{
					if (!agent.dead && !testOnly)
					{
						agent.deathMethod = "Explosion";

						if (agent2 != agent && !agent2.objectAgent)
							agent.deathKiller = agent2.agentName;
						else
							agent.deathKiller = "Self";
					}
				}
				else if (flag && !agent.dead && !testOnly)
				{
					agent.deathMethod = "Explosion";
					agent.deathKiller = "Nature";
				}
			}
			else if (damagerObject.isFire)
			{
				Fire fire = damagerObject.playfieldObjectFire;
				agent2 = fire.agent;

				if (fire.agent != null)
				{
					flag2 = true;
				
					if (flag && (!agent.movement.HasLOSAgent360(fire.agent) || Vector2.Distance(agent.curPosition, fire.agent.curPosition) > fire.agent.LOSRange / agent.hardToSeeFromDistance))
						flag4 = false;
				}

				num = (float)fire.damage;
				a = "Fire";
				
				if (flag)
				{
					if (flag2)
					{
						if (!agent.dead && !testOnly)
						{
							agent.deathMethod = "Fire";
				
							if (!agent2.objectAgent)
								agent.deathKiller = agent2.agentName;
						}
					}
					else if (!agent.dead && !testOnly)
					{
						agent.deathMethod = "Fire";
						agent.deathKiller = "Nature";
					}
				}
			}
			else if (damagerObject.isObjectReal)
			{
				ObjectReal objectReal2 = damagerObject.playfieldObjectReal;
				num = (float)objectReal2.hazardDamage;
				a = "Hazard";

				if (flag && agent.knockedByObject != null && agent.bouncy && agent.knockedByObject.playfieldObjectType == "Agent" && agent.lastHitByAgent != null)
				{
					agent2 = agent.lastHitByAgent;
					flag2 = true;
				}
				
				if (flag2 && flag)
				{
					if (!agent.dead && !testOnly)
					{
						agent.deathMethodItem = objectReal2.objectName;
						agent.deathMethodObject = objectReal2.objectName;
						agent.deathMethod = objectReal2.objectName;
				
						if (!agent2.objectAgent)
							agent.deathKiller = agent2.agentName;
					}
				}
				else if (flag)
				{
					if (!agent.dead && !testOnly)
					{
						agent.deathMethodItem = objectReal2.objectName;
						agent.deathMethodObject = objectReal2.objectName;
						agent.deathMethod = objectReal2.objectName;
						agent.deathKiller = "Nature";
					}
				}
				else if (flag3)
					num = 30f;
			}
			else if (damagerObject.isItem)
			{
				item = damagerObject.playfieldObjectItem;

				if (item.invItem.otherDamage > 0 && item.otherDamageMode)
				{
					if (item.hitCauser != null)
					{
						agent2 = item.hitCauser;
						flag2 = true;
					}
					else if (item.owner != null)
					{
						agent2 = item.owner;
						flag2 = true;
				
						if (flag && (!agent.movement.HasLOSAgent360(item.owner) || Vector2.Distance(agent.curPosition, item.owner.curPosition) > item.owner.LOSRange / agent.hardToSeeFromDistance))
							flag4 = false;
					}

					num = (float)item.invItem.otherDamage;
				}
				else if (item.invItem.touchDamage > 0 && __instance.playfieldObjectType == "Agent")
				{
					if (item.hitCauser != null)
					{
						agent2 = item.hitCauser;
						flag2 = true;
					}
					else if (item.owner != null)
					{
						agent2 = item.owner;
						flag2 = true;
					
						if (flag && (!agent.movement.HasLOSAgent360(item.owner) || Vector2.Distance(agent.curPosition, item.owner.curPosition) > item.owner.LOSRange / agent.hardToSeeFromDistance))
							flag4 = false;
					}
					
					if (item.invItem.touchDamage > 0)
						num = (float)item.invItem.touchDamage;
					else if (item.invItem.otherDamage > 0)
						num = (float)item.invItem.otherDamage;
					
					if (item.thrower != null)
						a = "Throw";
				}
				else
				{
					if (item.thrower != null && item.invItem.throwDamage != 0)
					{
						agent2 = item.thrower;
						flag2 = true;
					}
				
					num = (float)item.invItem.throwDamage;
					
					if (flag2 && item.invItem.invItemName == "TossItem" && (agent2.oma.superSpecialAbility || agent2.statusEffects.hasTrait("GoodThrower")))
						num *= 2f;
					
					a = "Throw";
				}

				if (!flag2 && item.thrower != null && item.thrower.statusEffects.hasTrait("KillerThrower"))
				{
					agent2 = item.thrower;
					flag2 = true;
					a = "Throw";
				}
				
				if (flag2 && flag)
				{
					if (!agent.dead && !testOnly)
					{
						agent.deathMethodItem = item.invItem.invItemName;
						agent.deathMethodObject = item.invItem.invItemName;
						agent.deathMethod = item.invItem.invItemName;
				
						if (!agent2.objectAgent)
							agent.deathKiller = agent2.agentName;
					}
				}
				else if (flag && !agent.dead && !testOnly)
				{
					agent.deathMethodItem = item.invItem.invItemName;
					agent.deathMethodObject = item.invItem.invItemName;
					agent.deathMethod = item.invItem.invItemName;
					agent.deathKiller = "Nature";
				}
			}

			bool flag10 = false;
			
			if (flag2)
			{
				if (agent2.isPlayer != 0 && !agent2.localPlayer)
					flag10 = true;
			
				if (flag && agent.isPlayer != 0 && agent2.isPlayer != 0 && !GC.pvp)
					flag6 = false;
			}
			if (a == "Melee")
			{
				if (agent2.statusEffects.hasTrait("Strength"))
					num *= 1.5f;
				
				if (agent2.statusEffects.hasTrait("StrengthSmall"))
					num *= 1.25f;
				
				if (agent2.statusEffects.hasTrait("Weak"))
					num *= 0.5f;
				
				if (agent2.statusEffects.hasTrait("Withdrawal"))
					num *= 0.75f;
			
				if (agent2.melee.specialLunge)
				{
					if (agent2.agentName == "WerewolfB")
						num *= 1.3f;
					else
						num *= 2f;
				}
				
				if (agent2.inventory.equippedWeapon.invItemName == "Fist" || agent2.inventory.equippedWeapon.itemType == "WeaponProjectile")
				{
					if (agent2.statusEffects.hasTrait("StrongFists2"))
						num *= 1.8f;
					else if (agent2.statusEffects.hasTrait("StrongFists"))
						num *= 1.4f;
				
					if (agent2.statusEffects.hasTrait("CantAttack") && __instance.isAgent)
					{
						num = 0f;
						flag8 = true;
					}
					else if (agent2.statusEffects.hasTrait("AttacksOneDamage") && __instance.isAgent)
					{
						num = 1f;
						flag9 = true;
					}
				}

				if (!flag10 && flag6)
				{
					if (agent2.inventory.equippedArmor != null && !testOnly && (agent2.inventory.equippedArmor.armorDepletionType == "MeleeAttack" && flag) && !agent.dead && !agent.mechEmpty && !agent.butlerBot)
						agent2.inventory.DepleteArmor("Normal", Mathf.Clamp((int)(num / 2f), 0, 12));
				
					if (agent2.inventory.equippedArmorHead != null && !testOnly && (agent2.inventory.equippedArmorHead.armorDepletionType == "MeleeAttack" && flag) && !agent.dead && !agent.mechEmpty && !agent.butlerBot)
						agent2.inventory.DepleteArmor("Head", Mathf.Clamp((int)(num / 2f), 0, 12));
				}

				if (flag)
				{
					float num3 = num / agent2.agentSpriteTransform.localScale.x;
				
					if (!agent.dead && !testOnly && !flag10 && flag6 && !agent.butlerBot && !agent.mechEmpty)
						agent2.inventory.DepleteMelee(Mathf.Clamp((int)num3, 0, 15), damagerObject.playfieldObjectMelee.invItem);
					
					if ((agent2.statusEffects.hasTrait("SleepKiller") || agent2.statusEffects.hasTrait("Backstabber")) && agent.sleeping)
					{
						num = 200f;
						agent.agentHitboxScript.wholeBodyMode = 0;
						agent2.melee.successfullySleepKilled = true;
					
						if (agent2.statusEffects.hasTrait("Backstabber"))
							agent.statusEffects.CreateBuffText("Backstab", agent.objectNetID);
					}
					else if ((agent2.melee.mustDoBackstab && num != 200f && !agent.dead) || (agent2.statusEffects.hasTrait("Backstabber") && ((agent.mostRecentGoalCode != goalType.Battle && agent.mostRecentGoalCode != goalType.Flee) || agent.frozen) && !agent.movement.HasLOSObjectBehind(agent2) && !agent.sleeping && num != 200f && !agent.dead))
					{
						agent.agentHelperTr.localPosition = new Vector3(-0.64f, 0f, 0f);

						if (!GC.tileInfo.IsOverlapping(agent.agentHelperTr.position, "Wall"))
						{
							agent.agentHelperTr.localPosition = Vector3.zero;
							agent.statusEffects.CreateBuffText("Backstab", agent.objectNetID);
						
							if (agent2.statusEffects.hasStatusEffect("InvisibleLimited") || (agent2.statusEffects.hasStatusEffect("Invisible") && agent2.statusEffects.hasSpecialAbility("InvisibleLimitedItem")))
							{
								num *= 10f;
								agent2.melee.successfullyBackstabbed = true;
								GC.OwnCheck(agent2, agent.go, "Normal", 0);
							}
							else
								num *= 2f;
						}
					}
					else if (agent2.statusEffects.hasStatusEffect("InvisibleLimited"))
					{
						bool flag11 = false;

						if (flag && agent.dead)
							flag11 = true;
						
						if (!flag10 && !flag11 && !agent2.oma.superSpecialAbility && !agent2.statusEffects.hasTrait("FailedAttacksDontEndCamouflage"))
							agent2.statusEffects.RemoveInvisibleLimited();
					}
				}
			}
			else if (a == "Bullet")
			{
				if (flag && !flag7)
				{
					if (agent.statusEffects.hasTrait("ResistBullets"))
						num /= 1.5f;
				
					if (agent.statusEffects.hasTrait("ResistBulletsSmall"))
						num /= 1.2f;
					
					if (agent.statusEffects.hasTrait("ResistBulletsTrait2"))
						num /= 2f;
					else if (agent.statusEffects.hasTrait("ResistBulletsTrait"))
						num /= 1.5f;
				}
			}
			else if (a == "Fire")
			{
				if (flag)
				{
					if (agent.statusEffects.hasTrait("ResistFire"))
						num /= 1.5f;
					
					if ((agent.oma.superSpecialAbility && agent.agentName == "Firefighter") || agent.statusEffects.hasTrait("FireproofSkin2"))
					{
						num = 0f;
						flag8 = true;
					}
					else if (agent.statusEffects.hasTrait("FireproofSkin"))
						num /= 1.5f;
				}
			}
			else if (a == "Throw")
			{
				if (flag2)
				{
					if (agent2.statusEffects.hasTrait("GoodThrower"))
						num *= 2f;
					
					if (flag && agent2.statusEffects.hasTrait("KillerThrower") && item.throwerReal == item.thrower)
					{
						if (agent != item.thrower)
						{
							if (agent.health >= 100f)
								num = 100f;
							else
								num = 200f;
						}
						else
							num = 20f;
					}
				}
			}
			else if (!(a == "Explosion"))
				_ = a == "Hazard";
			
			if (flag2 && flag && !testOnly)
			{
				if (agent2.statusEffects.hasTrait("BloodyMess"))
					agent.bloodyMessed = true;
			
				if ((agent2.invisible && !agent2.oma.hidden) || agent2.ghost)
				{
					agent2.gc.spawnerMain.SpawnDanger(agent2, "Targeted", "Spooked", agent);
					relStatus relCode = agent2.relationships.GetRelCode(agent);
				
					if (relCode != relStatus.Aligned && relCode != relStatus.Loyal)
					{
						List<Agent> agentList = GC.agentList;
					
						for (int i = 0; i < agentList.Count; i++)
						{
							Agent agent3 = agentList[i];
						
							if (agent3.employer == agent2)
							{
								relStatus relCode2 = agent3.relationships.GetRelCode(agent);
							
								if (relCode2 != relStatus.Aligned && relCode2 != relStatus.Loyal)
									agent3.relationships.SetRelHate(agent, 5);
								else if (relCode2 == relStatus.Aligned && agent3.relationships.GetRelCode(agent2) == relStatus.Loyal)
								{
									agent3.relationships.SetRelHate(agent2, 5);
									agent2.agentInteractions.LetGo(agent3, agent2);
								}
							}
						}
					}
				}
			}
			if (flag)
			{
				if (agent.statusEffects.hasTrait("NumbToPain"))
					num /= 3f;
				
				if (agent.statusEffects.hasTrait("ResistDamageSmall"))
					num /= 1.25f;
				
				if (agent.statusEffects.hasTrait("ResistDamageMed"))
					num /= 1.5f;
				
				if (agent.statusEffects.hasTrait("ResistDamageLarge"))
					num /= 2f;
				
				if (agent.statusEffects.hasTrait("Giant"))
					num /= 3f;
				
				if (agent.statusEffects.hasTrait("Shrunk"))
					num *= 3f;
				
				if (agent.statusEffects.hasTrait("Diminutive"))
					num *= 1.5f;
				
				if (agent.frozen)
					num *= 2f;
				
				if (agent.statusEffects.hasSpecialAbility("ProtectiveShell") && agent.objectMult.chargingSpecialLunge)
					num /= 8f;
				
				if (agent.hasEmployer && agent.employer.statusEffects.hasSpecialAbility("ProtectiveShell") && agent.employer.objectMult.chargingSpecialLunge)
					num /= 8f;
				
				if (agent.oma.mindControlled && agent.mindControlAgent != null && (agent.mindControlAgent.statusEffects.hasTrait("MindControlledResistDamage") || (agent.mindControlAgent.oma.superSpecialAbility && agent.mindControlAgent.agentName == "Alien")))
					num /= 1.5f;
				
				if (flag2 && flag6 && !agent2.dead)
				{
					if (agent2.statusEffects.hasTrait("MoreDamageWhenHealthLow") && agent2.agentID != agent.agentID)
					{
						int num4 = (int)(agent2.healthMax / 4f);
				
						if (agent2.health <= (float)num4)
						{
							float num5 = agent2.health / (float)num4;
							num5 = (1f - num5) * num * 1.5f;
							num += num5;
						}
					}
					else if (agent2.statusEffects.hasTrait("MoreDamageWhenHealthLow2") && agent2.agentID != agent.agentID)
					{
						int num6 = (int)(agent2.healthMax / 2f);
						
						if (agent2.health <= (float)num6)
						{
							float num7 = agent2.health / (float)num6;
							num7 = (1f - num7) * num * 1.5f;
							num += num7;
						}
					}

					if (!testOnly && agent2.agentID != agent.agentID)
					{
						int num8 = agent2.critChance;
						num8 = agent2.DetermineLuck(num8, "CritChance", true);

						if (Random.Range(0, 100) <= num8 - 1 && (!(GC.levelType == "Tutorial") || !(a == "Explosion")))
						{
							num *= 2f;
							agent.critted = true;
						}
						
						if (agent2.statusEffects.hasTrait("ChanceToSlowEnemies2"))
						{
							int myChance = agent2.DetermineLuck(20, "ChanceToSlowEnemies", true);
						
							if (GC.percentChance(myChance))
								agent.statusEffects.AddStatusEffect("Slow");
						}
						else if (agent2.statusEffects.hasTrait("ChanceToSlowEnemies"))
						{
							int myChance2 = agent2.DetermineLuck(8, "ChanceToSlowEnemies", true);
						
							if (GC.percentChance(myChance2))
								agent.statusEffects.AddStatusEffect("Slow");
						}
					}

					if (agent2.statusEffects.hasTrait("MoreFollowersCauseMoreDamage") || agent2.statusEffects.hasTrait("MoreFollowersCauseMoreDamage2"))
					{
						float num9 = 1.2f;

						if (agent2.statusEffects.hasTrait("MoreFollowersCauseMoreDamage2"))
							num9 = 1.4f;
						
						float num10 = num;
						int num11 = 0;
						
						for (int j = 0; j < GC.agentList.Count; j++)
						{
							Agent agent4 = GC.agentList[j];
						
							if (agent4.hasEmployer && agent4.employer == agent2 && Vector2.Distance(agent4.tr.position, agent.tr.position) < 10.24f)
							{
								num += num10 * num9 - num10;
								num11++;
							
								if (num11 >= 3 && !GC.challenges.Contains("NoLimits"))
									break;
							}
						}
					}

					if (agent2.oma.mindControlled && agent2.mindControlAgent != null && (agent2.mindControlAgent.statusEffects.hasTrait("MindControlledDamageMore") || (agent2.mindControlAgent.oma.superSpecialAbility && agent2.mindControlAgent.agentName == "Alien")))
						num *= 1.5f;
				}

				int num12 = 0;
				
				if (agent.inventory.equippedArmor != null && !testOnly && flag6)
				{
					InvItem equippedArmor = agent.inventory.equippedArmor;
				
					if (equippedArmor.armorDepletionType == "Everything")
						num12++;
					else if (equippedArmor.armorDepletionType == "Bullet" && a == "Bullet")
						num12++;
					else if (equippedArmor.armorDepletionType == "Fire" && a == "Fire")
						num12++;
					else if (equippedArmor.armorDepletionType == "FireAndEverything")
						num12++;
				}

				if (agent.inventory.equippedArmorHead != null && !testOnly && flag6)
				{
					InvItem equippedArmorHead = agent.inventory.equippedArmorHead;
				
					if (equippedArmorHead.armorDepletionType == "Everything")
						num12++;
					else if (equippedArmorHead.armorDepletionType == "Bullet" && a == "Bullet")
						num12++;
					else if (equippedArmorHead.armorDepletionType == "Fire" && a == "Fire")
						num12++;
					else if (equippedArmorHead.armorDepletionType == "FireAndEverything")
						num12++;
				}

				if (agent.inventory.equippedArmor != null && !testOnly && flag6)
				{
					InvItem equippedArmor2 = agent.inventory.equippedArmor;
				
					if (equippedArmor2.armorDepletionType == "Everything")
						agent.inventory.DepleteArmor("Normal", Mathf.Clamp((int)(num * 2f), 0, 12) / num12);
					else if (equippedArmor2.armorDepletionType == "Bullet" && a == "Bullet")
						agent.inventory.DepleteArmor("Normal", Mathf.Clamp((int)(num * 2f), 0, 12) / num12);
					else if (equippedArmor2.armorDepletionType == "Fire" && a == "Fire")
						agent.inventory.DepleteArmor("Normal", Mathf.Clamp((int)(num * 2f), 0, 12) / num12);
					else if (equippedArmor2.armorDepletionType == "FireAndEverything")
						agent.inventory.DepleteArmor("Normal", Mathf.Clamp((int)(num * 2f), 0, 12) / num12);
				}

				if (agent.inventory.equippedArmorHead != null && !testOnly && flag6)
				{
					InvItem equippedArmorHead2 = agent.inventory.equippedArmorHead;

					if (equippedArmorHead2.armorDepletionType == "Everything")
						agent.inventory.DepleteArmor("Head", Mathf.Clamp((int)(num * 2f), 0, 12) / num12);
					else if (equippedArmorHead2.armorDepletionType == "Bullet" && a == "Bullet")
						agent.inventory.DepleteArmor("Head", Mathf.Clamp((int)(num * 2f), 0, 12) / num12);
					else if (equippedArmorHead2.armorDepletionType == "Fire" && a == "Fire")
						agent.inventory.DepleteArmor("Head", Mathf.Clamp((int)(num * 2f), 0, 12) / num12);
					else if (equippedArmorHead2.armorDepletionType == "FireAndEverything")
						agent.inventory.DepleteArmor("Head", Mathf.Clamp((int)(num * 2f), 0, 12) / num12);
				}

				if (agent.statusEffects.hasTrait("MoreFollowersLessDamageToPlayer") || agent.statusEffects.hasTrait("MoreFollowersLessDamageToPlayer2"))
				{
					int num13 = 0;
					float num14 = 1.2f;
				
					if (agent.statusEffects.hasTrait("MoreFollowersLessDamageToPlayer2"))
						num14 = 1.4f;
					
					for (int k = 0; k < GC.agentList.Count; k++)
					{
						Agent agent5 = GC.agentList[k];
					
						if (agent5.hasEmployer && agent5.employer == agent && Vector2.Distance(agent5.tr.position, agent.tr.position) < 10.24f)
						{
							num /= num14;
							num13++;
						
							if (num13 >= 3 && !GC.challenges.Contains("NoLimits"))
								break;
						}
					}
				}

				if (!testOnly && flag4)
					agent.attackCooldown = 2f;
			}

			if (flag3 && flag2 && (__instance.objectName == "Bars" || __instance.objectName == "BarbedWire"))
			{
				if (agent2.statusEffects.hasTrait("MeleeDestroysWalls2"))
					num = 99f;
				else if (agent2.statusEffects.hasTrait("MeleeDestroysWalls") && __instance.objectName == "BarbedWire")
					num = 99f;
			}
			
			if (num > 200f)
				num = 200f;
			
			int num15 = Mathf.Clamp((int)num, 1, 1000);
			
			if ((damagerObject.isItem && a == "Throw" && num == 0f) || flag8)
				num15 = 0;
			else if (flag9)
				num15 = 1;
			
			if (flag2 && flag && !testOnly)
			{
				if ((float)num15 < agent.health)
				{
					Relationship relationship = agent.relationships.GetRelationship(agent2);
					relStatus myRel = relStatus.Neutral;
					bool flag12 = false;
			
					if (relationship != null)
					{
						myRel = relationship.relTypeCode;
						flag12 = relationship.sawBecomeHidden;
					}
					
					if ((!agent2.invisible || flag12) && flag4)
					{
						if ((agent2.isPlayer <= 0 || agent2.localPlayer || damagerObject.isItem || damagerObject.isExplosion || agent2.statusEffects.hasTrait("CantAttack")) && (!damagerObject.isExplosion || !damagerObject.noAngerOnHit) && !agent.mechEmpty)
						{
							agent.justHitByAgent3 = true;
							agent.relationships.AddRelHate(agent2, Mathf.Clamp(num15, 5, 200));
							agent.justHitByAgent3 = false;
						}
					
						agent.relationships.PotentialAlignmentCheck(myRel);
					}
				}

				if (flag4)
					agent.SetJustHitByAgent(agent2);
				
				agent.justHitByAgent2 = agent2;
				agent.lastHitByAgent = agent2;
				
				if (!agent2.killerRobot && !agent.killerRobot)
				{
					relStatus relCode3 = agent2.relationships.GetRelCode(agent);
				
					if (damagerObject.isExplosion)
					{
						Explosion explosion2 = (Explosion)damagerObject;
					
						if (explosion2.explosionType == "Huge" || explosion2.explosionType == "Ridiculous")
						{
							GC.EnforcerAlertAttack(agent2, agent, 10.8f, explosion2.tr.position);
						
							if (agent2.ownerID != 0 && relCode3 == relStatus.Hostile)
								GC.EnforcerAlertAttack(agent, agent2, 10.8f, explosion2.tr.position);
						}
						else
						{
							GC.EnforcerAlertAttack(agent2, agent, 10.8f, explosion2.tr.position);
						
							if (agent2.ownerID != 0 && relCode3 == relStatus.Hostile)
								GC.EnforcerAlertAttack(agent, agent2, 10.8f, explosion2.tr.position);
						}
					}
					else
					{
						GC.EnforcerAlertAttack(agent2, agent, 7.4f);
						
						if (agent2.ownerID != 0 && relCode3 == relStatus.Hostile)
							GC.EnforcerAlertAttack(agent, agent2, 7.4f);
					}
				}

				agent.damagedAmount = num15;
				
				if (agent.agentName == "Slave")
					__instance.StartCoroutine(agent.agentInteractions.OwnCheckSlaveOwners(agent, agent2));
				
				if (agent.isPlayer == 0 && !agent.hasEmployer && !agent.zombified && !agent.noEnforcerAlert)
					agent2.oma.hasAttacked = true;
			}

			if (flag3)
			{
				if (flag2)
				{
					if (!testOnly)
					{
						objectReal.lastHitByAgent = agent2;
						objectReal.damagedAmount = num15;
			
						if (objectReal.useForQuest != null || objectReal.destroyForQuest != null)
							GC.OwnCheck(agent2, objectReal.gameObject, "Normal", 0);
					}

					if (!agent2.objectAgent && agent2.agentSpriteTransform.localScale.x > 1f)
						num15 = 99;
				}
				else if (!testOnly)
				{
					objectReal.lastHitByAgent = null;
					objectReal.damagedAmount = num15;
				}
			}

			if (!flag5 || flag3 || fromClient)
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
