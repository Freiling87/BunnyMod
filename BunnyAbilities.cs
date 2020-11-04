using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using BepInEx;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;

namespace BunnyMod
{
	public class BunnyAbilities
	{
		#region generic
		public void Awake()
		{
			InitializeAbilities();

			BunnyHeader.MainInstance.PatchPrefix(typeof(Gun), "Shoot", GetType(), "Gun_Shoot", new Type[5] { typeof(bool), typeof(bool), typeof(bool), typeof(int), typeof(string) });

			BunnyHeader.MainInstance.PatchPostfix(typeof(InvItem), "SetupDetails", GetType(), "InvItem_SetupDetails", new Type[1] { typeof(bool) });

			BunnyHeader.MainInstance.PatchPrefix(typeof(ObjectMult), "AttackProjectilePos", GetType(), "ObjectMult_AttackProjectilePos", new Type[6] { typeof(Vector2), typeof(bool), typeof(bool), typeof(bool), typeof(int), typeof(string)});

			BunnyHeader.MainInstance.PatchPrefix(typeof(StatusEffects), "GiveSpecialAbility", GetType(), "StatusEffects_GiveSpecialAbility", new Type[1] { typeof(string) });
			BunnyHeader.MainInstance.PatchPrefix(typeof(StatusEffects), "HeldSpecialAbility", GetType(), "StatusEffects_HeldSpecialAbility");
			BunnyHeader.MainInstance.PatchPrefix(typeof(StatusEffects), "RechargeSpecialAbility", GetType(), "StatusEffects_RechargeSpecialAbility", new Type[1] { typeof(string) });
		}
		public static void InitializeAbilities()
		{
			#region pyromancy
			Sprite sprite = RogueUtilities.ConvertToSprite(Properties.Resources.Fireball);

			CustomAbility pyromancy = RogueLibs.CreateCustomAbility("Pyromancy", sprite, true,
				new CustomNameInfo("Pyromancy"),
				new CustomNameInfo("You can throw fireballs from your hands. This tends to fix a lot of your problems, and create much worse ones."),
				delegate (InvItem item)
				{
					item.initCount = 0;
				});
			pyromancy.Available = true;
			pyromancy.CostInCharacterCreation = 10;
			pyromancy.OnPressed = delegate (InvItem item, Agent agent)
			{
				if (item.invItemCount > 0)
				{
					//Gun.Shoot()
					//TODO: Mirror pretty much everything that mentions "LaserGun"
				}
				else
				{
					item.agent.gc.audioHandler.Play(item.agent, "CantDo");
				}
			};
			pyromancy.RechargeInterval = ((InvItem item, Agent agent) =>
				(item.invItemCount > 0) ? new WaitForSeconds(1f) : null);
			pyromancy.Recharge = delegate (InvItem item, Agent agent)
			{

			};
			#endregion
		}
		#endregion
		#region Agent
		//FindCategories - This seems to determine Trait choices based on special abilities
		#endregion
		#region Gun
		//GunUpdate - Seems to be generic, with some exceptions for laser gun I think I can ignore for now.
		public bool Gun_Shoot(bool specialAbility, bool silenced, bool rubber, int bulletNetID, string bulletStatusEffect, Gun __instance) // Prefix
		{
			GameController gc = BunnyHeader.gc;
			InvItem gunFired = specialAbility ? __instance.agent.inventory.equippedSpecialAbility : __instance.agent.inventory.equippedWeapon;
			InvItem special = __instance.agent.inventory.equippedSpecialAbility;
			bool hasAmmo = false;

			if (((gunFired.invItemName == "WaterCannon" || gunFired.invItemName == "LaserGun" || gunFired.invItemName == "Pyromancy") &&
				(__instance.mostRecentGunUsed != special.invItemName)) ||
				((__instance.mostRecentGunUsed == "WaterCannon" || __instance.mostRecentGunUsed == "LaserGun" || __instance.mostRecentGunUsed == "Pyromancy") && gunFired.invItemName != special.invItemName))
				__instance.HideGun();

			__instance.mostRecentGunUsed = gunFired.invItemName;

			if (gunFired.invItemCount > 0 || (!__instance.agent.localPlayer && __instance.agent.isPlayer != 0))
				hasAmmo = true;
			else if (__instance.agent.isPlayer > 0 && gunFired.rechargeAmountInverse == 0)
			{
				gc.audioHandler.Play(__instance.agent, "AmmoOut");
				__instance.agent.inventory.ChooseWeapon();
				Debug.Log("Out of Ammo");
			}

			if (hasAmmo)
			{
				__instance.ShowGun(gunFired);
				Bullet bullet = null;
				string invItemName = gunFired.invItemName;

				if (invItemName == "Pyromancy")
				{
					bullet = __instance.spawnBullet(bulletStatus.Rocket, gunFired, bulletNetID, false); // Change to Fireball once you verify this actually works, since FBs aren't verified to work

					gc.spawnerMain.SpawnLightTemp(__instance.tr.position, null, "GunFlash");

					if (gc.serverPlayer)
						gc.spawnerMain.SpawnDanger(__instance.agent, "Minor", "Huge");

					__instance.SetWeaponCooldown(0.39f, gunFired);
					__instance.SubtractBullets(1, gunFired);

					if (__instance.agent.isPlayer > 0 && __instance.agent.localPlayer)
						gc.ScreenBump(2f, 30, __instance.agent);

					gc.audioHandler.Play(__instance.agent, "RocketLauncherFire");
					gc.alienFX.FireGun(__instance.agent);
					gc.playerControl.Vibrate(__instance.agent.isPlayer, 0.25f, 0.2f);
					__instance.agent.statusEffects.timeSinceLastShot = 0f;
				}

				if (gunFired.gunKnockback != 0 && bullet != null && (__instance.agent.statusEffects.hasTrait("KnockbackMore") || gunFired.gunKnockback >= 30) && !__instance.agent.statusEffects.hasTrait("KnockbackLess") && !__instance.agent.statusEffects.hasTrait("KnockbackLess2"))
				{
					Quaternion rot = default;
					rot.eulerAngles = new Vector3(bullet.tr.eulerAngles.x, bullet.tr.eulerAngles.y, bullet.tr.eulerAngles.z + 270f);
					__instance.agent.movement.KnockBackBullet(rot, (float)gunFired.gunKnockback, true, null);
				}
				__instance.gunContainerAnim.Play("", -1, 0f);


				if (gunFired.invItemName == "WaterCannon")
				{
					if (!__instance.firstShotCompleted)
						__instance.gunContainerAnim.Play("GunForward-1");
					else
						__instance.gunContainerAnim.Play("GunForward-2");
				}
				else if (gunFired.invItemName == "LaserGun")
					__instance.gunContainerAnim.Play("Gun-Shoot-Forward");
				else
					__instance.gunContainerAnim.Play("Gun-Shoot", -1, 0f);

				__instance.shootAnimPlaying = true;

				if (__instance.agent.statusEffects.hasTrait("GunKnockback"))
					__instance.agent.movement.KnockBack(__instance.go, 300f, null);
				if (__instance.agent.isPlayer > 0 && __instance.agent.localPlayer && !gunFired.Categories.Contains("NotRealWeapons"))
					gc.stats.AddToStat(__instance.agent, "GunsUsed", true);
			}
			if (!__instance.firstShotCompleted)
				__instance.firstShotCompleted = true;

			return true;
		}
		// ShowGun - Only for visuals I think, which would be skipped for this
		#endregion
		#region InvItem
		public void InvItem_SetupDetails(bool notNew, InvItem __instance) // Postfix
		{
			if (__instance.invItemName == "Pyromancy")
			{
				__instance.LoadItemSprite("Fireball");
				__instance.itemType = "WeaponProjectile";
				__instance.weaponCode = weaponType.WeaponProjectile;
				__instance.Categories.Add("Weapons");
				__instance.isWeapon = true;
				__instance.rapidFire = false;
				__instance.initCount = 15;
				__instance.initCountAI = 15;
				__instance.shadowOffset = 2;
				__instance.gunKnockback = 0;
				// May need to set RechargeAmount as well, not sure.
				__instance.rechargeAmountInverse = __instance.initCount;
				__instance.cantDrop = true;
				__instance.Categories.Add("NPCsCantPickUp");
				__instance.dontAutomaticallySelect = true;
				__instance.dontSelectNPC = true;
				__instance.specialMeleeTexture = true;
				__instance.thiefCantSteal = true;
			}
		}
		#endregion
		#region ObjectMult
		public static bool ObjectMult_AttackProjectilePos(Vector2 aimPosition, bool specialAbility, bool silenced, bool rubber, int bulletNetID, string bulletStatusEffect, ObjectMult __instance) // Prefix
		{
			if (specialAbility && __instance.agent.statusEffects.hasSpecialAbility("Pyromancy"))
			{
				if (__instance.gc.debugMode)
					Debug.Log("AimPosition: " + aimPosition);

				__instance.agent.melee.attackObject = null;
				__instance.agent.melee.attackPosition = aimPosition;

				__instance.agent.gun.spawnBullet(bulletStatus.Fireball, null, bulletNetID, true);
				__instance.gc.audioHandler.Play(__instance.agent, "SpitFireball");

				__instance.agent.gun.holdAttackAimPoint = aimPosition;
				return false;
			}
			return true;
		}
		#endregion
		#region StatusEffects
		public static bool StatusEffects_GiveSpecialAbility(string abilityName, StatusEffects __instance) // Prefix
		{
			if (abilityName == "Pyromancy")
			{
				if (BunnyHeader.gc.levelType == "HomeBase" && !__instance.agent.isDummy)
					return false;

				InvItem invItem = new InvItem();
				invItem.invItemName = abilityName;
				invItem.SetupDetails(false);
				invItem.invItemCount = invItem.initCount;
				invItem.agent = __instance.agent;
				invItem.invInterface = __instance.agent.mainGUI.invInterface;
				__instance.agent.inventory.equippedSpecialAbility = invItem;

				if (BunnyHeader.gc.serverPlayer)
					__instance.agent.oma.equippedSpecialAbility = __instance.agent.oma.convertSpecialAbilityToInt(abilityName);

				__instance.agent.specialAbility = abilityName;

				if (__instance.agent.isPlayer != 0 && !BunnyHeader.gc.serverPlayer && __instance.agent.localPlayer && __instance.agent.agentName == "Custom")
					__instance.agent.objectMult.CallCmdAddSpecialAbility(abilityName);

				__instance.agent.hasSpecialAbilityArm2 = true; //TODO: Verify if this is the graphic character model
				__instance.RechargeSpecialAbility(abilityName);

				return false;
			}
			return true;
		}
		public static bool StatusEffects_HeldSpecialAbility (StatusEffects __instance) // Prefix
		{
			string specialAbility = __instance.agent.specialAbility;
			Agent agent = __instance.agent;
			GameController gc = BunnyHeader.gc;

			if (specialAbility == "Pyromancy")
			{
				float castTime = 0.6f;
				//if (agent.oma.superSpecialAbility || __instance.hasTrait("TODO: Pyromancy Recharge Upgrade")) // TODO
				//	num = 0.3f;

				if (gc.playerControl.pressedSpecialAbilityTime[agent.isPlayer - 1] > castTime)
				{
					gc.audioHandler.Play(agent, "ZombieSpitFire"); // TODO
					agent.objectMult.chargingSpecialLunge = false;
					gc.playerControl.pressedSpecialAbilityTime[agent.isPlayer - 1] = 0f;
					agent.gun.spawnBullet(bulletStatus.Fireball, null, -1, true);
					gc.spawnerMain.SpawnNoise(agent.tr.position, 2f, null, null, agent);

					if (agent.isPlayer > 0 && agent.localPlayer)
						gc.ScreenBump(2f, 30, agent);

					gc.alienFX.FireGun(agent);
					gc.playerControl.Vibrate(agent.isPlayer, 0.25f, 0.2f);

					if (agent.health != 1f && !agent.oma.superSpecialAbility && !__instance.hasTrait("NoZombieSpitHealthLoss"))
					{
						float healthCost = 3f;

						if (gc.challenges.Contains("LowHealth"))
							healthCost = 2f;

						if (agent.health <= healthCost)
							healthCost = agent.health - 1f;

						agent.deathMethod = "ZombieSpit";
						__instance.ChangeHealth(-healthCost);
					}

					gc.audioHandler.Stop(agent, "ZombieSpitCharge");
					agent.tr.eulerAngles = agent.melee.tr.eulerAngles;

					return false;
				}
				if (agent.controllerType == "Keyboard" && !BunnyHeader.gc.sessionDataBig.trackpadMode)
					agent.movement.RotateToMouseOffset(agent.agentCamera.actualCamera);

				return false;
			}
			return true;
		}
		public bool StatusEffects_RechargeSpecialAbility(string abilityName, StatusEffects __instance) // Prefix
		{
			if (!__instance.startedRechargeSpecialAbility && __instance.agent.inventory.equippedSpecialAbility.invItemName == "Pyromancy")
				__instance.StartCoroutine(StatusEffects_RechargeSpecialAbility3(abilityName, __instance));
			else
				return true;
			return false;
		}
		private static IEnumerator StatusEffects_RechargeSpecialAbility3(string abilityName, StatusEffects __instance) // Non-patch
		{
			Agent agent = __instance.agent;
			GameController gc = BunnyHeader.gc;
			bool doStop = false;
			__instance.startedRechargeSpecialAbility = true;
			__instance.rechargesSpecialAbility = true;

			do
			{
				if (agent.inventory.equippedSpecialAbility != null)
				{
					InvItem myItem;

					if (agent.inventory.equippedSpecialAbility.invItemName == "Pyromancy")
					{
						myItem = agent.inventory.equippedSpecialAbility;
						float timeSinceLastShotThreshold = 2f;
						float num2 = 0.25f;
						
						if (__instance.hasTrait("TODO: PyromancyChargesFaster") || agent.oma.superSpecialAbility)
						{
							timeSinceLastShotThreshold = 1.25f;
							num2 = 0.15f;
						}
						if (myItem.invItemCount < myItem.rechargeAmountInverse)
						{
							yield return new WaitForSeconds(num2);

							if (!agent.dead && !agent.FellInHole() && !agent.teleporting && !agent.KnockedOut() && gc.loadComplete && !agent.finishedLevel && !agent.finishedLevelImmediate && !agent.finishedLevelRealMult && __instance.timeSinceLastShot > timeSinceLastShotThreshold && myItem.invItemCount < myItem.rechargeAmountInverse)
							{
								gc.mainGUI.invInterface.DirtySlots();
								myItem.invItemCount++;
							}
						}
					}
				}
			}
			while (!doStop);
			yield break;
		}
		#endregion
	}
}
