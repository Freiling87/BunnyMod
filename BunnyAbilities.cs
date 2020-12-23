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
		#region Main
		public void Awake()
		{
			InitializeAbilities();
		}
		public static void InitializeAbilities()
		{
			#region Chronomancy
			bool timeWarp = false;

			Sprite spriteChronomancy = RogueUtilities.ConvertToSprite(Properties.Resources.Chronomancy);

			CustomAbility chronomancy = RogueLibs.CreateCustomAbility("Chronomancy", spriteChronomancy, true,
				new CustomNameInfo("Chronomancy"),
				new CustomNameInfo("You can slow down time for everyone but yourself. Sometimes you accidentally do the opposite."),
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

			chronomancy.Available = true;
			chronomancy.AvailableInCharacterCreation = true;
			chronomancy.CostInCharacterCreation = 8;

			chronomancy.OnPressed = delegate (InvItem item, Agent agent)
			{
				if (RollForMiscast(agent, item.invItemCount))
					ChronomancyMiscast(agent, item.invItemCount);

				if (item.invItemCount >= 20)
				{
					item.agent.gc.audioHandler.Play(item.agent, "CantDo");
					agent.Say("I need to take a \"time out!\" Get it? But seriously, my heart will stop.");
				}
				else
				{
					item.invItemCount += ManaCost(agent);

					if (item.invItemCount >= 20)
						ChronomancyMiscast(agent, item.invItemCount);

					ChronomancyCast(agent);
				}
			};
			chronomancy.Recharge = (item, agent) =>
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

			chronomancy.RechargeInterval = (item, myAgent) =>
				item.invItemCount > 0 ? new WaitForSeconds(1f) : null;

			#endregion
			#region Cryomancy

			#endregion
			#region Electromancy

			#endregion
			#region Pyromancy

			// Check out RotateToMouseOffset
			//					this.agent.movement.RotateToMouseOffset(this.agent.agentCamera.actualCamera);

			GameController gc = BunnyHeader.gc;

			Sprite spritePyromancy = RogueUtilities.ConvertToSprite(Properties.Resources.Pyromancy);

			CustomAbility pyromancy = RogueLibs.CreateCustomAbility("Pyromancy", spritePyromancy, true,
				new CustomNameInfo("Pyromancy"),
				new CustomNameInfo("You can throw fire from your hands. This tends to fix a lot of your problems, and create much worse ones."),
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
					item.initCount = 30;
					item.itemType = "WeaponProjectile";
					item.LoadItemSprite("Fireball");
					item.rapidFire = true;
					item.rechargeAmountInverse = item.initCount;
					item.shadowOffset = 2;
					item.specialMeleeTexture = true;
					item.stackable = true;
					item.thiefCantSteal = true;
					item.weaponCode = weaponType.WeaponProjectile;
				});

			pyromancy.Available = true;
			pyromancy.AvailableInCharacterCreation = true;
			pyromancy.CostInCharacterCreation = 10;

			pyromancy.OnPressed = delegate (InvItem item, Agent agent)
			{
				if (item.invItemCount == 0)
					item.agent.gc.audioHandler.Play(item.agent, "CantDo");
				else
				{
					//agent.gun.HideGun();

					Bullet bullet = agent.gc.spawnerMain.SpawnBullet(agent.gun.tr.position, bulletStatus.Fire, agent);

					if (agent.controllerType == "Keyboard" && !gc.sessionDataBig.trackpadMode)
					{
						bullet.movement.RotateToMouseTr(agent.agentCamera.actualCamera);
					}
					else if (agent.target.AttackTowardTarget())
					{
						bullet.tr.rotation = Quaternion.Euler(0f, 0f, agent.target.transform.eulerAngles.z);
					}
					else
					{
						bullet.tr.rotation = Quaternion.Euler(0f, 0f, agent.gun.FindWeaponAngleGamepad() - 90f);
					}

					item.invItemCount--;
				}
			};

			pyromancy.Recharge = (item, myAgent) =>
			{
				if (item.invItemCount < item.rechargeAmountInverse && myAgent.statusEffects.CanRecharge())
				{
					item.invItemCount++;

					if (item.invItemCount == 0) // Recharged
					{
						myAgent.statusEffects.CreateBuffText("Recharged", myAgent.objectNetID);
						myAgent.gc.audioHandler.Play(myAgent, "Recharge");
						myAgent.inventory.buffDisplay.specialAbilitySlot.MakeUsable();
					}
				}
			};

			pyromancy.RechargeInterval = (item, myAgent) =>
				item.invItemCount > 0 ? new WaitForSeconds(1f) : null;
			#endregion
			#region Telemancy
			Sprite spriteTelemancy = RogueUtilities.ConvertToSprite(Properties.Resources.Telemancy);

			CustomAbility telemancy = RogueLibs.CreateCustomAbility("Telemancy", spriteTelemancy, true,
				new CustomNameInfo("Telemancy"),
				new CustomNameInfo("You can teleport sort of at will, but it's unpredictable and makes you feel sick. Maybe you can get better at this?"),
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

			telemancy.Available = true;
			telemancy.AvailableInCharacterCreation = true;
			telemancy.CostInCharacterCreation = 8;

			telemancy.OnPressed = delegate (InvItem item, Agent agent)
			{
				if (RollForMiscast(agent, item.invItemCount))
					TelemancyMiscast(agent);

				if (item.invItemCount >= 20)
				{
					item.agent.gc.audioHandler.Play(item.agent, "CantDo");
					agent.Say("I need to rest or my head will explode. I've seen it happen.");
				}
				else
				{
					agent.SpawnParticleEffect("Spawn", agent.curPosition);

					Vector3 targetLocation = TelemancyCast(agent, false, false, true, true, true);

					agent.Teleport(targetLocation, false, true);
					agent.rb.velocity = Vector2.zero;

					agent.SpawnParticleEffect("Spawn", agent.tr.position, false);
					GameController.gameController.audioHandler.Play(agent, "Spawn");

					item.invItemCount += ManaCost(agent);

					if (item.invItemCount >= 20)
						TelemancyMiscast(agent);
				}
			};
			telemancy.Recharge = (item, agent) =>
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

			telemancy.RechargeInterval = (item, myAgent) =>
				item.invItemCount > 0 ? new WaitForSeconds(1f) : null;
			#endregion
		}
		#endregion

		#region Magic General
		public static bool RollForMiscast(Agent agent, int modifier)
		{
			int risk = 100 + modifier;

			if (agent.statusEffects.hasTrait("FocusedCasting_2"))
				risk -= 50;
			else if (agent.statusEffects.hasTrait("FocusedCasting"))
				risk -= 25;

			if (agent.statusEffects.hasTrait("WildCasting_2"))
				risk += 150;
			else if (agent.statusEffects.hasTrait("WildCasting"))
				risk += 75;

			if (agent.statusEffects.hasTrait("MagicTraining_2"))
				risk *= (3 / 5);
			else if (agent.statusEffects.hasTrait("MagicTraining"))
				risk *= (4 / 5);

			return (UnityEngine.Random.Range(0, 10000) <= risk);
		}
		public static int ManaCost(Agent agent) // Non-Patch
		{
			int minimum = 4;
			int maximum = 8;

			if (agent.specialAbility == "Telemancy")
			{
				if (agent.statusEffects.hasTrait("MagicTraining_2"))
				{
					minimum -= 2;
					maximum -= 2;
				}
				else if (agent.statusEffects.hasTrait("MagicTraining"))
				{
					minimum -= 1;
					maximum -= 1;
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
		#region Chronomancy
		public static void ChronomancyCast(Agent agent)
		{
			int durationMin = 5;
			int durationMax = 10;

			if (agent.statusEffects.hasTrait("MagicTraining"))
			{
				durationMin += 3;
				durationMax += 3;
			}

			agent.SpawnParticleEffect("Spawn", agent.curPosition);
			GameController.gameController.audioHandler.Play(agent, "Spawn");

			agent.gc.StartCoroutine(agent.gc.SetSecondaryTimeScale(0.5f, 5f));
			agent.statusEffects.AddStatusEffect("Fast", 5);
		}
		public static void ChronomancyMiscast(Agent agent, int degree)
		{
			agent.SpawnParticleEffect("Spawn", agent.curPosition);
			agent.gc.audioHandler.Play(agent, "ToiletTeleportOut");
			switch (UnityEngine.Random.Range(1, 3))
			{
				case 1:
					agent.Say("Iii ttthhhiiinnnkkk Iii mmmeeesssssseeeddd uuuppp...");
					break;
				case 2:
					agent.Say("Bullet Time? More like Bullshit Time!");
					break;
				case 3:
					agent.Say("(Slow Motion Noises)");
					break;
			}

			agent.gc.StartCoroutine(agent.gc.SetSecondaryTimeScale(1.2f, degree));
			agent.statusEffects.AddStatusEffect("Slow", (int)(degree / 1.2f));
			agent.inventory.buffDisplay.specialAbilitySlot.MakeNotUsable();
		}
		#endregion
		#region Telemancy
		public static Vector2 TelemancyCast(Agent agent, bool accountForObstacles, bool notInside, bool dontCareAboutDanger, bool teleporting, bool accountForWalls)
		{
			TileInfo tileInfo = agent.gc.tileInfo;
			Vector2 currentPosition = agent.curPosition;
			Vector2 targetPosition;
			float rangeNear = 3f; //
			float rangeFar = 6f; //

			if (agent.statusEffects.hasTrait("MagicTraining_2"))
			{
				rangeNear -= 2f;
				rangeFar -= 2f;
			}
			if (agent.statusEffects.hasTrait("MagicTraining"))
			{
				rangeNear -= 1f;
				rangeFar -= 1f;
			}
			if (agent.statusEffects.hasTrait("WildCasting"))
				rangeNear -= 1f;
			else if (agent.statusEffects.hasTrait("WildCasting_2"))
				rangeNear -= 2f;
			else if (agent.statusEffects.hasTrait("FocusedCasting"))
				rangeFar -= 1f;
			else if (agent.statusEffects.hasTrait("FocusedCasting_2"))
				rangeFar -= 2f;

			rangeNear = rangeNear < 0 ? 0 : rangeNear;
			rangeFar = rangeFar < 0 ? 0 : rangeFar;

			for (int i = 0; i < 50; i++)
			{
				float distance = UnityEngine.Random.Range(rangeNear, rangeFar);

				if (agent.statusEffects.hasTrait("MagicTraining"))
					targetPosition = MouseIngamePosition() + distance * UnityEngine.Random.insideUnitCircle.normalized;
				else
					targetPosition = currentPosition + distance * UnityEngine.Random.insideUnitCircle.normalized;

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
		public static void TelemancyMiscast(Agent agent)
		{
			agent.gc.audioHandler.Play(agent, "ZombieSpitFire");
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

			agent.statusEffects.ChangeHealth(-agent.inventory.equippedSpecialAbility.invItemCount);
			agent.statusEffects.AddStatusEffect("Dizzy", 5);
			agent.inventory.buffDisplay.specialAbilitySlot.MakeNotUsable();
		}
		#endregion
	}
}
