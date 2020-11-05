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

			BunnyHeader.MainInstance.PatchPrefix(typeof(ObjectMult), "AttackProjectilePos", GetType(), "ObjectMult_AttackProjectilePos", new Type[6] { typeof(Vector2), typeof(bool), typeof(bool), typeof(bool), typeof(int), typeof(string)});
		}
		public static void InitializeAbilities()
		{
			#region Pyromancy
			Sprite sprite = RogueUtilities.ConvertToSprite(Properties.Resources.Fireball);

			CustomAbility pyromancy = RogueLibs.CreateCustomAbility("Pyromancy", sprite, true,
				new CustomNameInfo("Pyromancy"),
				new CustomNameInfo("You can throw fireballs from your hands. This tends to fix a lot of your problems, and create much worse ones."),
				delegate (InvItem item)
				{
					item.cantDrop = true;
					item.Categories.Add("Weapons");
					item.Categories.Add("NPCsCantPickUp");
					item.dontAutomaticallySelect = true;
					item.dontSelectNPC = true;
					item.gunKnockback = 0;
					item.isWeapon = true;
					item.rapidFire = false;
					item.initCount = 15;
					item.initCountAI = 15;
					item.itemType = "WeaponProjectile";
					item.LoadItemSprite("Fireball");
					item.rechargeAmountInverse = item.initCount;
					item.shadowOffset = 2;
					item.specialMeleeTexture = true;
					item.thiefCantSteal = true;
					item.weaponCode = weaponType.WeaponProjectile;
				});
			pyromancy.Available = true;
			pyromancy.AvailableInCharacterCreation = true;
			pyromancy.CostInCharacterCreation = 10;
			pyromancy.OnPressed = delegate (InvItem item, Agent agent)
			{
				if (item.invItemCount > 0)
					item.agent.gc.audioHandler.Play(item.agent, "CantDo");
				else
				{

				}
			};

			pyromancy.Recharge = (item, myAgent) =>
			{
				if (item.invItemCount > 0 && myAgent.statusEffects.CanRecharge())
				{
					item.invItemCount--;

					if (item.invItemCount == 0) // ability recharged
					{
						myAgent.statusEffects.CreateBuffText("Recharged", myAgent.objectNetID);
						myAgent.gc.audioHandler.Play(myAgent, "Recharge");
						myAgent.inventory.buffDisplay.specialAbilitySlot.MakeUsable();
						// make special ability slot fully visible again
					}
				}
			};

			pyromancy.RechargeInterval = (item, myAgent) => 
				item.invItemCount > 0 ? new WaitForSeconds(1f) : null;
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
	}
}
