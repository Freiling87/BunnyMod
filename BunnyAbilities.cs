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
		}
		public static void InitializeAbilities()
		{
			#region pyromancy
			Sprite sprite = RogueUtilities.ConvertToSprite(Properties.Resources.Fireball);

			CustomAbility pyromancy = RogueLibs.CreateCustomAbility("pyromancy", sprite, true,
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
		//CheckAttack
		//Shoot
		//SpawnBullet
		#endregion
		#region ObjectMult
		//AttackProjectileObject
		public static bool AttackProjectilePos(Vector2 aimPosition, bool specialAbility, bool silenced, bool rubber, int bulletNetID, string bulletStatusEffect, ObjectMult __instance) // Prefix
		{
			if (specialAbility && __instance.agent.statusEffects.hasSpecialAbility("pyromancy"))
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
		//AttackProjectilePosMore
		//CmdAttackProjectileObject
		//CallCmdAttackProjectileObject
		//MultAttackProjectile ***
		//SpecialAbility
		#endregion
		#region objectMultAgent
		//ConvertIntToSpecialAbility
		//ConvertSpecialAbilityToInt
		#endregion
		#region StatusEffects
		//MoveNext
		public static bool HeldSpecialAbility (StatusEffects __instance, ref GameController ___gc) // Prefix
		{
			Agent agent = __instance.agent;
			string specialAbility = agent.specialAbility;

			//TODO: Go through this one, I've barely looked at it.

			if (!agent.ghost && !agent.teleporting && specialAbility == "pyromancy")
			{
				float num = 0.6f;

				if (agent.oma.superSpecialAbility || __instance.hasTrait("FasterZombieSpitCharge"))
					num = 0.3f;

				if (___gc.playerControl.pressedSpecialAbilityTime[agent.isPlayer - 1] > num)
				{
					___gc.audioHandler.Play(agent, "ZombieSpitFire");
					agent.objectMult.chargingSpecialLunge = false;
					___gc.playerControl.pressedSpecialAbilityTime[agent.isPlayer - 1] = 0f;
					agent.gun.spawnBullet(bulletStatus.ZombieSpit, null, -1, true);
					___gc.spawnerMain.SpawnNoise(agent.tr.position, 2f, null, null, agent);
					if (agent.isPlayer > 0 && agent.localPlayer)
					{
						___gc.ScreenBump(2f, 30, agent);
					}
					___gc.alienFX.FireGun(agent);
					___gc.playerControl.Vibrate(agent.isPlayer, 0.25f, 0.2f);
					if (agent.health != 1f && !agent.oma.superSpecialAbility && !__instance.hasTrait("NoZombieSpitHealthLoss"))
					{
						float num2 = 3f;
						if (___gc.challenges.Contains("LowHealth"))
						{
							num2 = 2f;
						}
						if (agent.health <= num2)
						{
							num2 = agent.health - 1f;
						}
						agent.deathMethod = "ZombieSpit";
						__instance.ChangeHealth(-num2);
					}
					___gc.audioHandler.Stop(agent, "ZombieSpitCharge");
					agent.tr.eulerAngles = agent.melee.tr.eulerAngles;
					return false;
				}
				if (agent.controllerType == "Keyboard" && !___gc.sessionDataBig.trackpadMode)
				{
					agent.movement.RotateToMouseOffset(agent.agentCamera.actualCamera);

					return false;
				}
			}
			return true;
		}
		public static bool PressedSpecialAbility(StatusEffects __instance) // Prefix
		{
			Agent agent = __instance.agent;
			string specialAbility = agent.specialAbility;

			if (!agent.ghost && !agent.teleporting && specialAbility == "pyromancy")
			{
				agent.objectMult.chargingSpecialLunge = true;
				agent.gun.HideGun();
				agent.gc.audioHandler.Play(agent, "LungePrepare");
				return false;
			}
			return true;
		}
		public static bool ReleasedSpecialAbility(StatusEffects __instance) // Prefix
		{
			Agent agent = __instance.agent;
			string specialAbility = agent.specialAbility;
			if (!agent.ghost && specialAbility == "pyromancy")
			{
				if (agent.objectMult.chargingSpecialLunge)
				{
					agent.objectMult.chargingSpecialLunge = false;
					agent.gc.audioHandler.FadeOnClients(agent, "SpitFireball");
					agent.tr.eulerAngles = agent.melee.tr.eulerAngles;
				}
				return false;
			}
			return true;
		}
		#endregion
	}
}
