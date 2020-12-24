﻿using System;
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
		#region Main
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
				new CustomNameInfo("You can teleport a short distance at will, but it's unpredictable and if you overdo it, you'll have a little aneurysm. Maybe you can get better at this?"),
				delegate (InvItem item)
				{
					item.cantDrop = true;
					item.Categories.Add("Usable"); //
					item.Categories.Add("NPCsCantPickup");
					item.dontAutomaticallySelect = true;
					item.dontSelectNPC = true;
					item.isWeapon = false;
					item.initCount = 0;
					item.itemType = ""; //
					item.rechargeAmountInverse = item.initCount;
					item.stackable = true;
					item.thiefCantSteal = true;
				});

			blink.Available = true;
			blink.AvailableInCharacterCreation = true;
			blink.CostInCharacterCreation = 7;

			blink.OnPressed = delegate (InvItem item, Agent agent)
			{
				if (RollForMiscast(agent, item.invItemCount))
				{
					BlinkMiscast(agent);
				}

				if (item.invItemCount >= 20)
				{
					item.agent.gc.audioHandler.Play(item.agent, "CantDo");
					agent.Say("I need to rest or my head will explode. I've seen it happen.");
				}
				else
				{
					agent.SpawnParticleEffect("Spawn", agent.curPosition);

					Vector3 targetLocation = BlinkRandomLocation(agent, false, false, true, true, true);

					agent.Teleport(targetLocation, false, true); 
					agent.rb.velocity = Vector2.zero;

					agent.SpawnParticleEffect("Spawn", agent.tr.position, false);
					GameController.gameController.audioHandler.Play(agent, "Spawn");

					item.invItemCount += ManaCost(agent);

					if (item.invItemCount >= 20)
						BlinkMiscast(agent);
				}
			};
			blink.Recharge = (item, agent) =>
			{
				if (item.invItemCount > 0 && agent.statusEffects.CanRecharge())
				{
					item.invItemCount--;

					if (item.invItemCount == 20)
					{
						agent.statusEffects.CreateBuffText("Recharged", agent.objectNetID);
						agent.gc.audioHandler.Play(agent, "Recharge");
						agent.inventory.buffDisplay.specialAbilitySlot.MakeUsable();
					}
				}
			};

			blink.RechargeInterval = (item, myAgent) =>
				item.invItemCount > 0 ? new WaitForSeconds(1f) : null;
			#endregion
			#region Cryomancy

			#endregion
			#region Pyromancy

			//// Check out RotateToMouseOffset
			////					this.agent.movement.RotateToMouseOffset(this.agent.agentCamera.actualCamera);


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

		#region Custom - Magic General
		public static bool RollForMiscast(Agent agent, int modifier)
		{
			int risk = 100 + modifier;

			if (agent.statusEffects.hasTrait("FocusedCasting_2"))
				risk /= 10;
			else if (agent.statusEffects.hasTrait("FocusedCasting"))
				risk /= 5;

			if (agent.statusEffects.hasTrait("WildCasting_2"))
				risk *= 5;
			else if (agent.statusEffects.hasTrait("WildCasting"))
				risk *= 5 / 2;

			return (UnityEngine.Random.Range(0, 10000) <= risk);
		}
		#endregion
		#region Custom - Blink
		public static void BlinkMiscast (Agent agent)
		{
			agent.inventory.buffDisplay.specialAbilitySlot.MakeNotUsable();
			switch (UnityEngine.Random.Range(1, 4))
			{
				case 1:
					agent.Say("I smell burning toast.");
					break;
				case 2:
					agent.Say("Blurgh. (Drool)");
					break;
				case 3:
					agent.Say("I pink I bust hab a stroke.");
					break;
				case 4:
					agent.Say("My head a splode.");
					break;
			}
			agent.gc.audioHandler.Play(agent, "ZombieSpitFire");
			agent.statusEffects.ChangeHealth(-agent.inventory.equippedSpecialAbility.invItemCount);
			agent.statusEffects.AddStatusEffect("Dizzy", 5);
		}
		public static Vector2 BlinkRandomLocation(Agent agent, bool accountForObstacles, bool notInside, bool dontCareAboutDanger, bool teleporting, bool accountForWalls) // Non-Patch
		{
			TileInfo tileInfo = agent.gc.tileInfo;
			Vector2 currentPosition = agent.curPosition;
			Vector2 targetPosition;
			float rangeNear = 2f; //
			float rangeFar = 5f; //

			if (agent.statusEffects.hasTrait("BlinkTraining_2"))
			{
				rangeNear = 0f;
				rangeFar = 0f;
			}

			if (agent.statusEffects.hasTrait("BlinkTraining"))
			{
				rangeNear = 1f;
				rangeFar = 3f;


				//TileData tileData = agent.gc.tileInfo.GetTileData(MouseIngamePosition()); This needs to point to the randomized location, not the mouse position

			}

			for (int i = 0; i < 50; i++)
			{
				if (agent.statusEffects.hasTrait("MagicTraining") || agent.statusEffects.hasTrait("MagicTraining_2"))
				{
					float distance = UnityEngine.Random.Range(rangeNear, rangeFar);
					targetPosition = MouseIngamePosition() + distance * UnityEngine.Random.insideUnitCircle.normalized;
				}
				else
				{
					float distance = UnityEngine.Random.Range(rangeNear, rangeFar);
					targetPosition = currentPosition + distance * UnityEngine.Random.insideUnitCircle.normalized;
				}

				TileData tileData = tileInfo.GetTileData(targetPosition);
				
				if (tileData.solidObject)
					continue;
				else if (tileData.dangerousToWalk && !dontCareAboutDanger && !tileData.spillOoze) // Consider allowing Ooze, for balance
					continue;
				else if (tileInfo.WallExist(tileData) && (accountForObstacles || accountForWalls))
					continue;
				else if (tileInfo.IsOverlapping(targetPosition, "Anything") && accountForObstacles) // Currently always false, but enable this if you're getting stuck on objects. Although that might be fun.
					continue;

				else if (!accountForObstacles)
					if (tileInfo.GetWallMaterial(targetPosition.x, targetPosition.y) == wallMaterialType.Border) // Removed Conveyor, Water, Hole
						continue;

				else if (teleporting && accountForObstacles && tileInfo.IsOverlapping(targetPosition, "Anything", 0.32f))
					continue;

				if (notInside && (tileInfo.IsIndoors(targetPosition) || tileData.owner == 55 || (tileData.floorMaterial == floorMaterialType.ClearFloor && tileData.owner != 0)))
					continue;

				return targetPosition;
			}
			return currentPosition;
		}
		public static int ManaCost(Agent agent) // Non-Patch
		{
			int minimum = 4;
			int maximum = 8;

			if (agent.specialAbility == "Blink")
			{
				if (agent.statusEffects.hasTrait("BlinkTraining"))
				{
					minimum -= 1;
					maximum -= 1;
				}
				if (agent.statusEffects.hasTrait("BlinkTraining_2"))
				{
					minimum -= 2;
					maximum -= 2;
				}
			}

			int roll = UnityEngine.Random.Range(minimum, maximum);

			if (agent.statusEffects.hasTrait("StrongGagReflex"))
				roll -= 1;
			else if (agent.statusEffects.hasTrait("StrongGagReflex_2"))
				roll -= 2;

			if (roll > maximum)
				roll = maximum;
			if (roll < minimum)
				roll = minimum;

			return roll;
		}
		public static Vector2 MouseIngamePosition()
		{
			Plane plane = new Plane(new Vector3(0, 0, 1), new Vector3(0, 0, 0));
			Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
			return plane.Raycast(ray, out float enter) ? (Vector2)ray.GetPoint(enter) : default;
		}
		#endregion
	}
}
