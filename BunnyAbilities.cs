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
		#region Generic
		public void Awake()
		{
			InitializeAbilities();

			//BunnyHeader.MainInstance.PatchPrefix(typeof(Gun), "Shoot", GetType(), "Gun_Shoot", new Type[5] { typeof(bool), typeof(bool), typeof(bool), typeof(int), typeof(string) });

			//BunnyHeader.MainInstance.PatchPrefix(typeof(ObjectMult), "AttackProjectilePos", GetType(), "ObjectMult_AttackProjectilePos", new Type[6] { typeof(Vector2), typeof(bool), typeof(bool), typeof(bool), typeof(int), typeof(string)});
		}
		public static void InitializeAbilities()
		{
			#region Blink
			Sprite spriteBlink = RogueUtilities.ConvertToSprite(Properties.Resources.Blink);

			CustomAbility blink = RogueLibs.CreateCustomAbility("Blink", spriteBlink, true,
				new CustomNameInfo("Blink"),
				new CustomNameInfo("Almost at will, you can instantly teleport to a nearby location. Whether this is better or worse than where you were before remains to be seen. It also recharges at an inconsistent rate. Maybe you can get better at this?"),
				delegate (InvItem item)
				{
					item.cantDrop = true;
					item.Categories.Add("Usable"); //
					item.Categories.Add("NPCsCantPickup");
					item.dontAutomaticallySelect = true;
					item.dontSelectNPC = true;
					item.isWeapon = false;
					item.initCount = 5;
					item.itemType = ""; //
					item.rechargeAmountInverse = item.initCount;
					item.stackable = true;
					item.thiefCantSteal = true;
				});

			blink.Available = true;
			blink.AvailableInCharacterCreation = true;
			blink.CostInCharacterCreation = 10;

			blink.OnPressed = delegate (InvItem item, Agent agent)
			{
				if (item.invItemCount > 0)
					item.agent.gc.audioHandler.Play(item.agent, "CantDo");
				else
				{
					agent.SpawnParticleEffect("Spawn", agent.curPosition);

					Vector3 targetLocation = BlinkRandomLocation(agent.gc.tileInfo, agent.curPosition, agent, 2f, 5f, false, false, true, true, true);

					agent.Teleport(targetLocation, false, true); 
					agent.rb.velocity = Vector2.zero;

					agent.SpawnParticleEffect("Spawn", agent.tr.position, false);
					GameController.gameController.audioHandler.Play(agent, "Spawn");

					item.invItemCount += BlinkNausea(agent);

					if (item.invItemCount >= 10)
					{
						// Apply Dizzy and/or Damage here
					}
				}
			};
			blink.Recharge = (item, myAgent) =>
			{
				if (item.invItemCount > 0 && myAgent.statusEffects.CanRecharge())
				{
					item.invItemCount--;

					if (item.invItemCount == 0) // Recharged
					{
						myAgent.statusEffects.CreateBuffText("Recharged", myAgent.objectNetID);
						myAgent.gc.audioHandler.Play(myAgent, "Recharge");
						myAgent.inventory.buffDisplay.specialAbilitySlot.MakeUsable(); // Remove possibly
					}
				}
			};

			blink.RechargeInterval = (item, myAgent) =>
				item.invItemCount > 0 ? new WaitForSeconds(1f) : null;
			#endregion
			#region Cryomancy

			#endregion
			#region Pyromancy
			//GameController gc = BunnyHeader.gc;

			//Sprite spritePyromancy = RogueUtilities.ConvertToSprite(Properties.Resources.Fireball);

			//CustomAbility pyromancy = RogueLibs.CreateCustomAbility("Pyromancy", spritePyromancy, true,
			//	new CustomNameInfo("Pyromancy"),
			//	new CustomNameInfo("You can throw fireballs from your hands. This tends to fix a lot of your problems, and create much worse ones."),
			//	delegate (InvItem item)
			//	{
			//		item.cantDrop = true;
			//		item.Categories.Add("Weapons");
			//		item.Categories.Add("NPCsCantPickUp");
			//		item.dontAutomaticallySelect = true;
			//		item.dontSelectNPC = true;
			//		item.gunKnockback = 0;
			//		item.isWeapon = true;
			//		item.rapidFire = false;
			//		item.initCount = 30;
			//		item.itemType = "WeaponProjectile";
			//		item.LoadItemSprite("Fireball");
			//		item.rapidFire = true;
			//		item.rechargeAmountInverse = item.initCount;
			//		item.shadowOffset = 2;
			//		item.specialMeleeTexture = true;
			//		item.stackable = true;
			//		item.thiefCantSteal = true;
			//		item.weaponCode = weaponType.WeaponProjectile;
			//	});

			//pyromancy.Available = true;
			//pyromancy.AvailableInCharacterCreation = true;
			//pyromancy.CostInCharacterCreation = 10;

			//pyromancy.OnPressed = delegate (InvItem item, Agent agent)
			//{
			//	if (item.invItemCount == 0)
			//		item.agent.gc.audioHandler.Play(item.agent, "CantDo");
			//	else
			//	{
			//		//agent.gun.HideGun();

			//		Bullet bullet = agent.gc.spawnerMain.SpawnBullet(agent.gun.tr.position, bulletStatus.Fire, agent);

			//		if (agent.controllerType == "Keyboard" && !gc.sessionDataBig.trackpadMode)
			//		{
			//			bullet.movement.RotateToMouseTr(agent.agentCamera.actualCamera);
			//		}
			//		else if (agent.target.AttackTowardTarget())
			//		{
			//			bullet.tr.rotation = Quaternion.Euler(0f, 0f, agent.target.transform.eulerAngles.z);
			//		}
			//		else
			//		{
			//			bullet.tr.rotation = Quaternion.Euler(0f, 0f, agent.gun.FindWeaponAngleGamepad() - 90f);
			//		}

			//		item.invItemCount--;
			//	}
			//};

			//pyromancy.Recharge = (item, myAgent) =>
			//{
			//	if (item.invItemCount < item.rechargeAmountInverse && myAgent.statusEffects.CanRecharge())
			//	{
			//		item.invItemCount++;

			//		if (item.invItemCount == 0) // Recharged
			//		{
			//			myAgent.statusEffects.CreateBuffText("Recharged", myAgent.objectNetID);
			//			myAgent.gc.audioHandler.Play(myAgent, "Recharge");
			//			myAgent.inventory.buffDisplay.specialAbilitySlot.MakeUsable();
			//		}
			//	}
			//};

			//pyromancy.RechargeInterval = (item, myAgent) => 
			//	item.invItemCount > 0 ? new WaitForSeconds(1f) : null;
			#endregion
		}
		#endregion

		#region Gun
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
		public static Vector2 BlinkRandomLocation(TileInfo tileInfo, Vector2 pos, Agent agent, float rangeNear, float rangeFar, bool accountForObstacles, bool notInside, bool dontCareAboutDanger, bool teleporting, bool accountForWalls) // Non-Patch
		{
			for (int i = 0; i < 50; i++)
			{
				float distance = UnityEngine.Random.Range(rangeNear, rangeFar);
				Vector2 vector = pos + distance * UnityEngine.Random.insideUnitCircle.normalized;

				TileData tileData = tileInfo.GetTileData(vector);
				
				if (tileData.solidObject)
					continue;
				else if (tileData.dangerousToWalk && !dontCareAboutDanger && !tileData.spillOoze) // Consider allowing Ooze, for balance
					continue;
				else if (tileInfo.WallExist(tileData) && (accountForObstacles || accountForWalls))
					continue;
				else if (tileInfo.IsOverlapping(vector, "Anything") && accountForObstacles) // Currently always false, but enable this if you're getting stuck on objects. Although that might be fun.
					continue;

				else if (!accountForObstacles)
					if (tileInfo.GetWallMaterial(vector.x, vector.y) == wallMaterialType.Border) // Removed Conveyor, Water, Hole
						continue;

				else if (teleporting && accountForObstacles && tileInfo.IsOverlapping(vector, "Anything", 0.32f))
					continue;

				if (notInside && (tileInfo.IsIndoors(vector) || tileData.owner == 55 || (tileData.floorMaterial == floorMaterialType.ClearFloor && tileData.owner != 0)))
					continue;

				return vector;
			}
			return pos;
		}
		public static int BlinkNausea(Agent agent) // Non-Patch
		{
			int minimum = 1;
			int maximum = 5;

			// Consider an Agent Remora to store variables rather than conditional checks every time

			return UnityEngine.Random.Range(minimum, maximum);
		}
	}
}
